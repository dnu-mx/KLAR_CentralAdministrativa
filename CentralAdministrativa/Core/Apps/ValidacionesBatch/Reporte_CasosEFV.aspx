<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Reporte_CasosEFV.aspx.cs"
    Inherits="ValidacionesBatch.Reporte_CasosEFV" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var template = '<span style="color:{0};text-decoration:underline;">{1}</span>';

        var link = function (value) {
            return String.format(template, (value != "") ? "blue" : "black", value);
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

        var submitValue = function (grid, hiddenFormat, format) {
            hiddenFormat.setValue(format);
            grid.submitData(false);
        };
    </script>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
    <ext:Window ID="WdwDetalleTarjeta" runat="server" Width="900" Height="500" Resizable="False"
        Hidden="true" Closable="true" Modal="true" Layout="FitLayout" Draggable="true" Padding="5">
        <Items>
            <ext:FormPanel ID="FormPanel2" runat="server" Layout="FitLayout">
                <%--<TopBar>
                    <ext:Toolbar runat="server" Flat="false">
                        <Items>
                            <ext:Toolbar runat="server" Flat="true" Width="5" />
                            <ext:Label ID="lblEstatusTarjeta" runat="server" Text="Tarjeta " />
                            <ext:ToolbarSeparator />
                            <ext:ToolbarFill ID="dummy" runat="server" />
                            <ext:ToolbarSeparator />
                        </Items>
                    </ext:Toolbar>
                </TopBar>--%>
                <Items>
                    <ext:FormPanel ID="FormPanelIncidencias" runat="server" Title="Detalle de Incidencias" Border="false"
                        Layout="FitLayout">
                        <Items>
                            <ext:GridPanel ID="GridResultadosIncidencias" runat="server" Header="true" Layout="FitLayout">
                                <Store>
                                    <ext:Store ID="StoreResultadosIncidencias" runat="server" OnSubmitData="StoreSubmit">
                                        <DirectEventConfig IsUpload="true" />
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_OperacionRespuestaRegla">
                                                <Fields>
                                                    <ext:RecordField Name="ID_OperacionRespuestaRegla" />
                                                    <ext:RecordField Name="ID_Operacion" />
                                                    <ext:RecordField Name="FechaRegistro" />
                                                    <ext:RecordField Name="FechaOperacion" />
                                                    <ext:RecordField Name="DiaSemana" />
                                                    <ext:RecordField Name="Tarjeta" />
                                                    <ext:RecordField Name="Regla" />
                                                    <ext:RecordField Name="Accion" />
                                                    <ext:RecordField Name="Incidencias" />
                                                    <ext:RecordField Name="ReglasRotas" />
                                                    <ext:RecordField Name="Estatus" />
                                                    <ext:RecordField Name="Afiliacion" />
                                                    <ext:RecordField Name="Comercio" />
                                                    <ext:RecordField Name="Pais" />
                                                    <ext:RecordField Name="Narrativa1" />
                                                    <ext:RecordField Name="Narrativa2" />
                                                    <ext:RecordField Name="Narrativa3" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                        <DirectEventConfig IsUpload="true" />
                                    </ext:Store>
                                </Store>
                                <ColumnModel ID="ColumnModel3" runat="server">
                                    <Columns>
                                        <ext:DateColumn DataIndex="FechaRegistro" Header="Fecha de Registro"
                                            Format="dd-MM-yyyy HH:mm:ss" Width="120" />
                                        <ext:Column DataIndex="FechaOperacion" Header="Fecha de la Operación" Width="120"
                                            Align="Center" />
                                        <ext:Column DataIndex="DiaSemana" Header="Día Semana" Width="75" Align="Center" />
                                        <ext:Column DataIndex="Regla" Header="Regla" Width="150" />
                                        <ext:Column DataIndex="Accion" Header="Acción" Width="60" />
                                        <ext:Column DataIndex="Incidencias" Header="Incidencias" Width="70" Align="Center" />
                                        <ext:Column DataIndex="ReglasRotas" Header="Reglas Rotas" Width="80" Align="Center" />
                                        <ext:Column DataIndex="Estatus" Header="Estatus" Width="90" />
                                        <ext:Column DataIndex="Afiliacion" Header="Afiliación" Width="105" />
                                        <ext:Column DataIndex="Comercio" Header="Comercio" Width="140" />
                                        <ext:Column DataIndex="Pais" Header="País" Width="100" />
                                        <ext:Column DataIndex="Narrativa1" Header="Detalle 1" Width="100" />
                                        <ext:Column DataIndex="Narrativa2" Header="Detalle 2" Width="100" />
                                        <ext:Column DataIndex="Narrativa3" Header="Detalle 3" Width="100" />
                                    </Columns>
                                </ColumnModel>
                                <Plugins>
                                    <ext:GridFilters runat="server" ID="GridFilters3" Local="true">
                                        <Filters>
                                            <ext:StringFilter DataIndex="Fecha" />
                                        </Filters>
                                    </ext:GridFilters>
                                </Plugins>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar3" runat="server" StoreID="StoreResultadosIncidencias" DisplayInfo="true"
                                        DisplayMsg="Mostrando Incidencias {0} - {1} de {2}" HideRefresh="true" />
                                </BottomBar>
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar3" runat="server">
                                        <Items>
                                            <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                                            <ext:Button ID="btnExportExcelInc" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="true">
                                                <DirectEvents>
                                                    <Click OnEvent="ExportGridToExcel" IsUpload="true" Success="Ext.Net.DirectMethods.Download({IsUpload:true})">
                                                        <ExtraParams>
                                                            <ext:Parameter Name="GridResult" Value="Ext.encode(#{GridResultadosIncidencias}.getRowsValues({selectedOnly : false}))" Mode="Raw" />
                                                            <ext:Parameter Name="Reporte" Value="I" Mode="Value" />
                                                        </ExtraParams>
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="btnExportCSVInc" runat="server" Text="Exportar a CSV" Icon="PageAttach" Disabled="true">
                                                <Listeners>
                                                    <Click Handler="submitValue(#{GridResultadosIncidencias}, #{FormatType}, 'csv');" />
                                                </Listeners>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                            </ext:GridPanel>
                        </Items>
                    </ext:FormPanel>
                </Items>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true" Collapsible="true">
            <ext:FormPanel ID="FormPanelFiltros" Width="300" Title="Selecciona los Filtros" runat="server"
                Border="false" Layout="FitLayout">
                <Content>
                    <ext:Hidden ID="FormatType" runat="server" />
                </Content>
                <Items>
                    <ext:Panel ID="Panel1" runat="server" Border="false" Padding="10">
                        <Items>
                            <ext:TextField ID="txtTarjeta" runat="server" FieldLabel="No. Tarjeta" Width="280" MaxLengthText="16"
                                MaskRe="[0-9]" />
                            <ext:DateField ID="dfFechaInicio" runat="server" Vtype="daterange" FieldLabel="Desde" AllowBlank="false"
                                MsgTarget="Side" Format="dd/MM/yyyy" Width="280" EnableKeyEvents="true" MaxWidth="280">
                                <CustomConfig>
                                    <ext:ConfigItem Name="endDateField" Value="#{dfFechaFin}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:DateField ID="dfFechaFin" runat="server" Vtype="daterange" FieldLabel="Hasta" AllowBlank="false"
                                MaxWidth="280" Width="280" MsgTarget="Side" Format="dd/MM/yyyy" EnableKeyEvents="true">
                                <CustomConfig>
                                    <ext:ConfigItem Name="startDateField" Value="#{dfFechaInicio}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:ComboBox ID="cBoxDictamen" runat="server" FieldLabel="Dictamen" Width="280"
                                DisplayField="Descripcion" ValueField="ID_DictamenCaso">
                                <Store>
                                    <ext:Store ID="StoreDictamen" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_DictamenCaso">
                                                <Fields>
                                                    <ext:RecordField Name="ID_DictamenCaso" />
                                                    <ext:RecordField Name="ClaveDictamenCaso" />
                                                    <ext:RecordField Name="Descripcion" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cBoxTipoFraude" runat="server" FieldLabel="Tipo de Fraude" Width="280"
                                DisplayField="Descripcion" ValueField="ID_TipoFraude" ListWidth="350">
                                <Store>
                                    <ext:Store ID="StoreTipoFraude" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_TipoFraude">
                                                <Fields>
                                                    <ext:RecordField Name="ID_TipoFraude" />
                                                    <ext:RecordField Name="ClaveTipoFraude" />
                                                    <ext:RecordField Name="Descripcion" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                            </ext:ComboBox>
                        </Items>
                    </ext:Panel>
                </Items>
                <FooterBar>
                    <ext:Toolbar ID="Toolbar2" runat="server">
                        <Items>
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                 <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click">
                                        <EventMask ShowMask="true" Msg="Obteniendo Casos..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>
            </ext:FormPanel>
        </West>
        <Center Split="true" Collapsible="false">
            <ext:FormPanel ID="FormPanelCasos" runat="server" Layout="FitLayout" Title="Casos obtenidos con los Filtros">
                <Items>
                    <ext:GridPanel ID="GridCasos" runat="server" StripeRows="true" Layout="FitLayout" Region="Center">
                        <Store>
                            <ext:Store ID="StoreCasos" runat="server"  OnRefreshData="btnBuscar_Click"
                                OnSubmitData="StoreSubmit">
                                <DirectEventConfig IsUpload="true"/>
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_Caso">
                                        <Fields>
                                            <ext:RecordField Name="ID_Caso" />
                                            <ext:RecordField Name="Tarjeta" />
                                            <ext:RecordField Name="EstatusTarjeta" />
                                            <ext:RecordField Name="FechaCreacion" />
                                            <ext:RecordField Name="FechaCierre" />
                                            <ext:RecordField Name="Dictamen" />
                                            <ext:RecordField Name="TipoFraude" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <ColumnModel ID="ColumnModel1" runat="server">
                            <Columns>
                                <ext:Column ColumnID="ID_Caso" Header="No. Caso" DataIndex="ID_Caso" Width="60" Sortable="true">
                                    <Renderer Fn="link" />
                                </ext:Column>
                                <ext:Column ColumnID="Tarjeta" Header="No. Tarjeta" Sortable="true" DataIndex="Tarjeta" Width="120" />
                                <ext:Column ColumnID="EstatusTarjeta" Header="Estatus Tarjeta" Sortable="true" DataIndex="EstatusTarjeta"
                                    Width="100" />
                                <ext:DateColumn ColumnID="FechaCreacion" Header="Fecha de Creación" Sortable="true"
                                    DataIndex="FechaCreacion" Format="dd/MM/yyyy" Width="110"/>
                                <ext:DateColumn ColumnID="FechaCierre" Header="Fecha de Cierre" Sortable="true"
                                    DataIndex="FechaCierre" Format="dd/MM/yyyy" Width="100"/>
                                <ext:Column ColumnID="Dictamen" Header="Dictamen" Sortable="true" DataIndex="Dictamen" Width="80" />
                                <ext:Column ColumnID="TipoFraude" Header="Tipo de Fraude" Sortable="true" DataIndex="TipoFraude" Width="150" />
                            </Columns>
                        </ColumnModel>
                        <SelectionModel>
                            <ext:CellSelectionModel runat="server">
                                <DirectEvents>
                                    <CellSelect OnEvent="CellGridCasos_Click" />
                                </DirectEvents>
                            </ext:CellSelectionModel>
                        </SelectionModel>
                        <TopBar>
                            <ext:Toolbar ID="Toolbar5" runat="server">
                                <Items>
                                    <ext:ToolbarFill ID="ToolbarFill6" runat="server" />
                                    <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="true">
                                        <DirectEvents>
                                            <Click OnEvent="ExportGridToExcel" IsUpload="true" Success="Ext.Net.DirectMethods.Download({IsUpload:true})"
                                                After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                    e.stopEvent(); 
                                                    ValidacionesBatch.StopMask();">
                                                <ExtraParams>
                                                    <ext:Parameter Name="GridResult" Value="Ext.encode(#{GridCasos}.getRowsValues({selectedOnly : false}))" Mode="Raw" />
                                                    <ext:Parameter Name="Reporte" Value="C" Mode="Value" />
                                                </ExtraParams>
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnExportCSV" runat="server" Text="Exportar a CSV" Icon="PageAttach" Disabled="true">
                                        <Listeners>
                                            <Click Handler="submitValue(#{GridCasos}, #{FormatType}, 'csv');"  />
                                        </Listeners>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                        <BottomBar>
                            <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreCasos" DisplayInfo="true"
                                DisplayMsg="Mostrando Casos {0} - {1} de {2}" />
                        </BottomBar>
                    </ext:GridPanel>
                </Items>
            </ext:FormPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
