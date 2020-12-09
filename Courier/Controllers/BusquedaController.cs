using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Courier.Models;
using System.IO;
using ClosedXML.Excel;

namespace Courier.Controllers
{
    public class BusquedaController : Controller
    {
        #region Tiempo Estimado Entrega
        public ActionResult TiempoEstimadoEntrega()
        {
            BusquedaModels oModel = new BusquedaModels();
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            var oListaSucursal = from suc in oPar.TB_PAR_SUC_SUCURSAL
                                 select new SelectListItem
                                 {
                                     Text=suc.FL_PAR_SUC_NOMBRE,
                                     Value=suc.PK_PAR_SUC_ID.ToString()
                                 };



            oModel.ListaSucursal = oListaSucursal;
            if (Session["SucId"]!=null)
               oModel.SucursalOrigen = Session["SucId"].ToString();


            return View(oModel);
        }
        #endregion

        #region Tiempo Estimado Destino
        public ActionResult TiempoEstimadoDestino()
        {
            BusquedaModels oModel = new BusquedaModels();
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            var oListaSucursal = from suc in oPar.TB_PAR_SUC_SUCURSAL
                                 select new SelectListItem
                                 {
                                     Text = suc.FL_PAR_SUC_NOMBRE,
                                     Value = suc.PK_PAR_SUC_ID.ToString()
                                 };

            var oListaRegion = from reg in oPar.TB_PAR_REG_REGION
                               select new SelectListItem
                               {
                                   Text=reg.FL_PAR_REG_NOMBRE,
                                   Value=reg.PK_PAR_REG_ID.ToString()
                               };


            List<SelectListItem> oListaBlancoComuna=new List<SelectListItem>();
            List<SelectListItem> oListaBlancoLocalidad=new List<SelectListItem>();

            oListaBlancoComuna.Add(new SelectListItem { Text="-- Seleccione Comuna --",Value="" });
            oListaBlancoLocalidad.Add(new SelectListItem { Text = "-- Seleccione Localidad (Opcional) --", Value = "" });


            oModel.ListaSucursal = oListaSucursal;
            oModel.ListaRegion = oListaRegion;
            oModel.ListaBlancoComuna = oListaBlancoComuna;
            oModel.ListaBlancoLocalidad = oListaBlancoLocalidad;

            return View(oModel);
        }
        #endregion

        #region Función Entre Sucursales
        public List<ListaTiempoEstimado> EntreSucursales(BusquedaModels oModel)
        {
            string oDiaActual = "";
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            List<ListaTiempoEstimado> oLista = new List<ListaTiempoEstimado>();

            var oDatos = from a in oPar.TB_PAR_SUC_SUCURSAL
                         select new SelectListItem
                         {
                             Text = a.FL_PAR_SUC_NOMBRE,
                             Value = a.PK_PAR_SUC_ID.ToString()
                         };

            oModel.ListaSucursal = oDatos;

            if (oModel.SucursalDestino == "" || oModel.SucursalDestino == null )
            {
                oLista.Add(new ListaTiempoEstimado { Linea = string.Format("Debe Seleccionar un Destino"), Horas = 0 });
            }
            
            string strOrigen = (from a in oModel.ListaSucursal
                                where a.Value == oModel.SucursalOrigen
                                select a).Single().Text;

            string strDestino = (from a in oModel.ListaSucursal
                                 where a.Value == oModel.SucursalDestino
                                 select a).Single().Text;


            
            DateTime dateValue = DateTime.Now;

            oDiaActual = EntoEs(dateValue.DayOfWeek.ToString());

            var oMensaje = "Hoy Es:" + oDiaActual;
            oLista.Add(new ListaTiempoEstimado { Linea = oMensaje, Horas = 0 });

            if (oModel.SucursalOrigen == oModel.SucursalDestino)
            {
                oMensaje = string.Format("Sucursal Origen {0} igual a Sucursal Destino {1}, sin tiempo de transito entre sucursales", strOrigen, strDestino);
                oLista.Add(new ListaTiempoEstimado { Linea = oMensaje, Horas = 0 });
            }
            else
            {

                var oDiaSalida = from a in oPar.TB_PAR_TRN_TRANSITO
                                 where a.PK_PAR_TRN_ORIGEN == Convert.ToInt32(oModel.SucursalOrigen)
                                 && a.PK_PAR_TRN_DESTINO == Convert.ToInt32(oModel.SucursalDestino)
                                 select a;

                if (oDiaSalida.Count() == 0)
                {

                    oMensaje = string.Format("No se ha definido tiempo de transito entre la Sucursal de Origen {0} y Destino {1}", strOrigen, strDestino);
                    oLista.Add(new ListaTiempoEstimado { Linea = oMensaje, Horas = 0 });
                }
                else
                {

                    var oSalida = oDiaSalida.ToList()[0];
                    oLista.Add(fCobertura(DiaToInt(oDiaActual), Convert.ToBoolean(oSalida.FL_PAR_TRN_LU), Convert.ToBoolean(oSalida.FL_PAR_TRN_MA), Convert.ToBoolean(oSalida.FL_PAR_TRN_MI), Convert.ToBoolean(oSalida.FL_PAR_TRN_JU), Convert.ToBoolean(oSalida.FL_PAR_TRN_VI), Convert.ToBoolean(oSalida.FL_PAR_TRN_SA), Convert.ToBoolean(oSalida.FL_PAR_TRN_DO), strOrigen, strDestino,0));
                    oLista.Add(new ListaTiempoEstimado { Linea = string.Format("Tiempo de Tránsito entre la Sucursal de Origen {0} y Destino {1}", strOrigen, strDestino), Horas = Convert.ToInt32(oSalida.FL_PAR_TRN_TIEMPO_ESTIMADO) });
                }

            }
            return oLista;
        }
        #endregion

