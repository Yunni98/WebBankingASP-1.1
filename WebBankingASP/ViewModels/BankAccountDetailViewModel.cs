using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBankingASP.Models;
using WebBankingASP.ViewModels;

namespace WebBankingASP.ViewModels
{
    public class BankAccountDetailViewModel
    {
        public string Title { get; set; }
        public BankAccount bankAccount { get; set; }
    }
}