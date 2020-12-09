using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Courier.Models;

namespace Courier.Controllers
{
    public class ImpresionController : Controller
    {
        //
        // GET: /Impresion/

        public ActionResult ImpresionEtiquetaGrande(decimal OTP, decimal OTD, decimal Bulto)
        {
            wsEncabezado oDatos = ImpresionEtiqueta(OTP, OTD);

            var oListaDetalle = from elem in oDatos.ListaDetalle
                                 where elem.idBulto == Bulto
                                 select elem;

            oDatos.ListaDetalle = oListaDetalle;


            return View(oDatos);
        }

        public class wsEncabezado
        {
            public decimal OTP { get; set; }
            public decimal OTD { get; set; }
            public decimal CantidadBultos { get; set; }
            public string SucursalDestino { get; set; }
            public string Comuna { get; set; }
            public string Localidad { get; set; }
            public string Via { get; set; }

            public IEnumerable<wsDetalleNormal> ListaDetalle { get; set; }

            public string Cliente { get; set; }
            public string Direccion { get; set; }
            public string Telefono { get; set; }
            public string Servicio { get; set; }
            public string Referencia1 { get; set; }
            public string Referencia2 { get; set; }
        }
        public class wsDetalleNormal
        {
            public decimal idBulto { get; set; }
            public int NumeroBulto { get; set; }
            public int TipoEtiqueta { get; set; }
            public float? Peso { get; set; }
        }
        
        public wsEncabezado ImpresionEtiqueta(decimal OTP, decimal OTD)
        {
            wsEncabezado oResult = new wsEncabezado();
            ValidationController oValidation = new ValidationController();
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            string oServicio = string.Empty;
            string oSucursal = string.Empty;
            string oVia = string.Empty;
            int oIdSucursal = 0;


            var oEncabezado = (from otd in oFol.TB_FOL_OTD_OT_DETALLE
                               where otd.PK_FOL_OTD_ID == OTD
                               && otd.PK_FOL_OTP_ID == OTP
                               select otd).Single();


            string Comuna = string.Empty, Localidad = string.Empty;
            if (oEncabezado.PK_PAR_LOC_ID == null || oEncabezado.PK_PAR_LOC_ID == 0)
            {
                var DatosLocalizacion = oValidation.RetornaNombreComunaRegion(oEncabezado.PK_PAR_COM_ID.ToString());
                Comuna = DatosLocalizacion.Comuna + " (" + DatosLocalizacion.ComunaIATA + ")";
            }
            else
            {
                var DatosLocalizacion = oValidation.RetornaNombreLocalidadComunaRegion(oEncabezado.PK_PAR_LOC_ID.ToString());
                Comuna = DatosLocalizacion.Comuna + " (" + DatosLocalizacion.ComunaIATA + ")";
                Localidad = DatosLocalizacion.Localidad + " (" + DatosLocalizacion.LocalidadIATA + ")";
            }



            oIdSucursal = oValidation.GetSucursalfromComuna(Convert.ToInt32(oEncabezado.PK_PAR_COM_ID));
            oSucursal = oValidation.GetSucursalNamefromIDSucursal(oIdSucursal);
            oServicio = oValidation.GetServicioNamefromOTP(oEncabezado.PK_FOL_OTP_ID);

            if (oEncabezado.FL_FOL_OTD_VIA == "T")
                oVia = "Terrestre";
            else if (oEncabezado.FL_FOL_OTD_VIA == "A")
                oVia = "Aérea";
            else
                oVia = oEncabezado.FL_FOL_OTD_VIA;


            oResult.OTP = oEncabezado.PK_FOL_OTP_ID;
            oResult.OTD = oEncabezado.PK_FOL_OTD_ID;
            oResult.CantidadBultos = Convert.ToDecimal(oEncabezado.FL_FOL_OTD_BULTO);
            oResult.Cliente = oEncabezado.FL_FOL_OTD_DESTINATARIO_NOMBRE;
            oResult.Comuna = Comuna;
            oResult.Localidad = Localidad;
            oResult.Direccion = oEncabezado.FL_FOL_OTD_DESTINATARIO_DIRECCION;
            oResult.Referencia1 = oEncabezado.FL_FOL_OTD_REFERENCIA1;
            oResult.Referencia2 = oEncabezado.FL_FOL_OTD_REFERENCIA2;
            oResult.Servicio = oServicio;
            oResult.SucursalDestino = oSucursal;
            oResult.Via = oVia;
            oResult.Telefono = oEncabezado.FL_FOL_OTD_TELEFONO;

            var oDetalle = from blt in oFol.TB_FOL_BLT_BULTO
                           where blt.PK_FOL_OTD_ID == OTD
                              && blt.PK_FOL_OTP_ID == OTP
                              && blt.PK_FOL_EST_ID != 13//Eliminado
                           orderby blt.PK_FOL_BLT_ID
                           select blt;

            List<wsDetalleNormal> oDetalleNormal = new List<wsDetalleNormal>();
            int oContador = 1;
            foreach (var oFila in oDetalle)
            {
                oDetalleNormal.Add(
                    new wsDetalleNormal
                    {

                        idBulto = oFila.PK_FOL_BLT_ID,
                        NumeroBulto = oContador,
                        Peso = oFila.FL_FOL_BLT_PESO
                    });
                oContador++;
            }

            oResult.ListaDetalle = oDetalleNormal;

            return oResult;
        }

    }
}
