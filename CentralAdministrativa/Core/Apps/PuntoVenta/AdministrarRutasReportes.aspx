<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="AdministrarRutasReportes.aspx.cs" Inherits="TpvWeb.AdministrarRutasReportes" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var prepareToolbar = function (grid, toolbar, rowIndex, record) {

            if (record.get("EsActivo") == true) { //Activo
                toolbar.items.get(1).hide(); //botón Rojo
            } else {
                toolbar.items.get(2).hide(); //botón Verde

                if (record.get("Ruta") == null) { //Si falta configuración
                    toolbar.items.get(1).hide(); //botón rojo
                }
            }
        }

        var prepareCat = function (grid, toolbar, rowIndex, record) {
            if (record.get("Activo") == 1) {
                toolbar.items.get(1).hide();
            } else {
                toolbar.items.get(0).hide();
            }
        }

        var submitValue = function (grid, hiddenFormat, format) {
            hiddenFormat.setValue(format);
            grid.submitData(false);
        };

        var fullName = function (value, metadata, record, rowIndex, colIndex, store) {
            return "<b>" + record.data.Nombre + "</b>";
        };

        var afterEdit = function (e) {
            TpvWeb.ActualizaValorParamContrato(e.record.data.ID_ValorContrato, e.record.data.ValorParametro);
        };

        var afterEdit_PE = function (e) {
            TpvWeb.ActualizaValorParamExtra(e.record.data.ID_Parametro, e.record.data.Valor);
        };
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
    <ext:Hidden ID="hdnIdColectiva" runat="server" />
    <ext:Hidden ID="hdnIdParametro" runat="server" />
    <ext:Hidden ID="hdnParametroNombre" runat="server" />
    <ext:Hidden ID="hdnOrigen" runat="server" />
    <ext:Window ID="WdwValorParametroCombo2" runat="server" Title="Editar Valor Parámetro" Width="420" Height="150" Hidden="true"
        Resizable="true">
        <Items>
            <ext:FormPanel runat="server" Padding="10" MonitorValid="true" LabelAlign="Left" LabelWidth="70">
                <Items>
                    <ext:TextField ID="txtParametroCombo" runat="server" FieldLabel="Descripción" Width="300"
                        AllowBlank="false" Selectable="false" ReadOnly="true" />
                    <ext:ComboBox ID="cBoxValorParametro" runat="server" FieldLabel="Valor" Width="300" AllowBlank="false"
                        DisplayField="Descripcion" ValueField="ID_Valor" Editable="false">
                        <Store>
                            <ext:Store ID="StoreValoresPrestablecidos2" runat="server">
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_Valor">
                                        <Fields>
                                            <ext:RecordField Name="ID_Valor" />
                                            <ext:RecordField Name="Clave" />
                                            <ext:RecordField Name="Descripcion" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <Triggers>
                            <ext:FieldTrigger Icon="Clear" Qtip="Borrar" />
                        </Triggers>
                        <Listeners>
                            <TriggerClick Handler="this.removeByValue(this.getValue());this.clearValue();" />
                        </Listeners>
                    </ext:ComboBox>
                </Items>
                <Buttons>
                    <ext:Button runat="server" Text="Cancelar" Icon="Cancel">
                        <Listeners>
                            <Click Handler="#{WdwValorParametroCombo2}.hide();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button runat="server" Text="Guardar Cambio" Icon="Disk">
                        <DirectEvents>
                            <Click OnEvent="btnGuardarValorParametro_Click2" Before="var valid= #{cBoxValorParametro}.isValid(); if (!valid) {} else { #{hdnOrigen}.setValue('CMB'); } return valid;" />
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:Window ID="WdwValorParametroTexto" runat="server" Title="Editar Valor Parámetro" Width="420" Height="270" Hidden="true"
        Resizable="false">
        <Items>
            <ext:FormPanel ID="FormPanelValorParamTxt" runat="server" Padding="10" MonitorValid="true" LabelAlign="Left" LabelWidth="70">
                <Items>
                    <ext:TextField ID="txtParametro" runat="server" FieldLabel="Descripción" Width="300"
                        AllowBlank="false" Selectable="false" ReadOnly="true" />
                    <ext:TextArea ID="txtValorParametro" runat="server" FieldLabel="Valor" Width="300" MaxLength="1000"
                        Height="150" />
                </Items>
                <Buttons>
                    <ext:Button runat="server" Text="Cancelar" Icon="Cancel">
                        <Listeners>
                            <Click Handler="#{WdwValorParametroTexto}.hide();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button runat="server" Text="Guardar Cambio" Icon="Disk">
                        <DirectEvents>
                            <Click OnEvent="btnGuardarValorParametro_Click2" Before="#{hdnOrigen}.setValue('TXT');" />
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:Window ID="wdwEditar" runat="server" Title="Editar Reporte" Hidden="true" Width="350" AutoHeight="true" Modal="true"
        Resizable="false" Closable="true">
        <Items>
            <ext:FormPanel ID="FormPanelEditar" runat="server" Layout="FormLayout" LabelWidth="100" Padding="10">
                <Items>
                    <ext:Hidden ID="hdnIdReporteColectivaConfiguracion" runat="server" />
                    <ext:Hidden ID="hdnIdReporteColectiva" runat="server" />
                    <ext:Hidden ID="hdnEstatus" runat="server" />
                    <ext:TextField ID="txtNombre" runat="server" Width="210" FieldLabel="Reporte" ReadOnly="true" />
                    <ext:TextField ID="txtArchivo" runat="server" Width="210" FieldLabel="Archivo" MaxLength="100" AllowBlank="false" />
                    <ext:TextField ID="txtRuta" runat="server" Width="210" FieldLabel="Ruta" MaxLength="200" AllowBlank="false" />
                    <ext:TimeField ID="tfHoraEjecucion" runat="server" FieldLabel="Hora de Ejecución" Format="HH:mm:ss" Width="210"
                        SelectedTime="00:00:00" Increment="1" AllowBlank="false" MaskRe="[0-9\:]">
                        <Triggers>
                            <ext:FieldTrigger Icon="Clear" HideTrigger="true" />
                        </Triggers>
                        <Listeners>
                            <BeforeQuery Handler="this.triggers[0][ this.getRawValue().toString().length == 0 ? 'hide' : 'show']();" />
                            <TriggerClick Handler="if (index == 0) { this.focus().clearValue(); trigger.hide();}" />
                            <Select Handler="this.triggers[0].show();" />
                        </Listeners>
                    </ext:TimeField>
                    <ext:ComboBox ID="cboxTipoServicio" runat="server" Width="210" FieldLabel="Servicio" ValueField="ID_TipoServicio" DisplayField="Descripcion">
                        <Store>
                            <ext:Store ID="StoreTipoServicio" runat="server">
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_TipoServicio">
                                        <Fields>
                                            <ext:RecordField Name="ID_TipoServicio" />
                                            <ext:RecordField Name="Clave" />
                                            <ext:RecordField Name="Descripcion" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cboxClasificacion" runat="server" Width="210" FieldLabel="Clasificación" ValueField="ID_Clasificacion" DisplayField="Descripcion">
                        <Store>
                            <ext:Store ID="StoreClasificacion" runat="server">
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_Clasificacion">
                                        <Fields>
                                            <ext:RecordField Name="ID_Clasificacion" />
                                            <ext:RecordField Name="Clave" />
                                            <ext:RecordField Name="Descripcion" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                    </ext:ComboBox>
                </Items>
                <Buttons>
                    <ext:Button ID="btnCancelar" runat="server" Text="Cancelar" Icon="Cancel">
                        <Listeners>
                            <Click Handler="#{wdwEditar}.hide();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button ID="btnGuardar" runat="server" Text="Guardar Cambios" Icon="Disk">
                        <DirectEvents>
                            <Click OnEvent="btnGuardar_Click" Before="var valid= #{FormPanelEditar}.getForm().isValid(); if (!valid) {} return valid;" />
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:Store ID="StoreTipoColectiva" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_TipoColectiva">
                <Fields>
                    <ext:RecordField Name="ID_TipoColectiva" />
                    <ext:RecordField Name="Clave" />
                    <ext:RecordField Name="Descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Store ID="StoreColectivas" runat="server">
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
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true">
            <ext:Panel runat="server" Width="325" Border="false" Layout="FitLayout" Title="Consulta">
                <Content>
                    <ext:BorderLayout ID="LeftBorderLayout" runat="server">
                        <South Split="true">
                            <ext:FormPanel ID="FormPanel3" runat="server" Height="25" Border="false">
                                <Items>
                                    <ext:Hidden ID="hdnClaveTipoCol" runat="server" />
                                    <ext:Hidden ID="hdnCatalogos" runat="server" />
                                    <ext:Toolbar runat="server">
                                        <Items>
                                            <ext:Button ID="btnLimpiarIzq" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                                <DirectEvents>
                                                    <Click OnEvent="btnLimpiarIzq_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:ToolbarFill runat="server" />
                                        </Items>
                                    </ext:Toolbar>
                                </Items>
                            </ext:FormPanel>
                        </South>
                        <Center Split="true">
                            <ext:GridPanel ID="GridResultados" runat="server" AutoExpandColumn="NombreORazonSocial"
                                StoreID="StoreColectivas" Border="false" Header="false">
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar2" runat="server">
                                        <Items>
                                            <ext:ComboBox ID="cBoxTipoColec" runat="server" EmptyText="Tipo de Colectiva" Width="120" AllowBlank="false"
                                                StoreID="StoreTipoColectiva" DisplayField="Descripcion" ValueField="ID_TipoColectiva">
                                                <Listeners>
                                                    <Select Handler="var id = this.getValue(); var record = this.getStore().getById(id);
                                                        #{hdnClaveTipoCol}.setValue(record.get('Clave'));" />
                                                </Listeners>
                                            </ext:ComboBox>
                                            <ext:TextField ID="txtColectiva" runat="server" EmptyText="Clave o Razón Social" Width="130" />
                                            <ext:Button ID="btnBuscarColectiva" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnBuscar_Click" Before="#{PanelCentral}.setTitle('');
                                                        #{PanelCentral}.setDisabled(true);
                                                        var valid= #{cBoxTipoColec}.isValid(); if (!valid) {} return valid;">
                                                        <EventMask ShowMask="true" Msg="Buscando Colectivas..." MinDelay="500" />
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <ColumnModel runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_Colectiva" Hidden="true" />
                                        <ext:Column DataIndex="ClaveColectiva" Header="Clave" Width="90" />
                                        <ext:Column DataIndex="NombreORazonSocial" Header="Nombre" Width="110" />
                                        <ext:Column DataIndex="ClaveTipoColectiva" Hidden="true" />
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel SingleSelect="true" />
                                </SelectionModel>
                                <DirectEvents>
                                    <RowClick OnEvent="selectRowResultados_Event">
                                        <EventMask ShowMask="true" Msg="Obteniendo Información de la Colectiva..." MinDelay="500" />
                                        <ExtraParams>
                                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridResultados}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                        </ExtraParams>
                                    </RowClick>
                                </DirectEvents>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreColectivas" DisplayInfo="true"
                                        DisplayMsg="{0} - {1} de {2}" HideRefresh="true" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </West>
        <Center Split="true">
            <ext:Panel ID="PanelCentral" runat="server" Height="250" Border="false" Title="-o-" Disabled="true">
                <Items>
                    <ext:BorderLayout runat="server">
                        <Center>
                            <ext:TabPanel runat="server">
                                <Items>
                                    <ext:FormPanel ID="FormPanelReportes" runat="server" Title="Reportes" Layout="FitLayout" Border="false" AutoScroll="true"
                                        MonitorResize="true">
                                        <Items>
                                            <ext:GridPanel ID="GridPanelReportes" runat="server" Header="true" Border="false" Layout="FitLayout" MonitorWindowResize="true">
                                                <Store>
                                                    <ext:Store ID="StoreValoresReportes" runat="server" AutoLoad="false">
                                                        <Reader>
                                                            <ext:JsonReader IDProperty="ID_Reporte">
                                                                <Fields>
                                                                    <ext:RecordField Name="ID_Reporte" />
                                                                    <ext:RecordField Name="ClaveReporte" />
                                                                    <ext:RecordField Name="Nombre" />
                                                                    <ext:RecordField Name="Ruta" />
                                                                    <ext:RecordField Name="TipoServicio" />
                                                                    <ext:RecordField Name="Clasificacion" />
                                                                    <ext:RecordField Name="Estatus" />
                                                                    <ext:RecordField Name="EsActivo" />
                                                                    <ext:RecordField Name="ID_TipoServicio" />
                                                                    <ext:RecordField Name="ID_Clasificacion" />
                                                                    <ext:RecordField Name="ID_ReporteColectiva" />
                                                                    <ext:RecordField Name="ID_ReporteColectivaConfiguracion" />
                                                                    <ext:RecordField Name="EsActivo" />
                                                                    <ext:RecordField Name="NombreArchivo" />
                                                                    <ext:RecordField Name="HoraEjecucion" />
                                                                </Fields>
                                                            </ext:JsonReader>
                                                        </Reader>
                                                    </ext:Store>
                                                </Store>
                                                <TopBar>
                                                    <ext:Toolbar runat="server">
                                                        <Items>
                                                            <ext:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                            <ext:ComboBox ID="cBoxReportes" runat="server" EmptyText="Reportes sin Asignar..." Width="180"
                                                                DisplayField="Nombre" ValueField="ID_Reporte" Disabled="false" AllowBlank="false">
                                                                <Store>
                                                                    <ext:Store ID="StoreReportes" runat="server">
                                                                        <Reader>
                                                                            <ext:JsonReader IDProperty="ID_Reporte">
                                                                                <Fields>
                                                                                    <ext:RecordField Name="ID_Reporte" />
                                                                                    <ext:RecordField Name="Nombre" />
                                                                                </Fields>
                                                                            </ext:JsonReader>
                                                                        </Reader>
                                                                    </ext:Store>
                                                                </Store>
                                                            </ext:ComboBox>
                                                            <ext:Button ID="btnAddReporte" runat="server" Text="Asignar Reporte" Icon="Add" Disabled="false">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnAddReportes_Click" Before="var valid= #{cBoxReportes}.isValid(); if (!valid) {} return valid;">
                                                                        <EventMask ShowMask="true" Msg="Asignando Reporte..." MinDelay="500" />
                                                                    </Click>
                                                                </DirectEvents>
                                                            </ext:Button>
                                                        </Items>
                                                    </ext:Toolbar>
                                                </TopBar>
                                                <ColumnModel runat="server">
                                                    <Columns>
                                                        <ext:Column ColumnID="ID_Reporte" DataIndex="ID_Reporte" Header="ID" Width="50" />
                                                        <ext:Column ColumnID="Nombre" DataIndex="Nombre" Header="Reporte" Width="150" />
                                                        <ext:Column ColumnID="Ruta" DataIndex="Ruta" Header="Ruta Archivo" Width="200" />
                                                        <ext:Column ColumnID="NombreArchivo" DataIndex="NombreArchivo" Header="Archivo" Width="100" />
                                                        <ext:Column ColumnID="TipoServicio" Header="Tipo Servicio" Sortable="true" DataIndex="TipoServicio" Width="100" />
                                                        <ext:Column ColumnID="Clasificacion" Header="Clasificacion" Sortable="true" DataIndex="Clasificacion" Width="100" />
                                                        <ext:Column ColumnID="Estatus" Header="Estatus" Sortable="true" DataIndex="Estatus" Width="100" />
                                                        <ext:Column ColumnID="HoraEjecucion" Header="Hora de Ejecución" Sortable="true" DataIndex="HoraEjecucion" Width="100" />
                                                        <ext:Column ColumnID="ID_TipoServicio" DataIndex="ID_TipoServicio" Hidden="true" />
                                                        <ext:Column ColumnID="ID_Clasificacion" DataIndex="ID_Clasificacion" Hidden="true" />
                                                        <ext:Column ColumnID="ID_ReporteColectiva" DataIndex="ID_ReporteColectiva" Hidden="true" />
                                                        <ext:Column ColumnID="ID_ReporteColectivaConfiguracion" DataIndex="ID_ReporteColectivaConfiguracion" Hidden="true" />
                                                        <ext:Column ColumnID="EsActivo" DataIndex="EsActivo" Hidden="true" />
                                                        <ext:CommandColumn Header="Acciones" Width="80">
                                                            <PrepareToolbar Fn="prepareToolbar" />
                                                            <Commands>
                                                                <ext:GridCommand Icon="Pencil" CommandName="Edit">
                                                                    <ToolTip Text="Editar Configuración" />
                                                                </ext:GridCommand>
                                                                <ext:GridCommand Icon="RecordRed" CommandName="CambiaEstatus">
                                                                    <ToolTip Text="Activar" />
                                                                </ext:GridCommand>
                                                                <ext:GridCommand Icon="RecordGreen" CommandName="CambiaEstatus">
                                                                    <ToolTip Text="Desactivar" />
                                                                </ext:GridCommand>
                                                            </Commands>
                                                        </ext:CommandColumn>
                                                    </Columns>
                                                </ColumnModel>
                                                <SelectionModel>
                                                    <ext:RowSelectionModel runat="server" SingleSelect="true" />
                                                </SelectionModel>
                                                <DirectEvents>
                                                    <Command OnEvent="EjecutarComandoReportes">
                                                        <ExtraParams>
                                                            <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                                            <ext:Parameter Name="Values" Value="Ext.encode(record.data)" Mode="Raw" />
                                                        </ExtraParams>
                                                    </Command>
                                                </DirectEvents>
                                                <LoadMask ShowMask="false" />
                                            </ext:GridPanel>
                                        </Items>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelParametros" runat="server" Title="Accesos Servidor" Layout="FitLayout" Border="false">
                                        <Items>
                                            <ext:Panel runat="server" Layout="FitLayout" AutoScroll="true" Border="false">
                                                <TopBar>
                                                    <ext:Toolbar runat="server">
                                                        <Items>
                                                            <ext:ComboBox ID="cBoxClasificacion2" runat="server" EmptyText="Clasificación..." Width="150"
                                                                DisplayField="Descripcion" ValueField="ID_ClasificacionParametros" AllowBlank="false"
                                                                ListWidth="250" Hidden="true">
                                                                <Store>
                                                                    <ext:Store ID="StoreClasif2" runat="server">
                                                                        <Reader>
                                                                            <ext:JsonReader IDProperty="ID_ClasificacionParametros">
                                                                                <Fields>
                                                                                    <ext:RecordField Name="ID_ClasificacionParametros" />
                                                                                    <ext:RecordField Name="Clave" />
                                                                                    <ext:RecordField Name="Descripcion" />
                                                                                </Fields>
                                                                            </ext:JsonReader>
                                                                        </Reader>
                                                                    </ext:Store>
                                                                </Store>
                                                                <DirectEvents>
                                                                    <Select OnEvent="SeleccionaClasificacion" Before="#{cBoxParametros}.reset();
                                                                        #{cBoxParametros}.setDisabled(false); #{btnAddParametros}.setDisabled(false);
                                                                        #{cBoxParametros}.getStore().removeAll(); #{GridPanelParametros}.getStore().removeAll();">
                                                                        <EventMask ShowMask="true" Msg="Obteniendo Parámetros..." MinDelay="200" />
                                                                    </Select>
                                                                </DirectEvents>
                                                            </ext:ComboBox>
                                                            <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                            <ext:ComboBox ID="cBoxParametros" runat="server" EmptyText="Parámetros sin Asignar..." Width="180"
                                                                DisplayField="Nombre" ValueField="ID_Parametro" Disabled="false" AllowBlank="false">
                                                                <Store>
                                                                    <ext:Store ID="StoreParametros" runat="server">
                                                                        <Reader>
                                                                            <ext:JsonReader IDProperty="ID_Parametro">
                                                                                <Fields>
                                                                                    <ext:RecordField Name="ID_Parametro" />
                                                                                    <ext:RecordField Name="Nombre" />
                                                                                    <ext:RecordField Name="Descripcion" />
                                                                                </Fields>
                                                                            </ext:JsonReader>
                                                                        </Reader>
                                                                    </ext:Store>
                                                                </Store>
                                                            </ext:ComboBox>
                                                            <ext:Button ID="btnAddParametros" runat="server" Text="Asignar Parámetro" Icon="Add" Disabled="false">
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
                                                        AutoHeight="true" Layout="FitLayout">
                                                        <Store>
                                                            <ext:Store ID="StoreValoresParametros2" runat="server" AutoLoad="false">
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="ID_Parametro">
                                                                        <Fields>
                                                                            <ext:RecordField Name="ID_Parametro" />
                                                                            <ext:RecordField Name="Nombre" />
                                                                            <ext:RecordField Name="Descripcion" />
                                                                            <ext:RecordField Name="Valor" />
                                                                            <ext:RecordField Name="ValorPrestablecido" />
                                                                            <ext:RecordField Name="ID_ValorPrestablecido" />
                                                                        </Fields>
                                                                    </ext:JsonReader>
                                                                </Reader>
                                                            </ext:Store>
                                                        </Store>
                                                        <ColumnModel runat="server">
                                                            <Columns>
                                                                <ext:Column ColumnID="ID_Parametro" runat="server" Hidden="true" DataIndex="ID_Parametro" />
                                                                <ext:Column ColumnID="Nombre" Header="Parámetro" Width="200" DataIndex="Nombre">
                                                                    <Renderer Fn="fullName" />
                                                                    <Editor>
                                                                        <ext:DisplayField runat="server" StyleSpec="font-weight:bold;font-family:segoe ui;font-size: 12px;" />
                                                                    </Editor>
                                                                </ext:Column>
                                                                <ext:Column ColumnID="Valor" Header="Valor" Sortable="true" DataIndex="Valor" Width="320" />
                                                                <ext:CommandColumn Header="Acciones" Width="80">
                                                                    <Commands>
                                                                        <ext:GridCommand Icon="Pencil" CommandName="Edit">
                                                                            <ToolTip Text="Editar Valor" />
                                                                        </ext:GridCommand>
                                                                        <ext:GridCommand Icon="Delete" CommandName="Delete">
                                                                            <ToolTip Text="Quitar Parámetro a la Colectiva" />
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
                                                        <LoadMask ShowMask="false" />
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