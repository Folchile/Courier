using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Globalization;


namespace Courier.Models
{
    public class FolBIModels
    {
        #region Atributos

        [Required]
        [Display(Name = "Fecha")]
        public string FechaPeriodo { get; set; }
        public string FechaPeriodoTermino { get; set; }
        public string FiltroEstado { get; set; }
        public string FiltroConductor { get; set; }
        public int SucursalActual { get; set; }
        public string RutUsuario { get; set; }
        public List<ReporteDespacho> listaReportes { get; set; }
        public List<ReporteDespachoDiaMes> listaReportesDiaMes { get; set; }
        public List<ReporteDespachoGlobal> listaReporteGlobal { get; set; }
        public List<ReporteDespachoHora> listaReporteHora { get; set; }
        public List<ReporteDespachoDiaSemana> listaReporteDiaSemana { get; set; }
        public List<ReporteDespachoConductoresGlobal> listaReporteConductoresGlobal { get; set; }
        public List<ReporteDespachoConductor> listaReporteConductores { get; set; }
        public List<ReporteDespachoGlobal> listaReporteGlobalDetalle { get; set; }
        #endregion

        //metodos
        //public List<ReporteDespacho> GetReporteDespachos()
        //{
        //    Controllers.ValidationController oValidation = new Controllers.ValidationController();
        //    DateTime Inicio = FechaPeriodo;
        //    LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
        //    List<ReporteDespacho> list = new List<ReporteDespacho>();

        //    FechaPeriodo = DateTime.Parse("2013-07-01 00:00:00"); 

        //    var firstDayOfMonth = new DateTime(FechaPeriodo.Year, FechaPeriodo.Month, 1);
        //    var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

        //    var oDatos = (from pr in oFol.PR_FOL_REPORTE_DESPACHO(FechaPeriodo, lastDayOfMonth, oValidation.GetSucursalIDfromActiveUser(), FiltroEstado)
        //                  select pr).ToList();


        //    foreach (var dato in oDatos)
        //    {
        //        list.Add(new ReporteDespacho(dato.ESTADO_NOMBRE, dato.ESTADO_ID, dato.CANTIDAD.Value));
        //    }

        //    //esta lista se debe cargar de un sp segun los parámetros indicados, definir parametros
        //    return list;

        //}

        public List<ReporteDespacho> GetReporteDespachosByServer()
        {
            Controllers.ValidationController oValidation = new Controllers.ValidationController();

            List<ReporteDespacho> list = new List<ReporteDespacho>();


            IList<ReporteDespachoGlobal> listaEntregadoOk = (from i in listaReporteGlobal
                                                             where i.EstadoId == 10
                                                             select i).ToList();

            IList<ReporteDespachoGlobal> listaRechazo = (from i in listaReporteGlobal
                                                             where i.EstadoId == 8
                                                             select i).ToList();

            IList<ReporteDespachoGlobal> listaPendiente = (from i in listaReporteGlobal
                                                             where i.EstadoId == 6
                                                             select i).ToList();

            IList<ReporteDespachoGlobal> listaClienteNo = (from i in listaReporteGlobal
                                                             where i.EstadoId == 15
                                                             select i).ToList();


            list.Add(new ReporteDespacho("Entregado Ok", 10, listaEntregadoOk.Count()));
            list.Add(new ReporteDespacho("Rechazado", 8, listaRechazo.Count()));
            list.Add(new ReporteDespacho("Cliente no Encontrado", 15, listaClienteNo.Count()));
            list.Add(new ReporteDespacho("Pendiente", 6, listaPendiente.Count()));

            return list;

        }

        //public List<ReporteDespachoDiaMes> GetReporteDespachosDiaMes()
        //{
        //    Controllers.ValidationController oValidation = new Controllers.ValidationController();
        //    DateTime Inicio = FechaPeriodo;
        //    LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
        //    List<ReporteDespachoDiaMes> list = new List<ReporteDespachoDiaMes>();

        //    FechaPeriodo = DateTime.Parse("2013-07-01 00:00:00");

        //    var firstDayOfMonth = new DateTime(FechaPeriodo.Year, FechaPeriodo.Month, 1);
        //    var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

        //    var oDatos = (from pr in oFol.PR_FOL_REPORTE_DESPACHO_DIA_MES(FechaPeriodo, lastDayOfMonth, oValidation.GetSucursalIDfromActiveUser(), FiltroEstado)
        //                  select pr).ToList().OrderBy(x => x.DIA);


