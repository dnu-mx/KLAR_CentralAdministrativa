<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ConsultaClientes.aspx.cs" Inherits="CentroContacto.ConsultaClientes" %>


<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var prepareToolbar = function (grid, toolbar, rowIndex, record) {
            if (!record.get("Activo")) { 
                toolbar.items.get(0).hide();
            }
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
    <ext:Window ID="WindowEstatusTarjetaRazon" runat="server" Title="Cambio de Estatus" Hidden="true"
        Resizable="false" Width="500" Height="220" Modal="true">
        <Items>
            <ext:FormPanel ID="FNuevoRango" runat="server" Padding="10" >
                <Items>
                    <ext:TextArea ID="txtRazones" runat="server" FieldLabel="Razón" BoxLabel="CheckBox" 
                        Height="130" AnchorHorizontal="100%" MaxLengthText="200" AllowBlank="false"/>
                </Items>
                <Buttons>
                    <ext:Button ID="btnGuardaRazonEstatusTarjeta" runat="server" Text="Guardar" Icon="Disk">
                        <DirectEvents>
                            <Click OnEvent="btnGuardaRazonEstatusTarjeta_Click" />
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>

    <ext:BorderLayout ID="MainBorderLayout" runat="server">
        <West Split="true">
            <ext:Panel runat="server" Width="350" Collapsible="true">
                <Content>
                    <ext:BorderLayout ID="LeftBorderLayout" runat="server">
                        <North Split="true">
                            <ext:FormPanel ID="FormPanelBusqueda" runat="server" Title="Consulta de Clientes " Height="330" Frame="true" LabelWidth="120" Collapsible="true">
                                <Content>
                                    <ext:Store ID="StoreCadenaComercial" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Colectiva">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Colectiva" />
                                                    <ext:RecordField Name="NombreORazonSocial" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                        <SortInfo Field="NombreORazonSocial" Direction="ASC" />
                                    </ext:Store>
                                </Content>
                                <Items>
                                    <ext:FieldSet ID="FieldSetBusqueda" runat="server" Title="Búsqueda">
                                        <Items>
                                            <ext:ComboBox ID="cBoxCadena" runat="server" LabelAlign="Right" FieldLabel="Cadena" StoreID="StoreCadenaComercial"
                                                DisplayField="NombreORazonSocial" ValueField="ID_Colectiva" Width="300" Mode="Local" AutoSelect="true" 
                                                Editable="true" ForceSelection="true" TypeAhead="true" MinChars="1" MatchFieldWidth="false" Name="colectivas"/>
                                            <ext:TextField ID="txtNombre" runat="server" LabelAlign="Right" FieldLabel="Nombre" MaxLength="30" Width="300" />
                                            <ext:TextField ID="txtApPaterno" runat="server" LabelAlign="Right" FieldLabel="Primer Apellido" MaxLength="30" Width="300" />
                                            <ext:TextField ID="txtApMaterno" runat="server" LabelAlign="Right" FieldLabel="Segundo Apellido" MaxLength="30" Width="300" />
                                            <ext:NumberField ID="nfIdCliente" runat="server" LabelAlign="Right" FieldLabel="IDCliente" MaxLength="20" Width="300"
                                                AllowDecimals="False" AllowNegative="False" />
                                            <ext:TextField ID="txtTarjeta" runat="server" LabelAlign="Right" FieldLabel="MDA/Tarjeta" MaxLength="16" Width="300" />
                                            <ext:TextField ID="txtCorreo" runat="server" LabelAlign="Right" FieldLabel="Correo Electrónico" MaxLength="30" Width="300" />
                                            <ext:DateField ID="dfFechaNac" runat="server" LabelAlign="Right" FieldLabel="Fecha de Nacimiento" Width="300"
                                                Format="dd/MM/yyyy" Name="FechaNacimiento" Vtype="daterange" Editable="false" />
                                        </Items>
                                        <Buttons>
                                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                                <DirectEvents>
                                                    <Click OnEvent="btnLimpiar_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnBuscar_Click" Before="var valid= #{FormPanelBusqueda}.getForm().isValid(); if (!valid) {} return valid;">
                                                        <EventMask ShowMask="true" Msg="Buscando Clientes..." MinDelay="500" />
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Buttons>
                                    </ext:FieldSet>
                                </Items>
                            </ext:FormPanel>
                        </North>
                        <Center Split="true">
                            <ext:FormPanel ID="FormPanelResultados" runat="server" Title="Resultados Clientes" Layout="FitLayout">
                                <Items>
                                    <ext:GridPanel ID="GridResultados" runat="server" AutoExpandColumn="NombreTarjetahabiente" Height="450" AutoDoLayout="true">
                                        <Store>
                                            <ext:Store ID="StoreClientes" runat="server" OnRefreshData="StoreClientes_Refresh">
                                                <Reader>
                                                    <ext:JsonReader IDProperty="ID_MA">
                                                        <Fields>
                                                            <ext:RecordField Name="ID_MA" />
                                                            <ext:RecordField Name="ID_Cadena" />
                                                            <ext:RecordField Name="Cadena" />
                                                            <ext:RecordField Name="ID_Cliente" />
                                                            <ext:RecordField Name="NombreTarjetahabiente" />
                                                            <ext:RecordField Name="MedioAcceso" />
                                                            <ext:RecordField Name="Email" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Store>
                                        <ColumnModel runat="server">
                                            <Columns>
                                                <ext:Column DataIndex="ID_MA" Hidden="true" />
                                                <ext:Column DataIndex="ID_Cadena" Hidden="true" />
                                                <ext:Column DataIndex="Cadena" Header="Cadena" Width="80" />
                                                <ext:Column DataIndex="ID_Cliente" Header="IdCliente" Width="60" />
                                                <ext:Column DataIndex="NombreTarjetahabiente" Header="Nombre" Width="100" />
                                                <ext:Column DataIndex="MedioAcceso" Header="MDA/Tarjeta" />
                                                <ext:Column DataIndex="Email" Header="Correo" />
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
                                            <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreClientes" DisplayInfo="true"
                                                DisplayMsg="Clientes {0} - {1} de {2}" />
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
            <ext:Panel runat="server" Height="250">
                <Items>
                    <ext:BorderLayout runat="server">
                        <Center>
                            <ext:TabPanel runat="server">
                                <Items>
                                    <ext:FormPanel ID="FormPanelDatos" runat="server" Title="Datos" LabelAlign="Left" LabelWidth="200" >
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
                                            <ext:FieldSet ID="FieldSetDatosCliente" runat="server" Title="Información del Cliente">
                                                <Items>
                                                    <ext:TextField ID="txtID_Cliente" runat="server" Hidden="true" Enabled="false" />
                                                    <ext:TextField ID="txtID_MA" runat="server" Hidden="true" Enabled="false" />
                                                    <ext:TextField ID="txtNombreCliente" runat="server" FieldLabel="Nombre" Width="500" />
                                                    <ext:TextField ID="txtApPaternoCliente" runat="server" FieldLabel="Primer Apellido" MaxLength="30" Width="500" />
                                                    <ext:TextField ID="txtApMaternoCliente" runat="server" FieldLabel="Segundo Apellido" MaxLength="30" Width="500" />
                                                    <ext:TextField ID="txtEmailCliente" runat="server" FieldLabel="Correo Electrónico" MaxLength="30" Width="500" />
                                                    <ext:NumberField ID="nfTelefonoCliente" runat="server" FieldLabel="Teléfono" MaxLength="10" Width="500" 
                                                        AllowDecimals="False" AllowNegative="False" />
                                                    <ext:DateField ID="dfFechaNacCliente" runat="server" FieldLabel="Fecha de Nacimiento" Format="dd/MM/yyyy" Width="500" 
                                                        Editable="false"  />                                                 
                                                    <ext:TextField ID="txtID_Direccion" runat="server" Hidden="true" Enabled="false" />
                                                    <ext:TextField ID="txtCPCliente" runat="server" FieldLabel="Código Postal" MinLength="5" MaxLength="5" Width="500" EnableKeyEvents="true">
                                                        <Listeners>
                                                            <SpecialKey Handler="if (e.getKey() == e.TAB || e.getKey() == e.ENTER) {
                                                                #{cBoxColonia}.reset(); #{txtColonia}.reset(); #{txtColonia}.hide();
                                                                ConsultaClientes.LlenaComboColonias();}" />
                                                        </Listeners>
                                                    </ext:TextField>
                                                    <ext:TextField ID="txtIDColonia" runat="server" Hidden="true" Enabled="false" />
                                                    <ext:ComboBox ID="cBoxColonia" runat="server" FieldLabel="Colonia" StoreID="StoreColonias" Width="500"
                                                        DisplayField="Colonia" ValueField="ID_Colonia">
                                                        <Listeners>
                                                            <Select Handler="var cp = #{txtCPCliente}.getValue() + '9999'; if (this.getValue() == cp) {#{txtColonia}.show();}" />
                                                        </Listeners>
                                                    </ext:ComboBox>
                                                    <ext:TextField ID="txtColonia" runat="server" FieldLabel="Nombre de la Colonia" Hidden="true" Enabled="false"
                                                        EmptyText="Por favor, especifique..." Width="500"/>
                                                    <ext:TextField ID="txtClaveMunicipio" runat="server" Hidden="true" Enabled="false" />
                                                    <ext:TextField ID="txtMunicipioCliente" runat="server" FieldLabel="Municipio" MaxLength="30" Width="500" />
                                                    <ext:TextField ID="txtClaveEstado" runat="server" Hidden="true" Enabled="false" />
                                                    <ext:TextField ID="txtEstadoCliente" runat="server" FieldLabel="Estado" MaxLength="30" Width="500" />
                                                </Items>
                                                <Buttons>
                                                    <ext:Button ID="btnGuardaDatos" runat="server" Text="Guardar" Icon="Disk">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnGuardaDatos_Click" Before="var valid= #{FormPanelDatos}.getForm().isValid(); if (!valid) {} return valid;" />
                                                        </DirectEvents>
                                                    </ext:Button>
                                                </Buttons>
                                            </ext:FieldSet>
                                        </Items>        
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelMovimientos" runat="server" Title="Movimientos">
                                        <Content>
                                            <ext:BorderLayout ID="BorderLayoutMovimientos" runat="server">
                                                <North Split="true">
                                                    <ext:FormPanel ID="FormPanelBuscarMov" runat="server" LabelWidth="200" Height="200">
                                                        <Content>
                                                            <ext:Store ID="StoreTipoOperacion" runat="server">
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="ID_Evento">
                                                                        <Fields>
                                                                            <ext:RecordField Name="ID_Evento" />
                                                                            <ext:RecordField Name="ClaveEvento" />
                                                                            <ext:RecordField Name="Descripcion" />
                                                                        </Fields>
                                                                    </ext:JsonReader>
                                                                </Reader>
                                                            </ext:Store>
                                                            <ext:Store ID="StoreTipoCuenta" runat="server">
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="ID_TipoCuenta">
                                                                        <Fields>
                                                                            <ext:RecordField Name="ID_TipoCuenta" />
                                                                            <ext:RecordField Name="Descripcion" />
                                                                        </Fields>
                                                                    </ext:JsonReader>
                                                                </Reader>
                                                            </ext:Store>
                                                        </Content>
                                                        <Items>
                                                            <ext:FieldSet ID="FieldSetBuscarMov" runat="server" Title="Movimientos del MDA/Tarjeta">
                                                                <Items>
                                                                    <ext:DateField ID="dfFechaInicialMov" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                                                        AllowBlank="false" Format="dd/MM/yyyy" MaxLength="12" EnableKeyEvents="true"
                                                                        Width="500" MaxWidth="500">
                                                                        <CustomConfig>
                                                                            <ext:ConfigItem Name="endDateField" Value="#{dfFechaFinalMov}" Mode="Value" />
                                                                        </CustomConfig>
                                                                        <Listeners>
                                                                            <KeyUp Fn="onKeyUp" />
                                                                        </Listeners>
                                                                    </ext:DateField>
                                                                    <ext:DateField ID="dfFechaFinalMov" runat="server" Vtype="daterange" FieldLabel="Fecha Final"
                                                                        AllowBlank="false" MaxLength="12" Format="dd/MM/yyyy" EnableKeyEvents="true"
                                                                        Width="500" MaxWidth="500">
                                                                        <CustomConfig>
                                                                            <ext:ConfigItem Name="startDateField" Value="#{dfFechaInicialMov}" Mode="Value" />
                                                                        </CustomConfig>
                                                                        <Listeners>
                                                                            <KeyUp Fn="onKeyUp" />
                                                                        </Listeners>
                                                                    </ext:DateField>
                                                                    <ext:ComboBox ID="cBoxTipoCuenta" runat="server" FieldLabel="Tipo de Cuenta" StoreID="StoreTipoCuenta" Width="500"
                                                                        DisplayField="Descripcion" ValueField="ID_TipoCuenta" />
                                                                    <ext:ComboBox ID="cBoxTipoOperacion" runat="server" FieldLabel="Tipo de Operación" StoreID="StoreTipoOperacion" Width="500"
                                                                        DisplayField="Descripcion" ValueField="ID_Evento" />
                                                                </Items>
                                                                <Buttons>
                                                                    <ext:Button ID="btnBuscarMov" runat="server" Text="Buscar" Icon="Magnifier">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="btnBuscarMov_Click" Before="var valid= #{FormPanelBuscarMov}.getForm().isValid(); if (!valid) {} return valid;" />
                                                                        </DirectEvents>
                                                                    </ext:Button>
                                                                </Buttons>
                                                            </ext:FieldSet>
                                                        </Items>
                                                    </ext:FormPanel>
                                                </North>
                                                <Center Split="true">
                                                    <ext:GridPanel ID="GridResultadosMov" runat="server" Header="true" Title="Nombre:">
                                                        <Store>
                                                            <ext:Store ID="StoreResultadosMov" runat="server" OnRefreshData="btnBuscarMov_Click">
                                                                <DirectEventConfig IsUpload="true" />
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="ID_Poliza">
                                                                        <Fields>
                                                                            <ext:RecordField Name="ID_Poliza" />
                                                                            <ext:RecordField Name="Fecha" />
                                                                            <ext:RecordField Name="Sucursal" />
                                                                            <ext:RecordField Name="Ticket" />
                                                                            <ext:RecordField Name="MontoTicket" />
                                                                            <ext:RecordField Name="TipoOperacion" />
                                                                            <ext:RecordField Name="MontoOperacion" />
                                                                            <ext:RecordField Name="SaldoInicial" />
                                                                            <ext:RecordField Name="SaldoFinal" />
                                                                            <ext:RecordField Name="SaldoActual" />
                                                                        </Fields>
                                                                    </ext:JsonReader>
                                                                </Reader>
                                                                <DirectEventConfig IsUpload="true" />
                                                            </ext:Store>
                                                        </Store>
                                                        <ColumnModel runat="server">
                                                            <Columns>
                                                                <%--<ext:Column DataIndex="ID_Poliza" Hidden="true" />
                                                                        <ext:Column DataIndex="ID_Operacion" Hidden="true" />
                                                                        <ext:Column DataIndex="ID_Cuenta" Hidden="true" />--%>
                                                                <ext:DateColumn DataIndex="Fecha" Header="Fecha" Align="Center" Format="dd-MMM-yyyy HH:mm:ss" Width="120" />
                                                                <ext:Column DataIndex="Sucursal" Header="Sucursal" Width="80" />
                                                                <ext:Column DataIndex="Ticket" Header="Ticket" />
                                                                <ext:Column DataIndex="MontoTicket" Header="Monto Ticket" Align="Right" />
                                                                <ext:Column DataIndex="TipoOperacion" Header="Tipo Operación" Align="Center" />
                                                                <ext:Column DataIndex="MontoOperacion" Header="Monto Operación" Align="Right" />
                                                                <ext:Column DataIndex="SaldoInicial" Header="Saldo Inicial" Align="Right" />
                                                                <ext:Column DataIndex="SaldoFinal" Header="Saldo Final" Align="Right" />
                                                                <%--<ext:Column DataIndex="SaldoActual" Hidden="true" />--%>
                                                            </Columns>
                                                        </ColumnModel>
                                                        <SelectionModel>
                                                            <ext:RowSelectionModel SingleSelect="true" />
                                                        </SelectionModel>
                                                        <BottomBar>
                                                            <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="StoreResultadosMov" DisplayInfo="true"
                                                                DisplayMsg="Movimientos {0} - {1} de {2}" />
                                                        </BottomBar>
                                                        <TopBar>
                                                            <ext:Toolbar ID="Toolbar5" runat="server">
                                                                <Items>
                                                                    <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                                    <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="true">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="ExportGridToExcel" IsUpload="true" Success="Ext.Net.DirectMethods.Download({IsUpload:true})">
                                                                                <ExtraParams>
                                                                                    <ext:Parameter Name="GridToExport" Value="Ext.encode(#{GridResultadosMov}.getRowsValues({selectedOnly : false}))" Mode="Raw" />
                                                                                </ExtraParams>
                                                                            </Click>
                                                                        </DirectEvents>
                                                                    </ext:Button>
                                                                </Items>
                                                            </ext:Toolbar>
                                                        </TopBar>
                                                    </ext:GridPanel>
                                                </Center>
                                            </ext:BorderLayout>
                                        </Content>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelTarjetas" runat="server" Title="Tarjetas">
                                        <Items>
                                            <%--<ext:Hidden ID="hidActivo" runat="server" />--%>
                                            <ext:Hidden ID="hidIdEstatus" runat="server" />
                                            <ext:FormPanel ID="FormPanelTarjetasName" runat="server" Title="Nombre:">
                                                <Items>
                                                    <ext:GridPanel ID="GridTarjetas" runat="server" Height="750">
                                                        <Store>
                                                            <ext:Store ID="StoreTarjetas" runat="server">
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="ID_MA">
                                                                        <Fields>
                                                                            <ext:RecordField Name="ID_MA" />
                                                                            <ext:RecordField Name="TipoMA" />
                                                                            <ext:RecordField Name="MedioAcceso" />
                                                                            <ext:RecordField Name="Activo" />
                                                                            <ext:RecordField Name="ID_EstatusMA" />
                                                                            <ext:RecordField Name="Estatus" />
                                                                            <ext:RecordField Name="FechaEstatus" />
                                                                            <ext:RecordField Name="Motivo" />
                                                                        </Fields>
                                                                    </ext:JsonReader>
                                                                </Reader>
                                                            </ext:Store>
                                                        </Store>
                                                        <DirectEvents>
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
                                                        </DirectEvents>
                                                        <ColumnModel ID="ColumnModel1" runat="server">
                                                            <Columns>
                                                                <ext:Column DataIndex="ID_MA" Hidden="true" />
                                                                <ext:Column DataIndex="TipoMA" Header="Tipo MA" />
                                                                <ext:Column DataIndex="MedioAcceso" Header="Medio de Acceso/Tarjeta" />
                                                                <ext:Column DataIndex="Activo" Hidden="true" />
                                                                <ext:Column DataIndex="ID_EstatusMA" Hidden="true" />
                                                                <ext:Column DataIndex="Estatus" Header="Estatus" />
                                                                <ext:DateColumn DataIndex="FechaEstatus" Header="Actualización" Align="Center" Format="dd-MMM-yyyy HH:mm:ss" Width="120"/>
                                                                <ext:Column DataIndex="Motivo" Header="Motivo" Width="150"/>
                                                                <ext:CommandColumn Header="Acción" Width="60">
                                                                    <PrepareToolbar Fn="prepareToolbar" />
                                                                    <Commands>
                                                                        <ext:GridCommand CommandName="Desactivar" ToolTip-Text="Cancelar"
                                                                            Icon="Decline">
                                                                        </ext:GridCommand>
                                                                    </Commands>
                                                                </ext:CommandColumn>
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
                                        </Items>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelAcumulacion" runat="server" Title="Acumulación" LabelAlign="Left" LabelWidth="200" Hidden="true">
                                        <Items>
                                            <ext:FormPanel ID="FormPanelDatosAcumulacion" runat="server" Title="Nombre">
                                                <Items>
                                                    <ext:FieldSet ID="FieldSetAcumulacion" runat="server" Title="Datos para Acumulación" Height="200">
                                                        <Items>
                                                            <ext:NumberField ID="nfSucursal" runat="server" FieldLabel="Sucursal" MaxLength="10" Width="500" 
                                                                AllowDecimals="False" AllowNegative="False" />
                                                            <ext:DateField ID="dfFechaTicket" runat="server" FieldLabel="Fecha Ticket" Width="500" Editable="false"
                                                                Format="dd/MM/yyyy" Name="FechaNacimiento" Vtype="daterange" />
                                                            <ext:NumberField ID="nfTicket" runat="server" FieldLabel="Ticket" MaxLength="20" Width="500" 
                                                                AllowDecimals="False" AllowNegative="False" />
                                                            <ext:NumberField ID="nfMontoTicket" runat="server" FieldLabel="Monto Ticket" MaxLength="20" Width="500" 
                                                                AllowDecimals="true" AllowNegative="false" />
                                                        </Items>
                                                        <Buttons>
                                                            <ext:Button ID="btnAcumular" runat="server" Text="Acumular" Icon="ControlAddBlue">
                                                                <%--<DirectEvents>
                                                                    <Click OnEvent="btnAcumular_Click" Before="var valid= #{FormPanelDatos}.getForm().isValid(); if (!valid) {} return valid;" />
                                                                </DirectEvents>--%>
                                                            </ext:Button>
                                                            <ext:Button ID="btnLimpiarAcum" runat="server" Text="Limpiar" Icon="ControlRemoveBlue">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnLimpiarAcum_Click" />
                                                                </DirectEvents>
                                                            </ext:Button>
                                                        </Buttons>
                                                    </ext:FieldSet>
                                                </Items>
                                            </ext:FormPanel>
                                        </Items>        
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelCapturarLlamada" runat="server" Title="Capturar Llamada" Hidden="true">
                                        <Content>
                                            <ext:Store ID="StoreMotivos" runat="server">
                                                <Reader>
                                                    <ext:JsonReader IDProperty="ID_Actividad">
                                                        <Fields>
                                                            <ext:RecordField Name="ID_Actividad" />
                                                            <ext:RecordField Name="Descripcion" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Content>
                                        <Items>
                                            <ext:FormPanel ID="FormPanelLlamada" runat="server" Title="Nombre:">
                                                <Items>
                                                    <ext:FieldSet ID="FieldSetCapturarLlamada" runat="server" Title="Captura" Height="450">
                                                        <Items>
                                                            <ext:ComboBox ID="cBoxMotivoLlamada" runat="server" FieldLabel="Motivo Llamada" StoreID="StoreMotivos" Width="500"
                                                                DisplayField="Descripcion" ValueField="ID_Actividad" AllowBlank="false"/>
                                                            <ext:TextArea ID="txtComentarios" runat="server" FieldLabel="Comentarios" BoxLabel="CheckBox" Width="500" 
                                                                Height="350" MaxLengthText="300" AllowBlank="false"/>
                                                        </Items>
                                                        <Buttons>
                                                            <ext:Button ID="btnGuardarLlamada" runat="server" Text="Guardar" Icon="Disk">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnGuardarLlamada_Click" Before="var valid= #{FormPanelLlamada}.getForm().isValid(); if (!valid) {} return valid;" />
                                                                </DirectEvents>
                                                            </ext:Button>
                                                        </Buttons>
                                                    </ext:FieldSet>
                                                </Items>
                                            </ext:FormPanel>
                                        </Items>        
                                    </ext:FormPanel>
                                </Items>
                            </ext:TabPanel>
                        </Center>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
