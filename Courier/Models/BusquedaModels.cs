using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Courier.Models
{
    public class BusquedaModels
    {
        [Required]
        [Display(Name = "Sucursal Origen")]
        public string SucursalOrigen { get; set; }

        [Required]
        [Display(Name = "Sucursal Destino")]
        public string SucursalDestino { get; set; }

        [Required]
        [Display(Name = "Región Destino")]
        public string RegionDestino { get; set; }
        
        [Required]
        [Display (Name="Comuna Destino")]
        public string ComunaDestino { get; set; }
        
        public string LocalidadDestino { get; set; }

        
        

        public IEnumerable<SelectListItem> ListaSucursal { get; set; }
        public IEnumerable<SelectListItem> ListaRegion { get; set; }
        public IEnumerable<SelectListItem> ListaComuna { get; set; }
        public IEnumerable<SelectListItem> ListaLocalidad { get; set; }
        public IEnumerable<SelectListItem> ListaBlancoComuna { get; set; }
        public IEnumerable<SelectListItem> ListaBlancoLocalidad { get; set; }

        public List<ListaTiempoEstimado> ListaTiempo { get; set; }

        public string Error{get;set;}
    }
    public class ListaTiempoEstimado
    {
        public string Linea { get; set; }
        public int Horas { get; set; }
    }
    public class CoberturaClass
    {
        public string Origen {get;set;}
        public string Destino { get; set; }
        public bool Lunes { get; set; }
        public bool Martes { get; set; }
        public bool Miercoles { get; set; }
        public bool Jueves { get; set; }
        public bool Viernes { get; set; }
        public bool Sabado { get; set; }
        public bool Domingo { get; set; }
        public int Duracion { get; set; }
    }

    public class EntregaRealizada
    {
        public DateTime Fecha { get; set; }
        public string Recibe { get; set; }
        public string Rut { get; set; }
        public string Estado { get; set; }
        public string Observacion { get; set; }
        public string Visita { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string RegistroId { get; set; }
    }

    #region BuscarOTModels

    public class BuscarOTModels
    {

        public IEnumerable<SelectListItem> ListaTipoReferencia { get; set; }

        [Required]
        [Display(Name = "Buscar En")]
        public string TipoReferencia { get; set; }

        [Required]
        [Display(Name = "Valor")]
        //[RegularExpression("\\d*[0-9]$", ErrorMessage = "N° Guía sólo se admiten Números")]
        public string Referencia { get; set; }

        [Required]
        [Remote("ValidarCliente", "Validation")]
        [RegularExpression("\\d*[0-9]-([0-9]|k|K)$", ErrorMessage = "Campo Rut Formato incorrecto, ejemplo: 12345678-9")]
        [Display(Name = "Rut Cliente")]
        public string RutCliente { get; set; }

        public bool PuedeRectificarUbicaciones { get; set; }

        public string Referencia1 { get; set; }
        public string Referencia2 { get; set; }

        [Required]
        public string Sucursal { get; set; }

        [Display(Name = "OT Detalle")]
        [Required]
        [Remote("vJsonValidarCodigoAnular", "Validation")]
        public string OT { get; set; }

        public decimal OTP { get; set; }
        public decimal OTD { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string Direccion { get; set; }

        public VI_FOL_DATOS_ANULAR DatosAnular { get; set; }
        public IEnumerable<VI_FOL_VER_BULTO_ESTADO> ListaBultos { get; set; }
        public IEnumerable<TB_FOL_ESA_ESTADO_ALTER> ListaEsa { get; set; }
        public IEnumerable<TB_FOL_ERO_ESTADO_ROLL_BACK> ListaEro { get; set; }
        public IEnumerable<TB_FOL_EEX_ESTADO_EXCEPCION> ListaEEX { get; set; }
        public IEnumerable<SelectListItem> ListaSucursal { get; set; }
        public IEnumerable<EntregaRealizada> ListaEntrega { get; set; }
        public IEnumerable<EntregaRealizada> ListaDevolucion { get; set; }

        public string ObservacionProducto { get; set; }
        public IEnumerable<HtmlString> ListaErrores { get; set; }
        #region GetDatosByReferencia
        public void GetDatosByReferencia()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();

            oFol.PR_FOL_ULS_INSERT(Convert.ToInt32(RutCliente.Substring(0, RutCliente.Length - 2)), oValidation.GetRutActiveUser(), TipoReferencia);

            if (oValidation.IsNumeric2(TipoReferencia) == true)
            {
                var oTR = (from tdo in oPar.TB_PAR_TDO_TIPO_DOCUMENTO
                           where tdo.PK_PAR_TDO_ID == Convert.ToInt32(TipoReferencia)
                           select tdo).Single();

                if (oTR.PK_PAR_TRE_ID == 1)//Documento
                {
                    if (oValidation.isDecimal(Referencia) == true)
                    {
                        var oDocList = from doa in oFol.TB_FOL_DOA_DOC_ASOCIADO
                                       where doa.PK_FOL_DOA_NUMERO == Convert.ToDecimal(Referencia)
                                            && doa.PK_PAR_TDO_ID == Convert.ToInt32(TipoReferencia)
                                            && doa.PK_CLI_EMP_RUT == Convert.ToInt32(RutCliente.Substring(0, RutCliente.Length - 2))
                                       select doa;
                        if (oDocList.Count() > 0)
                        {
                            var oDoc = oDocList.ToList()[0];
                            var oDatos = (from otd in oFol.PR_FOL_VER_OT_GUARDADA (oDoc.PK_FOL_OTP_ID,oDoc.PK_FOL_OTD_ID)
                                         where                                        
                                         otd.EMP_RUT == Convert.ToInt32(RutCliente.Substring(0, RutCliente.Length - 2))
                                         select otd).ToList();
                            if (oDatos.Count() > 0)
                            {
                                var oDatOT = oDatos.ToList()[0];
                                OTP=oDatOT.OTP_ID;
                                OTD=oDatOT.OTD_ID;
                            }
                        }
                    }
                }
                else
                {
                    var oRefList = from rea in oFol.TB_FOL_REA_REF_ASOCIADO
                                   where rea.PK_FOL_REA_NUMERO == Referencia
                                        && rea.PK_PAR_TDO_ID == Convert.ToInt32(TipoReferencia)
                                        && rea.PK_CLI_EMP_RUT == Convert.ToInt32(RutCliente.Substring(0, RutCliente.Length - 2))
                                   select rea;

                    if (oRefList.Count() > 0)
                    {
                        var oRef = oRefList.ToList()[0];
                        var oDatos = (from otd in oFol.PR_FOL_VER_OT_GUARDADA(oRef.PK_FOL_OTP_ID,oRef.PK_FOL_OTD_ID)
                                     where
                                     otd.EMP_RUT == Convert.ToInt32(RutCliente.Substring(0, RutCliente.Length - 2))
                                      select otd).ToList() ;
                        if (oDatos.Count() > 0)
                        {
                            var oDatOT = oDatos.ToList()[0];
                            OTP = oDatOT.OTP_ID;
                            OTD = oDatOT.OTD_ID;
                        } 
                    }
                }
            }
            else
            {
                var oOT = oValidation.TransformCodigoGenericoOTPOTD(Referencia);

                var oDatos = (from otd in oFol.PR_FOL_VER_OT_GUARDADA(oOT.OTP, oOT.OTD)                             
                             //&& otd.EMP_RUT == Convert.ToInt32(RutCliente.Substring(0, RutCliente.Length - 2))
                             select otd).ToList();
                if (oDatos.Count() > 0)
                {
                    var oDatOT = oDatos.ToList()[0];
                    OTP = oDatOT.OTP_ID;
                    OTD = oDatOT.OTD_ID;
                }                    
            }



        }
        #endregion
        public void GetDatosAnular(decimal OTP, decimal OTD)
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            var oDatos = (from x in oFol.VI_FOL_DATOS_ANULAR
                          where
                              x.PK_FOL_OTP_ID == OTP &&
                              x.PK_FOL_OTD_ID == OTD
                          select x).Single();
            DatosAnular = oDatos;

        }

        public void GetLatLng(decimal OTP, decimal OTD)
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            var oDatos = oFol.PR_FOL_LOCALIZACION_OT_SIMPLE(OTD.ToString(), OTP.ToString()).Single();
                          
            Latitud = oDatos.LATITUD.ToString();
            Longitud = oDatos.LONGITUD.ToString();
            Direccion = oDatos.DIRECCION + ',' + oDatos.NUMERO + ' ' + oDatos.COMUNA;
        }

        #region GetListaEntrega
        public void GetListaEntrega(decimal OTP, decimal OTD)
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from vi in oFol.PR_FOL_DATOS_ENTREGA_CLIENTE(OTP,OTD)                                                  
                         select vi;

            List<EntregaRealizada> oListaEntregaRealizada = new List<EntregaRealizada>();
            foreach (var oFila in oDatos)
            {
                if (oFila.FL_FOL_EST_DESCRIPCION != null && oFila.FL_FOL_EST_DESCRIPCION != string.Empty)
                {
                    oListaEntregaRealizada.Add(new EntregaRealizada { 
                        Estado=oFila.FL_FOL_EST_DESCRIPCION,
                        Fecha=Convert.ToDateTime(oFila.FL_FOL_ENT_FECHA),
                        Observacion=oFila.FL_FOL_OES_NOMBRE,
                        Recibe=oFila.FL_FOL_ENT_RECIBE,
                        Rut=oFila.FL_FOL_ENT_RUT,
                        Visita=oFila.DESCRIPCION,
                        Latitud = oFila.LAT,
                        Longitud = oFila.LNG,
                        RegistroId = oFila.PK_FOL_ENT_ID.ToString()
                    });
                }
                else
                {
                    oListaEntregaRealizada.Add(new EntregaRealizada
                    {
                        Estado = oFila.FL_FOL_EST_NOMBRE,
                        Fecha = Convert.ToDateTime(oFila.FL_FOL_ENT_FECHA),
                        Observacion = oFila.FL_FOL_OES_NOMBRE,
                        Recibe = oFila.FL_FOL_ENT_RECIBE,
                        Rut = oFila.FL_FOL_ENT_RUT,
                        Visita = oFila.DESCRIPCION,
                        Latitud = oFila.LAT,
                        Longitud = oFila.LNG,
                        RegistroId = oFila.PK_FOL_ENT_ID.ToString()
                    });
                }
            }
            ListaEntrega = oListaEntregaRealizada;
        }
        #endregion        
        #region GetListaDevolucion
        public void GetListaDevolucion(decimal OTP, decimal OTD)
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from vi in oFol.PR_FOL_DATOS_ENTREGA_DEVOLUCION(OTP, OTD)
                         select vi;

            List<EntregaRealizada> oListaEntregaRealizada = new List<EntregaRealizada>();
            foreach (var oFila in oDatos)
            {
                if (oFila.FL_FOL_EST_DESCRIPCION != null && oFila.FL_FOL_EST_DESCRIPCION != string.Empty)
                {
                    oListaEntregaRealizada.Add(new EntregaRealizada
                    {
                        Estado = oFila.FL_FOL_EST_DESCRIPCION,
                        Fecha = Convert.ToDateTime(oFila.FL_FOL_ENT_FECHA),
                        Observacion = oFila.FL_FOL_OES_NOMBRE,
                        Recibe = oFila.FL_FOL_ENT_RECIBE,
                        Rut = oFila.FL_FOL_ENT_RUT,
                        Visita = oFila.DESCRIPCION
                    });
                }
                else
                {
                    oListaEntregaRealizada.Add(new EntregaRealizada
                    {
                        Estado = oFila.FL_FOL_EST_NOMBRE,
                        Fecha = Convert.ToDateTime(oFila.FL_FOL_ENT_FECHA),
                        Observacion = oFila.FL_FOL_OES_NOMBRE,
                        Recibe = oFila.FL_FOL_ENT_RECIBE,
                        Rut = oFila.FL_FOL_ENT_RUT,
                        Visita = oFila.DESCRIPCION
                    });
                }
            }
            ListaDevolucion = oListaEntregaRealizada;
        }
        #endregion        
        #region GetObservacion
        public void GetObservacion(decimal IdOTP, decimal IdOTD)
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from oca in oFol.TB_FOL_OCA_OBSERVACION_CARGA
                         where oca.PK_FOL_OTP_ID == IdOTP && oca.PK_FOL_OTD_ID == IdOTD
                         orderby oca.PK_FOL_OCA_ID
                         select oca;
            if (oDatos.Count() > 0)
            {
                ObservacionProducto = oDatos.ToList()[0].FL_FOL_OCA_OBSERVACION;
            }
            else
            {
                ObservacionProducto = "";
            }
        }
        #endregion

        public void GetBultos(decimal OTP, decimal OTD)
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            var oDatos = from blt in oFol.VI_FOL_VER_BULTO_ESTADO
                         where blt.PK_FOL_OTP_ID == OTP
                         && blt.PK_FOL_OTD_ID == OTD
                         select blt;

            ListaBultos = oDatos;
        }
        public void GetReferencias(int idServicio)
        {
            Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();
            var oResult = oValidation.GetNombreReferencia(idServicio);
            Referencia1 = oResult.NombreRef1;
            Referencia2 = oResult.NombreRef2;
        }
        public void GetListaESA()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from esa in oFol.TB_FOL_ESA_ESTADO_ALTER
                         select esa;

            ListaEsa = oDatos;
        }
        public void GetListaERO()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from ero in oFol.TB_FOL_ERO_ESTADO_ROLL_BACK
                         select ero;

            ListaEro = oDatos;
        }
        public void GetListaEEX()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from eex in oFol.TB_FOL_EEX_ESTADO_EXCEPCION
                         select eex;

            ListaEEX = oDatos;
        }


        public int Bulto { get; set; }
        [Required]
        public string Estado { get; set; }

        public IEnumerable<SelectListItem> ListaEstado { get; set; }

        public void GetListaEstado()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from eex in oFol.TB_FOL_EEX_ESTADO_EXCEPCION
                         join est in oFol.TB_FOL_EST_ESTADO on eex.PK_FOL_EST_ID equals est.PK_FOL_EST_ID
                         select new SelectListItem
                         {
                             Text = est.FL_FOL_EST_NOMBRE,
                             Value = est.PK_FOL_EST_ID.ToString()
                         };
            ListaEstado = oDatos;
        }
        public void GetListaEstadoRoll()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from ero in oFol.TB_FOL_ERO_ESTADO_ROLL_BACK
                         join est in oFol.TB_FOL_EST_ESTADO on ero.PK_FOL_EST_ID equals est.PK_FOL_EST_ID
                         select new SelectListItem
                         {
                             Text = est.FL_FOL_EST_NOMBRE,
                             Value = est.PK_FOL_EST_ID.ToString()
                         };
            ListaEstado = oDatos;
        }
        public void GetListaSucursal()
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oDatos = from suc in oPar.TB_PAR_SUC_SUCURSAL
                         select new SelectListItem
                         {
                             Text = suc.FL_PAR_SUC_NOMBRE,
                             Value = suc.PK_PAR_SUC_ID.ToString()

                         };
            ListaSucursal = oDatos;
        }
    }
    #endregion
    #region BuscarManifiestoModels
    public class BuscarManifiestoModels
    {
        [Display (Name="Buscar Por")]
        public string BuscarPor { get; set; }

        [Display(Name = "N° Manifiesto")]
        public string NumeroManifiesto { get; set; }

        [Display(Name = "Creado Por")]
        public string CreadoPor { get; set; }

        public bool BuscarPorFecha { get; set; }

        [Display (Name="Fecha creación:")]
        public string Fecha { get; set; }

        public IEnumerable<SelectListItem> ListaBuscarPor { get; set; }
        public IEnumerable<SelectListItem> ListaCreadoPor { get; set; }
        public IEnumerable<VI_FOL_MAL_MANIFIESTO_LISTA> ListaManifiestos { get; set; }

        public void GetListaBuscarPor()
        {
            List<SelectListItem> oLista = new List<SelectListItem>();
            oLista.Add(new SelectListItem { Text="Fecha Creación",Value="fecha" });
            oLista.Add(new SelectListItem { Text = "N° Manifiesto", Value = "manifiesto" });
            oLista.Add(new SelectListItem { Text = "Creado Por", Value = "creado" });
            ListaBuscarPor = oLista;
        }

        public void GetListaCreadoPor()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();
            var oDatos = from pr in oFol.PR_FOL_LISTA_USUARIO_MAN_SUC(oValidation.GetSucursalIDfromActiveUser())                         
                         select new SelectListItem{
                            Text=pr.FL_PAR_USU_NOMBRES + " " + pr.FL_PAR_USU_APELLIDOS,
                            Value=pr.PK_PAR_USU_RUT.ToString()
                         
                         };
            ListaCreadoPor = oDatos;

        }                
    }
    #endregion
}
