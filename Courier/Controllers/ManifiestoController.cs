using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Courier.Models;

using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;

using Newtonsoft.Json;
using RestSharp;


  public class RetornoWs
    {
        public string success { get; set; }
        public string data { get; set; }
        public string msg { get; set; }
    }

namespace Courier.Controllers
{
  

    public class ManifiestoController : Controller
    {
        //
        // GET: /Manifiesto/

        [Authorize]
        public ActionResult Index()
        {
            ValidationController oValidation = new ValidationController();
            List<SelectListItem> BlankListaConductor= new List<SelectListItem>();
            ManifiestoModels oModel = new ManifiestoModels
            {
                SucursalActual = oValidation.GetSucursalIDfromActiveUser(),
                oListaConductor = BlankListaConductor
            };
            oModel.GetDatosListaDestino();
            oModel.GetListaTransportes();
            return View(oModel);
        }
        public ActionResult ListaDestino(ManifiestoModels oModel)
        {
            ValidationController oValidation = new ValidationController();

            oModel.SucursalActual = oValidation.GetSucursalIDfromActiveUser();
            oModel.GetDatosListaDestino();            
                        
            return PartialView("_ListaDestino", oModel);
        }

        #region BuscarManifiesto
        public ActionResult BuscarManifiesto(ManifiestoModels oModel)
        {
            ValidationController oValidation = new ValidationController();
            if (oValidation.isDecimal(oModel.ManifiestoPadre) == true)
            {
                oModel.GetListaManifiestoMap();
            }
            return PartialView("BuscarManifiesto", oModel);
        }
        public ActionResult EnviaManifiesto(ManifiestoModels oModel)
        {
            return PartialView("EnviaManifiesto", oModel);
        }

        private static string ConcatParams(string id_manifiesto)
        {
            StringBuilder Parametros = new StringBuilder();
            Parametros.Append("id_manifiesto=" + id_manifiesto);

            return Parametros == null ? string.Empty : Parametros.ToString();
        }


        [Authorize]
        private string GetPOSTResponse(string parametro_manifiesto)
        {
            string keyToken = "051de32597041e41f73b97d61c67a13b";
            string requestUrl = "http://app2.wspexpress.cl:8080/fusion/app/manifiesto";

            try
            {
                var client = new RestClient(requestUrl);

                var request = new RestRequest("resource/{id}", Method.POST);
                request.AddParameter("id_manifiesto", parametro_manifiesto);
                request.AddUrlSegment("id", "123");
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("X-AUTH-TOKEN", keyToken);

                // execute the request
                IRestResponse response = client.Execute(request);
                var content = response.Content;

                return content;

            }
            catch (System.Web.HttpException ex)
            {
                if (ex.ErrorCode == 404)
                    throw new Exception("No fue posible conectar con el servicio remoto.");
                else throw ex;
            }
        }
        #endregion

         [HttpPost]
        public ActionResult EnviarManifiesto(ManifiestoModels mModel)
        {

            Clases.Retorno oRetorno = new Clases.Retorno();

       
            try
            
            {
                Courier.wsFolcourier.wsFolCourierSoapClient ws = new Courier.wsFolcourier.wsFolCourierSoapClient();
               var retornado = ws.wmValidarManifiesto("k1we3ttr256sha19k201123lkoji98k1", int.Parse(mModel.IDMANIFIESTO));
                if (retornado.Estado)
                {
                    string retorno = GetPOSTResponse(mModel.IDMANIFIESTO);
 
                    var aux = retorno.Split(',');
                    string validar = aux[1];
                    int largo = validar.Length;
 
                    if (largo == 8)
                    {
                        oRetorno.Mensaje = "El código ingresado ya existe en el proveedor.";
                    }
                    else
                    {
                        oRetorno.Mensaje = "Código validado! Recepcionado por proveedor con ID: " + validar.Substring(15, (largo - 17));
                    }
 
                }
                else
                {
                    oRetorno.Mensaje = "El código ingresado no corresponde a un manifiesto habilitado.";
                }
            }
                catch (Exception ex)
            {
                oRetorno.Mensaje = ex.Message;
                oRetorno.Ok = false;
                //throw;
            } 
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
            
           
        }


