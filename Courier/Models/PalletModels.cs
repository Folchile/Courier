using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;



namespace Courier.Models
{

         

    public class VerDetallePallet
    {
        public decimal id { get; set; }
        public decimal Dpallet { get; set; }
        public decimal Dbultos { get; set; }
        public string Destado { get; set; }
    }

    public class VerDoctosAsoc
    {
       public decimal Ddocumento { get; set; }
    }


    public class PalletModels
    {
        public int IDPALLET { get; set; }
        public int BODEGACLIENTE { get; set; }
        public decimal OTP { get; set; }
        public decimal OTD { get; set; }

        [Required]
        [Display(Name = "N° Documento")]
        public string Documento { get; set; }


        [Required]
        [Display(Name = "N° Documento")]
        [Remote("jSonValidarDocumentoBuscar", "Validation")]
        public string ValidarDocumentoBuscar { get; set; }

        
        [Display(Name = "Rut Empresa")]
        public int RutEmpresa { get; set; }

        
        [Display(Name = "Razon Social")]
        public string RazonSocial { get; set; }

        
        [Display(Name = "Servicio")]
        public string Servicio { get; set; }

       
        
        [Display(Name = "Rut Destinatario")]
        public string RutDestinatario { get; set; }

        
        [Display(Name = "Nombre Destinatario")]
        public string NombreDestinatario { get; set; }

        
        [Display(Name = "Comuna")]
        public string Comuna { get; set; }

       
        [Display(Name = "Dirección Destinatario")]
        public string DireccionDestinatario { get; set; }

       
        [Display(Name = "Bultos")]
        public decimal Bultos { get; set; }

       
        [Display(Name = "Peso")]
        public double Peso { get; set; }

        public string NombreImpresora { get; set; }
       
        [Display(Name = "OT")]
        public string OT { get; set; }

        [Display(Name = "Pallet")]
        public double Pallet { get; set; }

        [Required]
        [Display(Name = "Bultos")]
        public double BultosDet { get; set; }



        public IEnumerable<VerDetallePallet> LstPallet { get; set; }
        public IEnumerable<VerDoctosAsoc> LstDoctos { get; set; }


        public void GetListaDoctosAsoc(decimal POTP, decimal POTD)
        {

            var oDataContext = new LinqBD_FOLDataContext();
            var odetDoctos = from p in oDataContext.PR_FOL_VER_DOCTOS_ASOCIADOS(POTP,POTD)
                             select new VerDoctosAsoc
                           {

                               Ddocumento = p.PK_FOL_DOA_NUMERO
                             
                           };
            LstDoctos = odetDoctos.ToList();
        }


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


        public void GetListaDetallePallet(decimal POTP,decimal POTD)
        {

            var oDataContext = new LinqBD_FOLDataContext();
            var odetPall = from p in oDataContext.PR_FOL_VER_PALLET(POTP,POTD)
                           select new VerDetallePallet
                          {
                              
                              id=p.PK_FOL_PAO_ID,
                              Dpallet=p.PK_FOL_PAO_NUMERO_PALLET,
                              Dbultos=p.FL_FOL_PAO_BULTOS,
                              Destado=p.FL_FOL_EPA_NOMBRE
                          };
            LstPallet = odetPall.ToList();
        }

        public Clases.Retorno InsertPallet(decimal otpp, decimal otde,int usu)
        {

            Clases.Retorno oRetorno = new Clases.Retorno();
            try
            {
                LinqBD_FOLDataContext dpallet = new LinqBD_FOLDataContext();
                dpallet.PR_FOL_AGREGA_PALLET(otpp, otde, Convert.ToDecimal(Pallet), Convert.ToDecimal(BultosDet), usu);
                oRetorno.bul = BultosDet;
                oRetorno.pall = Pallet;
                oRetorno.Mensaje = "Pallet ingresado exitosamente !";
                oRetorno.Ok = true;
            }
            catch (Exception e)
            {
                oRetorno.Mensaje = string.Format("Se ha producido un error: {0}", e.Message);
                oRetorno.Ok = false;
            }
            return oRetorno;
        }




        public Clases.Retorno FinishProcess(decimal otpp, decimal otde)
        {

            Clases.Retorno oRetorno = new Clases.Retorno();
            try
            {
                LinqBD_FOLDataContext dpallet = new LinqBD_FOLDataContext();
                dpallet.PR_FOL_FIN_PROCESO_PALLET(otpp, otde);
                oRetorno.Mensaje = "Proceso Finalizado exitosamente !";
                oRetorno.Ok = true;
            }
            catch (Exception e)
            {
                oRetorno.Mensaje = string.Format("Se ha producido un error: {0}", e.Message);
                oRetorno.Ok = false;
            }
            return oRetorno;
        }



        public RetornoModels DelPall(int id)
        {
            RetornoModels oRetorno = new RetornoModels();
            try
            {
                LinqBD_FOLDataContext oPall = new LinqBD_FOLDataContext();
                oPall.PR_FOL_ELIMINA_PALLET(id);
                oRetorno.Mensaje = "Pallet Eliminado Exitosamente!";
                oRetorno.Ok = true;
            }
            catch (Exception e)
            {
                oRetorno.Mensaje = string.Format("Se ha producido un error: {0}", e.Message);
                oRetorno.Ok = false;
            }
            return oRetorno;
        }


        public Clases.Retorno UpdatePall(decimal oTP, decimal oTD, int iDP)
        {
            Clases.Retorno oRetorno = new Clases.Retorno();
            try
            {
                LinqBD_FOLDataContext dPall = new LinqBD_FOLDataContext();
                dPall.PR_FOL_UPDATE_PALLET(oTP, oTD, iDP, Convert.ToDecimal(BultosDet));
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
}