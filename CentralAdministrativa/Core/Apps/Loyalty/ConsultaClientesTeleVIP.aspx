<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ConsultaClientesTeleVIP.aspx.cs" Inherits="CentroContacto.ConsultaClientesTeleVIP" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
         //var submitValue = function (grid, hiddenFormat, format) {
         //       hiddenFormat.setValue(format);
         //       grid.submitData(false);
         //};

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

         var indMoney = function(v) {
             v = (Math.round((v - 0) * 100)) / 100;
             v = (v == Math.floor(v)) ? v + ".00" : ((v * 10 == Math.floor(v * 10)) ? v + "0" : v);
             v = String(v);
             var ps = v.split('.'),
                 whole = ps[0],
                 sub = ps[1] ? '.' + ps[1] : '.00',
                 r = /(\d+)(\d{2})/;
             while (r.test(whole)) {
                 whole = whole.replace(r, '$1' + ',' + '$2');
             }
             v = whole + sub;
 
             return v;
         }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="MainBorderLayout" runat="server">
        <West Split="true">
            <ext:Panel ID="Panel1" runat="server" Width="350" Collapsible="true">
                <Content>
                    <ext:BorderLayout ID="LeftBorderLayout" runat="server">
                        <North Split="true">
                            <ext:FormPanel ID="FormPanelBusqueda" runat="server" Title="Consulta de Clientes" Height="310" Frame="true" LabelWidth="120" Collapsible="true">
                                <Items>
                                    <ext:FieldSet ID="FieldSetBusqueda" runat="server" Title="Búsqueda">
                                        <Items>
                                            <ext:TextField ID="txtNombre" runat="server" LabelAlign="Right" FieldLabel="Nombre" MaxLength="30" Width="300" />
                                            <ext:TextField ID="txtApPaterno" runat="server" LabelAlign="Right" FieldLabel="Primer Apellido" MaxLength="30" Width="300" />
                                            <ext:TextField ID="txtApMaterno" runat="server" LabelAlign="Right" FieldLabel="Segundo Apellido" MaxLength="30" Width="300" />
                                            <ext:NumberField ID="nfIdCliente" runat="server" LabelAlign="Right" FieldLabel="IDCliente" MaxLength="20" Width="300"
                                                AllowDecimals="False" AllowNegative="False" />
                                            <ext:NumberField ID="nfCuenta" runat="server" LabelAlign="Right" FieldLabel="Cuenta" MaxLength="20" Width="300"
                                                AllowDecimals="False" AllowNegative="False" />
                                            <ext:TextField ID="txtTag" runat="server" LabelAlign="Right" FieldLabel="Tag" MaxLength="16" Width="300" />
                                            <ext:TextField ID="txtCorreo" runat="server" LabelAlign="Right" FieldLabel="Correo Electrónico" MaxLength="30" Width="300" />
                                        </Items>
                                        <Buttons>
                                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                                <DirectEvents>
                                                    <Click OnEvent="btnLimpiar_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnBuscar_Click" Before="var valid= #{FormPanelBusqueda}.getForm().isValid(); if (!valid) {} return valid;">
                                                        <EventMask ShowMask="true" Msg="Buscando Clientes..." MinDelay="500" />
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Buttons>
                                    </ext:FieldSet>
                                </Items>
                            </ext:FormPanel>
                        </North>
                        <Center Split="true">
                            <ext:FormPanel ID="FormPanelResultados" runat="server" Title="Resultados Clientes" Layout="FitLayout">
                                <Items>
                                    <ext:GridPanel ID="GridResultados" runat="server" AutoExpandColumn="NombreTarjetahabiente">
                                        <Store>
                                            <ext:Store ID="StoreClientes" runat="server" OnRefreshData="StoreClientes_Refresh">
                                                <Reader>
                                                    <ext:JsonReader IDProperty="ID_MA">
                                                        <Fields>
                                                            <ext:RecordField Name="ID_MA" />
                                                            <ext:RecordField Name="ID_Cuenta" />
                                                            <ext:RecordField Name="ID_Cliente" />
                                                            <ext:RecordField Name="NombreTarjetahabiente" />
                                                            <ext:RecordField Name="MedioAcceso" />
                                                            <ext:RecordField Name="Email" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Store>
                                        <ColumnModel ID="ColumnModel1" runat="server">
                                            <Columns>
                                                <ext:Column DataIndex="ID_MA" Hidden="true" />
                                                <ext:Column DataIndex="ID_Cliente" Header="IdCliente" Width="50" />
                                                <ext:Column DataIndex="ID_Cuenta" Header="Cuenta" Width="75" />
                                                <ext:Column DataIndex="MedioAcceso" Header="Tag" Width="78" />
                                                <ext:Column DataIndex="NombreTarjetahabiente" Header="Nombre" Width="100" />
                                                <ext:Column DataIndex="Email" Header="Correo" Width="100" />
                                            </Columns>
                                        </ColumnModel>
                                        <SelectionModel>
                                            <ext:RowSelectionModel SingleSelect="true" />
                                        </SelectionModel>
                                        <DirectEvents>
                                            <RowClick OnEvent="selectRowResultados_Event">
                                                <ExtraParams>
                                                    <ext:Parameter Name="Values" Value="Ext.encode(#{GridResultados}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                                </ExtraParams>
                                            </RowClick>
                                        </DirectEvents>
                                        <BottomBar>
                                            <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreClientes" DisplayInfo="true"
                                                DisplayMsg="Clientes {0} - {1} de {2}" />
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
            <ext:Panel ID="Panel2" runat="server" Height="250">
                <Items>
                    <ext:BorderLayout ID="BorderLayout1" runat="server">
                        <Center>
                            <ext:TabPanel ID="TabPanel1" runat="server">
                                <Items>
                                    <ext:FormPanel ID="FormPanelDatos" runat="server" Title="Datos" LabelAlign="Left" LabelWidth="200" >
                                        <Items>
                                            <ext:FieldSet ID="FieldSetDatosCliente" runat="server" Title="Información del Cliente">
                                                <Items>
                                                    <ext:TextField ID="txtID_Cliente" runat="server" Hidden="true" Enabled="false" />
                                                    <ext:TextField ID="txtID_MA" runat="server" Hidden="true" Enabled="false" />
                                                    <ext:TextField ID="txtCuenta" runat="server" Hidden="true" Enabled="false" />
                                                    <ext:Label ID="lblNombreCliente" runat="server" FieldLabel="Nombre" Width="500" />
                                                    <ext:Label ID="lblApPaternoCliente" runat="server" FieldLabel="Primer Apellido" Width="500" />
                                                    <ext:Label ID="lblApMaternoCliente" runat="server" FieldLabel="Segundo Apellido" Width="500" />
                                                    <ext:Label ID="lblEmailCliente" runat="server" FieldLabel="Correo Electrónico" Width="500" />
                                                    <ext:Label ID="lblTelefonoCliente" runat="server" FieldLabel="Teléfono" Width="500" />
                                                    <ext:Label ID="lblFechaNacCliente" runat="server" FieldLabel="Fecha de Nacimiento" Width="500" />
                                                    <ext:Label ID="lblCPCliente" runat="server" FieldLabel="Código Postal" Width="500" />
                                                    <ext:Label ID="lblColonia" runat="server" FieldLabel="Colonia" Width="500"  />
                                                    <ext:Label ID="lblEstadoCliente" runat="server" FieldLabel="Estado" Width="500" />
                                                    <ext:Label ID="lblFechaRegCliente" runat="server" FieldLabel="Fecha de Registro" Width="500" />
                                                </Items>
                                            </ext:FieldSet>
                                        </Items>        
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelMovimientos" runat="server" Title="Movimientos">
                                        <Content>
                                            <%--<ext:Hidden ID="FormatType" runat="server" />--%>
                                            <ext:BorderLayout ID="BorderLayoutMovimientos" runat="server">
                                                <North Split="true">
                                                    <ext:FormPanel ID="FormPanelBuscarMov" runat="server" LabelWidth="200" Height="175">
                                                        <Content>
                                                            <ext:Store ID="StoreTagsPorCuenta" runat="server">
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="ID_MA">
                                                                        <Fields>
                                                                            <ext:RecordField Name="ID_MA" />
                                                                            <ext:RecordField Name="Tag" />
                                                                        </Fields>
                                                                    </ext:JsonReader>
                                                                </Reader>
                                                            </ext:Store>
                                                        </Content>
                                                        <Items>
                                                            <ext:FieldSet ID="FieldSetBuscarMov" runat="server" Title="Movimientos del Tag">
                                                                <Items>
                                                                    <%-- <ext:DateField ID="dfFechaInicialMov" runat="server" FieldLabel="Fecha Inicial" Width="500"
                                                                Text="<%# DateTime.Today.AddDays(-7) %>" MaxDate="<%# DateTime.Today %>"
                                                                Format="dd-MMM-yyyy" Name="FechaInicialMov" AutoDataBind="true" Editable="true" />--%>
                                                                    <ext:DateField ID="dfFechaInicialMov" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                                                        AllowBlank="false" Format="dd-MMM-yyyy" MaxLength="12" TabIndex="1" EnableKeyEvents="true"
                                                                        Width="500" MaxWidth="500">
                                                                        <CustomConfig>
                                                                            <ext:ConfigItem Name="FechaInicialMov" Value="#{dfFechaInicialMov}" Mode="Value" />
                                                                        </CustomConfig>
                                                                        <Listeners>
                                                                            <KeyUp Fn="onKeyUp" />
                                                                        </Listeners>
                                                                    </ext:DateField>
                                                                    <ext:DateField ID="dfFechaFinalMov" runat="server" Vtype="daterange" FieldLabel="Fecha Final"
                                                                        AllowBlank="false" MaxLength="12" Format="dd-MMM-yyyy" TabIndex="2" EnableKeyEvents="true"
                                                                        Width="500" MaxWidth="500">
                                                                        <CustomConfig>
                                                                            <ext:ConfigItem Name="FechaFinalMov" Value="#{dfFechaFinalMov}" Mode="Value" />
                                                                        </CustomConfig>
                                                                        <Listeners>
                                                                            <KeyUp Fn="onKeyUp" />
                                                                        </Listeners>
                                                                    </ext:DateField>
                                                                    <%--<ext:DateField ID="dfFechaFinalMov" runat="server" FieldLabel="Fecha Final" Width="500" 
                                                                MaxDate="<%# DateTime.Today %>" Text="<%# DateTime.Today %>" Format="dd-MMM-yyyy"
                                                                Name="FechaFinalMov" AutoDataBind="true" Editable="true" />--%>
                                                                    <ext:ComboBox ID="cBoxTags" runat="server" FieldLabel="Tags" StoreID="StoreTagsPorCuenta" Width="500"
                                                                        DisplayField="Tag" ValueField="Tag" />
                                                                </Items>
                                                                <Buttons>
                                                                    <ext:Button ID="btnBuscarMov" runat="server" Text="Buscar" Icon="Magnifier">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="btnBuscarMov_Click" Before="var valid= #{FormPanelBuscarMov}.getForm().isValid(); if (!valid) {} return valid;" />
                                                                        </DirectEvents>
                                                                    </ext:Button>
                                                                </Buttons>
                                                            </ext:FieldSet>
                                                        </Items>
                                                    </ext:FormPanel>
                                                </North>
                                                <Center Split="true">
                                                    <ext:Panel ID="PanelCentral" runat="server">
                                                        <Items>
                                                            <ext:BorderLayout ID="BorderLayoutCentralZone" runat="server">
                                                                <North Split="true">
                                                                    <ext:FormPanel ID="FormPanelMovsAcumulados" runat="server" Title="Periodo Actual" Collapsible="true" Height="120">
                                                                        <Items>
                                                                            <ext:Toolbar ID="Toolbar1" runat="server" LabelWidth="120">
                                                                                <Items>
                                                                                    <ext:Label ID="lblMes1" FieldLabel="Mes 1" Width="300" runat="server" StyleSpec="text-align:left;display:block;"/>
                                                                                    <ext:Label ID="lblAcumMes1" FieldLabel="Acumulado" Text="0.00" Width="200" runat="server" StyleSpec="text-align:right;display:block;"/>
                                                                                </Items>
                                                                            </ext:Toolbar>
                                                                            <ext:Toolbar ID="Toolbar2" runat="server" LabelWidth="120">
                                                                                <Items>
                                                                                    <ext:Label ID="lblMes2" FieldLabel="Mes 2" Width="300" runat="server" StyleSpec="text-align:left;display:block;"/>
                                                                                    <ext:Label ID="lblAcumMes2" FieldLabel="Acumulado" Text="0.00" Width="200" runat="server" StyleSpec="text-align:right;display:block;" />
                                                                                </Items>
                                                                            </ext:Toolbar>
                                                                            <ext:Toolbar ID="Toolbar3" runat="server" LabelWidth="120">
                                                                                <Items>
                                                                                    <ext:Label ID="lblMes3" FieldLabel="Mes 3" Width="300" runat="server" StyleSpec="text-align:left;display:block;"/>
                                                                                    <ext:Label ID="lblAcumMes3" FieldLabel="Acumulado" Text="0.00" Width="200" runat="server"  StyleSpec="text-align:right;display:block;align:center;"/>
                                                                                </Items>
                                                                            </ext:Toolbar>
                                                                            <ext:Toolbar ID="Toolbar4" runat="server" LabelWidth="150" >
                                                                                <Items>
                                                                                    <ext:Label ID="lblAcumuladoPeriodo" FieldLabel="Acumulado del Periodo" Width="200" runat="server" StyleSpec="text-align:center;display:block;"/>
                                                                                </Items>
                                                                            </ext:Toolbar>
                                                                        </Items>
                                                                    </ext:FormPanel>
                                                                </North>
                                                                <Center Split="true">
                                                                    <ext:GridPanel ID="GridResultadosMov" runat="server" Header="true" Title="Nombre:">
                                                                        <Store>
                                                                            <ext:Store ID="StoreResultadosMov" runat="server" OnRefreshData="btnBuscarMov_Click">
                                                                                <DirectEventConfig IsUpload="true" />
                                                                                <Reader>
                                                                                    <ext:JsonReader IDProperty="Fecha">
                                                                                        <Fields>
                                                                                            <ext:RecordField Name="ID_MOVIMIENTO" />
                                                                                            <ext:RecordField Name="EsAplicable" />
                                                                                            <ext:RecordField Name="Fecha" />
                                                                                            <ext:RecordField Name="Fecha" />
                                                                                            <ext:RecordField Name="SALIDA" />
                                                                                            <ext:RecordField Name="TARIFA" />
                                                                                        </Fields>
                                                                                    </ext:JsonReader>
                                                                                </Reader>
                                                                                <DirectEventConfig IsUpload="true" />
                                                                            </ext:Store>
                                                                        </Store>
                                                                        <ColumnModel ID="ColumnModel2" runat="server">
                                                                            <Columns>
                                                                                <ext:Column DataIndex="ID_MOVIMIENTO" Hidden="true" />
                                                                                <ext:Column DataIndex="EsAplicable" Header="Cuenta para recompensa" Width="150" />
                                                                                <ext:DateColumn DataIndex="Fecha" Header="Fecha" Align="Center" Format="dd-MMM-yyyy" Width="100" />
                                                                                <ext:DateColumn DataIndex="Fecha" Header="Hora" Align="Center" Format="HH:mm:ss" Width="100" />
                                                                                <ext:Column DataIndex="SALIDA" Header="Trayecto" />
                                                                                <ext:Column DataIndex="TARIFA" Header="Monto" Align="Right">
                                                                                    <Renderer Handler="return indMoney(value);" />
                                                                                </ext:Column>
                                                                            </Columns>
                                                                        </ColumnModel>
                                                                        <SelectionModel>
                                                                            <ext:RowSelectionModel SingleSelect="true" />
                                                                        </SelectionModel>
                                                                        <BottomBar>
                                                                            <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="StoreResultadosMov" DisplayInfo="true"
                                                                                DisplayMsg="Movimientos {0} - {1} de {2}" />
                                                                        </BottomBar>
                                                                        <TopBar>
                                                                            <ext:Toolbar ID="Toolbar5" runat="server">
                                                                                <Items>
                                                                                    <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                                                    <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="true">
                                                                                        <DirectEvents>
                                                                                            <Click OnEvent="ExportGridToExcel" IsUpload="true" Success="Ext.Net.DirectMethods.Download({IsUpload:true})">
                                                                                                <ExtraParams>
                                                                                                    <ext:Parameter Name="GridToExport" Value="Ext.encode(#{GridResultadosMov}.getRowsValues({selectedOnly : false}))" Mode="Raw" />
                                                                                                </ExtraParams>
                                                                                            </Click>
                                                                                        </DirectEvents>
                                                                                    </ext:Button>
                                                                                    <%--<ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" Icon="PageExcel">
                                                                                        <Listeners>
                                                                                            <Click Handler="submitValue(#{GridResultadosMov}, #{FormatType}, 'xls');" />
                                                                                        </Listeners>
                                                                                    </ext:Button>--%>
                                                                                </Items>
                                                                            </ext:Toolbar>
                                                                        </TopBar>
                                                                    </ext:GridPanel>
                                                                </Center>
                                                            </ext:BorderLayout>
                                                        </Items>
                                                    </ext:Panel>
                                                </Center>
                                            </ext:BorderLayout>
                                        </Content>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelRecompensas" runat="server" Title="Recompensas" LabelAlign="Left" LabelWidth="200">
                                        <Content>
                                            <ext:BorderLayout ID="BorderLayoutRecompensas" runat="server">
                                                <North Split="true">
                                                    <ext:FormPanel ID="FormPanelBuscarRec" runat="server" LabelWidth="200" Height="100">
                                                        <Content>
                                                            <ext:Store ID="StoreTagsRec" runat="server">
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="ID_MA">
                                                                        <Fields>
                                                                            <ext:RecordField Name="ID_MA" />
                                                                            <ext:RecordField Name="Tag" />
                                                                        </Fields>
                                                                    </ext:JsonReader>
                                                                </Reader>
                                                            </ext:Store>
                                                        </Content>
                                                        <Items>
                                                            <ext:FieldSet ID="FieldSetBuscarRec" runat="server" Title="Búsqueda">
                                                                <Items>
                                                                    <ext:ComboBox ID="cBoxTagsBuscarRec" runat="server" FieldLabel="Tags" StoreID="StoreTagsRec" Width="500"
                                                                        DisplayField="Tag" ValueField="Tag" />
                                                                </Items>
                                                                <Buttons>
                                                                    <ext:Button ID="btnBuscarRecompensas" runat="server" Text="Buscar" Icon="Magnifier">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="btnBuscarRecompensas_Click">
                                                                                <EventMask ShowMask="true" Msg="Buscando Recompensas..." MinDelay="500" />
                                                                            </Click>
                                                                        </DirectEvents>
                                                                    </ext:Button>
                                                                </Buttons>
                                                            </ext:FieldSet>
                                                        </Items>
                                                    </ext:FormPanel>
                                                </North>
                                                <Center Split="true">
                                                    <ext:GridPanel ID="GridResultadosRec" runat="server" Title="Nombre:">
                                                        <Store>
                                                            <ext:Store ID="StoreResultadosRec" runat="server">
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="ID_Recompensa">
                                                                        <Fields>
                                                                            <ext:RecordField Name="ID_Recompensa" />
                                                                            <ext:RecordField Name="FechaAplicacionRecompensa" />
                                                                            <ext:RecordField Name="MontoRecompensa" />
                                                                            <ext:RecordField Name="Descripcion" />
                                                                            <ext:RecordField Name="MotivoRecompensa" />
                                                                        </Fields>
                                                                    </ext:JsonReader>
                                                                </Reader>
                                                            </ext:Store>
                                                        </Store>
                                                        <ColumnModel ID="ColumnModel3" runat="server">
                                                            <Columns>
                                                                <ext:Column DataIndex="ID_Recompensa" Hidden="true" />
                                                                <ext:DateColumn DataIndex="FechaAplicacionRecompensa" Header="Fecha" Format="dd-MMM-yyyy" Width="80" />
                                                                <ext:Column DataIndex="MontoRecompensa" Header="Monto Recompensa" Align="Right" Width="150" />
                                                                <ext:Column DataIndex="Descripcion" Header="Estatus" Width="180" />
                                                                <ext:Column DataIndex="MotivoRecompensa" Header="Motivo" Width="150" />
                                                            </Columns>
                                                        </ColumnModel>
                                                        <SelectionModel>
                                                            <ext:RowSelectionModel SingleSelect="true" />
                                                        </SelectionModel>
                                                        <BottomBar>
                                                            <ext:PagingToolbar ID="PagingToolBar3" runat="server" StoreID="StoreResultadosRec" DisplayInfo="true"
                                                                DisplayMsg="Recompensas {0} - {1} de {2}" />
                                                        </BottomBar>
                                                    </ext:GridPanel>
                                                </Center>
                                            </ext:BorderLayout>
                                        </Content>        
                                    </ext:FormPanel>
                                    <%--<ext:FormPanel ID="FormPanelCapturarLlamada" runat="server" Title="Capturar Llamada">
                                        <Content>
                                            <ext:Store ID="StoreMotivos" runat="server">
                                                <Reader>
                                                    <ext:JsonReader IDProperty="ID_Actividad">
                                                        <Fields>
                                                            <ext:RecordField Name="ID_Actividad" />
                                                            <ext:RecordField Name="Descripcion" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Content>
                                        <Items>
                                            <ext:FormPanel ID="FormPanelLlamada" runat="server" Title="Nombre:">
                                                <Items>
                                                    <ext:FieldSet ID="FieldSetCapturarLlamada" runat="server" Title="Captura" Height="450">
                                                        <Items>
                                                            <ext:ComboBox ID="cBoxMotivoLlamada" runat="server" FieldLabel="Motivo Llamada" StoreID="StoreMotivos" Width="500"
                                                                DisplayField="Descripcion" ValueField="ID_Actividad" AllowBlank="false"/>
                                                            <ext:TextArea ID="txtComentarios" runat="server" FieldLabel="Comentarios" BoxLabel="CheckBox" Width="500" 
                                                                Height="350" MaxLengthText="300" AllowBlank="false"/>
                                                        </Items>
                                                        <Buttons>
                                                            <ext:Button ID="btnGuardarLlamada" runat="server" Text="Guardar" Icon="Disk">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnGuardarLlamada_Click" Before="var valid= #{FormPanelLlamada}.getForm().isValid(); if (!valid) {} return valid;" />
                                                                </DirectEvents>
                                                            </ext:Button>
                                                        </Buttons>
                                                    </ext:FieldSet>
                                                </Items>
                                            </ext:FormPanel>
                                        </Items>        
                                    </ext:FormPanel>--%>
                                </Items>
                            </ext:TabPanel>
                        </Center>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
