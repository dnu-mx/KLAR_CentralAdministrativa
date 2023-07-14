<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdministrarTipoCambio.aspx.cs" 
    Inherits="TpvWeb.AdministrarTipoCambio" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var prepareToolbar = function (grid, toolbar, rowIndex, record) {
            if (record.get("Reciente") != 1) {
                toolbar.items.get(0).hide();
                toolbar.items.get(1).hide();
            }
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Window ID="wdwEditar" runat="server" Title="Editar Tipo de Cambio" Hidden="true" Width="350" Height="170"
        Modal="true" Resizable="false" Closable="true">
        <Items>
            <ext:FormPanel ID="FormPanelEditar" runat="server" Padding="10" MonitorValid="true" LabelAlign="Right"
                LabelWidth="135">
                <Items>
                    <ext:TextField ID="txtFecha" runat="server" FieldLabel="Fecha" ReadOnly="true"
                        AnchorHorizontal="100%"/>
                    <ext:TextField ID="txtMoneda" runat="server" FieldLabel="Moneda" ReadOnly="true"
                        AnchorHorizontal="100%" />
                    <ext:TextField ID="txtTipoCambioEditar" runat="server" FieldLabel="Tipo de Cambio (Pesos)" 
                        AnchorHorizontal="100%" />
                </Items>
                <Buttons>
                    <ext:Button ID="btnGuardar" runat="server" Text="Guardar Cambios" Icon="Disk">
                        <DirectEvents>
                            <Click OnEvent="btnGuardar_Click" />
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
     <ext:Store ID="StoreMoneda" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_Divisa">
                <Fields>
                    <ext:RecordField Name="ID_Divisa" />
                    <ext:RecordField Name="ClaveMoneda" />
                    <ext:RecordField Name="Descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:BorderLayout ID="BorderLayoutOperaciones" runat="server">
        <North Split="true">
            <ext:FormPanel ID="FormPanelTipoCambio" runat="server" LabelAlign="Right">
                <TopBar>
                    <ext:Toolbar ID="ToolbarTipoCambio" runat="server">
                        <Items>
                            <ext:Hidden ID="hdnIdUSD" runat="server" />
                            <ext:Hidden ID="hdnIdTipoCambio" runat="server" />
                            <ext:DateField ID="dfFecha" runat="server" Vtype="daterange" FieldLabel="Fecha" LabelWidth="50"
                                AllowBlank="false" MsgTarget="Qtip" Format="dd/MM/yyyy" Width="200" MaxWidth="200" />
                            <ext:ComboBox ID="cBoxMoneda" runat="server" FieldLabel="Moneda" AllowBlank="false" Width="200"
                                LabelWidth="70" StoreID="StoreMoneda" ValueField="ID_Divisa" DisplayField="ClaveMoneda">
                                <DirectEvents>
                                    <Select OnEvent="SeleccionaDivisa" />
                                </DirectEvents>
                            </ext:ComboBox>
                            <ext:TextField ID="txtTipoCambio" runat="server" FieldLabel="Tipo de Cambio (Pesos)" Width="200"
                                LabelWidth="135" MaxLength="20"/>
                            <ext:Button ID="btnAgregar" runat="server" Text="Agregar" Icon="Add">
                                <DirectEvents>
                                    <Click OnEvent="btnAgregar_Click" Before="var valid= #{FormPanelTipoCambio}.getForm().isValid(); if (!valid) {} return valid;">
                                        <EventMask ShowMask="true" Msg="Agregando Tipo de Cambio..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarFill ID="dummy" runat="server" />
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
            </ext:FormPanel>
        </North>
        <Center Split="true">
            <ext:GridPanel ID="GridTiposCambio" runat="server" Header="true">
                <Store>
                    <ext:Store ID="StoreTiposCambio" runat="server">
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="ID_TipoCambioDivisa">
                                <Fields>
                                    <ext:RecordField Name="ID_TipoCambioDivisa" />
                                    <ext:RecordField Name="ID_Divisa" />
                                    <ext:RecordField Name="Fecha" />
                                    <ext:RecordField Name="Moneda" />
                                    <ext:RecordField Name="Pesos" />
                                    <ext:RecordField Name="Reciente" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <DirectEventConfig IsUpload="true" />
                    </ext:Store>
                </Store>
                <ColumnModel ID="ColumnModel2" runat="server">
                    <Columns>
                        <ext:CommandColumn Header="Acciones" Width="100">
                            <PrepareToolbar Fn="prepareToolbar" />
                            <Commands>
                                <ext:GridCommand Icon="Cross" CommandName="Delete">
                                    <ToolTip Text="Eliminar" />
                                </ext:GridCommand>
                                <ext:GridCommand Icon="PageWhiteEdit" CommandName="Edit">
                                    <ToolTip Text="Editar" />
                                </ext:GridCommand>
                            </Commands>
                        </ext:CommandColumn>
                        <ext:DateColumn ColumnID="Fecha" DataIndex="Fecha" Header="Fecha" Format="dd-MM-yyyy"
                            Width="250" Sortable="true" />
                        <ext:Column ColumnID="Moneda" DataIndex="Moneda" Header="Moneda" Width="250" />
                        <ext:Column ColumnID="Pesos" DataIndex="Pesos" Header="Tipo de Cambio" Width="250" />
                        <ext:Column ColumnID="Reciente" Hidden="true" DataIndex="Reciente" />
                    </Columns>
                </ColumnModel>
                <SelectionModel>
                    <ext:RowSelectionModel SingleSelect="true" />
                </SelectionModel>
                <DirectEvents>
                    <Command OnEvent="EjecutarComando">
                        <Confirmation BeforeConfirm="if (command == 'Edit') return false;" ConfirmRequest="true"
                             Title="Confirmación" Message="¿Estás seguro de Eliminar el Tipo de Cambio?" />
                        <ExtraParams>
                            <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                            <ext:Parameter Name="ID_TipoCambioDivisa" Value="Ext.encode(record.data.ID_TipoCambioDivisa)" Mode="Raw" />
                            <ext:Parameter Name="Fecha" Value="Ext.encode(record.data.Fecha)" Mode="Raw" />
                            <ext:Parameter Name="Moneda" Value="Ext.encode(record.data.Moneda)" Mode="Raw" />
                            <ext:Parameter Name="Pesos" Value="Ext.encode(record.data.Pesos)" Mode="Raw" />
                        </ExtraParams>
                    </Command>
                </DirectEvents>
                <Plugins>
                    <ext:GridFilters runat="server" ID="GridFilters2" Local="true">
                        <Filters>
                            <ext:StringFilter DataIndex="Fecha" />
                            <ext:StringFilter DataIndex="Moneda" />
                            <ext:StringFilter DataIndex="Pesos" />
                        </Filters>
                    </ext:GridFilters>
                </Plugins>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="StoreTiposCambio" DisplayInfo="true"
                        DisplayMsg="Mostrando Tipos de Cambio {0} - {1} de {2}" HideRefresh="true" />
                </BottomBar>
                <TopBar>
                    <ext:Toolbar ID="Toolbar1" runat="server">
                        <Items>
                            <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                            <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="true">
                                <DirectEvents>
                                    <Click OnEvent="ExportGridToExcel" IsUpload="true" Success="Ext.Net.DirectMethods.Download({IsUpload:true})">
                                        <ExtraParams>
                                            <ext:Parameter Name="GridToExport" Value="Ext.encode(#{GridTiposCambio}.getRowsValues({selectedOnly : false}))" Mode="Raw" />
                                        </ExtraParams>
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
            </ext:GridPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>