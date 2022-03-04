using HotelZaPse.Models.BO;
using HotelZaPse.Models.EFRepository;
using HotelZaPse.Models.Interfaces;
using System.Web.Mvc;

namespace HotelZaPse.Controllers
{
    public class AdminController : Controller
    {

        #region Fields
        private IAdminRepository adminRepository;
        #endregion

        #region Constructors
        public AdminController()
        {
            adminRepository = new AdminRepository();
        }
        #endregion

        #region Methods

        [Authorize(Roles = "admin")]
        public ActionResult ViewProfile()
        {
            AdminBo adminBo = adminRepository.GetByMail(User.Identity.Name);
            return View(adminBo);
        }

        #endregion
    }
}