<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ActualizacionDocumentos.aspx.cs" Inherits="TpvWeb.ActualizacionDocumentos" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="MainBorderLayout" runat="server">
        <West Split="true">
            <ext:Panel ID="Panel1" runat="server" Width="350" Collapsible="true">
                <Content>
                    <ext:BorderLayout ID="LeftBorderLayout" runat="server">
                        <North Split="true">
                            <ext:FormPanel ID="FormPanelBusqueda" runat="server" Title="Tienda DICONSA" Height="180" Layout="FitLayout" Frame="true" LabelWidth="120" Collapsible="true">
                                <Items>
                                    <ext:FieldSet ID="FieldSetBusqueda" runat="server" Title="Búsqueda">
                                        <Items>
                                            <ext:TextField ID="txtBusq_ClaveAlmacen" runat="server" LabelAlign="Right" FieldLabel="Clave Almacén" MaxLength="10" Width="300" />
                                            <ext:TextField ID="txtBusq_ClaveTienda" runat="server" LabelAlign="Right" FieldLabel="Clave Tienda" MaxLength="10" Width="300" />
                                        </Items>
                                        <Buttons>
                                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar">
                                                <DirectEvents>
                                                    <Click OnEvent="btnLimpiar_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnBuscar_Click" Before="var valid= #{FormPanelBusqueda}.getForm().isValid(); if (!valid) {} return valid;">
                                                        <EventMask ShowMask="true" Msg="Buscando Tiendas..." MinDelay="500" />
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Buttons>
                                    </ext:FieldSet>
                                </Items>
                            </ext:FormPanel>
                        </North>
                        <Center Split="true">
                            <ext:FormPanel ID="FormPanelResultados" runat="server" Title="Resultados Tiendas" Layout="FitLayout">
                                <Items>
                                    <ext:GridPanel ID="GridResultados" runat="server" AutoExpandColumn="NombreTienda" Height="550" AutoDoLayout="true">
                                        <Store>
                                            <ext:Store ID="StoreTiendas" runat="server" OnRefreshData="StoreTiendas_Refresh">
                                                <Reader>
                                                    <ext:JsonReader IDProperty="ID_Colectiva">
                                                        <Fields>
                                                            <ext:RecordField Name="ID_Colectiva" />
                                                            <ext:RecordField Name="Movil" />
                                                            <ext:RecordField Name="NombreTienda" />
                                                            <ext:RecordField Name="ClaveTienda" />
                                                            <ext:RecordField Name="ClaveAlmacen" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Store>
                                        <ColumnModel ID="ColumnModel1" runat="server">
                                            <Columns>
                                                <ext:Column DataIndex="ID_Colectiva" Hidden="true" />
                                                <ext:Column DataIndex="Movil" Header="Móvil" Width="100" />
                                                <ext:Column DataIndex="NombreTienda" Header="Tienda" Width="300" />
                                                <ext:Column DataIndex="ClaveTienda" Hidden="true" />
                                                <ext:Column DataIndex="ClaveAlmacen" Hidden="true" />
                                            </Columns>
                                        </ColumnModel>
                                        <SelectionModel>
                                            <ext:RowSelectionModel SingleSelect="true" />
                                        </SelectionModel>
                                        <DirectEvents>
                                            <RowClick OnEvent="selectRowResultados_Event">
                                                <ExtraParams>
                                                    <ext:Parameter Name="Values" Value="Ext.encode(#{GridResultados}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                                </ExtraParams>
                                            </RowClick>
                                        </DirectEvents>
                                        <BottomBar>
                                            <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreTiendas" DisplayInfo="true"
                                                DisplayMsg="Tiendas {0} - {1} de {2}" />
                                        </BottomBar>
                                    </ext:GridPanel>
                                </Items>
                            </ext:FormPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>       
        </West>
        <Center Split="true">
            <ext:Panel ID="Panel2" runat="server" Height="250">
                <Items>
                    <ext:BorderLayout ID="BorderLayout1" runat="server">
                        <Center>
                            <ext:FormPanel ID="FormPanelDocumentos" runat="server" Title="Documentos" LabelAlign="Right" 
                                LabelWidth="200" AutoScroll="true">
                                <Items>
                                    <ext:FieldSet ID="FieldSet1" runat="server" Title="Ingresa la Documentación">
                                        <Items>
                                            <ext:TextField ID="txtID_Colectiva" runat="server" Hidden="true" Enabled="false" />
                                            <ext:TextField ID="txtTarjeta" runat="server" FieldLabel="Número de Tarjeta" MaxLength="16"
                                                AllowBlank="false" Width="500" ReadOnly="true" Enabled="false" />
                                            <ext:TextField ID="txtEmail" runat="server" FieldLabel="Correo Electrónico" MaxLength="150"
                                                AllowBlank="false" Width="500" ReadOnly="true" Enabled="false" />
                                            <ext:TextField ID="txtURL" runat="server" FieldLabel="URL del Documento" MaxLength="180"
                                                AllowBlank="false" Width="500"/>
                                            <ext:ComboBox ID="cmbTipoDocumento" TabIndex="1" FieldLabel="Tipo de Documento" ListWidth="250"
                                                Width="500" runat="server" Resizable="true" AllowBlank="false">
                                                <Items>
                                                    <ext:ListItem Text="IFE (Frente)" Value="ife_front" />
                                                    <ext:ListItem Text="IFE (Reverso)" Value="ife_back" />
                                                    <ext:ListItem Text="Comprobante de Domicilio" Value="proof" />
                                                    <ext:ListItem Text="Firma" Value="signature" />
                                                </Items>
                                            </ext:ComboBox>
                                        </Items>
                                        <Buttons>
                                            <ext:Button ID="btnAceptar" runat="server" Text="Actualizar" Icon="Tick">
                                                <DirectEvents>
                                                    <Click OnEvent="btnAceptar_Click" Before="var valid= #{FormPanelDocumentos}.getForm().isValid(); if (!valid) {} return valid;" />
                                                </DirectEvents>
                                            </ext:Button>
                                        </Buttons>
                                    </ext:FieldSet>
                                </Items>
                            </ext:FormPanel>
                        </Center>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
