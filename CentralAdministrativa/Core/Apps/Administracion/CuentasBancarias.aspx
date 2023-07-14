<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="CuentasBancarias.aspx.cs" Inherits="Administracion.CuentasBancarias" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .chng-red-nf-css
        {
            color:red;
            font-weight:bold;
        }
    </style>
    <style type="text/css">
        .chng-blue-nf-css
        {
            color:blue;
            font-weight:bold;
        }
    </style>

    <script type="text/javascript">
        var prepareToolbar = function (grid, toolbar, rowIndex, record) {
            if (!record.get("Activo")) {
                toolbar.items.get(0).hide();
            }
        };

        var submitValue = function (grid, hiddenFormat, format) {
            hiddenFormat.setValue(format);
            grid.submitData(false);
        };

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
            <ext:Panel ID="MainPanel" runat="server" Width="350">
                <Content>
                    <ext:BorderLayout ID="LeftBorderLayout" runat="server">
                        <North Split="true">
                            <ext:FormPanel ID="FormPanelBusqueda" runat="server" Title="Consulta de Cuentas" Height="240" Frame="true" LabelWidth="160
                                " Collapsible="true">
                                <Items>
                                    <ext:FieldSet ID="FieldSetBusqueda" runat="server" Border="false">
                                        <Items>
                                            <ext:TextField ID="txtNumCuenta" runat="server" LabelAlign="Right" FieldLabel="Número de Cuenta" MaxLength="20" Width="300" />
                                            <ext:TextField ID="txtNumTarjeta" runat="server" LabelAlign="Right" FieldLabel="Número de Tarjeta" MaxLength="16" Width="300" />
                                            <ext:TextField ID="txtNumTarjetaAdicional" runat="server" LabelAlign="Right" FieldLabel="Número de Tarjeta Adicional" MaxLength="16" Width="300" />
                                            <ext:TextField ID="txtNombre" runat="server" LabelAlign="Right" FieldLabel="Nombre" MaxLength="30" Width="300" />
                                            <ext:TextField ID="txtApPaterno" runat="server" LabelAlign="Right" FieldLabel="Primer Apellido" MaxLength="30" Width="300" />
                                            <ext:TextField ID="txtApMaterno" runat="server" LabelAlign="Right" FieldLabel="Segundo Apellido" MaxLength="30" Width="300" />
                                        </Items>
                                        <Buttons>
                                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar">
                                                <DirectEvents>
                                                    <Click OnEvent="btnLimpiar_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnBuscar_Click" Before="var valid= #{FormPanelBusqueda}.getForm().isValid(); if (!valid) {} return valid;" />
                                                </DirectEvents>
                                            </ext:Button>
                                        </Buttons>
                                    </ext:FieldSet>
                                </Items>
                            </ext:FormPanel>
                        </North>
                        <Center Split="true">
                            <ext:FormPanel ID="FormPanelResultados" runat="server" Title="Resultados Cuentas" Layout="FitLayout">
                                <Items>
                                    <ext:GridPanel ID="GridResultados" runat="server" Height="450" AutoDoLayout="true">
                                        <Store>
                                            <ext:Store ID="StoreCuentas" runat="server" OnRefreshData="StoreCuentas_Refresh">
                                                <Reader>
                                                    <ext:JsonReader IDProperty="ID_CuentaCLDC">
                                                        <Fields>
                                                            <ext:RecordField Name="ID_CuentaCLDC" />
                                                            <ext:RecordField Name="ID_CuentaCCLC" />
                                                            <ext:RecordField Name="ID_MA" />
                                                            <ext:RecordField Name="NumTarjeta" />
                                                            <ext:RecordField Name="ID_Cadena" />
                                                            <ext:RecordField Name="ID_Colectiva" />
                                                            <ext:RecordField Name="NombreTarjetahabiente" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Store>
                                        <ColumnModel ID="ColumnModel1" runat="server">
                                            <Columns>
                                                <ext:Column DataIndex="ID_CuentaCLDC" Header="Cuenta" Width="90" />
                                                <ext:Column DataIndex="ID_CuentaCCLC" Hidden="true" />
                                                <ext:Column DataIndex="ID_MA" Hidden="true" />
                                                <ext:Column DataIndex="NumTarjeta" Header="Tarjeta" Width="90" />
                                                <ext:Column DataIndex="ID_Cadena" Hidden="true" />
                                                <ext:Column DataIndex="ID_Colectiva" Hidden="true" />
                                                <ext:Column DataIndex="NombreTarjetahabiente" Header="Nombre" Width="160" />
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
                                            <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreCuentas" DisplayInfo="true"
                                                DisplayMsg="Mostrando Cuentas {0} - {1} de {2}" />
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
            <ext:Panel ID="EastPanel" runat="server">
                <Items>
                    <ext:BorderLayout ID="BorderLayout1" runat="server">
                        <Center>
                            <ext:TabPanel ID="TabPanel1" runat="server">
                                <Items>
                                    <ext:FormPanel ID="FormPanelTitular" runat="server" Title="Titular" LabelAlign="Left" LabelWidth="150" AutoScroll="true" >
                                        <Content>
                                            <ext:Store ID="StoreColonias" runat="server">
                                                <Reader>
                                                    <ext:JsonReader IDProperty="ID_Colonia">
                                                        <Fields>
                                                            <ext:RecordField Name="ID_Colonia" />
                                                            <ext:RecordField Name="Colonia" />
                                                            <ext:RecordField Name="Municipio" />
                                                            <ext:RecordField Name="Estado" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Content>
                                        <Items>
                                            <ext:FieldSet ID="FieldSetDatosTitular" runat="server" Border="false" >
                                                <Items>
                                                    <ext:TextField ID="txtID_Cuenta" runat="server" Hidden="true" Enabled="false" />
                                                    <ext:TextField ID="txtID_MA" runat="server" Hidden="true" Enabled="false" />
                                                    <ext:TextField ID="txtID_Colectiva" runat="server" Hidden="true" Enabled="false" />
                                                    <ext:TextField ID="txtTarjetaTitular" runat="server" FieldLabel="Número de Tarjeta" MaxLength="16" Width="535" 
                                                        ReadOnly="true" Selectable="false" />
                                                    <ext:Panel ID="PanelDatosPersonales" runat="server" Title="Datos Personales" AutoHeight="true" LabelAlign="Left"
                                                        FormGroup="true" Layout="FormLayout" Width="600px">
                                                        <Items>
                                                            <ext:TextField ID="txtNombreClienteTitular" runat="server" FieldLabel="Nombre" Width="380" AllowBlank="false" />
                                                            <ext:TextField ID="txtApPaternoTitular" runat="server" FieldLabel="Primer Apellido" MaxLength="30" Width="380" AllowBlank="false" />
                                                            <ext:TextField ID="txtApMaternoTitular" runat="server" FieldLabel="Segundo Apellido" MaxLength="30" Width="380" />
                                                            <ext:TextField ID="txtRFCTitular" runat="server" FieldLabel="RFC" MaxLength="13" Width="380" />
                                                        </Items>
                                                    </ext:Panel>
                                                    
                                                    <ext:Panel ID="PanelDomicilio" runat="server" Title="Domicilio" AutoHeight="true" LabelAlign="Left"
                                                        FormGroup="true" Layout="FormLayout" Width="600px" Collapsed="true">
                                                        <Items>
                                                            <ext:TextField ID="txtID_Direccion" runat="server" Hidden="true" Enabled="false" />
                                                            <ext:TextField ID="txtCalle" runat="server" FieldLabel="Calle" MaxLength="30" Width="380" />
                                                            <ext:TextField ID="txtNumExterior" runat="server" FieldLabel="Número Exterior"
                                                                MaxLength="6" Width="380" />
                                                            <ext:TextField ID="txtNumInterior" runat="server" FieldLabel="Número Interior" 
                                                                MaxLength="6" Width="380" />
                                                            <ext:TextField ID="txtEntreCalles" runat="server" FieldLabel="Entre las Calles" 
                                                                MaxLength="100" Width="380" />
                                                            <ext:TextField ID="txtReferencias" runat="server" FieldLabel="Referencias del Domicilio" 
                                                                MaxLength="100" Width="380" />
                                                            <ext:TextField ID="txtCPCliente" runat="server" FieldLabel="Código Postal"
                                                                MinLength="5" MaxLength="5" Width="380" EnableKeyEvents="true">
                                                                <Listeners>
                                                                    <SpecialKey Handler="if (e.getKey() == e.TAB || e.getKey() == e.ENTER) {
                                                                #{cBoxColonia}.reset(); CuentasBancarias.LlenaComboColonias();}" />
                                                                </Listeners>
                                                            </ext:TextField>
                                                            <ext:TextField ID="txtIDColonia" runat="server" Hidden="true" Enabled="false" />
                                                            <ext:ComboBox ID="cBoxColonia" runat="server" FieldLabel="Colonia" StoreID="StoreColonias" 
                                                                Width="380" DisplayField="Colonia" ValueField="ID_Colonia" />
                                                            <ext:TextField ID="txtClaveMunicipio" runat="server" Hidden="true" Enabled="false" />
                                                            <ext:TextField ID="txtMunicipioTitular" runat="server" FieldLabel="Delegación o Municipio"
                                                                MaxLength="30" Width="380" />
                                                            <ext:TextField ID="txtClaveEstado" runat="server" Hidden="true" Enabled="false" />
                                                            <ext:TextField ID="txtEstadoTitular" runat="server" FieldLabel="Estado" MaxLength="30" 
                                                                Width="380" />
                                                        </Items>
                                                    </ext:Panel>

                                                    <ext:Panel ID="PanelDatosContacto" runat="server" Title="Datos de Contacto" AutoHeight="true" LabelAlign="Left"
                                                        FormGroup="true" Layout="FormLayout" Width="600px" >
                                                        <Items>
                                                            <ext:NumberField ID="nfTelParticular" runat="server" FieldLabel="Teléfono Particular" MaxLength="10" Width="380"
                                                                AllowDecimals="False" AllowNegative="False" />
                                                            <ext:NumberField ID="nfTelCelular" runat="server" FieldLabel="Teléfono Celular" MaxLength="10" Width="380"
                                                                AllowDecimals="False" AllowNegative="False" />
                                                            <ext:NumberField ID="nfTelTrabajo" runat="server" FieldLabel="Teléfono Trabajo" MaxLength="10" Width="380"
                                                                AllowDecimals="False" AllowNegative="False" />
                                                            <ext:TextField ID="txtCorreo" runat="server" FieldLabel="Correo Electrónico" MaxLength="30" Width="380" />
                                                        </Items>
                                                    </ext:Panel>
                                                </Items>
                                                <Buttons>
                                                    <ext:Button ID="btnGuardaDatos" runat="server" Text="Guardar" Icon="Disk">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnGuardaDatos_Click" />
                                                        </DirectEvents>
                                                    </ext:Button>
                                                </Buttons>
                                            </ext:FieldSet>
                                        </Items>        
                                    </ext:FormPanel>
                                    <ext:Panel ID="PanelTarjetas" runat="server" Title="Tarjetas">
                                        <Content>
                                            <ext:BorderLayout ID="BorderLayout2" runat="server">
                                                <North Split="true">
                                                    <ext:FormPanel ID="FormPanelCuenta" runat="server" Title="Parámetros de la Cuenta" LabelAlign="Left" LabelWidth="200" Height="210">
                                                        <Items>
                                                            <ext:FieldSet ID="FieldSetCuenta" runat="server" Border="false">
                                                                <Items>
                                                                    <ext:NumberField ID="nfFechaCorte" runat="server" FieldLabel="Fecha de Corte" Width="500" />
                                                                    <ext:TextField ID="txtHDFechaCorte" runat="server" Hidden="true" />

                                                                    <ext:NumberField ID="nfFechaLimitePago" runat="server" FieldLabel="Fecha Límite de Pago" Width="500" />
                                                                    <ext:TextField ID="txtHDFechaLimitePago" runat="server" Hidden="true" />


                                                                    <ext:NumberField ID="nfLimiteCredito" runat="server" FieldLabel="Límite de Crédito" Width="500"
                                                                        AllowDecimals="true" AllowNegative="false" />
                                                                    <ext:TextField ID="txtHDLimiteCredito" runat="server" Hidden="true" />
                                                                    <ext:TextField ID="txtHDLimiteCreditoPendiente" runat="server" Hidden="true" />

                                                                    <ext:TextField ID="txtNombreEmbozadoTitular" runat="server" FieldLabel="Nombre Embozado" Width="500" />
                                                                    <ext:TextField ID="txtHDEmbozado" runat="server" Hidden="true" />

                                                                    <ext:DateField ID="dfVigenciaTarjeta" runat="server" FieldLabel="Vigencia de la Tarjeta" Width="500" 
                                                                        Format="dd/MM/yyyy" Vtype="daterange" />
                                                                    <ext:DateField ID="dfHDVigenciaTarjeta" runat="server" Hidden="true" Format="dd/MM/yyyy"/>
                                                                </Items>
                                                                <Buttons>
                                                                    <ext:Button runat="server" ID="btnValorActual" Text="Ver Valor Actual" Icon="ControlStartBlue"
                                                                        Hidden="true">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="btnValorActual_Click" />
                                                                        </DirectEvents>
                                                                    </ext:Button>
                                                                    <ext:Button runat="server" ID="btnValorPendiente" Text="Ver Valor por Autorizar" Icon="ControlEndBlue"
                                                                        Hidden="true">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="btnValorPendiente_Click" />
                                                                        </DirectEvents>
                                                                    </ext:Button>
                                                                    <ext:Button ID="btnGuardarCuenta" runat="server" Text="Guardar" Icon="Disk">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="btnGuardarCuenta_Click" Before="var valid= #{FormPanelCuenta}.getForm().isValid(); if (!valid) {} return valid;" />
                                                                        </DirectEvents>
                                                                    </ext:Button>
                                                                    <ext:Button ID="btnAutorizarCambios" runat="server" Text="Autorizar Cambio" Icon="Tick" Hidden="true">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="btnAutorizarCambios_Click">
                                                                                <Confirmation ConfirmRequest="true" Title="Confirmación" Message="¿Autorizas el Cambio al Límite de Crédito?" />
                                                                            </Click>
                                                                        </DirectEvents>
                                                                    </ext:Button>
                                                                </Buttons>
                                                            </ext:FieldSet>
                                                        </Items>        
                                                    </ext:FormPanel>
                                                </North>
                                                <Center Split="true">
                                                    <ext:FormPanel ID="FormPanelAdicionales" runat="server" Title="Tarjetas Adicionales">
                                                        <Items>
                                                            <ext:GridPanel ID="GridTarjetas" runat="server" Height="750">
                                                                <Store>
                                                                    <ext:Store ID="StoreTarjetas" runat="server">
                                                                        <Reader>
                                                                            <ext:JsonReader IDProperty="ID_MA">
                                                                                <Fields>
                                                                                    <ext:RecordField Name="ID_MA" />
                                                                                    <ext:RecordField Name="Tarjeta" />
                                                                                    <ext:RecordField Name="Nombre" />
                                                                                    <ext:RecordField Name="APaterno" />
                                                                                    <ext:RecordField Name="AMaterno" />
                                                                                    <ext:RecordField Name="NombreEmbozo" />
                                                                                    <ext:RecordField Name="Expiracion" />
                                                                                    <ext:RecordField Name="LimiteCredito" />
                                                                                </Fields>
                                                                            </ext:JsonReader>
                                                                        </Reader>
                                                                    </ext:Store>
                                                                </Store>
                                                               <%-- <DirectEvents>
                                                                    <Command OnEvent="EjecutarComando">
                                                                        <ExtraParams>
                                                                            <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                                                            <ext:Parameter Name="Activo" Value="Ext.encode(record.data.Activo)" Mode="Raw" />
                                                                            <ext:Parameter Name="IdEstatus" Value="Ext.encode(record.data.ID_EstatusMA)" Mode="Raw" />
                                                                        </ExtraParams>
                                                                    </Command>
                                                                    <RowClick>
                                                                        <ExtraParams>
                                                                            <ext:Parameter Name="Activo" Value="Ext.encode(#{GridTarjetas}.getRowsValues({selectedOnly:true})[0].Activo)" Mode="Raw" />
                                                                            <ext:Parameter Name="IdEstatus" Value="Ext.encode(#{GridTarjetas}.getRowsValues({selectedOnly:true})[0].ID_EstatusMA)" Mode="Raw" />
                                                                        </ExtraParams>
                                                                    </RowClick>
                                                                </DirectEvents>--%>
                                                                <ColumnModel ID="ColumnModel3" runat="server">
                                                                    <Columns>
                                                                        <ext:Column DataIndex="ID_MA" Hidden="true" />
                                                                        <ext:Column DataIndex="Tarjeta" Header="Tarjeta" Width="50"/>
                                                                        <ext:Column DataIndex="Nombre" Header="Nombre" Width="50"/>
                                                                        <ext:Column DataIndex="APaterno" Header="Apellido Paterno" Width="70"/>
                                                                        <ext:Column DataIndex="AMaterno" Header="Apellido Materno" Width="70"/>
                                                                        <ext:Column DataIndex="NombreEmbozo" Header="Nombre Embozado" Width="110"/>
                                                                        <ext:DateColumn DataIndex="Expiracion" Header="Vigencia de la tarjeta" Align="Center" Format="dd-MMM-yyyy" Width="110"/>
                                                                        <ext:Column DataIndex="LimiteCredito" Header="Límite de Crédito" />
                                                                    </Columns>
                                                                </ColumnModel>
                                                                <SelectionModel>
                                                                    <ext:RowSelectionModel SingleSelect="true" />
                                                                </SelectionModel>
                                                                <BottomBar>
                                                                    <ext:PagingToolbar ID="PagingToolBar3" runat="server" StoreID="StoreTarjetas" DisplayInfo="true"
                                                                        DisplayMsg="Tarjetas {0} - {1} de {2}" />
                                                                </BottomBar>
                                                            </ext:GridPanel>
                                                        </Items>
                                                    </ext:FormPanel>
                                                </Center>
                                            </ext:BorderLayout>
                                        </Content>
                                    </ext:Panel>
                                </Items>
                            </ext:TabPanel>
                        </Center>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
