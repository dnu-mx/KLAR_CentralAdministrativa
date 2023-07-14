<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="CompensacionManual.aspx.cs" Inherits="TpvWeb.CompensacionManual" %>

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

        //var onBefore = function (record, monto, emisor) {
        //    monto.setValue(record.get("MontoTX"));
        //    emisor.setValue(record.get("ID_Emisor"));
        //};

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
    <ext:Hidden ID="hdnIdFicheroDetalle" runat="server" />
    <ext:Hidden ID="hdnTarjeta" runat="server" />
    <%--<ext:Hidden ID="hdnAutorizacion" runat="server" />--%>
    <ext:Hidden ID="hdnCodigoOperacion" runat="server" />
    <ext:Hidden ID="hdnMonto" runat="server" />
    <ext:Hidden ID="hdnEmisor" runat="server" />
    <ext:Window ID="WdwDetalleOp" runat="server" Width="350" AutoHeight="true" Hidden="true" Modal="true" Resizable="false"
        Closable="true" Title="Detalle de la Operación #">
        <Items>
            <ext:FormPanel runat="server" Border="false" LabelWidth="200" Padding="10">
                <Items>
                    <ext:FormPanel runat="server" Layout="ColumnLayout" Border="false" Height="25">
                        <Items>
                            <ext:Panel runat="server" Border="false" Width="150">
                                <Items>
                                    <ext:Label runat="server" FieldLabel="Número de Tarjeta" />
                                </Items>
                            </ext:Panel>
                            <ext:Panel runat="server" Border="false" Width="150" StyleSpec="text-align:right;font-weight:bold;font-family:segoe ui;font-size: 12px;"> 
                                <Items>
                                    <ext:Label ID="lblNumTarjeta" runat="server" Text="1234567890123456" LabelSeparator=" " />
                                </Items>
                            </ext:Panel>
                        </Items>
                    </ext:FormPanel>
                    <ext:FormPanel runat="server" Layout="ColumnLayout" Border="false" Height="25">
                        <Items>
                            <ext:Panel runat="server" Border="false" Width="150">
                                <Items>
                                    <ext:Label runat="server" FieldLabel="Código de Movimiento" />
                                </Items>
                            </ext:Panel>
                            <ext:Panel runat="server" Border="false" Width="150" StyleSpec="text-align:right;font-weight:bold;font-family:segoe ui;font-size: 12px;"> 
                                <Items>
                                    <ext:Label ID="lblCodMov" runat="server" Text="0312469879" LabelSeparator=" " />
                                </Items>
                            </ext:Panel>
                        </Items>
                    </ext:FormPanel>
                    <ext:FormPanel runat="server" Layout="ColumnLayout" Border="false" Height="25">
                        <Items>
                            <ext:Panel runat="server" Border="false" Width="150">
                                <Items>
                                    <ext:Label runat="server" FieldLabel="Fecha de Operación T112" />
                                </Items>
                            </ext:Panel>
                            <ext:Panel runat="server" Border="false" Width="150" StyleSpec="text-align:right;font-weight:bold;font-family:segoe ui;font-size: 12px;"> 
                                <Items>
                                    <ext:Label ID="lblFechaOp" runat="server" Text="30/06/2020" LabelSeparator=" " />
                                </Items>
                            </ext:Panel>
                        </Items>
                    </ext:FormPanel>
                    <ext:FormPanel runat="server" Layout="ColumnLayout" Border="false" Height="25">
                        <Items>
                            <ext:Panel runat="server" Border="false" Width="150">
                                <Items>
                                    <ext:Label runat="server" FieldLabel="Importe" />
                                </Items>
                            </ext:Panel>
                            <ext:Panel runat="server" Border="false" Width="150" StyleSpec="text-align:right;font-weight:bold;font-family:segoe ui;font-size: 12px;"> 
                                <Items>
                                    <ext:Label ID="lblImporte" runat="server" Text="$9,2345.50" LabelSeparator=" " />
                                </Items>
                            </ext:Panel>
                        </Items>
                    </ext:FormPanel>
                    <ext:FormPanel runat="server" Layout="ColumnLayout" Border="false" Height="25">
                        <Items>
                            <ext:Panel runat="server" Border="false" Width="150">
                                <Items>
                                    <ext:Label runat="server" FieldLabel="No. Autorización" />
                                </Items>
                            </ext:Panel>
                            <ext:Panel runat="server" Border="false" Width="150" StyleSpec="text-align:right;font-weight:bold;font-family:segoe ui;font-size: 12px;"> 
                                <Items>
                                    <ext:Label ID="lblAutorizacion" runat="server" Text="03457892311" LabelSeparator=" " />
                                </Items>
                            </ext:Panel>
                        </Items>
                    </ext:FormPanel>
                    <ext:FormPanel runat="server" Layout="ColumnLayout" Border="false" Height="25">
                        <Items>
                            <ext:Panel runat="server" Border="false" Width="150">
                                <Items>
                                    <ext:Label runat="server" FieldLabel="No. Documento" />
                                </Items>
                            </ext:Panel>
                            <ext:Panel runat="server" Border="false" Width="150" StyleSpec="text-align:right;font-weight:bold;font-family:segoe ui;font-size: 12px;"> 
                                <Items>
                                    <ext:Label ID="lblDocumento" runat="server" Text="894672" LabelSeparator=" " />
                                </Items>
                            </ext:Panel>
                        </Items>
                    </ext:FormPanel>
                </Items>
                <Buttons>
                    <ext:Button runat="server" Text="Cancelar" Icon="Cancel">
                        <Listeners>
                            <Click Handler="#{WdwDetalleOp}.hide();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button runat="server" Text="Compensar" Icon="Tick">
                       <%-- <DirectEvents>
                            <Click OnEvent="btnGuardarValorParametro_Click" />
                        </DirectEvents>--%>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center Split="true">
            <ext:Panel runat="server" Width="320">
                <Items>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <West Split="true">
                            <ext:FormPanel ID="FormPanelBusqueda" Width="320" Title="Selecciona los filtros" runat="server" Padding="10"
                                Border="false" Layout="FitLayout" LabelWidth="100">
                                <Items>
                                    <ext:ComboBox ID="cBoxCC" runat="server" FieldLabel="SubEmisor <span style='color:red;'>*   </span>"
                                        Width="300" DisplayField="NombreORazonSocial" ValueField="ID_Colectiva" Mode="Local" AutoSelect="true"
                                        ForceSelection="true" TypeAhead="true" MinChars="1" MatchFieldWidth="false"
                                        AllowBlank="false">
                                        <Store>
                                            <ext:Store ID="StoreCC" runat="server">
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
                                    <ext:DateField ID="dfFechaInicial" runat="server" Vtype="daterange" MaskRe="[0-9\/]" Width="300"
                                        FieldLabel="Fecha Inicial <span style='color:red;'>*   </span>" Format="dd/MM/yyyy"
                                        EnableKeyEvents="true" AllowBlank="false" AutoDataBind="true" MaxDate="<%# DateTime.Now %>"
                                        InvalidText="Fecha inválida. Debe tener el formato DD/MM/AAAA">
                                        <CustomConfig>
                                            <ext:ConfigItem Name="endDateField" Value="#{dfFechaFinal}" Mode="Value" />
                                        </CustomConfig>
                                        <Listeners>
                                            <KeyUp Fn="onKeyUp" />
                                        </Listeners>
                                    </ext:DateField>
                                    <ext:DateField ID="dfFechaFinal" runat="server" Vtype="daterange" MaskRe="[0-9\/]" Width="300"
                                        FieldLabel="Fecha Final <span style='color:red;'>*   </span>" Format="dd/MM/yyyy"
                                        EnableKeyEvents="true" AllowBlank="false" MaxDate="<%# DateTime.Now %>" AutoDataBind="true"
                                        InvalidText="Fecha inválida. Debe tener el formato DD/MM/AAAA">
                                        <CustomConfig>
                                            <ext:ConfigItem Name="startDateField" Value="#{dfFechaInicial}" Mode="Value" />
                                        </CustomConfig>
                                        <Listeners>
                                            <KeyUp Fn="onKeyUp" />
                                        </Listeners>
                                    </ext:DateField>
                                    <ext:TextField ID="txtTarjeta" runat="server" FieldLabel="Tarjeta" EmptyText="Todas"
                                        Width="300" MaskRe="[0-9]" MinLength="16" MaxLength="16" />
                                    <ext:Panel runat="server" Layout="FitLayout" Height="20" Border="false" />
                                    <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false" LabelWidth="5">
                                        <Defaults>
                                            <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                        </Defaults>
                                        <LayoutConfig>
                                            <ext:HBoxLayoutConfig Align="Top" />
                                        </LayoutConfig>
                                        <Items>
                                            <ext:Label runat="server" LabelSeparator=" " Width="200" />
                                            <ext:Hidden runat="server" Flex="1" Width="25" />
                                            <ext:Label runat="server" FieldLabel="<span style='color:red;'>*   </span>"
                                                Text="Obligatorios" LabelSeparator=" " StyleSpec="font-style: italic;font-family:segoe ui;font-size: 11px;" />
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
                                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Operaciones...' });
                                                        #{GridOpsComp}.getStore().reload({params:{start:0, sort:('','')}});" />
                                                </Listeners>
                                            </ext:Button>
                                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                                        Before="if (!#{FormPanelBusqueda}.getForm().isValid()) { return false; }
                                                            else { resetToolbar(#{PagingOpsComp});
                                                            #{GridOpsComp}.getStore().sortInfo = null; }">
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="btnOpsPorCompHide" runat="server" Hidden="true">
                                                <Listeners>
                                                    <Click Handler="resetToolbar(#{PagingTB_OpsXComp});
                                                        #{GridOpsPorComp}.getStore().sortInfo = null;
                                                        #{GridOpsPorComp}.getStore().reload({params:{start:0, sort:('','')}});" />
                                                </Listeners>
                                                <DirectEvents>
                                                    <Click Timeout="360000" />
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </FooterBar>
                            </ext:FormPanel>
                        </West>
                        <Center Split="true">
                            <ext:GridPanel ID="GridOpsComp" runat="server" StripeRows="true" Border="false" Layout="FitLayout"
                                Title="Operaciones obtenidas con los filtros seleccionados">
                                <LoadMask ShowMask="false" />
                                <Store>
                                    <ext:Store ID="StoreOpsComp" runat="server" RemoteSort="true" OnRefreshData="StoreOpsComp_RefreshData"
                                        AutoLoad="false">
                                        <AutoLoadParams>
                                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                        </AutoLoadParams>
                                        <Proxy>
                                            <ext:PageProxy />
                                        </Proxy>
                                        <DirectEventConfig IsUpload="true" />
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_FicheroDetalle">
                                                <Fields>
                                                    <ext:RecordField Name="ID_FicheroDetalle" />
                                                    <ext:RecordField Name="CodigoTX" />
                                                    <ext:RecordField Name="ClaveOperacion" />
                                                    <ext:RecordField Name="Tarjeta" />
                                                    <ext:RecordField Name="Referencia" />
                                                    <ext:RecordField Name="FechaTX" />
                                                    <ext:RecordField Name="MonedaTX" />
                                                    <ext:RecordField Name="MontoTX" />
                                                    <ext:RecordField Name="MontoMN" />
                                                    <ext:RecordField Name="MontoCompensado" />
                                                    <ext:RecordField Name="NumAutorizacion" />
                                                    <ext:RecordField Name="Comercio" />
                                                    <ext:RecordField Name="NumOperacionesCoinciden" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <ColumnModel runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_FicheroDetalle" Hidden="true" />
                                        <ext:Column DataIndex="CodigoTX" Hidden="true" />
                                        <ext:Column Header="Clave Operación" Sortable="true" DataIndex="ClaveOperacion" />
                                        <ext:Column Header="Tarjeta" Sortable="true" DataIndex="Tarjeta" Width="150"/>
                                        <ext:Column Header="Referencia" Sortable="true" DataIndex="Referencia" Width="180"/>
                                        <ext:DateColumn Header="Fecha TX" Sortable="true" DataIndex="FechaTX"
                                            Format="yyyy-MM-dd HH:mm:ss" Width="150" />
                                        <ext:Column Header="Moneda TX" Sortable="true" DataIndex="MonedaTX" />
                                        <ext:Column Header="Monto TX" Sortable="true" DataIndex="MontoTX">
                                            <Renderer Format="UsMoney" />
                                        </ext:Column>
                                        <ext:Column Header="Monto Moneda Nacional" Sortable="true" DataIndex="MontoMN"
                                            Width="150">
                                            <Renderer Format="UsMoney" />
                                        </ext:Column>
                                        <ext:Column Header="Monto Compensado" Sortable="true" DataIndex="MontoCompensado"
                                            Width="150">
                                            <Renderer Format="UsMoney" />
                                        </ext:Column>
                                        <ext:Column Header="No. Autorización" Sortable="true" DataIndex="NumAutorizacion" />
                                        <ext:Column Header="Comercio" Sortable="true" DataIndex="Comercio" Width="200" />
                                        <ext:Column Header="Operaciones que Coinciden" Sortable="true" Width="200"
                                            DataIndex="NumOperacionesCoinciden" />
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel SingleSelect="true" />
                                </SelectionModel>
                                 <DirectEvents>
                                    <RowClick OnEvent="selectRowOperaciones_Event" Before="var record = #{GridOpsComp}.getSelectionModel().getSelected();
                                        #{hdnMonto}.setValue(record.get('MontoCompensado'));">
                                        <EventMask ShowMask="true" Msg="Obteniendo Operaciones para Compensación..." MinDelay="500" />
                                        <ExtraParams>
                                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridOpsComp}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                        </ExtraParams>
                                    </RowClick>
                                </DirectEvents>
                                <Plugins>
                                    <ext:GridFilters runat="server" Local="true" FiltersText="Filtros">
                                        <Filters>
                                            <ext:DateFilter DataIndex="FechaTX" BeforeText="Antes de" OnText="El día"
                                                AfterText="Después de">
                                                <DatePickerOptions runat="server" TodayText="Hoy" />
                                            </ext:DateFilter>
                                            <ext:NumericFilter DataIndex="Tarjeta" />
                                            <ext:StringFilter DataIndex="ClaveOperacion" />
                                        </Filters>
                                    </ext:GridFilters>
                                </Plugins>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingOpsComp" runat="server" StoreID="StoreOpsComp" DisplayInfo="true"
                                        DisplayMsg="Mostrando Operaciones {0} - {1} de {2}" HideRefresh="true" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
        </Center>
        <South Split="true">
            <ext:GridPanel ID="GridOpsPorComp" runat="server" StripeRows="true" Border="false" Layout="FitLayout"
                Title="Operaciones para compensación" Height="200" Collapsed="true" Collapsible ="true">
                <LoadMask ShowMask="false" />
                <Store>
                    <ext:Store ID="StoreOpsPorComp" runat="server" RemoteSort="true" OnRefreshData="StoreOpsPorComp_RefreshData"
                        AutoLoad="false">
                        <AutoLoadParams>
                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                        </AutoLoadParams>
                        <Proxy>
                            <ext:PageProxy />
                        </Proxy>
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Operacion">
                                <Fields>
                                    <ext:RecordField Name="ID_Operacion" />
                                    <ext:RecordField Name="Tarjeta" />
                                    <ext:RecordField Name="FechaTX" />
                                    <ext:RecordField Name="MonedaTX" />
                                    <ext:RecordField Name="MontoTX" />
                                    <ext:RecordField Name="NumAutorizacion" />
                                    <ext:RecordField Name="Comercio" />
                                    <ext:RecordField Name="Estatus" />
                                    <ext:RecordField Name="ID_Emisor" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <ColumnModel runat="server">
                    <Columns>
                        <ext:Column DataIndex="ID_Operacion" Hidden="true" />
                        <ext:Column Header="Tarjeta" Sortable="true" DataIndex="Tarjeta" Width="120" />
                        <ext:DateColumn Header="Fecha TX" Sortable="true" DataIndex="FechaTX"
                            Format="yyyy-MM-dd HH:mm:ss" Width="150" />
                        <ext:Column Header="Moneda TX" Sortable="true" DataIndex="MonedaTX" />
                        <ext:Column Header="Monto TX" Sortable="true" DataIndex="MontoTX">
                            <Renderer Format="UsMoney" />
                        </ext:Column>
                        <ext:Column Header="No. Autorización" Sortable="true" DataIndex="NumAutorizacion" />
                        <ext:Column Header="Comercio" Sortable="true" DataIndex="Comercio" Width="200" />
                        <ext:Column Header="Estatus" Sortable="true" DataIndex="Estatus" />
                        <ext:Column DataIndex="ID_Emisor" Hidden="true" />
                    </Columns>
                </ColumnModel>
                <SelectionModel>
                    <ext:CheckboxSelectionModel ID="CheckboxSelectionModel1" runat="server" SingleSelect="true">
                        <Listeners>
                            <RowDeselect Handler="#{btnCompensar}.setDisabled(true);" />
                            <RowSelect Handler="#{btnCompensar}.setDisabled(false);
                                #{hdnEmisor}.setValue(record.get('ID_Emisor'));" />
                                <%--onBefore(record, #{hdnMonto}, #{hdnEmisor})" />--%>
                        </Listeners>
                    </ext:CheckboxSelectionModel>
                </SelectionModel>
                <Plugins>
                    <ext:GridFilters runat="server" Local="true" FiltersText="Filtros">
                        <Filters>
                            <ext:DateFilter DataIndex="FechaTX" BeforeText="Antes de" OnText="El día"
                                AfterText="Después de">
                                <DatePickerOptions runat="server" TodayText="Hoy" />
                            </ext:DateFilter>
                            <ext:NumericFilter DataIndex="Tarjeta" />
                        </Filters>
                    </ext:GridFilters>
                </Plugins>
                <TopBar>
                    <ext:Toolbar runat="server">
                        <Items>
                            <ext:ToolbarFill runat="server" />
                            <ext:Button ID="btnCompensar" runat="server" Icon="Accept" Text="Compensar Operación"
                                Disabled="true">
                                <DirectEvents>
                                    <Click OnEvent="btnCompensar_Click">
                                        <Confirmation ConfirmRequest="true" Title="Confirmación" Message="¿Estás seguro de compensar esta Operación?" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingTB_OpsXComp" runat="server" StoreID="StoreOpsPorComp" DisplayInfo="true"
                        DisplayMsg="Mostrando Operaciones {0} - {1} de {2}" HideRefresh="true" />
                </BottomBar>
            </ext:GridPanel>
        </South>
    </ext:BorderLayout>
</asp:Content>
