using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Courier.Models;
using System.Net;
using System.Xml.Linq;


namespace Courier.Controllers
{
    public class ProveedorController : Controller
    {

        #region AgregarTotalBultos
        [Authorize]
        public ActionResult AgregarTotalBultos(decimal eOTP, decimal eOTD, decimal txtAgregarBulto)
        {
            PreparacionModels oModel = new PreparacionModels();
            ValidationController oVaildation = new ValidationController();

            var oSucursal = oVaildation.Sucursal(User.Identity.Name.ToString());


            oModel.OTP = eOTP;
            oModel.OTD = eOTD;
            oModel.txtAgregarBulto = txtAgregarBulto;
            //oModel.AddTotalBultos();
            oModel.AddBultosOTD(oSucursal.Tipo);
            oModel.RecalcularTotal();
            oModel.GetDatosByOT();
            oModel.GetListaBultos();
            oModel.GetNombreImpresora(oVaildation.GetRutActiveUser());
            oModel.ingOTP = oModel.OTP;
            oModel.ingOTD = oModel.OTD;
            oModel.GetListaVia();
            return PartialView("_VerOTDetalle", oModel);
        }
        #endregion

        #region EditarManifiesto
        [Authorize]
        public ActionResult EditarManifiesto(DespachoProveedorModels oModel)
        {
            ValidationController oValidation = new ValidationController();

           // oModel.optDespachoPorOT = true; // oValidation.TieneRolByName("Despacho por OT");
            

            oModel.NumeroManifiesto = Convert.ToDecimal(oModel.NumeroBusquedaManifiesto);                       
            oModel.EstadoManifiesto = oValidation.GetEstadoManifiestoFromIdManifiesto(oModel.NumeroManifiesto);
            oModel.Proveedor = oValidation.GetProveedorManifiesto(oModel.NumeroManifiesto);
            oModel.Patente = oValidation.GetPatenteManifiesto(oModel.NumeroManifiesto);
            oModel.Conductor = oValidation.GetConductorManifiesto(oModel.NumeroManifiesto);
            
            if (oModel.EstadoManifiesto != "Abierto")
                oModel.GetDatosManifiesto();
            
            oModel.GetListaProveedores();
            oModel.GetListaPatentesProveedor();
            oModel.GetListaArrastres();
            oModel.GetListaConductores();
            oModel.GetListaDespachoPorOT();
            oModel.GetListaResumen();
            oModel.idSucursalActual = oValidation.GetSucursalIDfromActiveUser();
            oModel.GetOpcionesOrden();
            oModel.OpcionOrden = "1";            
            oModel.GetListaEXX();

            oModel.GetIdDatosPrevios();
            oModel.GetNombreDatosPrevios();

            oModel.AgregaOTMAN = oModel.NumeroManifiesto;

            return PartialView("EditarManifiesto", oModel);
        }
        #endregion

        #region Despacho
        [Authorize]
        public ActionResult Despacho()
        {
            DespachoProveedorModels oModel = new DespachoProveedorModels();
            oModel.GetOpcionesOrden();            
            return View(oModel);
        }
        #endregion

        #region DropDownConductor
        public ActionResult DropDownConductor(string idPatente)
        {
            DespachoProveedorModels oModel = new DespachoProveedorModels();            

            oModel.Patente = idPatente;
            oModel.GetListaConductores();
            return PartialView("_DropDownConductor", oModel);
        }
        #endregion

        #region DropDownPatente
        public ActionResult DropDownPatente(string idProveedor)
        {
            DespachoProveedorModels oModel = new DespachoProveedorModels();

            oModel.Proveedor = idProveedor;
            oModel.GetListaPatentesProveedor();
            return PartialView("_DropDownPatente", oModel);
        }
        #endregion

        #region EliminarBulto
        [Authorize]
        public ActionResult EliminarBulto(decimal delOTP, decimal delOTD, decimal delIdBulto)
        {
            PreparacionModels oModel = new PreparacionModels();
            ValidationController oValidation = new ValidationController();
            oModel.OTP = delOTP;
            oModel.OTD = delOTD;
            oModel.CodigoBulto = delIdBulto.ToString();
            oModel.DelBulto();
            oModel.RecalcularTotal();
            oModel.GetDatosByOT();
            oModel.GetListaBultos();
            oModel.GetNombreImpresora(oValidation.GetRutActiveUser());
            oModel.ingOTP = oModel.OTP;
            oModel.ingOTD = oModel.OTD;
            oModel.GetListaVia();
            return PartialView("_VerOTDetalle", oModel);
        }
        #endregion

