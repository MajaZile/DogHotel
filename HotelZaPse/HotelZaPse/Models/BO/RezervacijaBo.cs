using System;
using System.ComponentModel.DataAnnotations;

namespace HotelZaPse.Models.BO
{
    public class RezervacijaBo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Morate uneti datum prijave.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [DataType(DataType.Date)]
        public DateTime Prijava { get; set; }

        [Required(ErrorMessage = "Morate uneti datum odjave.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [DataType(DataType.Date)]
        public DateTime Odjava { get; set; }

        [MaxLength(500, ErrorMessage = "Prekoračili ste maksimalnu dozvoljenu dužinu.")]
        [DataType(DataType.MultilineText)]
        public string Napomena { get; set; }

        public double Cena { get; set; }
        public UslugaBo Usluga { get; set; }
        public PasBo Pas { get; set; }



        public int[] SelectedPasIds { get; set; }

    }
}