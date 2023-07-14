<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="CargoAbonoCuentaEjeCacao.aspx.cs" Inherits="Cortador.CargoAbonoCuentaEjeCacao" %>


<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Viewport ID="ViewPort1" runat="server">
        <Items>
            <ext:BorderLayout ID="BorderLayout1" runat="server">
                <Center Split="true">
                    <ext:FormPanel ID="FormPanelCaptura" runat="server" Border="false" LabelWidth="150" Layout="AnchorLayout" LabelAlign="Right"
                        DefaultAnchor="95%" Padding="10" Title="Captura de Movimiento" Frame="true" Disabled="true">
                        <Items>
                            <ext:ComboBox ID="cBoxCliente" runat="server" FieldLabel="Cliente" DisplayField="NombreCuentahabiente"
                                EmptyText="Selecciona el Cliente..." ValueField="ID_Cuenta" AllowBlank="false">
                                <Store>
                                    <ext:Store ID="StoreColectivasCuentasEje" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Cuenta">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Cuenta" />
                                                    <ext:RecordField Name="ID_Colectiva" />
                                                    <ext:RecordField Name="ClaveColectiva" />
                                                    <ext:RecordField Name="NombreCuentahabiente" />
                                                    <ext:RecordField Name="SaldoActual" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <Listeners>
                                    <Select Handler="#{cBoxTipoMov}.clear(); #{txtImporte}.clear(); #{txtObservaciones}.clear();
                                                #{hdnSaldoActual}.setValue(record.get('SaldoActual'));" />
                                </Listeners>
                            </ext:ComboBox>
                            <ext:Hidden ID="hdnSaldoActual" runat="server" />
                            <ext:Hidden ID="hdnNuevoSaldo" runat="server" />
                            <ext:ComboBox ID="cBoxTipoMov" runat="server" FieldLabel="Tipo de Movimiento" AllowBlank="false">
                                <Items>
                                    <ext:ListItem Text="Fondeo" Value="1" />
                                    <ext:ListItem Text="Retiro" Value="2" />
                                </Items>
                                <Listeners>
                                    <Select Handler="#{hdnNuevoSaldo}.clear(); #{txtImporte}.clear(); #{txtImporte}.focus(); #{txtImporte}.setDisabled(false);" />
                                </Listeners>
                            </ext:ComboBox>
                            <ext:TextField ID="txtImporte" runat="server" MaskRe="[0-9\.]" FieldLabel="Importe" AllowBlank="false" Disabled="true">
                                <Listeners>
                                    <Change Handler="var id = #{cBoxCliente}.getValue(); var saldoActual = parseFloat(#{hdnSaldoActual}.getValue());
                                        var importe = parseFloat(this.getValue()); if (#{cBoxTipoMov}.getValue() == 1) { 
                                        var ns = importe + saldoActual; } else {
                                        var ns = saldoActual - importe; } #{hdnNuevoSaldo}.setValue(ns);" />
                                </Listeners>
                            </ext:TextField>
                            <ext:TextArea ID="txtObservaciones" runat="server" FieldLabel="Observaciones" MaxLength="500" MaxLengthText="500"
                                AllowBlank="false" Height="60" />
                        </Items>
                        <Buttons>
                            <ext:Button runat="server" ID="btnGuardar" Text="Guardar Movimiento" Icon="Disk">
                                <DirectEvents>
                                    <Click OnEvent="btnGuardar_Click" Before="var valid= #{FormPanelCaptura}.getForm().isValid(); if (!valid) {} return valid;">
                                        <EventMask ShowMask="true" Msg="Guardando Movimiento..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                        </Buttons>
                    </ext:FormPanel>
                </Center>
                <South>
                    <ext:GridPanel ID="GridMovimientos" runat="server" Border="false" Height="280" Title="Autorización de Movimientos"
                        AutoExpandColumn="Observaciones">
                        <Store>
                            <ext:Store ID="StoreMovimientos" runat="server">
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_MovimientoCuentaEje">
                                        <Fields>
                                            <ext:RecordField Name="ID_MovimientoCuentaEje" />
                                            <ext:RecordField Name="Fecha" />
                                            <ext:RecordField Name="TipoMovimiento" />
                                            <ext:RecordField Name="Movimiento" />
                                            <ext:RecordField Name="ID_Cuenta" />
                                            <ext:RecordField Name="ID_Colectiva" />
                                            <ext:RecordField Name="ClienteCuenta" />
                                            <ext:RecordField Name="SaldoActual" />
                                            <ext:RecordField Name="Importe" />
                                            <ext:RecordField Name="Observaciones" />
                                            <ext:RecordField Name="Usuario" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <ColumnModel ID="ColumnModel1" runat="server">
                            <Columns>
                                <ext:Column Hidden="true" DataIndex="ID_MovimientoCuentaEje" />
                                <ext:DateColumn Header="Fecha" Sortable="true" DataIndex="Fecha" Width="120"
                                    Format="dd-MM-yyyy HH:mm:ss" />
                                <ext:Column Hidden="true" DataIndex="ID_Cuenta" />
                                <ext:Column Hidden="true" DataIndex="ID_Colectiva" />
                                <ext:Column Header="Cliente/Cuenta" Sortable="true" DataIndex="ClienteCuenta" Width="200" />
                                <ext:Column Hidden="true" DataIndex="SaldoActual" />
                                <ext:Column Hidden="true" DataIndex="TipoMovimiento" />
                                <ext:Column Header="Tipo de Movimiento" Sortable="true" DataIndex="Movimiento" Width="120" />
                                <ext:Column Header="Importe" Sortable="true" DataIndex="Importe">
                                    <Renderer Format="UsMoney" />
                                </ext:Column>
                                <ext:Column Header="Observaciones" DataIndex="Observaciones" Width="150"/>
                                <ext:Column Header="Capturó" Sortable="true" DataIndex="Usuario" Width="120" />
                                <ext:CommandColumn ColumnID="ComandosGrid" Header="Acción" Width="80" Hidden="true">
                                    <Commands>
                                        <ext:GridCommand Icon="Tick" CommandName="Aceptar">
                                            <ToolTip Text="Aceptar Movimiento" />
                                        </ext:GridCommand>
                                        <ext:GridCommand Icon="Cross" CommandName="Rechazar">
                                            <ToolTip Text="Rechazar Movimiento" />
                                        </ext:GridCommand>
                                    </Commands>
                                </ext:CommandColumn>
                            </Columns>
                        </ColumnModel>
                        <DirectEvents>
                            <Command OnEvent="EjecutarComando">
                                <Confirmation BeforeConfirm="if (command == 'Rechazar') return false;"
                                    ConfirmRequest="true" Title="Confirmación" Message="¿Autorizas el movimiento al saldo de la cuenta?" />
                                <EventMask ShowMask="true" Msg="Procesando..." MinDelay="500" />
                                <ExtraParams>
                                    <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                    <ext:Parameter Name="ID_MovimientoCuentaEje" Value="Ext.encode(record.data.ID_MovimientoCuentaEje)" Mode="Raw" />
                                    <ext:Parameter Name="TipoMovimiento" Value="Ext.encode(record.data.TipoMovimiento)" Mode="Raw" />
                                    <ext:Parameter Name="Importe" Value="Ext.encode(record.data.Importe)" Mode="Raw" />
                                    <ext:Parameter Name="Observaciones" Value="Ext.encode(record.data.Observaciones)" Mode="Raw" />
                                    <ext:Parameter Name="ID_Cuenta" Value="Ext.encode(record.data.ID_Cuenta)" Mode="Raw" />
                                    <ext:Parameter Name="ID_Colectiva" Value="Ext.encode(record.data.ID_Colectiva)" Mode="Raw" />
                                    <ext:Parameter Name="SaldoActual" Value="Ext.encode(record.data.SaldoActual)" Mode="Raw" />
                                    <ext:Parameter Name="UsuarioEjecutor" Value="Ext.encode(record.data.Usuario)" Mode="Raw" />
                                </ExtraParams>
                            </Command>
                        </DirectEvents>
                        <SelectionModel>
                            <ext:RowSelectionModel SingleSelect="true" />
                        </SelectionModel>
                        <BottomBar>
                            <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreMovimientos" DisplayInfo="true"
                                DisplayMsg="Movimientos {0} - {1} de {2}" PageSize="15" HideRefresh="true" />
                        </BottomBar>
                    </ext:GridPanel>                                            
                </South>
            </ext:BorderLayout>
        </Items>
    </ext:Viewport>
</asp:Content>
