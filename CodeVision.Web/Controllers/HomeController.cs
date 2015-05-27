using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using CodeVision.Model;
using CodeVision.Web.Common;
using CodeVision.Web.ViewModels;

namespace CodeVision.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpServerUtilityBase _server;

        const int PageSize = 10;

        public HomeController(HttpServerUtilityBase server)
        {
            _server = server;
        }

        public ActionResult Index()
        {
            return View(new SearchResult(string.Empty, new List<SearchHit>(), 1, PageSize, 0));
        }


        [HttpPost]
        public ActionResult Search(string searchExpression)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(searchExpression);
            string searchExpressionEncoded = Convert.ToBase64String(bytes);
            return RedirectToAction("Search", new {searchExpressionEncoded = searchExpressionEncoded, page = 1});
        }

        // Gets have search expression encoded because it may have special characters
        public ActionResult Search(string searchExpressionEncoded, int page)
        {
            var base64EncodedBytes = Convert.FromBase64String(searchExpressionEncoded);
            var searchExpression = Encoding.UTF8.GetString(base64EncodedBytes);
            
            var configuration = WebConfiguration.Load(_server);
            var searcher = new Searcher(configuration);
            ReadOnlyHitCollection hitCollection = searcher.Search(searchExpression, page, PageSize);
            List<SearchHit> modelHits = Mapper.Map<List<SearchHit>>(hitCollection);
            var model = new SearchResult(searchExpression, modelHits, page, PageSize, hitCollection.TotalHits);
            return View("Index", model);
        }

        public ActionResult Help()
        {
            return View();
        }
    }
}