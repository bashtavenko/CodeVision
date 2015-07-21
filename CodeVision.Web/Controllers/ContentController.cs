using System;
using System.Text;
using System.Web.Mvc;
using AutoMapper;
using CodeVision.Web.Common;
using Newtonsoft.Json;

namespace CodeVision.Web.Controllers
{
    public class ContentController : Controller
    {
        private readonly WebConfiguration _configuration;        

        public ContentController(WebConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public ActionResult Details(string hit)
        {
            var base64EncodedBytes = Convert.FromBase64String(hit);
            var json = Encoding.UTF8.GetString(base64EncodedBytes);
            var modelHit = JsonConvert.DeserializeObject<ViewModels.SearchHit>(json);
            var domainHit = Mapper.Map<Model.Hit>(modelHit);
                        
            var searcher = new Searcher(_configuration);
            string text = searcher.GetFileContent(domainHit);

            var viewModel = Mapper.Map<ViewModels.Document>(modelHit);
            viewModel.Text = text;
            return View((object)viewModel);
        }
    }
}