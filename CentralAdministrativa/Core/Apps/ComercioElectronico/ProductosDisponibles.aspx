<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ProductosDisponibles.aspx.cs" Inherits="ComercioElectronico.ProductosDisponibles" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var prepareToolbar = function (grid, toolbar, rowIndex, record) {
            if (record.get("Disponible") == 1) {
                toolbar.items.get(1).hide();
            } else {
            //if (record.get("Disponible") == 0) {
                toolbar.items.get(0).hide();
            }
        }
    </script>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Viewport runat="server" Layout="BorderLayout">
        <Items>
            <ext:Panel runat="server" Region="North" Layout="FitLayout" Border="false">
                <TopBar>
                    <ext:Toolbar ID="ToolbarConsulta" runat="server">
                        <Items>
                            <ext:ComboBox ID="cBoxFamilia" runat="server" FieldLabel="Familia" LabelAlign="Right"
                                LabelWidth="50" Width="180" ValueField="Id" DisplayField="Name">
                                <Store>
                                    <ext:Store ID="StoreFamilias" runat="server">
                                        <Model>
                                            <ext:Model runat="server" IDProperty="Id">
                                                <Fields>
                                                    <ext:ModelField Name="Id" />
                                                    <ext:ModelField Name="Name" />
                                                </Fields>
                                            </ext:Model>
                                        </Model>
                                    </ext:Store>
                                </Store>
                            </ext:ComboBox>
                            <ext:TextField ID="txtSKU" runat="server" FieldLabel="SKU" LabelAlign="Right"
                                LabelWidth="30" Width="150"/>
                            <ext:TextField ID="txtProducto" runat="server" FieldLabel="Nombre" LabelAlign="Right"
                                LabelWidth="50" Width="150"/>
                            <ext:ComboBox ID="cBoxSucursal" runat="server" FieldLabel="Sucursal" LabelAlign="Right"
                                LabelWidth="50" Width="180" ValueField="id_sucursal" DisplayField="nombre" AllowBlank="false">
                                <Store>
                                    <ext:Store ID="StoreSucursales" runat="server">
                                        <Model>
                                            <ext:Model runat="server" IDProperty="id_sucursal">
                                                <Fields>
                                                    <ext:ModelField Name="id_sucursal" />
                                                    <ext:ModelField Name="clave" />
                                                    <ext:ModelField Name="nombre" />
                                                </Fields>
                                            </ext:Model>
                                        </Model>
                                    </ext:Store>
                                </Store>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cBoxDisponibilidad" runat="server" FieldLabel="Disponibilidad" LabelAlign="Right"
                                LabelWidth="80" Width="180">
                                <Items>
                                    <ext:ListItem Text="Disponibles" Value="1" />
                                    <ext:ListItem Text="No Disponibles" Value="0" />
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:ToolbarFill ID="ToolbarFill2" runat="server" Width="5"/>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Before="var valid= #{cBoxSucursal}.isValid(); if (!valid) {} return valid;">
                                        <EventMask ShowMask="true" Msg="Buscando Productos..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
            </ext:Panel>
            <ext:GridPanel ID="GridProductos" runat="server" StripeRows="true" Border="false" Region="Center" Layout="FitLayout"
                OnRefreshData="btnBuscar_Click" >
                <Store>
                    <ext:Store ID="StoreProductos" runat="server">
                        <Model>
                            <ext:Model runat="server" IDProperty="ID_Producto">
                                <Fields>    
                                    <ext:ModelField Name="ID_Producto" />
                                    <ext:ModelField Name="SKU" />
                                    <ext:ModelField Name="Familia" />
                                    <ext:ModelField Name="NombreProducto" />
                                    <ext:ModelField Name="Descripcion" />
                                    <ext:ModelField Name="Disponible" />
                                </Fields>
                            </ext:Model>
                        </Model>
                    </ext:Store>
                </Store>
                <TopBar>
                    <ext:Toolbar ID="ToolbarExportar" runat="server">
                        <Items>
                            <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="true">
                                <DirectEvents>
                                    <Click OnEvent="ExportGridToExcel" IsUpload="true" Success="Ext.Net.DirectMethods.Download({IsUpload:true})">
                                        <ExtraParams>
                                            <ext:Parameter Name="GridToExport" Value="Ext.encode(#{GridProductos}.getRowsValues({selectedOnly : false}))" Mode="Raw" />
                                        </ExtraParams>
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                             <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                               <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <ColumnModel runat="server">
                    <Columns>
                        <ext:CommandColumn Header="Disponible" Width="70">
                            <PrepareToolbar Fn="prepareToolbar" />
                            <Commands>
                                <ext:GridCommand Icon="Tick" CommandName="Disp">
                                    <ToolTip Text="Disponible. Marcar como NO DISPONIBLE." />
                                </ext:GridCommand>
                                <ext:GridCommand Icon="Cross" CommandName="NoDisp">
                                    <ToolTip Text="No Disponible. Marcar como DISPONIBLE." />
                                </ext:GridCommand>
                            </Commands>
                            <DirectEvents>
                                <Command OnEvent="EjecutarComando">
                                    <Confirmation ConfirmRequest="true" Title="Confirmación" 
                                        Message="¿Estás seguro de cambiar la Disponibilidad del Producto?" />
                                    <ExtraParams>
                                        <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                        <ext:Parameter Name="ID_Producto" Value="Ext.encode(record.data.ID_Producto)" Mode="Raw" />
                                        <ext:Parameter Name="Disponible" Value="Ext.encode(record.data.Disponible)" Mode="Raw" />
                                    </ExtraParams>
                                </Command>
                            </DirectEvents>
                        </ext:CommandColumn>
                        <ext:Column ColumnID="SKU" Header="SKU" DataIndex="SKU" Width="50" />
                        <ext:Column ColumnID="Familia" Header="Familia" DataIndex="Familia" Width="150" />
                        <ext:Column ColumnID="NombreProducto" Header="Nombre" DataIndex="NombreProducto" Width="150" />
                        <ext:Column ColumnID="Descripcion" Header="Descripción" DataIndex="Descripcion" Width="480" />
                    </Columns>
                </ColumnModel>
                <SelectionModel>
                    <ext:RowSelectionModel SingleSelect="true" />
                </SelectionModel>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" DisplayInfo="true"
                        DisplayMsg="Mostrando Resultados de Productos {0} - {1} de {2}" />
                </BottomBar>
            </ext:GridPanel>
        </Items>
    </ext:Viewport>
</asp:Content>
