<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="LABjs.Sample._Default" %>
<%@ Register TagPrefix="lab" Namespace="LABjs.Net" Assembly="LABjs.Net" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>LABjs.Net Sample</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <lab:LabScriptManager ID="LabScriptManager1" runat="server" EnableCdnFailover="true">
            <lab:LabScriptReference Path="http://ajax.googleapis.com/ajax/libs/jquery/1.4.1/jquery.min.js" Test="jQuery">
                <lab:LabScriptReference Path="~/jquery-1.4.1.min.js" AllowDuplicates="No" />
            </lab:LabScriptReference>
            <lab:LabScriptCombine>
                <lab:LabScriptReference Path="~/script.js" />
                <lab:LabScriptReference Path="~/script2.js" />
            </lab:LabScriptCombine>
            <lab:LabWait>
                <script type="text/javascript">
                    Startup();
                    Startup2();
                </script>
            </lab:LabWait>
        </lab:LabScriptManager>
            
        <div>
            <h1>LABjs.Net Sample</h1>
            
            <p>This is a test of the LABjs helper interface for ASP.Net</p>
            
            <p><a href="Content.aspx">Switch To Content Page</a></p>
        </div>
    </form>
</body>
</html>
