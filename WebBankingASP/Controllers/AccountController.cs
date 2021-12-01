using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebBankingASP.Models;

namespace WebBankingASP.Controllers
{
    public class AccountController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        
        [HttpPost]
        public ActionResult Login(UserLoginData user)
        {
            using(WebBankingEntities model = new WebBankingEntities())
            {
                var utente = model.Users.FirstOrDefault(f => f.username == user.Username && f.password == user.Password);
                //Verifichiamo che nel database di Users siano presenti il campo username e password inseriti
                if(utente == null)
                {
                    //ritorno che non è stato trovato nessuno con le credenziali inserite
                    return HttpNotFound();
                }

                //Verifico se l'utente è un banchiere o un correntista
                if(utente.is_banker == true)
                {
                    /*Caso del banchiere
                     *In questo caso imposto un coockie per il banchiere che mi permettera di visualizzare le pagine specifiche di tale utente
                     *con gli opportuni privilegi*/
                    FormsAuthentication.SetAuthCookie("true", false);
                    utente.last_login = DateTime.Now;
                    //Una volta creato il coockie posso reindirizzare la pagina a conti-correnti
                    return RedirectToAction("Index", "ContiCorrenti");
                }
                else
                {
                    //Caso del Correntista
                    FormsAuthentication.SetAuthCookie(utente.id.ToString(), false);
                    utente.last_login = DateTime.Now;
                    //Una volta creato il coockie posso reindirizzare la pagina a conti-correnti
                    return RedirectToAction("Index", "ContiCorrenti");
                }

            }
        }

        // GET: Login
        public ActionResult Index_Logout()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Logout(UserLoginData user)
        {
            using (WebBankingEntities model = new WebBankingEntities())
            {
                var utente = model.Users.FirstOrDefault(f => f.username == user.Username && f.password == user.Password);
                //Verifichiamo che nel database di Users siano presenti il campo username e password inseriti
                if (utente == null)
                {
                    //ritorno che non è stato trovato nessuno con le credenziali inserite
                    return HttpNotFound();
                }

                utente.last_logout = DateTime.Now;
                model.SaveChanges();
                FormsAuthentication.SignOut();
                return RedirectToAction("Index", "Account");
            }
        }
    }
}