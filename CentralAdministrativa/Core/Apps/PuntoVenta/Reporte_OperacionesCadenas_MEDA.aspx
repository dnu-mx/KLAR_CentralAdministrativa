<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Reporte_OperacionesCadenas_MEDA.aspx.cs" Inherits="TpvWeb.Reporte_OperacionesCadenas_MEDA" %>

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
                    <ext:Store ID="StoreSucursal" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                                <Fields>
                                    <ext:RecordField Name="NombreORazonSocial" />
                                    <ext:RecordField Name="ClaveColectiva" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="NombreORazonSocial" Direction="ASC" />
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
                        <SortInfo Field="NombreORazonSocial" Direction="ASC" />
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
                        <SortInfo Field="NombreORazonSocial" Direction="ASC" />
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
                        <SortInfo Field="NombreORazonSocial" Direction="ASC" />
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
                        <SortInfo Field="Descripcion" Direction="ASC" />
                    </ext:Store>
                </Content>
                <Items>
                    <ext:Panel ID="Panel1" runat="server" Layout="FitLayout">
                        <Items>
                            <%-- <ext:DateField FieldLabel="Fecha Inicial" AllowBlank="false" MsgTarget="Side" Format="yyyy-MM-dd"
                                ID="datInicio" runat="server" EmptyText="Selecciona una Fecha Inicial" MaxWidth="300"
                                Width="300" MaxLength="12" Vtype="daterange" />
                            <ext:DateField FieldLabel="Fecha Final" AllowBlank="false" MsgTarget="Side" Format="yyyy-MM-dd"
                                TabIndex="2" ID="datFinal" runat="server" EmptyText="Selecciona una Fecha Final"
                                Width="300" MaxLength="12" Vtype="daterange" />--%>
                            <ext:DateField ID="datInicio" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                AllowBlank="false" MsgTarget="Side" Format="yyyy-MM-dd"
                                TabIndex="1" MaxLength="12" Width="300" EnableKeyEvents="true" MaxWidth="300">
                                <CustomConfig>
                                    <ext:ConfigItem Name="endDateField" Value="#{datFinal}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:DateField ID="datFinal" runat="server" Vtype="daterange" FieldLabel="Fecha Final"
                                AllowBlank="false" MaxLength="12" Width="300" MsgTarget="Side" Format="yyyy-MM-dd" TabIndex="2" EnableKeyEvents="true">
                                <CustomConfig>
                                    <ext:ConfigItem Name="startDateField" Value="#{datInicio}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:ComboBox ID="cmbCadenaComercial" TabIndex="3" FieldLabel="Cadena Comercial" EmptyText="Todas"
                                Resizable="true" ListWidth="350" Width="300" runat="server" StoreID="StoreCadenaComercial"
                                DisplayField="NombreORazonSocial" ValueField="ID_Colectiva">
                                <DirectEvents>
                                    <Select OnEvent="LlenaSucursales">
                                    </Select>
                                </DirectEvents>
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cmbSucursal" TabIndex="4" FieldLabel="Sucursal" EmptyText="Todas"
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
                            <ext:ComboBox ID="cmbAfiliacion" TabIndex="5" FieldLabel="Afiliación" Width="300"
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
                            <ext:ComboBox ID="cmbTerminal" FieldLabel="Terminal" TabIndex="6" EmptyText="Todas"
                                Resizable="true" ListWidth="350" Width="300" runat="server" StoreID="StoreTerminal"
                                DisplayField="NombreORazonSocial" ValueField="ClaveColectiva">
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cmbOperador" FieldLabel="Operador" TabIndex="7" EmptyText="Todos"
                                Resizable="true" ListWidth="350" Width="300" runat="server" StoreID="StoreOperador"
                                DisplayField="NameFin" ValueField="ClaveColectiva">
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cmbEstatus" FieldLabel="Estatus" StoreID="StoreEstatus" TabIndex="8"
                                EmptyText="Todos" Resizable="true" ListWidth="350" Width="300" runat="server"
                                DisplayField="Descripcion" ValueField="ID_EstatusOperacion">
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cmbBeneficiario" FieldLabel="Marca" TabIndex="9" EmptyText="Todos"
                                Resizable="true" ListWidth="350" Width="300" runat="server" StoreID="StoreBeneficiario"
                                DisplayField="NombreORazonSocial" ValueField="ClaveColectiva">
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:TextField ID="txtTelefono" FieldLabel="Referencia" EmptyText="Todos" TabIndex="10"
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
                                    <Click OnEvent="btnBuscar_Click" Before="var valid= #{FormPanel1}.getForm().isValid(); if (!valid) {} return valid;" />
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
                                    <ext:RecordField Name="Autorizacion" />
                                    <ext:RecordField Name="ID_Operacion" />
                                    <ext:RecordField Name="Sucursal" />
                                    <ext:RecordField Name="Estatus" />
                                    <ext:RecordField Name="Autorizacion" />
                                    <ext:RecordField Name="Telefono" />
                                    <ext:RecordField Name="Monto" />
                                    <ext:RecordField Name="MesesPagar" />
                                    <ext:RecordField Name="LimiteCredito" />
                                    <ext:RecordField Name="NombreCliente" />
                                    <ext:RecordField Name="ID_Cliente" />
                                    <ext:RecordField Name="Disponible" />
                                    <ext:RecordField Name="Mensualidad" />
                                    <ext:RecordField Name="Dia" />
                                    <ext:RecordField Name="Fecha" Type="Date" />
                                    <ext:RecordField Name="Hora" Type="Date" />
                                    <ext:RecordField Name="Plazos" />
                                    <ext:RecordField Name="Referencia" />
                                    <ext:RecordField Name="Cedido" />
                                    <ext:RecordField Name="NetoPagar" />
                                    <ext:RecordField Name="Condiciones" />
                                    <ext:RecordField Name="Descuento" />
                                    <ext:RecordField Name="FechaPago" />
                                    <ext:RecordField Name="Anio" />
                                    <ext:RecordField Name="Compras" />
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

                                        <ext:Column ColumnID="Autorizacion" Header="No." Sortable="true" DataIndex="Autorizacion" />
                                        <ext:Column ColumnID="ID_Operacion" Header="Identificador" Sortable="true" DataIndex="ID_Operacion" />
                                        <ext:Column ColumnID="Sucursal" Header="Sucursal" Sortable="true" DataIndex="Sucursal" />
                                        <ext:Column ColumnID="Estatus" Header="Estatus" Sortable="true" DataIndex="Estatus" />
                                        <ext:Column ColumnID="Autorizacion" Header="No." Sortable="true" DataIndex="Autorizacion" />
                                        <ext:Column ColumnID="Telefono" Header="Tarjeta" Sortable="true" DataIndex="Telefono" />
                                        <ext:Column ColumnID="Monto" Header="Monto" Sortable="true" DataIndex="Monto">
                                            <Renderer Format="UsMoney" />
                                        </ext:Column>
                                        <ext:Column ColumnID="MesesPagar" Header="MesesPagar" Sortable="true" DataIndex="MesesPagar" />
                                        <ext:Column ColumnID="LimiteCredito" Header="LimiteCredito" Sortable="true" DataIndex="LimiteCredito" />
                                        <ext:Column ColumnID="NombreCliente" Header="NombreCliente" Sortable="true" DataIndex="NombreCliente" />
                                        <ext:Column ColumnID="ID_Cliente" Header="ID_Cliente" Sortable="true" DataIndex="ID_Cliente" />
                                        <ext:Column ColumnID="Disponible" Header="Disponible" Sortable="true" DataIndex="Disponible" />
                                        <ext:Column ColumnID="Mensualidad" Header="Mensualidad" Sortable="true" DataIndex="Mensualidad" />
                                        <ext:DateColumn ColumnID="colFecha" Header="Fecha" Sortable="true" DataIndex="Fecha"
                                            Format="ddd" />
                                        <ext:DateColumn ColumnID="colFecha" Header="Fecha" Sortable="true" DataIndex="Fecha"
                                            Format="yyyy-MM-dd" />
                                        <ext:DateColumn ColumnID="colhora" Header="Hora" Sortable="true" DataIndex="Fecha"
                                            Format="HH:mm:ss" />
                                        <ext:Column ColumnID="Plazos" Header="Plazos" Sortable="true" DataIndex="Plazos" />
                                        <ext:Column ColumnID="Referencia" Header="Referencia" Sortable="true" DataIndex="Referencia" />
                                        <ext:Column ColumnID="Cedido" Header="Cedido" Sortable="true" DataIndex="Cedido" />
                                        <ext:Column ColumnID="NetoPagar" Header="NetoPagar" Sortable="true" DataIndex="NetoPagar" />
                                        <ext:Column ColumnID="Condiciones" Header="Condiciones" Sortable="true" DataIndex="Condiciones" />
                                        <ext:Column ColumnID="Descuento" Header="Descuento" Sortable="true" DataIndex="Descuento" />
                                        <ext:DateColumn ColumnID="FechaPago" Header="FechaPago" Sortable="true" DataIndex="FechaPago"
                                            Format="yyyy-MM-dd" />
                                        <ext:Column ColumnID="Descuento" Header="Descuento" Sortable="true" DataIndex="Descuento" />
                                        <ext:DateColumn ColumnID="FechaPago" Header="FechaPago" Sortable="true" DataIndex="FechaPago"
                                            Format="yyyy-MM-dd" />
                                        <ext:DateColumn ColumnID="Anio" Header="Anio" Sortable="true" DataIndex="Anio"
                                            Format="yyyy" />
                                        <ext:Column ColumnID="Compras" Header="Compras" Sortable="true" DataIndex="Compras" />
                                        <ext:Column ColumnID="NombreOperacion" Header="NombreOperacion" Sortable="true" DataIndex="NombreOperacion" />
                                    </Columns>
                                </ColumnModel>
                                <%--  <Plugins>
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
                                </Plugins>--%>
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


