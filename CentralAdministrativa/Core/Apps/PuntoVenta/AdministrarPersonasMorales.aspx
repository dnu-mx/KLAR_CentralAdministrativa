<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="AdministrarPersonasMorales.aspx.cs" Inherits="TpvWeb.AdministrarPersonasMorales" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
   <script type="text/javascript">
       var prepareToolbar = function (grid, toolbar, rowIndex, record) {
            if (record.get("Eliminar") != 1) { //Si no puede eliminar documentos
                toolbar.items.get(1).hide();
            } 
        }
   </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Hidden ID="hdnNew" runat="server" />
    <ext:Hidden ID="hdnEstatus" runat="server"/>
    <ext:Hidden ID="hdnIdPM" runat="server" />
    <ext:Hidden ID="hdnClaveCliente" runat="server" />
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
    <ext:Store ID="StoreEstatusCuenta" runat="server">
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
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true">
            <ext:Panel ID="MainPanel" runat="server" Width="350" Border="false">
                <Content>
                    <ext:BorderLayout ID="LeftBorderLayout" runat="server">
                        <North Split="true">
                            <ext:FormPanel ID="FormPanelBusqueda" runat="server" Title="Consulta" Height="190" Frame="true" LabelWidth="120"
                                Collapsible="true" Border="false">
                                <Items>
                                    <ext:FieldSet ID="FieldSetBusqueda" runat="server" Border="false">
                                        <Items>
                                            <ext:ComboBox ID="cBoxClientes" runat="server" FieldLabel="SubEmisor   <span style='color:red;'>*   </span>"
                                                StoreID="StoreColectivas" DisplayField="NombreORazonSocial" Width="310" ValueField="ClaveColectiva"
                                                Mode="Local" AutoSelect="true" ForceSelection="true" TypeAhead="true" MinChars="1" MatchFieldWidth="false"
                                                Name="col_" AllowBlank="false" />
                                            <ext:ComboBox ID="cBoxEstatus" runat="server" FieldLabel="Estatus   <span style='color:red;'>*   </span>"
                                                AllowBlank="false" Width="310" DisplayField="Descripcion" ValueField="ID_Estatus">
                                                <Store>
                                                    <ext:Store ID="StoreEstatusPM" runat="server">
                                                        <Reader>
                                                            <ext:JsonReader IDProperty="ID_Estatus">
                                                                <Fields>
                                                                    <ext:RecordField Name="ID_Estatus" />
                                                                    <ext:RecordField Name="Clave" />
                                                                    <ext:RecordField Name="Descripcion" />
                                                                </Fields>
                                                            </ext:JsonReader>
                                                        </Reader>
                                                    </ext:Store>
                                                </Store>
                                                <Listeners>
                                                    <Select Handler="var stat = record.get('Clave'); #{hdnEstatus}.setValue(stat);" />
                                                </Listeners>
                                            </ext:ComboBox>
                                            <ext:TextField ID="txtRazSoc" runat="server" FieldLabel="Razón Social" Width="310" />
                                            <ext:TextField ID="txt_CLABE" runat="server" FieldLabel="CLABE" Width="310" MinLength="8" MaxLength="18" />
                                        </Items>
                                        <Buttons>
                                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                                <DirectEvents>
                                                    <Click OnEvent="btnLimpiar_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnBuscar_Click" Before="if (!#{cBoxClientes}.getValue() || !#{cBoxEstatus}.getValue())
                                                        { Ext.Msg.alert('Consulta', 'Ingresa todos los criterios de búsqueda'); return false; }
                                                        else { #{GridResultados}.getStore().removeAll(); #{PanelCentral}.setTitle(' ');
                                                        #{hdnNew}.setValue(0); #{cBoxClienteNPM}.clear(); #{cBoxClienteNPM}.setDisabled(true);
                                                        #{btnNuevaPM}.hide(); #{btnGenerarCta}.hide(); #{btnRechazar}.hide(); #{btnARevision}.hide(); }">
                                                        <EventMask ShowMask="true" Msg="Buscando Personas Morales..." MinDelay="500" />
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Buttons>
                                    </ext:FieldSet>
                                </Items>
                            </ext:FormPanel>
                        </North>
                        <Center Split="true">
                            <ext:FormPanel ID="FormPanelResultados" runat="server" Title="Resultados Consulta" Layout="FitLayout" Border="false">
                                <Items>
                                    <ext:GridPanel ID="GridResultados" runat="server" Height="450" AutoDoLayout="true" Border="false">
                                        <Store>
                                            <ext:Store ID="StorePersonas" runat="server">
                                                <Reader>
                                                    <ext:JsonReader IDProperty="IdPersonaMoral">
                                                        <Fields>
                                                            <ext:RecordField Name="IdPersonaMoral" />
                                                            <ext:RecordField Name="RazonSocial" />
                                                            <ext:RecordField Name="RFC" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Store>
                                        <ColumnModel runat="server">
                                            <Columns>
                                                <ext:Column DataIndex="IdPersonaMoral" Hidden="true" />
                                                <ext:Column DataIndex="RazonSocial" Header="Razón Social" Width="220" />
                                                <ext:Column DataIndex="RFC" Header="RFC" Width="120" />
                                            </Columns>
                                        </ColumnModel>
                                        <SelectionModel>
                                            <ext:RowSelectionModel SingleSelect="true" />
                                        </SelectionModel>
                                        <DirectEvents>
                                            <RowClick OnEvent="selectRowResultados_Event">
                                                <EventMask ShowMask="true" Msg="Obteniendo Información de la Persona Moral..." MinDelay="500" />
                                                <ExtraParams>
                                                    <ext:Parameter Name="Values" Value="Ext.encode(#{GridResultados}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                                </ExtraParams>
                                            </RowClick>
                                        </DirectEvents>
                                        <BottomBar>
                                            <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StorePersonas" DisplayInfo="true" HideRefresh="true"
                                                DisplayMsg="Mostrando Personas Morales {0} - {1} de {2}" />
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
            <ext:Panel ID="PanelCentral" runat="server" Border="false" Title="-">
                <TopBar>
                    <ext:Toolbar runat="server">
                        <Items>
                            <ext:ComboBox ID="cBoxClienteNPM" runat="server" ListWidth="200" Width="200" Name="col_n"
                                EmptyText="Selecciona el SubEmisor..." StoreID="StoreColectivas" Hidden="true"
                                DisplayField="NombreORazonSocial" ValueField="ClaveColectiva" Mode="Local" AutoSelect="true"
                                ForceSelection="true" TypeAhead="true" MinChars="1" MatchFieldWidth="false">
                                <Triggers>
                                    <ext:FieldTrigger Icon="Clear" Qtip="Borrar" />
                                </Triggers>
                                <Listeners>
                                    <TriggerClick Handler="this.removeByValue(this.getValue());this.clearValue();" />
                                    <Select Handler="if (this.getValue()) { #{btnNuevaPM}.setDisabled(false); }
                                        else { #{btnNuevaPM}.setDisabled(true); }" />
                                </Listeners>
                            </ext:ComboBox>
                            <ext:Button ID="btnNuevaPM" runat="server" Text="Nueva Persona Moral" Icon="Add" Hidden="true"
                                ToolTip="Crear nueva persona moral" Disabled="true">
                                <DirectEvents>
                                    <Click OnEvent="btnNuevaPM_Click" Before="if (#{cBoxClienteNPM}.getValue()) 
                                        { #{hdnNew}.setValue(1); #{hdnEstatus}.setValue(''); 
                                        #{hdnClaveCliente}.setValue(#{cBoxClienteNPM}.getValue()); return true; } 
                                        else { return false; }" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarFill runat="server" />
                            <ext:Button ID="btnARevision" runat="server" Text="Dejar en Revisión" Icon="CheckError" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="btnARevision_Click" Before="if (!#{txtRazonSocial}.getValue()) return false;">
                                        <Confirmation ConfirmRequest="true" BeforeConfirm="if (!#{txtRazonSocial}.getValue()) return false;" 
                                            Title="Confirmación" Message="¿Estás seguro de dejar en revisón a la Persona Moral?" />
                                        <EventMask ShowMask="true" Msg="Procesando..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnRechazar" runat="server" Text="Rechazar" Icon="Cross" Hidden="false">
                                <DirectEvents>
                                    <Click OnEvent="btnRechazar_Click" Before="if (!#{txtRazonSocial}.getValue()) return false;">
                                        <Confirmation ConfirmRequest="true" Title="Confirmación"
                                            Message="¿Estás seguro de rechazar la Persona Moral?" />
                                        <EventMask ShowMask="true" Msg="Rechazando..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnGenerarCta" runat="server" Text="Autorizar y Generar Cuenta" Icon="Tick" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="btnGenerarCta_Click" Before="if (!#{txtRazonSocial}.getValue()) return false;">
                                        <Confirmation ConfirmRequest="true" BeforeConfirm="if (!#{txtRazonSocial}.getValue()) return false;" 
                                            Title="Confirmación" Message="¿Estás seguro de autorizar la Persona Moral y generar su cuenta?" />
                                        <EventMask ShowMask="true" Msg="Generando Cuenta..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <Items>
                    <ext:BorderLayout runat="server">
                        <Center>
                            <ext:TabPanel ID="TabPanel1" runat="server" Border="false">
                                <Items>
                                    <ext:FormPanel ID="FormPanelInfoGral" runat="server" Title="Información General" AutoScroll="true" Border="false"
                                        LabelWidth="200" Disabled="true">
                                        <Items>
                                            <ext:Panel runat="server" AutoHeight="true" LabelAlign="Left" FormGroup="true" Layout="FormLayout"
                                                Width="560px" Padding="10" Title=" ">
                                                <Items>
                                                    <ext:TextField ID="txtEstatus" runat="server" FieldLabel="Estatus" Width="330" ReadOnly="true" />
                                                    <ext:TextField ID="txtRazonSocial" runat="server" FieldLabel="Razón Social    <span style='color:red;'>*   </span>"
                                                        Width="330" AllowBlank="false" MinLength="200" MaxLength="200" />
                                                    <ext:TextField ID="txtCLABE" runat="server" FieldLabel="CLABE" Width="330" MinLength="18" MaxLength="18"
                                                        MaskRe="[0-9]"/>
                                                    <ext:TextField ID="txtRFC" runat="server" FieldLabel="RFC" MinLength="13" MaxLength="13" Width="330" />
                                                    <ext:TextField ID="txtTelefono" runat="server" FieldLabel="Teléfono" Width="330" MaskRe="[0-9]"
                                                        MaxLength="20" />
                                                    <ext:TextField ID="txtCorreo" runat="server" FieldLabel="Correo Electrónico" Width="330"
                                                        MaxLength="100" />
                                                    <ext:TextField ID="txtGiro" runat="server" FieldLabel="Giro de la Empresa" Width="330"
                                                        MaxLength="200" />
                                                    <ext:TextField ID="txtRepLegal" runat="server" FieldLabel="Nombre del Representante Legal"
                                                        Width="330" MaxLength="200" />
                                                    <ext:TextField ID="txtCentroCostos" runat="server" FieldLabel="Nombre del Centro de Costos"
                                                        Width="330" MaxLength="100" />
                                                </Items>
                                                <Buttons>
                                                    <ext:Button ID="btnInfoGral" runat="server" Text="Guardar" Icon="Disk">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnInfoGral_Click" Before="if (!#{txtRazonSocial}.getValue())
                                                                { return false; } else { return true; }">
                                                                <EventMask ShowMask="true" Msg="Guardando Información General..." MinDelay="500" />
                                                            </Click>
                                                        </DirectEvents>
                                                    </ext:Button>
                                                </Buttons>
                                            </ext:Panel>
                                            <ext:Panel runat="server" Title="Cuenta" AutoHeight="true" LabelAlign="Left" FormGroup="true"
                                                Layout="FormLayout" Width="560px" Padding="10">
                                                <Items>
                                                    <ext:TextField ID="txtTarjeta" runat="server" FieldLabel="Número de Tarjeta" Width="330" ReadOnly="true" />
                                                    <ext:TextField ID="txtTipoMan" runat="server" FieldLabel="Tipo de Manufactura" Width="330" ReadOnly="true" />
                                                </Items>
                                            </ext:Panel>
                                        </Items>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelDomicilio" runat="server" Title="Domicilio" LabelAlign="Left" AutoScroll="true" Border="false"
                                        LabelWidth="200" Disabled="true">
                                        <Items>
                                            <ext:Panel runat="server" Title=" " AutoHeight="true" LabelAlign="Left" FormGroup="true"
                                                Layout="FormLayout" Width="560px" Padding="10">
                                                <Items>
                                                    <ext:TextField ID="txtCalle" runat="server" FieldLabel="Calle" Width="330" MaxLength="100" />
                                                    <ext:TextField ID="txtNumExterior" runat="server" FieldLabel="Número Exterior" MaxLength="10"
                                                        Width="330" />
                                                    <ext:TextField ID="txtNumInterior" runat="server" FieldLabel="Número Interior" MaxLength="10"
                                                        Width="330" />
                                                    <ext:TextField ID="txtEntreCalles" runat="server" FieldLabel="Entre las Calles" MaxLength="400"
                                                        Width="330" />
                                                    <ext:TextField ID="txtReferencias" runat="server" FieldLabel="Referencias" Width="330"
                                                        MaxLength="400" />
                                                    <ext:TextField ID="txtCodigoPostal" runat="server" FieldLabel="Código Postal" MinLength="5"
                                                        Width="330" EnableKeyEvents="true">
                                                        <Listeners>
                                                            <SpecialKey Handler="if (e.getKey() == e.TAB || e.getKey() == e.ENTER) {
                                                                #{cBoxColonia}.reset(); PersonasMorales.LlenaComboColonias();}" />
                                                        </Listeners>
                                                    </ext:TextField>
                                                    <ext:ComboBox ID="cBoxColonia" runat="server" FieldLabel="Colonia" ValueField="ID_Colonia"
                                                        Width="330" DisplayField="Colonia">
                                                        <Store>
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
                                                        </Store>
                                                    </ext:ComboBox>
                                                    <ext:TextField ID="txtMunicipio" runat="server" FieldLabel="Delegación o Municipio"
                                                        MaxLength="100" Width="330" />
                                                    <ext:Hidden runat="server" Width="15" />
                                                    <ext:TextField ID="txtCiudad" runat="server" FieldLabel="Ciudad" MaxLength="100"
                                                        Width="330" />
                                                    <ext:TextField ID="txtEstado" runat="server" FieldLabel="Estado" MaxLength="100"
                                                        Width="330" />
                                                    <ext:ComboBox ID="cBoxPais" runat="server" FieldLabel="País" Width="330" Mode="Local"
                                                        DisplayField="Descripcion" ValueField="ID_Pais" AutoSelect="true" Editable="true"
                                                        ForceSelection="true" TypeAhead="true" MinChars="1" MatchFieldWidth="false"
                                                        ListWidth="300">
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
                                                    <ext:TextField ID="txtLatitud" runat="server" FieldLabel="Latitud" MaxLength="20"
                                                        MaskRe="[0-9\-.]" Width="330" />
                                                    <ext:TextField ID="txtLongitud" runat="server" FieldLabel="Longitud" MaxLength="20"
                                                        MaskRe="[0-9\-.]" Width="330" />
                                                </Items>
                                                <Buttons>
                                                    <ext:Button ID="btnDomicilio" runat="server" Text="Guardar" Icon="Disk">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnDomicilio_Click" Before="if (!#{txtRazonSocial}.getValue())
                                                                { Ext.MessageBox.show({
                                                                    title: 'Domicilio',
                                                                    icon: Ext.MessageBox.INFO,
                                                                    msg: 'No es posible guardar los cambios. La Razón Social es un dato obligatorio.',
                                                                    buttons: Ext.MessageBox.OK }); return false; }
                                                                else if ( !#{txtCalle}.getValue() &&
                                                                    !#{txtNumExterior}.getValue() &&
                                                                    !#{txtNumInterior}.getValue() &&
                                                                    !#{txtEntreCalles}.getValue() &&
                                                                    !#{txtReferencias}.getValue() &&
                                                                    !#{txtCodigoPostal}.getValue() &&
                                                                    !#{cBoxColonia}.getValue() &&
                                                                    !#{txtMunicipio}.getValue() &&
                                                                    !#{txtCiudad}.getValue() &&
                                                                    !#{txtEstado}.getValue() &&
                                                                    !#{cBoxPais}.getValue() &&
                                                                    !#{txtLatitud}.getValue() &&
                                                                    !#{txtLongitud}.getValue() ) { return false; }
                                                                else { return true; } ">
                                                                <EventMask ShowMask="true" Msg="Guardando Domicilio..." MinDelay="500" />
                                                            </Click>
                                                        </DirectEvents>
                                                    </ext:Button>
                                                </Buttons>
                                            </ext:Panel>
                                        </Items>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelCuestionario" runat="server" Title="Cuestionario" LabelAlign="Left" AutoScroll="true" Border="false"
                                        Disabled="true">
                                        <Items>
                                            <ext:Panel runat="server" Title="Selecciona la respuesta" AutoHeight="true" LabelAlign="Left" FormGroup="true"
                                                Layout="FormLayout" Width="560px" Padding="10" LabelWidth="430">
                                                <Items>
                                                    <ext:ComboBox ID="cBoxP1" runat="server" FieldLabel="1. ¿Alguno de los socios o accionistas desempeña 
                                                        o ha desempeñado funciones públicas destacadas en un país extranjero o en territorio nacional?"
                                                        Width="110" LabelSeparator=" ">
                                                        <Items>
                                                            <ext:ListItem Text="Sí" Value="Si" />
                                                            <ext:ListItem Text="No" Value="No" />
                                                        </Items>
                                                    </ext:ComboBox>
                                                    <ext:ComboBox ID="cBoxP2" runat="server" FieldLabel="2. ¿Alguno de los socios o accionistas es cónyuge, 
                                                        concubina, concubinario o mantiene parentesco por consanguinidad o afinidad hasta el segundo grado 
                                                        con una persona políticamente expuesta?"
                                                        Width="110" LabelSeparator=" ">
                                                        <Items>
                                                            <ext:ListItem Text="Sí" Value="Si" />
                                                            <ext:ListItem Text="No" Value="No" />
                                                        </Items>
                                                    </ext:ComboBox>
                                                    <ext:ComboBox ID="cBoxP3" runat="server" FieldLabel="3. ¿Alguno de los socios o accionistas es socio o 
                                                        asociado de una persona políticamente expuesta en alguna empresa?"
                                                        Width="110" LabelSeparator=" ">
                                                        <Items>
                                                            <ext:ListItem Text="Sí" Value="Si" />
                                                            <ext:ListItem Text="No" Value="No" />
                                                        </Items>
                                                    </ext:ComboBox>
                                                </Items>
                                            </ext:Panel>
                                            <ext:Panel runat="server" Title="Captura la respuesta" AutoHeight="true" LabelAlign="Left" FormGroup="true"
                                                Layout="FormLayout" Width="560px" Padding="10" LabelWidth="275">
                                                <Items>
                                                    <ext:TextField ID="txtP4" runat="server" MaskRe="[0-9]" Width="260" MaxLength="100"
                                                        FieldLabel="4. ¿Cuánto dinero piensa ingresar en la wallet al mes?" LabelSeparator=" " />
                                                    <ext:TextField ID="txtP5" runat="server" MaskRe="[0-9\.]" Width="260" MaxLength="100"
                                                        FieldLabel="5. ¿Cuántas veces piensa ingresar dinero en la wallet al mes?" LabelSeparator=" " />
                                                    <ext:TextField ID="txtP6" runat="server" Width="260" MaxLength="100" LabelSeparator=" "
                                                        FieldLabel="6. ¿Cada cuándo piensa ingresar dinero en la wallet al mes (frecuencia)?" />
                                                    <ext:TextArea ID="txtP7" runat="server" Width="260" Height="50" FieldLabel="7. Origen de los recursos"
                                                        MaxLength="500" />
                                                    <ext:TextArea ID="txtP8" runat="server" Width="260" Height="50" FieldLabel="8. Destino de los recursos"
                                                        MaxLength="500" />
                                                    <ext:TextArea ID="txtP9" runat="server" Width="260" Height="50" FieldLabel="9. Tipo de los recursos"
                                                        MaxLength="500" />
                                                    <ext:TextArea ID="txtP10" runat="server" Width="260" Height="50" FieldLabel="10. Naturaleza de los recursos"
                                                        MaxLength="500" />
                                                </Items>
                                                <Buttons>
                                                    <ext:Button ID="btnCuestionario" runat="server" Text="Guardar" Icon="Disk">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnCuestionario_Click" Before="if (!#{txtRazonSocial}.getValue())
                                                                { Ext.MessageBox.show({
                                                                    title: 'Cuestionario',
                                                                    icon: Ext.MessageBox.INFO,
                                                                    msg: 'No es posible guardar los cambios. La Razón Social es un dato obligatorio.',
                                                                    buttons: Ext.MessageBox.OK }); return false; }
                                                                else if ( !#{cBoxP1}.getValue() && !#{cBoxP2}.getValue() && 
                                                                !#{cBoxP3}.getValue() && !#{txtP4}.getValue() && !#{txtP5}.getValue() &&
                                                                !#{txtP6}.getValue() && !#{txtP7}.getValue() && !#{txtP8}.getValue() &&
                                                                !#{txtP9}.getValue() && !#{txtP10}.getValue() ) { return false; } else { return true; } ">
                                                                <EventMask ShowMask="true" Msg="Guardando Cuestionario..." MinDelay="500" />
                                                            </Click>
                                                        </DirectEvents>
                                                    </ext:Button>
                                                </Buttons>
                                            </ext:Panel>
                                        </Items>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelDocumentos" runat="server" Title="Documentos" Layout="FitLayout" Border="false"
                                        Disabled="true" AutoHeight="true">
                                        <Items>
                                            <ext:GridPanel ID="GridDocs" runat="server" Header="true" AutoWidth="true" AutoScroll="true"
                                                Border="false" Height="452">
                                                <Store>
                                                    <ext:Store ID="StoreDocumentos" runat="server" AutoLoad="false">
                                                        <DirectEventConfig IsUpload="true" />
                                                        <Reader>
                                                            <ext:JsonReader IDProperty="ID_DocumentoPM">
                                                                <Fields>
                                                                    <ext:RecordField Name="ID_DocumentoPM" />
                                                                    <ext:RecordField Name="Nombre" />
                                                                    <ext:RecordField Name="Eliminar" />
                                                                </Fields>
                                                            </ext:JsonReader>
                                                        </Reader>
                                                    </ext:Store>
                                                </Store>
                                                <TopBar>
                                                    <ext:Toolbar ID="ToolbarDocs" runat="server">
                                                        <Items>
                                                            <ext:FileUploadField ID="fufDocumentos" runat="server" ButtonText="Examinar..." Width="230"
                                                                Icon="Magnifier" />
                                                            <ext:ToolbarFill runat="server" />
                                                            <ext:Button ID="btnAgregaDoc" runat="server" Text="Añadir Documento"
                                                                Icon="PageWhiteAdd">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnAgregaDoc_Click" IsUpload="true" Before="if (
                                                                        !#{txtRazonSocial}.getValue()) { Ext.MessageBox.show({
                                                                        title: 'Documentos',
                                                                        icon: Ext.MessageBox.INFO,
                                                                        msg: 'No es posible añadir documentos. La Razón Social es un dato obligatorio.',
                                                                        buttons: Ext.MessageBox.OK }); return false; }"
                                                                        After="#{fufDocumentos}.reset();">
                                                                        <EventMask ShowMask="true" Msg="Añadiendo documento..." MinDelay="500" />
                                                                    </Click>
                                                                </DirectEvents>
                                                            </ext:Button>
                                                        </Items>
                                                    </ext:Toolbar>
                                                </TopBar>
                                                <ColumnModel runat="server">
                                                    <Columns>
                                                        <ext:Column DataIndex="ID_DocumentoPM" Hidden="true" />
                                                        <ext:Column DataIndex="Eliminar" Hidden="true" />
                                                        <ext:Column DataIndex="Nombre" Header="Nombre del Documento" Width="450" />
                                                        <ext:CommandColumn Width="100" Header="Acciones">
                                                            <PrepareToolbar Fn="prepareToolbar" />
                                                            <Commands>
                                                                <ext:GridCommand Icon="PageWhitePut" CommandName="Descargar">
                                                                    <ToolTip Text="Descargar" />
                                                                </ext:GridCommand>
                                                                <ext:GridCommand Icon="Cross" CommandName="Eliminar">
                                                                    <ToolTip Text="Eliminar" />
                                                                </ext:GridCommand>
                                                            </Commands>
                                                        </ext:CommandColumn>
                                                    </Columns>
                                                </ColumnModel>
                                                <SelectionModel>
                                                    <ext:RowSelectionModel SingleSelect="true" />
                                                </SelectionModel>
                                                <DirectEvents>
                                                    <Command OnEvent="EjecutarComando">
                                                        <Confirmation ConfirmRequest="true" BeforeConfirm="if (command == 'Descargar') return false;" 
                                                            Title="Confirmación" Message="¿Estás seguro de eliminar el documento de la Persona Moral?" />
                                                        <ExtraParams>
                                                            <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                                            <ext:Parameter Name="ID_DocumentoPM" Value="Ext.encode(record.data.ID_DocumentoPM)" Mode="Raw" />
                                                            <ext:Parameter Name="Nombre" Value="Ext.encode(record.data.Nombre)" Mode="Raw" />
                                                        </ExtraParams>
                                                    </Command>
                                                </DirectEvents>
                                                <BottomBar>
                                                    <ext:PagingToolbar ID="PagingToolBar3" runat="server" StoreID="StoreDocumentos" DisplayInfo="true"
                                                        DisplayMsg="Documentos {0} - {1} de {2}" />
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
