using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Courier.Models;
using System.Collections.Generic;
using Courier.Controllers;

namespace Courier.WebService
{
    [ServiceContract(Namespace = "")]
    [SilverlightFaultBehavior]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ServicioSilverlight
    {
        #region DoWork
        [OperationContract]
        public string  DoWork()
        {
            // Agregue aquí la implementación de la operación

            return "Prueba Funcionamiento 1";
        }
        #endregion

        #region ModificarImpresora
        [OperationContract]
        public bool ModificarImpresora(string oNombreImpresora, int oRut)
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oDatos = (from x in oPar.TB_PAR_USU_USUARIO
                         where x.PK_PAR_USU_RUT == oRut
                         select x).Single();

            oDatos.FL_PAR_USU_NOMBRE_IMPRESORA = oNombreImpresora;

            try
            {
                oPar.SubmitChanges();
                return true;
            }
            catch 
            {
                return false;
            }

        }
        #endregion        
        public class wsEncabezado
        {
            public decimal OTP {get;set;}
            public decimal OTD { get; set; }
            public decimal CantidadBultos { get; set; }
            public decimal Pallet { get; set; }
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
            public decimal idBulto {get;set;}
            public int NumeroBulto {get;set;}
            public int TipoEtiqueta { get; set; }
            public float? Peso { get; set; }
        }

        [OperationContract]
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

            var MaxGuia = (from pr in oFol.PR_FOL_DATOS_ETIQUETA_MAX_GUIA(OTP, OTD)
                          select pr).ToList();

            string ReferenciaGuia;

            if (MaxGuia.Count() > 0)
                ReferenciaGuia = MaxGuia.ToList()[0].PK_FOL_DOA_NUMERO.ToString();
            else
                ReferenciaGuia = oEncabezado.FL_FOL_OTD_REFERENCIA1;