         [HttpPost]
         public ActionResult EntregarManifiestos(ManifiestoModels mModel)
         {

             Clases.Retorno oRetorno = new Clases.Retorno();


             try
             {
                 Courier.wsFolcourier.wsFolCourierSoapClient ws = new Courier.wsFolcourier.wsFolCourierSoapClient();
                 var retornado = ws.wmValidarManifiesto("k1we3ttr256sha19k201123lkoji98k1", int.Parse(mModel.IDMANIFIESTO));
                 if (retornado.Estado)
                 {
                     string retorno = GetPOSTResponse(mModel.IDMANIFIESTO);

                     var aux = retorno.Split(',');
                     string validar = aux[1];
                     int largo = validar.Length;

                     if (largo == 8)
                     {
                         oRetorno.Mensaje = "El código ingresado ya existe en el proveedor.";
                     }
                     else
                     {
                         oRetorno.Mensaje = "Código validado! Recepcionado por proveedor con ID: " + validar.Substring(15, (largo - 17));
                     }

                 }
                 else
                 {
                     oRetorno.Mensaje = "El código ingresado no corresponde a un manifiesto habilitado.";
                 }
             }
             catch (Exception ex)
             {
                 oRetorno.Mensaje = ex.Message;
                 oRetorno.Ok = false;
                 //throw;
             }
             return Json(oRetorno, JsonRequestBehavior.AllowGet);


         }

