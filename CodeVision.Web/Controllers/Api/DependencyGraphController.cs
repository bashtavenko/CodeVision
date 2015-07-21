using System.Collections.Generic;
using System.Web.Http;

using CodeVision.CSharp.Semantic;


namespace CodeVision.Web.Controllers.Api
{
    public class DependencyGraphController : ApiController
    {
        private readonly DependencyGraph _graph;        

        public DependencyGraphController(DependencyGraph graph)
        {
            _graph = graph;
        }
        
        [Route("api/modules")]
        public IList<string> Get (string name)
        {
            var items = _graph.GetModulesBeginsWith(name);            
            return items;
        }

        [Route("api/graph/{direction}/{levels}")]
        public IList<string> Get(string key, DependencyDirection direction, DependencyLevels levels)
        {
            var items = _graph.GetDependencies(key, direction, levels);
            return items;
        }
    }
}
