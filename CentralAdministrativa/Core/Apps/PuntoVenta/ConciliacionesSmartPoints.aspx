<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ConciliacionesSmartPoints.aspx.cs" Inherits="TpvWeb.ConciliacionesSmartPoints" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
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
    <ext:Hidden ID="hdnIdArchivo" runat="server" />
    <ext:Hidden ID="hdnConsultaWhere" runat="server" />
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true">
            <ext:Panel runat="server" Width="350" Border="false">
                <Content>
                    <ext:BorderLayout ID="LeftBorderLayout" runat="server">
                        <North Split="true">
                            <ext:FormPanel ID="FormPanelBusqueda" runat="server" Title="Filtros" Height="190" LabelWidth="140"
                                Collapsible="true" Border="false" >
                                <Items>
                                    <ext:FieldSet ID="FieldSetBusqueda" runat="server" Border="false">
                                        <Items>
                                            <ext:TextField ID="txtNombreArchivo" runat="server" FieldLabel="Nombre Archivo" Width="320"
                                                MaxLength="200" />
                                            <ext:DateField ID="dfFechaArchivo" runat="server" Vtype="daterange" MaskRe="[0-9\/]" Width="320"
                                                FieldLabel="Fecha Archivo" Format="dd/MM/yyyy" EnableKeyEvents="true" MaxDate="<%# DateTime.Now %>"
                                                AutoDataBind="true" InvalidText="Fecha inválida. Debe tener el formato DD/MM/AAAA" />
                                            <ext:ComboBox ID="cBoxEstatusArchivo" runat="server" FieldLabel="Estatus del Archivo" Width="320"
                                                AllowBlank="false">
                                                <Items>
                                                    <ext:ListItem Text="Procesado" Value="1" />
                                                    <ext:ListItem Text="No Procesado" Value="0" />
                                                </Items>
                                            </ext:ComboBox>
                                            <ext:DateField ID="dfFechaProc" runat="server" Vtype="daterange" MaskRe="[0-9\/]" Width="320"
                                                FieldLabel="Fecha de Procesamiento" Format="dd/MM/yyyy" EnableKeyEvents="true" MaxDate="<%# DateTime.Now %>"
                                                AutoDataBind="true" InvalidText="Fecha inválida. Debe tener el formato DD/MM/AAAA" />
                                        </Items>
                                        <Buttons>
                                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                                <DirectEvents>
                                                    <Click OnEvent="btnLimpiar_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnBuscar_Click" Before="if (!#{txtNombreArchivo}.getValue() && !#{dfFechaArchivo}.getValue() 
                                                        && !#{cBoxEstatusArchivo}.getValue() && !#{dfFechaProc}.getValue() )
                                                        { Ext.Msg.alert('Búsqueda', 'Ingresa al menos un criterio de búsqueda'); return false; }
                                                        else { #{GridResultados}.getStore().removeAll(); }">
                                                        <EventMask ShowMask="true" Msg="Buscando Archivos..." MinDelay="500" />
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Buttons>
                                    </ext:FieldSet>
                                </Items>
                            </ext:FormPanel>
                        </North>
                        <Center Split="true">
                            <ext:GridPanel ID="GridResultados" runat="server" AutoExpandColumn="NombreFichero" Title="Resultados Filtros"
                                Border="false">
                                <Store>
                                    <ext:Store ID="StoreArchivos" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Fichero">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Fichero" />
                                                    <ext:RecordField Name="NombreFichero" />
                                                    <ext:RecordField Name="FechaProceso" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <ColumnModel runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_Fichero" Hidden="true" />
                                        <ext:Column DataIndex="NombreFichero" Header="Archivo" Width="170" />
                                        <ext:DateColumn Header="Fecha de Proceso" DataIndex="FechaProceso"
                                            Format="yyyy-MM-dd HH:mm:ss" Width="120"/>
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel SingleSelect="true" />
                                </SelectionModel>
                                <DirectEvents>
                                    <RowClick OnEvent="selectRowResultados_Event">
                                        <ExtraParams>
                                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridResultados}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                        </ExtraParams>
                                    </RowClick>
                                </DirectEvents>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreArchivos" DisplayInfo="true"
                                        DisplayMsg="{0} - {1} de {2}" HideRefresh="true">
                                        <Items>
                                            <ext:Button ID="btnLlenaGrids_Hide" runat="server" Hidden="true">
                                                <DirectEvents>
                                                    <Click Timeout="360000" />
                                                </DirectEvents>
                                                <Listeners>
                                                    <Click Handler="resetToolbar(#{PagingTBOpsConc});
                                                        #{GridOpsConciliadas}.getStore().sortInfo = null;
                                                        #{GridOpsConciliadas}.getStore().reload({params:{start:0, sort:('','')}});
                                                        resetToolbar(#{PagingTBOpsSiANoP});
                                                        #{GridOpsSiANoP}.getStore().sortInfo = null;
                                                        #{GridOpsSiANoP}.getStore().reload({params:{start:0, sort:('','')}});
                                                        resetToolbar(#{PagingTBOpsNoASiP});
                                                        #{GridOpsNoASiP}.getStore().sortInfo = null;
                                                        #{GridOpsNoASiP}.getStore().reload({params:{start:0, sort:('','')}});
                                                        Ext.net.Mask.show({ msg : 'Obteniendo Operaciones del Archivo...' });
                                                        ConcSP.StopMask();" />
                                                </Listeners>
                                            </ext:Button>
                                        </Items>
                                    </ext:PagingToolbar>
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </West>
        <Center Split="true">
            <ext:Panel ID="PanelCentral" runat="server" Height="250" Border="false" Title="Operaciones">
                <Items>
                    <ext:BorderLayout runat="server">
                        <Center>
                            <ext:TabPanel runat="server">
                                <Items>
                                    <ext:FormPanel ID="FormPanelOpsConc" runat="server" Title="Conciliadas" AutoScroll="true"
                                        Border="false" Layout="FitLayout">
                                        <Items>
                                            <ext:GridPanel ID="GridOpsConciliadas" runat="server" Layout="FitLayout">
                                                <Store>
                                                    <ext:Store ID="StoreOpsConciliadas" runat="server" RemoteSort="true" AutoLoad="false"
                                                        OnRefreshData="StoreOpsConciliadas_RefreshData" OnDataBinding="StoreOpsConciliadas_Msg">
                                                        <AutoLoadParams>
                                                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                                        </AutoLoadParams>
                                                        <Proxy>
                                                            <ext:PageProxy />
                                                        </Proxy>
                                                        <DirectEventConfig IsUpload="true" />
                                                        <Reader>
                                                            <ext:JsonReader IDProperty="ID_Operacion">
                                                                <Fields>
                                                                    <ext:RecordField Name="ID_Operacion" />
                                                                    <ext:RecordField Name="Fecha" />
                                                                    <ext:RecordField Name="Clave" />
                                                                    <ext:RecordField Name="Sucursal" />
                                                                    <ext:RecordField Name="Afiliacion" />
                                                                    <ext:RecordField Name="Terminal" />
                                                                    <ext:RecordField Name="Estatus" />
                                                                    <ext:RecordField Name="CodRespuesta" />
                                                                    <ext:RecordField Name="Autorizacion" />
                                                                </Fields>
                                                            </ext:JsonReader>
                                                        </Reader>
                                                    </ext:Store>
                                                </Store>
                                                <ColumnModel runat="server">
                                                    <Columns>
                                                        <ext:Column DataIndex="ID_Operacion" Header="ID Operación" />
                                                        <ext:DateColumn Header="Fecha" Sortable="true" DataIndex="Fecha"
                                                            Format="yyyy-MM-dd HH:mm:ss"/>
                                                        <ext:Column DataIndex="Clave" Header="Clave" />
                                                        <ext:Column DataIndex="Sucursal" Header="Sucursal" />
                                                        <ext:Column DataIndex="Afiliacion" Header="Afiliación" />
                                                        <ext:Column DataIndex="Terminal" Header="Terminal" />
                                                        <ext:Column DataIndex="Estatus" Header="Estatus" />
                                                        <ext:Column DataIndex="CodRespuesta" Header="Cód. Respuesta" />
                                                        <ext:Column DataIndex="Autorizacion" Header="Autorización" />
                                                    </Columns>
                                                </ColumnModel>
                                                <SelectionModel>
                                                    <ext:RowSelectionModel SingleSelect="true" />
                                                </SelectionModel>
                                                <BottomBar>
                                                    <ext:PagingToolbar ID="PagingTBOpsConc" runat="server" StoreID="StoreOpsConciliadas" DisplayInfo="true"
                                                        DisplayMsg="{0} - {1} de {2}" HideRefresh="true" />
                                                </BottomBar>
                                                <TopBar>
                                                    <ext:Toolbar runat="server">
                                                        <Items>
                                                            <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                            <ext:Button ID="btnExcelOpsConc" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="true">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnExcelOpsConc_Click" IsUpload="true"
                                                                        After="Ext.net.Mask.show({ msg : 'Exportando Operaciones a Excel...' });
                                                                            e.stopEvent(); 
                                                                            ConcSP.StopMask();" />
                                                                </DirectEvents>
                                                            </ext:Button>
                                                        </Items>
                                                    </ext:Toolbar>
                                                </TopBar>
                                            </ext:GridPanel>
                                        </Items>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelOpsSiANoP" runat="server" Title="En Archivo, no en Plataforma"
                                        AutoScroll="true" Border="false" Layout="FitLayout">
                                        <Items>
                                            <ext:GridPanel ID="GridOpsSiANoP" runat="server" Layout="FitLayout">
                                                <Store>
                                                    <ext:Store ID="StoreOpsSiANoP" runat="server" RemoteSort="true" AutoLoad="false"
                                                        OnRefreshData="StoreOpsSiANoP_RefreshData" OnDataBinding="StoreOpsSiANoP_Msg">
                                                        <AutoLoadParams>
                                                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                                        </AutoLoadParams>
                                                        <Proxy>
                                                            <ext:PageProxy />
                                                        </Proxy>
                                                        <DirectEventConfig IsUpload="true" />
                                                        <Reader>
                                                            <ext:JsonReader IDProperty="Consecutivo">
                                                                <Fields>
                                                                    <ext:RecordField Name="Consecutivo" />
                                                                    <ext:RecordField Name="Fecha" />
                                                                    <ext:RecordField Name="Hora" />
                                                                    <ext:RecordField Name="ID_Sucursal" />
                                                                    <ext:RecordField Name="Sucursal" />
                                                                    <ext:RecordField Name="PuntoVenta" />
                                                                    <ext:RecordField Name="Afiliacion" />
                                                                    <ext:RecordField Name="Monto" />
                                                                    <ext:RecordField Name="Autorizacion" />
                                                                    <ext:RecordField Name="Estatus" />
                                                                </Fields>
                                                            </ext:JsonReader>
                                                        </Reader>
                                                    </ext:Store>
                                                </Store>
                                                <ColumnModel runat="server">
                                                    <Columns>
                                                        <ext:Column DataIndex="Consecutivo" Header="Consecutivo" />
                                                        <ext:DateColumn Header="Fecha" Sortable="true" DataIndex="Fecha"
                                                            Format="yyyy-MM-dd"/>
                                                        <ext:Column Header="Hora" DataIndex="Hora"/>
                                                        <ext:Column DataIndex="ID_Sucursal" Header="ID Sucursal" />
                                                        <ext:Column DataIndex="Sucursal" Header="Sucursal" />
                                                        <ext:Column DataIndex="PuntoVenta" Header="PuntoVenta" />
                                                        <ext:Column DataIndex="Afiliacion" Header="Afiliación" />
                                                        <ext:Column Header="Monto" DataIndex="Monto">
                                                            <Renderer Format="UsMoney" />
                                                        </ext:Column>
                                                        <ext:Column DataIndex="Autorizacion" Header="Autorización" />
                                                        <ext:Column DataIndex="Estatus" Header="Estatus" />
                                                    </Columns>
                                                </ColumnModel>
                                                <SelectionModel>
                                                    <ext:RowSelectionModel SingleSelect="true" />
                                                </SelectionModel>
                                                <BottomBar>
                                                    <ext:PagingToolbar ID="PagingTBOpsSiANoP" runat="server" StoreID="StoreOpsSiANoP" DisplayInfo="true"
                                                        DisplayMsg="{0} - {1} de {2}" HideRefresh="true" />
                                                </BottomBar>
                                                <TopBar>
                                                    <ext:Toolbar runat="server">
                                                        <Items>
                                                            <ext:ToolbarFill runat="server" />
                                                            <ext:Button ID="btnExcelOpsSiANoP" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="true">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnExcelOpsSiANoP_Click" IsUpload="true"
                                                                         After="Ext.net.Mask.show({ msg : 'Exportando Operaciones a Excel...' });
                                                                            e.stopEvent(); 
                                                                            ConcSP.StopMask();" />
                                                                </DirectEvents>
                                                            </ext:Button>
                                                        </Items>
                                                    </ext:Toolbar>
                                                </TopBar>
                                            </ext:GridPanel>
                                        </Items>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelOpsNoASiP" runat="server" Title="En Plataforma, no en Archivo"
                                        AutoScroll="true" Border="false" Layout="FitLayout">
                                        <Items>
                                            <ext:GridPanel ID="GridOpsNoASiP" runat="server" Layout="FitLayout">
                                                <Store>
                                                    <ext:Store ID="StoreOpsNoASiP" runat="server" RemoteSort="true" AutoLoad="false"
                                                        OnRefreshData="StoreOpsNoASiP_RefreshData" OnDataBinding="StoreOpsNoASiP_Msg">
                                                        <AutoLoadParams>
                                                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                                        </AutoLoadParams>
                                                        <Proxy>
                                                            <ext:PageProxy />
                                                        </Proxy>
                                                        <DirectEventConfig IsUpload="true" />
                                                        <Reader>
                                                            <ext:JsonReader IDProperty="ID_Operacion">
                                                                <Fields>
                                                                    <ext:RecordField Name="ID_Operacion" />
                                                                    <ext:RecordField Name="Fecha" />
                                                                    <ext:RecordField Name="Clave" />
                                                                    <ext:RecordField Name="Sucursal" />
                                                                    <ext:RecordField Name="Afiliacion" />
                                                                    <ext:RecordField Name="Terminal" />
                                                                    <ext:RecordField Name="Estatus" />
                                                                    <ext:RecordField Name="CodRespuesta" />
                                                                    <ext:RecordField Name="Autorizacion" />
                                                                </Fields>
                                                            </ext:JsonReader>
                                                        </Reader>
                                                    </ext:Store>
                                                </Store>
                                                <ColumnModel runat="server">
                                                    <Columns>
                                                        <ext:Column DataIndex="ID_Operacion" Header="ID Operación" />
                                                        <ext:DateColumn Header="Fecha" Sortable="true" DataIndex="Fecha"
                                                            Format="yyyy-MM-dd HH:mm:ss" />
                                                        <ext:Column DataIndex="Clave" Header="Clave" />
                                                        <ext:Column DataIndex="Sucursal" Header="Sucursal" />
                                                        <ext:Column DataIndex="Afiliacion" Header="Afiliación" />
                                                        <ext:Column DataIndex="Terminal" Header="Terminal" />
                                                        <ext:Column DataIndex="Estatus" Header="Estatus" />
                                                        <ext:Column DataIndex="CodRespuesta" Header="Cód. Respuesta" />
                                                        <ext:Column DataIndex="Autorizacion" Header="Autorización" />
                                                    </Columns>
                                                </ColumnModel>
                                                <SelectionModel>
                                                    <ext:RowSelectionModel SingleSelect="true" />
                                                </SelectionModel>
                                                <BottomBar>
                                                    <ext:PagingToolbar ID="PagingTBOpsNoASiP" runat="server" StoreID="StoreOpsNoASiP" DisplayInfo="true"
                                                        DisplayMsg="{0} - {1} de {2}" HideRefresh="true" />
                                                </BottomBar>
                                                <TopBar>
                                                    <ext:Toolbar runat="server">
                                                        <Items>
                                                            <ext:ToolbarFill ID="ToolbarFill3" runat="server" />
                                                            <ext:Button ID="btnExcelOpsNoASiP" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="true">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnExcelOpsNoASiP_Click" IsUpload="true"
                                                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                                            e.stopEvent(); 
                                                                            ConcSP.StopMask();" />
                                                                </DirectEvents>
                                                            </ext:Button>
                                                        </Items>
                                                    </ext:Toolbar>
                                                </TopBar>
                                            </ext:GridPanel>
                                        </Items>
                                    </ext:FormPanel>
                                </Items>
                            </ext:TabPanel>
                        </Center>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
