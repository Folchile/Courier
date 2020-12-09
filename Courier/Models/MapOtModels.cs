using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Globalization;


namespace Courier.Models
{
    public class MapOtModels
    {
        #region Atributos

        [Required]
        [Display(Name = "Fecha")]
        public string FechaPeriodo { get; set; }
        public string FechaPeriodoTermino { get; set; }
        public string Estado { get; set; }
        public int SucursalActual { get; set; }
        public string RutUsuario { get; set; }

        [Required]
        [Display(Name = "EstadoNombre")]
        public string EstadoNombre = "EstadoNombre";
        public IEnumerable<SelectListItem> listaItems { get; set; }
        public List<PR_FOL_LOCALIZACION_OTResult> resultLista { get; set; }
        #endregion

        //metodos
        public void GetListaEstados()
        {
            ListaEstado item1 = new ListaEstado("Pendiente", "0");
            ListaEstado item2 = new ListaEstado("Actualizado", "1");

            List<ListaEstado> miLista = new List<ListaEstado>();
            miLista.Add(item1);
            miLista.Add(item2);

            listaItems = from i in miLista
                         select new SelectListItem
                         {
                            Text = i.Text,
                            Value = i.value
                         };

        }

        public List<PR_FOL_LOCALIZACION_OTResult> GetListaResultados()
        {
            Controllers.ValidationController oValidation = new Controllers.ValidationController();
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            //List<PR_FOL_LOCALIZACION_OTResult> list = new List<PR_FOL_LOCALIZACION_OTResult>();

            //Inicio = DateTime.Parse(Inicio.ToString("yyyy-MM-dd h:mm tt"));

            DateTime firstDayOfMonth = DateTime.Parse(this.FechaPeriodo); ; // new DateTime(Inicio.Year, Inicio.Month, 1);
            DateTime lastDayOfMonth = DateTime.Parse(this.FechaPeriodoTermino); ; // firstDayOfMonth.AddMonths(1).AddDays(-1);

            var oDatos = (from pr in oFol.PR_FOL_LOCALIZACION_OT(firstDayOfMonth, lastDayOfMonth, oValidation.GetSucursalIDfromActiveUser(), this.Estado)
                          select pr).ToList();


            return oDatos;

        }

        //clases

        public class ListaEstado
        {
            public string text;
            public string value;

            public string Text { get { return text; } }
            public string Value { get { return value; } }

            public ListaEstado(string text, string value)
            {
                this.text = text;
                this.value = value;
            }
           
        }

        public class RetornoDireccion
        {
            public string ot;

            [Required]
            [Display(Name = "Dirección")]
            public string direccion;

            [Required]
            [Display(Name = "Número")]
            public string numero;

            [Required]
            [Display(Name = "Latitud")]
            public string lat;

            [Required]
            [Display(Name = "Longitud")]
            public string lng;

            public string comuna;

            public string OT { get { return ot; } }
            public string Direccion { get { return direccion; } }
            public string Numero { get { return numero; } }
            public string Lat { get { return lat; } }
            public string Lng { get { return lng; } }
            public string Comuna { get { return comuna; } }

            public RetornoDireccion(string OT, string Direccion, string Numero, string Lat, string Lng)
            {
                this.ot = OT;
                this.direccion = Direccion;
                this.numero = Numero;
                this.lat = Lat;
                this.lng = Lng;
            }

            public RetornoDireccion(string OT, string Direccion, string Numero, string Lat, string Lng, string Comuna)
            {
                this.ot = OT;
                this.direccion = Direccion;
                this.numero = Numero;
                this.lat = Lat;
                this.lng = Lng;
                this.comuna = Comuna;
            }
        }

        //public class GoogleMarker
        //{
        //    public string SiteName { get; set; }
        //    public double Latitude { get; set; }
        //    public double Longitude { get; set; }
        //    public string InfoWindow { get; set; }
        //}
    }
}
