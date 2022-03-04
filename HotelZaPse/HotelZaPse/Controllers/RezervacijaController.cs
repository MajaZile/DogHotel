using HotelZaPse.Models;
using HotelZaPse.Models.BO;
using HotelZaPse.Models.EFRepository;
using HotelZaPse.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace HotelZaPse.Controllers
{
    public class RezervacijaController : Controller
    {

        #region Fields

        private IRezervacijaRepository rezervacijaRepository;
        private IVlasnikRepository vlasnikRepository;
        private IUslugaRepository uslugaRepository;
        private IPasRepository pasRepository;

        #endregion

        #region Constructors

        public RezervacijaController()
        {
            rezervacijaRepository = new RezervacijaRepository();
            vlasnikRepository = new VlasnikRepository();
            uslugaRepository = new UslugaRepository();
            pasRepository = new PasRepository();
        }

        #endregion


        #region Methods

        // GET: Rezervacija
        [Authorize]
        public ActionResult Index()
        {
            //Lista usluga za DropDownListu
            List<UslugaSelectViewModel> uslugeView = uslugaRepository.GetAll().Select(t => new
            UslugaSelectViewModel {
                Naziv = t.Naziv + " - šifra: " + t.Id,
                UslugaId = t.Id
            }).ToList();
            if (HttpContext.Request.Cookies["LangCookie"].Value == "lang=RS")
                uslugeView.Add(new UslugaSelectViewModel { Naziv = "Sve", UslugaId = 0 });
            else
                uslugeView.Add(new UslugaSelectViewModel { Naziv = "All", UslugaId = 0 });
            ViewBag.Usluge = uslugeView;

            //Slanje rezervacija za PartialView
            if (TempData["rezervacije"] != null)
            {
                ViewBag.Rezervacije = TempData["rezervacije"];
            }

            RezervacijaBo rezervacijaBo = new RezervacijaBo();
            if (TempData["uslugaId"] == null)
            {
                rezervacijaBo.Usluga = new UslugaBo { Id = 0 };
            }
            else
            {
                rezervacijaBo.Usluga = new UslugaBo { Id = (int)TempData["uslugaId"] };
            }

            if (TempData["datum"] != null)
            {
                rezervacijaBo.Prijava = (DateTime)TempData["datum"];
            }
            return View(rezervacijaBo);
        }
        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Index(RezervacijaBo rezervacijaBo)
        {
            IEnumerable<RezervacijaBo> rezervacije = rezervacijaRepository.Filter(rezervacijaBo.Usluga.Id, rezervacijaBo.Prijava);
            TempData["rezervacije"] = rezervacije;
            TempData["uslugaId"] = rezervacijaBo.Usluga.Id;
            TempData["datum"] = rezervacijaBo.Prijava;
            return RedirectToAction("Index");
        }


        [ChildActionOnly]
        public ActionResult GetRezervacije(IEnumerable<RezervacijaBo> rezervacije)
        {
            if (rezervacije == null)
            {
                if (User.IsInRole("admin"))
                {
                    return PartialView("ListaRezervacija", rezervacijaRepository.GetAll());
                }
                else
                {
                    //ako je vlasnik ulogovan prikazuju se samo njegove rezervacije
                    VlasnikBo vlasnikBo = vlasnikRepository.GetByMail(User.Identity.Name);
                    return PartialView("ListaRezervacija", rezervacijaRepository.GetAllByVlasnik(vlasnikBo));
                }
            }
            else
            {
                IEnumerable<RezervacijaBo> rezervacijeBos = rezervacije;
                return PartialView("ListaRezervacija", rezervacijeBos);
            }

        }

        [Authorize(Roles = "vlasnik")]
        public ActionResult PsiCheckboxes()
        {
            PsiCheckboxesViewModel psiCheckboxesViewModel = new PsiCheckboxesViewModel()
            {
                //svi psi u okviru profila ulogovanog vlasnika
                PasList = pasRepository.GetAllByVlasnik(vlasnikRepository.GetByMail(User.Identity.Name))
            };

            //slanje pasa u pogled za checkiranje
            return View(psiCheckboxesViewModel);
        }

        [Authorize(Roles = "vlasnik")]
        [HttpPost]
        public ActionResult PsiCheckboxes(PsiCheckboxesViewModel psiCheckboxes)
        {
            //ako vlasnik nije selektovao nijednog psa ne moze da nastavi sa kreiranjem rezervacije
            if (psiCheckboxes.SelectedPasIds == null)
            {
                ModelState.AddModelError("ZeroSelected", "Morate izabrati bar jednog psa.");

                PsiCheckboxesViewModel psiCheckboxesViewModel = new PsiCheckboxesViewModel()
                {
                    //svi psi u okviru profila ulogovanog vlasnika
                    PasList = pasRepository.GetAllByVlasnik(vlasnikRepository.GetByMail(User.Identity.Name))
                };

                return View("PsiCheckboxes", psiCheckboxesViewModel);
            }

            //niz Id-jeva selektovanih pasa se prosledjuje akcionoj metodi Create()
            TempData["selectedPasIds"] = psiCheckboxes.SelectedPasIds;
            return RedirectToAction("Create");

        }


        [Authorize(Roles = "vlasnik")]
        public ActionResult Create()
        {
            //vlasnik mora pristupiti akcionoj metodi PsiCheckboxes()
            //pre nego sto pristupi akcionoj metodi Create()
            if (TempData["selectedPasIds"] == null)
            {
                return RedirectToAction("PsiCheckboxes", "Rezervacija");
            }

            //niz Id-jeva selektovanih pasa se prosledjuje pogledu Create() -> HIDDEN
            ViewBag.selectedIds = TempData["selectedPasIds"];

            ViewBag.Usluge = FillViewBagUsluge();

            return View();
        }

        [Authorize(Roles = "vlasnik")]
        [HttpPost]
        public ActionResult Create(RezervacijaBo rezervacijaBo)
        {
            //1. datum prijave ne sme biti manji od danasnjeg
            //2. datum odjave ne sme biti manji od danasnjeg
            //3. datum odjave ne sme biti manji od datuma prijave
            //4. iz drop down liste mora biti selektovana usluga (default value = 0)
            /*5. za bilo koji datum iz opsega [prijava,odjava) ne sme biti premasen kapacitet
                 izabrane usluge*/
            /*6. za bilo koji datum iz opsega [prijava,odjava) u bazi ne sme postojati rezervacija
                 za bilo kog od selektovanih pasa*/
            #region Validation

            //1, 2, 3
            if (rezervacijaBo.Prijava < DateTime.Now || rezervacijaBo.Odjava < DateTime.Now || rezervacijaBo.Prijava > rezervacijaBo.Odjava)
            {
                if (rezervacijaBo.Prijava < DateTime.Now)
                {
                    ModelState.AddModelError("Prijava", "Datum prijave ne sme biti manji od današnjeg.");
                }
                if (rezervacijaBo.Odjava < DateTime.Now)
                {
                    ModelState.AddModelError("Odjava", "Datum odjave ne sme biti manji od današnjeg.");
                }
                if (rezervacijaBo.Prijava > rezervacijaBo.Odjava)
                {
                    ModelState.AddModelError("Odjava", "Datum odjave ne sme biti manji od datuma prijave.");
                }
                ViewBag.selectedIds = rezervacijaBo.SelectedPasIds;
                ViewBag.Usluge = FillViewBagUsluge();
                return View();
            }

            //4
            if (rezervacijaBo.Usluga.Id == 0)
            {
                ModelState.AddModelError("Usluga", "Morate izabrati uslugu.");
                ViewBag.selectedIds = rezervacijaBo.SelectedPasIds;
                ViewBag.Usluge = FillViewBagUsluge();
                return View();
            }

            //5
            rezervacijaBo.Usluga = uslugaRepository.GetById(rezervacijaBo.Usluga.Id);

            List<DateTime> overbookedDates = rezervacijaRepository.OverBooked(rezervacijaBo);
            if (overbookedDates.Count() != 0)
            {
                string error = "Izabrana usluga " + rezervacijaBo.Usluga.Naziv;
                error += " je nedostupna za dane: " + Environment.NewLine;
                foreach (var datum in overbookedDates)
                {
                    error += datum.ToString("dd.MM.yy") + Environment.NewLine;
                }
                ModelState.AddModelError("Overbooked", error);

                ViewBag.selectedIds = rezervacijaBo.SelectedPasIds;
                ViewBag.Usluge = FillViewBagUsluge();
                return View();
            }

            //6
            string modelError = "";
            for (int i = 0; i < rezervacijaBo.SelectedPasIds.Count(); i++)
            {
                VlasnikBo vlasnikBo = vlasnikRepository.GetByMail(User.Identity.Name);
                rezervacijaBo.Pas = pasRepository.GetById(vlasnikBo.Id, rezervacijaBo.SelectedPasIds[i]);
                List<DateTime> overlapingDates = rezervacijaRepository.SameDogOverlapingDates(rezervacijaBo);
                if (overlapingDates.Count() != 0)
                {
                    modelError += "Za psa " + rezervacijaBo.Pas.Ime;
                    modelError += " su već rezervisani dani: ";
                    foreach (var datum in overlapingDates)
                    {
                        modelError += datum.ToString("dd.MM.yy") + ", ";
                    }
                    //brisanje poslednja 2 karaktera (", ")
                    modelError = modelError.Remove(modelError.Length - 2);
                    modelError += Environment.NewLine;
                }
            }
            if (modelError != "")
            {
                ModelState.AddModelError("Overlaping", modelError);
                ViewBag.selectedIds = rezervacijaBo.SelectedPasIds;
                ViewBag.Usluge = FillViewBagUsluge();
                return View();
            }

            #endregion

            TempData["rezervacijaBo"] = rezervacijaBo;
            return RedirectToAction("Confirm");

        }

        [Authorize(Roles = "vlasnik")]
        public ActionResult Confirm()
        {
            //vlasnik mora pristupiti akcionoj metodi PsiCheckboxes(), pa akcionoj metodi Create()
            //pre nego sto pristupi akcionoj metodi Confirm()
            if (TempData["rezervacijaBo"] == null)
            {
                return RedirectToAction("PsiCheckboxes", "Rezervacija");
            }
            RezervacijaBo rezervacijaBo = (RezervacijaBo)TempData["rezervacijaBo"];

            int daysSelected = (rezervacijaBo.Odjava.Subtract(rezervacijaBo.Prijava)).Days;
            int dogsSelected = rezervacijaBo.SelectedPasIds.Count();
            ViewBag.UkupnaCena = rezervacijaBo.Usluga.Cena * daysSelected * dogsSelected;

            string dogsString = "";
            VlasnikBo vlasnikBo = vlasnikRepository.GetByMail(User.Identity.Name);
            foreach (var dogId in rezervacijaBo.SelectedPasIds)
            {
                dogsString += (pasRepository.GetById(vlasnikBo.Id, dogId)).Ime + ", ";
            }
            //brisanje poslednja 2 karaktera (", ")
            dogsString = dogsString.Remove(dogsString.Length - 2);
            ViewBag.dogsString = dogsString;

            return View(rezervacijaBo);
        }

        [Authorize(Roles = "vlasnik")]
        [HttpPost]
        public ActionResult Confirm(RezervacijaBo rezervacijaBo)
        {
            rezervacijaBo.Usluga = uslugaRepository.GetById(rezervacijaBo.Usluga.Id);

            for (int i = 0; i < rezervacijaBo.SelectedPasIds.Count(); i++)
            {
                VlasnikBo vlasnikBo = vlasnikRepository.GetByMail(User.Identity.Name);
                rezervacijaBo.Pas = pasRepository.GetById(vlasnikBo.Id, rezervacijaBo.SelectedPasIds[i]);
                rezervacijaRepository.Create(rezervacijaBo);
            }


            return RedirectToAction("Index");
        }


        [Authorize(Roles = "vlasnik, admin")]
        public ActionResult Delete(int id)
        {
            RezervacijaBo rezervacijaBo = rezervacijaRepository.GetById(id);
            return View(rezervacijaBo);
        }

        [Authorize(Roles = "vlasnik, admin")]
        [HttpPost]
        public ActionResult Delete(RezervacijaBo rezervacijaBo)
        {
            rezervacijaRepository.Delete(rezervacijaBo.Id);
            return RedirectToAction("Index");
        }

        #endregion

        #region HelperMethods

        private List<UslugaSelectViewModel> FillViewBagUsluge()
        {
            //Lista usluga za DropDownListu
            List<UslugaSelectViewModel> uslugeView = new List<UslugaSelectViewModel>();
            if (HttpContext.Request.Cookies["LangCookie"].Value == "lang=RS")
                uslugeView.Add(new UslugaSelectViewModel { Naziv = "Izaberite uslugu", UslugaId = 0 });
            else
                uslugeView.Add(new UslugaSelectViewModel { Naziv = "Choose a service", UslugaId = 0 });
            foreach (var usluga in uslugaRepository.GetAllActive())
            {
                uslugeView.Add(new UslugaSelectViewModel { Naziv = usluga.Naziv, UslugaId = usluga.Id });
            }
            return uslugeView;
        }

        #endregion
    }
}