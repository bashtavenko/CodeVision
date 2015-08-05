using System.IO;
using CodeVision.CSharp;
using NUnit.Framework;

namespace CodeVision.Tests
{
    [TestFixture]
    public class CSharpParserTests
    {
        private CSharpParser _parser;

        [TestFixtureSetUp]
        public void Setup()
        {
            _parser = new CSharpParser();
        }
        
        [TestCase("Lucene.Net.Memory\\MemoryIndexNormDocValues.cs")]
        [TestCase("Lucene.Net.Expressions\\Bindings.cs")]
        [TestCase("Lucene.Net.Expressions\\SimpleBindings.cs")]
        public void Parser_CanParse(string fileName)
        {
            string filePath = GetCompleteFilePath(fileName);
            var result = _parser.Parse(filePath);
        }

        [TestCase("Lucene.Net.Memory\\MemoryIndexNormDocValues.cs", "lucene.net.memory")]
        [TestCase("Lucene.Net.Expressions\\Bindings.cs", "lucene.net.expressions")]
        [TestCase("Lucene.Net.Expressions\\SimpleBindings.cs", "lucene.net.expressions")]
        [TestCase("Lucene.Net.Memory\\HashMapHelperClass.cs", null)]
        public void Parser_CanParseNamespace(string fileName, string expectedNamespace)
        {
            string filePath = GetCompleteFilePath(fileName);
            var result = _parser.Parse(filePath);
            Assert.That(result.Namespace, Is.EqualTo(expectedNamespace));
        }

        internal string GetCompleteFilePath(string fileName)
        {
            return Path.Combine("..\\..\\Content\\", fileName);
        }
    }
}
