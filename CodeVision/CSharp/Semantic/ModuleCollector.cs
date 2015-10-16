using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;

using Microsoft.CodeAnalysis.MSBuild;

namespace CodeVision.CSharp.Semantic
{
    public class ModuleCollector
    {
        private string[] _excludedProjectNames = { "test" };
        private string[] _excludedModuleNames = { "System", "Microsoft", "mscorlib" };

        public List<Module> GetModulesBySolution (string solutionPath)
        {
            using (var workspace = MSBuildWorkspace.Create())
            {
                var solution = workspace.OpenSolutionAsync(solutionPath).Result;

                var modules = new List<Module>();
                foreach (var project in solution.Projects)
                {
                    if (_excludedProjectNames.Any(w => project.Name.ToLower().Contains(w.ToLower())))
                    {
                        continue;
                    }

                    var projectModulePath = project.OutputFilePath;
                    var projectModuleName = Path.GetFileNameWithoutExtension(projectModulePath);
                    var reflectionPropeties = GetAssemblyReflectionProperties(projectModulePath);
                    var module = new Module(projectModuleName, reflectionPropeties.Version, reflectionPropeties.Description);

                    // Build the list of all dlls referenced by the project, both assembly(metadata) and project references.
                    List<string> dllPaths = project.MetadataReferences.Select(s => s.Display).ToList();
                    foreach (var projectReference in project.ProjectReferences)
                    {
                        var referencedProject = solution.Projects.Single(s => s.Id == projectReference.ProjectId);
                        dllPaths.Add(referencedProject.OutputFilePath);
                    }
                                        
                    foreach (var dllPath in dllPaths)
                    {                        
                        string fileName = Path.GetFileNameWithoutExtension(dllPath);
                        if (_excludedModuleNames.Any(w => fileName.ToLower().Contains(w.ToLower())))
                        {
                            continue;
                        }
                        var reflectionProperties = GetAssemblyReflectionProperties(dllPath);
                        module.AddReference(new Module(fileName, reflectionProperties.Version, reflectionProperties.Description));                                                
                    }
                    modules.Add(module);
                }
                
                return modules;
            }
        }

        private ReflectionProperties GetAssemblyReflectionProperties (string filePath)
        {
            try
            {
                var assembly = Assembly.LoadFrom(filePath);
                var descriptionAttribute = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>();
                return new ReflectionProperties
                {
                    Version = assembly.GetName().Version.ToString(),
                    Description = descriptionAttribute != null ? descriptionAttribute.Description : string.Empty                    
                };
            }
            catch (FileLoadException)
            {
                // Example: mixed mode assembly is built against version 'v2.0.50727' of the runtime and cannot be loaded in the 4.0 runtime
                return new ReflectionProperties
                {
                    Version = "Unknown"
                };
            }
        }
    }

    internal class ReflectionProperties
    {
        public string Version { get; set; }
        public string Description { get; set; }
    }
}
