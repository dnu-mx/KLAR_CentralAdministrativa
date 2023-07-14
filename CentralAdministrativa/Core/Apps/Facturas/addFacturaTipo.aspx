<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="addFacturaTipo.aspx.cs" Inherits="Facturas.addFacturaTipo" %>

<%@ Import Namespace="System.Xml.Xsl" %>
<%@ Import Namespace="System.Xml" %>
<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .cbStates-list
        {
            width: auto;
            font: 11px tahoma,arial,helvetica,sans-serif;
        }
        
        .cbStates-list th
        {
            font-weight: bold;
        }
        
        .cbStates-list td, .cbStates-list th
        {
            padding: 3px;
        }
    </style>
    <script type="text/javascript">
        // this "setGroupStyle" function is called when the GroupingView is refreshed.     
        var setGroupStyle = function (view) {
            // get an instance of the Groups
            var groups = view.getGroups();

            for (var i = 0; i < groups.length; i++) {
                var spans = Ext.query("span", groups[i]);

                if (spans && spans.length > 0) {
                    // Loop through the Groups, the do a query to find the <span> with our ColorCode
                    // Get the "id" from the <span> and split on the "-", the second array item should be our ColorCode
                    var color = "#" + spans[0].id.split("-")[1];

                    // Set the "background-color" of the original Group node.
                    Ext.get(groups[i]).setStyle("background-color", color);
                }
            }
        };
    </script>
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

        var submitValue = function (grid, hiddenFormat, format) {
            hiddenFormat.setValue(format);
            grid.submitData(false);
        };

        var template = '<span style="color:{0};">{1}</span>';

        var change = function (value) {
            return String.format(template, (value > 0) ? "green" : "red", value);
        };

        var pctChange = function (value) {
            return String.format(template, (value > 0) ? "green" : "red", value + "%");
        }

        var validaFechas = function () {

        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Window ID="frmAddDetalles" Title="Agregar Detalle Factura Tipo" Icon="ApplicationFormAdd"
        runat="server" Width="460" Height="320" Resizable="False" Hidden="true" Closable="true"
        Modal="true" Layout="FitLayout" Draggable="true" Padding="12">
        <Items>
            <ext:FormPanel ID="frmNuevo" runat="server" AnchorVertical="30%" Frame="true" Flex="1"
                Layout="FormLayout">
                <Items>
                    <ext:TextField ID="txtIDFacturaTipo" FieldLabel="Factura Tipo" Enabled="false" Disabled="true"
                        runat="server" Width="300" MsgTarget="Side" AllowBlank="false" AnchorHorizontal="90%">
                    </ext:TextField>
                    <ext:TextField ID="txtID_DetalleFacturaTipo" FieldLabel="ID" Enabled="false" Disabled="true"
                        runat="server" Width="300" MsgTarget="Side" AllowBlank="false" AnchorHorizontal="90%">
                    </ext:TextField>
                    <ext:ComboBox ID="cmbCadenaComercial" TabIndex="3" FieldLabel="Cadena Comercial"
                        EmptyText="Selecciona una Opción" Resizable="true" AllowBlank="false" Width="300"
                        AnchorHorizontal="90%" runat="server" StoreID="StoreCadenaComercial" DisplayField="NombreORazonSocial"
                        MaxWidth="300" ValueField="ID_Colectiva" Mode="Local" AutoSelect="true" Editable="true" 
                        ForceSelection="true" TypeAhead="true" MinChars="1" MatchFieldWidth="false" Name="colCadena">
                    </ext:ComboBox>
                    <ext:ComboBox FieldLabel="Evento" ID="cmbEventos" TabIndex="1" ForceSelection="true"
                        EmptyText="Selecciona una Opción..." runat="server" Width="300" StoreID="stEventosManuales"
                        MsgTarget="Side" AllowBlank="false" DisplayField="Descripcion" ValueField="ID_Evento"
                        Editable="false" AnchorHorizontal="90%" Resizable="true" TypeAhead="true" Mode="Local"
                        MinChars="1">
                    </ext:ComboBox>
                    <ext:ComboBox ID="cmbTipoCuenta" TabIndex="3" FieldLabel="Tipo Cuenta" EmptyText="Selecciona una Opción"
                        Resizable="true" AllowBlank="false" Width="300" AnchorHorizontal="90%" runat="server"
                        StoreID="StoreTipoCuenta" DisplayField="Descripcion" MaxWidth="300" ValueField="ID_TipoCuenta">
                    </ext:ComboBox>
                    <ext:TextField ID="txtformulaCantidad" FieldLabel="Formula Cantidad" runat="server"
                        Width="300" MsgTarget="Side" AnchorHorizontal="90%">
                    </ext:TextField>
                    <ext:TextField ID="txtformulaPrecioUnitario" FieldLabel="Formula PU" runat="server"
                        Width="300" MsgTarget="Side" AnchorHorizontal="90%">
                    </ext:TextField>
                    <ext:TextField ID="txtformulaTotalDetalle" FieldLabel="Formula Total" runat="server"
                        Width="300" MsgTarget="Side" AllowBlank="false" AnchorHorizontal="90%">
                    </ext:TextField>
                </Items>
                <BottomBar>
                    <ext:Toolbar ID="Toolbar4" runat="server">
                        <Items>
                            <ext:ToolbarFill ID="ToolbarFill2" runat="server" />
                            <ext:Button ID="btnAddDetalle" runat="server" Flat="false" Text="Guardar detalle"
                                ToolTip="Agregar el detalle" Icon="Accept">
                                <DirectEvents>
                                    <Click OnEvent="guardarDetalle" Before="var valid= #{frmNuevo}.getForm().isValid(); if (!valid) {} return valid;" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </BottomBar>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:Window ID="frmParametros" Title="Parámetros de la Factura Tipo" Icon="Pencil"
        runat="server" Width="560" Height="460" Resizable="False" Hidden="true" Closable="true"
        Modal="true" Layout="FitLayout" Draggable="true" Padding="12">
        <LayoutConfig>
            <ext:HBoxLayoutConfig Align="Stretch" DefaultMargins="2" />
        </LayoutConfig>
        <Content>
            <ext:Store ID="StoreParametros" runat="server">
                <Reader>
                    <ext:JsonReader IDProperty="ID_Evento">
                        <Fields>
                            <ext:RecordField Name="ID_Parametro" />
                            <ext:RecordField Name="EsAsignado" />
                            <ext:RecordField Name="Descripcion" />
                            <ext:RecordField Name="Nombre" />
                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>
        </Content>
        <Items>
            <ext:Hidden ID="hdnIdPeriodo" runat="server" />
            <ext:Panel ID="Panel8" runat="server" Frame="true" Flex="2" Layout="FitLayout" Title="Valores Parámetros">
                <Items>
                    <ext:PropertyGrid ID="GridPropiedades" runat="server" Header="false">
                        <Source>
                            <ext:PropertyGridParameter Name="(Los Parametros)" Value="Los Valores">
                            </ext:PropertyGridParameter>
                        </Source>
                        <DirectEvents>
                        </DirectEvents>
                        <View>
                            <ext:GridView ID="GridView22" ForceFit="true" ScrollOffset="2" runat="server" />
                        </View>
                        <FooterBar>
                            <ext:Toolbar ID="Toolbar32" runat="server" EnableOverflow="true">
                                <Items>
                                    <ext:Button ID="btnGuardar2" runat="server" Text="Guardar" Icon="Add">
                                        <DirectEvents>
                                            <Click OnEvent="btnGuardarParametros_Click" />
                                        </DirectEvents>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </FooterBar>
                    </ext:PropertyGrid>
                </Items>
            </ext:Panel>
        </Items>
    </ext:Window>
    <ext:Window ID="winFacturaTipo" Title="Agregar Detalle Factura Tipo" Icon="ApplicationFormAdd"
        runat="server" Width="460" Height="380" Resizable="False" Hidden="true" Closable="true"
        Modal="true" Layout="FitLayout" Draggable="true" Padding="12">
        <Items>
            <ext:TextField ID="txtFacturaTipoID" FieldLabel="Factura Tipo" Enabled="false" Disabled="true"
                runat="server" Width="300" MsgTarget="Side" AllowBlank="false" AnchorHorizontal="90%">
            </ext:TextField>
        </Items>
    </ext:Window>
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true" Collapsible="false">
            <ext:FormPanel ID="FormPanel2" Width="320" Title="Nueva Factura Tipo" runat="server"
                Border="false" Layout="Fit">
                <Content>
                    <ext:Hidden ID="FormatType" runat="server" />
                    <ext:Store ID="StoreTipoCuenta" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_TipoCuenta">
                                <Fields>
                                    <ext:RecordField Name="ID_TipoCuenta" />
                                    <ext:RecordField Name="Clave" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:Store ID="StoreCadenaComercial" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                                <Fields>
                                    <ext:RecordField Name="NombreORazonSocial" />
                                    <ext:RecordField Name="ClaveColectiva" />
                                    <ext:RecordField Name="ID_Colectiva" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="NombreORazonSocial" Direction="ASC" />
                    </ext:Store>
                    <ext:Store ID="StoreEmisor" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                                <Fields>
                                    <ext:RecordField Name="NombreORazonSocial" />
                                    <ext:RecordField Name="NameFin" />
                                    <ext:RecordField Name="ClaveColectiva" />
                                    <ext:RecordField Name="ID_Colectiva" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="NombreORazonSocial" Direction="ASC" />
                    </ext:Store>
                    <ext:Store ID="StoreReceptor" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                                <Fields>
                                    <ext:RecordField Name="NombreORazonSocial" />
                                    <ext:RecordField Name="NameFin" />
                                    <ext:RecordField Name="ClaveColectiva" />
                                    <ext:RecordField Name="ID_Colectiva" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="NombreORazonSocial" Direction="ASC" />
                    </ext:Store>
                    <ext:Store ID="StoreTipoColectiva" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_TipoColectiva">
                                <Fields>
                                    <ext:RecordField Name="ID_TipoColectiva" />
                                    <ext:RecordField Name="ClaveTipoColectiva" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <%--<SortInfo Field="Descripcion" Direction="ASC" />--%>
                    </ext:Store>
                    <ext:Store ID="Store2" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_TipoColectiva">
                                <Fields>
                                    <ext:RecordField Name="ID_TipoColectiva" />
                                    <ext:RecordField Name="ClaveTipoColectiva" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <%--<SortInfo Field="Descripcion" Direction="ASC" />--%>
                    </ext:Store>
                    <ext:Store ID="StoreTipoColectiva2" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_TipoColectiva">
                                <Fields>
                                    <ext:RecordField Name="ID_TipoColectiva" />
                                    <ext:RecordField Name="ClaveTipoColectiva" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <%--<SortInfo Field="Descripcion" Direction="ASC" />--%>
                    </ext:Store>
                    <ext:Store ID="StoreTipoPago" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_TipoPago">
                                <Fields>
                                    <ext:RecordField Name="ID_TipoPago" />
                                    <ext:RecordField Name="Clave" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <%--<SortInfo Field="Descripcion" Direction="ASC" />--%>
                    </ext:Store>
                    <ext:Store ID="StorePeriodo" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Periodo">
                                <Fields>
                                    <ext:RecordField Name="ID_Periodo" />
                                    <ext:RecordField Name="Clave" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <%--<SortInfo Field="Descripcion" Direction="ASC" />--%>
                    </ext:Store>
                    <ext:Store ID="StoreTipoDocto" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_TipoDocumento">
                                <Fields>
                                    <ext:RecordField Name="ID_TipoDocumento" />
                                    <ext:RecordField Name="Clave" />
                                    <ext:RecordField Name="Descripcion" />
                                    <ext:RecordField Name="Timbra" />
                                    <ext:RecordField Name="GeneraXML" />
                                    <ext:RecordField Name="CalculaIVA" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <%--<SortInfo Field="Descripcion" Direction="ASC" />--%>
                    </ext:Store>
                    <ext:Store ID="stContratos" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Contrato">
                                <Fields>
                                    <ext:RecordField Name="ID_Contrato" />
                                    <ext:RecordField Name="Clave" />
                                    <ext:RecordField Name="Descripcion" />
                                    <ext:RecordField Name="CadenaComercial" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Content>
                <Items>
                    <ext:FormPanel ID="FormPanel12" runat="server" Padding="10" AutoScroll="true">
                        <Items>
                            <ext:Panel ID="Panel7" runat="server" Title="Factura Tipo" AutoHeight="true" FormGroup="true"
                                LabelWidth="115">
                                <Items>
                                    <ext:TextField ID="txtIDFac" FieldLabel="ID" runat="server" Enabled="false" Disabled="true"
                                        MaxLength="100" MaxLengthText="100" Width="290" MsgTarget="Side" AllowBlank="false"
                                        AnchorHorizontal="90%">
                                    </ext:TextField>
                                    <ext:TextField ID="txtNombreFacturaTipo" FieldLabel="Nombre" runat="server" MaxLength="100"
                                        MaxLengthText="100" Width="290" MsgTarget="Side" AllowBlank="false" AnchorHorizontal="90%">
                                    </ext:TextField>
                                    <ext:ComboBox ID="cmbTipoFactura" TabIndex="3" FieldLabel="Tipo Documento" AllowBlank="false"
                                        EmptyText="Selecciona una Opción" Resizable="true" Width="290" runat="server"
                                        StoreID="StoreTipoDocto" DisplayField="Descripcion" MaxWidth="290" ValueField="ID_TipoDocumento">
                                    </ext:ComboBox>
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel3" runat="server" Title="Emisor de Factura" AutoHeight="true" FormGroup="true"
                                LabelWidth="115">
                                <Items>
                                    <ext:ComboBox ID="cmbTipoColectivaEmisor" FieldLabel="Tipo Colectiva" AllowBlank="false"
                                        EmptyText="Selecciona una Opción" Resizable="true" Width="290" runat="server"
                                        StoreID="StoreTipoColectiva" DisplayField="Descripcion" MaxWidth="290" ValueField="ID_TipoColectiva">
                                        <DirectEvents>
                                            <Select OnEvent="consultaEmisores">
                                            </Select>
                                        </DirectEvents>
                                    </ext:ComboBox>
                                    <ext:ComboBox ID="cmbColectivaEmisor" TabIndex="3" FieldLabel="Colectiva" AllowBlank="false"
                                        EmptyText="Selecciona una Opción" Resizable="true" Width="290" runat="server"
                                        StoreID="StoreEmisor" DisplayField="NameFin" MaxWidth="290" ValueField="ID_Colectiva"
                                        Mode="Local" AutoSelect="true" Editable="true" ForceSelection="true" TypeAhead="true"
                                        MinChars="1" MatchFieldWidth="false" Name="colEmisor">
                                    </ext:ComboBox>
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel4" runat="server" Title="Receptor de Factura" AutoHeight="true" FormGroup="true"
                                LabelWidth="115">
                                <Items>
                                    <ext:ComboBox ID="cmbtipoColectivaReceptor" AllowBlank="false" TabIndex="3" FieldLabel="Tipo Colectiva"
                                        EmptyText="Selecciona una Opción" Resizable="true" Width="290" runat="server"
                                        StoreID="StoreTipoColectiva2" DisplayField="Descripcion" MaxWidth="290" ValueField="ID_TipoColectiva">
                                        <DirectEvents>
                                            <Select OnEvent="consultaReceptores">
                                            </Select>
                                        </DirectEvents>
                                    </ext:ComboBox>
                                    <ext:ComboBox ID="cmbColectivaReceptor" TabIndex="3" AllowBlank="false" FieldLabel="Colectiva"
                                        EmptyText="Selecciona una Opción" Resizable="true" Width="290" runat="server"
                                        StoreID="StoreReceptor" DisplayField="NameFin" MaxWidth="290" ValueField="ID_Colectiva"
                                        Mode="Local" AutoSelect="true" Editable="true" ForceSelection="true" TypeAhead="true"
                                        MinChars="1" MatchFieldWidth="false" Name="colReceptor">
                                    </ext:ComboBox>
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel1" runat="server" Title="Niveles de ..." AutoHeight="true" FormGroup="true"
                                LabelWidth="115">
                                <Items>
                                    <ext:ComboBox ID="cmbNivelDatos" AllowBlank="false" TabIndex="3" FieldLabel="Datos"
                                        EmptyText="Selecciona una Opción" Resizable="true" Width="290" runat="server"
                                        StoreID="StoreTipoColectiva2" DisplayField="Descripcion" MaxWidth="290" ValueField="ID_TipoColectiva">
                                    </ext:ComboBox>
                                    <ext:ComboBox ID="cmbNivelReceptor" AllowBlank="false" TabIndex="3" FieldLabel="Receptor"
                                        EmptyText="Selecciona una Opción" Resizable="true" Width="290" runat="server"
                                        StoreID="StoreTipoColectiva2" DisplayField="Descripcion" MaxWidth="290" ValueField="ID_TipoColectiva">
                                    </ext:ComboBox>
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel5" runat="server" Title="Generación Automática" AutoHeight="true" FormGroup="true"
                                LabelWidth="115">
                                <Items>
                                    <ext:ComboBox ID="CmbPeriodo" TabIndex="3" FieldLabel="Periodo de Creación" EmptyText="Selecciona una Opción"
                                        Resizable="true" AllowBlank="false" Width="290" AnchorHorizontal="90%" runat="server"
                                        StoreID="StorePeriodo" DisplayField="Descripcion" MaxWidth="290" ValueField="ID_Periodo">
                                    </ext:ComboBox>
                                    <ext:ComboBox ID="cmbFolio" FieldLabel="Asignar Folio" runat="server" MaxLength="50" MaxLengthText="50"
                                        Width="290" MsgTarget="Side" AllowBlank="false" EmptyText="Selecciona una Opción">
                                        <Items>
                                            <ext:ListItem Text="NO" Value="false" />
                                            <ext:ListItem Text="SI" Value="true" />
                                        </Items>
                                    </ext:ComboBox>
                                    <%-- <ext:TextField ID="txtEmail" FieldLabel="Email de Aviso de Creación" runat="server"
                                        Width="300" MsgTarget="Side" AnchorHorizontal="90%">
                                    </ext:TextField>--%>
                                </Items>
                            </ext:Panel>
                            <%--     <ext:Panel ID="Panel6" runat="server" Title="Contrato" AutoHeight="true" FormGroup="true">
                                <Items>
                                    <ext:ComboBox FieldLabel="Contrato" ID="cmbContratos" AllowBlank="false" TabIndex="2"
                                        ForceSelection="true" EmptyText="Selecciona una Opción..." runat="server" Width="300"
                                        StoreID="stContratos" MsgTarget="Side" DisplayField="Descripcion" ValueField="ID_Contrato"
                                        Editable="false" AnchorHorizontal="90%" Resizable="true" TypeAhead="true" Mode="Local"
                                        MinChars="1" PageSize="10" ItemSelector="tr.list-item">
                                        <Template ID="Template2" runat="server">
                                            <Html>
                                                <tpl for=".">
						                            <tpl if="[xindex] == 1">
							                            <table class="cbStates-list">
								                            <tr>
									                            <th>Cadena Comercial</th>
									                            <th>Descripcion</th>
								                            </tr>
						                            </tpl>
						                            <tr class="list-item">
							                            <td style="padding:2px 0px;">{CadenaComercial}</td>
							                            <td>{Descripcion}</td>
						                            </tr>
						                            <tpl if="[xcount-xindex]==0">
							                            </table>
						                            </tpl>
					                            </tpl>
                                            </Html>
                                        </Template>
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
                            </ext:Panel>--%>
                            <%-- <ext:Panel ID="Panel5" runat="server" Title="Información General" AutoHeight="true"
                                FormGroup="true">
                                <Items>
                                    <ext:ComboBox ID="cmbTipoPago" TabIndex="3" FieldLabel="Forma de Pago" EmptyText="Selecciona una Opción"
                                        Resizable="true" Width="300" runat="server" StoreID="StoreTipoPago" DisplayField="Descripcion"
                                        MaxWidth="300" ValueField="ID_TipoPago">
                                    </ext:ComboBox>
                                </Items>
                            </ext:Panel>--%>
                        </Items>
                    </ext:FormPanel>
                </Items>
                <FooterBar>
                    <ext:Toolbar ID="Toolbar1" runat="server" EnableOverflow="false">
                        <Items>
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="Limpiar_Click" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnGuardar" runat="server" Text="Guardar" Icon="Add">
                                <DirectEvents>
                                    <Click OnEvent="Guardar_Click" Before="var valid= #{FormPanel12}.getForm().isValid(); if (!valid) {} return valid;" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>
            </ext:FormPanel>
        </West>
        <Center Split="true" Collapsible="false">
            <ext:Panel ID="Panel2" runat="server" Title="Las Facturas Tipo" Collapsed="false"
                Layout="Fit" AutoScroll="true">
                <Content>
                    <ext:Store ID="StoreFacturasTipo" runat="server" OnSubmitData="Store1_Submit" OnRefreshData="btnBuscar_Click"
                        RemoteSort="true">
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="ID_FacturaTipo">
                                <Fields>
                                    <ext:RecordField Name="ID_FacturaTipo" />
                                    <ext:RecordField Name="Descripcion" />
                                    <ext:RecordField Name="ID_Emisor" />
                                    <ext:RecordField Name="Emisor" />
                                    <ext:RecordField Name="ID_Receptor" />
                                    <ext:RecordField Name="Receptor" />
                                    <ext:RecordField Name="ID_Contrato" />
                                    <ext:RecordField Name="TipoColectivaNivelDatos" />
                                    <ext:RecordField Name="ID_TipoColectivaNivelDatos" />
                                    <ext:RecordField Name="ID_TipoColectivaReceptor" />
                                    <ext:RecordField Name="TipoColectivaReceptor" />
                                    <ext:RecordField Name="ID_TipoColectivaRE" />
                                    <ext:RecordField Name="ID_TipoColectivaEM" />
                                    <ext:RecordField Name="ID_Periodo" />
                                    <ext:RecordField Name="ID_TipoDocumento" />
                                    <ext:RecordField Name="DescripcionTipoDocumento" />
                                    <ext:RecordField Name="Foliada" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <DirectEventConfig IsUpload="true" />
                        <SortInfo Field="Descripcion" />
                    </ext:Store>
                    <ext:Store ID="StoreDetalleFacturaTipo" runat="server" OnSubmitData="Store1_Submit"
                        OnRefreshData="btnBuscar_Click" RemoteSort="true">
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="ID_DetalleFacturaTipo">
                                <Fields>
                                    <ext:RecordField Name="ID_DetalleFacturaTipo" />
                                    <ext:RecordField Name="Producto" />
                                    <ext:RecordField Name="Evento" />
                                    <ext:RecordField Name="ID_Evento" />
                                    <ext:RecordField Name="CadenaComercial" />
                                    <ext:RecordField Name="ID_CadenaComercial" />
                                    <ext:RecordField Name="ID_TipoCuenta" />
                                    <ext:RecordField Name="FormulaCantidad" />
                                    <ext:RecordField Name="FormulaPrecioUnitario" />
                                    <ext:RecordField Name="FormulaTotal" />
                                    <ext:RecordField Name="ID_FacturaTipo" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <DirectEventConfig IsUpload="true" />
                        <SortInfo Field="Producto" />
                    </ext:Store>
                    <ext:Store ID="stEventosManuales" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Evento">
                                <Fields>
                                    <ext:RecordField Name="ID_Evento" />
                                    <ext:RecordField Name="Clave" />
                                    <ext:RecordField Name="Descripcion" />
                                    <ext:RecordField Name="DescMostrar" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <North Split="true">
                            <ext:GridPanel ID="GridPanel1" runat="server" StoreID="StoreFacturasTipo" StripeRows="true"
                                Header="false" Height="200" Border="false">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                        <ext:CommandColumn ColumnID="acciones" Width="90" Header="">
                                            <Commands>
                                                <ext:GridCommand Icon="ApplicationFormAdd" CommandName="AddDetalle">
                                                    <ToolTip Text="Agregar un Detalle a la Factura Tipo" />
                                                </ext:GridCommand>
                                                <ext:GridCommand Icon="ApplicationFormEdit" CommandName="EditFacturaTipo">
                                                    <ToolTip Text="Editar el Encabezado de la Factura Tipo" />
                                                </ext:GridCommand>
                                                <ext:GridCommand Icon="PencilAdd" CommandName="ConfigParametros">
                                                    <ToolTip Text="Configurar parámetros de Facturación" />
                                                </ext:GridCommand>
                                                <ext:GridCommand Icon="Delete" CommandName="DeleteFacturaTipo">
                                                    <ToolTip Text="Eliminar la Factura Tipo" />
                                                </ext:GridCommand>
                                            </Commands>
                                        </ext:CommandColumn>
                                        <ext:Column ColumnID="DescripcionTipoDocumento" Header="Tipo" Sortable="true" DataIndex="DescripcionTipoDocumento" />
                                        <ext:Column ColumnID="Descripcion" Header="Descripcion" Width="200" Sortable="true"
                                            DataIndex="Descripcion" />
                                        <ext:Column ColumnID="Emisor" Header="Emisor" Sortable="true" DataIndex="Emisor" />
                                        <ext:Column ColumnID="Receptor" Header="Receptor" Sortable="true" DataIndex="Receptor" />
                                        <ext:Column ColumnID="TipoColectivaNivelDatos" Header="Nivel Datos" Sortable="true"
                                            DataIndex="TipoColectivaNivelDatos" />
                                        <ext:Column ColumnID="TipoColectivaNivelDatos" Header="Tipo Receptor" Sortable="true"
                                            DataIndex="TipoColectivaReceptor" />
                                        <ext:Column ColumnID="Foliada" Header="Foliar" Sortable="true" DataIndex="Foliada">
                                            <Renderer Handler="return (value) ? 'SI':'NO';" />
                                        </ext:Column>
                                    </Columns>
                                </ColumnModel>
                                <Listeners>
                                    <%--<Command Handler="if (command != 'DeleteFacturaTipo') {Ext.net.Mask.show({ msg : 'Procesando...' }); AddFacturaTipo.StopMask();}" />--%>
                                    <Command Handler="Ext.net.Mask.show({ msg : 'Procesando...' });" />
                                </Listeners>
                                <DirectEvents>
                                    <Command OnEvent="EjecutarComandoFactura" Timeout="360000">
                                        <Confirmation BeforeConfirm="if (command != 'DeleteFacturaTipo') return false;"
                                            ConfirmRequest="true" Title="Confirmación" Message="¿Estás seguro eliminar la Factura Tipo?" />
                                        <ExtraParams>
                                            <ext:Parameter Name="ID_FacturaTipo" Value="record.data['ID_FacturaTipo']" Mode="Raw" />
                                            <ext:Parameter Name="ID_Emisor" Value="record.data['ID_Emisor']" Mode="Raw" />
                                            <ext:Parameter Name="ID_Receptor" Value="record.data['ID_Receptor']" Mode="Raw" />
                                            <ext:Parameter Name="ID_TipoColectivaNivelDatos" Value="record.data['ID_TipoColectivaNivelDatos']"
                                                Mode="Raw" />
                                            <ext:Parameter Name="ID_TipoColectivaReceptor" Value="record.data['ID_TipoColectivaReceptor']"
                                                Mode="Raw" />
                                            <ext:Parameter Name="ID_TipoColectivaRE" Value="record.data['ID_TipoColectivaRE']"
                                                Mode="Raw" />
                                            <ext:Parameter Name="ID_TipoColectivaEM" Value="record.data['ID_TipoColectivaEM']"
                                                Mode="Raw" />
                                            <ext:Parameter Name="ID_TipoDocumento" Value="record.data['ID_TipoDocumento']" Mode="Raw" />
                                            <ext:Parameter Name="ID_Periodo" Value="record.data['ID_Periodo']" Mode="Raw" />
                                            <ext:Parameter Name="Descripcion" Value="record.data['Descripcion']" Mode="Raw" />
                                            <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                            <ext:Parameter Name="Foliada" Value="record.data['Foliada']" Mode="Raw" />
                                        </ExtraParams>
                                    </Command>
                                </DirectEvents>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                                        <DirectEvents>
                                            <RowSelect OnEvent="RowSelect" Buffer="100">
                                                <EventMask ShowMask="true" Target="CustomTarget" CustomTarget="#{Panel13}" />
                                                <ExtraParams>
                                                    <ext:Parameter Name="ID_FacturaTipo" Value="record.data['ID_FacturaTipo']" Mode="Raw" />
                                                </ExtraParams>
                                            </RowSelect>
                                        </DirectEvents>
                                    </ext:RowSelectionModel>
                                </SelectionModel>
                                <Plugins>
                                    <ext:GridFilters runat="server" ID="GridFilters1" Local="true">
                                        <Filters>
                                            <ext:StringFilter DataIndex="Descripcion" />
                                            <ext:StringFilter DataIndex="Emisor" />
                                            <ext:StringFilter DataIndex="Receptor" />
                                            <ext:StringFilter DataIndex="TipoColectivaNivelDatos" />
                                            <ext:StringFilter DataIndex="TipoColectivaReceptor" />
                                        </Filters>
                                    </ext:GridFilters>
                                </Plugins>
                                <TopBar>
                                    <ext:StatusBar ID="btnBusqueda" runat="server">
                                        <Items>
                                            <ext:TextField ID="txtBusqueda" runat="server" Width="200" MsgTarget="Side" LabelAlign="Left"
                                                EmptyText="Búsqueda por Nombre" AllowBlank="false">
                                            </ext:TextField>
                                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnBuscar_Click">
                                                        <EventMask ShowMask="true" Msg="Buscando Facturas Tipo..." MinDelay="500" />
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:StatusBar>
                                </TopBar>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreFacturasTipo"
                                        DisplayInfo="true" DisplayMsg="Mostrando FacturasTipo {0} - {1} de {2}" />
                                </BottomBar>
                            </ext:GridPanel>
                        </North>
                        <Center>
                            <ext:Panel ID="Panel13" runat="server" Border="false" Flex="1" Layout="FitLayout">
                                <LayoutConfig>
                                    <ext:HBoxLayoutConfig Align="Stretch" DefaultMargins="2" />
                                </LayoutConfig>
                                <Items>
                                    <ext:GridPanel ID="GridPanel2" runat="server" AnchorVertical="50%" StoreID="StoreDetalleFacturaTipo"
                                        StripeRows="true" Header="false" Height="200" Border="false">
                                        <LoadMask ShowMask="false" />
                                        <ColumnModel ID="ColumnModel2" runat="server">
                                            <Columns>
                                                <ext:CommandColumn ColumnID="acciones" Width="80" Header="">
                                                    <Commands>
                                                        <ext:GridCommand Icon="ApplicationFormEdit" CommandName="EditDetalle">
                                                            <ToolTip Text="Editar Detalle" />
                                                        </ext:GridCommand>
                                                        <ext:GridCommand Icon="Delete" CommandName="DeleteDetalleFacturaTipo">
                                                            <ToolTip Text="Eliminar Detalle" />
                                                        </ext:GridCommand>
                                                    </Commands>
                                                </ext:CommandColumn>
                                                <%-- <ext:Column ColumnID="ID_DetalleFacturaTipo" Header="ID_DetalleFacturaTipo" Sortable="true"
                                                    DataIndex="ID_DetalleFacturaTipo" />
                                                <ext:Column ColumnID="Producto" Header="Producto" Sortable="true" DataIndex="Producto" />
                                                <ext:Column ColumnID="Evento" Header="Evento" Sortable="true" DataIndex="Evento" />--%>
                                                <%--<ext:Column ColumnID="CadenaComercial" Header="CadenaComercial" Sortable="true" DataIndex="CadenaComercial" />--%>
                                                <ext:Column ColumnID="FormulaCantidad" Header="FormulaCantidad" Sortable="true" DataIndex="FormulaCantidad" />
                                                <ext:Column ColumnID="FormulaPrecioUnitario" Header="FormulaPrecioUnitario" Sortable="true"
                                                    DataIndex="FormulaPrecioUnitario" />
                                                <ext:Column ColumnID="FormulaTotal" Header="FormulaTotal" Sortable="true" DataIndex="FormulaTotal" />
                                            </Columns>
                                        </ColumnModel>
                                        <DirectEvents>
                                            <Command OnEvent="EjecutarComandoDetalle">
                                                <ExtraParams>
                                                    <ext:Parameter Name="ID_FacturaTipo" Value="record.data['ID_FacturaTipo']" Mode="Raw" />
                                                    <ext:Parameter Name="ID_DetalleFacturaTipo" Value="record.data['ID_DetalleFacturaTipo']"
                                                        Mode="Raw" />
                                                    <ext:Parameter Name="FormulaCantidad" Value="record.data['FormulaCantidad']" Mode="Raw" />
                                                    <ext:Parameter Name="FormulaPrecioUnitario" Value="record.data['FormulaPrecioUnitario']"
                                                        Mode="Raw" />
                                                    <ext:Parameter Name="FormulaTotal" Value="record.data['FormulaTotal']" Mode="Raw" />
                                                    <ext:Parameter Name="ID_CadenaComercial" Value="record.data['ID_CadenaComercial']"
                                                        Mode="Raw" />
                                                    <ext:Parameter Name="ID_TipoCuenta" Value="record.data['ID_TipoCuenta']" Mode="Raw" />
                                                    <ext:Parameter Name="ID_Evento" Value="record.data['ID_Evento']" Mode="Raw" />
                                                    <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                                </ExtraParams>
                                            </Command>
                                        </DirectEvents>
                                        <Plugins>
                                            <ext:GridFilters runat="server" ID="GridFilters2" Local="true">
                                                <Filters>
                                                    <ext:NumericFilter DataIndex="Producto" />
                                                    <ext:StringFilter DataIndex="Evento" />
                                                    <ext:StringFilter DataIndex="Afiliacion" />
                                                    <ext:StringFilter DataIndex="CadenaComercial" />
                                                    <ext:StringFilter DataIndex="FormulaCantidad" />
                                                    <ext:StringFilter DataIndex="FormulaPrecioUnitario" />
                                                    <ext:StringFilter DataIndex="FormulaTotal" />
                                                </Filters>
                                            </ext:GridFilters>
                                        </Plugins>
                                        <View>
                                            <ext:GroupingView ID="GroupingView2" HideGroupedColumn="true" runat="server" ForceFit="true"
                                                StartCollapsed="true" GroupTextTpl='<span id="ColorCode-{[values.rs[0].data.ColorCode]}"></span>{text} ({[values.rs.length]} {[values.rs.length > 1 ? "Items" : "Item"]})'
                                                EnableRowBody="true">
                                                <Listeners>
                                                    <Refresh Fn="setGroupStyle" />
                                                </Listeners>
                                                <GetRowClass Handler="var d = record.data; rowParams.body = String.format('<div style=\'padding:0 5px 5px 5px;\'><b></b><b></b> <b> <br/>Cadena Comercial:</b>{0}. <br/><b>Producto-Evento:</b>{1}-{2}<br /></div>', d.CadenaComercial, d.Producto, d.Evento);" />
                                            </ext:GroupingView>
                                        </View>
                                        <BottomBar>
                                            <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="StoreDetalleFacturaTipo"
                                                DisplayInfo="true" DisplayMsg="Mostrando Detalles de Factura Tipo {0} - {1} de {2}" />
                                        </BottomBar>
                                    </ext:GridPanel>
                                </Items>
                            </ext:Panel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
