using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using BarcodeLib;
using System.IO;
using System.Drawing.Imaging;

namespace Courier.Controllers
{
    public class BarcodeController : Controller
    {
        //
        // GET: /Barcode/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Imagen128(string Valor)
        {
            BarcodeLib.Barcode barcode = new BarcodeLib.Barcode()
            {
                IncludeLabel = true,                
                Width = 200,
                Height = 80,
                RotateFlipType = RotateFlipType.RotateNoneFlipNone,
                BackColor = Color.White,
                ForeColor = Color.Black,
            };

            Image imgBarCode = barcode.Encode(TYPE.CODE128B, Valor);

            MemoryStream oMemory = new MemoryStream();

            imgBarCode.Save(oMemory, ImageFormat.Png);

            // set HTTP response headers
            Response.ClearHeaders();
            Response.ClearContent();
            Response.Clear();

            Response.AddHeader("Content-Type", "image/png");
            //Response.AddHeader("Cache-Control", "no-cache");
            //Response.AddHeader("Accept-Ranges", "none");
            Response.AddHeader("Content-Disposition", "attachment; filename=Imagen128.png");

            // Enviar el archivo generado XLSX
            oMemory.WriteTo(Response.OutputStream);
            oMemory.Close();
            Response.Flush();
            Response.End();

            return PartialView();
        }
        public ActionResult Imagen128v3(string Valor)
        {
            BarcodeLib.Barcode barcode = new BarcodeLib.Barcode()
            {
                IncludeLabel = true,
                Width = 380,
                Height = 80,
                RotateFlipType = RotateFlipType.RotateNoneFlipNone,
                BackColor = Color.White,
                ForeColor = Color.Black,
            };

            Image imgBarCode = barcode.Encode(TYPE.CODE128B, Valor);

            MemoryStream oMemory = new MemoryStream();

            imgBarCode.Save(oMemory, ImageFormat.Png);

            // set HTTP response headers
            Response.ClearHeaders();
            Response.ClearContent();
            Response.Clear();

            Response.AddHeader("Content-Type", "image/png");
            //Response.AddHeader("Cache-Control", "no-cache");
            //Response.AddHeader("Accept-Ranges", "none");
            Response.AddHeader("Content-Disposition", "attachment; filename=Imagen128.png");

            // Enviar el archivo generado XLSX
            oMemory.WriteTo(Response.OutputStream);
            oMemory.Close();
            Response.Flush();
            Response.End();

            return PartialView();
        }
        public ActionResult Imagen128v2(string Valor)
        {
            BarcodeLib.Barcode barcode = new BarcodeLib.Barcode()
            {
                IncludeLabel = false,
                Width = 380,
                Height = 80,
                RotateFlipType = RotateFlipType.RotateNoneFlipNone,
                BackColor = Color.White,
                ForeColor = Color.Black,
            };

            Image imgBarCode = barcode.Encode(TYPE.CODE128B, Valor);

            MemoryStream oMemory = new MemoryStream();

            imgBarCode.Save(oMemory, ImageFormat.Png);

            // set HTTP response headers
            Response.ClearHeaders();
            Response.ClearContent();
            Response.Clear();

            Response.AddHeader("Content-Type", "image/png");
            //Response.AddHeader("Cache-Control", "no-cache");
            //Response.AddHeader("Accept-Ranges", "none");
            Response.AddHeader("Content-Disposition", "attachment; filename=Imagen128.png");

            // Enviar el archivo generado XLSX
            oMemory.WriteTo(Response.OutputStream);
            oMemory.Close();
            Response.Flush();
            Response.End();

            return PartialView();
        }
    }
}
