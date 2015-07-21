using System.Web;
using System.Web.Mvc;
using CodeVision.Web;
using CodeVision.Web.Controllers;
using CodeVision.Web.ViewModels;
using log4net;
using Moq;
using NUnit.Framework;
using CodeVision.Web.Common;

namespace CodeVision.Tests.Web
{
    [TestFixture]
    public class IntegrationTests
    {
        private HomeController _homeController;
        private ContentController _contentController;

        [TestFixtureSetUp]
        public void Setup()
        {
            AutoMapperConfig.CreateMaps();

            var server = new Mock<HttpServerUtilityBase>();
            server.Setup(s => s.MapPath(It.IsAny<string>())).Returns<string>(x => x);
            var configuration = WebConfiguration.Load(server.Object);
            
            _homeController = new HomeController(configuration, new Mock<ILog>().Object);
            _contentController = new ContentController(configuration);
        }

        [Test]
        public void Integration_CanSearch_and_ViewHit()
        {
            var redirectToRouteResult = _homeController.Search("apache", null, null) as RedirectToRouteResult;
            Assert.IsNotNull(redirectToRouteResult);

            var searchExpressionEncoded = redirectToRouteResult.RouteValues["searchExpressionEncoded"] as string;
            Assert.IsNotNull(searchExpressionEncoded);

            var result = _homeController.Search(searchExpressionEncoded, null, null, 1) as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as SearchResult;
            Assert.IsNotNull(model);

            Assert.That(model.Hits.Count, Is.AtLeast(2));

            var firstHit = model.Hits[0];
            var contentResult = _contentController.Details(firstHit.ToString()) as ViewResult;
            Assert.IsNotNull(contentResult);
            var document = contentResult.Model as Document;
            Assert.IsNotNull(document);
            Assert.That(document.Name, Is.EqualTo("MemoryIndexNormDocValues.cs"));
            Assert.IsNotNullOrEmpty(document.Text);
        }
    }
}