        //    foreach (var dato in oDatos)
        //    {
        //        list.Add(new ReporteDespachoDiaMes(dato.ESTADO_NOMBRE, dato.ESTADO_ID, dato.CANTIDAD.Value, dato.FECHA, dato.DIA.Value));
        //    }

        //    //esta lista se debe cargar de un sp segun los parámetros indicados, definir parametros
        //    return list;

        //}

        public List<ReporteDespachoDiaMes> GetReporteDespachosDiaMesByServer()
        {
            Controllers.ValidationController oValidation = new Controllers.ValidationController();

            List<ReporteDespachoDiaMes> list = new List<ReporteDespachoDiaMes>();
            List<ReporteDespachoGlobal> miListaGlobal = new List<ReporteDespachoGlobal>();

            //if (listaReporteGlobal == null)
            //{
            //    listaReporteGlobal = GetReporteDespachosGlobales();
            //}

            miListaGlobal = listaReporteGlobal;

            if (FiltroConductor != null && FiltroConductor != "")
            {
                miListaGlobal = (from i in miListaGlobal
                                 where i.NombreConductor == FiltroConductor
                                 select i).ToList();
            }

 
            //DateTime midate = DateTime.Parse(FechaPeriodo);
            var firstDayOfMonth = DateTime.Parse(FechaPeriodo);
            var recorrer = firstDayOfMonth;
            var lastDayOfMonth = DateTime.Parse(FechaPeriodoTermino);
            int listaEntregadoOk = 0;
            int listaRechazo = 0;
            int listaPendiente = 0;
            int listaClienteNo = 0;

            while (recorrer <= lastDayOfMonth)
            {
                switch (FiltroEstado)
                {
                    case "Entregado Ok":
                        listaEntregadoOk = (from i in miListaGlobal
                                            where i.EstadoId == 10
                                            && i.Fecha.Date == recorrer.Date
                                            select i).Count();

                        list.Add(new ReporteDespachoDiaMes("Entregado Ok", 10, listaEntregadoOk, recorrer.Date.ToString(), recorrer.Day));
                        break;

                    case "Rechazado":
                        listaRechazo = (from i in miListaGlobal
                                        where i.EstadoId == 8
                                        && i.Fecha.Date == recorrer.Date
                                        select i).Count();

                        list.Add(new ReporteDespachoDiaMes("Rechazado", 8, listaRechazo, recorrer.Date.ToString(), recorrer.Day));
                        break;

                    case "Pendiente":
                        listaPendiente = (from i in miListaGlobal
                                          where i.EstadoId == 6
                                          && i.Fecha.Date == recorrer.Date
                                          select i).Count();

                        list.Add(new ReporteDespachoDiaMes("Pendiente", 6, listaPendiente, recorrer.Date.ToString(), recorrer.Day));
                        break;

                    case "Cliente no Encontrado":
                        listaClienteNo = (from i in miListaGlobal
                                          where i.EstadoId == 15
                                          && i.Fecha.Date == recorrer.Date
                                          select i).Count();

                        list.Add(new ReporteDespachoDiaMes("Cliente no Encontrado", 15, listaClienteNo, recorrer.Date.ToString(), recorrer.Day));
                        break;

                    default:
                        listaEntregadoOk = (from i in miListaGlobal
                                            where i.EstadoId == 10 
                                            && i.Fecha.Date == recorrer.Date
                                            select i).Count();

                        listaRechazo = (from i in miListaGlobal
                                        where i.EstadoId == 8
                                        && i.Fecha.Date == recorrer.Date
                                        select i).Count();

                        listaPendiente = (from i in miListaGlobal
                                            where i.EstadoId == 6
                                            && i.Fecha.Date == recorrer.Date
                                            select i).Count();

                        listaClienteNo = (from i in miListaGlobal
                                            where i.EstadoId == 15
                                            && i.Fecha.Date == recorrer.Date
                                            select i).Count();


                        list.Add(new ReporteDespachoDiaMes("Entregado Ok", 10, listaEntregadoOk, recorrer.Date.ToString(), recorrer.Day));
                        list.Add(new ReporteDespachoDiaMes("Rechazado", 8, listaRechazo, recorrer.Date.ToString(), recorrer.Day));
                        list.Add(new ReporteDespachoDiaMes("Cliente no Encontrado", 15, listaClienteNo, recorrer.Date.ToString(), recorrer.Day));
                        list.Add(new ReporteDespachoDiaMes("Pendiente", 6, listaPendiente, recorrer.Date.ToString(), recorrer.Day));
                        break;
                }
               

                DateTime nuevoValor = recorrer.AddDays(1);
                recorrer = nuevoValor;
            }

            return list;

        }

