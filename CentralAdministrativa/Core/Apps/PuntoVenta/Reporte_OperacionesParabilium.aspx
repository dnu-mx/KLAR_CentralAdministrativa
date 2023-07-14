<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Reporte_OperacionesParabilium.aspx.cs" Inherits="TpvWeb.Reporte_OperacionesParabilium" %>

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

        var onKeyUpPres = function (field) {
            var v = this.processValue(this.getRawValue()),
                field;

            if (this.startDateFieldP) {
                field = Ext.getCmp(this.startDateFieldP);
                field.setMaxValue();
                this.dateRangeMax = null;
            } else if (this.endDateFieldP) {
                field = Ext.getCmp(this.endDateFieldP);
                field.setMinValue();
                this.dateRangeMin = null;
            }

            field.validate();
        };

        var onKeyUpComp = function (field) {
            var v = this.processValue(this.getRawValue()),
                field;

            if (this.startDateFieldC) {
                field = Ext.getCmp(this.startDateFieldC);
                field.setMaxValue();
                this.dateRangeMax = null;
            } else if (this.endDateFieldC) {
                field = Ext.getCmp(this.endDateFieldC);
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
        <West Split="true" Collapsible="true">
            <ext:FormPanel ID="FormPanel1" Width="320" Title="Selecciona los Filtros" runat="server" Padding="10"
                Border="false" Layout="FitLayout" LabelWidth="160">
                <Content>
                    <ext:Store ID="StoreEstatusOp" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_EstatusOperacion">
                                <Fields>
                                    <ext:RecordField Name="ID_EstatusOperacion" />
                                    <ext:RecordField Name="Clave" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:Store ID="StoreMotivoRechazo" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_CodRespuestaExterno">
                                <Fields>
                                    <ext:RecordField Name="ID_CodRespuestaExterno" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:Store ID="StoreEstatusComp" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_EstatusPostOperacion">
                                <Fields>
                                    <ext:RecordField Name="ID_EstatusPostOperacion" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Content>
                <Items>
                    <ext:ComboBox ID="cBoxTipoColectiva" runat="server" FieldLabel="Tipo de Colectiva <span style='color:red;'>*   </span>" 
                        Width="250" AllowBlank="false" DisplayField="Descripcion" ValueField="ID_TipoColectiva">
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
                            <Select OnEvent="EstableceColectivas" Before="#{cBoxGpoComercial}.clearValue();">
                                <EventMask ShowMask="true" Msg="Estableciendo Colectivas..." MinDelay="200" />
                            </Select>
                        </DirectEvents>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cBoxGpoComercial" runat="server" FieldLabel="Colectiva <span style='color:red;'>*   </span>"
                        Width="300" DisplayField="NombreORazonSocial" ValueField="ID_Colectiva" Mode="Local" AutoSelect="true"
                        ForceSelection="true" TypeAhead="true" MinChars="1" MatchFieldWidth="false" EmptyText="Todas"
                        AllowBlank="false" ListWidth="350">
                        <Items>
                            <ext:ListItem Text="( Todas )" Value="-1" />
                        </Items>
                        <Store>
                            <ext:Store ID="StoreGpoComercial" runat="server">
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
                    <ext:DateField ID="dfFechaInicio" runat="server" Vtype="daterange" MaskRe="[0-9\/]" Width="300"
                        FieldLabel="Fecha/Hora Inicial Operación" Format="yyyy/MM/dd" EnableKeyEvents="true" 
                        InvalidText="Fecha inválida. Debe tener el formato AAAA/MM/DD">
                        <CustomConfig>
                            <ext:ConfigItem Name="endDateField" Value="#{dfFechaFin}" Mode="Value" />
                        </CustomConfig>
                        <Listeners>
                            <KeyUp Fn="onKeyUp" />
                            <Change Handler="if (this.getValue() == '') { #{tfHoraInicio}.clear(); }" />
                        </Listeners>
                    </ext:DateField>
                    <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false">
                        <Defaults>
                            <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                        </Defaults>
                        <LayoutConfig>
                            <ext:HBoxLayoutConfig Align="Top" />
                        </LayoutConfig>
                        <Items>
                            <ext:Hidden runat="server" Flex="1" Width="165" />
                            <ext:TimeField ID="tfHoraInicio" runat="server" LabelSeparator=" " Format="HH:mm:ss" Width="135"
                                SelectedTime="00:00:00" Increment="30" AllowBlank="true" MaskRe="[0-9\:]">
                                <Triggers>
                                    <ext:FieldTrigger Icon="Clear" HideTrigger="true" />
                                </Triggers>
                                <Listeners>
                                    <BeforeQuery Handler="this.triggers[0][ this.getRawValue().toString().length == 0 ? 'hide' : 'show']();" />
                                    <TriggerClick Handler="if (index == 0) { this.focus().clearValue(); trigger.hide();}" />
                                    <Select Handler="this.triggers[0].show();" />
                                </Listeners>
                            </ext:TimeField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel runat="server" Layout="FitLayout" Width="300" Height="4" Border="false" />
                    <ext:DateField ID="dfFechaFin" runat="server" Vtype="daterange" MaskRe="[0-9\/]" Width="300"
                        FieldLabel="Fecha/Hora Final Operación" Format="yyyy/MM/dd" EnableKeyEvents="true"
                        InvalidText="Fecha inválida. Debe tener el formato AAAA/MM/DD">
                        <CustomConfig>
                            <ext:ConfigItem Name="startDateField" Value="#{dfFechaInicio}" Mode="Value" />
                        </CustomConfig>
                        <Listeners>
                            <KeyUp Fn="onKeyUp" />
                            <Change Handler="if (this.getValue() == '') { #{tfHoraFin}.clear(); }" />
                        </Listeners>
                    </ext:DateField>
                    <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false">
                        <Defaults>
                            <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                        </Defaults>
                        <LayoutConfig>
                            <ext:HBoxLayoutConfig Align="Top" />
                        </LayoutConfig>
                        <Items>
                            <ext:Hidden runat="server" Flex="1" Width="165" />
                            <ext:TimeField ID="tfHoraFin" runat="server" LabelSeparator=" " Format="HH:mm:ss" Width="135"
                                SelectedTime="23:59:59" Increment="30" AllowBlank="true" MaskRe="[0-9\:]">
                                <Triggers>
                                    <ext:FieldTrigger Icon="Clear" HideTrigger="true" />
                                </Triggers>
                                <Listeners>
                                    <BeforeQuery Handler="this.triggers[0][ this.getRawValue().toString().length == 0 ? 'hide' : 'show']();" />
                                    <TriggerClick Handler="if (index == 0) { this.focus().clearValue(); trigger.hide();}" />
                                    <Select Handler="this.triggers[0].show();" />
                                </Listeners>
                            </ext:TimeField>
                        </Items>
                    </ext:Panel>
                    <ext:Panel runat="server" Layout="FitLayout" Width="300" Height="4" Border="false" />
                    <ext:DateField ID="dfFechaIniPresen" runat="server" Vtype="daterange" MaskRe="[0-9\/]" Width="300"
                        FieldLabel="Fecha Inicial Presentación" Format="yyyy/MM/dd" EnableKeyEvents="true"
                        InvalidText="Fecha inválida. Debe tener el formato AAAA/MM/DD">
                        <CustomConfig>
                            <ext:ConfigItem Name="endDateFieldP" Value="#{dfFechaFinPresen}" Mode="Value" />
                        </CustomConfig>
                        <Listeners>
                            <KeyUp Fn="onKeyUpPres" />
                        </Listeners>
                    </ext:DateField>
                    <ext:DateField ID="dfFechaFinPresen" runat="server" Vtype="daterange" MaskRe="[0-9\/]" Width="300"
                        FieldLabel="Fecha Final Presentación" Format="yyyy/MM/dd" EnableKeyEvents="true"
                        InvalidText="Fecha inválida. Debe tener el formato AAAA/MM/DD">
                        <CustomConfig>
                            <ext:ConfigItem Name="startDateFieldP" Value="#{dfFechaIniPresen}" Mode="Value" />
                        </CustomConfig>
                        <Listeners>
                            <KeyUp Fn="onKeyUpPres" />
                        </Listeners>
                    </ext:DateField>
                    <ext:TextField ID="txtTarjeta" runat="server" FieldLabel="Tarjeta" EmptyText="Todas"
                        Width="300" MaskRe="[0-9]" MinLength="16" MaxLength="16" />
                    <ext:ComboBox ID="cBoxEstatusOp" FieldLabel="Estatus Operación" StoreID="StoreEstatusOp"
                        EmptyText="Todos" Width="300" runat="server" DisplayField="Descripcion" ListWidth="200"
                        ValueField="Clave">
                        <Items>
                            <ext:ListItem Text="( Todos )" Value="" />
                        </Items>
                        <Listeners>
                            <Select Handler="#{cBoxMotivoRechazo}.clear(); 
                                if (this.getValue() == 'ANOK') { #{cBoxMotivoRechazo}.setDisabled(false); }
                                else { #{cBoxMotivoRechazo}.setDisabled(true); }" />
                        </Listeners>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cBoxMotivoRechazo" FieldLabel="Motivo Rechazo" StoreID="StoreMotivoRechazo"
                        EmptyText="Todos" Width="300" runat="server" DisplayField="Descripcion" ListWidth="300"
                        ValueField="ID_CodRespuestaExterno" Disabled="true">
                        <Items>
                            <ext:ListItem Text="( Todos )" Value="-1" />
                        </Items>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cBoxEstatusComp" FieldLabel="Estatus Compensación" StoreID="StoreEstatusComp"
                        EmptyText="Todos" Width="300" runat="server" DisplayField="Descripcion" ListWidth="200"
                        ValueField="ID_EstatusPostOperacion">
                        <Items>
                            <ext:ListItem Text="( Todos )" Value="-1" />
                        </Items>
                    </ext:ComboBox>
                    <ext:Panel runat="server" Layout="FitLayout" Height="20" Border="false" />
                    <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false" LabelWidth="5" >
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
                                        #{GridPanelOperaciones}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                        Before="if (!#{FormPanel1}.getForm().isValid()) { return false; }
                                        else if (!#{dfFechaInicio}.getValue() && !#{dfFechaFin}.getValue()
                                        && !#{dfFechaIniPresen}.getValue() && !#{dfFechaFinPresen}.getValue()) 
                                        { Ext.Msg.alert('Aviso', 'Ingresa al menos una fecha de filtro.');
                                        return false; }
                                        else { resetToolbar(#{PagingRepOpParab});
                                        #{GridPanelOperaciones}.getStore().sortInfo = null; }">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                            RepOpParabilium.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>
            </ext:FormPanel>
        </West>
        <Center Split="true" Collapsible="false">
            <ext:FormPanel ID="FormPanelOperaciones" runat="server" Layout="FitLayout" Title="Operaciones obtenidas con los filtros seleccionados">
                <Items>
                    <ext:GridPanel ID="GridPanelOperaciones" runat="server" StripeRows="true" Header="false" Border="false">
                        <LoadMask ShowMask="false" />
                        <Store>
                            <ext:Store ID="StoreOperaciones" runat="server" RemoteSort="true" OnRefreshData="StoreOperaciones_RefreshData"
                                AutoLoad="false">
                                <AutoLoadParams>
                                    <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                </AutoLoadParams>
                                <Proxy>
                                    <ext:PageProxy />
                                </Proxy>                                
                                <DirectEventConfig IsUpload="true"/>
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_Operacion">
                                        <Fields>
                                            <ext:RecordField Name="ID_Operacion" />
                                            <ext:RecordField Name="ID_OperacionOriginal" />
                                            <ext:RecordField Name="Tarjeta" />
                                            <ext:RecordField Name="FechaOperacion" />
                                            <ext:RecordField Name="EstatusOperacion" />
                                            <ext:RecordField Name="ResponseCode" />
                                            <ext:RecordField Name="MotivoRechazo" />
                                            <ext:RecordField Name="EstatusCompensacion" />
                                            <ext:RecordField Name="Importe" />
                                            <ext:RecordField Name="Autorizacion" />
                                            <ext:RecordField Name="POSEntryMode" />
                                            <ext:RecordField Name="AfiliacionComercio" />
                                            <ext:RecordField Name="CodigoMoneda" />
                                            <ext:RecordField Name="Afiliacion" />
                                            <ext:RecordField Name="Comercio" />
                                            <ext:RecordField Name="Pais" />
                                            <ext:RecordField Name="ClaveTransaccion" />
                                            <ext:RecordField Name="FechaProcesamiento" />
                                            <ext:RecordField Name="ImporteCompensadoPesos" />
                                            <ext:RecordField Name="ImporteCompensadoDolar" />
                                            <ext:RecordField Name="ImporteCompensadoLocal" />
                                            <ext:RecordField Name="CodigoMonedaLocal" />
                                            <ext:RecordField Name="Comision" />
                                            <ext:RecordField Name="IVA" />
                                            <ext:RecordField Name="FechaPresentacion" />
                                            <ext:RecordField Name="NombreArchivo" />
                                            <ext:RecordField Name="NombreComercial" />
                                            <ext:RecordField Name="MCC" />
                                            <ext:RecordField Name="ClaveMovimiento" />
                                            <ext:RecordField Name="Descripcion" />
                                            <ext:RecordField Name="ProcessingCode" />
                                            <ext:RecordField Name="Adquiriente" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <ColumnModel ID="ColumnModel1" runat="server">
                            <Columns>
                                <ext:Column Hidden="true" DataIndex="ID_Operacion" />
                                <ext:Column Hidden="true" DataIndex="ID_OperacionOriginal" />
                                <ext:Column Header="Tarjeta" Sortable="true" DataIndex="Tarjeta">
                                    <Editor>
                                        <ext:TextField runat="server" />
                                    </Editor>
                                </ext:Column>
                                <ext:DateColumn Header="Fecha Operación" Sortable="true" DataIndex="FechaOperacion"
                                    Format="yyyy-MM-dd HH:mm:ss" />
                                <ext:Column Header="Estatus Operación" Sortable="true" DataIndex="EstatusOperacion" />
                                <ext:Column Header="ResponseCode" Sortable="true" DataIndex="ResponseCode" Width="150" />
                                <ext:Column Header="Motivo de Rechazo" Sortable="true" DataIndex="MotivoRechazo"
                                    Width="120" />
                                <ext:Column Header="Estatus Compensación" Sortable="true" DataIndex="EstatusCompensacion"
                                    Width="150" />
                                <ext:Column Header="Importe" Sortable="true" DataIndex="Importe">
                                    <Renderer Format="UsMoney" />
                                </ext:Column>
                                <ext:Column Header="Autorización" DataIndex="Autorizacion" />
                                <ext:Column Header="POSEntryMode" Sortable="true" DataIndex="POSEntryMode" Width="150"/>
                                <ext:Column Header="AfiliacionComercio" Sortable="true" DataIndex="AfiliacionComercio" Width="150"/>
                                <ext:Column Header="Moneda" Sortable="true" DataIndex="CodigoMoneda" />
                                <ext:Column Header="Afiliación" Sortable="true" DataIndex="Afiliacion" Width="105" />
                                <ext:Column Header="Comercio" Sortable="true" DataIndex="Comercio" Width="140" />
                                <ext:Column Header="País" Sortable="true" DataIndex="Pais" Width="40" />
                                <ext:Column Header="Clave Transacción" DataIndex="ClaveTransaccion" />
                                <ext:DateColumn Header="Fecha Procesamiento" Sortable="true" DataIndex="FechaProcesamiento"
                                    Format="yyyy-MM-dd HH:mm:ss" Width="120"/>
                                <ext:Column Header="Importe Compensado Pesos" Sortable="true" Width="150"
                                    DataIndex="ImporteCompensadoPesos">
                                    <Renderer Format="UsMoney" />
                                </ext:Column>
                                <ext:Column Header="Importe Compensado Dólar" Sortable="true" Width="150"
                                    DataIndex="ImporteCompensadoDolar">
                                    <Renderer Format="UsMoney" />
                                </ext:Column>
                                <ext:Column Header="Importe Compensado Local" Sortable="true" Width="150"
                                    DataIndex="ImporteCompensadoLocal">
                                    <Renderer Format="UsMoney" />
                                </ext:Column>
                                <ext:Column Header="Código Moneda Local" Sortable="true" DataIndex="CodigoMonedaLocal"
                                    Width="120" />
                                <ext:Column Header="Comisión" Sortable="true" DataIndex="Comision" />
                                <ext:Column Header="IVA" Sortable="true" DataIndex="IVA" />
                                <ext:DateColumn Header="Fecha Presentación" Sortable="true" DataIndex="FechaPresentacion"
                                    Format="yyyy-MM-dd" Width="120"/>
                                <ext:Column Header="Nombre Archivo" Sortable="true" DataIndex="NombreArchivo"
                                    Width="150" />
                                <ext:Column Header="Nombre Comercial" Sortable="true" DataIndex="NombreComercial"
                                    Width="150" />
                                   <ext:Column Header="MCC" Sortable="true" DataIndex="MCC" />
                                <ext:Column Header="ClaveMovimiento" Sortable="true" DataIndex="ClaveMovimiento" />
                                <ext:Column Header="Descripción" Sortable="true" DataIndex="Descripcion" Width="150"/>
                                <ext:Column Header="ProcessingCode" Sortable="true" DataIndex="ProcessingCode" Width="150"/>
                                <ext:Column Header="Adquiriente" Sortable="true" DataIndex="Adquiriente" Width="150"/>
                            </Columns>
                        </ColumnModel>
                        <SelectionModel>
                            <ext:RowSelectionModel SingleSelect="true"  />
                        </SelectionModel>
                        <Plugins>
                            <ext:GridFilters runat="server" ID="GridFilters1" Local="true" FiltersText="Filtros">
                                <Filters>
                                    <ext:DateFilter DataIndex="FechaRegistro" BeforeText="Antes de" OnText="El día"
                                        AfterText="Después de">
                                        <DatePickerOptions runat="server" TodayText="Hoy" />
                                    </ext:DateFilter>
                                    <ext:NumericFilter DataIndex="Tarjeta" />
                                    <ext:StringFilter DataIndex="EstatusOperacion" />
                                    <ext:StringFilter DataIndex="MotivoRechazo" />
                                    <ext:StringFilter DataIndex="EstatusCompensacion" />
                                    <ext:NumericFilter DataIndex="Importe" />
                                    <ext:StringFilter DataIndex="CodigoMoneda" />
                                    <ext:StringFilter DataIndex="Afiliacion" />
                                    <ext:StringFilter DataIndex="Comercio" />
                                    <ext:StringFilter DataIndex="Pais" />
                                    <ext:DateFilter DataIndex="FechaCompensacion" BeforeText="Antes de" OnText="El día"
                                        AfterText="Después de">
                                        <DatePickerOptions runat="server" TodayText="Hoy" />
                                    </ext:DateFilter>
                                    <ext:NumericFilter DataIndex="ImporteCompensadoPesos" />
                                    <ext:NumericFilter DataIndex="ImporteCompensadoDolar" />
                                    <ext:NumericFilter DataIndex="ImporteCompensadoLocal" />
                                    <ext:NumericFilter DataIndex="Comision" />
                                    <ext:NumericFilter DataIndex="IVA" />
                                    <ext:DateFilter DataIndex="FechaPresentacion" BeforeText="Antes de" OnText="El día"
                                        AfterText="Después de">
                                        <DatePickerOptions runat="server" TodayText="Hoy"/>
                                    </ext:DateFilter>
                                    <ext:StringFilter DataIndex="NombreArchivo" />
                                    <ext:StringFilter DataIndex="NombreComercial" />
                                </Filters>
                            </ext:GridFilters>
                        </Plugins>
                        <BottomBar>
                            <ext:PagingToolbar ID="PagingRepOpParab" runat="server" StoreID="StoreOperaciones" DisplayInfo="true"
                                DisplayMsg="Mostrando Operaciones {0} - {1} de {2}" HideRefresh="true"/>
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
                                                    RepOpParabilium.StopMask();" />
                                        </DirectEvents>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                    </ext:GridPanel>
                </Items>
            </ext:FormPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
