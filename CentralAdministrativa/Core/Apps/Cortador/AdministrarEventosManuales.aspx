<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="AdministrarEventosManuales.aspx.cs" Inherits="Cortador.AdministrarEventosManuales" %>


<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var prepareToolbar = function (grid, toolbar, rowIndex, record) {
            if (record.get("Estatus") == 1) { //Activo
                toolbar.items.get(1).hide();
            } else {
                toolbar.items.get(2).hide();
            }
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Window ID="WdwEditar" runat="server" Title="Editar Evento Manual" Width="500" Height="210" Hidden="true"
        Modal="true" Resizable="false" Icon="Pencil">
        <Items>
            <ext:FormPanel ID="FormPanelEditar" runat="server" Height="180" Padding="10" LabelWidth="130" 
                Layout="FormLayout" DefaultAnchor="100%">
                <Items>
                    <ext:Hidden ID="hdnIdEvento" runat="server" />
                    <ext:TextField ID="txtEditClave" runat="server" FieldLabel="Clave" Disabled="true" />
                    <ext:TextField ID="txtTipoMovimiento" runat="server" FieldLabel="Tipo de Movimiento" Disabled="true" />
                    <ext:TextField ID="txtEditDescr" runat="server" FieldLabel="Descripción Interna" MaxLength="100"
                        MaxLengthText="100" AllowBlank="false" />
                    <ext:TextArea ID="txtEditDescrEdoCta" runat="server" FieldLabel="Descripción para Estado de Cuenta" MaxLength="150"
                        MaxLengthText="150" AllowBlank="false" Height="40" />
                </Items>
                <Buttons>
                    <ext:Button ID="btnCancelar" runat="server" Text="Cancelar" Icon="Cancel">
                        <Listeners>
                            <Click Handler="#{WdwEditar}.hide();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button ID="btnGuardar" runat="server" Text="Guardar Cambios" Icon="Disk">
                        <DirectEvents>
                            <Click OnEvent="btnGuardar_Click" Before="var valid= #{FormPanelEditar}.getForm().isValid(); if (!valid) {} return valid;">
                                <EventMask ShowMask="true" Msg="Guardando Cambios..." MinDelay="500" />
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:Viewport ID="ViewPort1" runat="server">
        <Items>
            <ext:BorderLayout ID="BorderLayout1" runat="server">
                <Center Split="true">
                    <ext:FormPanel ID="FormPanelCrear" runat="server" Border="false" LabelWidth="150" Layout="AnchorLayout" LabelAlign="Right"
                        DefaultAnchor="95%" Padding="10" Title="Nuevo Movimiento de Fondeo o Retiro" Frame="true" >
                        <Items>
                            <ext:TextField ID="txtClave" runat="server" FieldLabel="Clave" MaxLength="10" MaxLengthText="10"
                                AllowBlank="false" />
                            <ext:ComboBox ID="cBoxTipoMov" runat="server" FieldLabel="Tipo de Movimiento" AllowBlank="false">
                                <Items>
                                    <ext:ListItem Text="Fondeo" Value="1" />
                                    <ext:ListItem Text="Retiro" Value="2" />
                                </Items>
                            </ext:ComboBox>
                            <ext:TextField ID="txtDescInterna" runat="server" FieldLabel="Descripción Interna" MaxLength="100"
                                MaxLengthText="100" AllowBlank="false" />
                            <ext:TextArea ID="txtDescEdoCta" runat="server" FieldLabel="Descripción para Estado de Cuenta" MaxLength="150"
                                MaxLengthText="150" AllowBlank="false" Height="40" />
                        </Items>
                        <Buttons>
                            <ext:Button runat="server" ID="btnCrear" Text="Crear Movimiento" Icon="Add">
                                <DirectEvents>
                                    <Click OnEvent="btnCrear_Click" Before="var valid= #{FormPanelCrear}.getForm().isValid(); if (!valid) {} return valid;">
                                        <EventMask ShowMask="true" Msg="Creando Evento Manual..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                        </Buttons>
                    </ext:FormPanel>
                </Center>
                <South>
                    <ext:GridPanel ID="GridEventos" runat="server" Border="false" Height="270" Title="Movimientos Existentes">
                        <Store>
                            <ext:Store ID="StoreEventos" runat="server">
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_Evento">
                                        <Fields>
                                            <ext:RecordField Name="ID_Evento" />
                                            <ext:RecordField Name="ClaveEvento" />
                                            <ext:RecordField Name="Descripcion" />
                                            <ext:RecordField Name="DescripcionEdoCta" />
                                            <ext:RecordField Name="TipoMovimiento" />
                                            <ext:RecordField Name="Estatus" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <ColumnModel ID="ColumnModel1" runat="server">
                            <Columns>
                                <ext:Column Hidden="true" DataIndex="ID_Evento" />
                                <ext:Column Header="Clave" Sortable="true" DataIndex="ClaveEvento" Width="150" />
                                <ext:Column Header="Descripción Interna" Sortable="true" DataIndex="Descripcion" Width="300" />
                                <ext:Column Header="Descripción para Estado de Cuenta" Sortable="true" DataIndex="DescripcionEdoCta" Width="300" />
                                <ext:Column Header="Tipo de Movimiento" Sortable="true" DataIndex="TipoMovimiento" Width="200" />
                                <ext:CommandColumn ColumnID="ComandosGrid" Header="Acción" Width="60">
                                    <PrepareToolbar Fn="prepareToolbar" />
                                    <Commands>
                                        <ext:GridCommand Icon="Pencil" CommandName="Edit">
                                            <ToolTip Text="Editar" />
                                        </ext:GridCommand>
                                        <ext:GridCommand Icon="RecordRed" CommandName="Unlock">
                                            <ToolTip Text="Activar" />
                                        </ext:GridCommand>
                                        <ext:GridCommand Icon="RecordGreen" CommandName="Lock">
                                            <ToolTip Text="Desactivar" />
                                        </ext:GridCommand>
                                    </Commands>
                                </ext:CommandColumn>
                            </Columns>
                        </ColumnModel>
                        <DirectEvents>
                            <Command OnEvent="EjecutarComando">
                                <Confirmation BeforeConfirm="if (command == 'Unlock' || command == 'Edit') return false;"
                                    ConfirmRequest="true" Title="Confirmación" Message="¿Estás seguro de desactivar el Evento Manual?" />
                                <EventMask ShowMask="true" Msg="Procesando..." MinDelay="500" />
                                <ExtraParams>
                                    <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                    <ext:Parameter Name="Values" Value="Ext.encode(record.data)" Mode="Raw" />
                                    <%--<ext:Parameter Name="ID_Evento" Value="Ext.encode(record.data.ID_Evento)" Mode="Raw" />--%>
                                </ExtraParams>
                            </Command>
                        </DirectEvents>
                        <SelectionModel>
                            <ext:RowSelectionModel SingleSelect="true" />
                        </SelectionModel>
                        <BottomBar>
                            <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreEventos" DisplayInfo="true"
                                DisplayMsg="Mostrando Eventos Manuales {0} - {1} de {2}" PageSize="15" HideRefresh="true" />
                        </BottomBar>
                    </ext:GridPanel>                                            
                </South>
            </ext:BorderLayout>
        </Items>
    </ext:Viewport>
</asp:Content>
