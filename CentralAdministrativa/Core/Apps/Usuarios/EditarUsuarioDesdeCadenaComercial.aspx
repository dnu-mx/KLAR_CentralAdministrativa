<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EditarUsuarioDesdeCadenaComercial.aspx.cs" Inherits="Usuarios.EditarUsuarioDesdeCadenaComercial" %>
<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">

        var prepareToolbar = function (grid, toolbar, rowIndex, record) {
            // for example hide 'Edit' button if price < 50
            if (record.get("IsLockedOut")) { //Activo

                //toolbar.items.get(0).hide(); //Delete
                toolbar.items.get(0).hide(); //asgina
                //toolbar.items.get(1).hide(); //asgina
                // toolbar.items.get(2).hide(); //asgina
                // toolbar.items.get(3).hide(); //asgina

            } else { //Otro no Activo
                //toolbar.items.get(0).hide(); //asgina
                toolbar.items.get(1).hide(); //asgina
                // toolbar.items.get(2).hide(); //asgina
                // toolbar.items.get(3).hide(); //asgina

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
                                Header="false" Border="false">
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
                                        DisplayMsg="Mostrando Usuarios {0} - {1} de {2}" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </Center>
        <East Split="true">
            <ext:FormPanel ID="FormPanelEditar" Width="428.5" Collapsible="true"  Collapsed="true" Title="Editar Usuario" runat="server"
                Border="false" Layout="Fit">
                <Content>
                    <ext:Store ID="SCColectiva" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                                <Fields>
                                    <ext:RecordField Name="ID_Colectiva" />
                                    <ext:RecordField Name="NombreORazonSocial" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:Store ID="SCTipoColectiva" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_TipoColectiva">
                                <Fields>
                                    <ext:RecordField Name="ID_TipoColectiva" />
                                    <ext:RecordField Name="ClaveTipoColectiva" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Content>
                <Items>
                    <ext:FormPanel ID="Panel55" runat="server" Border="false" Header="false" Width="500px"
                        LabelAlign="Left" Layout="FormLayout" Padding="10">
                        <Items>
                            <ext:Panel ID="Panel1" runat="server" Title="Datos Usuario" AutoHeight="true" LabelAlign="Top"
                                FormGroup="true" Layout="TableLayout" AutoWidth="true">
                                <Items>
                                    <ext:Panel ID="Panel4" runat="server" Border="false" Header="false" Width="200px"
                                        ColumnWidth=".5" Layout="Form" LabelAlign="Top">
                                        <Items>
                                            <ext:TextField TabIndex="1" ID="txtUsuario" FieldLabel="UserID" runat="server" MaxLength="20"
                                                MsgTarget="Side" AllowBlank="false" AnchorHorizontal="90%" />
                                            <ext:TextField TabIndex="3" ID="txtPass2" FieldLabel="Repite Contraseña" MaxLength="20"
                                                InputType="Password" runat="server" MsgTarget="Side" AllowBlank="false" AnchorHorizontal="90%" />
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel ID="Panel8" runat="server" Border="false" Header="false" ColumnWidth=".5"
                                        Width="200px" Layout="Form" LabelAlign="Top">
                                        <Items>
                                            <ext:TextField ID="txtPass1" TabIndex="2" FieldLabel="Contraseña" MaxLength="20" InputType="Password"
                                                runat="server" MsgTarget="Side" AllowBlank="false" AnchorHorizontal="90%">
                                            </ext:TextField>
                                        </Items>
                                    </ext:Panel>
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel2" runat="server" Title="Datos Personales" AutoHeight="true"
                                LabelAlign="Top" FormGroup="true" Layout="TableLayout" AutoWidth="true">
                                <Items>
                                    <ext:Panel ID="Panel6" runat="server" Border="false" Header="false" Width="200px"
                                        ColumnWidth=".5" Layout="Form" LabelAlign="Top">
                                        <Items>
                                            <ext:TextField ID="txtNombre" TabIndex="3" FieldLabel="Nombre" MaxLength="100" runat="server"
                                                Text="" MsgTarget="Side" AllowBlank="false" AnchorHorizontal="90%" />
                                            <ext:TextField ID="txtAmaterno" TabIndex="5" FieldLabel="Apellido Materno" MaxLength="50"
                                                runat="server" Text="" MsgTarget="Side" AllowBlank="false" AnchorHorizontal="90%" />
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel ID="Panel9" runat="server" Border="false" Header="false" ColumnWidth=".5"
                                        Width="200px" Layout="Form" LabelAlign="Top">
                                        <Items>
                                            <ext:TextField ID="txtApaterno" TabIndex="4" FieldLabel="Apellido Paterno" runat="server"
                                                Text="" MsgTarget="Side" AllowBlank="false" AnchorHorizontal="90%" />
                                            <ext:TextField ID="txtEmail" Vtype="email" TabIndex="6" FieldLabel="Email" MaxLength="50" runat="server"
                                                Text="" MsgTarget="Side" AllowBlank="false" AnchorHorizontal="90%" />
                                        </Items>
                                    </ext:Panel>
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel3" runat="server" Title="Nivel de Administracion" AutoHeight="true"
                                LabelAlign="Top" FormGroup="true" Layout="TableLayout" AutoWidth="true">
                                <Items>
                                    <ext:Panel ID="Panel7" runat="server" Border="false" Header="false" Width="200px"
                                        ColumnWidth=".5" Layout="Form" LabelAlign="Top">
                                        <Items>
                                            <ext:ComboBox ID="cmbTipoColectiva" TabIndex="7" FieldLabel="Tipos de Colectiva"
                                                MaxLength="100" runat="server" Text="" MsgTarget="Side" AnchorHorizontal="90%"
                                                StoreID="SCTipoColectiva" DisplayField="Descripcion" ValueField="ClaveTipoColectiva">
                                                <DirectEvents>
                                                    <Select OnEvent="TipoColectiva_Select">
                                                    </Select>
                                                </DirectEvents>
                                            </ext:ComboBox>
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel ID="Panel10" runat="server" Border="false" Header="false" Width="200px"
                                        ColumnWidth=".5" Layout="Form" LabelAlign="Top">
                                        <Items>
                                            <ext:ComboBox ID="cmbCadenaComercial2" TabIndex="8" FieldLabel="Cadena Comercial"
                                                MaxLength="100" runat="server" Text="" MsgTarget="Side" AllowBlank="false" AnchorHorizontal="90%"
                                                StoreID="SCColectiva" DisplayField="NombreORazonSocial" ValueField="ID_Colectiva" />
                                        </Items>
                                    </ext:Panel>
                                </Items>
                            </ext:Panel>
                        </Items>
                        <FooterBar>
                            <ext:Toolbar ID="Toolbar1" runat="server" EnableOverflow="true">
                                <Items>
                                    <ext:Button ID="btnGuardar" runat="server" Text="Guardar" Icon="Add">
                                        <DirectEvents>
                                            <Click OnEvent="btnGuardar_Click" Before="var valid= #{FormPanelEditar}.getForm().isValid(); if (!valid) {} return valid;" />
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

