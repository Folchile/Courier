using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Courier.Models;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using ClosedXML.Excel;

namespace Courier.Controllers
{
    public class DocumentoController : Controller
    {
        //
        // GET: /Documento/
        public class RetornoRespuesta
        {
            public int Ok { get; set; }
            //1: OT TIENE 1 DOCUMENTO ASOCIADO, PASA OKñ
            //2: OT TIENE MAS DE 1 DOCUMENTO ASOCIADO, SE DEBE MOSTRAR MENSAJE
            //3: OK
            //9: ERROR
            public string Mensaje { get; set; }
        }
        public ActionResult Index()
        {
            return View();
        }

        //RECEPCIÓN
        #region Recepcion
        [Authorize]
        public ActionResult Recepcion()
        {
            DocumentoModels oModel = new DocumentoModels();                        
            return View(oModel);            
        }
        #endregion
        #region AgregarDocumento
        public ActionResult AgregarDocumento(DocumentoModels oModel)
        {
            RetornoRespuesta oResult = new RetornoRespuesta();
            oResult.Ok = 1;
            oResult.Mensaje = "Erro";
            return Json(oResult);
        }
        #endregion
        #region FormRecepcionDocumento
        public ActionResult FormRecepcionDocumento(DocumentoModels oModel)
        {
            ValidationController oValidation = new ValidationController();

            var oResult=oValidation.TransformCodigoGenericoOTPOTD(oModel.NumeroDocumento);

            oModel.OTD = oResult.OTD;
            oModel.OTP = oResult.OTP;

            oModel.GetListaDocumentos();
            return PartialView("_FormRecepcionDocumento", oModel);
        }
        #endregion
        #region RecepcionGuardar
        public bool RecepcionGuardar(DocumentoModels oModel)
        {            
            try
            {                
                ValidationController oValidation = new ValidationController();
                var oOT = oValidation.TransformCodigoGenericoOTPOTD(oModel.NumeroDocumento);
                oModel.OTP = oOT.OTP;
                oModel.OTD = oOT.OTD;
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                oFol.PR_FOL_RDO_RECEPCION_INSERT(oModel.OTP, oModel.OTD,2, oValidation.GetRutActiveUser(),oValidation.GetSucursalIDfromActiveUser());
                return true;
            }
            catch
            {
                return false;
            }
            
        }
        #endregion
        #region ListaRecepcionHoy()
        public ActionResult ListaRecepcionHoy()
        {
            ValidationController oValidation = new ValidationController();
            DocumentoModels oModel = new DocumentoModels();
            oModel.GetListaHoy(oValidation.GetSucursalIDfromActiveUser(),2);
            return PartialView("_ListaRecepcionHoy", oModel);
        }
        #endregion        
        //SALIDA
        #region Salida
        public ActionResult Salida()
        {
            return View();
        }
        #endregion
        #region SalidaGuardar
        public bool SalidaGuardar(DocumentoModels oModel)
        {
            try
            {
                ValidationController oValidation = new ValidationController();
                var oOT = oValidation.TransformCodigoGenericoOTPOTD(oModel.NumeroDocumento);
                oModel.OTP = oOT.OTP;
                oModel.OTD = oOT.OTD;
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                oFol.PR_FOL_RDO_RECEPCION_INSERT(oModel.OTP, oModel.OTD, 3, oValidation.GetRutActiveUser(), oValidation.GetSucursalIDfromActiveUser());
                return true;
            }
            catch
            {
                return false;
            }

        }
        #endregion
        #region ListaSalidaHoy()
        public ActionResult ListaSalidaHoy()
        {
            ValidationController oValidation = new ValidationController();
            DocumentoModels oModel = new DocumentoModels();
            oModel.GetListaHoy(oValidation.GetSucursalIDfromActiveUser(), 3);
            return PartialView("_ListaRecepcionHoy", oModel);
        }
        #endregion
        //RENDICIÓN
        #region Rendicion
        public ActionResult Rendicion()
        {
            DocumentoModels oModel = new DocumentoModels();
            return View(oModel);
        }
        #endregion
        #region ListaRendidoHoy()
        public ActionResult ListaRendidoHoy()
        {
            ValidationController oValidation = new ValidationController();
            DocumentoModels oModel = new DocumentoModels();
            oModel.GetListaHoy(oValidation.GetSucursalIDfromActiveUser(), 4);
            return PartialView("_ListaRecepcionHoy", oModel);
        }
        #endregion
        #region RendicionGuardar
        public bool RendicionGuardar(DocumentoModels oModel)
        {
            try
            {
                ValidationController oValidation = new ValidationController();
                var oOT = oValidation.TransformCodigoGenericoOTPOTD(oModel.NumeroDocumento);
                oModel.OTP = oOT.OTP;
                oModel.OTD = oOT.OTD;
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                oFol.PR_FOL_RDO_RECEPCION_INSERT(oModel.OTP, oModel.OTD, 4,oValidation.GetRutActiveUser(),oValidation.GetSucursalIDfromActiveUser());
                return true;
            }
            catch
            {
                return false;
            }

        }
        #endregion
        //BUSCAR
        #region BuscarDocumento
        public ActionResult BuscarDocumento()
        {
            return View();
        }
        #endregion
        #region BuscarDocumentoForm
        public ActionResult BuscarDocumentoForm()
        {
            return Json(true);
        }
        #endregion        
        #region ListaBuscar
        public ActionResult ListaBuscar(DocumentoModels oModel)
        {
            ValidationController oValidation = new ValidationController();          

            var oResult = oValidation.TransformCodigoGenericoOTPOTD(oModel.NumeroDocumento);
            oModel.OTP=oResult.OTP;
            oModel.OTD=oResult.OTD;
            oModel.GetListaBuscar();
            return PartialView("_ListaBuscar", oModel);
        }
        #endregion
        #region ListaBuscarHistoria
        public ActionResult ListaBuscarHistoria(decimal PK_FOL_DOA_ID)
        {
             LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from pr in oFol.PR_FOL_RDO_BUSCAR_HISTORIA(PK_FOL_DOA_ID)
                         select new DocumentoModels.ConsultaHoy
                         {
                             DOA_NUMERO = pr.PK_FOL_DOA_NUMERO,
                             EDO_NOMBRE = pr.FL_FOL_EDO_NOMBRE,
                             RDO_FECHA = pr.FL_FOL_RDO_FECHA,
                             SUC_NOMBRE = pr.FL_PAR_SUC_NOMBRE,
                             USU_NOMBRE = pr.USUARIO,                             
                             TDO_NOMBRE = pr.FL_PAR_TDO_NOMBRE,
                             OTP = pr.PK_FOL_OTP_ID,
                             OTD = pr.PK_FOL_OTD_ID
                         };

            DocumentoModels oModel = new DocumentoModels();
            oModel.ListaHoy= oDatos.ToArray();
            return PartialView("_ListaBuscarHistoria",oModel);
        }
        #endregion

