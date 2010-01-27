<%@ Page Title="LABjs.Net Content Page Sample" Language="vb" AutoEventWireup="false" MasterPageFile="~/Sample.Master" CodeBehind="Content.aspx.vb" Inherits="LABjs.Sample.Content" %>
<%@ Register TagPrefix="lab" Namespace="LABjs.Net" Assembly="LABjs.Net" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <lab:LabScriptManagerProxy runat="server">
        <lab:LabActionGroup InsertAt="jQuery">
            <lab:LabScriptReference Path="script.js" />
            
            <lab:LabWait>
                <script type="text/javascript">
                    $('p:last').after('<p style="font-weight: bold">It Works!</p>');
                </script>
            </lab:LabWait>
        </lab:LabActionGroup>
        
        <lab:LabWait>
            $('p:last').after('<p style="font-weight: bold">And it works some more</p>');
        </lab:LabWait>
    </lab:LabScriptManagerProxy>
    
    <div>
        <h1>LABjs.Net Content Page Sample</h1>
        
        <p>This is a test of the LABjs helper interface for ASP.Net</p>
        
        <p><a href="Default.aspx">Switch To Regular Page</a></p>
    </div>
</asp:Content>