        #region DestinoFinal
        public List<ListaTiempoEstimado> DestinoFinal(BusquedaModels oModel)
        {
            string oDiaActual = "";
            int oHoras=0;
            List<ListaTiempoEstimado> oLista = new List<ListaTiempoEstimado>();
            oLista.AddRange(oModel.ListaTiempo);

            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            var oHorasTrasncurridas = from dat in oModel.ListaTiempo
                                      select dat;

            if (oHorasTrasncurridas != null)
                oHoras = oHorasTrasncurridas.Sum(m => m.Horas);


            DateTime dateValue = DateTime.Now.AddHours(oHoras);
            oDiaActual = EntoEs(dateValue.DayOfWeek.ToString());

            CoberturaClass oCobertura = new CoberturaClass();

            string strOrigen = (from a in oModel.ListaSucursal
                                where a.Value == oModel.SucursalDestino
                                select a).Single().Text;

            oCobertura.Origen=strOrigen;

            if (oModel.LocalidadDestino != "" && oModel.LocalidadDestino != null && oModel.LocalidadDestino!="0")
            {
                oCobertura.Destino = (from a in oPar.TB_PAR_LOC_LOCALIDAD
                                     where a.PK_PAR_LOC_ID == Convert.ToInt32(oModel.LocalidadDestino)
                                     select a.FL_PAR_LOC_NOMBRE).Single().ToString();

                var oResult=from cob in oPar.TB_PAR_CLO_COBERTURA_LOCALIDAD
                            where cob.PK_PAR_LOC_ID==Convert.ToInt32(oModel.LocalidadDestino)
                            select cob;
                if (oResult!=null)
                {
                    if (oResult.Count()>0)
                    {
                        var oDato=oResult.ToList()[0];                        
                        oCobertura.Lunes=oDato.FL_PAR_CLO_LU;
                        oCobertura.Martes=oDato.FL_PAR_CLO_MA;
                        oCobertura.Miercoles=oDato.FL_PAR_CLO_MI;
                        oCobertura.Jueves=oDato.FL_PAR_CLO_JU;
                        oCobertura.Viernes=oDato.FL_PAR_CLO_VI;
                        oCobertura.Sabado=oDato.FL_PAR_CLO_SA;
                        oCobertura.Domingo=oDato.FL_PAR_CLO_DO;
                        oCobertura.Duracion = Convert.ToInt32(oDato.FL_PAR_CLO_TIEMPO_ESTIMADO);
                    }
                }
            }
            else if (oModel.ComunaDestino != "" && oModel.ComunaDestino != null && (oCobertura.Destino=="" || oCobertura.Destino==null))
            {
                oCobertura.Destino = (from a in oPar.TB_PAR_COM_COMUNA
                                     where a.PK_PAR_COM_ID == Convert.ToInt32(oModel.ComunaDestino)
                                     select a.FL_PAR_COM_NOMBRE).Single().ToString();
                
                var oResult = from cob in oPar.TB_PAR_COB_COBERTURA_COMUNA
                              where cob.PK_PAR_COM_ID == Convert.ToInt32(oModel.ComunaDestino)
                              select cob;
                if (oResult != null)
                {
                    if (oResult.Count() > 0)
                    {
                        


                        var oDato = oResult.ToList()[0];                        
                        oCobertura.Lunes = oDato.FL_PAR_COB_LU;
                        oCobertura.Martes = oDato.FL_PAR_COB_MA;
                        oCobertura.Miercoles = oDato.FL_PAR_COB_MI;
                        oCobertura.Jueves = oDato.FL_PAR_COB_JU;
                        oCobertura.Viernes = oDato.FL_PAR_COB_VI;
                        oCobertura.Sabado = oDato.FL_PAR_COB_SA;
                        oCobertura.Domingo = oDato.FL_PAR_COB_DO;
                        oCobertura.Duracion = Convert.ToInt32(oDato.FL_PAR_COB_TIEMPO_ESTIMADO);
                    }
                }
            }

            if ((oCobertura.Destino != "" && oCobertura.Destino != null) && (oCobertura.Lunes==true || oCobertura.Martes==true || oCobertura.Miercoles==true || oCobertura.Jueves==true || oCobertura.Viernes==true || oCobertura.Sabado==true || oCobertura.Domingo==true))
            {
                oLista.Add(fCobertura(DiaToInt(oDiaActual), Convert.ToBoolean(oCobertura.Lunes), Convert.ToBoolean(oCobertura.Martes), Convert.ToBoolean(oCobertura.Miercoles), Convert.ToBoolean(oCobertura.Jueves), Convert.ToBoolean(oCobertura.Viernes), Convert.ToBoolean(oCobertura.Sabado), Convert.ToBoolean(oCobertura.Domingo), oCobertura.Origen, oCobertura.Destino, oModel.ListaTiempo.Sum(m => m.Horas)));
                if (oCobertura.Lunes == true || oCobertura.Martes == true || oCobertura.Miercoles == true || oCobertura.Jueves == true || oCobertura.Viernes == true || oCobertura.Sabado == true || oCobertura.Domingo == true)
                    oLista.Add(new ListaTiempoEstimado { Linea = String.Format("Tiempo de Tránsito entre Sucursal {0} y Destino {1}", oCobertura.Origen, oCobertura.Destino), Horas = oCobertura.Duracion });
            }
            else
            {
                oLista.Clear();
                oLista.Add(new ListaTiempoEstimado { Linea = oCobertura.Destino + " Sin Cobertura", Horas = 0 });
            }
            return oLista;  
        }
        #endregion

