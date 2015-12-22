using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using s32.Sceh.Code;

namespace s32.Sceh
{
    public class MvcApplication : HttpApplication
    {
        private CardImageManager.Worker _cardImageWorker;
        private Thread _cardImageWorkerThread;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            CardImageManager.SetApp(this);

            _cardImageWorker = new CardImageManager.Worker(this);
            _cardImageWorkerThread = new Thread(_cardImageWorker.ThreadStart);
            _cardImageWorkerThread.Start();
        }

        protected void Application_End()
        {
            _cardImageWorker.Terminate();
            _cardImageWorkerThread.Join(1000);
        }
    }
}