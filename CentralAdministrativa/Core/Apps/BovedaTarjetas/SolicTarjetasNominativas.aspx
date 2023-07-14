<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="SolicTarjetasNominativas.aspx.cs" Inherits="BovedaTarjetas.SolicTarjetasNominativas" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center Split="true">
            <ext:Panel runat="server" Border="false">
                <Content>
                    <ext:BorderLayout ID="LeftBorderLayout" runat="server">
                        <North Split="true">
                            <ext:FormPanel ID="FormPanel1" runat="server" Height="30" Border="false">
                                <Items>
                                    <ext:Toolbar ID="ToolbarConsulta" runat="server" Layout="HBoxLayout" BodyPadding="5"
                                        Region="North">
                                        <Defaults>
                                            <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                        </Defaults>
                                        <LayoutConfig>
                                            <ext:HBoxLayoutConfig Align="Middle" />
                                        </LayoutConfig>
                                        <Items>
                                            <ext:FileUploadField ID="FileUploadField1" runat="server" ButtonText="Examinar..."
                                                Icon="Magnifier" Flex="3" MarginSpec="0" />
                                            <ext:Hidden ID="Hidden1" runat="server" Flex="1" />
                                            <ext:Button ID="btnCargarArchivo" runat="server" Text="Cargar Archivo"
                                                Icon="PageWhitePut" Flex="1">
                                                <DirectEvents>
                                                    <Click OnEvent="btnCargarArchivo_Click" IsUpload="true">
                                                        <EventMask ShowMask="true" Msg="Cargando Archivo..." MinDelay="500" />
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </Items>
                            </ext:FormPanel>
                        </North>
                        <Center Split="true" Collapsible="false">
                            <ext:FormPanel ID="FormPanelResultados" runat="server" Layout="FitLayout" Border="false">
                                <Content>
                                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                                        <Center Split="true">
                                            <ext:GridPanel ID="GridDatosArchivo" runat="server" StripeRows="true" Border="false"
                                                Layout="FitLayout" AutoExpandColumn="Producto">
                                                <Store>
                                                    <ext:Store runat="server">
                                                        <Reader>
                                                            <ext:JsonReader IDProperty="ID">
                                                                <Fields>
                                                                    <ext:RecordField Name="Emisor" />
                                                                    <ext:RecordField Name="Producto" />
                                                                    <ext:RecordField Name="TipoTarjeta" />
                                                                    <ext:RecordField Name="DisenyoTarjeta" />
                                                                    <ext:RecordField Name="Cantidad" />
                                                                </Fields>
                                                            </ext:JsonReader>
                                                        </Reader>
                                                    </ext:Store>
                                                </Store>
                                                <ColumnModel runat="server">
                                                    <Columns>
                                                        <ext:Column Header="Emisor" Sortable="true" DataIndex="Emisor" Width="100" />
                                                        <ext:Column Header="Producto" Sortable="true" DataIndex="Producto" />
                                                        <ext:Column Header="Tipo de Tarjeta" Sortable="true" DataIndex="TipoTarjeta" Width="150" />
                                                        <ext:Column Header="Diseño de Tarjeta" Sortable="true" DataIndex="DisenyoTarjeta" Width="220" />
                                                        <ext:Column Header="Cantidad de Tarjetas a Solicitar" Sortable="true" DataIndex="Cantidad" Width="200" />
                                                    </Columns>
                                                </ColumnModel>
                                            </ext:GridPanel>
                                        </Center>
                                        <South Split="true">
                                            <ext:FormPanel ID="FormPanelInfo" runat="server" Layout="TableLayout" Border="false" Height="130" Padding="10" FormGroup="true"
                                                Title="Información en Bóveda">
                                                <Items>
                                                    <ext:Panel runat="server" Border="false" Header="false" Width="320px" ColumnWidth=".5" Layout="Form" LabelWidth="175">
                                                        <Items>
                                                            <ext:Label runat="server" FieldLabel="Número de Tarjetas Disponibles" LabelAlign="Top"/>
                                                            <ext:Hidden runat="server" Flex="1" Width="25" />
                                                            <ext:Label ID="lblTarjDisp" runat="server" Text="-" LabelSeparator=" "
                                                                Width="100" StyleSpec="font-weight:bold;font-family:segoe ui;font-size: 14px;text-align:right;" />
                                                        </Items>
                                                    </ext:Panel>
                                                    <ext:Panel runat="server" Border="false" Header="false" Width="320px" ColumnWidth=".5" Layout="Form" LabelWidth="175">
                                                        <Items>
                                                            <ext:Label runat="server" FieldLabel="Número de Tarjetas Emitidas" />
                                                            <ext:Hidden runat="server" Flex="1" Width="25" />
                                                            <ext:Label ID="lblTarjEmit" runat="server" Text="-" LabelSeparator=" "
                                                                Width="100" StyleSpec="font-weight:bold;font-family:segoe ui;font-size: 14px;text-align:right;" />
                                                        </Items>
                                                    </ext:Panel>
                                                    <ext:Panel runat="server" Border="false" Header="false" Width="320px" ColumnWidth=".5" Layout="Form" LabelWidth="120">
                                                        <Items>
                                                            <ext:TextField ID="txtClaveSolic" runat="server" FieldLabel="Clave de la Solicitud" Width="140" MaxLength="10"
                                                                AllowBlank="false" />
                                                        </Items>
                                                    </ext:Panel>
                                                </Items>
                                                <Buttons>
                                                    <ext:Button ID="btnSolicitarNom" runat="server" Text="Solicitar Tarjetas" Icon="Tick">
                                                        <%--<DirectEvents>
                                                            <Click OnEvent="btnAplicarCambios_Click">
                                                                <EventMask ShowMask="true" Msg="Aplicando cambios..." MinDelay="500" />
                                                            </Click>
                                                        </DirectEvents>--%>
                                                    </ext:Button>
                                                </Buttons>
                                            </ext:FormPanel>
                                        </South>
                                    </ext:BorderLayout>
                                </Content>
                            </ext:FormPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
