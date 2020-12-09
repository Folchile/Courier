using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using System.Security.Principal;
using System.Net;

namespace Courier.WebService
{
    public partial class ReportesRS : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
        protected void Page_Init(object sender, EventArgs e)
        {
            ReportViewer1.ServerReport.ReportServerCredentials = new ReportServerCredentials("administrador", "BlackBerry", "ANTARES");
        }
    }
    public class ReportServerCredentials : IReportServerCredentials
    {
        private string _userName;
        private string _password;
        private string _domain;

        public ReportServerCredentials(string userName, string password, string domain)
        {
            _userName = userName;
            _password = password;
            _domain = domain;
        }

        public WindowsIdentity ImpersonationUser
        {
            get
            {
                // Use default identity.
                return null;
            }
        }

        public ICredentials NetworkCredentials
        {
            get
            {
                // Use default identity.
                return new NetworkCredential(_userName, _password, _domain);
            }
        }

        public bool GetFormsCredentials(out Cookie authCookie, out string user, out string password, out string authority)
        {
            // Do not use forms credentials to authenticate.
            authCookie = null;
            user = password = authority = null;
            return false;
        }
    }
}