using System.Data;
using System.Data.SqlClient;
using System.Linq;
using CodeVision.Dependencies;
using CodeVision.Dependencies.Database;
using CodeVision.Dependencies.SqlStorage;
using Dapper;
using NUnit.Framework;
using DatabaseObject = CodeVision.Dependencies.Database.DatabaseObject;

namespace CodeVision.Tests
{
    [TestFixture]
    public class DatabaseObjectsGraphRepositoryTests
    {
        private string _connectionString;
        
        private DatabaseObjectsGraphRepository _repository;
        private DatabaseObjectsGraph _g;
        private IDbConnection _connection;

        //       D1
        //  T1   T2  S1   
        //  C1       
        private DatabaseObject _D1;
        private DatabaseObject _T1;
        private DatabaseObject _T2;
        private DatabaseObject _S1;
        private DatabaseObject _C1;


        [TestFixtureSetUp]
        public void Setup()
        {
            var configuration = CodeVisionConfigurationSection.Load();
            _connectionString = configuration.DependencyGraphConnectionString;
            _repository = new DatabaseObjectsGraphRepository(_connectionString);
            _connection = new SqlConnection(_connectionString);
        }

        [Test]
        public void DatabaseObjectsGraphRepository_BasicSave()
        {
            // Arrange
            _connection.Execute(@"truncate table DatabaseObjectProperty;
                                  delete from DatabaseObject;");

            // Act 
            var databaseObjects = _repository.GetDatabaseObjects();
            _g = new DatabaseObjectsGraph(new Memento<DatabaseObject[]>(databaseObjects));

            _D1 = new DatabaseObject(DatabaseObjectType.Database, "D1");
            _T1 = new DatabaseObject(DatabaseObjectType.Table, "T1");
            _T2 = new DatabaseObject(DatabaseObjectType.Table, "T2");
            _S1 = new DatabaseObject(DatabaseObjectType.StoredProcedure, "S1");
            _C1 = new DatabaseObject(DatabaseObjectType.Column, "C1");

            _g.AddDatabaseObject(_D1);
            _g.AddDependency(_D1, _T1);
            _g.AddDependency(_D1, _T2);
            _g.AddDependency(_D1, _S1);
            _g.AddDependency(_T1, _C1);

            _D1.Properties.Add(new RelevantToFinancialReportingProperty());
            _D1.Properties.Add(new CommentProperty("Comment"));

            _repository.SaveState(_g);

            // Assert
            Assert.That(_connection.ExecuteScalar("select count(*) from DatabaseObject;"), Is.EqualTo(5));
            Assert.That(_connection.ExecuteScalar("select count(*) from DatabaseObjectProperty;"), Is.EqualTo(2));
        }
    }
}