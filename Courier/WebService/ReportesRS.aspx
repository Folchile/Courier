<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportesRS.aspx.cs" Inherits="Courier.WebService.ReportesRS" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Font-Names="Verdana" 
            Font-Size="8pt" InteractiveDeviceInfos="(Colección)" ProcessingMode="Remote" 
            WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" Height="100%" 
            Width="100%" ZoomMode="FullPage">
            <ServerReport ReportPath="/Graficos_FOLWEB/ReporteOtDiaSamsung" 
                ReportServerUrl="http://192.168.33.4:81/ReportServer" />
        </rsweb:ReportViewer>
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    </div>
    </form>
</body>
</html>
