using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBankingASP.Models;

namespace WebBankingASP.ViewModels
{
    public class BankAccountsListViewModel
    {
        public string Titolo { get; set; }
        public List<BankAccount> listaAccount { get; set; }
    }
}