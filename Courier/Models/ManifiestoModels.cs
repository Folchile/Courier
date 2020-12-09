using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Courier.Models
{
    public class ListDestinoSucursal
    {
        public int Sucursal_Destino_ID { get; set; }
        public string Sucursal_Destino { get; set; }

        public string Sucursal_Bultos { get; set; }
        public int Sucursal_Bultos_ID { get; set; }

        public decimal Via_ID { get; set; }
        public string Via_Nombre { get; set; }

        public double? Peso_total { get; set; }
        public int? Cantidad_Bultos { get; set; }
        public int? Cantidad_OT { get; set; }
    }
    public class ManifiestoModels
    {

        public string IDMANIFIESTO { get; set; }
        public int BODEGACLIENTE { get; set; }


        [Required]
        [Display(Name = "N° Manifiesto")]
        public string Manifiesto { get; set; }

        public IEnumerable<ListDestinoSucursal> oListaDestino { get; set; }
        public IEnumerable<SelectListItem> oListaTransporte { get; set; }
        public IEnumerable<SelectListItem> oListaConductor { get; set; }

        public int SucursalActual { get; set; }

        [Required]
        public string Transporte { get; set; }

        [Required]
        public string Conductor { get; set; }

        public string ManifiestoPadre { get; set; }
        public IEnumerable<VI_FOL_MAN_CONSULTA_MAP> ListaManifiestoMap { get; set; }

        #region GetListaManifiestoMap
        public void GetListaManifiestoMap()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from vi in oFol.VI_FOL_MAN_CONSULTA_MAP
                         where vi.PK_FOL_MAP_ID == Convert.ToDecimal(ManifiestoPadre)
                         select vi;
            ListaManifiestoMap = oDatos;
        }
        #endregion
        #region GetDatosListaDestino
        public void GetDatosListaDestino()
        {
            Controllers.ValidationController oValidation = new Controllers.ValidationController();
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from pr in oFol.PR_FOL_MAN_DESTINO_RUTA_TRONCAL(SucursalActual)
                         orderby pr.FL_PAR_SUC_NOMBRE
                         select new ListDestinoSucursal
                         {
                             Sucursal_Bultos = pr.FL_PAR_SUC_NOMBRE,
                             Cantidad_Bultos = pr.CANTIDAD_BULTOS,
                             Cantidad_OT = pr.CANTIDAD_OT,
                             Peso_total = pr.PESO_KG_TOTAL,
                             Sucursal_Bultos_ID = pr.PK_PAR_SUC_ID,
                             Via_ID = pr.PK_PAR_VIA_ID,
                             Via_Nombre = pr.FL_PAR_VIA_NOMBRE
                         };
            if (Transporte != "0" && Transporte != "" && Transporte != null)
            {
                List<ListDestinoSucursal> oListaCustom = new List<ListDestinoSucursal>();
                LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
                foreach (var oFila in oDatos)
                {


                    var oConsultaCobertura = (from x in oPar.TB_PAR_COB_COBERTURA_COMUNA
                                              where x.PK_PAR_SUC_ID == SucursalActual
                                              select x).Count();

                    int oSucursal = 0;
                    if (oConsultaCobertura == 0)
                    {
                        var oCobertura = from cob in oPar.TB_PAR_COB_COBERTURA_COMUNA
                                         join suc in oPar.TB_PAR_SUC_SUCURSAL
                                         on cob.PK_PAR_COM_ID equals suc.PK_PAR_COM_ID
                                         select cob;
                        oSucursal = oCobertura.ToList()[0].PK_PAR_SUC_ID;
                    }
                    else
                    {
                        int iComuna = GetComunaFromSucursal(oFila.Sucursal_Bultos_ID);
                        var oLista = oValidation.GetListaOrigenDestino(SucursalActual, iComuna, Convert.ToInt32(oFila.Via_ID));

                        oSucursal = oValidation.fnSucDestinoManifiesto(oLista, Convert.ToInt32(Transporte), Convert.ToInt32(oFila.Via_ID));
                    }
                    if (oSucursal != 0 && oSucursal != SucursalActual)
                    {
                        ListDestinoSucursal oListaItem = new ListDestinoSucursal
                        {
                            Cantidad_Bultos = oFila.Cantidad_Bultos,
                            Cantidad_OT = oFila.Cantidad_OT,
                            Peso_total = oFila.Peso_total,
                            Sucursal_Bultos = oFila.Sucursal_Bultos,
                            Sucursal_Bultos_ID = oFila.Sucursal_Bultos_ID,
                            Sucursal_Destino = oValidation.GetSucursalNamefromIDSucursal(oSucursal),
                            Sucursal_Destino_ID = oSucursal,
                            Via_ID = oFila.Via_ID,
                            Via_Nombre = oFila.Via_Nombre
                        };
                        oListaCustom.Add(oListaItem);
                    }
                }
                oListaDestino = oListaCustom;
            }
            else
            {
                oListaDestino = oDatos.ToList();
            }
        }
        #endregion
        #region GetListaTransportes
        public void GetListaTransportes()
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();


            var oDatos = (from pr in oPar.PR_PAR_TRA_SALIDA_SUC(SucursalActual)
                          select pr).ToList();


            Controllers.ValidationController oValidation = new Controllers.ValidationController();
            List<SelectListItem> oSelect = new List<SelectListItem>();
            foreach (var oTra in oDatos)
            {
                ManifiestoModels oManif = new ManifiestoModels();

                oManif.SucursalActual = oValidation.GetSucursalIDfromActiveUser();
                oManif.Transporte = oTra.PK_PAR_TRA_ID.ToString();
                oManif.GetDatosListaDestino();

                foreach (var oDest in oManif.oListaDestino)
                {
                    var oValida = (from det in oPar.PR_PAR_DET_CONSULTA(oDest.Sucursal_Destino_ID, oTra.PK_PAR_TRA_ID)
                                   select det.CANTIDAD).Single();

                    if (oValida > 0)
                    {
                        if (oSelect.Where(x => x.Value == oTra.PK_PAR_TRA_ID.ToString()).Count() == 0)
                        {
                            oSelect.Add(new SelectListItem
                            {
                                Text = oTra.FL_PAR_TRA_PATENTE,
                                Value = oTra.PK_PAR_TRA_ID.ToString()
                            });
                        }
                    }
                }
            }

            //////var oListaTraNew = from tra in oSelect
            //////                   orderby tra.Value
            //////                   select tra;

            //////var txtTra = "";

            //////List<SelectListItem> oListNew = new List<SelectListItem>();
            //////foreach (var fil in oListaTraNew)
            //////{
            //////    if (fil.Value != txtTra)
            //////    {
            //////        oListNew.Add(fil);
            //////    }
            //////    txtTra = fil.Value;
            //////}

            //var oListNew = from x in oDatos
            //               select new SelectListItem
            //               {
            //                   Text = x.tra.FL_PAR_TRA_PATENTE.ToString(),
            //                   Value = x.tra.PK_PAR_TRA_ID.ToString()
            //               };

            oListaTransporte = oSelect;

        }
        #endregion
        #region GetComunaFromSucursal
        public int GetComunaFromSucursal(int Sucursal)
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            var oDatos = (from cob in oPar.TB_PAR_COB_COBERTURA_COMUNA
                          where cob.PK_PAR_SUC_ID == Sucursal
                          select cob).Take(1);
            return oDatos.ToList()[0].PK_PAR_COM_ID;
        }
        #endregion
    }
}