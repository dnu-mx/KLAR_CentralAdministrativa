<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ConsultaOperacionesGas.aspx.cs" 
    Inherits="ValidacionesBatch.ConsultaOperacionesGas" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
     <style type="text/css">
        .new-label-title
        {
            color:blue;
            font-weight:bold;
        }
    </style>

    <script type="text/javascript">
        var template = '<span style="color:{0};text-decoration:underline;">{1}</span>';

        var link = function (value) {
            return String.format(template, (value != "") ? "blue" : "black", value);
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

        var submitValue = function (grid, hiddenFormat, format) {
            hiddenFormat.setValue(format);
            grid.submitData(false);
        };
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Window ID="WdwEstacion" runat="server" Title="Estación" Hidden="true" Width="400" Height="270"
        Modal="true" Resizable="false" Closable="true">
        <Items>
            <ext:FormPanel ID="FormPanelEstacion" runat="server" Padding="10" MonitorValid="true" LabelAlign="Left">
                <Items>
                    <ext:TextField ID="txtEstacion" runat="server" FieldLabel="Estación" ReadOnly="true"
                        AnchorHorizontal="100%"/>
                    <ext:TextField ID="txtNombre" runat="server" FieldLabel="Nombre" ReadOnly="true"
                        AnchorHorizontal="100%" />
                    <ext:TextField ID="txtDireccion" runat="server" FieldLabel="Dirección" ReadOnly="true"
                        AnchorHorizontal="100%" />
                    <ext:TextField ID="txtColonia" runat="server" FieldLabel="Colonia" ReadOnly="true"
                        AnchorHorizontal="100%" />
                    <ext:TextField ID="txtCodigoPostal" runat="server" FieldLabel="Código Postal" ReadOnly="true"
                        AnchorHorizontal="100%" />
                    <ext:TextField ID="txtCiudad" runat="server" FieldLabel="Ciudad" ReadOnly="true"
                        AnchorHorizontal="100%" />
                    <ext:TextField ID="txtEstado" runat="server" FieldLabel="Estado" ReadOnly="true"
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

    <ext:BorderLayout ID="BorderLayoutOperaciones" runat="server">
        <North Split="true">
            <ext:FormPanel ID="FormPanelBuscarOperaciones" runat="server" LabelWidth="70" LabelAlign="Right">
                <TopBar>
                    <ext:Toolbar ID="ToolbarOper" runat="server">
                        <Items>
                            <ext:Hidden ID="FormatType" runat="server" />
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
                            <ext:TextField ID="txtTarjeta" runat="server" EmptyText="Tarjeta" AllowBlank="false"
                                Width="160" MaxLength="16" />
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
                        <DirectEventConfig IsUpload="true" />
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
                                    <ext:RecordField Name="Estacion" />
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
                        <ext:Column DataIndex="Estacion" Header="Estación" Width="60">
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
                            <ext:Label ID="lblTituloTarjeta" runat="server" StyleSpec="color:blue;font-weight:bold;"/>
                            <ext:ToolbarFill ID="ToolbarFill2" runat="server" />
                           <%-- <ext:Button ID="Button3" runat="server" Text="Exportar a Excel" Icon="PageExcel">
                                <Listeners>
                                    <Click Handler="submitValue(#{GridResultadosOper}, #{FormatType}, 'xls');" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="Button4" runat="server" Text="Exportar a CSV" Icon="PageAttach">
                                <Listeners>
                                    <Click Handler="submitValue(#{GridResultadosOper}, #{FormatType}, 'csv');" />
                                </Listeners>
                            </ext:Button>--%>
                            <ext:Button ID="Button3" runat="server" Text="Exportar a Excel" Icon="PageExcel">
                                <DirectEvents>
                                    <Click OnEvent="SubmitGrid" IsUpload="true" Success="Ext.Net.DirectMethods.Download({IsUpload:true})">
                                        <ExtraParams>
                                            <ext:Parameter Name="GridOper" Value="Ext.encode(#{GridResultadosOper}.getRowsValues({selectedOnly : false}))" Mode="Raw" />
                                        </ExtraParams>
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
            </ext:GridPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>