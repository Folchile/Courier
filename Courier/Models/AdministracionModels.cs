using System;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using System.Linq;

namespace Courier.Models
{
    public class CorreoModels
    {
        public string Rut { get; set; }
        public IEnumerable<PR_FOL_SUE_CONSULTA_POR_RUTResult> ListaCorreo { get; set; }


        public int RutCorreo { get; set; }

        [Required]
        [Display (Name="Servicio")]
        public string SUE_SER_ID { get; set; }
        
        [Required]
        [Display(Name = "Estado")]
        public string SUE_EST_ID { get; set; }

        [Required]        
        public string Empresa { get; set; }

        public IEnumerable<SelectListItem> ListaEmpresas { get; set; }
        public IEnumerable<SelectListItem> ListaServicios { get; set; }
        public IEnumerable<SelectListItem> ListaEstados { get; set; }

        #region GetListaCorreo
        public void GetListaCorreo ()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            var oDatos = (from pr in oFol.PR_FOL_SUE_CONSULTA_POR_RUT(RutCorreo)
                          select pr).ToList();
            ListaCorreo = oDatos;
        }
        #endregion
        #region GetListaEmpresas
        public void GetListaEmpresas()
        {
            LinqCLIDataContext oCli = new LinqCLIDataContext();
            var oDatos = from emp in oCli.TB_CLI_EMP_EMPRESAS
                         orderby emp.FL_CLI_EMP_FANTASIA
                         select new SelectListItem
                         {
                             Text=emp.FL_CLI_EMP_FANTASIA.ToUpper(),
                             Value=emp.PK_CLI_EMP_RUT.ToString()
                         };
            ListaEmpresas = oDatos;
        }
        #endregion
        #region GetListaServicios
        public void GetListaServicios()
        {

            int oEmpresa;

            if (!int.TryParse(Empresa, out oEmpresa))
                oEmpresa = 0;

            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oDatos = from ems in oPar.TB_PAR_EMS_EMPRESA_SERVICIO
                         join ser in oPar.TB_PAR_SER_SERVICIO
                            on ems.PK_PAR_SER_ID equals ser.PK_PAR_SER_ID
                         where
                            ems.PK_CLI_EMP_RUT==oEmpresa
                         orderby ser.FL_PAR_SER_NOMBRE
                         select new SelectListItem
                         {
                             Text = ser.FL_PAR_SER_NOMBRE.ToUpper(),
                             Value = ser.PK_PAR_SER_ID.ToString()
                         };
            ListaServicios = oDatos;
        }
        #endregion
        #region GetListaEstados
        public void GetListaEstados()
        {
                       
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from ese in oFol.TB_FOL_ESE_ESTADO_ENTREGA
                         join est in oFol.TB_FOL_EST_ESTADO
                         on ese.PK_FOL_EST_ID equals est.PK_FOL_EST_ID
                         orderby est.FL_FOL_EST_NOMBRE
                         select new SelectListItem
                         {
                             Text = est.FL_FOL_EST_NOMBRE.Trim().ToUpper() + " " + est.FL_FOL_EST_DESCRIPCION,
                             Value = ese.PK_FOL_EST_ID.ToString()
                         };
            ListaEstados = oDatos;
        }
        #endregion
    }
    public class AdministracionBultoModels
    {
        public IEnumerable<PR_FOL_VER_BULTO_ESTADOResult> ListaBultos { get; set; }
        public decimal bOTP { get; set; }
        public decimal bOTD { get; set; }
        public bool PuedeReImprimirEtiqueta { get; set; }
        public bool PuedeModificarPesoBulto { get; set; }

        public void GetListaBultos()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = (from pr in oFol.PR_FOL_VER_BULTO_ESTADO(bOTP, bOTD)
                         select pr).ToList();
            ListaBultos = oDatos;
        }
    }
    public class ModificaPesoBultoModels
    {
        [Display (Name="Id Bulto")]
        public decimal MP_Bulto { get; set; }
        [Display(Name = "Kg")]
        [Required]
        [RegularExpression(@"^[0-9]*([\,][0-9]{1,5})?$", ErrorMessage = "Peso admite números con decimales separados por coma (Máx. 5 decimales)")]
        public string MP_KG { get; set; }

        #region GetKGBulto
        public void GetKGBulto()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = (from blt in oFol.TB_FOL_BLT_BULTO
                          where blt.PK_FOL_BLT_ID == MP_Bulto
                          select blt).Single();
            MP_KG = oDatos.FL_FOL_BLT_PESO.ToString();
        }
        #endregion

        #region GuardaCambios
        public void GuardaCambios()
        {
            Controllers.ValidationController oValidation = new Controllers.ValidationController();
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = (from blt in oFol.TB_FOL_BLT_BULTO
                          where blt.PK_FOL_BLT_ID == MP_Bulto
                          select blt).Single();
            oDatos.FL_FOL_BLT_PESO = Convert.ToSingle(MP_KG);
            oDatos.PK_PAR_USU_RUT = oValidation.GetRutActiveUser();
            oFol.SubmitChanges();
        }
        #endregion

    }
    public class RecepcionOKManualModels
    {
        public bool PuedeModificarDireccion { get; set; }
        public bool PuedeSiniestrarOT { get; set; }
        public bool PuedeRecibirRechazarManual { get; set; }
        public bool PuedeCambiarServicio { get; set; }
        public bool PuedeModificarObservacion { get; set; }
        public bool PuedeRectificarUbicaciones { get; set; }

        public List<SelectListItem> ListaHora { get; set; }
        public List<SelectListItem> ListaMinuto { get; set; }

        public string ROK_OT { get; set; }        

        public decimal OTP { get;  set; } 
        public decimal OTD { get; set; }

        public decimal FOTP { get; set; }
        public decimal FOTD { get; set; }

        [Display (Name="Fecha Recepción")]
        [Required]
        public string FechaRecepcionOK { get; set; }
        
        [Required]
        [Display (Name="Hora")]
        public string HoraRecepcion { get; set; }

        [Required]
        [Display(Name = "Minuto")]
        public string MinutoRecepcion { get; set; }

        [Required]
        public string Estado { get; set; }

        [Required]
        public string Observacion { get; set; }

        [Remote("ValidarRutSolo","Validation")]
        [Display (Name="Recibe Rut")]
        public string Rut { get; set; }

        [Display(Name = "Recibe Nombre")]

        public string Nombre { get; set; }




        public IEnumerable<SelectListItem> ListaEstado { get; set; }
        public IEnumerable<SelectListItem> ListaObservaciones { get; set; }


        public int CantidadDias { get; set; }

        #region GetDiasDiferencia
        public void GetDiasDiferencia()
        {            
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            int? oCantidad = (from pr in oFol.PR_FOL_HID_CONSULTA_DIAS_DIF(OTP, OTD)
                             select pr).Single().DIAS_DIFERENCIA;
            if (oCantidad != null)
            {
                CantidadDias = Convert.ToInt32(oCantidad);
            }
            else
            {
                CantidadDias = 0;
            }
        }
        #endregion        
        #region GetListaEstado
        public void GetListaEstado()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from est in oFol.TB_FOL_EST_ESTADO
                         where est.PK_FOL_EST_ID == 10 || est.PK_FOL_EST_ID == 8 || est.PK_FOL_EST_ID == 15
                         select new SelectListItem{
                             Text=est.FL_FOL_EST_NOMBRE.Replace("EN RUTA","CLIENTE NO ENCONTRADO"),
                             Value=est.PK_FOL_EST_ID.ToString()
                         };
            ListaEstado=oDatos;
        }
        #endregion
        #region GetListaObservaciones
        public void GetListaObservaciones()
        {
            Controllers.ValidationController oValidation = new Controllers.ValidationController();

            if (oValidation.IsNumeric2(Estado))
            {
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                var oDatos = from obs in oFol.TB_FOL_OES_OBSERVACION_ESTADO
                             where obs.PK_FOL_EST_ID == Convert.ToInt32(Estado)
                             select new SelectListItem
                             {
                                 Text = obs.FL_FOL_OES_NOMBRE,
                                 Value = obs.PK_FOL_OES_ID.ToString()
                             };
                ListaObservaciones = oDatos;
            }
            else
            {
                List<SelectListItem> oLista = new List<SelectListItem>();
                ListaObservaciones = oLista;
            }
        }
        #endregion

        #region GetHora
        public void GetHora()
        {
            List<SelectListItem> oHora = new List<SelectListItem>();
            List<SelectListItem> oMinuto = new List<SelectListItem>();
            oHora.Add(new SelectListItem { Text = "00", Value = "00" });
            oHora.Add(new SelectListItem { Text = "01", Value = "01" });
            oHora.Add(new SelectListItem { Text = "02", Value = "02" });
            oHora.Add(new SelectListItem { Text = "03", Value = "03" });
            oHora.Add(new SelectListItem { Text = "04", Value = "04" });
            oHora.Add(new SelectListItem { Text = "05", Value = "05" });
            oHora.Add(new SelectListItem { Text = "06", Value = "06" });
            oHora.Add(new SelectListItem { Text = "07", Value = "07" });
            oHora.Add(new SelectListItem { Text = "08", Value = "08" });
            oHora.Add(new SelectListItem { Text = "09", Value = "09" });
            oHora.Add(new SelectListItem { Text = "10", Value = "10" });
            oHora.Add(new SelectListItem { Text = "11", Value = "11" });
            oHora.Add(new SelectListItem { Text = "12", Value = "12" });
            oHora.Add(new SelectListItem { Text = "13", Value = "13" });
            oHora.Add(new SelectListItem { Text = "14", Value = "14" });
            oHora.Add(new SelectListItem { Text = "15", Value = "15" });
            oHora.Add(new SelectListItem { Text = "16", Value = "16" });
            oHora.Add(new SelectListItem { Text = "17", Value = "17" });
            oHora.Add(new SelectListItem { Text = "18", Value = "18" });
            oHora.Add(new SelectListItem { Text = "19", Value = "19" });
            oHora.Add(new SelectListItem { Text = "20", Value = "20" });
            oHora.Add(new SelectListItem { Text = "21", Value = "21" });
            oHora.Add(new SelectListItem { Text = "22", Value = "22" });
            oHora.Add(new SelectListItem { Text = "23", Value = "23" });

            oMinuto.Add(new SelectListItem { Text = "00", Value = "00" });
            oMinuto.Add(new SelectListItem { Text = "01", Value = "01" });
            oMinuto.Add(new SelectListItem { Text = "02", Value = "02" });
            oMinuto.Add(new SelectListItem { Text = "03", Value = "03" });
            oMinuto.Add(new SelectListItem { Text = "04", Value = "04" });
            oMinuto.Add(new SelectListItem { Text = "05", Value = "05" });
            oMinuto.Add(new SelectListItem { Text = "06", Value = "06" });
            oMinuto.Add(new SelectListItem { Text = "07", Value = "07" });
            oMinuto.Add(new SelectListItem { Text = "08", Value = "08" });
            oMinuto.Add(new SelectListItem { Text = "09", Value = "09" });
            oMinuto.Add(new SelectListItem { Text = "10", Value = "10" });
            oMinuto.Add(new SelectListItem { Text = "11", Value = "11" });
            oMinuto.Add(new SelectListItem { Text = "12", Value = "12" });
            oMinuto.Add(new SelectListItem { Text = "13", Value = "13" });
            oMinuto.Add(new SelectListItem { Text = "14", Value = "14" });
            oMinuto.Add(new SelectListItem { Text = "15", Value = "15" });
            oMinuto.Add(new SelectListItem { Text = "16", Value = "16" });
            oMinuto.Add(new SelectListItem { Text = "17", Value = "17" });
            oMinuto.Add(new SelectListItem { Text = "18", Value = "18" });
            oMinuto.Add(new SelectListItem { Text = "19", Value = "19" });
            oMinuto.Add(new SelectListItem { Text = "20", Value = "20" });
            oMinuto.Add(new SelectListItem { Text = "21", Value = "21" });
            oMinuto.Add(new SelectListItem { Text = "22", Value = "22" });
            oMinuto.Add(new SelectListItem { Text = "23", Value = "23" });
            oMinuto.Add(new SelectListItem { Text = "24", Value = "24" });
            oMinuto.Add(new SelectListItem { Text = "25", Value = "25" });
            oMinuto.Add(new SelectListItem { Text = "26", Value = "26" });
            oMinuto.Add(new SelectListItem { Text = "27", Value = "27" });
            oMinuto.Add(new SelectListItem { Text = "28", Value = "28" });
            oMinuto.Add(new SelectListItem { Text = "29", Value = "29" });
            oMinuto.Add(new SelectListItem { Text = "30", Value = "30" });
            oMinuto.Add(new SelectListItem { Text = "31", Value = "31" });
            oMinuto.Add(new SelectListItem { Text = "32", Value = "32" });
            oMinuto.Add(new SelectListItem { Text = "33", Value = "33" });
            oMinuto.Add(new SelectListItem { Text = "34", Value = "34" });
            oMinuto.Add(new SelectListItem { Text = "35", Value = "35" });
            oMinuto.Add(new SelectListItem { Text = "36", Value = "36" });
            oMinuto.Add(new SelectListItem { Text = "37", Value = "37" });
            oMinuto.Add(new SelectListItem { Text = "38", Value = "38" });
            oMinuto.Add(new SelectListItem { Text = "39", Value = "39" });
            oMinuto.Add(new SelectListItem { Text = "40", Value = "40" });
            oMinuto.Add(new SelectListItem { Text = "41", Value = "41" });
            oMinuto.Add(new SelectListItem { Text = "42", Value = "42" });
            oMinuto.Add(new SelectListItem { Text = "43", Value = "43" });
            oMinuto.Add(new SelectListItem { Text = "44", Value = "44" });
            oMinuto.Add(new SelectListItem { Text = "45", Value = "45" });
            oMinuto.Add(new SelectListItem { Text = "46", Value = "46" });
            oMinuto.Add(new SelectListItem { Text = "47", Value = "47" });
            oMinuto.Add(new SelectListItem { Text = "48", Value = "48" });
            oMinuto.Add(new SelectListItem { Text = "49", Value = "49" });
            oMinuto.Add(new SelectListItem { Text = "50", Value = "50" });
            oMinuto.Add(new SelectListItem { Text = "51", Value = "51" });
            oMinuto.Add(new SelectListItem { Text = "52", Value = "52" });
            oMinuto.Add(new SelectListItem { Text = "53", Value = "53" });
            oMinuto.Add(new SelectListItem { Text = "54", Value = "54" });
            oMinuto.Add(new SelectListItem { Text = "55", Value = "55" });
            oMinuto.Add(new SelectListItem { Text = "56", Value = "56" });
            oMinuto.Add(new SelectListItem { Text = "57", Value = "57" });
            oMinuto.Add(new SelectListItem { Text = "58", Value = "58" });
            oMinuto.Add(new SelectListItem { Text = "59", Value = "59" });

            ListaHora = oHora;
            ListaMinuto = oMinuto;

        }
        #endregion

    }
    public class AdministracionUsuario//SEGURIDAD
    {
        public IEnumerable<VI_PAR_USUARIO> oListaUsuarios { get; set; }
        public IEnumerable<aspnet_Roles> oListaRoles { get; set; }
        public IEnumerable<PR_PAR_MAN_SEG_LISTA_ROLESResult> oListaRolesFiltro { get; set; }
        public IEnumerable<VI_PAR_FUNCIONES> oListaFunciones { get; set; }
        public IEnumerable<SelectListItem> oListaSucursal {get;set;}
        public IEnumerable<SelectListItem> oListaSistema { get; set; }

        public int USU_RUT { get; set; }
        public bool BLOQUEA { get; set; }

        public string Sistema { get; set; }

        [Required]
        [Display(Name = "Rut")]
        [RegularExpression("\\d*[0-9]-([0-9]|k|K)$", ErrorMessage = "Campo Rut Formato incorrecto, ejemplo: 12345678-9")]
        [Remote("ValidarUsuario", "Validation")]
        public string UserName { get; set; }

        public string UserNameEdit { get; set; }
        
        [Required]
        [Display(Name = "Nombres")]
        public string Nombres { get; set; }

        [Required]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La contraseña debe ser de al menos 6 caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirme Contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas deben ser iguales.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Sucursal/CD")]        
        public string Sucursal { get; set; }

        public List<BoolSetting> Roles { get; set; }
        [Required]
        [Display (Name="Tipo Usuario")]
        public string tipoUsuario { get; set; }

        public string Empresa { get; set; }

        [Display (Name="Sucursal Empresa")]
        public string SucursalEmpresa { get; set; }
        
        
        public IEnumerable<SelectListItem> ListaTipoUsuario { get; set; }
        public IEnumerable<SelectListItem> ListaEmpresa { get; set; }
        public IEnumerable<SelectListItem> ListaSucursalEmpresa { get; set; }

        #region GetListaTipoUsuario
        public void GetListaTipoUsuario()
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oDatos = from tus in oPar.TB_PAR_TUS_TIPO_USUARIO
                         select new SelectListItem{
                           Text=tus.FL_PAR_TUS_NOMBRE,
                           Value=tus.PK_PAR_TUS_ID.ToString()                         
                         };
            ListaTipoUsuario = oDatos;
        }
        #endregion
        #region GetListaEmpresa
        public void GetListaEmpresa ()
        {
            LinqCLIDataContext oCli = new LinqCLIDataContext();
            var oDatos = from emp in oCli.TB_CLI_EMP_EMPRESAS
                         orderby emp.FL_CLI_EMP_FANTASIA
                         select new SelectListItem {
                             Text = emp.FL_CLI_EMP_FANTASIA.ToUpper(),
                            Value=emp.PK_CLI_EMP_RUT.ToString()
                         };
            ListaEmpresa = oDatos;
        }
        #endregion
        #region GetListaSucursal
        public void GetListaSucursalEmpresa()
        {
            int oEmpresa = 0;
            Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();
            if (oValidation.isDecimal(Empresa))
            {
                oEmpresa = Convert.ToInt32(Empresa);
            }

            LinqCLIDataContext oCli = new LinqCLIDataContext();
            var oDatos = from suc in oCli.TB_CLI_SUC_SUCURSAL
                         where suc.PK_CLI_EMP_RUT == oEmpresa
                         orderby suc.FL_CLI_SUC_NOMBRE
                         select new SelectListItem { 
                            Text=suc.FL_CLI_SUC_NOMBRE,
                            Value=suc.PK_CLI_SUC_ID.ToString()
                         };
            ListaSucursalEmpresa = oDatos;
        }
        #endregion
    }
    public class AdministracionCarpeta //MENU
    {

        //CARPETA

        [Required]
        [StringLength(30,ErrorMessage="Nombre carpeta largo máximo 30")]        
        [DataType(DataType.Text)]
        [Display(Name="Nombre Carpeta")]
        public string NombreCarpeta{get;set;}

        [Required]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "Campo Orden, debe ingresar solo números ")]        
        [Display(Name = "Orden")]
        public int Orden { get; set; }
        
        [Required]
        [StringLength(30, ErrorMessage = "clase largo máximo 30")]
        [Display(Name = "Modo Carpeta")]
        public string Clase { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "Nombre carpeta largo máximo 30")]
        [DataType(DataType.Text)]
        [Display(Name = "Nombre Sub Carpeta")]
        public string NombreSubCarpeta { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "Nombre Item largo máximo 30")]
        [DataType(DataType.Text)]
        [Display(Name = "Nombre Item")]
        public string NombreItem { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "Nombre Controlador largo máximo 30")]
        [DataType(DataType.Text)]
        [Display(Name = "Controlador")]
        public string Controlador { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "Nombre Acción largo máximo 30")]
        [DataType(DataType.Text)]
        [Display(Name = "Acción")]
        public string Accion { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "Nombre Icono largo máximo 30")]
        [DataType(DataType.Text)]
        [Display(Name = "Icono Item")]
        public string ClaseItem { get; set; }
        
        [Display(Name = "Carpeta (Opcional)")]       
        public string IdPadre { get; set; }

        
        [Display(Name = "Sub Carpeta (Opcional)")]       
        public string idSubCarpeta { get; set; }


        public int Tipo { get; set; } //1: Agregar Carpeta, 2: Editar Carpeta
        public int Id { get; set; }
        public int Sistema { get; set; }

        public int IdItem { get; set; }

        public IEnumerable<TB_PAR_CAR_CARPETA> oListaCarpetas { get; set; }
        public IEnumerable<VI_PAR_SUBCARPETA> oListaSubCarpetas { get; set; }
        public IEnumerable<VI_PAR_LISTA_ITEM> oListaItems { get; set; }
        public IEnumerable<VI_PAR_LIST_ITEM_FOR_TABLE> oListaItemsForTable { get; set; }
        public IEnumerable<SelectListItem> oListaCarpetasDropdown { get; set; }
        public IEnumerable<SelectListItem> oListaSubCarpetasDropdown { get; set; }
        public IEnumerable<List<BoolSetting>> oCkeckBoxRoles { get; set; }
        public IEnumerable<SelectListItem> oListaSistema { get; set; }

        public List<BoolSetting> Roles { get; set; }
        public string sSistema { get; set; }
        
    }
    //CLASE PARA EL CHECKBOX
    public class BoolSetting
    {
        public string DisplayName { get; set; }
        public bool Value { get; set; }
        public string Text { get; set; }
    }

    public class MyViewModel
    {
        public List<BoolSetting> Settings { get; set; }
    }

    public class AdministracionExcel
    {
        public IEnumerable<TB_PAR_COL_COLUMNA> oListaColumna{get;set;}

        public int Cliente { get; set; }
        public int Servicio { get; set; }

        public IEnumerable<SelectListItem> oListaCliente { get; set; }
        public List<ColumnasExcel> oListaSerCol { get; set; }
        public IEnumerable<SelectListItem> oServiceList { get; set; }
        public IEnumerable<SelectListItem> oListaLetra { get; set; }
        public IEnumerable<SelectListItem> oListaColumnasforDD { get; set; } //para dropdown
        public IEnumerable<SelectListItem> oListaObligatorioforDD { get; set; }

        [Required]               
        [Display(Name = "Letra")]
        [Remote("ValidarLetraColumnaServicio", "Validation", AdditionalFields = "ServicioRequerido")]
        public string Letra { get; set; }

        [Required]
        [Display(Name = "Columna")]
        [Remote("ValidarColumnaServicio", "Validation", AdditionalFields = "ServicioRequerido")]
        public string Columna { get; set; }

        [Required (ErrorMessage="Debe seleccionar si la columna será obligatoria")]
        [Display(Name = "Obligatoria")]        
        public string Obligatoria { get; set; }

        public IEnumerable<SelectListItem> ListaLetraCustom { get; set; }
        public IEnumerable<SelectListItem> ListaColumnaCustom { get; set; }

        public void GetListaCliente()
        {
            LinqCLIDataContext oCli = new LinqCLIDataContext();
            var oDatos = from vi in oCli.TB_CLI_EMP_EMPRESAS
                         orderby vi.FL_CLI_EMP_RAZON_SOCIAL
                         select new SelectListItem
                         {
                             Text=vi.FL_CLI_EMP_RAZON_SOCIAL.ToUpper(),
                             Value=vi.PK_CLI_EMP_RUT.ToString()
                         };
            oListaCliente = oDatos;
        }
        public void GetListaServicio(int RutCliente)
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oServicios = from ser in oPar.TB_PAR_SER_SERVICIO
                             join ems in oPar.TB_PAR_EMS_EMPRESA_SERVICIO on ser.PK_PAR_SER_ID equals ems.PK_PAR_SER_ID
                             where ser.PK_PAR_EST_ID == 1  && ems.PK_CLI_EMP_RUT==RutCliente
                             orderby ser.FL_PAR_SER_NOMBRE
                             select new SelectListItem
                             {
                                 Text = ser.FL_PAR_SER_NOMBRE.ToUpper(),
                                 Value = ser.PK_PAR_SER_ID.ToString()
                             };
            oServiceList = oServicios;
        }
        public void GetListaLetra(int Servicio)
        {
            Courier.Controllers.AdministracionController oAdm = new Controllers.AdministracionController();

            List<idIntValorString> oLista = new List<idIntValorString>();
            oLista=oAdm.FunRetornaLetras();

            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oDatos = from pr in oPar.PR_PAR_EXC_LET_SER(Servicio)
                         join
                             letra in oLista on pr.PK_PAR_LET_ID equals letra.Id
                         select new SelectListItem
                         {
                             Text = letra.Valor,
                             Value = letra.Id.ToString()
                         };
            ListaLetraCustom = oDatos;

        }
        public void GetListaColumna(int Servicio)
        {                
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oDatos = from pr in oPar.PR_PAR_EXC_COL_SER(Servicio)                         
                         select new SelectListItem
                         {
                             Text = pr.FL_PAR_COL_NOMBRE,
                             Value = pr.PK_PAR_COL_ID.ToString()
                         };
            ListaColumnaCustom = oDatos;

        }
    }
    public class ColumnasExcel
    {
        public int Id { get; set; }
        public int Indice { get; set; }
        public string  Letra { get; set; }
        public string  Nombre { get; set; }
        public string  Descripcion { get; set; }
        public string  Obligatoria { get; set; }
    }
    public class idBoolValorString
    {
        public bool Id { get; set; }
        public string Valor { get; set; }
    }
    public class idIntValorString
    {
        public int Id { get; set; }
        public string Valor { get; set; }
    }

    public  class valoresListaComuna
    {
        public int comuna_id {get;set;}
        public string comuna_nombre{get;set;}
        public double comuna_latitud{get;set;}
        public double comuna_longitud{get;set;}
        public string comuna_provincia { get; set; }
        public string comuna_region { get; set; }
        public string clasificacion { get; set; }
    }
    public class ComunaModels
    {
        public IEnumerable<valoresListaComuna> ListaComunas { get; set; }
        public IEnumerable<valoresListaComuna> ListaComunasSC { get; set; }//Sin Clasificación
    }
    public class valoresListaLocalidades
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public double latitud { get; set; }
        public double longitud { get; set; }
        public string comuna { get; set; }
        public string provincia { get; set; }
        public string region { get; set; }
        public string codigo { get; set; }
    }
    public class LocalidadModels
    {
        public IEnumerable<valoresListaLocalidades> ListaLocalidades { get; set; }
    }

    #region CambiaEstadoBulto    

    public class CambiaEstadoBultoModels
    {
       

        public string Referencia1 { get; set; }
        public string Referencia2 { get; set; }

        [Required]
        public string Sucursal { get; set; }

        [Display(Name = "OT Detalle")]
        [Required]
        [Remote("vJsonValidarCodigoAnular", "Validation")]
        public string OT { get; set; }

        public VI_FOL_DATOS_ANULAR DatosAnular { get; set; }
        public IEnumerable<VI_FOL_VER_BULTO_ESTADO> ListaBultos { get; set; }
        public IEnumerable<TB_FOL_ESA_ESTADO_ALTER> ListaEsa { get; set; }
        public IEnumerable<TB_FOL_ERO_ESTADO_ROLL_BACK> ListaEro { get; set; }
        public IEnumerable<TB_FOL_EEX_ESTADO_EXCEPCION> ListaEEX { get; set; }
        public IEnumerable<SelectListItem> ListaSucursal { get; set; }

        public void GetDatosAnular(decimal OTP, decimal OTD)
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            var oDatos = (from x in oFol.VI_FOL_DATOS_ANULAR
                          where
                              x.PK_FOL_OTP_ID == OTP &&
                              x.PK_FOL_OTD_ID == OTD
                          select x).Single();
            DatosAnular = oDatos;

        }
        public void GetBultos(decimal OTP, decimal OTD)
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            var oDatos = from blt in oFol.VI_FOL_VER_BULTO_ESTADO                         
                         where blt.PK_FOL_OTP_ID == OTP
                         && blt.PK_FOL_OTD_ID == OTD
                         select blt;

            ListaBultos = oDatos;
        }
        public void GetReferencias(int idServicio)
        {
            Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();
            var oResult = oValidation.GetNombreReferencia(idServicio);
            Referencia1 = oResult.NombreRef1;
            Referencia2 = oResult.NombreRef2;
        }
        public void GetListaESA()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from esa in oFol.TB_FOL_ESA_ESTADO_ALTER
                         select esa;

            ListaEsa = oDatos;
        }
        public void GetListaERO()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from ero in oFol.TB_FOL_ERO_ESTADO_ROLL_BACK
                         select ero;

            ListaEro = oDatos;
        }
        public void GetListaEEX()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from eex in oFol.TB_FOL_EEX_ESTADO_EXCEPCION
                         select eex;

            ListaEEX = oDatos;
        }


        public int Bulto { get; set; }
        [Required]
        public string Estado { get; set; }

        public IEnumerable<SelectListItem> ListaEstado { get; set; }

        public void GetListaEstado()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from eex in oFol.TB_FOL_EEX_ESTADO_EXCEPCION
                         join est in oFol.TB_FOL_EST_ESTADO on eex.PK_FOL_EST_ID equals est.PK_FOL_EST_ID
                         select new SelectListItem
                         {
                             Text=est.FL_FOL_EST_NOMBRE,
                             Value=est.PK_FOL_EST_ID.ToString()
                         };
            ListaEstado = oDatos;
        }
        public void GetListaEstadoRoll()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from ero in oFol.TB_FOL_ERO_ESTADO_ROLL_BACK
                         join est in oFol.TB_FOL_EST_ESTADO on ero.PK_FOL_EST_ID equals est.PK_FOL_EST_ID
                         select new SelectListItem
                         {
                             Text = est.FL_FOL_EST_NOMBRE,
                             Value = est.PK_FOL_EST_ID.ToString()
                         };
            ListaEstado = oDatos;
        }
        public void GetListaSucursal()
        {
            LinqBD_PARDataContext oPar=new LinqBD_PARDataContext();
            var oDatos = from suc in oPar.TB_PAR_SUC_SUCURSAL
                         select new SelectListItem{
                            Text=suc.FL_PAR_SUC_NOMBRE,
                            Value=suc.PK_PAR_SUC_ID.ToString()
                         
                         };
            ListaSucursal = oDatos;
        }
    }
    #endregion




    #region ReenviarPedidoModels
    public class ReenviarPedidoModels:DevolucionModels
    {
        [Display (Name="Rut Cliente")]
        public string RutCliente { get; set; }
        public string Servicio { get; set; }

        [Display(Name = "Valor")]
        public string Referencia { get; set; }
        [Display(Name = "Tipo Referencia")]
        public string TipoReferencia { get; set; }

        public IEnumerable<SelectListItem> oListaServicios { get; set; }
        public IEnumerable<SelectListItem> ListaTipoReferencia { get; set; }

        public IEnumerable<VI_FOL_OTD_DATOS> oListaOTDVI { get; set; }

        #region ValidarRutCliente
        public RetornoModels ValidarRutCliente()
        {
            Courier.Controllers.ValidationController oValiadtion = new Controllers.ValidationController();
            RetornoModels oRetorno = new RetornoModels();
            if (RutCliente == "" || RutCliente==null)
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = "Rut Cliente en Blanco";
            }
            else if (oValiadtion.ValidarRut(RutCliente) == 1)
            {
                int Rut=Convert.ToInt32(RutCliente.Substring(0, RutCliente.Length - 2));
                LinqCLIDataContext oCli = new LinqCLIDataContext();
                var oEmp = from emp in oCli.TB_CLI_EMP_EMPRESAS
                           where emp.PK_CLI_EMP_RUT==Rut
                           select emp;
                if (oEmp.Count() > 0)
                {
                    oRetorno.Ok = true;                    
                }
                else
                {
                    oRetorno.Ok = false;
                    oRetorno.Mensaje = "No se encontró cliente en base de datos";
                }
                
            }
            else
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = "Rut Incorrecto";
            }

            return oRetorno;
        }
        #endregion
        #region ValidarTipoReferencia
        public RetornoModels ValidarTipoReferencia()
        {
            RetornoModels oRetorno = new RetornoModels();
            if (TipoReferencia == "" || TipoReferencia==null)
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = "Tipo Referencia en blanco";
            }
            else
            {
                oRetorno.Ok = true;
            }
            return oRetorno;
        }
        #endregion
        #region ValidarValorReferencia        
        public RetornoModels ValidarValorReferencia()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            RetornoModels oRetono = new RetornoModels();

            Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();

            oFol.PR_FOL_ULS_INSERT(Convert.ToInt32(RutCliente.Substring(0, RutCliente.Length - 2)), oValidation.GetRutActiveUser(), TipoReferencia);

            if (oValidation.IsNumeric2(TipoReferencia) == true)
            {
                int oTR = Convert.ToInt32(TipoReferencia);

                if (oTR == 1)//Documento
                {
                    if (oValidation.isDecimal(Referencia) == true)
                    {
                        var oDocList = from doa in oFol.TB_FOL_DOA_DOC_ASOCIADO
                                       where doa.PK_FOL_DOA_NUMERO == Convert.ToDecimal(Referencia)
                                            && doa.PK_PAR_TDO_ID == Convert.ToInt32(TipoReferencia)
                                            && doa.PK_CLI_EMP_RUT == Convert.ToInt32(RutCliente.Substring(0, RutCliente.Length - 2))
                                       select doa;
                        if (oDocList.Count() > 0)
                        {
                            var oLista = oDocList.ToList()[0];
                            oRetono.Ok = true;
                            oRetono.OTP = oLista.PK_FOL_OTP_ID;
                            oRetono.OTD = oLista.PK_FOL_OTD_ID;
                        }
                        else
                        {
                            oRetono.Ok = false;
                            oRetono.Mensaje = "Referencia no existe";
                        }
                    }
                    else 
                    {
                        oRetono.Ok = false;
                        oRetono.Mensaje = "Las referencias de documento tienen que ser de tipo número. Ej.: 1234";
                    }
                }
                else
                {
                    var oRefList = from rea in oFol.TB_FOL_REA_REF_ASOCIADO
                                   where rea.PK_FOL_REA_NUMERO == Referencia
                                        && rea.PK_PAR_TDO_ID == oTR
                                        && rea.PK_CLI_EMP_RUT == Convert.ToInt32(RutCliente.Substring(0, RutCliente.Length - 2))
                                   select rea;

                    if (oRefList.Count() > 0)
                    {                        
                        var oLista = oRefList.ToList()[0];
                        oRetono.Ok = true;
                        oRetono.OTP = oLista.PK_FOL_OTP_ID;
                        oRetono.OTD = oLista.PK_FOL_OTD_ID;
                    }
                    else
                    {
                        oRetono.Ok=false;
                        oRetono.Mensaje="Referencia no existe";
                    }
                }
            }
            else
            {
                var oOT = oValidation.TransformCodigoGenericoOTPOTD(Referencia);

                if (oOT.Error==1)
                {
                    oRetono.Ok=false;
                    oRetono.Mensaje=oOT.ErrorMensaje;
                }
                else
                {                   
                    oRetono.Ok = true;
                    oRetono.OTP = oOT.OTP;
                    oRetono.OTD = oOT.OTD;                    
                }
            }

            if (oRetono.Ok == true)
            {
                 var oConsulta = (from otd in oFol.TB_FOL_OTD_OT_DETALLE
                                    where otd.PK_FOL_OTP_ID == oRetono.OTP
                                    && otd.PK_FOL_OTD_ID == oRetono.OTD
                                    select otd).Single();
                if (oConsulta.PK_FOL_EST_ID == 10)
                {
                    oRetono.Ok = false;
                    oRetono.Mensaje = "La OT se encuentra Recepcionada OK";
                }                
            }
            return oRetono;

        }
        #endregion        
        #region R001_GetDatosOTD_DATOS
        public void R001_GetDatosOTD_DATOS()
        {
            try
            {
                Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();
                var oSucursal = oValidation.GetSucursalIDfromActiveUser();
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                var oDatos = from vi in oFol.VI_FOL_OTD_DATOS
                             where
                                 vi.PK_FOL_OTP_ID == OTP
                                 && vi.PK_FOL_OTD_ID == OTD
                             orderby vi.PK_FOL_OTP_ID descending
                             select vi;
                //var oDatos = (from pr in oFol.PR_FOL_OTD_DATOS_DEVOLUCION(oSucursal)
                //              where pr.PK_FOL_OTP_ID == OTP
                //              && pr.PK_FOL_OTD_ID == OTD
                //              select pr).ToList();
                oListaOTDVI = oDatos;
            }
            catch { }
        }
        #endregion

    }
    #endregion

    #region CrearOTManualModels
    public class ListaOTCreadas
    {
        public int Id { get; set; }
        public string CreadoPor { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int Cantidad { get; set; }
        public int PrimerOT { get; set; }
        public int UltimaOT { get; set; }
    }
    public class CrearOTManualModels
    {
        [Required]
        public string Cantidad { get; set; }
        public IEnumerable<SelectListItem> ListaCantidad { get; set; }
        public IEnumerable<VI_FOL_OMA_LISTA> ListaOMA { get; set; }

        public void GetListaCantidad()
        {
            List<SelectListItem> oLista = new List<SelectListItem>();

            oLista.Add(new SelectListItem {Text="100",Value="100"});
            oLista.Add(new SelectListItem { Text = "500", Value = "500" });
            oLista.Add(new SelectListItem { Text = "1000", Value = "1000" });
            oLista.Add(new SelectListItem { Text = "2000", Value = "2000" });
            oLista.Add(new SelectListItem { Text = "3000", Value = "3000" });
            oLista.Add(new SelectListItem { Text = "4000", Value = "4000" });
            oLista.Add(new SelectListItem { Text = "5000", Value = "5000" });
            oLista.Add(new SelectListItem { Text = "10000", Value = "10000" });

            ListaCantidad = oLista;
        }
        public ResultModels Guardar()
        {            
            ResultModels oResult = new ResultModels();
            try
            {
                Controllers.ValidationController oValidation = new Controllers.ValidationController();

                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

                System.Data.Linq.EntitySet<TB_FOL_ROO_RELACION_OTP_OMA> ListaROO = new System.Data.Linq.EntitySet<TB_FOL_ROO_RELACION_OTP_OMA>();
                int iCantidad= Convert.ToInt32(Cantidad);

                TB_FOL_OMA_OT_MANUAL OMA = new TB_FOL_OMA_OT_MANUAL
                {
                    FL_OMA_CANTIDAD = Convert.ToInt32(Cantidad),
                    FL_OMA_FECHA_CREACION = DateTime.Now,
                    PK_PAR_USU_RUT = oValidation.GetRutActiveUser()
                };
                

                for (var cc = 1; cc <= iCantidad; cc++)
                {

                    

                    TB_FOL_OTD_OT_DETALLE OTD = new TB_FOL_OTD_OT_DETALLE
                    {
                        PK_FOL_OTD_ID = 1,
                        PK_FOL_EST_ID = 27,
                        PK_PAR_USU_RUT = oValidation.GetRutActiveUser(),
                        PK_PAR_SUC_ID = oValidation.GetSucursalIDfromActiveUser()                        
                    };

                    System.Data.Linq.EntitySet<TB_FOL_OTD_OT_DETALLE> ListaOTD = new System.Data.Linq.EntitySet<TB_FOL_OTD_OT_DETALLE>();

                    ListaOTD.Add(OTD);

                    TB_FOL_OTP_OT_PRINCIPAL OTP = new TB_FOL_OTP_OT_PRINCIPAL
                    {
                        FL_FOL_OTP_FECHA_CREACION = DateTime.Now,
                        PK_CLI_EMP_RUT = 1,
                        PK_PAR_SER_ID = 1,
                        PK_PAR_SUC_ID = 1,
                        PK_FOL_EST_ID = 27,
                        PK_PAR_USU_RUT = oValidation.GetRutActiveUser(),
                        PK_FOL_TOT_ID = 3,
                        TB_FOL_OTD_OT_DETALLE = ListaOTD
                    };

                    TB_FOL_ROO_RELACION_OTP_OMA ROO = new TB_FOL_ROO_RELACION_OTP_OMA
                    {
                        TB_FOL_OMA_OT_MANUAL = OMA,
                        TB_FOL_OTP_OT_PRINCIPAL = OTP
                    };

                    ListaROO.Add(ROO);

                }
                oFol.TB_FOL_ROO_RELACION_OTP_OMA.InsertAllOnSubmit(ListaROO);
                oFol.SubmitChanges();   
                oResult.Ok = true;                
            }
            catch (Exception e)
            {
                oResult.Ok = false;
                oResult.Mensaje = e.Message;
            }
            return oResult;
        }
        public void GetListaOMA()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            var oDatos = from vi in oFol.VI_FOL_OMA_LISTA
                         orderby vi.FL_OMA_FECHA_CREACION descending
                         select vi;
            ListaOMA = oDatos;
        }
    }
    #endregion

    #region EliminarEstadoModels
    public class EliminarEstadoModels
    {
        [Required]
        [Remote("jSonValidarEstadoOT", "Validation")]
        public string OT { get; set; }

    }
    #endregion
  

    #region CambiaDireccionModels
        public class CambiaDireccionModels
        {
            [Display (Name="N° OT")]
            [Required]
            public string OT { get; set; }
        }
    #endregion

    #region RetornoDireccionEntregaModels
        public class RetornoDireccionEntregaModels
        {
            public string ot;

            [Required]
            [Display(Name = "Latitud OT")]
            public string lat_ot;

            [Required]
            [Display(Name = "Longitud OT")]
            public string lng_ot;

            [Required]
            [Display(Name = "Latitud Entrega")]
            public string lat_entrega;

            [Required]
            [Display(Name = "Longitud Entrega")]
            public string lng_entrega;

            public string ent_id;

            public string Lat_Entrega { get { return lat_entrega; } }
            public string Lng_Entrega { get { return lng_entrega; } }
            public string Lat_OT { get { return lat_ot; } }
            public string Lng_OT { get { return lng_ot; } }
            public string EntId { get { return ent_id; } }

            public RetornoDireccionEntregaModels(string lat_ot, string lng_ot, string lat_entrega, string lng_entrega, string ent_id)
            {
                this.ent_id = ent_id;
                this.lat_ot = lat_ot;
                this.lng_ot = lng_ot;
                this.lat_entrega = lat_entrega;
                this.lng_entrega = lng_entrega;
            }

        }
    #endregion

    #region SucursalesUsuarioModels
    public class SucursalesUsuarioModels
    {        
        public int Rut { get; set; }
        public int RutVE { get; set; }
        public int RutEmpresa { get; set; }

        
        [Display (Name="Tipo")]
        public string TipoSucursal { get; set; }
        public string Cliente { get; set; }

        public IEnumerable<SelectListItem> ListaTUS { get; set; }
        public IEnumerable<SelectListItem> ListaCLIENTE { get; set; }
        public IEnumerable<SelectListItem> ListaSUCURSAL { get; set; }

        [Required]
        public string Sucursal { get; set; }


        [Required]
        [Display (Name="Sucursal")]
        public string EditSucursal { get; set; }        
        [Display (Name="Cliente")]
        public string EditCliente { get; set; }
        [Display(Name = "Tipo")]
        public string EditTipoSucursal { get; set; }

        public IEnumerable<PR_PAR_SUCURSALES_USUARIOResult> oListaSucursales { get; set; }

        #region GetListaSucursales
        public void GetListaSucursales()
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oResult = from par in oPar.PR_PAR_SUCURSALES_USUARIO(Rut)
                          select par;
            oListaSucursales = oResult.ToList();
        }
        #endregion
        #region GetListaTipoSucursal
        public void GetListaTipoSucursal()
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            var oDatos = from tus in oPar.TB_PAR_TUS_TIPO_USUARIO
                         select new SelectListItem
                         {
                             Text = tus.FL_PAR_TUS_NOMBRE,
                             Value = tus.PK_PAR_TUS_ID.ToString()
                         };
            ListaTUS = oDatos;
        }
        #endregion
        #region GetListaCliente
        public void GetListaCliente()
        {
            if (TipoSucursal != "")
            {

                LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
                var oDatos = from pr in oPar.PR_PAR_CLIENTES_SUCURSALES(Convert.ToInt32(TipoSucursal))
                             select new SelectListItem
                             {
                                 Text = pr.FL_CLI_EMP_FANTASIA,
                                 Value = pr.PK_CLI_EMP_RUT.ToString()
                             };
                ListaCLIENTE = oDatos.ToList();
            }
            else
            {
                List<SelectListItem> oLst = new List<SelectListItem>();
                ListaCLIENTE = oLst;
            }
        }
        #endregion
        #region GetListaSucursalTipoUsr
        public void GetListaSucursalTipoUsr()
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            Controllers.ValidationController oValidation = new Controllers.ValidationController();


            int TUS =0;
            int EMP =0;

            if (oValidation.IsNumeric2(TipoSucursal))
                TUS=Convert.ToInt32(TipoSucursal);

            if (oValidation.IsNumeric2(Cliente))
                EMP=Convert.ToInt32(Cliente);

            var oDatos = from pr in oPar.PR_PAR_LISTA_SUCURSALES_EMP(EMP, TUS)
                         select new SelectListItem
                         {
                             Text = pr.FL_PAR_SUC_NOMBRE,
                             Value = pr.PK_PAR_SUC_ID.ToString()
                         };
            ListaSUCURSAL=oDatos.ToList();
        }
        #endregion
    }
    #endregion
    #region FormularioCambiaDireccionModels
    public class FormularioCambiaDireccionModels:DevolucionModels
    {
        public string Numero2 { get; set; }
        public IEnumerable<SelectListItem> ListaTDE { get; set; }
        public decimal DEN_ID { get; set; }

        [Required]
        [Display (Name="Tipo de Entrega")]
        public string TDE_ID { get; set; }

        #region GetDatosDireccion
        public void GetDatosDireccion()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            try
            {
                var oDen = (from pr in oFol.PR_FOL_DATOS_DIRECCION_ACTUAL_OT(OTP, OTD)
                            select pr).Single();

                DEN_ID = oDen.PK_FOL_DEN_ID;
                Region = oDen.PK_PAR_REG_ID.ToString();
                Comuna = oDen.PK_PAR_COM_ID.ToString();
                Localidad = oDen.PK_PAR_LOC_ID.ToString();
                Direccion = oDen.FL_FOL_DEN_DIRECCION;
                Numero = oDen.FL_FOL_DEN_NUMERO;
                Via = oDen.PK_PAR_VIA_ID.ToString();
                TDE_ID = oDen.PK_FOL_TDE_ID.ToString();

                var oTDE = from tde in oFol.TB_FOL_TDE_TIPO_DIRECCION_ENTREGA
                          select new SelectListItem
                          {
                              Text = tde.FL_FOL_TDE_NOMBRE,
                              Value = tde.PK_FOL_TDE_ID.ToString()
                          };
                ListaTDE = oTDE;
            }
            catch { }
        }
        #endregion
        
    }
    #endregion
    #region FormularioCS Cambio Servicio
    public class FormularioCSModels : DevolucionModels
    {
        [Required]
        public string Servicio { get; set; }
        public decimal csOTP { get; set; }
        public IEnumerable<SelectListItem> oListaServicio { get; set; }

        #region GetListaServicios
        public void GetListaServicios()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            try
            {
                
                var oCliente = (from otp in oFol.TB_FOL_OTP_OT_PRINCIPAL
                                where otp.PK_FOL_OTP_ID == OTP
                                select otp).Single();

                Servicio = oCliente.PK_PAR_SER_ID.ToString();

                var oServicios = from ser in oPar.TB_PAR_SER_SERVICIO
                                 join ems in oPar.TB_PAR_EMS_EMPRESA_SERVICIO
                                    on ser.PK_PAR_SER_ID equals ems.PK_PAR_SER_ID
                                 where
                                    ems.PK_CLI_EMP_RUT==oCliente.PK_CLI_EMP_RUT
                                 select new SelectListItem { 
                                     Text= ser.FL_PAR_SER_NOMBRE,
                                 Value=ser.PK_PAR_SER_ID.ToString()};
                oListaServicio = oServicios;
            }
            catch { }
        }
        #endregion

    }
    #endregion
    #region FormularioCS Cambio Servicio
    public class FormularioOBSModels
    {                
        public decimal obsOTP { get; set; }
        public decimal obsOTD { get; set; }

        [Display (Name="Observación")]
        public string obsObservacion { get; set; }
        #region GetDatosObservacion
        public void GetDatosObservacion()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from oca in oFol.TB_FOL_OCA_OBSERVACION_CARGA
                         where oca.PK_FOL_OTP_ID == obsOTP
                         && oca.PK_FOL_OTD_ID == obsOTD
                         select oca;
            if (oDatos.Count() > 0)
            {
                obsObservacion = oDatos.ToList().Take(1).Single().FL_FOL_OCA_OBSERVACION;
            }
            else
            {
                obsObservacion = "";
            }
        }
        #endregion
    }
    #endregion

    #region RestriccionesModels
    public class RestriccionesModels : CorreoModels
    {
        [Required]
        [Display (Name="Cliente")]
        public string resCliente { get; set; }

        [Required]
        [Display(Name = "Servicio")]
        public string resServicio { get; set; }

        public int USU_RUT { get; set; }

        public List<PR_FOL_RES_LISTA_RESTRICCIONESResult> ListaRestricciones { get; set; }
        
        #region GetListaRestricciones
        public void GetListaRestricciones()
        {
            LinqBD_FOLDataContext oFOL = new LinqBD_FOLDataContext();            
            ListaRestricciones = (from pr in oFOL.PR_FOL_RES_LISTA_RESTRICCIONES(USU_RUT)
                         select pr).ToList();
        }
        #endregion
        #region GuardaRestriccion
        public RetornoModels GuardaRestriccion()
        {
            RetornoModels oRetorno = new RetornoModels();

            try
            {
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                oFol.PR_FOL_RES_CREA_RESTRICCION(Convert.ToDecimal(resServicio), USU_RUT);
                oRetorno.Ok = true;
            }
            catch (Exception ex)
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = string.Format("Error: {0}", ex.Message);
            }
            return oRetorno;
        }
        #endregion
        #region EliminaRestriccion
        public RetornoModels EliminaRestriccion()
        {
            RetornoModels oRetorno = new RetornoModels();

            try
            {
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                oFol.PR_FOL_RES_BORRA_RESTRICCION(Convert.ToDecimal(resServicio), USU_RUT);
                oRetorno.Ok = true;
            }
            catch (Exception ex)
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = string.Format("Error: {0}", ex.Message);
            }
            return oRetorno;
        }
        #endregion
    }
    #endregion
}
