using System;
using System.Globalization;
using System.Web.Mvc;
using System.Web.UI;
using Courier.Models;
using System.Linq;
using System.Xml.Linq;
using System.Web;
using System.Web.Security;
using System.Collections.Generic;


namespace Courier.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
    public class ValidationController : Controller
    {

        #region Validar Columnas Servicio
        public JsonResult ValidarColumnas(string Servicio)
        {            

            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            var oDatos = from m in oPar.TB_PAR_EXC_EXCEL
                          where m.PK_PAR_SER_ID == Convert.ToInt32(Servicio)
                          select m;

            if (oDatos.Count() > 0)
            {

                if (oDatos.Where(m => m.PK_PAR_COL_ID == 1).Count() > 0)
                {
                   var oRef = (from pr in oPar.PR_PAR_SER_VALIDA_REFERENCIA (1,1,Convert.ToInt32(Servicio))
                              select pr.CANTIDAD).Single();
                   if (oRef == 0)
                   {
                       string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Servicio: No se han definido un tipo de documento en la referencia 1");
                       return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                   }
                }
                if (oDatos.Where(m => m.PK_PAR_COL_ID == 2).Count() > 0)
                {
                    var oRef = (from pr in oPar.PR_PAR_SER_VALIDA_REFERENCIA(2, 2, Convert.ToInt32(Servicio))
                                select pr.CANTIDAD).Single();
                    if (oRef == 0)
                    {
                        string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Servicio: No se han definido un tipo de documento en la referencia 2");
                        return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                    }
                }

                return Json(true, JsonRequestBehavior.AllowGet);//Correcto                
            }
            else
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, "No se ha definido un Archivo de Carga para este Servicio");
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region IsUID_Available
        public JsonResult IsUID_Available(string Username)
        {
            //True
            //return Json(true, JsonRequestBehavior.AllowGet);

            //False
            string suggestedUID = String.Format(CultureInfo.InvariantCulture, "{0} No disponible.", Username);
            return Json(suggestedUID, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region jSonValidarReferencia1
        public JsonResult jSonValidarReferencia1(string Id, string RutCliente, int Servicio)
        {
            var oResult = ExisteReferenciaV2(RutCliente, Id, 1, Servicio);
            if (oResult.Ok==true)
            {
                 return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, oResult.Mensaje);
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
            
        }
        #endregion

        #region jSonValidarReferencia2
        public JsonResult jSonValidarReferencia2(string Referencia, string RutCliente, int Servicio)
        {
            var oResult = ExisteReferenciaV2(RutCliente, Referencia, 2, Servicio);
            if (oResult.Ok == true)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, oResult.Mensaje);
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region jSonValidarVReferencia1
        public JsonResult jSonValidarVReferencia1(string vId, string RutCliente, int Servicio)
        {
            var oResult = ExisteReferenciaV2(RutCliente, vId, 1, Servicio);
            if (oResult.Ok == true)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, oResult.Mensaje);
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region jSonValidarVReferencia2
        public JsonResult jSonValidarVReferencia2(string vReferencia, string RutCliente, int Servicio)
        {

            var oResult = ExisteReferenciaV2(RutCliente, vReferencia, 2, Servicio);
            if (oResult.Ok == true)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, oResult.Mensaje);
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region jSonValidarOTPapel
        public JsonResult jSonValidarOTPapel(string OT_Papel)
        {

            //var oDatos = TransformCodigoGenericoOTPOTD(OT_Papel);
            if (isDecimal(OT_Papel))
            {
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();


                var oDatos = from otp in oFol.TB_FOL_OTP_OT_PRINCIPAL
                             where otp.PK_FOL_OTP_ID == Convert.ToDecimal(OT_Papel)
                             select otp;


                if (oDatos.Count() > 0)
                {

                    var oConsulta = oDatos.ToList()[0];

                    if (oConsulta.PK_FOL_TOT_ID == 3)
                    {
                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        string suggestedUID = String.Format(CultureInfo.InvariantCulture, "OT No corresponde a OT en papel");
                        return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Número de OT No Existe");
                    return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Número de OT Incorrecto");
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region jSonValidarCoberturaComuna
        public JsonResult jSonValidarCoberturaComuna(string Comuna)
        {
            if (ValidarCoberturaComuna(Convert.ToInt32(Comuna)) == 0)
                return Json(true, JsonRequestBehavior.AllowGet);
            else
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Comuna Sin Cobertura");
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region jSonValidarCoberturaComunaRetiro
        public JsonResult jSonValidarCoberturaComunaRetiro(string RetiroComuna)
        {
            if (ValidarCoberturaComuna(Convert.ToInt32(RetiroComuna)) == 0)
                return Json(true, JsonRequestBehavior.AllowGet);
            else
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Comuna Retiro Sin Cobertura");
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region jSonValidarCoberturaLocalidad
        public JsonResult jSonValidarCoberturaLocalidad(string Localidad)
        {
            if (ValidarCoberturaLocalidad(Convert.ToInt32(Localidad)) == 0)
                return Json(true, JsonRequestBehavior.AllowGet);
            else
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Localidad Sin Cobertura");
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region jSonValidarCoberturaLocalidadRetiro
        public JsonResult jSonValidarCoberturaLocalidadRetiro(string RetiroLocalidad)
        {
            if (ValidarCoberturaLocalidad(Convert.ToInt32(RetiroLocalidad)) == 0)
                return Json(true, JsonRequestBehavior.AllowGet);
            else
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Localidad Retiro Sin Cobertura");
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region vjSonValidarCoberturaComuna
        public JsonResult vjSonValidarCoberturaComuna(string vComuna)
        {
            if (ValidarCoberturaComuna(Convert.ToInt32(vComuna)) == 0)
                return Json(true, JsonRequestBehavior.AllowGet);
            else
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Comuna Sin Cobertura");
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region vjSonValidarCoberturaLocalidad
        public JsonResult vjSonValidarCoberturaLocalidad(string vLocalidad)
        {
            if (ValidarCoberturaLocalidad(Convert.ToInt32(vLocalidad)) == 0)
                return Json(true, JsonRequestBehavior.AllowGet);
            else
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Localidad Sin Cobertura");
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Validar Cliente
        [HttpGet]
        public JsonResult ValidarCliente()
        {
            string Rut = Request.QueryString[0].ToString();

            int oVR = ValidarRut(Rut);
            int oEC = ExisteCliente(Rut);

            if (oVR == 0)
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, "{0} Rut Incorrecto.", Rut);
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
            else if (oEC == 0)
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, "{0} Cliente No existe", Rut);
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region Validar Usuario
        [HttpGet]
        public JsonResult ValidarUsuario()
        {
            string Rut = Request.QueryString[0].ToString();

            int oVR = ValidarRut(Rut);



            if (oVR == 1)
            {
                LinqBD_SEGDataContext oSeg = new LinqBD_SEGDataContext();
                var oExiste = from user in oSeg.aspnet_Users
                              where user.UserName == Rut
                              select user;
                if (oExiste.Count() > 0)
                {
                    string suggestedUID = String.Format(CultureInfo.InvariantCulture, "{0} Usuario ya existe.", Rut);
                    return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(true, JsonRequestBehavior.AllowGet);

            }
            else
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, "{0} Rut Incorrecto.", Rut);
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }

        }

        #endregion

        #region Validar Archivo
        [HttpGet]
        public JsonResult ValidarArchivo(string Archivo)
        {
            if (Archivo.Substring(Archivo.Length - 3, 3).ToUpper() != "XLS" && Archivo.Substring(Archivo.Length - 4, 4).ToUpper() != "XLSX")
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Debe seleccionar un archivo con extensión XLS o XLSX");
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);//Correcto
            }
        }

        #endregion

        #region ValidarRutContacto1
        [HttpGet]
        public JsonResult ValidarRutContacto1(string Contacto1Rut)
        {
            int oVR = ValidarRut(Contacto1Rut);
            if (oVR != 1)
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, "{0}: {1}", ErrorValidarRut(oVR), Contacto1Rut);
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region ValidarRutContacto2
        [HttpGet]
        public JsonResult ValidarRutContacto2(string Contacto2Rut)
        {
            int oVR = ValidarRut(Contacto2Rut);
            if (oVR != 1)
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, "{0}: {1}", ErrorValidarRut(oVR), Contacto2Rut);
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region ValidarRutContacto3
        [HttpGet]
        public JsonResult ValidarRutContacto3(string Contacto3Rut)
        {
            int oVR = ValidarRut(Contacto3Rut);
            if (oVR != 1)
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, "{0}: {1}", ErrorValidarRut(oVR), Contacto3Rut);
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region ValidarRutvContacto1
        [HttpGet]
        public JsonResult ValidarRutvContacto1(string vContacto1Rut)
        {
            int oVR = ValidarRut(vContacto1Rut);
            if (oVR != 1)
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, "{0}: {1}", ErrorValidarRut(oVR), vContacto1Rut);
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region ValidarRutvContacto2
        [HttpGet]
        public JsonResult ValidarRutvContacto2(string vContacto2Rut)
        {
            int oVR = ValidarRut(vContacto2Rut);
            if (oVR != 1)
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, "{0}: {1}", ErrorValidarRut(oVR), vContacto2Rut);
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region ValidarRutvContacto3
        [HttpGet]
        public JsonResult ValidarRutvContacto3(string vContacto3Rut)
        {
            int oVR = ValidarRut(vContacto3Rut);
            if (oVR != 1)
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, "{0}: {1}", ErrorValidarRut(oVR), vContacto3Rut);
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion


        #region ValidarRutSolo
        [HttpGet]
        public JsonResult ValidarRutSolo(string Rut)
        {            
            int oVR = ValidarRut(Rut);
            if (oVR != 1)
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, "{0}: {1}", ErrorValidarRut(oVR), Rut);
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region vValidarRutSolo
        [HttpGet]
        public JsonResult vValidarRutSolo(string vRut)
        {
            if (vRut!=null)
            {
                int oVR = ValidarRut(vRut);
                if (oVR != 1)
                {
                    string suggestedUID = String.Format(CultureInfo.InvariantCulture, "{0}: {1}", ErrorValidarRut(oVR), vRut);
                    return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, "{0} {1}", "Rut: No es Número:",vRut);
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region ValidarRut

        public static Boolean IsNumeric(string stringToTest)
        {
            int result;
            return int.TryParse(stringToTest, out result);
        }
        public Boolean IsNumeric2(string stringToTest)
        {
            int result;
            return int.TryParse(stringToTest, out result);
        }

        public int ValidarRut(string RutCliente)
        {
            try
            {
                if (RutCliente.Length < 3)
                    return 3;
                else if (RutCliente.Substring(RutCliente.Length - 2, 1) != "-")
                    return 4;
                else if (IsNumeric(RutCliente.Substring(0, RutCliente.Length - 2)) == false)
                    return 4;

                int Digito;
                int Contador;
                int Multiplo;
                int Acumulador;
                string RutDigito;


                string Original = RutCliente;

                Contador = 2;
                Acumulador = 0;


                int oRutIzquierda = 0;

                string oDv = "";

                oDv = RutCliente.Substring(RutCliente.Length - 1, 1);
                oRutIzquierda = Convert.ToInt32(RutCliente.Substring(0, RutCliente.Length - 2));


                while (oRutIzquierda != 0)
                {
                    Multiplo = (oRutIzquierda % 10) * Contador;
                    Acumulador = Acumulador + Multiplo;
                    oRutIzquierda = oRutIzquierda / 10;
                    Contador = Contador + 1;
                    if (Contador == 8)
                    {
                        Contador = 2;
                    }

                }

                Digito = 11 - (Acumulador % 11);
                RutDigito = Digito.ToString().Trim();
                if (Digito == 10)
                {
                    RutDigito = "K";
                }
                if (Digito == 11)
                {
                    RutDigito = "0";
                }

                if (RutDigito.ToLower() == oDv.ToLower())
                    return 1;
                else
                    return 2;
            }
            catch 
            {
                return 5;
            }
        } //Retorna 1 cuando esta OK

        public string ErrorValidarRut(int oError)
        {
            switch (oError)
            {
                case 2: return "Rut Incorrecto";
                case 3: return "Rut con largo incorrecto";
                case 4: return "Rut con formato incorrecto, ejemplo XXXXXXXX-X";
                case 5: return "Error Desconocido";
                default: return "Error desconocido";
            }
        }

        #endregion

        #region Existe Cliente
        private int ExisteCliente(string RutCliente)
        {

            int oRutIzquierda = Convert.ToInt32(RutCliente.Substring(0, RutCliente.Length - 2));
            var oDataContext = new LinqCLIDataContext();
            var oCount = (from m in oDataContext.TB_CLI_EMP_EMPRESAS
                          where m.PK_CLI_EMP_RUT == oRutIzquierda
                          select m).Count();
            if (oCount > 0)
                return 1;
            else
                return 0;
        }
        #endregion

        #region Limpia Caracteres: Elmina caracteres de una cadena de texto, reemplazandolos ejemplo: a --> á

        public string LimpiaCaracteres(string oTexto)
        {
            oTexto = oTexto.ToLower();
            oTexto = oTexto.Replace('á', 'a');
            oTexto = oTexto.Replace('é', 'e');
            oTexto = oTexto.Replace('í', 'i');
            oTexto = oTexto.Replace('ó', 'o');
            oTexto = oTexto.Replace('ú', 'u');
            oTexto = oTexto.Replace('ñ', 'n');
            oTexto = oTexto.Replace("'", " ");
            oTexto = oTexto.Replace("-", " ");
            oTexto = oTexto.Replace("_", " ");

            return oTexto;
        }

        public static string LimpiaAcentos(string oTexto)
        {
            oTexto = oTexto.Replace('á', 'a');
            oTexto = oTexto.Replace('é', 'e');
            oTexto = oTexto.Replace('í', 'i');
            oTexto = oTexto.Replace('ó', 'o');
            oTexto = oTexto.Replace('ú', 'u');
            oTexto = oTexto.Replace('Á', 'A');
            oTexto = oTexto.Replace('É', 'E');
            oTexto = oTexto.Replace('Í', 'I');
            oTexto = oTexto.Replace('Ó', 'O');
            oTexto = oTexto.Replace('Ú', 'U');
            return oTexto;
        }
        #endregion

        #region Validar Comuna
        public class ResultValidarComuna
        {
            public string oError { get; set; }
            public int iError { get; set; }
            public string Comuna { get; set; }
            public int ComunaId { get; set; }
        }

        public ResultValidarComuna ValidarComuna(string oTexto)
        {


            var oResult = new ResultValidarComuna();

            if (oTexto == null || oTexto == "")
                oResult.iError = 3;

            var oDataPar = new LinqBD_PARDataContext();

            var oValor = 0;

            if (IsNumeric(oTexto) == true)
                oValor = Convert.ToInt32(oTexto);

            oTexto = LimpiaCaracteres(oTexto);


            var oQuery1 = from COM in oDataPar.TB_PAR_COM_COMUNA
                          where (COM.FL_PAR_COM_NOMBRE_FORMAT == oTexto || COM.PK_PAR_COM_ID == oValor)
                          select COM;


            IQueryable<TB_PAR_COM_COMUNA> oDatos;

            if (oQuery1.Count() == 0)
            {
                oDatos = from COM in oDataPar.TB_PAR_COM_COMUNA
                         where (COM.FL_PAR_COM_NOMBRE_FORMAT.Contains(oTexto) || COM.PK_PAR_COM_ID == oValor)
                         select COM;
            }
            else
            {
                oDatos = oQuery1;
            }


            if ((oDatos.Count() == 0))
                oResult.iError = 2;
            else if (oDatos.Count() > 1)
                oResult.iError = 4;
            else
            {
                foreach (var file in oDatos)
                {
                    oResult.iError = 1;
                    oResult.Comuna = file.FL_PAR_COM_NOMBRE;
                    oResult.ComunaId = file.PK_PAR_COM_ID;
                }
            }

            oResult.oError = ErrorValidarComuna(oResult.iError);

            return oResult;
        }
        public string ErrorValidarComuna(int oError)
        {
            switch (oError)
            {
                case 1: return "Registro Ok";
                case 2: return "Comuna no coincide con tabla comunas, verificar nombre o código";
                case 3: return "Comuna en Blanco";
                case 4: return "Se contraron dos o más comunas que podrían coincidir con el elemento, espefique mas el nombre o código de comuna";
                default: return "Error desconocido";
            }
        }
        #endregion

        #region ValidarCoberturaComuna 0:OK
        public int ValidarCoberturaComuna(int idComuna)
        {

            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            var oCobertura = from cob in oPar.TB_PAR_COB_COBERTURA_COMUNA
                             where cob.PK_PAR_COM_ID == idComuna
                             select cob;
            if (oCobertura.Count() == 0)
                return 1;
            else
            {
                var oFila = oCobertura.ToList()[0];

                if (oFila.FL_PAR_COB_LU == false && oFila.FL_PAR_COB_MA == false && oFila.FL_PAR_COB_MI == false && oFila.FL_PAR_COB_JU == false && oFila.FL_PAR_COB_VI == false && oFila.FL_PAR_COB_SA == false && oFila.FL_PAR_COB_DO == false)
                    return 2;
                else
                    return 0;//OK
            }
        }
        #endregion

        #region GetIdCobertura 0:OK
        public int GetIdCobertura(int idComuna)
        {

            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            var oCobertura = from cob in oPar.TB_PAR_COB_COBERTURA_COMUNA
                             where cob.PK_PAR_COM_ID == idComuna
                             select cob;
            if (oCobertura.Count() > 0)
            {
                return oCobertura.ToList()[0].PK_PAR_SUC_ID;
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region ValidarCoberturaLocalidad
        public int ValidarCoberturaLocalidad(int idLocalidad)
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            var oCobertura = from col in oPar.TB_PAR_CLO_COBERTURA_LOCALIDAD
                             where col.PK_PAR_LOC_ID == idLocalidad
                             select col;
            if (oCobertura.Count() == 0)
                return 1;
            else
            {
                var oFila = oCobertura.ToList()[0];

                if (oFila.FL_PAR_CLO_LU == false && oFila.FL_PAR_CLO_MA == false && oFila.FL_PAR_CLO_MI == false && oFila.FL_PAR_CLO_JU == false && oFila.FL_PAR_CLO_VI == false && oFila.FL_PAR_CLO_SA == false && oFila.FL_PAR_CLO_DO == false)
                    return 2;
                else
                    return 0;
            }
        }
        #endregion

        #region Validar Localidad
        public class ResultValidarLocalidad
        {
            public string oError { get; set; }
            public int iError { get; set; }
            public string Localidad { get; set; }
            public int LocalidadId { get; set; }
        }

        public ResultValidarLocalidad ValidarLocalidad(string oTexto, int oComuna)
        {


            var oResult = new ResultValidarLocalidad();

            if (oTexto == null || oTexto == "")
                oResult.iError = 3;

            if (oResult.iError != 3)
            {
                var oDataPar = new LinqBD_PARDataContext();

                var oValor = 0;

                if (IsNumeric(oTexto) == true)
                    oValor = Convert.ToInt32(oTexto);

                oTexto = LimpiaCaracteres(oTexto);


                var oQuery1 = from LOC in oDataPar.TB_PAR_LOC_LOCALIDAD
                              where (LOC.FL_PAR_LOC_NOMBRE_FORMAT == oTexto || LOC.PK_PAR_LOC_ID == oValor)
                              && LOC.PK_PAR_COM_ID == oComuna
                              select LOC;


                IQueryable<TB_PAR_LOC_LOCALIDAD> oDatos;

                if (oQuery1.Count() == 0)
                {
                    oDatos = from LOC in oDataPar.TB_PAR_LOC_LOCALIDAD
                             where (LOC.FL_PAR_LOC_NOMBRE_FORMAT.Contains(oTexto) || LOC.PK_PAR_LOC_ID == oValor)
                             && LOC.PK_PAR_COM_ID == oComuna
                             select LOC;
                }
                else
                {
                    oDatos = oQuery1;
                }


                if ((oDatos.Count() == 0))
                    oResult.iError = 2;
                else if (oDatos.Count() > 1)
                    oResult.iError = 4;
                else
                {
                    foreach (var file in oDatos)
                    {
                        oResult.iError = 1;
                        oResult.Localidad = file.FL_PAR_LOC_NOMBRE;
                        oResult.LocalidadId = Convert.ToInt32(file.PK_PAR_LOC_ID);
                    }
                }
            }
            oResult.oError = ErrorValidarLocalidad(oResult.iError);

            return oResult;
        }
        public string ErrorValidarLocalidad(int oError)
        {
            switch (oError)
            {
                case 1: return "Registro Ok";
                case 2: return "Localidad no se encuentra en la Comuna, verificar Nombre o Código";
                case 3: return "Localidad en Blanco";
                case 4: return "Se contraron dos o más Localidades que podrían coincidir con el elemento, espefique mas el nombre o código de la Localidad";
                default: return "Error desconocido";
            }
        }
        #endregion

        #region ValidarColumnaServicio
        [HttpGet]
        public JsonResult ValidarColumnaServicio(string Columna, string ServicioRequerido)
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            var oDatos = (from Dat in oPar.TB_PAR_EXC_EXCEL
                          where Dat.PK_PAR_COL_ID == Convert.ToInt32(Columna) && Dat.PK_PAR_SER_ID == Convert.ToInt32(ServicioRequerido)
                          select Dat
                              ).Count();

            if (oDatos > 0)
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, ErrorValidarColumnaServicio(1));
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(true, JsonRequestBehavior.AllowGet);

        }
        public string ErrorValidarColumnaServicio(int oError)
        {
            switch (oError)
            {
                case 0: return "Registro Ok";
                case 1: return "Columna ya se encuentra definida para este servicio";
                default: return "Error desconocido";
            }
        }
        #endregion

        #region ValidarLetraColumnaServicio
        [HttpGet]
        public JsonResult ValidarLetraColumnaServicio(string Letra, string ServicioRequerido)
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            var oDatos = (from Dat in oPar.TB_PAR_EXC_EXCEL
                          where Dat.FL_PAR_EXC_LETRA == Convert.ToInt32(Letra) && Dat.PK_PAR_SER_ID == Convert.ToInt32(ServicioRequerido)
                          select Dat
                              ).Count();

            if (oDatos > 0)
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, ErrorValidarLetraColumnaServicio(1));
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(true, JsonRequestBehavior.AllowGet);

        }
        public string ErrorValidarLetraColumnaServicio(int oError)
        {
            switch (oError)
            {
                case 0: return "Registro Ok";
                case 1: return "Letra ya se encuentra definida para este servicio";
                default: return "Error desconocido";
            }
        }
        #endregion

        #region Buscar Direccion

        public class Coordenada
        {
            public double Latitud { get; set; }
            public double Longitud { get; set; }
        }

        public static Coordenada BuscarDireccion(string strDireccion)
        {



            string oDireccion = LimpiaAcentos(strDireccion + ", Chile");

            XDocument xDoc = XDocument.Load("http://maps.googleapis.com/maps/api/geocode/xml?address=" + oDireccion + "&sensor=false");

            var oLine1 = xDoc.Element("GeocodeResponse");
            var oEstado = oLine1.Element("status").Value;


            Coordenada oResult = new Coordenada();

            oResult.Latitud = 0;
            oResult.Longitud = 0;

            if (oEstado == "OK")
            {
                var oResultado = oLine1.Element("result");
                var oGeometry = oResultado.Element("geometry");
                var oLocation = oGeometry.Element("location");
                var oLat = oLocation.Element("lat").Value.ToString().Replace(".", ",");
                var oLon = oLocation.Element("lng").Value.ToString().Replace(".", ",");

                oResult.Latitud = Convert.ToDouble(oLat);
                oResult.Longitud = Convert.ToDouble(oLon);

            }

            return (oResult);
        }
        #endregion

        #region Sucursal
        public class dSucursal
        {
            public int id { get; set; }
            public string Nombre { get; set; }
            public int idEmpresa { get; set; }
            public int Tipo { get; set; }
            public string NombreEmpresa { get; set; }
            public int idSucursalEmpresa { get; set; }
            public string DvEmpresa { get; set; }
            public string NombreUsuario { get; set; }
        }
        public dSucursal Sucursal(string UserName)
        {

            dSucursal rSucursal = new dSucursal();

            rSucursal.id = 0;
            rSucursal.Nombre = "";
            if (UserName != "")
            {
                int usu_rut=Convert.ToInt32(UserName.Substring(0, UserName.Length - 2));
                LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
                var oSucursal = (from suc in oPar.TB_PAR_SUC_SUCURSAL
                                 join usu in oPar.TB_PAR_USU_USUARIO on suc.PK_PAR_SUC_ID equals usu.PK_PAR_SUC_ID
                                 where usu.PK_PAR_USU_RUT == usu_rut
                                 select new { suc,usu }).Single();



                rSucursal.id = oSucursal.suc.PK_PAR_SUC_ID;
                rSucursal.Nombre = oSucursal.suc.FL_PAR_SUC_NOMBRE;
                rSucursal.idEmpresa = 0;
                rSucursal.NombreEmpresa = "";
                rSucursal.idSucursalEmpresa = 0;
                rSucursal.Tipo = oSucursal.usu.PK_PAR_TUS_ID;

                if (oSucursal.usu.PK_PAR_TUS_ID == 2)
                {
                    var oEmpresa = (from vi in oPar.VI_PAR_USUARIO
                                   where vi.PK_PAR_USU_RUT == usu_rut
                                   select vi).Single();
                    rSucursal.idEmpresa = Convert.ToInt32(oEmpresa.PK_CLI_EMP_RUT);
                    rSucursal.idSucursalEmpresa = Convert.ToInt32(oEmpresa.PK_CLI_SUC_ID);
                    rSucursal.NombreEmpresa = oEmpresa.FL_CLI_EMP_FANTASIA;
                    rSucursal.DvEmpresa = oEmpresa.FL_CLI_EMP_DV;                    
                }

                var oDatosUser = (from usu in oPar.TB_PAR_USU_USUARIO
                                 where usu.PK_PAR_USU_RUT == usu_rut
                                 select usu).Single();

                rSucursal.NombreUsuario = oDatosUser.FL_PAR_USU_NOMBRES + " " + oDatosUser.FL_PAR_USU_APELLIDOS;
            }

            

            return rSucursal;
        }
        #endregion

        #region funColumnasParametricas
        public static ColumnasParametricas funColumnasParametricas(decimal idServicio)
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oConsulta = from exc in oPar.TB_PAR_EXC_EXCEL
                            where exc.PK_PAR_SER_ID == idServicio
                            select exc;


            ColumnasParametricas oColumnaParametrica = new ColumnasParametricas();

            foreach (var oColumna in oConsulta)
            {
                switch (oColumna.PK_PAR_COL_ID)
                {
                    case 1://1	Id
                        oColumnaParametrica.Id = true;
                        break;
                    case 2://2	Referencia
                        oColumnaParametrica.Referencia = true;

                        break;
                    case 3://3	Destinatario
                        oColumnaParametrica.Destinatario = true;

                        break;
                    case 4:// 4	Rut
                        oColumnaParametrica.Rut = true;

                        break;
                    case 5://5	Contacto Entrega
                        oColumnaParametrica.ContactoEntrega = true;
                        break;
                    case 6://6	Dirección
                        oColumnaParametrica.Direccion = true;
                        break;
                    case 7://7	Comuna
                        oColumnaParametrica.Comuna = true;
                        break;
                    case 8://8	Bulto
                        oColumnaParametrica.Bulto = true;

                        break;
                    case 9://9	Peso
                        oColumnaParametrica.Peso = true;

                        break;
                    case 10://10	Fono
                        oColumnaParametrica.Fono = true;

                        break;
                    case 11://11	Número Direccion
                        oColumnaParametrica.NDireccion = true;

                        break;
                    case 12://12	Localidad
                        oColumnaParametrica.Localidad = true;

                        break;
                    case 13://13	Vía
                        oColumnaParametrica.Via = true;

                        break;
                    case 14://14	Código Postal
                        oColumnaParametrica.CodigoPostal = true;
                        break;
                }
            }
            return oColumnaParametrica;
        }        
        #endregion

        #region ExisteReferenciaV2
        public class RespuestaExisteReferencia
        {
            public bool Ok {get;set;}
            public string Mensaje {get;set;}
        }
        public RespuestaExisteReferencia ExisteReferenciaV2(string oRutCliente, string Referencia, int NumeroReferencia, int Servicio)
        {
            //oRutCliente: Rut cliente en formato texto con guion 12345678-1
            //Referencia, string con la cadena de referencia
            //NumeroReferencia: 1, referencia 1, 2..
            //Servicio, id servicio
            RespuestaExisteReferencia oResult = new RespuestaExisteReferencia();
            if (Referencia == null || Referencia == "")
            {
                oResult.Ok = true;//No deben llegar valores en blanco, si llegan es por un error
                return oResult;
            }
            else if (Referencia.Trim() == "")
            {
                oResult.Ok = true;
                return oResult;
            }
            
            Referencia = Referencia.Trim();
            
            
            
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            ValidationController oValidation = new ValidationController();
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            if (Referencia.Length > 100)
            {
                oResult.Ok = false;
                oResult.Mensaje = "Referencia Excede largo máximo";
            }
            else
            {
                if (ValidarRut(oRutCliente) != 1)
                {
                    oResult.Ok = false;
                    oResult.Mensaje = "Rut de cliente incorrecto";
                }
                else
                {

                    var oDatos = from res in oPar.TB_PAR_RES_REFERENCIA_SERVICIO
                                 join tdo in oPar.TB_PAR_TDO_TIPO_DOCUMENTO on res.PK_PAR_TDO_ID equals tdo.PK_PAR_TDO_ID
                                 where res.PK_PAR_SER_ID == Servicio
                                 && res.PK_PAR_REF_ID == NumeroReferencia
                                 select new { res, tdo };

                    if (oDatos.Count() == 0)//Si es 0 es que no hay definición
                    {
                        oResult.Ok = false;
                        oResult.Mensaje = "No se ha definido un tipo para la referencia " + NumeroReferencia.ToString();
                    }
                    else
                    {
                        var oElemento = oDatos.ToList()[0];//Solo debe existir 1 registro, por integridad de la bd
                        if (oElemento.tdo.PK_PAR_TRE_ID == 1)//1:documento
                        {
                            if (oValidation.isDecimal(Referencia) == false)
                            {
                                oResult.Ok = false;
                                oResult.Mensaje = string.Format("La referencia {0}, debe ser número", NumeroReferencia);
                            }
                            else
                            {
                                decimal DOA_NUMERO = Convert.ToDecimal(Referencia);

                                var oRefe = from refe in oFol.TB_FOL_DOA_DOC_ASOCIADO
                                            where refe.PK_FOL_DOA_NUMERO == DOA_NUMERO
                                            && refe.PK_CLI_EMP_RUT == Convert.ToInt32(oRutCliente.Substring(0, oRutCliente.Length - 2))
                                            && refe.PK_PAR_TDO_ID == oElemento.tdo.PK_PAR_TDO_ID
                                            select refe;
                                if (oRefe.Count() > 0)
                                {
                                    var oElementRefe = oRefe.ToList()[0];
                                    oResult.Ok = false;
                                    oResult.Mensaje = string.Format("Referencia {0}, ya existe en la OT {1}-{2}", NumeroReferencia, oElementRefe.PK_FOL_OTP_ID, oElementRefe.PK_FOL_OTD_ID);
                                }
                                else
                                {
                                    oResult.Ok = true;
                                }
                            }
                        }
                        else//, 2:referencia, hasta el momento del desarrollo de esta función solo existian 2 tipos y no se contempla crear mas en el futuro
                        {

                            var oRefe = from refe in oFol.TB_FOL_REA_REF_ASOCIADO
                                        where refe.PK_FOL_REA_NUMERO == Referencia
                                        && refe.PK_CLI_EMP_RUT == Convert.ToInt32(oRutCliente.Substring(0, oRutCliente.Length - 2))
                                        && refe.PK_PAR_TDO_ID == oElemento.tdo.PK_PAR_TDO_ID
                                        select refe;
                            if (oRefe.Count() > 0)
                            {
                                var oElementRefe = oRefe.ToList()[0];
                                oResult.Ok = false;
                                oResult.Mensaje = string.Format("Referencia {0}, ya existe en la OT {1}-{2}", NumeroReferencia, oElementRefe.PK_FOL_OTP_ID, oElementRefe.PK_FOL_OTD_ID);
                            }
                            else
                            {
                                oResult.Ok = true;
                            }
                        }
                    }
                }
            }
            return oResult;
        }
        #endregion

        #region Existe Referencia
        public int ExisteReferencia(string oRutCliente, string oReferencia1, string oReferencia2, int  Servicio)
        {
            if (ValidarRut(oRutCliente) != 1)
                return 1;

            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();           
            ValidationController oValidation = new ValidationController();
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();


            if (oReferencia1 != "" && oReferencia1!=null)
            {
                if (oReferencia1.Trim() != "" && oReferencia1.Count() > 0 && oReferencia1!=string.Empty)
                {
                    var oResult = from otd in oFol.TB_FOL_OTD_OT_DETALLE
                                  join otp in oFol.TB_FOL_OTP_OT_PRINCIPAL on otd.PK_FOL_OTP_ID equals otp.PK_FOL_OTP_ID
                                  where otd.FL_FOL_OTD_REFERENCIA1 == oReferencia1.Trim()
                                  && otp.PK_CLI_EMP_RUT == Convert.ToInt32(oRutCliente.Substring(0, oRutCliente.Length - 2))
                                  select otd;

                    if (oResult.Count() != 0)
                    {
                        return 2;
                    }

                    var oDatos = from res in oPar.TB_PAR_RES_REFERENCIA_SERVICIO
                                 join tdo in oPar.TB_PAR_TDO_TIPO_DOCUMENTO on res.PK_PAR_TDO_ID equals tdo.PK_PAR_TDO_ID
                                 where res.PK_PAR_SER_ID == Servicio
                                 && res.PK_PAR_REF_ID == 1//REFERENCIA 1
                                 select new { res, tdo };
                    if (oDatos.Count() != 0)//Si es 0 es que no hay definición
                    {
                        var oElemento = oDatos.ToList()[0];//Solo debe existir 1 registro, por integridad de la bd
                        if (oElemento.tdo.PK_PAR_TRE_ID == 1)//1:documento, 2:referencia
                        {
                            if (oValidation.isDecimal(oReferencia1) == false)
                            {
                                return 3;
                            }
                            else
                            {
                                decimal DOA_NUMERO = Convert.ToDecimal(oReferencia1);

                                var oRefe = from refe in oFol.TB_FOL_DOA_DOC_ASOCIADO
                                            where refe.PK_FOL_DOA_NUMERO == DOA_NUMERO
                                            && refe.PK_CLI_EMP_RUT == Convert.ToInt32(oRutCliente.Substring(0, oRutCliente.Length - 2))
                                            && refe.PK_PAR_TDO_ID == oElemento.tdo.PK_PAR_TDO_ID
                                            select refe;
                                if (oRefe.Count() > 0)
                                {
                                    return 4;
                                }
                            }
                        }
                    }
                }
            }
            else if (oReferencia2 != "")
            {
                if (oReferencia2.Trim() != "" && oReferencia2.Count() > 0 && oReferencia2 != string.Empty)
                {
                    var oResult = from otd in oFol.TB_FOL_OTD_OT_DETALLE
                                  join otp in oFol.TB_FOL_OTP_OT_PRINCIPAL on otd.PK_FOL_OTP_ID equals otp.PK_FOL_OTP_ID
                                  where otd.FL_FOL_OTD_REFERENCIA2 == oReferencia2.Trim()
                                  && otp.PK_CLI_EMP_RUT == Convert.ToInt32(oRutCliente.Substring(0, oRutCliente.Length - 2))
                                  select otd;

                    if (oResult.Count() != 0)
                    {
                        return 2;
                    }

                    var oDatos = from res in oPar.TB_PAR_RES_REFERENCIA_SERVICIO
                                 join tdo in oPar.TB_PAR_TDO_TIPO_DOCUMENTO on res.PK_PAR_TDO_ID equals tdo.PK_PAR_TDO_ID
                                 where res.PK_PAR_SER_ID == Servicio
                                 && res.PK_PAR_REF_ID == 2//REFERENCIA 2
                                 select new { res, tdo };
                    if (oDatos.Count() != 0)//Si es 0 es que no hay definición
                    {
                        var oElemento = oDatos.ToList()[0];//Solo debe existir 1 registro, por integridad de la bd
                        if (oElemento.tdo.PK_PAR_TRE_ID == 1)//1:documento, 2:referencia
                        {
                            if (oValidation.isDecimal(oReferencia2) == false)
                            {
                                return 3;
                            }
                            else
                            {
                                decimal DOA_NUMERO = Convert.ToDecimal(oReferencia2);

                                var oRefe = from refe in oFol.TB_FOL_DOA_DOC_ASOCIADO
                                            where refe.PK_FOL_DOA_NUMERO == DOA_NUMERO
                                            && refe.PK_CLI_EMP_RUT == Convert.ToInt32(oRutCliente.Substring(0, oRutCliente.Length - 2))
                                            && refe.PK_PAR_TDO_ID == oElemento.tdo.PK_PAR_TDO_ID
                                            select refe;
                                if (oRefe.Count() > 0)
                                {
                                    return 4;
                                }
                            }
                        }
                    }
                }
            }


            //0:OK
            //1:
            //2:Referencia Existe
            //3:Referencia, tipo documento debe ser numero
            //4:Referencia existe como documento asociado

            return 0;
        }
        #endregion //0 OK

        #region RetornaNombreRegion
        public string RetornaNombreRegion(int idRegion)
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oDatos = from x in oPar.TB_PAR_REG_REGION
                         where x.PK_PAR_REG_ID == idRegion
                         select x;

            if (oDatos.Count() == 1)
                return oDatos.ToList()[0].PK_PAR_REG_ID + " " + oDatos.ToList()[0].FL_PAR_REG_NOMBRE;
            else
                return "";

        }
        #endregion

        #region RetornaNombreComunaRegion
        public class NombreRegionComuna
        {
            public string Region { get; set; }
            public string Comuna { get; set; }
            public string ComunaIATA { get; set; }            
        }
        public NombreRegionComuna RetornaNombreComunaRegion(string sidComuna)
        {
            int idComuna = 0;
            if (IsNumeric(sidComuna))
                idComuna = Convert.ToInt32(sidComuna);

            NombreRegionComuna oNombres = new NombreRegionComuna();
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oDatos = from reg in oPar.TB_PAR_REG_REGION
                         join prv in oPar.TB_PAR_PRV_PROVINCIA on reg.PK_PAR_REG_ID equals prv.PK_PAR_REG_ID
                         join com in oPar.TB_PAR_COM_COMUNA on prv.PK_PAR_PRV_ID equals com.PK_PAR_PRV_ID
                         where com.PK_PAR_COM_ID == idComuna
                         select new
                         {
                             reg,
                             com
                         };

            if (oDatos.Count() == 1)
            {
                var DatosRetornados = oDatos.ToList()[0];
                oNombres.Region = DatosRetornados.reg.PK_PAR_REG_ID + " " + DatosRetornados.reg.FL_PAR_REG_NOMBRE;
                oNombres.Comuna = DatosRetornados.com.FL_PAR_COM_NOMBRE;
                oNombres.ComunaIATA = DatosRetornados.com.FL_PAR_COM_CODIGO;

                return oNombres;
            }
            else
            {
                oNombres.Region = "";
                oNombres.Comuna = "";
                return oNombres;
            }

        }
        #endregion

        #region RetornaNombreLocalidadComunaRegion
        public class NombreRegionComunaLocalidad
        {
            public string Region { get; set; }
            public string Comuna { get; set; }
            public string Localidad { get; set; }
            public string ComunaIATA { get; set; }
            public string LocalidadIATA { get; set; }
        }
        public NombreRegionComunaLocalidad RetornaNombreLocalidadComunaRegion(string sidLocalidad)
        {
            int idLocalidad = 0;
            if (IsNumeric(sidLocalidad))
                idLocalidad = Convert.ToInt32(sidLocalidad);

            NombreRegionComunaLocalidad oNombres = new NombreRegionComunaLocalidad();
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oDatos = from reg in oPar.TB_PAR_REG_REGION
                         join prv in oPar.TB_PAR_PRV_PROVINCIA on reg.PK_PAR_REG_ID equals prv.PK_PAR_REG_ID
                         join com in oPar.TB_PAR_COM_COMUNA on prv.PK_PAR_PRV_ID equals com.PK_PAR_PRV_ID
                         join loc in oPar.TB_PAR_LOC_LOCALIDAD on com.PK_PAR_COM_ID equals loc.PK_PAR_COM_ID
                         where loc.PK_PAR_LOC_ID == idLocalidad
                         select new
                         {
                             reg,
                             com,
                             loc
                         };

            if (oDatos.Count() == 1)
            {

                var DatosLista = oDatos.ToList()[0];

                oNombres.Region = DatosLista.reg.PK_PAR_REG_ID + " " + DatosLista.reg.FL_PAR_REG_NOMBRE;
                oNombres.Comuna = DatosLista.com.FL_PAR_COM_NOMBRE;
                oNombres.Localidad = DatosLista.loc.FL_PAR_LOC_NOMBRE;
                oNombres.ComunaIATA = DatosLista.com.FL_PAR_COM_CODIGO;
                oNombres.LocalidadIATA = DatosLista.loc.FL_PAR_LOC_CODIGO;

                return oNombres;
            }
            else
            {
                oNombres.Region = "";
                oNombres.Comuna = "";
                oNombres.Localidad = "";
                return oNombres;
            }

        }
        #endregion

        #region RetornaNombreEmpresa
        public string RetornaNombreEmpresa(int RutEmpresa)
        {
            LinqCLIDataContext oPar = new LinqCLIDataContext();
            var oDatos = from emp in oPar.TB_CLI_EMP_EMPRESAS
                         where emp.PK_CLI_EMP_RUT == RutEmpresa
                         select emp;

            if (oDatos.Count() > 0)
                return oDatos.ToList()[0].FL_CLI_EMP_RAZON_SOCIAL;
            else
                return "";

        }
        #endregion

        #region RetornaNombreServicio
        public string RetornaNombreServicio(int idServicio)
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oDatos = from ser in oPar.TB_PAR_SER_SERVICIO
                         where ser.PK_PAR_SER_ID == idServicio
                         select ser;

            if (oDatos.Count() > 0)
                return oDatos.ToList()[0].FL_PAR_SER_NOMBRE;
            else
                return "";

        }
        #endregion

        #region vjSonValidarIdBultoenOTD
        public JsonResult vjSonValidarIdBultoenOTD(string ingIdBulto, decimal ingOTP, decimal ingOTD, int Edit)
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            string strCodBulto;
            decimal decCodBulto = 0;

            if (ingIdBulto == "")
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Código en blanco");
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }

            if (ingIdBulto.Substring(0, 1).ToUpper() == "B")//Es Bulto
            {
                if (ingIdBulto.Length > 1)//tiene algo
                {
                    strCodBulto = ingIdBulto.Substring(1, ingIdBulto.Length - 1);
                    if (IsNumeric(strCodBulto))
                    {
                        decCodBulto = Convert.ToDecimal(strCodBulto);
                    }
                    else//error no es numero
                    {
                        string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Código de Bulto incorrecto");
                        return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                    }
                }
                else//Error
                {
                    string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Código de Bulto incorrecto");
                    return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                }

            }
            else if (ingIdBulto.Substring(0, 1).ToUpper() == "P")//Es OT
            {
                var oResult = GetOTOTDfromCodigo(ingIdBulto);
                if (oResult.OK == true)
                {
                    var oCodigos = from blt in oFol.TB_FOL_BLT_BULTO
                                   where
                                    blt.PK_FOL_OTP_ID == oResult.OTP
                                    &&
                                    blt.PK_FOL_OTD_ID == oResult.OTD
                                    &&
                                    blt.PK_FOL_EST_ID != 13 //Eliminado no se considera
                                   select blt.PK_FOL_BLT_ID;

                    int countBultos = oCodigos.Count();

                    if (countBultos == 1)
                    {
                        decCodBulto = oCodigos.ToList()[0];
                    }
                    else if (countBultos == 0)
                    {
                        string suggestedUID = String.Format(CultureInfo.InvariantCulture, "No se encontro ningun bulto asociado a esta OT");
                        return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        string suggestedUID = String.Format(CultureInfo.InvariantCulture, "OT tiene " + countBultos.ToString() + " bultos, debe ingresar los bultos por separado");
                        return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Código de OT incorrecto");
                    return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                }
            }
            else //No es nada
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Código no existe");
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
            var oDatos = from blt in oFol.TB_FOL_BLT_BULTO
                         where blt.PK_FOL_OTP_ID == ingOTP
                         && blt.PK_FOL_OTD_ID == ingOTD
                         && blt.PK_FOL_BLT_ID == decCodBulto
                         && blt.PK_FOL_EST_ID != 13 //Eliminado no puedo
                         select blt;

            if (oDatos.Count() == 1)
            {
                var oElemento = oDatos.ToList()[0];
                if ((oElemento.PK_FOL_EST_ID == 12 || oElemento.PK_FOL_EST_ID == 22) && Edit == 1)//1 Edición, 12 y 22 Preparado
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else if ((oElemento.PK_FOL_EST_ID == 11 || oElemento.PK_FOL_EST_ID == 21) && Edit == 2)//2 Nuevos datos, 11 y 21, En preparación
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else if (Edit == 1)
                {
                    string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Bulto no se encuentra en un estado habilitado para su edición");
                    return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Bulto no se encuentra en un estado habilitado para su ingreso");
                    return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Código Bulto no existe para la OT");
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region GetSucursalFromComuna
        public int GetSucursalfromComuna(int idComuna)
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oDatos = (from x in oPar.TB_PAR_COB_COBERTURA_COMUNA
                          where x.PK_PAR_COM_ID == idComuna
                          select x.PK_PAR_SUC_ID).Single();
            return oDatos;
        }
        #endregion

        #region GetSucursalNameFromIdScurusal
        public string GetSucursalNamefromIDSucursal(int idSucursal)
        {
            if (idSucursal > 0)
            {
                LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
                var oDatos = (from x in oPar.TB_PAR_SUC_SUCURSAL
                              where x.PK_PAR_SUC_ID == idSucursal
                              select x.FL_PAR_SUC_NOMBRE).Single();
                return oDatos;
            }
            else
            {
                return "";
            }
        }
        #endregion

        #region GetSucursalIDfromUser
        public int GetSucursalIDfromActiveUser()
        {
            int oUser = GetRutActiveUser();
            if (oUser != 0)
            {
                try
                {
                    LinqBD_PARDataContext oFol = new LinqBD_PARDataContext();
                    var oSucursal = from suc in oFol.TB_PAR_USU_USUARIO
                                    where suc.PK_PAR_USU_RUT == oUser
                                    select suc;
                    if (oSucursal.Count() > 0)
                    {
                        return Convert.ToInt32(oSucursal.ToList()[0].PK_PAR_SUC_ID);
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region GetSucursalIDfromUser
        public int GetSucursalEmpresaIDfromActiveUser()
        {
            int oUser = GetRutActiveUser();
            if (oUser != 0)
            {
                try
                {
                    LinqBD_PARDataContext oFol = new LinqBD_PARDataContext();
                    var oSucursal = from suc in oFol.TB_PAR_USE_USU_SUC_EMP
                                    where suc.PK_PAR_USU_RUT == oUser
                                    select suc;

                    if (oSucursal.Count() > 0)
                    {
                        return oSucursal.ToList()[0].PK_CLI_EMP_RUT;
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region GetServicioNamefromOTP
        public string GetServicioNamefromOTP(decimal OTP)
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oDatos = (from x in oFol.TB_FOL_OTP_OT_PRINCIPAL
                          where x.PK_FOL_OTP_ID == OTP
                          select x.PK_PAR_SER_ID).Single();

            var oServicio = (from x in oPar.TB_PAR_SER_SERVICIO
                             where x.PK_PAR_SER_ID == oDatos
                             select x.FL_PAR_SER_NOMBRE).Single();

            return oServicio;
        }
        #endregion

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

        #region GetEstadoManifiestoFromIdManifiesto
        public string GetEstadoManifiestoFromIdManifiesto(decimal IdManifiesto)
        {
            try
            {
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                var oDatos = (from man in oFol.TB_FOL_MAN_MANIFIESTO
                              join est in oFol.TB_FOL_MAE_MANIFIESTO_ESTADO on man.PK_FOL_MAE_ID equals est.PK_FOL_MAE_ID
                              where man.PK_FOL_MAN_ID == IdManifiesto
                              select est).Single();
                return oDatos.FL_FOL_MAE_NOMBRE;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        #endregion



        #region GetPatenteManifiesto
        public string GetPatenteManifiesto(decimal IdManifiesto)
        {
            try
            {
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                var oDatos = (from man in oFol.TB_FOL_MAN_MANIFIESTO                              
                              where man.PK_FOL_MAN_ID == IdManifiesto
                              select man).Single();
                return oDatos.PK_PAR_TRA_ID.ToString();
            }
            catch (Exception e)
            {
                return "";
            }
        }
        #endregion

        #region GetProveedorManifiesto
        public string GetProveedorManifiesto(decimal IdManifiesto)
        {
            try
            {
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                var oDatos = (from man in oFol.TB_FOL_MAN_MANIFIESTO
                              where man.PK_FOL_MAN_ID == IdManifiesto
                              select man).Single();
                return oDatos.PK_PAR_PRO_ID.ToString();
            }
            catch (Exception e)
            {
                return "";
            }
        }
        #endregion

        #region GetConductorManifiesto
        public string GetConductorManifiesto(decimal IdManifiesto)
        {
            try
            {
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                var oDatos = (from man in oFol.TB_FOL_MAN_MANIFIESTO
                              where man.PK_FOL_MAN_ID == IdManifiesto
                              select man).Single();
                return oDatos.PK_PAR_CON_RUT.ToString();
            }
            catch (Exception e)
            {
                return "";
            }
        }
        #endregion

        #region vJsonValidarExisteManifiestoId
        public JsonResult vJsonValidarExisteManifiestoId(string NumeroBusquedaManifiesto)
        {            
            if (isDecimal(NumeroBusquedaManifiesto))
            {
                decimal IdManifiesto = Convert.ToDecimal(NumeroBusquedaManifiesto);
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

                var oDatos = from man in oFol.TB_FOL_MAN_MANIFIESTO
                             where man.PK_FOL_MAN_ID == IdManifiesto
                             select man;

                if (oDatos.Count() == 1)
                    return Json(true, JsonRequestBehavior.AllowGet);
                else
                {
                    string suggestedUID = String.Format(CultureInfo.InvariantCulture, "N° de Manifiesto no existe");
                    return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, "N° de Manifiesto debe ser Número");
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region GetOTOTDfromCodigo
        public class OTOTD
        {
            public decimal OTP { get; set; }
            public decimal OTD { get; set; }
            public bool OK { get; set; }//true:sin Error
        }
        public OTOTD GetOTOTDfromCodigo(string CodigoOT)
        {
            string lotp = string.Empty;
            string lotd = string.Empty;
            int switchOTP = 0;
            foreach (var a in CodigoOT)
            {
                if (switchOTP == 1 && a.ToString() != "D")
                {
                    lotp += a.ToString();
                }
                else if (switchOTP == 2 && a.ToString() != "D")
                {
                    lotd += a.ToString();
                }
                if (a.ToString() == "P")
                {
                    switchOTP = 1;
                }
                if (a.ToString() == "D")
                {
                    switchOTP = 2;
                }
            }
            OTOTD oResult = new OTOTD();
            if (IsNumeric(lotp) && IsNumeric(lotd))
            {
                oResult.OTP = Convert.ToDecimal(lotp);
                oResult.OTD = Convert.ToDecimal(lotd);
                oResult.OK = true;
            }
            else
            {
                oResult.OTP = 0;
                oResult.OTD = 0;
                oResult.OK = false;
            }
            return oResult;
        }
        #endregion

        #region GetCodigoBultoFromCodigoGenerico
        public decimal GetCodigoBultoFromCodigoGenerico(string oCodigo)
        {
            if (oCodigo != null)
            {
                if (oCodigo.Length > 0)
                {
                    string oTipo = oCodigo.Substring(0, 1);
                    LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                    if (oTipo.ToUpper() == "P")//OT
                    {
                        var oOT = GetOTOTDfromCodigo(oCodigo);
                        if (oOT.OK == true)//si es un OT
                        {
                            var oGetCodigo = from blt in oFol.TB_FOL_BLT_BULTO
                                             where blt.PK_FOL_OTP_ID == oOT.OTP
                                             && blt.PK_FOL_OTD_ID == oOT.OTD
                                             select blt.PK_FOL_BLT_ID;
                            if (oGetCodigo.Count() == 1)
                            {
                                return oGetCodigo.ToList()[0];
                            }
                            else
                            {
                                return 0;
                            }
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else if (oTipo.ToUpper() == "B")//Bulto
                    {
                        string oParteCodigo = oCodigo.Substring(1, oCodigo.Length - 1);
                        if (IsNumeric(oParteCodigo))
                            return Convert.ToDecimal(oParteCodigo);
                        else
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region isDecimal
        public bool isDecimal(string valor)
        {
            if (valor != null)
            {
                int ContComas = 0;
                foreach (var a in valor)
                {
                    if (!IsNumeric(a.ToString()))
                    {
                        if (a.ToString() == ",")
                            ContComas += 1;
                        else
                        {
                            return false;
                        }
                    }
                    if (ContComas > 1)
                        return false;
                }


                decimal decVal;
                if (decimal.TryParse(valor, out decVal))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion
       
        #region vjSonValidarIdBultoDespacho
        public RetornoModels vjSonValidarIdBultoDespacho(string idBultoVal, decimal NumeroManifiesto)
        {
            string idBulto = idBultoVal;
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            string strCodBulto;
            decimal decCodBulto = 0;

            RetornoModels oRetorno = new RetornoModels();

            int oSucursal = GetSucursalIDfromActiveUser();

            if (idBulto.Substring(0, 1).ToUpper() == "B")//Es Bulto
            {
                if (idBulto.Length > 1)//tiene algo
                {
                    strCodBulto = idBulto.Substring(1, idBulto.Length - 1);
                    if (IsNumeric(strCodBulto))
                    {
                        decCodBulto = Convert.ToDecimal(strCodBulto);
                    }
                    else//error no es numero
                    {
                        oRetorno.Ok = false;
                        oRetorno.Mensaje = "Código de Bulto incorrecto";
                        return oRetorno ;
                    }
                }
                else//Error
                {
                    oRetorno.Ok = false;
                    oRetorno.Mensaje = "Código de Bulto incorrecto";
                    return oRetorno;
                }
            }
            else if (idBulto.Substring(0, 1).ToUpper() == "P")//Es OT
            {
                var oResult = GetCodigoBultoFromCodigoGenerico(idBulto);
                if (oResult == 0)
                {
                    oRetorno.Ok = false;
                    oRetorno.Mensaje = "Código de Bulto incorrecto";
                    return oRetorno;
                }
                else
                {
                    decCodBulto = oResult;
                }
            }
            else
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = "Código de Bulto no existe";
                return oRetorno;
            }

            var oCodigos = from blt in oFol.TB_FOL_BLT_BULTO
                           join est in oFol.TB_FOL_EST_ESTADO on blt.PK_FOL_EST_ID equals est.PK_FOL_EST_ID
                           join otd in oFol.TB_FOL_OTD_OT_DETALLE on
                                new { blt.PK_FOL_OTP_ID, blt.PK_FOL_OTD_ID } equals new { otd.PK_FOL_OTP_ID , otd.PK_FOL_OTD_ID}
                           join uab in oFol.TB_FOL_UAB_UBICACION_ACTUAL_BULTO on blt.PK_FOL_BLT_ID equals uab.PK_FOL_BLT_ID
                           join otp in oFol.TB_FOL_OTP_OT_PRINCIPAL on blt.PK_FOL_OTP_ID equals otp.PK_FOL_OTP_ID
                           where
                           blt.PK_FOL_BLT_ID == decCodBulto
                           select new
                           {
                               blt,
                               est,
                               otd,
                               uab,
                               otp
                           };

                        


            int countBultos = oCodigos.Count();
            if (countBultos == 0)
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = "Código de Bulto no existe";
                return oRetorno;
            }
            else
            {
                var oBulto = oCodigos.ToList()[0];


                if (oBulto.otd.FL_FOL_OTD_DEVOLUCION == true)
                {
                    var oDireccionEntrega = from den in oFol.TB_FOL_DEN_DIRECCION_ENTREGA
                                            where den.PK_FOL_OTP_ID == oBulto.otd.PK_FOL_OTP_ID && den.PK_FOL_OTD_ID == oBulto.otd.PK_FOL_OTD_ID
                                            orderby den.PK_FOL_DEN_ID descending
                                            select den;

                    if (oDireccionEntrega.Count() > 0)
                    {
                        var oItemDireccion = oDireccionEntrega.ToList()[0];
                        if (oItemDireccion.PK_FOL_TDE_ID == 1)
                        {
                            string suggestedUID = String.Format(CultureInfo.InvariantCulture, "OT EN DEVOLUCIÓN, debe ingresar una dirección de devolución antes de su despacho");                            
                            oRetorno.Ok = false;
                            oRetorno.Mensaje = suggestedUID;
                            return oRetorno;
                        }
                    }
                    else
                    {
                        string suggestedUID = String.Format(CultureInfo.InvariantCulture, "OT EN DEVOLUCIÓN, debe ingresar una dirección de devolución antes de su despacho");
                        oRetorno.Ok = false;
                        oRetorno.Mensaje = suggestedUID;
                        return oRetorno;
                    }
                }

                if (oBulto.blt.PK_FOL_EST_ID == 13)
                {
                    string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Bulto Eliminado del Sistema");
                    oRetorno.Ok = false;
                    oRetorno.Mensaje = suggestedUID;
                    return oRetorno;
                }
                else if (oBulto.blt.PK_FOL_EST_ID == 14)
                {
                    var oMab = from mab in oFol.TB_FOL_MAB_MANIFIESTO_BULTO                               
                               where mab.PK_FOL_BLT_ID == oBulto.blt.PK_FOL_BLT_ID
                               && mab.PK_FOL_MAN_ID == NumeroManifiesto
                               select mab;

                    if (oMab.Count() == 0)
                    {
                        string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Error: Bulto se encuentra en otro manifiesto.");
                        oRetorno.Ok = false;
                        oRetorno.Mensaje = suggestedUID;
                        return oRetorno;
                    }
                    else
                    {
                        string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Bulto ya ingresado.");
                        oRetorno.Ok = false;
                        oRetorno.Mensaje = suggestedUID;
                        return oRetorno;
                    }
                }
                else if (oBulto.blt.PK_FOL_EST_ID != 12 && oBulto.blt.PK_FOL_EST_ID!=22)
                {
                    string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Bulto se encuentra en estado " + oBulto.est.FL_FOL_EST_NOMBRE + ", " + oBulto.est.FL_FOL_EST_DESCRIPCION);
                    oRetorno.Ok = false;
                    oRetorno.Mensaje = suggestedUID;
                    return oRetorno;
                }
                else if (oBulto.otd.PK_FOL_EST_ID != 12 && oBulto.otd.PK_FOL_EST_ID != 22)
                {
                    string suggestedUID = String.Format(CultureInfo.InvariantCulture, "OT " + oBulto.otd.PK_FOL_OTP_ID + "-" + oBulto.otd.PK_FOL_OTD_ID + " no se encuentra Preparada, debe preparar todos los bultos de la OT antes de su despacho");
                    oRetorno.Ok = false;
                    oRetorno.Mensaje = suggestedUID;
                    return oRetorno;
                }

                if (oBulto.uab.PK_PAR_SUC_ID != oSucursal && oBulto.otd.PK_FOL_EST_ID==12)
                {
                    string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Bulto se encuentra en sucursal <strong>" + GetSucursalNamefromIDSucursal(oBulto.uab.PK_PAR_SUC_ID) + "<strong>");
                    oRetorno.Ok = false;
                    oRetorno.Mensaje = suggestedUID;
                    return oRetorno;
                }

                ValidationController oValidation = new ValidationController();
                var oSucEmpresa = oValidation.GetSucursalEmpresaIDfromActiveUser();
                
                if (oBulto.otd.PK_FOL_EST_ID == 22 && oSucEmpresa ==0)
                {
                    string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Bulto en bodega cliente");
                    oRetorno.Ok = false;
                    oRetorno.Mensaje = suggestedUID;
                    return oRetorno;
                }

                //if (oBulto.otd.PK_FOL_EST_ID == 22 && oSucEmpresa != oBulto.otp.PK_CLI_EMP_RUT)
                //{
                //    string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Bulto pertenece a otro cliente");
                //    oRetorno.Ok = false;
                //    oRetorno.Mensaje = suggestedUID;
                //    return oRetorno;
                //}

                var oSucDest = from m in oFol.PR_FOL_DEN_SUC_COBERTURA(oBulto.blt.PK_FOL_OTP_ID, oBulto.blt.PK_FOL_OTD_ID)
                               select m;//Sucursal Destino

                var oMan = from man in oFol.TB_FOL_MAN_MANIFIESTO
                           where man.PK_PAR_USU_RUT == GetRutActiveUser()
                           && man.PK_FOL_MAE_ID == 1
                           select man;
                if (oMan.Count() > 0)
                {

                    //if (oSucDest == null)
                    //{
                    //    string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Destino sin cobertura");
                    //    return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                    //}

                    //var oManSingle = oMan.ToList()[0];
                    //var oSucDestSingle = oSucDest.ToList()[0];
                    //var oDestinoMan = oManSingle.TB_FOL_MAD_MAN_DESTINO;
                    //if (oManSingle.PK_FOL_TPM_ID == 2 && oSucDestSingle.PK_PAR_SUC_ID != oValidation.GetSucursalIDfromActiveUser())
                    //{
                    //    string SucursalDestino = oValidation.GetSucursalNamefromIDSucursal(oSucDestSingle.PK_PAR_SUC_ID);
                    //    string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Manifiesto en <strong>Ruta Cliente</strong>, bulto con destino Sucursal <strong>" + SucursalDestino + "</strong");
                    //    return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                    //}
                    //else if (oManSingle.PK_FOL_TPM_ID == 1 && oDestinoMan != null)
                    //{
                    //    var oDestinoManSingle = oDestinoMan.ToList()[0];
                    //    if (oSucDestSingle.PK_PAR_SUC_ID != oDestinoManSingle.PK_PAR_SUC_ID)
                    //    {
                    //        string SucursalManifiesto = oValidation.GetSucursalNamefromIDSucursal(oDestinoManSingle.PK_PAR_SUC_ID);
                    //        string SucursalDestino = oValidation.GetSucursalNamefromIDSucursal(oSucDestSingle.PK_PAR_SUC_ID);

                    //        if (oSucDestSingle.PK_PAR_SUC_ID == oSucursal && oSucursal != oDestinoManSingle.PK_PAR_SUC_ID)
                    //        {
                    //            string suggestedUID = String.Format(CultureInfo.InvariantCulture, string.Format("El Bulto tiene destino <strong>Ruta Cliente</strong>, el manifiesto tiene destino <strong>{0}</strong>", SucursalManifiesto, SucursalDestino));
                    //            return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                    //        }
                    //        if (oDestinoManSingle.PK_PAR_SUC_ID == oSucDestSingle.PK_PAR_SUC_ID)
                    //        {
                    //            string suggestedUID = String.Format(CultureInfo.InvariantCulture, string.Format("El Bulto tiene destino sucursal <strong>{1}</strong>, el manifiesto tiene destino <strong>Ruta Cliente</strong>", SucursalManifiesto, SucursalDestino));
                    //            return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                    //        }
                    //        else
                    //        {
                    //            string suggestedUID = String.Format(CultureInfo.InvariantCulture, string.Format("El Bulto tiene destino sucursal <strong>{1}</strong>, el manifiesto tiene destino <strong>{0}</strong>", SucursalManifiesto, SucursalDestino));
                    //            return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                    //        }
                    //    }
                    //}
                    //else if (oManSingle.PK_FOL_TPM_ID == 1 && oDestinoMan == null)
                    //{
                    //    string suggestedUID = String.Format(CultureInfo.InvariantCulture, string.Format("Manifiesto sin destino"));
                    //    return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                    //}
                }
                else
                {
                    string suggestedUID = String.Format(CultureInfo.InvariantCulture, string.Format("Usuario sin manifiesto abierto"));
                    oRetorno.Ok = false;
                    oRetorno.Mensaje = suggestedUID;
                    return oRetorno;
                }

            }

            oRetorno.Ok = true;


            return oRetorno;

        }
        #endregion

        #region vJSonValidarNumeroDocumentoAsociado

        public JsonResult vJSonValidarNumeroDocumentoAsociado(string Numero, decimal OTP, decimal OTD, string Tipo)
        {
            decimal TDO_ID = 0;
            decimal NUM_DOC = 0;

            if (isDecimal(Tipo))
                TDO_ID=Convert.ToDecimal(Tipo);

            if (isDecimal(Numero))
                NUM_DOC=Convert.ToDecimal(Numero);


            if (TDO_ID > 0 && NUM_DOC > 0)
            {
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

                var oCli = from cli in oFol.TB_FOL_OTP_OT_PRINCIPAL
                           where cli.PK_FOL_OTP_ID == OTP                             
                           select cli;

                var intCli=oCli.ToList()[0].PK_CLI_EMP_RUT;

                var oDatos = from doa in oFol.TB_FOL_DOA_DOC_ASOCIADO
                             where 
                                doa.PK_CLI_EMP_RUT==intCli
                                && doa.PK_PAR_TDO_ID == TDO_ID
                                && doa.PK_FOL_DOA_NUMERO == NUM_DOC
                             select doa;

                if (oDatos.Count() > 0)
                {
                    string suggestedUID = String.Format(CultureInfo.InvariantCulture, "La realación Tipo Documento - N°. Ya Existe para el cliente");
                    return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Número o Tipo Incorrecto");
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion                

        #region vjSonValidarIdBultoRecepcion
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult vjSonValidarIdBultoRecepcion(string strValidationIdBulto)
        {
            RetornoModels oRetorno = new RetornoModels();
            string strIdBulto = strValidationIdBulto;   
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            string strCodBulto;
            decimal decCodBulto = 0;
            if (strIdBulto.Substring(0, 1).ToUpper() == "B")//Es Bulto
            {
                if (strIdBulto.Length > 1)//tiene algo
                {
                    strCodBulto = strIdBulto.Substring(1, strIdBulto.Length - 1);
                    if (IsNumeric(strCodBulto))
                    {
                        decCodBulto = Convert.ToDecimal(strCodBulto);
                    }
                    else//error no es numero
                    {
                        oRetorno.Mensaje = "Código de Bulto incorrecto";
                        oRetorno.Ok = false;
                        return Json(oRetorno, JsonRequestBehavior.AllowGet);
                        //string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Código de Bulto incorrecto");
                        //return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                    }
                }
                else//Error
                {
                    oRetorno.Mensaje = "Código de Bulto incorrecto";
                    oRetorno.Ok = false;
                    return Json(oRetorno, JsonRequestBehavior.AllowGet);
                    //string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Código de Bulto incorrecto");
                    //return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                }
            }
            else if (strIdBulto.Substring(0, 1).ToUpper() == "P")//Es OT
            {
                var oResult = GetCodigoBultoFromCodigoGenerico(strIdBulto);
                if (oResult == 0)
                {
                    oRetorno.Mensaje = "Código de Bulto incorrecto";
                    oRetorno.Ok = false;
                    return Json(oRetorno, JsonRequestBehavior.AllowGet);
                    //string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Código incorrecto");
                    //return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    decCodBulto = oResult;
                }
            }
            else
            {
                //string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Código no existe");
                //return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                oRetorno.Mensaje = "Código no existe";
                oRetorno.Ok = false;
                return Json(oRetorno, JsonRequestBehavior.AllowGet);
            }
            
            var oCodigos = from blt in oFol.TB_FOL_BLT_BULTO
                           join est in oFol.TB_FOL_EST_ESTADO on blt.PK_FOL_EST_ID equals est.PK_FOL_EST_ID
                           where
                           blt.PK_FOL_BLT_ID == decCodBulto
                           select new
                           {
                               blt.PK_FOL_EST_ID,
                               est.FL_FOL_EST_NOMBRE
                           };

            int countBultos = oCodigos.Count();


            if (countBultos == 0)
            {
                oRetorno.Mensaje = "Código de bulto no existe";
                oRetorno.Ok = false;
                return Json(oRetorno, JsonRequestBehavior.AllowGet);
                //string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Código de bulto no existe");
                //return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var oBulto = oCodigos.ToList()[0];

                var oHabilitadoRecepcion = from ere in oFol.TB_FOL_ERE_ESTADO_RECEPCION
                                           select ere;
                
                if (oHabilitadoRecepcion.Where(m=>m.PK_FOL_EST_ID==oBulto.PK_FOL_EST_ID).Count()>0)
                {
                    oRetorno.Ok = true;
                    oRetorno.Bulto = decCodBulto;
                    return Json(oRetorno, JsonRequestBehavior.AllowGet);                    
                }
                else
                {
                    oRetorno.Mensaje = String.Format("Bulto se encuentra en estado: {0}",oBulto.FL_FOL_EST_NOMBRE);
                    oRetorno.Ok = false;
                    return Json(oRetorno, JsonRequestBehavior.AllowGet);
                    //string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Bulto se encuentra en estado: " + oBulto.FL_FOL_EST_NOMBRE);
                    //return Json(suggestedUID, JsonRequestBehavior.AllowGet);                    
                }
            }            

        }
        #endregion       

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
                    oResult = new GetOTPOTD { OTP=0,OTD=0,Error=1,ErrorMensaje="Número Incorrecto" };
                    return oResult;
                }
            }
            else
            {
                oResult = new GetOTPOTD { OTP=0,OTD=0,Error=1,ErrorMensaje="Número Incorrecto" };
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

        #region vJsonValidarCodigoAnular
        public JsonResult vJsonValidarCodigoAnular(string OT)
        {

            var oResult = TransformCodigoGenericoOTPOTD(OT);

            if (oResult.Error == 0)
                return Json(true, JsonRequestBehavior.AllowGet);
            else
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, oResult.ErrorMensaje);
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region UltimoMovimientoUsuario
        [HttpPost]
        public string UltimoMovimientoUsuario()
        {
            try
            {
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                var oResult = oFol.PR_FOL_ULTIMO_MOVIMIENTO(GetRutActiveUser());
                string oRut = "";
                


                foreach(var oFila in oResult)
                    oRut = oFila.PK_CLI_EMP_RUT.ToString() + "-" + oFila.FL_CLI_EMP_DV.ToString();                
                return oRut;
            }
            catch (Exception e)
            {
                return "Error:" + e.Message;
            }
        }
        #endregion

        #region GetNombreReferencia
        public class Referencias
        {
            public string NombreRef1 { get; set; }
            public string NombreRef2 { get; set; }
        }
        public Referencias GetNombreReferencia(decimal Servicio)
        {
            string NombreRef1;
            string NombreRef2;

            try
            {                

                LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

                var oDatos = from x in oPar.TB_PAR_RES_REFERENCIA_SERVICIO
                             join y in oPar.TB_PAR_TDO_TIPO_DOCUMENTO on x.PK_PAR_TDO_ID equals y.PK_PAR_TDO_ID
                             where x.PK_PAR_SER_ID == Servicio
                             select new
                             {
                                 x.PK_PAR_REF_ID,
                                 y.FL_PAR_TDO_NOMBRE
                             };

                NombreRef1 = "";
                NombreRef2 = "";

                foreach (var fila in oDatos)
                {
                    if (fila.PK_PAR_REF_ID == 1)//Referencia 1
                        NombreRef1 = fila.FL_PAR_TDO_NOMBRE;
                    else if (fila.PK_PAR_REF_ID == 2)//Referencia 2
                        NombreRef2 = fila.FL_PAR_TDO_NOMBRE;
                }
            }
            catch
            {
                NombreRef1 = "";
                NombreRef2 = "";
            }

            return new Referencias
            {
                NombreRef1 = NombreRef1,
                NombreRef2 = NombreRef2
            };

        }
        #endregion

        #region GetNombreServicio
        public string GetNombreServicio(int idServicio)
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oDatos = from x in oPar.TB_PAR_SER_SERVICIO
                         where x.PK_PAR_SER_ID == Convert.ToInt32(idServicio)
                         select x;
            foreach (var oFila in oDatos)
            {
                return oFila.FL_PAR_SER_NOMBRE;
            }

            return "";
        }
        #endregion

        #region vJsonValidarDocumentoOk
        public JsonResult vJsonValidarDocumentoOk(string ValidationNumeroDocumento)
        {

            var oResult = TransformCodigoGenericoOTPOTD(ValidationNumeroDocumento);

            if (oResult.Error == 0)//Es un OT
            {
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                var oDatos = from otd in oFol.TB_FOL_OTD_OT_DETALLE
                             join dar in oFol.TB_FOL_DAR_DOC_RECEPCION on otd.PK_FOL_EST_ID equals dar.PK_FOL_EST_ID
                             where otd.PK_FOL_OTP_ID == oResult.OTP
                             && otd.PK_FOL_OTD_ID == oResult.OTD
                             select otd;
                if (oDatos.Count() == 0)
                {
                    string suggestedUID = String.Format(CultureInfo.InvariantCulture,"La OT no se encuentra en un estado habilitado para la recepción de documentos.");
                    return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                }

                var oDOA = from doa in oFol.TB_FOL_DOA_DOC_ASOCIADO
                             where doa.PK_FOL_OTP_ID == oResult.OTP
                             && doa.PK_FOL_OTD_ID == oResult.OTD
                             select doa;

                if (oDOA.Count() == 0)
                {
                    string suggestedUID = String.Format(CultureInfo.InvariantCulture, "OT sin documentos asociados.");
                    return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                }

                var oEstado = from pr in oFol.PR_FOL_DOA_ESTADO(oResult.OTP, oResult.OTD)
                              select pr;

                int c = 1;                
                foreach (var oFila in oEstado)
                {
                    if (c == 1)
                    {
                        
                        if (oFila.PK_FOL_EDO_ID == 4)
                        {
                            string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Documento ya se encuentra <strong>Rendido</strong>. Ingresado en sucursal " + oFila.FL_PAR_SUC_NOMBRE + ", Fecha:" + oFila.FL_FOL_RDO_FECHA + ", Usuario:" + oFila.Usuario);
                            return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                        }
                        else if (oFila.PK_FOL_EDO_ID == 2)
                        {                            
                            string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Documento ya se encuentra ingresado en Sucursal <strong>" + oFila.FL_PAR_SUC_NOMBRE + "</strong>");
                            return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                        }                        
                    }
                    c++;
                }

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, oResult.ErrorMensaje);
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region vJsonValidarDocumentoOkSalida
        public JsonResult vJsonValidarDocumentoOkSalida(string ValidationNumeroDocumentoSalida)
        {

            var oResult = TransformCodigoGenericoOTPOTD(ValidationNumeroDocumentoSalida);

            if (oResult.Error == 0)//Es un OT
            {
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();


                var oDOA = from doa in oFol.TB_FOL_DOA_DOC_ASOCIADO
                           where doa.PK_FOL_OTP_ID == oResult.OTP
                           && doa.PK_FOL_OTD_ID == oResult.OTD
                           select doa;

                if (oDOA.Count() == 0)
                {
                    string suggestedUID = String.Format(CultureInfo.InvariantCulture, "OT sin documentos asociados.");
                    return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                }

                var oEstado = from pr in oFol.PR_FOL_DOA_ESTADO(oResult.OTP, oResult.OTD)
                              select pr;

                int c = 1;
                int oSucursal = GetSucursalIDfromActiveUser();
                foreach (var oFila in oEstado)
                {
                    if (c == 1)
                    {
                        if (oFila.PK_FOL_EDO_ID == 3)
                        {
                            string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Documento se encuentra <strong>En Ruta</strong>. Ingresado en sucursal " + oFila.FL_PAR_SUC_NOMBRE + ", Fecha:" + oFila.FL_FOL_RDO_FECHA + ", Usuario:" + oFila.Usuario);
                            return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                        }
                        else if (oFila.PK_FOL_EDO_ID == 4)
                        {
                            string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Documento ya se encuentra <strong>Rendido</strong>. Ingresado en sucursal " + oFila.FL_PAR_SUC_NOMBRE + ", Fecha:" + oFila.FL_FOL_RDO_FECHA + ", Usuario:" + oFila.Usuario);
                            return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                        }
                        else if (oFila.PK_FOL_EDO_ID == 2 && oFila.PK_PAR_SUC_ID != oSucursal)
                        {
                            string nSucursal = GetSucursalNamefromIDSucursal(oSucursal);
                            string suggestedUID = String.Format(CultureInfo.InvariantCulture, "OT sin documentos ingresados en sucursal <strong>" + nSucursal + "</strong>");
                            return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                        }
                        else if (oFila.PK_FOL_EDO_ID == 1)
                        {
                            string nSucursal = GetSucursalNamefromIDSucursal(oSucursal);
                            string suggestedUID = String.Format(CultureInfo.InvariantCulture, "OT sin documentos ingresados en sucursal <strong>" + nSucursal + "</strong>");
                            return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                
                
                

                var oDatos = from otd in oFol.TB_FOL_OTD_OT_DETALLE
                             join dar in oFol.TB_FOL_DAR_DOC_RECEPCION on otd.PK_FOL_EST_ID equals dar.PK_FOL_EST_ID
                             where otd.PK_FOL_OTP_ID == oResult.OTP
                             && otd.PK_FOL_OTD_ID == oResult.OTD
                             select otd;
                if (oDatos.Count() == 0)
                {
                    string suggestedUID = String.Format(CultureInfo.InvariantCulture, "La OT no se encuentra en un estado habilitado para la salida de documentos.");
                    return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, oResult.ErrorMensaje);
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region vJsonValidarDocumentoOkRendicion
        public JsonResult vJsonValidarDocumentoOkRendicion(string ValidationNumeroDocumentoRendicion)
        {

            var oResult = TransformCodigoGenericoOTPOTD(ValidationNumeroDocumentoRendicion);

            if (oResult.Error == 0)//Es un OT
            {
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();


                var oDOA = from doa in oFol.TB_FOL_DOA_DOC_ASOCIADO
                           where doa.PK_FOL_OTP_ID == oResult.OTP
                           && doa.PK_FOL_OTD_ID == oResult.OTD
                           select doa;

                if (oDOA.Count() == 0)
                {
                    string suggestedUID = String.Format(CultureInfo.InvariantCulture, "OT sin documentos asociados.");
                    return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                }

                var oEstado = from pr in oFol.PR_FOL_DOA_ESTADO(oResult.OTP, oResult.OTD)
                              select pr;

                int c = 1;        
                int oSucursal = GetSucursalIDfromActiveUser();
                foreach (var oFila in oEstado)
                {
                    if (c == 1)
                    {
                        if (oFila.PK_FOL_EDO_ID == 3)
                        {
                            string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Documento se encuentra <strong>En Ruta</strong>. Ingresado en sucursal " + oFila.FL_PAR_SUC_NOMBRE + ", Fecha:" + oFila.FL_FOL_RDO_FECHA + ", Usuario:" + oFila.Usuario);
                            return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                        }
                        else if (oFila.PK_FOL_EDO_ID == 4)
                        {
                            string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Documento ya se encuentra <strong>Rendido</strong>. Ingresado en sucursal " + oFila.FL_PAR_SUC_NOMBRE + ", Fecha:" + oFila.FL_FOL_RDO_FECHA + ", Usuario:" + oFila.Usuario);
                            return Json(suggestedUID, JsonRequestBehavior.AllowGet); 
                        }
                        else if (oFila.PK_FOL_EDO_ID == 2 && oFila.PK_PAR_SUC_ID!=oSucursal)
                        {
                            string nSucursal = GetSucursalNamefromIDSucursal(oSucursal);
                            string suggestedUID = String.Format(CultureInfo.InvariantCulture, "OT sin documentos ingresados en sucursal <strong>" + nSucursal + "</strong>");
                            return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                        }
                        else if (oFila.PK_FOL_EDO_ID ==1)
                        {
                            string nSucursal = GetSucursalNamefromIDSucursal(oSucursal);
                            string suggestedUID = String.Format(CultureInfo.InvariantCulture, "OT sin documentos ingresados en sucursal <strong>" + nSucursal + "</strong>");
                            return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                        }
                    }                  
                }

               


                var oDatos = from otd in oFol.TB_FOL_OTD_OT_DETALLE
                             join dar in oFol.TB_FOL_DAR_DOC_RECEPCION on otd.PK_FOL_EST_ID equals dar.PK_FOL_EST_ID
                             where otd.PK_FOL_OTP_ID == oResult.OTP
                             && otd.PK_FOL_OTD_ID == oResult.OTD
                             select otd;

                if (oDatos.Count() == 0)
                {
                    string suggestedUID = String.Format(CultureInfo.InvariantCulture, "La OT no se encuentra en un estado habilitado para la rendición de documentos.");
                    return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, oResult.ErrorMensaje);
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region jSonValidarDocumentoBuscar
        public JsonResult jSonValidarDocumentoBuscar(string ValidarDocumentoBuscar)
        {

            var oResult = TransformCodigoGenericoOTPOTD(ValidarDocumentoBuscar);

            if (oResult.Error == 0)//Es un OT
            {
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();


                var oDOA = from doa in oFol.TB_FOL_DOA_DOC_ASOCIADO
                           where doa.PK_FOL_OTP_ID == oResult.OTP
                           && doa.PK_FOL_OTD_ID == oResult.OTD
                           select doa;

                if (oDOA.Count() == 0)
                {
                    string suggestedUID = String.Format(CultureInfo.InvariantCulture, "OT sin documentos asociados.");
                    return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                }               
               
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, oResult.ErrorMensaje);
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region GetNombreServicioFromIdServicio
        public string GetNombreServicioFromIdServicio(int SER_ID)
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oDatos = from ser in oPar.TB_PAR_SER_SERVICIO
                         where ser.PK_PAR_SER_ID == SER_ID
                         select ser;
            if (oDatos.Count() > 0)
            {
                return oDatos.ToList()[0].FL_PAR_SER_NOMBRE;
            }
            else
            {
                return "";
            }
        }
        #endregion

        #region jSonValidarEstadoOT
        public JsonResult jSonValidarEstadoOT(string OT)
        {
            var oDatos = TransformCodigoGenericoOTPOTD(OT);
            if (oDatos.Error == 0)
            {
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                var oOTD = (from otd in oFol.TB_FOL_OTD_OT_DETALLE
                           where otd.PK_FOL_OTP_ID == oDatos.OTP
                           && otd.PK_FOL_OTD_ID == oDatos.OTD
                           select otd).Single();

                if (oOTD.PK_FOL_EST_ID == 8 || oOTD.PK_FOL_EST_ID == 10)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string suggestedUID = String.Format(CultureInfo.InvariantCulture, string.Format("La OT se debe encontrar en estado: Recepcionado OK, Rechazo Total"));
                    return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, string.Format("Se ha encontrado un error: {0}", oDatos.ErrorMensaje));
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region ValidarUsuarioSesionFunction
        public string ValidarUsuarioSesionFunction(string UserName)
        {
            int oResult = ValidarRut(UserName);
            int Rut = 0;
            if (oResult == 1)
            {
                Rut = Convert.ToInt32(UserName.Substring(0, UserName.Length - 2));

                LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

                var oTokenBD = from pr in oPar.PR_PAR_USU_LOGIN(Rut, 1)
                               select pr;

                var oTokenList = oTokenBD.ToList();



                if (oTokenList.Count() > 0)
                {
                    return oTokenList[0].PK_PAR_TOK_ID.ToString();
                }
                else
                {
                    return "";
                    //var NewToken = (from pr in oPar.PR_PAR_USU_LOGIN_INSERT(Rut, 1)
                    //                select pr).Single();

                    //return NewToken.PK_PAR_TOK_ID.ToString();;
                }
            }
            else
            {
                return "";
            }
        }
        public JsonResult ValidarUsuarioSesion(string UserName)
        {
            int oResult = ValidarRut(UserName);
            if (oResult != 1)
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, string.Format("Rut Incorrecto"));
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            int Rut = Convert.ToInt32(UserName.Substring(0, UserName.Length - 2));
            var oTokenBD = from pr in oPar.PR_PAR_USU_LOGIN(Rut, 1)
                           select pr;

            var oTokenList = oTokenBD.ToList();



            if (oTokenList.Count() > 0)
            {
                if (Session == null)
                {
                    string suggestedUID = String.Format(CultureInfo.InvariantCulture, string.Format("Usuario conectado, no se admiten ingresos duplicados, intente más tarde"));
                    return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                }
                if (Session["Token"] == null)
                {
                    string suggestedUID = String.Format(CultureInfo.InvariantCulture, string.Format("Usuario conectado, no se admiten ingresos duplicados, intente más tarde"));
                    return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                }
                if (Session["Token"].ToString() != oTokenList[0].PK_PAR_TOK_ID.ToString())
                {
                    string suggestedUID = String.Format(CultureInfo.InvariantCulture, string.Format("Usuario conectado, no se admiten ingresos duplicados, intente más tarde"));
                    return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
        public string CrearToken()
        {
            try
            {
                int Rut = GetRutActiveUser();
                LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

                var NewToken = (from pr in oPar.PR_PAR_USU_LOGIN_INSERT(Rut, 1)
                                select pr).Single();

                return NewToken.PK_PAR_TOK_ID.ToString(); ;
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region JsonValidaManifiestoDocumento
        public JsonResult JsonValidaManifiestoDocumento(string CodBuscarManifiesto)
        {
            ValidationController oValidation = new ValidationController();
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();


            if (oValidation.isDecimal(CodBuscarManifiesto) == true)
            {
                var oDatos = from mdo in oFol.TB_FOL_MDO_MANIFIESTO_DOCUMENTO
                             where mdo.PK_FOL_MDO_ID == Convert.ToDecimal(CodBuscarManifiesto)
                             select mdo;

                if (oDatos.Count() > 0)//Existe
                {

                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string suggestedUID = String.Format(CultureInfo.InvariantCulture, "Manifiesto no existe");
                    return Json(suggestedUID, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                string suggestedUID = String.Format(CultureInfo.InvariantCulture, "N° Manifiesto Incorrecto");
                return Json(suggestedUID, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region GetListaOrigenDestino
        public class clsListaDestino
        {
            public int Orden { get; set; }//1 el primero
            public int SUC_DESTINO { get; set; }
            public string Descripcion { get; set; }
            public string Ruta { get; set; }
        }

        public List<clsListaDestino> GetListaOrigenDestino(int SucursalOrigen, int ComunaDestino, int Via)
        {
            //Origen y Destino, se refiere al SUC_ID
            List<clsListaDestino> oLista = new List<clsListaDestino>();
            int SucursalCobertura = ValidaConCobertura(SucursalOrigen, ComunaDestino,Via);
            oLista.Add(new clsListaDestino
            {
                Descripcion = "Origen",
                Orden = 0,
                Ruta = "0",
                SUC_DESTINO = SucursalCobertura
            });
                oLista.AddRange(fnOrigenDestino(0, SucursalOrigen, SucursalCobertura, SucursalCobertura, "0.1", Via));

            List<clsListaDestino> oListaR = new List<clsListaDestino>();
            string oRuta = "";
            foreach (var oFila in oLista)
            {
                if (oFila.Ruta.Length > 3)
                {
                    if (oFila.Ruta.Substring(oFila.Ruta.Length - 3, 3) == "Fin")
                    {
                        oRuta = oFila.Ruta;
                        oListaR.Add(new clsListaDestino
                        {
                            Descripcion = "Origen",
                            Orden = 0,
                            Ruta = "",
                            SUC_DESTINO = oFila.SUC_DESTINO
                        });
                    }
                }
            }
            var oDatos = oRuta.Split('.');
            int Niveles = oDatos.Count() - 2;
            int Cont = 0;
            string Valor = "Destino Final";
            string NivelActual = "";
            while (Cont < Niveles)
            {
                NivelActual = NivelActual + oDatos[Cont];
                if (Cont > 0)
                {
                    Valor = "Transito";
                }
                foreach (var oA in oLista.Where(m => m.Ruta == NivelActual))
                {
                    oListaR.Add(new clsListaDestino
                    {
                        Descripcion = Valor,
                        Orden = 0,
                        Ruta = "",
                        SUC_DESTINO = oA.SUC_DESTINO
                    }); ;
                }
                NivelActual = NivelActual + ".";
                Cont += 1;
            }


            return oListaR;
        }
        public List<clsListaDestino> fnOrigenDestino(int SucursalAnterior, int SucursalOrigen, int SucursalCobertura, int DondeVoy, string Ruta, int Via)
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            List<clsListaDestino> oLista = new List<clsListaDestino>();
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            if (SucursalOrigen == DondeVoy)
            {
                oLista.Add(new clsListaDestino
                {
                    Orden = 0,
                    SUC_DESTINO = DondeVoy,
                    Descripcion = "Destino Final",
                    Ruta = Ruta + ".Fin"
                });
                return oLista;
            }


            //var oDatos = from cdd in oPar.TB_PAR_CDD_CDI_DES
            //             where cdd.PK_PAR_DES_ID == DondeVoy
            //             && cdd.PK_PAR_CDI_ID != SucursalAnterior
            //             && cdd.PK_PAR_VIA_ID==Via                         
            //             select cdd;

            var oDatos = from pr in oFol.PR_FOL_CONSULTA_SUCURSALES(DondeVoy,Via,SucursalAnterior)
                         select pr;

            //if (oDatos.Where(m => m.PK_PAR_DES_ID == SucursalCobertura).Count() > 0)
            //    oDatos = oDatos.Where(m => m.PK_PAR_DES_ID == SucursalCobertura);
            

            var oListaProcesada = oDatos.ToList();

            int iRuta = 1;

            foreach (var oFila in oListaProcesada)
            {
                if (oLista.Where(m => m.Descripcion == "Destino Final").Count() == 0)
                {
                    if (oFila.PK_PAR_CDI_ID == SucursalOrigen)
                    {
                        oLista.Add(new clsListaDestino
                        {
                            Orden = 0,
                            SUC_DESTINO = oFila.PK_PAR_CDI_ID,
                            Descripcion = "Destino Final",
                            Ruta = Ruta + ".Fin"
                        });
                        return oLista;
                    }
                    else if (oFila.PK_PAR_DES_ID != SucursalAnterior)
                    {
                        oLista.Add(new clsListaDestino
                        {
                            Orden = 0,
                            SUC_DESTINO = oFila.PK_PAR_CDI_ID,
                            Descripcion = "Transito",
                            Ruta = Ruta
                        });
                        oLista.AddRange(fnOrigenDestino(oFila.PK_PAR_DES_ID, SucursalOrigen, SucursalCobertura, oFila.PK_PAR_CDI_ID, Ruta + "." + iRuta.ToString(),Via));
                    }
                    iRuta = iRuta + 1;
                }
            }
            if (oLista.Where(m => m.Descripcion == "Destino Final").Count() == 0)
            {
                List<clsListaDestino> oLista2 = new List<clsListaDestino>();
                return oLista2;
            }
            else
            {
                return oLista;
            }
        }

        public int ValidaConCobertura(int SucursalOrigen, int ComunaDestino, int Via_Id)
        {
            try
            {
                LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
                var oDatos = (from cob in oPar.TB_PAR_COB_COBERTURA_COMUNA
                              where cob.PK_PAR_COM_ID == ComunaDestino
                              //&& cob.PK_PAR_VIA_ID==Via_Id-- ver por que hice esto...
                              select cob).Single();
                return oDatos.PK_PAR_SUC_ID;
            }
            catch(Exception e)
            {
                return 0;
            }            
            
        }
        public int fnSucDestinoManifiesto(List<clsListaDestino> oListaDestinos, int Tra_id, int Via_id)
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            var oDET = from det in oPar.TB_PAR_DET_DESTINO_TRANSPORTE
                       where det.PK_PAR_TRA_ID == Tra_id
                       && det.PK_PAR_VIA_ID == Via_id
                       select det;

            int SucDestMan = 0;

            foreach (var oFila in oListaDestinos)
            {
                foreach (var oFilaDet in oDET)
                {
                    if (oFilaDet.PK_PAR_SUC_ID == oFila.SUC_DESTINO)
                    {
                        SucDestMan=oFilaDet.PK_PAR_SUC_ID;
                    }
                }
            }

            return SucDestMan;
        }
        #endregion

        #region ValidaBultoSiguiente
        [Authorize][HttpPost]
        public ActionResult ValidaBultoSiguiente(decimal OTP, decimal OTD)
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            RetornoModels oRetorno = new RetornoModels();
            var oDatos = from otd in oFol.TB_FOL_BLT_BULTO
                         where otd.PK_FOL_OTP_ID == OTP && otd.PK_FOL_OTD_ID == OTD
                         && (otd.PK_FOL_EST_ID==11 || otd.PK_FOL_EST_ID==21)
                         orderby otd.PK_FOL_BLT_ID
                         select otd;
            if (oDatos.Count() == 0)
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = "No quedan bultos por ingresar";
            }
            else
            {
                oRetorno.Ok = true;
                oRetorno.Bulto = oDatos.ToList()[0].PK_FOL_BLT_ID;
            }

            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }
        #endregion
        public string GetNombreImpresora(int Rut)
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            try
            {
                var oNombre = (from x in oPar.TB_PAR_USU_USUARIO
                               where x.PK_PAR_USU_RUT == Rut
                               select x).Single();
                return oNombre.FL_PAR_USU_NOMBRE_IMPRESORA;
            }
            catch
            {
                return "";
            }
        }

        #region ValidaOT
        public JsonResult ValiadOT(string OT)
        {
            var oResult = TransformCodigoGenericoOTPOTD(OT);
            RetornoModels oRetorno = new RetornoModels();
            if (oResult.Error == 0)
            {
                oRetorno.Ok = true;
            }
            else
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = oResult.ErrorMensaje;
            }
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region TieneRolByName
        public bool TieneRolByName(string RolName)
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oQry = (from pr in oPar.PR_PAR_TIENE_ROL_BY_NAME(GetRutActiveUser(), RolName)
                       select pr).Single();

            if (oQry.TIENE_ROL == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion
    }
}
