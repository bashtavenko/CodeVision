using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NuGet;

namespace CodeVision.Dependencies.Nugets
{
    public class ProjectPackagesCollector
    {
        private static readonly string[] ExcludedPackageNames = { "System", "Microsoft" };

        public static IList<Package> Collect(string projectFolder)
        {
            string packageReferenceFilePath = Path.Combine(projectFolder, "packages.config");
            if (!File.Exists(packageReferenceFilePath))
            {
                return new List<Package>();
            }

            string solutionFolder = Path.GetFullPath(Path.Combine(projectFolder, @"..\"));
            string packageRepositoryPath = Path.Combine(solutionFolder, "packages");

            var packageReferences= new PackageReferenceFile(packageReferenceFilePath);
            var packageRepository = new LocalPackageRepository(packageRepositoryPath);
            var packages = new List<Package>();

            foreach (var packageReference in packageReferences.GetPackageReferences())
            {
                string packageId = packageReference.Id;
                if (ExcludedPackageNames.Any(w => packageId.ToLower().Contains(w.ToLower())))
                {
                    continue;
                }
                var package = new Package
                {
                    Name = packageId,
                    Version = packageReference.Version.Version.ToString(),
                    TargetFramework = packageReference?.TargetFramework?.Version?.ToString()
                };

                // Try to get remaining attributes from local package store
                var nugetPackage = packageRepository.FindPackage(packageId, packageReference.Version);
                if (nugetPackage != null)
                {
                    package.Summary = nugetPackage.Summary;
                    package.Description = nugetPackage.Description;
                    package.Authors = String.Join(",", nugetPackage.Authors);
                    package.IconUrl = nugetPackage.IconUrl;
                    package.ProjectUrl = nugetPackage.ProjectUrl;
                }
                packages.Add(package);
            }

            return packages;
        }
    }
}