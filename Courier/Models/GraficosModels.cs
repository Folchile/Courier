using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Courier.Models
{
    public class GraficosModels
    {
        public List<PR_FOL_GRA_OTS_GRAL_DIA_DETALLEResult> ListaOT { get; set; }
        public void getListaOT()
        {
            Linq_SPDataContext oSP = new Linq_SPDataContext();
            ListaOT=oSP.PR_FOL_GRA_OTS_GRAL_DIA_DETALLE().ToList();
        }
    }
}