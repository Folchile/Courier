using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Courier.Models;
using System.IO;
using System.Text;
using Courier.Controllers;

//NPOI libreria para leer archivos XLS
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;


//La libreria ClosedXML utiliza OpenXML, para Leer Archivos XLSX
using DocumentFormat.OpenXml;
using ClosedXML.Excel;
using System.Net.Mime;



namespace Courier.Controllers
{
    public class PedidoController : Controller
    {
  
        #region Servicios 
        public ActionResult ServiciosEmpresa(string Rut)
        {
            ValidationController oFunciones = new ValidationController();
            var iRut =0;
            if (oFunciones.ValidarRut(Rut) == 1)
            {
                iRut = Convert.ToInt32(Rut.Substring(0, Rut.Length - 2));
            }
            var oDataContext = new LinqBD_PARDataContext();
            var oServicios = from EMS in oDataContext.TB_PAR_EMS_EMPRESA_SERVICIO
                                join SER in oDataContext.TB_PAR_SER_SERVICIO
                                on EMS.PK_PAR_SER_ID equals SER.PK_PAR_SER_ID
                                where EMS.PK_PAR_EST_ID == 1 && SER.PK_PAR_EST_ID == 1 && EMS.PK_CLI_EMP_RUT == iRut
                                select
                                new { EMS.PK_PAR_SER_ID, SER.FL_PAR_SER_NOMBRE };

            return Json(oServicios.ToList(), JsonRequestBehavior.AllowGet);
            
        }

        #endregion

        #region index
        public ActionResult Index()
        {            
            return View();
        }
        #endregion

        #region Upload

        public List<ListaExcel> LeerExcel(Stream oMs, string oType, string Servicio)
        {

            string oExtension = oType.Substring(oType.Length - 3, 3).ToLower();

            List<ListaExcel> oLista = new List<ListaExcel>();

            if (oType.Substring(oType.Length - 3, 3).ToLower() == "xls")
                oType = "xls";
            else if (oType.Substring(oType.Length - 4, 4).ToLower() == "xlsx")
                oType = "xlsx";
            else
            {
                oType = "X";
            }

            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oListaColumnas = from m in oPar.TB_PAR_EXC_EXCEL
                                 where m.PK_PAR_SER_ID == Convert.ToInt32(Servicio)
                                 select m;

            //EN LA BASE DE DATOS LA DEFINICIÓN DE COLUMNAS ES LA SIGUIENTE

            //PK_PAR_COL_ID	FL_PAR_COL_NOMBRE
            //1	Id
            //2	Referencia
            //3	Destinatario
            //4	Rut
            //5	Contacto Entrega
            //6	Dirección
            //7	Comuna
            //8	Bulto
            //9	Peso
            //10 Fono
            //11 N° Dirección
            //12 Localidad
            //13 Vía
            //14 Código Postal
            //15 Largo
            //16 Alto
            //17 ANcho
            //18 Observación 1
            //19 Observación 2
            //20 Contacto 1, Rut
            //21 Contacto 1, Nombre
            //22 Contacto 1, Teléfono
            //23 Contacto 2, Rut
            //24 Contacto 2, Nombre
            //25 Contacto 2, Teléfono
            //26 Contacto 3, Rut
            //27 Contacto 3, Nombre
            //28 Contacto 3, Telefono
            //29 Tipo Documento
            //30 N° Documento
            //31 Peso Volumétrico
            //32 ContraPago


            //SE DEBE TENER EN CUENTA ESTO A MODO DE ENTENDER LA RELACIÓN

            //A=1
            //B=2
            //C=3
            //D=4
            //E=5
            //F=6
            //G=7
            //H=8
            //I=9
            //J=10
            //K=11
            //L=12
            //M=13
            //N=14
            //O=15
            //P=16
            //Q=17
            //R=18
            //S=19
            //T=20
            //U=21
            //V=22
            //W=23
            //X=24
            //Y=25
            //Z=26
            //AA=27
            //AB=28
            //AC=29
            //AD=30
            //AE=31            
            //AF=32

            //SE IDENTIFICA CADA LETRA Y SE LEE QUE COLUMNA LA CONTIENE

            string[] iValores;
            iValores = new string[34];//por si creo mas columnas en el futuro

          

            #region Excel 2003 XLS
            if (oType == "xls")
            {
                HSSFWorkbook oHWb;                
                oHWb = new HSSFWorkbook(oMs);     
          
                ISheet sheet = oHWb.GetSheet(oHWb.GetSheetName(0));
                for (int iRow = 1; iRow <= sheet.LastRowNum; iRow++)
                {
                    iValores[1] = "";//A
                    iValores[2] = "";//B
                    iValores[3] = "";//C
                    iValores[4] = "";//D
                    iValores[5] = "";//E
                    iValores[6] = "";//F
                    iValores[7] = "";//G
                    iValores[8] = "";//H
                    iValores[9] = "";//I
                    iValores[10] = "";//J Fono
                    iValores[11] = "";//K
                    iValores[12] = "";//L
                    iValores[13] = "";//M
                    iValores[14] = "";//N
                    iValores[15] = "";//O
                    iValores[16] = "";//P
                    iValores[17] = "";//Q
                    iValores[18] = "";//R
                    iValores[19] = "";//S
                    iValores[20] = "";//T
                    iValores[21] = "";//U
                    iValores[22] = "";//V
                    iValores[23] = "";//W
                    iValores[24] = "";//X
                    iValores[25] = "";//Y
                    iValores[26] = "";//Z
                    iValores[27] = "";//AA
                    iValores[28] = "";//AB
                    iValores[29] = "";//AC
                    iValores[30] = "";//AD
                    iValores[31] = "";//AE
                    iValores[32] = "";//AF
                    
              


                    foreach (var oFile in oListaColumnas)
                    {                        
                        if (sheet.GetRow(iRow).GetCell(oFile.FL_PAR_EXC_LETRA - 1) != null)
                            iValores[oFile.PK_PAR_COL_ID] = sheet.GetRow(iRow).GetCell(oFile.FL_PAR_EXC_LETRA - 1).ToString();
                    }

                    bool oColumnas = false;
                    for (int oInd = 1; oInd <= 32; oInd++)
                    {
                        if (iValores[oInd] != null && iValores[oInd] != "")
                            oColumnas = true;
                    }

                    if (oColumnas == true)
                    {
                        ListaExcel oListaExcel = new ListaExcel
                        {
                            Id = iValores[1],
                            Referencia = iValores[2],
                            Destinatario = iValores[3],
                            Rut = iValores[4],
                            ContactoEntrega = iValores[5],
                            Direccion = iValores[6],
                            Comuna = iValores[7],
                            Bulto = iValores[8],
                            Peso = iValores[9],
                            Telefono = iValores[10],
                            IdComuna = 0,
                            DireccionNumero = iValores[11],
                            Localidad = iValores[12],
                            Via = iValores[13].ToUpper(),
                            CodigoPostal = iValores[14],
                            Largo = iValores[15],
                            Alto = iValores[16],
                            Ancho = iValores[17],
                            Observacion1 = iValores[18],
                            Observacion2 = iValores[19],
                            Contacto1Rut = iValores[20],
                            Contacto1Nombre = iValores[21],                            
                            Contacto1Telefono = iValores[22],
                            Contacto2Rut = iValores[23],
                            Contacto2Nombre = iValores[24],
                            Contacto2Telefono = iValores[25],
                            Contacto3Rut = iValores[26],
                            Contacto3Nombre = iValores[27],
                            Contacto3Telefono = iValores[28],
                            TipoDocumento=iValores[29],
                            NumeroDocumento = iValores[30],
                            PesoVolumetrico = iValores[31],
                            ContraPago = iValores[32]
                        };
                        oLista.Add(oListaExcel);
                    }

                }
                oHWb = null;
                sheet = null;
            }
            #endregion

            #region Excel 2007 XLSX
            else if (oType == "xlsx")
            {                

                var oWorkBook = new XLWorkbook(oMs);
                var oWorkSheet = oWorkBook.Worksheet(1);
                //var oRange = oWorkSheet.Range("A1:E200");



                var oCursorRow = oWorkSheet.FirstRow();
                oCursorRow = oCursorRow.RowBelow();

                while (!oCursorRow.Cell(1).IsEmpty())
                {
                    iValores[1] = "";//A
                    iValores[2] = "";//B
                    iValores[3] = "";//C
                    iValores[4] = "";//D
                    iValores[5] = "";//E
                    iValores[6] = "";//F
                    iValores[7] = "";//G
                    iValores[8] = "";//H
                    iValores[9] = "";//I
                    iValores[10] = "";//J Fono
                    iValores[11] = "";//K
                    iValores[12] = "";//L
                    iValores[13] = "";//M
                    iValores[14] = "";//N
                    iValores[15] = "";//O
                    iValores[16] = "";//P
                    iValores[17] = "";//Q
                    iValores[18] = "";//R
                    iValores[19] = "";//S
                    iValores[20] = "";//T
                    iValores[21] = "";//U
                    iValores[22] = "";//V
                    iValores[23] = "";//W
                    iValores[24] = "";//X
                    iValores[25] = "";//Y
                    iValores[26] = "";//Z
                    iValores[27] = "";//AA
                    iValores[28] = "";//AB
                    iValores[29] = "";//AC
                    iValores[30] = "";//AD
                    iValores[31] = "";//AE
                    iValores[32] = "";//AF

                    foreach (var oFile in oListaColumnas)
                    {
                        iValores[oFile.PK_PAR_COL_ID] = oCursorRow.Cell(oFile.FL_PAR_EXC_LETRA).Value.ToString();                       
                    }

                    ListaExcel oListaExcel = new ListaExcel();

                    oListaExcel.Id = iValores[1]; //el 1 representa el id de la tabla TB_PAR_COL_COLUMNA
                    oListaExcel.Referencia = iValores[2];
                    oListaExcel.Destinatario = iValores[3];
                    oListaExcel.Rut = iValores[4];
                    oListaExcel.ContactoEntrega = iValores[5];
                    oListaExcel.Direccion = iValores[6];
                    oListaExcel.Comuna = iValores[7];
                    oListaExcel.Bulto = iValores[8];
                    oListaExcel.Peso = iValores[9];
                    oListaExcel.Telefono = iValores[10];
                    oListaExcel.IdComuna = 0;
                    oListaExcel.DireccionNumero = iValores[11];
                    oListaExcel.Localidad = iValores[12];
                    oListaExcel.Via = iValores[13].ToUpper();
                    oListaExcel.CodigoPostal = iValores[14];
                    oListaExcel.Largo = iValores[15];
                    oListaExcel.Alto = iValores[16];
                    oListaExcel.Ancho = iValores[17];
                    oListaExcel.Observacion1 = iValores[18];
                    oListaExcel.Observacion2 = iValores[19];
                    oListaExcel.Contacto1Rut = iValores[20];
                    oListaExcel.Contacto1Nombre = iValores[21];                          
                    oListaExcel.Contacto1Telefono = iValores[22];
                    oListaExcel.Contacto2Rut = iValores[23];
                    oListaExcel.Contacto2Nombre = iValores[24];
                    oListaExcel.Contacto2Telefono = iValores[25];
                    oListaExcel.Contacto3Rut = iValores[26];
                    oListaExcel.Contacto3Nombre = iValores[27];
                    oListaExcel.Contacto3Telefono = iValores[28];
                    oListaExcel.TipoDocumento=iValores[29];
                    oListaExcel.NumeroDocumento = iValores[30];
                    oListaExcel.PesoVolumetrico = iValores[31];
                    oListaExcel.ContraPago = iValores[32];

                    oLista.Add(oListaExcel);

                    oCursorRow = oCursorRow.RowBelow();
                }                

                oCursorRow = null;
                oWorkBook = null;
                oWorkSheet = null;
                //oRange = null;
                #endregion

            }
            else
            {
                oLista = null;
            }           
                        
            
            return (oLista);
            

        }
        #endregion

        #region DropDownComunaRetiro
        public ActionResult DropDownComunaRetiro(string idRegion)
        {
            ValidationController oValidation = new ValidationController();

            int intRegion=0;
            if (oValidation.isDecimal(idRegion) == true)
            {
                intRegion = Convert.ToInt32(idRegion);
            }

            LinqBD_PARDataContext oPar= new LinqBD_PARDataContext();

            var oDatos = from com in oPar.TB_PAR_COM_COMUNA
                         join prv in oPar.TB_PAR_PRV_PROVINCIA on com.PK_PAR_PRV_ID equals prv.PK_PAR_PRV_ID
                         where prv.PK_PAR_REG_ID==intRegion
                         select new SelectListItem
                         {
                             Text=com.FL_PAR_COM_NOMBRE,
                             Value=com.PK_PAR_COM_ID.ToString()
                         };
            IngresoPedidoModel oModel= new IngresoPedidoModel{
                oListaComuna=oDatos,
                RetiroRegion=idRegion
            };
            return PartialView("_DropDownComunaRetiro", oModel);
        }
        #endregion

        #region DropDownLocalidadRetiro
        [Authorize]
        public ActionResult DropDownLocalidadRetiro(string idRegion, string idComuna)
        {
            ValidationController oValidation = new ValidationController();
            int intRegion = 0;
            if (oValidation.isDecimal(idRegion) == true)
            {
                intRegion = Convert.ToInt32(idRegion);
            }
            int intComuna = 0;
            if (oValidation.isDecimal(idComuna) == true)
            {
                intComuna = Convert.ToInt32(idComuna);
            }
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oDatos = from loc in oPar.TB_PAR_LOC_LOCALIDAD
                         where loc.PK_PAR_COM_ID == intComuna
                         select new SelectListItem
                         {
                            Text=loc.FL_PAR_LOC_NOMBRE,
                            Value=loc.PK_PAR_LOC_ID.ToString()
                         };

            IngresoPedidoModel oModel = new IngresoPedidoModel
            {
                oListaLocalidad=oDatos
            };
            return PartialView("_DropDownLocalidadRetiro", oModel);
        }
        #endregion

