<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="RelacionUsuarioEmpresa.aspx.cs" Inherits="CentroContacto.RelacionUsuarioEmpresa" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout runat="server">
        <Center Split="true">
            <ext:GridPanel ID="GridPanelUsuarios" runat="server" Border="false" Layout="FitLayout" Title="Usuarios" AutoScroll="true"
                AutoExpandColumn="Email">
                <LoadMask ShowMask="false" />
                <Store>
                    <ext:Store ID="StoreUsuarios" runat="server" RemoteSort="true" OnRefreshData="StoreUsuarios_RefreshData">
                        <AutoLoadParams>
                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                        </AutoLoadParams>
                        <Proxy>
                            <ext:PageProxy />
                        </Proxy>
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Usuario">
                                <Fields>
                                    <ext:RecordField Name="ID_Usuario" />
                                    <ext:RecordField Name="Movil" />
                                    <ext:RecordField Name="Email" />
                                    <ext:RecordField Name="Usuario" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <ColumnModel runat="server">
                    <Columns>
                        <ext:Column DataIndex="ID_Usuario" Header="ID_Usuario" />
                        <ext:Column DataIndex="Usuario" Header="Usuario" />
                        <ext:Column DataIndex="Email" Header="Email" />
                        <ext:Column DataIndex="Movil" Header="Movil" />
                    </Columns>
                </ColumnModel>
                <DirectEvents>
                    <RowDblClick OnEvent="GridPanelUsuarios_DblClik">
                        <EventMask ShowMask="true" Msg="Buscando Datos del Usuario..." MinDelay="500" />
                        <ExtraParams>
                            <ext:Parameter Name="ID_Usuario" Value="Ext.encode(#{GridPanelUsuarios}.getRowsValues({selectedOnly:true})[0].ID_Usuario)" Mode="Raw" />
                            <ext:Parameter Name="Usuario" Value="Ext.encode(#{GridPanelUsuarios}.getRowsValues({selectedOnly:true})[0].Usuario)" Mode="Raw" />
                        </ExtraParams>
                    </RowDblClick>
                </DirectEvents>
                <SelectionModel>
                    <ext:RowSelectionModel runat="server" SingleSelect="true">
                        <DirectEvents>
                            <RowDeselect OnEvent="QuitarSeleccion" />
                        </DirectEvents>
                    </ext:RowSelectionModel>
                </SelectionModel>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingUsuarios" runat="server" StoreID="StoreUsuarios" DisplayInfo="true"
                        DisplayMsg="Mostrando Usuarios {0} - {1} de {2}" HideRefresh="true" />
                </BottomBar>
            </ext:GridPanel>
        </Center>
        <East Split="true">
            <ext:FormPanel ID="EastFormPanel" runat="server" Title="Usuario - " Layout="FormLayout" Collapsed="true"
                Collapsible="true" Width="400" Padding="10" LabelWidth="120">
                <Items>
                    <ext:BorderLayout runat="server">
                        <North Split="true">
                            <ext:FormPanel runat="server" LabelWidth="120" Layout="FitLayout" Border="false" Height="200">
                                <Items>
                                    <ext:FieldSet ID="FieldSetRelActual" runat="server" Title="Empresa Relacionada" Layout="FormLayout">
                                        <Items>
                                            <ext:Label ID="lblRelacionActual" runat="server" StyleSpec="font-weight: bold;font-family:segoe ui;font-size: 12px;" />
                                        </Items>
                                    </ext:FieldSet>
                                </Items>
                            </ext:FormPanel>
                        </North>
                        <Center Split="true">
                            <ext:FormPanel ID="FormPanelNuevaRel" runat="server" LabelWidth="75" Layout="FitLayout" Border="false">
                                <Items>
                                    <ext:FieldSet ID="FieldSetNuevaRel" runat="server" Title="Relacionar Otra Empresa" Layout="FormLayout">
                                        <Items>
                                            <ext:Hidden ID="hdnIdUsuario" runat="server" />
                                            <ext:Hidden ID="hdnClaveEmpresa" runat="server" />
                                            <ext:Hidden ID="hdnNombreEmpresa" runat="server" />
                                            <ext:Hidden ID="hdnCuentaEmpresa" runat="server" />
                                            <ext:Panel runat="server" Border="false" Height="20" />
                                            <ext:ComboBox ID="cBoxEmpresa" runat="server" FieldLabel="Empresa" EmptyText="Selecciona..."
                                                Width="250" ValueField="ID_Colectiva" DisplayField="NombreORazonSocial" Mode="Local"
                                                AutoSelect="true" Editable="true" ForceSelection="true" TypeAhead="true" MinChars="1"
                                                MatchFieldWidth="false" AllowBlank="false">
                                                <Store>
                                                    <ext:Store ID="StoreEmpresa" runat="server">
                                                        <Reader>
                                                            <ext:JsonReader IDProperty="ID_Colectiva">
                                                                <Fields>
                                                                    <ext:RecordField Name="ID_Colectiva" />
                                                                    <ext:RecordField Name="ClaveColectiva" />
                                                                    <ext:RecordField Name="NombreORazonSocial" />
                                                                    <ext:RecordField Name="Cuenta" />
                                                                </Fields>
                                                            </ext:JsonReader>
                                                        </Reader>
                                                        <SortInfo Field="NombreORazonSocial" Direction="ASC" />
                                                    </ext:Store>
                                                </Store>
                                                <Listeners>
                                                    <Select Handler="var id = this.getValue(); var record = this.getStore().getById(id);
                                                        #{hdnClaveEmpresa}.setValue(record.get('ClaveColectiva'));
                                                        #{hdnNombreEmpresa}.setValue(record.get('NombreORazonSocial'));
                                                        #{hdnCuentaEmpresa}.setValue(record.get('Cuenta')); " />
                                                </Listeners>
                                            </ext:ComboBox>
                                            <ext:Panel runat="server" Border="false" Height="40" />
                                        </Items>
                                    </ext:FieldSet>
                                </Items>
                                <Buttons>
                                    <ext:Button ID="btnAceptar" runat="server" Icon="Tick" Text="Aceptar">
                                        <DirectEvents>
                                            <Click OnEvent="btnActualizaEmpresa_Click" Before="var valid= #{FormPanelNuevaRel}.getForm().isValid(); if (!valid) {} return valid;">
                                                <EventMask ShowMask="true" Msg="Relacionando Empresa..." MinDelay="500" />
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnCancelar" runat="server" Icon="Cross" Text="Cancelar">
                                        <DirectEvents>
                                            <Click OnEvent="btnCancelar_Click" />
                                        </DirectEvents>
                                    </ext:Button>
                                </Buttons>
                            </ext:FormPanel>
                        </Center>
                    </ext:BorderLayout>
                </Items>
            </ext:FormPanel>
        </East>
    </ext:BorderLayout>
</asp:Content>
