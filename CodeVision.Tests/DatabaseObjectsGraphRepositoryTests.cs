using System.Data;
using System.Data.SqlClient;
using System.Linq;
using CodeVision.Dependencies;
using CodeVision.Dependencies.Database;
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
        private IDbConnection _connection;

        //       D1
        //  T1   T2  S1   
        //  C1       

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
            WipeoutAll();

            // Act 
            var g = new DatabaseObjectsGraph();

            var d1 = new DatabaseObject(DatabaseObjectType.Database, "D1");
            var t1 = new DatabaseObject(DatabaseObjectType.Table, "T1");
            var t2 = new DatabaseObject(DatabaseObjectType.Table, "T2");
            var s1 = new DatabaseObject(DatabaseObjectType.StoredProcedure, "S1");
            var c1 = new DatabaseObject(DatabaseObjectType.Column, "C1");

            g.AddDatabaseObject(d1);
            g.AddDependency(d1, t1);
            g.AddDependency(d1, t2);
            g.AddDependency(d1, s1);
            g.AddDependency(t1, c1);

            d1.Properties.Add(new RelevantToFinancialReportingProperty());
            d1.Properties.Add(new CommentProperty("Comment"));

            _repository.SaveState(g);

            // Assert
            Assert.That(_connection.ExecuteScalar("select count(*) from DatabaseObject;"), Is.EqualTo(5));
            Assert.That(_connection.ExecuteScalar("select count(*) from DatabaseObjectsGraph;"), Is.EqualTo(5));
            Assert.That(_connection.ExecuteScalar("select count(*) from DatabaseObjectProperty;"), Is.EqualTo(2));
        }

        [Test]
        public void DatabaseObjectsGraphRepository_AppendObject()
        {
            // Arrange
            WipeoutAll();
            
            var g = new DatabaseObjectsGraph();
            var d1 = new DatabaseObject(DatabaseObjectType.Database, "D1");
            var t1 = new DatabaseObject(DatabaseObjectType.Table, "T1");
            var t2 = new DatabaseObject(DatabaseObjectType.Table, "T2");
            
            g.AddDatabaseObject(d1);
            g.AddDependency(d1, t1);
            g.AddDependency(d1, t2);

            _repository.SaveState(g);
            
            Assert.That(_connection.ExecuteScalar("select count(*) from DatabaseObject;"), Is.EqualTo(3));
            
            var anotherGraph = _repository.LoadState();
            var t1FromGraph =  anotherGraph.GetDatabaseObjectsBeginsWith("T1").Single();

            // Act
            var s1 = new DatabaseObject(DatabaseObjectType.StoredProcedure, "S1");
            anotherGraph.AddDependency(t1FromGraph, s1);
            _repository.SaveState(anotherGraph);

            // Assert
            Assert.That(_connection.ExecuteScalar("select count(*) from DatabaseObject;"), Is.EqualTo(4));
        }

        [Test]
        public void DatabaseObjectsGraphRepository_AppendObject2()
        {
            // Arrange
            WipeoutAll();
            
            var g = new DatabaseObjectsGraph();
            var d1 = new DatabaseObject(DatabaseObjectType.Database, "D1");
            var t1 = new DatabaseObject(DatabaseObjectType.Table, "T1");
            var c1 = new DatabaseObject(DatabaseObjectType.Table, "C1");
            var s1 = new DatabaseObject(DatabaseObjectType.Table, "S1");
            
            g.AddDependency(d1, t1);
            g.AddDependency(t1, c1);
            g.AddDependency(d1, s1);
            _repository.SaveState(g);

            Assert.That(_connection.ExecuteScalar("select count(*) from DatabaseObject;"), Is.EqualTo(4));
            var adjacencyListForS1 = _connection.ExecuteScalar("select AdjacencyListJson from DatabaseObjectsGraph where VertexId = 3") as string;
            Assert.That(adjacencyListForS1, Is.EqualTo("[]"));
            
            var anotherGraph = _repository.LoadState();
            var s1FromGraph = anotherGraph.GetDatabaseObjectsBeginsWith("S1").Single();
            var c1FromGraph = anotherGraph.GetDatabaseObjectsBeginsWith("C1").Single();

            // Act
            anotherGraph.AddDependency(s1FromGraph, c1FromGraph);
            // We don't modify object state because the whole graph in db will be overwritten
            _repository.SaveState(anotherGraph);

            // Assert
            Assert.That(_connection.ExecuteScalar("select count(*) from DatabaseObject;"), Is.EqualTo(4));
            adjacencyListForS1 = _connection.ExecuteScalar("select AdjacencyListJson from DatabaseObjectsGraph where VertexId = 3") as string;
            Assert.That(adjacencyListForS1, Is.EqualTo("[2]"));
        }

        [Test]
        public void DatabaseObjectsGraphRepository_AddProperty()
        {
            // Arrange
            WipeoutAll();
            
            var g = new DatabaseObjectsGraph();

            var d1 = new DatabaseObject(DatabaseObjectType.Database, "D1");
            var t1 = new DatabaseObject(DatabaseObjectType.Table, "T1");
            var c1 = new DatabaseObject(DatabaseObjectType.Column, "C1");

            g.AddDatabaseObject(d1);
            g.AddDependency(d1, t1);
            g.AddDependency(t1, c1);
            _repository.SaveState(g);

            Assert.That(_connection.ExecuteScalar("select count(*) from DatabaseObject;"), Is.EqualTo(3));
            Assert.That(_connection.ExecuteScalar("select count(*) from DatabaseObjectProperty;"), Is.EqualTo(0));
            
            var anotherGraph = _repository.LoadState();

            var c1FromGraph = anotherGraph.GetDatabaseObjectsBeginsWith("C1").Single();
            Assert.That(c1FromGraph.ObjectType, Is.EqualTo(DatabaseObjectType.Column));

            // Act
            c1FromGraph.Properties.Add(new RelevantToFinancialReportingProperty());
            c1FromGraph.ObjectState = ObjectState.PropertiesModified;
            _repository.SaveState(anotherGraph);

            // Assert
            Assert.That(_connection.ExecuteScalar("select count(*) from DatabaseObjectProperty;"), Is.EqualTo(1));
        }

        [Test]
        public void DatabaseObjectsGraphRepository_RemoveProperty()
        {
            // Arrange
            WipeoutAll();
            var g = new DatabaseObjectsGraph();

            var d1 = new DatabaseObject(DatabaseObjectType.Database, "D1");
            var t1 = new DatabaseObject(DatabaseObjectType.Table, "T1");
            var c1 = new DatabaseObject(DatabaseObjectType.Column, "C1");

            g.AddDatabaseObject(d1);
            g.AddDependency(d1, t1);
            g.AddDependency(t1, c1);

            c1.Properties.Add(new RelevantToFinancialReportingProperty());
            c1.Properties.Add(new CommentProperty("Comment"));

            _repository.SaveState(g);
            
            Assert.That(_connection.ExecuteScalar("select count(*) from DatabaseObject;"), Is.EqualTo(3));
            Assert.That(_connection.ExecuteScalar("select count(*) from DatabaseObjectsGraph;"), Is.EqualTo(3));
            Assert.That(_connection.ExecuteScalar("select count(*) from DatabaseObjectProperty;"), Is.EqualTo(2));
            
            var anotherGraph = _repository.LoadState();

            var c1FromGraph = anotherGraph.GetDatabaseObjectsBeginsWith("C1").Single();
            Assert.That(c1FromGraph.ObjectType, Is.EqualTo(DatabaseObjectType.Column));
            Assert.That(c1FromGraph.Properties.Count, Is.EqualTo(2));

            // Act
            c1FromGraph.Properties.RemoveAt(1);
            c1FromGraph.ObjectState = ObjectState.PropertiesModified;
            _repository.SaveState(anotherGraph);

            // Assert
            Assert.That(_connection.ExecuteScalar("select count(*) from DatabaseObjectProperty;"), Is.EqualTo(1));
        }

        private void WipeoutAll()
        {
            _connection.Execute(@"truncate table DatabaseObjectProperty;
                                  delete from DatabaseObject;");
        }
    }
}