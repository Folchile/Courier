using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Courier.Models;
using System.IO;
using ClosedXML.Excel;


namespace Courier.Controllers
{
    public class CargaController : Controller
    {

        #region EntregaTransporte
        public ActionResult EntregaTransporte(CargaModels oModel)
        {
            return View("EntregaTransporte", oModel);
        }
        #endregion

        #region GrabarEntregaForm
        public ActionResult GrabarEntregaForm(FormCollection form)
        {
            var ch = form.GetValues("assignChkBx");
            RetornoModels oRetorno = new RetornoModels();
            oRetorno.Ok = true;
            oRetorno.Mensaje = "Manifiestos Actualizado!"; 
            CargaModels oModel = new CargaModels();
            if (ch != null)
            {
                try
                {

                    foreach (var id in ch)
                    {
                        oModel.GuardarEntrega(decimal.Parse(id.ToString()));
                    }
                }
                catch (Exception ex)
                {
                    oRetorno.Ok = false;
                    oRetorno.Mensaje = "Error: " + ex.Message;
                }
            }
            else
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = "Error: Debe seleccionar los manifiestos que entregará a transporte";
            }
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region EntregaManifiesto
        public ActionResult EntregaManifiesto(CargaModels oModel)
        {

            Courier.Models.CargaModels.ReturnManifiestoGuardar oResult = new Courier.Models.CargaModels.ReturnManifiestoGuardar();
            oResult = oModel.GuardarEntrega(oModel.NumeroManifiesto);
            return Json(oResult, JsonRequestBehavior.AllowGet);

            
        }
        #endregion

        #region EditarPatente
        [Authorize]
        public ActionResult EditarPatente(CargaModels oModel)
        {
            ValidationController oValidation = new ValidationController();

            oModel.NumeroManifiesto = Convert.ToDecimal(oModel.NumeroManifiesto);    
            oModel.Proveedor = oValidation.GetProveedorManifiesto(oModel.NumeroManifiesto);
            oModel.Patente = oValidation.GetPatenteManifiesto(oModel.NumeroManifiesto);
            oModel.Conductor = oValidation.GetConductorManifiesto(oModel.NumeroManifiesto);  
            oModel.GetListaProveedores();
            oModel.GetListaPatentes();
            oModel.GetListaConductores();

            return PartialView("_EditarPatente", oModel);
        }
        #endregion
        #region DropDownConductor
        public ActionResult DropDownConductor(string idPatente)
        {
            CargaModels oModel = new CargaModels();

            oModel.Patente = idPatente;
            oModel.GetListaConductores();
            return PartialView("_DropDownConductor", oModel);
        }
        #endregion

        #region DropDownPatente
        public ActionResult DropDownPatente(string idProveedor)
        {
            CargaModels oModel = new CargaModels();

            oModel.Proveedor = idProveedor;
            oModel.GetListaPatentesProveedor();
            return PartialView("_DropDownPatente", oModel);
        }
        #endregion
        #region GuardarPatenteManifiesto
        public ActionResult GuardarPatenteManifiesto(int NumeroManifiesto, string Patente)
        {
            ValidationController oValidation = new ValidationController();
            int? intPatente = null;

            if (oValidation.IsNumeric2(Patente))
                intPatente = Convert.ToInt32(Patente);

            try
            {

                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                var oDatos = (from man in oFol.TB_FOL_MAN_MANIFIESTO
                              where man.PK_FOL_MAN_ID == NumeroManifiesto
                              select man).Single();

                oDatos.PK_PAR_TRA_ID = intPatente;
                oFol.SubmitChanges();

                var oMab = from mab in oFol.TB_FOL_MAB_MANIFIESTO_BULTO
                           join blt in oFol.TB_FOL_BLT_BULTO on mab.PK_FOL_BLT_ID equals blt.PK_FOL_BLT_ID
                           where mab.PK_FOL_MAN_ID == NumeroManifiesto
                           select blt;

                return Json(true, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region ListaManifiestoCarga 
        public ActionResult ListaManifiestoCarga()
        {
            CargaModels oModel = new CargaModels();
            oModel.GetListaManCargas();
            oModel.GetListaManCargadosHoy();
            return PartialView("_ListaManifiestoCarga", oModel);
        }
        #endregion

        #region GuardarPatente
        public JsonResult GuardarPatente(int? provId, int patId, int condId, decimal MAN_ID)
        {

            ValidationController oValidation = new ValidationController();
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            //var oResult = oValidation.vjSonValidarIdBultoDespacho(idBulto, MAN_ID);
            RetornoModels oRetorno = new RetornoModels();
            oRetorno.Ok = true;
            oRetorno.Mensaje = "Manifiesto Actualizado!";

            //int intPatente = 0;
            try
            {
               
                if (patId == 0)
                {
                    oRetorno.Ok = false;
                    oRetorno.Mensaje = "Debe seleccionar una Patente";
                    return Json(oRetorno, JsonRequestBehavior.AllowGet);
                }
                if (condId == 0)
                {
                    oRetorno.Ok = false;
                    oRetorno.Mensaje = "Debe seleccionar un Conductor";
                    return Json(oRetorno, JsonRequestBehavior.AllowGet);
                }

                if (oRetorno.Ok == true)
                {

                    LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                    var oDatos = (from man in oFol.TB_FOL_MAN_MANIFIESTO
                                  where man.PK_FOL_MAN_ID == MAN_ID
                                  select man).Single();

                    oDatos.PK_PAR_TRA_ID = patId;
                    oDatos.PK_PAR_CON_RUT = condId;
                    oDatos.PK_PAR_PRO_ID = provId;
                    oFol.SubmitChanges();

                }
                else
                {
                    oRetorno.Ok = false;
                    oRetorno.Mensaje = oRetorno.Mensaje;
                }
            }
            catch (Exception e)
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = "Error al guardar datos: " + e.Message + " pongase en contacto con el administrador.";
            }

            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }
        #endregion 

        #region DescargarExcelMan
        public ActionResult DescargarExcelMan(decimal NumeroManifiesto)
        {

            //DateTime FechaInicial = Convert.ToDateTime(FECINI);
            //DateTime FechaFinal = Convert.ToDateTime(FECTER);

           


            MemoryStream Stream = new MemoryStream();
            var workbook = new XLWorkbook();
            //var worksheet = workbook.Worksheets.Add("Planilla Transporte");


            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            oFol.CommandTimeout = 600;

            var oDatos = (from exc in oFol.PR_FOL_MRC_MANIFIESTO_ENTREGA_EXCEL(NumeroManifiesto).AsEnumerable() select exc).ToList();


            var oResult = oDatos.ToDataSet();

            workbook.Worksheets.Add(oResult);

            workbook.Worksheet(1).Name = "Manifiestos ";

            workbook.SaveAs(Stream);

            Response.ClearHeaders();
            Response.ClearContent();
            Response.Clear();

            Response.AddHeader("Content-Type", "application/vnd.ms-excel.12");

            Response.AddHeader("Content-Disposition", "attachment; filename=\"InformeManifiestos.xlsx\"");

            Stream.WriteTo(Response.OutputStream);
            Stream.Close();
            Response.Flush();
            Response.End();

            return View();
        }
        #endregion
    }
}

