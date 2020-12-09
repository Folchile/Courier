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
using System.Windows.Printing;
using System.Net.Sockets;
using System.Windows.Navigation;
using SilverlightEnumPrinters;
using System.Runtime.InteropServices;
using System.Windows.Browser;
using System.Data.Services.Client;
using SilverlightApp.ReferenciaServicioWeb;
using System.IO;
using System.Windows.Media.Imaging;
using System.ComponentModel;



namespace SilverlightApp
{
    

    public class DatosBulto
    {
        public decimal CodigoBulto { get; set; }
        public string Peso { get; set; }
        public string CantidadBulto { get; set; }
    }
    public class ImpresionCustom
    {
        public int TipoEtiqueta { get; set; }
        public int Cantidad { get; set; }
        public string CantidadBultos { get; set; }
        public string ComunaDestino { get; set; }
        public string SucursalDestino { get; set; }



        public List<DatosBulto> Datos { get; set; }

    }
    public partial class MainPage : UserControl
    {
        public string userNombreImpresora;
        public string useroOpcion;
        public string userIdBulto;
                
        public MainPage(string idBulto, string OTD, string OTP, string REF1, string REF2, string Impresora, string oOpcion, ImpresionCustom oImpresion, string Pallet, string blt)
        {
            InitializeComponent();
            useroOpcion = oOpcion;
            userNombreImpresora = Impresora;
            userIdBulto = idBulto;
            ServicioSilverlightClient oServicio = new ServicioSilverlightClient();
            if (useroOpcion == "6")
            {
                oServicio.ImpresionGuiaMasivaCompleted += new EventHandler<ImpresionGuiaMasivaCompletedEventArgs>(oServicio_ImpresionGuiaMasivaCompleted);
                oServicio.ImpresionGuiaMasivaAsync(Convert.ToDecimal(OTP));
            }
            else if (useroOpcion == "88")
            {
                oServicio.ImpresionEtiquetaPorOTPCompleted+=new EventHandler<ImpresionEtiquetaPorOTPCompletedEventArgs>(oServicio_ImpresionEtiquetaPorOTPCompleted);
                oServicio.ImpresionEtiquetaPorOTPAsync(Convert.ToDecimal(OTP));
            }
            else if (useroOpcion == "90")
            {
                oServicio.ImpresionEtiquetaPalletCompleted+=new EventHandler<ImpresionEtiquetaPalletCompletedEventArgs>(oServicio_ImpresionEtiquetaPalletCompleted);
                oServicio.ImpresionEtiquetaPalletAsync(Convert.ToDecimal(OTP), Convert.ToDecimal(OTD), Convert.ToInt32(Pallet),  Convert.ToInt32(blt));
                //oServicio.ImpresionEtiquetaPorOTPCompleted += new EventHandler<ImpresionEtiquetaPorOTPCompletedEventArgs>(oServicio_ImpresionEtiquetaPorOTPCompleted);
                //oServicio.ImpresionEtiquetaPorOTPAsync(Convert.ToDecimal(OTP));
            }
            else if (useroOpcion == "9909")
            {
                List<TipoEtiquetaImpresion> oLista = new List<TipoEtiquetaImpresion>();
                //TipoEtiqueta 1:Grande 2:Normal 3:Guia    
                if (oImpresion.Datos != null)
                {
                    foreach (var oFila in oImpresion.Datos.OrderBy(m=>m.CodigoBulto))
                    {
                        oLista.Add(new TipoEtiquetaImpresion
                        {
                            TipoEtiqueta = 2,
                            OTP = OTP,
                            OTD = OTD,
                            CantidadBultos = oImpresion.CantidadBultos,
                            Cliente = "Sin Datos",
                            Comuna = oImpresion.ComunaDestino.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U').Replace('ñ', 'n').Replace('Ñ', 'N'),
                            Localidad = "Sin Datos",
                            Direccion = "Sin Datos",
                            idBulto = Convert.ToDecimal(oFila.CodigoBulto),
                            NumeroBulto = oFila.CantidadBulto,
                            Referencia1 = "Sin Datos",
                            Referencia2 = "Sin Datos",
                            Servicio = "",
                            SucursalDestino = oImpresion.SucursalDestino.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U').Replace('ñ', 'n').Replace('Ñ', 'N'),
                            Telefono = "",
                            Via = "",
                            Peso = oFila.Peso
                        });

                    }
                    ImpresionEtiqueta(oLista);
                }
                //oServicio.ImpresionCustomCompleted += new EventHandler<ImpresionCustomCompletedEventArgs>(oServicio_ImpresionCustomCompleted);
                //oServicio.ImpresionCustomAsync(Convert.ToDecimal(OTP),Convert.ToDecimal(OTD), oImpresion.Cantidad, oImpresion.TipoEtiqueta, oImpresion.Bulto1, oImpresion.Bulto2, oImpresion.Bulto3, oImpresion.Bulto4);
            }
            else
            {
                oServicio.ImpresionEtiquetaCompleted += new EventHandler<ImpresionEtiquetaCompletedEventArgs>(oServicio_ImpresionEtiquetaCompleted);
                oServicio.ImpresionEtiquetaAsync(Convert.ToDecimal(OTP), Convert.ToDecimal(OTD));
            }
        }

