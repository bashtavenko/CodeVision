using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using AutoMapper;
using CodeVision.Model;
using CodeVision.Web.ViewModels;

namespace CodeVision.Web.Controllers
{
    public class HomeController : Controller
    {
        const int PageSize = 3;

        public ActionResult Index()
        {
            return View(new SearchResult(string.Empty, new List<SearchHit>(), 1, PageSize, 0));
        }

        public ActionResult Search(string searchExpression, int page = 1)
        {
            var configuration = WebConfiguration.Load(Server);
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