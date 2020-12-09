using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Courier.Models;

namespace Courier.Controllers
{
    public class GenericoController : Controller
    {
        [Authorize]
        public ActionResult BuscarCliente(string TextoFiltro)
        {
            if (TextoFiltro == null) TextoFiltro = string.Empty;
            GenericoModels oModel = new GenericoModels();
            oModel.GetListaBusqueda(TextoFiltro);
            oModel.GetListaServicios(0);
            return PartialView("BuscarCliente",oModel);
            //return View(oModel);
        }
        public ActionResult GetListaServicios(string RutEmpresa)
        {            
            ValidationController oValidation = new ValidationController();
            int intRutEmpresa=0;
            if (oValidation.ValidarRut(RutEmpresa)==1)
            {
                intRutEmpresa = Convert.ToInt32(RutEmpresa.Substring(0, RutEmpresa.Length - 2));             
            }
                
            GenericoModels oModel = new GenericoModels();
            oModel.GetListaServicios(intRutEmpresa);
            
            return PartialView("_ListaServicioGenerico", oModel);
        }
        public ActionResult Formulariobuscarcliente()
        {
            GenericoModels oModel = new GenericoModels();
            oModel.GetListaServicios(0);
            return PartialView("_FormularioBuscarCliente",oModel);
        }
        
        public ActionResult DatosOTD(decimal OTP, decimal OTD)
        {
            GenericOTDModels oModel = new GenericOTDModels();
            ValidationController oValidation = new ValidationController();
            oModel.GetDatosOTD_DATOS(OTP, OTD);
            oModel.GetContraPago(OTP, OTD); 
            var oDatos = oValidation.GetNombreReferencia(Convert.ToDecimal(oModel.OTD_DATOS.PK_PAR_SER_ID));
            oModel.NombreReferencia1 = oDatos.NombreRef1;
            oModel.NombreReferencia2 = oDatos.NombreRef2;

            oModel.EditaVia= oValidation.TieneRolByName("Puede Cambiar Vía");
            return PartialView("DatosOTD",oModel);
        }

        [HttpPost]
        public ActionResult CambiaCarpetaMenu(int CarId, string Clase)
        {
            try
            {
                ValidationController oValidation = new ValidationController();

                int Usuario = oValidation.GetRutActiveUser();
                string oUsuario = System.Web.HttpContext.Current.User.Identity.Name;

                LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

                oPar.PR_PAR_CUS_CARPETA(Usuario, CarId, Clase);

                Autenticacion.Controllers.AccountController oAut = new Autenticacion.Controllers.AccountController();

                //Session["Menu"] = oAut.sConsultaMenu(oUsuario,null);
                Session["conMenu"] = "1";
            }
            catch (Exception e) { return Json(false); }

            return Json(true);
        }
        
        [HttpPost]
        public ActionResult CambiaCarpetaMenuS(int SBCId, string Clase)
        {
            try
            {
                ValidationController oValidation = new ValidationController();

                int Usuario = oValidation.GetRutActiveUser();
                string oUsuario = System.Web.HttpContext.Current.User.Identity.Name;

                LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

                oPar.PR_PAR_CUB_SUBCARPETA(Usuario,SBCId, Clase);

                Autenticacion.Controllers.AccountController oAut = new Autenticacion.Controllers.AccountController();

                Session["Menu"] = oAut.sConsultaMenu(oUsuario,null);
            }
            catch { return Json(false); }

            return Json(true);
        }

        
        public ActionResult ConsultaManHistorico(GenericManHistorico oModel)
        {
            oModel.GetDatosListaHistoria();
            return PartialView("_ListaManHis", oModel);
            
        }

        [Authorize]
        public ActionResult Observaciones(ObservacionesModels oModel)
        {
            oModel.OTP_Obs = oModel.OTP;
            oModel.OTD_Obs = oModel.OTD;

            oModel.GetListaObservaciones();
            oModel.GetListaTipoObs();
            oModel.GetHabilitadoObs();
            
            return PartialView("_Observaciones", oModel);
        }

        [Authorize]
        public ActionResult FormularioObservaciones(ObservacionesModels oModel)
        {
            oModel.GetListaTipoObs();
            return PartialView("_FormularioObservaciones", oModel);
        }
        [Authorize]
        public ActionResult AgregarObservacion(ObservacionesModels oModel)
        {
            
            RetornoModels oResult = new RetornoModels();
            try
            {
                ValidationController oValidation = new ValidationController();
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                TB_FOL_OBS_OBSERVACION oObs = new TB_FOL_OBS_OBSERVACION
                {
                    FL_FOL_OBS_FECHA = DateTime.Now,
                    PK_FOL_OTP_ID = oModel.OTP_Obs,
                    PK_FOL_OTD_ID = oModel.OTD_Obs,
                    PK_PAR_SUC_ID = oValidation.GetSucursalIDfromActiveUser(),
                    PK_PAR_USU_RUT = oValidation.GetRutActiveUser(),
                    PK_FOL_TOB_ID = Convert.ToInt32(oModel.TipoObservacion)
                };
                oFol.TB_FOL_OBS_OBSERVACION.InsertOnSubmit(oObs);
                oFol.SubmitChanges();
                oResult.Ok = true;                
            }
            catch (Exception e)
            {
                oResult.Ok = false;
                oResult.Mensaje = "<p>No fue posible guardar la información</p>" + e.Message;
            }            

            return Json(oResult);
        }

        [Authorize]
        public ActionResult BuscarCond(string TextoFiltro)
        {
            if (TextoFiltro == null) TextoFiltro = string.Empty;
            GenericCondModels oModel = new GenericCondModels();
            oModel.GetListaBuscaCond(TextoFiltro);
            return PartialView("BuscarConductor", oModel);
        }


        [Authorize]
        public ActionResult BuscarPtte(string TextoFiltro)
        {
            if (TextoFiltro == null) TextoFiltro = string.Empty;
            GenericPteModels oModel = new GenericPteModels();
            oModel.GetListaBuscaPtte(TextoFiltro);
            return PartialView("BuscarPatente", oModel);
        }

    }
}
