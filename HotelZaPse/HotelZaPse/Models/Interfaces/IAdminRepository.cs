using HotelZaPse.Models.BO;

namespace HotelZaPse.Models.Interfaces
{
    public interface IAdminRepository
    {

        bool AmIAdmin(string email, string password);
        int GetIdByUser(UserBo user);
        AdminBo GetByMail(string mail);
    }
}
