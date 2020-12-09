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
    public class PalletController : Controller
    {
        //
        // GET: /Pallet/

        public ActionResult Pallet()
        {
            return View();
        }

        #region GetRutActiveUser
        [Authorize]
        public int GetRutActiveUser()
        {

            string oUsuario = System.Web.HttpContext.Current.User.Identity.Name;
            if (oUsuario.Length >= 3)
            {
                int oLargo = oUsuario.Length;
                int oRut = Convert.ToInt32(oUsuario.Substring(0, oLargo - 2));
                return oRut;
            }
            else
            {
                return 0;
            }
        }
        #endregion


        [HttpPost]
        public ActionResult InsertaPallet(PalletModels pModel)
        {
            Clases.Retorno oRetorno = new Clases.Retorno();
            var oResult = TransformCodigoGenericoOTPOTD(pModel.OT);
            oRetorno = pModel.InsertPallet(oResult.OTP, oResult.OTD, GetRutActiveUser());
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult FinalizarProceso(PalletModels pModel)
        {
            Clases.Retorno oRetorno = new Clases.Retorno();
            var oResult = TransformCodigoGenericoOTPOTD(pModel.OT);
             oRetorno = pModel.FinishProcess(oResult.OTP, oResult.OTD);
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AgregPallet(PalletModels pModel)
        {
            var oResult = TransformCodigoGenericoOTPOTD(pModel.OT);
            LinqBD_FOLDataContext oPall = new LinqBD_FOLDataContext();
            var oDatos = (from d in oPall.PR_FOL_RESCATA_ULT_PALLET(oResult.OTP, oResult.OTD)
                          select d).Single();
            pModel.Pallet = Convert.ToInt32(oDatos.BLTOS) + 1;
            pModel.Documento = pModel.OT;           
            return PartialView("AgregaPallet", pModel);
        }


        [HttpPost]
        public ActionResult EditarPallet(PalletModels pModel)
        {
            //PalletModels pModel = new PalletModels();
            
            LinqBD_FOLDataContext oPall = new LinqBD_FOLDataContext();
            var pDatos = from p in oPall.TB_FOL_PAO_PALLET_ASOCIADO
                         where p.PK_FOL_PAO_ID == pModel.IDPALLET
                         select p;
            foreach (var oFile in pDatos)
            {
                pModel.Pallet = Convert.ToDouble(oFile.PK_FOL_PAO_NUMERO_PALLET);
                pModel.BultosDet = Convert.ToDouble(oFile.FL_FOL_PAO_BULTOS);
                //pModel.IDPALLET = Convert.ToInt32(gForm["id_edpall"]);
            }
            //pModel.OT = pModel.OT;
            return PartialView("_editarPallet", pModel);
        }

        [HttpPost]
        public ActionResult ActualizaPallet(PalletModels pModel)
        {
            var oResult = TransformCodigoGenericoOTPOTD(pModel.OT);
            Clases.Retorno oRetorno = new Clases.Retorno();
            oRetorno = pModel.UpdatePall(oResult.OTP, oResult.OTD, pModel.IDPALLET);
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult EliminarPallet(FormCollection vForm)
        {
            PalletModels pModel = new PalletModels();
            RetornoModels oRetorno = new RetornoModels();
            oRetorno = pModel.DelPall(Convert.ToInt32(vForm["id_pallet"]));
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }


        

        public ActionResult CargaCabeceraDocto(PalletModels pModel)
        {
            decimal OTP;
            decimal OTD;
            var oResult = TransformCodigoGenericoOTPOTD(pModel.Documento);
            if (oResult.OTP == 0)
            {
                BuscarOTModels oModel = new BuscarOTModels { RutCliente = "96786140-5", TipoReferencia="1",Referencia=pModel.Documento };

                oModel.GetDatosByReferencia();
                
                OTP = oModel.OTP;
                OTD = oModel.OTD;
            }
            else
            {
                OTP=oResult.OTP;
                OTD = oResult.OTD;
            }

            

            Clases.Retorno oRetorno = new Clases.Retorno();

            if (OTP == 0)
            {
                oRetorno.Mensaje = string.Format("Documento ingresado no Existe");
                oRetorno.Ok = false;
                return Json(oRetorno, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {

                    //LinqBD_FOLDataContext odOT = new LinqBD_FOLDataContext();
                    LinqBD_FOLDataContext oPall = new LinqBD_FOLDataContext();
                    //Clases.Retorno Resultado = new Clases.Retorno();

                    var oDatos = (from otd in oPall.TB_FOL_OTD_OT_DETALLE
                                  where otd.PK_FOL_OTP_ID == OTP
                                  && otd.PK_FOL_OTD_ID == OTD
                                  select otd).Single();
                    if (oDatos.PK_FOL_EST_ID == 20)
                       pModel.BODEGACLIENTE= 1;
                    else
                        pModel.BODEGACLIENTE = 2;
                    
                    //LinqBD_FOLDataContext oPall = new LinqBD_FOLDataContext();
                    var pDatos = from p in oPall.PR_FOL_RESCATA_DATOS_DOC(OTP, OTD)
                                 select p;
                    foreach (var pFile in pDatos)
                    {
                        pModel.RutEmpresa = pFile.PK_CLI_EMP_RUT;
                        pModel.RazonSocial = pFile.FL_CLI_EMP_RAZON_SOCIAL;
                        pModel.Comuna = pFile.FL_PAR_COM_NOMBRE;
                        pModel.NombreDestinatario = pFile.FL_FOL_OTD_DESTINATARIO_NOMBRE;
                        pModel.DireccionDestinatario = pFile.FL_FOL_OTD_DESTINATARIO_DIRECCION;
                        pModel.RutDestinatario = pFile.FL_FOL_OTD_DESTINATARIO_RUT;
                        pModel.Servicio = pFile.FL_PAR_SER_NOMBRE;
                        pModel.Bultos = Convert.ToDecimal(pFile.FL_FOL_OTD_BULTO);
                        pModel.Peso = Convert.ToDouble(pFile.PESO);
                        pModel.OT = Convert.ToString(pFile.PK_FOL_OTP_ID) + "-" + pFile.PK_FOL_OTD_ID;
                    }
                    return PartialView("CabeceraDocto", pModel);
                }
                catch (Exception e)
                {
                    oRetorno.Mensaje = string.Format("Se ha producido un error: {0}", e.Message);
                    oRetorno.Ok = false;
                    return Json(oRetorno, JsonRequestBehavior.AllowGet);
                }
            }
        }


        #region ImpresionPallet
        [Authorize]
        public ActionResult ImpresionPallet(PalletModels pModel)
        {
            var oResult = TransformCodigoGenericoOTPOTD(pModel.OT);

            pModel.OTP = oResult.OTP;
            pModel.OTD = oResult.OTD;
     
            ValidationController oValidation = new ValidationController();

            pModel.GetNombreImpresora(Convert.ToInt32(oValidation.GetRutActiveUser()));
            //pModel.Pallet = pModel.Pallet;
            //pModel.BultosDet = pModel.BultosDet;

            return PartialView("_ImpresionEtiquetaPallet", pModel);
          
        }
        #endregion



        public ActionResult CargPalletAsociados(PalletModels pModel)
        {
            var oResult = TransformCodigoGenericoOTPOTD(pModel.OT);
            pModel.GetListaDetallePallet(oResult.OTP, oResult.OTD);
            return PartialView("DetalleDocumento", pModel);
        }


        public ActionResult CargDoctosAsociados(PalletModels pModel)
        {
            var oResult = TransformCodigoGenericoOTPOTD(pModel.OT);
            pModel.GetListaDoctosAsoc(oResult.OTP, oResult.OTD);
            return PartialView("DocumentosAsociados", pModel);
        }


        public ActionResult ValidaBultos(PalletModels pModel)
        {
            var oResult = TransformCodigoGenericoOTPOTD(pModel.OT);
            LinqBD_FOLDataContext odOT = new LinqBD_FOLDataContext();
            Clases.Retorno Resultado = new Clases.Retorno();
            var oDatos = (from b in odOT.PR_FOL_SUM_CANT_BULTOS(oResult.OTP, oResult.OTD)
                         select b).Single();

            Resultado.CantBultos = Convert.ToDecimal(oDatos.BULTOS);
            return Json(Resultado, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ValidaEstadoOT(PalletModels pModel)
        {
            var oResult = TransformCodigoGenericoOTPOTD(pModel.OT);
            LinqBD_FOLDataContext odOT = new LinqBD_FOLDataContext();
            Clases.Retorno Resultado = new Clases.Retorno();

            var oDatos = (from otd in odOT.TB_FOL_OTD_OT_DETALLE
                          where otd.PK_FOL_OTP_ID == oResult.OTP
                          && otd.PK_FOL_OTD_ID == oResult.OTD
                          select otd).Single();
            if (oDatos.PK_FOL_EST_ID == 20)
                Resultado.Ok = true;
            else
                Resultado.Ok = false;
            return Json(Resultado, JsonRequestBehavior.AllowGet);
        }


       
        public static Boolean IsNumeric(string stringToTest)
        {
            int result;
            return int.TryParse(stringToTest, out result);
        }


        #region TransformCodigoGenericoOTPOTD
        public class GetOTPOTD
        {
            public decimal OTP { get; set; }
            public decimal OTD { get; set; }
            public int Error { get; set; }
            public string ErrorMensaje { get; set; }
        }


        public GetOTPOTD TransformCodigoGenericoOTPOTD(string strCodigo)
        {
            GetOTPOTD oResult = new GetOTPOTD();
            string fOTP = "";
            string fOTD = "";
            int swt = 0;
            int CuentaGuiones = 0;
            if (strCodigo != null)
            {
                if (IsNumeric(strCodigo.Substring(0,1)))
                {
                    foreach (var oCar in strCodigo)
                    {
                        if (IsNumeric(oCar.ToString()))
                        {
                            if (swt == 0)
                            {
                                fOTP += oCar.ToString();
                            }
                            else if (swt==1)
                            {
                                fOTD += oCar.ToString();
                            }                            
                        }
                        else if (oCar.ToString() == "-")
                        {
                            swt = 1;
                            CuentaGuiones += 1;
                        }                        
                    }
                    if (CuentaGuiones > 1 || CuentaGuiones == 0)
                    {
                        oResult = new GetOTPOTD { OTP = 0, OTD = 0, Error = 1, ErrorMensaje = "Número Incorrecto" };
                        return oResult;
                    }
                    else if (IsNumeric(fOTP) == false || IsNumeric(fOTD) == false)
                    {
                        oResult = new GetOTPOTD { OTP = 0, OTD = 0, Error = 1, ErrorMensaje = "Número Incorrecto" };
                        return oResult;
                    }                    
                }
                else if (strCodigo.Substring(0,1).ToUpper()=="P")
                {                    
                    foreach (var oCar in strCodigo)
                    {
                        if (IsNumeric(oCar.ToString()))
                        {
                            if (swt == 0)
                            {
                                fOTP += oCar.ToString();
                            }
                            else if (swt == 1)
                            {
                                fOTD += oCar.ToString();
                            }
                        }
                        else if (oCar.ToString().ToUpper() == "D")
                        {
                            swt = 1;
                            CuentaGuiones += 1;
                        }
                    }

                    if (CuentaGuiones > 1 || CuentaGuiones == 0)
                    {
                        oResult = new GetOTPOTD { OTP = 0, OTD = 0, Error = 1, ErrorMensaje = "Número Incorrecto" };
                        return oResult;
                    }
                    else if (IsNumeric(fOTP) == false || IsNumeric(fOTD) == false)
                    {
                        oResult = new GetOTPOTD { OTP = 0, OTD = 0, Error = 1, ErrorMensaje = "Número Incorrecto" };
                        return oResult;
                    }                    
                }
                else
                {
                    oResult = new GetOTPOTD { OTP = 0, OTD = 0, Error = 1, ErrorMensaje = "Número Incorrecto" };
                    return oResult;
                }
            }
            else
            {
                oResult = new GetOTPOTD { OTP = 0, OTD = 0, Error = 1, ErrorMensaje = "Número Incorrecto" };
                return oResult;
            }
            decimal dOTP = Convert.ToDecimal(fOTP);
            decimal dOTD = Convert.ToDecimal(fOTD);
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            var oDatos = from x in oFol.TB_FOL_OTD_OT_DETALLE
                         where 
                             x.PK_FOL_OTP_ID==dOTP
                             && x.PK_FOL_OTD_ID==dOTD
                         select x;

            if (oDatos.Count() > 0)
            {
                oResult = new GetOTPOTD { OTP = dOTP, OTD = dOTD, Error = 0, ErrorMensaje = "Ok" };
                return oResult;
            }
            else
            {
                oResult = new GetOTPOTD { OTP = 0, OTD = 0, Error = 1, ErrorMensaje = "OT no existe en el sistema" };
                return oResult;
            }

        }
        #endregion
       




    }
}
