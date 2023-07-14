<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CambioNivelMoshi.aspx.cs" Inherits="Lealtad.CambioNivelMoshi" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .cbStates-list
        {
            width: auto;
            font: 11px tahoma,arial,helvetica,sans-serif;
        }
        
        .cbStates-list th
        {
            font-weight: bold;
        }
        
        .cbStates-list td, .cbStates-list th
        {
            padding: 3px;
        }
        .Titulo
        {
            font-size: 20px;
            font-weight: bolder;
            font-family: Arial Unicode MS;
            color: Black;
        }
        
        .descripcion
        {
            font-size: 12px;
            text-justify: distribute;
            font-weight: normal;
            font-family: Arial Unicode MS;
            color: Black;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="MainBorderLayout" runat="server">
        <Content>
            <ext:Store ID="StoreNivelLealtad" runat="server">
                <Reader>
                    <ext:JsonReader IDProperty="ID_GrupoCuenta">
                        <Fields>
                            <ext:RecordField Name="ID_GrupoCuenta" />
                            <ext:RecordField Name="Descripcion" />
                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>
        </Content>
        <Center Split="true">
            <ext:Panel ID="PanelGeneral" runat="server" Layout="FitLayout" Title="Configuración de Regla">
                <Items>
                    <ext:BorderLayout ID="InternalBorderLayout" runat="server">
                        <Center Split="true">
                            <ext:FormPanel ID="FormPanel1" runat="server" Border="false" Layout="ColumnLayout"
                                Height="210" >
                                <Items>
                                    <ext:Panel ID="PanelColumnDummy" runat="server" Border="false" Width="50" Height="210">
                                        <Items>
                                        </Items>
                                    </ext:Panel>
                                   <ext:Panel ID="Panel1" runat="server" Border="false" Width="250" Height="210">
                                        <Items>
                                            <ext:Image ID="Image1" runat="server" Width="180" Height="150" ImageUrl="Images/cup.jpg">
                                            </ext:Image>
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel ID="Panel2" runat="server" Border="false" Layout="RowLayout" AutoScroll="true" Width="600">
                                        <Items>
                                            <ext:Panel ID="Panel_Dummy" runat="server" Border="false" Header="false" LabelAlign="Left"
                                                Layout="FormLayout">
                                                <Items>
                                                    <ext:Label ID="Label8" runat="server" Text="">
                                                    </ext:Label>
                                                </Items>
                                            </ext:Panel>
                                            <ext:Panel ID="Panel3" runat="server" Border="false" Header="false" LabelAlign="Left"
                                                Layout="FormLayout">
                                                <Items>
                                                    <ext:Label runat="server" ID="lblNombreRegla" Cls="Titulo" FieldLabel="Nombre de Regla"
                                                        Text="Cambio de Nivel">
                                                    </ext:Label>
                                                </Items>
                                            </ext:Panel>
                                            <ext:Panel ID="PanelDummy1" runat="server" Border="false" Header="false" LabelAlign="Left"
                                                Layout="FormLayout">
                                                <Items>
                                                    <ext:Label ID="Label2" runat="server" Text="">
                                                    </ext:Label>
                                                </Items>
                                            </ext:Panel>
                                            <ext:Panel ID="PanelDummy2" runat="server" Border="false" Header="false" LabelAlign="Left"
                                                Layout="FormLayout">
                                                <Items>
                                                    <ext:Label ID="Label1" runat="server" Text="">
                                                    </ext:Label>
                                                </Items>
                                            </ext:Panel>
                                            <ext:Panel ID="PanelDummy3" runat="server" Border="false" Header="false" LabelAlign="Left"
                                                Layout="FormLayout">
                                                <Items>
                                                    <ext:Label ID="Label3" runat="server" Text="">
                                                    </ext:Label>
                                                </Items>
                                            </ext:Panel>
                                            <ext:Panel ID="Panel4" runat="server" Border="false" Header="false" LabelAlign="Left" Layout="FormLayout">
                                                <Items>
                                                    <ext:Label runat="server" ID="lblDescripcionRegla" Cls="descripcion" FieldLabel="Descripción"
                                                        Text="Al cumplir un cliente determinado número de visitas, asciende al siguiente nivel de beneficios.">
                                                    </ext:Label>
                                                </Items>
                                            </ext:Panel>
                                            <ext:Panel ID="PanelDummy4" runat="server" Border="false" Header="false" LabelAlign="Left"
                                                Layout="FormLayout">
                                                <Items>
                                                    <ext:Label ID="Label4" runat="server" Text="">
                                                    </ext:Label>
                                                </Items>
                                            </ext:Panel>
                                            <ext:Panel ID="PanelDummy5" runat="server" Border="false" Header="false" LabelAlign="Left"
                                                Layout="FormLayout">
                                                <Items>
                                                    <ext:Label ID="Label5" runat="server" Text="">
                                                    </ext:Label>
                                                </Items>
                                            </ext:Panel>
                                            <ext:Panel ID="PanelDummy6" runat="server" Border="false" Header="false" LabelAlign="Left"
                                                Layout="FormLayout">
                                                <Items>
                                                    <ext:Label ID="Label6" runat="server" Text="">
                                                    </ext:Label>
                                                </Items>
                                            </ext:Panel>
                                            <ext:Panel ID="Panel5" runat="server" Border="false" Header="false" LabelAlign="Left"
                                                Layout="FormLayout">
                                                <Items>
                                                    <ext:ComboBox ID="cBoxNivelLealtad" FieldLabel="Nivel" ForceSelection="true"
                                                        EmptyText="Selecciona un Nivel..." runat="server" Width="380" StoreID="StoreNivelLealtad"
                                                        MsgTarget="Side" AllowBlank="false" DisplayField="Descripcion" ValueField="ID_GrupoCuenta"
                                                        Editable="false" TypeAhead="true" Mode="Local">
                                                        <DirectEvents>
                                                            <Select OnEvent="select_NivelLealtad" />
                                                        </DirectEvents>
                                                    </ext:ComboBox>
                                                </Items>
                                            </ext:Panel>
                                        </Items>
                                    </ext:Panel>
                                </Items>
                            </ext:FormPanel>
                        </Center>
                        <South Split="true">
                            <ext:Panel ID="Panel7" runat="server" Title="Parámetros de la Regla" Height="300"
                                Split="true" Padding="6" MinWidth="650" Collapsible="false" Layout="FitLayout">
                                <Items>
                                    <%-- Poner las variables automaticas --%>
                                    <ext:PropertyGrid ID="GridPropiedades" runat="server" Header="false">
                                        <Source>
                                            <ext:PropertyGridParameter Name="(Parámetros)" Value="Valores">
                                            </ext:PropertyGridParameter>
                                        </Source>
                                        <DirectEvents>
                                        </DirectEvents>
                                        <View>
                                            <ext:GridView ID="GridView2" ForceFit="true" ScrollOffset="2" runat="server" />
                                        </View>
                                        <FooterBar>
                                            <ext:Toolbar ID="Toolbar2" runat="server" EnableOverflow="true">
                                                <Items>
                                                    <ext:Button ID="btnGuardar" runat="server" Text="Guardar" Icon="Add">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnGuardar_Click" />
                                                        </DirectEvents>
                                                    </ext:Button>
                                                    <ext:Button ID="btnCancelar" runat="server" Text="Cancelar" Icon="Cancel">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnCancelar_Click" />
                                                        </DirectEvents>
                                                    </ext:Button>
                                                </Items>
                                            </ext:Toolbar>
                                        </FooterBar>
                                    </ext:PropertyGrid>
                                </Items>
                            </ext:Panel>
                        </South>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
