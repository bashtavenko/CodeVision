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
            if (!_context.DatabaseObjectTypeLookups.Any())
            {
                _context.DatabaseObjectTypeLookups.AddRange(new DatabaseObjectTypeLookup[]
                {
                    new DatabaseObjectTypeLookup { DatabaseObjectTypeId = (int) DatabaseObjectType.Database, Name = "Database"},
                    new DatabaseObjectTypeLookup { DatabaseObjectTypeId = (int) DatabaseObjectType.Table, Name = "Table"},
                    new DatabaseObjectTypeLookup { DatabaseObjectTypeId = (int) DatabaseObjectType.StoredProcedure, Name = "Stored Procedure"},
                    new DatabaseObjectTypeLookup { DatabaseObjectTypeId = (int) DatabaseObjectType.Column, Name = "Column"}
                });
            }

            if (!_context.DatabaseObjectPropertyTypeLookups.Any())
            {
                _context.DatabaseObjectPropertyTypeLookups.AddRange(new DatabaseObjectPropertyTypeLookup[]
                {
                    new DatabaseObjectPropertyTypeLookup { DatabaseObjectPropertyTypeId = (int) DatabaseObjectPropertyType.RelevantToFinancialReporting, Name = "Relevant to Financial Reporting"},
                    new DatabaseObjectPropertyTypeLookup { DatabaseObjectPropertyTypeId = (int) DatabaseObjectPropertyType.Comment, Name = "Comment"},
                });
            }

            _context.SaveChanges();
        }
        
    }
}