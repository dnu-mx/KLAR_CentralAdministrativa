<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="LosPagosRegistrados.aspx.cs" Inherits="Facturas.LosPagosRegistrados" %>

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

        var prepareToolbar = function (grid, toolbar, rowIndex, record) {
            if (record.get("TieneFacturaPago") == 0) {
                toolbar.items.get(0).hide(); //Descargar PDF
                toolbar.items.get(1).hide(); //Enviar CFDI
            }
        };

    </script>
    <script type="text/javascript">
        var getSum = function (grid, index) {
            var dataIndex = grid.getColumnModel().getDataIndex(index),
            sum = 0;
            grid.getStore().each(function (record)
            { sum += record.get(dataIndex); });
            return sum;
        };   
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Window ID="winFaturas" Title="Ver Facturas" Icon="ApplicationFormAdd" runat="server"
        Width="460" Height="380" Resizable="False" Hidden="true" Closable="true" Modal="true"
        Layout="FitLayout" Draggable="true" Padding="12">
        <Content>
            <ext:Store ID="storeFactura" runat="server">
                <Reader>
                    <ext:JsonReader IDProperty="ID_PagoAsignado">
                        <Fields>
                            <ext:RecordField Name="ID_Factura" />
                            <ext:RecordField Name="Folio" />
                            <ext:RecordField Name="ImporteAplicado" />
                            <ext:RecordField Name="FechaEmision" />
                            <ext:RecordField Name="FechaAsignacion" Type="Date" />
                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>
        </Content>
        <Items>
            <ext:GridPanel ID="GridPanel3" runat="server" AnchorVertical="50%" StoreID="storeFactura"
                StripeRows="true" Header="false" Height="200" Border="false">
                <LoadMask ShowMask="false" />
                <ColumnModel ID="ColumnModel3" runat="server">
                    <Columns>
                        <ext:DateColumn ColumnID="FechaAsignacion" Header="Fecha Asignación" Sortable="true" DataIndex="FechaAsignacion"
                            Format="yyyy-MMM-dd" />
                        <ext:Column ColumnID="" Header="Folio" Sortable="true" DataIndex="Folio" />
                        <ext:DateColumn ColumnID="FechaEmision" Header="Fecha Emisión" Sortable="true" DataIndex="FechaEmision"
                            Format="yyyy-MMM-dd" />
                        <ext:Column ColumnID="ImporteAplicado" Header="Importe Aplicado" Sortable="true"
                            DataIndex="ImporteAplicado">
                            <Renderer Format="UsMoney" />
                        </ext:Column>
                    </Columns>
                </ColumnModel>
            </ext:GridPanel>
        </Items>
    </ext:Window>
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
                                    <Click OnEvent="EnviarCorreo">
                                        <EventMask ShowMask="true" Msg="Enviando Correo..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </BottomBar>
            </ext:Panel>
        </Items>
    </ext:Window>
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Content>
            <ext:Store ID="stPolizasPago" runat="server" OnSubmitData="Store1_Submit">
                <DirectEventConfig IsUpload="true" />
                <Reader>
                    <ext:JsonReader IDProperty="ID_Pagos">
                        <Fields>
                            <ext:RecordField Name="ID_Pagos" />
                            <ext:RecordField Name="ID_Poliza" />
                            <ext:RecordField Name="TipoCuentaDNU" />
                            <ext:RecordField Name="TipoCuentaCadena" />
                            <ext:RecordField Name="CadenaComercial" />
                            <ext:RecordField Name="Fecha" Type="Date" />
                            <ext:RecordField Name="Monto" />
                            <ext:RecordField Name="Remanente" />
                            <ext:RecordField Name="Referencia" />
                            <ext:RecordField Name="Observaciones" />
                            <ext:RecordField Name="TieneFacturaPago" />
                            <ext:RecordField Name="ID_Factura" />
                            <ext:RecordField Name="EmailEmisor" />
                            <ext:RecordField Name="EmailReceptor" />
                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>
            <ext:Hidden ID="FormatType" runat="server" />
            <ext:Hidden ID="ColConfig" runat="server" />
        </Content>
        <Center Split="true">
            <ext:Panel ID="Panel1" runat="server"  Layout="FitLayout">
                <Items>
                    <ext:Panel ID="Panel2" runat="server" Title="Pagos Registrados" Region="West" Split="true"
                        Padding="6" Collapsible="false" Layout="FitLayout">
                        <TopBar>
                            <ext:Toolbar ID="Toolbar2" runat="server">
                                <Items>
                                    <ext:ToolbarFill runat="server" ID="fill_" />
                                    <ext:Hidden ID="hdnIdPoliza" runat="server" />
                                    <ext:Hidden ID="hdnIdFactura" runat="server" />
                                    <ext:TextField ID="txtCadenaComercial" runat="server" EmptyText="Cadena Comercial"/>
                                    <ext:DateField ID="datPagoInicio" runat="server" Text="" />
                                    <ext:DateField ID="datPagoFin" runat="server" Text="" />
                                    <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                        <DirectEvents>
                                            <Click OnEvent="btnBuscar_Click">
                                                <EventMask ShowMask="true" Msg="Buscando Pagos..." MinDelay="500" />
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                        <Items>
                            <ext:GridPanel Visible="true" ID="GridPanel2" runat="server" StoreID="stPolizasPago"
                                StripeRows="true" RemoveViewState="true" Header="false" Border="false">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                        <ext:CommandColumn ColumnID="acciones" Width="70" Header="">
                                            <PrepareToolbar Fn="prepareToolbar" />
                                            <Commands>
                                                <ext:GridCommand Icon="PageWhiteAcrobat" CommandName="DescargarPDF">
                                                    <ToolTip Text="Descargar PDF" />
                                                </ext:GridCommand>
                                                <ext:GridCommand Icon="EmailAttach" CommandName="CFDI">
                                                    <ToolTip Text="Enviar CFDI" />
                                                </ext:GridCommand>
                                                <ext:GridCommand Icon="MoneyDollar" CommandName="VerFacturas">
                                                    <ToolTip Text="Ver Facturas" />
                                                </ext:GridCommand>
                                            </Commands>
                                        </ext:CommandColumn>
                                        <ext:Column ColumnID="ID_Pagos" Hidden="true" DataIndex="ID_Pagos" />
                                        <ext:Column ColumnID="ID_Poliza" Header="ID_Poliza" Sortable="true" DataIndex="ID_Poliza" />
                                        <ext:DateColumn ColumnID="Fecha" Header="Fecha" Sortable="true" DataIndex="Fecha"
                                            Format="yyyy-MMM-dd" />
                                        <ext:Column ColumnID="TipoCuentaCadena" Header="Cuenta Destino" Sortable="true" DataIndex="TipoCuentaCadena" />
                                        <ext:Column ColumnID="CadenaComercial" Header="Cadena Comercial" DataIndex="CadenaComercial"
                                            Sortable="true" />
                                        <ext:Column ColumnID="Remanente" Header="Remanente" Align="Right" Sortable="true"
                                            DataIndex="Remanente">
                                            <Renderer Format="UsMoney" />
                                        </ext:Column>
                                        <ext:Column ColumnID="Monto" Header="Monto" Sortable="true" Align="Right" DataIndex="Monto">
                                            <Renderer Format="UsMoney" />
                                        </ext:Column>
                                        <ext:Column ColumnID="Referencia" Header="Referencia" Sortable="true" DataIndex="Referencia" />
                                        <ext:Column ColumnID="Observaciones" Header="Observaciones" Sortable="true" DataIndex="Observaciones" />
                                        <ext:Column ColumnID="TipoCuentaDNU" Header="Cuenta DNU" Sortable="true" DataIndex="TipoCuentaDNU" />
                                        <ext:Column ColumnID="ID_Factura" Hidden="true" DataIndex="ID_Factura" />
                                        <ext:Column ColumnID="EmailEmisor" Hidden="true" DataIndex="EmailEmisor" />
                                        <ext:Column ColumnID="EmailReceptor" Hidden="true" DataIndex="EmailReceptor" />
                                    </Columns>
                                </ColumnModel>
                                <Listeners>
                                    <Command Handler="Ext.net.Mask.show({ msg : 'Procesando...' }); Facturas.StopMask();" />
                                    </Listeners>
                                 <DirectEvents>
                                    <Command OnEvent="EjecutarComando" IsUpload="true">
                                        
                                        <ExtraParams>
                                            <ext:Parameter Name="ID_Poliza" Value="record.data['ID_Poliza']" Mode="Raw" />
                                            <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                            <ext:Parameter Name="ID_Factura" Value="record.data['ID_Factura']" Mode="Raw" />
                                            <ext:Parameter Name="EmailEmisor" Value="record.data['EmailEmisor']" Mode="Raw" />
                                            <ext:Parameter Name="EmailReceptor" Value="record.data['EmailReceptor']" Mode="Raw" />
                                        </ExtraParams>
                                    </Command>
                                </DirectEvents>
                            </ext:GridPanel>
                        </Items>
                    </ext:Panel>
                </Items>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
