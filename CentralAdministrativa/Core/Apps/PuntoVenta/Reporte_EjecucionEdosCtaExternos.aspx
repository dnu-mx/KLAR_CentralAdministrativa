<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="Reporte_EjecucionEdosCtaExternos.aspx.cs" Inherits="TpvWeb.Reporte_EjecucionEdosCtaExternos" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function resetToolbar(tbar) {
            tbar.updateInfo();
            tbar.inputItem.setValue(1);
            tbar.afterTextItem.setText(String.format(tbar.afterPageText, 1));
            tbar.next.setDisabled(true);
            tbar.prev.setDisabled(true);
            tbar.first.setDisabled(true);
            tbar.last.setDisabled(true);
        }
        
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
    <ext:Hidden ID="hdnIdArchivo" runat="server" />
    <ext:BorderLayout ID="MainBorderLayout" runat="server">
        <Center Split="true">
            <ext:GridPanel ID="GridPanelBusqueda" runat="server" StripeRows="true" Header="false" Border="false" AutoExpandColumn="Nombre"
                Layout="FitLayout" AutoScroll="true">
                <Store>
                    <ext:Store ID="StoreBusqueda" runat="server" RemoteSort="true" AutoLoad="false" OnRefreshData="StoreBusqueda_RefreshData">
                        <AutoLoadParams>
                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                        </AutoLoadParams>
                        <Proxy>
                            <ext:PageProxy />
                        </Proxy>
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Archivo">
                                <Fields>
                                    <ext:RecordField Name="ID_Archivo" />
                                    <ext:RecordField Name="Descripcion" />
                                    <ext:RecordField Name="Nombre" />
                                    <ext:RecordField Name="TipoArchivo" />
                                    <ext:RecordField Name="Fecha" />
                                    <ext:RecordField Name="TotalProcesados" />
                                    <ext:RecordField Name="ProcesadosOK" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <TopBar>
                    <ext:Toolbar runat="server">
                        <Items>
                            <ext:DateField ID="dfFechaInicio" runat="server" Vtype="daterange" Format="dd/MM/yyyy" Editable="false"
                                EmptyText="Fecha Inicial Presentación" Width="180" AllowBlank="false">
                                <CustomConfig>
                                    <ext:ConfigItem Name="endDateField" Value="#{dfFechaFin}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:DateField ID="dfFechaFin" runat="server" Vtype="daterange" Format="dd/MM/yyyy" Editable="false"
                                EmptyText="Fecha Final Presentación" Width="180" AllowBlank="false">
                                <CustomConfig>
                                    <ext:ConfigItem Name="startDateField" Value="#{dfFechaInicio}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                        Before="var valid = #{dfFechaInicio}.isValid(); if (!valid) {} return valid;">
                                    </Click>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                        Before="if (!#{dfFechaInicio}.isValid()) { return false; }
                                        else { resetToolbar(#{PagingBusqueda});
                                        #{GridPanelBusqueda}.getStore().sortInfo = null; }">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarSeparator runat="server" />
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarSeparator runat="server" />
                            <ext:ToolbarFill runat="server" />
                            <ext:Button ID="btnExportExcel" runat="server" Text="Exportar Ejecuciones a Excel" Icon="PageExcel" Disabled="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                            e.stopEvent(); 
                                            RepEjecEdoCtaExt.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnBuscarHide" runat="server" Hidden="true">
                                <Listeners>
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Ejecuciones...' });
                                        #{GridPanelBusqueda}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                        RepEjecEdoCtaExt.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <ColumnModel runat="server">
                    <Columns>
                        <ext:Column Header="ID_Archivo" Sortable="true" DataIndex="ID_Archivo" Width="65" />
                        <ext:Column Header="Descripción" Sortable="true" DataIndex="Descripcion" Width="150" />
                        <ext:Column Header="Nombre del Archivo" Sortable="true" DataIndex="Nombre" Width="250" />
                        <ext:Column Header="Tipo de Archivo" Sortable="true" DataIndex="TipoArchivo" Width="100" />
                        <ext:DateColumn Header="Fecha" Sortable="true" DataIndex="Fecha" Width="120" Format="yyyy-MM-dd HH:mm:ss" />
                        <ext:Column Header="Total Procesados" Sortable="true" DataIndex="TotalProcesados" Width="120" />
                        <ext:Column Header="Procesados Correctamente" Sortable="true" DataIndex="ProcesadosOK" Width="150" />
                    </Columns>
                </ColumnModel>
                <SelectionModel>
                    <ext:RowSelectionModel SingleSelect="true" />
                </SelectionModel>
                <DirectEvents>
                    <RowClick OnEvent="selectRowResultados_Event">
                        <ExtraParams>
                            <ext:Parameter Name="ID_Archivo" Value="Ext.encode(#{GridPanelBusqueda}.getRowsValues({selectedOnly:true})[0].ID_Archivo)" Mode="Raw" />
                        </ExtraParams>
                        <EventMask ShowMask="true" Msg="Obteniendo Detalle..." MinDelay="500" />
                    </RowClick>
                </DirectEvents>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingBusqueda" runat="server" StoreID="StoreBusqueda" DisplayInfo="true"
                        DisplayMsg="Mostrando Ejecuciones {0} - {1} de {2}" HideRefresh="true" />
                </BottomBar>
            </ext:GridPanel>  
        </Center>
        <South Split="true">
            <ext:GridPanel ID="GridDetalleEdoCta" runat="server" StripeRows="true" Height="200" Title="Detalle" Border="false"
                Layout="FitLayout" Disabled="true" Collapsible="true">
                <Store>
                    <ext:Store ID="StoreDetalleEdoCta" runat="server" AutoLoad="false" OnRefreshData="StoreDetalleEdoCta_RefreshData">
                        <AutoLoadParams>
                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                        </AutoLoadParams>
                        <Proxy>
                            <ext:PageProxy />
                        </Proxy>
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="Id_ArchivoDetalleEdoCtaExterno">
                                <Fields>
                                    <ext:RecordField Name="Id_ArchivoDetalleEdoCtaExterno" />
                                    <ext:RecordField Name="ID_Archivo" />
                                    <ext:RecordField Name="AnioMes" />
                                    <ext:RecordField Name="SucursalID" />
                                    <ext:RecordField Name="NombreSucursalCte" />
                                    <ext:RecordField Name="ClienteID" />
                                    <ext:RecordField Name="NombreCompleto" />
                                    <ext:RecordField Name="TipoPersona" />
                                    <ext:RecordField Name="DireccionCompleta" />
                                    <ext:RecordField Name="NombreInstitucion" />
                                    <ext:RecordField Name="DireccionInstitucion" />
                                    <ext:RecordField Name="FechaGeneracion" />
                                    <ext:RecordField Name="TotalInteres" />
                                    <ext:RecordField Name="IvaComisiones" />
                                    <ext:RecordField Name="Comisiones" />
                                    <ext:RecordField Name="Intereses" />
                                    <ext:RecordField Name="NumTransaccion" />
                                    <ext:RecordField Name="SaldoInicial" />
                                    <ext:RecordField Name="CuentaClabe" />
                                    <ext:RecordField Name="Fecha" />
                                    <ext:RecordField Name="Depositos" />
                                    <ext:RecordField Name="Retiros" />
                                    <ext:RecordField Name="Interes" />
                                    <ext:RecordField Name="SaldoActual " />
                                    <ext:RecordField Name="ProcesadoCorrectamente" />
                                    <ext:RecordField Name="Timbrado" />
                                    <ext:RecordField Name="FechaInsercion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <TopBar>
                    <ext:Toolbar runat="server">
                        <Items>
                            <ext:Button ID="btnBuscaDetalles" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscaDetalles_Click" Timeout="360000"
                                        Before="resetToolbar(#{PagingDetalleEdoCta});
                                        #{GridDetalleEdoCta}.getStore().sortInfo = null;">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnSelectDetHide" runat="server" Hidden="true">
                                <Listeners>
                                    <Click Handler="#{GridDetalleEdoCta}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:ToolbarSeparator runat="server" />
                            <ext:ToolbarFill runat="server" />
                            <ext:Button ID="btnDownloadDetHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="DownloadDetalles" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Detalles a Excel...' });
                                        RepEjecEdoCtaExt.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnExcelDetalles" runat="server" Text="Exportar Detalles a Excel" Icon="PageExcel" Disabled="true">
                                <DirectEvents>
                                    <Click OnEvent="DownloadDetalles" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Detalles a Excel...' });
                                            e.stopEvent(); 
                                            RepEjecEdoCtaExt.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <ColumnModel runat="server">
                    <Columns>
                        <ext:Column Header="ID Archivo" DataIndex="ID_Archivo" Width="100" />
                        <ext:Column Header="Año/Mes" DataIndex="AnioMes" Width="100" />
                        <ext:Column Header="ID Sucursal" DataIndex="SucursalID" Width="100" />
                        <ext:Column Header="Nombre Sucursal Cte" DataIndex="NombreSucursalCte" Width="100" />
                        <ext:Column Header="ID Cliente" DataIndex="ClienteID" Width="100" />
                        <ext:Column Header="Nombre Completo" DataIndex="NombreCompleto" Width="100" />
                        <ext:Column Header="Tipo de Persona" DataIndex="TipoPersona" Width="100" />
                        <ext:Column Header="Domicilio" DataIndex="DireccionCompleta" Width="100" />
                        <ext:Column Header="Nombre de la Institución" DataIndex="NombreInstitucion" Width="100" />
                        <ext:Column Header="Dirección de la Institución" DataIndex="DireccionInstitucion" Width="100" />
                        <ext:DateColumn Header="Fecha de Generación" DataIndex="FechaGeneracion" Width="100" Format="yyyy-MM-dd HH:mm:ss" />
                        <ext:Column Header="Total de Interés" DataIndex="TotalInteres" Width="100" />
                        <ext:Column Header="IVA Comisiones" DataIndex="IvaComisiones" Width="100" />
                        <ext:Column Header="Comisiones" DataIndex="Comisiones" Width="100" />
                        <ext:Column Header="Intereses" DataIndex="Intereses" Width="100" />
                        <ext:Column Header="No. Transacción" DataIndex="NumTransaccion" Width="100" />
                        <ext:Column Header="Saldo Inicial" DataIndex="SaldoInicial" Width="100" />
                        <ext:Column Header="CLABE" DataIndex="CuentaClabe" Width="100" />
                        <ext:DateColumn Header="Fecha" DataIndex="Fecha" Width="100" Format="yyyy-MM-dd HH:mm:ss" />
                        <ext:Column Header="Depósitos" DataIndex="Depositos" Width="100" />
                        <ext:Column Header="Retiros" DataIndex="Retiros" Width="100" />
                        <ext:Column Header="Interés" DataIndex="Interes" Width="100" />
                        <ext:Column Header="SaldoActual" DataIndex="SaldoActual" Width="100">
                            <Renderer Format="UsMoney" />
                        </ext:Column>
                        <ext:Column Header="Procesado Correctamente" DataIndex="ProcesadoCorrectamente" Width="100" />
                        <ext:Column Header="Timbrado" DataIndex="Timbrado" Width="100" />
                        <ext:DateColumn Header="Fecha de Inserción" DataIndex="FechaInsercion" Width="100" Format="yyyy-MM-dd HH:mm:ss" />
                    </Columns>
                </ColumnModel>
                <SelectionModel>
                    <ext:RowSelectionModel SingleSelect="true" />
                </SelectionModel>
                <Listeners>
                    <RowClick Handler="#{btnRelacionar}.setDisabled(false); var record = #{GridDetalleEdoCta}.getSelectionModel().getSelected();
                        #{hdnIdOp}.setValue(record.get('IdOperacion'));" />
                </Listeners>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingDetalleEdoCta" runat="server" StoreID="StoreDetalleEdoCta" DisplayInfo="true"
                        DisplayMsg="Mostrando Operaciones {0} - {1} de {2}" HideRefresh="true" />
                </BottomBar>
            </ext:GridPanel>
        </South>
    </ext:BorderLayout>
</asp:Content>
