<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="DevolucionesDecisionEmisor.aspx.cs" Inherits="TpvWeb.DevolucionesDecisionEmisor" %>

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

        var confirmaCambio = function (grid) {
            var title = 'Confirmación';
            var msg = '¿Estás seguro de modificar el estatus de las operaciones seleccionadas?';

            if (grid.getSelectionModel().selections.length == 0) {
                Ext.Msg.alert('Cambio de estatus', 'Por favor, selecciona al menos una operación.');
                return false;
            }

            Ext.Msg.confirm(title, msg, function (btn) {
                if (btn == 'yes') {
                    Ext.net.Mask.show({ msg : 'Procesando...' });
                    OpsNoVencidas.CambiaEstatusAOperaciones();
                    return true;
                } else {
                    return false;
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true" Collapsible="true">
            <ext:FormPanel ID="FormPanelDevsDecEmisor" Width="350" Title="Selecciona los Filtros" runat="server" Padding="10"
                Border="false" Layout="FormLayout" LabelWidth="150">
                <Items>
                    <ext:TextField ID="txtTarjeta" runat="server" FieldLabel="Tarjeta <span style='color:red;'>*   </span>"
                        AllowBlank="false" Width="180" MaskRe="[0-9]" MinLength="16" MaxLength="16" />
                    <ext:ComboBox ID="cBoxEstatusPostOp" FieldLabel="Estatus Post Operación <span style='color:red;'>*   </span>"
                        Width="180" runat="server" DisplayField="Descripcion" ListWidth="200" 
                        ValueField="ID_EstatusPostOperacion" AllowBlank="false">
                        <Store>
                            <ext:Store ID="StoreEstatusPostOp" runat="server">
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_EstatusPostOperacion">
                                        <Fields>
                                            <ext:RecordField Name="ID_EstatusPostOperacion" />
                                            <ext:RecordField Name="Clave" />
                                            <ext:RecordField Name="Descripcion" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                    </ext:ComboBox>
                    <ext:DateField ID="dfFechaInicio" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial Operación"
                        Format="yyyy/MM/dd" Width="180" Editable="false">
                        <CustomConfig>
                            <ext:ConfigItem Name="endDateField" Value="#{dfFechaFin}" Mode="Value" />
                        </CustomConfig>
                        <Listeners>
                            <KeyUp Fn="onKeyUp" />
                        </Listeners>
                    </ext:DateField>
                    <ext:DateField ID="dfFechaFin" runat="server" Vtype="daterange" FieldLabel="Fecha Final Operación"
                        Width="180" Format="yyyy/MM/dd" Editable="false">
                        <CustomConfig>
                            <ext:ConfigItem Name="startDateField" Value="#{dfFechaInicio}" Mode="Value" />
                        </CustomConfig>
                        <Listeners>
                            <KeyUp Fn="onKeyUp" />
                        </Listeners>
                    </ext:DateField>
                    <ext:Panel runat="server" Layout="FitLayout" Height="20" Border="false" />
                    <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false" LabelWidth="5">
                        <Defaults>
                            <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                        </Defaults>
                        <LayoutConfig>
                            <ext:HBoxLayoutConfig Align="Top" />
                        </LayoutConfig>
                        <Items>
                            <ext:Label runat="server" LabelSeparator=" " Width="220" />
                            <ext:Hidden runat="server" Flex="1" Width="25" />
                            <ext:Label runat="server" FieldLabel="<span style='color:red;'>*   </span>"
                                Text="Obligatorios" LabelSeparator=" " StyleSpec="font-style: italic;font-family:segoe ui;font-size: 11px;" />
                        </Items>
                    </ext:Panel>
                </Items>
                <FooterBar>
                    <ext:Toolbar runat="server">
                        <Items>
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnBuscarHide" runat="server" Hidden="true">
                                <Listeners>
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Operaciones...' });
                                        #{GridDevsDecEmisor}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                        Before="if (!#{FormPanelDevsDecEmisor}.getForm().isValid()) { return false; }
                                        else { resetToolbar(#{PagingDevsDecEmisor});
                                        #{GridDevsDecEmisor}.getStore().sortInfo = null; }">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                            OpsNoVencidas.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>
            </ext:FormPanel>
        </West>
        <Center Split="true" Collapsible="false">
            <ext:FormPanel ID="FormPanelOpsVenc" runat="server" Layout="FitLayout" Title="Operaciones obtenidas con los filtros seleccionados">
                <Items>
                    <ext:GridPanel ID="GridDevsDecEmisor" runat="server" StripeRows="true" Header="false" Border="false" AutoExpandColumn="Tarjeta">
                        <LoadMask ShowMask="false" />
                        <Store>
                            <ext:Store ID="StoreOpsNoVencidas" runat="server" RemoteSort="true" OnRefreshData="StoreOpsNoVencidas_RefreshData"
                                AutoLoad="false">
                                <AutoLoadParams>
                                    <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                </AutoLoadParams>
                                <Proxy>
                                    <ext:PageProxy />
                                </Proxy>                                
                                <DirectEventConfig IsUpload="true"/>
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_Operacion">
                                        <Fields>
                                            <ext:RecordField Name="ID_Operacion" />
                                            <ext:RecordField Name="Tarjeta" />
                                            <ext:RecordField Name="FechaOperacion" />
                                            <ext:RecordField Name="EstatusPostOperacion" />
                                            <ext:RecordField Name="Importe" />
                                            <ext:RecordField Name="Autorizacion" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <ColumnModel ID="ColumnModel1" runat="server">
                            <Columns>
                                <ext:Column Hidden="true" DataIndex="ID_Operacion" />
                                <ext:Column Header="Tarjeta" Sortable="true" DataIndex="Tarjeta">
                                    <Editor>
                                        <ext:TextField runat="server" />
                                    </Editor>
                                </ext:Column>
                                <ext:DateColumn Header="Fecha Operación" Sortable="true" DataIndex="FechaOperacion"
                                    Format="yyyy-MM-dd HH:mm:ss" Width="120"/>
                                <ext:Column Header="Estatus Post Operación" Sortable="true" DataIndex="EstatusPostOperacion"
                                    Width="150"/>
                                <ext:Column Header="Importe" Sortable="true" DataIndex="Importe">
                                    <Renderer Format="UsMoney" />
                                </ext:Column>
                                <ext:Column Header="Autorización" DataIndex="Autorizacion" />
                            </Columns>
                        </ColumnModel>
                        <SelectionModel>
                            <ext:CheckboxSelectionModel runat="server" />
                        </SelectionModel>
                        <Plugins>
                            <ext:GridFilters runat="server" ID="GridFilters1" Local="true" FiltersText="Filtros">
                                <Filters>
                                    <ext:NumericFilter DataIndex="Tarjeta" />
                                    <ext:DateFilter DataIndex="FechaOperacion" BeforeText="Antes de" OnText="El día"
                                        AfterText="Después de">
                                        <DatePickerOptions runat="server" TodayText="Hoy" />
                                    </ext:DateFilter>
                                    <ext:StringFilter DataIndex="EstatusPostOperacion" />
                                    <ext:NumericFilter DataIndex="Importe" />
                                    <ext:NumericFilter DataIndex="Autorizacion" />
                                </Filters>
                            </ext:GridFilters>
                        </Plugins>
                        <BottomBar>
                            <ext:PagingToolbar ID="PagingDevsDecEmisor" runat="server" StoreID="StoreOpsNoVencidas" DisplayInfo="true"
                                DisplayMsg="Mostrando Operaciones {0} - {1} de {2}" HideRefresh="true" />
                        </BottomBar>
                        <TopBar>
                            <ext:Toolbar ID="Toolbar2" runat="server">
                                <Items>
                                    <ext:Button runat="server" ID="btnCambiaEstatus" Icon="ArrowSwitchBluegreen" Text="Cambiar Estatus">
                                        <Listeners>
                                            <Click Handler="return confirmaCambio(#{GridDevsDecEmisor});" />
                                        </Listeners>
                                    </ext:Button>
                                    <ext:ToolbarSeparator runat="server" />
                                    <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                                    <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="true">
                                        <DirectEvents>
                                            <Click OnEvent="Download" IsUpload="true"
                                                After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                    e.stopEvent(); 
                                                    OpsNoVencidas.StopMask();" />
                                        </DirectEvents>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                    </ext:GridPanel>
                </Items>
            </ext:FormPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