        #region DropDownComuna
        public ActionResult DropDownComuna(string idRegion,int Servicio)
        {
            IngresoPedidoModel oModel = new IngresoPedidoModel();

            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oDatos = from col in oPar.TB_PAR_EXC_EXCEL
                         where col.PK_PAR_SER_ID == Servicio
                         select col;

            ColumnasParametricas oColumnaObligatoria = new ColumnasParametricas();
            foreach (var oColumna in oDatos)
            {
                switch (oColumna.PK_PAR_COL_ID)
                {
                    case 1://1	Id
                        
                        oColumnaObligatoria.Id = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 2://2	Referencia
                        
                        oColumnaObligatoria.Referencia = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 3://3	Destinatario
                       
                        oColumnaObligatoria.Destinatario = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 4:// 4	Rut
                       
                        oColumnaObligatoria.Rut = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 5://5	Contacto Entrega
                        
                        oColumnaObligatoria.ContactoEntrega = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 6://6	Dirección
                        
                        oColumnaObligatoria.Direccion = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 7://7	Comuna
                        
                        oColumnaObligatoria.Comuna = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 8://8	Bulto
                       
                        oColumnaObligatoria.Bulto = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 9://9	Peso
                        
                        oColumnaObligatoria.Peso = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 10://10	Fono                        
                        oColumnaObligatoria.Fono = oColumna.FL_PAR_EXC_OBLIGATORIA;break;
                    case 11://11	Número Direccion                        
                        oColumnaObligatoria.NDireccion = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 12://12	Localidad                       
                        oColumnaObligatoria.Localidad = oColumna.FL_PAR_EXC_OBLIGATORIA;break;
                    case 13://13	Vía                        
                        oColumnaObligatoria.Via = oColumna.FL_PAR_EXC_OBLIGATORIA;break;
                    case 14://14	Código Postal                        
                        oColumnaObligatoria.CodigoPostal = oColumna.FL_PAR_EXC_OBLIGATORIA;break;
                }
            }
            
            oModel.oObligatoria = oColumnaObligatoria;


            if (ValidationController.IsNumeric(idRegion) == true)
            {

                

                List<SelectListItem> oComuna = new List<SelectListItem>();

                oComuna.Add(new SelectListItem { Text = "-- Seleccione Comuna --", Value = "",Selected=true });


                oComuna.AddRange(from com in oPar.TB_PAR_COM_COMUNA
                                     join prv in oPar.TB_PAR_PRV_PROVINCIA on com.PK_PAR_PRV_ID equals prv.PK_PAR_PRV_ID
                                     where prv.PK_PAR_REG_ID == Convert.ToInt32(idRegion)
                                     orderby com.FL_PAR_COM_NOMBRE
                                     select new SelectListItem
                                     {
                                         Text = com.FL_PAR_COM_NOMBRE,
                                         Value = com.PK_PAR_COM_ID.ToString()
                                     });
                oModel.oListaComuna = oComuna;
            }
            else
            {
                List<SelectListItem> oComunaBlank = new List<SelectListItem>();
                oComunaBlank.Add(new SelectListItem { Text = "-- Seleccione Comuna --", Value = "" });
                                
                oModel.oListaComuna = oComunaBlank;
            }
            return PartialView("_DropDownComuna", oModel);
        }
        #endregion

        #region DropDownLocalidad
        public ActionResult DropDownLocalidad(string idComuna,int Servicio)
        {
            IngresoPedidoModel oModel = new IngresoPedidoModel();
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            var oDatos = from col in oPar.TB_PAR_EXC_EXCEL
                         where col.PK_PAR_SER_ID == Servicio
                         select col;

            ColumnasParametricas oColumnaObligatoria = new ColumnasParametricas();
            foreach (var oColumna in oDatos)
            {
                switch (oColumna.PK_PAR_COL_ID)
                {
                    case 1://1	Id

                        oColumnaObligatoria.Id = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 2://2	Referencia

                        oColumnaObligatoria.Referencia = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 3://3	Destinatario

                        oColumnaObligatoria.Destinatario = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 4:// 4	Rut

                        oColumnaObligatoria.Rut = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 5://5	Contacto Entrega

                        oColumnaObligatoria.ContactoEntrega = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 6://6	Dirección

                        oColumnaObligatoria.Direccion = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 7://7	Comuna

                        oColumnaObligatoria.Comuna = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 8://8	Bulto

                        oColumnaObligatoria.Bulto = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 9://9	Peso

                        oColumnaObligatoria.Peso = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 10://10	Fono                        
                        oColumnaObligatoria.Fono = oColumna.FL_PAR_EXC_OBLIGATORIA; break;
                    case 11://11	Número Direccion                        
                        oColumnaObligatoria.NDireccion = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 12://12	Localidad                       
                        oColumnaObligatoria.Localidad = oColumna.FL_PAR_EXC_OBLIGATORIA; break;
                    case 13://13	Vía                        
                        oColumnaObligatoria.Via = oColumna.FL_PAR_EXC_OBLIGATORIA; break;
                    case 14://14	Código Postal                        
                        oColumnaObligatoria.CodigoPostal = oColumna.FL_PAR_EXC_OBLIGATORIA; break;
                }
            }

            oModel.oObligatoria = oColumnaObligatoria;
            if (ValidationController.IsNumeric(idComuna) == true)
            {
                

                List<SelectListItem> oLocalidad = new List<SelectListItem>();

                oLocalidad.Add(new SelectListItem { Text = "-- Seleccione Localidad --", Value = "", Selected = true });

                oLocalidad.AddRange (from loc in oPar.TB_PAR_LOC_LOCALIDAD
                                        where loc.PK_PAR_COM_ID == Convert.ToInt32(idComuna)
                                        orderby loc.FL_PAR_LOC_NOMBRE
                                        select new SelectListItem
                                        {
                                            Text = loc.FL_PAR_LOC_NOMBRE,
                                            Value = loc.PK_PAR_LOC_ID.ToString()
                                        });
                oModel.oListaLocalidad=oLocalidad;
            }
            else
            {
                List<SelectListItem> oListaBlancoLocalidad = new List<SelectListItem>();
                oListaBlancoLocalidad.Add(new SelectListItem { Text = "-- Seleccione Localidad --", Value = "" });
                oModel.oListaLocalidad = oListaBlancoLocalidad;
            }
            return PartialView("_DropDownLocalidad", oModel);
        }
        #endregion

