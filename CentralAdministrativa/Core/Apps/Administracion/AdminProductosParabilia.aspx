<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="AdminProductosParabilia.aspx.cs" Inherits="Administracion.AdminProductosParabilia" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var fullName = function (value, metadata, record, rowIndex, colIndex, store) {
            return "<b>" + record.data.Nombre + "</b>";
        };

        var prepareToolbar = function (grid, toolbar, rowIndex, record) {
            if (record.get("esAutorizable") == 1) {
                toolbar.items.get(0).hide();
            }
        }

        var fnTipoMAExt = function (combo, clave, descripcion) {
            var title = 'Tipo de Medio de Acceso Externo';
            
            if (combo.getValue()) {
                if (clave.getValue() || descripcion.getValue()) {
                    Ext.Msg.alert(title, 'Por favor, elige entre un Tipo ya existente o uno nuevo, no ambos.');
                    return false;
                }
            } else {
                if (clave.getValue() && !descripcion.getValue()) {
                    Ext.Msg.alert(title, 'La Descripción del nuevo Tipo es obligatoria.');
                    return false;
                } else if (!clave.getValue() && descripcion.getValue()) {
                    Ext.Msg.alert(title, 'La Clave del nuevo Tipo es obligatoria.');
                    return false;
                } else if (!clave.getValue() && !descripcion.getValue()) {
                    Ext.Msg.alert(title, 'Por favor, elige entre un Tipo ya existente o uno nuevo.');
                    return false;
                }
            }

            return true;
        }

        var onKeyUp = function (field) {
            var v = this.processValue(this.getRawValue()),
                field;

            if (this.startDateField) {
                field = Ext.getCmp(this.startDateField);
                field.setMaxValue();
                this.dateRangeMax = null;
            } else if (this.endDateField) {
                field = Ext.getCmp(this.endDateField);
                field.setMinValue();
                this.dateRangeMin = null;
            }

            field.validate();
        };

        var estatusCampanya = function (grid, toolbar, rowIndex, record) {
            if (record.get("Activa") == 1) {
                toolbar.items.get(0).hide();
            } else {
                toolbar.items.get(1).hide();
            }
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
    <ext:Store ID="StoreTipoProducto" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_TipoProducto">
                <Fields>
                    <ext:RecordField Name="ID_TipoProducto" />
                    <ext:RecordField Name="Clave" />
                    <ext:RecordField Name="Descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
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
    <ext:Store ID="StoreTipoIntegracion" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_TipoIntegracion">
                <Fields>
                    <ext:RecordField Name="ID_TipoIntegracion" />
                    <ext:RecordField Name="ClaveTipoIntegracion" />
                    <ext:RecordField Name="Descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Store ID="StoreGruposMA" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_GrupoMA">
                <Fields>
                    <ext:RecordField Name="ID_GrupoMA" />
                    <ext:RecordField Name="ClaveGrupo" />
                    <ext:RecordField Name="Descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Store ID="StoreProdPadre" runat="server">
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
    <ext:Hidden ID="hdnNuevoTipoProd" runat="server" />
    <ext:Hidden ID="hdnGeneraMAExt" runat="server" />
    <ext:Hidden ID="hdnValidaTipoMAExt" runat="server" />
    <ext:Hidden ID="hdnGeneraMACLABE" runat="server" />
    <ext:Hidden ID="hdnValidaMACLABE" runat="server" />
    <ext:Hidden ID="hdnValorIniPMA" runat="server" />
    <ext:Hidden ID="hdnValorFinPMA" runat="server" />
    <ext:Window ID="WdwNuevoProducto" runat="server" Title="Nuevo Producto" Width="400" Height="390" Hidden="true"
        Modal="true" Resizable="false" Icon="Add">
        <Items>
            <ext:FormPanel ID="FormPanelNuevoProd" runat="server" Height="360" Padding="3" LabelAlign="Right" LabelWidth="120" Border="false">
                <Items>
                    <ext:FieldSet runat="server" Title="Producto">
                        <Items>
                            <ext:Hidden ID="hdnCatFlag" runat="server" />
                            <ext:Hidden ID="hdnClaveGpoMA" runat="server" />
                            <ext:ComboBox ID="cBoxColNuevoProd" runat="server" FieldLabel="<span style='color:red;'>*</span> Emisor"
                                Width="355" StoreID="StoreSubEmisores" ValueField="ID_Colectiva" DisplayField="NombreORazonSocial"
                                Mode="Local" AutoSelect="true" Editable="true" ForceSelection="true" TypeAhead="true" MinChars="1"
                                MatchFieldWidth="false" Name="colNuevoProd" AllowBlank="false">
                                <DirectEvents>
                                    <Select OnEvent="Emisor_Select" Before="#{cBoxTipoProducto}.clearValue(); #{cBoxDisTarj}.clearValue();">
                                        <EventMask ShowMask="true" Msg="Estableciendo Tipos de Producto..." MinDelay="200" />
                                    </Select>
                                </DirectEvents>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cBoxTipoProducto" runat="server" FieldLabel="<span style='color:red;'>*</span> Tipo de Producto"
                                Width="355" AllowBlank="false" StoreID="StoreTipoProducto" DisplayField="Descripcion" ListWidth ="450" 
                                ValueField="ID_TipoProducto">
                                <DirectEvents>
                                    <Select OnEvent="TipoProducto_Select" Before="#{hdnNuevoTipoProd}.clear();
                                        #{hdnNuevoTipoProd}.setValue(this.getValue()); #{cBoxProdPadre}.clearValue();"
                                        After="#{chkAdicional}.setSelectable(true);" />
                                </DirectEvents>
                            </ext:ComboBox>
                            <ext:TextField ID="txtClaveProd" runat="server" FieldLabel="<span style='color:red;'>*</span> Clave de Producto"
                                MaxLength="10" Width="355" AllowBlank="false" />
                            <ext:TextField ID="txtDescProd" runat="server" FieldLabel="<span style='color:red;'>*</span> Descripción"
                                MaxLength="50" Width="355" AllowBlank="false" />
                            <ext:ComboBox ID="cBoxTipoIntegracion" runat="server" FieldLabel="<span style='color:red;'>*</span> Tipo de Integración"
                                Width="355" StoreID="StoreTipoIntegracion" ValueField="ID_TipoIntegracion" DisplayField="Descripcion"
                                AllowBlank="false" Editable="false" />
                            <ext:MultiCombo ID="mcGrupoMA" runat="server" FieldLabel="<span style='color:red;'>*</span> Medios de Acceso"
                                Width="355" StoreID="StoreGruposMA" DisplayField="Descripcion" ValueField="ID_GrupoMA" AllowBlank="false"
                                Editable="false">
                                <Listeners>
                                    <Select Handler="var id = this.getValue(); var record = this.getStore().getById(id);
                                        if (record.get('ClaveGrupo') == #{hdnClaveGpoMA}.getValue() && #{hdnCatFlag}.getValue() == 1)
                                        { #{cBoxDisTarj}.setDisabled(false); #{cBoxDisTarj}.allowBlank = false; } else {
                                        #{cBoxDisTarj}.setDisabled(true); #{cBoxDisTarj}.allowBlank = true; }" />
                                </Listeners>
                            </ext:MultiCombo>
                            <ext:ComboBox ID="cBoxDisTarj" runat="server" FieldLabel="Diseño de Tarjetas" Width="355" ValueField="ID_ValorPreePMA"
                                StoreID="StoreCatalogoPMA" DisplayField="Descripcion" Editable="false" Disabled="true" />
                            <ext:Checkbox ID="chkAdicional" runat="server" FieldLabel="Es Adicional" Selectable="false">
                                <Listeners>
                                    <AfterRender Handler="this.el.on('change', function (e, el) {
                                        var estatus = el.checked ? false : true;
                                        #{cBoxProdPadre}.setDisabled(estatus);
                                        if (estatus) { #{cBoxProdPadre}.clearValue(); }
                                        });" />
                                </Listeners>
                            </ext:Checkbox>
                        </Items>
                    </ext:FieldSet>
                    <ext:FieldSet runat="server" Title="Producto Titular">
                        <Items>
                            <ext:ComboBox ID="cBoxProdPadre" runat="server" FieldLabel="Producto" Width="355"
                                StoreID="StoreProdPadre" DisplayField="Descripcion" ValueField="ID_Producto"
                                Disabled="true" Name="prodsPadreNP"/>
                        </Items>
                    </ext:FieldSet>
                </Items>
                <Buttons>
                    <ext:Button runat="server" Text="Cancelar" Icon="Cancel">
                        <Listeners>
                            <Click Handler="#{WdwNuevoProducto}.hide();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button ID="btnNuevoProducto" runat="server" Text="Aceptar" Icon="Tick">
                        <DirectEvents>
                            <Click OnEvent="btnNuevoProducto_Click" Before="
                                var valid= #{FormPanelNuevoProd}.getForm().isValid();
                                if (!valid) { return false; } else {
                                    if (#{chkAdicional}.checked && !#{cBoxProdPadre}.getValue()) {
                                        Ext.MessageBox.show({
                                        icon: Ext.MessageBox.WARNING,
                                        msg: 'Si el nuevo producto es adicional, debes seleccionar el Producto Titular.',
                                        buttons: Ext.MessageBox.OK
                                        });
                                        return false;
                                    }
                                }" />
                        </DirectEvents>
                    </ext:Button>
                    <ext:Button ID="btnValidaCtaExt" runat="server" Hidden="true">
                        <DirectEvents>
                            <Click OnEvent="TiposMAExternos" />
                        </DirectEvents>
                    </ext:Button>
                    <ext:Button ID="btnValidaCtaCLABE" runat="server" Hidden="true">
                        <DirectEvents>
                            <Click OnEvent="EstableceParametrosCLABE" />
                        </DirectEvents>
                    </ext:Button>
                    <ext:Button ID="btnCreaProducto" runat="server" Hidden="true">
                        <DirectEvents>
                            <Click OnEvent="CreaNuevoProducto"
                                After="Ext.net.Mask.show({ msg : 'Generando Nuevo Producto...' });
                                AdminProdParabilia.StopMask();" />
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:Window ID="WdwTipoMAExterno" runat="server" Width="350" Height="260" Title="Tipo de Medio de Acceso Externo"
        Hidden="true" Modal="true" Resizable="false">
        <Items>
            <ext:FormPanel runat="server" Padding="10" LabelWidth="100" LabelAlign="Right" Border="false" Layout="FormLayout">
                <Items>
                    <ext:FieldSet runat="server" Title="Selecciona un Tipo para asociarlo al Producto...">
                        <Items>
                            <ext:ComboBox ID="cBoxTiposMAExternos" runat="server" FieldLabel="Tipos Existentes" Width="290"
                                BlankText="Seleciona un Tipo..." DisplayField="Descripcion" ValueField="Clave">
                                <Store>
                                    <ext:Store ID="StoreTipoMAExterno" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_TipoMA">
                                                <Fields>
                                                    <ext:RecordField Name="ID_TipoMA" />
                                                    <ext:RecordField Name="Clave" />
                                                    <ext:RecordField Name="Descripcion" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <Triggers>
                                    <ext:FieldTrigger Icon="Clear" HideTrigger="true" />
                                </Triggers>
                                <Listeners>
                                    <BeforeQuery Handler="this.triggers[0][ this.getRawValue().toString().length == 0 ? 'hide' : 'show']();" />
                                    <TriggerClick Handler="if (index == 0) { this.focus().clearValue(); trigger.hide();}" />
                                    <Select Handler="this.triggers[0].show();" />
                                </Listeners>
                            </ext:ComboBox>
                        </Items>
                    </ext:FieldSet>
                    <ext:FieldSet runat="server" Title="... O crea un nuevo Tipo">
                        <Items>
                            <ext:TextField ID="txtClaveTMAExt" runat="server" FieldLabel="Clave" MaxLength="10" Width="290" />
                            <ext:TextField ID="txtDescTMAExt" runat="server" FieldLabel="Descripción" MaxLength="50" Width="290" />
                        </Items>
                    </ext:FieldSet>
                </Items>
                <Buttons>
                    <ext:Button runat="server" Text="Cancelar" Icon="Cancel">
                        <Listeners>
                            <Click Handler="#{hdnValidaTipoMAExt}.setValue(''); #{hdnGeneraMAExt}.setValue('');
                                #{WdwTipoMAExterno}.hide(); #{WdwNuevoProducto}.hide();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button ID="btnTipoMAExt" runat="server" Text="Aceptar" Icon="Tick">
                        <Listeners>
                            <Click Handler="if(fnTipoMAExt(#{cBoxTiposMAExternos}, #{txtClaveTMAExt}, #{txtDescTMAExt})) {
                                if (#{hdnValidaMACLABE}.getValue() == true && !#{hdnGeneraMACLABE}.getValue())
                                {
                                    Ext.Msg.confirm('Tipos de Medios de Acceso',
                                    '¿El nuevo producto generará un Medio de Acceso CLABE al crear nuevas cuentas?',
                                        function (btn) {
                                            if (btn == 'yes') {
                                                AdminProdParabilia.CuentaCLABEDePaso(true);
                                            } else {
                                                AdminProdParabilia.CuentaCLABEDePaso(false);
                                            }
                                        });
                                } else {
                                    AdminProdParabilia.CreaProductoDePaso();
                                }       
                                }" />
                        </Listeners>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:Window ID="WdwTipoMACLABE" runat="server" Width="400" Height="250" Title="Tipo de Medio de Acceso CLABE"
        Hidden="true" Modal="true" Resizable="false" Layout="FitLayout">
        <Items>
            <ext:PropertyGrid ID="ParametrosCLABE" runat="server" Title="Captura todos los Valores (son obligatorios)">
                <Source>
                    <ext:PropertyGridParameter Name="(Parámetros)" Value="Valores">
                    </ext:PropertyGridParameter>
                </Source>
                <View>
                    <ext:GridView ID="GridView1" ForceFit="true" ScrollOffset="2" runat="server" />
                </View>
            </ext:PropertyGrid>
        </Items>
        <Buttons>
            <ext:Button runat="server" Text="Cancelar" Icon="Cancel">
                <Listeners>
                    <Click Handler="#{hdnValidaMACLABE}.setValue(''); #{hdnGeneraMACLABE}.setValue('');
                        #{WdwTipoMACLABE}.hide(); #{WdwTipoMAExterno}.hide();
                        #{WdwNuevoProducto}.hide();" />
                </Listeners>
            </ext:Button>
            <ext:Button runat="server" Text="Aceptar" Icon="Tick">
                <Listeners>
                    <Click Handler="if (#{hdnValidaTipoMAExt}.getValue() == true && 
                        !#{hdnGeneraMAExt}.getValue()) {
                            Ext.Msg.confirm('Tipos de Medios de Acceso',
                            '¿El nuevo producto generará un Medio de Acceso Externo al crear nuevas cuentas?',
                                function (btn) {
                                    if (btn == 'yes') {
                                        AdminProdParabilia.CuentaEXTDePaso(true);
                                    } else {
                                        AdminProdParabilia.CuentaEXTDePaso(false);
                                    }
                                });
                        } else {
                            AdminProdParabilia.EstableceValoresParametrosCLABE();
                        }" />
                </Listeners>
            </ext:Button>
        </Buttons>
    </ext:Window>
    <ext:Window ID="WdwEditarBIN" runat="server" Title="Editar Bin" Icon="Pencil" Width="420" Height="190"
        Hidden="true" Modal="true" Resizable="false">
        <Items>
            <ext:FormPanel ID="FormPanelEditarBin" runat="server" Padding="10" LabelWidth="80" Border="false"
                Layout="FormLayout" LabelAlign="Right">
                <Items>
                    <ext:Hidden ID="hdnIdBin" runat="server" />
                    <ext:TextField ID="txtClaveBin" runat="server" FieldLabel="Bin   <span style='color:red;'>*   </span>"
                        AllowBlank="false" Width="300" MaxLength="20" MaskRe="[0-9]"/>
                    <ext:TextArea ID="txtDescBin" runat="server" FieldLabel="Observaciones" MaxLength="200" Width="300"
                        Height="70" />
                </Items>
                <Buttons>
                    <ext:Button runat="server" Text="Cancelar" Icon="Cancel">
                        <Listeners>
                            <Click Handler="#{WdwEditarBIN}.hide();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button ID="btnGuardarEdicionBin" runat="server" Text="Guardar Cambios" Icon="Disk">
                        <DirectEvents>
                            <Click OnEvent="btnGuardarEdicionBin_Click" Before="var valid= #{FormPanelEditarBin}.getForm().isValid(); if (!valid) {} return valid;">
                                <EventMask ShowMask="true" Msg="Guardando Cambios..." MinDelay="500" />
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:Window ID="WdwValorParametro" runat="server" Title="Editar Valor Parámetro" Width="420" AutoHeight="true" Hidden="true"
        Resizable="false">
        <Items>
            <ext:FormPanel ID="FormPanelValorParamTxt" runat="server" Padding="10" MonitorValid="true" LabelAlign="Left" LabelWidth="70">
                <Items>
                    <ext:TextField ID="txtParametro" runat="server" FieldLabel="Descripción" Width="300"
                        AllowBlank="false" Selectable="false" ReadOnly="true" />
                    <ext:TextField ID="txtValorParFloat" runat="server" FieldLabel="Valor" Width="300" MaxLength="50"
                        MaskRe="[0-9\.]" Hidden="true" Regex="^-?[0-9]*(\.[0-9]{1,4})?$">
                        <Listeners>
                            <Change Handler="var inicial = parseFloat(#{hdnValorIniPMA}.getValue());
                                var final = parseFloat(#{hdnValorFinPMA}.getValue());
                                var actual = parseFloat(this.getValue());
                                var _vmsg = 'El valor del parámetro debe estar entre ' + inicial + ' y ' + final;
                                if ((actual < inicial || actual > final)) {
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
                    <ext:TextField ID="txtValorParInt" runat="server" FieldLabel="Valor" Width="300" MaxLength="50"
                        MaskRe="[0-9]" Hidden="true">
                        <Listeners>
                            <Change Handler="var inicial = parseInt(#{hdnValorIniPMA}.getValue());
                                var final = parseInt(#{hdnValorFinPMA}.getValue());
                                var actual = parseInt(this.getValue());
                                var _vmsg = 'El valor del parámetro debe estar entre ' + inicial + ' y ' + final;
                                if (actual < inicial || actual > final) {
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
                    <ext:TextArea ID="txtValorParString" runat="server" FieldLabel="Valor" Width="300" AutoHeight="true"
                        MaxLength="5000" Hidden="true" />
                    <ext:ComboBox ID="cBoxValorPar" runat="server" FieldLabel="Valor" Width="300" Hidden="true">
                        <Items>
                            <ext:ListItem Text="Sí" Value="true" />
                            <ext:ListItem Text="No" Value="false" />
                        </Items>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cBoxCatalogoPMA" runat="server" FieldLabel="Valor" Width="300" Hidden="true"
                        StoreID="StoreCatalogoPMA" ValueField="ID_ValorPreePMA" DisplayField="Descripcion" />
                </Items>
                <Buttons>
                    <ext:Button runat="server" Text="Cancelar" Icon="Cancel">
                        <Listeners>
                            <Click Handler="#{WdwValorParametro}.hide();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button runat="server" Text="Guardar Cambio" Icon="Disk">
                        <DirectEvents>
                            <Click OnEvent="btnGuardarValorParametro_Click"
                                Before="if ((#{txtValorParFloat}.hidden == false) && (!#{txtValorParFloat}.isValid())) { return false; }" />
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:Window ID="WdwEditarSubproducto" runat="server" Title="Editar Subproducto" Icon="Pencil" Width="420" Height="190"
        Hidden="true" Modal="true" Resizable="false">
        <Items>
            <ext:FormPanel ID="FormPanelEditSubProd" runat="server" Padding="10" LabelWidth="80" Border="false"
                Layout="FormLayout" LabelAlign="Right">
                <Items>
                    <ext:Hidden ID="hndIdSubP" runat="server" />
                    <ext:TextField ID="txtClaveEdtSubP" runat="server" FieldLabel="Clave   <span style='color:red;'>*   </span>"
                        AllowBlank="false" Width="300" MaxLength="10" />
                    <ext:TextArea ID="txtDescEdtSubP" runat="server" FieldLabel="Descripción   <span style='color:red;'>*   </span>"
                        MaxLength="200" Width="300" Height="70" />
                </Items>
                <Buttons>
                    <ext:Button runat="server" Text="Cancelar" Icon="Cancel">
                        <Listeners>
                            <Click Handler="#{WdwEditarSubproducto}.hide();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button ID="btnEditSubProd" runat="server" Text="Guardar Cambios" Icon="Disk">
                        <DirectEvents>
                            <Click OnEvent="btnEditSubProd_Click" Before="var valid= #{FormPanelEditSubProd}.getForm().isValid(); if (!valid) {} return valid;">
                                <EventMask ShowMask="true" Msg="Guardando Cambios..." MinDelay="500" />
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:Window ID="WdwCampanya" runat="server" Width="400" AutoHeight="true" Hidden="true" Modal="true" Resizable="false">
        <Items>
            <ext:FormPanel ID="FormPanelEdNewCampanya" runat="server" Padding="10" LabelWidth="85" Border="false"
                Layout="FormLayout" LabelAlign="Right">
                <Items>
                    <ext:Hidden ID="hdnIdCampanya" runat="server" />
                    <ext:TextField ID="txtClaveCamp" runat="server" FieldLabel="Clave   <span style='color:red;'>*   </span>"
                        AllowBlank="false" Width="270" MaxLength="20" />
                    <ext:TextField ID="txtDescripcionCamp" runat="server" FieldLabel="Descripción   <span style='color:red;'>*   </span>" 
                        AllowBlank="false" MaxLength="100" Width="270" />
                    <ext:Panel runat="server" Layout="FitLayout" Width="300" Height="5" Border="false" />
                    <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false">
                        <Defaults>
                            <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                        </Defaults>
                        <LayoutConfig>
                            <ext:HBoxLayoutConfig Align="Top" />
                        </LayoutConfig>
                        <Items>
                            <ext:DateField ID="dfFIniCampanya" runat="server" Vtype="daterange" FieldLabel="Fecha Inicio   <span style='color:red;'>*   </span>"
                                AllowBlank="false" Editable="false" MsgTarget="Side" Format="dd/MM/yyyy" Width="180">
                                <CustomConfig>
                                    <ext:ConfigItem Name="endDateField" Value="#{dfFFinCampanya}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:Hidden runat="server" Width="1" />
                            <ext:DateField ID="dfFFinCampanya" runat="server" Vtype="daterange" FieldLabel="Fecha Fin   <span style='color:red;'>*   </span>"
                                AllowBlank="false" Editable="false" Width="180" MsgTarget="Side" Format="dd/MM/yyyy">
                                <CustomConfig>
                                    <ext:ConfigItem Name="startDateField" Value="#{dfFIniCampanya}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                        </Items>
                    </ext:Panel>
                </Items>
                <Buttons>
                    <ext:Button runat="server" Text="Cancelar" Icon="Cancel">
                        <Listeners>
                            <Click Handler="#{WdwCampanya}.hide();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button ID="btnGuardarCampanya" runat="server" Text="Guardar Cambios" Icon="Disk">
                        <DirectEvents>
                            <Click OnEvent="btnGuardarCampanya_Click" Before="var valid= #{FormPanelEdNewCampanya}.getForm().isValid();
                                if (!valid) {} return valid;">
                                <EventMask ShowMask="true" Msg="Guardando Cambios..." MinDelay="500" />
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
     <ext:Window ID="WdwCfgCampanya" runat="server" Width="400" AutoHeight="true" Hidden="true" Modal="true" Resizable="false">
        <Items>
            <ext:FormPanel ID="FormPanelCampCfg" runat="server" Padding="10" LabelWidth="100" Border="false"
                Layout="FormLayout" LabelAlign="Right">
                <Items>
                    <ext:Hidden ID="hdnIdCampCfg" runat="server" />
                    <ext:Hidden ID="hdnIdPromocion" runat="server" />
                    <ext:Hidden ID="hdnClavePromo" runat="server" />
                    <ext:ComboBox ID="cBoxPromociones" runat="server" FieldLabel="<span style='color:red;'>*   </span>Promociones" 
                        AllowBlank="false" Width="250" DisplayField="Descripcion" ValueField="ID_Promocion" Editable="false">
                        <Store>
                            <ext:Store ID="StorePromociones" runat="server">
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_Promocion">
                                        <Fields>
                                            <ext:RecordField Name="ID_Promocion" />
                                            <ext:RecordField Name="Clave" />
                                            <ext:RecordField Name="Descripcion" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <Listeners>
                            <Change Handler="var id = this.getValue(); var record = this.getStore().getById(id);
                                #{hdnClavePromo}.setValue(record.get('Clave'));" />
                        </Listeners>
                    </ext:ComboBox>
                    <ext:TextField ID="txtMeses" runat="server" FieldLabel="<span style='color:red;'>*   </span>Meses"
                        Width="250" MaxLength="2" MaskRe="[0-9]" AllowBlank="false">
                        <Listeners>
                            <Change Handler="var inicial = parseInt(3);
                                var final = parseInt(48);
                                var actual = parseInt(this.getValue());
                                var _vmsg = 'El número de meses debe estar entre ' + inicial + ' y ' + final;
                                if (actual < inicial || actual > final) {
                                    this.clear();
                                    Ext.MessageBox.show({
                                        icon: Ext.MessageBox.WARNING,
                                        msg: _vmsg,
                                        buttons: Ext.MessageBox.OK,
                                        });
                                    return false; }" />
                        </Listeners>
                    </ext:TextField>
                    <ext:TextField ID="txtTasaInteres" runat="server" FieldLabel="<span style='color:red;'>*   </span>Tasa de Interés"
                        Width="250" MaxLength="6" MaskRe="[0-9\.]" Regex="^-?[0]*(\.[0-9]{1,4})?$" AllowBlank="false">
                        <Listeners>
                            <Change Handler="var inicial = parseFloat(0);
                                var final = parseFloat(0.99);
                                var actual = parseFloat(this.getValue());
                                var _vmsg = 'El valor de la tasa de interés debe estar entre ' + inicial + '% y ' + final + '%';
                                if ((actual < inicial || actual > final)) {
                                    this.clear();
                                    Ext.MessageBox.show({
                                        icon: Ext.MessageBox.WARNING,
                                        msg: _vmsg,
                                        buttons: Ext.MessageBox.OK,
                                        });
                                    return false; }" />
                        </Listeners>
                    </ext:TextField>
                    <ext:TextField ID="txtDiferimiento" runat="server" FieldLabel="<span style='color:red;'>*   </span>Diferimiento"
                        Width="250" MaxLength="2" MaskRe="[0-9]" AllowBlank="false">
                        <Listeners>
                            <Change Handler="var inicial = parseInt(0);
                                var final = parseInt(12);
                                var actual = parseInt(this.getValue());
                                var _vmsg = 'El valor del diferimiento debe estar entre ' + inicial + ' y ' + final;
                                if (actual < inicial || actual > final) {
                                    this.clear();
                                    Ext.MessageBox.show({
                                        icon: Ext.MessageBox.WARNING,
                                        msg: _vmsg,
                                        buttons: Ext.MessageBox.OK,
                                        });
                                    return false; }" />
                        </Listeners>
                    </ext:TextField>
                </Items>
                <Buttons>
                    <ext:Button runat="server" Text="Cancelar" Icon="Cancel">
                        <Listeners>
                            <Click Handler="#{WdwCfgCampanya}.hide();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button ID="btnGuardarCfgCamp" runat="server" Text="Guardar Cambios" Icon="Disk">
                        <DirectEvents>
                            <Click OnEvent="btnGuardarCfgCamp_Click" Before="var valid= #{FormPanelCampCfg}.getForm().isValid();
                                if (!valid) {} return valid;">
                                <EventMask ShowMask="true" Msg="Guardando Cambios..." MinDelay="500" />
                            </Click>
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
                                            <ext:Button ID="btnLimpiarIzq" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                                <DirectEvents>
                                                    <Click OnEvent="btnLimpiarIzq_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:ToolbarFill runat="server" />
                                            <ext:Button runat="server" Icon="Add" ToolTip="Crear Nuevo Producto"
                                                Text="Nuevo Producto">
                                                <Listeners>
                                                    <Click Handler="#{FormPanelNuevoProd}.reset(); #{chkAdicional}.setSelectable(false);
                                                        #{hdnValidaTipoMAExt}.setValue(); #{hdnGeneraMAExt}.setValue();
                                                        #{hdnValidaMACLABE}.setValue(); #{hdnGeneraMACLABE}.setValue();
                                                        #{hdnCatFlag}.setValue(); #{hdnClaveGpoMA}.setValue();
                                                        #{cBoxDisTarj}.getStore().removeAll();
                                                        #{WdwNuevoProducto}.show();" />
                                                </Listeners>
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
                                                MinChars="1" MatchFieldWidth="false" Name="colSubEmisor" ListWidth="300" />
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
                                    <ext:FormPanel ID="FormPanelInfoAd_Prod" runat="server" Title="Información General" AutoScroll="true" Border="false">
                                        <Content>
                                            <ext:BorderLayout ID="BorderLayout2" runat="server">
                                                <Center Split="true">
                                                    <ext:FormPanel ID="FormPanelDataInfoAdProd" runat="server" LabelAlign="Left" LabelWidth="150">
                                                        <Items>
                                                            <ext:FieldSet runat="server" Title="Datos Generales del Producto" Layout="FormLayout">
                                                                <Items>
                                                                    <ext:Hidden ID="hdnIdProducto" runat="server" />                                                                    
                                                                    <ext:ComboBox ID="cBoxColectiva" runat="server" FieldLabel="<span style='color:red;'>*</span> Emisor"
                                                                        Width="540" StoreID="StoreSubEmisores" ValueField="ID_Colectiva" DisplayField="NombreORazonSocial"
                                                                        Mode="Local" AutoSelect="true" Editable="true" ForceSelection="true" TypeAhead="true" MinChars="1"
                                                                        MatchFieldWidth="false" Name="colProducto" AllowBlank="false" >
                                                                            <DirectEvents>
                                                                                <Select OnEvent="Emisor_Select_Edit" Before="#{cBoxTipoProd}.clearValue();"/>
                                                                            </DirectEvents>
                                                                    </ext:ComboBox>
                                                                    <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false">
                                                                        <Defaults>
                                                                            <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                                        </Defaults>
                                                                        <LayoutConfig>
                                                                            <ext:HBoxLayoutConfig Align="Top" />
                                                                        </LayoutConfig>
                                                                        <Items>
                                                                            <ext:ComboBox ID="cBoxTipoProd" runat="server" FieldLabel="<span style='color:red;'>*</span> Tipo de Producto"
                                                                                Width="500" AllowBlank="false" StoreID="StoreTipoProducto" DisplayField="Descripcion"
                                                                                ValueField="ID_TipoProducto" />
                                                                            <ext:Hidden runat="server" Width="20" />
                                                                            <ext:TextField ID="txtClaveProducto" runat="server" FieldLabel="Clave" LabelWidth="30"
                                                                                Width="175" Disabled="true" />
                                                                        </Items>
                                                                    </ext:Panel>
                                                                    <ext:Panel runat="server" Layout="FitLayout" Width="540" Height="5" Border="false" />
                                                                    <ext:TextField ID="txtDescripcionProd" runat="server" FieldLabel="<span style='color:red;'>*</span> Nombre o Descripción"
                                                                        MaxLength="50" Width="540" AllowBlank="false" BlankText="La Descripción es Obligatoria" />
                                                                    <ext:ComboBox ID="cBoxTipoIntegProd" runat="server" FieldLabel="<span style='color:red;'>*</span> Tipo de Integración"
                                                                        Width="540" StoreID="StoreTipoIntegracion" ValueField="ID_TipoIntegracion" DisplayField="Descripcion"
                                                                        AllowBlank="false" />
                                                                    <ext:MultiCombo ID="mcGrupoMAProd" runat="server" FieldLabel="<span style='color:red;'>*</span> Medios de Acceso"
                                                                        Width="540" StoreID="StoreGruposMA" DisplayField="Descripcion" ValueField="ID_GrupoMA" AllowBlank="false"
                                                                        Editable="false" />
                                                                    <ext:Checkbox ID="chkBoxEsAdicional" runat="server" FieldLabel="Es Adicional">
                                                                        <Listeners>
                                                                            <AfterRender Handler="this.el.on('change', function (e, el) {
                                                                                var estatus = el.checked ? false : true;
                                                                                #{cBoxProductoPadre}.setDisabled(estatus);
                                                                                if (estatus) { #{cBoxProductoPadre}.clearValue(); }
                                                                                });" />
                                                                        </Listeners>
                                                                    </ext:Checkbox>
                                                                    <ext:ComboBox ID="cBoxProductoPadre" runat="server" FieldLabel="Producto Titular" Width="540"
                                                                        StoreID="StoreProdPadre" DisplayField="Descripcion" ValueField="ID_Producto"
                                                                        Disabled="true" Name="prodsPadre" />
                                                                </Items>
                                                                <Buttons>
                                                                    <ext:Button ID="btnGuardarInfoAd_Prod" runat="server" Text="Guardar" Icon="Disk">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="btnGuardarInfoAd_Prod_Click"
                                                                                Before="var valid= #{FormPanelDataInfoAdProd}.getForm().isValid(); 
                                                                                    if (!valid) { return false; } else {
                                                                                        if (#{chkBoxEsAdicional}.checked && !#{cBoxProductoPadre}.getValue()) {
                                                                                            Ext.MessageBox.show({
                                                                                            icon: Ext.MessageBox.WARNING,
                                                                                            msg: 'Si el Producto será Adicional, debes seleccionar el Producto Titular.',
                                                                                            buttons: Ext.MessageBox.OK
                                                                                            });
                                                                                            return false;
                                                                                        }
                                                                                    }">
                                                                                <EventMask ShowMask="true" Msg="Guardando Información..." MinDelay="500" />
                                                                            </Click>
                                                                        </DirectEvents>
                                                                    </ext:Button>
                                                                </Buttons>
                                                            </ext:FieldSet>
                                                        </Items>
                                                    </ext:FormPanel>
                                                </Center>
                                                <South Split="true">
                                                    <ext:GridPanel ID="GridTiposCuenta" runat="server" Layout="FitLayout" Height="160" Border="false"
                                                        Header="true" Title="Tipos de Cuenta" AutoExpandColumn="Descripcion">
                                                        <Store>
                                                            <ext:Store ID="StoreTiposCuenta" runat="server">
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="ID_TipoCuenta">
                                                                        <Fields>
                                                                            <ext:RecordField Name="ID_TipoCuenta" />
                                                                            <ext:RecordField Name="ClaveTipoCuenta" />
                                                                            <ext:RecordField Name="Descripcion" />
                                                                        </Fields>
                                                                    </ext:JsonReader>
                                                                </Reader>
                                                            </ext:Store>
                                                        </Store>
                                                        <ColumnModel runat="server">
                                                            <Columns>
                                                                <ext:Column DataIndex="ID_TipoCuenta" Hidden="true" />
                                                                <ext:Column DataIndex="ClaveTipoCuenta" Header="Clave" Width="120" />
                                                                <ext:Column DataIndex="Descripcion" Header="Descripción" Width="400 " />
                                                            </Columns>
                                                        </ColumnModel>
                                                        <SelectionModel>
                                                            <ext:RowSelectionModel SingleSelect="true" />
                                                        </SelectionModel>
                                                        <BottomBar>
                                                            <ext:PagingToolbar ID="PagingToolBar6" runat="server" StoreID="StoreTiposCuenta" DisplayInfo="true"
                                                                DisplayMsg="{0} - {1} de {2}" HideRefresh="true" />
                                                        </BottomBar>
                                                    </ext:GridPanel>
                                                </South>
                                                </ext:BorderLayout>
                                        </Content>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelEventos" runat="server" Title="Eventos" Layout="FitLayout">
                                        <Content>
                                            <ext:BorderLayout ID="BorderLayout1" runat="server">
                                                <Center Split="true">
                                                    <ext:GridPanel ID="GridEventos" runat="server" Header="true" Layout="FitLayout" Border="false"
                                                        AutoExpandColumn="Descripcion">
                                                        <Store>
                                                            <ext:Store ID="StoreEventos" runat="server">
                                                                <DirectEventConfig IsUpload="true" />
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="ID_Evento">
                                                                        <Fields>
                                                                            <ext:RecordField Name="ID_Evento" />
                                                                            <ext:RecordField Name="TipoEvento" />
                                                                            <ext:RecordField Name="ClaveEvento" />
                                                                            <ext:RecordField Name="Descripcion" />
                                                                            <ext:RecordField Name="DescripcionEdoCta" />
                                                                        </Fields>
                                                                    </ext:JsonReader>
                                                                </Reader>
                                                                <DirectEventConfig IsUpload="true" />
                                                            </ext:Store>
                                                        </Store>
                                                        <ColumnModel runat="server">
                                                            <Columns>
                                                                <ext:Column DataIndex="ID_Evento" Hidden="true" />
                                                                <ext:Column DataIndex="ClaveEvento" Header="Clave" Width="70" />
                                                                <ext:Column DataIndex="TipoEvento" Header="Tipo de Evento" Width="120" />
                                                                <ext:Column DataIndex="DescripcionEdoCta" Header="Descripción Estado de Cuenta" Width="220" />
                                                                <ext:Column DataIndex="Descripcion" Header="Descripción Interna" Width="120" />
                                                                <ext:CommandColumn Width="30">
                                                                     <Commands>
                                                                         <ext:GridCommand Icon="PageWhiteMagnify" CommandName="Select">
                                                                             <ToolTip Text="Ver Detalles" />
                                                                         </ext:GridCommand>
                                                                     </Commands>
                                                                 </ext:CommandColumn>
                                                            </Columns>
                                                        </ColumnModel>
                                                        <DirectEvents>
                                                            <Command OnEvent="EjecutarComandoEventos">
                                                                <ExtraParams>
                                                                    <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                                                    <ext:Parameter Name="ID_Evento" Value="Ext.encode(record.data.ID_Evento)" Mode="Raw" />
                                                                </ExtraParams>
                                                            </Command>
                                                        </DirectEvents>
                                                        <SelectionModel>
                                                            <ext:RowSelectionModel SingleSelect="true" />
                                                        </SelectionModel>
                                                        <BottomBar>
                                                            <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="StoreEventos" DisplayInfo="true"
                                                                DisplayMsg="{0} - {1} de {2}" HideRefresh="true" PageSize="5" />
                                                        </BottomBar>
                                                    </ext:GridPanel>
                                                </Center>
                                                <South Split="true">
                                                    <ext:FormPanel ID="FormDatosEvento" runat="server" Layout="FitLayout" Border="false" Height="200">
                                                        <Items>
                                                            <ext:Hidden ID="hdnIdEvento" runat="server" />
                                                            <ext:GridPanel ID="GridScripts" runat="server" Header="true" Border="false" Height="200"
                                                                AutoExpandColumn="TipoColectiva" Title="_">
                                                                <Store>
                                                                    <ext:Store ID="StoreScripts" runat="server">
                                                                        <Reader>
                                                                            <ext:JsonReader IDProperty="ID_Script">
                                                                                <Fields>
                                                                                    <ext:RecordField Name="ID_Script" />
                                                                                    <ext:RecordField Name="TipoColectiva" />
                                                                                    <ext:RecordField Name="TipoCuenta" />
                                                                                    <ext:RecordField Name="Formula" />
                                                                                    <ext:RecordField Name="EsAbono" />
                                                                                    <ext:RecordField Name="ValidaSaldo" />
                                                                                </Fields>
                                                                            </ext:JsonReader>
                                                                        </Reader>
                                                                    </ext:Store>
                                                                </Store>
                                                                <ColumnModel runat="server">
                                                                    <Columns>
                                                                        <ext:Column DataIndex="ID_Script" Hidden="true" />
                                                                        <ext:Column DataIndex="TipoColectiva" Header="Tipo Colectiva" Width="150" />
                                                                        <ext:Column DataIndex="TipoCuenta" Header="Tipo Cuenta" Width="150" />
                                                                        <ext:Column DataIndex="Formula" Header="Fórmula" Width="90" />
                                                                        <ext:BooleanColumn DataIndex="EsAbono" Header="Es Abono" Width="70" />
                                                                        <ext:BooleanColumn DataIndex="ValidaSaldo" Header="Valida Saldo" />
                                                                    </Columns>
                                                                </ColumnModel>
                                                                <SelectionModel>
                                                                    <ext:RowSelectionModel SingleSelect="true" />
                                                                </SelectionModel>
                                                                <BottomBar>
                                                                    <ext:PagingToolbar ID="PagingToolBar5" runat="server" StoreID="StoreScripts" DisplayInfo="true"
                                                                        DisplayMsg="{0} - {1} de {2}" HideRefresh="true" PageSize="10" />
                                                                </BottomBar>
                                                            </ext:GridPanel>
                                                        </Items>
                                                    </ext:FormPanel>
                                                </South>
                                            </ext:BorderLayout>
                                        </Content>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelBines" runat="server" Title="Bines" Layout="FitLayout">
                                        <Content>
                                            <ext:BorderLayout runat="server">
                                                <North Split="true">
                                                    <ext:FormPanel ID="FormPanelNuevoBin" runat="server" LabelWidth="150" Height="180">
                                                        <Items>
                                                            <ext:FieldSet runat="server" Title="Datos del Nuevo Bin" Layout="FormLayout">
                                                                <Items>
                                                                    <ext:TextField ID="txtClaveNuevoBin" runat="server" FieldLabel="Clave   <span style='color:red;'>*   </span>"
                                                                        AllowBlank="false" Width="550" MaxLength="20" MaskRe="[0-9]"/>
                                                                    <ext:TextArea ID="txtDescNuevoBin" runat="server" FieldLabel="Observaciones" MaxLength="200" Width="550"
                                                                        Height="75"/>
                                                                </Items>
                                                                <Buttons>
                                                                    <ext:Button ID="btnAddBin" runat="server" Text="Añadir Bin" Icon="Add">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="btnAddBin_Click" Before="var valid= #{FormPanelNuevoBin}.getForm().isValid(); if (!valid) {} return valid;">
                                                                                <EventMask ShowMask="true" Msg="Añadiendo BIN al Producto..." MinDelay="500" />
                                                                            </Click>
                                                                        </DirectEvents>
                                                                    </ext:Button>
                                                                </Buttons>
                                                            </ext:FieldSet>
                                                        </Items>
                                                    </ext:FormPanel>
                                                </North>
                                                <Center Split="true">
                                                    <ext:GridPanel ID="GridBines" runat="server" Header="true" Height="200" AutoExpandColumn="Descripcion">
                                                        <Store>
                                                            <ext:Store ID="StoreBinesProducto" runat="server">
                                                                <DirectEventConfig IsUpload="true" />
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="ID_BIN">
                                                                        <Fields>
                                                                            <ext:RecordField Name="ID_BIN" />
                                                                            <ext:RecordField Name="BIN" />
                                                                            <ext:RecordField Name="Descripcion" />
                                                                        </Fields>
                                                                    </ext:JsonReader>
                                                                </Reader>
                                                                <DirectEventConfig IsUpload="true" />
                                                            </ext:Store>
                                                        </Store>
                                                        <ColumnModel runat="server">
                                                            <Columns>
                                                                <ext:Column DataIndex="ID_BIN" Hidden="true" />
                                                                <ext:Column DataIndex="BIN" Header="Bin" Width="120" />
                                                                <ext:Column DataIndex="Descripcion" Header="Observaciones" Width="400" />
                                                                <ext:CommandColumn Width="60">
                                                                    <Commands>
                                                                        <ext:GridCommand Icon="Pencil" CommandName="Edit">
                                                                            <ToolTip Text="Editar BIN" />
                                                                        </ext:GridCommand>
                                                                    </Commands>
                                                                </ext:CommandColumn>
                                                            </Columns>
                                                        </ColumnModel>
                                                        <DirectEvents>
                                                            <Command OnEvent="EjecutarComandoBines">
                                                                <ExtraParams>
                                                                    <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                                                    <ext:Parameter Name="ID_BIN" Value="Ext.encode(record.data.ID_BIN)" Mode="Raw" />
                                                                    <ext:Parameter Name="BIN" Value="Ext.encode(record.data.BIN)" Mode="Raw" />
                                                                    <ext:Parameter Name="Descripcion" Value="Ext.encode(record.data.Descripcion)" Mode="Raw" />
                                                                </ExtraParams>
                                                            </Command>
                                                        </DirectEvents>
                                                        <SelectionModel>
                                                            <ext:RowSelectionModel SingleSelect="true" />
                                                        </SelectionModel>
                                                        <BottomBar>
                                                            <ext:PagingToolbar ID="PagingToolBar3" runat="server" StoreID="StoreBinesProducto" DisplayInfo="true"
                                                                DisplayMsg="Bines {0} - {1} de {2}" HideRefresh="true" />
                                                        </BottomBar>
                                                    </ext:GridPanel>
                                                </Center>
                                            </ext:BorderLayout>
                                        </Content>
                                    </ext:FormPanel>
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
                                                                            <ext:RecordField Name="esAutorizable" />
                                                                            <ext:RecordField Name="TipoValidacion" />
                                                                            <ext:RecordField Name="ValorInicial" />
                                                                            <ext:RecordField Name="ValorFinal" />
                                                                            <ext:RecordField Name="ExpresionRegular" />
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
                                                                <ext:CommandColumn Width="80" >
                                                                    <PrepareToolbar Fn="prepareToolbar" />
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
                                                        <LoadMask ShowMask="false" />
                                                    </ext:GridPanel>
                                                </Items>
                                            </ext:Panel>
                                        </Items>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelSubproductos" runat="server" Title="Subproductos" Layout="FitLayout">
                                        <Content>
                                            <ext:BorderLayout runat="server">
                                                <North Split="true">
                                                    <ext:FormPanel ID="FormPanelNuevoSubproducto" runat="server" LabelWidth="100" Height="180">
                                                        <Items>
                                                            <ext:FieldSet runat="server" Title="Datos del Nuevo Subproducto" Layout="FormLayout">
                                                                <Items>
                                                                    <ext:TextField ID="txtClaveSubproducto" runat="server" FieldLabel="Clave   <span style='color:red;'>*   </span>"
                                                                        AllowBlank="false" Width="550" MaxLength="10" />
                                                                    <ext:TextArea ID="txtDescSubproducto" runat="server" FieldLabel="Descripción   <span style='color:red;'>*   </span>"
                                                                        AllowBlank="false" MaxLength="200" Width="550" Height="75" />
                                                                </Items>
                                                                <Buttons>
                                                                    <ext:Button ID="btnNuevoSubproducto" runat="server" Text="Añadir Subproducto" Icon="Add">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="btnNuevoSubproducto_Click" Before="var valid= #{FormPanelNuevoSubproducto}.getForm().isValid(); if (!valid) {} return valid;">
                                                                                <EventMask ShowMask="true" Msg="Añadiendo Subproducto..." MinDelay="500" />
                                                                            </Click>
                                                                        </DirectEvents>
                                                                    </ext:Button>
                                                                </Buttons>
                                                            </ext:FieldSet>
                                                        </Items>
                                                    </ext:FormPanel>
                                                </North>
                                                <Center Split="true">
                                                    <ext:GridPanel ID="GridPanelSubProductos" runat="server" Header="true" Height="200" AutoExpandColumn="Descripcion">
                                                        <Store>
                                                            <ext:Store ID="StoreSubproductos" runat="server">
                                                                <DirectEventConfig IsUpload="true" />
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="ID_Plantilla">
                                                                        <Fields>
                                                                            <ext:RecordField Name="ID_Plantilla" />
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
                                                                <ext:Column DataIndex="ID_Plantilla" Hidden="true" />
                                                                <ext:Column DataIndex="Clave" Header="Clave" Width="120" />
                                                                <ext:Column DataIndex="Descripcion" Header="Descripción" Width="400" />
                                                                <ext:CommandColumn Width="60">
                                                                    <Commands>
                                                                        <ext:GridCommand Icon="Pencil" CommandName="Edit">
                                                                            <ToolTip Text="Editar Subproducto" />
                                                                        </ext:GridCommand>
                                                                    </Commands>
                                                                </ext:CommandColumn>
                                                            </Columns>
                                                        </ColumnModel>
                                                        <DirectEvents>
                                                            <Command OnEvent="EjecutarComandoSubProd">
                                                                <ExtraParams>
                                                                    <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                                                    <ext:Parameter Name="ID_Plantilla" Value="Ext.encode(record.data.ID_Plantilla)" Mode="Raw" />
                                                                    <ext:Parameter Name="Clave" Value="Ext.encode(record.data.Clave)" Mode="Raw" />
                                                                    <ext:Parameter Name="Descripcion" Value="Ext.encode(record.data.Descripcion)" Mode="Raw" />
                                                                </ExtraParams>
                                                            </Command>
                                                        </DirectEvents>
                                                        <SelectionModel>
                                                            <ext:RowSelectionModel SingleSelect="true" />
                                                        </SelectionModel>
                                                        <BottomBar>
                                                            <ext:PagingToolbar ID="PagingToolBar4" runat="server" StoreID="StoreSubproductos" DisplayInfo="true"
                                                                DisplayMsg="Mostrando SubProductos {0} - {1} de {2}" HideRefresh="true" />
                                                        </BottomBar>
                                                    </ext:GridPanel>
                                                </Center>
                                            </ext:BorderLayout>
                                        </Content>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelCampanias" runat="server" Title="Campañas" Layout="FitLayout">
                                        <Content>
                                            <ext:BorderLayout runat="server">
                                                <Center Split="true">
                                                    <ext:GridPanel ID="GridPanelCampanyas" runat="server" Header="true" Layout="FitLayout" Border="false"
                                                        AutoExpandColumn="Descripcion">
                                                        <Store>
                                                            <ext:Store ID="StoreCampanyas" runat="server">
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="ID_Campania">
                                                                        <Fields>
                                                                            <ext:RecordField Name="ID_Campania" />
                                                                            <ext:RecordField Name="Clave" />
                                                                            <ext:RecordField Name="Descripcion" />
                                                                            <ext:RecordField Name="FechaInicio" />
                                                                            <ext:RecordField Name="FechaFin" />
                                                                            <ext:RecordField Name="Activa" />
                                                                        </Fields>
                                                                    </ext:JsonReader>
                                                                </Reader>
                                                            </ext:Store>
                                                        </Store>
                                                        <ColumnModel runat="server">
                                                            <Columns>
                                                                <ext:Column DataIndex="ID_Campania" Hidden="true" />
                                                                <ext:Column DataIndex="Clave" Header="Clave" Width="100" />
                                                                <ext:Column DataIndex="Descripcion" Header="Descripción" />
                                                                <ext:DateColumn DataIndex="FechaInicio" Header="Fecha Inicio" Width="150"
                                                                    Format="dd/MM/yyyy" />
                                                                <ext:DateColumn DataIndex="FechaFin" Header="Fecha Fin" Width="150"
                                                                    Format="dd/MM/yyyy" />
                                                                <ext:CommandColumn Width="60">
                                                                    <PrepareToolbar Fn="estatusCampanya" />
                                                                     <Commands>
                                                                         <ext:GridCommand Icon="RecordRed" CommandName="Activar">
                                                                             <ToolTip Text="Activar Campaña" />
                                                                         </ext:GridCommand>
                                                                         <ext:GridCommand Icon="RecordGreen" CommandName="Desactivar">
                                                                             <ToolTip Text="Desactivar Campaña" />
                                                                         </ext:GridCommand>
                                                                         <ext:GridCommand Icon="Pencil" CommandName="Editar">
                                                                             <ToolTip Text="Editar Campaña" />
                                                                         </ext:GridCommand>
                                                                     </Commands>
                                                                 </ext:CommandColumn>
                                                            </Columns>
                                                        </ColumnModel>
                                                        <DirectEvents>
                                                            <Command OnEvent="EjecutarComandoCampanyas">
                                                                <Confirmation BeforeConfirm="if (command == 'Editar') return false;" 
                                                                    ConfirmRequest="true" Title="Confirmación" Message="¿Estás seguro de modificar el estatus de la campaña?" />
                                                                <EventMask ShowMask="true" Msg="Procesando..." MinDelay="500" />
                                                                <ExtraParams>
                                                                    <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                                                    <ext:Parameter Name="Values" Value="Ext.encode(record.data)" Mode="Raw" />
                                                                </ExtraParams>
                                                            </Command>
                                                        </DirectEvents>
                                                        <SelectionModel>
                                                            <ext:RowSelectionModel runat="server" SingleSelect="true">
                                                                <DirectEvents>
                                                                    <RowSelect OnEvent="SeleccionaCampanya" Buffer="100">
                                                                        <EventMask ShowMask="true" 
                                                                            Msg="Obteniendo Planes de Campaña..." MinDelay="500"/>
                                                                        <ExtraParams>
                                                                            <ext:Parameter Name="ID_Campania" Value="record.data['ID_Campania']" Mode="Raw" />
                                                                            <ext:Parameter Name="Clave" Value="record.data['Clave']" Mode="Raw" />
                                                                            <ext:Parameter Name="Descripcion" Value="record.data['Descripcion']" Mode="Raw" />
                                                                        </ExtraParams>
                                                                    </RowSelect>
                                                                </DirectEvents>
                                                            </ext:RowSelectionModel>
                                                        </SelectionModel>
                                                        <BottomBar>
                                                            <ext:PagingToolbar ID="PagingTBCampanyas" runat="server" StoreID="StoreCampanyas" DisplayInfo="false"
                                                                DisplayMsg="{0} - {1} de {2}" HideRefresh="true" PageSize="10" >
                                                                <Items>
                                                                    <ext:ToolbarFill runat="server" />
                                                                    <ext:ToolbarSeparator runat="server" />
                                                                    <ext:Button ID="btnNuevaCampanya" runat="server" Text="Nueva Campaña" Icon="Add">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="btnNuevaCampanya_Click" />
                                                                        </DirectEvents>
                                                                    </ext:Button>
                                                                    <ext:ToolbarSeparator runat="server" />
                                                                </Items>
                                                            </ext:PagingToolbar>
                                                        </BottomBar>
                                                    </ext:GridPanel>
                                                </Center>
                                                <South Split="true">
                                                    <ext:GridPanel ID="GridConfigCamp" runat="server" Layout="FitLayout" Header="true" Border="false" Height="200"
                                                        Title="Configuración de la Campaña" AutoExpandColumn="Descripcion">
                                                        <Store>
                                                            <ext:Store ID="StoreConfigCamp" runat="server">
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="ID_CampaniaConfiguracion">
                                                                        <Fields>
                                                                            <ext:RecordField Name="ID_CampaniaConfiguracion" />
                                                                            <ext:RecordField Name="ID_Promocion" />
                                                                            <ext:RecordField Name="Clave" />
                                                                            <ext:RecordField Name="Descripcion" />
                                                                            <ext:RecordField Name="Diferimiento" />
                                                                            <ext:RecordField Name="Meses" />
                                                                            <ext:RecordField Name="TasaInteres" />
                                                                        </Fields>
                                                                    </ext:JsonReader>
                                                                </Reader>
                                                            </ext:Store>
                                                        </Store>
                                                        <ColumnModel runat="server">
                                                            <Columns>
                                                                <ext:Column DataIndex="ID_CampaniaConfiguracion" Hidden="true" />
                                                                <ext:Column DataIndex="ID_Promocion" Hidden="true" />
                                                                <ext:Column DataIndex="Descripcion" Header="Promoción" />
                                                                <ext:Column DataIndex="Meses" Header="Meses" Width="150" />
                                                                <ext:Column DataIndex="Diferimiento" Header="Diferimiento" Width="150" />
                                                                <ext:Column DataIndex="TasaInteres" Header="Tasa de Interés" Width="150" />
                                                                <ext:CommandColumn Width="60" Header="Acciones">
                                                                    <Commands>
                                                                        <ext:GridCommand Icon="Pencil" CommandName="Editar">
                                                                            <ToolTip Text="Editar Plan" />
                                                                        </ext:GridCommand>
                                                                        <ext:GridCommand Icon="Cross" CommandName="Eliminar">
                                                                            <ToolTip Text="Eliminar Plan" />
                                                                        </ext:GridCommand>
                                                                    </Commands>
                                                                </ext:CommandColumn>
                                                            </Columns>
                                                        </ColumnModel>
                                                        <SelectionModel>
                                                            <ext:RowSelectionModel SingleSelect="true" />
                                                        </SelectionModel>
                                                        <DirectEvents>
                                                            <Command OnEvent="EjecutarComandoCfgCamp">
                                                                <Confirmation BeforeConfirm="if (command == 'Editar') return false;"
                                                                    ConfirmRequest="true" Title="Confirmación" Message="<span style='font-weight: bold;'>*RECUERDA* Una vez eliminado el plan de la campaña, el proceso es irreversible.</span></br></br>¿Estás seguro de eliminarlo?</br>" />
                                                                <EventMask ShowMask="true" Msg="Procesando..." MinDelay="500" />
                                                                <ExtraParams>
                                                                    <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                                                    <ext:Parameter Name="Values" Value="Ext.encode(record.data)" Mode="Raw" />
                                                                </ExtraParams>
                                                            </Command>
                                                        </DirectEvents>
                                                        <BottomBar>
                                                            <ext:PagingToolbar ID="PagingConfigCamp" runat="server" StoreID="StoreConfigCamp" DisplayInfo="false"
                                                                DisplayMsg="{0} - {1} de {2}" HideRefresh="true" PageSize="10">
                                                                <Items>
                                                                    <ext:ToolbarFill runat="server" />
                                                                    <ext:ToolbarSeparator runat="server" />
                                                                    <ext:Button ID="btnNuevaCfgCamp" runat="server" Text="Nueva Configuración" Icon="Add">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="btnNuevaCfgCamp_Click" Before="if (#{hdnIdCampanya}.getValue() == '') { 
                                                                                Ext.MessageBox.show({
                                                                                    msg: 'Debes seleccionar una campaña para la configuración',
                                                                                    title: 'Nueva Configuración',
                                                                                    buttons: Ext.MessageBox.OK,
                                                                                    });
                                                                                return false; } else { #{hdnIdCampCfg}.clear(); #{hdnIdCampCfg}.setValue(0); }" />
                                                                        </DirectEvents>
                                                                    </ext:Button>
                                                                    <ext:ToolbarSeparator runat="server" />
                                                                </Items>
                                                            </ext:PagingToolbar>
                                                        </BottomBar>
                                                    </ext:GridPanel>
                                                </South>
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
