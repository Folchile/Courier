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
using MvcReporteViajes.Models;
using Courier.Controllers;


using DocumentFormat.OpenXml;
using ClosedXML.Excel;
using System.Net.Mime;
using System.Data;


namespace MvcReporteViajes.Controllers
{

    public static class DataSetExtension
    {

        public static DataSet ToDataSet<T>(this List<T> list)
        {

            Type type = typeof(T);

            DataSet ds = new DataSet();

            DataTable dt = new DataTable(type.Name);

            ds.Tables.Add(dt);



            var propertyInfos = type.GetProperties().ToList();



            //For each property of generic List (T), add a column to table

            propertyInfos.ForEach(propertyInfo =>
            {

                Type columnType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

                dt.Columns.Add(propertyInfo.Name, columnType);

            });



            //Visit every property of generic List (T) and add each value to the data table 

            list.ForEach(item =>
            {

                DataRow row = dt.NewRow();

                propertyInfos.ForEach(

                                       propertyInfo =>

                                       row[propertyInfo.Name] = propertyInfo.GetValue(item, null) ?? DBNull.Value

                                     );

                dt.Rows.Add(row);

            });



            //Return the dataset

            return ds;

        }

    }
    
    
    
    
    public class IngresosController : Controller
    {
        //
        // GET: /Ingresos/

        [Authorize]
        public ActionResult IngresoReportes(CabeceraFoliosModels cModel)
        {
            ValidationController oValidation = new ValidationController();
            cModel.SoloIngresoRutas = oValidation.TieneRolByName("Ingreso Control Rutas");
            cModel.AdminReport = TieneRolByName("Admin.Reporte Viajes"); 
            return View("IngresoReportes",cModel);
        }

