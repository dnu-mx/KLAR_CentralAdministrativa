<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="AdministrarColectivasTAE.aspx.cs" Inherits="TpvWeb.AdministrarColectivasTAE" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var prepareToolbar = function (grid, toolbar, rowIndex, record) {
            if (record.get("Estatus") == 1) { //ACTIVA
                toolbar.items.get(2).hide();
            } else if (record.get("Estatus") == 2) {
                toolbar.items.get(1).hide();
            } else {
                toolbar.items.get(0).hide();
                toolbar.items.get(1).hide();
                toolbar.items.get(2).hide();
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

        var toolbarPtsBanc = function (grid, toolbar, rowIndex, record) {
            if (record.get("Activo") == true) {
                toolbar.items.get(1).hide();
            } else {
                toolbar.items.get(0).hide();
            }
        }

        var fullPlugin = function (value, metadata, record, rowIndex, colIndex, store) {
            return "<b>" + record.data.Descripcion + "</b>";
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
    <ext:Hidden ID="hdnIdParametro" runat="server" />
    <ext:Hidden ID="hdnOrigen" runat="server" />
    <ext:Window ID="WdwNuevaColectiva" runat="server" Title="Nueva Colectiva" Width="400" Height="330" Hidden="true"
        Modal="true" Resizable="false">
        <Items>
            <ext:FormPanel ID="FormPanelDatosBase" runat="server" Height="300" Padding="3" LabelWidth="120">
                <Content>
                    <ext:Store ID="StoreColectivaPadre" runat="server">
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
                </Content>
                <Items>
                    <ext:FieldSet ID="FieldSetColectiva" runat="server" Title="Colectiva">
                        <Items>
                            <ext:Hidden ID="hdnClaveTipoColectiva" runat="server" />
                            <ext:ComboBox ID="cBoxTipoColectiva" runat="server" FieldLabel="Tipo de Colectiva"
                                Width="355" AllowBlank="false" StoreID="StoreTipoColectiva" DisplayField="Descripcion"
                                ValueField="ID_TipoColectiva">
                                <DirectEvents>
                                    <Select OnEvent="PrestableceTColecPadre" Before="#{hdnClaveTipoColectiva}.clear();
                                        #{hdnClaveTipoColectiva}.setValue(record.get('Clave'));
                                        #{txtTColecAsc}.setValue(''); #{cBoxColecPadre}.clearValue();">
                                        <EventMask ShowMask="true" Msg="Estableciendo Colectiva Padre..." MinDelay="200" />
                                    </Select>
                                </DirectEvents>
                            </ext:ComboBox>
                            <ext:TextField ID="txtClaveColectiva" runat="server" FieldLabel="Clave" MaxLength="50" Width="355"
                                AllowBlank="false" />
                            <ext:TextField ID="txtRazonSocial" runat="server" FieldLabel="Razón Social" MaxLength="50"
                                Width="355" AllowBlank="false"/>
                            <ext:TextField ID="txtNombreComercial" runat="server" FieldLabel="Nombre Comercial" MaxLength="50"
                                Width="355" AllowBlank="false"/>
                        </Items>
                    </ext:FieldSet>
                    <ext:FieldSet ID="FieldSetAscendencia" runat="server" Title="Ascendencia">
                        <Items>
                            <ext:TextField ID="txtTColecAsc" runat="server" FieldLabel="Tipo de Colectiva" Width="355" 
                                ReadOnly="true" Disabled="true"/>
                            <ext:ComboBox ID="cBoxColecPadre" runat="server" FieldLabel="Colectiva Padre" Width="355"
                                StoreID="StoreColectivaPadre" DisplayField="NombreORazonSocial" ValueField="ID_Colectiva" Disabled="true"
                                Mode="Local" AutoSelect="true" Editable="true" ForceSelection="true" TypeAhead="true" MinChars="1"
                                MatchFieldWidth="false" Name="colPadreKey" AllowBlank="false" />
                        </Items>
                    </ext:FieldSet>
                </Items>
                <Buttons>
                    <ext:Button ID="btnNuevaColectiva" runat="server" Text="Crear Colectiva" Icon="Add">
                        <DirectEvents>
                            <Click OnEvent="btnNuevaColectiva_Click" Before="var valid= #{FormPanelDatosBase}.getForm().isValid(); if (!valid) {} return valid;">
                                <EventMask ShowMask="true" Msg="Creando Colectiva..." MinDelay="500" />
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:Window ID="WdwCuenta" runat="server" Title="Datos de la Cuenta" Width="480" Height="220" Hidden="true"
        Modal="true" Resizable="false" Closable="true">
        <Items>
            <ext:FormPanel ID="FormPanelCuenta" runat="server" Padding="10" MonitorValid="true" LabelAlign="Left" LabelWidth="180">
                <Items>
                    <ext:Hidden ID="hdnIdCuenta" runat="server" />
                    <ext:ComboBox ID="cBoxTipoCta" runat="server" FieldLabel="Tipo de Cuenta" Width="250"
                        StoreID="StoreTipoCuenta" DisplayField="Descripcion" ValueField="ID_TipoCuenta"
                        AllowBlank="false" Disabled="true" />
                    <ext:ComboBox ID="cBoxGrupoCuentas" runat="server" FieldLabel="Grupo de Cuentas" Width="250"
                        StoreID="StoreGrupoCuentas" DisplayField="Descripcion" ValueField="ID_GrupoCuenta" 
                        AllowBlank="false" />
                    <ext:TextField ID="txtCuenta" runat="server" FieldLabel="Descripción" Width="250"
                        AllowBlank="false" />
                    <ext:DateField ID="dfVigCuenta" runat="server" Vtype="daterange" FieldLabel="Vigencia"
                        Format="dd/MM/yyyy" MaxLength="12" Width="250" MaxWidth="250" />
                    <ext:ComboBox ID="cBoxCCRCuenta" runat="server" FieldLabel="Cadena Comercial Relacionada"
                        Width="250" StoreID="StoreCCR" DisplayField="NombreORazonSocial" ValueField="ID_Colectiva"
                        Mode="Local" AutoSelect="true" Editable="true" ForceSelection="true" TypeAhead="true"
                        MinChars="1" MatchFieldWidth="false" Name="colCCR_WdwCtas">
                        <Triggers>
                            <ext:FieldTrigger Icon="Clear" Qtip="Borrar" />
                        </Triggers>
                        <Listeners>
                            <TriggerClick Handler="this.removeByValue(this.getValue());this.clearValue();" />
                        </Listeners>
                    </ext:ComboBox>
                    <ext:NumberField ID="nmbNivel" runat="server" FieldLabel="Nivel" Width="250" AllowDecimals="false"
                        AllowNegative="false" Hidden="true"/>
                    <ext:ComboBox ID="cBoxPeriodo" runat="server" FieldLabel="Periodo" Width="250" Hidden="true"
                        DisplayField="Descripcion" ValueField="ID_Periodo">
                        <Store>
                            <ext:Store ID="StorePeriodo" runat="server">
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
                        </Store>
                        <Triggers>
                            <ext:FieldTrigger Icon="Clear" Qtip="Borrar" />
                        </Triggers>
                        <Listeners>
                            <TriggerClick Handler="this.removeByValue(this.getValue());this.clearValue();" />
                        </Listeners>
                    </ext:ComboBox>
                    <ext:CheckboxGroup ID="chkBoxHereda" runat="server" Cls="x-check-group-alt" FieldLabel="Hereda Saldo"
                        Hidden="true">
                        <Items>
                            <ext:Checkbox ID="chkBoxSi" runat="server" BoxLabel="Sí" AnchorVertical="90%" AnchorHorizontal="50%" />
                            <ext:Checkbox ID="chkBoxNo" runat="server" BoxLabel="No" AnchorVertical="90%" AnchorHorizontal="50%" />
                        </Items>
                    </ext:CheckboxGroup>
                </Items>
                <Buttons>
                    <ext:Button ID="btnGuardaCuenta" runat="server" Text="Guardar Cambios" Icon="Disk">
                        <DirectEvents>
                            <Click OnEvent="btnGuardaCuenta_Click" Before="var valid= #{FormPanelCuenta}.getForm().isValid(); if (!valid) {} return valid;" />
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
                            <Click OnEvent="btnGuardarValorParametro_Click" Before="#{hdnOrigen}.setValue('TXT');" />
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:Window ID="WdwValorParametroCombo" runat="server" Title="Editar Valor Parámetro" Width="420" Height="150" Hidden="true"
        Resizable="true">
        <Items>
            <ext:FormPanel runat="server" Padding="10" MonitorValid="true" LabelAlign="Left" LabelWidth="70">
                <Items>
                    <ext:TextField ID="txtParametroCombo" runat="server" FieldLabel="Descripción" Width="300"
                        AllowBlank="false" Selectable="false" ReadOnly="true" />
                    <ext:ComboBox ID="cBoxValorParametro" runat="server" FieldLabel="Valor" Width="300" AllowBlank="false"
                        DisplayField="NombreORazonSocial" ValueField="ID_Colectiva" Editable="false">
                        <Store>
                            <ext:Store ID="StoreColectivasContrato" runat="server">
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_Colectiva">
                                        <Fields>
                                            <ext:RecordField Name="ID_Colectiva" />
                                            <ext:RecordField Name="ClaveColectiva" />
                                            <ext:RecordField Name="NombreORazonSocial" />
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
                            <Click Handler="#{WdwValorParametroCombo}.hide();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button runat="server" Text="Guardar Cambio" Icon="Disk">
                        <DirectEvents>
                            <Click OnEvent="btnGuardarValorParametro_Click" Before="var valid= #{cBoxValorParametro}.isValid(); if (!valid) {} else { #{hdnOrigen}.setValue('CMB'); } return valid;" />
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:Window ID="WdwValorParamExtra" runat="server" Title="Editar Valor Parámetro Extra" Width="410" Height="270" Hidden="true"
        Modal="true" Resizable="false" Closable="true">
        <Items>
            <ext:FormPanel ID="FormPanel1" runat="server" Padding="10" MonitorValid="true" LabelAlign="Left" LabelWidth="70">
                <Items>
                    <ext:Hidden ID="hdnIdParamExtra" runat="server" />
                    <ext:TextField ID="txtParamExtra" runat="server" FieldLabel="Descripción" Width="300"
                        AllowBlank="false" Selectable="false" ReadOnly="true" />
                    <ext:TextArea ID="txtValorParamExtra" runat="server" FieldLabel="Valor" Width="300" MaxLength="1000"
                        Height="150"/>
                </Items>
                <Buttons>
                    <ext:Button runat="server" Text="Cancelar" Icon="Cancel">
                        <Listeners>
                            <Click Handler="#{WdwValorParamExtra}.hide();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button ID="btnGuardarValorParamExtra" runat="server" Text="Guardar Cambio" Icon="Disk">
                        <DirectEvents>
                            <Click OnEvent="btnGuardarValorParamExtra_Click" />
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:Window ID="WdwValorParamExtra_1" runat="server" Title="Editar Valor Parámetro Extra" Width="410" Height="140" Hidden="true"
        Modal="true" Resizable="false" Closable="true">
        <Items>
            <ext:FormPanel ID="FormPanel2" runat="server" Padding="10" MonitorValid="true" LabelAlign="Left" LabelWidth="70">
                <Items>
                    <ext:TextField ID="txtParamExtra_1" runat="server" FieldLabel="Descripción" Width="300"
                        AllowBlank="false" Selectable="false" ReadOnly="true" />
                    <ext:ComboBox ID="cBoxValorParamExtra" runat="server" FieldLabel="Valor" Width="300"
                        DisplayField="Descripcion" ValueField="ID_Valor">
                        <Store>
                            <ext:Store ID="StoreValoresParamExtra" runat="server">
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
                            <Click Handler="#{WdwValorParamExtra_1}.hide();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button ID="btnGuardarValorParamExtra_1" runat="server" Text="Guardar Cambio" Icon="Disk">
                        <DirectEvents>
                            <Click OnEvent="btnGuardarValorParamExtra_Click" />
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:Window ID="WdwNuevoBin" runat="server" Title="Añadir Bin" Icon="Add" Width="420" Height="190"
        Hidden="true" Modal="true" Resizable="false">
        <Items>
            <ext:FormPanel ID="FormPanelNuevoBin" runat="server" Padding="10" LabelWidth="80" Border="false"
                Layout="FormLayout" LabelAlign="Right">
                <Items>
                    <ext:TextField ID="txtSKU" runat="server" FieldLabel="SKU   <span style='color:red;'>*   </span>"
                        AllowBlank="false" Width="300" MaxLength="50" />
                    <ext:TextField ID="txtBIN" runat="server" FieldLabel="BIN   <span style='color:red;'>*   </span>"
                        AllowBlank="false" Width="300" MaxLength="30" />
                    <ext:TextField ID="txtClave" runat="server" FieldLabel="Clave   <span style='color:red;'>*   </span>"
                        AllowBlank="false" Width="300" MaxLength="100" />
                    <ext:TextField ID="txtDescripcion" runat="server" FieldLabel="Descripción   <span style='color:red;'>*   </span>"
                        AllowBlank="false" Width="300" MaxLength="150" />
                </Items>
                <Buttons>
                    <ext:Button runat="server" Text="Cancelar" Icon="Cancel">
                        <Listeners>
                            <Click Handler="#{WdwNuevoBin}.hide();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button ID="btnNuevoBin" runat="server" Text="Añadir BIN" Icon="Tick">
                        <DirectEvents>
                            <Click OnEvent="btnNuevoBin_Click" Before="var valid= #{FormPanelNuevoBin}.getForm().isValid(); if (!valid) {} return valid;">
                                <EventMask ShowMask="true" Msg="Añadiendo BIN..." MinDelay="500" />
                            </Click>
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
    <ext:Store ID="StoreCCR" runat="server">
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
    <ext:Store ID="StoreTipoCuenta" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_TipoCuenta">
                <Fields>
                    <ext:RecordField Name="ID_TipoCuenta" />
                    <ext:RecordField Name="ClaveDescripcion" />
                    <ext:RecordField Name="Descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Store ID="StoreGrupoCuentas" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_GrupoCuenta">
                <Fields>
                    <ext:RecordField Name="ID_GrupoCuenta" />
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
                    <ext:RecordField Name="ClaveTipoColectiva" />
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
                                    <ext:Toolbar runat="server">
                                        <Items>
                                            <ext:Button ID="btnLimpiarIzq" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                                <DirectEvents>
                                                    <Click OnEvent="btnLimpiarIzq_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:ToolbarFill runat="server" />
                                            <ext:Button runat="server" Icon="Add" ToolTip="Crear nueva colectiva"
                                                Text="Nueva Colectiva">
                                                <Listeners>
                                                    <Click Handler="#{FormPanelDatosBase}.reset(); #{WdwNuevaColectiva}.show();" />
                                                </Listeners>
                                            </ext:Button>
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
                                                StoreID="StoreTipoColectiva" DisplayField="Descripcion" ValueField="ID_TipoColectiva" />
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
                                        DisplayMsg="{0} - {1} de {2}" />
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
                                    <ext:FormPanel ID="FormPanelInfoAd" runat="server" Title="Información Adicional" LabelAlign="Left" LabelWidth="200"
                                        AutoScroll="true">
                                        <Content>
                                            <ext:Store ID="StoreEstatus"  runat="server">
                                                <Reader>
                                                    <ext:JsonReader IDProperty="ID_EstatusColectiva">
                                                        <Fields>
                                                            <ext:RecordField Name="ID_EstatusColectiva" />
                                                            <ext:RecordField Name="Clave" />
                                                            <ext:RecordField Name="Descripcion" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Content>
                                        <Items>
                                            <ext:FieldSet ID="FieldSetDatosAd" runat="server" Title="Datos Adicionales de la Colectiva">
                                                <Items>
                                                    <ext:Hidden ID="hdnIdColectiva" runat="server" />
                                                    <ext:TextField ID="txtClaveCol" runat="server" FieldLabel="Clave  <span style='color:red;'>*   </span>"
                                                        MaxLength="50" Width="500" Disabled="true" />
                                                    <ext:TextField ID="txtRazonSoc" runat="server" FieldLabel="Razón Social   <span style='color:red;'>*   </span>"
                                                        MaxLength="50" Width="500" AllowBlank="false" BlankText="La Razón Social es Obligatoria"/>
                                                    <ext:TextField ID="txtNombreCom" runat="server" FieldLabel="Nombre Comercial   <span style='color:red;'>*   </span>"
                                                        MaxLength="50" Width="500" AllowBlank="false" BlankText="El Nombre Comercial es Obligatorio"/>
                                                    <ext:TextField ID="txtApPaterno" runat="server" FieldLabel="Apellido Paterno" MaxLength="50" Width="500" />
                                                    <ext:TextField ID="txtApMaterno" runat="server" FieldLabel="Apellido Materno" MaxLength="50" Width="500" />
                                                     <ext:DateField ID="dfFechaNac" runat="server" FieldLabel="Fecha de Nacimiento" Format="dd/MM/yyyy" Width="500"
                                                        Editable="false" />
                                                    <ext:TextField ID="txtCURP" runat="server" FieldLabel="CURP" MaxLength="15" Width="500" />
                                                    <ext:TextField ID="txtRFC" runat="server" FieldLabel="RFC" MaxLength="15" Width="500" />
                                                    <ext:TextField ID="txtEmail" runat="server" FieldLabel="Correo Electrónico" MaxLength="30" Width="500" />
                                                    <ext:NumberField ID="nfTelefonoFijo" runat="server" FieldLabel="Teléfono Fijo" MaxLength="10" Width="500"
                                                        AllowDecimals="False" AllowNegative="False" />
                                                    <ext:NumberField ID="nfTelefonoCel" runat="server" FieldLabel="Teléfono Celular" MaxLength="10" Width="500"
                                                        AllowDecimals="False" AllowNegative="False" />
                                                    <ext:ComboBox ID="cBoxCadena" runat="server" FieldLabel="Cadena Comercial Relacionada" Width="500" 
                                                        StoreID="StoreCCR" DisplayField="NombreORazonSocial" ValueField="ID_Colectiva" Mode="Local"
                                                        AutoSelect="true" ForceSelection="true" TypeAhead="true" MinChars="1"
                                                        MatchFieldWidth="false" Name="colCCR_DE">
                                                        <Triggers>
                                                            <ext:FieldTrigger Icon="Clear" Qtip="Borrar" />
                                                        </Triggers>
                                                        <Listeners>
                                                            <TriggerClick Handler="this.removeByValue(this.getValue());this.clearValue();" />
                                                        </Listeners>
                                                        </ext:ComboBox>
                                                    <ext:ComboBox ID="cBoxEstatus" runat="server" FieldLabel="Estatus  <span style='color:red;'>*   </span>"
                                                        Width="500" StoreID="StoreEstatus" DisplayField="Descripcion" ValueField="ID_EstatusColectiva"
                                                        AllowBlank="false" />
                                                    <ext:TextField ID="txtPassword" runat="server" FieldLabel="Contraseña" Width="500" InputType="Password" />
                                                    <ext:TextField ID="txtRePassword" runat="server" FieldLabel="Repite la Contraseña" Width="500"
                                                        InputType="Password" />
                                                </Items>
                                                <Buttons>
                                                    <ext:Button ID="btnGuardarInfoAd" runat="server" Text="Guardar" Icon="Disk" Disabled="true">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnGuardarInfoAd_Click" Before="var valid= #{FormPanelInfoAd}.getForm().isValid(); if (!valid) {} return valid;">
                                                                <EventMask ShowMask="true" Msg="Guardando Información..." MinDelay="500" />
                                                            </Click>
                                                        </DirectEvents>
                                                    </ext:Button>
                                                </Buttons>
                                            </ext:FieldSet>
                                        </Items>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelDirecciones" runat="server" Title="Domicilios" LabelAlign="Left" LabelWidth="200">
                                        <Content>
                                            <ext:Store ID="StoreColonias" runat="server">
                                                <Reader>
                                                    <ext:JsonReader IDProperty="ID_Colonia">
                                                        <Fields>
                                                            <ext:RecordField Name="ID_Colonia" />
                                                            <ext:RecordField Name="Colonia" />
                                                            <ext:RecordField Name="Municipio" />
                                                            <ext:RecordField Name="Estado" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Content>
                                        <Items>
                                            <ext:FieldSet ID="FieldSetDirecciones" runat="server" Title="Datos del Domicilio">
                                                <Items>
                                                    <ext:TextField ID="txtID_Direccion" runat="server" Hidden="true" Enabled="false" />
                                                    <ext:ComboBox ID="cBoxTipoDomicilio" runat="server" FieldLabel="Tipo de Domicilio" Width="500"
                                                        OnDirectSelect="cBoxTipoDomicilio_Select" AllowBlank="false" Disabled="true">
                                                        <Items>
                                                            <ext:ListItem Text="Físico" Value="1"/>
                                                            <ext:ListItem Text="Fiscal" Value="2" />
                                                        </Items>
                                                    </ext:ComboBox>
                                                    <ext:TextField ID="txtCalle" runat="server" FieldLabel="Calle" MaxLength="30" Width="500" />
                                                    <ext:TextField ID="txtNumExterior" runat="server" FieldLabel="Número Exterior" MaxLength="6" Width="500" />
                                                    <ext:TextField ID="txtNumInterior" runat="server" FieldLabel="Número Interior" MaxLength="6" Width="500" />
                                                    <ext:TextField ID="txtEntreCalles" runat="server" FieldLabel="Entre las Calles" Width="500" MaxLength="100" />
                                                    <ext:TextField ID="txtReferencias" runat="server" FieldLabel="Referencias" Width="500" MaxLength="200" />
                                                    <ext:TextField ID="txtCP" runat="server" FieldLabel="Código Postal" MinLength="5" MaxLength="5"
                                                        Width="500" EnableKeyEvents="true">
                                                        <Listeners>
                                                            <Change Handler="#{cBoxColonia}.clear(); #{txtColonia}.clear(); #{txtColonia}.hide();
                                                                TpvWeb.LlenaComboColonias();" />
                                                        </Listeners>
                                                    </ext:TextField>
                                                    <ext:TextField ID="txtIDColonia" runat="server" Hidden="true" Enabled="false" />
                                                    <ext:ComboBox ID="cBoxColonia" runat="server" FieldLabel="Colonia" StoreID="StoreColonias" Width="500"
                                                        DisplayField="Colonia" ValueField="ID_Colonia">
                                                        <Triggers>
                                                            <ext:FieldTrigger Icon="Clear" Qtip="Borrar" />
                                                        </Triggers>
                                                        <Listeners>
                                                            <Select Handler="var cp = this.getValue(); var patt = new RegExp(parseInt(#{txtCP}.getValue() + '999'));
                                                                var record = this.getStore().getById(cp);
                                                                var texto = record.get('Colonia');
                                                                if (patt.test(cp) && texto == 'Otra') {#{txtColonia}.show();}
                                                                else {#{txtColonia}.clear(); #{txtColonia}.hide();}" />
                                                            <TriggerClick Handler="this.removeByValue(this.getValue());this.clearValue();" />
                                                        </Listeners>
                                                    </ext:ComboBox>
                                                    <ext:TextField ID="txtColonia" runat="server" FieldLabel="Nombre de la Colonia" Hidden="true" Enabled="false"
                                                        EmptyText="Por favor, especifique..." Width="500" />
                                                    <ext:TextField ID="txtClaveMunicipio" runat="server" Hidden="true" Enabled="false" />
                                                    <ext:TextField ID="txtMunicipio" runat="server" FieldLabel="Municipio" MaxLength="70" Width="500" ReadOnly="true"/>
                                                    <ext:TextField ID="txtClaveEstado" runat="server" Hidden="true" Enabled="false" />
                                                    <ext:TextField ID="txtEstado" runat="server" FieldLabel="Estado" MaxLength="50" Width="500" ReadOnly="true"/>
                                                </Items>
                                                <Buttons>
                                                    <ext:Button ID="btnGuardaDireccion" runat="server" Text="Guardar" Icon="Disk" Disabled="true">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnGuardaDireccion_Click" Before="var valid= #{FormPanelDirecciones}.getForm().isValid(); if (!valid) {} return valid;">
                                                                <EventMask ShowMask="true" Msg="Guardando Dirección..." MinDelay="500" />
                                                            </Click>
                                                        </DirectEvents>
                                                    </ext:Button>
                                                </Buttons>
                                            </ext:FieldSet>
                                        </Items>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelCuentas" runat="server" Title="Cuentas" LabelAlign="Left" LabelWidth="200">
                                        <Content>
                                            <ext:BorderLayout ID="BorderLayoutCuentas" runat="server">
                                                <North Split="true">
                                                    <ext:FormPanel ID="FormPanelNuevaCuenta" runat="server" LabelWidth="200" Height="210">
                                                        <Items>
                                                            <ext:FieldSet ID="FieldSetCuentas" runat="server" Title="Datos de la Nueva Cuenta">
                                                                <Items>
                                                                    <ext:ComboBox ID="cBoxTipoCuenta" runat="server" FieldLabel="Tipo de Cuenta" Width="500"
                                                                        StoreID="StoreTipoCuenta" DisplayField="ClaveDescripcion" ValueField="ID_TipoCuenta"
                                                                        AllowBlank="false">
                                                                        <Listeners>
                                                                            <Select Handler="var id = this.getValue(); var record = this.getStore().getById(id);
                                                                                var texto = record.get('Descripcion'); #{txtDescCuenta}.setValue(texto);" />
                                                                        </Listeners>
                                                                    </ext:ComboBox>
                                                                    <ext:ComboBox ID="cBoxGpoCuentas" runat="server" FieldLabel="Grupo de Cuentas" Width="500"
                                                                        StoreID="StoreGrupoCuentas" DisplayField="Descripcion" ValueField="ID_GrupoCuenta"
                                                                        AllowBlank="false" />
                                                                    <ext:TextField ID="txtDescCuenta" runat="server" FieldLabel="Descripción" Width="500"
                                                                        AllowBlank="false" />
                                                                    <ext:DateField ID="dfVigenciaCuenta" runat="server" FieldLabel="Vigencia" Vtype="daterange"
                                                                        Format="dd/MM/yyyy" MaxLength="12" Width="500" MaxWidth="500" EnableKeyEvents="true" />
                                                                    <ext:ComboBox ID="cBoxCadComerCuentas" runat="server" FieldLabel="Cadena Comercial Relacionada"
                                                                        Width="500" StoreID="StoreCCR" DisplayField="NombreORazonSocial" ValueField="ID_Colectiva"
                                                                        Mode="Local" AutoSelect="true" Editable="true" ForceSelection="true" TypeAhead="true"
                                                                        MinChars="1" MatchFieldWidth="false" Name="colCCR_Ctas">
                                                                        <Triggers>
                                                                            <ext:FieldTrigger Icon="Clear" Qtip="Borrar" />
                                                                        </Triggers>
                                                                        <Listeners>
                                                                            <TriggerClick Handler="this.removeByValue(this.getValue());this.clearValue();" />
                                                                        </Listeners>
                                                                    </ext:ComboBox>
                                                                </Items>
                                                                <Buttons>
                                                                    <ext:Button ID="btnNuevaCuenta" runat="server" Text="Añadir Cuenta" Icon="Add" Disabled="true">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="btnNuevaCuenta_Click" Before="var valid= #{FormPanelNuevaCuenta}.getForm().isValid(); if (!valid) {} return valid;">
                                                                                <EventMask ShowMask="true" Msg="Añadiendo Cuenta..." MinDelay="500" />
                                                                            </Click>
                                                                        </DirectEvents>
                                                                    </ext:Button>
                                                                </Buttons>
                                                            </ext:FieldSet>
                                                        </Items>
                                                    </ext:FormPanel>
                                                </North>
                                                <Center Split="true">
                                                    <ext:GridPanel ID="GridCuentas" runat="server" Header="true" Title="Cuentas de la Colectiva">
                                                        <Store>
                                                            <ext:Store ID="StoreCuentas" runat="server">
                                                                <DirectEventConfig IsUpload="true" />
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="ID_Cuenta">
                                                                        <Fields>
                                                                            <ext:RecordField Name="ID_Cuenta" />
                                                                            <ext:RecordField Name="ID_TipoCuenta" />
                                                                            <ext:RecordField Name="TipoCuenta" />
                                                                            <ext:RecordField Name="Descripcion" />
                                                                            <ext:RecordField Name="SaldoActual" />
                                                                            <ext:RecordField Name="ID_ColectivaCadenaComercial" />
                                                                            <ext:RecordField Name="CadenaRelacionada" />
                                                                            <ext:RecordField Name="ClaveEstatus" />
                                                                            <ext:RecordField Name="Estatus" />
                                                                        </Fields>
                                                                    </ext:JsonReader>
                                                                </Reader>
                                                                <DirectEventConfig IsUpload="true" />
                                                            </ext:Store>
                                                        </Store>
                                                        <ColumnModel runat="server">
                                                            <Columns>
                                                                <ext:Column DataIndex="ID_Cuenta" Hidden="true" />
                                                                <ext:CommandColumn Header="Acciones" Width="60" >
                                                                    <PrepareToolbar Fn="prepareToolbar" />
                                                                    <Commands>
                                                                        <ext:GridCommand Icon="PageWhiteEdit" CommandName="Edit">
                                                                            <ToolTip Text="Editar Cuenta" />
                                                                        </ext:GridCommand>
                                                                        <ext:GridCommand Icon="LockOpen" CommandName="Lock">
                                                                            <ToolTip Text="Bloquear Cuenta" />
                                                                        </ext:GridCommand>
                                                                        <ext:GridCommand Icon="Lock" CommandName="Unlock">
                                                                            <ToolTip Text="Desbloquear Cuenta" />
                                                                        </ext:GridCommand>
                                                                    </Commands>
                                                                </ext:CommandColumn>
                                                                <ext:Column DataIndex="ID_Cuenta" Hidden="true" ColumnID="ID_Cuenta" />
                                                                <ext:Column DataIndex="ID_TipoCuenta" Hidden="true" ColumnID="ID_TipoCuenta" />
                                                                <ext:Column DataIndex="TipoCuenta" Header="Tipo de Cuenta" Width="90" />
                                                                <ext:Column DataIndex="Descripcion" Header="Descripción" ColumnID="Descripcion" Width="170"/>
                                                                <ext:Column DataIndex="SaldoActual" Header="Saldo Actual" ColumnID="SaldoActual" Width="80">
                                                                    <Renderer Format="UsMoney" />
                                                                </ext:Column>
                                                                <ext:Column DataIndex="ID_ColectivaCadenaComercial" Hidden="true" ColumnID="ID_ColectivaCadenaComercial" />
                                                                <ext:Column DataIndex="CadenaRelacionada" Header="Cadena Relacionada" ColumnID="CadenaRelacionada" Width="200" />
                                                                <ext:Column DataIndex="ClaveEstatus" Hidden="true" ColumnID="ClaveEstatus" />
                                                                <ext:Column DataIndex="Estatus" Hidden="true" ColumnID="Estatus" />
                                                            </Columns>
                                                        </ColumnModel>
                                                        <DirectEvents>
                                                            <Command OnEvent="EjecutarComando">
                                                                <Confirmation BeforeConfirm="if (command == 'Edit') return false;" 
                                                                    ConfirmRequest="true" Title="Confirmación" Message="¿Estás seguro de modificar el estatus de la Cuenta?" />
                                                                <ExtraParams>
                                                                    <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                                                    <ext:Parameter Name="ID_Cuenta" Value="Ext.encode(record.data.ID_Cuenta)" Mode="Raw" />
                                                                    <ext:Parameter Name="ID_TipoCuenta" Value="Ext.encode(record.data.ID_TipoCuenta)" Mode="Raw" />
                                                                    <ext:Parameter Name="Descripcion" Value="Ext.encode(record.data.Descripcion)" Mode="Raw" />
                                                                    <ext:Parameter Name="ID_ColectivaCadenaComercial" Value="Ext.encode(record.data.ID_ColectivaCadenaComercial)" Mode="Raw" />
                                                                    <ext:Parameter Name="TipoCuenta" Value="Ext.encode(record.data.TipoCuenta)" Mode="Raw" />
                                                                    <ext:Parameter Name="ClaveEstatus" Value="Ext.encode(record.data.ClaveEstatus)" Mode="Raw" />
                                                                </ExtraParams>
                                                            </Command>
                                                        </DirectEvents>
                                                        <SelectionModel>
                                                            <ext:RowSelectionModel SingleSelect="true" />
                                                        </SelectionModel>
                                                        <BottomBar>
                                                            <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="StoreCuentas" DisplayInfo="true"
                                                                DisplayMsg="Cuentas {0} - {1} de {2}" />
                                                        </BottomBar>
                                                    </ext:GridPanel>
                                                </Center>
                                            </ext:BorderLayout>
                                        </Content>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelContrato" runat="server" Title="Contrato" Border="false">
                                        <Items>
                                            <ext:Panel ID="PanelParametros" runat="server" Layout="FitLayout" Border="false">
                                                <TopBar>
                                                    <ext:Toolbar ID="ToolbarOper" runat="server">
                                                        <Items>
                                                            <ext:ComboBox ID="cBoxColectivaOrigen" runat="server" EmptyText="Selecciona Colectiva Origen"
                                                                Width="180" StoreID="StoreCCR" DisplayField="NombreORazonSocial" ValueField="ID_Colectiva"
                                                                Mode="Local" AutoSelect="true" Editable="true" ForceSelection="true" TypeAhead="true"
                                                                MinChars="1" MatchFieldWidth="false" Name="colCCR_Clon" AllowBlank="false">
                                                                <Triggers>
                                                                    <ext:FieldTrigger Icon="Clear" Qtip="Borrar" />
                                                                </Triggers>
                                                                <Listeners>
                                                                    <TriggerClick Handler="this.removeByValue(this.getValue());this.clearValue();" />
                                                                </Listeners>
                                                            </ext:ComboBox>
                                                            <ext:Button ID="btnClonarParams" runat="server" Text="Clonar Parámetros" Icon="Add">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnClonarParams_Click" Before="var valid= #{cBoxColectivaOrigen}.isValid(); if (!valid) {} return valid;">
                                                                        <EventMask ShowMask="true" Msg="Clonando Contrato..." MinDelay="500" />
                                                                    </Click>
                                                                </DirectEvents>
                                                            </ext:Button>
                                                            <ext:ToolbarFill ID="tbfDummy" runat="server" />
                                                            <ext:ComboBox ID="cBoxValoresContrato" runat="server" EmptyText="Valores sin Asignar" Width="120"
                                                                DisplayField="Nombre" ValueField="ID_ValorContrato" AllowBlank="false">
                                                                <Store>
                                                                    <ext:Store ID="StoreValoresContrato" runat="server">
                                                                        <Reader>
                                                                            <ext:JsonReader IDProperty="ID_ValorContrato">
                                                                                <Fields>
                                                                                    <ext:RecordField Name="ID_ValorContrato" />
                                                                                    <ext:RecordField Name="Nombre" />
                                                                                    <ext:RecordField Name="Descripcion" />
                                                                                </Fields>
                                                                            </ext:JsonReader>
                                                                        </Reader>
                                                                    </ext:Store>
                                                                </Store>
                                                            </ext:ComboBox>
                                                            <ext:Button ID="btnAddValor" runat="server" Text="Añadir Valor al Contrato" Icon="Add">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnAddValor_Click" Before="var valid= #{cBoxValoresContrato}.isValid(); if (!valid) {} return valid;">
                                                                        <EventMask ShowMask="true" Msg="Añadiendo Valor..." MinDelay="500" />
                                                                    </Click>
                                                                </DirectEvents>
                                                            </ext:Button>
                                                        </Items>
                                                    </ext:Toolbar>
                                                </TopBar>
                                                <Items>
                                                    <ext:GridPanel ID="GridParamsContrato" runat="server" Header="true" Border="false"
                                                         Height="417" Layout="FitLayout">
                                                        <Store>
                                                            <ext:Store ID="StoreParamsContrato" runat="server" >
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="ID_ValorContrato">
                                                                        <Fields>
                                                                            <ext:RecordField Name="ID_ValorContrato" />
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
                                                        <ColumnModel ID="ColumnModel1" runat="server">
                                                            <Columns>
                                                                <ext:Column ColumnID="ID_ValorContrato" runat="server" Hidden="true" DataIndex="ID_ValorContrato" />
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
                                                        <BottomBar>
                                                            <ext:PagingToolbar ID="PagingToolBar3" runat="server" StoreID="StoreParamsContrato" DisplayInfo="true"
                                                                DisplayMsg="Parámetros de Contrato {0} - {1} de {2}" PageSize="10" />
                                                        </BottomBar>
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
                                    <ext:FormPanel ID="FormPanelParamsExtra" runat="server" Title="Parámetros Extra" Layout="FitLayout"
                                        Border="false">
                                        <Items>
                                            <ext:Panel ID="PanelPE" runat="server" Layout="FitLayout" AutoScroll="true" Border="false">
                                                <TopBar>
                                                    <ext:Toolbar ID="Toolbar1" runat="server">
                                                        <Items>
                                                            <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                            <ext:ComboBox ID="cBoxParamsExtra" runat="server" EmptyText="Parámetros sin Asignar..." Width="180"
                                                                DisplayField="Nombre" ValueField="ID_Parametro" AllowBlank="false">
                                                                <Store>
                                                                    <ext:Store ID="StoreParamsExtra" runat="server">
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
                                                            <ext:Button ID="btnAddParam" runat="server" Text="Añadir Parámetro" Icon="Add">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnAddParam_Click" Before="var valid= #{cBoxParamsExtra}.isValid(); if (!valid) {} return valid;">
                                                                        <EventMask ShowMask="true" Msg="Añadiendo Parámetro..." MinDelay="500" />
                                                                    </Click>
                                                                </DirectEvents>
                                                            </ext:Button>
                                                        </Items>
                                                    </ext:Toolbar>
                                                </TopBar>
                                                <Items>
                                                    <ext:GridPanel ID="GridParamsExtra" runat="server" Header="true" Border="false" AutoScroll="true"
                                                        AutoHeight="true" Layout="FitLayout">
                                                        <Store>
                                                            <ext:Store ID="StoreGridParamsExtra" runat="server">
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="ID_Parametro">
                                                                        <Fields>
                                                                            <ext:RecordField Name="ID_Parametro" />
                                                                            <ext:RecordField Name="Nombre" />
                                                                            <ext:RecordField Name="Descripcion" />
                                                                            <ext:RecordField Name="Valor" />
                                                                            <ext:RecordField Name="ValorPrestablecido" />
                                                                        </Fields>
                                                                    </ext:JsonReader>
                                                                </Reader>
                                                            </ext:Store>
                                                        </Store>
                                                        <ColumnModel ID="ColumnModel2" runat="server">
                                                            <Columns>
                                                                <ext:Column ColumnID="ID_Parametro" runat="server" Hidden="true" DataIndex="ID_Parametro" />
                                                                <ext:Column ColumnID="Nombre" Header="Parámetro" Width="200" DataIndex="Nombre">
                                                                    <Renderer Fn="fullName" />
                                                                    <Editor>
                                                                        <ext:DisplayField runat="server" StyleSpec="font-weight:bold;font-family:segoe ui;font-size: 12px;" />
                                                                    </Editor>
                                                                </ext:Column>
                                                                <ext:Column ColumnID="Valor" Header="Valor" Sortable="true" DataIndex="Valor" Width="320" />
                                                                <ext:CommandColumn Header="Acciones" Width="80" >
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
                                                                <GetRowClass Handler="rowParams.body = '<p>' + record.data.Descripcion + '</p>'; return 'x-grid4-row-expanded';" />
                                                            </ext:GridView>
                                                        </View>
                                                        <SelectionModel>
                                                            <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true" />
                                                        </SelectionModel>
                                                        <DirectEvents>
                                                            <Command OnEvent="EjecutarComandoParamExtra">
                                                                <Confirmation BeforeConfirm="if (command == 'Edit') return false;" 
                                                                    ConfirmRequest="true" Title="Confirmación" Message="¿Estás seguro de quitar el parámetro a la colectiva?" />
                                                                <ExtraParams>
                                                                    <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                                                    <ext:Parameter Name="ID_Parametro" Value="Ext.encode(record.data.ID_Parametro)" Mode="Raw" />
                                                                    <ext:Parameter Name="Nombre" Value="Ext.encode(record.data.Nombre)" Mode="Raw" />
                                                                    <ext:Parameter Name="Descripcion" Value="Ext.encode(record.data.Descripcion)" Mode="Raw" />
                                                                    <ext:Parameter Name="Valor" Value="Ext.encode(record.data.Valor)" Mode="Raw" />
                                                                    <ext:Parameter Name="ValorPrestablecido" Value="Ext.encode(record.data.ValorPrestablecido)" Mode="Raw" />
                                                                </ExtraParams>
                                                            </Command>
                                                        </DirectEvents>
                                                        <LoadMask ShowMask="false" />
                                                    </ext:GridPanel>
                                                </Items>
                                            </ext:Panel>
                                        </Items>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelPtsBancarios" runat="server" Title="Puntos Bancarios" Layout="FitLayout" 
                                        Border="false">
                                        <Items>
                                            <ext:Panel runat="server" Layout="FitLayout" AutoScroll="true" Border="false">
                                                <TopBar>
                                                    <ext:Toolbar runat="server">
                                                        <Items>
                                                            <ext:ToolbarFill runat="server" />
                                                            <ext:Button ID="btnAddBin" runat="server" Text="Añadir BIN" Icon="Add">
                                                                <Listeners>
                                                                    <Click Handler="#{WdwNuevoBin}.show();" />
                                                                </Listeners>
                                                            </ext:Button>
                                                        </Items>
                                                    </ext:Toolbar>
                                                </TopBar>
                                                <Items>
                                                    <ext:GridPanel ID="GridPanelPtsBancarios" runat="server" Header="true" Border="false" AutoScroll="true"
                                                        AutoHeight="true" Layout="FitLayout">
                                                        <Store>
                                                            <ext:Store ID="StorePtsBancarios" runat="server">
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="ID_ProductoPlugin">
                                                                        <Fields>
                                                                            <ext:RecordField Name="ID_ProductoPlugin" />
                                                                            <ext:RecordField Name="SKU" />
                                                                            <ext:RecordField Name="BIN" />
                                                                            <ext:RecordField Name="Clave" />
                                                                            <ext:RecordField Name="Descripcion" />
                                                                            <ext:RecordField Name="Activo" />
                                                                        </Fields>
                                                                    </ext:JsonReader>
                                                                </Reader>
                                                            </ext:Store>
                                                        </Store>
                                                        <ColumnModel runat="server">
                                                            <Columns>
                                                                <ext:CommandColumn Header="Activo" Width="50">
                                                                    <PrepareToolbar Fn="toolbarPtsBanc" />
                                                                    <Commands>
                                                                        <ext:GridCommand Icon="RecordGreen" CommandName="Desactivar">
                                                                            <ToolTip Text="Desactivar BIN en la Colectiva" />
                                                                        </ext:GridCommand>
                                                                        <ext:GridCommand Icon="RecordRed" CommandName="Activar">
                                                                            <ToolTip Text="Activar BIN en la Colectiva" />
                                                                        </ext:GridCommand>
                                                                    </Commands>
                                                                </ext:CommandColumn>
                                                                <ext:Column runat="server" Hidden="true" DataIndex="ID_ProductoPlugin" />
                                                                <ext:Column Header="Clave" Width="100" DataIndex="Clave" />
                                                                <ext:Column Header="Descripción" Width="200" DataIndex="Descripcion" />
                                                                <ext:Column Header="SKU" Sortable="true" DataIndex="SKU" Width="160" />
                                                                <ext:Column Header="BIN" Sortable="true" DataIndex="BIN" Width="160" />
                                                            </Columns>
                                                        </ColumnModel>
                                                        <Plugins>
                                                            <ext:GridFilters runat="server" Local="true" FiltersText="Filtros">
                                                                <Filters>
                                                                    <ext:StringFilter DataIndex="Clave" />
                                                                    <ext:StringFilter DataIndex="Descripcion" />
                                                                    <ext:StringFilter DataIndex="SKU" />
                                                                    <ext:StringFilter DataIndex="BIN" />
                                                                </Filters>
                                                            </ext:GridFilters>
                                                        </Plugins>
                                                        <SelectionModel>
                                                            <ext:RowSelectionModel ID="RowSelectionModel2" runat="server" SingleSelect="true" />
                                                        </SelectionModel>
                                                        <DirectEvents>
                                                            <Command OnEvent="EjecutarComandoPtsBancarios">
                                                                <ExtraParams>
                                                                    <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                                                    <ext:Parameter Name="ID_ProductoPlugIn" Value="Ext.encode(record.data.ID_ProductoPlugin)" Mode="Raw" />
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
