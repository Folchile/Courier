using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Courier.Models;

namespace Courier.Controllers
{
    public class MantencionController : Controller
    {
        #region Index
        public ActionResult Index()
        {
            return View();
        }
        #endregion


        public class RetornaValidaciones
        {
            public Boolean ExisteRutCliente { get; set; }
        }


        public ActionResult RetornoValidarCliente(MantencionModels mModel)
        {
            int rtCli;
            rtCli = Convert.ToInt32(mModel.Rut.Substring(0, mModel.Rut.Length - 2));
            RetornaValidaciones Resultado = new RetornaValidaciones();
            Courier.Models.LinqCLIDataContext oCli = new LinqCLIDataContext();
            var cDatos = from c in oCli.TB_CLI_EMP_EMPRESAS
                         where c.PK_CLI_EMP_RUT == rtCli
                         select c;
            Resultado.ExisteRutCliente = false;
            if (cDatos.Count() == 1)
            {
                Resultado.ExisteRutCliente = true;
            }
            return Json(Resultado, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult AgregaCliente(MantencionModels mModel)
        {
            MantencionModels.Respuesta oResult = new MantencionModels.Respuesta();

            oResult = mModel.AgregaClient();
            return Json(oResult, JsonRequestBehavior.AllowGet);
        }



        #region Ingreso Cliente
        public ActionResult IngresoCliente()
        {
            return View();
        }
        #endregion

        #region Agregar Cliente
        public ActionResult AgregarCliente()
        {
            return View();
        }
        #endregion

        #region Buscar Cliente
        public ActionResult BuscarCliente()
        {
            var oRazonSocial = "";
            var oDataContext = new LinqCLIDataContext();
            var oEmpresas = from m in oDataContext.TB_CLI_EMP_EMPRESAS
                            where
                                m.FL_CLI_EMP_FANTASIA.Contains(oRazonSocial) ||
                                m.FL_CLI_EMP_RAZON_SOCIAL.Contains(oRazonSocial)
                            select m;

            var model = new MantencionModels()
            {
                oItems = oEmpresas.ToList()
            };
            return View(model);
        }


        public ActionResult FormIngresoClientes()
        {
            MantencionModels mModel = new MantencionModels();
            return PartialView("IngresoClientes", mModel);
        }


        public ActionResult CargLstSucursales(FormCollection gForm)
        {
            MantencionModels mModel = new MantencionModels();
            mModel.GetListaSucCltes(Convert.ToInt32(gForm["rtcli"]));
            return PartialView("VerSucursales", mModel);
        }


        [HttpPost]
        public ActionResult BuscarCliente(FormCollection oForm)
        {
            var oRazonSocial = "";
            if (oForm["razonsocial"] != null)
            {
                oRazonSocial = oForm["razonsocial"];
            }

            var oDataContext = new LinqCLIDataContext();
            var oEmpresas = from m in oDataContext.TB_CLI_EMP_EMPRESAS
                            where
                                m.FL_CLI_EMP_RAZON_SOCIAL.Contains(oRazonSocial) ||
                                m.FL_CLI_EMP_FANTASIA.Contains(oRazonSocial)
                            select m;

            var model = new MantencionModels()
            {
                oItems = oEmpresas.ToList()
            };
            return PartialView("_ListadoClientes", model);
        }
        #endregion

        #region Listado Clientes
        public ActionResult _ListadoClientes(FormCollection oForm)
        {
            //MantencionModels model = new MantencionModels();
            var oDataContext = new LinqCLIDataContext();
            var oEmpresas = from m in oDataContext.TB_CLI_EMP_EMPRESAS
                            where
                                m.FL_CLI_EMP_FANTASIA.Contains(oForm["razonsocial"]) ||
                                m.FL_CLI_EMP_RAZON_SOCIAL.Contains(oForm["razonsocial"])
                            select m;


            var model = new MantencionModels()
            {
                oItems = oEmpresas.ToList()
            };

            return PartialView("_ListadoClientes", model);
        }
        #endregion

        #region Buscar Servicio
        public ActionResult BuscarServicio()
        {
            var oDataContext = new LinqBD_PARDataContext();
            var oServicio = from m in oDataContext.TB_PAR_SER_SERVICIO
                            select m;

            var model = new MantencionModels()
            {
                // oServicio = oServicio.ToList()
            };
            return View(model);
        }
        [HttpPost]
        public ActionResult BuscarServicio(FormCollection oForm)
        {
            var oServiciotxt = "";
            if (oForm["servicio"] != null)
            {
                oServiciotxt = oForm["servicio"];
            }

            var oDataContext = new LinqBD_PARDataContext();
            var oServicio = from m in oDataContext.TB_PAR_SER_SERVICIO
                            where
                                m.FL_PAR_SER_NOMBRE.Contains(oServiciotxt)
                            select m;

            var model = new MantencionModels()
            {
                // oServicio = oServicio.ToList()
            };
            return PartialView("_ListadoServicio", model);
        }
        #endregion

        #region Cliente - Servicios
        public ActionResult ClienteServicio()
        {
            return View();
        }
        #endregion

        #region Clasificacion
        public ActionResult Clasificacion()
        {

            return View();
        }
        #endregion

        #region Sucursal
        public ActionResult Sucursal()
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();


            var oSuc = from suc in oPar.TB_PAR_SUC_SUCURSAL
                       join com in oPar.TB_PAR_COM_COMUNA on suc.PK_PAR_COM_ID equals com.PK_PAR_COM_ID
                       //join cor in oPar.TB_PAR_COR_COORDENADAS on com.PK_PAR_COR_ID equals cor.PK_PAR_COR_ID
                       orderby suc.FL_PAR_SUC_NOMBRE
                       select new ListaSucursal
                       {
                           IdSucursal = suc.PK_PAR_SUC_ID,
                           NombreSucursal = suc.FL_PAR_SUC_NOMBRE,
                           //latitud=Convert.ToDouble(cor.FL_PAR_COR_LATITUD),
                           //longitud=Convert.ToDouble(cor.FL_PAR_COR_LONGITUD),
                           comunaId = com.PK_PAR_COM_ID
                       };



            SucursalModels oModel = new SucursalModels();

            oModel.oLista = oSuc;

            return View(oModel);
        }
        public ActionResult CoberturaSucursal(int idSucursal)
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();


            var oCob = from cob in oPar.TB_PAR_COB_COBERTURA_COMUNA
                       join com in oPar.TB_PAR_COM_COMUNA on cob.PK_PAR_COM_ID equals com.PK_PAR_COM_ID
                       join prv in oPar.TB_PAR_PRV_PROVINCIA on com.PK_PAR_PRV_ID equals prv.PK_PAR_PRV_ID
                       join reg in oPar.TB_PAR_REG_REGION on prv.PK_PAR_REG_ID equals reg.PK_PAR_REG_ID
                       where cob.PK_PAR_SUC_ID == idSucursal
                       &&
                        (cob.FL_PAR_COB_DO == true
                        || cob.FL_PAR_COB_LU == true
                        || cob.FL_PAR_COB_MA == true
                        || cob.FL_PAR_COB_MI == true
                        || cob.FL_PAR_COB_JU == true
                        || cob.FL_PAR_COB_VI == true
                        || cob.FL_PAR_COB_SA == true)
                       orderby reg.FL_PAR_REG_NOMBRE, com.FL_PAR_COM_NOMBRE
                       select new ListaCobertura
                       {
                           NombreComuna = com.FL_PAR_COM_NOMBRE,
                           Lunes = cob.FL_PAR_COB_LU,
                           Martes = cob.FL_PAR_COB_MA,
                           Miercoles = cob.FL_PAR_COB_MI,
                           Jueves = cob.FL_PAR_COB_JU,
                           Viernes = cob.FL_PAR_COB_VI,
                           Sabado = cob.FL_PAR_COB_SA,
                           Domingo = cob.FL_PAR_COB_DO,
                           NombreRegion = reg.FL_PAR_REG_NOMBRE
                       };

            var oSCob = from cob in oPar.TB_PAR_COB_COBERTURA_COMUNA
                        join com in oPar.TB_PAR_COM_COMUNA on cob.PK_PAR_COM_ID equals com.PK_PAR_COM_ID
                        join prv in oPar.TB_PAR_PRV_PROVINCIA on com.PK_PAR_PRV_ID equals prv.PK_PAR_PRV_ID
                        join reg in oPar.TB_PAR_REG_REGION on prv.PK_PAR_REG_ID equals reg.PK_PAR_REG_ID
                        where cob.PK_PAR_SUC_ID == idSucursal
                        &&
                         (cob.FL_PAR_COB_DO == false
                         && cob.FL_PAR_COB_LU == false
                         && cob.FL_PAR_COB_MA == false
                         && cob.FL_PAR_COB_MI == false
                         && cob.FL_PAR_COB_JU == false
                         && cob.FL_PAR_COB_VI == false
                         && cob.FL_PAR_COB_SA == false)
                        orderby com.FL_PAR_COM_NOMBRE
                        select new ListaCobertura
                        {
                            NombreComuna = com.FL_PAR_COM_NOMBRE,
                            Lunes = cob.FL_PAR_COB_LU,
                            Martes = cob.FL_PAR_COB_MA,
                            Miercoles = cob.FL_PAR_COB_MI,
                            Jueves = cob.FL_PAR_COB_JU,
                            Viernes = cob.FL_PAR_COB_VI,
                            Sabado = cob.FL_PAR_COB_SA,
                            Domingo = cob.FL_PAR_COB_DO,
                            NombreRegion = reg.FL_PAR_REG_NOMBRE
                        };

            SucursalModels oModel = new SucursalModels();

            oModel.oCobertura = oCob;
            oModel.oSinCobertura = oSCob;

            return PartialView("_CoberturaSucursal", oModel);
        }
        #endregion

