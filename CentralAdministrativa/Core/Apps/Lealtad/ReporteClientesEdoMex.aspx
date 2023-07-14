<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ReporteClientesEdoMex.aspx.cs" Inherits="Lealtad.ReporteClientesEdoMex" %>

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

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true" Collapsible="true">
            <ext:FormPanel ID="FormPanel1" Width="300" Title="Selecciona los Filtros" runat="server"
                Border="false" Layout="FitLayout">
                <Content>
                    <ext:Hidden ID="FormatType" runat="server" />
                </Content>

                <%--Panel de Filtros--%>
                <Items>
                    <ext:Panel ID="PanelFiltros" runat="server" Border="false" Padding="10">
                        <Items>
                            <ext:TextField ID="txtNombre" runat="server" FieldLabel="Nombre" Width="280" EmptyText="Todos"/>
                            <ext:TextField ID="txtApellidoPat" runat="server" FieldLabel="Ap. Paterno" Width="280" EmptyText="Todos"/>
                            <ext:TextField ID="txtApellidoMat" runat="server" FieldLabel="Ap. Materno" Width="280" EmptyText="Todos"/>   
                            <ext:TextField ID="txtCorreo" runat="server" FieldLabel="Correo" Width="280" EmptyText="Todos"/>
                            <ext:TextField ID="txtPlaca" runat="server" FieldLabel="Placa" Width="280" EmptyText="Todos"/>
                            <ext:TextField ID="txtCURP" runat="server" FieldLabel="CURP" Width="280" EmptyText="Todas"/>
                            
                            <ext:Panel ID="Panel3" runat="server" Title="Periodo de Modificación" Padding="3" FormGroup="true">
                                <Items>
                                    <ext:DateField ID="dfFechaInicio" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                        Format="yyyy/MM/dd" Width="275" EnableKeyEvents="true">
                                        <CustomConfig>
                                            <ext:ConfigItem Name="endDateField" Value="#{dfFechaFin}" Mode="Value" />
                                        </CustomConfig>
                                        <Listeners>
                                            <KeyUp Fn="onKeyUp" />
                                        </Listeners>
                                    </ext:DateField>
                                    <ext:DateField ID="dfFechaFin" runat="server" Vtype="daterange" FieldLabel="Fecha Final"
                                        Width="275" Format="yyyy/MM/dd" EnableKeyEvents="true">
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
                            <ext:Button ID="btnBuscarHide" runat="server" Hidden="true">
                                <Listeners>
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Clientes...' });
                                        #{GridPanelClientes}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                        Before="if (!#{FormPanel1}.getForm().isValid()) { return false; }
                                        else { resetToolbar(#{PagingToolBar1});
                                        #{GridPanelClientes}.getStore().sortInfo = null; }">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                            RepCtesEdoMex.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>
            </ext:FormPanel>
        </West>
        <Center Split="true">
            <ext:FormPanel ID="FormPanelClientes" runat="server" Layout="FitLayout" Title="Clientes obtenidos con los Filtros">
                <Items>
                    <ext:GridPanel ID="GridPanelClientes" runat="server" StripeRows="true" Header="true"
                        Layout="fit" Region="Center">
                        <Store>
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
                                    <ext:JsonReader IDProperty="ID_Promocion">
                                        <Fields>
                                            <ext:RecordField Name="Nombre" />
                                            <ext:RecordField Name="ApellidoPaterno" />
                                            <ext:RecordField Name="ApellidoMaterno" />
                                            <ext:RecordField Name="RFC" />
                                            <ext:RecordField Name="CURP" />
                                            <ext:RecordField Name="FechaPago" />
                                            <ext:RecordField Name="Placa" />
                                            <ext:RecordField Name="Marca" />
                                            <ext:RecordField Name="Modelo" />
                                            <ext:RecordField Name="Color" />
                                            <ext:RecordField Name="Email" />
                                            <ext:RecordField Name="Celular" />
                                            <ext:RecordField Name="FechaRegistro" />
                                            <ext:RecordField Name="EmailRegistrado" />
                                            <ext:RecordField Name="CelularRegistrado" />
                                            <ext:RecordField Name="FechaRegistroWeb" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <ColumnModel ID="ColumnModel12" runat="server">
                            <Columns>
                                <ext:Column ColumnID="Nombre" Header="Nombre" Sortable="true" DataIndex="Nombre" />
                                <ext:Column ColumnID="ApellidoPaterno" Header="Apellido Paterno" Sortable="true" DataIndex="ApellidoPaterno" />
                                <ext:Column ColumnID="ApellidoMaterno" Header="Apellido Materno" Sortable="true" DataIndex="ApellidoMaterno" />
                                <ext:Column ColumnID="RFC" Header="RFC" Sortable="true" DataIndex="RFC" />
                                <ext:Column ColumnID="CURP" Header="CURP" Sortable="true" DataIndex="CURP" />
                                <ext:DateColumn ColumnID="FechaPago" Header="F. Pago (dd/mm/aaaa)" Sortable="true" DataIndex="FechaPago" Format="dd/MM/yyyy" />
                                <ext:Column ColumnID="Placa" Header="Placa" Sortable="true" DataIndex="Placa" />
                                <ext:Column ColumnID="Marca" Header="Marca" Sortable="true" DataIndex="Marca" />
                                <ext:Column ColumnID="Modelo" Header="Modelo" Sortable="true" DataIndex="Modelo" />
                                <ext:Column ColumnID="Color" Header="Color" Sortable="true" DataIndex="Color" />
                                <ext:Column ColumnID="Email" Header="Email" Sortable="true" DataIndex="Email" />
                                <ext:Column ColumnID="Celular" Header="Celular" Sortable="true" DataIndex="Celular" />
                                <ext:DateColumn ColumnID="FechaRegistro" Header="F. Registro (dd/mm/aaaa)" Sortable="true" DataIndex="FechaRegistro" Format="dd/MM/yyyy" />
                                <ext:Column ColumnID="EmailRegistrado" Header="Email Registrado" Sortable="true" DataIndex="EmailRegistrado" />
                                <ext:Column ColumnID="CelularRegistrado" Header="Celular Registrado" Sortable="true" DataIndex="CelularRegistrado" />
                                <ext:DateColumn ColumnID="FechaRegistroWeb" Header="F. Registro Web (dd/mm/aaaa)" Sortable="true" DataIndex="FechaRegistroWeb" Format="dd/MM/yyyy" />
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
                                                    RepCtesEdoMex.StopMask();" />
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
                        <BottomBar>
                            <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreClientes" DisplayInfo="true"
                                DisplayMsg="Mostrando Beneficios {0} - {1} de {2}" />
                        </BottomBar>
                    </ext:GridPanel>
                </Items>
            </ext:FormPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
