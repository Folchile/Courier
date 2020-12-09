using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Courier.Models
{
    public class DocumentoModels
    {
        #region ConsultaHoy
        public class ConsultaHoy
        {
            public decimal DOA_ID { get; set; }
            public decimal DOA_NUMERO{ get; set; }
            public DateTime RDO_FECHA{ get; set; }
            public string SUC_NOMBRE { get; set; }
            public string EDO_NOMBRE { get; set; }
            public string USU_NOMBRE { get; set; }
            public string EMP_NOMBRE { get; set; }
            public string TDO_NOMBRE { get; set; }
            public decimal OTP { get; set; }
            public decimal OTD { get; set; }
        }
        #endregion
        public decimal OTP { get; set; }
        public decimal OTD { get; set; }

        public List<Courier.Models.DocumentosModels.ElementDOA> ListaDocumentos { get; set; }

        [Display (Name="N° Documento")]
        public string NumeroDocumento { get; set; }

        [Required]
        [Display(Name = "N° Documento")]
        [Remote ("vJsonValidarDocumentoOk","Validation")]
        public string ValidationNumeroDocumento { get; set; }

        [Required]
        [Display(Name = "N° Documento")]
        [Remote("vJsonValidarDocumentoOkSalida", "Validation")]
        public string ValidationNumeroDocumentoSalida { get; set; }

        [Required]
        [Display(Name = "N° Documento")]
        [Remote("vJsonValidarDocumentoOkRendicion", "Validation")]
        public string ValidationNumeroDocumentoRendicion { get; set; }

        [Required]
        [Display(Name = "N° Documento")]
        [Remote("jSonValidarDocumentoBuscar", "Validation")]
        public string ValidarDocumentoBuscar { get; set; }

        public IEnumerable<ConsultaHoy> ListaHoy { get; set; }

        #region GetListaHoy
        public void GetListaHoy(int SUC_ID, int EDO_ID)
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from pr in oFol.PR_FOL_RDO_CONSULTA_HOY(SUC_ID, EDO_ID)
                         select new ConsultaHoy {
                            DOA_NUMERO=pr.PK_FOL_DOA_NUMERO,
                            EDO_NOMBRE=pr.FL_FOL_EDO_NOMBRE,
                            RDO_FECHA=pr.FL_FOL_RDO_FECHA,
                            SUC_NOMBRE=pr.FL_PAR_SUC_NOMBRE,
                            USU_NOMBRE=pr.USUARIO,
                            EMP_NOMBRE=pr.FL_CLI_EMP_RAZON_SOCIAL,
                            TDO_NOMBRE=pr.FL_PAR_TDO_NOMBRE,
                            OTP=pr.PK_FOL_OTP_ID,
                            OTD=pr.PK_FOL_OTD_ID,
                            DOA_ID=pr.PK_FOL_DOA_ID
                         };
            ListaHoy = oDatos.ToArray();
        }
        #endregion
        #region GetListaDocumentos
        public void GetListaDocumentos()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oResult = oFol.PR_FOL_DOA_CONSULTA(OTP, OTD);
            List<Courier.Models.DocumentosModels.ElementDOA> lstDocs = new List<Courier.Models.DocumentosModels.ElementDOA>();
            foreach (var oFila in oResult)
            {
                lstDocs.Add(new Courier.Models.DocumentosModels.ElementDOA
                {
                    Fecha = oFila.FL_FOL_DOA_FECHA,
                    Nombre = oFila.FL_PAR_TDO_NOMBRE,
                    Numero = oFila.PK_FOL_DOA_NUMERO
                });
            }
            ListaDocumentos = lstDocs;
        }
        #endregion
        #region GetListaBuscar
        public void GetListaBuscar()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from pr in oFol.PR_FOL_RDO_BUSCAR(OTP,OTD)
                         select new ConsultaHoy
                         {
                             DOA_NUMERO = pr.PK_FOL_DOA_NUMERO,
                             EDO_NOMBRE = pr.FL_FOL_EDO_NOMBRE,
                             RDO_FECHA = pr.FL_FOL_RDO_FECHA,
                             SUC_NOMBRE = pr.FL_PAR_SUC_NOMBRE,
                             USU_NOMBRE = pr.USUARIO,
                             EMP_NOMBRE = pr.FL_CLI_EMP_RAZON_SOCIAL,
                             TDO_NOMBRE = pr.FL_PAR_TDO_NOMBRE,
                             OTP = pr.PK_FOL_OTP_ID,
                             OTD = pr.PK_FOL_OTD_ID,
                             DOA_ID = pr.PK_FOL_DOA_ID
                         };
            ListaHoy = oDatos.ToArray();
        }
        #endregion
       
    }
    public class ManifiestoDocumentoModels
    {
        [Required]
        [RegularExpression("\\d*[0-9]", ErrorMessage = "El valor debe ser un número Ej. 123456")]
        [Display(Name = "N° Manifiesto")]
        [Remote("JsonValidaManifiestoDocumento", "Validation")]
        public string CodBuscarManifiesto { get; set; }

        [Required]
        [RegularExpression("\\d*[0-9]", ErrorMessage = "El valor debe ser un número Ej. 123456")]
        [Display(Name = "N° Manifiesto")]
        public string CodManifiesto { get; set; }

        [Required]
        [RegularExpression("\\d*[0-9]", ErrorMessage = "El valor debe ser un número Ej. 123456")]
        public string CodDocumento { get; set; }


        public string Estado { get; set; }
        public DateTime Fecha { get; set; }

        [Display(Name = "N° Documento (OT)")]
        public string NumeroDocumento { get; set; }

        public int TDO_ID { get; set; }
        public decimal DOA_NUMERO { get; set; }

        public string Cliente { get; set; }
        public int RutCliente { get; set; }

        public IEnumerable<VI_FOL_MDO_LISTA_DOA> ListaDOA { get; set; }

        public void GetDatosManifiesto()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = (from mdo in oFol.TB_FOL_MDO_MANIFIESTO_DOCUMENTO
                          where mdo.PK_FOL_MDO_ID == Convert.ToInt32(CodBuscarManifiesto)
                          select mdo).Single();
            Estado = oDatos.TB_FOL_MAE_MANIFIESTO_ESTADO.FL_FOL_MAE_NOMBRE;
            Fecha = oDatos.PK_FOL_MDO_FECHA;
            if (oDatos.PK_CLI_EMP_RUT != null)
            {
                try
                {
                    LinqCLIDataContext oCli = new LinqCLIDataContext();
                    var oEMP = (from emp in oCli.TB_CLI_EMP_EMPRESAS
                                where emp.PK_CLI_EMP_RUT == oDatos.PK_CLI_EMP_RUT
                                select emp).Single();
                    Cliente = oEMP.FL_CLI_EMP_RAZON_SOCIAL;
                    RutCliente = oEMP.PK_CLI_EMP_RUT;
                }
                catch
                {

                }
            }
        }
        public void GetDatosListaDOA()
        {
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var oDatos = from vi in oFol.VI_FOL_MDO_LISTA_DOA
                         where vi.PK_FOL_MDO_ID == Convert.ToInt32(CodBuscarManifiesto)
                         select vi;
            ListaDOA = oDatos;
        }
    }

    public class RendicionMasivaModels{

    }
            
    
    

}