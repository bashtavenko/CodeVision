using System;
using System.Collections.Generic;

namespace CodeVision.Dependencies.Database
{
    public class DatabaseObjectGraphCollector
    {
        private readonly DatabaseCollector _databaseCollector;
        private readonly DatabaseObjectsGraph _graph;
        private readonly DatabaseObjectsGraphRepository _repository;
        private readonly ILogger _logger;

        public DatabaseObjectGraphCollector(string targetConnectionString, string repositoryConnectionString, ILogger logger)
        {
            _databaseCollector = new DatabaseCollector(targetConnectionString);
            _repository = new DatabaseObjectsGraphRepository(repositoryConnectionString);

            // We want graph with pre-initialized database objects / symbol table because we don't want to overwrite existing objects properties or create duplicated objects.
            _graph = _repository.LoadState();

            _logger = logger;
        }

        public void CollectDependencies(List<string> databaseNames)
        {
            if (databaseNames == null)
            {
                throw new ArgumentNullException(nameof(databaseNames));
            }

            foreach (var databaseName in databaseNames)
            {
                try
                {
                    CollectDatabase(databaseName);
                }
                catch (Exception ex)
                {
                    _logger.Log($"Failed to collect dependencies from {databaseName} database - {ex}");
                }
            }
            
            _repository.SaveState(_graph);
        }

        private void CollectDatabase(string databaseName)
        {
            var db = _databaseCollector.Collect(databaseName);
            var databaseObject = new DatabaseObject(DatabaseObjectType.Database, db.Name);
            _graph.AddDatabaseObject(databaseObject);
            foreach (var table in db.Tables)
            {
                var tableObject = new DatabaseObject(DatabaseObjectType.Table, table.FullyQualifiedName);
                _graph.AddDependency(databaseObject, tableObject);
                foreach (var column in table.Columns)
                {
                    var columnObject = new DatabaseObject(DatabaseObjectType.Column, column.FullyQualifiedName);
                    _graph.AddDependency(tableObject, columnObject);
                }
            }

            foreach (var storedProcedure in db.StoredProcedures)
            {
                var storedProcedureObject = new DatabaseObject(DatabaseObjectType.StoredProcedure, storedProcedure.FullyQualifiedName);
                _graph.AddDependency(databaseObject, storedProcedureObject);
            }
        }
    }
}