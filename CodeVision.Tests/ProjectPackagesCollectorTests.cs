using CodeVision.Dependencies.Nugets;
using NUnit.Framework;

namespace CodeVision.Tests
{
    [TestFixture]
    public class ProjectPackagesCollectorTests
    {
        [Test]
        public void ProjectPackagesCollector_CanGetPackagesWithMetadata()
        {
            string projectPath = @"..\..\..\CodeVision";
            var packages = ProjectPackagesCollector.Collect(projectPath);
            Assert.IsNotEmpty(packages);
        }

        [Test]
        public void ProjectPackagesCollector_NoPackages()
        {
            string projectPath = @"..\..\..\";
            var packages = ProjectPackagesCollector.Collect(projectPath);
            Assert.IsEmpty(packages);
        }
    }
}