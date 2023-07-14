<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="Reporte_FacturasPagos.aspx.cs" Inherits="Facturas.Reporte_FacturasPagos" %>

<%@ Import Namespace="System.Xml.Xsl" %>
<%@ Import Namespace="System.Xml" %>
<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">   
    <script type="text/javascript">
        var onKeyUpFact = function (field) {
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

        var onKeyUpPago = function (field) {
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
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true" Collapsible="true">
            <ext:FormPanel ID="FormPanel1" Width="300" Title="Selecciona los Filtros" runat="server"
                Border="false" Layout="Fit">
                <Content>
                    <ext:Hidden ID="FormatType" runat="server" />
                    <ext:Store ID="StoreCadenaComercial" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                                <Fields>
                                    <ext:RecordField Name="NombreORazonSocial" />
                                    <ext:RecordField Name="ClaveColectiva" />
                                    <ext:RecordField Name="ID_Colectiva" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="NombreORazonSocial" Direction="ASC" />
                    </ext:Store>
                    <ext:Store ID="StoreEstatusFactura" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Estatus">
                                <Fields>
                                    <ext:RecordField Name="ID_Estatus" />
                                    <ext:RecordField Name="Clave" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                     <ext:Store ID="StoreEstatusPago" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_EstatusPago">
                                <Fields>
                                    <ext:RecordField Name="ID_EstatusPago" />
                                    <ext:RecordField Name="Clave" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Content>
                <Items>
                    <ext:Panel ID="Panel1" runat="server" Layout="FitLayout" Padding="10">
                        <Items>
                            <ext:ComboBox ID="cmbCadenaComercial" runat="server" FieldLabel="Cadena Comercial" EmptyText="Todas"
                                ListWidth="250" Width="250" StoreID="StoreCadenaComercial" DisplayField="NombreORazonSocial" 
                                ValueField="ID_Colectiva" Mode="Local" AutoSelect="true" ForceSelection="true" TypeAhead="true"
                                MinChars="1" MatchFieldWidth="false" Name="colCCR_DE">
                                <Items>
                                    <ext:ListItem Text="( Todas )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:FieldSet ID="FieldSet1" runat="server" Title="Facturas" Layout="AnchorLayout"
                                DefaultAnchor="100%">
                                <Items>
                                    <ext:DateField ID="datInicioFactura" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                        AllowBlank="false" MsgTarget="Side" Format="yyyy-MM-dd" MaxLength="12"
                                        Width="250" EnableKeyEvents="true">
                                        <CustomConfig>
                                            <ext:ConfigItem Name="endDateField" Value="#{datFinFactura}" Mode="Value" />
                                        </CustomConfig>
                                        <Listeners>
                                            <KeyUp Fn="onKeyUpFact" />
                                        </Listeners>
                                    </ext:DateField>
                                    <ext:DateField ID="datFinFactura" runat="server" Vtype="daterange" FieldLabel="Fecha Final"
                                        AllowBlank="false" MaxLength="12" Width="250" MsgTarget="Side" Format="yyyy-MM-dd"
                                        EnableKeyEvents="true">
                                        <CustomConfig>
                                            <ext:ConfigItem Name="startDateField" Value="#{datInicioFactura}" Mode="Value" />
                                        </CustomConfig>
                                        <Listeners>
                                            <KeyUp Fn="onKeyUpFact" />
                                        </Listeners>
                                    </ext:DateField>
                                    <ext:ComboBox ID="cmbEstatusFactura" runat="server" FieldLabel="Estatus" EmptyText="Todos"
                                        ListWidth="250" Width="250" StoreID="StoreEstatusFactura" DisplayField="Descripcion"
                                        ValueField="ID_Estatus">
                                        <Items>
                                            <ext:ListItem Text="( Todos )" Value="-1" />
                                        </Items>
                                    </ext:ComboBox>
                                </Items>
                            </ext:FieldSet>
                            <ext:FieldSet ID="FieldSetAjustesManuales" runat="server" Title="Pagos" Layout="AnchorLayout"
                                DefaultAnchor="100%">
                                <Items>
                                    <ext:DateField ID="datInicioPago" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                        MsgTarget="Side" Format="yyyy-MM-dd" MaxLength="12" Width="250" EnableKeyEvents="true">
                                        <CustomConfig>
                                            <ext:ConfigItem Name="endDateField" Value="#{datFinaPago}" Mode="Value" />
                                        </CustomConfig>
                                        <Listeners>
                                            <KeyUp Fn="onKeyUpPago" />
                                        </Listeners>
                                    </ext:DateField>
                                    <ext:DateField ID="datFinPago" runat="server" Vtype="daterange" FieldLabel="Fecha Final"
                                        MaxLength="12" Width="250" MsgTarget="Side" Format="yyyy-MM-dd" EnableKeyEvents="true">
                                        <CustomConfig>
                                            <ext:ConfigItem Name="startDateField" Value="#{datInicioPago}" Mode="Value" />
                                        </CustomConfig>
                                        <Listeners>
                                            <KeyUp Fn="onKeyUpPago" />
                                        </Listeners>
                                    </ext:DateField>
                                    <ext:TextField ID="txtImporteInferior" FieldLabel="Monto Inferior" EmptyText="Todos"
                                        TabIndex="10" MaxLength="50" Width="250" runat="server" Text="" />
                                    <ext:TextField ID="txtImporteSuperior" FieldLabel="Monto Superior" EmptyText="Todos"
                                        TabIndex="10" MaxLength="50" Width="250" runat="server" Text="" />
                                    <ext:ComboBox ID="cmbEstatusPago" runat="server" FieldLabel="Estatus" EmptyText="Todos"
                                        ListWidth="250" Width="250" StoreID="StoreEstatusPago" DisplayField="Descripcion"
                                        ValueField="ID_EstatusPago">
                                        <Items>
                                            <ext:ListItem Text="( Todos )" Value="-1" />
                                        </Items>
                                    </ext:ComboBox>
                                </Items>
                            </ext:FieldSet>
                        </Items>
                    </ext:Panel>
                </Items>
                <FooterBar>
                    <ext:Toolbar ID="Toolbar1" runat="server">
                        <Items>
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Before="var valid= #{FormPanel1}.getForm().isValid(); if (!valid) {} return valid;">
                                        <EventMask ShowMask="true" Msg="Obteniendo Facturas y Pagos..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>
            </ext:FormPanel>
        </West>
        <Center Split="true" Collapsible="false">
            <ext:Panel ID="Panel2" runat="server" Title="Facturas y Pagos Obtenidos con los Filtros Seleccionados"
                Collapsed="false" Layout="Fit" AutoScroll="true">
                <Content>
                    <ext:Store ID="Store1" runat="server" OnSubmitData="Store1_Submit" OnRefreshData="btnBuscar_Click">
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="ID_PolizaDetalle">
                                <Fields>
                                    <ext:RecordField Name="ID_PolizaDetalle" />
                                    <ext:RecordField Name="ID_Poliza" />
                                    <ext:RecordField Name="ID_Factura" />
                                    <ext:RecordField Name="Folio" />
                                    <ext:RecordField Name="FechaFactura" Type="Date" />
                                    <ext:RecordField Name="FechaPago" Type="Date" />
                                    <ext:RecordField Name="ID_Colectiva" />
                                    <ext:RecordField Name="Receptor" />
                                    <ext:RecordField Name="Referencia" />
                                    <ext:RecordField Name="ImportePago" />
                                    <ext:RecordField Name="ImporteFactura" />
                                    <ext:RecordField Name="ImporteAplicado" />
                                    <ext:RecordField Name="ImporteFaltanteFactura" />
                                    <ext:RecordField Name="EstatusPago" />
                                    <ext:RecordField Name="EstatusFactura" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <DirectEventConfig IsUpload="true" />
                        <SortInfo Field="FechaFactura" />
                    </ext:Store>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanelReporte" runat="server" StoreID="Store1" StripeRows="true"
                                Header="false" Border="false">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                        <ext:Column ColumnID="Folio" Header="Factura Folio" Sortable="true" DataIndex="Folio" Width="80"/>
                                        <ext:Column ColumnID="ID_Poliza" Header="Póliza" Sortable="true" DataIndex="ID_Poliza" Width="60"/>
                                        <ext:DateColumn ColumnID="FechaFactura" Header="Fecha/Hora Factura" Sortable="true" DataIndex="FechaFactura"
                                            Format="yyyy-MM-dd HH:mm:ss" Width="120"/>
                                        <ext:Column ColumnID="EstatusFactura" Header="Estatus Factura" Sortable="true" DataIndex="EstatusFactura" />
                                        <ext:DateColumn ColumnID="FechaPago" Header="Fecha/Hora  Pago" Sortable="true" DataIndex="FechaPago"
                                            Format="yyyy-MM-dd HH:mm:ss" />
                                        <ext:Column ColumnID="EstatusPago" Header="Estatus Pago" Sortable="true" DataIndex="EstatusPago" />
                                        <ext:Column ColumnID="ID_Colectiva" Header="Cadena Comercial" Sortable="true" DataIndex="ID_Colectiva" />
                                        <ext:Column ColumnID="Receptor" Header="Receptor" Sortable="true" DataIndex="Receptor" />
                                        <ext:Column ColumnID="Referencia" Header="Referencia" Sortable="true" DataIndex="Referencia" Width="70" />
                                        <ext:Column ColumnID="ImporteFactura" Header="Importe Factura" Sortable="true" DataIndex="ImporteFactura"
                                            Width="90">
                                            <Renderer Format="UsMoney" />
                                        </ext:Column>
                                        <ext:Column ColumnID="ImportePago" Header="Importe Pago" Sortable="true" DataIndex="ImportePago"
                                            Width="80">
                                            <Renderer Format="UsMoney" />
                                        </ext:Column>
                                        <ext:Column ColumnID="ImporteAplicado" Header="Importe Aplicado" Sortable="true" DataIndex="ImporteAplicado"
                                            Width="90">
                                            <Renderer Format="UsMoney" />
                                        </ext:Column>
                                        <ext:Column ColumnID="ImporteFaltanteFactura" Header="Importe Faltante Factura" Sortable="true" DataIndex="ImporteFaltanteFactura">
                                            <Renderer Format="UsMoney" />
                                        </ext:Column>
                                    </Columns>
                                </ColumnModel>
                                <Plugins>
                                    <ext:GridFilters runat="server" ID="GridFilters1" Local="true">
                                        <Filters>
                                            <ext:NumericFilter DataIndex="ID_Poliza" />
                                            <ext:StringFilter DataIndex="Folio" />
                                            <ext:DateFilter DataIndex="FechaFactura">
                                                <DatePickerOptions runat="server" TodayText="Hoy" />
                                            </ext:DateFilter>
                                            <ext:StringFilter DataIndex="EstatusFactura" />
                                            <ext:DateFilter DataIndex="FechaPago">
                                                <DatePickerOptions runat="server" TodayText="Hoy" />
                                            </ext:DateFilter>
                                            <ext:StringFilter DataIndex="EstatusPago" />
                                            <ext:NumericFilter DataIndex="ID_Colectiva" />
                                            <ext:StringFilter DataIndex="Receptor" />
                                            <ext:StringFilter DataIndex="Referencia" />
                                            <ext:NumericFilter DataIndex="ImportePago" />
                                            <ext:NumericFilter DataIndex="ImporteFactura" />
                                            <ext:NumericFilter DataIndex="ImporteAplicado" />
                                            <ext:NumericFilter DataIndex="ImporteFaltanteFactura" />
                                        </Filters>
                                    </ext:GridFilters>
                                </Plugins>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="Store1" DisplayInfo="true"
                                        DisplayMsg="Mostrando Operaciones {0} - {1} de {2}" />
                                </BottomBar>
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar2" runat="server">
                                        <Items>
                                            <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                                            <ext:Button ID="Button2" runat="server" Text="Exportar a XML" Icon="PageCode">
                                                <Listeners>
                                                    <Click Handler="submitValue(#{GridPanelReporte}, #{FormatType}, 'xml');" />
                                                </Listeners>
                                            </ext:Button>
                                            <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" Icon="PageExcel">
                                                <DirectEvents>
                                                    <Click OnEvent="ExportGridToExcel" IsUpload="true" Success="Ext.Net.DirectMethods.Download({IsUpload:true})">
                                                        <ExtraParams>
                                                            <ext:Parameter Name="GridToExport" Value="Ext.encode(#{GridPanelReporte}.getRowsValues({selectedOnly : false}))" Mode="Raw" />
                                                        </ExtraParams>
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="Button4" runat="server" Text="Exportar a CSV" Icon="PageAttach">
                                                <Listeners>
                                                    <Click Handler="submitValue(#{GridPanelReporte}, #{FormatType}, 'csv');" />
                                                </Listeners>
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
    </ext:BorderLayout>
</asp:Content>

