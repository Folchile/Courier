using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Courier.Models
{
    public class InformeCelsouthModels
    {
        [Required]
        [Display (Name="Fecha Inicio")]
        public string FechaInicio { get; set; }

        [Required]
        [Display(Name = "Fecha Termino")]
        public string FechaTermino { get; set; }
    }
    public class ReportesModels
    {
        public int RutCli { get; set; }

        [Required(ErrorMessage = "Dato Obligatorio")]
        [Display(Name = "Fecha Inicio")]
        //[MustBeGreaterThan("FechaTer", "Start date must be greater than End date")]
        public string FechaIni { get; set; }

        [Required(ErrorMessage = "Dato Obligatorio")]
        [Display(Name = "Fecha Termino")]
        public string FechaTer { get; set; }

        [Required(ErrorMessage = "Dato Obligatorio")]
        [Display(Name = "Sucursal")]
        public string Sucursal { get; set; }

        [Required(ErrorMessage = "Dato Obligatorio")]
        [Display(Name = "Informes")]
        public string Reportes { get; set; }

        public string NomIndexReporte { get; set; }

        public IEnumerable<SelectListItem> ListaReportes { get; set; }
        public IEnumerable<SelectListItem> ListaSucursal { get; set; }

        public void GetListaReportes()
        {
            List<SelectListItem> oListaReportes = new List<SelectListItem>();

            oListaReportes.Add(new SelectListItem
            {
                Text = "Seleccionar Informe...",
                Value = "0"
            });

            oListaReportes.Add(new SelectListItem
            {
                Text = "Informe Planilla Transporte",
                Value = "1"
            });

            oListaReportes.Add(new SelectListItem
            {
                Text = "Informe Planilla Hoja de Ruta",
                Value = "2"
            });

            oListaReportes.Add(new SelectListItem
            {
                Text = "Informe Planilla Stock",
                Value = "3"
            });

            oListaReportes.Add(new SelectListItem
            {
                Text = "Informe Planilla Devoluciones",
                Value = "4"
            });

            ListaReportes = oListaReportes;
        }

        #region GetSucursales
        public void GetSucursales()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            Controllers.ValidationController oValidation = new Controllers.ValidationController();

            var oZonal = (from pr in oFol.PR_FOL_USU_ZONAL(System.Web.HttpContext.Current.User.Identity.Name)
                          select pr).Single();

            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            List<SelectListItem> oListaSucursal = new List<SelectListItem>();

            if (oZonal.Cantidad > 0)
            {
                oListaSucursal.Add(new SelectListItem { Text = "-- Seleccione Sucursal --", Value = "", Selected = true });
                oListaSucursal.Add(new SelectListItem { Text = "-- Todas las Sucursales --", Value = "0", Selected = false });
                oListaSucursal.AddRange(from suc in oPar.TB_PAR_SUC_SUCURSAL
                                        orderby suc.FL_PAR_SUC_NOMBRE
                                        select new SelectListItem
                                        {
                                            Text = suc.FL_PAR_SUC_NOMBRE,
                                            Value = suc.PK_PAR_SUC_ID.ToString(),
                                        });
            }
            else
            {
                oListaSucursal.AddRange(from suc in oPar.TB_PAR_SUC_SUCURSAL
                                        where suc.PK_PAR_SUC_ID == oValidation.GetSucursalIDfromActiveUser()
                                        orderby suc.FL_PAR_SUC_NOMBRE
                                        select new SelectListItem
                                        {
                                            Text = suc.FL_PAR_SUC_NOMBRE,
                                            Value = suc.PK_PAR_SUC_ID.ToString(),
                                            Selected = true,
                                        });
            }



            ListaSucursal = oListaSucursal;
        }
        #endregion

    }

    #region EnRutaTroncalModels
    public class EnRutaTroncalModels
    {
        public IEnumerable<PR_FOL_OTD_RECEPCION_SUCURSALResult> oListaRecepcion { get; set; }
        public IEnumerable<PR_FOL_OTD_RECEPCION_SUCURSAL_DETALLEResult> oListaRecepcionDetalle { get; set; }

        public decimal MAN_ID { get; set; }
        public Int32 COM_ID { get; set; }

        public void GetDatosListaRecepcion()
        {
            Controllers.ValidationController oValidation = new Controllers.ValidationController();
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = (from vi in oFol.PR_FOL_OTD_RECEPCION_SUCURSAL(oValidation.GetSucursalIDfromActiveUser())
                         select vi).ToList();
            oListaRecepcion = oDatos;
        }

        public void GetDatosListaRecepcionDetalle()
        {
            Controllers.ValidationController oValidation = new Controllers.ValidationController();
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = (from vi in oFol.PR_FOL_OTD_RECEPCION_SUCURSAL_DETALLE(oValidation.GetSucursalIDfromActiveUser(),COM_ID,MAN_ID)
                          select vi).ToList();
            oListaRecepcionDetalle = oDatos;
        }
    }
    #endregion
}