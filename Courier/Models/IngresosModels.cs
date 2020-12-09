using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using Courier.Models;



namespace MvcReporteViajes.Models
{
    public class IngresosModels
    {
        public IEnumerable<VerDetalleRuta> DetalleFolio { get; set; }

    }


    public class Prueba
    {
        public string InformeGastos { get; set; }
    }

    public class VerDetalleRuta
    {
        public int DRid { get; set; }
        public string DRfecha { get; set; }
        public string DROrigen { get; set; }
        public int DRKMInicial { get; set; }
        public string DRDestino { get; set; }
        public int DRKMFinal { get; set; }
        public int DRKMRecorridos { get; set; }
        public string DRObservacion { get; set; }
    }


    public class VerTodoslosAsistentes
    {
        public int Arut { get; set; }
        public string Adig { get; set; }
        public string Anombres { get; set; }
        public string Aapellidos { get; set; }
        public int Acodigo { get; set; }
        public string Aestado { get; set; }
        public string Asucursal { get; set; }
    }

    public class VerTodoslosDespachadores
    {
        public int Drut { get; set; }
        public string Ddig { get; set; }
        public string Dnombres { get; set; }
        public string Dapellidos { get; set; }
        public string Destado { get; set; }
        public string Dsucursal { get; set; }
    }


    public class VerTodoslosVehiculos
    {
        public int Vid { get; set; }
        public string Vpatente { get; set; }
        public string Vmodelo { get; set; }
        public string Vmarca { get; set; }
        public int Vcapkgs { get; set; }
        public int Vcapm3 { get; set; }
        public string Vsucursal { get; set; }
    }



    public class VerTodoslosConductores
    {
        public int Crut { get; set; }
        public string Cdig { get; set; }
        public string Cnombres { get; set; }
        public int Ccodigo { get; set; }
        public string Csucursal { get; set; }
        public decimal Cestado { get; set; }
    }


    public class VerTransAsoc
    {
        public int Tid { get; set; }
        public string Tpatente { get; set; }
        public string Tmarca { get; set; }
        public string Tmodelo { get; set; }
    }


    public class VerTransportes
    {
        public int id { get; set; }
        public string patente { get; set; }
        public string marca { get; set; }
        public string modelo { get; set; }
        public decimal capm3 { get; set; }
    }


    public class VerFolios
    {
        public int Fid { get; set; }
        public int Fdesde { get; set; }
        public int Fhasta { get; set; }
        public String FTrans { get; set; }
        public DateTime Ffecha { get; set; }
        public String Frut { get; set; }
    }


    public class VerFoliosActivos
    {
        public int Fact { get; set; }
        public String Fpatente { get; set; }
        public String Fusuario { get; set; }
        public String Fconductor { get; set; }
        public String Frampla { get; set; }
        public float Fktransp { get; set; }
        public int Fbtransp { get; set; }
    }


    public class VerFolioCambioestado
    {
        public int Fact { get; set; }
        public String Fpatente { get; set; }
        public String Fusuario { get; set; }
        public String Fconductor { get; set; }
        public String Frampla { get; set; }
        public float Fktransp { get; set; }
        public int Fbtransp { get; set; }
        public String Festado { get; set; }
        public String Fsucursal { get; set; }
        public int Fnumest { get; set; }
    }


    public class VerDetalleRgoPatentes
    {
        public int Ffolio { get; set; }
        public String Fpatente { get; set; }
        public String Frampla { get; set; }
        public String Fusuario { get; set; }
        public String Fconductor { get; set; }
        public float Fktransp { get; set; }
        public int Fbtransp { get; set; }
        public String Festado { get; set; }
        public String Fsucursal { get; set; }
    }



    public class VerRangoPatentes
    {
        public String Fpatente { get; set; }
        public int Fid { get; set; }
        public int Fdesde { get; set; }
        public int Fhasta { get; set; }
        public String Fusuario { get; set; }
    }


    public class VerCombustible
    {
        public int DCid { get; set; }
        public DateTime DCfecha { get; set; }
        public int DCNum { get; set; }
        public float DCLitros { get; set; }
        public int DCValor { get; set; }
        public string DCObservacion { get; set; }
    }

    public class VerAsistentes
    {
        public int DAid { get; set; }
        public int DAcodigo { get; set; }
        public string DAnombres { get; set; }
    }

    public class VerGastos
    {
        public int DGid { get; set; }
        public DateTime DGfecha { get; set; }
        public int DGpeaje { get; set; }
        public int DGestacionamiento{ get; set; }
        public int DGviatico { get; set; }
        public int DGalojamiento { get; set; }
        public int DGmantencion { get; set; }
        public int DGcargadescarga { get; set; }
        public int DGotros { get; set; }
        public int DGiva { get; set; }
        public int DGtotal { get; set; }
        public String DGobservacion { get; set; }
    }

    //public class VertotGrillagastos
    //{
    //    public int TFolio { get; set; }
    //    public int TPeaje { get; set; }
    //    public int TEstac { get; set; }
    //    public int TViat { get; set; }
    //    public int TAloj { get; set; }
    //    public int TMant { get; set; }
    //    public int TCarDes { get; set; }
    //    public int TOtros { get; set; }
    //    public int TIva { get; set; }
    //    public int TTotal { get; set; }
    //}


    public class VerRutas
    {
        public int DRID { get; set; }
        public DateTime DRFECHA { get; set; }
        public String DRRORIGEN { get; set; }
        public String DRCOMORIGEN { get; set; }
        public String DRLOCORIGEN { get; set; }
        public int DRKMINICIAL { get; set; }
        public String DRREGDESTINO { get; set; }
        public String DRCOMDESTINO { get; set; }
        public int DRLOCDESTINO { get; set; }
        public int DRKMFINAL { get; set; }
        public int DRKMRECORIDOS { get; set; }
        public int OBSERVACION { get; set; }
        public String NPEDIDO { get; set; }
        public String NTRANSPORTE { get; set; }
        public String NZONA { get; set; }
    }


    public class DetalleRutaModels
    {
        public int IDRTA { get; set; }

        public int FOLRTA { get; set; }

        [Required]
        [Display(Name = "Fecha")]
        public string FECHA { get; set; }
        //public DateTime FECHA { get; set; }

        [Required]
        [Display(Name = "Región Origen")]
        public int REGORIGEN { get; set; }

        [Required]
        [Display(Name = "Comuna Origen")]
        public int COMORIGEN { get; set; }

       
        [Display(Name = "Localidad Origen")]
        public int LOCORIGEN { get; set; }

        [Required]
        [Display(Name = "Dirección Origen")]
        public String DIRORIGEN { get; set; }

        [Required]
        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        [Display(Name = "km Inicial")]
        public int KMINICIAL { get; set; }

        [Required]
        [Display(Name = "Región Destino")]
        public int REGDESTINO { get; set; }

        [Required]
        [Display(Name = "Comuna Destino")]
        public int COMDESTINO { get; set; }

        [Display(Name = "Localidad Destino")]
        public int LOCDESTINO { get; set; }

        [Required]
        [Display(Name = "Dirección Destino")]
        public String DIRDESTINO { get; set; }

        [Required]
        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        [Display(Name = "Km Final")]
        public int KMFINAL { get; set; }

        [Required]
        [Display(Name = "Km Recorridos")]
        public int KMRECORIDOS { get; set; }


        [Display(Name = "Observación")]
        public string OBSERVACION { get; set; }

        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        [Display(Name = "Nro Transporte")]
        public string TRANSPORTE { get; set; }

        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        [Display(Name = "Lt.Transportados")]
        public string PEDIDO { get; set; }

        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        [Display(Name = "Zona")]
        public string ZONA { get; set; }
        

        //edicion
        [Required]
        [Display(Name = "Fecha")]
        public string EDFECHA { get; set; }
        //public DateTime FECHA { get; set; }

        [Required]
        [Display(Name = "Región Origen")]
        public int EDREGORIGEN { get; set; }

        [Required]
        [Display(Name = "Comuna Origen")]
        public int EDCOMORIGEN { get; set; }


        [Display(Name = "Localidad Origen")]
        public int EDLOCORIGEN { get; set; }

        [Required]
        [Display(Name = "Dirección Origen")]
        public String EDDIRORIGEN { get; set; }

        [Required]
        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        [Display(Name = "km Inicial")]
        public int EDKMINICIAL { get; set; }

        [Required]
        [Display(Name = "Región Destino")]
        public int EDREGDESTINO { get; set; }

        [Required]
        [Display(Name = "Comuna Destino")]
        public int EDCOMDESTINO { get; set; }

        [Display(Name = "Localidad Destino")]
        public int EDLOCDESTINO { get; set; }

        [Required]
        [Display(Name = "Dirección Destino")]
        public String EDDIRDESTINO { get; set; }

        [Required]
        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        [Display(Name = "Km Final")]
        public int EDKMFINAL { get; set; }

        [Required]
        [Display(Name = "Km Recorridos")]
        public int EDKMRECORIDOS { get; set; }


        [Display(Name = "Observación")]
        public string EDOBSERVACION { get; set; }

        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        [Display(Name = "Nro Transporte")]
        public string EDTRANSPORTE { get; set; }

        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        [Display(Name = "Lt.Transportados")]
        public string EDPEDIDO { get; set; }

        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        [Display(Name = "Zona")]
        public string EDZONA { get; set; }


        public IEnumerable<SelectListItem> ListaRegion { get; set; }
        public IEnumerable<SelectListItem> ListaComunasOrigen { get; set; }
        public IEnumerable<SelectListItem> oListaBlanco { get; set; }
        public IEnumerable<SelectListItem> ListaComOrigen { get; set; }
        public IEnumerable<SelectListItem> ListaComDestino { get; set; }



        public Respuesta GetModificaDetalleRuta()
        {
            Respuesta oResultado = new Respuesta();
            try
            {
                Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext oFol = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();
                oFol.PR_REP_MODIFICA_DETALLE_RUTAS(IDRTA, Convert.ToDateTime(EDFECHA), EDKMINICIAL, EDKMFINAL, EDOBSERVACION, EDREGORIGEN, EDCOMORIGEN, EDDIRORIGEN, EDREGDESTINO, EDCOMDESTINO, EDDIRDESTINO, Convert.ToDecimal(EDPEDIDO), Convert.ToDecimal(EDTRANSPORTE),Convert.ToDecimal(EDZONA));
                oResultado.Ok = true;
                oResultado.Mensaje = "<p>Registro Actualizado Exitosamente!</p>";
            }
            catch (Exception e)
            {

                oResultado.Ok = false;
                oResultado.Mensaje = "<p>Error al Actualizar Datos</p>" + e.Message;
            }
            return oResultado;
        }


