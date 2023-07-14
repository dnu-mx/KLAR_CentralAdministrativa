<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" ValidateRequest="false"
    CodeBehind="Reporte_Monitoreo.aspx.cs" Inherits="CentroContacto.Reporte_Monitoreo" %>

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
            <ext:GridPanel ID="GridBitacoraMonitoreo" runat="server" StripeRows="true" Header="false" Border="false"
                Layout="FitLayout" AutoScroll="true">
                <Store>
                    <ext:Store ID="StoreBitacoraMonitoreo" runat="server" RemoteSort="true" AutoLoad="false"
                        OnRefreshData="StoreBitacoraMonitoreo_RefreshData">
                        <AutoLoadParams>
                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                        </AutoLoadParams>
                        <Proxy>
                            <ext:PageProxy />
                        </Proxy>
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="ID_BitacoraMonitoreo">
                                <Fields>
                                    <ext:RecordField Name="ID_BitacoraMonitoreo" />
                                    <ext:RecordField Name="DatoMonitoreo" />
                                    <ext:RecordField Name="FechaDatoMostrado" />
                                    <ext:RecordField Name="FechaDatoCapturado" />                                
                                    <ext:RecordField Name="Usuario" />
                                    <ext:RecordField Name="Valor" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <TopBar>
                    <ext:Toolbar runat="server" LabelWidth="80" LabelAlign="Right">
                        <Items>
                            <ext:DateField ID="dfFIniRepMon" runat="server" Vtype="daterange" MaskRe="[0-9\/]" Width="200"
                                FieldLabel="Fecha Inicial <span style='color:red;'>*   </span>" Format="dd/MM/yyyy"
                                EnableKeyEvents="true" AllowBlank="false" AutoDataBind="true" MaxDate="<%# DateTime.Now %>"
                                InvalidText="Fecha inválida. Debe tener el formato DD/MM/AAAA">
                                <CustomConfig>
                                    <ext:ConfigItem Name="endDateField" Value="#{dfFFinRepMon}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:DateField ID="dfFFinRepMon" runat="server" Vtype="daterange" MaskRe="[0-9\/]" Width="200"
                                FieldLabel="Fecha Final <span style='color:red;'>*   </span>" Format="dd/MM/yyyy"
                                EnableKeyEvents="true" AllowBlank="false" MaxDate="<%# DateTime.Now %>" AutoDataBind="true"
                                InvalidText="Fecha inválida. Debe tener el formato DD/MM/AAAA">
                                <CustomConfig>
                                    <ext:ConfigItem Name="startDateField" Value="#{dfFIniRepMon}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:TextField ID="txtUsuario" runat="server" EmptyText="Usuario" Width="100" />
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                        Before="var valid1 = #{dfFIniRepMon}.isValid(); 
                                        if (!valid1) {
                                            var valid = valid1; }
                                        else {
                                            var valid2 = #{dfFFinRepMon}.isValid();
                                            if (!valid2) {
                                                var valid = valid2; }
                                            else {
                                                resetToolbar(#{PagingRepMonitoreo});
                                                #{GridBitacoraMonitoreo}.getStore().sortInfo = null; }
                                        }
                                        return valid; ">
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
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Datos de Monitoreo...' });
                                        #{GridBitacoraMonitoreo}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                        RepMon.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarFill runat="server" />
                            <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                            e.stopEvent(); 
                                            RepMon.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <ColumnModel ID="ColumnModel1" runat="server">
                    <Columns>
                        <ext:Column DataIndex="ID_BitacoraMonitoreo" Hidden="true" />
                        <ext:Column Header="Dato" Sortable="true" DataIndex="DatoMonitoreo" Width="250"/>
                        <ext:DateColumn Header="Fecha/Hora de Muestra" Sortable="true" DataIndex="FechaDatoMostrado"
                            Format="yyyy-MM-dd HH:mm:ss" Width="150"/>
                        <ext:DateColumn Header="Fecha/Hora de Captura" Sortable="true" DataIndex="FechaDatoCapturado"
                            Format="yyyy-MM-dd HH:mm:ss" Width="150"/>
                        <ext:Column Header="Usuario" Sortable="true" DataIndex="Usuario" Width="150"/>
                        <ext:Column Header="Valor" Sortable="true" DataIndex="Valor" Width="250" />
                    </Columns>
                </ColumnModel>
                <SelectionModel>
                    <ext:RowSelectionModel runat="server" SingleSelect="true" />
                </SelectionModel>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingRepMonitoreo" runat="server" StoreID="StoreBitacoraMonitoreo" DisplayInfo="true"
                        DisplayMsg="Mostrando Datos de Monitoreo {0} - {1} de {2}" HideRefresh="true" />
                </BottomBar>
            </ext:GridPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
