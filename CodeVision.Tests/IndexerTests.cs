using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
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
            _indexer = new Indexer(null);
            _indexer.Index(GetCompletePath("Lucene.Net.Memory"));
        }
        
        [TestCase("TryGetValue")]
        [TestCase("start = ArrayUtil.grow(start, ord.Length);")]
        [TestCase("class:HashMapHelperClass")]
        [TestCase("Apache")]
        [TestCase("Software")]
        [TestCase("Apache Software")]
        [TestCase("method:GetValueOrNull")]
        //[TestCase("using:System.Collection.Generic")] this fails
        [TestCase("parameter:dictionary")]
        //[TestCase("return:TokenStream")] TODO:
        public void Indexer_CanSearch(string searchExpression)
        {
            var searcher = new Searcher();
            var hits = searcher.Search(searchExpression);
            Assert.IsNotEmpty(hits);
            Assert.IsFalse(hits.Any(s => string.IsNullOrEmpty(s.BestFragment)), "Must have best fragment for all hits");
            Assert.IsFalse(hits.Any(s => s.Offsets == null), "Must have offsets for all hits");
        }

        internal string GetCompletePath(string contentPath)
        {
            return Path.Combine("..\\..\\Content\\", contentPath);
        }
    }
}
