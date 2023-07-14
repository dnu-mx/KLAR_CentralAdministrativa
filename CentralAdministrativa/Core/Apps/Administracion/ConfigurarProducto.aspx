<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="ConfigurarProducto.aspx.cs" Inherits="Administracion.ConfigurarProducto" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">

      var iniciaEdicion = function (e) {
            if (e.getKey() === e.ENTER) {
                var grid = entityName == GridRangos,
                record = grid.getSelectionModel().getSelected(),
                index = grid.store.indexOf(record);

                grid.startEditing(index, 1);
            }
        };

        var finEdicion = function (e) {
            Administracion.GuardarRangos(e.record.data.ID_Rango,
                e.record.data.RangoInicial,
                e.record.data.RangoFinal);
        };

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent"  runat="server">
    <ext:Store ID="StoreCadenaComercial" runat="server">
        <Reader>
        <ext:JsonReader IDProperty="ID_Colectiva">
            <Fields>
                <ext:RecordField Name="ID_Colectiva" />
                <ext:RecordField Name="NombreORazonSocial" />
            </Fields>
        </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:BorderLayout ID="MainBorderLayout" runat="server" ValidateRequestMode="Disabled">
        <Center Split="true">
            <ext:FormPanel ID="FormPanelProductos" runat="server" Width="428.5" Title="Productos" Collapsed="false"
                Layout="Fit" AutoScroll="true">
                <TopBar>
                    <ext:Toolbar ID="tbCadenaComercial" runat="server">
                        <Items>
                            <ext:ComboBox ID="cmbCadenaComercial"
                                runat="server"
                                EmptyText="Cadena Comercial"
                                StoreID="StoreCadenaComercial"
                                DisplayField="NombreORazonSocial"
                                ValueField="ID_Colectiva"
                                Width="300"
                                AllowBlank="false" />
                            <ext:TextField ID="txtProducto" runat="server" EmptyText="Producto" />
                            <ext:Button ID="btnBuscarProductos" runat="server" Text="Buscar" Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscarProductos_Click"
                                        Before="var valid= #{cmbCadenaComercial}.isValid(); if (!valid) {} return valid;">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <Content>
                    <ext:Store ID="StoreProductos" runat="server" OnRefreshData="RefreshGridProductos">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_GrupoMA">
                                <Fields>
                                    <ext:RecordField Name="ID_GrupoMA" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridProductos" runat="server" StoreID="StoreProductos" StripeRows="true"
                                RemoveViewState="true" Header="false" Border="false" AutoExpandColumn="Descripcion">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                        <ext:ImageCommandColumn Header="Acciones" Width="60">
                                            <%--<Commands>
                                                <ext:GridCommand Icon="NoteEdit" CommandName="Edit">
                                                    <ToolTip Text="Editar" />
                                                </ext:GridCommand>
                                            </Commands>--%>
                                            <Commands>
                                                <ext:ImageCommand Icon="Tick" CommandName="Select">
                                                    <ToolTip Text="Seleccionar" />
                                                </ext:ImageCommand>
                                            </Commands>
                                        </ext:ImageCommandColumn>
                                        <ext:Column DataIndex="ID_GrupoMA" Hidden="true" />
                                        <ext:Column DataIndex="Descripcion" Header="Descripción" />
                                    </Columns>
                                </ColumnModel>
                                <DirectEvents>
                                    <RowDblClick OnEvent="GridProductos_DblClik">
                                        <ExtraParams>
                                            <ext:Parameter Name="ID_GrupoMA" Value="Ext.encode(#{GridProductos}.getRowsValues({selectedOnly:true})[0].ID_GrupoMA)" Mode="Raw" />
                                            <ext:Parameter Name="Descripcion" Value="Ext.encode(#{GridProductos}.getRowsValues({selectedOnly:true})[0].Descripcion)" Mode="Raw" />
                                        </ExtraParams>
                                    </RowDblClick>
                                    <Command OnEvent="GridProductos_DblClik">
                                        <ExtraParams>
                                            <ext:Parameter Name="CommandName" Value="command" Mode="Raw" />
                                            <ext:Parameter Name="ID_GrupoMA" Value="Ext.encode(record.data.ID_GrupoMA)" Mode="Raw" />
                                            <ext:Parameter Name="Descripcion" Value="Ext.encode(record.data.Descripcion)" Mode="Raw" />
                                        </ExtraParams>
                                    </Command>
                                </DirectEvents>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                                        <DirectEvents>
                                            <RowDeselect OnEvent="QuitarSeleccion">
                                            </RowDeselect>
                                        </DirectEvents>
                                    </ext:RowSelectionModel>
                                </SelectionModel>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreProductos" DisplayInfo="true"
                                        DisplayMsg="Mostrando Productos {0} - {1} de {2}" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:FormPanel>
        </Center>
        <East Split="true">
            <ext:Panel ID="Panel1" runat="server" Width="550" Collapsible="true" Collapsed="true">
                <Items>
                    <ext:BorderLayout ID="BorderLayout1" runat="server">
                        <Center Split="true">
                            <ext:Panel ID="PanelParametros" runat="server" Width="550" Title="Parámetros del Producto " Height="500"
                                Collapsed="true" ValidateRequestMode="Disabled" Collapsible="true" Layout="Fit" AutoScroll="true">
                                <Content>
                                    <ext:PropertyGrid ID="GridPropiedades" runat="server" ValidateRequestMode="Disabled" Width="550">
                                        <Source>
                                            <ext:PropertyGridParameter Name="(Nombre)" Value="" />
                                        </Source>
                                        <View>
                                            <ext:GridView ID="GridView1" ForceFit="true" ScrollOffset="2" runat="server" >
                                                <Listeners>
                                                    <Refresh Handler="this.grid.colModel.setColumnWidth(0, 275);" />
                                                </Listeners>
                                            </ext:GridView>
                                        </View>
                                        <Buttons>
                                            <ext:Button runat="server" ID="btnGuardarParams" Text="Guardar Cambios" Icon="Disk">
                                                <DirectEvents>
                                                    <Click OnEvent="btnGuardarParams_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                        </Buttons>
                                    </ext:PropertyGrid>
                                    <ext:PropertyGrid ID="GridParamsHidden" runat="server" ValidateRequestMode="Disabled" Width="550" AutoHeight="true" Hidden="true" Disabled="true">
                                        <Source>
                                            <ext:PropertyGridParameter Name="(name)" Value="Grid Params" />
                                        </Source>
                                    </ext:PropertyGrid>
                                </Content>
                            </ext:Panel>
                        </Center>
                        <South Split="true">
                            <ext:FormPanel ID="FormPanelRangos" runat="server" Title="Rangos de Tarjetas del Producto " 
                                Layout="FitLayout" Height="200">
                                <Items>
                                    <ext:GridPanel ID="GridRangos" runat="server">
                                        <Store>
                                            <ext:Store ID="StoreRangos" runat="server">
                                                <Reader>
                                                    <ext:JsonReader IDProperty="ID_Rango">
                                                        <Fields>
                                                            <ext:RecordField Name="ID_Rango" />
                                                            <ext:RecordField Name="Clave" />
                                                            <ext:RecordField Name="Descripcion" />
                                                            <ext:RecordField Name="RangoInicial" />
                                                            <ext:RecordField Name="RangoFinal" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Store>
                                        <Listeners>
                                            <KeyDown Fn="iniciaEdicion" />
                                            <AfterEdit Fn="finEdicion" />
                                        </Listeners>
                                        <ColumnModel ID="ColumnModel3" runat="server">
                                            <Columns>
                                                <ext:Column DataIndex="ID_Rango" Hidden="true" />
                                                <ext:Column DataIndex="Clave" Header="Clave" Width="70"/>
                                                <ext:Column DataIndex="Descripcion" Header="Descripción" Width="170"/>
                                                <ext:Column DataIndex="RangoInicial" Header="Rango Inicial" Width="150">
                                                    <Editor>
                                                        <ext:TextField ID="txtEdRangoInicial" runat="server" />
                                                    </Editor>
                                                </ext:Column>
                                                <ext:Column DataIndex="RangoFinal" Header="Rango Final" Width="150">
                                                    <Editor>
                                                        <ext:TextField ID="txtEdRangoFinal" runat="server" />
                                                    </Editor>
                                                </ext:Column>
                                            </Columns>
                                        </ColumnModel>
                                        <SelectionModel>
                                            <ext:RowSelectionModel ID="RowSelectionModel2" runat="server" SingleSelect="true" />
                                        </SelectionModel>
                                    </ext:GridPanel>
                                </Items>
                            </ext:FormPanel>
                        </South>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
        </East>
    </ext:BorderLayout>

</asp:Content>
