<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ActualizaBeneficiosSams.aspx.cs" Inherits="Lealtad.ActualizaBeneficiosSams" %>

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
                                                    <Click OnEvent="btnCargarArchivo_Click" IsUpload="true" Before="#{GridDatosArchivo}.getStore().removeAll();"
                                                        After="#{FileUploadField1}.reset();">
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
                                                    <ext:JsonReader IDProperty="ID_Colectiva">
                                                        <Fields>
                                                            <ext:RecordField Name="CLAVECADENA" />
                                                            <ext:RecordField Name="CADENA" />
                                                            <ext:RecordField Name="GIRO" />
                                                            <ext:RecordField Name="CLAVEPROMO" />
                                                            <ext:RecordField Name="TITULOPROMOCION" />
                                                            <ext:RecordField Name="TIPODESCUENTO" />
                                                            <ext:RecordField Name="DESCRIPCIONBENEFICIO" />
                                                            <ext:RecordField Name="RESTRICCIONES" />
                                                            <ext:RecordField Name="FACEBOOK" />
                                                            <ext:RecordField Name="WEB" />
                                                            <ext:RecordField Name="ESHOTDEAL" />
                                                            <ext:RecordField Name="CARRUSELHOME" />
                                                            <ext:RecordField Name="PROMOHOME" />
                                                            <ext:RecordField Name="ORDEN" />
                                                            <ext:RecordField Name="ACTIVA" />
                                                            <ext:RecordField Name="FECHAINICIO(DD/MM/AAAA)" />
                                                            <ext:RecordField Name="FECHAFIN(DD/MM/AAAA)" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Store>
                                        <ColumnModel ID="ColumnModel12" runat="server">
                                            <Columns>
                                                <ext:Column ColumnID="CLAVECADENA" Header="Clave Cadena" Sortable="true" DataIndex="CLAVECADENA" />
                                                <ext:Column ColumnID="CADENA" Header="Cadena" Sortable="true" DataIndex="CADENA" />
                                                <ext:Column ColumnID="GIRO" Header="Giro" Sortable="true" DataIndex="GIRO" />
                                                <ext:Column ColumnID="CLAVEPROMO" Header="Clave Promoción" Sortable="true" DataIndex="CLAVEPROMO" />
                                                <ext:Column ColumnID="TITULOPROMOCION" Header="Título Promoción" Sortable="true" DataIndex="TITULOPROMOCION" />
                                                <ext:Column ColumnID="TIPODESCUENTO" Header="Tipo Descuento" Sortable="true" DataIndex="TIPODESCUENTO" />
                                                <ext:Column ColumnID="DESCRIPCIONBENEFICIO" Header="Descripción Beneficio" Sortable="true" DataIndex="DESCRIPCIONBENEFICIO" />
                                                <ext:Column ColumnID="RESTRICCIONES" Header="Restricciones" Sortable="true" DataIndex="RESTRICCIONES" />
                                                <ext:Column ColumnID="FACEBOOK" Header="Facebook" Sortable="true" DataIndex="FACEBOOK" />
                                                <ext:Column ColumnID="WEB" Header="Web" Sortable="true" DataIndex="WEB" />
                                                <ext:Column ColumnID="ESHOTDEAL" Header="EsHotDeal" Sortable="true" DataIndex="ESHOTDEAL" />
                                                <ext:Column ColumnID="CARRUSELHOME" Header="CarruselHome" Sortable="true" DataIndex="CARRUSELHOME" />
                                                <ext:Column ColumnID="PROMOHOME" Header="PromoHome" Sortable="true" DataIndex="PROMOHOME" />
                                                <ext:Column ColumnID="ORDEN" Header="Orden" Sortable="true" DataIndex="ORDEN" />
                                                <ext:Column ColumnID="ACTIVA" Header="Activa" Sortable="true" DataIndex="ACTIVA" />
                                                <ext:DateColumn ColumnID="FECHAINICIO(DD/MM/AAAA)" Header="Fecha Inicio" Sortable="true" DataIndex="FECHAINICIO(DD/MM/AAAA)"
                                                    Format="dd/MM/yyyy" />
                                                <ext:DateColumn ColumnID="FECHAFIN(DD/MM/AAAA)" Header="Fecha Fin" Sortable="true" DataIndex="FECHAFIN(DD/MM/AAAA)"
                                                    Format="dd/MM/yyyy" />
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
