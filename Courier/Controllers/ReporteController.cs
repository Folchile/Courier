using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data.Metadata.Edm;
using Courier.Models;

namespace Courier.Controllers
{
    public class ReporteController : Controller
    {
        //
        // GET: /Reporte/

        public ActionResult Index(decimal MAN_ID)
        {
            //Exception exeption = new Exception();
            RS2005.ReportingService2005 rs;
            RE2005.ReportExecutionService rsExec;

            // Create a new proxy to the web service
            rs = new RS2005.ReportingService2005();
            rsExec = new RE2005.ReportExecutionService();


            System.Net.NetworkCredential nwc= new System.Net.NetworkCredential();

            nwc.UserName = "AdminApp";
            nwc.Password = "AdminApp";
            nwc.Domain = "FRANCISCOROJAS";            


            // Authenticate to the Web service using Windows credentials
            //rs.Credentials = System.Net.CredentialCache.DefaultCredentials;
            //rsExec.Credentials = System.Net.CredentialCache.DefaultCredentials;

            rs.Credentials = nwc;
            rsExec.Credentials = nwc;


            


            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            


            var oTipoManifiesto = (from man in oFol.TB_FOL_MAN_MANIFIESTO
                                   where man.PK_FOL_MAN_ID == MAN_ID
                                   select man).Single();

            int ValorReporte = 0;//2 es RutaCliente, 5 es RutaSucursal
            if (oTipoManifiesto.PK_FOL_TPM_ID == 1) //1:Ruta Sucursal / CD 2:Ruta Cliente
            {
                ValorReporte = 5;
            }
            else
            {
                ValorReporte = 2;
            }


            var oParametro = from rep in oFol.TB_FOL_REP_REPORTE
                             select rep;

            string oServidor = oParametro.Where(m => m.PK_FOL_REP_ID == 1).Single().FL_FOL_REP_DIRECCION;
            string oReporte = oParametro.Where(m => m.PK_FOL_REP_ID == ValorReporte).Single().FL_FOL_REP_DIRECCION;

            rs.Url = oServidor +  "reportservice2005.asmx";
            rsExec.Url = oServidor + "reportexecution2005.asmx";

            string historyID = null;
            string deviceInfo = null;
            string format = "PDF";
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
                    parameters[0].Label = "MAN_ID";
                    parameters[0].Name = "MAN_ID";
                    parameters[0].Value = MAN_ID.ToString();
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
            Response.Clear();
            Response.AddHeader("Content-Type", "application/pdf");
            //Response.AddHeader("Content-Type", "application/xls");


            //Response.AddHeader("Cache-Control", "no-cache");
            //Response.AddHeader("Accept-Ranges", "none");
            
            Response.AddHeader("Content-Disposition", "attachment; filename=Manifiesto_" + MAN_ID.ToString()+ ".pdf");
            //Response.AddHeader("Content-Disposition", "attachment; filename=Manifiesto_" + MAN_ID.ToString() + ".xls");


            // Enviar el archivo generado XLSX
            Stream.WriteTo(Response.OutputStream);
            Stream.Close();
            Response.Flush();
            Response.End();


            return View();
        }

