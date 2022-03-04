using System.ComponentModel.DataAnnotations;

namespace HotelZaPse.Models.BO
{
    public class PasBo
    {
        public int Id { get; set; }

        public VlasnikBo Vlasnik { get; set; }

        [Required(ErrorMessage = "Morate uneti ime psa.")]
        [MaxLength(50, ErrorMessage = "Prekoračili ste maksimalnu dozvoljenu dužinu.")]
        public string Ime { get; set; }

        [Required]
        public string Rasa { get; set; }

        [Required(ErrorMessage = "Morate izabrati pol psa.")]
        public bool Pol { get; set; }

        [Required]
        public bool Sterilisan { get; set; }

        [MaxLength(500, ErrorMessage = "Prekoračili ste maksimalnu dozvoljenu dužinu.")]
        [DataType(DataType.MultilineText)]
        public string Opis { get; set; }

        public bool Obrisan { get; set; }


    }
}