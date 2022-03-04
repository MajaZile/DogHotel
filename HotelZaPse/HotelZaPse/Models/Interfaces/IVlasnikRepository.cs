using HotelZaPse.Models.BO;

namespace HotelZaPse.Models.Interfaces
{
    public interface IVlasnikRepository
    {

        bool AmIVlasnik(string email, string password);
        bool IsMailUnique(VlasnikBo vlasnikBo);
        void Create(VlasnikBo vlasnikBo);
        VlasnikBo GetByMail(string mail);
        void Edit(VlasnikBo vlasnikBo);
        VlasnikBo GetById(int id);
    }
}
