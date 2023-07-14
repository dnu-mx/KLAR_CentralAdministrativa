<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ActualizaSucursalesSams.aspx.cs"
    Inherits="Lealtad.ActualizaSucursalesSams" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content runat="server" ContentPlaceHolderID="MainContent">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center Split="true">
            <ext:Panel ID="Panel1" runat="server" Width="350">
                <Content>
                    <ext:BorderLayout ID="LeftBorderLayout" runat="server">
                        <North Split="true">
                            <ext:FormPanel ID="FormPanel1" runat="server" Height="30">
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
                            <ext:FormPanel ID="FormPanelResultados" runat="server" Layout="FitLayout">
                                <Items>
                                    <ext:GridPanel ID="GridDatosArchivo" runat="server" StripeRows="true"
                                        Layout="fit" Region="Center">
                                        <Store>
                                            <ext:Store runat="server">
                                                <Reader>
                                                    <ext:JsonReader IDProperty="ID">
                                                        <Fields>
                                                            <ext:RecordField Name="ClaveCadena" />
                                                            <ext:RecordField Name="ClaveSucursal" />
                                                            <ext:RecordField Name="NombreSucursal" />
                                                            <ext:RecordField Name="Direccion" />
                                                            <ext:RecordField Name="Colonia" />
                                                            <ext:RecordField Name="Ciudad" />
                                                            <ext:RecordField Name="CP" />
                                                            <ext:RecordField Name="Estado" />
                                                            <ext:RecordField Name="Telefono" />
                                                            <ext:RecordField Name="Latitud" />
                                                            <ext:RecordField Name="Longitud" />
                                                            <ext:RecordField Name="Activa" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Store>
                                        <ColumnModel ID="ColumnModel12" runat="server">
                                            <Columns>
                                                <ext:Column ColumnID="ClaveCadena" Header="Clave Cadena" Sortable="true" DataIndex="ClaveCadena" Width="80"/>
                                                <ext:Column ColumnID="ClaveSucursal" Header="Clave Sucursal" Sortable="true" DataIndex="ClaveSucursal" Width="90"/>
                                                <ext:Column ColumnID="NombreSucursal" Header="Nombre Sucursal" Sortable="true" DataIndex="NombreSucursal" />
                                                <ext:Column ColumnID="Direccion" Header="Dirección" Sortable="true" DataIndex="Direccion" Width="150"/>
                                                <ext:Column ColumnID="Colonia" Header="Colonia" Sortable="true" DataIndex="Colonia" Width="120"/>
                                                <ext:Column ColumnID="Ciudad" Header="Ciudad" Sortable="true" DataIndex="Ciudad" />
                                                <ext:Column ColumnID="CP" Header="Código Postal" Sortable="true" DataIndex="CP" Width="80"/>
                                                <ext:Column ColumnID="Estado" Header="Estado" Sortable="true" DataIndex="Estado" Width="60"/>
                                                <ext:Column ColumnID="Telefono" Header="Teléfono" Sortable="true" DataIndex="Telefono" />
                                                <ext:Column ColumnID="Latitud" Header="Latitud" Sortable="true" DataIndex="Latitud" Width="80" />
                                                <ext:Column ColumnID="Longitud" Header="Longitud" Sortable="true" DataIndex="Longitud" Width="80" />
                                                <ext:Column ColumnID="Activa" Header="Activa" Sortable="true" DataIndex="Activa" Width="50" />
                                            </Columns>
                                        </ColumnModel>
                                        <BottomBar>
                                            <ext:Toolbar ID="Toolbar1" runat="server">
                                                <Items>
                                                    <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                    <ext:Button ID="btnAplicarCambios" runat="server" Text="Aplicar Cambios" Icon="Tick">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnAplicarCambios_Click">
                                                                <EventMask ShowMask="true" Msg="Aplicando cambios..." MinDelay="500" />
                                                            </Click>
                                                        </DirectEvents>
                                                    </ext:Button>
                                                </Items>
                                            </ext:Toolbar>
                                        </BottomBar>

                                    </ext:GridPanel>
                                </Items>
                            </ext:FormPanel>



                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>

</asp:Content>