        public List<ReporteDespachoHora> GetReporteDespachosHoraByServer()
        {
            Controllers.ValidationController oValidation = new Controllers.ValidationController();

            List<ReporteDespachoHora> list = new List<ReporteDespachoHora>();
            List<ReporteDespachoGlobal> miListaGlobal = new List<ReporteDespachoGlobal>();

            //if (listaReporteGlobal == null)
            //{
            //    listaReporteGlobal = GetReporteDespachosGlobales();
            //}

            miListaGlobal = listaReporteGlobal;

            if (FiltroConductor != null && FiltroConductor != "")
            {
                miListaGlobal = (from i in miListaGlobal
                                where i.NombreConductor == FiltroConductor
                                select i).ToList();
            }

            int hora = 0;
            int listaCount = 0;
            while (hora < 24)
            {

                switch (FiltroEstado)
                {
                    case "Entregado Ok":
                        listaCount = (from i in miListaGlobal.Where(f => f.EstadoId == 10)
                                      where i.Fecha.Hour == hora
                                      select i).Count();
                        break;

                    case "Rechazado":
                        listaCount = (from i in miListaGlobal.Where(f => f.EstadoId == 8)
                                      where i.Fecha.Hour == hora
                                      select i).Count();
                        break;

                    case "Pendiente":
                        listaCount = (from i in miListaGlobal.Where(f => f.EstadoId == 6)
                                      where i.Fecha.Hour == hora
                                      select i).Count();
                        break;

                    case "Cliente no Encontrado":
                        listaCount = (from i in miListaGlobal.Where(f => f.EstadoId == 15)
                                      where i.Fecha.Hour == hora
                                      select i).Count();
                        break;

                    default:
                        listaCount = (from i in miListaGlobal
                                      where i.Fecha.Hour == hora
                                      select i).Count();
                        break;
                }

                list.Add(new ReporteDespachoHora(listaCount, hora));
                hora++;
            }

            return list;

        }

        public List<ReporteDespachoDiaSemana> GetReporteDespachosDiaSemanaByServer()
        {
            Controllers.ValidationController oValidation = new Controllers.ValidationController();

            List<ReporteDespachoDiaSemana> list = new List<ReporteDespachoDiaSemana>();
            List<ReporteDespachoGlobal> myListGlobal = new List<ReporteDespachoGlobal>();

            //if (listaReporteGlobal == null)
            //{
            //    listaReporteGlobal = GetReporteDespachosGlobales();
            //}
            
            switch (FiltroEstado)
            {
                case "Entregado Ok":
                    myListGlobal = listaReporteGlobal.Where(f => f.EstadoId == 10).ToList();
                    break;

                case "Rechazado":
                    myListGlobal = listaReporteGlobal.Where(f => f.EstadoId == 8).ToList();
                    break;

                case "Pendiente":
                    myListGlobal = listaReporteGlobal.Where(f => f.EstadoId == 6).ToList();
                    break;

                case "Cliente no Encontrado":
                    myListGlobal = listaReporteGlobal.Where(f => f.EstadoId == 15).ToList();
                    break;

                default:
                    myListGlobal = listaReporteGlobal.ToList();
                    break;
            }

            if (FiltroConductor != null && FiltroConductor != "")
            {
                myListGlobal = (from i in myListGlobal
                               where i.NombreConductor == FiltroConductor
                               select i).ToList();
            }

            CultureInfo ci = new CultureInfo("Es-Es");

            int listaLunes = (from i in myListGlobal
                                where ci.DateTimeFormat.GetDayName(i.Fecha.Date.DayOfWeek) == "lunes"
                                select i).Count();
            list.Add(new ReporteDespachoDiaSemana(listaLunes, "Lunes"));
            int listaMartes = (from i in myListGlobal
                                where ci.DateTimeFormat.GetDayName(i.Fecha.Date.DayOfWeek) == "martes"
                                select i).Count();
            list.Add(new ReporteDespachoDiaSemana(listaMartes, "Martes"));
            int listaMiercoles = (from i in myListGlobal
                                    where ci.DateTimeFormat.GetDayName(i.Fecha.Date.DayOfWeek) == "miércoles"
                                    select i).Count();
            list.Add(new ReporteDespachoDiaSemana(listaMiercoles, "Miércoles"));
            int listaJueves = (from i in myListGlobal
                                where ci.DateTimeFormat.GetDayName(i.Fecha.Date.DayOfWeek) == "jueves"
                                select i).Count();
            list.Add(new ReporteDespachoDiaSemana(listaJueves, "Jueves"));
            int listaViernes = (from i in myListGlobal
                                where ci.DateTimeFormat.GetDayName(i.Fecha.Date.DayOfWeek) == "viernes"
                                select i).Count();
            list.Add(new ReporteDespachoDiaSemana(listaViernes, "Viernes"));
            int listaSabado = (from i in myListGlobal
                                where ci.DateTimeFormat.GetDayName(i.Fecha.Date.DayOfWeek) == "sábado"
                                select i).Count();
            list.Add(new ReporteDespachoDiaSemana(listaSabado, "Sábado"));
            int listaDomingo = (from i in myListGlobal
                                where ci.DateTimeFormat.GetDayName(i.Fecha.Date.DayOfWeek) == "domingo"
                                select i).Count();       
            list.Add(new ReporteDespachoDiaSemana(listaDomingo, "Domingo"));

            return list;

        }

