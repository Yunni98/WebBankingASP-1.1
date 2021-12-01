using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBankingASP.Models;

namespace WebBankingASP.ViewModels
{
    public class AccountMovementsDetailViewModel
    {
        public string  Titolo { get; set; }
        public AccountMovement movimentiAccount { get; set; }
    }
}