using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeVision.Dependencies.Modules
{
    public class ModulesGraph : SymbolGraph<Module>
    {
        public IEnumerable<Module> Modules => Symbols;

        public ModulesGraph() : base()
        {
        }

        public ModulesGraph(Memento<Module[]> memento, Digraph g) : base(memento, g)
        {
        }

        public void AddModule(Module module)
        {
            AddSymbol(module);
        }

        public void AddDependency(Module fromModule, Module toModule)
        {
            AddEdge(fromModule, toModule);
        } 

        public List<Module> GetModulesBeginsWith (string name)
        {
            return SymbolTable.Keys
                .Where(w => w.Name.StartsWith(name, StringComparison.InvariantCultureIgnoreCase))
                .OrderBy(o => o.Name)
                .ThenBy(o => o.Version)
                .Take(10)
                .ToList();
        }
    }
}
