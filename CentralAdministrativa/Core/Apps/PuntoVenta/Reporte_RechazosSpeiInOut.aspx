<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" ValidateRequest="false"
    CodeBehind="Reporte_RechazosSpeiInOut.aspx.cs" Inherits="TpvWeb.Reporte_RechazosSpeiInOut" %>

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
            <ext:GridPanel ID="GridPanelRechazoSpeiInOut" runat="server" StripeRows="true" Header="false" Border="false"
                Layout="FitLayout" AutoScroll="true" AutoExpandColumn="Mensaje">
                <Store>
                    <ext:Store ID="StoreRechazosSpeiInOut" runat="server" RemoteSort="true" AutoLoad="false"
                        OnRefreshData="StoreRechazosSpeiInOut_RefreshData">
                        <AutoLoadParams>
                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                        </AutoLoadParams>
                        <Proxy>
                            <ext:PageProxy />
                        </Proxy>
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="ID_RegistroSTP">
                                <Fields>
                                    <ext:RecordField Name="ID_RegistroSTP" />
                                    <ext:RecordField Name="FechaOperacion" />
                                    <ext:RecordField Name="FechaRechazo" />
                                    <ext:RecordField Name="EsSpeiIn" />
                                    <ext:RecordField Name="CuentaClabeBeneficiario" />
                                    <ext:RecordField Name="CuentaClabeOrdenante" />
                                    <ext:RecordField Name="ClaveRastreo" />
                                    <ext:RecordField Name="CodigoError" />
                                    <ext:RecordField Name="Mensaje" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <TopBar>
                    <ext:Toolbar ID="Toolbar2" runat="server" LabelWidth="70" LabelAlign="Right">
                        <Items>
                            <ext:DateField ID="dfFechaInicio" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                Format="dd/MM/yyyy" Width="160" EnableKeyEvents="true" AllowBlank="false">
                                <CustomConfig>
                                    <ext:ConfigItem Name="endDateField" Value="#{dfFechaFin}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:DateField ID="dfFechaFin" runat="server" Vtype="daterange" FieldLabel="Fecha Final"
                                Width="160" Format="dd/MM/yyyy" EnableKeyEvents="true" AllowBlank="false">
                                <CustomConfig>
                                    <ext:ConfigItem Name="startDateField" Value="#{dfFechaInicio}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:ComboBox ID="cBoxSpei" runat="server" EmptyText="Tipo Spei" Width="80"
                                AllowBlank="false">
                                <Items>
                                    <ext:ListItem Text="Todos" Value="-1" />
                                    <ext:ListItem Text="Spei In" Value="1" />
                                    <ext:ListItem Text="Spei Out" Value="0" />
                                </Items>
                            </ext:ComboBox>
                            <ext:TextField ID="txtClaveRastreo" runat="server" EmptyText="Clave de Rastreo" Width="190"
                                  MaxLength="30" />
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
                                                var valid3 = #{cBoxSpei}.isValid();
                                                if (!valid3) {
                                                    var valid = valid3; }
                                                else {
                                                    resetToolbar(#{PagingRechazoSpeiInOut});
                                                    #{GridPanelRechazoSpeiInOut}.getStore().sortInfo = null; }
                                                }
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
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Rechazos SPEI...' });
                                        #{GridPanelRechazoSpeiInOut}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                        RepRechSpei.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarFill runat="server"/>
                            <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a  Excel" Icon="PageExcel" ToolTip="Obtener Datos en un Archivo Excel"
                                Disabled="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                        e.stopEvent(); 
                                        RepRechSpei.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarFill runat="server" />
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <ColumnModel ID="ColumnModel1" runat="server">
                    <Columns>
                        <ext:Column Header="#" DataIndex="ID_RegistroSTP" Width="60" Align="Left" />
                        <ext:DateColumn Header="Fecha de Operación" Width="120" DataIndex="FechaOperacion" Align="Center"
                            Format="yyyy-MM-dd HH:mm:ss" />
                        <ext:DateColumn Header="Fecha de Rechazo" Width="120" DataIndex="FechaRechazo" Align="Center"
                            Format="yyyy-MM-dd HH:mm:ss" />
                        <ext:Column Header="Tipo Spei" Sortable="true" DataIndex="EsSpeiIn" Width="65" />
                        <ext:Column Header="Cuenta Clabe Beneficiario" Width="140" DataIndex="CuentaClabeBeneficiario" />
                        <ext:Column Header="Cuenta Clabe Ordenante" Width="140" DataIndex="CuentaClabeOrdenante" />
                        <ext:Column Header="Clave de Rastreo" Width="160" DataIndex="ClaveRastreo" />
                        <ext:Column Header="Codigo de Error" Width="90" DataIndex="CodigoError" />
                        <ext:Column Header="Mensaje" DataIndex="Mensaje" />
                    </Columns>
                </ColumnModel>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingRechazoSpeiInOut" runat="server" StoreID="StoreRechazosSpeiInOut" DisplayInfo="true"
                        DisplayMsg="Mostrando Rechazos Spei In/Out {0} - {1} de {2}" />
                </BottomBar>
            </ext:GridPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
