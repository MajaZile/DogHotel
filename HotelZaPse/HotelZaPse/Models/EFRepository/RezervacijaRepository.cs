using HotelZaPse.Models.BO;
using HotelZaPse.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelZaPse.Models.EFRepository
{

    public class RezervacijaRepository : IRezervacijaRepository
    {
        #region Fields
        private HotelZaPseDBEntities entities = new HotelZaPseDBEntities();
        #endregion

        #region Methods

        public IEnumerable<RezervacijaBo> GetAll()
        {
            List<RezervacijaBo> rezervacije = new List<RezervacijaBo>();
            foreach (RezervacijaModel rezervacijaModel in entities.RezervacijaModels)
            {
                rezervacije.Add(Map(rezervacijaModel));
            }
            return rezervacije;
        }

        public IEnumerable<RezervacijaBo> GetAllByVlasnik(VlasnikBo vlasnikBo)
        {
            List<RezervacijaBo> rezervacije = new List<RezervacijaBo>();
            foreach (RezervacijaModel rezervacijaModel in entities.RezervacijaModels.Where(t => t.vlasnikID == vlasnikBo.Id))
            {
                rezervacije.Add(Map(rezervacijaModel));
            }
            return rezervacije;
        }

        public IEnumerable<RezervacijaBo> Filter(int idUsluga, DateTime datumBoravka)
        {
            List<RezervacijaBo> rezervacije = new List<RezervacijaBo>();

            DateTime date = new DateTime();

            if (idUsluga == 0 && datumBoravka == date)
            {
                foreach (RezervacijaModel rezervacijaModel in entities.RezervacijaModels)
                {
                    rezervacije.Add(Map(rezervacijaModel));
                }
            }
            else if (datumBoravka == date)
            {
                foreach (RezervacijaModel rezervacijaModel in entities.RezervacijaModels.Where(t => t.uslugaID == idUsluga))
                {
                    rezervacije.Add(Map(rezervacijaModel));
                }
            }
            else if (idUsluga == 0)
            {
                foreach (RezervacijaModel rezervacijaModel in entities.RezervacijaModels.Where(t=> datumBoravka >= t.datumPrijavljivanja && datumBoravka < t.datumOdjavljivanja))
                {
                    rezervacije.Add(Map(rezervacijaModel));
                }
            }
            else
            {
                foreach (RezervacijaModel rezervacijaModel in entities.RezervacijaModels.Where(t => t.uslugaID == idUsluga && datumBoravka >= t.datumPrijavljivanja && datumBoravka < t.datumOdjavljivanja))
                {
                    rezervacije.Add(Map(rezervacijaModel));
                }
            } 
            return rezervacije;
        }

        public void Delete(int id)
        {
            RezervacijaModel rezervacijaModel = entities.RezervacijaModels.Single(t => t.rezervacijaID == id);

            entities.RezervacijaModels.Remove(rezervacijaModel);

            try
            {
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public void Create(RezervacijaBo rezervacijaBo)
        {
            RezervacijaModel rezervacijaModel = new RezervacijaModel();
            rezervacijaModel.datumPrijavljivanja = rezervacijaBo.Prijava;
            rezervacijaModel.datumOdjavljivanja = rezervacijaBo.Odjava;
            rezervacijaModel.napomena = rezervacijaBo.Napomena;
            rezervacijaModel.uslugaID = rezervacijaBo.Usluga.Id;
            rezervacijaModel.vlasnikID = rezervacijaBo.Pas.Vlasnik.Id;
            rezervacijaModel.pasID = rezervacijaBo.Pas.Id;

            int daysSelected = (rezervacijaBo.Odjava.Subtract(rezervacijaBo.Prijava)).Days;
            rezervacijaModel.cena = daysSelected * rezervacijaBo.Usluga.Cena;

            entities.RezervacijaModels.Add(rezervacijaModel);
            try
            {
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public List<DateTime> OverBooked(RezervacijaBo rezervacijaBo)
        {
            List<DateTime> overBookedDates = new List<DateTime>();

            int daysSelected = (rezervacijaBo.Odjava.Subtract(rezervacijaBo.Prijava)).Days;
            int dogsSelected = rezervacijaBo.SelectedPasIds.Count();

            //proverava se raspolozivost za svaki dan iz opsega IzabraniDatumPrijave - IzabraniDatumOdjave
            for (int i = 0; i < daysSelected; i++)
            {
                DateTime day = rezervacijaBo.Prijava.AddDays(i);

                //prebroj koliko rezervacija postoji za tacno taj dan za selektovanu uslugu
                int existingReservationsForDate =
                    entities.RezervacijaModels.Where(t => t.uslugaID == rezervacijaBo.Usluga.Id
                    && day >= t.datumPrijavljivanja && day < t.datumOdjavljivanja).Count();


                if (existingReservationsForDate + dogsSelected > rezervacijaBo.Usluga.Kapacitet)
                {
                    overBookedDates.Add(day);
                }

            }

            return overBookedDates;
        }

        public List<DateTime> SameDogOverlapingDates(RezervacijaBo rezervacijaBo)
        {
            List<DateTime> overlapingDates = new List<DateTime>();

            int daysSelected = (rezervacijaBo.Odjava.Subtract(rezervacijaBo.Prijava)).Days;

            //proverava se da li za psa vec postoji rezervacija
            //za bilo koji datum iz opsega IzabraniDatumPrijave - IzabraniDatumOdjave
            for (int i = 0; i < daysSelected; i++)
            {
                DateTime day = rezervacijaBo.Prijava.AddDays(i);

                bool reservationAlreadyExistsForDate = entities.RezervacijaModels.Any(
                    t => t.vlasnikID == rezervacijaBo.Pas.Vlasnik.Id &&
                    t.pasID == rezervacijaBo.Pas.Id &&
                    day >= t.datumPrijavljivanja && day < t.datumOdjavljivanja);

                if (reservationAlreadyExistsForDate)
                    overlapingDates.Add(day);

            }

            return overlapingDates;
        }

        public RezervacijaBo GetById(int id)
        {
            RezervacijaModel rezervacijaModel = entities.RezervacijaModels.Single(t => t.rezervacijaID == id);
            return Map(rezervacijaModel);
        }

        #endregion

        #region HelperMethods

        private RezervacijaBo Map(RezervacijaModel rezervacijaModel)
        {
            RezervacijaBo rezervacijaBo = new RezervacijaBo();
            rezervacijaBo.Id = rezervacijaModel.rezervacijaID;
            rezervacijaBo.Prijava = rezervacijaModel.datumPrijavljivanja;
            rezervacijaBo.Odjava = rezervacijaModel.datumOdjavljivanja;
            rezervacijaBo.Napomena = rezervacijaModel.napomena;
            rezervacijaBo.Cena = rezervacijaModel.cena;

            rezervacijaBo.Usluga = new UslugaBo()
            {
                Id = rezervacijaModel.Usluga.uslugaID,
                Cena = rezervacijaModel.Usluga.cena,
                Kapacitet = rezervacijaModel.Usluga.kapacitet,
                Naziv = rezervacijaModel.Usluga.naziv,
                Aktivna = rezervacijaModel.Usluga.aktivna,
                Opis = rezervacijaModel.Usluga.opis,
                Admin = new AdminBo()
                {
                    Id = rezervacijaModel.Usluga.Admin.adminID,
                    Ime = rezervacijaModel.Usluga.Admin.ime,
                    Prezime = rezervacijaModel.Usluga.Admin.prezime,
                    Mail = rezervacijaModel.Usluga.Admin.mail,
                    Lozinka = rezervacijaModel.Usluga.Admin.lozinka
                }
            };

            rezervacijaBo.Pas = new PasBo()
            {
                Id = rezervacijaModel.Pas.pasID,
                Ime = rezervacijaModel.Pas.imePsa,
                Rasa = rezervacijaModel.Pas.rasa,
                Pol = rezervacijaModel.Pas.pol,
                Sterilisan = rezervacijaModel.Pas.sterilisan,
                Opis = rezervacijaModel.Pas.opis,
                Obrisan = rezervacijaModel.Pas.obrisan,
                Vlasnik = new VlasnikBo()
                {
                    Id = rezervacijaModel.Pas.Vlasnik.vlasnikID,
                    Ime = rezervacijaModel.Pas.Vlasnik.ime,
                    Prezime = rezervacijaModel.Pas.Vlasnik.prezime,
                    Adresa = rezervacijaModel.Pas.Vlasnik.adresa,
                    Telefon = rezervacijaModel.Pas.Vlasnik.telefon,
                    Mail = rezervacijaModel.Pas.Vlasnik.mail,
                    Lozinka = rezervacijaModel.Pas.Vlasnik.lozinka
                }
            };

            return rezervacijaBo;
        }

        #endregion
    }
}