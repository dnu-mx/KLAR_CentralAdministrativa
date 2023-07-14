<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Configuraciones.aspx.cs" Inherits="Cortador.Configuraciones" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <ext:PropertyGrid ID="Propiedades" runat="server" Width="700" AutoHeight="true">
        <Source>
            <ext:PropertyGridParameter Name="(Aplicacion)"  Value="Cajero">
            
            </ext:PropertyGridParameter>
        </Source>
        <View>
            <ext:GridView ID="GridView1" ForceFit="true" ScrollOffset="2" runat="server" />
        </View>
        <Buttons>
            <ext:Button runat="server" ID="Button1" Text="Guardar Cambios" Icon="Disk">
                <DirectEvents>
                    <Click OnEvent="Button1_Click" />
                </DirectEvents>
            </ext:Button>
        </Buttons>
    </ext:PropertyGrid>

   

</asp:Content>