        #region GuardarIngresoManual
        public class RetornoIngresoManual
        {
            public bool Ok { get; set; }
            public string Mensaje { get; set; }
        }
        [HttpPost]
        [Authorize]
        public ActionResult GuardarIngresoManual(IngresoPedidoModel oModel)
        {
            RetornoIngresoManual oResultIngreso = new RetornoIngresoManual();
            try
            {
                ValidationController oValidation = new ValidationController();
                if (oModel.vBulto != "" && oModel.vBulto != null) oModel.Bulto = oModel.vBulto;
                if (oModel.vCodigoPostal != "" && oModel.vCodigoPostal != null) oModel.CodigoPostal = oModel.vCodigoPostal;
                if (oModel.vComuna != "" && oModel.vComuna != null) oModel.Comuna = oModel.vComuna;
                if (oModel.vContactoEntrega != "" && oModel.vContactoEntrega != null) oModel.ContactoEntrega = oModel.vContactoEntrega;
                if (oModel.vDestinatario != "" && oModel.vDestinatario != null) oModel.Destinatario = oModel.vDestinatario;
                if (oModel.vDireccion != "" && oModel.vDireccion != null) oModel.Direccion = oModel.vDireccion;
                if (oModel.vDireccionNumero != "" && oModel.vDireccionNumero != null) oModel.DireccionNumero = oModel.vDireccionNumero;
                if (oModel.vId != "" && oModel.vId != null) oModel.Id = oModel.vId;
                if (oModel.vLocalidad != "" && oModel.vLocalidad != null) oModel.Localidad = oModel.vLocalidad;
                if (oModel.vPeso != "" && oModel.vPeso != null) oModel.Peso = oModel.vPeso;
                if (oModel.vReferencia != "" && oModel.vReferencia != null) oModel.Referencia = oModel.vReferencia;
                if (oModel.vRut != "" && oModel.vRut != null) oModel.Rut = oModel.vRut;
                if (oModel.vTelefono != "" && oModel.vTelefono != null) oModel.Telefono = oModel.vTelefono;
                if (oModel.vVia != "" && oModel.vVia != null) oModel.Via = oModel.vVia;
                if (oModel.vLargo != "" && oModel.vLargo != null) oModel.Largo = oModel.vLargo;
                if (oModel.vAncho != "" && oModel.vAncho != null) oModel.Ancho = oModel.vAncho;
                if (oModel.vAlto != "" && oModel.vAlto != null) oModel.Alto = oModel.vAlto;

                if (oModel.vObservacion1 != "" && oModel.vObservacion1 != null) oModel.Observacion1 = oModel.vObservacion1;
                if (oModel.vObservacion2 != "" && oModel.vObservacion2 != null) oModel.Observacion2 = oModel.vObservacion2;

                if (oModel.vTipoDocumento != "" && oModel.vTipoDocumento != null) oModel.TipoDocumento = oModel.vTipoDocumento;
                if (oModel.vNumeroDocumento != "" && oModel.vNumeroDocumento != null) oModel.NumeroDocumento = oModel.vNumeroDocumento;

                if (oModel.vContacto1Rut != "" && oModel.vContacto1Rut != null) oModel.Contacto1Rut = oModel.vContacto1Rut;
                if (oModel.vContacto1Nombre != "" && oModel.vContacto1Nombre != null) oModel.Contacto1Nombre = oModel.vContacto1Nombre;
                if (oModel.vContacto1Telefono != "" && oModel.vContacto1Telefono != null) oModel.Contacto1Telefono = oModel.vContacto1Telefono;

                if (oModel.vContacto2Rut != "" && oModel.vContacto2Rut != null) oModel.Contacto2Rut = oModel.vContacto2Rut;
                if (oModel.vContacto2Nombre != "" && oModel.vContacto2Nombre != null) oModel.Contacto2Nombre = oModel.vContacto2Nombre;
                if (oModel.vContacto2Telefono != "" && oModel.vContacto2Telefono != null) oModel.Contacto2Telefono = oModel.vContacto2Telefono;

                if (oModel.vContacto3Rut != "" && oModel.vContacto3Rut != null) oModel.Contacto3Rut = oModel.vContacto3Rut;
                if (oModel.vContacto3Nombre != "" && oModel.vContacto3Nombre != null) oModel.Contacto3Nombre = oModel.vContacto3Nombre;
                if (oModel.vContacto3Telefono != "" && oModel.vContacto3Telefono != null) oModel.Contacto3Telefono = oModel.vContacto3Telefono;

                if (oModel.vPesoVolumetrico != "" && oModel.vPesoVolumetrico != null) oModel.PesoVolumetrico = oModel.vPesoVolumetrico;

                if (oModel.vContraPago != "" && oModel.vContraPago != null) oModel.ContraPago = oModel.vContraPago;

                //Vía Nulo dejo Terrestre

                if (oModel.Via == null || oModel.Via == "")
                {
                    oModel.Via = "T";
                }

            
                //Validación de espacios referencias
                if (oModel.Referencia != null) oModel.Referencia = oModel.Referencia.Trim();
                if (oModel.Id != null) oModel.Id = oModel.Id.Trim();

                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                TB_FOL_OTD_OT_DETALLE oOTD = new TB_FOL_OTD_OT_DETALLE();
                TB_FOL_OTP_OT_PRINCIPAL oOTP = new TB_FOL_OTP_OT_PRINCIPAL();
                

                int oSwt = 0;
                int oSwtObs = 0;
                int oSwtCon = 0;

                System.Data.Linq.EntitySet<TB_FOL_OCA_OBSERVACION_CARGA> oOCA = new System.Data.Linq.EntitySet<TB_FOL_OCA_OBSERVACION_CARGA>();
                System.Data.Linq.EntitySet<TB_FOL_CEN_CONTACTO_ENTREGA> oCEN = new System.Data.Linq.EntitySet<TB_FOL_CEN_CONTACTO_ENTREGA>();
                System.Data.Linq.EntitySet<TB_FOL_RCL_REFERENCIA_CLIENTE> oRCL = new System.Data.Linq.EntitySet<TB_FOL_RCL_REFERENCIA_CLIENTE>();



                decimal Contrapago = 0;
                if (oValidation.isDecimal(oModel.ContraPago)==true)
                    Contrapago=Convert.ToDecimal(oModel.ContraPago);


                if (oValidation.isDecimal(oModel.OT_Papel_Paso))
                {
                    decimal oValidaPaper = Convert.ToDecimal(oModel.OT_Papel_Paso);

                    if (oValidation.IsNumeric2(oModel.TMM_ID))
                    {
                        oFol.PR_FOL_RMM_INSERT(oValidaPaper, Convert.ToInt32(oModel.TMM_ID));
                    }
                    oOTP = (from otp in oFol.TB_FOL_OTP_OT_PRINCIPAL
                            where otp.PK_FOL_OTP_ID == oValidaPaper
                            select otp).Single();

                    var ListoOTD = from otd in oFol.TB_FOL_OTD_OT_DETALLE
                                   where otd.PK_FOL_OTP_ID == oValidaPaper
                                   orderby otd.PK_FOL_OTD_ID descending
                                   select otd;

                    if (ListoOTD.Count() == 0)
                    {
                        oOTD = new TB_FOL_OTD_OT_DETALLE
                        {
                            PK_FOL_OTD_ID = 1
                        };
                    }
                    else
                    {
                        var LastOT = ListoOTD.ToList()[0];
                        if (LastOT.PK_FOL_EST_ID == 27)
                        {
                            oOTD = (from otd in oFol.TB_FOL_OTD_OT_DETALLE
                                    where otd.PK_FOL_OTP_ID == oValidaPaper
                                    && otd.PK_FOL_OTD_ID == 1
                                    select otd).Single();
                            oSwt = 1;
                        }
                        else
                        {
                            oOTD = new TB_FOL_OTD_OT_DETALLE
                            {
                                PK_FOL_OTD_ID = LastOT.PK_FOL_OTD_ID + 1
                            };
                        }
                    }


                }

                if (oModel.Observacion1 != null && oModel.Observacion1 != "")
                {
                    oOCA.Add(new TB_FOL_OCA_OBSERVACION_CARGA
                    {
                        FL_FOL_OCA_OBSERVACION = oModel.Observacion1
                    });
                    oSwtObs = 1;
                }

                if (oModel.Observacion2 != null && oModel.Observacion2 != "")
                {
                    oOCA.Add(new TB_FOL_OCA_OBSERVACION_CARGA
                    {
                        FL_FOL_OCA_OBSERVACION = oModel.Observacion2
                    });
                    oSwtObs = 1;
                }

                if ((oModel.Contacto1Rut != null && oModel.Contacto1Rut != "") || (oModel.Contacto1Nombre != null && oModel.Contacto1Nombre != "") || (oModel.Contacto1Telefono != null && oModel.Contacto1Telefono != ""))
                {
                    oCEN.Add(new TB_FOL_CEN_CONTACTO_ENTREGA
                    {
                        FL_FOL_CEN_NOMBRE = oModel.Contacto1Nombre,
                        FL_FOL_CEN_RUT = oModel.Contacto1Rut,
                        FL_FOL_CEN_TELEFONO = oModel.Contacto1Telefono
                    });
                    oSwtCon = 1;
                }
                if ((oModel.Contacto2Rut != null && oModel.Contacto2Rut != "") || (oModel.Contacto2Nombre != null && oModel.Contacto2Nombre != "") || (oModel.Contacto2Telefono != null && oModel.Contacto2Telefono != ""))
                {
                    oCEN.Add(new TB_FOL_CEN_CONTACTO_ENTREGA
                    {
                        FL_FOL_CEN_NOMBRE = oModel.Contacto2Nombre,
                        FL_FOL_CEN_RUT = oModel.Contacto2Rut,
                        FL_FOL_CEN_TELEFONO = oModel.Contacto2Telefono
                    });
                    oSwtCon = 1;
                }
                if ((oModel.Contacto3Rut != null && oModel.Contacto3Rut != "") || (oModel.Contacto3Nombre != null && oModel.Contacto3Nombre != "") || (oModel.Contacto3Telefono != null && oModel.Contacto3Telefono != ""))
                {
                    oCEN.Add(new TB_FOL_CEN_CONTACTO_ENTREGA
                    {
                        FL_FOL_CEN_NOMBRE = oModel.Contacto3Nombre,
                        FL_FOL_CEN_RUT = oModel.Contacto3Rut,
                        FL_FOL_CEN_TELEFONO = oModel.Contacto3Telefono
                    });
                    oSwtCon = 1;
                }

                if (oSwtObs == 1) //Si hay observación la agrego
                {
                    oOTD.TB_FOL_OCA_OBSERVACION_CARGA = oOCA;
                }
                if (oSwtCon == 1) //Si hay Contacto de entrega lo agrego
                {
                    oOTD.TB_FOL_CEN_CONTACTO_ENTREGA = oCEN;
                }

                //1	Id
                oOTD.FL_FOL_OTD_REFERENCIA1 = oModel.Id;
                //2	Referencia
                oOTD.FL_FOL_OTD_REFERENCIA2 = oModel.Referencia;
                //3	Destinatario
                oOTD.FL_FOL_OTD_DESTINATARIO_NOMBRE = oModel.Destinatario;
                //4	Rut
                oOTD.FL_FOL_OTD_DESTINATARIO_RUT = oModel.Rut;
                //5	Contacto Entrega
                oOTD.FL_FOL_OTD_CONTACTO_ENTREGA = oModel.ContactoEntrega;
                //6	Dirección
                oOTD.FL_FOL_OTD_DESTINATARIO_DIRECCION = oModel.Direccion;
                //7	Comuna
                if (ValidationController.IsNumeric(oModel.Comuna))
                {
                    oOTD.PK_PAR_COM_ID = Convert.ToInt32(oModel.Comuna);
                }
                //8	Bulto            
                if (ValidationController.IsNumeric(oModel.Bulto))
                    oOTD.FL_FOL_OTD_BULTO = Convert.ToDecimal(oModel.Bulto);
                else
                    oOTD.FL_FOL_OTD_BULTO = 0;
                //9	Peso
                float oPeso = 0;
                if (oValidation.isDecimal(oModel.Peso))
                {
                    oPeso = float.Parse(oModel.Peso);
                }
                else
                {
                    oPeso = 0;
                }

                oOTD.FL_FOL_OTD_PESO = oPeso;
                //10 Fono
                oOTD.FL_FOL_OTD_TELEFONO = oModel.Telefono;
                //11 N° Dirección
                oOTD.FL_FOL_OTD_DESTINATARIO_NUMERO = oModel.DireccionNumero;
                //12 Localidad
                if (ValidationController.IsNumeric(oModel.Localidad))
                    oOTD.PK_PAR_LOC_ID = Convert.ToInt32(oModel.Localidad);
                //13 Vía
                oOTD.FL_FOL_OTD_VIA = oModel.Via;
                //14 Código Postal
                oOTD.FL_FOL_OTD_CODIGO_POSTAL = oModel.CodigoPostal;
                //15	Largo
                if (oValidation.isDecimal(oModel.Largo))
                {
                    oOTD.FL_FOL_OTD_LARGO = float.Parse(oModel.Largo);
                }
                //16	Alto
                if (oValidation.isDecimal(oModel.Alto))
                {
                    oOTD.FL_FOL_OTD_ALTO = float.Parse(oModel.Alto);
                }
                //17	Ancho
                if (oValidation.isDecimal(oModel.Ancho))
                {
                    oOTD.FL_FOL_OTD_ANCHO = float.Parse(oModel.Ancho);
                }

                var oResultTipo = oValidation.Sucursal(oValidation.GetRutActiveUser().ToString() + "-1");
                int EstadoFinal = 1;
                if (oResultTipo.Tipo == 2)
                    EstadoFinal = 20;//EN RUTA DESDE CLIENTE A SUCURSAL


                oOTD.PK_FOL_EST_ID = EstadoFinal;//EN BODEGA CLIENTE

                //ID OT DETALLE
                if (!oValidation.isDecimal(oModel.OT_Papel_Paso))
                {
                    oOTD.PK_FOL_OTD_ID = 1;
                }

                //Sucursal COmuna                        
                oOTD.PK_PAR_SUC_ID = oValidation.GetSucursalIDfromActiveUser();
                oOTD.PK_PAR_USU_RUT = oValidation.GetRutActiveUser();


                //PIN
                float oPesoVolumetrico = 0;

                //Peso Volumétrico
                if (oValidation.isDecimal(oModel.PesoVolumetrico))
                    float.TryParse(oModel.PesoVolumetrico, out oPesoVolumetrico);
                else
                {
                    oPesoVolumetrico = 0;
                }

                System.Data.Linq.EntitySet<TB_FOL_PIN_PESO_INFORMADO> oPIN = new System.Data.Linq.EntitySet<TB_FOL_PIN_PESO_INFORMADO>();


                oPIN.Add(new TB_FOL_PIN_PESO_INFORMADO
                {
                    FL_FOL_PIN_PESO = oPeso,
                    FL_FOL_PIN_PESOVOLUMETRICO = oPesoVolumetrico
                });

                //OT PRINCIPAL
                oOTP.PK_CLI_EMP_RUT = Convert.ToInt32(oModel.RutCliente.Substring(0, oModel.RutCliente.Length - 2));
                oOTP.PK_PAR_SER_ID = Convert.ToInt32(oModel.Servicio);
                oOTP.FL_FOL_OTP_FECHA_CREACION = DateTime.Now;
                oOTP.PK_FOL_EST_ID = EstadoFinal;//EN BODEGA CLIENTE
                oOTP.PK_PAR_SUC_ID = oValidation.GetSucursalIDfromActiveUser();
                oOTP.PK_PAR_USU_RUT = oValidation.GetRutActiveUser();
                if (oValidation.isDecimal(oModel.OT_Papel_Paso))
                {
                    oOTP.PK_FOL_TOT_ID = 3;
                }
                else
                {
                    oOTP.PK_FOL_TOT_ID = 2;
                }

                //RETIRO
                if (oModel.opt == "2")
                {
                    int intLocalidad = 0;
                    if (oModel.RetiroLocalidad != null && oModel.RetiroLocalidad != "")
                    {
                        intLocalidad = Convert.ToInt32(oModel.RetiroLocalidad);
                    }

                    System.Data.Linq.EntitySet<TB_FOL_RET_RETIRO> oRet = new System.Data.Linq.EntitySet<TB_FOL_RET_RETIRO>();

                    oRet.Add(new TB_FOL_RET_RETIRO
                    {
                        PK_PAR_COM_ID = Convert.ToInt32(oModel.RetiroComuna),
                        PK_PAR_LOC_ID = intLocalidad,
                        FL_FOL_RET_FECHA = Convert.ToDateTime(oModel.RetiroFecha),
                        FL_FOL_RET_DIRECCION = oModel.RetiroDireccion,
                        FL_FOL_RET_NUMERO = oModel.RetiroNumero
                    });

                    oOTD.TB_FOL_RET_RETIRO = oRet;

                }





                //ASIGNO OTP A OTD, EL ENCABEZADO AL DETALLE.

                if (oValidation.isDecimal(oModel.OT_Papel_Paso))
                {
                    oOTD.PK_FOL_OTP_ID = Convert.ToDecimal(oModel.OT_Papel_Paso);
                }
                else
                {
                    oOTD.TB_FOL_OTP_OT_PRINCIPAL = oOTP;
                }

                // CREO REFERENCIA

                //REF 1
                System.Data.Linq.EntitySet<TB_FOL_DOA_DOC_ASOCIADO> oDOC = new System.Data.Linq.EntitySet<TB_FOL_DOA_DOC_ASOCIADO>();
                System.Data.Linq.EntitySet<TB_FOL_REA_REF_ASOCIADO> oREA = new System.Data.Linq.EntitySet<TB_FOL_REA_REF_ASOCIADO>();
                


                if (oOTD.FL_FOL_OTD_REFERENCIA1 != string.Empty && oOTD.FL_FOL_OTD_REFERENCIA1 != null)
                {
                    if (oValidation.isDecimal(oOTD.FL_FOL_OTD_REFERENCIA1))
                    {
                        LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
                        var oRef = (from res in oPar.TB_PAR_RES_REFERENCIA_SERVICIO
                                    join refe in oPar.TB_PAR_REF_REFERENCIA on res.PK_PAR_REF_ID equals refe.PK_PAR_REF_ID
                                    join tdo in oPar.TB_PAR_TDO_TIPO_DOCUMENTO on res.PK_PAR_TDO_ID equals tdo.PK_PAR_TDO_ID
                                    where
                                     res.PK_PAR_SER_ID == oOTP.PK_PAR_SER_ID                                     
                                     && refe.PK_PAR_REF_ID == 1//REF 1
                                    select new { res, refe,tdo }).Single();

                        if (oRef.tdo.PK_PAR_TRE_ID == 1)//Es tipo Documento, se puede asociar.
                        {
                            oDOC.Add(new TB_FOL_DOA_DOC_ASOCIADO
                            {
                                PK_PAR_TDO_ID = oRef.res.PK_PAR_TDO_ID,
                                PK_CLI_EMP_RUT = Convert.ToInt32(oOTP.PK_CLI_EMP_RUT),
                                PK_FOL_DOA_NUMERO = Convert.ToDecimal(oOTD.FL_FOL_OTD_REFERENCIA1),
                                PK_PAR_SUC_ID = Convert.ToInt32(oOTP.PK_PAR_SUC_ID),
                                PK_PAR_USU_RUT = Convert.ToInt32(oOTP.PK_PAR_USU_RUT),
                                FL_FOL_DOA_FECHA = DateTime.Now,
                                FL_FOL_DOA_CONTRAPAGO=Contrapago
                            });
                            Contrapago=0;
                        }
                        else //Es tipo Referencia, se agrega a referencias
                        {
                            oREA.Add(new TB_FOL_REA_REF_ASOCIADO
                            {
                                PK_PAR_TDO_ID=oRef.res.PK_PAR_TDO_ID,
                                PK_CLI_EMP_RUT = Convert.ToInt32(oOTP.PK_CLI_EMP_RUT),
                                PK_FOL_REA_NUMERO=oOTD.FL_FOL_OTD_REFERENCIA1,
                                PK_PAR_SUC_ID = Convert.ToInt32(oOTP.PK_PAR_SUC_ID),
                                PK_PAR_USU_RUT = Convert.ToInt32(oOTP.PK_PAR_USU_RUT),
                                FL_FOL_REA_FECHA = DateTime.Now                                
                            });
                        }
                    }
                }
                if (oOTD.FL_FOL_OTD_REFERENCIA2 != string.Empty && oOTD.FL_FOL_OTD_REFERENCIA2 != null)
                {
                    if (oValidation.isDecimal(oOTD.FL_FOL_OTD_REFERENCIA2))
                    {
                        LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
                        var oRef = (from res in oPar.TB_PAR_RES_REFERENCIA_SERVICIO
                                    join refe in oPar.TB_PAR_REF_REFERENCIA on res.PK_PAR_REF_ID equals refe.PK_PAR_REF_ID
                                    join tdo in oPar.TB_PAR_TDO_TIPO_DOCUMENTO on res.PK_PAR_TDO_ID equals tdo.PK_PAR_TDO_ID
                                    where
                                     res.PK_PAR_SER_ID == oOTP.PK_PAR_SER_ID                                     
                                     && refe.PK_PAR_REF_ID == 2//REF 2
                                    select new { res, refe,tdo }).Single();
                        if (oRef.tdo.PK_PAR_TRE_ID == 1)//Es tipo Documento, se puede asociar.
                        {
                            oDOC.Add(new TB_FOL_DOA_DOC_ASOCIADO
                            {
                                PK_PAR_TDO_ID = oRef.res.PK_PAR_TDO_ID,
                                PK_CLI_EMP_RUT = Convert.ToInt32(oOTP.PK_CLI_EMP_RUT),
                                PK_FOL_DOA_NUMERO = Convert.ToDecimal(oOTD.FL_FOL_OTD_REFERENCIA2),
                                PK_PAR_SUC_ID = Convert.ToInt32(oOTP.PK_PAR_SUC_ID),
                                PK_PAR_USU_RUT = Convert.ToInt32(oOTP.PK_PAR_USU_RUT),
                                FL_FOL_DOA_FECHA = DateTime.Now,
                                FL_FOL_DOA_CONTRAPAGO=Contrapago
                            });
                            Contrapago=0;
                        }
                        else //Es tipo Referencia, se agrega a referencias
                        {
                            oREA.Add(new TB_FOL_REA_REF_ASOCIADO
                            {
                                PK_PAR_TDO_ID = oRef.res.PK_PAR_TDO_ID,
                                PK_CLI_EMP_RUT = Convert.ToInt32(oOTP.PK_CLI_EMP_RUT),
                                PK_FOL_REA_NUMERO = oOTD.FL_FOL_OTD_REFERENCIA2,
                                PK_PAR_SUC_ID = Convert.ToInt32(oOTP.PK_PAR_SUC_ID),
                                PK_PAR_USU_RUT = Convert.ToInt32(oOTP.PK_PAR_USU_RUT),
                                FL_FOL_REA_FECHA = DateTime.Now
                            });
                        }
                    }
                }

                //TIPO DOCUMENTOS DEFINIDOS POR EL CLIENTE
                if (oModel.TipoDocumento != null && oModel.TipoDocumento != "" && oModel.NumeroDocumento != null && oModel.NumeroDocumento != "")
                {
                    var oCOC = (from pr in oFol.PR_FOL_COC_BUSCAR_ORIGEN(Convert.ToInt32(oModel.TipoDocumento), Convert.ToInt32(oModel.RutCliente.Substring(0, oModel.RutCliente.Length - 2)))
                               select pr).ToList();

                    if (oCOC.Count() > 0)
                    {
                        var oElCOC = oCOC.ToList()[0];
                        if (oElCOC.PK_PAR_TRE_ID == 1)//Es tipo Documento, se puede asociar.
                        {
                            oDOC.Add(new TB_FOL_DOA_DOC_ASOCIADO
                            {
                                PK_PAR_TDO_ID = oElCOC.PK_PAR_TDO_ID,
                                PK_CLI_EMP_RUT = Convert.ToInt32(oOTP.PK_CLI_EMP_RUT),
                                PK_FOL_DOA_NUMERO = Convert.ToDecimal(oModel.NumeroDocumento),
                                PK_PAR_SUC_ID = Convert.ToInt32(oOTP.PK_PAR_SUC_ID),
                                PK_PAR_USU_RUT = Convert.ToInt32(oOTP.PK_PAR_USU_RUT),
                                FL_FOL_DOA_FECHA = DateTime.Now,
                                FL_FOL_DOA_CONTRAPAGO=Contrapago
                            });
                            Contrapago=0;
                        }
                        else //Es tipo Referencia, se agrega a referencias
                        {
                            oREA.Add(new TB_FOL_REA_REF_ASOCIADO
                            {
                                PK_PAR_TDO_ID = oElCOC.PK_PAR_TDO_ID,
                                PK_CLI_EMP_RUT = Convert.ToInt32(oOTP.PK_CLI_EMP_RUT),
                                PK_FOL_REA_NUMERO = oModel.NumeroDocumento,
                                PK_PAR_SUC_ID = Convert.ToInt32(oOTP.PK_PAR_SUC_ID),
                                PK_PAR_USU_RUT = Convert.ToInt32(oOTP.PK_PAR_USU_RUT),
                                FL_FOL_REA_FECHA = DateTime.Now
                            });
                        }
                        
                        oRCL.Add(new TB_FOL_RCL_REFERENCIA_CLIENTE
                        {
                            FL_FOL_RCL_TIPO = Convert.ToInt32(oModel.TipoDocumento),
                            FL_FOL_RCL_NUMERO = oModel.NumeroDocumento
                        });

                        oOTD.TB_FOL_RCL_REFERENCIA_CLIENTE = oRCL;
                    }
                }
                //INSERTO

                oOTD.TB_FOL_DOA_DOC_ASOCIADO = oDOC;
                oOTD.TB_FOL_REA_REF_ASOCIADO = oREA;
                oOTD.TB_FOL_PIN_PESO_INFORMADO = oPIN;

                if (oSwt==0)
                {
                    oFol.TB_FOL_OTD_OT_DETALLE.InsertOnSubmit(oOTD);
                }

              

                DateTime oTiempo = DateTime.Now;

                int ResultLocalidadid = 0;
                ValidationController.ResultValidarLocalidad oResultLocalidad = new ValidationController.ResultValidarLocalidad();
                
                if (oModel.Localidad != "")
                {
                    oResultLocalidad = oValidation.ValidarLocalidad(oModel.Localidad, Convert.ToInt32(oModel.Comuna));
                    ResultLocalidadid = oResultLocalidad.LocalidadId;
                }

                if (ValidationController.IsNumeric(oModel.Localidad))
                {
                    BusquedaController oBusqueda = new BusquedaController();
                    BusquedaModels oModelBusqueda = new BusquedaModels();
                    ValidationController.ResultValidarComuna oResultValidar = new ValidationController.ResultValidarComuna();
                    oModelBusqueda.ComunaDestino = oModel.Comuna;
                    oModelBusqueda.LocalidadDestino = ResultLocalidadid.ToString();
                    oModelBusqueda.SucursalOrigen = oValidation.GetSucursalIDfromActiveUser().ToString();
                    oModelBusqueda = oBusqueda.FunctionProcesarBusqueda(oModelBusqueda);
                    oTiempo = DateTime.Now.AddHours(oModelBusqueda.ListaTiempo.Sum(m => m.Horas));
                }             
                else if (ValidationController.IsNumeric(oModel.Comuna))
                {
                    BusquedaController oBusqueda = new BusquedaController();
                    BusquedaModels oModelBusqueda = new BusquedaModels();
                    ValidationController.ResultValidarComuna oResultValidar = new ValidationController.ResultValidarComuna();                    
                    oModelBusqueda.ComunaDestino=oModel.Comuna;                    
                    oModelBusqueda.SucursalOrigen = oValidation.GetSucursalIDfromActiveUser().ToString();
                    oModelBusqueda = oBusqueda.FunctionProcesarBusqueda(oModelBusqueda);
                    oTiempo = DateTime.Now.AddHours(oModelBusqueda.ListaTiempo.Sum(m => m.Horas));
                }


                oOTD.FL_FOL_OTD_FECHA_ESTIMADA = oTiempo;
                
                oFol.SubmitChanges();
                oResultIngreso.Ok = true;
           
                oResultIngreso.Mensaje = string.Format("Registro Guardado, Fecha estimada de Entrega: {0} <br/> <strong>OT: {1}-{2}</strong>", oTiempo.ToShortDateString(), oOTP.PK_FOL_OTP_ID, oOTD.PK_FOL_OTD_ID);
           
           
            }
            catch (Exception e)
            {
                oResultIngreso.Ok = false;
                oResultIngreso.Mensaje = "Error :" + e.Message;
            }           
            return Json(oResultIngreso);
        }
        #endregion

