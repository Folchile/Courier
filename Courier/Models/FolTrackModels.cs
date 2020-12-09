using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.ComponentModel.DataAnnotations;

namespace Courier.Models
{  
    public struct RealPoint {
            DateTime x;
            decimal y;

            public DateTime X { get { return x; } set { x = value; } }
            public decimal Y { get { return y; } set { y = value; } }

            public RealPoint(DateTime x, decimal y)
            {
                this.x = x;
                this.y = y;
            }
        }
    public class RealPointSeries {
            RealPoint point;
            string z;
            string toolTip;
            
            public RealPoint Point {
                get { return point; }
                set { point = value; }
            }
            public DateTime X
            {
                get { return point.X; }
            }
            public decimal Y {
                get { return point.Y; }
            }

            public string Z {
                get { return z; }
            }
            public string ToolTip {
                get { return toolTip; }
            }
            public RealPointSeries(DateTime x, decimal y, string z, string toolTip)
            {
                Point = new RealPoint(x, y);
                this.z = z;
                this.toolTip = toolTip;
            }
        }
    public class FolTrackModels
    {

        #region Atributos
            [Required]
            public string Conductor { get; set; }


            [Required]
            public string Patente { get; set; }

            [Required]
            public int? RutConductor { get; set; }

            [Required]
            public string Transporte { get; set; }

            [Required]
            [Display(Name = "Fecha")]
            public DateTime FechaConsulta { get; set; }

            public int SucursalActual { get; set; }


            public IEnumerable<PR_FOL_CONSULTA_OT_MAN_TRACKINGResult> ListaMan { get; set; }
            public List<RealPointSeries> ListaManifiestos { get; set; }
            public List<int?> Conductores { get; set; }

            public List<string> Patentes { get; set; }

        #endregion

          
            #region ListaManifiestos
                public void GetListaManifiestos()
                {
                    Controllers.ValidationController oValidation = new Controllers.ValidationController();
                    LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
              
                    var oDatos = (from pr in oFol.PR_FOL_CONSULTA_OT_MAN_TRACKING(FechaConsulta, oValidation.GetSucursalIDfromActiveUser())
                                 select pr);

                   ListaMan = oDatos.ToList();
                   GetListaPatentes();
                }
            #endregion
            #region ListaPatentes
                private void GetListaPatentes()
                {
                    var oDatos = (from pr in ListaMan  
                                 select pr.PATENTE).Distinct();
                    Patentes = oDatos.ToList();
                }
            #endregion
            #region DatosSerie
                public List<RealPointSeries> GetDatosSeries()
                {
                    GetListaManifiestos();
                    Controllers.ValidationController oValidation = new Controllers.ValidationController();
                    var oDatos = (from pr in ListaMan
                                  where pr.PATENTE == Patente
                                  select pr).ToList();
                    foreach (var dato in oDatos)
                    {
                        if (dato != null)
                        {
                            Conductor = dato.CONDUCTOR;
                            RutConductor = dato.RUT_CONDUCTOR;
                        }
                    }
                    Random random = new Random();
                    List<RealPointSeries> list = new List<RealPointSeries>();
                    list.AddRange(GetRechazados());
                    list.AddRange(GetRecepcionados());
                    list.AddRange(GetEnRutaCliente());
                    list.AddRange(GetNoEncontrado());
                    list.AddRange(GetGestionados());
                    return list;
                }
            #endregion
            public RealPoint[] CalcRandomPoints(Random random, int count, int xMin, int xMax, int yMin, int yMax)
            {
                DateTime Inicio = FechaConsulta;
                RealPoint[] points;
                if (yMax == 1)
                {
                    points = new RealPoint[xMax - xMin];
                }
                else
                {
                    points = new RealPoint[count];
                }
                if (count != 0) {
                    for (int i = 0; i < points.Length; i++)
                    {
                        bool repeat;
                        do
                        {
                            repeat = false;
                            if (yMax == 1)
                                points[i].X = Inicio.AddMinutes(9*60 +( xMin * (60 * 9) /count )+ i*60*9/count );//.ToString("HH:mm"));
                            else
                                points[i].X = Inicio.AddMinutes(random.Next(xMin*60, xMax*60) + 9 * 60);//.ToString("HH:mm");
                       
                            points[i].Y = yMax;//random.Next(yMin, yMax) ;

                            for (int k = 0; k < i; k++)
                                if (points[k].X == points[i].X && points[k].Y == points[i].Y)
                                {
                                    repeat = true;
                                    break;
                                }
                        } while (repeat);
                    } 
                }
                return points;
            }
            public List<RealPointSeries> GetRandomPoints(Random random, int count, int xMin, int xMax, int yMin, int yMax, int z)
            {
                RealPoint[] points = CalcRandomPoints(random, count, xMin, xMax, yMin, yMax);
                List<RealPointSeries> list = new List<RealPointSeries>();
                for (int i = 0; i < points.Length; i++)
                {
                    list.Add(new RealPointSeries(points[i].X, points[i].Y, z == 4 ? "No Encontrado" : z == 3 ? "En Ruta" : z == 2 ? "Recepcionado OK" : z == 1 ? "Rechazado" : "Gestionado",""));
                }
                return list;
            }

