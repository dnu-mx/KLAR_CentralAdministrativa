<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="EstadosCuenta.aspx.cs" Inherits="Empresarial.EstadosCuenta" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center Split="true">
            <ext:Panel ID="Panel2" runat="server" Title="Usuarios de Mi Comercio" Collapsed="false"
                Layout="Fit" AutoScroll="true" Width="428.5">
                <Content>
                    <ext:Store ID="Store2" runat="server" OnRefreshData="RefreshGrid">
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
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </Center>
        <East Split="true">
            <ext:Panel ID="Panel3" runat="server" Title="Estados de Cuenta Disponibles" Layout="Fit" Collapsed="true"
                AutoScroll="true" Width="650.5">
                <Content>
                    <ext:Store ID="Store1" runat="server" OnRefreshData="RefreshGridCorte">
                        <Reader>
                            <ext:JsonReader >
                                <Fields>
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:BorderLayout ID="BorderLayout378" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanel2" runat="server" StoreID="Store1" StripeRows="true"
                                Header="false" Border="false"  AutoExpandColumn="NombreCorte">
                                <LoadMask ShowMask="false" />
                                <DirectEvents >
                                
                                    <Command OnEvent="EjecutarComando" IsUpload="true">
                                        <Confirmation BeforeConfirm="if (command=='ObtieneEstadoCuenta') return false;" ConfirmRequest="true"
                                            Message="¿Confirmas obtener el Estado de cuenta?" Title="Estados de Cuenta" />
                                     
                                        <%--<ExtraParams >
                                            <ext:Parameter Name="id" Value="record.data.ID" Mode="Raw" />
                                        </ExtraParams>--%>
                                    </Command>
                                </DirectEvents>

                                <ColumnModel ID="ColumnModel2" runat="server">
                                
                                </ColumnModel> 
                                
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel2" runat="server" SingleSelect="true">
                                    </ext:RowSelectionModel>
                                </SelectionModel> 
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="Store1" BufferResize="100"
                                        DisplayInfo="true" DisplayMsg="Mostrando Colectivas {0} - {1} de {2}" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </East>
    </ext:BorderLayout>
</asp:Content>
