<%@ Page Title="" Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master"
    CodeBehind="Rep_SaldoEnCuentasCadena.aspx.cs" Inherits="Empresarial.Rep_SaldoEnCuentasCadena" %>

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

        var template = '<span style="color:{0};">{1}</span>';

        var change = function (value) {
            return String.format(template, (value > 0) ? "green" : "red", value);
        };

        var pctChange = function (value) {
            return String.format(template, (value > 0) ? "green" : "red", value + "%");
        };

        var link_template = '<span style="color:{0};text-decoration:underline;">{1}</span>';

        var link = function (value) {
            return String.format(link_template, (value != "") ? "blue" : "black", value);
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center Split="true">
            <ext:Panel ID="pnlcuentas" runat="server" Width="850" Title="Saldos" Collapsed="false"
                Collapsible="false" Layout="Fit" AutoScroll="true">
                <Content>
                    <ext:Hidden ID="MainFormatType" runat="server" />
                    <ext:Hidden ID="FormatType" runat="server" />
                    <ext:Store ID="Store1" runat="server" OnRefreshData="RefreshGridSaldos" GroupField="CadenaComercial"
                        OnSubmitData="StoreSubmit">
                        <DirectEventConfig IsUpload="true"  />
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Cuenta">
                            </ext:JsonReader>
                        </Reader>
                        <DirectEventConfig IsUpload="true" />
                        <SortInfo  Field="ID_Cuenta"  />
                    </ext:Store>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store1" StripeRows="true"
                                Header="false" Border="false">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                    </Columns>
                                    <%--  <DirectEvents>
                                        <command onevent="EjecutarComando" isupload="true">
                                    </command>
                                    </DirectEvents>--%>
                                </ColumnModel>
                                <DirectEvents>
                                    <Command OnEvent="Seleccionar">
                                        <ExtraParams>
                                            <ext:Parameter Name="ID_Cuenta" Value="record.data.ID_Cuenta" Mode="Raw"/>
                                        </ExtraParams>
                                    </Command>
                                </DirectEvents>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel2" runat="server" SingleSelect="true">
                                        <DirectEvents>
                                            <RowDeselect OnEvent="QuitarSeleccion" IsUpload="true">
                                            </RowDeselect>
                                        </DirectEvents>
                                    </ext:RowSelectionModel>
                                </SelectionModel>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="Store1" DisplayInfo="true"
                                        DisplayMsg="Mostrando Saldos {0} - {1} de {2}" />
                                </BottomBar>
                                <View>
                                    <ext:GroupingView ID="GroupingView1" runat="server" ForceFit="true" MarkDirty="false"
                                        ShowGroupName="false" EnableNoGroups="true" HideGroupedColumn="true" />
                                </View>
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar2" runat="server">
                                        <Items>
                                            <%--<ext:Button ID="btnGetEdocta" runat="server" Text="Obtener PDF" ToolTip="Descargar reporte en PDF"
                                                Icon="PageWhiteAcrobat">
                                                <DirectEvents>
                                                    <Click OnEvent="GetEstadoCuenta" IsUpload="true">
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>--%>
                                            <%--<ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" ToolTip="Descargar Reporte en un Archivo Excel"
                                                Icon="PageExcel">
                                                <Listeners>
                                                    <Click Handler="submitValue(#{GridPanel1}, #{MainFormatType}, 'xls');" />
                                                </Listeners>
                                            </ext:Button>--%>
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
            <ext:Panel ID="pnlDetalles" runat="server" Width="550" Collapsed="true" Collapsible="true"
                Layout="FitLayout" >
                <Content>
                    <ext:BorderLayout ID="BorderLayout3" runat="server">
                        <Center Split="true">
                            <ext:Panel ID="Panel12" runat="server" Height="450" Width="450" Layout="FitLayout" LabelWidth="85"
                                LabelAlign="Right" Collapsed="false" Collapsible="false">
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar_1" runat="server">
                                        <Items>
                                            <ext:DateField ID="datInicio" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                                AllowBlank="false" MsgTarget="Side" Format="yyyy-MM-dd" MaxLength="12"
                                                Width="200" EnableKeyEvents="true">
                                                <CustomConfig>
                                                    <ext:ConfigItem Name="endDateField" Value="#{datFinal}" Mode="Value" />
                                                </CustomConfig>
                                                <Listeners>
                                                    <KeyUp Fn="onKeyUp" />
                                                </Listeners>
                                            </ext:DateField>
                                            <ext:DateField ID="datFinal" runat="server" Vtype="daterange" FieldLabel="Fecha Final"
                                                AllowBlank="false" MaxLength="12" Width="200" MsgTarget="Side" Format="yyyy-MM-dd"
                                                EnableKeyEvents="true">
                                                <CustomConfig>
                                                    <ext:ConfigItem Name="startDateField" Value="#{datInicio}" Mode="Value" />
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
                                                    <Click OnEvent="ObtenerDatos">
                                                        <EventMask ShowMask="true" Msg="Buscando Movimientos..." MinDelay="500" />
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <Items>
                                    <ext:GridPanel ID="GridDetalle" runat="server" Title="Cuentas" StripeRows="true" Header="false"
                                        Border="false" Layout="FitLayout">
                                        <Store>
                                            <ext:Store ID="Store2" runat="server" OnSubmitData="Store1_Submit" RemoteSort="true">
                                                <DirectEventConfig IsUpload="true" />
                                                <Reader>
                                                    <ext:JsonReader IDProperty="IDReporte">
                                                    </ext:JsonReader>
                                                </Reader>
                                                <DirectEventConfig IsUpload="true" />
                                                <SortInfo Field="IDReporte" />
                                            </ext:Store>
                                        </Store>
                                        <LoadMask ShowMask="false" />
                                        <ColumnModel ID="ColumnModel2" runat="server">
                                        </ColumnModel>
                                        <SelectionModel>
                                            <ext:CellSelectionModel runat="server">
                                                <DirectEvents>
                                                    <CellSelect OnEvent="Cell_Click" />
                                                </DirectEvents>
                                            </ext:CellSelectionModel>
                                        </SelectionModel>
                                        <TopBar>
                                            <ext:Toolbar runat="server">
                                                <Items>
                                                    <ext:ToolbarFill runat="server" />
                                                    <ext:ToolbarSeparator runat="server" />
                                                    <%--<ext:Button ID="Button1" runat="server" Text="Reporte PDF" ToolTip="Obtener Reporte en PDF"
                                                        Icon="PageWhiteAcrobat">
                                                        <DirectEvents>
                                                            <Click OnEvent="ExportarPDF" IsUpload="true">
                                                            </Click>
                                                        </DirectEvents>
                                                    </ext:Button>
                                                    <ext:ToolbarSeparator runat="server" />--%>
                                                    <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" Icon="PageExcel" ToolTip="Obtener Datos en un Archivo Excel">
                                                        <DirectEvents>
                                                            <Click OnEvent="ExportGridToExcel" IsUpload="true" Success="Ext.Net.DirectMethods.Download({IsUpload:true})">
                                                                <ExtraParams>
                                                                    <ext:Parameter Name="GridToExport" Value="Ext.encode(#{GridDetalle}.getRowsValues({selectedOnly : false}))" Mode="Raw" />
                                                                </ExtraParams>
                                                            </Click>
                                                        </DirectEvents>
                                                    </ext:Button>
                                                    <ext:ToolbarSeparator runat="server" />
                                                    <ext:Button ID="Button3" runat="server" Text="Exportar a CSV" ToolTip="Obtener Datos separados por comas"
                                                        Icon="PageAttach">
                                                        <Listeners>
                                                            <Click Handler="submitValue(#{GridDetalle}, #{FormatType}, 'csv');" />
                                                        </Listeners>
                                                    </ext:Button>
                                                </Items>
                                            </ext:Toolbar>
                                        </TopBar>
                                    </ext:GridPanel>
                                </Items>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="Store2" DisplayInfo="true"
                                        DisplayMsg="Mostrando Saldos {0} - {1} de {2}" />
                                </BottomBar>
                            </ext:Panel>
                        </Center>
                        <South Split="true">
                            <ext:Panel ID="PanelDetallePoliza" runat="server" Width="450" Height="150" Title="Detalle Póliza" Collapsed="true"
                                Collapsible="true" Layout="FitLayout">
                                <Items>
                                    <ext:GridPanel ID="GridDetallePoliza" runat="server" StripeRows="true" Header="false"
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
                                                    <Renderer Format="UsMoney" />
                                                </ext:Column>
                                                <ext:Column ColumnID="Abono" Header="Abono" DataIndex="Abono" Width="70">
                                                   <Renderer Format="UsMoney" />
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
