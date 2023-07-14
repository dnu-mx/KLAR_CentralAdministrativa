<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Reporte_ClientesParco.aspx.cs" Inherits="CentroContacto.Reporte_ClientesParco" %>

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
            <ext:FormPanel ID="FormPanel1" Width="320" Title="Selecciona los Filtros" runat="server" Border="false" Layout="FitLayout">
                <Content>
                    <ext:Hidden ID="FormatType" runat="server" />
                </Content>
                <Items>
                    <ext:Panel ID="Panel1" runat="server" Layout="FitLayout" LabelWidth="120" Border="false" Padding="10">
                        <Items>
                            <ext:DateField ID="datInicio" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                AllowBlank="false" MsgTarget="Side" Format="yyyy-MM-dd" MaxLength="12" Width="300"
                                EnableKeyEvents="true" MaxWidth="300">
                                <CustomConfig>
                                    <ext:ConfigItem Name="endDateField" Value="#{datFinal}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:DateField ID="datFinal" runat="server" Vtype="daterange" FieldLabel="Fecha Final" 
                                AllowBlank="false" MsgTarget="Side" Format="yyyy-MM-dd" MaxLength="12" Width="300"  
                                EnableKeyEvents="true">
                                <CustomConfig>
                                    <ext:ConfigItem Name="startDateField" Value="#{datInicio}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            
                            <ext:TextField ID="txtNombre" runat="server" FieldLabel="Nombre" MaxLength="100" Width="300" />
                            <ext:TextField ID="txtApPaterno" runat="server" FieldLabel="Primer Apellido" MaxLength="100" Width="300" />
                            <ext:TextField ID="txtApMaterno" runat="server" FieldLabel="Segundo Apellido" MaxLength="100" Width="300" />
                            <ext:TextField ID="txtCorreo" runat="server" FieldLabel="Correo Electrónico" MaxLength="100" Width="300" />
                            
                        </Items>
                    </ext:Panel>
                </Items>
                <FooterBar>
                    <ext:Toolbar ID="Toolbar1" runat="server">
                        <Items>
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click" Before="var valid= #{FormPanel1}.getForm().isValid(); if (!valid) {} return valid;" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnBuscarHide" runat="server" Hidden="true">
                                <Listeners>
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Movimientos de Lealtad...' });
                                        #{GridPanelMovimientos}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000" Before="var valid= #{FormPanel1}.getForm().isValid();
                                        if (!valid) { } else { resetToolbar(#{PagingToolBar1});
                                        #{GridPanelMovimientos}.getStore().sortInfo = null; } return valid;">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                            RepClientesParco.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>
            </ext:FormPanel>
        </West>
        <Center Split="true" Collapsible="false">
            <ext:Panel ID="Panel2" runat="server" Title="Movimientos de Lealtad Obtenidos con los Filtros Seleccionados"
                Collapsed="false" Layout="Fit" AutoScroll="true">
                <Content>
                    <ext:Store ID="StoreMovimientos" runat="server" OnSubmitData="StoreSubmit" RemoteSort="true"
                        OnRefreshData="StoreMovimientos_RefreshData" AutoLoad="false">
                        <AutoLoadParams>
                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                        </AutoLoadParams>
                        <Proxy>
                            <ext:PageProxy />
                        </Proxy>
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="IdCliente">
                                <Fields>
                                    <ext:RecordField Name="IdCliente" />
                                    <ext:RecordField Name="IdParco" />
                                    <ext:RecordField Name="Nombre" />
                                    <ext:RecordField Name="apellido_pat" />
                                    <ext:RecordField Name="ApellidoMaterno" />
                                    <ext:RecordField Name="Correo" />
                                    <ext:RecordField Name="Telefono" />
                                    <ext:RecordField Name="FechaRegistro" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <DirectEventConfig IsUpload="true" />
                        <SortInfo Field="FechaRegistro" />
                    </ext:Store>

                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanelMovimientos" runat="server" StoreID="StoreMovimientos" StripeRows="true"
                                Header="false" Border="false">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel1"  runat="server">
                                    <Columns>
                                        <ext:Column ColumnID="IdCliente" Header="IdCliente" Sortable="true" DataIndex="IdCliente" Width="120" />
                                        <ext:Column ColumnID="IdParco" Header="IdParco" Sortable="true" DataIndex="IdParco" Width="120" />
                                        <ext:Column ColumnID="Nombre" Header="Nombre" Sortable="true" DataIndex="Nombre" Width="120" />
                                        <ext:Column ColumnID="apellido_pat" Header="Primer Apellido" Sortable="true" DataIndex="apellido_pat" Width="120" />
                                        <ext:Column ColumnID="ApellidoMaterno" Header="Segundo Apellido" Sortable="true" DataIndex="ApellidoMaterno" Width="120" />
                                        <ext:Column ColumnID="Correo" Header="Correo" Sortable="true" DataIndex="Correo" Width="180" />
                                        <ext:Column ColumnID="Telefono" Header="Telefono" Sortable="true" DataIndex="Telefono" Width="120" />
                                        <ext:DateColumn ColumnID="FechaRegistro" Header="FechaRegistro" Sortable="true" DataIndex="FechaRegistro" Format="yyyy-MM-dd HH:mm" />
                                    </Columns>
                                </ColumnModel>
                                <Plugins>
                                    <ext:GridFilters runat="server" ID="GridFilters1" Local="true">
                                        <Filters>
                                            <ext:DateFilter DataIndex="Fecha">
                                                <DatePickerOptions runat="server" TodayText="Hoy" />
                                            </ext:DateFilter>
                                        </Filters>
                                    </ext:GridFilters>
                                </Plugins>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreMovimientos" DisplayInfo="true"
                                        DisplayMsg="Mostrando Reporte {0} - {1} de {2}" />
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
                                                            RepClientesParco.StopMask();" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="btnExportCSV" runat="server" Text="Exportar a CSV" Icon="PageAttach" Disabled="true">
                                                <Listeners>
                                                    <Click Handler="submitValue(#{GridPanelMovimientos}, #{FormatType}, 'csv');" />
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

