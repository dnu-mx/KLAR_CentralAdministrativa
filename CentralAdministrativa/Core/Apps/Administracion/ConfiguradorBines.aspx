<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="ConfiguradorBines.aspx.cs" Inherits="Administracion.ConfiguradorBines" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var prepareToolbar = function (grid, toolbar, rowIndex, record) {
            if (record.get("Asignado") == 1) { //SI
                toolbar.items.get(0).hide();
            } else {
                toolbar.items.get(1).hide();
            }
        }

        var afterEdit = function (e) {
            Bines.ActualizaTipoSaldo(
                e.record.data.ID_GrupoMA);
        };
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent"  runat="server">
    <ext:Hidden ID="hdnIdColectiva" runat="server" />
    <ext:Hidden ID="hdnColectiva" runat="server" />
    <ext:Hidden ID="hdnIDGrupoMA" runat="server" />
    <ext:Window ID="WdwNuevoBin" runat="server" Title="Datos del Nuevo Bin" Width="380" Height="210"
        Hidden="true" Modal="true" Resizable="false" Closable="true">
        <Items>
            <ext:FormPanel ID="FormPanelNuevoBin" runat="server" Padding="10" MonitorValid="true" 
                LabelAlign="Left" LabelWidth="80">
                <Items>
                    <ext:TextField ID="txtClaveBin" runat="server" FieldLabel="Clave" Width="250" MaskRe="[0-9]"
                        AllowBlank="false" MaxLength="8" MinLength="8"/>
                    <ext:TextArea ID="txtDescripcionBin" runat="server" FieldLabel="Descripción" Width="250" Height="90"
                        AllowBlank="false" MaxLength="200" />
                </Items>
                <Buttons>
                    <ext:Button ID="btnCrearNuevoBin" runat="server" Text="Crear Bin" Icon="Add">
                        <DirectEvents>
                            <Click OnEvent="btnCrearNuevoBin_Click" Before="var valid= #{FormPanelNuevoBin}.getForm().isValid(); if (!valid) {} return valid;" />
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:BorderLayout ID="MainBorderLayout" runat="server">
        <West Split="true">
            <ext:Panel runat="server" Width="325" Border="false" Layout="FitLayout" Title="Consulta">
                <Content>
                    <ext:BorderLayout ID="LeftBorderLayout" runat="server">
                        <North Split="true">
                            <ext:FormPanel ID="FormPanelBusqueda" runat="server" Height="100" Border="false" Layout="FormLayout"
                                Padding="10" LabelWidth="120">
                                <Items>
                                    <ext:ComboBox ID="cBoxTipoColec" runat="server" FieldLabel="Tipo de Colectiva" Width="180"
                                        AllowBlank="false" DisplayField="Descripcion" ValueField="ID_TipoColectiva">
                                        <Store>
                                            <ext:Store ID="StoreTipoColectiva" runat="server">
                                                <Reader>
                                                    <ext:JsonReader IDProperty="ID_TipoColectiva">
                                                        <Fields>
                                                            <ext:RecordField Name="ID_TipoColectiva" />
                                                            <ext:RecordField Name="Clave" />
                                                            <ext:RecordField Name="Descripcion" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Store>
                                    </ext:ComboBox>
                                    <ext:TextField ID="txtColectiva" runat="server" FieldLabel="Clave o Razón Social" Width="180" />
                                    <ext:Panel runat="server" Width="325" Border="false" Height="10"/>
                                    <ext:Panel runat="server" Layout="ColumnLayout" Width="325" Height="25" Border="false">
                                        <Items>
                                            <ext:Panel runat="server" Border="false" Width="135" />
                                            <ext:Panel runat="server" Border="false" Width="90" Height="25">
                                                <Items>
                                                    <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh" Width="80">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnLimpiar_Click" />
                                                        </DirectEvents>
                                                    </ext:Button>
                                                </Items>
                                            </ext:Panel>
                                            <ext:Panel runat="server" Border="false" Width="90" Height="25">
                                                <Items>
                                                    <ext:Button ID="btnBuscarColectiva" runat="server" Text="Buscar" Icon="Magnifier" Width="80">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnBuscar_Click" Before="var valid= #{cBoxTipoColec}.isValid(); if (!valid) {} return valid;">
                                                                <EventMask ShowMask="true" Msg="Buscando Colectivas..." MinDelay="500" />
                                                            </Click>
                                                        </DirectEvents>
                                                    </ext:Button>
                                                </Items>
                                            </ext:Panel>
                                        </Items>
                                    </ext:Panel>
                                </Items>
                            </ext:FormPanel>
                        </North>
                        <Center Split="true">
                            <ext:GridPanel ID="GridColectivas" runat="server" Title="Colectivas" StripeRows="true" Border="false"
                                AutoExpandColumn="NombreORazonSocial" Width="350">
                                <LoadMask ShowMask="false" />
                                <Store>
                                    <ext:Store ID="StoreColectivas" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Colectiva">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Colectiva" />
                                                    <ext:RecordField Name="ClaveColectiva" />
                                                    <ext:RecordField Name="NombreORazonSocial" />
                                                    <ext:RecordField Name="ClaveTipoColectiva" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                        <SortInfo Field="NombreORazonSocial" Direction="ASC" />
                                    </ext:Store>
                                </Store>
                                <ColumnModel runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_Colectiva" Hidden="true" />
                                        <ext:Column DataIndex="ClaveColectiva" Header="Clave" Width="90" />
                                        <ext:Column DataIndex="NombreORazonSocial" Header="Nombre" Width="110" />
                                        <ext:Column DataIndex="ClaveTipoColectiva" Hidden="true" />
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel SingleSelect="true" />
                                </SelectionModel>
                                <DirectEvents>
                                    <RowClick OnEvent="selectRowColectivas_Event">
                                        <EventMask ShowMask="true" Msg="Obteniendo Información de la Colectiva..." MinDelay="500" />
                                        <ExtraParams>
                                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridColectivas}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                        </ExtraParams>
                                    </RowClick>
                                </DirectEvents>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreColectivas" DisplayInfo="true"
                                        DisplayMsg="Mostrando Colectivas {0} - {1} de {2}" HideRefresh="true" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </West>
        <Center Split="true">
            <ext:Panel ID="PanelGpoMA" runat="server" Border="false" Layout="FitLayout" Title="Grupo de Tarjetas">
                <Content>
                    <ext:BorderLayout runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridGruposMA" runat="server" StripeRows="true" Border="false" AutoExpandColumn="Descripcion">
                                <LoadMask ShowMask="false" />
                                <Store>
                                    <ext:Store ID="StoreGrupoMA" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_GrupoMA">
                                                <Fields>
                                                    <ext:RecordField Name="ID_GrupoMA" />
                                                    <ext:RecordField Name="ClaveGrupo" />
                                                    <ext:RecordField Name="Descripcion" />
                                                    <ext:RecordField Name="TipoSaldo" />
                                                    <ext:RecordField Name="GrupoMAPadre" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                        <ext:CommandColumn Width="30">
                                            <Commands>
                                                <ext:GridCommand Icon="BulletArrowDown" CommandName="BinesGMA">
                                                    <ToolTip Text="Mostrar Bines Asociados" />
                                                </ext:GridCommand>
                                            </Commands>
                                        </ext:CommandColumn>
                                        <ext:Column DataIndex="ID_GrupoMA" Header="ID GMA" />
                                        <ext:Column DataIndex="Descripcion" Header="Descripción" />
                                        <ext:Column DataIndex="TipoSaldo" Header="Tipo Saldo">
                                            <Editor>
                                                <ext:ComboBox ID="cBoxTipoIntegracion" runat="server"
                                                    DisplayField="Descripcion" ValueField="ID_TipoIntegracion">
                                                    <Store>
                                                        <ext:Store ID="StoreTipoIntegracion" runat="server">
                                                            <Reader>
                                                                <ext:JsonReader IDProperty="ID_TipoIntegracion">
                                                                    <Fields>
                                                                        <ext:RecordField Name="ID_TipoIntegracion" />
                                                                        <ext:RecordField Name="ClaveTipoIntegracion" />
                                                                        <ext:RecordField Name="Descripcion" />
                                                                    </Fields>
                                                                </ext:JsonReader>
                                                            </Reader>
                                                        </ext:Store>
                                                    </Store>
                                                    </ext:ComboBox>
                                            </Editor>
                                        </ext:Column>
                                        <ext:Column DataIndex="GrupoMAPadre" Header="Grupo Padre" />
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel SingleSelect="true" />
                                </SelectionModel>
                                <DirectEvents>
                                    <Command OnEvent="ComandoGMA">
                                        <EventMask ShowMask="true" Msg="Buscando Bines..." MinDelay="500" />
                                        <ExtraParams>
                                            <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                            <ext:Parameter Name="ID_GrupoMA" Value="record.data.ID_GrupoMA" Mode="Raw"/>
                                            <ext:Parameter Name="Descripcion" Value="record.data.Descripcion" Mode="Raw"/>
                                        </ExtraParams>
                                    </Command>
                                </DirectEvents>
                                <Listeners>
                                    <AfterEdit Fn="afterEdit" />
                                </Listeners>
                                <Plugins>
                                    <ext:RowEditor runat="server" SaveText="Actualizar" CancelText="Cancelar">
                                        <Listeners>
                                            <AfterEdit Fn="afterEdit" />
                                        </Listeners>
                                    </ext:RowEditor>
                                </Plugins>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar3" runat="server" StoreID="StoreGrupoMA" DisplayInfo="true"
                                        DisplayMsg="Mostrando Clientes {0} - {1} de {2}" HideRefresh="true" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                        <South Split="true">
                            <ext:GridPanel ID="GridBines" runat="server" StripeRows="true" Header="true" Layout="FitLayout" Title="Bines"
                                Height="250" Border="false">
                                <Store>
                                    <ext:Store ID="StoreBines" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_BINGrupoMA">
                                                <Fields>
                                                    <ext:RecordField Name="ID_BINGrupoMA" />
                                                    <ext:RecordField Name="ClaveBIN" />
                                                    <ext:RecordField Name="Descripcion" />
                                                    <ext:RecordField Name="Asignado" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <TopBar>
                                    <ext:Toolbar runat="server">
                                        <Items>
                                            <ext:ToolbarFill runat="server" />
                                            <ext:Button ID="btnNuevoBin" runat="server" Text="Nuevo Bin" Icon="Add" Width="100" Disabled="true">
                                                <Listeners>
                                                    <Click Handler="#{FormPanelNuevoBin}.reset(); #{WdwNuevoBin}.show();" />
                                                </Listeners>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <ColumnModel ID="ColumnModel2" runat="server">
                                    <Columns>
                                        <ext:Column runat="server" Hidden="true" DataIndex="ID_BINGrupoMA" />
                                        <ext:Column Header="Clave Bin" Width="100" DataIndex="ClaveBIN" />
                                        <ext:Column Header="Descripción" Sortable="true" DataIndex="Descripcion" Width="400" />
                                        <ext:Column Hidden="true" DataIndex="Asignado" />
                                        <ext:CommandColumn Header="Acción" Width="50">
                                            <PrepareToolbar Fn="prepareToolbar" />
                                            <Commands>
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
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel2" runat="server" SingleSelect="true" />
                                </SelectionModel>
                                <DirectEvents>
                                    <Command OnEvent="EjecutarComando">
                                        <Confirmation BeforeConfirm="if (command == 'Unlock') return false;"
                                            ConfirmRequest="true" Title="Confirmación" Message="¿Estás seguro de inactivar el BIN al Cliente?" />
                                        <ExtraParams>
                                            <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                            <ext:Parameter Name="ID_BINGrupoMA" Value="Ext.encode(record.data.ID_BINGrupoMA)" Mode="Raw" />
                                        </ExtraParams>
                                    </Command>
                                </DirectEvents>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="StoreBines" DisplayInfo="true"
                                        DisplayMsg="Mostrando Bines {0} - {1} de {2}" HideRefresh="true" />
                                </BottomBar>
                            </ext:GridPanel>
                        </South>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
