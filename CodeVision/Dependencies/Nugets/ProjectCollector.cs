using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;

namespace CodeVision.Dependencies.Nugets
{
    public class ProjectCollector
    {
        private readonly string[] _excludedProjectNames = { "test" };

        public List<Project> GetProjectsBySolution(string solutionPath)
        {
            using (var workspace = MSBuildWorkspace.Create())
            {
                var solution = workspace.OpenSolutionAsync(solutionPath).Result;

                var projects = new List<Project>();
                foreach (var roslynProject in solution.Projects)
                {
                    string projectName = roslynProject.Name;
                    if (_excludedProjectNames.Any(w => projectName.ToLower().Contains(w.ToLower())))
                    {
                        continue;
                    }

                    var project = new Project
                    {
                        Name = roslynProject.Name,
                        OutputKind = roslynProject.CompilationOptions.OutputKind.ToString(),
                        Platform = roslynProject.CompilationOptions.Platform.ToString(),
                        Packages = GetNugetPackages(roslynProject.FilePath)
                    };

                    projects.Add(project);
                }

                return projects;
            }
        }

        private IList<Package> GetNugetPackages(string projectFilePath)
        {
            return ProjectPackagesCollector.Collect(Path.GetDirectoryName(projectFilePath));
        }
    }
}