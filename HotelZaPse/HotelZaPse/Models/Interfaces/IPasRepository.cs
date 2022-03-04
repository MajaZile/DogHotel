using HotelZaPse.Models.BO;
using System.Collections.Generic;


namespace HotelZaPse.Models.Interfaces
{
    interface IPasRepository
    {

        IEnumerable<PasBo> GetAllByVlasnik(VlasnikBo vlasnikBo);
        void Create(PasBo pasBo);
        PasBo GetById(int vlasnikId, int pasId);
        void Edit(PasBo pasBo);
        void Delete(int vlasnikId, int pasId);
        bool InvalidNameCreate(PasBo pasBo);
        bool InvalidNameEdit(PasBo pasBo);
        int FutureReservations(PasBo pasBo);

    }
}
