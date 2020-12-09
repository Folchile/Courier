using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Courier.Models;
using System.IO;
namespace Courier.Controllers
{
    public class InformesController : Controller
    {
        //
        // GET: /Informes/
        [Authorize]        
        public ActionResult Index()
        {
            return View();
        }

        #region ConsultaEstado
        [Authorize]
        public ActionResult ConsultaEstado()
        {
            InformesConsultaEstadosModels oModel = new InformesConsultaEstadosModels();
            oModel.GetListaEstados();
            return View(oModel);
        }
        #endregion

        #region VerListaEstados
        public ActionResult VerListaEstados()
        {
            InformesConsultaEstadosModels oModel = new InformesConsultaEstadosModels();
            oModel = TempData["Model"] as InformesConsultaEstadosModels;
            TempData["Model"] = oModel;
            return PartialView("_VerListaEstados", oModel);
        }
        [Authorize]
        [HttpPost]
        public ActionResult VerListaEstados(InformesConsultaEstadosModels oModel)
        {
            oModel.GetListaInforme();
            TempData["Model"] = oModel;
            return PartialView("_VerListaEstados", oModel);
        }
        #endregion

        #region IndexConsultas
        [Authorize]
        public ActionResult IndexConsultas(string pEstado)
        {
            Controllers.ValidationController oValidation = new Controllers.ValidationController();
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();


            var oZonal = (from pr in oFol.PR_FOL_USU_ZONAL(System.Web.HttpContext.Current.User.Identity.Name)
                          select pr).Single();
            int Sucursal = 0;
            int intEstado = 0;
            if (oValidation.isDecimal(pEstado))
                intEstado = Convert.ToInt32(pEstado);

            if (oZonal.Cantidad == 0)
            {
                Sucursal = oValidation.GetSucursalIDfromActiveUser();
            }

            //Exception exeption = new Exception();
            RS2005.ReportingService2005 rs;
            RE2005.ReportExecutionService rsExec;

            // Create a new proxy to the web service
            rs = new RS2005.ReportingService2005();
            rsExec = new RE2005.ReportExecutionService();


            System.Net.NetworkCredential nwc = new System.Net.NetworkCredential();

            nwc.UserName = "AdminApp"; //AdminApp";
            nwc.Password = "AdminApp"; //"AdminApp";
            nwc.Domain = "HMIRA-PC"; //"FRANCISCOROJAS";

            rs.Credentials = nwc;
            rsExec.Credentials = nwc;

            var oParametro = from rep in oFol.TB_FOL_REP_REPORTE
                             select rep;

            var oEstado = from suc in oFol.TB_FOL_EST_ESTADO
                          select suc;

            string oServidor = oParametro.Where(m => m.PK_FOL_REP_ID == 1).Single().FL_FOL_REP_DIRECCION;
            string oReporte = "";
            string oNombreReporte = "";
            if (pEstado == "10")
            {
                oReporte = oParametro.Where(m => m.PK_FOL_REP_ID == 12).Single().FL_FOL_REP_DIRECCION;
                oNombreReporte = oParametro.Where(m => m.PK_FOL_REP_ID == 12).Single().FL_FOL_REP_NOMBRE;
            }
            else
            {
                oReporte = oParametro.Where(m => m.PK_FOL_REP_ID == 11).Single().FL_FOL_REP_DIRECCION;
                oNombreReporte = oParametro.Where(m => m.PK_FOL_REP_ID == 11).Single().FL_FOL_REP_NOMBRE;
            }



            string oNombre = oEstado.Where(m => m.PK_FOL_EST_ID == Convert.ToInt32(pEstado)).Single().FL_FOL_EST_NOMBRE;
            string oDescripcion = oEstado.Where(m => m.PK_FOL_EST_ID == Convert.ToInt32(pEstado)).Single().FL_FOL_EST_DESCRIPCION;
            if (oDescripcion == null) { oDescripcion = ""; }

            //int oSucursal = oValidation.GetSucursalIDfromActiveUser();

            rs.Url = oServidor + "reportservice2005.asmx";
            rsExec.Url = oServidor + "reportexecution2005.asmx";

            string historyID = null;
            string deviceInfo = null;
            string format = "excel";
            Byte[] results;
            string encoding = String.Empty;
            string mimeType = String.Empty;
            string extension = String.Empty;
            RE2005.Warning[] warnings = null;
            string[] streamIDs = null;

            // Path of the Report - XLS, PDF etc.
            //string fileName = @"c:\samplereport.pdf";
            // Name of the report - Please note this is not the RDL file.
            string _reportName = oReporte;
            string _historyID = null;
            bool _forRendering = false;
            RS2005.ParameterValue[] _values = null;
            RS2005.DataSourceCredentials[] _credentials = null;
            RS2005.ReportParameter[] _parameters = null;

            //try
            //{
            _parameters = rs.GetReportParameters(_reportName, _historyID, _forRendering, _values, _credentials);
            RE2005.ExecutionInfo ei = rsExec.LoadReport(_reportName, historyID);
            RE2005.ParameterValue[] parameters = new RE2005.ParameterValue[3];

            if (_parameters.Length > 0)
            {
                parameters[0] = new RE2005.ParameterValue();
                parameters[0].Label = "Estado";
                parameters[0].Name = "Estado";
                parameters[0].Value = pEstado.ToString();

                parameters[1] = new RE2005.ParameterValue();
                parameters[1].Label = "Titulo";
                parameters[1].Name = "Titulo";
                parameters[1].Value = oNombre.ToString() + " " + oDescripcion.ToString();

                parameters[2] = new RE2005.ParameterValue();
                parameters[2].Label = "SUC_ID";
                parameters[2].Name = "SUC_ID";
                parameters[2].Value = Sucursal.ToString();

            }
            rsExec.SetExecutionParameters(parameters, "en-us");

            results = rsExec.Render(format, deviceInfo,
                      out extension, out encoding,
                      out mimeType, out warnings, out streamIDs);

            MemoryStream Stream = new MemoryStream(results);


            //    using (FileStream stream =  System.IO.File.OpenWrite(fileName))
            //    {
            //        stream.Write(results, 0, results.Length);
            //    }
            ////}
            ////catch (Exception ex)
            ////{
            ////    exeption = ex;
            ////}
            //    MemoryStream Stream = new MemoryStream();
            string fecha = String.Format("{0:yyyyMMdd}", DateTime.Now);
            Response.Clear();
            Response.AddHeader("Content-Type", "application/xls");
            //Response.AddHeader("Cache-Control", "no-cache");
            //Response.AddHeader("Accept-Ranges", "none");
            Response.AddHeader("Content-Disposition", "attachment; filename=InformeEstado" + "_" + oNombre + "_" + fecha + ".xls");

            // Enviar el archivo generado XLSX
            Stream.WriteTo(Response.OutputStream);
            Stream.Close();
            Response.Flush();
            Response.End();


            return View();
        }
        #endregion

    }
}