        #region Carga Formulario Ingreso Manual
        [HttpPost]
        public ActionResult CargaFormularioIngresoManual(IngresoMasivoModel oModel2)
        {
            
            IngresoPedidoModel oModel = new IngresoPedidoModel() { Servicio=0 };

            oModel.opt = oModel2.opt;
            oModel.OT_Papel_Paso = oModel2.OT_Papel;
            oModel.GetListaTMM();

            oModel.RutCliente = oModel2.RutCliente;
            if (ValidationController.IsNumeric(oModel2.Servicio))//Valido que el servicio es Número
                oModel.Servicio = Convert.ToInt32(oModel2.Servicio);

            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oDatos = from col in oPar.TB_PAR_EXC_EXCEL
                         where col.PK_PAR_SER_ID==oModel.Servicio
                         select col;


            List<SelectListItem> oRegion = new List<SelectListItem>();

            oRegion.Add(new SelectListItem { Value="",Text="-- Seleccione Región --",Selected=true });

            oRegion.AddRange (from reg in oPar.TB_PAR_REG_REGION
                          orderby reg.PK_PAR_REG_ID
                          select new SelectListItem
                          {
                              Text = reg.FL_PAR_REG_NOMBRE,
                              Value = reg.PK_PAR_REG_ID.ToString()
                          });

            List<SelectListItem> oVia = new List<SelectListItem>();

            oVia.Add(new SelectListItem { Value = "", Text = "-- Seleccione --", Selected = true });
            oVia.AddRange(from via in oPar.TB_PAR_VIA_VIA                             
                             select new SelectListItem
                             {
                                 Text = via.FL_PAR_VIA_NOMBRE,
                                 Value = via.FL_PAR_VIA_ABREVIATURA.ToString()
                             });


            List<SelectListItem> oComunaBlank = new List<SelectListItem>();
            List<SelectListItem> oLocalidadBlank = new List<SelectListItem>();
           



            oComunaBlank.Add(new SelectListItem { Text = "-- Seleccione Comuna --", Value = "" });            
            oLocalidadBlank.Add(new SelectListItem { Text = "-- Seleccione Locadlidad --", Value = "" });


            oModel.oListaRegion = oRegion;
            oModel.oListaComuna = oComunaBlank;
            oModel.oListaLocalidad = oLocalidadBlank;
            oModel.oListaVia = oVia;

            ColumnasParametricas oColumnaParametrica = new ColumnasParametricas();
            ColumnasParametricas oColumnaObligatoria = new ColumnasParametricas();

            foreach (var oColumna in oDatos)
            {
                switch (oColumna.PK_PAR_COL_ID)
                {
                    case 1://1	Id
                        oColumnaParametrica.Id = true;
                        oColumnaObligatoria.Id = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 2://2	Referencia
                        oColumnaParametrica.Referencia = true;
                        oColumnaObligatoria.Referencia = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 3://3	Destinatario
                        oColumnaParametrica.Destinatario = true;
                        oColumnaObligatoria.Destinatario = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 4:// 4	Rut
                        oColumnaParametrica.Rut = true;
                        oColumnaObligatoria.Rut = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 5://5	Contacto Entrega
                        oColumnaParametrica.ContactoEntrega = true;
                        oColumnaObligatoria.ContactoEntrega = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 6://6	Dirección
                        oColumnaParametrica.Direccion = true;
                        oColumnaObligatoria.Direccion = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 7://7	Comuna
                        oColumnaParametrica.Comuna = true;
                        oColumnaObligatoria.Comuna = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 8://8	Bulto
                        oColumnaParametrica.Bulto = true;
                        oColumnaObligatoria.Bulto = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 9://9	Peso
                        oColumnaParametrica.Peso = true;
                        oColumnaObligatoria.Peso = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 10://10	Fono
                        oColumnaParametrica.Fono = true;
                        oColumnaObligatoria.Fono = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 11://11	Número Direccion
                        oColumnaParametrica.NDireccion = true;
                        oColumnaObligatoria.NDireccion = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 12://12	Localidad
                        oColumnaParametrica.Localidad = true;
                        oColumnaObligatoria.Localidad = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 13://13	Vía
                        oColumnaParametrica.Via = true;
                        oColumnaObligatoria.Via = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 14://14	Código Postal
                        oColumnaParametrica.CodigoPostal = true;
                        oColumnaObligatoria.CodigoPostal = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 15://15 Largo
                        oColumnaParametrica.Largo = true;
                        oColumnaObligatoria.Largo = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 16://16 Alto
                        oColumnaParametrica.Alto = true;
                        oColumnaObligatoria.Alto = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 17://17 Ancho
                        oColumnaParametrica.Ancho = true;
                        oColumnaObligatoria.Ancho = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 18://18 Observacion1
                        oColumnaParametrica.Observacion1 = true;
                        oColumnaObligatoria.Observacion1 = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 19://19 Observacion2
                        oColumnaParametrica.Observacion2 = true;
                        oColumnaObligatoria.Observacion2 = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 20://20 Contacto1Rut
                        oColumnaParametrica.Contacto1Rut = true;
                        oColumnaObligatoria.Contacto1Rut = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 21://21 Contacto1Nombre
                        oColumnaParametrica.Contacto1Nombre = true;
                        oColumnaObligatoria.Contacto1Nombre = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 22://22 Contacto1Telefono
                        oColumnaParametrica.Contacto1Telefono = true;
                        oColumnaObligatoria.Contacto1Telefono = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 23://23 Contacto2Rut
                        oColumnaParametrica.Contacto2Rut = true;
                        oColumnaObligatoria.Contacto2Rut = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 24://24 Contacto2Nombre
                        oColumnaParametrica.Contacto2Nombre = true;
                        oColumnaObligatoria.Contacto2Nombre = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 25://25 Contacto2Telefono
                        oColumnaParametrica.Contacto2Telefono = true;
                        oColumnaObligatoria.Contacto2Telefono = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 26://26 Contacto3Rut
                        oColumnaParametrica.Contacto3Rut = true;
                        oColumnaObligatoria.Contacto3Rut = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 27://27 Contacto3Nombre
                        oColumnaParametrica.Contacto3Nombre = true;
                        oColumnaObligatoria.Contacto3Nombre = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 28://28 Contacto3Telefono
                        oColumnaParametrica.Contacto3Telefono = true;
                        oColumnaObligatoria.Contacto3Telefono = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 29://29 Tipo Documento
                        oColumnaParametrica.TipoDocumento = true;
                        oColumnaObligatoria.TipoDocumento = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 30://30 N° Documento
                        oColumnaParametrica.NumeroDocumento = true;
                        oColumnaObligatoria.NumeroDocumento = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 31://31 Peso Volumétrico
                        oColumnaParametrica.PesoVolumetrico = true;
                        oColumnaObligatoria.PesoVolumetrico = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                    case 32://32 Contra Pago
                        oColumnaParametrica.ContraPago = true;
                        oColumnaObligatoria.ContraPago = oColumna.FL_PAR_EXC_OBLIGATORIA;
                        break;
                }                
            }

            oModel.oColumnas = oColumnaParametrica;
            oModel.oObligatoria = oColumnaObligatoria;
            oModel.GetTipoDocumento();

            oModel.GetNombreReferencia();

            return PartialView("_FormularioIngresoManual", oModel);
        }
        #endregion

        #region Ingreso Manual
        [Authorize]
        public ActionResult IngresoManual(string opt)
        {           
            var oPar = new LinqCLIDataContext();
            var oEmpresas = from m in oPar.TB_CLI_EMP_EMPRESAS
                            select m;


            //var datos = new ModelService().ObtenerEjemplo();
           
            
            var oModel = new IngresoMasivoModel()
            {                                            
                oItems = oEmpresas.ToList()
            };

            if (opt != null)
            {
                if (opt == "2")
                {
                    oModel.opt = "2";
                }
                else
                {
                    oModel.opt = "1";
                }
            }
            else
            {
                oModel.opt = "1";
            }


            List<SelectListItem> oServicios = new List<SelectListItem>();
            oServicios.Add(new SelectListItem { Text = "-- Seleccione --", Value = "" });

            oModel.oListaServicios = oServicios;

            
            return View(oModel);
        }               
        #endregion

        #region IngresoMasivoCarga
        public ActionResult IngresoMasivoCarga()
        {
            IngresoMasivoModel oModel = new IngresoMasivoModel();

            oModel = TempData["ModelCarga"] as IngresoMasivoModel;
            TempData["ModelCarga"] = oModel;
            
            return View(oModel);
        }
        #endregion

