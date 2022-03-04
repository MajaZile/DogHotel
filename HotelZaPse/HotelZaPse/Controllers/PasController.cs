using HotelZaPse.Models.BO;
using HotelZaPse.Models.EFRepository;
using HotelZaPse.Models.Interfaces;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace HotelZaPse.Controllers
{
    public class PasController : Controller
    {

        #region Fields

        private IPasRepository pasRepository;
        private IVlasnikRepository vlasnikRepository;

        #endregion

        #region Constructors

        public PasController()
        {
            pasRepository = new PasRepository();
            vlasnikRepository = new VlasnikRepository();
        }

        #endregion

        #region Methods

        [ChildActionOnly]
        public ActionResult GetPsi()
        {
            VlasnikBo vlasnikBo = vlasnikRepository.GetByMail(User.Identity.Name);
            return PartialView("ListaPasa", pasRepository.GetAllByVlasnik(vlasnikBo));
        }

        [Authorize(Roles = "vlasnik")]
        public ActionResult Create()
        {
            //u zavisnosti odakle je pozvana Pas/Create ce nakon uspesnog kreiranja psa biti Redirected
            //ili na Vlasnik/Edit ili na Rezervacija/PsiCheckBoxes
            ViewBag.UrlReferrer = HttpContext.Request.UrlReferrer.ToString();

            if (HttpContext.Request.Cookies["LangCookie"].Value == "lang=RS")
                ViewBag.rase = PasRepository.GetRase();
            else
                ViewBag.rase = PasRepository.GetBreeds();
            return View();
        }

        [Authorize(Roles = "vlasnik")]
        [HttpPost]
        public ActionResult Create(PasBo pasBo)
        {
            //vlasnik koji je kreirao psa (trenutno ulogovan)
            pasBo.Vlasnik = vlasnikRepository.GetByMail(User.Identity.Name);

            if (pasRepository.InvalidNameCreate(pasBo))
            {
                ModelState.AddModelError("Ime", "Već imate psa sa unetim imenom.");
                if (HttpContext.Request.Cookies["LangCookie"].Value == "lang=RS")
                    ViewBag.rase = PasRepository.GetRase();
                else
                    ViewBag.rase = PasRepository.GetBreeds();
                return View();
            }

            pasRepository.Create(pasBo);


            //ako je Pas/Create pozvana u okviru kreiranja rezervacije
            string UrlReferrer = Request.Form["urlReferrer"];
            if (UrlReferrer.Contains("Rezervacija/PsiCheckboxes"))
            {
                return RedirectToAction("PsiCheckboxes", "Rezervacija");
            }
            //ako je Pas/Create pozvana u okviru ViewProfile
            else if (UrlReferrer.Contains("Vlasnik/ViewProfile"))
            {
                return RedirectToAction("ViewProfile", "Vlasnik");
            }
            //ako je Pas/Create pozvana u okviru editovanja profila
            return RedirectToAction("Edit", "Vlasnik");

        }

        [Authorize(Roles = "vlasnik")]
        public ActionResult Edit(int vid, int pid)
        {
            //pas ima kompozitni kljuc (vlasnikId & pasId)
            PasBo pasBo = pasRepository.GetById(vid, pid);

            ViewBag.UrlReferrer = HttpContext.Request.UrlReferrer.ToString();

            return View(pasBo);
        }

        [Authorize(Roles = "vlasnik")]
        [HttpPost]
        public ActionResult Edit(PasBo pasBo)
        {
            if (pasRepository.InvalidNameEdit(pasBo))
            {
                ModelState.AddModelError("Ime", "Već imate psa sa unetim imenom.");
                return View(pasBo);
            }

            pasRepository.Edit(pasBo);

            //ako je Pas/Edit pozvana u okviru Vlasnik/ViewProfile
            string UrlReferrer = Request.Form["urlReferrer"];
            if (UrlReferrer.Contains("Vlasnik/ViewProfile"))
            {
                return RedirectToAction("ViewProfile", "Vlasnik");
            }
            //ako je Pas/Create pozvana u okviru Vlasnik/Edit
            return RedirectToAction("Edit", "Vlasnik");
        }

        [Authorize(Roles = "vlasnik")]
        public ActionResult Delete(int vid, int pid)
        {
            //u zavisnosti odakle je pozvana Pas/Delete ce nakon uspesnog kreiranja psa biti Redirected
            //ili na Vlasnik/Edit ili na Vlasnik/ViewProfile
            ViewBag.UrlReferrer = HttpContext.Request.UrlReferrer.ToString();

            PasBo pasBo = pasRepository.GetById(vid, pid);
            //ako vlasnik pokusa da obrise psa koji ima buduce rezervacije, bice obavesten
            //da se brisanjem psa automatski otkazuju sve buduce rezervacije za tog psa
            int futureReservations = pasRepository.FutureReservations(pasBo);
            ViewBag.FutureReservations = futureReservations;

            return View(pasBo);
        }

        [Authorize(Roles = "vlasnik")]
        [HttpPost]
        public ActionResult Delete(PasBo pasBo)
        {

            pasRepository.Delete(pasBo.Vlasnik.Id, pasBo.Id);

            //ako je Pas/Delete pozvana u okviru Vlasnik/ViewProfile
            string UrlReferrer = Request.Form["urlReferrer"];
            if (UrlReferrer.Contains("Vlasnik/ViewProfile"))
            {
                return RedirectToAction("ViewProfile", "Vlasnik");
            }
            //ako je Pas/Create pozvana u okviru Vlasnik/Edit
            return RedirectToAction("Edit", "Vlasnik");

        }

        [Authorize(Roles = "admin, vlasnik")]
        public ActionResult ViewProfileByAdmin(int vid = 0, int pid = 0)
        {
            PasBo pasBo = pasRepository.GetById(vid, pid);
            return View("ViewPas", pasBo);
        }

        #endregion
    }
}