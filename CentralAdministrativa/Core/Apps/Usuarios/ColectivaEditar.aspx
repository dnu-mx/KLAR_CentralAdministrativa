<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ColectivaEditar.aspx.cs" Inherits="Usuarios.ColectivaEditar" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center Split="true">
            <ext:Panel ID="Panel2" runat="server" Title="Entidades" Collapsed="false" Layout="Fit"
                AutoScroll="true" Width="428.5">
                <Content>
                    <ext:Store ID="Store2" runat="server" OnRefreshData="RefreshGrid" GroupField="TipoColectiva">
                        <Reader>
                            <ext:JsonReader IDProperty="ID">
                                <Fields>
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
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
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="Store2" BufferResize="100"
                                        DisplayInfo="true" DisplayMsg="Mostrando Entidades {0} - {1} de {2}" />
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
        <East Split="true">
            <ext:FormPanel ID="FormPanel1" Width="428.5" Title="Mi Grupo Comercial" runat="server"
                Collapsed="true" Collapsible="true" Border="false" Layout="Fit">
                <Items>
                    <ext:TabPanel runat="server" ActiveTabIndex="0" TabPosition="Bottom" Border="false"
                        AutoScroll="true" ID="TabPanel1" Title="ctl71">
                        <Items>
                            <ext:Panel ID="Panel1" runat="server" Title="Datos de Sesion" AutoHeight="true" FormGroup="true">
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
                                    <ext:Store ID="SEstado" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="CveEstado">
                                                <Fields>
                                                    <ext:RecordField Name="CveEstado" />
                                                    <ext:RecordField Name="DesEstado" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                    <ext:Store ID="SMunicipio" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="CveMunicipio">
                                                <Fields>
                                                    <ext:RecordField Name="CveMunicipio" />
                                                    <ext:RecordField Name="DesMunicipio" />
                                                    <ext:RecordField Name="CveEstado" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                    <ext:Store ID="SAsentamiento" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="CveAsentamiento">
                                                <Fields>
                                                    <ext:RecordField Name="CveAsentamiento" />
                                                    <ext:RecordField Name="DesAsentamiento" />
                                                    <ext:RecordField Name="ID_Asentamiento" />
                                                    <ext:RecordField Name="CveEstado" />
                                                    <ext:RecordField Name="CveMunicipio" />
                                                    <ext:RecordField Name="CveCiudad" />
                                                    <ext:RecordField Name="CodigoPostal" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                    <ext:Store ID="SFEdo" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="CveEstado">
                                                <Fields>
                                                    <ext:RecordField Name="CveEstado" />
                                                    <ext:RecordField Name="DesEstado" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                    <ext:Store ID="SFMpio" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="CveMunicipio">
                                                <Fields>
                                                    <ext:RecordField Name="CveMunicipio" />
                                                    <ext:RecordField Name="DesMunicipio" />
                                                    <ext:RecordField Name="CveEstado" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                    <ext:Store ID="SFAsen" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="CveAsentamiento">
                                                <Fields>
                                                    <ext:RecordField Name="CveAsentamiento" />
                                                    <ext:RecordField Name="ID_Asentamiento" />
                                                    <ext:RecordField Name="DesAsentamiento" />
                                                    <ext:RecordField Name="CveEstado" />
                                                    <ext:RecordField Name="CveMunicipio" />
                                                    <ext:RecordField Name="CveCiudad" />
                                                    <ext:RecordField Name="CodigoPostal" />
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
                                        ValueField="ClaveTipoColectiva">
                                        <DirectEvents>
                                            <Select OnEvent="TipoColectiva_Select">
                                            </Select>
                                        </DirectEvents>
                                    </ext:ComboBox>
                                    <ext:ComboBox ID="cmbPadreTipoColectiva" EmptyText="Selecciona una Opción" Enabled="false"
                                        Disabled="true" FieldLabel="Colectiva Padre" AllowBlank="false" MsgTarget="Side"
                                        runat="server" Width="400" StoreID="SCPadre" DisplayField="NombreORazonSocial"
                                        ValueField="ID_Colectiva">
                                    </ext:ComboBox>
                                    <ext:TextField ID="txtNombre" FieldLabel="Nombre/RS" AllowBlank="false" MsgTarget="Side"
                                        MaxLength="50" runat="server" Width="400" Text="" />
                                    <ext:TextField ID="txtApaterno" FieldLabel="Apellido Paterno" AllowBlank="false"
                                        MsgTarget="Side" MaxLength="50" runat="server" Text="" Width="400" />
                                    <ext:TextField ID="txtAmaterno" FieldLabel="Apellido Materno" AllowBlank="false"
                                        MsgTarget="Side" MaxLength="50" runat="server" Width="400" Text="" />
                                    <ext:DateField Visible="false" FieldLabel="Fecha Nacimiento" AllowBlank="false" MsgTarget="Side"
                                        Format="yyyy-MM-dd" Name="Fecha" TabIndex="4" ID="datFecha" Width="400" runat="server"
                                        EmptyText="Selecciona una Opción" MaxLength="12" Vtype="daterange" />
                                    <ext:ComboBox ID="cmbSexo" Visible="false" FieldLabel="Sexo" AllowBlank="false" MsgTarget="Side"
                                        runat="server" Width="400">
                                        <Items>
                                            <ext:ListItem Text="Hombre" Value="0" />
                                            <ext:ListItem Text="Mujer" Value="1" />
                                        </Items>
                                    </ext:ComboBox>
                                    <ext:TextField ID="txtRFC" FieldLabel="RFC" AllowBlank="true" MsgTarget="Side" MaxLength="20"
                                        runat="server" Text="" Width="400" />
                                    <ext:TextField ID="txtCURP" FieldLabel="CURP" AllowBlank="true" MsgTarget="Side"
                                        MaxLength="50" runat="server" Width="400" Text="" />
                                    <ext:TextField ID="txtMovil" Enabled="false" FieldLabel="Telefono Movil" AllowBlank="true"
                                        MsgTarget="Side" MaxLength="10" runat="server" Text="" Width="400" />
                                    <ext:TextField ID="txtFijo" FieldLabel="Telefono Fijo" AllowBlank="true" MsgTarget="Side"
                                        MaxLength="10" runat="server" Width="400" Text="" />
                                    <ext:TextField ID="txtemail2" Enabled="false" FieldLabel="Email" Disabled="true"
                                        AllowBlank="true" MsgTarget="Side" MaxLength="50" runat="server" Text="" Width="400" />
                                    <ext:ComboBox ID="cmbAfiliacion" FieldLabel="Afiliación Transacccional" EmptyText="Selecciona una Opción"
                                        AllowBlank="false" MsgTarget="Side" runat="server" StoreID="SAfiliaciones" DisplayField="NombreAfiliacion"
                                        ValueField="Afiliacion" Enabled="false" Visible="false" Width="400">
                                    </ext:ComboBox>
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel8" runat="server" Title="Direccion Ubicación" AutoHeight="true"
                                FormGroup="true">
                                <Items>
                                    <ext:TextField ID="UtxtCodigoPostal" FieldLabel="Codigo Postal" MsgTarget="Side"
                                        Enabled="false" MaxLength="5" Disabled="true" runat="server" Width="400" Text="" />
                                    <ext:ComboBox ID="UcmbEstado" EmptyText="Selecciona una Opción" FieldLabel="Estado"
                                        MsgTarget="Side" runat="server" Width="400" StoreID="SEstado" DisplayField="DesEstado"
                                        ValueField="CveEstado">
                                        <DirectEvents>
                                            <Select OnEvent="EstadosUbicacion_Select">
                                            </Select>
                                        </DirectEvents>
                                    </ext:ComboBox>
                                    <ext:ComboBox ID="UcmbMunicipios" EmptyText="Selecciona una Opción" FieldLabel="Municipio"
                                        MsgTarget="Side" runat="server" Width="400" StoreID="SMunicipio" DisplayField="DesMunicipio"
                                        ValueField="CveMunicipio">
                                        <DirectEvents>
                                            <Select OnEvent="MunicipioUbicacion_Select">
                                            </Select>
                                        </DirectEvents>
                                    </ext:ComboBox>
                                    <ext:ComboBox ID="UcmbAsentamiento" EmptyText="Selecciona una Opción" FieldLabel="Asentamiento"
                                        MsgTarget="Side" runat="server" Width="400" StoreID="SAsentamiento" DisplayField="DesAsentamiento"
                                        ValueField="ID_Asentamiento">
                                        <DirectEvents>
                                            <Select OnEvent="AsentamientoUbicacion_Select">
                                            </Select>
                                        </DirectEvents>
                                    </ext:ComboBox>
                                    <ext:TextField ID="UtxtCalle" FieldLabel="Calle" MsgTarget="Side" MaxLength="100"
                                        runat="server" Width="400" Text="" />
                                    <ext:TextField ID="Utxtinterior" FieldLabel="Num. Int." MaxLength="5" runat="server"
                                        Width="400" Text="" />
                                    <ext:TextField ID="UtxtExterior" FieldLabel="Num. Ext." MaxLength="5" runat="server"
                                        Width="400" Text="" />
                                    <ext:TextArea ID="UtxtEntreCalles" FieldLabel="Entre Calles" MaxLength="500" runat="server"
                                        Width="400" Height="50" />
                                    <ext:TextArea ID="UtxtReferencias" FieldLabel="Referencias" MaxLength="500" runat="server"
                                        Width="400" Height="50" />
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel9" runat="server" Title="Dirección Facturación" AutoHeight="true"
                                FormGroup="true">
                                <Items>
                                    <ext:TextField ID="FtxtCodigoPostal" FieldLabel="Codigo Postal" MaxLength="5" Enabled="false"
                                        Disabled="true" runat="server" Width="400" Text="" />
                                    <ext:ComboBox ID="fcmbEstado" EmptyText="Selecciona una Opción" runat="server" FieldLabel="Estado"
                                        Width="400" StoreID="SFEdo" DisplayField="DesEstado" ValueField="CveEstado">
                                        <DirectEvents>
                                            <Select OnEvent="EstadosFacturacion_Select">
                                            </Select>
                                        </DirectEvents>
                                    </ext:ComboBox>
                                    <ext:ComboBox ID="FcmbMunicipio" EmptyText="Selecciona una Opción" FieldLabel="Municipio"
                                        runat="server" Width="400" StoreID="SFMpio" DisplayField="DesMunicipio" ValueField="CveMunicipio">
                                        <DirectEvents>
                                            <Select OnEvent="MunicipioFacturacion_Select">
                                            </Select>
                                        </DirectEvents>
                                    </ext:ComboBox>
                                    <ext:ComboBox ID="FcmbAsentamiento" EmptyText="Selecciona una Opción" FieldLabel="Asentamiento"
                                        runat="server" Width="400" StoreID="SFAsen" DisplayField="DesAsentamiento" ValueField="ID_Asentamiento">
                                        <DirectEvents>
                                            <Select OnEvent="AsentamientoFacturacion_Select">
                                            </Select>
                                        </DirectEvents>
                                    </ext:ComboBox>
                                    <ext:TextField ID="FtxtCalle" FieldLabel="Calle" MaxLength="100" runat="server" Width="400"
                                        Text="" />
                                    <ext:TextField ID="FtxtInterior" FieldLabel="Num. Int." MaxLength="5" runat="server"
                                        Width="400" Text="" />
                                    <ext:TextField ID="FtxtExterior" FieldLabel="Num. Ext." MaxLength="5" runat="server"
                                        Width="400" Text="" />
                                    <ext:TextArea ID="FtxtEntreCalle" FieldLabel="Entre Calles" MaxLength="500" runat="server"
                                        Width="400" Height="50" />
                                    <ext:TextArea ID="FtxtReferencia" FieldLabel="Referencias" MaxLength="500" runat="server"
                                        Width="400" Height="50" />
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel4" runat="server" Title="Valores Contrato" AutoHeight="true"
                                FormGroup="true">
                                <Items>
                                    <ext:PropertyGrid ID="GridContrato" runat="server" Header="false">
                                        <Source>
                                            <ext:PropertyGridParameter Name="(Los Parametros)" Value="Los Valores">
                                            </ext:PropertyGridParameter>
                                        </Source>
                                        <DirectEvents>
                                        </DirectEvents>
                                        <View>
                                            <ext:GridView ID="GridView2" ForceFit="true" ScrollOffset="2" runat="server" />
                                        </View>
                                    </ext:PropertyGrid>
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel3" runat="server" Title="Valores Extras" AutoHeight="true" FormGroup="true">
                                <Items>
                                    <ext:PropertyGrid ID="GridExtras" runat="server" Header="false">
                                        <Source>
                                            <ext:PropertyGridParameter Name="(Los Parametros)" Value="Los Valores">
                                            </ext:PropertyGridParameter>
                                        </Source>
                                        <DirectEvents>
                                        </DirectEvents>
                                        <View>
                                            <ext:GridView ID="GridView1" ForceFit="true" ScrollOffset="2" runat="server" />
                                        </View>
                                    </ext:PropertyGrid>
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
