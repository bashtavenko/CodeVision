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
        [TestCase("Lucene.Net.Expressions\\Test.cs")]
        public void Parser_CanParse(string fileName)
        {
            string filePath = GetCompleteFilePath(fileName);
            var result = _parser.Parse(filePath);
        }

        internal string GetCompleteFilePath(string fileName)
        {
            return Path.Combine("..\\..\\Content\\", fileName);
        }
    }
}
