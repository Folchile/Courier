using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Courier.Models
{

    


    #region Mantencion Models
    public class MantencionModels
    {
        public IEnumerable<TB_CLI_EMP_EMPRESAS> oItems { get; set; }
        public IEnumerable<VI_PAR_SER_REF_LISTA> oServicio { get; set; }
        public IEnumerable<VerSucursalesCliente> LstaSucCltes { get; set; }
        public IEnumerable<SelectListItem> ListaDocumentos { get; set; }

        public bool PideHU { get; set; }
        public bool AlertaKilo { get; set; }

        [RegularExpression("\\d*[0-9]", ErrorMessage = "Sólo admite números enteros  ")]
        public string ValorAlertaKilo { get; set; }


        [Required]
        [RegularExpression("\\d*[0-9]-([0-9]|k|K)$", ErrorMessage = "Campo Rut Formato incorrecto, ejemplo: 12345678-9")]
        [Remote("ValidarRutSolo", "Validation")]
        [Display(Name = "Rut Cliente")]
        public string Rut { get; set; }

        [Display(Name = "Rut Cliente")]
        public string RutCliente { get; set; }

        [Required]
        [Display(Name = "Razón Social")]
        public string RazonSocial { get; set; }

        [Required]
        [Display(Name = "Nombre Fantasia")]
        public string NombreFantasia { get; set; }


        [Display(Name = "Giro Comercial")]
        public string GiroComercial { get; set; }



        public string RutClienteServicio { get; set; }

        [Required]
        public int Servicio { get; set; }

        [Required]
        [Display(Name = "Nombre Servicio")]
        public string NombreServicio { get; set; }

        public string TDO_ID { get; set; }

        public string Referencia1 { get; set; }
        public string Referencia2 { get; set; }

        [Display(Name = "Factor Volumetrico")]
        [Required]
        [RegularExpression("\\d*[0-9]", ErrorMessage = "Sólo admite números enteros  ")]
        [Range(1, 10000000)]
        public string FactorVolumetrico { get; set; }

        [Display(Name = "Pide KG.")]
        public bool PideKG { get; set; }

        [Display(Name = "Pide Dimensiones")]
        public bool PideDimensiones { get; set; }

        [Required]
        [RegularExpression("\\d*[0-9]", ErrorMessage = "Sólo admite números enteros  ")]
        [Display(Name = "Cantidad Max. Visitas")]
        public string CantidadVisitas { get; set; }

        [Required]
        [Display(Name = "Tipo Servicio")]
        public string TipoServicio { get; set; }

        [Required]
        [Display(Name = "Nombre Sucursal")]
        public string NombreSucursal { get; set; }

        [Required]
        [Display(Name = "Tipo Sucursal")]
        public int TipoSucursal { get; set; }


        public IEnumerable<SelectListItem> oListaTipoServicio { get; set; }


        public class VerSucursalesCliente
        {
            public int Sid { get; set; }
            public string Sdescripcion { get; set; }
            public string Stipo { get; set; }
        }


        public void GetListaSucCltes(int rtcl)
        {
            //int rtCli;
            //rtCli = Convert.ToInt32(Rut.Substring(0, Rut.Length - 2));
            var oDataContext = new LinqBD_FOLDataContext();
            var olistaCli = from m in oDataContext.PR_FOL_LISTA_SUCURSALES(rtcl)
                            select new VerSucursalesCliente
                          {
                              Sid=m.PK_PAR_SUC_ID,
                              Sdescripcion=m.FL_PAR_SUC_NOMBRE,
                              Stipo=m.FL_PAR_TUS_NOMBRE,
                          };
            LstaSucCltes = olistaCli.ToList();
        }


        public class Respuesta
        {
            public bool Ok { get; set; }
            public string Mensaje { get; set; }
        }

        #region AgregaClient
        public Respuesta AgregaClient()
        {
            int rtCli;
            string dig = "";
            rtCli = Convert.ToInt32(Rut.Substring(0, Rut.Length - 2));
            dig = Rut.Substring(Rut.Length - 1, 1);
            Respuesta oResultado = new Respuesta();
            try
            {
                Courier.Models.LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                oFol.PR_FOL_AGREGA_CLIENTE(rtCli, dig, RazonSocial, NombreFantasia, GiroComercial, 115);
                oResultado.Ok = true;
                oResultado.Mensaje = "<p>Registro Ingresado Exitosamente!</p>";
            }
            catch (Exception e)
            {

                oResultado.Ok = false;
                oResultado.Mensaje = "<p>Error al Actualizar Datos</p>" + e.Message;
            }
            return oResultado;
        }

        #endregion


        #region GetListaTipoServicio
        public void GetListaTipoServicio()
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oDatos = from tse in oPar.TB_PAR_TSE_TIPO_SERVICIO
                         select new SelectListItem
                         {
                             Text = tse.FL_PAR_TSE_NOMBRE,
                             Value = tse.PK_PAR_TSE_ID.ToString()
                         };
            oListaTipoServicio = oDatos;
        }
        #endregion

        #region GetListaServicio
        public void GetListaServicio()
        {

            int iRutCliente = 0;

            Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();
            if (oValidation.ValidarRut(RutCliente) == 1)
                iRutCliente = Convert.ToInt32(RutCliente.Substring(0, RutCliente.Length - 2));


            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oDatos = from ser in oPar.VI_PAR_SER_REF_LISTA
                         where ser.PK_CLI_EMP_RUT == iRutCliente
                         select ser;

            oServicio = oDatos;
        }
        #endregion
        #region GetListaDocumentos
        public void GetListaDocumentos(int TDO_ID) //TDO_ID, recibe el documento que se filtrará
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            var oTDO = from tdo in oPar.TB_PAR_TDO_TIPO_DOCUMENTO
                       where tdo.PK_PAR_TDO_ID != TDO_ID
                       select new SelectListItem
                       {
                           Text = tdo.FL_PAR_TDO_NOMBRE,
                           Value = tdo.PK_PAR_TDO_ID.ToString()
                       };

            ListaDocumentos = oTDO;
        }
        #endregion
        #region GetDatosDocumento
        public void GetDatosDocumento()
        {
            Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();

            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();


            var oDatos = (from ser in oPar.PR_PAR_SER_LISTA_SERV(Servicio)
                          select ser).ToList();

            if (oDatos.Count() > 0)
            {
                var oValor = oDatos.ToList()[0];

                Referencia1 = oValor.TDO_1.ToString();
                Referencia2 = oValor.TDO_2.ToString();

                FactorVolumetrico = oValor.FL_PAR_SER_FACTOR_VOLUMETRICO.ToString();

                PideKG = Convert.ToBoolean(oValor.FL_PAR_SER_TRABAJA_PESO_KG);
                PideDimensiones = Convert.ToBoolean(oValor.FL_PAR_SER_TRABAJA_PESO_VOL);

                NombreServicio = oValor.FL_PAR_SER_NOMBRE.Trim();

                CantidadVisitas = oValor.FL_PAR_SER_CANTIDAD_VISITAS.ToString();
                TipoServicio = oValor.PK_PAR_TSE_ID.ToString();

                bool HU = false;
                bool KILO = false;

                if (oValor.FL_PAR_SER_PIDE_HU == true)
                    HU = true;
                if (oValor.FL_PAR_SER_ALERTA_KILO == true)
                    KILO = true;


                PideHU = HU;
                AlertaKilo = KILO;
                ValorAlertaKilo = oValor.FL_PAR_SER_ALERTA_VALOR.ToString();

            }
        }
        #endregion
        #region GuardarEditar
        public class ResultGuardarEditar
        {
            public bool Ok { get; set; }
            public string Mensaje { get; set; }
        }
        public ResultGuardarEditar GuardarEditar()
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            ResultGuardarEditar oResult = new ResultGuardarEditar();
            Courier.Controllers.ValidationController oValidation = new Controllers.ValidationController();

            int iReferencia1 = 0;
            int iCantidadVisitas = 0;
            if (oValidation.IsNumeric2(Referencia1) == true)
                iReferencia1 = Convert.ToInt32(Referencia1);

            int iReferencia2 = 0;
            if (oValidation.IsNumeric2(Referencia2) == true)
                iReferencia2 = Convert.ToInt32(Referencia2);

            if (oValidation.IsNumeric2(CantidadVisitas))
            {
                iCantidadVisitas = Convert.ToInt32(CantidadVisitas);
            }

            int iTSE = Convert.ToInt32(TipoServicio);

            int ValorKilo = 0;
            if (oValidation.IsNumeric2(ValorAlertaKilo))
            {
                ValorKilo = Convert.ToInt32(ValorAlertaKilo);
            }


            try
            {
                oPar.PR_PAR_SER_INSERT_UPD(Servicio, NombreServicio, Convert.ToBoolean(PideKG), Convert.ToBoolean(PideDimensiones), Convert.ToInt32(FactorVolumetrico), iReferencia1, iReferencia2, iCantidadVisitas, iTSE, Convert.ToBoolean(PideHU), Convert.ToBoolean(AlertaKilo), ValorKilo);
                oResult.Ok = true;
                oResult.Mensaje = "Ok";
            }
            catch (Exception e)
            {
                oResult.Ok = false;
                oResult.Mensaje = "<p>No fue posible guardar la información</p>" + e.Message;
            }
            return oResult;
        }
        #endregion
    }
    #endregion

    #region Lista Cobertrua
    public class ListaCobertura
    {
        public string NombreComuna { get; set; }
        public string NombreRegion { get; set; }
        public bool Lunes { get; set; }
        public bool Martes { get; set; }
        public bool Miercoles { get; set; }
        public bool Jueves { get; set; }
        public bool Viernes { get; set; }
        public bool Sabado { get; set; }
        public bool Domingo { get; set; }
    }
    #endregion

    #region Lista Sucursal
    public class ListaSucursal
    {
        public string NombreSucursal { get; set; }
        public int IdSucursal { get; set; }
        public double latitud { get; set; }
        public double longitud { get; set; }
        public int comunaId { get; set; }
    }
    #endregion

    #region Sucursal Models
    public class SucursalModels
    {
        public IEnumerable<ListaSucursal> oLista { get; set; }
        public IEnumerable<ListaCobertura> oCobertura { get; set; }
        public IEnumerable<ListaCobertura> oSinCobertura { get; set; }

    }
    #endregion

    #region Cobertura Models
    public class CoberturaModels
    {
        public IEnumerable<VI_PAR_COBERTURA> ListaCobertura { get; set; }
        public IEnumerable<SelectListItem> ListaRegiones { get; set; }
        public string Region { get; set; }
    }
    #endregion

    #region Entre Sucursales

    public class ListaCoberturaSucursal
    {
        public string Origen { get; set; }
        public string Destino { get; set; }
        public bool? Lunes { get; set; }
        public bool? Martes { get; set; }
        public bool? Miercoles { get; set; }
        public bool? Jueves { get; set; }
        public bool? Viernes { get; set; }
        public bool? Sabado { get; set; }
        public bool? Domingo { get; set; }
        public int Idcom { get; set; }
        public decimal Idloc { get; set; }
        public int TiempoEstimado { get; set; }
        public string Via { get; set; }
        public string Clasificacion { get; set; }

    }

    public class EntreSucursalesModels
    {
        public IEnumerable<ListaCoberturaSucursal> ListaCobertura { get; set; }
        public IEnumerable<ListaCoberturaSucursal> ListaCoberturaGeneral { get; set; }
        public int Origen { get; set; }
    }


    public class VerClasificaciones
    {
        public decimal Cid { get; set; }
        public string Cclasificacion { get; set; }
    }


    public class VerVias
    {
        public decimal Vid { get; set; }
        public string Vnombre { get; set; }
    }

    public class ListaCoberturaModels
    {

        public int IDCOM { get; set; }
        public int IDSUC { get; set; }
        public string SUCDESTINO { get; set; }


        [Required]
        [Display(Name = "Comuna")]
        public int Comuna { get; set; }

        [Required]
        [Display(Name = "Comuna")]
        public int ComunaLoc { get; set; }


        [Required]
        [Display(Name = "Localidad")]
        public int Localidad { get; set; }

        [Required]
        [Display(Name = "Localidad")]
        public string Locldad { get; set; }

        [Required]
        [Display(Name = "Comuna")]
        public string Comna { get; set; }


        [Required]
        [Display(Name = "Vía")]
        public decimal Via { get; set; }

        [Required]
        [Display(Name = "Vía")]
        public decimal ViaLoc { get; set; }

        [Required]
        [Display(Name = "Clasificación")]
        public string Clasificacion { get; set; }

        [Required]
        [Display(Name = "Clasificación")]
        public string ClasificacionLoc { get; set; }


        [Display(Name = "Sucursal Actual")]
        public string SucursalActual { get; set; }

        [Display(Name = "Sucursal Actual")]
        public string SucursalActualLoc { get; set; }


        [Display(Name = "Lunes")]
        public Boolean Lunes { get; set; }

        [Display(Name = "Lunes")]
        public Boolean LunesLoc { get; set; }

        [Display(Name = "Martes")]
        public Boolean Martes { get; set; }

        [Display(Name = "Martes")]
        public Boolean MartesLoc { get; set; }

        [Display(Name = "Miercoles")]
        public Boolean Miercoles { get; set; }

        [Display(Name = "Miercoles")]
        public Boolean MiercolesLoc { get; set; }

        [Display(Name = "Jueves")]
        public Boolean Jueves { get; set; }

        [Display(Name = "Jueves")]
        public Boolean JuevesLoc { get; set; }

        [Display(Name = "Viernes")]
        public Boolean Viernes { get; set; }

        [Display(Name = "Viernes")]
        public Boolean ViernesLoc { get; set; }

        [Display(Name = "Sabado")]
        public Boolean Sabado { get; set; }

        [Display(Name = "Sabado")]
        public Boolean SabadoLoc { get; set; }


        [Display(Name = "Domingo")]
        public Boolean Domingo { get; set; }

        [Display(Name = "Domingo")]
        public Boolean DomingoLoc { get; set; }


        [Required]
        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        [Display(Name = "Tiempo Estimado hrs.")]
        public decimal TiempoEstimado { get; set; }

        [Required]
        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        [Display(Name = "Tiempo Estimado hrs.")]
        public decimal TiempoEstimadoLoc { get; set; }

        public IEnumerable<SelectListItem> ListaVias { get; set; }
        public IEnumerable<SelectListItem> ListaComunas { get; set; }
        public IEnumerable<SelectListItem> ListaLocalidadesCob { get; set; }
        //public IEnumerable<VerVias> ListaVias { get; set; }
        public IEnumerable<VerClasificaciones> ListaClasificaciones { get; set; }
        public IEnumerable<ListaCoberturaSucursal> ListaCobertura { get; set; }
        public IEnumerable<ListaCoberturaSucursal> ListaCoberturaLocalidad { get; set; }
        public IEnumerable<SelectListItem> ListaSucursal { get; set; }
        public string ContainerId { get; set; }
        public string Sucursal { get; set; }


        public void GetListaClasificacion()
        {

            var oDataContext = new LinqBD_PARDataContext();
            var odetCla = from m in oDataContext.TB_PAR_CLA_CLASIFICACION
                          select new VerClasificaciones
                          {
                              Cid = m.PK_PAR_CLA_ID,
                              Cclasificacion = m.FL_PAR_CLA_NOMBRE,
                          };
            ListaClasificaciones = odetCla.ToList();
        }


        public void GetListaVias()
        {

            Courier.Models.LinqBD_PARDataContext vList = new LinqBD_PARDataContext();
            var vLista = from v in vList.TB_PAR_VIA_VIA
                         select new SelectListItem
                         {
                             Text = v.FL_PAR_VIA_NOMBRE,
                             Value = v.PK_PAR_VIA_ID.ToString(),
                         };
            ListaVias = vLista;
        }


        public void GetListaComunas(int tpo, int ID_suc)
        {

            LinqBD_FOLDataContext cList = new LinqBD_FOLDataContext();
            var cLista = from c in cList.PR_FOL_FIL_COM_LOC_COB(tpo, ID_suc, 0)
                         orderby c.NOMBRE
                         select new SelectListItem
                         {
                             Text = c.NOMBRE,
                             Value = c.ID.ToString(),
                         };
            ListaComunas = cLista;
        }


        public void GetListaLocalidadesCob(int ID_suc, int Com)
        {

            LinqBD_FOLDataContext cListloc = new LinqBD_FOLDataContext();
            var cListaloc = (from l in cListloc.PR_FOL_LISTA_LOCALID(2, ID_suc, Com)
                             orderby l.NOMBRE
                             select new SelectListItem
                             {
                                 Text = l.NOMBRE,
                                 Value = l.ID.ToString(),
                             }).ToList();
            ListaLocalidadesCob = cListaloc.ToList();
        }



        public Clases.Retorno UpdateCobertura(int tip)
        {
            Clases.Retorno oRetorno = new Clases.Retorno();
            try
            {
                LinqBD_FOLDataContext dCob = new LinqBD_FOLDataContext();
                if (tip == 1)
                {
                    dCob.PR_FOL_ACTUALIZA_COBERTURA(1, IDSUC, IDCOM, Convert.ToInt32(Via), Lunes, Martes, Miercoles, Jueves, Viernes, Sabado, Domingo, TiempoEstimado);
                }
                else
                {
                    dCob.PR_FOL_ACTUALIZA_COBERTURA(2, IDSUC, Localidad, Convert.ToInt32(ViaLoc), LunesLoc, MartesLoc, MiercolesLoc, JuevesLoc, ViernesLoc, SabadoLoc, DomingoLoc, TiempoEstimadoLoc);
                }
                oRetorno.Mensaje = "Datos Actualizados Exitosamente !";
                oRetorno.Ok = true;
            }
            catch (Exception e)
            {
                oRetorno.Mensaje = string.Format("Se ha producido un error: {0}", e.Message);
                oRetorno.Ok = false;
            }
            return oRetorno;
        }


        public Clases.Retorno ActualizaSucCobertura(int cob, int local)
        {
            Clases.Retorno oRetorno = new Clases.Retorno();
            try
            {
                LinqBD_FOLDataContext dCob = new LinqBD_FOLDataContext();
                if (cob == 1)
                {
                    dCob.PR_FOL_CAMBIA_COBERTURA(cob, Comuna, Convert.ToInt32(Sucursal), Lunes, Martes, Miercoles, Jueves, Viernes, Sabado, Domingo, Convert.ToInt32(TiempoEstimado), Convert.ToInt32(Via), local);
                }
                else
                {
                    dCob.PR_FOL_CAMBIA_COBERTURA(cob, Comuna, Convert.ToInt32(Sucursal), LunesLoc, MartesLoc, MiercolesLoc, JuevesLoc, ViernesLoc, SabadoLoc, DomingoLoc, Convert.ToInt32(TiempoEstimadoLoc), Convert.ToInt32(ViaLoc), local);
                }
                oRetorno.Mensaje = "Datos Actualizados Exitosamente !";
                oRetorno.Ok = true;
            }
            catch (Exception e)
            {
                oRetorno.Mensaje = string.Format("Se ha producido un error: {0}", e.Message);
                oRetorno.Ok = false;
            }
            return oRetorno;
        }


    }

    #endregion

    #region TransporteModels
    public class TransporteModels
    {

        IEnumerable<TB_PAR_TRA_TRANSPORTE> ListaTransporte { get; set; }

        public void GetListaTransporte()
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oDatos = from tra in oPar.TB_PAR_TRA_TRANSPORTE
                         select tra;

            ListaTransporte = oDatos;
        }
    }
    #endregion
}