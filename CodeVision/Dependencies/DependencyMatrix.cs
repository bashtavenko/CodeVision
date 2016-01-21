using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeVision.Dependencies
{
    public class DependencyMatrix
    {
        public bool[,] Matrix => BuildMatrix();
        public List<DependencyMatrixItem> Rows { get; private set; }
        public List<DependencyMatrixItem> Columns { get; private set; }

        private readonly List<Tuple<int, int>> _dependencies;

        public DependencyMatrix()
        {
            _dependencies = new List<Tuple<int, int>>();
            Rows = new List<DependencyMatrixItem>();
            Columns = new List<DependencyMatrixItem>();
        }

        public void AddDependency(string rowValue, string columnValue)
        {
            int rowId = AddItem(Rows, rowValue);
            int columnId = AddItem(Columns, columnValue);
            _dependencies.Add(new Tuple<int, int>(rowId, columnId));
        }

        public void Sort()
        {
            Rows = Rows.OrderBy(s => s.Value).ToList();
            Columns = Columns.OrderBy(s => s.Value).ToList();
        }
        
        private int AddItem(List<DependencyMatrixItem> items, string newItemValue)
        {
            var itemInTheList = GetItem(items, newItemValue);
            if (itemInTheList == null)
            {
                var newId = items.Any() ? items.Max(m => m.Id) + 1 : 0;
                items.Add(new DependencyMatrixItem(newId, newItemValue));
                return newId;
            }
            return itemInTheList.Id;
        }

        private bool[,] BuildMatrix()
        {
            if (!_dependencies.Any())
            {
                return new bool[,] {};
            }

            int rows = _dependencies.Max(m => m.Item1) + 1;
            int columns = _dependencies.Max(m => m.Item2) + 1;
            var matrix = new bool[rows, columns];

            foreach (var tuple in _dependencies)
            {
                matrix[tuple.Item1, tuple.Item2] = true;
            }

            return matrix;
        }

        private DependencyMatrixItem GetItem(List<DependencyMatrixItem> items, string value)
        {
            return items.SingleOrDefault(s => s.Value == value);
        }
    }
}