<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="CentralUsuariosEditar.aspx.cs" Inherits="Usuarios.CentralUsuariosEditar" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var prepareToolbar = function (grid, toolbar, rowIndex, record) {
            if (record.get("IsLockedOut")) { //Está bloqueado
                toolbar.items.get(0).hide();

            } else { //Activo
                toolbar.items.get(1).hide();
            }
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center Split="true">
            <ext:Panel ID="Panel5" runat="server" Width="428.5" Title="Usuarios de Central Administrativa"
                Collapsed="false" Layout="Fit" AutoScroll="true" Collapsible="true">
                <Content>
                    <ext:Store ID="SAllUser" runat="server" OnRefreshData="RefreshGrid">
                        <Reader>
                            <ext:JsonReader IDProperty="UserId">
                                <Fields>
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanel1" runat="server" StoreID="SAllUser" StripeRows="true"
                                Header="false" Border="false" Layout="FitLayout" AutoExpandColumn="Email">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel1" runat="server">
                                </ColumnModel>
                                <DirectEvents>
                                    <RowDblClick OnEvent="GridUsuarios_DblClik">
                                    </RowDblClick>
                                    <Command OnEvent="EjecutarComando">
                                        <ExtraParams>
                                            <ext:Parameter Name="id" Value="record.data.ID" Mode="Raw" />
                                        </ExtraParams>
                                    </Command>
                                </DirectEvents>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                                        <DirectEvents>
                                            <RowDeselect OnEvent="QuitarSeleccion">
                                            </RowDeselect>
                                        </DirectEvents>
                                    </ext:RowSelectionModel>
                                </SelectionModel>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="SAllUser" DisplayInfo="true"
                                        DisplayMsg="Mostrando Usuarios {0} - {1} de {2}" HideRefresh="true" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </Center>
        <East Split="true">
            <ext:FormPanel ID="FormPanelEditar" Width="428.5" Collapsible="true" Collapsed="true"
                Title="Editar Usuario" runat="server" Border="false" Layout="Fit">
                <Content>
                    <ext:Store ID="SCAplicaciones" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ApplicationId">
                                <Fields>
                                    <ext:RecordField Name="ApplicationId" />
                                    <ext:RecordField Name="Description" />
                                    <ext:RecordField Name="ApplicationName" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Content>
                <Items>
                    <ext:FormPanel ID="FormPanelUserData" runat="server" Border="false" Header="false" Width="500px"
                        LabelAlign="Left" Layout="FormLayout" Padding="10">
                        <Items>
                            <ext:Hidden ID="hdnCurrentStatusIP" runat="server" />
                            <ext:Hidden ID="hdnCurrentIP" runat="server" />
                            <ext:Hidden ID="hdnNewIP" runat="server" />
                            <ext:Hidden ID="hdnCheckReset" runat="server" />
                            <ext:TextField ID="txtUsuario" FieldLabel="Usuario (ID)" runat="server" MaxLength="50"
                                MsgTarget="Side" AllowBlank="false" AnchorHorizontal="100%" ReadOnly="true" />
                            <ext:Panel ID="Panel1" runat="server" Title="Datos Generales" AutoHeight="true" LabelAlign="Top"
                                FormGroup="true" Layout="TableLayout" AutoWidth="true" Padding="10">
                                <Items>
                                    <ext:Panel ID="Panel4" runat="server" Border="false" Header="false" Width="200px"
                                        ColumnWidth=".5" Layout="Form" LabelAlign="Top">
                                        <Items>
                                            <ext:TextField ID="txtNombre" TabIndex="1" FieldLabel="Nombre" MaxLength="100" runat="server"
                                                Text="" MsgTarget="Side" AllowBlank="false" AnchorHorizontal="90%" />
                                            <ext:TextField ID="txtAmaterno" TabIndex="3" FieldLabel="Apellido Materno" MaxLength="50"
                                                runat="server" Text="" MsgTarget="Side" AllowBlank="false" AnchorHorizontal="90%" />
                                            <ext:Checkbox ID="chkBxValidateHashIpSecurity" runat="server" TabIndex="5" AnchorHorizontal="100%" Height="40"
                                                BoxLabel="Validar Nodo<span style='font-style: italic;font-family:segoe ui;font-size: 11px;'> (El usuario accederá por un solo nodo de red)</span>">
                                                <Listeners>
                                                    <AfterRender Handler="this.el.on('change', function (e, el) {
                                                        if (!el.checked) {
                                                            #{txtIP}.clear();
                                                        }
                                                     });" />
                                                </Listeners>
                                                 <ToolTips>
                                                    <ext:ToolTip runat="server" Html="Al marcar Validar Nodo, debes ingresar una Dirección IP válida." TrackMouse="true" />
                                                </ToolTips>
                                            </ext:Checkbox>
                                            <ext:TextField ID="txtIP" runat="server" TabIndex="7" FieldLabel="Dirección IP" MaxLength="15"
                                                MaskRe="[0-9\.]" AnchorHorizontal="90%" />
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel ID="Panel8" runat="server" Border="false" Header="false" ColumnWidth=".5"
                                        Width="200px" Layout="Form" LabelAlign="Top">
                                        <Items>
                                            <ext:TextField ID="txtApaterno" TabIndex="2" FieldLabel="Apellido Paterno" runat="server"
                                                Text="" MsgTarget="Side" AllowBlank="false" AnchorHorizontal="90%" />
                                            <ext:TextField ID="txtEmail" Vtype="email" TabIndex="4" FieldLabel="Email" MaxLength="50"
                                                runat="server" Text="" MsgTarget="Side" AllowBlank="false" AnchorHorizontal="90%"
                                                ReadOnly="true" />
                                            <ext:Checkbox ID="chkBxRstPwd" runat="server" TabIndex="6" BoxLabel="Restablecer Contraseña"
                                                AnchorHorizontal="90%">
                                                <Listeners>
                                                    <AfterRender Handler="this.el.on('change', function (e, el) {
                                                        #{hdnCheckReset}.setValue(el.checked); });" />
                                                </Listeners>
                                            </ext:Checkbox>
                                        </Items>
                                    </ext:Panel>
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel3" runat="server" Title="Asignar Aplicación a Usuario" AutoHeight="true"
                                LabelAlign="Top" FormGroup="true" Layout="TableLayout" Width="500px" Padding="10">
                                <Items>
                                    <ext:Panel ID="Panel7" runat="server" Border="false" Header="false" Width="200px"
                                        ColumnWidth=".5" Layout="Form" LabelAlign="Top">
                                        <Items>
                                            <ext:Hidden ID="hdnAppId" runat="server" />
                                            <ext:ComboBox ID="cmbAplicacion" EmptyText="Selecciona una Opción" TabIndex="8" FieldLabel="Aplicaciones" MaxLength="100"
                                                runat="server" Text="" MsgTarget="Side" AllowBlank="false" AnchorHorizontal="90%"
                                                StoreID="SCAplicaciones" DisplayField="Description" ValueField="ApplicationName">
                                                <Listeners>
                                                    <Select Handler="#{hdnAppId}.setValue(record.get('ApplicationId'));" />
                                                </Listeners>
                                            </ext:ComboBox>
                                       </Items>
                                    </ext:Panel>
                                </Items>
                            </ext:Panel>
                        </Items>
                        <FooterBar>
                            <ext:Toolbar runat="server" EnableOverflow="true">
                                <Items>
                                    <ext:Button ID="btnValidaModNodo" runat="server" Hidden="true">
                                        <Listeners>
                                            <Click Handler="
                                                Ext.MessageBox.show({
                                                    title: 'Advertencia',
                                                    msg: 'Al modificar los valores sobre la validación del nodo, el usuario ya no tendrá acceso al sitio hasta que reestablezca su contraseña, ¿Confirmas y continuas con la modificación?',
                                                    icon: Ext.MessageBox.WARNING,
                                                    buttons: Ext.MessageBox.YESNO,
                                                    fn: function (btn) {
                                                        if (btn == 'yes') {
                                                            EditaUsuarios.ModificaNodo(true);
                                                        }
                                                        else {
                                                            EditaUsuarios.ModificaNodo(false);
                                                        }
                                                    }
                                                });" />
                                        </Listeners>
                                    </ext:Button>
                                    <ext:Button ID="btnGuardar" runat="server" Text="Aceptar" Icon="Tick">
                                        <DirectEvents>
                                            <Click OnEvent="btnGuardar_Click" Before="var valid= #{FormPanelEditar}.getForm().isValid();
                                                if ((#{chkBxValidateHashIpSecurity}.checked && !#{txtIP}.getValue())
                                                || (!#{chkBxValidateHashIpSecurity}.checked && #{txtIP}.getValue())) { 
                                                Ext.Msg.alert('Editar Usuario', 'Los campos Validar Nodo y Dirección IP deben tener ambos, o no, un valor.'); 
                                                valid = false } if (!valid) {} return valid;">
                                                <EventMask ShowMask="true" Msg="Procesando..." MinDelay="500" />
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnCancelar" runat="server" Text="Cancelar" Icon="Cancel">
                                        <DirectEvents>
                                            <Click OnEvent="btnCancelar_Click" />
                                        </DirectEvents>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </FooterBar>
                    </ext:FormPanel>
                </Items>
            </ext:FormPanel>
        </East>
    </ext:BorderLayout>
</asp:Content>
