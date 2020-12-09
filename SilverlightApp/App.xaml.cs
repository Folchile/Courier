using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Browser;

namespace SilverlightApp
{
    public partial class App : Application
    {

        public App()
        {
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;

            InitializeComponent();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            HtmlPage.RegisterScriptableObject("ControlWMS", new SilverJs());            

            string idBulto = string.Empty;
            string idOTP = string.Empty;
            string idOTD = string.Empty;
            string Ref1 = string.Empty;
            string Ref2 = string.Empty;
            string Impresora = string.Empty;
            string Configuracion = string.Empty;
            string RutUsuario = string.Empty;
            string Pallet = string.Empty;
            string blt = string.Empty;


            string CantidadBultos = string.Empty;
            string ComunaDestino = string.Empty;
            string SucursalDestino = string.Empty;

            int Cantidad = 0;
            int Etiqueta = 0;

            string Bulto1Cantidad = string.Empty;
            string Bulto1Codigo = string.Empty;
            string Bulto1Peso=string.Empty;

            string Bulto2Cantidad = string.Empty;
            string Bulto2Codigo = string.Empty;
            string Bulto2Peso = string.Empty;

            string Bulto3Cantidad = string.Empty;
            string Bulto3Codigo = string.Empty;
            string Bulto3Peso = string.Empty;

            string Bulto4Cantidad = string.Empty;
            string Bulto4Codigo = string.Empty;
            string Bulto4Peso = string.Empty;

            if (e.InitParams.ContainsKey("idBulto"))
                idBulto = e.InitParams["idBulto"];
            if (e.InitParams.ContainsKey("OTP"))
                idOTP = e.InitParams["OTP"];
            if (e.InitParams.ContainsKey("OTD"))
                idOTD = e.InitParams["OTD"];
            if (e.InitParams.ContainsKey("REF1"))
                Ref1 = e.InitParams["REF1"];
            if (e.InitParams.ContainsKey("REF2"))
                Ref2 = e.InitParams["REF2"];
            if (e.InitParams.ContainsKey("PALLET"))
                Pallet = e.InitParams["PALLET"];
            if (e.InitParams.ContainsKey("BLT"))
                blt = e.InitParams["BLT"];


            if (e.InitParams.ContainsKey("CantidadBultos"))
                CantidadBultos = e.InitParams["CantidadBultos"];
            if (e.InitParams.ContainsKey("ComunaDestino"))
                ComunaDestino = e.InitParams["ComunaDestino"];
            if (e.InitParams.ContainsKey("SucursalDestino"))
                SucursalDestino = e.InitParams["SucursalDestino"];


            if (e.InitParams.ContainsKey("Impresora"))
                Impresora = e.InitParams["Impresora"];
            if (e.InitParams.ContainsKey("Configuracion"))
                Configuracion = e.InitParams["Configuracion"];
            if (e.InitParams.ContainsKey("RutUsuario"))
                RutUsuario = e.InitParams["RutUsuario"];
            if (e.InitParams.ContainsKey("Cantidad"))
                Cantidad = Convert.ToInt32(e.InitParams["Cantidad"]);

            ImpresionCustom oImpresion = new ImpresionCustom
            {               
                Cantidad = Cantidad,
                TipoEtiqueta = Etiqueta,
                CantidadBultos=CantidadBultos,
                ComunaDestino=ComunaDestino,
                SucursalDestino=SucursalDestino
            };


            List<DatosBulto> oListDB = new List<DatosBulto>();
            for (int c = 1; c <= Cantidad; c++)
            {                
                if (e.InitParams.ContainsKey("Bulto" + c + "Cantidad"))
                {
                    DatosBulto oDB  = new DatosBulto
                    {
                        CantidadBulto = e.InitParams["Bulto" + c + "Cantidad"],
                        CodigoBulto = Convert.ToDecimal(e.InitParams["Bulto" + c + "Codigo"]),
                        Peso = e.InitParams["Bulto" + c + "Peso"].Replace("_",",")
                    };
                    oListDB.Add(oDB);

                }
            }
            oImpresion.Datos = oListDB;
            

            var oOpcion = Configuracion;

            if (oOpcion == "1" || oOpcion == "4" || oOpcion == "5" || oOpcion=="6" || oOpcion=="88")
                this.RootVisual = new MainPage(idBulto, idOTD, idOTP, Ref1, Ref2, Impresora, oOpcion, oImpresion, Pallet,blt);
            else if (oOpcion=="3")
                this.RootVisual = new PrintbyBrowser();
            else if (oOpcion=="2")
                this.RootVisual = new Printer(Impresora,Configuracion, Convert.ToInt32(RutUsuario));
            else if (oOpcion == "90")
                this.RootVisual = new MainPage(idBulto, idOTD, idOTP, Ref1, Ref2, Impresora, oOpcion, oImpresion, Pallet,blt);
            else if (oOpcion=="9909" && oImpresion.Cantidad>0)
                this.RootVisual = new MainPage(idBulto, idOTD, idOTP, Ref1, Ref2, Impresora, oOpcion, oImpresion, Pallet,blt);
        }
        
        private void Application_Exit(object sender, EventArgs e)
        {

        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // Si la aplicación se está ejecutando fuera del depurador, informe de la excepción mediante
            // el mecanismo de excepciones del explorador. En IE se mostrará un icono de alerta amarillo 
            // en la barra de estado y en Firefox se mostrará un error de script.
            if (!System.Diagnostics.Debugger.IsAttached)
            {

                // NOTA: esto permitirá a la aplicación continuar ejecutándose después de que una excepción se haya producido
                // pero no controlado. 
                // Para las aplicaciones de producción, este control de errores se debe reemplazar por algo que 
                // informará del error al sitio web y detendrá la aplicación.
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
            }
        }

        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
            }
            catch (Exception)
            {
            }
        }
    }
}
