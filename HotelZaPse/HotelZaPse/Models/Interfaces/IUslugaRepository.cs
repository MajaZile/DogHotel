using HotelZaPse.Models.BO;
using System.Collections.Generic;

namespace HotelZaPse.Models.Interfaces
{
    public interface IUslugaRepository
    {

        void Create(UslugaBo uslugaBo);
        IEnumerable<UslugaBo> GetAll();
        UslugaBo GetById(int id);
        void Edit(UslugaBo uslugaBo);
        void Delete(int id, int adminId);
        IEnumerable<UslugaBo> GetAllActive();
        bool InvalidNameCreate(UslugaBo uslugaBo);
        bool InvalidNameEdit(UslugaBo uslugaBo);
        bool IsUsed(UslugaBo uslugaBo);


    }
}
