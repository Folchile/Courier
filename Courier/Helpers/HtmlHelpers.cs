using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Courier.Models;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;


namespace Courier.Helpers
{
    public static class HtmlHelpers
    {       

        public static HtmlString ConsultaMenu(this HtmlHelper helper, string oRut, UrlHelper oUrl)
        {
            Autenticacion.Controllers.AccountController oAccount = new Autenticacion.Controllers.AccountController();
           
            return oAccount.sConsultaMenu(oRut,oUrl);
        }
        
        public static HtmlString LargoMaximoString(this HtmlHelper helpepr, string oRut, int oLargoMaximo)
        {
            HtmlString oIhtml;
            if (oRut.Length <= oLargoMaximo)
            {
                oIhtml = new HtmlString(oRut);
            }
            else
            {
                oIhtml = new HtmlString(oRut.Substring(1, oLargoMaximo) + "..." );
            }
            return oIhtml;            
        }

        
    }
}