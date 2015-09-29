using System;
using System.Collections.Generic;
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

        // Throws Lucene.Net.QueryParsers.QueryParser.LookaheadSuccess. Probably bug in Lucene 
        //[TestCase("return:void")] 

        // No highlits or offsets since these are stop words
        [TestCase("comment:try-catch")]
        [TestCase("postLinkFn")] // js
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

            hitCollection = searcher.Search(searchExpression, null, null, 1, 1);
            Assert.That(hitCollection.Count, Is.EqualTo(1));

            hitCollection = searcher.Search(searchExpression, null, null, 2, 1);
            Assert.That(hitCollection.Count, Is.EqualTo(1));
        }

        [Test]
        public void Indexer_GetFileContent()
        {
            // Arrange
            var searcher = new Searcher();
            var hit = new Hit(1, "..\\..\\Content\\", GetCompletePath("Lucene.Net.Memory\\MemoryIndexNormDocValues.cs"), 1f, Languages.CSharp);
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
            var hit = new Hit(1, "..\\..\\Content\\", GetCompletePath("Lucene.Net.Memory\\MemoryIndexNormDocValues.cs"), 1f, Languages.CSharp);
            hit.Offsets.Add(new Offset { StartOffset = 1000, EndOffset = 2000 });
            
            // Act/Assert
            Assert.Throws<ArgumentException>(() => searcher.GetFileContent(hit));
        }

        [Test]
        public void Indexer_MoreThanOneLanguage()
        {
            var searcher = new Searcher();
            var hitCollection = searcher.Search("remove");
            Assert.That(hitCollection.Count, Is.AtLeast(3));
        }

        [Test]
        public void Indexer_ThreeLanguages()
        {
            var searcher = new Searcher();
            var hitCollection = searcher.Search("select");
            Assert.That(hitCollection.Count, Is.AtLeast(3));

            hitCollection = searcher.Search("select", new Filter(Fields.Language, new List<string>{"sql"}));
            Assert.That(hitCollection.Count, Is.AtLeast(1));

            hitCollection = searcher.Search("select", new Filter(Fields.Language, new List<string> {"cs"}));
            Assert.That(hitCollection.Count, Is.AtLeast(1));

            hitCollection = searcher.Search("select", new Filter(Fields.Language, new List<string> { "js" }));
            Assert.That(hitCollection.Count, Is.AtLeast(1));
        }


        [Test]
        public void Indexer_Filter()
        {
            var searcher = new Searcher();
            var filter = new Filter(Fields.Language, new List<string>() {"js"});
            var hitCollection = searcher.Search("remove", filter);
            Assert.That(hitCollection.Count, Is.AtLeast(1));
        }

        [Test]
        public void Indexer_LexicalError()
        {
            var searcher = new Searcher();
            Assert.Throws<SearchException>(() => searcher.Search("return:void]"));
        }

        // This doesn't work because it thinks content:AND
        //[Test]
        public void Indexer_MoreThanOneLanguage_OneField()
        {
            var searcher = new Searcher();
            var hitCollection = searcher.Search("content:remove AND language:js");
            Assert.That(hitCollection.Count, Is.AtLeast(1));
        }

        [Test]
        public void Indexer_Dups()
        {
            var searcher = new Searcher();
            var hitCollection = searcher.Search("HashSet");
            Assert.That(hitCollection.Count, Is.EqualTo(1));
        }

        // May alter state of shared index, it should probably be in a separate file
        //[Test]
        public void Indexer_WithExcludedFiles()
        {
            var indexer = new Indexer(null);
            indexer.Index(GetCompletePath("."), new List<string> { "sql" });

            var searcher = new Searcher();
            var hitCollection = searcher.Search("SalesByCategory");
            Assert.False(hitCollection.Any(s => s.Language == Languages.Sql), "Must not have any SQL files");            
        }

        internal string GetCompletePath(string contentPath)
        {
            return Path.Combine("..\\..\\Content\\", contentPath);
        }
    }
}
