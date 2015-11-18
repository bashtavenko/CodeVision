using System;
using System.Data.SqlClient;
using NUnit.Framework;
using Dapper;

namespace CodeVision.Tests
{
    public class DatabaseTest
    {
        protected string ConnectionString { get; set; }
        protected SqlConnection Connection { get; set; }

        [TestFixtureSetUp]
        public void SetupBase()
        {
            var configuration = CodeVisionConfigurationSection.Load();
            ConnectionString = configuration.DependencyGraphConnectionString;
            Connection = new SqlConnection(ConnectionString);
        }

        [TestFixtureTearDown]
        public void TearDownBase()
        {
            Connection.Close();
        }

        protected int GetTableRowCount(string tableName)
        {
            return Convert.ToInt32(Connection.ExecuteScalar($"select count(*) from {tableName};"));
        }

        protected void WipeoutAll()
        {
            Connection.Execute(@"truncate table DatabaseObjectProperty;
                                  delete from DatabaseObject;");
        }
    }
}