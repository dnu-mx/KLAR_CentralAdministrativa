<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" ValidateRequest="false"
    CodeBehind="Reporte_CompensacionApiCacao.aspx.cs" Inherits="OperacionesEvertec.Reporte_CompensacionApiCacao" %>

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
        <%--<Content>
            <ext:Hidden ID="FormatType" runat="server" />
        </Content>--%>
        <Center Split="true">
            <ext:GridPanel ID="GridFechasCompensacion" runat="server" Border="false" Header="false" AutoScroll="true"
                Layout="FitLayout" LabelWidth="80" LabelAlign="Right">
                <LoadMask ShowMask="true" Msg="Obteniendo reporte...." />
                <TopBar>
                    <ext:Toolbar ID="Toolbar2" runat="server">
                        <Items>
                            <ext:DateField ID="dfFechaInicio" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                Format="dd/MM/yyyy" Width="170" EnableKeyEvents="true" AllowBlank="false">
                                <CustomConfig>
                                    <ext:ConfigItem Name="endDateField" Value="#{dfFechaFin}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:DateField ID="dfFechaFin" runat="server" Vtype="daterange" FieldLabel="Fecha Final"
                                Width="170" Format="dd/MM/yyyy" EnableKeyEvents="true" AllowBlank="false">
                                <CustomConfig>
                                    <ext:ConfigItem Name="startDateField" Value="#{dfFechaInicio}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                        Before="if (!#{dfFechaInicio}.getValue() && !#{dfFechaFin}.getValue())
                                        { return false; } else { resetToolbar(#{PagingFechasCompensacion});
                                        #{GridFechasCompensacion}.getStore().sortInfo = null; }">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                             <ext:Button ID="btnBuscarHide" runat="server" Hidden="true">
                                <Listeners>
                                    <Click Handler="
                                        #{GridFechasCompensacion}.getStore().reload({params:{start:0, sort:('','')}});" />
                                    <%--<Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Operaciones...' });
                                        #{GridFechasCompensacion}.getStore().reload({params:{start:0, sort:('','')}});" />--%>
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                            <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                e.stopEvent(); 
                                                RepProcBatchOpEvtc.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                            RepProcBatchOpEvtc.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <Store>
                    <%--OnSubmitData="StoreFicheros_Submit"--%>
                    <ext:Store ID="StoreFicheros" runat="server" GroupField="Fecha" RemoteSort="true"
                        OnRefreshData="StoreFicheros_RefreshData">
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
                                    <ext:RecordField Name="Fecha" />
                                    <ext:RecordField Name="NombreFichero" />
                                    <ext:RecordField Name="Compensadas" />
                                    <ext:RecordField Name="NoEncontradas" />
                                    <ext:RecordField Name="Duplicadas" />
                                    <ext:RecordField Name="ReversadasPorAclarar" />
                                    <ext:RecordField Name="ReversadasOK" />
                                    <ext:RecordField Name="Devueltas" />
                                    <ext:RecordField Name="DevolucionesPrevias" />
                                    <ext:RecordField Name="NoSonTransacciones" />
                                    <ext:RecordField Name="CasosNoPrevistos" />
                                    <ext:RecordField Name="Reversadas" />
                                    <ext:RecordField Name="NoAplica" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <ColumnModel runat="server">
                    <Columns>
                        <ext:Column DataIndex="ID_Fichero" Hidden="true" />
                        <ext:ImageCommandColumn Width="40" Header="Fecha" />
                        <ext:GroupingSummaryColumn DataIndex="Fecha" Hidden="true" />
                        <ext:GroupingSummaryColumn Header="Archivo" DataIndex="NombreFichero" Width="100" />
                        <ext:GroupingSummaryColumn Header="Compensadas" DataIndex="Compensadas" Width="80" />
                        <ext:GroupingSummaryColumn Header="No Encontradas" DataIndex="NoEncontradas" Width="80" />
                        <ext:GroupingSummaryColumn Header="Duplicadas" DataIndex="Duplicadas" Width="80" />
                        <ext:GroupingSummaryColumn Header="Reversadas por Aclarar" DataIndex="ReversadasPorAclarar" Width="80" />
                        <ext:GroupingSummaryColumn Header="Reversadas OK" DataIndex="ReversadasOK" Width="80" />
                        <ext:GroupingSummaryColumn Header="Devueltas" DataIndex="Devueltas" Width="80" />
                        <ext:GroupingSummaryColumn Header="Devueltas Previamente" DataIndex="DevolucionesPrevias" Width="80" />
                        <ext:GroupingSummaryColumn Header="No son Transacciones" DataIndex="NoSonTransacciones" Width="80" />
                        <ext:GroupingSummaryColumn Header="Casos no Previstos" DataIndex="CasosNoPrevistos" Width="80" />
                        <ext:GroupingSummaryColumn Header="Reversadas" DataIndex="Reversadas" Width="80" />
                        <ext:GroupingSummaryColumn Header="No Aplica" DataIndex="NoAplica" Width="80" />
                    </Columns>
                </ColumnModel>
                <SelectionModel>
                    <ext:RowSelectionModel SingleSelect="true" />
                </SelectionModel>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingFechasCompensacion" runat="server" StoreID="StoreFicheros" DisplayInfo="true"
                        DisplayMsg="Mostrando Fechas {0} - {1} de {2}" HideRefresh="true" PageSize="8" />
                </BottomBar>
                <View>
                    <ext:GroupingView ID="GroupingView1" runat="server" ForceFit="true" MarkDirty="false"
                        ShowGroupName="false" EnableNoGroups="true" HideGroupedColumn="true" />
                </View>
            </ext:GridPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