        public void GetListaComDestino(int idD)
        {
            if (idD != 0)
            {
                LinqBD_REP_REPORTE_VIAJESDataContext oList = new LinqBD_REP_REPORTE_VIAJESDataContext();
                var oListaA = from a in oList.PR_REP_LISTA_COMUNAS(idD)
                              orderby a.FL_PAR_COM_NOMBRE
                              select new SelectListItem
                              {
                                  Text = a.FL_PAR_COM_NOMBRE,
                                  Value = a.PK_PAR_COM_ID.ToString(),
                              };
                ListaComDestino = oListaA;
            }
            else
            {
                List<SelectListItem> ol3 = new List<SelectListItem>();
                ListaComDestino = ol3;
            }
        }

        public void GetListaComOrigen(int idR)
        {
            if (idR != 0)
            {
                LinqBD_REP_REPORTE_VIAJESDataContext oList = new LinqBD_REP_REPORTE_VIAJESDataContext();
                var oListaA = from a in oList.PR_REP_LISTA_COMUNAS(idR)
                              orderby a.FL_PAR_COM_NOMBRE
                              select new SelectListItem
                              {
                                  Text = a.FL_PAR_COM_NOMBRE,
                                  Value = a.PK_PAR_COM_ID.ToString(),
                              };
                ListaComOrigen = oListaA;
            }
            else
            {
                List<SelectListItem> ol2 = new List<SelectListItem>();
                ListaComOrigen = ol2;
            }
        }



        public void GesListaRegion()
        {
            Courier.Models.LinqBD_PARDataContext rList = new Courier.Models.LinqBD_PARDataContext();
            var rLista = from r in rList.TB_PAR_REG_REGION
                         orderby r.PK_PAR_REG_ID
                         select new SelectListItem
                         {
                             Text = r.FL_PAR_REG_NOMBRE,
                             Value = r.PK_PAR_REG_ID.ToString(),
                         };
            ListaRegion = rLista;
        }

        public void GesListaComunas(int idC)
        {
            Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext cList = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();
            var cLista = from c in cList.VI_REP_PROVINCIA_COMUNA
                         orderby c.FL_PAR_COM_NOMBRE
                         where c.PK_PAR_REG_ID == idC
                         select new SelectListItem
                         {
                             Text = c.FL_PAR_COM_NOMBRE,
                             Value = c.PK_PAR_COM_ID.ToString(),
                         };
            ListaComunasOrigen = cLista;
        }


        #region GetListaBlanco
        public void GetListaBlanco()
        {
            List<SelectListItem> oLista = new List<SelectListItem>();
            oListaBlanco = oLista;
        }
        #endregion


        public class Respuesta
        {
            public bool Ok { get; set; }
            public string Mensaje { get; set; }
        }



