<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="CompensacionesSinOper.aspx.cs" Inherits="TpvWeb.CompensacionesSinOper" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function resetToolbar(tbar) {
            tbar.updateInfo();
            tbar.inputItem.setValue(1);
            tbar.afterTextItem.setText(String.format(tbar.afterPageText, 1));
            tbar.next.setDisabled(true);
            tbar.prev.setDisabled(true);
            tbar.first.setDisabled(true);
            tbar.last.setDisabled(true);
        }
        
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

        var Confirm = function (value1, value2) {
            var title = 'Confirmación de Relación';
            var msg = 'Se relacionará la Compensación de la Tarjeta: <b>' + value1 + '</b></br></br>Con la Operación con ID: <b>' + value2 + '</b></br></br> ¿Es correcto?';

            Ext.Msg.confirm(title, msg, function (btn) {
                if (btn == 'yes') {
                    Ext.net.Mask.show({ msg: 'Estableciendo la Relación...' });
                    RelCSO.btnRelacionarClick();
                    return true;
                } else {
                    return false;
                }
            });
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Hidden ID="hdnIdRegComp" runat="server" />
    <ext:Hidden ID="hdnCardMsk" runat="server" />
    <ext:Hidden ID="hdnIdOp" runat="server" />
    <ext:BorderLayout ID="MainBorderLayout" runat="server">
        <Center Split="true">
            <ext:GridPanel ID="GridPanelRelCSO" runat="server" StripeRows="true" Header="false" Border="false" Layout="FitLayout"
                AutoScroll="true">
                <Store>
                    <ext:Store ID="StoreRelCSO" runat="server" RemoteSort="true" AutoLoad="false" OnRefreshData="StoreRelCSO_RefreshData">
                        <AutoLoadParams>
                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                        </AutoLoadParams>
                        <Proxy>
                            <ext:PageProxy />
                        </Proxy>
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="ID_RegistroCompensacion">
                                <Fields>
                                    <ext:RecordField Name="ID_RegistroCompensacion" />
                                    <ext:RecordField Name="CodigoProceso" />
                                    <ext:RecordField Name="Tarjeta" />
                                    <ext:RecordField Name="ClaveMA" />
                                    <ext:RecordField Name="Autorizacion" />
                                    <ext:RecordField Name="Referencia" />
                                    <ext:RecordField Name="Referencia2" />
                                    <ext:RecordField Name="FechaOperacion" />
                                    <ext:RecordField Name="ImporteOperacion" />
                                    <ext:RecordField Name="MonedaOperacion" />
                                    <ext:RecordField Name="ImporteCompensacion" />
                                    <ext:RecordField Name="MonedaCompensacion" />
                                    <ext:RecordField Name="Comercio" />
                                    <ext:RecordField Name="MCC" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <TopBar>
                    <ext:Toolbar runat="server">
                        <Items>
                            <ext:DateField ID="dfFechaInicio" runat="server" Vtype="daterange" Format="dd/MM/yyyy" Editable="false"
                                EmptyText="Fecha Inicial Presentación" Width="180" AllowBlank="false">
                                <CustomConfig>
                                    <ext:ConfigItem Name="endDateField" Value="#{dfFechaFin}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:DateField ID="dfFechaFin" runat="server" Vtype="daterange" Format="dd/MM/yyyy" Editable="false"
                                EmptyText="Fecha Final Presentación" Width="180">
                                <CustomConfig>
                                    <ext:ConfigItem Name="startDateField" Value="#{dfFechaInicio}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:TextField ID="txtNumTarjeta" runat="server" EmptyText="Número de Tarjeta" MaxLength="16"
                                MinLength="16" Width="150" MaskRe="[0-9]" />
                            <ext:TextField ID="txtReferencia" runat="server" EmptyText="Referencia" MaxLength="25" Width="180"
                                MaskRe="[0-9]" />
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                        Before="var valid = #{dfFechaInicio}.isValid(); if (!valid) {} return valid;">
                                    </Click>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                        Before="if (!#{dfFechaInicio}.isValid()) { return false; }
                                        else { resetToolbar(#{PagingRelCSO});
                                        #{GridPanelRelCSO}.getStore().sortInfo = null; }">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarSeparator runat="server" />
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarSeparator runat="server" />
                            <ext:Button ID="btnBuscarHide" runat="server" Hidden="true">
                                <Listeners>
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Compensaciones...' });
                                        #{GridPanelRelCSO}.getStore().reload({params:{start:0, sort:('','')}});" />
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
                            <ext:ToolbarSeparator runat="server" />
                            <ext:Button ID="btnRelacionar" runat="server" Text="Relacionar" Icon="ChartLine" Disabled="true"
                                Width="100">
                                <Listeners>
                                    <Click Handler="return Confirm(#{hdnCardMsk}.getValue(), #{hdnIdOp}.getValue());" />
                                </Listeners>
                            </ext:Button>
                            <ext:ToolbarSeparator runat="server" />
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <ColumnModel ID="ColumnModel1" runat="server">
                    <Columns>
                        <ext:Column Header="ID_RegistroCompensacion" Hidden="true" />
                        <ext:Column Header="Código de Proceso" Sortable="true" DataIndex="CodigoProceso" Width="120" />
                        <ext:Column Header="Tarjeta" Sortable="true" DataIndex="ClaveMA" Width="120" />
                        <ext:Column Header="Autorización" Sortable="true" DataIndex="Autorizacion" />
                        <ext:Column Header="Referencia" Sortable="true" DataIndex="Referencia" Width="150" />
                        <ext:Column Header="Referencia 2" Sortable="true" DataIndex="Referencia2" Width="150" />
                        <ext:DateColumn Header="Fecha Operación" Sortable="true" DataIndex="FechaOperacion" Format="yyyy-MM-dd" />
                        <ext:Column Header="Importe Operación" Sortable="true" DataIndex="ImporteOperacion" Align="Right" Width="115">
                            <Renderer Format="UsMoney" />
                        </ext:Column>
                        <ext:Column Header="Moneda Operación" Sortable="true" DataIndex="MonedaOperacion" Width="120" Align="Center" />
                        <ext:Column Header="Importe Compensación" Sortable="true" DataIndex="ImporteCompensacion" Align="Right" Width="135">
                            <Renderer Format="UsMoney" />
                        </ext:Column>
                        <ext:Column Header="Moneda Compensación" Sortable="true" DataIndex="MonedaCompensacion" Width="140" Align="Center" />
                        <ext:Column Header="Comercio" Sortable="true" DataIndex="Comercio" Width="200" />
                        <ext:Column Header="MCC" Sortable="true" DataIndex="MCC" />
                    </Columns>
                </ColumnModel>
                <SelectionModel>
                    <ext:RowSelectionModel SingleSelect="true" />
                </SelectionModel>
                <DirectEvents>
                    <RowClick OnEvent="selectRowResultados_Event">
                        <ExtraParams>
                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridPanelRelCSO}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                        </ExtraParams>
                        <EventMask ShowMask="true" Msg="Obteniendo Operaciones..." MinDelay="500" />
                    </RowClick>
                </DirectEvents>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingRelCSO" runat="server" StoreID="StoreRelCSO" DisplayInfo="true"
                        DisplayMsg="Mostrando Compensaciones {0} - {1} de {2}" HideRefresh="true" />
                </BottomBar>
            </ext:GridPanel>  
        </Center>
        <South Split="true">
            <ext:GridPanel ID="GridOpsTransito" runat="server" StripeRows="true" Height="200" Title="Operaciones" Border="false"
                Layout="FitLayout" Disabled="true" Collapsible="true" AutoExpandColumn="Comercio">
                <Store>
                    <ext:Store ID="StoreOpsTransito" runat="server">
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="IdOperacion">
                                <Fields>
                                    <ext:RecordField Name="IdOperacion" />
                                    <ext:RecordField Name="NumTarjeta" />
                                    <ext:RecordField Name="Autorizacion" />
                                    <ext:RecordField Name="FechaOperacion" />
                                    <ext:RecordField Name="ImporteOperacion" />
                                    <ext:RecordField Name="MonedaOperacion" />
                                    <ext:RecordField Name="Comercio" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <DirectEventConfig IsUpload="true" />
                    </ext:Store>
                </Store>
                <ColumnModel runat="server">
                    <Columns>
                        <ext:Column Header="IdOperacion" DataIndex="IdOperacion" Width="100" />
                        <ext:Column Header="Tarjeta" DataIndex="NumTarjeta" Width="150" />
                        <ext:Column Header="Autorización" DataIndex="Autorizacion" Width="100" />
                        <ext:DateColumn Header="Fecha Operación" DataIndex="FechaOperacion" Width="150" Format="yyyy-MM-dd HH:mm:ss" />
                        <ext:Column Header="Importe Operación" DataIndex="ImporteOperacion" Align="Right" Width="120">
                            <Renderer Format="UsMoney" />
                        </ext:Column>
                        <ext:Column Header="Moneda Operación" DataIndex="MonedaOperacion" Align="Center" Width="120" />
                        <ext:Column Header="Comercio" DataIndex="Comercio" Width="300" />
                    </Columns>
                </ColumnModel>
                <SelectionModel>
                    <ext:RowSelectionModel SingleSelect="true" />
                </SelectionModel>
                <Listeners>
                    <RowClick Handler="#{btnRelacionar}.setDisabled(false); var record = #{GridOpsTransito}.getSelectionModel().getSelected();
                        #{hdnIdOp}.setValue(record.get('IdOperacion'));" />
                </Listeners>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingOpsTransitoCSO" runat="server" StoreID="StoreOpsTransito" DisplayInfo="true"
                        DisplayMsg="Mostrando Operaciones {0} - {1} de {2}" HideRefresh="true" />
                </BottomBar>
            </ext:GridPanel>
        </South>
    </ext:BorderLayout>
</asp:Content>
