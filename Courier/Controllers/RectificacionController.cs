using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Courier.Models;

namespace Courier.Controllers
{
    public class RectificacionController : Controller
    {
        //
        // GET: /Rectificacion/

        public ActionResult RecepcionOk()
        {
            return View();
        }
        public ActionResult RecepcionOkModificar(RectificacionRecepcionModels oModel)
        {
            oModel.GetUltimaFecha();
            oModel.GetListasHora();
            oModel.GetListaObservacion();
            return PartialView(oModel);
        }
        public ActionResult ValidarGuardar(RectificacionRecepcionModels oModel)
        {
            RetornoFechaModels oRetorno = new RetornoFechaModels();

            string oFecha = oModel.FechaEntrega + " " + oModel.Hora + ":" + oModel.Minutos;

            ValidationController oVAlidato = new ValidationController();

            DateTime oResult;

            oModel.GetUltimaFecha();

            if (!DateTime.TryParse(oFecha, out oResult))
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = "Fecha Incorrecta";
            }
            else if ((oResult - oModel.UltimaFecha).Minutes <= 0)
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = "La fecha debe ser mayor (en al menos 1 minuto), a la última fecha registrada.";
            }
            else
            {
                oRetorno.Ok = true;
                oRetorno.Fecha = oResult;
            }
            

            return Json(oRetorno,JsonRequestBehavior.AllowGet);
        }
    }
}
