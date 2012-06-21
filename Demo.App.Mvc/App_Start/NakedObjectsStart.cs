using System.Web.Mvc;
using System.Web.Routing;
using NakedObjects.Web.Mvc;
using WebActivator;

[assembly: PreApplicationStartMethod(typeof(Demo.App.Mvc.App_Start.NakedObjectsStart), "PreStart")]
[assembly: PostApplicationStartMethod(typeof(Demo.App.Mvc.App_Start.NakedObjectsStart), "PostStart")]

namespace Demo.App.Mvc.App_Start {
    public static class NakedObjectsStart {
        public static void PreStart() {
            RegisterRoutes(RouteTable.Routes);
        }

        public static void PostStart() {
            RunWeb.Run();
            DependencyResolver.SetResolver(new NakedObjectsDependencyResolver());
        }

        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{*favicon}", new {favicon = @"(.*/)?favicon.ico(/.*)?"});
            routes.IgnoreRoute("{*nakedobjects}", new {nakedobjects = @"(.*/)?nakedobjects.ico(/.*)?"});

            RunMvc.RegisterGenericRoutes(routes);
        }      
    }
}