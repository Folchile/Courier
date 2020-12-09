using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Courier.Helpers;
using Courier.Models;

namespace Courier.Controllers
{
    public class HomeController : Controller
    {
        public double Convertir(string variable)
        {            

            var grados= variable.Substring(0,2);
            var min=variable.Substring(3,2);
            var seg=variable.Substring(6,2);

            double total = 0;

            total = Convert.ToDouble(grados);
            total += Convert.ToDouble(min) / 60;
            total += Convert.ToDouble(seg) / 3600;

            return (total);
        }


        [HttpGet]
        public ActionResult  Distancia(string Origen, string Destino)
        {
            ComunasCoordenadasModels oModel = new ComunasCoordenadasModels();

            if (Origen != null && Destino != null)
            {

                
                LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

                var dOrigen = ( from com in oPar.TB_PAR_COM_COMUNA
                                join prv in oPar.TB_PAR_PRV_PROVINCIA on com.PK_PAR_PRV_ID equals prv.PK_PAR_PRV_ID
                                join reg in oPar.TB_PAR_REG_REGION on prv.PK_PAR_REG_ID equals reg.PK_PAR_REG_ID
                                join pai in oPar.TB_PAR_PAI_PAIS on reg.PK_PAR_PAI_ID equals pai.PK_PAR_PAI_ID
                                where com.PK_PAR_COM_ID==Convert.ToInt32(Origen)
                                select new {
                                    com.FL_PAR_COM_NOMBRE,
                                    prv.FL_PAR_PRV_NOMBRE,
                                    reg.FL_PAR_REG_NOMBRE,
                                    pai.FL_PAR_PAI_NOMBRE
                                }).Single();



                var cOrigen = ValidationController.BuscarDireccion("Comuna de " + dOrigen.FL_PAR_COM_NOMBRE + "," + dOrigen.FL_PAR_PRV_NOMBRE + "," + dOrigen.FL_PAR_REG_NOMBRE + "," + dOrigen.FL_PAR_PAI_NOMBRE);


                var lat1 = cOrigen.Latitud;
                var lon1 = cOrigen.Longitud;


                var dDestino = (from com in oPar.TB_PAR_COM_COMUNA
                               join prv in oPar.TB_PAR_PRV_PROVINCIA on com.PK_PAR_PRV_ID equals prv.PK_PAR_PRV_ID
                               join reg in oPar.TB_PAR_REG_REGION on prv.PK_PAR_REG_ID equals reg.PK_PAR_REG_ID
                               join pai in oPar.TB_PAR_PAI_PAIS on reg.PK_PAR_PAI_ID equals pai.PK_PAR_PAI_ID
                               where com.PK_PAR_COM_ID == Convert.ToInt32(Destino)
                               select new
                               {
                                   com.FL_PAR_COM_NOMBRE,
                                   prv.FL_PAR_PRV_NOMBRE,
                                   reg.FL_PAR_REG_NOMBRE,
                                   pai.FL_PAR_PAI_NOMBRE
                               }).Single();


                var cDestino = ValidationController.BuscarDireccion("Comuna de "+ dDestino.FL_PAR_COM_NOMBRE + "," + dDestino.FL_PAR_PRV_NOMBRE + "," + dDestino.FL_PAR_REG_NOMBRE + "," + dDestino.FL_PAR_PAI_NOMBRE);

                var lat2 = cDestino.Latitud;
                var lon2 = cDestino.Longitud;


                double olat1 = Convert.ToDouble((lat1));
                double olat2 = Convert.ToDouble((lat2));
                double olong1 = Convert.ToDouble((lon1));
                double olong2 = Convert.ToDouble((lon2));


                double degtorad = 0.01745329;
                double radtodeg = 57.29577951;
                double dlong = olong1 - olong2;
                double dvalue = (Math.Sin(olat1 * degtorad) * Math.Sin(olat2 * degtorad)) + (Math.Cos(olat1 * degtorad) * Math.Cos(olat2 * degtorad) * Math.Cos(dlong * degtorad));

                double dd = Math.Acos(dvalue) * radtodeg;

                double kilometros = (dd * 111.302);

                
                oModel.cOrigen  = new ComunasCoordenadasModels.Coordenada {
                                        Latitud=olat1.ToString(),
                                        Longitud=olong1.ToString()
                                        };
                
                oModel.cDestino = new ComunasCoordenadasModels.Coordenada{
                    Latitud = olat2.ToString(),
                    Longitud = olong2.ToString()
                };

                oModel.Kilometros = kilometros;
            }
           
            return PartialView("Distancia",oModel);
        }


