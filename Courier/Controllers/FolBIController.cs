using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using Courier.Models;
using System.Threading;

namespace Courier.Controllers
{
    public class FolBIController : Controller
    {
        private static FolBIModels omodel = null;

        public static FolBIModels oModel
        {
            get
            {
                if (omodel == null)
                    omodel = new FolBIModels();

                return omodel;
            }
        }

        public ActionResult Despachos(FolBIModels oModels)
        {
            oModels = oModel;
            return View("Despachos", oModel);
        }

        public ActionResult Detalles(FolBIModels oModels)
        {
            oModels = oModel;
            return View("Detalles", oModel);
        }

        [HttpGet]
        public ActionResult Index()
        {
            FolBIModels modelo = oModel;
            return View(modelo);
        }

        [HttpGet]
        public ActionResult DonutGlobalPartial(FolBIModels oModels)
        {
            oModel.FechaPeriodo = DateTime.Parse(oModels.FechaPeriodo).ToString("yyyy-MM-dd h:mm tt");
            oModel.FechaPeriodoTermino = DateTime.Parse(oModels.FechaPeriodoTermino).ToString("yyyy-MM-dd h:mm tt");
            oModel.listaReporteGlobal = oModel.GetReporteDespachosGlobales();
            oModel.listaReportes = oModel.GetReporteDespachosByServer(); ;
            return PartialView("_DonutGlobalPartial", oModel);
        }

        [HttpGet]
        public ActionResult BarDespachosPartial(FolBIModels oModels)
        {
            oModel.FiltroEstado = oModels.FiltroEstado;
            oModel.FiltroConductor = oModels.FiltroConductor;
            oModel.listaReportesDiaMes = oModel.GetReporteDespachosDiaMesByServer();

            return PartialView("_BarDespachosPartial", oModel);
        }

        [HttpGet]
        public ActionResult BarDespachosHoraDíaPartial(FolBIModels oModels)
        {
            oModel.FiltroEstado = oModels.FiltroEstado;
            oModel.FiltroConductor = oModels.FiltroConductor;
            oModel.listaReporteHora = null;
            oModel.listaReporteHora = oModel.GetReporteDespachosHoraByServer();

            return PartialView("_BarDespachosHoraDíaPartial", oModel);
        }

        [HttpGet]
        public ActionResult BarDespachosDiaSemanaPartial(FolBIModels oModels)
        {
            oModel.FiltroEstado = oModels.FiltroEstado;
            oModel.FiltroConductor = oModels.FiltroConductor;
            oModel.listaReporteDiaSemana = null;
            oModel.listaReporteDiaSemana = oModel.GetReporteDespachosDiaSemanaByServer();

            return PartialView("_BarDespachosDiaSemanaPartial", oModel);
        }

        [HttpGet]
        public ActionResult BarDespachosConductores(FolBIModels oModels)
        {
            oModel.FiltroEstado = oModels.FiltroEstado;
            oModel.listaReporteConductores = oModel.GetReporteDespachosConductoresByServer();

            return PartialView("_BarDespachosConductores", oModel);
        }

        public ActionResult LoadingPanel()
        {
            if (DevExpressHelper.IsCallback)
                Thread.Sleep(500);
            return PartialView("_CallBackPartial");
        }

        [HttpGet]
        public ActionResult ListaDetalles(FolBIModels oModels)
        {
            oModels.listaReporteGlobalDetalle = oModel.listaReporteGlobal;

            if (oModel.FiltroEstado != null && oModel.FiltroEstado != "")
            {
                oModels.listaReporteGlobalDetalle = (from i in oModels.listaReporteGlobalDetalle
                                                     where i.EstadoNombre == oModel.FiltroEstado
                                                     select i).ToList();
            }

            if (oModel.FiltroConductor != null && oModel.FiltroConductor != "")
            {
                oModels.listaReporteGlobalDetalle = (from i in oModels.listaReporteGlobalDetalle
                                                    where i.NombreConductor == oModel.FiltroConductor
                                                    select i).ToList();
            }

            return PartialView("_ListaDetalles", oModels);
        }

    }
}
