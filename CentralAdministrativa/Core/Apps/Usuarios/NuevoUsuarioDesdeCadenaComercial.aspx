<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="NuevoUsuarioDesdeCadenaComercial.aspx.cs" Inherits="Usuarios.NuevoUsuarioDesdeCadenaComercial" %>
<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true">
            <ext:FormPanel ID="FormPanel1" Width="428.5" Title="Agregar Usuario"  runat="server"
                Border="false" Layout="FitLayout" Padding="10">
                <Content>
                    <ext:Store ID="SCColectiva" runat="server" >
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                                <Fields>
                                    <ext:RecordField Name="ID_Colectiva" />
                                    <ext:RecordField Name="NombreORazonSocial" />
                                    <ext:RecordField Name="ColectivaNombre" />
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
                        LabelAlign="Left" Layout="FormLayout">
                        <Items>
                            <ext:Panel ID="Panel1" runat="server" Title="Datos Usuario" AutoHeight="true" LabelAlign="Top"
                                FormGroup="true" Layout="TableLayout" Width="500px">
                                <Items>
                                    <ext:Panel ID="Panel4" runat="server" Border="false" Header="false" Width="200px"
                                        ColumnWidth=".5" Layout="Form" LabelAlign="Top">
                                        <Items>
                                            <ext:TextField TabIndex="1" ID="txtUsuario" FieldLabel="UserID" runat="server" MaxLength="20"
                                                MsgTarget="Side" AllowBlank="false" AnchorHorizontal="90%" />
                                            <ext:TextField TabIndex="3" ID="txtPass2" FieldLabel="RE-Password" MaxLength="20" InputType="Password"
                                                runat="server" MsgTarget="Side" AllowBlank="false" AnchorHorizontal="90%" />
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel ID="Panel8" runat="server" Border="false" Header="false" ColumnWidth=".5"
                                        Width="200px" Layout="Form" LabelAlign="Top">
                                        <Items>
                                            <ext:TextField ID="txtPass1" TabIndex="2" FieldLabel="Password" MaxLength="20" InputType="Password"
                                                runat="server" MsgTarget="Side" AllowBlank="false" AnchorHorizontal="90%">
                                            </ext:TextField>
                                        </Items>
                                    </ext:Panel>
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel2" runat="server" Title="Datos Personales" AutoHeight="true"
                                LabelAlign="Top" FormGroup="true" Layout="TableLayout" Width="500px">
                                <Items>
                                    <ext:Panel ID="Panel6" runat="server" Border="false" Header="false" Width="200px"
                                        ColumnWidth=".5" Layout="Form" LabelAlign="Top">
                                        <Items>
                                            <ext:TextField ID="txtNombre" TabIndex="3" FieldLabel="Nombre" MaxLength="100" runat="server"
                                                Text="" MsgTarget="Side" AllowBlank="false" AnchorHorizontal="90%" />
                                            <ext:TextField ID="txtAmaterno" TabIndex="5" FieldLabel="Apellido Materno" MaxLength="50" runat="server"
                                                Text="" MsgTarget="Side" AllowBlank="false" AnchorHorizontal="90%" />
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel ID="Panel9" runat="server" Border="false" Header="false" ColumnWidth=".5"
                                        Width="200px" Layout="Form" LabelAlign="Top">
                                        <Items>
                                            <ext:TextField ID="txtApaterno" TabIndex="4" FieldLabel="Apellido Paterno" runat="server" Text=""
                                                MsgTarget="Side" AllowBlank="false" AnchorHorizontal="90%" />
                                            <ext:TextField ID="txtEmail" TabIndex="6"  Vtype="email" FieldLabel="Email" MaxLength="50" runat="server" Text=""
                                                MsgTarget="Side" AllowBlank="false" AnchorHorizontal="90%" />
                                        </Items>
                                    </ext:Panel>
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel3" runat="server" Title="Asignar Usuario a Colectiva" AutoHeight="true"
                                LabelAlign="Top" FormGroup="true" Layout="TableLayout" Width="500px">
                                <Items>
                                    <ext:Panel ID="Panel7" runat="server" Border="false" Header="false" Width="200px"
                                        ColumnWidth=".5" Layout="Form" LabelAlign="Top">
                                        <Items>
                                            <ext:ComboBox ID="cmbTipoColectiva" EmptyText="Selecciona una Opcion" TabIndex="7" FieldLabel="Tipos de Colectiva" MaxLength="100"
                                                runat="server" Text="" MsgTarget="Side" AllowBlank="false" AnchorHorizontal="90%"
                                                StoreID="SCTipoColectiva" DisplayField="Descripcion" ValueField="ClaveTipoColectiva">
                                                <DirectEvents>
                                                    <Select OnEvent="TipoColectiva_Select" />
                                                </DirectEvents>
                                            </ext:ComboBox>
                                       </Items>
                                    </ext:Panel>
                                    <ext:Panel ID="Panel10" runat="server" Border="false" Header="false" Width="200px"
                                        ColumnWidth=".5" Layout="Form" LabelAlign="Top">
                                        <Items>
                                            <ext:ComboBox ID="cmbColectiva" Resizable="true" TabIndex="8" EmptyText="Selecciona una Opcion"  FieldLabel="Colectiva Asignada al Usuario" MaxLength="100"
                                                runat="server" Text="" MsgTarget="Side" AllowBlank="false" AnchorHorizontal="90%"
                                                StoreID="SCColectiva" DisplayField="ColectivaNombre" ValueField="ID_Colectiva" />
                                        </Items>
                                    </ext:Panel>
                                </Items>
                            </ext:Panel>
                        </Items>
                        <FooterBar>
                            <ext:Toolbar ID="Toolbar1" runat="server" EnableOverflow="true">
                                <Items>
                                    <ext:Button ID="btnGuardar" TabIndex="9" runat="server" Text="Guardar" Icon="Add">
                                        <DirectEvents>
                                            <Click OnEvent="btnGuardar_Click" Before="var valid= #{FormPanel1}.getForm().isValid(); if (!valid) {} return valid;" />
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
                                Header="false" Border="false">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel1" runat="server">
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                                    </ext:RowSelectionModel>
                                </SelectionModel>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="SAllUser" DisplayInfo="true"
                                        DisplayMsg="Mostrando Colectivas {0} - {1} de {2}" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
