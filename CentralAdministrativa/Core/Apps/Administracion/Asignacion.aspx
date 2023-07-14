<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Asignacion.aspx.cs" Inherits="Administracion.Asignacion" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <ext:Store ID="StoreProductos" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_GrupoMA">
                <Fields>
                    <ext:RecordField Name="ID_GrupoMA" />
                    <ext:RecordField Name="Descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:Store ID="StoreProgramas" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_GrupoCuenta">
                <Fields>
                    <ext:RecordField Name="ID_GrupoCuenta" />
                    <ext:RecordField Name="Descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:Store ID="StoreTiposCuenta" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_TipoCuenta">
                <Fields>
                    <ext:RecordField Name="ID_TipoCuenta" />
                    <ext:RecordField Name="Descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:Store ID="StoreCadenaComercial" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="IDCadena">
                <Fields>
                    <ext:RecordField Name="IDCadena" />
                    <ext:RecordField Name="Descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:Store ID="StoreAsignaciones" runat="server">
    </ext:Store>

    <ext:BorderLayout runat="server">
        <Center>
            <ext:Panel runat="server">
                <BottomBar>
                    <ext:Toolbar runat="server">
                        <Items>
                            <ext:CenterLayout runat="server">
                                <Items>
                                <ext:Button runat="server" Text="Asignar" Icon="ArrowDown" Width="250">

                                </ext:Button>
                                    </Items>
                            </ext:CenterLayout>
                        </Items>
                    </ext:Toolbar>
                </BottomBar>
                <Items>
                    <ext:ColumnLayout ID="ColumnLayout1" runat="server" FitHeight="true" Split="true">
                        <Columns>
                            <ext:LayoutColumn ColumnWidth="0.25">
                                <ext:Panel runat="server" Title="Productos">
                                    <Items>
                                        <ext:BorderLayout runat="server">
                                            <Center>
                                                <ext:GridPanel ID="GridProductos" runat="server" StoreID="StoreProductos">
                                                    <SelectionModel>
                                                        <ext:CheckboxSelectionModel ID="CheckboxSelectionModel2" runat="server" RowSpan="2" SingleSelect="true" />
                                                    </SelectionModel>
                                                    <BottomBar><ext:PagingToolbar runat="server" /></BottomBar>
                                                </ext:GridPanel>
                                            </Center>
                                        </ext:BorderLayout>
                                    </Items>
                                </ext:Panel>
                            </ext:LayoutColumn>
                            <ext:LayoutColumn ColumnWidth="0.25">
                                <ext:Panel ID="Panel2" runat="server" Title="Programas">
                                    <Items>
                                        <ext:BorderLayout ID="BorderLayout1" runat="server">
                                            <Center>
                                                <ext:GridPanel ID="GridProgramas" runat="server" StoreID="StoreProgramas">
                                                    <SelectionModel>
                                                        <ext:CheckboxSelectionModel ID="CheckboxSelectionModel1" runat="server" RowSpan="2" SingleSelect="true" />
                                                    </SelectionModel>
                                                    <BottomBar><ext:PagingToolbar ID="PagingToolbar1" runat="server" /></BottomBar>
                                                </ext:GridPanel>
                                            </Center>
                                        </ext:BorderLayout>
                                    </Items>
                                </ext:Panel>
                            </ext:LayoutColumn>
                            <ext:LayoutColumn ColumnWidth="0.25">
                                <ext:Panel ID="Panel3" runat="server" Title="Tipos de Cuenta">
                                    <Items>
                                        <ext:BorderLayout ID="BorderLayout2" runat="server">
                                            <Center>
                                                <ext:GridPanel ID="GridTiposCuenta" runat="server" StoreID="StoreTiposCuenta">
                                                    <SelectionModel>
                                                        <ext:CheckboxSelectionModel ID="CheckboxSelectionModel3" runat="server" RowSpan="2" SingleSelect="true" />
                                                    </SelectionModel>
                                                    <BottomBar><ext:PagingToolbar ID="PagingToolbar2" runat="server" /></BottomBar>
                                                </ext:GridPanel>
                                            </Center>
                                        </ext:BorderLayout>
                                    </Items>
                                </ext:Panel>
                            </ext:LayoutColumn>
                            <ext:LayoutColumn ColumnWidth="0.25">
                                <ext:Panel ID="Panel1" runat="server" Title="Cadenas Comerciales">
                                    <Items>
                                        <ext:BorderLayout ID="BorderLayout3" runat="server">
                                            <Center>
                                                <ext:GridPanel ID="GridCadenaComercial" runat="server" StoreID="StoreCadenaComercial">
                                                    <SelectionModel>
                                                        <ext:CheckboxSelectionModel ID="CheckboxSelectionModel4" runat="server" RowSpan="2" SingleSelect="true" />
                                                    </SelectionModel>
                                                    <BottomBar><ext:PagingToolbar ID="PagingToolbar3" runat="server" /></BottomBar>
                                                </ext:GridPanel>
                                            </Center>
                                        </ext:BorderLayout>
                                    </Items>
                                </ext:Panel>
                            </ext:LayoutColumn>
                        </Columns>
                    </ext:ColumnLayout>
                </Items>
            </ext:Panel>
        </Center>
        <South>
            <ext:Panel runat="server" Height="200">
                <Items>
                    <ext:BorderLayout ID="BorderLayout4" runat="server">
                        <Center>
                            <ext:GridPanel ID="GridAsignaciones" runat="server" StoreID="StoreAsignaciones">
                                <BottomBar><ext:PagingToolbar ID="PagingToolbar4" runat="server" /></BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
        </South>
    </ext:BorderLayout>
</asp:Content>
