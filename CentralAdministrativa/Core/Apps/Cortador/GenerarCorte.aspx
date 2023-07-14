<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="GenerarCorte.aspx.cs" Inherits="Cortador.GenerarCorte" %>

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


    </script>
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center Split="true">
            <ext:Panel ID="Panel1" runat="server" Width="428.5" Title="Miembros de la Cadena Comercial"
                Collapsed="false" Layout="Fit" AutoScroll="true">
                <Content>
                    <ext:Store ID="StoreConfiguracion" runat="server" OnRefreshData="RefreshGridCortes">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_ConfiguracionCorte">
                                <Fields>
                                    <ext:RecordField Name="ID_ConfiguracionCorte" />
                                    <ext:RecordField Name="ClaveConfiguracion" />
                                    <ext:RecordField Name="NombreConfiguracion" />
                                    <ext:RecordField Name="descConfiguracion" />
                                    <ext:RecordField Name="Estatus" />
                                    <ext:RecordField Name="DescTipoCuenta" />
                                    <ext:RecordField Name="DescPeriodo" />
                                    <ext:RecordField Name="DescEvento" />
                                    <ext:RecordField Name="descTipoContrato" />
                                    <ext:RecordField Name="ID_TipoCuenta" />
                                    <ext:RecordField Name="ID_Evento" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    
                    <ext:BorderLayout ID="BorderLayout3" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanel1" runat="server" StoreID="StoreConfiguracion" StripeRows="true"
                                RemoveViewState="true" Header="false" Border="false">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel1" runat="server">
                                </ColumnModel>
                                <DirectEvents>
                                    <RowDblClick OnEvent="GridConfig_DblClik">
                                    </RowDblClick>
                                   <%-- <Command OnEvent="EjecutarComando">
                                        <Confirmation BeforeConfirm="if (command=='Iniciar') return false;" ConfirmRequest="true"
                                            Message="¿Estas Seguro que deseas Intentar Asignar el Registro?" Title="Asingación Ficha Deposito" />
                                        <ExtraParams>
                                            <ext:Parameter Name="id" Value="record.data.ID" Mode="Raw" />
                                        </ExtraParams>
                                    </Command>--%>
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
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreConfiguracion"
                                        DisplayInfo="true" DisplayMsg="Mostrando Empleados {0} - {1} de {2}" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </Center>
        <East Split="true">
            <ext:Panel ID="Panel5" runat="server" Width="850" Title="Cuentas Posibles para Procesar"
                Collapsed="true" Collapsible="true" Layout="Fit" AutoScroll="true">
                <Content>
                <ext:Store ID="StoreCuentas" runat="server" OnRefreshData="RefreshGridCuentas">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Cuenta">
                                <Fields> 
                                    <ext:RecordField Name="ID_CuentaHabiente" />
                                    <ext:RecordField Name="ID_Evento" />
                                    <ext:RecordField Name="ID_TipoColectiva" />
                                    <ext:RecordField Name="ID_CadenaComercial" />
                                    <ext:RecordField Name="ID_TipoContrato" />
                                    <ext:RecordField Name="ID_ConfiguracionCorte" />
                                    <ext:RecordField Name="ID_Cuenta" />
                                    <ext:RecordField Name="ID_TipoCuenta" />
                                    <ext:RecordField Name="ID_EstatusConfiguracion" />
                                    <ext:RecordField Name="ID_Periodo" />
                                    <ext:RecordField Name="ClaveTipocontrato" />
                                    <ext:RecordField Name="descTipoContrato" />
                                    <ext:RecordField Name="ClaveEvento" />
                                    <ext:RecordField Name="DescEvento" />
                                    <ext:RecordField Name="DescTipoCuenta" />
                                    <ext:RecordField Name="TipoCuentaISO" />
                                    <ext:RecordField Name="CuentaHabiente" />
                                    <ext:RecordField Name="DescPeriodo" />
                                    <ext:RecordField Name="ClaveEjecucion" />
                                    <ext:RecordField Name="DescEjecucion" />
                                    <ext:RecordField Name="CadenaComercial" />
                                     <ext:RecordField Name="DiasComprendidos" />
                                    <ext:RecordField Name="DiasMes" />
                                    <ext:RecordField Name="MesesAnio" />
                                    <ext:RecordField Name="ID_Corte" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanel2" runat="server" StoreID="StoreCuentas" StripeRows="true"
                                RemoveViewState="true" Header="false" Border="false">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel2" runat="server">
                                </ColumnModel>
                                <DirectEvents>
                                    <Command OnEvent="EjecutarComandoCuenta">
                                        <Confirmation BeforeConfirm="if (command=='Iniciar') return false;" ConfirmRequest="true"
                                            Message="¿Estas Seguro que deseas Iniciar la Ejecucion del Evento a la cuenta seleccionada?" Title="Iniciar el Corte a la Cuenta Seleccionada" />
                                         <ExtraParams>
                                            <ext:Parameter Name="id" Value="record.data.ID" Mode="Raw" />
                                        </ExtraParams>
                                   </Command>
                                </DirectEvents>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel2" runat="server" SingleSelect="true">
                                    </ext:RowSelectionModel>
                                </SelectionModel>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="StoreCuentas" DisplayInfo="true"
                                        DisplayMsg="Mostrando Empleados {0} - {1} de {2}" />
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
