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
        }

        [Test]
        public void Indexer_CanIndex()
        {
            _indexer.Index(GetCompletePath("Lucene.Net.Memory"));
        }

        [Test]
        public void Indexer_CanIndexAndSearch()
        {
            _indexer.Index(GetCompletePath("Lucene.Net.Memory"));
            var searcher = new Searcher();
            var hits = searcher.Search("static");
        }

        internal string GetCompletePath(string contentPath)
        {
            return Path.Combine("..\\..\\Content\\", contentPath);
        }
    }
}
