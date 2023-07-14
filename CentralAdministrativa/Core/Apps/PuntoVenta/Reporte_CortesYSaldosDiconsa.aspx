<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Reporte_CortesYSaldosDiconsa.aspx.cs" Inherits="TpvWeb.Reporte_CortesYSaldosDiconsa" %>

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
                    <ext:Panel ID="Panel1" runat="server">
                        <Items>
                            <ext:ComboBox ID="cmbCorte" TabIndex="1" FieldLabel="Corte" ListWidth="300"
                                Width="300" runat="server" Resizable="true" AllowBlank="false">
                                <Items>
                                    <ext:ListItem Text="Último" Value="1" />
                                    <ext:ListItem Text="Penúltimo" Value="2" />
                                    <ext:ListItem Text="Antepenúltimo" Value="3" />
                                </Items>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cmbSucursal" TabIndex="2" FieldLabel="Sucursal" EmptyText="Todas"
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
                            <ext:ComboBox ID="cmbUnidadOperativa" TabIndex="3" FieldLabel="Unidad Operativa" Width="300"
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
                            <ext:ComboBox ID="cmbAlmacen" FieldLabel="Almacén" TabIndex="4" EmptyText="Todos"
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
                            <ext:ComboBox ID="cmbTienda" TabIndex="5" FieldLabel="Tienda" EmptyText="Todas"
                                Resizable="true" ListWidth="350" Width="300" runat="server" StoreID="StoreTienda"
                                DisplayField="NombreTienda" ValueField="ID_Colectiva">
                                 <Items>
                                    <ext:ListItem Text="( Todas )" Value="-1" />
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
                                        <EventMask ShowMask="true" Msg="Obteniendo Cortes y Saldos..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>
            </ext:FormPanel>
        </West>
        <Center Split="true" Collapsible="false">
            <ext:Panel ID="Panel2" runat="server" Title="Cortes y Saldos Obtenidos con el Filtro Seleccionado"
                Collapsed="false" Layout="Fit" AutoScroll="true">
                <Content>
                    <ext:Store ID="Store1" runat="server" OnSubmitData="Store1_Submit" OnRefreshData="btnBuscar_Click" RemoteSort="true">
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                                <Fields>
                                    <ext:RecordField Name="ID_Colectiva" />
                                    <ext:RecordField Name="ID_Cuenta" />
                                    <ext:RecordField Name="Tienda" />
                                    <ext:RecordField Name="LineaCredito" />
                                    <ext:RecordField Name="AntiguedadCorte" />
                                    <ext:RecordField Name="FechaCorte" Type="Date" />
                                    <ext:RecordField Name="SaldoAlCorte" />
                                    <ext:RecordField Name="CargosDespuesDelCorte" />
                                    <ext:RecordField Name="AbonosDespuesDelCorte" />
                                    <ext:RecordField Name="AdeudoAlDia" />
                                    <ext:RecordField Name="SaldoDisponible" />
                                    <ext:RecordField Name="AbonosDespuesDelCorte" />
                                    <ext:RecordField Name="FechaUltimaRecoleccion" Type="Date" />
                                    <ext:RecordField Name="ImporteUltimaRecoleccion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <DirectEventConfig IsUpload="true" />
                        <SortInfo Field="FechaCorte" />
                    </ext:Store>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store1" StripeRows="true"
                                Header="false" Border="false">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                        <ext:Column ColumnID="Tienda" Header="Tienda" Sortable="true" DataIndex="Tienda" Width="100"/>
                                        <ext:Column ColumnID="LineaCredito" Header="Límite de Crédito" Sortable="true" DataIndex="LineaCredito"
                                            Width="100">
                                            <Renderer Format="UsMoney" />
                                        </ext:Column>

                                        <ext:DateColumn ColumnID="FechaCorte" Header="Fecha Último Corte" Sortable="true" DataIndex="FechaCorte"
                                            Format="dd-MM-yyyy" Width="130"/>
                                        <ext:Column ColumnID="SaldoAlCorte" Header="Adeudo al Corte" Sortable="true" DataIndex="SaldoAlCorte"
                                            Width="100">
                                            <Renderer Format="UsMoney" />
                                        </ext:Column>
                                        <ext:Column ColumnID="CargosDespuesDelCorte" Header="Cargos Posteriores" Sortable="true" DataIndex="CargosDespuesDelCorte"
                                            Width="110">
                                            <Renderer Format="UsMoney" />
                                        </ext:Column>
                                        <ext:Column ColumnID="AbonosDespuesDelCorte" Header="Abonos Posteriores" Sortable="true" DataIndex="AbonosDespuesDelCorte"
                                            Width="110">
                                            <Renderer Format="UsMoney" />
                                        </ext:Column>
                                        <ext:Column ColumnID="AdeudoAlDia" Header="Adeudo Actual" Sortable="true" DataIndex="AdeudoAlDia"
                                            Width="90">
                                            <Renderer Format="UsMoney" />
                                        </ext:Column>
                                        <ext:DateColumn ColumnID="FechaUltimaRecoleccion" Header="Fecha Última Recolección" Sortable="true"
                                            DataIndex="FechaUltimaRecoleccion" Format="dd-MM-yyyy" Width="145"/>
                                        <ext:Column ColumnID="ImporteUltimaRecoleccion" Header="Importe Última Recolección" Sortable="true" 
                                            DataIndex="ImporteUltimaRecoleccion" Width="155">
                                            <Renderer Format="UsMoney" />
                                        </ext:Column>
                                    </Columns>
                                </ColumnModel>
                                <Plugins>
                                    <ext:GridFilters runat="server" ID="GridFilters1" Local="true">
                                        <Filters>
                                            <ext:StringFilter DataIndex="Tienda" />
                                            <ext:DateFilter DataIndex="FechaCorte">
                                                <DatePickerOptions runat="server" TodayText="Hoy" />
                                            </ext:DateFilter>
                                            <ext:StringFilter DataIndex="SaldoAlCorte" />
                                            <ext:DateFilter DataIndex="FechaUltimaRecoleccion">
                                                <DatePickerOptions runat="server" TodayText="Hoy" />
                                            </ext:DateFilter>
                                            <ext:StringFilter DataIndex="ImporteUltimaRecoleccion" />
                                            <ext:StringFilter DataIndex="AdeudoAlDia" />
                                            <ext:StringFilter DataIndex="LineaCredito" />
                                        </Filters>
                                    </ext:GridFilters>
                                </Plugins>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="Store1" DisplayInfo="true"
                                        DisplayMsg="Mostrando Cortes y Saldos {0} - {1} de {2}" />
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

