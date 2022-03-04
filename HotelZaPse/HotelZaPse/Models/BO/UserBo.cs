using HotelZaPse.Models.EFRepository;
using HotelZaPse.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace HotelZaPse.Models.BO
{
    public class UserBo
    {
        [Required(ErrorMessage = "Morate uneti mail adresu.")]
        [MaxLength(50, ErrorMessage = "Prekoračili ste maksimalnu dozvoljenu dužinu.")]
        [RegularExpression(@"^[0-9a-zA-Z]+([_.-]?[0-9a-zA-Z]+)*@([.]?[0-9a-zA-Z]+)*.[a-zA-Z]{2,4}$",
         ErrorMessage = "Niste uneli ispravanu mail adresu.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Morate uneti lozinku.")]
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])[a-zA-Z0-9]{7,20}$",
        // ErrorMessage = "Lozinka mora sadrzati 7 - 20 karaktera (bar jedan broj, bar jedno malo i bar jedno veliko slovo).")]
        public string Password { get; set; }
        public string Role { get; private set; }

        public void setRole()
        {
            IVlasnikRepository vlasnikRepository = new VlasnikRepository();
            IAdminRepository adminRepository = new AdminRepository();

            if (vlasnikRepository.AmIVlasnik(Email, Password))
            {
                Role = "vlasnik";
            }
            else if (adminRepository.AmIAdmin(Email, Password))
            {
                Role = "admin";
            }
            else
            {
                Role = "";
            }
        }
    }
}