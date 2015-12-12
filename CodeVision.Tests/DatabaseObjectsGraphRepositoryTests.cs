using System.Configuration;
using System.Linq;
using CodeVision.Dependencies.Database;
using Dapper;
using NUnit.Framework;

namespace CodeVision.Tests
{
    [TestFixture]
    public class DatabaseObjectsGraphRepositoryTests : DatabaseTest
    {
        private DatabaseObjectsGraphRepository _repository;
        //       D1
        //  T1   T2  S1   
        //  C1       

        [TestFixtureSetUp]
        public void Setup()
        {
            var configuration = CodeVisionConfigurationSection.Load();
            _repository = new DatabaseObjectsGraphRepository(ConnectionString);
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
            
            _repository.SaveState(g);

            // Assert
            Assert.That(GetTableRowCount("DatabaseObject"), Is.EqualTo(5));
            Assert.That(GetTableRowCount("DatabaseObjectsGraph"), Is.EqualTo(5));
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
            
            Assert.That(GetTableRowCount("DatabaseObject"), Is.EqualTo(3));
            
            var anotherGraph = _repository.LoadState();
            var t1FromGraph =  anotherGraph.GetDatabaseObjectsBeginsWith("T1").Single();

            // Act
            var s1 = new DatabaseObject(DatabaseObjectType.StoredProcedure, "S1");
            anotherGraph.AddDependency(t1FromGraph, s1);
            _repository.SaveState(anotherGraph);

            // Assert
            Assert.That(GetTableRowCount("DatabaseObject"), Is.EqualTo(4));
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

            Assert.That(GetTableRowCount("DatabaseObject"), Is.EqualTo(4));
            var adjacencyListForS1 = Connection.ExecuteScalar("select AdjacencyListJson from DatabaseObjectsGraph where VertexId = 3") as string;
            Assert.That(adjacencyListForS1, Is.EqualTo("[]"));
            
            var anotherGraph = _repository.LoadState();
            var s1FromGraph = anotherGraph.GetDatabaseObjectsBeginsWith("S1").Single();
            var c1FromGraph = anotherGraph.GetDatabaseObjectsBeginsWith("C1").Single();

            // Act
            anotherGraph.AddDependency(s1FromGraph, c1FromGraph);
            // We don't modify object state because the whole graph in db will be overwritten
            _repository.SaveState(anotherGraph);

            // Assert
            Assert.That(GetTableRowCount("DatabaseObject"), Is.EqualTo(4));
            adjacencyListForS1 = Connection.ExecuteScalar("select AdjacencyListJson from DatabaseObjectsGraph where VertexId = 3") as string;
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

            Assert.That(GetTableRowCount("DatabaseObject"), Is.EqualTo(3));
            Assert.That(GetTableRowCount("DatabaseObjectProperty"), Is.EqualTo(0));
            
            var anotherGraph = _repository.LoadState();

            var c1FromGraph = anotherGraph.GetDatabaseObjectsBeginsWith("C1").Single();
            Assert.That(c1FromGraph.ObjectType, Is.EqualTo(DatabaseObjectType.Column));

            // Act
            anotherGraph.AddProperty(c1FromGraph, new RelevantToFinancialReportingProperty());
            _repository.SaveState(anotherGraph);

            // Assert
            Assert.That(GetTableRowCount("DatabaseObjectProperty"), Is.EqualTo(1));
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

            // That's fine for test, but won't work with AddProperty
            c1.Properties.Add(new RelevantToFinancialReportingProperty());
            c1.Properties.Add(new CommentProperty("Comment"));

            _repository.SaveState(g);
            
            Assert.That(GetTableRowCount("DatabaseObject"), Is.EqualTo(3));
            Assert.That(GetTableRowCount("DatabaseObjectsGraph"), Is.EqualTo(3));
            Assert.That(GetTableRowCount("DatabaseObjectProperty"), Is.EqualTo(2));
            
            var anotherGraph = _repository.LoadState();

            var c1FromGraph = anotherGraph.GetDatabaseObjectsBeginsWith("C1").Single();
            Assert.That(c1FromGraph.ObjectType, Is.EqualTo(DatabaseObjectType.Column));
            Assert.That(c1FromGraph.Properties.Count, Is.EqualTo(2));

            // Act
            anotherGraph.RemoveProperty(c1FromGraph, c1FromGraph.Properties.First());
            _repository.SaveState(anotherGraph);

            // Assert
            Assert.That(GetTableRowCount("DatabaseObjectProperty"), Is.EqualTo(1));
        }

        [Test]
        public void DatabaseObjectsGraphRepository_DuplicateObject()
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

            Assert.That(GetTableRowCount("DatabaseObject"), Is.EqualTo(3));
            Assert.That(GetTableRowCount("DatabaseObjectsGraph"), Is.EqualTo(3));

            var anotherGraph = _repository.LoadState();

            // Act
            // Adding the same objects again
            var d12 = new DatabaseObject(DatabaseObjectType.Database, "D1");
            var t12 = new DatabaseObject(DatabaseObjectType.Table, "T1");
            var c12 = new DatabaseObject(DatabaseObjectType.Column, "C1");
            g.AddDatabaseObject(d12);
            g.AddDependency(d12, t12);
            g.AddDependency(t12, c12);

            // Assert
            Assert.That(GetTableRowCount("DatabaseObject"), Is.EqualTo(3));
            Assert.That(GetTableRowCount("DatabaseObjectsGraph"), Is.EqualTo(3));
        }


        [Test]
        public void DatabaseObjectsGraphRepository_ChangeComment()
        {
            // Arrange
            WipeoutAll();
            var g = new DatabaseObjectsGraph();
            
            var c1 = new DatabaseObject(DatabaseObjectType.Column, "C1");
            c1.Properties.Add(new CommentProperty("Comment"));
            g.AddDatabaseObject(c1);

            _repository.SaveState(g);

            Assert.That(GetTableRowCount("DatabaseObject"), Is.EqualTo(1));
            Assert.That(GetTableRowCount("DatabaseObjectProperty"), Is.EqualTo(1));
            var originalComment = Connection.ExecuteScalar("select top(1) PropertyValue from DatabaseObjectProperty") as string;

            var anotherGraph = _repository.LoadState();

            // Act
            const string newCommentText = "New";
            var c1FromGraph = anotherGraph.GetDatabaseObjectsBeginsWith("C1").Single();
            anotherGraph.UpdatedCommentText(c1FromGraph, newCommentText);
            _repository.SaveState(anotherGraph);

            // Assert
            var udpatedComment = Connection.ExecuteScalar("select top(1) PropertyValue from DatabaseObjectProperty") as string;
            Assert.That(udpatedComment, Is.EqualTo(newCommentText));
        }

        // Load from another database to track performance
        //[Test]
        public void DataBaseObjectsGraphRepository_LoadState()
        {
            var repository = new DatabaseObjectsGraphRepository(ConfigurationManager.ConnectionStrings["ReadOnlyDatabase"].ToString());
            DatabaseObjectsGraph graph = repository.LoadState();
        }
    }
}