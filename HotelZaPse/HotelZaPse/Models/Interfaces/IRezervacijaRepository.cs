using HotelZaPse.Models.BO;
using System;
using System.Collections.Generic;

namespace HotelZaPse.Models.Interfaces
{
    interface IRezervacijaRepository
    {
        IEnumerable<RezervacijaBo> GetAll();
        IEnumerable<RezervacijaBo> GetAllByVlasnik(VlasnikBo vlasnikBo);
        IEnumerable<RezervacijaBo> Filter(int idUsluga, DateTime datumBoravka);
        void Delete(int id);
        void Create(RezervacijaBo rezervacijaBo);
        List<DateTime> OverBooked(RezervacijaBo rezervacijaBo);
        List<DateTime> SameDogOverlapingDates(RezervacijaBo rezervacijaBo);
        RezervacijaBo GetById(int id);
    }
}
