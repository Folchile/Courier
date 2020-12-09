using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Courier.Models;

namespace Courier.Controllers
{
    public class RecepcionController : Controller
    {
        //
        // GET: /Recepcion/

        #region Index
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
        #endregion
        #region AgregarBulto
        [Authorize]
        public ActionResult AgregarBulto()
        {            
            RecepcionModels oModel= new RecepcionModels();
            oModel = TempData["ModelCarga"] as RecepcionModels;
            TempData["ModelCarga"] = oModel;
            return PartialView("_VerBulto", oModel);

        }

        [Authorize][HttpPost]
        public JsonResult AgregarBulto(RecepcionModels oModel)
        {
            RetornoModels oRetorno = new RetornoModels();          
            ValidationController oValidation = new ValidationController();
            oModel.decIdBulto = oValidation.GetCodigoBultoFromCodigoGenerico(oModel.strIdBulto);
            oModel.GetDatosOTPOTD();
            //oModel.GetDatosByIdBulto();
            oModel.GetRecepcionActiva();                

            if (oModel.NumeroManifiesto == 0)
            {
                oModel.CreaRecepcion(oValidation.GetRutActiveUser(), oValidation.GetSucursalIDfromActiveUser(), oModel.OTP, oModel.OTD);
            }
            oModel.AgregarBultoRecepcion(oModel.OTP, oModel.OTD, oModel.decIdBulto, oModel.NumeroManifiesto);
            //oModel.GetListaRecepcion();            
            TempData["ModelCarga"] = oModel;
            oRetorno.Ok = true;
            oRetorno.Comprobacion = oModel.Devolucion;
            return Json(oRetorno,JsonRequestBehavior.AllowGet);                              
        }
        #endregion
        #region ExisteManifiesto
        [Authorize][HttpPost]
        public decimal ExisteRecepcion()
        {
            ValidationController oValidation = new ValidationController();
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            int RutUsuario = 0;

            RutUsuario = oValidation.GetRutActiveUser();

            var oDatos = from rec in oFol.TB_FOL_REC_RECEPCION
                         where 
                         rec.PK_PAR_SUC_ID==oValidation.GetSucursalIDfromActiveUser()
                         && rec.PK_FOL_MAE_ID == 1                         
                         select rec.PK_FOL_REC_ID;

            if (oDatos.Count() > 0)
            {
                return oDatos.ToList()[0];
            }
            else
            {
                return 0;
            }
        }
        #endregion
        #region NuevaRecepcion
        [Authorize]
        public ActionResult NuevaRecepcion()
        {
            RecepcionModels oModel = new RecepcionModels();
            ValidationController oValidation = new ValidationController();

            int oRut = oValidation.GetRutActiveUser();
            int oSucursal = oValidation.GetSucursalIDfromActiveUser();

            oModel.CreaRecepcion(oRut, oSucursal,oModel.VerTotalOT.PK_FOL_OTP_ID,oModel.VerTotalOT.PK_FOL_OTD_ID);
            oModel.GetListaRecepcion();

            return PartialView("_VerBulto", oModel);
        }
        #endregion
        #region RecepcionAbierta
        [Authorize]
        public ActionResult RecepcionAbierta()
        {
            RecepcionModels oModel = new RecepcionModels();
            oModel.GetUltimaRecepcion();
            oModel.GetRecepcionActiva();
            oModel.GetListaRecepcion();            
            if (oModel.decIdBulto != 0)
                oModel.GetDatosByIdBulto();
            else
                oModel.GetDatosByUltimoMovimiento();

            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            if (oModel.VerTotalOT != null)
            {
                oModel.Devolucion = (from otd in oFol.TB_FOL_OTD_OT_DETALLE
                                     where otd.PK_FOL_OTP_ID == oModel.VerTotalOT.PK_FOL_OTP_ID && otd.PK_FOL_OTD_ID == oModel.VerTotalOT.PK_FOL_OTD_ID
                                     select otd.FL_FOL_OTD_DEVOLUCION).Single();
            }
            return PartialView("_VerBulto", oModel);
        }
        #endregion
        #region UltimaRecepcion
        [Authorize]
        public ActionResult UltimaRecepcion()
        {
            RecepcionModels oModel = new RecepcionModels();
            oModel.GetUltimaRecepcion();
            oModel.GetListaRecepcion();
            if (oModel.oListaRecepcion.Count()> 0)
            {
                oModel.decIdBulto = oModel.oListaRecepcion[0].idBulto;
            }
            //if (oModel.decIdBulto != 0)
            //    oModel.GetDatosByIdBulto();
            //else
            oModel.GetDatosByUltimoMovimiento();

            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            if (oModel.VerTotalOT != null)
            {
                oModel.Devolucion = (from otd in oFol.TB_FOL_OTD_OT_DETALLE
                                     where otd.PK_FOL_OTP_ID == oModel.VerTotalOT.PK_FOL_OTP_ID && otd.PK_FOL_OTD_ID == oModel.VerTotalOT.PK_FOL_OTD_ID
                                     select otd.FL_FOL_OTD_DEVOLUCION).Single();
            }
            return PartialView("_VerBulto", oModel);
        }
        #endregion
        #region ValidaRecpcionSucursal

