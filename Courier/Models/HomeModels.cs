using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Courier.Models
{
    public class ComunasCoordenadasModels
    {
        public class Coordenada
        {
            public string Latitud { get; set;}
            public string Longitud { get; set; }
        }

        public IEnumerable<SelectListItem> oListado {get;set;}        

        public Coordenada cOrigen {get;set;}
        public Coordenada cDestino {get;set;}

        public string sOrigen { get; set; }
        public string sDestino { get; set; }

        public double Kilometros { set; get; }
    }
    public class SucursalIngresoModels
    {
        public bool SeleccionSucursal { get; set; }
        public string Sucursal {get;set;}

        public IEnumerable<SelectListItem> ListaSucursales { get; set; }

        public void GetListaSucursales()
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            Controllers.ValidationController oValidation = new Controllers.ValidationController();


            var oQry = (from pr in oPar.PR_PAR_SUCURSALES_USUARIO (oValidation.GetRutActiveUser())
                       select pr).ToList();
            List<SelectListItem> oList = new List<SelectListItem>();
            if (oQry.Count() <= 1)
            {
                SeleccionSucursal = false;
                
                ListaSucursales = oList;
            }
            else
            {
                SeleccionSucursal = true;
                int Suc = oValidation.GetSucursalIDfromActiveUser();
                
                foreach (var oFile in oQry)
                {
                    if (Suc == oFile.PK_PAR_SUC_ID)
                    {
                        oList.Add(new SelectListItem
                        {
                            Text = oFile.FL_PAR_SUC_NOMBRE,
                            Value = oFile.PK_PAR_SUC_ID.ToString(),
                            Selected=true
                        });
                    }
                    else
                    {
                        oList.Add(new SelectListItem
                        {
                            Text = oFile.FL_PAR_SUC_NOMBRE,
                            Value = oFile.PK_PAR_SUC_ID.ToString()
                        });
                    }
                }
                ListaSucursales = oList;
            }
        }
    }
}