        [Authorize]
        public ActionResult GuardaManifiestoPadre(FormCollection oForm)
        {
            RetornoModels oRetorno = new RetornoModels();
            try
            {
                var Sucursal_Destino = oForm["CheckDestino"];
                var Conductor = Convert.ToInt32(oForm["Conductor"]);
                var Transporte = Convert.ToInt32(oForm["Transporte"]);

                ValidationController oValidation = new ValidationController();
                ManifiestoModels oModel = new ManifiestoModels();
                oModel.SucursalActual = oValidation.GetSucursalIDfromActiveUser();
                oModel.Transporte = Transporte.ToString() ;
                oModel.GetDatosListaDestino();
                var DestinosValidos = Sucursal_Destino.Split(',');

                List<TB_FOL_MAN_MANIFIESTO> oListaManifiesto = new List<TB_FOL_MAN_MANIFIESTO>();
                System.Data.Linq.EntitySet<TB_FOL_MMR_MAN_MAP_RELACION> oListaRelacion = new System.Data.Linq.EntitySet<TB_FOL_MMR_MAN_MAP_RELACION>();


                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

                foreach (var oElemento in oModel.oListaDestino)
                {
                    foreach (var intID in DestinosValidos)
                    {
                        if (oValidation.IsNumeric2(intID.Substring(1,intID.Length-1)))
                        {
                            int oVia = 1;
                            if (intID.Substring(0, 1) == "A")
                                oVia = 2;
                            if (Convert.ToInt32(intID.Substring(1,intID.Length-1)) == oElemento.Sucursal_Bultos_ID && oElemento.Via_ID==oVia)
                            {
                                TB_FOL_MAD_MAN_DESTINO oMad = new TB_FOL_MAD_MAN_DESTINO
                                {
                                    PK_PAR_SUC_ID = oElemento.Sucursal_Destino_ID
                                };

                                System.Data.Linq.EntitySet<TB_FOL_MAD_MAN_DESTINO> oListaMad = new System.Data.Linq.EntitySet<TB_FOL_MAD_MAN_DESTINO>();
                                oListaMad.Add(oMad);
                                                               

                                var oMab = from mab in oFol.PR_FOL_MAN_DESTINO_RUTA_TRONCAL_BLT(oModel.SucursalActual, Convert.ToInt32(intID.Substring(1,intID.Length-1)))
                                           where mab.PK_PAR_VIA_ID==oVia
                                           select mab.PK_FOL_BLT_ID;

                                System.Data.Linq.EntitySet<TB_FOL_MAB_MANIFIESTO_BULTO> oListaBulto = new System.Data.Linq.EntitySet<TB_FOL_MAB_MANIFIESTO_BULTO>();

                                foreach (var oFileMab in oMab)
                                {
                                    oListaBulto.Add(new TB_FOL_MAB_MANIFIESTO_BULTO
                                    {
                                        FL_FOL_MAB_OK = true,
                                        PK_FOL_BLT_ID = oFileMab
                                    });
                                }

                                System.Data.Linq.EntitySet<TB_FOL_MDB_MAN_DESTINO_BULTO> oListaMDB = new System.Data.Linq.EntitySet<TB_FOL_MDB_MAN_DESTINO_BULTO>();

                                oListaMDB.Add(new TB_FOL_MDB_MAN_DESTINO_BULTO {
                                    PK_PAR_SUC_ID = oElemento.Sucursal_Bultos_ID
                                });


                                TB_FOL_MAN_MANIFIESTO oMan = new TB_FOL_MAN_MANIFIESTO
                                {
                                    FL_FOL_MAN_FECHA_CREACION = DateTime.Now,
                                    PK_FOL_MAE_ID = 2,
                                    PK_FOL_TPM_ID = 1,
                                    PK_PAR_USU_RUT = oValidation.GetRutActiveUser(),
                                    PK_PAR_CON_RUT = Conductor,
                                    PK_PAR_SUC_ID = oValidation.GetSucursalIDfromActiveUser(),
                                    PK_PAR_TRA_ID = Transporte,
                                    TB_FOL_MAD_MAN_DESTINO = oListaMad,
                                    TB_FOL_MAB_MANIFIESTO_BULTO = oListaBulto,
                                    TB_FOL_MDB_MAN_DESTINO_BULTO = oListaMDB,
                                    PK_FOL_ORI_ID=1
                                };

                                oListaRelacion.Add(new TB_FOL_MMR_MAN_MAP_RELACION
                                {
                                    TB_FOL_MAN_MANIFIESTO = oMan
                                });
                            }

                        }
                    }
                }

                TB_FOL_MAP_MANIFIESTO_PADRE oMAP = new TB_FOL_MAP_MANIFIESTO_PADRE();
                if (oListaRelacion.Count() > 0)
                {
                    oMAP.FL_FOL_MAP_FECHA = DateTime.Now;
                    oMAP.PK_PAR_CON_RUT = Conductor;
                    oMAP.PK_PAR_TRA_ID = Transporte;
                    oMAP.PK_PAR_USU_RUT = oValidation.GetRutActiveUser();
                    oMAP.TB_FOL_MMR_MAN_MAP_RELACION = oListaRelacion;

                    oFol.TB_FOL_MAP_MANIFIESTO_PADRE.InsertOnSubmit(oMAP);
                    oFol.SubmitChanges();
                    oFol.PR_FOL_MAN_DESTINO_RUTA_TRONCAL_UPT(oMAP.PK_FOL_MAP_ID, oValidation.GetRutActiveUser(), oModel.SucursalActual);
                    oRetorno.OTP = oMAP.PK_FOL_MAP_ID;//en otp dejo el valor de MAP
                    oRetorno.Ok = true;
                }
                else
                {
                    oRetorno.Ok = false;
                    oRetorno.Mensaje = "Sin Manifiestos por Enviar";

                }
               
            }
            catch (Exception e)
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = string.Format("No fue posible guardar la información: {0}", e.Message);
            }
            return Json(oRetorno,JsonRequestBehavior.AllowGet);
        }
    }
}