        [Authorize]
        public ActionResult IngresoFolios(IngresoFoliosModels oModel)
        {
            oModel.GetListaVehiculos();
            return View("IngresoFolios",oModel);
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



        #region GetSucursalIDfromUser
        public int GetSucursalIDfromActiveUser()
        {
            int oUser = GetRutActiveUser();
            if (oUser != 0)
            {
                try
                {

                    Courier.Models.LinqBD_PARDataContext oFol = new Courier.Models.LinqBD_PARDataContext();
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




        public ActionResult MostrarDatosVehiculo(CabeceraFoliosModels oModel)
        {
            Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext oFolio = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();
            var oDatos = from fol in oFolio.TB_REP_CAB_CABECERA_FOLIOS
                          where fol.PK_REP_CAB_FOLIO == Convert.ToInt32(oModel.FOLIO)
                          select fol;

            //oModel.MMOUSUARIO = 1;
            //oModel.ESTADO = oDatos.TP_REP_ECF_ESTADO_CABECERA_FOLIO.FL_REP_ECF_DESCRIPCION;
            //oModel.TIPO = oModel.DatosVehiculo(oDatos.PK_PAR_TRA_ID);


            if (oDatos.Count() > 0)
            {
                foreach (var oDat in oDatos)
                {
                    oModel.ESTADO = oDat.TP_REP_ECF_ESTADO_CABECERA_FOLIO.FL_REP_ECF_DESCRIPCION;
                    oModel.TIPO = oModel.DatosVehiculo(oDat.PK_PAR_TRA_ID);
                    if (oDat.PK_PAR_USU_RUT != null)
                    {
                        oModel.USUARIOACTUAL = oModel.DatosUsuario(Convert.ToInt32(oDat.PK_PAR_USU_RUT));
                    }
                }
            }
            return PartialView("CabeceraVehiculo", oModel);
        }


        public ActionResult MostrarDatosFondos(int FOLIO, CabeceraFoliosModels cModel)
        {
            Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext oFolio = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();
            var oDatos = (from fol in oFolio.TB_REP_CAB_CABECERA_FOLIOS
                          where fol.PK_REP_CAB_FOLIO == FOLIO
                          select fol).Single();
            cModel.FARENDIR = Convert.ToInt32(oDatos.FL_REP_CAB_FONDOS_A_RENDIR);
            cModel.TRENDIDO = Convert.ToInt32(oDatos.FL_REP_CAB_TOTAL_RENDIDO);
            cModel.MDEVOLRECIB = Convert.ToInt32(oDatos.FL_REP_CAB_MONTO_DEVOL_REC);
            return PartialView("Fondos", cModel);
        }


        //public class RetornoDatosRampla
        //{
        //    public string RPLA { get; set; }
        //    public string KG { get; set; }
        //    public string M3 { get; set; }
        //}


        //public JsonResult BuscarDatosRampla(CabeceraFoliosModels oModel)
        //{
        //    LinqBD_REP_REPORTE_VIAJESDataContext oRamplas = new LinqBD_REP_REPORTE_VIAJESDataContext();
        //    RetornoDatosRampla oDatostramplas = (from r in oRamplas.TB_REP_RAM_RAMPLAS
        //                                         where r.PK_PAR_CAB_FOLIO == oModel.FOLIO
        //                                         select new RetornoDatosRampla
        //                                         {
        //                                             //KG = r.FL_CTV_MOV_CAPACIDAD_KGS.ToString(),
        //                                             //M3 = r.FL_CTV_MOV_CAPACIDAD_M3.ToString()
        //                                         }).Single();



        //    return Json(oDatostramplas, JsonRequestBehavior.AllowGet);
        //}


        
               
        
        public class RetornoDatosRampla
        {
            public int FKG { get; set; }
            public int FM3 { get; set; }
        }


        public class RetornaFolioUsuario
        {
            public Boolean ExisteFolio { get; set; }
            public Boolean MismoUsuario { get; set; }
            public String UsuFolio { get; set; }
            public Boolean FolioActivo { get; set; }
        }


        public class RetornaCodigo
        {
            public Boolean ExisteCodigo { get; set; }
            public Boolean ExisteRut { get; set; }
        }

        public class RetornaExisteAsistente
        {
            public Boolean ExisteAsistente { get; set; }
        }


        public JsonResult BuscarDatosRampla(CabeceraFoliosModels oModel)
        {
            
            RetornoDatosRampla oResultado = new RetornoDatosRampla();
            Courier.Models.LinqBD_PARDataContext oRamplas = new Courier.Models.LinqBD_PARDataContext();
            var oDatosRampla = from r in oRamplas.TB_PAR_TRA_TRANSPORTE
                                              where r.PK_PAR_TRA_ID == Convert.ToInt32(oModel.RAMPLA)
                                              select r;
                                               //{
                                               //    FKG = Convert.ToInt32(r.FL_PAR_TRA_CAPACIDAD_KGS),
                                               //    FM3 = Convert.ToInt32(r.FL_PAR_TRA_CAPACIDAD_M3)
                                               //}).Single();
            if (oDatosRampla.Count() > 0)
            {
                
                foreach (var odRampla in oDatosRampla)
                {
              
                oResultado.FKG = Convert.ToInt32(odRampla.FL_PAR_TRA_CAPACIDAD_KGS);
                oResultado.FM3 = Convert.ToInt32(odRampla.FL_PAR_TRA_CAPACIDAD_M3);
                }
            }
            else
            {
                oResultado.FKG = 0;
                oResultado.FM3 = 0;
            }

            return Json(oResultado, JsonRequestBehavior.AllowGet);
        }


        public JsonResult BuscarDatosRamplaAntiguos(CabeceraFoliosModels oModel)
        {
            Courier.Models.LinqBD_PARDataContext oRamplas = new Courier.Models.LinqBD_PARDataContext();
            RetornoDatosRampla oDatosRampla = (from r in oRamplas.TB_PAR_TRA_TRANSPORTE
                                               where r.PK_PAR_TRA_ID == Convert.ToInt32(oModel.RAMPLA)
                                               select new RetornoDatosRampla
                                               {
                                                   FKG = Convert.ToInt32(r.FL_PAR_TRA_CAPACIDAD_KGS),
                                                   FM3 = Convert.ToInt32(r.FL_PAR_TRA_CAPACIDAD_M3)
                                               }).Single();
            return Json(oDatosRampla, JsonRequestBehavior.AllowGet);
        }


        public ActionResult TotalGridGastos(int FOLIO, DetalleGastosModels oModel)
        {
            CabeceraFoliosModels cModel = new CabeceraFoliosModels();
            Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext oFolio = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();
            var oDatos = from fol in oFolio.VI_REP_TOTAL_GRILLA_GASTOS
                          where fol.PK_REP_CAB_FOLIO == FOLIO
                          select fol;

            if (oDatos.Count() == 1)
            {
                foreach (var oFile in oDatos)
                {
                    oModel.TAloj = Convert.ToInt32(oFile.TOTALOJAMIENTO);
                    oModel.TCarDes = Convert.ToInt32(oFile.TOTCARGADESC);
                    oModel.TEstac = Convert.ToInt32(oFile.TOTESTACIONAMIENTO);
                    oModel.TIva = Convert.ToInt32(oFile.TOTIVA);
                    oModel.TMant = Convert.ToInt32(oFile.TOTMANTENCION);
                    oModel.TOtros = Convert.ToInt32(oFile.TOTOTROS);
                    oModel.TPeaje = Convert.ToInt32(oFile.TOTPEAJE);
                    oModel.TViat = Convert.ToInt32(oFile.TOTVIATICO);
                    oModel.TTotal = Convert.ToInt32(oFile.TOTTOTAL);
                }
            }
            else
            {
                oModel.TAloj = 0;
                oModel.TCarDes = 0;
                oModel.TEstac = 0;
                oModel.TIva = 0;
                oModel.TMant = 0;
                oModel.TOtros = 0;
                oModel.TPeaje = 0;
                oModel.TViat = 0;
                oModel.TTotal = 0;
            }
            return PartialView("_totalizaGastos", oModel);

        }

        
        #region TieneRolByName
        public bool TieneRolByName(string RolName)
        {
            Courier.Models.LinqBD_PARDataContext oPar = new Courier.Models.LinqBD_PARDataContext();
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


        public string DatosConductor(int rutcon)
        {
            
            string dcond="";
            CabeceraFoliosModels oModel = new CabeceraFoliosModels();
            Courier.Models.LinqBD_PARDataContext Ccond = new Courier.Models.LinqBD_PARDataContext();
            var cDatos = from c in Ccond.TB_PAR_CON_CONDUCTOR
                         where c.PK_PAR_CON_RUT == rutcon
                         select c;

            if (cDatos.Count() > 0)
            {
                
                foreach (var odcon in cDatos)
                {
                    dcond = odcon.FL_PAR_CON_CODIGO + "-" + odcon.FL_PAR_CON_NOMBRE;
                }
            }
            
            return dcond;
        }


        public string DatosAsistte(int rutasi)
        {
            string dcond = "";
            CabeceraFoliosModels oModel = new CabeceraFoliosModels();
            Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext Casis = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();
            var cDatos = from c in Casis.TB_REP_ASI_ASISTENTES
                         where c.PK_REP_ASI_RUT == rutasi
                         select c;

            if (cDatos.Count() > 0)
            {

                foreach (var odcon in cDatos)
                {
                    dcond = odcon.FL_REP_ASI_CODIGO + "-" + odcon.FL_REP_ASI_NOMBRES+ " "+odcon.FL_REP_ASI_APELLIDOS;
                }
            }

            return dcond;
        }

        public string DatosDesp(int rutdesp)
        {
            string dcond = "";
            CabeceraFoliosModels oModel = new CabeceraFoliosModels();
            Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext Casis = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();
            var cDatos = from c in Casis.TB_REP_DES_DESPACHADORES
                         where c.PK_REP_DES_RUT == rutdesp
                         select c;

            if (cDatos.Count() > 0)
            {

                foreach (var odcon in cDatos)
                {
                    dcond = odcon.FL_REP_DES_NOMBRES + " " + odcon.FL_REP_DES_APELLIDOS;
                }
            }

            return dcond;
        }


        public ActionResult MostrarCabecera(CabeceraFoliosModels oModel)
        {
            
            
            
            //DetalleCombustibleModels cModel = new DetalleCombustibleModels();
            //cModel.FOLCOMB = oModel.FOLIO;
            Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext oFolio = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();
            var oDatos = (from fol in oFolio.TB_REP_CAB_CABECERA_FOLIOS
                          where fol.PK_REP_CAB_FOLIO == Convert.ToInt32(oModel.FOLIO)
                          select fol).Single();

            oModel.NUMBULTOS = Convert.ToInt32(oDatos.FL_REP_CAB_BULTOS_TRANSPORTADOS);
            oModel.KGTRANS = oDatos.FL_REP_CAB_KILOS_TRANSPORTADOS.ToString();
            oModel.CONDUCTOR = Convert.ToInt32(oDatos.PK_PAR_CON_RUT);
            oModel.DESPACHADOR= Convert.ToInt32(oDatos.PK_REP_DES_RUT);
            oModel.ASISTENTE= Convert.ToInt32(oDatos.PK_REP_ASI_RUT);
            oModel.AREA = Convert.ToInt32 (oDatos.PK_REP_ARE_ID);
            oModel.ZONA = Convert.ToInt32(oDatos.PK_REP_ZON_ID);
            oModel.SERVICIO=Convert.ToInt32 (oDatos.PK_REP_SER_ID);
            oModel.OBSERVACIONCAB = oDatos.FL_REP_CAB_OBSERVACION;
            oModel.CONDUCTORTEXT = DatosConductor(Convert.ToInt32(oDatos.PK_PAR_CON_RUT));
            oModel.ASISTENTETEXT = DatosAsistte(Convert.ToInt32(oDatos.PK_REP_ASI_RUT));
            oModel.DESPACHADORTEXT = DatosDesp(Convert.ToInt32(oDatos.PK_REP_DES_RUT));
            oModel.REFERENCIA = oDatos.FL_REP_CAB_REFERENCIA;
            oModel.Rescatarmpla(); 
            oModel.GetListaRamplas();
            oModel.GetListaServicios();
            oModel.GetListaAreas();
            oModel.AdminisReporteViajes = TieneRolByName("Admin.Reporte Viajes");
            //if (oModel.AdminisReporteViajes == true)
            //{
                oModel.GetListaConductores();
                oModel.GetListaAsistentes();
                oModel.GetListaDespachadores();
            //}
            //else
            //{
            //    int oSuc = GetSucursalIDfromActiveUser();
            //    oModel.GetListaConductoresXSuc(oSuc);
            //    oModel.GetListaAsistentesXSuc(oSuc);
            //    oModel.GetListaDespachadoresXSuc(oSuc);
            //}
            //oModel.GetListaBlancoZonas();
            int idA = 0;
            if (oDatos.PK_REP_ZON_ID != null)
                idA = Convert.ToInt32(oDatos.PK_REP_ARE_ID);            

            oModel.GetListaZonas(idA);
            //if (oDatos.PK_REP_ZON_ID == null)
            //{
            //    oModel.GetListaBlancoZonas();
            //}
            //else
            //{
            //    oModel.GetListaZonas(Convert.ToInt32(oDatos.PK_REP_ZON_ID));
            //}
            //oModel.GetListaZonas(Convert.ToInt32(oDatos.PK_REP_ZON_ID));
            return PartialView("CabeceraFolios", oModel);
        }

        #region LstTransAsociados
        public ActionResult LstTransAsociados(VehiculosModels vModel)
        {
            vModel.getListaTransAsoc();
            return PartialView("_LstTransportesAsociados", vModel);
        }
        #endregion

        #region LstSucAsociadas
        public ActionResult LstSucAsociadas(VehiculosModels vModel)
        {
            vModel.GetListaSucAsignadas();
            return PartialView("_LstSucursalesAsignadas", vModel);
        }
        #endregion

        #region LstSucNoAsociadas
        public ActionResult LstSucNoAsociadas(VehiculosModels vModel)
        {
            vModel.GetListaSucNoAsignadas();
            return PartialView("_LstSucNoAsignadas", vModel);
        }
        #endregion

        #region LstTransPorSuc
        public ActionResult LstTransPorSuc(VehiculosModels vModel)
        {
            vModel.getListaTransPorSucursal();
            return PartialView("_ListaTransportesPorSuc", vModel);
        }
        #endregion

        public ActionResult RetornoValidarCodAsis(int CODIGO)
        {
            RetornaCodigo cResult = new RetornaCodigo();
            Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext oFolios = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();
            var oDatos = from f in oFolios.TB_REP_ASI_ASISTENTES
                         where f.FL_REP_ASI_CODIGO == CODIGO
                         select f;
            cResult.ExisteCodigo = false;
            if (oDatos.Count() == 1)
            {
                cResult.ExisteCodigo = true;
            } 
            return Json(cResult, JsonRequestBehavior.AllowGet); 
        }

        
        public ActionResult RetornoValidaAsistente(int Rut)
        {
            RetornaExisteAsistente aResult = new RetornaExisteAsistente();
            Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext oFolios = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();
            var oDatos = from f in oFolios.TB_REP_ASI_ASISTENTES
                         where f.PK_REP_ASI_RUT == Rut
                         select f;
            aResult.ExisteAsistente = false;
            if (oDatos.Count() == 1)
            {
                aResult.ExisteAsistente = true;
            }
            
            return Json(aResult, JsonRequestBehavior.AllowGet);
        }


        public ActionResult RetornoValidaConductor(ConductoresModels cModel)
        {

            int rutcond;
            rutcond = Convert.ToInt32(cModel.Rut.Substring(0, cModel.Rut.Length - 2));
            RetornaCodigo cResult = new RetornaCodigo();
            cResult.ExisteRut = false;
            cResult.ExisteCodigo = false;
            Courier.Models.LinqBD_PARDataContext oPar = new Courier.Models.LinqBD_PARDataContext();
            var oDatos = from f in oPar.TB_PAR_CON_CONDUCTOR
                         where f.PK_PAR_CON_RUT == rutcond 
                         select f;
            if (oDatos.Count() > 0)
            {
                cResult.ExisteRut = true;
            }
            var cDatos = from f in oPar.TB_PAR_CON_CONDUCTOR
                         where f.FL_PAR_CON_CODIGO == cModel.CODIGO && cModel.CODIGO > 0
                             select f;
            if (cDatos.Count() > 0)
            {
                cResult.ExisteCodigo = true;
            }
            return Json(cResult, JsonRequestBehavior.AllowGet);
        }


        public ActionResult RetornoValidarFolio(int FOLIO)
        {
            CabeceraFoliosModels oModel = new CabeceraFoliosModels();
            oModel.AdminisReporteViajes = TieneRolByName("Admin.Reporte Viajes");
            RetornaFolioUsuario Resultado = new RetornaFolioUsuario();
            Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext oFolios = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();
            var oDatos = from f in oFolios.TB_REP_CAB_CABECERA_FOLIOS
                         where f.PK_REP_CAB_FOLIO == FOLIO
                         select f;
            if (oDatos.Count() == 1)
            {
                Resultado.ExisteFolio = true;
                Resultado.MismoUsuario = true;
                Resultado.UsuFolio="";
                foreach (var oFile in oDatos)
                {
                    Resultado.FolioActivo = true;
                    if (oFile.PK_REP_ECF_ID != 1)
                        Resultado.FolioActivo = false;
                    if (oFile.PK_PAR_USU_RUT != null)
                    {
                        Courier.Models.LinqBD_PARDataContext oPar = new Courier.Models.LinqBD_PARDataContext();
                        var oUsu = (from usu in oPar.TB_PAR_USU_USUARIO
                                    where usu.PK_PAR_USU_RUT == oFile.PK_PAR_USU_RUT
                                    select usu).Single();

                        Resultado.UsuFolio = oUsu.FL_PAR_USU_NOMBRES + " " + oUsu.FL_PAR_USU_APELLIDOS;
                        
                        
                        if (oFile.PK_PAR_USU_RUT == GetRutActiveUser())
                        {
                            Resultado.MismoUsuario = true;
                            //Resultado.UsuFolio="";
                        }
                        else
                        {
                            
                            //Courier.Models.LinqBD_PARDataContext oPar = new Courier.Models.LinqBD_PARDataContext();
                            //var oUsu = (from usu in oPar.TB_PAR_USU_USUARIO
                            //where usu.PK_PAR_USU_RUT == oFile.PK_PAR_USU_RUT
                            //select usu).Single(); 

                            //Resultado.UsuFolio = oUsu.FL_PAR_USU_NOMBRES + " " + oUsu.FL_PAR_USU_APELLIDOS;
                            Resultado.MismoUsuario = false;
                             
                        }
                    }
                    
                }
                if (oModel.AdminisReporteViajes == true)
                    Resultado.MismoUsuario = true;
                return Json(Resultado, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Resultado.ExisteFolio = false;
                Resultado.MismoUsuario = false;
                return Json(Resultado, JsonRequestBehavior.AllowGet);
            }
        }

        
        public ActionResult dcontrolCombustible(CabeceraFoliosModels oModel)
        {
            oModel.RescataDatosControlCombustible();
            return PartialView("ControlCombustible", oModel);
        }

        #region IndexTransp
        public ActionResult IndexTransp(ConductoresModels cModel)
        {
            VehiculosModels vModel = new VehiculosModels();
            vModel.GetLstSucrsales();
            vModel.RutCondAux = cModel.RUTAUX;
            vModel.NombreConductor = cModel.NOMBRECONDUCTOR;
            return PartialView("IndexTransportes",vModel);
        }
        #endregion

        #region IndexSucursales
        public ActionResult IndexSucursales(VehiculosModels vModel)
        {
            return PartialView("IndexSucursales", vModel);
        }
        #endregion

        public ActionResult dcontrolRutas(CabeceraFoliosModels oModel)
        {
            oModel.RescataDatosControlRuta();
            return PartialView("ControlRutas", oModel);
        }


        public ActionResult dfoliosActivos(CabeceraFoliosModels oModel)
        {
            oModel.RescataFoliosActivos();
            return PartialView("CargaFoliosActivosSucursal", oModel);
        }


              

        public ActionResult drangoPatentes(CabeceraFoliosModels oModel)
        {
            oModel.RescataRangoPatentes();
            return PartialView("CargaRangoPatentes", oModel);
        }


        public ActionResult dcamestadofolio(CabeceraFoliosModels oModel)
        {
            oModel.RescataDatosFolio();
            return PartialView("DetalleFolioCambioEstado", oModel);
        }


        public ActionResult dReasignarFolio(CabeceraFoliosModels oModel)
        {
            oModel.RescataDatosFolio();
            return PartialView("DetalleFolioReasignacion", oModel);
        }



        public ActionResult CargadetAsistentes(CabeceraFoliosModels oModel)
        {
            oModel.RescataDatosAsistentes();
            return PartialView("AsignaAsistentes", oModel); 
        }


        public ActionResult dcontrolGastos(CabeceraFoliosModels oModel)
        {
            oModel.RescataDatosControlGastos();
            return PartialView("ControlGastos", oModel);
        }



        [Authorize]
        public ActionResult MantenedorAsistentes()
        {
            AsistentesModels oModel = new AsistentesModels();
            //oModel.RescataDatosAsistentes();
            return View("MantenedorAsistentes", oModel);
        }

        [Authorize]
        public ActionResult MantenedorConductores()
        {
            ConductoresModels oModel = new ConductoresModels();
            //oModel.RescataDatosAsistentes();
            return View("MantenedorConductores", oModel);
        }

        [Authorize]
        public ActionResult MantenedorVehiculos()
        {
            VehiculosModels oModel = new VehiculosModels();
            return View("MantenedorVehiculos", oModel);
        }

        [Authorize]
        public ActionResult MantenedorDespachadores()
        {
            DespachadorModels oModel = new DespachadorModels();
            return View("MantenedorDespachadores", oModel);
        }



        public ActionResult VerDetalleAsistentes(AsistentesModels oModel)
        {
            oModel.RescataDatosAsistentes();
            return PartialView("MostrarAsistentes", oModel);
        }

        public ActionResult VerDetalleConductores(ConductoresModels oModel)
        {
            oModel.RescataDatosConductores();
            return PartialView("MostrarConductores", oModel);
        }

        public ActionResult VerDetalleVehiculos(VehiculosModels oModel)
        {
            oModel.RescataDatosVehiculos();
            return PartialView("MostrarVehiculos", oModel);
        }



        public ActionResult VerDetalleDespachadores(DespachadorModels oModel)
        {
            oModel.RescataDatosDespachadores();
            return PartialView("MostrarDespachadores", oModel);
        }


        public ActionResult DetalleFolios(IngresoFoliosModels oModel)
        {
            oModel.RescatarDatosFolio();
            return PartialView("DetalleIngresoFolios", oModel);
        }

        [HttpPost]
        public ActionResult CargaFomularioDetalleGastos(DetalleGastosModels gModel)
        {
            DetalleGastosModels.Respuesta oResult = new DetalleGastosModels.Respuesta();
            //RetornoModels oRetorno = new RetornoModels();
            //oRetorno = dModel.AgregaDetalleCombustible();
            oResult = gModel.AgregaDetalleGastos();
            return Json(oResult, JsonRequestBehavior.AllowGet);
        }


        
        public ActionResult fnDesasociarMovil(VehiculosModels vModel)
        {
            RetornoModels oRetorno = new RetornoModels();
            oRetorno = vModel.DesasociarMovil();
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult ModificaDetalleRuta(DetalleRutaModels rModel)
        {
            DetalleRutaModels.Respuesta oResult = new DetalleRutaModels.Respuesta();
            oResult = rModel.GetModificaDetalleRuta();
            return Json(oResult, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult AgregaDetalleRuta(DetalleRutaModels rModel)
        {
            DetalleRutaModels.Respuesta oResult = new DetalleRutaModels.Respuesta();
            oResult = rModel.AgregaDetRuta();
            return Json(oResult, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult CargaFomularioDetalleCombustible(DetalleCombustibleModels dModel)
        {
            
            DetalleCombustibleModels.Respuesta oResult = new DetalleCombustibleModels.Respuesta();
            oResult = dModel.AgregaDetalleCombustible();
            return Json(oResult, JsonRequestBehavior.AllowGet);
        }


        public ActionResult DetalleGastos(DetalleGastosModels oModel)
        {
            //CabeceraFoliosModels cModel = new CabeceraFoliosModels();
            //cModel.RescataTotGrillaGastos();
            return PartialView("DetalleGastos", oModel);
        }

        public ActionResult DetalleCombustibles(DetalleCombustibleModels oModel)
        {
            return PartialView("DetalleCombustibles", oModel);
        }


        public ActionResult CreaAsistentes(AsistentesModels oModel)
        {
            oModel.GetListaSuc();
            return PartialView("AgregaAsistente", oModel);
        }


        public ActionResult AgregaConductor(ConductoresModels oModel)
        {
            oModel.GetListaSuc();
            return PartialView("AgregarConductor", oModel);
        }

        public ActionResult Form_AgregaTransporte(VehiculosModels oModel)
        {
            return PartialView("AgregaTransporte", oModel);
        }


        public ActionResult CreaDespachador(DespachadorModels oModel)
        {
            oModel.GetListaSuc();
            return PartialView("AgregaDespachador", oModel);
        }



        public ActionResult DetalleAsistentes(CabeceraFoliosModels oModel)
        {
            
            //oModel.GetListaAsistentes();
            oModel.AdminisReporteViajes = TieneRolByName("Admin.Reporte Viajes");
            //if (oModel.AdminisReporteViajes == true)
            //{
                oModel.GetListaAsistentes();
            //}
            //else
            //{
            //    int oSuc = GetSucursalIDfromActiveUser();
            //    oModel.GetListaAsistentesXSuc(oSuc);
            //}
            return PartialView("_otrosAsistentes", oModel);
        }


        public ActionResult LstVehiculosReasignacion(CabeceraFoliosModels oModel)
        {
            oModel.GetListaVehiculos2();
            return PartialView("_listaPatentesReasignacion", oModel);
        }


        public ActionResult CargaComunaOrigen(int idOrigen)
        {
            DetalleRutaModels rModel = new DetalleRutaModels();
            rModel.REGORIGEN = idOrigen;
            rModel.EDREGORIGEN = idOrigen;
            rModel.GesListaComunas(idOrigen);
            return PartialView("_DropDownComunas", rModel);
        }

        public ActionResult CargaZonas(int idZona)
        {
            CabeceraFoliosModels cModel = new CabeceraFoliosModels();
            //if (idZona == null)
            //    idZona = 0;
            cModel.ZONA = idZona;
            cModel.GetListaZonas(idZona);
            return PartialView("_DropDownZonas", cModel);
        }



        public ActionResult CargaComunaDestino(int idDestino)
        {
            DetalleRutaModels rModel = new DetalleRutaModels();
            rModel.REGDESTINO = idDestino;
            rModel.EDREGDESTINO = idDestino;
            rModel.GesListaComunas(idDestino);
            return PartialView("_DropDownComunasDestino", rModel);
        }


      
        public ActionResult DetalleRutas(int detFol)
        {
           
            DetalleRutaModels oModel = new DetalleRutaModels();
            Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext Odr = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();
            var gDatos = from da in Odr.PR_REP_RESCATA_ULT_KILOMETRAJE(detFol)
                         select da;
            foreach (var oFile in gDatos)
            {
                oModel.KMINICIAL = Convert.ToInt32(oFile.KMFINAL); 
            }
            oModel.GetListaBlanco();
            oModel.GesListaRegion();
            return PartialView("DetalleRuta", oModel);
        }

      
        [HttpPost]
        public ActionResult AnularFolio(FormCollection fForm)
        {

            CabeceraFoliosModels oModel = new CabeceraFoliosModels();
            RetornoModels oRetorno = new RetornoModels();
            oRetorno = oModel.AnulaFolio(Convert.ToInt32(fForm["id_folA"]));
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult AnularFoliodetalle(FormCollection fForm)
        {

            CabeceraFoliosModels oModel = new CabeceraFoliosModels();
            RetornoModels oRetorno = new RetornoModels();
            oRetorno = oModel.AnulaFolio(Convert.ToInt32(fForm["id_folAD"]));
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }
        



        [HttpPost]
        public ActionResult CerrarFolio(FormCollection fForm)
        {

            CabeceraFoliosModels oModel = new CabeceraFoliosModels();
            RetornoModels oRetorno = new RetornoModels();
            oRetorno = oModel.CierraFolio(Convert.ToInt32(fForm["id_folC"]));
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CargadetRangoFolios(int id_folR)
        {
            CabeceraFoliosModels oModel = new CabeceraFoliosModels();
            oModel.RescataDetalleRgoPatentes(id_folR);
            return PartialView("DetalleRangos", oModel);
         }


        [HttpPost]
        public ActionResult CerrarFoliodetalle(FormCollection fForm)
        {

            CabeceraFoliosModels oModel = new CabeceraFoliosModels();
            RetornoModels oRetorno = new RetornoModels();
            oRetorno = oModel.CierraFolio(Convert.ToInt32(fForm["id_folCD"]));
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult ActivarFoliodetalle(FormCollection fForm)
        {

            CabeceraFoliosModels oModel = new CabeceraFoliosModels();
            RetornoModels oRetorno = new RetornoModels();
            oRetorno = oModel.ActivaFolio(Convert.ToInt32(fForm["id_folCV"]));
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }

        
        [HttpPost]
        public ActionResult ActivarAsistente(FormCollection fForm)
        {
            AsistentesModels oModel = new AsistentesModels();
            RetornoModels oRetorno = new RetornoModels();
            oRetorno = oModel.ActivaAsistente(Convert.ToInt32(fForm["id_rutact"]),1);
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult InactivarAsistente(FormCollection fForm)
        {
            AsistentesModels oModel = new AsistentesModels();
            RetornoModels oRetorno = new RetornoModels();
            oRetorno = oModel.ActivaAsistente(Convert.ToInt32(fForm["id_rutinact"]), 2);
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult InactivarConductor(FormCollection fForm)
        {
            ConductoresModels cModel = new ConductoresModels();
            RetornoModels oRetorno = new RetornoModels();
            oRetorno = cModel.ActDesCond(Convert.ToInt32(fForm["irutcond"]), 2);
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult ActivarConductor(FormCollection fForm)
        {
            ConductoresModels cModel = new ConductoresModels();
            RetornoModels oRetorno = new RetornoModels();
            oRetorno = cModel.ActDesCond(Convert.ToInt32(fForm["irutconda"]), 1);
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult InactivarDespachador(FormCollection fForm)
        {
            DespachadorModels oModel = new DespachadorModels();
            RetornoModels oRetorno = new RetornoModels();
            oRetorno = oModel.CambiaestadoDespachador(Convert.ToInt32(fForm["rut_inacdesp"]), 2);
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult ActivarDespachador(FormCollection fForm)
        {
            DespachadorModels oModel = new DespachadorModels();
            RetornoModels oRetorno = new RetornoModels();
            oRetorno = oModel.CambiaestadoDespachador(Convert.ToInt32(fForm["rut_acdesp"]), 1);
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult ActuaCabecera(CabeceraFoliosModels oModel)
        {

            RetornoModels oRetorno = new RetornoModels();
            int oUser = GetRutActiveUser();
            int oSuc = GetSucursalIDfromActiveUser();
            oRetorno = oModel.ActualizaCabecera(oUser, oSuc);
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult InsertAsist(AsistentesModels oModel)
        {

            RetornoModels oRetorno = new RetornoModels();
            oRetorno = oModel.InsertAsistente();
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult insertConductor(ConductoresModels cModel)
        {

            RetornoModels oRetorno = new RetornoModels();
            oRetorno = cModel.insertaConductor();
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }


        public ActionResult InsertaVehiculo(VehiculosModels cModel)
        {

            RetornoModels oRetorno = new RetornoModels();
            oRetorno = cModel.GetinsertaVehiculo();
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }


        public ActionResult fnDesasignaSucursal(VehiculosModels cModel)
                {

            RetornoModels oRetorno = new RetornoModels();
            oRetorno = cModel.GetEliminaSucursal();
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AsignaSucursal(VehiculosModels cModel)
        {

            RetornoModels oRetorno = new RetornoModels();
            oRetorno = cModel.GetAsignaSucursal();
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult InsertDesp(DespachadorModels oModel)
        {

            RetornoModels oRetorno = new RetornoModels();
            oRetorno = oModel.InsertDespachador();
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult ActuaAsist(AsistentesModels oModel)
        {

            RetornoModels oRetorno = new RetornoModels();
            oRetorno = oModel.ActualizaAsistente();
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        public ActionResult editCond(ConductoresModels oModel)
        {

            RetornoModels oRetorno = new RetornoModels();
            oRetorno = oModel.ActualizaConductor();
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult editVehi(VehiculosModels oModel)
        {

            RetornoModels oRetorno = new RetornoModels();
            oRetorno = oModel.ActualizaVehiculo();
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult editDesp(DespachadorModels oModel)
        {

            RetornoModels oRetorno = new RetornoModels();
            oRetorno = oModel.ActualizaDespachador();
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult ActuaFondos(CabeceraFoliosModels oModel)
        {

            RetornoModels oRetorno = new RetornoModels();
            oRetorno = oModel.ActualizaFondos();
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult AgregaAsistentes(CabeceraFoliosModels oModel)
        {

            RetornoModels oRetorno = new RetornoModels();
            oRetorno = oModel.AgregaAsistente();
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult cReasignaPatente(CabeceraFoliosModels oModel)
        {

            RetornoModels oRetorno = new RetornoModels();
            int oUser = GetRutActiveUser();
            oRetorno = oModel.mReasignaPatente(oUser);
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult ActuaDetalleGastos(DetalleGastosModels oModel)
        {
            RetornoModels oRetorno = new RetornoModels();
            oRetorno = oModel.ActualizaDetalleGastos();
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult ActuaDetalleCombustible(DetalleCombustibleModels oModel)
        {
            RetornoModels oRetorno = new RetornoModels();
            oRetorno = oModel.ActualizaDetalleCombustible();
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult AsigFolios(IngresoFoliosModels oModel)
        {
            RetornoModels oRetorno = new RetornoModels();
            int oUser = GetRutActiveUser();
            oRetorno = oModel.AsignaFolios(oUser);
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult ElimFolios(IngresoFoliosModels oModel)
        {
            RetornoModels oRetorno = new RetornoModels();
            oRetorno = oModel.EliminaFolios();
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public string EliminarDetalleGastos(FormCollection oForm)
        {


            Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext oDc = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();
            var oQuery = from dcomb in oDc.TB_REP_DCG_DETALLE_CONTROL_GASTOS
                         where dcomb.PK_REP_DCG_ID == Convert.ToInt32(oForm["id_gastos"])
                         select dcomb;

            oDc.TB_REP_DCG_DETALLE_CONTROL_GASTOS.DeleteAllOnSubmit(oQuery);
         
            try
            {
                oDc.SubmitChanges();
                return ("Registro Eliminado");
            }
            catch (Exception e)
            {
                return ("<p>No fue posible eliminar la información</p><p>" + e.Message + "</p>");
            }

        }

        //[HttpPost]
        public ActionResult AsociarTransporte(VehiculosModels vModel)
        {
            ConductoresModels cModel = new ConductoresModels();
            RetornoModels oRetorno = new RetornoModels();
            cModel.IDPATENTE = vModel.IdAux;
            cModel.RUTAUX = vModel.RutCondAux;
            oRetorno = cModel.GetAsociaTransp();
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult DesasociarTransporte(ConductoresModels cModel)
        {
            RetornoModels oRetorno = new RetornoModels();
            oRetorno = cModel.DesTransp();
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }

        
        [HttpPost]
        public ActionResult AgregarTransporte(FormCollection oForm)
        {

            ConductoresModels cModel = new ConductoresModels();
            RetornoModels oRetorno = new RetornoModels();
            oRetorno = cModel.AgrTransp(Convert.ToInt32(oForm["id_astr"]), Convert.ToInt32(oForm["rtax"]));
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }
            
        


        //[HttpPost]
        //public string DesasociarTransporte(ConductoresModels cModel)
        //{
        //    Courier.Models.LinqBD_PARDataContext oTrc = new Courier.Models.LinqBD_PARDataContext();
        //    var oQuery = from ds in oTrc.TB_PAR_TRC_TRA_CON
        //                 where ds.PK_PAR_CON_RUT==cModel.RUTAUX && ds.PK_PAR_TRA_ID==cModel.IDPATENTE
        //                 select ds;
        //    oTrc.TB_PAR_TRC_TRA_CON.DeleteAllOnSubmit(oQuery);
        //    try
        //    {
        //        oTrc.SubmitChanges();
        //        return ("Registro Eliminado");
        //    }
        //    catch (Exception e)
        //    {
        //        return ("<p>No fue posible eliminar la información</p><p>" + e.Message + "</p>");
        //    }

        //}


        [HttpPost]
        public string EliminardetalleAsistente(FormCollection oForm)

        {

            Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext oDc = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();
            var oQuery = from dcomb in oDc.TB_REP_RFA_RELACION_FOLIO_ASISTENTES
                         where dcomb.PK_REP_RFA_ID == Convert.ToInt32(oForm["id_asis"])
                         select dcomb;
            oDc.TB_REP_RFA_RELACION_FOLIO_ASISTENTES.DeleteAllOnSubmit(oQuery);
            try
            {
                oDc.SubmitChanges();
                return ("Registro Eliminado");
            }
            catch (Exception e)
            {
                return ("<p>No fue posible eliminar la información</p><p>" + e.Message + "</p>");
            }

        }




        [HttpPost]
        public string EliminarDetalleRutas(FormCollection oForm)
        {


            Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext oDc = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();
            var oQuery2 = from relRuta in oDc.TB_REP_ROD_RELACION_ORIGEN_DESTINO
                          where relRuta.PK_REP_DCR_ID == Convert.ToInt32(oForm["id_ruta"])
                          select relRuta;
            oDc.TB_REP_ROD_RELACION_ORIGEN_DESTINO.DeleteAllOnSubmit(oQuery2);
            try
            {

                var oQuery = from druta in oDc.TB_REP_DCR_DETALLE_CONTROL_RUTA
                             where druta.PK_REP_DCR_ID == Convert.ToInt32(oForm["id_ruta"])
                             select druta;
                oDc.TB_REP_DCR_DETALLE_CONTROL_RUTA.DeleteAllOnSubmit(oQuery);

                try
                {
                    oDc.SubmitChanges();
                    return ("Registro Eliminado");
                }
                catch (Exception e)
                {
                    return ("<p>No fue posible eliminar la información error en relacion</p><p>" + e.Message + "</p>");
                }
            }
            catch (Exception e)
            {
                return ("<p>No fue posible eliminar la información error en cabecera</p><p>" + e.Message + "</p>");
            }
        }





        [HttpPost]
        public string EliminarDetalleComb(FormCollection oForm)
        {
            Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext oDc = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();
            var oQuery = from dcomb in oDc.TB_REP_DCC_DETALLE_CONTROL_COMBUSTIBLE
                         where dcomb.PK_REP_DCC_ID==Convert.ToInt32(oForm["id_detgastos"])
                         select dcomb;

            oDc.TB_REP_DCC_DETALLE_CONTROL_COMBUSTIBLE.DeleteAllOnSubmit(oQuery);

            try
            {
                oDc.SubmitChanges();
                return ("Registro Eliminado");
            }
            catch (Exception e)
            {
                return ("<p>No fue posible eliminar la información</p><p>" + e.Message + "</p>");
            }

        }


        [HttpPost]
        public ActionResult EditarDetalleGastos(FormCollection gForm)
        {
            DetalleGastosModels gModel = new DetalleGastosModels();
            Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext Odg = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();

            var gDatos = from dg in Odg.TB_REP_DCG_DETALLE_CONTROL_GASTOS
                         where dg.PK_REP_DCG_ID == Convert.ToInt32(gForm["Detallegstos_edit_id"])
                         select dg;
            foreach (var oFile in gDatos)
            {
                gModel.IDGTOS = oFile.PK_REP_DCG_ID;
                gModel.FECHADETGASTOS=oFile.FL_REP_DCG_FECHA;
                gModel.ALOJAMIENTO = Convert.ToInt32(oFile.FL_REP_DCG_ALOJAMIENTO);
                gModel.CARGADESCARGA = Convert.ToInt32(oFile.FL_REP_DCG_CARGA_DESCARGA);
                gModel.ESTACIONAMIENTO =Convert.ToInt32(oFile.FL_REP_DCG_ESTACIONAMIENTO);
                gModel.IVA = Convert.ToInt32(oFile.FL_REP_DCG_IVA);
                gModel.MANTENIMIENTO = Convert.ToInt32(oFile.FL_REP_DCG_MANTENCION);
                gModel.OBSERVACIONGASTOS = oFile.FL_REP_DCG_OBSERVACION;
                gModel.PEAJE = Convert.ToInt32(oFile.FL_REP_DCG_PEAJE);
                gModel.VIATICO =Convert.ToInt32( oFile.FL_REP_DCG_VIATICO);
                gModel.OTROS = Convert.ToInt32(oFile.FL_REP_DCG_OTROS);
            }

            return PartialView("_editardetalleGastos", gModel);
        }


        [HttpPost]
        public ActionResult EditarDetalleAsistente(FormCollection gForm)
        {
            AsistentesModels aModel = new AsistentesModels();
            Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext Oda = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();
            var gDatos = from da in Oda.PR_REP_EDITA_ASISTENTE()
                         where da.PK_REP_ASI_RUT == Convert.ToInt32(gForm["rut_asis"])
                         select da;
            foreach (var oFile in gDatos)
            {
                //aModel.Rut = oFile.PK_REP_ASI_RUT + '-' + oFile.FL_REP_ASI_DV;}
                aModel.Rut = Convert.ToString(oFile.PK_REP_ASI_RUT) + '-' + oFile.FL_REP_ASI_DV; 
                aModel.NOMBRESASISTENTE = oFile.FL_REP_ASI_NOMBRES;
                aModel.APELLIDOSASISTENTE = oFile.FL_REP_ASI_APELLIDOS;
                aModel.CODIGO = oFile.FL_REP_ASI_CODIGO;
                aModel.SUCASISTENTE = Convert.ToInt32(oFile.PK_PAR_SUC_ID);
            }
            aModel.GetListaSuc();
            return PartialView("_editarAsistentes", aModel);
        }


        [HttpPost]
        public ActionResult EditarDetalleConductor(FormCollection gForm)
        {
            ConductoresModels cModel = new ConductoresModels();
            Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext Oda = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();
            var gDatos = from da in Oda.PR_REP_EDITA_CONDUCTOR()
                         where da.PK_PAR_CON_RUT == Convert.ToInt32(gForm["rut_cond"])
                         select da;
            foreach (var oFile in gDatos)
            {
                //aModel.Rut = oFile.PK_REP_ASI_RUT + '-' + oFile.FL_REP_ASI_DV;}
                cModel.Rut = Convert.ToString(oFile.PK_PAR_CON_RUT) + '-' + oFile.FL_PAR_CON_DV;
                cModel.NOMBRECONDUCTOR = oFile.FL_PAR_CON_NOMBRE;
                cModel.CODIGO = Convert.ToInt32(oFile.FL_PAR_CON_CODIGO);
                cModel.SUCCONDUCTOR = Convert.ToInt32(oFile.PK_PAR_SUC_ID);
            }
            cModel.GetListaSuc();
            return PartialView("_editarConductor", cModel);
        }


        [HttpPost]
        public ActionResult EditarDetalleVehiculo(FormCollection gForm)
        {
            VehiculosModels cModel = new VehiculosModels();
            Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext Oda = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();
            var gDatos = from da in Oda.PR_REP_EDITA_VEHICULO()
                         where da.PK_PAR_TRA_ID == Convert.ToInt32(gForm["id_vehic"])
                         select da;
            foreach (var oFile in gDatos)
            {

                cModel.ID = oFile.PK_PAR_TRA_ID;
                cModel.PATENTE = oFile.FL_PAR_TRA_PATENTE;
                cModel.MARCA = oFile.FL_PAR_TRA_MARCA;
                cModel.MODELO = oFile.FL_PAR_TRA_MODELO;
                cModel.CAPACIDADKGS = Convert.ToInt32(oFile.FL_PAR_TRA_CAPACIDAD_KGS);
                cModel.CAPACIDADM3 = Convert.ToInt32(oFile.FL_PAR_TRA_CAPACIDAD_M3);
                //cModel.SUCVEHICULO = Convert.ToInt32(oFile.PK_PAR_SUC_ID);
            }
            cModel.GetListaSuc();
            return PartialView("_editarVehiculo", cModel);
        }



        [HttpPost]
        public ActionResult EditarDetalleDespachador(FormCollection gForm)
        {
            DespachadorModels dModel = new DespachadorModels();
            Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext Oda = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();
            var dDatos = from dd in Oda.PR_REP_EDITA_DESPACHADOR()
                         where dd.PK_REP_DES_RUT == Convert.ToInt32(gForm["rut_desp"])
                         select dd;
                       
            foreach (var oFile in dDatos)
            {
                
                dModel.Rut = Convert.ToString(oFile.PK_REP_DES_RUT) + '-' + oFile.FL_REP_DES_DV;
                dModel.NOMBRESDESPACHADOR = oFile.FL_REP_DES_NOMBRES;
                dModel.APELLIDOSDESPACHADOR = oFile.FL_REP_DES_APELLIDOS;
                dModel.SUCDESPACHADOR = Convert.ToInt32(oFile.PK_PAR_SUC_ID);
            }
            dModel.GetListaSuc();
            return PartialView("_editarDespachador", dModel);
        }

   
        public ActionResult VehiculosAsignados(ConductoresModels cModel)
        {
            
            ConductoresModels dModel = new ConductoresModels();
            Courier.Models.LinqBD_PARDataContext Dcond = new Courier.Models.LinqBD_PARDataContext();
            var dDatos = from c in Dcond.TB_PAR_CON_CONDUCTOR
                         where c.PK_PAR_CON_RUT==cModel.RUTAUX
                         select c;
            foreach (var dFile in dDatos)
            {
                cModel.NOMAUX=dFile.FL_PAR_CON_NOMBRE;
            }

            cModel.RescataTransAsoc(cModel.RUTAUX);
            return PartialView("VerVehiculosAsignados", cModel);
        }


        public ActionResult MostrarTransporte(ConductoresModels cModel)
        {
            if (cModel.BUSCARPAT == "")
            {
                cModel.BUSCARPAT = "";
            }
            else if (cModel.BUSCARPAT == null)
            {
                cModel.BUSCARPAT = "";
            }
            cModel.VerTransportesLib(cModel.BUSCARPAT);
            return PartialView("AsignarTransporte", cModel);
        }


        //public ActionResult MostrarCabTransporte(ConductoresModels cModel)
        //{
        //    cModel.VerTransportesLib(cModel.RUTAUX);cModel.BUSCARPAT
        //    return PartialView("CabAsignaTransporte", cModel);
        //}


        [HttpPost]
        public ActionResult EditarDetalleRutas(FormCollection gForm)
        {
            DetalleRutaModels cModel = new DetalleRutaModels();
            Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext Odr = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();
            var rDatos = from dr in Odr.VI_REP_ORIGEN_DESTINO2
                         where dr.PK_REP_DCR_ID == Convert.ToInt32(gForm["Detalleruta_edit_id"])
                         select dr;
            foreach (var oFile in rDatos)
            {
                cModel.IDRTA = oFile.PK_REP_DCR_ID;
                cModel.EDFECHA = Convert.ToString(oFile.FL_REP_DCR_FECHA);
                cModel.EDKMINICIAL = oFile.FL_REP_DCR_KM_INICIAL;
                cModel.EDKMFINAL = oFile.FL_REP_DCR_KM_FINAL;
                cModel.EDOBSERVACION = oFile.FL_REP_DCR_OBSERVACION;
                cModel.EDREGORIGEN = Convert.ToInt32(oFile.ID_ORIG_REG);
                cModel.EDCOMORIGEN = oFile.ID_ORIGEN;
                cModel.EDDIRORIGEN = oFile.DIR_ORIGEN;
                cModel.EDREGDESTINO = Convert.ToInt32(oFile.ID_DEST_REG);
                cModel.EDCOMDESTINO = oFile.ID_DESTINO;
                cModel.EDDIRDESTINO = oFile.DIR_DESTINO;
                cModel.EDPEDIDO = Convert.ToString(oFile.FL_REP_DCR_NUM_PEDIDO);
                cModel.EDTRANSPORTE = Convert.ToString(oFile.FL_REP_DCR_NUM_TRANSPORTE);
                cModel.EDZONA = Convert.ToString(oFile.FL_REP_DCR_ZONA);
            }
            cModel.GesListaRegion();
            //cModel.GetListaBlanco();
            //int idA = 0;
            //if (oDatos.PK_REP_ZON_ID != null)
            //    idA = Convert.ToInt32(oDatos.PK_REP_ARE_ID);
            cModel.GetListaComOrigen(cModel.EDREGORIGEN);
            cModel.GetListaComDestino(cModel.EDREGDESTINO);
            //cModel.oListaBlanco(cModel.COMORIGEN);
            return PartialView("_editardetalleRutas", cModel);
        }

        [Authorize]
        public ActionResult CerrarFolios()
        {
            CabeceraFoliosModels oModel = new CabeceraFoliosModels();
            oModel.GetListaOpBusFol();
            oModel.GetListaSuc();
            return View("CerrarFolios", oModel);
        }

        [Authorize]
        public ActionResult ReasignarFolios()
        {
            CabeceraFoliosModels oModel = new CabeceraFoliosModels();
            oModel.GetListaOpReasigFolios();
            oModel.GetListaVehiculos2();
            return View("ReasignarFolios", oModel);
        }


        [Authorize]
        public ActionResult ConsultaInformes()
        {
            InformeViajesModels oModel = new InformeViajesModels();
            oModel.GetListaInformes();
            return View("ConsultaInformes", oModel);
        }



        [HttpPost]
        public ActionResult EditarDetalleCombustible(FormCollection gForm)
        {
            DetalleCombustibleModels cModel = new DetalleCombustibleModels();
            Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext Odg = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();

            var cDatos = from dc in Odg.TB_REP_DCC_DETALLE_CONTROL_COMBUSTIBLE
                         where dc.PK_REP_DCC_ID == Convert.ToInt32(gForm["Detallecomb_edit_id"])
                         select dc;
            foreach (var oFile in cDatos)
            {
                cModel.IDCOMB = oFile.PK_REP_DCC_ID;
                cModel.FECHADETCOMB = Convert.ToDateTime(oFile.FL_REP_DCC_FECHA);
                cModel.NUMERO = oFile.FL_REP_DCC_NUMERO_DOCUMENTO;
                cModel.LITROS = oFile.FL_REP_DCC_CANTIDAD_LITROS.ToString();
                cModel.VALOR = oFile.FL_REP_DCC_VALOR;
                cModel.OBSERVACIONCOMB = oFile.FL_REP_DCC_OBSERVACION;
            }

            return PartialView("_editardetalleCombustible", cModel); 
        }



        #region GeneraExcelCombustible
        [Authorize]
        public ActionResult GeneraExcelCombustible(string FECINI, string FECTER)
        {
            Prueba pr = new Prueba();
            CabeceraFoliosModels oModel = new CabeceraFoliosModels();
            string Fecha_Ini = FECINI.Substring(6, 4) + FECINI.Substring(3, 2) + FECINI.Substring(0, 2) + " 00:00:00.000";
            string Fecha_Fin = FECTER.Substring(6, 4) + FECTER.Substring(3, 2) + FECTER.Substring(0, 2) + " 23:59:59.999";


            MemoryStream Stream = new MemoryStream();
            var workbook = new XLWorkbook();



            Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext oFol = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();

            oModel.AdminisReporteViajes = TieneRolByName("Admin.Reporte Viajes");
            if (oModel.AdminisReporteViajes == true)
            {
                var oDatos = (from exc in oFol.PR_REP_INF_COMB(Fecha_Ini, Fecha_Fin)
                              orderby exc.FECHA
                              select exc).ToList();

                var oResult = oDatos.ToDataSet();
                workbook.Worksheets.Add(oResult);


            }
            else
            {
                int oSuc = GetSucursalIDfromActiveUser();
                var oDatos = (from exc in oFol.PR_REP_INF_COMBXSUC(Fecha_Ini, Fecha_Fin, oSuc)
                              orderby exc.FECHA
                              select exc).ToList();
                var oResult = oDatos.ToDataSet();
                workbook.Worksheets.Add(oResult);
            }

            workbook.Worksheet(1).Name = "Informe";

            workbook.SaveAs(Stream);
            Response.ClearHeaders();
            Response.ClearContent();
            Response.Clear();

            Response.AddHeader("Content-Type", "application/vnd.ms-excel.12");
            Response.AddHeader("Content-Disposition", "attachment; filename=\"Informe.xlsx\"");

            Stream.WriteTo(Response.OutputStream);
            Stream.Close();
            Response.Flush();
            Response.End();

            return View();
        }
        #endregion





        #region GeneraExcelRutas
        [Authorize]
        public ActionResult GeneraExcelRutas(string FECINI, string FECTER)
        {
            Prueba pr = new Prueba();
            CabeceraFoliosModels oModel = new CabeceraFoliosModels();
            string Fecha_Ini = FECINI.Substring(6, 4) + FECINI.Substring(3, 2) + FECINI.Substring(0, 2) + " 00:00:00.000";
            string Fecha_Fin = FECTER.Substring(6, 4) + FECTER.Substring(3, 2) + FECTER.Substring(0, 2) + " 23:59:59.999";


            MemoryStream Stream = new MemoryStream();
            var workbook = new XLWorkbook();



            Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext oFol = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();

            oModel.AdminisReporteViajes = TieneRolByName("Admin.Reporte Viajes");
            if (oModel.AdminisReporteViajes == true)
            {
                var oDatos = (from exc in oFol.PR_REP_INF_RUTAS(Fecha_Ini, Fecha_Fin)
                              orderby exc.FECHA
                              select exc).ToList();

                var oResult = oDatos.ToDataSet();
                workbook.Worksheets.Add(oResult);


            }
            else
            {
                int oSuc = GetSucursalIDfromActiveUser();
                var oDatos = (from exc in oFol.PR_REP_INF_RUTASXSUC(Fecha_Ini, Fecha_Fin, oSuc)
                              orderby exc.FECHA
                              select exc).ToList();
                var oResult = oDatos.ToDataSet();
                workbook.Worksheets.Add(oResult);
            }

            workbook.Worksheet(1).Name = "Informe";

            workbook.SaveAs(Stream);
            Response.ClearHeaders();
            Response.ClearContent();
            Response.Clear();

            Response.AddHeader("Content-Type", "application/vnd.ms-excel.12");
            Response.AddHeader("Content-Disposition", "attachment; filename=\"Informe.xlsx\"");

            Stream.WriteTo(Response.OutputStream);
            Stream.Close();
            Response.Flush();
            Response.End();

            return View();
        }
        #endregion





        #region GeneraExcelGastos
        [Authorize]
        public ActionResult GeneraExcelGastos(string FECINI, string FECTER)
        {
            Prueba pr = new Prueba();
            CabeceraFoliosModels oModel = new CabeceraFoliosModels();
            string Fecha_Ini = FECINI.Substring(6, 4) + FECINI.Substring(3, 2) + FECINI.Substring(0, 2) + " 00:00:00.000";
            string Fecha_Fin = FECTER.Substring(6, 4) + FECTER.Substring(3, 2) + FECTER.Substring(0, 2) + " 23:59:59.999";

           
            MemoryStream Stream = new MemoryStream();
            var workbook = new XLWorkbook();


       
            Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext oFol = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();

            oModel.AdminisReporteViajes = TieneRolByName("Admin.Reporte Viajes");
            if (oModel.AdminisReporteViajes == true)
            {
                var oDatos = (from exc in oFol.PR_REP_INF_GASTOS(Fecha_Ini, Fecha_Fin)
                              orderby exc.FECHA
                              select exc).ToList();

                var oResult = oDatos.ToDataSet();
                workbook.Worksheets.Add(oResult);
                

            }
            else
            {
                int oSuc = GetSucursalIDfromActiveUser();
                var oDatos = (from exc in oFol.PR_REP_INF_GASTOSXSUC(Fecha_Ini, Fecha_Fin, oSuc)
                              orderby exc.FECHA
                              select exc).ToList();
                var oResult = oDatos.ToDataSet();
                workbook.Worksheets.Add(oResult);
            }

            workbook.Worksheet(1).Name = "Informe";

            workbook.SaveAs(Stream);
            Response.ClearHeaders();
            Response.ClearContent();
            Response.Clear();

            Response.AddHeader("Content-Type", "application/vnd.ms-excel.12");
            Response.AddHeader("Content-Disposition", "attachment; filename=\"Informe.xlsx\"");

            Stream.WriteTo(Response.OutputStream);
            Stream.Close();
            Response.Flush();
            Response.End();

            return View();
        }
        #endregion


       
    }
}
