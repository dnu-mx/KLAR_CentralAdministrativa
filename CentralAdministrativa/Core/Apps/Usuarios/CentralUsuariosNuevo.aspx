<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="CentralUsuariosNuevo.aspx.cs" Inherits="Usuarios.CentralUsuariosNuevo" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true">
            <ext:FormPanel ID="FormPanel1" Width="428.5" Title="Agregar Usuario"  runat="server"
                Border="false" Layout="FitLayout">
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
                    <ext:FormPanel ID="Panel55" runat="server" Border="false" Header="false" Width="500px" AutoScroll="true"
                        Layout="FormLayout">
                        <Items>
                            <ext:Panel ID="Panel1" runat="server" Title="Datos Generales" AutoHeight="true" FormGroup="true" Layout="TableLayout"
                                Width="400px" Padding="10">
                                <Items>
                                    <ext:Panel ID="Panel4" runat="server" Border="false" Header="false" Width="200px"
                                        ColumnWidth=".5" Layout="Form" LabelAlign="Top">
                                        <Items>
                                            <ext:TextField TabIndex="1" ID="txtUsuario" FieldLabel="Usuario (ID)" runat="server" MaxLength="200"
                                                MsgTarget="Side" AllowBlank="false" AnchorHorizontal="90%" />
                                            <ext:TextField TabIndex="3" ID="txtPass2" FieldLabel="*   Repite la Contraseña"
                                                MaxLength="200" InputType="Password" runat="server" MsgTarget="Side" AllowBlank="false"
                                                AnchorHorizontal="90%" />
                                            <ext:Checkbox ID="chkBxValidateHashIpSecurity" runat="server" TabIndex="5" AnchorHorizontal="100%" Height="40"
                                                BoxLabel="Validar Nodo<span style='font-style: italic;font-family:segoe ui;font-size: 11px;'> (El Usuario accederá por un solo nodo de red)</span>">
                                                <ToolTips>
                                                    <ext:ToolTip runat="server" Html="Al marcar Validar Nodo, debes ingresar una Dirección IP válida." TrackMouse="true" />
                                                </ToolTips>
                                            </ext:Checkbox>
                                            <ext:TextField ID="TextFieldIP" runat="server" TabIndex="7" FieldLabel="Dirección IP" MaxLength="15"
                                                AnchorHorizontal="90%" MaskRe="[0-9\.]"/>
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel ID="Panel8" runat="server" Border="false" Header="false" ColumnWidth=".5"
                                        Width="200px" Layout="Form" LabelAlign="Top">
                                        <Items>
                                            <ext:TextField ID="txtPass1" TabIndex="2" FieldLabel="*   Contraseña"
                                                MaxLength="200" InputType="Password" runat="server" MsgTarget="Side" AllowBlank="false" AnchorHorizontal="90%">
                                            </ext:TextField>
                                            <ext:Label runat="server" LabelSeparator=" " StyleSpec="font-style: italic;font-family:segoe ui;font-size: 11px;"
                                                Text="*  La contraseña debe estar compuesta de al menos 7 caracteres. Debe contener al menos una letra minúscula, una letra mayúscula, un número y un caracter especial." />
                                        </Items>
                                    </ext:Panel>
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel2" runat="server" Title="Datos Personales" AutoHeight="true" Padding="10" LabelAlign="Top" FormGroup="true"
                                Width="400px">
                                <Items>
                                    <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false">
                                        <Defaults>
                                            <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                        </Defaults>
                                        <LayoutConfig>
                                            <ext:HBoxLayoutConfig Align="Top" />
                                        </LayoutConfig>
                                        <Items>
                                            <ext:TextField ID="txtNombre" FieldLabel="Nombre" MaxLength="100" runat="server"
                                                MsgTarget="Side" AllowBlank="false" Height="50" Width="180" TabIndex="8" />
                                            <ext:Hidden runat="server" Flex="1" Width="20" />
                                            <ext:TextField ID="txtApaterno" FieldLabel="Apellido Paterno" runat="server" Text=""
                                                MsgTarget="Side" AllowBlank="false" Width="180" TabIndex="9" />
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false">
                                        <Defaults>
                                            <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                        </Defaults>
                                        <LayoutConfig>
                                            <ext:HBoxLayoutConfig Align="Top" />
                                        </LayoutConfig>
                                        <Items>
                                            <ext:TextField ID="txtAmaterno" FieldLabel="Apellido Materno" MaxLength="50" runat="server"
                                                Text="" MsgTarget="Side" AllowBlank="false" Height="50" Width="180" TabIndex="10" />
                                            <ext:Hidden runat="server" Flex="1" Width="20" />
                                            <ext:TextField ID="txtEmail" Vtype="email" FieldLabel="Correo Electrónico" MaxLength="50"
                                                runat="server" MsgTarget="Side" AllowBlank="false" Width="180" TabIndex="11" />
                                        </Items>
                                    </ext:Panel>
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel3" runat="server" Title="Asignar Aplicación a Usuario" AutoHeight="true"
                                LabelAlign="Top" FormGroup="true" Layout="TableLayout" Width="400px" Padding="10">
                                <Items>
                                    <ext:Hidden ID="hdnAppId" runat="server" />
                                    <ext:Panel ID="Panel7" runat="server" Border="false" Header="false" Width="200px"
                                        ColumnWidth=".5" Layout="Form" LabelAlign="Top">
                                        <Items>
                                            <ext:ComboBox ID="cmbAplicacion" EmptyText="Selecciona una Opción" TabIndex="12" FieldLabel="Aplicaciones" MaxLength="100"
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
                            <ext:Toolbar ID="Toolbar1" runat="server" EnableOverflow="true">
                                <Items>
                                    <ext:Button ID="btnGuardar" TabIndex="13" runat="server" Text="Aceptar" Icon="Tick">
                                        <DirectEvents>
                                            <Click OnEvent="btnGuardar_Click" Before="var valid= #{FormPanel1}.getForm().isValid(); 
                                                if ((#{chkBxValidateHashIpSecurity}.checked && !#{TextFieldIP}.getValue())
                                                || (!#{chkBxValidateHashIpSecurity}.checked && #{TextFieldIP}.getValue())) {
                                                Ext.Msg.alert('Agregar Usuario', 'Los campos Validar Nodo y Dirección IP deben tener ambos, o no, un valor.'); 
                                                valid = false } if (!valid) { } return valid;">
                                                <EventMask ShowMask="true" Msg="Creando Usuario..." MinDelay="500" />
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </FooterBar>
                    </ext:FormPanel>
                </Items>
            </ext:FormPanel>
        </West>
        <Center Split="true">
            <ext:Panel ID="Panel5" runat="server" Width="428.5" Title="Usuarios de Central Administrativa"
                Collapsed="false"  Collapsible="true" Layout="Fit" AutoScroll="true" >
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
                                Header="false" Border="false" AutoExpandColumn="Email">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel1" runat="server">
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                                    </ext:RowSelectionModel>
                                </SelectionModel>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="SAllUser" DisplayInfo="true"
                                        DisplayMsg="Mostrando Colectivas {0} - {1} de {2}" HideRefresh="true" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
