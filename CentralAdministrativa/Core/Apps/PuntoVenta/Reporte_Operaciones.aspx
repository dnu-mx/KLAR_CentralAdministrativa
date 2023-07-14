<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Reporte_Operaciones.aspx.cs" Inherits="TpvWeb.Reporte_Operaciones" %>

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
                                    <ext:RecordField Name="NombreORazonSocial" />
                                    <ext:RecordField Name="ClaveColectiva" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="NombreORazonSocial"  Direction="ASC"  />
                    </ext:Store>
                    <ext:Store ID="StoreAfiliacion" runat="server">
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
                    <ext:Store ID="StoreTerminal" runat="server">
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
                      <ext:Store ID="StoreBeneficiario" runat="server">
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
                                    <ext:RecordField Name="NameFin" />
                                    <ext:RecordField Name="ClaveColectiva" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        
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
                    <ext:Panel ID="Panel1" runat="server" Layout="FitLayout" Padding="10">
                        <Items>
                            <%-- <ext:DateField FieldLabel="Fecha Inicial" AllowBlank="false" MsgTarget="Side" Format="yyyy-MM-dd"
                                ID="datInicio" runat="server" EmptyText="Selecciona una Fecha Inicial" MaxWidth="300"
                                Width="300" MaxLength="12" Vtype="daterange" />
                            <ext:DateField FieldLabel="Fecha Final" AllowBlank="false" MsgTarget="Side" Format="yyyy-MM-dd"
                                TabIndex="2" ID="datFinal" runat="server" EmptyText="Selecciona una Fecha Final"
                                Width="300" MaxLength="12" Vtype="daterange" />--%>
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
                            <ext:ComboBox ID="cmbSucursal" TabIndex="3" FieldLabel="Sucursal" EmptyText="Todas"
                                Resizable="true" ListWidth="350" Width="300" runat="server" StoreID="StoreSucursal"
                                DisplayField="NombreORazonSocial" ValueField="ClaveColectiva">
                                <DirectEvents>
                                    <Select OnEvent="LlenaAfiliaciones">
                                    </Select>
                                </DirectEvents>
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cmbAfiliacion" TabIndex="4" FieldLabel="Afiliación" Width="300"
                                Resizable="true" ListWidth="350" EmptyText="Todas" runat="server" StoreID="StoreAfiliacion"
                                DisplayField="NombreORazonSocial" ValueField="ClaveColectiva">
                                <DirectEvents>
                                    <Select OnEvent="LlenaTerminales">
                                    </Select>
                                </DirectEvents>
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cmbTerminal" FieldLabel="Terminal" TabIndex="5" EmptyText="Todas"
                                Resizable="true" ListWidth="350" Width="300" runat="server" StoreID="StoreTerminal"
                                DisplayField="NombreORazonSocial" ValueField="ClaveColectiva">
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cmbOperador" FieldLabel="Operador" TabIndex="6" EmptyText="Todos"
                                Resizable="true" ListWidth="350" Width="300" runat="server" StoreID="StoreOperador"
                                DisplayField="NameFin" ValueField="ClaveColectiva">
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cmbEstatus" FieldLabel="Estatus" StoreID="StoreEstatus" TabIndex="7"
                                EmptyText="Todos" Resizable="true" ListWidth="350" Width="300" runat="server"
                                DisplayField="Descripcion" ValueField="ID_EstatusOperacion">
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                              <ext:ComboBox ID="cmbBeneficiario" FieldLabel="Marca" TabIndex="6" EmptyText="Todos"
                                Resizable="true" ListWidth="350" Width="300" runat="server" StoreID="StoreBeneficiario"
                                DisplayField="NombreORazonSocial" ValueField="ClaveColectiva">
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:TextField ID="txtTelefono" FieldLabel="Referencia" EmptyText="Todos" TabIndex="8"
                                MaxLength="50" Width="300" runat="server" Text="" />
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
                                        <EventMask ShowMask="true" Msg="Buscando Operaciones..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>
            </ext:FormPanel>
        </West>
        <Center Split="true" Collapsible="false">
            <ext:Panel ID="Panel2" runat="server" Title="Operaciones Obtenidas con el Filtro Seleccionado"
                Collapsed="false" Layout="Fit" AutoScroll="true">
                <Content>
                    <ext:Store ID="Store1" runat="server" OnSubmitData="Store1_Submit" OnRefreshData="btnBuscar_Click" RemoteSort="true">
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Operacion">
                                <%--<ext:ArrayReader IDProperty="ID_Operacion">--%>
                                <Fields>
                                    <ext:RecordField Name="Fecha" Type="Date" />
                                    <ext:RecordField Name="ID_Operacion" />
                                    <ext:RecordField Name="Sucursal" />
                                    <ext:RecordField Name="Terminal" />
                                    <ext:RecordField Name="Operador" />
                                    <ext:RecordField Name="Estatus" />
                                    <ext:RecordField Name="Autorizacion" />
                                    <ext:RecordField Name="Beneficiario" />
                                    <ext:RecordField Name="Telefono" />
                                    <ext:RecordField Name="Monto" />
                                    <ext:RecordField Name="Mensualidades" />
                                    <ext:RecordField Name="NombreOperacion" />
                                    <ext:RecordField Name="ID_Cliente" />
                                    <ext:RecordField Name="TarjetaHabiente" />
                                    <ext:RecordField Name="DescripcionPromo" />
                                </Fields>
                                <%--</ext:ArrayReader>--%>
                            </ext:JsonReader>
                        </Reader>
                        <DirectEventConfig IsUpload="true" />
                        <SortInfo Field="Fecha" />
                    </ext:Store>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store1" StripeRows="true"
                                Header="false" Border="false" AutoExpandColumn="Telefono">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                        <ext:DateColumn ColumnID="colFecha" Header="Fecha" Sortable="true" DataIndex="Fecha"
                                            Format="yyyy-MM-dd" />
                                        <ext:DateColumn ColumnID="colhora" Header="Hora" Sortable="true" DataIndex="Fecha"
                                            Format="HH:mm:ss" />
                                        <ext:Column ColumnID="ID_Operacion" Header="Identificador" Sortable="true" DataIndex="ID_Operacion" />
                                        <ext:Column ColumnID="Sucursal" Header="Sucursal" Sortable="true" DataIndex="Sucursal" />
                                        <%--<ext:Column ColumnID="Afiliacion" Header="Afiliacion" Sortable="true" DataIndex="Afiliacion" />--%>
                                        <ext:Column ColumnID="Terminal" Header="Terminal" Sortable="true" DataIndex="Terminal" />
                                        <ext:Column ColumnID="Operador" Header="Operador" Sortable="true" DataIndex="Operador" />
                                        <ext:Column ColumnID="Estatus" Header="Estatus" Sortable="true" DataIndex="Estatus" />
                                        <ext:Column ColumnID="Beneficiario" Header="Marca" Sortable="true" DataIndex="Beneficiario" />
                                        <ext:Column ColumnID="Autorizacion" Header="Autorización" Sortable="true" DataIndex="Autorizacion" />
                                        <ext:Column ColumnID="Telefono" Header="Referencia" Sortable="true" DataIndex="Telefono" />
                                        <ext:Column ColumnID="Monto" Header="Monto" Sortable="true" DataIndex="Monto">
                                            <Renderer Format="UsMoney" />
                                        </ext:Column>
                                         <ext:Column ColumnID="Mensualidades" Header="Mensualidades" Sortable="true" DataIndex="Mensualidades" />
                                         <ext:Column ColumnID="DescripcionPromo" Header="Promoción" Sortable="true" DataIndex="DescripcionPromo" />
                                         <ext:Column ColumnID="NombreOperacion" Header="Operacion" Sortable="true" DataIndex="NombreOperacion" />
                                         <ext:Column ColumnID="ID_Cliente" Header="ID_Cliente" Sortable="true" DataIndex="ID_Cliente" />
                                         <ext:Column ColumnID="TarjetaHabiente" Header="TarjetaHabiente" Sortable="true" DataIndex="TarjetaHabiente" />
                                    </Columns>
                                </ColumnModel>
                                <Plugins>
                                    <ext:GridFilters runat="server" ID="GridFilters1" Local="true">
                                        <Filters>
                                         <ext:StringFilter DataIndex="NombreOperacion" />
                                        <ext:StringFilter DataIndex="ID_Cliente" />
                                        <ext:StringFilter DataIndex="TarjetaHabiente" />
                                            <ext:NumericFilter DataIndex="ID_Operacion" />
                                            <ext:StringFilter DataIndex="Sucursal" />
                                            <ext:StringFilter DataIndex="Afiliacion" />
                                            <ext:StringFilter DataIndex="Terminal" />
                                            <ext:StringFilter DataIndex="Operador" />
                                            <ext:StringFilter DataIndex="Autorizacion" />
                                            <ext:StringFilter DataIndex="Beneficiario" />
                                            <ext:StringFilter DataIndex="Telefono" />
                                            <ext:NumericFilter DataIndex="Monto" />
                                            <ext:DateFilter DataIndex="Fecha">
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
                                            <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" Icon="PageExcel">
                                                <DirectEvents>
                                                    <Click OnEvent="ExportGridToExcel" IsUpload="true" Success="Ext.Net.DirectMethods.Download({IsUpload:true})">
                                                        <ExtraParams>
                                                            <ext:Parameter Name="GridToExport" Value="Ext.encode(#{GridPanel1}.getRowsValues({selectedOnly : false}))" Mode="Raw" />
                                                        </ExtraParams>
                                                    </Click>
                                                </DirectEvents>
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
