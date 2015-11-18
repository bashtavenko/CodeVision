using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis.MSBuild;

namespace CodeVision.Dependencies
{
    public class ModuleCollector
    {
        private readonly string[] _excludedProjectNames = { "test" };
        private readonly string[] _excludedModuleNames = { "System", "Microsoft", "mscorlib" };

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

        // This is public for unit testing.
        public static string Insertx86ToPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return path;
            }
            
            var pathParts = new List<string>(path.Split(Path.DirectorySeparatorChar));
            var binIndex = pathParts.FindIndex(w => w == "bin");
            if (binIndex == -1)
            {
                return path;
            }
            pathParts.Insert(binIndex + 1, "x86");
            return string.Join(Path.DirectorySeparatorChar.ToString(), pathParts);
        }
        
        internal ReflectionProperties GetAssemblyReflectionProperties (string filePath)
        {
            // If solution contains projects with different build configuration (AnyCpu and x86) Roslyn
            // sets project.OutputFilePath to "..bin\Debug\output.dll" as opposed to "..bin\x86\Debug" as it
            // specified in the default solution configuration (https://github.com/dotnet/roslyn/issues/6535#event-456527993) 
            // Inserting x86 seems to be the only way to fix this until the bug is fixed.
            if (!File.Exists(filePath))
            {
                filePath = Insertx86ToPath(filePath);
                // Let it fail with FileNotFoundException if still missing
            }

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
