using System;
using System.Data.Entity;
using System.Linq;
using AutoMapper;
using AutoMapper.Mappers;
using CodeVision.Dependencies.SqlStorage;
using Newtonsoft.Json;

namespace CodeVision.Dependencies.Database
{
    public class DatabaseObjectsGraphRepository
    {
        private readonly string _connectionString;
        private readonly MappingEngine _engine;

        public DatabaseObjectsGraphRepository(string connectionString)
        {
            _connectionString = connectionString;

            var store = new ConfigurationStore(new TypeMapFactory(), MapperRegistry.Mappers);
            store.AssertConfigurationIsValid();
            _engine = new MappingEngine(store);
            CreateMaps(store);
        }

        // SaveState wipes out graph but keeps DatabaseObjects because:
        //  TODO: We could try updating modified vertices only.
        public void SaveState(DatabaseObjectsGraph dg)
        {
            DatabaseObject[] symbolTable = dg.CreateMemento().State;
            int[][] jaggedArray = dg.Digraph.CreateMemento().State;

            // Save
            using (var ctx = new DependencyGraphContext(_connectionString))
            {
                ctx.Database.ExecuteSqlCommand("TRUNCATE TABLE DatabaseObjectsGraph;");

                foreach (var databaseObject in symbolTable)
                {
                    switch (databaseObject.ObjectState)
                    {
                        case ObjectState.VertexAdded:
                            var sqlStorageDatabaseObject = _engine.Map<SqlStorage.DatabaseObject>(databaseObject);
                            ctx.DatabaseObjects.Add(sqlStorageDatabaseObject);
                            break;
                        case ObjectState.PropertiesModified:
                            // Properties can be addedd, deleted or change values
                            var objectInDatabase = ctx.DatabaseObjects.SingleOrDefault(f => f.FullyQualifiedName == databaseObject.FullyQualifiedName);
                            if (objectInDatabase == null)
                            {
                                // We can't add a property to an object that hasn't been saved yet.
                                throw new ArgumentException(nameof(objectInDatabase));
                            }
                            objectInDatabase.Properties.Clear();
                            foreach (var property in databaseObject.Properties)
                            {
                                objectInDatabase.Properties.Add(DatabaseObjectConverter.Convert(property));
                            }
                            ctx.Entry(objectInDatabase).State = EntityState.Modified;
                            break;
                    }
                }

                for (int i = 0; i < jaggedArray.Length; i++)
                {
                    var json = JsonConvert.SerializeObject(jaggedArray[i]);
                    ctx.DatabaseObjectVertices.Add(new DatabaseObjectVertex() {  VertexId = i, AdjacencyListJson = json });
                }
                ctx.SaveChanges();
            }
        }

        // Used by UI, returns fully initialized DatabaseObjectsGraph
        public DatabaseObjectsGraph LoadState()
        {
            int[][] jaggedArray;
            DatabaseObject[] symbolTable;

            // Load
            using (var ctx = new DependencyGraphContext(_connectionString))
            {
                var vertices = ctx.DatabaseObjectVertices.ToList();
                jaggedArray = new int[vertices.Count()][];                
                for (int i = 0; i < jaggedArray.Length; i++)
                {
                    var json = vertices[i].AdjacencyListJson;
                    int[] adjencencyList = JsonConvert.DeserializeObject<int[]>(json);
                    jaggedArray[i] = adjencencyList;                    
                }

                symbolTable = GetDatabaseObjectsInternal(ctx);
            }
            
            var g = new Digraph(new Memento<int[][]>(jaggedArray));
            var dg = new DatabaseObjectsGraph(new Memento<DatabaseObject[]>(symbolTable), g);
            return dg;
        }

        // This may be helpful if only need DatabaseObjects and not graph.
        public DatabaseObject[] GetDatabaseObjects()
        {
            using (var ctx = new DependencyGraphContext(_connectionString))
            {
                return GetDatabaseObjectsInternal(ctx);
            }
        }

        private DatabaseObject[] GetDatabaseObjectsInternal(DependencyGraphContext context)
        {
            var items = context.DatabaseObjects
                .ToList() // Materialize objects
                .Select(s => _engine.Map<Dependencies.Database.DatabaseObject>(s))
                .ToArray();
            return items;
        }

        private void CreateMaps(ConfigurationStore store)
        {
            store.CreateMap<SqlStorage.DatabaseObject, DatabaseObject>().ConvertUsing<SqlStorageDatabaseObjectConverter>();
            store.CreateMap<DatabaseObject, SqlStorage.DatabaseObject>().ConvertUsing<DatabaseObjectConverter>();
        }
    }

    internal class SqlStorageDatabaseObjectConverter : ITypeConverter<SqlStorage.DatabaseObject, DatabaseObject>
    {
        public DatabaseObject Convert(ResolutionContext context)
        {
            var source = context.SourceValue as SqlStorage.DatabaseObject;
            if (source == null)
            {
                throw new ArgumentException(nameof(context));
            }

            var databaseObject = new DatabaseObject(source.ObjectType, source.FullyQualifiedName);

            foreach (var objectProperty in source.Properties)
            {
                switch (objectProperty.PropertyType)
                {
                    case DatabaseObjectPropertyType.RelevantToFinancialReporting:
                        databaseObject.Properties.Add(new RelevantToFinancialReportingProperty());
                        break;
                    case DatabaseObjectPropertyType.Comment:
                        databaseObject.Properties.Add(new CommentProperty(objectProperty.PropertyValue));
                        break;
                }
            }
            return databaseObject;
        }
    }

    internal class DatabaseObjectConverter : ITypeConverter<DatabaseObject, SqlStorage.DatabaseObject>
    {
        public SqlStorage.DatabaseObject Convert(ResolutionContext context)
        {
            var source = context.SourceValue as DatabaseObject;
            if (source == null)
            {
                throw new ArgumentException(nameof(context));
            }

            if (!source.Id.HasValue)
            {
                throw new ArgumentNullException(nameof(source.Id));
            }
            var databaseObject = new SqlStorage.DatabaseObject {ObjectType = source.ObjectType, FullyQualifiedName = source.FullyQualifiedName, DatabaseObjectId =  source.Id.Value};

            foreach (var objectProperty in source.Properties)
            {
                var targetProperty = DatabaseObjectConverter.Convert(objectProperty);
                if (targetProperty != null)
                {
                    databaseObject.Properties.Add(targetProperty);
                }
            }
            return databaseObject;
        }

        public static SqlStorage.DatabaseObjectProperty Convert(ObjectProperty source)
        {
            RelevantToFinancialReportingProperty relevantProperty = source as RelevantToFinancialReportingProperty;
            if (relevantProperty != null)
            {
                return new DatabaseObjectProperty { PropertyType = DatabaseObjectPropertyType.RelevantToFinancialReporting };
            }

            CommentProperty commentProperty = source as CommentProperty;
            if (commentProperty != null)
            {
                return new DatabaseObjectProperty { PropertyType = DatabaseObjectPropertyType.Comment, PropertyValue = commentProperty.Text };
            }

            return null;
        }
    }
}