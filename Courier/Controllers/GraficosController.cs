using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Courier.Models;
using System.Web.Script.Serialization;
using System.Text;
using Google.DataTable.Net.Wrapper;

namespace Courier.Controllers
{
    public class GraficosController : Controller
    {
        //
        // GET: /Graficos/

        public string jSONDatosPieChart()
        {
            Linq_SPDataContext oSP = new Linq_SPDataContext();
            var oDatos = oSP.PR_FOL_GRA_OTS_GRAL_DIA().ToList();
                        
        

            //let's instantiate the DataTable.
            var dt = new Google.DataTable.Net.Wrapper.DataTable();
            dt.AddColumn(new Column(ColumnType.String, "Estado", "Estado"));
            dt.AddColumn(new Column(ColumnType.Number, "Cantidad", "Cantidad"));

            foreach (var item in oDatos)
            {
                Row r = dt.NewRow();
                r.AddCellRange(new Cell[]
                {
                    new Cell(item.ESTADO),
                    new Cell(item.CANTIDAD)
                });
                dt.AddRow(r);
            }

            //Let's create a Json string as expected by the Google Charts API.
            return dt.GetJson();
        }
        
        public ActionResult Index()
        {
            return PartialView();
        }
        

        #region ListaOT
        public ActionResult _ListaOT()
        {
            GraficosModels oModel = new GraficosModels();
            oModel.getListaOT();
            return PartialView(oModel);
        }
        #endregion

        #region WebGraficosSamsung
        public ActionResult WebGraficosSamsung()
        {
            return Redirect("../Graficos/GraficoSamsung.aspx");
        }
        #endregion


        #region PanelGraficos
        public ActionResult PanelGraficos()
        {
            return PartialView();
        }            
        #endregion

        public ActionResult Contenedor()
        {
            return View();
        }

        public ActionResult Grafico()
        {
            return PartialView();
        }

        public ActionResult ContenedorOT()
        {
            return PartialView("_ContenedorListaOT");
        }
    }
}
