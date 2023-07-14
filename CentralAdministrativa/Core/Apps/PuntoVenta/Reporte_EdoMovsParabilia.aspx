<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" ValidateRequest="false"
    CodeBehind="Reporte_EdoMovsParabilia.aspx.cs" Inherits="TpvWeb.Reporte_EdoMovsParabilia" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var prepareToolbar = function (grid, toolbar, rowIndex, record) {
            if (record.get("PDFGenerado") == 0) {
                toolbar.items.get(0).hide();
                toolbar.items.get(1).hide();
            }
        }

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
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center Split="true">
            <ext:GridPanel ID="GridMovimientos" runat="server" StripeRows="true" Header="false" Border="false"
                Layout="FitLayout" AutoScroll="true">
                <Store>
                    <ext:Store ID="StoreMovimientos" runat="server" RemoteSort="true" AutoLoad="false"
                        OnRefreshData="StoreMovimientos_RefreshData">
                        <AutoLoadParams>
                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                        </AutoLoadParams>
                        <Proxy>
                            <ext:PageProxy />
                        </Proxy>
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Corte">
                                <Fields>
                                    <ext:RecordField Name="ID_Corte" />
                                    <ext:RecordField Name="FechaCorte" />
                                    <ext:RecordField Name="NumTarjeta" />
                                    <ext:RecordField Name="NombreTarjetahabiente" />                                
                                    <ext:RecordField Name="EstatusCorreo" />
                                    <ext:RecordField Name="PDFGenerado" />
                                    <ext:RecordField Name="Ruta" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <TopBar>
                    <ext:Toolbar ID="Toolbar2" runat="server">
                        <Items>
                            <ext:Hidden ID="hdnFechaCorte" runat="server" />
                            <ext:ComboBox ID="cBoxSubEmisor" runat="server" EmptyText="SubEmisor" Width="150"
                                DisplayField="NombreORazonSocial" ValueField="ID_Colectiva" Mode="Local" AutoSelect="true"
                                ForceSelection="true" TypeAhead="true" MinChars="1" MatchFieldWidth="false"
                                AllowBlank="false" ListWidth="180">
                                <Store>
                                    <ext:Store ID="StoreClientes" runat="server">
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
                            <ext:DateField ID="dfCorte" runat="server" Format="MM/Y" EmptyText="Mes/Año Corte"
                                Width="150" AllowBlank="false">
                                <Plugins>
                                    <ext:MonthPicker runat="server" />
                                </Plugins>
                            </ext:DateField>
                            <ext:ComboBox ID="cBoxEstatus" runat="server" EmptyText="Estatus de Envío" Width="150"
                                AllowBlank="false">
                                <Items>
                                    <ext:ListItem Text="Enviado" Value="1" />
                                    <ext:ListItem Text="No Enviado" Value="0" />
                                </Items>
                            </ext:ComboBox>
                            <ext:TextField ID="txtTarjeta" runat="server" EmptyText="Tarjeta" Width="180"
                                MaskRe="[0-9]" MinLength="16" MaxLength="16" />
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                        Before="var valid1 = #{cBoxSubEmisor}.isValid(); 
                                        if (!valid1) {
                                            var valid = valid1; }
                                        else {
                                            var valid2 = #{dfCorte}.isValid();
                                            if (!valid2) {
                                                var valid = valid2; }
                                            else {
                                                var valid3 = #{cBoxEstatus}.isValid();
                                                if (!valid3) {
                                                    var valid = valid3; }
                                                else {
                                                    var valid4 = #{txtTarjeta}.isValid();
                                                    if (!valid4) {
                                                        var valid = valid4; }
                                                    else {
                                                        #{hdnFechaCorte}.setValue(#{dfCorte}.getValue());
                                                        resetToolbar(#{PagingEdoMovs});
                                                        #{GridMovimientos}.getStore().sortInfo = null; }
                                                }
                                            }
                                        }
                                        return valid; ">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnBuscarHide" runat="server" Hidden="true">
                                <Listeners>
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Estado de Movimientos...' });
                                        #{GridMovimientos}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                        EdoMovs.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarFill runat="server" />
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <ColumnModel ID="ColumnModel1" runat="server">
                    <Columns>
                        <ext:Column DataIndex="ID_Corte" Hidden="true" />
                        <ext:DateColumn Header="Fecha Corte" Sortable="true" DataIndex="FechaCorte"
                            Format="yyyy-MM-dd" Width="150"/>
                        <ext:Column Header="Tarjeta" Sortable="true" DataIndex="NumTarjeta" Width="150"/>
                        <ext:Column Header="Nombre" Sortable="true" DataIndex="NombreTarjetahabiente"
                            Width="400" />
                        <ext:Column Header="Estatus" Sortable="true" DataIndex="EstatusCorreo" Width="150"/>
                        <ext:CommandColumn Width="60">
                            <PrepareToolbar Fn="prepareToolbar" />
                            <Commands>
                                <ext:GridCommand Icon="PageWhiteAcrobat" CommandName="PDF">
                                    <ToolTip Text="Descargar PDF" />
                                </ext:GridCommand>
                                <ext:GridCommand Icon="EmailGo" CommandName="Enviar">
                                    <ToolTip Text="Enviar Correo" />
                                </ext:GridCommand>
                            </Commands>
                        </ext:CommandColumn>
                        <ext:Column DataIndex="PDFGenerado" Hidden="true" />
                        <ext:Column DataIndex="Ruta" Hidden="true" />
                    </Columns>
                </ColumnModel>
                <SelectionModel>
                    <ext:RowSelectionModel ID="RowSelectionModel2" runat="server" SingleSelect="true" />
                </SelectionModel>
                <DirectEvents>
                    <Command OnEvent="EjecutaComando" IsUpload="true">
                        <ExtraParams>
                            <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                            <ext:Parameter Name="ID_Corte" Value="Ext.encode(record.data.ID_Corte)" Mode="Raw" />
                            <ext:Parameter Name="Ruta" Value="Ext.encode(record.data.Ruta)" Mode="Raw" />
                        </ExtraParams>
                    </Command>
                </DirectEvents>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingEdoMovs" runat="server" StoreID="StoreMovimientos" DisplayInfo="true"
                        DisplayMsg="Mostrando Movimientos {0} - {1} de {2}" HideRefresh="true" />
                </BottomBar>
            </ext:GridPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
