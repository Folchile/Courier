using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Globalization;
using System.ComponentModel.DataAnnotations;

namespace Courier.Models
{

    public class VerTransportes
    {
        public int Did { get; set; }
        public string Dpatente { get; set; }
        public string Dmarca { get; set; }
        public string Dmodelo { get; set; }
        public int Destado { get; set; }
        public int Dano { get; set; }
        public decimal Dcapkgs { get; set; }
        public decimal Dcapm3 { get; set; }
    }

    public class VerSucursalesAsoc
    {
        public int Did { get; set; }
        public string Dsucursal { get; set; }
    }
    

    public class VehiculosModels
    {
        public int IDVEH { get; set; }
        public int IDSUC { get; set; }
                
        [Required]
        [Display(Name = "Patente")]
        public string BUSCARPATENTE { get; set; }

        [Required]
        //[RegularExpression("([A-Z0-9][A-Z0-9][A-Z0-9][A-Z0-9])-[A-Z0-9][A-Z0-9]$", ErrorMessage = "Campo Patente Formato incorrecto, ejemplo: XXXX-XX")]  
        [Display(Name = "Patente")]
        public string PATENTE { get; set; }

        [Required]
        [Display(Name = "Marca")]
        public string MARCA { get; set; }
        
        [Required]
        [Display(Name = "Modelo")]
        public string MODELO { get; set; }

        [Required]
        [Display(Name = "Sucursal")]
        public int SUCURSAL { get; set; }

        [Required]
        [Display(Name = "Año")]
        public int ANO { get; set; }

        [Required]
        [Display(Name = "Destino Cliente")]
        public Boolean DESTINOCLIENTE { get; set; }

        [Required]
        [Display(Name = "Rampla")]
        public Boolean RAMPLA { get; set; }

        
        [Display(Name = "Capacidad KGS")]
        public int CAPKILOS { get; set; }

        
        [Display(Name = "Capacidad M3")]
        public int CAPM3 { get; set; }




        public IEnumerable<VerSucursalesAsoc> LstSucDest { get; set; }
        public IEnumerable<VerTransportes> LstVehiculos { get; set; }
        public IEnumerable<SelectListItem> LstSucFilt { get; set; }


        public void GetListaSucAsoc(int idv)
        {
           
            var oDataContext = new LinqBD_FOLDataContext();
            var odetVeh = from m in oDataContext.PR_FOL_RESCATA_DESTINO_VEHI(idv)
                          select new VerSucursalesAsoc
                          {
                              Did=m.PK_PAR_SUC_ID,
                              Dsucursal=m.FL_PAR_SUC_NOMBRE,
                          };
            LstSucDest = odetVeh.ToList();
        }



        public RetornoModels CambiaestadoVehiculo(int patte, int tpo)
        {
            RetornoModels oRetorno = new RetornoModels();
            try
            {
                LinqBD_FOLDataContext oVeh = new LinqBD_FOLDataContext();
                oVeh.PR_FOL_CAMBIA_EST_VEHI(patte, tpo);    
                if (tpo == 1)
                {
                    oRetorno.Mensaje = "Vehiculo Activado Exitosamente!";
                }
                else
                {
                    oRetorno.Mensaje = "Vehiculo Inactivado Exitosamente!";
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







        public void GesListaSucFil(int idve)
        {
            LinqBD_FOLDataContext sList = new LinqBD_FOLDataContext();
            
            var sLista = from r in sList.PR_FOL_FILTRA_SUCURSALES(idve)
                         orderby r.FL_PAR_SUC_NOMBRE
                         select new SelectListItem
                         {
                             Text = r.FL_PAR_SUC_NOMBRE,
                             Value = r.PK_PAR_SUC_ID.ToString(),
                         };
            LstSucFilt = sLista;
        }



        public void GetListaVehiculos(string pat)
        {
            if (pat == null)
            {
                pat="";
            }
            string est;
            var oDataContext = new LinqBD_FOLDataContext();
            var odetVeh = from m in oDataContext.PR_FOL_LISTA_VEHICULOS(pat)
                          select new VerTransportes
                          {
                              Did = m.PK_PAR_TRA_ID,
                              Dpatente = m.FL_PAR_TRA_PATENTE,
                              Dmarca=m.FL_PAR_TRA_MARCA,
                              Dmodelo=m.FL_PAR_TRA_MODELO,
                              Destado =Convert.ToInt32( m.PK_PAR_ETR_ESTADO_TRANS), 
                              Dano =Convert.ToInt32(m.FL_PAR_TRA_ANIO),
                              Dcapkgs =Convert.ToInt32(m.FL_PAR_TRA_CAPACIDAD_KGS),
                              Dcapm3 =Convert.ToInt32(m.FL_PAR_TRA_CAPACIDAD_M3)
                          };
            LstVehiculos = odetVeh.ToList();
        }

        public Clases.Retorno InsertVehic()
        {
            
            Clases.Retorno oRetorno = new Clases.Retorno();
            try
            {
                LinqBD_FOLDataContext dVehi = new LinqBD_FOLDataContext();
                dVehi.PR_FOL_INSERT_VEHICULO(PATENTE, MARCA, MODELO, ANO, DESTINOCLIENTE, RAMPLA, CAPKILOS, CAPM3);
                oRetorno.Mensaje = "Vehiculo ingresado Exitosamente !";
                oRetorno.Ok = true;
            }
            catch (Exception e)
            {
                oRetorno.Mensaje = string.Format("Se ha producido un error: {0}", e.Message);
                oRetorno.Ok = false;
            }
            return oRetorno;
        }


        public Clases.Retorno InsertDestTrans()
        {

            Clases.Retorno oRetorno = new Clases.Retorno();
            try
            {
                LinqBD_FOLDataContext dVehi = new LinqBD_FOLDataContext();
                dVehi.PR_FOL_INSERT_DESTINO_TRANS(IDVEH, SUCURSAL);
                oRetorno.Mensaje = "Sucursal Asignada Exitosamente !";
                oRetorno.Ok = true;
            }
            catch (Exception e)
            {
                oRetorno.Mensaje = string.Format("Se ha producido un error: {0}", e.Message);
                oRetorno.Ok = false;
            }
            return oRetorno;
        }



        public Clases.Retorno UpdateVehiculo()
        {
            Clases.Retorno oRetorno = new Clases.Retorno();
            try
            {
                LinqBD_FOLDataContext dVeh = new LinqBD_FOLDataContext();
                dVeh.PR_FOL_ACTUALIZA_VEHICULO(IDVEH, MARCA, MODELO, ANO, DESTINOCLIENTE, RAMPLA, CAPKILOS, CAPM3);
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

        public Clases.Retorno DeleteRelVehic(int ids, int idv)
        {
            Clases.Retorno oRetorno = new Clases.Retorno();
            try
            {
                LinqBD_FOLDataContext dVeh = new LinqBD_FOLDataContext();
                dVeh.PR_FOL_ELIM_DESTINO_TRANS(idv,ids);
                oRetorno.Mensaje = "Registro eliminado Exitosamente !";
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
}