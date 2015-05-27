using System;
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
            _indexer.Index(GetCompletePath("."));
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
        //[TestCase("return:void")]
        public void Indexer_CanSearch(string searchExpression)
        {
            var searcher = new Searcher();
            var hitCollection = searcher.Search(searchExpression);
            Assert.IsNotEmpty(hitCollection);
            Assert.IsFalse(hitCollection.Any(s => string.IsNullOrEmpty(s.BestFragment)), "Must have best fragment for all hits");
            Assert.IsFalse(hitCollection.Any(s => s.Offsets.Count == 0), "Must have offsets for all hits");
        }

        [TestCase("Apa*")]
        public void Indexer_CanSearchWithWildcard(string searchExpression)
        {
            var searcher = new Searcher();
            var hitCollection = searcher.Search(searchExpression);
            Assert.IsNotEmpty(hitCollection);
            Assert.IsFalse(hitCollection.Any(s => string.IsNullOrEmpty(s.BestFragment)), "Must have best fragment for all hits");
        }

        [Test]
        public void Indexer_CanSearch_WithPaging()
        {
            var searcher = new Searcher();
            const string searchExpression = "int";
            
            var hitCollection = searcher.Search(searchExpression);
            Assert.That(hitCollection.TotalHits, Is.GreaterThanOrEqualTo(3));

            hitCollection = searcher.Search(searchExpression, 1, 1);
            Assert.That(hitCollection.Count, Is.EqualTo(1));

            hitCollection = searcher.Search(searchExpression, 2, 1);
            Assert.That(hitCollection.Count, Is.EqualTo(1));
        }

        [Test]
        public void Indexer_GetFileContent()
        {
            // Arrange
            var searcher = new Searcher();
            var hit = new Hit(1, "..\\..\\Content\\", GetCompletePath("Lucene.Net.Memory\\MemoryIndexNormDocValues.cs"), 1f);
            hit.Offsets.Add(new Offset { StartOffset = 90, EndOffset = 96 });
            hit.Offsets.Add(new Offset { StartOffset = 347, EndOffset = 353 });
            hit.Offsets.Add(new Offset { StartOffset = 545, EndOffset = 551 });

            // Act
            var result = searcher.GetFileContent(hit);

            // Assert
            Assert.IsNotNullOrEmpty(result);
        }

        [Test]
        public void Indexer_GetFileContent_InvalidOffsets()
        {
            // Arrange
            var searcher = new Searcher();
            var hit = new Hit(1, "..\\..\\Content\\", GetCompletePath("Lucene.Net.Memory\\MemoryIndexNormDocValues.cs"), 1f);
            hit.Offsets.Add(new Offset { StartOffset = 1000, EndOffset = 2000 });
            
            // Act/Assert
            Assert.Throws<ArgumentException>(() => searcher.GetFileContent(hit));
        }

        internal string GetCompletePath(string contentPath)
        {
            return Path.Combine("..\\..\\Content\\", contentPath);
        }
    }
}
