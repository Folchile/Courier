using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Courier.Models
{
    public class RecepcionModels
    {
        [Required]
        [Display (Name="N° OT")]
        public string RecOT { get; set; }

        public decimal OTP { get; set; }
        public decimal OTD { get; set; }
        public bool? Devolucion { get; set; }

        [Display (Name="Código Bulto")]
        [Required]        
        public string strIdBulto { get; set; }

        //[Remote("vjSonValidarIdBultoRecepcion", "Validation")]
        [Required]  
        [Display (Name="Código Bulto")]
        public string strValidationIdBulto { get; set; }
        
        
        
        public decimal decIdBulto { get; set; }
        public PR_FOL_VER_TOTAL_OTResult VerTotalOT { get; set; }
        public List<ListaDespacho> oListaRecepcion { get; set; }
        
        public decimal NumeroManifiesto { get; set; }//Representa el número de recepción, para re-utilizar codigo reutilizo el nombre de variable

        #region GetDatosByIdBulto
        public void GetDatosByIdBulto()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oResult = oFol.PR_FOL_VER_TOTAL_OT(decIdBulto);
            foreach (var oFila in oResult)
            {
                VerTotalOT = oFila;
            }            
        }
        #endregion
        #region GetDatosByUltimoMovimiento
        public void GetDatosByUltimoMovimiento()
        {
            try
            {
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();

                var Ultimo = (from reb in oFol.TB_FOL_REB_RECEPCION_BULTO
                             join rec in oFol.TB_FOL_REC_RECEPCION on reb.PK_FOL_REC_ID equals rec.PK_FOL_REC_ID
                             where rec.PK_PAR_USU_RUT == oValidation.GetRutActiveUser()
                             orderby reb.FL_FOL_REB_FECHA descending
                             select reb).Take(1);

                if (Ultimo.Count() > 0)
                {
                    decIdBulto = Ultimo.ToList()[0].PK_FOL_BLT_ID;
                    var oResult = oFol.PR_FOL_VER_TOTAL_OT(decIdBulto);
                    foreach (var oFila in oResult)
                    {
                        VerTotalOT = oFila;
                    }
                }
            }
            catch { }
        }
        #endregion
        #region GetListaRecepcion
        public void GetListaRecepcion()
        {
            Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();

            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = (from blt in oFol.PR_FOL_BULTOS_REC(oValidation.GetSucursalIDfromActiveUser())                                                                                  
                         orderby blt.FL_FOL_REB_OK
                         select new ListaDespacho
                         {
                             Alto = Convert.ToDouble(blt.FL_FOL_BLT_ALTO),
                             Ancho = Convert.ToDouble(blt.FL_FOL_BLT_ANCHO),
                             Estado = blt.FL_FOL_EST_NOMBRE,
                             EstadoDetalle=blt.FL_FOL_EST_DESCRIPCION,
                             idBulto = blt.PK_FOL_BLT_ID,
                             Largo = Convert.ToDouble(blt.FL_FOL_BLT_LARGO),
                             OK = blt.FL_FOL_REB_OK,
                             Peso = Convert.ToDouble(blt.FL_FOL_BLT_PESO),
                             PesoVolumetrico = Convert.ToDouble(blt.FL_FOL_BLT_PESO_VOLUMETRICO),
                             xn = blt.Row + "/" + blt.FL_FOL_OTD_BULTO.ToString(),
                             OTD=blt.PK_FOL_OTD_ID,
                             OTP=blt.PK_FOL_OTP_ID,
                             NumeroManifiesto=blt.PK_FOL_REC_ID
                         }).ToList() ;

            oListaRecepcion = oDatos;
        }
        #endregion        
        #region AgregarBultoRecepcion
        public bool AgregarBultoRecepcion(decimal OTP_ID, decimal OTD_ID, decimal BLT_ID, decimal MAN_ID)
        {
            Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();
            try
            {
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                oFol.PR_FOL_AGREGA_BULTO_REC(OTP_ID, OTD_ID, MAN_ID, BLT_ID, oValidation.GetRutActiveUser(), oValidation.GetSucursalIDfromActiveUser());
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
        #region CreaRecepcion
        public bool CreaRecepcion(int RutUsuario, int SucId, decimal OTP, decimal OTD)
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            TB_FOL_REC_RECEPCION oMan = new TB_FOL_REC_RECEPCION
            {
                FL_FOL_REC_FECHA_CREACION = DateTime.Now,
                PK_FOL_MAE_ID = 1,//Abierto
                PK_PAR_SUC_ID = SucId,
                PK_PAR_USU_RUT = RutUsuario,
                PK_FOL_OTP_ID=OTP,
                PK_FOL_OTD_ID=OTD
            };
            try
            {
                oFol.TB_FOL_REC_RECEPCION.InsertOnSubmit(oMan);
                oFol.SubmitChanges();
                NumeroManifiesto = oMan.PK_FOL_REC_ID;
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
        #region GetUltimaRecepcion
        public void GetUltimaRecepcion()
        {
            Controllers.ValidationController oValidation = new Controllers.ValidationController();

            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            //var oRecepcion = from
            //                      rec in oFol.TB_FOL_REC_RECEPCION
            //                 join reb in oFol.TB_FOL_REB_RECEPCION_BULTO on rec.PK_FOL_REC_ID equals reb.PK_FOL_REC_ID
            //                 where
            //                    rec.PK_PAR_SUC_ID==oValidation.GetSucursalIDfromActiveUser()
            //                    //rec.PK_PAR_USU_RUT == oValidation.GetRutActiveUser()
            //                    && rec.PK_FOL_MAE_ID == 1//abierta
            //                 orderby reb.FL_FOL_REB_FECHA descending
            //                 select new { 
            //                     rec.PK_FOL_REC_ID, 
            //                     rec.PK_FOL_OTP_ID,
            //                     rec.PK_FOL_OTD_ID,
            //                     reb.PK_FOL_BLT_ID };

            var oRecepcion = (from rec in oFol.PR_FOL_ULTIMA_RECEPCION(oValidation.GetSucursalIDfromActiveUser())
                             select rec).ToList();            
            if (oRecepcion.Count() > 0)
            {
                var oDatos = oRecepcion.ToList()[0];
                NumeroManifiesto = oDatos.PK_FOL_REC_ID;
                OTP = oDatos.PK_FOL_OTP_ID;
                OTD = oDatos.PK_FOL_OTD_ID;
                decIdBulto = oDatos.PK_FOL_BLT_ID;
            }
            else
            {
                NumeroManifiesto = 0;
                OTP = 0;
                OTD = 0;
            }
        }   
        #endregion
        #region GetRecepcionActiva
        public void GetRecepcionActiva()
        {
            Controllers.ValidationController oValidation = new Controllers.ValidationController();

            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            
            var oRecepcion = from rec in oFol.TB_FOL_REC_RECEPCION
                             where 
                             //rec.PK_PAR_USU_RUT == oValidation.GetRutActiveUser()&& 
                             rec.PK_PAR_SUC_ID==oValidation.GetSucursalIDfromActiveUser()
                             && rec.PK_FOL_MAE_ID==1//Abierta                             
                             && rec.PK_FOL_OTP_ID==OTP
                             && rec.PK_FOL_OTD_ID==OTD
                             select rec.PK_FOL_REC_ID;

            if (oRecepcion.Count() > 0)
                NumeroManifiesto= oRecepcion.ToList()[0];
            else
                NumeroManifiesto=0;


        }   
        #endregion
        #region GetDatosOTPOTD
        public void GetDatosOTPOTD()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = (from blt in oFol.TB_FOL_BLT_BULTO                         
                          where blt.PK_FOL_BLT_ID == decIdBulto
                          select new { blt.PK_FOL_OTP_ID, blt.PK_FOL_OTD_ID }).Single();
            OTP = oDatos.PK_FOL_OTP_ID;
            OTD = oDatos.PK_FOL_OTD_ID;            
        }
        #endregion

    }
    public class RecepcionManifiestoModels
    {
        [Display (Name="N° Manifiesto")]
        public string NumeroManifiesto { get; set; }

        public string PantNumeroManifiesto { get; set; }

        public IEnumerable<RecepcionManifiestoDatos> oListaDatosManifiesto { get; set; }

        public void GetListaDatosManifiesto()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            decimal decMan = Convert.ToDecimal(NumeroManifiesto);
            var oLista = from pr in oFol.PR_FOL_MAN_CONSULTA(decMan)
                         select new RecepcionManifiestoDatos
                         {
                             Bultos = pr.BULTOS,
                             OTP = pr.PK_FOL_OTP_ID,
                             OTD = pr.PK_FOL_OTD_ID,
                             Peso_total = pr.PESO_FISICO,
                             Servicio = pr.FL_PAR_SER_NOMBRE
                         };
            oListaDatosManifiesto = oLista.ToList();
        }
    }
    public class RecepcionManifiestoDatos
    {
        public decimal OTP { get; set; }
        public decimal OTD { get; set; }
        public string Servicio { get; set; }
        public int? Bultos { get; set; }
        public double? Peso_total { get; set; }
    }
}

