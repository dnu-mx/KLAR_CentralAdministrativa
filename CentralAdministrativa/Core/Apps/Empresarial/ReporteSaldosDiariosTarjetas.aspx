<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" ValidateRequest="false"
    CodeBehind="ReporteSaldosDiariosTarjetas.aspx.cs" Inherits="Empresarial.ReporteSaldosDiariosTarjetas" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var submitValue = function (grid, hiddenFormat, format) {
            hiddenFormat.setValue(format);
            grid.submitData(false);
        };

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
        <Content>
            <ext:Hidden ID="FormatType" runat="server" />
        </Content>
        <Center Split="true">
            <ext:GridPanel ID="GridSaldosDiariosTarjetas" runat="server" StripeRows="true" Header="false" Border="false"
                Layout="FitLayout" AutoScroll="true">
                <Store>
                    <ext:Store ID="StoreTarjetasSD" runat="server" RemoteSort="true" AutoLoad="false"
                        OnRefreshData="StoreTarjetasSD_RefreshData">
                        <AutoLoadParams>
                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                        </AutoLoadParams>
                        <Proxy>
                            <ext:PageProxy />
                        </Proxy>
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Tarjeta">
                                <Fields>
                                    <ext:RecordField Name="ID_Tarjeta" />
                                    <ext:RecordField Name="NumTarjeta" />
                                    <ext:RecordField Name="NombreTarjetahabiente" />
                                    <ext:RecordField Name="CuentaInterna" />
                                    <ext:RecordField Name="CuentaExterna" />
                                    <ext:RecordField Name="CuentaCLABE" />
                                    <ext:RecordField Name="EstatusTarjeta" />
                                    <ext:RecordField Name="FechaConsulta" />
                                    <ext:RecordField Name="Saldo" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <TopBar>
                    <ext:Toolbar ID="Toolbar2" runat="server">
                        <Items>
                            <ext:ComboBox ID="cBoxTipoColectiva" runat="server" EmptyText="Tipo de Colectiva" Width="150"
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
                                <DirectEvents>
                                    <Select OnEvent="EstableceColectivas" Before="#{cBoxCliente}.clearValue();">
                                        <EventMask ShowMask="true" Msg="Estableciendo Colectivas..." MinDelay="200" />
                                    </Select>
                                </DirectEvents>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cBoxCliente" runat="server" EmptyText="Selecciona la Colectiva..." Width="180"
                                AllowBlank="false" DisplayField="NombreORazonSocial" ValueField="ID_Colectiva">
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
                                    </ext:Store>
                                </Store>
                            </ext:ComboBox>
                            <ext:DateField ID="dfFecha" runat="server" Vtype="daterange" EmptyText="Fecha"
                                Format="dd/MM/yyyy" Width="160" EnableKeyEvents="true" AllowBlank="false">
                            </ext:DateField>
                            <ext:TextField ID="txtTarjeta" runat="server" EmptyText="Número de Tarjeta" Width="130"
                                MaskRe="[0-9]" MaxLength="16" MinLength="16" />
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                        Before="var valid1 = #{cBoxCliente}.isValid(); 
                                        if (!valid1) {
                                            var valid = valid1; }
                                        else {
                                            var valid2 = #{dfFecha}.isValid();
                                            if (!valid2) {
                                                var valid = valid2; }
                                            else {
                                                var valid3 = #{txtTarjeta}.isValid();
                                                if (!valid3) {
                                                    var valid = valid3; }
                                                else {
                                                    resetToolbar(#{PagingSDTarjetas});
                                                    #{GridSaldosDiariosTarjetas}.getStore().sortInfo = null; }
                                                }
                                        } return valid; ">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarSeparator runat="server" />
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnBuscarHide" runat="server" Hidden="true">
                                <Listeners>
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Saldos de Tarjetas...' });
                                        #{GridSaldosDiariosTarjetas}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                        RepSDTarjetas.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarFill runat="server" />
                            <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a  Excel" Icon="PageExcel" ToolTip="Obtener Datos en un Archivo Excel"
                                Disabled="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                        e.stopEvent(); 
                                        RepSDTarjetas.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <ColumnModel ID="ColumnModel1" runat="server">
                    <Columns>
                        <ext:Column Header="Número de Tarjeta" Sortable="true" DataIndex="NumTarjeta" Width="100" />
                        <ext:Column Header="Nombre" Sortable="true" DataIndex="NombreTarjetahabiente"
                            Width="150" />
                        <ext:Column Header="Cuenta Interna" Sortable="true" DataIndex="CuentaInterna" />
                        <ext:Column Header="MA Externo" Sortable="true" DataIndex="CuentaExterna" />
                        <ext:Column Header="CLABE" Sortable="true" DataIndex="CuentaCLABE" />
                        <ext:Column Header="Estatus Tarjeta" Sortable="true" DataIndex="EstatusTarjeta" />
                        <ext:DateColumn Header="Fecha de Consulta" Sortable="true" DataIndex="FechaConsulta"
                            Format="yyyy-MM-dd HH:mm:ss" />
                        <ext:Column Header="Saldo" Sortable="true" DataIndex="Saldo">
                            <Renderer Format="UsMoney" />
                        </ext:Column>
                    </Columns>
                </ColumnModel>
                <View>
                    <ext:GroupingView ID="GroupingView1" runat="server" ForceFit="true" MarkDirty="false"
                        ShowGroupName="false" EnableNoGroups="true" HideGroupedColumn="true" />
                </View>
                <SelectionModel>
                    <ext:RowSelectionModel ID="RowSelectionModel2" runat="server" SingleSelect="true" />
                </SelectionModel>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingSDTarjetas" runat="server" StoreID="StoreTarjetasSD" DisplayInfo="true"
                        DisplayMsg="Mostrando Tarjetas {0} - {1} de {2}" />
                </BottomBar>
            </ext:GridPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
