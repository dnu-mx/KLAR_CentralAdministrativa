<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="SaldosEmpleado.aspx.cs" Inherits="Empresarial.SaldosEmpleado" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">

        var template = '<span style="color:{0};">{1}</span>';

        var change = function (value) {
            return String.format(template, (value > 0) ? "green" : "red", value);
        };

        var pctChange = function (value) {
            return String.format(template, (value > 0) ? "green" : "red", value + "%");
        };

        var startEditing = function (e) {
            if (e.getKey() == e.ENTER) {
                var grid = GridPanel1,
                    record = grid.getSelectionModel().getSelected(),
                    index = grid.store.indexOf(record);

                grid.startEditing(index, 1);
            }
        };

        var Prueba = function (o, e) {
            if (e.getKey() == e.ENTER) {
                var grid = GridPanel1,
                    record = grid.getSelectionModel().getSelected(),
                    index = grid.store.indexOf(record);

                grid.startEditing(index, 1);
            }
        };


        function validaFloat(numero) {
            if (!/^([0-9])*[.]?[0-9]*$/.test(numero))
                alert("El valor " + numero + " no es un número");
        }

        var prepareToolbar = function (grid, toolbar, rowIndex, record) {
            // for example hide 'Edit' button if price < 50

            if (record.get("EditarSaldoGrid") == true) { //ACTIVO
                toolbar.items.get(1).hide(); //sep
            } else if (record.get("EditarSaldoGrid") == false) { //Asignado
                toolbar.items.get(0).hide(); //Delete
                //grid.colModel.columns[7].editor = null;
            }

        };

        var afterEdit = function (e) {
            /*
            Properties of 'e' include:
            e.grid - This grid
            e.record - The record being edited
            e.field - The field name being edited
            e.value - The value being set
            e.originalValue - The original value for the field, before the edit.
            e.row - The grid row index
            e.column - The grid column index
            */


            if (!/^([0-9])*[.]?[0-9]*$/.test(e.record.data.LimiteCredito)) {
                alert("El valor ingresado no es un Saldo válido");
                Empresarial.ActualizaGridSaldos();
                return;
            }

            if (!e.record.data.EditarSaldoGrid) {
                e.record.data.LimiteCredito = 0;
                alert("El Tipo de Cuenta no Permite Edición en Grid");
                Empresarial.ActualizaGridSaldos();
                return;
            }

            // Call DirectMethod
            Empresarial.AfterEdit(e.record.data.ID_Cuenta, e.record.data.SaldoDisponible, e.record.data.ID_Colectiva, e.record.data.CodTipoCuentaISO, e.record.data.CodigoMoneda, e.record.data.EditarSaldoGrid, e.record.data.Afiliacion);
        };

    </script>
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center Split="true">
            <ext:Panel ID="Panel1" runat="server" Width="428.5" Title="Empleados"
                Collapsed="false" Layout="Fit" AutoScroll="true">
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
                                    <RowDblClick OnEvent="GridEmpleados_DblClik">
                                    </RowDblClick>
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
                                        DisplayMsg="Mostrando Empleados {0} - {1} de {2}" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </Center>
        <East Split="true">
            <ext:Panel ID="Panel5" runat="server" Width="850" Title="Cuentas del Grupo Comercial"
                Collapsed="true" Collapsible="true" Layout="Fit" AutoScroll="true">
                <Content>
                    <ext:Store ID="Store1" runat="server" OnRefreshData="RefreshGridSaldos" GroupField="CuentaHabiente">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Cuenta">
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store1" StripeRows="true"
                                Header="false" Border="false" AutoExpandColumn="BreveDescripcion">
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar1" runat="server" EnableOverflow="true">
                                        <Items>
                                            <ext:Button ID="btnCrearCta" runat="server" Text="Crear Cuentas" ToolTip="Crear Cuentas a Colectiva Seleccionada"
                                                Icon="MoneyAdd">
                                                <DirectEvents>
                                                    <Click OnEvent="btnCrearCta_Click">
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <Listeners>
                                    <AfterEdit Fn="afterEdit" />
                                </Listeners>
                                <Plugins>
                                    <ext:RowEditor ID="RowEditor1" runat="server" SaveText="Actualizar" CancelText="Cancelar">
                                        <Listeners>
                                            <AfterEdit Fn="afterEdit" />
                                        </Listeners>
                                    </ext:RowEditor>
                                </Plugins>
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
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="Store1" DisplayInfo="true"
                                        DisplayMsg="Mostrando Saldos {0} - {1} de {2}" />
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
