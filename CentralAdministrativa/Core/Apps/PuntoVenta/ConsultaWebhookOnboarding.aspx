<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ConsultaWebhookOnboarding.aspx.cs" Inherits="TpvWeb.ConsultaWebhookOnboarding" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function resetToolbar(tbar) {
            tbar.updateInfo();
            tbar.inputItem.setValue(1);
            tbar.afterTextItem.setText(String.format(tbar.afterPageText, 1));
            tbar.next.setDisabled(true);
            tbar.prev.setDisabled(true);
            tbar.first.setDisabled(true);
            tbar.last.setDisabled(true);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Hidden ID="hdnFlag" runat="server" />
    <ext:Hidden ID="hdnCveColectiva" runat="server" />
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true" Collapsible="true">
            <ext:FormPanel ID="FormPanel1" Width="320" Title="Selecciona los filtros" runat="server" Padding="10"
                Border="false" Layout="FitLayout" LabelWidth="160">
                <Items>
                    <ext:ComboBox ID="cBoxCC" runat="server" FieldLabel="SubEmisor <span style='color:red;'>*   </span>"
                        Width="250" DisplayField="NombreORazonSocial" ValueField="ID_Colectiva" Mode="Local" AutoSelect="true"
                        ForceSelection="true" TypeAhead="true" MinChars="1" MatchFieldWidth="false"
                        AllowBlank="false">
                        <Store>
                            <ext:Store ID="StoreCC" runat="server">
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_Colectiva">
                                        <Fields>
                                            <ext:RecordField Name="ID_Colectiva" />
                                            <ext:RecordField Name="ClaveColectiva" />
                                            <ext:RecordField Name="NombreORazonSocial" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                                <SortInfo Field="NombreORazonSocial" Direction="ASC" />
                            </ext:Store>
                        </Store>
                        <Listeners>
                            <Select Handler="var id = this.getValue(); var record = this.getStore().getById(id);
                                #{hdnCveColectiva}.setValue(record.get('ClaveColectiva'));" />
                        </Listeners>
                    </ext:ComboBox>
                    <ext:DateField ID="dfFecha" runat="server" Vtype="daterange" MaskRe="[0-9\/]" Width="300"
                        FieldLabel="Fecha <span style='color:red;'>*   </span>" Format="dd/MM/yyyy"
                        EnableKeyEvents="true" MaxDate="<%# DateTime.Now %>" AutoDataBind="true" AllowBlank="false"
                        InvalidText="Fecha inválida. Debe tener el formato DD/MM/AAAA" />
                    <ext:ComboBox ID="cBoxEstatus" FieldLabel="Estatus de Envío  <span style='color:red;'>*   </span>"
                        Width="300" runat="server" AllowBlank="false" DisplayField="Descripcion" ValueField="ID_EstatusEnvioWebhook">
                        <Store>
                            <ext:Store ID="StoreEstatusWH" runat="server">
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_EstatusEnvioWebhook">
                                        <Fields>
                                            <ext:RecordField Name="ID_EstatusEnvioWebhook" />
                                            <ext:RecordField Name="Clave" />
                                            <ext:RecordField Name="Descripcion" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                    </ext:ComboBox>
                    <ext:TextField ID="txtIdMensaje" runat="server" FieldLabel="ID Mensaje" EmptyText="Todos"
                        Width="300" MaskRe="[0-9]" MinLength="1" MaxLength="30" />
                    <ext:Panel runat="server" Layout="FitLayout" Height="20" Border="false" />
                    <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false" LabelWidth="5" >
                        <Defaults>
                            <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                        </Defaults>
                        <LayoutConfig>
                            <ext:HBoxLayoutConfig Align="Top" />
                        </LayoutConfig>
                        <Items>
                            <ext:Label runat="server" LabelSeparator=" " Width="200" />
                            <ext:Hidden runat="server" Flex="1" Width="25" />
                            <ext:Label runat="server" FieldLabel="<span style='color:red;'>*   </span>"
                                Text="Obligatorios" LabelSeparator=" " StyleSpec="font-style: italic;font-family:segoe ui;font-size: 11px;" />
                        </Items>
                    </ext:Panel>
                </Items>
                <FooterBar>
                    <ext:Toolbar ID="Toolbar1" runat="server">
                        <Items>
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnBuscarHide" runat="server" Hidden="true">
                                <Listeners>
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Webhook...' });
                                        #{GridWHOnboard}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                        Before="if (!#{FormPanel1}.getForm().isValid()) { return false; }
                                        else { resetToolbar(#{PagingWHOnboard}); #{hdnFlag}.setValue(0);
                                        #{GridWHOnboard}.getStore().sortInfo = null; }">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnReenvioMasivo" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="btnReenvioMasivo_Click">
                                        <EventMask ShowMask="true" Msg="Reenviando..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>
            </ext:FormPanel>
        </West>
        <Center Split="true" Collapsible="false">
            <ext:FormPanel ID="FormPanelWHOnboard" runat="server" Layout="FitLayout" Title="Webhook Onboarding obtenidos con los filtros seleccionados">
                <Items>
                    <ext:GridPanel ID="GridWHOnboard" runat="server" StripeRows="true" Header="false" Border="false">
                        <LoadMask ShowMask="false" />
                        <Store>
                            <ext:Store ID="StoreWHOnboard" runat="server" RemoteSort="true" OnRefreshData="StoreWHOnboard_RefreshData"
                                AutoLoad="false">
                                <AutoLoadParams>
                                    <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                </AutoLoadParams>
                                <Proxy>
                                    <ext:PageProxy />
                                </Proxy>                                
                                <DirectEventConfig IsUpload="true"/>
                                <Reader>
                                    <ext:JsonReader IDProperty="IdMensaje">
                                        <Fields>
                                            <ext:RecordField Name="IdMensaje" />
                                            <ext:RecordField Name="FechaOperacion" />
                                            <ext:RecordField Name="EstatusEnvio" />                                            
                                            <ext:RecordField Name="DescripcionMensaje" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <ColumnModel runat="server">
                            <Columns>
                                <ext:DateColumn Header="Fecha" Sortable="true" DataIndex="FechaOperacion"
                                    Format="yyyy-MM-dd HH:mm:ss"  Width="120"/>
                                <ext:Column Header="ID Mensaje" Sortable="true" DataIndex="IdMensaje" />
                                <ext:Column Header="Estatus" Sortable="true" DataIndex="EstatusEnvio" />
                                <ext:Column Header="Descripción del Mensaje" DataIndex="DescripcionMensaje" Width="500" />
                            </Columns>
                        </ColumnModel>
                        <SelectionModel>
                            <ext:CheckboxSelectionModel runat="server">
                                <Listeners>
                                    <AfterCheckAllClick Handler="#{hdnFlag}.setValue(1);" />
                                </Listeners>
                            </ext:CheckboxSelectionModel>
                        </SelectionModel>
                        <Plugins>
                            <ext:GridFilters runat="server" Local="true" FiltersText="Filtros">
                                <Filters>
                                    <ext:DateFilter DataIndex="FechaOperacion" BeforeText="Antes de" OnText="El día"
                                        AfterText="Después de">
                                        <DatePickerOptions runat="server" TodayText="Hoy" />
                                    </ext:DateFilter>
                                    <ext:StringFilter DataIndex="EstatusEnvio" />
                                </Filters>
                            </ext:GridFilters>
                        </Plugins>
                        <BottomBar>
                            <ext:PagingToolbar ID="PagingWHOnboard" runat="server" StoreID="StoreWHOnboard" DisplayInfo="true"
                                DisplayMsg="Mostrando Mensajes {0} - {1} de {2}" HideRefresh="true" />
                        </BottomBar>
                        <TopBar>
                            <ext:Toolbar ID="Toolbar2" runat="server">
                                <Items>
                                    <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                                    <ext:Button ID="btnReenviar" runat="server" Text="Reenviar Seleccionado(s)" Icon="PageWhiteGo" Disabled="true">
                                        <DirectEvents>
                                            <Click OnEvent="btnReenviar_Click" Before="if (!#{GridWHOnboard}.getSelectionModel().hasSelection()) return false;">
                                                <Confirmation ConfirmRequest="true" Message="¿Estás seguro de reenviar el(los) seleccionado(s)?"
                                                    BeforeConfirm="if (!#{GridWHOnboard}.getSelectionModel().hasSelection()) return false;"
                                                    Title="Confirmación" />
                                                <EventMask ShowMask="true" Msg="Reenviando..." MinDelay="500" />
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:ToolbarSeparator runat="server" />
                                    <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="true">
                                        <DirectEvents>
                                            <Click OnEvent="btnExportExcel_Click" IsUpload="true"
                                                After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                    e.stopEvent(); 
                                                    RepWHOnboard.StopMask();" />
                                        </DirectEvents>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                    </ext:GridPanel>
                </Items>
            </ext:FormPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
