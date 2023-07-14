<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" ValidateRequest="false"
    CodeBehind="Rep_SaldoEnMedioAccesoCuentas.aspx.cs" Inherits="Empresarial.Rep_SaldoEnMedioAccesoCuentas" %>

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

        var template = '<span style="color:{0};">{1}</span>';

        var change = function (value) {
            return String.format(template, (value > 0) ? "green" : "red", value);
        };

        var pctChange = function (value) {
            return String.format(template, (value > 0) ? "green" : "red", value + "%");
        };


    </script>
    <script type="text/javascript">
        function setFields(grid, hiddenFormat, hiddenColConfig, format) {
            hiddenFormat.setValue(format);         // Calculate hidden columns.       
            var hidden = '<columns>';
            var i;
            var visible;
            for (i = 0; i < grid.colModel.columns.length; i++) {
                hidden = hidden + '<column ';
                hidden = hidden + 'dataField="' + grid.colModel.columns[i].dataIndex + '"';
                visible = grid.colModel.columns[i].hidden == undefined ? true : !grid.colModel.columns[i].hidden;
                hidden = hidden + ' visible="' + visible.toString() + '"/>';
                hidden = hidden + ' index="' + i.toString() + '"/>';
            }
            hidden = hidden + '</columns>';
            hiddenColConfig.setValue(hidden);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Content>
            <ext:Hidden ID="FormatType" runat="server" />
            <ext:Hidden ID="ColConfig" runat="server" />
        </Content>
        <Center Split="true">
            <ext:Panel ID="pnlcuentas" runat="server" Width="850" Title="Saldos" Collapsed="false"
                Collapsible="false" Layout="Fit" AutoScroll="true">
                <Items>
                </Items>
                <Content>
                    <ext:Toolbar runat="server">
                        <Items>
                        </Items>
                    </ext:Toolbar>
                    <ext:Store ID="Store1" runat="server" OnRefreshData="RefreshGridSaldos" OnSubmitData="Store1_Submit"
                        GroupField="ClaveMA">
                        <%--GroupField="ctaHabienteOriginal">--%>
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Cuenta">
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store1" StripeRows="true"
                                Header="false" Border="false" AutoScroll="true">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                    </Columns>
                                </ColumnModel>
                                <DirectEvents>
                                    <Command OnEvent="Seleccionar">
                                        <ExtraParams>
                                            <ext:Parameter Name="ID_Cuenta" Value="record.data.ID_Cuenta" Mode="Raw"/>
                                        </ExtraParams>
                                    </Command>
                                </DirectEvents>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel2" runat="server" SingleSelect="true">
                                        <DirectEvents>
                                            <RowDeselect OnEvent="QuitarSeleccion" IsUpload="true">
                                            </RowDeselect>
                                        </DirectEvents>
                                    </ext:RowSelectionModel>
                                </SelectionModel>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="Store1" DisplayInfo="true"
                                        DisplayMsg="Mostrando Saldos {0} - {1} de {2}" />
                                </BottomBar>
                                <View>
                                    <ext:GroupingView ID="GroupingView1" runat="server" ForceFit="true" MarkDirty="false"
                                        ShowGroupName="false" EnableNoGroups="true" HideGroupedColumn="true" />
                                </View>
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar2" runat="server">
                                        <Items>
                                            <ext:TextField Name="Tarjeta" TabIndex="5" EmptyText="Número de Tarjeta" MaxLength="16"
                                                ID="txtTarjeta" runat="server" Width="180" Text="" />
                                            <ext:TextField Name="Nombre" TabIndex="5" EmptyText="Nombre y Apellidos" MaxLength="100"
                                                ID="txtNombre" runat="server" Width="180" Text="" />
                                            <ext:Button ID="btnGuardar" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnBuscar_Click">
                                                        <EventMask ShowMask="true" Msg="Buscando Cuentas..." MinDelay="500" />
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:ToolbarFill runat="server" />
                                            <%--<ext:Button ID="btnGetEdocta" runat="server" Text="Obtener PDF" ToolTip="Descargar reporte en PDF"
                                                Icon="PageWhiteAcrobat">
                                                <DirectEvents>
                                                    <Click OnEvent="GetEstadoCuenta" IsUpload="true">
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>--%>
                                            <ext:Button ID="btnExportExcel" runat="server" Text="Obtener Excel" Icon="PageExcel" ToolTip="Obtener Datos en un Archivo Excel">
                                                <DirectEvents>
                                                    <Click OnEvent="ExportGridToExcel" IsUpload="true" Success="Ext.Net.DirectMethods.Download({IsUpload:true})">
                                                        <ExtraParams>
                                                            <ext:Parameter Name="GridToExport" Value="Ext.encode(#{gridPanel1}.getRowsValues({selectedOnly : false}))" Mode="Raw" />
                                                        </ExtraParams>
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                            <%--<ext:Button runat="server" ID="btnCSV" Text="Obtener CSV" Icon="PageAttach" AutoPostBack="true">
                                                <Listeners>
                                                    <Click Handler="setFields(#{gridPanel1}, #{FormatType}, #{ColConfig}, 'CSV');" />
                                                </Listeners>
                                            </ext:Button>
                                            <ext:Button runat="server" ID="btnXml" Text="Obtener XML" Icon="PageCode" AutoPostBack="true">
                                                <Listeners>
                                                    <Click Handler="setFields(#{gridPanel1}, #{FormatType}, #{ColConfig}, 'XML');" />
                                                </Listeners>
                                            </ext:Button>--%>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </Center>
        <East Split="true">
            <ext:Panel ID="pnlDetalles" runat="server" Width="550" Title="Detalles" Collapsed="true"
                Collapsible="true" Layout="Fit" AutoScroll="true">
                <Content>
                    <ext:BorderLayout ID="BorderLayout3" runat="server">
                        <North>
                            <ext:Panel ID="Panel12" runat="server" Width="450" Height="110" Title="Filtro de movimientos"
                                Collapsed="false" Collapsible="false" Layout="Fit" AutoScroll="true" Padding="5">
                                <Items>
                                    <ext:DateField ID="datInicio" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                        AllowBlank="false" MsgTarget="Side" Format="yyyy-MM-dd" TabIndex="1" MaxLength="12"
                                        Width="300" EnableKeyEvents="true" MaxWidth="300">
                                        <CustomConfig>
                                            <ext:ConfigItem Name="endDateField" Value="#{datFinal}" Mode="Value" />
                                        </CustomConfig>
                                        <Listeners>
                                            <KeyUp Fn="onKeyUp" />
                                        </Listeners>
                                    </ext:DateField>
                                    <ext:DateField ID="datFinal" runat="server" Vtype="daterange" FieldLabel="Fecha Final"
                                        AllowBlank="false" MaxLength="12" Width="300" MsgTarget="Side" Format="yyyy-MM-dd"
                                        TabIndex="2" EnableKeyEvents="true">
                                        <CustomConfig>
                                            <ext:ConfigItem Name="startDateField" Value="#{datInicio}" Mode="Value" />
                                        </CustomConfig>
                                        <Listeners>
                                            <KeyUp Fn="onKeyUp" />
                                        </Listeners>
                                    </ext:DateField>
                                </Items>
                                <BottomBar>
                                    <ext:Toolbar ID="Toolbar1" runat="server">
                                        <Items>
                                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." ToolTip="Buscar Detalles del Periodo"
                                                Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="ObtenerDatos">
                                                        <EventMask ShowMask="true" Msg="Buscando Detalles..." MinDelay="500" />
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:ToolbarFill runat="server" />
                                           <%-- <ext:Button ID="Button1" runat="server" Text="Reporte PDF" ToolTip="Obtener Reporte en PDF"
                                                Icon="PageWhiteAcrobat">
                                                <DirectEvents>
                                                    <Click OnEvent="ExportarPDF" IsUpload="true">
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>--%>
                                            <ext:Button ID="Button2" runat="server" Text="Exportar a Excel" ToolTip="Obtener Datos en un Archivo Excel"
                                                Icon="PageExcel">
                                                <Listeners>
                                                    <Click Handler="submitValue(#{GridPanel2}, #{FormatType}, 'xls');" />
                                                </Listeners>
                                            </ext:Button>
                                            <ext:Button ID="Button3" runat="server" Text="Exportar a CSV" ToolTip="Obtener Datos separados por comas"
                                                Icon="PageAttach">
                                                <Listeners>
                                                    <Click Handler="submitValue(#{GridPanel2}, #{FormatType}, 'csv');" />
                                                </Listeners>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </BottomBar>
                            </ext:Panel>
                        </North>
                        <Center Split="true">
                            <ext:Panel ID="Panel1" runat="server" Width="450" Title="Cuentas" Collapsed="false"
                                Collapsible="false" Layout="Fit" AutoScroll="true">
                                <Content>
                                    <ext:Store ID="Store2" runat="server" OnSubmitData="Store1_Submit" RemoteSort="true">
                                        <DirectEventConfig IsUpload="true" />
                                        <Reader>
                                            <ext:JsonReader IDProperty="IDReporte">
                                            </ext:JsonReader>
                                        </Reader>
                                        <DirectEventConfig IsUpload="true" />
                                        <SortInfo Field="ID_Poliza" />
                                    </ext:Store>
                                    <ext:BorderLayout ID="BorderLayout4" runat="server">
                                        <Center Split="true">
                                            <ext:GridPanel ID="GridPanel2" runat="server" StoreID="Store2" StripeRows="true"
                                                Header="false" Border="false">
                                                <LoadMask ShowMask="false" />
                                                <ColumnModel ID="ColumnModel2" runat="server">
                                                </ColumnModel>
                                                <BottomBar>
                                                    <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="Store2" DisplayInfo="true"
                                                        DisplayMsg="Mostrando Saldos {0} - {1} de {2}" />
                                                </BottomBar>
                                            </ext:GridPanel>
                                        </Center>
                                    </ext:BorderLayout>
                                </Content>
                            </ext:Panel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </East>
    </ext:BorderLayout>
</asp:Content>
