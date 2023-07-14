<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" ValidateRequest="false"
    CodeBehind="ReporteMediosAcceso.aspx.cs" Inherits="Empresarial.ReporteMediosAcceso" %>

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
            <ext:GridPanel ID="GridPanelMA" runat="server" StripeRows="true" Header="false" Border="false"
                Layout="FitLayout" AutoScroll="true">
                <Store>
                    <ext:Store ID="StoreMediosAcceso" runat="server" RemoteSort="true" AutoLoad="false"
                        OnRefreshData="StoreMediosAcceso_RefreshData" GroupField="Tarjeta">
                        <AutoLoadParams>
                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                        </AutoLoadParams>
                        <Proxy>
                            <ext:PageProxy />
                        </Proxy>
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="ID_MedioAcceso">
                                <Fields>
                                    <ext:RecordField Name="ID_Tarjeta" />
                                    <ext:RecordField Name="Tarjeta" />
                                    <ext:RecordField Name="NombreTarjetahabiente" />
                                    <ext:RecordField Name="RFC" />
                                    <ext:RecordField Name="CURP" />
                                    <ext:RecordField Name="EstatusTarjeta" />
                                    <ext:RecordField Name="TipoManufactura" />
                                    <ext:RecordField Name="SaldoDisponible" />
                                    <ext:RecordField Name="ID_MedioAcceso" />
                                    <ext:RecordField Name="MedioAcceso" />
                                    <ext:RecordField Name="EstatusMedioAcceso" />
                                    <ext:RecordField Name="ClaveMA" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <TopBar>
                    <ext:Toolbar ID="Toolbar2" runat="server">
                        <Items>
                            <ext:ComboBox ID="cBoxTipoColectiva" runat="server" EmptyText="Tipo de Colectiva" 
                                Width="150" AllowBlank="false" DisplayField="Descripcion" ValueField="ID_TipoColectiva">
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
                                    <Select OnEvent="EstableceColectivas" Before="#{cBoxCliente}.clearValue(); 
                                        #{txtTarjeta}.reset();" After="#{cBoxCliente}.setDisabled(false);">
                                        <EventMask ShowMask="true" Msg="Estableciendo Colectivas..." MinDelay="200" />
                                    </Select>
                                </DirectEvents>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cBoxCliente" runat="server" EmptyText="Selecciona la Colectiva..." Disabled="true"
                                Width="180" AllowBlank="false" DisplayField="NombreORazonSocial" ValueField="ID_Colectiva">
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
                                <Listeners>
                                    <Select Handler="#{txtTarjeta}.setDisabled(false);"/>
                                </Listeners>
                            </ext:ComboBox>
                            <ext:TextField ID="txtTarjeta" runat="server" EmptyText="Número de Tarjeta" AllowBlank="false"
                                 Width="180" MaskRe="[0-9]" MaxLength="16" MinLength="16" Disabled="true" />
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                        Before="var valid1 = #{cBoxTipoColectiva}.isValid();
                                        if (!valid1) { var valid = valid1; } else { var valid2 = #{cBoxCliente}.isValid();
                                        if (!valid2) { var valid = valid2; } else { var valid3 = #{txtTarjeta}.isValid();
                                        if (!valid3) { var valid = valid3; } else { resetToolbar(#{PagingMAs});
                                        #{GridPanelMA}.getStore().sortInfo = null; }}} return valid; ">
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
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Medios de Acceso...' });
                                        #{GridPanelMA}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                        RepMediosAcceso.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarFill runat="server" />
                            <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a  Excel" Icon="PageExcel" ToolTip="Obtener Datos en un Archivo Excel"
                                Disabled="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                        e.stopEvent(); 
                                        RepMediosAcceso.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarSeparator runat="server" />
                            <ext:Button ID="btnReporte" runat="server" Text="Reporte" Icon="PageExcel" ToolTip="Obtener Datos en un Archivo Excel"
                                Disabled="true">
                                <DirectEvents>
                                    <Click OnEvent="btnReporte_click" Timeout="360000">
                                        <EventMask ShowMask="true" Msg="Solicitado Reporte..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="ExportaReporte" IsUpload="true" Timeout="360000"
                                       After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                        RepMediosAcceso.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <ColumnModel ID="ColumnModel1" runat="server">
                    <Columns>
                        <ext:GroupingSummaryColumn Header="Tarjeta" Sortable="true" DataIndex="Tarjeta" />
                        <ext:GroupingSummaryColumn Header="Nombre" Sortable="true" DataIndex="NombreTarjetahabiente"
                            Width="150" />
                        <ext:GroupingSummaryColumn Header="RFC" Sortable="true" DataIndex="RFC" />
                        <ext:GroupingSummaryColumn Header="CURP" Sortable="true" DataIndex="CURP" />
                        <ext:GroupingSummaryColumn Header="Estatus Tarjeta" Sortable="true" DataIndex="EstatusTarjeta" />
                        <ext:GroupingSummaryColumn Header="Tipo Manufactura" Sortable="true" DataIndex="TipoManufactura" />
                        <ext:NumberColumn Header="Saldo Disponible" Sortable="true" DataIndex="SaldoDisponible"
                            Width="100" Format="$0,0.00">
                            <Renderer Format="UsMoney" />
                        </ext:NumberColumn>
                        <ext:GroupingSummaryColumn Header="Medio de Acceso" Sortable="true" DataIndex="MedioAcceso" />
                        <ext:GroupingSummaryColumn Header="Estatus Medio Acceso" Sortable="true" DataIndex="EstatusMedioAcceso"
                            Width="130" />
                        <ext:GroupingSummaryColumn Header="No Medio Acceso" Sortable="true" DataIndex="ClaveMA" />
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
                    <ext:PagingToolbar ID="PagingMAs" runat="server" StoreID="StoreMediosAcceso" DisplayInfo="true"
                        DisplayMsg="Mostrando Medios de Acceso {0} - {1} de {2}" />
                </BottomBar>
            </ext:GridPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
