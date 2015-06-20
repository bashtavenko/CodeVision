using System.Web;
using System.Web.Mvc;
using CodeVision.Web.Common;
using CodeVision.Web.Controllers;
using log4net;

namespace CodeVision.Web
{
    public class FilterConfig
    {
        private static ILog _log;

        static FilterConfig()
        {
            _log = LogManager.GetLogger(typeof(HomeController));
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ExceptionFilter(_log, string.Empty, "Error"));
        }
    }
}