        #region FinalizarPreparacion
        [Authorize]
        public bool FinalizarPreparacion(decimal OTP, decimal OTD)
        {
            try 
            {
                ValidationController oValidation = new ValidationController();
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                var oDatos = (from otd in oFol.TB_FOL_OTD_OT_DETALLE
                              where otd.PK_FOL_OTP_ID == OTP
                              && otd.PK_FOL_OTD_ID == OTD
                              select otd).Single();

                if (oDatos.PK_FOL_EST_ID==1)//Sucursal
                    oDatos.PK_FOL_EST_ID = 12;
                else if (oDatos.PK_FOL_EST_ID==20)//Cliente
                    oDatos.PK_FOL_EST_ID = 22;
                oDatos.PK_PAR_USU_RUT = oValidation.GetRutActiveUser();
                oDatos.PK_PAR_SUC_ID = oValidation.GetSucursalIDfromActiveUser();
                oFol.SubmitChanges();

               

                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region FinalizarPreparacionMasiva
        [Authorize]
        public bool FinalizarPreparacionMasiva(decimal OTP)
        {
            try
            {
                ValidationController oValidation = new ValidationController();
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                //selecciono todas las ot hijas
                oFol.PR_FOL_CAMBIAR_ESTADO_OTP(OTP,12,oValidation.GetRutActiveUser(),oValidation.GetSucursalIDfromActiveUser());//OT DETALLE Y BULTOS A ESTADO 12
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region GuardarTotalBultos
        [Authorize]
        public ActionResult GuardarTotalBultos(PreparacionModels oModel)
        {
            if (oModel.OTD == 0)
            {
                oModel = TempData["ModelCarga"] as PreparacionModels;
            }
            else
            {                

                ValidationController oValidation = new ValidationController();
                var oSucursal = oValidation.Sucursal(User.Identity.Name.ToString());
                //oModel.SetTotalBultos();  
                oModel.CambiaViaOTD2();
                oModel.CrearBultosOTD(oSucursal.Tipo);
                oModel.RecalcularTotal();
                oModel.GetDatosByOT();
                oModel.GetListaBultos();
                oModel.GetNombreImpresora(oValidation.GetRutActiveUser());
                oModel.ingOTP = oModel.OTP;
                oModel.ingOTD = oModel.OTD;
                oModel.GetListaVia();
            }
            TempData["ModelCarga"] = oModel;
            return PartialView("_VerOTDetalle", oModel);
        }
        #endregion           

        #region ImprimirEtiquetaM1
        [HttpPost]
        [Authorize]
        public ActionResult ImprimirEtiquetaM1(FormCollection oForm)
        {
            ImpresionModels oModel = new ImpresionModels();

            oModel.idBulto = oForm["idBulto"].ToString();
            oModel.OTD = oForm["OTD"].ToString();
            oModel.OTP = oForm["OTP"].ToString();
            oModel.REF1 = oForm["REF1"].ToString();
            oModel.REF2 = oForm["REF2"].ToString();


            return PartialView("_ImprimirEtiquetaM1", oModel);
        }
        #endregion

        #region ImpresionBultosOTD
        [Authorize]
        public ActionResult ImpresionBultosOTD(decimal vOTD, decimal vOTP, string vConf, decimal vIdBulto)
        {
            PreparacionModels oModel = new PreparacionModels();
            oModel.OTD = vOTD;
            oModel.OTP = vOTP;
            oModel.CodigoBulto = vIdBulto.ToString();
            ValidationController oValidation = new ValidationController();

            oModel.GetNombreImpresora(Convert.ToInt32(oValidation.GetRutActiveUser()));


            if (vConf == "4")
            {
                return PartialView("_ImpresionEtiquetaGuia", oModel);
            }
            else if (vConf == "5")
            {
                return PartialView("_ImpresionEtiquetaBulto", oModel);
            }
            else if (vConf == "6")
            {
                return PartialView("_ImpresionEtiquetaMasivo", oModel);
            }
            else
            {
                return PartialView("_ImpresionEtiqueta", oModel);
            }
        }
        #endregion

        #region ImpresionBultosOTP
        [Authorize]
        public ActionResult ImpresionBultosOTP(decimal POTP, string PConf)
        {
            PreparacionModels oModel = new PreparacionModels();            
            oModel.OTP = POTP;            
            ValidationController oValidation = new ValidationController();

            oModel.GetNombreImpresora(Convert.ToInt32(oValidation.GetRutActiveUser()));            
            return PartialView("_ImpresionEtiquetaPorOTP", oModel);            
        }
        #endregion

        #region CustomImpresion
        [Authorize]
        public ActionResult CustomImpresion(decimal CustomOTD, decimal CustomOTP, int CustomEtiqueta, int CustomCantidad, decimal CustomBulto1, decimal CustomBulto2, decimal CustomBulto3, decimal CustomBulto4)
        {

            RetornoModels oModel = new RetornoModels();
            oModel.OTP = CustomOTP;
            oModel.OTD = CustomOTD;


            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();


            oModel.Impresion = new BultoImpresion
            {
                Cantidad=CustomCantidad,
                TipoEtiqueta=CustomEtiqueta
            };
            DatosBulto DB= new DatosBulto();
            if (CustomCantidad > 0)
            {
                var oBultos = from blt in oFol.TB_FOL_BLT_BULTO
                              where
                                blt.PK_FOL_OTP_ID==CustomOTP
                                && blt.PK_FOL_OTD_ID==CustomOTD
                                && blt.PK_FOL_EST_ID!=13
                              orderby blt.PK_FOL_BLT_ID 
                              select blt;

                int oCantidadElementos = 1;

                oModel.Impresion.DatosBulto1 = DB;
                oModel.Impresion.DatosBulto2 = DB;
                oModel.Impresion.DatosBulto3 = DB;
                oModel.Impresion.DatosBulto4 = DB;

                foreach (var oBultosLine in oBultos)
                {
                    if (oBultosLine.PK_FOL_BLT_ID == CustomBulto1)
                    {
                        oModel.Impresion.DatosBulto1 = new DatosBulto
                        {
                            CantidadBulto = oCantidadElementos.ToString(),
                            CodigoBulto = oBultosLine.PK_FOL_BLT_ID,
                            PesoString = oBultosLine.FL_FOL_BLT_PESO.ToString().Replace(".", "_").Replace(",", "_")
                        };
                    }                    

                    if (oBultosLine.PK_FOL_BLT_ID == CustomBulto2)
                    {
                        oModel.Impresion.DatosBulto2 = new DatosBulto
                        {
                            CantidadBulto = oCantidadElementos.ToString(),
                            CodigoBulto = oBultosLine.PK_FOL_BLT_ID,
                            PesoString = oBultosLine.FL_FOL_BLT_PESO.ToString().Replace(".", "_").Replace(",","_")
                        };
                    }
                    

                    if (oBultosLine.PK_FOL_BLT_ID == CustomBulto3)
                    {

                        oModel.Impresion.DatosBulto3 = new DatosBulto
                        {
                            CantidadBulto = oCantidadElementos.ToString(),
                            CodigoBulto = oBultosLine.PK_FOL_BLT_ID,
                            PesoString = oBultosLine.FL_FOL_BLT_PESO.ToString().Replace(".", "_").Replace(",", "_")
                        };
                    }
                    

                    if (oBultosLine.PK_FOL_BLT_ID == CustomBulto4)
                    {
                        oModel.Impresion.DatosBulto4 = new DatosBulto
                        {
                            CantidadBulto = oCantidadElementos.ToString(),
                            CodigoBulto = oBultosLine.PK_FOL_BLT_ID,
                            PesoString = oBultosLine.FL_FOL_BLT_PESO.ToString().Replace(".", "_").Replace(",", "_")
                        };
                    }
                    

                    oCantidadElementos += 1;
                }

                var oDatosEncabezado = (from pr in oFol.PR_FOL_DATOS_ETIQUETA(CustomOTP,CustomOTD)
                                       select pr).Single();

                oModel.Impresion.CantidadBultos = Convert.ToInt32(oDatosEncabezado.CantidadBultos);
                oModel.Impresion.ComunaDestino = oDatosEncabezado.ComunaDestino;
                oModel.Impresion.SucursalDestino = oDatosEncabezado.SucursalDestino;
            }
            else
            {
                oModel.Impresion.DatosBulto1 = DB;
                oModel.Impresion.DatosBulto2 = DB;
                oModel.Impresion.DatosBulto3 = DB;
                oModel.Impresion.DatosBulto4 = DB;
            }

            ValidationController oValidation = new ValidationController();
            oModel.Mensaje = oValidation.GetNombreImpresora(Convert.ToInt32(oValidation.GetRutActiveUser()));
            return PartialView("_ImpresionCustomEtiqueta", oModel);
        }
        #endregion

        #region Index
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region CambiarVia
        public bool CambiarVia(decimal OTP, decimal OTD, string Via)
        {
            try
            {
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                var oOTD = (from otd in oFol.TB_FOL_OTD_OT_DETALLE
                            where otd.PK_FOL_OTP_ID == OTP
                            && otd.PK_FOL_OTD_ID == OTD
                            select otd).Single();
                oOTD.FL_FOL_OTD_VIA = Via;
                oFol.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region IngresoTotalBultos //Aca se determina el inicio de la preparación
        [Authorize][OutputCache (Duration=0)]
        public ActionResult IngresoTotalBultos(PreparacionModels oModel)
        {
            List<HtmlString> oError = new List<HtmlString>();
            ValidationController oValidation = new ValidationController();
            oModel.GetListaVia();

            if (oModel.Referencia == null)
            {
                oModel = TempData["ModelCarga"] as PreparacionModels;
                TempData["ModelCarga"] = oModel;
            }
            else
            {
                TempData["ModelCarga"] = oModel;
            }

            oModel.GetDatosByReferencia();                                

            if (oModel.OTGuardada == null)
            {
                oError.Add(new HtmlString("La referencia ingresada no existe"));
                oModel.ListaErrores=oError;
                return PartialView("_OTFinalizada", oModel);
            }

            

            if (oModel.OTGuardada.BULTO != 0)
            {
                oModel.TotalBultos = oModel.OTGuardada.BULTO.ToString();
            }

            
            oModel.GetViaList();
            oModel.Via = oModel.OTGuardada.VIA;
            
            var oSucursal = oValidation.Sucursal(User.Identity.Name.ToString());

            if (oSucursal.Tipo == 1)
            {
                if (oModel.OTGuardada.OTD_EST_ID == 1 && oModel.OTGuardada.PK_PAR_SUC_ID != oSucursal.id)
                    oError.Add(new HtmlString("Err 1. La OT se encuentra en sucursal <strong>" + oSucursal.Nombre + "<strong>"));
                else if (oModel.OTGuardada.OTD_EST_ID == 20)
                    oError.Add(new HtmlString("Err 2. La OT se encuentra en Cliente"));
                else if (oModel.OTGuardada.OTD_EST_ID != 1)
                    oError.Add(new HtmlString("Err 3. La OT se encuentra en estado <strong>" + oModel.OTGuardada.FL_FOL_EST_NOMBRE + "</strong>"));
            }
            else //2 Empresa
            {
                if (oModel.OTGuardada.OTD_EST_ID ==1)
                    oError.Add(new HtmlString("Err 4. La OT se encuentra en sucursal <strong>" + oSucursal.Nombre + "<strong>"));
                else if (oModel.OTGuardada.OTD_EST_ID != 20 ) //Empresa, en otro estado
                    oError.Add(new HtmlString("Err 5. La OT ya se encuentra preparada"));
               
            }
            if (oError.Count()>0)
            {
                oModel.ListaErrores=oError;
                return PartialView("_OTFinalizada", oModel);
            }
            
            
            oModel.OTP = oModel.OTGuardada.OTP_ID;
            oModel.OTD = oModel.OTGuardada.OTD_ID;

            oModel.ingOTP = oModel.OTGuardada.OTP_ID;
            oModel.ingOTD = oModel.OTGuardada.OTD_ID;

            if (oModel.GetBultos() > 0) //Ya tiene los bultos ingresados
            {
                oModel.GetListaBultos();
                oModel.GetNombreImpresora(oValidation.GetRutActiveUser());
                return PartialView("_VerOTDetalle", oModel);
            }
            else //No tiene bultos ingresados
            {
                if (oModel.OTGuardada.BULTO == 1)//Tiene 1 bulto Crea el bulto con los datos del OTD
                {
                    //tiene 1 bulto, un otp y un otd, no es necesaria la impresión masiva                            
                    oModel.AddBultoConDatosOTD(oSucursal.Tipo);
                    oModel.GetListaBultos();
                    oModel.GetNombreImpresora(oValidation.GetRutActiveUser());
                    return PartialView("_VerOTDetalle", oModel);
                }
                else //tiene más de un bulto
                {
                    return PartialView("_TotalBultos", oModel);
                }
            }          
            
        }
        #endregion

        #region LoadTipoMovil
        [Authorize]
        public string LoadTipoMovil(string idPatente)
        {
            if (idPatente != "" && idPatente != null)
            {
                LinqBD_PARDataContext oFol = new LinqBD_PARDataContext();
                var oDatos = from tra in oFol.TB_PAR_TRA_TRANSPORTE
                             where tra.PK_PAR_TRA_ID == Convert.ToInt32(idPatente)
                             select tra;
                if (oDatos.Count() == 0)
                    return "";
                else
                    return oDatos.ToList()[0].FL_PAR_TRA_MODELO;
            }
            return "";
        }
        #endregion

        #region GuardarBulto
        [Authorize]
        public ActionResult GuardarBulto(PreparacionModels oModel)
        {
            ValidationController oValidation = new ValidationController();
            RetornoModels oRetorno = new RetornoModels();
            if (oModel.ingOTP != 0)
            {
                try
                {
                    oModel.OTP = oModel.ingOTP;
                    oModel.OTD = oModel.ingOTD;
                    oModel.ingIdBulto = oModel.ingIdBulto.Substring(1, oModel.ingIdBulto.Length - 1); //oValidation.GetCodigoBultoFromCodigoGenerico(oModel.ingIdBulto).ToString();
                    oModel.GetFactorVolumetrico();
                    oModel.GuardarBulto();
                    oRetorno.Impresion = oModel.CalculaElementosImpresionEtiqueta();
                    
                    oRetorno.Bulto = Convert.ToDecimal(oModel.ingIdBulto);
                    oRetorno.Ok = true;
                }
                catch (Exception e)
                {
                    oRetorno.Ok = false;
                    oRetorno.Mensaje = "No fue posible guardar la información " + e.Message;
                }
            }
            else
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = "No fue posible guardar la información";                
            }

            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ActualizaListaBultoPreparacion

        [Authorize]
        public ActionResult ActualizaListaBultoPreparacion(PreparacionModels oModel)
        {
            oModel.GetListaBultos();
            return PartialView("_VerListaBultos", oModel);
        }
        #endregion

        #region DatosPreviosManifiesto
        [Authorize]
        public ActionResult DatosPreviosManifiesto(string idManifiesto)
        {
            ValidationController oValidation = new ValidationController();
            DespachoProveedorModels oModel = new DespachoProveedorModels();                        
            oModel.GetlistaSucursalDestino();
            oModel.GetlistaTipoManifiesto();
            if (oValidation.isDecimal(idManifiesto))
            {
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                var oDatos = from man in oFol.TB_FOL_MAN_MANIFIESTO
                             where man.PK_FOL_MAN_ID == Convert.ToDecimal(idManifiesto)
                             select man;
                if (oDatos.Count() > 0)
                {
                    oModel.NumeroManifiesto = Convert.ToDecimal(idManifiesto);
                    var oMan = oDatos.ToList()[0];

                    oModel.TipoManifiesto = oMan.PK_FOL_TPM_ID.ToString();
                    if (oMan.TB_FOL_MAD_MAN_DESTINO.Count() > 0)
                    {
                        oModel.SucursalDestino = oMan.TB_FOL_MAD_MAN_DESTINO.ToList()[0].PK_PAR_SUC_ID.ToString();
                    }
                    else
                    {
                        List<SelectListItem> oList= new List<SelectListItem>();                        
                        oModel.ListaSucursalDestino = oList;
                    }
                }
            }
            
            return PartialView("_FormularioDatosPreviosMan", oModel);
        }
        #endregion

        #region NuevoManifiesto
        [Authorize]
        public ActionResult NuevoManifiesto(DespachoProveedorModels oModel)
        {

            //oModel.TipoManifiesto = oModel.cTipoManifiesto;
            //oModel.SucursalDestino = oModel.cSucursalDestino;
            
            
            ValidationController oValidation = new ValidationController();

            int suc_id=0;
            if (oValidation.IsNumeric2(oModel.SucursalDestino) == true)
            {
                suc_id = Convert.ToInt32(oModel.SucursalDestino);
            }


            //oModel.optDespachoPorOT = oValidation.TieneRolByName("Despacho por OT");
            int oRut = oValidation.GetRutActiveUser();
            int oSucursal = oValidation.GetSucursalIDfromActiveUser();
            var oResult= oModel.CrearManifiesto(oRut, oSucursal, 1,suc_id, 2);
            if (oResult.Ok == false)
            {
                return null;
            }
            else
            {
                oModel.GetListaProveedores();
                oModel.GetListaPatentes();
                oModel.GetListaConductores();
                oModel.GetListaDespachoPorOT();
                oModel.EstadoManifiesto = oValidation.GetEstadoManifiestoFromIdManifiesto(oModel.NumeroManifiesto);
                oModel.GetOpcionesOrden();
                oModel.OpcionOrden = "1";
                oModel.GetNombreDatosPrevios();

                oModel.AgregaOTMAN = oModel.NumeroManifiesto;

                return PartialView("EditarManifiesto", oModel);
            }
        }
        #endregion

        #region DropDownSucursalDestino
        public ActionResult DropDownSucursalDestino(DespachoProveedorModels oModel)
        {
            if (oModel.TipoManifiesto == "1")
            {
                List<SelectListItem> ListaNew = new List<SelectListItem>();
                ListaNew.Add(new SelectListItem
                {
                    Text = "-- Seleccione--",
                    Value="",
                    Selected=true
                });                
                oModel.GetlistaSucursalDestino();
                ListaNew.AddRange(oModel.ListaSucursalDestino);
                oModel.ListaSucursalDestino = ListaNew;
            }
            else
            {
                List<SelectListItem> ListaNew = new List<SelectListItem>();
                ListaNew.Add(new SelectListItem { 
                    Text="-- <Omitir Sucursal> --",
                    Value=""
                
                });
                oModel.ListaSucursalDestino = ListaNew;
            }

           


            return PartialView("_DropDownSucursal", oModel); ;
        }
        #endregion

        #region ValidaDatosPreviosMan
        public ActionResult ValidaDatosPreviosMan(DespachoProveedorModels oModel)
        {
            DevolucionController.RetornoDevolucion oResult = new DevolucionController.RetornoDevolucion();

            if (oModel.TipoManifiesto == "1" && (oModel.SucursalDestino == "" || oModel.SucursalDestino == null))
            {
                oResult.Mensaje = "Debe ingresar una sucursal de destino";
                oResult.OK = false;
            }
            else
            {
                oResult.OK = true;
            }
            
            return Json(oResult);
        }
        #endregion

        #region GuardarCambiosEditPrevios
        public ActionResult GuardarCambiosEditPrevios(DespachoProveedorModels oModel)
        {
            DevolucionController.RetornoDevolucion oResult = new DevolucionController.RetornoDevolucion();
            ValidationController oValidation = new ValidationController();

            oModel.NumeroManifiesto = oModel.EditNumeroManifiesto;

            if (oModel.TipoManifiesto == "1" && (oModel.SucursalDestino == "" || oModel.SucursalDestino == null))
            {
                oResult.Mensaje = "Debe ingresar una sucursal de destino";
                oResult.OK = false;
            }
            else
            {
                try
                {
                    LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                    var oDatos = from man in oFol.TB_FOL_MAN_MANIFIESTO
                                 where man.PK_FOL_MAN_ID == Convert.ToDecimal(oModel.NumeroManifiesto)
                                 select man;
                    if (oDatos.Count() > 0)
                    {
                        var oDato = oDatos.ToList()[0];
                        oDato.PK_FOL_TPM_ID = Convert.ToInt32(oModel.TipoManifiesto);

                        var oMAB = from mab in oFol.TB_FOL_MAB_MANIFIESTO_BULTO
                                   where mab.PK_FOL_MAN_ID == Convert.ToDecimal(oModel.NumeroManifiesto)
                                   select mab;

                        var oBLT = from blt in oFol.TB_FOL_BLT_BULTO
                                   join mab in oFol.TB_FOL_MAB_MANIFIESTO_BULTO
                                   on blt.PK_FOL_BLT_ID equals mab.PK_FOL_BLT_ID
                                   where mab.PK_FOL_MAN_ID == Convert.ToDecimal(oModel.NumeroManifiesto)
                                   && blt.PK_FOL_EST_ID == 14
                                   select blt;

                        foreach (var oBulto in oBLT)
                        {
                            oBulto.PK_FOL_EST_ID = 12;
                        }

                        oFol.TB_FOL_MAB_MANIFIESTO_BULTO.DeleteAllOnSubmit(oMAB);

                        if (oValidation.IsNumeric2(oModel.SucursalDestino))
                        {
                            if (oDato.TB_FOL_MAD_MAN_DESTINO.Count() > 0)
                            {
                                oDato.TB_FOL_MAD_MAN_DESTINO.ToList()[0].PK_PAR_SUC_ID = Convert.ToInt32(oModel.SucursalDestino);
                            }
                            else
                            {
                                System.Data.Linq.EntitySet<TB_FOL_MAD_MAN_DESTINO> oDestino = new System.Data.Linq.EntitySet<TB_FOL_MAD_MAN_DESTINO>();
                                oDestino.Add(new TB_FOL_MAD_MAN_DESTINO
                                {
                                    PK_PAR_SUC_ID = Convert.ToInt32(oModel.SucursalDestino)
                                });

                                oDato.TB_FOL_MAD_MAN_DESTINO = oDestino;
                            }

                        }
                        else
                        {
                            if (oDato.TB_FOL_MAD_MAN_DESTINO.Count() > 0)
                            {
                                var oElements= from mad in oFol.TB_FOL_MAD_MAN_DESTINO
                                                 where mad.PK_FOL_MAN_ID == Convert.ToDecimal(oModel.NumeroManifiesto)
                                                 select mad;
                                oFol.TB_FOL_MAD_MAN_DESTINO.DeleteAllOnSubmit(oElements);                                
                            }
                        }

                    }
                    oFol.SubmitChanges();
                    oResult.OK = true;
                }
                catch (Exception e)
                {
                    oResult.OK = false;
                    oResult.Mensaje = "No fue posible guardar la información: " + e.Message;
                }
            }

            return Json(oResult);
        }
        #endregion

        #region PreparacionPedido
        [Authorize]
        public ActionResult PreparacionPedido()
        {
            PreparacionModels oModel = new PreparacionModels();

            LinqCLIDataContext oPar = new LinqCLIDataContext();
    
            oModel.oItems = from emp in oPar.TB_CLI_EMP_EMPRESAS
                            select emp;
            List<SelectListItem> oTipoReferencia = new List<SelectListItem>();
          

            oModel.ListaTipoReferencia = oTipoReferencia;

            oModel.TipoReferencia = "1";

            return View(oModel);
        }
        #endregion

        #region DropDownBuscarEn
        [Authorize]
        public ActionResult DropDownBuscarEn(string RutCliente)
        {
            ValidationController oValidation = new ValidationController();
            int iRut = 0;

            if (oValidation.ValidarRut(RutCliente) == 1)
            {
                iRut = Convert.ToInt32(RutCliente.Substring(0, RutCliente.Length - 2));
            }

            PreparacionModels oModel = new PreparacionModels();


            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            var oLista = from pr in oPar.PR_PAR_REF_CLIENTE(iRut)
                         select pr;

            List<SelectListItem> oListaSelect = new List<SelectListItem>();

            foreach (var oElemento in oLista)
            {
                oListaSelect.Add(new SelectListItem { 
                    Text=oElemento.FL_PAR_TDO_NOMBRE,
                    Value=oElemento.PK_PAR_TDO_ID.ToString()                    
                });
            }

            oListaSelect.Add(new SelectListItem
            {
                Text = "OT",
                Value = "OT"                
            });

            oModel.ListaTipoReferencia = oListaSelect;


            //agregar valor default

            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from uls in oFol.TB_FOL_ULS_ULTIMA_SELECCION
                         where uls.PK_CLI_EMP_RUT==iRut
                         && uls.PK_PAR_USU_RUT==oValidation.GetRutActiveUser()
                         select uls;
            
            if (oDatos.Count()>0)
            {
                var oULS = oDatos.ToList()[0];
                if (oModel.ListaTipoReferencia.Where(m => m.Value == oULS.FL_FOL_ULS_VALOR.ToString()).Count() > 0)
                {
                    oModel.ListaTipoReferencia.Where(m => m.Value == oULS.FL_FOL_ULS_VALOR.ToString()).Single().Selected = true;
                }
            }


            return PartialView("_DropDownBuscarEn", oModel);
        }
        #endregion

        #region ValidarTodoPreparado
        [HttpPost]
        [Authorize]
        public bool ValidarTodoPreparado(decimal OTP, decimal OTD)
        {
            try
            {

                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                var oDatos = from blt in oFol.TB_FOL_BLT_BULTO
                             where blt.PK_FOL_OTP_ID == OTP
                             && blt.PK_FOL_OTD_ID == OTD
                             && blt.PK_FOL_EST_ID != 13//Elimnado
                             && blt.PK_FOL_EST_ID != 12//Preparado 
                             && blt.PK_FOL_EST_ID != 22//Preparado Cliente
                             select blt; //si encuentro bultos, quiere decir que les FALTA por preparar
                if (oDatos.Count() == 0)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region VerOTDetalle
        [Authorize]        
        public ActionResult VerOTDetalle(PreparacionModels oModel)
        {
            ValidationController oValidation = new ValidationController();
            oModel.GetDatosByOT();
            oModel.GetListaBultos();
            oModel.GetNombreImpresora(oValidation.GetRutActiveUser());
            oModel.ingOTP = oModel.OTP;
            oModel.ingOTD = oModel.OTD;
            oModel.GetListaVia();
            return PartialView("_VerOTDetalle", oModel);
        }
        #endregion

        #region PrepararPorOTP
        [Authorize]
        public ActionResult PrepararPorOTP(decimal OTP, decimal OTD)
        {
            PreparacionModels oModel = new PreparacionModels();

            oModel.OTP = OTP;
            oModel.OTD = OTD;

            if (oModel.ValidaOTPadreNHijos1Bulto())
            {
                oModel.AddBultosMasivoOTP();
                oModel.GetOTDBulto();                
                return PartialView("_OTPNOTD1Bulto", oModel);
            }
            else
            {
                return PartialView("_OTModificada", oModel);
            }
        }
        #endregion

        #region VerHistorialOT
        [Authorize]
        public ActionResult VerHistorialOT(decimal OTP, decimal OTD, decimal IdBulto, int iOTP, int iOTD, int iBulto)
        {
            ValidationController oValidation = new ValidationController();
            HistorialModels oModel = new HistorialModels { 
                OTP_ID=OTP,
                OTD_ID=OTD,
                BLT_ID=IdBulto            
            };

            if (iBulto==1)            
                oModel.GetHistoriaBulto();
            if (iOTD==1)
                oModel.GetHistoriaOTD();
            if (iOTP == 1)
                oModel.GetHistoriaOTP();

            return PartialView("_HistorialOT", oModel);
        }
        #endregion

        #region AgregarBultoMan
        [Authorize]
        public bool AgregarBultoMan(string idBulto, decimal NumeroManifiesto)
        {
            try
            {
                ValidationController oValidation = new ValidationController();
                DespachoProveedorModels oModel = new DespachoProveedorModels();

                decimal BLT_ID = 0;
                bool oResult = false;
                BLT_ID = oValidation.GetCodigoBultoFromCodigoGenerico(idBulto);
                oModel.GetOTPOTDfromBLT(BLT_ID);
                oModel.NumeroManifiesto = NumeroManifiesto;

                oResult = oModel.AgregarBultoManifiesto(oModel.OTP, oModel.OTD, BLT_ID, NumeroManifiesto);

                //if (oResult==true)//Tengo que Mejorar Retorno por posibles errores de conexión...

                //oModel.GetListaDespacho();
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region ListaResumenMan
        [Authorize]
        public ActionResult ListaResumenMan(decimal NumeroManifiesto)
        {
            DespachoProveedorModels oModel = new DespachoProveedorModels();
            ValidationController oValidation = new ValidationController();
            oModel.NumeroManifiesto = NumeroManifiesto;
            
            oModel.GetListaResumen();
            
            oModel.idSucursalActual = oValidation.GetSucursalIDfromActiveUser();
           
            return PartialView("_ListaResumen", oModel);
            
        }
        #endregion

        #region ListaDespachoMan
        [Authorize]
        public ActionResult ListaDespachoMan(decimal NumeroManifiesto, string Orden)
        {
            DespachoProveedorModels oModel = new DespachoProveedorModels();
            ValidationController oValidation = new ValidationController();
            oModel.NumeroManifiesto = NumeroManifiesto;
            int iOrden = 1;

            if (Orden != null)
                iOrden = Convert.ToInt32(Orden);

            oModel.GetListaEXX();

            oModel.idSucursalActual = oValidation.GetSucursalIDfromActiveUser();
            oModel.EstadoManifiesto=oValidation.GetEstadoManifiestoFromIdManifiesto(NumeroManifiesto);


            if (iOrden == 1)
            {
                oModel.GetListaDespachoPorOT();
                return PartialView("_ListaDespacho", oModel);
            }
            else if (iOrden == 2)
            {
                oModel.GetListaResumen();    
                return PartialView("_ListaResumen", oModel);
            }
            else
            {                
                oModel.GetListaDespachoPorOT();
                 return PartialView("_ListaDespachoPorOT", oModel);
            }
            
            
                
                           
        }
        #endregion

        #region ExisteManifiesto
        [Authorize]
        public decimal ExisteManifiesto()
        {
            ValidationController oValidation = new ValidationController();
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            int RutUsuario = 0;

            RutUsuario = oValidation.GetRutActiveUser();

            var oDatos = from man in oFol.TB_FOL_MAN_MANIFIESTO
                         where man.PK_PAR_USU_RUT == RutUsuario
                         && man.PK_FOL_MAE_ID == 1
                         select man.PK_FOL_MAN_ID;

            if (oDatos.Count() > 0)
            {
                return oDatos.ToList()[0];
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region DesasociarOT
        [Authorize]
        public ActionResult DesasociarOT(decimal OTP, decimal OTD, decimal NumeroManifiesto)
        {
            DesasociarModels oModel = new DesasociarModels
            {
                OTP = OTP,
                OTD = OTD,
                NumeroManifiesto = NumeroManifiesto
            };
            try
            {                
                oModel.GetListaDespacho();
            }
            catch { }
            return PartialView("_DesasociarOT", oModel);
        }
        #endregion

        #region DesasociarOTCompleted
        [Authorize]
        public bool DesasociarOTCompleted(DespachoProveedorModels oModel)
        {
            return oModel.DesasociarOT();
        }
        #endregion

        #region ListaDocumentos
        [Authorize]
        public ActionResult ListaDocumentos(DocumentosModels oModel)
        {
            oModel.GetListaDocumentos();
            oModel.GetListaTipo();
            oModel.GetEstadoOT();
            oModel.GetEsAdmin();
            return PartialView("_DocumentosAsociados", oModel);
        }
        #endregion

        #region FormularioAgregaEditaDOA
        public ActionResult FormularioAgregaEditaDOA(DocumentosModels oModel)
        {
            if (oModel.Edita == "Editar")
            {
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                var oDatos = (from doa in oFol.TB_FOL_DOA_DOC_ASOCIADO
                             where doa.PK_FOL_DOA_ID == oModel.DOA_ID
                             select doa).Single();

                oModel.Tipo = oDatos.PK_PAR_TDO_ID.ToString();
                oModel.Numero = oDatos.PK_FOL_DOA_NUMERO.ToString();
                oModel.ContraPago = oDatos.FL_FOL_DOA_CONTRAPAGO;                
            }
            oModel.GetListaTipo();            

            return PartialView("_FormularioAgregaEditaDOA",oModel);
        }
        #endregion

        #region AgregarDocumentoAsociado
        [Authorize]
        public bool AgregarDocumentoAsociado(DocumentosModels oModel)
        {
            return oModel.AddDOA();
        }
        #endregion

        #region ValidarFormularioDOA1
        [Authorize]
        public ActionResult ValidarFormularioDOA1(string OTP, string OTD, string Edita, string DOA_ID, string Tipo, string Numero, string ContraPago)
        {
            RetornoModels oRetorno = new RetornoModels();
            try
            {
               
                ValidationController oValidation = new ValidationController();

                if (!oValidation.IsNumeric2(Tipo))
                {
                    oRetorno.Ok = false;
                    oRetorno.Mensaje = "Debe seleccionar un Tipo de Documento";
                    return Json(oRetorno, JsonRequestBehavior.AllowGet);
                }
                if (!oValidation.isDecimal(Numero))
                {
                    oRetorno.Ok = false;
                    oRetorno.Mensaje = "Debe ingresar un Número valido";
                    return Json(oRetorno, JsonRequestBehavior.AllowGet);
                }

                int iTipo = Convert.ToInt32(Tipo);
                decimal dNumero = Convert.ToDecimal(Numero);
                decimal dOTP = Convert.ToDecimal(OTP);
                decimal dOTD = Convert.ToDecimal(OTD);
                decimal dContrapago = 0;

                if (iTipo == 2)
                {
                    if (ContraPago != "" && ContraPago != null)
                    {
                        if (!oValidation.isDecimal(ContraPago))
                        {
                            oRetorno.Ok = false;
                            oRetorno.Mensaje = "El Contra Pago debe ser un número válido";
                            return Json(oRetorno, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            dContrapago = Convert.ToDecimal(ContraPago);
                        }
                    }
                }

                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

                var oOT = (from ot in oFol.TB_FOL_OTP_OT_PRINCIPAL
                           where ot.PK_FOL_OTP_ID == dOTP
                           select ot).Single();

                if (Edita == "Editar")
                {
                    oRetorno.Ok = false;
                    oRetorno.Mensaje = "Editar";

                    decimal dDOA_ID = Convert.ToDecimal(DOA_ID);

                    var oDOA = (from doa in oFol.TB_FOL_DOA_DOC_ASOCIADO
                                where doa.PK_FOL_DOA_ID == dDOA_ID
                                select doa).Single();

                    if (oDOA.PK_FOL_DOA_NUMERO != dNumero || oDOA.PK_PAR_TDO_ID != iTipo)
                    {
                        var oDOAEx = from doa in oFol.TB_FOL_DOA_DOC_ASOCIADO
                                   where doa.PK_FOL_DOA_NUMERO == dNumero
                                   && doa.PK_PAR_TDO_ID == iTipo
                                   && doa.PK_CLI_EMP_RUT == oOT.PK_CLI_EMP_RUT
                                   select doa;

                        if (oDOAEx.Count() > 0)
                        {
                            oRetorno.Ok = false;
                            oRetorno.Mensaje = "El N° de Documento ya existe para el cliente";
                            return Json(oRetorno, JsonRequestBehavior.AllowGet);
                        }
                    }

                    if (oDOA.PK_FOL_DOA_NUMERO == dNumero && oDOA.PK_PAR_TDO_ID == iTipo)
                    {
                        oDOA.FL_FOL_DOA_CONTRAPAGO = dContrapago;
                        oFol.SubmitChanges();
                        oRetorno.Ok = true;
                        return Json(oRetorno, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        oFol.TB_FOL_RDO_DOA_EDO.DeleteAllOnSubmit(oDOA.TB_FOL_RDO_DOA_EDO);
                        oFol.TB_FOL_DOA_DOC_ASOCIADO.DeleteOnSubmit(oDOA);


                        TB_FOL_DOA_DOC_ASOCIADO oInsertDoa = new TB_FOL_DOA_DOC_ASOCIADO
                        {
                            FL_FOL_DOA_FECHA = DateTime.Now,
                            FL_FOL_DOA_CONTRAPAGO = dContrapago,
                            PK_CLI_EMP_RUT = oOT.PK_CLI_EMP_RUT,
                            PK_FOL_DOA_NUMERO = dNumero,
                            PK_FOL_OTP_ID = dOTP,
                            PK_FOL_OTD_ID = dOTD,
                            PK_PAR_SUC_ID = oValidation.GetSucursalIDfromActiveUser(),
                            PK_PAR_USU_RUT = oValidation.GetRutActiveUser(),
                            PK_PAR_TDO_ID = iTipo
                        };
                        oFol.TB_FOL_DOA_DOC_ASOCIADO.InsertOnSubmit(oInsertDoa);
                        oFol.SubmitChanges();
                        oRetorno.Ok = true;
                        return Json(oRetorno, JsonRequestBehavior.AllowGet);
                    }
                }
                else//Nuevo
                {                   

                    var oDOA = from doa in oFol.TB_FOL_DOA_DOC_ASOCIADO
                               where doa.PK_FOL_DOA_NUMERO == dNumero
                               && doa.PK_PAR_TDO_ID == iTipo
                               && doa.PK_CLI_EMP_RUT == oOT.PK_CLI_EMP_RUT
                               select doa;

                    if (oDOA.Count() > 0)
                    {
                        oRetorno.Ok = false;
                        oRetorno.Mensaje = "El N° de Documento ya existe para el cliente";
                        return Json(oRetorno, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        TB_FOL_DOA_DOC_ASOCIADO oDoa = new TB_FOL_DOA_DOC_ASOCIADO
                        {
                            FL_FOL_DOA_FECHA = DateTime.Now,
                            FL_FOL_DOA_CONTRAPAGO = dContrapago,
                            PK_CLI_EMP_RUT = oOT.PK_CLI_EMP_RUT,
                            PK_FOL_DOA_NUMERO = dNumero,
                            PK_FOL_OTP_ID = dOTP,
                            PK_FOL_OTD_ID = dOTD,
                            PK_PAR_SUC_ID = oValidation.GetSucursalIDfromActiveUser(),
                            PK_PAR_USU_RUT = oValidation.GetRutActiveUser(),
                            PK_PAR_TDO_ID = iTipo
                        };
                        oFol.TB_FOL_DOA_DOC_ASOCIADO.InsertOnSubmit(oDoa);
                        oFol.SubmitChanges();
                        oRetorno.Ok = true;
                        return Json(oRetorno, JsonRequestBehavior.AllowGet);
                    }
                }                
            }
            catch (Exception e)
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje="No fue posible verificar la información:" + e.Message;
                return Json(oRetorno, JsonRequestBehavior.AllowGet);
            }            
        }
        #endregion

        #region ValidaCerrarManifiesto
        [Authorize]
        public string ValidaCerrarManifiesto(decimal MAN_ID)
        {


            ValidationController oValidation = new ValidationController();
            int ActiveUser = oValidation.GetRutActiveUser();
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from man in oFol.TB_FOL_MAN_MANIFIESTO
                         where man.PK_PAR_USU_RUT == ActiveUser
                         && man.PK_FOL_MAN_ID==MAN_ID
                         && man.PK_FOL_MAE_ID == 1//Abierto
                         select man;


            if (oDatos.Count() == 0)
            {
                return "Usuario sin manifiesto abierto";
            }
            else
            {
                if (oDatos.Single().PK_PAR_TRA_ID == null || oDatos.Single().PK_PAR_PRO_ID == null || oDatos.Single().PK_PAR_CON_RUT == null)
                {
                    return "Faltan datos por guardar en la cabecera del manifiesto.";
                }

                int idManifiesto = Convert.ToInt32(oDatos.ToList()[0].PK_FOL_MAN_ID);
                var oMab = (from pr in oFol.PR_FOL_MAN_VALIDA(ActiveUser,false,idManifiesto)
                               select pr.Column1).Single();

                if (oMab>0)
                {
                    return "Existen OT incompletas, debe ingresar todos los bultos";
                }
                else
                {
                    var oConBultos = (from pr in oFol.PR_FOL_MAN_VALIDA(ActiveUser,true,idManifiesto)
                               select pr.Column1).Single();
                                        
                    if (oConBultos > 0)
                        return "OK";
                    else
                        return "Manifiesto sin bultos";
                }
            }
        }
        #endregion

        #region CerrarManCarga
        [HttpPost]
        public ActionResult CerrarManCarga(DespachoProveedorModels oModel)
        {
            ValidationController oValidation = new ValidationController();
            int ActiveUser = oValidation.GetRutActiveUser();
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from man in oFol.TB_FOL_MAN_MANIFIESTO
                         where man.PK_PAR_USU_RUT == ActiveUser
                         && man.PK_FOL_MAN_ID == oModel.NumeroManifiesto
                         && man.PK_FOL_MAE_ID == 1//Abierto
                         select man;

            if (oDatos.Count() ==1){

                oModel.Patente = oDatos.Single().PK_PAR_TRA_ID.ToString();
                oModel.Conductor = oDatos.Single().PK_PAR_CON_RUT.ToString();
            }

            return Json(oModel.CierraManifiesto());

           
        }
        #endregion  
      
        #region GuardarCabecera  
        public JsonResult GuardarDatosCabecera(int provId, int patId, int condId, decimal MAN_ID)
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
                if (provId == null)
                {
                    oRetorno.Ok = false;
                    oRetorno.Mensaje = "Debe seleccionar un Proveedor";
                    return Json(oRetorno, JsonRequestBehavior.AllowGet);
                }
                if (patId == null)
                {
                    oRetorno.Ok = false;
                    oRetorno.Mensaje = "Debe seleccionar una Patente";
                    return Json(oRetorno, JsonRequestBehavior.AllowGet);
                }
                if (condId == null)
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

                    var oMab = from mab in oFol.TB_FOL_MAB_MANIFIESTO_BULTO
                               join blt in oFol.TB_FOL_BLT_BULTO on mab.PK_FOL_BLT_ID equals blt.PK_FOL_BLT_ID
                               where mab.PK_FOL_MAN_ID == MAN_ID
                               select blt;

                    foreach (var oFila in oMab)
                    {
                        oFol.PR_FOL_DESASOCIAR_OT(oFila.PK_FOL_OTP_ID, oFila.PK_FOL_OTD_ID, MAN_ID, oValidation.GetSucursalIDfromActiveUser(), oValidation.GetRutActiveUser());
                    }


                    //return Json(true, JsonRequestBehavior.AllowGet);

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
                oRetorno.Mensaje = e.Message;
                //throw;
            }

            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }
        #endregion 

        #region CargarDatosBulto
        [HttpPost][Authorize]
        public string CargarDatosBulto(string idBulto)
        {
            try
            {
                ValidationController oValidation = new ValidationController();

                var oResult = oValidation.GetCodigoBultoFromCodigoGenerico(idBulto);

                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                var oDatos = (from x in oFol.TB_FOL_BLT_BULTO
                              where x.PK_FOL_BLT_ID == oResult
                              select x).Single();

                string oElement = "";

                oElement = oDatos.FL_FOL_BLT_ALTO.ToString() + "_";
                oElement += oDatos.FL_FOL_BLT_ANCHO.ToString() + "_";
                oElement += oDatos.FL_FOL_BLT_LARGO.ToString() + "_";
                oElement += oDatos.FL_FOL_BLT_PESO.ToString();

                return (oElement);
            }
            catch {
                return "";
            }
        }
        #endregion

        #region ListadDestino
        public ActionResult ListaDestino()
        {
            CalculoDestinoModels oModel = new CalculoDestinoModels();
            oModel.SucursalOrigen=22;
            oModel.ComunaDestino=1101;
            oModel.GetDatosDestino(0);

            return View(oModel);
        }
        #endregion

        #region CreaDestinoManifiesto 
        public JsonResult CreaDestinoManifiesto(decimal MAN_ID, string idBulto, string Patente)
        {
            ValidationController oValidation = new ValidationController();
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            var oResult = oValidation.vjSonValidarIdBultoDespacho(idBulto, MAN_ID);
            RetornoModels oRetorno = new RetornoModels();
            oRetorno.Ok = true;

            int intPatente = 0;


            if (Patente == null || Patente == "")
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = "Debe seleccionar una Patente";
                return Json(oRetorno, JsonRequestBehavior.AllowGet);
            }
            else
            {
                intPatente = Convert.ToInt32(Patente);
            }

            if (oResult.Ok == true && oRetorno.Ok == true)
            {

                decimal intBulto = oValidation.GetCodigoBultoFromCodigoGenerico(idBulto);

                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();


                var oBlt = from blt in oFol.TB_FOL_BLT_BULTO
                           join otd in oFol.TB_FOL_OTD_OT_DETALLE on new { blt.PK_FOL_OTP_ID, blt.PK_FOL_OTD_ID } equals new { otd.PK_FOL_OTP_ID, otd.PK_FOL_OTD_ID }
                           where blt.PK_FOL_BLT_ID == intBulto
                           select new { blt, otd };



                var oDatosBulto = oBlt.ToList();

                int oMSuc = 0;


                if (oDatosBulto.Count() > 0)
                {

                    var oResultBulto = (from pr in oFol.PR_FOL_TIPO_SERVICIO_BULTO(intBulto)
                                        select pr).Single();

                    if (oResultBulto.PK_PAR_TSE_ID == 2)
                    {
                        var oMAB2 = from mab in oFol.TB_FOL_MAB_MANIFIESTO_BULTO
                               where mab.PK_FOL_MAN_ID == MAN_ID
                               select mab;

                        if (oMAB2.Count() == 0)
                        {
                            oFol.PR_FOL_MAD_ELIMINAR(MAN_ID);
                        }
                            var oMAN = (from man in oFol.TB_FOL_MAN_MANIFIESTO
                                        where man.PK_FOL_MAN_ID == MAN_ID
                                        select man).Single();


                            oMAN.PK_FOL_TPM_ID = 2;
                            oFol.SubmitChanges();
                            oRetorno.Ok = true;
                            return Json(oRetorno, JsonRequestBehavior.AllowGet);
                        
                    }


                    CalculoDestinoModels oModel = new CalculoDestinoModels();

                    int ComunaDestino = 0;

                    //New
                    var oComunaDestinoFromDEN = from den in oFol.TB_FOL_DEN_DIRECCION_ENTREGA
                                                join blt in oFol.TB_FOL_BLT_BULTO
                                                    on new { den.PK_FOL_OTP_ID, den.PK_FOL_OTD_ID } equals new { blt.PK_FOL_OTP_ID, blt.PK_FOL_OTD_ID }
                                                where blt.PK_FOL_BLT_ID == intBulto
                                                orderby den.PK_FOL_DEN_ID descending
                                                select den;


                    ComunaDestino = Convert.ToInt32(oComunaDestinoFromDEN.ToList()[0].PK_PAR_COM_ID);
                    int Via = Convert.ToInt32(oComunaDestinoFromDEN.ToList()[0].PK_PAR_VIA_ID);

                    //if (oValidation.IsNumeric2(oDatosBulto[0].otd.PK_PAR_COM_ID.ToString()))
                    //{
                    //    ComunaDestino = Convert.ToInt32(oDatosBulto[0].otd.PK_PAR_COM_ID);                        
                    //}

                    //New

                    oModel.SucursalOrigen = oValidation.GetSucursalIDfromActiveUser();

                    //Inicio, Si el usuario es externo, puede que la sucursal envíe al cliente.
                    var oSucursalReemplazo = from paso in oPar.TB_PAR_SUC_SUCURSAL
                                             join suc in oPar.TB_PAR_SUC_SUCURSAL
                                                on paso.PK_PAR_SUC_ID equals suc.PK_PAR_SUC_ID
                                             where paso.PK_PAR_SUC_ID == oModel.SucursalOrigen
                                             select suc;

                    if (oSucursalReemplazo.Count() > 0)
                    {
                        var oElementoReemplazo = oSucursalReemplazo.ToList()[0];
                        var oNuevaSucursal = from cob in oPar.TB_PAR_COB_COBERTURA_COMUNA
                                             where cob.PK_PAR_COM_ID == oElementoReemplazo.PK_PAR_COM_ID
                                             select cob;

                        if (oNuevaSucursal.Count() > 0)
                        {
                            oModel.SucursalOrigen = oNuevaSucursal.ToList()[0].PK_PAR_SUC_ID;
                            oMSuc = oModel.SucursalOrigen;
                        }
                    }


                    //Fin


                    oModel.ComunaDestino = ComunaDestino;
                    oModel.Patente = intPatente;

                    int SucursalDestino22 = oValidation.GetIdCobertura(ComunaDestino);
                    if (SucursalDestino22 == oModel.SucursalOrigen)
                    {
                        Via = 1;
                    }
                    oModel.GetDatosDestino(Via);

                    

                    //if (oModel.SucursalDestinoManifiesto == 0)
                    //{
                    //    oRetorno.Ok = false;
                    //    oRetorno.Mensaje = "Destino Bulto no es posible enviar en el Transporte " + Patente + ", sin ruta definida destino";
                    //    return Json(oRetorno, JsonRequestBehavior.AllowGet);
                    //}


                    var oMAB = from mab in oFol.TB_FOL_MAB_MANIFIESTO_BULTO
                               where mab.PK_FOL_MAN_ID == MAN_ID
                               select mab;

                    if (oMAB.Count() == 0)
                    {
                        oFol.PR_FOL_MAD_ELIMINAR(MAN_ID);

                        var oMAN = (from man in oFol.TB_FOL_MAN_MANIFIESTO
                                    where man.PK_FOL_MAN_ID == MAN_ID
                                    select man).Single();

                        //Nuevo
                        var oSucListNew = from pr in oFol.PR_FOL_DEN_SUC_COBERTURA_BULTO(intBulto)
                                          select pr;
                        var oSucursalNew = oSucListNew.ToList();
                        var oSucursalCobertura = oSucursalNew[0].PK_PAR_SUC_ID;
                        //Fin Nuevo

                        int oSucursalUser = 0;
                        if (oMSuc == 0)
                        {
                            oSucursalUser = oValidation.GetSucursalIDfromActiveUser();
                        }
                        else// quiere decir que es un cliente que despacha ruta cliente
                        {
                            oSucursalUser = oMSuc;
                        }

                        if (oModel.SucursalDestinoManifiesto == oSucursalUser && oSucursalCobertura == oSucursalUser)
                        {
                            var oTra = from tra in oPar.TB_PAR_TRA_TRANSPORTE
                                       where tra.PK_PAR_TRA_ID == intPatente
                                       select tra;

                            if (oTra.Count() > 0)
                            {
                                var oDat = oTra.ToList()[0];
                                if (oDat.FL_PAR_TRA_DESTINOCLIENTE == true)
                                {
                                    oMAN.PK_FOL_TPM_ID = 2;
                                    oFol.SubmitChanges();
                                    oRetorno.Ok = true;
                                    return Json(oRetorno, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    oRetorno.Ok = false;
                                    oRetorno.Mensaje = "El Bulto tiene destino Ruta Cliente, el Transporte, no permite el envio a Cliente";
                                    return Json(oRetorno, JsonRequestBehavior.AllowGet);
                                }
                            }

                        }
                        if (oModel.SucursalDestinoManifiesto == oSucursalUser && oSucursalCobertura != oSucursalUser && oModel.SucursalOrigen == oValidation.GetSucursalIDfromActiveUser())
                        {
                            oRetorno.Ok = false;
                            oRetorno.Mensaje = "El Transporte no tiene Ruta definida para Destino Bulto";
                            return Json(oRetorno, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            oMAN.PK_FOL_TPM_ID = 1;
                            System.Data.Linq.EntitySet<TB_FOL_MAD_MAN_DESTINO> oEmad = new System.Data.Linq.EntitySet<TB_FOL_MAD_MAN_DESTINO>();
                            oEmad.Add(new TB_FOL_MAD_MAN_DESTINO
                            {
                                PK_FOL_MAN_ID = MAN_ID,
                                PK_PAR_SUC_ID = oModel.SucursalDestinoManifiesto
                            });

                            oMAN.TB_FOL_MAD_MAN_DESTINO = oEmad;
                            oFol.SubmitChanges();
                            oRetorno.Ok = true;
                        }

                    }
                    else
                    {
                        var oMan = (from man in oFol.TB_FOL_MAN_MANIFIESTO
                                    where man.PK_FOL_MAN_ID == MAN_ID
                                    select man).Single();


                        int oSucursalUser = 0;
                        if (oMSuc == 0)
                        {
                            oSucursalUser = oValidation.GetSucursalIDfromActiveUser();
                        }
                        else// quiere decir que es un cliente que despacha ruta cliente
                        {
                            oSucursalUser = oMSuc;
                        }

                        var oBultotake = (from blt in oFol.TB_FOL_MAB_MANIFIESTO_BULTO
                                            where blt.PK_FOL_MAN_ID == MAN_ID
                                            select blt).Take(1);

                        var oBultoTake1 = oBultotake.ToList()[0];

                        var oSucListNew2 = from pr in oFol.PR_FOL_DEN_SUC_COBERTURA_BULTO(oBultoTake1.PK_FOL_BLT_ID)
                                            select pr;
                        var oSucursalNew2 = oSucListNew2.ToList();
                        var oSucursalCobertura2 = oSucursalNew2[0].PK_PAR_SUC_ID;

                        //Nuevo
                        var oSucListNew = from pr in oFol.PR_FOL_DEN_SUC_COBERTURA_BULTO(intBulto)
                                            select pr;
                        var oSucursalNew = oSucListNew.ToList();
                        var oSucursalCobertura = oSucursalNew[0].PK_PAR_SUC_ID;

                    }
                }
            }
            else
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = oResult.Mensaje;
            }
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ValidarContenidoVisible
        public ActionResult ValidarContenidoVisible(int MAN_ID)
        {
            try
            {
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                var oDatos = from mab in oFol.TB_FOL_MAB_MANIFIESTO_BULTO
                             where mab.PK_FOL_MAN_ID == MAN_ID
                             && mab.FL_FOL_MAB_OK==true
                             select mab;
                if (oDatos.Count() == 0)
                    return Json(1, JsonRequestBehavior.AllowGet);
                else if (oDatos.Count()==1)
                    return Json(2, JsonRequestBehavior.AllowGet);
                else
                    return Json(3, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region GuardarPatenteManifiesto
        public ActionResult GuardarPatenteManifiesto(int MAN_ID, string Patente)
        {
            ValidationController oValidation = new ValidationController();
            int? intPatente = null;

            if (oValidation.IsNumeric2(Patente))
                intPatente = Convert.ToInt32(Patente);

            try
            {
                
                    LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                    var oDatos = (from man in oFol.TB_FOL_MAN_MANIFIESTO
                                  where man.PK_FOL_MAN_ID == MAN_ID
                                  select man).Single();

                    oDatos.PK_PAR_TRA_ID = intPatente;
                    oFol.SubmitChanges();

                    var oMab = from mab in oFol.TB_FOL_MAB_MANIFIESTO_BULTO
                               join blt in oFol.TB_FOL_BLT_BULTO on mab.PK_FOL_BLT_ID equals blt.PK_FOL_BLT_ID
                               where mab.PK_FOL_MAN_ID == MAN_ID
                               select blt;

                    foreach (var oFila in oMab)
                    {
                        oFol.PR_FOL_DESASOCIAR_OT(oFila.PK_FOL_OTP_ID, oFila.PK_FOL_OTD_ID, MAN_ID, oValidation.GetSucursalIDfromActiveUser(), oValidation.GetRutActiveUser());
                    }

                    return Json(true, JsonRequestBehavior.AllowGet);
                                  
            }
            catch (Exception e)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region GetDatosTipoManifiesto
        public ActionResult GetDatosTipoManifiesto(DespachoProveedorModels oModel)
        {
            oModel.GetIdDatosPrevios();
            oModel.GetNombreDatosPrevios();
            return Json(oModel, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ListaManifiesto
        public ActionResult ListaManifiesto()
        {
            DespachoProveedorModels oModel = new DespachoProveedorModels();
            oModel.GetListaManAbiertosProveedor();
            oModel.GetListaManCerradosHoyProveedor();
            return PartialView("_ListaManifiestoAbierto", oModel);
        }
        #endregion

        #region VerificaAddOT
        public ActionResult VerificaAddOT(DespachoProveedorModels oModel)
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            ValidationController oValidation = new ValidationController();
            RetornoModels oRetorno = new RetornoModels();
            var oResult = oValidation.TransformCodigoGenericoOTPOTD(oModel.AgregaOT);
            if (oResult.Error == 1)
            {
                oRetorno.Mensaje = oResult.ErrorMensaje;
                oRetorno.Ok = false;
            }
            else
            {

                var oDatos = (from pr in oFol.PR_FOL_MAN_VERIFICA_ADD_OT(oResult.OTP, oResult.OTD, oModel.AgregaOTMAN, oValidation.GetRutActiveUser())
                              select pr).Single();
                if (oDatos.ERR_ID == 0)
                {
                    try
                    {
                        var oResultDestino=CreaDestinoManifiesto(oModel.AgregaOTMAN, 'B' + oDatos.BLT_ID.ToString(), oDatos.TRA_ID.ToString());

                        RetornoModels oResultMod = oResultDestino.Data as RetornoModels;

                        if (oResultMod.Ok == true)
                        {
                            oFol.PR_FOL_MAN_ADD_OTD(oResult.OTP, oResult.OTD, oModel.AgregaOTMAN, oValidation.GetRutActiveUser());
                            oRetorno.Mensaje = "";
                            oRetorno.Ok = true;
                        }
                        else
                        {
                            oRetorno.Mensaje = oResultMod.Mensaje;
                            oRetorno.Ok = false;
                        }
                        
                    }
                    catch(Exception e)
                    {
                        oRetorno.Mensaje = e.Message;
                        oRetorno.Ok = false;
                    }
                    
                    
                }
                else
                {
                    oRetorno.Mensaje = oDatos.ERR_MENSAJE;
                    oRetorno.Ok = false;
                }
            }
            return Json(oRetorno,JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ValidarOTPOP
        public ActionResult ValidarOTPOP(PreparacionModels oModel)
        {
            RetornoHUModels oRetorno = new RetornoHUModels();
            bool oHU = false;
            try{
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

                ValidationController oValidation = new ValidationController();

                oModel.GetDatosByReferencia();

                var oDatos = (from pr in oFol.PR_FOL_IHU_CONSULTA(oModel.OTGuardada.OTP_ID, oModel.OTGuardada.OTD_ID, oValidation.GetRutActiveUser())
                             select pr).Single();

                if (oDatos.FL_PAR_SER_PIDE_HU == true)
                    oHU = true;

                oRetorno.Ok = true;
                oRetorno.HU = oHU;
                oRetorno.OTP = oModel.OTGuardada.OTP_ID;
                oRetorno.OTD = oModel.OTGuardada.OTD_ID;
            }
            catch(Exception e){
                oRetorno.Ok = false;
                oRetorno.Mensaje = "No existe información, Verifique los datos";
            }

            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region _FormualrioHU
        public ActionResult _FormularioHU(FormularioHUModels oModel)
        {
            return PartialView("_FormularioHU", oModel);
        }
        #endregion
        #region GuardarHU
        public ActionResult GuardarHU(FormularioHUModels oModel)
        {
            RetornoModels oRetorno = new RetornoModels();
            try
            {
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                TB_FOL_IHU_HU oHU = new TB_FOL_IHU_HU
                {
                    PK_FOL_OTP_ID=oModel.huOTP,
                    PK_FOL_OTD_ID=oModel.huOTD,
                    FL_FOL_IHU_VALOR=oModel.huHU
                };
                oFol.TB_FOL_IHU_HU.InsertOnSubmit(oHU);
                oFol.SubmitChanges();
                oRetorno.Ok = true;                
                
            }
            catch (Exception e)
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = "No fue posible guardar la información " + e.Message;
            }

            return Json(oRetorno,JsonRequestBehavior.AllowGet);
        }
        #endregion

        public ActionResult ValidaPesoBulto(decimal OTP, decimal OTD, string Peso)
        {
            RetornoPesoModels oRetorno = new RetornoPesoModels();
            ValidationController oValidation = new ValidationController();
            if (oValidation.isDecimal(Peso))
            {
                decimal oDecPeso = Convert.ToDecimal(Peso);
                try
                {
                    LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                    var oDatos = (from pr in oFol.PR_FOL_PES_PESO_MAXIMO(OTP, OTD)
                                  select pr).Single();
                    if (oDatos.FL_PAR_SER_ALERTA_KILO == true)
                    {
                        if (oDecPeso > oDatos.FL_PAR_SER_ALERTA_VALOR)
                        {
                            oRetorno.Ok = true;
                            oRetorno.PesoMaximo = Convert.ToInt32(oDecPeso);
                        }
                        else
                        {
                            oRetorno.Ok = false;
                        }
                    }
                    else
                    {
                        oRetorno.Ok = false;
                    }
                }
                catch
                {
                    oRetorno.Ok = false;
                }
            }
            else
            {
                oRetorno.Ok = false;
            }
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }
    }
}

