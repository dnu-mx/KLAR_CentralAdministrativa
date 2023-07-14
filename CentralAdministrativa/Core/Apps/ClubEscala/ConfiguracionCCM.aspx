<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ConfiguracionCCM.aspx.cs" Inherits="ClubEscala.ConfiguracionCCM" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Panel ID="Panel1" runat="server" AutoScroll="true" >
        <Items>
            <ext:PropertyGrid ID="Propiedades" AutoScroll="true" runat="server"  Border ="false" Height="500" Width="700">
                <Source>
                    <ext:PropertyGridParameter Name="(Aplicacion)" Value="Importador de Empleados">
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
        </Items>
    </ext:Panel>
</asp:Content>

