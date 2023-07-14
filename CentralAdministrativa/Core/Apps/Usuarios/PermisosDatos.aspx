﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="PermisosDatos.aspx.cs" Inherits="Usuarios.PermisosDatos" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        var template = '<span style="color:{0};">{1}</span>';

        var change = function (value) {
            return String.format(template, (value = true) ? "green" : "red", value);
        };

        var prepareToolbar = function (grid, toolbar, rowIndex, record) {
            // for example hide 'Edit' button if price < 50
            if (record.get("Permitir") == true) { //ACTIVO
                //                toolbar.items.get(0).hide(); //Delete
                toolbar.items.get(1).hide(); //sep
                //                toolbar.items.get(2).hide(); //asgina
            } else {
                toolbar.items.get(0).hide();
            }
        }
    

    </script>
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center Split="true">
            <ext:Panel ID="Panel2" runat="server" Title="Usuarios de Central Administrativa"
                Collapsed="false" Layout="Fit" AutoScroll="true" Width="428.5">
                <Content>
                    <ext:Store ID="SAllUser" runat="server" OnRefreshData="RefreshGridUsuarios">
                        <Reader>
                            <ext:JsonReader IDProperty="UserId">
                                <Fields>
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:Store ID="StoreUsuarios" runat="server" OnRefreshData="RefreshGridUsuarios">
                        <Reader>
                            <ext:JsonReader IDProperty="UserId">
                                <Fields>
                                    <ext:RecordField Name="UserId" />
                                    <ext:RecordField Name="UserName" />
                                    <ext:RecordField Name="Nombre" />
                                    <ext:RecordField Name="Apaterno" />
                                    <ext:RecordField Name="Amaterno" />
                                    <ext:RecordField Name="Email" />
                                    <ext:RecordField Name="Estatus" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:Store ID="SAplicaciones" runat="server">
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
                    <ext:Store ID="STables" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="TableId">
                                <Fields>
                                    <ext:RecordField Name="TableId" />
                                    <ext:RecordField Name="TableName" />
                                    <ext:RecordField Name="ApplicationId" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:Store ID="SFields" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="TableId">
                                <Fields>
                                    <ext:RecordField Name="TableId" />
                                    <ext:RecordField Name="FieldName" />
                                    <ext:RecordField Name="ApplicationId" />
                                    <ext:RecordField Name="StoredProcedure" />
                                    <ext:RecordField Name="ConnectionConfigName" />
                                    <ext:RecordField Name="NombreCortoCampo" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:Store ID="SCamposTablasUsuario" runat="server" OnRefreshData="RefreshGridValores">
                        <Reader>
                            <ext:JsonReader IDProperty="registerId">
                                <Fields>
                                    <ext:RecordField Name="ApplicationName" />
                                    <ext:RecordField Name="TableId" />
                                    <ext:RecordField Name="UserId" />
                                    <ext:RecordField Name="ValueDescription" />
                                    <ext:RecordField Name="Value" />
                                    <ext:RecordField Name="FieldName" />
                                    <ext:RecordField Name="RegisterId" />
                                    <ext:RecordField Name="Permitir" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanel1" runat="server" StoreID="SAllUser" StripeRows="true"
                                Header="true" Border="false" AutoExpandColumn="Email">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel1" runat="server">
                                </ColumnModel>
                                <DirectEvents>
                                    <RowDblClick OnEvent="GridUsuarios_DblClik">
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
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="SAllUser" BufferResize="100"
                                        DisplayInfo="true" DisplayMsg="Mostrando Usuarios {0} - {1} de {2}" HideRefresh="true" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </Center>
        <East Split="true">
            <ext:FormPanel ID="FormPanelAsignarVistas" Width="600px" Collapsible="true" Collapsed="true"
                runat="server" Border="true" AutoScroll="false" Layout="FitLayout">
                <Content>
                    <ext:BorderLayout ID="BorderLayout3" runat="server">
                        <North Split="true">
                            <ext:FormPanel ID="FormPanelDetalle" runat="server" Border="false" Header="false" Width="600"
                                LabelAlign="Left" Layout="FormLayout" Height="210" Padding="10">
                                <Items>
                                    <ext:Panel ID="Panel1" runat="server" Title="Clonar Vistas a partir de un Usuario"
                                        AutoHeight="true" LabelAlign="Top" FormGroup="true" Layout="TableLayout" AutoWidth="true">
                                        <Items>
                                            <ext:Panel ID="Panel7" runat="server" Border="false" Header="false" Width="300px"
                                                ColumnWidth=".5" Layout="Form" LabelAlign="Top">
                                                <Items>
                                                    <ext:ComboBox ID="cmbUsuarioOrigen" TabIndex="2" runat="server" FieldLabel="Usuario Origen"
                                                        StoreID="StoreUsuarios" DisplayField="UserName" ValueField="UserId" MsgTarget="Side"
                                                        EmptyText="Selecciona una Opción" AllowBlank="false" AnchorHorizontal="90%" Width="300px">
                                                    </ext:ComboBox>
                                                </Items>
                                            </ext:Panel>
                                            <ext:Panel ID="Panel10" runat="server" Border="false" Header="false" Width="300px"
                                                ColumnWidth=".5" Layout="Form" LabelAlign="Top">
                                                <Items>
                                                    <ext:Button ID="btnGuardar" FieldLabel="Clonar Permisos del Usuario Origen"
                                                        runat="server" Text="Clonar Permisos" Icon="Add">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnClonar_Click" />
                                                        </DirectEvents>
                                                    </ext:Button>
                                                </Items>
                                            </ext:Panel>
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel ID="Panel9" runat="server" Title="Detallar Vistas del Usuario" AutoHeight="true"
                                        LabelAlign="Top" FormGroup="true" Layout="TableLayout" AutoWidth="true">
                                        <Items>
                                            <ext:Panel ID="Panel4" runat="server" Border="false" Header="false" Width="300px"
                                                ColumnWidth=".5" Layout="Form" LabelAlign="Top">
                                                <Items>
                                                    <ext:ComboBox ID="cBoxApp" TabIndex="1" FieldLabel="Aplicación" runat="server" StoreID="SAplicaciones"
                                                        DisplayField="Description" ValueField="ApplicationId" EmptyText="Selecciona una Opción"
                                                        MsgTarget="Side" AllowBlank="false" AnchorHorizontal="90%" Width="300px">
                                                        <DirectEvents>
                                                            <Select OnEvent="App_Select" />
                                                            <Select OnEvent="Table_Select" />
                                                        </DirectEvents>
                                                        <Listeners>
                                                            <Select Handler="#{txtFiltro}.reset(); #{cmbFields}.reset(); #{GridPanel3}.getStore().removeAll();" />
                                                        </Listeners>
                                                    </ext:ComboBox>
                                                    <ext:TextField ID="txtFiltro" AnchorHorizontal="90%" LabelAlign="Top" EmptyText="(Opcional)"
                                                        FieldLabel="Filtro para Descripción del Valor" runat="server" Width="575px">
                                                    </ext:TextField>
                                                </Items>
                                            </ext:Panel>
                                            <ext:Panel ID="Panel5" runat="server" Border="false" Header="false" Width="300px"
                                                ColumnWidth=".5" Layout="Form" LabelAlign="Top">
                                                <Items>
                                                    <ext:ComboBox ID="cmbFields" TabIndex="2" runat="server" FieldLabel="Campo" StoreID="SFields"
                                                        DisplayField="NombreCortoCampo" ValueField="TableId" MsgTarget="Side" EmptyText="Selecciona una Opción"
                                                        AllowBlank="false" AnchorHorizontal="90%" Width="300px">
                                                        <Listeners>
                                                            <Select Handler="#{txtFiltro}.reset(); #{GridPanel3}.getStore().removeAll();" />
                                                        </Listeners>
                                                    </ext:ComboBox>
                                                    <ext:Button ID="btnBuscar" FieldLabel="Aplicar Criterios de Búsqueda" Text="Buscar"
                                                        Icon="Magnifier" runat="server">
                                                        <DirectEvents>
                                                            <Click OnEvent="LlenaPosiblesValues" Before="if (!#{cBoxApp}.isValid() || !#{cmbFields}.isValid())
                                                                { return false; } 
                                                                else { #{GridPanel3}.getStore().removeAll(); }">
                                                                <EventMask ShowMask="true" Msg="Buscando Filtros..." MinDelay="500" />
                                                            </Click>
                                                        </DirectEvents>
                                                    </ext:Button>
                                                </Items>
                                            </ext:Panel>
                                        </Items>
                                    </ext:Panel>
                                </Items>
                            </ext:FormPanel>
                        </North>
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanel3" runat="server" StoreID="SCamposTablasUsuario" StripeRows="true"
                                Border="false" AutoExpandColumn="ValueDescription">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel4" runat="server">
                                </ColumnModel>
                                <DirectEvents>
                                    <Command OnEvent="EjecutarComando">
                                        <ExtraParams>
                                            <ext:Parameter Name="id" Value="record.data.ID" Mode="Raw" />
                                        </ExtraParams>
                                    </Command>
                                </DirectEvents>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel4" runat="server" SingleSelect="true">
                                    </ext:RowSelectionModel>
                                </SelectionModel>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBarGrid" runat="server" StoreID="SCamposTablasUsuario"
                                        DisplayInfo="true" DisplayMsg="Mostrando Valores {0} - {1} de {2}" HideRefresh="true">
                                        <Items>
                                            <ext:Label ID="Label1" runat="server" Text="Valores por página:" />
                                            <ext:ToolbarSpacer ID="ToolbarSpacer1" runat="server" Width="10" />
                                            <ext:ComboBox ID="ComboBox1" runat="server" Width="80">
                                                <Items>
                                                    <ext:ListItem Text="10" />
                                                    <ext:ListItem Text="20" />
                                                    <ext:ListItem Text="30" />
                                                </Items>
                                                <SelectedItem Value="10" />
                                                <Listeners>
                                                    <Select Handler="#{PagingToolBarGrid}.pageSize = parseInt(this.getValue()); #{PagingToolBarGrid}.doLoad();" />
                                                </Listeners>
                                            </ext:ComboBox>
                                        </Items>
                                    </ext:PagingToolbar>
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:FormPanel>
        </East>
    </ext:BorderLayout>
</asp:Content>
