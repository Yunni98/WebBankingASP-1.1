using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBankingASP.Models;

namespace WebBankingASP.ViewModels
{
    public class AccountMovementsListViewModel
    {
        public string Titolo { get; set; }
        public List<AccountMovement> listaMovimenti { get; set; }
    }
}