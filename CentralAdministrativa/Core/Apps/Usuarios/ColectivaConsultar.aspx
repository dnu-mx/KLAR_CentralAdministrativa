<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ColectivaConsultar.aspx.cs" Inherits="Usuarios.ColectivaConsultar" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center Split="true">
            <ext:Panel ID="Panel2" runat="server" Title="Entidades" Collapsed="false"
                Layout="Fit" AutoScroll="true" Width="428.5">
                <Content>
                    <ext:Store ID="Store2" runat="server"  OnRefreshData="RefreshGrid"   GroupField="TipoColectiva" >
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
                                        DisplayInfo="true" DisplayMsg="Mostrando Colectivas {0} - {1} de {2}" />
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
         
            <ext:FormPanel ID="FormPanel1" Width="428.5" Title="Mi Grupo Comercial" Collapsed="true" Collapsible="true" runat="server"
                Border="false" Layout="Fit">
                <Items>
                    <ext:TabPanel runat="server" ActiveTabIndex="0" TabPosition="Bottom" Border="false"
                        AutoScroll="true" ID="TabPanel1" Title="ctl71">
                        <Items>
                            <ext:Panel ID="Panel1" runat="server" Title="Datos de Sesion" AutoHeight="true"  FormGroup="true">
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
                                </Content>
                                <Items>
                                    <ext:TextField ID="txtemail" Disabled ="true" FieldLabel="Email" AllowBlank="false" MsgTarget="Side"
                                        MaxLength="50" runat="server" Text="" Width="400" />
                                    <ext:TextField ID="txtPass1" Disabled ="true" Visible="false" FieldLabel="Password" AllowBlank="false" MsgTarget="Side"
                                        MaxLength="20" InputType="Password" runat="server" Width="400" Text="" />
                                    <ext:TextField ID="txtPass2" Disabled ="true"  Visible="false"  FieldLabel="Repite Password" AllowBlank="false" MsgTarget="Side"
                                        MaxLength="20" InputType="Password" runat="server" Text="" Width="400" />
                                    <ext:ComboBox ID="cmbTipoUsuario" Disabled ="true" FieldLabel="Tipo Colectiva" AllowBlank="false"
                                        MsgTarget="Side" runat="server" Width="400" StoreID="STipoColectiva" DisplayField="Descripcion"
                                        ValueField="ClaveTipoColectiva">
                                        <DirectEvents>
                                            <Select OnEvent="TipoColectiva_Select">
                                            </Select>
                                        </DirectEvents>
                                    </ext:ComboBox>
                                    <ext:ComboBox ID="cmbPadreTipoColectiva" Disabled ="true" FieldLabel="Colectiva Padre" AllowBlank="false"
                                        MsgTarget="Side" runat="server" Width="400" StoreID="SCPadre" DisplayField="NombreORazonSocial"
                                        ValueField="ID_Colectiva">
                                    </ext:ComboBox>
                                    <ext:TextField ID="txtNombre"  Disabled ="true" FieldLabel="Nombre O Razón Social" AllowBlank="false"
                                        MsgTarget="Side" MaxLength="50" runat="server" Width="400" Text="" />
                                    <ext:TextField ID="txtApaterno" Disabled ="true"  FieldLabel="Apellido Paterno" AllowBlank="false"
                                        MsgTarget="Side" MaxLength="50" runat="server" Text="" Width="400" />
                                    <ext:TextField ID="txtAmaterno" Disabled ="true"  FieldLabel="Apellido Materno" AllowBlank="false"
                                        MsgTarget="Side" MaxLength="50" runat="server" Width="400" Text="" />
                                    <ext:DateField  Visible="false" Disabled ="true"  FieldLabel="Fecha Nacimiento" AllowBlank="false" MsgTarget="Side"
                                        Format="yyyy-MM-dd" Name="Fecha" TabIndex="4" ID="datFecha" Width="400" runat="server"
                                        EmptyText="Selecciona una Opción" MaxLength="12" Vtype="daterange" />
                                    <ext:ComboBox  Visible="false" Disabled ="true"   ID="cmbSexo" FieldLabel="Sexo" AllowBlank="false" MsgTarget="Side"
                                        runat="server" Width="400">
                                        <Items>
                                            <ext:ListItem Text="Hombre" Value="0" />
                                            <ext:ListItem Text="Mujer" Value="1" />
                                        </Items>
                                    </ext:ComboBox>
                                    <ext:TextField ID="txtRFC" Disabled ="true"  FieldLabel="RFC" AllowBlank="false" MsgTarget="Side" MaxLength="20"
                                        runat="server" Text="" Width="400" />
                                    <ext:TextField ID="txtCURP" Disabled ="true"  FieldLabel="CURP" AllowBlank="false" MsgTarget="Side"
                                        MaxLength="50" runat="server" Width="400" Text="" />
                                    <ext:TextField ID="txtMovil"   FieldLabel="Telefono Movil" AllowBlank="false" MsgTarget="Side"
                                        MaxLength="10" runat="server" Disabled ="true"  Text="" Width="400" />
                                    <ext:TextField ID="txtFijo"  Disabled ="true" FieldLabel="Telefono Fijo" AllowBlank="false" MsgTarget="Side"
                                        MaxLength="10" runat="server" Width="400" Text="" />
                                    <ext:TextField ID="txtemail2" Disabled ="true"  FieldLabel="Email" AllowBlank="false" MsgTarget="Side"
                                        MaxLength="50" runat="server" Text="" Width="400" />
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel8" runat="server" Title="Direccion Ubicación" AutoHeight="true"
                                FormGroup="true">
                                <Items>
                                    <ext:TextField ID="UtxtCodigoPostal"  Disabled ="true" FieldLabel="Codigo Postal" MsgTarget="Side"
                                        Enabled="false" MaxLength="5"  runat="server" Width="400" Text="" />
                                    <ext:ComboBox ID="UcmbEstado"  Disabled ="true" FieldLabel="Estado" MsgTarget="Side" runat="server"
                                        Width="400" StoreID="SEstado" DisplayField="DesEstado" ValueField="CveEstado">
                                        <DirectEvents>
                                            <Select OnEvent="EstadosUbicacion_Select">
                                            </Select>
                                        </DirectEvents>
                                    </ext:ComboBox>
                                    <ext:ComboBox ID="UcmbMunicipios"  Disabled ="true" FieldLabel="Municipio o Delegacion" MsgTarget="Side"
                                        runat="server" Width="400" StoreID="SMunicipio" DisplayField="DesMunicipio" ValueField="CveMunicipio">
                                        <DirectEvents>
                                            <Select OnEvent="MunicipioUbicacion_Select">
                                            </Select>
                                        </DirectEvents>
                                    </ext:ComboBox>
                                    <ext:ComboBox ID="UcmbAsentamiento"  Disabled ="true" FieldLabel="Asentamiento" MsgTarget="Side" runat="server"
                                        Width="400" StoreID="SAsentamiento" DisplayField="DesAsentamiento" ValueField="ID_Asentamiento">
                                        <DirectEvents>
                                            <Select OnEvent="AsentamientoUbicacion_Select">
                                            </Select>
                                        </DirectEvents>
                                    </ext:ComboBox>
                                    <ext:TextField ID="UtxtCalle" FieldLabel="Calle" Disabled ="true"  MsgTarget="Side" MaxLength="100"
                                        runat="server" Width="400" Text="" />
                                    <ext:TextField ID="Utxtinterior" FieldLabel="Num. Int." Disabled ="true"  MaxLength="5" runat="server"
                                        Width="400" Text="" />
                                    <ext:TextField ID="UtxtExterior" FieldLabel="Num. Ext." Disabled ="true"  MaxLength="5" runat="server"
                                        Width="400" Text="" />
                                    <ext:TextArea ID="UtxtEntreCalles" FieldLabel="Entre Calles"  Disabled ="true" MaxLength="500" runat="server"
                                        Width="400" Height="50" />
                                    <ext:TextArea ID="UtxtReferencias" FieldLabel="Referencias" Disabled ="true"  MaxLength="500" runat="server"
                                        Width="400" Height="50" />
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel9" runat="server" Title="Dirección Facturación" AutoHeight="true"
                                FormGroup="true">
                                <Items>
                                    <ext:TextField ID="FtxtCodigoPostal" FieldLabel="Codigo Postal" Disabled ="true"  MaxLength="5" Enabled="false"
                                         runat="server" Width="400" Text="" />
                                    <ext:ComboBox ID="fcmbEstado" runat="server" FieldLabel="Estado"  Disabled ="true" Width="400" StoreID="SFEdo"
                                        DisplayField="DesEstado" ValueField="CveEstado">
                                        <DirectEvents>
                                            <Select OnEvent="EstadosFacturacion_Select">
                                            </Select>
                                        </DirectEvents>
                                    </ext:ComboBox>
                                    <ext:ComboBox ID="FcmbMunicipio" FieldLabel="Municipio" Disabled ="true"  runat="server"
                                        Width="400" StoreID="SFMpio" DisplayField="DesMunicipio" ValueField="CveMunicipio">
                                        <DirectEvents>
                                            <Select OnEvent="MunicipioFacturacion_Select">
                                            </Select>
                                        </DirectEvents>
                                    </ext:ComboBox>
                                    <ext:ComboBox ID="FcmbAsentamiento" FieldLabel="Asentamiento" Disabled ="true"  runat="server" Width="400"
                                        StoreID="SFAsen" DisplayField="DesAsentamiento" ValueField="ID_Asentamiento">
                                        <DirectEvents>
                                            <Select OnEvent="AsentamientoFacturacion_Select">
                                            </Select>
                                        </DirectEvents>
                                    </ext:ComboBox>
                                    <ext:TextField ID="FtxtCalle" FieldLabel="Calle"  Disabled ="true" MaxLength="100" runat="server" Width="400"
                                        Text="" />
                                    <ext:TextField ID="FtxtInterior" FieldLabel="Num. Int." Disabled ="true"  MaxLength="5" runat="server"
                                        Width="400" Text="" />
                                    <ext:TextField ID="FtxtExterior" FieldLabel="Num. Ext." Disabled ="true"  MaxLength="5" runat="server"
                                        Width="400" Text="" />
                                    <ext:TextArea ID="FtxtEntreCalle" FieldLabel="Entre Calles"  Disabled ="true" MaxLength="500" runat="server"
                                        Width="400" Height="50" />
                                    <ext:TextArea ID="FtxtReferencia" FieldLabel="Referencias"  Disabled ="true" MaxLength="500" runat="server"
                                        Width="400" Height="50" />
                                </Items>
                            </ext:Panel>
                        </Items>
                        <%--<FooterBar>
                            <ext:Toolbar ID="Toolbar1" runat="server" EnableOverflow="true">
                                <Items>
                                    <ext:Button ID="btnGuardar" runat="server" Text="Guardar" Icon="Add">
                                        <DirectEvents>
                                            <Click OnEvent="btnGuardar_Click" Before="var valid= #{FormPanel1}.getForm().isValid(); if (!valid) {} return valid;" />
                                        </DirectEvents>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </FooterBar>--%>
                    </ext:TabPanel>
                </Items>
            </ext:FormPanel>
         </East>
    </ext:BorderLayout>
</asp:Content>
