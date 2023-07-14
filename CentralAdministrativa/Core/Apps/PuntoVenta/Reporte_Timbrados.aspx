<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Reporte_Timbrados.aspx.cs" Inherits="TpvWeb.Reporte_Timbrados" %>


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
        <Center Split="true" Collapsible="false">
            <ext:GridPanel ID="GridPanelTimbrados" runat="server" StripeRows="true" Header="false" Border="false">
                <LoadMask ShowMask="false" />
                <Store>
                    <ext:Store ID="StoreTimbrados" runat="server" RemoteSort="true" OnRefreshData="StoreTimbrados_RefreshData"
                        AutoLoad="false">
                        <AutoLoadParams>
                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                        </AutoLoadParams>
                        <Proxy>
                            <ext:PageProxy />
                        </Proxy>
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="ID_XML">
                                <Fields>
                                    <ext:RecordField Name="ID_XML" />
                                    <ext:RecordField Name="XML" />
                                    <ext:RecordField Name="RFC" />
                                    <ext:RecordField Name="Receptor" />
                                    <ext:RecordField Name="ClaveMA" />
                                    <ext:RecordField Name="Tipo" />
                                    <ext:RecordField Name="Serie" />
                                    <ext:RecordField Name="Folio" />
                                    <ext:RecordField Name="Fecha" />
                                    <ext:RecordField Name="SubTotal" />
                                    <ext:RecordField Name="Descuento" />
                                    <ext:RecordField Name="TotalImpuestoTrasladado" />
                                    <ext:RecordField Name="NombreImpuesto" />
                                    <ext:RecordField Name="Total" />
                                    <ext:RecordField Name="UUID" />
                                    <ext:RecordField Name="Moneda" />
                                    <ext:RecordField Name="TipoCambio" />
                                    <ext:RecordField Name="Version" />
                                    <ext:RecordField Name="Version" />
                                    <ext:RecordField Name="Estado" />
                                    <ext:RecordField Name="Concepto" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <TopBar>
                    <ext:Toolbar runat="server" LabelWidth="70" LabelAlign="Right">
                        <Items>
                            <ext:DateField ID="dfFI_Timbrados" runat="server" Vtype="daterange" Width="180" Editable="false"
                                FieldLabel="Fecha Inicial" Format="yyyy/MM/dd" EnableKeyEvents="true" MaxDate="<%# DateTime.Now %>"
                                AutoDataBind="true" AllowBlank="false" InvalidText="Fecha inválida. Debe tener el formato AAAA/MM/DD">
                                <CustomConfig>
                                    <ext:ConfigItem Name="endDateField" Value="#{dfFF_Timbrados}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:DateField ID="dfFF_Timbrados" runat="server" Vtype="daterange" Width="180" Editable="false"
                                FieldLabel="Fecha Final" Format="yyyy/MM/dd" EnableKeyEvents="true" MaxDate="<%# DateTime.Now %>"
                                AutoDataBind="true" AllowBlank="false" InvalidText="Fecha inválida. Debe tener el formato AAAA/MM/DD">
                                <CustomConfig>
                                    <ext:ConfigItem Name="startDateField" Value="#{dfFI_Timbrados}" Mode="Value" />
                                </CustomConfig>
                                 <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:Button ID="btnBuscarTimbradosHide" runat="server" Hidden="true">
                                <Listeners>
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Timbrados...' });
                                        #{GridPanelTimbrados}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnBuscarTimbrados" runat="server" Text="Buscar..." Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscarTimbrados_Click" Timeout="360000"
                                        Before="if (!#{dfFI_Timbrados}.getValue() || !#{dfFF_Timbrados}.getValue())
                                        { return false; } else { resetToolbar(#{PagingTimbrados});
                                        #{GridPanelTimbrados}.getStore().sortInfo = null; }">
                                         <EventMask ShowMask="true" Msg="Buscando Timbrados..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnDownTimbHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                            RepTimbrados.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarSeparator runat="server" />
                            <ext:Button ID="btnLimpiarTimbrados" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiarTimbrados_Click" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarSeparator runat="server" />
                            <ext:ToolbarFill runat="server" />
                            <ext:Button ID="btnExcelTimbrados" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando a Excel...' });
                                            e.stopEvent(); 
                                            RepTimbrados.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <ColumnModel runat="server">
                    <Columns>
                        <ext:Column Header="ID" DataIndex="ID_XML" Width="50" />
                        <ext:Column Header="XML" DataIndex="XML" Width="250" />
                        <ext:Column Header="RFC Receptor" DataIndex="RFC" />
                        <ext:Column Header="Nombre Receptor" DataIndex="Receptor" Width="200" />
                        <ext:Column Header="Número de Tarjeta" DataIndex="ClaveMA" />
                        <ext:Column Header="Tipo" DataIndex="Tipo" Width="70" />
                        <ext:Column Header="Serie" DataIndex="Serie" Width="70" />
                        <ext:Column Header="Folio" DataIndex="Folio" Width="200" />
                        <ext:Column Header="Fecha" DataIndex="Fecha" Width="150" />
                        <ext:Column Header="Subtotal" DataIndex="SubTotal">
                            <Renderer Format="UsMoney" />
                        </ext:Column>
                        <ext:Column Header="Descuento" DataIndex="Descuento" />
                        <ext:Column Header="Total Impuesto Trasladado" DataIndex="TotalImpuestoTrasladado" Width="150">
                            <Renderer Format="UsMoney" />
                        </ext:Column>
                        <ext:Column Header="Nombre del Impuesto" DataIndex="NombreImpuesto" Width="150" />
                        <ext:Column Header="Total" DataIndex="Total">
                            <Renderer Format="UsMoney" />
                        </ext:Column>
                        <ext:Column Header="UUID" DataIndex="UUID" Width="250" />
                        <ext:Column Header="Moneda" DataIndex="Moneda" Width="70" />
                        <ext:Column Header="Tipo de Cambio" DataIndex="TipoCambio" />
                        <ext:Column Header="Versión" DataIndex="Version" Width="70" />
                        <ext:Column Header="Estado" DataIndex="Estado" />
                        <ext:Column Header="Concepto(s)" DataIndex="Concepto" Width="300" />
                    </Columns>
                </ColumnModel>
                <SelectionModel>
                    <ext:RowSelectionModel SingleSelect="true" />
                </SelectionModel>
                <Plugins>
                            <ext:GridFilters runat="server" ID="GridFiltersTimbrados" Local="true" FiltersText="Filtros">
                                <Filters>
                                    <ext:NumericFilter DataIndex="ID_XML" />
                                    <ext:StringFilter DataIndex="RFC" />
                                    <ext:StringFilter DataIndex="Receptor" />
                                    <ext:StringFilter DataIndex="Folio" />
                                    <ext:StringFilter DataIndex="Fecha" />
                                    <ext:StringFilter DataIndex="Estado" />
                                    <ext:StringFilter DataIndex="Concepto" />
                                </Filters>
                            </ext:GridFilters>
                        </Plugins>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingTimbrados" runat="server" StoreID="StoreTimbrados" DisplayInfo="true"
                        DisplayMsg="Mostrando Timbrados {0} - {1} de {2}" HideRefresh="true" />
                </BottomBar>
            </ext:GridPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>