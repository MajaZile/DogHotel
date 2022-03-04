using System.ComponentModel.DataAnnotations;

namespace HotelZaPse.Models.BO
{
    public class VlasnikBo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Morate uneti ime.")]
        [MaxLength(50, ErrorMessage = "Prekoračili ste maksimalnu dozvoljenu dužinu.")]
        public string Ime { get; set; }

        [Required(ErrorMessage = "Morate uneti prezime.")]
        [MaxLength(50, ErrorMessage = "Prekoračili ste maksimalnu dozvoljenu dužinu.")]
        public string Prezime { get; set; }

        [Required(ErrorMessage = "Morate uneti adresu.")]
        [MaxLength(50, ErrorMessage = "Prekoračili ste maksimalnu dozvoljenu dužinu.")]
        [RegularExpression(@"^[a-zA-Z]{1}[a-zA-Z\s]+[0-9]+[a-zA-Z\s]*,(\s)?[a-zA-Z]{1}[a-zA-Z\s]+$",
            ErrorMessage ="Niste ispravno uneli adresu.")]
        public string Adresa { get; set; }

        [Required(ErrorMessage = "Morate uneti telefon.")]
        [RegularExpression(@"^(\+3816[0-9]{7,8})|(06[0-9]{7,8})|(0[0-9]{8,9})$",
         ErrorMessage = "Niste ispravno uneli broj telefona.")]
        public string Telefon { get; set; }

        [Required(ErrorMessage = "Morate uneti mail.")]
        [MaxLength(50, ErrorMessage = "Prekoračili ste maksimalnu dozvoljenu dužinu.")]
        [RegularExpression(@"^[0-9a-zA-Z]+([_.-]?[0-9a-zA-Z]+)*@([.]?[0-9a-zA-Z]+)*.[a-zA-Z]{2,4}$",
         ErrorMessage = "Niste ispravno uneli mail adresu.")]
        public string Mail { get; set; }

        [Required(ErrorMessage = "Morate uneti lozinku.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])[a-zA-Z0-9]{7,20}$",
         ErrorMessage = "Lozinka mora sadrzati 7 - 20 karaktera (bar jedan broj, bar jedno malo i bar jedno veliko slovo).")]
        public string Lozinka { get; set; }

        [Required(ErrorMessage = "Morate potvrditi lozinku.")]
        [Compare("Lozinka", ErrorMessage = "Lozinke se moraju poklapati.")]
        public string Lozinka2 { get; set; }
    }
}