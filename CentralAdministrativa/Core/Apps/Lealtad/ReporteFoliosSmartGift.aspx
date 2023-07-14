<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReporteFoliosSmartGift.aspx.cs"
    Inherits="Lealtad.ReporteFoliosSmartGift" %>

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

<asp:Content runat="server" ContentPlaceHolderID="MainContent">       
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true" Collapsible="true">
            <ext:FormPanel ID="FormPanel1" Width="300" Title="Selecciona los Filtros" runat="server"
                Border="false" Layout="FitLayout">
                <Content>
                    <ext:Hidden ID="FormatType" runat="server" />
                </Content>
                <Items>
                    <ext:Panel ID="Panel1" runat="server" Border="false" Padding="10">
                        <Items>
                            <ext:TextField ID="txtNombre" runat="server" FieldLabel="Nombre" Width="280" MaxLengthText="100" />
                            <ext:TextField ID="txtApellido" runat="server" FieldLabel="Apellido" Width="280" MaxLengthText="100" />
                            <ext:TextField ID="txtCorreo" runat="server" FieldLabel="Correo" Width="280" MaxLengthText="100" />
                            <ext:DateField ID="dfFechaInicio" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                MsgTarget="Side" Format="yyyy/MM/dd" Width="280" EnableKeyEvents="true" MaxWidth="280"
                                AllowBlank="false">
                                <CustomConfig>
                                    <ext:ConfigItem Name="endDateField" Value="#{dfFechaFin}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:DateField ID="dfFechaFin" runat="server" Vtype="daterange" FieldLabel="Fecha Final"
                                MaxWidth="280" Width="280" MsgTarget="Side" Format="yyyy/MM/dd" EnableKeyEvents="true"
                                AllowBlank="false">
                                <CustomConfig>
                                    <ext:ConfigItem Name="startDateField" Value="#{dfFechaInicio}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:NumberField ID="nmbPedido" runat="server" FieldLabel="No. Pedido" Width="280" AllowDecimals="false"
                                AllowNegative="false"/>
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
                            <ext:Button ID="btnBuscarHide" runat="server" Hidden="true">
                                <Listeners>
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Folios...' });
                                        #{GridPanelFolios}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                        Before="resetToolbar(#{PagingToolBar1});
                                        #{GridPanelFolios}.getStore().sortInfo = null;">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                            Lealtad.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>
            </ext:FormPanel>
        </West>
        <Center Split="true" Collapsible="false">
            <ext:FormPanel ID="FormPanelFolios" runat="server" Layout="FitLayout" Title="Folios obtenidos con los Filtros">
                <Items>
                    <ext:GridPanel ID="GridPanelFolios" runat="server" StripeRows="true" Layout="FitLayout" Region="Center">
                        <Store>
                            <ext:Store ID="StoreFolios" runat="server" OnSubmitData="StoreSubmit" RemoteSort="true" 
                                OnRefreshData="StoreFolios_RefreshData" AutoLoad="false">
                                <AutoLoadParams>
                                    <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                </AutoLoadParams>
                                <Proxy>
                                    <ext:PageProxy />
                                </Proxy>                                
                                <DirectEventConfig IsUpload="true" />
                                <Reader>
                                    <ext:JsonReader IDProperty="ID">
                                        <Fields>
                                            <ext:RecordField Name="IdPedido" />
                                            <ext:RecordField Name="FechaCreacion" />
                                            <ext:RecordField Name="Nombre" />
                                            <ext:RecordField Name="Apellido" />
                                            <ext:RecordField Name="Correo" />
                                            <ext:RecordField Name="Empresa" />
                                            <ext:RecordField Name="ClaveValidacion" />
                                            <ext:RecordField Name="Total" />
                                            <ext:RecordField Name="SKU" />
                                            <ext:RecordField Name="NombreProducto" />
                                            <ext:RecordField Name="Cantidad" />
                                            <ext:RecordField Name="Precio" />
                                            <ext:RecordField Name="Codigo" />
                                            <ext:RecordField Name="PAN" />
                                            <ext:RecordField Name="IDOperacion" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <ColumnModel ID="ColumnModel1" runat="server">
                            <Columns>
                                <ext:Column ColumnID="IdPedido" Header="ID Pedido" DataIndex="IdPedido" Width="80" Sortable="true"/>
                                <ext:DateColumn ColumnID="FechaCreacion" Header="Fecha de Creación" Sortable="true"
                                    DataIndex="FechaCreacion" Format="dd/MM/yyyy" Width="100"/>
                                <ext:Column ColumnID="Nombre" Header="Nombre" Sortable="true" DataIndex="Nombre" Width="120" />
                                <ext:Column ColumnID="Apellido" Header="Apellido" Sortable="true" DataIndex="Apellido" Width="120" />
                                <ext:Column ColumnID="Correo" Header="Correo" Sortable="true" DataIndex="Correo" Width="120" />
                                <ext:Column ColumnID="Empresa" Header="Empresa" Sortable="true" DataIndex="Empresa" Width="150" />
                                <ext:Column ColumnID="ClaveValidacion" Header="Clave de Validación" Sortable="true" 
                                    DataIndex="ClaveValidacion" Width="120" />
                                <ext:Column ColumnID="Total" Header="Total" Sortable="true" DataIndex="Total" Width="80"/>
                                <ext:Column ColumnID="SKU" Header="SKU" Sortable="true" DataIndex="SKU" Width="80" />
                                <ext:Column ColumnID="NombreProducto" Header="Nombre del Producto" Sortable="true" 
                                    DataIndex="NombreProducto" Width="200" />
                                <ext:Column ColumnID="Cantidad" Header="Cantidad" Sortable="true" DataIndex="Cantidad" />
                                <ext:Column ColumnID="Precio" Header="Precio" Sortable="true" DataIndex="Precio" Width="80">
                                    <Renderer Format="UsMoney" />
                                </ext:Column>
                                <ext:Column ColumnID="Codigo" Header="Código" Sortable="true" DataIndex="Codigo" Width="100" />
                                <ext:Column ColumnID="PAN" Header="PAN" Sortable="true" DataIndex="PAN" Width="80" />
                                <ext:Column ColumnID="IDOperacion" Header="No. Operación" Sortable="true" DataIndex="IDOperacion"
                                    Width="100" />
                            </Columns>
                        </ColumnModel>
                        <TopBar>
                            <ext:Toolbar ID="Toolbar5" runat="server">
                                <Items>
                                    <ext:ToolbarFill ID="ToolbarFill6" runat="server" />
                                    <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="true">
                                        <DirectEvents>
                                            <Click OnEvent="Download" IsUpload="true"
                                                After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                    e.stopEvent(); 
                                                    Lealtad.StopMask();" />
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnExportCSV" runat="server" Text="Exportar a CSV" Icon="PageAttach" Disabled="true">
                                        <Listeners>
                                            <Click Handler="submitValue(#{GridPanelFolios}, #{FormatType}, 'csv');"  />
                                        </Listeners>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                        <BottomBar>
                            <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreFolios" DisplayInfo="true"
                                DisplayMsg="Mostrando Folios {0} - {1} de {2}" />
                        </BottomBar>
                    </ext:GridPanel>
                </Items>
            </ext:FormPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
