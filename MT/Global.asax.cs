using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MT
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode,
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// http://www.dotnet-tricks.com/Tutorial/mvc/0a9M050113-Bundling-and-minification-in-MVC3-and-Asp.Net-4.0.html
        /// </summary>
        /// <param name="bundles"></param>
        public static void RegisterBundles(BundleCollection bundles)
        {

            var bundle = new StyleBundle("~/css/gen").Include(
                "~/css/foundation.css",
                "~/css/app.css",
                "~/css/mt.css",
                "~/css/fontawesome.css",
                "~/css/tinyscrollbar.css"
                );            
            bundle.Transforms.Add(new CssMinify());
            bundles.Add(bundle);   
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");  // http://stackoverflow.com/questions/4624190/mvc-does-the-favicon-ico-also-look-for-a-controller
            routes.MapRoute("Varios", "{AllValues}.html", new { controller = "Varios", action = "Index" });
            routes.MapRoute("Empresa", "Directorio/Empresa/{id}.html", new { controller = "Directorio", action = "Empresa", id = UrlParameter.Optional });
            routes.MapRoute("Index", String.Empty, new { controller = "Directorio", action = "Home" });
            routes.MapRoute("Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }

        protected void Application_Start()
        {
            //HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            RegisterBundles(BundleTable.Bundles);
            BundleTable.EnableOptimizations = true;             
        }
    }
}