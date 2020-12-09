using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Courier.Models;
using System.Net;
using System.Xml.Linq;


namespace Courier.Controllers
{
    public class MapOTController : Controller
    {

        public ActionResult Index()
        {       
            return View();
        }

        public ActionResult _DropDownEstados(MapOtModels oModels)
        {
            oModels.GetListaEstados();
            return PartialView("_DropDownEstados", oModels);
        }

        [HttpGet]
        public ActionResult ListaOt(MapOtModels oModels)
        {
            if (oModels.Estado == null)
            {
                oModels.Estado = "";
            }
            oModels.resultLista = oModels.GetListaResultados();
            return PartialView("_ListaOt", oModels);
        }

        [Authorize]
        public ActionResult BuscaLocalizacion(string OT, string Direccion, string Numero, string Comuna)
        {
            RetornoBoolModels oRetorno = new RetornoBoolModels();
            oRetorno.Ok = true;
            oRetorno.Mensaje = "Coordenadas Actualizadas!"; 

            //servicio api google
            var address = "";
            address = Direccion + " " + Numero + ", " + Comuna;
            var requestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false", Uri.EscapeDataString(address));

            var request = WebRequest.Create(requestUri);
            var response = request.GetResponse();
            var xdoc = XDocument.Load(response.GetResponseStream());
            var status = xdoc.Element("GeocodeResponse").Element("status").ToString();
            status = status.Replace("<status>", "").Replace("</status>", "");
            if (status == "OK")
            {
                var result = xdoc.Element("GeocodeResponse").Element("result");
                if (result != null)
                {
                    var locationElement = result.Element("geometry").Element("location");
                    if (locationElement != null)
                    {
                        var lat = locationElement.Element("lat").ToString().Replace("<lat>", "").Replace("</lat>", "");
                        var lng = locationElement.Element("lng").ToString().Replace("<lng>", "").Replace("</lng>", "");
                        string[] otDetalle = OT.Split('-');

                        try
                        {
                            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                            oFol.PR_FOL_UPD_LOCALIZACION_OT(otDetalle[0], otDetalle[1],lat ,lng);

                        }
                        catch (Exception e)
                        {
                            oRetorno.Ok = true;
                            oRetorno.Mensaje = "Error: " + e.Message;              
                            throw;
                        }
                    }
                }
            }
            else
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = "No se logró ubicar la dirección ingresada. Intente editando los datos de ingresados."; 
            }

            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult BuscaDireccion( string Direccion, string Numero, string Comuna)
        {
            RetornoLocationModels oRetorno = new RetornoLocationModels();
            oRetorno.Ok = true;
            oRetorno.Mensaje = "Coordenadas Actualizadas!";

            //servicio api google
            var address = "";
            address = Direccion + " " + Numero + ", " + Comuna;
            var requestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false", Uri.EscapeDataString(address));

            var request = WebRequest.Create(requestUri);
            var response = request.GetResponse();
            var xdoc = XDocument.Load(response.GetResponseStream());
            var status = xdoc.Element("GeocodeResponse").Element("status").ToString();
            status = status.Replace("<status>", "").Replace("</status>", "");
            if (status == "OK")
            {
                var result = xdoc.Element("GeocodeResponse").Element("result");
                if (result != null)
                {
                    var locationElement = result.Element("geometry").Element("location");
                    if (locationElement != null)
                    {
                        var lat = locationElement.Element("lat").ToString().Replace("<lat>", "").Replace("</lat>", "");
                        var lng = locationElement.Element("lng").ToString().Replace("<lng>", "").Replace("</lng>", "");
                        oRetorno.Lat = lat;
                        oRetorno.Lng = lng;
                    }
                }
            }
            else
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = "No se logró ubicar la dirección ingresada. Intente editando los datos de ingresados.";
            }

            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult EditaDireccionPopup(string OT)
        {
            string[] datosOT = OT.Split('-');

            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var miOT = oFol.PR_FOL_LOCALIZACION_OT_SIMPLE(datosOT[1], datosOT[0]).Single();

            MapOtModels.RetornoDireccion miDireccion = new MapOtModels.RetornoDireccion(OT, miOT.DIRECCION, miOT.NUMERO, miOT.LATITUD.ToString(), miOT.LONGITUD.ToString(), miOT.COMUNA);
            return PartialView("_EditaDireccion", miDireccion);//agregar model o clase para pasar a vista
        }

        [Authorize]
        public ActionResult ActualizaDireccion(string OT, string Direccion, string Numero, string Lat, string Lng)
        {
            RetornoBoolModels oRetorno = new RetornoBoolModels();
            oRetorno.Ok = true;
            oRetorno.Mensaje = "Datos Actualizados!";
            string[] miOT = OT.Split('-');

          
            try
            {
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                oFol.PR_FOL_UPD_LOCALIZACION_OT_DIRECCION(miOT[0], miOT[1],Lat, Lng, Direccion, Numero);

            }
            catch (Exception e)
            {
                oRetorno.Ok = true;
                oRetorno.Mensaje = "Error: " + e.Message;
                throw;
            }


            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }

        //[HttpGet]
        //public ActionResult GetMarkersAsync()
        //{
        //    return Json(MarkerRepository.GetMarkers());
        //}

        [Authorize]
        public ActionResult VerMapa(string OT)
        {

            string[] datosOT = OT.Split('-');

            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var miOT = oFol.PR_FOL_LOCALIZACION_OT_SIMPLE(datosOT[1], datosOT[0]).Single();
            MapOtModels.RetornoDireccion miDireccion = new MapOtModels.RetornoDireccion(OT, miOT.DIRECCION, miOT.NUMERO, miOT.LATITUD.ToString(), miOT.LONGITUD.ToString());
            
            return PartialView("_MapLocation", miDireccion);//agregar model o clase para pasar a vista
        }

    }
}
