using System.IO;
using System.Web.Mvc;
using AutoMapper;
using CodeVision.Model;
using CodeVision.Web.ViewModels;

namespace CodeVision.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View(new SearchResult());
        }

        public ActionResult Search(string searchExpression)
        {
            var configuration = WebConfiguration.Load(Server);
            var searcher = new Searcher(configuration);
            ReadOnlyHitCollection hitCollection = searcher.Search(searchExpression);
            SearchResult model = Mapper.Map<SearchResult>(hitCollection);
            return View("Index", model);
        }

        public ActionResult Help()
        {
            return View();
        }
    }
}