        public ActionResult ManifiestoDocumento(decimal MDO_ID)
        {
            //Exception exeption = new Exception();
            RS2005.ReportingService2005 rs;
            RE2005.ReportExecutionService rsExec;

            // Create a new proxy to the web service
            rs = new RS2005.ReportingService2005();
            rsExec = new RE2005.ReportExecutionService();


            System.Net.NetworkCredential nwc = new System.Net.NetworkCredential();

            nwc.UserName = "AdminApp";
            nwc.Password = "AdminApp";
            nwc.Domain = "FRANCISCOROJAS";


            // Authenticate to the Web service using Windows credentials
            //rs.Credentials = System.Net.CredentialCache.DefaultCredentials;
            //rsExec.Credentials = System.Net.CredentialCache.DefaultCredentials;

            rs.Credentials = nwc;
            rsExec.Credentials = nwc;

            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            var oParametro = from rep in oFol.TB_FOL_REP_REPORTE
                             select rep;

            string oServidor = oParametro.Where(m => m.PK_FOL_REP_ID == 1).Single().FL_FOL_REP_DIRECCION;
            string oReporte = oParametro.Where(m => m.PK_FOL_REP_ID == 3).Single().FL_FOL_REP_DIRECCION;

            rs.Url = oServidor + "reportservice2005.asmx";
            rsExec.Url = oServidor + "reportexecution2005.asmx";

            string historyID = null;
            string deviceInfo = null;
            string format = "PDF";
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
                parameters[0].Label = "MDO_ID";
                parameters[0].Name = "MDO_ID";
                parameters[0].Value = MDO_ID.ToString();
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
            Response.Clear();
            Response.AddHeader("Content-Type", "application/pdf");
            //Response.AddHeader("Content-Type", "application/xls");


            //Response.AddHeader("Cache-Control", "no-cache");
            //Response.AddHeader("Accept-Ranges", "none");

            Response.AddHeader("Content-Disposition", "attachment; filename=ManifiestoDocumento_" + MDO_ID.ToString() + ".pdf");
            //Response.AddHeader("Content-Disposition", "attachment; filename=Manifiesto_" + MAN_ID.ToString() + ".xls");


            // Enviar el archivo generado XLSX
            Stream.WriteTo(Response.OutputStream);
            Stream.Close();
            Response.Flush();
            Response.End();


            return View();
        }

        public ActionResult Map(decimal MAP_ID)
        {
            //Exception exeption = new Exception();
            RS2005.ReportingService2005 rs;
            RE2005.ReportExecutionService rsExec;

            // Create a new proxy to the web service
            rs = new RS2005.ReportingService2005();
            rsExec = new RE2005.ReportExecutionService();


            System.Net.NetworkCredential nwc = new System.Net.NetworkCredential();

            nwc.UserName = "AdminApp";
            nwc.Password = "AdminApp";
            nwc.Domain = "FRANCISCOROJAS";


            // Authenticate to the Web service using Windows credentials
            //rs.Credentials = System.Net.CredentialCache.DefaultCredentials;
            //rsExec.Credentials = System.Net.CredentialCache.DefaultCredentials;

            rs.Credentials = nwc;
            rsExec.Credentials = nwc;




            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            var oParametro = from rep in oFol.TB_FOL_REP_REPORTE
                             select rep;

            string oServidor = oParametro.Where(m => m.PK_FOL_REP_ID == 1).Single().FL_FOL_REP_DIRECCION;
            string oReporte = oParametro.Where(m => m.PK_FOL_REP_ID == 13).Single().FL_FOL_REP_DIRECCION;

            rs.Url = oServidor + "reportservice2005.asmx";
            rsExec.Url = oServidor + "reportexecution2005.asmx";

            string historyID = null;
            string deviceInfo = null;
            string format = "PDF";
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
                parameters[0].Label = "MAP_ID";
                parameters[0].Name = "MAP_ID";
                parameters[0].Value = MAP_ID.ToString();
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
            Response.Clear();
            Response.AddHeader("Content-Type", "application/pdf");
            //Response.AddHeader("Content-Type", "application/xls");


            //Response.AddHeader("Cache-Control", "no-cache");
            //Response.AddHeader("Accept-Ranges", "none");

            Response.AddHeader("Content-Disposition", "attachment; filename=ManifiestoPadre_" + MAP_ID.ToString() + ".pdf");
            //Response.AddHeader("Content-Disposition", "attachment; filename=Manifiesto_" + MAN_ID.ToString() + ".xls");


            // Enviar el archivo generado XLSX
            Stream.WriteTo(Response.OutputStream);
            Stream.Close();
            Response.Flush();
            Response.End();


            return View();
        }

    }
}
