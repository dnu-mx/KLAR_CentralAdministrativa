<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="AdminCuentasOnBoarding.aspx.cs" Inherits="Administracion.AdminCuentasOnBoarding" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var fullName = function (value, metadata, record, rowIndex, colIndex, store) {
            return "<b>" + record.data.Nombre + "</b>";
        };

        var prepareTB_CtasOnBParams = function (grid, toolbar, rowIndex, record) {
            if (record.get("esAutorizable") == 1) {
                toolbar.items.get(0).hide();
                toolbar.items.get(1).hide();
            }
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Hidden ID="hdnIdCuentaLDC" runat="server" />
    <ext:Hidden ID="hdnIdCuentaCC" runat="server" />
    <ext:Hidden ID="hdnIdMA" runat="server" />
    <ext:Hidden ID="hdnIdColectiva" runat="server" />
    <ext:Hidden ID="hdnIdCadena" runat="server" />
    <ext:Hidden ID="hdnValorIniPMA" runat="server" />
    <ext:Hidden ID="hdnValorFinPMA" runat="server" />
    <ext:Store ID="StoreEstadosRepMex" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="CveEstado">
                <Fields>
                    <ext:RecordField Name="CveEstado" />
                    <ext:RecordField Name="DesEstado" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
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
    <ext:BorderLayout ID="MainBorderLayout" runat="server">
        <West Split="true">
            <ext:Panel ID="MainPanel" runat="server" Width="350" Border="false">
                <Content>
                    <ext:BorderLayout ID="LeftBorderLayout" runat="server">
                        <North Split="true">
                            <ext:FormPanel ID="FormPanelBusqueda" runat="server" Title="Consulta de Cuentas" Height="240" Frame="true" LabelWidth="120"
                                Collapsible="true" Border="false">
                                <Items>
                                    <ext:FieldSet ID="FieldSetBusqueda" runat="server" Border="false">
                                        <Items>
                                            <ext:TextField ID="txtNombre" runat="server" FieldLabel="Nombre" MaxLength="30" Width="310" />
                                            <ext:TextField ID="txtApPaterno" runat="server" FieldLabel="Primer Apellido" MaxLength="30" Width="310" />
                                            <ext:TextField ID="txtApMaterno" runat="server" FieldLabel="Segundo Apellido" MaxLength="30" Width="310" />
                                            <ext:TextField ID="txtNumCuenta" runat="server" FieldLabel="Número de Cuenta" MaxLength="16" 
                                                MinLength="16" Width="310" MaskRe="[0-9]" />
                                            <ext:TextField ID="txtNumTarjeta" runat="server" FieldLabel="Número de Tarjeta" MaxLength="16"
                                                MinLength="16" Width="310" MaskRe="[0-9]" />
                                            <ext:Checkbox ID="chkBoxSoloAdicionales" runat="server" FieldLabel="Sólo Adicionales" />
                                        </Items>
                                        <Buttons>
                                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                                <DirectEvents>
                                                    <Click OnEvent="btnLimpiar_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                                        Before="if (!#{txtNumCuenta}.isValid()) { return false; }
                                                        if (!#{txtNumTarjeta}.isValid()) { return false; }
                                                        if (!#{txtNumCuenta}.getValue() && !#{txtNumTarjeta}.getValue() &&
                                                        !#{txtNombre}.getValue() && !#{txtApPaterno}.getValue() && !#{txtApMaterno}.getValue())
                                                        { Ext.Msg.alert('Consulta de Cuentas', 'Ingresa al menos un criterio de búsqueda'); return false; }
                                                        else { #{EastPanel}.setTitle('_'); #{EastPanel}.setDisabled(true); }">
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
                            <ext:FormPanel ID="FormPanelResultados" runat="server" Title="Resultados Cuentas" Layout="FitLayout" Border="false">
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
                                                <ext:Column DataIndex="Tarjeta" Header="Tarjeta" Width="120" />
                                                <ext:Column DataIndex="NumeroTarjeta" Hidden="true" />
                                                <ext:Column DataIndex="IdCadenaComercial" Hidden="true" />
                                                <ext:Column DataIndex="IdColectivaCuentahabiente" Hidden="true" />
                                                <ext:Column DataIndex="NombreTarjetahabiente" Header="Nombre" Width="200" />
                                                <ext:Column DataIndex="CLDC" Hidden="true" />
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
                                    <ext:FormPanel ID="FormPanelTitular" runat="server" Title="Titular" LabelAlign="Left" LabelWidth="140" AutoScroll="true"
                                        Border="false">
                                        <Content>
                                            <ext:Store ID="StoreColonias" runat="server">
                                                <Reader>
                                                    <ext:JsonReader IDProperty="ID_Colonia">
                                                        <Fields>
                                                            <ext:RecordField Name="ID_Colonia" />
                                                            <ext:RecordField Name="Colonia" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Content>
                                        <Items>
                                            <ext:FieldSet ID="FieldSetDatosTitular" runat="server" Border="false" Layout="FormLayout">
                                                <Items>
                                                    <ext:Panel ID="PanelDatosPersonales" runat="server" Title="Datos Personales" AutoHeight="true" LabelAlign="Left"
                                                        FormGroup="true" Layout="FormLayout">
                                                        <Items>
                                                            <ext:TextField ID="txtNombreClienteTitular" runat="server" FieldLabel="Nombre" Width="550"
                                                                AllowBlank="false" ReadOnly="true" />
                                                            <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false">
                                                                <Defaults>
                                                                    <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                                </Defaults>
                                                                <LayoutConfig>
                                                                    <ext:HBoxLayoutConfig Align="Top" />
                                                                </LayoutConfig>
                                                                <Items>
                                                                    <ext:TextField ID="txtApPaternoTitular" runat="server" FieldLabel="Primer Apellido" 
                                                                        Width="350" AllowBlank="false" ReadOnly="true" />
                                                                    <ext:Hidden runat="server" Width="15" />
                                                                    <ext:TextField ID="txtApMaternoTitular" runat="server" LabelWidth="110" Width="330"
                                                                        FieldLabel="Segundo Apellido" AllowBlank="false" ReadOnly="true" />
                                                                </Items>
                                                            </ext:Panel>
                                                            <ext:Panel runat="server" Layout="FitLayout" Width="380" Height="5" Border="false" />
                                                            <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false">
                                                                <Defaults>
                                                                    <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                                </Defaults>
                                                                <LayoutConfig>
                                                                    <ext:HBoxLayoutConfig Align="Top" />
                                                                </LayoutConfig>
                                                                <Items>
                                                                    <ext:DateField ID="dfFechaNac" runat="server" FieldLabel="Fecha de Nacimiento"
                                                                        Width="350" Format="dd/MM/yyyy" Vtype="daterange" Editable="false" ReadOnly="true" />
                                                                    <ext:Hidden runat="server" Width="15" />
                                                                    <ext:ComboBox ID="cBoxEdoNac" runat="server" LabelWidth="110" StoreID="StoreEstadosRepMex"
                                                                        DisplayField="DesEstado" ValueField="CveEstado" FieldLabel="Entidad Nacimiento" 
                                                                        Width="330" ListWidth="350" ReadOnly="true" />
                                                                </Items>
                                                            </ext:Panel>
                                                            <ext:Panel runat="server" Layout="FitLayout" Width="380" Height="5" Border="false" />
                                                            <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false">
                                                                <Defaults>
                                                                    <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                                </Defaults>
                                                                <LayoutConfig>
                                                                    <ext:HBoxLayoutConfig Align="Top" />
                                                                </LayoutConfig>
                                                                <Items>
                                                                    <ext:ComboBox ID="cBoxGenero" runat="server" FieldLabel="Género" ReadOnly="true"
                                                                        Width="350" DisplayField="Descripcion" ValueField="Clave">
                                                                        <Store>
                                                                            <ext:Store ID="StoreGenero" runat="server">
                                                                                <Reader>
                                                                                    <ext:JsonReader IDProperty="Clave">
                                                                                        <Fields>
                                                                                            <ext:RecordField Name="Clave" />
                                                                                            <ext:RecordField Name="Descripcion" />
                                                                                        </Fields>
                                                                                    </ext:JsonReader>
                                                                                </Reader>
                                                                            </ext:Store>
                                                                        </Store>
                                                                    </ext:ComboBox>
                                                                    <ext:Hidden runat="server" Width="15" />
                                                                    <ext:TextField ID="txtNumID" runat="server"  LabelWidth="110" ReadOnly="true"
                                                                        FieldLabel="No. Identificación" Width="330" />
                                                                </Items>
                                                            </ext:Panel>
                                                            <ext:Panel runat="server" Layout="FitLayout" Width="380" Height="5" Border="false" />
                                                            <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false">
                                                                <Defaults>
                                                                    <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                                </Defaults>
                                                                <LayoutConfig>
                                                                    <ext:HBoxLayoutConfig Align="Top" />
                                                                </LayoutConfig>
                                                                <Items>
                                                                    <ext:TextField ID="txtRFCTitular" runat="server" FieldLabel="RFC" ReadOnly="true"
                                                                        MinLength="13" Width="350" />
                                                                    <ext:Hidden runat="server" Width="15" />
                                                                    <ext:TextField ID="txtCURP" runat="server" LabelWidth="70" FieldLabel="CURP"
                                                                        MinLength="18" Width="330" ReadOnly="true" />
                                                                </Items>
                                                            </ext:Panel>
                                                            <ext:Panel runat="server" Layout="FitLayout" Width="380" Height="5" Border="false" />
                                                            <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false">
                                                                <Defaults>
                                                                    <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                                </Defaults>
                                                                <LayoutConfig>
                                                                    <ext:HBoxLayoutConfig Align="Top" />
                                                                </LayoutConfig>
                                                                <Items>
                                                                    <ext:TextField ID="txtOcupacion" runat="server" FieldLabel="Ocupación"
                                                                        Width="350" ReadOnly="true"/>
                                                                    <ext:Hidden runat="server" Width="15" />
                                                                    <ext:TextField ID="txtProfesion" runat="server" LabelWidth="70" ReadOnly="true"
                                                                        FieldLabel="Profesión" Width="330" />
                                                                </Items>
                                                            </ext:Panel>
                                                            <ext:Panel runat="server" Layout="FitLayout" Width="380" Height="5" Border="false" />
                                                            <ext:TextField ID="txtNacionalidad" runat="server" FieldLabel="Nacionalidad" Width="550"
                                                                ReadOnly="true" />
                                                        </Items>
                                                    </ext:Panel>
                                                    <ext:Panel ID="PanelDomicilio" runat="server" Title="Domicilio" AutoHeight="true" LabelAlign="Left"
                                                        FormGroup="true" Layout="FormLayout" Collapsed="true">
                                                        <Items>
                                                            <ext:Hidden ID="hdnClaveCiudad" runat="server" />
                                                            <ext:TextField ID="txtID_Direccion" runat="server" Hidden="true" Enabled="false" />
                                                            <ext:TextField ID="txtCalle" runat="server" FieldLabel="Calle" Width="550" ReadOnly="true" />
                                                            <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false">
                                                                <Defaults>
                                                                    <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                                </Defaults>
                                                                <LayoutConfig>
                                                                    <ext:HBoxLayoutConfig Align="Top" />
                                                                </LayoutConfig>
                                                                <Items>
                                                                    <ext:TextField ID="txtNumExterior" runat="server" FieldLabel="Número Exterior"
                                                                        Width="350" ReadOnly="true" />
                                                                    <ext:Hidden runat="server" Width="15" />
                                                                    <ext:TextField ID="txtNumInterior" runat="server" LabelWidth="110" ReadOnly="true"
                                                                        FieldLabel="Número Interior" Width="330" />
                                                                </Items>
                                                            </ext:Panel>
                                                            <ext:Panel runat="server" Layout="FitLayout" Width="390" Height="5" Border="false" />
                                                            <ext:TextField ID="txtEntreCalles" runat="server" FieldLabel="Entre las Calles" 
                                                                Width="550" ReadOnly="true" />
                                                            <ext:TextField ID="txtReferencias" runat="server" FieldLabel="Referencias del Domicilio" 
                                                                Width="550" ReadOnly="true" />
                                                            <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false">
                                                                <Defaults>
                                                                    <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                                </Defaults>
                                                                <LayoutConfig>
                                                                    <ext:HBoxLayoutConfig Align="Top" />
                                                                </LayoutConfig>
                                                                <Items>
                                                                    <ext:TextField ID="txtCPCliente" runat="server" FieldLabel="Código Postal" MinLength="5"
                                                                        Width="350" EnableKeyEvents="true" ReadOnly="true">
                                                                        <Listeners>
                                                                            <SpecialKey Handler="if (e.getKey() == e.TAB || e.getKey() == e.ENTER) {
                                                                                #{cBoxColonia}.reset(); CuentasBancarias.LlenaComboColonias();}" />
                                                                        </Listeners>
                                                                    </ext:TextField>
                                                                    <ext:TextField ID="txtIDColonia" runat="server" Hidden="true" Enabled="false" />
                                                                    <ext:Hidden runat="server" Width="15" />
                                                                    <ext:ComboBox ID="cBoxColonia" runat="server" FieldLabel="Colonia" LabelWidth="70"
                                                                        StoreID="StoreColonias" Width="235" DisplayField="Colonia" ReadOnly="true"
                                                                        ValueField="ID_Colonia" ListWidth="330" />
                                                                </Items>
                                                            </ext:Panel>
                                                            <ext:Panel runat="server" Layout="FitLayout" Width="380" Height="5" Border="false" />
                                                            <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false">
                                                                <Defaults>
                                                                    <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                                </Defaults>
                                                                <LayoutConfig>
                                                                    <ext:HBoxLayoutConfig Align="Top" />
                                                                </LayoutConfig>
                                                                <Items>
                                                                    <ext:TextField ID="txtClaveMunicipio" runat="server" Hidden="true" Enabled="false" />
                                                                    <ext:TextField ID="txtMunicipioTitular" runat="server" FieldLabel="Delegación o Municipio"
                                                                        ReadOnly="true" Width="350" />
                                                                    <ext:Hidden runat="server" Width="15" />
                                                                    <ext:TextField ID="txtCiudad" runat="server" FieldLabel="Ciudad" ReadOnly="true"
                                                                       LabelWidth="70" Width="330" />
                                                                </Items>
                                                            </ext:Panel>
                                                            <ext:Panel runat="server" Layout="FitLayout" Width="380" Height="5" Border="false" />
                                                            <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false">
                                                                <Defaults>
                                                                    <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                                </Defaults>
                                                                <LayoutConfig>
                                                                    <ext:HBoxLayoutConfig Align="Top" />
                                                                </LayoutConfig>
                                                                <Items>
                                                                    <ext:TextField ID="txtClaveEstado" runat="server" Hidden="true" Enabled="false" />
                                                                    <ext:TextField ID="txtEstadoTitular" runat="server" FieldLabel="Estado" ReadOnly="true"
                                                                        Width="350" />
                                                                    <ext:Hidden runat="server" Width="15" />
                                                                    <ext:ComboBox ID="cBoxPais" runat="server" FieldLabel="País" Width="330" Mode="Local"
                                                                        DisplayField="Descripcion" ValueField="ID_Pais" AutoSelect="true" Editable="true"
                                                                        ForceSelection="true" TypeAhead="true" MinChars="1" MatchFieldWidth="false"
                                                                        ListWidth="300" LabelWidth="70" ReadOnly="true">
                                                                        <Store>
                                                                            <ext:Store ID="StorePaises" runat="server">
                                                                                <Reader>
                                                                                    <ext:JsonReader IDProperty="ID_Pais">
                                                                                        <Fields>
                                                                                            <ext:RecordField Name="ID_Pais" />
                                                                                            <ext:RecordField Name="Clave" />
                                                                                            <ext:RecordField Name="Descripcion" />
                                                                                        </Fields>
                                                                                    </ext:JsonReader>
                                                                                </Reader>
                                                                            </ext:Store>
                                                                        </Store>
                                                                    </ext:ComboBox>
                                                                </Items>
                                                            </ext:Panel>
                                                            <ext:Panel runat="server" Layout="FitLayout" Width="380" Height="5" Border="false" />
                                                            <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false">
                                                                <Defaults>
                                                                    <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                                </Defaults>
                                                                <LayoutConfig>
                                                                    <ext:HBoxLayoutConfig Align="Top" />
                                                                </LayoutConfig>
                                                                <Items>
                                                                    <ext:TextField ID="txtLatitud" runat="server" FieldLabel="Latitud" ReadOnly="true"
                                                                        MaskRe="[0-9\-.]" Width="350" />
                                                                    <ext:Hidden runat="server" Width="15" />
                                                                    <ext:TextField ID="txtLongitud" runat="server" FieldLabel="Longitud" ReadOnly="true"
                                                                        MaskRe="[0-9\-.]" LabelWidth="70" Width="330" />
                                                                </Items>
                                                            </ext:Panel>
                                                            <ext:Panel runat="server" Layout="FitLayout" Width="380" Height="5" Border="false" />
                                                            <ext:TextField ID="txtGiroNegocio" runat="server" FieldLabel="Giro Negocio" ReadOnly="true"
                                                                Width="550" />
                                                        </Items>
                                                    </ext:Panel>
                                                    <ext:Panel ID="PanelDatosContacto" runat="server" Title="Datos de Contacto" AutoHeight="true" LabelAlign="Left"
                                                        FormGroup="true" Layout="FormLayout" >
                                                        <Items>
                                                            <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false">
                                                                <Defaults>
                                                                    <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                                </Defaults>
                                                                <LayoutConfig>
                                                                    <ext:HBoxLayoutConfig Align="Top" />
                                                                </LayoutConfig>
                                                                <Items>
                                                                    <ext:NumberField ID="nfTelParticular" runat="server" FieldLabel="Teléfono Particular" 
                                                                        Width="350" AllowDecimals="False" AllowNegative="False" ReadOnly="true" />
                                                                    <ext:Hidden runat="server" Width="15" />
                                                                    <ext:NumberField ID="nfTelCelular" runat="server" FieldLabel="Teléfono Celular" ReadOnly="true"
                                                                        LabelWidth="110" Width="330" AllowDecimals="False" AllowNegative="False" />
                                                                </Items>
                                                            </ext:Panel>
                                                            <ext:Panel runat="server" Layout="FitLayout" Width="380" Height="5" Border="false" />
                                                            <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false">
                                                                <Defaults>
                                                                    <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                                </Defaults>
                                                                <LayoutConfig>
                                                                    <ext:HBoxLayoutConfig Align="Top" />
                                                                </LayoutConfig>
                                                                <Items>
                                                                    <ext:NumberField ID="nfTelTrabajo" runat="server" FieldLabel="Teléfono Trabajo" ReadOnly="true"
                                                                        Width="350" AllowDecimals="False" AllowNegative="False" />
                                                                    <ext:Hidden runat="server" Width="15" />
                                                                    <ext:TextField ID="txtCorreo" runat="server" FieldLabel="Correo Electrónico" ReadOnly="true"
                                                                       LabelWidth="110" Width="330" />
                                                                </Items>
                                                            </ext:Panel>
                                                        </Items>
                                                    </ext:Panel>
                                                </Items>
                                                <Buttons>
                                                    <ext:Button ID="btnGuardaDatos" runat="server" Text="Guardar Cambios" Icon="Disk" Hidden="true">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnGuardaDatos_Click" Before="if (!#{txtNombreClienteTitular}.isValid() || 
                                                                !#{txtApPaternoTitular}.isValid() || !#{txtApMaterno}.isValid()) { return false; }" />
                                                        </DirectEvents>
                                                    </ext:Button>
                                                </Buttons>
                                            </ext:FieldSet>
                                        </Items>        
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelTarjetas" runat="server" Title="Tarjetas" Layout="FitLayout" 
                                        AutoScroll="true" Border="false">
                                        <Items>
                                            <ext:GridPanel ID="GridTarjetas" runat="server" Height="750" Title="Tarjetas de la Cuenta"
                                                Border="false" AutoExpandColumn="Tarjetahabiente">
                                                <Store>
                                                    <ext:Store ID="StoreTarjetas" runat="server">
                                                        <Reader>
                                                            <ext:JsonReader IDProperty="ID_MA">
                                                                <Fields>
                                                                    <ext:RecordField Name="ID_MA" />
                                                                    <ext:RecordField Name="Tarjeta" />
                                                                    <ext:RecordField Name="Tipo" />
                                                                    <ext:RecordField Name="Tarjetahabiente" />
                                                                    <ext:RecordField Name="NombreEmbozo" />
                                                                    <ext:RecordField Name="TipoManufactura" />
                                                                    <ext:RecordField Name="FechaAlta" />
                                                                    <ext:RecordField Name="Expiracion" />
                                                                    <ext:RecordField Name="NivelCuenta" />
                                                                </Fields>
                                                            </ext:JsonReader>
                                                        </Reader>
                                                    </ext:Store>
                                                </Store>
                                                <ColumnModel ID="ColumnModel3" runat="server">
                                                    <Columns>
                                                        <ext:Column DataIndex="ID_MA" Hidden="true" />
                                                        <ext:Column DataIndex="Tarjeta" Header="Tarjeta" Width="110" />
                                                        <ext:Column DataIndex="Tipo" Header="Tipo" />
                                                        <ext:Column DataIndex="Tarjetahabiente" Header="Tarjetahabiente" />
                                                        <ext:Column DataIndex="NombreEmbozo" Header="Nombre Embozo" />
                                                        <ext:Column DataIndex="TipoManufactura" Header="Manufactura" Width="80" />
                                                        <ext:DateColumn DataIndex="FechaAlta" Header="Fecha Alta" Align="Center"
                                                            Format="dd-MMM-yyyy" Width="80  " />
                                                        <ext:DateColumn DataIndex="Expiracion" Header="Expiración" Align="Center"
                                                            Format="dd-MMM-yyyy" Width="80  " />
                                                        <ext:Column DataIndex="NivelCuenta" Header="Nivel Cuenta" Width="75" />
                                                    </Columns>
                                                </ColumnModel>
                                                <SelectionModel>
                                                    <ext:RowSelectionModel SingleSelect="true" />
                                                </SelectionModel>
                                                <BottomBar>
                                                    <ext:PagingToolbar ID="PagingToolBar3" runat="server" StoreID="StoreTarjetas" DisplayInfo="true"
                                                        DisplayMsg="Tarjetas {0} - {1} de {2}" />
                                                </BottomBar>
                                            </ext:GridPanel>
                                        </Items>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelTiposMA" runat="server" Title="Medios de Acceso" Layout="FitLayout" 
                                        AutoScroll="true" Border="false">
                                        <Items>
                                            <ext:GridPanel ID="GridPanelTiposMA" runat="server" Height="750" Title="Medios de Acceso de la Cuenta"
                                                Border="false" AutoExpandColumn="MA_CLABE">
                                                <Store>
                                                    <ext:Store ID="StoreTiposMA" runat="server">
                                                        <Reader>
                                                            <ext:JsonReader IDProperty="ID">
                                                                <Fields>
                                                                    <ext:RecordField Name="ID" />
                                                                    <ext:RecordField Name="MA_IDCTA" />
                                                                    <ext:RecordField Name="MA_NOCTA" />
                                                                    <ext:RecordField Name="MA_CACAO" />
                                                                    <ext:RecordField Name="MA_CLABE" />
                                                                </Fields>
                                                            </ext:JsonReader>
                                                        </Reader>
                                                    </ext:Store>
                                                </Store>
                                                <ColumnModel ID="ColumnModel5" runat="server">
                                                    <Columns>
                                                        <ext:Column DataIndex="ID" Hidden="true" />
                                                        <ext:Column DataIndex="MA_IDCTA" Hidden="true" />
                                                        <ext:Column DataIndex="MA_NOCTA" Header="Número Cuenta" Width="100" />
                                                        <ext:Column DataIndex="MA_CACAO" Header="Cuenta Interna" Width="150" />
                                                        <ext:Column DataIndex="MA_CLABE" Header="CLABE" Width="150" />
                                                    </Columns>
                                                </ColumnModel>
                                                <SelectionModel>
                                                    <ext:RowSelectionModel SingleSelect="true" />
                                                </SelectionModel>
                                                <BottomBar>
                                                    <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="StoreTiposMA" DisplayInfo="true" />
                                                </BottomBar>
                                            </ext:GridPanel>
                                        </Items>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelParams" runat="server" Title="Parámetros" Layout="FitLayout" Border="false">
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
                                                        AutoHeight="true" Layout="FitLayout">
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
                                                                    <PrepareToolbar Fn="prepareTB_CtasOnBParams" />
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
