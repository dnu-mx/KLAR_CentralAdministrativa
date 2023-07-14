<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Manual.aspx.cs" Inherits="Facturas.Manual" %>

<%@ Import Namespace="System.Xml.Xsl" %>
<%@ Import Namespace="System.Xml" %>
<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
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
    <style type="text/css">
        .word-status .x-status-text {
            color: #777;
        }

        .word-status .x-status-busy {
            background: transparent url(images/saving.gif) no-repeat scroll 3px 3px !important;
            padding-left: 25px !important;
        }

        .word-status .x-status-saved {
            background: transparent url(/icons/accept-png/ext.axd) no-repeat scroll 3px 3px !important;
            padding-left: 25px !important;
        }
    </style>
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

        //var template = '<span style="color:{0};">{1}</span>';

        //var change = function (value) {
        //    return String.format(template, (value > 0) ? "green" : "red", value);
        //};

        //var pctChange = function (value) {
        //    return String.format(template, (value > 0) ? "green" : "red", value + "%");
        //};
    </script>
    <script>        function balance(value, meta, record, rowIndex, colIndex, store) {
            record.data.balance = record.data.Cantidad * record.data.PrecioUnitario;
            return Ext.util.Format.number(Math.abs(record.data.balance), '0.00');
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Window ID="frmImportFile" Title="Importar Productos desde Archivo" Icon="ApplicationFormAdd"
        runat="server" Width="460" Height="150" Resizable="False" Hidden="true" Closable="true"
        Modal="true" Layout="FitLayout" Draggable="true" Padding="12">
        <Items>
            <ext:FormPanel ID="frmNuevo" runat="server" Frame="true" Layout="FitLayout">
                <Items>
                    <ext:FileUploadField ID="FileSelect" LabelAlign="Top" runat="server" Icon="Attach"
                        Regex="\d+|.(csv|txt)$" RegexText="Selecciona Archivo (.csv) o (.txt)" />
                </Items>
                <BottomBar>
                    <ext:Toolbar ID="Toolbar5" runat="server">
                        <Items>
                            <ext:ToolbarFill ID="ToolbarFill3" runat="server" />
                            <ext:ToolbarSeparator ID="ToolbarSeparator5" runat="server" />
                            <ext:ToolbarTextItem ID="ToolbarTextItem1" runat="server" Text="Presiona el boton para importar los productos"
                                CtCls="x-status-text-panel" />
                            <ext:ToolbarSeparator ID="ToolbarSeparator4" runat="server" />
                            <ext:Button ID="btnUpload" LabelAlign="Top" runat="server" Text="Subir Archivo" Icon="Add">
                                <DirectEvents>
                                    <Click OnEvent="FileUploadField_FileSelected" IsUpload="true" Before=" 
                                                            Ext.Msg.wait('Obteniendo Productos para la Factura...', 'Procesando Archivo');"
                                        Success="Ext.Msg.hide(); Ext.Msg.show({ 
                                                            title   : 'Upload Exitoso', 
                                                            msg     : 'El Archivo se importó correctamente.', 
                                                            minWidth: 200, 
                                                            modal   : true, 
                                                            icon    : Ext.Msg.INFO, 
                                                            buttons : Ext.Msg.OK 
                                                        });"
                                        Failure="Ext.Msg.show({ 
                                                            title   : 'Error', 
                                                            msg     : 'Ocurrio un Error al Intenar subir el Archivo seleccionado al Servidor', 
                                                            minWidth: 200, 
                                                            modal   : true, 
                                                            icon    : Ext.Msg.ERROR, 
                                                            buttons : Ext.Msg.OK 
                                                        });">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </BottomBar>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true" Collapsible="true">
            <ext:FormPanel ID="FormPanel1" Width="500" Title="Nueva Factura Manual" runat="server"
                Border="false" AutoScroll="true" Layout="FitLayout">
                <LayoutConfig>
                    <ext:VBoxLayoutConfig Align="Stretch" Pack="Start" DefaultMargins="8" />
                </LayoutConfig>
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
                    <ext:Store ID="StoreEmisor" runat="server">
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
                    <ext:Store ID="StoreTipoDocto" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_TipoDocumento">
                                <Fields>
                                    <ext:RecordField Name="ID_TipoDocumento" />
                                    <ext:RecordField Name="Clave" />
                                    <ext:RecordField Name="Descripcion" />
                                    <ext:RecordField Name="Timbra" />
                                    <ext:RecordField Name="GeneraXML" />
                                    <ext:RecordField Name="CalculaIVA" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:Store ID="StoreReceptor" runat="server">
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
                    <ext:Store ID="StoreTipoColectiva" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_TipoColectiva">
                                <Fields>
                                    <ext:RecordField Name="ID_TipoColectiva" />
                                    <ext:RecordField Name="Clave" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:Store ID="StoreTipoColectiva2" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_TipoColectiva">
                                <Fields>
                                    <ext:RecordField Name="ID_TipoColectiva" />
                                    <ext:RecordField Name="Clave" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
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
                    </ext:Store>
                </Content>
                <Items>
                    <ext:Panel runat="server" Title="Factura" Padding="5" Layout="FormLayout" AutoHeight="true"
                        FormGroup="true" Flex="3">
                        <Items>
                            <ext:TextField ID="txtDescripcion" runat="server" FieldLabel="Descripción"
                                Width="365" AllowBlank="false" TabIndex="1" />
                            <ext:ComboBox ID="cmbTipoComprobante" FieldLabel="Tipo Documento" AllowBlank="false"
                                EmptyText="Selecciona una Opción" Resizable="true" Width="365" runat="server"
                                StoreID="StoreTipoDocto" DisplayField="Descripcion" ValueField="Clave">
                            </ext:ComboBox>
                        </Items>
                    </ext:Panel>
                    <ext:Panel runat="server" Title="Emisor" Padding="5" Layout="FormLayout" AutoHeight="true"
                        FormGroup="true" Flex="3">
                        <Items>
                            <ext:ComboBox ID="cmbTipoColectivaEmisor" runat="server" FieldLabel="Tipo Colectiva"
                                EmptyText="Selecciona una Opción" Width="365" StoreID="StoreTipoColectiva"
                                DisplayField="Descripcion" ValueField="Clave">
                                <DirectEvents>
                                    <Select OnEvent="consultaEmisores">
                                    </Select>
                                </DirectEvents>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cmbColectivaEmisor" runat="server" FieldLabel="Colectiva" Width="365"
                                EmptyText="Selecciona una Opción" StoreID="StoreEmisor" DisplayField="NombreORazonSocial"
                                ValueField="ID_Colectiva" Mode="Local" AutoSelect="true" Editable="true" ForceSelection="true"
                                TypeAhead="true" MinChars="1" MatchFieldWidth="false" Name="colEmisor">
                                <Listeners>
                                    <Select Handler="FacturaManual.PrestableceParametros(0, 1);" />
                                </Listeners>
                            </ext:ComboBox>
                        </Items>
                    </ext:Panel>
                    <ext:Panel runat="server" Title="Receptor" Padding="5" Layout="FormLayout" AutoHeight="true"
                        FormGroup="true" Flex="3">
                        <Items>
                            <ext:ComboBox ID="cmbtipoColectivaReceptor" runat="server" FieldLabel="Tipo Colectiva"
                                EmptyText="Selecciona una Opción" Width="365" StoreID="StoreTipoColectiva2"
                                DisplayField="Descripcion" ValueField="Clave">
                                <DirectEvents>
                                    <Select OnEvent="consultaReceptores">
                                    </Select>
                                </DirectEvents>
                            </ext:ComboBox>
                            <ext:Hidden runat="server" Width="15" />
                            <ext:ComboBox ID="cmbColectivaReceptor" runat="server" FieldLabel="Colectiva" EmptyText="Selecciona una Opción"
                                Width="365" StoreID="StoreReceptor" DisplayField="NombreORazonSocial" ValueField="ID_Colectiva"
                                Mode="Local" AutoSelect="true" Editable="true" ForceSelection="true" TypeAhead="true" MinChars="1"
                                MatchFieldWidth="false" Name="colReceptor">
                                <Listeners>
                                    <Select Handler="FacturaManual.PrestableceParametros(1, 0);" />
                                </Listeners>
                            </ext:ComboBox>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel8" Title="Parámetros" FormGroup="true" Padding="10" MinHeight="150"
                        runat="server" Frame="true" Border="true" Flex="10" Layout="FitLayout">
                        <Items>
                            <ext:PropertyGrid ID="GridPropiedades" runat="server" TabIndex="7" Layout="FitLayout"
                                Header="false">
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
                                            <ext:Button ID="Button5" runat="server" Text="Calcular Facturación" Icon="Add">
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </FooterBar>
                            </ext:PropertyGrid>
                        </Items>
                    </ext:Panel>
                </Items>
            </ext:FormPanel>
        </West>
        <Center Split="true" Collapsible="false">
            <ext:Panel ID="Panel2" runat="server" Title="Detalles de la Factura" Collapsed="false"
                Layout="FitLayout" AutoScroll="true">
                <Content>
                    <ext:Store ID="StoreDetalleFactura" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_DetalleFactura">
                                <Fields>
                                    <ext:RecordField Name="ID_Factura" />
                                    <ext:RecordField Name="Cantidad" />
                                    <ext:RecordField Name="Unidad" />
                                    <ext:RecordField Name="ID_Producto" />
                                    <ext:RecordField Name="NombreProducto" />
                                    <ext:RecordField Name="PrecioUnitario" />
                                    <ext:RecordField Name="ImporteIVA" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Content>
                <LayoutConfig>
                    <ext:VBoxLayoutConfig Align="Stretch" Pack="Start" DefaultMargins="2" />
                </LayoutConfig>
                <Items>
                    <ext:Panel ID="Panel5" runat="server" Padding="5" MinHeight="150" AnchorVertical="100%"
                        Layout="FitLayout" Flex="2">
                        <Items>
                            <ext:GridPanel ID="GridPanel2" runat="server" AnchorVertical="100%" StoreID="StoreDetalleFactura"
                                StripeRows="true" Header="false" Layout="FitLayout" Border="false">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel2" runat="server">
                                    <Columns>
                                        <ext:Column Header="ID" Width="40" DataIndex="Id" />
                                        <ext:Column ColumnID="Cantidad" Header="Cantidad" Sortable="true" DataIndex="Cantidad">
                                            <Editor>
                                                <ext:TextField ID="TextField1" runat="server" />
                                            </Editor>
                                        </ext:Column>
                                        <ext:Column ColumnID="ClaveProducto" Header="ClaveProducto" Sortable="true" DataIndex="NombreProducto">
                                            <Editor>
                                                <ext:ComboBox ID="cmbClaveProducto" ValueField="ID_ProductoFacturacion" DisplayField="DescripcionProducto"
                                                    runat="server" ListWidth="500">
                                                    <Store>
                                                        <ext:Store ID="StoreProductos" runat="server">
                                                            <Reader>
                                                                <ext:JsonReader IDProperty="ID_ProductoFacturacion">
                                                                    <Fields>
                                                                        <ext:RecordField Name="ID_ProductoFacturacion" />
                                                                        <ext:RecordField Name="SKU" />
                                                                        <ext:RecordField Name="NombreProducto" />
                                                                        <ext:RecordField Name="DescripcionProducto" />
                                                                        <ext:RecordField Name="PrecioUnitario" />
                                                                        <ext:RecordField Name="IVA" />
                                                                        <ext:RecordField Name="ClaveProdServ" />
                                                                        <ext:RecordField Name="ClaveImpuesto" />
                                                                        <ext:RecordField Name="Descripcion" />
                                                                        <ext:RecordField Name="IVA" />
                                                                        <ext:RecordField Name="ID_TipoFactor" />
                                                                        <ext:RecordField Name="ID_Impuesto" />
                                                                        <ext:RecordField Name="ID_UnidadProductoFacturacion" />
                                                                        <ext:RecordField Name="ClaveUnidad" />
                                                                        <ext:RecordField Name="Nombre" />
                                                                    </Fields>
                                                                </ext:JsonReader>
                                                            </Reader>
                                                        </ext:Store>
                                                    </Store>
                                                </ext:ComboBox>
                                            </Editor>
                                        </ext:Column>
                                        <ext:Column ColumnID="PrecioUnitario" Header="Precio Unitario" Align="Right" Sortable="true"
                                            DataIndex="PrecioUnitario">
                                            <Editor>
                                                <ext:TextField ID="TextField4" runat="server" />
                                            </Editor>
                                            <Renderer Format="UsMoney" />
                                        </ext:Column>
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" />
                                </SelectionModel>
                                <View>
                                    <ext:GridView ID="GridView1" runat="server" ForceFit="true" />
                                </View>
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar2" runat="server">
                                        <Items>
                                            <ext:Button ID="btnImportar" runat="server" Flat="false" Text="Importar Detalles desde archivo..."
                                                ToolTip="Agregar el detalle" Icon="FolderPage">
                                                <Listeners>
                                                    <Click Handler="#{frmImportFile}.setVisible(true);" />
                                                </Listeners>
                                            </ext:Button>
                                            <ext:Button ID="Button3" runat="server" Text="Nuevo Registro" Icon="Add">
                                                <Listeners>
                                                    <Click Handler="#{GridPanel2}.insertRecord();#{GridPanel2}.getRowEditor().startEditing(0);" />
                                                </Listeners>
                                            </ext:Button>
                                            <ext:Button ID="Button4" runat="server" Text="Eliminar Seleccionado" Icon="Exclamation">
                                                <Listeners>
                                                    <Click Handler="#{GridPanel2}.deleteSelected();" />
                                                </Listeners>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <Plugins>
                                    <ext:RowEditor ID="RowEditor1" runat="server" SaveText="Guardar" CancelText="Cancelar" />
                                </Plugins>
                            </ext:GridPanel>
                        </Items>
                        <BottomBar>
                            <ext:StatusBar ID="StatusBar4" CtCls="word-status" runat="server" DefaultText="Ready">
                                <Items>
                                    <ext:Button ID="Button2" runat="server" Flat="false" Text="Generar Factura" ToolTip="Agregar el detalle"
                                        Icon="Accept">
                                        <DirectEvents>
                                            <Click OnEvent="GenerarFactura" Before="var valid= #{FormPanel1}.getForm().isValid(); if (!valid) {} return valid;">
                                                <ExtraParams>
                                                    <ext:Parameter Name="TotalRegistros" Value="#{GridPanel2}.getStore().getTotalCount()"
                                                        Mode="Raw" />
                                                    <ext:Parameter Name="Values" Value="Ext.encode(#{GridPanel2}.getRowsValues({selectedOnly:false}))"
                                                        Mode="Raw" />
                                                </ExtraParams>
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                                    <ext:ToolbarTextItem ID="txtSubTotal" runat="server" Text="SubTotal: 0" CtCls="x-status-text-panel" />
                                    <ext:ToolbarSeparator ID="ToolbarSeparator1" runat="server" />
                                    <ext:ToolbarTextItem ID="txtIVA" runat="server" Text="IVA: 0" CtCls="x-status-text-panel" />
                                    <ext:ToolbarSeparator ID="ToolbarSeparator2" runat="server" />
                                    <ext:ToolbarTextItem ID="txtTotal" runat="server" Text="Total: 0 " CtCls="x-status-text-panel" />
                                    <ext:ToolbarSeparator ID="ToolbarSeparator3" runat="server" />
                                </Items>
                            </ext:StatusBar>
                        </BottomBar>
                    </ext:Panel>
                </Items>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
