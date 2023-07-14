<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReporteRedenciones.aspx.cs"
    Inherits="Lealtad.ReporteRedenciones" %>

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
    </script>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true" Collapsible="true">
            <ext:FormPanel ID="FormPanel1" Width="300" Title="Selecciona los Filtros" runat="server"
                Border="false" Layout="FitLayout">
                <Content>
                    <ext:Hidden ID="FormatType" runat="server" />
                    <ext:Store ID="StoreEmisor" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                                <Fields>
                                    <ext:RecordField Name="ID_Colectiva" />
                                    <ext:RecordField Name="ClaveColectiva" />
                                    <ext:RecordField Name="NombreORazonSocial" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="NombreORazonSocial" Direction="ASC" />
                    </ext:Store>
                    <ext:Store ID="StoreCadena" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                                <Fields>
                                    <ext:RecordField Name="ID_Colectiva" />
                                    <ext:RecordField Name="ClaveColectiva" />
                                    <ext:RecordField Name="NombreORazonSocial" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="NombreORazonSocial" Direction="ASC" />
                    </ext:Store>
                    <ext:Store ID="StoreSucursal" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                                <Fields>
                                    <ext:RecordField Name="ID_Colectiva" />
                                    <ext:RecordField Name="ClaveColectiva" />
                                    <ext:RecordField Name="NombreORazonSocial" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="NombreORazonSocial" Direction="ASC" />
                    </ext:Store>
                    <ext:Store ID="StoreEstados" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="CveEstado">
                                <Fields>
                                    <ext:RecordField Name="CveEstado" />
                                    <ext:RecordField Name="Estado" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:Store ID="StoreCiudades" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID">
                                <Fields>
                                    <ext:RecordField Name="Ciudad" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Content>
                <Items>
                    <ext:Panel ID="Panel1" runat="server" Border="false" Padding="10">
                        <Items>
                            <ext:ComboBox ID="cBoxEmisor" runat="server" FieldLabel="Emisor" Width="280" EmptyText="Todos"
                                StoreID="StoreEmisor" DisplayField="NombreORazonSocial" ValueField="ClaveColectiva">
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="" />
                                </Items>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cBoxCadena" runat="server" FieldLabel="Cadena" ListWidth="350" Width="280"
                                EmptyText="Todas" StoreID="StoreCadena" DisplayField="NombreORazonSocial" ValueField="ID_Colectiva"
                                Mode="Local" AutoSelect="true" Editable="true" ForceSelection="true" TypeAhead="true" MinChars="1"
                                MatchFieldWidth="false" Name="cadenaComercial">
                                <Items>
                                    <ext:ListItem Text="( Todas )" Value="-1" />
                                </Items>
                                <DirectEvents>
                                    <Select OnEvent="ObtieneSucursales" Before="#{cBoxSucursal}.clearValue();">
                                        <EventMask ShowMask="true" Msg="Buscando Sucursales..." MinDelay="200" />
                                    </Select>
                                </DirectEvents>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cBoxSucursal" runat="server" FieldLabel="Sucursal" ListWidth="350" Width="280"
                                EmptyText="Todas" StoreID="StoreSucursal" DisplayField="NombreORazonSocial" ValueField="ID_Colectiva"
                                Mode="Local" AutoSelect="true" Editable="true" ForceSelection="true" TypeAhead="true" MinChars="1"
                                MatchFieldWidth="false" Name="sucursal">
                                <Items>
                                    <ext:ListItem Text="( Todas )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cBoxEstado" runat="server" FieldLabel="Estado" Width="280" ListWidth="350"
                                EmptyText="Todos" StoreID="StoreEstados" DisplayField="Estado" ValueField="CveEstado">
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="" />
                                </Items>
                                <DirectEvents>
                                    <Select OnEvent="BuscaCiudades" Before="#{cBoxCiudad}.clearValue(); #{cBoxCiudad}.setDisabled(false);">
                                        <EventMask ShowMask="true" Msg="Buscando Ciudades..." MinDelay="100" />
                                    </Select>
                                </DirectEvents>
                            </ext:ComboBox>
                             <ext:ComboBox ID="cBoxCiudad" runat="server" FieldLabel="Ciudad" Width="280" ListWidth="350"
                                EmptyText="Todos" StoreID="StoreCiudades" DisplayField="Ciudad" ValueField="Ciudad"
                                 Disabled="true">
                                <Items>
                                    <ext:ListItem Text="( Todas )" Value="" />
                                </Items>
                            </ext:ComboBox>
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
                                        <EventMask ShowMask="true" Msg="Obteniendo Redenciones..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>
            </ext:FormPanel>
        </West>
        <Center Split="true" Collapsible="false">
            <ext:FormPanel ID="FormPanelRedenciones" runat="server" Layout="FitLayout" Title="Redenciones obtenidas con los Filtros">
                <Items>
                    <ext:GridPanel ID="GridPanelRedenciones" runat="server" StripeRows="true" Layout="FitLayout" Region="Center">
                        <Store>
                            <ext:Store ID="StoreRedenciones" runat="server"  OnRefreshData="btnBuscar_Click"
                                OnSubmitData="StoreSubmit">
                                <DirectEventConfig IsUpload="true"/>
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_Operacion">
                                        <Fields>
                                            <ext:RecordField Name="ID_Operacion" />
                                            <ext:RecordField Name="Emisor" />
                                            <ext:RecordField Name="Cadena" />
                                            <ext:RecordField Name="Sucursal" />
                                            <ext:RecordField Name="Estado" />
                                            <ext:RecordField Name="Ciudad" />
                                            <ext:RecordField Name="Promocion" />
                                            <ext:RecordField Name="Operador" />
                                            <ext:RecordField Name="FechaRedencion" />
                                            <ext:RecordField Name="CodigoCupon" />
                                            <ext:RecordField Name="Autorizacion" />
                                            <ext:RecordField Name="FormaPago" />
                                            <ext:RecordField Name="TicketVenta" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <ColumnModel ID="ColumnModel1" runat="server">
                            <Columns>
                                <ext:Column ColumnID="ID_Operacion" Hidden="true" DataIndex="ID_Operacion"/>
                                <ext:Column ColumnID="Emisor" Header="Emisor" Sortable="true" DataIndex="Emisor" Width="100" />
                                <ext:Column ColumnID="Cadena" Header="Cadena" Sortable="true" DataIndex="Cadena" Width="150" />
                                <ext:Column ColumnID="Sucursal" Header="Sucursal" Sortable="true" DataIndex="Sucursal"
                                    Width="120" />
                                <ext:Column ColumnID="Ciudad" Header="Ciudad" Sortable="true" DataIndex="Ciudad" Width="120" />
                                <ext:Column ColumnID="Estado" Header="Estado" Sortable="true" DataIndex="Estado" Width="100" />
                                <ext:Column ColumnID="Promocion" Header="Promoción" Sortable="true" DataIndex="Promocion"
                                    Width="150"/>
                                <ext:Column ColumnID="Operador" Header="Operador" Sortable="true" DataIndex="Operador"
                                    Width="150" />
                                <ext:DateColumn ColumnID="FechaRedencion" Header="Fecha de Redención" Sortable="true"
                                    DataIndex="FechaRedencion" Format="dd/MM/yyyy" Width="100"/>
                                <ext:Column ColumnID="CodigoCupon" Header="Código de Redención" Sortable="true" 
                                    DataIndex="CodigoCupon" Width="150" />
                                <ext:Column ColumnID="Autorizacion" Header="No. Autorización" Sortable="true"
                                    DataIndex="Autorizacion" />
                                <ext:Column ColumnID="FormaPago" Header="Forma de Pago" Sortable="true" DataIndex="FormaPago"
                                    Width="100" />
                                <ext:Column ColumnID="TicketVenta" Header="Ticket de Venta" Sortable="true" DataIndex="TicketVenta"
                                    Width="100" />
                            </Columns>
                        </ColumnModel>
                        <TopBar>
                            <ext:Toolbar ID="Toolbar5" runat="server">
                                <Items>
                                    <ext:ToolbarFill ID="ToolbarFill6" runat="server" />
                                    <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="true">
                                        <DirectEvents>
                                            <Click OnEvent="ExportGridToExcel" IsUpload="true" Success="Ext.Net.DirectMethods.Download({IsUpload:true})"
                                                After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                    e.stopEvent(); 
                                                    Lealtad.StopMask();">
                                                <ExtraParams>
                                                    <ext:Parameter Name="GridResult" Value="Ext.encode(#{GridPanelRedenciones}.getRowsValues({selectedOnly : false}))" Mode="Raw" />
                                                </ExtraParams>
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnExportCSV" runat="server" Text="Exportar a CSV" Icon="PageAttach" Disabled="true">
                                        <Listeners>
                                            <Click Handler="submitValue(#{GridPanelRedenciones}, #{FormatType}, 'csv');"  />
                                        </Listeners>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                        <BottomBar>
                            <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreRedenciones" DisplayInfo="true"
                                DisplayMsg="Mostrando Redenciones {0} - {1} de {2}" />
                        </BottomBar>
                    </ext:GridPanel>
                </Items>
            </ext:FormPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
