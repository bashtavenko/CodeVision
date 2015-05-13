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


        [Test]
        public void Parser_CanParse()
        {
            string filePath = GetCompleteFilePath("MemoryIndexNormDocValues.cs");
            var result = _parser.Parse(filePath);
        }

        internal string GetCompleteFilePath(string fileName)
        {
            return Path.Combine("..\\..\\Content\\Lucene.Net.Memory", fileName);
        }
    }
}
