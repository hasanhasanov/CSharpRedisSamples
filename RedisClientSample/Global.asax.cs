using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using RedisClientSample.Controllers;
using RedisProvider.SessionProvider;
using ServiceStack.Configuration;
using ServiceStack.Logging;
using ServiceStack.Logging.Support.Logging;
using ServiceStack.Mvc;
using ServiceStack.Redis;
using ServiceStack.WebHost.Endpoints;
using Funq;


namespace RedisClientSample
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            // register Redis log
            if (ConfigUtils.GetAppSetting("log", false)) { LogManager.LogFactory = new ConsoleLogFactory(); }

            //Initialize Funq IoC with Redis Repository
            // and register controllers
            Container container = InitializeContainer();
            ControllerBuilder.Current.SetControllerFactory(new FunqControllerFactory(container));
        }

        private Container InitializeContainer()
        {
            Container container = new Container();

            container.Register<IRedisClientsManager>(c => new PooledRedisClientManager());
            container.Register<IRepository>(c => new Repository(c.Resolve<IRedisClientsManager>()));

            /*container.Register<IFormsAuthentication>(c => new FormsAuthenticationService());
            container.Register<IMembershipService>(c => new AccountMembershipService());
            container.Register<IController>("Account",
                c => new AccountController(c.Resolve<IFormsAuthentication>(), c.Resolve<IMembershipService>()))
                .ReusedWithin(ReuseScope.None)
                .OwnedBy(Owner.External);*/

            container.Register<IController>("Question", c => 
                new QuestionController{RedisRepo = container.Resolve<IRepository>()})
                .ReusedWithin(ReuseScope.None).OwnedBy(Owner.External);

            container.Register<IController>("Answer", c => 
                new AnswerController{RedisRepo = container.Resolve<IRepository>()})
                .ReusedWithin(ReuseScope.None).OwnedBy(Owner.External);
            
            return container;
        }

        protected void Session_Start()
        {
            Session["testRedisSession"] = "Message from the redis ression";
        }
    }
}