        void oServicio_ImpresionCustomCompleted(object sender, ImpresionCustomCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                List<TipoEtiquetaImpresion> oLista = new List<TipoEtiquetaImpresion>();
                //TipoEtiqueta 1:Grande 2:Normal 3:Guia                
                foreach (var oFila in e.Result.ListaDetalle)
                {
                    oLista.Add(new TipoEtiquetaImpresion
                    {
                        TipoEtiqueta = oFila.TipoEtiqueta,
                        OTP = e.Result.OTP.ToString(),
                        OTD = e.Result.OTD.ToString(),
                        CantidadBultos = e.Result.CantidadBultos.ToString(),
                        Cliente = e.Result.Cliente.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U').Replace('ñ', 'n').Replace('Ñ', 'N'),
                        Comuna = e.Result.Comuna.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U').Replace('ñ', 'n').Replace('Ñ', 'N'),
                        Localidad = e.Result.Localidad,
                        Direccion = e.Result.Direccion.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U').Replace('ñ', 'n').Replace('Ñ', 'N'),
                        idBulto = oFila.idBulto,
                        NumeroBulto = oFila.NumeroBulto.ToString(),
                        Referencia1 = e.Result.Referencia1,
                        Referencia2 = e.Result.Referencia2,
                        Servicio = e.Result.Servicio.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U').Replace('ñ', 'n').Replace('Ñ', 'N'),
                        SucursalDestino = e.Result.SucursalDestino.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U').Replace('ñ', 'n').Replace('Ñ', 'N'),
                        Telefono = e.Result.Telefono,
                        Via = e.Result.Via.Replace('í', 'i').Replace('é', 'e'),
                        Peso=oFila.Peso.ToString(),
                    });                        
                    
                }                
                ImpresionEtiqueta(oLista);
                //MessageBox.Show("Impresión Ok");   
            }
        }
        
