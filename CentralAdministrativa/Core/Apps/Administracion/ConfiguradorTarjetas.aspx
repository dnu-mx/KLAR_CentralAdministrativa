<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="ConfiguradorTarjetas.aspx.cs" Inherits="Administracion.ConfiguradorTarjetas" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .chng-ppgrid-value
        {
            color:red;
            font-weight:bold;
        }
    </style>
    <style type="text/css">
        .curr-ppgrid-value
        {
            /*background-color:yellow;
            color:black;*/
            color:blue;
            font-weight:bold;
        }
    </style>
    <style type="text/css">
        .set-ppgrid-value
        {
            color:black;
            font-weight:normal;
        }
    </style>

    <script type="text/javascript">
        //var pendingValueRenderer = function (data, metadata) {
        //    var titulo = 'PENDIENTE DE AUTORIZACIÓN&amp;nbsp;';

        //    if (metadata !== undefined) {
        //        metadata.attr = 'ext:qtitle="' + titulo + '"' + ' ext:qtip="Valor actual: ' + data + '"';

        //        metadata.css = 'chng-ppgrid-value';
        //    }

        //    return data;
        //};

        var pendingValueRenderer = function (data, metadata) {
            if (metadata !== undefined) {
                metadata.css = 'chng-ppgrid-value';
            }
            
            return data;
        };

        var currentValueRenderer = function (data, metadata) {
            if (metadata !== undefined) {
                metadata.css = 'curr-ppgrid-value';
            }
            return data;
        };

        var restoreValueRenderer = function (data, metadata) {
            if (metadata !== undefined) {
                metadata.css = 'set-ppgrid-value';
            }
            return data;
        };

        var removePPGridEdition = function (e) {
            return false;
        };

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
    <ext:Window ID="frmGrafica" Title="Historial" Icon="ChartBar"
        runat="server" Width="900" Height="500" Resizable="False" Hidden="true" Closable="true"
        Modal="true" Layout="FitLayout" Draggable="true" Padding="12">
        <Items>
            <ext:Panel ID="Panel2" runat="server" Title="" Split="true" Padding="6" Collapsible="false"
                ShowOnLoad="true" CenterOnLoad="true">
                <LoadMask ShowMask="true" />
                <AutoLoad Url="http://localhost/CentralAdministrativa/PuntoVenta/Maker.aspx" Mode="IFrame" ShowMask="true" MaskMsg="Por favor espere...">
                </AutoLoad>
                <TopBar>
                    <ext:Toolbar ID="Toolbar1" runat="server">
                        <Items>
                            <ext:ToolbarFill ID="ToolbarFill1" />
                            <ext:Button ID="Button1" runat="server" Text="Graficar Historial" Icon="ArrowRotateClockwise">
                                <Listeners>
                                    <Click Handler="#{Panel2}.reload();" />
                                </Listeners>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
            </ext:Panel>
        </Items>
    </ext:Window>

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
                                        Before="var valid= #{cmbCadenaComercial}.isValid(); if (!valid) {} return valid;" />
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
                                <Items>
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
                                            <ext:Button runat="server" ID="btnValoresActuales" Text="Ver Valores Actuales" Icon="ControlStartBlue"
                                                Hidden="true">
                                                <DirectEvents>
                                                    <Click OnEvent="btnValoresActuales_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button runat="server" ID="btnValoresPendientes" Text="Ver Valores por Autorizar" Icon="ControlEndBlue"
                                                Hidden="true">
                                                <DirectEvents>
                                                    <Click OnEvent="btnValoresPendientes_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button runat="server" ID="btnGuardarParams" Text="Guardar Cambios" Icon="Disk">
                                                <DirectEvents>
                                                    <Click OnEvent="btnGuardarParams_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button runat="server" ID="btnAutorizar" Text="Autorizar Cambios" Icon="Tick" Hidden="true">
                                                <DirectEvents>
                                                    <Click OnEvent="btnAutorizar_Click">
                                                        <Confirmation ConfirmRequest="true" Title="Confirmación" Message="¿Confirmas la autorización de los cambios a los parámetros?" />
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Buttons>
                                    </ext:PropertyGrid>
                                    <ext:PropertyGrid ID="GridParamsHidden" runat="server" ValidateRequestMode="Disabled" Width="550" AutoHeight="true" Hidden="true" Disabled="true">
                                        <Source>
                                            <ext:PropertyGridParameter Name="(Nombre)" Value="" />
                                        </Source>
                                    </ext:PropertyGrid>
                                </Items>
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
