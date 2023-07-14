<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PermisosRolesDesdeCadenaComercial.aspx.cs" Inherits="Usuarios.PermisosRolesDesdeCadenaComercial" %>
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
                    <ext:Store ID="SRoles" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="RoleId">
                                <Fields>
                                    <ext:RecordField Name="RoleId" />
                                    <ext:RecordField Name="RoleName" />
                                    <ext:RecordField Name="Description" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:Store ID="SRolesAsignados" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="RoleId">
                                <Fields>
                                    <ext:RecordField Name="RoleId" />
                                    <ext:RecordField Name="RoleName" />
                                    <ext:RecordField Name="Description" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:Store ID="SValues" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="Value">
                                <Fields>
                                    <ext:RecordField Name="Value" />
                                    <ext:RecordField Name="ValueDescription" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:Store ID="SAppPerfil" runat="server" OnRefreshData="RefreshGridRoles">
                        <Reader>
                            <ext:JsonReader IDProperty="RoleId">
                                <Fields>
                                    <ext:RecordField Name="ApplicationId" />
                                    <ext:RecordField Name="RoleId" />
                                    <ext:RecordField Name="UserId" />
                                    <ext:RecordField Name="Description" />
                                    <ext:RecordField Name="RoleName" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:Store ID="SCamposTablasUsuario" runat="server"  OnRefreshData="RefreshGridValores">
                        <Reader>
                            <ext:JsonReader IDProperty="registerId">
                                <Fields>
                                    <ext:RecordField Name="ApplicationName" />
                                    <ext:RecordField Name="TableId" />
                                    <ext:RecordField Name="UserId" />
                                    <ext:RecordField Name="NombreCortoCampo" />
                                    <ext:RecordField Name="Value" />
                                    <ext:RecordField Name="FieldName" />
                                    <ext:RecordField Name="registerId" />
                                    <ext:RecordField Name="Permitir" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanel1" runat="server" StoreID="SAllUser" StripeRows="true"
                                Header="true" Border="false">
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
                                        DisplayInfo="true" DisplayMsg="Mostrando Usuarios {0} - {1} de {2}" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </Center>
        <East Split="true">
            <ext:Panel ID="FormPanel1" Width="600px" Collapsible="true" Collapsed="true" Title="Editando Usuarios"
                runat="server" Border="true" AutoScroll="false" Layout="FitLayout">
                <Content>
                    <ext:TabPanel runat="server" Width="400.5px" ActiveTabIndex="0" TabPosition="Bottom"
                        Border="false" AutoScroll="false" AutoWidth="true" ID="TabPanel1" Title="ctl71">
                        <Items>
                            <ext:Panel ID="Panel12" runat="server" Title="Asignación de Roles" Width="440" FormGroup="true">
                                <Content>
                                    <ext:BorderLayout ID="BorderLayout12" runat="server">
                                        <North Split="true" MinHeight="90px" MaxHeight="90px">
                                            <ext:Panel ID="Panel1" Height="90px" runat="server" Border="false">
                                                <Content>
                                                    <table>
                                                        <tr>
                                                            <td colspan="2">
                                                                <ext:ComboBox ID="cmbApp" runat="server" Width="550px" Visible="false" StoreID="SAplicaciones"
                                                                    DisplayField="Description" ValueField="ApplicationId">
                                                                </ext:ComboBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                Roles:<br />
                                                                <ext:ComboBox ID="cmbPerfil" runat="server" Width="550px" StoreID="SRoles" DisplayField="Description"
                                                                    ValueField="RoleId">
                                                                </ext:ComboBox>
                                                            </td>
                                                            <td>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2">
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2" align="right">
                                                                <table>
                                                                    <tr>
                                                                        <td>
                                                                            <ext:Button ID="Button1" runat="server" Text="Agregar" Icon="Add">
                                                                                <DirectEvents>
                                                                                    <Click OnEvent="btnAgregarRole_Click">
                                                                                        <ExtraParams>
                                                                                            <ext:Parameter Name="User" Value="Ext.encode(#{GridPanel1}.getRowsValues({selectedOnly:true})[0].UserName)" Mode="Raw" />
                                                                                            <ext:Parameter Name="Nombre" Value="Ext.encode(#{GridPanel1}.getRowsValues({selectedOnly:true})[0].Nombre)" Mode="Raw" />
                                                                                            <ext:Parameter Name="ApPaterno" Value="Ext.encode(#{GridPanel1}.getRowsValues({selectedOnly:true})[0].Apaterno)" Mode="Raw" />
                                                                                            <ext:Parameter Name="ApMaterno" Value="Ext.encode(#{GridPanel1}.getRowsValues({selectedOnly:true})[0].Amaterno)" Mode="Raw" />
                                                                                            <ext:Parameter Name="Email" Value="Ext.encode(#{GridPanel1}.getRowsValues({selectedOnly:true})[0].Email)" Mode="Raw" />
                                                                                        </ExtraParams>
                                                                                    </Click>
                                                                                </DirectEvents>
                                                                            </ext:Button>
                                                                        </td>
                                                                        <td>
                                                                            <ext:Button ID="Button2" runat="server" Text="Eliminar" Icon="Delete">
                                                                                <DirectEvents>
                                                                                    <Click OnEvent="btnEliminarRole_Click" />
                                                                                </DirectEvents>
                                                                            </ext:Button>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                        </tr>
                                                    </table>
                                                </Content>
                                            </ext:Panel>
                                        </North>
                                        <Center Split="true">
                                            <ext:GridPanel ID="GridPanel2" runat="server" Header="true" Title="Roles Asignados"
                                                StoreID="SAppPerfil" Layout="FitLayout" StripeRows="true" AutoHeight="false"
                                                Border="false" AutoExpandColumn="Description">
                                                <LoadMask ShowMask="false" />
                                                <ColumnModel ID="ColumnModel2" runat="server">
                                                </ColumnModel>
                                                <SelectionModel>
                                                    <ext:RowSelectionModel ID="RowSelectionModel2" runat="server" SingleSelect="true">
                                                    </ext:RowSelectionModel>
                                                </SelectionModel>
                                                <BottomBar>
                                                    <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="SAppPerfil" DisplayInfo="true"
                                                        DisplayMsg="Mostrando Roles Asignados {0} - {1} de {2}" />
                                                </BottomBar>
                                            </ext:GridPanel>
                                        </Center>
                                    </ext:BorderLayout>
                                </Content>
                            </ext:Panel>
                            <ext:Panel ID="Panel8" runat="server" FormGroup="true" Visible="false" Title="Filtros a Datos" Width="600px">
                                <Content>
                                    <ext:BorderLayout ID="BorderLayout3" runat="server">
                                        <North Split="true" MinHeight="220px" MaxHeight="220px">
                                            <ext:FormPanel ID="FormPanel2" Width="600px" Height="220px"  runat="server" Border="false"
                                                Collapsed="false" Collapsible="false">
                                                <Items>
                                                    <ext:Panel ID="Panel3"  runat="server" Border="false" LabelAlign="Top"
                                                        Layout="TableLayout" Width="600px">
                                                        <Items>
                                                            <ext:Panel ID="Panel4" runat="server" Border="false" Header="false" Width="300px"
                                                                ColumnWidth=".5" Layout="Form" LabelAlign="Top">
                                                                <Items>
                                                                    <ext:ComboBox ID="cmbApp2" TabIndex="1" FieldLabel="Aplicacion" runat="server" StoreID="SAplicaciones"
                                                                        DisplayField="Description" ValueField="ApplicationId" EmptyText="Selecciona una Opción"
                                                                        MsgTarget="Side" AllowBlank="false" AnchorHorizontal="90%" Width="300px">
                                                                        <DirectEvents>
                                                                            <Select OnEvent="App_Select" />
                                                                            <Select OnEvent="Table_Select" />
                                                                        </DirectEvents>
                                                                    </ext:ComboBox>
                                                                    <ext:ComboBox ID="cmbPermiso" TabIndex="3" runat="server" FieldLabel="Nivel Permiso"
                                                                        MsgTarget="Side" EmptyText="Selecciona una Opción" AllowBlank="false" AnchorHorizontal="90%"
                                                                        Width="300px">
                                                                        <Items>
                                                                            <ext:ListItem Text="Permitir" Value="true" />
                                                                            <%--<ext:ListItem Text="NO Permitir" Value="false" />--%>
                                                                        </Items>
                                                                    </ext:ComboBox>
                                                                </Items>
                                                            </ext:Panel>
                                                            <ext:Panel ID="Panel5" runat="server" Border="false" Header="false" Width="300px"
                                                                ColumnWidth=".5" Layout="Form" LabelAlign="Top">
                                                                <Items>
                                                                    <ext:ComboBox ID="cmbFields" TabIndex="2" runat="server" FieldLabel="Campo" StoreID="SFields"
                                                                        DisplayField="NombreCortoCampo" ValueField="TableId" MsgTarget="Side" EmptyText="Selecciona una Opción"
                                                                        AllowBlank="false" AnchorHorizontal="90%" Width="300px">
                                                                     <%--   <DirectEvents>
                                                                            <Select OnEvent="LlenaPosiblesValues">
                                                                            </Select>
                                                                        </DirectEvents>--%>
                                                                    </ext:ComboBox>
                                                                    <ext:ComboBox ID="cmbValues" TabIndex="4" FieldLabel="Valores" runat="server" StoreID="SValues"
                                                                        DisplayField="ValueDescription" ValueField="Value" MsgTarget="Side" EmptyText="Selecciona una Opción"
                                                                        AllowBlank="false" AnchorHorizontal="90%" Width="300px">
                                                                    </ext:ComboBox>
                                                                </Items>
                                                            </ext:Panel>
                                                        </Items>
                                                    </ext:Panel>
                                                    <ext:Panel ID="Panel6" runat="server" Border="false" Header="false" Width="600px"
                                                        ColumnWidth=".5" LabelAlign="Top">
                                                        <Items>
                                                        <ext:TextArea ID="txtDescripcion" Height="70px" Disabled="true" AnchorHorizontal="90%" LabelAlign="Top" FieldLabel="Breve Descripción del Campo"
                                                                runat="server" Width="575px">
                                                            </ext:TextArea>
                                                        </Items>
                                                    </ext:Panel>
                                                </Items>
                                                <FooterBar >
                                                    <ext:Toolbar ID="Toolbar1" runat="server" EnableOverflow="true">
                                                        <Items>
                                                            <ext:Button ID="Button3" runat="server" Text="Agregar" Icon="Add">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnAgregarTableValue_Click" Before="var valid= #{FormPanel2}.getForm().isValid(); if (!valid) {} return valid;" />
                                                                </DirectEvents>
                                                            </ext:Button>
                                                            <ext:Button ID="Button4" runat="server" Text="Eliminar" Icon="Delete">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnEliminarTableValue_Click" />
                                                                </DirectEvents>
                                                            </ext:Button>
                                                        </Items>
                                                    </ext:Toolbar>
                                                </FooterBar>
                                            </ext:FormPanel>
                                        </North>
                                        <Center Split="true">
                                            <ext:GridPanel ID="GridPanel3" runat="server" StoreID="SCamposTablasUsuario" StripeRows="true"
                                                Border="false" AutoExpandColumn="NombreCortoCampo">
                                                <LoadMask ShowMask="false" />
                                                <ColumnModel ID="ColumnModel4" runat="server">
                                                </ColumnModel>
                                                <SelectionModel>
                                                    <ext:RowSelectionModel ID="RowSelectionModel4" runat="server" SingleSelect="true">
                                                    </ext:RowSelectionModel>
                                                </SelectionModel>
                                                <BottomBar>
                                                    <ext:PagingToolbar ID="PagingToolBar4" runat="server" StoreID="SCamposTablasUsuario"
                                                        DisplayInfo="true" DisplayMsg="Mostrando Valores Filtro Asignados {0} - {1} de {2}" />
                                                </BottomBar>
                                            </ext:GridPanel>
                                        </Center>
                                    </ext:BorderLayout>
                                </Content>
                            </ext:Panel>
                        </Items>
                    </ext:TabPanel>
                </Content>
            </ext:Panel>
        </East>
    </ext:BorderLayout>
</asp:Content>

