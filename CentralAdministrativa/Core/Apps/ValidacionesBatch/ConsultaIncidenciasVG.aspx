<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ConsultaIncidenciasVG.aspx.cs"
    Inherits="ValidacionesBatch.ConsultaIncidenciasVG" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var template = '<span style="color:{0};text-decoration:underline;">{1}</span>';

        var link = function (value) {
            return String.format(template, (value != "") ? "blue" : "black", value);
        };

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

        var onKeyUpOper = function (field) {
            var v = this.processValue(this.getRawValue()),
                field;

            if (this.startDateFieldOper) {
                field = Ext.getCmp(this.startDateFieldOper);
                field.setMaxValue();
                this.dateRangeMax = null;
            } else if (this.endDateField) {
                field = Ext.getCmp(this.endDateFieldOper);
                field.setMinValue();
                this.dateRangeMin = null;
            }

            field.validate();
        };

        var onKeyUpInc = function (field) {
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
    <ext:Store ID="StoreAcciones" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_Accion">
                <Fields>
                    <ext:RecordField Name="ID_Accion" />
                    <ext:RecordField Name="Clave" />
                    <ext:RecordField Name="Accion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Store ID="StoreReglas" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_Regla">
                <Fields>
                    <ext:RecordField Name="ID_Regla" />
                    <ext:RecordField Name="Clave" />
                    <ext:RecordField Name="Nombre" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Window ID="WdwAfiliacion" runat="server" Title="Afiliación" Hidden="true" Width="600" Height="350"
        Modal="true" Resizable="false" Closable="true">
        <Items>
            <ext:FormPanel ID="FormPanelAfiliacion" runat="server" Padding="10" MonitorValid="true" LabelAlign="Left">
                <Items>
                    <ext:TextField ID="txtAfiliacion" runat="server" FieldLabel="Afiliación" ReadOnly="true"
                        AnchorHorizontal="100%"/>
                    <ext:TextField ID="txtNombre" runat="server" FieldLabel="Nombre" ReadOnly="true"
                        AnchorHorizontal="100%" />
                    <ext:TextField ID="txtPropietario" runat="server" FieldLabel="Propietario" ReadOnly="true"
                        AnchorHorizontal="100%" />
                    <ext:TextField ID="txtRazonSocial" runat="server" FieldLabel="Razón Social" ReadOnly="true"
                        AnchorHorizontal="100%" />
                    <ext:TextField ID="txtRFC" runat="server" FieldLabel="RFC" ReadOnly="true"
                        AnchorHorizontal="100%" />
                    <ext:TextField ID="txtDomicilio" runat="server" FieldLabel="Domicilio" ReadOnly="true"
                        AnchorHorizontal="100%" />
                    <ext:TextField ID="txtColonia" runat="server" FieldLabel="Colonia" ReadOnly="true"
                        AnchorHorizontal="100%" />
                    <ext:TextField ID="txtCodigoPostal" runat="server" FieldLabel="Código Postal" ReadOnly="true"
                        AnchorHorizontal="100%" />
                    <ext:TextField ID="txtEstado" runat="server" FieldLabel="Estado" ReadOnly="true"
                        AnchorHorizontal="100%" />
                    <ext:TextField ID="txtDescripcion" runat="server" FieldLabel="Descripción" ReadOnly="true"
                        AnchorHorizontal="100%" />
                </Items>
                <Buttons>
                    <ext:Button ID="btnAceptar" runat="server" Text="Aceptar" Icon="Tick">
                        <DirectEvents>
                            <Click OnEvent="btnAceptar_Click" />
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:Window ID="WdwOperacionesFraude" runat="server" Title="Cierre de caso" Width="900" Height="400" Resizable="False"
        Hidden="true" Closable="true" Modal="true" Layout="FitLayout" Draggable="true" Padding="5">
        <Items>
            <ext:BorderLayout ID="BorderLayout1" runat="server">
                <North Split="true">
                    <ext:FormPanel ID="FormPanelOpFraude" runat="server" Border="false">
                        <TopBar>
                            <ext:Toolbar runat="server" Flat="false">
                                <Items>
                                    <ext:Toolbar runat="server" Flat="true" Width="20" />
                                    <ext:ComboBox ID="cBoxDictamen" runat="server" EmptyText="Dictamen" AllowBlank="false"
                                        DisplayField="Descripcion" ValueField="ClaveDictamenCaso">
                                        <Store>
                                            <ext:Store ID="StoreDictamen" runat="server">
                                                <Reader>
                                                    <ext:JsonReader IDProperty="ID_DictamenCaso">
                                                        <Fields>
                                                            <ext:RecordField Name="ID_DictamenCaso" />
                                                            <ext:RecordField Name="ClaveDictamenCaso" />
                                                            <ext:RecordField Name="Descripcion" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Store>
                                        <Listeners>
                                            <Select Handler="if (this.getValue() == 'FRAU') {
                                                    #{cBoxTipoFraude}.setDisabled(false);
                                                    #{GridPanelOpFraude}.setDisabled(false);
                                                    #{cBoxTipoFraude}.focus();
                                                }
                                                else {
                                                    #{cBoxTipoFraude}.setDisabled(true);
                                                    #{GridPanelOpFraude}.setDisabled(true);
                                                }" />
                                        </Listeners>
                                    </ext:ComboBox>
                                    <ext:Toolbar runat="server" Flat="true" Width="50" />
                                    <ext:ComboBox ID="cBoxTipoFraude" runat="server" EmptyText="Tipo de Fraude" AutoFocus="true" Disabled="true"
                                        DisplayField="Descripcion" ValueField="ID_TipoFraude" AllowBlank="false" ListWidth="300">
                                        <Store>
                                            <ext:Store ID="StoreTipoFraude" runat="server">
                                                <Reader>
                                                    <ext:JsonReader IDProperty="ID_TipoFraude">
                                                        <Fields>
                                                            <ext:RecordField Name="ID_TipoFraude" />
                                                            <ext:RecordField Name="ClaveTipoFraude" />
                                                            <ext:RecordField Name="Descripcion" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Store>
                                    </ext:ComboBox>
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                    </ext:FormPanel>
                </North>
                <Center Split="true">
                    <ext:Panel ID="Panel123" runat="server" Border="false">
                        <Content>
                            <ext:BorderLayout ID="BorderLayout2" runat="server">
                                <North Split="true">
                                    <ext:GridPanel ID="GridPanelOpFraude" runat="server" Header="true" Disabled="true"  Height="310"
                                        Title="Selecciona las operaciones que se marcarán como fraudulentas:">
                                        <Store>
                                            <ext:Store ID="StoreOpFraude" runat="server" OnRefreshData="btnBuscaInc_Click">
                                                <DirectEventConfig IsUpload="true" />
                                                <Reader>
                                                    <ext:JsonReader IDProperty="ID_Operacion">
                                                        <Fields>
                                                            <ext:RecordField Name="ID_Operacion" />
                                                            <ext:RecordField Name="FechaRegistro" />
                                                            <ext:RecordField Name="FechaOperacion" />
                                                            <ext:RecordField Name="DiaSemana" />
                                                            <ext:RecordField Name="Tarjeta" />
                                                            <ext:RecordField Name="Regla" />
                                                            <ext:RecordField Name="Accion" />
                                                            <ext:RecordField Name="Incidencias" />
                                                            <ext:RecordField Name="ReglasRotas" />
                                                            <ext:RecordField Name="Estatus" />
                                                            <ext:RecordField Name="Afiliacion" />
                                                            <ext:RecordField Name="Comercio" />
                                                            <ext:RecordField Name="Pais" />
                                                            <ext:RecordField Name="Narrativa1" />
                                                            <ext:RecordField Name="Narrativa2" />
                                                            <ext:RecordField Name="Narrativa3" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                                <DirectEventConfig IsUpload="true" />
                                            </ext:Store>
                                        </Store>
                                        <ColumnModel ID="ColumnModel2" runat="server">
                                            <Columns>
                                                <ext:DateColumn DataIndex="FechaRegistro" Header="Fecha de Registro" Width="120"
                                                    Format="dd-MM-yyyy HH:mm:ss" />
                                                <ext:Column DataIndex="FechaOperacion" Header="Fecha de la Operación" Width="120"
                                                    Align="Center"/>
                                                <ext:Column DataIndex="DiaSemana" Header="Día Semana" Width="75" Align="Center" />
                                                <ext:Column DataIndex="Regla" Header="Regla" Width="150" />
                                                <ext:Column DataIndex="Accion" Header="Acción" Width="60" />
                                                <ext:Column DataIndex="Estatus" Header="Estatus" Width="50" />
                                                <ext:Column DataIndex="Afiliacion" Header="Afiliación" Width="105" />
                                                <ext:Column DataIndex="Comercio" Header="Comercio" Width="140" />
                                                <ext:Column DataIndex="Pais" Header="País" Width="40" />
                                                <ext:Column DataIndex="Narrativa1" Header="Detalle 1" Width="100" />
                                                <ext:Column DataIndex="Narrativa2" Header="Detalle 2" Width="100" />
                                                <ext:Column DataIndex="Narrativa3" Header="Detalle 3" Width="100" />
                                            </Columns>
                                        </ColumnModel>
                                        <SelectionModel>
                                            <ext:CheckboxSelectionModel ID="chkSMOperaciones" runat="server" Enabled="false" />
                                        </SelectionModel>
                                        <Plugins>
                                            <ext:GridFilters runat="server" ID="GridFilters4" Local="true">
                                                <Filters>
                                                    <ext:StringFilter DataIndex="Fecha" />
                                                </Filters>
                                            </ext:GridFilters>
                                        </Plugins>
                                        <BottomBar>
                                            <ext:PagingToolbar ID="PagingToolBar4" runat="server" StoreID="StoreOpFraude" DisplayInfo="true"
                                                DisplayMsg="Incidencias {0} - {1} de {2}" />
                                        </BottomBar>
                                    </ext:GridPanel>
                                </North>
                                <Center>
                                    <ext:FormPanel ID="FormPanelppp" runat="server" Layout="FitLayout" Border="false" Height="50">
                                        <TopBar>
                                            <ext:Toolbar runat="server" Flat="false">
                                                <Items>
                                                    <ext:ToolbarFill ID="ToolbarFillDummy" runat="server" />
                                                    <ext:ToolbarSeparator />
                                                    <ext:Button ID="btnAceptarCierre" runat="server" Text="Aceptar" Icon="Tick">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnAceptarCierre_Click" Before="var valid = #{cBoxDictamen}.isValid();
                                                                var valid_2 = #{cBoxTipoFraude}.isValid();
                                                                if (!valid || !valid_2) {} return valid;">
                                                                <ExtraParams>
                                                                    <ext:Parameter Name="Values" Value="Ext.encode(#{GridPanelOpFraude}.getRowsValues())" Mode="Raw" />
                                                                </ExtraParams>
                                                            </Click>
                                                        </DirectEvents>
                                                    </ext:Button>
                                                    <ext:ToolbarSeparator />
                                                    <ext:Toolbar runat="server" Flat="true" Width="20" />
                                                    <ext:ToolbarSeparator />
                                                    <ext:Button ID="btnCancelar" runat="server" Text="Cancelar" Icon="Cross">
                                                        <Listeners>
                                                            <Click Handler="#{WdwOperacionesFraude}.hide();" />
                                                        </Listeners>
                                                    </ext:Button>
                                                    <ext:ToolbarSeparator />
                                                    <ext:Toolbar runat="server" Flat="true" Width="20" />
                                                </Items>
                                            </ext:Toolbar>
                                        </TopBar>
                                    </ext:FormPanel>
                                </Center>
                            </ext:BorderLayout>
                        </Content>
                    </ext:Panel>
                </Center>
            </ext:BorderLayout>
        </Items>
    </ext:Window>
    <ext:Window ID="WdwDetalleTarjeta" runat="server" Title="Tarjeta: " Width="900" Height="500" Resizable="False"
        Hidden="true" Closable="true" Modal="true" Layout="FitLayout" Draggable="true" Padding="5">
        <Items>
            <ext:FormPanel ID="FormPanel1" runat="server" Layout="FitLayout">
                <TopBar>
                    <ext:Toolbar runat="server" Flat="false">
                        <Items>
                            <ext:Toolbar runat="server" Flat="true" Width="5" />
                            <ext:Label ID="lblEstatusTarjeta" runat="server" Text="Tarjeta " />
                            <ext:Toolbar runat="server" Flat="true" Width="30" />
                            <ext:ComboBox ID="cBoxEstatusTarjeta" runat="server" FieldLabel="Cambiar Estatus"
                                ValueField="ID_EstatusMA" DisplayField="Descripcion" LabelWidth="80"
                                LabelAlign="Left" Width="220">
                                <Store>
                                    <ext:Store ID="StoreEstatusTarjeta" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_EstatusMA">
                                                <Fields>
                                                    <ext:RecordField Name="ID_EstatusMA" />
                                                    <ext:RecordField Name="Clave" />
                                                    <ext:RecordField Name="Descripcion" />
                                                    <ext:RecordField Name="EstatusActual" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                            </ext:ComboBox>
                            <ext:Button ID="btnAceptarEstatus" runat="server" Icon="Tick" Text="Aceptar">
                                <Listeners>
                                    <Click Handler="
                                        var id = #{cBoxEstatusTarjeta}.getValue();
                                        var record = #{cBoxEstatusTarjeta}.getStore().getById(id);
                                        var estatusSelected = record.get('Descripcion');
                                        var nuevoEstatus = estatusSelected == 'ACTIVA' ? 'Activación' : 
                                            estatusSelected == 'CANCELADA' ? 'Cancelación' : 'Bloqueo';

                                        if (estatusSelected == 'CANCELADA') {
                                            Ext.MessageBox.show({
                                                icon: Ext.MessageBox.WARNING,
                                                title: 'Confirmación',
                                                msg: '¿Estás seguro de CANCELAR la Tarjeta? Esta acción es definitiva e irreversible.',
                                                buttons: Ext.MessageBox.YESNO,
                                                fn: function (btn) {
                                                    if (btn == 'yes') {
                                                        ValidacionesBatch.CambiaEstatusTarjeta(nuevoEstatus);
                                                    } else {
                                                        #{cBoxEstatusTarjeta}.clear();
                                                    }
                                                }
                                            });
                                        } else {
                                            var estatusMsj = estatusSelected == 'ACTIVA' ? 'Activar' : 'Bloquear';

                                            Ext.MessageBox.show({
                                                title: 'Confirmación',
                                                msg: '¿Estás seguro de ' + estatusMsj + ' la Tarjeta?',
                                                buttons: Ext.MessageBox.YESNO,
                                                fn: function (btn) {
                                                    if (btn == 'yes') {
                                                        ValidacionesBatch.CambiaEstatusTarjeta(nuevoEstatus);
                                                    } else {
                                                        #{cBoxEstatusTarjeta}.clear();
                                                    }
                                                }
                                            });
                                        }
                                    " />
                                </Listeners>
                            </ext:Button>
                            <ext:ToolbarSeparator />
                            <ext:ToolbarFill ID="dummy" runat="server" />
                            <ext:ToolbarSeparator />
                            <ext:Button ID="btnCerrarCaso" runat="server" Icon="BulletCross" Text="Cerrar Caso">
                                <Listeners>
                                    <Click Handler="Ext.MessageBox.show({
                                        title: 'Confirmación',
                                        msg: '¿Estás seguro de Cerrar el Caso?',
                                        buttons: Ext.MessageBox.YESNO,
                                        fn: function (btn) {
                                            if (btn == 'yes') {
                                                ValidacionesBatch.LlenaGridPanelOpFraude();
                                            }
                                        }
                                    });" />
                                </Listeners>
                            </ext:Button>
                            <ext:Toolbar runat="server" Flat="true" Width="5" />
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <Items>
                    <ext:TabPanel ID="TabPanel1" runat="server" Border="false">
                        <Items>
                            <ext:FormPanel ID="FormPanelIncidencias" runat="server" Title="Incidencias" Border="false">
                                <Content>
                                    <ext:BorderLayout ID="BorderLayoutIncidencias" runat="server">
                                        <North Split="true">
                                            <ext:FormPanel ID="FormPanelBuscaIncidencias" runat="server" LabelWidth="70" LabelAlign="Right" Border="false">
                                                <TopBar>
                                                    <ext:Toolbar ID="Toolbar1" runat="server">
                                                        <Items>
                                                            <ext:DateField ID="dfFechaInicialInc" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                                                AllowBlank="false" MsgTarget="Qtip" Format="yyyy/MM/dd" LabelWidth="70"
                                                                LabelAlign="Right" Width="200" EnableKeyEvents="true" MaxWidth="200">
                                                                <CustomConfig>
                                                                    <ext:ConfigItem Name="endDateFieldInc" Value="#{dfFechaFinalInc}" Mode="Value" />
                                                                </CustomConfig>
                                                                <Listeners>
                                                                    <KeyUp Fn="onKeyUpInc" />
                                                                </Listeners>
                                                            </ext:DateField>
                                                            <ext:DateField ID="dfFechaFinalInc" runat="server" Vtype="daterange" FieldLabel="Fecha Final"
                                                                AllowBlank="false" MaxWidth="200" Width="200" MsgTarget="Qtip" Format="yyyy/MM/dd"
                                                                LabelWidth="70" LabelAlign="Right" EnableKeyEvents="true">
                                                                <CustomConfig>
                                                                    <ext:ConfigItem Name="startDateFieldInc" Value="#{dfFechaInicialInc}" Mode="Value" />
                                                                </CustomConfig>
                                                                <Listeners>
                                                                    <KeyUp Fn="onKeyUpInc" />
                                                                </Listeners>
                                                            </ext:DateField>
                                                            <ext:ComboBox ID="cBoxReglasInc" runat="server" EmptyText="Regla" Resizable="true" ListWidth="250"
                                                                Width="150" StoreID="StoreReglas" DisplayField="Nombre" ValueField="ID_Regla" />
                                                            <ext:ComboBox ID="cBoxAccionesInc" runat="server" EmptyText="Acción" Resizable="true" Width="100"
                                                                StoreID="StoreAcciones" DisplayField="Accion" ValueField="ID_Accion">
                                                                <Items>
                                                                    <ext:ListItem Text="( Todas )" Value="-1" />
                                                                </Items>
                                                            </ext:ComboBox>
                                                            <ext:Button ID="btnBuscaInc" runat="server" Text="Buscar" Icon="Magnifier">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnBuscaInc_Click" Before="var valid= #{FormPanelBuscaIncidencias}.getForm().isValid(); if (!valid) {} return valid;">
                                                                        <EventMask ShowMask="true" Msg="Buscando Incidencias..." MinDelay="500" />
                                                                    </Click>
                                                                </DirectEvents>
                                                            </ext:Button>
                                                            <ext:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                            <ext:Button ID="btnLimpiaInc" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnLimpiaInc_Click" />
                                                                </DirectEvents>
                                                            </ext:Button>
                                                        </Items>
                                                    </ext:Toolbar>
                                                </TopBar>
                                            </ext:FormPanel>
                                        </North>
                                        <Center Split="true">
                                            <ext:GridPanel ID="GridResultadosIncidencias" runat="server" Header="true">
                                                <Store>
                                                    <ext:Store ID="StoreResultadosIncidencias" runat="server" OnRefreshData="btnBuscaInc_Click"
                                                        OnSubmitData="StoreSubmit">
                                                        <DirectEventConfig IsUpload="true" />
                                                        <Reader>
                                                            <ext:JsonReader IDProperty="ID_OperacionRespuestaRegla">
                                                                <Fields>
                                                                    <ext:RecordField Name="ID_OperacionRespuestaRegla" />
                                                                    <ext:RecordField Name="ID_Operacion" />
                                                                    <ext:RecordField Name="FechaRegistro" />
                                                                    <ext:RecordField Name="FechaOperacion" />
                                                                    <ext:RecordField Name="DiaSemana" />
                                                                    <ext:RecordField Name="Tarjeta" />
                                                                    <ext:RecordField Name="Regla" />
                                                                    <ext:RecordField Name="Accion" />
                                                                    <ext:RecordField Name="Incidencias" />
                                                                    <ext:RecordField Name="ReglasRotas" />
                                                                    <ext:RecordField Name="Estatus" />
                                                                    <ext:RecordField Name="MotivoRechazo" />
                                                                    <ext:RecordField Name="Afiliacion" />
                                                                    <ext:RecordField Name="Comercio" />
                                                                    <ext:RecordField Name="Pais" />
                                                                    <ext:RecordField Name="Narrativa1" />
                                                                    <ext:RecordField Name="Narrativa2" />
                                                                    <ext:RecordField Name="Narrativa3" />
                                                                </Fields>
                                                            </ext:JsonReader>
                                                        </Reader>
                                                        <DirectEventConfig IsUpload="true" />
                                                    </ext:Store>
                                                </Store>
                                                <ColumnModel ID="ColumnModel3" runat="server">
                                                    <Columns>
                                                        <ext:DateColumn DataIndex="FechaRegistro" Header="Fecha de Registro"
                                                            Format="dd-MM-yyyy HH:mm:ss" Width="120" />
                                                        <ext:Column DataIndex="FechaOperacion" Header="Fecha de la Operación" Width="120"
                                                            Align="Center"/>
                                                        <ext:Column DataIndex="DiaSemana" Header="Día Semana" Width="75" Align="Center"/>
                                                        <ext:Column DataIndex="Regla" Header="Regla" Width="150" />
                                                        <ext:Column DataIndex="Accion" Header="Acción" Width="60" />
                                                        <ext:Column DataIndex="Incidencias" Header="Incidencias" Width="70" Align="Center"/>
                                                        <ext:Column DataIndex="ReglasRotas" Header="Reglas Rotas" Width="80" Align="Center"/>
                                                        <ext:Column DataIndex="Estatus" Header="Estatus" Width="90" />
                                                        <ext:Column ColumnID="MotivoRechazo" Header="Motivo Rechazo" Sortable="true" 
                                                            DataIndex="MotivoRechazo" Width="120" />
                                                        <ext:Column DataIndex="Afiliacion" Header="Afiliación" Width="105">
                                                            <Renderer Fn="link" />
                                                        </ext:Column>
                                                        <ext:Column DataIndex="Comercio" Header="Comercio" Width="140" />
                                                        <ext:Column DataIndex="Pais" Header="País" Width="100" />
                                                        <ext:Column DataIndex="Narrativa1" Header="Detalle 1" Width="100" />
                                                        <ext:Column DataIndex="Narrativa2" Header="Detalle 2" Width="100" />
                                                        <ext:Column DataIndex="Narrativa3" Header="Detalle 3" Width="100" />
                                                    </Columns>
                                                </ColumnModel>
                                                <SelectionModel>
                                                    <ext:CellSelectionModel runat="server">
                                                        <DirectEvents>
                                                            <CellSelect OnEvent="CellGridResultadosIncidencias_Click" />
                                                        </DirectEvents>
                                                    </ext:CellSelectionModel>
                                                </SelectionModel>
                                                <Plugins>
                                                    <ext:GridFilters runat="server" ID="GridFilters3" Local="true">
                                                        <Filters>
                                                            <ext:StringFilter DataIndex="Fecha" />
                                                        </Filters>
                                                    </ext:GridFilters>
                                                </Plugins>
                                                <BottomBar>
                                                    <ext:PagingToolbar ID="PagingToolBar3" runat="server" StoreID="StoreResultadosIncidencias" DisplayInfo="true"
                                                        DisplayMsg="Mostrando Incidencias {0} - {1} de {2}" />
                                                </BottomBar>
                                                <TopBar>
                                                    <ext:Toolbar ID="Toolbar5" runat="server">
                                                        <Items>
                                                            <ext:ToolbarFill ID="ToolbarFill6" runat="server" />
                                                            <ext:Button ID="btnExportExcelInc" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="true">
                                                                <DirectEvents>
                                                                    <Click OnEvent="ExportGridToExcel" IsUpload="true" Success="Ext.Net.DirectMethods.Download({IsUpload:true})">
                                                                        <ExtraParams>
                                                                            <ext:Parameter Name="GridResult" Value="Ext.encode(#{GridResultadosIncidencias}.getRowsValues({selectedOnly : false}))" Mode="Raw" />
                                                                            <ext:Parameter Name="Reporte" Value="I" Mode="Value" />
                                                                        </ExtraParams>
                                                                    </Click>
                                                                </DirectEvents>
                                                            </ext:Button>
                                                            <ext:Button ID="btnExportCSVInc" runat="server" Text="Exportar a CSV" Icon="PageAttach" Disabled="true">
                                                                <Listeners>
                                                                    <Click Handler="submitValue(#{GridResultadosIncidencias}, #{FormatType}, 'csv');" />
                                                                </Listeners>
                                                            </ext:Button>
                                                        </Items>
                                                    </ext:Toolbar>
                                                </TopBar>
                                            </ext:GridPanel>
                                        </Center>
                                    </ext:BorderLayout>
                                </Content>
                            </ext:FormPanel>
                            <ext:FormPanel ID="FormPanelOperaciones" runat="server" Title="Operaciones" Border="false">
                                <Content>
                                    <ext:BorderLayout ID="BorderLayoutOperaciones" runat="server">
                                        <North Split="true">
                                            <ext:FormPanel ID="FormPanelBuscarOperaciones" runat="server" LabelAlign="Right" Border="false">
                                                <TopBar>
                                                    <ext:Toolbar ID="Toolbar2" runat="server">
                                                        <Items>
                                                            <ext:Hidden ID="hdnIdOperacion" runat="server" />
                                                            <ext:DateField ID="dfFechaInicialOper" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                                                AllowBlank="false" MsgTarget="Qtip" Format="yyyy/MM/dd" Width="170" EnableKeyEvents="true"
                                                                 LabelWidth="70">
                                                                <CustomConfig>
                                                                    <ext:ConfigItem Name="endDateFieldOper" Value="#{dfFechaFinalOper}" Mode="Value" />
                                                                </CustomConfig>
                                                                <Listeners>
                                                                    <KeyUp Fn="onKeyUpOper" />
                                                                </Listeners>
                                                            </ext:DateField>
                                                            <ext:DateField ID="dfFechaFinalOper" runat="server" Vtype="daterange" FieldLabel="Fecha Final"
                                                                AllowBlank="false" Width="170" MsgTarget="Qtip" Format="yyyy/MM/dd" EnableKeyEvents="true"
                                                                 LabelWidth="70">
                                                                <CustomConfig>
                                                                    <ext:ConfigItem Name="startDateFieldOper" Value="#{dfFechaInicialOper}" Mode="Value" />
                                                                </CustomConfig>
                                                                <Listeners>
                                                                    <KeyUp Fn="onKeyUpOper" />
                                                                </Listeners>
                                                            </ext:DateField>
                                                            <ext:ComboBox ID="cBoxEstatusOper" runat="server" FieldLabel="Estatus" Width="170" LabelWidth="50">
                                                                <Items>
                                                                    <ext:ListItem Text="Aprobadas" Value="1" />
                                                                    <ext:ListItem Text="Rechazadas" Value="0" />
                                                                    <ext:ListItem Text="( Todas )" Value="-1" />
                                                                </Items>
                                                            </ext:ComboBox>
                                                            <ext:TextField ID="txtAfiliacionOper" runat="server" FieldLabel="Afiliación" Width="150" MaxLength="50" 
                                                                LabelWidth="60" />
                                                            <ext:Button ID="btnBuscarOper" runat="server" Text="Buscar" Icon="Magnifier">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnBuscarOper_Click" Before="var valid= #{FormPanelBuscarOperaciones}.getForm().isValid(); if (!valid) {} return valid;">
                                                                        <EventMask ShowMask="true" Msg="Buscando Operaciones..." MinDelay="500" />
                                                                    </Click>
                                                                </DirectEvents>
                                                            </ext:Button>
                                                            <ext:ToolbarFill ID="ToolbarFill3" runat="server" />
                                                            <ext:Button ID="btnLimpiarOper" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnLimpiarOper_Click" />
                                                                </DirectEvents>
                                                            </ext:Button>
                                                        </Items>
                                                    </ext:Toolbar>
                                                </TopBar>
                                            </ext:FormPanel>
                                        </North>
                                        <Center Split="true">
                                            <ext:GridPanel ID="GridResultadosOper" runat="server" Header="true">
                                                <Store>
                                                    <ext:Store ID="StoreResultadosOper" runat="server" OnRefreshData="btnBuscarOper_Click"
                                                        OnSubmitData="StoreSubmit">
                                                        <DirectEventConfig IsUpload="true" />
                                                        <Reader>
                                                            <ext:JsonReader IDProperty="ID_Operacion">
                                                                <Fields>
                                                                    <ext:RecordField Name="ID_Operacion" />
                                                                    <ext:RecordField Name="Tarjeta" />
                                                                    <ext:RecordField Name="FechaHora" />
                                                                    <ext:RecordField Name="DiaSemana" />
                                                                    <ext:RecordField Name="Estatus" />
                                                                    <ext:RecordField Name="MotivoRechazo" />
                                                                    <ext:RecordField Name="Importe" />
                                                                    <ext:RecordField Name="Autorizacion" />
                                                                    <ext:RecordField Name="Afiliacion" />
                                                                    <ext:RecordField Name="Comercio" />
                                                                    <ext:RecordField Name="Pais" />
                                                                    <ext:RecordField Name="Estado" />
                                                                    <ext:RecordField Name="Poblacion" />
                                                                </Fields>
                                                            </ext:JsonReader>
                                                        </Reader>
                                                        <DirectEventConfig IsUpload="true" />
                                                    </ext:Store>
                                                </Store>
                                                <ColumnModel ID="ColumnModel1" runat="server">
                                                    <Columns>
                                                        <ext:CommandColumn Width="30">
                                                            <Commands>
                                                                <ext:GridCommand Icon="Pencil" CommandName="Delete">
                                                                    <ToolTip Text="Marcar como Incidencia" />
                                                                </ext:GridCommand>
                                                            </Commands>
                                                        </ext:CommandColumn>
                                                        <ext:DateColumn DataIndex="FechaHora" Header="Fecha" Format="yyyy-MM-dd HH:mm:ss"
                                                            Width="120" Sortable="true" />
                                                        <ext:Column DataIndex="DiaSemana" Header="Día Semana" Width="75" Align="Center"/>
                                                        <ext:Column DataIndex="Estatus" Header="Estatus" Width="80" />
                                                        <ext:Column ColumnID="MotivoRechazo" Header="Motivo Rechazo" Sortable="true" 
                                                            DataIndex="MotivoRechazo" Width="120" />
                                                        <ext:Column DataIndex="Importe" Header="Importe" Width="80">
                                                            <Renderer Format="UsMoney" />
                                                        </ext:Column>
                                                        <ext:Column DataIndex="Autorizacion" Header="Autorización" Width="80" />
                                                        <ext:Column DataIndex="Afiliacion" Header="Afiliación" Width="105">
                                                            <Renderer Fn="link" />
                                                        </ext:Column>
                                                        <ext:Column DataIndex="Comercio" Header="Comercio" Width="140" />
                                                        <ext:Column DataIndex="Pais" Header="País" Width="100" />
                                                        <ext:Column DataIndex="Estado" Header="Estado" Width="100" />
                                                        <ext:Column DataIndex="Poblacion" Header="Población" Width="100" />
                                                    </Columns>
                                                </ColumnModel>
                                                <SelectionModel>
                                                    <ext:CellSelectionModel runat="server">
                                                        <DirectEvents>
                                                            <CellSelect OnEvent="CellGridResultadosOper_Click" />
                                                        </DirectEvents>
                                                    </ext:CellSelectionModel>
                                                </SelectionModel>
                                                <Listeners>
                                                    <Command Handler="#{hdnIdOperacion}.setValue(record.data.ID_Operacion); 
                                                        #{txtComentarios}.reset(); #{WdwComentarios}.show();" />
                                                </Listeners>
                                                <Plugins>
                                                    <ext:GridFilters runat="server" ID="GridFilters2" Local="true">
                                                        <Filters>
                                                            <ext:StringFilter DataIndex="FechaHora" />
                                                        </Filters>
                                                    </ext:GridFilters>
                                                </Plugins>
                                                <BottomBar>
                                                    <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="StoreResultadosOper" DisplayInfo="true"
                                                        DisplayMsg="Mostrando Operaciones {0} - {1} de {2}" PageSize="12" />
                                                </BottomBar>
                                                <TopBar>
                                                    <ext:Toolbar ID="Toolbar3" runat="server">
                                                        <Items>
                                                            <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                            <ext:Button ID="btnExportExcelOper" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="true">
                                                                <DirectEvents>
                                                                    <Click OnEvent="ExportGridToExcel" IsUpload="true" Success="Ext.Net.DirectMethods.Download({IsUpload:true})">
                                                                        <ExtraParams>
                                                                            <ext:Parameter Name="GridResult" Value="Ext.encode(#{GridResultadosOper}.getRowsValues({selectedOnly : false}))" Mode="Raw" />
                                                                            <ext:Parameter Name="Reporte" Value="O" Mode="Value" />
                                                                        </ExtraParams>
                                                                    </Click>
                                                                </DirectEvents>
                                                            </ext:Button>
                                                            <ext:Button ID="btnExportCSVOper" runat="server" Text="Exportar a CSV" Icon="PageAttach" Disabled="true">
                                                                <Listeners>
                                                                    <Click Handler="submitValue(#{GridResultadosOper}, #{FormatType}, 'csv');" />
                                                                </Listeners>
                                                            </ext:Button>
                                                        </Items>
                                                    </ext:Toolbar>
                                                </TopBar>
                                            </ext:GridPanel>
                                        </Center>
                                    </ext:BorderLayout>
                                </Content>
                            </ext:FormPanel>
                        </Items>
                    </ext:TabPanel>
                </Items>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:Window ID="WdwComentarios" runat="server" Title="Comentarios" Hidden="true" Width="350" Height="170"
        Modal="true" Resizable="false" Closable="true" >
        <Items>
            <ext:FormPanel ID="FormPanel2" runat="server" Padding="5" HideLabels="true" Border="false">
                <Items>
                    <ext:TextArea ID="txtComentarios" runat="server" AnchorHorizontal="100%" AnchorVertical="103%"
                        EmptyText="Por favor, ingresa los comentarios de la Incidencia Manual (máximo 150 caracteres)"
                        AllowBlank="false" MaxLength="150"/>
                </Items>
            </ext:FormPanel>
        </Items>
        <Buttons>
            <ext:Button ID="btnMarcar" runat="server" Text="Marcar Operación" Icon="Exclamation">
                <DirectEvents>
                    <Click OnEvent="btnMarcar_Click" />
                </DirectEvents>
            </ext:Button>
        </Buttons>
    </ext:Window>
    
    <ext:BorderLayout ID="MainBorderLayout" runat="server">
        <North Split="true">
            <ext:FormPanel ID="FormPanelBusqueda" runat="server" Layout="FitLayout" Border="false">
                <TopBar>
                    <ext:Toolbar ID="Toolbar_1" runat="server">
                        <Items>
                            <ext:Hidden ID="hdnTarjeta" runat="server"/>
                            <ext:Hidden ID="FormatType" runat="server" />
                            <ext:DateField ID="dfFechaInicial" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                AllowBlank="false" MsgTarget="Side" Format="yyyy/MM/dd" LabelWidth="70" 
                                LabelAlign="Right" Width="170" EnableKeyEvents="true" MaxWidth="170">
                                 <CustomConfig>
                                     <ext:ConfigItem Name="endDateField" Value="#{dfFechaFinal}" Mode="Value" />
                                 </CustomConfig>
                                 <Listeners>
                                     <KeyUp Fn="onKeyUp" />
                                 </Listeners>
                            </ext:DateField>
                            <ext:DateField ID="dfFechaFinal" runat="server" Vtype="daterange" FieldLabel="Fecha Final"
                                AllowBlank="false" MaxWidth="170" Width="170" MsgTarget="Side" Format="yyyy/MM/dd"
                                LabelWidth="70" LabelAlign="Right" EnableKeyEvents="true">
                                <CustomConfig>
                                    <ext:ConfigItem Name="startDateField" Value="#{dfFechaInicial}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:TextField ID="txtTarjeta" runat="server" EmptyText="Tarjeta" MaskRe="[0-9]"
                                Width="125" MaxLength="16" MinLength="16"/>
                            <ext:ToolbarSeparator />
                            <ext:Toolbar runat="server" Flat="true" Width="10" />
                            <ext:ComboBox ID="cBoxIncidencias" runat="server" EmptyText="Incidencias" Width="110">
                                <Items>
                                    <ext:ListItem Text="Con Incidencias" Value="1" />
                                    <ext:ListItem Text="Sin Incidencias" Value="0" />
                                    <ext:ListItem Text="( Todas )" Value="-1" />
                                </Items>
                                <Listeners>
                                    <Select Handler="
                                        if(this.getValue() == 0) {
                                            #{cmbRegla}.setDisabled(true);
                                            #{cmbAccion}.setDisabled(true);
                                        } else {
                                            #{cmbRegla}.setDisabled(false);
                                            #{cmbAccion}.setDisabled(false);
                                        }" />
                                </Listeners>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cmbRegla" runat="server" EmptyText="Regla" Resizable="true" ListWidth="200"
                                Width="100" StoreID="StoreReglas" DisplayField="Nombre" ValueField="ID_Regla" Disabled="true">
                                <Items>
                                    <ext:ListItem Text="( Todas )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cmbAccion" runat="server" EmptyText="Acción" Resizable="true" Width="100"
                                StoreID="StoreAcciones" DisplayField="Accion" ValueField="ID_Accion" Disabled="true"> 
                            </ext:ComboBox>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                        Before="var valid= #{FormPanelBusqueda}.getForm().isValid(); if (!valid) {}
                                        else { resetToolbar(#{PagingIncidencias}); #{GridResultados}.getStore().sortInfo = null;}
                                        return valid;">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnBuscarHide" runat="server" Hidden="true">
                                <Listeners>
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Consultando Incidencias...' });
                                        #{GridResultados}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Toolbar runat="server" Flat="true" Width="15" />
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click"/>
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
            </ext:FormPanel>
        </North>
        <Center Split="true">
            <ext:GridPanel ID="GridResultados" runat="server" AutoDoLayout="true" Frame="true" AutoScroll="true">
                <TopBar>
                    <ext:Toolbar ID="Toolbar4" runat="server">
                        <Items>
                            <ext:ToolbarFill ID="ToolbarFill4" runat="server" />
                            <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a  Excel" Icon="PageExcel" Disabled="true">
                                <DirectEvents>
                                    <Click OnEvent="ExportDataTableToExcel" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                        e.stopEvent(); 
                                        ValidacionesBatch.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnExportCSV" runat="server" Text="Exportar a CSV" Icon="PageAttach" Disabled="true">
                                <Listeners>
                                    <Click Handler="submitValue(#{GridResultados}, #{FormatType}, 'csv');" />
                                </Listeners>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <Store>
                    <ext:Store ID="StoreIncidencias" runat="server" OnRefreshData="StoreIncidencias_RefreshData" RemoteSort="true"
                        OnSubmitData="StoreSubmit">
                        <AutoLoadParams>
                            <ext:Parameter Name="startInc" Value="0" Mode="Raw" />
                        </AutoLoadParams>
                        <Proxy>
                            <ext:PageProxy />
                        </Proxy>
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Operacion">
                                <Fields>
                                    <ext:RecordField Name="ID_OperacionRespuestaRegla" />
                                    <ext:RecordField Name="ID_Operacion" />
                                    <ext:RecordField Name="FechaRegistro" />
                                    <ext:RecordField Name="FechaOperacion" />
                                    <ext:RecordField Name="DiaSemana" />
                                    <ext:RecordField Name="Tarjeta" />
                                    <ext:RecordField Name="ID_Regla" />
                                    <ext:RecordField Name="Regla" />
                                    <ext:RecordField Name="Accion" />
                                    <ext:RecordField Name="Incidencias" />
                                    <ext:RecordField Name="ReglasRotas" />
                                    <ext:RecordField Name="Estatus" />
                                    <ext:RecordField Name="MotivoRechazo" />
                                    <ext:RecordField Name="Afiliacion" />
                                    <ext:RecordField Name="Comercio" />
                                    <ext:RecordField Name="Pais" />
                                    <ext:RecordField Name="Narrativa1" />
                                    <ext:RecordField Name="Narrativa2" />
                                    <ext:RecordField Name="Narrativa3" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <ColumnModel runat="server">
                    <Columns>
                        <ext:Column ColumnID="ID_OperacionRespuestaRegla" Hidden="true" DataIndex="ID_OperacionRespuestaRegla" />
                        <ext:Column ColumnID="ID_Operacion" Hidden="true" DataIndex="ID_Operacion" />
                        <ext:DateColumn ColumnID="FechaRegistro" Header="Fecha de Registro" Sortable="true" DataIndex="FechaRegistro"
                            Format="dd-MM-yyyy HH:mm:ss" Width="120"/>
                        <ext:Column ColumnID="FechaOperacion" Header="Fecha de la Operación" DataIndex="FechaOperacion"
                            Width="120" Align="Center" />
                        <ext:Column ColumnID="DiaSemana" Header="Día Semana" Sortable="true" DataIndex="DiaSemana" Width="75" 
                            Align="Center"/>
                        <ext:Column ColumnID="Tarjeta" Header="Tarjeta" Sortable="true" DataIndex="Tarjeta" Width="120">
                            <Renderer Fn="link" />
                        </ext:Column>
                        <ext:Column ColumnID="Regla" Header="Regla" Sortable="true" DataIndex="Regla" Width="140" />
                        <ext:Column ColumnID="Accion" Header="Acción" Sortable="true" DataIndex="Accion" Width="60" />
                        <ext:Column ColumnID="Incidencias" Header="Incidencias" Sortable="true" DataIndex="Incidencias"
                            Width="70" Align="Center"/>
                        <ext:Column ColumnID="ReglasRotas" Header="Reglas Rotas" Sortable="true" DataIndex="ReglasRotas"
                            Width="80" Align="Center"/>
                        <ext:Column ColumnID="Estatus" Header="Estatus" Sortable="true" DataIndex="Estatus" Width="90" />
                        <ext:Column ColumnID="MotivoRechazo" Header="Motivo Rechazo" Sortable="true" 
                            DataIndex="MotivoRechazo" Width="120" />
                        <ext:Column ColumnID="Afiliacion" Header="Afiliación" Sortable="true" DataIndex="Afiliacion" Width="105">
                            <Renderer Fn="link" />
                        </ext:Column>
                        <ext:Column ColumnID="Comercio" Header="Comercio" Sortable="true" DataIndex="Comercio" Width="140"/>
                        <ext:Column ColumnID="Pais" Header="País" Sortable="true" DataIndex="Pais" Width="100"/>
                        <ext:Column ColumnID="Narrativa1" Header="Detalle 1" DataIndex="Narrativa1" />
                        <ext:Column ColumnID="Narrativa2" Header="Detalle 2" DataIndex="Narrativa2" />
                        <ext:Column ColumnID="Narrativa3" Header="Detalle 3" DataIndex="Narrativa3" />
                    </Columns>
                </ColumnModel>
                <SelectionModel>
                    <ext:CellSelectionModel runat="server">
                        <DirectEvents>
                            <CellSelect OnEvent="Cell_Click"/>
                        </DirectEvents>
                    </ext:CellSelectionModel>
                </SelectionModel>
                <Plugins>
                    <ext:GridFilters runat="server" ID="GridFilters1" Local="true">
                        <Filters>
                            <ext:StringFilter DataIndex="Fecha" />
                            <ext:StringFilter DataIndex="Tarjeta" />
                            <ext:StringFilter DataIndex="Regla" />
                            <ext:StringFilter DataIndex="Accion" />
                            <ext:StringFilter DataIndex="Afiliacion" />
                            <ext:StringFilter DataIndex="NombreORazonSocial" />
                        </Filters>
                    </ext:GridFilters>
                </Plugins>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingIncidencias" runat="server" StoreID="StoreIncidencias"
                        DisplayInfo="true" DisplayMsg="Mostrando Incidencias {0} - {1} de {2}">
                       <%-- <Items>
                            <ext:ToolbarFill ID="ToolbarFill5" runat="server" />
                            <ext:NumberField ID="nmbTop" runat="server" AllowBlank="false" FieldLabel="Registros a Mostrar "
                                LabelWidth="120" LabelAlign="Right" Text="100" Width="175" Disabled="true" MinValue="1"
                                MaxValue="1000">
                                <Listeners>
                                    <SpecialKey Handler="if(e.getKey() == e.ENTER) {
                                        resetToolbar(#{PagingIncidencias}); 
                                        #{GridResultados}.getStore().sortInfo = null;
                                        e.stopEvent();
                                        ValidacionesBatch.CambiaNumRegistros();}" />
                                    <SpecialKey Handler="if(ef.getKey() == e.ENTER) {
                                        Ext.net.Mask.show({ msg : 'Buscando Incidencias...' });
                                        #{GridResultados}.store.reload();
                                        e.stopEvent(); 
                                        ValidacionesBatch.StopMask(); }" />
                                </Listeners>
                            </ext:NumberField>
                        </Items>--%>
                    </ext:PagingToolbar>
                </BottomBar>
            </ext:GridPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
