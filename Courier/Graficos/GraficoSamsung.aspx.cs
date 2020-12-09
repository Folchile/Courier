using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Microsoft.Reporting.WebForms;


namespace Courier.Graficos
{
    public partial class GraficoSamsung : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
      
            if (!Page.IsPostBack)
            {

                //BD_FOL_FOLCOURIERDataSet.PR_FOL_GRA_OTS_GRAL_DIADataTable dt = new BD_FOL_FOLCOURIERDataSet.PR_FOL_GRA_OTS_GRAL_DIADataTable();
                //BD_FOL_FOLCOURIERDataSetTableAdapters.PR_FOL_GRA_OTS_GRAL_DIATableAdapter da =
                //    new BD_FOL_FOLCOURIERDataSetTableAdapters.PR_FOL_GRA_OTS_GRAL_DIATableAdapter();
                //da.Fill(dt);

                //DataSet2.TB_PAR_EST_ESTADODataTable dt = new DataSet2.TB_PAR_EST_ESTADODataTable();
                //DataSet2TableAdapters.TB_PAR_EST_ESTADOTableAdapter da =
                //    new DataSet2TableAdapters.TB_PAR_EST_ESTADOTableAdapter();


                DataSet2.PR_FOL_GRA_OTS_GRAL_DIA_DETALLEDataTable dt2 = new DataSet2.PR_FOL_GRA_OTS_GRAL_DIA_DETALLEDataTable();
                DataSet2TableAdapters.PR_FOL_GRA_OTS_GRAL_DIA_DETALLETableAdapter da2 =
                    new DataSet2TableAdapters.PR_FOL_GRA_OTS_GRAL_DIA_DETALLETableAdapter();
                da2.Fill(dt2);
                ReportDataSource RD2 = new ReportDataSource();
                RD2.Value = dt2;
                RD2.Name = "DataSet2";


                DataSet1.PR_FOL_GRA_OTS_GRAL_DIADataTable dt = new DataSet1.PR_FOL_GRA_OTS_GRAL_DIADataTable();
                DataSet1TableAdapters.PR_FOL_GRA_OTS_GRAL_DIATableAdapter da =
                    new DataSet1TableAdapters.PR_FOL_GRA_OTS_GRAL_DIATableAdapter();

                da.Fill(dt);

                ReportDataSource RD = new ReportDataSource();
                RD.Value = dt;
                RD.Name = "DataSet1";

                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(RD);
                ReportViewer1.LocalReport.DataSources.Add(RD2);
                ReportViewer1.LocalReport.ReportEmbeddedResource = "Report1.rdlc";
                ReportViewer1.LocalReport.ReportPath = @"Report1.rdlc";
                ReportViewer1.LocalReport.Refresh();

                //if (dt.Rows.Count > 0)
                //{
                //    ReportViewer1.LocalReport.Refresh();
                //}



               
            }
        }
    }
}