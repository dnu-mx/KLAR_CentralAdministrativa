<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EditTerminales.aspx.cs" Inherits="OperadoraWeb.EditTerminales" %>
<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
<script type="text/javascript">

    var prepareToolbar = function (grid, toolbar, rowIndex, record) {
        // for example hide 'Edit' button if price < 50
        if (record.get("ID_EstatusColectiva") == 1) { //Activo

            //toolbar.items.get(0).hide(); //Delete
            toolbar.items.get(0).hide(); //asgina
            toolbar.items.get(1).hide(); //asgina
            toolbar.items.get(2).hide(); //asgina
            toolbar.items.get(3).hide(); //asgina
            toolbar.items.get(5).hide(); //sep
        } else { //Otro no Activo
            toolbar.items.get(0).hide(); //asgina
            toolbar.items.get(1).hide(); //asgina
            toolbar.items.get(2).hide(); //asgina
            toolbar.items.get(3).hide(); //asgina
            toolbar.items.get(4).hide(); //asgina
        }


    };

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
     <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center Split="true">
            <ext:Panel ID="Panel2" runat="server" Title="Terminales" Collapsed="false"
                Layout="Fit" AutoScroll="true" Width="428.5">
                <Content>
                    <ext:Store ID="Store2" runat="server" OnRefreshData="RefreshGrid" GroupField="ColectivaPadre">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="NombreORazonSocial" />
                    </ext:Store>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store2" StripeRows="true"
                                Header="false" Border="false" AutoExpandColumn="NombreORazonSocial">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel1" runat="server">
                                </ColumnModel>
                                <DirectEvents>
                                    <RowDblClick OnEvent="GridEmpleados_DblClik">
                                    </RowDblClick>
                                </DirectEvents>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                                        <DirectEvents>
                                            <RowDeselect OnEvent="QuitarSeleccion">
                                            </RowDeselect>
                                        </DirectEvents>
                                    </ext:RowSelectionModel>
                                </SelectionModel>
                                 <DirectEvents>
                                    <Command OnEvent="EjecutarComando">
                                        <ExtraParams>
                                            <ext:Parameter Name="id" Value="record.data.ID" Mode="Raw" />
                                        </ExtraParams>
                                    </Command>
                                </DirectEvents>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="Store2" BufferResize="100"
                                        DisplayInfo="true" DisplayMsg="Mostrando Terminales {0} - {1} de {2}" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </Center>
        <East Split="true">
            <ext:FormPanel ID="FormPanel1" Width="428.5" Title="Mi Grupo Comercial" runat="server"
                Collapsed="true" Collapsible="true" Border="false" Layout="Fit">
                <Items>
                    <ext:TabPanel runat="server" ActiveTabIndex="0" TabPosition="Bottom" Border="false"
                        AutoScroll="true" ID="TabPanel1" Title="ctl71">
                        <Items>
                            <ext:Panel ID="Panel1" runat="server" Title="Terminal" AutoHeight="true" FormGroup="true">
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
                                    </ext:Store>
                                    <ext:Store ID="Store1" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Colectiva">
                                            </ext:JsonReader>
                                        </Reader>
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
                                    </ext:Store>
                                    <ext:Store ID="SAfiliaciones" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_TipoColectiva">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Colectiva" />
                                                    <ext:RecordField Name="Afiliacion" />
                                                    <ext:RecordField Name="NombreAfiliacion" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Content>
                                <Items>
                                    <ext:TextField ID="txtUserName" Enabled="false" Disabled="true" FieldLabel="Clave Usuario"
                                        AllowBlank="false" MsgTarget="Side" MaxLength="50" runat="server" Text="" Width="400" />
                                    <ext:TextField ID="txtPass1" FieldLabel="Password" Visible="false" AllowBlank="false"
                                        MsgTarget="Side" MaxLength="20" InputType="Password" runat="server" Width="400"
                                        Text="" />
                                    <ext:TextField ID="txtPass2" FieldLabel="Repite Password" Visible="false" AllowBlank="false"
                                        MsgTarget="Side" MaxLength="20" InputType="Password" runat="server" Text="" Width="400" />
                                    <ext:ComboBox ID="cmbTipoUsuario" EmptyText="Selecciona una Opción" Enabled="false"
                                        Disabled="true" FieldLabel="Tipo Colectiva" AllowBlank="false" MsgTarget="Side"
                                        runat="server" Width="400" StoreID="STipoColectiva" DisplayField="Descripcion"
                                        ValueField="ClaveTipoColectiva" Visible="false">
                                        <DirectEvents>
                                            <Select OnEvent="TipoColectiva_Select">
                                            </Select>
                                        </DirectEvents>
                                    </ext:ComboBox>
                                    <ext:ComboBox ID="cmbPadreTipoColectiva" EmptyText="Selecciona una Opción"  FieldLabel="Colectiva Padre" AllowBlank="false" MsgTarget="Side"
                                        runat="server" Width="400" StoreID="SCPadre" DisplayField="NombreORazonSocial"
                                        ValueField="ID_Colectiva" Visible="true">
                                    </ext:ComboBox>
                                    <ext:TextField ID="txtNombre" FieldLabel="Descripción" AllowBlank="false" MsgTarget="Side"
                                        MaxLength="50" runat="server" Width="400" Text="" />
                                    <ext:TextField ID="txtApaterno" FieldLabel="Apellido Paterno" AllowBlank="false"
                                        MsgTarget="Side" MaxLength="50" runat="server" Text="" Width="400" />
                                    <ext:TextField ID="txtAmaterno" FieldLabel="Ubicación 1" AllowBlank="false"
                                        MsgTarget="Side" MaxLength="50" runat="server" Width="400" Text="" />
                                    <ext:DateField Visible="false" FieldLabel="Ubicación 2" AllowBlank="false" MsgTarget="Side"
                                        Format="yyyy-MM-dd" Name="Fecha" TabIndex="4" ID="datFecha" Width="400" runat="server"
                                        EmptyText="Selecciona una Opción" MaxLength="12" Vtype="daterange" />
                                    <ext:ComboBox ID="cmbSexo" Visible="false" FieldLabel="Sexo" AllowBlank="false" MsgTarget="Side"
                                        runat="server" Width="400">
                                        <Items>
                                            <ext:ListItem Text="Hombre" Value="0" />
                                            <ext:ListItem Text="Mujer" Value="1" />
                                        </Items>
                                    </ext:ComboBox>
                                    <ext:TextField ID="txtRFC" FieldLabel="RFC" AllowBlank="false" MsgTarget="Side" MaxLength="20"
                                        runat="server" Text="" Width="400" Visible="false" />
                                    <ext:TextField ID="txtCURP" FieldLabel="CURP" AllowBlank="false" MsgTarget="Side"
                                        MaxLength="50" runat="server" Width="400" Text="" Visible="false" />
                                    <ext:TextField ID="txtMovil" Enabled="false" FieldLabel="Telefono Movil" AllowBlank="false"
                                        MsgTarget="Side" MaxLength="10" runat="server" Text="" Width="400" Visible="false" />
                                    <ext:TextField ID="txtFijo" FieldLabel="Telefono Fijo" AllowBlank="false" MsgTarget="Side"
                                        MaxLength="10" runat="server" Width="400" Text="" Visible="false" />
                                    <ext:TextField ID="txtemail2" Enabled="false" FieldLabel="Email" Disabled="true"
                                        AllowBlank="false" MsgTarget="Side" MaxLength="50" runat="server" Text="" Width="400"
                                        Visible="false" />
                                </Items>
                            </ext:Panel>
                        </Items>
                        <FooterBar>
                            <ext:Toolbar ID="Toolbar1" runat="server" EnableOverflow="true">
                                <Items>
                                    <ext:Button ID="btnGuardar" runat="server" Text="Guardar" Icon="Accept">
                                        <DirectEvents>
                                            <Click OnEvent="btnGuardar_Click" Before="var valid= #{FormPanel1}.getForm().isValid(); if (!valid) {} return valid;" />
                                        </DirectEvents>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </FooterBar>
                    </ext:TabPanel>
                </Items>
            </ext:FormPanel>
        </East>
    </ext:BorderLayout>
</asp:Content>
