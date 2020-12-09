using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Courier.Models;
namespace Courier.Controllers
{
    public class DevolucionController : Controller
    {
        //
        // GET: /Devolucion/
        public ActionResult DropDownEmpresa(DevolucionModels oModel)
        {
            oModel.GetDatosEmpresa();
            return PartialView("_DropDownEmpresa", oModel);
        }
        public ActionResult DropDownServicio(DevolucionModels oModel)
        {
            oModel.GetDatosServicio();
            return PartialView("_DropDownServicio", oModel);
        }
        public ActionResult ListaDevoluciones(DevolucionModels oModel)
        {
            oModel.GetDatosOTD_DATOS();
            return PartialView("_ListaDevoluciones",oModel);
        }
        
        public ActionResult ListaDevolucionesFiltrada(DevolucionModels oModel)
        {
            oModel.GetDatosOTD_DATOS();
            return PartialView("_ListaDevoluciones", oModel);
        }
        public ActionResult Index()
        {
            DevolucionModels oModel = new DevolucionModels();
            //oModel.GetDatosOTD_DATOS();
            oModel.CboBusquedaServicio = new List<SelectListItem>();
            oModel.CboBusquedaEmpresa = new List<SelectListItem>();

            List<SelectListItem> oFlt = new List<SelectListItem>();
            oFlt.Add(new SelectListItem {
                Text="VER SOLO INGRESO PENDIENTE",
                Value="1"
            });

            oModel.CboFiltroTipo = oFlt;


            return View(oModel);
        }
        public ActionResult FormularioIngreso(DevolucionModels oModel)
        {
            oModel.GetDatosRegion();
            oModel.GetDatosComuna(0);
            oModel.GetDatosLocalidad(0);
            oModel.GetDatosVia();
            return PartialView("_FormularioIngresoDireccion", oModel);
        }
        public ActionResult FormularioIngresoMasivo(DevolucionModels oModel)
        {
            oModel.GetDatosRegion();
            oModel.GetDatosComuna(0);
            oModel.GetDatosLocalidad(0);
            oModel.GetDatosVia();
            return PartialView("_FormularioIngresoMasivo", oModel);
        }
        public ActionResult DropDownComuna(string Region)
        {
            int intRegion = 0;
            ValidationController oValidation = new ValidationController();
            if (oValidation.IsNumeric2(Region) == true)
            {
                intRegion = Convert.ToInt32(Region);
            }
            DevolucionModels oModel = new DevolucionModels();
            oModel.GetDatosComuna(intRegion);
            return PartialView("_DropDownComuna", oModel);
            
        }
        public ActionResult DropDownLocalidad(string Comuna)
        {
            int idComuna = 0;
            ValidationController oValidation = new ValidationController();
            if (oValidation.IsNumeric2(Comuna) == true)
            {
                idComuna = Convert.ToInt32(Comuna);
            }
            DevolucionModels oModel = new DevolucionModels();
            oModel.GetDatosLocalidad(idComuna);
            return PartialView("_DropDownLocalidad", oModel);

        }
        public ActionResult GuardarDireccion(DevolucionModels oModel)
        {
            RetornoDevolucion oResult = new RetornoDevolucion();
            try
            {
                ValidationController oValidation = new ValidationController();
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

                if (oModel.OTP == 0 && oModel.OTD == 0 && oModel.EditEmpresa != "" && oModel.EditEmpresa != null)
                {
                    int? oEmpresa = null;
                    int? oServicio = null;

                    if (oModel.EditEmpresa != null && oModel.EditEmpresa != "")
                        oEmpresa = Convert.ToInt32(oModel.EditEmpresa);

                    if (oModel.EditServicio != null && oModel.EditServicio != "")
                        oServicio = Convert.ToInt32(oModel.EditServicio);

                    int oTipo = 2; //2 las toma todas
                    if (oModel.EditTipo != null && oModel.EditTipo != "")
                        oTipo = Convert.ToInt32(oModel.EditTipo);

                    int? Localidad=null;
                      if (oValidation.IsNumeric2(oModel.Localidad))
                            Localidad = Convert.ToInt32(oModel.Localidad);

                      oFol.PR_FOL_OTD_DATOS_DEVOLUCION_GUARDA_MASIVO(oValidation.GetSucursalIDfromActiveUser(),
                          oEmpresa, oServicio, oTipo,
                          Convert.ToInt32(oModel.Comuna),
                          oModel.Numero,
                          Convert.ToInt32(oModel.Via),
                          Localidad,
                          oModel.Direccion,
                          oValidation.GetRutActiveUser());
                }
                else
                {
                    var oOTD = (from otd in oFol.TB_FOL_OTD_OT_DETALLE
                                where otd.PK_FOL_OTP_ID == oModel.OTP
                                && otd.PK_FOL_OTD_ID == oModel.OTD
                                select otd
                                    ).Single();
                    int oTipoDireccion = 1;

                    if (oOTD.FL_FOL_OTD_DEVOLUCION == true)
                    {
                        oTipoDireccion = 2;
                    }

                    TB_FOL_DEN_DIRECCION_ENTREGA oDen = new TB_FOL_DEN_DIRECCION_ENTREGA()
                    {
                        PK_FOL_OTP_ID = oModel.OTP,
                        PK_FOL_OTD_ID = oModel.OTD,
                        PK_PAR_COM_ID = Convert.ToInt32(oModel.Comuna),
                        FL_FOL_DEN_DIRECCION = oModel.Direccion,
                        FL_FOL_DEN_NUMERO = oModel.Numero,
                        PK_PAR_USU_RUT = oValidation.GetRutActiveUser(),
                        PK_PAR_VIA_ID = Convert.ToInt32(oModel.Via),
                        PK_FOL_TDE_ID = oTipoDireccion //Tipo Devolución.
                    };

                    if (oValidation.IsNumeric2(oModel.Localidad))
                        oDen.PK_PAR_LOC_ID = Convert.ToInt32(oModel.Localidad);


                    oFol.TB_FOL_DEN_DIRECCION_ENTREGA.InsertOnSubmit(oDen);
                    oFol.SubmitChanges();
                }
                oResult.Mensaje = "Información guardada exitosamente.";
                oResult.OK = true;                
            }
            catch (Exception e)
            {
                oResult.OK = false;
                oResult.Mensaje = "Error al guardar la información, " + e.Message;
            }
            return Json(oResult);
        }
        public class RetornoDevolucion
        {
            public bool OK { get; set; }
            public string Mensaje { get; set; }
        }
        public class RetornoDevolucionOT:RetornoDevolucion
        {
            public decimal OTP { get; set; }
            public decimal OTD { get; set; }
        }
        public ActionResult ValidaOT(string OT)
        {
            RetornoDevolucionOT oRetorno = new RetornoDevolucionOT();
            ValidationController oValidation = new ValidationController();
            var oResult = oValidation.TransformCodigoGenericoOTPOTD(OT);            
            if (oResult.Error == 0)
            {
                oRetorno.OK = true;
                oRetorno.OTP = oResult.OTP;
                oRetorno.OTD = oResult.OTD;
            }
            else
            {
                oRetorno.OK = false;
                oRetorno.Mensaje = oResult.ErrorMensaje;
            }
            return Json(oRetorno,JsonRequestBehavior.AllowGet);
        }
    }
}
