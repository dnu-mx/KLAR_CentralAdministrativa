<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="AddMediosDePago.aspx.cs" Inherits="TpvWeb.AddMediosDePago" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">

        var prepareToolbar = function (grid, toolbar, rowIndex, record) {
            // for example hide 'Edit' button if price < 50
            if (record.get("ExisteEnOperadora") == 1) { //Activo

                //toolbar.items.get(0).hide(); //Delete
                toolbar.items.get(1).hide(); //asgina
               
            } else { //Otro no Activo
                toolbar.items.get(0).hide(); //asgina
                
            }


        };




    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center Split="true">
            <ext:Panel ID="Panel5" runat="server" Width="428.5" Title="Los Operadores" Collapsed="false"
                Layout="Fit" AutoScroll="true">
                <Content>
                    <ext:Store ID="Store2" runat="server" OnRefreshData="RefreshGrid" GroupField="ColectivaPadre">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                            </ext:JsonReader>
                        </Reader>

                    </ext:Store>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store2" StripeRows="true"
                                RemoveViewState="true" Header="false" Border="false" AutoExpandColumn="NombreORazonSocial">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel2" runat="server">
                                </ColumnModel>
                                <DirectEvents>
                                    <RowDblClick OnEvent="GridOperadores_DblClik">
                                    </RowDblClick>
                                </DirectEvents>
                                <View>
                                    <ext:GroupingView ID="GroupingView2" runat="server" ForceFit="true" MarkDirty="false"
                                        ShowGroupName="false" EnableNoGroups="true" HideGroupedColumn="true" />
                                </View>
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
                                        DisplayMsg="Mostrando los Operadores {0} - {1} de {2}" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </Center>
        <East Split="true">
            <ext:Panel ID="Panel1" runat="server" Width="650" Title="Los Medios de Pago" Collapsed="true"
                Collapsible="true" Layout="Fit" AutoScroll="true">
                <Content>
                    <ext:Store ID="storeMedioPago" runat="server" OnRefreshData="RefreshGridMediosPago" GroupField="descCuentaHabiente">
                        <Reader>
                            <ext:JsonReader IDProperty="ClaveMA">
                                <Fields>
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:BorderLayout ID="BorderLayout3" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanel2" runat="server" StoreID="storeMedioPago" StripeRows="true"
                                Header="false" Border="false" AutoExpandColumn="BreveDescripcion">
                               <DirectEvents>
                                    <Command OnEvent="EjecutarComando">
                                        <Confirmation BeforeConfirm="if (command=='Asignar') return false;" ConfirmRequest="true"
                                            Message="¿Estas Seguro que deseas Ejecutar la Accion Seleccionada al Operador?"
                                            Title="Comandos de Medios de Pago" />
                                        <Confirmation BeforeConfirm="if (command=='Desasignar') return false;" ConfirmRequest="true"
                                            Message="¿Estas Seguro que deseas Ejecutar la Accion Seleccionada al Operador?"
                                            Title="Comandos de Medios de Pago" />
                                        <ExtraParams>
                                            <ext:Parameter Name="id" Value="record.data.ID" Mode="Raw" />
                                        </ExtraParams>
                                    </Command>
                                </DirectEvents>
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel1" runat="server">
                                </ColumnModel>
                                <%--<DirectEvents>
                                    <RowDblClick OnEvent="Seleccionar">
                                    </RowDblClick>
                                </DirectEvents>--%>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" />
                                </SelectionModel>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="storeMedioPago" DisplayInfo="true"
                                        DisplayMsg="Mostrando Medios de Pago {0} - {1} de {2}" />
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
        </East>
    </ext:BorderLayout>
</asp:Content>
