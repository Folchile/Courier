using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Courier.Models
{
    public class GenericListaBusqueda
    {
        public int Rut {get;set;}
        public string Dv { get; set; }
        public string RazonSocial {get;set;}
        public string Fantasia {get;set;}
    }
    public class GenericoModels
    {        

        [Required(ErrorMessage = "El campo Rut es obligatorio")]         
        [Display(Name = "Rut")]
        [Remote("ValidarCliente", "Validation")]
        [RegularExpression("\\d*[0-9]-([0-9]|k|K)$", ErrorMessage = "Campo Rut Formato incorrecto, ejemplo: 12345678-9")]
        public string RutCliente { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [RegularExpression("\\d*[0-9]", ErrorMessage = "No Corresponde")]
        [Display(Name = "Servicio")]
        [Remote("ValidarColumnas", "Validation")]
        public string Servicio { get; set; }  

        [Display (Name="Buscar")]
        public string StringBusqueda { get; set; }
        public IEnumerable<GenericListaBusqueda> ListaBusqueda { get; set; }

        public IEnumerable<SelectListItem> ListaServicios { get; set; }

        #region GetListaBusqueda
        public void GetListaBusqueda(string TextoFiltro)
        {
                        
            LinqCLIDataContext oCli = new LinqCLIDataContext();

            var oLista = from dat in oCli.TB_CLI_EMP_EMPRESAS
                         where dat.FL_CLI_EMP_FANTASIA.Contains(TextoFiltro)
                         || dat.FL_CLI_EMP_RAZON_SOCIAL.Contains(TextoFiltro)
                         || dat.PK_CLI_EMP_RUT.ToString().Contains(TextoFiltro)
                         orderby dat.FL_CLI_EMP_RAZON_SOCIAL
                         select new GenericListaBusqueda { 
                            Fantasia=dat.FL_CLI_EMP_FANTASIA.ToUpper(),
                            RazonSocial=dat.FL_CLI_EMP_RAZON_SOCIAL.ToUpper(),
                            Rut=dat.PK_CLI_EMP_RUT,
                            Dv=dat.FL_CLI_EMP_DV.ToUpper()                      
                         };
            ListaBusqueda = oLista;

        }
        #endregion
        #region GetListaServicios
        public void GetListaServicios(int intRutEmpresa)
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oDatos = from ser in oPar.TB_PAR_SER_SERVICIO
                         join ems in oPar.TB_PAR_EMS_EMPRESA_SERVICIO on ser.PK_PAR_SER_ID equals ems.PK_PAR_SER_ID
                         where ems.PK_CLI_EMP_RUT == intRutEmpresa
                         orderby ser.FL_PAR_SER_NOMBRE
                         select new SelectListItem
                         {
                             Text=ser.FL_PAR_SER_NOMBRE.ToUpper(),
                             Value=ser.PK_PAR_SER_ID.ToString()
                         };
            ListaServicios = oDatos;
        }
        #endregion
    }
    public class GenericOTDModels
    {
        public PR_FOL_OTD_DATOSResult OTD_DATOS { get; set; }
        public int CantidadBultos { get; set; }
        public bool ContraPago { get; set; }

        public string NombreReferencia1 { get; set; }
        public string NombreReferencia2 { get; set; }

        public bool EditaVia { get; set; }

        public void GetDatosOTD_DATOS(decimal OTP, decimal OTD)
        {
            try
            {
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                var oDatos = (from pr in oFol.PR_FOL_OTD_DATOS(OTP, OTD)
                             select pr).Single();
                                                            
                OTD_DATOS = oDatos;

                CantidadBultos = (from blt in oFol.TB_FOL_BLT_BULTO where blt.PK_FOL_EST_ID != 13 && blt.PK_FOL_OTP_ID == OTP && blt.PK_FOL_OTD_ID == OTD select blt.PK_FOL_BLT_ID).Count();                
            }
            catch { }
        }
        public void GetContraPago(decimal OTP, decimal OTD)
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oContra = from doa in oFol.TB_FOL_DOA_DOC_ASOCIADO
                          where doa.FL_FOL_DOA_CONTRAPAGO > 0
                          && doa.PK_FOL_OTP_ID == OTP
                          && doa.PK_FOL_OTD_ID == OTD
                          select doa;
            if (oContra.Count() > 0)
            {
                ContraPago = true;
            }
            else
            {
                ContraPago = false;
            }
        }
    }
    public class GenericManHistorico
    {
        public decimal OTP { get; set; }
        public decimal OTD { get; set; }

        public IEnumerable<PR_FOL_MAN_CONSULTA_HISTResult> ListaHistoria { get; set; }

        public void GetDatosListaHistoria()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = (from vi in oFol.PR_FOL_MAN_CONSULTA_HIST (OTP,OTD)                         
                         select vi).ToList();

            ListaHistoria = oDatos;
        }
    }
    public class BultoImpresion
    {
        public int TipoEtiqueta { get; set; }
        public int Cantidad { get; set; }
        public decimal Bulto1 { get; set; }
        public decimal Bulto2 { get; set; }
        public decimal Bulto3 { get; set; }
        public decimal Bulto4 { get; set; }

        public int CantidadBultos{get;set;}
        public string SucursalDestino { get; set; }
        public string ComunaDestino { get; set; }

        public DatosBulto DatosBulto1 { get; set; }
        public DatosBulto DatosBulto2 { get; set; }
        public DatosBulto DatosBulto3 { get; set; }
        public DatosBulto DatosBulto4 { get; set; }

        public string Sucursal { get; set; }
        public string OT { get; set; }
        public string Destino { get; set; }
    }
    public class DatosBulto
    {
        public decimal CodigoBulto { get; set; }
        public float? Peso { get; set; }
        public string PesoString { get; set; }
        public string CantidadBulto { get; set; }        
    }
    public class RetornoFechaModels
    {
        public bool Ok { get; set; }
        public string Mensaje { get; set; }
        public DateTime Fecha { get; set; }
    }
    public class RetornoBoolModels
    {
        public bool Ok { get; set; }
        public string Mensaje { get; set; }
        public bool Resultado { get; set; }
    }

    public class RetornoLocationModels
    {
        public bool Ok { get; set; }
        public string Mensaje { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
    }

    public class RetornoModels
    {
        public bool Ok { get; set; }
        public string Mensaje { get; set; }
        public decimal OTP { get; set; }
        public decimal OTD { get; set; }
        public decimal Bulto { get; set; }
        public BultoImpresion Impresion { get; set; }
        public bool? Comprobacion { get; set; }
    }
    public class ResultModels : RetornoModels
    {
    }
    public class ObservacionesModels
    {
        public IEnumerable<VI_FOL_OBSERVACIONES> ListaObservaciones { get; set; }
        public IEnumerable<SelectListItem> ListaTipoObs { get; set; }

        public decimal OTP { get; set; }
        public decimal OTD { get; set; }

        public decimal OTP_Obs { get; set; }
        public decimal OTD_Obs { get; set; }

        public bool Habilitado {get;set;}

        [Required]
        [Display (Name="Observación")]
        public string TipoObservacion { get; set; }

        public void GetListaObservaciones()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from vi in oFol.VI_FOL_OBSERVACIONES
                         where vi.PK_FOL_OTP_ID == OTP
                         && vi.PK_FOL_OTD_ID == OTD
                         orderby vi.FL_FOL_OBS_FECHA descending
                         select vi;
            ListaObservaciones = oDatos;
        }
        public void GetListaTipoObs()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from tob in oFol.TB_FOL_TOB_TIPO_OBS
                         select new SelectListItem
                         {
                             Text=tob.FL_FOL_TOB_NOMBRE,
                             Value=tob.PK_FOL_TOB_ID.ToString()
                         };
            ListaTipoObs = oDatos;
        }
        public void GetHabilitadoObs()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from eob in oFol.TB_FOL_EOB_ESTADO_OBSERVACION
                         join otd in oFol.TB_FOL_OTD_OT_DETALLE on eob.PK_FOL_EST_ID equals otd.PK_FOL_EST_ID
                         where 
                            otd.PK_FOL_OTP_ID==OTP && otd.PK_FOL_OTD_ID==OTD
                         select eob;
            if (oDatos.Count() > 0)
            {
                Habilitado = true;
            }
            else
            {
                Habilitado = false;
            }

        }
    }
    public class RetornoHUModels
    {
        public bool Ok { get; set; }
        public string Mensaje { get; set; }
        public bool HU { get; set; }

        public decimal OTP { get; set; }
        public decimal OTD { get; set; }
    }
    public class RetornoPesoModels
    {
        public bool Ok { get; set; }
        public string Mensaje { get; set; }
        public int PesoMaximo { get; set; }
    }

    public class GenListaConductor
    {
        public string Rut { get; set; }
        public string Digito { get; set; }
        public string Nombre { get; set; }
        public int Codigo { get; set; }
    }


    public class GenListaPtte
    {
       public string ptte { get; set; }
    }

    
    public class GenericPteModels
    {
        [Required(ErrorMessage = "Patente Obligatoria")]
        [Display(Name = "Patente")]
        public string PATENTE { get; set; }

        [Display(Name = "Buscar")]
        public string StringBusquedaPtte { get; set; }

        public IEnumerable<GenListaPtte> ListaBuscarPtte { get; set; }

        #region GetListaBuscaPtte
        public void GetListaBuscaPtte(string TextoFiltro)
        {
            LinqBD_PARDataContext oParam = new LinqBD_PARDataContext();
      
            var oLista = from ptte in oParam.TB_PAR_TRA_TRANSPORTE
                         where ptte.FL_PAR_TRA_PATENTE.Contains(TextoFiltro)
                         orderby ptte.FL_PAR_TRA_PATENTE
                         select new GenListaPtte
                         {
                            ptte=ptte.FL_PAR_TRA_PATENTE
                         };
            ListaBuscarPtte = oLista;
        }
        #endregion

    }



    public class GenericCondModels
    {
        [Required(ErrorMessage = "Rut Obligatorio")]
        [Display(Name = "Rut")]
        public string RUT { get; set; }

        [Display(Name = "Buscar")]
        public string StringBusquedaCond { get; set; }

        public IEnumerable<GenListaConductor> ListaBuscarCond { get; set; }

        
        #region GetListaBuscaCond
        public void GetListaBuscaCond(string TextoFiltro)
        {
            LinqBD_PARDataContext oCond = new LinqBD_PARDataContext();

            var oLista = from cond in oCond.TB_PAR_CON_CONDUCTOR
                         where cond.PK_PAR_CON_RUT.ToString().Contains(TextoFiltro)
                         || cond.FL_PAR_CON_NOMBRE.Contains(TextoFiltro)
                         || cond.FL_PAR_CON_CODIGO.ToString().Contains(TextoFiltro)
                         orderby cond.FL_PAR_CON_NOMBRE
                         select new GenListaConductor
                         {
                             Rut = Convert.ToString(cond.PK_PAR_CON_RUT) + '-'+ cond.FL_PAR_CON_DV,
                             //Digito = cond.FL_PAR_CON_DV,
                             Nombre = cond.FL_PAR_CON_NOMBRE,
                             Codigo = Convert.ToInt32(cond.FL_PAR_CON_CODIGO)
                         };
            ListaBuscarCond = oLista;

        }
        #endregion

    }


}