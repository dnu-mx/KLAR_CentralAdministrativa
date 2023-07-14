<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="ParametrosTarjetas.aspx.cs" Inherits="PreAutorizador.ParametrosTarjetas" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .x-grid3-row-body p {
            margin: 3px 3px 7px 3px !important;
            width: 99%;
            color: black;
        }
    </style>

    <script type="text/javascript">        
        var fullName = function (value, metadata, record, rowIndex, colIndex, store) {
            return "<b>" + record.data.Nombre + "</b>";
        };

        var hideMenuItems = function(grid){
            var ms = grid.view.hmenu.items;
            ms.get("columns").hide(true);
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Hidden ID="hdnIdCuentaLDC" runat="server" />
    <ext:Hidden ID="hdnIdCuentaCC" runat="server" />
    <ext:Hidden ID="hdnIdMA" runat="server" />
    <ext:Hidden ID="hdnIdColectiva" runat="server" />
    <ext:Hidden ID="hdnIdCadena" runat="server" />
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
                            <Click OnEvent="btnGuardarValorParametro_Click" />
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:BorderLayout ID="MainBorderLayout" runat="server">
        <West Split="true">
            <ext:Panel ID="MainPanel" runat="server" Width="350" Border="false">
                <Content>
                    <ext:BorderLayout ID="LeftBorderLayout" runat="server">
                        <North Split="true">
                            <ext:FormPanel ID="FormPanelBusqueda" runat="server" Title="Consulta de Tarjetas" Height="240" Frame="true" LabelWidth="120"
                                Collapsible="true" Border="false">
                                <Items>
                                    <ext:FieldSet ID="FieldSetBusqueda" runat="server" Border="false">
                                        <Items>
                                            <ext:TextField ID="txtNombre" runat="server" FieldLabel="Nombre" MaxLength="30" Width="310" />
                                            <ext:TextField ID="txtApPaterno" runat="server" FieldLabel="Primer Apellido" MaxLength="30" Width="310" />
                                            <ext:TextField ID="txtApMaterno" runat="server" FieldLabel="Segundo Apellido" MaxLength="30" Width="310" />
                                            <ext:TextField ID="txtNumCuenta" runat="server" FieldLabel="Número de Cuenta" MaxLength="20" Width="310" />
                                            <ext:TextField ID="txtNumTarjeta" runat="server" FieldLabel="Número de Tarjeta" MaxLength="16" Width="310"
                                                MaskRe="[0-9]" MinLength="16" />
                                            <ext:Checkbox ID="chkBoxSoloAdicionales" runat="server" FieldLabel="Sólo Adicionales" />
                                        </Items>
                                        <Buttons>
                                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar">
                                                <DirectEvents>
                                                    <Click OnEvent="btnLimpiar_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                                        Before="if (!#{txtNumTarjeta}.isValid()) { return false; }
                                                        if (!#{txtNumCuenta}.getValue() && !#{txtNumTarjeta}.getValue() && !#{txtNombre}.getValue() &&
                                                        !#{txtApPaterno}.getValue() && !#{txtApMaterno}.getValue())
                                                        { Ext.Msg.alert('Consulta de Cuentas', 'Ingresa al menos un criterio de búsqueda'); return false; }
                                                        else { #{GridResultados}.getStore().removeAll(); #{EastPanel}.setTitle('_');
                                                        #{EastPanel}.setDisabled(true); }">
                                                        <EventMask ShowMask="true" Msg="Buscando Cuentas..." MinDelay="500" />
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Buttons>
                                    </ext:FieldSet>
                                </Items>
                            </ext:FormPanel>
                        </North>
                        <Center Split="true">
                            <ext:FormPanel ID="FormPanelResultados" runat="server" Title="Resultados de Tarjetas" Layout="FitLayout" Border="false">
                                <Items>
                                    <ext:GridPanel ID="GridResultados" runat="server" Height="450" AutoDoLayout="true" Border="false">
                                        <Store>
                                            <ext:Store ID="StoreCuentas" runat="server">
                                                <Reader>
                                                    <ext:JsonReader IDProperty="IdTarjeta">
                                                        <Fields>
                                                            <ext:RecordField Name="IdTarjeta" />
                                                            <ext:RecordField Name="Tarjeta" />
                                                            <ext:RecordField Name="NumeroTarjeta" />
                                                            <ext:RecordField Name="IdCadenaComercial" />  
                                                            <ext:RecordField Name="IdColectivaCuentahabiente" />
                                                            <ext:RecordField Name="NombreTarjetahabiente" />
                                                            <ext:RecordField Name="CLDC" />
                                                            <ext:RecordField Name="CCLC" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Store>
                                        <ColumnModel ID="ColumnModel1" runat="server">
                                            <Columns>
                                                <ext:Column DataIndex="IdTarjeta" Hidden="true" />
                                                <ext:Column DataIndex="Tarjeta" Header="Tarjeta" Width="90" />
                                                <ext:Column DataIndex="NumeroTarjeta" Hidden="true" />
                                                <ext:Column DataIndex="IdCadenaComercial" Hidden="true" />
                                                <ext:Column DataIndex="IdColectivaCuentahabiente" Hidden="true" />
                                                <ext:Column DataIndex="NombreTarjetahabiente" Header="Nombre" Width="160" />
                                                <ext:Column DataIndex="CLDC" Header="Cuenta" Width="90" />
                                                <ext:Column DataIndex="CCLC" Hidden="true" />
                                            </Columns>
                                        </ColumnModel>
                                        <SelectionModel>
                                            <ext:RowSelectionModel SingleSelect="true" />
                                        </SelectionModel>
                                        <DirectEvents>
                                            <RowClick OnEvent="selectRowResultados_Event">
                                                <EventMask ShowMask="true" Msg="Obteniendo Información de la Cuenta..." MinDelay="500" />
                                                <ExtraParams>
                                                    <ext:Parameter Name="Values" Value="Ext.encode(#{GridResultados}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                                </ExtraParams>
                                            </RowClick>
                                        </DirectEvents>
                                        <BottomBar>
                                            <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreCuentas" DisplayInfo="true" HideRefresh="true"
                                                DisplayMsg="Mostrando Cuentas {0} - {1} de {2}" />
                                        </BottomBar>
                                    </ext:GridPanel>
                                </Items>
                            </ext:FormPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>       
        </West>
        <Center Split="true">
            <ext:Panel ID="EastPanel" runat="server" Title="_" Disabled="true" Border="false">
                <Items>
                    <ext:BorderLayout ID="BorderLayout1" runat="server">
                        <Center>
                            <ext:TabPanel ID="TabPanel1" runat="server">
                                <Items>
                                    <ext:FormPanel ID="FormPanelParams" runat="server" Title="Editar Parámetros" Layout="FitLayout" Border="false">
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
                                                                                    <ext:RecordField Name="ID_Plantilla" />
                                                                                </Fields>
                                                                            </ext:JsonReader>
                                                                        </Reader>
                                                                    </ext:Store>
                                                                </Store>
                                                                <Listeners>
                                                                    <Select Handler="var id = this.getValue(); var record = this.getStore().getById(id);
                                                                        #{hdnIdPlantilla}.setValue(record.get('ID_Plantilla'));" />
                                                                </Listeners>
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
                                                                            <ext:RecordField Name="ID_Plantilla" />
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
                                                                            <ToolTip Text="Quitar Parámetro a la Cuenta" />
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
                                                        <Plugins>
                                                            <ext:GridFilters runat="server" Local="true" FiltersText="Filtros">
                                                                <Filters>
                                                                    <ext:StringFilter DataIndex="Plantilla" />
                                                                    <ext:StringFilter DataIndex="Nombre" />
                                                                </Filters>
                                                            </ext:GridFilters>
                                                        </Plugins>
                                                        <SelectionModel>
                                                            <ext:RowSelectionModel runat="server" SingleSelect="true" />
                                                        </SelectionModel>
                                                        <Listeners>
                                                            <Render Fn="hideMenuItems" />
                                                        </Listeners>
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
                                                        <LoadMask ShowMask="false" />
                                                    </ext:GridPanel>
                                                </Items>
                                            </ext:Panel>
                                        </Items>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelTodos" runat="server" Title="Consultar Parámetros" Layout="FitLayout" Border="false"
                                        AutoScroll="true">
                                        <Items>
                                            <ext:GridPanel ID="GridPanelTodos" runat="server" Header="true" Border="false" AutoScroll="true"
                                                AutoHeight="true" Layout="FitLayout" AutoExpandColumn="Nombre">
                                                <Store>
                                                    <ext:Store ID="StoreAllParams" runat="server">
                                                        <Reader>
                                                            <ext:JsonReader IDProperty="ID_ParametroMultiasignacion">
                                                                <Fields>
                                                                    <ext:RecordField Name="ID_ParametroMultiasignacion" />
                                                                    <ext:RecordField Name="Nombre" />
                                                                    <ext:RecordField Name="Descripcion" />
                                                                    <ext:RecordField Name="Valor" />
                                                                    <ext:RecordField Name="Plantilla" />
                                                                </Fields>
                                                            </ext:JsonReader>
                                                        </Reader>
                                                    </ext:Store>
                                                </Store>
                                                <ColumnModel runat="server">
                                                    <Columns>
                                                        <ext:Column DataIndex="Nombre" Header="Parámetro" Width="300">
                                                            <Renderer Fn="fullName" />
                                                            <Editor>
                                                                <ext:DisplayField runat="server" StyleSpec="font-weight:bold;font-family:segoe ui;font-size: 12px;" />
                                                            </Editor>
                                                        </ext:Column>
                                                        <ext:Column DataIndex="Valor" Header="Valor" Sortable="true" Width="80" />
                                                        <ext:Column DataIndex="Plantilla" Header="Plantilla" Width="350" />
                                                    </Columns>
                                                </ColumnModel>
                                                <View>
                                                    <ext:GridView runat="server" EnableRowBody="true">
                                                        <GetRowClass Handler="rowParams.body = '<p>' + record.data.Descripcion + '</p>'; return 'x-grid3-row-expanded';" />
                                                    </ext:GridView>
                                                </View>
                                                <Plugins>
                                                    <ext:GridFilters runat="server" Local="true" FiltersText="Filtros">
                                                        <Filters>
                                                            <ext:StringFilter DataIndex="Plantilla" />
                                                            <ext:StringFilter DataIndex="Nombre" />
                                                        </Filters>
                                                    </ext:GridFilters>
                                                </Plugins>
                                                <SelectionModel>
                                                    <ext:RowSelectionModel runat="server" SingleSelect="true" />
                                                </SelectionModel>
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
