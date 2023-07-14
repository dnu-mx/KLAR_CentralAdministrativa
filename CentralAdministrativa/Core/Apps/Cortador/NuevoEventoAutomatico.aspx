<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="NuevoEventoAutomatico.aspx.cs" Inherits="Cortador.NuevoEventoAutomatico" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var uncheckEvent = function (sm, rowIndex, record) {
            Cortador.AgruparEventosAutomaticos(record.data.ID_Evento);
            //alert("RECORD: " + record.data.ID_Evento);
        };

        var ckeckEvent = function (sm, rowIndex, r) {
            Cortador.AgruparEventosAutomaticos(r.data.ID_Evento);
            //alert("RECORD: " + r.data.ID_Evento);
        };

        var uncheckEventTX = function (sm, rowIndex, record) {
            Cortador.AsociarEventosTX(record.data.ID_EventoAgrupado, record.data.ID_Evento);
        };

        var ckeckEventTX = function (sm, rowIndex, r) {
            Cortador.AsociarEventosTX(r.data.ID_EventoAgrupado, r.data.ID_Evento);
        };
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <ext:Window ID="WindowNuevoEvento" runat="server" Title="Evento" Hidden="true" Width="450" Height="212"
        Modal="true" Resizable="false">
        <Items>
            <ext:FormPanel ID="FormPanelNuevoEvento" runat="server" Padding="10" MonitorValid="true" LabelAlign="Left" LabelWidth="150">
                <Content>
                    <ext:Store ID="StoreTipoCuenta" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_TipoCuenta">
                                <Fields>
                                    <ext:RecordField Name="ID_TipoCuenta" />
                                    <ext:RecordField Name="ClaveTipoCuenta" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:Store ID="StoreEstatusConfig" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_EstatusConfiguracion">
                                <Fields>
                                    <ext:RecordField Name="ID_EstatusConfiguracion" />
                                    <ext:RecordField Name="Clave" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Content>
                <Items>
                    <ext:TextField ID="txtNombre" runat="server" FieldLabel="Nombre" MaxLength="50"
                        AllowBlank="false" AnchorHorizontal="100%" />
                    <ext:TextField ID="txtClave" runat="server" FieldLabel="Clave" MaxLength="10" AllowBlank="false"
                        Selectable="false" Enabled="false" Disabled="true" AnchorHorizontal="100%" />
                    <ext:TextField ID="txtDescripcion" runat="server" FieldLabel="Descripción" AllowBlank="false"
                        MaxLength="100" AnchorHorizontal="100%" />
                    <ext:ComboBox ID="cmbTipoCuenta" runat="server" FieldLabel="Tipo de Cuenta" AllowBlank="false"
                        StoreID="StoreTipoCuenta" ValueField="ID_TipoCuenta" DisplayField="Descripcion" AnchorHorizontal="100%" />
                    <ext:ComboBox ID="cmbEstatusConfig" runat="server" FieldLabel="Estatus de Configuración" AllowBlank="false"
                        StoreID="StoreEstatusConfig" ValueField="ID_EstatusConfiguracion" DisplayField="Descripcion" AnchorHorizontal="100%" />
                </Items>
            </ext:FormPanel>
        </Items>
        <FooterBar>
            <ext:Toolbar ID="ToolbarNuevoEvento" runat="server" EnableOverflow="true">
                <Items>
                    <ext:Button ID="btnGuardar" TabIndex="9" runat="server" Text="Guardar" Icon="Disk">
                        <DirectEvents>
                            <Click OnEvent="btnGuardar_Click" Before="var valid= #{FormPanelNuevoEvento}.getForm().isValid(); if (!valid) {} return valid;" />
                        </DirectEvents>
                    </ext:Button>
                </Items>
            </ext:Toolbar>
        </FooterBar>
    </ext:Window>

    <ext:Viewport ID="ViewPort1" runat="server">
        <Items>
            <ext:BorderLayout ID="BorderLayout1" runat="server">
                <Center Split="true">
                    <ext:FormPanel ID="FormPanelConsultaEventos" runat="server" Title="Consulta de Eventos" Layout="FitLayout">
                        <Items>
                            <ext:GridPanel ID="GridPanelConsulta" runat="server" Layout="FitLayout" StripeRows="true"
                                Header="false" Border="false">
                                <Store>
                                    <ext:Store ID="StoreConsulta" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_ConfiguracionCorte">
                                                <Fields>
                                                    <ext:RecordField Name="ID_ConfiguracionCorte" />
                                                    <ext:RecordField Name="ID_TipoCuenta" />
                                                    <ext:RecordField Name="TipoCuenta" />
                                                    <ext:RecordField Name="ID_EstatusConfiguracion" />
                                                    <ext:RecordField Name="EstatusConfiguracion" />
                                                    <ext:RecordField Name="Nombre" />
                                                    <ext:RecordField Name="Clave" />
                                                    <ext:RecordField Name="Descripcion" />
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
                                            <ext:TextField ID="txtIdConfigCorte" runat="server" Hidden="true" />
                                            <ext:Button ID="btnBuscarEvento" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnBuscarEvento_Click" Before="var valid= #{FormPanelConsultaEventos}.getForm().isValid(); if (!valid) {} return valid;">
                                                        <EventMask ShowMask="true" Msg="Buscando Eventos..." MinDelay="500" />
                                                    </Click>
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
                                        <ext:CommandColumn Width="50">
                                            <Commands>
                                                <ext:GridCommand Icon="NoteEdit" CommandName="Edit">
                                                    <ToolTip Text="Editar" />
                                                </ext:GridCommand>
                                            </Commands>
                                            <Commands>
                                                <ext:GridCommand Icon="Tick" CommandName="Select">
                                                    <ToolTip Text="Seleccionar" />
                                                </ext:GridCommand>
                                            </Commands>
                                        </ext:CommandColumn>
                                        <ext:Column DataIndex="ID_ConfiguracionCorte" Hidden="true" />
                                        <ext:Column DataIndex="ID_TipoCuenta" Hidden="true" />
                                        <ext:Column DataIndex="ID_EstatusConfiguracion" Hidden="true" />
                                        <ext:Column DataIndex="Clave" Header="Clave" Width="40"/>
                                        <ext:Column DataIndex="Nombre" Header="Nombre" Width="150"/>
                                        <ext:Column DataIndex="Descripcion" Header="Descripción" Width="200" />
                                        <ext:Column DataIndex="TipoCuenta" Header="Tipo de Cuenta" Width="250"/>
                                        <ext:Column DataIndex="EstatusConfiguracion" Header="Estatus de Configuración" Width="150"/>
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel SingleSelect="true" />
                                </SelectionModel>
                                <DirectEvents>
                                    <%--<RowClick OnEvent="selectEvento_Event">
                                        <ExtraParams>
                                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridPanelConsulta}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                        </ExtraParams>
                                    </RowClick>--%>
                                    <Command OnEvent="EventoComando">
                                        <ExtraParams>
                                            <ext:Parameter Name="Values" Value="Ext.encode(record.data)" Mode="Raw" />
                                            <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                        </ExtraParams>
                                    </Command>
                                </DirectEvents>
                            </ext:GridPanel>
                        </Items>
                    </ext:FormPanel>
                </Center>
                <South Split="true">
                    <ext:Panel ID="PanelSur" runat="server" Height="350" Collapsible="true" Collapsed="true">
                        <Items>
                            <ext:BorderLayout ID="BorderLayout2" runat="server">
                                <Center Split="true">
                                    <ext:Panel ID="PanelEventosEjecutar" runat="server" Title="Eventos a Ejecutar" 
                                        Layout="FitLayout" Width="500" >
                                        <Items>
                                            <ext:GridPanel ID="GridEventosAutomaticos" runat="server" Layout="FitLayout" StripeRows="true"
                                                Header="false" Border="false">
                                                <Store>
                                                    <ext:Store ID="StoreEvAutomaticos" runat="server">
                                                        <Reader>
                                                            <ext:JsonReader IDProperty="ID_Evento">
                                                                <Fields>
                                                                    <ext:RecordField Name="ID_EventoAgrupado" />
                                                                    <ext:RecordField Name="ID_Evento" />
                                                                    <ext:RecordField Name="ClaveEvento" />
                                                                    <ext:RecordField Name="Descripcion" />
                                                                    <ext:RecordField Name="Agrupado" />
                                                                </Fields>
                                                            </ext:JsonReader>
                                                        </Reader>
                                                    </ext:Store>
                                                </Store>
                                                <TopBar>
                                                    <ext:Toolbar ID="ToolbarEventosAutomaticos" runat="server">
                                                        <Items>
                                                            <ext:TextField ID="txtClaveEvAutom" EmptyText="Clave" Width="80" runat="server" />
                                                            <ext:TextField ID="txtDescEvAutom" EmptyText="Descripción" Width="200" runat="server" />
                                                            <ext:TextField ID="txtIdEvAgrupado" runat="server" Hidden="true"/>
                                                            <ext:Button ID="btnBuscarEvAutom" runat="server" Text="Buscar" Icon="Magnifier">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnBuscarEvAutom_Click" />
                                                                </DirectEvents>
                                                            </ext:Button>
                                                        </Items>
                                                    </ext:Toolbar>
                                                </TopBar>
                                                <ColumnModel ID="ColumnModel6" runat="server">
                                                    <Columns>
                                                        <ext:Column DataIndex="ID_EventoAgrupado" Hidden="true" />
                                                        <ext:Column DataIndex="ID_Evento" Hidden="true" />
                                                        <ext:Column DataIndex="Descripcion" Header="Nombre" Width="450" />
                                                    </Columns>
                                                </ColumnModel>
                                                <SelectionModel>
                                                    <ext:CheckboxSelectionModel ID="CheckboxSelectionModel1" runat="server">
                                                        <Listeners>
                                                            <RowDeselect Fn="uncheckEvent" />
                                                            <RowSelect Fn="ckeckEvent" />
                                                        </Listeners>
                                                    </ext:CheckboxSelectionModel>
                                                </SelectionModel>
                                                <DirectEvents>
                                                    <RowClick OnEvent="selectEvAutomatico_Event">
                                                        <ExtraParams>
                                                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridEventosAutomaticos}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                                        </ExtraParams>
                                                    </RowClick>
                                                </DirectEvents>
                                                <BottomBar>
                                                    <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="StoreEvAutomaticos" DisplayInfo="true"
                                                        DisplayMsg="Eventos por Ejecutar {0} - {1} de {2}" />
                                                </BottomBar>
                                            </ext:GridPanel>
                                        </Items>
                                    </ext:Panel>
                                </Center>
                                <East Split="true">
                                    <ext:Panel ID="PanelEventosTX" runat="server" Title="Eventos Transaccionales" 
                                        Layout="FitLayout" Width="500" >
                                        <Items>
                                            <ext:GridPanel ID="GridEventosTX" runat="server" Layout="FitLayout" StripeRows="true"
                                                Header="false" Border="false">
                                                <Store>
                                                    <ext:Store ID="StoreEvTX" runat="server">
                                                        <Reader>
                                                            <ext:JsonReader IDProperty="ID_Evento">
                                                                <Fields>
                                                                    <ext:RecordField Name="ID_EventoOperacion" />
                                                                    <ext:RecordField Name="ID_EventoAgrupado" />
                                                                    <ext:RecordField Name="ID_Evento" />
                                                                    <ext:RecordField Name="ClaveEvento" />
                                                                    <ext:RecordField Name="Descripcion" />
                                                                    <ext:RecordField Name="Activo" />
                                                                </Fields>
                                                            </ext:JsonReader>
                                                        </Reader>
                                                    </ext:Store>
                                                </Store>
                                                <TopBar>
                                                    <ext:Toolbar ID="ToolbarEvTX" runat="server">
                                                        <Items>
                                                            <ext:TextField ID="txtClaveEvTX" EmptyText="Clave" Width="80" runat="server" />
                                                            <ext:TextField ID="txtDescEvTX" EmptyText="Descripción" Width="200" runat="server" />
                                                            <ext:Button ID="btnBuscarEvTX" runat="server" Text="Buscar" Icon="Magnifier">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnBuscarEvTX_Click" />
                                                                </DirectEvents>
                                                            </ext:Button>
                                                        </Items>
                                                    </ext:Toolbar>
                                                </TopBar>
                                                <ColumnModel ID="ColumnModel1" runat="server">
                                                    <Columns>
                                                        <ext:Column DataIndex="ID_Evento" Hidden="true" />
                                                        <ext:Column DataIndex="Descripcion" Header="Nombre" Width="280" />
                                                    </Columns>
                                                </ColumnModel>
                                                <SelectionModel>
                                                    <ext:CheckboxSelectionModel ID="CheckboxSelectionModel2" runat="server">
                                                        <Listeners>
                                                            <RowDeselect Fn="uncheckEventTX" />
                                                            <RowSelect Fn="ckeckEventTX" />
                                                        </Listeners>
                                                    </ext:CheckboxSelectionModel>
                                                </SelectionModel>
                                                <BottomBar>
                                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreEvTX" DisplayInfo="true"
                                                        DisplayMsg="Eventos Transaccionales {0} - {1} de {2}" />
                                                </BottomBar>
                                            </ext:GridPanel>
                                        </Items>
                                    </ext:Panel>
                                </East>
                            </ext:BorderLayout>
                        </Items>
                    </ext:Panel>
                </South>
            </ext:BorderLayout>
        </Items>
    </ext:Viewport>
</asp:Content>
