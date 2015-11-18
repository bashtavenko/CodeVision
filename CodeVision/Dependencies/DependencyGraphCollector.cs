using System;
using System.Collections.Generic;
using System.IO;

namespace CodeVision.Dependencies
{
    public class DependencyGraphCollector
    {
        private readonly string _connectionString;
        private readonly ILogger _logger;

        public DependencyGraphCollector(string connectionString, ILogger logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public void CollectDependencies (List<string> solutionPaths)
        {
            var graph = new DependencyGraph();
            var collector = new ModuleCollector();

            foreach (var solutionPath in solutionPaths)
            {
                try
                {
                    _logger.Log($"Geting modules for {Path.GetFileName(solutionPath)}...");

                    var modules = collector.GetModulesBySolution(solutionPath);
                    foreach (var v in modules)
                    {
                        graph.AddModule(v);
                        foreach (var w in v.References)
                        {
                            graph.AddDependency(v, w);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Log($"Failed to get modules for {Path.GetFileName(solutionPath)} solution - {ex.Message}", ex);
                }
            }
            var repository = new DependencyGraphRepository(_connectionString);
            repository.SaveState(graph);
        }
    }
}
