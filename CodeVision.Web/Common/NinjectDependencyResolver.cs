using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http.ExceptionHandling;
using System.Web.Mvc;
using CodeVision.Web.Controllers;
using log4net;
using Ninject;
using Ninject.Web.Common;
using CodeVision.CSharp.Semantic;
using Ninject.Activation;

namespace CodeVision.Web.Common
{
    public class NinjectDependencyResolver : IDependencyResolver    
    {
        private IKernel kernel;

        public NinjectDependencyResolver(IKernel kernelParam)
        {
            kernel = kernelParam;
            AddBindings();
        }

        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }

        private void AddBindings()
        {
            kernel.Bind<ILog>().ToMethod(ctx => LogManager.GetLogger(typeof(HomeController)));
            kernel.Bind<IExceptionLogger>().To<Log4NetExceptionLogger>().InRequestScope();            

            // These are singletons, watch out..
            kernel.Bind<WebConfiguration>().ToMethod(GetWebConfiguration).InSingletonScope();
            kernel.Bind<DependencyGraph>().ToMethod(GetDependencyGraph).InSingletonScope();
        }

        private WebConfiguration GetWebConfiguration(IContext context)
        {
            var configuration = WebConfiguration.Load(new HttpServerUtilityWrapper(HttpContext.Current.Server));
            return configuration;
        }

        private DependencyGraph GetDependencyGraph (IContext context)
        {
            var configuration = GetWebConfiguration(context);
            var repository = new DependencyGraphRepository(configuration.DependencyGraphConnectionString);
            if (string.IsNullOrEmpty(configuration.DependencyGraphConnectionString))
            {
                throw new ArgumentNullException("Must have DependencyGraphConnectionString in web.config");
            }
            var graph = repository.LoadState();
            return graph;
        }
    }
}