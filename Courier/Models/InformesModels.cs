using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Courier.Models
{
    public class InformesConsultaEstadosModels
    {
        [Required]
        public string Estado { get; set; }
        public IEnumerable<SelectListItem> ListaEstados { get; set; }
        public IEnumerable<VI_FOL_CONSULTA_ESTADOS> ListaInforme { get; set; }
        public IEnumerable<VI_FOL_CONSULTA_RECEPCION> ListaInformerRecepcion { get; set; }

        #region GetListaEstados
        public void GetListaEstados()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            Controllers.ValidationController oValidation = new Controllers.ValidationController();

            var oZonal = (from pr in oFol.PR_FOL_USU_ZONAL(System.Web.HttpContext.Current.User.Identity.Name)
                          select pr).Single();

            if (oZonal.Cantidad > 1)
            {

                var oDatos = from est in oFol.VI_FOL_ESTADO_OT_GROUP
                             select new SelectListItem
                             {
                                 Text = string.Format("{0} {1} ({2})", est.FL_FOL_EST_NOMBRE, est.FL_FOL_EST_DESCRIPCION, est.CANT_OT),
                                 Value = est.PK_FOL_EST_ID.ToString()
                             };
                ListaEstados = oDatos;
            }
            else
            {
                var oDatos = from est in oFol.VI_FOL_ESTADO_OT_GROUP_SUC
                             where est.PK_PAR_SUC_ID == oValidation.GetSucursalIDfromActiveUser()
                             select new SelectListItem
                             {
                                 Text = string.Format("{0} {1} ({2})", est.FL_FOL_EST_NOMBRE, est.FL_FOL_EST_DESCRIPCION, est.CANT_OT),
                                 Value = est.PK_FOL_EST_ID.ToString()
                             };
                ListaEstados = oDatos;
            }
        }
        #endregion

        #region GetListaInforme
        public void GetListaInforme()
        {
            Controllers.ValidationController oValidation = new Controllers.ValidationController();
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            var oZonal = (from pr in oFol.PR_FOL_USU_ZONAL(System.Web.HttpContext.Current.User.Identity.Name)
                          select pr).Single();


            int intEstado = 0;


            if (oValidation.isDecimal(Estado))
                intEstado = Convert.ToInt32(Estado);

            if (oZonal.Cantidad > 0)
            {
                if (intEstado == 10)
                {
                    var oDatos = from vi in oFol.VI_FOL_CONSULTA_RECEPCION
                                 where vi.PK_FOL_EST_ID == intEstado
                                 select vi;
                    ListaInformerRecepcion = oDatos;
                }
                else
                {
                    var oDatos = from vi in oFol.VI_FOL_CONSULTA_ESTADOS
                                 where vi.PK_FOL_EST_ID == intEstado
                                 select vi;
                    ListaInforme = oDatos;
                }

            }
            else
            {
                if (intEstado == 10)
                {
                    var oDatos = from vi in oFol.VI_FOL_CONSULTA_RECEPCION
                                 where vi.PK_FOL_EST_ID == intEstado && vi.PK_PAR_SUC_ID == oValidation.GetSucursalIDfromActiveUser()
                                 select vi;
                    ListaInformerRecepcion = oDatos;
                }
                else
                {
                    var oDatos = from vi in oFol.VI_FOL_CONSULTA_ESTADOS
                                 where vi.PK_FOL_EST_ID == intEstado && vi.PK_PAR_SUC_ID == oValidation.GetSucursalIDfromActiveUser()
                                 select vi;
                    ListaInforme = oDatos;
                }

            }



        }
        #endregion



    }    
}