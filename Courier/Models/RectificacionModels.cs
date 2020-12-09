using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Courier.Models
{
    public class RectificacionRecepcionModels
    {
        public decimal OTP { get; set; }
        public decimal OTD { get; set; }

        [Required]
        [Display (Name="N° OT")]
        public string OT { get; set; }

        [Required]
        [Display(Name = "Fecha Entrega")]
        public string FechaEntrega { get; set; }

        public DateTime UltimaFecha { get; set; }
        public int DiferenciaFechas { get; set; }

        [Required]
        public string Hora { get; set; }
        [Required]
        public string Minutos { get; set; }

        [Required]
        public string Observacion { get; set; }
        

        public IEnumerable<SelectListItem> ListaHora { get; set; }
        public IEnumerable<SelectListItem> ListaMinutos { get; set; }
        public IEnumerable<SelectListItem> ListaObservacion { get; set; }

        public void GetUltimaFecha()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            Controllers.ValidationController oValidation = new Controllers.ValidationController();
            var oResult=oValidation.TransformCodigoGenericoOTPOTD(OT);
            var oHistoria = (from pr in oFol.PR_FOL_ULTIMA_HISTORIA(oResult.OTP,oResult.OTD)
                             select pr).Single() ;

            if ((Convert.ToDateTime(DateTime.Now.ToShortDateString()) - Convert.ToDateTime(oHistoria.FL_FOL_HID_FECHA.ToShortDateString())).TotalDays - (Convert.ToDateTime(DateTime.Now.ToShortDateString()) - Convert.ToDateTime(oHistoria.FL_FOL_HID_FECHA.ToShortDateString())).Days > 0)
                DiferenciaFechas=(Convert.ToDateTime(DateTime.Now.ToShortDateString()) - Convert.ToDateTime(oHistoria.FL_FOL_HID_FECHA.ToShortDateString())).Days + 1;
            else
                DiferenciaFechas = ( Convert.ToDateTime(DateTime.Now.ToShortDateString()) -  Convert.ToDateTime(oHistoria.FL_FOL_HID_FECHA.ToShortDateString())).Days;
            UltimaFecha = oHistoria.FL_FOL_HID_FECHA;            
            OTP = oResult.OTP;
            OTD = oResult.OTD;
        }
        public void GetListasHora()
        {
            List<SelectListItem> CustomHora = new List<SelectListItem>();
            List<SelectListItem> CustomMinutos = new List<SelectListItem>();
            string oAdicion = "";
            for (var c = 0; c <= 23; c++)
            {
                if (c.ToString().Length == 1)
                {
                    oAdicion = "0";
                }
                else
                {
                    oAdicion = "";
                }
                CustomHora.Add(new SelectListItem {
                    Text = oAdicion + c.ToString(),
                    Value=c.ToString()
                });
            }
            
            for (var c = 0; c <= 59; c++)
            {
                if (c.ToString().Length == 1)
                {
                    oAdicion = "0";
                }
                else
                {
                    oAdicion = "";
                }
                CustomMinutos.Add(new SelectListItem
                {
                    Text = oAdicion + c.ToString(),
                    Value = c.ToString()
                });
            }
            ListaHora = CustomHora;
            ListaMinutos = CustomMinutos;
        }
        public void GetListaObservacion()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from oes in oFol.TB_FOL_OES_OBSERVACION_ESTADO
                         where oes.PK_FOL_EST_ID == 10
                         select new SelectListItem
                         {
                             Text=oes.FL_FOL_OES_NOMBRE,
                             Value=oes.PK_FOL_OES_ID.ToString()
                         };
            ListaObservacion=oDatos;
        }
    }
}