        public List<ReporteDespachoGlobal> GetReporteDespachosGlobales( )
        {
            Controllers.ValidationController oValidation = new Controllers.ValidationController();
            //var aux = this.FechaPeriodo;
            //DateTime Inicio = DateTime.Parse(this.FechaPeriodo);
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            List<ReporteDespachoGlobal> list = new List<ReporteDespachoGlobal>();

            //Inicio = DateTime.Parse(Inicio.ToString("yyyy-MM-dd h:mm tt"));

            DateTime firstDayOfMonth = DateTime.Parse(this.FechaPeriodo); ; // new DateTime(Inicio.Year, Inicio.Month, 1);
            DateTime lastDayOfMonth = DateTime.Parse(this.FechaPeriodoTermino); ; // firstDayOfMonth.AddMonths(1).AddDays(-1);

            var oDatos = (from pr in oFol.PR_FOL_REPORTE_DESPACHO_GLOBAL(firstDayOfMonth, lastDayOfMonth, oValidation.GetSucursalIDfromActiveUser(), FiltroEstado)
                          select pr).ToList();


            foreach (var dato in oDatos)
            {
                list.Add(new ReporteDespachoGlobal(dato.ESTADO_NOMBRE, dato.ESTADO_ID, dato.FECHA, dato.NOMBRE_CONDUCTOR, dato.RUT_CONDUCTOR, dato.OT));
            }

            return list;

        }

        //public List<ReporteDespachoConductoresGlobal> GetReporteDespachoConductoresGlobales()
        //{
        //    Controllers.ValidationController oValidation = new Controllers.ValidationController();
        //    //DateTime Inicio = FechaPeriodo;
        //    LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
        //    List<ReporteDespachoConductoresGlobal> list = new List<ReporteDespachoConductoresGlobal>();

        //    FechaPeriodo = DateTime.Parse("2013-07-01 00:00:00");

        //    var firstDayOfMonth = new DateTime(FechaPeriodo.Year, FechaPeriodo.Month, 1);
        //    var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

        //    var oDatos = (from pr in oFol.PR_FOL_REPORTE_DESPACHO_CONDUCTORES(FechaPeriodo, lastDayOfMonth, oValidation.GetSucursalIDfromActiveUser(), FiltroEstado)
        //                  select pr).ToList();


        //    foreach (var dato in oDatos)
        //    {
        //        list.Add(new ReporteDespachoConductoresGlobal(dato.ESTADO_NOMBRE, dato.ESTADO_ID, dato.OBSERVACION_NOMBRE, dato.OBSRVACION_ID.Value, dato.CONDUCTOR, dato.RUT_CONDUCTOR.Value));
        //    }

        //    return list;

        //}

