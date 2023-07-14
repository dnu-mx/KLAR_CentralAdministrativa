<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" ValidateRequest="false"
    CodeBehind="Reporte_IngresosDeTarjetas.aspx.cs" Inherits="TpvWeb.Reporte_IngresosDeTarjetas" %>

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
            <ext:FormPanel ID="FormPanelIngresosDeTarjetas" runat="server" Layout="FitLayout">
                <Items>
                    <ext:GridPanel ID="GridPanelIngresosDeTarjetas" runat="server" StripeRows="true" Header="false" Border="false"
                        Layout="FitLayout" AutoScroll="true" AutoExpandColumn="NombreORazonSocial">
                        <LoadMask ShowMask="false" />
                        <Store>
                            <ext:Store ID="StoreIngresosDeTarjetas" runat="server" RemoteSort="true"
                                OnRefreshData="StoreIngresosDeTarjetas_RefreshData" AutoLoad="false">
                                <AutoLoadParams>
                                    <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                </AutoLoadParams>
                                <Proxy>
                                    <ext:PageProxy />
                                </Proxy>
                                <DirectEventConfig IsUpload="true" />
                                <Reader>
                                    <ext:JsonReader IDProperty="Row">
                                        <Fields>
                                            <ext:RecordField Name="Row" />
                                            <ext:RecordField Name="NombreORazonSocial" />
                                            <ext:RecordField Name="ClaveEventoEdoCta" />
                                            <ext:RecordField Name="Mes" />
                                            <ext:RecordField Name="CantOperaciones" />
                                            <ext:RecordField Name="ImporteProm" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <TopBar>
                            <ext:Toolbar ID="Toolbar2" runat="server" LabelWidth="70" LabelAlign="Right">
                                <Items>
                                    <ext:DateField ID="dfFechaInicio" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                        Format="yyyy/MM/dd" Width="200" EnableKeyEvents="true" AllowBlank="false" InvalidText="Fecha inválida. Debe tener el formato AAAA/MM/DD">
                                        <CustomConfig>
                                            <ext:ConfigItem Name="endDateField" Value="#{dfFechaFin}" Mode="Value" />
                                        </CustomConfig>
                                        <Listeners>
                                            <KeyUp Fn="onKeyUp" />
                                        </Listeners>
                                    </ext:DateField>
                                    <ext:DateField ID="dfFechaFin" runat="server" Vtype="daterange" FieldLabel="Fecha Final"
                                        Width="200" Format="yyyy/MM/dd" EnableKeyEvents="true" AllowBlank="false" InvalidText="Fecha inválida. Debe tener el formato AAAA/MM/DD">
                                        <CustomConfig>
                                            <ext:ConfigItem Name="startDateField" Value="#{dfFechaInicio}" Mode="Value" />
                                        </CustomConfig>
                                        <Listeners>
                                            <KeyUp Fn="onKeyUp" />
                                        </Listeners>
                                    </ext:DateField>
                                    <ext:ToolbarSeparator runat="server" />
                                    <ext:Button ID="btnBuscarHide" runat="server" Hidden="true">
                                        <Listeners>
                                            <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Ingreso de Tarjetas ...' });
                                        #{GridPanelIngresosDeTarjetas}.getStore().reload({params:{start:0, sort:('','')}});" />
                                        </Listeners>
                                    </ext:Button>
                                    <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                        <DirectEvents>
                                            <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                                Before="var valid1 = #{dfFechaInicio}.isValid(); 
                                        if (!valid1) {
                                            var valid = valid1; }
                                        else {
                                            var valid2 = #{dfFechaFin}.isValid();
                                            if (!valid2) {
                                                var valid = valid2; }
                                                else {
                                                    resetToolbar(#{PagingStoreIngTarj});
                                                    #{GridPanelIngresosDeTarjetas}.getStore().sortInfo = null; }
                                            }
                                            return valid; ">
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                        <DirectEvents>
                                            <Click OnEvent="Download" IsUpload="true"
                                                After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                            RepIngresosTarjetas.StopMask();" />
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:ToolbarSeparator runat="server" />
                                    <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                        <DirectEvents>
                                            <Click OnEvent="btnLimpiar_Click" />
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:ToolbarSeparator runat="server" />
                                    <ext:ToolbarFill runat="server" />
                                    <ext:ToolbarSeparator runat="server" />
                                    <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="true">
                                        <DirectEvents>
                                            <Click OnEvent="Download" IsUpload="true"
                                                After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                            e.stopEvent(); 
                                            RepIngresosTarjetas.StopMask();" />
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:ToolbarFill runat="server" />
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                        <ColumnModel ID="ColumnModel1" runat="server">
                            <Columns>
                                <ext:NumberColumn Header="#" Width="30" DataIndex="Row" Hidden="true" />
                                <ext:Column Header="Cliente" Width="200" DataIndex="NombreORazonSocial" />
                                <ext:Column Header="Clave Evento" Width="150" DataIndex="ClaveEventoEdoCta" />
                                <ext:Column Header="Mes" Width="100" DataIndex="Mes" />
                                <ext:NumberColumn Header="Cantidad" Width="150" Sortable="true" DataIndex="CantOperaciones" />
                                <ext:Column Header="Importe(Promedio)" Width="120" Sortable="true" DataIndex="ImporteProm">
                                    <Renderer Format="UsMoney" />
                                </ext:Column>
                            </Columns>
                        </ColumnModel>
                        <BottomBar>
                            <ext:PagingToolbar ID="PagingStoreIngTarj" runat="server" StoreID="StoreIngresosDeTarjetas" DisplayInfo="true"
                                DisplayMsg="Mostrando Ingreso de Tarjetas {0} - {1} de {2}" />
                        </BottomBar>
                    </ext:GridPanel>
                </Items>
            </ext:FormPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
