using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using System.Web;



namespace HotelZaPse
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            

        }
        protected void Session_Start()
        {
            if (this.Context.Request.Cookies["LangCookie"] == null)
            {
                HttpCookie httpCookie = new HttpCookie("LangCookie");
                httpCookie.Values.Add("lang", "EN");
                Response.Cookies.Add(httpCookie);
            }
        }
    }
}
