using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebBankingASP.Models;
using WebBankingASP.ViewModels;

namespace WebBankingASP.Controllers
{
    public class ContiCorrentiController : Controller
    {
        public string CookieName()
        {
            string cookieName = FormsAuthentication.FormsCookieName; //Trovo il cookie name
            HttpCookie authCookie = HttpContext.Request.Cookies[cookieName]; //Cerco il cookie con il suo nome
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value); //Decripto il cookie così da ottenere il nome
            return ticket.Name;//restituisco lo username
        }

        [HttpGet]
        public ActionResult Index()
        {
            /*string cookieName = FormsAuthentication.FormsCookieName; //Trovo il cookie name
            HttpCookie authCookie = HttpContext.Request.Cookies[cookieName]; //Cerco il cookie con il suo nome
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value); //Decripto il cookie così da ottenere il nome
            */
            string UserName = CookieName(); //Ottengo lo username
            //Verifico se l'Utente che si è loggato è un banchiere
            if(UserName == "true")
            {
                TempData["Banchiere View"] = UserName;
                //Caso del Banchiere: scarico e visualizzo la lista di tutti i conti correnti presenti nel DB
                using (WebBankingEntities model = new WebBankingEntities())
                {
                    List<BankAccount> listaConti = model.BankAccounts.ToList();
                    //Ritorno NotFound nel caso di risorsa non trovata
                    if(listaConti == null)
                    {
                        return HttpNotFound();
                    }
                    return View(new BankAccountsListViewModel
                    {
                        Titolo = "Conti Correnti",
                        listaAccount = listaConti
                    });
                }
            }
            else 
            {
                TempData["Banchiere View"] = UserName;
                //Caso del Correntista: scarico e visualizzo la lista di tutti i conti aperti del correntista
                using (WebBankingEntities model = new WebBankingEntities())
                {
                    int id = Convert.ToInt32(UserName);
                    var listaConti = model.BankAccounts.Where(w => w.fk_user == id).ToList();
                    //Ritorno NotFound nel caso di risorsa non trovata
                    if (listaConti == null)
                    {
                        return HttpNotFound();
                    }
                    return View(new BankAccountsListViewModel
                    {
                        Titolo = "Conti Correnti",
                        listaAccount = listaConti
                    });
                }
            }
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            //Restituisco i dettagli del singolo conto corrente passato per parametro id
            using(WebBankingEntities model = new WebBankingEntities())
            {
                BankAccount dettaglioConto = model.BankAccounts.FirstOrDefault(f => f.id == id);
                //Verifico che il conto desiderato sia presente nel DB e venga restituito
                if(dettaglioConto == null)
                {
                    return HttpNotFound();
                }
                return View(new BankAccountDetailViewModel { 
                Title = "Dettaglio conto",
                bankAccount = dettaglioConto
                });
            }
        }

        [HttpGet]
        public ActionResult Movements(int id)
        {
            //Restituisco i movimenti di un singolo conto corrente ordinati per data
            using(WebBankingEntities model = new WebBankingEntities())
            {
                List<AccountMovement> movimentiConto = model.AccountMovements.Where(w => w.fk_bankAccount == id).OrderBy(o => o.date).ToList();
                //Verifico che la lista dei movimenti del conto desiderato sia presente nel DB e venga restituito
                if(movimentiConto == null)
                {
                    return HttpNotFound();
                }
                return View(new AccountMovementsListViewModel
                {
                    Titolo = "Movimenti Conto",
                    listaMovimenti = movimentiConto
                });
            }
        }

        [HttpGet]
        public ActionResult Movement(int id2)
        {
            //Restituisco i dettagli di un singolo movimento
            using(WebBankingEntities model = new WebBankingEntities())
            {
                AccountMovement dettaglioMovimento = model.AccountMovements.FirstOrDefault(f => f.id == id2);
                //Verifico che venga restituito il dato dal DB
                if(dettaglioMovimento == null)
                {
                    return HttpNotFound();
                }
                return View(new AccountMovementsDetailViewModel
                {
                    Titolo = "Dettaglio del movimento",
                    movimentiAccount = dettaglioMovimento
                });
            }
        }