        public List<ReporteDespachoConductor> GetReporteDespachosConductoresByServer()
        {
            Controllers.ValidationController oValidation = new Controllers.ValidationController();

            List<ReporteDespachoConductor> list = new List<ReporteDespachoConductor>();

            //if (listaReporteConductoresGlobal == null)
            //{
            //    listaReporteConductoresGlobal = GetReporteDespachoConductoresGlobales();
            //}

            //if (listaReporteGlobal == null)
            //{
            //    listaReporteGlobal = GetReporteDespachosGlobales();
            //}

            List<int> rutConductores = (from i in listaReporteGlobal
                                        select i.RutConductor).Distinct().ToList();

            foreach (int rut in rutConductores)
            {
                switch (FiltroEstado)
                {
                    case "Entregado Ok":
                        var auxEntregaOk = (from i in listaReporteGlobal.Where(f => f.EstadoId == 10).ToList()
                                    where i.RutConductor == rut
                                    select i).ToList();
                        if (auxEntregaOk.Count() > 0)
                        {
                            list.Add(new ReporteDespachoConductor("Entregado Ok", 10, auxEntregaOk.First().NombreConductor, rut, auxEntregaOk.Count()));
                        }
                        break;

                    case "Rechazado":
                        var auxRechazado = (from i in listaReporteGlobal.Where(f => f.EstadoId == 8).ToList()
                                    where i.RutConductor == rut
                                    select i).ToList();
                        if (auxRechazado.Count() > 0)
                        {
                            list.Add(new ReporteDespachoConductor("Rechazado", 8, auxRechazado.First().NombreConductor, rut, auxRechazado.Count()));
                        }
                        break;

                    case "Cliente no Encontrado":
                        var auxCliente = (from i in listaReporteGlobal.Where(f => f.EstadoId == 15).ToList()
                                    where i.RutConductor == rut
                                    select i).ToList();
                        if (auxCliente.Count() > 0)
                        {
                            list.Add(new ReporteDespachoConductor("Cliente no Encontrado", 15, auxCliente.First().NombreConductor, rut, auxCliente.Count()));
                        }
                        break;

                    case "Pendiente":
                        var auxPendiente = (from i in listaReporteGlobal.Where(f => f.EstadoId == 6).ToList()
                                    where i.RutConductor == rut
                                    select i).ToList();
                        if (auxPendiente.Count > 0)
                        {
                            list.Add(new ReporteDespachoConductor("Pendiente", 6, auxPendiente.First().NombreConductor, rut, auxPendiente.Count()));
                        }
                        break;

                    default:
                        var auxTotal = (from i in listaReporteGlobal
                                    where i.RutConductor == rut
                                    select i).ToList();
                         if (auxTotal.Count() > 0)
                        {
                            list.Add(new ReporteDespachoConductor("Entregas", 10, auxTotal.First().NombreConductor, rut, auxTotal.Count()));
                        }
                        break;
                }
                
                
            }

            return list;

        }
       //clases 
        public class ReporteDespachoGlobal
        {
            int estadoId;
            string estadoNombre;
            DateTime fecha;
            int rutConductor;
            string nombreConductor;
            string ot;

            public string EstadoNombre { get { return estadoNombre; } }
            public int EstadoId { get { return estadoId; } }
            public DateTime Fecha { get { return fecha; } }
            public string NombreConductor { get { return nombreConductor; } }
            public int RutConductor { get { return rutConductor; } }
            public string Ot { get { return ot; } }

            public ReporteDespachoGlobal(string EstadoNombre, int EstadoId, DateTime Fecha, string NombreConductor, int RutConductor, string Ot)
            {
                this.estadoNombre = EstadoNombre;
                this.estadoId = EstadoId;
                this.fecha = Fecha;
                this.nombreConductor = NombreConductor;
                this.rutConductor = RutConductor;
                this.ot = Ot;
            }
        }

        public class ReporteDespacho
        {
            int estadoId;
            string estadoNombre;
            float cantidad;

            public string EstadoNombre { get { return estadoNombre; } }
            public int EstadoId { get { return estadoId; } }
            public float Cantidad { get { return cantidad; } }

            public ReporteDespacho(string EstadoNombre, int EstadoId, float Cantidad)
            {
                this.estadoNombre = EstadoNombre;
                this.estadoId = EstadoId;
                this.cantidad = Cantidad;
            }
        }

        public class ReporteDespachoDiaMes
        {
            int estadoId;
            string estadoNombre;
            string fecha;
            float cantidad;
            int dia;

            public string EstadoNombre { get { return estadoNombre; } }
            public int EstadoId { get { return estadoId; } }
            public float Cantidad { get { return cantidad; } }
            public string Fecha { get { return fecha; } }
            public int Dia { get { return dia; } }