        #region Cobertura

        private CoberturaModels ConsultaCobertura(string idRegion)
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            CoberturaModels oModel = new CoberturaModels();

            //Lista de Cobertura        
            if (idRegion == "" || idRegion == null)
            {
                var oListado = from vi in oPar.VI_PAR_COBERTURA
                               select vi;
                oModel.ListaCobertura = oListado;
            }
            else
            {
                var oListado = from vi in oPar.VI_PAR_COBERTURA
                               where vi.PK_PAR_REG_ID == Convert.ToInt32(idRegion)
                               select vi;
                oModel.ListaCobertura = oListado;
            }
            //Regiones para Dropdownlistbox
            List<SelectListItem> oSeleccione = new List<SelectListItem>();


            if (idRegion == null)
            {
                var oRegion = from reg in oPar.TB_PAR_REG_REGION
                              select new SelectListItem
                              {
                                  Text = reg.FL_PAR_REG_NOMBRE,
                                  Value = reg.PK_PAR_REG_ID.ToString()
                              };

                oSeleccione.Add(new SelectListItem { Value = "", Text = "-- Seleccione Región --", Selected = true });
                oSeleccione.AddRange(oRegion);
            }
            oModel.ListaRegiones = oSeleccione;

            return (oModel);
        }

        [Authorize]
        public ActionResult ListaCoberturaComuna(string idRegion)
        {
            var oModel = ConsultaCobertura(idRegion);
            return PartialView("_ListaCoberturaComuna", oModel);
        }

