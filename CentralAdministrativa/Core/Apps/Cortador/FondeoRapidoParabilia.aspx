<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="FondeoRapidoParabilia.aspx.cs" Inherits="Cortador.FondeoRapidoParabilia" %>


<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var getRowClass = function (record, grid) {
            if (record.get("TipoVigencia") == 1) {
                return "verde_background_color";
            }
            else if (record.get("TipoVigencia") == 0 ) {
                return "amarillo_background_color";
            }
            else {
                return "rojo_background_color";
            }
        }
    </script>
    <style type="text/css">
        .rojo_background_color {
            background-color: lightpink;
        }
        .amarillo_background_color {
            background-color: lightgoldenrodyellow;
        }
        .verde_background_color {
            background-color: lightgreen;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Viewport ID="ViewPort1" runat="server" Layout="FitLayout">
        <Items>
            <ext:GridPanel ID="GridMovimientos" runat="server" Border="false" Height="280" Title="Autorización de Movimientos"
                AutoScroll="true">
                <Store>
                    <ext:Store ID="StoreMovimientos" runat="server" OnRefreshData="StoreMovimientos_RefreshData">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_MovimientoFondeoRapido">
                                <Fields>
                                    <ext:RecordField Name="ID_MovimientoFondeoRapido" />
                                    <ext:RecordField Name="Fecha" />
                                    <ext:RecordField Name="ID_Cuenta" />
                                    <ext:RecordField Name="ID_Colectiva" />
                                    <ext:RecordField Name="ClaveColectiva" />
                                    <ext:RecordField Name="Cuentahabiente" />
                                    <ext:RecordField Name="SaldoActual" />
                                    <ext:RecordField Name="Importe" />
                                    <ext:RecordField Name="Observaciones" />
                                    <ext:RecordField Name="UsuarioEjecutor" />
                                    <ext:RecordField Name="TipoVigencia" />
                                    <ext:RecordField Name="ClaveEventoAutoriza" />
                                    <ext:RecordField Name="ClaveEventoDeclina" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <ColumnModel ID="ColumnModel1" runat="server">
                    <Columns>
                        <ext:Column Hidden="true" DataIndex="ID_MovimientoFondeoRapido" />
                        <ext:DateColumn Header="Fecha" Sortable="true" DataIndex="Fecha" Width="150"
                            Format="dd-MM-yyyy HH:mm:ss" />
                        <ext:Column Hidden="true" DataIndex="ID_Cuenta" />
                        <ext:Column Hidden="true" DataIndex="ID_Colectiva" />
                        <ext:Column Hidden="true" DataIndex="ClaveColectiva" />
                        <ext:Column Header="Cuentahabiente" Sortable="true" DataIndex="Cuentahabiente" Width="220" />
                        <ext:Column Hidden="true" DataIndex="SaldoActual" />
                        <ext:Column Header="Importe" Sortable="true" DataIndex="Importe">
                            <Renderer Format="UsMoney" />
                        </ext:Column>
                        <ext:Column Header="Observaciones" DataIndex="Observaciones" Width="380" />
                        <ext:Column Hidden="true" DataIndex="UsuarioEjecutor" />
                        <ext:CommandColumn Header="Acción" Width="80" Hidden="true">
                            <Commands>
                                <ext:GridCommand Icon="Tick" CommandName="Autorizar">
                                    <ToolTip Text="Autorizar Movimiento" />
                                </ext:GridCommand>
                                <ext:GridCommand Icon="Cross" CommandName="Rechazar">
                                    <ToolTip Text="Rechazar Movimiento" />
                                </ext:GridCommand>
                            </Commands>
                        </ext:CommandColumn>
                        <ext:Column Hidden="true" DataIndex="TipoVigencia" />
                        <ext:Column Hidden="true" DataIndex="ClaveEventoAutoriza" />
                        <ext:Column Hidden="true" DataIndex="ClaveEventoDeclina" />
                    </Columns>
                </ColumnModel>
                 <View>
                     <ext:GridView runat="server">
                         <GetRowClass Fn="getRowClass" />
                     </ext:GridView>
                </View>
                <DirectEvents>
                    <Command OnEvent="EjecutarComando">
                        <Confirmation BeforeConfirm = "config.confirmation.message = '¿Estas seguro de  ' + command + 
                            ' el fondeo a la cuenta?';" ConfirmRequest="true" Title="Confirmación" />
                        <EventMask ShowMask="true" Msg="Procesando..." MinDelay="500" />
                        <ExtraParams>
                            <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                            <ext:Parameter Name="ValuesFR" Value="Ext.encode(record.data)" Mode="Raw" />
                        </ExtraParams>
                    </Command>
                </DirectEvents>
                <SelectionModel>
                    <ext:RowSelectionModel SingleSelect="true" />
                </SelectionModel>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreMovimientos" DisplayInfo="true"
                        DisplayMsg="Movimientos {0} - {1} de {2}" PageSize="15" />
                </BottomBar>
            </ext:GridPanel>
        </Items>
    </ext:Viewport>
</asp:Content>
