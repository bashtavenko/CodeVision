﻿using System.Linq;
using Newtonsoft.Json;
using CodeVision.CSharp.Semantic.SqlStorage;

namespace CodeVision.CSharp.Semantic
{
    public class DependencyGraphRepository
    {
        private readonly string _connectionString;

        public DependencyGraphRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void SaveState(DependencyGraph dg)
        {
            Module[] symbolTable = dg.CreateMemento().State;
            int[][] jaggedArray = dg.Digraph.CreateMemento().State;

            // Save
            using (var ctx = new DependencyGraphContext(_connectionString))
            {
                ctx.Database.ExecuteSqlCommand("TRUNCATE TABLE DependencyGraphModule;");
                ctx.Database.ExecuteSqlCommand("TRUNCATE TABLE DependencyGraph;");

                foreach (var module in symbolTable)
                {
                    ctx.Modules.Add(new SqlStorage.Module { ModuleId = module.Id.Value, Name = module.Name, Version = module.Version, Description = module.Description });
                }

                for (int i = 0; i < jaggedArray.Length; i++)
                {
                    var json = JsonConvert.SerializeObject(jaggedArray[i]);
                    ctx.Vertices.Add(new SqlStorage.Vertex { VertexId = i, AdjacencyListJson = json });
                }
                ctx.SaveChanges();
            }
        }

        public DependencyGraph LoadState()
        {
            int[][] jaggedArray;
            Module[] symbolTable;

            // Load
            using (var ctx = new DependencyGraphContext(_connectionString))
            {
                jaggedArray = new int[ctx.Vertices.Count()][];                
                for (int i = 0; i < jaggedArray.Length; i++)
                {
                    var json = ctx.Vertices.Find(i).AdjacencyListJson;
                    int[] adjencencyList = JsonConvert.DeserializeObject<int[]>(json);
                    jaggedArray[i] = adjencencyList;                    
                }

                symbolTable = ctx.Modules
                    .ToList()
                    .Select(s => new Module(s.Name, s.Version, s.Description, s.ModuleId)).ToArray();
            }
            
            var g = new Digraph(new Memento<int[][]>(jaggedArray));
            var dg = new DependencyGraph(new Memento<Module[]>(symbolTable), g);
            return dg;
        }
    }
}
