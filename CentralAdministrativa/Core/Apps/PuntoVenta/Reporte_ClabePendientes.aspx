<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="Reporte_ClabePendientes.aspx.cs" Inherits="TpvWeb.Reporte_ClabePendientes1" %>

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
     <style type="text/css">
       #ctl00_MainContent_FormPanel1 .x-panel-body{
            height:auto!important;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Hidden ID="hdnFlag" runat="server" />
        <ext:Hidden ID="hdnIdLog" runat="server" />
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center Split="true" Collapsible="false">
            <ext:GridPanel ID="GridCLABEs" runat="server" StripeRows="true" Header="false" Border="false" AutoExpandColumn="Mensaje">
                <LoadMask ShowMask="false" />
                <Store>
                    <ext:Store ID="StoreCLABEs" runat="server" RemoteSort="true" AutoLoad="false"
                        OnRefreshData="StoreCLABEs_RefreshData">
                        <AutoLoadParams>
                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                        </AutoLoadParams>
                        <Proxy>
                            <ext:PageProxy />
                        </Proxy>
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="IdReporte">
                                <Fields>
                                    <ext:RecordField Name="IdReporte" />
                                    <ext:RecordField Name="IdLog" />
                                    <ext:RecordField Name="NumeroTarjeta" />
                                    <ext:RecordField Name="CuentaCACAO" />
                                    <ext:RecordField Name="ID" />
                                    <ext:RecordField Name="FechaOperacion" />
                                    <ext:RecordField Name="EstatusEnvio" />
                                    <ext:RecordField Name="Mensaje" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <ColumnModel runat="server">
                    <Columns>
                        <ext:Column Header="IdLog" Sortable="true" DataIndex="IdLog" Hidden="true" />
                        <ext:Column Header="Número de Tarjeta" Sortable="true" DataIndex="NumeroTarjeta" />
                        <ext:Column Header="Cuenta Interna" Sortable="true" DataIndex="CuentaCACAO" Width="110" />
                        <ext:Column Header="ID" Sortable="true" DataIndex="ID"
                            Width="150" />
                        <ext:DateColumn Header="Fecha" Sortable="true" DataIndex="FechaOperacion"
                            Format="yyyy/MM/dd HH:mm:ss" Width="120" />
                        <ext:Column Header="Estatus" Sortable="true" DataIndex="EstatusEnvio" />
                        <ext:Column Header="Descripción del Mensaje" DataIndex="Mensaje" Width="300" />
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
                            <ext:NumericFilter DataIndex="Tarjeta" />
                            <ext:StringFilter DataIndex="EstatusEnvio" />
                        </Filters>
                    </ext:GridFilters>
                </Plugins>
                <BottomBar>
                    <ext:PagingToolbar ID="_PagingCLABEs" runat="server" StoreID="StoreCLABEs" DisplayInfo="true"
                        DisplayMsg="Mostrando Cuentas CLABE {0} - {1} de {2}" HideRefresh="true" />
                </BottomBar>
                <TopBar>
                    <ext:Toolbar runat="server" LabelWidth="50" LabelAlign="Right">
                        <Items>
                            <ext:ComboBox ID="cBoxCC" runat="server" EmptyText="Selecciona el SubEmisor..."
                                Width="180" DisplayField="NombreORazonSocial" ValueField="ID_Colectiva" Mode="Local" AutoSelect="true"
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
                            </ext:ComboBox>
                            <ext:DateField ID="dfFecha" runat="server" Vtype="daterange" MaskRe="[0-9\/]" Width="160"
                                FieldLabel="Fecha" Format="dd/MM/yyyy" EnableKeyEvents="true" MaxDate="<%# DateTime.Now %>"
                                AutoDataBind="true" AllowBlank="false" InvalidText="Fecha inválida. Debe tener el formato DD/MM/AAAA" />
                            <ext:ComboBox ID="cBoxEstatus" EmptyText="Estatus de Envío" Width="180" runat="server"
                                AllowBlank="false" ListWidth="200">
                                <Items>
                                    <ext:ListItem Text="Insertado con error" Value="0" />
                                    <ext:ListItem Text="No se ha podido generar la CLABE" Value="1" />
                                    <ext:ListItem Text="Clabe generada" Value="2" />
                                </Items>
                            </ext:ComboBox>
                            <ext:TextField ID="txtTarjeta" runat="server" EmptyText="Número de Tarjeta"
                                Width="150" MaskRe="[0-9]" MinLength="1" MaxLength="30" />
                            <ext:Button ID="btnBuscarHide" runat="server" Hidden="true">
                                <Listeners>
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Cuentas CLABE...' });
                                        #{GridCLABEs}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click"
                                        Before="if (!#{cBoxCC}.isValid() || !#{dfFecha}.isValid() ||
                                        !#{cBoxEstatus}.isValid()) { return false; }
                                        else { resetToolbar(#{_PagingCLABEs});
                                        #{GridCLABEs}.getStore().sortInfo = null; }" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                            RepClabe.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarFill runat="server" />
                            <ext:Button ID="btnEnviar" runat="server" Text="Enviar Seleccionado(s)" Icon="PageWhiteGo"
                                Disabled="true">
                                <DirectEvents>
                                    <Click OnEvent="btnEnviar_Click">
                                        <EventMask ShowMask="true" Msg="Enviando..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarSeparator runat="server" />
                            <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" Icon="PageExcel"
                                Disabled="true">
                                <DirectEvents>
                                    <Click OnEvent="btnExportExcel_Click" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                            e.stopEvent(); 
                                            RepClabe.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
            </ext:GridPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>