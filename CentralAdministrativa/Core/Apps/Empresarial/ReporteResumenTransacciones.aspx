<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" ValidateRequest="false"
    CodeBehind="ReporteResumenTransacciones.aspx.cs" Inherits="Empresarial.ReporteResumenTransacciones" %>

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


        var submitValue = function (grid, hiddenFormat, format) {
            hiddenFormat.setValue(format);
            grid.submitData(false);
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
        <Content>
            <ext:Hidden ID="FormatType" runat="server" />
        </Content>
        <Center Split="true">
            <ext:GridPanel ID="GrinPanelTXs" runat="server" StripeRows="true" Header="false" Border="false"
                Layout="FitLayout" AutoScroll="true">
                <Store>
                    <ext:Store ID="StoreResumenTXs" runat="server" RemoteSort="true" AutoLoad="false"
                        OnRefreshData="StoreResumenTXs_RefreshData">
                        <AutoLoadParams>
                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                        </AutoLoadParams>
                        <Proxy>
                            <ext:PageProxy />
                        </Proxy>
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="ID">
                                <Fields>
                                    <ext:RecordField Name="ID" />
                                    <ext:RecordField Name="Descripcion" />
                                    <ext:RecordField Name="Transacciones" />
                                    <ext:RecordField Name="Canceladas" />
                                    <ext:RecordField Name="Totales" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <TopBar>
                    <ext:Toolbar ID="Toolbar2" runat="server" LabelWidth="70" LabelAlign="Right">
                        <Items>                          
                            <ext:DateField ID="dfFechaIniResTX" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                Format="dd/MM/yyyy" Width="200" EnableKeyEvents="true" AllowBlank="false">
                                <CustomConfig>
                                    <ext:ConfigItem Name="endDateField" Value="#{dfFechaFinResTX}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:DateField ID="dfFechaFinResTX" runat="server" Vtype="daterange" FieldLabel="Fecha Final"
                                Width="200" Format="dd/MM/yyyy" EnableKeyEvents="true" AllowBlank="false">
                                <CustomConfig>
                                    <ext:ConfigItem Name="startDateField" Value="#{dfFechaIniResTX}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                        Before="var valid1 = #{dfFechaIniResTX}.isValid(); 
                                        if (!valid1) {
                                            var valid = valid1; }
                                        else {
                                            var valid2 = #{dfFechaFinResTX}.isValid();
                                            if (!valid2) {
                                                var valid = valid2; }
                                            else {
                                                resetToolbar(#{PagingTXs});
                                                #{GrinPanelTXs}.getStore().sortInfo = null; }
                                            } return valid; ">
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
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Transacciones...' });
                                        #{GrinPanelTXs}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                        RepResumenTXs.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarFill runat="server" />
                            <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a  Excel" Icon="PageExcel" ToolTip="Obtener Datos en un Archivo Excel"
                                Disabled="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                        e.stopEvent(); 
                                        RepResumenTXs.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <ColumnModel runat="server">
                    <Columns>
                        <ext:Column Hidden="true" DataIndex="ID" />
                        <ext:Column Header="Descripción" Sortable="true" DataIndex="Descripcion" Width="450" />
                        <ext:Column Header="Transacciones" Sortable="true" DataIndex="Transacciones" Width="200" />
                        <ext:Column Header="Canceladas" Sortable="true" DataIndex="Canceladas" Width="200"  />
                        <ext:Column Header="Totales" Sortable="true" DataIndex="Totales" Width="200"  />
                    </Columns>
                </ColumnModel>
                <SelectionModel>
                    <ext:RowSelectionModel runat="server" SingleSelect="true" />
                </SelectionModel>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingTXs" runat="server" StoreID="StoreResumenTXs" DisplayInfo="true"
                        DisplayMsg="Mostrando Transacciones {0} - {1} de {2}" HideRefresh="true" />
                </BottomBar>
            </ext:GridPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