            string Comuna=string.Empty,Localidad=string.Empty;
            if (oEncabezado.PK_PAR_LOC_ID == null || oEncabezado.PK_PAR_LOC_ID == 0)
            {
                var DatosLocalizacion = oValidation.RetornaNombreComunaRegion(oEncabezado.PK_PAR_COM_ID.ToString());
                Comuna = DatosLocalizacion.Comuna + " (" + DatosLocalizacion.ComunaIATA + ")";
            }
            else
            {
                var DatosLocalizacion = oValidation.RetornaNombreLocalidadComunaRegion(oEncabezado.PK_PAR_LOC_ID.ToString());
                Comuna = DatosLocalizacion.Comuna + " (" + DatosLocalizacion.ComunaIATA + ")"  ;
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
            oResult.Referencia1 = ReferenciaGuia;
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
                        Peso=oFila.FL_FOL_BLT_PESO
                    });
                oContador++;
            }

            oResult.ListaDetalle = oDetalleNormal;

            return oResult;
        }



        [OperationContract]
        public wsEncabezado ImpresionEtiquetaPallet(decimal OTP, decimal OTD, int NPALLET, int BLTOS)
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

            var MaxGuia = (from pr in oFol.PR_FOL_DATOS_ETIQUETA_MAX_GUIA(OTP, OTD)
                           select pr).ToList();

            string ReferenciaGuia;

            if (MaxGuia.Count() > 0)
                ReferenciaGuia = MaxGuia.ToList()[0].PK_FOL_DOA_NUMERO.ToString();
            else
                ReferenciaGuia = oEncabezado.FL_FOL_OTD_REFERENCIA1;

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

            //if (oEncabezado.FL_FOL_OTD_VIA == "T")
            //    oVia = "Terrestre";
            //else if (oEncabezado.FL_FOL_OTD_VIA == "A")
            //    oVia = "Aérea";
            //else
            //    oVia = oEncabezado.FL_FOL_OTD_VIA;


            oResult.OTP = oEncabezado.PK_FOL_OTP_ID;
            oResult.OTD = oEncabezado.PK_FOL_OTD_ID;
            oResult.CantidadBultos = BLTOS;
            oResult.Pallet = NPALLET;
            oResult.Cliente = oEncabezado.FL_FOL_OTD_DESTINATARIO_NOMBRE;
            oResult.Comuna = Comuna;
            oResult.Localidad = Localidad;
            oResult.Direccion = oEncabezado.FL_FOL_OTD_DESTINATARIO_DIRECCION;
            oResult.Referencia1 = ReferenciaGuia;
            oResult.Referencia2 = oEncabezado.FL_FOL_OTD_REFERENCIA2;
            oResult.Servicio = oServicio;
            oResult.SucursalDestino = oSucursal;
            //oResult.Via = oVia;
            oResult.Telefono = oEncabezado.FL_FOL_OTD_TELEFONO;

            //var oDetalle = from blt in oFol.TB_FOL_BLT_BULTO
            //               where blt.PK_FOL_OTD_ID == OTD
            //                  && blt.PK_FOL_OTP_ID == OTP
            //                  && blt.PK_FOL_EST_ID != 13//Eliminado
            //               orderby blt.PK_FOL_BLT_ID
            //               select blt;

            //List<wsDetalleNormal> oDetalleNormal = new List<wsDetalleNormal>();
            //int oContador = 1;
            //foreach (var oFila in oDetalle)
            //{
            //    oDetalleNormal.Add(
            //        new wsDetalleNormal
            //        {

            //            idBulto = oFila.PK_FOL_BLT_ID,
            //            NumeroBulto = oContador,
            //            Peso = oFila.FL_FOL_BLT_PESO
            //        });
            //    oContador++;
            //}

            //oResult.ListaDetalle = oDetalleNormal;

            return oResult;
        }








        [OperationContract]
        public IEnumerable<wsEncabezado> ImpresionEtiquetaPorOTP(decimal OTP)
        {
            List<wsEncabezado> oLista = new List<wsEncabezado>();
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            var oDatos = from otd in oFol.TB_FOL_OTD_OT_DETALLE
                         where otd.PK_FOL_OTP_ID == OTP
                         select otd;

            foreach (var oFila in oDatos)
            {
                oLista.Add(ImpresionEtiqueta(oFila.PK_FOL_OTP_ID,oFila.PK_FOL_OTD_ID));
            }
            return oLista;
        }

        public class classImpresionGuia
        {
            public decimal OTP {get;set;}
            public decimal OTD { get; set; }
            public string Referencia1 { get; set; }
        }
        [OperationContract]
        public List<classImpresionGuia> ImpresionGuiaMasiva(decimal OTP)
        {           
            ValidationController oValidation = new ValidationController();
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            List<classImpresionGuia> oDatos = new List<classImpresionGuia>();

            var oResult = from otd in oFol.TB_FOL_OTD_OT_DETALLE
                          where otd.PK_FOL_OTP_ID == OTP
                          select
                          new
                            {
                                otd.FL_FOL_OTD_REFERENCIA1,
                                otd.PK_FOL_OTD_ID
                            };

            foreach (var oFile in oResult)
            {
                oDatos.Add(new classImpresionGuia
                {
                    Referencia1 = oFile.FL_FOL_OTD_REFERENCIA1,
                    OTP = OTP,
                    OTD = oFile.PK_FOL_OTD_ID,
                });
            }

            return oDatos;
        }
        [OperationContract]
        public wsEncabezado ImpresionCustom(decimal OTP,decimal OTD,int Cantidad, int TipoEtiqueta, decimal Bulto1, decimal Bulto2, decimal Bulto3 , decimal Bulto4)
        {
            ValidationController oValidation = new ValidationController();
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            wsEncabezado oEncabezado = ImpresionEtiqueta(OTP, OTD);

            var oBlt = from blt in oFol.TB_FOL_BLT_BULTO
                          where
                           blt.PK_FOL_OTP_ID==OTP && blt.PK_FOL_OTD_ID==OTD
                           && blt.PK_FOL_EST_ID!=13
                           orderby blt.PK_FOL_BLT_ID
                          select
                          new
                          {
                              blt.PK_FOL_BLT_ID,                            
                              blt.FL_FOL_BLT_PESO
                          };

            List<wsDetalleNormal> oDetalle = new List<wsDetalleNormal>();

            int oCount = 1;
            foreach (var oFila in oBlt)
            {
                if (oFila.PK_FOL_BLT_ID == Bulto1 || oFila.PK_FOL_BLT_ID == Bulto2 || oFila.PK_FOL_BLT_ID == Bulto3 || oFila.PK_FOL_BLT_ID == Bulto4)
                {
                    oDetalle.Add(new wsDetalleNormal
                    {
                        idBulto = oFila.PK_FOL_BLT_ID,
                        NumeroBulto = oCount,
                        TipoEtiqueta=TipoEtiqueta,
                        Peso=oFila.FL_FOL_BLT_PESO
                    });                    
                }
                oCount += 1;
            }

            oEncabezado.ListaDetalle = oDetalle;

            return oEncabezado;
        }
        // Agregue aquí más operaciones y márquelas con [OperationContract]
    }
}

