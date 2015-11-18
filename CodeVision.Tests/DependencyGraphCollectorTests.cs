using System;
using System.Collections.Generic;

using Moq;
using NUnit.Framework;
using CodeVision.Dependencies;

namespace CodeVision.Tests
{
    [TestFixture]
    public class DependencyGraphCollectorTests
    {   
        private Mock<ILogger> _loggerMock;
        private DependencyGraphCollector _collector;

        [TestFixtureSetUp]
        public void Setup()
        {            
            var connectionString = CodeVisionConfigurationSection.Load().DependencyGraphConnectionString;
            _loggerMock = new Mock<ILogger>();
            _collector = new DependencyGraphCollector(connectionString, _loggerMock.Object);
        }

        [Test]
        public void DependencyGraphCollector_CanRunOne()
        {
            string solutionPath = @"..\..\..\CodeVision.sln";            
            _collector.CollectDependencies(new List<string> { solutionPath });
            _loggerMock.Verify(v => v.Log(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);
        }
    }
}