        #region Procesar Busqueda
        public ActionResult  ProcesarBusqueda(BusquedaModels oModel)
        {            

            oModel.ListaTiempo = EntreSucursales(oModel);

            return PartialView("ProcesarBusqueda", oModel);
        }
        #endregion

        #region BuscarSucursalDestino
        public string BuscarSucursalDestino(BusquedaModels oModel)
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            string oDestino = "";

            if (oModel.LocalidadDestino != "" && oModel.LocalidadDestino != null)
            {
                var oSucursal = from suc in oPar.TB_PAR_CLO_COBERTURA_LOCALIDAD
                                where suc.PK_PAR_LOC_ID == Convert.ToInt32(oModel.LocalidadDestino)
                                select suc;
                if (oSucursal.Count() > 0)
                    oDestino = oSucursal.ToList()[0].PK_PAR_SUC_ID.ToString();
            }

            if (oDestino == "")
            {
                var oSucursal = from suc in oPar.TB_PAR_COB_COBERTURA_COMUNA
                                where suc.PK_PAR_COM_ID == Convert.ToInt32(oModel.ComunaDestino)
                                select suc;
                if (oSucursal.Count() > 0)
                    oDestino = oSucursal.ToList()[0].PK_PAR_SUC_ID.ToString();
            }
            return oDestino;
        }
        #endregion

        #region Procesar Destino
        public ActionResult ProcesarDestino(BusquedaModels oModel)
        {

            oModel = FunctionProcesarBusqueda(oModel);
            
            return PartialView("ProcesarBusqueda", oModel);
        }
        #endregion

