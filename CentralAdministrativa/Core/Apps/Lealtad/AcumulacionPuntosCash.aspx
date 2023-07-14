<%@ Page Language="C#"  MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="AcumulacionPuntosCash.aspx.cs" Inherits="Lealtad.AcumulacionPuntosCash" %>

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
            <ext:Hidden ID="hdnIdCadenaCash" runat="server" />
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
            <ext:Panel ID="PanelGeneral" runat="server" Layout="FitLayout" Title="Configurar Reglas">
                <Items>
                    <ext:BorderLayout ID="InternalBorderLayout" runat="server">
                        <Center Split="true">
                            <ext:FormPanel ID="FormPanel1" runat="server" Border="false" Layout="ColumnLayout"
                                Height="210">
                                <Items>
                                    <ext:Panel ID="PanelColumnDummy" runat="server" Border="false" Width="80" Height="210">
                                        <Items>
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel runat="server" Border="false" Width="300" Height="210">
                                        <Items>
                                            <ext:Image ID="Image1" runat="server" AnchorHorizontal="90%" AnchorVertical="100%"
                                                ImageUrl="Images/coins.jpeg">
                                            </ext:Image>
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel runat="server" Border="false" Layout="RowLayout" AutoScroll="true">
                                        <Items>
                                            <ext:Panel runat="server" Border="false" Header="false" LabelAlign="Left" Width="500"
                                                Layout="FormLayout">
                                                <Items>
                                                    <ext:Label runat="server" ID="lblNombreRegla" Cls="Titulo" Text="Acumulación de Puntos">
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
                                            <ext:Panel runat="server" Border="false" Header="false" LabelAlign="Left" Layout="FormLayout">
                                                <Items>
                                                    <ext:Label runat="server" ID="lblDescripcionRegla" Cls="descripcion" 
                                                        Text="Cada compra será evaluada y un porcentaje de la compra se abonará en puntos a la cuenta del cliente.">
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
                                            <ext:Panel runat="server" Border="false" Header="false" LabelAlign="Left" Layout="FormLayout">
                                                <Items>
                                                    <ext:ComboBox ID="cBoxNivelLealtad" FieldLabel="Nivel" ForceSelection="true"
                                                        EmptyText="Selecciona un Nivel..." runat="server" Width="480" StoreID="StoreNivelLealtad"
                                                        MsgTarget="Side" AllowBlank="false" DisplayField="Descripcion" ValueField="ID_GrupoCuenta"
                                                        Editable="false" AnchorHorizontal="90%" Resizable="true" TypeAhead="true" Mode="Local">
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
                                            <ext:Panel runat="server" Border="false" Header="false" LabelAlign="Left" Layout="FormLayout">
                                                <Items>
                                                    <ext:ComboBox ID="cBoxReglas" FieldLabel="Reglas" ForceSelection="true" AllowBlank="false"
                                                        EmptyText="Selecciona un Regla..." runat="server" Width="480" MsgTarget="Side"
                                                        DisplayField="Nombre" ValueField="ID_Regla" Editable="false" AnchorHorizontal="90%">
                                                        <Store>
                                                            <ext:Store ID="StoreRegla" runat="server">
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="ID_Regla">
                                                                        <Fields>
                                                                            <ext:RecordField Name="ID_Regla" />
                                                                            <ext:RecordField Name="Clave" />
                                                                            <ext:RecordField Name="Nombre" />
                                                                        </Fields>
                                                                    </ext:JsonReader>
                                                                </Reader>
                                                            </ext:Store>
                                                        </Store>
                                                        <DirectEvents>
                                                            <Select OnEvent="selectRegla_Click" Before="var valid= #{FormPanel1}.getForm().isValid();
                                                                if (!valid) {} else { #{Panel7}.setTitle( 'Parámetros de la Regla ' + record.get('Nombre')); } return valid;">
                                                                <EventMask ShowMask="true" Msg="Buscando Parámetros de la Regla..." MinDelay="200" />
                                                            </Select>
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