        public ActionResult BonificoView(int id)
        {
            TempData["IdConto"] = id;
            return View();
        }

        [HttpPost]
        public ActionResult Bonifico(BonificoData datiBonifico)
        {
            int id = Int32.Parse(TempData["IdConto"].ToString());
            //Verifichiamo che l'id del conto che vuole effettuare il bonifico non sia nullo
            /*if (id == null)
            {
                ModelState.AddModelError("", "Errore l'id del conto bancario che vuole fare il bonifico è nullo");
            }*/
            //Effettuiamo il bonifico dal conto identificato al conto destinatario
            using (WebBankingEntities model = new WebBankingEntities())
            {
                //Controlliamo il saldo
                double? saldo = model.AccountMovements.Where(W => W.BankAccount.id == id).Sum(s => (s.@in == null ? 0 : s.@in) - (s.@out == null ? 0 : s.@out));
                //Controlliamo le specifiche del bonifico
                if(datiBonifico == null || datiBonifico.Importo < 0 || saldo < datiBonifico.Importo)
                {
                    //Ritorniamo un messaggio di errore
                    ModelState.AddModelError("", "Non è possibile effettuare il Bonifico controllare il proprio saldo o l'importo immesso");
                }
                //Controlliamo se l'iban passato nella vista sia presente nel DB
                if(model.BankAccounts.Where(w => w.iban == datiBonifico.IbanDestinatario).Count() == 0)
                {
                    //creo il movimento di invio del denaro
                    model.AccountMovements.Add(new AccountMovement { fk_bankAccount = id, date = DateTime.Now, description = "Bonifico inviato", @in = null, @out = datiBonifico.Importo });
                    //creo il movimento di ricezione del denaro
                    model.AccountMovements.Add(new AccountMovement { fk_bankAccount = model.AccountMovements.FirstOrDefault(w => w.BankAccount.iban == datiBonifico.IbanDestinatario).id, date = DateTime.Now, description = "Bonifico ricevuto", @in = datiBonifico.Importo, @out = null });
                    //Salvo le modifiche
                    model.SaveChanges();
                    return RedirectToAction("Index", "ContiCorrenti");
                }
                else
                {
                    return HttpNotFound();
                }
            }
        }

        public ActionResult CreateNew()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(ContoData nuovoUtente)
        {
            using (WebBankingEntities model = new WebBankingEntities())
            {
                //Prima di creare il conto corrente verifichiamo se l'utente sia un banchiere o no
                /*string cookieName = FormsAuthentication.FormsCookieName; //Trovo il cookie name
                HttpCookie authCookie = HttpContext.Request.Cookies[cookieName]; //Cerco il cookie con il suo nome
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value); //Decripto il cookie così da ottenere il nome
                */
                string UserName = CookieName(); //Ottengo lo username
                //Verifico se l'Utente che si è loggato è un banchiere
                if (UserName == "true")
                {
                    //Controlliamo se l'iban che inseriamo non sia già presente nel DB (l'iban è unico per ogni conto corrente)
                    if (model.BankAccounts.Where(w => w.iban == nuovoUtente.Iban).Count() == 0)
                    {
                        //Aggiungo il nuovo Conto al DB
                        model.BankAccounts.Add(new BankAccount { iban = nuovoUtente.Iban, fk_user = nuovoUtente.User });
                        //Salvo le modifiche apportate
                        model.SaveChanges();
                        return RedirectToAction("Index", "ContiCorrenti");
                    }
                    else
                    {
                        //Caso in cui l'iban è già presente
                        //Ritorniamo un messaggio di errore
                        ModelState.AddModelError("", "Non aggiungere l'iban presente in quanto già presente nel DB (Iban univoco per ciascun conto)");
                    }
                }

                //L'utente non è un banchiere di conseguenza non ha il privilegio di poter aggiungere nuovi conti correnti
                //Ritorno un NotAuthorized
                return View("NonAutorizzato");
            }
        }

