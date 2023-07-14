<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Reporte_EventosEjecutados.aspx.cs" Inherits="Facturas.Reporte_EventosEjecutados" %>

<%@ Import Namespace="System.Xml.Xsl" %>
<%@ Import Namespace="System.Xml" %>
<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
<script type="text/javascript">
    // this "setGroupStyle" function is called when the GroupingView is refreshed.     
    var setGroupStyle = function (view) {
        // get an instance of the Groups
        var groups = view.getGroups();

        for (var i = 0; i < groups.length; i++) {
            var spans = Ext.query("span", groups[i]);

            if (spans && spans.length > 0) {
                // Loop through the Groups, the do a query to find the <span> with our ColorCode
                // Get the "id" from the <span> and split on the "-", the second array item should be our ColorCode
                var color = "#" + spans[0].id.split("-")[1];

                // Set the "background-color" of the original Group node.
                Ext.get(groups[i]).setStyle("background-color", color);
            }
        }
    };
    </script>    
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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true" Collapsible="true">
            <ext:FormPanel ID="FormPanel1" Width="320" Title="Selecciona los Filtros" runat="server"
                Border="false" Layout="Fit">
                <Content>
                    <ext:Hidden ID="FormatType" runat="server" />
                    <ext:Store ID="StoreCadenaComercial" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                                <Fields>
                                    <ext:RecordField Name="NombreORazonSocial" />
                                    <ext:RecordField Name="ClaveColectiva" />
                                    <ext:RecordField Name="ID_Colectiva" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="NombreORazonSocial" Direction="ASC" />
                    </ext:Store>
                </Content>
                <Items>
                    <ext:Panel ID="Panel1" runat="server" Layout="FitLayout" Padding="10">
                        <Items>
                            <ext:ComboBox ID="cmbCadenaComercial" FieldLabel="Cadena Comercial" EmptyText="Todas"
                                ListWidth="300" Width="280" runat="server" StoreID="StoreCadenaComercial"
                                DisplayField="NombreORazonSocial" ValueField="ID_Colectiva" Mode="Local"
                                AutoSelect="true" ForceSelection="true" TypeAhead="true" MinChars="1"
                                MatchFieldWidth="false" Name="colCCR_DE">
                                <Items>
                                    <ext:ListItem Text="( Todas )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:DateField ID="datInicio" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                AllowBlank="false" MsgTarget="Side" Format="yyyy-MM-dd" MaxLength="12"
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
                                EnableKeyEvents="true">
                                <CustomConfig>
                                    <ext:ConfigItem Name="startDateField" Value="#{datInicio}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:TextField ID="txtImporteInferior" FieldLabel="Monto Inferior" EmptyText="Todos"
                                MaxLength="50" Width="300" runat="server" Text="" />
                            <ext:TextField ID="txtImporteSuperior" FieldLabel="Monto Superior" EmptyText="Todos"
                                MaxLength="50" Width="300" runat="server" Text="" />
                        </Items>
                    </ext:Panel>
                </Items>
                <FooterBar>
                    <ext:Toolbar ID="Toolbar1" runat="server">
                        <Items>
                            <ext:Button ID="Button1" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Before="var valid= #{FormPanel1}.getForm().isValid(); if (!valid) {} return valid;">
                                        <EventMask ShowMask="true" Msg="Buscando Pólizas..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>
            </ext:FormPanel>
        </West>
        <Center Split="true" Collapsible="false">
            <ext:Panel ID="Panel2" runat="server" Title="Operaciones Obtenidas con el Filtro Seleccionado"
                Collapsed="false" Layout="Fit" AutoScroll="true">
                <Content>
                    <ext:Store ID="Store1" runat="server" OnSubmitData="Store1_Submit" OnRefreshData="btnBuscar_Click"
                        RemoteSort="true">
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Operacion">
                                <%--<ext:ArrayReader IDProperty="ID_Operacion">--%>
                                <Fields>
                                    <ext:RecordField Name="Fecha" Type="Date" />
                                    <ext:RecordField Name="ID_Poliza" />
                                    <ext:RecordField Name="ID_Colectiva" />
                                    <ext:RecordField Name="NombreORazonSocial" />
                                    <ext:RecordField Name="ID_Cuenta" />
                                    <ext:RecordField Name="Cuenta" />
                                    <ext:RecordField Name="Cargo" />
                                    <ext:RecordField Name="Abono" />
                                    <ext:RecordField Name="Referencia" />
                                    <ext:RecordField Name="Observaciones" />
                                    <ext:RecordField Name="Concepto" />
                                </Fields>
                                <%--</ext:ArrayReader>--%>
                            </ext:JsonReader>
                        </Reader>
                        <DirectEventConfig IsUpload="true" />
                        <SortInfo Field="Fecha" />
                    </ext:Store>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store1" StripeRows="true"
                                Header="false" Border="false">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                        <ext:DateColumn ColumnID="colFecha" Header="Fecha" Sortable="true" DataIndex="Fecha"
                                            Format="yyyy-MM-dd" />
                                        <ext:DateColumn ColumnID="colhora" Header="Hora" Sortable="true" DataIndex="Fecha"
                                            Format="HH:mm:ss" />
                                        <ext:Column ColumnID="ID_Poliza" Header="Poliza" Sortable="true" DataIndex="ID_Poliza" />
                                        <%--<ext:Column ColumnID="ID_Colectiva" Header="Cadena Comercial" Sortable="true" DataIndex="ID_Colectiva" />--%>
                                        <%--<ext:Column ColumnID="NombreORazonSocial" Header="Colectiva" Sortable="true" DataIndex="NombreORazonSocial" />--%>
                                        <%--<ext:Column ColumnID="Concepto" Width="300" Header="Concepto de Poliza" Sortable="true"
                                            DataIndex="Concepto" />--%>
                                        <ext:Column ColumnID="Cuenta" Header="Cuenta" Sortable="true" DataIndex="Cuenta" />
                                        <ext:Column ColumnID="Cargo" Header="Cargos" Sortable="true" DataIndex="Cargo">
                                            <Renderer Format="UsMoney" />
                                        </ext:Column>
                                        <ext:Column ColumnID="Abono" Header="Abono" Sortable="true" DataIndex="Abono">
                                            <Renderer Format="UsMoney" />
                                        </ext:Column>
                                        <ext:Column ColumnID="Referencia" Header="Referencia" Sortable="true" DataIndex="Referencia" />
                                        <%--<ext:Column ColumnID="Observaciones" Header="Observaciones" Sortable="true" DataIndex="Observaciones" />--%>
                                    </Columns>
                                </ColumnModel>
                                <Plugins>
                                    <ext:GridFilters runat="server" ID="GridFilters1" Local="true">
                                        <Filters>
                                            <ext:NumericFilter DataIndex="ID_Operacion" />
                                            <ext:StringFilter DataIndex="Sucursal" />
                                            <ext:StringFilter DataIndex="Afiliacion" />
                                            <ext:StringFilter DataIndex="Terminal" />
                                            <ext:StringFilter DataIndex="Operador" />
                                            <ext:StringFilter DataIndex="Autorizacion" />
                                            <ext:StringFilter DataIndex="Beneficiario" />
                                            <ext:StringFilter DataIndex="Telefono" />
                                            <ext:NumericFilter DataIndex="Monto" />
                                            <ext:DateFilter DataIndex="Fecha">
                                                <DatePickerOptions runat="server" TodayText="Hoy" />
                                            </ext:DateFilter>
                                        </Filters>
                                    </ext:GridFilters>
                                </Plugins>
                                <View>
                                    <ext:GroupingView ID="GroupingView1" HideGroupedColumn="true" runat="server" ForceFit="true"
                                        StartCollapsed="true" GroupTextTpl='<span id="ColorCode-{[values.rs[0].data.ColorCode]}"></span>{text} ({[values.rs.length]} {[values.rs.length > 1 ? "Items" : "Item"]})'
                                        EnableRowBody="true">
                                        <Listeners>
                                            <Refresh Fn="setGroupStyle" />
                                        </Listeners>
                                        <GetRowClass Handler="var d = record.data; rowParams.body = String.format('<div style=\'padding:0 5px 5px 5px;\'><b></b> [{3}], <b>{2}</b> <b> <br/>Concepto:</b>{0}. <br/><b>Observaciones:</b>{1}<br /></div>', d.Concepto, d.Observaciones, d.NombreORazonSocial, d.Cuenta);" />
                                    </ext:GroupingView>
                                </View>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="Store1" DisplayInfo="true"
                                        DisplayMsg="Mostrando Operaciones {0} - {1} de {2}" />
                                </BottomBar>
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar2" runat="server">
                                        <Items>
                                            <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                                            <ext:Button ID="Button2" runat="server" Text="Exportar a XML" Icon="PageCode">
                                                <Listeners>
                                                    <Click Handler="submitValue(#{GridPanel1}, #{FormatType}, 'xml');" />
                                                </Listeners>
                                            </ext:Button>
                                            <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" Icon="PageExcel">
                                                <DirectEvents>
                                                    <Click OnEvent="ExportGridToExcel" IsUpload="true" Success="Ext.Net.DirectMethods.Download({IsUpload:true})">
                                                        <ExtraParams>
                                                            <ext:Parameter Name="GridToExport" Value="Ext.encode(#{GridPanel1}.getRowsValues({selectedOnly : false}))" Mode="Raw" />
                                                        </ExtraParams>
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="Button4" runat="server" Text="Exportar a CSV" Icon="PageAttach">
                                                <Listeners>
                                                    <Click Handler="submitValue(#{GridPanel1}, #{FormatType}, 'csv');" />
                                                </Listeners>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>

