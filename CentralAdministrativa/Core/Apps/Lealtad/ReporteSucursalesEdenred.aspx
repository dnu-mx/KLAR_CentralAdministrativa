<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReporteSucursalesEdenred.aspx.cs"
    Inherits="Lealtad.ReporteSucursalesEdenred" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
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
                    <ext:Store ID="StoreClaveCadena" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Cadena">
                                <Fields>
                                    <ext:RecordField Name="ID_Cadena" />
                                    <ext:RecordField Name="ClaveCadena" />
                                    <ext:RecordField Name="NombreComercial" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:Store ID="StoreClaveSucursal" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="id_sucursal">
                                <Fields>
                                    <ext:RecordField Name="id_sucursal" />
                                    <ext:RecordField Name="clave" />
                                    <ext:RecordField Name="nombre" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="clave" Direction="ASC" />
                    </ext:Store>
                    <ext:Store ID="StoreSucursal" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="id_sucursal">
                                <Fields>
                                     <ext:RecordField Name="id_sucursal" />
                                    <ext:RecordField Name="clave" />
                                    <ext:RecordField Name="nombre" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="nombre" Direction="ASC" />
                    </ext:Store>
                    <ext:Store ID="StoreEstados" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="CveEstado">
                                <Fields>
                                    <ext:RecordField Name="CveEstado" />
                                    <ext:RecordField Name="Estado" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Content>
                <Items>
                    <ext:Panel ID="Panel1" runat="server" Border="false" Padding="10">
                        <Items>
                            <ext:ComboBox ID="cBoxClaveCadena" runat="server" FieldLabel="Clave Cadena" ListWidth="350"
                                Width="280" EmptyText="Todas" StoreID="StoreClaveCadena" DisplayField="ClaveCadena" ValueField="ID_Cadena"
                                Mode="Local" AutoSelect="true" Editable="true" ForceSelection="true" TypeAhead="true" MinChars="1"
                                MatchFieldWidth="false" Name="claveCadena">
                                <Items>
                                    <ext:ListItem Text="( Todas )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:TextField ID="txtCadena" runat="server" FieldLabel="Cadena" Width="280" EmptyText="Todas" />
                            <ext:ComboBox ID="cBoxClaveSucursal" runat="server" FieldLabel="Clave Sucursal" ListWidth="350"
                                Width="280" EmptyText="Todas" StoreID="StoreClaveSucursal" DisplayField="clave" ValueField="id_sucursal"
                                Mode="Local" AutoSelect="true" Editable="true" ForceSelection="true" TypeAhead="true" MinChars="1"
                                MatchFieldWidth="false" Name="claveSucursal">
                                <Items>
                                    <ext:ListItem Text="( Todas )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:TextField ID="txtSucursal" runat="server" FieldLabel="Nombre Sucursal" Width="280" EmptyText="Todas" />
                            <ext:ComboBox ID="cBoxEstado" runat="server" FieldLabel="Estado" Width="280" ListWidth="350"
                                EmptyText="Todos" StoreID="StoreEstados" DisplayField="Estado" ValueField="CveEstado">
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="" />
                                </Items>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cBoxActiva" runat="server" FieldLabel="Activa" EmptyText="Todas"
                                Width="280">
                                 <Items>
                                     <ext:ListItem Text="( Todas )" Value="-1" />
                                     <ext:ListItem Text="Sí" Value="1" />
                                     <ext:ListItem Text="No" Value="0" />
                                </Items>
                            </ext:ComboBox>
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
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Sucursales...' });
                                        #{GridPanelSucursales}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                        Before="resetToolbar(#{PagingToolBar1});
                                        #{GridPanelSucursales}.getStore().sortInfo = null;">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                            RepSucEdn.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>
            </ext:FormPanel>
        </West>
        <Center Split="true" Collapsible="false">
            <ext:FormPanel ID="FormPanelSucursales" runat="server" Layout="FitLayout" Title="Sucursales obtenidas con los Filtros">
                <Items>
                    <ext:GridPanel ID="GridPanelSucursales" runat="server" StripeRows="true" Layout="FitLayout" Region="Center">
                        <Store>
                            <ext:Store ID="StoreSucursales" runat="server" OnSubmitData="StoreSubmit" RemoteSort="true" 
                                OnRefreshData="StoreSucursales_RefreshData" AutoLoad="false">
                                <AutoLoadParams>
                                    <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                </AutoLoadParams>
                                <Proxy>
                                    <ext:PageProxy />
                                </Proxy>                                
                                <DirectEventConfig IsUpload="true"/>
                                <Reader>
                                    <ext:JsonReader IDProperty="ID">
                                        <Fields>
                                            <ext:RecordField Name="ClaveCadena" />
                                            <ext:RecordField Name="NombreCadena" />
                                            <ext:RecordField Name="ClaveSucursal" />
                                            <ext:RecordField Name="NombreSucursal" />
                                            <ext:RecordField Name="Direccion" />
                                            <ext:RecordField Name="Colonia" />
                                            <ext:RecordField Name="Ciudad" />
                                            <ext:RecordField Name="CP" />
                                            <ext:RecordField Name="Estado" />
                                            <ext:RecordField Name="Telefono" />
                                            <ext:RecordField Name="Latitud" />
                                            <ext:RecordField Name="Longitud" />
                                            <ext:RecordField Name="Activa" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <ColumnModel ID="ColumnModel12" runat="server">
                            <Columns>
                                <ext:Column ColumnID="ClaveCadena" Header="Clave Cadena" Sortable="true" DataIndex="ClaveCadena" Width="80" />
                                <ext:Column ColumnID="NombreCadena" Header="Nombre Cadena" Sortable="true" DataIndex="NombreCadena" />
                                <ext:Column ColumnID="ClaveSucursal" Header="Clave Sucursal" Sortable="true" DataIndex="ClaveSucursal" Width="90" />
                                <ext:Column ColumnID="NombreSucursal" Header="Nombre Sucursal" Sortable="true" DataIndex="NombreSucursal" />
                                <ext:Column ColumnID="Direccion" Header="Dirección" Sortable="true" DataIndex="Direccion" Width="150" />
                                <ext:Column ColumnID="Colonia" Header="Colonia" Sortable="true" DataIndex="Colonia" Width="120" />
                                <ext:Column ColumnID="Ciudad" Header="Ciudad" Sortable="true" DataIndex="Ciudad" />
                                <ext:Column ColumnID="CP" Header="Código Postal" Sortable="true" DataIndex="CP" Width="80" />
                                <ext:Column ColumnID="Estado" Header="Estado" Sortable="true" DataIndex="Estado" Width="60" />
                                <ext:Column ColumnID="Telefono" Header="Teléfono" Sortable="true" DataIndex="Telefono" />
                                <ext:Column ColumnID="Latitud" Header="Latitud" Sortable="true" DataIndex="Latitud" Width="80" />
                                <ext:Column ColumnID="Longitud" Header="Longitud" Sortable="true" DataIndex="Longitud" Width="80" />
                                <ext:Column ColumnID="Activa" Header="Activa" Sortable="true" DataIndex="Activa" Width="50" />
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
                                                    RepSucEdn.StopMask();" />
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnExportCSV" runat="server" Text="Exportar a CSV" Icon="PageAttach" Disabled="true">
                                        <Listeners>
                                            <Click Handler="submitValue(#{GridPanelSucursales}, #{FormatType}, 'csv');"  />
                                        </Listeners>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                        <BottomBar>
                            <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreSucursales" DisplayInfo="true"
                                DisplayMsg="Mostrando Sucursales {0} - {1} de {2}" />
                        </BottomBar>
                    </ext:GridPanel>
                </Items>
            </ext:FormPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