            public ReporteDespachoDiaMes(string EstadoNombre, int EstadoId, float Cantidad, string Fecha, int Dia)
            {
                this.estadoNombre = EstadoNombre;
                this.estadoId = EstadoId;
                this.cantidad = Cantidad;
                this.fecha =Fecha.Substring(0,10);
                this.dia = Dia;
            }
        }

        public class ReporteDespachoDiaMesHora
        {
            int estadoId;
            string estadoNombre;
            DateTime fecha;
            string fechaString;
            float cantidad;
            int dia;
            int hora;
            string diaSemana;

            public string EstadoNombre { get { return estadoNombre; } }
            public int EstadoId { get { return estadoId; } }
            public float Cantidad { get { return cantidad; } }
            public DateTime Fecha { get { return fecha; } }
            public int Dia { get { return dia; } }
            public int Hora { get { return hora; } }
            public string DiaSemana { get { return diaSemana; } }
            public string FechaString { get { return fechaString; } }

            public ReporteDespachoDiaMesHora(string EstadoNombre, int EstadoId, DateTime Fecha)
            {
                this.estadoNombre = EstadoNombre;
                this.estadoId = EstadoId;
                this.cantidad = Cantidad;
                this.fecha = Fecha;
                this.fechaString = Fecha.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);;
                CultureInfo ci = new CultureInfo("Es-Es");
                this.diaSemana = ci.DateTimeFormat.GetDayName(Fecha.DayOfWeek);
                this.hora = Fecha.Hour;
                this.dia = Fecha.Day;
            }
        }

        public class ReporteDespachoHora
        {
            float cantidad;
            int hora;
            string horaString;
            DateTime horaDate;

            public float Cantidad { get { return cantidad; } }
            public int Hora { get { return hora; } }
            public string HoraString { get { return horaString; } }
            public DateTime HoraDate { get { return horaDate; } }

            public ReporteDespachoHora(float Cantidad, int Hora)
            {
                DateTime aux = DateTime.Parse("01/09/2017");
                aux.AddHours(Hora);
                
                this.cantidad = Cantidad;
                this.hora = Hora;
                this.horaDate = aux;
                if (Hora == 1 || Hora == 0)
                {
                    this.horaString = Hora.ToString() + ":00 hr";
                }
                else
                {
                    this.horaString = Hora.ToString() + ":00 hrs";
                }
                
            }
        }

        public class ReporteDespachoDiaSemana
        {
            float cantidad;
            string dia;

            public string Dia { get { return dia; } }
            public float Cantidad { get { return cantidad; } }

            public ReporteDespachoDiaSemana(float Cantidad, string Dia)
            {
                this.cantidad = Cantidad;
                this.dia = Dia;
            }
        }

        public class ReporteDespachoConductoresGlobal
        {
            int estadoId;
            string estadoNombre;
            int observaacionId;
            string observacionNombre;
            int rutConductor;
            string conductor;

            public string EstadoNombre { get { return estadoNombre; } }
            public int EstadoId { get { return estadoId; } }
            public string ObservacionNombre { get { return observacionNombre; } }
            public int ObservacionId { get { return observaacionId; } }
            public string ConductorNombre { get { return conductor; } }
            public int RutConductor { get { return rutConductor; } }
            

            public ReporteDespachoConductoresGlobal(string EstadoNombre, int EstadoId, string ObservacionNombre, int ObservacionId, string ConductorNombre, int RutConductor)
            {
                this.estadoNombre = EstadoNombre;
                this.estadoId = EstadoId;
                this.observaacionId = ObservacionId;
                this.observacionNombre = ObservacionNombre;
                this.conductor = ConductorNombre;
                this.rutConductor = RutConductor;
            }
        }

        public class ReporteDespachoConductor
        {
            int estadoId;
            string estadoNombre;
            int rutConductor;
            string conductor;
            float cantidad;

            public string EstadoNombre { get { return estadoNombre; } }
            public int EstadoId { get { return estadoId; } }
            public string ConductorNombre { get { return conductor; } }
            public int RutConductor { get { return rutConductor; } }
            public float Cantidad { get { return cantidad; } }


            public ReporteDespachoConductor(string EstadoNombre, int EstadoId, string ConductorNombre, int RutConductor, float Cantidad)
            {
                this.estadoNombre = EstadoNombre;
                this.estadoId = EstadoId;
                this.conductor = ConductorNombre;
                this.rutConductor = RutConductor;
                this.cantidad = Cantidad;
            }
        }

    }
}
