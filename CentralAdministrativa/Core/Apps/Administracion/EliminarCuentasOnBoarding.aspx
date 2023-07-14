<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="EliminarCuentasOnBoarding.aspx.cs" Inherits="Administracion.EliminarCuentasOnBoarding" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .chng-red-nf-css
        {
            color:red;
            font-weight:bold;
        }
    </style>
    <style type="text/css">
        .chng-blue-nf-css
        {
            color:blue;
            font-weight:bold;
        }
    </style>
    <style type="text/css">
        .x-grid3-row-body p {
            margin: 3px 3px 7px 3px !important;
            width: 99%;
            color: black;
        }
    </style>

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


        var commandHandlerTarjetas = function (cmd, record) {
            Ext.Msg.confirm('Eliminar Tarjeta',
                '¿Esta seguro que desea eliminar la Tarjeta?',
                function (btn) {
                    if (btn == 'yes') {
                        Ext.net.Mask.show({ msg: 'Procesando...' });
                        AdminObjeto.Elimina(record.json);
                    } 
                });
        }

        var commandHandlerCuentas = function (cmd, record) {
            Ext.Msg.confirm('Cancelar Cuentas',
                '¿Esta seguro que desea cancelar todas las cuentas del Tarjetahabiente?',
                function (btn) {
                    if (btn == 'yes') {
                        Ext.net.Mask.show({ msg: 'Procesando...' });
                        AdminObjeto.CancelaCuentas();
                    }
                });
        }

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Hidden ID="hdnIdCuentaLDC" runat="server" />
    <ext:Hidden ID="hdnIdCuentaCC" runat="server" />
    <ext:Hidden ID="hdnIdMA" runat="server" />
    <ext:Hidden ID="hdnTarjeta" runat="server" />
    <ext:Hidden ID="hdnIdColectiva" runat="server" />
    <ext:Hidden ID="hdnIdCadena" runat="server" />
    <ext:Hidden ID="hdnClaveColectivaPadre" runat="server" />

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
    <ext:BorderLayout ID="MainBorderLayout" runat="server">
        <West Split="true">
            <ext:Panel ID="MainPanel" runat="server" Width="350" Border="false">
                <Content>
                    <ext:BorderLayout ID="LeftBorderLayout" runat="server">
                        <North Split="true">
                            <ext:FormPanel ID="FormPanelBusqueda" runat="server" Title="Consulta de Cuentas" Height="150" Frame="true" LabelWidth="120"
                                Collapsible="true" Border="false">
                                <Items>
                                    <ext:FieldSet ID="FieldSetBusqueda" runat="server" Border="false">
                                        <Items>
                                            <ext:TextField ID="txtNumCuenta" runat="server" FieldLabel="Número de Cuenta" MaxLength="20" 
                                                Width="310" MaskRe="[0-9]" />
                                            <ext:TextField ID="txtNumTarjeta" runat="server" FieldLabel="Número de Tarjeta" MaxLength="16"
                                                Width="310" MaskRe="[0-9]" text=""  />
                                        </Items>
                                        <Buttons>
                                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                                <DirectEvents>
                                                    <Click OnEvent="btnLimpiar_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnBuscar_Click" Before="if (!#{txtNumCuenta}.getValue() && !#{txtNumTarjeta}.getValue())
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
                                            <ext:Store ID="StoreResultados" runat="server">
                                                <Reader>
                                                    <ext:JsonReader IDProperty="IdTarjeta">
                                                        <Fields>
                                                            <ext:RecordField Name="IdTarjeta" />
                                                            <ext:RecordField Name="Tarjeta" />
                                                            <ext:RecordField Name="ClaveMA" />
                                                            <ext:RecordField Name="IdCadenaComercial" />  
                                                            <ext:RecordField Name="IdColectivaCuentahabiente" />
                                                            <ext:RecordField Name="NombreTarjetahabiente" />
                                                            <ext:RecordField Name="CLDC" />
                                                            <ext:RecordField Name="CCLC" />
                                                            <ext:RecordField Name="ClaveColectivaPadre" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Store>
                                        <ColumnModel ID="ColumnModel1" runat="server">
                                            <Columns>
                                                <ext:Column DataIndex="IdTarjeta" Hidden="true" />
                                                <ext:Column DataIndex="Tarjeta" Header="Tarjeta" Width="120" />
                                                <ext:Column DataIndex="IdCadenaComercial" Hidden="true" />
                                                <ext:Column DataIndex="IdColectivaCuentahabiente" Hidden="true" />
                                                <ext:Column DataIndex="ClaveColectivaPadre" Hidden="true" />
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
                                            <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreResultados" DisplayInfo="true" HideRefresh="true"
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
                                    <ext:FormPanel ID="FormPanelTitular" runat="server" Title="Información" LabelAlign="Left" LabelWidth="140" AutoScroll="true"
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
                                            <ext:FieldSet ID="FieldSetDatosTitular" runat="server" Border="false" >
                                                <Items>
                                                    <ext:Panel ID="PanelDatosPersonales" runat="server" Title="Datos Personales" AutoHeight="true" LabelAlign="Left"
                                                        FormGroup="true" Layout="FormLayout" Collapsed="true">
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
                                                                        Width="330" ListWidth="300" ReadOnly="true" />
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
                                                                        StoreID="StoreColonias" Width="330" DisplayField="Colonia" ReadOnly="true"
                                                                        ValueField="ID_Colonia" ListWidth="350" />
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
                                                                        ListWidth="350" LabelWidth="70" ReadOnly="true">
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
                                                        FormGroup="true" Layout="FormLayout" Collapsed="true">
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
                                                    <ext:GridPanel ID="GridTarjetas" runat="server" Height="150" Title="Tarjetas" Border="false">
                                                        <Store>
                                                            <ext:Store ID="StoreTarjetas" runat="server">
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="ID_MA">
                                                                        <Fields>
                                                                            <ext:RecordField Name="ID_MA" />
                                                                            <ext:RecordField Name="ClaveMA" />
                                                                            <ext:RecordField Name="Estatus" />
                                                                            <ext:RecordField Name="Tarjetahabiente" />
                                                                            <ext:RecordField Name="NombreEmbozo" />
                                                                            <ext:RecordField Name="TipoManufactura" />
                                                                            <ext:RecordField Name="FechaAlta" />
                                                                            <ext:RecordField Name="Expiracion" />
                                                                            <ext:RecordField Name="TieneAdicionales" />
                                                                            <ext:RecordField Name="TipoTarjeta" />
                                                                        </Fields>
                                                                    </ext:JsonReader>
                                                                </Reader>
                                                            </ext:Store>
                                                        </Store>
                                                        <ColumnModel ID="ColumnModel2" runat="server">
                                                            <Columns>
                                                                <ext:Column DataIndex="ID_MA" Hidden="true" />
                                                                <ext:Column DataIndex="ClaveMA" Header="Tarjeta" Width="120" />
                                                                <ext:Column DataIndex="TipoTarjeta" Header="Tipo" />
                                                                <ext:Column DataIndex="Estatus" Header="Estatus" Width="80" />
                                                                <ext:Column DataIndex="Tarjetahabiente" Header="Tarjetahabiente" />
                                                                <ext:Column DataIndex="NombreEmbozo" Header="Nombre Embozo" />
                                                                <ext:Column DataIndex="TipoManufactura" Header="Manufactura" Width="80" />
                                                                <ext:DateColumn DataIndex="FechaAlta" Header="Fecha Alta" Align="Center"
                                                                    Format="dd-MMM-yyyy" Width="80  " />
                                                                <ext:DateColumn DataIndex="Expiracion" Header="Expiración" Align="Center"
                                                                    Format="dd-MMM-yyyy" Width="80  " />
                                                                <ext:Column DataIndex="TieneAdicionales" Header="Tiene Adicionales" Width="100" />
                                                                <ext:CommandColumn Width="30">
                                                                    <Commands>
                                                                        <ext:GridCommand CommandName="Eliminar_Event" Icon="Cross">
                                                                            <ToolTip Text="Eliminar Tarjeta" />
                                                                        </ext:GridCommand>
                                                                    </Commands>
                                                                </ext:CommandColumn>
                                                            </Columns>
                                                        </ColumnModel>
                                                        <SelectionModel>
                                                            <ext:RowSelectionModel SingleSelect="true" />
                                                        </SelectionModel>
                                                        <Listeners>
                                                            <Command Fn="commandHandlerTarjetas" />
                                                        </Listeners>
                                                        <BottomBar>
                                                            <ext:PagingToolbar ID="PagingToolBar4" runat="server" StoreID="StoreTarjetas" DisplayInfo="true"
                                                                DisplayMsg="Tarjetas {0} - {1} de {2}" HideRefresh="true" />
                                                        </BottomBar>
                                                    </ext:GridPanel>
                                                    <ext:GridPanel ID="GridMediosAcceso" runat="server" Height="170" Title="Medios Acceso" Border="false">
                                                        <Store>
                                                            <ext:Store ID="StoreMediosAcceso" runat="server">
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="ID_MA">
                                                                        <Fields>
                                                                            <ext:RecordField Name="ID_MA" />
                                                                            <ext:RecordField Name="Tipo" />
                                                                            <ext:RecordField Name="ClaveMA" />
                                                                            <ext:RecordField Name="Estatus" />
                                                                        </Fields>
                                                                    </ext:JsonReader>
                                                                </Reader>
                                                            </ext:Store>
                                                        </Store>
                                                        <ColumnModel ID="ColumnModel4" runat="server">
                                                            <Columns>
                                                                <ext:Column DataIndex="ID_MA" Hidden="true" />
                                                                <ext:Column DataIndex="Tipo" Header="Tipo" Width="250" />
                                                                <ext:Column DataIndex="ClaveMA" Header="Clave" Width="250" />
                                                                <ext:Column DataIndex="Estatus" Header="Estatus" Width="250" />
                                                            </Columns>
                                                        </ColumnModel>
                                                        <SelectionModel>
                                                            <ext:RowSelectionModel SingleSelect="true" />
                                                        </SelectionModel>
                                                        <BottomBar>
                                                            <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="StoreTarjetas" DisplayInfo="true"
                                                                DisplayMsg="Tarjetas {0} - {1} de {2}" HideRefresh="true" />
                                                        </BottomBar>
                                                    </ext:GridPanel>
                                                    <ext:GridPanel ID="GridPanelCuentas" runat="server" Height="220" Title="Cuentas" Border="false"
                                                        AutoExpandColumn="SaldoActual">
                                                        <Store>
                                                            <ext:Store ID="StoreCuentas" runat="server">
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="ID_Cuenta">
                                                                        <Fields>
                                                                            <ext:RecordField Name="ID_Cuenta" />
                                                                            <ext:RecordField Name="ClaveMA" />
                                                                            <ext:RecordField Name="ID_ColectivaCuentahabiente" />
                                                                            <ext:RecordField Name="Tipo" />
                                                                            <ext:RecordField Name="Estatus" />
                                                                            <ext:RecordField Name="SaldoActual" />
                                                                        </Fields>
                                                                    </ext:JsonReader>
                                                                </Reader>
                                                            </ext:Store>
                                                        </Store>
                                                        <ColumnModel ID="ColumnModel3" runat="server">
                                                            <Columns>
                                                                <ext:Column DataIndex="ID_Cuenta" Hidden="true" />
                                                                <ext:Column DataIndex="Tipo" Header="Tipo" Width="250"/>
                                                                <ext:Column DataIndex="Estatus" Header="Estatus" Width="125" />
                                                                <ext:NumberColumn DataIndex="SaldoActual" Align="Right" Format="$0,0.00" Header="Saldo Actual" Width="125" />
                                                            </Columns>
                                                        </ColumnModel>
                                                        <SelectionModel>
                                                            <ext:RowSelectionModel SingleSelect="true" />
                                                        </SelectionModel>
                                                        <Buttons>
                                                            <ext:Button ID="Button2" runat="server" Text="Cancelar Cuentas" Icon="Cross">
                                                                <Listeners>
                                                                    <Click Handler="commandHandlerCuentas();" />
                                                                </Listeners>
                                                            </ext:Button>
                                                        </Buttons>
                                                    </ext:GridPanel>

                                                </Items>
                                            </ext:FieldSet>
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
