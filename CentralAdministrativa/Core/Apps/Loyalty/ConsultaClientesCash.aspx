<%@ Page Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" 
    CodeBehind="ConsultaClientesCash.aspx.cs" Inherits="CentroContacto.ConsultaClientesCash" %>


<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var onKeyUpPed = function (field) {
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

        var onKeyUpMov = function (field) {
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
                            <ext:FormPanel ID="FormPanelBusqueda" runat="server" Title="Consulta de Clientes" Height="280" Frame="true" LabelWidth="120" Collapsible="true">
                                <Items>
                                    <ext:FieldSet ID="FieldSetBusqueda" runat="server" Title="Búsqueda">
                                        <Items>
                                            <ext:TextField ID="txtNombre" runat="server" LabelAlign="Right" FieldLabel="Nombre" MaxLength="30" Width="300" />
                                            <ext:TextField ID="txtApPaterno" runat="server" LabelAlign="Right" FieldLabel="Primer Apellido" MaxLength="30" Width="300" />
                                            <ext:TextField ID="txtApMaterno" runat="server" LabelAlign="Right" FieldLabel="Segundo Apellido" MaxLength="30" Width="300" />
                                            <ext:TextField ID="txtCorreo" runat="server" LabelAlign="Right" FieldLabel="Correo Electrónico" MaxLength="60" Width="300" />
                                            <ext:DateField ID="dfFechaNac" runat="server" LabelAlign="Right" FieldLabel="Fecha de Nacimiento" Width="300"
                                                Format="dd/MM/yyyy" Name="FechaNacimiento" Vtype="daterange" Editable="false" />
                                            <ext:NumberField ID="nfTelefono" runat="server" LabelAlign="Right" FieldLabel="Teléfono" MaxLength="10" Width="300"
                                                AllowDecimals="false" AllowNegative="false" />
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
                                    <ext:GridPanel ID="GridResultados" runat="server" AutoExpandColumn="Nombre" Height="600" AutoDoLayout="true">
                                        <Store>
                                            <ext:Store ID="StoreClientes" runat="server" OnRefreshData="StoreClientes_Refresh">
                                                <Reader>
                                                    <ext:JsonReader IDProperty="IdColectiva">
                                                        <Fields>
                                                            <ext:RecordField Name="IdColectiva" />
                                                            <ext:RecordField Name="IdTipoColectiva" />
                                                            <ext:RecordField Name="EstatusMA" />
                                                            <ext:RecordField Name="Email" />
                                                            <ext:RecordField Name="Telefono" />
                                                            <ext:RecordField Name="Nombre" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Store>
                                        <ColumnModel ID="ColumnModel1" runat="server">
                                            <Columns>
                                                <ext:Column DataIndex="IdColectiva" Header="IdCliente" Width="60" />
                                                <ext:Column DataIndex="IdTipoColectiva" Hidden="true" />
                                                <ext:Column DataIndex="EstatusMA" Hidden="true" />
                                                <ext:Column DataIndex="Email" Header="Correo" />
                                                <ext:Column DataIndex="Nombre" Header="Nombre" Width="100" />
                                            </Columns>
                                        </ColumnModel>
                                        <SelectionModel>
                                            <ext:RowSelectionModel SingleSelect="true" />
                                        </SelectionModel>
                                        <DirectEvents>
                                            <RowClick OnEvent="selectRowResultados_Event">
                                                <EventMask ShowMask="true" Msg="Obteniendo Información del Cliente..." MinDelay="500" />
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
            <ext:Panel ID="PanelCentral" runat="server" Height="250">
                <Items>
                    <ext:BorderLayout ID="BorderLayout1" runat="server">
                        <Center>
                            <ext:TabPanel ID="TabPanel1" runat="server">
                                <Items>
                                    <ext:FormPanel ID="FormPanelDatos" runat="server" Title="Datos" LabelAlign="Left" LabelWidth="200">
                                        <Items>
                                            <ext:FieldSet ID="FieldSetDatosCliente" runat="server" Title="Información del Cliente">
                                                <Items>
                                                    <ext:Hidden ID="hdID_Colectiva" runat="server" />
                                                    <ext:Hidden ID="hdID_TipoColectiva" runat="server" />
                                                    <ext:Hidden ID="hdTelefono" runat="server" />
                                                    <ext:Hidden ID="hdIdEstatusMA" runat="server" />
                                                    <ext:TextField ID="txtNombreCliente" runat="server" FieldLabel="Nombre   <span style='color:red;'>*   </span>" Width="500" MaxLength="30"
                                                        AllowBlank="false" BlankText="El Nombre es Obligatorio"/>
                                                    <ext:TextField ID="txtApPaternoCliente" runat="server" FieldLabel="Primer Apellido   <span style='color:red;'>*   </span>" MaxLength="30"
                                                        Width="500" AllowBlank="false" BlankText="El Primer Apellido es Obligatorio" />
                                                    <ext:TextField ID="txtApMaternoCliente" runat="server" FieldLabel="Segundo Apellido" MaxLength="30" Width="500" />
                                                    <ext:TextField ID="txtEmailCliente" runat="server" FieldLabel="Correo Electrónico" Width="500"
                                                        ReadOnly="true" Enabled="false" />
                                                    <ext:DateField ID="dfFechaNacCliente" runat="server" FieldLabel="Fecha de Nacimiento" Format="dd/MM/yyyy" Width="500" />
                                                    <ext:DateField ID="dfFechaAlta" runat="server" FieldLabel="Fecha de Alta" Format="dd/MM/yyyy"
                                                        Width="500" ReadOnly="true" Enabled="false" />
                                                    <ext:DateField ID="dfFechaConfirmacion" runat="server" FieldLabel="Fecha de Confirmación"
                                                        Format="dd/MM/yyyy" Width="500" ReadOnly="true" Enabled="false" />
                                                    <ext:TextField ID="txtNivelLealtad" runat="server" FieldLabel="Nivel de Lealtad" MaxLength="30"
                                                        Width="500" ReadOnly="true" Enabled="false" />
                                                    <ext:TextField ID="txtTelef" runat="server" FieldLabel="Teléfono" MaxLength="20"
                                                        Width="500" ReadOnly="true" Enabled="false" />
                                                    <ext:TextField ID="txtTarjeta" runat="server" FieldLabel="Número de Tarjeta" MaxLength="20"
                                                        Width="500" ReadOnly="true" Enabled="false" />
                                                    <ext:TextField ID="txtCP" runat="server" FieldLabel="Código Postal" MaxLength="6"
                                                        Width="500" MaskRe="[0-9]"/>
                                                    <ext:TextField ID="txtRFC" runat="server" FieldLabel="RFC" MaxLength="20" Width="500" />
                                                </Items>
                                                <Buttons>
                                                    <ext:Button ID="btnGuardaDatos" runat="server" Text="Guardar" Icon="Disk" Disabled="true">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnGuardaDatos_Click" Before="var valid= #{FormPanelDatos}.getForm().isValid(); if (!valid) {} return valid;" />
                                                        </DirectEvents>
                                                    </ext:Button>
                                                </Buttons>
                                            </ext:FieldSet>
                                        </Items>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelDirecciones" runat="server" Title="Direcciones" LabelAlign="Left" LabelWidth="200" Hidden="true">
                                        <Content>
                                            <ext:Store ID="StoreAliasDirecciones" runat="server">
                                                <Reader>
                                                    <ext:JsonReader IDProperty="IdDomicilio">
                                                        <Fields>
                                                            <ext:RecordField Name="IdDomicilio" />
                                                            <ext:RecordField Name="Alias" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                                <SortInfo Field="Alias" Direction="ASC" />
                                            </ext:Store>
                                        </Content>
                                        <Items>
                                            <ext:FieldSet ID="FieldSetDirecciones" runat="server">
                                                <Items>
                                                    <ext:ComboBox ID="cBoxAliasDireccion" runat="server" FieldLabel="Alias Dirección" Width="500"
                                                        ListWidth="350" StoreID="StoreAliasDirecciones" DisplayField="Alias" ValueField="IdDomicilio">
                                                        <DirectEvents>
                                                            <Select OnEvent="LlenaFieldSetDirecciones">
                                                            </Select>
                                                        </DirectEvents>
                                                    </ext:ComboBox>
                                                    <ext:TextField ID="txtCalle" runat="server" FieldLabel="Calle" ReadOnly="true" Enabled="false" Width="500" />
                                                    <ext:TextField ID="txtNumeroExterior" runat="server" FieldLabel="Número Exterior" ReadOnly="true" Enabled="false" Width="500" />
                                                    <ext:TextField ID="txtNumeroInterior" runat="server" FieldLabel="Número Interior" ReadOnly="true" Enabled="false" Width="500" />
                                                    <ext:TextField ID="txtCodigoPostal" runat="server" FieldLabel="Código Postal" ReadOnly="true" Enabled="false" Width="500" />
                                                    <ext:TextField ID="txtColonia" runat="server" FieldLabel="Colonia" ReadOnly="true" Enabled="false" Width="500" />
                                                    <ext:TextField ID="txtEntreCalles" runat="server" FieldLabel="Entre Calles" ReadOnly="true" Enabled="false" Width="500" />
                                                    <ext:TextField ID="txtReferencias" runat="server" FieldLabel="Referencias" ReadOnly="true" Enabled="false" Width="500" />
                                                    <ext:TextField ID="txtTelefono" runat="server" FieldLabel="Teléfono" ReadOnly="true" Enabled="false" Width="500" />
                                                    <ext:TextField ID="txtExtension" runat="server" FieldLabel="Extensión" ReadOnly="true" Enabled="false" Width="500" />
                                                </Items>
                                            </ext:FieldSet>
                                        </Items>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelPedidos" runat="server" Title="Pedidos" Hidden="true">
                                        <Content>
                                            <ext:BorderLayout ID="BorderLayoutPedidos" runat="server">
                                                <North Split="true">
                                                    <ext:FormPanel ID="FormPanelBuscarPedidos" runat="server" LabelWidth="200" Height="150">
                                                        <Items>
                                                            <ext:FieldSet ID="FieldSetBuscarPedidos" runat="server" Title="Pedidos">
                                                                <Items>
                                                                    <ext:DateField ID="dfFechaInicialPed" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                                                        AllowBlank="false" Format="dd-MM-yyyy" MaxLength="12" TabIndex="1" EnableKeyEvents="true"
                                                                        Width="500" MaxWidth="500">
                                                                        <CustomConfig>
                                                                            <ext:ConfigItem Name="endDateField" Value="#{dfFechaFinalPed}" Mode="Value" />
                                                                        </CustomConfig>
                                                                        <Listeners>
                                                                            <KeyUp Fn="onKeyUpPed" />
                                                                        </Listeners>
                                                                    </ext:DateField>
                                                                    <ext:DateField ID="dfFechaFinalPed" runat="server" Vtype="daterange" FieldLabel="Fecha Final"
                                                                        AllowBlank="false" MaxLength="12" Format="dd-MM-yyyy" TabIndex="2" EnableKeyEvents="true"
                                                                        Width="500" MaxWidth="500">
                                                                        <CustomConfig>
                                                                            <ext:ConfigItem Name="startDateField" Value="#{dfFechaInicialPed}" Mode="Value" />
                                                                        </CustomConfig>
                                                                        <Listeners>
                                                                            <KeyUp Fn="onKeyUpPed" />
                                                                        </Listeners>
                                                                    </ext:DateField>
                                                                </Items>
                                                                <Buttons>
                                                                    <ext:Button ID="btnBuscarPed" runat="server" Text="Buscar" Icon="Magnifier">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="btnBuscarPed_Click" Before="var valid= #{FormPanelBuscarPedidos}.getForm().isValid(); if (!valid) {} return valid;">
                                                                                <EventMask ShowMask="true" Msg="Buscando Pedidos..." MinDelay="500" />
                                                                            </Click>
                                                                        </DirectEvents>
                                                                    </ext:Button>
                                                                </Buttons>
                                                            </ext:FieldSet>
                                                        </Items>
                                                    </ext:FormPanel>
                                                </North>
                                                <Center Split="true">
                                                    <ext:GridPanel ID="GridResultadosPed" runat="server" Header="true">
                                                        <Store>
                                                            <ext:Store ID="StoreResultadosPed" runat="server" OnRefreshData="btnBuscarPed_Click">
                                                                <DirectEventConfig IsUpload="true" />
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="Id_pedido">
                                                                        <Fields>
                                                                            <ext:RecordField Name="Id_pedido" />
                                                                            <ext:RecordField Name="FechaPedido" />
                                                                            <ext:RecordField Name="TipoServicio" />
                                                                            <ext:RecordField Name="Sucursal" />
                                                                            <ext:RecordField Name="Ticket" />
                                                                            <ext:RecordField Name="Importe" />
                                                                            <ext:RecordField Name="NumProductos" />
                                                                            <ext:RecordField Name="AliasDomicilio" />
                                                                            <ext:RecordField Name="MontoTDC" />
                                                                            <ext:RecordField Name="MontoEfectivo" />
                                                                            <ext:RecordField Name="PagoPts" />
                                                                            <ext:RecordField Name="PtsGenerados" />
                                                                        </Fields>
                                                                    </ext:JsonReader>
                                                                </Reader>
                                                                <DirectEventConfig IsUpload="true" />
                                                            </ext:Store>
                                                        </Store>
                                                        <ColumnModel ID="ColumnModel2" runat="server">
                                                            <Columns>
                                                                <ext:DateColumn DataIndex="FechaPedido" Header="Fecha de Pedido" Align="Center" Format="dd-MMM-yyyy HH:mm:ss" Width="120" />
                                                                <ext:Column DataIndex="Id_pedido" Header="Id_pedido" Width="60" Align="Center" />
                                                                <ext:Column DataIndex="TipoServicio" Header="Tipo de Servicio" Width="120" />
                                                                <ext:Column DataIndex="Sucursal" Header="Sucursal" Width="180" />
                                                                <ext:Column DataIndex="Ticket" Header="Ticket" Width="60" />
                                                                <ext:Column DataIndex="Importe" Header="Importe" Align="Right" Width="70">
                                                                    <Renderer Format="UsMoney" />
                                                                </ext:Column>
                                                                <ext:Column DataIndex="NumProductos" Header="No. Productos" Align="Center" Width="80" />
                                                                <ext:Column DataIndex="AliasDomicilio" Header="Alias Dirección" />
                                                                <ext:Column DataIndex="MontoTDC" Header="TDC" Align="Right" Width="70">
                                                                    <Renderer Format="UsMoney" />
                                                                </ext:Column>
                                                                <ext:Column DataIndex="MontoEfectivo" Header="Efectivo" Align="Right" Width="70">
                                                                    <Renderer Format="UsMoney" />
                                                                </ext:Column>
                                                                <ext:Column DataIndex="PagoPts" Header="Pago Pts" Align="Right" />
                                                                <ext:Column DataIndex="PtsGenerados" Header="Pts Gen" Align="Right" />
                                                            </Columns>
                                                        </ColumnModel>
                                                        <SelectionModel>
                                                            <ext:RowSelectionModel SingleSelect="true" />
                                                        </SelectionModel>
                                                        <BottomBar>
                                                            <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="StoreResultadosPed" DisplayInfo="true"
                                                                DisplayMsg="Pedidos {0} - {1} de {2}" />
                                                        </BottomBar>
                                                        <TopBar>
                                                            <ext:Toolbar ID="Toolbar5" runat="server">
                                                                <Items>
                                                                    <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                                    <ext:Button ID="btnExcelPed" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="true">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="ExportGridToExcel" IsUpload="true" Success="Ext.Net.DirectMethods.Download({IsUpload:true})">
                                                                                <ExtraParams>
                                                                                    <ext:Parameter Name="GridToExport" Value="Ext.encode(#{GridResultadosPed}.getRowsValues({selectedOnly : false}))" Mode="Raw" />
                                                                                    <ext:Parameter Name="Reporte" Value="P" Mode="Value" />
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
                                    <ext:FormPanel ID="FormPanelMovLealtad" runat="server" Title="Movimientos Lealtad">
                                        <Content>
                                            <ext:BorderLayout ID="BorderLayoutMovLeal" runat="server">
                                                <North Split="true">
                                                    <ext:FormPanel ID="FormPanelBuscarMovLeal" runat="server" LabelWidth="200" Height="150">
                                                        <Items>
                                                            <ext:FieldSet ID="FieldSetMovLeal" runat="server" Title="Movimientos Lealtad">
                                                                <Items>
                                                                    <ext:DateField ID="dfFechaInicialMovLeal" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                                                        AllowBlank="false" Format="dd-MM-yyyy" MaxLength="12" TabIndex="1" EnableKeyEvents="true"
                                                                        Width="500" MaxWidth="500">
                                                                        <CustomConfig>
                                                                            <ext:ConfigItem Name="endDateField" Value="#{dfFechaFinalMovLeal}" Mode="Value" />
                                                                        </CustomConfig>
                                                                        <Listeners>
                                                                            <KeyUp Fn="onKeyUpMov" />
                                                                        </Listeners>
                                                                    </ext:DateField>
                                                                    <ext:DateField ID="dfFechaFinalMovLeal" runat="server" Vtype="daterange" FieldLabel="Fecha Final"
                                                                        AllowBlank="false" MaxLength="12" Format="dd-MM-yyyy" TabIndex="2" EnableKeyEvents="true"
                                                                        Width="500" MaxWidth="500">
                                                                        <CustomConfig>
                                                                            <ext:ConfigItem Name="startDateField" Value="#{dfFechaInicialMovLeal}" Mode="Value" />
                                                                        </CustomConfig>
                                                                        <Listeners>
                                                                            <KeyUp Fn="onKeyUpMov" />
                                                                        </Listeners>
                                                                    </ext:DateField>
                                                                </Items>
                                                                <Buttons>
                                                                    <ext:Button ID="btnBuscarMovLeal" runat="server" Text="Buscar" Icon="Magnifier">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="btnBuscarMovLeal_Click" Before="var valid= #{FormPanelBuscarMovLeal}.getForm().isValid(); if (!valid) {} return valid;">
                                                                                <EventMask ShowMask="true" Msg="Buscando Movimientos..." MinDelay="500" />
                                                                            </Click>
                                                                        </DirectEvents>
                                                                    </ext:Button>
                                                                </Buttons>
                                                            </ext:FieldSet>
                                                        </Items>
                                                    </ext:FormPanel>
                                                </North>
                                                <Center Split="true">
                                                    <ext:GridPanel ID="GridResultadosMovLeal" runat="server" Header="true" Title="Saldo: ">
                                                        <Store>
                                                            <ext:Store ID="StoreResultadosMovLeal" runat="server" OnRefreshData="btnBuscarMovLeal_Click">
                                                                <DirectEventConfig IsUpload="true" />
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="ID_Poliza">
                                                                        <Fields>
                                                                            <ext:RecordField Name="ID_Poliza" />
                                                                            <ext:RecordField Name="Fecha" />
                                                                            <ext:RecordField Name="TipoMovimiento" />
                                                                            <ext:RecordField Name="Sucursal" />
                                                                            <ext:RecordField Name="Ticket" />
                                                                            <ext:RecordField Name="MontoTicket" />
                                                                            <ext:RecordField Name="Cargo" />
                                                                            <ext:RecordField Name="Abono" />
                                                                            <ext:RecordField Name="MontoOperacion" />
                                                                            <ext:RecordField Name="SaldoInicial" />
                                                                            <ext:RecordField Name="SaldoFinal" />
                                                                        </Fields>
                                                                    </ext:JsonReader>
                                                                </Reader>
                                                                <DirectEventConfig IsUpload="true" />
                                                            </ext:Store>
                                                        </Store>
                                                        <ColumnModel ID="ColumnModel3" runat="server">
                                                            <Columns>
                                                                <ext:DateColumn DataIndex="Fecha" Header="Fecha" Format="dd-MMM-yyyy HH:mm:ss" Width="120" />
                                                                <ext:Column DataIndex="TipoMovimiento" Header="Tipo de Movimiento" Width="180" />
                                                                <ext:Column DataIndex="Sucursal" Header="Sucursal" Width="150" />
                                                                <ext:Column DataIndex="Ticket" Header="Ticket" Width="50" />
                                                                <ext:Column DataIndex="MontoTicket" Header="Importe" Align="Right" Width="65">
                                                                    <Renderer Format="UsMoney" />
                                                                </ext:Column>
                                                                <ext:Column DataIndex="MontoOperacion" Header="Puntos" Align="Right" Width="58" />
                                                            </Columns>
                                                        </ColumnModel>
                                                        <SelectionModel>
                                                            <ext:RowSelectionModel SingleSelect="true" />
                                                        </SelectionModel>
                                                        <BottomBar>
                                                            <ext:PagingToolbar ID="PagingToolBar3" runat="server" StoreID="StoreResultadosMovLeal" DisplayInfo="true"
                                                                DisplayMsg="Movimientos Lealtad{0} - {1} de {2}" />
                                                        </BottomBar>
                                                        <TopBar>
                                                            <ext:Toolbar ID="Toolbar1" runat="server">
                                                                <Items>
                                                                    <ext:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                                    <ext:Button ID="btnExcelMovLeal" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="true">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="ExportGridToExcel" IsUpload="true" Success="Ext.Net.DirectMethods.Download({IsUpload:true})">
                                                                                <ExtraParams>
                                                                                    <ext:Parameter Name="GridToExport" Value="Ext.encode(#{GridResultadosMovLeal}.getRowsValues({selectedOnly : false}))" Mode="Raw" />
                                                                                    <ext:Parameter Name="Reporte" Value="M" Mode="Value" />
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
                                    <ext:FormPanel ID="FormPanelAjustesManuales" runat="server" Title="Ajustes Manuales" LabelAlign="Left" LabelWidth="120">
                                        <Content>
                                            <ext:Store ID="StoreNivelesLealtad" runat="server">
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
                                        <Items>
                                            <ext:FieldSet ID="FieldSetAjustesManuales" runat="server" Title="Cambio de Nivel" Layout="AnchorLayout"
                                                DefaultAnchor="100%" Collapsible="true" Collapsed="true">
                                                <Items>
                                                    <ext:ComboBox ID="cBoxNivel" runat="server" FieldLabel="Seleccionar Nivel" Width="500"
                                                        ListWidth="350" StoreID="StoreNivelesLealtad" DisplayField="Descripcion"
                                                        ValueField="ID_GrupoCuenta" AllowBlank="false" />
                                                </Items>
                                                <Buttons>
                                                    <ext:Button ID="btnGuardarNivel" runat="server" Text="Guardar" Icon="Disk" Disabled="true">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnGuardarNivel_Click" Before="var valid= #{cBoxNivel}.isValid();
                                                                if (!valid) {} return valid;" />
                                                        </DirectEvents>
                                                    </ext:Button>
                                                </Buttons>
                                            </ext:FieldSet>
                                            <ext:FieldSet ID="FieldSetCarga" runat="server" Title="Carga de Puntos" Layout="AnchorLayout"
                                                DefaultAnchor="100%" Collapsible="true" Collapsed="true">
                                                <Items>
                                                    <ext:NumberField ID="nfImporteCarga" runat="server" FieldLabel="Importe" MaxLength="20" Width="500"
                                                        AllowDecimals="true" AllowNegative="false" AllowBlank="false" />
                                                    <ext:TextArea ID="txtObsCargaPts" runat="server" FieldLabel="Observaciones" MaxLength="500"
                                                        MaxLengthText="500" AllowBlank="false" />
                                                </Items>
                                                <Buttons>
                                                    <ext:Button ID="btnAplicaCarga" runat="server" Text="Aplicar" Icon="ControlRemoveBlue">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnAplicaCarga_Click" Before="var valid1= #{nfImporteCarga}.isValid(); 
                                                                 var valid2= #{txtObsCargaPts}.isValid(); if (!valid1 || !valid2) {} return valid1;" />
                                                        </DirectEvents>
                                                    </ext:Button>
                                                </Buttons>
                                            </ext:FieldSet>
                                            <ext:FieldSet ID="FieldSetAbono" runat="server" Title="Abono de Puntos" Layout="AnchorLayout"
                                                DefaultAnchor="100%" Collapsible="true" Collapsed="true">
                                                <Items>
                                                    <ext:NumberField ID="nfImporteAbono" runat="server" FieldLabel="Importe" MaxLength="20" Width="500"
                                                        AllowDecimals="true" AllowNegative="false" AllowBlank="false" />
                                                    <ext:TextArea ID="txtObsAbonoPts" runat="server" FieldLabel="Observaciones" MaxLength="500"
                                                        MaxLengthText="500" AllowBlank="false" />
                                                </Items>
                                                <Buttons>
                                                    <ext:Button ID="btnAplicaAbono" runat="server" Text="Aplicar" Icon="ControlAddBlue">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnAplicaAbono_Click" Before="var valid1= #{nfImporteAbono}.isValid(); 
                                                                 var valid2= #{txtObsAbonoPts}.isValid(); if (!valid1 || !valid2) {} return valid1;" />
                                                        </DirectEvents>
                                                    </ext:Button>
                                                </Buttons>
                                            </ext:FieldSet>
                                            <ext:FieldSet ID="FieldSetEstatus" runat="server" Title="Cambio de Estatus" Layout="AnchorLayout"
                                                DefaultAnchor="100%" Collapsible="true">
                                                <Items>
                                                    <ext:Hidden ID="hdnPageLoad" runat="server" Text="1" />
                                                    <ext:RadioGroup ID="RadioGroupEstatus" runat="server" GroupName="RadioGroupEstatus" FieldLabel="Seleccione Estatus"
                                                        Cls="x-check-group-alt">
                                                        <Items>
                                                            <ext:Radio ID="rdActivo" runat="server" BoxLabel="Activo" />
                                                            <ext:Radio ID="rdInactivo" runat="server" BoxLabel="Inactivo" />
                                                        </Items>
                                                    </ext:RadioGroup>
                                                    <ext:TextArea ID="txtMotivo" runat="server" FieldLabel="Motivo" MaxLength="500"
                                                        MaxLengthText="500" AllowBlank="false" />
                                                </Items>
                                                <Buttons>
                                                    <ext:Button ID="btnGuardarEstatus" runat="server" Text="Guardar" Icon="Disk">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnGuardarEstatus_Click" />
                                                        </DirectEvents>
                                                    </ext:Button>
                                                </Buttons>
                                            </ext:FieldSet>
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
