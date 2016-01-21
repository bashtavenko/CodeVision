using System.Collections.Generic;
using System.Web.Http;
using AutoMapper;
using CodeVision.Dependencies.Nugets;

namespace CodeVision.Web.Controllers.Api
{
    public class NuGetsController : ApiController
    {
        private readonly ProjectRepository _repository;

        public NuGetsController(ProjectRepository repository)
        {
            _repository = repository;
        }

        [Route("api/nugets/packages")]
        public IList<ViewModels.Package> Get(string name)
        {
            var items = _repository.GetPackages(name);
            return Mapper.Map<List<ViewModels.Package>>(items);
        }

        [Route("api/nugets/projects/{packageId}")]
        public IList<ViewModels.Project> GetProjects(int packageId)
        {
            var items = _repository.GetProjects(packageId);
            return Mapper.Map<List<ViewModels.Project>>(items);
        }

        [Route("api/nugets/matrix")]
        public ViewModels.DependencyMatrix GetDependencyMatrix(string project = null, string package = null)
        {
            var dm = _repository.GetDependencyMatrix(project, package);
            return Mapper.Map<ViewModels.DependencyMatrix>(dm);
        }
    }
}