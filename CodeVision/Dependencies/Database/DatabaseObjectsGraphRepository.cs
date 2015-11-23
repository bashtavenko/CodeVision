using System;
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

        // SaveState wipes out graph but keeps DatabaseObjects.
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
                    // TODO: Use DatabaseObject change tracking to figure out what objects to add, delete or update.
                    ctx.DatabaseObjects.Add(_engine.Map<SqlStorage.DatabaseObject>(databaseObject));
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
                jaggedArray = new int[ctx.DatabaseObjectVertices.Count()][];                
                for (int i = 0; i < jaggedArray.Length; i++)
                {
                    var json = ctx.DatabaseObjectVertices.Find(i).AdjacencyListJson;
                    int[] adjencencyList = JsonConvert.DeserializeObject<int[]>(json);
                    jaggedArray[i] = adjencencyList;                    
                }

                symbolTable = GetDatabaseObjectsInternal(ctx);
            }
            
            var g = new Digraph(new Memento<int[][]>(jaggedArray));
            var dg = new DatabaseObjectsGraph(new Memento<DatabaseObject[]>(symbolTable), g);
            return dg;
        }

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

            var databaseObject = new SqlStorage.DatabaseObject {ObjectType = source.ObjectType, FullyQualifiedName = source.FullyQualifiedName};

            foreach (var objectProperty in source.Properties)
            {
                RelevantToFinancialReportingProperty relevantProperty = objectProperty as RelevantToFinancialReportingProperty;
                if (relevantProperty != null)
                {
                    databaseObject.Properties.Add(new DatabaseObjectProperty {PropertyType = DatabaseObjectPropertyType.RelevantToFinancialReporting});
                }

                CommentProperty commentProperty = objectProperty as CommentProperty;
                if (commentProperty != null)
                {
                    databaseObject.Properties.Add(new DatabaseObjectProperty { PropertyType = DatabaseObjectPropertyType.Comment, PropertyValue = commentProperty.Text});
                }
            }
            return databaseObject;
        }
    }
}