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
using SilverlightEnumPrinters;
using System.Data.Services.Client;
using SilverlightApp.ReferenciaServicioWeb;

namespace SilverlightApp
{
    public partial class Printer : UserControl
    {
        int RutUsuarioG;
        public Printer(string Impresora, string Configuracion, int RutUsuario)
        {
            RutUsuarioG = RutUsuario;
                if (!Application.Current.HasElevatedPermissions)
                {
                    MessageBox.Show("La aplicación no esta funcionando! Requiere permisos elevados");
                    return;
                }
                InitializeComponent();

                if (Impresora == "")
                    label2.Content = "";
                else
                {
                    label2.Content = "Su Impresora predeterminada actualmente es : " + Impresora;
                }


                PrinterService oService = new PrinterService();
                oService.ClearPrinter();
                oService.LoadPrinters(PrinterEnumFlags.Connections);
                oService.LoadPrinters(PrinterEnumFlags.Local);
                

                var oDatos = from x in oService.Printers
                                select x.PrinterName;
                
                
                comboBox1.ItemsSource = oDatos;
                
        }    

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox1.SelectedValue.ToString() == "")
            {
                MessageBox.Show("Debe seleccionar una impresora del listado");           
            }
            else
            {
                ServicioSilverlightClient oServicio = new ServicioSilverlightClient();

                oServicio.ModificarImpresoraCompleted += new EventHandler<ModificarImpresoraCompletedEventArgs>(oServicio_ModificarImpresoraCompleted);


                oServicio.ModificarImpresoraAsync(comboBox1.SelectedValue.ToString(), RutUsuarioG);
            }
        }

        void oServicio_ModificarImpresoraCompleted(object sender, ModificarImpresoraCompletedEventArgs e)
        {
            if (e.Result == true)
            {
                MessageBox.Show("Impresora Actualizada");
                label2.Content = "Impresora Actualizada";
            }
            else
            {
                MessageBox.Show("No fue posible guardar la información");
            }
        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

             
    }
}
