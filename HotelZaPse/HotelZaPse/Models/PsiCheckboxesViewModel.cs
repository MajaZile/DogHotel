using HotelZaPse.Models.BO;
using System.Collections.Generic;

namespace HotelZaPse.Models
{
    //ViewModel klasa za pogled CreatePsiCheckboxes u okviru kontrolera Rezervacija
    public class PsiCheckboxesViewModel
    {
        public IEnumerable<PasBo> PasList { get; set; }
        public int[] SelectedPasIds { get; set; }

    }
}