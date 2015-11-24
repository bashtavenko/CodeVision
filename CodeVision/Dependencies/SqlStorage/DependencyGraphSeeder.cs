using System.Linq;
using CodeVision.Dependencies.Database;

namespace CodeVision.Dependencies.SqlStorage
{
    public class DependencyGraphSeeder
    {
        private readonly DependencyGraphContext _context;

        public DependencyGraphSeeder(DependencyGraphContext context)
        {
            _context = context;
        }

        public void Seed()
        {
            _context.DatabaseObjectTypeLookups.AddRange(new DatabaseObjectTypeLookup[]
            {
                new DatabaseObjectTypeLookup { ObjectType = DatabaseObjectType.Database, Name = "Database"},
                new DatabaseObjectTypeLookup { ObjectType = DatabaseObjectType.Table, Name = "Table"},
                new DatabaseObjectTypeLookup { ObjectType = DatabaseObjectType.StoredProcedure, Name = "Stored Procedure"},
                new DatabaseObjectTypeLookup { ObjectType = DatabaseObjectType.Column, Name = "Column"}
            });
            
            _context.DatabaseObjectPropertyTypeLookups.AddRange(new DatabaseObjectPropertyTypeLookup[]
            {
                new DatabaseObjectPropertyTypeLookup { PropertyType = DatabaseObjectPropertyType.RelevantToFinancialReporting, Name = "Relevant to Financial Reporting"},
                new DatabaseObjectPropertyTypeLookup { PropertyType = DatabaseObjectPropertyType.Comment, Name = "Comment"},
            });

            _context.SaveChanges();
        }
    }
}