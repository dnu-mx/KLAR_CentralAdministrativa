<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="NuevoEvento.aspx.cs" Inherits="Cortador.NuevoEvento" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Window ID="WindowNuevoEvento" runat="server" Title="Evento" Hidden="true" 
        Width="600" Height="450" Modal="true" Resizable="false">
        <Items>
            <ext:FormPanel ID="FormPanelNuevoEvento" runat="server" Border="false" Layout="Fit">
                <Content>
                    <ext:Store ID="StoreTipoEvento" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_TipoEvento">
                                <Fields>
                                    <ext:RecordField Name="ID_TipoEvento" />
                                    <ext:RecordField Name="Clave" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Content>
                <Items>
                    <ext:FormPanel ID="SubNuevoEvento" runat="server" Border="false" Header="false" Width="600px"
                        LabelAlign="Left" Layout="FormLayout">
                        <Items>
                            <ext:Panel ID="Panel1" runat="server" Title="Datos Generales" AutoHeight="true" LabelAlign="Top"
                                FormGroup="true" Layout="TableLayout" Width="600px">
                                <Items>
                                    <ext:Panel ID="Panel2" runat="server" Border="false" Header="false" Width="200px"
                                        ColumnWidth=".5" Layout="Form" LabelAlign="Top">
                                        <Items>
                                            <ext:TextField ID="txtClave" runat="server" FieldLabel="Clave" MaxLength="10" AllowBlank="false"
                                                AnchorHorizontal="90%" TabIndex="1" Selectable="false" Enabled="false" Disabled="true"/>
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel ID="Panel3" runat="server" Border="false" Header="false" ColumnWidth=".5"
                                        Width="200px" Layout="Form" LabelAlign="Top">
                                        <Items>
                                            <ext:TextField ID="txtDescripcion" runat="server" FieldLabel="Descripción" AllowBlank="false"
                                                MaxLength="100" AnchorHorizontal="90%" TabIndex="2" />
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel ID="Panel4" runat="server" Border="false" Header="false" ColumnWidth=".5"
                                        Width="200px" Layout="Form" LabelAlign="Top">
                                        <Items>
                                            <ext:ComboBox ID="cBoxTipoEvento" runat="server" FieldLabel="Tipo de Evento" AllowBlank="false" 
                                                Editable="false" StoreID="StoreTipoEvento" DisplayField="Descripcion"
                                                ValueField="ID_TipoEvento" AnchorHorizontal="90%" TabIndex="3"/>
                                        </Items>
                                    </ext:Panel>
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel5" runat="server" Title="Datos Lógicos" AutoHeight="true" LabelAlign="Top"
                                FormGroup="true" Layout="TableLayout" Width="600px">
                                <Items>
                                    <ext:Panel ID="Panel6" runat="server" Border="false" Header="false" Width="200px"
                                        ColumnWidth=".5" Layout="Form" LabelAlign="Top">
                                        <Items>
                                            <ext:SelectBox ID="sBoxActivo" runat="server" FieldLabel="Activo" AllowBlank="false"
                                                AnchorHorizontal="90%" TabIndex="1">
                                                <Items>
                                                    <ext:ListItem Text="Sí" Value="1" />
                                                    <ext:ListItem Text="No" Value="0" />
                                                </Items>
                                            </ext:SelectBox>
                                            <ext:SelectBox ID="sBoxTransaccional" runat="server" FieldLabel="Transaccional" AllowBlank="false"
                                                AnchorHorizontal="90%" TabIndex="4">
                                                <Items>
                                                    <ext:ListItem Text="Sí" Value="1" />
                                                    <ext:ListItem Text="No" Value="0" />
                                                </Items>
                                            </ext:SelectBox>
                                            <ext:SelectBox ID="sBoxPostValidaciones" runat="server" FieldLabel="PostValidaciones" AllowBlank="false"
                                                AnchorHorizontal="90%" TabIndex="7">
                                                <Items>
                                                    <ext:ListItem Text="Sí" Value="1" />
                                                    <ext:ListItem Text="No" Value="0" />
                                                </Items>
                                            </ext:SelectBox>
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel ID="Panel7" runat="server" Border="false" Header="false" ColumnWidth=".5"
                                        Width="200px" Layout="Form" LabelAlign="Top">
                                        <Items>
                                            <ext:SelectBox ID="sBoxReversable" runat="server" FieldLabel="Reversable" AllowBlank="false"
                                                AnchorHorizontal="90%" TabIndex="2">
                                                <Items>
                                                    <ext:ListItem Text="Sí" Value="1" />
                                                    <ext:ListItem Text="No" Value="0" />
                                                </Items>
                                            </ext:SelectBox>
                                            <ext:SelectBox ID="sBoxPoliza" runat="server" FieldLabel="Genera Póliza" AllowBlank="false"
                                                AnchorHorizontal="90%" TabIndex="5">
                                                <Items>
                                                    <ext:ListItem Text="Sí" Value="1" />
                                                    <ext:ListItem Text="No" Value="0" />
                                                </Items>
                                            </ext:SelectBox>
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel ID="Panel8" runat="server" Border="false" Header="false" ColumnWidth=".5"
                                        Width="200px" Layout="Form" LabelAlign="Top">
                                        <Items>
                                            <ext:SelectBox ID="sBoxCancelable" runat="server" FieldLabel="Cancelable" AllowBlank="false"
                                                AnchorHorizontal="90%" TabIndex="3">
                                                <Items>
                                                    <ext:ListItem Text="Sí" Value="1" />
                                                    <ext:ListItem Text="No" Value="0" />
                                                </Items>
                                            </ext:SelectBox>
                                            <ext:SelectBox ID="sBoxPreValidaciones" runat="server" FieldLabel="PreValidaciones" AllowBlank="false"
                                                AnchorHorizontal="90%" TabIndex="6">
                                                <Items>
                                                    <ext:ListItem Text="Sí" Value="1" />
                                                    <ext:ListItem Text="No" Value="0" />
                                                </Items>
                                            </ext:SelectBox>
                                        </Items>
                                    </ext:Panel>
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel9" runat="server" Title="Estado de Cuenta" AutoHeight="true" LabelAlign="Top"
                                FormGroup="true" Layout="TableLayout" Width="600px">
                                <Items>
                                    <ext:Panel ID="Panel10" runat="server" Border="false" Header="false" Width="580px"
                                        ColumnWidth=".5" Layout="Form" LabelAlign="Top">
                                        <Items>
                                            <ext:TextArea ID="txtDescripcionEdoCta" runat="server" FieldLabel="Descripción"
                                                BoxLabel="CheckBox" Height="95" AllowBlank="false" MaxLength="150"
                                                AnchorHorizontal="95%" TabIndex="1" />
                                        </Items>
                                    </ext:Panel>
                                </Items>
                            </ext:Panel>
                        </Items>
                    </ext:FormPanel>
                </Items>
            </ext:FormPanel>
        </Items>
        <FooterBar>
            <ext:Toolbar ID="ToolbarNuevoEvento" runat="server" EnableOverflow="true">
                <Items>
                    <ext:Button ID="btnGuardar" TabIndex="9" runat="server" Text="Guardar" Icon="Disk">
                        <DirectEvents>
                            <Click OnEvent="btnGuardar_Click" Before="var valid= #{SubNuevoEvento}.getForm().isValid(); if (!valid) {} return valid;" />
                        </DirectEvents>
                    </ext:Button>
                </Items>
            </ext:Toolbar>
        </FooterBar>
    </ext:Window>

    <ext:Viewport ID="ViewPort1" runat="server">
        <Items>
            <ext:BorderLayout ID="MainBorderLayout" runat="server">
                <Content>
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
                </Content>
                <Center Split="true">
                    <ext:FormPanel ID="FormPanelConsultaEventos" runat="server" Title="Consulta de Eventos" Layout="FitLayout">
                        <Items>
                            <ext:GridPanel ID="GridPanelConsulta" runat="server" Layout="FitLayout" StripeRows="true"
                                Header="false" Border="false">
                                <Store>
                                    <ext:Store ID="StoreConsulta" runat="server">
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
                                                    <ext:RecordField Name="ID_TipoEvento" />
                                                    <ext:RecordField Name="DescripcionEdoCta" />
                                                    <ext:RecordField Name="GeneraPoliza" />
                                                    <ext:RecordField Name="PreValidaciones" />
                                                    <ext:RecordField Name="PostValidaciones" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <TopBar>
                                    <ext:Toolbar ID="ToolbarConsulta" runat="server">
                                        <Items>
                                            <ext:TextField ID="txtClaveEvento" EmptyText="Clave" Width="80" runat="server" />
                                            <ext:TextField ID="txtDescrEvento" EmptyText="Descripción" Width="200" runat="server" />
                                            <ext:Button ID="btnBuscarEvento" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnBuscarEvento_Click" Before="var valid= #{FormPanelConsultaEventos}.getForm().isValid(); if (!valid) {} return valid;" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:ToolbarFill ID="dummy" runat="server" />
                                            <ext:Button ID="btnNuevoEvento" runat="server" Text="Nuevo Evento" Icon="Add">
                                                <DirectEvents>
                                                    <Click OnEvent="btnNuevoEvento_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <ColumnModel ID="ColumnModel8" runat="server">
                                    <Columns>
                                        <ext:CommandColumn Width="40">
                                            <Commands>
                                                <ext:GridCommand Icon="NoteEdit" CommandName="Edit">
                                                    <ToolTip Text="Editar" />
                                                </ext:GridCommand>
                                            </Commands>
                                        </ext:CommandColumn>
                                        <ext:Column DataIndex="ID_Evento" Hidden="true" />
                                        <ext:Column DataIndex="ClaveEvento" Header="Clave" />
                                        <ext:Column DataIndex="Descripcion" Header="Descripción" Width="300" />
                                        <ext:Column DataIndex="EsActivo" Hidden="true" />
                                        <ext:Column DataIndex="EsReversable" Hidden="true" />
                                        <ext:Column DataIndex="EsCancelable" Hidden="true" />
                                        <ext:Column DataIndex="EsTransaccional" Hidden="true" />
                                        <ext:Column DataIndex="ID_TipoEvento" Hidden="true" />
                                        <ext:Column DataIndex="DescripcionEdoCta" Hidden="true" />
                                        <ext:Column DataIndex="GeneraPoliza" Hidden="true" />
                                        <ext:Column DataIndex="PreValidaciones" Hidden="true" />
                                        <ext:Column DataIndex="PostValidaciones" Hidden="true" />
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel SingleSelect="true" />
                                </SelectionModel>
                                <DirectEvents>
                                    <RowClick OnEvent="selectEvento_Event">
                                        <ExtraParams>
                                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridPanelConsulta}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                        </ExtraParams>
                                    </RowClick>
                                    <Command OnEvent="EditarEvento">
                                        <ExtraParams>
                                            <ext:Parameter Name="Values" Value="Ext.encode(record.data)" Mode="Raw" />
                                        </ExtraParams>
                                    </Command>
                                </DirectEvents>
                            </ext:GridPanel>
                        </Items>
                    </ext:FormPanel>
                </Center>
                <South Split="true">
                    <ext:Panel ID="PanelTabs" runat="server" Height="350" Collapsible="true">
                        <Items>
                            <ext:BorderLayout ID="BorderLayout1" runat="server">
                                <Center>
                                    <ext:TabPanel ID="TabPanel1" runat="server">
                                        <Items>
                                            <ext:FormPanel ID="FormPanelReglas" runat="server" Title="Reglas" Layout="FitLayout" StripeRows="true"
                                                Header="false" Border="false">
                                                <Items>
                                                    <ext:GridPanel ID="GridReglas" runat="server">
                                                        <Store>
                                                            <ext:Store ID="StoreReglas" runat="server">
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="ID_Regla">
                                                                        <Fields>
                                                                            <ext:RecordField Name="ID_Evento" />
                                                                            <ext:RecordField Name="ID_Regla" />
                                                                            <ext:RecordField Name="Nombre" />
                                                                            <ext:RecordField Name="Activa" />
                                                                        </Fields>
                                                                    </ext:JsonReader>
                                                                </Reader>
                                                            </ext:Store>
                                                        </Store>
                                                        <ColumnModel ID="ColumnModel6" runat="server">
                                                            <Columns>
                                                                <ext:Column DataIndex="ID_Evento" Hidden="true" />
                                                                <ext:Column DataIndex="ID_Regla" Hidden="true" />
                                                                <ext:Column DataIndex="Nombre" Header="Nombre" Width="280" />
                                                            </Columns>
                                                        </ColumnModel>
                                                        <SelectionModel>
                                                            <ext:CheckboxSelectionModel ID="chkSM_Reglas" runat="server" />
                                                        </SelectionModel>
                                                        <Buttons>
                                                            <ext:Button ID="btnActivaReglas" runat="server" Text="Activar Reglas" Icon="Add">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnActivaReglas_Click">
                                                                        <ExtraParams>
                                                                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridReglas}.getRowsValues())" Mode="Raw" />
                                                                        </ExtraParams>
                                                                    </Click>
                                                                </DirectEvents>
                                                            </ext:Button>
                                                        </Buttons>
                                                    </ext:GridPanel>
                                                </Items>
                                            </ext:FormPanel>
                                            <ext:FormPanel ID="FormPanelPoliza" runat="server" Title="Póliza">
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
                                                                            <ext:Button ID="Button3" runat="server" Text="Ver Parámetros para Fórmula" Icon="StyleAdd">
                                                                                <DirectEvents>
                                                                                    <Click OnEvent="btnVer_Parametros" />
                                                                                </DirectEvents>
                                                                            </ext:Button>
                                                                            <ext:Button ID="Button1" runat="server" Text="Guardar" Icon="Add">
                                                                                <DirectEvents>
                                                                                    <Click OnEvent="btnGuardarScript_Click" Before="var valid= #{FormPanel2}.getForm().isValid(); if (!valid) {} return valid;" />
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
                                                                            Message="¿Estas Seguro que deseas Intentar Asignar el Registro?" Title="Asingación Póliza" />
                                                                        <Confirmation BeforeConfirm="if (command=='Asignar') return false;" ConfirmRequest="true"
                                                                            Message="¿Estas Seguro que deseas Eliminar el Registro?" Title="Eliminar Póliza" />
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
                                            </ext:FormPanel>
                                            <ext:FormPanel ID="FormPanelPlugins" runat="server" Title="PlugIns Validación" Layout="FitLayout" StripeRows="true"
                                                Header="false" Border="false">
                                                <Items>
                                                    <ext:GridPanel ID="GridPlugins" runat="server">
                                                        <Store>
                                                            <ext:Store ID="StorePlugins" runat="server">
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="ID_Plugin">
                                                                        <Fields>
                                                                            <ext:RecordField Name="ID_Evento" />
                                                                            <ext:RecordField Name="ID_Plugin" />
                                                                            <ext:RecordField Name="NombrePlugin" />
                                                                            <ext:RecordField Name="Activo" />
                                                                            <ext:RecordField Name="OrdenEjecucion" />
                                                                            <ext:RecordField Name="EsRespuestaISO" />
                                                                            <ext:RecordField Name="EsObligatorioParaReverso" />
                                                                        </Fields>
                                                                    </ext:JsonReader>
                                                                </Reader>
                                                            </ext:Store>
                                                        </Store>
                                                        <ColumnModel ID="ColumnModel2" runat="server">
                                                            <Columns>
                                                                <ext:Column DataIndex="ID_Evento" Hidden="true" />
                                                                <ext:Column DataIndex="ID_Plugin" Hidden="true" />
                                                                <ext:Column DataIndex="NombrePlugin" Header="Nombre" Width="300" />
                                                                <ext:Column DataIndex="OrdenEjecucion" Header="Orden de Ejecución" Width="150">
                                                                    <Editor>
                                                                        <ext:ComboBox ID="ComboBox1" runat="server" Editable="false">
                                                                            <Items>
                                                                                <ext:ListItem Text="1" Value="1" />
                                                                                <ext:ListItem Text="2" Value="2" />
                                                                                <ext:ListItem Text="3" Value="3" />
                                                                                <ext:ListItem Text="4" Value="4" />
                                                                                <ext:ListItem Text="5" Value="5" />
                                                                            </Items>
                                                                        </ext:ComboBox>
                                                                    </Editor>
                                                                </ext:Column>
                                                                <ext:Column DataIndex="EsRespuestaISO" Header="Es Respuesta ISO" Width="150">
                                                                    <Editor>
                                                                        <ext:SelectBox ID="sBox1" runat="server" Editable="false">
                                                                            <Items>
                                                                                <ext:ListItem Text="Sí" Value="true" />
                                                                                <ext:ListItem Text="No" Value="false" />
                                                                            </Items>
                                                                        </ext:SelectBox>
                                                                    </Editor>
                                                                </ext:Column>
                                                                <ext:Column DataIndex="EsObligatorioParaReverso" Header="Es Obligatorio para Reverso" Width="200">
                                                                    <Editor>
                                                                        <ext:SelectBox ID="sBox2" runat="server" Editable="false">
                                                                            <Items>
                                                                                <ext:ListItem Text="Sí" Value="true" />
                                                                                <ext:ListItem Text="No" Value="false" />
                                                                            </Items>
                                                                        </ext:SelectBox>
                                                                    </Editor>
                                                                </ext:Column>
                                                            </Columns>
                                                        </ColumnModel>
                                                        <SelectionModel>
                                                            <ext:CheckboxSelectionModel ID="chkSM_Plugins" runat="server" CheckOnly="True" />
                                                        </SelectionModel>
                                                        <Buttons>
                                                            <ext:Button ID="btnActivaPlugins" runat="server" Text="Activar PlugIns" Icon="Add">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnActivaPlugins_Click">
                                                                        <ExtraParams>
                                                                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridPlugins}.getRowsValues())" Mode="Raw" />
                                                                        </ExtraParams>
                                                                    </Click>
                                                                </DirectEvents>
                                                            </ext:Button>
                                                        </Buttons>
                                                    </ext:GridPanel>
                                                </Items>
                                            </ext:FormPanel>
                                            <ext:FormPanel ID="FormPanelAutorizador" runat="server" Title="Autorizador Externa" Layout="FitLayout" StripeRows="true"
                                                Header="false" Border="false">
                                                <Items>
                                                    <ext:GridPanel ID="GridAutorizador" runat="server">
                                                        <Store>
                                                            <ext:Store ID="StoreAutorizador" runat="server">
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="ID_Plugin">
                                                                        <Fields>
                                                                            <ext:RecordField Name="ID_Evento" />
                                                                            <ext:RecordField Name="ID_Plugin" />
                                                                            <ext:RecordField Name="NombrePlugin" />
                                                                            <ext:RecordField Name="Activo" />
                                                                            <ext:RecordField Name="OrdenEjecucion" />
                                                                            <ext:RecordField Name="EsRespuestaISO" />
                                                                            <ext:RecordField Name="EsObligatorioParaReverso" />
                                                                        </Fields>
                                                                    </ext:JsonReader>
                                                                </Reader>
                                                            </ext:Store>
                                                        </Store>
                                                        <ColumnModel ID="ColumnModel3" runat="server">
                                                            <Columns>
                                                                <ext:Column DataIndex="ID_Evento" Hidden="true" />
                                                                <ext:Column DataIndex="ID_Plugin" Hidden="true" />
                                                                <ext:Column DataIndex="NombrePlugin" Header="Nombre" Width="300" />
                                                                <ext:Column DataIndex="OrdenEjecucion" Header="Orden de Ejecución" Width="150">
                                                                    <Editor>
                                                                        <ext:ComboBox ID="ComboBox2" runat="server" Editable="false">
                                                                            <Items>
                                                                                <ext:ListItem Text="1" Value="1" />
                                                                                <ext:ListItem Text="2" Value="2" />
                                                                                <ext:ListItem Text="3" Value="3" />
                                                                                <ext:ListItem Text="4" Value="4" />
                                                                                <ext:ListItem Text="5" Value="5" />
                                                                            </Items>
                                                                        </ext:ComboBox>
                                                                    </Editor>
                                                                </ext:Column>
                                                                <ext:Column DataIndex="EsRespuestaISO" Header="Es Respuesta ISO" Width="150">
                                                                    <Editor>
                                                                        <ext:SelectBox ID="Selectbox1" runat="server" Editable="false">
                                                                            <Items>
                                                                                <ext:ListItem Text="Sí" Value="true" />
                                                                                <ext:ListItem Text="No" Value="false" />
                                                                            </Items>
                                                                        </ext:SelectBox>
                                                                    </Editor>
                                                                </ext:Column>
                                                                <ext:Column DataIndex="EsObligatorioParaReverso" Header="Es Obligatorio para Reverso" Width="200">
                                                                    <Editor>
                                                                        <ext:SelectBox ID="Selectbox2" runat="server" Editable="false">
                                                                            <Items>
                                                                                <ext:ListItem Text="Sí" Value="true" />
                                                                                <ext:ListItem Text="No" Value="false" />
                                                                            </Items>
                                                                        </ext:SelectBox>
                                                                    </Editor>
                                                                </ext:Column>
                                                            </Columns>
                                                        </ColumnModel>
                                                        <SelectionModel>
                                                            <ext:CheckboxSelectionModel ID="chkSM_Autorizador" runat="server" />
                                                        </SelectionModel>
                                                        <Buttons>
                                                            <ext:Button ID="btnActivaPlugins2" runat="server" Text="Activar PlugIns" Icon="Add">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnActivaPlugins2_Click">
                                                                        <ExtraParams>
                                                                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridAutorizador}.getRowsValues())" Mode="Raw" />
                                                                        </ExtraParams>
                                                                    </Click>
                                                                </DirectEvents>
                                                            </ext:Button>
                                                        </Buttons>
                                                    </ext:GridPanel>
                                                </Items>
                                            </ext:FormPanel>
                                        </Items>
                                    </ext:TabPanel>
                                </Center>
                            </ext:BorderLayout>
                        </Items>
                    </ext:Panel>
                </South>
            </ext:BorderLayout>
        </Items>
    </ext:Viewport>

    <ext:Window ID="Window1" AutoScroll="true" runat="server" Icon="House" Title="Parámetros para Formula"
        Hidden="true" Width="700" Height="400" />
</asp:Content>

