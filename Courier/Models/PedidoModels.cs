using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Courier.Models
{

    #region ColumnasParametricas
    public class ColumnasParametricas
    {
        public bool Id { get; set; }
        public bool Referencia { get; set; }
        public bool Destinatario { get; set; }
        public bool Rut { get; set; }
        public bool ContactoEntrega { get; set; }
        public bool Direccion { get; set; }
        public bool Comuna { get; set; }
        public bool Bulto { get; set; }
        public bool Peso { get; set; }
        public bool Fono { get; set; }
        public bool NDireccion { get; set; }
        public bool Localidad { get; set; }
        public bool Via { get; set; }
        public bool CodigoPostal { get; set; }
        public bool Alto {get;set;}
        public bool Ancho { get; set; }
        public bool Largo { get; set; }

        public bool Observacion1 { get; set; }
        public bool Observacion2 { get; set; }
        public bool Contacto1Rut { get; set; }
        public bool Contacto1Nombre { get; set; }
        public bool Contacto1Telefono { get; set; }
        public bool Contacto2Rut { get; set; }
        public bool Contacto2Nombre { get; set; }
        public bool Contacto2Telefono { get; set; }
        public bool Contacto3Rut { get; set; }
        public bool Contacto3Nombre { get; set; }
        public bool Contacto3Telefono { get; set; }

        public bool TipoDocumento { get; set; }
        public bool NumeroDocumento { get; set; }

        public bool PesoVolumetrico { get; set; }
        public bool ContraPago { get; set; }
    }
    #endregion

    #region Ingreso Masivo
    public class IngresoMasivoModel
    {

        [Required]
        [Display(Name = "OT")]
        [Remote("jSonValidarOTPapel", "Validation")]
        public string OT_Papel { get; set; }

        public ClienteModel Cliente;
        public string opt { get; set; }

        [Required(ErrorMessage = "El campo Rut es obligatorio")]
        //[StringLength(10,MinimumLength=9,ErrorMessage="El campo Rut Cliente Incorrecto Ej. 12345678-9")]
        //[DataType(DataType.Text)]       
        [Display(Name = "Rut")]
        [Remote("ValidarCliente", "Validation")]
        [RegularExpression("\\d*[0-9]-([0-9]|k|K)$", ErrorMessage = "Campo Rut Formato incorrecto, ejemplo: 12345678-9")]
        public string RutCliente { get; set; }


        public List<ListaExcel> Lista;
        public List<ListaErrores> ListaErrores;        
        public IEnumerable<TB_CLI_EMP_EMPRESAS> oItems { get; set; }
        public Stream oStream { get; set; }
        public String oType { get; set; }
        public int[] oColumnas { get; set; }
        
        [Required]
        [DataType(DataType.Text)]
        [RegularExpression("\\d*[0-9]", ErrorMessage = "No Corresponde")]
        [Display(Name = "Servicio")]
        [Remote("ValidarColumnas", "Validation")]
        public string Servicio { get; set; }                

        public IEnumerable<SelectListItem> oListaServicios { get; set; }


        [Required(ErrorMessage="El Archivo es obligatorio.")]
        [DataType(DataType.Text)]      
        [Display(Name = "Archivo")]
        [Remote("ValidarArchivo", "Validation")]
        public string Archivo { get; set; }
        //CONTACTO ENTREGA
        public ContactoEntregaModel ContactoEntrega;
        //ENTREGA
        public EntregaModel Entrega;

        public string NombreRef1 { get; set; }
        public string NombreRef2 { get; set; }

        #region GetNombreReferencia
        public void GetNombreReferencia()
        {
            try
            {

                LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

                var oDatos = from x in oPar.TB_PAR_RES_REFERENCIA_SERVICIO
                             join y in oPar.TB_PAR_TDO_TIPO_DOCUMENTO on x.PK_PAR_TDO_ID equals y.PK_PAR_TDO_ID
                             where x.PK_PAR_SER_ID == Convert.ToInt32(Servicio)
                             select new
                             {
                                 x.PK_PAR_REF_ID,
                                 y.FL_PAR_TDO_NOMBRE
                             };
                NombreRef1 = "Referencia 1";
                NombreRef2 = "Referencia 2";
                foreach (var fila in oDatos)
                {
                    if (fila.PK_PAR_REF_ID == 1)//Referencia 1
                        NombreRef1 = fila.FL_PAR_TDO_NOMBRE;
                    else if (fila.PK_PAR_REF_ID == 2)//Referencia 2
                        NombreRef2 = fila.FL_PAR_TDO_NOMBRE;
                }
            }
            catch
            {

            }
        }
        #endregion
    }

    public  class ListaErrores
    {
        private int _Fila;
        private string _Error;


        public ListaErrores(int Fila, string Error)
        {
            _Fila=Fila;
            _Error=Error;

        }
                
        public int Fila 
        { 
            get { return _Fila; }
            set { Fila = value; }
        }

        public string Error
        {
            get { return _Error; }
            set { Error = value; }
        }

    }

    
    public  class ListaExcel
    {
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
        //17 Ancho
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
        //28 Contacto 3, Teléfono
        //29	Tipo Documento
        //30	N° Documento
        //31	Peso Volumétrico
        //32	Contrapago


        [Display(Name = "Referencia 1")]
        public string Id { get; set; }//new

        [Display(Name = "Referencia 2")]
        public string Referencia { get; set; }
        public string Destinatario { get; set; }//new
        public string Rut { get; set; }
        public string ContactoEntrega { get; set; }//new
        public string Direccion { get; set; }
        public string Comuna { get; set; }
        public string Bulto { get; set; }
        public string Peso { get; set; }
        public string Telefono { get; set; }
        public int IdComuna { get; set; }
        public string DireccionNumero { get; set; }//11
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public int idLocalidad { get; set; }
        public string Localidad { get; set; }
        public string Via { get; set; }
        public string CodigoPostal { get; set; }
        public string Largo { get; set; }
        public string Alto { get; set; }
        public string Ancho { get; set; }

        public string Observacion1 { get; set; }
        public string Observacion2 { get; set; }

        public string Contacto1Nombre { get; set; }
        public string Contacto1Rut { get; set; }
        public string Contacto1Telefono { get; set; }

        public string Contacto2Nombre { get; set; }
        public string Contacto2Rut { get; set; }
        public string Contacto2Telefono { get; set; }

        public string Contacto3Nombre { get; set; }
        public string Contacto3Rut { get; set; }
        public string Contacto3Telefono { get; set; }

        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }

        public string PesoVolumetrico { get; set; }

        [Display (Name="Contra Pago")]
        public string ContraPago { get; set; }

        public DateTime FechaEstimada { get; set; }

        private decimal OTDetalle { get; set; }
        private decimal OTPrincipal { get; set; }
        private int idSucursal { get; set; }

        public void AgregarOTD(decimal OTD)
        {
            OTDetalle = OTD;
        }
        public void AgregarOTP(decimal OTP)
        {
            OTPrincipal = OTP;
        }
        public decimal GetOTD()
        {
            return OTDetalle;
        }
        public decimal GetOTP()
        {
            return OTPrincipal;
        }

        public int GetSucursal()
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oDatos = (from x in oPar.TB_PAR_COB_COBERTURA_COMUNA
                         where x.PK_PAR_COM_ID == IdComuna
                         select x.PK_PAR_SUC_ID).Single();
            return oDatos;
        }
       
    }
    #endregion

    #region Ingreso Pedido Model
    public class IngresoPedidoModel
    {
        public string RutCliente { get; set; }
        public int Servicio { get; set; }
        public ColumnasParametricas oColumnas { get; set; }
        public ColumnasParametricas oObligatoria { get; set; }   
        public IEnumerable<SelectListItem> oListaRegion {get;set;}
        public IEnumerable<SelectListItem> oListaComuna { get; set; }
        public IEnumerable<SelectListItem> oListaLocalidad { get; set; }
        public IEnumerable<SelectListItem> oListaVia { get; set; }
        public string opt { get; set; }


        [Required]
        [Display (Name="OT")]
        [Remote("jSonValidarOTPapel", "Validation")]
        public string OT_Papel{ get; set; }

        public string OT_Papel_Paso { get; set; }
        
        [Required]
        [Display(Name = "Tipo")]        
        public string TMM_ID { get; set; }

        public IEnumerable<SelectListItem> oLista_TMM { get; set; }

        #region GetListaTMM
        public void GetListaTMM()
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oDatos = from tmm in oPar.TB_PAR_TMM_TIPO_OT_MANUAL
                         select new SelectListItem
                         {
                             Text = tmm.FL_PAR_TMM_NOMBRE,
                             Value = tmm.PK_PAR_TMM_ID.ToString()
                         };
            oLista_TMM = oDatos;

            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            Controllers.ValidationController oValidation = new Controllers.ValidationController();
            if (oValidation.isDecimal(OT_Papel_Paso))
            {
                var oDatos2 = from rmm in oFol.TB_FOL_RMM_REL_TMM_OTP
                              where rmm.PK_FOL_OTP_ID == Convert.ToDecimal(OT_Papel_Paso)
                              select rmm;
                if (oDatos2.Count() > 0)
                {
                    TMM_ID = oDatos2.ToList()[0].PK_PAR_TMM_ID.ToString();
                }
            }
        }
        #endregion        

        //Parametrico

        

        [Display(Name = "Referencia 1")]
        [Remote("jSonValidarReferencia1", "Validation",AdditionalFields="RutCliente, Servicio")]
        public string Id { get; set; }//new

        [Display(Name = "Referencia 2")]
        [Remote("jSonValidarReferencia2", "Validation", AdditionalFields = "RutCliente, Servicio")]
        public string Referencia { get; set; }

        public string Destinatario { get; set; }//new

        
        [Remote("ValidarRutSolo", "Validation")]        
        public string Rut { get; set; }

        [Display (Name="Región")]        
        public string Region { get; set; }

        [Display (Name="Contacto Entrega")]
        public string ContactoEntrega { get; set; }//new

        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        [Remote("jSonValidarCoberturaComuna", "Validation")]
        public string Comuna { get; set; }

        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        [Display (Name="N° Bultos")]
        public string Bulto { get; set; }


        [RegularExpression(@"^[0-9]*([\,][0-9]{1,5})?$", ErrorMessage = "Peso admite números con decimales separados por coma (Máx. 5 decimales)")]
        public string Peso { get; set; }

        [RegularExpression(@"^[0-9]*([\,][0-9]{1,5})?$", ErrorMessage = "Peso Volumétrico admite números con decimales separados por coma (Máx. 5 decimales)")]
        [Display (Name="Peso Volumétrico")]
        public string PesoVolumetrico { get; set; }


        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        [Display(Name = "Comuna")]                
        public int IdComuna { get; set; }

        [Display(Name = "N°")]
        public string DireccionNumero { get; set; }//11
        public double Latitud { get; set; }
        public double Longitud { get; set; }

        [Display (Name="Localidad")]
        public int idLocalidad { get; set; }

        [Remote("jSonValidarCoberturaLocalidad", "Validation")]
        public string Localidad { get; set; }
        public string Via { get; set; }

        [Display (Name="Código Postal")]
        public string CodigoPostal { get; set; }
        [RegularExpression(@"^[0-9]*([\,][0-9]{1,5})?$", ErrorMessage = "Largo admite números con decimales separados por coma (Máx. 5 decimales)")]
        public string Largo { get; set; }
        [RegularExpression(@"^[0-9]*([\,][0-9]{1,5})?$", ErrorMessage = "Alto admite números con decimales separados por coma (Máx. 5 decimales)")]
        public string Alto { get; set; }
        [RegularExpression(@"^[0-9]*([\,][0-9]{1,5})?$", ErrorMessage = "Ancho admite números con decimales separados por coma (Máx. 5 decimales)")]
        public string Ancho { get; set; }

        public string Observacion1 { get; set; }
        public string Observacion2 { get; set; }

        [RegularExpression("\\d*[0-9]-([0-9]|k|K)$", ErrorMessage = "Campo Rut Formato incorrecto, ejemplo: 12345678-9")]
        [Remote("ValidarRutContacto1", "Validation")]
        [Display(Name = "Contacto 1 Rut")]
        public string Contacto1Rut { get; set; }
        [Display(Name = "Contacto 1 Nombre")]
        public string Contacto1Nombre { get; set; }
        [Display(Name = "Contacto 1 Teléfono")]
        public string Contacto1Telefono { get; set; }

        [RegularExpression("\\d*[0-9]-([0-9]|k|K)$", ErrorMessage = "Campo Rut Formato incorrecto, ejemplo: 12345678-9")]
        [Remote("ValidarRutContacto2", "Validation")]
        [Display(Name = "Contacto 2 Rut")]
        public string Contacto2Rut { get; set; }
        [Display(Name = "Contacto 2 Nombre")]
        public string Contacto2Nombre { get; set; }
        [Display(Name = "Contacto 2 Teléfono")]
        public string Contacto2Telefono { get; set; }

        [RegularExpression("\\d*[0-9]-([0-9]|k|K)$", ErrorMessage = "Campo Rut Formato incorrecto, ejemplo: 12345678-9")]
        [Remote("ValidarRutContacto3", "Validation")]
        [Display(Name = "Contacto 3 Rut")]
        public string Contacto3Rut { get; set; }
        [Display(Name = "Contacto 3 Nombre")]
        public string Contacto3Nombre { get; set; }
        [Display (Name="Contacto 3 Teléfono")]
        public string Contacto3Telefono { get; set; }

        [Display(Name = "Tipo Documento")]
        public string TipoDocumento { get; set; }

        [Display(Name = "N° Documento")]
        public string NumeroDocumento { get; set; }

        [Display(Name = "Contra Pago")]
        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        public string ContraPago { get; set; }  

        ////OBLIGATORIO

        [Required]
        [Display(Name = "Referencia 1")]
        [Remote("jSonValidarVReferencia1", "Validation", AdditionalFields = "RutCliente,Servicio")]
        public string vId { get; set; }//new

        [Required]
        [Display(Name = "Referencia 2")]
        [Remote("jSonValidarVReferencia2", "Validation", AdditionalFields = "RutCliente,Servicio")]
        public string vReferencia { get; set; }

        [Required]
        [Display(Name = "Destinatario")]
        public string vDestinatario { get; set; }//new

        [Required]
        [Display(Name = "Rut")]
        [Remote("vValidarRutSolo", "Validation")]
        public string vRut { get; set; }

        [Required]
        [Display(Name = "Región")]
        public string vRegion { get; set; }

        [Required]
        [Display(Name = "Contacto Entrega")]
        public string vContactoEntrega { get; set; }//new

        [Required]
        [Display(Name = "Dirección")]
        public string vDireccion { get; set; }

        [Display(Name = "Comuna")]
        [Required]
        [Remote("vjSonValidarCoberturaComuna", "Validation")]
        public string vComuna { get; set; }

        [Required]
        [Display(Name = "N° Bulto")]
        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        public string vBulto { get; set; }

        [Required]
        [Display(Name = "Peso")]
        [RegularExpression(@"^[0-9]*([\,][0-9]{1,5})?$", ErrorMessage = "Peso admite números con decimales separados por coma (Máx. 5 decimales)")]
        public string vPeso { get; set; }

        [Required]
        [Display(Name = "Peso Volumétrico")]
        [RegularExpression(@"^[0-9]*([\,][0-9]{1,5})?$", ErrorMessage = "Peso Volumétrico admite números con decimales separados por coma (Máx. 5 decimales)")]
        public string vPesoVolumetrico { get; set; }

        [Required]
        [Display(Name = "Teléfono")]
        public string vTelefono { get; set; }


        [Required]
        [Display(Name = "N°")]
        public string vDireccionNumero { get; set; }//11

        [Required]
        [Display(Name = "Localidad")]
        [Remote("vjSonValidarCoberturaLocalidad", "Validation")]
        public string vLocalidad { get; set; }

        [Required]
        [Display(Name = "Vía")]
        public string vVia { get; set; }

        [Required]
        [Display(Name = "Código Postal")]
        public string vCodigoPostal { get; set; }

        [Required]
        [Display(Name = "Largo")]
        [RegularExpression(@"^[0-9]*([\,][0-9]{1,5})?$", ErrorMessage = "Largo admite números con decimales separados por coma (Máx. 5 decimales)")]
        public string vLargo { get; set; }

        [Required]
        [Display(Name = "Alto")]
        [RegularExpression(@"^[0-9]*([\,][0-9]{1,5})?$", ErrorMessage = "Alto admite números con decimales separados por coma (Máx. 5 decimales)")]
        public string vAlto { get; set; }

        [Required]
        [Display(Name = "Ancho")]
        [RegularExpression(@"^[0-9]*([\,][0-9]{1,5})?$", ErrorMessage = "Ancho admite números con decimales separados por coma (Máx. 5 decimales)")]
        public string vAncho { get; set; }


        public string lComuna { get; set; }
        public string lLocalidad { get; set; }

        [Required]
        [Display (Name="Observacion1")]
        public string vObservacion1 { get; set; }
        [Required]
        [Display(Name = "Observacion2")]
        public string vObservacion2 { get; set; }

        [RegularExpression("\\d*[0-9]-([0-9]|k|K)$", ErrorMessage = "Campo Rut Formato incorrecto, ejemplo: 12345678-9")]
        [Remote("ValidarRutvContacto1", "Validation")]
        [Required]
        [Display (Name="Contacto 1 Rut")]
        public string vContacto1Rut { get; set; }
        
        [Required]
        [Display(Name = "Contacto 1 Nombre")]        
        public string vContacto1Nombre { get; set; }
        
        [Required]
        [Display(Name = "Contacto 1 Teléfono")]
        public string vContacto1Telefono { get; set; }

        [RegularExpression("\\d*[0-9]-([0-9]|k|K)$", ErrorMessage = "Campo Rut Formato incorrecto, ejemplo: 12345678-9")]
        [Remote("ValidarRutvContacto2", "Validation")]
        [Required]
        [Display(Name = "Contacto 2 Rut")]
        public string vContacto2Rut { get; set; }
        
        [Required]
        [Display(Name = "Contacto 2 Nombre")]
        public string vContacto2Nombre { get; set; }
        
        [Required]
        [Display(Name = "Contacto 2 Teléfono")]
        public string vContacto2Telefono { get; set; }

        [RegularExpression("\\d*[0-9]-([0-9]|k|K)$", ErrorMessage = "Campo Rut Formato incorrecto, ejemplo: 12345678-9")]
        [Remote("ValidarRutvContacto3", "Validation")]
        [Display(Name = "Contacto 3 Rut")]
        [Required]
        public string vContacto3Rut { get; set; }
        
        [Display(Name = "Contacto 3 Nombre")]
        [Required]
        public string vContacto3Nombre { get; set; }
        
        [Display(Name = "Contacto 3 Teléfono")]
        [Required]
        public string vContacto3Telefono { get; set; }

        [Display(Name = "Tipo Documento")]
        [Required]
        public string vTipoDocumento { get; set; }

        [Display(Name = "N° Documento")]
        [Required]
        public string vNumeroDocumento { get; set; }

        [Display(Name = "Contra Pago")]
        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        [Required]
        public string vContraPago { get; set; }


        //Retiro

        [Required]
        [Display(Name = "Fecha")]
        public string RetiroFecha { get; set; }

        [Required]
        [Display(Name = "Región")]
        public string RetiroRegion { get; set; }

        [Required]
        [Remote("jSonValidarCoberturaComunaRetiro", "Validation")]
        [Display(Name = "Comuna")]
        public string RetiroComuna { get; set; }
        
        [Display(Name = "Localidad")]
        [Remote("jSonValidarCoberturaLocalidadRetiro", "Validation")]
        public string RetiroLocalidad { get; set; }

        [Required]
        [Display(Name = "Dirección")]
        public string RetiroDireccion { get; set; }

        [Required]
        [Display(Name = "N°")]
        public string RetiroNumero { get; set; }

        //Referencias

        public string NombreRef1 { get; set; }
        public string NombreRef2 { get; set; }

        public IEnumerable<SelectListItem> ListaTipoDocumento { get; set; }

        #region GetNombreReferencia
        public void GetNombreReferencia()
        {
            Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();
            var oResult = oValidation.GetNombreReferencia(Servicio);
            NombreRef1 = oResult.NombreRef1;
            NombreRef2 = oResult.NombreRef2;
        }
        #endregion
        #region GetTipoDocumento
        public void GetTipoDocumento()
        {
            Controllers.ValidationController oValidation = new Controllers.ValidationController();            
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from pr in oFol.PR_FOL_COC_CONSULTA(Convert.ToInt32(RutCliente.Substring(0, RutCliente.Length - 2)))
                         select new SelectListItem{ 
                             Value=pr.PK_FOL_COC_ID.ToString(),
                             Text=pr.FL_PAR_TDO_NOMBRE
                         };
            ListaTipoDocumento = oDatos;

        }
        #endregion
    }
    #endregion

    #region Client Model
    public class ClienteModel
    {
        [Required(ErrorMessage="El campo Rut es obligatorio")]
        //[StringLength(10,MinimumLength=9,ErrorMessage="El campo Rut Cliente Incorrecto Ej. 12345678-9")]
        //[DataType(DataType.Text)]       
        [Display(Name = "Rut")]        
        [Remote("ValidarCliente", "Validation")]        
        [RegularExpression("\\d*[0-9]-([0-9]|k|K)$",ErrorMessage="Campo Rut Formato incorrecto, ejemplo: 12345678-9")]        
        public object RutCliente { get; set; }
                        
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Cliente")]
        public string Cliente { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [RegularExpression("\\d*[0-9]",ErrorMessage="No Corresponde")]
        [Display(Name = "Servicio")]
        public string Servicio { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [RegularExpression("\\d*[0-9]", ErrorMessage = "No Corresponde")]
        [Display(Name = "Servicio")]
        public string Servicio2 { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Comuna")]
        public string Comuna { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Ciudad")]
        public string Ciudad { get; set; }


        [DataType(DataType.Text)]
        [Display(Name = "IATA")]
        public string IATA { get; set; }


        [DataType(DataType.Text)]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        //SUCURSAL
        [DataType(DataType.Text)]
        [Display(Name = "Sucursal")]
        public string Sucursal { get; set; }

        //Datos Correspondientes a la Vista de Ingreso de Clientes
        //RAZON SOCIAL
        [DataType(DataType.Text)]
        [Display(Name = "Razón Social")]
        public string RSocial { get; set; }

        //NOMBRE FANTACIA
        [DataType(DataType.Text)]
        [Display(Name = "Nombre Fantasia")]
        public string NFantasia { get; set; }

        //GIRO COMERCIAL
        [DataType(DataType.Text)]
        [Display(Name = "Giro Comercial")]
        public string GComercial { get; set; }

        //REPRESENTANTE LEGAL
        [DataType(DataType.Text)]
        [Display(Name = "Nombre Representante")]
        public string NRepr { get; set; }

        //RUT REPRESENTANTE
        [DataType(DataType.Text)]
        [Display(Name = "Rut")]
        public string RutRepr { get; set; }

        //APELLIDO PATERNO
        [DataType(DataType.Text)]
        [Display(Name = "Apellido Paterno")]
        public string APaterno { get; set; }

        //APELLIDO MATERNO
        [DataType(DataType.Text)]
        [Display(Name = "Apellido Materno")]
        public string AMaterno { get; set; }

        //REGIONES
        [DataType(DataType.Text)]
        [Display(Name = "Región")]
        public string Region { get; set; }

        //FONO
        [DataType(DataType.Text)]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        //FAX
        [DataType(DataType.Text)]
        [Display(Name = "Fax")]
        public string Fax { get; set; }

        //NOMBRE CONTACTO COMERCIAL
        [DataType(DataType.Text)]
        [Display(Name = "Nombres")]
        public string NCont { get; set; }
        public string NRet { get; set; }

        //RUT CONTACTO COMERCIA
        [DataType(DataType.Text)]
        [Display(Name = "Rut")]
        public string RutCont { get; set; }
        public string RutRet { get; set; }

        //APELLIDO PATERNO CONTACTO COMERCIAL
        [DataType(DataType.Text)]
        [Display(Name = "Apellido Paterno")]
        public string APaternoCont { get; set; }
        public string APaternoRet { get; set; }

        //APELLIDO MATERNO CONTACTO COMERCIAL
        [DataType(DataType.Text)]
        [Display(Name = "Apellido Materno")]
        public string AMaternoCont { get; set; }
        public string AMaternoRet { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Comuna")]
        public string ComunaCont { get; set; }
        public string ComunaRet { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Ciudad")]
        [Editable(false)]
        public string CiudadCont { get; set; }
        public string CiudadRet { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Dirección")]
        public string DirCont { get; set; }
        public string DirRet { get; set; }

        //REGIONES CONTACTO COMERCIAL
        [DataType(DataType.Text)]
        [Display(Name = "Región")]
        public string RegionCont { get; set; }
        public string RegionRet { get; set; }

        //FONO CONTACTO COMERCIAL
        [DataType(DataType.Text)]
        [Display(Name = "Teléfono")]
        public string TelefonoCont { get; set; }
        public string TelefonoRet { get; set; }

        //FAX CONTACTO COMERCIAL
        [DataType(DataType.Text)]
        [Display(Name = "Fax")]
        public string FaxCont { get; set; }
        public string FaxRet { get; set; }

        //EMAIL CONTACTO COMERCIAL
        [DataType(DataType.Text)]
        [Display(Name = "Email")]
        public string EmailCont { get; set; }
        public string EmailRet { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Nombre (Encargado de Pago)")]
        public string NEncargado { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Segmento Negocio")]
        public string SNegocio { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Fecha Apertura Cuenta")]
        public string FApertura { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Fecha Inicio Servicio")]
        public string FInicio { get; set; }
    }
    #endregion

    #region ContactoEntregaModel
    public class ContactoEntregaModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Destino")]
        public string Destino { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Ciudad")]
        public string Ciudad { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "IATA")]
        public string IATA { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Sucursal / CD")]
        public string Sucursal { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "RUT")]
        public string RUT { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Apellido Paterno")]
        public string ApellidoPaterno { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Apellido Materno")]
        public string ApellidoMaterno { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Nombres")]
        public string Nombres { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Vía")]
        public string Via { get; set; }
    }
#endregion

    #region Entrega
    public class EntregaModel
    {        
        [DataType(DataType.Text)]
        [Display(Name = "Zona")]
        public string Zona { get; set; }
        
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha Estimada Entrega")]
        public string FechaEstimadaEntrega { get; set; }
    }
    #endregion

    #region VerOT
    public class VerOTModel
    {
        public int OTPrincipal { get; set; }
        public int Rut { get; set; }
        public string Servicio { get; set; }
        public IEnumerable<ListaExcel> oListaOTDetalle { get; set; }
        public int[] oColumnas { get; set; }

        public string NombreRef1 { get; set; }
        public string NombreRef2 { get; set; }

        #region GetNombreReferencia
        public void GetNombreReferencia()
        {
            try
            {

                LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

                var oDatos = from x in oPar.TB_PAR_RES_REFERENCIA_SERVICIO
                             join y in oPar.TB_PAR_TDO_TIPO_DOCUMENTO on x.PK_PAR_TDO_ID equals y.PK_PAR_TDO_ID
                             where x.PK_PAR_SER_ID == Convert.ToInt32(Servicio)
                             select new
                             {
                                 x.PK_PAR_REF_ID,
                                 y.FL_PAR_TDO_NOMBRE
                             };
                NombreRef1 = "Referencia 1";
                NombreRef2 = "Referencia 2";
                foreach (var fila in oDatos)
                {
                    if (fila.PK_PAR_REF_ID == 1)//Referencia 1
                        NombreRef1 = fila.FL_PAR_TDO_NOMBRE;
                    else if (fila.PK_PAR_REF_ID == 2)//Referencia 2
                        NombreRef2 = fila.FL_PAR_TDO_NOMBRE;
                }
            }
            catch
            {

            }
        }
        #endregion
    }

    #endregion

    #region AnularPedidoModels
    public class AnularPedidoModels
    {
        [Display (Name="OT Detalle")]
        [Required]
        [Remote("vJsonValidarCodigoAnular","Validation")]
        public string OT { get; set; }

        public VI_FOL_DATOS_ANULAR DatosAnular { get; set; }
        public IEnumerable<TB_FOL_BLT_BULTO> ListaBultos { get; set; }

        public void GetDatosAnular (decimal OTP, decimal OTD)
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            
            var oDatos = (from x in oFol.VI_FOL_DATOS_ANULAR
                         where
                             x.PK_FOL_OTP_ID==OTP &&
                             x.PK_FOL_OTD_ID==OTD
                         select x).Single();
            DatosAnular=oDatos;
            
        }
        public void GetBultos(decimal OTP, decimal OTD)
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            var oDatos = from blt in oFol.TB_FOL_BLT_BULTO
                         where blt.PK_FOL_OTP_ID == OTP
                         && blt.PK_FOL_OTD_ID == OTD
                         select blt;

            ListaBultos = oDatos;
        }
    }   
    #endregion
}