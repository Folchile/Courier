using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Courier.Models
{
    public class DevolucionModels
    {
        public decimal OTP { get; set; }
        public decimal OTD { get; set; }

        public string RutEmpresa { get; set; }

        public string FiltroTipo { get; set; }
        public IEnumerable<SelectListItem> CboFiltroTipo { get; set; }
        public IEnumerable<PR_FOL_OTD_DATOS_DEVOLUCION_FILTERResult> List_OTD_DATOS { get; set; }


        public string EditTipo { get; set; }
        public string EditEmpresa { get; set; }
        public string EditServicio { get; set; }


        [Required]
        public string Via { get; set; }        

        [Required]
        [Display (Name="Dirección")]
        public string Direccion { get; set; }

        [Required]
        [Display(Name = "N°")]
        public virtual string Numero { get; set; }


        [Display(Name = "Región")]
        [Required]
        public string Region { get; set; }

        [Required]
        [Remote("jSonValidarCoberturaComuna", "Validation")]
        public string Comuna { get; set; }
        
        [Remote("JsonValidarCoberturaLocalidad", "Validation")]
        public string Localidad { get; set; }


        public IEnumerable<SelectListItem> ListaVia { get; set; }
        public IEnumerable<SelectListItem> ListaRegion { get; set; }
        public IEnumerable<SelectListItem> ListaComuna { get; set; }
        public IEnumerable<SelectListItem> ListaLocalidad { get; set; }        

        public string BusquedaEmpresa { get; set; }
        public string BusquedaServicio { get; set; }
        public IEnumerable<SelectListItem> CboBusquedaEmpresa { get; set; }
        public IEnumerable<SelectListItem> CboBusquedaServicio { get; set; }


        #region GetDatosEmpresa
        public void GetDatosEmpresa()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            Controllers.ValidationController oValidation=new Controllers.ValidationController();
            int oTipo = 2; //2 las toma todas
            if (FiltroTipo != null && FiltroTipo != "")
                oTipo = Convert.ToInt32(FiltroTipo);
            var oDatos = from pr in oFol.PR_FOL_OTD_DATOS_DEVOLUCION_EMPRESA(oValidation.GetSucursalIDfromActiveUser(),oTipo)
                         select new SelectListItem
                         {
                             Text=pr.FL_CLI_EMP_RAZON_SOCIAL.ToUpper(),
                             Value=pr.PK_CLI_EMP_RUT.ToString()
                         };

            CboBusquedaEmpresa = oDatos;
        }
        #endregion
        #region GetDatosServicio
        public void GetDatosServicio()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            Controllers.ValidationController oValidation = new Controllers.ValidationController();
            int oTipo = 2; //2 las toma todas
            if (FiltroTipo != null && FiltroTipo != "")
                oTipo = Convert.ToInt32(FiltroTipo);
            int Empresa=0;
            if (BusquedaEmpresa!=null && BusquedaEmpresa!="")
                Empresa=Convert.ToInt32(BusquedaEmpresa);


            var oDatos = from pr in oFol.PR_FOL_OTD_DATOS_DEVOLUCION_SERVICIO(oValidation.GetSucursalIDfromActiveUser(), Empresa, oTipo)
                         select new SelectListItem
                         {
                             Text = pr.FL_PAR_SER_NOMBRE.ToUpper(),
                             Value = pr.PK_PAR_SER_ID.ToString()
                         };

            CboBusquedaServicio = oDatos;
        }
        #endregion
        #region GetDatosOTD_DATOS
        public void GetDatosOTD_DATOS()
        {
            try
            {
                Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();
                var oSucursal = oValidation.GetSucursalIDfromActiveUser();
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                //var oDatos = from vi in oFol.VI_FOL_OTD_DATOS
                //                where 
                //                    vi.FL_FOL_OTD_DEVOLUCION==true
                //                    && vi.PK_FOL_EST_ID!=10//Recepcionado OK
                //                    && vi.PK_PAR_SUC_ID==oSucursal
                //                orderby vi.PK_FOL_OTP_ID descending
                //                select vi;

                int? oEmpresa=null;
                int? oServicio=null;

                if (BusquedaEmpresa != null && BusquedaEmpresa != "")
                    oEmpresa = Convert.ToInt32(BusquedaEmpresa);

                if (BusquedaServicio != null && BusquedaServicio != "")
                    oServicio = Convert.ToInt32(BusquedaServicio);

                decimal? oOTP = null;
                decimal? oOTD = null;
                if (OTP != 0)
                    oOTP = OTP;
                if (OTD != 0)
                    oOTD = OTD;


                int oTipo = 2; //2 las toma todas
                if (FiltroTipo != null && FiltroTipo != "")
                    oTipo = Convert.ToInt32(FiltroTipo);

                var oDatos = (from pr in oFol.PR_FOL_OTD_DATOS_DEVOLUCION_FILTER(oSucursal,oEmpresa,oServicio,oTipo,oOTP,oOTD)
                              select pr).ToList();
                
                List_OTD_DATOS = oDatos;

                CboBusquedaEmpresa = new List<SelectListItem>();
                CboBusquedaServicio = new List<SelectListItem>();

            }
            catch { }
        }
        #endregion
        //#region GetDatosOTD_DATOS_FILTRADOS
        //public void GetDatosOTD_DATOS_FILTRADOS()
        //{
        //    try
        //    {
        //        Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();
        //        var oSucursal = oValidation.GetSucursalIDfromActiveUser();
        //        LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
        //        //var oDatos = from vi in oFol.VI_FOL_OTD_DATOS
        //        //                where 
        //        //                    vi.FL_FOL_OTD_DEVOLUCION==true
        //        //                    && vi.PK_FOL_EST_ID!=10//Recepcionado OK
        //        //                    && vi.PK_PAR_SUC_ID==oSucursal
        //        //                orderby vi.PK_FOL_OTP_ID descending
        //        //                select vi;

        //        var oDatos = (from pr in oFol.PR_FOL_OTD_DATOS_DEVOLUCION_FILTER(oSucursal, null, null, 2, OTP, OTD)
        //                      where pr.PK_FOL_OTP_ID==OTP && pr.PK_FOL_OTD_ID==OTD
        //                      select pr).ToList();
        //        List_OTD_DATOS = oDatos;
        //    }
        //    catch { }
        //}
        //#endregion
        #region GetDatosVia
        public void GetDatosVia()
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oDatos = from via in oPar.TB_PAR_VIA_VIA
                         select new SelectListItem
                         {
                             Text = via.FL_PAR_VIA_NOMBRE,
                             Value = via.PK_PAR_VIA_ID.ToString()
                         };
            ListaVia = oDatos;
        }
        #endregion
        #region GetDatosComuna
        public void GetDatosComuna(int PK_PAR_REG_ID)
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oDatos = from com in oPar.TB_PAR_COM_COMUNA
                         join prv in oPar.TB_PAR_PRV_PROVINCIA on com.PK_PAR_PRV_ID equals prv.PK_PAR_PRV_ID
                         where prv.PK_PAR_REG_ID==PK_PAR_REG_ID
                         orderby com.FL_PAR_COM_NOMBRE
                         select new SelectListItem
                         {
                             Text=com.FL_PAR_COM_NOMBRE,
                             Value=com.PK_PAR_COM_ID.ToString()
                         };
            ListaComuna = oDatos;
        }
        #endregion
        #region GetDatosLocalidad
        public void GetDatosLocalidad(int PK_PAR_COM_ID)
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oDatos = from loc in oPar.TB_PAR_LOC_LOCALIDAD                         
                         where loc.PK_PAR_COM_ID== PK_PAR_COM_ID
                         orderby loc.FL_PAR_LOC_NOMBRE
                         select new SelectListItem
                         {
                             Text = loc.FL_PAR_LOC_NOMBRE,
                             Value = loc.PK_PAR_LOC_ID.ToString()
                         };
            ListaLocalidad = oDatos;
        }
        #endregion
        #region GetDatosRegion
        public void GetDatosRegion()
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oDatos = from reg in oPar.TB_PAR_REG_REGION
                         select new SelectListItem
                         {
                             Text=reg.FL_PAR_REG_NOMBRE,
                             Value=reg.PK_PAR_REG_ID.ToString()
                         };
            ListaRegion = oDatos;
        }
        #endregion
        
    }
}