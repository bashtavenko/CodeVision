using System.Diagnostics.Tracing;
using System.IO;
using NUnit.Framework;

namespace CodeVision.Tests
{
    [TestFixture]
    public class IndexerTests
    {
        private Indexer _indexer;

        [TestFixtureSetUp]
        public void Setup()
        {
            _indexer = new Indexer();
            _indexer.Index(GetCompletePath("Lucene.Net.Memory"));
        }
        
        [TestCase("TryGetValue")]
        [TestCase("class:HashMapHelperClass")]
        [TestCase("method:GetValueOrNull")]
        //[TestCase("using:System.Collection.Generic")] TODO: blows up
        [TestCase("parameter:dictionary")]
        //[TestCase("return:TokenStream")] TODO:
        public void Indexer_CanSearch(string searchExpression)
        {
            var searcher = new Searcher();
            var hits = searcher.Search(searchExpression);
            Assert.IsNotEmpty(hits);
        }

        internal string GetCompletePath(string contentPath)
        {
            return Path.Combine("..\\..\\Content\\", contentPath);
        }
    }
}
