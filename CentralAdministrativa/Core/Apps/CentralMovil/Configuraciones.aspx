<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Configuraciones.aspx.cs" Inherits="CentralMovil.Configuraciones" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 <ext:FormPanel ID="FormPanel1" Visible="false" runat="server">
        <Items>
            <ext:Panel ID="Panel1" runat="server" Title="Datos Usuario" AutoHeight="true" LabelAlign="Top"
                FormGroup="true" Layout="FormLayout" Width="428px">
                <Items>
                    <ext:TextField ID="txtNombre" FieldLabel="Nombre de Variable" Enabled="false" Selectable="false"
                        runat="server" MaxLength="20" Text="" Width="130" MsgTarget="Side" AllowBlank="false"
                        AnchorHorizontal="90%" />
                    <ext:TextField ID="txtValor" FieldLabel="Valor" MaxLength="20" InputType="Password"
                        runat="server" Width="130" MsgTarget="Side" AllowBlank="false" AnchorHorizontal="90%">
                    </ext:TextField>
                    <ext:TextField ID="txtDescripcion" FieldLabel="Descripcion" MaxLength="20" runat="server"
                        Text="" MsgTarget="Side" AllowBlank="false" AnchorHorizontal="90%">
                    </ext:TextField>
                </Items>
            </ext:Panel>
        </Items>
        <FooterBar>
            <ext:Toolbar ID="Toolbar1" runat="server" EnableOverflow="true">
                <Items>
                    <ext:Button ID="btnGuardar" runat="server" Text="Guardar" Icon="Add">
                        <DirectEvents>
                            <Click OnEvent="btnGuardar_Click" Before="var valid= #{FormPanel1}.getForm().isValid(); if (!valid) {} return valid;" />
                        </DirectEvents>
                    </ext:Button>
                </Items>
            </ext:Toolbar>
        </FooterBar>
    </ext:FormPanel>
    <ext:PropertyGrid ID="Propiedades" runat="server" Width="700" AutoHeight="true">
        <Source>
            <ext:PropertyGridParameter Name="(Aplicacion)" Value="Usuarios" />
        </Source>
        <View>
            <ext:GridView ForceFit="true" ScrollOffset="2" runat="server" />
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
