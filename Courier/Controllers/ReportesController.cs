using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data.Metadata.Edm;
using Courier.Models;

//La libreria ClosedXML utiliza OpenXML, para Leer Archivos XLSX
using DocumentFormat.OpenXml;
using ClosedXML.Excel;
using System.Net.Mime;

using System.Data;


namespace Courier.Controllers
{
    public class ReportesController : Controller
    {
        //
        // GET: /Reportes/
        #region IndexTransporte
        [Authorize]
        public ActionResult IndexTransporte(string pFecIni, string pFecTer, string pSuc)
        {
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


            // Authenticate to the Web service using Windows credentials
            //rs.Credentials = System.Net.CredentialCache.DefaultCredentials;
            //rsExec.Credentials = System.Net.CredentialCache.DefaultCredentials;

            rs.Credentials = nwc;
            rsExec.Credentials = nwc;

            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            var oParametro = from rep in oFol.TB_FOL_REP_REPORTE
                             select rep;

            string oServidor = oParametro.Where(m => m.PK_FOL_REP_ID == 1).Single().FL_FOL_REP_DIRECCION;
            string oReporte = oParametro.Where(m => m.PK_FOL_REP_ID == 9).Single().FL_FOL_REP_DIRECCION;

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
            pFecIni = String.Format("{0:yyyyMMdd}", Convert.ToDateTime(pFecIni));
            pFecTer = String.Format("{0:yyyyMMdd}", Convert.ToDateTime(pFecTer));

            if (_parameters.Length > 0)
            {
                parameters[0] = new RE2005.ParameterValue();
                parameters[0].Label = "FECINI";
                parameters[0].Name = "FECINI";
                parameters[0].Value = pFecIni.ToString();

                parameters[1] = new RE2005.ParameterValue();
                parameters[1].Label = "FECTER";
                parameters[1].Name = "FECTER";
                parameters[1].Value = pFecTer.ToString();

                parameters[2] = new RE2005.ParameterValue();
                parameters[2].Label = "ID_SUC";
                parameters[2].Name = "ID_SUC";
                parameters[2].Value = pSuc.ToString();
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
            Response.AddHeader("Content-Disposition", "attachment; filename=InformeTransporte_" + fecha + ".xls");

            // Enviar el archivo generado XLSX
            Stream.WriteTo(Response.OutputStream);
            Stream.Close();
            Response.Flush();
            Response.End();


            return View();
        }
        #endregion

        #region IndexHojaRuta
        [Authorize]
        public ActionResult IndexHojaRuta(string pFecIni, string pFecTer, string pSuc)
        {
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


            // Authenticate to the Web service using Windows credentials
            //rs.Credentials = System.Net.CredentialCache.DefaultCredentials;
            //rsExec.Credentials = System.Net.CredentialCache.DefaultCredentials;

            rs.Credentials = nwc;
            rsExec.Credentials = nwc;

            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            var oParametro = from rep in oFol.TB_FOL_REP_REPORTE
                             select rep;

            string oServidor = oParametro.Where(m => m.PK_FOL_REP_ID == 1).Single().FL_FOL_REP_DIRECCION;
            string oReporte = oParametro.Where(m => m.PK_FOL_REP_ID == 8).Single().FL_FOL_REP_DIRECCION;

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
            RE2005.ParameterValue[] parameters = new RE2005.ParameterValue[2];
            pFecIni = String.Format("{0:yyyyMMdd}", DateTime.Now);

            if (_parameters.Length > 0)
            {

                parameters[0] = new RE2005.ParameterValue();
                parameters[0].Label = "ID_SUC";
                parameters[0].Name = "ID_SUC";
                parameters[0].Value = pSuc.ToString();

                parameters[1] = new RE2005.ParameterValue();
                parameters[1].Label = "FECINI";
                parameters[1].Name = "FECINI";
                parameters[1].Value = pFecIni.ToString();


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
            Response.AddHeader("Content-Disposition", "attachment; filename=InformeHojaRuta_" + fecha + ".xls");

            // Enviar el archivo generado XLSX
            Stream.WriteTo(Response.OutputStream);
            Stream.Close();
            Response.Flush();
            Response.End();


            return View();
        }
        #endregion

        #region IndexStock
        [Authorize]
        public ActionResult IndexStock(string pSuc)
        {
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


            // Authenticate to the Web service using Windows credentials
            //rs.Credentials = System.Net.CredentialCache.DefaultCredentials;
            //rsExec.Credentials = System.Net.CredentialCache.DefaultCredentials;

            rs.Credentials = nwc;
            rsExec.Credentials = nwc;

            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            var oParametro = from rep in oFol.TB_FOL_REP_REPORTE
                             select rep;

            string oServidor = oParametro.Where(m => m.PK_FOL_REP_ID == 1).Single().FL_FOL_REP_DIRECCION;
            string oReporte = oParametro.Where(m => m.PK_FOL_REP_ID == 6).Single().FL_FOL_REP_DIRECCION;

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
            RE2005.ParameterValue[] parameters = new RE2005.ParameterValue[1];

            if (_parameters.Length > 0)
            {
                parameters[0] = new RE2005.ParameterValue();
                parameters[0].Label = "ID_SUC";
                parameters[0].Name = "ID_SUC";
                parameters[0].Value = pSuc.ToString();
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
            Response.AddHeader("Cache-Control", "no-cache");
            Response.AddHeader("Accept-Ranges", "none");
            Response.AddHeader("Content-Disposition", "attachment; filename=InformeStock_" + fecha + ".xls");

            // Enviar el archivo generado XLSX
            Stream.WriteTo(Response.OutputStream);
            Stream.Close();
            Response.Flush();
            Response.End();


            return View();
        }
        #endregion

        #region IndexDevolucion
        [Authorize]
        public ActionResult IndexDevolucion(string pFecIni, string pFecTer, string pSuc)
        {
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


            // Authenticate to the Web service using Windows credentials
            //rs.Credentials = System.Net.CredentialCache.DefaultCredentials;
            //rsExec.Credentials = System.Net.CredentialCache.DefaultCredentials;

            rs.Credentials = nwc;
            rsExec.Credentials = nwc;

            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            var oParametro = from rep in oFol.TB_FOL_REP_REPORTE
                             select rep;

            string oServidor = oParametro.Where(m => m.PK_FOL_REP_ID == 1).Single().FL_FOL_REP_DIRECCION;
            string oReporte = oParametro.Where(m => m.PK_FOL_REP_ID == 7).Single().FL_FOL_REP_DIRECCION;

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
            pFecIni = String.Format("{0:yyyyMMdd}", Convert.ToDateTime(pFecIni));
            pFecTer = String.Format("{0:yyyyMMdd}", Convert.ToDateTime(pFecTer));

            if (_parameters.Length > 0)
            {
                parameters[0] = new RE2005.ParameterValue();
                parameters[0].Label = "FECINI";
                parameters[0].Name = "FECINI";
                parameters[0].Value = pFecIni.ToString();

                parameters[1] = new RE2005.ParameterValue();
                parameters[1].Label = "FECTER";
                parameters[1].Name = "FECTER";
                parameters[1].Value = pFecTer.ToString();

                parameters[2] = new RE2005.ParameterValue();
                parameters[2].Label = "ID_SUC";
                parameters[2].Name = "ID_SUC";
                parameters[2].Value = pSuc.ToString();
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
            Response.AddHeader("Content-Disposition", "attachment; filename=InformeDevolucion_" + fecha + ".xls");

            // Enviar el archivo generado XLSX
            Stream.WriteTo(Response.OutputStream);
            Stream.Close();
            Response.Flush();
            Response.End();


            return View();

        }
        #endregion

        #region ConsultaReportes
        public ActionResult ConsultaReportes()
        {
            ReportesModels oModel = new ReportesModels();
            oModel.GetListaReportes();
            return View("ConsultaReportes", oModel);
        }
        #endregion

        #region VerListaReportes
        public ActionResult VerFiltroReportes(string Reportes)
        {
            ValidationController oValidation = new ValidationController();      
            var oSucEmpresa = oValidation.GetSucursalEmpresaIDfromActiveUser(); 

            ReportesModels oModel = new ReportesModels();
            oModel.RutCli = oSucEmpresa; 					
            oModel.GetSucursales();
            oModel.Reportes = Reportes;
            return PartialView("FiltrosReportes", oModel);
        }

        [Authorize]
        [HttpPost]
        public ActionResult VerFiltroReportes(ReportesModels oModel)
        {
            oModel.GetListaReportes();
            TempData["Model"] = oModel;
            return PartialView("FiltrosReportes", oModel);
        }
        #endregion

        #region PendienteRecepcion
        public ActionResult PendienteRecepcion()
        {
            EnRutaTroncalModels oModel = new EnRutaTroncalModels();
            oModel.GetDatosListaRecepcion();
            return View("EnRutaTroncalASucursal", oModel);
        }
        #endregion
        #region PendienteRecepcionDetalle
        public ActionResult PendienteRecepcionDetalle(EnRutaTroncalModels oModel)
        {
            oModel.GetDatosListaRecepcionDetalle();
            return PartialView("_PendienteRecepcionDetalle", oModel);
        }
        #endregion


        #region GeneraExcelTransporte
        public ActionResult GeneraExcelTransporte(string FECINI, string FECTER, string SUC_ID)
        {

            //DateTime FechaInicial = Convert.ToDateTime(FECINI);
            //DateTime FechaFinal = Convert.ToDateTime(FECTER);

            string fecha1 = FECINI.Substring(6, 4) + FECINI.Substring(3, 2) + FECINI.Substring(0, 2);
            string fecha2 = FECTER.Substring(6, 4) + FECTER.Substring(3, 2) + FECTER.Substring(0, 2);

            string Fecha_Ini = FECINI.Substring(6, 4) + FECINI.Substring(3, 2) + FECINI.Substring(0, 2) + " 00:00:00.000";
            string Fecha_Fin = FECTER.Substring(6, 4) + FECTER.Substring(3, 2) + FECTER.Substring(0, 2) + " 23:59:59.999";

            MemoryStream Stream = new MemoryStream();
            var workbook = new XLWorkbook();
            //var worksheet = workbook.Worksheets.Add("Planilla Transporte");


            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            //LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            Linq_SPDataContext oSP = new Linq_SPDataContext();

            oSP.CommandTimeout = 600;

            var oDatos = (from exc in oSP.PR_FOL_INFORME_TRANSPORTE(fecha1, fecha2, Convert.ToInt32(SUC_ID)).AsEnumerable()
                          select new { 
                            OT=exc.OT,
                            DOCUMENTO=exc.FL_FOL_OTD_REFERENCIA1,
                            TOTAL_DOCTO=exc.FL_FOL_CANT_DOCTO,
                            FECHA_CREACION=exc.FL_FOL_OTP_FECHA_CREACION,
                            FECHA_ESTIMADA=exc.FL_FOL_OTD_FECHA_ESTIMADA,
                            FECHA_ENTREGA=exc.FECHA_ENTREGA_REAL,
                            SUCURSAL_ESTADO=exc.FL_SUCURSAL_ESTADO,
                            ESTADO=exc.FL_FOL_EST_NOMBRE,
                            FECHA_ESTADO=exc.FECHA_ESTADO,
                            BULTOS=exc.FL_FOL_OTD_BULTO,
                            PESO_FISICO=exc.FL_FOL_OTD_PESO,
                            PESO_VOLUMETRICO=exc.PESO_VOLUMETRICO,
                            PESO_FACTURADO=exc.PESO_FACTURADO,
                            SUCURSAL_ORIGEN=exc.FL_PAR_SUC_NOMBRE_ORG,
                            SUCURSAL_DESTINO=exc.FL_PAR_SUC_NOMBRE,
                            CLIENTE=exc.FL_CLI_EMP_FANTASIA,
                            SERVICIO=exc.FL_PAR_SER_NOMBRE,
                            SUB_CLIENTE=exc.FL_FOL_OTD_DESTINATARIO_NOMBRE,
                            RUT=exc.FL_FOL_OTD_DESTINATARIO_RUT,
                            DIRECCION_ENTREGA=exc.FL_FOL_OTD_DESTINATARIO_DIRECCION,
                            NUMERO=exc.FL_FOL_OTD_DESTINATARIO_NUMERO,
                            COMUNA=exc.FL_PAR_COM_NOMBRE,
                            IATA=exc.FL_PAR_COM_CODIGO,
                            TELEFONO=exc.FL_FOL_OTD_TELEFONO,
                            CONDUCTOR_TRONCAL=exc.FL_PAR_CON_NOMBRE_T,
                            MODELO_TRONCAL=exc.FL_PAR_TRA_MODELO_T,
                            PATENTE_TRONCAL=exc.FL_PAR_TRA_PATENTE_T,
                            CONDUCTOR_SUCURSAL=exc.FL_PAR_CON_NOMBRE_S,
                            MODELO_SUCURSAL=exc.FL_PAR_TRA_MODELO_S,
                            PATENTE_SUCURSAL=exc.FL_PAR_TRA_PATENTE_S,
                            PERSONA_RECIBE=exc.FL_FOL_ENT_RECIBE,
                            PERSONA_RUT=exc.FL_FOL_ENT_RUT,
                            REFERENCIA2=exc.FL_FOL_OTD_REFERENCIA2,
                            DESTINO_DEVOLUCION=exc.TIPO_DES_DEV,
                            ESTADO_DOCTO=exc.FL_FOL_EDO_NOMBRE,
                            FECHA_DOCTO=exc.FL_FOL_RDO_FECHA,
                            FECHA_PRIMERA_VISITA=exc.FL_FOL_PRIMERA_VISITA,
                            VIA=exc.FL_PAR_VIA_NOMBRE,
                            OT_PADRE=exc.OTPadre,
                            CONTRAPAGO=exc.FL_FOL_DOA_CONTRAPAGO,
                            OBSERVACION_SUCURSAL=exc.FL_FOL_TOB_NOMBRE,
                            OBSERVACION_ULTIMA_VISITA=exc.OBS_ULTIMO_ESTADO,
                            OBSERVACION1=exc.FL_FOL_OCA_OBSERVACION,
                            FECHA_ULTIMO_ESTADO=exc.FECHA_ULTIMO_ESTADO
                          }).ToList();

            

            
            var oResult=oDatos.ToDataSet();

            workbook.Worksheets.Add(oResult);

            workbook.Worksheet(1).Name = "Planilla Transporte";
            //workbook.Worksheet(1).Cell(1, 1).Value = "OT";
            //workbook.Worksheet(1).Cell(1, 2).Value = "DOCUMENTO";
            //workbook.Worksheet(1).Cell(1, 3).Value = "TOTAL DOCTO";
            //workbook.Worksheet(1).Cell(1, 4).Value = "'FECHA CREACIÓN";
            //workbook.Worksheet(1).Cell(1, 5).Value = "'FECHA ESTIMADA";
            //workbook.Worksheet(1).Cell(1, 6).Value = "'FECHA ENTREGA";
            //workbook.Worksheet(1).Cell(1, 7).Value = "'SUCURSAL ESTADO";
            //workbook.Worksheet(1).Cell(1, 8).Value = "'ESTADO";
            //workbook.Worksheet(1).Cell(1, 9).Value = "'FECHA ESTADO";
            //workbook.Worksheet(1).Cell(1, 10).Value = "'BULTOS";
            //workbook.Worksheet(1).Cell(1, 11).Value = "'PESO FÍSICO";
            //workbook.Worksheet(1).Cell(1, 12).Value = "'PESO VOLUMETRICO";
            //workbook.Worksheet(1).Cell(1, 13).Value = "'PESO FACTURADO";
            //workbook.Worksheet(1).Cell(1, 14).Value = "'SUCURSAL ORIGEN";
            //workbook.Worksheet(1).Cell(1, 15).Value = "'SUCURSAL DESTINO";
            //workbook.Worksheet(1).Cell(1, 16).Value = "'CLIENTE";
            //workbook.Worksheet(1).Cell(1, 17).Value = "'SERVICIO";
            //workbook.Worksheet(1).Cell(1, 18).Value = "'SUB-CLIENTE";
            //workbook.Worksheet(1).Cell(1, 19).Value = "'RUT";
            //workbook.Worksheet(1).Cell(1, 20).Value = "'DIRECCIÓN ENTREGA";
            //workbook.Worksheet(1).Cell(1, 21).Value = "'NÚMERO";
            //workbook.Worksheet(1).Cell(1, 22).Value = "'COMUNA";
            //workbook.Worksheet(1).Cell(1, 23).Value = "'IATA";
            //workbook.Worksheet(1).Cell(1, 24).Value = "'TELÉFONO";
            //workbook.Worksheet(1).Cell(1, 25).Value = "'CONDUCTOR TRONCAL";
            //workbook.Worksheet(1).Cell(1, 26).Value = "'MODELO";
            //workbook.Worksheet(1).Cell(1, 27).Value = "'PATENTE";
            //workbook.Worksheet(1).Cell(1, 28).Value = "'CONDUCTOR SUCURSAL";
            //workbook.Worksheet(1).Cell(1, 29).Value = "'MODELO";
            //workbook.Worksheet(1).Cell(1, 30).Value = "'PATENTE";
            //workbook.Worksheet(1).Cell(1, 31).Value = "'PERSONA RECIBE";
            //workbook.Worksheet(1).Cell(1, 32).Value = "'RUT RECIBE";
            //workbook.Worksheet(1).Cell(1, 33).Value = "'REFERENCIA";
            //workbook.Worksheet(1).Cell(1, 34).Value = "'DESTINO / DEVOLUCIÓN";
            //workbook.Worksheet(1).Cell(1, 35).Value = "'ESTADO DOCTO.";
            //workbook.Worksheet(1).Cell(1, 36).Value = "'FECHA DOCTO.";
            //workbook.Worksheet(1).Cell(1, 37).Value = "'FECHA PRIMERA VISITA";
            //workbook.Worksheet(1).Cell(1, 38).Value = "'VÍA";
            //workbook.Worksheet(1).Cell(1, 39).Value = "'OT PADRE";
            //workbook.Worksheet(1).Cell(1, 40).Value = "'CONTRAPAGO";
            //workbook.Worksheet(1).Cell(1, 41).Value = "'OBSERVACIÓN SUCURSAL";
            //workbook.Worksheet(1).Cell(1, 42).Value = "'OBSERVACIÓN ULTIMA VISITA";




            ////TITULOS            
            //worksheet.Cell(1, 1).Value = "'OT";
            //worksheet.Cell(1, 2).Value = "'DOCUMENTO";
            //worksheet.Cell(1, 3).Value = "'TOTAL DOCTO";
            //worksheet.Cell(1, 4).Value = "'FECHA CREACIÓN";
            //worksheet.Cell(1, 5).Value = "'FECHA ESTIMADA";
            //worksheet.Cell(1, 6).Value = "'FECHA ENTREGA";
            //worksheet.Cell(1, 7).Value = "'SUCURSAL ESTADO";
            //worksheet.Cell(1, 8).Value = "'ESTADO";
            //worksheet.Cell(1, 9).Value = "'FECHA ESTADO";
            //worksheet.Cell(1, 10).Value = "'BULTOS";
            //worksheet.Cell(1, 11).Value = "'PESO FÍSICO";
            //worksheet.Cell(1, 12).Value = "'PESO VOLUMETRICO";
            //worksheet.Cell(1, 13).Value = "'PESO FACTURADO";
            //worksheet.Cell(1, 14).Value = "'SUCURSAL ORIGEN";
            //worksheet.Cell(1, 15).Value = "'SUCURSAL DESTINO";
            //worksheet.Cell(1, 16).Value = "'CLIENTE";
            //worksheet.Cell(1, 17).Value = "'SERVICIO";
            //worksheet.Cell(1, 18).Value = "'SUB-CLIENTE";
            //worksheet.Cell(1, 19).Value = "'RUT";
            //worksheet.Cell(1, 20).Value = "'DIRECCIÓN ENTREGA";
            //worksheet.Cell(1, 21).Value = "'NÚMERO";
            //worksheet.Cell(1, 22).Value = "'COMUNA";
            //worksheet.Cell(1, 23).Value = "'IATA";
            //worksheet.Cell(1, 24).Value = "'TELÉFONO";
            //worksheet.Cell(1, 25).Value = "'CONDUCTOR TRONCAL";
            //worksheet.Cell(1, 26).Value = "'MODELO";
            //worksheet.Cell(1, 27).Value = "'PATENTE";
            //worksheet.Cell(1, 28).Value = "'CONDUCTOR SUCURSAL";
            //worksheet.Cell(1, 29).Value = "'MODELO";
            //worksheet.Cell(1, 30).Value = "'PATENTE";
            //worksheet.Cell(1, 31).Value = "'PERSONA RECIBE";
            //worksheet.Cell(1, 32).Value = "'RUT RECIBE";
            //worksheet.Cell(1, 33).Value = "'REFERENCIA";
            //worksheet.Cell(1, 34).Value = "'DESTINO / DEVOLUCIÓN";
            //worksheet.Cell(1, 35).Value = "'ESTADO DOCTO.";
            //worksheet.Cell(1, 36).Value = "'FECHA DOCTO.";
            //worksheet.Cell(1, 37).Value = "'FECHA PRIMERA VISITA";
            //worksheet.Cell(1, 38).Value = "'VÍA";
            //worksheet.Cell(1, 39).Value = "'OT PADRE";
            //worksheet.Cell(1, 40).Value = "'CONTRAPAGO";
            //worksheet.Cell(1, 41).Value = "'OBSERVACIÓN SUCURSAL";
            //worksheet.Cell(1, 42).Value = "'OBSERVACIÓN ULTIMA VISITA";

            //int oFila = 2;
            //foreach (var oElemento in oDatos)
            //{
            //    worksheet.Cell(oFila, 1).Value = "'" + oElemento.OT;
            //    worksheet.Cell(oFila, 2).Value = oElemento.FL_FOL_OTD_REFERENCIA1;
            //    worksheet.Cell(oFila, 3).Value = oElemento.FL_FOL_CANT_DOCTO;
            //    worksheet.Cell(oFila, 4).Value = oElemento.FL_FOL_OTP_FECHA_CREACION;
            //    worksheet.Cell(oFila, 5).Value = oElemento.FL_FOL_OTD_FECHA_ESTIMADA;
            //    worksheet.Cell(oFila, 6).Value = oElemento.FECHA_ENTREGA_REAL;
            //    worksheet.Cell(oFila, 7).Value = oElemento.FL_SUCURSAL_ESTADO;
            //    worksheet.Cell(oFila, 8).Value = oElemento.FL_FOL_EST_NOMBRE;
            //    worksheet.Cell(oFila, 9).Value = oElemento.FECHA_ESTADO;
            //    worksheet.Cell(oFila, 10).Value = oElemento.FL_FOL_OTD_BULTO;
            //    worksheet.Cell(oFila, 11).Value = oElemento.FL_FOL_OTD_PESO;
            //    worksheet.Cell(oFila, 12).Value = oElemento.PESO_VOLUMETRICO;
            //    worksheet.Cell(oFila, 13).Value = oElemento.PESO_FACTURADO;
            //    worksheet.Cell(oFila, 14).Value = oElemento.FL_PAR_SUC_NOMBRE_ORG;
            //    worksheet.Cell(oFila, 15).Value = oElemento.FL_PAR_SUC_NOMBRE;
            //    worksheet.Cell(oFila, 16).Value = oElemento.FL_CLI_EMP_FANTASIA;
            //    worksheet.Cell(oFila, 17).Value = oElemento.FL_PAR_SER_NOMBRE;
            //    worksheet.Cell(oFila, 18).Value = oElemento.FL_FOL_OTD_DESTINATARIO_NOMBRE;
            //    worksheet.Cell(oFila, 19).Value = oElemento.FL_FOL_OTD_DESTINATARIO_RUT;
            //    worksheet.Cell(oFila, 20).Value = oElemento.FL_FOL_OTD_DESTINATARIO_DIRECCION;//Revisar
            //    worksheet.Cell(oFila, 21).Value = oElemento.FL_FOL_OTD_DESTINATARIO_NUMERO;//Revisar
            //    worksheet.Cell(oFila, 22).Value = oElemento.FL_PAR_COM_NOMBRE;
            //    worksheet.Cell(oFila, 23).Value = oElemento.FL_PAR_COM_CODIGO;
            //    worksheet.Cell(oFila, 24).Value = oElemento.FL_FOL_OTD_TELEFONO;
            //    worksheet.Cell(oFila, 25).Value = oElemento.FL_PAR_CON_NOMBRE_T;
            //    worksheet.Cell(oFila, 26).Value = oElemento.FL_PAR_TRA_MODELO_T;
            //    worksheet.Cell(oFila, 27).Value = oElemento.FL_PAR_TRA_PATENTE_T;
            //    worksheet.Cell(oFila, 28).Value = oElemento.FL_PAR_CON_NOMBRE_S;
            //    worksheet.Cell(oFila, 29).Value = oElemento.FL_PAR_TRA_MODELO_S;
            //    worksheet.Cell(oFila, 30).Value = oElemento.FL_PAR_TRA_PATENTE_S;
            //    worksheet.Cell(oFila, 31).Value = oElemento.FL_FOL_ENT_RECIBE;
            //    worksheet.Cell(oFila, 32).Value = oElemento.FL_FOL_ENT_RUT;
            //    worksheet.Cell(oFila, 33).Value = oElemento.FL_FOL_OTD_REFERENCIA2;
            //    worksheet.Cell(oFila, 34).Value = oElemento.TIPO_DES_DEV;
            //    worksheet.Cell(oFila, 35).Value = oElemento.FL_FOL_EDO_NOMBRE;
            //    worksheet.Cell(oFila, 36).Value = oElemento.FL_FOL_RDO_FECHA;
            //    worksheet.Cell(oFila, 37).Value = oElemento.FL_FOL_PRIMERA_VISITA;
            //    worksheet.Cell(oFila, 38).Value = oElemento.FL_PAR_VIA_NOMBRE;
            //    worksheet.Cell(oFila, 39).Value = oElemento.OTPadre;
            //    worksheet.Cell(oFila, 40).Value = oElemento.FL_FOL_DOA_CONTRAPAGO;
            //    worksheet.Cell(oFila, 41).Value = oElemento.FL_FOL_TOB_NOMBRE;
            //    worksheet.Cell(oFila, 42).Value = oElemento.OBS_ULTIMO_ESTADO;
            //    oFila++;
            //}

            //for (int c = 1; c <= 42; c++)
            //{
            //    worksheet.Column(c).AdjustToContents();
            //}

            workbook.SaveAs(Stream);

            // set HTTP response headers
            Response.ClearHeaders();
            Response.ClearContent();
            Response.Clear();

            Response.AddHeader("Content-Type", "application/vnd.ms-excel.12");
            //Response.AddHeader("Cache-Control", "no-cache");
            //Response.AddHeader("Accept-Ranges", "none");
            Response.AddHeader("Content-Disposition", "attachment; filename=\"PlanillaTransporte_" + fecha1 + "AL" + fecha2 + ".xlsx\"");

            // Enviar el archivo generado XLSX
            Stream.WriteTo(Response.OutputStream);
            Stream.Close();
            Response.Flush();
            Response.End();

            return PartialView();
        }
        #endregion


        #region GeneraExcelTransporte_Konexia
        [Authorize]
        public ActionResult GeneraExcelTransporte_Konexia(string FECINI, string FECTER, string SUC_ID, int RUTCLI)
        {

            //DateTime FechaInicial = Convert.ToDateTime(FECINI);
            //DateTime FechaFinal = Convert.ToDateTime(FECTER);

            string fecha1 = FECINI.Substring(6, 4) + FECINI.Substring(3, 2) + FECINI.Substring(0, 2);
            string fecha2 = FECTER.Substring(6, 4) + FECTER.Substring(3, 2) + FECTER.Substring(0, 2);

            string Fecha_Ini = FECINI.Substring(6, 4) + FECINI.Substring(3, 2) + FECINI.Substring(0, 2) + " 00:00:00.000";
            string Fecha_Fin = FECTER.Substring(6, 4) + FECTER.Substring(3, 2) + FECTER.Substring(0, 2) + " 23:59:59.999";

            MemoryStream Stream = new MemoryStream();
            var workbook = new XLWorkbook();
            //var worksheet = workbook.Worksheets.Add("Planilla Transporte");


            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            oFol.CommandTimeout = 600;

            var oDatos = (from exc in oFol.PR_FOL_INFORME_TRANSPORTE_KONEXIA(fecha1, fecha2, Convert.ToInt32(SUC_ID), RUTCLI).AsEnumerable()
                          select new
                          {
                              OT = exc.OT,
                              DOCUMENTO = exc.FL_FOL_OTD_REFERENCIA1,
                              TOTAL_DOCTO = exc.FL_FOL_CANT_DOCTO,
                              FECHA_CREACION = exc.FL_FOL_OTP_FECHA_CREACION,
                              FECHA_ESTIMADA = exc.FL_FOL_OTD_FECHA_ESTIMADA,
                              FECHA_ENTREGA = exc.FECHA_ENTREGA_REAL,
                              SUCURSAL_ESTADO = exc.FL_SUCURSAL_ESTADO,
                              ESTADO = exc.FL_FOL_EST_NOMBRE,
                              FECHA_ESTADO = exc.FECHA_ESTADO,
                              BULTOS = exc.FL_FOL_OTD_BULTO,
                              PESO_FISICO = exc.FL_FOL_OTD_PESO,
                              PESO_VOLUMETRICO = exc.PESO_VOLUMETRICO,
                              PESO_FACTURADO = exc.PESO_FACTURADO,
                              SUCURSAL_ORIGEN = exc.FL_PAR_SUC_NOMBRE_ORG,
                              SUCURSAL_DESTINO = exc.FL_PAR_SUC_NOMBRE,
                              CLIENTE = exc.FL_CLI_EMP_FANTASIA,
                              SERVICIO = exc.FL_PAR_SER_NOMBRE,
                              SUB_CLIENTE = exc.FL_FOL_OTD_DESTINATARIO_NOMBRE,
                              RUT = exc.FL_FOL_OTD_DESTINATARIO_RUT,
                              DIRECCION_ENTREGA = exc.FL_FOL_OTD_DESTINATARIO_DIRECCION,
                              NUMERO = exc.FL_FOL_OTD_DESTINATARIO_NUMERO,
                              COMUNA = exc.FL_PAR_COM_NOMBRE,
                              IATA = exc.FL_PAR_COM_CODIGO,
                              TELEFONO = exc.FL_FOL_OTD_TELEFONO,
                              CONDUCTOR_TRONCAL = exc.FL_PAR_CON_NOMBRE_T,
                              MODELO_TRONCAL = exc.FL_PAR_TRA_MODELO_T,
                              PATENTE_TRONCAL = exc.FL_PAR_TRA_PATENTE_T,
                              CONDUCTOR_SUCURSAL = exc.FL_PAR_CON_NOMBRE_S,
                              MODELO_SUCURSAL = exc.FL_PAR_TRA_MODELO_S,
                              PATENTE_SUCURSAL = exc.FL_PAR_TRA_PATENTE_S,
                              PERSONA_RECIBE = exc.FL_FOL_ENT_RECIBE,
                              PERSONA_RUT = exc.FL_FOL_ENT_RUT,
                              REFERENCIA2 = exc.FL_FOL_OTD_REFERENCIA2,
                              DESTINO_DEVOLUCION = exc.TIPO_DES_DEV,
                              ESTADO_DOCTO = exc.FL_FOL_EDO_NOMBRE,
                              FECHA_DOCTO = exc.FL_FOL_RDO_FECHA,
                              FECHA_PRIMERA_VISITA = exc.FL_FOL_PRIMERA_VISITA,
                              VIA = exc.FL_PAR_VIA_NOMBRE,
                              OT_PADRE = exc.OTPadre,
                              CONTRAPAGO = exc.FL_FOL_DOA_CONTRAPAGO,
                              OBSERVACION_SUCURSAL = exc.FL_FOL_TOB_NOMBRE,
                              OBSERVACION_ULTIMA_VISITA = exc.OBS_ULTIMO_ESTADO,
                              OBSERVACION1 = exc.FL_FOL_OCA_OBSERVACION
                          }).ToList();




            var oResult = oDatos.ToDataSet();

            workbook.Worksheets.Add(oResult);

            workbook.Worksheet(1).Name = "Planilla Transporte";


            workbook.SaveAs(Stream);

            // set HTTP response headers
            Response.ClearHeaders();
            Response.ClearContent();
            Response.Clear();

            Response.AddHeader("Content-Type", "application/vnd.ms-excel.12");
            //Response.AddHeader("Cache-Control", "no-cache");
            //Response.AddHeader("Accept-Ranges", "none");
            Response.AddHeader("Content-Disposition", "attachment; filename=\"PlanillaTransporte_" + fecha1 + "AL" + fecha2 + ".xlsx\"");

            // Enviar el archivo generado XLSX
            Stream.WriteTo(Response.OutputStream);
            Stream.Close();
            Response.Flush();
            Response.End();

            return View();
        }
        #endregion

        
        #region GeneraExcelTransporte_TechData
        [Authorize]
        public ActionResult GeneraExcelTransporte_TechData(string FECINI, string FECTER, string SUC_ID, int RUTCLI)
        {

            //DateTime FechaInicial = Convert.ToDateTime(FECINI);
            //DateTime FechaFinal = Convert.ToDateTime(FECTER);

            string fecha1 = FECINI.Substring(6, 4) + FECINI.Substring(3, 2) + FECINI.Substring(0, 2);
            string fecha2 = FECTER.Substring(6, 4) + FECTER.Substring(3, 2) + FECTER.Substring(0, 2);

            string Fecha_Ini = FECINI.Substring(6, 4) + FECINI.Substring(3, 2) + FECINI.Substring(0, 2) + " 00:00:00.000";
            string Fecha_Fin = FECTER.Substring(6, 4) + FECTER.Substring(3, 2) + FECTER.Substring(0, 2) + " 23:59:59.999";

            MemoryStream Stream = new MemoryStream();
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Planilla Transporte");


            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            oFol.CommandTimeout = 600;

            var oDatos = (from exc in oFol.PR_FOL_INFORME_TRANSPORTE_TECHDATA(Fecha_Ini, Fecha_Fin, Convert.ToInt32(SUC_ID), RUTCLI)
                          select exc).ToList();



            //TITULOS            
            int oCC = 1;
            worksheet.Cell(1, oCC).Value = "'OT";
            oCC++;
            worksheet.Cell(1, oCC).Value = "'TIPO";
            oCC++;
            worksheet.Cell(1, oCC).Value = "'NUMERO";
            oCC++;
            worksheet.Cell(1, oCC).Value = "'TOTAL DOCTO";
            oCC++;
            worksheet.Cell(1, oCC).Value = "'FECHA CREACIÓN";
            oCC++;
            worksheet.Cell(1, oCC).Value = "'FECHA ESTIMADA";
            oCC++;
            worksheet.Cell(1, oCC).Value = "'FECHA ENTREGA";
            oCC++;
            worksheet.Cell(1, oCC).Value = "'SUCURSAL ESTADO";
            oCC++;
            worksheet.Cell(1, oCC).Value = "'ESTADO";
            oCC++;
            worksheet.Cell(1, oCC).Value = "'FECHA ESTADO";
            oCC++;
            worksheet.Cell(1, oCC).Value = "'BULTOS";
            oCC++;
            worksheet.Cell(1, oCC).Value = "'PESO FÍSICO";
            oCC++;
            worksheet.Cell(1, oCC).Value = "'PESO VOLUMETRICO";
            oCC++;
            worksheet.Cell(1, oCC).Value = "'SUCURSAL ORIGEN";
            oCC++;
            worksheet.Cell(1, oCC).Value = "'SUCURSAL DESTINO";
            oCC++;
            worksheet.Cell(1, oCC).Value = "'CLIENTE";
            oCC++;
            worksheet.Cell(1, oCC).Value = "'SERVICIO";
            oCC++;
            worksheet.Cell(1, oCC).Value = "'SUB-CLIENTE";
            oCC++;
            worksheet.Cell(1, oCC).Value = "'RUT";
            oCC++;
            worksheet.Cell(1, oCC).Value = "'DIRECCIÓN ENTREGA";
            oCC++;
            worksheet.Cell(1, oCC).Value = "'NÚMERO";
            oCC++;
            worksheet.Cell(1, oCC).Value = "'COMUNA";                        
            oCC++;
            worksheet.Cell(1, oCC).Value = "'TELÉFONO";
            oCC++;            
            worksheet.Cell(1, oCC).Value = "'PERSONA RECIBE";
            oCC++;
            worksheet.Cell(1, oCC).Value = "'RUT RECIBE";
            oCC++;
            worksheet.Cell(1, oCC).Value = "'REFERENCIA";
            oCC++;
            worksheet.Cell(1, oCC).Value = "'DESTINO / DEVOLUCIÓN";
            oCC++;
            worksheet.Cell(1, oCC).Value = "'ESTADO DOCTO.";
            oCC++;
            worksheet.Cell(1, oCC).Value = "'FECHA DOCTO.";
            oCC++;
            worksheet.Cell(1, oCC).Value = "'FECHA PRIMERA VISITA";
            oCC++;
            worksheet.Cell(1, oCC).Value = "'OBSERVACIÓN PRIMERA VISITA";
            oCC++;
            worksheet.Cell(1, oCC).Value = "'CANTIDAD VISITA";
            oCC++;
            worksheet.Cell(1, oCC).Value = "'VÍA";
            oCC++;
            worksheet.Cell(1, oCC).Value = "'OT PADRE";
            oCC++;
            worksheet.Cell(1, oCC).Value = "'CONTRAPAGO";
            oCC++;
            worksheet.Cell(1, oCC).Value = "'OBSERVACIÓN";
            oCC++;
            worksheet.Cell(1, oCC).Value = "'RECEPCIÓN BODEGA";
            

            int oFila = 2;
            foreach (var oElemento in oDatos)
            {

                string Fecha = "";
                DateTime TomarFecha = new DateTime();

                if (DateTime.TryParse(oElemento.RECEPCION.ToString(),out TomarFecha))
                {
                    Fecha=TomarFecha.ToString("dd/MM/yyyy HH:mm:ss");
                }
                

                int oC = 1;
                worksheet.Cell(oFila, oC).Value = "'" + oElemento.OT;
                oC++;
                worksheet.Cell(oFila, oC).Value = oElemento.FL_FOL_RCL_TIPO;
                oC++;
                worksheet.Cell(oFila, oC).Value = oElemento.FL_FOL_RCL_NUMERO;
                oC++;
                worksheet.Cell(oFila, oC).Value = oElemento.FL_FOL_CANT_DOCTO;
                oC++;
                worksheet.Cell(oFila, oC).Value = oElemento.FL_FOL_OTP_FECHA_CREACION;
                oC++;
                worksheet.Cell(oFila, oC).Value = oElemento.FL_FOL_OTD_FECHA_ESTIMADA;
                oC++;
                worksheet.Cell(oFila, oC).Value = oElemento.FECHA_ENTREGA_REAL;
                oC++;
                worksheet.Cell(oFila, oC).Value = oElemento.FL_SUCURSAL_ESTADO;
                oC++;
                worksheet.Cell(oFila, oC).Value = oElemento.FL_FOL_EST_NOMBRE;
                oC++;
                worksheet.Cell(oFila, oC).Value = oElemento.FECHA_ESTADO;
                oC++;
                worksheet.Cell(oFila, oC).Value = oElemento.FL_FOL_OTD_BULTO;
                oC++;
                worksheet.Cell(oFila, oC).Value = oElemento.FL_FOL_OTD_PESO;
                oC++;
                worksheet.Cell(oFila, oC).Value = oElemento.PESO_VOLUMETRICO;
                oC++;
                worksheet.Cell(oFila, oC).Value = oElemento.FL_PAR_SUC_NOMBRE_ORG;
                oC++;
                worksheet.Cell(oFila, oC).Value = oElemento.FL_PAR_SUC_NOMBRE;
                oC++;
                worksheet.Cell(oFila, oC).Value = oElemento.FL_CLI_EMP_FANTASIA;
                oC++;
                worksheet.Cell(oFila, oC).Value = oElemento.FL_PAR_SER_NOMBRE;
                oC++;
                worksheet.Cell(oFila, oC).Value = oElemento.FL_FOL_OTD_DESTINATARIO_NOMBRE;
                oC++;
                worksheet.Cell(oFila, oC).Value = oElemento.FL_FOL_OTD_DESTINATARIO_RUT;
                oC++;
                worksheet.Cell(oFila, oC).Value = oElemento.FL_FOL_OTD_DESTINATARIO_DIRECCION;//Revisar
                oC++;
                worksheet.Cell(oFila, oC).Value = oElemento.FL_FOL_OTD_DESTINATARIO_NUMERO;//Revisar
                oC++;
                worksheet.Cell(oFila, oC).Value = oElemento.FL_PAR_COM_NOMBRE;                
                oC++;
                worksheet.Cell(oFila, oC).Value = oElemento.FL_FOL_OTD_TELEFONO;
                oC++;                
                worksheet.Cell(oFila, oC).Value = oElemento.FL_FOL_ENT_RECIBE;
                oC++;
                worksheet.Cell(oFila, oC).Value = oElemento.FL_FOL_ENT_RUT;
                oC++;
                worksheet.Cell(oFila, oC).Value = oElemento.FL_FOL_OTD_REFERENCIA2;
                oC++;
                worksheet.Cell(oFila, oC).Value = oElemento.TIPO_DES_DEV;
                oC++;
                worksheet.Cell(oFila, oC).Value = oElemento.FL_FOL_EDO_NOMBRE;
                oC++;
                worksheet.Cell(oFila, oC).Value = oElemento.FL_FOL_RDO_FECHA;
                oC++;
                worksheet.Cell(oFila, oC).Value = oElemento.FL_FOL_PRIMERA_VISITA;
                oC++;
                worksheet.Cell(oFila, oC).Value = oElemento.OBS_PRIMERA_VISITA;
                oC++;
                worksheet.Cell(oFila, oC).Value = oElemento.FL_FOL_CANTIDAD_VISITA;
                oC++;
                worksheet.Cell(oFila, oC).Value = oElemento.FL_PAR_VIA_NOMBRE;
                oC++;
                worksheet.Cell(oFila, oC).Value = oElemento.OTPadre;
                oC++;
                worksheet.Cell(oFila, oC).Value = oElemento.FL_FOL_DOA_CONTRAPAGO;
                oC++;
                worksheet.Cell(oFila, oC).Value = oElemento.FL_FOL_TOB_NOMBRE;
                oC++;
                worksheet.Cell(oFila, oC).Value = Fecha;

                oFila++;

            }

            for (int c = 1; c <= 42; c++)
            {
                worksheet.Column(c).AdjustToContents();
            }

            workbook.SaveAs(Stream);

            // set HTTP response headers
            Response.ClearHeaders();
            Response.ClearContent();
            Response.Clear();

            Response.AddHeader("Content-Type", "application/vnd.ms-excel.12");
            //Response.AddHeader("Cache-Control", "no-cache");
            //Response.AddHeader("Accept-Ranges", "none");
            Response.AddHeader("Content-Disposition", "attachment; filename=\"PlanillaTransporte_" + fecha1 + "AL" + fecha2 + ".xlsx\"");

            // Enviar el archivo generado XLSX
            Stream.WriteTo(Response.OutputStream);
            Stream.Close();
            Response.Flush();
            Response.End();

            return View();
        }
        #endregion

        #region GeneraExcelPreManifiesto
        [Authorize]
        public ActionResult GeneraExcelPreManifiesto(int SUC_ID)
        {

            ValidationController oValidation = new ValidationController();
            string SucursalDestino = oValidation.GetSucursalNamefromIDSucursal(SUC_ID);

            MemoryStream Stream = new MemoryStream();
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("PreManifiesto " + SucursalDestino);


            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

           

            var oDatos = (from exc in oFol.PR_FOL_MAN_DESTINO_RUTA_TRONCAL_BLT(oValidation.GetSucursalIDfromActiveUser(),SUC_ID)
                          select exc).ToList();

            //TITULOS            
            worksheet.Cell(1, 1).Value = "'Destino";
            worksheet.Cell(1, 2).Value = "'Código Bulto";
            worksheet.Cell(1, 3).Value = "'OT";            
            worksheet.Cell(1, 4).Value = "'Vía";            

            int oFila = 2;            
            string Via = "";
            foreach (var oElemento in oDatos)
            {
                worksheet.Cell(oFila, 1).Value = SucursalDestino;
                worksheet.Cell(oFila, 2).Value = "B" + oElemento.PK_FOL_BLT_ID;
                worksheet.Cell(oFila, 3).Value = "'" + oElemento.PK_FOL_OTP_ID.ToString() +'-' + oElemento.PK_FOL_OTD_ID.ToString();
                if (oElemento.PK_PAR_VIA_ID == 1)
                    Via = "Terrestre";
                else
                {
                    Via = "Aérea";
                }
                worksheet.Cell(oFila, 4).Value = Via;

                oFila += 1;
            }

            for (int c = 1; c <= 4; c++)
            {
                worksheet.Column(c).AdjustToContents();
            }

            workbook.SaveAs(Stream);

            // set HTTP response headers
            Response.ClearHeaders();
            Response.ClearContent();
            Response.Clear();

            Response.AddHeader("Content-Type", "application/vnd.ms-excel.12");
            //Response.AddHeader("Cache-Control", "no-cache");
            //Response.AddHeader("Accept-Ranges", "none");
            Response.AddHeader("Content-Disposition", "attachment; filename=\"PreManifiesto_" + SucursalDestino + "_" + DateTime.Now.ToShortDateString().Replace("/","").Replace("-","") + ".xlsx\"");

            // Enviar el archivo generado XLSX
            Stream.WriteTo(Response.OutputStream);
            Stream.Close();
            Response.Flush();
            Response.End();

            return View();
        }
        #endregion

        #region InformeCelsouth
        public ActionResult InformeCelsouth()
        {
            return View();
        }
        #endregion


        #region GeneraExcelCelsouth
        public ActionResult GeneraExcelCelsouth(string FECINI, string FECTER)
        {

            //DateTime FechaInicial = Convert.ToDateTime(FECINI);
            //DateTime FechaFinal = Convert.ToDateTime(FECTER);

            string fecha1 = FECINI.Substring(6, 4) + FECINI.Substring(3, 2) + FECINI.Substring(0, 2);
            string fecha2 = FECTER.Substring(6, 4) + FECTER.Substring(3, 2) + FECTER.Substring(0, 2);

            string Fecha_Ini = FECINI.Substring(6, 4) + FECINI.Substring(3, 2) + FECINI.Substring(0, 2) + " 00:00:00.000";
            string Fecha_Fin = FECTER.Substring(6, 4) + FECTER.Substring(3, 2) + FECTER.Substring(0, 2) + " 23:59:59.999";

            MemoryStream Stream = new MemoryStream();
            var workbook = new XLWorkbook();
            //var worksheet = workbook.Worksheets.Add("Planilla Transporte");


            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            oFol.CommandTimeout = 600;

            var oDatos = (from exc in oFol.PR_FOL_INFORME_CELSOUTH(fecha1, fecha2).AsEnumerable() select exc).ToList();


            var oResult = oDatos.ToDataSet();

            workbook.Worksheets.Add(oResult);

            workbook.Worksheet(1).Name = "Planilla Transporte";
            

            workbook.SaveAs(Stream);
            
            Response.ClearHeaders();
            Response.ClearContent();
            Response.Clear();

            Response.AddHeader("Content-Type", "application/vnd.ms-excel.12");
           
            Response.AddHeader("Content-Disposition", "attachment; filename=\"InformeCelsouth_" + fecha1 + "AL" + fecha2 + ".xlsx\"");
            
            Stream.WriteTo(Response.OutputStream);
            Stream.Close();
            Response.Flush();
            Response.End();

            return View();
        }
        #endregion

    }
}
