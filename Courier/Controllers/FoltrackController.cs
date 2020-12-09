using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using Courier.Models;
namespace Courier.Controllers
{
    public class FoltrackController : Controller
    {
        public ActionResult MonitorConductores( FolTrackModels oModels)
        {
            // DXCOMMENT: Pass a data model for GridView
            // oModels.FechaConsulta = DateTime.Now;
            oModels.GetListaManifiestos();
            return View("MonitorConductores", oModels);
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ChartPartial(FolTrackModels oModels)
        {
            //oModels.GetListaManifiestos();
            oModels.ListaManifiestos = oModels.GetDatosSeries();

            return PartialView("_ChartPartial", oModels);
        }
        [HttpGet]
        public ActionResult CalendarPartial(FolTrackModels oModels)
        {
            return PartialView("_CalendarPartial", oModels);
        }
    }
   
}