        public Respuesta AgregaDetRuta()
        {
            Respuesta oResultado = new Respuesta();
            try
            {
                Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext oFol = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();
                oFol.PR_REP_AGREGA_DETALLE_RUTA(FOLRTA,Convert.ToDateTime(FECHA),KMINICIAL,KMFINAL, OBSERVACION,REGORIGEN,COMORIGEN, DIRORIGEN, REGDESTINO, COMDESTINO, DIRDESTINO,Convert.ToDecimal(PEDIDO),Convert.ToDecimal(TRANSPORTE),Convert.ToDecimal(ZONA));
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




    }


    public class InformeViajesModels
    {
        [Required(ErrorMessage = "Dato Obligatorio")]
        [Display(Name = "Fecha Inicio")]
        public string FechaIni { get; set; }

        [Required(ErrorMessage = "Dato Obligatorio")]
        [Display(Name = "Fecha Termino")]
        public string FechaTer { get; set; }

        [Required(ErrorMessage = "Dato Obligatorio")]
        [Display(Name = "Informes")]
        public string Informes { get; set; }

        //public string NomIndexReporte { get; set; }

        public IEnumerable<SelectListItem> ListaInformes { get; set; }

        public void GetListaInformes()
        {
            List<SelectListItem> oListaInformes = new List<SelectListItem>();

            oListaInformes.Add(new SelectListItem
            {
                Text = "Seleccionar Informe...",
                Value = "0"
            });

            oListaInformes.Add(new SelectListItem
            {
                Text = "Informe Planilla Combustibles",
                Value = "1"
            });

            oListaInformes.Add(new SelectListItem
            {
                Text = "Informe Planilla Gastos",
                Value = "2"
            });

            oListaInformes.Add(new SelectListItem
            {
                Text = "Informe Planilla Rutas",
                Value = "3"
            });

            ListaInformes = oListaInformes;

        }



    }




    public class DetalleGastosModels
    {
        public int IDGTOS { get; set; }

        public int FOLGTOS { get; set; }


        public int TPeaje { get; set; }
        public int TEstac { get; set; }
        public int TViat { get; set; }
        public int TAloj { get; set; }
        public int TMant { get; set; }
        public int TCarDes { get; set; }
        public int TOtros { get; set; }
        public int TIva { get; set; }
        public int TTotal { get; set; }
               
        [Required]
        [Display(Name = "Fecha")]
        public DateTime FECHADETGASTOS { get; set; }

        [Required]
        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        [Display(Name = "Peaje")]
        public int PEAJE { get; set; }

        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        [Display(Name = "Estacionamiento")]
        public int ESTACIONAMIENTO { get; set; }

        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        [Display(Name = "Viatico")]
        public int VIATICO { get; set; }

        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        [Display(Name = "Alojamiento")]
        public int ALOJAMIENTO { get; set; }

        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        [Display(Name = "Mantenimiento")]
        public int MANTENIMIENTO { get; set; }

        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        [Display(Name = "Carga / Descarga")]
        public int CARGADESCARGA { get; set; }

        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        [Display(Name = "Otros")]
        public int OTROS { get; set; }

        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        [Display(Name = "Iva")]
        public int IVA { get; set; }


        [Display(Name = "Observacion")]
        public string OBSERVACIONGASTOS { get; set; }


        

        public class Respuesta
        {
            public bool Ok { get; set; }
            public string Mensaje { get; set; }
        }



        public Respuesta AgregaDetalleGastos()
        {
            Respuesta oResultado = new Respuesta();
            try
            {
                Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext oFol = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();
                oFol.PR_REP_AGREGA_DETALLE_GASTOS(FOLGTOS,FECHADETGASTOS,PEAJE,ESTACIONAMIENTO,VIATICO,ALOJAMIENTO,MANTENIMIENTO,CARGADESCARGA,OTROS,IVA,OBSERVACIONGASTOS);
                oResultado.Ok = true;
                oResultado.Mensaje = "<p>Registro Ingresado Exitosamente!</p>";
                //oRetorno.Mensaje = "Registro ingresado Exitosamente!";
                //oRetorno.Ok = true;
            }
            catch (Exception e)
            {

                oResultado.Ok = false;
                oResultado.Mensaje = "<p>Error al Actualizar Datos</p>" + e.Message;
                //oRetorno.Mensaje = string.Format("Se ha producido un error: {0}", e.Message);
                //oRetorno.Ok = false;
            }

            //return oRetorno;
            return oResultado;

            
        }

        public RetornoModels ActualizaDetalleGastos()
        {
            RetornoModels oRetorno = new RetornoModels();
            try
            {
                Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext oFol = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();
                oFol.PR_REP_MODIFICA_DETALLE_GASTOS(IDGTOS, FECHADETGASTOS, PEAJE, ESTACIONAMIENTO, VIATICO, ALOJAMIENTO, MANTENIMIENTO, CARGADESCARGA, OTROS, IVA, OBSERVACIONGASTOS);
                oRetorno.Mensaje = "Registro actualizado Exitosamente!";
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



    public class DetalleCombustibleModels
    {
        public int IDCOMB { get; set; }

        public int FOLCOMB { get; set; }
        
        [Required]
        [Display(Name = "Fecha")]
        public DateTime FECHADETCOMB { get; set; }


        [Required]
        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        [Display(Name = "Numero")]
        public int NUMERO { get; set; }


        [Required]        
        [RegularExpression(@"^[0-9]*([\,][0-9]{1,5})?$", ErrorMessage = "Peso admite números con decimales separados por coma (Máx. 5 decimales)")]
        [Display(Name = "Litros")]
        public string LITROS { get; set; }

        [Required]
        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        [Display(Name = "Valor")]
        public int VALOR { get; set; }


        [Display(Name = "Observación")]
        public string OBSERVACIONCOMB { get; set; }



        public class Respuesta
        {
            public bool Ok { get; set; }
            public string Mensaje { get; set; }
        }




        public RetornoModels ActualizaDetalleCombustible()
        {
            RetornoModels oRetorno = new RetornoModels();
            try
            {
                Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext oFol = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();

                double oLitros = 0;
                Double.TryParse(LITROS, out oLitros);

                oFol.PR_REP_MODIFICA_DETALLE_COMBUSTIBLE(IDCOMB, FECHADETCOMB, NUMERO, oLitros, VALOR, OBSERVACIONCOMB);
                oRetorno.Mensaje = "Registro actualizado Exitosamente!";
                oRetorno.Ok = true;
            }
            catch (Exception e)
            {
                oRetorno.Mensaje = string.Format("Se ha producido un error: {0}", e.Message);
                oRetorno.Ok = false;
            }
            return oRetorno;

        }

        


        public Respuesta AgregaDetalleCombustible()
        {
            
            //CabeceraFoliosModels oModel = new CabeceraFoliosModels();
            //RetornoModels oRetorno = new RetornoModels();
            Respuesta oResultado = new Respuesta();
            try
            {
                Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext oFol = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();
                double oLitros = 0;
                Double.TryParse(LITROS, out oLitros);

                //oFol.PR_REP_AGREGA_DETALLE_COMBUSTIBLE(55,Convert.ToDateTime("2013-06-12"),667,100,850,"observacion1");
                //oFol.PR_REP_AGREGA_DETALLE_COMBUSTIBLE(55, FECHADETCOMB, NUMERO, LITROS, VALOR, OBSERVACIONCOMB);
                oFol.PR_REP_AGREGA_DETALLE_COMBUSTIBLE(FOLCOMB, FECHADETCOMB, NUMERO, oLitros, VALOR, OBSERVACIONCOMB);
                oResultado.Ok = true;
                oResultado.Mensaje = "<p>Registro Ingresado Exitosamente!</p>";
                //oRetorno.Mensaje = "Registro ingresado Exitosamente!";
                //oRetorno.Ok = true;
            }
            catch (Exception e)
            {

                oResultado.Ok = false;
                oResultado.Mensaje = "<p>Error al Actualizar Datos</p>" + e.Message;
                //oRetorno.Mensaje = string.Format("Se ha producido un error: {0}", e.Message);
                //oRetorno.Ok = false;
            }

            //return oRetorno;
            return oResultado;
          
        }

    }



    public class VehiculosModels
    {
        public int RutCondAux { get; set; }
        public int IdAux { get; set; }
        public string NombreConductor { get; set; }
        public int IdVehic { get; set; }
        public int IdSuc { get; set; }

        [Display(Name = "Transporte")]
        public string PatenteAux { get; set; }

        [Display(Name = "Buscar")]
        public string BuscarSucursal { get; set; }
                
        
        [Display(Name = "Buscar")]
        public string BuscarPatente { get; set; }
        
        [Display(Name = "Id")]
        public int ID { get; set; }

        [Display(Name = "Patente")]
        public string PATENTE { get; set; }

        [Display(Name = "Modelo")]
        public string MODELO { get; set; }

        [Display(Name = "Marca")]
        public string MARCA { get; set; }

        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        [Display(Name = "Capacidad KGS")]
        public int CAPACIDADKGS { get; set; }

        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        [Display(Name = "Capacidad M3")]
        public int CAPACIDADM3 { get; set; }

        [Required(ErrorMessage = "Obligatorio")]
        [Display(Name = "Sucursal")]
        public int SUCVEHICULO { get; set; }

        
        [Display(Name = "Sucursal")]
        public int SUCURSAL { get; set; }


        public IEnumerable<VerTodoslosVehiculos> VerVehiculosdet { get; set; }
        public IEnumerable<SelectListItem> ListaSucVehi { get; set; }
        public IEnumerable<PR_REP_LISTA_TRANS_CONDUCTORResult> ListaTransportesAsociados { get; set; }
        public IEnumerable<SelectListItem> LstSucursales { get; set; }
        public IEnumerable<PR_REP_LISTA_TRANS_POR_SUCResult> ListaTransPorSuc { get; set; }
        public IEnumerable<PR_REP_LISTA_SUC_ASIGNADASResult> LstSucAsignadas { get; set; }
        public IEnumerable<PR_REP_LISTA_SUC_NO_ASIGNADASResult> LstSucNoAsignadas { get; set; }



        #region GetEliminaSucursal
        public RetornoModels GetEliminaSucursal()
        {
            RetornoModels oRetorno = new RetornoModels();
            try
            {

                LinqBD_REP_REPORTE_VIAJESDataContext oVeh = new LinqBD_REP_REPORTE_VIAJESDataContext();
                oVeh.PR_REP_ELIMINA_SUCURSAL(IdVehic, IdSuc);
                oRetorno.Mensaje = "Sucursal eliminada Exitosamente !";
                oRetorno.Ok = true;
            }
            catch (Exception e)
            {
                oRetorno.Mensaje = string.Format("Se ha producido un error: {0}", e.Message);
                oRetorno.Ok = false;
            }
            return oRetorno;

        }
        #endregion

        #region GetAsignaSucursal
        public RetornoModels GetAsignaSucursal()
        {
            RetornoModels oRetorno = new RetornoModels();
            try
            {

                LinqBD_REP_REPORTE_VIAJESDataContext oVeh = new LinqBD_REP_REPORTE_VIAJESDataContext();
                oVeh.PR_REP_ASIGNA_SUCURSAL(IdVehic, IdSuc);
                oRetorno.Mensaje = "Sucursal ingresada Exitosamente !";
                oRetorno.Ok = true;
            }
            catch (Exception e)
            {
                oRetorno.Mensaje = string.Format("Se ha producido un error: {0}", e.Message);
                oRetorno.Ok = false;
            }
            return oRetorno;

        }
        #endregion



        #region GetListaSucNoAsignadas
        public void GetListaSucNoAsignadas()
        {

            if (BuscarSucursal == null)
            {
                BuscarSucursal="";
            }
            LinqBD_REP_REPORTE_VIAJESDataContext oRep = new LinqBD_REP_REPORTE_VIAJESDataContext();
            var oDatos = from pr in oRep.PR_REP_LISTA_SUC_NO_ASIGNADAS(IdVehic, BuscarSucursal)
                         select pr;
            LstSucNoAsignadas = oDatos.ToList();
        }
        #endregion


        #region GetListaSucAsignadas
        public void GetListaSucAsignadas()
        {
            LinqBD_REP_REPORTE_VIAJESDataContext oRep = new LinqBD_REP_REPORTE_VIAJESDataContext();
            var oDatos = from pr in oRep.PR_REP_LISTA_SUC_ASIGNADAS(IdVehic)
                         select pr;
            LstSucAsignadas = oDatos.ToList();
        }
        #endregion


        #region GetinsertaVehiculo
        public RetornoModels GetinsertaVehiculo()
        {
            RetornoModels oRetorno = new RetornoModels();
            try
            {

                LinqBD_REP_REPORTE_VIAJESDataContext oVeh = new LinqBD_REP_REPORTE_VIAJESDataContext();
                oVeh.PR_REP_AGREGA_TRANSPORTE(PATENTE, MARCA, MODELO, CAPACIDADKGS,CAPACIDADM3);
                oRetorno.Mensaje = "Transporte ingresado Exitosamente !";
                oRetorno.Ok = true;
            }
            catch (Exception e)
            {
                oRetorno.Mensaje = string.Format("Se ha producido un error: {0}", e.Message);
                oRetorno.Ok = false;
            }
            return oRetorno;

        }
        #endregion


        #region getListaTransPorSucursal
        public void getListaTransPorSucursal()
        {
            LinqBD_REP_REPORTE_VIAJESDataContext oRep = new LinqBD_REP_REPORTE_VIAJESDataContext();
            var oDatos = from pr in oRep.PR_REP_LISTA_TRANS_POR_SUC(SUCURSAL)
                         select pr;
            ListaTransPorSuc = oDatos.ToList();
        }
        #endregion


        #region GetLstSucrsales
        public void GetLstSucrsales()
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oResult = from su in oPar.TB_PAR_SUC_SUCURSAL
                          //where
                          //su.PK_PAR_TUS_ID == 1
                          select new SelectListItem
                          {
                              Text = su.FL_PAR_SUC_NOMBRE,
                              Value = su.PK_PAR_SUC_ID.ToString()
                          };
            LstSucursales = oResult.ToList();
        }
        #endregion

        #region DesasociarMovil
        public RetornoModels DesasociarMovil()
        {
            RetornoModels oRetorno = new RetornoModels();
            try
            {
                Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext oFol = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();

                LinqBD_REP_REPORTE_VIAJESDataContext oElim = new LinqBD_REP_REPORTE_VIAJESDataContext();
                oElim.PR_REP_ELIMINA_TRANS_CONDUCTOR(RutCondAux, IdAux); 
                oRetorno.Mensaje = "Movil Eliminado Exitosamente!";
                oRetorno.Ok = true;
            }
            catch (Exception e)
            {
                oRetorno.Mensaje = string.Format("Se ha producido un error: {0}", e.Message);
                oRetorno.Ok = false;
            }
            return oRetorno;
        }
        #endregion

        #region getListaTransAsoc
        public void getListaTransAsoc()
        {
            LinqBD_REP_REPORTE_VIAJESDataContext oRep = new LinqBD_REP_REPORTE_VIAJESDataContext();
            var oDatos = from pr in oRep.PR_REP_LISTA_TRANS_CONDUCTOR(RutCondAux)
                         select pr;
            ListaTransportesAsociados = oDatos.ToList();
        }
        #endregion


        public void RescataDatosVehiculos()
        {
            if (BuscarPatente == null)
            {
                BuscarPatente = "";
            }
            var oDataContext = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();
            var oDetVehi = from dv in oDataContext.PR_REP_MUESTRA_VEHICULOS(BuscarPatente)
                           select new VerTodoslosVehiculos
                           {
                               
                               Vid=dv.PK_PAR_TRA_ID,
                               Vpatente=dv.FL_PAR_TRA_PATENTE,
                               Vmodelo=dv.FL_PAR_TRA_MODELO,
                               Vmarca=dv.FL_PAR_TRA_MARCA,
                               Vcapkgs=Convert.ToInt32(dv.FL_PAR_TRA_CAPACIDAD_KGS),
                               Vcapm3 = Convert.ToInt32(dv.FL_PAR_TRA_CAPACIDAD_M3),
                           };
            VerVehiculosdet = oDetVehi.ToList();
        }

        public void GetListaSuc()
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oResult = from su in oPar.TB_PAR_SUC_SUCURSAL
                          where
                          su.PK_PAR_TUS_ID == 1
                          select new SelectListItem
                          {
                              Text = su.FL_PAR_SUC_NOMBRE,
                              Value = su.PK_PAR_SUC_ID.ToString(),
                          };
            ListaSucVehi = oResult;
        }


        public RetornoModels ActualizaVehiculo()
        {
            RetornoModels oRetorno = new RetornoModels();
            try
            {
           
                LinqBD_REP_REPORTE_VIAJESDataContext aFol = new LinqBD_REP_REPORTE_VIAJESDataContext();
                aFol.PR_REP_ACTUALIZA_VEHICULO(ID,PATENTE,MARCA,MODELO,CAPACIDADKGS,CAPACIDADM3);
                oRetorno.Mensaje = "Registro Actualizado !";
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



    public class ConductoresModels
    {
        public int RUTAUX { get; set; }
        public int IDPATENTE { get; set; }

        [Display(Name = "Buscar")]
        public string BuscarConductor { get; set; }



        [Display(Name = "CONDUCTOR : ")]
        public string NOMAUX { get; set; }

        [Required]
        [RegularExpression("\\d*[0-9]-([0-9]|k|K)$", ErrorMessage = "Campo Rut Formato incorrecto, ejemplo: 12345678-9")]
        [Remote("ValidarRutSolo", "Validation")]
        [Display(Name = "Rut")]
        public string Rut { get; set; }

        [Display(Name = "Digito")]
        public string DIGITOCONDUCTOR { get; set; }

        [Required(ErrorMessage = "Obligatorio")]
        [Display(Name = "Nombres")]
        public string NOMBRECONDUCTOR { get; set; }

        //[Required(ErrorMessage = "Obligatorio")]
        [Display(Name = "Codigo")]
        public int CODIGO { get; set; }

        //[Required(ErrorMessage = "Obligatorio")]
        [Display(Name = "Sucursal")]
        public int SUCCONDUCTOR { get; set; }

        [Display(Name = "Rut")]
        public string BUSCARRUT { get; set; }

        [Display(Name = "Patente")]
        public string BUSCARPAT { get; set; }

        public IEnumerable<VerTransportes> ListaTransportes { get; set; }
        public IEnumerable<VerTransAsoc> ListaTransAsoc { get; set; }
        public IEnumerable<VerTodoslosConductores> VerConductoresdet { get; set; }
        public IEnumerable<SelectListItem> ListaSucCond { get; set; }

        #region GetAsociaTransp
        public RetornoModels GetAsociaTransp()
        {
            RetornoModels oRetorno = new RetornoModels();
            try
            {
                LinqBD_REP_REPORTE_VIAJESDataContext oAgr = new LinqBD_REP_REPORTE_VIAJESDataContext();
                oAgr.PR_REP_ASIGNA_SUC_A_CONDUCTOR(RUTAUX, IDPATENTE);
                oRetorno.Mensaje = "Transporte Asociado Existosamente !";
                oRetorno.Ok = true;
            }
            catch (Exception e)
            {
                oRetorno.Mensaje = string.Format("Se ha producido un error: {0}", e.Message);
                oRetorno.Ok = false;
            }
            return oRetorno;
        }
        #endregion

        public void RescataTransAsoc(int rtc)
        {
            var oDataContext = new Courier.Models.LinqBD_FOLDataContext();
            var oDetAsoc = from t in oDataContext.PR_FOL_VER_TRANS_ASOCIADOS(rtc)
                           select new VerTransAsoc
                           {
                               Tid = t.PK_PAR_TRA_ID,
                               Tpatente = t.FL_PAR_TRA_PATENTE,
                               Tmarca = t.FL_PAR_TRA_MARCA,
                               Tmodelo = t.FL_PAR_TRA_MODELO
                           };

            ListaTransAsoc = oDetAsoc.ToList();

        }



         public RetornoModels DesTransp()
         {
             RetornoModels oRetorno = new RetornoModels();
             try
             {
                 Courier.Models.LinqBD_FOLDataContext oTrc = new Courier.Models.LinqBD_FOLDataContext();
                 oTrc.PR_FOL_DESASOCIAR_VEHIC(RUTAUX, IDPATENTE);
                 oRetorno.Mensaje = "Registro Eliminado !";
                 oRetorno.Ok = true;
             }
             catch (Exception e)
             {
                 oRetorno.Mensaje = string.Format("Se ha producido un error: {0}", e.Message);
                 oRetorno.Ok = false;
             }
             return oRetorno;
         }


         public RetornoModels AgrTransp(int idast, int rta)
         {
             RetornoModels oRetorno = new RetornoModels();
             try
             {
                 Courier.Models.LinqBD_FOLDataContext oTrc = new Courier.Models.LinqBD_FOLDataContext();
                 oTrc.PR_FOL_ASOCIAR_VEHIC(idast, rta);
                 oRetorno.Mensaje = "Registro Ingresado !";
                 oRetorno.Ok = true;
             }
             catch (Exception e)
             {
                 oRetorno.Mensaje = string.Format("Se ha producido un error: {0}", e.Message);
                 oRetorno.Ok = false;
             }
             return oRetorno;
         }


         public RetornoModels ActDesCond(int rtc, int tpo)
         {
             RetornoModels oRetorno = new RetornoModels();
             try
             {
                 LinqBD_REP_REPORTE_VIAJESDataContext oTrc = new LinqBD_REP_REPORTE_VIAJESDataContext();
                 oTrc.PR_REP_ACT_DESAC_COND(rtc, tpo);
                 oRetorno.Mensaje = "Registro Actualizado !";
                 oRetorno.Ok = true;
             }
             catch (Exception e)
             {
                 oRetorno.Mensaje = string.Format("Se ha producido un error: {0}", e.Message);
                 oRetorno.Ok = false;
             }
             return oRetorno;
         }


         public void VerTransportesLib(string trns)
         {
             var oDataContext = new Courier.Models.LinqBD_FOLDataContext();
             var oDetTrans = from t in oDataContext.PR_FOL_VER_TRANSPORTES(RUTAUX, trns)
                            select new VerTransportes
                            {
                                id = t.PK_PAR_TRA_ID,
                                patente = t.FL_PAR_TRA_PATENTE,
                                marca = t.FL_PAR_TRA_MARCA,
                                modelo = t.FL_PAR_TRA_MODELO,
                                capm3=Convert.ToDecimal(t.FL_PAR_TRA_CAPACIDAD_KGS)
                            };

             ListaTransportes = oDetTrans.ToList();

         }



        public void RescataDatosConductores()
        {
            //int rtcond;
            //if (BUSCARRUT != null)
            //{
            //    rtcond = Convert.ToInt32(BUSCARRUT.Substring(0, BUSCARRUT.Length - 2));
            //    BUSCARRUT = Convert.ToString(rtcond);
            //}
            if (BuscarConductor==null){
                BuscarConductor = "";
            }
            var oDataContext = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();
            var oDetCond = from dc in oDataContext.PR_REP_MUESTRA_CONDUCTORES(BuscarConductor)
                           select new VerTodoslosConductores
                           {
                               Crut=dc.PK_PAR_CON_RUT,
                               Cdig=dc.FL_PAR_CON_DV,
                               Cnombres=dc.FL_PAR_CON_NOMBRE,
                               Ccodigo=Convert.ToInt32(dc.FL_PAR_CON_CODIGO), 
                               Csucursal=dc.FL_PAR_SUC_NOMBRE,
                               Cestado=dc.PK_PAR_EST_ID
                           };
            VerConductoresdet = oDetCond.ToList();
        }


        public void GetListaSuc()
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oResult = from su in oPar.TB_PAR_SUC_SUCURSAL
                          //where
                          //su.PK_PAR_TUS_ID == 1
                          select new SelectListItem
                          {
                              Text = su.FL_PAR_SUC_NOMBRE,
                              Value = su.PK_PAR_SUC_ID.ToString(),
                          };
            ListaSucCond = oResult;
        }

        
        
        public RetornoModels ActualizaConductor()
        {
            RetornoModels oRetorno = new RetornoModels();
            try
            {
                string DGVER;
                int RUTA = 0;
                DGVER = Rut.Substring(Rut.Length - 1, 1);
                RUTA = Convert.ToInt32(Rut.Substring(0, Rut.Length - 2));

                LinqBD_REP_REPORTE_VIAJESDataContext aFol = new LinqBD_REP_REPORTE_VIAJESDataContext();
                aFol.PR_REP_ACTUALIZA_CONDUCTOR(RUTA,Convert.ToInt32(CODIGO),NOMBRECONDUCTOR);
                oRetorno.Mensaje = "Registro Actualizado !";
                oRetorno.Ok = true;
            }
            catch (Exception e)
            {
                oRetorno.Mensaje = string.Format("Se ha producido un error: {0}", e.Message);
                oRetorno.Ok = false;
            }
            return oRetorno;
        }

        public RetornoModels insertaConductor()
        {
            RetornoModels oRetorno = new RetornoModels();
            try
            {
                string DGVER;
                int RUTC = 0;
                DGVER = Rut.Substring(Rut.Length - 1, 1);
                RUTC = Convert.ToInt32(Rut.Substring(0, Rut.Length - 2));

                LinqBD_PARDataContext oCond = new LinqBD_PARDataContext();
                oCond.PR_PAR_AGREGA_CONDUCTOR(RUTC, DGVER, NOMBRECONDUCTOR, CODIGO);
                oRetorno.Mensaje = "Conductor ingresado Exitosamente !";
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



    public class DespachadorModels
    {
        [Required]
        [RegularExpression("\\d*[0-9]-([0-9]|k|K)$", ErrorMessage = "Campo Rut Formato incorrecto, ejemplo: 12345678-9")]
        [Remote("ValidarRutSolo", "Validation")]
        [Display(Name = "Rut")]
        public string Rut { get; set; }

        [Display(Name = "Digito")]
        public string DIGITODESPACHADOR { get; set; }

        [Required(ErrorMessage = "Obligatorio")]
        [Display(Name = "Nombres")]
        public string NOMBRESDESPACHADOR { get; set; }

        [Required(ErrorMessage = "Obligatorio")]
        [Display(Name = "Apellidos")]
        public string APELLIDOSDESPACHADOR { get; set; }

        [Required(ErrorMessage = "Obligatorio")]
        [Display(Name = "Sucursal")]
        public int SUCDESPACHADOR { get; set; }

        public IEnumerable<VerTodoslosDespachadores> VerDespachadoresd { get; set; }
        public IEnumerable<SelectListItem> ListaSucDesp { get; set; }
        

        public void RescataDatosDespachadores()
        {
            var oDataContext = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();
            var oDetDesp = from dd in oDataContext.PR_REP_MUESTRA_DESPACHADORES()
                           select new VerTodoslosDespachadores
                           {
                               Drut = dd.PK_REP_DES_RUT,
                               Ddig = dd.FL_REP_DES_DV,
                               Dnombres = dd.FL_REP_DES_NOMBRES,
                               Dapellidos = dd.FL_REP_DES_APELLIDOS,
                               Destado = dd.FL_REP_ESE_DESCRIPCION,
                               Dsucursal = dd.FL_PAR_SUC_NOMBRE
                           };
            VerDespachadoresd = oDetDesp.ToList();
        }


        public void GetListaSuc()
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oResult = from su in oPar.TB_PAR_SUC_SUCURSAL
                          where
                          su.PK_PAR_TUS_ID == 1
                          select new SelectListItem
                          {
                              Text = su.FL_PAR_SUC_NOMBRE,
                              Value = su.PK_PAR_SUC_ID.ToString(),
                          };
            ListaSucDesp = oResult;
        }


        public RetornoModels InsertDespachador()
        {
            RetornoModels oRetorno = new RetornoModels();
            try
            {
                string DGVER;
                int RUTA = 0;
                DGVER = Rut.Substring(Rut.Length - 1, 1);
                RUTA = Convert.ToInt32(Rut.Substring(0, Rut.Length - 2));

                LinqBD_REP_REPORTE_VIAJESDataContext oFol = new LinqBD_REP_REPORTE_VIAJESDataContext();
                oFol.PR_REP_INA_INSERT_DESPACHADOR(RUTA, DGVER, NOMBRESDESPACHADOR, APELLIDOSDESPACHADOR, Convert.ToInt32(SUCDESPACHADOR));
                oRetorno.Mensaje = "Despachador ingresado Exitosamente !";
                oRetorno.Ok = true;
            }
            catch (Exception e)
            {
                oRetorno.Mensaje = string.Format("Se ha producido un error: {0}", e.Message);
                oRetorno.Ok = false;
            }
            return oRetorno;

        }


        public RetornoModels ActualizaDespachador()
        {
            RetornoModels oRetorno = new RetornoModels();
            try
            {
                string DGVER;
                int RUTA = 0;
                DGVER = Rut.Substring(Rut.Length - 1, 1);
                RUTA = Convert.ToInt32(Rut.Substring(0, Rut.Length - 2));

                LinqBD_REP_REPORTE_VIAJESDataContext aFol = new LinqBD_REP_REPORTE_VIAJESDataContext();
                aFol.PR_REP_ACTUALIZA_DESPACHADOR(RUTA,NOMBRESDESPACHADOR, APELLIDOSDESPACHADOR,Convert.ToInt32(SUCDESPACHADOR));
                oRetorno.Mensaje = "Registro Actualizado !";
                oRetorno.Ok = true;
            }
            catch (Exception e)
            {
                oRetorno.Mensaje = string.Format("Se ha producido un error: {0}", e.Message);
                oRetorno.Ok = false;
            }
            return oRetorno;
        }


        public RetornoModels CambiaestadoDespachador(int RutDs, int Est)
        {
            RetornoModels oRetorno = new RetornoModels();
            try
            {
                LinqBD_REP_REPORTE_VIAJESDataContext oFolC = new LinqBD_REP_REPORTE_VIAJESDataContext();
                oFolC.PR_REP_ACTUALIZA_ESTADO_DESPACHADOR(RutDs, Est);
                if (Est == 1)
                {
                    oRetorno.Mensaje = "Asistente Activado Exitosamente!";
                }
                else
                {
                    oRetorno.Mensaje = "Asistente Inactivado Exitosamente!";
                }
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


    

    public class AsistentesModels
    {

        [Required]
        [RegularExpression("\\d*[0-9]-([0-9]|k|K)$", ErrorMessage = "Campo Rut Formato incorrecto, ejemplo: 12345678-9")]
        [Remote("ValidarRutSolo", "Validation")]
        [Display(Name = "Rut")]
        public string Rut { get; set; }

        [Display(Name = "Digito")]
        public string DIGITOASISTENTE { get; set; }

        [Required(ErrorMessage = "Obligatorio")]
        [Display(Name = "Nombres")]
        public string NOMBRESASISTENTE { get; set; }

        [Required(ErrorMessage = "Obligatorio")]
        [Display(Name = "Apellidos")]
        public string APELLIDOSASISTENTE { get; set; }

        [Required(ErrorMessage = "Obligatorio")]
        [Display(Name = "Codigo")]
        public int CODIGO { get; set; }

        [Required(ErrorMessage = "Obligatorio")]
        [Display(Name = "Sucursal")]
        public int SUCASISTENTE { get; set; }

        public IEnumerable<VerTodoslosAsistentes> VerAsistentesd { get; set; }
        public IEnumerable<SelectListItem> ListaSucAsis{ get; set; }
      

        public void RescataDatosAsistentes()
        {
            var oDataContext = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();
            var oDetAsis= from da in oDataContext.PR_REP_MUESTRA_ASISTENTES()
                          select new VerTodoslosAsistentes
                          {
                             Arut = da.PK_REP_ASI_RUT,
                             Adig =da.FL_REP_ASI_DV,
                             Anombres =da.FL_REP_ASI_NOMBRES,
                             Aapellidos =da.FL_REP_ASI_APELLIDOS,
                             Acodigo =da.FL_REP_ASI_CODIGO,
                             Aestado =da.FL_REP_ESE_DESCRIPCION,
                             Asucursal =da.FL_PAR_SUC_NOMBRE
                          };
            VerAsistentesd=oDetAsis.ToList();
        }

        public void GetListaSuc()
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oResult = from su in oPar.TB_PAR_SUC_SUCURSAL
                          where
                          su.PK_PAR_TUS_ID == 1
                          select new SelectListItem
                          {
                              Text = su.FL_PAR_SUC_NOMBRE,
                              Value = su.PK_PAR_SUC_ID.ToString(),
                          };
            ListaSucAsis = oResult;
        }


        public RetornoModels ActualizaAsistente()
        {
            RetornoModels oRetorno = new RetornoModels();
            try
            {
                string DGVER;
                int RUTA = 0;
                DGVER = Rut.Substring(Rut.Length - 1, 1);
                RUTA = Convert.ToInt32(Rut.Substring(0, Rut.Length - 2));

                LinqBD_REP_REPORTE_VIAJESDataContext aFol = new LinqBD_REP_REPORTE_VIAJESDataContext();
                aFol.PR_REP_ACA_ACTULIZA_ASISTENTE(RUTA, DGVER, NOMBRESASISTENTE, APELLIDOSASISTENTE, Convert.ToInt32(CODIGO), Convert.ToInt32(SUCASISTENTE));
                oRetorno.Mensaje = "Registro Actualizado !";
                oRetorno.Ok = true;
            }
            catch (Exception e)
            {
                oRetorno.Mensaje = string.Format("Se ha producido un error: {0}", e.Message);
                oRetorno.Ok = false;
            }
            return oRetorno;
        }

               

        public RetornoModels InsertAsistente()
        {
            RetornoModels oRetorno = new RetornoModels();
            try
            {
                string DGVER ;
                int RUTA = 0;
                DGVER = Rut.Substring(Rut.Length - 1, 1);
                RUTA = Convert.ToInt32(Rut.Substring(0, Rut.Length - 2));
                               
                LinqBD_REP_REPORTE_VIAJESDataContext oFol = new LinqBD_REP_REPORTE_VIAJESDataContext();
                oFol.PR_REP_INA_INSERT_ASISTENTE(RUTA,DGVER, NOMBRESASISTENTE, APELLIDOSASISTENTE, Convert.ToInt32(CODIGO),Convert.ToInt32(SUCASISTENTE));
                oRetorno.Mensaje = "Asistente ingresado Exitosamente !";
                oRetorno.Ok = true;
            }
            catch (Exception e)
            {
                oRetorno.Mensaje = string.Format("Se ha producido un error: {0}", e.Message);
                oRetorno.Ok = false;
            }
            return oRetorno;

        }


        public RetornoModels ActivaAsistente(int RutAs, int Est)
        {
            RetornoModels oRetorno = new RetornoModels();
            try
            {
                LinqBD_REP_REPORTE_VIAJESDataContext oFolC = new LinqBD_REP_REPORTE_VIAJESDataContext();
                oFolC.PR_REP_ACTUALIZA_ESTADO_ASISTENTE(RutAs, Est);
                if (Est == 1)
                {
                    oRetorno.Mensaje = "Asistente Activado Exitosamente!";
                }
                else
                {
                    oRetorno.Mensaje = "Asistente Inactivado Exitosamente!";
                }
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




    
    public class CabeceraFoliosModels
    {

        public bool SoloIngresoRutas { get; set; }
        public bool AdminReport { get; set; }

        public int MMOUSUARIO { get; set; }
        public string USUARIOOLD { get; set; }
        public int CONUSUARIO { get; set; }
        public bool AdminisReporteViajes { get; set; }
        public string CONDUCTORTEXT { get; set; }
        public string ASISTENTETEXT { get; set; }
        public string DESPACHADORTEXT { get; set; }

        [Display(Name = "N° Folio")]
        [Required(ErrorMessage = "Obligatorio")]
        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        //[Remote("JsonValidarFolio", "Ingreso")]
        public string FOLIO { get; set; }
        
        
        
        [Required]
        [Display(Name = "Codigo 1")]
        public int CONDUCTOR { get; set; }

        
        [Required(ErrorMessage = "Debe Seleccionar Conductor")]
        [Display(Name = "Rut")]
        public string RutC { get; set; }

        //[Required]
        [Display(Name = "Codigo 2")]
        public int? ASISTENTE { get; set; }

        [Required]
        [Display(Name = "Rut")]
        public string RutA { get; set; }

        [Required(ErrorMessage = "Debe Seleccionar Area de Operación")]
        [Display(Name = "Área Operación")]
        public int AREA { get; set; }

        
        [Required(ErrorMessage = "Debe Seleccionar Zona")]
        [Display(Name = "Zona")]
        public int ZONA { get; set; }

        [Required]
        [Display(Name = "Vehiculo")]
        public string VEHICULO { get; set; }

        //[Required]
        [Display(Name = "Placa Rampla")]
        public string RAMPLA { get; set; }

        [Required]
        [Display(Name = "Servicio")]
        public int SERVICIO { get; set; }

        [Required]
        [Display(Name = "Tipo")]
        public string TIPO { get; set; }

        [Required]
        [Display(Name = "Cap. Carga KG")]
        public int CAPKG { get; set; }

        [Required]
        [Display(Name = "Cap. Carga M3")]
        public int CAPM3 { get; set; }

        [Required]
        [Display(Name = "Cap. Carga KG")]
        public int CAPKGRAMPLA { get; set; }

        [Required]
        [Display(Name = "Cap. Carga M3")]
        public int CAPM3RAMPLA { get; set; }
        
        //[Required]
        [Display(Name = "Despachador")]
        public int? DESPACHADOR { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]*([\,][0-9]{1,5})?$", ErrorMessage = "KG Transportados admite números con decimales separados por coma (Máx. 5 decimales)")]
        [Display(Name = "KG Transportados")]
        public string KGTRANS { get; set; }

        [Required]
        [Display(Name = "Bultos/M3")]
        public int NUMBULTOS { get; set; }

     
        [Display(Name = "Observación")]
        public string OBSERVACIONCAB { get; set; }

        [Required]
        [Display(Name = "Fondo a Rendir")]
        public int FARENDIR { get; set; }

        [Required]
        [Display(Name = "Total Rendido")]
        public int TRENDIDO { get; set; }

        [Required]
        [Display(Name = "Monto Devolver o Recibir")]
        public int MDEVOLRECIB { get; set; }


        [Required]
        [Display(Name = "Codigo Asistente")]
        public int OTROASISTENTE { get; set; }

 
        [Display(Name = "Referencia")]
        public string REFERENCIA { get; set; }

        //[Required]
        [Display(Name = "Sucursal")]
        public string SUCURSAL { get; set; }

        [Required(ErrorMessage = "Obligatorio")]
        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        [Display(Name = "FOLIO")]
        public int FOLIOABUSCAR { get; set; }


        [Required(ErrorMessage = "Dato Obligatorio")]
        [Display(Name = "Busqueda")]
        public string BUSQUEDA { get; set; }

        [Display(Name = "Estado")]
        public string ESTADO { get; set; }

        [Display(Name = "Usuario")]
        public string USUARIOACTUAL { get; set; }


        public IEnumerable<SelectListItem> ListaConductores { get; set; }
        public IEnumerable<SelectListItem> ListaAsistentes { get; set; }
        public IEnumerable<SelectListItem> ListaAreas { get; set; }
        public IEnumerable<SelectListItem> ListaZonas { get; set; }
        public IEnumerable<SelectListItem> ListaServicios { get; set; }
        public IEnumerable<SelectListItem> ListaRamplas { get; set; }
        public IEnumerable<SelectListItem> ListaDespachadores { get; set; }
        public IEnumerable<VerCombustible> DetalleCombustible { get; set; }
        public IEnumerable<VerGastos> DetalleGastos { get; set; }
        public IEnumerable<VerRutas> DetalleRutas { get; set; }
        public IEnumerable<VerAsistentes> DetalleAsistentes { get; set; }
        public IEnumerable<SelectListItem> ListaSUCURSALES { get; set; }
        public IEnumerable<SelectListItem> ListaOpBusquedaFolios { get; set; }
        public IEnumerable<SelectListItem> ListaOpReasignaFolios { get; set; }
        public IEnumerable<VerFoliosActivos> DetalleFoliosActivos { get; set; }
        public IEnumerable<VerRangoPatentes> RangoPatentes { get; set; }
        public IEnumerable<VerFolioCambioestado> DetalleFoliocambEst { get; set; }
        public IEnumerable<VerDetalleRgoPatentes> DetalleRgoPatentes { get; set; } 
        public IEnumerable<SelectListItem> ListaBlancoZonas { get; set; }
        public IEnumerable<SelectListItem> ListaVehiculos2 { get; set; }

        public void GetListaVehiculos2()
        {
            LinqBD_PARDataContext oList = new LinqBD_PARDataContext();
            var oListaV = from v in oList.TB_PAR_TRA_TRANSPORTE
                          orderby v.FL_PAR_TRA_PATENTE
                          where v.FL_PAR_TRA_RAMPLA == false
                          select new SelectListItem
                          {
                              Text = v.FL_PAR_TRA_PATENTE,
                              Value = v.PK_PAR_TRA_ID.ToString(),
                          };

            ListaVehiculos2 = oListaV;
        }




        #region GetListaBlancoZonas
        public void GetListaBlancoZonas()
        {
            List<SelectListItem> oLista = new List<SelectListItem>();
            ListaBlancoZonas = oLista;
        }
        #endregion



        //public IEnumerable<VertotGrillagastos> DetalleTotalGastos { get; set; }

        public void GetListaZonas(int idA)
        {
            if (idA != 0)
            {
                LinqBD_REP_REPORTE_VIAJESDataContext oList = new LinqBD_REP_REPORTE_VIAJESDataContext();
                var oListaA = from a in oList.VI_REP_AREA_ZONA
                              orderby a.FL_REP_ZON_DESCRIPCION
                              where a.PK_REP_ARE_ID == idA
                              select new SelectListItem
                              {
                                  Text = a.FL_REP_ZON_DESCRIPCION,
                                  Value = a.PK_REP_ZON_ID.ToString(),
                              };
                ListaZonas = oListaA;
            }
            else
            {
                List<SelectListItem> ol1 = new List<SelectListItem>();
                ListaZonas = ol1;
            }
        }



        public void GetListaOpReasigFolios()
        {
            List<SelectListItem> oListaOpf = new List<SelectListItem>();

            oListaOpf.Add(new SelectListItem
            {
                Text = "Seleccione Busqueda...",
                Value = "0"
            });

            oListaOpf.Add(new SelectListItem
            {
                Text = "Folio",
                Value = "1"
            });

            oListaOpf.Add(new SelectListItem
            {
                Text = "Patente",
                Value = "2"
            });

            ListaOpReasignaFolios = oListaOpf;
        }




        public void GetListaOpBusFol()
        {
            List<SelectListItem> oListaOp = new List<SelectListItem>();

            oListaOp.Add(new SelectListItem
            {
                Text = "Seleccione Busqueda...",
                Value = "0"
            });

            oListaOp.Add(new SelectListItem
            {
                Text = "Folio",
                Value = "1"
            });

            oListaOp.Add(new SelectListItem
            {
                Text = "Sucursal",
                Value = "2"
            });

            ListaOpBusquedaFolios = oListaOp;
        }
        
        
        
        
        public void GetListaSuc()
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oResult = from su in oPar.TB_PAR_SUC_SUCURSAL
                          where 
                          su.PK_PAR_TUS_ID==1
                          select new SelectListItem
                          {
                              Text = su.FL_PAR_SUC_NOMBRE,
                              Value = su.PK_PAR_SUC_ID.ToString()
                          };
            ListaSUCURSALES = oResult.ToList();
        }



        public void RescataDatosAsistentes()
        {
            var oDataContext = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();
            var oDetFolio = from dc in oDataContext.TB_REP_RFA_RELACION_FOLIO_ASISTENTES
                            where
                            dc.PK_REP_CAB_FOLIO == Convert.ToInt32(FOLIO)
                            select new VerAsistentes
                            {
                                DAid=dc.PK_REP_RFA_ID,
                                DAcodigo = dc.TB_REP_ASI_ASISTENTES.FL_REP_ASI_CODIGO,
                                DAnombres=dc.TB_REP_ASI_ASISTENTES.FL_REP_ASI_NOMBRES+dc.TB_REP_ASI_ASISTENTES.FL_REP_ASI_APELLIDOS
                            };
            DetalleAsistentes = oDetFolio;
        }



        public void RescataDatosControlRuta()
        {
            var oDataContex = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();
            var oDetRuta = from r in oDataContex.VI_REP_ORIGEN_DESTINO2
                           where
                           r.PK_REP_CAB_FOLIO==Convert.ToInt32(FOLIO)
                           select new VerRutas
                           {
                               DRID=r.PK_REP_DCR_ID,
                               DRFECHA=r.FL_REP_DCR_FECHA,
                               DRRORIGEN=r.REGION_ORIGEN,
                               DRCOMORIGEN=r.COMUNA_ORIGEN,
                               DRKMINICIAL=r.FL_REP_DCR_KM_INICIAL,
                               DRREGDESTINO=r.REGION_DESTINO,
                               DRCOMDESTINO=r.COM_DESTINO,
                               DRKMFINAL=r.FL_REP_DCR_KM_FINAL,
                               DRKMRECORIDOS=r.FL_REP_DCR_KM_RECORRIDOS,
                               NPEDIDO=Convert.ToString(r.FL_REP_DCR_NUM_PEDIDO),
                               NTRANSPORTE=Convert.ToString(r.FL_REP_DCR_NUM_TRANSPORTE),
                               NZONA=Convert.ToString(r.FL_REP_DCR_ZONA),
                            };
            DetalleRutas = oDetRuta;
        }


        public void RescataFoliosActivos()
        {
            var oDataContex = new LinqBD_REP_REPORTE_VIAJESDataContext();
            var oFoliosAct = from f in oDataContex.PR_REP_MUESTRA_FOLIOS_ACTIVOS(Convert.ToInt32(SUCURSAL))
                          select new VerFoliosActivos
                          {
                              Fact =f.PK_REP_CAB_FOLIO,
                              Fpatente=f.FL_PAR_TRA_PATENTE,
                              Fusuario =f.USUARIO_INGRESO,
                              Fconductor =f.FL_PAR_CON_NOMBRE,
                              Frampla =f.RAMPLA,
                              Fktransp =Convert.ToInt32(f.FL_REP_CAB_KILOS_TRANSPORTADOS),
                              Fbtransp =Convert.ToInt32(f.FL_REP_CAB_BULTOS_TRANSPORTADOS)
                          };
            DetalleFoliosActivos = oFoliosAct.ToList();
                                
        }

        public void RescataDetalleRgoPatentes(int idT)
        {
            var oDataContex = new LinqBD_REP_REPORTE_VIAJESDataContext();
            var oFoliosd = from d in oDataContex.PR_REP_MUESTRA_DETALLE_RANGOS(idT)
                           select new VerDetalleRgoPatentes
                           {
                              Ffolio = d.PK_REP_CAB_FOLIO,
                              Fpatente=d.FL_PAR_TRA_PATENTE,
                              Frampla =d.RAMPLA,
                              Fusuario =d.USUARIO_INGRESO,
                              Fconductor =d.FL_PAR_CON_NOMBRE,
                              Fktransp =Convert.ToInt32(d.FL_REP_CAB_KILOS_TRANSPORTADOS),
                              Fbtransp =Convert.ToInt32(d.FL_REP_CAB_BULTOS_TRANSPORTADOS),
                              Festado =d.ESTADO,
                              Fsucursal=d.SUCURSAL
                           };
            DetalleRgoPatentes = oFoliosd.ToList();
        }




        public void RescataRangoPatentes()
        {
            var oDataContex = new LinqBD_REP_REPORTE_VIAJESDataContext();
            var oRangoFolios = from r in oDataContex.PR_REP_MUESTRA_RANGO_PATENTES(Convert.ToInt32(VEHICULO))
                               select new VerRangoPatentes
                               {
                                   Fpatente = r.FL_PAR_TRA_PATENTE,
                                   Fid = r.PK_REP_FOL_ID_CARGA,
                                   Fdesde = r.PK_REP_FOL_DESDE,
                                   Fhasta = r.PK_REP_FOL_HASTA,
                                   Fusuario = r.USUARIO_INGRESO
                               };
            RangoPatentes = oRangoFolios.ToList();
        }




        public void RescataDatosFolio()
        {
            var oDataContex = new LinqBD_REP_REPORTE_VIAJESDataContext();
            var oFolcamEst = from f in oDataContex.PR_REP_MUESTRA_FOLIO(FOLIOABUSCAR)
                             select new VerFolioCambioestado
                             {
                                 Fact = f.PK_REP_CAB_FOLIO,
                                 Fpatente = f.FL_PAR_TRA_PATENTE,
                                 Fusuario = f.USUARIO_INGRESO,
                                 Fconductor = f.FL_PAR_CON_NOMBRE,
                                 Festado =f.FL_REP_ECF_DESCRIPCION,
                                 Fsucursal=f.FL_PAR_SUC_NOMBRE,
                                 Frampla = f.RAMPLA,
                                 Fktransp = Convert.ToInt32(f.FL_REP_CAB_KILOS_TRANSPORTADOS),
                                 Fbtransp = Convert.ToInt32(f.FL_REP_CAB_BULTOS_TRANSPORTADOS),
                                 Fnumest=f.PK_REP_ECF_ID
                             };
            DetalleFoliocambEst = oFolcamEst.ToList();
        }



        public void RescataDatosControlGastos()
        {
            var oDataContext = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();
            var oDetFolioG = from dg in oDataContext.TB_REP_DCG_DETALLE_CONTROL_GASTOS
                             where
                             dg.PK_REP_CAB_FOLIO == Convert.ToInt32(FOLIO)
                             select new VerGastos
                             {
                                 DGid = dg.PK_REP_DCG_ID,
                                 DGfecha = Convert.ToDateTime(dg.FL_REP_DCG_FECHA),
                                 DGpeaje = Convert.ToInt32(dg.FL_REP_DCG_PEAJE),
                                 DGestacionamiento = Convert.ToInt32(dg.FL_REP_DCG_ESTACIONAMIENTO),
                                 DGviatico = Convert.ToInt32(dg.FL_REP_DCG_VIATICO),
                                 DGalojamiento = Convert.ToInt32(dg.FL_REP_DCG_ALOJAMIENTO),
                                 DGmantencion = Convert.ToInt32(dg.FL_REP_DCG_MANTENCION),
                                 DGcargadescarga = Convert.ToInt32(dg.FL_REP_DCG_CARGA_DESCARGA),
                                 DGotros = Convert.ToInt32(dg.FL_REP_DCG_OTROS),
                                 DGiva = Convert.ToInt32(dg.FL_REP_DCG_IVA),
                                 DGtotal = Convert.ToInt32(dg.FL_REP_DCG_TOTAL),
                                 DGobservacion = dg.FL_REP_DCG_OBSERVACION,
                             };
                DetalleGastos = oDetFolioG;
        }


        






        public void RescataDatosControlCombustible()
        {
            var oDataContext = new Courier.Models.LinqBD_REP_REPORTE_VIAJESDataContext();
            var oDetFolio = from dc in oDataContext.TB_REP_DCC_DETALLE_CONTROL_COMBUSTIBLE
                            where
                            dc.PK_REP_CAB_FOLIO == Convert.ToInt32(FOLIO)
                            select new VerCombustible
                            {
                                DCid = dc.PK_REP_DCC_ID,
                                DCfecha = Convert.ToDateTime(dc.FL_REP_DCC_FECHA),
                                //DCfecha = Convert.ToDateTime(dc.FL_CTV_DCC_FECHA).ToShortDateString(),
                                DCNum = Convert.ToInt32(dc.FL_REP_DCC_NUMERO_DOCUMENTO),
                                DCLitros = dc.FL_REP_DCC_CANTIDAD_LITROS,
                                DCValor = Convert.ToInt32(dc.FL_REP_DCC_VALOR),
                                DCObservacion = dc.FL_REP_DCC_OBSERVACION

                            };

            //select d;
            DetalleCombustible = oDetFolio;
        }
        
        
        
        
        public void GetListaDespachadores()
        {
            LinqBD_REP_REPORTE_VIAJESDataContext oList = new LinqBD_REP_REPORTE_VIAJESDataContext();
            var oListaC = from c in oList.TB_REP_DES_DESPACHADORES
                          where c.PK_REP_ESE_ID ==1
                          orderby c.FL_REP_DES_NOMBRES 
                          select new SelectListItem
                          {
                              Text = c.FL_REP_DES_NOMBRES +" "+ c.FL_REP_DES_APELLIDOS,
                              Value = c.PK_REP_DES_RUT.ToString(),
                          };
            ListaDespachadores = oListaC;
        }

        
        public void GetListaConductores()
        {
            LinqBD_PARDataContext oList = new LinqBD_PARDataContext();
            var oListaC = from c in oList.TB_PAR_CON_CONDUCTOR
                          where c.FL_PAR_CON_CODIGO != null && c.PK_PAR_EST_ID == 1
                          orderby c.FL_PAR_CON_CODIGO
                          select new SelectListItem
                          {
                              Text = string.Format("{0}-{1}",c.FL_PAR_CON_CODIGO,c.FL_PAR_CON_NOMBRE.ToUpper()),
                              Value= c.PK_PAR_CON_RUT.ToString(),
                          };
            ListaConductores = oListaC;
        }


        public void GetListaDespachadoresXSuc(int lcsuc)
        {
            LinqBD_REP_REPORTE_VIAJESDataContext oList = new LinqBD_REP_REPORTE_VIAJESDataContext();
            var oListaC = from c in oList.PR_REP_LISTA_DESPACHADORES_XSUCURSAL(lcsuc)
                          orderby c.FL_REP_DES_NOMBRES
                          select new SelectListItem
                          {
                              Text = c.FL_REP_DES_NOMBRES + " " + c.FL_REP_DES_APELLIDOS,
                              Value = c.PK_REP_DES_RUT.ToString(),
                          };
            ListaDespachadores = oListaC;
        }


        public void GetListaConductoresXSuc(int lcsuc)
        {
            LinqBD_REP_REPORTE_VIAJESDataContext oList = new LinqBD_REP_REPORTE_VIAJESDataContext();
            var oListaC = from c in oList.PR_REP_LISTA_CONDUCTORES_XSUCURSAL(lcsuc)
                          where c.FL_PAR_CON_CODIGO != null
                          select new SelectListItem
                          {
                              Text = string.Format("{0}-{1}", c.FL_PAR_CON_CODIGO, c.FL_PAR_CON_NOMBRE.ToUpper()),
                              Value = c.PK_PAR_CON_RUT.ToString(),
                          };
            ListaConductores = oListaC;
        }

        public void GetListaAsistentesXSuc(int lcsuc)
        {
            LinqBD_REP_REPORTE_VIAJESDataContext oList = new LinqBD_REP_REPORTE_VIAJESDataContext();
            var oListaA = from a in oList.PR_REP_LISTA_ASISTENTES_XSUCURSAL(lcsuc)
                          orderby a.FL_REP_ASI_CODIGO
                          select new SelectListItem
                          {
                              Text = a.FL_REP_ASI_CODIGO + " - " + a.FL_REP_ASI_NOMBRES + " " + a.FL_REP_ASI_APELLIDOS,
                              Value = a.PK_REP_ASI_RUT.ToString(),
                          };
            ListaAsistentes = oListaA;
        }


        public void GetListaAsistentes()
        {
            LinqBD_REP_REPORTE_VIAJESDataContext oList = new LinqBD_REP_REPORTE_VIAJESDataContext();
            var oListaA = from a in oList.TB_REP_ASI_ASISTENTES
                          where a.PK_REP_EAS_ID==1
                          orderby a.FL_REP_ASI_CODIGO
                          select new SelectListItem
                          {
                              Text = a.FL_REP_ASI_CODIGO + " - "+ a.FL_REP_ASI_NOMBRES +" "+ a.FL_REP_ASI_APELLIDOS,
                              Value=a.PK_REP_ASI_RUT.ToString(),
                          };
            ListaAsistentes = oListaA;
        }


        
        public void GetListaAreas()
        {
            LinqBD_REP_REPORTE_VIAJESDataContext oList = new LinqBD_REP_REPORTE_VIAJESDataContext();
            var oListaA = from a in oList.TB_REP_ARE_AREA
                          orderby a.FL_REP_ARE_DESCRIPCION
                          select new SelectListItem
                          {
                              Text = a.FL_REP_ARE_DESCRIPCION,
                              Value = a.PK_REP_ARE_ID.ToString(),
                          };
            ListaAreas = oListaA;
        }

        public void GetListaZonas2()
        {
            LinqBD_REP_REPORTE_VIAJESDataContext oList = new LinqBD_REP_REPORTE_VIAJESDataContext();
            var oListaA = from a in oList.TB_REP_ZON_ZONA
                          orderby a.FL_REP_ZON_DESCRIPCION
                          select new SelectListItem
                          {
                              Text = a.FL_REP_ZON_DESCRIPCION,
                              Value = a.PK_REP_ZON_ID.ToString(),
                          };
            ListaZonas = oListaA;
        }

        public void GetListaServicios()
        {
            LinqBD_REP_REPORTE_VIAJESDataContext oList = new LinqBD_REP_REPORTE_VIAJESDataContext();
            var oListaA = from a in oList.TB_REP_SER_SERVICIO
                          orderby a.FL_REP_SER_DESCRIPCION
                          select new SelectListItem
                          {
                              Text = a.FL_REP_SER_DESCRIPCION,
                              Value = a.PK_REP_SER_ID.ToString(),
                          };
            ListaServicios = oListaA;
        }


        public String DatosVehiculo (int idVehiculo)
        {
            LinqBD_PARDataContext oDvehi = new LinqBD_PARDataContext();
            var oDatos = (from v in oDvehi.TB_PAR_TRA_TRANSPORTE
                         where v.PK_PAR_TRA_ID==idVehiculo
                         select v).Single();
            VEHICULO=oDatos.FL_PAR_TRA_PATENTE;
            CAPKG = Convert.ToInt32(oDatos.FL_PAR_TRA_CAPACIDAD_KGS);
            CAPM3 = Convert.ToInt32(oDatos.FL_PAR_TRA_CAPACIDAD_M3);
            return oDatos.FL_PAR_TRA_MODELO;
         
        }

        public String DatosUsuario(int rutUsuario)
        {
            LinqBD_PARDataContext oDusu = new LinqBD_PARDataContext();
            var uDatos = (from u in oDusu.TB_PAR_USU_USUARIO
                          where u.PK_PAR_USU_RUT == rutUsuario
                          select u).Single();
            return uDatos.FL_PAR_USU_NOMBRES + ' ' + uDatos.FL_PAR_USU_APELLIDOS;       
        }





        public void Rescatarmpla()
        {

                LinqBD_REP_REPORTE_VIAJESDataContext oFol = new LinqBD_REP_REPORTE_VIAJESDataContext();
                var fDatos = from f in oFol.TB_REP_RAM_RAMPLAS
                             where f.PK_PAR_CAB_FOLIO == Convert.ToInt32(FOLIO)
                             select f;

                 
                foreach (var oFila in fDatos)
                {
                    if (oFila.PK_PAR_CAB_FOLIO >0)
                    {
                        DatosRampla(oFila.PK_PAR_TRA_ID);
                    }
                }
        
        }

        

        
        public void DatosRampla(int idVehiculo)
        {
            LinqBD_PARDataContext oDvehi = new LinqBD_PARDataContext();
            var oDatos = from v in oDvehi.TB_PAR_TRA_TRANSPORTE
                         where v.PK_PAR_TRA_ID == idVehiculo
                         select v;
            if (oDatos.Count() > 0)
            {
                var oRampla = oDatos.ToList()[0];
                RAMPLA = oRampla.PK_PAR_TRA_ID.ToString();
                CAPKGRAMPLA = Convert.ToInt32(oRampla.FL_PAR_TRA_CAPACIDAD_KGS);
                CAPM3RAMPLA = Convert.ToInt32(oRampla.FL_PAR_TRA_CAPACIDAD_M3);
            }
            else
            {
                RAMPLA = "0";
                CAPKGRAMPLA = 0;
                CAPM3RAMPLA = 0;
            }

        }


        public void GetListaRamplas()
        {
            LinqBD_PARDataContext oList = new LinqBD_PARDataContext();
            var oListaV = from v in oList.TB_PAR_TRA_TRANSPORTE
                          orderby v.FL_PAR_TRA_PATENTE
                          where v.FL_PAR_TRA_RAMPLA == true
                          select new SelectListItem
                          {
                              Text = v.FL_PAR_TRA_PATENTE,
                              Value = v.PK_PAR_TRA_ID.ToString(),
                          };

            ListaRamplas = oListaV;
        }


        
        public RetornoModels AnulaFolio(int folA)
        {
            RetornoModels oRetorno = new RetornoModels();
            try
            {
                LinqBD_REP_REPORTE_VIAJESDataContext oFolA = new LinqBD_REP_REPORTE_VIAJESDataContext();
                oFolA.PR_REP_ANULAR_FOLIO(folA);
                oRetorno.Mensaje = "Folio Anulado Exitosamente!";
                oRetorno.Ok = true;
            }
            catch (Exception e)
            {
                oRetorno.Mensaje = string.Format("Se ha producido un error: {0}", e.Message);
                oRetorno.Ok = false;
            }
            return oRetorno;
        }


        public RetornoModels CierraFolio(int folC)
        {
            RetornoModels oRetorno = new RetornoModels();
            try
            {
                LinqBD_REP_REPORTE_VIAJESDataContext oFolC = new LinqBD_REP_REPORTE_VIAJESDataContext();
                oFolC.PR_REP_CERRAR_FOLIO(folC);
                oRetorno.Mensaje = "Folio Cerrado Exitosamente!";
                oRetorno.Ok = true;
            }
            catch (Exception e)
            {
                oRetorno.Mensaje = string.Format("Se ha producido un error: {0}", e.Message);
                oRetorno.Ok = false;
            }
            return oRetorno;
        }

        public RetornoModels ActivaFolio(int folV)
        {
            RetornoModels oRetorno = new RetornoModels();
            try
            {
                LinqBD_REP_REPORTE_VIAJESDataContext oFolC = new LinqBD_REP_REPORTE_VIAJESDataContext();
                oFolC.PR_REP_ACTIVAR_FOLIO(folV);
                oRetorno.Mensaje = "Folio Activado Exitosamente!";
                oRetorno.Ok = true;
            }
            catch (Exception e)
            {
                oRetorno.Mensaje = string.Format("Se ha producido un error: {0}", e.Message);
                oRetorno.Ok = false;
            }
            return oRetorno;
        }

        

        public RetornoModels ActualizaCabecera(int usu, int suc)
        {
            RetornoModels oRetorno = new RetornoModels();
            try
            {

                float okgtrans = 0;
                //okgtrans = val.Replace("á", "a");
                float.TryParse(KGTRANS, out okgtrans);

                LinqBD_REP_REPORTE_VIAJESDataContext oFol = new LinqBD_REP_REPORTE_VIAJESDataContext();
                oFol.PR_REP_ACT_ACTUALIZA_CABECERA(Convert.ToInt32(FOLIO), NUMBULTOS, okgtrans, AREA, ZONA, SERVICIO, Convert.ToInt32(CONDUCTOR), Convert.ToInt32(ASISTENTE), Convert.ToInt32(DESPACHADOR), Convert.ToInt32(RAMPLA), usu, suc,REFERENCIA, OBSERVACIONCAB);
                oRetorno.Mensaje = "Registro actualizado Exitosamente!";
                oRetorno.Ok = true;
            }
            catch (Exception e)
            {
                oRetorno.Mensaje = string.Format("Se ha producido un error: {0}", e.Message);
                oRetorno.Ok = false;
            }
            return oRetorno;

        }



        public RetornoModels AgregaAsistente()
        {
            RetornoModels oRetorno = new RetornoModels();
            try
            {
                LinqBD_REP_REPORTE_VIAJESDataContext oFol = new LinqBD_REP_REPORTE_VIAJESDataContext();
                oFol.PR_REP_AGREGA_ASISTENTE(Convert.ToInt32(FOLIO), ASISTENTE);
                oRetorno.Mensaje = "Registro actualizado Exitosamente!";
                oRetorno.Ok = true;
            }
            catch (Exception e)
            {
                oRetorno.Mensaje = string.Format("Se ha producido un error: {0}", e.Message);
                oRetorno.Ok = false;
            }
            return oRetorno;

        }

        public RetornoModels mReasignaPatente(int usu)
        {
            RetornoModels oRetorno = new RetornoModels();
            try
            {
                LinqBD_REP_REPORTE_VIAJESDataContext oFol = new LinqBD_REP_REPORTE_VIAJESDataContext();
                oFol.PR_REP_REASIGNA_PATENTES_XFOLIO(FOLIOABUSCAR, Convert.ToInt32(VEHICULO),usu);
                oRetorno.Mensaje = "Registro actualizado Exitosamente!";
                oRetorno.Ok = true;
            }
            catch (Exception e)
            {
                oRetorno.Mensaje = string.Format("Se ha producido un error: {0}", e.Message);
                oRetorno.Ok = false;
            }
            return oRetorno;

        }



        public RetornoModels ActualizaFondos()
        {
          RetornoModels oRetorno = new RetornoModels();
          try
          {
              LinqBD_REP_REPORTE_VIAJESDataContext oFol = new LinqBD_REP_REPORTE_VIAJESDataContext();
              oFol.PR_REP_ACT_ACTUALIZA_FONDOS(Convert.ToInt32(FOLIO), FARENDIR, TRENDIDO, MDEVOLRECIB);
              oRetorno.Mensaje = "Registro actualizado Exitosamente!";
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


    public class IngresoFoliosModels
    {
        
        [Required(ErrorMessage = "La patente es requerida")]
        [Display(Name = "Patente")]
        public int VEHICULO { get; set; }

        [Required(ErrorMessage="El folio desde es requerido")]
        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        [Display(Name = "Desde")]
        public int DESDE { get; set; }

        [Required(ErrorMessage = "El folio hasta es requerido")]
        [RegularExpression("\\d*[0-9]$", ErrorMessage = "Sólo se admiten Números")]
        [Display(Name = "Hasta")]
        public int HASTA { get; set; }

        public int IDFOLIO { get; set; }

        public IEnumerable<VerFolios> DetalleFolios { get; set; }
        public IEnumerable<SelectListItem> ListaVehiculos { get; set; }
       

        public void GetListaVehiculos()
        {
            LinqBD_PARDataContext oList = new LinqBD_PARDataContext();
            var oListaV = from v in oList.TB_PAR_TRA_TRANSPORTE
                          orderby v.FL_PAR_TRA_PATENTE where v.FL_PAR_TRA_RAMPLA == false
                          select new SelectListItem
                          {
                              Text = v.FL_PAR_TRA_PATENTE,
                              Value = v.PK_PAR_TRA_ID.ToString(),
                          };

            ListaVehiculos = oListaV;
        }

        

        public void RescatarDatosFolio()
        {

            
            var oDataContex = new LinqBD_REP_REPORTE_VIAJESDataContext();
            var oFolios = from f in oDataContex.PR_REP_VER_FOLIOS()
                          select new VerFolios
                          {
                              Fid = f.PK_REP_FOL_ID_CARGA,
                              Fdesde = f.PK_REP_FOL_DESDE,
                              Fhasta = f.PK_REP_FOL_HASTA,
                              FTrans = f.FL_PAR_TRA_PATENTE,
                              Ffecha = f.FL_REP_FOL_FECHA,
                              Frut = f.USUARIO_CARGA
                          };
            DetalleFolios = oFolios.ToList();
        }





        public RetornoModels AsignaFolios(int usu)
        {
            RetornoModels oRetorno = new RetornoModels();
            try
            {
                LinqBD_REP_REPORTE_VIAJESDataContext oFol = new LinqBD_REP_REPORTE_VIAJESDataContext();
                oFol.PR_REP_ASI_ASIGNA_FOLIOS(DESDE, HASTA, VEHICULO,usu);
                oRetorno.Mensaje = "Registro ingresado Exitosamente!";
                oRetorno.Ok=true;
            }
            catch (Exception e)
            {
                oRetorno.Mensaje = string.Format("Se ha producido un error: {0}", e.Message);
                oRetorno.Ok=false;
            }
            return oRetorno;
        }


        public RetornoModels EliminaFolios()
        {
            RetornoModels oRetorno = new RetornoModels();
            try
            {
                LinqBD_REP_REPORTE_VIAJESDataContext oFol = new LinqBD_REP_REPORTE_VIAJESDataContext();

                oFol.PR_REP_ELIMINA_FOLIOS(IDFOLIO);
                oRetorno.Mensaje="Registro eliminado Correctamente!";
                oRetorno.Ok=true;
            }
            catch (Exception e)
            {
                oRetorno.Mensaje= string.Format("Se ha producido un error: {0}", e.Message);
                oRetorno.Ok=false;
            }
            return oRetorno;

        }



    }
}