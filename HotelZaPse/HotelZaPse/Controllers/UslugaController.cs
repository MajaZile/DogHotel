using HotelZaPse.Models.BO;
using HotelZaPse.Models.EFRepository;
using HotelZaPse.Models.Interfaces;
using System.Web.Mvc;

namespace HotelZaPse.Controllers
{
    public class UslugaController : Controller
    {
        #region Fields

        private IUslugaRepository uslugaRepository;
        private IAdminRepository adminRepository;

        #endregion

        #region Constructors

        public UslugaController()
        {
            uslugaRepository = new UslugaRepository();
            adminRepository = new AdminRepository();
        }

        #endregion

        #region Methods

        // GET: Usluga

        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult GetUsluge()
        {
            if (User.IsInRole("admin"))
            {
                return PartialView("ListaUsluga", uslugaRepository.GetAll());
            }

            return PartialView("ListaUsluga", uslugaRepository.GetAllActive());
        }

        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult Create(UslugaBo uslugaBo)
        {
            //admin koji je poslednji izmenio uslugu (trenutno ulogovan)
            uslugaBo.Admin = adminRepository.GetByMail(User.Identity.Name);

            if (uslugaRepository.InvalidNameCreate(uslugaBo))
            {
                ModelState.AddModelError("Naziv", "Usluga sa datim imenom već postoji (i aktivna je).");
                return View();
            }

            uslugaRepository.Create(uslugaBo);
            return View("Index");
        }

        [Authorize(Roles = "admin")]
        public ActionResult Edit(int id)
        {
            UslugaBo uslugaBo = uslugaRepository.GetById(id);

            ViewBag.Used = uslugaRepository.IsUsed(uslugaBo);

            return View(uslugaBo);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult Edit(UslugaBo uslugaBo)
        {
            //admin koji je poslednji izmenio uslugu (trenutno ulogovan)
            uslugaBo.Admin = adminRepository.GetByMail(User.Identity.Name);

            if (uslugaRepository.InvalidNameEdit(uslugaBo))
            {
                ModelState.AddModelError("Naziv", "Usluga sa datim imenom već postoji (i aktivna je).");
                return View();//uslugaBo.Id
            }

            uslugaRepository.Edit(uslugaBo);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "admin")]
        public ActionResult Delete(int id)
        {
            UslugaBo uslugaBo = uslugaRepository.GetById(id);
            return View(uslugaBo);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult Delete(UslugaBo uslugaBo)
        {
            //pamti se admin koji je poslednji izmenio uslugu
            //ovde u slucaju logickog brisanja
            AdminBo adminBo = adminRepository.GetByMail(User.Identity.Name);

            uslugaRepository.Delete(uslugaBo.Id, adminBo.Id);
            return View("Index");
        }

        //[Authorize(Roles = "admin, vlasnik")]
        public ActionResult ViewUsluga(int id)
        {
            UslugaBo uslugaBo = uslugaRepository.GetById(id);
            return View(uslugaBo);
        }

        [Authorize(Roles = "vlasnik")]
        public ActionResult UslugaDetails(int id)
        {
            if (id == 0)
            {
                ViewBag.IdIsZero = true;
            }
            else
            {
                ViewBag.IdIsZero = false;
            }
            return PartialView("UslugaDetails", uslugaRepository.GetById(id));
        }

        #endregion

    }
}