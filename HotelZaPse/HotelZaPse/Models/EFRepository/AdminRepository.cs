using HotelZaPse.Models.BO;
using HotelZaPse.Models.Interfaces;
using System;
using System.Linq;

namespace HotelZaPse.Models.EFRepository
{
    public class AdminRepository : IAdminRepository
    {
        #region Fields
        private HotelZaPseDBEntities entities = new HotelZaPseDBEntities();
        #endregion

        #region Methods
        public bool AmIAdmin(string email, string password)
        {
            bool amIAdmin = false;
            try
            {
                AdminModel admin = entities.AdminModels.Single(t => (t.mail == email && t.lozinka == password));
                bool sameMail = String.Equals(admin.mail, email, StringComparison.Ordinal);
                bool samePassword = String.Equals(admin.lozinka, password, StringComparison.Ordinal);
                if(sameMail && samePassword)
                    amIAdmin = true;
            }
            catch
            {
            }
            return amIAdmin;
        }

        public int GetIdByUser(UserBo user)
        {
            AdminModel admin = entities.AdminModels.Single(v => (v.mail == user.Email && v.lozinka == user.Password));
            return admin.adminID;
        }

        public AdminBo GetByMail(string mail)
        {
            return Map(entities.AdminModels.Single(t => t.mail == mail));
        }

        #endregion

        #region HelperMethods

        private AdminBo Map(AdminModel adminModel)
        {
            AdminBo adminBo = new AdminBo()
            {
                Id = adminModel.adminID,
                Ime = adminModel.ime,
                Prezime = adminModel.prezime,
                Mail = adminModel.mail,
                Lozinka = adminModel.lozinka
            };
            return adminBo;
        }

        #endregion
    }
}