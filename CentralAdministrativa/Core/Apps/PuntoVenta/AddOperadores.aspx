<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="AddOperadores.aspx.cs" Inherits="TpvWeb.AddOperadores" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true">
            <ext:FormPanel ID="FormPanel1" Width="350" Title="Nuevo Operador" runat="server"
                Border="false" Layout="Fit">
                <Items>
                    <ext:Panel ID="Panel1" runat="server" AutoHeight="true" FormGroup="true"
                        Padding="10">
                        <Content>
                            <ext:Store ID="SCPadre" runat="server">
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_Colectiva">
                                        <Fields>
                                            <ext:RecordField Name="ID_Colectiva" />
                                            <ext:RecordField Name="NombreORazonSocial" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                                <SortInfo Field="NombreORazonSocial" />
                            </ext:Store>
                            <ext:Store ID="storeCadenaComercial" runat="server">
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_Colectiva">
                                        <Fields>
                                            <ext:RecordField Name="ID_Colectiva" />
                                            <ext:RecordField Name="NombreORazonSocial" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                                <SortInfo Field="NombreORazonSocial" Direction="ASC" />
                            </ext:Store>
                            <ext:Store ID="Store2" runat="server" OnRefreshData="RefreshGrid" GroupField="ColectivaPadre">
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_Colectiva">
                                    </ext:JsonReader>
                                </Reader>
                                <SortInfo Field="NombreORazonSocial" />
                            </ext:Store>
                            <ext:Store ID="STipoColectiva" runat="server">
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_TipoColectiva">
                                        <Fields>
                                            <ext:RecordField Name="ID_TipoColectiva" />
                                            <ext:RecordField Name="Descripcion" />
                                            <ext:RecordField Name="ClaveTipoColectiva" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                                <SortInfo Field="Descripcion" />
                            </ext:Store>

                        </Content>
                        <Items>
                            <ext:ComboBox ID="cmbTipoUsuario" FieldLabel="Tipo Entidad" AllowBlank="false" MsgTarget="Side"
                                runat="server" EmptyText="Selecciona una Opción" Width="320" StoreID="STipoColectiva"
                                Disabled="true" DisplayField="Descripcion" ValueField="ClaveTipoColectiva">
                                <DirectEvents>
                                    <Select OnEvent="TipoColectiva_Select">
                                    </Select>
                                </DirectEvents>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cmbPadreTipoColectiva" FieldLabel="Terminal" Visible="false" AllowBlank="false"
                                MsgTarget="Side" runat="server" EmptyText="Selecciona una Terminal" Width="320"
                                StoreID="SCPadre" DisplayField="NombreORazonSocial" ValueField="ID_Colectiva">
                                <%--  <DirectEvents>
                                            <Select OnEvent="SeleccionaPadre_Select">
                                            </Select>
                                        </DirectEvents>--%>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cmbCadenaComercial" FieldLabel="Cadena Comercial" Visible="true" AllowBlank="false"
                                MsgTarget="Side" runat="server" EmptyText="Selecciona una Cadena Comercial" Width="320"
                                StoreID="storeCadenaComercial" DisplayField="NombreORazonSocial" ValueField="ID_Colectiva"
                                Mode="Local" AutoSelect="true" Editable="true" ForceSelection="true" TypeAhead="true" MinChars="1"
                                MatchFieldWidth="false" Name="colCadena" />
                            <ext:CompositeField ID="UserId" runat="server" FieldLabel="Clave">
                                <Items>
                                    <ext:TextField ID="txtUserName" FieldLabel="UserID" AllowBlank="false" MsgTarget="Side"
                                        MaxLength="10" runat="server" Text="" Width="215" />
                                    <ext:Label ID="lblArroba" runat="server" Text="@" Visible="false">
                                    </ext:Label>
                                    <ext:TextField ID="txtCadena" FieldLabel="" Disabled="true" Visible="false" AllowBlank="false" MsgTarget="Side"
                                        MaxLength="20" runat="server" Text="" Width="115" />
                                </Items>
                            </ext:CompositeField>
                            <ext:TextField ID="txtPass1" FieldLabel="Contraseña" AllowBlank="false" MsgTarget="Side"
                                MaxLength="20" InputType="Password" runat="server" Width="320" Text="" />
                            <ext:TextField ID="txtPass2" FieldLabel="Re-Contraseña" AllowBlank="false" MsgTarget="Side"
                                MaxLength="20" InputType="Password" runat="server" Text="" Width="320" />
                            <ext:TextField ID="txtNombre" FieldLabel="Nombre" AllowBlank="false" MsgTarget="Side"
                                MaxLength="50" runat="server" Width="320" Text="" />
                            <ext:TextField ID="txtApaterno" FieldLabel="Apellido Paterno" AllowBlank="false"
                                MsgTarget="Side" MaxLength="50" runat="server" Text="" Width="320" />
                            <ext:TextField ID="txtAmaterno" FieldLabel="Apellido Materno" AllowBlank="false"
                                MsgTarget="Side" MaxLength="50" runat="server" Width="320" Text="" />
                            <ext:DateField FieldLabel="Fecha" AllowBlank="false" MsgTarget="Side" Format="yyyy-MM-dd"
                                Name="Fecha Inicio" ID="datFecha" Width="320" runat="server" EmptyText="Selecciona una Opción"
                                MaxLength="12" Vtype="daterange" />
                            <ext:ComboBox ID="cmbSexo" FieldLabel="Género" EmptyText="Selecciona una Opción" AllowBlank="false"
                                MsgTarget="Side" runat="server" Width="320">
                                <Items>
                                    <ext:ListItem Text="Hombre" Value="0" />
                                    <ext:ListItem Text="Mujer" Value="1" />
                                </Items>
                            </ext:ComboBox>
                            <%--  <ext:TextField ID="txtRFC" FieldLabel="RFC" AllowBlank="false" MsgTarget="Side" MaxLength="20"
                                        runat="server" Text="" Width="400" />
                                    <ext:TextField ID="txtCURP" FieldLabel="CURP" AllowBlank="false" MsgTarget="Side"
                                        MaxLength="50" runat="server" Width="400" Text="" />--%>
                            <ext:TextField ID="txtMovil" FieldLabel="Telefono Movil" AllowBlank="false" MsgTarget="Side"
                                MaxLength="10" runat="server" Text="" Width="320" Visible="false" />
                            <ext:TextField ID="txtFijo" FieldLabel="Telefono Fijo" AllowBlank="false" MsgTarget="Side"
                                MaxLength="10" runat="server" Width="320" Text="" Visible="false" />
                            <ext:TextField ID="txtemail2" Vtype="email" FieldLabel="Email" AllowBlank="false"
                                MsgTarget="Side" MaxLength="50" runat="server" Text="" Width="320" Visible="false" />
                            <%-- <ext:Checkbox runat="server" ID="esClubEscala" FieldLabel="Agregar a Club Escala">
                                    </ext:Checkbox>--%>
                            <%-- <ext:ComboBox ID="cmbAfiliacion" FieldLabel="Afiliación Transacccional" EmptyText="Selecciona una Opción"
                                        AllowBlank="false" MsgTarget="Side" runat="server" StoreID="SAfiliaciones" DisplayField="NombreAfiliacion"
                                        ValueField="Afiliacion" Width="400">
                                    </ext:ComboBox>--%>
                        </Items>
                    </ext:Panel>
                </Items>
                <FooterBar>
                    <ext:Toolbar ID="Toolbar1" runat="server" EnableOverflow="true">
                        <Items>
                            <ext:Button ID="btnGuardar" runat="server" Text="Guardar" Icon="Add">
                                <DirectEvents>
                                    <Click OnEvent="btnGuardar_Click" Before="var valid= #{FormPanel1}.getForm().isValid(); if (!valid) {} return valid;" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>
            </ext:FormPanel>
        </West>
        <Center Split="true">
            <ext:Panel ID="Panel5" runat="server" Title="Los Operadores" Collapsed="false" Layout="Fit"
                AutoScroll="true" Collapsible="true">
                <Content>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store2" StripeRows="true"
                                Header="false" Border="false" AutoExpandColumn="NombreORazonSocial">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel1" runat="server">
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                                    </ext:RowSelectionModel>
                                </SelectionModel>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="Store2" DisplayInfo="true"
                                        DisplayMsg="Mostrando Operadores {0} - {1} de {2}" />
                                </BottomBar>
                                <View>
                                    <ext:GroupingView ID="GroupingView1" runat="server" ForceFit="true" MarkDirty="false"
                                        ShowGroupName="false" EnableNoGroups="true" HideGroupedColumn="true" />
                                </View>
                               
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
