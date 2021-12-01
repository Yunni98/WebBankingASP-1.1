using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebBankingASP.Models
{
    public class ContoData
    {
        [Display(Name = "Iban")]
        [Required(ErrorMessage = "Campo obbligatorio")]
        [MinLength(5, ErrorMessage = "Il campo può contenere minimo 5 caratteri")]
        [MaxLength(50, ErrorMessage = "Il campo può contenere al massimo 50 caratteri")]
        [Key]
        public string Iban { get; set; }

        [Display(Name = "FK_User")]
        [Required(ErrorMessage = "Campo obbligatorio")]
        [Range(1, 100, ErrorMessage = "Il valore massimo che si può usare è 100")]
        [Key]
        public int User { get; set; }
    }
}