        #region ReadFileFromDatabase
        private IngresoMasivoModel ReadFileFromDatabase(int oId)
        {

            Stream oMs = null;
            string oType=null;

            var oDataContext = new LinqBD_FOLDataContext();
            var oArchivos = (from m in oDataContext.TB_FOL_UPL_UPLOAD                            
                            where m.PK_FOL_UPL_ID==oId                            
                            select m).Take(1);

            foreach (var ofile in oArchivos)
            {
                oMs = new MemoryStream(ofile.FL_FOL_UPL_FILE.ToArray());
                oType= ofile.FL_FOL_UPL_CONTENTTYPE;
            }
            var oMode = new IngresoMasivoModel
            {
                oStream=oMs,
                oType=oType
            };

            return oMode;
        }
        #endregion

        #region IngresoMasivoError
        public ActionResult IngresoMasivoError(IngresoMasivoModel oModel)
        {
            oModel = TempData["ListaErrores"] as IngresoMasivoModel;
            TempData["ListaErrores"] = oModel;

            if (oModel == null)
                return RedirectToAction("IngresoMasivo", "Pedido");
            else
                return View(oModel);

        }
        #endregion

        #region Función Truncate
        //TRUNCA LA CADENA DE TEXTO
        //oValor, Cadena de texto a Truncar
        //oLargo, Largo máximo a truncar, toma la posición 1 hasta el largo
        public string Truncate(string oValor, int oLargo)
        {
            if (oValor.Length <= oLargo)
                return oValor;
            else
            {
                return oValor.Substring(1, oLargo) + "...";
            }
        }
        #endregion

        #region IngresoMasivoCarga
        [HttpPost]
        public ActionResult IngresoMasivoCarga(FormCollection oForm, HttpPostedFileBase file1, IngresoMasivoModel oModel2)
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            List<ListaExcel> oResult = new List<ListaExcel>();
            List<ListaErrores> oListaErrores = new List<ListaErrores>();
            Stream oMs = null;
            decimal oIdUPL=0;
            if (file1 != null)
            {

                LinqBD_FOLDataContext oDC = new LinqBD_FOLDataContext();
                var oBinary = new byte[file1.ContentLength];
                string oFileName = file1.FileName;
                file1.InputStream.Read(oBinary, 0, file1.ContentLength);

                TB_FOL_UPL_UPLOAD oUpload = new TB_FOL_UPL_UPLOAD
                {
                    FL_FOL_UPL_FILE = oBinary,
                    FL_FOL_UPL_CONTENTTYPE = file1.ContentType,
                    FL_FOL_UPL_FILENAME = oFileName,
                    FL_FOL_UPL_SIZE=file1.ContentLength                    
                };

                oMs = new MemoryStream(oBinary);               
                oDC.TB_FOL_UPL_UPLOAD.InsertOnSubmit(oUpload);

                try
                {
                    oDC.SubmitChanges();

                    oIdUPL = oUpload.PK_FOL_UPL_ID;

                    var a = oUpload.PK_FOL_UPL_ID.ToString();

                    Session["FileName"] = a.ToString();
                    oResult = LeerExcel(oMs, file1.FileName, oModel2.Servicio);
                    
                    


                }
                catch 
                {
                    //SE DEBE AGREGAR PANTALLA DE ERROR
                    View();
                }


            }
            else
            {
                //SE DEBE AGREGAR PANTALLA DE ERROR
                //oResult="No ha subido nada";
            }

            if (oIdUPL != 0)
            {
                
                oListaErrores= ValidarModelo (oResult,oModel2.Servicio,oModel2.RutCliente);
            }

            if (oListaErrores.Count() == 0)
            {

                List<ListaExcel> oActualizado = new List<ListaExcel>();
                
                string oDireccion = "";

                if (oResult != null)
                {
                    foreach (var oFila in oResult)
                    {
                        ValidationController oValidacion = new ValidationController();
                        ValidationController.ResultValidarComuna oResultValidar = new ValidationController.ResultValidarComuna();
                        ValidationController.ResultValidarLocalidad oResultLocalidad = new ValidationController.ResultValidarLocalidad();
                        oResultValidar=oValidacion.ValidarComuna(oFila.Comuna);
                        int ResultLocalidadid = 0;
                        if (oFila.Localidad != "")
                        {
                            oResultLocalidad = oValidacion.ValidarLocalidad(oFila.Localidad, oResultValidar.ComunaId);
                            ResultLocalidadid = oResultLocalidad.LocalidadId;
                        }



                        ListaExcel oListaExcel = new ListaExcel();
                        
                        oListaExcel.Id =oFila.Id;
                        oListaExcel.Referencia =oFila.Referencia;
                        oListaExcel.Destinatario =oFila.Destinatario;
                        oListaExcel.Rut =oFila.Rut;
                        oListaExcel.ContactoEntrega =oFila.ContactoEntrega;
                        oListaExcel.Direccion =oFila.Direccion;
                        oListaExcel.Comuna =oResultValidar.Comuna;
                        oListaExcel.Bulto =oFila.Bulto;
                        oListaExcel.Peso =oFila.Peso;
                        oListaExcel.Telefono =oFila.Telefono;
                        oListaExcel.IdComuna =oResultValidar.ComunaId;
                        oListaExcel.DireccionNumero = oFila.DireccionNumero;
                        oListaExcel.Via = oFila.Via;
                        oListaExcel.CodigoPostal = oFila.CodigoPostal;
                        oListaExcel.Localidad = oFila.Localidad;
                        oListaExcel.idLocalidad = ResultLocalidadid;
                        oListaExcel.Largo = oFila.Largo;
                        oListaExcel.Ancho = oFila.Ancho;
                        oListaExcel.Alto = oFila.Alto;
                        oListaExcel.TipoDocumento = oFila.TipoDocumento;
                        oListaExcel.NumeroDocumento = oFila.NumeroDocumento;
                        oListaExcel.Contacto1Rut = oFila.Contacto1Rut;
                        oListaExcel.Contacto1Nombre = oFila.Contacto1Nombre;
                        oListaExcel.Contacto1Telefono = oFila.Contacto1Telefono;
                        oListaExcel.Contacto2Rut = oFila.Contacto2Rut;
                        oListaExcel.Contacto2Nombre = oFila.Contacto2Nombre;
                        oListaExcel.Contacto2Telefono = oFila.Contacto2Telefono;
                        oListaExcel.Contacto3Rut = oFila.Contacto3Rut;
                        oListaExcel.Contacto3Nombre = oFila.Contacto3Nombre;
                        oListaExcel.Contacto3Telefono = oFila.Contacto3Telefono;
                        oListaExcel.Observacion1 = oFila.Observacion1;
                        oListaExcel.Observacion2 = oFila.Observacion2;
                        oListaExcel.PesoVolumetrico = oFila.PesoVolumetrico;
                        oListaExcel.ContraPago = oFila.ContraPago;


                        if (Session["SucId"] == null)
                        {
                            var oVC = new ValidationController();
                            var oResult1=oVC.Sucursal(User.Identity.Name.ToString());
                            Session["SucName"] = oResult1.Nombre;
                            Session["SucName2"] = oResult1.Nombre;
                            Session["SucId"] = oResult1.id;
    
                        }

                        DateTime oTiempo = DateTime.Now;
                        if (ValidationController.IsNumeric(oFila.idLocalidad.ToString()))
                        {
                            BusquedaController oBusqueda = new BusquedaController();
                            BusquedaModels oModelBusqueda = new BusquedaModels();

                            oModelBusqueda.ComunaDestino = oResultValidar.ComunaId.ToString();
                            oModelBusqueda.LocalidadDestino = ResultLocalidadid.ToString();
                            oModelBusqueda.SucursalOrigen = oValidacion.GetSucursalIDfromActiveUser().ToString();
                            oModelBusqueda = oBusqueda.FunctionProcesarBusqueda(oModelBusqueda);
                            oTiempo = DateTime.Now.AddHours(oModelBusqueda.ListaTiempo.Sum(m => m.Horas));
                        }
                        else if (ValidationController.IsNumeric(oFila.Comuna.ToString()))
                        {
                            BusquedaController oBusqueda = new BusquedaController();
                            BusquedaModels oModelBusqueda = new BusquedaModels();

                            oModelBusqueda.ComunaDestino = oResultValidar.ComunaId.ToString();                            
                            oModelBusqueda.SucursalOrigen = oValidacion.GetSucursalIDfromActiveUser().ToString();
                            oModelBusqueda = oBusqueda.FunctionProcesarBusqueda(oModelBusqueda);
                            oTiempo = DateTime.Now.AddHours(oModelBusqueda.ListaTiempo.Sum(m => m.Horas));
                        }

                        oListaExcel.FechaEstimada = oTiempo;

                        //Determino la dirección


                        var oDatosCompleto = (from com in oPar.TB_PAR_COM_COMUNA
                                              join prv in oPar.TB_PAR_PRV_PROVINCIA on com.PK_PAR_PRV_ID equals prv.PK_PAR_PRV_ID
                                              join reg in oPar.TB_PAR_REG_REGION on prv.PK_PAR_REG_ID equals reg.PK_PAR_REG_ID
                                              where com.PK_PAR_COM_ID == oResultValidar.ComunaId
                                              select new
                                              {
                                                  com.FL_PAR_COM_NOMBRE,
                                                  prv.FL_PAR_PRV_NOMBRE,
                                                  reg.FL_PAR_REG_NOMBRE
                                              }).Single();

                        oDireccion = oListaExcel.Direccion + " " + oListaExcel.DireccionNumero + "," + oDatosCompleto.FL_PAR_COM_NOMBRE + "," + oDatosCompleto.FL_PAR_PRV_NOMBRE + "," + oDatosCompleto.FL_PAR_REG_NOMBRE;

                        //var oCoordenada = ValidationController.BuscarDireccion(oDireccion);

                        //oListaExcel.Latitud = oCoordenada.Latitud;
                        //oListaExcel.Longitud = oCoordenada.Longitud;

                        oListaExcel.Latitud = 0;
                        oListaExcel.Longitud = 0;
                        
                        //fin 

                        

                        oActualizado.Add(oListaExcel);

                    }
                }
                
                
                oModel2.Lista= oActualizado;
                oModel2.oStream = oMs;               

                TempData["ModelCarga"] = oModel2;

                

                var oDatos = from m in oPar.TB_PAR_EXC_EXCEL
                             where m.PK_PAR_SER_ID == Convert.ToInt32(oModel2.Servicio)
                             select m;

                oModel2.oColumnas = new int[33];

                foreach (var oFile in oDatos)
                {
                    oModel2.oColumnas[oFile.PK_PAR_COL_ID]=1;
                }

                oModel2.GetNombreReferencia();

                return View("IngresoMasivoCarga", oModel2);
            }
            else
            {
                oModel2.ListaErrores = oListaErrores;
                TempData["ListaErrores"] = oModel2;
                return RedirectToAction("IngresoMasivoError", oModel2);
            }
        }
        #endregion
        
