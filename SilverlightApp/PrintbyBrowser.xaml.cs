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

namespace SilverlightApp
{
    public partial class PrintbyBrowser : UserControl
    {
        public PrintbyBrowser()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            

            PrintDocument oPg = new PrintDocument();            
            PrinterFallbackSettings  oFB= new PrinterFallbackSettings();                                    
            oPg.PrintPage += new EventHandler<PrintPageEventArgs>(oPg_PrintPage);           

          
            oPg.Print("Prueba",oFB,true); 
        }

        void oPg_PrintPage(object sender, PrintPageEventArgs e)
        {            

            var a = e.PrintableArea;

            MessageBox.Show(a.Width.ToString());
            MessageBox.Show(a.Height.ToString());

            e.PageVisual = border1;
                        
                        
        }
    }
}
