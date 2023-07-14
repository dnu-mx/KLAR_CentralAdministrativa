<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master"
    CodeBehind="ReporteSubscripcionNewsLetterPrana.aspx.cs" Inherits="Lealtad.ReporteSubscripcionNewsLetterPrana" %>

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
        <West Split="true" Collapsible="true">
            <ext:FormPanel ID="FormPanel1" Width="320" Title="Selecciona los Filtros" runat="server"
                Border="false" Layout="FormLayout" Padding="10">
                <Content>
                    <ext:Hidden ID="FormatType" runat="server" />
                </Content>
                <Items>                    
                    <ext:Panel runat="server" Title="Periodo" Padding="3" FormGroup="true">
                        <Items>
                            <ext:DateField ID="dfFechaInicio" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                Format="yyyy/MM/dd" Width="275" EnableKeyEvents="true" AllowBlank="false">
                                <CustomConfig>
                                    <ext:ConfigItem Name="endDateField" Value="#{dfFechaFin}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:DateField ID="dfFechaFin" runat="server" Vtype="daterange" FieldLabel="Fecha Final"
                                Width="275" Format="yyyy/MM/dd" EnableKeyEvents="true" AllowBlank="false">
                                <CustomConfig>
                                    <ext:ConfigItem Name="startDateField" Value="#{dfFechaInicio}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
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
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Subcripciones NewsLetter...' });
                                        #{GridPanelNewsLetter}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000" Before="var valid= #{FormPanel1}.getForm().isValid();
                                        if (!valid) {} else { resetToolbar(#{PagingToolBar1});
                                        #{GridPanelNewsLetter}.getStore().sortInfo = null; } return valid;">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                            RepNewsLetter.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>
            </ext:FormPanel>
        </West>
        <Center Split="true" Collapsible="false">
            <ext:Panel ID="Panel2" runat="server" Title="Suscripciones Newsletter por Periodo Seleccionado"
                Collapsed="false" Layout="Fit" AutoScroll="true">
                <Content>
                    <ext:Store ID="StoreNewsLetter" runat="server" OnSubmitData="StoreSubmit" RemoteSort="true"
                        OnRefreshData="StoreNewsLetter_RefreshData" AutoLoad="false">
                        <AutoLoadParams>
                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                        </AutoLoadParams>
                        <Proxy>
                            <ext:PageProxy />
                        </Proxy>
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Suscripcion">
                                <Fields>
                                    <ext:RecordField Name="ID_Suscripcion" />
                                    <ext:RecordField Name="Clave" />
                                    <ext:RecordField Name="Correo" />
                                    <ext:RecordField Name="Estatus" />
                                    <ext:RecordField Name="Fecha_Insertado" />
                                    <ext:RecordField Name="FechaRegistro" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <DirectEventConfig IsUpload="true" />
                    </ext:Store>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanelNewsLetter" runat="server" StoreID="StoreNewsLetter" StripeRows="true" Header="false" Border="false">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                        <ext:Column Header="ID Suscripción" Sortable="true" DataIndex="ID_Suscripcion" />
                                        <ext:Column Header="Clave" Sortable="true" Width="150px" DataIndex="Clave" />
                                        <ext:Column Header="Correo" Sortable="true" Width="250px" DataIndex="Correo" />
                                        <ext:DateColumn Header="Fecha Insertado" Sortable="true" DataIndex="Fecha_Insertado" Format="yyyy-MM-dd" />
                                        <ext:DateColumn Header="Fecha Registro" Sortable="true" DataIndex="FechaRegistro" Format="yyyy-MM-dd" />
                                    </Columns>
                                </ColumnModel>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreNewsLetter" DisplayInfo="true"
                                        DisplayMsg="Mostrando NewsLetter {0} - {1} de {2}" />
                                </BottomBar>
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar2" runat="server">
                                        <Items>
                                            <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                                            <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="true">
                                                <DirectEvents>
                                                    <Click OnEvent="Download" IsUpload="true"
                                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                            e.stopEvent(); 
                                                            RepNewsLetter.StopMask();" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="btnExportCSV" runat="server" Text="Exportar a CSV" Icon="PageAttach" Disabled="true">
                                                <Listeners>
                                                    <Click Handler="submitValue(#{GridPanelNewsLetter}, #{FormatType}, 'csv');" />
                                                </Listeners>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
