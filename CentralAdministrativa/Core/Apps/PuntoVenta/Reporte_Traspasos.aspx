<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" ValidateRequest="false"
    CodeBehind="Reporte_Traspasos.aspx.cs" Inherits="TpvWeb.Reporte_Traspasos" %>

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
            <ext:GridPanel ID="GridPanelTraspasos" runat="server" StripeRows="true" Header="false" Border="false"
                Layout="FitLayout" AutoScroll="true">
                <Store>
                    <ext:Store ID="StoreTraspasos" runat="server" RemoteSort="true" AutoLoad="false"
                        OnRefreshData="StoreTraspasos_RefreshData">
                        <AutoLoadParams>
                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                        </AutoLoadParams>
                        <Proxy>
                            <ext:PageProxy />
                        </Proxy>
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Poliza">
                                <Fields>
                                    <ext:RecordField Name="ID_Poliza" />
                                    <ext:RecordField Name="FechaOperacion" />
                                    <ext:RecordField Name="CuentaCacaoOrigen" />
                                    <ext:RecordField Name="TipoMovimiento" />
                                    <ext:RecordField Name="Concepto" />
                                    <ext:RecordField Name="Observaciones" />
                                    <ext:RecordField Name="ReferenciaNumerica" />
                                    <ext:RecordField Name="MontoOperacion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <TopBar>
                    <ext:Toolbar ID="Toolbar2" runat="server" LabelWidth="70" LabelAlign="Right">
                        <Items>
                            <ext:ComboBox ID="cBoxCliente" runat="server" EmptyText="Selecciona el SubEmisor..."
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
                                        <SortInfo Field="NombreORazonSocial" Direction="ASC" />
                                    </ext:Store>
                                </Store>
                            </ext:ComboBox>
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
                            <ext:TextField ID="txtMedioAcceso" runat="server" EmptyText="Medio de Acceso" Width="150"
                                MaskRe="[0-9]" MaxLength="18" MinLength="8" AllowBlank="false" />
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                        Before="var valid1 = #{cBoxCliente}.isValid(); 
                                        if (!valid1) {
                                            var valid = valid1; }
                                        else {
                                            var valid2 = #{dfFechaInicio}.isValid();
                                            if (!valid2) {
                                                var valid = valid2; }
                                            else {
                                                var valid3 = #{dfFechaFin}.isValid();
                                                if (!valid3) {
                                                    var valid = valid3; }
                                                else {
                                                    var valid4 = #{txtMedioAcceso}.isValid();
                                                    if (!valid4) {
                                                        var valid = valid4; }
                                                    else {
                                                        resetToolbar(#{PagingTraspasos});
                                                        #{GridPanelTraspasos}.getStore().sortInfo = null; }
                                                }
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
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Traspasos...' });
                                        #{GridPanelTraspasos}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                        RepActTraspasos.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarFill runat="server" />
                            <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a  Excel" Icon="PageExcel" ToolTip="Obtener Datos en un Archivo Excel"
                                Disabled="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                        e.stopEvent(); 
                                        RepActTraspasos.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <ColumnModel ID="ColumnModel1" runat="server">
                    <Columns>
                        <ext:DateColumn Header="Fecha Operacion" Sortable="true" DataIndex="FechaOperacion" Format="yyyy-MM-dd HH:mm:ss" Width="70px" />
                        <ext:Column Header="Cuenta Interna Origen" Sortable="true" DataIndex="CuentaCacaoOrigen" Width="80px" />
                        <ext:Column Header="Tipo Movimiento" Sortable="true" DataIndex="TipoMovimiento" Width="70px" />
                        <ext:Column Header="Concepto" Sortable="true" DataIndex="Concepto" Width="200px" />
                        <ext:Column Header="Observaciones" Sortable="true" DataIndex="Observaciones" Width="110px" />
                        <ext:Column Header="Referencia Numérica" Sortable="true" DataIndex="ReferenciaNumerica" Width="70px" />
                        <ext:NumberColumn Header="Monto Operacion" Sortable="true" DataIndex="MontoOperacion" Width="50px" Align="Right" Format="0,0.00"/>
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
                    <ext:PagingToolbar ID="PagingTraspasos" runat="server" StoreID="StoreTraspasos" DisplayInfo="true"
                        DisplayMsg="Mostrando Traspasos {0} - {1} de {2}" HideRefresh="true" />
                </BottomBar>
            </ext:GridPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