        [Authorize]
        public ActionResult ValidaRecpcionSucursal(string CodigoBulto)
        {
            RetornoModels oResult = new RetornoModels();    
            ValidationController oValidation = new ValidationController();
            //var oBLT = oValidation.GetCodigoBultoFromCodigoGenerico(CodigoBulto);
            decimal oBLT = Convert.ToDecimal(CodigoBulto);

            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            var oSucList=from pr in oFol.PR_FOL_DEN_SUC_COBERTURA_BULTO(oBLT)
                             select pr;

            oResult.Ok = true;
            var oSucursal = oSucList.ToList();
            if (oSucursal.Count()>0)
            {
                
                if (oSucursal[0].PK_PAR_SUC_ID!=oValidation.GetSucursalIDfromActiveUser())
                {
                    var oMan = from rec in oFol.TB_FOL_REC_RECEPCION
                               join reb in oFol.TB_FOL_REB_RECEPCION_BULTO on rec.PK_FOL_REC_ID equals reb.PK_FOL_REC_ID
                               where 
                                    //rec.PK_PAR_USU_RUT==oValidation.GetRutActiveUser()
                                    rec.PK_PAR_SUC_ID==oValidation.GetSucursalIDfromActiveUser()
                                    && rec.PK_FOL_MAE_ID==1 //Abierto
                                    && reb.PK_FOL_BLT_ID==oBLT
                               select rec;
                    if (oMan.Count()==0)
                        oResult.Ok = false;
                    oResult.Mensaje = oValidation.GetSucursalNamefromIDSucursal(oSucursal[0].PK_PAR_SUC_ID);
                }
            }



            return Json(oResult);
        }
        #endregion
        #region Manifiesto
        [Authorize]
        public ActionResult Manifiesto()
        {
            return View();
        }
        #endregion

