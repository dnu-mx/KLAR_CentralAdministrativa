<%@ Page Title="" Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master"
    CodeBehind="MenuAsigna.aspx.cs" Inherits="TpvWeb.MenuAsigna" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <ext:Store ID="stTPVs" runat="server" OnRefreshData="stTPVs_Refresh">
        <Reader>
            <ext:JsonReader IDProperty="ID_Colectiva">
                <Fields>
                    <ext:RecordField Name="ID_Colectiva"/>
                    <ext:RecordField Name="ClaveColectiva"/>
                    <ext:RecordField Name="NombreORazonSocial"/>
                    <ext:RecordField Name="NombreTPV"/>
                    <ext:RecordField Name="ClaveMenu"/>
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:Store ID="stMenus" runat="server" OnRefreshData="stMenus_Refresh">
        <Reader>
            <ext:JsonReader IDProperty="ID_Menu">
                <Fields>
                    <ext:RecordField Name="ID_Menu" />
                    <ext:RecordField Name="Clave" />
                    <ext:RecordField Name="Descripcion" />
                    <ext:RecordField Name="Version" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:ColumnLayout runat="server" FitHeight="true" Split="true">
        <Columns>
            <ext:LayoutColumn ColumnWidth="0.50">
                <ext:Panel ID="panel_tpvs" runat="server" Title="TPVs">
                    <Items>
                        <ext:BorderLayout runat="server">
                            <Center>
                                <ext:GridPanel ID="grid_tpvs" runat="server" StoreID="stTPVs" AnchorVertical="100%">
                                    <TopBar>
                                        <ext:Toolbar ID="Toolbar1" runat="server">
                                            <Items>
                                                <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                <ext:Button ID="btn_asignar_menu" runat="server" Text="Desasignar Menú" Icon="Delete">
                                                    <DirectEvents>
                                                        <Click OnEvent="Click_DesasignarMenuATPV">
                                                            <ExtraParams>
                                                                <ext:Parameter Name="Values" Value="Ext.encode(#{grid_tpvs}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                                            </ExtraParams>
                                                        </Click>
                                                    </DirectEvents>
                                                </ext:Button>
                                            </Items>
                                        </ext:Toolbar>
                                    </TopBar>
                                    <SelectionModel>
                                        <ext:CheckboxSelectionModel ID="CheckboxSelectionModel1" runat="server" RowSpan="2" />
                                    </SelectionModel>
                                    <Plugins>
                                        <ext:GridFilters runat="server" ID="GridFilters1" Local="true">
                                            <Filters>
                                                <ext:StringFilter DataIndex="ClaveColectiva" />
                                                <ext:StringFilter DataIndex="NombreORazonSocial" />
                                            </Filters>
                                        </ext:GridFilters>
                                    </Plugins>
                                    <BottomBar>
                                        <ext:PagingToolbar runat="server"></ext:PagingToolbar>
                                    </BottomBar>
                                </ext:GridPanel>
                            </Center>
                        </ext:BorderLayout>
                    </Items>
                </ext:Panel>
            </ext:LayoutColumn>
            <ext:LayoutColumn ColumnWidth="0.50">
                <ext:Panel ID="panel_menus" runat="server" Title="Menús">
                    <Items>
                        <ext:BorderLayout ID="BorderLayout1" runat="server">
                            <Center>
                                <ext:GridPanel ID="grid_menus" runat="server" StoreID="stMenus">
                                    <TopBar>
                                        <ext:Toolbar ID="Toolbar2" runat="server">
                                            <Items>
                                                <ext:Button ID="Button1" runat="server" Text="Asignar Menú" Icon="ArrowLeft">
                                                    <DirectEvents>
                                                        <Click OnEvent="Click_AsignarMenuATPV" >
                                                            <ExtraParams>
                                                                <ext:Parameter Name="ValuesMenu" Value="Ext.encode(#{grid_menus}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                                                <ext:Parameter Name="ValuesTPV" Value="Ext.encode(#{grid_tpvs}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                                            </ExtraParams>
                                                        </Click>
                                                    </DirectEvents>
                                                </ext:Button>
                                            </Items>
                                        </ext:Toolbar>
                                    </TopBar>
                                    <SelectionModel>
                                        <ext:CheckboxSelectionModel ID="CheckboxSelectionModel2" runat="server" RowSpan="2" SingleSelect="true" />
                                    </SelectionModel>

                                    <BottomBar>
                                        <ext:PagingToolbar ID="PagingToolbar1" runat="server"></ext:PagingToolbar>
                                    </BottomBar>
                                </ext:GridPanel>
                            </Center>
                        </ext:BorderLayout>
                    </Items>
                </ext:Panel>
            </ext:LayoutColumn>
        </Columns>
    </ext:ColumnLayout>
</asp:Content>
