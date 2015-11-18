using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using CodeVision.Dependencies.SqlStorage;
using CodeVision.Model;
using CodeVision.Web.Common;
using CodeVision.Web.ViewModels;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CodeVision.Web.Controllers
{
    public class HomeController : Controller
    {        
        private readonly ILog _log;
        private readonly WebConfiguration _configuration;

        const int PageSize = 10;        

        public HomeController(WebConfiguration configuration, ILog log)
        {
            _configuration = configuration;
            _log = log;
        }

        public ActionResult Index()
        {
            return View(new SearchResult());
        }

        [HttpPost]
        public ActionResult Search(string searchExpression, string language, string sort)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(searchExpression);
            string searchExpressionEncoded = Convert.ToBase64String(bytes);
            return RedirectToAction("Search", new {searchExpressionEncoded = searchExpressionEncoded, language = language, sort = sort, page = 1});
        }

        // Gets have search expression encoded because it may have special characters
        public ActionResult Search(string searchExpressionEncoded, string language, string sort, int page)
        {
            var base64EncodedBytes = Convert.FromBase64String(searchExpressionEncoded);
            var searchExpression = Encoding.UTF8.GetString(base64EncodedBytes);
            var languageParam = language == "-1" ? string.Empty : language;
            var sortParam = sort == "-1" ? string.Empty : language;
                        
            var searcher = new Searcher(_configuration);
            var filter = !string.IsNullOrEmpty(languageParam) ? new Model.Filter(Fields.Language, languageParam) : null;
            ReadOnlyHitCollection hitCollection;
            SearchResult model;
            try
            {
                hitCollection = searcher.Search(searchExpression, filter, sortParam, page, PageSize);
            }
            catch (SearchException ex)
            {
                model = new SearchResult(searchExpression, language, sort, page, PageSize, ex.Message);
                _log.Error("Search failure", ex);
                return View("Index", model);
            }
            List<SearchHit> modelHits = Mapper.Map<List<SearchHit>>(hitCollection);
            model = new SearchResult(searchExpression, modelHits, language, sort, page, PageSize, hitCollection.TotalHits);
            return View("Index", model);
        }

        public ActionResult Graph()
        {
            return View();
        }

        public ActionResult Dictionary()
        {
            var list = Enum.GetValues(typeof(DatabaseObjectPropertyType)).Cast<DatabaseObjectPropertyType>().ToList();
            var properties = Mapper.Map<List<ViewModels.DatabaseObjectProperty>>(list);
            var bootrsrapData = new {Properties = properties};
            var jsonSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var json = JsonConvert.SerializeObject(bootrsrapData, Formatting.None, jsonSettings);
            return View((object)json);
        }

        public ActionResult Help()
        {
            return View();
        }
    }
}