        [Authorize]
        public ActionResult Index()
        {
            ViewBag.Message = "Bienvenido";


            //LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            //var oLista = from m in oPar.TB_PAR_COM_COMUNA                         
            //             orderby m.FL_PAR_COM_NOMBRE
            //             select new SelectListItem
            //             {
            //                 Text=m.FL_PAR_COM_NOMBRE,
            //                 Value=m.PK_PAR_COM_ID.ToString()
            //             };

            //ComunasCoordenadasModels oModel = new ComunasCoordenadasModels();

            //oModel.oListado = oLista;            
            SucursalIngresoModels oModel = new SucursalIngresoModels();
            oModel.GetListaSucursales();
            return View(oModel);
        }

        #region About
        public ActionResult About()
        {
            return View();
        }
        #endregion
        #region Ayuda
        [Authorize]
        public ActionResult Ayuda()
        {
            return View();
        }
        #endregion

        public ActionResult UnderConstruction()
        {
            ViewBag.title = "Mantenedor";
            return View();
        }

        public string Enviar()
        {
            Funciones oFunciones = new Funciones();

            var oResult = oFunciones.EnviarCorreo("fcorojasc@gmail.com", "prueba", "Prueba");

            return "hola";
        }
        public ActionResult Version()
        {
            return View();
        }
        public ActionResult CambiarSucursalUsuario(SucursalIngresoModels oModel)
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            RetornoModels oRetorno = new RetornoModels();
            ValidationController oValidation = new ValidationController();
            try
            {
                var oResult = oPar.PR_PAR_CAMBIA_SUC_USUARIO(oValidation.GetRutActiveUser(), Convert.ToInt32(oModel.Sucursal));
                var oDatosActual = (from usu in oPar.TB_PAR_USU_USUARIO
                                    join suc in oPar.TB_PAR_SUC_SUCURSAL on usu.PK_PAR_SUC_ID equals suc.PK_PAR_SUC_ID
                                   where usu.PK_PAR_USU_RUT == oValidation.GetRutActiveUser()
                                   select suc).Single();

                Session["SucName2"] = oDatosActual.FL_PAR_SUC_NOMBRE;
                if (oDatosActual.PK_PAR_TUS_ID == 1)
                {
                    Session["RutEmpresa"] = null;
                    Session["SucName"] = oDatosActual.FL_PAR_SUC_NOMBRE;                    
                }
                else
                {
                    LinqCLIDataContext oCli = new LinqCLIDataContext();
                    var oEmpresa = (from emp in oCli.TB_CLI_EMP_EMPRESAS
                                   where emp.PK_CLI_EMP_RUT == oDatosActual.PK_CLI_EMP_RUT
                                   select emp).Single();

                    Session["RutEmpresa"] = oEmpresa.PK_CLI_EMP_RUT + '-' + oEmpresa.FL_CLI_EMP_DV;
                    Session["SucName"] = oEmpresa.FL_CLI_EMP_FANTASIA;
                }                
                

                oRetorno.Ok = true;
            }
            catch (Exception e)
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje="No fue posible guardar la información<br>" + e.Message;
            }                                      

            oModel.SeleccionSucursal = false;

            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }

    }
}