        //Get 
        public ActionResult EditView(int id)
        {
            //Prima di creare il conto corrente verifichiamo se l'utente sia un banchiere o no
            /*string cookieName = FormsAuthentication.FormsCookieName; //Trovo il cookie name
            HttpCookie authCookie = HttpContext.Request.Cookies[cookieName]; //Cerco il cookie con il suo nome
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value); //Decripto il cookie così da ottenere il nome
            */
            string UserName = CookieName(); //Ottengo lo username
            //Verifico se l'Utente che si è loggato è un banchiere
            if (UserName == "true")
            {
                using (WebBankingEntities model = new WebBankingEntities())
                {
                    BankAccount account = model.BankAccounts.FirstOrDefault(f => f.id == id);
                    //Controllo che l'id passato sia presente il conto nel DB
                    if (account == null)
                    {
                        return HttpNotFound();
                    }

                    return View(account);
                }
            }
            else
            {
                //messagio di errore mancata autorizzazione
                return View("NonAutorizzato");
            }
        }

        [HttpPost]
        public ActionResult Edit(BankAccount conto)
        {
            using (WebBankingEntities model = new WebBankingEntities())
            {
               
                    //Ulteriore controllo sull'Iban che si vuole inserire, non può essere ugugale a uno già presente (Iban univoco)
                    if (model.BankAccounts.Where(w => w.iban == conto.iban).Count() < 1)
                    {
                        //Prelevo in conto originario
                        var modifiche = model.BankAccounts.FirstOrDefault(w => w.id == conto.id);
                        if(modifiche == null)
                        {
                            return HttpNotFound();
                        }
                        //Apporto le modifiche eventualmente effettuate
                        modifiche.iban = conto.iban;
                        modifiche.fk_user = conto.fk_user;
                        //Salvo le modifiche apportate
                        model.SaveChanges();
                        return RedirectToAction("Index", "ContiCorrenti");
                    }
                    else
                    {
                        //Controllo su iban, l'iban che si vuole inserire è già presente 
                        //Ritorniamo un Problem
                        ModelState.AddModelError("", "L'id del conto non è presente nella lista dei conti, verificare se il conto da modificare esista");
                        return HttpNotFound();
                    }
            }
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            using (WebBankingEntities model = new WebBankingEntities())
            {
                //Prima di eliminare il conto corrente verifichiamo se l'utente sia un banchiere o no
                /*string cookieName = FormsAuthentication.FormsCookieName; //Trovo il cookie name
                HttpCookie authCookie = HttpContext.Request.Cookies[cookieName]; //Cerco il cookie con il suo nome
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value); //Decripto il cookie così da ottenere il nome
                */
                string UserName = CookieName(); //Ottengo lo username
                //Verifico se l'Utente che si è loggato è un banchiere
                if (UserName == "true")
                {
                    //Caso del banchiere
                    if (model.BankAccounts.Where(w => w.id == id).Count() > 0) //Verifico se ci sono conti nel DB con questo ID
                    {
                        var conto = model.BankAccounts.FirstOrDefault(f => f.id == id);
                        model.AccountMovements.RemoveRange(model.AccountMovements.Where(w => w.fk_bankAccount == conto.id));
                        model.BankAccounts.Remove(conto);
                        model.SaveChanges();
                        return RedirectToAction("Index", "ContiCorrenti");
                    }
                    else
                    {
                        //Conto non trovato
                        return HttpNotFound();
                    }
                }
                else
                {
                    //L'utente non è un banchiere di conseguenza non ha il privilegio di poter aggiungere nuovi conti correnti
                    //Ritorno un NotAuthorized
                    return View("NonAutorizzato");
                }
            }
        }
    }
}