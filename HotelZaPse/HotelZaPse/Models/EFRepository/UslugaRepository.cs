using HotelZaPse.Models.BO;
using HotelZaPse.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;



namespace HotelZaPse.Models.EFRepository
{
    public class UslugaRepository : IUslugaRepository
    {

        #region Fields
        private HotelZaPseDBEntities entities = new HotelZaPseDBEntities();
        #endregion

        #region Methods
        public void Create(UslugaBo uslugaBo)
        {
            UslugaModel uslugaModel = new UslugaModel();
            uslugaModel.naziv = uslugaBo.Naziv;
            uslugaModel.cena = uslugaBo.Cena;
            uslugaModel.opis = uslugaBo.Opis;
            uslugaModel.kapacitet = uslugaBo.Kapacitet;
            uslugaModel.aktivna = uslugaBo.Aktivna;
            uslugaModel.adminID = uslugaBo.Admin.Id;

            entities.UslugaModels.Add(uslugaModel);
            try
            {
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public void Delete(int id, int adminID)
        {
            UslugaModel uslugaModel = entities.UslugaModels.Single(t => t.uslugaID == id);

            //ako je usluga koriscena u nekoj rezervaciji
            if (entities.RezervacijaModels.Any(t => t.uslugaID == uslugaModel.uslugaID))
            {
                //logicko brisanje
                uslugaModel.aktivna = false;
                uslugaModel.adminID = adminID;
            }
            else
            {
                //fizicko brisanje
                entities.UslugaModels.Remove(uslugaModel);
            }

            try
            {
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public void Edit(UslugaBo uslugaBo)
        {
            UslugaModel uslugaModel = entities.UslugaModels.Single(t => t.uslugaID == uslugaBo.Id);
            uslugaModel.naziv = uslugaBo.Naziv;
            uslugaModel.cena = uslugaBo.Cena;
            uslugaModel.opis = uslugaBo.Opis;
            uslugaModel.kapacitet = uslugaBo.Kapacitet;
            uslugaModel.aktivna = uslugaBo.Aktivna;
            uslugaModel.adminID = uslugaBo.Admin.Id;

            try
            {
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public IEnumerable<UslugaBo> GetAll()
        {
            List<UslugaBo> usluge = new List<UslugaBo>();
            foreach (UslugaModel uslugaModel in entities.UslugaModels)
            {
                usluge.Add(Map(uslugaModel));
            }
            return usluge;
        }

        public UslugaBo GetById(int id)
        {
            try
            {
                UslugaModel uslugaModel = entities.UslugaModels.Single(t => t.uslugaID == id);
                return Map(uslugaModel);
            }
            //jedini put kada ce metoda biti pozvana sa prosledjenim id=0
            //ako prilikom Rezervacija/Create ne bude izabrana nijedna usluga
            catch
            {
                return null;
            }
            
        }

        public IEnumerable<UslugaBo> GetAllActive()
        {
            List<UslugaBo> usluge = new List<UslugaBo>();
            foreach (UslugaModel uslugaModel in entities.UslugaModels.Where(t => t.aktivna == true))
            {
                usluge.Add(Map(uslugaModel));
            }
            return usluge;
        }

        public bool InvalidNameCreate(UslugaBo uslugaBo)
        {
            if (uslugaBo.Aktivna)
            {
                //u bazi ne mogu postojati dve usluge sa istim imenom, a pritom da su obe aktivne
                return entities.UslugaModels.Any(t => t.naziv == uslugaBo.Naziv && t.aktivna == true);
            }
            else return false;

        }
        public bool InvalidNameEdit(UslugaBo uslugaBo)
        {
            if (uslugaBo.Aktivna)
            {
                return entities.UslugaModels.Any(t => t.naziv == uslugaBo.Naziv && t.aktivna == true && t.uslugaID != uslugaBo.Id);
            }
            else return false;

        }


        public bool IsUsed(UslugaBo uslugaBo)
        {
            return entities.RezervacijaModels.Any(t => t.uslugaID == uslugaBo.Id);
        }

        #endregion

        #region HelperMethods

        private UslugaBo Map(UslugaModel uslugaModel)
        {
            UslugaBo uslugaBo = new UslugaBo();
            uslugaBo.Id = uslugaModel.uslugaID;
            uslugaBo.Cena = uslugaModel.cena;
            uslugaBo.Kapacitet = uslugaModel.kapacitet;
            uslugaBo.Naziv = uslugaModel.naziv;
            uslugaBo.Aktivna = uslugaModel.aktivna;
            uslugaBo.Opis = uslugaModel.opis;
            uslugaBo.Admin = new AdminBo()
            {
                Id = uslugaModel.adminID,
                Ime = uslugaModel.Admin.ime,
                Prezime = uslugaModel.Admin.prezime,
                Mail = uslugaModel.Admin.mail,
                Lozinka = uslugaModel.Admin.lozinka
            };

            return uslugaBo;
        }

        #endregion
    }
}