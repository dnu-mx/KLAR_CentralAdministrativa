<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="Reporte_PromocionesLoyalty.aspx.cs" 
    Inherits="TpvWeb.Reporte_PromocionesLoyalty" %>

<%@ Import Namespace="System.Xml.Xsl" %>
<%@ Import Namespace="System.Xml" %>
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


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true" Collapsible="true">
            <ext:FormPanel ID="FormPanelFiltros" Width="350" Title="Selecciona los Filtros" runat="server"
                Border="false" Layout="Fit">
                <Content>
                    <ext:Hidden ID="FormatType" runat="server" />
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
                        <SortInfo Field="NombreORazonSocial"  Direction="ASC"  />
                    </ext:Store>
                    <ext:Store ID="StoreSucursal" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                                <Fields>
                                    <ext:RecordField Name="NombreORazonSocial" />
                                    <ext:RecordField Name="ClaveColectiva" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="NombreORazonSocial"  Direction="ASC"  />
                    </ext:Store>
                    <ext:Store ID="StoreOperador" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                                <Fields>
                                    <ext:RecordField Name="ClaveColectiva" />
                                    <ext:RecordField Name="Nombre" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Content>
                <Items>
                    <ext:Panel ID="Panel1" runat="server" Layout="FitLayout" Padding="10">
                        <Items>
                            <ext:DateField ID="datInicio" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                AllowBlank="false" MsgTarget="Side" Format="yyyy-MM-dd" TabIndex="1"
                                MaxLength="12" Width="300" EnableKeyEvents="true" MaxWidth="300"  >
                                <CustomConfig>
                                    <ext:ConfigItem Name="endDateField" Value="#{datFinal}" Mode="Value" />
                                   </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:DateField ID="datFinal" runat="server" Vtype="daterange" FieldLabel="Fecha Final" 
                                AllowBlank="false" MsgTarget="Side" Format="yyyy-MM-dd" TabIndex="2"
                                MaxLength="12" Width="300" EnableKeyEvents="true">
                                <CustomConfig>
                                    <ext:ConfigItem Name="startDateField" Value="#{datInicio}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:ComboBox ID="cmbCadena" runat="server" FieldLabel="Cadena" EmptyText="Todas"
                                TabIndex="3" Resizable="true" ListWidth="350" Width="300" StoreID="StoreCadena"
                                DisplayField="NombreORazonSocial" ValueField="ID_Colectiva" Mode="Local" AutoSelect="true"
                                Editable="true" ForceSelection="true" TypeAhead="true" MinChars="1" MatchFieldWidth="false"
                                Name="colCadena">
                                <DirectEvents>
                                    <Select OnEvent="LlenaSucursales">
                                    </Select>
                                </DirectEvents>
                                <Items>
                                    <ext:ListItem Text="( Todas )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cmbSucursal" runat="server" FieldLabel="Sucursal" EmptyText="Todas"
                                TabIndex="4" Resizable="true" ListWidth="350" Width="300" StoreID="StoreSucursal"
                                DisplayField="NombreORazonSocial" ValueField="ClaveColectiva">
                                <DirectEvents>
                                    <Select OnEvent="LlenaOperadores">
                                    </Select>
                                </DirectEvents>
                                <Items>
                                    <ext:ListItem Text="( Todas )" Value="" />
                                </Items>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cmbOperador" runat="server" FieldLabel="Operador" EmptyText="Todos"
                                TabIndex="5" Resizable="true" ListWidth="350" Width="300" StoreID="StoreOperador"
                                DisplayField="Nombre" ValueField="ClaveColectiva">
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="" />
                                </Items>
                            </ext:ComboBox>
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
                                    <Click OnEvent="btnBuscar_Click" Before="var valid= #{FormPanelFiltros}.getForm().isValid(); if (!valid) {} return valid;">
                                        <EventMask ShowMask="true" Msg="Buscando Promociones..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>
            </ext:FormPanel>
        </West>
        <Center Split="true" Collapsible="false">
            <ext:Panel ID="Panel2" runat="server" Title="Promociones Obtenidas con el Filtro Seleccionado"
                Collapsed="false" Layout="Fit" AutoScroll="true">
                <Content>
                    <ext:Store ID="StoreReporte" runat="server" OnSubmitData="StoreReporte_Submit" OnRefreshData="btnBuscar_Click" RemoteSort="true">
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Operacion">
                                <Fields>
                                    <ext:RecordField Name="ID_Operacion" />
                                    <ext:RecordField Name="Fecha" Type="Date" />
                                    <ext:RecordField Name="Promocion" />
                                    <ext:RecordField Name="Vigencia" />
                                    <ext:RecordField Name="OrigenEmision" />
                                    <ext:RecordField Name="Autorizacion" />
                                    <ext:RecordField Name="Operador" />
                                    <ext:RecordField Name="Ticket" />
                                    <ext:RecordField Name="FormaPago" />
                                    <ext:RecordField Name="ClaveCadena" />
                                    <ext:RecordField Name="NombreCadena" />
                                    <ext:RecordField Name="Sucursal" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <DirectEventConfig IsUpload="true" />
                        <SortInfo Field="Fecha" />
                    </ext:Store>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanelReporte" runat="server" StoreID="StoreReporte" StripeRows="true"
                                Header="false" Border="false">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                        <ext:DateColumn ColumnID="colFecha" Header="Fecha" Sortable="true" DataIndex="Fecha"
                                            Format="yyyy-MM-dd" Width="75" />
                                        <ext:DateColumn ColumnID="colhora" Header="Hora" Sortable="true" DataIndex="Fecha"
                                            Format="HH:mm:ss" Width="65" />
                                        <ext:Column ColumnID="Promocion" Header="Promoción" Sortable="true" DataIndex="Promocion"
                                            Width="200" />
                                        <ext:Column ColumnID="Vigencia" Header="Vigencia" Sortable="true" DataIndex="Vigencia" 
                                            Width="75" />
                                        <ext:Column ColumnID="OrigenEmision" Header="Origen de Emisión" Sortable="true" DataIndex="OrigenEmision"
                                            Width="120" />
                                        <ext:Column ColumnID="Autorizacion" Header="Autorización" Sortable="true" DataIndex="Autorizacion" 
                                            Width="80" />
                                        <ext:Column ColumnID="Operador" Header="Operador" Sortable="true" DataIndex="Operador"
                                            Width="80" />
                                        <ext:Column ColumnID="Ticket" Header="Ticket" Sortable="true" DataIndex="Ticket" />
                                        <ext:Column ColumnID="FormaPago" Header="Forma de Pago" Sortable="true" DataIndex="FormaPago" />
                                        <ext:Column ColumnID="ClaveCadena" Header="Clave de la Cadena" Sortable="true" DataIndex="ClaveCadena"
                                            Width="120" />
                                        <ext:Column ColumnID="NombreCadena" Header="Nombre de la Cadena" Sortable="true" DataIndex="NombreCadena"
                                            Width="120" />
                                        <ext:Column ColumnID="Sucursal" Header="Sucursal" Sortable="true" DataIndex="Sucursal"
                                            Width="80" />
                                    </Columns>
                                </ColumnModel>
                                <Plugins>
                                    <ext:GridFilters runat="server" ID="GridFilters1" Local="true">
                                        <Filters>
                                            <ext:DateFilter DataIndex="Fecha">
                                                <DatePickerOptions runat="server" TodayText="Hoy" />
                                            </ext:DateFilter>
                                        </Filters>
                                    </ext:GridFilters>
                                </Plugins>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreReporte" DisplayInfo="true"
                                        DisplayMsg="Mostrando Promociones {0} - {1} de {2}" />
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
