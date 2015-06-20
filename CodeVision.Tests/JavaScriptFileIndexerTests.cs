using NUnit.Framework;

namespace CodeVision.Tests
{
    [TestFixture]
    public class JavaScriptFileIndexerTests
    {
        private JavaScriptFileIndexer _indexer;

        [TestFixtureSetUp]
        public void Setup()
        {
            _indexer = new JavaScriptFileIndexer(null, null);
        }

        [TestCase(@"C:\Jenkins\workspace\Portal\Scripts\jquery-ui-1.8.19.js", true)]
        [TestCase(@"C:\Jenkins\workspace\Portal\js\2011.3.1306\telerik.autocomplete.js", true)]
        [TestCase(@"C:\Jenkins\workspace\Portal\editors\tiny_mce3\themes\anchor.js", true)]
        [TestCase(@"C:\Jenkins\workspace\Portal\WebForms\MSAjax\MicrosoftAjaxWebServices.js", true)]
        [TestCase(@"C:\Jenkins\workspace\Portal\bower_components\webshim\js-webshim\dev\shims\form-core.j", true)]
        public void JavaScriptFileIndexer_IsThirdParty(string filePath, bool thirdParty)
        {
            Assert.IsTrue(_indexer.IsThirdParty(filePath) == thirdParty);
        }
    }
}
