using System;
using System.Collections.Generic;
using System.IO;

namespace CodeVision.Dependencies.Nugets
{
    public class NugetCollector
    {
        private readonly ILogger _logger;
        private readonly string _connectionString;

        public NugetCollector(string connectionString, ILogger logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public void CollectNugets (List<string> solutionPaths)
        {
            using (var repository = new ProjectRepository(_connectionString))
            {
                var projectCollector = new ProjectCollector();
                foreach (var solutionPath in solutionPaths)
                {
                    try
                    {
                        _logger.Log($"Geting nugets for {Path.GetFileName(solutionPath)}...");

                        var projects = projectCollector.GetProjectsBySolution(solutionPath);
                        foreach (var project in projects)
                        {
                            repository.SaveProject(project);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Log($"Failed to get nugets for this solution - {ex.Message}", ex);
                    }
                }
                _logger.Log("Done.");
            }
        }
    }
}