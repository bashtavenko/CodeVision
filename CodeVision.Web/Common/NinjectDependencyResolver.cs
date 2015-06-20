using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http.ExceptionHandling;
using System.Web.Mvc;
using CodeVision.Web.Controllers;
using log4net;
using Ninject;
using Ninject.Web.Common;

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
            kernel.Bind<IExceptionLogger>().To<ExceptionLogger>().InRequestScope();
            kernel.Bind<HttpServerUtilityBase>().ToMethod(c => new HttpServerUtilityWrapper(HttpContext.Current.Server));
        }        
    }
}