<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master"
    CodeBehind="ReporteClientesPrana.aspx.cs" Inherits="Lealtad.ReporteClientesPrana" %>

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

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true" Collapsible="true">
            <ext:FormPanel ID="FormPanel1" Width="320" Title="Selecciona los Filtros" runat="server"
                Border="false" Layout="FormLayout" Padding="10">
                <Content>
                    <ext:Hidden ID="FormatType" runat="server" />
                </Content>
                <Items>
                    <ext:TextField ID="txtNombre" FieldLabel="Nombre" EmptyText="Todos" AnchorHorizontal="95%"
                        MaxLength="50" Width="300" runat="server" Text="" />
                    <ext:TextField ID="txtApellido" FieldLabel="Apellido" EmptyText="Todos" AnchorHorizontal="95%"
                        MaxLength="50" Width="300" runat="server" Text="" />
                    <ext:TextField ID="txtCorreo" FieldLabel="Correo" EmptyText="Todos" AnchorHorizontal="95%"
                        MaxLength="50" Width="300" runat="server" Text="" />
                    <ext:TextField ID="txtMembresia" FieldLabel="Membresía" EmptyText="Todas" AnchorHorizontal="95%"
                        MaxLength="50" Width="300" runat="server" Text="" />
                    <ext:Panel runat="server" Title="Periodo de Registro" Padding="3" FormGroup="true">
                        <Items>
                            <ext:DateField ID="dfFechaInicio" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                Format="yyyy/MM/dd" Width="275" EnableKeyEvents="true" AllowBlank="false">
                                <CustomConfig>
                                    <ext:ConfigItem Name="endDateField" Value="#{dfFechaFin}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:DateField ID="dfFechaFin" runat="server" Vtype="daterange" FieldLabel="Fecha Final"
                                Width="275" Format="yyyy/MM/dd" EnableKeyEvents="true" AllowBlank="false">
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
                    <ext:Toolbar ID="Toolbar1" runat="server">
                        <Items>
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
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
                                            RepClientesPrana.StopMask();" />
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
                                    <ext:RecordField Name="ApellidoMat" />
                                    <ext:RecordField Name="FechaNacimiento" />
                                    <ext:RecordField Name="Genero" />
                                    <ext:RecordField Name="Email" />
                                    <ext:RecordField Name="FechaAlta" />
                                    <ext:RecordField Name="Membresia" />
                                    <ext:RecordField Name="EstatusActivacion" />
                                    <ext:RecordField Name="FechaConfirmacion" />
                                    <ext:RecordField Name="EsMovil" />
                                    <ext:RecordField Name="Origen" />
                                    <ext:RecordField Name="FechaRenovacion" />
                                    <ext:RecordField Name="Nivel" />
                                    <ext:RecordField Name="EstadoCliente" />
                                    <ext:RecordField Name="FechaCambio" />
                                    <ext:RecordField Name="UsuarioCambio" />
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
                                        <ext:Column Header="Nombre" Sortable="true" DataIndex="Nombre" />
                                        <ext:Column Header="Apellido" Sortable="true" DataIndex="Apellido" />
                                        <ext:DateColumn Header="Fecha Nacimiento" Sortable="true" DataIndex="FechaNacimiento" Format="yyyy-MM-dd" />
                                        <ext:Column Header="Genero" Sortable="true" DataIndex="Genero" />
                                        <ext:Column Header="Email " Sortable="true" DataIndex="Email" />
                                        <ext:DateColumn Header="Fecha Alta" Sortable="true" DataIndex="FechaAlta" Format="yyyy-MM-dd" />
                                        <ext:Column Header="Membresía" Sortable="true" DataIndex="Membresia" />
                                        <ext:Column Header="Estatus Activación" Sortable="true" DataIndex="EstatusActivacion" />
                                        <ext:DateColumn Header="Fecha Confirmación" Sortable="true" DataIndex="FechaConfirmacion" />
                                        <ext:Column Header="Es Móvil" Sortable="true" DataIndex="EsMovil" />
                                        <ext:Column Header="Origen" Sortable="true" DataIndex="Origen" />
                                        <ext:DateColumn Header="Fecha Renovación" Sortable="true" DataIndex="FechaRenovacion" Format="yyyy-MM-dd"/>
                                        <ext:Column Header="Nivel" Sortable="true" DataIndex="Nivel" />
                                        <ext:Column Header="Estado Cliente" Sortable="true" DataIndex="EstadoCliente" />
                                        <ext:Column Header="Fecha Cambio Membresia" Sortable="true" DataIndex="FechaCambio" Width="180px" />
                                        <ext:Column Header="Usuario Cambio Membresia" Sortable="true" DataIndex="UsuarioCambio" Width="180px" />
                                    </Columns>
                                </ColumnModel>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreClientes" DisplayInfo="true"
                                        DisplayMsg="Mostrando Clientes {0} - {1} de {2}" />
                                </BottomBar>
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar2" runat="server">
                                        <Items>
                                            <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                                            <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="true">
                                                <DirectEvents>
                                                    <Click OnEvent="Download" IsUpload="true"
                                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                            e.stopEvent(); 
                                                            RepClientesPrana.StopMask();" />
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
