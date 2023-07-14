<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="AsignarPagoFactura.aspx.cs" Inherits="Facturas.AsignarPagoFactura" %>

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


    </script>
    <script>
        function invertSelection(grid) {
            var sm = grid.getSelectionModel(), selected = sm.getSelection();
            sm.deselect(selected); sm.select(Ext.Array.difference(grid.store.getRange(), selected));
        }

        function sumPago(grid, esMoneda) {
            var sm = grid.getSelectionModel(), selected = sm.selections, sum = 0;
            for (var i = 0, len = selected.length; i < len; i++) {
                sum += selected.items[i].data.Monto;
            }
            //alert(formatCurrency(sum));
            if (esMoneda == 1) {
                return formatCurrency(sum);
            } else {
                //return sum;
                return Math.round(sum * 100) / 100;
            }
        }

        function sumFactura(grid) {
            var sm = grid.getSelectionModel(), selected = sm.selections, sum = 0;
            for (var i = 0, len = selected.length; i < len; i++) {
                sum += selected.items[i].data.Importe;
            }
            //alert(formatCurrency(sum));
            return formatCurrency(sum);
        }

        function sumAdeudos(grid) {
            var sm = grid.getSelectionModel(), selected = sm.selections, sum = 0;
            for (var i = 0, len = selected.length; i < len; i++) {
                sum += selected.items[i].data.Adeudo;
            }
            
            return Math.round(sum * 100) / 100;
        }
        
        function formatCurrency(num) {
            num = num.toString().replace(/\$|\,/g, '');

            if (isNaN(num))
                num = '0';

            var signo = (num == (num = Math.abs(num)));
            num = Math.floor(num * 100 + 0.50000000001);
            centavos = num % 100;
            num = Math.floor(num / 100).toString();

            if (centavos < 10)
                centavos = '0' + centavos;

            for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3); i++)
                num = num.substring(0, num.length - (4 * i + 3)) + ',' + num.substring(num.length - (4 * i + 3));

            return (((signo) ? '' : '-') + '$' + num + '.' + centavos);
        }


        var Confirm = function (grid1, grid2) {
            var title = 'Confirmación de Asignación';
            var msg = 'Se asignaran los elementos seleccionados </br>' + grid1.getSelectionModel().selections.length + ' Pago(s): <b>' + sumPago(grid1, 1) + '</b></br>' + grid2.getSelectionModel().selections.length + ' Factura(s): <b>' + sumFactura(grid2) + '</b> </br> ¿Es correcto?';

            if (grid1.getSelectionModel().selections.length == 0) {
                Ext.Msg.alert('Asignacion de Pagos', 'Por favor, Selecciona por lo menos un Pago.');
                return false;
            }


            if (grid2.getSelectionModel().selections.length == 0) {
                Ext.Msg.alert('Asignacion de Pagos', 'Por favor, Selecciona por lo menos una Factura.');
                return false;
            }


            if (grid2.getSelectionModel().selections.length >= 2 & grid1.getSelectionModel().selections.length >= 2) {
                Ext.Msg.alert('Asignacion de Pagos', 'Sólo es posible Asignar un Pago a varias facturas o una Factura a Varios Pagos.');
                return false;
            }

            if (sumPago(grid1, 0) > sumAdeudos(grid2))
            {
                Ext.Msg.alert('Asignacion de Pagos', 'El monto del(os) Pago(s) debe ser menor o igual al total de adeudos de la(s) Factura(s).');
                return false;
            }

            Ext.Msg.confirm(title, msg, function (btn) {
                if (btn == 'yes') {
                    Ext.net.Mask.show({ msg : 'Procesando...' });
                    Asignaciones.btnAsignarPagos();
                    return true;
                } else {
                    return false;
                }
            });
        }

    </script>
    <script language="javascript" type="text/javascript">
        function confirmCreate() {
            Ext.Msg.confirm('Confirm', 'Really?', function (btn, text) {
                if (btn == 'yes') {
                    Ext.net.DirectMethods.CreateType(
                    {
                        success: function (result) {
                            Ext.Msg.alert('Manager', result);
                        }
                    })
                }
            });
        }
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
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Content>
            <ext:Store ID="stFacturas" runat="server" OnSubmitData="Store1_Submit">
                <AutoLoadParams>
                    <ext:Parameter Name="start" Value="0" Mode="Raw" />
                    <ext:Parameter Name="limit" Value="20" Mode="Raw" />
                </AutoLoadParams>
                <DirectEventConfig IsUpload="true" />
                <Reader>
                    <ext:JsonReader IDProperty="ID_Factura">
                        <Fields>
                            <ext:RecordField Name="ID_Factura" />
                            <ext:RecordField Name="Folio" />
                            <ext:RecordField Name="FechaEmision" Type="Date" />
                            <ext:RecordField Name="RFCReceptor" />
                            <ext:RecordField Name="NombreReceptor" />
                            <ext:RecordField Name="DescFaturaTipo" />
                            <ext:RecordField Name="Importe" />
                            <ext:RecordField Name="Adeudo" />
                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>
            <ext:Hidden ID="FormatType" runat="server" />
            <ext:Hidden ID="ColConfig" runat="server" />
        </Content>
        <Center Split="true">
            <ext:Panel runat="server" Title="Asignación de Pagos a Facturas">
                <Items>
                    <ext:ColumnLayout ID="BorderLayout2" Split="true" FitHeight="true" runat="server">
                        <Columns>
                            <ext:LayoutColumn ColumnWidth="0.50">
                                <ext:Panel ID="Panel1" runat="server" Title="Pagos Registrados" Split="true"
                                    Padding="6" Layout="FitLayout">
                                    <TopBar>
                                        <ext:Toolbar ID="Toolbar2" runat="server">
                                            <Items>
                                                <ext:ToolbarFill />
                                                <ext:TextField ID="txtCadenaComercial" runat="server" EmptyText="Cadena Comercial" />
                                                <ext:DateField ID="datPagoInicio" runat="server" Width="100" Text="" />
                                                <ext:DateField ID="datPagoFin" runat="server" Width="100" Text="" />
                                                <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                                    <DirectEvents>
                                                        <Click OnEvent="btnBuscar_Click">
                                                            <EventMask ShowMask="true" Msg="Buscando Pagos..." MinDelay="500" />
                                                        </Click>
                                                    </DirectEvents>
                                                </ext:Button>
                                                <%-- <ext:Button runat="server" ID="btnExcel" Text="To Excel" Icon="PageExcel" AutoPostBack="true">
                                               <Listeners>
                                                    <Click Handler="submitValue(#{GridPanel1}, #{FormatType}, 'xls');" />
                                                </Listeners>
                                            </ext:Button>
                                             <ext:Button runat="server" ID="btnCSV" Text="To CSV" Icon="PageAttach" AutoPostBack="true">
                                                <Listeners>
                                                    <Click Handler="setFields(#{GridPanel2}, #{FormatType}, #{ColConfig}, 'CSV');" />
                                                </Listeners>
                                            </ext:Button>
                                            <ext:Button runat="server" ID="btnXml" Text="To XML" Icon="PageCode" AutoPostBack="true">
                                                <Listeners>
                                                    <Click Handler="setFields(#{GridPanel2}, #{FormatType}, #{ColConfig}, 'XML');" />
                                                </Listeners>
                                            </ext:Button>--%>
                                            </Items>
                                        </ext:Toolbar>
                                    </TopBar>
                                    <Items>
                                        <ext:GridPanel Visible="true" ID="GridPanel2" runat="server" StripeRows="true" RemoveViewState="true"
                                            Header="false" TrackMouseOver="true" Border="false">
                                            <Store>
                                                <ext:Store ID="stPolizasPago" runat="server" OnSubmitData="Store1_Submit">
                                                    <AutoLoadParams>
                                                        <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                                        <ext:Parameter Name="limit" Value="20" Mode="Raw" />
                                                    </AutoLoadParams>
                                                    <DirectEventConfig IsUpload="true" />
                                                    <Reader>
                                                        <ext:JsonReader IDProperty="ID_Poliza">
                                                            <Fields>
                                                                <ext:RecordField Name="ID_Poliza" />
                                                                <ext:RecordField Name="TipoCuentaDNU" />
                                                                <ext:RecordField Name="TipoCuentaCadena" />
                                                                <ext:RecordField Name="CadenaComercial" />
                                                                <ext:RecordField Name="Fecha" Type="Date" />
                                                                <ext:RecordField Name="Monto" />
                                                                <ext:RecordField Name="Remanente" />
                                                                <ext:RecordField Name="Referencia" />
                                                                <ext:RecordField Name="Observaciones" />
                                                            </Fields>
                                                        </ext:JsonReader>
                                                    </Reader>
                                                </ext:Store>
                                            </Store>
                                            <LoadMask ShowMask="false" />
                                            <ColumnModel ID="ColumnModel1" runat="server">
                                                <Columns>
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
                                                </Columns>
                                            </ColumnModel>
                                            <SelectionModel>
                                                <ext:CheckboxSelectionModel ID="CheckboxSelectionModel1" runat="server" CheckOnly="True" />
                                                <%--<ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true" />--%>
                                            </SelectionModel>
                                            <BottomBar>
                                                <ext:PagingToolbar ID="PagingToolbar1" runat="server" PageSize="20">
                                                </ext:PagingToolbar>
                                            </BottomBar>
                                            <ToolTips>
                                                <ext:ToolTip ID="ToolTip1" runat="server" Target="={#{GridPanel2}.getView().mainBody}"
                                                    Delegate=".x-grid3-row" TrackMouse="true">
                                                    <Listeners>
                                                        <%--<Show Handler="var rowIndex = #{GridPanel2}.view.findRowIndex(this.triggerElement);this.body.dom.innerHTML = '<b>Company :</b> ' + #{stPolizasPago}.getAt(rowIndex).get('Monto');" />--%>
                                                        <Show Handler="var rowIndex = #{GridPanel2}.view.findRowIndex(this.triggerElement);this.body.dom.innerHTML = '<b>Cadena :</b> ' + #{stPolizasPago}.getAt(rowIndex).get('CadenaComercial') + '</br><b>Importe :</b> ' + formatCurrency(#{stPolizasPago}.getAt(rowIndex).get('Monto')) + '</br><b>Remanente :</b> ' + formatCurrency(#{stPolizasPago}.getAt(rowIndex).get('Remanente')) + '</br><b>Referencia :</b> ' + #{stPolizasPago}.getAt(rowIndex).get('Referencia') + '</br><b>Observaciones :</b> ' + #{stPolizasPago}.getAt(rowIndex).get('Observaciones');" />
                                                    </Listeners>
                                                </ext:ToolTip>
                                            </ToolTips>
                                        </ext:GridPanel>
                                    </Items>
                                </ext:Panel>
                            </ext:LayoutColumn>
                            <ext:LayoutColumn ColumnWidth="0.50">
                                <ext:Panel ID="PnlHeader" runat="server" Title="Facturas Pendientes de Pago"
                                    Split="true" Padding="6" Collapsible="false" Layout="FitLayout">
                                    <TopBar>
                                        <ext:Toolbar ID="Toolbar3" runat="server">
                                            <Items>
                                                <ext:ToolbarFill />
                                                <ext:TextField Name="Folio" TabIndex="5" EmptyText="Folio" ID="txtFolio" runat="server"
                                                    Width="80" Text="" />
                                                <ext:TextField Name="CadenaOReceptor" TabIndex="5" EmptyText="Cadena ó Receptor" ID="txtCadenaOReceptor"
                                                    runat="server" Width="100" Text="" />
                                                <ext:DateField runat="server" ID="datFacturaInicio" Width="100" Text="" />
                                                <ext:DateField runat="server" ID="datFacturaFin" Width="100" Text="" />
                                                <ext:Button ID="btnBuscar2" runat="server" Text="Buscar" Icon="Magnifier">
                                                    <DirectEvents>
                                                        <Click OnEvent="btnBuscar2_Click">
                                                              <EventMask ShowMask="true" Msg="Buscando Facturas..." MinDelay="500" />
                                                        </Click>
                                                    </DirectEvents>
                                                </ext:Button>
                                                <%-- <ext:Button runat="server" ID="btnExcel2" Text="To Excel" Icon="PageExcel" AutoPostBack="true">
                                                <Listeners>
                                                    <Click Handler="setFields(#{GridPanel1}, #{FormatType}, #{ColConfig}, 'Excel');" />
                                                </Listeners>
                                            </ext:Button>
                                          <ext:Button runat="server" ID="btnCSV2" Text="To CSV" Icon="PageAttach" AutoPostBack="true">
                                                        <Listeners>
                                                            <Click Handler="setFields(#{GridPanel1}, #{FormatType}, #{ColConfig}, 'CSV');" />
                                                        </Listeners>
                                                    </ext:Button>
                                                    <ext:Button runat="server" ID="btnXml2" Text="To XML" Icon="PageCode" AutoPostBack="true">
                                                        <Listeners>
                                                            <Click Handler="setFields(#{GridPanel1}, #{FormatType}, #{ColConfig}, 'XML');" />
                                                        </Listeners>
                                                    </ext:Button>--%>
                                            </Items>
                                        </ext:Toolbar>
                                    </TopBar>
                                    <Items>
                                        <%-- Poner las variables automaticas --%>
                                        <ext:GridPanel Visible="true" ID="GridPanel1" runat="server" TrackMouseOver="true"
                                            StoreID="stFacturas" StripeRows="true" RemoveViewState="true" Header="false"
                                            Border="false">
                                            <LoadMask ShowMask="false" />
                                            <ColumnModel ID="ColumnModel2" runat="server">
                                                <Columns>
                                                    <ext:Column ColumnID="ID_Factura" Hidden="true" Header="ID_Factura" Sortable="true"
                                                        DataIndex="ID_Factura" />
                                                    <ext:Column ColumnID="Folio" Header="Folio" DataIndex="Folio" Sortable="true" />
                                                    <ext:DateColumn ColumnID="FechaEmision" Header="Fecha" Sortable="true" DataIndex="FechaEmision"
                                                        Format="yyyy-MMM-dd" />
                                                     <ext:Column ColumnID="FacturaTipo" Header="FacturaTipo" Sortable="true" DataIndex="DescFaturaTipo" />
                                                    <ext:Column ColumnID="Importe" Header="Importe" Sortable="true" Align="Right" DataIndex="Importe">
                                                        <Renderer Format="UsMoney" />
                                                    </ext:Column>
                                                    <ext:Column ColumnID="Adeudo" Header="Adeudo" Align="Right" Sortable="true" DataIndex="Adeudo">
                                                        <Renderer Format="UsMoney" />
                                                    </ext:Column>
                                                    <ext:Column ColumnID="RFCReceptor" Header="RFCReceptor" Sortable="true" DataIndex="RFCReceptor" />
                                                    <ext:Column ColumnID="NombreReceptor" Header="NombreReceptor" Sortable="true" DataIndex="NombreReceptor" />
                                                </Columns>
                                            </ColumnModel>
                                            <SelectionModel>
                                                <ext:CheckboxSelectionModel ID="CheckboxSelectionModel2" runat="server" CheckOnly="True">
                                                </ext:CheckboxSelectionModel>
                                            </SelectionModel>
                                            <BottomBar>
                                                <ext:PagingToolbar ID="PagingToolbar2" runat="server" PageSize="20">
                                                </ext:PagingToolbar>
                                            </BottomBar>
                                            <%--  <BottomBar>
                                            <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="stFacturas" DisplayInfo="true"
                                                DisplayMsg="Mostrando Facturas {0} - {1} de {2}" />
                                        </BottomBar>--%>
                                            <%-- <Items>
                                            <ext:Toolbar ID="Toolbar4" runat="server">
                                                <Items>
                                                    <ext:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                    <ext:Button ID="Button2" runat="server" Text="Asignar Pago a Factura" Icon="MoneyAdd">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnAsignarPagos">
                                                                <Confirmation ConfirmRequest="true" Title="Confirmación" Message="¿Confirmas que deseas ejecutar la acción Seleccionada?" />
                                                            </Click>
                                                        </DirectEvents>
                                                        <Listeners>
                                                            <Click Handler="alert(getSum(this, 6));" />
                                                        </Listeners>
                                                    </ext:Button>
                                                </Items>
                                            </ext:Toolbar>
                                        </Items>--%>
                                            <ToolTips>
                                                <ext:ToolTip ID="ToolTip2" runat="server" Target="={#{GridPanel1}.getView().mainBody}"
                                                    Delegate=".x-grid3-row" TrackMouse="true">
                                                    <Listeners>
                                                        <%--<Show Handler="var rowIndex = #{GridPanel2}.view.findRowIndex(this.triggerElement);this.body.dom.innerHTML = '<b>Company :</b> ' + #{stPolizasPago}.getAt(rowIndex).get('Monto');" />--%>
                                                        <Show Handler="var rowIndex = #{GridPanel1}.view.findRowIndex(this.triggerElement);this.body.dom.innerHTML = '<b>Importe :</b> ' + formatCurrency( #{stFacturas}.getAt(rowIndex).get('Importe')) + '</br><b>Adeudo :</b> ' + formatCurrency( #{stFacturas}.getAt(rowIndex).get('Adeudo'));" />
                                                    </Listeners>
                                                </ext:ToolTip>
                                            </ToolTips>
                                            <%--<Buttons>
                                            <ext:Button ID="Button5" runat="server" Text="Sum of selected (price)">
                                                <Listeners>
                                                    <Click Handler="sum(#{GridPanel1});" />
                                                </Listeners>
                                            </ext:Button>
                                        </Buttons>--%>
                                        </ext:GridPanel>
                                    </Items>
                                </ext:Panel>
                            </ext:LayoutColumn>
                        </Columns>
                    </ext:ColumnLayout>
                </Items>
                <BottomBar>
                    <ext:Toolbar ID="Toolbar1" runat="server">
                        <Items>
                            <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                            <ext:Button ID="Button2" runat="server" Text="Limpiar Selección" Icon="Erase">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiarSeleccion">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="Button1" runat="server" Text="Asignar Pago a Factura" Icon="MoneyAdd">
                                <%--<DirectEvents>
                                    <Click>
                                        <EventMask ShowMask="true" Msg="Buscando Facturas..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>--%>

                                <%-- <Click OnEvent="btnAsignarPagos">--%>
                                <%--<Confirmation ConfirmRequest="true" BeforeConfirm="return Confirm(#{GridPanel2},#{GridPanel1});"
                                            Message="Hola" Title="Confirmación" />--%>
                                <%-- <ExtraParams>
                                        <Click OnEvent="btnGuardar_Click" Before="var valid= #{FormPanel1}.getForm().isValid(); if (!valid) {} return valid;" />
                                            <ext:Parameter Name="Facturas" Value="Ext.encode(#{GridPanel1}.getRowsValues({selectedOnly:true}))"
                                                Mode="Raw" />
                                            <ext:Parameter Name="Polizas" Value="Ext.encode(#{GridPanel2}.getRowsValues({selectedOnly:true}))"
                                                Mode="Raw" />
                                        </ExtraParams>--%>
                                <%-- </Click>
                                    <Click Handler="return Confirm(#{GridPanel2},#{GridPanel1});" />

                                </DirectEvents>--%>

                                <Listeners>
                                    <Click Handler="return Confirm(#{GridPanel2},#{GridPanel1});"/>
                                </Listeners>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </BottomBar>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
