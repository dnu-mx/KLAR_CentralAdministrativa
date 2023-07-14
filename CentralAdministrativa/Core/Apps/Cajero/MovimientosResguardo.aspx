<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="MovimientosResguardo.aspx.cs" Inherits="Cajero.MovimientosResguardo" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        var showResult = function (btn) {
            Ext.Msg.notify("Button Click", "You clicked the " + btn + " button");
        };

        var fnConfirmar = function (btn, text) {

            if (btn != "ok") {
                return;
            }
            
            if (!/^([0-9])*?[0-9]*$/.test(text)) {
                alert("La Ficha Ingresada no es un Valor Numérico Válido");
                return;
            }
            Cajero.ConfirmarOperacion(text);
        };

        var fnAsignar = function (btn) {
            if (btn == "ok") {
                Cajero.AsignarOperacion();
            }

        };
    </script>
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center Split="true">
            <ext:Panel ID="Panel2" runat="server" Width="428.5" Collapsible="true" Title="Mis Movimientos Bancarios"
                Collapsed="false" Layout="Fit" AutoScroll="true">
                <Content>
                    <ext:Store ID="Store1" runat="server" OnRefreshData="RefreshGrid">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Movimiento">
                                <Fields>
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Content>
                <Items>
                    <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store1" StripeRows="true"
                        Header="false" Border="false">
                        <LoadMask ShowMask="false" />
                        <ColumnModel ID="ColumnModel1" runat="server">
                        </ColumnModel>
                        <Listeners>
                            <Command Handler="Ext.Msg.alert(command, record.data.ID_Movimiento);"></Command>
                        </Listeners>
                        <DirectEvents>
                            <Command OnEvent="EjecutarComando">
                                <Confirmation ConfirmRequest="true" Message="¿Estas Seguro de Ejecutar la Accion Seleccionada?"
                                    Title="Confirmación de Acción" />
                                <ExtraParams>
                                    <ext:Parameter Name="id" Value="record.data.ID" Mode="Raw" />
                                </ExtraParams>
                            </Command>
                        </DirectEvents>
                        <%--                        <DirectEvents>
                            <RowDblClick OnEvent="Seleccionar">
                            </RowDblClick>
                        </DirectEvents>--%>
                        <SelectionModel>
                            <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                            </ext:RowSelectionModel>
                        </SelectionModel>
                        <BottomBar>
                            <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="Store1" DisplayInfo="true"
                                DisplayMsg="Movimientos en Resguardo {0} - {1} de {2}" />
                        </BottomBar>
                    </ext:GridPanel>
                </Items>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
