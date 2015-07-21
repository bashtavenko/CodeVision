using CodeVision.CSharp.Semantic;
using NUnit.Framework;
using System.Linq;

namespace CodeVision.Tests
{
    [TestFixture]
    public class ModuleCollectorTests
    {
        [Test]
        public void ModuleCollector_CanCollectBySolution()
        {
            string solutionPath = @"..\..\..\CodeVision.sln";            
            var moduleCollector = new ModuleCollector();
            var modules = moduleCollector.GetModulesBySolution(solutionPath);
            Assert.IsNotNull(modules);

            var console = modules.SingleOrDefault(s => s.FileName == "CodeVision.Console.exe");
            Assert.IsNotNull(console);

            Assert.IsNotNull(console.References.SingleOrDefault(s => s.FileName == "CommandLine.dll"), "Assembly reference");
            Assert.IsNotNull(console.References.SingleOrDefault(s => s.FileName == "CodeVision.dll"), "Project reference");
        }
    }
}
