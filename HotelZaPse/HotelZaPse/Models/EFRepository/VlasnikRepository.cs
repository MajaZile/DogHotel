using HotelZaPse.Models.BO;
using HotelZaPse.Models.Interfaces;
using System;
using System.Linq;

namespace HotelZaPse.Models.EFRepository
{
    public class VlasnikRepository : IVlasnikRepository
    {
        #region Fields
        private HotelZaPseDBEntities entities = new HotelZaPseDBEntities();
        #endregion

        #region Methods
        public bool AmIVlasnik(string email, string password)
        {
            bool amIVlasnik = false;
            try
            {
                VlasnikModel vlasnik = entities.VlasnikModels.Single(t => (t.mail == email && t.lozinka == password));
                bool sameMail = String.Equals(vlasnik.mail, email, StringComparison.Ordinal);
                bool samePassword = String.Equals(vlasnik.lozinka, password, StringComparison.Ordinal);
                if (sameMail && samePassword)
                    amIVlasnik = true;
            }
            catch
            {
            }
            return amIVlasnik;
        }

        public void Create(VlasnikBo vlasnikBo)
        {
            VlasnikModel vlasnikModel = new VlasnikModel();
            vlasnikModel.ime = vlasnikBo.Ime;
            vlasnikModel.prezime = vlasnikBo.Prezime;
            vlasnikModel.adresa = vlasnikBo.Adresa;
            vlasnikModel.telefon = vlasnikBo.Telefon;
            vlasnikModel.mail = vlasnikBo.Mail;
            vlasnikModel.lozinka = vlasnikBo.Lozinka;
            //dodacu tek duplu validaciju lozinke

            entities.VlasnikModels.Add(vlasnikModel);

            try
            {
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public void Edit(VlasnikBo vlasnikBo)
        {
            VlasnikModel vlasnikModel = entities.VlasnikModels.Single(t => t.vlasnikID == vlasnikBo.Id);
            vlasnikModel.ime = vlasnikBo.Ime;
            vlasnikModel.prezime = vlasnikBo.Prezime;
            vlasnikModel.adresa = vlasnikBo.Adresa;
            vlasnikModel.telefon = vlasnikBo.Telefon;
            //mail ne menjamo
            vlasnikModel.lozinka = vlasnikBo.Lozinka;
            //dodacu tek duplu validaciju lozinke

            try
            {
                entities.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public VlasnikBo GetByMail(string mail)
        {
            return Map(entities.VlasnikModels.Single(t => t.mail == mail));
        }

        public bool IsMailUnique(VlasnikBo vlasnikBo)
        {
            return !entities.VlasnikModels.Any(t => t.mail == vlasnikBo.Mail);
        }

        public VlasnikBo GetById(int id)
        {
            return Map(entities.VlasnikModels.Single(t => t.vlasnikID == id));
        }

        #endregion

        #region HelperMethods
        VlasnikBo Map(VlasnikModel vlasnikModel)
        {
            VlasnikBo vlasnikBo = new VlasnikBo()
            {
                Id = vlasnikModel.vlasnikID,
                Ime = vlasnikModel.ime,
                Prezime = vlasnikModel.prezime,
                Adresa = vlasnikModel.adresa,
                Telefon = vlasnikModel.telefon,
                Mail = vlasnikModel.mail,
                Lozinka = vlasnikModel.lozinka
            };
            return vlasnikBo;
        }

        #endregion
    }
}