            public List<RealPointSeries> GetRechazados()
            {
                Controllers.ValidationController oValidation = new Controllers.ValidationController();
                DateTime Inicio = FechaConsulta;
                List<RealPointSeries> list = new List<RealPointSeries>();
                    var oDatos = (from pr in ListaMan
                                  where pr.ESTADO == "RECHAZO TOTAL"
                                  && pr.RUT_CONDUCTOR == RutConductor
                                  && pr.PATENTE == Patente
                                  select pr).ToList();
                    foreach (var dato in oDatos)
                    {
                        list.Add(new RealPointSeries(dato.FECHA_GESTION.HasValue? dato.FECHA_GESTION.Value: Inicio, 3,"Rechazado" , dato.OTP.ToString() + "-" + dato.OTD.ToString() + "<br/>" + "posición: " +  dato.COMUNA));
                    }
                return list;
            }

            public List<RealPointSeries> GetRecepcionados()
            {
                Controllers.ValidationController oValidation = new Controllers.ValidationController();
                DateTime Inicio = FechaConsulta;

                List<RealPointSeries> list = new List<RealPointSeries>();

                var oDatos = (from pr in ListaMan
                              where pr.ESTADO == "RECEPCIONADO OK"
                              && pr.RUT_CONDUCTOR == RutConductor
                              && pr.PATENTE == Patente
                              select pr).ToList();
                foreach (var dato in oDatos)
                {
                    list.Add(new RealPointSeries(dato.FECHA_GESTION.HasValue? dato.FECHA_GESTION.Value : Inicio,
                                                2, 
                                                "Recepcionado OK", 
                                                dato.OTP.ToString() + "-" + dato.OTD.ToString() + "<br/>" + "posición: " + dato.COMUNA)
                                                );
                }
                return list;
            }
            public List<RealPointSeries> GetEnRutaCliente()
            {
                Controllers.ValidationController oValidation = new Controllers.ValidationController();
                DateTime Inicio = FechaConsulta;
           
                var oDatos = (from pr in ListaMan
                              where pr.ESTADO == "EN RUTA CLIENTE"
                              && pr.FECHA_GESTION == null
                              && pr.RUT_CONDUCTOR == RutConductor
                              && pr.PATENTE == Patente
                              select pr).ToList();
                
                Random random = new Random();
                return GetRandomPoints(random
                                        , ListaMan.Where(lm => lm.RUT_CONDUCTOR == RutConductor && lm.PATENTE == Patente).Count()
                                        , ListaMan.Where(lm => lm.RUT_CONDUCTOR == RutConductor && lm.PATENTE == Patente).Count() - oDatos.Count()
                                        , ListaMan.Where(lm => lm.RUT_CONDUCTOR == RutConductor && lm.PATENTE == Patente).Count()
                                        , 0
                                        , 1
                                        , 3);
           
            }

            public List<RealPointSeries> GetNoEncontrado()
            {
                Controllers.ValidationController oValidation = new Controllers.ValidationController();
                DateTime Inicio = FechaConsulta;
                List<RealPointSeries> list = new List<RealPointSeries>();

                var oDatos = (from pr in ListaMan
                              where (pr.ESTADO == "EN RUTA CLIENTE" || pr.ESTADO == "EN RUTA") 
                              && pr.FECHA_GESTION != null
                              && pr.RUT_CONDUCTOR == RutConductor
                              && pr.PATENTE == Patente
                              select pr).ToList();
                foreach (var dato in oDatos)
                {
                    list.Add(new RealPointSeries(dato.FECHA_GESTION.HasValue ? dato.FECHA_GESTION.Value : Inicio,
                                                4,
                                                "Cliente No Encontrado",
                                                dato.OTP.ToString() + "-" + dato.OTD.ToString() + "<br/>" + "posición: " + dato.COMUNA)
                                                );
                }
                return list;

            }
            public List<RealPointSeries> GetGestionados()
            {
                Controllers.ValidationController oValidation = new Controllers.ValidationController();
                DateTime Inicio = FechaConsulta;
                List<RealPointSeries> list = new List<RealPointSeries>();

                var oDatos = (from pr in ListaMan
                              where pr.FECHA_GESTION != null
                              && pr.RUT_CONDUCTOR == RutConductor
                              && pr.PATENTE == Patente
                              select pr).ToList();

                Random random = new Random();
                return GetRandomPoints(random
                                        , ListaMan.Where(lm => lm.RUT_CONDUCTOR == RutConductor && lm.PATENTE == Patente).Count()
                                        , 0
                                        , oDatos.Count()
                                        , 0
                                        , 1
                                        , 0);
            }
      
        }

}