        #region FunctionProcesarBusqueda
        public BusquedaModels FunctionProcesarBusqueda(BusquedaModels oModel)
        {
            if (oModel.ComunaDestino == null || oModel.ComunaDestino == "")
            {
                oModel.ListaTiempo.Add(new ListaTiempoEstimado { Linea = String.Format("Debe Ingresar Comuna", oModel.ComunaDestino), Horas = 0 });
            }
            else
            {
                oModel.SucursalDestino = BuscarSucursalDestino(oModel);
                oModel.ListaTiempo = EntreSucursales(oModel);
                oModel.ListaTiempo = DestinoFinal(oModel); //va la lista incluida, añade nuevas al final
            }
            return oModel;
        }
        #endregion

        #region Función fCobertura
        public ListaTiempoEstimado fCobertura (int oDia,bool oLu,bool oMa, bool oMi,bool oJu,bool oVi,bool oSa,bool oDo,string strOrigen,string strDestino,int intHorasAcumuladas)
        {
            string oMensaje="";
            int oHoras=0;            
            ListaTiempoEstimado oLista = new ListaTiempoEstimado(); 
            
            if (oDia==6)//Domingo
                oDia=0;//Lunes
            else
                oDia++;//Suma 1 Día


            bool[] oCobertura = {oLu,oMa,oMi,oJu,oVi,oSa,oDo };

            if (oLu == false && oMa == false && oMi == false && oJu == false && oVi == false && oSa == false && oDo == false)
            {
                oMensaje = strDestino + " Sin Cobertura";
                oHoras = 0;
            }
            else
            {
                var oCont=0;
                var oEstimada = oDia;
                var oDiaCobertura = 0;
                while (oCont == 0)
                {
                    oHoras += 24;
                    if (oCobertura[oEstimada] == true) { oCont = 1; oDiaCobertura = oEstimada; };                       
                    if (oEstimada == 6) oEstimada = 0; else oEstimada++;
                    if (oHoras > 168) oCont = 1;//En Caso de Error, evita loop infinito.
                }
                oMensaje = "En Bodega, salida de " + strOrigen + " con destino " + strDestino + " el día " + DiaToStr(oDiaCobertura) + " " + DateTime.Now.AddHours(oHoras + intHorasAcumuladas).ToString("dd-MM-yyyy"); ;                                
            }

            

            oLista.Linea=oMensaje;
            oLista.Horas=oHoras;
            return (oLista);
        }
        #endregion

