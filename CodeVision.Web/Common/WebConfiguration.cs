using System.Web;

namespace CodeVision.Web.Common
{
    public class WebConfiguration : IConfiguration
    {
        public string IndexPath { get; private set; }
        public string ContentRootPath { get; private set; }
        public string DependencyGraphConnectionString { get; private set; }

        public static WebConfiguration Load(HttpServerUtilityBase server)
        {
            var configuration = CodeVisionConfigurationSection.Load();
            return new WebConfiguration()
            {
                ContentRootPath = server.MapPath(configuration.ContentRootPath),
                IndexPath = server.MapPath(configuration.IndexPath),
                DependencyGraphConnectionString = configuration.DependencyGraphConnectionString
            };
        }
    }
}