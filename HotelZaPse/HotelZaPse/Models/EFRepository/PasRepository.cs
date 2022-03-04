using HotelZaPse.Models.BO;
using HotelZaPse.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelZaPse.Models.EFRepository
{
    public class PasRepository : IPasRepository
    {
        #region Fields
        private HotelZaPseDBEntities entities = new HotelZaPseDBEntities();
        #endregion

        #region Methods

        public IEnumerable<PasBo> GetAllByVlasnik(VlasnikBo vlasnikBo)
        {
            List<PasBo> psi = new List<PasBo>();
            foreach (PasModel pasModel in entities.PasModels.Where(t => t.vlasnikID == vlasnikBo.Id && t.obrisan == false))
            {
                psi.Add(Map(pasModel));
            }
            return psi;
        }

        public void Create(PasBo pasBo)
        {
            PasModel pasModel = new PasModel();
            pasModel.imePsa = pasBo.Ime;
            pasModel.rasa = pasBo.Rasa;
            pasModel.pol = pasBo.Pol;
            pasModel.sterilisan = pasBo.Sterilisan;
            pasModel.opis = pasBo.Opis;
            pasModel.vlasnikID = pasBo.Vlasnik.Id;
            pasModel.obrisan = false;

            pasModel.pasID = GenerateKey(pasBo.Vlasnik);

            entities.PasModels.Add(pasModel);
            try
            {
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public PasBo GetById(int vlasnikId, int pasId)
        {
            PasModel pasModel = entities.PasModels.Single(t => t.vlasnikID == vlasnikId && t.pasID == pasId);
            return Map(pasModel);
        }

        public void Edit(PasBo pasBo)
        {
            PasModel pasModel = entities.PasModels.Single(t => t.vlasnikID == pasBo.Vlasnik.Id && t.pasID == pasBo.Id);
            pasModel.imePsa = pasBo.Ime;
            pasModel.rasa = pasBo.Rasa;
            pasModel.pol = pasBo.Pol;
            pasModel.sterilisan = pasBo.Sterilisan;
            pasModel.opis = pasBo.Opis;

            try
            {
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public void Delete(int vlasnikId, int pasId)
        {
            PasModel pasModel = entities.PasModels.Single(t => t.vlasnikID == vlasnikId && t.pasID == pasId);

            //automatski se otkazuju sve buduce rezervacije za psa
            DateTime now = DateTime.Now.AddDays(1);
            foreach (var r in entities.RezervacijaModels.
                Where(t => t.pasID == pasModel.pasID && t.vlasnikID == pasModel.vlasnikID
                && t.datumPrijavljivanja > now))
            {
                entities.RezervacijaModels.Remove(r);
            }

            //ako je pas koriscen u nekoj rezervaciji
            if (entities.RezervacijaModels.Any(t => t.vlasnikID == vlasnikId && t.pasID == pasId))
            {
                //logicko brisanje
                pasModel.obrisan = true;
            }
            else
            {
                //fizicko brisanje
                entities.PasModels.Remove(pasModel);
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

        public int FutureReservations(PasBo pasBo)
        {
            DateTime now = DateTime.Now.AddDays(1);

            int numFutureReservations = 0;
            foreach (var r in entities.RezervacijaModels.
                Where(t => t.pasID == pasBo.Id && t.vlasnikID == pasBo.Vlasnik.Id
                && t.datumPrijavljivanja > now))
            {
                numFutureReservations++;
            }
            return numFutureReservations;
        }

        public bool InvalidNameCreate(PasBo pasBo)
        {
            return entities.PasModels.Any(t => t.vlasnikID == pasBo.Vlasnik.Id && t.imePsa == pasBo.Ime && t.obrisan == false);

        }

        public bool InvalidNameEdit(PasBo pasBo)
        {
            return entities.PasModels.Any(t => t.vlasnikID == pasBo.Vlasnik.Id && t.imePsa == pasBo.Ime && t.obrisan == false && t.pasID != pasBo.Id);
        }

        #endregion

        #region HelperMethods

        private PasBo Map(PasModel pasModel)
        {
            PasBo pasBo = new PasBo();
            pasBo.Id = pasModel.pasID;
            pasBo.Ime = pasModel.imePsa;
            pasBo.Rasa = pasModel.rasa;
            pasBo.Pol = pasModel.pol;
            pasBo.Sterilisan = pasModel.sterilisan;
            pasBo.Opis = pasModel.opis;
            pasBo.Obrisan = pasModel.obrisan;

            pasBo.Vlasnik = new VlasnikBo
            {
                Id = pasModel.Vlasnik.vlasnikID,
                Ime = pasModel.Vlasnik.ime,
                Prezime = pasModel.Vlasnik.prezime,
                Adresa = pasModel.Vlasnik.adresa,
                Telefon = pasModel.Vlasnik.telefon,
                Mail = pasModel.Vlasnik.mail,
                Lozinka = pasModel.Vlasnik.lozinka
            };

            return pasBo;
        }

        private int GenerateKey(VlasnikBo vlasnikBo)
        {
            int max;
            try
            {
                //nadji psa ulogovanog vlasnika koji ima max id
                max = entities.PasModels.Where(t => t.vlasnikID == vlasnikBo.Id).Select(p => p.pasID).Max();
            }
            catch
            {
                //ako vlasnik nema nijednog psa u bazi 
                max = 0;
            }

            return max + 1;
        }

        public static List<string> GetRase()
        {

            List<string> rase = new List<string>();
            rase.Add("Bernandinac");
            rase.Add("Bigl");
            rase.Add("Doberman");
            rase.Add("Engleski buldog");
            rase.Add("Francuski buldog");
            rase.Add("Hrt");
            rase.Add("Jorkširski terijer");
            rase.Add("Koker španijel");
            rase.Add("Labrador");
            rase.Add("Maltezer");
            rase.Add("Mops");
            rase.Add("Nemački ovčar");
            rase.Add("Pudla");
            rase.Add("Rotvajler");
            rase.Add("Samojed");
            rase.Add("Sibirski haski");
            rase.Add("Zlatni retriver");
            rase.Add("Mešanac");

            return rase;

        }

        public static List<string> GetBreeds()
        {

            List<string> rase = new List<string>();
            rase.Add("St. Bernard");
            rase.Add("Beagle");
            rase.Add("Dobermann");
            rase.Add("English Bulldog");
            rase.Add("French Bulldog");
            rase.Add("Greyhound");
            rase.Add("Yorkshire Terrier");
            rase.Add("English Cocker Spaniel ");
            rase.Add("Labrador Retriever");
            rase.Add("Maltese dog");
            rase.Add("Pug");
            rase.Add("German Shepard");
            rase.Add("Poodle");
            rase.Add("Rottweiler");
            rase.Add("Samoyed");
            rase.Add("Siberian Husky");
            rase.Add("Golden Retriever");
            rase.Add("Mixed breed");

            return rase;

        }

        #endregion
    }
}