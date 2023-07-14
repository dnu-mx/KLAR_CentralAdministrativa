<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="AdministrarReportes.aspx.cs" Inherits="TpvWeb.AdministrarReportes" %>


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
    <ext:Store ID="StoreTiposGenRep" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_TipoGeneracionReporte">
                <Fields>
                    <ext:RecordField Name="ID_TipoGeneracionReporte" />
                    <ext:RecordField Name="Clave" />
                    <ext:RecordField Name="Descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Window ID="WdwEditar" runat="server" Title="Editar Reporte" Width="500" AutoHeight="true" Hidden="true" Modal="true"
        Resizable="false" Icon="Pencil">
        <Items>
            <ext:FormPanel ID="FormPanelEditar" runat="server" Padding="10" LabelAlign="Right" LabelWidth="190" Layout="FormLayout" AutoHeight="true">
                <Items>
                    <ext:Hidden ID="hdnIdReporte" runat="server" />
                    <ext:TextField ID="txtEditClave" runat="server" FieldLabel="Clave" Disabled="true" Width="260" />
                    <ext:ComboBox ID="cBoxEditTipoGenRep" runat="server" FieldLabel="Tipo de Generación del Reporte <span style='color:red;'>*</span> "
                        Width="260" AllowBlank="false" StoreID="StoreTiposGenRep" DisplayField="Descripcion" ValueField="ID_TipoGeneracionReporte" />
                    <ext:TextField ID="txtEditNombre" runat="server" FieldLabel="Nombre <span style='color:red;'>*</span> " MaxLength="100"
                        Width="260" MaxLengthText="100" AllowBlank="false" />
                    <ext:TextField ID="txtEditSp" runat="server" FieldLabel="Procedimiento Almacenado <span style='color:red;'>*</span> "
                        Width="260" MaxLength="150" MaxLengthText="150" AllowBlank="false" />
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
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center Split="true">
            <ext:Panel runat="server" Title="Registro de Alta de Reportes" Border="false">
                <Items>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <North Split="true">
                            <ext:FormPanel ID="FormPanelCrear" runat="server" Height="135" Padding="10" LabelWidth="220" Border="false"
                                LabelAlign="Right" Frame="true">
                                <Items>
                                    <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false">
                                        <Defaults>
                                            <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                        </Defaults>
                                        <LayoutConfig>
                                            <ext:HBoxLayoutConfig Align="Top" />
                                        </LayoutConfig>
                                        <Items>
                                            <ext:ComboBox ID="cBoxTipoGenRep" runat="server" FieldLabel="Tipo de Generación del Reporte <span style='color:red;'>*</span> "
                                                Width="420" AllowBlank="false" StoreID="StoreTiposGenRep" DisplayField="Descripcion" ValueField="ID_TipoGeneracionReporte" />
                                            <ext:TextField ID="txtClave" runat="server" FieldLabel="Clave del Reporte <span style='color:red;'>*</span> "
                                                AllowBlank="false" Width="345" MaxLength="10" LabelWidth="100" />
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel runat="server" Layout="FitLayout" Width="540" Height="5" Border="false" />
                                    <ext:TextField ID="txtNombre" runat="server" FieldLabel="Nombre del Reporte  <span style='color:red;'>*</span> "
                                        MaxLength="100" Width="540" MaxLengthText="100" AllowBlank="false" />
                                    <ext:TextField ID="txtSP" runat="server" FieldLabel="Procedimiento Almacenado  <span style='color:red;'>*</span> "
                                        MaxLength="150" Width="540" MaxLengthText="150" AllowBlank="false" />
                                </Items>
                                <Buttons>
                                    <ext:Button runat="server" ID="btnCrear" Text="Crear Reporte" Icon="Add">
                                        <DirectEvents>
                                            <Click OnEvent="btnCrear_Click" Before="var valid= #{FormPanelCrear}.getForm().isValid(); if (!valid) {} return valid;">
                                                <EventMask ShowMask="true" Msg="Creando Reporte..." MinDelay="500" />
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                </Buttons>
                            </ext:FormPanel>
                        </North>
                        <Center Collapsible="true">
                            <ext:GridPanel ID="GridReportes" runat="server" Border="false" Height="270" Title="Reportes Existentes"
                                AutoExpandColumn="Sp">
                                <Store>
                                    <ext:Store ID="StoreReportes" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Reporte">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Reporte" />
                                                    <ext:RecordField Name="ClaveReporte" />
                                                    <ext:RecordField Name="Nombre" />
                                                    <ext:RecordField Name="Sp" />
                                                    <ext:RecordField Name="ID_TipoGeneracionReporte" />
                                                    <ext:RecordField Name="TipoGeneracion" />
                                                    <ext:RecordField Name="Estatus" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                        <ext:Column Hidden="true" DataIndex="ID_Reporte" />
                                        <ext:Column Header="Clave" Sortable="true" DataIndex="ClaveReporte" Width="100" />
                                        <ext:Column Header="Nombre del Reporte" Sortable="true" DataIndex="Nombre" Width="220" />
                                        <ext:Column Header="Procedimiento Almacenado (SP)" Sortable="true" DataIndex="Sp" Width="100" />
                                        <ext:Column Header="Tipo de Generación del Reporte" Sortable="true" DataIndex="TipoGeneracion" Width="220" />
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
                                            ConfirmRequest="true" Title="Confirmación" Message="¿Estás seguro de desactivar el Reporte?" />
                                        <EventMask ShowMask="true" Msg="Procesando..." MinDelay="500" />
                                        <ExtraParams>
                                            <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                            <ext:Parameter Name="Values" Value="Ext.encode(record.data)" Mode="Raw" />
                                        </ExtraParams>
                                    </Command>
                                </DirectEvents>
                                <SelectionModel>
                                    <ext:RowSelectionModel SingleSelect="true" />
                                </SelectionModel>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreReportes" DisplayInfo="true"
                                        DisplayMsg="Mostrando Reportes {0} - {1} de {2}" PageSize="15" HideRefresh="true" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>


