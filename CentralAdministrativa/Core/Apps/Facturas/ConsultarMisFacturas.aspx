<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ConsultarMisFacturas.aspx.cs" Inherits="Facturas.ConsultarMisFacturas" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var prepareToolbar = function (grid, toolbar, rowIndex, record) {
            if (record.get("ID_Estatus") == 1) { //SUGERENCIA
                //toolbar.items.get(0).hide(); //Descargar PDF
                toolbar.items.get(1).hide(); //Generar/Asignar Folio
                toolbar.items.get(2).hide(); //Timbrar
                toolbar.items.get(3).hide(); //Enviar
                toolbar.items.get(4).hide(); //Reenviar
                toolbar.items.get(5).hide(); //Graficar
                toolbar.items.get(6).hide(); //Cancelar
               //toolbar.items.get(7).hide(); //Detalles
                toolbar.items.get(8).hide(); //Eliminar
                //toolbar.items.get(9).hide(); //Pagos
            } else if (record.get("ID_Estatus") == 2) { //CREADA
                //toolbar.items.get(0).hide(); //Descargar PDF
                toolbar.items.get(1).hide(); //Generar
                toolbar.items.get(2).hide(); //Timbrar
                toolbar.items.get(3).hide(); //Enviar
                toolbar.items.get(4).hide(); //Reenviar
                toolbar.items.get(5).hide(); //Graficar
                toolbar.items.get(6).hide(); //Cancelar
                //toolbar.items.get(7).hide(); //Detalles
                toolbar.items.get(8).hide(); //Eliminar
                //toolbar.items.get(9).hide(); //Pagos
            }
            else if (record.get("ID_Estatus") == 3) { //TIMBRADA
                //toolbar.items.get(0).hide(); //Descargar PDF
                toolbar.items.get(1).hide(); //Generar
                toolbar.items.get(2).hide(); //Timbrar
                toolbar.items.get(3).hide(); //Enviar
                toolbar.items.get(4).hide(); //Reenviar
                toolbar.items.get(5).hide(); //Graficar
                toolbar.items.get(6).hide(); //Cancelar
                //toolbar.items.get(7).hide(); //DEtalles
                toolbar.items.get(8).hide(); //Eliminar
                //toolbar.items.get(9).hide(); //Pagos
            } else if (record.get("ID_Estatus") == 4) { //CANCELADA
                //toolbar.items.get(0).hide(); //Descargar PDF
                toolbar.items.get(1).hide(); //Generar
                toolbar.items.get(2).hide(); //Timbrar
                toolbar.items.get(3).hide(); //Enviar
                toolbar.items.get(4).hide(); //Reenviar
                toolbar.items.get(5).hide(); //Graficar
                toolbar.items.get(6).hide(); //Cancelar
                //toolbar.items.get(7).hide(); //Detalles
                toolbar.items.get(8).hide(); //Eliminar
                //toolbar.items.get(9).hide(); //Pagos
            }
            else if (record.get("ID_Estatus") == 5) { //CONFIRMADA
                //toolbar.items.get(0).hide(); //Descargar PDF
                toolbar.items.get(1).hide(); //Generar
                toolbar.items.get(2).hide(); //Timbrar
                toolbar.items.get(3).hide(); //Enviar
                toolbar.items.get(4).hide(); //Reenviar
                toolbar.items.get(5).hide(); //Graficar
                toolbar.items.get(6).hide(); //Cancelar
                //toolbar.items.get(7).hide(); //Detalles
                toolbar.items.get(8).hide(); //Eliminar
                //toolbar.items.get(9).hide(); //Pagos
            } else if (record.get("ID_Estatus") == 6) { //TIMBRADA ENVIADA
                //toolbar.items.get(0).hide(); //Descargar PDF
                toolbar.items.get(1).hide(); //Generar
                toolbar.items.get(2).hide(); //Timbrar
                toolbar.items.get(3).hide(); //Enviar
                //toolbar.items.get(4).hide(); //Reenviar
                toolbar.items.get(5).hide(); //Graficar
                toolbar.items.get(6).hide(); //Cancelar
                //toolbar.items.get(7).hide(); //Detalles
                toolbar.items.get(8).hide(); //Eliminar
                //toolbar.items.get(9).hide(); //Pagos
            }
            else if (record.get("ID_Estatus") == 7) { //SUGERENCIA ENVIADA
                //toolbar.items.get(0).hide(); //Descargar PDF
                toolbar.items.get(1).hide(); //Generar
                toolbar.items.get(2).hide(); //Timbrar
                toolbar.items.get(3).hide(); //Enviar
                toolbar.items.get(4).hide(); //Reenviar
                toolbar.items.get(5).hide(); //Graficar
                toolbar.items.get(6).hide(); //Cancelar
                //toolbar.items.get(7).hide(); //Detalles
                toolbar.items.get(8).hide(); //Eliminar
                //toolbar.items.get(9).hide(); //Pagos
            }
            else if (record.get("ID_Estatus") == 8) { //CONFIRMADA ENVIADA
                //toolbar.items.get(0).hide(); //Descargar PDF
                toolbar.items.get(1).hide(); //Generar
                toolbar.items.get(2).hide(); //Timbrar
                toolbar.items.get(3).hide(); //Enviar
                //toolbar.items.get(4).hide(); //Reenviar
                toolbar.items.get(5).hide(); //Graficar
                toolbar.items.get(6).hide(); //Cancelar
                //toolbar.items.get(7).hide(); //Detalles
                toolbar.items.get(8).hide(); //Eliminar
                //toolbar.items.get(9).hide(); //Pagos
            }
        };

        //in PrepareCommands we can modify commands collection
        var prepareCommands = function (grid, commands, record, row) {
            if (record.get("price") >= 50) {
                commands.push({
                    command: "accept",
                    iconCls: "icon-accept"
                });
            }
        };

        //in PrepareCommand we can modify command
        var prepareCommand = function (grid, command, record, row) {
            if (command.command == 'Edit' && record.get("price") < 50) {
                command.hidden = true;
                command.hideMode = 'display'; //you can try 'visibility' also                 
            }
        };

        var prepareCellCommand = function (grid, command, record, row, col, value) {
            if (command.command == 'Dollar' && record.get("price") < 50) {
                command.iconCls = "icon-moneyeuro";
            }
        };

        var prepareCellCommands = function (grid, commands, record, row, col, value) {
            if (record.get("price") >= 50) {
                commands.push({
                    iconCls: "icon-moneyadd",
                    command: "moneyadd"
                });
            }
        };
    </script>
    <script type="text/javascript">

        var ocultarVentanaSend = function () {



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
            <ext:GridPanel ID="GridPanel2" runat="server" AnchorVertical="50%" StoreID="StoreDetalleFactura"
                StripeRows="true" Header="false" Height="200" Border="false">
                <LoadMask ShowMask="false" />
                <ColumnModel ID="ColumnModel2" runat="server">
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
    <ext:Window ID="winPagos" Title="Ver Pagos de Factura" Icon="ApplicationFormAdd"
        runat="server" Width="460" Height="380" Resizable="False" Hidden="true" Closable="true"
        Modal="true" Layout="FitLayout" Draggable="true" Padding="12">
        <Content>
            <ext:Store ID="storePagos" runat="server">
                <Reader>
                    <ext:JsonReader IDProperty="ID_PagoAsignado">
                        <Fields>
                            <ext:RecordField Name="ID_Poliza" />
                            <ext:RecordField Name="Folio" />
                            <ext:RecordField Name="ImporteAplicado" />
                            <ext:RecordField Name="ID_PagoAsignado" />
                            <ext:RecordField Name="FechaAsignacion" Type="Date" />
                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>
        </Content>
        <Items>
            <ext:GridPanel ID="GridPanel3" runat="server" AnchorVertical="50%" StoreID="storePagos"
                StripeRows="true" Header="false" Height="200" Border="false">
                <LoadMask ShowMask="false" />
                <ColumnModel ID="ColumnModel3" runat="server">
                    <Columns>
                        <ext:CommandColumn ColumnID="acciones" Width="50" Header="">
                            <Commands>
                                <ext:GridCommand Icon="Delete" CommandName="DeletePago">
                                    <ToolTip Text="Eliminar Pago" />
                                </ext:GridCommand>
                            </Commands>
                        </ext:CommandColumn>
                        <ext:DateColumn ColumnID="FechaAsignacion" Header="Fecha" Sortable="true" DataIndex="FechaAsignacion"
                            Format="yyyy-MMM-dd" />
                        <ext:Column ColumnID="ID_Poliza" Header="Poliza" Sortable="true" DataIndex="ID_Poliza" />
                        <ext:Column ColumnID="Folio" Header="Folio" Sortable="true" DataIndex="Folio" />
                        <ext:Column ColumnID="ImporteAplicado" Header="Importe Aplicado" Sortable="true" DataIndex="ImporteAplicado">
                            <Renderer Format="UsMoney" />
                        </ext:Column>
                    </Columns>
                </ColumnModel>
                <DirectEvents>
                    <Command OnEvent="EjecutarComandoDetallePago" IsUpload="true">
                        <Confirmation ConfirmRequest="true" Title="Confirmación" Message="¿Confirmas que deseas Eliminar el Pago?" />
                        <ExtraParams>
                            <ext:Parameter Name="ID" Value="record.data['ID_PagoAsignado']" Mode="Raw" />
                            <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                        </ExtraParams>
                    </Command>
                </DirectEvents>
            </ext:GridPanel>
        </Items>
    </ext:Window>
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true" Collapsible="true">
            <ext:FormPanel ID="FormPanel1" Width="320" Title="Selecciona los Filtros" runat="server"
                Border="false" Layout="Fit">
                <Content>
                    <ext:Hidden ID="FormatType" runat="server" />
                    <ext:Store ID="StoreEmisor" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                                <Fields>
                                    <ext:RecordField Name="Nombre" />
                                    <ext:RecordField Name="ClaveColectiva" />
                                    <ext:RecordField Name="ID_Colectiva" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="Nombre" Direction="ASC" />
                    </ext:Store>
                    <ext:Store ID="StoreReceptor" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                                <Fields>
                                    <ext:RecordField Name="Nombre" />
                                    <ext:RecordField Name="ClaveColectiva" />
                                    <ext:RecordField Name="ID_Colectiva" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="Nombre" Direction="ASC" />
                    </ext:Store>
                    <ext:Store ID="StoreEstatus" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_EstatusOperacion">
                                <Fields>
                                    <ext:RecordField Name="ID_Estatus" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="Descripcion" Direction="ASC" />
                    </ext:Store>
                </Content>
                <Items>
                    <ext:Panel ID="Panel1" Padding="5" runat="server" Layout="FitLayout">
                        <Items>
                            <ext:DateField FieldLabel="Fecha Inicial" AllowBlank="false" MsgTarget="Side" Format="yyyy-MM-dd"
                                ID="datInicio" runat="server" EmptyText="Selecciona una Fecha Inicial" MaxWidth="300"
                                Width="300" MaxLength="12" Vtype="daterange">
                                <CustomConfig>
                                    <ext:ConfigItem Name="endDateField" Value="#{datFinal}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:DateField FieldLabel="Fecha Final" AllowBlank="false" MsgTarget="Side" Format="yyyy-MM-dd"
                                TabIndex="2" ID="datFinal" runat="server" EmptyText="Selecciona una Fecha Final"
                                Width="300" MaxLength="12" Vtype="daterange">
                                <CustomConfig>
                                    <ext:ConfigItem Name="startDateField" Value="#{datInicio}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <%--<ext:DateField ID="datInicio" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                AllowBlank="false" MsgTarget="Side" Format="dd-MMM-yyyy" MaxLength="12"
                                Width="300" EnableKeyEvents="true" MaxWidth="300">
                                <CustomConfig>
                                    <ext:ConfigItem Name="endDateField" Value="#{datFinal}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:DateField ID="datFinal" runat="server" Vtype="daterange" FieldLabel="Fecha Final"
                                AllowBlank="false" MaxLength="12" Width="300" MsgTarget="Side" Format="dd-MMM-yyyy"
                                TabIndex="2" EnableKeyEvents="true">
                                <CustomConfig>
                                    <ext:ConfigItem Name="startDateField" Value="#{datInicio}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>--%>
                            <ext:ComboBox ID="cmbEmisor" TabIndex="3" FieldLabel="Emisores" EmptyText="Todas"
                                Resizable="true" ListWidth="350" Width="300" runat="server" StoreID="StoreEmisor"
                                DisplayField="Nombre" ValueField="ID_Colectiva">
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cmbReceptor" TabIndex="4" FieldLabel="Receptores" EmptyText="Todas"
                                Resizable="true" ListWidth="350" Width="300" runat="server" StoreID="StoreReceptor"
                                DisplayField="Nombre" ValueField="ID_Colectiva">
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cmbEstatus" TabIndex="4" FieldLabel="Estatus" EmptyText="Todos"
                                Resizable="true" ListWidth="350" Width="300" runat="server" StoreID="StoreEstatus"
                                DisplayField="Descripcion" ValueField="ID_Estatus">
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                        </Items>
                    </ext:Panel>
                </Items>
                <FooterBar>
                    <ext:Toolbar ID="Toolbar1" runat="server">
                        <Items>
                            <ext:Button ID="Button1" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Before="var valid= #{FormPanel1}.getForm().isValid(); if (!valid) {} return valid;">
                                        <%--<EventMask ShowMask="true" Target="CustomTarget" CustomTarget="#{BorderLayout2}" Msg="Buscando Facturas..." MinDelay="500"/>--%>
                                        <EventMask ShowMask="true" Msg="Buscando Facturas..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>
            </ext:FormPanel>
        </West>
        <Center Split="true" Collapsible="false">
            <ext:Panel ID="Panel2" runat="server" Title="Mis Facturas" Collapsed="false" Layout="Fit"
                AutoScroll="true">
                <Content>
                    <ext:Window ID="frmSendMail" Title="Enviar Factura" Icon="EmailAttach" runat="server"
                        Width="360" Height="220" Resizable="False" Hidden="true" Closable="true" Modal="true"
                        Layout="FitLayout" Draggable="true" Padding="12">
                        <Items>
                            <ext:Panel ID="Panel3" runat="server" Title="" Layout="RowLayout" Split="true" Padding="6"
                                Collapsible="false">
                                <Items>
                                    <ext:Label runat="server" ID="AN53" Html="Ingresa las direcciones de correo electronico separadas por punto y coma (;) para enviar el PDF y el XML.  <br /><br />">
                                    </ext:Label>
                                    <ext:TextArea ID="txtCorreos" runat="server" FieldLabel="" LabelAlign="Top">
                                    </ext:TextArea>
                                </Items>
                                <BottomBar>
                                    <ext:Toolbar ID="Toolbar3" runat="server">
                                        <Items>
                                            <ext:ToolbarFill ID="ToolbarFill2" runat="server" />
                                            <ext:Button ID="btnEnviar" runat="server" Flat="false" Text="Enviar Factura" ToolTip="Enviar la factura al los correos indicados"
                                                Icon="Accept">
                                                <DirectEvents>
                                                    <Click OnEvent="EnviarCorreo" />
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </BottomBar>
                            </ext:Panel>
                        </Items>
                    </ext:Window>
                    <ext:Window ID="frmGrafica" Title="Historial de Facturación del Receptor" Icon="ChartBar"
                        runat="server" Width="560" Height="360" Resizable="False" Hidden="true" Closable="true"
                        Modal="true" Layout="FitLayout" Draggable="true" Padding="12">
                        <Items>
                            <ext:Panel ID="Panel4" runat="server" Title="" Split="true" Padding="6" Collapsible="false"
                                ShowOnLoad="true" CenterOnLoad="true">
                                <LoadMask ShowMask="true" />
                                <AutoLoad Url="laGrafica.aspx" Mode="IFrame" ShowMask="true" MaskMsg="Generando Gráfica, por favor espere...">
                                </AutoLoad>
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar4" runat="server">
                                        <Items>
                                            <ext:ToolbarFill ID="AN56" />
                                            <ext:Button ID="Button6" runat="server" Text="Graficar Historial" Icon="ArrowRotateClockwise">
                                                <Listeners>
                                                    <Click Handler="#{Panel4}.reload();" />
                                                </Listeners>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                            </ext:Panel>
                        </Items>
                    </ext:Window>
                    <ext:Store ID="Store1" runat="server" OnSubmitData="Store1_Submit" OnRefreshData="btnBuscar_Click"
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
                                    <ext:RecordField Name="EmailEmisor" />
                                    <ext:RecordField Name="IVA" />
                                    <ext:RecordField Name="SubTotal" />
                                    <ext:RecordField Name="ImporteTotal" />
                                    <ext:RecordField Name="FechaTimbrado" Type="Date" />
                                    <ext:RecordField Name="DescripcionFactura" />
                                    <ext:RecordField Name="ID_Emisor" />
                                    <ext:RecordField Name="NombreEmisor" />
                                    <ext:RecordField Name="ID_Estatus" />
                                    <ext:RecordField Name="Descripcion" />
                                    <ext:RecordField Name="TipoComprobante" />
                                    <ext:RecordField Name="EstatusPago" />
                                </Fields>
                                <%--</ext:ArrayReader>--%>
                            </ext:JsonReader>
                        </Reader>
                        <DirectEventConfig IsUpload="true" />
                        <SortInfo Field="FechaEmision" />
                    </ext:Store>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store1" StripeRows="true"
                                Header="false" Border="false">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                        <ext:CommandColumn ColumnID="acciones" Width="150" Header="">
                                            <Commands>
                                                <ext:GridCommand Icon="PageWhiteAcrobat" CommandName="DescargarPDF">
                                                    <ToolTip Text="ObtenerPDF" />
                                                </ext:GridCommand>
                                                <ext:GridCommand Icon="Accept" CommandName="Foliar">
                                                    <ToolTip Text="Asignar Folio a Factura" />
                                                </ext:GridCommand>
                                                <ext:GridCommand Icon="Bell" CommandName="Timbrar">
                                                    <ToolTip Text="Timbrar" />
                                                </ext:GridCommand>
                                                <ext:GridCommand Icon="EmailAttach" CommandName="Reenviar">
                                                    <ToolTip Text="Enviar a los Emails Registrados" />
                                                </ext:GridCommand>
                                                <ext:GridCommand Icon="EmailTransfer" CommandName="Reenviar">
                                                    <ToolTip Text="Reenviar a los Emails" />
                                                </ext:GridCommand>
                                                <%-- <ext:GridCommand Icon="XhtmlGo" CommandName="DownXML">
                                                    <ToolTip Text="Descargar el XML" />
                                                </ext:GridCommand>
                                                <ext:GridCommand Icon="PageWhiteAcrobat" CommandName="DownPDF">
                                                    <ToolTip Text="Descargar el PDF" />
                                                </ext:GridCommand>--%>
                                                <ext:GridCommand Icon="ChartLine" CommandName="Graficar">
                                                    <ToolTip Text="Graficar Comportamiento Receptor" />
                                                </ext:GridCommand>
                                                <ext:GridCommand Icon="Delete" CommandName="Cancelar">
                                                    <ToolTip Text="Cancelar" />
                                                </ext:GridCommand>
                                                <ext:GridCommand Icon="TextListBullets" CommandName="Detalles">
                                                    <ToolTip Text="Ver Detalles" />
                                                </ext:GridCommand>
                                                <ext:GridCommand Icon="Decline" CommandName="Eliminar">
                                                    <ToolTip Text="Eliminar" />
                                                </ext:GridCommand>
                                                 <ext:GridCommand Icon="MoneyDollar" CommandName="Pagos">
                                                    <ToolTip Text="Ver Pagos a Factura" />
                                                </ext:GridCommand>
                                            </Commands>
                                            <PrepareToolbar Fn="prepareToolbar" />
                                        </ext:CommandColumn>
                                        <ext:Column ColumnID="ID_Factura" Header="Identificador" Sortable="true" DataIndex="ID_Factura" />
                                        <ext:Column ColumnID="EstatusPago" Header="Estatus Pago" Sortable="true" DataIndex="EstatusPago" />
                                        <ext:Column ColumnID="Descripcion" Header="Estatus" Sortable="true" DataIndex="Descripcion" />
                                        <ext:Column ColumnID="Folio" Header="Folio" Sortable="true" DataIndex="Folio" />
                                        <ext:Column ColumnID="TipoComprobante" Header="Tipo Comprobante" Sortable="true"
                                            DataIndex="TipoComprobante" />
                                        <ext:DateColumn ColumnID="FechaEmision" Header="Fecha" Sortable="true" DataIndex="FechaEmision"
                                            Format="dd-MMM-yyyy" />
                                        <ext:DateColumn ColumnID="FechaTimbrado" Header="Fecha Timbre" Sortable="true" DataIndex="FechaTimbrado"
                                            Format="dd-MMM-yyyy" />
                                        <ext:Column ColumnID="DescripcionFactura" Header="Factura" Sortable="true" DataIndex="DescripcionFactura" />
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
                                    <Command OnEvent="EjecutarComando" IsUpload="true">
                                        <Confirmation BeforeConfirm="if (command=='Detalles' | command=='DescargarPDF' | command=='Graficar'| command=='Reenviar' | command=='Pagos'  ) return false;"
                                            ConfirmRequest="true" Title="Confirmación" Message="¿Confirmas que deseas ejecutar la acción Seleccionada?" />
                                        <ExtraParams>
                                            <ext:Parameter Name="ID" Value="record.data['ID_Factura']" Mode="Raw" />
                                            <ext:Parameter Name="EmailReceptor" Value="record.data['EmailReceptor']" Mode="Raw" />
                                            <ext:Parameter Name="EmailEmisor" Value="record.data['EmailEmisor']" Mode="Raw" />
                                            <ext:Parameter Name="ID_Receptor" Value="record.data['ID_ColectivaReceptora']" Mode="Raw" />
                                            <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                        </ExtraParams>
                                    </Command>
                                </DirectEvents>
                                <Plugins>
                                    <ext:GridFilters runat="server" ID="GridFilters1" Local="true">
                                        <Filters>
                                            <ext:NumericFilter DataIndex="ID_Operacion" />
                                            <ext:StringFilter DataIndex="NombreEmisor" />
                                            <ext:StringFilter DataIndex="NombreReceptor" />
                                            <ext:StringFilter DataIndex="SubTotal" />
                                            <ext:StringFilter DataIndex="IVA" />
                                            <ext:StringFilter DataIndex="ImporteTotal" />
                                            <ext:StringFilter DataIndex="DescripcionFactura" />
                                            <ext:StringFilter DataIndex="Folio" />
                                            <ext:DateFilter DataIndex="FechaEmision">
                                                <DatePickerOptions runat="server" TodayText="Hoy" />
                                            </ext:DateFilter>
                                        </Filters>
                                    </ext:GridFilters>
                                </Plugins>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="Store1" DisplayInfo="true"
                                        DisplayMsg="Mostrando Facturas {0} - {1} de {2}" />
                                </BottomBar>
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar2" runat="server">
                                        <Items>
                                            <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                                            <ext:Button ID="Button2" runat="server" Text="Exportar a XML" Icon="PageCode">
                                                <Listeners>
                                                    <Click Handler="submitValue(#{GridPanel1}, #{FormatType}, 'xml');" />
                                                </Listeners>
                                            </ext:Button>
                                            <ext:Button ID="Button3" runat="server" Text="Exportar a Excel" Icon="PageExcel">
                                                <Listeners>
                                                    <Click Handler="submitValue(#{GridPanel1}, #{FormatType}, 'xls');" />
                                                </Listeners>
                                            </ext:Button>
                                            <ext:Button ID="Button4" runat="server" Text="Exportar a CSV" Icon="PageAttach">
                                                <Listeners>
                                                    <Click Handler="submitValue(#{GridPanel1}, #{FormatType}, 'csv');" />
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

