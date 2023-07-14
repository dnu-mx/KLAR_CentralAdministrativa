<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="EditAfiliaciones.aspx.cs" Inherits="TpvWeb.EditAfiliaciones" %>

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

        var afterEdit = function (e) {
            EditaAfiliaciones.ActualizarClaveExtra(e.record.data.ID_ColectivaClaveExtra,
                e.record.data.ClaveColectiva, e.record.data.FechaAsignacion);
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Hidden ID="hdnIdColectiva" runat="server" />
    <ext:Window ID="WdwNuevaClave" runat="server" Title="Nueva" Icon="Add" Width="300" Height="180"
        Hidden="true" Modal="true" Resizable="false">
        <Items>
            <ext:FormPanel ID="FormPanelNuevaClave" runat="server" Padding="10" LabelWidth="80" Border="false"
                Layout="FormLayout" LabelAlign="Top">
                <Items>
                    <ext:Hidden ID="hdnIdBin" runat="server" />
                    <ext:TextField ID="txtNuevaClave" runat="server" FieldLabel="Clave" AllowBlank="false"
                        Width="265" MaxLength="50" />
                    <ext:DateField ID="dfNuevaFechaAs" runat="server" Vtype="daterange" AllowBlank="false"
                        FieldLabel="Fecha de Asignación" MsgTarget="Side" Format="dd/MM/yyyy"
                        Width="265" Editable="false" MaxDate="<%# DateTime.Now %>" AutoDataBind="true" />
                </Items>
                <Buttons>
                    <ext:Button runat="server" Text="Cancelar" Icon="Cancel">
                        <Listeners>
                            <Click Handler="#{WdwNuevaClave}.hide();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button ID="btnAddNuevaClave" runat="server" Text="Aceptar" Icon="Tick">
                        <DirectEvents>
                            <Click OnEvent="btnAddNuevaClave_Click" Before="var valid= #{FormPanelNuevaClave}.getForm().isValid(); if (!valid) {} return valid;">
                                <EventMask ShowMask="true" Msg="Creando Nueva Clave..." MinDelay="500" />
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center Split="true">
            <ext:Panel ID="Panel2" runat="server" Title="Afiliaciones" Collapsed="false" Layout="Fit"
                AutoScroll="true" Width="428.5">
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
                                        DisplayInfo="true" DisplayMsg="Mostrando Afiliaciones {0} - {1} de {2}" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </Center>
        <East Split="true">
            <ext:FormPanel ID="FormPanel1" Width="428.5" Title="-" runat="server" Collapsed="true" Collapsible="true"
                Border="false" Layout="FitLayout" >
                <Items>
                    <ext:Panel ID="PanelInfoGral" runat="server" Padding="10" Title="Información General" AutoHeight="true"
                        FormGroup="true" Layout="FormLayout">
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
                                AllowBlank="false" MsgTarget="Side" MaxLength="50" runat="server" Text="" Width="300"
                                Hidden="true"/>
                            <ext:ComboBox ID="cmbPadreTipoColectiva" EmptyText="Selecciona una Opción" FieldLabel="Colectiva Padre"
                                AllowBlank="false" MsgTarget="Side" runat="server" Width="300" StoreID="SCPadre"
                                DisplayField="NombreORazonSocial" ValueField="ID_Colectiva">
                            </ext:ComboBox>
                            <ext:TextField ID="txtNombre" FieldLabel="Descripción" AllowBlank="false" MsgTarget="Side"
                                MaxLength="50" runat="server" Width="300" Text="" />
                            <ext:TextField ID="txtApaterno" FieldLabel="Ubicación 1" AllowBlank="false" MsgTarget="Side"
                                MaxLength="50" runat="server" Text="" Width="300" />
                            <ext:TextField ID="txtAmaterno" FieldLabel="Ubicación 2" AllowBlank="false" MsgTarget="Side"
                                MaxLength="50" runat="server" Width="300" Text="" />
                            <ext:TextField ID="txtClaveSupervisor" InputType="Password" FieldLabel="Clave o Tarjeta de Supervisor"
                                AllowBlank="true" MsgTarget="Side" MaxLength="50" runat="server" Width="300"
                                Text="">
                            </ext:TextField>
                            <ext:Panel runat="server" Layout="ColumnLayout" Width="400" Height="25" Border="false">
                                <Items>
                                    <ext:Panel runat="server" Border="false" Width="300" Height="25"/>
                                    <ext:Panel runat="server" Border="false" Width="100" Height="25">
                                        <Items>
                                            <ext:Button ID="btnGuardar" runat="server" Text="Guardar" Icon="Disk" Width="100">
                                                <DirectEvents>
                                                    <Click OnEvent="btnGuardar_Click" Before="var valid= #{FormPanel1}.getForm().isValid(); if (!valid) {} return valid;" />
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Panel>
                                </Items>
                            </ext:Panel>
                        </Items>
                    </ext:Panel>
                    <ext:GridPanel ID="GridInfoAd" runat="server" Title="Claves Adicionales" Height="235" Border="false" 
                        Layout="FitLayout">
                        <TopBar>
                            <ext:Toolbar runat="server">
                                <Items>
                                    <ext:ToolbarFill runat="server" />
                                    <ext:ToolbarSeparator runat="server" />
                                    <ext:Button ID="btnNuevaClaveExtra" runat="server" Text="Nueva Clave" Icon="Add">
                                        <Listeners>
                                            <Click Handler="#{WdwNuevaClave}.show();" />
                                        </Listeners>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                        <Store>
                            <ext:Store ID="StoreClavesExtra" runat="server">
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_ColectivaClaveExtra">
                                        <Fields>
                                            <ext:RecordField Name="ID_ColectivaClaveExtra" />
                                            <ext:RecordField Name="ClaveColectiva" />
                                            <ext:RecordField Name="FechaAsignacion" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <ColumnModel ID="ColumnModel2" runat="server">
                            <Columns>
                                <ext:Column Hidden="true" DataIndex="ID_ColectivasClavesExtras" />
                                <ext:Column Header="Clave" Sortable="true" DataIndex="ClaveColectiva" Width="200">
                                    <Editor>
                                        <ext:TextField EmptyText="" runat="server" AllowBlank="false" />
                                    </Editor>
                                </ext:Column>
                                <ext:DateColumn DataIndex="FechaAsignacion" Header="Fecha de Asignación"
                                    Format="dd/MM/yyyy" Width="200">
                                    <Editor>
                                        <ext:DateField runat="server" MaxDate="<%# DateTime.Now %>"
                                             AutoDataBind="true" Vtype="daterange" AllowBlank="false" />
                                    </Editor>
                                </ext:DateColumn>
                            </Columns>
                        </ColumnModel>
                        <SelectionModel>
                            <ext:RowSelectionModel SingleSelect="true" />
                        </SelectionModel>
                        <Plugins>
                            <ext:RowEditor runat="server" SaveText="Actualizar" CancelText="Cancelar">
                                <Listeners>
                                    <AfterEdit Fn="afterEdit" />
                                </Listeners>
                            </ext:RowEditor>
                        </Plugins>
                        <BottomBar>
                            <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="StoreClavesExtra" DisplayInfo="true"
                                DisplayMsg="Mostrando Claves Adicionales {0} - {1} de {2}" HideRefresh="true" />
                        </BottomBar>
                    </ext:GridPanel>
                </Items>
            </ext:FormPanel>
        </East>
    </ext:BorderLayout>
</asp:Content>
