<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="NuevoProducto.aspx.cs" Inherits="Administracion.NuevoProducto" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" language="javascript">
        function buildNodeTip(tip) {
            var e = Ext.fly(tip.triggerElement).findParent('div.x-tree-node-el', null, true),
            node = e ? productos.getNodeById(e.getAttribute('tree-node-id', 'ext')) : null;
            if (node) {
                var data = String.format("<b>{0}</b>", node.text);
                if (tip.rendered) {
                    tip.update(data);
                } else {
                    tip.html = data;
                }
            } else {
                return false;
            }
        }
    </script>

    <script type="text/javascript">
        var checkNode = function (node) {
            MainContent_GridProductos.setVisible(false);
            MainContent_panelRangos.setVisible(false);
            MainContent_panelReglasMA.setVisible(false);
            MainContent_panelReglaPMA.setVisible(false);
            MainContent_panelTipoCuentaPMA.setVisible(false);
            MainContent_panelGpoCuentaPMA.setVisible(false);
            MainContent_panelGpoTarjetaPMA.setVisible(false);
            MainContent_panelTarjetaCtaPMA.setVisible(false);
            MainContent_panelValidaciones.setVisible(false);
            MainContent_GridVMA.setVisible(false);
            //MainContent_GridTipoCtaVMA.setVisible(false);
            //MainContent_FieldSetTiposCtaVMA.setVisible(false);
            //MainContent_GridGpoCuentaVMA.setVisible(false);
            //MainContent_GridGpoTarjetaVMA.setVisible(false);
            //MainContent_GridTarjetaCtaVMA.setVisible(false);
            //MainContent_panelTipoCtaRMA.setVisible(false);
            //MainContent_panelGpoCuentaRMA.setVisible(false);
            //MainContent_panelGpoTarjetaRMA.setVisible(false);
            //MainContent_panelTarjetaCtaRMA.setVisible(false);

            entityName = node.text;
            idGMA = node.id;

            if (node.text != 'Rangos') {
                Administracion.LlenarComboVigencias();
            }

            if (node.text == 'Propiedades') {
                Administracion.LlenarGridProductos(idGMA, 0);
            }
            else if (node.text == 'Rangos') {
                Administracion.LlenarGridRangos(idGMA, 0);
            }
            else if (node.text == 'Reglas') {
                Administracion.LlenarGridReglasMA(idGMA, 0);
            }
            else if (node.text == 'Regla') {
                //if (node.parentNode.text == 'Parámetros') {
                Administracion.LlenarFieldSetReglasPMA();
                //}
                //else {
                //    Administracion.LlenaGridVMA(MainContent_cmbCadenaComercial.value, node.text);
                //}
            }
            else if (node.text == 'Tipo de Cuenta') {
                //if (node.parentNode.text == 'Parámetros') {
                Administracion.LlenarFieldSetTiposCtaPMA();
                //}
                //else if (node.parentNode.text == 'Validaciones') {
                //    Administracion.LlenaGridVMA(MainContent_cmbCadenaComercial.value, node.text);
                //}
                //else {
                //    Administracion.LlenarGridTipoCtaRMA(idGMA);
                //}
            }
            else if (node.text == 'Grupo de Cuenta') {
                //if (node.parentNode.text == 'Parámetros') {
                Administracion.LlenarFieldSetGposCuentaPMA();
                //}
                //else if (node.parentNode.text == 'Validaciones') {
                //    Administracion.LlenaGridVMA(MainContent_cmbCadenaComercial.value, node.text);
                //}
                //else {
                //    Administracion.LlenarGridGpoCuentaRMA(idGMA);
                //}
            }
            //else if (node.text == 'Grupo de Tarjeta') {
            else if (node.text == 'Grupo') {
                //if (node.parentNode.text == 'Parámetros') {
                    Administracion.LlenarGridGpoTarjetaPMA(idGMA);
                //}
                //else if (node.parentNode.text == 'Validaciones') {
                //    Administracion.LlenaGridVMA(MainContent_cmbCadenaComercial.value, node.text);
                //}
                //else {
                //    Administracion.LlenarGridGpoTarjetaRMA(idGMA);
                //}
            }
            else if (node.text == 'Tarjeta/Cuenta') {
                //if (node.parentNode.text == 'Parámetros') {
                Administracion.LlenarFieldSetTarjetaCuentaPMA();
                //}
                //else if (node.parentNode.text == 'Validaciones') {
                //    Administracion.LlenaGridVMA(MainContent_cmbCadenaComercial.value, node.text);
                //}
                //else {
                //    Administracion.LlenarGridTarjetaCtaRMA(idGMA);
                //}
            }
            else if (node.text == 'Validaciones') {
                Administracion.LlenaGridVMA(MainContent_cmbCadenaComercial.value, node.text);
            }
        }

        var iniciaEdicionPMA = function (e) {
            if (e.getKey() === e.ENTER) {
                //var grid = entityName == 'Regla' ? GridReglaPMA : 
                //    entityName == 'Tipo de Cuenta' ? GridTipoCuentaPMA :
                //    entityName == 'Grupo de Cuenta' ? GridGpoCuentaPMA :
                //    entityName == 'Grupo de Tarjeta' ? GridGpoTarjetaPMA :
                //    GridTarjetaCtaPMA,
                var grid = entityName == GridGpoTarjetaPMA,
                record = grid.getSelectionModel().getSelected(),
                index = grid.store.indexOf(record);

                grid.startEditing(index, 1);
            }
        };

        var trasEdicionPMA = function (e) {
            /*
            Properties of 'e' include:
                e.grid - This grid
                e.record - The record being edited
                e.field - The field name being edited
                e.value - The value being set
                e.originalValue - The original value for the field, before the edit.
                e.row - The grid row index
                e.column - The grid column index
            */

            // Call DirectMethod
            Administracion.NuevoValorPMA(e.record.data.ID_ValorParametroMultiasignacion,
                e.record.data.ID_ParametroMultiasignacion,
                e.record.data.ID_Entidad,
                e.grid.id,
                e.record.data.ID_Origen,
                e.record.data.ID_Producto,
                e.record.data.ID_CadenaComercial,
                e.record.data.ID_Vigencia,
                e.record.data.Valor);
        };

        var iniciaEdicionRMA = function (e) {
            if (e.getKey() === e.ENTER) {
                var grid = GridReglasMA,
                //var grid = entityName == 'Reglas' ? GridReglaRMA :
                //    entityName == 'Tipo de Cuenta' ? GridTipoCtaRMA :
                //    entityName == 'Grupo de Cuenta' ? GridGpoCuentaRMA :
                //    entityName == 'Grupo de Tarjeta' ? GridGpoTarjetaRMA :
                //    GridTarjetaCtaRMA,
                record = grid.getSelectionModel().getSelected(),
                index = grid.store.indexOf(record);

                grid.startEditing(index, 1);
            }
            //record.select();
        };

        var trasEdicionRMA = function (e) {        
            Administracion.ValidaCamposRMA(
                e.record.data.Prioridad,
                e.record.data.ID_Vigencia,
                e.record.data.OrdenEjecucion);
        };

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

        function setGridName() {
            gridName = MainContent_GridVMA;
            //if (entityName == 'Regla') {
            //    gridName = MainContent_GridVMA;
            //}
            //else if (entityName == 'Tipo de Cuenta') {
            //    gridName = MainContent_GridTipoCtaVMA;
            //}
            //else if (entityName == 'Grupo de Cuenta') {
            //    gridName = MainContent_GridGpoCuentaVMA;
            //}
            //else if (entityName == 'Grupo de Tarjeta') {
            //    gridName = MainContent_GridGpoTarjetaVMA;
            //}
            //else if (entityName == 'Tarjeta/Cuenta') {
            //    gridName = MainContent_GridTarjetaCtaVMA;
            //}
        }

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%--                <ext:ToolTip ID="NodeTip" 
                runat="server" 
                Target="={productos.body}"
                Delegate="div.x-tree-node-el"
                TrackMouse="true">
                <Listeners>
                    <BeforeShow Fn="buildNodeTip" />
                </Listeners>
            </ext:ToolTip>--%>

    <%--<ext:Store ID="StoreProductos" runat="server" OnRefreshData="StoreProductos_Refresh">
        <DirectEventConfig>
            <EventMask ShowMask="false" />
        </DirectEventConfig>
        <Reader>
            <ext:JsonReader IDProperty="ID_GrupoMA">
                <Fields>
                    <ext:RecordField Name="ID_GrupoMA" />
                    <ext:RecordField Name="Descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
         <Reader>
            <ext:JsonReader IDProperty="id_grupoma">
                <Fields>
                    <ext:RecordField Name="ID_GrupoMA" Type="String" Mapping="id_grupoma" />
                    <ext:RecordField Name="Descripcion" Type="String" Mapping="descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
       <Listeners>
            <Load Handler="#{cmbProducto}.setValue(#{cmbProducto}.store.getAt(0).get('ID_GrupoMA'));" />
        </Listeners>
    </ext:Store>--%>
     <ext:Store ID="StoreProductos" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_GrupoMA">
                <Fields>
                    <ext:RecordField Name="ID_GrupoMA" />
                    <ext:RecordField Name="Clave" />
                    <ext:RecordField Name="Descripcion" />
                    <ext:RecordField Name="ID_Vigencia" />
                    <ext:RecordField Name="Vigencia" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:Store ID="StoreRangos" runat="server">
        <Reader>
            <ext:JsonReader>
                <Fields>
                    <ext:RecordField Name="ID_Rango" />
                    <ext:RecordField Name="ID_GrupoMA" />
                    <ext:RecordField Name="Clave" />
                    <ext:RecordField Name="Descripcion" />
                    <ext:RecordField Name="Inicio" />
                    <ext:RecordField Name="Fin" />
                    <ext:RecordField Name="esActivo" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:Store ID="StoreVigencias" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_Vigencia">
                <Fields>
                    <ext:RecordField Name="ID_Vigencia" />
                    <ext:RecordField Name="Clave" />
                    <ext:RecordField Name="Descripcion" />
                    <ext:RecordField Name="ID_TipoVigencia" />
                    <ext:RecordField Name="FechaIncial" />
                    <ext:RecordField Name="FechaFinal" />
                    <ext:RecordField Name="HoraInicial" />
                    <ext:RecordField Name="HoraFinal" />
                    <ext:RecordField Name="ID_Periodo" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

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

    <ext:Store ID="StoreTipoVigencia" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_TipoVigencia">
                <Fields>
                    <ext:RecordField Name="ID_TipoVigencia" />
                    <ext:RecordField Name="Clave" />
                    <ext:RecordField Name="Descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:Store ID="StorePeriodos" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_Periodo">
                <Fields>
                    <ext:RecordField Name="ID_Periodo" />
                    <ext:RecordField Name="Cve_Periodo" />
                    <ext:RecordField Name="Descripcion" />
                    <ext:RecordField Name="DiasComprendidos" />
                    <ext:RecordField Name="DiasMes" />
                    <ext:RecordField Name="MesesAnio" />
                    <ext:RecordField Name="DiaSemana" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:Store ID="StoreGMAOrdenValidacion" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="Value">
                <Fields>
                    <ext:RecordField Name="Text" />
                    <ext:RecordField Name="Value" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:Store ID="StoreGMATipoElementoComparar" runat="server">
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
    
    <ext:Store ID="StorePMA" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_ParametroMultiasignacion">
                <Fields>
                    <ext:RecordField Name="ID_ValorParametroMultiasignacion" />
                    <ext:RecordField Name="ID_ParametroMultiasignacion" />
                    <ext:RecordField Name="ID_Entidad" />
                    <ext:RecordField Name="ID_CadenaComercial" />
                    <ext:RecordField Name="ID_Origen" />
                    <ext:RecordField Name="ID_Producto" />
                    <ext:RecordField Name="ID_Vigencia" />
                    <ext:RecordField Name="Nombre" />
                    <ext:RecordField Name="Descripcion" />
                    <ext:RecordField Name="Valor" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:Store ID="StoreReglas" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="Nombre">
                <Fields>
                    <ext:RecordField Name="ID_Regla" />
                    <ext:RecordField Name="Nombre" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:Store ID="StoreTiposCta" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="Descripcion">
                <Fields>
                    <ext:RecordField Name="ID_TipoCuenta" />
                    <ext:RecordField Name="ClaveTipoCuenta" />
                    <ext:RecordField Name="Descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:Store ID="StoreGpoCuenta" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="Descripcion">
                <Fields>
                    <ext:RecordField Name="ID_GrupoCuenta" />
                    <ext:RecordField Name="ClaveGrupoCuenta" />
                    <ext:RecordField Name="Descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:Store ID="StoreTarjetaCta" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="Tarjeta">
                <Fields>
                    <ext:RecordField Name="ID_Cuenta" />
                    <ext:RecordField Name="ID_MA" />
                    <ext:RecordField Name="Tarjeta" />
                    <ext:RecordField Name="NombreTarjetahabiente" />
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
                            <ext:MenuItem ID="Rnueva_base" runat="server" Text="Nueva Validación Base" Icon="Add">
                                <Listeners>
                                    <Click Handler="showFormNuevaValidacion(#{GridVMA}, #{GridVMA}.getRootNode(), 1);" />
                                </Listeners>
                            </ext:MenuItem>
                            <ext:MenuSeparator />
                            <ext:MenuItem ID="Rnueva_true" runat="server" Text="Nueva Validación True" Icon="Accept">
                                <Listeners>
                                    <Click Handler="showFormNuevaValidacion(#{GridVMA}, #{GridVMA}.menuNode, 2);" />
                                </Listeners>
                            </ext:MenuItem>
                            <ext:MenuItem ID="Rnueva_false" runat="server" Text="Nueva Validación False" Icon="Decline">
                                <Listeners>
                                    <Click Handler="showFormNuevaValidacion(#{GridVMA}, #{GridVMA}.menuNode, 3);" />
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
                        Before="extraParams.idNodo=#{GridVMA}.menuNode.id;
                        extraParams.entidad = 'Regla';">
                    </Click>
                </DirectEvents>
            </ext:MenuItem>
            <ext:MenuItem ID="MenuItem3" runat="server" Text="Activar" Icon="Decline">
                <DirectEvents>
                    <Click OnEvent="ActivarValidacion_Event"
                        Before="extraParams.idNodo=#{GridVMA}.menuNode.id;
                        extraParams.entidad = 'Regla';">
                    </Click>
                </DirectEvents>
            </ext:MenuItem>
            <ext:MenuItem ID="MenuItem1" runat="server" Text="Eliminar" Icon="Delete">
                <DirectEvents>
                    <Click OnEvent="EliminarValidacion_Event"
                        Before="extraParams.idNodo=#{GridVMA}.menuNode.id;
                        extraParams.entidad = 'Regla';">
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
                                        StoreID="StoreGMAOrdenValidacion" DisplayField="Text" ValueField="Value" Width="200" />
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
                                        Before="extraParams.idNodo=#{GridVMA}.menuNode.id;
                                        extraParams.activar=true;
                                        extraParams.entidad = 'Regla';">
                                    </Click>
                                </DirectEvents>
                            </ext:MenuItem>
                            <ext:MenuItem ID="MenuItem4" runat="server" Text="Inactivar PreReglas" Icon="Decline">
                                <DirectEvents>
                                    <Click OnEvent="CambiarEjecucionPrereglasValidacion_Event"
                                        Before="extraParams.idNodo=#{GridVMA}.menuNode.id;
                                        extraParams.activar=false;
                                        extraParams.entidad = 'Regla';">
                                    </Click>
                                </DirectEvents>
                            </ext:MenuItem>
                            <ext:MenuItem ID="itemPostReglas" runat="server" Text="Activar PostReglas" Icon="Accept">
                                <DirectEvents>
                                    <Click OnEvent="CambiarEjecucionPostReglasValidacion_Event"
                                        Before="extraParams.idNodo=#{GridVMA}.menuNode.id;
                                        extraParams.activar=true;
                                        extraParams.entidad = 'Regla';">
                                    </Click>
                                </DirectEvents>
                            </ext:MenuItem>
                            <ext:MenuItem ID="MenuItem5" runat="server" Text="Inactivar PostReglas" Icon="Decline">
                                <DirectEvents>
                                    <Click OnEvent="CambiarEjecucionPostReglasValidacion_Event"
                                        Before="extraParams.idNodo=#{GridVMA}.menuNode.id;
                                        extraParams.activar=false;
                                        extraParams.entidad = 'Regla';">
                                    </Click>
                                </DirectEvents>
                            </ext:MenuItem>
                        </Items>
                    </ext:Menu>
                </Menu>
            </ext:MenuItem>
        </Items>
    </ext:Menu>
 <%--   <ext:Menu ID="TreeContextMenuTipoCta" runat="server" EnableScrolling="false">
        <Items>
            <ext:MenuItem Text="Nuevo" Icon="NewBlue">
                <Menu>
                    <ext:Menu ID="Menu5" runat="server">
                        <Items>
                            <ext:MenuItem ID="TiCnueva_base" runat="server" Text="Nueva Validación Base" Icon="Add">
                                <Listeners>
                                    <Click Handler="showFormNuevaValidacion(#{GridTipoCtaVMA}, #{GridTipoCtaVMA}.getRootNode(), 1);" />
                                </Listeners>
                            </ext:MenuItem>
                            <ext:MenuSeparator />
                            <ext:MenuItem ID="TiCnueva_true" runat="server" Text="Nueva Validación True" Icon="Accept">
                                <Listeners>
                                    <Click Handler="showFormNuevaValidacion(#{GridTipoCtaVMA}, #{GridTipoCtaVMA}.menuNode, 2);" />
                                </Listeners>
                            </ext:MenuItem>
                            <ext:MenuItem ID="TiCnueva_false" runat="server" Text="Nueva Validación False" Icon="Decline">
                                <Listeners>
                                    <Click Handler="showFormNuevaValidacion(#{GridTipoCtaVMA}, #{GridTipoCtaVMA}.menuNode, 3);" />
                                </Listeners>
                            </ext:MenuItem>
                        </Items>
                    </ext:Menu>
                </Menu>
            </ext:MenuItem>
            <ext:MenuSeparator />
            <ext:MenuItem ID="MenuItem9" runat="server" Text="Inactivar" Icon="Decline">
                <DirectEvents>
                    <Click OnEvent="InactivarValidacion_Event"
                        Before="extraParams.idNodo=#{GridTipoCtaVMA}.menuNode.id;
                        extraParams.entidad = 'Tipo de Cuenta';">
                    </Click>
                </DirectEvents>
            </ext:MenuItem>
            <ext:MenuItem ID="MenuItem10" runat="server" Text="Activar" Icon="Decline">
                <DirectEvents>
                    <Click OnEvent="ActivarValidacion_Event"
                        Before="extraParams.idNodo=#{GridTipoCtaVMA}.menuNode.id;
                        extraParams.entidad = 'Tipo de Cuenta';">
                    </Click>
                </DirectEvents>
            </ext:MenuItem>
            <ext:MenuItem ID="MenuItem11" runat="server" Text="Eliminar" Icon="Delete">
                <DirectEvents>
                    <Click OnEvent="EliminarValidacion_Event"
                        Before="extraParams.idNodo=#{GridTipoCtaVMA}.menuNode.id;
                        extraParams.entidad = 'Tipo de Cuenta';">
                    </Click>
                </DirectEvents>
            </ext:MenuItem>
            <ext:MenuSeparator />
            <ext:MenuItem Hidden="true" ID="MenuItem12" runat="server" Text="Cambiar Orden de Validación" Icon="SortAscending">
                <Menu>
                    <ext:Menu ID="Menu6" runat="server">
                        <Items>
                            <ext:MenuItem Text="Modificar Orden de Validación" Enabled="false" />
                            <ext:ComponentMenuItem ID="ComponentMenuItem1" Text="Cambiar Orden de Validación" runat="server" Width="250" Shift="false">
                                <Component>
                                    <ext:ComboBox runat="server" ID="ComboBox2" FieldLabel="Orden de Validación" LabelAlign="Top" AllowBlank="false" Editable="false"
                                        StoreID="StoreGMAOrdenValidacion" DisplayField="Text" ValueField="Value" Width="200" />
                                </Component>
                            </ext:ComponentMenuItem>
                        </Items>
                    </ext:Menu>
                </Menu>
            </ext:MenuItem>
            <ext:MenuSeparator />
            <ext:MenuItem Text="Ejecución" Icon="ScriptGo">
                <Menu>
                    <ext:Menu ID="Menu7" runat="server">
                        <Items>
                            <ext:MenuItem ID="MenuItem13" runat="server" Text="Activar PreReglas" Icon="Accept">
                                <DirectEvents>
                                    <Click OnEvent="CambiarEjecucionPrereglasValidacion_Event"
                                        Before="extraParams.idNodo=#{GridTipoCtaVMA}.menuNode.id;
                                        extraParams.activar=true;
                                        extraParams.entidad = 'Tipo de Cuenta';">
                                    </Click>
                                </DirectEvents>
                            </ext:MenuItem>
                            <ext:MenuItem ID="MenuItem14" runat="server" Text="Inactivar PreReglas" Icon="Decline">
                                <DirectEvents>
                                    <Click OnEvent="CambiarEjecucionPrereglasValidacion_Event"
                                        Before="extraParams.idNodo=#{GridTipoCtaVMA}.menuNode.id;
                                        extraParams.activar=false;
                                        extraParams.entidad = 'Tipo de Cuenta';">
                                    </Click>
                                </DirectEvents>
                            </ext:MenuItem>
                            <ext:MenuItem ID="MenuItem15" runat="server" Text="Activar PostReglas" Icon="Accept">
                                <DirectEvents>
                                    <Click OnEvent="CambiarEjecucionPostReglasValidacion_Event"
                                        Before="extraParams.idNodo=#{GridTipoCtaVMA}.menuNode.id;
                                        extraParams.activar=true;
                                        extraParams.entidad = 'Tipo de Cuenta';">
                                    </Click>
                                </DirectEvents>
                            </ext:MenuItem>
                            <ext:MenuItem ID="MenuItem16" runat="server" Text="Inactivar PostReglas" Icon="Decline">
                                <DirectEvents>
                                    <Click OnEvent="CambiarEjecucionPostReglasValidacion_Event"
                                        Before="extraParams.idNodo=#{GridTipoCtaVMA}.menuNode.id;
                                        extraParams.activar=false;
                                        extraParams.entidad = 'Tipo de Cuenta';">
                                    </Click>
                                </DirectEvents>
                            </ext:MenuItem>
                        </Items>
                    </ext:Menu>
                </Menu>
            </ext:MenuItem>
        </Items>
    </ext:Menu>
    <ext:Menu ID="TreeContextMenuGpoCuenta" runat="server" EnableScrolling="false">
        <Items>
            <ext:MenuItem Text="Nuevo" Icon="NewBlue">
                <Menu>
                    <ext:Menu ID="Menu9" runat="server">
                        <Items>
                            <ext:MenuItem ID="MenuItem17" runat="server" Text="Nueva Validación Base" Icon="Add">
                                <Listeners>
                                    <Click Handler="showFormNuevaValidacion(#{GridGpoCuentaVMA}, #{GridGpoCuentaVMA}.getRootNode(), 1);" />
                                </Listeners>
                            </ext:MenuItem>
                            <ext:MenuSeparator />
                            <ext:MenuItem ID="MenuItem18" runat="server" Text="Nueva Validación True" Icon="Accept">
                                <Listeners>
                                    <Click Handler="showFormNuevaValidacion(#{GridGpoCuentaVMA}, #{GridGpoCuentaVMA}.menuNode, 2);" />
                                </Listeners>
                            </ext:MenuItem>
                            <ext:MenuItem ID="MenuItem19" runat="server" Text="Nueva Validación False" Icon="Decline">
                                <Listeners>
                                    <Click Handler="showFormNuevaValidacion(#{GridGpoCuentaVMA}, #{GridGpoCuentaVMA}.menuNode, 3);" />
                                </Listeners>
                            </ext:MenuItem>
                        </Items>
                    </ext:Menu>
                </Menu>
            </ext:MenuItem>
            <ext:MenuSeparator />
            <ext:MenuItem ID="MenuItem20" runat="server" Text="Inactivar" Icon="Decline">
                <DirectEvents>
                    <Click OnEvent="InactivarValidacion_Event"
                        Before="extraParams.idNodo=#{GridGpoCuentaVMA}.menuNode.id;
                        extraParams.entidad = 'Grupo de Cuenta';">
                    </Click>
                </DirectEvents>
            </ext:MenuItem>
            <ext:MenuItem ID="MenuItem21" runat="server" Text="Activar" Icon="Decline">
                <DirectEvents>
                    <Click OnEvent="ActivarValidacion_Event"
                        Before="extraParams.idNodo=#{GridGpoCuentaVMA}.menuNode.id;
                        extraParams.entidad = 'Grupo de Cuenta';">
                    </Click>
                </DirectEvents>
            </ext:MenuItem>
            <ext:MenuItem ID="MenuItem22" runat="server" Text="Eliminar" Icon="Delete">
                <DirectEvents>
                    <Click OnEvent="EliminarValidacion_Event"
                        Before="extraParams.idNodo=#{GridGpoCuentaVMA}.menuNode.id;
                        extraParams.entidad = 'Grupo de Cuenta';">
                    </Click>
                </DirectEvents>
            </ext:MenuItem>
            <ext:MenuSeparator />
            <ext:MenuItem Hidden="true" ID="MenuItem23" runat="server" Text="Cambiar Orden de Validación" Icon="SortAscending">
                <Menu>
                    <ext:Menu ID="Menu10" runat="server">
                        <Items>
                            <ext:MenuItem Text="Modificar Orden de Validación" Enabled="false" />
                            <ext:ComponentMenuItem ID="ComponentMenuItem2" Text="Cambiar Orden de Validación" runat="server" Width="250" Shift="false">
                                <Component>
                                    <ext:ComboBox runat="server" ID="ComboBox3" FieldLabel="Orden de Validación" LabelAlign="Top" AllowBlank="false" Editable="false"
                                        StoreID="StoreGMAOrdenValidacion" DisplayField="Text" ValueField="Value" Width="200" />
                                </Component>
                            </ext:ComponentMenuItem>
                        </Items>
                    </ext:Menu>
                </Menu>
            </ext:MenuItem>
            <ext:MenuSeparator />
            <ext:MenuItem Text="Ejecución" Icon="ScriptGo">
                <Menu>
                    <ext:Menu ID="Menu11" runat="server">
                        <Items>
                            <ext:MenuItem ID="MenuItem24" runat="server" Text="Activar PreReglas" Icon="Accept">
                                <DirectEvents>
                                    <Click OnEvent="CambiarEjecucionPrereglasValidacion_Event"
                                        Before="extraParams.idNodo=#{GridGpoCuentaVMA}.menuNode.id;
                                        extraParams.activar=true;
                                        extraParams.entidad = 'Grupo de Cuenta';">
                                    </Click>
                                </DirectEvents>
                            </ext:MenuItem>
                            <ext:MenuItem ID="MenuItem25" runat="server" Text="Inactivar PreReglas" Icon="Decline">
                                <DirectEvents>
                                    <Click OnEvent="CambiarEjecucionPrereglasValidacion_Event"
                                        Before="extraParams.idNodo=#{GridGpoCuentaVMA}.menuNode.id;
                                        extraParams.activar=false;
                                        extraParams.entidad = 'Grupo de Cuenta';">
                                    </Click>
                                </DirectEvents>
                            </ext:MenuItem>
                            <ext:MenuItem ID="MenuItem26" runat="server" Text="Activar PostReglas" Icon="Accept">
                                <DirectEvents>
                                    <Click OnEvent="CambiarEjecucionPostReglasValidacion_Event"
                                        Before="extraParams.idNodo=#{GridGpoCuentaVMA}.menuNode.id;
                                        extraParams.activar=true;
                                        extraParams.entidad = 'Grupo de Cuenta';">
                                    </Click>
                                </DirectEvents>
                            </ext:MenuItem>
                            <ext:MenuItem ID="MenuItem27" runat="server" Text="Inactivar PostReglas" Icon="Decline">
                                <DirectEvents>
                                    <Click OnEvent="CambiarEjecucionPostReglasValidacion_Event"
                                        Before="extraParams.idNodo=#{GridGpoCuentaVMA}.menuNode.id;
                                        extraParams.activar=false;
                                        extraParams.entidad = 'Grupo de Cuenta';">
                                    </Click>
                                </DirectEvents>
                            </ext:MenuItem>
                        </Items>
                    </ext:Menu>
                </Menu>
            </ext:MenuItem>
        </Items>
    </ext:Menu>
    <ext:Menu ID="TreeContextMenuGpoTarjeta" runat="server" EnableScrolling="false">
        <Items>
            <ext:MenuItem Text="Nuevo" Icon="NewBlue">
                <Menu>
                    <ext:Menu ID="Menu13" runat="server">
                        <Items>
                            <ext:MenuItem ID="MenuItem28" runat="server" Text="Nueva Validación Base" Icon="Add">
                                <Listeners>
                                    <Click Handler="showFormNuevaValidacion(#{GridGpoTarjetaVMA}, #{GridGpoTarjetaVMA}.getRootNode(), 1);" />
                                </Listeners>
                            </ext:MenuItem>
                            <ext:MenuSeparator />
                            <ext:MenuItem ID="MenuItem29" runat="server" Text="Nueva Validación True" Icon="Accept">
                                <Listeners>
                                    <Click Handler="showFormNuevaValidacion(#{GridGpoTarjetaVMA}, #{GridGpoTarjetaVMA}.menuNode, 2);" />
                                </Listeners>
                            </ext:MenuItem>
                            <ext:MenuItem ID="MenuItem30" runat="server" Text="Nueva Validación False" Icon="Decline">
                                <Listeners>
                                    <Click Handler="showFormNuevaValidacion(#{GridGpoTarjetaVMA}, #{GridGpoTarjetaVMA}.menuNode, 3);" />
                                </Listeners>
                            </ext:MenuItem>
                        </Items>
                    </ext:Menu>
                </Menu>
            </ext:MenuItem>
            <ext:MenuSeparator />
            <ext:MenuItem ID="MenuItem31" runat="server" Text="Inactivar" Icon="Decline">
                <DirectEvents>
                    <Click OnEvent="InactivarValidacion_Event"
                        Before="extraParams.idNodo=#{GridGpoTarjetaVMA}.menuNode.id;
                        extraParams.entidad = 'Grupo de Tarjeta';">
                    </Click>
                </DirectEvents>
            </ext:MenuItem>
            <ext:MenuItem ID="MenuItem32" runat="server" Text="Activar" Icon="Decline">
                <DirectEvents>
                    <Click OnEvent="ActivarValidacion_Event"
                        Before="extraParams.idNodo=#{GridGpoTarjetaVMA}.menuNode.id;
                        extraParams.entidad = 'Grupo de Tarjeta';">
                    </Click>
                </DirectEvents>
            </ext:MenuItem>
            <ext:MenuItem ID="MenuItem33" runat="server" Text="Eliminar" Icon="Delete">
                <DirectEvents>
                    <Click OnEvent="EliminarValidacion_Event"
                        Before="extraParams.idNodo=#{GridGpoTarjetaVMA}.menuNode.id;
                        extraParams.entidad = 'Grupo de Tarjeta';">
                    </Click>
                </DirectEvents>
            </ext:MenuItem>
            <ext:MenuSeparator />
            <ext:MenuItem Hidden="true" ID="MenuItem34" runat="server" Text="Cambiar Orden de Validación" Icon="SortAscending">
                <Menu>
                    <ext:Menu ID="Menu14" runat="server">
                        <Items>
                            <ext:MenuItem Text="Modificar Orden de Validación" Enabled="false" />
                            <ext:ComponentMenuItem ID="ComponentMenuItem3" Text="Cambiar Orden de Validación" runat="server" Width="250" Shift="false">
                                <Component>
                                    <ext:ComboBox runat="server" ID="ComboBox4" FieldLabel="Orden de Validación" LabelAlign="Top" AllowBlank="false" Editable="false"
                                        StoreID="StoreGMAOrdenValidacion" DisplayField="Text" ValueField="Value" Width="200" />
                                </Component>
                            </ext:ComponentMenuItem>
                        </Items>
                    </ext:Menu>
                </Menu>
            </ext:MenuItem>
            <ext:MenuSeparator />
            <ext:MenuItem Text="Ejecución" Icon="ScriptGo">
                <Menu>
                    <ext:Menu ID="Menu15" runat="server">
                        <Items>
                            <ext:MenuItem ID="MenuItem35" runat="server" Text="Activar PreReglas" Icon="Accept">
                                <DirectEvents>
                                    <Click OnEvent="CambiarEjecucionPrereglasValidacion_Event"
                                        Before="extraParams.idNodo=#{GridGpoTarjetaVMA}.menuNode.id;
                                        extraParams.activar=true;
                                        extraParams.entidad = 'Grupo de Tarjeta';">
                                    </Click>
                                </DirectEvents>
                            </ext:MenuItem>
                            <ext:MenuItem ID="MenuItem36" runat="server" Text="Inactivar PreReglas" Icon="Decline">
                                <DirectEvents>
                                    <Click OnEvent="CambiarEjecucionPrereglasValidacion_Event"
                                        Before="extraParams.idNodo=#{GridGpoTarjetaVMA}.menuNode.id;
                                        extraParams.activar=false;
                                        extraParams.entidad = 'Grupo de Tarjeta';">
                                    </Click>
                                </DirectEvents>
                            </ext:MenuItem>
                            <ext:MenuItem ID="MenuItem37" runat="server" Text="Activar PostReglas" Icon="Accept">
                                <DirectEvents>
                                    <Click OnEvent="CambiarEjecucionPostReglasValidacion_Event"
                                        Before="extraParams.idNodo=#{GridGpoTarjetaVMA}.menuNode.id;
                                        extraParams.activar=true;
                                        extraParams.entidad = 'Grupo de Tarjeta';">
                                    </Click>
                                </DirectEvents>
                            </ext:MenuItem>
                            <ext:MenuItem ID="MenuItem38" runat="server" Text="Inactivar PostReglas" Icon="Decline">
                                <DirectEvents>
                                    <Click OnEvent="CambiarEjecucionPostReglasValidacion_Event"
                                        Before="extraParams.idNodo=#{GridGpoTarjetaVMA}.menuNode.id;
                                        extraParams.activar=false;
                                        extraParams.entidad = 'Grupo de Tarjeta';">
                                    </Click>
                                </DirectEvents>
                            </ext:MenuItem>
                        </Items>
                    </ext:Menu>
                </Menu>
            </ext:MenuItem>
        </Items>
    </ext:Menu>
    <ext:Menu ID="TreeContextMenuTarjetaCta" runat="server" EnableScrolling="false">
        <Items>
            <ext:MenuItem Text="Nuevo" Icon="NewBlue">
                <Menu>
                    <ext:Menu ID="Menu17" runat="server">
                        <Items>
                            <ext:MenuItem ID="MenuItem39" runat="server" Text="Nueva Validación Base" Icon="Add">
                                <Listeners>
                                    <Click Handler="showFormNuevaValidacion(#{GridTarjetaCtaVMA}, #{GridTarjetaCtaVMA}.getRootNode(), 1);" />
                                </Listeners>
                            </ext:MenuItem>
                            <ext:MenuSeparator />
                            <ext:MenuItem ID="MenuItem40" runat="server" Text="Nueva Validación True" Icon="Accept">
                                <Listeners>
                                    <Click Handler="showFormNuevaValidacion(#{GridTarjetaCtaVMA}, #{GridTarjetaCtaVMA}.menuNode, 2);" />
                                </Listeners>
                            </ext:MenuItem>
                            <ext:MenuItem ID="MenuItem41" runat="server" Text="Nueva Validación False" Icon="Decline">
                                <Listeners>
                                    <Click Handler="showFormNuevaValidacion(#{GridTarjetaCtaVMA}, #{GridTarjetaCtaVMA}.menuNode, 3);" />
                                </Listeners>
                            </ext:MenuItem>
                        </Items>
                    </ext:Menu>
                </Menu>
            </ext:MenuItem>
            <ext:MenuSeparator />
            <ext:MenuItem ID="MenuItem42" runat="server" Text="Inactivar" Icon="Decline">
                <DirectEvents>
                    <Click OnEvent="InactivarValidacion_Event"
                        Before="extraParams.idNodo=#{GridTarjetaCtaVMA}.menuNode.id;
                        extraParams.entidad = 'Tarjeta/Cuenta';">
                    </Click>
                </DirectEvents>
            </ext:MenuItem>
            <ext:MenuItem ID="MenuItem43" runat="server" Text="Activar" Icon="Decline">
                <DirectEvents>
                    <Click OnEvent="ActivarValidacion_Event"
                        Before="extraParams.idNodo=#{GridTarjetaCtaVMA}.menuNode.id;
                        extraParams.entidad = 'Tarjeta/Cuenta';">
                    </Click>
                </DirectEvents>
            </ext:MenuItem>
            <ext:MenuItem ID="MenuItem44" runat="server" Text="Eliminar" Icon="Delete">
                <DirectEvents>
                    <Click OnEvent="EliminarValidacion_Event"
                        Before="extraParams.idNodo=#{GridTarjetaCtaVMA}.menuNode.id;
                        extraParams.entidad = 'Tarjeta/Cuenta';">
                    </Click>
                </DirectEvents>
            </ext:MenuItem>
            <ext:MenuSeparator />
            <ext:MenuItem Hidden="true" ID="MenuItem45" runat="server" Text="Cambiar Orden de Validación" Icon="SortAscending">
                <Menu>
                    <ext:Menu ID="Menu18" runat="server">
                        <Items>
                            <ext:MenuItem Text="Modificar Orden de Validación" Enabled="false" />
                            <ext:ComponentMenuItem ID="ComponentMenuItem4" Text="Cambiar Orden de Validación" runat="server" Width="250" Shift="false">
                                <Component>
                                    <ext:ComboBox runat="server" ID="ComboBox5" FieldLabel="Orden de Validación" LabelAlign="Top" AllowBlank="false" Editable="false"
                                        StoreID="StoreGMAOrdenValidacion" DisplayField="Text" ValueField="Value" Width="200" />
                                </Component>
                            </ext:ComponentMenuItem>
                        </Items>
                    </ext:Menu>
                </Menu>
            </ext:MenuItem>
            <ext:MenuSeparator />
            <ext:MenuItem Text="Ejecución" Icon="ScriptGo">
                <Menu>
                    <ext:Menu ID="Menu19" runat="server">
                        <Items>
                            <ext:MenuItem ID="MenuItem46" runat="server" Text="Activar PreReglas" Icon="Accept">
                                <DirectEvents>
                                    <Click OnEvent="CambiarEjecucionPrereglasValidacion_Event"
                                        Before="extraParams.idNodo=#{GridTarjetaCtaVMA}.menuNode.id;
                                        extraParams.activar=true;
                                        extraParams.entidad = 'Tarjeta/Cuenta';">
                                    </Click>
                                </DirectEvents>
                            </ext:MenuItem>
                            <ext:MenuItem ID="MenuItem47" runat="server" Text="Inactivar PreReglas" Icon="Decline">
                                <DirectEvents>
                                    <Click OnEvent="CambiarEjecucionPrereglasValidacion_Event"
                                        Before="extraParams.idNodo=#{GridTarjetaCtaVMA}.menuNode.id;
                                        extraParams.activar=false;
                                        extraParams.entidad = 'Tarjeta/Cuenta';">
                                    </Click>
                                </DirectEvents>
                            </ext:MenuItem>
                            <ext:MenuItem ID="MenuItem48" runat="server" Text="Activar PostReglas" Icon="Accept">
                                <DirectEvents>
                                    <Click OnEvent="CambiarEjecucionPostReglasValidacion_Event"
                                        Before="extraParams.idNodo=#{GridTarjetaCtaVMA}.menuNode.id;
                                        extraParams.activar=true;
                                        extraParams.entidad = 'Tarjeta/Cuenta';">
                                    </Click>
                                </DirectEvents>
                            </ext:MenuItem>
                            <ext:MenuItem ID="MenuItem49" runat="server" Text="Inactivar PostReglas" Icon="Decline">
                                <DirectEvents>
                                    <Click OnEvent="CambiarEjecucionPostReglasValidacion_Event"
                                        Before="extraParams.idNodo=#{GridTarjetaCtaVMA}.menuNode.id;
                                        extraParams.activar=false;
                                        extraParams.entidad = 'Tarjeta/Cuenta';">
                                    </Click>
                                </DirectEvents>
                            </ext:MenuItem>
                        </Items>
                    </ext:Menu>
                </Menu>
            </ext:MenuItem>
        </Items>
    </ext:Menu>--%>

    <ext:Window runat="server" ID="DialogValidacion" Title="Nueva validación" Modal="true" Layout="FitLayout"
        Width="500" Closable="false" Hidden="true" Resizable="false">
        <TopBar>
            <ext:Toolbar runat="server">
                <Items>
                    <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                    <ext:Button ID="btnNuevaValidacion" runat="server" Icon="Disk" Text="Guardar">
                        <DirectEvents>
                            <Click OnEvent="crearValidacion_Event"
                                Before="setGridName();
                                extraParams.TipoValidacion=gridName.tipo;
                                extraParams.idNodo=gridName.menuNode.id;
                                extraParams.idProd = idGMA">
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                    <ext:Button ID="btnCancel" runat="server" Icon="Cancel" Text="Cancelar">
                        <Listeners>
                            <Click Handler="function(){#{DialogValidacion}.hide(); #{nueva_validacion}.reset();}" />
                        </Listeners>
                    </ext:Button>
                </Items>
            </ext:Toolbar>
        </TopBar>
        <Items>
            <ext:FormPanel runat="server" ID="nueva_validacion" MonitorValid="true" Padding="5">
                <Items>
                    <ext:TextField runat="server" ID="f_validacion" DataIndex="Validacion" FieldLabel="Descripción"
                        MaxLength="150" AllowBlank="false" BlankText="Valor Requerido" AnchorHorizontal="100%" />
                    <ext:TextField runat="server" ID="f_campo" DataIndex="Campo" FieldLabel="Campo"
                        MaxLength="50" AllowBlank="false" BlankText="Valor Requerido" AnchorHorizontal="100%" />
                    <ext:ComboBox runat="server" ID="f_tipo_elemento" FieldLabel="Tipo de Elemento a Comparar"
                        StoreID="StoreGMATipoElementoComparar" Editable="false" AllowBlank="false"
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
                    <ext:ComboBox runat="server" ID="f_orden" FieldLabel="Orden de Validación" AllowBlank="false" Editable="false"
                        StoreID="StoreGMAOrdenValidacion" DisplayField="Text" ValueField="Value" />
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
                    <ext:ComboBox runat="server" ID="f_vigencia" FieldLabel="Vigencia" AllowBlank="false" Editable="false"
                        StoreID="StoreVigencias" DisplayField="Descripcion" ValueField="ID_Vigencia" />
                    <ext:ComboBox runat="server" ID="f_prioridad" FieldLabel="Prioridad" AllowBlank="false" Editable="false">
                        <Items>
                            <ext:ListItem Text="1" Value="1" />
                            <ext:ListItem Text="2" Value="2" />
                            <ext:ListItem Text="3" Value="3" />
                            <ext:ListItem Text="4" Value="4" />
                            <ext:ListItem Text="5" Value="5" />
                        </Items>
                    </ext:ComboBox>
                </Items>
                <Listeners>
                    <ClientValidation Handler="#{btnNuevaValidacion}.setDisabled(!valid);" />
                </Listeners>
            </ext:FormPanel>
        </Items>
    </ext:Window>

    <ext:Window ID="DialogNuevoProducto" runat="server" Title="Nuevo Grupo MA"
        Hidden="true" Width="550" Height="170" Modal="true">
        <Items>
            <ext:BorderLayout runat="server">
                <Center>
                    <ext:FormPanel ID="FormNuevoProducto" runat="server" Padding="10" MonitorValid="true">
                        <Items>
                            <ext:TextField ID="TxtIDGrupoMA"
                                runat="server"
                                Hidden="true" />
                            <ext:TextField ID="TxtClaveGrupoMA"
                                runat="server"
                                FieldLabel="Clave"
                                AnchorHorizontal="100%"
                                MaxLength="10"
                                AllowBlank="false" />
                            <ext:TextField ID="TxtDescripcionGrupoMA"
                                runat="server"
                                FieldLabel="Descripción"
                                AnchorHorizontal="100%"
                                MaxLength="50"
                                AllowBlank="false" />
                            <ext:Panel runat="server" Height="25" BodyStyle="background-color: #FFF; border: 0px;">
                                <Items>
                                    <ext:BorderLayout runat="server">
                                        <Center>
                                            <ext:ComboBox ID="ComboVigenciaGrupoMA"
                                                runat="server"
                                                FieldLabel="Vigencia"
                                                AnchorHorizontal="100%"
                                                AllowBlank="false"
                                                EmptyText="Selecciona una Vigencia"
                                                StoreID="StoreVigencias"
                                                DisplayField="Descripcion"
                                                ValueField="ID_Vigencia" />
                                        </Center>
                                        <East>
                                            <ext:Button ID="btnAddVigencia" runat="server" Text="Vigencia" Icon="Add" Margins="0 0 0 10" Hidden="true">
                                                <DirectEvents>
                                                    <Click OnEvent="Click_NuevaVigencia" />
                                                </DirectEvents>
                                            </ext:Button>
                                        </East>
                                    </ext:BorderLayout>
                                </Items>
                            </ext:Panel>
                        </Items>
                        <Buttons>
                            <ext:Button ID="Button4" runat="server" Icon="Disk" Text="Guardar">
                                <DirectEvents>
                                    <Click OnEvent="Click_GuardarNuevoProducto" />
                                </DirectEvents>
                            </ext:Button>
                        </Buttons>
                        <Listeners>
                            <ClientValidation Handler="#{Button4}.setDisabled(!valid);" />
                        </Listeners>
                    </ext:FormPanel>
                </Center>
            </ext:BorderLayout>
        </Items>
    </ext:Window>

    <ext:Window ID="DialogNuevaVigencia" runat="server" Title="Nueva Vigencia"
        Hidden="true" Width="500" Height="310" Modal="true">
        <Items>
            <ext:BorderLayout ID="BorderLayout1" runat="server">
                <Center>
                    <ext:FormPanel ID="FrmNuevaVigencia" runat="server" Padding="10" MonitorValid="true">
                        <Items>
                            <ext:TextField ID="TxtIDVigencia"
                                runat="server"
                                Hidden="true" />
                            <ext:TextField ID="TxtVigenciaCalve"
                                runat="server"
                                FieldLabel="Clave"
                                AnchorHorizontal="100%"
                                MaxLength="10"
                                AllowBlank="false" />
                            <ext:TextField ID="TxtVigenciaDescripcion"
                                runat="server"
                                FieldLabel="Descripción"
                                AnchorHorizontal="100%"
                                MaxLength="150"
                                AllowBlank="false" />
                            <ext:ComboBox ID="ComboTipoVigencia"
                                runat="server"
                                FieldLabel="Tipo de Vigencia"
                                AnchorHorizontal="100%"
                                EmptyText="Selecciona un Tipo de Vigencia"
                                StoreID="StoreTipoVigencia"
                                DisplayField="Descripcion"
                                ValueField="ID_TipoVigencia"
                                AllowBlank="false">
                                <Listeners>
                                    <BeforeSelect Handler="#{DateFechaInicial}.setDisabled(true); #{DateFechaInicial}.reset();
                                        #{DateFechaFinal}.setDisabled(true); #{DateFechaFinal}.reset();
                                        #{TimeHoraInicial}.setDisabled(true); #{TimeHoraInicial}.reset(); 
                                        #{TimeHoraFinal}.setDisabled(true); #{TimeHoraFinal}.reset();
                                        #{ComboPeriodo}.setDisabled(true); #{ComboPeriodo}.reset();" />
                                    <Select Handler="if (this.getValue() == 1 || this.getValue() == 3 || this.getValue() == 4) { #{DateFechaInicial}.setDisabled(); #{DateFechaFinal}.setDisabled(); }
                                        else if (this.getValue() == 2 || this.getValue() == 5 || this.getValue() == 6) { #{TimeHoraInicial}.setDisabled(); #{TimeHoraFinal}.setDisabled(); }
                                        else { #{ComboPeriodo}.setDisabled(); }" />
                                </Listeners>
                            </ext:ComboBox>
                            <ext:DateField ID="DateFechaInicial"
                                runat="server"
                                FieldLabel="Fecha Inicial"
                                AnchorHorizontal="100%"
                                AllowBlank="false"
                                Disabled="true" />
                            <ext:DateField ID="DateFechaFinal"
                                runat="server"
                                FieldLabel="Fecha Teminación"
                                AnchorHorizontal="100%"
                                AllowBlank="false"
                                Disabled="true" />
                            <ext:TimeField ID="TimeHoraInicial"
                                runat="server"
                                FieldLabel="Hora Inicial"
                                AnchorHorizontal="100%"
                                AllowBlank="false"
                                Disabled="true" />
                            <ext:TimeField ID="TimeHoraFinal"
                                runat="server"
                                FieldLabel="Hora de Terminación"
                                AnchorHorizontal="100%"
                                AllowBlank="false"
                                Disabled="true" />
                            <ext:ComboBox ID="ComboPeriodo"
                                runat="server"
                                FieldLabel="Periodo"
                                AnchorHorizontal="100%"
                                EmptyText="Selecciona un Periodo"
                                StoreID="StorePeriodos"
                                DisplayField="Descripcion"
                                ValueField="ID_Periodo"
                                AllowBlank="false"
                                Disabled="true" />
                        </Items>
                        <Buttons>
                            <ext:Button ID="btnNuevaVigencia" runat="server" Icon="Disk" Text="Guardar">
                                <DirectEvents>
                                    <Click OnEvent="Click_GuardarNuevaVigencia" />
                                </DirectEvents>
                            </ext:Button>
                        </Buttons>
                        <Listeners>
                            <ClientValidation Handler="#{btnNuevaVigencia}.setDisabled(!valid);" />
                        </Listeners>
                    </ext:FormPanel>
                </Center>
            </ext:BorderLayout>
        </Items>
    </ext:Window>

    <ext:Window ID="WNuevoRango" runat="server" Title="Nuevo Rango" Hidden="true" Width="600" Height="350"
        Modal="true" Resizable="false">
        <Items>
            <ext:FormPanel ID="FNuevoRango" runat="server" Padding="10" MonitorValid="true" LabelAlign="Right">
                <Items>
                    <ext:TextField ID="TxTRangoID_GrupoMA" runat="server" Hidden="true" />
                    <ext:TextField ID="TxTRangoClave" runat="server" FieldLabel="Clave" MaxLength="20"
                        AnchorHorizontal="100%" AllowBlank="false" />
                    <ext:TextField ID="TxTRangoDescripcion" runat="server" FieldLabel="Descripción" MaxLength="50"
                        AnchorHorizontal="100%" AllowBlank="false" />
                    <ext:NumberField ID="NumRangoBin" runat="server" FieldLabel="BIN" MinLength="6" MaxLength="6"
                        AnchorHorizontal="100%" AllowBlank="false" AllowDecimals="false" AllowNegative="false" />
                    <ext:NumberField ID="NumRangoInicio" runat="server" FieldLabel="Inicio" MinLength="9" MaxLength="9"
                        AnchorHorizontal="100%" AllowBlank="false" AllowDecimals="false" AllowNegative="false" />
                    <ext:NumberField ID="NumRangoFin" runat="server" FieldLabel="Fin" MinLength="9" MaxLength="9"
                        AnchorHorizontal="100%" AllowBlank="false" AllowDecimals="false" AllowNegative="false" />
                    <ext:ComboBox ID="cmbServiceCode1" runat="server" FieldLabel="Service Code - D1"
                        AnchorHorizontal="100%" AllowBlank="false">
                        <Items>
                            <ext:ListItem Text="1" Value="1"/>
                            <ext:ListItem Text="2" Value="2"/>
                            <ext:ListItem Text="5" Value="5" />
                            <ext:ListItem Text="6" Value="6" />
                            <ext:ListItem Text="7" Value="7" />
                            <ext:ListItem Text="9" Value="9" />
                        </Items>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cmbServiceCode2" runat="server" FieldLabel="Service Code - D2"
                        AnchorHorizontal="100%" AllowBlank="false">
                        <Items>
                            <ext:ListItem Text="0" Value="1"/>
                            <ext:ListItem Text="2" Value="2"/>
                            <ext:ListItem Text="4" Value="4" />
                        </Items>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cmbServiceCode3" runat="server" FieldLabel="Service Code - D3"
                        AnchorHorizontal="100%" AllowBlank="false">
                        <Items>
                            <ext:ListItem Text="1" Value="1"/>
                            <ext:ListItem Text="3" Value="3"/>
                            <ext:ListItem Text="4" Value="4" />
                            <ext:ListItem Text="5" Value="5" />
                            <ext:ListItem Text="6" Value="6" />
                            <ext:ListItem Text="7" Value="7" />
                        </Items>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cmbAniosVigencia" runat="server" FieldLabel="Años de Vigencia"
                        AnchorHorizontal="100%" AllowBlank="false">
                        <Items>
                            <ext:ListItem Text="1" Value="1"/>
                            <ext:ListItem Text="2" Value="2"/>
                            <ext:ListItem Text="3" Value="3" />
                            <ext:ListItem Text="4" Value="4" />
                            <ext:ListItem Text="5" Value="5" />
                            <ext:ListItem Text="6" Value="6" />
                            <ext:ListItem Text="7" Value="7" />
                            <ext:ListItem Text="9" Value="9" />
                            <ext:ListItem Text="10" Value="10" />
                        </Items>
                    </ext:ComboBox>
                    <ext:SelectBox ID="SelRangoEsActivo" runat="server" FieldLabel="Es Activo"
                        AnchorHorizontal="100%" AllowBlank="false">
                        <Items>
                            <ext:ListItem Text="Sí" Value="1"/>
                            <ext:ListItem Text="No" Value="0"/>
                        </Items>
                    </ext:SelectBox>
                </Items>
                <Buttons>
                    <ext:Button ID="BtnNuevoRango" runat="server" Text="Guardar" Icon="Disk">
                        <%--<DirectEvents>
                            <Click OnEvent="GuardarNuevoRango_Click" />
                        </DirectEvents>--%>
                    </ext:Button>
                </Buttons>
                <Listeners>
                    <ClientValidation Handler="#{BtnNuevoRango}.setDisabled(!valid);" />
                </Listeners>
            </ext:FormPanel>
        </Items>
    </ext:Window>

    <ext:BorderLayout runat="server">
        <North>
            <ext:Panel ID="PanelHeader" runat="server">
                <TopBar>
                    <ext:Toolbar ID="tbCadenaComercial" runat="server">
                        <Items>
                            <ext:ComboBox ID="cmbCadenaComercial"
                                runat="server"
                                FieldLabel="Cadena Comercial"
                                LabelAlign="Right"
                                AnchorHorizontal="80%"
                                EmptyText="Selecciona una Cadena Comercial"
                                StoreID="StoreCadenaComercial"
                                DisplayField="NombreORazonSocial"
                                ValueField="ID_Colectiva"
                                Width="300"
                                AllowBlank="false">
                                <Listeners>
                                    <Select Handler="if(this.isValid()) {
                                        #{pArbolMenu}.setTitle('Productos de ' + this.getRawValue());
                                        #{pArbolMenu}.expand();
                                        #{pArbolMenu}.setVisible(true); }" />
                                </Listeners>
                            </ext:ComboBox>
                            <%--<ext:ComboBox ID="cmbProducto" 
                                runat="server" 
                                StoreID="StoreProductos" 
                                TypeAhead="true" 
                                Mode="Local"
                                ForceSelection="true" 
                                TriggerAction="All" 
                                DisplayField="Descripcion" 
                                ValueField="ID_GrupoMA"
                                EmptyText="Búsqueda por Producto..." 
                                ValueNotFoundText="Búsqueda por Producto..." >
                                <Listeners>
                                    <Select Handler="#{StoreProductos}.reload();" />
                                </Listeners>
                            </ext:ComboBox>
                            <ext:Button ID="btnBuscarProducto"
                                runat="server"
                                Text="Buscar"
                                Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" 
                                        Before="var valid= #{cmbCadenaComercial}.isValid(); if (!valid) {}
                                        #{pArbolMenu}.setTitle('Productos de ' + #{cmbCadenaComercial}.getRawValue());
                                        return valid;">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>--%>
                            <ext:ToolbarFill ID="dummy" runat="server" />
                            <ext:Button ID="btnNuevoGMA" runat="server" Text="Nuevo Producto" Icon="Add">
                                <DirectEvents>
                                    <Click OnEvent="Click_NuevoProducto" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
            </ext:Panel>
        </North>
        <West>
            <ext:Panel ID="pArbolMenu" runat="server" Layout="accordion" Hidden="true" Collapsed="true"
                Width="300" MinWidth="300" MaxWidth="350" Split="true" Collapsible="true">
            </ext:Panel>
        </West>
        <Center Split="true">
            <ext:Panel ID="PanelCentral" runat="server" Layout="FitLayout">
                <Items>
                    <ext:GridPanel ID="GridProductos" runat="server" StoreID="StoreProductos" Hidden="true">
                    </ext:GridPanel>
                    <ext:FormPanel ID="panelRangos" runat="server" Title="Configuraciones de Rangos" Hidden="true" Layout="FitLayout">
                        <Items>
                            <ext:GridPanel ID="GridRangos" runat="server" StoreID="StoreRangos">
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar1" runat="server">
                                        <Items>
                                            <ext:ToolbarFill runat="server" ID="ToolbarFill2" />
                                            <ext:Button ID="BtnShowNuevoRango" runat="server" Text="Nuevo Rango" Icon="Add">
                                                <DirectEvents>
                                                    <Click OnEvent="ShowNuevoRango_Event" />
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                            </ext:GridPanel>
                        </Items>
                    </ext:FormPanel>
                    <ext:Panel ID="panelReglasMA" runat="server" Title="Configuraciones de Reglas" Hidden="true" Layout="FitLayout">
                        <Items> 
                            <ext:GridPanel ID="GridReglasMA" runat="server" Layout="FitLayout" StripeRows="true"
                                Header="false" Border="false">
                                <Store>
                                    <ext:Store ID="StoreReglasMA" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Regla">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Regla" />
                                                    <ext:RecordField Name="ID_ReglaMultiasignacion" />
                                                    <ext:RecordField Name="ID_Entidad" />
                                                    <ext:RecordField Name="Nombre" />
                                                    <ext:RecordField Name="ID_Vigencia" />
                                                    <ext:RecordField Name="Vigencia" />
                                                    <ext:RecordField Name="Prioridad" />
                                                    <ext:RecordField Name="OrdenEjecucion" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                               <%-- <Listeners>
                                    <KeyDown Fn="iniciaEdicionRMA" />
                                    <AfterEdit Fn="trasEdicionRMA" />
                                </Listeners>--%>
                                <ColumnModel ID="ColumnModel6" runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_Regla" Hidden="true" />
                                        <ext:Column DataIndex="ID_ReglaMultiasignacion" Hidden="true" />
                                        <ext:Column DataIndex="ID_Entidad" Hidden="true" />
                                        <ext:Column DataIndex="Nombre" Header="Nombre" Width="280" />
                                         <ext:Column DataIndex="ID_Vigencia" Header="Vigencia" Width="120">
                                            <Editor>
                                                <ext:ComboBox ID="cmbVigenciaReglasMA"
                                                    runat="server"
                                                    StoreID="StoreVigencias"
                                                    DisplayField="Descripcion"
                                                    ValueField="ID_Vigencia" />
                                            </Editor>
                                        </ext:Column>
                                        <ext:Column DataIndex="Prioridad" Header="Prioridad" Width="60" Align="Center">
                                            <Editor>
                                                <ext:ComboBox runat="server" Editable="false">
                                                    <Items>
                                                        <ext:ListItem Text="1" Value="1" />
                                                        <ext:ListItem Text="2" Value="2" />
                                                        <ext:ListItem Text="3" Value="3" />
                                                        <ext:ListItem Text="4" Value="4" />
                                                        <ext:ListItem Text="5" Value="5" />
                                                    </Items>
                                                </ext:ComboBox>
                                            </Editor>
                                        </ext:Column>
                                        <ext:Column DataIndex="OrdenEjecucion" Header="Orden de Ejecución" Width="120" Align="Center">
                                            <Editor>
                                                <ext:ComboBox runat="server" Editable="false">
                                                    <Items>
                                                        <ext:ListItem Text="1" Value="1" />
                                                        <ext:ListItem Text="2" Value="2" />
                                                        <ext:ListItem Text="3" Value="3" />
                                                        <ext:ListItem Text="4" Value="4" />
                                                        <ext:ListItem Text="5" Value="5" />
                                                    </Items>
                                                </ext:ComboBox>
                                            </Editor>
                                        </ext:Column>
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:CheckboxSelectionModel ID="chkSM_Reglas" runat="server" />
                                </SelectionModel>
                                <Buttons>
                                    <ext:Button ID="btnActivaReglas" runat="server" Text="Activar Reglas">
                                        <DirectEvents>
                                            <Click OnEvent="btnActivaReglas_Click">
                                                <ExtraParams>
                                                    <ext:Parameter Name="Values" Value="Ext.encode(#{GridReglasMA}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                                    <ext:Parameter Name="IdProducto" Value="idGMA" Mode="Raw" />
                                                </ExtraParams>
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                </Buttons>
                            </ext:GridPanel>
                        </Items>
                    </ext:Panel>
                    <ext:FormPanel ID="panelReglaPMA" runat="server" Title="Configuraciones de Parámetros" Hidden="true">
                        <Items>
                            <ext:GridPanel ID="GridPanelReglas" runat="server" StoreID="StoreReglas" Height="250" Layout="FitLayout" StripeRows="true"
                                    Header="false" Border="false">
                                <TopBar>
                                    <ext:Toolbar runat="server">
                                        <Items>
                                            <ext:TextField ID="txtReglasPMA" EmptyText="Nombre Regla" Width="200" runat="server" />
                                            <ext:Button ID="btnReglasPMA" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnReglasPMA_Click" Before="var valid= #{panelReglaPMA}.getForm().isValid(); if (!valid) {} return valid;">
                                                        <ExtraParams>
                                                            <ext:Parameter Name="IdProducto" Value="idGMA" Mode="Raw" />
                                                        </ExtraParams>
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <ColumnModel ID="cmReglaPMA" runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_Regla" Hidden="true" />
                                        <ext:Column DataIndex="Nombre" Header="Regla" Width="400" />
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel SingleSelect="true" />
                                </SelectionModel>
                                <DirectEvents>
                                    <RowClick OnEvent="selectRegla_Event">
                                        <ExtraParams>
                                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridPanelReglas}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                            <ext:Parameter Name="IdProducto" Value="idGMA" Mode="Raw" />
                                        </ExtraParams>
                                    </RowClick>
                                </DirectEvents>
                            </ext:GridPanel>
                            <ext:GridPanel ID="GridReglaPMA" runat="server" StoreID="StorePMA" Title = "Parámetros de la Regla" 
                                    Height="500" Layout="FitLayout" StripeRows="true">
                                <Listeners>
                                    <KeyDown Fn="iniciaEdicionPMA" />
                                    <AfterEdit Fn="trasEdicionPMA" />
                                </Listeners>
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_ValorParametroMultiasignacion" Hidden="true" />
                                        <ext:Column DataIndex="ID_ParametroMultiasignacion" Hidden="true" />
                                        <ext:Column DataIndex="ID_Entidad" Hidden="true" />
                                        <ext:Column DataIndex="ID_CadenaComercial" Hidden="true" />
                                        <ext:Column DataIndex="ID_Origen" Hidden="true" />
                                        <ext:Column DataIndex="ID_Producto" Hidden="true" />
                                        <ext:Column DataIndex="Nombre" Header="Nombre" />
                                        <ext:Column DataIndex="Descripcion" Header="Descripcion" />
                                        <ext:Column DataIndex="Valor" Header="Valor" Width="300">
                                            <Editor>
                                                <ext:TextField ID="txtValorRPMA" runat="server" />
                                            </Editor>
                                        </ext:Column>
                                        <ext:Column DataIndex="ID_Vigencia" Header="Vigencia">
                                            <Editor>
                                                <ext:ComboBox ID="cmbVigenciaPMA"
                                                    runat="server"
                                                    StoreID="StoreVigencias"
                                                    DisplayField="Descripcion"
                                                    ValueField="ID_Vigencia" />
                                            </Editor>
                                        </ext:Column>
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true" />
                                </SelectionModel>
                            </ext:GridPanel>
                        </Items>
                    </ext:FormPanel>
                    <ext:FormPanel ID="panelTipoCuentaPMA" runat="server" Title="Configuraciones de Parámetros" Hidden="true">
                        <Items>
                            <%--<ext:FieldSet ID="FieldSetBuscarTipoCtaPMA" runat="server" Title="Ingrese un criterio de búsqueda">
                                <Items>
                                    <ext:TextField
                                        ID="txtTiposCtaPMA"
                                        runat="server"
                                        FieldLabel="Descripción"
                                        MaxLength="20"
                                        Width="350" />
                                    <ext:ComboBox
                                        FieldLabel="Tipos de Cuenta"
                                        ID="CBoxTiposCtaPMA"
                                        EmptyText="Selecciona una Opción..."
                                        runat="server"
                                        StoreID="StoreTiposCta"
                                        MsgTarget="Side"
                                        DisplayField="Descripcion"
                                        ValueField="ID_TipoCuenta"
                                        Editable="false"
                                        Width="350" />
                                </Items>
                                <Buttons>
                                    <ext:Button
                                        ID="btnTiposCtaPMA"
                                        runat="server"
                                        Text="Buscar"
                                        Icon="Magnifier">
                                        <DirectEvents>
                                            <Click OnEvent="btnTiposCtaPMA_Click" Before="var valid= #{panelTipoCuentaPMA}.getForm().isValid(); if (!valid) {} return valid;">
                                                <ExtraParams>
                                                    <ext:Parameter Name="IdProducto" Value="idGMA" Mode="Raw" />
                                                </ExtraParams>
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                </Buttons>
                            </ext:FieldSet>--%>
                            <ext:GridPanel ID="GridPanelTiposCta" runat="server" StoreID="StoreTiposCta" Height="250" Layout="FitLayout" StripeRows="true"
                                    Header="false" Border="false">
                                <TopBar>
                                    <ext:Toolbar runat="server">
                                        <Items>
                                            <ext:TextField ID="txtTiposCtaPMA" EmptyText="Descripción" Width="200" runat="server" />
                                            <ext:Button ID="btnTiposCtaPMA" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnTiposCtaPMA_Click" Before="var valid= #{panelTipoCuentaPMA}.getForm().isValid(); if (!valid) {} return valid;">
                                                        <ExtraParams>
                                                            <ext:Parameter Name="IdProducto" Value="idGMA" Mode="Raw" />
                                                        </ExtraParams>
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <ColumnModel runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_TipoCuenta" Hidden="true" />
                                        <ext:Column DataIndex="ClaveTipoCuenta" Header="Clave" />
                                        <ext:Column DataIndex="Descripcion" Header="Descripción" Width="250" />
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel SingleSelect="true" />
                                </SelectionModel>
                                <DirectEvents>
                                    <RowClick OnEvent="selectTipoCta_Event">
                                        <ExtraParams>
                                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridPanelTiposCta}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                            <ext:Parameter Name="IdProducto" Value="idGMA" Mode="Raw" />
                                        </ExtraParams>
                                    </RowClick>
                                </DirectEvents>
                            </ext:GridPanel>
                            <ext:GridPanel ID="GridTipoCuentaPMA" runat="server" StoreID="StorePMA" Title="Parámetros del Tipo de Cuenta" 
                                    Height="500" Layout="FitLayout" StripeRows="true">
                                <Listeners>
                                    <KeyDown Fn="iniciaEdicionPMA" />
                                    <AfterEdit Fn="trasEdicionPMA" />
                                </Listeners>
                                <ColumnModel ID="ColumnModel2" runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_ValorParametroMultiasignacion" Hidden="true" />
                                        <ext:Column DataIndex="ID_ParametroMultiasignacion" Hidden="true" />
                                        <ext:Column DataIndex="ID_Entidad" Hidden="true" />
                                        <ext:Column DataIndex="ID_CadenaComercial" Hidden="true" />
                                        <ext:Column DataIndex="ID_Origen" Hidden="true" />
                                        <ext:Column DataIndex="ID_Producto" Hidden="true" />
                                        <ext:Column DataIndex="Nombre" Header="Nombre" />
                                        <ext:Column DataIndex="Descripcion" Header="Descripción" />
                                        <ext:Column Header="Valor" DataIndex="Valor" Width="300">
                                            <Editor>
                                                <ext:TextField ID="txtValorTCPMA" runat="server" />
                                            </Editor>
                                        </ext:Column>
                                        <ext:Column DataIndex="ID_Vigencia" Header="Vigencia">
                                            <Editor>
                                                <ext:ComboBox ID="ComboBox6"
                                                    runat="server"
                                                    StoreID="StoreVigencias"
                                                    DisplayField="Descripcion"
                                                    ValueField="ID_Vigencia" />
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
                    <ext:FormPanel ID="panelGpoCuentaPMA" runat="server" Title="Configuraciones de Parámetros" Hidden="true">
                        <Items>
                            <ext:GridPanel ID="GridPanelGposCuenta" runat="server" StoreID="StoreGpoCuenta" Height="250" Layout="FitLayout" StripeRows="true"
                                    Header="false" Border="false">
                                <TopBar>
                                    <ext:Toolbar runat="server">
                                        <Items>
                                            <ext:TextField ID="txtGrupoCuenta" EmptyText="Descripción" Width="200" runat="server" />
                                            <ext:Button ID="btnGpoCuentaPMA" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnGpoCuentaPMA_Click" Before="var valid= #{panelGpoCuentaPMA}.getForm().isValid(); if (!valid) {} return valid;">
                                                        <ExtraParams>
                                                            <ext:Parameter Name="IdProducto" Value="idGMA" Mode="Raw" />
                                                        </ExtraParams>
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <ColumnModel ID="ColumnModel7" runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_GrupoCuenta" Hidden="true" />
                                        <ext:Column DataIndex="ClaveGrupoCuenta" Header="Clave" />
                                        <ext:Column DataIndex="Descripcion" Header="Descripción" Width="400" />
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel SingleSelect="true" />
                                </SelectionModel>
                                <DirectEvents>
                                    <RowClick OnEvent="selectGpoCuenta_Event">
                                        <ExtraParams>
                                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridPanelGposCuenta}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                            <ext:Parameter Name="IdProducto" Value="idGMA" Mode="Raw" />
                                        </ExtraParams>
                                    </RowClick>
                                </DirectEvents>
                            </ext:GridPanel>
                            <ext:GridPanel ID="GridGpoCuentaPMA" runat="server" StoreID="StorePMA" Title="Parámetros del Grupo de Cuenta" 
                                    Height="500" Layout="FitLayout" StripeRows="true">
                                <Listeners>
                                    <KeyDown Fn="iniciaEdicionPMA" />
                                    <AfterEdit Fn="trasEdicionPMA" />
                                </Listeners>
                                <ColumnModel ID="ColumnModel3" runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_ValorParametroMultiasignacion" Hidden="true" />
                                        <ext:Column DataIndex="ID_ParametroMultiasignacion" Hidden="true" />
                                        <ext:Column DataIndex="ID_Entidad" Hidden="true" />
                                        <ext:Column DataIndex="ID_CadenaComercial" Hidden="true" />
                                        <ext:Column DataIndex="ID_Origen" Hidden="true" />
                                        <ext:Column DataIndex="ID_Producto" Hidden="true" />
                                        <ext:Column DataIndex="Nombre" Header="Nombre" />
                                        <ext:Column DataIndex="Descripcion" Header="Descripcion" />
                                        <ext:Column Header="Valor" DataIndex="Valor" Width="300">
                                            <Editor>
                                                <ext:TextField ID="txtValorGCPMA" runat="server" />
                                            </Editor>
                                        </ext:Column>
                                        <ext:Column DataIndex="ID_Vigencia" Header="Vigencia">
                                            <Editor>
                                                <ext:ComboBox ID="ComboBox7"
                                                    runat="server"
                                                    StoreID="StoreVigencias"
                                                    DisplayField="Descripcion"
                                                    ValueField="ID_Vigencia" />
                                            </Editor>
                                        </ext:Column>
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel3" runat="server" SingleSelect="true" />
                                </SelectionModel>
                            </ext:GridPanel>
                        </Items>
                    </ext:FormPanel>
                    <ext:FormPanel ID="panelGpoTarjetaPMA" runat="server" Title="Configuraciones de Parámetros" Hidden="true">
                        <Items>
                            <ext:GridPanel ID="GridGpoTarjetaPMA" runat="server" StoreID="StorePMA" AutoExpandColumn="Valor" Title="Grupo de Tarjeta">
                                <Listeners>
                                    <KeyDown Fn="iniciaEdicionPMA" />
                                    <AfterEdit Fn="trasEdicionPMA" />
                                </Listeners>
                                <ColumnModel ID="ColumnModel4" runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_ValorParametroMultiasignacion" Hidden="true" />
                                        <ext:Column DataIndex="ID_ParametroMultiasignacion" Hidden="true" />
                                        <ext:Column DataIndex="ID_Entidad" Hidden="true" />
                                        <ext:Column DataIndex="ID_CadenaComercial" Hidden="true" />
                                        <ext:Column DataIndex="ID_Origen" Hidden="true" />
                                        <ext:Column DataIndex="ID_Producto" Hidden="true" />
                                        <ext:Column DataIndex="Nombre" Header="Nombre" />
                                        <ext:Column DataIndex="Descripcion" Header="Descripcion" />
                                        <ext:Column Header="Valor" DataIndex="Valor" Width="300">
                                            <Editor>
                                                <ext:TextField ID="txtValorGTPMA" runat="server" />
                                            </Editor>
                                        </ext:Column>
                                        <ext:Column DataIndex="ID_Vigencia" Header="Vigencia">
                                            <Editor>
                                                <ext:ComboBox ID="ComboBox8"
                                                    runat="server"
                                                    StoreID="StoreVigencias"
                                                    DisplayField="Descripcion"
                                                    ValueField="ID_Vigencia" />
                                            </Editor>
                                        </ext:Column>
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel4" runat="server" SingleSelect="true" />
                                </SelectionModel>
                            </ext:GridPanel>
                        </Items>
                    </ext:FormPanel>
                    <ext:FormPanel ID="panelTarjetaCtaPMA" runat="server" Title="Configuraciones de Parámetros" Hidden="true">
                        <Items>
                            <ext:GridPanel ID="GridPanelTarjetasCtas" runat="server" StoreID="StoreTarjetaCta" Height="250" Layout="FitLayout" StripeRows="true"
                                    Header="false" Border="false">
                                <TopBar>
                                    <ext:Toolbar runat="server">
                                        <Items>
                                            <ext:NumberField ID="nfNumTarjeta" EmptyText="Número de Tarjeta" Width="100" runat="server"
                                                AllowNegative="False" MaxLength="16" />
                                            <ext:TextField ID="txtApPaterno" EmptyText="Apellido Paterno" Width="100" runat="server" />
                                            <ext:TextField ID="txtApMaterno" EmptyText="Apellido Materno" Width="100" runat="server" />
                                            <ext:TextField ID="txtNombre" EmptyText="Nombre" Width="200" runat="server" />
                                            <ext:Button ID="btnTarjetaCta" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnTarjetaCta_Click" Before="var valid= #{panelTarjetaCtaPMA}.getForm().isValid(); if (!valid) {} return valid;">
                                                        <ExtraParams>
                                                            <ext:Parameter Name="IdProducto" Value="idGMA" Mode="Raw" />
                                                        </ExtraParams>
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <ColumnModel ID="ColumnModel8" runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_Cuenta" Hidden="true" />
                                        <ext:Column DataIndex="ID_MA" Hidden="true" />
                                        <ext:Column DataIndex="Tarjeta" Header="Número de Tarjeta" Width="200" />
                                        <ext:Column DataIndex="NombreTarjetahabiente" Header="Tarjetahabiente" Width="400" />
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel SingleSelect="true" />
                                </SelectionModel>
                                <DirectEvents>
                                    <RowClick OnEvent="selectTarjetaCta_Event">
                                        <ExtraParams>
                                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridPanelTarjetasCtas}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                            <ext:Parameter Name="IdProducto" Value="idGMA" Mode="Raw" />
                                        </ExtraParams>
                                    </RowClick>
                                </DirectEvents>
                            </ext:GridPanel>
                            <ext:GridPanel ID="GridTarjetaCtaPMA" runat="server" StoreID="StorePMA" AutoExpandColumn="Valor" Title="Tarjeta/Cuenta"
                                Height="500" Layout="FitLayout" StripeRows="true">
                                <Listeners>
                                    <KeyDown Fn="iniciaEdicionPMA" />
                                    <AfterEdit Fn="trasEdicionPMA" />
                                </Listeners>
                                <ColumnModel ID="ColumnModel5" runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_ValorParametroMultiasignacion" Hidden="true" />
                                        <ext:Column DataIndex="ID_ParametroMultiasignacion" Hidden="true" />
                                        <ext:Column DataIndex="ID_Entidad" Hidden="true" />
                                        <ext:Column DataIndex="ID_CadenaComercial" Hidden="true" />
                                        <ext:Column DataIndex="ID_Origen" Hidden="true" />
                                        <ext:Column DataIndex="ID_Producto" Hidden="true" />
                                        <ext:Column DataIndex="Nombre" Header="Nombre" />
                                        <ext:Column DataIndex="Descripcion" Header="Descripcion" />
                                        <ext:Column Header="Valor" DataIndex="Valor" Width="300">
                                            <Editor>
                                                <ext:TextField ID="txtValorTCtaPMA" runat="server" />
                                            </Editor>
                                        </ext:Column>
                                        <ext:Column DataIndex="ID_Vigencia" Header="Vigencia">
                                            <Editor>
                                                <ext:ComboBox ID="ComboBox9"
                                                    runat="server"
                                                    StoreID="StoreVigencias"
                                                    DisplayField="Descripcion"
                                                    ValueField="ID_Vigencia" />
                                            </Editor>
                                        </ext:Column>
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel5" runat="server" SingleSelect="true" />
                                </SelectionModel>
                            </ext:GridPanel>
                        </Items>
                    </ext:FormPanel>
                    <ext:Panel ID="panelValidaciones" runat="server" Title="Configuraciones de Validaciones" Hidden="true">
                        <Items>
