using System;

using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace Courier.Models
{
    #region PreparacionModels
    public class PreparacionModels
    {
        public int? FactorVolumetrico { get; set; }
        public string NombreRef1 { get; set; }
        public string NombreRef2 { get; set; }

        public IEnumerable<TB_CLI_EMP_EMPRESAS> oItems { get; set; }
        public ColumnasParametricas oColumnas { get; set; } //Ver que columnas mostrar
        public ListaExcel DatosOT { get; set; }
        public IEnumerable<SelectListItem> ListaTipoReferencia { get; set; }
        public PR_FOL_VER_OT_GUARDADAResult OTGuardada { get; set; }
        public IEnumerable<VI_FOL_VER_BULTO_ESTADO> oListaBultos { get; set; }
        public IEnumerable<SelectListItem> ListaVia {get;set;}
        public IEnumerable<VI_FOL_OTD_BULTO> ListaOTDBulto { get; set; }
        public IEnumerable<HtmlString> ListaErrores { get; set; }

        [Required]
        [Display (Name="Buscar En")]
        public string TipoReferencia { get; set; }

        [Required]
        [Display (Name="Valor")]
        //[RegularExpression("\\d*[0-9]$", ErrorMessage = "N° Guía sólo se admiten Números")]
        public string Referencia { get; set; }

        [Required]
        [Remote("ValidarCliente", "Validation")]
        [RegularExpression("\\d*[0-9]-([0-9]|k|K)$", ErrorMessage = "Campo Rut Formato incorrecto, ejemplo: 12345678-9")]
        [Display (Name="Rut Cliente")]
        public string RutCliente { get; set; }

        public decimal idServicio { get; set; }

        public string RazonSocial { get; set; }
        public string NombreServicio { get; set; }

        [Required]
        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        [Display (Name="Ingrese total de bultos")]
        [Range(1, 1000, ErrorMessage = "Se pueden agregar entre 1 y 1000 bultos")]
        public string TotalBultos { get; set; }

        [Required]        
        [Display(Name = "Vía")]
        public string Via { get; set; }


        [Display (Name="Código")]
        public string CodigoBulto { get; set; }

        public string Peso { get; set; }
        public string Alto { get; set; }
        public string Largo { get; set; }
        public string Ancho { get; set; }


        public decimal OTP { get; set; }
        public decimal OTD { get; set; }

        public string NombreImpresora { get; set; }

        //Bulto

        [Required (ErrorMessage="El campo es obligatorio")]
        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        [Range(1, 100, ErrorMessage = "Se pueden agregar entre 1 y 100 bultos")]
        [Display(Name = "Ingrese bultos adicionales")]

        public decimal txtAgregarBulto { get; set; }

        public decimal ingOTP { get; set; }
        public decimal ingOTD { get; set; }
        
        [Display(Name = "Código")]
        [Required]
        //[Remote("vjSonValidarIdBultoenOTD", "Validation", AdditionalFields = "ingOTP, ingOTD")]
        public string ingIdBulto { get; set; }
        
        [Display(Name = "Peso (KG)")]
        [Required]
        [RegularExpression(@"^[0-9]*([\,][0-9]{1,5})?$", ErrorMessage = "Peso admite números con decimales separados por coma (Máx. 5 decimales)")]
        public string ingPeso { get; set; }

        [Required]
        [Display(Name = "Largo (CM)")]
        [RegularExpression(@"^[0-9]*([\,][0-9]{1,5})?$", ErrorMessage = "Largo admite números con decimales separados por coma (Máx. 5 decimales)")]
        public string ingLargo { get; set; }
        [Display(Name = "Alto (CM)")]

        [Required]
        [RegularExpression(@"^[0-9]*([\,][0-9]{1,5})?$", ErrorMessage = "Alto admite números con decimales separados por coma (Máx. 5 decimales)")]
        public string ingAlto { get; set; }

        [Required]
        [Display(Name = "Ancho (CM)")]
        [RegularExpression(@"^[0-9]*([\,][0-9]{1,5})?$", ErrorMessage = "Ancho admite números con decimales separados por coma (Máx. 5 decimales)")]
        public string ingAncho { get; set; }

        public decimal ingPesoVolumetrico { get; set; }

        public void GetNombreImpresora(int Rut)
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            try
            {
                var oNombre = (from x in oPar.TB_PAR_USU_USUARIO
                               where x.PK_PAR_USU_RUT == Rut
                               select x).Single();
                NombreImpresora = oNombre.FL_PAR_USU_NOMBRE_IMPRESORA;
            }
            catch
            {
                NombreImpresora = "";
            }
        }
        #region GetViaList
        public void GetViaList()
        {
            try
            {
                LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
                var oDatos = from via in oPar.TB_PAR_VIA_VIA
                             select new SelectListItem
                             {
                                 Value = via.FL_PAR_VIA_ABREVIATURA,
                                 Text = via.FL_PAR_VIA_NOMBRE
                             };
                ListaVia = oDatos;
            }
            catch { }
        }
        #endregion
        #region GetDatosByReferencia
        public void GetDatosByReferencia()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();

            oFol.PR_FOL_ULS_INSERT(Convert.ToInt32(RutCliente.Substring(0, RutCliente.Length - 2)), oValidation.GetRutActiveUser(), TipoReferencia);

            if (oValidation.IsNumeric2(TipoReferencia) == true)
            {
                var oTR = (from tdo in oPar.TB_PAR_TDO_TIPO_DOCUMENTO
                          where tdo.PK_PAR_TDO_ID == Convert.ToInt32(TipoReferencia)
                          select tdo).Single() ;

                if (oTR.PK_PAR_TRE_ID == 1)//Documento
                {
                    if (oValidation.isDecimal(Referencia) == true)
                    {
                        var oDocList = from doa in oFol.TB_FOL_DOA_DOC_ASOCIADO
                                       where doa.PK_FOL_DOA_NUMERO == Convert.ToDecimal(Referencia)
                                            && doa.PK_PAR_TDO_ID == Convert.ToInt32(TipoReferencia)
                                            && doa.PK_CLI_EMP_RUT==Convert.ToInt32(RutCliente.Substring(0, RutCliente.Length - 2))
                                       select doa;
                        if (oDocList.Count() > 0)
                        {
                            var oDoc = oDocList.ToList()[0];
                            var oDatos = (from otd in oFol.PR_FOL_VER_OT_GUARDADA(oDoc.PK_FOL_OTP_ID, oDoc.PK_FOL_OTD_ID)
                                         where                                        
                                         otd.EMP_RUT == Convert.ToInt32(RutCliente.Substring(0, RutCliente.Length - 2))
                                         select otd).ToList();
                            if (oDatos.Count() > 0)
                                OTGuardada = oDatos.ToList()[0];
                        }
                    }
                }
                else
                {
                    var oRefList = from rea in oFol.TB_FOL_REA_REF_ASOCIADO
                                where rea.PK_FOL_REA_NUMERO == Referencia
                                     && rea.PK_PAR_TDO_ID == Convert.ToInt32(TipoReferencia)
                                     && rea.PK_CLI_EMP_RUT == Convert.ToInt32(RutCliente.Substring(0, RutCliente.Length - 2))
                                select rea;

                    if (oRefList.Count() > 0)
                    {
                        var oRef = oRefList.ToList()[0];
                        var oDatos = (from otd in oFol.PR_FOL_VER_OT_GUARDADA(oRef.PK_FOL_OTP_ID,oRef.PK_FOL_OTD_ID)
                                     where                                     
                                     otd.EMP_RUT == Convert.ToInt32(RutCliente.Substring(0, RutCliente.Length - 2))
                                     select otd).ToList();
                        if (oDatos.Count() > 0)
                            OTGuardada = oDatos.ToList()[0];
                    }
                }
            }
            else
            {
                var oOT = oValidation.TransformCodigoGenericoOTPOTD(Referencia);

                var oDatos = (from otd in oFol.PR_FOL_VER_OT_GUARDADA(oOT.OTP,oOT.OTD)
                             where                            
                             otd.EMP_RUT == Convert.ToInt32(RutCliente.Substring(0, RutCliente.Length - 2))
                             select otd).ToList();
                if (oDatos.Count() > 0)
                    OTGuardada = oDatos.ToList()[0];
            }


            
        }
        #endregion
        #region GetDatosByOT
        public void GetDatosByOT()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = (from otd in oFol.PR_FOL_VER_OT_GUARDADA(OTP,OTD)                         
                         select otd).ToList();
            if (oDatos.Count() > 0)
                OTGuardada = oDatos.ToList()[0];
        }
        #endregion
        #region GetFactorVolumetrico
        public void GetFactorVolumetrico()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = (from ser in oFol.PR_FOL_PESO_VOL_OTP(OTP)                         
                         select ser.FL_PAR_SER_FACTOR_VOLUMETRICO).Single();
            FactorVolumetrico = oDatos;
        }
        #endregion
        #region GetBultos
        public int  GetBultos()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oBultos = from blt in oFol.TB_FOL_BLT_BULTO
                          where blt.PK_FOL_OTP_ID == OTP
                          && blt.PK_FOL_OTD_ID == OTD
                          select blt;
            return oBultos.Count();
        }
        #endregion
        #region GetListaBultos
        public void GetListaBultos()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oBultos = from blt in oFol.PR_FOL_VER_BULTO_ESTADO(OTP,OTD)                          
                          select new VI_FOL_VER_BULTO_ESTADO {
                            Expr1=blt.Expr1,
                            FL_FOL_BLT_ALTO=blt.FL_FOL_BLT_ALTO,
                            FL_FOL_BLT_ANCHO=blt.FL_FOL_BLT_ANCHO,
                            FL_FOL_BLT_FECHA_BULTO=blt.FL_FOL_BLT_FECHA_BULTO,
                            FL_FOL_BLT_LARGO=blt.FL_FOL_BLT_LARGO,
                            SUC_ACTUAL=blt.SUC_ACTUAL,
                            FL_FOL_BLT_PESO=blt.FL_FOL_BLT_PESO,
                            FL_FOL_BLT_PESO_VOLUMETRICO=blt.FL_FOL_BLT_PESO_VOLUMETRICO,
                            FL_FOL_EST_DESCRIPCION=blt.FL_FOL_EST_DESCRIPCION,
                            FL_FOL_EST_NOMBRE=blt.FL_FOL_EST_NOMBRE,
                            PK_FOL_BLT_ID=blt.PK_FOL_BLT_ID,
                            PK_FOL_EST_ID=blt.PK_FOL_EST_ID,
                            PK_FOL_OTD_ID=blt.PK_FOL_OTD_ID,
                            PK_FOL_OTP_ID=blt.PK_FOL_OTP_ID,
                            SUC_ORIGEN=blt.SUC_ORIGEN
                          };

            oListaBultos = oBultos.ToList();
        }
        #endregion
        #region SetTotalBultos
        public bool SetTotalBultos()
        {
            Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = (from otd in oFol.TB_FOL_OTD_OT_DETALLE
                          where otd.PK_FOL_OTD_ID == OTD
                          && otd.PK_FOL_OTP_ID == OTP
                          select otd).Single();
            try
            {
                oDatos.FL_FOL_OTD_BULTO = Convert.ToDecimal(TotalBultos);
                oDatos.FL_FOL_OTD_VIA = Via;
                oDatos.PK_PAR_USU_RUT = oValidation.GetRutActiveUser();
                oDatos.PK_PAR_SUC_ID = oValidation.GetSucursalIDfromActiveUser();

                oFol.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
            
        }
        #endregion
        #region AddTotalBultos
        public bool AddTotalBultos()
        {
            Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = (from otd in oFol.TB_FOL_OTD_OT_DETALLE
                          where otd.PK_FOL_OTD_ID == OTD
                          && otd.PK_FOL_OTP_ID == OTP
                          select otd).Single();
            try
            {
                oDatos.FL_FOL_OTD_BULTO += Convert.ToDecimal(txtAgregarBulto);
                oDatos.PK_PAR_USU_RUT = oValidation.GetRutActiveUser();
                oDatos.PK_PAR_SUC_ID = oValidation.GetSucursalIDfromActiveUser();
                oFol.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
            
        }
        #endregion
        #region DelBulto
        public bool DelBulto()
        {
            Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();            
            var oBulto = (from blt in oFol.TB_FOL_BLT_BULTO
                          where blt.PK_FOL_BLT_ID==Convert.ToDecimal(CodigoBulto)
                          select blt).Single();

            try
            {
                oBulto.PK_FOL_EST_ID = 13;
                oBulto.PK_PAR_SUC_ID = oValidation.GetSucursalIDfromActiveUser();
                oBulto.PK_PAR_USU_RUT = oValidation.GetRutActiveUser();
                oFol.SubmitChanges();
                return true;
            }
            catch 
            {
                return false;
            }

        }
        #endregion
        #region RecalcularTotal
        public bool RecalcularTotal()
        {
            Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oBulto = (from blt in oFol.TB_FOL_BLT_BULTO
                          where blt.PK_FOL_OTD_ID==OTD
                          && blt.PK_FOL_OTP_ID == OTP
                          && blt.PK_FOL_EST_ID!=13//Eliminado
                          select blt.PK_FOL_BLT_ID);//Cuento los elementos

            var oOTD = (from otd in oFol.TB_FOL_OTD_OT_DETALLE
                       where otd.PK_FOL_OTD_ID == OTD
                          && otd.PK_FOL_OTP_ID == OTP
                       select otd).Single();

            try
            {
                decimal oTotal = oBulto.Count();
                oOTD.FL_FOL_OTD_BULTO = oTotal;
                oOTD.PK_PAR_SUC_ID = oValidation.GetSucursalIDfromActiveUser();
                oOTD.PK_PAR_USU_RUT = oValidation.GetRutActiveUser();
                oFol.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }

        }
        #endregion
        #region CambiaViaOTD2
        public void CambiaViaOTD2()
        {
            try { 
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                var oOT = (from otd in oFol.TB_FOL_OTD_OT_DETALLE
                           where otd.PK_FOL_OTP_ID == OTP
                           && otd.PK_FOL_OTD_ID == OTD
                           select otd).Single();
                oOT.FL_FOL_OTD_VIA = Via;
                oFol.SubmitChanges();
            }
            catch { }
        }
        #endregion
        #region CrearBultosOTD
        public bool CrearBultosOTD(int oTipo)
        {
            Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            List<TB_FOL_BLT_BULTO> oBulto = new List<TB_FOL_BLT_BULTO>();
            int oIndice = 1;

            int oEstado = 11;

            if (oTipo == 2)
            {
                oEstado = 21;
            }

            while (oIndice <= Convert.ToInt32(TotalBultos))
            {
                oBulto.Add(new TB_FOL_BLT_BULTO { 
                    FL_FOL_BLT_FECHA_BULTO=DateTime.Now,
                    PK_FOL_OTD_ID=OTD,
                    PK_FOL_OTP_ID=OTP,
                    PK_FOL_EST_ID=oEstado,
                    PK_PAR_SUC_ID=oValidation.GetSucursalIDfromActiveUser(),
                    PK_PAR_USU_RUT=oValidation.GetRutActiveUser()
                });
                oIndice++;
            }
            oFol.TB_FOL_BLT_BULTO.InsertAllOnSubmit(oBulto);
            try
            {
                oFol.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
        #region AddBultosOTD
        public bool AddBultosOTD(int oTipo)
        {
            Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            List<TB_FOL_BLT_BULTO> oBulto = new List<TB_FOL_BLT_BULTO>();
            int oIndice = 1;

            int oEstado = 11;

            if (oTipo == 2)
            {
                oEstado = 21;
            }

            while (oIndice <= Convert.ToInt32(txtAgregarBulto))
            {
                oBulto.Add(new TB_FOL_BLT_BULTO
                {
                    FL_FOL_BLT_FECHA_BULTO = DateTime.Now,
                    PK_FOL_OTD_ID = OTD,
                    PK_FOL_OTP_ID = OTP,
                    PK_FOL_EST_ID = oEstado,
                    PK_PAR_SUC_ID=oValidation.GetSucursalIDfromActiveUser(),
                    PK_PAR_USU_RUT= oValidation.GetRutActiveUser()
                });
                oIndice++;
            }
            oFol.TB_FOL_BLT_BULTO.InsertAllOnSubmit(oBulto);
            try
            {
                oFol.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
        #region AddBultoConDatosOTD
        public bool AddBultoConDatosOTD(int oTipo)
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            List<TB_FOL_BLT_BULTO> oBulto = new List<TB_FOL_BLT_BULTO>();
            Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();

            var oPesoVolumetrico = 
                    (OTGuardada.FL_FOL_OTD_ALTO *
                    OTGuardada.FL_FOL_OTD_ANCHO *
                    OTGuardada.FL_FOL_OTD_LARGO) ;
            int Factor = Convert.ToInt32(OTGuardada.FL_PAR_SER_FACTOR_VOLUMETRICO);

            oPesoVolumetrico = oPesoVolumetrico / Factor;

            int oEstado = 11;

            if (oTipo == 2)
            {
                oEstado = 21;
            }
            

            oBulto.Add(new TB_FOL_BLT_BULTO
            {
                FL_FOL_BLT_FECHA_BULTO = DateTime.Now,
                PK_FOL_OTD_ID = OTD,
                PK_FOL_OTP_ID = OTP,
                FL_FOL_BLT_ALTO=OTGuardada.FL_FOL_OTD_ALTO,
                FL_FOL_BLT_ANCHO=OTGuardada.FL_FOL_OTD_ANCHO,
                FL_FOL_BLT_LARGO=OTGuardada.FL_FOL_OTD_LARGO,
                FL_FOL_BLT_PESO=OTGuardada.PESO,
                PK_FOL_EST_ID = oEstado,
                FL_FOL_BLT_PESO_VOLUMETRICO=oPesoVolumetrico,
                PK_PAR_SUC_ID=oValidation.GetSucursalIDfromActiveUser(),
                PK_PAR_USU_RUT= oValidation.GetRutActiveUser()
            });
            
            oFol.TB_FOL_BLT_BULTO.InsertAllOnSubmit(oBulto);
            try
            {
                oFol.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
        #region GuardarBulto        
        public bool GuardarBulto()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();
            try
            {
                var oBulto = (from blt in oFol.TB_FOL_BLT_BULTO
                             where blt.PK_FOL_OTP_ID == ingOTP
                             && blt.PK_FOL_OTD_ID == ingOTD
                             && blt.PK_FOL_BLT_ID == Convert.ToDecimal(ingIdBulto)
                             select blt).Single();           

                float Alto = 0, Ancho = 0, Largo = 0, Peso = 0;


                if (oValidation.isDecimal(ingAlto))
                    Alto = Convert.ToSingle(ingAlto);
                if (oValidation.isDecimal(ingAncho))
                    Ancho = Convert.ToSingle(ingAncho);
                if (oValidation.isDecimal(ingLargo))
                    Largo = Convert.ToSingle(ingLargo);
                if (oValidation.isDecimal(ingPeso))
                    Peso = Convert.ToSingle(ingPeso);

                int oEstado = 12;
                if (oBulto.PK_FOL_EST_ID == 21)
                    oEstado = 22;


                oBulto.FL_FOL_BLT_ALTO = Alto;
                oBulto.FL_FOL_BLT_ANCHO = Ancho;
                oBulto.FL_FOL_BLT_LARGO = Largo;
                oBulto.FL_FOL_BLT_PESO = Peso;
                oBulto.PK_FOL_EST_ID = oEstado;
                oBulto.PK_PAR_SUC_ID = oValidation.GetSucursalIDfromActiveUser();
                oBulto.PK_PAR_USU_RUT = oValidation.GetRutActiveUser();                

                oBulto.FL_FOL_BLT_PESO_VOLUMETRICO = (Alto*Largo*Ancho)/FactorVolumetrico;//calcular            
            
                oFol.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
        #region ValidaOTPadreNHijos1Bulto
        public bool ValidaOTPadreNHijos1Bulto()
        {
            //La finalidad es poder determinar si un OTP tiene varios OTD, con 1 solo bulto
            //Esto significaria que se puede imprimir masivo por OTP

            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oListaOTD = from otd in oFol.TB_FOL_OTD_OT_DETALLE
                            where otd.PK_FOL_OTP_ID == OTP
                            select otd;
            if (oListaOTD.Count() == 1)//OT Padre con 1 OTD
                return false;

            foreach (var oFila in oListaOTD)
            {
                if (oFila.FL_FOL_OTD_BULTO != 1)
                    return false;
            }
            return true;
        }
        #endregion
        #region GetOTDBulto
        public void GetOTDBulto()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            try
            {
                ListaOTDBulto = from vi in oFol.VI_FOL_OTD_BULTO
                                where vi.PK_FOL_OTP_ID == OTP
                                orderby vi.PK_FOL_OTD_ID
                                select vi;
            }
            catch { }
        }
        #endregion        
        #region AddBultosMasivoOTP
        public void AddBultosMasivoOTP()
        {
            Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            try
            {
                oFol.PR_FOL_CREAR_BULTOS_OTP(OTP,oValidation.GetSucursalIDfromActiveUser(),oValidation.GetRutActiveUser());
            }
            catch
            {
            }
        }
        #endregion        
        #region GetListaVia
        public void GetListaVia()
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oVia = from x in oPar.TB_PAR_VIA_VIA
                       select new SelectListItem
                       {
                           Text=x.FL_PAR_VIA_NOMBRE,
                           Value=x.FL_PAR_VIA_ABREVIATURA
                       };
            ListaVia = oVia;
        }
        #endregion
        #region GetNombreReferencia
        public void GetNombreReferencia()
        {
            try
            {

                LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

                var oDatos = from x in oPar.TB_PAR_RES_REFERENCIA_SERVICIO
                             join y in oPar.TB_PAR_TDO_TIPO_DOCUMENTO on x.PK_PAR_TDO_ID equals y.PK_PAR_TDO_ID
                             where x.PK_PAR_SER_ID == idServicio
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
        #region CalculaElementosImpresionEtiqueta
        public BultoImpresion CalculaElementosImpresionEtiqueta()
        {            
            BultoImpresion oImpresion = new BultoImpresion
            {
                Bulto1=0,
                Bulto2=0,
                Bulto3=0,
                Bulto4=0,
                Cantidad=0,
                TipoEtiqueta=0
            };

            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            var oBultos = from blt in oFol.TB_FOL_BLT_BULTO
                          where blt.PK_FOL_OTP_ID == OTP
                                && blt.PK_FOL_OTD_ID == OTD
                                && blt.PK_FOL_EST_ID!=13 //Eliminado
                          orderby blt.PK_FOL_BLT_ID
                          select blt;
            decimal idBultoBuscado = Convert.ToDecimal(ingIdBulto);
            int oCount = 1;
            int CuentaTotal = 1;
            int oTotalBultos = oBultos.Count();
            int PrimeraEtiqueta = 1;
            foreach (var oFila in oBultos)
            {
                if (oCount == 1)
                {
                    oImpresion.Bulto1 = 0;
                    oImpresion.Bulto2 = 0;
                    oImpresion.Bulto3 = 0;
                    oImpresion.Bulto4 = 0;
                }
                if (PrimeraEtiqueta == 1 && oFila.PK_FOL_BLT_ID == idBultoBuscado)
                {
                    oImpresion.Cantidad = 1;
                    oImpresion.TipoEtiqueta = 1;
                    oImpresion.Bulto1 = idBultoBuscado;                    
                    return oImpresion;
                }
                else if (PrimeraEtiqueta==0)
                {
                    if (oImpresion.Bulto3 != 0) oImpresion.Bulto4 = oImpresion.Bulto3;                    
                    if (oImpresion.Bulto2 != 0) oImpresion.Bulto3 = oImpresion.Bulto2;
                    if (oImpresion.Bulto1 != 0) oImpresion.Bulto2 = oImpresion.Bulto1;
                    oImpresion.Bulto1 = oFila.PK_FOL_BLT_ID;
                    if (oFila.PK_FOL_BLT_ID == idBultoBuscado)
                    {
                        if (oCount == 4)
                        {
                            oImpresion.Cantidad = 4;
                            oImpresion.TipoEtiqueta = 2;                            
                            return oImpresion;
                        }
                        else if (CuentaTotal == oTotalBultos)
                        {
                            oImpresion.Cantidad = oCount;
                            oImpresion.TipoEtiqueta = 2;                            
                            return oImpresion;
                        }
                    }
                    oCount += 1;
                }
                CuentaTotal += 1;
                PrimeraEtiqueta = 0;
                if (oCount == 5)
                {
                    oCount = 1;
                }
            }


            return oImpresion;
        }
        #endregion
    }
    #endregion
    #region ImpresionModels
    public class ImpresionModels
    {
        public string idBulto { get; set; }
        public string OTP { get; set; }
        public string OTD { get; set; }
        public string REF1 { get; set; }
        public string REF2 { get; set; }
    }
    #endregion
    #region ListaDespacho
    public class ListaDespacho
    {
        public bool OK { get; set; }
        public string xn { get; set; }
        public decimal OTP { get; set; }
        public decimal OTD { get; set; }
        public decimal idBulto { get; set; }
        public string Estado { get; set; }
        public string EstadoDetalle { get; set; }
        public double Peso { get; set; }
        public double Largo { get; set; }
        public double Alto { get; set; }
        public double Ancho { get; set; }
        public double PesoVolumetrico { get; set; }
        public decimal NumeroManifiesto { get; set; }
        public int PK_FOL_EST_ID { get; set; }
        public int PK_PAR_RUT_USU { get; set; }
    }
    #endregion

    #region ListaDespachoOT
    public class ListaDespachoOT
    {
        //public bool OK { get; set; }
        public decimal OTP { get; set; }
        public decimal OTD { get; set; }
        public string OT { get; set; }
        public Nullable<decimal> Guia { get; set; }
        public int? Bultos { get; set; }
        public string Region { get; set; }
        public string Comuna { get; set; }
        public decimal NumeroManifiesto { get; set; }

    }
    #endregion

    #region DespachoModels
    public class DespachoModels
    {
        public bool optDespachoPorOT { get; set; }

        public VI_FOL_TRA_CON_MAN oDatosMan { get; set; }

        [Display (Name="N° Manifiesto")]
        [Required (ErrorMessage="Obligatorio")]
        public decimal NumeroManifiesto { get; set; }
        public decimal NumeroManifiesto2 { get; set; }
        public decimal EditNumeroManifiesto { get; set; }

        [Display(Name = "N° Manifiesto")]
        [Required(ErrorMessage = "Obligatorio")]
        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        [Remote("vJsonValidarExisteManifiestoId", "Validation")]
        public string NumeroBusquedaManifiesto { get; set; }                

        [Display (Name="Estado")]
        public string EstadoManifiesto { get; set; }
        public IEnumerable<SelectListItem> oListaPatentes { get; set; }
        public IEnumerable<SelectListItem> oListaProveedores { get; set; }
        public IEnumerable<SelectListItem> oListaArrastres { get; set; }
        public IEnumerable<SelectListItem> oListaConductores { get; set; }
        public IEnumerable<ListaDespacho> oListaDespacho { get; set; }
        public IEnumerable<PR_FOL_MAN_VISTA_RESUMEN_MANResult> oListaResumen { get; set; }

        [Display (Name="Tipo Manifiesto")]
        [Required]
        public string TipoManifiesto { get; set; }
        [Display(Name = "Sucursal Destino")]
        public string SucursalDestino { get; set; }

        public string cTipoManifiesto { get; set; }
        public string cSucursalDestino { get; set; }

        public string NombreTipoManifiesto { get; set; }
        public string NombreSucursalDestino { get; set; }

        public IEnumerable<SelectListItem> ListaTipoManifiesto { get; set; }
        public IEnumerable<SelectListItem> ListaSucursalDestino { get; set; }
        public IEnumerable<PR_FOL_MAN_LISTA_ABIERTOSResult> ListaManAbierto { get; set; }
        public IEnumerable<PR_FOL_MAN_LISTA_CERRADOS_HOYResult> ListaManCerradoHoy { get; set; }
        //Proveedor
        [Display(Name = "Proveedor")]
        public string Proveedor { get; set; }

        [Display(Name = "Arrastre")]
        public string Arrastre { get; set; }

        //Movil
        [Required]
        public string Patente { get; set; }

        [Required]
        public string Conductor { get; set; }

        
        [Display (Name="Móvil")]
        public string Movil { get; set; }

        [Display (Name="Cód. Bulto")]
        [Required]        
        public string idBulto { get; set; }

        //[Remote("vjSonValidarIdBultoDespacho", "Validation", AdditionalFields = "NumeroManifiesto")]
        public string idBultoVal { get; set; }

        [Display (Name="OT Princial")]
        public decimal OTP { get; set; }

        [Display(Name = "OT Detalle")]
        public decimal OTD { get; set; }

        public string OpcionOrden { get; set; }
        public IEnumerable<SelectListItem> ListOpcionOrden {get;set;}
        public IEnumerable<TB_FOL_EEX_ESTADO_EXCEPCION> ListaExx { get; set; }

        public string SucursalMenor { get; set; }
        public int idSucursalActual { get; set; }

        [Required]
        [Display (Name="N° OT")]
        public string AgregaOT { get; set; }
        
        public decimal AgregaOTMAN { get; set; }

        #region GetListaPatentes
        public void GetListaPatentes()
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            
            Controllers.ValidationController oValidation = new Controllers.ValidationController();

            var a = oValidation.GetSucursalIDfromActiveUser();


            var oDatos = from tra in oPar.TB_PAR_TRA_TRANSPORTE
                         join det in oPar.TB_PAR_DET_DESTINO_TRANSPORTE
                            on tra.PK_PAR_TRA_ID equals det.PK_PAR_TRA_ID
                         where det.PK_PAR_SUC_ID==oValidation.GetSucursalIDfromActiveUser()
                         select new SelectListItem
                         {
                             Text=tra.FL_PAR_TRA_PATENTE,
                             Value=tra.PK_PAR_TRA_ID.ToString()
                         };

            oListaPatentes = oDatos.Distinct();
        }
        #endregion

        #region GetListaProveedores
        public void GetListaProveedores()
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();


            Controllers.ValidationController oValidation = new Controllers.ValidationController();

            var a = oValidation.GetSucursalIDfromActiveUser();


            var oDatos = from PRO in oPar.TB_PAR_PRO_PROVEEDOR
                         join SPR in oPar.TB_PAR_SPR_SUCURSAL_PROVEEDOR
                            on PRO.PK_PAR_PRO_ID equals SPR.PK_PAR_PRO_ID
                         where SPR.PK_PAR_SUC_ID == oValidation.GetSucursalIDfromActiveUser()
                         select new SelectListItem
                         {
                             Text = PRO.FL_PAR_PRO_NOMBRE,
                             Value = PRO.PK_PAR_PRO_ID.ToString()
                         };

            oListaProveedores = oDatos.Distinct();
        }
        #endregion

        #region GetListaArrastres
        public void GetListaArrastres()
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();


            Controllers.ValidationController oValidation = new Controllers.ValidationController();

            var a = oValidation.GetSucursalIDfromActiveUser();


            var oDatos = from tra in oPar.TB_PAR_TRA_TRANSPORTE
                         join det in oPar.TB_PAR_DET_DESTINO_TRANSPORTE
                            on tra.PK_PAR_TRA_ID equals det.PK_PAR_TRA_ID
                         where det.PK_PAR_SUC_ID == oValidation.GetSucursalIDfromActiveUser()
                         select new SelectListItem
                         {
                             Text = tra.FL_PAR_TRA_PATENTE,
                             Value = tra.PK_PAR_TRA_ID.ToString()
                         };

            oListaArrastres = oDatos.Distinct();
        }
        #endregion

        #region GetListaConductores
        public void GetListaConductores()
        {
            List<SelectListItem> oLst = new List<SelectListItem>();

            oLst.Add(new SelectListItem {
                Text="-- Seleccione Conductor --",
                Value="",
                Selected=true
            });


            if (Patente != "" && Patente != null)
            {
                LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
                var oDatos = from con in oPar.TB_PAR_CON_CONDUCTOR
                             join trc in oPar.TB_PAR_TRC_TRA_CON on con.PK_PAR_CON_RUT equals trc.PK_PAR_CON_RUT
                             join tra in oPar.TB_PAR_TRA_TRANSPORTE on trc.PK_PAR_TRA_ID equals tra.PK_PAR_TRA_ID
                             where tra.PK_PAR_TRA_ID==Convert.ToInt32(Patente)
                             select new SelectListItem
                             {
                                 Text = con.FL_PAR_CON_NOMBRE,
                                 Value = con.PK_PAR_CON_RUT.ToString()
                             };
                oLst.AddRange(oDatos);
            }
            oListaConductores = oLst;
        }
        #endregion
        #region GetListaDespacho
        public void GetListaDespacho()
        {            
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from blt in oFol.PR_FOL_CONSULTA_BULTOS_MAN (NumeroManifiesto)                         
                         select new ListaDespacho
                         {
                             Alto = Convert.ToDouble(blt.FL_FOL_BLT_ALTO),
                             Ancho = Convert.ToDouble(blt.FL_FOL_BLT_ANCHO),
                             Estado = blt.FL_FOL_EST_NOMBRE,
                             EstadoDetalle=blt.FL_FOL_EST_DESCRIPCION,
                             idBulto = blt.PK_FOL_BLT_ID,
                             Largo = Convert.ToDouble(blt.FL_FOL_BLT_LARGO),
                             OK = blt.FL_FOL_MAB_OK,
                             Peso = Convert.ToDouble(blt.FL_FOL_BLT_PESO),
                             PesoVolumetrico = Convert.ToDouble(blt.FL_FOL_BLT_PESO_VOLUMETRICO),
                             xn = blt.Row + "/" + blt.FL_FOL_OTD_BULTO.ToString(),
                             OTD=blt.PK_FOL_OTD_ID,
                             OTP=blt.PK_FOL_OTP_ID,
                             NumeroManifiesto=blt.PK_FOL_MAN_ID,
                             PK_FOL_EST_ID=Convert.ToInt32(blt.PK_FOL_EST_ID)
                         };

            oListaDespacho = oDatos.ToList();
        }
        #endregion
        #region GetListaDespachoPorOT
        public bool Verifica(string a, string b)
        {
            if (a == b)
                return true;
            else
                return false;
        }
        public void GetListaDespachoPorOT()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from blt in oFol.VI_FOL_BULTOS_MAN_GROUP_OT
                         where blt.PK_FOL_MAN_ID == NumeroManifiesto
                         
                         select new ListaDespacho
                         {                             
                             OK = Verifica(blt.CANTIDAD_OK.ToString(),blt.FL_FOL_OTD_BULTO.ToString()),                             
                             xn = blt.CANTIDAD_OK + "/" + blt.FL_FOL_OTD_BULTO,
                             OTD = blt.PK_FOL_OTD_ID,
                             OTP = blt.PK_FOL_OTP_ID,
                             NumeroManifiesto = blt.PK_FOL_MAN_ID,
                             Peso=Convert.ToDouble(blt.PESO_KG),
                             PesoVolumetrico=Convert.ToDouble(blt.PESO_VOL)
                         };

            oListaDespacho = oDatos;
        }
        #endregion
        #region GetListaResumen
        public void GetListaResumen()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from pr in oFol.PR_FOL_MAN_VISTA_RESUMEN_MAN(NumeroManifiesto)                         
                         select pr;
            oListaResumen = oDatos.ToList();
        }
        #endregion
        #region CrearManifiesto
        
        public RetornoModels CrearManifiesto(int RutUsuario, int SucId, int tpm_id, int Suc_id)//Suc_id:0-->ruta cliente
        {

            RetornoModels oResult = new RetornoModels();

            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            Controllers.ValidationController oValidation = new Controllers.ValidationController();

            //var oConsultaMan = from man in oFol.TB_FOL_MAN_MANIFIESTO
            //                   where man.PK_PAR_USU_RUT == oValidation.GetRutActiveUser()
            //                   && man.PK_FOL_MAE_ID==1                               
            //                   select man;

            //if (oConsultaMan.Count() == 0)
            //{
                TB_FOL_MAN_MANIFIESTO oMan = new TB_FOL_MAN_MANIFIESTO
                {
                    FL_FOL_MAN_FECHA_CREACION = DateTime.Now,
                    PK_FOL_MAE_ID = 1,//Abierto
                    PK_PAR_SUC_ID = SucId,
                    PK_PAR_USU_RUT = RutUsuario,
                    PK_FOL_TPM_ID = tpm_id,
                    PK_FOL_ORI_ID = 1
                };

                if (Suc_id > 0)
                {
                    System.Data.Linq.EntitySet<TB_FOL_MAD_MAN_DESTINO> oDestino = new System.Data.Linq.EntitySet<TB_FOL_MAD_MAN_DESTINO>();
                    oDestino.Add(new TB_FOL_MAD_MAN_DESTINO
                    {
                        PK_PAR_SUC_ID = Suc_id
                    });
                    oMan.TB_FOL_MAD_MAN_DESTINO = oDestino;
                }

                try
                {
                    oFol.TB_FOL_MAN_MANIFIESTO.InsertOnSubmit(oMan);
                    oFol.SubmitChanges();
                    NumeroManifiesto = oMan.PK_FOL_MAN_ID;
                    oResult.Ok = true;                    
                    return oResult;
                }
                catch (Exception e)
                {
                    oResult.Ok = false;
                    oResult.Mensaje = "No fue posible crear el manifiesto " + e.Message;
                    return oResult;
                }
            //}
            //else
            //{
            //    oResult.Ok = false;
            //    oResult.Mensaje = "Ya existe un manifiesto creado para el usuario";
            //    return oResult;
            //}
        }
        #endregion
        #region AgregarBultoManifiesto
        public bool AgregarBultoManifiesto(decimal OTP_ID, decimal OTD_ID, decimal BLT_ID, decimal MAN_ID)
        {
            Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();
            try
            {
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                oFol.PR_FOL_AGREGA_BULTO_MAN(OTP_ID, OTD_ID, MAN_ID, BLT_ID,oValidation.GetSucursalIDfromActiveUser(),oValidation.GetRutActiveUser());
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
        #region GetOTPOTDfromBLT
        public void GetOTPOTDfromBLT(decimal BLT_ID)
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from blt in oFol.TB_FOL_BLT_BULTO
                         where blt.PK_FOL_BLT_ID == BLT_ID
                         select blt;
            if (oDatos.Count() > 0)
            {
                OTP = oDatos.ToList()[0].PK_FOL_OTP_ID;
                OTD = oDatos.ToList()[0].PK_FOL_OTD_ID;
            }
            else
            {
                OTP = 0;
                OTD = 0;
            }
        }
        #endregion
        #region DesasociarOT
        public bool DesasociarOT()
        {
            try
            {
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();
                oFol.PR_FOL_DESASOCIAR_OT(OTP, OTD, NumeroManifiesto, oValidation.GetSucursalIDfromActiveUser(), oValidation.GetRutActiveUser());
                return true;
            }
            catch
            {
                return false;
            }
            
        }
        #endregion
        #region GetOpcionesOrden
        public void GetOpcionesOrden()
        {
            List<SelectListItem> oLista = new List<SelectListItem>();
            oLista.Add(new SelectListItem {
                Text = "Ver todos los bultos",
                Value="1"            
            });
            oLista.Add(new SelectListItem
            {
                Text = "Ver Destinos",
                Value = "2"
            });
            oLista.Add(new SelectListItem
            {
                Text = "Agrupar por OT",
                Value = "3"
            });
            ListOpcionOrden = oLista;
        }
        #endregion
        #region CierraManifiesto
        public class ReturnManifiesto
        {
            public bool Ok { get; set; }
            public string Mensaje { get; set; }
        }
        public ReturnManifiesto CierraManifiesto()
        {
            ReturnManifiesto oResult = new ReturnManifiesto();
            try
            {
                int oRutCon = 0;
                Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();

                if (oValidation.isDecimal(Conductor))
                    oRutCon = Convert.ToInt32(Conductor);


                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                oFol.PR_FOL_MAN_CERRAR(NumeroManifiesto2, oRutCon, Convert.ToInt32(Patente), oValidation.GetSucursalIDfromActiveUser(), oValidation.GetRutActiveUser());
                oResult.Ok = true;
                oResult.Mensaje = "<p>Manifiesto Cerrado Exitosamente.</p>";                
            }
            catch(Exception e)
            {
                oResult.Ok = false;
                oResult.Mensaje = "<p>Error al cerrar el manifiesto</p>" + e.Message;
            }
            return oResult;
        }
        #endregion
        #region GetDatosManifiesto
        public void GetDatosManifiesto()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oResult = from datos in oFol.VI_FOL_TRA_CON_MAN
                          where datos.PK_FOL_MAN_ID == NumeroManifiesto
                          select datos;
            if (oResult.Count() > 0)
            {
                oDatosMan = oResult.ToList()[0];
            }
        }
        #endregion
        #region GetListaEXX
        public void GetListaEXX()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from eex in oFol.TB_FOL_EEX_ESTADO_EXCEPCION
                         select eex;
            ListaExx = oDatos;
        }
        #endregion
        #region GetlistaTipoManifiesto
        public void GetlistaTipoManifiesto()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from tpm in oFol.TB_FOL_TPM_TIPO_MANIFIESTO
                         select new SelectListItem { 
                            Text=tpm.FL_FOL_TPM_NOMBRE,
                            Value=tpm.PK_FOL_TPM_ID.ToString()
                         };
            ListaTipoManifiesto = oDatos;
        }
        #endregion
        #region GetlistaSucursalDestino
        public void GetlistaSucursalDestino()
        {
            Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oDatos = from suc in oPar.TB_PAR_SUC_SUCURSAL
                         where suc.PK_PAR_SUC_ID != oValidation.GetSucursalIDfromActiveUser()
                         select new SelectListItem
                         {
                             Text=suc.FL_PAR_SUC_NOMBRE,
                             Value=suc.PK_PAR_SUC_ID.ToString()
                         };
            ListaSucursalDestino = oDatos;
        }
        #endregion
        #region GetNombreDatosPrevios
        public void GetNombreDatosPrevios()
        {

            Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();
            NombreTipoManifiesto = "";
            NombreSucursalDestino = "";
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            var oDatos2 = from mab in oFol.TB_FOL_MAB_MANIFIESTO_BULTO
                         where mab.PK_FOL_MAN_ID == NumeroManifiesto
                         select mab;

            if (oDatos2.Count() > 0)
            {
                if (oValidation.IsNumeric2(TipoManifiesto) == true)
                {
                    var oDatos = from tpm in oFol.TB_FOL_TPM_TIPO_MANIFIESTO
                                 where tpm.PK_FOL_TPM_ID == Convert.ToInt32(TipoManifiesto)
                                 select tpm;
                    if (oDatos.Count() > 0)
                    {
                        var oDato = oDatos.ToList()[0];
                        NombreTipoManifiesto = oDato.FL_FOL_TPM_NOMBRE;
                    }
                }

                if (oValidation.IsNumeric2(SucursalDestino) == true)
                {
                    
                    NombreSucursalDestino = oValidation.GetSucursalNamefromIDSucursal(Convert.ToInt32(SucursalDestino));

                }

                if (NombreTipoManifiesto == "Ruta Cliente")
                {
                    SucursalMenor = "";
                }
            }
        }
        #endregion
        #region GetIdDatosPrevios
        public void GetIdDatosPrevios()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();
            var oDatos = from man in oFol.TB_FOL_MAN_MANIFIESTO
                         where man.PK_FOL_MAN_ID == NumeroManifiesto
                         select man;
            if (oDatos.Count() > 0)
            {
                var oDato = oDatos.ToList()[0];
                TipoManifiesto = oDatos.ToList()[0].PK_FOL_TPM_ID.ToString();
                if (oDatos.ToList()[0].TB_FOL_MAD_MAN_DESTINO.Count()>0)
                    SucursalDestino= oDatos.ToList()[0].TB_FOL_MAD_MAN_DESTINO.ToList()[0].PK_PAR_SUC_ID.ToString();
            }
            var oBultotake = from blt in oFol.TB_FOL_MAB_MANIFIESTO_BULTO
                              where blt.PK_FOL_MAN_ID == NumeroManifiesto
                              select blt;
            if (oBultotake.Count() > 0)
            {
                var oBultoTake1 = oBultotake.ToList()[0];

                var oSucListNew2 = from pr in oFol.PR_FOL_DEN_SUC_COBERTURA_BULTO(oBultoTake1.PK_FOL_BLT_ID)
                                   select pr;
                var oSucursalNew2 = oSucListNew2.ToList();
                SucursalMenor = oValidation.GetSucursalNamefromIDSucursal(oSucursalNew2[0].PK_PAR_SUC_ID);
            }
            else
            {
                SucursalMenor = "";
            }
        }
        #endregion
        #region GetListaManAbiertos
        public void GetListaManAbiertos()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            Controllers.ValidationController oValidation = new Controllers.ValidationController();

            var oDatos = (from pr in oFol.PR_FOL_MAN_LISTA_ABIERTOS(oValidation.GetRutActiveUser())
                         select pr).ToList();

            ListaManAbierto = oDatos;
        }
        #endregion
        #region GetListaManCerradosHoy
        public void GetListaManCerradosHoy()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            Controllers.ValidationController oValidation = new Controllers.ValidationController();

            var oDatos = (from pr in oFol.PR_FOL_MAN_LISTA_CERRADOS_HOY(oValidation.GetRutActiveUser())
                          select pr).ToList();

            ListaManCerradoHoy = oDatos;
        }
        #endregion
    }
    #endregion

    #region DespachoProveedorModels
    public class DespachoProveedorModels
    {
        //public bool optDespachoPorOT { get; set; }

        public VI_FOL_TRA_CON_MAN oDatosMan { get; set; }

        [Display(Name = "N° Manifiesto")]
        [Required(ErrorMessage = "Obligatorio")]
        public decimal NumeroManifiesto { get; set; }
        public decimal NumeroManifiesto2 { get; set; }
        public decimal EditNumeroManifiesto { get; set; }

        [Display(Name = "N° Manifiesto")]
        [Required(ErrorMessage = "Obligatorio")]
        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        [Remote("vJsonValidarExisteManifiestoId", "Validation")]
        public string NumeroBusquedaManifiesto { get; set; }

        [Display(Name = "Estado")]
        public string EstadoManifiesto { get; set; }
        public IEnumerable<SelectListItem> oListaPatentes { get; set; }
        public IEnumerable<SelectListItem> oListaProveedores { get; set; }
        public IEnumerable<SelectListItem> oListaArrastres { get; set; }
        public IEnumerable<SelectListItem> oListaConductores { get; set; }
        public IEnumerable<ListaDespacho> oListaDespacho { get; set; }

        public IEnumerable<ListaDespachoOT> oListaDespachoOt { get; set; }
        public IEnumerable<PR_FOL_MAN_VISTA_RESUMEN_MANResult> oListaResumen { get; set; }

        [Display(Name = "Tipo Manifiesto")]
        [Required]
        public string TipoManifiesto { get; set; }
        [Display(Name = "Sucursal Destino")]
        public string SucursalDestino { get; set; }

        public string cTipoManifiesto { get; set; }
        public string cSucursalDestino { get; set; }

        public string NombreTipoManifiesto { get; set; }
        public string NombreSucursalDestino { get; set; }

        public IEnumerable<SelectListItem> ListaTipoManifiesto { get; set; }
        public IEnumerable<SelectListItem> ListaSucursalDestino { get; set; }
        public IEnumerable<PR_FOL_MAN_LISTA_ABIERTOSResult> ListaManAbierto { get; set; }
        public IEnumerable<PR_FOL_MAN_LISTA_ABIERTOS_PROVEEDORResult> ListaManAbiertoProveedor { get; set; }
        public IEnumerable<PR_FOL_MAN_LISTA_CERRADOS_HOYResult> ListaManCerradoHoy { get; set; }
        public IEnumerable<PR_FOL_MAN_LISTA_CERRADOS_HOY_PROVEEDORResult> ListaManCerradoHoyProveedor { get; set; }
        //Proveedor
        [Display(Name = "Proveedor")]
        public string Proveedor { get; set; }

        [Display(Name = "Arrastre")]
        public string Arrastre { get; set; }

        //Movil
        [Required]
        public string Patente { get; set; }

        [Required]
        public string Courier { get; set; }

        [Required]
        public string Conductor { get; set; }


        [Display(Name = "Móvil")]
        public string Movil { get; set; }

        [Display(Name = "Cód. Bulto")]
        [Required]
        public string idBulto { get; set; }

        //[Remote("vjSonValidarIdBultoDespacho", "Validation", AdditionalFields = "NumeroManifiesto")]
        public string idBultoVal { get; set; }

        [Display(Name = "OT Princial")]
        public decimal OTP { get; set; }

        [Display(Name = "OT Detalle")]
        public decimal OTD { get; set; }

        public string OpcionOrden { get; set; }
        public IEnumerable<SelectListItem> ListOpcionOrden { get; set; }
        public IEnumerable<TB_FOL_EEX_ESTADO_EXCEPCION> ListaExx { get; set; }

        public string SucursalMenor { get; set; }
        public int idSucursalActual { get; set; }

        [Required]
        [Display(Name = "N° OT")]
        public string AgregaOT { get; set; }

        public decimal AgregaOTMAN { get; set; }

        #region GetListaPatentes
        public void GetListaPatentes()
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();


            Controllers.ValidationController oValidation = new Controllers.ValidationController();

            var a = oValidation.GetSucursalIDfromActiveUser();


            var oDatos = from tra in oPar.TB_PAR_TRA_TRANSPORTE
                         join det in oPar.TB_PAR_DET_DESTINO_TRANSPORTE
                            on tra.PK_PAR_TRA_ID equals det.PK_PAR_TRA_ID
                         where det.PK_PAR_SUC_ID == oValidation.GetSucursalIDfromActiveUser()
                         select new SelectListItem
                         {
                             Text = tra.FL_PAR_TRA_PATENTE,
                             Value = tra.PK_PAR_TRA_ID.ToString()
                         };

            oListaPatentes = oDatos.Distinct();
        }

        public void GetListaPatentesProveedor()
        {
            List<SelectListItem> oLst = new List<SelectListItem>();

            oLst.Add(new SelectListItem
            {
                Text = "-- Seleccione Patente --",
                Value = "",
                Selected = true
            });


            if (Proveedor != "" && Proveedor != null)
            {
                LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
                var oDatos = from tra in oPar.TB_PAR_TRA_TRANSPORTE
                             join pro in oPar.TB_PAR_TPR_TRANSPORTE_PROVEEDOR
                                on tra.PK_PAR_TRA_ID equals pro.PK_PAR_TRA_ID
                             where pro.PK_PAR_PRO_ID == Convert.ToInt32(Proveedor)
                             select new SelectListItem
                             {
                                 Text = tra.FL_PAR_TRA_PATENTE,
                                 Value = tra.PK_PAR_TRA_ID.ToString()
                             };

                oLst.AddRange(oDatos);
                //oListaPatentes = oDatos.Distinct();

            }

            oListaPatentes = oLst;
        }
        #endregion

        #region GetListaProveedores
        public void GetListaProveedores()
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();


            Controllers.ValidationController oValidation = new Controllers.ValidationController();

            var a = oValidation.GetSucursalIDfromActiveUser();


            var oDatos = from PRO in oPar.TB_PAR_PRO_PROVEEDOR
                         join SPR in oPar.TB_PAR_SPR_SUCURSAL_PROVEEDOR
                            on PRO.PK_PAR_PRO_ID equals SPR.PK_PAR_PRO_ID
                         where SPR.PK_PAR_SUC_ID == oValidation.GetSucursalIDfromActiveUser()
                         select new SelectListItem
                         {
                             Text = PRO.FL_PAR_PRO_NOMBRE,
                             Value = PRO.PK_PAR_PRO_ID.ToString()
                         };

            oListaProveedores = oDatos.Distinct();
        }
        #endregion

        #region GetListaArrastres
        public void GetListaArrastres()
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();


            Controllers.ValidationController oValidation = new Controllers.ValidationController();

            var a = oValidation.GetSucursalIDfromActiveUser();


            var oDatos = from tra in oPar.TB_PAR_TRA_TRANSPORTE
                         join det in oPar.TB_PAR_DET_DESTINO_TRANSPORTE
                            on tra.PK_PAR_TRA_ID equals det.PK_PAR_TRA_ID
                         where det.PK_PAR_SUC_ID == oValidation.GetSucursalIDfromActiveUser()
                         select new SelectListItem
                         {
                             Text = tra.FL_PAR_TRA_PATENTE,
                             Value = tra.PK_PAR_TRA_ID.ToString()
                         };

            oListaArrastres = oDatos.Distinct();
        }
        #endregion

        #region GetListaConductores
        public void GetListaConductores()
        {
            List<SelectListItem> oLst = new List<SelectListItem>();

            oLst.Add(new SelectListItem
            {
                Text = "-- Seleccione Conductor --",
                Value = "",
                Selected = true
            });


            if (Patente != "" && Patente != null)
            {
                LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
                var oDatos = from con in oPar.TB_PAR_CON_CONDUCTOR
                             join trc in oPar.TB_PAR_TRC_TRA_CON on con.PK_PAR_CON_RUT equals trc.PK_PAR_CON_RUT
                             join tra in oPar.TB_PAR_TRA_TRANSPORTE on trc.PK_PAR_TRA_ID equals tra.PK_PAR_TRA_ID
                             where tra.PK_PAR_TRA_ID == Convert.ToInt32(Patente)
                             select new SelectListItem
                             {
                                 Text = con.FL_PAR_CON_NOMBRE,
                                 Value = con.PK_PAR_CON_RUT.ToString()
                             };
                oLst.AddRange(oDatos);
            }
            oListaConductores = oLst;
        }
        #endregion
        #region GetListaDespacho
        public void GetListaDespacho()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from blt in oFol.PR_FOL_CONSULTA_BULTOS_MAN(NumeroManifiesto)
                         select new ListaDespacho
                         {
                             Alto = Convert.ToDouble(blt.FL_FOL_BLT_ALTO),
                             Ancho = Convert.ToDouble(blt.FL_FOL_BLT_ANCHO),
                             Estado = blt.FL_FOL_EST_NOMBRE,
                             EstadoDetalle = blt.FL_FOL_EST_DESCRIPCION,
                             idBulto = blt.PK_FOL_BLT_ID,
                             Largo = Convert.ToDouble(blt.FL_FOL_BLT_LARGO),
                             OK = blt.FL_FOL_MAB_OK,
                             Peso = Convert.ToDouble(blt.FL_FOL_BLT_PESO),
                             PesoVolumetrico = Convert.ToDouble(blt.FL_FOL_BLT_PESO_VOLUMETRICO),
                             xn = blt.Row + "/" + blt.FL_FOL_OTD_BULTO.ToString(),
                             OTD = blt.PK_FOL_OTD_ID,
                             OTP = blt.PK_FOL_OTP_ID,
                             NumeroManifiesto = blt.PK_FOL_MAN_ID,
                             PK_FOL_EST_ID = Convert.ToInt32(blt.PK_FOL_EST_ID)
                         };

            oListaDespacho = oDatos.ToList();
        }
        #endregion
        #region GetListaDespachoPorOT
        public bool Verifica(string a, string b)
        {
            if (a == b)
                return true;
            else
                return false;
        }
        public void GetListaDespachoPorOT()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from ot in oFol.PR_FOL_CONSULTA_OT_MAN(NumeroManifiesto)
                         select new ListaDespachoOT
                         {
                             OTD = ot.OTD,
                             OTP = ot.OTP,
                             OT = ot.OT,
                             NumeroManifiesto = ot.Numeromanifiesto,
                             Guia = ot.GUIA,
                             Comuna = ot.COMUNA,
                             Region = ot.REGION,
                             Bultos = ot.BULTOS
                         };

            oListaDespachoOt = oDatos.ToList();
        }

        #endregion
        #region GetListaResumen
        public void GetListaResumen()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from pr in oFol.PR_FOL_MAN_VISTA_RESUMEN_MAN(NumeroManifiesto)
                         select pr;
            oListaResumen = oDatos.ToList();
        }
        #endregion
        #region CrearManifiesto

        public RetornoModels CrearManifiesto(int RutUsuario, int SucId, int tpm_id, int Suc_id)//Suc_id:0-->ruta cliente
        {

            return CrearManifiesto(RutUsuario,  SucId,  tpm_id,  Suc_id, 1);
            //RetornoModels oResult = new RetornoModels();

            //LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            //Controllers.ValidationController oValidation = new Controllers.ValidationController();

            ////var oConsultaMan = from man in oFol.TB_FOL_MAN_MANIFIESTO
            ////                   where man.PK_PAR_USU_RUT == oValidation.GetRutActiveUser()
            ////                   && man.PK_FOL_MAE_ID==1                               
            ////                   select man;

            ////if (oConsultaMan.Count() == 0)
            ////{
            //TB_FOL_MAN_MANIFIESTO oMan = new TB_FOL_MAN_MANIFIESTO
            //{
            //    FL_FOL_MAN_FECHA_CREACION = DateTime.Now,
            //    PK_FOL_MAE_ID = 1,//Abierto
            //    PK_PAR_SUC_ID = SucId,
            //    PK_PAR_USU_RUT = RutUsuario,
            //    PK_FOL_TPM_ID = tpm_id
            //};

            //if (Suc_id > 0)
            //{
            //    System.Data.Linq.EntitySet<TB_FOL_MAD_MAN_DESTINO> oDestino = new System.Data.Linq.EntitySet<TB_FOL_MAD_MAN_DESTINO>();
            //    oDestino.Add(new TB_FOL_MAD_MAN_DESTINO
            //    {
            //        PK_PAR_SUC_ID = Suc_id
            //    });
            //    oMan.TB_FOL_MAD_MAN_DESTINO = oDestino;
            //}

            //try
            //{
            //    oFol.TB_FOL_MAN_MANIFIESTO.InsertOnSubmit(oMan);
            //    oFol.SubmitChanges();
            //    NumeroManifiesto = oMan.PK_FOL_MAN_ID;
            //    oResult.Ok = true;
            //    return oResult;
            //}
            //catch (Exception e)
            //{
            //    oResult.Ok = false;
            //    oResult.Mensaje = "No fue posible crear el manifiesto " + e.Message;
            //    return oResult;
            //}
            ////}
            ////else
            ////{
            ////    oResult.Ok = false;
            ////    oResult.Mensaje = "Ya existe un manifiesto creado para el usuario";
            ////    return oResult;
            ////}
        }

        public RetornoModels CrearManifiesto(int RutUsuario, int SucId, int tpm_id, int Suc_id, int origen)
        {

            RetornoModels oResult = new RetornoModels();

            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            Controllers.ValidationController oValidation = new Controllers.ValidationController();

            TB_FOL_MAN_MANIFIESTO oMan = new TB_FOL_MAN_MANIFIESTO
            {
                FL_FOL_MAN_FECHA_CREACION = DateTime.Now,
                PK_FOL_MAE_ID = 1,//Abierto
                PK_PAR_SUC_ID = SucId,
                PK_PAR_USU_RUT = RutUsuario,
                PK_FOL_TPM_ID = tpm_id,
                PK_FOL_ORI_ID = origen
                
            };

            if (Suc_id > 0)
            {
                System.Data.Linq.EntitySet<TB_FOL_MAD_MAN_DESTINO> oDestino = new System.Data.Linq.EntitySet<TB_FOL_MAD_MAN_DESTINO>();
                oDestino.Add(new TB_FOL_MAD_MAN_DESTINO
                {
                    PK_PAR_SUC_ID = Suc_id
                });
                oMan.TB_FOL_MAD_MAN_DESTINO = oDestino;
            }

            try
            {
                oFol.TB_FOL_MAN_MANIFIESTO.InsertOnSubmit(oMan);
                oFol.SubmitChanges();
                NumeroManifiesto = oMan.PK_FOL_MAN_ID;
                oResult.Ok = true;
                return oResult;
            }
            catch (Exception e)
            {
                oResult.Ok = false;
                oResult.Mensaje = "No fue posible crear el manifiesto " + e.Message;
                return oResult;
            }

        }



        #endregion
        #region AgregarBultoManifiesto
        public bool AgregarBultoManifiesto(decimal OTP_ID, decimal OTD_ID, decimal BLT_ID, decimal MAN_ID)
        {
            Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();
            try
            {
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                oFol.PR_FOL_AGREGA_BULTO_MAN(OTP_ID, OTD_ID, MAN_ID, BLT_ID, oValidation.GetSucursalIDfromActiveUser(), oValidation.GetRutActiveUser());
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
        #region GetOTPOTDfromBLT
        public void GetOTPOTDfromBLT(decimal BLT_ID)
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from blt in oFol.TB_FOL_BLT_BULTO
                         where blt.PK_FOL_BLT_ID == BLT_ID
                         select blt;
            if (oDatos.Count() > 0)
            {
                OTP = oDatos.ToList()[0].PK_FOL_OTP_ID;
                OTD = oDatos.ToList()[0].PK_FOL_OTD_ID;
            }
            else
            {
                OTP = 0;
                OTD = 0;
            }
        }
        #endregion
        #region DesasociarOT
        public bool DesasociarOT()
        {
            try
            {
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();
                oFol.PR_FOL_DESASOCIAR_OT(OTP, OTD, NumeroManifiesto, oValidation.GetSucursalIDfromActiveUser(), oValidation.GetRutActiveUser());
                return true;
            }
            catch
            {
                return false;
            }

        }
        #endregion
        #region GetOpcionesOrden
        public void GetOpcionesOrden()
        {
            List<SelectListItem> oLista = new List<SelectListItem>();
            oLista.Add(new SelectListItem
            {
                Text = "Ver todos los bultos",
                Value = "1"
            });
            oLista.Add(new SelectListItem
            {
                Text = "Ver Destinos",
                Value = "2"
            });
            oLista.Add(new SelectListItem
            {
                Text = "Agrupar por OT",
                Value = "3"
            });
            ListOpcionOrden = oLista;
        }
        #endregion

        #region CierraManifiesto
        public class ReturnManifiesto
        {
            public bool Ok { get; set; }
            public string Mensaje { get; set; }
        }
        public ReturnManifiesto CierraManifiesto()
        {
            ReturnManifiesto oResult = new ReturnManifiesto();
            try
            {
                int oRutCon = 0;
                Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();

                if (oValidation.isDecimal(Conductor))
                    oRutCon = Convert.ToInt32(Conductor);


                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                oFol.PR_FOL_MAN_CERRAR(NumeroManifiesto, oRutCon, Convert.ToInt32(Patente), oValidation.GetSucursalIDfromActiveUser(), oValidation.GetRutActiveUser());
                oResult.Ok = true;
                oResult.Mensaje = "<p>Manifiesto Cerrado Exitosamente.</p>";
            }
            catch (Exception e)
            {
                oResult.Ok = false;
                oResult.Mensaje = "<p>Error al cerrar el manifiesto</p>" + e.Message;
            }
            return oResult;
        }

        #endregion

        #region GuardarCabecera
        public class ReturnManifiestoGuardar
        {
            public bool OkGuardado { get; set; }
            public string MensajeGuardado { get; set; }
        }
        public ReturnManifiestoGuardar GuardarCabecera()
        {
            ReturnManifiestoGuardar oResult = new ReturnManifiestoGuardar();
            try
            {
                int oRutCon = 0;
                Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();

                if (oValidation.isDecimal(Conductor))
                    oRutCon = Convert.ToInt32(Conductor);


                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                oFol.PR_FOL_MAN_CERRAR(NumeroManifiesto2, oRutCon, Convert.ToInt32(Patente), oValidation.GetSucursalIDfromActiveUser(), oValidation.GetRutActiveUser());
                oResult.OkGuardado = true;
                oResult.MensajeGuardado = "<p>Manifiesto Guardado Exitosamente.</p>";
            }
            catch (Exception e)
            {
                oResult.OkGuardado = false;
                oResult.MensajeGuardado = "<p>Error al guardar los datos: </p>" + e.Message;
            }
            return oResult;
        }

        #endregion


        #region GetDatosManifiesto
        public void GetDatosManifiesto()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oResult = from datos in oFol.VI_FOL_TRA_CON_MAN
                          where datos.PK_FOL_MAN_ID == NumeroManifiesto
                          select datos;
            if (oResult.Count() > 0)
            {
                oDatosMan = oResult.ToList()[0];
            }
        }
        #endregion
        #region GetListaEXX
        public void GetListaEXX()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from eex in oFol.TB_FOL_EEX_ESTADO_EXCEPCION
                         select eex;
            ListaExx = oDatos;
        }
        #endregion
        #region GetlistaTipoManifiesto
        public void GetlistaTipoManifiesto()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from tpm in oFol.TB_FOL_TPM_TIPO_MANIFIESTO
                         select new SelectListItem
                         {
                             Text = tpm.FL_FOL_TPM_NOMBRE,
                             Value = tpm.PK_FOL_TPM_ID.ToString()
                         };
            ListaTipoManifiesto = oDatos;
        }
        #endregion
        #region GetlistaSucursalDestino
        public void GetlistaSucursalDestino()
        {
            Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oDatos = from suc in oPar.TB_PAR_SUC_SUCURSAL
                         where suc.PK_PAR_SUC_ID != oValidation.GetSucursalIDfromActiveUser()
                         select new SelectListItem
                         {
                             Text = suc.FL_PAR_SUC_NOMBRE,
                             Value = suc.PK_PAR_SUC_ID.ToString()
                         };
            ListaSucursalDestino = oDatos;
        }
        #endregion
        #region GetNombreDatosPrevios
        public void GetNombreDatosPrevios()
        {

            Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();
            NombreTipoManifiesto = "";
            NombreSucursalDestino = "";
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            var oDatos2 = from mab in oFol.TB_FOL_MAB_MANIFIESTO_BULTO
                          where mab.PK_FOL_MAN_ID == NumeroManifiesto
                          select mab;

            if (oDatos2.Count() > 0)
            {
                if (oValidation.IsNumeric2(TipoManifiesto) == true)
                {
                    var oDatos = from tpm in oFol.TB_FOL_TPM_TIPO_MANIFIESTO
                                 where tpm.PK_FOL_TPM_ID == Convert.ToInt32(TipoManifiesto)
                                 select tpm;
                    if (oDatos.Count() > 0)
                    {
                        var oDato = oDatos.ToList()[0];
                        NombreTipoManifiesto = oDato.FL_FOL_TPM_NOMBRE;
                    }
                }

                if (oValidation.IsNumeric2(SucursalDestino) == true)
                {

                    NombreSucursalDestino = oValidation.GetSucursalNamefromIDSucursal(Convert.ToInt32(SucursalDestino));

                }

                if (NombreTipoManifiesto == "Ruta Cliente")
                {
                    SucursalMenor = "";
                }
            }
        }
        #endregion
        #region GetIdDatosPrevios
        public void GetIdDatosPrevios()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();
            var oDatos = from man in oFol.TB_FOL_MAN_MANIFIESTO
                         where man.PK_FOL_MAN_ID == NumeroManifiesto
                         select man;
            if (oDatos.Count() > 0)
            {
                var oDato = oDatos.ToList()[0];
                TipoManifiesto = oDatos.ToList()[0].PK_FOL_TPM_ID.ToString();
                if (oDatos.ToList()[0].TB_FOL_MAD_MAN_DESTINO.Count() > 0)
                    SucursalDestino = oDatos.ToList()[0].TB_FOL_MAD_MAN_DESTINO.ToList()[0].PK_PAR_SUC_ID.ToString();
            }
            var oBultotake = from blt in oFol.TB_FOL_MAB_MANIFIESTO_BULTO
                             where blt.PK_FOL_MAN_ID == NumeroManifiesto
                             select blt;
            if (oBultotake.Count() > 0)
            {
                var oBultoTake1 = oBultotake.ToList()[0];

                var oSucListNew2 = from pr in oFol.PR_FOL_DEN_SUC_COBERTURA_BULTO(oBultoTake1.PK_FOL_BLT_ID)
                                   select pr;
                var oSucursalNew2 = oSucListNew2.ToList();
                SucursalMenor = oValidation.GetSucursalNamefromIDSucursal(oSucursalNew2[0].PK_PAR_SUC_ID);
            }
            else
            {
                SucursalMenor = "";
            }



        }
        #endregion
        #region GetListaManAbiertosProveedor 
        public void GetListaManAbiertosProveedor()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            Controllers.ValidationController oValidation = new Controllers.ValidationController();

            var oDatos = (from pr in oFol.PR_FOL_MAN_LISTA_ABIERTOS_PROVEEDOR(oValidation.GetRutActiveUser())
                          select pr).ToList();

            ListaManAbiertoProveedor = oDatos;
        }

        #endregion
        #region GetListaManCerradosHoyProveedor
        public void GetListaManCerradosHoyProveedor()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            Controllers.ValidationController oValidation = new Controllers.ValidationController();

            var oDatos = (from pr in oFol.PR_FOL_MAN_LISTA_CERRADOS_HOY_PROVEEDOR(oValidation.GetRutActiveUser())
                          select pr).ToList();

            ListaManCerradoHoyProveedor = oDatos;
        }
        #endregion
    }
    #endregion

    #region HistorialModels
    public class HistorialModels
    {
        public IEnumerable<VI_FOL_HIB_HISTORIA_BULTO> oListaHistoriaBulto { get; set; }
        public IEnumerable<VI_FOL_HID_HISTORIA_DETALLE> oListaHistoriaOTD { get; set; }
        public IEnumerable<VI_FOL_HIP_HISTORIA_PRINCIPAL> oListaHistoriaOTP { get; set; }
        public decimal OTD_ID { get; set; }
        public decimal OTP_ID { get; set; }
        public decimal BLT_ID { get; set; }

        #region GetHistoriaBulto
        public void GetHistoriaBulto()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var Datos = from hib in oFol.VI_FOL_HIB_HISTORIA_BULTO
                        where hib.PK_FOL_BLT_ID == BLT_ID
                        orderby hib.PK_FOL_HIB_ID descending
                        select hib;
            oListaHistoriaBulto = Datos;
        }
        #endregion
        #region GetHistoriaOTD
        public void GetHistoriaOTD()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var Datos = from hid in oFol.VI_FOL_HID_HISTORIA_DETALLE
                        where hid.PK_FOL_OTD_ID == OTD_ID && hid.PK_FOL_OTP_ID==OTP_ID
                        orderby hid.PK_FOL_HID_ID descending
                        select hid;
            oListaHistoriaOTD = Datos;
        }
        #endregion
        #region GetHistoriaOTP
        public void GetHistoriaOTP()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var Datos = from hip in oFol.VI_FOL_HIP_HISTORIA_PRINCIPAL
                        where hip.PK_FOL_OTP_ID == OTP_ID
                        orderby hip.PK_FOL_HIP_ID descending
                        select hip;
            oListaHistoriaOTP = Datos;
        }
        #endregion

    }
    #endregion
    #region DocumentosModels
    public class DocumentosModels
    {
        public decimal OTP { get; set; }
        public decimal OTD { get; set; }
        public decimal? ContraPago { get; set; }
        public string Edita { get; set; }
        public decimal? DOA_ID { get; set; }

        public bool EsAdmin { get; set; }

        public List<ElementDOA> ListaDocumentos { get; set; }


        //Formulario de ingreso
        
        [Required]
        public string Tipo { get; set; }

        [Required]
        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        [Remote("vJSonValidarNumeroDocumentoAsociado", "Validation", AdditionalFields = "OTP,OTD,Tipo")]
        public string Numero { get; set; }
        public int Estado { get; set; }

        public IEnumerable<SelectListItem> ListaTipo { get; set; }
        //---

        public class ElementDOA
        {
            public decimal? DOA_ID { get; set; }
            public DateTime Fecha { get; set; }
            public decimal Numero { get; set; }
            public string Nombre { get; set; }
            public decimal? ContraPago { get; set; }
        }


        #region GetEsadmin
        public void GetEsAdmin()
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            Controllers.ValidationController oValiadtion = new Controllers.ValidationController();
            ///EXEC PR_PAR_TIENE_ROL 15334940,'39072ee7-afb0-4fa9-84a1-5afc16a79816'
            var oCant = (from pr in oPar.PR_PAR_TIENE_ROL (oValiadtion.GetRutActiveUser(),"39072ee7-afb0-4fa9-84a1-5afc16a79816")
                        select pr).Single().TIENE_ROL;
            EsAdmin=false;
            if (oCant == 1)
            {
                EsAdmin = true;
            }
            

            if (EsAdmin == false)
            {
                Controllers.ValidationController oValidation = new Controllers.ValidationController();
                EsAdmin = oValidation.TieneRolByName("Puede agregar documentos");
            }

        }
        #endregion
        #region GetListaDocumentos
        public void GetListaDocumentos()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oResult=oFol.PR_FOL_DOA_CONSULTA(OTP, OTD);
            List<ElementDOA> lstDocs= new List<ElementDOA>();
            foreach (var oFila in oResult)
            {
                lstDocs.Add(new ElementDOA
                { 
                    Fecha=oFila.FL_FOL_DOA_FECHA,
                    Nombre=oFila.FL_PAR_TDO_NOMBRE,
                    Numero=oFila.PK_FOL_DOA_NUMERO,
                    ContraPago=oFila.FL_FOL_DOA_CONTRAPAGO,
                    DOA_ID=oFila.PK_FOL_DOA_ID
                });
            }
            ListaDocumentos = lstDocs;
        }
        #endregion
        #region AddDOA
        public bool AddDOA()
        {
            Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();
            try
            {
                if (oValidation.isDecimal(Numero) && oValidation.isDecimal(Tipo))
                {
                    int TipoDoc = Convert.ToInt32(Tipo);
                    decimal iContraPago = 0;
                    decimal DecNumero = Convert.ToDecimal(Numero);

                    if (ContraPago > 0)
                    {
                        iContraPago = Convert.ToDecimal(ContraPago);
                    }

                    if (DOA_ID == null) DOA_ID = 0;

                    LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                    oFol.PR_FOL_DOA_AGREGAR(OTP, OTD, TipoDoc, DecNumero, oValidation.GetRutActiveUser(), oValidation.GetSucursalIDfromActiveUser(),iContraPago, DOA_ID);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch {
                return false;
            }
        }
        #endregion
        #region GetListaTipo
        public void GetListaTipo()
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oDatos = from tdo in oPar.TB_PAR_TDO_TIPO_DOCUMENTO
                         select new SelectListItem
                         {
                             Text = tdo.FL_PAR_TDO_NOMBRE,
                             Value = tdo.PK_PAR_TDO_ID.ToString()
                         };
            ListaTipo = oDatos;
        }
        #endregion
        #region GetEstadoOT
        public void GetEstadoOT()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            Estado = (from otd in oFol.TB_FOL_OTD_OT_DETALLE
                         where otd.PK_FOL_OTP_ID==OTP
                         && otd.PK_FOL_OTD_ID==OTD
                         select otd.PK_FOL_EST_ID     
                              ).Single();
            
        }
        #endregion
    }
    #endregion
    #region DesasociarModels
    public class DesasociarModels
    {
        public decimal OTP { get; set; }
        public decimal OTD { get; set; }
        public decimal NumeroManifiesto { get; set; }
        public IEnumerable<ListaDespacho> oListaDespacho { get; set; }
        #region GetListaDespacho
        public void GetListaDespacho()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from blt in oFol.PR_FOL_BULTOS_MAN (OTP,OTD,NumeroManifiesto)
                         //where blt.PK_FOL_MAN_ID == NumeroManifiesto
                         //&& blt.PK_FOL_OTP_ID==OTP
                         //&& blt.PK_FOL_OTD_ID==OTD
                         select new ListaDespacho
                         {
                             Alto = Convert.ToDouble(blt.FL_FOL_BLT_ALTO),
                             Ancho = Convert.ToDouble(blt.FL_FOL_BLT_ANCHO),
                             Estado = blt.FL_FOL_EST_NOMBRE,
                             EstadoDetalle = blt.FL_FOL_EST_DESCRIPCION,
                             idBulto = blt.PK_FOL_BLT_ID,
                             Largo = Convert.ToDouble(blt.FL_FOL_BLT_LARGO),
                             OK = blt.FL_FOL_MAB_OK,
                             Peso = Convert.ToDouble(blt.FL_FOL_BLT_PESO),
                             PesoVolumetrico = Convert.ToDouble(blt.FL_FOL_BLT_PESO_VOLUMETRICO),
                             xn = blt.Row + "/" + blt.FL_FOL_OTD_BULTO.ToString(),
                             OTD = blt.PK_FOL_OTD_ID,
                             OTP = blt.PK_FOL_OTP_ID,
                             NumeroManifiesto=blt.PK_FOL_MAN_ID
                         };

            oListaDespacho = oDatos.ToList();
        }
        #endregion
    }
    #endregion
    #region CalculoDestinoModels
    public class CalculoDestinoModels
    {
        public List<Controllers.ValidationController.clsListaDestino> oListaDestino { get; set; }
        public int SucursalOrigen { get; set; }
        public int ComunaDestino { get; set; }
        public int SucursalDestinoManifiesto { get; set; }
        public int Patente { get; set; }

        public void GetDatosDestino(int Via)
        {
            Controllers.ValidationController oValidation = new Controllers.ValidationController();
            List<Controllers.ValidationController.clsListaDestino> oTemporal = new List<Controllers.ValidationController.clsListaDestino>();
            if (Via == 0)
            {
                oTemporal.AddRange(oValidation.GetListaOrigenDestino(SucursalOrigen, ComunaDestino, 1));
                oTemporal.AddRange(oValidation.GetListaOrigenDestino(SucursalOrigen, ComunaDestino, 2));
                oListaDestino = oTemporal;
                //SucursalDestinoManifiesto = oValidation.fnSucDestinoManifiesto(oListaDestino, Patente, Via);//ver..
            }
            else
            {
                oListaDestino = oValidation.GetListaOrigenDestino(SucursalOrigen, ComunaDestino, Via);
                SucursalDestinoManifiesto = oValidation.fnSucDestinoManifiesto(oListaDestino, Patente, Via);
            }
        }
    }
    #endregion
    #region FormularioHUModels
    public class FormularioHUModels
    {
        public decimal huOTP { get; set; }
        public decimal huOTD { get; set; }

        [Required]
        [Display (Name="HU")]
        public string huHU { get; set; }
    }
    #endregion
}