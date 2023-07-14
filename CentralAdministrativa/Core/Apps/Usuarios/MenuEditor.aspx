<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="MenuEditor.aspx.cs"
    Inherits="Usuarios.MenuEditor" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript">
    var ActualizaNodo = function (tabPanel, node, ID_Rol) {
        // Call DirectMethod
        Usuarios.ActualizarMenu(node.id.toString(), ID_Rol, node.attributes.checked);
        }
    </script>
    <ext:BorderLayout ID="BorderlayCentro" runat="server">
        <Center>
            <ext:Panel ID="PnlMenu2" runat="server" Title="Menú General" Layout="accordion" Split="true"
                Collapsible="true">
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