        [Authorize]
        public ActionResult Cobertura(string idRegion)
        {
            var oModel = ConsultaCobertura(idRegion);
            return View(oModel);
        }

        #endregion

        #region Cobertura Sucursal
        public ActionResult CoberturaEntreSucursal(EntreSucursalesModels oModel)
        {
            if (Session["SucName"] == null || Session["SucId"] == null)
            {
                var oVC = new ValidationController();
                var oResult = oVC.Sucursal(User.Identity.Name.ToString());
                Session["SucName"] = oResult.Nombre;
                Session["SucName2"] = oResult.Nombre;
                Session["SucId"] = oResult.id;

            }

            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            oModel.Origen = Convert.ToInt32(Session["SucId"]);

            var oLista = from malla in oPar.VI_PAR_MALLA_COBERTURA
                         select new ListaCoberturaSucursal
                         {
                             Origen = malla.ORIGEN,
                             Destino = malla.DESTINO,
                             Lunes = malla.FL_PAR_TRN_LU,
                             Martes = malla.FL_PAR_TRN_MA,
                             Miercoles = malla.FL_PAR_TRN_MI,
                             Jueves = malla.FL_PAR_TRN_JU,
                             Viernes = malla.FL_PAR_TRN_VI,
                             Sabado = malla.FL_PAR_TRN_SA,
                             Domingo = malla.FL_PAR_TRN_DO,
                             Via = malla.FL_PAR_VIA_NOMBRE
                         };
            oModel.ListaCoberturaGeneral = oLista;

            oLista = from malla in oPar.VI_PAR_MALLA_COBERTURA
                     where malla.PK_PAR_CDI_ID == oModel.Origen
                     orderby malla.PK_PAR_VIA_ID
                     select new ListaCoberturaSucursal
                     {
                         Origen = malla.ORIGEN,
                         Destino = malla.DESTINO,
                         Lunes = malla.FL_PAR_TRN_LU,
                         Martes = malla.FL_PAR_TRN_MA,
                         Miercoles = malla.FL_PAR_TRN_MI,
                         Jueves = malla.FL_PAR_TRN_JU,
                         Viernes = malla.FL_PAR_TRN_VI,
                         Sabado = malla.FL_PAR_TRN_SA,
                         Domingo = malla.FL_PAR_TRN_DO,
                         Via = malla.FL_PAR_VIA_NOMBRE
                     };

            oModel.ListaCobertura = oLista;

            return View(oModel);
        }
        #endregion

