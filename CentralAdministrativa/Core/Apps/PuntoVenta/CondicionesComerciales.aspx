<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" ValidateRequest="false"
    CodeBehind="CondicionesComerciales.aspx.cs" Inherits="PuntoVenta.CondicionesComerciales" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent"  runat="server">
    <ext:BorderLayout ID="BorderLayout1" runat="server" ValidateRequestMode="Disabled">
        <Center Split="true">
            <ext:Panel ID="Panel1" runat="server" Width="428.5" Title="Cadenas Comerciales" Collapsed="false"
                Layout="Fit" AutoScroll="true">
                <Content>
                    <ext:Store ID="Store2" runat="server" OnRefreshData="RefreshGridEmpleados">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                                <Fields>
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:BorderLayout ID="BorderLayout3" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanel2" runat="server" StoreID="Store2" StripeRows="true"
                                RemoveViewState="true" Header="false" Border="false" AutoExpandColumn="NombreORazonSocial">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel2" runat="server">
                                </ColumnModel>
                                <DirectEvents>
                                    <Command OnEvent="GridEmpleados_DblClik" />
                                    <RowDblClick OnEvent="GridEmpleados_DblClik" />
                                </DirectEvents>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel2" runat="server" SingleSelect="true">
                                        <DirectEvents>
                                            <RowDeselect OnEvent="QuitarSeleccion">
                                            </RowDeselect>
                                        </DirectEvents>
                                    </ext:RowSelectionModel>
                                </SelectionModel>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="Store2" DisplayInfo="true"
                                        DisplayMsg="Mostrando Cadenas Comerciales {0} - {1} de {2}" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </Center>
        <East Split="true">
            <ext:Panel ID="Panel5" runat="server" Width="550" Title="Condiciones Comerciales"
                Collapsed="true" ValidateRequestMode="Disabled" Collapsible="true" Layout="Fit" AutoScroll="true">
                <Content>
                    <ext:PropertyGrid ID="GridPropiedades" runat="server" ValidateRequestMode="Disabled" Width="700" AutoHeight="true">
                        <Source>
                            <ext:PropertyGridParameter Name="(Nombre)" Value="" />
                        </Source>
                        <View>
                            <ext:GridView ID="GridView1" ForceFit="true" ScrollOffset="2" runat="server" >
                                <Listeners>
                                    <Refresh Handler="this.grid.colModel.setColumnWidth(0, 350);" />
                                </Listeners>
                            </ext:GridView>
                        </View>
                        <Buttons>
                            <ext:Button runat="server" ID="Button1" Text="Guardar Cambios" Icon="Disk">
                                <DirectEvents>
                                    <Click OnEvent="Button1_Click" />
                                </DirectEvents>
                            </ext:Button>
                        </Buttons>
                    </ext:PropertyGrid>
                     <ext:PropertyGrid ID="GridParamsHidden" runat="server" ValidateRequestMode="Disabled" Width="700" AutoHeight="true" Hidden="true" Disabled="true">
                        <Source>
                            <ext:PropertyGridParameter Name="(name)" Value="Grid Params" />
                        </Source>
                    </ext:PropertyGrid>
                </Content>
            </ext:Panel>
        </East>
    </ext:BorderLayout>
</asp:Content>
