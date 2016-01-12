using CodeVision.Dependencies.Nugets;
using NUnit.Framework;

namespace CodeVision.Tests
{
    [TestFixture]
    public class ProjectCollectorTests
    {
        [Test]
        public void ProjectCollector_CanCollectBySolution()
        {
            string solutionPath = @"..\..\..\CodeVision.sln";            
            var projectCollector = new ProjectCollector();
            var projects = projectCollector.GetProjectsBySolution(solutionPath);
            Assert.That(projects.Count, Is.GreaterThanOrEqualTo(3));
            Assert.That(projects[0].Packages.Count, Is.GreaterThan(0));
        }
    }
}