        #region Cobertura Con Comuna
        public ListaCoberturaModels RetornaListaCoberturaModels(ListaCoberturaModels oModel)
        {
            if (Session["SucName"] == null && oModel.Sucursal == null)
            {
                ValidationController oVal = new ValidationController();
                var oSuc = oVal.Sucursal(User.Identity.Name);
                Session["SucName"] = oSuc.Nombre;
                Session["SucName2"] = oSuc.Nombre;
                Session["SucId"] = oSuc.id;
            }


            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            var oListaSucursal = from suc in oPar.TB_PAR_SUC_SUCURSAL
                                 where suc.PK_PAR_TUS_ID==1
                                 select new SelectListItem
                                 {
                                     Text = "Sucursal " + suc.FL_PAR_SUC_NOMBRE,
                                     Value = suc.PK_PAR_SUC_ID.ToString()
                                 };

            oModel.ListaSucursal = oListaSucursal;
            if (oModel.Sucursal == null)
                oModel.Sucursal = Session["SucId"].ToString();

            var oLista = from cob in oPar.TB_PAR_COB_COBERTURA_COMUNA
                         join destino in oPar.TB_PAR_COM_COMUNA on cob.PK_PAR_COM_ID equals destino.PK_PAR_COM_ID
                         join via in oPar.TB_PAR_VIA_VIA on cob.PK_PAR_VIA_ID equals via.PK_PAR_VIA_ID
                         join cla in oPar.TB_PAR_CLA_CLASIFICACION on destino.PK_PAR_CLA_ID equals cla.PK_PAR_CLA_ID
                         where cob.PK_PAR_SUC_ID == Convert.ToInt32(oModel.Sucursal)
                         orderby destino.FL_PAR_COM_NOMBRE
                         select new ListaCoberturaSucursal
                         {
                             Destino = destino.FL_PAR_COM_NOMBRE,
                             Lunes = Convert.ToBoolean(cob.FL_PAR_COB_LU),
                             Martes = Convert.ToBoolean(cob.FL_PAR_COB_MA),
                             Miercoles = Convert.ToBoolean(cob.FL_PAR_COB_MI),
                             Jueves = Convert.ToBoolean(cob.FL_PAR_COB_JU),
                             Viernes = Convert.ToBoolean(cob.FL_PAR_COB_VI),
                             Sabado = Convert.ToBoolean(cob.FL_PAR_COB_SA),
                             Domingo = Convert.ToBoolean(cob.FL_PAR_COB_DO),
                             TiempoEstimado = Convert.ToInt32(cob.FL_PAR_COB_TIEMPO_ESTIMADO),
                             Via = via.FL_PAR_VIA_NOMBRE,
                             Clasificacion = cla.FL_PAR_CLA_NOMBRE,
                             Idcom = cob.PK_PAR_COM_ID
                         };

            var oListaLoc = from col in oPar.TB_PAR_CLO_COBERTURA_LOCALIDAD
                            join destino in oPar.TB_PAR_LOC_LOCALIDAD on col.PK_PAR_LOC_ID equals destino.PK_PAR_LOC_ID
                            join via in oPar.TB_PAR_VIA_VIA on col.PK_PAR_VIA_ID equals via.PK_PAR_VIA_ID
                            join cla in oPar.TB_PAR_CLA_CLASIFICACION on destino.PK_PAR_CLA_ID equals cla.PK_PAR_CLA_ID
                            where col.PK_PAR_SUC_ID == Convert.ToInt32(oModel.Sucursal)
                            orderby destino.FL_PAR_LOC_NOMBRE
                            select new ListaCoberturaSucursal
                            {
                                Destino = destino.FL_PAR_LOC_NOMBRE,
                                Lunes = Convert.ToBoolean(col.FL_PAR_CLO_LU),
                                Martes = Convert.ToBoolean(col.FL_PAR_CLO_MA),
                                Miercoles = Convert.ToBoolean(col.FL_PAR_CLO_MI),
                                Jueves = Convert.ToBoolean(col.FL_PAR_CLO_JU),
                                Viernes = Convert.ToBoolean(col.FL_PAR_CLO_VI),
                                Sabado = Convert.ToBoolean(col.FL_PAR_CLO_SA),
                                Domingo = Convert.ToBoolean(col.FL_PAR_CLO_DO),
                                TiempoEstimado = Convert.ToInt32(col.FL_PAR_CLO_TIEMPO_ESTIMADO),
                                Via = via.FL_PAR_VIA_NOMBRE,
                                Clasificacion = cla.FL_PAR_CLA_NOMBRE,
                                Idloc = destino.PK_PAR_LOC_ID
                            };


            oModel.ListaCobertura = oLista;
            oModel.ListaCoberturaLocalidad = oListaLoc;
            return oModel;
        }
        public ActionResult CoberturaConComuna(ListaCoberturaModels oModel, string idSucursal)
        {
            if (ValidationController.IsNumeric(idSucursal))
                oModel.Sucursal = idSucursal;
            oModel = RetornaListaCoberturaModels(oModel);
            return View(oModel);
        }

