<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="AdministrarColectivasParabilia.aspx.cs" Inherits="TpvWeb.AdministrarColectivasParabilia" %>

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
    <ext:Hidden ID="hdnIdParametro" runat="server" />
    <ext:Hidden ID="hdnParametroNombre" runat="server" />
    <ext:Hidden ID="hdnOrigen" runat="server" />
    <ext:Hidden ID="hdnValorIniPMA" runat="server" />
    <ext:Hidden ID="hdnValorFinPMA" runat="server" />
    <ext:Window ID="WdwNuevaColectiva" runat="server" Title="Nueva Colectiva" Width="400" Height="350" Hidden="true"
        Modal="true" Resizable="false">
        <Items>
            <ext:FormPanel ID="FormPanelDatosBase" runat="server" Height="320" Padding="3" LabelWidth="120">
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
                    <ext:Store ID="StoreDivisa" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Divisa">
                                <Fields>
                                    <ext:RecordField Name="ID_Divisa" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="Descripcion" Direction="ASC" />
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
                            <ext:TextField ID="txtClaveColectiva" runat="server" FieldLabel="Clave Colectiva" MaxLength="50"
                                Width="355" AllowBlank="false" />
                            <ext:TextField ID="txtRazonSocial" runat="server" FieldLabel="Razón Social" MaxLength="50"
                                Width="355" AllowBlank="false" />
                            <ext:TextField ID="txtNombreComercial" runat="server" FieldLabel="Nombre Comercial" MaxLength="50"
                                Width="355" AllowBlank="false" />
                        </Items>
                    </ext:FieldSet>
                    <ext:FieldSet ID="FieldSetAscendencia" runat="server" Title="Ascendencia">
                        <Items>
                            <ext:TextField ID="txtTColecAsc" runat="server" FieldLabel="Tipo de Colectiva" Width="355"
                                ReadOnly="true" Disabled="true" />
                            <ext:ComboBox ID="cBoxColecPadre" runat="server" FieldLabel="Colectiva Padre" Width="355"
                                StoreID="StoreColectivaPadre" DisplayField="NombreORazonSocial" ValueField="ID_Colectiva" Disabled="true"
                                Mode="Local" AutoSelect="true" Editable="true" ForceSelection="true" TypeAhead="true" MinChars="1"
                                MatchFieldWidth="false" Name="colPadreKey" AllowBlank="false" />
                            <ext:ComboBox ID="cBoxDivisa" runat="server" FieldLabel="Divisa" Width="355"
                                StoreID="StoreDivisa" DisplayField="Descripcion" ValueField="ID_Divisa" Disabled="false" AllowBlank="false" />
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
    <ext:Window ID="WdwCuenta" runat="server" Title="Datos de la Cuenta" Width="450" AutoHeight="true" Hidden="true"
        Modal="true" Resizable="false" Closable="true">
        <Items>
            <ext:FormPanel ID="FormPanelCuenta" runat="server" Padding="10" MonitorValid="true" LabelAlign="Left" LabelWidth="150">
                <Items>
                    <ext:Hidden ID="hdnIdCuenta" runat="server" />
                    <ext:ComboBox ID="cBoxTipoCta" runat="server" FieldLabel="Tipo de Cuenta" Width="250"
                        StoreID="StoreTipoCuenta" DisplayField="Descripcion" ValueField="ID_TipoCuenta"
                        AllowBlank="false" Disabled="true" />
                    <ext:TextField ID="txtCuenta" runat="server" FieldLabel="Descripción" Width="250"
                        AllowBlank="false" />
                    <ext:DateField ID="dfVigCuenta" runat="server" Vtype="daterange" FieldLabel="Vigencia"
                        Format="dd/MM/yyyy" MaxLength="12" Width="250" MaxWidth="250" />
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
                        DisplayField="Descripcion" ValueField="ID_Valor" Editable="false">
                        <Store>
                            <ext:Store ID="StoreValoresPrestablecidos" runat="server">
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
    <ext:Window ID="WdwValorParametroMA" runat="server" Title="Editar Valor Parámetro Adicional" Width="420" Height="150" Hidden="true"
        Resizable="true">
        <Items>
            <ext:FormPanel ID="FormPanelValorParamTxtMA" runat="server" Padding="10" MonitorValid="true" LabelAlign="Left" LabelWidth="70">
                <Items>
                    <ext:TextField ID="txtParametroMA" runat="server" FieldLabel="Descripción" Width="300"
                        AllowBlank="false" Selectable="false" ReadOnly="true" />
                    <ext:TextField ID="txtValorParFloatMA" runat="server" FieldLabel="Valor" Width="300" MaxLength="50"
                        MaskRe="[0-9\.]" Hidden="true" Regex="^-?[0-9]*(\.[0-9]{1,4})?$">
                        <Listeners>
                            <Change Handler="var inicial = parseFloat(#{hdnValorIniPMA}.getValue());
                                var final = parseFloat(#{hdnValorFinPMA}.getValue());
                                var actual = parseFloat(this.getValue());
                                var _vmsg = 'El valor del parámetro debe estar entre ' + inicial + ' y ' + final;
                                if ((inicial != '' && final != '') && (actual < inicial || actual > final)) {
                                    this.clear();
                                    Ext.MessageBox.show({
                                        icon: Ext.MessageBox.ERROR,
                                        title: 'Valor erróneo',
                                        msg: _vmsg,
                                        buttons: Ext.MessageBox.OK,
                                        });
                                    return false; }" />
                        </Listeners>
                    </ext:TextField>
                    <ext:TextField ID="txtValorParIntMA" runat="server" FieldLabel="Valor" Width="300" MaxLength="50"
                        MaskRe="[0-9]" Hidden="true">
                        <Listeners>
                            <Change Handler="var inicial = parseInt(#{hdnValorIniPMA}.getValue());
                                var final = parseInt(#{hdnValorFinPMA}.getValue());
                                var actual = parseInt(this.getValue());
                                var _vmsg = 'El valor del parámetro debe estar entre ' + inicial + ' y ' + final;
                                if ((inicial != '' && final != '') && (actual < inicial || actual > final)) {
                                    this.clear();
                                    Ext.MessageBox.show({
                                        icon: Ext.MessageBox.ERROR,
                                        title: 'Valor erróneo',
                                        msg: _vmsg,
                                        buttons: Ext.MessageBox.OK,
                                        });
                                    return false; }" />
                        </Listeners>
                    </ext:TextField>
                    <ext:TextArea ID="txtValorParStringMA" runat="server" FieldLabel="Valor" Width="300" AutoHeight="true"
                        MaxLength="5000" Hidden="true" />
                    <ext:ComboBox ID="cBoxValorParMA" runat="server" FieldLabel="Valor" Width="300" Hidden="true">
                        <Items>
                            <ext:ListItem Text="Sí" Value="true" />
                            <ext:ListItem Text="No" Value="false" />
                        </Items>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cBoxCatalogoPMA" runat="server" FieldLabel="Valor" Width="300" Hidden="true"
                        ValueField="ID_ValorPreePMA" DisplayField="Descripcion">
                        <Store>
                            <ext:Store ID="StoreCatalogoPMA" runat="server">
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_ValorPreePMA">
                                        <Fields>
                                            <ext:RecordField Name="ID_ValorPreePMA" />
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
                    <ext:Button runat="server" Text="Cancelar" Icon="Cancel">
                        <Listeners>
                            <Click Handler="#{WdwValorParametroMA}.hide();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button runat="server" Text="Guardar Cambio" Icon="Disk">
                        <DirectEvents>
                            <Click OnEvent="btnGuardarValorParametroMA_Click"/>
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
    <ext:Store ID="StorePMAsCat" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_PMA">
                <Fields>
                    <ext:RecordField Name="ID_PMA" />
                    <ext:RecordField Name="Clave" />
                    <ext:RecordField Name="Descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
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
                                            <ext:Button ID="btnCreaColectivas" runat="server" Icon="Add" ToolTip="Crear nueva colectiva"
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
                                    <RowClick OnEvent="selectRowResultados_Event" Before="if (((#{hdnClaveTipoCol}.getValue() == 'GCM') ||
                                        (#{hdnClaveTipoColectiva}.getValue() == 'GCM')) && #{hdnCatalogos}.getValue() == 1) { 
                                        #{FormPanelCatalogos}.setDisabled(false); } else { #{FormPanelCatalogos}.setDisabled(true); }">
                                        <EventMask ShowMask="true" Msg="Obteniendo Información de la Colectiva..." MinDelay="500" />
                                        <ExtraParams>
                                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridResultados}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                        </ExtraParams>
                                    </RowClick>
                                </DirectEvents>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreColectivas" DisplayInfo="false"
                                        HideRefresh="true" />
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
                                            <ext:FieldSet ID="FieldSetDatosAd" runat="server" Title="Datos Adicionales de la Colectiva" Layout="FormLayout">
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
                                                    <ext:TextField ID="txtTelefonoFijo" runat="server" FieldLabel="Teléfono Fijo" MaxLength="15" Width="500"
                                                        MaskRe="[0-9]" />
                                                    <ext:TextField ID="txtTelefonoCel" runat="server" FieldLabel="Teléfono Celular" MaxLength="15" Width="500"
                                                        MaskRe="[0-9]" />
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
                                            <ext:FieldSet ID="FieldSetDirecciones" runat="server" Title="Datos del Domicilio" Layout="FormLayout">
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
                                                    <ext:TextField ID="txtNumExterior" runat="server" FieldLabel="Número Exterior" MaxLength="50" Width="500" />
                                                    <ext:TextField ID="txtNumInterior" runat="server" FieldLabel="Número Interior" MaxLength="50" Width="500" />
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
                                        <Items>
                                            <ext:BorderLayout ID="BorderLayoutCuentas" runat="server">
                                                <North Split="true">
                                                    <ext:FormPanel runat="server" Height="170" Layout="FitLayout" Border="false">
                                                        <Content>
                                                            <ext:BorderLayout runat="server">
                                                                <Center Split="true">
                                                                    <ext:FormPanel ID="FormPanelNuevaCuenta" runat="server" LabelWidth="120" Layout="FitLayout" Border="false">
                                                                        <Items>
                                                                            <ext:FieldSet ID="FieldSetCuentas" runat="server" Title="Datos de la Nueva Cuenta" Layout="FormLayout">
                                                                                <Items>
                                                                                    <ext:ComboBox ID="cBoxTipoCuenta" runat="server" FieldLabel="Tipo de Cuenta" Width="240"
                                                                                        StoreID="StoreTipoCuenta" DisplayField="ClaveDescripcion" ValueField="ID_TipoCuenta"
                                                                                        AllowBlank="false">
                                                                                        <Listeners>
                                                                                            <Select Handler="var id = this.getValue(); var record = this.getStore().getById(id);
                                                                                                var texto = record.get('Descripcion'); #{txtDescCuenta}.setValue(texto);" />
                                                                                        </Listeners>
                                                                                    </ext:ComboBox>
                                                                                    <ext:TextField ID="txtDescCuenta" runat="server" FieldLabel="Descripción" Width="240"
                                                                                        AllowBlank="false" />
                                                                                    <ext:DateField ID="dfVigenciaCuenta" runat="server" FieldLabel="Vigencia" Vtype="daterange"
                                                                                        Format="dd/MM/yyyy" MaxLength="12" Width="240" EnableKeyEvents="true" />
                                                                                </Items>
                                                                                <Buttons>
                                                                                    <ext:Button ID="btnNuevaCuenta" runat="server" Text="Añadir Cuenta" Icon="Add" Disabled="true">
                                                                                        <DirectEvents>
                                                                                            <Click OnEvent="btnNuevaCuenta_Click" Before="if (!#{cBoxTipoCuenta}.validate() || 
                                                                                                !#{txtDescCuenta}.validate() || !#{dfVigenciaCuenta}.validate()) {return false};">
                                                                                                <EventMask ShowMask="true" Msg="Añadiendo Cuenta..." MinDelay="500" />
                                                                                            </Click>
                                                                                        </DirectEvents>
                                                                                    </ext:Button>
                                                                                </Buttons>
                                                                            </ext:FieldSet>
                                                                        </Items>
                                                                    </ext:FormPanel>
                                                                </Center>
                                                                <East Split="true">
                                                                    <ext:FormPanel ID="FormPanelCLABE" runat="server" LabelWidth="70" Layout="FitLayout" Border="false" Width="400">
                                                                        <Items>
                                                                            <ext:FieldSet ID="FieldSetCLABE" runat="server" Title="Datos de la Cuenta CLABE" Layout="FormLayout">
                                                                                <Items>
                                                                                    <ext:TextField ID="txtCuentaCLABE" runat="server" FieldLabel="CLABE" Width="290" AllowBlank="false" 
                                                                                        MaxLength="18" MinLength="18" MaskRe="[0-9]" />
                                                                                </Items>
                                                                                <Buttons>
                                                                                    <ext:Button ID="btnActualizaCLABE" runat="server" Text="Actualizar CLABE" Icon="Pencil">
                                                                                        <DirectEvents>
                                                                                            <Click OnEvent="btnActualizaCuentaCLABE_Click" Before="if (!#{txtCuentaCLABE}.validate()) { return false };">
                                                                                                <EventMask ShowMask="true" Msg="Actualizando Cuenta CLABE..." MinDelay="500" />
                                                                                            </Click>
                                                                                        </DirectEvents>
                                                                                    </ext:Button>
                                                                                </Buttons>
                                                                            </ext:FieldSet>
                                                                        </Items>
                                                                    </ext:FormPanel>
                                                                </East>
                                                            </ext:BorderLayout>
                                                        </Content>
                                                    </ext:FormPanel>
                                                </North>
                                                <Center Split="true">
                                                    <ext:GridPanel ID="GridCuentas" runat="server" Header="true" Title="Cuentas de la Colectiva" Border="false"
                                                        AutoExpandColumn="Descripcion">
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
                                                                <ext:CommandColumn Header="Acciones" Width="100">
                                                                    <PrepareToolbar Fn="prepareToolbar" />
                                                                    <Commands>
                                                                        <ext:GridCommand Icon="Pencil" CommandName="Edit">
                                                                            <ToolTip Text="Editar Cuenta" />
                                                                        </ext:GridCommand>
                                                                        <ext:GridCommand Icon="RecordGreen" CommandName="Lock">
                                                                            <ToolTip Text="Bloquear Cuenta" />
                                                                        </ext:GridCommand>
                                                                        <ext:GridCommand Icon="RecordRed" CommandName="Unlock">
                                                                            <ToolTip Text="Desbloquear Cuenta" />
                                                                        </ext:GridCommand>
                                                                    </Commands>
                                                                </ext:CommandColumn>
                                                                <ext:Column DataIndex="ID_Cuenta" Hidden="true" ColumnID="ID_Cuenta" />
                                                                <ext:Column DataIndex="ID_TipoCuenta" Hidden="true" ColumnID="ID_TipoCuenta" />
                                                                <ext:Column DataIndex="TipoCuenta" Header="Tipo de Cuenta" Width="120" />
                                                                <ext:Column DataIndex="Descripcion" Header="Descripción" ColumnID="Descripcion" Width="250" />
                                                                <ext:Column DataIndex="SaldoActual" Header="Saldo Actual" ColumnID="SaldoActual" Width="120">
                                                                    <Renderer Format="UsMoney" />
                                                                </ext:Column>
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
                                        </Items>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelParametros" runat="server" Title="Parámetros" Layout="FitLayout" Border="false">
                                        <Items>
                                            <ext:Panel runat="server" Layout="FitLayout" AutoScroll="true" Border="false">
                                                <TopBar>
                                                    <ext:Toolbar runat="server">
                                                        <Items>
                                                            <ext:ComboBox ID="cBoxClasificacion" runat="server" FieldLabel="Clasificación" Width="250"
                                                                DisplayField="Descripcion" ValueField="ID_ClasificacionParametros" AllowBlank="false"
                                                                ListWidth="250" LabelWidth="70">
                                                                <Store>
                                                                    <ext:Store ID="StoreClasificacion" runat="server">
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
                                                                        #{cBoxParametros}.getStore().removeAll(); #{GridPanelParametros}.getStore().removeAll();" >
                                                                        <EventMask ShowMask="true" Msg="Obteniendo Parámetros..." MinDelay="200" />
                                                                    </Select>
                                                                </DirectEvents>
                                                            </ext:ComboBox>
                                                            <ext:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                            <ext:ComboBox ID="cBoxParametros" runat="server" FieldLabel="Parámetros sin Asignar" Width="300"
                                                                DisplayField="Nombre" ValueField="ID_Parametro" Disabled="true" AllowBlank="false" LabelWidth="120"
                                                                ListWidth="300">
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
                                                        AutoHeight="true" Layout="FitLayout" MonitorResize="true" MonitorWindowResize="true" AutoExpandColumn="Nombre">
                                                        <Store>
                                                            <ext:Store ID="StoreValoresParametros" runat="server" AutoLoad="false">
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
                                    <ext:FormPanel ID="FormPanelParamsMA" runat="server" Title="Parámetros Adicionales" Layout="FitLayout" Border="false" Visible="false">
                                        <Items>
                                            <ext:Panel ID="PanelMA" runat="server" Layout="FitLayout" AutoScroll="true" Border="false">
                                                <TopBar>
                                                    <ext:Toolbar runat="server">
                                                        <Items>
                                                            <ext:Hidden ID="hdnIdParametroMA" runat="server" />
                                                            <ext:Hidden ID="hdnIdPlantilla" runat="server" />
                                                            <ext:Hidden ID="hdnIdValorPMA" runat="server" />
                                                            <ext:ComboBox ID="cBoxTipoParametroMA" runat="server" FieldLabel="Tipo de Parámetros" Width="350"
                                                                DisplayField="Descripcion" ValueField="ID_TipoParametroMultiasignacion" AllowBlank="false"
                                                                ListWidth="400" LabelWidth="100">
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
                                                                    <Select OnEvent="SeleccionaTipoPMA" Before="#{cBoxParametrosMA}.setDisabled(false);
                                                                        #{btnAddParametrosMA}.setDisabled(false); #{cBoxParametrosMA}.getStore().removeAll();
                                                                        #{cBoxParametrosMA}.reset(); #{GridPanelParametrosMA}.getStore().removeAll();">
                                                                        <EventMask ShowMask="true" Msg="Obteniendo Parámetros Adicionales..." MinDelay="200" />
                                                                    </Select>
                                                                </DirectEvents>
                                                            </ext:ComboBox>
                                                            <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                            <ext:ComboBox ID="cBoxParametrosMA" runat="server" FieldLabel="Parámetros sin Asignar" Width="300"
                                                                DisplayField="Nombre" ValueField="ID_ParametroMultiasignacion" Disabled="true" AllowBlank="false"
                                                                ListWidth="300" LabelWidth="120">
                                                                <Store>
                                                                    <ext:Store ID="StoreParametrosMA" runat="server">
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
                                                            <ext:Button ID="btnAddParametrosMA" runat="server" Text="Asignar Parámetro" Icon="Add" Disabled="true">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnAddParametrosMA_Click" Before="var valid= #{cBoxParametrosMA}.isValid(); if (!valid) {} return valid;">
                                                                        <EventMask ShowMask="true" Msg="Asignando Parámetro..." MinDelay="500" />
                                                                    </Click>
                                                                </DirectEvents>
                                                            </ext:Button>
                                                        </Items>
                                                    </ext:Toolbar>
                                                </TopBar>
                                                <Items>
                                                    <ext:GridPanel ID="GridPanelParametrosMA" runat="server" Header="true" Border="false" AutoScroll="true"
                                                        AutoHeight="true" Layout="FitLayout">
                                                        <Store>
                                                            <ext:Store ID="StoreValoresParametrosMA" runat="server">
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
                                                                            <ext:RecordField Name="esAutorizable" />
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
                                                            <Command OnEvent="EjecutarComandoParametrosMA">
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
                                    <ext:FormPanel ID="FormPanelCatalogos" runat="server" Title="Catálogos" LabelAlign="Left" LabelWidth="200" Disabled="true">
                                        <Content>
                                            <ext:BorderLayout runat="server">
                                                <North Split="true">
                                                    <ext:FormPanel ID="FormPanelNuevoCat" runat="server" LabelWidth="200" AutoHeight="true" Border="false" Layout="FormLayout">
                                                        <Items>
                                                            <ext:FieldSet ID="FieldSetCatalogos" runat="server" Title="Nuevo Elemento del Catálogo">
                                                                <Items>
                                                                    <ext:ComboBox ID="cBoxCatalogo" runat="server" FieldLabel="Catálogo   <span style='color:red;'>*   </span>"
                                                                        Width="500" AllowBlank="false" StoreID="StorePMAsCat" DisplayField="Descripcion" ValueField="ID_PMA"
                                                                        Editable="false" TabIndex="1"/>
                                                                    <ext:TextField ID="txtClaveCat" runat="server" FieldLabel="Clave    <span style='color:red;'>*   </span>"
                                                                        Width="500" AllowBlank="false" TabIndex="2"/>
                                                                    <ext:TextField ID="txtDescCat" runat="server" FieldLabel="Descripción   <span style='color:red;'>*   </span>"
                                                                        Width="500" AllowBlank="false" TabIndex="3"/>
                                                                </Items>
                                                                <Buttons>
                                                                    <ext:Button ID="btnAgregarCat" runat="server" Text="Añadir al Catálogo" Icon="Add">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="btnAgregarCat_Click" Before="if (!#{cBoxCatalogo}.validate() || !#{txtClaveCat}.validate() 
                                                                                || !#{txtDescCat}.validate()) {return false};">
                                                                                <EventMask ShowMask="true" Msg="Añadiendo elemento al Catálogo..." MinDelay="500" />
                                                                            </Click>
                                                                        </DirectEvents>
                                                                    </ext:Button>
                                                                </Buttons>
                                                            </ext:FieldSet>
                                                        </Items>
                                                    </ext:FormPanel>
                                                </North>
                                                <Center Split="true">
                                                    <ext:GridPanel ID="GridPanelCatalogo" runat="server" Header="true" Title="Catálogos de la Colectiva" Border="false"
                                                        AutoExpandColumn="Descripcion">
                                                        <Store>
                                                            <ext:Store ID="StoreElemsCat" runat="server">
                                                                <DirectEventConfig IsUpload="true" />
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="ID_ValorPreePMA">
                                                                        <Fields>
                                                                            <ext:RecordField Name="ID_ValorPreePMA" />
                                                                            <ext:RecordField Name="Clave" />
                                                                            <ext:RecordField Name="Descripcion" />
                                                                            <ext:RecordField Name="Activo" />
                                                                        </Fields>
                                                                    </ext:JsonReader>
                                                                </Reader>
                                                            </ext:Store>
                                                        </Store>
                                                        <TopBar>
                                                            <ext:Toolbar runat="server">
                                                                <Items>
                                                                    <ext:ToolbarFill runat="server" />
                                                                    <ext:ComboBox ID="cBoxCatGrid" runat="server" EmptyText="Selecciona el Catálogo..." AllowBlank="false"
                                                                        Width="200" Editable="false" StoreID="StorePMAsCat" ValueField="ID_PMA" DisplayField="Descripcion">
                                                                        <DirectEvents>
                                                                            <Select OnEvent="EstableceItemsCat" Before="if (!this.isValid()) { return false; } else {
                                                                                #{GridPanelCatalogo}.getStore().removeAll(); }">
                                                                                <EventMask ShowMask="true" Msg="Obteniendo Elementos del Catálogo..." MinDelay="200" />
                                                                            </Select>
                                                                        </DirectEvents>
                                                                    </ext:ComboBox>
                                                                </Items>
                                                            </ext:Toolbar>
                                                        </TopBar>
                                                        <ColumnModel runat="server">
                                                            <Columns>
                                                                <ext:Column DataIndex="ID_ValorPreePMA" Hidden="true" />
                                                                <ext:CommandColumn Width="30">
                                                                    <PrepareToolbar Fn="prepareCat" />
                                                                    <Commands>
                                                                        <ext:GridCommand Icon="RecordGreen" CommandName="Desact">
                                                                            <ToolTip Text="Desactivar" />
                                                                        </ext:GridCommand>
                                                                        <ext:GridCommand Icon="RecordRed" CommandName="Act">
                                                                            <ToolTip Text="Activar" />
                                                                        </ext:GridCommand>
                                                                    </Commands>
                                                                </ext:CommandColumn>
                                                                <ext:Column DataIndex="Clave" Header="Clave" Width="80"/>
                                                                <ext:Column DataIndex="Descripcion" Header="Descripción" Width="250" />
                                                            </Columns>
                                                        </ColumnModel>
                                                        <DirectEvents>
                                                            <Command OnEvent="ActDesacCat">
                                                                <Confirmation ConfirmRequest="true" Title="Confirmación"
                                                                    Message="¿Estás seguro de modificar el estatus del elemento del Catálogo?" />
                                                                <ExtraParams>
                                                                    <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                                                    <ext:Parameter Name="ID_ValorPreePMA" Value="Ext.encode(record.data.ID_ValorPreePMA)" Mode="Raw" />
                                                                    <ext:Parameter Name="Activo" Value="Ext.encode(record.data.Activo)" Mode="Raw" />
                                                                </ExtraParams>
                                                            </Command>
                                                        </DirectEvents>
                                                        <SelectionModel>
                                                            <ext:RowSelectionModel SingleSelect="true" />
                                                        </SelectionModel>
                                                        <BottomBar>
                                                            <ext:PagingToolbar ID="PagingToolBarCat" runat="server" StoreID="StoreElemsCat" HideRefresh="true" />
                                                        </BottomBar>
                                                    </ext:GridPanel>
                                                </Center>
                                            </ext:BorderLayout>
                                        </Content>
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
