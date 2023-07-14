<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" ValidateRequest="false"
    CodeBehind="ReporteSaldosPorTarjeta.aspx.cs" Inherits="Empresarial.ReporteSaldosPorTarjeta" %>

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
                            <ext:GridPanel ID="GridPanelSaldos" runat="server" StripeRows="true" Header="false" Border="false">
                                <Store>
                                    <ext:Store ID="StoreSaldos" runat="server" RemoteSort="true" AutoLoad="false"
                                        OnRefreshData="StoreSaldos_RefreshData" GroupField="ClaveMA">
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
                                                    <ext:RecordField Name="ClaveMA" />
                                                    <ext:RecordField Name="DescEstatus" />
                                                    <ext:RecordField Name="EstatusMA" />
                                                    <ext:RecordField Name="CuentaHabiente" />
                                                    <ext:RecordField Name="TipoTarjeta" />
                                                    <ext:RecordField Name="CuentaInterna" />
                                                    <ext:RecordField Name="NombreCuenta" />
                                                    <ext:RecordField Name="SaldoDisponible" />
                                                    <ext:RecordField Name="CodigoMoneda" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                        <ext:ImageCommandColumn ColumnID="colDetalleMovimientos" Width="60">
                                            <Commands>
                                                <ext:ImageCommand Icon="Zoom" CommandName="verDetalles" ToolTip-Text="Ver Detalle"/>
                                            </Commands>
                                        </ext:ImageCommandColumn>
                                        <ext:GroupingSummaryColumn Header="Estatus Cuenta" Sortable="true" DataIndex="DescEstatus"
                                            Width="120" />
                                        <ext:GroupingSummaryColumn Header="Tarjeta" Sortable="true" DataIndex="ClaveMA" />
                                        <ext:GroupingSummaryColumn Header="Estatus Tarjeta" Sortable="true" DataIndex="EstatusMA"
                                            Width="120" />
                                        <ext:GroupingSummaryColumn Header="CuentaHabiente" Sortable="true" DataIndex="CuentaHabiente"
                                            Width="150" />
                                        <ext:GroupingSummaryColumn Header="Tipo Tarjeta" Sortable="true" DataIndex="TipoTarjeta"
                                            Width="100" />
                                        <ext:GroupingSummaryColumn Header="Cuenta Interna" Sortable="true" DataIndex="CuentaInterna"
                                            Width="120" />
                                        <ext:GroupingSummaryColumn Header="ID Cuenta" Sortable="true" DataIndex="ID_Cuenta" Width="80" />
                                        <ext:GroupingSummaryColumn Header="Nombre de Cuenta" Sortable="true" Width="150"
                                            DataIndex="NombreCuenta" />
                                        <ext:NumberColumn Header="Saldo Disponible" Sortable="true" DataIndex="SaldoDisponible"
                                            Width="150" Align="Right" Format="$0,0.00">
                                            <Renderer Format="UsMoney"/>
                                        </ext:NumberColumn>
                                        <ext:GroupingSummaryColumn Header="Moneda" Sortable="true" DataIndex="CodigoMoneda" Width="100" />
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
                                    <ext:PagingToolbar ID="PagingSaldosTarjeta" runat="server" StoreID="StoreSaldos" DisplayInfo="true"
                                        DisplayMsg="Mostrando Saldos {0} - {1} de {2}" />
                                </BottomBar>
                                <View>
                                    <ext:GroupingView ID="GroupingView1" runat="server" ForceFit="true" MarkDirty="false"
                                        ShowGroupName="false" EnableNoGroups="true" HideGroupedColumn="true" />
                                </View>
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar2" runat="server">
                                        <Items>
                                            <ext:TextField Name="Tarjeta" TabIndex="5" EmptyText="Número de Tarjeta" MinLength="16"
                                                MaxLength="16" ID="txtTarjeta" runat="server" Width="180" Text="" MaskRe="[0-9]"/>
                                            <ext:TextField Name="Nombre" TabIndex="5" EmptyText="Nombre y Apellidos" MaxLength="100"
                                                ID="txtNombre" runat="server" Width="180" Text="" />
                                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                                        Before="if (!#{txtTarjeta}.isValid()) { return false; }
                                                        else { resetToolbar(#{PagingSaldosTarjeta});
                                                        #{GridPanelSaldos}.getStore().sortInfo = null; }">
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:ToolbarSeparator runat="server" />
                                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                                <DirectEvents>
                                                    <Click OnEvent="btnLimpiar_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="btnBuscarHide" runat="server" Hidden="true">
                                                <Listeners>
                                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Cuentas...' });
                                                        #{GridPanelSaldos}.getStore().reload({params:{start:0, sort:('','')}});" />
                                                </Listeners>
                                            </ext:Button>
                                            <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                                <DirectEvents>
                                                    <Click OnEvent="Download" IsUpload="true"
                                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                        RepSaldosPorTarjeta.StopMask();" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:ToolbarFill runat="server" />
                                            <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a  Excel" Icon="PageExcel" ToolTip="Obtener Datos en un Archivo Excel"
                                                Disabled="true">
                                                <DirectEvents>
                                                    <Click OnEvent="Download" IsUpload="true"
                                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                            e.stopEvent(); 
                                                            RepSaldosPorTarjeta.StopMask();" />
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </Center>
        <East Split="true">
            <ext:Panel ID="pnlDetalles" runat="server" Width="550" Title="Consulta de Detalle de Movimientos." Collapsed="true"
                Collapsible="true" Layout="FitLayout" AutoScroll="true" LabelWidth="70" LabelAlign="Right">
                <TopBar>
                    <ext:Toolbar runat="server">
                        <Items>
                            <ext:Hidden ID="hdnIdCuenta" runat="server" />
                            <ext:DateField ID="datInicio" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                AllowBlank="false" MsgTarget="Side" Format="yyyy-MM-dd" TabIndex="1" MaxLength="12"
                                Width="160" EnableKeyEvents="true">
                                <CustomConfig>
                                    <ext:ConfigItem Name="endDateField" Value="#{datFinal}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:DateField ID="datFinal" runat="server" Vtype="daterange" FieldLabel="Fecha Final"
                                AllowBlank="false" MaxLength="12" Width="160" MsgTarget="Side" Format="yyyy-MM-dd"
                                EnableKeyEvents="true">
                                <CustomConfig>
                                    <ext:ConfigItem Name="startDateField" Value="#{datInicio}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:Button ID="btnBuscarDetalle" runat="server" Text="Buscar..." ToolTip="Buscar Detalles del Periodo"
                                Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscarDetalle_Click" Timeout="360000"
                                        Before="resetToolbar(#{PagingDetSaldosTarjeta});
                                        #{GridPanelDetalle}.getStore().sortInfo = null;">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarSeparator runat="server" />
                            <ext:Button ID="btnLimpiarDetalle" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiarDetalle_Click" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnBuscarDetalleHide" runat="server" Hidden="true">
                                <Listeners>
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Detalle de Movimientos...' });
                                        #{GridPanelDetalle}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnDownloadDetalleHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="DownloadDetalle" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                RepSaldosPorTarjeta.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <Items>
                    <ext:GridPanel ID="GridPanelDetalle" runat="server" StripeRows="true" Header="false" Border="false">
                        <LoadMask ShowMask="false" />
                        <Store>
                            <ext:Store ID="StoreDetalle" runat="server" RemoteSort="true" AutoLoad="false"
                                OnRefreshData="StoreDetalle_RefreshData">
                                <AutoLoadParams>
                                    <ext:Parameter Name="startDet" Value="0" Mode="Raw" />
                                </AutoLoadParams>
                                <Proxy>
                                    <ext:PageProxy />
                                </Proxy>
                                <DirectEventConfig IsUpload="true" />
                                <Reader>
                                    <ext:JsonReader IDProperty="IDReporte">
                                        <Fields>
                                            <ext:RecordField Name="IDReporte" />
                                            <ext:RecordField Name="ID_Poliza" />
                                            <ext:RecordField Name="FechaPoliza" />
                                            <ext:RecordField Name="Concepto" />
                                            <ext:RecordField Name="Comercio" />
                                            <ext:RecordField Name="Cargo" />
                                            <ext:RecordField Name="Abono" />
                                            <ext:RecordField Name="SaldoFinalDetalle" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <ColumnModel ID="ColumnModel2" runat="server">
                            <Columns>
                                <ext:GroupingSummaryColumn Header="Póliza" Sortable="true" DataIndex="ID_Poliza" Width="60" />
                                <ext:DateColumn Header="Fecha" Sortable="true" DataIndex="FechaPoliza" Format="yyyy-MM-dd"
                                    Width="80"/>
                                <ext:GroupingSummaryColumn Header="Concepto" Sortable="true" DataIndex="Concepto" Width="110" />
                                <ext:GroupingSummaryColumn Header="Comercio" Sortable="true" DataIndex="Comercio" Width="60" />
                                <ext:NumberColumn Header="Cargo" Sortable="true" DataIndex="Cargo" Format="$0,0.00"
                                    Align="Right" Width="80">
                                    <Renderer Format="UsMoney" />
                                </ext:NumberColumn>
                                <ext:NumberColumn Header="Abono" Sortable="true" DataIndex="Abono" Format="$0,0.00"
                                    Align="Right" Width="80">
                                    <Renderer Format="UsMoney" />
                                </ext:NumberColumn>
                                <ext:NumberColumn Header="Saldo Después" Sortable="true" DataIndex="SaldoFinalDetalle"
                                    Format="$0,0.00" Align="Right">
                                    <Renderer Format="UsMoney" />
                                </ext:NumberColumn>
                            </Columns>
                        </ColumnModel>
                        <TopBar>
                            <ext:Toolbar runat="server">
                                <Items>
                                    <ext:ToolbarFill runat="server" />
                                    <ext:ToolbarSeparator runat="server" />
                                    <ext:Button ID="btnExportaDetallesExcel" runat="server" Text="Exportar a Excel" ToolTip="Obtener Datos en un Archivo Excel"
                                        Icon="PageExcel" Disabled="true">
                                        <DirectEvents>
                                            <Click OnEvent="DownloadDetalle" IsUpload="true"
                                                After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                e.stopEvent(); 
                                                RepSaldosPorTarjeta.StopMask();" />
                                        </DirectEvents>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                        <BottomBar>
                            <ext:PagingToolbar ID="PagingDetSaldosTarjeta" runat="server" StoreID="StoreDetalle" DisplayInfo="true"
                                DisplayMsg="Mostrando Detalle de Saldos {0} - {1} de {2}" />
                        </BottomBar>
                    </ext:GridPanel>
                </Items>
            </ext:Panel>
        </East>
    </ext:BorderLayout>
</asp:Content>
