<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="EditarProducto.aspx.cs" Inherits="Administracion.EditarProducto" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

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
                Administracion.LlenarFieldSetReglasPMA();
            }
            else if (node.text == 'Tipo de Cuenta') {
                Administracion.LlenarFieldSetTiposCtaPMA();
            }
            else if (node.text == 'Grupo de Cuenta') {
                Administracion.LlenarFieldSetGposCuentaPMA();
            }
            //else if (node.text == 'Grupo de Tarjeta') {
            else if (node.text == 'Grupo') {
                Administracion.LlenarGridGpoTarjetaPMA(idGMA);
            }
            else if (node.text == 'Tarjeta/Cuenta') {
                Administracion.LlenarFieldSetTarjetaCuentaPMA();
            }
            else if (node.text == 'Validaciones') {
                Administracion.LlenaGridVMA(MainContent_cmbCadenaComercial.value);
            }
        }

        var iniciaEdicionPMA = function (e) {
            if (e.getKey() === e.ENTER) {
                //var grid = entityName == 'Regla' ? GridReglaPMA :
                //    entityName == 'Tipo de Cuenta' ? GridTipoCuentaPMA :
                //    entityName == 'Grupo de Cuenta' ? GridGpoCuentaPMA :
                //    entityName == 'Grupo de Tarjeta' ? GridGpoTarjetaPMA :
                //    GridTarjetaCtaPMA,
                var grid = GridGpoTarjetaPMA,
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
            Administracion.EdicionPMA(e.record.data.ID_ValorParametroMultiasignacion,
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
                record = grid.getSelectionModel().getSelected(),
                index = grid.store.indexOf(record);

                grid.startEditing(index, 1);
            }
        };

        var trasEdicionRMA = function (e) {
            Administracion.ValidaCamposRMA(
                e.record.data.Prioridad,
                e.record.data.ID_Vigencia,
                e.record.data.OrdenEjecucion);
        };

        function setGridName() {
            gridName = MainContent_GridVMA;
        }

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
        
        function refreshTree(tree, result) {
            var nodes = eval(result);

            if (nodes != null && nodes.length > 0) {
                tree.initChildren(nodes);
            } else {
                tree.getRootNode().removeChildren();
            }
        }

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
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

    <ext:Store ID="StoreVigencias" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_Vigencia">
                <Fields>
                    <ext:RecordField Name="ID_Vigencia" />
                    <ext:RecordField Name="Clave" />
                    <ext:RecordField Name="Descripcion" />
                    <ext:RecordField Name="ID_TipoVigencia" />
                    <ext:RecordField Name="TipoVigencia" />
                    <ext:RecordField Name="FechaIncial" />
                    <ext:RecordField Name="FechaFinal" />
                    <ext:RecordField Name="HoraInicial" />
                    <ext:RecordField Name="HoraFinal" />
                    <ext:RecordField Name="ID_Periodo" />
                    <ext:RecordField Name="Periodo" />
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
            <ext:MenuItem ID="MenuItemEditar" runat="server" Text="Editar" Icon="NoteEdit">
                <DirectEvents>
                    <Click OnEvent="muestraDialogValidacion"
                        Before="extraParams.idProd = idGMA;
                        extraParams.idVM        = #{GridVMA}.getSelectionModel().getSelectedNode().attributes['ID_ValidadorMultiasignacion'];
                        extraParams.validacion  = #{GridVMA}.getSelectionModel().getSelectedNode().attributes['Validacion'];
                        extraParams.formula     = #{GridVMA}.getSelectionModel().getSelectedNode().attributes['Formula'];
                        extraParams.tipoVal     = #{GridVMA}.getSelectionModel().getSelectedNode().attributes['TipoValidacion'];
                        extraParams.declinar    = #{GridVMA}.getSelectionModel().getSelectedNode().attributes['Declinar'];
                        extraParams.ordenVal    = #{GridVMA}.getSelectionModel().getSelectedNode().attributes['OrdenValidacion'];
                        extraParams.preReglas   = #{GridVMA}.getSelectionModel().getSelectedNode().attributes['PreReglas'];
                        extraParams.postReglas  = #{GridVMA}.getSelectionModel().getSelectedNode().attributes['PostReglas'];
                        extraParams.vigencia    = #{GridVMA}.getSelectionModel().getSelectedNode().attributes['Vigencia'];
                        extraParams.prioridad   = #{GridVMA}.getSelectionModel().getSelectedNode().attributes['Prioridad'];" />
                </DirectEvents>
            </ext:MenuItem>
        </Items>
    </ext:Menu>


    <ext:Window ID="DialogEditarProducto" runat="server">
        <Items>
            <ext:BorderLayout ID="BorderLayout2" runat="server">
                <Center>
                    <ext:FormPanel ID="FormEditarProducto" runat="server" Padding="10" MonitorValid="true">
                        <Items>
                            <ext:TextField ID="TxtIDGrupoMA" runat="server" />
                            <ext:TextField ID="TxtClaveGrupoMA" runat="server" />
                            <ext:TextField ID="TxtDescripcionGrupoMA" runat="server" />
                            <ext:Panel ID="Panel1" runat="server" Height="25" BodyStyle="background-color: #FFF; border: 0px;">
                                <Items>
                                    <ext:BorderLayout ID="BorderLayout3" runat="server">
                                        <Center>
                                            <ext:ComboBox ID="ComboVigenciaGrupoMA" runat="server" StoreID="StoreVigencias" />
                                        </Center>
                                        <East>
                                            <ext:Button ID="Button1" runat="server" Text="Vigencia" Icon="Add" Margins="0 0 0 10">
                                                <DirectEvents>
                                                    <Click OnEvent="NuevaVigencia_Click" />
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
                                    <Click OnEvent="Click_GuardarProducto" />
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

    <ext:Window ID="DialogEditarVigencia" runat="server">
        <Items>
            <ext:BorderLayout runat="server">
                <Center>
                    <ext:FormPanel ID="FormEditarVigencia" runat="server" Padding="10" MonitorValid="true">
                        <Items>
                            <ext:TextField ID="TxtIDVigencia" runat="server" />
                            <ext:TextField ID="TxtVigenciaCalve" runat="server" />
                            <ext:TextField ID="TxtVigenciaDescripcion" runat="server" />
                            <ext:SelectBox ID="ComboTipoVigencia" StoreID="StoreTipoVigencia" runat="server" />
                            <ext:DateField ID="DateFechaInicial" runat="server" />
                            <ext:DateField ID="DateFechaFinal" runat="server" />
                            <ext:TimeField ID="TimeHoraInicial" runat="server" />
                            <ext:TimeField ID="TimeHoraFinal" runat="server" />
                            <ext:SelectBox ID="ComboPeriodo" StoreID="StorePeriodos" runat="server" />
                        </Items>
                        <Buttons>
                            <ext:Button ID="btnEditarVigencia" runat="server" Icon="Disk" Text="Guardar">
                                <DirectEvents>
                                    <Click OnEvent="GuardarVigencia_Click" />
                                </DirectEvents>
                            </ext:Button>
                        </Buttons>
                        <Listeners>
                            <ClientValidation Handler="#{btnEditarVigencia}.setDisabled(!valid);" />
                        </Listeners>
                    </ext:FormPanel>
                </Center>
            </ext:BorderLayout>
        </Items>
    </ext:Window>

    <ext:Window ID="TablaVigencias" runat="server">
        <TopBar>
            <ext:Toolbar ID="Toolbar1" runat="server">
                <Items>
                    <ext:Button ID="Button2" runat="server" Text="Vigencia" Icon="Add">
                        <DirectEvents>
                            <Click OnEvent="NuevaVigencia_Click" />
                        </DirectEvents>
                    </ext:Button>
                </Items>
            </ext:Toolbar>
        </TopBar>
        <Items>
            <ext:BorderLayout runat="server">
                <Center>
                    <ext:GridPanel ID="GridPanelVigencias" runat="server" StoreID="StoreVigencias">
                        <SelectionModel>
                            <ext:RowSelectionModel SingleSelect="true" />
                        </SelectionModel>
                        <DirectEvents>
                            <Command OnEvent="EditarVigencia_Click" />
                        </DirectEvents>
                        <BottomBar>
                            <ext:PagingToolbar ID="PagingToolbar2" runat="server" />
                        </BottomBar>
                    </ext:GridPanel>
                </Center>
            </ext:BorderLayout>
        </Items>
    </ext:Window>

    <ext:Window ID="DialogEditarRango" runat="server" Hidden="true" Width="500" Height="220" Modal="true"
        Title="Editar Rango" Resizable="false">
        <Items>
            <ext:FormPanel ID="FPEditarRango" runat="server" Padding="10" MonitorValid="true">
                <Items>
                    <ext:TextField ID="txtRango_IDGrupoMA" runat="server" Hidden="true" />
                    <ext:TextField ID="txtIDRango" runat="server" Hidden="true" />
                    <ext:TextField ID="txtRango_Clave" runat="server" FieldLabel="Clave"
                        MaxLength="20" AnchorHorizontal="100%" AllowBlank="false" />
                    <ext:TextField ID="txtRango_Descripcion" runat="server" FieldLabel="Descripción"
                        MaxLength="50" AnchorHorizontal="100%" AllowBlank="false" />
                    <ext:NumberField ID="NumRangoInicio" runat="server" AllowNegative="False"
                        FieldLabel="Inicio" MaxLength="15" AnchorHorizontal="100%" AllowBlank="false" />
                    <ext:NumberField ID="NumRangoFin" runat="server" AllowNegative="False"
                        FieldLabel="Fin" MaxLength="15" AnchorHorizontal="100%" AllowBlank="false" />
                    <ext:SelectBox ID="SelRangoEsActivo" runat="server" FieldLabel="Es Activo"
                        AnchorHorizontal="100%" AllowBlank="false">
                        <Items>
                            <ext:ListItem Text="SI" Value="1" />
                            <ext:ListItem Text="NO" Value="0" />
                        </Items>
                    </ext:SelectBox>
                </Items>
                <Buttons>
                    <ext:Button ID="btnGuardarRango" runat="server" Text="Guardar" Icon="Disk">
                        <DirectEvents>
                            <Click OnEvent="GuardarRangoEditado_Click" />
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
                <Listeners>
                    <ClientValidation Handler="#{btnGuardarRango}.setDisabled(!valid);" />
                </Listeners>
            </ext:FormPanel>
        </Items>
    </ext:Window>

    <ext:Window runat="server" ID="DialogValidacion" Title="Editar Validación" Modal="true" Layout="FitLayout" Width="500" 
        Closable="false" Hidden="true" Resizable="false" Icon="NoteEdit">
        <TopBar>
            <ext:Toolbar ID="Toolbar2" runat="server">
                <Items>
                    <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                    <ext:Button ID="btnEditaValidacion" runat="server" Icon="Disk" Text="Guardar">
                        <DirectEvents>
                            <Click OnEvent="editarValidacion_Event"
                                Before="setGridName();
                                extraParams.TipoValidacion=gridName.tipo;
                                extraParams.idNodo=gridName.menuNode.id;
                                extraParams.idProd = idGMA">
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                    <ext:Button ID="btnCancel" runat="server" Icon="Cancel" Text="Cancelar">
                        <Listeners>
                            <Click Handler="function(){#{DialogValidacion}.hide(); #{edita_validacion}.reset();}" />
                        </Listeners>
                    </ext:Button>
                </Items>
            </ext:Toolbar>
        </TopBar>
        <Items>
            <ext:FormPanel runat="server" ID="edita_validacion" MonitorValid="true" Padding="5">
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
                    <ClientValidation Handler="#{btnEditaValidacion}.setDisabled(!valid);" />
                </Listeners>
            </ext:FormPanel>
        </Items>
    </ext:Window>


    <ext:BorderLayout ID="BorderLayout1" runat="server">
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
        <Center>
            <ext:Panel ID="PanelCentral" runat="server">
                <Items>
                    <ext:GridPanel ID="GridProductos" runat="server" StoreID="StoreProductos" Hidden="true">
                        <TopBar>
                            <ext:Toolbar ID="Toolbar3" runat="server">
                                <Items>
                                    <ext:ToolbarFill runat="server" ID="ToolbarFill" />
                                    <ext:Button ID="BtnVerVigencia" runat="server" Text="Editar Vigencias" Icon="NoteEdit">
                                        <DirectEvents>
                                            <Click OnEvent="VerVigencias_Click" />
                                        </DirectEvents>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                        <SelectionModel>
                            <ext:RowSelectionModel SingleSelect="true" />
                        </SelectionModel>
                        <DirectEvents>
                            <Command OnEvent="Click_EditarProducto" />
                        </DirectEvents>
                    </ext:GridPanel>
                    <ext:FormPanel ID="panelRangos" runat="server" Title="Configuraciones de Rangos" Hidden="true">
                        <Items>
                            <ext:GridPanel ID="GridRangos" runat="server" StoreID="StoreRangos">
                                <DirectEvents>
                                    <Command OnEvent="Click_EditarRango" />
                                </DirectEvents>
                            </ext:GridPanel>
                        </Items>
                    </ext:FormPanel>
                    <ext:Panel ID="panelReglasMA" runat="server" Title="Configuraciones de Reglas" Hidden="true">
                        <Items>
                            <ext:GridPanel ID="GridReglasMA" runat="server">
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
                                    <ext:RowSelectionModel SingleSelect="true" />
                                </SelectionModel>
                                <Buttons>
                                    <ext:Button ID="btnEditarReglas" runat="server" Text="Guardar Cambios" Icon="Disk">
                                        <DirectEvents>
                                            <Click OnEvent="btnEditarReglas_Click">
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
                                    Height="500" Layout="FitLayout" StripeRows="true" >
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
                                        <ext:Column DataIndex="Valor" Header="Valor" Width="250">
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
                                <ColumnModel ID="ColumnModel3" runat="server">
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
                                <ColumnModel ID="ColumnModel9" runat="server">
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
                            <ext:TreeGrid ID="GridVMA" runat="server" AutoExpandColumn="Validacion" Hidden="true">
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
                                </Listeners>
                            </ext:TreeGrid>
                        </Items>
                    </ext:Panel>
                </Items>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