        #region EnToEs Traduce Inglés a Español Día
        public string EntoEs(string oValor)
        {
            string oResult = "";
            oResult = oValor;
            string[] DiasEs = { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" };
            string[] DiasEn = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            var c=0;
            foreach (var Dia in DiasEs){
                 oResult = oResult.Replace(DiasEn[c],Dia);
                 c++;
            }
            return oResult;
        }
        #endregion

        #region DiaToInt, Traduce un día (Lunes) en su valor númerico, ejemplo 0:Lunes, 1:Martes
        public int DiaToInt(string oValor)
        {            
            string[] DiasEs = { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" };            
            var c = 0;
            int oSwitch = 0;
            foreach (var Dia in DiasEs)
            {
                if (oValor == Dia)
                    oSwitch = 1;                
                if (oSwitch == 0)
                    c++;
            }
            return c;
        }
        #endregion

        #region DiaToStr Inverso de DiaToInt
        public string DiaToStr(int oValor)//0 Lunes //6 Domingo
        {
            string[] DiasEs = { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" };                       
            return DiasEs[oValor];
        }
        #endregion

        #region DropDownComuna
            public ActionResult DropDownComuna(string idRegion)
            {
                BusquedaModels oModel = new BusquedaModels();
                if (ValidationController.IsNumeric(idRegion) == true)
                {

                    LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
                    oModel.ListaComuna = from com in oPar.TB_PAR_COM_COMUNA
                                         join prv in oPar.TB_PAR_PRV_PROVINCIA on com.PK_PAR_PRV_ID equals prv.PK_PAR_PRV_ID
                                         where prv.PK_PAR_REG_ID == Convert.ToInt32(idRegion)
                                         orderby com.FL_PAR_COM_NOMBRE
                                         select new SelectListItem
                                         {
                                             Text = com.FL_PAR_COM_NOMBRE,
                                             Value = com.PK_PAR_COM_ID.ToString()
                                         };
                }
                else
                {
                    List<SelectListItem> oListaBlancoComuna = new List<SelectListItem>();                    
                    oListaBlancoComuna.Add(new SelectListItem { Text = "-- Seleccione Comuna --", Value = "" });
                    oModel.ListaBlancoComuna = oListaBlancoComuna;
                }
                return PartialView("_DropDownComuna", oModel);
            }      
        #endregion

        #region DropDownLocalidad
        public ActionResult DropDownLocalidad(string idComuna)
        {
            BusquedaModels oModel = new BusquedaModels();
            if (ValidationController.IsNumeric(idComuna) == true)
            {
                LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

                oModel.ListaLocalidad = from loc in oPar.TB_PAR_LOC_LOCALIDAD
                                        where loc.PK_PAR_COM_ID == Convert.ToInt32(idComuna)
                                        orderby loc.FL_PAR_LOC_NOMBRE
                                        select new SelectListItem
                                        {
                                            Text = loc.FL_PAR_LOC_NOMBRE,
                                            Value = loc.PK_PAR_LOC_ID.ToString()
                                        };
            }
            else
            {                
                List<SelectListItem> oListaBlancoLocalidad = new List<SelectListItem>();                
                oListaBlancoLocalidad.Add(new SelectListItem { Text = "-- Seleccione Localidad (Opcional) --", Value = "" });
                oModel.ListaBlancoLocalidad = oListaBlancoLocalidad;
            }
            return PartialView("_DropDownLocalidad", oModel);
        }
        #endregion


        //BuscarOT        
        #region BuscarOT
        [Authorize]
        public ActionResult BuscarOT(string OTP, string OTD)
        {
            ValidationController oValidation = new ValidationController();
            BuscarOTModels oModel = new BuscarOTModels();
            List<SelectListItem> oTipoReferencia = new List<SelectListItem>();


            oModel.ListaTipoReferencia = oTipoReferencia;
            if (oValidation.isDecimal(OTP) && oValidation.isDecimal(OTD))
            {
                oModel.OT = OTP + "-" + OTD;
            }
            return View(oModel);
        }
        #endregion

        #region BuscarBulto
        [Authorize]
        public ActionResult BuscarBulto(BuscarOTModels oModel)
        {

            List<HtmlString> oError = new List<HtmlString>();

            if (oModel.Referencia == "" || oModel.Referencia==null)
            {
                oModel = TempData["ModelBuscar"] as BuscarOTModels;
                TempData["ModelBuscar"] = oModel;
            }
            else
            {
                oModel.GetDatosByReferencia();
                if (oModel.OTP == 0)
                {
                    oError.Add(new HtmlString("La referencia ingresada no existe"));
                    oModel.ListaErrores = oError;
                    return PartialView("_OTFinalizada", oModel);
                }

                ValidationController oValidation = new ValidationController();
                //var oResult = oValidation.TransformCodigoGenericoOTPOTD(oModel.OT);
                oModel.GetDatosAnular(oModel.OTP, oModel.OTD);
                oModel.GetLatLng(oModel.OTP, oModel.OTD);
                oModel.GetBultos(oModel.OTP, oModel.OTD);
                oModel.GetReferencias(Convert.ToInt32(oModel.DatosAnular.PK_PAR_SER_ID));
                oModel.GetListaESA();
                oModel.GetListaEEX();
                oModel.GetListaEntrega(oModel.OTP, oModel.OTD);
                oModel.GetListaDevolucion(oModel.OTP, oModel.OTD);
                oModel.GetObservacion(oModel.OTP,oModel.OTD);



                TempData["ModelBuscar"] = oModel;
            }

            return PartialView("_CargaDatosBulto", oModel);
        }

        [Authorize]
        public ActionResult BuscarBultoFromDetalle(string referencia)
        {
            ValidationController oValidation = new ValidationController();
            BuscarOTModels oModel = new BuscarOTModels();
            oModel.Referencia = referencia;
            oModel.RutCliente = oValidation.GetRutActiveUser().ToString();


            List<HtmlString> oError = new List<HtmlString>();

            
            oModel.GetDatosByReferencia();
            if (oModel.OTP == 0)
            {
                oError.Add(new HtmlString("La referencia ingresada no existe"));
                oModel.ListaErrores = oError;
                return PartialView("_OTFinalizada", oModel);
            }

           
            //var oResult = oValidation.TransformCodigoGenericoOTPOTD(oModel.OT);
            oModel.GetDatosAnular(oModel.OTP, oModel.OTD);
            oModel.GetBultos(oModel.OTP, oModel.OTD);
            oModel.GetReferencias(Convert.ToInt32(oModel.DatosAnular.PK_PAR_SER_ID));
            oModel.GetListaESA();
            oModel.GetListaEEX();
            oModel.GetListaEntrega(oModel.OTP, oModel.OTD);
            oModel.GetListaDevolucion(oModel.OTP, oModel.OTD);
            oModel.GetObservacion(oModel.OTP, oModel.OTD);



            TempData["ModelBuscar"] = oModel;
            

            return PartialView("_CargaDatosBulto", oModel);
        }
        #endregion

        #region Manifiesto
        public ActionResult Manifiesto()
        {
            BuscarManifiestoModels oModel = new BuscarManifiestoModels();
            oModel.GetListaBuscarPor();
            oModel.GetListaCreadoPor();
            return View(oModel);
        }
        #endregion

        #region ListaCreadoPor
        public ActionResult ListaCreadoPor(int CreadoPor)
        {
            BuscarManifiestoModels oModel = new BuscarManifiestoModels();
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from vi in oFol.VI_FOL_MAL_MANIFIESTO_LISTA
                         where vi.PK_PAR_USU_RUT == CreadoPor
                         orderby vi.FL_FOL_MAN_FECHA_CREACION descending
                         select vi;

            oModel.ListaManifiestos = oDatos;
            return PartialView("_ListaManifiestos", oModel);
        }
        #endregion

        #region ListaFecha
        public ActionResult ListaFecha(string Fecha)
        {

            DateTime Fecha2 = new DateTime(Convert.ToInt32(Fecha.Substring(6,4))
                ,Convert.ToInt32(Fecha.Substring(3,2))
                ,Convert.ToInt32(Fecha.Substring(0,2)));

            BuscarManifiestoModels oModel = new BuscarManifiestoModels();
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from vi in oFol.VI_FOL_MAL_MANIFIESTO_LISTA
                         where 
                         vi.FL_FOL_MAN_FECHA_CREACION>=new DateTime(Fecha2.Year,Fecha2.Month,Fecha2.Day) &&
                         vi.FL_FOL_MAN_FECHA_CREACION<=new DateTime(Fecha2.Year,Fecha2.Month,Fecha2.Day,12,59,59) 
                         select vi;

            oModel.ListaManifiestos = oDatos;
            oModel.BuscarPorFecha = true;
            return PartialView("_ListaManifiestos", oModel);
        }
        #endregion

        #region ListaNumero
        public ActionResult ListaNumero(decimal Numero)
        {
            BuscarManifiestoModels oModel = new BuscarManifiestoModels();
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from vi in oFol.VI_FOL_MAL_MANIFIESTO_LISTA
                         where vi.PK_FOL_MAN_ID == Numero
                         select vi;

            oModel.ListaManifiestos = oDatos;
            return PartialView("_ListaManifiestos", oModel);
        }
        #endregion

        #region DescargarExcelMan
        public ActionResult DescargarExcelMan(string FECINI)
        {

            //DateTime FechaInicial = Convert.ToDateTime(FECINI);
            //DateTime FechaFinal = Convert.ToDateTime(FECTER);

            string fecha1 = FECINI.Substring(6, 4) + FECINI.Substring(3, 2) + FECINI.Substring(0, 2);
            

            MemoryStream Stream = new MemoryStream();
            var workbook = new XLWorkbook();
            //var worksheet = workbook.Worksheets.Add("Planilla Transporte");


            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            oFol.CommandTimeout = 600;

            var oDatos = (from exc in oFol.PR_FOL_MRC_MANIFIESTO_RUTA_CLIENTE_EXCEL(fecha1).AsEnumerable() select exc).ToList();


            var oResult = oDatos.ToDataSet();

            workbook.Worksheets.Add(oResult);

            workbook.Worksheet(1).Name = "Manifiestos " + fecha1;


            workbook.SaveAs(Stream);

            Response.ClearHeaders();
            Response.ClearContent();
            Response.Clear();

            Response.AddHeader("Content-Type", "application/vnd.ms-excel.12");

            Response.AddHeader("Content-Disposition", "attachment; filename=\"InformeManifiestos_" + fecha1 + ".xlsx\"");

            Stream.WriteTo(Response.OutputStream);
            Stream.Close();
            Response.Flush();
            Response.End();

            return View();
        }
        #endregion
    }    
}