        #region BuscarManifiesto
        [Authorize]
        public ActionResult BuscarManifiesto(RecepcionManifiestoModels oModel)
        {
            oModel.GetListaDatosManifiesto();
            oModel.PantNumeroManifiesto = oModel.NumeroManifiesto;
            return PartialView("BuscarManifiesto", oModel);
        }
        #endregion
        #region ValidarManifiesto
        [Authorize]
        public ActionResult ValidarManifiesto(RecepcionManifiestoModels oModel)
        {
            RetornoModels oRetorno = new RetornoModels();
            ValidationController oValidation = new ValidationController();

            if (oValidation.isDecimal(oModel.NumeroManifiesto) == false)
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = "N° de manifiesto incorrecto";
            }
            else
            {
                try
                {
                    decimal decMan = Convert.ToDecimal(oModel.NumeroManifiesto);
                    LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                    var oMan = from man in oFol.TB_FOL_MAN_MANIFIESTO
                               where man.PK_FOL_MAN_ID == decMan
                               select man;
                    if (oMan.Count() == 0)
                    {
                        oRetorno.Ok = false;
                        oRetorno.Mensaje = "N° de manifiesto no existe";
                    }
                    else
                    {
                        oRetorno.Ok = true;
                    }
                }
                catch (Exception e)
                {
                    oRetorno.Ok = false;
                    oRetorno.Mensaje = string.Format("No fue posible validar el manifiesto: {0}",e.Message);
                }
            }
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region GuardaRecepcionManifiesto
        [Authorize]
        public ActionResult GuardaRecepcionManifiesto(RecepcionManifiestoModels oModel)
        {
            RetornoModels oRetorno = new RetornoModels();

            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            ValidationController oValidation = new ValidationController();

            decimal decMan=Convert.ToDecimal(oModel.PantNumeroManifiesto);


            var oBultos = from blt in oFol.TB_FOL_BLT_BULTO
                          join otd in oFol.TB_FOL_OTD_OT_DETALLE
                            on new { blt.PK_FOL_OTP_ID, blt.PK_FOL_OTD_ID } equals new { otd.PK_FOL_OTP_ID, otd.PK_FOL_OTD_ID }
                          join mab in oFol.TB_FOL_MAB_MANIFIESTO_BULTO
                            on blt.PK_FOL_BLT_ID equals mab.PK_FOL_BLT_ID
                          where mab.PK_FOL_MAN_ID == decMan
                          select new { blt,otd };
            int oSwt = 0;
            foreach (var oFila in oBultos)
            {
                if (oFila.blt.PK_FOL_EST_ID != 4 && oFila.blt.PK_FOL_EST_ID != 25)
                {
                    oRetorno.Ok = false;
                    oRetorno.Mensaje = "Ya se han recepcionado bultos del manifiesto, no se puede completar la acción.";
                    return Json(oRetorno, JsonRequestBehavior.AllowGet);
                }

                oSwt = 1;
            }

            int RutActiveUser=oValidation.GetRutActiveUser();
            int Sucursal=oValidation.GetSucursalIDfromActiveUser();

            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            var oSucursalExterna = from ext in oPar.TB_PAR_SUC_SUCURSAL
                                   where ext.PK_PAR_SUC_ID==Sucursal
                                   && ext.PK_PAR_SUC_ENTREGA_FORZADA==true
                                   select ext;
            int oEstado=12;
            if (oSucursalExterna.Count() > 0)
            {
                oEstado = 22;
            }

            if (oSwt == 1)
            {
                try
                {
                    foreach (var oFila in oBultos)
                    {
                        oFila.blt.PK_FOL_EST_ID = oEstado;
                        oFila.blt.PK_PAR_USU_RUT = RutActiveUser;
                        oFila.blt.PK_PAR_SUC_ID = Sucursal;
                        oFila.otd.PK_FOL_EST_ID = oEstado;
                        oFila.otd.PK_PAR_USU_RUT = RutActiveUser;
                        oFila.otd.PK_PAR_SUC_ID = Sucursal;
                    }
                    oFol.SubmitChanges();
                    oRetorno.Ok = true;
                    return Json(oRetorno, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    oRetorno.Ok=false;
                    oRetorno.Mensaje = string.Format("No fue posible guardar la información, Error: {0}",e.Message);
                    return Json(oRetorno, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = "No se encontraron bultos en el manifiesto";
                return Json(oRetorno, JsonRequestBehavior.AllowGet);
            }

            
        }
        #endregion

        #region GuardarRecepcionOT
        [HttpPost]
        public ActionResult GuardarRecepcionOT(RecepcionModels oModel)
        {
            ValidationController oValidation = new ValidationController();
            RetornoModels oRetorno = new RetornoModels();
            var oDatosOT = oValidation.TransformCodigoGenericoOTPOTD(oModel.RecOT);
            if (oDatosOT.Error == 0)
            {
                try
                {

                    LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                    oFol.PR_FOL_REC_RECEPCION_POR_OTD(oDatosOT.OTP, oDatosOT.OTD, oValidation.GetRutActiveUser());
                    oRetorno.Ok = true;
                }
                catch (Exception e)
                {
                    oRetorno.Ok = false;
                    oRetorno.Mensaje = string.Format("No fue posible guardar la información:{0}", e.Message);
                }

            }
            else
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = oDatosOT.ErrorMensaje;
            }
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
