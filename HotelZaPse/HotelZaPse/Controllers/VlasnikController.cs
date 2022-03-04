using HotelZaPse.Models.BO;
using HotelZaPse.Models.EFRepository;
using HotelZaPse.Models.Interfaces;
using System.Web.Mvc;

namespace HotelZaPse.Controllers
{
    public class VlasnikController : Controller
    {
        #region Fields
        private IVlasnikRepository vlasnikRepository;
        #endregion

        #region Constructors
        public VlasnikController()
        {
            vlasnikRepository = new VlasnikRepository();
        }
        #endregion

        #region Methods
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "vlasnik")]
        public ActionResult ViewProfile()
        {
            VlasnikBo vlasnikBo = vlasnikRepository.GetByMail(User.Identity.Name);
            return View(vlasnikBo);
        }

        [Authorize(Roles = "admin")]
        public ActionResult ViewProfileByAdmin(int id)
        {
            VlasnikBo vlasnikBo = vlasnikRepository.GetById(id);
            return View("ViewProfile", vlasnikBo);
        }

        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registration(VlasnikBo vlasnikBo)
        {
            if (vlasnikRepository.IsMailUnique(vlasnikBo))
            {
                vlasnikRepository.Create(vlasnikBo);
                return RedirectToAction("Login", "Home");
            }
            ModelState.AddModelError("Mail", "Profil sa unetim mail-om već postoji.");
            return View();

        }

        [Authorize(Roles = "vlasnik")]
        public ActionResult Edit()
        {
            VlasnikBo vlasnikBo = vlasnikRepository.GetByMail(User.Identity.Name);

            return View(vlasnikBo);
        }

        [Authorize(Roles = "vlasnik")]
        [HttpPost]
        public ActionResult Edit(VlasnikBo vlasnikBo)
        {
            vlasnikRepository.Edit(vlasnikBo);
            return RedirectToAction("ViewProfile");
        }

        #endregion
    }
}