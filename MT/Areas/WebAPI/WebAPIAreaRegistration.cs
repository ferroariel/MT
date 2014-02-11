using System.Web.Mvc;

namespace Website.Areas.Api
{
    public class ApiAreaRegistration : AreaRegistration
    {
        public override string AreaName { get { return "WebAPI"; } }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            /*
            context.MapRoute(
                "IncioMuro",
                "WebAPI/Widget/MTAct",
                new
                {
                    controller = "Widget",
                    action = "MTAct"
                }
            );
            context.MapRoute(
                "RecargaMuro",
                "WebAPI/Widget/MTActN",
                new
                {
                    controller = "Widget",
                    action = "MTActN"
                }
            );*/
            context.MapRoute(
                "MuroEnJSON",
                "WebAPI/MT/MuroJ",
                new
                {
                    controller = "MT",
                    action = "MuroJ",
                    _e = UrlParameter.Optional
                }
            );
            context.MapRoute(
                "MuroEnXML",
                "WebAPI/MT/MuroX",
                new
                {
                    controller = "MT",
                    action = "MuroX",
                    _e = UrlParameter.Optional
                }
            );
        }
    }
}