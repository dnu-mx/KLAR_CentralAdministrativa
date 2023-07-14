<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ReporteDispersionesCuentasEje.aspx.cs" Inherits="Empresarial.ReporteDispersionesCuentasEje" %>

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
            <ext:FormPanel ID="FormPanelFiltros" Width="300" Title="Selecciona los Filtros" runat="server"
                Border="false" Layout="FormLayout" Padding="10" LabelAlign="Top">
                <Items>
                    <ext:DateField ID="dfFechaInicio" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                        Format="yyyy/MM/dd" Width="280" EnableKeyEvents="true" AllowBlank="false">
                        <CustomConfig>
                            <ext:ConfigItem Name="endDateField" Value="#{dfFechaFin}" Mode="Value" />
                        </CustomConfig>
                        <Listeners>
                            <KeyUp Fn="onKeyUp" />
                        </Listeners>
                    </ext:DateField>
                    <ext:DateField ID="dfFechaFin" runat="server" Vtype="daterange" FieldLabel="Fecha Final"
                        Width="280" Format="yyyy/MM/dd" EnableKeyEvents="true" AllowBlank="false">
                        <CustomConfig>
                            <ext:ConfigItem Name="startDateField" Value="#{dfFechaInicio}" Mode="Value" />
                        </CustomConfig>
                        <Listeners>
                            <KeyUp Fn="onKeyUp" />
                        </Listeners>
                    </ext:DateField>
                    <ext:ComboBox ID="cBoxCliente" runat="server" FieldLabel="Cliente" DisplayField="NombreCuentahabiente"
                        EmptyText="(Todos)" ValueField="ID_Cuenta" Width="280">
                        <Store>
                            <ext:Store ID="StoreColectivasCuentasEje" runat="server">
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_Cuenta">
                                        <Fields>
                                            <ext:RecordField Name="ID_Cuenta" />
                                            <ext:RecordField Name="ID_Colectiva" />
                                            <ext:RecordField Name="ClaveColectiva" />
                                            <ext:RecordField Name="NombreCuentahabiente" />
                                            <ext:RecordField Name="SaldoActual" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <Items>
                            <ext:ListItem Text="(Todos)" Value="-1" />
                        </Items>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cBoxTipoMovimiento" runat="server" FieldLabel="Tipo de Movimiento" Width="280"
                        EmptyText="(Todos)">
                        <Items>
                            <ext:ListItem Text="(Todos)" Value="-1" />
                            <ext:ListItem Text="Fondeo" Value="1" />
                            <ext:ListItem Text="Retiro" Value="2" />
                        </Items>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cBoxEstatus" runat="server" FieldLabel="Estatus" Width="280"
                        EmptyText="(Todos)">
                        <Items>
                            <ext:ListItem Text="(Todos)" Value="-1" />
                            <ext:ListItem Text="Autorizado" Value="AUT" />
                            <ext:ListItem Text="Rechazado" Value="RECH" />
                        </Items>
                    </ext:ComboBox>
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
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Movimientos...' });
                                        #{GridPanelDispersiones}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                        Before="resetToolbar(#{PagingToolBar1});
                                        #{GridPanelDispersiones}.getStore().sortInfo = null;">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                            RepDispersiones.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>
            </ext:FormPanel>
        </West>
        <Center Split="true">
            <ext:FormPanel ID="FormPanelDispersiones" runat="server" Layout="FitLayout" Title="Movimientos obtenidos con los Filtros">
                <Items>
                    <ext:GridPanel ID="GridPanelDispersiones" runat="server" StripeRows="true" Header="true"
                        Layout="FitLayout" Region="Center">
                        <Store>
                            <ext:Store ID="StoreDispersiones" runat="server" RemoteSort="true" OnRefreshData="StoreDispersiones_RefreshData">
                                <AutoLoadParams>
                                    <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                </AutoLoadParams>
                                <Proxy>
                                    <ext:PageProxy />
                                </Proxy>                                
                                <DirectEventConfig IsUpload="true" />
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_MovimientoCuentaEje">
                                        <Fields>
                                            <ext:RecordField Name="ID_MovimientoCuentaEje" />
                                            <ext:RecordField Name="FechaCaptura" />
                                            <ext:RecordField Name="UsuarioCaptura" />
                                            <ext:RecordField Name="TipoMovimiento" />
                                            <ext:RecordField Name="Cliente" />
                                            <ext:RecordField Name="Importe" />
                                            <ext:RecordField Name="Observaciones" />
                                            <ext:RecordField Name="FechaAutoriza" />
                                            <ext:RecordField Name="UsuarioAutoriza" />
                                            <ext:RecordField Name="FechaRechaza" />
                                            <ext:RecordField Name="UsuarioRechaza" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <ColumnModel ID="ColumnModel12" runat="server">
                            <Columns>
                                <ext:Column Hidden="true" DataIndex="ID_MovimientoCuentaEje" />
                                <ext:DateColumn Header="Fecha de Captura" Sortable="true" DataIndex="FechaCaptura" Width="120"
                                    Format="dd-MM-yyyy HH:mm:ss" />
                                <ext:Column Header="Capturó" DataIndex="UsuarioCaptura" Width="120"/>
                                <ext:Column Header="Tipo de Movimiento" Sortable="true" DataIndex="TipoMovimiento" Width="120" />
                                <ext:Column Header="Cliente" Sortable="true" DataIndex="Cliente" Width="120" />
                                <ext:Column Header="Importe" Sortable="true" DataIndex="Importe">
                                    <Renderer Format="UsMoney" />
                                </ext:Column>
                                <ext:Column Header="Observaciones" DataIndex="Observaciones" Width="150"/>
                                <ext:DateColumn Header="Fecha de Autorización" Sortable="true" DataIndex="FechaAutoriza"
                                    Width="130" Format="dd-MM-yyyy HH:mm:ss" />
                                <ext:Column Header="Autorizó" DataIndex="UsuarioAutoriza" Width="120"/>
                                <ext:DateColumn Header="Fecha de Rechazo" Sortable="true" DataIndex="FechaRechaza"
                                    Width="120" Format="dd-MM-yyyy HH:mm:ss" />
                                <ext:Column Header="Rechazó" DataIndex="UsuarioRechaza" Width="120"/>
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
                                                    RepDispersiones.StopMask();" />
                                        </DirectEvents>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                        <BottomBar>
                            <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreDispersiones" DisplayInfo="true"
                                DisplayMsg="Mostrando Movimientos {0} - {1} de {2}" />
                        </BottomBar>
                    </ext:GridPanel>
                </Items>
            </ext:FormPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
