using System.ComponentModel.DataAnnotations;

namespace HotelZaPse.Models.BO
{
    public class UslugaBo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Morate uneti naziv usluge.")]
        [MaxLength(50, ErrorMessage = "Prekoračili ste maksimalnu dozvoljenu dužinu.")]
        public string Naziv { get; set; }

        [Required(ErrorMessage = "Morate uneti cenu usluge.")]
        [Range(100, 10000, ErrorMessage = "Cena mora biti u opsegu 100 - 10 000 dinara.")]
        public double Cena { get; set; }

        [Required(ErrorMessage = "Morate uneti opis usluge.")]
        [MaxLength(500, ErrorMessage = "Prekoračili ste maksimalnu dozvoljenu dužinu.")]
        [DataType(DataType.MultilineText)]
        public string Opis { get; set; }

        [Required(ErrorMessage = "Morate uneti kapacitet.")]
        [Range(1, 100, ErrorMessage = "Kapacitet mora biti u opsegu 1 - 100.")]
        public int Kapacitet { get; set; }
        public bool Aktivna { get; set; }
        public AdminBo Admin { get; set; }

    }
}