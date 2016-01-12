using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http.ExceptionHandling;
using System.Web.Mvc;
using System.Web.Services.Description;
using CodeVision.Web.Controllers;
using log4net;
using Ninject;
using Ninject.Web.Common;
using CodeVision.Dependencies;
using CodeVision.Dependencies.Database;
using CodeVision.Dependencies.Modules;
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
            kernel.Bind<DatabaseObjectsGraphRepository>().ToMethod(GetDatabaseObjectsGraphRepository).InRequestScope();

            // These are singletons, watch out..
            kernel.Bind<WebConfiguration>().ToMethod(GetWebConfiguration).InSingletonScope();
            kernel.Bind<ModulesGraph>().ToMethod(GetDependencyGraph).InSingletonScope();
            kernel.Bind<DatabaseObjectsGraph>().ToMethod(GetDatabaseObjectsGraph).InSingletonScope();
        }

        private WebConfiguration GetWebConfiguration(IContext context)
        {
            var configuration = WebConfiguration.Load(new HttpServerUtilityWrapper(HttpContext.Current.Server));
            return configuration;
        }

        private ModulesGraph GetDependencyGraph(IContext context)
        {
            var repository = GetDependencyGraphRepository(context);
            var graph = repository.LoadState();
            return graph;
        }

        private DatabaseObjectsGraph GetDatabaseObjectsGraph(IContext context)
        {
            var repository = GetDatabaseObjectsGraphRepository(context);
            var graph = repository.LoadState();
            return graph;
        }

        private ModulesGraphRepository GetDependencyGraphRepository(IContext context)
        {
            var configuration = WebConfiguration.Load(new HttpServerUtilityWrapper(HttpContext.Current.Server));
            if (string.IsNullOrEmpty(configuration.DependencyGraphConnectionString))
            {
                throw new ArgumentException("Must have DependencyGraphConnectionString in web.config");
            }
            var repository = new ModulesGraphRepository(configuration.DependencyGraphConnectionString);
            return repository;;
        }

        private DatabaseObjectsGraphRepository GetDatabaseObjectsGraphRepository(IContext context)
        {
            var configuration = WebConfiguration.Load(new HttpServerUtilityWrapper(HttpContext.Current.Server));
            if (string.IsNullOrEmpty(configuration.DependencyGraphConnectionString))
            {
                throw new ArgumentException("Must have DependencyGraphConnectionString in web.config");
            }
            var repository = new DatabaseObjectsGraphRepository(configuration.DependencyGraphConnectionString);
            return repository; ;
        }
    }
}