        public ActionResult ListaSucursalComuna(string idSucursal)
        {

            ListaCoberturaModels oModel = new ListaCoberturaModels();

            oModel.Sucursal = idSucursal;

            oModel = RetornaListaCoberturaModels(oModel);

            return PartialView("_ListaSucursalComuna", oModel);
        }
        public ActionResult ListaSucursalLocalidad(string idSucursal)
        {

            ListaCoberturaModels oModel = new ListaCoberturaModels();

            oModel.Sucursal = idSucursal;

            oModel = RetornaListaCoberturaModels(oModel);

            return PartialView("_ListaSucursalLocalidad", oModel);
        }
        #endregion

        #region MantencionListaServicios
        [Authorize]
        public ActionResult MantencionListaServicios(MantencionModels oModel)
        {
            oModel.GetListaServicio();
            return PartialView("_ListadoServicio", oModel);
        }
        #endregion

        #region CargaEditarServicio
        [Authorize]
        public ActionResult CargaEditarServicio(MantencionModels oModel)
        {
            ValidationController oValidation = new ValidationController();
            oModel.GetListaTipoServicio();
            oModel.GetListaDocumentos(0);
            oModel.GetDatosDocumento();
            return PartialView("_FormularioEditarServicio", oModel);
        }
        #endregion

        [Authorize]
        public ActionResult CargaListaDocumentos(MantencionModels oModel)
        {
            ValidationController oValidation = new ValidationController();
            int iTDO_ID = 0;
            if (oValidation.IsNumeric2(oModel.TDO_ID))
                iTDO_ID = Convert.ToInt32(oModel.TDO_ID);

            oModel.GetListaDocumentos(iTDO_ID);

            return PartialView("_ListaDocumentos", oModel);
        }
        [Authorize]
        [HttpPost]
        public ActionResult GuardarServicio(MantencionModels oModel)
        {
            if (oModel.Servicio > 0)
            {
                var oResult = oModel.GuardarEditar();
                return Json(oResult);
            }
            else
            {
                var oResult = GuardarServicioNuevo(oModel);
                return Json(oResult);
            }

        }
        [Authorize]
        public Courier.Models.MantencionModels.ResultGuardarEditar GuardarServicioNuevo(MantencionModels oModel)
        {
            Courier.Models.MantencionModels.ResultGuardarEditar oResult = new Courier.Models.MantencionModels.ResultGuardarEditar();
            try
            {

                ValidationController oValidation = new ValidationController();

                int iRutCliete = 0;
                if (oValidation.ValidarRut(oModel.RutClienteServicio) == 1)
                    iRutCliete = Convert.ToInt32(oModel.RutClienteServicio.Substring(0, oModel.RutClienteServicio.Length - 2));

                //Relación Empresa - Servicio
                System.Data.Linq.EntitySet<TB_PAR_EMS_EMPRESA_SERVICIO> oEMS = new System.Data.Linq.EntitySet<TB_PAR_EMS_EMPRESA_SERVICIO>();
                oEMS.Add(new TB_PAR_EMS_EMPRESA_SERVICIO
                {
                    PK_CLI_EMP_RUT = iRutCliete,
                    FL_PAR_EMS_FECHA_CREACION = DateTime.Now,
                    PK_PAR_EST_ID = 1,
                });

                //Servicio - Referencias

                System.Data.Linq.EntitySet<TB_PAR_RES_REFERENCIA_SERVICIO> oRes = new System.Data.Linq.EntitySet<TB_PAR_RES_REFERENCIA_SERVICIO>();

                if (oValidation.IsNumeric2(oModel.Referencia1))
                {
                    if (Convert.ToInt32(oModel.Referencia1) > 0)
                        oRes.Add(new TB_PAR_RES_REFERENCIA_SERVICIO
                        {
                            PK_PAR_REF_ID = 1,
                            PK_PAR_TDO_ID = Convert.ToInt32(oModel.Referencia1)
                        });
                }
                if (oValidation.IsNumeric2(oModel.Referencia2))
                {
                    if (Convert.ToInt32(oModel.Referencia2) > 0)
                        oRes.Add(new TB_PAR_RES_REFERENCIA_SERVICIO
                        {
                            PK_PAR_REF_ID = 2,
                            PK_PAR_TDO_ID = Convert.ToInt32(oModel.Referencia2)
                        });
                }

                int iCantidadVisitas = 0;
                if (oValidation.IsNumeric2(oModel.CantidadVisitas))
                {
                    iCantidadVisitas = Convert.ToInt32(oModel.CantidadVisitas);
                }

                int iTSE = Convert.ToInt32(oModel.TipoServicio);

                int ValorPeso = 0;
                if (oValidation.IsNumeric2(oModel.ValorAlertaKilo))
                {
                    ValorPeso = Convert.ToInt32(oModel.ValorAlertaKilo);
                }


                //Servicio
                TB_PAR_SER_SERVICIO oSer = new TB_PAR_SER_SERVICIO
                {
                    FL_PAR_SER_FACTOR_VOLUMETRICO = Convert.ToInt32(oModel.FactorVolumetrico),
                    FL_PAR_SER_NOMBRE = oModel.NombreServicio,
                    FL_PAR_SER_TRABAJA_PESO_KG = Convert.ToBoolean(oModel.PideKG),
                    FL_PAR_SER_TRABAJA_PESO_VOL = Convert.ToBoolean(oModel.PideDimensiones),
                    PK_PAR_EST_ID = 1,
                    TB_PAR_EMS_EMPRESA_SERVICIO = oEMS,
                    TB_PAR_RES_REFERENCIA_SERVICIO = oRes,
                    FL_PAR_SER_CANTIDAD_VISITAS = iCantidadVisitas,
                    PK_PAR_TSE_ID = iTSE,
                    FL_PAR_SER_ALERTA_KILO = Convert.ToBoolean(oModel.AlertaKilo),
                    FL_PAR_SER_PIDE_HU = Convert.ToBoolean(oModel.PideHU),
                    FL_PAR_SER_ALERTA_VALOR = ValorPeso
                };

                LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
                oPar.TB_PAR_SER_SERVICIO.InsertOnSubmit(oSer);
                oPar.SubmitChanges();
                oResult.Ok = true;
                oResult.Mensaje = "OK";
            }
            catch (Exception e)
            {
                oResult.Ok = false;
                oResult.Mensaje = string.Format("<p>No fue posible guardar la información</p> {0}", e.Message);
            }


            return oResult;
        }


