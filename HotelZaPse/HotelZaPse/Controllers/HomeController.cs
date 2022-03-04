using HotelZaPse.Models.BO;
using HotelZaPse.Models.EFRepository;
using HotelZaPse.Models.Interfaces;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace HotelZaPse.Controllers
{
    public class HomeController : Controller
    {
        #region Fields
        private IVlasnikRepository vlasnikRepository;
        private IAdminRepository adminRepository;
        #endregion

        #region Constructors
        public HomeController()
        {
            vlasnikRepository = new VlasnikRepository();
            adminRepository = new AdminRepository();
        }
        #endregion

        #region Methods
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
      
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return View("Index");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
     
        [HttpPost]
        public ActionResult Login(UserBo user, string returnUrl)
        {
            //proverava da li user postoji u bazi
            //ako user postoji, setuje rolu na "vlasnik" ili "admin" u zavisnosti od unetog email-a 
            user.setRole();

            //smestanje role u kolacic kojem ce kasnije pristupati CustomRoleProvider
            HttpCookie httpCookie = new HttpCookie("AdditionalCookie");
            httpCookie.Values.Add("role", user.Role);

            Response.Cookies.Add(httpCookie);
            FormsAuthentication.SetAuthCookie(user.Email, false);


            if (user.Role == "vlasnik" || user.Role == "admin")
            {
                //ako je requested neka strana, nakon logovanja redirekcija na request
                if (returnUrl == null) return RedirectToAction("Index", "Home");
                //ako nije requested (user se samo loguje)
                else return Redirect(returnUrl);
            }

            ModelState.AddModelError("", "Pogrešan mail ili lozinka.");
            return View();
        }


        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }

        public ActionResult Language(string lang, string returnUrl)
        {
            HttpCookie cookie = HttpContext.Request.Cookies["LangCookie"];
            cookie.Values["lang"] = lang;
            Response.Cookies.Set(cookie);

            return Redirect(returnUrl);
        }
        #endregion
    }
}