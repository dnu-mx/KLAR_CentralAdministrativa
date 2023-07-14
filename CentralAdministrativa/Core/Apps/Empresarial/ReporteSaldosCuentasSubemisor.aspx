<%@ Page Title="" Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master"
    CodeBehind="ReporteSaldosCuentasSubemisor.aspx.cs" Inherits="Empresarial.ReporteSaldosCuentasSubemisor" %>

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

        var link_template = '<span style="color:{0};text-decoration:underline;">{1}</span>';

        var link = function (value) {
            return String.format(link_template, (value != "") ? "blue" : "black", value);
        };

        function resetToolbar(tbar) {
            tbar.updateInfo();
            tbar.inputItem.setValue(1);
            tbar.afterTextItem.setText(String.format(tbar.afterPageText, 1));
            tbar.next.setDisabled(true);
            tbar.prev.setDisabled(true);
            tbar.first.setDisabled(true);
            tbar.last.setDisabled(true);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center Split="true">
            <ext:Panel ID="pnlcuentas" runat="server" Width="850" Title="Saldos" Collapsed="false"
                Collapsible="false" Layout="Fit" AutoScroll="true">
                <Content>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanelCuentasSubem" runat="server" StripeRows="true" Header="false" Border="false">
                                <Store>
                                    <ext:Store ID="StoreCuentasSubemisor" runat="server" RemoteSort="true" OnRefreshData="StoreCuentasSubemisor_RefreshData"
                                        GroupField="SubemisorEmpresaOSubempresa">
                                        <AutoLoadParams>
                                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                        </AutoLoadParams>
                                        <Proxy>
                                            <ext:PageProxy />
                                        </Proxy>
                                        <DirectEventConfig IsUpload="true" />
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Cuenta">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Cuenta" />
                                                    <ext:RecordField Name="SubemisorEmpresaOSubempresa" />
                                                    <ext:RecordField Name="EstatusCuenta" />
                                                    <ext:RecordField Name="DescripcionCuenta" />
                                                    <ext:RecordField Name="SaldoDisponible" />
                                                    <ext:RecordField Name="CodigoMoneda" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <ColumnModel runat="server">
                                    <Columns>
                                        <ext:ImageCommandColumn Width="60">
                                            <Commands>
                                                <ext:ImageCommand Icon="Zoom" CommandName="verDetalles" ToolTip-Text="Ver Detalle" />
                                            </Commands>
                                        </ext:ImageCommandColumn>
                                        <ext:GroupingSummaryColumn Header="Estatus" Sortable="true" DataIndex="EstatusCuenta"
                                            Width="50" />
                                        <ext:GroupingSummaryColumn Header="Cadena Comercial" Sortable="true" DataIndex="SubemisorEmpresaOSubempresa"
                                            Hidden="true" Width="50" />
                                        <ext:GroupingSummaryColumn Header="Nombre de Cuenta" Sortable="true" DataIndex="DescripcionCuenta" />
                                        <ext:NumberColumn Header="Saldo Disponible" Sortable="true" DataIndex="SaldoDisponible" Align="Right"
                                            Format="$0,0.00">
                                            <Renderer Format="UsMoney" />
                                        </ext:NumberColumn>
                                        <ext:GroupingSummaryColumn Header="Moneda" Sortable="true" DataIndex="CodigoMoneda" Width="80" />
                                        <ext:GroupingSummaryColumn Header="ID Cuenta" Sortable="true" DataIndex="ID_Cuenta"
                                            Width="50" />
                                    </Columns>
                                </ColumnModel>
                                <DirectEvents>
                                    <Command OnEvent="Seleccionar">
                                        <ExtraParams>
                                            <ext:Parameter Name="ID_Cuenta" Value="record.data.ID_Cuenta" Mode="Raw"/>
                                        </ExtraParams>
                                    </Command>
                                </DirectEvents>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel2" runat="server" SingleSelect="true" />                                 
                                </SelectionModel>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingSaldosCtasSubem" runat="server" StoreID="StoreCuentasSubemisor" DisplayInfo="true"
                                        DisplayMsg="Mostrando Saldos {0} - {1} de {2}">
                                        <Items>
                                            <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                                <DirectEvents>
                                                    <Click OnEvent="Download" IsUpload="true"
                                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                        RepSaldosCtasSubem.StopMask();" />
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:PagingToolbar>
                                </BottomBar>
                                <View>
                                    <ext:GroupingView ID="GroupingView1" runat="server" ForceFit="true" MarkDirty="false"
                                        ShowGroupName="false" EnableNoGroups="true" HideGroupedColumn="true" />
                                </View>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </Center>
        <East Split="true">
            <ext:Panel ID="pnlDetalleCuentaSubem" runat="server" Width="550" Collapsed="true" Collapsible="true" Layout="FitLayout" MonitorResize="true"
                AutoScroll="true" MonitorWindowResize="true">
                <Content>
                    <ext:Hidden ID="hdnIdCuentaSubem" runat="server" />
                    <ext:BorderLayout ID="BorderLayout3" runat="server">
                        <Center Split="true">
                            <ext:Panel runat="server" Height="450" Width="450" Layout="FitLayout" LabelWidth="85" MonitorResize="true"
                                LabelAlign="Right">
                                <TopBar>
                                    <ext:Toolbar runat="server">
                                        <Items>
                                            <ext:DateField ID="datInicioCC" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                                AllowBlank="false" MsgTarget="Side" Format="yyyy-MM-dd" MaxLength="12"
                                                Width="200" EnableKeyEvents="true">
                                                <CustomConfig>
                                                    <ext:ConfigItem Name="endDateField" Value="#{datFinalCC}" Mode="Value" />
                                                </CustomConfig>
                                                <Listeners>
                                                    <KeyUp Fn="onKeyUp" />
                                                </Listeners>
                                            </ext:DateField>
                                            <ext:DateField ID="datFinalCC" runat="server" Vtype="daterange" FieldLabel="Fecha Final"
                                                AllowBlank="false" MaxLength="12" Width="200" MsgTarget="Side" Format="yyyy-MM-dd"
                                                EnableKeyEvents="true">
                                                <CustomConfig>
                                                    <ext:ConfigItem Name="startDateField" Value="#{datInicioCC}" Mode="Value" />
                                                </CustomConfig>
                                                <Listeners>
                                                    <KeyUp Fn="onKeyUp" />
                                                </Listeners>
                                            </ext:DateField>
                                            <ext:ToolbarFill runat="server" />
                                            <ext:ToolbarSeparator runat="server" />
                                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." ToolTip="Buscar Detalles del Periodo"
                                                Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                                        Before="resetToolbar(#{PagingDetSaldosCS});
                                                        #{GridDetalleCtasSubem}.getStore().sortInfo = null;">
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="btnBuscarDetalleCCHide" runat="server" Hidden="true">
                                                <Listeners>
                                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Detalle de Movimientos...' });
                                                        #{GridDetalleCtasSubem}.getStore().reload({params:{start:0, sort:('','')}});" />
                                                </Listeners>
                                            </ext:Button>
                                            <ext:Button ID="btnDownloadDetCS" runat="server" Hidden="true">
                                                <DirectEvents>
                                                    <Click OnEvent="DownloadDetalleCtaSubem" IsUpload="true"
                                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                        RepSaldosCtasSubem.StopMask();" />
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <Items>
                                    <ext:GridPanel ID="GridDetalleCtasSubem" runat="server" Title="Cuentas" StripeRows="true" Header="false" MonitorResize="true"
                                        Border="false" Layout="FitLayout" MonitorWindowResize="true" >
                                        <Store>
                                            <ext:Store ID="StoreDetalleCtaSubem" runat="server" RemoteSort="true" AutoLoad="false"
                                                OnRefreshData="StoreDetalleCtaSubem_RefreshData" >
                                                <AutoLoadParams>
                                                    <ext:Parameter Name="startDCC" Value="0" Mode="Raw" />
                                                </AutoLoadParams>
                                                <Proxy>
                                                    <ext:PageProxy />
                                                </Proxy>
                                                <DirectEventConfig IsUpload="true" />
                                                <Reader>
                                                    <ext:JsonReader IDProperty="ID_Reporte">
                                                        <Fields>
                                                            <ext:RecordField Name="ID_Reporte" />
                                                            <ext:RecordField Name="ID_Poliza" />
                                                            <ext:RecordField Name="FechaValor" />
                                                            <ext:RecordField Name="FechaPresentacion" />
                                                            <ext:RecordField Name="FechaPoliza" />
                                                            <ext:RecordField Name="Tarjeta" />
                                                            <ext:RecordField Name="Cuenta" />
                                                            <ext:RecordField Name="Tarjetahabiente" />
                                                            <ext:RecordField Name="Producto" />
                                                            <ext:RecordField Name="Concepto" />
                                                            <ext:RecordField Name="Cargo" />
                                                            <ext:RecordField Name="Abono" />
                                                            <ext:RecordField Name="Autorizacion" />
                                                            <ext:RecordField Name="ReferenciaPagoServicio" />
                                                            <ext:RecordField Name="ReferenciaNumerica" />
                                                            <ext:RecordField Name="Observaciones" />
                                                            <ext:RecordField Name="SaldoAntes" />
                                                            <ext:RecordField Name="SaldoDespues" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                                <DirectEventConfig IsUpload="true" />
                                            </ext:Store>
                                        </Store>
                                        <LoadMask ShowMask="false" />
                                        <ColumnModel ID="ColumnModel2" runat="server">
                                            <Columns>
                                                <ext:Column Header="ID" DataIndex="ID_Reporte" Sortable="true" Width="60" />
                                                <ext:Column Header="Poliza" DataIndex="ID_Poliza" Sortable="true" Width="80">
                                                     <Renderer Fn="link" />
                                                </ext:Column>
                                                <ext:DateColumn Header="Fecha Valor" DataIndex="FechaValor" Sortable="true" Width="110"
                                                    Format="yyyy-MM-dd HH:mm:ss" />
                                                <ext:DateColumn Header="Fecha Presentación" DataIndex="FechaPresentacion" Sortable="true" Width="110"
                                                    Format="yyyy-MM-dd" />
                                                <ext:DateColumn Header="Fecha Póliza" DataIndex="FechaPoliza" Sortable="true" Width="110"
                                                    Format="yyyy-MM-dd HH:mm:ss" />
                                                <ext:Column Header="Tarjeta" DataIndex="Tarjeta" Sortable="true" Width="200"/>
                                                <ext:Column Header="Cuenta" DataIndex="Cuenta" Sortable="true" Width="200"/>
                                                <ext:Column Header="Tarjetahabiente" DataIndex="Tarjetahabiente" Sortable="true" Width="200"/>
                                                <ext:Column Header="Producto" DataIndex="Producto" Sortable="true" Width="200"/>
                                                <ext:Column Header="Concepto" DataIndex="Concepto" Sortable="true" Width="200" />
                                                <ext:NumberColumn Header="Cargo" DataIndex="Cargo" Sortable="true" Width="90" Align="Right"
                                                    Format="$0,0.00">
                                                    <Renderer Format="UsMoney" />
                                                </ext:NumberColumn>
                                                <ext:NumberColumn Header="Abono" DataIndex="Abono" Sortable="true" Width="90" Align="Right"
                                                    Format="$0,0.00">
                                                    <Renderer Format="UsMoney" />
                                                </ext:NumberColumn>
                                                <ext:Column Header="Saldo Antes" DataIndex="SaldoAntes" Sortable="true" Width="100" Align="Right">
                                                    <Renderer Format="UsMoney" />
                                                </ext:Column>
                                                <ext:Column Header="Saldo Después" DataIndex="SaldoDespues" Sortable="true" Width="100" Align="Right">
                                                    <Renderer Format="UsMoney" />
                                                </ext:Column>
                                                <ext:Column Header="Autorización" DataIndex="Autorizacion" Sortable="true" Width="90" />
                                                <ext:Column Header="Referencia" DataIndex="ReferenciaPagoServicio" Sortable="true" Width="100" />
                                                <ext:Column Header="Ref. Num." DataIndex="ReferenciaNumerica" Sortable="true" Width="80" />
                                                <ext:Column Header="Observaciones" DataIndex="Observaciones" Sortable="true" Width="200"/>
                                            </Columns>
                                        </ColumnModel>
                                        <SelectionModel>
                                           <ext:CellSelectionModel runat="server">
                                                <DirectEvents>
                                                    <CellSelect OnEvent="Cell_Click" />
                                                </DirectEvents>
                                            </ext:CellSelectionModel>
                                        </SelectionModel>
                                        <Plugins>
                                            <ext:GridFilters runat="server" Local="true" FiltersText="Filtros">
                                                <Filters>
                                                    <ext:DateFilter DataIndex="FechaValor" BeforeText="Antes de" OnText="El día"
                                                        AfterText="Después de">
                                                        <DatePickerOptions runat="server" TodayText="Hoy" />
                                                    </ext:DateFilter>
                                                    <ext:DateFilter DataIndex="FechaPresentacion" BeforeText="Antes de" OnText="El día"
                                                        AfterText="Después de">
                                                        <DatePickerOptions runat="server" TodayText="Hoy" />
                                                    </ext:DateFilter>
                                                    <ext:DateFilter DataIndex="FechaPoliza" BeforeText="Antes de" OnText="El día"
                                                        AfterText="Después de">
                                                        <DatePickerOptions runat="server" TodayText="Hoy" />
                                                    </ext:DateFilter>
                                                    <ext:StringFilter DataIndex="Tarjetahabiente" />
                                                    <ext:StringFilter DataIndex="Producto" />
                                                    <ext:StringFilter DataIndex="Concepto" />
                                                </Filters>
                                            </ext:GridFilters>
                                        </Plugins>
                                        <TopBar>
                                            <ext:Toolbar runat="server">
                                                <Items>
                                                    <ext:ToolbarFill runat="server" />
                                                    <ext:ToolbarSeparator runat="server" />
                                                    <ext:Button ID="btnExportDCS_Excel" runat="server" Text="Exportar a Excel" Icon="PageExcel" 
                                                        ToolTip="Obtener Datos en un Archivo Excel" Disabled="true">
                                                        <DirectEvents>
                                                            <Click OnEvent="DownloadDetalleCtaSubem" IsUpload="true"
                                                                After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                                e.stopEvent(); 
                                                                RepSaldosCtasSubem.StopMask();" />
                                                        </DirectEvents>
                                                    </ext:Button>
                                                </Items>
                                            </ext:Toolbar>
                                        </TopBar>
                                    </ext:GridPanel>
                                </Items>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingDetSaldosCS" runat="server" StoreID="StoreDetalleCtaSubem" DisplayInfo="true"
                                        DisplayMsg="Mostrando Detalle de Movimientos {0} - {1} de {2}" />
                                </BottomBar>
                            </ext:Panel>
                        </Center>
                        <South Split="true">
                            <ext:Panel ID="PanelDetallePoliza" runat="server" Width="450" height="150" Title="Detalle Póliza" Collapsed="true"
                                Collapsible="true" Layout="FitLayout">
                                <Items>
                                    <ext:GridPanel id="GridDetallePoliza" runat="server" StripeRows="true" Header="false"
                                        Border="false" Layout="FitLayout">
                                        <Store>
                                            <ext:Store ID="StoreDetallePoliza" runat="server">
                                                <DirectEventConfig IsUpload="true" />
                                                <Reader>
                                                    <ext:JsonReader IDProperty="ID_PolizaDetalle">
                                                        <Fields>
                                                            <ext:RecordField Name="ID_Poliza" />
                                                            <ext:RecordField Name="ID_PolizaDetalle" />
                                                            <ext:RecordField Name="ID_Cuenta" />
                                                            <ext:RecordField Name="DescripcionCuenta" />
                                                            <ext:RecordField Name="NombreCuentahabiente" />
                                                            <ext:RecordField Name="Cargo" />
                                                            <ext:RecordField Name="Abono" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                                <DirectEventConfig IsUpload="true" />
                                            </ext:Store>
                                        </Store>
                                        <ColumnModel runat="server">
                                            <Columns>
                                                <ext:Column ColumnID="ID_PolizaDetalle" Hidden="true" DataIndex="ID_PolizaDetalle" />
                                                <ext:Column ColumnID="ID_Cuenta" Header="ID Cuenta" DataIndex="ID_Cuenta" Width="75" />
                                                <ext:Column ColumnID="DescripcionCuenta" Header="Descripción Cuenta" DataIndex="DescripcionCuenta"
                                                    Width="170" />
                                                <ext:Column ColumnID="NombreCuentahabiente" Header="Cuentahabiente" DataIndex="NombreCuentahabiente"
                                                    Width="170" />
                                                <ext:Column ColumnID="Cargo" Header="Cargo" DataIndex="Cargo" Width="70">
                                                    <renderer Format="UsMoney" />
                                                </ext:Column>
                                                <ext:Column ColumnID="Abono" Header="Abono" DataIndex="Abono" Width="70">
                                                    <renderer Format="UsMoney" />
                                                </ext:Column>
                                            </Columns>
                                        </ColumnModel>
                                    </ext:GridPanel>
                                </Items>
                            </ext:Panel>
                        </South>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </East>
    </ext:BorderLayout>
</asp:Content>
