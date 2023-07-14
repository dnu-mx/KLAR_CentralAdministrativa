<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ActualizacionDatos.aspx.cs" Inherits="TpvWeb.ActualizacionDatos" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var onKeyUp = function (field) {
            var v = this.processValue(this.getRawValue()),
                field;

            if (this.startDateField) {
                field = Ext.getCmp(this.startDateField);
                field.setMaxValue();
                this.dateRangeMax = null;
            } else if (this.endDateField) {
                field = Ext.getCmp(this.endDateField);
                field.setMinValue();
                this.dateRangeMin = null;
            }

            field.validate();
        };
    </script>
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
                            <ext:FormPanel ID="FormPanelDatos" runat="server" Title="Información de la Tienda" LabelAlign="Right" 
                                LabelWidth="200" AutoScroll="true">
                                <Items>
                                    <ext:TextField ID="txtID_Colectiva" runat="server" Hidden="true" Enabled="false" />
                                    <ext:Panel ID="PanelTienda" runat="server" Title="Tienda" AutoHeight="true" FormGroup="true" 
                                        Layout="FormLayout" Width="550px">
                                        <Items>
                                            <ext:TextField ID="txtClaveAlmacen" runat="server" FieldLabel="Clave del Almacén" MaxLength="30"
                                                AllowBlank="false" Width="300" ReadOnly="true" Selectable="false" />
                                            <ext:TextField ID="txtClaveTienda" runat="server" FieldLabel="Clave de la Tienda" MaxLength="30"
                                                AllowBlank="false" Width="300" ReadOnly="true" Selectable="false" />
                                            <ext:TextField ID="txtNombreTienda" runat="server" FieldLabel="Nombre Tienda"
                                                AllowBlank="false" Width="300" />
                                            <ext:TextField ID="txtCalle" runat="server" FieldLabel="Calle"
                                                AllowBlank="false" Width="300" />
                                            <ext:TextField ID="txtLocalidad" runat="server" FieldLabel="Localidad"
                                                AllowBlank="false" Width="300" />
                                            <ext:TextField ID="txtMunicipio" runat="server" FieldLabel="Municipio"
                                                AllowBlank="false" Width="300" />
                                            <ext:TextField ID="txtEstado" runat="server" FieldLabel="Estado"
                                                AllowBlank="false" Width="300" />
                                            <ext:TextField ID="txtCodigoPostal" runat="server" FieldLabel="Código Postal" MaxLength="5"
                                                AllowBlank="false" Width="300" />
                                            <ext:TextField ID="txtTelefono" runat="server" FieldLabel="Teléfono" MaxLength="10"
                                                AllowBlank="false" Width="300" />
                                            <ext:TextField ID="txtPassword" runat="server" FieldLabel="Contraseña" MaxLength="10"
                                                AllowBlank="false" Width="300" />
                                            <ext:TextField ID="txtEmail" runat="server" FieldLabel="Correo Electrónico" MaxLength="150"
                                                AllowBlank="false" Width="300" />
                                            <ext:TextField ID="txtTarjeta" runat="server" FieldLabel="Número de Tarjeta" MaxLength="16"
                                                AllowBlank="false" Width="300" />
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel ID="PanelOperador" runat="server" Title="Operador" AutoHeight="true" FormGroup="true" 
                                        Layout="FormLayout" Width="550px">
                                        <Items>
                                            <ext:TextField ID="txtID_Operador" runat="server" Hidden="true" Enabled="false" />
                                            <ext:TextField ID="txtNombreOperador" runat="server" FieldLabel="Nombre"
                                                AllowBlank="false" Width="300" />
                                            <ext:TextField ID="txtApPaternoOperador" runat="server" FieldLabel="Apellido Paterno" MaxLength="30"
                                                AllowBlank="false" Width="300" />
                                            <ext:TextField ID="txtApMaternoOperador" runat="server" FieldLabel="Apellido Materno" MaxLength="30"
                                                AllowBlank="false" Width="300" />
                                            <ext:DateField ID="dfFechaNacimiento" runat="server" FieldLabel="Fecha de Nacimiento" Format="dd-MM-yyyy"
                                                Name="FechaNacimiento" Vtype="daterange" Editable="false" AllowBlank="false" Width="300">
                                                <Listeners>
                                                    <KeyUp Fn="onKeyUp" />
                                                </Listeners>
                                            </ext:DateField>
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel ID="PanelURL" runat="server" Title="URLs" AutoHeight="true" FormGroup="true" 
                                        Layout="FormLayout" Width="550px">
                                        <Items>
                                            <ext:TextField ID="txtURL_IFE_Frente" runat="server" FieldLabel="URL IFE (frente)" Width="300" />
                                            <ext:TextField ID="txtURL_IFE_Reverso" runat="server" FieldLabel="URL IFE (reverso)" Width="300" />
                                            <ext:TextField ID="txtURL_Firma" runat="server" FieldLabel="URL Firma" Width="300" />
                                            <ext:TextField ID="txtURL_Domicilio" runat="server" FieldLabel="URL Comprobante de Domicilio"
                                                Width="300" />
                                            <ext:Hidden ID="AltaEnSrPago" runat="server" />
                                        </Items>
                                    </ext:Panel>
                                </Items>
                                <Buttons>
                                    <ext:Button ID="btnAceptar" runat="server" Text="Dar de Alta" Icon="Tick">
                                        <DirectEvents>
                                            <Click OnEvent="btnAceptar_Click" Before="var valid= #{FormPanelDatos}.getForm().isValid(); if (!valid) {} return valid;" />
                                        </DirectEvents>
                                    </ext:Button>
                                </Buttons>
                            </ext:FormPanel>
                        </Center>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
