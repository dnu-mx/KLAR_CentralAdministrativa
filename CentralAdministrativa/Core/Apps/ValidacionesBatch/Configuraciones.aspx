<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
     CodeBehind="Configuraciones.aspx.cs" Inherits="ValidacionesBatch.Configuraciones" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Store ID="StoreReglas" runat="server" OnRefreshData="StoreReglas_Refresh">
        <Reader>
            <ext:JsonReader IDProperty="ID_Regla">
                <Fields>
                    <ext:RecordField Name="ID_Regla" />
                    <ext:RecordField Name="Nombre" />
                    <ext:RecordField Name="StoreProcedure" />
                    <ext:RecordField Name="OrdenEjecucion" />
                    <ext:RecordField Name="EsActiva" />
                    <ext:RecordField Name="EsAccion" />
                    <ext:RecordField Name="DescripcionRegla" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center>
            <ext:TabPanel ID="TabConfig" runat="server">
                <Items>
                    <ext:GridPanel ID="GridReglas" runat="server" StoreID="StoreReglas">
                        <SelectionModel>
                            <ext:RowSelectionModel SingleSelect="true" />
                        </SelectionModel>
                        <DirectEvents>
                            <Command OnEvent="Click_ConfigurarRegla" />
                        </DirectEvents>
                        <BottomBar>
                            <ext:PagingToolbar ID="PagingToolbar1" runat="server" />
                        </BottomBar>
                    </ext:GridPanel>

                    <ext:Panel ID="PanelConfig" runat="server">
                        <Items>
                            <ext:RowLayout ID="RowLayout1" runat="server">
                                <Rows>
                                    <ext:LayoutRow>
                                        <ext:Panel runat="server">
                                            <Items>
                                                <ext:ColumnLayout ID="DatosRegla" runat="server" FitHeight="true">
                                                    <Columns>
                                                        <ext:LayoutColumn>
                                                            <ext:Image runat="server" ImageUrl="Images/promo.png" />
                                                        </ext:LayoutColumn>

                                                        <ext:LayoutColumn ColumnWidth="0.75">
                                                            <ext:FormPanel ID="FormDatos" runat="server" Border="false">
                                                                <Items>
                                                                    <ext:TextField ID="TxtNombreRegla" runat="server" />
                                                                    <ext:TextField ID="TxtDescripcion" runat="server" />
                                                                    <ext:SelectBox ID="SelectCadena" runat="server" />
                                                                </Items>
                                                            </ext:FormPanel>
                                                        </ext:LayoutColumn>
                                                    </Columns>
                                                </ext:ColumnLayout>
                                            </Items>
                                        </ext:Panel>
                                    </ext:LayoutRow>

                                    <ext:LayoutRow RowHeight="1">
                                        <ext:Panel ID="DatosConfig" runat="server">
                                            <Items>
                                                <ext:TabPanel runat="server">
                                                    <Items>
                                                        <ext:Panel ID="PanelRegla" runat="server">
                                                            <Items>
                                                                <ext:GridPanel ID="GridConfigReglas" StoreID="StoreReglas" runat="server" >
                                                                </ext:GridPanel>
                                                            </Items>
                                                        </ext:Panel>
                                                        <ext:Panel ID="PanelTipoCuenta" runat="server" >
                                                            <Items>
                                                                <ext:GridPanel ID="GridConfigTipocuenta" StoreID="StoreReglas" runat="server" >
                                                                </ext:GridPanel>
                                                            </Items>
                                                        </ext:Panel>
                                                        <ext:Panel ID="PanelGrupoCuenta" runat="server" >
                                                            <Items>
                                                                <ext:GridPanel ID="GridConfigGrupoCuenta" StoreID="StoreReglas" runat="server" >
                                                                </ext:GridPanel>
                                                            </Items>
                                                        </ext:Panel>
                                                        <ext:Panel ID="PanelCuenta" runat="server" >
                                                            <Items>
                                                                <ext:GridPanel ID="GridConfigCuenta" StoreID="StoreReglas" runat="server" >
                                                                </ext:GridPanel>
                                                            </Items>
                                                        </ext:Panel>
                                                        <ext:Panel ID="PanelGrupoTarjeta" runat="server" >
                                                            <Items>
                                                                <ext:GridPanel ID="GridConfigGrupoTarjeta" StoreID="StoreReglas" runat="server" >
                                                                </ext:GridPanel>
                                                            </Items>
                                                        </ext:Panel>
                                                        <ext:Panel ID="PanelTarjeta" runat="server" >
                                                            <Items>
                                                                <ext:GridPanel ID="GridConfigTarjeta" StoreID="StoreReglas" runat="server" >
                                                                </ext:GridPanel>
                                                            </Items>
                                                        </ext:Panel>
                                                    </Items>
                                                </ext:TabPanel>
                                            </Items>
                                        </ext:Panel>
                                    </ext:LayoutRow>
                                </Rows>
                            </ext:RowLayout>
                        </Items>
                    </ext:Panel>
                </Items>
            </ext:TabPanel>
        </Center>
    </ext:BorderLayout>

</asp:Content>