<%--                            <ext:FieldSet ID="FieldSetTiposCtaVMA" runat="server" Title="Datos para Búsqueda" Layout="Fit" Hidden="true">
                                <Items>
                                    <ext:ComboBox
                                        LabelAlign="Left"
                                        FieldLabel="Tipos de Cuenta"
                                        ID="CBoxTiposCtaVMA"
                                        ForceSelection="false"
                                        EmptyText="Selecciona una Opción..."
                                        runat="server"
                                        StoreID="StoreTiposCta"
                                        MsgTarget="Side"
                                        DisplayField="Descripcion"
                                        ValueField="ID_TipoCuenta"
                                        Editable="false" />
                                </Items>
                                <Buttons>
                                    <ext:Button
                                        ID="btnTiposCtaVMA"
                                        runat="server"
                                        Text="Buscar"
                                        Icon="Magnifier">
                                        <DirectEvents>
                                            <Click OnEvent="btnTiposCtaVMA_Click" Before="var valid= #{panelValidaciones}.getForm().isValid(); if (!valid) {} return valid;">
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                </Buttons>
                            </ext:FieldSet>
                            <ext:FieldSet ID="FieldSetGruposCtaVMA" runat="server" Title="Datos para Búsqueda" Layout="Fit" Hidden="true">
                                <Items>
                                    <ext:ComboBox
                                        LabelAlign="Left"
                                        FieldLabel="Grupos de Cuenta"
                                        ID="cBoxGruposCtaVMA"
                                        ForceSelection="false"
                                        EmptyText="Selecciona una Opción..."
                                        runat="server"
                                        StoreID="StoreGruposCta"
                                        MsgTarget="Side"
                                        DisplayField="Descripcion"
                                        ValueField="ID_GrupoCuenta"
                                        Editable="false" />
                                </Items>
                                <Buttons>
                                    <ext:Button
                                        ID="btnGruposCtaVMA"
                                        runat="server"
                                        Text="Buscar"
                                        Icon="Magnifier">
                                        <DirectEvents>
                                            <Click OnEvent="btnGruposCtaVMA_Click" Before="var valid= #{panelValidaciones}.getForm().isValid(); if (!valid) {} return valid;">
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                </Buttons>
                            </ext:FieldSet>--%>
                            <ext:TreeGrid ID="GridVMA" runat="server" AutoExpandColumn="Validacion" Hidden="true" Layout="FitLayout">
                                <Columns>
                                    <ext:TreeGridColumn DataIndex="ID_ValidadorMultiasignacion" Hidden="true" />
                                    <ext:TreeGridColumn DataIndex="Validacion" Header="Validación" Width="80" />
                                    <ext:TreeGridColumn DataIndex="Formula" Header="Formula" Width="80" />
                                    <ext:TreeGridColumn DataIndex="TipoValidacion" Header="Tipo de Validación" Width="150" />
                                    <ext:TreeGridColumn DataIndex="Declinar" Header="Declinar" Width="80" />
                                    <ext:TreeGridColumn DataIndex="Estatus" Header="Estatus" Width="80" />
                                    <ext:TreeGridColumn DataIndex="OrdenValidacion" Header="Orden Validación" Width="80" />
                                    <ext:TreeGridColumn DataIndex="PreReglas" Header="PreReglas" Width="80" />
                                    <ext:TreeGridColumn DataIndex="PostReglas" Header="PostReglas" Width="80" />
                                    <ext:TreeGridColumn DataIndex="Vigencia" Header="Vigencia" Width="80" />
                                    <ext:TreeGridColumn DataIndex="Prioridad" Header="Prioridad" Width="80" />
                                    <ext:TreeGridColumn DataIndex="NodosTrue" Hidden="true" />
                                    <ext:TreeGridColumn DataIndex="NodosFalse" Hidden="true" />
                                </Columns>
                                <Root>
                                    <ext:TreeNode NodeID="0" Text="Root" Icon="FolderGo" />
                                </Root>
                                <Listeners>
                                    <ContextMenu Handler="showMenu(#{GridVMA}, #{TreeContextMenu}, node, e);" />
                                    <AfterRender Handler="this.body.on('dblclick', function() {showFormNuevaValidacion(#{GridVMA}, #{GridVMA}.getRootNode(), 0)})" />
                                </Listeners>
                            </ext:TreeGrid>
                           <%-- <ext:TreeGrid ID="GridTipoCtaVMA" runat="server" AutoExpandColumn="Validacion" Title="Tipo de Cuenta" Hidden="true">
                                <Columns>
                                    <ext:TreeGridColumn DataIndex="ID_ValidadorMultiasignacion" Hidden="true" />
                                    <ext:TreeGridColumn DataIndex="Validacion" Header="Validación" Width="80" />
                                    <ext:TreeGridColumn DataIndex="Formula" Header="Formula" Width="80" />
                                    <ext:TreeGridColumn DataIndex="TipoValidacion" Header="Tipo de Validación" Width="150" />
                                    <ext:TreeGridColumn DataIndex="Declinar" Header="Declinar" Width="80" />
                                    <ext:TreeGridColumn DataIndex="Estatus" Header="Estatus" Width="80" />
                                    <ext:TreeGridColumn DataIndex="OrdenValidacion" Header="Orden Validación" Width="80" />
                                    <ext:TreeGridColumn DataIndex="PreReglas" Header="PreReglas" Width="80" />
                                    <ext:TreeGridColumn DataIndex="PostReglas" Header="PostReglas" Width="80" />
                                    <ext:TreeGridColumn DataIndex="Vigencia" Header="Vigencia" Width="80" />
                                    <ext:TreeGridColumn DataIndex="Prioridad" Header="Prioridad" Width="80" />
                                    <ext:TreeGridColumn DataIndex="NodosTrue" Hidden="true" />
                                    <ext:TreeGridColumn DataIndex="NodosFalse" Hidden="true" />
                                </Columns>
                                <Root>
                                    <ext:TreeNode NodeID="0" Text="Root" Icon="FolderGo" />
                                </Root>
                                <Listeners>
                                    <ContextMenu Handler="showMenu(#{GridTipoCtaVMA}, #{TreeContextMenuTipoCta}, node, e);" />
                                    <AfterRender Handler="this.body.on('dblclick', function() {showFormNuevaValidacion(#{GridTipoCtaVMA}, #{GridTipoCtaVMA}.getRootNode(), 0)})" />
                                </Listeners>
                            </ext:TreeGrid>
                            <ext:TreeGrid ID="GridGpoCuentaVMA" runat="server" AutoExpandColumn="Validacion" Title="Grupo de Cuenta" Hidden="true">
                                <Columns>
                                    <ext:TreeGridColumn DataIndex="ID_ValidadorMultiasignacion" Hidden="true" />
                                    <ext:TreeGridColumn DataIndex="Validacion" Header="Validación" Width="80" />
                                    <ext:TreeGridColumn DataIndex="Formula" Header="Formula" Width="80" />
                                    <ext:TreeGridColumn DataIndex="TipoValidacion" Header="Tipo de Validación" Width="150" />
                                    <ext:TreeGridColumn DataIndex="Declinar" Header="Declinar" Width="80" />
                                    <ext:TreeGridColumn DataIndex="Estatus" Header="Estatus" Width="80" />
                                    <ext:TreeGridColumn DataIndex="OrdenValidacion" Header="Orden Validación" Width="80" />
                                    <ext:TreeGridColumn DataIndex="PreReglas" Header="PreReglas" Width="80" />
                                    <ext:TreeGridColumn DataIndex="PostReglas" Header="PostReglas" Width="80" />
                                    <ext:TreeGridColumn DataIndex="Vigencia" Header="Vigencia" Width="80" />
                                    <ext:TreeGridColumn DataIndex="Prioridad" Header="Prioridad" Width="80" />
                                    <ext:TreeGridColumn DataIndex="NodosTrue" Hidden="true" />
                                    <ext:TreeGridColumn DataIndex="NodosFalse" Hidden="true" />
                                </Columns>
                                <Root>
                                    <ext:TreeNode NodeID="0" Text="Root" Icon="FolderGo" />
                                </Root>
                                <Listeners>
                                    <ContextMenu Handler="showMenu(#{GridGpoCuentaVMA}, #{TreeContextMenuGpoCuenta}, node, e);" />
                                    <AfterRender Handler="this.body.on('dblclick', function() {showFormNuevaValidacion(#{GridGpoCuentaVMA}, #{GridGpoCuentaVMA}.getRootNode(), 0)})" />
                                </Listeners>
                            </ext:TreeGrid>
                            <ext:TreeGrid ID="GridGpoTarjetaVMA" runat="server" AutoExpandColumn="Validacion" Title="Grupo de Tarjeta" Hidden="true">
                                <Columns>
                                    <ext:TreeGridColumn DataIndex="ID_ValidadorMultiasignacion" Hidden="true" />
                                    <ext:TreeGridColumn DataIndex="Validacion" Header="Validación" Width="80" />
                                    <ext:TreeGridColumn DataIndex="Formula" Header="Formula" Width="80" />
                                    <ext:TreeGridColumn DataIndex="TipoValidacion" Header="Tipo de Validación" Width="150" />
                                    <ext:TreeGridColumn DataIndex="Declinar" Header="Declinar" Width="80" />
                                    <ext:TreeGridColumn DataIndex="Estatus" Header="Estatus" Width="80" />
                                    <ext:TreeGridColumn DataIndex="OrdenValidacion" Header="Orden Validación" Width="80" />
                                    <ext:TreeGridColumn DataIndex="PreReglas" Header="PreReglas" Width="80" />
                                    <ext:TreeGridColumn DataIndex="PostReglas" Header="PostReglas" Width="80" />
                                    <ext:TreeGridColumn DataIndex="Vigencia" Header="Vigencia" Width="80" />
                                    <ext:TreeGridColumn DataIndex="Prioridad" Header="Prioridad" Width="80" />
                                    <ext:TreeGridColumn DataIndex="NodosTrue" Hidden="true" />
                                    <ext:TreeGridColumn DataIndex="NodosFalse" Hidden="true" />
                                </Columns>
                                <Root>
                                    <ext:TreeNode NodeID="0" Text="Root" Icon="FolderGo" />
                                </Root>
                                <Listeners>
                                    <ContextMenu Handler="showMenu(#{GridGpoTarjetaVMA}, #{TreeContextMenuGpoTarjeta}, node, e);" />
                                    <AfterRender Handler="this.body.on('dblclick', function() {showFormNuevaValidacion(#{GridGpoTarjetaVMA}, #{GridGpoTarjetaVMA}.getRootNode(), 0)})" />
                                </Listeners>
                            </ext:TreeGrid>
                            <ext:TreeGrid ID="GridTarjetaCtaVMA" runat="server" AutoExpandColumn="Validacion" Title="Tarjeta/Cuenta" Hidden="true">
                                <Columns>
                                    <ext:TreeGridColumn DataIndex="ID_ValidadorMultiasignacion" Hidden="true" />
                                    <ext:TreeGridColumn DataIndex="Validacion" Header="Validación" Width="80" />
                                    <ext:TreeGridColumn DataIndex="Formula" Header="Formula" Width="80" />
                                    <ext:TreeGridColumn DataIndex="TipoValidacion" Header="Tipo de Validación" Width="150" />
                                    <ext:TreeGridColumn DataIndex="Declinar" Header="Declinar" Width="80" />
                                    <ext:TreeGridColumn DataIndex="Estatus" Header="Estatus" Width="80" />
                                    <ext:TreeGridColumn DataIndex="OrdenValidacion" Header="Orden Validación" Width="80" />
                                    <ext:TreeGridColumn DataIndex="PreReglas" Header="PreReglas" Width="80" />
                                    <ext:TreeGridColumn DataIndex="PostReglas" Header="PostReglas" Width="80" />
                                    <ext:TreeGridColumn DataIndex="Vigencia" Header="Vigencia" Width="80" />
                                    <ext:TreeGridColumn DataIndex="Prioridad" Header="Prioridad" Width="80" />
                                    <ext:TreeGridColumn DataIndex="NodosTrue" Hidden="true" />
                                    <ext:TreeGridColumn DataIndex="NodosFalse" Hidden="true" />
                                </Columns>
                                <Root>
                                    <ext:TreeNode NodeID="0" Text="Root" Icon="FolderGo" />
                                </Root>
                                <Listeners>
                                    <ContextMenu Handler="showMenu(#{GridTarjetaCtaVMA}, #{TreeContextMenuTarjetaCta}, node, e);" />
                                    <AfterRender Handler="this.body.on('dblclick', function() {showFormNuevaValidacion(#{GridTarjetaCtaVMA}, #{GridTarjetaCtaVMA}.getRootNode(), 0)})" />
                                </Listeners>
                            </ext:TreeGrid>--%>
                        </Items>
                    </ext:Panel>
                  <%--  <ext:Panel ID="panelTipoCtaRMA" runat="server" Title="Configuraciones de Reglas" Hidden="true">
                        <Items>
                            <ext:FieldSet ID="FieldSetTiposCtaRMA" runat="server" Title="Datos para Búsqueda" Layout="Fit">
                                <Items>
                                    <ext:ComboBox
                                        LabelAlign="Left"
                                        FieldLabel="Tipos de Cuenta"
                                        ID="CBoxTiposCtaRMA"
                                        ForceSelection="false"
                                        EmptyText="Selecciona una Opción..."
                                        runat="server"
                                        StoreID="StoreTiposCta"
                                        MsgTarget="Side"
                                        DisplayField="Descripcion"
                                        ValueField="ID_TipoCuenta"
                                        Editable="false" />
                                </Items>
                                <Buttons>
                                    <ext:Button
                                        ID="btnTiposCtaRMA"
                                        runat="server"
                                        Text="Buscar"
                                        Icon="Magnifier">
                                        <DirectEvents>
                                            <Click OnEvent="btnTiposCtaRMA_Click" Before="var valid= #{panelTipoCtaRMA}.getForm().isValid(); if (!valid) {} return valid;">
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                </Buttons>
                            </ext:FieldSet>
                            <ext:GridPanel ID="GridTipoCtaRMA" runat="server" Title="Tipo de Cuenta">
                                <Store>
                                    <ext:Store ID="StoreTipoCtaRMA" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_ReglaMultiasignacion">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Regla" />
                                                    <ext:RecordField Name="ID_ReglaMultiasignacion" />
                                                    <ext:RecordField Name="ID_Entidad" />
                                                    <ext:RecordField Name="ID_CadenaComercial" />
                                                    <ext:RecordField Name="Nombre" />
                                                    <ext:RecordField Name="Vigencia" />
                                                    <ext:RecordField Name="OrdenEjecucion" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <Listeners>
                                    <KeyDown Fn="iniciaEdicionRMA" />
                                    <AfterEdit Fn="trasEdicionRMA" />
                                </Listeners>
                                <ColumnModel ID="ColumnModel7" runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_Regla" Hidden="true" />
                                        <ext:Column DataIndex="ID_ReglaMultiasignacion" Hidden="true" />
                                        <ext:Column DataIndex="ID_Entidad" Hidden="true" />
                                        <ext:Column DataIndex="ID_CadenaComercial" Hidden="true" />
                                        <ext:Column DataIndex="Nombre" Header="Nombre" Width="200" />
                                        <ext:Column DataIndex="Vigencia" Header="Vigencia" Width="200" />
                                        <ext:Column DataIndex="OrdenEjecucion" Header="Orden de Ejecución" Width="150">
                                            <Editor>
                                                <ext:TextField ID="txtOrEjTipoCtaRMA" runat="server" />
                                            </Editor>
                                        </ext:Column>
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel7" runat="server" SingleSelect="true" />
                                </SelectionModel>
                            </ext:GridPanel>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="panelGpoCuentaRMA" runat="server" Title="Configuraciones de Reglas" Hidden="true">
                        <Items>
                            <ext:FieldSet ID="FieldSetGruposCtaRMA" runat="server" Title="Datos para Búsqueda" Layout="Fit">
                                <Items>
                                    <ext:ComboBox
                                        LabelAlign="Left"
                                        FieldLabel="Grupos de Cuenta"
                                        ID="cBoxGruposCtaRMA"
                                        ForceSelection="false"
                                        EmptyText="Selecciona una Opción..."
                                        runat="server"
                                        StoreID="StoreGruposCta"
                                        MsgTarget="Side"
                                        DisplayField="Descripcion"
                                        ValueField="ID_GrupoCuenta"
                                        Editable="false" />
                                </Items>
                                <Buttons>
                                    <ext:Button
                                        ID="btnGruposCtaRMA"
                                        runat="server"
                                        Text="Buscar"
                                        Icon="Magnifier">
                                        <DirectEvents>
                                            <Click OnEvent="btnGruposCtaRMA_Click" Before="var valid= #{panelGpoCuentaRMA}.getForm().isValid(); if (!valid) {} return valid;">
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                </Buttons>
                            </ext:FieldSet>
                            <ext:GridPanel ID="GridGpoCuentaRMA" runat="server" Title="Grupo de Cuenta">
                                <Store>
                                    <ext:Store ID="StoreGpoCuentaRMA" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_ReglaMultiasignacion">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Regla" />
                                                    <ext:RecordField Name="ID_ReglaMultiasignacion" />
                                                    <ext:RecordField Name="ID_Entidad" />
                                                    <ext:RecordField Name="ID_CadenaComercial" />
                                                    <ext:RecordField Name="Nombre" />
                                                    <ext:RecordField Name="Vigencia" />
                                                    <ext:RecordField Name="OrdenEjecucion" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <Listeners>
                                    <KeyDown Fn="iniciaEdicionRMA" />
                                    <AfterEdit Fn="trasEdicionRMA" />
                                </Listeners>
                                <ColumnModel ID="ColumnModel8" runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_Regla" Hidden="true" />
                                        <ext:Column DataIndex="ID_ReglaMultiasignacion" Hidden="true" />
                                        <ext:Column DataIndex="ID_Entidad" Hidden="true" />
                                        <ext:Column DataIndex="ID_CadenaComercial" Hidden="true" />
                                        <ext:Column DataIndex="Nombre" Header="Nombre" Width="200" />
                                        <ext:Column DataIndex="Vigencia" Header="Vigencia" Width="200" />
                                        <ext:Column DataIndex="OrdenEjecucion" Header="Orden de Ejecución" Width="150">
                                            <Editor>
                                                <ext:TextField ID="txtOrEjGpoCtaRMA" runat="server" />
                                            </Editor>
                                        </ext:Column>
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel8" runat="server" SingleSelect="true" />
                                </SelectionModel>
                            </ext:GridPanel>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="panelGpoTarjetaRMA" runat="server" Title="Configuraciones de Reglas" Hidden="true">
                        <Items>
                            <ext:GridPanel ID="GridGpoTarjetaRMA" runat="server" Title="Grupo de Tarjeta">
                                <Store>
                                    <ext:Store ID="StoreGpoTarjetaRMA" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_ReglaMultiasignacion">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Regla" />
                                                    <ext:RecordField Name="ID_ReglaMultiasignacion" />
                                                    <ext:RecordField Name="ID_Entidad" />
                                                    <ext:RecordField Name="ID_CadenaComercial" />
                                                    <ext:RecordField Name="Nombre" />
                                                    <ext:RecordField Name="Vigencia" />
                                                    <ext:RecordField Name="OrdenEjecucion" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <Listeners>
                                    <KeyDown Fn="iniciaEdicionRMA" />
                                    <AfterEdit Fn="trasEdicionRMA" />
                                </Listeners>
                                <ColumnModel ID="ColumnModel9" runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_Regla" Hidden="true" />
                                        <ext:Column DataIndex="ID_ReglaMultiasignacion" Hidden="true" />
                                        <ext:Column DataIndex="ID_Entidad" Hidden="true" />
                                        <ext:Column DataIndex="ID_CadenaComercial" Hidden="true" />
                                        <ext:Column DataIndex="Nombre" Header="Nombre" Width="200" />
                                        <ext:Column DataIndex="Vigencia" Header="Vigencia" Width="200" />
                                        <ext:Column DataIndex="OrdenEjecucion" Header="Orden de Ejecución" Width="150">
                                            <Editor>
                                                <ext:TextField ID="txtOrEjGpoTarjRMA" runat="server" />
                                            </Editor>
                                        </ext:Column>
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel9" runat="server" SingleSelect="true" />
                                </SelectionModel>
                            </ext:GridPanel>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="panelTarjetaCtaRMA" runat="server" Title="Configuraciones de Reglas" Hidden="true">
                        <Items>
                            <ext:GridPanel ID="GridTarjetaCtaRMA" runat="server" Title="Tarjeta/Cuenta">
                                <Store>
                                    <ext:Store ID="StoreTarjetaCtaRMA" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_ReglaMultiasignacion">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Regla" />
                                                    <ext:RecordField Name="ID_ReglaMultiasignacion" />
                                                    <ext:RecordField Name="ID_Entidad" />
                                                    <ext:RecordField Name="ID_CadenaComercial" />
                                                    <ext:RecordField Name="Nombre" />
                                                    <ext:RecordField Name="Vigencia" />
                                                    <ext:RecordField Name="OrdenEjecucion" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <Listeners>
                                    <KeyDown Fn="iniciaEdicionRMA" />
                                    <AfterEdit Fn="trasEdicionRMA" />
                                </Listeners>
                                <ColumnModel ID="ColumnModel10" runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_Regla" Hidden="true" />
                                        <ext:Column DataIndex="ID_ReglaMultiasignacion" Hidden="true" />
                                        <ext:Column DataIndex="ID_Entidad" Hidden="true" />
                                        <ext:Column DataIndex="ID_CadenaComercial" Hidden="true" />
                                        <ext:Column DataIndex="Nombre" Header="Nombre" Width="200" />
                                        <ext:Column DataIndex="Vigencia" Header="Vigencia" Width="200" />
                                        <ext:Column DataIndex="OrdenEjecucion" Header="Orden de Ejecución" Width="150">
                                            <Editor>
                                                <ext:TextField ID="txtOrEjTarjCtaRMA" runat="server" />
                                            </Editor>
                                        </ext:Column>
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel10" runat="server" SingleSelect="true" />
                                </SelectionModel>
                            </ext:GridPanel>
                        </Items>
                    </ext:Panel>--%>
                </Items>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>

