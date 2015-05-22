using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using CodeVision.Model;
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
        [TestCase("HashMapHelperClass")]
        [TestCase("class:HashMapHelperClass")]
        [TestCase("Apache")]
        [TestCase("Software")]
        [TestCase("Apache Software")]
        [TestCase("method:GetValueOrNull")]
        [TestCase("parameter:dictionary")]
        // TODO
        //[TestCase("using:System.Collection.Generic")]
        //[TestCase("return:TokenStream")]
        public void Indexer_CanSearch(string searchExpression)
        {
            var searcher = new Searcher();
            var hitCollection = searcher.Search(searchExpression);
            Assert.IsNotEmpty(hitCollection);
            Assert.IsFalse(hitCollection.Any(s => string.IsNullOrEmpty(s.BestFragment)), "Must have best fragment for all hits");
            Assert.IsFalse(hitCollection.Any(s => s.Offsets.Count == 0), "Must have offsets for all hits");
        }

        [Test]
        public void Indexer_CanSearch_WithPaging()
        {
            var searcher = new Searcher();
            const string searchExpression = "is";
            
            var hitCollection = searcher.Search(searchExpression);
            Assert.That(hitCollection.TotalHits, Is.GreaterThanOrEqualTo(3));

            hitCollection = searcher.Search(searchExpression, 1, 1);
            Assert.That(hitCollection.Count, Is.EqualTo(1));

            hitCollection = searcher.Search(searchExpression, 2, 1);
            Assert.That(hitCollection.Count, Is.EqualTo(1));
        }

        internal string GetCompletePath(string contentPath)
        {
            return Path.Combine("..\\..\\Content\\", contentPath);
        }
    }
}
