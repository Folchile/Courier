using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Courier.Models
{
    public class DashBoardModels
    {
        public string NombreImpresora { get; set; }
        public void GetNombreImpresora(int RutUsuario)
        {
            NombreImpresora = "";
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oDatos = from usu in oPar.TB_PAR_USU_USUARIO
                         where usu.PK_PAR_USU_RUT == RutUsuario
                         select usu.FL_PAR_USU_NOMBRE_IMPRESORA;
            if (oDatos.Count() > 0)
                NombreImpresora = oDatos.ToList()[0];
        }
    }    
}