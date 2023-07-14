<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="MonitoreoCompensacion.aspx.cs" Inherits="TpvWeb.MonitoreoCompensacion" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var template = '<span style="color:{0};text-decoration:underline;">{1}</span>';

        var link = function (value) {
            if (value > 0) {
                return String.format(template, "blue", value);
            } else {
                return value;
            }
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
    <ext:Hidden ID="hdnIdFichTemp" runat="server" />
    <ext:Hidden ID="hdnIdFichero" runat="server" />
    <ext:Window ID="WdwDetalleRegistros" runat="server" Title="Detalle de Registros" Width="900" Height="400" Resizable="False"
        Hidden="true" Closable="true" Modal="true" Layout="FitLayout" Draggable="true" Padding="5">
        <Items>
            <ext:GridPanel ID="GridDetalleRegistros" runat="server" Layout="FitLayout" Border="false" AutoScroll="true">
                <Store>
                    <ext:Store ID="StoreDetalle" runat="server" RemoteSort="true" AutoLoad="false"
                        OnRefreshData="StoreDetalle_RefreshData">
                        <AutoLoadParams>
                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                        </AutoLoadParams>
                        <Proxy>
                            <ext:PageProxy />
                        </Proxy>
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="Id">
                                <Fields>
                                    <ext:RecordField Name="Id" />
                                    <ext:RecordField Name="Codigo" />
                                    <ext:RecordField Name="Tarjeta" />
                                    <ext:RecordField Name="Fecha" />
                                    <ext:RecordField Name="Autorizacion" />
                                    <ext:RecordField Name="ImporteOperacion" />
                                    <ext:RecordField Name="MonedaOperacion" />
                                    <ext:RecordField Name="ImporteCompensacion" />
                                    <ext:RecordField Name="MonedaCompensacion" />
                                    <ext:RecordField Name="Comercio" />
                                    <ext:RecordField Name="Pais" />
                                    <ext:RecordField Name="MCC" />
                                    <ext:RecordField Name="Referencia" />
                                    <ext:RecordField Name="Tipo" />
                                    <ext:RecordField Name="Motivo" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <TopBar>
                    <ext:Toolbar runat="server">
                        <Items>
                            <ext:ToolbarFill runat="server" />
                            <ext:Button ID="btnExcelDetalle" runat="server" Text="Exportar Registros"
                                Icon="PageExcel" ToolTip="Obtener Detalle de los Registros en un Archivo Excel"
                                Disabled="true">
                                <DirectEvents>
                                    <Click OnEvent="Download_DR" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                            e.stopEvent(); 
                                            MonitComp.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnDownload_DR" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="Download_DR" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                            MonitComp.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <ColumnModel runat="server">
                    <Columns>
                        <ext:Column Header="Id" Sortable="true" DataIndex="Id" Width="70" />
                        <ext:Column Header="Código" Sortable="true" DataIndex="Codigo" Width="50" />
                        <ext:Column Header="Tarjeta" Sortable="true" DataIndex="Tarjeta" Width="120" />
                        <ext:DateColumn Header="Fecha" Sortable="true" DataIndex="Fecha" Format="yyyy-MM-dd"
                            Width="70" />
                        <ext:Column Header="Autorización" Sortable="true" DataIndex="Autorizacion" />
                        <ext:Column Header="Importe Operación" Sortable="true" DataIndex="ImporteOperacion">
                            <Renderer Format="UsMoney" />
                        </ext:Column>
                        <ext:Column Header="Moneda Operación" Sortable="true" DataIndex="MonedaOperacion"
                            Width="120"/>
                        <ext:Column Header="Importe Compensación" Sortable="true" DataIndex="ImporteCompensacion"
                            Width="150">
                            <Renderer Format="UsMoney" />
                        </ext:Column>
                        <ext:Column Header="Moneda Compensación" Sortable="true" DataIndex="MonedaCompensacion"
                            Width="150"/>
                        <ext:Column Header="Comercio" Sortable="true" DataIndex="Comercio" Width="150"/>
                        <ext:Column Header="País" Sortable="true" DataIndex="Pais" Width="50"/>
                        <ext:Column Header="MCC" Sortable="true" DataIndex="MCC" Width="50"/>
                        <ext:Column Header="Referencia" Sortable="true" DataIndex="Referencia" Width="150"/>
                        <ext:Column Header="Tipo" Sortable="true" DataIndex="Tipo" ColumnID="ColTipo" />
                        <ext:Column Header="Motivo" Sortable="true" DataIndex="Motivo" Width="120"/>
                    </Columns>
                </ColumnModel>
                <Plugins>
                    <ext:GridFilters runat="server" Local="true" FiltersText="Filtros">
                        <Filters>
                            <ext:DateFilter DataIndex="Fecha" BeforeText="Antes de" OnText="El día"
                                AfterText="Después de">
                                <DatePickerOptions runat="server" TodayText="Hoy" />
                            </ext:DateFilter>
                        </Filters>
                    </ext:GridFilters>
                </Plugins>
                <SelectionModel>
                    <ext:RowSelectionModel runat="server" SingleSelect="true" />
                </SelectionModel>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingDetalle" runat="server" StoreID="StoreDetalle" DisplayInfo="true"
                        DisplayMsg="Mostrando Detalle de Registros {0} - {1} de {2}" HideRefresh="true" />
                </BottomBar>
            </ext:GridPanel>
        </Items>
    </ext:Window>
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <North Split="true">
            <ext:Panel runat="server" Layout="FitLayout" Border="false">
                <TopBar>
                    <ext:Toolbar ID="Toolbar2" runat="server" LabelWidth="115" LabelAlign="Right" HideBorders="true">
                        <Items>
                            <ext:Checkbox ID="chkBoxArchPendientes" runat="server" FieldLabel="Archivos Pendientes">
                                <Listeners>
                                    <AfterRender Handler="this.el.on('change', function (e, el) {
                                        var estatus = el.checked ? true : false;
                                        #{dfFechaCompensacion}.setDisabled(estatus);
                                        #{cBoxMarca}.setDisabled(estatus);
                                        if (estatus) { #{dfFechaCompensacion}.reset();
                                        #{cBoxMarca}.reset(); }
                                        });" />
                                </Listeners>
                            </ext:Checkbox>
                            <ext:ToolbarSeparator runat="server" />
                            <ext:DateField ID="dfFechaCompensacion" runat="server" Format="dd/MM/yyyy" Width="210"
                                AllowBlank="false" MaxDate="<%# DateTime.Now %>" AutoDataBind="true"
                                FieldLabel="Fecha de Presentación" Editable="false"/>
                            <ext:ToolbarSeparator runat="server" />
                            <ext:ComboBox ID="cBoxMarca" runat="server" EmptyText="Selecciona la Marca..."
                                Width="140" AllowBlank="false">
                                <Items>
                                    <ext:ListItem Text="MasterCard" Value="MasterCard" />
                                    <ext:ListItem Text="Visa" Value="Visa" />
                                </Items>
                            </ext:ComboBox>
                            <ext:ToolbarSeparator runat="server" />
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                 <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                        Before="if (!#{chkBoxArchPendientes}.checked && (
                                        !#{dfFechaCompensacion}.getValue() && !#{cBoxMarca}.getValue()))
                                        { Ext.Msg.alert('Monitoreo Compensación', 'Ingresa al menos un criterio de búsqueda');
                                        return false; } else { resetToolbar(#{PagingCargaArchivos});
                                        resetToolbar(#{PagingHomologacion}); resetToolbar(#{PagingCompRegistros});
                                        #{GridCargaArchivos}.getStore().sortInfo = null;
                                        #{GridHomologacion}.getStore().sortInfo = null;
                                        #{GridCompRegistros}.getStore().sortInfo = null; }">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarSeparator runat="server" />
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnBuscarHide" runat="server" Hidden="true">
                                <Listeners>
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Información...' });
                                        #{GridCargaArchivos}.getStore().reload({params:{start:0, sort:('','')}});
                                        #{GridHomologacion}.getStore().reload({params:{start:0, sort:('','')}});
                                        #{GridCompRegistros}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
            </ext:Panel>
        </North>
        <Center Split="true">
            <ext:Panel runat="server" Layout="FormLayout" Border="false">
                <Items>
                    <ext:Panel ID="PanelCargaArchivos" runat="server" Title="Paso 1. Carga de Archivos" Layout="FitLayout"
                        Border="false" AutoHeight="true" Collapsible="true" Collapsed="true">
                        <Items>
                            <ext:GridPanel ID="GridCargaArchivos" runat="server" Header="true" Border="false" AutoHeight="true"
                                Layout="FitLayout" AutoExpandColumn="NombreFichero" AutoWidth="true" AutoScroll="true">
                                <Store>
                                    <ext:Store ID="StoreCargaArchivos" runat="server" RemoteSort="true" AutoLoad="false"
                                        OnRefreshData="StoreCargaArchivos_RefreshData">
                                        <AutoLoadParams>
                                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                        </AutoLoadParams>
                                        <Proxy>
                                            <ext:PageProxy />
                                        </Proxy>
                                        <DirectEventConfig IsUpload="true" />
                                        <Reader>
                                            <ext:JsonReader IDProperty="Id">
                                                <Fields>
                                                    <ext:RecordField Name="Id" />
                                                    <ext:RecordField Name="NombreFichero" />
                                                    <ext:RecordField Name="FechaProceso" />
                                                    <ext:RecordField Name="EstatusFicheros" />
                                                    <ext:RecordField Name="TotalRegistros" />
                                                    <ext:RecordField Name="Mensaje" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <TopBar>
                                    <ext:Toolbar runat="server">
                                        <Items>
                                            <ext:ToolbarFill runat="server" />
                                            <ext:Button ID="btnExcelCarga" runat="server" Text="Exportar Carga de Archivos" Icon="PageExcel"
                                                ToolTip="Obtener Carga de Archivos en un Archivo Excel" Disabled="true">
                                                <DirectEvents>
                                                    <Click OnEvent="Download_CA" IsUpload="true"
                                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                            e.stopEvent(); 
                                                            MonitComp.StopMask();" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="btnDownload_CA" runat="server" Hidden="true">
                                                <DirectEvents>
                                                    <Click OnEvent="Download_CA" IsUpload="true"
                                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                        MonitComp.StopMask();" />
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <ColumnModel runat="server">
                                    <Columns>
                                        <ext:Column Header="Id" Sortable="true" DataIndex="Id" Width="70" />
                                        <ext:Column Header="Nombre de Archivo" Sortable="true" DataIndex="NombreFichero" Width="100" />
                                        <ext:DateColumn Header="Fecha de Carga" Sortable="true" DataIndex="FechaProceso" 
                                            Format="yyyy-MM-dd HH:mm:ss" Width="150" />
                                        <ext:Column Header="Estatus de Carga" Sortable="true" DataIndex="EstatusFicheros" />
                                        <ext:Column Header="Total de Registros" Sortable="true" DataIndex="TotalRegistros"/>
                                        <ext:Column Header="Mensaje" Sortable="true" DataIndex="Mensaje"/>
                                    </Columns>
                                </ColumnModel>
                                <Plugins>
                                    <ext:GridFilters runat="server" Local="true" FiltersText="Filtros">
                                        <Filters>
                                            <ext:DateFilter DataIndex="FechaProceso" BeforeText="Antes de" OnText="El día"
                                                AfterText="Después de">
                                                <DatePickerOptions runat="server" TodayText="Hoy" />
                                            </ext:DateFilter>
                                            <ext:StringFilter DataIndex="NombreFichero" />
                                        </Filters>
                                    </ext:GridFilters>
                                </Plugins>
                                <SelectionModel>
                                    <ext:RowSelectionModel runat="server" SingleSelect="true" />
                                </SelectionModel>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingCargaArchivos" runat="server" StoreID="StoreCargaArchivos" DisplayInfo="true"
                                        DisplayMsg="Mostrando Carga de Archivos {0} - {1} de {2}" HideRefresh="true" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="PanelHomologacion" runat="server" Title="Paso 2. Homologación de Registros"
                        Layout="FitLayout" Border="false" AutoHeight="true" Collapsible="true" Collapsed="true">
                        <Items>
                            <ext:GridPanel ID="GridHomologacion" runat="server" AutoExpandColumn="NombreFichero" Border="false"
                                Layout="FitLayout" AutoHeight="true" AutoWidth="true" AutoScroll="true">
                                <LoadMask ShowMask="false" />
                                <Store>
                                    <ext:Store ID="StoreHomologacion" runat="server" RemoteSort="true" AutoLoad="false"
                                        OnRefreshData="StoreHomologacion_RefreshData">
                                        <AutoLoadParams>
                                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                        </AutoLoadParams>
                                        <Proxy>
                                            <ext:PageProxy />
                                        </Proxy>
                                        <DirectEventConfig IsUpload="true" />
                                        <Reader>
                                            <ext:JsonReader IDProperty="Id">
                                                <Fields>
                                                    <ext:RecordField Name="Id" />
                                                    <ext:RecordField Name="NombreFichero" />
                                                    <ext:RecordField Name="FechaProceso" />
                                                    <ext:RecordField Name="RegistrosPendientes" />
                                                    <ext:RecordField Name="RegistrosInvalidos" />
                                                    <ext:RecordField Name="RegistrosHomologados" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <TopBar>
                                    <ext:Toolbar runat="server">
                                        <Items>
                                            <ext:ToolbarFill runat="server" />
                                            <ext:Button ID="btnExcelHomol" runat="server" Text="Exportar Homologación de Registros" 
                                                Icon="PageExcel" ToolTip="Obtener Homologación de Registros en un Archivo Excel"
                                                Disabled="true">
                                                <DirectEvents>
                                                    <Click OnEvent="Download_HR" IsUpload="true"
                                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                            e.stopEvent(); 
                                                            MonitComp.StopMask();" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="btnDownload_HR" runat="server" Hidden="true">
                                                <DirectEvents>
                                                    <Click OnEvent="Download_HR" IsUpload="true"
                                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                        MonitComp.StopMask();" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="btnDetallesHide" runat="server" Hidden="true">
                                                <Listeners>
                                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Detalles de Registros...' });
                                                        #{GridDetalleRegistros}.getStore().reload({params:{start:0, sort:('','')}});" />
                                                </Listeners>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <ColumnModel runat="server">
                                    <Columns>
                                        <ext:Column Header="Id" Sortable="true" DataIndex="Id" Width="70" />
                                        <ext:Column Header="Nombre de Archivo" Sortable="true" DataIndex="NombreFichero" Width="100"/>
                                        <ext:DateColumn Header="Fecha de Homologación" Sortable="true" DataIndex="FechaProceso"
                                            Format="yyyy-MM-dd HH:mm:ss" Width="150"/>
                                        <ext:Column Header="Registros Pendientes" Sortable="true" DataIndex="RegistrosPendientes"
                                            Width="150"/>
                                        <ext:Column Header="Registros Inválidos" Sortable="true" DataIndex="RegistrosInvalidos"
                                            Width="150">
                                            <Renderer Fn="link" />
                                        </ext:Column>
                                        <ext:Column Header="Registros Homologados" Sortable="true" DataIndex="RegistrosHomologados"
                                            Width="150"/>
                                    </Columns>
                                </ColumnModel>
                                <Plugins>
                                    <ext:GridFilters runat="server" Local="true" FiltersText="Filtros">
                                        <Filters>
                                            <ext:DateFilter DataIndex="FechaProceso" BeforeText="Antes de" OnText="El día"
                                                AfterText="Después de">
                                                <DatePickerOptions runat="server" TodayText="Hoy" />
                                            </ext:DateFilter>
                                            <ext:StringFilter DataIndex="NombreFichero" />
                                        </Filters>
                                    </ext:GridFilters>
                                </Plugins>
                                <SelectionModel>
                                    <ext:CellSelectionModel runat="server">
                                        <DirectEvents>
                                            <CellSelect OnEvent="CellGridHomologacion_Click" Before="resetToolbar(#{PagingDetalle});
                                                #{GridDetalleRegistros}.getStore().sortInfo = null;" />
                                        </DirectEvents>
                                    </ext:CellSelectionModel>
                                </SelectionModel>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingHomologacion" runat="server" StoreID="StoreHomologacion" DisplayInfo="true"
                                        DisplayMsg="Mostrando Operaciones {0} - {1} de {2}" HideRefresh="true" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="PanelCompRegistros" runat="server" Title="Paso 3. Compensación de Registros"
                        Layout="FitLayout" Border="false" AutoHeight="true" Collapsible="true" Collapsed="true"
                        AutoScroll="true">
                        <Items>
                            <ext:GridPanel ID="GridCompRegistros" runat="server" AutoExpandColumn="NombreFichero" Border="false"
                                Layout="FitLayout" AutoHeight="true" AutoScroll="true" AutoWidth="true">
                                <LoadMask ShowMask="false" />
                                <Store>
                                    <ext:Store ID="StoreCompReg" runat="server" RemoteSort="true" AutoLoad="false"
                                        OnRefreshData="StoreCompReg_RefreshData">
                                        <AutoLoadParams>
                                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                        </AutoLoadParams>
                                        <Proxy>
                                            <ext:PageProxy />
                                        </Proxy>
                                        <DirectEventConfig IsUpload="true" />
                                        <Reader>
                                            <ext:JsonReader IDProperty="Id">
                                                <Fields>
                                                    <ext:RecordField Name="Id" />
                                                    <ext:RecordField Name="NombreFichero" />
                                                    <ext:RecordField Name="FechaProceso" />
                                                    <ext:RecordField Name="RegistrosPendientes" />
                                                    <ext:RecordField Name="RegistrosError" />
                                                    <ext:RecordField Name="RegistrosCompensados" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <TopBar>
                                    <ext:Toolbar runat="server">
                                        <Items>
                                            <ext:ToolbarFill runat="server" />
                                            <ext:Button ID="btnExcelComp" runat="server" Text="Exportar Compensación de Registros"
                                                Icon="PageExcel" ToolTip="Obtener Compensación de Registros en un Archivo Excel"
                                                Disabled="true">
                                                <DirectEvents>
                                                    <Click OnEvent="Download_CR" IsUpload="true"
                                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                            e.stopEvent(); 
                                                            MonitComp.StopMask();" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="btnDownload_CR" runat="server" Hidden="true">
                                                <DirectEvents>
                                                    <Click OnEvent="Download_CR" IsUpload="true"
                                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                        MonitComp.StopMask();" />
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <ColumnModel runat="server">
                                    <Columns>
                                        <ext:Column Header="Id" Sortable="true" DataIndex="Id" Width="70" />
                                        <ext:Column Header="Nombre de Archivo" Sortable="true" DataIndex="NombreFichero" />
                                        <ext:DateColumn Header="Fecha de Compensación" Sortable="true" DataIndex="FechaProceso"
                                            Format="yyyy-MM-dd HH:mm:ss" Width="150" />
                                        <ext:Column Header="Registros Pendientes" Sortable="true" DataIndex="RegistrosPendientes"
                                            Width="150" />
                                        <ext:Column Header="Registros con Error" Sortable="true" DataIndex="RegistrosError"
                                            Width="150">
                                            <Renderer Fn="link" />
                                        </ext:Column>
                                        <ext:Column Header="Registros Compensados" Sortable="true" DataIndex="RegistrosCompensados"
                                            Width="150" />
                                    </Columns>
                                </ColumnModel>
                                <Plugins>
                                    <ext:GridFilters runat="server" Local="true" FiltersText="Filtros">
                                        <Filters>
                                            <ext:DateFilter DataIndex="FechaTX" BeforeText="Antes de" OnText="El día"
                                                AfterText="Después de">
                                                <DatePickerOptions runat="server" TodayText="Hoy" />
                                            </ext:DateFilter>
                                            <ext:NumericFilter DataIndex="Tarjeta" />
                                        </Filters>
                                    </ext:GridFilters>
                                </Plugins>
                                <SelectionModel>
                                    <ext:CellSelectionModel runat="server">
                                        <DirectEvents>
                                            <CellSelect OnEvent="CellGridCompRegistros_Click" Before="resetToolbar(#{PagingDetalle});
                                                #{GridDetalleRegistros}.getStore().sortInfo = null;" />
                                        </DirectEvents>
                                    </ext:CellSelectionModel>
                                </SelectionModel>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingCompRegistros" runat="server" StoreID="StoreCompReg" DisplayInfo="true"
                                        DisplayMsg="Mostrando Operaciones {0} - {1} de {2}" HideRefresh="true" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Items>
                    </ext:Panel>
                </Items>
                <%--<Content>
                    <ext:BorderLayout runat="server">
                        <North Split="true">
                            
                        </North>
                        <Center Split="true">
                            
                        </Center>
                        <South Split="true">
                            
                        </South>
                    </ext:BorderLayout>
                </Content>--%>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
