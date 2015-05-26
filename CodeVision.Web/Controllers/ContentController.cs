using System;
using System.Text;
using System.Web.Mvc;
using AutoMapper;
using CodeVision.Model;
using CodeVision.Web.ViewModels;
using Newtonsoft.Json;
using PagedList;

namespace CodeVision.Web.Controllers
{
    public class ContentController : Controller
    {
        public ActionResult Details(string hit)
        {
            var base64EncodedBytes = Convert.FromBase64String(hit);
            var json = Encoding.UTF8.GetString(base64EncodedBytes);
            var modelHit = JsonConvert.DeserializeObject<ViewModels.SearchHit>(json);
            var domainHit = Mapper.Map<Model.Hit>(modelHit);

            var configuration = WebConfiguration.Load(Server);
            var searcher = new Searcher(configuration);
            string text = searcher.GetFileContent(domainHit);
            
            var viewModel = new Document {Name = modelHit.FriendlyFileName, Text = text};
            return View((object)viewModel);
        }
    }
}