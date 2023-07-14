<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ParametrosDefault.aspx.cs" Inherits="PreAutorizador.ParametrosDefault" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var fullName = function (value, metadata, record, rowIndex, colIndex, store) {
            return "<b>" + record.data.Nombre + "</b>";
        };

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
    <ext:Hidden ID="hdnIdProducto" runat="server" />
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
    <ext:Window ID="WdwValorParametro" runat="server" Title="Editar Valor Parámetro" Width="420" AutoHeight="true" Hidden="true"
        Resizable="false">
        <Items>
            <ext:FormPanel ID="FormPanelValorParamTxt" runat="server" Padding="10" MonitorValid="true" LabelAlign="Left" LabelWidth="70">
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
    <ext:BorderLayout runat="server">
        <West Split="true">
            <ext:Panel runat="server" Width="335" Border="false" Layout="FitLayout" Title="Consulta de Productos">
                <Content>
                    <ext:BorderLayout runat="server">
                        <South Split="true">
                            <ext:FormPanel runat="server" Height="25" Border="false">
                                <Items>
                                    <ext:Toolbar runat="server">
                                        <Items>
                                            <ext:ToolbarFill runat="server" />
                                            <ext:Button ID="btnLimpiarIzq" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                                <DirectEvents>
                                                    <Click OnEvent="btnLimpiarIzq_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </Items>
                            </ext:FormPanel>
                        </South>
                        <Center Split="true">
                            <ext:GridPanel ID="GridResultadosProdsParab" runat="server" AutoExpandColumn="Descripcion" 
                                Border="false" Header="false">
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar2" runat="server">
                                        <Items>
                                            <ext:ComboBox ID="cBoxSubEmisor" runat="server" EmptyText="SubEmisor" Width="120"
                                                StoreID="StoreSubEmisores" DisplayField="NombreORazonSocial" ValueField="ID_Colectiva"
                                                Mode="Local" AutoSelect="true" Editable="true" ForceSelection="true" TypeAhead="true"
                                                MinChars="1" MatchFieldWidth="false" Name="colSubEmisor" ListWidth="200" />
                                            <ext:TextField ID="txtProducto" runat="server" EmptyText="Clave/Descripción Producto" />
                                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnBuscar_Click" Before="if (!#{cBoxSubEmisor}.getValue() &&
                                                        !#{txtProducto}.getValue()) { Ext.Msg.alert('Búsqueda', 'Ingresa al menos un filtro.');
                                                        return false; } else { #{GridResultadosProdsParab}.getStore().removeAll(); 
                                                        #{PanelCentralProds}.setTitle('_'); #{PanelCentralProds}.setDisabled(true); }">
                                                        <EventMask ShowMask="true" Msg="Buscando Productos..." MinDelay="500" />
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <Store>
                                    <ext:Store ID="StoreProductos" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Producto">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Producto" />
                                                    <ext:RecordField Name="Clave" />
                                                    <ext:RecordField Name="Descripcion" />
                                                    <ext:RecordField Name="ID_TipoProducto" />
                                                    <ext:RecordField Name="ID_Colectiva" />
                                                    <ext:RecordField Name="Colectiva" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <ColumnModel runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_Producto" Hidden="true" />
                                        <ext:Column DataIndex="Clave" Header="Clave" Width="90" />
                                        <ext:Column DataIndex="Descripcion" Header="Producto" Width="110" />
                                        <ext:Column DataIndex="ID_TipoProducto" Hidden="true" />
                                        <ext:Column DataIndex="ID_Colectiva" Hidden="true" />
                                        <ext:Column DataIndex="Colectiva" Hidden="true" />
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel SingleSelect="true" />
                                </SelectionModel>
                                <DirectEvents>
                                    <RowClick OnEvent="selectRowResultadosPP_Event">
                                        <EventMask ShowMask="true" Msg="Obteniendo Información del Producto..." MinDelay="500" />
                                        <ExtraParams>
                                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridResultadosProdsParab}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                        </ExtraParams>
                                    </RowClick>
                                </DirectEvents>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreProductos" DisplayInfo="true"
                                        DisplayMsg="{0} - {1} de {2}" HideRefresh="true" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </West>
        <Center Split="true">
            <ext:Panel ID="PanelCentralProds" runat="server" Height="250" Border="false" Title="_" Disabled="true">
                <Items>
                    <ext:BorderLayout runat="server">
                        <Center>
                            <ext:TabPanel runat="server">
                                <Items>
                                    <ext:FormPanel ID="FormPanelParametros" runat="server" Title="Parámetros" Layout="FitLayout" Border="false">
                                        <Items>
                                            <ext:Panel runat="server" Layout="FitLayout" AutoScroll="true" Border="false">
                                                <TopBar>
                                                    <ext:Toolbar runat="server">
                                                        <Items>
                                                            <ext:Hidden ID="hdnIdParametroMA" runat="server" />
                                                            <ext:Hidden ID="hdnIdPlantilla" runat="server" />
                                                            <ext:Hidden ID="hdnIdValorPMA" runat="server" />
                                                            <ext:ComboBox ID="cBoxTipoParametroMA" runat="server" EmptyText="Tipo de Parámetros..." Width="150"
                                                                DisplayField="Descripcion" ValueField="ID_TipoParametroMultiasignacion" AllowBlank="false"
                                                                ListWidth="200">
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
                                                                    <Select OnEvent="SeleccionaTipoParamMA" Before="#{GridPanelParametros}.getStore().removeAll();">
                                                                        <EventMask ShowMask="true" Msg="Obteniendo Parámetros..." MinDelay="200" />
                                                                    </Select>
                                                                </DirectEvents>
                                                            </ext:ComboBox>
                                                            <ext:ToolbarFill runat="server" />
                                                        </Items>
                                                    </ext:Toolbar>
                                                </TopBar>
                                                <Items>
                                                    <ext:GridPanel ID="GridPanelParametros" runat="server" Header="true" Border="false" AutoScroll="true"
                                                        AutoHeight="true" Layout="FitLayout" AutoExpandColumn="Nombre">
                                                        <Store>
                                                            <ext:Store ID="StoreValoresParametros" runat="server">
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="ID_ParametroMultiasignacion">
                                                                        <Fields>
                                                                            <ext:RecordField Name="ID_ParametroMultiasignacion" />
                                                                            <ext:RecordField Name="ID_Plantilla" />
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
                                                                <ext:Column runat="server" Hidden="true" DataIndex="ID_ParametroMultiasignacion" />
                                                                <ext:Column runat="server" Hidden="true" DataIndex="ID_Plantilla" />
                                                                <ext:Column runat="server" Hidden="true" DataIndex="ID_ValorParametroMultiasignacion" />
                                                                <ext:Column Header="Parámetro" Width="370" DataIndex="Nombre">
                                                                    <Renderer Fn="fullName" />
                                                                    <Editor>
                                                                        <ext:DisplayField runat="server" StyleSpec="font-weight:bold;font-family:segoe ui;font-size: 12px;" />
                                                                    </Editor>
                                                                </ext:Column>
                                                                <ext:Column Header="Valor" Sortable="true" DataIndex="Valor" Width="150" />
                                                                <ext:Column runat="server" Hidden="true" DataIndex="TipoDato" />
                                                                <ext:Column runat="server" Hidden="true" DataIndex="Instrucciones" />
                                                                <ext:CommandColumn Width="80" >
                                                                    <Commands>
                                                                        <ext:GridCommand Icon="Pencil" CommandName="Edit">
                                                                            <ToolTip Text="Editar Valor" />
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
                                </Items>
                            </ext:TabPanel>
                        </Center>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
