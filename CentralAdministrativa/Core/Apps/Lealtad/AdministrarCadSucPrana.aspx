<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="AdministrarCadSucPrana.aspx.cs" Inherits="Lealtad.AdministrarCadSucPrana" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
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
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Store ID="StoreGiros_Suc" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_Giro">
                <Fields>
                    <ext:RecordField Name="ID_Giro" />
                    <ext:RecordField Name="Clave" />
                    <ext:RecordField Name="Descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Store ID="StoreSubGiros_Suc" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_SubGiro">
                <Fields>
                    <ext:RecordField Name="ID_SubGiro" />
                    <ext:RecordField Name="Clave" />
                    <ext:RecordField Name="Descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Store ID="StorePresencia_Suc" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_Presencia">
                <Fields>
                    <ext:RecordField Name="ID_Presencia" />
                    <ext:RecordField Name="Clave" />
                    <ext:RecordField Name="Descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Store ID="StoreClasif_Suc" runat="server">
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
    <ext:Store ID="StorePais" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ClavePais">
                <Fields>
                    <ext:RecordField Name="ClavePais" />
                    <ext:RecordField Name="Descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Store ID="StoreEstado" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="CveEstado">
                <Fields>
                    <ext:RecordField Name="CveEstado" />
                    <ext:RecordField Name="Estado" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Store ID="StorePerfilNSE_Suc" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_PerfilNSE">
                <Fields>
                    <ext:RecordField Name="ID_PerfilNSE" />
                    <ext:RecordField Name="Clave" />
                    <ext:RecordField Name="Descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Store ID="StoreTipoEstablecimiento" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_TipoEstablecimiento">
                <Fields>
                    <ext:RecordField Name="ID_TipoEstablecimiento" />
                    <ext:RecordField Name="Clave" />
                    <ext:RecordField Name="Descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:Window ID="WdwNC_Suc" runat="server" Title="Alta de Cadena" Width="350" Height="440" Hidden="true"
        Modal="true" Resizable="false" Icon="LinkAdd">
        <Items>
            <ext:FormPanel ID="FormPanelNC_Suc" runat="server" Height="410" Padding="3" LabelWidth="120" AutoScroll="true">
                <Items>
                    <ext:FieldSet runat="server" Title="Datos de la Nueva Cadena">
                        <Items>
                            <ext:TextField ID="txtClaveCadena_Suc" runat="server" FieldLabel="Clave Cadena" MaxLength="100"
                                Width="300" AllowBlank="false"/>
                            <ext:TextField ID="txtCadena_Suc" runat="server" FieldLabel="Nombre Comercial" MaxLength="200"
                                Width="300" AllowBlank="false"/>
                            <ext:ComboBox ID="cBoxGiro_Suc" runat="server" FieldLabel="Giro" Width="300" AllowBlank="false"
                                DisplayField="Descripcion" ValueField="ID_Giro" StoreID="StoreGiros_Suc" >
                                <DirectEvents>
                                    <Change OnEvent="ObtieneSubGiroSucEvent"></Change>
                                </DirectEvents>
                            </ext:ComboBox>
                                                            
                            <ext:ComboBox ID="cBoxSubGiro_Suc" runat="server" FieldLabel="SubGiro" Width="300" AllowBlank="true"
                                DisplayField="Descripcion" ValueField="ID_SubGiro" StoreID="StoreSubGiros_Suc" />
                            <ext:TextField ID="txtTicketPromedio_Suc" runat="server" FieldLabel="Ticket Promedio" MaxLength="100"
                                Width="300" AllowBlank="true"/>
                            <ext:ComboBox ID="cBoxPerfilNSE_Suc" runat="server" FieldLabel="Perfil NSE" Width="300" AllowBlank="true"
                                DisplayField="Descripcion" ValueField="ID_PerfilNSE" StoreID="StorePerfilNSE_Suc" />
                            <ext:ComboBox ID="cBoxPresencia_Suc" runat="server" FieldLabel="Presencia" Width="300"
                                AllowBlank="false" DisplayField="Descripcion" ValueField="ID_Presencia"
                                StoreID="StorePresencia_Suc" />
                            <ext:ComboBox ID="cBoxTipoEstablecimiento_Suc" runat="server" FieldLabel="Tipo Establecimiento" Width="300" AllowBlank="true"
                                DisplayField="Descripcion" ValueField="ID_TipoEstablecimiento" StoreID="StoreTipoEstablecimiento" />

                            <ext:TextArea ID="txtFacebook_Suc" runat="server" FieldLabel="Facebook" MaxLength="250"
                                Width="300" Height="100"/>
                            <ext:TextArea ID="txtWeb_Suc" runat="server" FieldLabel="Web" MaxLength="250"
                                Width="300" Height="100"/>
                            <ext:TextField ID="txtCuentaCLABE_Suc" runat="server" FieldLabel="Cuenta CLABE" MaxLength="18"
                                Width="300" AllowBlank="true" MaskRe="[0-9]"/>
                            <ext:TextField ID="txtContacto_Suc" runat="server" FieldLabel="Contacto" MaxLength="150"
                                Width="300" AllowBlank="true"/>
                            <ext:TextField ID="txtTelContacto_Suc" runat="server" FieldLabel="Tel. Contacto" MaxLength="20"
                                Width="300" AllowBlank="true"/>
                            <ext:TextField ID="txtCargo_Suc" runat="server" FieldLabel="Cargo" MaxLength="20"
                                Width="300" AllowBlank="true"/>
                            <ext:TextField ID="txtCelContacto_Suc" runat="server" FieldLabel="Cel. Contacto" MaxLength="20"
                                Width="300" AllowBlank="true"/>
                            <ext:TextField ID="txtCorreo_Suc" runat="server" FieldLabel="Correo" MaxLength="50"
                                Width="300" AllowBlank="true"/>
                            <ext:TextArea ID="txtExtracto_Suc" runat="server" FieldLabel="Extracto" MaxLength="5000"
                                Width="300" Height="100"/>
                        </Items>
                    </ext:FieldSet>
                </Items>
                <Buttons>
                    <ext:Button runat="server" Text="Cancelar" Icon="Cancel">
                        <Listeners>
                            <Click Handler="#{WdwNC_Suc}.hide();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button ID="btnAddCadena_Suc" runat="server" Text="Crear Cadena" Icon="Tick">
                        <DirectEvents>
                            <Click OnEvent="btnAddCadena_SucClick" Before="var valid= #{FormPanelNC_Suc}.getForm().isValid(); if (!valid) {} return valid;">
                                <EventMask ShowMask="true" Msg="Creando Cadena..." MinDelay="500" />
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:Window ID="WdwNuevaSucursal" runat="server" Title="Alta de Sucursal" Width="350" Height="510" Hidden="true"
        Modal="true" Resizable="false" Icon="HouseStar">
        <Items>
            <ext:FormPanel ID="FormPanelNuevaSuc" runat="server" Height="480" Padding="3" LabelWidth="90">
                <Items>
                    <ext:FieldSet runat="server" Title="Datos de la Nueva Sucursal">
                        <Items>
                            <ext:TextField ID="txtClaveNuevaSuc" runat="server" FieldLabel="Clave" MaxLength="50"
                                Width="300" AllowBlank="false" />
                            <ext:TextField ID="txtNombreNuevaSuc" runat="server" FieldLabel="Nombre" MaxLength="150"
                                Width="300" AllowBlank="false" />
                            <ext:TextArea ID="txtDirecNuevaSuc" runat="server" FieldLabel="Dirección" MaxLength="4000"
                                Width="300" Height="100" AllowBlank="false" />
                            <ext:TextField ID="txtColNuevaSuc" runat="server" FieldLabel="Colonia" MaxLength="150" Width="300" />
                            <ext:TextField ID="txtCdNuevaSuc" runat="server" FieldLabel="Ciudad" MaxLength="50" Width="300" />
                            <ext:TextField ID="txtCpNuevaSuc" runat="server" FieldLabel="Código Postal" MinLength="5" MaxLength="10"
                                Width="300" MaskRe="[0-9]" />
                            <ext:ComboBox ID="cBoxPaisNuevaSuc" runat="server" FieldLabel="País" MaxLength="50" Width="300"
                                AllowBlank="false" StoreID="StorePais" DisplayField="Descripcion" ValueField="ClavePais" >
                                <DirectEvents>
                                    <Select OnEvent="EstableceEstadosNuevo" Before="#{cBoxEstadoNuevaSuc}.clearValue();">
                                        <EventMask ShowMask="true" Msg="Estableciendo Estados..." MinDelay="200" />
                                    </Select>
                                </DirectEvents>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cBoxEstadoNuevaSuc" runat="server" FieldLabel="Estado" MaxLength="50" Width="300"
                                AllowBlank="false" StoreID="StoreEstado" DisplayField="Estado" ValueField="CveEstado" />
                            <ext:TextField ID="txtTelNuevaSuc" runat="server" FieldLabel="Teléfono" MaxLength="50" Width="300"
                                MaskRe="[0-9\-()]"/>
                            <ext:TextField ID="txtLatNuevaSuc" runat="server" FieldLabel="Latitud" MaxLength="50" Width="300"
                                MaskRe="[0-9\.-]" />
                            <ext:TextField ID="txtLongNuevaSuc" runat="server" FieldLabel="Longitud" MaxLength="50" Width="300"
                                MaskRe="[0-9\.-]" />
                            <ext:ComboBox ID="cBoxClasifNuevaSuc" runat="server" FieldLabel="Clasificación" Width="300" AllowBlank="false"
                                DisplayField="Clave" ValueField="ID_Clasificacion" StoreID="StoreClasif_Suc" />
                        </Items>
                    </ext:FieldSet>
                </Items>
                <Buttons>
                    <ext:Button runat="server" Text="Cancelar" Icon="Cancel">
                        <Listeners>
                            <Click Handler="#{WdwNuevaSucursal}.hide();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button ID="btnAddSucursal" runat="server" Text="Crear Sucursal" Icon="Tick">
                        <DirectEvents>
                            <Click OnEvent="btnAddSucursal_Click" Before="var valid= #{FormPanelNuevaSuc}.getForm().isValid(); if (!valid) {} return valid;">
                                <EventMask ShowMask="true" Msg="Creando Sucursal..." MinDelay="500" />
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true">
            <ext:Panel runat="server" Width="325" Border="false" Layout="FitLayout" Title="Consulta de Cadenas">
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
                                            <ext:Button runat="server" Icon="LinkAdd" ToolTip="Crear Nueva Cadena"
                                                Text="Nueva Cadena">
                                                <Listeners>
                                                    <Click Handler="#{FormPanelNC_Suc}.reset(); #{WdwNC_Suc}.show();" />
                                                </Listeners>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </Items>
                            </ext:FormPanel>
                        </South>
                        <Center Split="true">
                            <ext:GridPanel ID="GridResultados" runat="server" AutoExpandColumn="NombreComercial" Border="false"
                                Header="false">
                                <TopBar>
                                    <ext:Toolbar runat="server">
                                        <Items>
                                            <ext:TextField ID="txtClaveCad" runat="server" EmptyText="Clave Cadena" Width="100"/>
                                            <ext:TextField ID="txtNombreCom" runat="server" EmptyText="Nombre Comercial" MaxLength="200" />
                                            <ext:Button ID="btnBuscarCad" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnBuscarCad_Click" Before="#{PanelCentralCadSuc}.setTitle('');
                                                        #{PanelCentralCadSuc}.setDisabled(true);
                                                        if (!#{txtClaveCad}.getValue() && !#{txtNombreCom}.getValue())
                                                        { return false; }">
                                                        <EventMask ShowMask="true" Msg="Buscando Cadenas..." MinDelay="500" />
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <Store>
                                    <ext:Store ID="StoreCadenas_Suc" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Cadena">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Cadena" />
                                                    <ext:RecordField Name="ClaveCadena" />
                                                    <ext:RecordField Name="NombreComercial" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <ColumnModel runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_Cadena" Hidden="true" />
                                        <ext:Column DataIndex="ClaveCadena" Header="Clave" Width="90" />
                                        <ext:Column DataIndex="NombreComercial" Header="Nombre Comercial" Width="110" />
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel SingleSelect="true" />
                                </SelectionModel>
                                <DirectEvents>
                                    <RowClick OnEvent="selectRowResultados_Event">
                                        <EventMask ShowMask="true" Msg="Obteniendo Información de la Cadena..." MinDelay="500" />
                                        <ExtraParams>
                                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridResultados}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                        </ExtraParams>
                                    </RowClick>
                                </DirectEvents>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreCadenas_Suc" DisplayInfo="true"
                                        DisplayMsg="{0} - {1} de {2}" HideRefresh="true" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </West>
        <Center Split="true">
            <ext:Panel ID="PanelCentralCadSuc" runat="server" Height="250" Border="false" Title="_" Disabled="true">
                <Items>
                    <ext:BorderLayout runat="server">
                        <Center>
                            <ext:TabPanel runat="server">
                                <Items>
                                    <ext:FormPanel ID="FormPanelInfoAdCadena" runat="server" Title="Cadena" LabelAlign="Left" LabelWidth="120"
                                        AutoScroll="true">
                                        <Items>
                                            <ext:FieldSet runat="server" Title="Datos de la Cadena">
                                                <Items>
                                                    <ext:Hidden ID="hdnIdCad_Suc" runat="server" />
                                                    <ext:TextField ID="txtClaveCadInfoAd" runat="server" FieldLabel="Clave Cadena" MaxLength="100"
                                                        Width="550" Disabled="true" />
                                                    <ext:TextField ID="txtNombreComInfoAd" runat="server" FieldLabel="Nombre Comercial" MaxLength="200"
                                                        Width="550"  AllowBlank="false" />
                                                    <ext:ComboBox ID="cBoxGiroInfoAd" runat="server" FieldLabel="Giro" Width="550" AllowBlank="false"
                                                        DisplayField="Descripcion" ValueField="ID_Giro" StoreID="StoreGiros_Suc">
                                                        <DirectEvents>
                                                            <Change OnEvent="ObtieneSubGiroInfoEvent"></Change>
                                                        </DirectEvents>
                                                    </ext:ComboBox>

                                                    <ext:ComboBox ID="cBoxSubGiroInfoAd" runat="server" FieldLabel="SubGiro" Width="550" AllowBlank="true"
                                                        DisplayField="Descripcion" ValueField="ID_SubGiro" StoreID="StoreSubGiros_Suc" />
                                                    <ext:TextField ID="txtTicketPromedioInfoAd" runat="server" FieldLabel="Ticket Promedio" MaxLength="100"
                                                        Width="550" AllowBlank="true"/>
                                                    <ext:ComboBox ID="cBoxPerfilNSEInfoAd" runat="server" FieldLabel="Perfil NSE" Width="550" AllowBlank="true"
                                                        DisplayField="Descripcion" ValueField="ID_PerfilNSE" StoreID="StorePerfilNSE_Suc" />
                                                    <ext:ComboBox ID="cBoxPresenciaInfoAd" runat="server" FieldLabel="Presencia" Width="550"
                                                        AllowBlank="false" DisplayField="Descripcion" ValueField="ID_Presencia"
                                                        StoreID="StorePresencia_Suc" />

                                                    <ext:ComboBox ID="cBoxTipoEstablecimientoInfoAd" runat="server" FieldLabel="Tipo Establecimiento" Width="550"
                                                        AllowBlank="true" DisplayField="Descripcion" ValueField="ID_TipoEstablecimiento"
                                                        StoreID="StoreTipoEstablecimiento" />

                                                    <ext:TextArea ID="txtFacebookInfoAd" runat="server" FieldLabel="Facebook" MaxLength="250"
                                                        Width="550" Height="100" />
                                                    <ext:TextArea ID="txtWebInfoAd" runat="server" FieldLabel="Web" MaxLength="250"
                                                        Width="550" Height="100" />
                                                    <ext:TextField ID="txtCuentaCLABEInfoAd" runat="server" FieldLabel="Cuenta CLABE" MaxLength="18"
                                                        Width="550" AllowBlank="true" MaskRe="[0-9]"/>
                                                    <ext:TextField ID="txtContactoInfoAd" runat="server" FieldLabel="Contacto" MaxLength="150"
                                                        Width="550" AllowBlank="true"/>
                                                    <ext:TextField ID="txtTelContactoInfoAd" runat="server" FieldLabel="Tel. Contacto" MaxLength="20"
                                                        Width="550" AllowBlank="true"/>
                                                    <ext:TextField ID="txtCargoInfoAd" runat="server" FieldLabel="Cargo" MaxLength="20"
                                                        Width="550" AllowBlank="true"/>
                                                    <ext:TextField ID="txtCelContactoInfoAd" runat="server" FieldLabel="Cel. Contacto" MaxLength="20"
                                                        Width="550" AllowBlank="true"/>
                                                    <ext:TextField ID="txtCorreoInfoAd" runat="server" FieldLabel="Correo" MaxLength="50"
                                                        Width="550" AllowBlank="true"/>
                                                    <ext:TextArea ID="txtExtractoInfoAd" runat="server" FieldLabel="Extracto" MaxLength="5000"
                                                        Width="550" Height="100" />
                                                </Items>
                                                <Buttons>
                                                    <ext:Button ID="btnGuardaInfoAdCadena" runat="server" Text="Guardar" Icon="Disk">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnGuardarInfoAd_Click" Before="var valid= #{FormPanelInfoAdCadena}.getForm().isValid(); if (!valid) {} return valid;">
                                                                <EventMask ShowMask="true" Msg="Guardando Cambios..." MinDelay="500" />
                                                            </Click>
                                                        </DirectEvents>
                                                    </ext:Button>
                                                </Buttons>
                                            </ext:FieldSet>
                                        </Items>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelSucursales" runat="server" Title="Sucursales" Layout="FitLayout" Border="false">
                                        <Items>
                                            <ext:Panel runat="server" Border="false">
                                                <Content>
                                                    <ext:BorderLayout runat="server">
                                                        <Center Split="true">
                                                            <ext:Panel runat="server" Border="false">
                                                                <Content>
                                                                    <ext:BorderLayout runat="server">
                                                                        <Center Split="true">
                                                                            <ext:GridPanel ID="GridSucursales" runat="server" AutoExpandColumn="Nombre" Border="false" Header="false"
                                                                                Layout="FitLayout">
                                                                                <TopBar>
                                                                                    <ext:Toolbar runat="server">
                                                                                        <Items>
                                                                                            <ext:TextField ID="txtClaveSuc" runat="server" EmptyText="Clave Sucursal" Width="100" />
                                                                                            <ext:TextField ID="_txtNombreSuc" runat="server" EmptyText="Nombre" />
                                                                                            <ext:Button ID="btnBuscarSuc" runat="server" Text="Buscar" Icon="Magnifier">
                                                                                                <DirectEvents>
                                                                                                    <Click OnEvent="btnBuscarSuc_Click" Before="if (!#{txtClaveSuc}.getValue() &&
                                                                                                        !#{_txtNombreSuc}.getValue()) { return false; }">
                                                                                                        <EventMask ShowMask="true" Msg="Buscando Sucursales..." MinDelay="500" />
                                                                                                    </Click>
                                                                                                </DirectEvents>
                                                                                            </ext:Button>
                                                                                        </Items>
                                                                                    </ext:Toolbar>
                                                                                </TopBar>
                                                                                <Store>
                                                                                    <ext:Store ID="StoreSucursales" runat="server">
                                                                                        <Reader>
                                                                                            <ext:JsonReader IDProperty="ID_Sucursal">
                                                                                                <Fields>
                                                                                                    <ext:RecordField Name="ID_Sucursal" />
                                                                                                    <ext:RecordField Name="Clave" />
                                                                                                    <ext:RecordField Name="Nombre" />
                                                                                                </Fields>
                                                                                            </ext:JsonReader>
                                                                                        </Reader>
                                                                                    </ext:Store>
                                                                                </Store>
                                                                                <ColumnModel runat="server">
                                                                                    <Columns>
                                                                                        <ext:Column DataIndex="ID_Sucursal" Hidden="true" />
                                                                                        <ext:Column DataIndex="Clave" Header="Clave" Width="120" />
                                                                                        <ext:Column DataIndex="Nombre" Header="Nombre" Width="120" />
                                                                                    </Columns>
                                                                                </ColumnModel>
                                                                                <SelectionModel>
                                                                                    <ext:RowSelectionModel SingleSelect="true" />
                                                                                </SelectionModel>
                                                                                <DirectEvents>
                                                                                    <RowClick OnEvent="selectRowSucursal_Event">
                                                                                        <EventMask ShowMask="true" Msg="Obteniendo Información de la Sucursal..." MinDelay="500" />
                                                                                        <ExtraParams>
                                                                                            <ext:Parameter Name="SucValues" Value="Ext.encode(#{GridSucursales}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                                                                        </ExtraParams>
                                                                                    </RowClick>
                                                                                </DirectEvents>
                                                                                <BottomBar>
                                                                                    <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="StoreSucursales" DisplayInfo="true"
                                                                                        DisplayMsg="{0} - {1} de {2}" HideRefresh="true" />
                                                                                </BottomBar>
                                                                            </ext:GridPanel>
                                                                        </Center>
                                                                        <South>
                                                                            <ext:FormPanel runat="server" Height="25" Border="false">
                                                                                <Items>
                                                                                    <ext:Toolbar runat="server">
                                                                                        <Items>
                                                                                            <ext:ToolbarFill runat="server" />
                                                                                            <ext:Button runat="server" Icon="HouseStar" ToolTip="Crear Nueva Sucursal"
                                                                                                Text="Nueva Sucursal">
                                                                                                <Listeners>
                                                                                                    <Click Handler="#{FormPanelNuevaSuc}.reset(); #{WdwNuevaSucursal}.show();" />
                                                                                                </Listeners>
                                                                                            </ext:Button>
                                                                                        </Items>
                                                                                    </ext:Toolbar>
                                                                                </Items>
                                                                            </ext:FormPanel>
                                                                        </South>
                                                                    </ext:BorderLayout>
                                                                </Content>
                                                            </ext:Panel>
                                                        </Center>
                                                        <East Split="true">
                                                            <ext:FormPanel ID="FormPanelDatosSuc" runat="server" Title="Sucursal" LabelAlign="Top" Width="340"
                                                                Padding="5" Layout="FormLayout" AutoScroll="true" Disabled="true" Collapsible="true">
                                                                <Items>
                                                                    <ext:Hidden ID="hdnIdSucursal" runat="server" />
                                                                    <ext:TextField ID="txtClaveSucInfo" runat="server" FieldLabel="Clave" MaxLength="50"
                                                                        Width="310" Disabled="true" />
                                                                    <ext:TextField ID="txtNombreSucInfo" runat="server" FieldLabel="Nombre" MaxLength="150"
                                                                        Width="310" AllowBlank="false" />
                                                                    <ext:TextArea ID="txtDireccion" runat="server" FieldLabel="Dirección" MaxLength="4000"
                                                                        Width="310" Height="85" AllowBlank="false" />
                                                                    <ext:TextField ID="txtColonia" runat="server" FieldLabel="Colonia" MaxLength="150" Width="310" />
                                                                    <ext:TextField ID="txtCiudad" runat="server" FieldLabel="Ciudad" MaxLength="50" Width="310" />
                                                                    <ext:TextField ID="txtCP" runat="server" FieldLabel="Código Postal" MinLength="5" MaxLength="10"
                                                                        Width="310" MaskRe="[0-9]" />
                                                                     <ext:ComboBox ID="cBoxPais" runat="server" FieldLabel="País" MaxLength="50" Width="300"
                                                                          AllowBlank="false" StoreID="StorePais" DisplayField="Descripcion" ValueField="ClavePais" >
                                                                          <DirectEvents>
                                                                            <Select OnEvent="EstableceEstados" Before="#{cBoxEstado}.clearValue();">
                                                                                <EventMask ShowMask="true" Msg="Estableciendo Estados..." MinDelay="200" />
                                                                            </Select>
                                                                        </DirectEvents>
                                                                    </ext:ComboBox>
                                                                    <ext:ComboBox ID="cBoxEstado" runat="server" FieldLabel="Estado" MaxLength="50" Width="310"
                                                                        AllowBlank="false" StoreID="StoreEstado" DisplayField="Estado" ValueField="CveEstado" />
                                                                    <ext:TextField ID="txtTelefono" runat="server" FieldLabel="Teléfono" MaxLength="50" Width="310"
                                                                        MaskRe="[0-9\-()]" />
                                                                    <ext:TextField ID="txtLatitud" runat="server" FieldLabel="Latitud" MaxLength="50" Width="310"
                                                                        AllowBlank="false" MaskRe="[0-9\.-]" />
                                                                    <ext:TextField ID="txtLongitud" runat="server" FieldLabel="Longitud" MaxLength="50" Width="310"
                                                                        AllowBlank="false" MaskRe="[0-9\.-]" />
                                                                    <ext:ComboBox ID="cBoxSucActiva" runat="server" FieldLabel="Activa" Width="310" AllowBlank="false">
                                                                        <Items>
                                                                            <ext:ListItem Text="Sí" Value="1" />
                                                                            <ext:ListItem Text="No" Value="0" />
                                                                        </Items>
                                                                    </ext:ComboBox>
                                                                    <ext:ComboBox ID="cBoxClasificacionSuc" runat="server" FieldLabel="Clasificación" Width="310"
                                                                        AllowBlank="false" DisplayField="Clave" ValueField="ID_Clasificacion" StoreID="StoreClasif_Suc" />
                                                                </Items>
                                                                <Buttons>
                                                                    <ext:Button ID="btnGuardaInfoSuc" runat="server" Text="Guardar" Icon="Disk" Disabled="true">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="btnGuardaInfoSuc_Click" Before="var valid= #{FormPanelDatosSuc}.getForm().isValid(); if (!valid) {} return valid;">
                                                                                <EventMask ShowMask="true" Msg="Guardando Cambios..." MinDelay="500" />
                                                                            </Click>
                                                                        </DirectEvents>
                                                                    </ext:Button>
                                                                </Buttons>
                                                            </ext:FormPanel>
                                                        </East>
                                                    </ext:BorderLayout>
                                                </Content>
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
