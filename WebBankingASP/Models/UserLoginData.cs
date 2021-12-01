using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebBankingASP.Models
{
    public class UserLoginData
    {
        [Display(Name ="Username")]
        [Required(ErrorMessage ="Campo obbligatorio")]
        [MinLength(1)]
        [MaxLength(255, ErrorMessage ="Il campo può contenere al massimo 255 caratteri")]
        [Key]
        public string Username { get; set; }

        [Display(Name ="Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Campo obbligatorio")]
        [MinLength(1)]
        [MaxLength(255, ErrorMessage = "Il campo può contenere al massimo 255 caratteri")]
        [Key]
        public string Password { get; set; }
    }
}