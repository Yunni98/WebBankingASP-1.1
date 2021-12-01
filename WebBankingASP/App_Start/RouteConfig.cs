using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebBankingASP
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "Accesso",
                url: "login",
                defaults: new { controller = "Account", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Scollegarsi",
                url: "logout",
                defaults: new { controller = "Account", action = "Index1", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Lista di tutti i Conti",
                url: "conti-correnti",
                defaults: new { controller = "ContiCorrenti", action = "Index"}
            );

            routes.MapRoute(
                name: "Dettagli di un singolo conto",
                url: "conti-correnti/{id}",
                defaults: new { controller = "ContiCorrenti", action = "Details", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Lista dei movimenti di un singolo conto corrente",
                url: "conti-correnti/{id}/movimenti",
                defaults: new { controller = "ContiCorrenti", action = "Movements", id = UrlParameter.Optional }
                );

            routes.MapRoute(
                name: "Dettagli di un singolo movimento",
                url: "conti-correnti/{id1}/movimenti/{id2}",
                defaults: new { controller = "ContiCorrenti", action = "Movement", id1 = UrlParameter.Optional, id2 = UrlParameter.Optional }
                );

            routes.MapRoute(
                name: "Bonifico",
                url: "conti-correnti/{id}/bonifico",
                defaults: new { controller = "ContiCorrenti", action = "BonificoView", id = UrlParameter.Optional }
                );

            routes.MapRoute(
                name: "Nuovo Conto",
                url: "conto-corrente/new",
                defaults: new { controller = "ContiCorrenti", action = "CreateNew" }
                );

            routes.MapRoute(
                name: "Aggiorna Conto",
                url: "conti-correnti/{id}/edit",
                defaults: new { controller = "ContiCorrenti", action = "EditView", id = UrlParameter.Optional }
                );

            routes.MapRoute(
                name: "Elimina Conto",
                url: "conti-correnti/{id}/delete",
                defaults: new { controller = "ContiCorrenti", action = "Delete", id = UrlParameter.Optional }
                );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
