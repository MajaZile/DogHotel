using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HotelZaPse.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            var x = Response.StatusCode;
            return View();
        }

        //404
        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View();
        }

    }
}