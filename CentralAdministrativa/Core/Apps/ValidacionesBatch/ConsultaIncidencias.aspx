<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ConsultaIncidencias.aspx.cs" Inherits="ValidacionesBatch.ConsultaIncidencias" %>

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
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
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

    <ext:Window ID="WdwDetalleTarjeta" runat="server" Title="Tarjeta: " Width="900" Height="500" Resizable="False"
        Hidden="true" Closable="true" Modal="true" Layout="FitLayout" Draggable="true" Padding="5">
        <Items>
            <ext:FormPanel ID="FormPanel1" runat="server" Layout="FitLayout">
                <Items>
                    <ext:TabPanel ID="TabPanel1" runat="server">
                        <Items>
                            <ext:FormPanel ID="FormPanelOperaciones" runat="server" Title="Operaciones">
                                <Content>
                                    <ext:BorderLayout ID="BorderLayoutOperaciones" runat="server">
                                        <North Split="true">
                                            <ext:FormPanel ID="FormPanelBuscarOperaciones" runat="server" LabelWidth="70" LabelAlign="Right">
                                                <TopBar>
                                                    <ext:Toolbar ID="ToolbarOper" runat="server">
                                                        <Items>
                                                            <ext:DateField ID="dfFechaInicialOper" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                                                AllowBlank="false" MsgTarget="Qtip" Format="yyyy/MM/dd" Width="200" MaxWidth="200"
                                                                EnableKeyEvents="true">
                                                                <CustomConfig>
                                                                    <ext:ConfigItem Name="endDateFieldOper" Value="#{dfFechaFinalOper}" Mode="Value" />
                                                                </CustomConfig>
                                                                <Listeners>
                                                                    <KeyUp Fn="onKeyUpOper" />
                                                                </Listeners>
                                                            </ext:DateField>
                                                            <ext:DateField ID="dfFechaFinalOper" runat="server" Vtype="daterange" FieldLabel="Fecha Final"
                                                                AllowBlank="false" Width="200" MsgTarget="Qtip" Format="yyyy/MM/dd"
                                                                EnableKeyEvents="true">
                                                                <CustomConfig>
                                                                    <ext:ConfigItem Name="startDateFieldOper" Value="#{dfFechaInicialOper}" Mode="Value" />
                                                                </CustomConfig>
                                                                <Listeners>
                                                                    <KeyUp Fn="onKeyUpOper" />
                                                                </Listeners>
                                                            </ext:DateField>
                                                            <ext:TextField ID="txtAfiliacionOper" runat="server" FieldLabel="Afiliación"
                                                                Width="150" MaxLength="50" />
                                                            <ext:Button ID="btnBuscarOper" runat="server" Text="Buscar" Icon="Magnifier">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnBuscarOper_Click" Before="var valid= #{FormPanelBuscarOperaciones}.getForm().isValid(); if (!valid) {} return valid;">
                                                                        <EventMask ShowMask="true" Msg="Buscando Operaciones..." MinDelay="500" />
                                                                    </Click>
                                                                </DirectEvents>
                                                            </ext:Button>
                                                            <ext:ToolbarFill ID="dummy" runat="server" />
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
                                                        <DirectEventConfig IsUpload="true"/>
                                                        <Reader>
                                                            <ext:JsonReader IDProperty="FechaHora">
                                                                <Fields>
                                                                    <ext:RecordField Name="FechaHora" />
                                                                    <ext:RecordField Name="DiaSemana" />
                                                                    <ext:RecordField Name="Tarjeta" />
                                                                    <ext:RecordField Name="Movimiento" />
                                                                    <ext:RecordField Name="Importe" />
                                                                    <ext:RecordField Name="Saldo" />
                                                                    <ext:RecordField Name="Autorizacion" />
                                                                    <ext:RecordField Name="Comercio" />
                                                                    <ext:RecordField Name="Afiliacion" />
                                                                    <ext:RecordField Name="Estado" />
                                                                    <ext:RecordField Name="Poblacion" />
                                                                </Fields>
                                                            </ext:JsonReader>
                                                        </Reader>
                                                        <DirectEventConfig IsUpload="true" />
                                                    </ext:Store>
                                                </Store>
                                                <ColumnModel ID="ColumnModel2" runat="server">
                                                    <Columns>
                                                        <ext:DateColumn DataIndex="FechaHora" Header="Fecha" Format="yyyy-MM-dd HH:mm:ss"
                                                            Width="120" Sortable="true" />
                                                        <ext:Column DataIndex="DiaSemana" Header="Día Semana" Width="75" />
                                                        <ext:Column DataIndex="Movimiento" Header="Movimiento" Width="150" />
                                                        <ext:Column DataIndex="Importe" Header="Importe" Width="80">
                                                            <Renderer Format="UsMoney" />
                                                        </ext:Column>
                                                        <ext:Column DataIndex="Saldo" Header="Saldo" Width="80">
                                                            <Renderer Format="UsMoney" />
                                                        </ext:Column>
                                                        <ext:Column DataIndex="Autorizacion" Header="Autorización" Width="80" />
                                                        <ext:Column DataIndex="Comercio" Header="Comercio" Width="200" />
                                                        <ext:Column DataIndex="Afiliacion" Header="Afiliación" Width="60">
                                                            <Renderer Fn="link" />
                                                        </ext:Column>
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
                                                <Plugins>
                                                    <ext:GridFilters runat="server" ID="GridFilters2" Local="true">
                                                        <Filters>
                                                            <ext:StringFilter DataIndex="FechaHora" />
                                                        </Filters>
                                                    </ext:GridFilters>
                                                </Plugins>
                                                <BottomBar>
                                                    <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="StoreResultadosOper" DisplayInfo="true"
                                                        DisplayMsg="Mostrando Operaciones {0} - {1} de {2}" />
                                                </BottomBar>
                                                <TopBar>
                                                    <ext:Toolbar ID="Toolbar1" runat="server">
                                                        <Items>
                                                            <ext:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                            <ext:Button ID="Button3" runat="server" Text="Exportar a Excel" Icon="PageExcel">
                                                                <Listeners>
                                                                    <Click Handler="submitValue(#{GridResultadosOper}, #{FormatType}, 'xls');" />
                                                                </Listeners>
                                                            </ext:Button>
                                                            <ext:Button ID="Button4" runat="server" Text="Exportar a CSV" Icon="PageAttach">
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
                            <ext:FormPanel ID="FormPanelIncidencias" runat="server" Title="Incidencias">
                                <Content>
                                    <ext:BorderLayout ID="BorderLayoutIncidencias" runat="server">
                                        <North Split="true">
                                            <ext:FormPanel ID="FormPanelBuscaIncidencias" runat="server" LabelWidth="70" LabelAlign="Right">
                                                <TopBar>
                                                    <ext:Toolbar ID="Toolbar2" runat="server">
                                                        <Items>
                                                            <ext:DateField ID="dfFechaInicialInc" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                                                AllowBlank="false" MsgTarget="Qtip" Format="yyyy/MM/dd" LabelWidth="70"
                                                                LabelAlign="Right" Width="200" EnableKeyEvents="true" MaxWidth="200">
                                                                <CustomConfig>
                                                                    <ext:ConfigItem Name="endDateField" Value="#{dfFechaFinalInc}" Mode="Value" />
                                                                </CustomConfig>
                                                                <Listeners>
                                                                    <KeyUp Fn="onKeyUpInc" />
                                                                </Listeners>
                                                            </ext:DateField>
                                                            <ext:DateField ID="dfFechaFinalInc" runat="server" Vtype="daterange" FieldLabel="Fecha Final"
                                                                AllowBlank="false" MaxWidth="200" Width="200" MsgTarget="Qtip" Format="yyyy/MM/dd"
                                                                LabelWidth="70" LabelAlign="Right" EnableKeyEvents="true">
                                                                <CustomConfig>
                                                                    <ext:ConfigItem Name="startDateField" Value="#{dfFechaInicialInc}" Mode="Value" />
                                                                </CustomConfig>
                                                                <Listeners>
                                                                    <KeyUp Fn="onKeyUpInc" />
                                                                </Listeners>
                                                            </ext:DateField>
                                                            <ext:ComboBox ID="cmbReglasInc" runat="server" EmptyText="Regla" Resizable="true" ListWidth="250"
                                                                Width="150" StoreID="StoreReglasInc" DisplayField="Nombre" ValueField="ID_Regla">
                                                                <Items>
                                                                    <ext:ListItem Text="( Todas )" Value="-1" />
                                                                </Items>
                                                            </ext:ComboBox>
                                                            <ext:ComboBox ID="cmbAccionInc" runat="server" EmptyText="Acción" Resizable="true" Width="100">
                                                                <Items>
                                                                    <ext:ListItem Text="( Todas )" Value="-1" />
                                                                    <ext:ListItem Text="Informar" Value="1" />
                                                                    <ext:ListItem Text="Bloquear" Value="2" />
                                                                    <ext:ListItem Text="Cancelar" Value="3" />
                                                                </Items>
                                                            </ext:ComboBox>
                                                            <ext:Button ID="btnBuscaInc" runat="server" Text="Buscar" Icon="Magnifier">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnBuscaInc_Click" Before="var valid= #{FormPanelBuscaIncidencias}.getForm().isValid(); if (!valid) {} return valid;">
                                                                        <EventMask ShowMask="true" Msg="Buscando Incidencias..." MinDelay="500" />
                                                                    </Click>
                                                                </DirectEvents>
                                                            </ext:Button>
                                                            <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                            <ext:Button ID="btnLimpiaInc" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnLimpiaInc_Click" />
                                                                </DirectEvents>
                                                            </ext:Button>
                                                        </Items>
                                                    </ext:Toolbar>
                                                </TopBar>
                                                <Content>
                                                    <ext:Store ID="StoreReglasInc" runat="server">
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
                                                </Content>
                                            </ext:FormPanel>
                                        </North>
                                        <Center Split="true">
                                            <ext:GridPanel ID="GridResultadosIncidencias" runat="server" Header="true">
                                                <Store>
                                                    <ext:Store ID="StoreResultadosIncidencias" runat="server" OnRefreshData="btnBuscaInc_Click"
                                                        OnSubmitData="StoreSubmit">
                                                        <DirectEventConfig IsUpload="true" />
                                                        <Reader>
                                                            <ext:JsonReader IDProperty="ID_Operacion">
                                                                <Fields>
                                                                    <ext:RecordField Name="ID_Operacion" />
                                                                    <ext:RecordField Name="Fecha" />
                                                                    <ext:RecordField Name="DiaSemana" />
                                                                    <ext:RecordField Name="Tarjeta" />
                                                                    <ext:RecordField Name="Regla" />
                                                                    <ext:RecordField Name="Accion" />
                                                                    <ext:RecordField Name="Incidencias" />
                                                                    <ext:RecordField Name="ReglasRotas" />
                                                                    <ext:RecordField Name="Estatus" />
                                                                    <ext:RecordField Name="Afiliacion" />
                                                                    <ext:RecordField Name="RazonSocial" />
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
                                                        <ext:DateColumn DataIndex="Fecha" Header="Fecha" Format="dd-MM-yyyy" Width="70" />
                                                        <ext:Column DataIndex="DiaSemana" Header="Día Semana" Width="75" />
                                                        <ext:Column DataIndex="Regla" Header="Regla" Width="150" />
                                                        <ext:Column DataIndex="Accion" Header="Acción Sugerida" Width="100" />
                                                        <ext:Column DataIndex="Incidencias" Header="Incidencias" Width="70" />
                                                        <ext:Column DataIndex="ReglasRotas" Header="Reglas Rotas" Width="80" />
                                                        <ext:Column DataIndex="Estatus" Header="Estatus" Width="95" />
                                                        <ext:Column DataIndex="Afiliacion" Header="Afiliación" Width="60">
                                                            <Renderer Fn="link" />
                                                        </ext:Column>
                                                        <ext:Column DataIndex="RazonSocial" Header="Razón Social" Width="150" />
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
                                                    <ext:Toolbar ID="Toolbar3" runat="server">
                                                        <Items>
                                                            <ext:ToolbarFill ID="ToolbarFill3" runat="server" />
                                                            <ext:Button ID="Button1" runat="server" Text="Exportar a Excel" Icon="PageExcel">
                                                                <Listeners>
                                                                    <Click Handler="submitValue(#{GridResultadosIncidencias}, #{FormatType}, 'xls');" />
                                                                </Listeners>
                                                            </ext:Button>
                                                            <ext:Button ID="Button2" runat="server" Text="Exportar a CSV" Icon="PageAttach">
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
                        </Items>
                    </ext:TabPanel>
                </Items>
            </ext:FormPanel>
        </Items>
    </ext:Window>

    <ext:BorderLayout ID="MainBorderLayout" runat="server">
        <North Split="true">
            <ext:FormPanel ID="FormPanelBusqueda" runat="server" Border="true" Layout="FitLayout" 
                Title="Ingresa los datos para la búsqueda" AutoScroll="true">
                <TopBar>
                    <ext:Toolbar ID="tbCadenaComercial" runat="server">
                        <Items>
                            <ext:Hidden ID="hdnTarjeta" runat="server"/>
                            <ext:Hidden ID="FormatType" runat="server" />
                            <ext:DateField ID="dfFechaInicial" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                AllowBlank="false" MsgTarget="Side" Format="yyyy/MM/dd" LabelWidth="70" 
                                LabelAlign="Right" Width="200" EnableKeyEvents="true" MaxWidth="200">
                                 <CustomConfig>
                                     <ext:ConfigItem Name="endDateField" Value="#{dfFechaFinal}" Mode="Value" />
                                 </CustomConfig>
                                 <Listeners>
                                     <KeyUp Fn="onKeyUp" />
                                 </Listeners>
                            </ext:DateField>
                            <ext:DateField ID="dfFechaFinal" runat="server" Vtype="daterange" FieldLabel="Fecha Final"
                                AllowBlank="false" MaxWidth="200" Width="200" MsgTarget="Side" Format="yyyy/MM/dd"
                                LabelWidth="70" LabelAlign="Right" EnableKeyEvents="true">
                                <CustomConfig>
                                    <ext:ConfigItem Name="startDateField" Value="#{dfFechaInicial}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:ComboBox ID="cmbRegla" runat="server" EmptyText="Regla" Resizable="true" ListWidth="250"
                                Width="150" StoreID="StoreReglas" DisplayField="Nombre" ValueField="ID_Regla">
                                <Items>
                                    <ext:ListItem Text="( Todas )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cmbAccion" runat="server" EmptyText="Acción" Resizable="true" Width="100"> 
                                <Items>
                                    <ext:ListItem Text="( Todas )" Value="-1" />
                                    <ext:ListItem Text="Informar" Value="1" />
                                    <ext:ListItem Text="Bloquear" Value="2" />
                                    <ext:ListItem Text="Cancelar" Value="3" />
                                </Items>
                            </ext:ComboBox>
                            <ext:TextField ID="txtTarjeta" runat="server" EmptyText="Tarjeta"
                                Width="150" MaxLength="16" MinLength="16"/>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Before="var valid= #{FormPanelBusqueda}.getForm().isValid(); if (!valid) {} return valid;">
                                        <EventMask ShowMask="true" Msg="Consultando Incidencias..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click"/>
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <Content>
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
                </Content>
            </ext:FormPanel>
        </North>
        <Center Split="true">
            <ext:GridPanel ID="GridIncidencias" runat="server" Layout="FitLayout" Frame="true" AutoScroll="true">
                <LoadMask ShowMask="false" />
                <Store>
                    <ext:Store ID="StoreIncidencias" runat="server" OnRefreshData="btnBuscar_Click" OnSubmitData="StoreSubmit">
                        <DirectEventConfig IsUpload="true"/>
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Operacion">
                                <Fields>
                                    <ext:RecordField Name="ID_Operacion" />
                                    <ext:RecordField Name="Fecha" />
                                    <ext:RecordField Name="DiaSemana" />
                                    <ext:RecordField Name="Tarjeta" />
                                    <ext:RecordField Name="ID_Regla" />
                                    <ext:RecordField Name="Regla" />
                                    <ext:RecordField Name="Accion" />
                                    <ext:RecordField Name="Incidencias" />
                                    <ext:RecordField Name="ReglasRotas" />
                                    <ext:RecordField Name="Estatus" />
                                    <ext:RecordField Name="Afiliacion" />
                                    <ext:RecordField Name="RazonSocial" />
                                    <ext:RecordField Name="Narrativa1" />
                                    <ext:RecordField Name="Narrativa2" />
                                    <ext:RecordField Name="Narrativa3" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <DirectEventConfig IsUpload="true"/>
                    </ext:Store>
                </Store>
                <ColumnModel runat="server">
                    <Columns>
                        <ext:Column ColumnID="ID_Operacion" Hidden="true" DataIndex="ID_Operacion" />
                        <ext:DateColumn ColumnID="Fecha" Header="Fecha" Sortable="true" DataIndex="Fecha"
                            Format="dd-MM-yyyy" Width="70"/>
                        <ext:Column ColumnID="DiaSemana" Header="Día Semana" Sortable="true" DataIndex="DiaSemana" Width="75" />
                        <ext:Column ColumnID="Tarjeta" Header="Tarjeta" Sortable="true" DataIndex="Tarjeta" Width="120">
                            <Renderer Fn="link" />
                        </ext:Column>
                        <ext:Column ColumnID="Regla" Header="Regla" Sortable="true" DataIndex="Regla" Width="140" />
                        <ext:Column ColumnID="Accion" Header="Acción Sugerida" Sortable="true" DataIndex="Accion" Width="95" />
                        <ext:Column ColumnID="Incidencias" Header="Incidencias" Sortable="true" DataIndex="Incidencias" Width="70" />
                        <ext:Column ColumnID="ReglasRotas" Header="Reglas Rotas" Sortable="true" DataIndex="ReglasRotas" Width="80" />
                        <ext:Column ColumnID="Estatus" Header="Estatus" Sortable="true" DataIndex="Estatus" Width="80" />
                        <ext:Column ColumnID="Afiliacion" Header="Afiliación" Sortable="true" DataIndex="Afiliacion"  Width="60">
                            <Renderer Fn="link" />
                        </ext:Column>
                        <ext:Column ColumnID="ID_Colectiva" Hidden="true" DataIndex="ID_Colectiva" />
                        <ext:Column ColumnID="RazonSocial" Header="Razón Social" Sortable="true" DataIndex="RazonSocial" Width="140"/>
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
                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreIncidencias"
                        DisplayInfo="true" DisplayMsg="Mostrando Incidencias {0} - {1} de {2}"/>
                </BottomBar>
                <TopBar>
                    <ext:Toolbar ID="Toolbar4" runat="server">
                        <Items>
                            <ext:ToolbarFill ID="ToolbarFill4" runat="server" />
                            <ext:Button ID="Button5" runat="server" Text="Exportar a Excel" Icon="PageExcel">
                                <Listeners>
                                    <Click Handler="submitValue(#{GridIncidencias}, #{FormatType}, 'xls');" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="Button6" runat="server" Text="Exportar a CSV" Icon="PageAttach">
                                <Listeners>
                                    <Click Handler="submitValue(#{GridIncidencias}, #{FormatType}, 'csv');" />
                                </Listeners>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
            </ext:GridPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