        [HttpPost]
        public ActionResult ActualizaCobertura(ListaCoberturaModels vModel)
        {

            Clases.Retorno oRetorno = new Clases.Retorno();
            oRetorno = vModel.UpdateCobertura(1);
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult ActualizaCoberturaLoc(ListaCoberturaModels vModel)
        {

            Clases.Retorno oRetorno = new Clases.Retorno();
            oRetorno = vModel.UpdateCobertura(2);
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult AsignSucursal(ListaCoberturaModels vModel)
        {

            Clases.Retorno oRetorno = new Clases.Retorno();
            oRetorno = vModel.ActualizaSucCobertura(1, 0);
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult AsignLocalidad(ListaCoberturaModels vModel)
        {

            Clases.Retorno oRetorno = new Clases.Retorno();
            oRetorno = vModel.ActualizaSucCobertura(2, vModel.Localidad);
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }


        public ActionResult MostrarCobComunas(ListaCoberturaModels cModel)
        {
            cModel.GetListaComunas(1, Convert.ToInt32(cModel.Sucursal));
            cModel.GetListaVias();
            return PartialView("_AsignaCobComuna", cModel);
        }

        public ActionResult MostrarCobLocalidades(ListaCoberturaModels cModel)
        {

            cModel.GetListaComunas(3, Convert.ToInt32(cModel.Sucursal));
            cModel.GetListaLocalidadesCob(0, 0);
            cModel.GetListaVias();
            return PartialView("_AsignaCobLocalidad", cModel);
        }

        public ActionResult CargaLocalidades(ListaCoberturaModels cModel)
        {
            cModel.GetListaLocalidadesCob(Convert.ToInt32(cModel.Sucursal), Convert.ToInt32(cModel.ComunaLoc));
            return PartialView("_DropDownLocalidades", cModel);
        }

        public class RetornoClasificacion
        {
            public decimal Va { get; set; }
            public string clasif { get; set; }
            public string sucact { get; set; }
            public string sucdest { get; set; }
            public decimal TmpoEstimado { get; set; }
            public bool Lun { get; set; }
            public bool Mar { get; set; }
            public bool Mie { get; set; }
            public bool Jue { get; set; }
            public bool Vie { get; set; }
            public bool Sab { get; set; }
            public bool Dom { get; set; }
        }


        public JsonResult RescataClasificacion(ListaCoberturaModels lModel)
        {

            RetornoClasificacion oResultado = new RetornoClasificacion();
            LinqBD_PARDataContext oClasif = new LinqBD_PARDataContext();
            var oDatosClasif = from c in oClasif.TB_PAR_COM_COMUNA
                               join des in oClasif.TB_PAR_CLA_CLASIFICACION on c.PK_PAR_CLA_ID equals des.PK_PAR_CLA_ID
                               where c.PK_PAR_COM_ID == lModel.Comuna
                               select c;
            if (oDatosClasif.Count() > 0)
            {

                foreach (var odClasif in oDatosClasif)
                {

                    oResultado.clasif = odClasif.TB_PAR_CLA_CLASIFICACION.FL_PAR_CLA_NOMBRE;
                }
            }

            var oDatosSuc = from d in oClasif.TB_PAR_COB_COBERTURA_COMUNA
                            join com in oClasif.TB_PAR_SUC_SUCURSAL on d.PK_PAR_SUC_ID equals com.PK_PAR_SUC_ID
                            where d.PK_PAR_COM_ID == lModel.Comuna
                            select d;
            if (oDatosSuc.Count() > 0)
            {
                foreach (var odSuc in oDatosSuc)
                {

                    oResultado.Lun = odSuc.FL_PAR_COB_LU;
                    oResultado.Mar = odSuc.FL_PAR_COB_MA;
                    oResultado.Mie = odSuc.FL_PAR_COB_MI;
                    oResultado.Jue = odSuc.FL_PAR_COB_JU;
                    oResultado.Vie = odSuc.FL_PAR_COB_VI;
                    oResultado.Sab = odSuc.FL_PAR_COB_SA;
                    oResultado.Dom = odSuc.FL_PAR_COB_DO;
                    oResultado.TmpoEstimado = Convert.ToDecimal(odSuc.FL_PAR_COB_TIEMPO_ESTIMADO);
                    oResultado.Va = odSuc.PK_PAR_VIA_ID;
                    oResultado.sucact = odSuc.TB_PAR_SUC_SUCURSAL.FL_PAR_SUC_NOMBRE;
                }
            }

            var oSucDest = from s in oClasif.TB_PAR_SUC_SUCURSAL
                           where s.PK_PAR_SUC_ID == Convert.ToInt32(lModel.Sucursal)
                           select s;
            if (oSucDest.Count() > 0)
            {

                foreach (var sucdes in oSucDest)
                {

                    oResultado.sucdest = sucdes.FL_PAR_SUC_NOMBRE;
                }
            }

            return Json(oResultado, JsonRequestBehavior.AllowGet);
        }



        public JsonResult RescataDatosLocalidad(ListaCoberturaModels lModel)
        {

            RetornoClasificacion oResultado = new RetornoClasificacion();
            LinqBD_PARDataContext oClasif = new LinqBD_PARDataContext();
            var oDatosClasif = from c in oClasif.TB_PAR_COM_COMUNA
                               join des in oClasif.TB_PAR_CLA_CLASIFICACION on c.PK_PAR_CLA_ID equals des.PK_PAR_CLA_ID
                               where c.PK_PAR_COM_ID == lModel.ComunaLoc
                               select c;
            if (oDatosClasif.Count() > 0)
            {

                foreach (var odClasif in oDatosClasif)
                {

                    oResultado.clasif = odClasif.TB_PAR_CLA_CLASIFICACION.FL_PAR_CLA_NOMBRE;
                }
            }

            var oDatosSuc = from d in oClasif.TB_PAR_CLO_COBERTURA_LOCALIDAD
                            join com in oClasif.TB_PAR_SUC_SUCURSAL on d.PK_PAR_SUC_ID equals com.PK_PAR_SUC_ID
                            where d.PK_PAR_LOC_ID == lModel.Localidad
                            select d;
            if (oDatosSuc.Count() > 0)
            {
                foreach (var odSuc in oDatosSuc)
                {

                    oResultado.Lun = odSuc.FL_PAR_CLO_LU;
                    oResultado.Mar = odSuc.FL_PAR_CLO_MA;
                    oResultado.Mie = odSuc.FL_PAR_CLO_MI;
                    oResultado.Jue = odSuc.FL_PAR_CLO_JU;
                    oResultado.Vie = odSuc.FL_PAR_CLO_VI;
                    oResultado.Sab = odSuc.FL_PAR_CLO_SA;
                    oResultado.Dom = odSuc.FL_PAR_CLO_SA;
                    oResultado.TmpoEstimado = Convert.ToDecimal(odSuc.FL_PAR_CLO_TIEMPO_ESTIMADO);
                    oResultado.Va = odSuc.PK_PAR_VIA_ID;
                    oResultado.sucact = odSuc.TB_PAR_SUC_SUCURSAL.FL_PAR_SUC_NOMBRE;
                }
            }

            var oSucDest = from s in oClasif.TB_PAR_SUC_SUCURSAL
                           where s.PK_PAR_SUC_ID == Convert.ToInt32(lModel.Sucursal)
                           select s;
            if (oSucDest.Count() > 0)
            {

                foreach (var sucdes in oSucDest)
                {

                    oResultado.sucdest = sucdes.FL_PAR_SUC_NOMBRE;
                }
            }

            return Json(oResultado, JsonRequestBehavior.AllowGet);
        }





        [HttpPost]
        public ActionResult EditarCobertura(FormCollection gForm)
        {
            ListaCoberturaModels cModel = new ListaCoberturaModels();
            LinqBD_FOLDataContext cCob = new LinqBD_FOLDataContext();
            var cDatos = from c in cCob.PR_FOL_CONSULTA_COBERTURA(Convert.ToInt32(gForm["id_suc"]), Convert.ToInt32(gForm["id_cob"]))
                         select c;

            foreach (var oFile in cDatos)
            {
                cModel.IDCOM = oFile.PK_PAR_COM_ID;
                cModel.IDSUC = oFile.PK_PAR_SUC_ID;
                cModel.Clasificacion = oFile.FL_PAR_CLA_NOMBRE;
                cModel.Comna = oFile.FL_PAR_COM_NOMBRE;
                cModel.TiempoEstimado = Convert.ToDecimal(oFile.FL_PAR_COB_TIEMPO_ESTIMADO);
                cModel.Via = oFile.PK_PAR_VIA_ID;
                cModel.Lunes = oFile.FL_PAR_COB_LU;
                cModel.Martes = oFile.FL_PAR_COB_MA;
                cModel.Miercoles = oFile.FL_PAR_COB_MI;
                cModel.Jueves = oFile.FL_PAR_COB_JU;
                cModel.Viernes = oFile.FL_PAR_COB_VI;
                cModel.Sabado = oFile.FL_PAR_COB_SA;
                cModel.Domingo = oFile.FL_PAR_COB_DO;
            }
            cModel.GetListaVias();
            return PartialView("_EditaDiasCobertura", cModel);
        }


        [HttpPost]
        public ActionResult EditarCoberturaLoc(FormCollection gForm)
        {
            ListaCoberturaModels cModel = new ListaCoberturaModels();
            LinqBD_FOLDataContext cCob = new LinqBD_FOLDataContext();
            var cDatos = from c in cCob.PR_FOL_CONSULTA_COB_LOC(Convert.ToInt32(gForm["id_cobloc"]))
                         select c;

            foreach (var oFile in cDatos)
            {
                //cModel.IDCOM = oFile.PK_PAR_COM_ID;
                cModel.IDSUC = oFile.PK_PAR_SUC_ID;
                cModel.ClasificacionLoc = oFile.FL_PAR_CLA_NOMBRE;
                cModel.Locldad = oFile.FL_PAR_LOC_NOMBRE;
                cModel.Localidad = Convert.ToInt32(oFile.PK_PAR_LOC_ID);
                cModel.TiempoEstimadoLoc = Convert.ToDecimal(oFile.FL_PAR_CLO_TIEMPO_ESTIMADO);
                cModel.ViaLoc = oFile.PK_PAR_VIA_ID;
                cModel.LunesLoc = oFile.FL_PAR_CLO_LU;
                cModel.MartesLoc = oFile.FL_PAR_CLO_MA;
                cModel.MiercolesLoc = oFile.FL_PAR_CLO_MI;
                cModel.JuevesLoc = oFile.FL_PAR_CLO_JU;
                cModel.ViernesLoc = oFile.FL_PAR_CLO_VI;
                cModel.SabadoLoc = oFile.FL_PAR_CLO_SA;
                cModel.DomingoLoc = oFile.FL_PAR_CLO_DO;
            }
            cModel.GetListaVias();
            return PartialView("_EditaDiasCoberturaLoc", cModel);
        }

        #region Transporte
        public ActionResult Transporte()
        {

            return View();
        }
        #endregion
    }
}
