<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="AsignarOperadoresASucursal.aspx.cs" Inherits="TpvWeb.AsignarOperadoresASucursal" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">

        var prepareToolbar = function (grid, toolbar, rowIndex, record) {
            // for example hide 'Edit' button if price < 50
            if (record.get("ID_EstatusAsignacion") == 0) { //Activo

                //toolbar.items.get(0).hide(); //Delete
                toolbar.items.get(1).hide(); //asgina
                toolbar.items.get(2).hide(); //asgina
            } else if (record.get("ID_EstatusAsignacion") == 1) { //otra por otra sucursal

                //toolbar.items.get(0).hide(); //Delete
                toolbar.items.get(1).hide(); //asgina
                toolbar.items.get(0).hide(); //asgina
            } if (record.get("ID_EstatusAsignacion") == 2) { //Sucursal Actual

                //toolbar.items.get(0).hide(); //Delete
                toolbar.items.get(0).hide(); //asgina
                toolbar.items.get(2).hide(); //asgina
            }

        };

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center Split="true">
            <ext:Panel ID="Panel2" runat="server" Title="Sucursales" Collapsed="false" Layout="Fit"
                AutoScroll="true" Width="428.5">
                <Content>
                    <ext:Store ID="Store2" runat="server" OnRefreshData="RefreshGrid" GroupField="ColectivaPadre">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="NombreORazonSocial" />
                    </ext:Store>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store2" StripeRows="true"
                                Header="false" Border="false" AutoExpandColumn="NombreORazonSocial">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel1" runat="server">
                                </ColumnModel>
                                <DirectEvents>
                                    <RowDblClick OnEvent="GridEmpleados_DblClik">
                                    </RowDblClick>
                                </DirectEvents>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                                        <DirectEvents>
                                            <RowDeselect OnEvent="QuitarSeleccion">
                                            </RowDeselect>
                                        </DirectEvents>
                                    </ext:RowSelectionModel>
                                </SelectionModel>
                                <DirectEvents>
                                    <Command OnEvent="EjecutarComando">
                                        <ExtraParams>
                                            <ext:Parameter Name="id" Value="record.data.ID" Mode="Raw" />
                                            
                                        </ExtraParams>
                                    </Command>
                                </DirectEvents>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="Store2" BufferResize="100"
                                        DisplayInfo="true" DisplayMsg="Mostrando Sucursales {0} - {1} de {2}" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </Center>
        <East Split="true">
            <ext:FormPanel ID="FormPanel1" Width="628.5" Title="Operadores Disponibles" runat="server"
                Collapsed="true" Collapsible="true" Border="false" Layout="Fit">
                <Content>
                    <ext:Store ID="storeOperadores" runat="server" GroupField="Estatus">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                                <Fields>
                                    <ext:RecordField Name="ID_OperadorSucursal" />
                                    <ext:RecordField Name="ID_ColectivaSucursal" />
                                    <ext:RecordField Name="ID_ColectivaOperador" />
                                    <ext:RecordField Name="Estatus" />
                                    <ext:RecordField Name="ID_ColectivaSucursal" />
                                    <ext:RecordField Name="ClaveSucursal" />
                                    <ext:RecordField Name="Sucursal" />
                                    <ext:RecordField Name="ID_ColectivaOperador" />
                                    <ext:RecordField Name="ClaveOperador" />
                                    <ext:RecordField Name="Operador" />
                                    <ext:RecordField Name="ID_EstatusAsignacion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Content>
                <Items>
                    <%--                    <ext:Panel ID="Panel1" runat="server" Title="Sucursal" Layout="FitLayout" AutoHeight="true"
                        FormGroup="true">
                        <Items>--%>
                    <ext:GridPanel ID="GridPanel2" runat="server" StoreID="storeOperadores" StripeRows="true"
                        Header="false" Border="false" AutoExpandColumn="Operador" Layout="FitLayout">
                        <LoadMask ShowMask="false" />
                        <ColumnModel ID="ColumnModel2" runat="server">
                            <Columns>
                                <ext:CommandColumn ColumnID="Acciones" Width="50" Header="Acciones">
                                    <Commands>
                                        <ext:GridCommand CommandName="Asignar" Icon="UserAdd" ToolTip-Text="Asignar Operador a Sucursal Actual">
                                        </ext:GridCommand>
                                        <ext:GridCommand CommandName="Asignar" Icon="User" ToolTip-Text="Desasignar Operador de Sucursal Actual">
                                        </ext:GridCommand>
                                        <ext:GridCommand CommandName="Asignar" Icon="UserAlert" ToolTip-Text="Asignar Operador a Sucursal Actual">
                                        </ext:GridCommand>
                                    </Commands>
                                    <PrepareToolbar Fn="prepareToolbar" />
                                </ext:CommandColumn>
                                <ext:Column ColumnID="ID_OperadorSucursal" Header="ID_OperadorSucursal" Hidden="true"
                                    Sortable="true" DataIndex="ID_OperadorSucursal" />
                                <ext:Column ColumnID="ID_ColectivaSucursal" Header="ID_ColectivaSucursal" Hidden="true"
                                    Sortable="true" DataIndex="ID_ColectivaSucursal" />
                                <ext:Column ColumnID="ID_ColectivaOperador" Header="ID_ColectivaOperador" Hidden="true"
                                    Sortable="true" DataIndex="ID_ColectivaOperador" />
                                <ext:Column ColumnID="Estatus" Header="Estatus" Width="100" Sortable="true" DataIndex="Estatus" />
                                <ext:Column ColumnID="Clave" Header="ClaveOperador" Width="100"  Hidden="true" Sortable="true" DataIndex="ClaveOperador" />
                                <ext:Column ColumnID="Nombre" Header="Operador" Width="300"  Sortable="true" DataIndex="Operador" />
                                <ext:Column ColumnID="Clave" Header="Clave Sucursal" Width="100"  Sortable="true" DataIndex="ClaveSucursal" />
                                <ext:Column ColumnID="Sucursal" Header="Sucursal" Width="300" Sortable="true" DataIndex="Sucursal" />
                            </Columns>
                        </ColumnModel>
                        <Plugins>
                            <ext:GridFilters runat="server" ID="GridFilters1" Local="true">
                                <Filters>

                                    <ext:StringFilter DataIndex="Operador" />
                                    <ext:StringFilter DataIndex="ClaveOperador" />
                                    <ext:StringFilter DataIndex="Sucursal" />
                                    <ext:StringFilter DataIndex="ClaveSucursal" />
                                   
                                </Filters>
                            </ext:GridFilters>
                        </Plugins>
                        <DirectEvents>
                            <Command OnEvent="EjecutarComando">
                                <ExtraParams>
                                    <ext:Parameter Name="command" Value="command" Mode="Raw" />
                                    <ext:Parameter Name="ID_OperadorSucursal" Value="record.data.ID_OperadorSucursal"
                                        Mode="Raw" />
                                    <ext:Parameter Name="ID_ColectivaSucursal" Value="record.data.ID_ColectivaSucursal"
                                        Mode="Raw" />
                                    <ext:Parameter Name="ID_ColectivaOperador" Value="record.data.ID_ColectivaOperador"
                                        Mode="Raw" />
                                </ExtraParams>
                            </Command>
                        </DirectEvents>
                        <BottomBar>
                            <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="storeOperadores" BufferResize="100"
                                DisplayInfo="true" DisplayMsg="Mostrando Operadores {0} - {1} de {2}" />
                        </BottomBar>
                        <View>
                            <ext:GroupingView ID="GroupingView1" runat="server" ForceFit="true" MarkDirty="false"
                                ShowGroupName="false" EnableNoGroups="true" HideGroupedColumn="true" />
                        </View>
                    </ext:GridPanel>
                </Items>
                <%--                    </ext:Panel>
                </Items>--%>
            </ext:FormPanel>
        </East>
    </ext:BorderLayout>
</asp:Content>
