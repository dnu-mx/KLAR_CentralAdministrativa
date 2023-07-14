<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Reporte_Pedidos.aspx.cs" Inherits="CentroContacto.Reporte_Pedidos" %>


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

        Number.prototype.formatMoney = function (c) {
            var n = this,
                c = isNaN(c = Math.abs(c)) ? 2 : c,
                d = d == undefined ? "." : d,
                t = t == undefined ? "," : t,
                s = n < 0 ? "-" : "",
                i = String(parseInt(n = Math.abs(Number(n) || 0).toFixed(c))),
                j = (j = i.length) > 3 ? j % 3 : 0;
            return s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
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
        <West Split="true" Collapsible="true">
            <ext:FormPanel ID="FormPanel1" Width="320" Title="Selecciona los Filtros" runat="server" Padding="10"
                Border="false" Layout="FitLayout">
                <Content>
                    <ext:Hidden ID="FormatType" runat="server" />
                    <ext:Store ID="StoreSucursal" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="id_sucursal">
                                <Fields>
                                    <ext:RecordField Name="id_sucursal" />
                                    <ext:RecordField Name="clave" />
                                    <ext:RecordField Name="nombre" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="nombre"  Direction="ASC"  />
                    </ext:Store>
                    <ext:Store ID="StoreTipoServicio" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="id">
                                <Fields>
                                    <ext:RecordField Name="id" />
                                    <ext:RecordField Name="nombre" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="nombre" Direction="ASC" />
                    </ext:Store>
                </Content>
                <Items>
                    <ext:Panel ID="Panel1" runat="server" Layout="FitLayout" Border="false">
                        <Items>                            
                           <ext:DateField ID="datInicio" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                AllowBlank="false" MsgTarget="Side" Format="yyyy-MM-dd" MaxLength="12" Width="300"
                                EnableKeyEvents="true" MaxWidth="300">
                                <CustomConfig>
                                    <ext:ConfigItem Name="endDateField" Value="#{datFinal}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:DateField ID="datFinal" runat="server" Vtype="daterange" FieldLabel="Fecha Final" 
                                AllowBlank="false" MsgTarget="Side" Format="yyyy-MM-dd" MaxLength="12" Width="300"
                                EnableKeyEvents="true">
                                <CustomConfig>
                                    <ext:ConfigItem Name="startDateField" Value="#{datInicio}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:ComboBox ID="cmbSucursal" runat="server" FieldLabel="Sucursal" Resizable="true"
                                ListWidth="350" Width="300" StoreID="StoreSucursal" AllowBlank="false"
                                DisplayField="nombre" ValueField="id_sucursal" />                                
                            <ext:ComboBox ID="cmbTipoServicio" runat="server" FieldLabel="Tipo de servicio" EmptyText="Todos"
                                Resizable="true" ListWidth="350" Width="300" StoreID="StoreTipoServicio"
                                DisplayField="nombre" ValueField="id">
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="0" />
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
                            <ext:Button ID="btnBuscarHide" runat="server" Hidden="true">
                                <Listeners>
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Pedidos...' });
                                        #{GridPanelPedidos}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000" Before="var valid= #{FormPanel1}.getForm().isValid();
                                        if (!valid) {} else { resetToolbar(#{PagingToolBar1});
                                        #{GridPanelPedidos}.getStore().sortInfo = null; } return valid;">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                            RepPedidosMoshi.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>
            </ext:FormPanel>
        </West>
        <Center Split="true" Collapsible="false">
            <ext:Panel ID="Panel2" runat="server" Title="Pedidos Obtenidos con los Filtros Seleccionados"
                Collapsed="false" Layout="Fit" AutoScroll="true">
                <Content>
                    <ext:Store ID="StorePedidos" runat="server" OnSubmitData="StoreSubmit" RemoteSort="true"
                        OnRefreshData="StorePedidos_RefreshData" AutoLoad="false">
                        <AutoLoadParams>
                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                        </AutoLoadParams>
                        <Proxy>
                            <ext:PageProxy />
                        </Proxy>
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="IdPedido">
                                <Fields>
                                    <ext:RecordField Name="IdPedido" /> 
                                    <ext:RecordField Name="FechaPedido" /> 
                                    <ext:RecordField Name="NombreCliente" /> 
                                    <ext:RecordField Name="Email" />
                                    <ext:RecordField Name="TipoServicio" />                                                                                                    
                                    <ext:RecordField Name="Sucursal" />
                                    <ext:RecordField Name="Ticket" />
                                    <ext:RecordField Name="Importe" />
                                    <ext:RecordField Name="CantidadProductos" />
                                    <ext:RecordField Name="AliasDireccion" />
                                    <ext:RecordField Name="MontoTDC" />
                                    <ext:RecordField Name="MontoEfectivo" />
                                    <ext:RecordField Name="PagoPts" />
                                    <ext:RecordField Name="PtsGenerados" DefaultValue="0" />
                                    <ext:RecordField Name="Descuento" />
                                    <ext:RecordField Name="Codigo" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <DirectEventConfig IsUpload="true" />
                    </ext:Store>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanelPedidos" runat="server" StoreID="StorePedidos" StripeRows="true"
                                Header="false" Border="false">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel1"  runat="server" >
                                    <Columns>                                        
                                        <ext:Column ColumnID="IdPedido" Header="Id Pedido" Sortable="true" DataIndex="IdPedido" />
                                        <ext:DateColumn ColumnID="FechaPedido" Header="Fecha Pedido" Sortable="false" DataIndex="FechaPedido" Format="yyyy-MM-dd" />
                                        <ext:Column ColumnID="NombreCliente" Header="Nombre" Sortable="true" DataIndex="NombreCliente" />
                                        <ext:Column ColumnID="Email" Header="Correo" Sortable="true" DataIndex="Email" />
                                        <ext:Column ColumnID="TipoServicio" Header="Tipo de Servicio" Sortable="true" DataIndex="TipoServicio" />
                                        <ext:Column ColumnID="Sucursal" Header="Sucursal" Sortable="true" DataIndex="Sucursal" />
                                        <ext:Column ColumnID="Ticket" Header="Ticket" Sortable="true" DataIndex="Ticket" />
                                        <ext:Column ColumnID="Importe" Header="Importe" Sortable="true" DataIndex="Importe" Align="Right" >
                                            <Renderer Format="UsMoney" />
                                        </ext:Column>
                                        <ext:Column ColumnID="CantidadProductos" Header="No. Productos" Sortable="true" DataIndex="CantidadProductos" />
                                        <ext:Column ColumnID="AliasDireccion" Header="Alias Direccion" Sortable="true" DataIndex="AliasDireccion" />
                                        <ext:Column ColumnID="MontoEfectivo" Header="Pago Efectivo" Sortable="true" DataIndex="MontoEfectivo" Align="Right">
                                            <Renderer Format="UsMoney" />
                                        </ext:Column>
                                        <ext:Column ColumnID="MontoTDC" Header="Pago Tarjeta" Sortable="true" DataIndex="MontoTDC" Align="Right">
                                            <Renderer Format="UsMoney" />
                                        </ext:Column>
                                        <ext:Column ColumnID="PagoPts" Header="Pago Pts" Sortable="true" DataIndex="PagoPts" Align="Right" />
                                        <ext:Column ColumnID="PtsGenerados" Header="Ptos Gen" Sortable="true" DataIndex="PtsGenerados"  EmptyGroupText="1" Align="Right" />
                                        <ext:Column ColumnID="Descuento" Header="Descuento" Sortable="true" DataIndex="Descuento" Align="Right">
                                            <Renderer Handler="return (value).formatMoney(2);" />
                                        </ext:Column>
                                        <ext:Column ColumnID="Codigo" Header="Código" Sortable="true" DataIndex="Codigo" Align="Right" />
                                    </Columns>
                                </ColumnModel>
                                <Plugins>
                                    <ext:GridFilters runat="server" ID="GridFilters1" Local="true">
                                        <Filters>
                                            <ext:DateFilter DataIndex="FechaPedido">
                                                <DatePickerOptions runat="server" TodayText="Hoy" />
                                            </ext:DateFilter>
                                        </Filters>
                                    </ext:GridFilters>
                                </Plugins>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StorePedidos" DisplayInfo="true"
                                        DisplayMsg="Mostrando Reporte {0} - {1} de {2}" />
                                </BottomBar>
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar2" runat="server">
                                        <Items>
                                            <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                                            <ext:Button ID="btnExportXML" runat="server" Text="Exportar a XML" Icon="PageCode" Disabled="true">
                                                <Listeners>
                                                    <Click Handler="submitValue(#{GridPanelPedidos}, #{FormatType}, 'xml');" />
                                                </Listeners>
                                            </ext:Button>
                                            <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="true">
                                                <DirectEvents>
                                                    <Click OnEvent="Download" IsUpload="true"
                                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                            e.stopEvent(); 
                                                            RepPedidosMoshi.StopMask();" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="btnExportCSV" runat="server" Text="Exportar a CSV" Icon="PageAttach" Disabled="true">
                                                <Listeners>
                                                    <Click Handler="submitValue(#{GridPanelPedidos}, #{FormatType}, 'csv');" />
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