        void oServicio_ImpresionGuiaMasivaCompleted(object sender, ImpresionGuiaMasivaCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                List<TipoEtiquetaImpresion> oLista = new List<TipoEtiquetaImpresion>();    
                foreach (var oFila in e.Result)
                {
                    int oCont = 1;
                    while (oCont <= 4)
                    {
                        oLista.Add(new TipoEtiquetaImpresion
                        {
                            TipoEtiqueta = 3,
                            OTD = oFila.OTD.ToString(),
                            OTP = oFila.OTP.ToString(),
                            Referencia1 = oFila.Referencia1
                        });
                        oCont++;
                    }
                }
                ImpresionEtiqueta(oLista);
            }
        }
        void oServicio_ImpresionEtiquetaPalletCompleted(object sender,ImpresionEtiquetaPalletCompletedEventArgs e)
        {
            if (e.Error == null)
            {

                ImpresionEtiquetaPallet(new List<TipoEtiquetaImpresion> {
                    new TipoEtiquetaImpresion{
                        CantidadBultos=e.Result.CantidadBultos.ToString(),
                        Cliente=e.Result.Cliente.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U').Replace('ñ', 'n').Replace('Ñ', 'N'),
                        OTD=e.Result.OTD.ToString(),
                        OTP=e.Result.OTP.ToString(),
                        Localidad=e.Result.Localidad,
                        Comuna=e.Result.Comuna.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U').Replace('ñ', 'n').Replace('Ñ', 'N'),
                        Direccion=e.Result.Direccion.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U').Replace('ñ', 'n').Replace('Ñ', 'N'),
                        Pallet=e.Result.Pallet.ToString(),
                        Referencia1=e.Result.Referencia1,
                        Referencia2=e.Result.Referencia2,
                        Telefono=e.Result.Telefono,
                        Servicio=e.Result.Servicio
                    }
                });
                        
                        //TipoEtiqueta = TipoEtiqueta,
                                //OTP = oFila.OTP.ToString(),
                                //OTD = oFila.OTD.ToString(),
                                //CantidadBultos = oFila.CantidadBultos.ToString(),
                                //Cliente = oFila.Cliente.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U').Replace('ñ', 'n').Replace('Ñ', 'N'),
                                //Comuna = oFila.Comuna.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U').Replace('ñ', 'n').Replace('Ñ', 'N'),
                                //Localidad = oFila.Localidad,
                                //Direccion = oFila.Direccion.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U').Replace('ñ', 'n').Replace('Ñ', 'N'),
                                //idBulto = oElemento.idBulto,
                                //NumeroBulto = oElemento.NumeroBulto.ToString(),
                                //Referencia1 = oFila.Referencia1,
                                //Referencia2 = oFila.Referencia2,
                                //Servicio = oFila.Servicio.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U').Replace('ñ', 'n').Replace('Ñ', 'N'),
                                //SucursalDestino = oFila.SucursalDestino.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U').Replace('ñ', 'n').Replace('Ñ', 'N'),
                                //Telefono = oFila.Telefono,
                                //Via = oFila.Via.Replace('í', 'i').Replace('é', 'e')

                            
                
            }
        }
        void oServicio_ImpresionEtiquetaPorOTPCompleted(object sender, ImpresionEtiquetaPorOTPCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                List<TipoEtiquetaImpresion> oLista = new List<TipoEtiquetaImpresion>();
                foreach (var oFila in e.Result)
                {
                    int TipoEtiqueta = 1;
                    foreach (var oElemento in oFila.ListaDetalle)
                    {
                        if (TipoEtiqueta == 1)
                        {
                            oLista.Add(new TipoEtiquetaImpresion
                            {
                                TipoEtiqueta = TipoEtiqueta,
                                OTP = oFila.OTP.ToString(),
                                OTD = oFila.OTD.ToString(),
                                CantidadBultos = oFila.CantidadBultos.ToString(),
                                Cliente = oFila.Cliente.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U').Replace('ñ', 'n').Replace('Ñ', 'N'),
                                Comuna = oFila.Comuna.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U').Replace('ñ', 'n').Replace('Ñ', 'N'),
                                Localidad = oFila.Localidad,
                                Direccion = oFila.Direccion.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U').Replace('ñ', 'n').Replace('Ñ', 'N'),
                                idBulto = oElemento.idBulto,
                                NumeroBulto = oElemento.NumeroBulto.ToString(),
                                Referencia1 = oFila.Referencia1,
                                Referencia2 = oFila.Referencia2,
                                Servicio = oFila.Servicio.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U').Replace('ñ', 'n').Replace('Ñ', 'N'),
                                SucursalDestino = oFila.SucursalDestino.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U').Replace('ñ', 'n').Replace('Ñ', 'N'),
                                Telefono = oFila.Telefono,
                                Via = oFila.Via.Replace('í', 'i').Replace('é', 'e')     
                          
                            });
                            TipoEtiqueta = 2;
                        }
                        else
                        {
                            oLista.Add(new TipoEtiquetaImpresion
                            {
                                TipoEtiqueta = TipoEtiqueta,
                                OTP = oFila.OTP.ToString(),
                                OTD = oFila.OTD.ToString(),
                                Localidad = oFila.Localidad.ToString(),
                                CantidadBultos = oFila.CantidadBultos.ToString(),
                                Cliente = oFila.Cliente.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U').Replace('ñ', 'n').Replace('Ñ', 'N'),
                                Comuna = oFila.Comuna.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U').Replace('ñ', 'n').Replace('Ñ', 'N'),
                                Direccion = oFila.Direccion.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U').Replace('ñ', 'n').Replace('Ñ', 'N'),
                                idBulto = oElemento.idBulto,
                                NumeroBulto = oElemento.NumeroBulto.ToString(),
                                Referencia1 = oFila.Referencia1,
                                Referencia2 = oFila.Referencia2,
                                Servicio = oFila.Servicio.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U').Replace('ñ', 'n').Replace('Ñ', 'N'),
                                SucursalDestino = oFila.SucursalDestino.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U').Replace('ñ', 'n').Replace('Ñ', 'N'),
                                Telefono = oFila.Telefono,
                                Via = oFila.Via.Replace('í', 'i').Replace('é', 'e')                               
                            });
                        }
                    }
                    int oCont = 1;
                    while (oCont <= 4)
                    {
                        oLista.Add(new TipoEtiquetaImpresion
                        {
                            TipoEtiqueta = 3,
                            OTD = oFila.OTD.ToString(),
                            OTP = oFila.OTP.ToString(),
                            Referencia1 = oFila.Referencia1
                        });
                        oCont++;
                    }
                }
                ImpresionEtiqueta(oLista);
            }
        }

        void oServicio_ImpresionEtiquetaCompleted(object sender, ImpresionEtiquetaCompletedEventArgs e)
        {
            
            if (e.Error==null)
            {
                List<TipoEtiquetaImpresion> oLista = new List<TipoEtiquetaImpresion>();                
                //TipoEtiqueta 1:Grande 2:Normal 3:Guia
                if (useroOpcion == "1" || useroOpcion == "5")//Opcion de imprimir todo o 5 Elemento
                {
                    int TipoEtiqueta = 1;
                    foreach (var oFila in e.Result.ListaDetalle)
                    {
                        if (useroOpcion == "5")
                        {
                            if (userIdBulto == oFila.idBulto.ToString())
                            {
                                oLista.Add(new TipoEtiquetaImpresion
                                {
                                    TipoEtiqueta = TipoEtiqueta,
                                    OTP = e.Result.OTP.ToString(),
                                    OTD = e.Result.OTD.ToString(),
                                    CantidadBultos = e.Result.CantidadBultos.ToString(),
                                    Cliente = e.Result.Cliente.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U').Replace('ñ', 'n').Replace('Ñ', 'N'),
                                    Comuna = e.Result.Comuna.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U').Replace('ñ', 'n').Replace('Ñ', 'N'),
                                    Localidad=e.Result.Localidad,
                                    Direccion = e.Result.Direccion.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U').Replace('ñ', 'n').Replace('Ñ', 'N'),
                                    idBulto = oFila.idBulto,
                                    NumeroBulto = oFila.NumeroBulto.ToString(),
                                    Referencia1 = e.Result.Referencia1,
                                    Referencia2 = e.Result.Referencia2,
                                    Servicio = e.Result.Servicio.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U').Replace('ñ', 'n').Replace('Ñ', 'N'),
                                    SucursalDestino = e.Result.SucursalDestino.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U').Replace('ñ', 'n').Replace('Ñ', 'N'),
                                    Telefono = e.Result.Telefono,
                                    Via = e.Result.Via.Replace('í', 'i').Replace('é', 'e'),
                                    Peso= oFila.Peso.ToString()
                                });
                            }
                        }
                        else
                        {
                            oLista.Add(new TipoEtiquetaImpresion
                            {
                                TipoEtiqueta = TipoEtiqueta,
                                OTP = e.Result.OTP.ToString(),
                                OTD = e.Result.OTD.ToString(),
                                Localidad=e.Result.Localidad.ToString(),
                                CantidadBultos = e.Result.CantidadBultos.ToString(),
                                Cliente = e.Result.Cliente.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U').Replace('ñ', 'n').Replace('Ñ', 'N'),
                                Comuna = e.Result.Comuna.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U').Replace('ñ', 'n').Replace('Ñ', 'N'),
                                Direccion = e.Result.Direccion.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U').Replace('ñ', 'n').Replace('Ñ', 'N'),
                                idBulto = oFila.idBulto,
                                NumeroBulto = oFila.NumeroBulto.ToString(),
                                Referencia1 = e.Result.Referencia1,
                                Referencia2 = e.Result.Referencia2,
                                Servicio = e.Result.Servicio.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U').Replace('ñ', 'n').Replace('Ñ', 'N'),
                                SucursalDestino = e.Result.SucursalDestino.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u').Replace('Á', 'A').Replace('É', 'E').Replace('Í', 'I').Replace('Ó', 'O').Replace('Ú', 'U').Replace('ñ', 'n').Replace('Ñ', 'N'),
                                Telefono = e.Result.Telefono,
                                Via = e.Result.Via.Replace('í', 'i').Replace('é', 'e'),
                                Peso=oFila.Peso.ToString()
                            });
                        }
                        TipoEtiqueta = 2;
                    }
                }
                if (useroOpcion != "5") //La opción 5 es para imprimir 1 solo bulto
                {
                    int oCont = 1;
                    while (oCont <= 4)
                    {
                        oLista.Add(new TipoEtiquetaImpresion
                        {
                            TipoEtiqueta = 3,
                            OTD = e.Result.OTD.ToString(),
                            OTP = e.Result.OTP.ToString(),
                            Referencia1 = e.Result.Referencia1
                        });
                        oCont++;
                    }
                }

                ImpresionEtiqueta(oLista);
                //MessageBox.Show("Impresión Ok");                
            }
            else
            {
                //MessageBox.Show("No fue posible obtener la información para imprimir " + e.Error);
            }
        }


        public class TipoEtiquetaImpresion
        {                        
            public int TipoEtiqueta{get;set;}//1:Grande, 2:Normal , 3:Guía

            public string OTP { get; set; }
            public string OTD { get; set; }
            public string CantidadBultos { get; set; }
            public string SucursalDestino { get; set; }
            public string Comuna { get; set; }
            public string Localidad { get; set; }
            public string Via { get; set; }            
            public string Cliente { get; set; }
            public string Direccion { get; set; }
            public string Telefono { get; set; }
            public string Servicio { get; set; }
            public string Referencia1 { get; set; }
            public string Referencia2 { get; set; }
            public decimal idBulto { get; set; }
            public string NumeroBulto { get; set; }
            public string Peso { get; set; }
            public string Pallet { get; set; }
        }

        public string EtiquetaGuia(int Posicion, TipoEtiquetaImpresion oDatos)
        {
            //MD5 el N° 5 representa la opacidad de la imagen

            //string oComando = "^MD5";
            string oComando = "";
            oComando += "^FO" + (10 + Posicion).ToString() + ",50^BY2^BCB,100,Y,N,N";
            oComando += "^FDP" + oDatos.OTP + "D" + oDatos.OTD + "^FS";
            oComando += "^FPH^FO" + (10 + Posicion).ToString() + ",515^A0N,25,25^FDOT^FS";
            oComando += "^FPH^FO" + (10 + Posicion).ToString() + ",540^A0N,25,25^FD" + oDatos.OTP + "-" + oDatos.OTD + "^FS";
            oComando += "^FT" + (180 + Posicion).ToString() + ",400^A0B,30,30^FDGUIA: "+ oDatos.Referencia1 + "^FS";            
            return oComando;
        }

        public string EtiquetaNormal(int Posicion,TipoEtiquetaImpresion oDatos)
        {
            string oComando = "^FO" + (10 + Posicion).ToString() + ",50^BY2^BCB,100,Y,N,N";
            oComando += "^FDB" + oDatos.idBulto + "^FS";
            oComando += "^FT" + (152 + Posicion).ToString() + ",540^A0B,20,20^FDSucursal: " + oDatos.SucursalDestino + "^FS";
            oComando += "^FT" + (152 + Posicion).ToString() + ",300^A0B,20,20^FDOT: " + oDatos.OTP + "-" + oDatos.OTD + "^FS";
            oComando += "^FT" + (152 + Posicion).ToString() + ",150^A0B,20,20^FDPESO: " + oDatos.Peso + "^FS";
            oComando += "^FT" + (175 + Posicion).ToString() + ",540^A0B,20,20^FDBulto " + oDatos.NumeroBulto + " de " + oDatos.CantidadBultos + "  Comuna Destino: " + oDatos.Comuna + "^FS";
            return oComando;
        }

        public void ImpresionEtiqueta(List<TipoEtiquetaImpresion> oListaImpresion)
        {
     

            string oComando="";
            
            //var service = new PrinterService();
            //PrinterList.ItemsSource = service.Printers;

            //service.LoadPrinters(PrinterEnumFlags.Local);           
            
            int[] oEtiquetas = new int[4];

            oEtiquetas[0] = 0;
            oEtiquetas[1] = 199;
            oEtiquetas[2] = 394;
            oEtiquetas[3] = 593;
            
            int oPosicion = 0;

            string oInicio = "^XA^PW780";
            string oFin = "^XZ";            
            string oConcatenaComando = "";
            int oBucle = 1;

            

            foreach(var oFila in oListaImpresion)
            {
                if (oFila.TipoEtiqueta == 1)
                {
                    //Control Largo Cliente (2 filas)
                    string Cliente1 = string.Empty, Cliente2=string.Empty;
                    if (oFila.Cliente.Length > 27)
                    {
                        if (oFila.Cliente.Length > 96)
                        {
                            Cliente1 = oFila.Cliente.Substring(0, 48);
                            Cliente2 = oFila.Cliente.Substring(48, 48) + "...";
                        }
                        else if (oFila.Cliente.Length <= 48)
                        {
                            Cliente1 = oFila.Cliente;
                            Cliente2 = ".";
                        }
                        else
                        {
                            Cliente1 = oFila.Cliente.Substring(0, 48);
                            Cliente2 = oFila.Cliente.Substring(48, oFila.Cliente.Length - 48);
                        }
                    }
                    else
                    {
                        Cliente1 = oFila.Cliente;
                    }

                    //Control Largo Dirección (2 filas)
                    string Direccion1 = string.Empty, Direccion2 = string.Empty;
                    if (oFila.Direccion.Length > 27)
                    {                        
                        if (oFila.Direccion.Length > 96)
                        {
                            Direccion1 = oFila.Direccion.Substring(0, 48);
                            Direccion2 = oFila.Direccion.Substring(48, 48) + "...";
                        }
                        else if (oFila.Direccion.Length <= 48)
                        {
                            Direccion1 = oFila.Direccion;
                            Direccion2 = ".";
                        }
                        else
                        {
                            Direccion1 = oFila.Direccion.Substring(0, 48);
                            Direccion2 = oFila.Direccion.Substring(48, oFila.Direccion.Length - 48);
                        }
                    }
                    else
                    {
                        Direccion1 = oFila.Direccion;
                    }



                    oComando = "^FO5,5^GB775,540,3^FS";
                    oComando += "^FO5,180^GB775,0,3^FS";
                    oComando += "^FO5,360^GB593,0,3^FS";
                    oComando += "^FO593,180^GB0,365,3^FS";
                    oComando += "^FPH^FO15,370^A0N,25,25^FDREFERENCIAS:^FS";
                    oComando += "^FPH^FO365,370^A0N,30,30^FDOT:" + oFila.OTP + "-" + oFila.OTD + "^FS";
                    oComando += "^FPH^FO15,410^A0N,30,20^FDReferencia 1^FS";
                    oComando += "^FPH^FO15,450^A0N,30,20^FDReferencia 2^FS";
                    oComando += "^FPH^FO125,410^A0N,30,20^FD: "+ oFila.Referencia1+ "^FS";
                    oComando += "^FPH^FO125,450^A0N,30,20^FD: " + oFila.Referencia2 + "^FS";
                    oComando += "^FPH^FO15,510^A0N,20,20^FDBulto " + oFila.NumeroBulto + " de " + oFila.CantidadBultos + "^FS";
                    oComando += "^FPH^FO15,195^A0N,20,20^FDCliente^FS";
                    oComando += "^FPH^FO15,235^A0N,20,20^FDDireccion^FS";
                    


                    //Cliente
                    if (Cliente2 == string.Empty)
                    {
                        oComando += "^FPH^FO105,190^A0N,30,30^FD: " + Cliente1 + "^FS";
                    }
                    else
                    {
                        oComando += "^FPH^FO105,190^A0N,20,20^FD: " + Cliente1 + "^FS";
                        oComando += "^FPH^FO105,210^A0N,20,20^FD  " + Cliente2 + "^FS";
                    }


                    //Dirección
                    if (Direccion2 == string.Empty)
                    {
                        oComando += "^FPH^FO105,235^A0N,30,30^FD: " + Direccion1 + "^FS";
                    }
                    else
                    {
                        oComando += "^FPH^FO105,240^A0N,20,20^FD: " + Direccion1 + "^FS";
                        oComando += "^FPH^FO105,260^A0N,20,20^FD  " + Direccion2 + "^FS";
                    }

                    //Telefono
                    oComando += "^FPH^FO15,290^A0N,20,20^FDTelefono^FS";                    
                    oComando += "^FPH^FO105,285^A0N,30,30^FD: "+ oFila.Telefono+"^FS";

                    //Servicio
                    oComando += "^FPH^FO15,325^A0N,20,20^FDServicio^FS";
                    oComando += "^FPH^FO105,320^A0N,30,30^FD: "+ oFila.Servicio +"^FS";


                    oComando += "^FPH^FO455,30^A0N,20,20^FDFecha de Emision: " + DateTime.Now.ToString("dd-MM-yyyy") + "^FS";                    
                   
                    oComando += "^FPH^FO15,150^A0N,20,20^FDCant. Bultos^FS";
                    oComando += "^FPH^FO130,145^A0N,30,30^FD: " + oFila.CantidadBultos + "^FS";

                    oComando += "^FPH^FO280,150^A0N,20,20^FDVia:^FS";
                    oComando += "^FPH^FO320,145^A0N,30,30^FD" + oFila.Via + "^FS";
                   

                    oComando += "^FPH^FO480,150^A0N,20,20^FDPeso: " + oFila.Peso + "^FS";

                    oComando += "^FPH^FO15,30^A0N,20,20^FDSuc. Destino^FS";                    
                    oComando += "^FPH^FO15,70^A0N,20,20^FDComuna^FS";                    
                    oComando += "^FPH^FO15,110^A0N,20,20^FDLocalidad^FS";

                    oComando += "^FPH^FO130,20^A0N,40,40^FD: "+oFila.SucursalDestino+"^FS";
                    oComando += "^FPH^FO130,60^A0N,40,40^FD: "+oFila.Comuna+"^FS";

                    if (oFila.Localidad != string.Empty)
                    {
                        oComando += "^FPH^FO130,100^A0N,40,40^FD: " + oFila.Localidad + "^FS";
                    }
                    else
                    {
                        oComando += "^FPH^FO130,100^A0N,40,40^FD: -^FS";
                    }

                       oComando += "^FO655,225^BY2^BCR,100,Y,N,N";
                       oComando += "^FDB" + oFila.idBulto.ToString() + "^FS";
                   
                    oBucle = 0;
                    
                    oConcatenaComando = oInicio + oComando + oFin;
                    RawPrinter.SendToPrinter("FolCourierET1", oConcatenaComando, userNombreImpresora);
                    oConcatenaComando = "";
                    oComando = "";
                }
                else if (oFila.TipoEtiqueta==2)
                {
                    oComando = EtiquetaNormal(oEtiquetas[oPosicion], oFila);
                    oPosicion++;
                }
                else if (oFila.TipoEtiqueta == 3)
                {
                    oComando = EtiquetaGuia(oEtiquetas[oPosicion], oFila);
                    oPosicion++;
                }
                oConcatenaComando += oComando;

                if (oBucle == 4)
                {
                    oBucle = 0;
                    oConcatenaComando = oInicio + oConcatenaComando + oFin;
                    RawPrinter.SendToPrinter("FolCourierET4", oConcatenaComando, userNombreImpresora);
                    oConcatenaComando = "";
                }
                
               
                if (oPosicion == 4)
                    oPosicion = 0;
                oBucle++;
            }
            if (oBucle>1)
            {
                oConcatenaComando = oInicio + oConcatenaComando + oFin;
                RawPrinter.SendToPrinter("FolCourierETG", oConcatenaComando, userNombreImpresora);
            }

        }




        public void ImpresionEtiquetaPallet(List<TipoEtiquetaImpresion> oListaImpresion)
        {


            string oComando = "";

            //var service = new PrinterService();
            //PrinterList.ItemsSource = service.Printers;

            //service.LoadPrinters(PrinterEnumFlags.Local);           

            int[] oEtiquetas = new int[4];

            oEtiquetas[0] = 0;
            oEtiquetas[1] = 199;
            oEtiquetas[2] = 394;
            oEtiquetas[3] = 593;

            int oPosicion = 0;

            string oInicio = "^XA^PW780";
            string oFin = "^XZ";
            string oConcatenaComando = "";
            int oBucle = 1;



            foreach (var oFila in oListaImpresion)
            {
                //if (oFila.TipoEtiqueta == 1)
                //{
                    //Control Largo Cliente (2 filas)
                    string Cliente1 = string.Empty, Cliente2 = string.Empty;
                    if (oFila.Cliente.Length > 27)
                    {
                        if (oFila.Cliente.Length > 96)
                        {
                            Cliente1 = oFila.Cliente.Substring(0, 48);
                            Cliente2 = oFila.Cliente.Substring(48, 48) + "...";
                        }
                        else if (oFila.Cliente.Length <= 48)
                        {
                            Cliente1 = oFila.Cliente;
                            Cliente2 = ".";
                        }
                        else
                        {
                            Cliente1 = oFila.Cliente.Substring(0, 48);
                            Cliente2 = oFila.Cliente.Substring(48, oFila.Cliente.Length - 48);
                        }
                    }
                    else
                    {
                        Cliente1 = oFila.Cliente;
                    }

                    //Control Largo Dirección (2 filas)
                    string Direccion1 = string.Empty, Direccion2 = string.Empty;
                    if (oFila.Direccion.Length > 27)
                    {
                        if (oFila.Direccion.Length > 96)
                        {
                            Direccion1 = oFila.Direccion.Substring(0, 48);
                            Direccion2 = oFila.Direccion.Substring(48, 48) + "...";
                        }
                        else if (oFila.Direccion.Length <= 48)
                        {
                            Direccion1 = oFila.Direccion;
                            Direccion2 = ".";
                        }
                        else
                        {
                            Direccion1 = oFila.Direccion.Substring(0, 48);
                            Direccion2 = oFila.Direccion.Substring(48, oFila.Direccion.Length - 48);
                        }
                    }
                    else
                    {
                        Direccion1 = oFila.Direccion;
                    }



                    oComando = "^FO5,5^GB775,540,3^FS";
                    oComando += "^FO5,180^GB775,0,3^FS";
                    oComando += "^FO5,360^GB593,0,3^FS";
                    oComando += "^FO593,180^GB0,365,3^FS";
                    oComando += "^FPH^FO15,370^A0N,25,25^FDREFERENCIAS:^FS";
                    oComando += "^FPH^FO365,370^A0N,30,30^FDOT:" + oFila.OTP + "-" + oFila.OTD + "^FS";
                    oComando += "^FPH^FO15,410^A0N,30,20^FDReferencia 1^FS";
                    oComando += "^FPH^FO15,450^A0N,30,20^FDReferencia 2^FS";
                    oComando += "^FPH^FO125,410^A0N,30,20^FD: " + oFila.Referencia1 + "^FS";
                    oComando += "^FPH^FO125,450^A0N,30,20^FD: " + oFila.Referencia2 + "^FS";
                    oComando += "^FPH^FO15,195^A0N,20,20^FDCliente^FS";
                    oComando += "^FPH^FO15,235^A0N,20,20^FDDireccion^FS";



                    //Cliente
                    if (Cliente2 == string.Empty)
                    {
                        oComando += "^FPH^FO105,190^A0N,30,30^FD: " + Cliente1 + "^FS";
                    }
                    else
                    {
                        oComando += "^FPH^FO105,190^A0N,20,20^FD: " + Cliente1 + "^FS";
                        oComando += "^FPH^FO105,210^A0N,20,20^FD  " + Cliente2 + "^FS";
                    }


                    //Dirección
                    if (Direccion2 == string.Empty)
                    {
                        oComando += "^FPH^FO105,235^A0N,30,30^FD: " + Direccion1 + "^FS";
                    }
                    else
                    {
                        oComando += "^FPH^FO105,240^A0N,20,20^FD: " + Direccion1 + "^FS";
                        oComando += "^FPH^FO105,260^A0N,20,20^FD  " + Direccion2 + "^FS";
                    }

                    //Telefono
                    oComando += "^FPH^FO15,290^A0N,20,20^FDTelefono^FS";
                    oComando += "^FPH^FO105,285^A0N,30,30^FD: " + oFila.Telefono + "^FS";

                    //Servicio
                    oComando += "^FPH^FO15,325^A0N,20,20^FDServicio^FS";
                    oComando += "^FPH^FO105,320^A0N,30,30^FD: " + oFila.Servicio + "^FS";


                    oComando += "^FPH^FO455,30^A0N,20,20^FDFecha de Emision: " + DateTime.Now.ToString("dd-MM-yyyy") + "^FS";

                    oComando += "^FPH^FO15,150^A0N,20,20^FDCant. Bultos:^FS";
                    oComando += "^FPH^FO140,138^A0N,40,40^FD" + oFila.CantidadBultos + "^FS";

                    oComando += "^FPH^FO280,150^A0N,20,20^FDPallet:^FS";
                    oComando += "^FPH^FO360,138^A0N,40,40^FD" + oFila.Pallet + "^FS";
                    

                    //oComando += "^FPH^FO480,150^A0N,20,20^FDPeso: " + oFila.Peso + "^FS";

                    //oComando += "^FPH^FO15,30^A0N,20,20^FDSuc. Destino^FS";
                    oComando += "^FPH^FO15,70^A0N,20,20^FDComuna^FS";
                    //oComando += "^FPH^FO15,110^A0N,20,20^FDLocalidad^FS";

                    //oComando += "^FPH^FO130,20^A0N,40,40^FD: " + oFila.SucursalDestino + "^FS";
                    oComando += "^FPH^FO130,60^A0N,40,40^FD: " + oFila.Comuna + "^FS";

                    //if (oFila.Localidad != string.Empty)
                    //{
                    //    oComando += "^FPH^FO130,100^A0N,40,40^FD: " + oFila.Localidad + "^FS";
                    //}
                    //else
                    //{
                    //    oComando += "^FPH^FO130,100^A0N,40,40^FD: -^FS";
                    //}

                   
                    oBucle = 0;

                    oConcatenaComando = oInicio + oComando + oFin;
                    RawPrinter.SendToPrinter("FolCourierET1", oConcatenaComando, userNombreImpresora);
                    oConcatenaComando = "";
                    oComando = "";
                }
                //else if (oFila.TipoEtiqueta == 2)
                //{
                //    oComando = EtiquetaNormal(oEtiquetas[oPosicion], oFila);
                //    oPosicion++;
                //}
                //else if (oFila.TipoEtiqueta == 3)
                //{
                //    oComando = EtiquetaGuia(oEtiquetas[oPosicion], oFila);
                //    oPosicion++;
                //}
                oConcatenaComando += oComando;

                //if (oBucle == 4)
                //{
                //    oBucle = 0;
                //    oConcatenaComando = oInicio + oConcatenaComando + oFin;
                //    RawPrinter.SendToPrinter("FolCourierET4", oConcatenaComando, userNombreImpresora);
                //    oConcatenaComando = "";
                //}


                if (oPosicion == 4)
                    oPosicion = 0;
                oBucle++;
            //}
            //if (oBucle > 1)
            //{
            //    oConcatenaComando = oInicio + oConcatenaComando + oFin;
            //    RawPrinter.SendToPrinter("FolCourierETG", oConcatenaComando, userNombreImpresora);
            //}

        }





        private void button4_Click(object sender, RoutedEventArgs e)
        {        
            //List<string>oTexto = new List<string>();
            //oTexto.Add("^XA");
            //oTexto.Add("^PW468");
            //oTexto.Add("^FO0,0");
            //oTexto.Add("^GB468,233,5^FS");            
            //oTexto.Add("^XZ");

            //string prueba = "";
            //foreach (var fila in oTexto)
            //    prueba += fila;

            //RawPrinter.SendToPrinter("Prueba", prueba, "ZDesigner GK420t");
            
        }

        #region Clase Impresion
        [StructLayout(LayoutKind.Sequential)]
        public struct DOCINFO
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string printerDocumentName;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string printerDocumentDataType;
        }

        public class RawPrinter
        {
            [
                DllImport("winspool.drv", CharSet = CharSet.Unicode, ExactSpelling = false,
                    CallingConvention = CallingConvention.StdCall)]
            public static extern long OpenPrinter(string pPrinterName, ref IntPtr phPrinter, int pDefault);

            [
                DllImport("winspool.drv", CharSet = CharSet.Unicode, ExactSpelling = false,
                    CallingConvention = CallingConvention.StdCall)]
            public static extern long StartDocPrinter(IntPtr hPrinter, int Level, ref DOCINFO pDocInfo);

            [
                DllImport("winspool.drv", CharSet = CharSet.Unicode, ExactSpelling = true,
                    CallingConvention = CallingConvention.StdCall)]
            public static extern long StartPagePrinter(IntPtr hPrinter);

            [
                DllImport("winspool.drv", CharSet = CharSet.Ansi, ExactSpelling = true,
                    CallingConvention = CallingConvention.StdCall)]
            public static extern long WritePrinter(IntPtr hPrinter, string data, int buf, ref int pcWritten);

            [
                DllImport("winspool.drv", CharSet = CharSet.Unicode, ExactSpelling = true,
                    CallingConvention = CallingConvention.StdCall)]
            public static extern long EndPagePrinter(IntPtr hPrinter);

            [
                DllImport("winspool.drv", CharSet = CharSet.Unicode, ExactSpelling = true,
                    CallingConvention = CallingConvention.StdCall)]
            public static extern long EndDocPrinter(IntPtr hPrinter);

            [
                DllImport("winspool.drv", CharSet = CharSet.Unicode, ExactSpelling = true,
                    CallingConvention = CallingConvention.StdCall)]
            public static extern long ClosePrinter(IntPtr hPrinter);

            public static void SendToPrinter(string printerJobName, string rawStringToSendToThePrinter,
                                             string printerNameAsDescribedByPrintManager)
            {
                IntPtr handleForTheOpenPrinter = new IntPtr();
                DOCINFO documentInformation = new DOCINFO();
                int printerBytesWritten = 0;
                documentInformation.printerDocumentName = printerJobName;
                documentInformation.printerDocumentDataType = "RAW";
                OpenPrinter(printerNameAsDescribedByPrintManager, ref handleForTheOpenPrinter, 0);
                StartDocPrinter(handleForTheOpenPrinter, 1, ref documentInformation);
                StartPagePrinter(handleForTheOpenPrinter);
                WritePrinter(handleForTheOpenPrinter, rawStringToSendToThePrinter, rawStringToSendToThePrinter.Length,
                             ref printerBytesWritten);
                EndPagePrinter(handleForTheOpenPrinter);
                EndDocPrinter(handleForTheOpenPrinter);
                ClosePrinter(handleForTheOpenPrinter);
            }
        }
        #endregion
    }
}
