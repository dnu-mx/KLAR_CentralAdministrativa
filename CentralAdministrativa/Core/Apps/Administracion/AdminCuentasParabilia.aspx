<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="AdminCuentasParabilia.aspx.cs" Inherits="Administracion.AdminCuentasParabilia" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .chng-red-nf-css
        {
            color:red;
            font-weight:bold;
        }
    </style>
    <style type="text/css">
        .chng-blue-nf-css
        {
            color:blue;
            font-weight:bold;
        }
    </style>
    <style type="text/css">
        .x-grid3-row-body p {
            margin: 3px 3px 7px 3px !important;
            width: 99%;
            color: black;
        }
    </style>

    <script type="text/javascript">
        var prepareToolbar = function (grid, toolbar, rowIndex, record) {
            if (record.get("Estatus") == 1) { //Activa
                toolbar.items.get(0).hide();
            } else if (record.get("Estatus") == 0) { //Inactiva, Bloqueada
                toolbar.items.get(1).hide();
            } else { //if (record.get("Estatus") == 2)  //Cancelada y cualquier otro estatus
                toolbar.items.get(0).hide();
                toolbar.items.get(1).hide();
            }
        }

        var prepareTB_CtasParams = function (grid, toolbar, rowIndex, record) {
            if (record.get("esAutorizable") == 1) {
                toolbar.items.get(0).hide();
                toolbar.items.get(1).hide();
            }
        }

        function resetToolbar(tbar) {
            tbar.updateInfo();
            tbar.inputItem.setValue(1);
            tbar.afterTextItem.setText(String.format(tbar.afterPageText, 1));
            tbar.next.setDisabled(true);
            tbar.prev.setDisabled(true);
            tbar.first.setDisabled(true);
            tbar.last.setDisabled(true);
        }
        
        var fullName = function (value, metadata, record, rowIndex, colIndex, store) {
            return "<b>" + record.data.Nombre + "</b>";
        };

        var fnModificarLDC = function (btn) {
            if (btn == 'yes') {
                Ext.net.Mask.show({ msg: 'Procesando...' });
                AdminCtasParab.ModificarLDC();
            }
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

        function formatNumber(value, format) {
            /// <summary> format a number using specified format string </summary>
            return new Number(value).numberFormat(format);
        }

        function formatNumberRenderer(format) {
            /// <summary> return curry function to formatNumber.  Used for grid column renderer. </summary>
            return function(value) {
                return formatNumber(value, format);
            }
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Hidden ID="hdnValorIVA" runat="server" />
    <ext:Hidden ID="hdnIdCuentaLDC" runat="server" />
    <ext:Hidden ID="hdnIdCuentaCC" runat="server" />
    <ext:Hidden ID="hdnIdCuentaCDC" runat="server" />
    <ext:Hidden ID="hdnIdMA" runat="server" />
    <ext:Hidden ID="hdnTarjeta" runat="server" />
    <ext:Hidden ID="hdnMaskCard" runat="server" />
    <ext:Hidden ID="hdnIdColectiva" runat="server" />
    <ext:Hidden ID="hdnIdCadena" runat="server" />
    <ext:Hidden ID="hdnValorIniPMA" runat="server" />
    <ext:Hidden ID="hdnValorFinPMA" runat="server" />
    <ext:Window ID="WdwValorParametro" runat="server" Title="Editar Valor Parámetro" Width="420" AutoHeight="true" Hidden="true"
        Resizable="false">
        <Items>
            <ext:FormPanel ID="FormPanelValorParamTxt" runat="server" Padding="10" MonitorValid="true" LabelAlign="Left" LabelWidth="70">
                <Items>
                    <ext:TextField ID="txtParametro" runat="server" FieldLabel="Descripción" Width="300"
                        AllowBlank="false" Selectable="false" ReadOnly="true" />
                    <ext:TextField ID="txtValorParFloat" runat="server" FieldLabel="Valor" Width="300" MaxLength="50"
                        MaskRe="[0-9\.]" Hidden="true" Regex="^-?[0-9]*(\.[0-9]{1,4})?$">
                        <Listeners>
                            <Change Handler="var inicial = parseFloat(#{hdnValorIniPMA}.getValue());
                                var final = parseFloat(#{hdnValorFinPMA}.getValue());
                                var actual = parseFloat(this.getValue());
                                var _vmsg = 'El valor del parámetro debe estar entre ' + inicial + ' y ' + final;
                                if (actual < inicial || actual > final) {
                                    this.clear();
                                    Ext.MessageBox.show({
                                        icon: Ext.MessageBox.ERROR,
                                        title: 'Valor erróneo',
                                        msg: _vmsg,
                                        buttons: Ext.MessageBox.OK,
                                        });
                                    return false; }" />
                        </Listeners>
                    </ext:TextField>
                    <ext:TextField ID="txtValorParInt" runat="server" FieldLabel="Valor" Width="300" MaxLength="50"
                        MaskRe="[0-9]" Hidden="true">
                        <Listeners>
                            <Change Handler="var inicial = parseInt(#{hdnValorIniPMA}.getValue());
                                var final = parseInt(#{hdnValorFinPMA}.getValue());
                                var actual = parseInt(this.getValue());
                                var _vmsg = 'El valor del parámetro debe estar entre ' + inicial + ' y ' + final;
                                if (actual < inicial || actual > final) {
                                    this.clear();
                                    Ext.MessageBox.show({
                                        icon: Ext.MessageBox.ERROR,
                                        title: 'Valor erróneo',
                                        msg: _vmsg,
                                        buttons: Ext.MessageBox.OK,
                                        });
                                    return false; }" />
                        </Listeners>
                    </ext:TextField>
                    <ext:TextArea ID="txtValorParString" runat="server" FieldLabel="Valor" Width="300" AutoHeight="true"
                        MaxLength="5000" Hidden="true" />
                    <ext:ComboBox ID="cBoxValorPar" runat="server" FieldLabel="Valor" Width="300" Hidden="true">
                        <Items>
                            <ext:ListItem Text="Sí" Value="true" />
                            <ext:ListItem Text="No" Value="false" />
                        </Items>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cBoxCatalogoPMA" runat="server" FieldLabel="Valor" Width="300" Hidden="true"
                        ValueField="ID_ValorPreePMA" DisplayField="Descripcion">
                        <Store>
                            <ext:Store ID="StoreCatalogoPMA" runat="server">
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_ValorPreePMA">
                                        <Fields>
                                            <ext:RecordField Name="ID_ValorPreePMA" />
                                            <ext:RecordField Name="Clave" />
                                            <ext:RecordField Name="Descripcion" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                    </ext:ComboBox>
                </Items>
                <Buttons>
                    <ext:Button runat="server" Text="Cancelar" Icon="Cancel">
                        <Listeners>
                            <Click Handler="#{WdwValorParametro}.hide();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button runat="server" Text="Guardar Cambio" Icon="Disk">
                        <DirectEvents>
                            <Click OnEvent="btnGuardarValorParametro_Click"
                                Before="if ((#{txtValorParFloat}.hidden == false) && (!#{txtValorParFloat}.isValid())) { return false; }" />
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:Window ID="WdwDetalleMovs" runat="server" Title="Detalle de Movimientos " Width="900" Height="460" Resizable="False"
        Hidden="true" Closable="true" Modal="true" Layout="FitLayout">
        <Items>
            <ext:Panel runat="server" Layout="FitLayout">
                <Content>
                    <ext:BorderLayout runat="server">
                        <North Split="true">
                            <ext:GridPanel ID="GridDetMovs" runat="server" Header="true" Border="false" Title="Movimientos" Height="240">
                                <Store>
                                    <ext:Store ID="StoreSaldosYMovs" runat="server" RemoteSort="true" AutoLoad="false"
                                        OnRefreshData="StoreSaldosYMovs_RefreshData">
                                        <AutoLoadParams>
                                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                        </AutoLoadParams>
                                        <Proxy>
                                            <ext:PageProxy />
                                        </Proxy>
                                        <DirectEventConfig IsUpload="true" />
                                        <Reader>
                                            <ext:JsonReader IDProperty="IDReporte">
                                                <Fields>
                                                    <ext:RecordField Name="IDReporte" />
                                                    <ext:RecordField Name="FechaValor" />
                                                    <ext:RecordField Name="FechaPoliza" />
                                                    <ext:RecordField Name="Concepto" />
                                                    <ext:RecordField Name="Cargo" />
                                                    <ext:RecordField Name="Abono" />
                                                    <ext:RecordField Name="Autorizacion" />
                                                    <ext:RecordField Name="ReferenciaPagoServicio" />
                                                    <ext:RecordField Name="ReferenciaNumerica" />
                                                    <ext:RecordField Name="Observaciones" />
                                                    <ext:RecordField Name="SaldoAntes" />
                                                    <ext:RecordField Name="SaldoDespues" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                        <DirectEventConfig IsUpload="true" />
                                    </ext:Store>
                                </Store>
                                <ColumnModel ID="ColumnModel2" runat="server">
                                    <Columns>
                                        <ext:Column Hidden="true" DataIndex="IDReporte" />
                                        <ext:DateColumn Header="Fecha Valor" DataIndex="FechaValor" Sortable="true" Width="110"
                                            Format="yyyy-MM-dd HH:mm:ss" />
                                        <ext:DateColumn Header="Fecha" DataIndex="FechaPoliza" Sortable="true" Width="110"
                                            Format="yyyy-MM-dd HH:mm:ss" />
                                        <ext:Column Header="Concepto" DataIndex="Concepto" Sortable="true" Width="200" />
                                        <ext:NumberColumn Header="Cargo" DataIndex="Cargo" Sortable="true" Width="90" Align="Right"
                                            Format="$0,0.00">
                                            <Renderer Format="UsMoney" />
                                        </ext:NumberColumn>
                                        <ext:NumberColumn Header="Abono" DataIndex="Abono" Sortable="true" Width="90" Align="Right"
                                            Format="$0,0.00">
                                            <Renderer Format="UsMoney" />
                                        </ext:NumberColumn>
                                        <ext:Column Header="Saldo Antes" DataIndex="SaldoAntes" Sortable="true" Width="100" Align="Right">
                                            <Renderer Format="UsMoney" />
                                        </ext:Column>
                                        <ext:Column Header="Saldo Después" DataIndex="SaldoDespues" Sortable="true" Width="100" Align="Right">
                                            <Renderer Format="UsMoney" />
                                        </ext:Column>
                                        <ext:Column Header="Referencia" DataIndex="ReferenciaPagoServicio" Sortable="true" Width="100" />
                                        <ext:Column Header="Autorización" DataIndex="Autorizacion" Sortable="true" Width="90" />
                                        <ext:Column Header="Ref. Num." DataIndex="ReferenciaNumerica" Sortable="true" Width="80" />
                                        <ext:Column Header="Observaciones" DataIndex="Observaciones" Sortable="true" Width="200"/>
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel SingleSelect="true" />
                                </SelectionModel>
                                <TopBar>
                                    <ext:Toolbar runat="server">
                                        <Items>
                                            <ext:ToolbarFill runat="server" />
                                            <ext:ToolbarSeparator runat="server" />
                                            <ext:Button ID="btnExcelSaldosYMovs" runat="server" Text="Exportar a Excel" Icon="PageExcel"
                                                ToolTip="Obtener Datos en un Archivo Excel">
                                                <DirectEvents>
                                                    <Click OnEvent="DownloadSaldosyMovs" IsUpload="true"
                                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                                e.stopEvent(); 
                                                                AdminCtasParab.StopMask();" />
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingTBSaldosYMovs" runat="server" StoreID="StoreSaldosYMovs" DisplayInfo="true"
                                        DisplayMsg="{0} - {1} de {2}" HideRefresh="true" PageSize="10">
                                        <Items>
                                            <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                                <DirectEvents>
                                                    <Click OnEvent="DownloadSaldosyMovs" IsUpload="true"
                                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                                AdminCtasParab.StopMask();" />
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:PagingToolbar>
                                </BottomBar>
                            </ext:GridPanel>
                        </North>
                        <Center Split="true">
                            <ext:GridPanel ID="GridOpsPendientes" runat="server" Header="true" Border="false" Title="Movimientos Pendientes en Tránsito">
                                <Store>
                                    <ext:Store ID="StoreOpsPendientes" runat="server" RemoteSort="true" AutoLoad="false"
                                        OnRefreshData="StoreOpsPendientes_RefreshData">
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
                                                    <ext:RecordField Name="FechaOperacion" />
                                                    <ext:RecordField Name="EstatusOperacion" />
                                                    <ext:RecordField Name="Importe" />
                                                    <ext:RecordField Name="Autorizacion" />
                                                    <ext:RecordField Name="CodigoMoneda" />
                                                    <ext:RecordField Name="Afiliacion" />
                                                    <ext:RecordField Name="Comercio" />
                                                    <ext:RecordField Name="Pais" />
                                                    <ext:RecordField Name="ClaveTransaccion" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                        <DirectEventConfig IsUpload="true" />
                                    </ext:Store>
                                </Store>
                                <ColumnModel ID="ColumnModel4" runat="server">
                                    <Columns>
                                        <ext:Column Hidden="true" DataIndex="ID_Operacion" />
                                        <ext:DateColumn Header="Fecha Operación" Sortable="true" DataIndex="FechaOperacion"
                                            Format="yyyy-MM-dd HH:mm:ss" />
                                        <ext:Column Header="Estatus Operación" Sortable="true" DataIndex="EstatusOperacion" />
                                        <ext:Column Header="Importe" Sortable="true" DataIndex="Importe" Align="Right">
                                            <Renderer Format="UsMoney" />
                                        </ext:Column>
                                        <ext:Column Header="Autorización" DataIndex="Autorizacion" />
                                        <ext:Column Header="Moneda" Sortable="true" DataIndex="CodigoMoneda" />
                                        <ext:Column Header="Afiliación" Sortable="true" DataIndex="Afiliacion" Width="105" />
                                        <ext:Column Header="Comercio" Sortable="true" DataIndex="Comercio" Width="140" />
                                        <ext:Column Header="País" Sortable="true" DataIndex="Pais" Width="40" />
                                        <ext:Column Header="Clave Transacción" DataIndex="ClaveTransaccion" />
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel SingleSelect="true" />
                                </SelectionModel>
                                <Plugins>
                                    <ext:GridFilters runat="server" Local="true">
                                        <Filters>
                                            <ext:StringFilter DataIndex="FechaOperacion" />
                                        </Filters>
                                    </ext:GridFilters>
                                </Plugins>
                                <TopBar>
                                    <ext:Toolbar runat="server">
                                        <Items>
                                            <ext:ToolbarFill runat="server" />
                                            <ext:ToolbarSeparator runat="server" />
                                            <ext:Button ID="btnExcelOpsEnT" runat="server" Text="Exportar a Excel" Icon="PageExcel"
                                                ToolTip="Obtener Datos en un Archivo Excel">
                                                <DirectEvents>
                                                    <Click OnEvent="DownloadOpsPendientes" IsUpload="true"
                                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                            e.stopEvent(); 
                                                            AdminCtasParab.StopMask();" />
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingTBOpsPendientes" runat="server" StoreID="StoreOpsPendientes" DisplayInfo="true"
                                        HideRefresh="true" DisplayMsg="Mostrando Operaciones {0} - {1} de {2}" PageSize="5">
                                        <Items>
                                            <ext:Button ID="btnDownOpsPendientes" runat="server" Hidden="true">
                                                <DirectEvents>
                                                    <Click OnEvent="DownloadOpsPendientes" IsUpload="true"
                                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                            AdminCtasParab.StopMask();" />
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:PagingToolbar>
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </Items>
    </ext:Window>
    <ext:Window ID="WdwConfirmaBloqueo" runat="server" Title="Bloquear Tarjeta" Width="380" AutoHeight="true" Icon="Stop"
        Hidden="true" Modal="true" Resizable="false">
        <Items>
            <ext:FormPanel ID="FormPanelConfBloq" runat="server" LabelAlign="Top" Padding="5" Border="false" Layout="FormLayout">
                <Items>
                    <ext:Hidden ID="hdnSelectedCard" runat="server" />
                    <ext:Hidden ID="hdnSelectedIdMA" runat="server" />
                    <ext:ComboBox ID="cBoxMotivo" runat="server" LabelAlign="Left" FieldLabel="Selecciona el Motivo de Bloqueo" LabelWidth="220"
                        AllowBlank="false" AutoWidth="true" ListWidth="450">
                        <Items>
                            <ext:ListItem Text="Número de reintentos de acceso excedidos" Value="001" />
                            <ext:ListItem Text="Morosidad" Value="002" />
                            <ext:ListItem Text="Solicitud de la autoridad / Requerimiento jurídico" Value="003" />
                            <ext:ListItem Text="Requerimiento operativo" Value="004" />
                            <ext:ListItem Text="Decisión manual de analistas de prevención de fraudes" Value="005" />
                            <ext:ListItem Text="Posible fraude con tarjeta robada" Value="006" />
                            <ext:ListItem Text="Posible fraude con tarjeta extraviada" Value="007" />
                            <ext:ListItem Text="Posible fraude por clonación" Value="008" />
                            <ext:ListItem Text="Posible fraude por impresión múltiple de tarjeta" Value="009" />
                            <ext:ListItem Text="Posible fraude en operación de tarjeta no presente (comercio electrónico)" Value="010" />
                            <ext:ListItem Text="Posible fraude con tarjeta nunca recibida " Value="011" />
                            <ext:ListItem Text="Posible fraude por usurpación de cuenta" Value="012" />
                            <ext:ListItem Text="Posible fraude por comercio en confabulación (bust-out)" Value="013" />
                            <ext:ListItem Text="Posible fraude por punto compromiso" Value="014" />
                            <ext:ListItem Text="Posible actividad de lavado de dinero (PLD)" Value="015" />
                        </Items>
                    </ext:ComboBox>
                </Items>
                <Buttons>
                    <ext:Button ID="btnBloquea" runat="server" Text="Aceptar" Icon="Tick">
                        <DirectEvents>
                            <Click OnEvent="btnBloquea_Click" Before="var valid= #{FormPanelConfBloq}.getForm().isValid(); if (!valid) {} return valid;">
                                <EventMask ShowMask="true" Msg="Bloqueando Tarjeta..." MinDelay="500" />
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                    <ext:Button runat="server" Text="Cancelar" Icon="Cancel">
                        <Listeners>
                            <Click Handler="#{WdwConfirmaBloqueo}.hide();" />
                        </Listeners>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:BorderLayout ID="MainBorderLayout" runat="server">
        <West Split="true">
            <ext:Panel ID="MainPanel" runat="server" Width="350" Border="false">
                <Content>
                    <ext:BorderLayout ID="LeftBorderLayout" runat="server">
                        <North Split="true">
                            <ext:FormPanel ID="FormPanelBusqueda" runat="server" Title="Consulta de Cuentas" Height="240" Frame="true" LabelWidth="120"
                                Collapsible="true" Border="false">
                                <Items>
                                    <ext:FieldSet ID="FieldSetBusqueda" runat="server" Border="false">
                                        <Items>
                                            <ext:TextField ID="txtNombre" runat="server" FieldLabel="Nombre" MaxLength="30" Width="310" />
                                            <ext:TextField ID="txtApPaterno" runat="server" FieldLabel="Primer Apellido" MaxLength="30" Width="310" />
                                            <ext:TextField ID="txtApMaterno" runat="server" FieldLabel="Segundo Apellido" MaxLength="30" Width="310" />
                                            <ext:TextField ID="txtNumCuenta" runat="server" FieldLabel="Número de Cuenta" MaxLength="20" 
                                                Width="310" MaskRe="[0-9]" />
                                            <ext:TextField ID="txtNumTarjeta" runat="server" FieldLabel="Número de Tarjeta" MaxLength="16"
                                                MinLength="16" Width="310" MaskRe="[0-9]" />
                                            <ext:Checkbox ID="chkBoxSoloAdicionales" runat="server" FieldLabel="Sólo Adicionales" />
                                        </Items>
                                        <Buttons>
                                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                                <DirectEvents>
                                                    <Click OnEvent="btnLimpiar_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                                        Before="if (!#{txtNumTarjeta}.isValid()) { return false; }
                                                        if (!#{txtNumCuenta}.getValue() && !#{txtNumTarjeta}.getValue() &&
                                                        !#{txtNombre}.getValue() && !#{txtApPaterno}.getValue() && !#{txtApMaterno}.getValue())
                                                        { Ext.Msg.alert('Consulta de Cuentas', 'Ingresa al menos un criterio de búsqueda'); return false; }
                                                        else { #{GridResultados}.getStore().removeAll(); #{EastPanel}.setTitle('_');
                                                        #{EastPanel}.setDisabled(true); }">
                                                        <EventMask ShowMask="true" Msg="Buscando Cuentas..." MinDelay="500" />
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Buttons>
                                    </ext:FieldSet>
                                </Items>
                            </ext:FormPanel>
                        </North>
                        <Center Split="true">
                            <ext:FormPanel ID="FormPanelResultados" runat="server" Title="Resultados Cuentas" Layout="FitLayout" Border="false">
                                <Items>
                                    <ext:GridPanel ID="GridResultados" runat="server" Height="450" AutoDoLayout="true" Border="false">
                                        <Store>
                                            <ext:Store ID="StoreCuentas" runat="server">
                                                <Reader>
                                                    <ext:JsonReader IDProperty="IdTarjeta">
                                                        <Fields>
                                                            <ext:RecordField Name="IdTarjeta" />
                                                            <ext:RecordField Name="Tarjeta" /> 
                                                            <ext:RecordField Name="NumTarjeta" />    
                                                            <ext:RecordField Name="IdCadenaComercial" />  
                                                            <ext:RecordField Name="IdColectivaCuentahabiente" />
                                                            <ext:RecordField Name="NombreTarjetahabiente" />
                                                            <ext:RecordField Name="CLDC" />
                                                            <ext:RecordField Name="CCLC" />
                                                            <ext:RecordField Name="CDC" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Store>
                                        <ColumnModel ID="ColumnModel1" runat="server">
                                            <Columns>
                                                <ext:Column DataIndex="IdTarjeta" Hidden="true" />
                                                <ext:Column DataIndex="Tarjeta" Header="Tarjeta" Width="90" /> 
                                                <ext:Column DataIndex="NumTarjeta" Hidden="true" />
                                                <ext:Column DataIndex="IdCadenaComercial" Hidden="true" />
                                                <ext:Column DataIndex="IdColectivaCuentahabiente" Hidden="true" />
                                                <ext:Column DataIndex="NombreTarjetahabiente" Header="Nombre" Width="160" />
                                                <ext:Column DataIndex="CLDC" Header="Cuenta" Width="90" />
                                                <ext:Column DataIndex="CCLC" Hidden="true" />
                                                <ext:Column DataIndex="CDC" Hidden="true" />
                                            </Columns>
                                        </ColumnModel>
                                        <SelectionModel>
                                            <ext:RowSelectionModel SingleSelect="true" />
                                        </SelectionModel>
                                        <DirectEvents>
                                            <RowClick OnEvent="selectRowResultados_Event">
                                                <EventMask ShowMask="true" Msg="Obteniendo Información de la Cuenta..." MinDelay="500" />
                                                <ExtraParams>
                                                    <ext:Parameter Name="Values" Value="Ext.encode(#{GridResultados}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                                </ExtraParams>
                                            </RowClick>
                                        </DirectEvents>
                                        <BottomBar>
                                            <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreCuentas" DisplayInfo="true" HideRefresh="true"
                                                DisplayMsg="Mostrando Cuentas {0} - {1} de {2}" />
                                        </BottomBar>
                                    </ext:GridPanel>
                                </Items>
                            </ext:FormPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>       
        </West>
        <Center Split="true">
            <ext:Panel ID="EastPanel" runat="server" Title="_" Disabled="true" Border="false" MonitorResize="true" MonitorWindowResize="true">
                <Items>
                    <ext:BorderLayout ID="BorderLayout1" runat="server">
                        <Center>
                            <ext:TabPanel ID="TabPanel1" runat="server">
                                <Items>
                                    <ext:FormPanel ID="FormPanelTitular" runat="server" Title="Titular" LabelAlign="Left" LabelWidth="150" AutoScroll="true"
                                        Border="false" Layout="FormLayout">
                                        <Content>
                                            <ext:Store ID="StoreColonias" runat="server">
                                                <Reader>
                                                    <ext:JsonReader IDProperty="ID_Colonia">
                                                        <Fields>
                                                            <ext:RecordField Name="ID_Colonia" />
                                                            <ext:RecordField Name="Colonia" />
                                                            <ext:RecordField Name="Municipio" />
                                                            <ext:RecordField Name="Estado" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Content>
                                        <Items>
                                            <ext:FieldSet ID="FieldSetDatosTitular" runat="server" Border="false" Layout="FormLayout">
                                                <Items>
                                                    <ext:TextField ID="txtTarjetaTitular" runat="server" FieldLabel="Número de Tarjeta" MaxLength="16" Width="535" 
                                                        ReadOnly="true" Selectable="false" />
                                                    <ext:Panel ID="PanelDatosPersonales" runat="server" Title="Datos Personales" AutoHeight="true" LabelAlign="Left"
                                                        FormGroup="true" Layout="FormLayout">
                                                        <Items>
                                                            <ext:TextField ID="txtNombreClienteTitular" runat="server" FieldLabel="Nombre" Width="535"
																MaxLength="100" AllowBlank="false" />
                                                            <ext:TextField ID="txtApPaternoTitular" runat="server" FieldLabel="Primer Apellido"
																MaxLength="100" Width="535" AllowBlank="false" />
                                                            <ext:TextField ID="txtApMaternoTitular" runat="server" FieldLabel="Segundo Apellido"
																MaxLength="100" Width="535" />
                                                            <ext:TextField ID="txtNombreEmbozo" runat="server" FieldLabel="Nombre Embozo"
																MaxLength="50" Width="535" />
                                                            <ext:Panel runat="server" Layout="FitLayout" Width="535" Height="5" Border="false" />
                                                            <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false">
                                                                <Defaults>
                                                                    <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                                </Defaults>
                                                                <LayoutConfig>
                                                                    <ext:HBoxLayoutConfig Align="Top" />
                                                                </LayoutConfig>
                                                                <Items>
                                                                    <ext:DateField ID="dfFechaNac" runat="server" FieldLabel="Fecha de Nacimiento"
                                                                        Width="285" Format="dd/MM/yyyy" Vtype="daterange" />
                                                                    <ext:Hidden runat="server" Width="15" />
                                                                    <ext:TextField ID="txtRFCTitular" runat="server" FieldLabel="RFC" LabelWidth="40" MaxLength="13" MinLength="13" Width="175" />
                                                                    <ext:Hidden runat="server" Width="15" />
                                                                    <ext:TextField ID="txtCURP" runat="server" FieldLabel="CURP" LabelWidth="50" MinLength="18" MaxLength="18" Width="200" />
                                                                </Items>
                                                            </ext:Panel>
                                                        </Items>
                                                    </ext:Panel>
                                                    <ext:Panel ID="PanelDomicilio" runat="server" Title="Domicilio" AutoHeight="true" LabelAlign="Left"
                                                        FormGroup="true" Layout="FormLayout" Collapsed="true">
                                                        <Items>
                                                            <ext:TextField ID="txtID_Direccion" runat="server" Hidden="true" Enabled="false" />
                                                            <ext:TextField ID="txtCalle" runat="server" FieldLabel="Calle" MaxLength="100" Width="535" />
                                                            <ext:TextField ID="txtNumExterior" runat="server" FieldLabel="Número Exterior"
                                                                MaxLength="50" Width="535" />
                                                            <ext:TextField ID="txtNumInterior" runat="server" FieldLabel="Número Interior" 
                                                                MaxLength="50" Width="535" />
                                                            <ext:TextField ID="txtEntreCalles" runat="server" FieldLabel="Entre las Calles" 
                                                                MaxLength="100" Width="535" />
                                                            <ext:TextField ID="txtReferencias" runat="server" FieldLabel="Referencias del Domicilio" 
                                                                MaxLength="200" Width="535" />
                                                            <ext:TextField ID="txtCPCliente" runat="server" FieldLabel="Código Postal" MaskRe="[0-9]"
                                                                MinLength="5" MaxLength="5" Width="535" EnableKeyEvents="true">
                                                                <Listeners>
                                                                    <SpecialKey Handler="if (e.getKey() == e.TAB || e.getKey() == e.ENTER) {
                                                                #{cBoxColonia}.reset(); CuentasBancarias.LlenaComboColonias();}" />
                                                                </Listeners>
                                                            </ext:TextField>
                                                            <ext:TextField ID="txtIDColonia" runat="server" Hidden="true" Enabled="false" />
                                                            <ext:ComboBox ID="cBoxColonia" runat="server" FieldLabel="Colonia" StoreID="StoreColonias" 
                                                                Width="535" DisplayField="Colonia" ValueField="ID_Colonia" />
                                                            <ext:TextField ID="txtClaveMunicipio" runat="server" Hidden="true" Enabled="false" />
                                                            <ext:TextField ID="txtMunicipioTitular" runat="server" FieldLabel="Delegación o Municipio"
                                                                ReadOnly="true" Width="535" />
                                                            <ext:TextField ID="txtClaveEstado" runat="server" Hidden="true" Enabled="false" />
                                                            <ext:TextField ID="txtEstadoTitular" runat="server" FieldLabel="Estado" ReadOnly="true"
                                                                Width="535" />
                                                        </Items>
                                                    </ext:Panel>
                                                    <ext:Panel ID="PanelDatosContacto" runat="server" Title="Datos de Contacto" AutoHeight="true" LabelAlign="Left"
                                                        FormGroup="true" Layout="FormLayout" Collapsed="true">
                                                        <Items>
                                                            <ext:NumberField ID="nfTelParticular" runat="server" FieldLabel="Teléfono Particular" MaxLength="20" Width="535"
                                                                AllowDecimals="False" AllowNegative="False" />
                                                            <ext:NumberField ID="nfTelCelular" runat="server" FieldLabel="Teléfono Celular" MaxLength="20" Width="535"
                                                                AllowDecimals="False" AllowNegative="False" />
                                                            <ext:NumberField ID="nfTelTrabajo" runat="server" FieldLabel="Teléfono Trabajo" MaxLength="20" Width="535"
                                                                AllowDecimals="False" AllowNegative="False" />
                                                            <ext:TextField ID="txtCorreo" runat="server" FieldLabel="Correo Electrónico" MaxLength="120" Width="535" />
                                                        </Items>
                                                    </ext:Panel>
                                                    <ext:Panel ID="PanelDomFiscal" runat="server" Title="Datos Fiscales" AutoHeight="true" LabelAlign="Left"
                                                        FormGroup="true" Layout="FormLayout">
                                                        <Items>
                                                            <ext:TextField ID="txtIDDirFiscal" runat="server" Hidden="true" Enabled="false" />
                                                            <ext:TextField ID="txtCalleFiscal" runat="server" FieldLabel="Calle" MaxLength="100" Width="535" />
                                                            <ext:TextField ID="txtNumExtFiscal" runat="server" FieldLabel="Número Exterior" MaxLength="50" Width="535" />
                                                            <ext:TextField ID="txtNumIntFiscal" runat="server" FieldLabel="Número Interior" MaxLength="50" Width="535" />
                                                            <ext:TextField ID="txtCPFiscal" runat="server" FieldLabel="Código Postal" MinLength="5" MaxLength="5"
                                                                Width="535" EnableKeyEvents="true" MaskRe="[0-9]">
                                                                <Listeners>
                                                                    <SpecialKey Handler="if (e.getKey() == e.TAB || e.getKey() == e.ENTER) {
                                                                        #{cBoxColFiscal}.reset(); CuentasBancarias.LlenaColoniasFiscales();}" />
                                                                </Listeners>
                                                            </ext:TextField>
                                                            <ext:TextField ID="txtIDColFiscal" runat="server" Hidden="true" Enabled="false" />
                                                            <ext:ComboBox ID="cBoxColFiscal" runat="server" FieldLabel="Colonia" Width="535" DisplayField="Colonia"
                                                                ValueField="ID_Colonia" Editable="false">
                                                                <Store>
                                                                    <ext:Store ID="StoreColFiscal" runat="server">
                                                                        <Reader>
                                                                            <ext:JsonReader IDProperty="ID_Colonia">
                                                                                <Fields>
                                                                                    <ext:RecordField Name="ID_Colonia" />
                                                                                    <ext:RecordField Name="Colonia" />
                                                                                    <ext:RecordField Name="Municipio" />
                                                                                    <ext:RecordField Name="Estado" />
                                                                                </Fields>
                                                                            </ext:JsonReader>
                                                                        </Reader>
                                                                    </ext:Store>
                                                                </Store>
                                                            </ext:ComboBox>
                                                            <ext:TextField ID="txtCveMunFiscal" runat="server" Hidden="true" Enabled="false" />
                                                            <ext:TextField ID="txtDelMunFiscal" runat="server" FieldLabel="Delegación o Municipio"
                                                                ReadOnly="true" Width="535" />
                                                            <ext:TextField ID="txtCveEdoFiscal" runat="server" Hidden="true" Enabled="false" />
                                                            <ext:TextField ID="txtEstadoFiscal" runat="server" FieldLabel="Estado" ReadOnly="true"
                                                                Width="535" />
                                                            <ext:Hidden ID="hdnCveRegimenFiscal" runat="server" />
                                                            <ext:ComboBox ID="cBoxRegimenFiscal" runat="server" FieldLabel="Régimen Fiscal" Width="535"
                                                                DisplayField="Descripcion" ValueField="ID_ParametroPreestablecido" Editable="false"
                                                                AllowBlank="false">
                                                                <Store>
                                                                    <ext:Store ID="StoreRegFiscal" runat="server">
                                                                        <Reader>
                                                                            <ext:JsonReader IDProperty="ID_ParametroPreestablecido">
                                                                                <Fields>
                                                                                    <ext:RecordField Name="ID_ParametroPreestablecido" />
                                                                                    <ext:RecordField Name="Clave" />
                                                                                    <ext:RecordField Name="Descripcion" />
                                                                                </Fields>
                                                                            </ext:JsonReader>
                                                                        </Reader>
                                                                    </ext:Store>
                                                                </Store>
                                                                <Listeners>
                                                                    <Select Handler="var id = this.getValue(); var record = this.getStore().getById(id);
                                                                        #{hdnCveRegimenFiscal}.setValue(record.get('Clave'));" />
                                                                </Listeners>
                                                            </ext:ComboBox>
                                                            <ext:Hidden ID="hdnCveUsoCFDI" runat="server" />
                                                            <ext:ComboBox ID="cBoxUsoCFDI" runat="server" FieldLabel="Uso de CFDI" Width="535"
                                                                DisplayField="Descripcion" ValueField="ID_ParametroPreestablecido" Editable="false"
                                                                AllowBlank="false">
                                                                <Store>
                                                                    <ext:Store ID="StoreUsoCFDI" runat="server">
                                                                        <Reader>
                                                                            <ext:JsonReader IDProperty="ID_ParametroPreestablecido">
                                                                                <Fields>
                                                                                    <ext:RecordField Name="ID_ParametroPreestablecido" />
                                                                                    <ext:RecordField Name="Clave" />
                                                                                    <ext:RecordField Name="Descripcion" />
                                                                                </Fields>
                                                                            </ext:JsonReader>
                                                                        </Reader>
                                                                    </ext:Store>
                                                                </Store>
                                                                <Listeners>
                                                                    <Select Handler="var id = this.getValue(); var record = this.getStore().getById(id);
                                                                        #{hdnCveUsoCFDI}.setValue(record.get('Clave'));" />
                                                                </Listeners>
                                                            </ext:ComboBox>
                                                        </Items>
                                                    </ext:Panel>
                                                </Items>
                                                <Buttons>
                                                    <ext:Button ID="btnGuardaDatos" runat="server" Text="Guardar Cambios" Icon="Disk">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnGuardaDatos_Click" Before="if (!#{txtNombreClienteTitular}.isValid() || 
                                                                !#{txtApPaternoTitular}.isValid() || !#{txtApMaterno}.isValid() || !#{cBoxRegimenFiscal}.isValid()
                                                                || !#{cBoxUsoCFDI}.isValid()) { return false; }">
                                                                <EventMask ShowMask="true" Msg="Guardando..." MinDelay="500" />
                                                            </Click>
                                                        </DirectEvents>
                                                    </ext:Button>
                                                </Buttons>
                                            </ext:FieldSet>
                                        </Items>        
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelTarjetas" runat="server" Title="Tarjetas" Layout="FitLayout" Border="false" AutoScroll="true"
                                        MonitorResize="true" MonitorWindowResize="true">
                                        <Items>
                                            <ext:GridPanel ID="GridTarjetas" runat="server" Header="true" Border="false" Title="Tarjetas de la Cuenta" Layout="FitLayout"
                                                AutoExpandColumn="Tarjetahabiente" MonitorResize="true" MonitorWindowResize="true">
                                                <Store>
                                                    <ext:Store ID="StoreTarjetas" runat="server">
                                                        <Reader>
                                                            <ext:JsonReader IDProperty="ID_MA">
                                                                <Fields>
                                                                    <ext:RecordField Name="ID_MA" />
                                                                    <ext:RecordField Name="Tarjeta" />
                                                                    <ext:RecordField Name="ClaveMA" />
                                                                    <ext:RecordField Name="Tipo" />
                                                                    <ext:RecordField Name="Tarjetahabiente" />
                                                                    <ext:RecordField Name="TipoManufactura" />
                                                                    <ext:RecordField Name="Expiracion" />
                                                                    <ext:RecordField Name="LimiteCredito" />
                                                                    <ext:RecordField Name="EstatusLetra" />
                                                                    <ext:RecordField Name="Estatus" />
                                                                </Fields>
                                                            </ext:JsonReader>
                                                        </Reader>
                                                    </ext:Store>
                                                </Store>
                                                <ColumnModel ID="ColumnModel3" runat="server">
                                                    <Columns>
                                                        <ext:Column DataIndex="ID_MA" Hidden="true" />
                                                        <ext:Column DataIndex="Tarjeta" Header="Tarjeta" Width="110" />
                                                        <ext:Column DataIndex="Tipo" Header="Tipo" />
                                                        <ext:Column DataIndex="Tarjetahabiente" Header="Tarjetahabiente" />
                                                        <ext:Column DataIndex="TipoManufactura" Header="Manufactura" Width="80" />
                                                        <ext:DateColumn DataIndex="Expiracion" Header="Expiración" Align="Center"
                                                            Format="dd-MMM-yyyy" Width="80  " />
                                                        <ext:Column DataIndex="LimiteCredito" Header="Límite de Crédito">
                                                            <Renderer Format="UsMoney" />
                                                        </ext:Column>
                                                        <ext:Column DataIndex="EstatusLetra" Header="Estatus" />
                                                        <ext:CommandColumn Header="Acciones" Width="60">
                                                            <PrepareToolbar Fn="prepareToolbar" />
                                                            <Commands>
                                                                <ext:GridCommand Icon="RecordRed" CommandName="Activar">
                                                                    <ToolTip Text="Activar Tarjeta" />
                                                                </ext:GridCommand>
                                                                <ext:GridCommand Icon="RecordGreen" CommandName="Bloquear">
                                                                    <ToolTip Text="Bloquear Tarjeta" />
                                                                </ext:GridCommand>
                                                            </Commands>
                                                        </ext:CommandColumn>
                                                    </Columns>
                                                </ColumnModel>
                                                <SelectionModel>
                                                    <ext:RowSelectionModel SingleSelect="true" />
                                                </SelectionModel>
                                                <DirectEvents>
                                                    <Command OnEvent="EjecutarComandoTarjetas" Timeout="360000">
                                                        <Confirmation BeforeConfirm="if (command == 'Bloquear') return false;" 
                                                            ConfirmRequest="true" Title="Confirmación" Message="¿Estás seguro de activar la tarjeta?" />
                                                        <EventMask ShowMask="true" Msg="Procesando..." MinDelay="500" />
                                                        <ExtraParams>
                                                            <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                                            <ext:Parameter Name="ID_MA" Value="Ext.encode(record.data.ID_MA)" Mode="Raw" />
                                                            <ext:Parameter Name="ClaveMA" Value="Ext.encode(record.data.ClaveMA)" Mode="Raw" />
                                                            <ext:Parameter Name="Tarjeta" Value="Ext.encode(record.data.Tarjeta)" Mode="Raw" />
                                                        </ExtraParams>
                                                    </Command>
                                                </DirectEvents>
                                                <BottomBar>
                                                    <ext:PagingToolbar ID="PagingToolBar3" runat="server" StoreID="StoreTarjetas" DisplayInfo="true"
                                                        DisplayMsg="Tarjetas {0} - {1} de {2}" />
                                                </BottomBar>
                                            </ext:GridPanel>
                                        </Items>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelLimiteCredito" runat="server" Title="Límite de Crédito" Border="false" Hidden="true">
                                        <Items>
                                            <ext:FieldSet Title="Modificar" runat="server" Padding="10" LabelWidth="120" Layout="FormLayout">
                                                <Items>
                                                    <ext:Hidden ID="hdnLDCActual" runat="server" />
                                                    <ext:Hidden ID="hdnLDCPendiente" runat="server" />
                                                    <ext:TextField ID="txtLDCActual" runat="server" FieldLabel="Valor Actual" Width="550"
                                                        ReadOnly="true" Cls="chng-blue-nf-css" />
                                                    <ext:TextField ID="txtLDCPendiente" runat="server" FieldLabel="Valor por Autorizar" Width="550"
                                                        ReadOnly="true" Cls="chng-red-nf-css" />
                                                    <ext:TextField ID="txtImporteLDC" runat="server" FieldLabel="Importe   <span style='color:red;'>*   </span>"
                                                        Width="550" MaskRe="[0-9\.-]" MaxLength="20" AllowBlank="false">
                                                        <Listeners>
                                                            <Change Handler="var imp = Ext.util.Format.number(this.getValue(), '$0,0.00');
                                                                this.setValue(imp);" />
                                                        </Listeners>
                                                    </ext:TextField>
                                                    <ext:TextField ID="txtReferencia" FieldLabel="Referencia" runat="server" MaxLength="15"
                                                        MaskRe="[0-9]" Width="550"/>
                                                    <ext:TextArea ID="txtObservaciones" runat="server" FieldLabel="Observaciones   <span style='color:red;'>*   </span>"
                                                        MaxLength="120" Width="550" Height="120" AllowBlank="false" />
                                                    <ext:Panel runat="server" Layout="FitLayout" Height="20" Border="false" />
                                                    <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false" LabelWidth="5">
                                                        <Defaults>
                                                            <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                        </Defaults>
                                                        <LayoutConfig>
                                                            <ext:HBoxLayoutConfig Align="Top" />
                                                        </LayoutConfig>
                                                        <Items>
                                                            <ext:Label runat="server" FieldLabel="<span style='color:red;'>*   </span>"
                                                                Text="Obligatorios" LabelSeparator=" " StyleSpec="font-style: italic;font-family:segoe ui;font-size: 11px;" />
                                                        </Items>
                                                    </ext:Panel>
                                                </Items>
                                                <Buttons>
                                                    <ext:Button ID="btnModificarLDC" runat="server" Text="Modificar" Icon="Tick">
                                                        <Listeners>
                                                            <Click Handler="if (!#{txtImporteLDC}.isValid() || 
                                                                !#{txtObservaciones}.isValid()) {
                                                                    return false;
                                                                } else {
                                                                    AdminCtasParab.ConfirmaModificacionLDC();
                                                                }" />
                                                        </Listeners>
                                                    </ext:Button>
                                                    <ext:Button ID="btnAutorizarCambios" runat="server" Text="Autorizar" Icon="Tick" Hidden="true">
                                                        <Listeners>
                                                            <Click Handler="if (#{txtObservaciones}.isValid()) {
                                                                Ext.Msg.confirm('Confirmación',
                                                                '¿Autorizas el ajuste al Límite de Crédito?',
                                                                    function (btn) {
                                                                        if (btn == 'yes') {
                                                                            Ext.net.Mask.show({ msg: 'Autorizando ajuste...' });
                                                                            AdminCtasParab.AutorizaModificarLDC();
                                                                        } else {
                                                                            Ext.net.Mask.show({ msg: 'Rechazando ajuste...' });
                                                                            AdminCtasParab.RechazaModificarLDC();
                                                                        }
                                                                    });
                                                                } else {
                                                                    return false;
                                                                }" />
                                                        </Listeners>
                                                    </ext:Button>
                                                </Buttons>
                                            </ext:FieldSet>
                                        </Items>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelParams" runat="server" Title="Parámetros" Layout="FitLayout" Border="false">
                                        <Items>
                                            <ext:Panel runat="server" Layout="FitLayout" AutoScroll="true" Border="false">
                                                <TopBar>
                                                    <ext:Toolbar runat="server">
                                                        <Items>
                                                            <ext:Hidden ID="hdnIdParametroMA" runat="server" />
                                                            <ext:Hidden ID="hdnIdPlantilla" runat="server" />
                                                            <ext:Hidden ID="hdnIdValorPMA" runat="server" />
                                                            <ext:ComboBox ID="cBoxTipoParametroMA" runat="server" EmptyText="Tipo de Parámetros..." Width="150"
                                                                DisplayField="Descripcion" ValueField="ID_TipoParametroMultiasignacion" AllowBlank="false"
                                                                ListWidth="200">
                                                                <Store>
                                                                    <ext:Store ID="StoreTipoParametroMA" runat="server">
                                                                        <Reader>
                                                                            <ext:JsonReader IDProperty="ID_TipoParametroMultiasignacion">
                                                                                <Fields>
                                                                                    <ext:RecordField Name="ID_TipoParametroMultiasignacion" />
                                                                                    <ext:RecordField Name="Clave" />
                                                                                    <ext:RecordField Name="Descripcion" />
                                                                                </Fields>
                                                                            </ext:JsonReader>
                                                                        </Reader>
                                                                    </ext:Store>
                                                                </Store>
                                                                <DirectEvents>
                                                                    <Select OnEvent="SeleccionaTipoPMA" Before="#{cBoxParametros}.setDisabled(false);
                                                                        #{btnAddParametros}.setDisabled(false); #{cBoxParametros}.getStore().removeAll();
                                                                        #{cBoxParametros}.reset(); #{GridPanelParametros}.getStore().removeAll();">
                                                                        <EventMask ShowMask="true" Msg="Obteniendo Parámetros..." MinDelay="200" />
                                                                    </Select>
                                                                </DirectEvents>
                                                            </ext:ComboBox>
                                                            <ext:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                            <ext:ComboBox ID="cBoxParametros" runat="server" EmptyText="Parámetros sin Asignar..." Width="180"
                                                                DisplayField="Nombre" ValueField="ID_ParametroMultiasignacion" Disabled="true" AllowBlank="false">
                                                                <Store>
                                                                    <ext:Store ID="StoreParametros" runat="server">
                                                                        <Reader>
                                                                            <ext:JsonReader IDProperty="ID_ParametroMultiasignacion">
                                                                                <Fields>
                                                                                    <ext:RecordField Name="ID_ParametroMultiasignacion" />
                                                                                    <ext:RecordField Name="Nombre" />
                                                                                    <ext:RecordField Name="Descripcion" />
                                                                                    <ext:RecordField Name="ID_Plantilla" />
                                                                                </Fields>
                                                                            </ext:JsonReader>
                                                                        </Reader>
                                                                    </ext:Store>
                                                                </Store>
                                                                <Listeners>
                                                                    <Select Handler="var id = this.getValue(); var record = this.getStore().getById(id);
                                                                        #{hdnIdPlantilla}.setValue(record.get('ID_Plantilla'));" />
                                                                </Listeners>
                                                            </ext:ComboBox>
                                                            <ext:Button ID="btnAddParametros" runat="server" Text="Asignar Parámetro" Icon="Add" Disabled="true">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnAddParametros_Click" Before="var valid= #{cBoxParametros}.isValid(); if (!valid) {} return valid;">
                                                                        <EventMask ShowMask="true" Msg="Asignando Parámetro..." MinDelay="500" />
                                                                    </Click>
                                                                </DirectEvents>
                                                            </ext:Button>
                                                        </Items>
                                                    </ext:Toolbar>
                                                </TopBar>
                                                <Items>
                                                    <ext:GridPanel ID="GridPanelParametros" runat="server" Header="true" Border="false" AutoScroll="true"
                                                        AutoHeight="true" Layout="FitLayout" AutoExpandColumn="Nombre">
                                                        <Store>
                                                            <ext:Store ID="StoreValoresParametros" runat="server">
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="ID_ValorParametroMultiasignacion">
                                                                        <Fields>
                                                                            <ext:RecordField Name="ID_ParametroMultiasignacion" />
                                                                            <ext:RecordField Name="ID_ValorParametroMultiasignacion" />
                                                                            <ext:RecordField Name="Nombre" />
                                                                            <ext:RecordField Name="Descripcion" />
                                                                            <ext:RecordField Name="Valor" />
                                                                            <ext:RecordField Name="ID_Plantilla" />
                                                                            <ext:RecordField Name="TipoDato" />
                                                                            <ext:RecordField Name="esAutorizable" />
                                                                            <ext:RecordField Name="TipoValidacion" />
                                                                            <ext:RecordField Name="ValorInicial" />
                                                                            <ext:RecordField Name="ValorFinal" />
                                                                            <ext:RecordField Name="ExpresionRegular" />
                                                                        </Fields>
                                                                    </ext:JsonReader>
                                                                </Reader>
                                                            </ext:Store>
                                                        </Store>
                                                        <ColumnModel runat="server">
                                                            <Columns>
                                                                <ext:Column ColumnID="ID_Parametro" runat="server" Hidden="true" DataIndex="ID_Parametro" />
                                                                <ext:Column ColumnID="Nombre" Header="Parámetro" Width="350" DataIndex="Nombre">
                                                                    <Renderer Fn="fullName" />
                                                                    <Editor>
                                                                        <ext:DisplayField runat="server" StyleSpec="font-weight:bold;font-family:segoe ui;font-size: 12px;" />
                                                                    </Editor>
                                                                </ext:Column>
                                                                <ext:Column ColumnID="Valor" Header="Valor" Sortable="true" DataIndex="Valor" Width="140" />
                                                                <ext:Column runat="server" Hidden="true" DataIndex="TipoDato" />
                                                                <ext:CommandColumn Header="Acciones" Width="80">
                                                                    <PrepareToolbar Fn="prepareTB_CtasParams" />
                                                                    <Commands>
                                                                        <ext:GridCommand Icon="Pencil" CommandName="Edit">
                                                                            <ToolTip Text="Editar Valor" />
                                                                        </ext:GridCommand>
                                                                        <ext:GridCommand Icon="Delete" CommandName="Delete">
                                                                            <ToolTip Text="Quitar Parámetro a la Cuenta" />
                                                                        </ext:GridCommand>
                                                                    </Commands>
                                                                </ext:CommandColumn>
                                                            </Columns>
                                                        </ColumnModel>
                                                        <View>
                                                            <ext:GridView runat="server" EnableRowBody="true">
                                                                <GetRowClass Handler="rowParams.body = '<p>' + record.data.Descripcion + '</p>'; return 'x-grid3-row-expanded';" />
                                                            </ext:GridView>
                                                        </View>
                                                        <SelectionModel>
                                                            <ext:RowSelectionModel runat="server" SingleSelect="true" />
                                                        </SelectionModel>
                                                        <DirectEvents>
                                                            <Command OnEvent="EjecutarComandoParametros">
                                                                <Confirmation BeforeConfirm="if (command == 'Edit') return false;" 
                                                                    ConfirmRequest="true" Title="Confirmación" Message="¿Estás seguro de quitar el parámetro a la colectiva?" />
                                                                <ExtraParams>
                                                                    <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                                                    <ext:Parameter Name="Values" Value="Ext.encode(record.data)" Mode="Raw" />
                                                                </ExtraParams>
                                                            </Command>
                                                        </DirectEvents>
                                                        <LoadMask ShowMask="false" />
                                                    </ext:GridPanel>
                                                </Items>
                                            </ext:Panel>
                                        </Items>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelMovsManuales" runat="server" Title="Movimientos Manuales" LabelAlign="Left" LabelWidth="160"
                                        Border="false" Hidden="true">
                                        <Items>
                                            <ext:FieldSet runat="server" Title="Selecciona el Movimiento" Layout="FormLayout" Padding="10">
                                                <Items>
                                                    <ext:ComboBox ID="cBoxPertenencias" runat="server" FieldLabel="Movimiento <span style='color:red;'>*   </span>"
                                                        Width="520" AllowBlank="false" DisplayField="Descripcion" ValueField="ID_Pertenencia" Editable="false">
                                                        <Store>
                                                            <ext:Store ID="StorePertenenciasManuales" runat="server">
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="ID_Pertenencia">
                                                                        <Fields>
                                                                            <ext:RecordField Name="ID_Pertenencia" />
                                                                            <ext:RecordField Name="Descripcion" />
                                                                        </Fields>
                                                                    </ext:JsonReader>
                                                                </Reader>
                                                            </ext:Store>
                                                        </Store>
                                                    </ext:ComboBox>
                                                    <ext:DateField ID="dfFechaAjusteManual" runat="server" Vtype="daterange" AllowBlank="false" Format="dd/MM/yyyy" Width="520" 
                                                        MsgTarget="Side" FieldLabel="Fecha Real del Movimiento <span style='color:red;'>*   </span>" MaxLength="12"
                                                        MaxDate="<%# DateTime.Now %>" SelectedValue="<%# DateTime.Now %>" AutoDataBind="true" MaskRe="[0-9\/]" />
                                                    <ext:TextField ID="txtImporte" runat="server" FieldLabel="Importe <span style='color:red;'>*   </span>"
                                                        MaxLength="20" Width="520" MaskRe="[0-9\.]" AllowBlank="false">
                                                        <Listeners>
                                                            <Change Handler="var imp = Ext.util.Format.number(this.getValue(), '$0,0.00');
                                                                this.setValue(imp);" />
                                                        </Listeners>
                                                    </ext:TextField>
                                                    <ext:TextField ID="txtRefAjustesManuales" FieldLabel="Referencia" runat="server" MaxLength="15"
                                                        Width="520" MaskRe="[0-9]" />
                                                    <ext:TextArea ID="txtObsAjustesManuales" runat="server" FieldLabel="Observaciones <span style='color:red;'>*   </span>"
                                                        AllowBlank="false" MaxLength="500" Width="520" />
                                                </Items>
                                                <Buttons>
                                                    <ext:Button ID="btnAplicar" runat="server" Text="Aplicar" Icon="Tick">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnAplicar_Click" Before="if (!#{cBoxPertenencias}.isValid() || !#{txtImporte}.isValid()
                                                                || !#{txtObsAjustesManuales}.isValid()) { return false; }
                                                                if ( #{txtImporte}.getValue() == 0 ) { Ext.Msg.alert('Movimientos Manuales', 'El Importe no puede ser cero.'); return false; }" />
                                                        </DirectEvents>
                                                    </ext:Button>
                                                </Buttons>
                                            </ext:FieldSet>
                                        </Items>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelSaldosYMovs" runat="server" Title="Saldos y Movimientos" AutoScroll="true" Layout="FormLayout" Border="false" Padding="10">
                                        <Items>
                                            <ext:FieldSet runat="server" Title="Ingresa el Periodo de Consulta" Layout="FormLayout" Padding="10">
                                                <Items>
                                                    <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false" LabelWidth="50">
                                                        <Defaults>
                                                            <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                        </Defaults>
                                                        <LayoutConfig>
                                                            <ext:HBoxLayoutConfig Align="Top" />
                                                        </LayoutConfig>
                                                        <Items>
                                                            <ext:DateField ID="dfInicioSYM" runat="server" Vtype="daterange" FieldLabel="Desde"
                                                                AllowBlank="false" MsgTarget="Side" Format="dd/MM/yyyy" MaxLength="12"
                                                                Width="200" EnableKeyEvents="true" MaskRe="[0-9\/]">
                                                                <CustomConfig>
                                                                    <ext:ConfigItem Name="endDateField" Value="#{dfFinSYM}" Mode="Value" />
                                                                </CustomConfig>
                                                                <Listeners>
                                                                    <KeyUp Fn="onKeyUp" />
                                                                </Listeners>
                                                            </ext:DateField>
                                                            <ext:Hidden runat="server" Width="30" />
                                                            <ext:DateField ID="dfFinSYM" runat="server" Vtype="daterange" FieldLabel="Hasta"
                                                                AllowBlank="false" MaxLength="12" Width="200" MsgTarget="Side" Format="dd/MM/yyyy"
                                                                EnableKeyEvents="true" AutoDataBind="true" MaskRe="[0-9\/]">
                                                                <CustomConfig>
                                                                    <ext:ConfigItem Name="startDateField" Value="#{dfInicioSYM}" Mode="Value" />
                                                                </CustomConfig>
                                                                <Listeners>
                                                                    <KeyUp Fn="onKeyUp" />
                                                                </Listeners>
                                                            </ext:DateField>
                                                            <ext:Hidden runat="server" Width="30" />
                                                            <ext:Button ID="btnBuscarSaldosYMovs" runat="server" Text="Buscar" Icon="Magnifier" Width="80">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnBuscarSaldosYMovs_Click" Before="if (!#{dfInicioSYM}.isValid()
                                                                        && !#{dfFinSYM}.isValid()) { return false; } else { #{GridDetMovs}.getStore().removeAll();
                                                                        #{GridOpsPendientes}.getStore().removeAll(); }">
                                                                        <EventMask ShowMask="true" Msg="Buscando Movimientos..." MinDelay="500" />
                                                                    </Click>
                                                                </DirectEvents>
                                                            </ext:Button>
                                                        </Items>
                                                    </ext:Panel>
                                                </Items>
                                            </ext:FieldSet>
                                            <ext:FieldSet ID="FieldSetResMovs" runat="server" Title="Resumen de Movimientos" Layout="AnchorLayout" Padding="10" 
                                                Collapsible="true" Collapsed="true">
                                                <Items>
                                                    <ext:FormPanel ID="FormPanelResumenMovs" runat="server" Layout="FormLayout" Header="false" Border="false">
                                                        <Items>
                                                            <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false" LabelWidth="250">
                                                                <Defaults>
                                                                    <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                                </Defaults>
                                                                <LayoutConfig>
                                                                    <ext:HBoxLayoutConfig Align="Top" />
                                                                </LayoutConfig>
                                                                <Items>
                                                                    <ext:Label runat="server" FieldLabel="Saldo Inicial" />
                                                                    <ext:Hidden runat="server" Flex="1" Width="25" />
                                                                    <ext:Label ID="lblSaldoInicial" runat="server" Text="-" LabelSeparator=" " 
                                                                        Width="150" StyleSpec="text-align:right" />
                                                                </Items>
                                                            </ext:Panel>
                                                            <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false" LabelWidth="250">
                                                                <Defaults>
                                                                    <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                                </Defaults>
                                                                <LayoutConfig>
                                                                    <ext:HBoxLayoutConfig Align="Top" />
                                                                </LayoutConfig>
                                                                <Items>
                                                                    <ext:Label runat="server" FieldLabel="Compras/Disposiciones (+)" />
                                                                    <ext:Hidden runat="server" Flex="1" Width="25" />
                                                                    <ext:Label ID="lblCargos" runat="server" Text="-" LabelSeparator=" "
                                                                        Width="150" StyleSpec="text-align:right" />
                                                                </Items>
                                                            </ext:Panel>
                                                            <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false" LabelWidth="250">
                                                                <Defaults>
                                                                    <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                                </Defaults>
                                                                <LayoutConfig>
                                                                    <ext:HBoxLayoutConfig Align="Top" />
                                                                </LayoutConfig>
                                                                <Items>
                                                                    <ext:Label runat="server" FieldLabel="Pagos/Reembolsos/Devoluciones (-)" />
                                                                    <ext:Hidden runat="server" Flex="1" Width="25" />
                                                                    <ext:Label ID="lblAbonos" runat="server" Text="-" LabelSeparator=" "
                                                                        Width="150" StyleSpec="text-align:right" />
                                                                </Items>
                                                            </ext:Panel>
                                                            <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false" LabelWidth="250">
                                                                <Defaults>
                                                                    <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                                </Defaults>
                                                                <LayoutConfig>
                                                                    <ext:HBoxLayoutConfig Align="Top" />
                                                                </LayoutConfig>
                                                                <Items>
                                                                    <ext:Label runat="server" FieldLabel="Saldo Final" />
                                                                    <ext:Hidden runat="server" Flex="1" Width="25" />
                                                                    <ext:Label ID="lblSaldoFinal" runat="server" Text="-" LabelSeparator=" " 
                                                                        Width="150" StyleSpec="text-align:right" />
                                                                </Items>
                                                            </ext:Panel>
                                                            <ext:Panel runat="server" Layout="FitLayout" Width="500" Height="20" Border="false" />
                                                            <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false" LabelWidth="250">
                                                                <Defaults>
                                                                    <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                                </Defaults>
                                                                <LayoutConfig>
                                                                    <ext:HBoxLayoutConfig Align="Top" />
                                                                </LayoutConfig>
                                                                <Items>
                                                                    <ext:Hidden runat="server" Flex="1" Width="460" />
                                                                    <ext:Button ID="btnDetMovsHide" runat="server" Hidden="true">
                                                                        <Listeners>
                                                                            <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Detalle de Movimientos...' });
                                                                                #{GridDetMovs}.getStore().reload({params:{start:0, sort:('','')}});" />
                                                                        </Listeners>
                                                                    </ext:Button>
                                                                    <ext:Button ID="btnDetMovsPendHide" runat="server" Hidden="true">
                                                                        <Listeners>
                                                                            <Click Handler="#{GridOpsPendientes}.getStore().reload({params:{start:0, sort:('','')}});" />
                                                                        </Listeners>
                                                                    </ext:Button>
                                                                    <ext:Button ID="btnMovimientos" runat="server" Text="Detalle de Movimientos..." Disabled="true">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="btnMovimientos_Click" Timeout="360000"
                                                                                Before="resetToolbar(#{PagingTBSaldosYMovs});
                                                                                    #{GridDetMovs}.getStore().sortInfo = null;
                                                                                    resetToolbar(#{PagingTBOpsPendientes});
                                                                                    #{GridOpsPendientes}.getStore().sortInfo = null;">
                                                                            </Click>
                                                                        </DirectEvents>
                                                                    </ext:Button>
                                                                </Items>
                                                            </ext:Panel>
                                                        </Items>
                                                    </ext:FormPanel>
                                                </Items>
                                            </ext:FieldSet>
                                            <ext:FieldSet ID="FieldSetResMovsCorte" runat="server" Title="Información Actual (Hoy)" Layout="AnchorLayout" Padding="10"
                                                Collapsible="true" Collapsed="true" Visible="true">
                                                <Items>
                                                    <ext:FormPanel ID="FormPanelResumenMovsCorte" runat="server" Layout="FormLayout" Header="false" Border="false">
                                                        <Items>
                                                            <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false" LabelWidth="250">
                                                                <Defaults>
                                                                    <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                                </Defaults>
                                                                <LayoutConfig>
                                                                    <ext:HBoxLayoutConfig Align="Top" />
                                                                </LayoutConfig>
                                                                <Items>
                                                                    <ext:Label runat="server" FieldLabel="Fecha Último Corte" />
                                                                    <ext:Hidden runat="server" Flex="1" Width="25" />
                                                                    <ext:Label ID="lblFechaUltimoCorte" runat="server" Text="-" LabelSeparator=" "
                                                                        Width="150" StyleSpec="text-align:right" />
                                                                </Items>
                                                            </ext:Panel>
                                                            <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false" LabelWidth="250">
                                                                <Defaults>
                                                                    <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                                </Defaults>
                                                                <LayoutConfig>
                                                                    <ext:HBoxLayoutConfig Align="Top" />
                                                                </LayoutConfig>
                                                                <Items>
                                                                    <ext:Label runat="server" FieldLabel="Límite de Crédito" />
                                                                    <ext:Hidden runat="server" Flex="1" Width="25" />
                                                                    <ext:Label ID="lblLimiteCredito" runat="server" Text="-" LabelSeparator=" "
                                                                        Width="150" StyleSpec="text-align:right" />
                                                                </Items>
                                                            </ext:Panel>
                                                            <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false" LabelWidth="250">
                                                                <Defaults>
                                                                    <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                                </Defaults>
                                                                <LayoutConfig>
                                                                    <ext:HBoxLayoutConfig Align="Top" />
                                                                </LayoutConfig>
                                                                <Items>
                                                                    <ext:Label runat="server" FieldLabel="Saldo Actual" />
                                                                    <ext:Hidden runat="server" Flex="1" Width="25" />
                                                                    <ext:Label ID="lblSaldo" runat="server" Text="-" LabelSeparator=" "
                                                                        Width="150" StyleSpec="text-align:right" />
                                                                </Items>
                                                            </ext:Panel>
                                                            <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false" LabelWidth="250">
                                                                <Defaults>
                                                                    <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                                </Defaults>
                                                                <LayoutConfig>
                                                                    <ext:HBoxLayoutConfig Align="Top" />
                                                                </LayoutConfig>
                                                                <Items>
                                                                    <ext:Label runat="server" FieldLabel="Movimientos Pendientes en Tránsito" />
                                                                    <ext:Hidden runat="server" Flex="1" Width="25" />
                                                                    <ext:Label ID="lblMovsPendientes" runat="server" Text="-" LabelSeparator=" "
                                                                        Width="150" StyleSpec="text-align:right" />
                                                                </Items>
                                                            </ext:Panel>
                                                            <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false" LabelWidth="250">
                                                                <Defaults>
                                                                    <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                                </Defaults>
                                                                <LayoutConfig>
                                                                    <ext:HBoxLayoutConfig Align="Top" />
                                                                </LayoutConfig>
                                                                <Items>
                                                                    <ext:Label runat="server" FieldLabel="Crédito Disponible" />
                                                                    <ext:Hidden runat="server" Flex="1" Width="25" />
                                                                    <ext:Label ID="lblCreditoDisponible" runat="server" Text="-" LabelSeparator=" "
                                                                        Width="150" StyleSpec="text-align:right" />
                                                                </Items>
                                                            </ext:Panel>
                                                            <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false" LabelWidth="250">
                                                                <Defaults>
                                                                    <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                                </Defaults>
                                                                <LayoutConfig>
                                                                    <ext:HBoxLayoutConfig Align="Top" />
                                                                </LayoutConfig>
                                                                <Items>
                                                                    <ext:Label runat="server" FieldLabel="Saldo Disponible" />
                                                                    <ext:Hidden runat="server" Flex="1" Width="25" />
                                                                    <ext:Label ID="lblSaldoDisponible" runat="server" Text="-" LabelSeparator=" "
                                                                        Width="150" StyleSpec="text-align:right" />
                                                                </Items>
                                                            </ext:Panel>
                                                            <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false" LabelWidth="250">
                                                                <Defaults>
                                                                    <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                                </Defaults>
                                                                <LayoutConfig>
                                                                    <ext:HBoxLayoutConfig Align="Top" />
                                                                </LayoutConfig>
                                                                <Items>
                                                                    <ext:Label runat="server" FieldLabel="Pago Mínimo" />
                                                                    <ext:Hidden runat="server" Flex="1" Width="25" />
                                                                    <ext:Label ID="lblPagoMinimo" runat="server" Text="-" LabelSeparator=" "
                                                                        Width="150" StyleSpec="text-align:right" />
                                                                </Items>
                                                            </ext:Panel>
                                                            <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false" LabelWidth="250">
                                                                <Defaults>
                                                                    <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                                </Defaults>
                                                                <LayoutConfig>
                                                                    <ext:HBoxLayoutConfig Align="Top" />
                                                                </LayoutConfig>
                                                                <Items>
                                                                    <ext:Label runat="server" FieldLabel="Pago p No Generar Intereses (Antes de FLP)" />
                                                                    <ext:Hidden runat="server" Flex="1" Width="10" />
                                                                    <ext:Label ID="lblPagoNoIntereses" runat="server" Text="-" LabelSeparator=" "
                                                                        Width="165" StyleSpec="text-align:right" />
                                                                </Items>
                                                            </ext:Panel>
                                                            <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false" LabelWidth="250">
                                                                <Defaults>
                                                                    <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                                </Defaults>
                                                                <LayoutConfig>
                                                                    <ext:HBoxLayoutConfig Align="Top" />
                                                                </LayoutConfig>
                                                                <Items>
                                                                    <ext:Label runat="server" FieldLabel="Fecha Límite de Pago (FLP)" />
                                                                    <ext:Hidden runat="server" Flex="1" Width="25" />
                                                                    <ext:Label ID="lblFechaLimitePago" runat="server" Text="-" LabelSeparator=" "
                                                                        Width="150" StyleSpec="text-align:right" />
                                                                </Items>
                                                            </ext:Panel>
                                                            <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false" LabelWidth="250">
                                                                <Defaults>
                                                                    <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                                </Defaults>
                                                                <LayoutConfig>
                                                                    <ext:HBoxLayoutConfig Align="Top" />
                                                                </LayoutConfig>
                                                                <Items>
                                                                    <ext:Label runat="server" FieldLabel="No. de Pagos Vencidos" />
                                                                    <ext:Hidden runat="server" Flex="1" Width="25" />
                                                                    <ext:Label ID="lblNumPagosVencidos" runat="server" Text="-" LabelSeparator=" "
                                                                        Width="150" StyleSpec="text-align:right" />
                                                                </Items>
                                                            </ext:Panel>
                                                            <ext:Panel runat="server" Layout="FitLayout" Width="500" Height="20" Border="false" />
                                                        </Items>
                                                    </ext:FormPanel>
                                                </Items>
                                            </ext:FieldSet>
                                        </Items>
                                    </ext:FormPanel>
                                </Items>
                            </ext:TabPanel>
                        </Center>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
