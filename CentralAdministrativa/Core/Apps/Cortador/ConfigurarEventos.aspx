<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ConfigurarEventos.aspx.cs" Inherits="Cortador.ConfigurarEventos" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server" />

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">

        function showMenu(grid, menu, node, e) {
            if (node.browserEvent) {
                grid.menuNode = grid.getRootNode();
                grid.getSelectionModel().clearSelections();
                e = node;
            } else {
                grid.menuNode = node;
                node.select();
            }

            menu.showAt(e.getXY());
            e.stopEvent();
        }

        function showFormNuevaValidacion(grid, node, tipo) {
            grid.menuNode = node;
            grid.tipo = tipo;
            Ext.net.DirectMethods.showFormNuevaValidacion(
                grid.getRootNode().childNodes.length,
                grid.convertToSubmitNode(node),
                tipo
            );
        }

        function refreshTree(tree, result) {
            var nodes = eval(result);

            if (nodes != null && nodes.length > 0) {
                tree.initChildren(nodes);
            } else {
                tree.getRootNode().removeChildren();
            }
            
        }
    </script>

    
    <ext:Store runat="server" ID="stTipoElementoComparar">
        <Reader>
            <ext:JsonReader IDProperty="ID_TipoElemento">
                <Fields>
                    <ext:RecordField Name="ID_TipoElemento" />
                    <ext:RecordField Name="Clave" />
                    <ext:RecordField Name="Descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:Store runat="server" ID="stOrdenValidacion">
        <Reader>
            <ext:JsonReader IDProperty="Value">
                <Fields>
                    <ext:RecordField Name="Text" />
                    <ext:RecordField Name="Value" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:Menu ID="TreeContextMenu" runat="server" EnableScrolling="false">
        <Items>
            <ext:MenuItem Text="Nuevo" Icon="NewBlue">
                <Menu>
                    <ext:Menu ID="Menu3" runat="server">
                        <Items>
                            <ext:MenuItem ID="nueva_base" runat="server" Text="Nueva Validación Base" Icon="Add">
                                <Listeners>
                                    <Click Handler="showFormNuevaValidacion(#{panel_validacion}, #{panel_validacion}.getRootNode(), 1);" />
                                </Listeners>
                            </ext:MenuItem>

                            <ext:MenuSeparator />
                            <ext:MenuItem ID="nueva_true" runat="server" Text="Nueva Validación True" Icon="Accept">
                                <Listeners>
                                    <Click Handler="showFormNuevaValidacion(#{panel_validacion}, #{panel_validacion}.menuNode, 2);" />
                                </Listeners>
                            </ext:MenuItem>

                            <ext:MenuItem ID="nueva_false" runat="server" Text="Nueva Validación False" Icon="Decline">
                                <Listeners>
                                    <Click Handler="showFormNuevaValidacion(#{panel_validacion}, #{panel_validacion}.menuNode, 3);" />
                                </Listeners>
                            </ext:MenuItem>
                        </Items>
                    </ext:Menu>
                </Menu>
            </ext:MenuItem>

            <ext:MenuSeparator />

            <ext:MenuItem ID="MenuItem2" runat="server" Text="Inactivar" Icon="Decline">
                <DirectEvents>
                    <Click OnEvent="InactivarValidacion_Event"
                        Before="extraParams.idNodo=#{panel_validacion}.menuNode.id;">
                    </Click>
                </DirectEvents>
            </ext:MenuItem>

            <ext:MenuItem ID="MenuItem3" runat="server" Text="Activar" Icon="Decline">
                <DirectEvents>
                    <Click OnEvent="ActivarValidacion_Event"
                        Before="extraParams.idNodo=#{panel_validacion}.menuNode.id;">
                    </Click>
                </DirectEvents>
            </ext:MenuItem>

            <ext:MenuItem ID="MenuItem1" runat="server" Text="Eliminar" Icon="Delete">
                <DirectEvents>
                    <Click OnEvent="EliminarValidacion_Event"
                        Before="extraParams.idNodo=#{panel_validacion}.menuNode.id;">
                    </Click>
                </DirectEvents>
            </ext:MenuItem>

            <ext:MenuSeparator />
            <ext:MenuItem Hidden="true" ID="itemOrden" runat="server" Text="Cambiar Orden de Validación" Icon="SortAscending">
                <Menu>
                    <ext:Menu ID="Menu2" runat="server">
                        <Items>
                            <ext:MenuItem Text="Modificar Orden de Validación" Enabled="false" />
                            <ext:ComponentMenuItem ID="OpcionOrden" Text="Cambiar Orden de Validación" runat="server" Width="250" Shift="false">
                                <Component>
                                    <ext:ComboBox runat="server" ID="ComboBox1" FieldLabel="Orden de Validación" LabelAlign="Top" AllowBlank="false" Editable="false"
                                        StoreID="stOrdenValidacion" DisplayField="Text" ValueField="Value" Width="200" />
                                </Component>
                            </ext:ComponentMenuItem>
                        </Items>
                    </ext:Menu>
                </Menu>
            </ext:MenuItem>

            <ext:MenuSeparator />
            <ext:MenuItem Text="Ejecución" Icon="ScriptGo">
                <Menu>
                    <ext:Menu ID="Menu1" runat="server">
                        <Items>
                            <ext:MenuItem ID="itemPreReglas" runat="server" Text="Activar PreReglas" Icon="Accept">
                                <DirectEvents>
                                    <Click OnEvent="CambiarEjecucionPrereglasValidacion_Event"
                                        Before="extraParams.idNodo=#{panel_validacion}.menuNode.id;extraParams.activar=true;">
                                    </Click>
                                </DirectEvents>
                            </ext:MenuItem>

                            <ext:MenuItem ID="MenuItem4" runat="server" Text="Inactivar PreReglas" Icon="Decline">
                                <DirectEvents>
                                    <Click OnEvent="CambiarEjecucionPrereglasValidacion_Event"
                                        Before="extraParams.idNodo=#{panel_validacion}.menuNode.id;extraParams.activar=false;">
                                    </Click>
                                </DirectEvents>
                            </ext:MenuItem>

                            <ext:MenuItem ID="itemPostReglas" runat="server" Text="Activar PostReglas" Icon="Accept">
                                <DirectEvents>
                                    <Click OnEvent="CambiarEjecucionPostReglasValidacion_Event"
                                        Before="extraParams.idNodo=#{panel_validacion}.menuNode.id;extraParams.activar=true;">
                                    </Click>
                                </DirectEvents>
                            </ext:MenuItem>

                            <ext:MenuItem ID="MenuItem5" runat="server" Text="Inactivar PostReglas" Icon="Decline">
                                <DirectEvents>
                                    <Click OnEvent="CambiarEjecucionPostReglasValidacion_Event"
                                        Before="extraParams.idNodo=#{panel_validacion}.menuNode.id;extraParams.activar=false;">
                                    </Click>
                                </DirectEvents>
                            </ext:MenuItem>
                        </Items>
                    </ext:Menu>
                </Menu>
            </ext:MenuItem>
        </Items>
    </ext:Menu>

    
    
    <ext:Window runat="server" ID="dialog_validacion" Title="Nueva validación" Modal="true" Layout="Fit"
                Width="500" Height="370" Closable="false" Hidden="true" Resizable="false">
        <TopBar>
            <ext:Toolbar runat="server">
                <Items>
                    <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                    <ext:Button ID="btnNuevaValidacion" runat="server" Icon="Disk" Text="Guardar">
                        <DirectEvents>
                            <Click OnEvent="crearValidacion_Event" 
                                Before="extraParams.TipoValidacion=#{panel_validacion}.tipo;extraParams.idNodo=#{panel_validacion}.menuNode.id;">
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                    <ext:Button ID="btnCancel" runat="server" Icon="Cancel" Text="Cancelar">
                        <Listeners>
                            <Click Handler="function(){#{dialog_validacion}.hide(); #{nueva_validacion}.reset();}" />
                        </Listeners>
                    </ext:Button>
                </Items>
            </ext:Toolbar>
        </TopBar>
        <Items>
            <ext:FormPanel runat="server" ID="nueva_validacion" MonitorValid="true" Padding="5" >
                <Items>
                    <ext:TextField  runat="server" ID="f_validacion" DataIndex="Validacion" FieldLabel="Descripción" 
                                    MaxLength="150" AllowBlank="false"  BlankText="Valor Requerido" AnchorHorizontal="100%" />
                    <ext:TextField  runat="server" ID="f_campo" DataIndex="Campo" FieldLabel="Campo" 
                                    MaxLength="50" AllowBlank="false"  BlankText="Valor Requerido" AnchorHorizontal="100%" />
                    <ext:ComboBox   runat="server"  ID="f_tipo_elemento" FieldLabel="Tipo de Elemento a Comparar" 
                                    StoreID="stTipoElementoComparar" Editable="false"  AllowBlank="false" 
                                    BlankText="Valor Requerido"
                                    ValueField="Clave" DisplayField="Descripcion" LoadingText="Loading..." Mode="Local"
                                    TriggerAction="All" EmptyText="Selecciona una Opción" AnchorHorizontal="100%" />
                    <ext:TextArea runat="server" ID="f_formula" DataIndex="Formula" FieldLabel="Formula"  
                                  MaxLength="800" AnchorHorizontal="100%" />
                    <ext:NumberField runat="server" ID="f_error" DataIndex="CodigoError" FieldLabel="Código de error"
                                     MaxLength="4" AllowBlank="false" BlankText="Valor Requerido" AnchorHorizontal="100%" />
                    <ext:ComboBox runat="server" ID="f_declinar" FieldLabel="Declinar" AllowBlank="false" Editable="false">
                        <SelectedItem Value="false" />
                        <Items>
                            <ext:ListItem Text="TRUE" Value="true" />
                            <ext:ListItem Text="FALSE" Value="false" />
                        </Items>
                    </ext:ComboBox>
                    <ext:ComboBox   runat="server" ID="f_orden" FieldLabel="Orden de Validación" AllowBlank="false" Editable="false"
                                    StoreID="stOrdenValidacion" DisplayField="Text" ValueField="Value"/>
                    <ext:ComboBox runat="server" ID="f_prereglas" FieldLabel="PreReglas" AllowBlank="false" Editable="false">
                        <SelectedItem Value="false" />
                        <Items>
                            <ext:ListItem Text="TRUE" Value="true" />
                            <ext:ListItem Text="FALSE" Value="false" />
                        </Items>
                    </ext:ComboBox>
                    <ext:ComboBox runat="server" ID="f_postreglas" FieldLabel="PostReglas" AllowBlank="false" Editable="false">
                        <SelectedItem Value="false" />
                        <Items>
                            <ext:ListItem Text="TRUE" Value="true" />
                            <ext:ListItem Text="FALSE" Value="false" />
                        </Items>
                    </ext:ComboBox>
                </Items>

                <Listeners>
                    <ClientValidation Handler="#{btnNuevaValidacion}.setDisabled(!valid);" />
                </Listeners>
            </ext:FormPanel>
        </Items>
    </ext:Window>

    <ext:Viewport ID="ViewPort1" runat="server">
        <Items>
            <ext:BorderLayout ID="BorderLayout1" runat="server">
                <Center MarginsSummary="0 5 0 5">
                    <ext:GridPanel runat="server" ID="panel_eventos" Title="Eventos" AutoExpandColumn="Nombre" Width="600" >
                        <Store>
                            <ext:Store runat="server" ID="stEventos" OnRefreshData="stEventos_Refresh">
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_Evento">
                                        <Fields>
                                            <ext:RecordField Name="ID_Evento" />
                                            <ext:RecordField Name="Clave" />
                                            <ext:RecordField Name="Nombre" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <TopBar>
                            <ext:Toolbar runat="server" ID="toolbar_eventos">
                                <Items>
                                    <ext:ToolbarFill runat="server" ID="fill_" />
                                    <ext:ComboBox runat="server" ID="cbTipoBusqueda" Editable="false" FieldLabel="Buscar por" LabelAlign="Right" Width="180">
                                        <SelectedItem Value="0" />
                                        <Items>
                                            <ext:ListItem Text="Clave" Value="0" />
                                            <ext:ListItem Text="Nombre" Value="1" />
                                        </Items>
                                    </ext:ComboBox>
                                    <ext:TriggerField runat="server" ID="txtCriterioBusqueada" TriggerIcon="Search" >
                                        <DirectEvents>
                                            <TriggerClick OnEvent="llenarPanelEventos">
                                                <EventMask ShowMask="true" Msg="Buscando Eventos..." MinDelay="500" />
                                            </TriggerClick>
                                            <SpecialKey OnEvent="llenarPanelEventos" Before="return e.getKey() == Ext.EventObject.ENTER;"></SpecialKey>
                                        </DirectEvents>
                                    </ext:TriggerField>
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                        <ColumnModel>
                            <Columns>
                                <ext:Column ColumnID="ID_Evento" DataIndex="ID_Evento" Header="ID" Width="80" />
                                <ext:Column DataIndex="Clave" Header="Clave" Width="80" />
                                <ext:Column DataIndex="Nombre" Header="Nombre" Width="160" />
                            </Columns>
                        </ColumnModel>
                        <DirectEvents>
                            <RowClick OnEvent="evento_Event">
                                <ExtraParams>
                                    <ext:Parameter Name="Values" Value="Ext.encode(#{panel_eventos}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                </ExtraParams>
                            </RowClick>
                        </DirectEvents>
                        <SelectionModel>
                            <ext:RowSelectionModel runat="server" SingleSelect="true" />
                        </SelectionModel>
                        <BottomBar>
                            <ext:PagingToolbar runat="server" ID="paging_eventos" HideRefresh="true" PageSize="15" />
                        </BottomBar>
                    </ext:GridPanel>
                </Center>
                <South  MarginsSummary="5 5 5 5" Split="true" MinWidth="600" MinHeight="250">
                    <ext:Panel ID="panel_propiedades" runat="server" Title="Configuración"  Width="600" Height="250" Collapsible="true">
                        <Items>
                            <ext:Hidden runat="server" ID="ID_EventoSeleccionado" />
                            <ext:TabPanel ID="subpanel_propiedades" runat="server">
                                <Items>
                                    <ext:TreeGrid runat="server" ID="panel_validacion" Title="Validación" AutoExpandColumn="Validacion" Height="195" Width="600">
                                        <TopBar>
                                            <ext:Toolbar ID="Toolbar1" runat="server">
                                                <Items>
                                                    <ext:ToolbarFill runat="server" ID="ToolbarFill2" />
                                                    <ext:Button runat="server" ID="btn_primeraValidacion" Text="Nueva Validación" Icon="Add">
                                                        <Listeners>
                                                            <Click Handler="showFormNuevaValidacion(#{panel_validacion}, #{panel_validacion}.getRootNode(), 0);" />
                                                        </Listeners>
                                                    </ext:Button>
                                                </Items>
                                            </ext:Toolbar>
                                        </TopBar>
                                        <Columns>
                                            <ext:TreeGridColumn DataIndex="Validacion" Header="Validación" Width="80" />
                                            <ext:TreeGridColumn DataIndex="Formula" Header="Formula" Width="80" />
                                            <ext:TreeGridColumn DataIndex="TipoValidacion" Header="Tipo de Validación" Width="150" />
                                            <ext:TreeGridColumn DataIndex="Declinar" Header="Declinar" Width="80" />
                                            <ext:TreeGridColumn DataIndex="Estatus" Header="Estatus" Width="80" />
                                            <ext:TreeGridColumn DataIndex="OrdenValidacion" Header="Orden Validación" Width="80" />
                                            <ext:TreeGridColumn DataIndex="PreReglas" Header="PreReglas" Width="80" />
                                            <ext:TreeGridColumn DataIndex="PostReglas" Header="PostReglas" Width="80" />
                                            <ext:TreeGridColumn DataIndex="NodosTrue" Hidden="true" />
                                            <ext:TreeGridColumn DataIndex="NodosFalse" Hidden="true" />
                                            <ext:TreeGridColumn DataIndex="ID_Validacion" Hidden="true" />
                                            <ext:TreeGridColumn DataIndex="ID_Evento" Hidden="true" />
                                        </Columns>
                                        <Root>
                                            <ext:TreeNode NodeID="0" Text="Root" Icon="FolderGo" />
                                        </Root>
                                        <Listeners>
                                            <ContextMenu Handler="showMenu(#{panel_validacion}, #{TreeContextMenu}, node, e);" />
                                            <AfterRender Handler="this.body.on('dblclick', function() {showFormNuevaValidacion(#{panel_validacion}, #{panel_validacion}.getRootNode(), 0)})" />
                                        </Listeners>
                                    </ext:TreeGrid>
                                    
                                    <ext:Panel runat="server" ID="panel_reglas" Title="Reglas"></ext:Panel>
                                    <ext:Panel runat="server" ID="panel_script" Title="Script"></ext:Panel>
                                    <ext:Panel runat="server" ID="panel_plugins" Title="PlugIns"></ext:Panel>
                                    <ext:Panel runat="server" ID="panel_iso" Title="ISO Respuesta"></ext:Panel>
                                </Items>
                            </ext:TabPanel>
                        </Items>
                    </ext:Panel>
                </South>
            </ext:BorderLayout>
        </Items>
    </ext:Viewport>
</asp:Content>
