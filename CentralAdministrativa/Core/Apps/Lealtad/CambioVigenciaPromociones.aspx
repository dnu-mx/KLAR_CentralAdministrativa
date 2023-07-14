<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CambioVigenciaPromociones.aspx.cs" 
    Inherits="Lealtad.CambioVigenciaPromociones" %>

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
            <ext:Store ID="StoreDetallePromocion" runat="server">
                <Reader>
                    <ext:JsonReader IDProperty="ID_GrupoMA">
                        <Fields>
                            <ext:RecordField Name="ID_GrupoMA" />
                            <ext:RecordField Name="Descripcion" />
                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>
        </Content>
        <North Split="true">
            <ext:FormPanel ID="FormPanel1" runat="server" Border="false" Layout="ColumnLayout"
                Height="210">
                <Items>
                    <ext:Panel ID="Panel1" runat="server" Border="false" Width="300" Height="210">
                        <Items>
                            <ext:Image ID="Image1" runat="server" Width="300" Height="200" ImageUrl="Images/promociones.jpg">
                            </ext:Image>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="PanelColumnDummy" runat="server" Border="false" Width="80" Height="210">
                        <Items>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel2" runat="server" Border="false" Layout="RowLayout" AutoScroll="true">
                        <Items>
                            <ext:Panel ID="Panel3" runat="server" Border="false" Header="false" LabelAlign="Left" Width="500"
                                Layout="FormLayout">
                                <Items>
                                    <ext:Label runat="server" ID="lblNombreRegla" Cls="Titulo" Text="Selecciona la Promoción a la que deseas cambiar su Vigencia">
                                    </ext:Label>
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="PanelDummy1" runat="server" Border="false" Header="false" LabelAlign="Left"
                                Layout="FormLayout">
                                <Items>
                                    <ext:Label ID="Label5" runat="server" Text="">
                                    </ext:Label>
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="PanelDummy2" runat="server" Border="false" Header="false" LabelAlign="Left"
                                Layout="FormLayout">
                                <Items>
                                    <ext:Label ID="Label6" runat="server" Text="">
                                    </ext:Label>
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel4" runat="server" Border="false" Header="false" LabelAlign="Left"
                                Layout="FormLayout">
                                <Items>
                                    <ext:ComboBox ID="cBoxPromocion" FieldLabel="Promoción" ForceSelection="true"
                                        EmptyText="Selecciona una Promoción..." runat="server" Width="480" StoreID="StorePromocion"
                                        MsgTarget="Side" AllowBlank="false" DisplayField="Descripcion" ValueField="ID_Evento"
                                        Editable="false" AnchorHorizontal="90%" Resizable="true" TypeAhead="true" Mode="Local">
                                        <DirectEvents>
                                            <Select OnEvent="selectPromocion" />
                                        </DirectEvents>
                                    </ext:ComboBox>
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="PanelDummy3" runat="server" Border="false" Header="false" LabelAlign="Left"
                                Layout="FormLayout">
                                <Items>
                                    <ext:Label ID="Label1" runat="server" Text="">
                                    </ext:Label>
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="PanelDummy4" runat="server" Border="false" Header="false" LabelAlign="Left"
                                Layout="FormLayout">
                                <Items>
                                    <ext:Label ID="Label2" runat="server" Text="">
                                    </ext:Label>
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel5" runat="server" Border="false" Header="false" LabelAlign="Left"
                                Layout="FormLayout">
                                <Items>
                                    <ext:ComboBox ID="cBoxDetalle" FieldLabel="Detalle" ForceSelection="true"
                                        EmptyText="Selecciona el Detalle de la Promoción..." runat="server" Width="480" StoreID="StoreDetallePromocion"
                                        MsgTarget="Side" AllowBlank="false" DisplayField="Descripcion" ValueField="ID_GrupoMA"
                                        Editable="false" AnchorHorizontal="90%" Resizable="true" TypeAhead="true" Mode="Local">
                                        <DirectEvents>
                                            <Select OnEvent="selectDetalle" Before="var valid= #{FormPanel1}.getForm().isValid(); if (!valid) {} return valid;" />
                                        </DirectEvents>
                                    </ext:ComboBox>
                                </Items>
                            </ext:Panel>
                        </Items>
                    </ext:Panel>
                </Items>
            </ext:FormPanel>
        </North>
        <Center Split="true">
            <ext:GridPanel ID="GridVigencias" runat="server" Title="Vigencia de la Promoción" Layout="FitLayout">
                <LoadMask ShowMask="false" />
                <Store>
                    <ext:Store ID="StoreVigencias" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_FechaVigencia">
                                <Fields>
                                    <ext:RecordField Name="ID_FechaVigencia" />
                                    <ext:RecordField Name="Descripcion" />
                                    <ext:RecordField Name="Valor" />
                                    <ext:RecordField Name="EsFechaInicial" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <View>
                    <ext:GridView ID="GridView1" ForceFit="true" ScrollOffset="2" runat="server" />
                </View>
                <ColumnModel runat="server">
                    <Columns>
                        <ext:Column DataIndex="ID_FechaVigencia" Hidden="true" />
                        <ext:Column DataIndex="Descripcion" Header="Parámetro" />
                        <ext:DateColumn DataIndex="Valor" Header="Valor" Format="dd-MMM-yyyy">
                            <Editor>
                                <ext:DateField ID="dfValorVigencia"
                                    runat="server"
                                    AllowBlank="false" />
                            </Editor>
                        </ext:DateColumn>
                        <ext:Column DataIndex="EsFechaInicial" Hidden="true" />
                    </Columns>
                </ColumnModel>
                <Buttons>
                    <ext:Button ID="btnGuardar" runat="server" Text="Guardar" Icon="Add">
                        <DirectEvents>
                            <Click OnEvent="btnGuardar_Click">
                                <ExtraParams>
                                    <ext:Parameter Name="Values" Value="Ext.encode(#{GridVigencias}.getRowsValues())" Mode="Raw" />
                                </ExtraParams>
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                    <ext:Button ID="btnCancelar" runat="server" Text="Cancelar" Icon="Cancel">
                        <DirectEvents>
                            <Click OnEvent="btnCancelar_Click" />
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:GridPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
