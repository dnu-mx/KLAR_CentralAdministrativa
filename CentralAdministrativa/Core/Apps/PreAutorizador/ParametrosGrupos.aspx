<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ParametrosGrupos.aspx.cs" Inherits="PreAutorizador.ParametrosGrupos" %>

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
        
        var fullName = function (value, metadata, record, rowIndex, colIndex, store) {
            return "<b>" + record.data.Nombre + "</b>";
        };

        var Confirm = function (grupo, grid) {
            var title = 'Confirmación de Asignación';
            var msg = 'Se asignará(n)  <b>[' + grid.getSelectionModel().selections.length + ']</b>  tarjeta(s) al Grupo <b>['
                + grupo.getValue() + '</b>]';
            var sm = grid.getSelectionModel(), selected = sm.selections;

            if (grid.getSelectionModel().selections.length == 0) {
                Ext.Msg.alert('Asignación de Tarjetas', 'Por favor, selecciona al menos una tarjeta.');
                return false;
            }

            for (var i = 0, len = selected.length; i < len; i++) {
                if (selected.items[i].data.ID_Plantilla != -1) {
                    msg += ', y alguna(s) ya está(n) asignada(s) a otro Grupo. <br /><br />¿Confirmas la asignación?';
                    i = len;
                }
            }
            
            Ext.Msg.confirm(title, msg, function (btn) {
                if (btn == 'yes') {
                    Ext.net.Mask.show({ msg: 'Procesando...' });
                    ParamPreautPlantilla.AsignarTarjetas();
                    return true;
                } else {
                    return false;
                }
            });
        }

        var confirmaMasivo = function (grupo, grid) {
            var title = 'Confirmación de Asignación';
            var total = grid.getStore().getTotalCount();
            var msg = 'Se asignará(n)  <b>[' + total + ']</b>  tarjeta(s) al Grupo <b>[' + grupo.getValue() +
                '</b>]. Algunas podrían estar asignadas a otro Grupo.<br /><br />¿Confirmas la asignación?';

            Ext.Msg.confirm(title, msg, function (btn) {
                if (btn == 'yes') {
                    Ext.net.Mask.show({ msg: 'Procesando...' });
                    ParamPreautPlantilla.AsignaTarjetasMasivo();
                    return true;
                } else {
                    return false;
                }
            });
        }

        var hideMenuItems = function(grid){
            var ms = grid.view.hmenu.items;
            ms.get("columns").hide(true);
        }
    </script>
    <style type="text/css">
        .x-grid3-row-body p {
            margin: 3px 3px 7px 3px !important;
            width: 99%;
            color: black;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Hidden ID="hdnIdGrupo" runat="server" />
    <ext:Hidden ID="hdnGrupo" runat="server" />
    <ext:Store ID="StoreSubEmisores" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_Colectiva">
                <Fields>
                    <ext:RecordField Name="ID_Colectiva" />
                    <ext:RecordField Name="ClaveColectiva" />
                    <ext:RecordField Name="NombreORazonSocial" />
                </Fields>
            </ext:JsonReader>
        </Reader>
        <SortInfo Field="NombreORazonSocial" Direction="ASC" />
    </ext:Store>
    <ext:Store ID="StoreProductos" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_Producto">
                <Fields>
                    <ext:RecordField Name="ID_Producto" />
                    <ext:RecordField Name="Clave" />
                    <ext:RecordField Name="Descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Window ID="WdwNuevaPlantillaGpo" runat="server" Title="Nuevo Grupo" Icon="Add" Width="420" Height="230"
        Hidden="true" Modal="true" Resizable="false">
        <Items>
            <ext:FormPanel ID="FormPanelNuevoGpo" runat="server" Padding="5" LabelWidth="100" Border="false"
                Layout="FormLayout" LabelAlign="Right">
                <Items>
                    <ext:ComboBox ID="cBoxColectivaGpo" runat="server" ListWidth="200" Width="280"
                        FieldLabel="SubEmisor   <span style='color:red;'>*   </span>" 
                        StoreID="StoreSubEmisores" DisplayField="NombreORazonSocial" Mode="Local"
                        ValueField="ID_Colectiva" AutoSelect="true" Editable="true" ForceSelection="true" MinChars="1"
                        TypeAhead="true" MatchFieldWidth="false" Name="colNuevoGpo" AllowBlank="false">
                        <DirectEvents>
                            <Select OnEvent="PrestableceProdsNuevoGpo" Before="#{cBoxProdGpo}.clearValue();" />
                        </DirectEvents>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cBoxProdGpo" runat="server" ListWidth="200" Width="280" StoreID="StoreProductos"
                        FieldLabel="Producto   <span style='color:red;'>*   </span>" DisplayField="Descripcion"
                        ValueField="ID_Producto" AllowBlank="false" />
                    <ext:TextField ID="txtClaveGpo" runat="server" FieldLabel="Clave   <span style='color:red;'>*   </span>"
                        AllowBlank="false" Width="280" MaxLength="10" />
                    <ext:TextArea ID="txtDescGpo" runat="server" FieldLabel="Descripción   <span style='color:red;'>*   </span>"
                        MaxLength="200" Width="280" Height="70" AllowBlank="false" />
                </Items>
                <Buttons>
                    <ext:Button runat="server" Text="Cancelar" Icon="Cancel">
                        <Listeners>
                            <Click Handler="#{WdwNuevaPlantillaGpo}.hide();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button ID="btnNuevoGpo" runat="server" Text="Aceptar" Icon="Tick">
                        <DirectEvents>
                            <Click OnEvent="btnNuevoGpo_Click" Before="var valid= #{FormPanelNuevoGpo}.getForm().isValid(); if (!valid) {} return valid;">
                                <EventMask ShowMask="true" Msg="Generando Nuevo Grupo..." MinDelay="500" />
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:Window ID="WdwValorParametro" runat="server" Title="Editar Valor Parámetro" Width="420" AutoHeight="true" 
        Hidden="true" Resizable="false">
        <Items>
            <ext:FormPanel ID="FormPanelValorParamTxt" runat="server" Padding="10" MonitorValid="true" LabelAlign="Left"
                LabelWidth="80">
                <Items>
                    <ext:TextField ID="txtParametro" runat="server" FieldLabel="Descripción" Width="300"
                        AllowBlank="false" Selectable="false" ReadOnly="true" />
                    <ext:TextField ID="txtValorParFloat" runat="server" FieldLabel="Valor" Width="300" MaxLength="50"
                        MaskRe="[0-9\.]" Hidden="true"/>
                    <ext:TextField ID="txtValorParInt" runat="server" FieldLabel="Valor" Width="300" MaxLength="50"
                        MaskRe="[0-9]" Hidden="true"/>
                    <ext:TextArea ID="txtValorParString" runat="server" FieldLabel="Valor" Width="300" AutoHeight="true"
                        MaxLength="5000" Hidden="true" />
                    <ext:ComboBox ID="cBoxValorPar" runat="server" FieldLabel="Valor" Width="300" MaxLength="1000"
                        Hidden="true">
                        <Items>
                            <ext:ListItem Text="Sí" Value="true" />
                            <ext:ListItem Text="No" Value="false" />
                        </Items>
                    </ext:ComboBox>
                    <ext:Label ID="lblInstruc" runat="server" LabelSeparator=" " Width="300" StyleSpec="text-align:left" />
                </Items>
                <Buttons>
                    <ext:Button runat="server" Text="Cancelar" Icon="Cancel">
                        <Listeners>
                            <Click Handler="#{WdwValorParametro}.hide();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button runat="server" Text="Guardar Cambio" Icon="Disk">
                        <DirectEvents>
                            <Click OnEvent="btnGuardarValorParametro_Click"/>
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:Window ID="WdwCargaMasiva" runat="server" Title="Asignar Tarjetas por Archivo" Icon="PageWhiteExcel" Width="420"
        AutoHeight="true" Hidden="true" Modal="true" Resizable="false" Closable="true">
        <Items>
            <ext:GridPanel ID="GridTarjetasArchivo" runat="server" Border="false" Header="false" AutoScroll="true"
                Layout="FitLayout" Height="350">
                <TopBar>
                    <ext:Toolbar runat="server">
                        <Items>
                            <ext:FileUploadField ID="fufCargaMasiva" runat="server" ButtonText="Examinar..." Icon="Magnifier" Width="300" />
                            <ext:ToolbarFill runat="server" />
                            <ext:Button ID="btnCargarArchivo" runat="server" Text="Cargar Archivo" Icon="PageWhitePut">
                                <DirectEvents>
                                    <Click OnEvent="btnCargarArchivo_Click" IsUpload="true" Before="#{GridTarjetasArchivo}.getStore().removeAll();"
                                        After="#{fufCargaMasiva}.reset();">
                                        <EventMask ShowMask="true" Msg="Cargando Archivo..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <Store>
                    <ext:Store ID="StoreTarjArchivo" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_MA">
                                <Fields>
                                    <ext:RecordField Name="ID_MA" />
                                    <ext:RecordField Name="IdPlantilla" />
                                    <ext:RecordField Name="Plantilla" />
                                    <ext:RecordField Name="Tarjeta" />
                                    <ext:RecordField Name="Nombre" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <ColumnModel runat="server">
                    <Columns>
                        <ext:Column DataIndex="ID_MA" Hidden="true" />
                        <ext:Column DataIndex="IdPlantilla" Hidden="true" />
                        <ext:Column DataIndex="Tarjeta" Header="Número de Tarjeta" Width="150"/>
                        <ext:Column DataIndex="Plantilla" Header="Grupo" Width="120"/>
                        <ext:Column DataIndex="Nombre" Header="Nombre" />
                    </Columns>
                </ColumnModel>
                <SelectionModel>
                    <ext:RowSelectionModel runat="server" />
                </SelectionModel>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingAsignMasiva" runat="server" StoreID="StoreTarjArchivo" DisplayInfo="true"
                        DisplayMsg="Mostrando Tarjetas {0} - {1} de {2}" HideRefresh="true" />
                </BottomBar>
                <Listeners>
                    <Render Fn="hideMenuItems" />
                </Listeners>
                <Plugins>
                    <ext:GridFilters runat="server" Local="true" FiltersText="Filtros">
                        <Filters>
                            <ext:StringFilter DataIndex="Tarjeta" />
                            <ext:StringFilter DataIndex="Grupo" />
                            <ext:StringFilter DataIndex="Nombre" />
                        </Filters>
                    </ext:GridFilters>
                </Plugins>
            </ext:GridPanel>
        </Items>
        <Buttons>
            <ext:Button ID="btnAsignarMasivo" runat="server" Text="Asignar Tarjetas" Icon="Tick"
                Disabled="true">
                <Listeners>
                    <Click Handler="return confirmaMasivo(#{hdnGrupo}, #{GridTarjetasArchivo});" />
                </Listeners>
            </ext:Button>
            <ext:Button runat="server" Text="Cancelar" Icon="Cancel">
                <Listeners>
                    <Click Handler="#{WdwCargaMasiva}.hide();" />
                </Listeners>
            </ext:Button>
        </Buttons>
    </ext:Window>
    <ext:BorderLayout runat="server">
        <West Split="true">
            <ext:Panel runat="server" Width="335" Border="false" Layout="FitLayout" Title="Consulta de Grupos">
                <Content>
                    <ext:BorderLayout runat="server">
                        <South Split="true">
                            <ext:FormPanel runat="server" Height="25" Border="false">
                                <Items>
                                    <ext:Toolbar runat="server">
                                        <Items>
                                            <ext:Button ID="btnLimpiarIzq" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                                <DirectEvents>
                                                    <Click OnEvent="btnLimpiarIzq_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:ToolbarFill runat="server" />
                                            <ext:Button runat="server" Icon="Add" ToolTip="Crear Nuevo Grupo"
                                                Text="Nuevo Grupo">
                                                <Listeners>
                                                     <Click Handler="#{FormPanelNuevoGpo}.reset(); #{WdwNuevaPlantillaGpo}.show();" />
                                                </Listeners>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </Items>
                            </ext:FormPanel>
                        </South>
                        <Center Split="true">
                            <ext:GridPanel ID="GridResultGposParab" runat="server" AutoExpandColumn="Descripcion" 
                                Border="false" Header="false">
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar2" runat="server">
                                        <Items>
                                            <ext:Hidden ID="hdnIdColectiva" runat="server" />
                                            <ext:ComboBox ID="cBoxSubEmisor" runat="server" EmptyText="SubEmisor" ListWidth="200"
                                                Width="120" StoreID="StoreSubEmisores" DisplayField="NombreORazonSocial" Mode="Local"
                                                ValueField="ID_Colectiva" AutoSelect="true" Editable="true" ForceSelection="true" MinChars="1"
                                                TypeAhead="true" MatchFieldWidth="false" Name="colSubEmisor" AllowBlank="false">
                                                <DirectEvents>
                                                    <Select OnEvent="PrestableceProductos" Before="#{hdnIdColectiva}.clear();
                                                        #{hdnIdColectiva}.setValue(this.getValue()); #{cBoxProducto}.clearValue();" />
                                                </DirectEvents>
                                            </ext:ComboBox>
                                            <ext:ComboBox ID="cBoxProducto" runat="server" EmptyText="Producto" ListWidth="200"
                                                Width="120" StoreID="StoreProductos" DisplayField="Descripcion"
                                                ValueField="ID_Producto" AllowBlank="false" />
                                            <ext:Button ID="btnBuscarGrupo" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnBuscarGrupo_Click" Before="if (!#{cBoxSubEmisor}.isValid() ||
                                                        !#{cBoxProducto}.isValid()) { return false; } else { 
                                                        #{GridResultGposParab}.getStore().removeAll();
                                                        #{PanelCentralGpo}.setTitle('_'); #{PanelCentralGpo}.setDisabled(true); }">
                                                        <EventMask ShowMask="true" Msg="Buscando Grupos..." MinDelay="500" />
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <Store>
                                    <ext:Store ID="StoreGrupos" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Plantilla">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Plantilla" />
                                                    <ext:RecordField Name="Clave" />
                                                    <ext:RecordField Name="Descripcion" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <ColumnModel runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_Grupo" Hidden="true" />
                                        <ext:Column DataIndex="Clave" Header="Clave" Width="90" />
                                        <ext:Column DataIndex="Descripcion" Header="Grupo" Width="110" />
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel SingleSelect="true" />
                                </SelectionModel>
                                <DirectEvents>
                                    <RowClick OnEvent="selectRowResultadosSP_Event">
                                        <EventMask ShowMask="true" Msg="Obteniendo Información del Grupo..." MinDelay="500" />
                                        <ExtraParams>
                                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridResultGposParab}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                        </ExtraParams>
                                    </RowClick>
                                </DirectEvents>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreGrupos" DisplayInfo="true"
                                        DisplayMsg="{0} - {1} de {2}" HideRefresh="true" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </West>
        <Center Split="true">
            <ext:Panel ID="PanelCentralGpo" runat="server" Height="250" Border="false" Title="_" Disabled="true">
                <Items>
                    <ext:BorderLayout runat="server">
                        <Center>
                            <ext:TabPanel runat="server">
                                <Items>
                                    <ext:FormPanel ID="FormPanelParametrosSP" runat="server" Title="Parámetros" Layout="FitLayout" Border="false">
                                        <Items>
                                            <ext:Panel runat="server" Layout="FitLayout" AutoScroll="true" Border="false">
                                                <TopBar>
                                                    <ext:Toolbar runat="server">
                                                        <Items>
                                                            <ext:Hidden ID="hdnIdParametroMA" runat="server" />
                                                            <ext:Hidden ID="hdnIdValorPMA" runat="server" />
                                                            <ext:ComboBox ID="cBoxTipoParametroMA" runat="server" EmptyText="Tipo de Parámetros..." Width="150"
                                                                DisplayField="Descripcion" ValueField="ID_TipoParametroMultiasignacion" AllowBlank="false">
                                                                <Store>
                                                                    <ext:Store ID="StoreTipoParametroMA" runat="server">
                                                                        <Reader>
                                                                            <ext:JsonReader IDProperty="ID_TipoParametroMultiasignacion">
                                                                                <Fields>
                                                                                    <ext:RecordField Name="ID_TipoParametroMultiasignacion" />
                                                                                    <ext:RecordField Name="Clave" />
                                                                                    <ext:RecordField Name="Descripcion" />
                                                                                </Fields>
                                                                            </ext:JsonReader>
                                                                        </Reader>
                                                                    </ext:Store>
                                                                </Store>
                                                                <DirectEvents>
                                                                    <Select OnEvent="SeleccionaTipoPMA" Before="#{cBoxParametros}.setDisabled(false);
                                                                        #{btnAddParametros}.setDisabled(false); #{cBoxParametros}.getStore().removeAll();
                                                                        #{cBoxParametros}.reset(); #{GridPanelParametros}.getStore().removeAll();">
                                                                        <EventMask ShowMask="true" Msg="Obteniendo Parámetros..." MinDelay="200" />
                                                                    </Select>
                                                                </DirectEvents>
                                                            </ext:ComboBox>
                                                            <ext:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                            <ext:ComboBox ID="cBoxParametros" runat="server" EmptyText="Parámetros sin Asignar..." Width="180"
                                                                DisplayField="Nombre" ValueField="ID_ParametroMultiasignacion" Disabled="true" AllowBlank="false">
                                                                <Store>
                                                                    <ext:Store ID="StoreParametros" runat="server">
                                                                        <Reader>
                                                                            <ext:JsonReader IDProperty="ID_ParametroMultiasignacion">
                                                                                <Fields>
                                                                                    <ext:RecordField Name="ID_ParametroMultiasignacion" />
                                                                                    <ext:RecordField Name="Nombre" />
                                                                                    <ext:RecordField Name="Descripcion" />
                                                                                </Fields>
                                                                            </ext:JsonReader>
                                                                        </Reader>
                                                                    </ext:Store>
                                                                </Store>
                                                            </ext:ComboBox>
                                                            <ext:Button ID="btnAddParametros" runat="server" Text="Asignar Parámetro" Icon="Add" Disabled="true">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnAddParametros_Click" Before="var valid= #{cBoxParametros}.isValid(); if (!valid) {} return valid;">
                                                                        <EventMask ShowMask="true" Msg="Asignando Parámetro..." MinDelay="500" />
                                                                    </Click>
                                                                </DirectEvents>
                                                            </ext:Button>
                                                        </Items>
                                                    </ext:Toolbar>
                                                </TopBar>
                                                <Items>
                                                    <ext:GridPanel ID="GridPanelParametros" runat="server" Header="true" Border="false" AutoScroll="true"
                                                        AutoHeight="true" Layout="FitLayout" AutoExpandColumn="Nombre">
                                                        <Store>
                                                            <ext:Store ID="StoreValoresParametros" runat="server">
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="ID_ValorParametroMultiasignacion">
                                                                        <Fields>
                                                                            <ext:RecordField Name="ID_ParametroMultiasignacion" />
                                                                            <ext:RecordField Name="ID_ValorParametroMultiasignacion" />
                                                                            <ext:RecordField Name="Nombre" />
                                                                            <ext:RecordField Name="Descripcion" />
                                                                            <ext:RecordField Name="Valor" />
                                                                            <ext:RecordField Name="TipoDato" />
                                                                            <ext:RecordField Name="Instrucciones" />
                                                                        </Fields>
                                                                    </ext:JsonReader>
                                                                </Reader>
                                                            </ext:Store>
                                                        </Store>
                                                        <ColumnModel runat="server">
                                                            <Columns>
                                                                <ext:Column ColumnID="ID_Parametro" runat="server" Hidden="true" DataIndex="ID_Parametro" />
                                                                <ext:Column ColumnID="Nombre" Header="Parámetro" Width="350" DataIndex="Nombre">
                                                                    <Renderer Fn="fullName" />
                                                                    <Editor>
                                                                        <ext:DisplayField runat="server" StyleSpec="font-weight:bold;font-family:segoe ui;font-size: 12px;" />
                                                                    </Editor>
                                                                </ext:Column>
                                                                <ext:Column ColumnID="Valor" Header="Valor" Sortable="true" DataIndex="Valor" Width="140" />
                                                                <ext:Column runat="server" Hidden="true" DataIndex="TipoDato" />
                                                                <ext:Column runat="server" Hidden="true" DataIndex="Instrucciones" />
                                                                <ext:CommandColumn Header="Acciones" Width="80" >
                                                                    <Commands>
                                                                        <ext:GridCommand Icon="Pencil" CommandName="Edit">
                                                                            <ToolTip Text="Editar Valor" />
                                                                        </ext:GridCommand>
                                                                        <ext:GridCommand Icon="Delete" CommandName="Delete">
                                                                            <ToolTip Text="Quitar Parámetro al Grupo" />
                                                                        </ext:GridCommand>
                                                                    </Commands>
                                                                </ext:CommandColumn>
                                                            </Columns>
                                                        </ColumnModel>
                                                        <View>
                                                            <ext:GridView runat="server" EnableRowBody="true">
                                                                <GetRowClass Handler="rowParams.body = '<p>' + record.data.Descripcion + '</p>'; return 'x-grid3-row-expanded';" />
                                                            </ext:GridView>
                                                        </View>
                                                        <SelectionModel>
                                                            <ext:RowSelectionModel runat="server" SingleSelect="true" />
                                                        </SelectionModel>
                                                        <DirectEvents>
                                                            <Command OnEvent="EjecutarComandoParametros">
                                                                <Confirmation BeforeConfirm="if (command == 'Edit') return false;" 
                                                                    ConfirmRequest="true" Title="Confirmación" Message="¿Estás seguro de quitar el parámetro a la colectiva?" />
                                                                <ExtraParams>
                                                                    <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                                                    <ext:Parameter Name="Values" Value="Ext.encode(record.data)" Mode="Raw" />
                                                                </ExtraParams>
                                                            </Command>
                                                        </DirectEvents>
                                                        <Listeners>
                                                            <Render Fn="hideMenuItems" />
                                                        </Listeners>
                                                        <Plugins>
                                                            <ext:GridFilters runat="server" Local="true" FiltersText="Filtros">
                                                                <Filters>
                                                                    <ext:StringFilter DataIndex="Nombre" />
                                                                </Filters>
                                                            </ext:GridFilters>
                                                        </Plugins>
                                                    </ext:GridPanel>
                                                </Items>
                                            </ext:Panel>
                                        </Items>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelVerTarjetas" runat="server" Title="Ver Tarjetas" Layout="FitLayout">
                                        <Items>
                                            <ext:GridPanel ID="GridTarjetas" runat="server" Header="true" Layout="FitLayout" Border="false" AutoExpandColumn="Nombre">
                                                <TopBar>
                                                    <ext:Toolbar runat="server">
                                                        <Items>
                                                            <ext:TextField ID="txtFiltroTarjeta" runat="server" Width="200" EmptyText="Ingresa el Número de Tarjeta"
                                                                MaskRe="[0-9]" MinLength="16" MaxLength="16" AllowBlank="false"/>
                                                            <ext:ToolbarSeparator runat="server" />
                                                            <ext:Button ID="btnFiltrar" runat="server" Text="Filtrar" Icon="Drink">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnFiltrar_Click" Timeout="360000"
                                                                        Before="if (!#{txtFiltroTarjeta}.isValid()) { return false; } else {
                                                                        resetToolbar(#{PagingTB_TarjetasGpo}); #{GridTarjetas}.getStore().sortInfo = null; }" />
                                                                </DirectEvents>
                                                            </ext:Button>
                                                            <ext:Button ID="btnFiltrarHide" runat="server" Hidden="true">
                                                                <Listeners>
                                                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Tarjetas...' });
                                                                        #{GridTarjetas}.getStore().reload({params:{start:0, sort:('','')}});" />
                                                                </Listeners>
                                                            </ext:Button>
                                                            <ext:ToolbarFill runat="server" />
                                                            <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                                                <DirectEvents>
                                                                    <Click OnEvent="Download" IsUpload="true"
                                                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                                        AdminGpo.StopMask();" />
                                                                </DirectEvents>
                                                            </ext:Button>
                                                            <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="true">
                                                                <DirectEvents>
                                                                    <Click OnEvent="Download" IsUpload="true"
                                                                        After="Ext.net.Mask.show({ msg : 'Exportando Tarjetas a Excel...' }); e.stopEvent();
                                                                        AdminGpo.StopMask();" />
                                                                </DirectEvents>
                                                            </ext:Button>
                                                        </Items>
                                                    </ext:Toolbar>
                                                </TopBar>
                                                <Store>
                                                    <ext:Store ID="StoreTarjetas" runat="server" RemoteSort="true" OnRefreshData="StoreTarjetas_RefreshData"
                                                        AutoLoad="false">
                                                        <AutoLoadParams>
                                                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                                        </AutoLoadParams>
                                                        <Proxy>
                                                            <ext:PageProxy />
                                                        </Proxy>
                                                        <DirectEventConfig IsUpload="true" />
                                                        <Reader>
                                                            <ext:JsonReader IDProperty="ID_MA">
                                                                <Fields>
                                                                    <ext:RecordField Name="ID_MA" />
                                                                    <ext:RecordField Name="ClaveMA" />
                                                                    <ext:RecordField Name="Nombre" />
                                                                </Fields>
                                                            </ext:JsonReader>
                                                        </Reader>
                                                        <DirectEventConfig IsUpload="true" />
                                                    </ext:Store>
                                                </Store>
                                                <ColumnModel runat="server">
                                                    <Columns>
                                                        <ext:Column DataIndex="ID_Tarjeta" Hidden="true" />
                                                        <ext:Column DataIndex="ClaveMA" Header="Número de Tarjeta" Width="200" />
                                                        <ext:Column DataIndex="Nombre" Header="Nombre" Width="350" />
                                                    </Columns>
                                                </ColumnModel>
                                                <SelectionModel>
                                                    <ext:RowSelectionModel SingleSelect="true" />
                                                </SelectionModel>
                                                <BottomBar>
                                                    <ext:PagingToolbar ID="PagingTB_TarjetasGpo" runat="server" StoreID="StoreTarjetas" DisplayInfo="true"
                                                        DisplayMsg="Mostrando Tarjetas {0} - {1} de {2}" HideRefresh="true" />
                                                </BottomBar>
                                            </ext:GridPanel>
                                        </Items>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelAT" runat="server" Title="Asignar Tarjetas" Layout="FitLayout">
                                        <Items>
                                            <ext:GridPanel ID="GridTarjetasPA" runat="server" Header="true" Layout="FitLayout" Border="false" AutoExpandColumn="Nombre">
                                                <TopBar>
                                                    <ext:Toolbar runat="server">
                                                        <Items>
                                                            <ext:TextField ID="txtTarjetaPA" runat="server" Width="200" EmptyText="Ingresa el Número de Tarjeta"
                                                                MaskRe="[0-9]" MinLength="16" MaxLength="16" AllowBlank="false"/>
                                                            <ext:ToolbarSeparator runat="server" />
                                                            <ext:Button ID="btnFiltrarPA" runat="server" Text="Filtrar" Icon="Drink">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnFiltrarPA_Click" Timeout="360000"
                                                                        Before="if (!#{txtTarjetaPA}.isValid()) { return false; } else {
                                                                        resetToolbar(#{PagingTB_TarjetasPA}); #{GridTarjetasPA}.getStore().sortInfo = null; }" />
                                                                </DirectEvents>
                                                            </ext:Button>
                                                            <ext:Button ID="btnFiltrarHidePA" runat="server" Hidden="true">
                                                                 <Listeners>
                                                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Tarjetas...' });
                                                                        #{GridTarjetasPA}.getStore().reload({params:{start:0, sort:('','')}});" />
                                                                </Listeners>
                                                            </ext:Button>
                                                            <ext:ToolbarFill runat="server" />
                                                           <ext:Button ID="btnAsignarTarjetas" runat="server" Text="Asignar Seleccionada(s)" Icon="Tick"
                                                               ToolTip="Asignar Tarjetas Seleccionadas" Disabled="true">
                                                                <Listeners>
                                                                    <Click Handler="return Confirm(#{hdnGrupo}, #{GridTarjetasPA});" />
                                                                </Listeners>
                                                            </ext:Button>
                                                            <ext:ToolbarSeparator runat="server" />
                                                            <ext:Button runat="server" Text="Asignar por Archivo..." Icon="PageWhiteExcel"
                                                                ToolTip="Asignar Tarjetas a través de un Archivo">
                                                                <Listeners>
                                                                    <Click Handler="#{GridTarjetasArchivo}.getStore().removeAll(); 
                                                                        #{btnAsignarMasivo}.setDisabled(true); #{WdwCargaMasiva}.show();" />
                                                                </Listeners>
                                                            </ext:Button>
                                                        </Items>
                                                    </ext:Toolbar>
                                                </TopBar>
                                                <Store>
                                                    <ext:Store ID="StoreTarjetasPA" runat="server" RemoteSort="true" OnRefreshData="StoreTarjetasPA_RefreshData"
                                                        AutoLoad="false">
                                                        <AutoLoadParams>
                                                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                                        </AutoLoadParams>
                                                        <Proxy>
                                                            <ext:PageProxy />
                                                        </Proxy>
                                                        <DirectEventConfig IsUpload="true" />
                                                        <Reader>
                                                            <ext:JsonReader IDProperty="ID_Tarjeta">
                                                                <Fields>
                                                                    <ext:RecordField Name="ID_Tarjeta" />
                                                                    <ext:RecordField Name="ID_Plantilla" />
                                                                    <ext:RecordField Name="Plantilla" />
                                                                    <ext:RecordField Name="Tarjeta" />
                                                                    <ext:RecordField Name="Nombre" />
                                                                </Fields>
                                                            </ext:JsonReader>
                                                        </Reader>
                                                        <DirectEventConfig IsUpload="true" />
                                                    </ext:Store>
                                                </Store>
                                                <ColumnModel runat="server">
                                                    <Columns>
                                                        <ext:Column DataIndex="ID_Tarjeta" Hidden="true" />
                                                        <ext:Column DataIndex="ID_Plantilla" Hidden="true" />
                                                        <ext:Column DataIndex="Plantilla" Header="Grupo" />
                                                        <ext:Column DataIndex="Tarjeta" Header="Número de Tarjeta" Width="200" />
                                                        <ext:Column DataIndex="Nombre" Header="Nombre" Width="350" />
                                                    </Columns>
                                                </ColumnModel>
                                                <SelectionModel>
                                                    <ext:CheckboxSelectionModel runat="server" />
                                                </SelectionModel>
                                                <BottomBar>
                                                    <ext:PagingToolbar ID="PagingTB_TarjetasPA" runat="server" StoreID="StoreTarjetasPA" DisplayInfo="true"
                                                        DisplayMsg="Mostrando Tarjetas {0} - {1} de {2}" HideRefresh="true" />
                                                </BottomBar>
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
