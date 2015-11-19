using System;
using System.Data.SqlClient;
using AutoMapper;
using AutoMapper.Mappers;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.Linq;

namespace CodeVision.Dependencies.Database
{
    public class DatabaseCollector : IDisposable
    {
        private readonly Server _server;
        private readonly ServerConnection _connection;
        private readonly MappingEngine _engine;

        public DatabaseCollector(string connectionString)
        {
            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder {ConnectionString = connectionString};
            _connection = new ServerConnection(sqlConnectionStringBuilder.DataSource);
            if (!sqlConnectionStringBuilder.IntegratedSecurity)
            {
                _connection.LoginSecure = false;
                _connection.Login = sqlConnectionStringBuilder.UserID;
                _connection.Password = sqlConnectionStringBuilder.Password;
            };
            _server = new Server(_connection);

            // Instead of traditional Mapper.CreateMap<T1, T2>() which is global, we want instance mapping since those maps are just for this particular class
            var store = new ConfigurationStore(new TypeMapFactory(), MapperRegistry.Mappers);
            store.AssertConfigurationIsValid();
            _engine = new MappingEngine(store);
            CreateMaps(store);
        }

        public Database Collect(string databaseName)
        {
            var db = CollectDatabase(databaseName);
            ResolveTableReferences(db);
            return db;
        }

        ~DatabaseCollector()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _connection?.Disconnect();
            }
        }

        private Database CollectDatabase (string databaseName)
        {
            if (!_server.Databases.Contains(databaseName))
            {
                throw new ArgumentException(nameof(databaseName));
            }
            var db = _server.Databases[databaseName];

            var dbToReturn = _engine.Map<Database>(db);
            foreach (Microsoft.SqlServer.Management.Smo.StoredProcedure storedProcedure in db.StoredProcedures)
            {
                if (!storedProcedure.IsSystemObject)
                {
                    dbToReturn.StoredProcedures.Add(_engine.Map<StoredProcedure>(storedProcedure));
                }
            }

            return dbToReturn;
        }

        private void ResolveTableReferences(Database db)
        {
            foreach (var table in db.Tables)
            {
                foreach (var foreignKey in table.ForeignKeys)
                {
                    var referringTable = db.Tables.SingleOrDefault(s => s.FullyQualifiedName == foreignKey.FullyQualifiedReferencedTable);
                    if (referringTable == null)
                    {
                        throw new ArgumentException($"Referring table {foreignKey.FullyQualifiedReferencedTable} not found."); // This should be impossible
                    }
                    referringTable.DependentTables.Add(table);
                }
            }
        }

        private void CreateMaps(ConfigurationStore store)
        {
            store.CreateMap<Microsoft.SqlServer.Management.Smo.Database, Database>()
                .ForMember(s => s.StoredProcedures, t => t.Ignore()); // We need to exclude system sprocs

            store.CreateMap<Microsoft.SqlServer.Management.Smo.Table, Table>()
                .ForMember(s => s.FullyQualifiedName, t => t.MapFrom(m => $"{m.Parent.Name}.{m.Schema}.{m.Name}"));

            store.CreateMap<Microsoft.SqlServer.Management.Smo.Column, Column>()
                .ForMember(s => s.FullyQualifiedName, t => t.ResolveUsing<ColumnFulllyQualifiedNameResolver>());

            store.CreateMap<Microsoft.SqlServer.Management.Smo.ForeignKey, ForeignKey>()
                .ForMember(s => s.FullyQualifiedReferencedTable, t => t.MapFrom(m => $"{m.Parent.Parent.Name}.{m.ReferencedTableSchema}.{m.ReferencedTable}"));

            store.CreateMap<Microsoft.SqlServer.Management.Smo.StoredProcedure, StoredProcedure>()
               .ForMember(s => s.FullyQualifiedName, t => t.MapFrom(m => $"{m.Parent.Name}.{m.Schema}.{m.Name}"));
        }
    }

    internal class ColumnFulllyQualifiedNameResolver : ValueResolver<Microsoft.SqlServer.Management.Smo.Column, string>
    {
        protected override string ResolveCore(Microsoft.SqlServer.Management.Smo.Column source)
        {
            var parentTable = source.Parent as Microsoft.SqlServer.Management.Smo.Table;
            if (parentTable == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return $"{parentTable.Parent.Name}.{parentTable.Schema}.{parentTable.Name}.{source.Name}";
        }
    }
}
