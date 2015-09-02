using NUnit.Framework;

namespace CodeVision.Tests
{
    [TestFixture]
    public class SearcherTests
    {
        [TestCase("List<<kbd>Account</kbd>>", "List&lt;<kbd>Account</kbd>&gt;")]
        [TestCase("System.Nullable<System.DateTime", "System.Nullable&lt;System.DateTime")]
        public void Searcher_CanEscapeHtmlMarkup(string input, string expectedOutput)
        {
            string output = new Searcher().EscapeHtmlMarkup(input);
            Assert.That(output, Is.EqualTo(expectedOutput));
        }
    }
}
