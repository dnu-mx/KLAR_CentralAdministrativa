<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="MisFacturasTipo.aspx.cs" Inherits="Facturas.MisFacturasTipo" %>

<%@ Import Namespace="System.Xml.Xsl" %>
<%@ Import Namespace="System.Xml" %>
<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .cbStates-list
        {
            width: auto;
            font: 11px tahoma,arial,helvetica,sans-serif;
        }
        
        .cbStates-list th
        {
            font-weight: bold;
        }
        
        .cbStates-list td, .cbStates-list th
        {
            padding: 3px;
        }
    </style>
    <script type="text/javascript">
        // this "setGroupStyle" function is called when the GroupingView is refreshed.     
        var setGroupStyle = function (view) {
            // get an instance of the Groups
            var groups = view.getGroups();

            for (var i = 0; i < groups.length; i++) {
                var spans = Ext.query("span", groups[i]);

                if (spans && spans.length > 0) {
                    // Loop through the Groups, the do a query to find the <span> with our ColorCode
                    // Get the "id" from the <span> and split on the "-", the second array item should be our ColorCode
                    var color = "#" + spans[0].id.split("-")[1];

                    // Set the "background-color" of the original Group node.
                    Ext.get(groups[i]).setStyle("background-color", color);
                }
            }
        };
    </script>
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
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Hidden ID="hdnIdPeriodo" runat="server" />
    <ext:Window ID="winDetallesFactura" Title="Ver detalles de Factura" Icon="ApplicationFormAdd"
        runat="server" Width="460" Height="380" Resizable="False" Hidden="true" Closable="true"
        Modal="true" Layout="FitLayout" Draggable="true" Padding="12">
        <Content>
            <ext:Store ID="StoreDetalleFactura" runat="server">
                <Reader>
                    <ext:JsonReader IDProperty="ID_DetalleFactura">
                        <Fields>
                            <ext:RecordField Name="ID_Factura" />
                            <ext:RecordField Name="Cantidad" />
                            <ext:RecordField Name="Unidad" />
                            <ext:RecordField Name="ID_Producto" />
                            <ext:RecordField Name="PrecioUnitario" />
                            <ext:RecordField Name="Total" />
                            <ext:RecordField Name="ImporteIVA" />
                            <ext:RecordField Name="Descripcion" />
                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>
        </Content>
        <Items>
            <ext:GridPanel ID="GridPanel3" runat="server" AnchorVertical="50%" StoreID="StoreDetalleFactura"
                StripeRows="true" Header="false" Height="200" Border="false">
                <LoadMask ShowMask="false" />
                <ColumnModel ID="ColumnModel3" runat="server">
                    <Columns>
                        <ext:Column ColumnID="Cantidad" Header="Cantidad" Sortable="true" DataIndex="Cantidad" />
                        <ext:Column ColumnID="Unidad" Header="Unidad" Sortable="true" DataIndex="Unidad" />
                        <ext:Column ColumnID="Descripcion" Header="Descripcion" Sortable="true" DataIndex="Descripcion" />
                        <ext:Column ColumnID="PrecioUnitario" Header="Precio Unitario" Sortable="true" DataIndex="PrecioUnitario">
                            <Renderer Format="UsMoney" />
                        </ext:Column>
                        <ext:Column ColumnID="ImporteIVA" Header="IVA" Sortable="true" DataIndex="ImporteIVA">
                            <Renderer Format="UsMoney" />
                        </ext:Column>
                        <ext:Column ColumnID="Total" Header="Total" Sortable="true" DataIndex="Total">
                            <Renderer Format="UsMoney" />
                        </ext:Column>
                    </Columns>
                </ColumnModel>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="StoreDetalleFactura"
                        DisplayInfo="true" DisplayMsg="Mostrando Detalles de Factura{0} - {1} de {2}" />
                </BottomBar>
            </ext:GridPanel>
        </Items>
    </ext:Window>
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true" Collapsible="true">
            <ext:FormPanel ID="FormPanel1" Width="500" Title="Las Facturas Tipo" runat="server"
                Border="false" Layout="Fit">
                <Content>
                    <ext:Hidden ID="FormatType" runat="server" />
                    <ext:Store ID="StoreFacturasTipo" runat="server" OnRefreshData="RefreshGrid">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_FacturaTipo" >
                                <Fields>
                                    <ext:RecordField Name="ID_FacturaTipo" />
                                    <ext:RecordField Name="DescripcionTipoDocumento" />
                                    <ext:RecordField Name="Descripcion" />
                                    <ext:RecordField Name="ID_Emisor" />
                                    <ext:RecordField Name="Emisor" />
                                    <ext:RecordField Name="ID_Receptor" />
                                    <ext:RecordField Name="Receptor" />
                                    <ext:RecordField Name="ID_Contrato" />
                                    <ext:RecordField Name="TipoColectivaNivelDatos" />
                                    <ext:RecordField Name="ID_CadenaComercial" />
                                    <ext:RecordField Name="CadenaComercial" />
                                    <ext:RecordField Name="ID_Periodo" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="Descripcion" Direction="ASC" />
                    </ext:Store>
                    <ext:Store ID="StoreEmisor" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                                <Fields>
                                    <ext:RecordField Name="NombreORazonSocial" />
                                    <ext:RecordField Name="NameFin" />
                                    <ext:RecordField Name="ClaveColectiva" />
                                    <ext:RecordField Name="ID_Colectiva" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="NombreORazonSocial" Direction="ASC" />
                    </ext:Store>
                    <ext:Store ID="StoreReceptor" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                                <Fields>
                                    <ext:RecordField Name="NombreORazonSocial" />
                                    <ext:RecordField Name="NameFin" />
                                    <ext:RecordField Name="ClaveColectiva" />
                                    <ext:RecordField Name="ID_Colectiva" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="NombreORazonSocial" Direction="ASC" />
                    </ext:Store>
                    <ext:Store ID="StoreTipoColectiva" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_TipoColectiva">
                                <Fields>
                                    <ext:RecordField Name="ID_TipoColectiva" />
                                    <ext:RecordField Name="ClaveTipoColectiva" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <%--<SortInfo Field="Descripcion" Direction="ASC" />--%>
                    </ext:Store>
                    <ext:Store ID="Store2" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_TipoColectiva">
                                <Fields>
                                    <ext:RecordField Name="ID_TipoColectiva" />
                                    <ext:RecordField Name="ClaveTipoColectiva" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <%--<SortInfo Field="Descripcion" Direction="ASC" />--%>
                    </ext:Store>
                    <ext:Store ID="StoreTipoColectiva2" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_TipoColectiva">
                                <Fields>
                                    <ext:RecordField Name="ID_TipoColectiva" />
                                    <ext:RecordField Name="ClaveTipoColectiva" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <%--<SortInfo Field="Descripcion" Direction="ASC" />--%>
                    </ext:Store>
                    <ext:Store ID="StoreTipoPago" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_TipoPago">
                                <Fields>
                                    <ext:RecordField Name="ID_TipoPago" />
                                    <ext:RecordField Name="Clave" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <%--<SortInfo Field="Descripcion" Direction="ASC" />--%>
                    </ext:Store>
                    <ext:Store ID="stContratos" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Contrato">
                                <Fields>
                                    <ext:RecordField Name="ID_Contrato" />
                                    <ext:RecordField Name="Clave" />
                                    <ext:RecordField Name="Descripcion" />
                                    <ext:RecordField Name="CadenaComercial" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Content>
                <Items>
                    <ext:GridPanel ID="GridPanel1" runat="server" StoreID="StoreFacturasTipo" StripeRows="true"
                        Header="false" Height="200" Border="false">
                        <LoadMask ShowMask="false" />
                        <ColumnModel ID="ColumnModel1" runat="server">
                            <Columns>
                                <ext:Column ColumnID="DescripcionTipoDocumento" Header="Documento" Sortable="true"
                                    DataIndex="DescripcionTipoDocumento" />
                                <ext:Column ColumnID="Descripcion" Header="Descripcion" Width="200" Sortable="true"
                                    DataIndex="Descripcion" />
                                <ext:Column ColumnID="Emisor" Header="Emisor" Sortable="true" DataIndex="Emisor" />
                                <ext:Column ColumnID="Receptor" Header="Receptor" Sortable="true" DataIndex="Receptor" />
                                <ext:Column ColumnID="TipoColectivaNivelDatos" Header="TipoContrato" Sortable="true"
                                    DataIndex="TipoColectivaNivelDatos" />
                            </Columns>
                        </ColumnModel>
                        <SelectionModel>
                            <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                                <DirectEvents>
                                    <RowSelect OnEvent="RowSelect" Buffer="100">
                                        <EventMask ShowMask="true" Target="CustomTarget" CustomTarget="#{Panel2}" />
                                        <ExtraParams>
                                            <ext:Parameter Name="ID_FacturaTipo" Value="record.data['ID_FacturaTipo']" Mode="Raw" />
                                            <ext:Parameter Name="ID_CadenaComercial" Value="record.data['ID_CadenaComercial']"
                                                Mode="Raw" />
                                            <ext:Parameter Name="ID_Periodo" Value="record.data['ID_Periodo']" Mode="Raw" />
                                        </ExtraParams>
                                    </RowSelect>
                                </DirectEvents>
                            </ext:RowSelectionModel>
                        </SelectionModel>
                        <Plugins>
                            <ext:GridFilters runat="server" ID="GridFilters1" Local="true">
                                <Filters>
                                    <ext:StringFilter DataIndex="Descripcion" />
                                    <ext:StringFilter DataIndex="Emisor" />
                                    <ext:StringFilter DataIndex="Receptor" />
                                    <ext:StringFilter DataIndex="TipoColectivaNivelDatos" />
                                    <ext:StringFilter DataIndex="CadenaComercial" />
                                </Filters>
                            </ext:GridFilters>
                        </Plugins>
                        <BottomBar>
                            <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreFacturasTipo" DisplayInfo="true"
                                DisplayMsg="Mostrando FacturasTipo {0} - {1} de {2}" />
                        </BottomBar>
                    </ext:GridPanel>
                </Items>
            </ext:FormPanel>
        </West>
        <Center Split="true" Collapsible="false">
            <ext:Panel ID="Panel2" runat="server" Title="Generación de Factura" Collapsed="false"
                Layout="Fit" AutoScroll="true">
                <Content>
                    <ext:Store ID="Store1" runat="server" OnSubmitData="Store1_Submit" OnRefreshData="btnBuscar_Click"
                        RemoteSort="true">
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Operacion">
                                <%--<ext:ArrayReader IDProperty="ID_Operacion">--%>
                                <Fields>
                                    <ext:RecordField Name="Fecha" Type="Date" />
                                    <ext:RecordField Name="ID_Poliza" />
                                    <ext:RecordField Name="ID_Colectiva" />
                                    <ext:RecordField Name="NombreORazonSocial" />
                                    <ext:RecordField Name="ID_Cuenta" />
                                    <ext:RecordField Name="Cuenta" />
                                    <ext:RecordField Name="Cargo" />
                                    <ext:RecordField Name="Abono" />
                                    <ext:RecordField Name="Referencia" />
                                    <ext:RecordField Name="Observaciones" />
                                    <ext:RecordField Name="Concepto" />
                                </Fields>
                                <%--</ext:ArrayReader>--%>
                            </ext:JsonReader>
                        </Reader>
                        <DirectEventConfig IsUpload="true" />
                        <SortInfo Field="Fecha" />
                    </ext:Store>
                    <ext:Store ID="stEventosManuales" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Evento">
                                <Fields>
                                    <ext:RecordField Name="ID_Evento" />
                                    <ext:RecordField Name="Clave" />
                                    <ext:RecordField Name="Descripcion" />
                                    <ext:RecordField Name="DescMostrar" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center>
                            <ext:Panel ID="Panel13" runat="server" Layout="" Border="false" Flex="2">
                                <LayoutConfig>
                                    <ext:VBoxLayoutConfig Align="Stretch" DefaultMargins="2" />
                                </LayoutConfig>
                                <Items>
                                    <ext:Panel ID="Panel8" runat="server" Frame="true" Border="true" Flex="1" Layout="FitLayout">
                                        <Items>
                                            <ext:PropertyGrid ID="GridPropiedades" runat="server" Header="false">
                                                <Source>
                                                    <ext:PropertyGridParameter Name="(Los Parametros)" Value="Los Valores">
                                                    </ext:PropertyGridParameter>
                                                </Source>
                                                <DirectEvents>
                                                </DirectEvents>
                                                <View>
                                                    <ext:GridView ID="GridView2" ForceFit="true" ScrollOffset="2" runat="server" />
                                                </View>
                                                <FooterBar>
                                                    <ext:Toolbar ID="Toolbar3" Visible="false" runat="server" EnableOverflow="true">
                                                        <Items>
                                                            <ext:Button ID="Button1" runat="server" Text="Calcular Facturación" Icon="Add">
                                                                <DirectEvents>
                                                                    <Click OnEvent="GenerarFactura" />
                                                                </DirectEvents>
                                                            </ext:Button>
                                                            <%--<ext:Button ID="btnCancelar" runat="server" Text="Cancelar" Icon="Cancel">
                                                            </ext:Button>--%>
                                                        </Items>
                                                    </ext:Toolbar>
                                                </FooterBar>
                                            </ext:PropertyGrid>
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel ID="Panel10" runat="server" AutoScroll="true" Frame="true" Layout="FitLayout"
                                        Border="false" Flex="2">
                                        <Content>
                                            <ext:Store ID="StoreFactura" runat="server" OnSubmitData="Store1_Submit" OnRefreshData="btnBuscar_Click"
                                                RemoteSort="true">
                                                <DirectEventConfig IsUpload="true" />
                                                <Reader>
                                                    <ext:JsonReader IDProperty="ID_Factura">
                                                        <%--<ext:ArrayReader IDProperty="ID_Operacion">--%>
                                                        <Fields>
                                                            <ext:RecordField Name="FechaEmision" Type="Date" />
                                                            <ext:RecordField Name="Folio" />
                                                            <ext:RecordField Name="ID_Factura" />
                                                            <ext:RecordField Name="ID_ColectivaReceptora" />
                                                            <ext:RecordField Name="NombreReceptor" />
                                                            <ext:RecordField Name="EmailReceptor" />
                                                            <ext:RecordField Name="IVA" />
                                                            <ext:RecordField Name="SubTotal" />
                                                            <ext:RecordField Name="ImporteTotal" />
                                                            <ext:RecordField Name="FechaTimbrado" Type="Date" />
                                                            <ext:RecordField Name="DescripcionFactura" />
                                                            <ext:RecordField Name="ID_Emisor" />
                                                            <ext:RecordField Name="NombreEmisor" />
                                                            <ext:RecordField Name="ID_Estatus" />
                                                            <ext:RecordField Name="Descripcion" />
                                                            <ext:RecordField Name="Estatus" />
                                                             <ext:RecordField Name="TipoComprobante" />
                                                        </Fields>
                                                        <%--</ext:ArrayReader>--%>
                                                    </ext:JsonReader>
                                                </Reader>
                                                <DirectEventConfig IsUpload="true" />
                                                <SortInfo Field="FechaEmision" />
                                            </ext:Store>
                                        </Content>
                                        <Items>
                                            <ext:Panel ID="Panel1" runat="server" Layout="" Border="false" Flex="1">
                                                <LayoutConfig>
                                                    <ext:VBoxLayoutConfig Align="Stretch" DefaultMargins="0" />
                                                </LayoutConfig>
                                                <Items>
                                                    <ext:Panel ID="Panel3" runat="server" AutoScroll="true" Frame="false" Layout="FitLayout"
                                                        Border="false" Flex="2">
                                                        <Items>
                                                            <ext:GridPanel ID="GridPanel2" runat="server" StoreID="StoreFactura" StripeRows="true"
                                                                Header="false" Border="false">
                                                                <LoadMask ShowMask="false" />
                                                                <ColumnModel ID="ColumnModel2" runat="server">
                                                                    <Columns>
                                                                        <ext:CommandColumn ColumnID="acciones" Width="30" Header="">
                                                                            <Commands>
                                                                                <ext:GridCommand Icon="TextListBullets" CommandName="Detalles">
                                                                                    <ToolTip Text="Ver Detalles" />
                                                                                </ext:GridCommand>
                                                                            </Commands>
                                                                        </ext:CommandColumn>
                                                                        <ext:Column ColumnID="ID_Factura" Header="Identificador" Sortable="true" DataIndex="ID_Factura" />
                                                                         <ext:Column ColumnID="TipoComprobante" Header="Tipo Comprobante" Sortable="true" DataIndex="TipoComprobante" />
                                                                        <ext:Column ColumnID="Estatus" Header="Estatus" Sortable="true" DataIndex="Estatus" />
                                                                        <ext:DateColumn ColumnID="FechaEmision" Header="Fecha" Sortable="true" DataIndex="FechaEmision"
                                                                            Format="dd-MMM-yyyy" />
                                                                        <%--<ext:Column ColumnID="DescripcionFactura" Header="Factura" Sortable="true" DataIndex="DescripcionFactura" />--%>
                                                                        <ext:Column ColumnID="NombreEmisor" Header="Emisor" Sortable="true" DataIndex="NombreEmisor" />
                                                                        <ext:Column ColumnID="NombreReceptor" Header="Receptor" Sortable="true" DataIndex="NombreReceptor" />
                                                                        <ext:Column ColumnID="SubTotal" Header="SubTotal" Sortable="true" DataIndex="SubTotal">
                                                                            <Renderer Format="UsMoney" />
                                                                        </ext:Column>
                                                                        <ext:Column ColumnID="IVA" Header="IVA" Sortable="true" DataIndex="IVA">
                                                                            <Renderer Format="UsMoney" />
                                                                        </ext:Column>
                                                                        <ext:Column ColumnID="ImporteTotal" Header="Importe Total" Sortable="true" DataIndex="ImporteTotal">
                                                                            <Renderer Format="UsMoney" />
                                                                        </ext:Column>
                                                                    </Columns>
                                                                </ColumnModel>
                                                                <DirectEvents>
                                                                    <Command OnEvent="EjecutarComando">
                                                                        <ExtraParams>
                                                                            <ext:Parameter Name="ID" Value="record.data['ID_Factura']" Mode="Raw" />
                                                                             <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                                                        </ExtraParams>
                                                                    </Command>
                                                                </DirectEvents>
                                                            </ext:GridPanel>
                                                        </Items>
                                                    </ext:Panel>
                                                    <%--   <ext:Panel ID="Panel4" runat="server" AutoScroll="true" Frame="false" Layout="FormLayout"
                                                        Border="false" Flex="1">
                                                        <Items>
                                                            <ext:TextField ID="txtSubtotal" FieldLabel="SubTotal" runat="server" AnchorHorizontal="100%"
                                                                Text="" StyleSpec="text-align:right;" />
                                                            <ext:TextField ID="txtIVA" FieldLabel="IVA" runat="server" AnchorHorizontal="100%"
                                                                Text="" StyleSpec="text-align:right;" />
                                                            <ext:TextField ID="txtTotal" FieldLabel="Total" runat="server" AnchorHorizontal="100%"
                                                                Text="" StyleSpec="text-align:right;" />
                                                        </Items>
                                                    </ext:Panel>--%>
                                                </Items>
                                            </ext:Panel>
                                        </Items>
                                        <FooterBar>
                                            <ext:Toolbar ID="Toolbar2" runat="server" EnableOverflow="false">
                                                <Items>
                                                    <ext:Button ID="Button2" runat="server" Text="Generar Factura" Icon="Add">
                                                        <DirectEvents>
                                                            <Click OnEvent="GuardarFactura" />
                                                        </DirectEvents>
                                                    </ext:Button>
                                                    <%--<ext:Button ID="Button3" runat="server" Text="Cancelar" Icon="Cancel">
                                                            </ext:Button>--%>
                                                </Items>
                                            </ext:Toolbar>
                                        </FooterBar>
                                    </ext:Panel>
                                </Items>
                            </ext:Panel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
