<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" ValidateRequest="false"
    CodeBehind="ReporteAltaYAniversariosTarjetas.aspx.cs" Inherits="Empresarial.ReporteAltaYAniversariosTarjetas" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var onKeyUp = function (field) {
            var v = this.processValue(this.getRawValue()),
                field;

            if (this.startDateField) {
                field = Ext.getCmp(this.startDateField);
                field.setMaxValue();
                this.dateRangeMax = null;
            } else if (this.endDateField) {
                field = Ext.getCmp(this.endDateField);
                field.setMinValue();
                this.dateRangeMin = null;
            }

            field.validate();
        };


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
            <ext:GridPanel ID="GridPanelTarjetas" runat="server" StripeRows="true" Header="false" Border="false"
                Layout="FitLayout" AutoScroll="true">
                <Store>
                    <ext:Store ID="StoreTarjetas" runat="server" RemoteSort="true" AutoLoad="false"
                        OnRefreshData="StoreTarjetas_RefreshData">
                        <AutoLoadParams>
                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                        </AutoLoadParams>
                        <Proxy>
                            <ext:PageProxy />
                        </Proxy>
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="ID">
                                <Fields>
                                    <ext:RecordField Name="ID" />
                                    <ext:RecordField Name="Producto" />
                                    <ext:RecordField Name="Clasificacion" />
                                    <ext:RecordField Name="Total" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <TopBar>
                    <ext:Toolbar runat="server">
                        <Items>
                            <ext:ComboBox ID="cBoxSubemisor" runat="server" EmptyText="SubEmisor" ListWidth="250"
                                Width="150" AllowBlank="false" DisplayField="NombreORazonSocial" ValueField="ID_Colectiva">
                                <Store>
                                    <ext:Store ID="StoreColectivaSub" runat="server">
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
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                                <DirectEvents>
                                    <Select OnEvent="EstableceProductos" Before="#{cBoxProductosSubEm}.clearValue();">
                                        <EventMask ShowMask="true" Msg="Estableciendo Productos..." MinDelay="200" />
                                    </Select>
                                </DirectEvents>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cBoxProductosSubEm" runat="server" EmptyText="Producto"
                                Width="180" AllowBlank="false" DisplayField="Descripcion" ValueField="ID_Producto">
                                <Store>
                                    <ext:Store ID="StoreProductosSubEm" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Producto">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Producto" />
                                                    <ext:RecordField Name="Clave" />
                                                    <ext:RecordField Name="Descripcion" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:DateField ID="dfPeriodo" runat="server" EmptyText="Periodo" Format="MM/yyyy" Width="160"
                                Editable="false" AllowBlank="false" Vtype="daterange">
                                <Plugins>
                                    <ext:MonthPicker runat="server" />
                                </Plugins>
                            </ext:DateField>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                        Before="var valid1 = #{cBoxSubemisor}.isValid(); 
                                        if (!valid1) {
                                            var valid = valid1; }
                                        else {
                                            var valid2 = #{cBoxProductosSubEm}.isValid();
                                            if (!valid2) {
                                                var valid = valid2; }
                                            else {
                                                var valid3 = #{dfPeriodo}.isValid();
                                                if (!valid3) {
                                                    var valid = valid3; }
                                                else {
                                                    resetToolbar(#{PagingTarjetas});
                                                    #{GridPanelTarjetas}.getStore().sortInfo = null; }
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
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Tarjetas...' });
                                        #{GridPanelTarjetas}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                        RepAltAnivTarjetas.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarFill runat="server" />
                            <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a  Excel" Icon="PageExcel" ToolTip="Obtener Datos en un Archivo Excel"
                                Disabled="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                        e.stopEvent(); 
                                        RepAltAnivTarjetas.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <ColumnModel runat="server">
                    <Columns>
                        <ext:Column Hidden="true" DataIndex="ID" />
                        <ext:Column Header="Producto" Sortable="true" DataIndex="Producto" Width="250" />
                        <ext:Column Header="Clasificación" Sortable="true" DataIndex="Clasificacion" Width="350" />
                        <ext:Column Header="Total" Sortable="true" DataIndex="Total" />
                    </Columns>
                </ColumnModel>
                <SelectionModel>
                    <ext:RowSelectionModel runat="server" SingleSelect="true" />
                </SelectionModel>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingTarjetas" runat="server" StoreID="StoreTarjetas" DisplayInfo="true"
                        DisplayMsg="Mostrando Tarjetas {0} - {1} de {2}" HideRefresh="true" />
                </BottomBar>
            </ext:GridPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
