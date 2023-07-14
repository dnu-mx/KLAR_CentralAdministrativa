<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="SaldosEmpresa.aspx.cs" Inherits="Empresarial.Saldos" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        var updateTotal = function (grid) {
            var fbar = grid.getBottomToolbar(),
                column,
                field,
                width,
                data = {},
                c,
                cs = grid.view.getColumnData();

            for (var j = 0, jlen = grid.store.getCount(); j < jlen; j++) {
                var r = grid.store.getAt(j);

                for (var i = 0, len = cs.length; i < len; i++) {
                    c = cs[i];
                    column = grid.getColumnModel().columns[i];

                    if (column.summaryType) {
                        data[c.name] = Ext.grid.GroupSummary.Calculations[column.summaryType](data[c.name] || 0, r, c.name, data);
                    }
                }
            }

            for (var i = 0; i < grid.getColumnModel().columns.length; i++) {
                column = grid.getColumnModel().columns[i];

                if (column.dataIndex != grid.store.groupField) {
                    field = fbar.findBy(function (item) {
                        return item.dataIndex === column.dataIndex;
                    })[0];

                    c = cs[i];
                    fbar.remove(field, false);
                    fbar.insert(i, field);
                    width = grid.getColumnModel().getColumnWidth(i);
                    field.setWidth(width - 5);
                    field.setValue((column.summaryRenderer || c.renderer)(data[c.name], {}, {}, 0, i, grid.store));
                }
            }

            fbar.doLayout();
        }
    </script>
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center Split="true">
            <ext:Panel ID="Panel5" runat="server" Width="460" Title="Cuentas del comercio"
                Collapsed="false" Layout="Fit" AutoScroll="true">
                <Content>
                    <ext:Store ID="Store1" runat="server" OnRefreshData="RefreshGrid" GroupField="CuentaHabiente">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Cuenta" >
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center Split="true">
                           <%-- <ext:GridPanel ID="GridPanel1" runat="server" Frame="true" StripeRows="true" Title="Sponsored Projects"
                                AutoExpandColumn="CuentaHabiente" Collapsible="true" AnimCollapse="false" Icon="ApplicationViewColumns"
                                TrackMouseOver="false" Width="800" Height="450" StoreID="Store1" ClicksToEdit="1">
                                <ColumnModel ID="ColumnModel2" runat="server">
                                                                   </ColumnModel>

                                <View>
                                    <ext:GroupingView ID="GroupingView1" runat="server" ForceFit="true" MarkDirty="false"
                                        ShowGroupName="false" EnableNoGroups="true" HideGroupedColumn="true" />
                                </View>

                            </ext:GridPanel>--%>
                               <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store1" StripeRows="true"
                                Header="false" Border="false" AutoExpandColumn="DescripcionCuenta">
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
        </Center>
        <East Split="true">
            <ext:Panel ID="Panel1" runat="server" Width="370" Title="Cuenta Seleccionada" Collapsed="true"
                Layout="Fit" AutoScroll="true" Collapsible="true">
                <Content>
                    <ext:Panel ID="Panel7" runat="server" Title="Tipo Cuenta" AutoHeight="true" FormGroup="true">
                        <Content>
                            <table>
                                <caption></caption>
                                <tr>
                                    <td>
                                        <ext:TextField FieldLabel="Estatus" LabelAlign="Top" ID="txtEstatus" runat="server"
                                            Text="" Width="130" />
                                    </td>
                                    <td colspan="2">
                                        <ext:TextField FieldLabel="Descripción" LabelAlign="Top" ID="txtDescripcion" runat="server"
                                            Width="230" Text="" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3">
                                        <ext:TextField FieldLabel="Tipo Cuenta ISO" LabelAlign="Top" ID="txtCtaISO" runat="server"
                                            Text="" Width="360" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3">
                                        <ext:TextField FieldLabel="Grupo de Cuenta" LabelAlign="Top" ID="txtGrupoCuenta"
                                            runat="server" Width="360" Text="" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <ext:TextField FieldLabel="Tipo Colectiva" LabelAlign="Top" ID="txtTipoColectiva"
                                            runat="server" Text="" Width="130" />
                                    </td>
                                    <td colspan="2">
                                        <ext:TextField FieldLabel="Nombre CuentaHabiente" LabelAlign="Top" ID="txtCuentahabiete"
                                            runat="server" Width="230" Text="" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <ext:TextField FieldLabel="Codigo Moneda" LabelAlign="Top" ID="txtcodigoMoneda" runat="server"
                                            Width="130" Text="" />
                                    </td>
                                    <td>
                                        <ext:TextField FieldLabel="Saldo Actual" LabelAlign="Top" ID="txtSaldo" runat="server"
                                            Width="108" Text="" />
                                    </td>
                                    <td>
                                        <ext:TextField FieldLabel="Saldo Disponible" LabelAlign="Top" ID="txtSaldoDisponible"
                                            runat="server" Width="108" Text="" ClientIDMode="Inherit" />
                                    </td>
                                </tr>
                            </table>
                        </Content>
                    </ext:Panel>
                </Content>
            </ext:Panel>
        </East>
    </ext:BorderLayout>
</asp:Content>
