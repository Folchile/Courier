using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using BarcodeLib;
using Courier.Models;



namespace Courier.Controllers
{
    public class DashboardController : Controller
    {
        //
        // GET: /Dashboard/

        [Authorize]
        public ActionResult Index()
        {
            DashBoardModels oModel = new DashBoardModels();
            ValidationController oValidation = new ValidationController();            
            int oRut = oValidation.GetRutActiveUser();
            oModel.GetNombreImpresora(oRut);            
            return View(oModel);
        }

        public ActionResult Imagen()
        {
            return View();
        }

        public ActionResult somepage()
        {
            return PartialView("somepage");
        }

        public ActionResult pngimage()
        {

            BarcodeLib.Barcode barcode = new BarcodeLib.Barcode()
            {
                IncludeLabel = true,
                Alignment = AlignmentPositions.CENTER,
                Width = 180,
                Height = 80,
                RotateFlipType = RotateFlipType.RotateNoneFlipNone,
                BackColor = Color.White,
                ForeColor = Color.Black,
            };

            Image imgBarCode = barcode.Encode(TYPE.CODE128B, "11223442234");

            Pen greenPen = new Pen(Color.Black,1);
            Rectangle recRectangle = new Rectangle(82,0,200,99);
            
            Bitmap oBitMap = new Bitmap(283, 100);

            Bitmap oVertical = new Bitmap(100, 201);

            Graphics oGraphics = Graphics.FromImage(oBitMap);
            oGraphics.DrawRectangle(greenPen, recRectangle);

            Graphics oGraphicBar = Graphics.FromImage(imgBarCode);

            oGraphicBar.TranslateTransform((float)imgBarCode.Width / 2, (float)imgBarCode.Height / 2);
            oGraphicBar.RotateTransform(270);
            oGraphicBar.TranslateTransform((float)imgBarCode.Width / 2, (float)imgBarCode.Height / 2);
            oGraphicBar.DrawImage(imgBarCode, new Point(0, 0));

            oGraphics.DrawImage(imgBarCode,new Point(93,10));

            //string oPath= Path.GetFullPath("../Images/idisk_globe.png");

            //Bitmap oImage = new Bitmap("C:\\Francisco\\Desarrollos\\Courier\\Fuentes\\Courier\\Courier\\Images\\idisk_globe.png");

            ////string inputImage = "C:\\Francisco\\Desarrollos\\Courier\\Fuentes\\Courier\\Courier\\Images\\idisk_globe.png";
            ////string outputImageFilePath = @"C:\OutPut.jpg";
            //string textToDisplayOnImage = "Prueba de imagen";

            //Bitmap oOriginal = new Bitmap(img);
            //Bitmap oImageIz = new Bitmap(img);

            //Bitmap imageInBitMap = new Bitmap(oOriginal);
            //Bitmap oBitMap = new Bitmap(203,203);


            //Graphics imageGraphics = Graphics.FromImage(oBitMap);

            
            //StringFormat formatAssignment = new StringFormat();
            //formatAssignment.Alignment = StringAlignment.Near;

            
            //Color assignColorToString = System.Drawing.ColorTranslator.FromHtml("#000000");

                    
            
            //imageGraphics.DrawString(textToDisplayOnImage, new Font("Arial", 11, FontStyle.Regular), new SolidBrush(assignColorToString), new Point(1, 1), formatAssignment);

            //imageGraphics.TranslateTransform((float)imageInBitMap.Width / 2, (float)imageInBitMap.Height / 2);
            //imageGraphics.RotateTransform(270);
            //imageGraphics.TranslateTransform(-(float)imageInBitMap.Width / 2, -(float)imageInBitMap.Height / 2);
            
            //imageGraphics.DrawImage(imageInBitMap, new Point(0, 0));

            //imageGraphics.DrawImage(oImage, new Point(0, 0));

            ////imageGraphics = Graphics.FromImage(oImageIz);
            ////imageGraphics.DrawImage(imageInBitMap, new Point(0, 0));
            ////saving in the computer
            ////imageInBitMap.Save(outputImageFilePath);

            

            MemoryStream oStream= new MemoryStream();

            imgBarCode.Save(oStream, ImageFormat.Png);

            return View(oStream);
        }
        public ActionResult pngimage2()
        {

            BarcodeLib.Barcode barcode = new BarcodeLib.Barcode()
            {
                IncludeLabel = true,
                Alignment = AlignmentPositions.CENTER,
                Width = 180,
                Height = 80,
                RotateFlipType = RotateFlipType.RotateNoneFlipNone,
                BackColor = Color.White,
                ForeColor = Color.Black,
            };

            Image imgBarCode = barcode.Encode(TYPE.CODE128B, "11223442234");


            Bitmap imgBarCode2 = new Bitmap(80, 180);

            Pen greenPen = new Pen(Color.Black, 1);
            Rectangle recRectangle = new Rectangle(82, 0, 200, 99);

            Bitmap oBitMap = new Bitmap(283, 100);

            Bitmap oVertical = new Bitmap(100, 201);

            Graphics oGraphics = Graphics.FromImage(imgBarCode2);

            oGraphics.DrawRectangle(greenPen, recRectangle);

            Graphics oGraphicBar = Graphics.FromImage(imgBarCode);

            oGraphicBar.TranslateTransform((float)imgBarCode.Width / 2, (float)imgBarCode.Height / 2);
            oGraphicBar.RotateTransform(270);
            oGraphicBar.TranslateTransform((float)imgBarCode.Width / 2, (float)imgBarCode.Height / 2);
            oGraphicBar.DrawImage(imgBarCode, new Point(0, 0));

            oGraphics.DrawImage(imgBarCode, new Point(0, 0));

            MemoryStream oStream = new MemoryStream();

            imgBarCode2.Save(oStream, ImageFormat.Png);

            return View("pngimage",oStream);
        }
    }
}