        #region ValidarModelo
        private List<ListaErrores> ValidarModelo(List<ListaExcel> oDatos, string Servicio,string oRutCliente)
        {


            /*           
            
             * Las columnas son dinamicas, es decir, puede tener mas elementos en el tiempo, sin embargo,
             * se debe programar cada nueva columna, para que conserve su integridad en la base de datos.
             * ya que, al ser una carga de excel "dinamica", se debe almacenar la información en la columna correcta de la bd
             * para que cuando por ejemplo se consulte por un rut, en esa columna existan unicamente ruts.
                         
            */

            //Defino el Data Context de la base de Parámetros
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            //Consulto las columnas del servicio, cada servicio tiene definidas sus propias columnas al momento de realizar la carga
            var oColumnas = from m in oPar.TB_PAR_EXC_EXCEL
                            where m.PK_PAR_SER_ID == Convert.ToInt32(Servicio)
                            select m;

            //Aquí almacenare los errores detectados
            List<ListaErrores> oListaErrores = new List<ListaErrores>();

            int oTre = 0;

            int oCount=1;
            if (oDatos != null) //Por si llego a este punto sin una Lista para validar
            {
                ValidationController oValidation = new ValidationController(); //Objeto que tiene algunas funciones de validación
                foreach (var oFile in oDatos)
                {
                    List<string> oErroresFila = new List<string>(); //Almaceno los errores de la fila
                    foreach (var oColumna in oColumnas) //Por cada una de las columnas del Servicio
                    {                        
                        switch (oColumna.PK_PAR_COL_ID) 
                        {
                            case 1://1	Id
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA==true && (oFile.Id==null || oFile.Id==""))
                                    oErroresFila.Add (ErroresValidacion(1,"Id"));
                                if (oFile.Id != "")
                                {
                                    var oResult = oValidation.ExisteReferenciaV2(oRutCliente,oFile.Id, 1, Convert.ToInt32(Servicio));
                                    if (oResult.Ok == false)
                                    {
                                        oErroresFila.Add("N° Referencia 1:" + oResult.Mensaje);
                                    }
                                }
                                if (oDatos.Where(m => m.Id == oFile.Id).Count() > 1 && oFile.Id != "")
                                {
                                    oErroresFila.Add("N° Referencia 1:" + oFile.Id + " duplicada en Archivo ");
                                }
                                break;
                            case 2://2	Referencia
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA == true && (oFile.Referencia == null || oFile.Referencia == ""))
                                {
                                    oErroresFila.Add(ErroresValidacion(1, "Referencia"));
                                }
                                if (oFile.Referencia != "")
                                {                                    

                                    var oResult = oValidation.ExisteReferenciaV2(oRutCliente, oFile.Referencia,2, Convert.ToInt32(Servicio));
                                    if (oResult.Ok == false)
                                    {
                                        oErroresFila.Add("N° Referencia 2:" + oResult.Mensaje);
                                    }
                                }
                                if (oDatos.Where(m => m.Referencia == oFile.Referencia).Count() > 1 && oFile.Referencia!="")
                                {
                                    oErroresFila.Add("N° Referencia 2:" + oFile.Referencia + " duplicada en Archivo ");
                                }
                                break;
                            case 3://3	Destinatario
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA == true && (oFile.Destinatario == null || oFile.Destinatario == ""))
                                    oErroresFila.Add (ErroresValidacion(1,"Destinatario"));
                                break;
                            case 4:// 4	Rut
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA==true && (oFile.Rut==null || oFile.Rut==""))
                                    oErroresFila.Add (ErroresValidacion(1,"Rut"));
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA == true && (oFile.Rut != null && oFile.Rut != ""))
                                {
                                    var oRut = oValidation.ValidarRut(oFile.Rut);
                                    if (oRut != 1) //1: significa que esta correcto
                                    {
                                        oErroresFila.Add(oValidation.ErrorValidarRut(oRut));
                                    }
                                }
                                break;
                            case 5://5	Contacto Entrega
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA==true && (oFile.ContactoEntrega==null || oFile.ContactoEntrega==""))
                                    oErroresFila.Add (ErroresValidacion(1,"Contacto Entrega"));
                                break;
                            case 6://6	Dirección
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA==true && (oFile.Direccion==null || oFile.Direccion==""))
                                    oErroresFila.Add (ErroresValidacion(1,"Dirección"));
                                break;
                            case 7://7	Comuna
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA==true && (oFile.Comuna==null || oFile.Comuna==""))
                                    oErroresFila.Add (ErroresValidacion(1,"Comuna"));
                                var oResultComuna = oValidation.ValidarComuna(oFile.Comuna);
                                if (oResultComuna.iError != 1)//1: significa que esta correcto
                                {                                    
                                    oErroresFila.Add (oFile.Comuna + ": " + oResultComuna.oError);
                                }
                                if (oValidation.ValidarCoberturaComuna(oResultComuna.ComunaId) != 0 && oFile.Localidad == "")
                                    oErroresFila.Add("Comuna " + oFile.Comuna + ": Sin Cobertura");
                                
                                if (oFile.Localidad != "" && oResultComuna.iError==1) //Localidad trae valor y Comuna esta validada
                                {
                                    var oResultLocalidad = oValidation.ValidarLocalidad(oFile.Localidad,oResultComuna.ComunaId);
                                    if (oResultLocalidad.iError != 1)//1: significa que esta correcto
                                    {                                        
                                        oErroresFila.Add(oFile.Localidad + ": " + oResultLocalidad.oError);
                                    }
                                    if (oResultLocalidad.iError == 1)
                                    {
                                        if(oValidation.ValidarCoberturaLocalidad(oResultLocalidad.LocalidadId) != 0)
                                            oErroresFila.Add("Localidad " + oFile.Localidad + ": Sin Cobertura");
                                    }
                                }
                                break;
                            case 8://8	Bulto
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA==true && (oFile.Bulto==null || oFile.Bulto==""))
                                    oErroresFila.Add (ErroresValidacion(1,"Bulto"));
                                break;
                            case 9://9	Peso
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA==true && (oFile.Peso==null || oFile.Peso==""))
                                    oErroresFila.Add (ErroresValidacion(1,"Peso"));
                                break;
                            case 10://10	Fono
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA==true && (oFile.Telefono==null || oFile.Telefono==""))
                                    oErroresFila.Add (ErroresValidacion(1,"Fono"));
                                break;
                            case 11://11	Número Direccion
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA == true && (oFile.DireccionNumero == null || oFile.DireccionNumero== ""))
                                    oErroresFila.Add(ErroresValidacion(1, "Número Dirección"));
                                break;
                            case 12://12	Localidad
                                //ESTA EN LA COMUNA CASE:7 LA VALIDACION DE INTEGRIDAD
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA == true && (oFile.Localidad == null || oFile.Localidad == ""))
                                    oErroresFila.Add(ErroresValidacion(1, "Localidad"));
                                break;
                            case 13://13	Vía
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA == true && (oFile.Via == null || oFile.Via == ""))
                                    oErroresFila.Add(ErroresValidacion(1, "Vía"));
                                if (oFile.Via != "")
                                {
                                    if (oFile.Via != "A" && oFile.Via != "T")
                                    {
                                        oErroresFila.Add(ErroresValidacion(2,oFile.Via));
                                    }
                                }
                                break;
                            case 14://14	Código Postal
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA == true && (oFile.CodigoPostal == null || oFile.CodigoPostal == ""))
                                    oErroresFila.Add(ErroresValidacion(1, "Código Postal"));
                                break;
                            case 15://15	Largo
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA == true && (oFile.Largo == null || oFile.Largo == ""))
                                    oErroresFila.Add(ErroresValidacion(1, "Largo"));
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA == true && !oValidation.isDecimal(oFile.Largo.ToString()))
                                    oErroresFila.Add("Largo, solo admite valores decimales separados por coma");
                                break;
                            case 16://16	Alto
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA == true && (oFile.Alto == null || oFile.Alto == ""))
                                    oErroresFila.Add(ErroresValidacion(1, "Alto"));
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA == true && !oValidation.isDecimal(oFile.Alto.ToString()))
                                    oErroresFila.Add("Alto, solo admite valores decimales separados por coma");
                                break;
                            case 17://17	Ancho
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA == true && (oFile.Ancho == null || oFile.Ancho == ""))
                                    oErroresFila.Add(ErroresValidacion(1, "Ancho"));
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA == true && !oValidation.isDecimal(oFile.Ancho.ToString()))
                                    oErroresFila.Add("Ancho, solo admite valores decimales separados por coma");
                                break;
                            case 18://18	Observacion 1
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA == true && (oFile.Observacion1 == null || oFile.Observacion1 == ""))
                                    oErroresFila.Add(ErroresValidacion(1, "Observacion1"));
                                break;
                            case 19://19 Observacion 2
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA == true && (oFile.Observacion2 == null || oFile.Observacion2 == ""))
                                    oErroresFila.Add(ErroresValidacion(1, "Observacion2"));
                                break;
                            case 20://20	Contacto 1 Rut
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA == true && (oFile.Contacto1Rut == null || oFile.Contacto1Rut == ""))
                                    oErroresFila.Add(ErroresValidacion(1, "Contacto 1, Rut"));
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA == true && (oFile.Contacto1Rut != null && oFile.Contacto1Rut != ""))
                                {
                                    var oRut = oValidation.ValidarRut(oFile.Contacto1Rut);
                                    if (oRut != 1) //1: significa que esta correcto
                                    {
                                        oErroresFila.Add(oValidation.ErrorValidarRut(oRut));
                                    }
                                }
                                break;
                            case 21://21	Contacto 1 Nombre
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA == true && (oFile.Contacto1Nombre == null || oFile.Contacto1Nombre == ""))
                                    oErroresFila.Add(ErroresValidacion(1, "Contacto 1, Nombre"));
                                break;
                            case 22://22	Contacto 1 Telefono
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA == true && (oFile.Contacto1Telefono == null || oFile.Contacto1Telefono == ""))
                                    oErroresFila.Add(ErroresValidacion(1, "Contacto 1, Telefono"));
                                break;
                            case 23://23	Contacto 2 Rut
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA == true && (oFile.Contacto2Rut == null || oFile.Contacto2Rut == ""))
                                    oErroresFila.Add(ErroresValidacion(1, "Contacto 2, Rut"));
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA == true && (oFile.Contacto2Rut != null && oFile.Contacto2Rut != ""))
                                {
                                    var oRut = oValidation.ValidarRut(oFile.Contacto2Rut);
                                    if (oRut != 1) //1: significa que esta correcto
                                    {
                                        oErroresFila.Add(oValidation.ErrorValidarRut(oRut));
                                    }
                                }
                                break;
                            case 24://24	Contacto 2 Nombre
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA == true && (oFile.Contacto2Nombre == null || oFile.Contacto2Nombre == ""))
                                    oErroresFila.Add(ErroresValidacion(1, "Contacto 2, Nombre"));
                                break;
                            case 25://25	Contacto 2 Telefono
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA == true && (oFile.Contacto2Telefono == null || oFile.Contacto2Telefono == ""))
                                    oErroresFila.Add(ErroresValidacion(1, "Contacto 2, Telefono"));
                                break;
                            case 26://26	Contacto 3 Rut
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA == true && (oFile.Contacto3Rut == null || oFile.Contacto3Rut == ""))
                                    oErroresFila.Add(ErroresValidacion(1, "Contacto 3, Rut"));
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA == true && (oFile.Contacto3Rut != null && oFile.Contacto3Rut != ""))
                                {
                                    var oRut = oValidation.ValidarRut(oFile.Contacto3Rut);
                                    if (oRut != 1) //1: significa que esta correcto
                                    {
                                        oErroresFila.Add(oValidation.ErrorValidarRut(oRut));
                                    }
                                }
                                break;
                            case 27://27	Contacto 3 Nombre
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA == true && (oFile.Contacto3Nombre == null || oFile.Contacto3Nombre == ""))
                                    oErroresFila.Add(ErroresValidacion(1, "Contacto 3, Nombre"));
                                break;
                            case 28://28	Contacto 3 Telefono
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA == true && (oFile.Contacto3Telefono == null || oFile.Contacto3Telefono == ""))
                                    oErroresFila.Add(ErroresValidacion(1, "Contacto 3, Telefono"));
                                break;
                            case 29://29	Tipo Documento
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA == true && (oFile.TipoDocumento == null || oFile.TipoDocumento == ""))
                                    oErroresFila.Add(ErroresValidacion(1, "Tipo Documento"));
                                else
                                {
                                    var oExisteTipo = (from coc in oFol.PR_FOL_COC_BUSCAR_ORIGEN(Convert.ToInt32(oFile.TipoDocumento), Convert.ToInt32(oRutCliente.Substring(0, oRutCliente.Length - 2)))
                                                      select coc).ToList();
                                    if (oExisteTipo.Count() == 0)
                                    {
                                        oErroresFila.Add("Tipo de Documento no existe para el cliente.");
                                    }
                                    else
                                    {
                                        oTre = Convert.ToInt32(oExisteTipo[0].PK_PAR_TRE_ID);
                                    }
                                }
                                break;
                            case 30://30	N° Documento
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA == true && (oFile.NumeroDocumento == null || oFile.NumeroDocumento == ""))
                                    oErroresFila.Add(ErroresValidacion(1, "N° Documento"));
                                else
                                {
                                    if (oTre == 0)
                                    {
                                        oErroresFila.Add("Debe existir un tipo para consultar por el N° de Documento");
                                    }
                                    else
                                    {
                                        if (oTre == 1)
                                        {
                                            var oExisteDOA = (from coc in oFol.PR_FOL_COC_VALIDA_TRE_1(Convert.ToInt32(oRutCliente.Substring(0, oRutCliente.Length - 2)),Convert.ToInt32(oFile.TipoDocumento),Convert.ToDecimal(oFile.NumeroDocumento))
                                                               select coc).Single();
                                            if (oExisteDOA.Column1 > 0)
                                            {
                                                oErroresFila.Add("N° de Documento ya existe");
                                            }
                                        }
                                        else
                                        {
                                            var oExisteREA = (from coc in oFol.PR_FOL_COC_VALIDA_TRE_2(Convert.ToInt32(oRutCliente.Substring(0, oRutCliente.Length - 2)), Convert.ToInt32(oFile.TipoDocumento),oFile.NumeroDocumento)
                                                               select coc).Single();
                                            if (oExisteREA.Column1 > 0)
                                            {
                                                oErroresFila.Add("N° de Documento ya existe");
                                            }

                                        }
                                    }
                                }
                                break;
                            case 31://31    Peso Volumetrico
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA == true && (oFile.PesoVolumetrico == null || oFile.PesoVolumetrico == ""))
                                    oErroresFila.Add(ErroresValidacion(1, "Peso Volumétrico"));
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA == true && !oValidation.isDecimal(oFile.PesoVolumetrico.ToString()))
                                    oErroresFila.Add("Peso Volumétrico, solo admite valores decimales separados por coma");
                                break;
                            case 32://31    ContraPago
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA == true && (oFile.ContraPago == null || oFile.ContraPago == ""))
                                    oErroresFila.Add(ErroresValidacion(1, "Contra Pago"));
                                if (oColumna.FL_PAR_EXC_OBLIGATORIA == true && !oValidation.IsNumeric2(oFile.ContraPago.ToString()))
                                    oErroresFila.Add("Contra Pago, solo admite valores enteros");
                                break;
                        }                                                                      
                    }
                    var oTextoErrores = ""; 
                    if (oErroresFila.Count() > 0)
                    {
                        foreach (var oError in oErroresFila)
                        {
                            if (oTextoErrores != "")
                                oTextoErrores += ", ";
                            oTextoErrores += oError;
                        }
                        oListaErrores.Add(new ListaErrores(oCount+1, oTextoErrores));
                    }                     
                    oCount++;
                }
            }

            return oListaErrores;
        }
        
        public string ErroresValidacion(int oError, string NombreColumna)
        {
            switch (oError)
            {
                case 1:                    
                    return (string.Format("El campo {0} es obligatorio ",NombreColumna));     
                case 2:
                    return (string.Format("El campo vía permite valores T:Terrestre y A:Aéreo, el valor ingresado fue {0} ", NombreColumna));     
                default:
                    return ("Error Desconocido");                    
            }
        }
        #endregion

        #region Ingreso Masivo
        [Authorize]
        public ActionResult IngresoMasivo()
        {

            var oDataContext = new LinqCLIDataContext();
            var oEmpresas = from m in oDataContext.TB_CLI_EMP_EMPRESAS
                            select m;


            List<SelectListItem> oServicios=new List<SelectListItem>();
            oServicios.Add(new SelectListItem { Text="-- Seleccione --",Value=""});

            var oIngreso = new IngresoMasivoModel()
            {
                oItems = oEmpresas.ToList(),
                oListaServicios =oServicios
            };

            

            Session["FileName"] = null;
            ViewBag.title = "Ingreso Masivo";

            return View(oIngreso);
           
        }       
        
        #endregion

        #region Ingreso Retiro Programado
        public ActionResult IngresoRetiroProgramado()
        {
            var oDataContext = new LinqCLIDataContext();
            var oEmpresas = from m in oDataContext.TB_CLI_EMP_EMPRESAS
                            select m;


            //var datos = new ModelService().ObtenerEjemplo();

            var model = new IngresoMasivoModel()
            {
                oItems = oEmpresas.ToList()
            };

            List<SelectListItem> oServicios = new List<SelectListItem>();
            oServicios.Add(new SelectListItem { Text = "-- Seleccione --", Value = "" });

            model.oListaServicios = oServicios;

            return View(model);
        }
        #endregion

        #region Anular Pedido
        public ActionResult AnularPedido()
        {
            AnularPedidoModels oModel = new AnularPedidoModels();
            return View(oModel);
        }
        #endregion 

        #region BuscarAnular
        [HttpPost]
        public ActionResult BuscarAnular(AnularPedidoModels oModel)
        {
            ValidationController oValidation = new ValidationController();
            var oResult = oValidation.TransformCodigoGenericoOTPOTD(oModel.OT);
            oModel.GetDatosAnular(oResult.OTP,oResult.OTD);
            oModel.GetBultos(oResult.OTP, oResult.OTD);
            return PartialView("_CargaDatosAnular", oModel);
        }
        #endregion

        #region CompletarAnularOT
        [HttpPost][Authorize]
        public bool CompletarAnularOT(decimal OTP, decimal OTD)
        {
            try
            {
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                ValidationController oValidation = new ValidationController();
                oFol.PR_FOL_ANULAR_OT(OTP, OTD, oValidation.GetSucursalIDfromActiveUser(), oValidation.GetRutActiveUser());
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region vucServicio
        [HttpGet]
        public ActionResult vucServicio(string Rut)
        {
            var oVal = false;
            List<SelectListItem> oList = new List<SelectListItem>();
            IngresoMasivoModel oModel = new IngresoMasivoModel();
            if (Rut != "" && Rut != null && Rut.Length>=3)
            {
                if (ValidationController.IsNumeric(Rut.Substring(0, Rut.Length - 2)) == true)
                {                  
                    LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
                    var oDatos = from SER in oPar.TB_PAR_SER_SERVICIO
                                 join EMS in oPar.TB_PAR_EMS_EMPRESA_SERVICIO on SER.PK_PAR_SER_ID equals EMS.PK_PAR_SER_ID
                                 where EMS.PK_CLI_EMP_RUT == Convert.ToInt32(Rut.Substring(0, Rut.Length - 2))
                                 orderby SER.FL_PAR_SER_NOMBRE
                                 select new SelectListItem
                                 {
                                     Text = SER.FL_PAR_SER_NOMBRE.ToUpper(),
                                     Value = SER.PK_PAR_SER_ID.ToString()
                                 };

                    oList.Add(new SelectListItem { Text = "-- Seleccione --", Value = "", Selected = true });
                    oList.AddRange(oDatos.ToList());
                                       

                    oModel.oListaServicios = oList;
                    oVal = true;
                }
            }
            if (oVal == false)
            {
                
                oList.Add(new SelectListItem { Text = "-- Seleccione --", Value = "" });
                oModel.oListaServicios = oList;
            }
            return PartialView(oModel);
        }

        #endregion        

        #region Guardar Ingreso Masivo
        [Authorize]
        public string GuardarIngresoMasivo()
        {
            ValidationController oValidation = new ValidationController();
            IngresoMasivoModel oModel = new IngresoMasivoModel();
            oModel = TempData["ModelCarga"] as IngresoMasivoModel;
            TempData["ModelCarga"] = oModel;

           

            //List<TB_FOL_OTD_OT_DETALLE> oDetalle = new List<TB_FOL_OTD_OT_DETALLE>();
            System.Data.Linq.EntitySet<TB_FOL_OTD_OT_DETALLE> oDetalle = new System.Data.Linq.EntitySet<TB_FOL_OTD_OT_DETALLE>();

            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            int oCont=1;
            int oBulto=0;
            float oPeso=0;
            float oAlto = 0;
            float oAncho = 0;
            float oLargo = 0;
            float oPesoVolumetrico = 0;
            decimal ContraPago = 0;

            DateTime oFechaCreacion= DateTime.Now;

            int oSucursal = oValidation.GetSucursalIDfromActiveUser();
            int oRut = oValidation.GetRutActiveUser();

            var oResultTipo = oValidation.Sucursal(oRut.ToString() + "-1");
            int EstadoFinal = 1;//EN BODEGA SUCURSAL
            if (oResultTipo.Tipo == 2)
                EstadoFinal = 20;//EN RUTA DESDE CLIENTE A SUCURSAL

            foreach (var oFila in oModel.Lista)
            {
                if (ValidationController.IsNumeric(oFila.Bulto))
                    oBulto=Convert.ToInt32(oFila.Bulto);
                else
                {
                    oBulto=0;
                }


                //Peso
                if (oValidation.isDecimal(oFila.Peso))
                    float.TryParse(oFila.Peso, out oPeso);
                else
                {
                    oPeso = 0;
                }

                //Peso Volumétrico
                if (oValidation.isDecimal(oFila.PesoVolumetrico))
                    float.TryParse(oFila.PesoVolumetrico, out oPesoVolumetrico);
                else
                {
                    oPesoVolumetrico = 0;
                }

                //Alto
                if (oValidation.isDecimal(oFila.Alto))
                    float.TryParse(oFila.Alto, out oAlto);
                else
                {
                    oAlto = 0;
                }

                //Ancho
                if (oValidation.isDecimal(oFila.Ancho))
                    float.TryParse(oFila.Ancho, out oAncho);
                else
                {
                    oAncho = 0;
                }

                //Largo
                if (oValidation.isDecimal(oFila.Largo))
                    float.TryParse(oFila.Largo, out oLargo);
                else
                {
                    oLargo = 0;
                }

                //Contrapago
                if (oValidation.isDecimal(oFila.ContraPago))
                    Decimal.TryParse(oFila.ContraPago, out ContraPago);
                else
                {
                    ContraPago = 0;
                }

                string Via ="T";
                if (oFila.Via != "" && oFila.Via != null)
                {
                    Via = oFila.Via;
                }

                
                int oSwtObs = 0;
                int oSwtCon = 0;

                System.Data.Linq.EntitySet<TB_FOL_PIN_PESO_INFORMADO> oPIN = new System.Data.Linq.EntitySet<TB_FOL_PIN_PESO_INFORMADO>();
                System.Data.Linq.EntitySet<TB_FOL_OCA_OBSERVACION_CARGA> oOCA = new System.Data.Linq.EntitySet<TB_FOL_OCA_OBSERVACION_CARGA>();
                System.Data.Linq.EntitySet<TB_FOL_CEN_CONTACTO_ENTREGA> oCEN = new System.Data.Linq.EntitySet<TB_FOL_CEN_CONTACTO_ENTREGA>();
                System.Data.Linq.EntitySet<TB_FOL_RCL_REFERENCIA_CLIENTE> oRCL = new System.Data.Linq.EntitySet<TB_FOL_RCL_REFERENCIA_CLIENTE>();

                if (oFila.Observacion1 != null && oFila.Observacion1 != "")
                {
                    oOCA.Add(new TB_FOL_OCA_OBSERVACION_CARGA
                    {
                        FL_FOL_OCA_OBSERVACION = oFila.Observacion1
                    });
                    oSwtObs = 1;
                }

                if (oFila.Observacion2 != null && oFila.Observacion2 != "")
                {
                    oOCA.Add(new TB_FOL_OCA_OBSERVACION_CARGA
                    {
                        FL_FOL_OCA_OBSERVACION = oFila.Observacion2
                    });
                    oSwtObs = 1;
                }

                if ((oFila.Contacto1Rut != null && oFila.Contacto1Rut != "") || (oFila.Contacto1Nombre != null && oFila.Contacto1Nombre != "") || (oFila.Contacto1Telefono != null && oFila.Contacto1Telefono != ""))
                {
                    oCEN.Add(new TB_FOL_CEN_CONTACTO_ENTREGA
                    {
                        FL_FOL_CEN_NOMBRE = oFila.Contacto1Nombre,
                        FL_FOL_CEN_RUT = oFila.Contacto1Rut,
                        FL_FOL_CEN_TELEFONO = oFila.Contacto1Telefono
                    });
                    oSwtCon = 1;
                }
                if ((oFila.Contacto2Rut != null && oFila.Contacto2Rut != "") || (oFila.Contacto2Nombre != null && oFila.Contacto2Nombre != "") || (oFila.Contacto2Telefono != null && oFila.Contacto2Telefono != ""))
                {
                    oCEN.Add(new TB_FOL_CEN_CONTACTO_ENTREGA
                    {
                        FL_FOL_CEN_NOMBRE = oFila.Contacto2Nombre,
                        FL_FOL_CEN_RUT = oFila.Contacto2Rut,
                        FL_FOL_CEN_TELEFONO = oFila.Contacto2Telefono
                    });
                    oSwtCon = 1;
                }
                if ((oFila.Contacto3Rut != null && oFila.Contacto3Rut != "") || (oFila.Contacto3Nombre != null && oFila.Contacto3Nombre != "") || (oFila.Contacto3Telefono != null && oFila.Contacto3Telefono != ""))
                {
                    oCEN.Add(new TB_FOL_CEN_CONTACTO_ENTREGA
                    {
                        FL_FOL_CEN_NOMBRE = oFila.Contacto3Nombre,
                        FL_FOL_CEN_RUT = oFila.Contacto3Rut,
                        FL_FOL_CEN_TELEFONO = oFila.Contacto3Telefono
                    });
                    oSwtCon = 1;
                }

                oPIN.Add(new TB_FOL_PIN_PESO_INFORMADO { 
                    FL_FOL_PIN_PESO=oPeso,
                    FL_FOL_PIN_PESOVOLUMETRICO=oPesoVolumetrico
                });
                              
                TB_FOL_OTD_OT_DETALLE oOTD = new TB_FOL_OTD_OT_DETALLE{
                    PK_FOL_OTD_ID=oCont,
                    FL_FOL_OTD_REFERENCIA1 =oFila.Id.Trim(),
                    FL_FOL_OTD_REFERENCIA2=oFila.Referencia.Trim(),                    
                    FL_FOL_OTD_BULTO=oBulto,
                    FL_FOL_OTD_DESTINATARIO_DIRECCION=oFila.Direccion,
                    FL_FOL_OTD_DESTINATARIO_NOMBRE=oFila.Destinatario,
                    FL_FOL_OTD_DESTINATARIO_NUMERO=oFila.DireccionNumero,
                    FL_FOL_OTD_DESTINATARIO_RUT=oFila.Rut,
                    FL_FOL_OTD_TELEFONO=oFila.Telefono,
                    PK_PAR_COM_ID=oFila.IdComuna,
                    FL_FOL_OTD_CODIGO_POSTAL=oFila.CodigoPostal,
                    FL_FOL_OTD_VIA = Via,//Cambie formula Vía, para aquellos pedidos sin vía obligatorio.
                    PK_PAR_LOC_ID=oFila.idLocalidad,
                    FL_FOL_OTD_CONTACTO_ENTREGA=oFila.ContactoEntrega,
                    FL_FOL_OTD_PESO=oPeso,
                    FL_FOL_OTD_FECHA_ESTIMADA=oFila.FechaEstimada,
                    PK_FOL_EST_ID = EstadoFinal,
                    PK_PAR_SUC_ID=oValidation.GetSucursalIDfromActiveUser(),
                    FL_FOL_OTD_ALTO=oAlto,
                    FL_FOL_OTD_ANCHO=oAncho,
                    FL_FOL_OTD_LARGO=oLargo,
                    PK_PAR_USU_RUT=oValidation.GetRutActiveUser(),
                    TB_FOL_PIN_PESO_INFORMADO=oPIN
                };

                if (oSwtObs == 1) //Si hay observación la agrego
                {
                    oOTD.TB_FOL_OCA_OBSERVACION_CARGA = oOCA;
                }
                if (oSwtCon == 1) //Si hay Contacto de entrega lo agrego
                {
                    oOTD.TB_FOL_CEN_CONTACTO_ENTREGA = oCEN;
                }
                

                //REF 1
                System.Data.Linq.EntitySet<TB_FOL_DOA_DOC_ASOCIADO> oDOC = new System.Data.Linq.EntitySet<TB_FOL_DOA_DOC_ASOCIADO>();
                System.Data.Linq.EntitySet<TB_FOL_REA_REF_ASOCIADO> oREA = new System.Data.Linq.EntitySet<TB_FOL_REA_REF_ASOCIADO>();
                if (oOTD.FL_FOL_OTD_REFERENCIA1 != string.Empty && oOTD.FL_FOL_OTD_REFERENCIA1 != null)
                {
                    if (oOTD.FL_FOL_OTD_REFERENCIA1.Trim().Count()>0)
                    {
                        LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
                        var oRef = (from res in oPar.TB_PAR_RES_REFERENCIA_SERVICIO
                                    join refe in oPar.TB_PAR_REF_REFERENCIA on res.PK_PAR_REF_ID equals refe.PK_PAR_REF_ID
                                    join tdo in oPar.TB_PAR_TDO_TIPO_DOCUMENTO on res.PK_PAR_TDO_ID equals tdo.PK_PAR_TDO_ID
                                    where
                                     res.PK_PAR_SER_ID == Convert.ToInt32(oModel.Servicio)
                                     && refe.PK_PAR_REF_ID == 1//REF 1
                                    select new { res, refe,tdo }).Single();
                        if (oRef.tdo.PK_PAR_TRE_ID == 1 && oValidation.isDecimal(oOTD.FL_FOL_OTD_REFERENCIA1)==true)//Es tipo Documento, se puede asociar.
                        {
                            oDOC.Add(new TB_FOL_DOA_DOC_ASOCIADO
                            {
                                PK_PAR_TDO_ID = oRef.res.PK_PAR_TDO_ID,
                                PK_CLI_EMP_RUT = Convert.ToInt32(oModel.RutCliente.Substring(0, oModel.RutCliente.Length - 2)),
                                PK_FOL_DOA_NUMERO = Convert.ToDecimal(oOTD.FL_FOL_OTD_REFERENCIA1),
                                PK_PAR_SUC_ID = oSucursal,
                                PK_PAR_USU_RUT = oRut,
                                FL_FOL_DOA_FECHA = DateTime.Now,
                                FL_FOL_DOA_CONTRAPAGO=ContraPago
                            });
                            ContraPago = 0;
                        }
                        else //Es tipo Referencia, se agrega a referencias
                        {
                            oREA.Add(new TB_FOL_REA_REF_ASOCIADO
                            {
                                PK_PAR_TDO_ID = oRef.res.PK_PAR_TDO_ID,
                                PK_CLI_EMP_RUT = Convert.ToInt32(oModel.RutCliente.Substring(0, oModel.RutCliente.Length - 2)),
                                PK_FOL_REA_NUMERO = oOTD.FL_FOL_OTD_REFERENCIA1,
                                PK_PAR_SUC_ID = oSucursal,
                                PK_PAR_USU_RUT = oRut,
                                FL_FOL_REA_FECHA = DateTime.Now
                            });
                        }
                    }
                }
                if (oOTD.FL_FOL_OTD_REFERENCIA2 != string.Empty && oOTD.FL_FOL_OTD_REFERENCIA2 != null)
                {
                    if (oOTD.FL_FOL_OTD_REFERENCIA2.Trim().Count()>0)
                    {
                        LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
                        var oRef = (from res in oPar.TB_PAR_RES_REFERENCIA_SERVICIO
                                    join refe in oPar.TB_PAR_REF_REFERENCIA on res.PK_PAR_REF_ID equals refe.PK_PAR_REF_ID
                                    join tdo in oPar.TB_PAR_TDO_TIPO_DOCUMENTO on res.PK_PAR_TDO_ID equals tdo.PK_PAR_TDO_ID
                                    where
                                     res.PK_PAR_SER_ID == Convert.ToInt32(oModel.Servicio)
                                     && refe.PK_PAR_REF_ID == 2//REF 2
                                    select new { res, refe,tdo }).Single();
                        if (oRef.tdo.PK_PAR_TRE_ID == 1 && oValidation.isDecimal(oOTD.FL_FOL_OTD_REFERENCIA2) == true)//Es tipo Documento, se puede asociar.
                        {
                            oDOC.Add(new TB_FOL_DOA_DOC_ASOCIADO
                            {
                                PK_PAR_TDO_ID = oRef.res.PK_PAR_TDO_ID,
                                PK_CLI_EMP_RUT = Convert.ToInt32(oModel.RutCliente.Substring(0, oModel.RutCliente.Length - 2)),
                                PK_FOL_DOA_NUMERO = Convert.ToDecimal(oOTD.FL_FOL_OTD_REFERENCIA2),
                                PK_PAR_SUC_ID = oSucursal,
                                PK_PAR_USU_RUT = oRut,
                                FL_FOL_DOA_FECHA = DateTime.Now,
                                FL_FOL_DOA_CONTRAPAGO=ContraPago
                            });
                            ContraPago = 0;
                        }
                        else //Es tipo Referencia, se agrega a referencias
                        {
                            oREA.Add(new TB_FOL_REA_REF_ASOCIADO
                            {
                                PK_PAR_TDO_ID = oRef.res.PK_PAR_TDO_ID,
                                PK_CLI_EMP_RUT = Convert.ToInt32(oModel.RutCliente.Substring(0, oModel.RutCliente.Length - 2)),
                                PK_FOL_REA_NUMERO = oOTD.FL_FOL_OTD_REFERENCIA2,
                                PK_PAR_SUC_ID = oSucursal,
                                PK_PAR_USU_RUT = oRut,
                                FL_FOL_REA_FECHA = DateTime.Now
                            });
                        }
                    }
                }

                //TIPO DOCUMENTOS DEFINIDOS POR EL CLIENTE
                if (oFila.TipoDocumento != null && oFila.TipoDocumento != "" && oFila.NumeroDocumento != null && oFila.NumeroDocumento != "")
                {
                    var oCOC = (from pr in oFol.PR_FOL_COC_BUSCAR_ORIGEN(Convert.ToInt32(oFila.TipoDocumento), Convert.ToInt32(oModel.RutCliente.Substring(0, oModel.RutCliente.Length - 2)))
                                select pr).ToList();

                    if (oCOC.Count() > 0)
                    {
                        var oElCOC = oCOC.ToList()[0];
                        if (oElCOC.PK_PAR_TRE_ID == 1)//Es tipo Documento, se puede asociar.
                        {
                            oDOC.Add(new TB_FOL_DOA_DOC_ASOCIADO
                            {
                                PK_PAR_TDO_ID = oElCOC.PK_PAR_TDO_ID,
                                PK_CLI_EMP_RUT = Convert.ToInt32(oModel.RutCliente.Substring(0, oModel.RutCliente.Length - 2)),
                                PK_FOL_DOA_NUMERO = Convert.ToDecimal(oFila.NumeroDocumento),
                                PK_PAR_SUC_ID = oSucursal,
                                PK_PAR_USU_RUT = oRut,
                                FL_FOL_DOA_FECHA = DateTime.Now,
                                FL_FOL_DOA_CONTRAPAGO=ContraPago
                            });
                            ContraPago = 0;
                            
                        }
                        else //Es tipo Referencia, se agrega a referencias
                        {
                            oREA.Add(new TB_FOL_REA_REF_ASOCIADO
                            {
                                PK_PAR_TDO_ID = oElCOC.PK_PAR_TDO_ID,
                                PK_CLI_EMP_RUT = Convert.ToInt32(oModel.RutCliente.Substring(0, oModel.RutCliente.Length - 2)),
                                PK_FOL_REA_NUMERO = oFila.NumeroDocumento,
                                PK_PAR_SUC_ID = oSucursal,
                                PK_PAR_USU_RUT = oRut,
                                FL_FOL_REA_FECHA = DateTime.Now
                            });
                        }

                        oRCL.Add(new TB_FOL_RCL_REFERENCIA_CLIENTE
                        {
                            FL_FOL_RCL_TIPO = Convert.ToInt32(oFila.TipoDocumento),
                            FL_FOL_RCL_NUMERO = oFila.NumeroDocumento
                        });

                        oOTD.TB_FOL_RCL_REFERENCIA_CLIENTE = oRCL;

                    }
                }


                oOTD.TB_FOL_DOA_DOC_ASOCIADO = oDOC;
                oOTD.TB_FOL_REA_REF_ASOCIADO = oREA;

                oDetalle.Add(oOTD);
            oCont++;
            }

            


            TB_FOL_OTP_OT_PRINCIPAL oPrincipal = new TB_FOL_OTP_OT_PRINCIPAL();
            oPrincipal.PK_CLI_EMP_RUT = Convert.ToInt32(oModel.RutCliente.Substring(0, oModel.RutCliente.Length - 2));
            oPrincipal.PK_PAR_SER_ID=Convert.ToInt32(oModel.Servicio);
            oPrincipal.TB_FOL_OTD_OT_DETALLE = oDetalle;
            oPrincipal.FL_FOL_OTP_FECHA_CREACION = oFechaCreacion;
            oPrincipal.PK_FOL_EST_ID = EstadoFinal;//EN BODEGA CLIENTE   
            oPrincipal.PK_PAR_SUC_ID = oSucursal;
            oPrincipal.PK_PAR_USU_RUT = oRut;
            oPrincipal.PK_FOL_TOT_ID = 1;
            oFol.TB_FOL_OTP_OT_PRINCIPAL.InsertOnSubmit(oPrincipal);            
            

            try
            {
                oFol.SubmitChanges();

                VerOTModel oVerModel = new VerOTModel();

                oVerModel.Rut = Convert.ToInt32(oModel.RutCliente.Substring(0, oModel.RutCliente.Length - 2));
                oVerModel.OTPrincipal = Convert.ToInt32(oPrincipal.PK_FOL_OTP_ID);
                oVerModel.oColumnas = oModel.oColumnas;
                TempData["ModelCarga"] = null;
                TempData["ModelView"] = oVerModel;
                return ("1");
            }
            catch(Exception e)
            {
                return "Error: " + e.Message;
            }           
        }
        #endregion

        #region Ver Ingreso Masivo
       
        public ActionResult VerIngresoMasivo()
        {
            VerOTModel oModel = TempData["ModelView"] as VerOTModel;
            

            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();


            var oResult = (from otp in oFol.TB_FOL_OTP_OT_PRINCIPAL
                           where otp.PK_FOL_OTP_ID == oModel.OTPrincipal
                           select otp).Single();


                     

            oModel.Rut = Convert.ToInt32(oResult.PK_CLI_EMP_RUT);
            oModel.Servicio = oResult.PK_PAR_SER_ID.ToString();

            var oOTDetalle = from otd in oFol.VI_FOL_VER_OT_DETALLE
                             where otd.PK_FOL_OTP_ID == oModel.OTPrincipal
                             select new ListaExcel
                             {
                                 Id=otd.FL_FOL_OTD_REFERENCIA1,
                                 Referencia=otd.FL_FOL_OTD_REFERENCIA2,
                                 Bulto=otd.FL_FOL_OTD_BULTO.ToString(),
                                 CodigoPostal=otd.FL_FOL_OTD_CODIGO_POSTAL,
                                 Comuna=otd.FL_PAR_COM_NOMBRE,
                                 ContactoEntrega=otd.FL_FOL_OTD_CONTACTO_ENTREGA,
                                 IdComuna=Convert.ToInt32(otd.PK_PAR_COM_ID),
                                 Destinatario=otd.FL_FOL_OTD_DESTINATARIO_NOMBRE,
                                 Direccion=otd.FL_FOL_OTD_DESTINATARIO_DIRECCION,
                                 DireccionNumero=otd.FL_FOL_OTD_DESTINATARIO_NUMERO,
                                 idLocalidad=Convert.ToInt32(otd.PK_PAR_LOC_ID),
                                 Localidad=otd.FL_PAR_LOC_NOMBRE,
                                 Peso = String.Format("{0:0.000}", otd.FL_FOL_OTD_PESO.ToString()),
                                 Rut=otd.FL_FOL_OTD_DESTINATARIO_RUT,
                                 Telefono=otd.FL_FOL_OTD_TELEFONO,
                                 Via=otd.FL_FOL_OTD_VIA,
                                 FechaEstimada=Convert.ToDateTime(otd.FL_FOL_OTD_FECHA_ESTIMADA),
                                 Alto=otd.FL_FOL_OTD_ALTO.ToString(),
                                 Ancho = otd.FL_FOL_OTD_ANCHO.ToString(),
                                 Largo = otd.FL_FOL_OTD_LARGO.ToString()                                 

                                 //Latitud=Convert.ToDouble(otd.FL_FOL_OTD_LATITUD.ToString().Replace("","0")),
                                 //Longitud=Convert.ToDouble(otd.FL_FOL_OTD_LONGITUD.ToString().Replace("","0"))
                             };
            
            oModel.oListaOTDetalle = oOTDetalle;
            oModel.GetNombreReferencia();

            TempData["ModelView"] = oModel;

            return (View(oModel));
        }
        #endregion

        #region vucGoogleMaps

        public double Convertir(string variable)
        {

            var grados = variable.Substring(0, 2);
            var min = variable.Substring(3, 2);
            var seg = variable.Substring(6, 2);

            double total = 0;

            total = Convert.ToDouble(grados);
            total += Convert.ToDouble(min) / 60;
            total += Convert.ToDouble(seg) / 3600;

            return (total);
        }
        public ActionResult vucGoogleMaps(string Origen, string Destino)
        {
           
            ComunasCoordenadasModels oModel = new ComunasCoordenadasModels();

            if (Origen != null && Destino != null)
            {



                LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

                var dOrigen = (from x in oPar.TB_PAR_COM_COMUNA
                               where x.PK_PAR_COM_ID == Convert.ToInt32(Origen)
                               select x).Single();

                var lat1 = dOrigen.FL_PAR_COM_LATITUD.ToString();
                var lon1 = dOrigen.FL_PAR_COM_LONGITUD.ToString();


                var dDestino = (from x in oPar.TB_PAR_COM_COMUNA
                                where x.PK_PAR_COM_ID == Convert.ToInt32(Destino)
                                select x).Single();

                var lat2 = dDestino.FL_PAR_COM_LATITUD.ToString();
                var lon2 = dDestino.FL_PAR_COM_LONGITUD.ToString();


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


                oModel.cOrigen = new ComunasCoordenadasModels.Coordenada
                {
                    Latitud = olat1.ToString(),
                    Longitud = olong1.ToString()
                };

                oModel.cDestino = new ComunasCoordenadasModels.Coordenada
                {
                    Latitud = olat2.ToString(),
                    Longitud = olong2.ToString()
                };

                oModel.Kilometros = kilometros;
            }
            
            return PartialView("vucGoogleMaps",oModel);
        }
        #endregion

        #region GeneraExcel
        public ActionResult GeneraExcelMasivo(int Servicio)
        {
            ValidationController oValidation = new ValidationController();
            string NombreServicio = oValidation.GetNombreServicio(Servicio).Trim();

            MemoryStream Stream = new MemoryStream();
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Carga Masiva");                      


            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            var oDatos = from exc in oPar.TB_PAR_EXC_EXCEL
                         join col in oPar.TB_PAR_COL_COLUMNA on exc.PK_PAR_COL_ID equals col.PK_PAR_COL_ID
                         where exc.PK_PAR_SER_ID == Servicio
                         orderby exc.FL_PAR_EXC_LETRA
                         select new { exc, col };

            //TITULOS
            int oCont = 1;
            
            foreach (var oFila in oDatos)
            {               
               
                worksheet.Cell(1, oFila.exc.FL_PAR_EXC_LETRA).Value = "'" + oFila.col.FL_PAR_COL_NOMBRE.Trim();
                if (oFila.col.FL_PAR_COL_DESCRIPCION != null)
                    worksheet.Cell(1, oFila.exc.FL_PAR_EXC_LETRA).Comment.AddText(oFila.col.FL_PAR_COL_DESCRIPCION);
                if (oFila.exc.FL_PAR_EXC_OBLIGATORIA == true)
                    worksheet.Cell(1, oFila.exc.FL_PAR_EXC_LETRA).Style.Font.FontColor = XLColor.Red;

                oCont=oFila.exc.FL_PAR_EXC_LETRA;
            }




            for (int c = 1; c <= oCont; c++)
            {
                worksheet.Column(c).AdjustToContents();
            }

            workbook.SaveAs(Stream);

            // set HTTP response headers
            Response.ClearHeaders();
            Response.ClearContent();
            Response.Clear();            

            Response.AddHeader("Content-Type", "application/vnd.ms-excel.12");
            //Response.AddHeader("Cache-Control", "no-cache");
            //Response.AddHeader("Accept-Ranges", "none");
            Response.AddHeader("Content-Disposition", "attachment; filename=\"Modelo_Carga_Masiva_" + NombreServicio + ".xlsx\"");

            // Enviar el archivo generado XLSX
            Stream.WriteTo(Response.OutputStream);
            Stream.Close();
            Response.Flush();
            Response.End();

            return View();
        }
        #endregion

        #region Ingreso Manual Papel
        [Authorize]
        public ActionResult IngresoManualPapel(string opt)
        {
            var oPar = new LinqCLIDataContext();
            var oEmpresas = from m in oPar.TB_CLI_EMP_EMPRESAS
                            select m;


            //var datos = new ModelService().ObtenerEjemplo();


            var oModel = new IngresoMasivoModel()
            {
                oItems = oEmpresas.ToList()
            };

            if (opt != null)
            {
                if (opt == "2")
                {
                    oModel.opt = "2";
                }
                else
                {
                    oModel.opt = "1";
                }
            }
            else
            {
                oModel.opt = "1";
            }


            List<SelectListItem> oServicios = new List<SelectListItem>();
            oServicios.Add(new SelectListItem { Text = "-- Seleccione --", Value = "" });

            oModel.oListaServicios = oServicios;


            return View(oModel);
        }
        #endregion
    }
}
