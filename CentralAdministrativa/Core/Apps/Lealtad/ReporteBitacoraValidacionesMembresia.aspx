<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master"
    CodeBehind="ReporteBitacoraValidacionesMembresia.aspx.cs" Inherits="Lealtad.ReporteBitacoraValidacionesMembresia" %>

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
                            <ext:TextField ID="txtMembresia" FieldLabel="Membresia" EmptyText="Todos" AnchorHorizontal="95%"
                                MaxLength="50" Width="275" runat="server" Text="" />
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
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Bitacora Validaciones de Membresia...' });
                                        #{GridDatos}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000" Before="var valid= #{FormPanel1}.getForm().isValid();
                                        if (!valid) {} else { resetToolbar(#{PagingToolBar1});
                                        #{GridDatos}.getStore().sortInfo = null; } return valid;">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                            RepDatos.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>
            </ext:FormPanel>
        </West>
        <Center Split="true" Collapsible="false">
            <ext:Panel ID="Panel2" runat="server" Title="Bitacora Validaciones de Membresia"
                Collapsed="false" Layout="Fit" AutoScroll="true">
                <Content>
                    <ext:Store ID="StoreInfo" runat="server" OnSubmitData="StoreSubmit" RemoteSort="true"
                        OnRefreshData="StoreInfo_RefreshData" AutoLoad="false">
                        <AutoLoadParams>
                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                        </AutoLoadParams>
                        <Proxy>
                            <ext:PageProxy />
                        </Proxy>
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="id">
                                <Fields>
                                    <ext:RecordField Name="id" />
                                    <ext:RecordField Name="Membresia" />
                                    <ext:RecordField Name="Codigo" />
                                    <ext:RecordField Name="Nivel" />
                                    <ext:RecordField Name="Fecha_Renovacion" />
                                    <ext:RecordField Name="Fecha_Insert" />
                                    <ext:RecordField Name="MembresiaXref" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <DirectEventConfig IsUpload="true" />
                    </ext:Store>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridDatos" runat="server" StoreID="StoreInfo" StripeRows="true" Header="false" Border="false">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                        <ext:Column Header="Membresia" Sortable="true" Width="150px" DataIndex="Membresia" />
                                        <ext:Column Header="Codigo" Sortable="true" Width="200px" DataIndex="Codigo" />
                                        <ext:Column Header="Nivel" Sortable="true" Width="150px" DataIndex="Nivel" />
                                        <ext:DateColumn Header="Fecha Renovacion" Width="150px" Sortable="true" DataIndex="Fecha_Renovacion" Format="yyyy-MM-dd" />
                                        <ext:DateColumn Header="Fecha Insert" Width="150px" Sortable="true" DataIndex="Fecha_Insert" Format="yyyy-MM-dd" />
                                        <ext:Column Header="MembresiaXref" Width="150px" Sortable="true" DataIndex="MembresiaXref" />
                                    </Columns>
                                </ColumnModel>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreInfo" DisplayInfo="true"
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
                                                            RepDatos.StopMask();" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="btnExportCSV" runat="server" Text="Exportar a CSV" Icon="PageAttach" Disabled="true">
                                                <Listeners>
                                                    <Click Handler="submitValue(#{GridDatos}, #{FormatType}, 'csv');" />
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
