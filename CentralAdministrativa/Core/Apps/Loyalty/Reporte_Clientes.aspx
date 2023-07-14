<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="Reporte_Clientes.aspx.cs" Inherits="CentroContacto.Reporte_Clientes" %>

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
            <ext:FormPanel ID="FormPanel1" Width="350" Title="Selecciona los Filtros" runat="server" LabelWidth="130"
                Border="false" Layout="FitLayout" Padding="10">
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
                        <SortInfo Field="NombreORazonSocial"  Direction="ASC"  />
                    </ext:Store>
                    <ext:Store ID="StoreSucursal" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                                <Fields>
                                    <ext:RecordField Name="NombreORazonSocial" />
                                    <ext:RecordField Name="ClaveColectiva" />
                                    <ext:RecordField Name="ID_Colectiva" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="NombreORazonSocial"  Direction="ASC"  />
                    </ext:Store>
                     <ext:Store ID="StoreTipoMediosAccesos" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_TipoMA">
                                <Fields>
                                    <ext:RecordField Name="Clave" />
                                    <ext:RecordField Name="Descripcion" />
                                    <ext:RecordField Name="ID_TipoMA" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="Descripcion"  Direction="ASC"  />
                    </ext:Store>
                </Content>
                <Items>
                    <ext:Panel ID="Panel1" runat="server" Layout="FitLayout" Border="false">
                        <Items>
                            <%-- <ext:DateField FieldLabel="Fecha Inicial" AllowBlank="false" MsgTarget="Side" Format="yyyy-MM-dd"
                                ID="datInicio" runat="server" EmptyText="Selecciona una Fecha Inicial" MaxWidth="300"
                                Width="300" MaxLength="12" Vtype="daterange" />
                            <ext:DateField FieldLabel="Fecha Final" AllowBlank="false" MsgTarget="Side" Format="yyyy-MM-dd"
                                TabIndex="2" ID="datFinal" runat="server" EmptyText="Selecciona una Fecha Final"
                                Width="300" MaxLength="12" Vtype="daterange" />--%>
                            <ext:DateField ID="datInicio" runat="server" Vtype="daterange" FieldLabel="Fecha Activación Inicial"
                                AllowBlank="false" MsgTarget="Side" Format="yyyy-MM-dd"
                                TabIndex="1" MaxLength="12" Width="330" EnableKeyEvents="true" MaxWidth="330">
                                <CustomConfig>
                                    <ext:ConfigItem Name="endDateField" Value="#{datFinal}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:DateField ID="datFinal" runat="server" Vtype="daterange" FieldLabel="Fecha Activación Final"
                                AllowBlank="false" MaxLength="12" Width="330" MsgTarget="Side" Format="yyyy-MM-dd" TabIndex="2" EnableKeyEvents="true">
                                <CustomConfig>
                                    <ext:ConfigItem Name="startDateField" Value="#{datInicio}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:ComboBox ID="cmbCadenaComercial" TabIndex="3" FieldLabel="Cadena Comercial" EmptyText="Todas"
                                Resizable="true" ListWidth="350" Width="330" runat="server" StoreID="StoreCadenaComercial"
                                DisplayField="NombreORazonSocial" ValueField="ID_Colectiva" Mode="Local" AutoSelect="true"
                                 Editable="true" ForceSelection="true" TypeAhead="true" MinChars="1" MatchFieldWidth="false"
                                 Name="cboxCadenas">
                                <DirectEvents>
                                    <Select OnEvent="LlenaSucursales">
                                    </Select>
                                </DirectEvents>
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cmbSucursal" TabIndex="4" FieldLabel="Sucursal" EmptyText="Todas"
                                Resizable="true" ListWidth="350" Width="330" runat="server" StoreID="StoreSucursal"
                                DisplayField="NombreORazonSocial" ValueField="ID_Colectiva">
                                <DirectEvents>
                                </DirectEvents>
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cmbTipoMedioAcceso" TabIndex="3" FieldLabel="Medio de Acceso" EmptyText="Todas"
                                Resizable="true" ListWidth="350" Width="330" runat="server" StoreID="StoreTipoMediosAccesos"
                                DisplayField="Descripcion" ValueField="ID_TipoMA">
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <%--<ext:TextField ID="txtMedioAcceso" FieldLabel="Tarjeta/MDA" EmptyText="Todos" TabIndex="10"
                                MaxLength="50" Width="330" runat="server" Text="" />--%>

                            <ext:TextField ID="txtEmail" FieldLabel="Email" EmptyText="Todos" TabIndex="10"
                                MaxLength="50" Width="330" Vtype="email" runat="server" Text="" />

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
                                        <EventMask ShowMask="true" Msg="Buscando Clientes..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>
            </ext:FormPanel>
        </West>
        <Center Split="true" Collapsible="false">
            <ext:Panel ID="Panel2" runat="server" Title="Clientes Obtenidos con los Filtros Seleccionados"
                Collapsed="false" Layout="Fit" AutoScroll="true">
                <Content>
                    <ext:Store ID="Store1" runat="server" OnSubmitData="Store1_Submit" OnRefreshData="btnBuscar_Click" RemoteSort="true">
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                                <%--<ext:ArrayReader IDProperty="ID_Operacion">--%>
                                <Fields>
                                    <%--<ext:RecordField Name="Fecha" Type="Date" />--%>
                                    <ext:RecordField Name="ID_Colectiva" />
                                    <ext:RecordField Name="ClaveMA" />
                                    <ext:RecordField Name="Descripcion" />
                                    <ext:RecordField Name="Nombre" />
                                    <ext:RecordField Name="Email" />
                                    <ext:RecordField Name="FechaNacimiento" />
                                    <ext:RecordField Name="SaldoActual" />
                                    <ext:RecordField Name="Descripcion1" />
                                    <ext:RecordField Name="CadenaActivacion" />
                                    <ext:RecordField Name="ID_SucursalActivacion" />
                                    <ext:RecordField Name="NombreORazonSocial" />
                                    <ext:RecordField Name="FechaActivacion" />
                                    <ext:RecordField Name="UsuarioActivacion" />
                                    <ext:RecordField Name="CorreoUsuarioActivacion" />
                                    <%--<ext:RecordField Name="TarjetaHabiente" />
                                    <ext:RecordField Name="DescripcionPromo" />--%>
                                </Fields>
                                <%--</ext:ArrayReader>--%>
                            </ext:JsonReader>
                        </Reader>
                        <DirectEventConfig IsUpload="true" />
                        <SortInfo Field="FechaActivacion" />
                    </ext:Store>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store1" StripeRows="true"
                                Header="false" Border="false">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                        <%--<ext:DateColumn ColumnID="colFecha" Header="Fecha" Sortable="true" DataIndex="Fecha"
                                            Format="yyyy-MM-dd" />
                                        <ext:DateColumn ColumnID="colhora" Header="Hora" Sortable="true" DataIndex="Fecha"
                                            Format="HH:mm:ss" />--%>
                                        <ext:Column ColumnID="ID_Colectiva" Header="ID Cliente" Sortable="true" DataIndex="ID_Colectiva" />
                                        <ext:Column ColumnID="ClaveMA" Header="Tarjeta/MDA" Sortable="true" DataIndex="ClaveMA" />
                                        <ext:Column ColumnID="Descripcion" Header="Tipo de Medio de Acceso" Sortable="true" DataIndex="Descripcion" />
                                        <ext:Column ColumnID="Nombre" Header="Nombre del Cliente" Sortable="true" DataIndex="Nombre" />
                                        <ext:Column ColumnID="Email" Header="Email" Sortable="true" DataIndex="Email" />
                                        <ext:DateColumn ColumnID="FechaNacimiento" Header="Fecha de Nacimiento" Sortable="true" DataIndex="FechaNacimiento" Format="dd-MM-yyyy" />
                                        <ext:Column ColumnID="SaldoActual" Header="Saldo Actual" Sortable="true" DataIndex="SaldoActual" />
                                        <ext:Column ColumnID="Descripcion1" Header="Estatus" Sortable="true" DataIndex="Descripcion1" />
                                        <ext:Column ColumnID="CadenaActivacion" Header="Cadena Activación" Sortable="true" DataIndex="CadenaActivacion" />
                                        <ext:Column ColumnID="ID_SucursalActivacion" Header="No. SucursalActivación" Sortable="true" DataIndex="ID_SucursalActivacion" />
                                        <ext:Column ColumnID="NombreORazonSocial" Header="Nombre Sucursal Activación" Sortable="true" DataIndex="NombreORazonSocial" />
                                        <ext:DateColumn ColumnID="FechaActivacion" Header="Fecha Activación" Sortable="true" DataIndex="FechaActivacion" Format="dd-MM-yyyy" />                                                                                   <ext:Column ColumnID="UsuarioActivacion" Header="Usuario Activación" Sortable="true" DataIndex="UsuarioActivacion" />
                                        <ext:Column ColumnID="CorreoUsuarioActivacion" Header="Correo Usuario Activación" Sortable="true" DataIndex="CorreoUsuarioActivacion" />
                                         <%--<ext:Column ColumnID="NombreOperacion" Header="Operacion" Sortable="true" DataIndex="NombreOperacion" />
                                         <ext:Column ColumnID="ID_Cliente" Header="ID_Cliente" Sortable="true" DataIndex="ID_Cliente" />
                                         <ext:Column ColumnID="TarjetaHabiente" Header="TarjetaHabiente" Sortable="true" DataIndex="TarjetaHabiente" />--%>
                                    </Columns>
                                </ColumnModel>
                                <Plugins>
                                    <ext:GridFilters runat="server" ID="GridFilters1" Local="true">
                                        <Filters>
                                        <%--<ext:StringFilter DataIndex="ID_Colectiva" />
                                        <ext:StringFilter DataIndex="ClaveMA" />
                                        <ext:StringFilter DataIndex="Descripcion" />
                                            <ext:NumericFilter DataIndex="ID_Operacion" />
                                            <ext:StringFilter DataIndex="Nombre" />
                                            <ext:StringFilter DataIndex="Email" />
                                            <ext:StringFilter DataIndex="SaldoActual" />
                                            <ext:StringFilter DataIndex="Descripcion" />
                                            <ext:StringFilter DataIndex="CadenaActivacion" />
                                            <ext:StringFilter DataIndex="ID_SucursalActivacion" />
                                            <ext:StringFilter DataIndex="NombreORazonSocial" />
                                            <ext:StringFilter DataIndex="UsuarioActivacion" />
                                            <ext:StringFilter DataIndex="CorreoUsuarioActivacion" />
                                            <%--<ext:NumericFilter DataIndex="Monto" />
                                            <ext:DateFilter DataIndex="FechaActivacion">
                                                <DatePickerOptions runat="server" TodayText="Hoy" />
                                            </ext:DateFilter>--%>
                                        </Filters>
                                    </ext:GridFilters>
                                </Plugins>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="Store1" DisplayInfo="true"
                                        DisplayMsg="Mostrando Reporte {0} - {1} de {2}" />
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
