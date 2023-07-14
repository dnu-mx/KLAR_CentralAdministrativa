<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ConfiguracionCorte.aspx.cs" Inherits="Cortador.ConfiguracionCorte" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Content>
            <ext:Store ID="StorePeriodo" runat="server">
                <Reader>
                    <ext:JsonReader IDProperty="ID_Periodo">
                        <Fields>
                            <ext:RecordField Name="ID_Periodo" />
                            <ext:RecordField Name="cve_Periodo" />
                            <ext:RecordField Name="Descripcion" />
                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>
            <ext:Store ID="StoreTipoCta" runat="server">
                <Reader>
                    <ext:JsonReader IDProperty="ID_TipoCuenta">
                        <Fields>
                            <ext:RecordField Name="ID_TipoCuenta" />
                            <ext:RecordField Name="CodTipoCuentaISO" />
                            <ext:RecordField Name="ClaveTipoCuenta" />
                            <ext:RecordField Name="Descripcion" />
                            <ext:RecordField Name="GeneraDetalle" />
                            <ext:RecordField Name="GeneraCorte" />
                            <ext:RecordField Name="ID_Divisa" />
                            <ext:RecordField Name="ID_Periodo" />
                            <ext:RecordField Name="BreveDescripcion" />
                            <ext:RecordField Name="EditarSaldoGrid" />
                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>
            <ext:Store ID="StoreTipoContrato" runat="server">
                <Reader>
                    <ext:JsonReader IDProperty="ID_TipoContrato">
                        <Fields>
                            <ext:RecordField Name="ID_TipoContrato" />
                            <ext:RecordField Name="ClaveTipoContrato" />
                            <ext:RecordField Name="Descripcion" />
                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>
            <ext:Store ID="StoreEvento" runat="server">
                <Reader>
                    <ext:JsonReader IDProperty="ID_Evento">
                        <Fields>
                            <ext:RecordField Name="ID_Evento" />
                            <ext:RecordField Name="ClaveEvento" />
                            <ext:RecordField Name="Descripcion" />
                            <ext:RecordField Name="EsActivo" />
                            <ext:RecordField Name="EsReversable" />
                            <ext:RecordField Name="EsCancelable" />
                            <ext:RecordField Name="EsTransaccional" />
                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>
            <ext:Store ID="StoreTipoColectiva" runat="server">
                <Reader>
                    <ext:JsonReader IDProperty="ID_TipoColectiva">
                        <Fields>
                            <ext:RecordField Name="ID_TipoColectiva" />
                            <ext:RecordField Name="ID_TipoColectivaPadre" />
                            <ext:RecordField Name="ID_TipoColectivaHijo" />
                            <ext:RecordField Name="ClaveTipoColectiva" />
                            <ext:RecordField Name="Descripcion" />
                            <ext:RecordField Name="LongitudClave" />
                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>
            <ext:Store ID="StoreConfiguracion" runat="server">
                <Reader>
                    <ext:JsonReader IDProperty="UNO">
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
            <ext:Store ID="StoreScript" runat="server">
                <Reader>
                    <ext:JsonReader IDProperty="ID_Script">
                        <Fields>
                            <ext:RecordField Name="ID_Script" />
                            <ext:RecordField Name="descTipoCuenta" />
                            <ext:RecordField Name="descTipoColectiva" />
                            <ext:RecordField Name="EsAbono" />
                            <ext:RecordField Name="Formula" />
                            <ext:RecordField Name="ValidaSaldo" />
                            <ext:RecordField Name="EsActiva" />
                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>
        </Content>
        <Center Split="true">
            <ext:Panel runat="server" Title="Configurar Ejecuciones Periódicas">
                <Items>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <North Split="true">
                            <ext:FormPanel ID="FormPanel1" runat="server" Border="false" Height="240">
                                <Items>
                                    <ext:TextField ID="txtClave" TabIndex="3" FieldLabel="Clave" MaxLength="50" AnchorHorizontal="90%"
                                        runat="server" MsgTarget="Side" AllowBlank="false" Text="" Width="180" />
                                    <ext:TextField ID="txtNombre" TabIndex="3" FieldLabel="Nombre" MaxLength="50" AnchorHorizontal="90%"
                                        runat="server" MsgTarget="Side" AllowBlank="false" Text="" Width="180" />
                                    <ext:TextField ID="txtDescripcion" TabIndex="3" FieldLabel="Descripcion" MaxLength="50"
                                        AnchorHorizontal="90%" runat="server" MsgTarget="Side" AllowBlank="false" Text=""
                                        Width="180" />
                                    <ext:ComboBox FieldLabel="Tipo Cuenta" ID="cmbTipoCuenta1" TabIndex="1" ForceSelection="true"
                                        EmptyText="Selecciona una Opción..." runat="server" Width="180" StoreID="StoreTipoCta"
                                        MsgTarget="Side" AllowBlank="false" DisplayField="Descripcion" ValueField="ID_TipoCuenta"
                                        Editable="false" AnchorHorizontal="90%">
                                    </ext:ComboBox>
                                    <ext:ComboBox FieldLabel="Periodo" ID="cmbPeriodo" TabIndex="1" ForceSelection="true"
                                        EmptyText="Selecciona una Opción..." runat="server" Width="180" StoreID="StorePeriodo"
                                        MsgTarget="Side" AllowBlank="false" DisplayField="Descripcion" ValueField="ID_Periodo"
                                        Editable="false" AnchorHorizontal="90%">
                                    </ext:ComboBox>
                                    <ext:ComboBox FieldLabel="Tipo de Contrato" ID="cmbTipocontrato" TabIndex="1" ForceSelection="true"
                                        EmptyText="Selecciona una Opción..." runat="server" Width="180" StoreID="StoreTipoContrato"
                                        MsgTarget="Side" AllowBlank="false" DisplayField="Descripcion" ValueField="ID_TipoContrato"
                                        Editable="false" AnchorHorizontal="90%">
                                    </ext:ComboBox>
                                    <ext:ComboBox FieldLabel="Evento" ID="cmbEvento" TabIndex="1" ForceSelection="true"
                                        EmptyText="Selecciona una Opción..." runat="server" Width="180" StoreID="StoreEvento"
                                        MsgTarget="Side" AllowBlank="false" DisplayField="Descripcion" ValueField="ID_Evento"
                                        Editable="false" AnchorHorizontal="90%">
                                    </ext:ComboBox>
                                </Items>
                                <FooterBar>
                                    <ext:Toolbar ID="Toolbar2" runat="server" EnableOverflow="true">
                                        <Items>
                                            <ext:Button ID="btnGuardar" runat="server" Text="Guardar" Icon="Add">
                                                <DirectEvents>
                                                    <Click OnEvent="btnGuardarConfig_Click" Before="var valid= #{FormPanel1}.getForm().isValid(); if (!valid) {} return valid;">
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="btnCancelar" runat="server" Text="Cancelar" Icon="Cancel">
                                                <DirectEvents>
                                                    <Click OnEvent="btnCancelarConfig_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </FooterBar>
                            </ext:FormPanel>
                        </North>
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanel1" runat="server" StoreID="StoreConfiguracion" StripeRows="true"
                                RemoveViewState="true" Header="false" Border="false">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel2" runat="server">
                                </ColumnModel>
                                <DirectEvents>
                                    <RowDblClick OnEvent="GridConfig_DblClik">
                                    </RowDblClick>
                                    <Command OnEvent="EjecutarComando">
                                        <Confirmation BeforeConfirm="if (command=='Eliminar') return false;" ConfirmRequest="true"
                                            Message="¿Estas Seguro que deseas Intentar Asignar el Registro?" Title="Asingación Ficha Deposito" />
                                        <Confirmation BeforeConfirm="if (command=='Asignar') return false;" ConfirmRequest="true"
                                            Message="¿Estas Seguro que deseas Eliminar el Registro?" Title="Eliminar Ficha Deposito" />
                                        <ExtraParams>
                                            <ext:Parameter Name="id" Value="record.data.ID" Mode="Raw" />
                                        </ExtraParams>
                                    </Command>
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
                                    <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="StoreConfiguracion"
                                        DisplayInfo="true" DisplayMsg="Mostrando Empleados {0} - {1} de {2}" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
        </Center>
        <East Split="true">
            <ext:Panel runat="server" Width="600" ID="PanelScript" Collapsible="true" Collapsed="true"
                Title="Scripts de Ejecucion Periódica [NOMBRE]">
                <Items>
                    <ext:BorderLayout ID="BorderLayout3" runat="server">
                        <North Split="true">
                            <ext:FormPanel ID="FormPanel2" runat="server" Border="false" Height="180">
                                <Items>
                                    <ext:ComboBox FieldLabel="Tipo Colectiva" ID="cmbTipocolectiva" TabIndex="1" ForceSelection="true"
                                        EmptyText="Selecciona una Opción..." runat="server" Width="180" StoreID="StoreTipoColectiva"
                                        MsgTarget="Side" AllowBlank="false" DisplayField="Descripcion" ValueField="ID_TipoColectiva"
                                        Editable="false" AnchorHorizontal="90%">
                                    </ext:ComboBox>
                                    <ext:ComboBox FieldLabel="Tipo Cuenta" ID="cmbTipoCuenta2" TabIndex="1" ForceSelection="true"
                                        EmptyText="Selecciona una Opción..." runat="server" Width="180" StoreID="StoreTipoCta"
                                        MsgTarget="Side" AllowBlank="false" DisplayField="Descripcion" ValueField="ID_TipoCuenta"
                                        Editable="false" AnchorHorizontal="90%">
                                    </ext:ComboBox>
                                    <ext:ComboBox FieldLabel="Tipo Movimiento" ID="cmbTipoMovimiento" TabIndex="1" ForceSelection="true"
                                        EmptyText="Selecciona una Opción..." runat="server" Width="180" MsgTarget="Side"
                                        AllowBlank="false" Editable="false" AnchorHorizontal="90%">
                                        <Items>
                                            <ext:ListItem Text="Cargo" Value="false" />
                                            <ext:ListItem Text="Abono" Value="true" />
                                        </Items>
                                    </ext:ComboBox>
                                    <ext:ComboBox FieldLabel="Valida Saldo" ID="cmbValidaSaldo" TabIndex="1" ForceSelection="true"
                                        EmptyText="Selecciona una Opción..." runat="server" Width="180" MsgTarget="Side"
                                        AllowBlank="false" Editable="false" AnchorHorizontal="90%">
                                        <Items>
                                            <ext:ListItem Text="Si Valida Saldo" Value="true" />
                                            <ext:ListItem Text="No Valida Saldo" Value="false" />
                                        </Items>
                                    </ext:ComboBox>
                                    <ext:TextField ID="txtFormula" TabIndex="3" FieldLabel="Formula" Name="Formula" MaxLength="150"
                                        AnchorHorizontal="90%" runat="server" MsgTarget="Side" AllowBlank="false" Text=""
                                        Width="180" />
                                </Items>
                                <FooterBar>
                                    <ext:Toolbar ID="Toolbar1" runat="server" EnableOverflow="true">
                                        <Items>
                                            <ext:Button ID="Button3" runat="server" Text="Ver Parametros para Formula" Icon="StyleAdd">
                                                <DirectEvents>
                                                    <Click OnEvent="btnVer_Parametros">
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="Button1" runat="server" Text="Guardar" Icon="Add">
                                                <DirectEvents>
                                                    <Click OnEvent="btnGuardarScript_Click" Before="var valid= #{FormPanel2}.getForm().isValid(); if (!valid) {} return valid;">
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="Button2" runat="server" Text="Cancelar" Icon="Cancel">
                                                <DirectEvents>
                                                    <Click OnEvent="btnCancelarScript_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </FooterBar>
                            </ext:FormPanel>
                        </North>
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanel2" runat="server" StoreID="StoreScript" StripeRows="true"
                                RemoveViewState="true" Header="false" Border="false">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel1" runat="server">
                                </ColumnModel>
                                <DirectEvents>
                                    <Command OnEvent="EjecutarComandoScript">
                                        <Confirmation BeforeConfirm="if (command=='Eliminar') return false;" ConfirmRequest="true"
                                            Message="¿Estas Seguro que deseas Intentar Asignar el Registro?" Title="Asingación Ficha Deposito" />
                                        <Confirmation BeforeConfirm="if (command=='Asignar') return false;" ConfirmRequest="true"
                                            Message="¿Estas Seguro que deseas Eliminar el Registro?" Title="Eliminar Ficha Deposito" />
                                        <ExtraParams>
                                            <ext:Parameter Name="id" Value="record.data.ID" Mode="Raw" />
                                        </ExtraParams>
                                    </Command>
                                </DirectEvents>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                                    </ext:RowSelectionModel>
                                </SelectionModel>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreScript" DisplayInfo="true"
                                        DisplayMsg="Mostrando Empleados {0} - {1} de {2}" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
        </East>
    </ext:BorderLayout>
    <ext:Window ID="Window1" AutoScroll="true" runat="server" Icon="House" Title="Parámetros para Formula"
        Hidden="true" Width="700" Height="400" />
</asp:Content>
