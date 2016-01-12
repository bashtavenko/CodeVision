using System.Linq;
using CodeVision.Dependencies.SqlStorage;
using Newtonsoft.Json;

namespace CodeVision.Dependencies.Modules
{
    public class ModulesGraphRepository
    {
        private readonly string _connectionString;

        public ModulesGraphRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void SaveState(ModulesGraph dg)
        {
            Module[] symbolTable = dg.CreateMemento().State;
            int[][] jaggedArray = dg.Digraph.CreateMemento().State;

            // Save
            using (var ctx = new DependencyGraphContext(_connectionString))
            {
                ctx.Database.ExecuteSqlCommand("TRUNCATE TABLE Module;");
                ctx.Database.ExecuteSqlCommand("TRUNCATE TABLE ModulesGraph;");

                foreach (var module in symbolTable)
                {
                    ctx.Modules.Add(new SqlStorage.Module { ModuleId = module.Id.Value, Name = module.Name, Version = module.Version, Description = module.Description });
                }

                for (int i = 0; i < jaggedArray.Length; i++)
                {
                    var json = JsonConvert.SerializeObject(jaggedArray[i]);
                    ctx.ModuleVertices.Add(new ModuleVertex { VertexId = i, AdjacencyListJson = json });
                }
                ctx.SaveChanges();
            }
        }

        public ModulesGraph LoadState()
        {
            int[][] jaggedArray;
            Module[] symbolTable;

            // Load
            using (var ctx = new DependencyGraphContext(_connectionString))
            {
                jaggedArray = new int[ctx.ModuleVertices.Count()][];                
                for (int i = 0; i < jaggedArray.Length; i++)
                {
                    var json = ctx.ModuleVertices.Find(i).AdjacencyListJson;
                    int[] adjencencyList = JsonConvert.DeserializeObject<int[]>(json);
                    jaggedArray[i] = adjencencyList;                    
                }

                symbolTable = ctx.Modules
                    .ToList()
                    .Select(s => new Module(s.Name, s.Version, s.Description, s.ModuleId)).ToArray();
            }
            
            var g = new Digraph(new Memento<int[][]>(jaggedArray));
            var dg = new ModulesGraph(new Memento<Module[]>(symbolTable), g);
            return dg;
        }
    }
}
