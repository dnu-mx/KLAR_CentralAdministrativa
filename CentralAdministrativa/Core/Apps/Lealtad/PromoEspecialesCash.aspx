<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="PromoEspecialesCash.aspx.cs" Inherits="Lealtad.PromoEspecialesCash" %>


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
    <ext:Window ID="WindowNuevaPromocion" runat="server" Title="Nueva Promoción" Hidden="true" Width="450" Height="140"
        Modal="true" Resizable="false">
        <Items>
            <ext:FormPanel ID="FormPanelNuevaPromocion" runat="server" Padding="10" LabelAlign="Left" LabelWidth="150">
                <Items>
                    <ext:TextArea ID="txNuevaPromocion" runat="server" FieldLabel="Nombre de la Promoción" 
                        AllowBlank="false" MaxLength="100" Width="250" Height="55" />
                </Items>
            </ext:FormPanel>
        </Items>
        <FooterBar>
            <ext:Toolbar ID="ToolbarNuevaPromocion" runat="server">
                <Items>
                    <ext:Button ID="btnAceptaNuevaPromo" runat="server" Text="Aceptar" Icon="Accept">
                        <DirectEvents>
                            <Click OnEvent="btnAceptaNuevaPromo_Click" Before="var valid= #{FormPanelNuevaPromocion}.getForm().isValid(); if (!valid) {} return valid;" />
                        </DirectEvents>
                    </ext:Button>
                    <ext:Button ID="btnCancelaNuevaPromo" runat="server" Text="Cancelar" Icon="Cancel">
                        <DirectEvents>
                            <Click OnEvent="btnCancelaNuevaPromo_Click" />
                        </DirectEvents>
                    </ext:Button>
                </Items>
            </ext:Toolbar>
        </FooterBar>
    </ext:Window>
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
            <ext:Store ID="StorePromocion" runat="server">
                <Reader>
                    <ext:JsonReader IDProperty="ID_Evento">
                        <Fields>
                            <ext:RecordField Name="ID_Evento" />
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
                                Height="210">
                                <Items>
                                   <ext:Panel ID="Panel1" runat="server" Border="false" Width="300" Height="210">
                                        <Items>
                                           <ext:Image ID="Image1" runat="server" Width="300" Height="190" ImageUrl="Images/promocion2.jpg">
                                            </ext:Image>
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel ID="Panel2" runat="server" Border="false" Layout="RowLayout" AutoScroll="true" Width="600">
                                        <Items>
                                            <ext:Panel ID="Panel3" runat="server" Border="false" Header="false" LabelAlign="Left" Width="500"
                                                Layout="FormLayout">
                                                <Items>
                                                    <ext:Label runat="server" ID="lblNombreRegla" Cls="Titulo" FieldLabel="Nombre de Regla"
                                                        Text="Promociones Especiales">
                                                    </ext:Label>
                                                </Items>
                                            </ext:Panel>
                                            <ext:Panel ID="PanelDummy1" runat="server" Border="false" Header="false" LabelAlign="Left"
                                                Layout="FormLayout">
                                                <Items>
                                                    <ext:Label ID="Label1" runat="server" Text="">
                                                    </ext:Label>
                                                </Items>
                                            </ext:Panel>
                                            <ext:Panel ID="Panel4" runat="server" Border="false" Header="false" LabelAlign="Left" Layout="FormLayout">
                                                <Items>
                                                    <ext:Label runat="server" ID="lblDescripcionRegla" Cls="descripcion" FieldLabel="Descripción"
                                                        Text="Se pueden crear promociones especiales para bonificar puntos adicionales por periodos 
                                                        específicos de tiempo, de acuerdo a los valores configurados para la promoción.">
                                                    </ext:Label>
                                                </Items>
                                            </ext:Panel>
                                            <ext:Panel ID="PanelDummy2" runat="server" Border="false" Header="false" LabelAlign="Left"
                                                Layout="FormLayout">
                                                <Items>
                                                    <ext:Label ID="Label2" runat="server" Text="">
                                                    </ext:Label>
                                                </Items>
                                            </ext:Panel>
                                            <ext:Panel ID="Panel5" runat="server" Border="false" Header="false" LabelAlign="Left"
                                                Layout="FormLayout">
                                                <Items>
                                                    <ext:Hidden ID="hdnIdReglaPE" runat="server" />
                                                    <ext:Hidden ID="hdnIdCadena" runat="server" />
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
                                            <ext:Panel ID="PanelDummy3" runat="server" Border="false" Header="false" LabelAlign="Left"
                                                Layout="FormLayout">
                                                <Items>
                                                    <ext:Label ID="Label3" runat="server" Text="">
                                                    </ext:Label>
                                                </Items>
                                            </ext:Panel>
                                            <ext:Panel ID="Panel6" runat="server" Layout="HBoxLayout" LabelAlign="Left" BodyPadding="5" Border="false">
                                                <Defaults>
                                                    <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                </Defaults>
                                                <LayoutConfig>
                                                    <ext:HBoxLayoutConfig Align="Top" />
                                                </LayoutConfig>
                                                <Items>
                                                    <ext:ComboBox ID="cBoxPromocion" FieldLabel="Promoción" ForceSelection="true"
                                                        EmptyText="Selecciona una Promoción..." runat="server" Width="485" StoreID="StorePromocion"
                                                        MsgTarget="Side" AllowBlank="false" DisplayField="Descripcion" ValueField="ID_Evento"
                                                        Editable="false" TypeAhead="true" Mode="Local">
                                                        <DirectEvents>
                                                            <Select OnEvent="select_Promocion" Before="var valid= #{FormPanel1}.getForm().isValid(); if (!valid) {} return valid;" />
                                                        </DirectEvents>
                                                    </ext:ComboBox>
                                                    <ext:Hidden runat="server" Flex="1" />
                                                    <ext:Button ID="btnNuevaPromocion" runat="server" Text="Nueva" Icon="Add" Width="80">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnNuevaPromocion_Click" />
                                                        </DirectEvents>
                                                    </ext:Button>
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
                                    <ext:PropertyGrid ID="GridPropiedades" runat="server" Header="false">
                                        <Source>
                                            <ext:PropertyGridParameter Name="(Parámetros)" Value="Valores">
                                            </ext:PropertyGridParameter>
                                        </Source>
                                        <View>
                                            <ext:GridView ID="GridView1" ForceFit="true" ScrollOffset="2" runat="server" />
                                        </View>
                                        <FooterBar>
                                            <ext:Toolbar ID="Toolbar1" runat="server" EnableOverflow="true">
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
