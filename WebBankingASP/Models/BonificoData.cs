using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebBankingASP.Models
{
    public class BonificoData
    {
        [Display(Name = "Iban destinatario")]
        [Required(ErrorMessage = "Campo obbligatorio")]
        [MinLength(5, ErrorMessage = "Il campo può contenere minimo 5 caratteri")]
        [MaxLength(50, ErrorMessage = "Il campo può contenere al massimo 50 caratteri")]
        [Key]
        public string IbanDestinatario { get; set; }

        [Display(Name = "Importo")]
        [Required(ErrorMessage = "Campo obbligatorio")]
        [Range(1, 9999, ErrorMessage ="Il valore massimo che si può inviare è 9999 euro")]
        [Key]
        public double? Importo { get; set; }
    }
}