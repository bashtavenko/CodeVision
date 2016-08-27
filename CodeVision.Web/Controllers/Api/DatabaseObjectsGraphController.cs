using System;
using System.Collections.Generic;
using System.Web.Http;
using AutoMapper;
using CodeVision.Dependencies;
using CodeVision.Dependencies.Database;
using System.Net;
using System.Net.Http;
using CodeVision.Dependencies.SqlStorage;
using CodeVision.Web.Common;
using System.Linq;
using CodeVision.Web.ViewModels;

namespace CodeVision.Web.Controllers.Api
{
    public class DatabaseObjectsGraphController : ApiController
    {
        private DatabaseObjectsGraph _graph;
        private readonly DatabaseObjectsGraphRepository _repository;

        public DatabaseObjectsGraphController(DatabaseObjectsGraph graph, DatabaseObjectsGraphRepository repository)
        {
            _graph = graph;
            _repository = repository;
        }
        
        [Route("api/objects/{onlyTheseTypes?}")]
        public IList<ViewModels.DatabaseObject> Get(string name, string onlyTheseTypes = null)
        {
            DatabaseObjectType[] onlyTheseTypesArray = Converters.FromCommaSeparatedListToEnumArray<DatabaseObjectType>(onlyTheseTypes);
            var items = _graph.GetDatabaseObjectsBeginsWith(name, onlyTheseTypesArray);
            return Mapper.Map<List<ViewModels.DatabaseObject>>(items);
        }

        [Route("api/object/{objectId}", Name = "GetObjectById")]
        public ViewModels.DatabaseObject Get(int objectId)
        {
            var item = _graph.GetDatabaseObject(objectId);
            return Mapper.Map<ViewModels.DatabaseObject>(item);
        }

        [Route("api/properties")]
        public List<ViewModels.DatabaseObjectProperty> Get()
        {
            var list = Enum.GetValues(typeof (DatabaseObjectPropertyType)).Cast<DatabaseObjectPropertyType>().ToList();
            return Mapper.Map<List<ViewModels.DatabaseObjectProperty>>(list);
        }

        [Route("api/sproc/{objectId}", Name = "GetStoredProcedure")]
        public ViewModels.StoredProcedure GetStoredProcedure(int objectId)
        {
            List<Dependencies.Database.DatabaseObject> columns = _graph.GetColumnsForStoredProcedure(objectId);
            List<ViewModels.DatabaseObject> modelColumns = Mapper.Map<List<ViewModels.DatabaseObject>>(columns);
            return new ViewModels.StoredProcedure(modelColumns);
        }

        [Route("api/object/{objectId}/property")]
        public HttpResponseMessage Post(int objectId, ViewModels.DatabaseObjectProperty viewModelObjectProperty)
        {
            try
            {
                var objectProperty = Mapper.Map<ObjectProperty>(viewModelObjectProperty);
                _graph.AddProperty(objectId, objectProperty);

                // Return HTTP 201 Created with Location http://localhost:3500/api/object/4 and object in the body
                var item = Get(objectId);
                var response = Request.CreateResponse<ViewModels.DatabaseObject>(HttpStatusCode.Created, item);
                string uri = Url.Link("GetObjectById", new { objectId = objectId });
                response.Headers.Location = new Uri(uri);
                return response;
            }
            catch (ArgumentException)
            {
                // .. or HTTP 409 if dup
                var resp = new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent($"Property {viewModelObjectProperty.PropertyType} already exists."),
                    ReasonPhrase = "Duplicate property"
                };
                throw new HttpResponseException(resp);
            }
        }

        [Route("api/sproc/{objectId}/columns/{columnId}")]
        public HttpResponseMessage Post(int objectId, int columnId)
        {
            _graph.AddDependency(objectId, columnId);
            var item = GetStoredProcedure(objectId);
            var response = Request.CreateResponse<ViewModels.StoredProcedure>(HttpStatusCode.Created, item);
            string uri = Url.Link("GetStoredProcedure", new { objectId = objectId });
            response.Headers.Location = new Uri(uri);
            return response;
        }

        [HttpPut]
        [Route("api/object/{objectId}/{dependentObjectId}")]
        // This is PUT and not POST because we don't know what kind of dependencies to return
        public void AddDependentObject(int objectId, int dependentObjectId)
        {
            _graph.AddDependency(objectId, dependentObjectId);
        }

        [Route("api/object/{objectId}/{dependentObjectId}")]
        public void DeleteEdge(int objectId, int dependentObjectId)
        {
            _graph.RemoveEdge(objectId, dependentObjectId);
        }

        [Route("api/object/{objectId}/property/{propertyType}")]
        // Return DatabaseObject to make easier for UI to show updated state
        public ViewModels.DatabaseObject Delete(int objectId, DatabaseObjectPropertyType propertyType)
        {
            var viewModelPropertyType = new ViewModels.DatabaseObjectProperty {PropertyType = propertyType}; // Don't care about the value
            var domainProperty = Mapper.Map<ObjectProperty>(viewModelPropertyType);
            _graph.RemoveProperty(objectId, domainProperty);
            return Get(objectId);
        }

        [Route("api/sproc/{objectId}/column/{columnId}")]
        public ViewModels.StoredProcedure Delete(int objectId, int columnId)
        {
            _graph.RemoveColumn(objectId, columnId);
            return GetStoredProcedure(objectId);
        }

        [Route("api/object/{objectId}/comment")]
        public void Put(int objectId, [FromBody]string text)
        {
            // Binds to quoted string ("sometext") in body
            _graph.UpdatedCommentText(objectId, text);
        }

        [Route("api/objects/{objectId}/{direction}/{level}/{objectsType?}")]
        public IList<ViewModels.DatabaseObject> Get(int objectId, DependencyDirection direction, DependencyLevel level, DatabaseObjectType? objectsType = null)
        {
            var items = _graph.GetDependencies(objectId, direction, level, objectsType);
            return Mapper.Map<List<ViewModels.DatabaseObject>>(items);
        }

        [Route("api/objects/save")]
        public void Put()
        {
            _repository.SaveState(_graph);
        }

        [Route("api/objects/load")]
        public void PutLoad()
        {
            // TODO: this isn't very helpful because DI singleton graph is still in memory
            _graph = _repository.LoadState();
        }
    }
}