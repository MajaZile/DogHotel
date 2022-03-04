using System.Web.Mvc;
using System.Web.Routing;

namespace HotelZaPse
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //custom ruta -> za prenos 2 parametra iz View-a -> Controller
            routes.MapRoute(
                name: "ViewProfileByAdminCustomRoute",
                url: "Pas/ViewProfileByAdmin/{vid}/{pid}",
                defaults: new
                {
                    controller = "Pas",
                    action = "ViewProfileByAdmin",
                    vid = "",
                    pid = ""
                }
                );

            //custom ruta -> za prenos 2 parametra iz View-a -> Controller
            routes.MapRoute(
                name: "EditPasCustomRoute",
                url: "Pas/Edit/{vid}/{pid}",
                defaults: new
                {
                    controller = "Pas",
                    action = "Edit",
                    vid = "",
                    pid = ""
                }
                );

            //custom ruta -> za prenos 2 parametra iz View-a -> Controller
            routes.MapRoute(
                name: "DeletePasCustomRoute",
                url: "Pas/Delete/{vid}/{pid}",
                defaults: new
                {
                    controller = "Pas",
                    action = "Delete",
                    vid = "",
                    pid = ""
                }
                );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
