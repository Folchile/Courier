using System;

using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace Courier.Models
{
    #region CargaModels
    public class CargaModels
    {

        [Display(Name = "N° Manifiesto")]
        [Required(ErrorMessage = "Obligatorio")]
        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        [Remote("vJsonValidarExisteManifiestoId", "Validation")]
        public string NumeroBusquedaManifiesto { get; set; }          
        //Proveedor
        [Display(Name = "Proveedor")]
        public string Proveedor { get; set; }

        [Display(Name = "Arrastre")]
        public string Arrastre { get; set; }

        //Movil
        [Required]
        public string Patente { get; set; }

        [Required]
        public string Conductor { get; set; }

        [Display (Name="N° Manifiesto")]
        [Required (ErrorMessage="Obligatorio")]
        public decimal NumeroManifiesto { get; set; }
        public decimal NumeroManifiesto2 { get; set; }
        public decimal EditNumeroManifiesto { get; set; }

        public IEnumerable<PR_FOL_LST_LISTA_MAN_SUC_CERRADOResult> ListaManCarga { get; set; }
        public IEnumerable<PR_FOL_MAN_LISTA_CARGADOS_HOYResult> ListaManCargadoHoy { get; set; }

        public IEnumerable<SelectListItem> oListaProveedores { get; set; }

        public IEnumerable<SelectListItem> oListaConductores { get; set; }
        public IEnumerable<SelectListItem> oListaPatentes { get; set; }


        #region GetListaPatentes
        public void GetListaPatentes()
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            
            Controllers.ValidationController oValidation = new Controllers.ValidationController();

            var a = oValidation.GetSucursalIDfromActiveUser();


            var oDatos = from tra in oPar.TB_PAR_TRA_TRANSPORTE
                         join det in oPar.TB_PAR_DET_DESTINO_TRANSPORTE
                            on tra.PK_PAR_TRA_ID equals det.PK_PAR_TRA_ID
                         where det.PK_PAR_SUC_ID==oValidation.GetSucursalIDfromActiveUser()
                         select new SelectListItem
                         {
                             Text=tra.FL_PAR_TRA_PATENTE,
                             Value=tra.PK_PAR_TRA_ID.ToString()
                         };

            oListaPatentes = oDatos.Distinct();
        }
        #endregion
        
        #region GetListaConductores
        public void GetListaConductores()
        {
            List<SelectListItem> oLst = new List<SelectListItem>();

            oLst.Add(new SelectListItem {
                Text="-- Seleccione Conductor --",
                Value="",
                Selected=true
            });


            if (Patente != "" && Patente != null)
            {
                LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
                var oDatos = from con in oPar.TB_PAR_CON_CONDUCTOR
                             join trc in oPar.TB_PAR_TRC_TRA_CON on con.PK_PAR_CON_RUT equals trc.PK_PAR_CON_RUT
                             join tra in oPar.TB_PAR_TRA_TRANSPORTE on trc.PK_PAR_TRA_ID equals tra.PK_PAR_TRA_ID
                             where tra.PK_PAR_TRA_ID==Convert.ToInt32(Patente)
                             select new SelectListItem
                             {
                                 Text = con.FL_PAR_CON_NOMBRE,
                                 Value = con.PK_PAR_CON_RUT.ToString()
                             };
                oLst.AddRange(oDatos);
            }
            oListaConductores = oLst;
        }
        #endregion

        #region AgregarBultoManifiesto
        public bool AgregarBultoManifiesto(decimal OTP_ID, decimal OTD_ID, decimal BLT_ID, decimal MAN_ID)
        {
            Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();
            try
            {
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                oFol.PR_FOL_AGREGA_BULTO_MAN(OTP_ID, OTD_ID, MAN_ID, BLT_ID,oValidation.GetSucursalIDfromActiveUser(),oValidation.GetRutActiveUser());
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
        
        #region CierraManifiesto
        public class ReturnManifiesto
        {
            public bool Ok { get; set; }
            public string Mensaje { get; set; }
        }
        public ReturnManifiesto CierraManifiesto()
        {
            ReturnManifiesto oResult = new ReturnManifiesto();
            try
            {
                int oRutCon = 0;
                Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();

                if (oValidation.isDecimal(Conductor))
                    oRutCon = Convert.ToInt32(Conductor);


                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                oFol.PR_FOL_MAN_CERRAR(NumeroManifiesto2, oRutCon, Convert.ToInt32(Patente), oValidation.GetSucursalIDfromActiveUser(), oValidation.GetRutActiveUser());
                oResult.Ok = true;
                oResult.Mensaje = "<p>Manifiesto Cerrado Exitosamente.</p>";                
            }
            catch(Exception e)
            {
                oResult.Ok = false;
                oResult.Mensaje = "<p>Error al cerrar el manifiesto</p>" + e.Message;
            }
            return oResult;
        }
        #endregion

        #region GetListaManCargas
        public void GetListaManCargas()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            Controllers.ValidationController oValidation = new Controllers.ValidationController();
          

            var oDatos = (from pr in oFol.PR_FOL_LST_LISTA_MAN_SUC_CERRADO(oValidation.GetSucursalIDfromActiveUser(),oValidation.GetRutActiveUser())
                         select pr).ToList();

            ListaManCarga = oDatos;
        }
        #endregion

        #region GetListaManCerradosHoy
        public void GetListaManCargadosHoy()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            Controllers.ValidationController oValidation = new Controllers.ValidationController();

            var oDatos = (from pr in oFol.PR_FOL_MAN_LISTA_CARGADOS_HOY(oValidation.GetSucursalIDfromActiveUser(), oValidation.GetRutActiveUser())
                          select pr).ToList();

            ListaManCargadoHoy = oDatos;
        }
        #endregion

        #region GetListaPatentesProveedor
        public void GetListaPatentesProveedor()
        {
            List<SelectListItem> oLst = new List<SelectListItem>();

            oLst.Add(new SelectListItem
            {
                Text = "-- Seleccione Patente --",
                Value = "",
                Selected = true
            });


            if (Proveedor != "" && Proveedor != null)
            {
                LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
                var oDatos = from tra in oPar.TB_PAR_TRA_TRANSPORTE
                             join pro in oPar.TB_PAR_TPR_TRANSPORTE_PROVEEDOR
                                on tra.PK_PAR_TRA_ID equals pro.PK_PAR_TRA_ID
                             where pro.PK_PAR_PRO_ID == Convert.ToInt32(Proveedor)
                             select new SelectListItem
                             {
                                 Text = tra.FL_PAR_TRA_PATENTE,
                                 Value = tra.PK_PAR_TRA_ID.ToString()
                             };

                oLst.AddRange(oDatos);
                //oListaPatentes = oDatos.Distinct();

            }

            oListaPatentes = oLst;
        }
        #endregion

        #region GetListaProveedores
        public void GetListaProveedores()
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();


            Controllers.ValidationController oValidation = new Controllers.ValidationController();

            var a = oValidation.GetSucursalIDfromActiveUser();


            var oDatos = from PRO in oPar.TB_PAR_PRO_PROVEEDOR
                         join SPR in oPar.TB_PAR_SPR_SUCURSAL_PROVEEDOR
                            on PRO.PK_PAR_PRO_ID equals SPR.PK_PAR_PRO_ID
                         where SPR.PK_PAR_SUC_ID == oValidation.GetSucursalIDfromActiveUser()
                         select new SelectListItem
                         {
                             Text = PRO.FL_PAR_PRO_NOMBRE,
                             Value = PRO.PK_PAR_PRO_ID.ToString()
                         };

            oListaProveedores = oDatos.Distinct();
        }
        #endregion

        #region GuardarEntrega
        public class ReturnManifiestoGuardar
        {
            public bool Ok { get; set; }
            public string MensajeGuardado { get; set; }
        }
        public ReturnManifiestoGuardar GuardarEntrega(decimal NumeroManifiesto)
        {
            ReturnManifiestoGuardar oResult = new ReturnManifiestoGuardar();
            try
            {
                int oRutCon = 0;
                Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();

                if (oValidation.isDecimal(Conductor))
                    oRutCon = Convert.ToInt32(Conductor);


                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                oFol.PR_FOL_MAN_CARGAR_TRANSPORTE(NumeroManifiesto, oValidation.GetSucursalIDfromActiveUser(), oValidation.GetRutActiveUser());
                oResult.Ok = true;
                oResult.MensajeGuardado = "<p>Manifiesto Guardado Exitosamente.</p>";
            }
            catch (Exception e)
            {
                oResult.Ok = false;
                oResult.MensajeGuardado = "<p>Error al guardar los datos: </p>" + e.Message;
            }
            return oResult;
        }

        #endregion
        
    }


    #endregion
}