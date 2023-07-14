<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="ConsultaClientesTarjetas.aspx.cs" Inherits="CentroContacto.ConsultaClientesTarjetas" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .total-field {
            font-weight      : bold !important;
            //color            : #000;
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
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Hidden ID="hdnIdCuentaLDC" runat="server" />
    <ext:Hidden ID="hdnIdCuentaCC" runat="server" />
    <ext:Hidden ID="hdnIdCuentaCDC" runat="server" />
    <ext:Hidden ID="hdnIdMA" runat="server" />
    <ext:Hidden ID="hdnTarjeta" runat="server" />
    <ext:Hidden ID="hdnMaskCard" runat="server" />
    <ext:Hidden ID="hdnIdColectiva" runat="server" />
    <ext:Hidden ID="hdnNumCel" runat="server" />
    <ext:Hidden ID="hdnTarjetahabiente" runat="server" />
    <ext:Window ID="WdwConfirmaTokenSMS" runat="server" Title="Confirmar Token" Width="300" Height="160"
        Hidden="true" Modal="true" Resizable="false">
        <Items>
            <ext:FormPanel ID="FormPanelTknSMS" runat="server" Padding="10" LabelWidth="5" Border="false" Layout="FormLayout">
                <Items>
                    <ext:Label ID="lblTokenSMS" runat="server" LabelAlign="Top" Width="200"
                        Text="Ingresa el Token que te Proporcione el Cliente, mismo que Recibió por Mensaje SMS:" />
                    <ext:Panel runat="server" Layout="FitLayout" Height="10" Border="false" />
                    <ext:TextField ID="txtTokenSMS" runat="server" LabelAlign="Top" AllowBlank="false" Width="250" MaxLength="10"/>
                </Items>
                <Buttons>
                    <ext:Button ID="btnValidaToken" runat="server" Text="Validar Token">
                        <DirectEvents>
                            <Click OnEvent="btnValidaToken_Click" Before="var valid= #{FormPanelTknSMS}.getForm().isValid(); if (!valid) {} return valid;">
                                <EventMask ShowMask="true" Msg="Validando Token..." MinDelay="500" />
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                    <ext:Button runat="server" Text="Cancelar y Registrar Llamada">
                        <Listeners>
                            <Click Handler="#{WdwConfirmaTokenSMS}.hide(); ConsultaClientesTarjetas.CancelarSMS();" />
                        </Listeners>
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
                                                                ConsultaClientesTarjetas.StopMask();" />
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
                                                                ConsultaClientesTarjetas.StopMask();" />
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
                                                            ConsultaClientesTarjetas.StopMask();" />
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
                                                            ConsultaClientesTarjetas.StopMask();" />
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
                            <ext:FormPanel ID="FormPanelBusqueda" runat="server" Title="Filtros" Height="260" Frame="true" LabelWidth="120"
                                Collapsible="true" Border="false">
                                <Items>
                                    <ext:FieldSet ID="FieldSetBusqueda" runat="server" Border="false">
                                        <Items>
                                            <ext:ComboBox ID="cBoxSubEm" runat="server" FieldLabel="SubEmisor" Width="310" MinChars="1"
                                                DisplayField="NombreORazonSocial" ValueField="ID_Colectiva" Mode="Local" AutoSelect="true"
                                                Editable="true" ForceSelection="true" TypeAhead="true" MatchFieldWidth="false" ListWidth="320">
                                                <Store>
                                                    <ext:Store ID="StoreSubEmisores" runat="server">
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
                                                <Triggers>
                                                    <ext:FieldTrigger Icon="Clear" HideTrigger="true" />
                                                </Triggers>
                                                <Listeners>
                                                    <BeforeQuery Handler="this.triggers[0][ this.getRawValue().toString().length == 0 ? 'hide' : 'show']();" />
                                                    <TriggerClick Handler="if (index == 0) { this.focus().clearValue(); trigger.hide();}" />
                                                    <Select Handler="this.triggers[0].show();" />
                                                </Listeners>
                                            </ext:ComboBox>
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
                                                        Before="if (!#{txtNumTarjeta}.isValid()) {
                                                            return false; }
                                                        else if (!#{cBoxSubEm}.getValue() && !#{txtNumCuenta}.getValue() && !#{txtNumTarjeta}.getValue() &&
                                                        !#{txtNombre}.getValue() && !#{txtApPaterno}.getValue() && !#{txtApMaterno}.getValue()) {
                                                            Ext.Msg.alert('Consulta de Cuentas', 'Ingresa al menos un criterio de búsqueda'); 
                                                            return false; }
                                                        else if (#{cBoxSubEm}.isValid() && (!#{txtNumCuenta}.getValue() && !#{txtNumTarjeta}.getValue() &&
                                                        !#{txtNombre}.getValue() && !#{txtApPaterno}.getValue() && !#{txtApMaterno}.getValue())) { 
                                                            Ext.Msg.alert('Consulta de Cuentas', 'El filtro SubEmisor necesita al menos otro criterio de búsqueda');
                                                            return false; }
                                                        else {
                                                            #{GridResultados}.getStore().removeAll(); #{EastPanelCCT}.setTitle('_');
                                                            #{EastPanelCCT}.setDisabled(true); }">
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
                            <ext:FormPanel ID="FormPanelResultados" runat="server" Title="Resultados" Layout="FitLayout" Border="false">
                                <Items>
                                    <ext:GridPanel ID="GridResultados" runat="server" Height="450" AutoDoLayout="true" Border="false"
                                        AutoExpandColumn="NombreTarjetahabiente">
                                        <Store>
                                            <ext:Store ID="StoreCuentas" runat="server">
                                                <Reader>
                                                    <ext:JsonReader IDProperty="IdTarjeta">
                                                        <Fields>
                                                            <ext:RecordField Name="IdTarjeta" />
                                                            <ext:RecordField Name="ClaveMA" />
                                                            <ext:RecordField Name="NumTarjeta" />
                                                            <ext:RecordField Name="IdColectivaCuentahabiente" />
                                                            <ext:RecordField Name="NombreTarjetahabiente" />
                                                            <ext:RecordField Name="TelCelular" />
                                                            <ext:RecordField Name="MaskCel" />
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
                                                <ext:Column DataIndex="NumTarjeta" Header="Tarjeta" Width="110" /> 
                                                <ext:Column DataIndex="NombreTarjetahabiente" Header="Nombre" />
                                                <ext:Column DataIndex="MaskCel" Header="Tel. Celular" />
                                            </Columns>
                                        </ColumnModel>
                                        <SelectionModel>
                                            <ext:RowSelectionModel SingleSelect="true" />
                                        </SelectionModel>
                                        <DirectEvents>
                                            <RowClick OnEvent="selectRowResultados_Event" After="#{lblMensaje}.text = msjConfirma(
                                                #{hdnTarjetahabiente}.getValue(), #{hdnMaskCard}.getValue(), #{hdnNumCel}.getValue());">
                                                <EventMask ShowMask="true" Msg="Obteniendo Información de la Cuenta..." MinDelay="500" />
                                                <ExtraParams>
                                                    <ext:Parameter Name="Values" Value="Ext.encode(#{GridResultados}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                                </ExtraParams>
                                            </RowClick>
                                        </DirectEvents>
                                        <BottomBar>
                                            <ext:PagingToolbar ID="PagingCuentas" runat="server" StoreID="StoreCuentas" DisplayInfo="true" HideRefresh="true"
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
            <ext:Panel ID="EastPanelCCT" runat="server" Title="_" Disabled="true" Border="false">
                <Items>
                    <ext:BorderLayout runat="server">
                        <Center>
                            <ext:TabPanel runat="server">
                                <Items>
                                    <ext:FormPanel ID="FormPanelLlamadas" runat="server" Title="Registrar Llamada" Layout="FormLayout" Border="false"
                                        LabelAlign="Left" LabelWidth="150">
                                        <Content>
                                            <ext:BorderLayout runat="server">
                                                <Center Split="true">
                                                    <ext:FieldSet ID="FieldSetLlamadas" runat="server" Border="false" Layout="FormLayout">
                                                        <Items>
                                                            <ext:ComboBox ID="cBoxMotivoLlamada" runat="server" FieldLabel="Motivo de Llamada" Width="535"
                                                                DisplayField="Descripcion" ValueField="ID_MotivoLlamada" AllowBlank="false">
                                                                <Store>
                                                                    <ext:Store ID="StoreMotivos" runat="server">
                                                                        <Reader>
                                                                            <ext:JsonReader IDProperty="ID_MotivoLlamada">
                                                                                <Fields>
                                                                                    <ext:RecordField Name="ID_MotivoLlamada" />
                                                                                    <ext:RecordField Name="Descripcion" />
                                                                                </Fields>
                                                                            </ext:JsonReader>
                                                                        </Reader>
                                                                    </ext:Store>
                                                                </Store>
                                                            </ext:ComboBox>
                                                            <ext:TextField ID="txtUsuarioLlama" runat="server" FieldLabel="Persona que Llama" Width="535" MaxLength="50" />
                                                            <ext:TextArea ID="txtComentarios" runat="server" FieldLabel="Comentarios" BoxLabel="CheckBox" Width="535"
                                                                Height="100" MaxLength="2000" AllowBlank="false" />
                                                        </Items>
                                                        <Buttons>
                                                            <ext:Button ID="btnHistLlamadas" runat="server" Hidden="true">
                                                                <Listeners>
                                                                    <Click Handler="resetToolbar(#{PagingHistorial});
                                                                        #{GridHistorial}.getStore().sortInfo = null;
                                                                        #{GridHistorial}.getStore().reload({params:{start:0, sort:('','')}});" />
                                                                </Listeners>
                                                            </ext:Button>
                                                            <ext:Button ID="btnGuardarLlamada" runat="server" Text="Guardar" Icon="Disk">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnGuardarLlamada_Click" Before="if (!#{cBoxMotivoLlamada}.isValid() ||
                                                                        !#{txtComentarios}.isValid()) { return false; }" />
                                                                </DirectEvents>
                                                            </ext:Button>
                                                        </Buttons>
                                                    </ext:FieldSet>
                                                </Center>
                                                <South Split="true">
                                                    <ext:GridPanel ID="GridHistorial" runat="server" Layout="FitLayout" Height="250" Border="false"
                                                        Header="true" Title="Historial de Llamadas" AutoExpandColumn="Comentarios">
                                                        <Store>
                                                            <ext:Store ID="StoreHistorial" runat="server" RemoteSort="true" AutoLoad="false"
                                                                OnRefreshData="StoreHistorial_RefreshData">
                                                                <AutoLoadParams>
                                                                    <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                                                </AutoLoadParams>
                                                                <Proxy>
                                                                    <ext:PageProxy />
                                                                </Proxy>
                                                                <DirectEventConfig IsUpload="true" />
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="ID_BitacoraLlamada">
                                                                        <Fields>
                                                                            <ext:RecordField Name="ID_BitacoraLlamada" />
                                                                            <ext:RecordField Name="Fecha" />
                                                                            <ext:RecordField Name="Motivo" />
                                                                            <ext:RecordField Name="Persona" />
                                                                            <ext:RecordField Name="Comentarios" />
                                                                        </Fields>
                                                                    </ext:JsonReader>
                                                                </Reader>
                                                            </ext:Store>
                                                        </Store>
                                                        <ColumnModel runat="server">
                                                            <Columns>
                                                                <ext:Column DataIndex="ID_BitacoraLlamada" Hidden="true" />
                                                                <ext:DateColumn DataIndex="Fecha" Header="Fecha/Hora" Format="yyyy-MM-dd HH:mm:ss" Width="150" />
                                                                <ext:Column DataIndex="Motivo" Header="Motivo" Width="150"/>
                                                                <ext:Column DataIndex="Persona" Header="Persona" Width="150" />
                                                                <ext:Column DataIndex="Comentarios" Header="Comentarios" />
                                                            </Columns>
                                                        </ColumnModel>
                                                        <SelectionModel>
                                                            <ext:RowSelectionModel SingleSelect="true" />
                                                        </SelectionModel>
                                                        <TopBar>
                                                            <ext:Toolbar runat="server">
                                                                <Items>
                                                                    <ext:ToolbarFill runat="server" />
                                                                    <ext:Button ID="btnExcelHistorial" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="true">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="btnExcelHistorial_Click" IsUpload="true"
                                                                                After="Ext.net.Mask.show({ msg : 'Exportando a Excel...' });
                                                                                    ConsultaClientesTarjetas.StopMask();" />
                                                                        </DirectEvents>
                                                                    </ext:Button>
                                                                </Items>
                                                            </ext:Toolbar>
                                                        </TopBar>
                                                        <BottomBar>
                                                            <ext:PagingToolbar ID="PagingHistorial" runat="server" StoreID="StoreHistorial" DisplayInfo="true"
                                                                DisplayMsg="Mostrando Llamadas {0} - {1} de {2}" HideRefresh="true" />
                                                        </BottomBar>
                                                    </ext:GridPanel>
                                                </South>
                                            </ext:BorderLayout>
                                        </Content>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelTitular" runat="server" Title="Titular" LabelAlign="Left" LabelWidth="150" AutoScroll="true"
                                        Border="false" Layout="FormLayout" Disabled="true">
                                        <Items>
                                            <ext:FieldSet ID="FieldSetDatosTitular" runat="server" Border="false" Layout="FormLayout">
                                                <Items>
                                                    <ext:Panel ID="PanelDatosPersonales" runat="server" Title="Datos Personales" AutoHeight="true" LabelAlign="Left"
                                                        FormGroup="true" Layout="FormLayout">
                                                        <Items>
                                                            <ext:TextField ID="txtNombreClienteTitular" runat="server" FieldLabel="Nombre" Width="535" ReadOnly="true" />
                                                            <ext:TextField ID="txtApPaternoTitular" runat="server" FieldLabel="Primer Apellido" Width="535" ReadOnly="true" />
                                                            <ext:TextField ID="txtApMaternoTitular" runat="server" FieldLabel="Segundo Apellido" Width="535" ReadOnly="true" />
                                                            <ext:TextField ID="txtNombreEmbozo" runat="server" FieldLabel="Nombre Embozo" Width="535" ReadOnly="true" />
                                                            <ext:Panel runat="server" Layout="FitLayout" Width="535" Height="5" Border="false" />
                                                            <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false">
                                                                <Defaults>
                                                                    <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                                </Defaults>
                                                                <LayoutConfig>
                                                                    <ext:HBoxLayoutConfig Align="Top" />
                                                                </LayoutConfig>
                                                                <Items>
                                                                    <ext:DateField ID="dfFechaNac" runat="server" FieldLabel="Fecha de Nacimiento" ReadOnly="true"
                                                                        Width="285" Format="dd/MM/yyyy" Vtype="daterange" />
                                                                    <ext:Hidden runat="server" Width="15" />
                                                                    <ext:TextField ID="txtRFCTitular" runat="server" FieldLabel="RFC" LabelWidth="40" ReadOnly="true" Width="175" />
                                                                    <ext:Hidden runat="server" Width="15" />
                                                                    <ext:TextField ID="txtCURP" runat="server" FieldLabel="CURP" LabelWidth="50" ReadOnly="true" Width="200" />
                                                                </Items>
                                                            </ext:Panel>
                                                        </Items>
                                                    </ext:Panel>
                                                    <ext:Panel ID="PanelDomicilio" runat="server" Title="Domicilio" AutoHeight="true" LabelAlign="Left"
                                                        FormGroup="true" Layout="FormLayout" Collapsed="true">
                                                        <Items>
                                                            <ext:TextField ID="txtID_Direccion" runat="server" Hidden="true" Enabled="false" />
                                                            <ext:TextField ID="txtCalle" runat="server" FieldLabel="Calle" Width="535" ReadOnly="true"/>
                                                            <ext:TextField ID="txtNumExterior" runat="server" FieldLabel="Número Exterior" Width="535" ReadOnly="true" />
                                                            <ext:TextField ID="txtNumInterior" runat="server" FieldLabel="Número Interior" Width="535" ReadOnly="true" />
                                                            <ext:TextField ID="txtEntreCalles" runat="server" FieldLabel="Entre las Calles" Width="535" ReadOnly="true" />
                                                            <ext:TextField ID="txtReferencias" runat="server" FieldLabel="Referencias del Domicilio" Width="535" ReadOnly="true" />
                                                            <ext:TextField ID="txtCPCliente" runat="server" FieldLabel="Código Postal" Width="535" ReadOnly="true" />
                                                            <ext:TextField ID="txtIDColonia" runat="server" Hidden="true" Enabled="false" />
                                                            <ext:TextField ID="txtColonia" runat="server" FieldLabel="Colonia" Width="535" ReadOnly="true" />
                                                            <ext:TextField ID="txtClaveMunicipio" runat="server" Hidden="true" Enabled="false" />
                                                            <ext:TextField ID="txtMunicipioTitular" runat="server" FieldLabel="Delegación o Municipio" ReadOnly="true" Width="535" />
                                                            <ext:TextField ID="txtClaveEstado" runat="server" Hidden="true" Enabled="false" />
                                                            <ext:TextField ID="txtEstadoTitular" runat="server" FieldLabel="Estado" ReadOnly="true" Width="535" />
                                                        </Items>
                                                    </ext:Panel>
                                                    <ext:Panel ID="PanelDatosContacto" runat="server" Title="Datos de Contacto" AutoHeight="true" LabelAlign="Left"
                                                        FormGroup="true" Layout="FormLayout">
                                                        <Items>
                                                            <ext:TextField ID="txtTelParticular" runat="server" FieldLabel="Teléfono Particular" MaxLength="20" Width="535"
                                                                ReadOnly="true" />
                                                            <ext:TextField ID="txtTelCelular" runat="server" FieldLabel="Teléfono Celular" MaxLength="20" Width="535"
                                                                ReadOnly="true" />
                                                            <ext:TextField ID="txtTelTrabajo" runat="server" FieldLabel="Teléfono Trabajo" MaxLength="20" Width="535"
                                                                ReadOnly="true" />
                                                            <ext:TextField ID="txtCorreo" runat="server" FieldLabel="Correo Electrónico" MaxLength="120" Width="535"
                                                                ReadOnly="true" />
                                                        </Items>
                                                    </ext:Panel>
                                                    <ext:Panel ID="PanelDomFiscal" runat="server" Title="Datos Fiscales" AutoHeight="true" LabelAlign="Left"
                                                        FormGroup="true" Layout="FormLayout" Collapsed="true">
                                                        <Items>
                                                            <ext:TextField ID="txtIDDirFiscal" runat="server" Hidden="true" Enabled="false" />
                                                            <ext:TextField ID="txtCalleFiscal" runat="server" FieldLabel="Calle" Width="535" ReadOnly="true"/>
                                                            <ext:TextField ID="txtNumExtFiscal" runat="server" FieldLabel="Número Exterior" MaxLength="50" Width="535" ReadOnly="true" />
                                                            <ext:TextField ID="txtNumIntFiscal" runat="server" FieldLabel="Número Interior" MaxLength="50" Width="535" ReadOnly="true" />
                                                            <ext:TextField ID="txtCPFiscal" runat="server" FieldLabel="Código Postal" Width="535" EnableKeyEvents="true" ReadOnly="true" />
                                                            <ext:TextField ID="txtIDColFiscal" runat="server" Hidden="true" Enabled="false" />
                                                            <ext:TextField ID="txtColFiscal" runat="server" FieldLabel="Colonia" Width="535" ReadOnly="true" />
                                                            <ext:TextField ID="txtCveMunFiscal" runat="server" Hidden="true" Enabled="false" />
                                                            <ext:TextField ID="txtDelMunFiscal" runat="server" FieldLabel="Delegación o Municipio" ReadOnly="true" Width="535" />
                                                            <ext:TextField ID="txtCveEdoFiscal" runat="server" Hidden="true" Enabled="false" />
                                                            <ext:TextField ID="txtEstadoFiscal" runat="server" FieldLabel="Estado" ReadOnly="true" Width="535" />
                                                            <ext:TextField ID="txtRegimenFiscal" runat="server" FieldLabel="Régimen Fiscal" Width="535" ReadOnly="true" />
                                                            <ext:TextField ID="txtUsoCFDI" runat="server" FieldLabel="Uso de CFDI" Width="535" ReadOnly="true" />
                                                        </Items>
                                                    </ext:Panel>
                                                </Items>
                                            </ext:FieldSet>
                                        </Items>        
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelTarjetas" runat="server" Title="Tarjetas" Layout="FitLayout" AutoScroll="true"
                                        Border="false" Disabled="true">
                                        <Items>
                                            <ext:GridPanel ID="GridFamTarjetas" runat="server" Height="750" Title="Tarjetas de la Cuenta"
                                                Border="false" AutoExpandColumn="Tarjetahabiente">
                                                <Store>
                                                    <ext:Store ID="StoreFamTarjetas" runat="server">
                                                        <Reader>
                                                            <ext:JsonReader IDProperty="ID_MA">
                                                                <Fields>
                                                                    <ext:RecordField Name="ID_MA" />
                                                                    <ext:RecordField Name="ClaveMA" />
                                                                    <ext:RecordField Name="Tarjeta" />
                                                                    <ext:RecordField Name="Tipo" />
                                                                    <ext:RecordField Name="Tarjetahabiente" />
                                                                    <ext:RecordField Name="TipoManufactura" />
                                                                    <ext:RecordField Name="Expiracion" />
                                                                    <ext:RecordField Name="LimiteCredito" />
                                                                    <ext:RecordField Name="Estatus" />
                                                                    <ext:RecordField Name="EstatusLetra" />
                                                                </Fields>
                                                            </ext:JsonReader>
                                                        </Reader>
                                                    </ext:Store>
                                                </Store>
                                                <ColumnModel ID="ColumnModel3" runat="server">
                                                    <Columns>
                                                        <ext:Column DataIndex="ID_MA" Hidden="true" />
                                                        <ext:Column DataIndex="Tarjeta" Header="Tarjeta" Width="110" />
                                                        <ext:Column DataIndex="Tipo" Header="Tipo" Width="80" />
                                                        <ext:Column DataIndex="Tarjetahabiente" Header="Tarjetahabiente" />
                                                        <ext:Column DataIndex="TipoManufactura" Header="Manufactura" Width="80" />
                                                        <ext:DateColumn DataIndex="Expiracion" Header="Expiración" Align="Center"
                                                            Format="dd-MMM-yyyy" Width="80  " />
                                                        <ext:Column DataIndex="LimiteCredito" Header="Límite de Crédito">
                                                            <Renderer Format="UsMoney" />
                                                        </ext:Column>
                                                        <ext:Column DataIndex="EstatusLetra" Header="Estatus" Width="80" />
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
                                                    <ext:PagingToolbar ID="PagingToolBar3" runat="server" StoreID="StoreFamTarjetas" DisplayInfo="true"
                                                        DisplayMsg="Tarjetas {0} - {1} de {2}" />
                                                </BottomBar>
                                            </ext:GridPanel>
                                        </Items>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelSaldosYMovs" runat="server" Title="Saldos y Movimientos" AutoScroll="true" Layout="FormLayout"
                                        Border="false" Padding="10" Disabled="true">
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
