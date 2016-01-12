using System;
using System.Collections.Generic;
using CodeVision.Dependencies.Nugets;
using Moq;
using NUnit.Framework;

namespace CodeVision.Tests
{
    [TestFixture]
    public class NugetCollectorTests
    {   
        private Mock<ILogger> _loggerMock;
        private NugetCollector _collector;

        [TestFixtureSetUp]
        public void Setup()
        {            
            var connectionString = CodeVisionConfigurationSection.Load().DependencyGraphConnectionString;
            _loggerMock = new Mock<ILogger>();
            _collector = new NugetCollector(connectionString, _loggerMock.Object);
        }

        [Test]
        public void NugetCollector_CanRunOne()
        {
            string solutionPath = @"..\..\..\CodeVision.sln";            
            _collector.CollectNugets(new List<string> { solutionPath });
            _loggerMock.Verify(v => v.Log(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);
        }
    }
}