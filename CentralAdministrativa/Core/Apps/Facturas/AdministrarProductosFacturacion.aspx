<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="AdministrarProductosFacturacion.aspx.cs" Inherits="Facturas.AdministrarProductosFacturacion" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var validaDatos = function (panel, unidad, producto) {
            if (!panel.getForm().isValid() || !unidad.isValid()
                || !producto.isValid()) {
                return false;
            }
            else {
                return true;
            }
        }

        var hideMenuItems = function(grid){
            var ms = grid.view.hmenu.items;
            ms.get("columns").hide(true);
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Hidden ID="hdnIdUnidad" runat="server" />
    <ext:Hidden ID="hdnClaveProdServ" runat="server" />
    <ext:Window ID="WdwNuevoEvento" runat="server" Width="450" Height="230" Hidden="true" Modal="true" Resizable="false"
        Closable="true" Icon="Add">
        <Items>
            <ext:FormPanel ID="FormPanelEvento" runat="server" Padding="10" MonitorValid="true" LabelAlign="Left"
                LabelWidth="100">
                <Items>
                    <ext:Panel runat="server" Layout="ColumnLayout" Width="440" Height="25" Border="false">
                        <Items>
                            <ext:Panel runat="server" Border="false" Height="25">
                                <Items>
                                    <ext:TextField ID="txtClaveEvento" runat="server" FieldLabel="Clave del Evento" Width="375"
                                        AllowBlank="false" MaxLength="10" />
                                </Items>
                            </ext:Panel>
                            <ext:Panel runat="server" Border="false" Width="10" Height="25" />
                            <ext:Panel runat="server" Border="false" Height="25">
                                <Items>
                                    <ext:Button ID="btnTip" runat="server" Icon="Information">
                                    </ext:Button>
                                </Items>
                            </ext:Panel>
                        </Items>
                    </ext:Panel>
                    <ext:TextArea ID="txtDescripcionEv" runat="server" FieldLabel="Descripción" Width="305"
                        AllowBlank="false" MaxLength="100" Height="50" />
                    <ext:TextArea ID="txtDescrEdoCta" runat="server" FieldLabel="Descripción Estado de Cuenta" Width="305"
                        MaxLength="200" Height="50" />
                </Items>
                <Buttons>
                    <ext:Button ID="btnGuardaEvento" runat="server" Text="Crear Evento" Icon="Add">
                        <DirectEvents>
                            <Click OnEvent="btnGuardaEvento_Click" Before="var valid= #{FormPanelEvento}.getForm().isValid(); if (!valid) {} return valid;" />
                        </DirectEvents>
                    </ext:Button>
                    <ext:Button runat="server" Text="Cancelar" Icon="Cancel">
                        <Listeners>
                            <Click Handler="#{FormPanelEvento}.getForm().reset(); #{WdwNuevoEvento}.hide();" />
                        </Listeners>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:Window ID="WdwEventos" runat="server" Title="Asociar Eventos" Width="480" Height="400" Hidden="true"
        Modal="true" Resizable="false" Closable="true">
        <Items>
            <ext:GridPanel ID="GridCatEventos" runat="server" Border="false" Header="false" AutoScroll="true"
                Layout="FitLayout" Height="335">
                <Store>
                    <ext:Store ID="StoreCatEventos" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Evento">
                                <Fields>
                                    <ext:RecordField Name="ID_Evento" />
                                    <ext:RecordField Name="ClaveEvento" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <ColumnModel runat="server">
                    <Columns>
                        <ext:Column DataIndex="ID_Evento" Hidden="true" />
                        <ext:Column DataIndex="ClaveEvento" Header="Clave" />
                        <ext:Column DataIndex="Descripcion" Header="Descripción" Width="320"/>
                    </Columns>
                </ColumnModel>
                <SelectionModel>
                    <ext:CheckboxSelectionModel runat="server" />
                </SelectionModel>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingCatEventos" runat="server" StoreID="StoreCatEventos" DisplayInfo="true"
                        DisplayMsg="Mostrando Eventos {0} - {1} de {2}" HideRefresh="true" />
                </BottomBar>
                <Listeners>
                    <Render Fn="hideMenuItems" />
                </Listeners>
                <Plugins>
                    <ext:GridFilters runat="server" Local="true" FiltersText="Filtros">
                        <Filters>
                            <ext:StringFilter DataIndex="ClaveEvento" />
                            <ext:StringFilter DataIndex="Descripcion" />
                        </Filters>
                    </ext:GridFilters>
                </Plugins>
            </ext:GridPanel>
        </Items>
        <Buttons>
            <ext:Button ID="btnAsociarEventos" runat="server" Text="Asociar" Icon="Add">
                <DirectEvents>
                    <Click OnEvent="btnAsociarEventos_Click">
                        <EventMask ShowMask="true" Msg="Asociando Eventos..." MinDelay="500" />
                    </Click>
                </DirectEvents>
                </ext:Button>
            <ext:Button runat="server" Text="Cancelar" Icon="Cancel">
                <Listeners>
                    <Click Handler="#{WdwEventos}.hide();" />
                </Listeners>
            </ext:Button>
        </Buttons>
    </ext:Window>
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center Split="true">
            <ext:Panel runat="server" Layout="FitLayout" Border="false">
                <Content>
                    <ext:BorderLayout runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridProductosFact" runat="server" Layout="FitLayout" StripeRows="true"
                                Header="false" Border="false">
                                <Store>
                                    <ext:Store ID="StoreProductosFact" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_ProductoFacturacion">
                                                <Fields>
                                                    <ext:RecordField Name="ID_ProductoFacturacion" />
                                                    <ext:RecordField Name="Descripcion" />
                                                    <ext:RecordField Name="ID_Unidad" />
                                                    <ext:RecordField Name="Unidad" />
                                                    <ext:RecordField Name="ID_TipoImpuesto" />
                                                    <ext:RecordField Name="TipoImpuesto" />
                                                    <ext:RecordField Name="ClaveProdServ" />
                                                    <ext:RecordField Name="ProductoServicio" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <TopBar>
                                    <ext:Toolbar ID="ToolbarConsulta" runat="server">
                                        <Items>
                                            <ext:Hidden ID="hdnIdProductoFacturacion" runat="server" />
                                            <ext:TextField ID="txtProductoFact" EmptyText="Ingresa Concepto..." Width="250" runat="server"
                                                MaxLengthText="200" />
                                            <ext:Button ID="btnBuscarProdFact" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnBuscarProdFact_Click" Before="if (#{txtProductoFact}.getValue() == '') { return false };">
                                                        <EventMask ShowMask="true" Msg="Buscando Conceptos de Facturación..." MinDelay="500" />
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Toolbar runat="server" Flat="true" Width="20" />
                                            <ext:ToolbarSeparator runat="server" />
                                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                                <DirectEvents>
                                                    <Click OnEvent="btnLimpiar_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:ToolbarFill runat="server" />
                                            <ext:ToolbarSeparator runat="server" />
                                            <ext:Button ID="btnNuevoProducto" runat="server" Text="Nuevo Concepto" Icon="Add">
                                                <Listeners>
                                                    <Click Handler="#{FormPanelNuevoPF}.reset(); #{GridCatalogos}.getStore().removeAll();
                                                        #{GridCatalogos}.setDisabled(true); #{PanelLateral}.setTitle('Nuevo Concepto');
                                                        #{PanelLateral}.expand();" />
                                                </Listeners>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <ColumnModel ID="ColumnModel8" runat="server">
                                    <Columns>
                                        <ext:CommandColumn Width="75">
                                            <Commands>
                                                <ext:GridCommand Icon="Lightning" CommandName="Eventos">
                                                    <ToolTip Text="Ver Eventos Asociados" />
                                                </ext:GridCommand>
                                                <ext:GridCommand Icon="Pencil" CommandName="Edit">
                                                    <ToolTip Text="Editar" />
                                                </ext:GridCommand>
                                                <ext:GridCommand Icon="Delete" CommandName="Delete">
                                                    <ToolTip Text="Eliminar" />
                                                </ext:GridCommand>
                                            </Commands>
                                        </ext:CommandColumn>
                                        <ext:Column DataIndex="ID_ProductoFacturacion" Hidden="true" />
                                        <ext:Column DataIndex="Descripcion" Header="Descripción" Width="320" />
                                        <ext:Column DataIndex="ID_Unidad" Hidden="true" />
                                        <ext:Column DataIndex="Unidad" Header="Unidad SAT" Width="150" />
                                        <ext:Column DataIndex="ID_TipoImpuesto" Hidden="true" />
                                        <ext:Column DataIndex="TipoImpuesto" Header="Tipo de Impuesto" Width="100" />
                                        <ext:Column DataIndex="ClaveProdServ" Hidden="true" />
                                        <ext:Column DataIndex="ProductoServicio" Header="Producto o Servicio SAT" Width="200" />
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel SingleSelect="true" />
                                </SelectionModel>
                                <DirectEvents>
                                    <Command OnEvent="EjecutarComando">
                                        <Confirmation BeforeConfirm="if ((command == 'Edit') || command == 'Eventos') return false;"
                                            ConfirmRequest="true" Title="Confirmación" Message="¿Estás seguro de eliminar el concepto de facturación?" />
                                        <%--<ExtraParams>
                                            <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridProductosFact}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                        </ExtraParams>--%>
                                        <ExtraParams>
                                            <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                            <ext:Parameter Name="ID_ProductoFacturacion" Value="Ext.encode(record.data.ID_ProductoFacturacion)" Mode="Raw" />
                                            <ext:Parameter Name="Descripcion" Value="Ext.encode(record.data.Descripcion)" Mode="Raw" />
                                            <ext:Parameter Name="ID_TipoImpuesto" Value="Ext.encode(record.data.ID_TipoImpuesto)" Mode="Raw" />
                                            <ext:Parameter Name="TipoImpuesto" Value="Ext.encode(record.data.TipoImpuesto)" Mode="Raw" />
                                            <ext:Parameter Name="ID_Unidad" Value="Ext.encode(record.data.ID_Unidad)" Mode="Raw" />
                                            <ext:Parameter Name="Unidad" Value="Ext.encode(record.data.Unidad)" Mode="Raw" />
                                            <ext:Parameter Name="ClaveProdServ" Value="Ext.encode(record.data.ClaveProdServ)" Mode="Raw" />
                                            <ext:Parameter Name="ProductoServicio" Value="Ext.encode(record.data.ProductoServicio)" Mode="Raw" />
                                        </ExtraParams>
                                    </Command>
                                    <%--<RowClick OnEvent="selectProdFact_Event">
                                        <EventMask ShowMask="true" Msg="Cargando Eventos Asociados..." MinDelay="500" />
                                        <ExtraParams>
                                            <ext:Parameter Name="ValuesPF" Value="Ext.encode(#{GridProductosFact}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                        </ExtraParams>
                                    </RowClick>--%>
                                </DirectEvents>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreProductosFact" DisplayInfo="true"
                                        DisplayMsg="Mostrando Conceptos {0} - {1} de {2}" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                        <South Split="true">
                            <ext:GridPanel ID="GridEventos" runat="server" Layout="FitLayout" Height="250" Disabled="true"
                                Title="Eventos Asociados" AutoScroll="true">
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar1" runat="server">
                                        <Items>
                                            <ext:Button runat="server" Text="Asociar Eventos" Icon="LightningAdd">
                                                <DirectEvents>
                                                    <Click OnEvent="AsociarEventos" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:ToolbarFill runat="server" />
                                            <ext:Button ID="btnNuevoEvento" runat="server" Text="Nuevo Evento" Icon="Add">
                                                <Listeners>
                                                    <Click Handler="#{WdwNuevoEvento}.show();" />
                                                </Listeners>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <Store>
                                    <ext:Store ID="StoreEventos" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Evento">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Evento" />
                                                    <ext:RecordField Name="ClaveEvento" />
                                                    <ext:RecordField Name="Descripcion" />
                                                    <ext:RecordField Name="TipoEvento" />
                                                    <ext:RecordField Name="DescripcionEdoCta" />
                                                    <ext:RecordField Name="EsActivo" />
                                                    <ext:RecordField Name="EsReversable" />
                                                    <ext:RecordField Name="EsCancelable" />
                                                    <ext:RecordField Name="EsTransaccional" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <SelectionModel>
                                    <ext:RowSelectionModel SingleSelect="true" />
                                </SelectionModel>
                                <ColumnModel ID="ColumnModel6" runat="server">
                                    <Columns>
                                        <ext:CommandColumn Width="30">
                                            <Commands>
                                                <ext:GridCommand Icon="Cross" CommandName="Delete">
                                                    <ToolTip Text="Desasociar Evento" />
                                                </ext:GridCommand>
                                            </Commands>
                                        </ext:CommandColumn>
                                        <ext:Column DataIndex="ID_Evento" Hidden="true" />
                                        <ext:Column DataIndex="ClaveEvento" Header="Clave Evento" />
                                        <ext:Column DataIndex="Descripcion" Header="Descripción" Width="200" />
                                        <ext:Column DataIndex="TipoEvento" Header="Tipo de Evento" Width="100" />
                                        <ext:Column DataIndex="DescripcionEdoCta" Header="Descripción Estado de Cuenta" Width="200" />
                                        <ext:Column DataIndex="EsActivo" Header="Activo"/>
                                        <ext:Column DataIndex="EsReversable" Header="Reversable"/>
                                        <ext:Column DataIndex="EsCancelable" Header="Cancelable"/>
                                        <ext:Column DataIndex="EsTransaccional" Header="Transaccional"/>
                                    </Columns>
                                </ColumnModel>
                                <DirectEvents>
                                    <Command OnEvent="DesasociarEvento">
                                        <Confirmation 
                                            ConfirmRequest="true" Title="Confirmación" Message="¿Estás seguro de desasociar el evento al concepto de facturación?" />
                                        <ExtraParams>
                                            <ext:Parameter Name="ID_Evento" Value="Ext.encode(record.data.ID_Evento)" Mode="Raw" />
                                        </ExtraParams>
                                    </Command>
                                </DirectEvents>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="StoreEventos" DisplayInfo="true"
                                        DisplayMsg="Eventos Asociados {0} - {1} de {2}" />
                                </BottomBar>
                            </ext:GridPanel>
                        </South>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </Center>
        <East Split="true">
            <ext:Panel ID="PanelLateral" runat="server" Width="450" Collapsed="true" Collapsible="true" 
                Layout="FitLayout" >
                <Content>
                    <ext:BorderLayout ID="BorderLayout3" runat="server">
                        <Center Split="true">
                            <ext:FormPanel ID="FormPanelNuevoPF" runat="server" Layout="FitLayout" Border="false" LabelWidth="130">
                                <Items>
                                    <ext:FieldSet runat="server" Title="Datos del Concepto" Padding="3">
                                        <Items>
                                            <ext:Hidden ID="hdnEsUnidad" runat="server" />
                                            <ext:Hidden ID="hdnCatUnidad" runat="server" />
                                            <ext:Hidden ID="hdnEsProdServ" runat="server" />
                                            <ext:Hidden ID="hdnCatProdServ" runat="server" />
                                            <ext:TextField ID="txtDescripcion" runat="server" FieldLabel="Descripción"
                                                MaxLength="50" Width="415" AllowBlank="false" />
                                            <ext:ComboBox ID="cBoxTipoImpuesto" runat="server" FieldLabel="Tipo de Impuesto" Width="415"
                                                ValueField="ID_TipoImpuesto" DisplayField="Descripcion" AllowBlank="false">
                                                <Store>
                                                    <ext:Store ID="StoreTipoImpuesto" runat="server">
                                                        <DirectEventConfig IsUpload="true" />
                                                        <Reader>
                                                            <ext:JsonReader IDProperty="ID_TipoImpuesto">
                                                                <Fields>
                                                                    <ext:RecordField Name="ID_TipoImpuesto" />
                                                                    <ext:RecordField Name="Clave" />
                                                                    <ext:RecordField Name="Descripcion" />
                                                                </Fields>
                                                            </ext:JsonReader>
                                                        </Reader>
                                                        <DirectEventConfig IsUpload="true" />
                                                    </ext:Store>
                                                </Store>
                                            </ext:ComboBox>
                                            <ext:Panel runat="server" Layout="ColumnLayout" Width="415" Height="25" Border="false"> 
                                                <Items>
                                                    <ext:Panel runat="server" Border="false" Height="25">
                                                        <Items>
                                                            <ext:TextField ID="txtUnidad" runat="server" FieldLabel="Unidad SAT"
                                                                Width="345" AllowBlank="false">
                                                                <Listeners>
                                                                    <Change Handler="#{hdnEsUnidad}.setValue(true);
                                                                        #{hdnEsProdServ}.setValue(false);
                                                                        #{hdnCatUnidad}.setValue('');" />
                                                                </Listeners>
                                                            </ext:TextField>
                                                        </Items>
                                                    </ext:Panel>
                                                    <ext:Panel runat="server" Border="false" Width="10" Height="25" />
                                                    <ext:Panel runat="server" Border="false" Height="25">
                                                        <Items>
                                                            <ext:Button ID="btnUnidad" runat="server" Text="Buscar" Icon="Magnifier"
                                                                ToolTip="Buscar en catálogo">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnUnidad_Click" Before="if (!#{txtUnidad}.getValue())
                                                                        { return false; } else { #{GridCatalogos}.getStore().removeAll();
                                                                        Ext.net.Mask.show({ msg : 'Buscando Unidades...' }); }" />
                                                                </DirectEvents>
                                                            </ext:Button>
                                                        </Items>
                                                    </ext:Panel>
                                                </Items>
                                            </ext:Panel>
                                            <ext:Panel runat="server" Layout="ColumnLayout" Width="415" Height="25" Border="false"> 
                                                <Items>
                                                    <ext:Panel runat="server" Border="false" Height="25">
                                                        <Items>
                                                            <ext:TextField ID="txtProdServ" runat="server" FieldLabel="Producto/Servicio SAT"
                                                                Width="345" AllowBlank="false">
                                                                <Listeners>
                                                                    <Change Handler="#{hdnEsUnidad}.setValue(false);
                                                                        #{hdnEsProdServ}.setValue(true);
                                                                        #{hdnCatProdServ}.setValue('');" />
                                                                </Listeners>
                                                            </ext:TextField>
                                                        </Items>
                                                    </ext:Panel>
                                                    <ext:Panel runat="server" Border="false" Width="10" Height="25" />
                                                    <ext:Panel runat="server" Border="false" Height="25">
                                                        <Items>
                                                            <ext:Button ID="btnProdServ" runat="server" Text="Buscar" Icon="Magnifier"
                                                                ToolTip="Buscar en catálogo">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnProdServ_Click" Before="if (!#{txtProdServ}.getValue())
                                                                        { return false; } else { #{GridCatalogos}.getStore().removeAll();
                                                                        Ext.net.Mask.show({ msg : 'Buscando Productos y Servicios...' }); }" />
                                                                </DirectEvents>
                                                            </ext:Button>
                                                        </Items>
                                                    </ext:Panel>
                                                </Items>
                                            </ext:Panel>
                                            <ext:Panel runat="server" Layout="RowLayout" Width="415" Height="15" Border="false" />
                                            <ext:Panel runat="server" Layout="ColumnLayout" Width="415" Height="25" Border="false">
                                                <Items>
                                                    <ext:Panel runat="server" Border="false" Height="25" Width="250" />
                                                    <ext:Panel runat="server" Border="false" Height="25">
                                                        <Items>
                                                            <ext:Button runat="server" Text="Cancelar" Icon="Cancel" Width="75">
                                                                <Listeners>
                                                                    <Click Handler="#{PanelLateral}.collapse(true);" />
                                                                </Listeners>
                                                            </ext:Button>
                                                        </Items>
                                                    </ext:Panel>
                                                    <ext:Panel runat="server" Border="false" Height="25" Width="15" />
                                                    <ext:Panel runat="server" Border="false" Height="25">
                                                        <Items>
                                                            <ext:Button ID="btnGuardar" runat="server" Text="Guardar" Width="75"
                                                                Icon="Disk">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnGuardar_Click"
                                                                        Before="if (!validaDatos(#{FormPanelNuevoPF}, #{txtUnidad}, #{txtProdServ}))
                                                                        { return false; } else if (!#{hdnCatUnidad}.getValue()) { 
                                                                        Ext.Msg.alert('Unidad SAT','Selecciona una Unidad SAT del catálogo');
                                                                        return false; } else if (!#{hdnCatProdServ}.getValue()) {
                                                                        Ext.Msg.alert('Producto/Servicio SAT','Selecciona un Producto o Servicio SAT del catálogo');
                                                                        return false; }"/>
                                                                </DirectEvents>
                                                            </ext:Button>
                                                        </Items>
                                                    </ext:Panel>
                                                </Items>
                                            </ext:Panel>
                                        </Items>
                                    </ext:FieldSet>
                                </Items>
                            </ext:FormPanel>
                        </Center>
                        <South Split="true">
                            <ext:GridPanel ID="GridCatalogos" runat="server" StripeRows="true" Header="false" Height="280" Title="-"
                                Border="false" Layout="FitLayout" Disabled="true">
                                <Store>
                                    <ext:Store ID="StoreCatalogo" runat="server">
                                        <DirectEventConfig IsUpload="true" />
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID">
                                                <Fields>
                                                    <ext:RecordField Name="ID" />
                                                    <ext:RecordField Name="Clave" />
                                                    <ext:RecordField Name="Descripcion" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                        <DirectEventConfig IsUpload="true" />
                                    </ext:Store>
                                </Store>
                                <ColumnModel runat="server">
                                    <Columns>
                                        <ext:Column Hidden="true" DataIndex="ID" />
                                        <ext:Column Header="Clave" DataIndex="Clave" Width="100" />
                                        <ext:Column Header="Descripción" DataIndex="Descripcion" Width="350" />
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel SingleSelect="true" />
                                </SelectionModel>
                                <DirectEvents>
                                    <RowClick OnEvent="SeleccionaItemCatalogo">
                                        <ExtraParams>
                                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridCatalogos}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                        </ExtraParams>
                                    </RowClick>
                                </DirectEvents>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingCatalogos" runat="server" StoreID="StoreCatalogo" DisplayInfo="true"
                                        DisplayMsg="{0} - {1} de {2}" HideRefresh="true" />
                                </BottomBar>
                            </ext:GridPanel>
                        </South>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </East>
    </ext:BorderLayout>
</asp:Content>
