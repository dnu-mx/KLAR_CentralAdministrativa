<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Reporte_CobrosConTarjetaDiconsa.aspx.cs" Inherits="TpvWeb.Reporte_CobrosConTarjetaDiconsa" %>

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
            <ext:FormPanel ID="FormPanel1" Width="350" Title="Selecciona los Filtros" runat="server"
                Border="false" Layout="Fit">
                <Content>
                    <ext:Hidden ID="FormatType" runat="server" />
                    <ext:Store ID="StoreSucursal" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                                <Fields>
                                    <ext:RecordField Name="ID_Colectiva" />
                                    <ext:RecordField Name="Nombre" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="Nombre"  Direction="ASC"  />
                    </ext:Store>
                    <ext:Store ID="StoreUnidadOper" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                                <Fields>
                                    <ext:RecordField Name="ID_Colectiva" />
                                    <ext:RecordField Name="Nombre" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="Nombre"  Direction="ASC"  />
                    </ext:Store>
                    <ext:Store ID="StoreAlmacen" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                                <Fields>
                                    <ext:RecordField Name="ID_Colectiva" />
                                    <ext:RecordField Name="Nombre" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="Nombre"  Direction="ASC"  />
                    </ext:Store>
                    <ext:Store ID="StoreTienda" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                                <Fields>
                                    <ext:RecordField Name="ID_Colectiva" />
                                    <ext:RecordField Name="NombreTienda" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="NombreTienda"  Direction="ASC"  />
                    </ext:Store>
                      <ext:Store ID="StoreTipoTarjeta" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_TipoTarjeta">
                                <Fields>
                                    <ext:RecordField Name="ID_TipoTarjeta" />
                                    <ext:RecordField Name="ClaveTipoTarjeta" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="ID_TipoTarjeta"  Direction="ASC"  />
                    </ext:Store>
                    <ext:Store ID="StoreEstatus" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_EstatusOperacion">
                                <Fields>
                                    <ext:RecordField Name="ID_EstatusOperacion" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="Descripcion"  Direction="ASC"  />
                    </ext:Store>
                </Content>
                <Items>
                    <ext:Panel ID="Panel1" runat="server" Layout="FitLayout">
                        <Items>
                            <ext:DateField ID="datInicio" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                AllowBlank="false" MsgTarget="Side" Format="yyyy-MM-dd"
                                TabIndex="1"  MaxLength="12" Width="300" EnableKeyEvents="true" MaxWidth="300"  >
                                <CustomConfig>
                                    <ext:ConfigItem Name="endDateField" Value="#{datFinal}" Mode="Value" />
                                   </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:DateField ID="datFinal" runat="server" Vtype="daterange" FieldLabel="Fecha Final" 
                                AllowBlank="false"  MaxLength="12" Width="300" MsgTarget="Side" Format="yyyy-MM-dd" TabIndex="2" EnableKeyEvents="true">
                                <CustomConfig>
                                    <ext:ConfigItem Name="startDateField" Value="#{datInicio}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:ComboBox ID="cmbSucursal" TabIndex="4" FieldLabel="Sucursal" EmptyText="Todas"
                                Resizable="true" ListWidth="350" Width="300" runat="server" StoreID="StoreSucursal"
                                DisplayField="Nombre" ValueField="ID_Colectiva">
                                <DirectEvents>
                                    <Select OnEvent="LlenaUnidades">
                                    </Select>
                                </DirectEvents>
                                <Items>
                                    <ext:ListItem Text="( Todas )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cmbUnidadOperativa" TabIndex="5" FieldLabel="Unidad Operativa" Width="300"
                                Resizable="true" ListWidth="350" EmptyText="Todas" runat="server" StoreID="StoreUnidadOper"
                                DisplayField="Nombre" ValueField="ID_Colectiva">
                                <DirectEvents>
                                    <Select OnEvent="LlenaAlmacenes">
                                    </Select>
                                </DirectEvents>
                                <Items>
                                    <ext:ListItem Text="( Todas )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cmbAlmacen" FieldLabel="Almacén" TabIndex="6" EmptyText="Todos"
                                Resizable="true" ListWidth="350" Width="300" runat="server" StoreID="StoreAlmacen"
                                DisplayField="Nombre" ValueField="ID_Colectiva">
                                <DirectEvents>
                                    <Select OnEvent="LlenaTiendas">
                                    </Select>
                                </DirectEvents>
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cmbTienda" TabIndex="3" FieldLabel="Tienda" EmptyText="Todas"
                                Resizable="true" ListWidth="350" Width="300" runat="server" StoreID="StoreTienda"
                                DisplayField="NombreTienda" ValueField="ID_Colectiva">
                                 <Items>
                                    <ext:ListItem Text="( Todas )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cmbEstatus" FieldLabel="Estatus" StoreID="StoreEstatus" TabIndex="8"
                                EmptyText="Todos" Resizable="true" ListWidth="350" Width="300" runat="server"
                                DisplayField="Descripcion" ValueField="ID_EstatusOperacion">
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                              <ext:ComboBox ID="cmbTipoTarjeta" FieldLabel="Tipo de Tarjeta" TabIndex="9" EmptyText="Todos"
                                Resizable="true" ListWidth="350" Width="300" runat="server" StoreID="StoreTipoTarjeta"
                                DisplayField="Descripcion" ValueField="ID_TipoTarjeta">
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:TextField ID="txtTarjeta" FieldLabel="Últimos 4 Dígitos Tarjeta" EmptyText="Todas" TabIndex="10"
                                MinLength="4" MaxLength="4" Width="300" runat="server" AllowBlank="true" />
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
                                        <EventMask ShowMask="true" Msg="Obteniendo Cobros con Tarjeta..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>
            </ext:FormPanel>
        </West>
        <Center Split="true" Collapsible="false">
            <ext:Panel ID="Panel2" runat="server" Title="Cobros Obtenidos con el Filtro Seleccionado"
                Collapsed="false" Layout="Fit" AutoScroll="true">
                <Content>
                    <ext:Store ID="Store1" runat="server" OnSubmitData="Store1_Submit" OnRefreshData="btnBuscar_Click" RemoteSort="true">
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Operacion">
                                <Fields>
                                    <ext:RecordField Name="FechaHora" Type="Date" />
                                    <ext:RecordField Name="ID_Operacion" />
                                    <ext:RecordField Name="Sucursal" />
                                    <ext:RecordField Name="UnidadOperativa" />
                                    <ext:RecordField Name="Almacen" />
                                    <ext:RecordField Name="Tienda" />
                                    <ext:RecordField Name="Estatus" />
                                    <ext:RecordField Name="CodRespuesta" />
                                    <ext:RecordField Name="Autorizacion" />
                                    <ext:RecordField Name="Marca" />
                                    <ext:RecordField Name="Referencia" />
                                    <ext:RecordField Name="Monto" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <DirectEventConfig IsUpload="true" />
                        <SortInfo Field="FechaHora" />
                    </ext:Store>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store1" StripeRows="true"
                                Header="false" Border="false" AutoExpandColumn="Tarjeta">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                        <ext:DateColumn ColumnID="colFecha" Header="Fecha" Sortable="true" DataIndex="FechaHora"
                                            Format="yyyy-MM-dd" />
                                        <ext:DateColumn ColumnID="colhora" Header="Hora" Sortable="true" DataIndex="FechaHora"
                                            Format="HH:mm:ss" />
                                        <ext:Column ColumnID="ID_Operacion" Header="Identificador" Sortable="true" DataIndex="ID_Operacion" />
                                        <ext:Column ColumnID="Sucursal" Header="Sucursal" Sortable="true" DataIndex="Sucursal" />
                                        <ext:Column ColumnID="UnidadOperativa" Header="Unidad Operativa" Sortable="true" DataIndex="UnidadOperativa" />
                                        <ext:Column ColumnID="Almacen" Header="Almacén" Sortable="true" DataIndex="Almacen" />
                                        <ext:Column ColumnID="Tienda" Header="Tienda" Sortable="true" DataIndex="Tienda" />
                                        <ext:Column ColumnID="Estatus" Header="Estatus" Sortable="true" DataIndex="Estatus" />
                                        <ext:Column ColumnID="TipoTarjeta" Header="Tipo de Tarjeta" Sortable="true" DataIndex="Marca" />
                                        <ext:Column ColumnID="CodRespuesta" Header="Codigo Respuesta" Sortable="true" DataIndex="CodRespuesta" />
                                        <ext:Column ColumnID="Autorizacion" Header="Autorización" Sortable="true" DataIndex="Autorizacion" />
                                        <ext:Column ColumnID="Tarjeta" Header="Tarjeta" Sortable="true" DataIndex="Referencia" />
                                        <ext:Column ColumnID="Monto" Header="Monto" Sortable="true" DataIndex="Monto">
                                            <Renderer Format="UsMoney" />
                                        </ext:Column>
                                    </Columns>
                                </ColumnModel>
                                <Plugins>
                                    <ext:GridFilters runat="server" ID="GridFilters1" Local="true">
                                        <Filters>
                                            <ext:NumericFilter DataIndex="ID_Operacion" />
                                            <ext:StringFilter DataIndex="Sucursal" />
                                            <ext:StringFilter DataIndex="UnidadOperativa" />
                                            <ext:StringFilter DataIndex="Almacen" />
                                            <ext:StringFilter DataIndex="Autorizacion" />
                                            <ext:StringFilter DataIndex="TipoTarjeta" />
                                            <ext:StringFilter DataIndex="Tarjeta" />
                                            <ext:NumericFilter DataIndex="Monto" />
                                            <ext:DateFilter DataIndex="FechaHora">
                                                <DatePickerOptions runat="server" TodayText="Hoy" />
                                            </ext:DateFilter>
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

