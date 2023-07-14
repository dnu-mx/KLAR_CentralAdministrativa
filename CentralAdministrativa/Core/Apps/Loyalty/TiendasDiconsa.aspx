<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TiendasDiconsa.aspx.cs" Inherits="CentroContacto.TiendasDiconsa" %>


<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
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

        Number.prototype.formatMoney = function(c){
            var n = this, 
                c = isNaN(c = Math.abs(c)) ? 2 : c, 
                d = d == undefined ? "." : d, 
                t = t == undefined ? "," : t, 
                s = n < 0 ? "-" : "", 
                i = String(parseInt(n = Math.abs(Number(n) || 0).toFixed(c))), 
                j = (j = i.length) > 3 ? j % 3 : 0;
            return s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
        };
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="MainBorderLayout" runat="server">
        <West Split="true">
            <ext:Panel runat="server" Width="350" Collapsible="true">
                <Content>
                    <ext:BorderLayout ID="LeftBorderLayout" runat="server">
                        <North Split="true">
                            <ext:FormPanel ID="FormPanelBusqueda" runat="server" Title="Consulta Tiendas DICONSA" Height="265" Layout="FitLayout" Frame="true" LabelWidth="120" Collapsible="true">
                                <Items>
                                    <ext:FieldSet ID="FieldSetBusqueda" runat="server" Title="Búsqueda">
                                        <Items>
                                            <ext:TextField ID="txtMovil" runat="server" LabelAlign="Right" FieldLabel="Móvil" MaxLength="10" Width="300" />
                                            <ext:TextField ID="txtClaveAlmacen" runat="server" LabelAlign="Right" FieldLabel="Clave Almacén" MaxLength="30" Width="300" />
                                            <ext:TextField ID="txtClaveTienda" runat="server" LabelAlign="Right" FieldLabel="Clave Tienda" MaxLength="30" Width="300" />
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
                            <ext:FormPanel ID="FormPanelResultados" runat="server" Title="Resultados Tiendas" Layout="FitLayout">
                                <Items>
                                    <ext:GridPanel ID="GridResultados" runat="server" Height="550" AutoDoLayout="true">
                                        <Store>
                                            <ext:Store ID="StoreTiendas" runat="server" OnRefreshData="StoreTiendas_Refresh">
                                                <Reader>
                                                    <ext:JsonReader IDProperty="ID_Colectiva">
                                                        <Fields>
                                                            <ext:RecordField Name="ID_Colectiva" />
                                                            <ext:RecordField Name="Movil" />
                                                            <ext:RecordField Name="NombreTienda" />
                                                            <ext:RecordField Name="Email" />
                                                            <ext:RecordField Name="Operador" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Store>
                                        <ColumnModel runat="server">
                                            <Columns>
                                                <ext:Column DataIndex="ID_Colectiva" Hidden="true" />
                                                <ext:Column DataIndex="Movil" Header="Móvil" Width="75" />
                                                <ext:Column DataIndex="NombreTienda" Header="Tienda" Width="150" />
                                                <ext:Column DataIndex="Operador" Header="Encargado" Width="115" />
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
            <ext:Panel runat="server" Height="250">
                <Items>
                    <ext:BorderLayout runat="server">
                        <Center>
                            <ext:TabPanel runat="server">
                                <Items>
                                    <ext:FormPanel ID="FormPanelDatos" runat="server" Title="Datos" LabelAlign="Left" LabelWidth="200" >
                                        <Items>
                                            <ext:FieldSet ID="FieldSetDatos" runat="server" Title="Información de la Tienda">
                                                <Items>
                                                    <ext:TextField ID="txtID_Colectiva" runat="server" Hidden="true" Enabled="false" />
                                                    <ext:TextField ID="txtNombreTienda" runat="server" ReadOnly="true" FieldLabel="Nombre" Width="500" />
                                                    <ext:TextField ID="txtClaveAlmacenTienda" runat="server" ReadOnly="true" FieldLabel="Clave del Almacén" MaxLength="30" Width="500" />
                                                    <ext:TextField ID="txtClaveTienda_Tienda" runat="server" ReadOnly="true" FieldLabel="Clave de la Tienda" MaxLength="30" Width="500" />
                                                    <ext:TextField ID="txtEmailTienda" runat="server" ReadOnly="true" FieldLabel="Correo Electrónico" MaxLength="150" Width="500" />
                                                    <ext:TextField ID="txtMovilTienda" runat="server" ReadOnly="true" FieldLabel="Móvil" MaxLength="10" Width="500" />
                                                    <ext:TextField ID="txtLimiteCredito" runat="server" ReadOnly="true" FieldLabel="Límite de Crédito" Width="500" />
                                                    <ext:TextField ID="txtAdeudoDia" runat="server" ReadOnly="true" FieldLabel="Adeudo al Día" Width="500" />
                                                    <ext:TextField ID="txtSaldoDisponible" runat="server" ReadOnly="true" FieldLabel="Saldo Disponible" Width="500" />
                                                    <ext:DateField ID="dfFechaUltCorte" runat="server" ReadOnly="true" FieldLabel="Fecha de Último Corte" Width="500" />
                                                    <ext:TextField ID="txtAdeudoUltCorte" runat="server" ReadOnly="true" FieldLabel="Adeudo al Último Corte" Width="500" />
                                                    <ext:DateField ID="dfFechaUltRec" runat="server" ReadOnly="true" FieldLabel="Fecha de Última Recolección" Width="500" />
                                                    <ext:TextField ID="txtMontoUltRec" runat="server" ReadOnly="true" FieldLabel="Monto de la Última Recolección" Width="500" />
                                                </Items>
                                            </ext:FieldSet>
                                        </Items>        
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelOperador" runat="server" Title="Encargado" LabelAlign="Left" LabelWidth="200" >
                                        <Items>
                                            <ext:FieldSet ID="FieldSetOperador" runat="server" Title="Información del Encargado">
                                                <Items>
                                                    <ext:TextField ID="txtID_Operador" runat="server" Hidden="true" Enabled="false" />
                                                    <ext:TextField ID="txtNombreOperador" runat="server" ReadOnly="true" FieldLabel="Nombre" Width="500" />
                                                    <ext:TextField ID="txtApPaternoOperador" runat="server" ReadOnly="true" FieldLabel="Primer Apellido" MaxLength="30" Width="500" />
                                                    <ext:TextField ID="txtApMaternoOperador" runat="server" ReadOnly="true" FieldLabel="Segundo Apellido" MaxLength="30" Width="500" />
                                                    <ext:TextField ID="txtEmailOperdor" runat="server" FieldLabel="Correo Electrónico" MaxLength="150" Width="500" />
                                                    <ext:NumberField ID="nfMovilOperador" runat="server" FieldLabel="Móvil" MaxLength="10" Width="500" 
                                                        AllowDecimals="false" AllowNegative="false" />
                                                </Items>
                                                <Buttons>
                                                    <ext:Button ID="btnGuardaOperador" runat="server" Text="Guardar" Icon="Disk">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnGuardaOperador_Click" Before="var valid= #{FormPanelOperador}.getForm().isValid(); if (!valid) {} return valid;" />
                                                        </DirectEvents>
                                                    </ext:Button>
                                                </Buttons>
                                            </ext:FieldSet>
                                        </Items>        
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelMovimientos" runat="server" Title="Movimientos">
                                        <Content>
                                            <ext:Hidden ID="FormatType" runat="server" />
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
                                                            <ext:FieldSet ID="FieldSetBuscarMov" runat="server" Title="Movimientos de la Cuenta">
                                                                <Items>
                                                                    <ext:DateField ID="dfFechaInicialMov" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                                                        AllowBlank="false" Format="dd-MMM-yyyy" MaxLength="12" TabIndex="1" EnableKeyEvents="true"
                                                                        Width="500" MaxWidth="500">
                                                                        <CustomConfig>
                                                                            <ext:ConfigItem Name="FechaInicialMov" Value="#{dfFechaInicialMov}" Mode="Value" />
                                                                        </CustomConfig>
                                                                        <Listeners>
                                                                            <KeyUp Fn="onKeyUp" />
                                                                        </Listeners>
                                                                    </ext:DateField>
                                                                    <ext:DateField ID="dfFechaFinalMov" runat="server" Vtype="daterange" FieldLabel="Fecha Final"
                                                                        AllowBlank="false" MaxLength="12" Format="dd-MMM-yyyy" TabIndex="2" EnableKeyEvents="true"
                                                                        Width="500" MaxWidth="500">
                                                                        <CustomConfig>
                                                                            <ext:ConfigItem Name="FechaFinalMov" Value="#{dfFechaFinalMov}" Mode="Value" />
                                                                        </CustomConfig>
                                                                        <Listeners>
                                                                            <KeyUp Fn="onKeyUp" />
                                                                        </Listeners>
                                                                    </ext:DateField>
                                                                    <ext:ComboBox ID="cBoxTipoCuenta" runat="server" FieldLabel="Tipo de Cuenta" StoreID="StoreTipoCuenta" Width="500"
                                                                        DisplayField="Descripcion" ValueField="ID_TipoCuenta" />
                                                                    <ext:ComboBox ID="cBoxTipoOperacion" runat="server" FieldLabel="Tipo de Operación" StoreID="StoreTipoOperacion" Width="500"
                                                                        DisplayField="Descripcion" ValueField="ID_Evento" EmptyText="Todos"/>
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
                                                    <ext:GridPanel ID="GridResultadosMov" runat="server" Header="true" Title="Tienda:">
                                                        <Store>
                                                            <ext:Store ID="StoreResultadosMov" runat="server" OnSubmitData="StoreResultadosMov_Submit" OnRefreshData="btnBuscarMov_Click">
                                                                <DirectEventConfig IsUpload="true" />
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="ID_Poliza">
                                                                        <Fields>
                                                                            <ext:RecordField Name="ID_Poliza" />
                                                                            <ext:RecordField Name="Fecha" />
                                                                            <ext:RecordField Name="TipoOperacion" />
                                                                            <ext:RecordField Name="Cargo" />
                                                                            <ext:RecordField Name="Abono" />
                                                                            <ext:RecordField Name="SaldoFinal" />
                                                                        </Fields>
                                                                    </ext:JsonReader>
                                                                </Reader>
                                                                <DirectEventConfig IsUpload="true" />
                                                            </ext:Store>
                                                        </Store>
                                                        <ColumnModel runat="server">
                                                            <Columns>
                                                                <ext:Column DataIndex="ID_Poliza" Header="Poliza" Width="45" />
                                                                <ext:DateColumn DataIndex="Fecha" Header="Fecha/Hora" Format="dd-MMM-yyyy HH:mm:ss" Width="120" />
                                                                <ext:Column DataIndex="TipoOperacion" Header="Concepto" Width="170" />
                                                                <ext:Column DataIndex="Cargo" Header="Cargo" Align="Right" Width="60">
                                                                    <Renderer Handler="return (value).formatMoney(2);" />
                                                                </ext:Column>
                                                                <ext:Column DataIndex="Abono" Header="Abono" Align="Right" Width="60">
                                                                    <Renderer Handler="return (value).formatMoney(2);" />
                                                                </ext:Column>
                                                                <ext:Column DataIndex="SaldoFinal" Header="Saldo" Align="Right"/>
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
                                                                    <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" Icon="PageExcel">
                                                                        <%--<Listeners>
                                                                            <Click Handler="submitValue(#{GridResultadosMov}, #{FormatType}, 'xls');" />
                                                                        </Listeners>--%>
                                                                        <Listeners>
                                                                            <Click Handler="selectRowResultados_Event">
                                                                                <ExtraParams>
                                                                                    <ext:Parameter Name="Values" Value="Ext.encode(#{GridResultadosMov}.getRowsValues({selectedOnly:false}))" Mode="Raw" />
                                                                                </ExtraParams>
                                                                            </Click>
                                                                        </Listeners>
                                                                    </ext:Button>
                                                                </Items>
                                                            </ext:Toolbar>
                                                        </TopBar>
                                                    </ext:GridPanel>
                                                </Center>
                                            </ext:BorderLayout>
                                        </Content>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelCapturarLlamada" runat="server" Title="Registrar Llamada">
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
                                            <ext:FormPanel ID="FormPanelLlamada" runat="server" Title="Tienda:">
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
                                    <ext:FormPanel ID="FormPanelCobros" runat="server" Title="Cobros con Tarjeta">
                                        <Content>
                                            <ext:Hidden ID="FormatTypeCob" runat="server" />
                                            <ext:BorderLayout ID="BorderLayoutCobros" runat="server">
                                                <North Split="true">
                                                    <ext:FormPanel ID="FormPanelBuscarCobros" runat="server" LabelWidth="200" Height="130" Layout="FitLayout">
                                                        <Items>
                                                            <ext:FieldSet ID="FieldSetCobros" runat="server" Title="Consulta de Cobros con Tarjeta">
                                                                <Items>
                                                                    <ext:DateField ID="dfFechaInicialCob" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                                                        AllowBlank="false" Format="dd-MMM-yyyy" MaxLength="12" TabIndex="1" EnableKeyEvents="true"
                                                                        Width="500" MaxWidth="500">
                                                                        <CustomConfig>
                                                                            <ext:ConfigItem Name="FechaInicialCob" Value="#{dfFechaInicialCob}" Mode="Value" />
                                                                        </CustomConfig>
                                                                        <Listeners>
                                                                            <KeyUp Fn="onKeyUp" />
                                                                        </Listeners>
                                                                    </ext:DateField>
                                                                    <ext:DateField ID="dfFechaFinalCob" runat="server" Vtype="daterange" FieldLabel="Fecha Final"
                                                                        AllowBlank="false" MaxLength="12" Format="dd-MMM-yyyy" TabIndex="2" EnableKeyEvents="true"
                                                                        Width="500" MaxWidth="500">
                                                                        <CustomConfig>
                                                                            <ext:ConfigItem Name="FechaFinalCob" Value="#{dfFechaFinalCob}" Mode="Value" />
                                                                        </CustomConfig>
                                                                        <Listeners>
                                                                            <KeyUp Fn="onKeyUp" />
                                                                        </Listeners>
                                                                    </ext:DateField>
                                                                </Items>
                                                                <Buttons>
                                                                    <ext:Button ID="btnBuscarCob" runat="server" Text="Buscar" Icon="Magnifier">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="btnBuscarCob_Click" Before="var valid= #{FormPanelBuscarCobros}.getForm().isValid(); if (!valid) {} return valid;" />
                                                                        </DirectEvents>
                                                                    </ext:Button>
                                                                </Buttons>
                                                            </ext:FieldSet>
                                                        </Items>
                                                    </ext:FormPanel>
                                                </North>
                                                <Center Split="true">
                                                    <ext:GridPanel ID="GridResultadosCob" runat="server" Header="true" Title="Tienda:">
                                                        <Store>
                                                            <ext:Store ID="StoreResultadosCob" runat="server" OnSubmitData="StoreResultadosCob_Submit" OnRefreshData="btnBuscarCob_Click">
                                                                <DirectEventConfig IsUpload="true" />
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="Transaction">
                                                                        <Fields>
                                                                            <ext:RecordField Name="Transaction" />
                                                                            <ext:RecordField Name="Fecha" />
                                                                            <ext:RecordField Name="MetodoPago" />
                                                                            <ext:RecordField Name="Referencia" />
                                                                            <ext:RecordField Name="Total" />
                                                                            <ext:RecordField Name="Comision" />
                                                                        </Fields>
                                                                    </ext:JsonReader>
                                                                </Reader>
                                                                <DirectEventConfig IsUpload="true" />
                                                            </ext:Store>
                                                        </Store>
                                                        <ColumnModel ID="ColumnModel1" runat="server">
                                                            <Columns>
                                                                <ext:Column DataIndex="Transaction" Hidden="true" />
                                                                <ext:Column DataIndex="Fecha" Header="Fecha" Width="80" />
                                                                <ext:Column DataIndex="MetodoPago" Header="Método de Pago" Width="100" />
                                                                <ext:Column DataIndex="Referencia" Header="Referencia" Width="200" />
                                                                <ext:Column DataIndex="Total" Header="Total" />
                                                                <ext:Column DataIndex="Comision" Header="Comisión" />
                                                            </Columns>
                                                        </ColumnModel>
                                                        <SelectionModel>
                                                            <ext:RowSelectionModel SingleSelect="true" />
                                                        </SelectionModel>
                                                        <BottomBar>
                                                            <ext:PagingToolbar ID="PagingToolBar3" runat="server" StoreID="StoreResultadosCob" DisplayInfo="true"
                                                                DisplayMsg="Mostrando Cobros con Tarjeta {0} - {1} de {2}" />
                                                        </BottomBar>
                                                        <TopBar>
                                                            <ext:Toolbar ID="ToolbarResultadosCob" runat="server">
                                                                <Items>
                                                                    <ext:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                                    <ext:Button ID="btnExportarResultadosCob" runat="server" Text="Exportar a Excel" Icon="PageExcel">
                                                                        <Listeners>
                                                                            <Click Handler="submitValue(#{GridResultadosCob}, #{FormatTypeCob}, 'xls');" />
                                                                        </Listeners>
                                                                    </ext:Button>
                                                                </Items>
                                                            </ext:Toolbar>
                                                        </TopBar>
                                                    </ext:GridPanel>
                                                </Center>
                                            </ext:BorderLayout>
                                        </Content>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelRembolsos" runat="server" Title="Rembolsos">
                                        <Content>
                                            <ext:Hidden ID="FormatTypeRmb" runat="server" />
                                            <ext:BorderLayout ID="BorderLayoutRembolsos" runat="server">
                                                <North Split="true">
                                                    <ext:FormPanel ID="FormPanelBuscarRmb" runat="server" LabelWidth="200" Height="130" Layout="FitLayout">
                                                        <Items>
                                                            <ext:FieldSet ID="FieldSetBuscarRmb" runat="server" Title="Consulta de Rembolsos">
                                                                <Items>
                                                                    <ext:DateField ID="dfFechaInicialRmb" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                                                        AllowBlank="false" Format="dd-MMM-yyyy" MaxLength="12" TabIndex="1" EnableKeyEvents="true"
                                                                        Width="500" MaxWidth="500">
                                                                        <CustomConfig>
                                                                            <ext:ConfigItem Name="FechaInicialRmb" Value="#{dfFechaInicialRmb}" Mode="Value" />
                                                                        </CustomConfig>
                                                                        <Listeners>
                                                                            <KeyUp Fn="onKeyUp" />
                                                                        </Listeners>
                                                                    </ext:DateField>
                                                                    <ext:DateField ID="dfFechaFinalRmb" runat="server" Vtype="daterange" FieldLabel="Fecha Final"
                                                                        AllowBlank="false" MaxLength="12" Format="dd-MMM-yyyy" TabIndex="2" EnableKeyEvents="true"
                                                                        Width="500" MaxWidth="500">
                                                                        <CustomConfig>
                                                                            <ext:ConfigItem Name="FechaFinalRmb" Value="#{dfFechaFinalRmb}" Mode="Value" />
                                                                        </CustomConfig>
                                                                        <Listeners>
                                                                            <KeyUp Fn="onKeyUp" />
                                                                        </Listeners>
                                                                    </ext:DateField>
                                                                </Items>
                                                                <Buttons>
                                                                    <ext:Button ID="btnBuscarRmb" runat="server" Text="Buscar" Icon="Magnifier">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="btnBuscarRmb_Click" Before="var valid= #{FormPanelBuscarRmb}.getForm().isValid(); if (!valid) {} return valid;" />
                                                                        </DirectEvents>
                                                                    </ext:Button>
                                                                </Buttons>
                                                            </ext:FieldSet>
                                                        </Items>
                                                    </ext:FormPanel>
                                                </North>
                                                <Center Split="true">
                                                    <ext:GridPanel ID="GridResultadosRmb" runat="server" Header="true" Title="Tienda:">
                                                        <Store>
                                                            <ext:Store ID="StoreResultadosRmb" runat="server" OnSubmitData="StoreResultadosRmb_Submit" OnRefreshData="btnBuscarRmb_Click">
                                                                <DirectEventConfig IsUpload="true" />
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="DateRequest">
                                                                        <Fields>
                                                                            <ext:RecordField Name="DateRequest" />
                                                                            <ext:RecordField Name="Fecha" />
                                                                            <ext:RecordField Name="Banco" />
                                                                            <ext:RecordField Name="Tarjetahabiente" />
                                                                            <ext:RecordField Name="Importe" />
                                                                            <ext:RecordField Name="Comision" />
                                                                            <ext:RecordField Name="Estado" />
                                                                        </Fields>
                                                                    </ext:JsonReader>
                                                                </Reader>
                                                                <DirectEventConfig IsUpload="true" />
                                                            </ext:Store>
                                                        </Store>
                                                        <ColumnModel ID="ColumnModel2" runat="server">
                                                            <Columns>
                                                                <ext:Column DataIndex="ID_DateRequestRembolso" Hidden="true" />
                                                                <ext:Column DataIndex="Fecha" Header="Fecha" Width="80" />
                                                                <ext:Column DataIndex="Banco" Header="Banco" Width="100" />
                                                                <ext:Column DataIndex="Tarjetahabiente" Header="Tarjetahabiente" Width="200" />
                                                                <ext:Column DataIndex="Importe" Header="Importe" />
                                                                <ext:Column DataIndex="Comision" Header="Comisión" />
                                                                <ext:Column DataIndex="Estado" Header="Estado" />
                                                            </Columns>
                                                        </ColumnModel>
                                                        <SelectionModel>
                                                            <ext:RowSelectionModel SingleSelect="true" />
                                                        </SelectionModel>
                                                        <BottomBar>
                                                            <ext:PagingToolbar ID="PagingToolBar4" runat="server" StoreID="StoreResultadosRmb" DisplayInfo="true"
                                                                DisplayMsg="Mostrando Rembolsos {0} - {1} de {2}" />
                                                        </BottomBar>
                                                        <TopBar>
                                                            <ext:Toolbar ID="Toolbar1" runat="server">
                                                                <Items>
                                                                    <ext:ToolbarFill ID="ToolbarFill3" runat="server" />
                                                                    <ext:Button ID="btnExportarExcelRmb" runat="server" Text="Exportar a Excel" Icon="PageExcel">
                                                                        <Listeners>
                                                                            <Click Handler="submitValue(#{GridResultadosRmb}, #{FormatTypeRmb}, 'xls');" />
                                                                        </Listeners>
                                                                    </ext:Button>
                                                                </Items>
                                                            </ext:Toolbar>
                                                        </TopBar>
                                                    </ext:GridPanel>
                                                </Center>
                                            </ext:BorderLayout>
                                        </Content>
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
