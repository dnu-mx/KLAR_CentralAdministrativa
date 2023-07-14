<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="Reporte_Clientes_v2.aspx.cs" Inherits="CentroContacto.Reporte_Clientes_v2" %>

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
            <ext:FormPanel ID="FormPanel1" Width="320" Title="Selecciona los Filtros" runat="server"
                Border="false" Layout="FitLayout">
                <Content>
                    <ext:Hidden ID="FormatType" runat="server" />
                    <ext:Store ID="StoreNivelLealtad" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_GrupoCuenta">
                                <Fields>
                                    <ext:RecordField Name="ID_GrupoCuenta" />
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
                            <ext:ComboBox ID="comboNivelLealtad" TabIndex="3" FieldLabel="Nivel Lealtad" EmptyText="Todas"
                                Resizable="true" ListWidth="350" Width="300" runat="server" StoreID="StoreNivelLealtad"
                                DisplayField="Descripcion" ValueField="ID_GrupoCuenta">
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:ComboBox ID="comboEstatusUsuario" TabIndex="3" FieldLabel="Estatus Usuario" EmptyText="Todas"
                                Resizable="true" ListWidth="350" Width="300" runat="server">
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                    <ext:ListItem Text="Activo" Value="1" />
                                    <ext:ListItem Text="Inactivo" Value="2" />
                                </Items>
                            </ext:ComboBox>
                            <ext:TextField ID="txtDominioCorreo" FieldLabel="Correo Dominio" EmptyText="Todos" TabIndex="10"
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
                            <ext:Button ID="btnBuscarHide" runat="server" Hidden="true">
                                <Listeners>
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Clientes...' });
                                        #{GridPanelClientes}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000" Before="var valid= #{FormPanel1}.getForm().isValid();
                                        if (!valid) {} else { resetToolbar(#{PagingToolBar1});
                                        #{GridPanelClientes}.getStore().sortInfo = null; } return valid;">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                            RepClientesMoshi.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>
            </ext:FormPanel>
        </West>
        <Center Split="true" Collapsible="false">
            <ext:Panel ID="Panel2" runat="server" Title="Clientes Obtenidos con el Filtro Seleccionado"
                Collapsed="false" Layout="Fit" AutoScroll="true">
                <Content>
                    <ext:Store ID="StoreClientes" runat="server" OnSubmitData="StoreSubmit" RemoteSort="true"
                        OnRefreshData="StoreClientes_RefreshData" AutoLoad="false">
                        <AutoLoadParams>
                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                        </AutoLoadParams>
                        <Proxy>
                            <ext:PageProxy />
                        </Proxy>
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="cliente_id">
                                <Fields>
                                    <ext:RecordField Name="cliente_id" />
                                    <ext:RecordField Name="Nombre" />
                                    <ext:RecordField Name="Apellido" />
                                    <ext:RecordField Name="FechaNacimiento" />
                                    <ext:RecordField Name="Genero" />
                                    <ext:RecordField Name="Email" />
                                    <ext:RecordField Name="EmailEmpresa" />
                                    <ext:RecordField Name="LugarTrabajo" />
                                    <ext:RecordField Name="Convenio" />
                                    <ext:RecordField Name="FechaAlta" />
                                    <ext:RecordField Name="FechaActivacion" />
                                    <ext:RecordField Name="FechaAltaLealtad" />
                                    <ext:RecordField Name="NivelLealtad" />
                                    <ext:RecordField Name="Visitas" />
                                    <ext:RecordField Name="PuntosMoshi" />
                                    <ext:RecordField Name="EstatusUsuario" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <DirectEventConfig IsUpload="true" />
                        <SortInfo Field="FechaActivacion" />
                    </ext:Store>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanelClientes" runat="server" StoreID="StoreClientes" StripeRows="true"
                                Header="false" Border="false">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                        <ext:Column Header="ID Cliente" Sortable="true" DataIndex="cliente_id" />
                                        <ext:Column ColumnID="nombre" Header="Nombre" Sortable="true" DataIndex="Nombre" />
                                        <ext:Column Header="Apellido" Sortable="true" DataIndex="Apellido" />
                                        <ext:DateColumn ColumnID="FechaNacimiento" Header="Fecha Nacimiento" Sortable="true" DataIndex="FechaNacimiento" Format="yyyy-MM-dd" />
                                        <ext:Column Header="Genero" Sortable="true" DataIndex="Genero" />
                                        <ext:Column Header="Email " Sortable="true" DataIndex="Email" />
                                        <ext:Column Header="Email Empresa" Sortable="true" DataIndex="EmailEmpresa" />
                                        <ext:Column Header="Lugar Trabajo" Sortable="true" DataIndex="LugarTrabajo" />
                                        <ext:Column Header="Convenio" Sortable="true" DataIndex="Convenio" />
                                        <ext:DateColumn ColumnID="FechaAlta" Header="Fecha Alta" Sortable="true" DataIndex="FechaAlta" Format="yyyy-MM-dd" />
                                        <ext:DateColumn ColumnID="FechaAltaLealtad" Header="Fecha Alta Lealtad" Sortable="true" DataIndex="FechaAltaLealtad" Format="yyyy-MM-dd" />
                                        <ext:Column Header="Nivel Lealtad" Sortable="true" DataIndex="NivelLealtad" />
                                        <ext:Column Header="Visitas" Sortable="true" DataIndex="Visitas" />
                                        <ext:Column Header="Puntos Moshi" Sortable="true" DataIndex="PuntosMoshi" />
                                        <ext:Column Header="Estatus Usuario" Sortable="true" DataIndex="EstatusUsuario" />
                                    </Columns>
                                </ColumnModel>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreClientes" DisplayInfo="true"
                                        DisplayMsg="Mostrando Reporte {0} - {1} de {2}" />
                                </BottomBar>
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar2" runat="server">
                                        <Items>
                                            <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                                            <ext:Button ID="btnExportXML" runat="server" Text="Exportar a XML" Icon="PageCode" Disabled="true">
                                                <Listeners>
                                                    <Click Handler="submitValue(#{GridPanelClientes}, #{FormatType}, 'xml');" />
                                                </Listeners>
                                            </ext:Button>
                                            <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="true">
                                                <DirectEvents>
                                                    <Click OnEvent="Download" IsUpload="true"
                                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                            e.stopEvent(); 
                                                            RepClientesMoshi.StopMask();" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="btnExportCSV" runat="server" Text="Exportar a CSV" Icon="PageAttach" Disabled="true">
                                                <Listeners>
                                                    <Click Handler="submitValue(#{GridPanelClientes}, #{FormatType}, 'csv');" />
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