        //MANIFIESTO
        public class RetornoManifiesto
        {
            public bool Ok { get; set; }
            public bool Existe { get; set; }
            public decimal NumeroManifiesto { get; set; }
            public string Mensaje { get; set; }
        }
        #region Manifiesto
        [Authorize]
        public ActionResult Manifiesto()
        {
            return View();
        }
        #endregion
        #region ExisteManifiesto
        public ActionResult ExisteManifiesto()
        {
            RetornoManifiesto oResult = new RetornoManifiesto();
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            ValidationController oValidation = new ValidationController();


            var oDatos = from mdo in oFol.TB_FOL_MDO_MANIFIESTO_DOCUMENTO
                         where mdo.PK_FOL_MAE_ID==1 && mdo.PK_PAR_USU_RUT== oValidation.GetRutActiveUser()
                         select mdo;
            if (oDatos.Count() > 0)
            {
                foreach (var oFila in oDatos)
                {
                    oResult.NumeroManifiesto = oFila.PK_FOL_MDO_ID;
                    oResult.Existe = true;
                }
            }
            else
            {
                oResult.Existe = false;
            }

            return Json(oResult,JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region CrearManifiesto
        public ActionResult CrearManifiesto()
        {
            RetornoManifiesto oResult = new RetornoManifiesto();
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            ValidationController oValidation = new ValidationController();
            try
            {
                TB_FOL_MDO_MANIFIESTO_DOCUMENTO oMDO = new TB_FOL_MDO_MANIFIESTO_DOCUMENTO
                {
                    PK_FOL_MDO_FECHA = DateTime.Now,
                    PK_FOL_MAE_ID = 1,//Abierto
                    PK_PAR_USU_RUT = oValidation.GetRutActiveUser()
                };
                oFol.TB_FOL_MDO_MANIFIESTO_DOCUMENTO.InsertOnSubmit(oMDO);                
                oFol.SubmitChanges();
                oResult.NumeroManifiesto = oMDO.PK_FOL_MDO_ID;
                oResult.Ok = true;                
            }
            catch (Exception e)
            {
                oResult.Ok = false;
                oResult.Mensaje = string.Format("No fue posible crear el manifiesto <br>", e.Message);
            }

           
            return Json(oResult, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region CargarCuerpoManifiesto
        public ActionResult CuerpoManifiesto(ManifiestoDocumentoModels oModel)
        {
            oModel.GetDatosManifiesto();
            oModel.GetDatosListaDOA();
            return PartialView("CuerpoManifiesto",oModel);
        }
        #endregion
        #region GuardaRutCliente
        public ActionResult GuardarRutCliente(ManifiestoDocumentoModels oModel)
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            RetornoModels oRetorno = new RetornoModels();
            try
            {
                var oRmd = from rmd in oFol.TB_FOL_RMD_RELACION_MDO_DOA
                       where rmd.PK_FOL_MDO_ID==Convert.ToDecimal(oModel.CodBuscarManifiesto)
                       select rmd;

                if (oRmd.Count() > 0)
                {
                    oRetorno.Ok = false;
                    oRetorno.Mensaje = "<p>Debe eliminar todos los documentos del manifiesto, antes de cambiar el Cliente</p>";
                    return Json(oRetorno, JsonRequestBehavior.AllowGet);
                }
            
               
                var oDatos = (from mdo in oFol.TB_FOL_MDO_MANIFIESTO_DOCUMENTO
                              where mdo.PK_FOL_MDO_ID == Convert.ToDecimal(oModel.CodBuscarManifiesto)
                              select mdo).Single();
                oDatos.PK_CLI_EMP_RUT = oModel.RutCliente;
                oFol.SubmitChanges();
                oRetorno.Ok = true;
                return Json(oRetorno,JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje="<p>No fue posible guardar la información</p>" + e.Message;
                return Json(oRetorno, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region VerificarEstadoDocumento
        public ActionResult VerificarEstadoDocumento(ManifiestoDocumentoModels oModel)
        {
            ValidationController oValidation = new ValidationController();
            RetornoModels oRetorno = new RetornoModels();

            var oOT=oValidation.TransformCodigoGenericoOTPOTD(oModel.NumeroDocumento);
            if (oOT.Error == 0)
            {

                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

                var oDatosOT = (from otd in oFol.TB_FOL_OTD_OT_DETALLE
                                join otp in oFol.TB_FOL_OTP_OT_PRINCIPAL on otd.PK_FOL_OTP_ID equals otp.PK_FOL_OTP_ID
                                where otd.PK_FOL_OTP_ID == oOT.OTP && otd.PK_FOL_OTD_ID == oOT.OTD
                                select new { otd, otp }).Single();

                if (oDatosOT.otd.PK_FOL_EST_ID != 10)
                {
                    oRetorno.Ok = false;
                    oRetorno.Mensaje = "La OT NO se encuentra en estado -Recepcionado OK- ";
                }
                else if (oDatosOT.otp.PK_CLI_EMP_RUT != oModel.RutCliente)
                {
                    oRetorno.Ok = false;
                    oRetorno.Mensaje = "Documento pertenece a otro cliente. (Validación Cliente Manifiesto)";
                }
                else
                {

                    var oDatos = from pr in oFol.PR_FOL_DOA_ESTADO(oOT.OTP, oOT.OTD)
                                 select pr;

                    var oElementos = oDatos.ToList();
                    if (oElementos.Count() > 0)
                    {
                        if (oElementos[0].PK_FOL_EDO_ID == 5)
                        {
                            oRetorno.Ok = false;
                            oRetorno.Mensaje = "El documento ya se encuentra en un manifiesto";
                        }
                        else if (oElementos[0].PK_FOL_EDO_ID == 4)
                        {
                            oRetorno.Ok = false;
                            oRetorno.Mensaje = "El documento ya se encuentra rendido";
                        }
                        else
                        {
                            oRetorno.Ok = true;
                        }
                    }
                }
            }
            else
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = oOT.ErrorMensaje;
            }

            return Json(oRetorno,JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region AsociarDocumentoManifiesto
        public ActionResult AsociarDocumentoManifiesto(ManifiestoDocumentoModels oModel)
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            ValidationController oValidation = new ValidationController();
            RetornoModels oRetorno = new RetornoModels();

            try
            {
                var oOT = oValidation.TransformCodigoGenericoOTPOTD(oModel.NumeroDocumento);

                            

                


                var oDoa = from doa in oFol.TB_FOL_DOA_DOC_ASOCIADO
                           where doa.PK_FOL_OTP_ID == oOT.OTP && doa.PK_FOL_OTD_ID == oOT.OTD
                           select doa;

                List<TB_FOL_RDO_DOA_EDO> oListado = new List<TB_FOL_RDO_DOA_EDO>();
                List<TB_FOL_RMD_RELACION_MDO_DOA> oListadoRmd = new List<TB_FOL_RMD_RELACION_MDO_DOA>();

                foreach (var oElemento in oDoa)
                {
                    TB_FOL_RDO_DOA_EDO oRdo = new TB_FOL_RDO_DOA_EDO
                    {
                        FL_FOL_RDO_FECHA=DateTime.Now,                                                
                        PK_FOL_EDO_ID=5,
                        PK_PAR_SUC_ID=oValidation.GetSucursalIDfromActiveUser(),
                        PK_PAR_USU_RUT=oValidation.GetRutActiveUser(),
                        PK_CLI_EMP_RUT=oElemento.PK_CLI_EMP_RUT,
                        PK_FOL_DOA_NUMERO=oElemento.PK_FOL_DOA_NUMERO,
                        PK_PAR_TDO_ID = oElemento.PK_PAR_TDO_ID
                    };

                    TB_FOL_RMD_RELACION_MDO_DOA oRmd = new TB_FOL_RMD_RELACION_MDO_DOA
                    {
                        PK_FOL_MDO_ID = Convert.ToInt32(oModel.CodBuscarManifiesto),
                        PK_CLI_EMP_RUT = oElemento.PK_CLI_EMP_RUT,
                        PK_FOL_DOA_NUMERO = oElemento.PK_FOL_DOA_NUMERO,
                        PK_PAR_TDO_ID = oElemento.PK_PAR_TDO_ID                        
                    };

                    oListadoRmd.Add(oRmd);
                    oListado.Add(oRdo);                                        
                }
                oFol.TB_FOL_RDO_DOA_EDO.InsertAllOnSubmit(oListado);
                oFol.TB_FOL_RMD_RELACION_MDO_DOA.InsertAllOnSubmit(oListadoRmd);

                oFol.SubmitChanges();
                oRetorno.Ok=true;
            }
            catch (Exception e)
            {
                oRetorno.Mensaje = string.Format("<p>No fue posible guardar la información:</p>{0}", e.Message);
                oRetorno.Ok=false;
            }            
            return Json(oRetorno,JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region EliminarRelacionManifiestoDoc
        public ActionResult EliminarRelacionManifiestoDoc(ManifiestoDocumentoModels oModel)
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            RetornoModels oRetorno = new RetornoModels();
            try
            {
                oFol.PR_FOL_DOA_ELI_REL(oModel.RutCliente, oModel.DOA_NUMERO, Convert.ToDecimal(oModel.CodBuscarManifiesto), oModel.TDO_ID);
                oRetorno.Ok = true;
            }
            catch (Exception e)
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = "<p>No fue posible eliminar</p>" + e.Message;
                
            }

            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ValidaCerrarManifiesto
        public ActionResult ValidaCerrarManifiesto(ManifiestoDocumentoModels oModel)
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            RetornoModels oRetorno = new RetornoModels();

            var oRmd = from rmd in oFol.TB_FOL_RMD_RELACION_MDO_DOA
                       where rmd.PK_FOL_MDO_ID == Convert.ToDecimal(oModel.CodBuscarManifiesto)
                       select rmd;
            if (oRmd.Count() == 0)
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = "<p>Debe agregar documentos antes de cerrar un manifiesto.</p>";
            }
            else
            {
                oRetorno.Ok=true;
            }
            return Json(oRetorno,JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region CerrarManifiestoDoc
        public ActionResult CerrarManifiestoDoc(ManifiestoDocumentoModels oModel)
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            RetornoModels oRetorno = new RetornoModels();
            try
            {
                var oDatos = (from mdo in oFol.TB_FOL_MDO_MANIFIESTO_DOCUMENTO
                             where mdo.PK_FOL_MDO_ID == Convert.ToDecimal(oModel.CodBuscarManifiesto)
                             select mdo).Single();
                oDatos.PK_FOL_MAE_ID = 2;
                oFol.SubmitChanges();
                oRetorno.Ok = true;
            }
            catch (Exception e)
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = "<p>No fue posible cerrar el manifiesto</p>" + e.Message;
            }
            
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }
        #endregion






        #region Rendicion Masiva
        public ActionResult RendicionMasiva()
        {
            return View("RendicionMasiva/RendicionMasiva");
        }
        #endregion
        #region ProcesarDocumento
        public class ExcelResultado
        {
            public string OT { get; set; }
            public string Guia { get; set; }
            public string Fecha { get; set; }
            public string Resultado { get; set; }
        }
        [HttpPost]
        public ActionResult ProcesarDocumento(HttpPostedFileBase file1)
        {
            var oBinary = new byte[file1.ContentLength];
            string oFileName = file1.FileName;
            file1.InputStream.Read(oBinary, 0, file1.ContentLength);
            
            Stream oMs = new MemoryStream(oBinary);  
            string oType = file1.FileName;
            string oExtension = oType.Substring(oType.Length - 3, 3).ToLower();

            if (oType.Substring(oType.Length - 3, 3).ToLower() == "xls")
                oType = "xls";
            else if (oType.Substring(oType.Length - 4, 4).ToLower() == "xlsx")
                oType = "xlsx";
            else
            {
                oType = "X";
            }

            List<ExcelResultado> oTablaResultado = new List<ExcelResultado>();
            List<ExcelResultado> oTablaResultado2 = new List<ExcelResultado>();

            #region Excel 2003 XLS
            if (oType == "xls")
            {
                HSSFWorkbook oHWb;
                oHWb = new HSSFWorkbook(oMs);

                ISheet sheet = oHWb.GetSheet(oHWb.GetSheetName(0));
                for (int iRow = 1; iRow <= sheet.LastRowNum; iRow++)
                {
                    oTablaResultado.Add(new ExcelResultado {
                        OT = sheet.GetRow(iRow).GetCell(1).ToString(),
                        Guia = sheet.GetRow(iRow).GetCell(2).ToString(),
                        Fecha= sheet.GetRow(iRow).GetCell(3).ToString()
                    });                    
                }
            }
            #endregion
            #region Excel 2007 XLSX
            else if (oType == "xlsx")
            {

                var oWorkBook = new XLWorkbook(oMs);
                var oWorkSheet = oWorkBook.Worksheet(1);                


                var oCursorRow = oWorkSheet.FirstRow();
                oCursorRow = oCursorRow.RowBelow();

                while (!oCursorRow.Cell(1).IsEmpty())
                {
                    oTablaResultado.Add(new ExcelResultado
                    {
                        OT = oCursorRow.Cell(1).Value.ToString(),
                        Guia = oCursorRow.Cell(2).Value.ToString(),
                        Fecha = oCursorRow.Cell(3).Value.ToString()
                    }); 
                    oCursorRow = oCursorRow.RowBelow();
                }
            }
            #endregion

            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            Controllers.ValidationController oValidation = new ValidationController();
            int UsuRut = oValidation.GetRutActiveUser(); 

            foreach (var oElement in oTablaResultado)
            {
                try
                {
                    var OT = oElement.OT.Split('-');
                    var oFecha = oElement.Fecha.Substring(6, 4) +'-'+ oElement.Fecha.Substring(3, 2) +'-'+  oElement.Fecha.Substring(0, 2) + " 12:00:00";


                    oFol.PR_FOL_DOC_RENDICION(Convert.ToDecimal(OT[0]), Convert.ToDecimal(OT[1]), Convert.ToDecimal(oElement.Guia), UsuRut, Convert.ToDateTime(oFecha));
                    oTablaResultado2.Add(new ExcelResultado { 
                        OT=oElement.OT,
                        Fecha=oElement.Fecha,
                        Guia=oElement.Guia,
                        Resultado="Ok"
                    });
                }
                catch (Exception ex)
                {
                    oTablaResultado2.Add(new ExcelResultado
                    {
                        OT = oElement.OT,
                        Fecha = oElement.Fecha,
                        Guia = oElement.Guia,
                        Resultado=ex.Message
                    });
                }
            }



            MemoryStream Stream = new MemoryStream();
            var workbook = new XLWorkbook();
            var oResult = oTablaResultado2.ToDataSet();
            workbook.Worksheets.Add(oResult);
            workbook.Worksheet(1).Name = "Resultado Rendición";
            workbook.SaveAs(Stream);

            // set HTTP response headers
            Response.ClearHeaders();
            Response.ClearContent();
            Response.Clear();

            Response.AddHeader("Content-Type", "application/vnd.ms-excel.12");            
            Response.AddHeader("Content-Disposition", "attachment; filename=\"Rendición.xlsx\"");

            // Enviar el archivo generado XLSX
            Stream.WriteTo(Response.OutputStream);
            Stream.Close();
            Response.Flush();
            Response.End();

            return View("RendicionMasiva/ProcesarDocumento");            
        }
        #endregion
    }
}

