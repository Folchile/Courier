using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Xml.Linq;
using System.Web.Security;
using System.Globalization;
using System.Data.Metadata.Edm;
using System.IO;
using System.Text;
using Courier.Models;
using DocumentFormat.OpenXml;
using ClosedXML.Excel;
using System.Net.Mime;
using System.Data;




namespace Courier.Controllers
{
    public class TransporteController : Controller
    {
        //
        // GET: /Transporte/

        public ActionResult Transporte()
        {
            //VehiculosModels tModel = new VehiculosModels();
            //tModel.GetListaVehiculos("0", 1);
            //return View("DetalleVehiculos", tModel);
            return View();
        }



        [HttpPost]
        public ActionResult EliminaAsociSuc(VehiculosModels tModel)
        {
            Clases.Retorno oRetorno = new Clases.Retorno();
            //VehiculosModels tModel = new VehiculosModels();
            oRetorno = tModel.DeleteRelVehic(tModel.IDSUC, tModel.IDVEH);
            //oRetorno = tModel.DeleteRelVehic(Convert.ToInt32(oForm["id_Asu"]), Convert.ToInt32(oForm["id_veh"]));
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult InactivarVehiculo(FormCollection vForm)
        {
            VehiculosModels vModel = new VehiculosModels();
            RetornoModels oRetorno = new RetornoModels();
            oRetorno = vModel.CambiaestadoVehiculo(Convert.ToInt32(vForm["inac_veh"]), 2);
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult ActivarVehiculo(FormCollection vForm)
        {
            VehiculosModels vModel = new VehiculosModels();
            RetornoModels oRetorno = new RetornoModels();
            oRetorno = vModel.CambiaestadoVehiculo(Convert.ToInt32(vForm["act_veh"]), 1);
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CargDetVehiculos(VehiculosModels tModel)
        {
            tModel.GetListaVehiculos(tModel.BUSCARPATENTE);
            return PartialView("DetalleVehiculos", tModel);
        }

        public ActionResult CargDetSucursales(VehiculosModels tModel)
        {
            
            LinqBD_PARDataContext oVeh = new LinqBD_PARDataContext();
            var vDatos = from v in oVeh.TB_PAR_TRA_TRANSPORTE
                         where v.PK_PAR_TRA_ID == tModel.IDVEH
                         select v;

            foreach (var oFile in vDatos)
            {
                tModel.PATENTE = oFile.FL_PAR_TRA_PATENTE;
            }

            tModel.GetListaSucAsoc(tModel.IDVEH);
            return PartialView("DetalleSucursales", tModel);
        }

        public ActionResult MostrarAgregaVehic()
        {
            VehiculosModels tModel = new VehiculosModels();
            return PartialView("AgregaVehiculo", tModel);
        }

        public ActionResult MostrarAgregaSuc(VehiculosModels tModel)
        {
            //VehiculosModels tModel = new VehiculosModels();
            tModel.GesListaSucFil(tModel.IDVEH);
            return PartialView("AgregarSucursal", tModel);
        }


        [HttpPost]
        public ActionResult InsertaVehiculo(VehiculosModels mModel)
        {
            Clases.Retorno  oRetorno = new Clases.Retorno();

            oRetorno = mModel.InsertVehic();
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult InsertaSuc(VehiculosModels mModel)
        {
            Clases.Retorno oRetorno = new Clases.Retorno();
            oRetorno = mModel.InsertDestTrans();
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult ActualizaVehiculo(VehiculosModels vModel)
        {
            
            Clases.Retorno oRetorno = new Clases.Retorno();
            oRetorno = vModel.UpdateVehiculo();
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult EditarVehiculo(FormCollection gForm)
        {
            VehiculosModels vModel = new VehiculosModels();
            LinqBD_FOLDataContext oVeh = new LinqBD_FOLDataContext();
            var vDatos = from v in oVeh.PR_FOL_EDITA_VEHICULO(Convert.ToInt32(gForm["id_veh"]))
                         select v;
   
            foreach (var oFile in vDatos)
            {
                vModel.IDVEH = oFile.PK_PAR_TRA_ID;
                vModel.PATENTE=oFile.FL_PAR_TRA_PATENTE;
                vModel.MARCA=oFile.FL_PAR_TRA_MARCA;
                vModel.MODELO=oFile.FL_PAR_TRA_MODELO;
                vModel.ANO=Convert.ToInt32(oFile.FL_PAR_TRA_ANIO);
                vModel.DESTINOCLIENTE=Convert.ToBoolean(oFile.FL_PAR_TRA_DESTINOCLIENTE);
                vModel.RAMPLA=Convert.ToBoolean(oFile.FL_PAR_TRA_RAMPLA);
                vModel.CAPKILOS=Convert.ToInt32(oFile.FL_PAR_TRA_CAPACIDAD_KGS);
                vModel.CAPM3=Convert.ToInt32(oFile.FL_PAR_TRA_CAPACIDAD_M3);
            }
            return PartialView("_editarVehiculo", vModel);
        }


        #region GeneraExcelVehiculos
        [Authorize]
        public ActionResult GeneraExcelVehiculos()
        {
            string Mater;
            Mater = null;
            ValidationController oValidation = new ValidationController();
            MemoryStream Stream = new MemoryStream();
            var workbook = new XLWorkbook();

            LinqBD_FOLDataContext oVex = new LinqBD_FOLDataContext();
            
            var oDatos = (from exc in oVex.PR_FOL_EXCEL_VEHICULOS()
                          select exc).ToList();

            var oResult = oDatos.ToDataSet();
            workbook.Worksheets.Add(oResult);


            workbook.Worksheet(1).Name = "Informe";

            workbook.SaveAs(Stream);
            Response.ClearHeaders();
            Response.ClearContent();
            Response.Clear();

            Response.AddHeader("Content-Type", "application/vnd.ms-excel.12");
            Response.AddHeader("Content-Disposition", "attachment; filename=\"Informe.xlsx\"");

            Stream.WriteTo(Response.OutputStream);
            Stream.Close();
            Response.Flush();
            Response.End();

            return View();
        }
        #endregion

    }
}
