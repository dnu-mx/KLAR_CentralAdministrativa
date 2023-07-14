<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Reporte_OperacionesCadConcentradoNuevo.aspx.cs" Inherits="TpvWeb.Reporte_OperacionesCadConcentradoNuevo" %>

<%@ Import Namespace="System.Xml.Xsl" %>
<%@ Import Namespace="System.Xml" %>
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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true" Collapsible="true">
            <ext:FormPanel ID="FormPanel1" Width="350" Title="Selecciona los Filtros" runat="server"
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
                    <ext:Store ID="StoreBeneficiario" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                                <Fields>
                                    <ext:RecordField Name="NombreORazonSocial" />
                                    <ext:RecordField Name="ClaveColectiva" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="NombreORazonSocial" Direction="ASC" />
                    </ext:Store>
                    <ext:Store ID="StoreEstatus" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_EstatusOperacion">
                                <Fields>
                                    <ext:RecordField Name="ID_EstatusOperacion" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="Descripcion" Direction="ASC" />
                    </ext:Store>
                </Content>
                <Items>
                    <ext:Panel ID="Panel1" runat="server" Layout="FitLayout">
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
                            <ext:ComboBox ID="cmbCadenaComercial" TabIndex="3" FieldLabel="Cadena Comercial"
                                EmptyText="Todas" Resizable="true" ListWidth="350" Width="300" runat="server"
                                StoreID="StoreCadenaComercial" DisplayField="NombreORazonSocial" ValueField="ID_Colectiva">
                                <DirectEvents>
                                    <Select OnEvent="LlenaSucursales">
                                    </Select>
                                </DirectEvents>
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cmbBeneficiario" FieldLabel="Marca" TabIndex="9" EmptyText="Todos"
                                Resizable="true" ListWidth="350" Width="300" runat="server" StoreID="StoreBeneficiario"
                                DisplayField="NombreORazonSocial" ValueField="ClaveColectiva">
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
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
                            <ext:Button ID="btnBuscar" runat="server" Text="Consultar" Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Before="var valid= #{FormPanel1}.getForm().isValid(); if (!valid) {} return valid;" />
                                </DirectEvents>
                                <Listeners>
                                    <Click Handler="#{Pages}.addTab(#{Panel2}, false);" />
                                </Listeners>
                            </ext:Button>
                           <%-- <ext:Button ID="Button8" runat="server" Text="Agrupado por Periodo" Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar2_Click" Before="var valid= #{FormPanel1}.getForm().isValid(); if (!valid) {} return valid;" />
                                     
                                </DirectEvents>
                                <Listeners>
                                    <Click Handler="#{Pages}.addTab(#{Panel3}, false);" />
                                </Listeners>
                            </ext:Button>--%>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>
            </ext:FormPanel>
        </West>
        <Center Split="true" Collapsible="false">
            <ext:TabPanel ID="Pages" runat="server" Border="false" EnableTabScroll="false">
                <Items>
                    <ext:Panel ID="Panel2" runat="server" Title="Agrupado por Día" Collapsed="false" 
                        Layout="Fit" AutoScroll="true">
                        <Content>
                            <ext:Store ID="Store1" runat="server" OnSubmitData="Store1_Submit" OnRefreshData="btnBuscar_Click"
                                GroupField="ClaveCadena" RemoteSort="true">
                                <DirectEventConfig IsUpload="true" />
                                <Reader>
                                    <ext:JsonReader>
                                        <%--<ext:ArrayReader IDProperty="ID_Operacion">--%>
                                        <Fields>
                                            <ext:RecordField Name="Fecha" Type="Date" />
                                            <ext:RecordField Name="Beneficiario" />
                                            <ext:RecordField Name="Monto" />
                                            <ext:RecordField Name="ClaveCadena" />
                                            <ext:RecordField Name="TotalOperaciones" />
                                            <%--<ext:RecordField Name="Estatus" />--%>
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
                                        Header="false" Border="false" AutoExpandColumn="ClaveCadena">
                                        <LoadMask ShowMask="false" />
                                        <ColumnModel ID="ColumnModel1" runat="server">
                                            <Columns>
                                                <ext:DateColumn ColumnID="colFecha" Header="Fecha" Sortable="true" DataIndex="Fecha"
                                                    Format="yyyy-MM-dd" />
                                                <ext:Column ColumnID="ClaveCadena" Header="Cadena Comercial" Sortable="true" DataIndex="ClaveCadena" />
                                                <ext:Column ColumnID="Beneficiario" Header="Marca" Sortable="true" DataIndex="Beneficiario" />
                                                <ext:Column ColumnID="TotalOperaciones" Header="Num. Trx." Sortable="true" Align="Right" DataIndex="TotalOperaciones">
                                                </ext:Column>
                                                <ext:Column ColumnID="Monto" Header="Monto" Sortable="true" DataIndex="Monto"  Align="Right">
                                                    <Renderer Format="UsMoney" />
                                                </ext:Column>
                                            </Columns>
                                        </ColumnModel>
                                        <Plugins>
                                            <ext:GridFilters runat="server" ID="GridFilters1" Local="true">
                                                <Filters>
                                                    <ext:StringFilter DataIndex="Beneficiario" />
                                                    <ext:NumericFilter DataIndex="Monto" />
                                                    <ext:DateFilter DataIndex="Fecha">
                                                        <DatePickerOptions runat="server" TodayText="Hoy" />
                                                    </ext:DateFilter>
                                                </Filters>
                                            </ext:GridFilters>
                                        </Plugins>
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
                                                    <ext:Button ID="Button3" runat="server" Text="Exportar a Excel" Icon="PageExcel">
                                                        <Listeners>
                                                            <Click Handler="submitValue(#{GridPanel1}, #{FormatType}, 'xls');" />
                                                        </Listeners>
                                                    </ext:Button>
                                                    <ext:Button ID="Button4" runat="server" Text="Exportar a CSV" Icon="PageAttach">
                                                        <Listeners>
                                                            <Click Handler="submitValue(#{GridPanel1}, #{FormatType}, 'csv');" />
                                                        </Listeners>
                                                    </ext:Button>
                                                </Items>
                                            </ext:Toolbar>
                                        </TopBar>
                                        <View>
                                            <ext:GroupingView ID="GroupingView1" runat="server" ForceFit="true" MarkDirty="false"
                                                ShowGroupName="false" EnableNoGroups="true" HideGroupedColumn="true" />
                                        </View>
                                    </ext:GridPanel>
                                </Center>
                            </ext:BorderLayout>
                        </Content>
                    </ext:Panel>
                    <ext:Panel ID="Panel3" runat="server" Title="Agrupado Cadena por Periodo" Collapsed="false"
                        Layout="Fit" AutoScroll="true">
                        <Content>
                            <ext:Store ID="Store2" runat="server" OnSubmitData="Store2_Submit" OnRefreshData="btnBuscar_Click"
                                GroupField="ClaveCadena" RemoteSort="true" >
                                <DirectEventConfig IsUpload="true" />
                                <Reader>
                                    <ext:JsonReader IDProperty="ID" >
                                        <%--<ext:ArrayReader IDProperty="ID_Operacion">--%>
                                        <Fields>
                                        <ext:RecordField Name="ID" />
                                            <ext:RecordField Name="Fecha" Type="Date" />
                                            <ext:RecordField Name="Beneficiario" />
                                            <ext:RecordField Name="Monto" />
                                            <ext:RecordField Name="ClaveCadena" />
                                            <ext:RecordField Name="TotalOperaciones" />
                                            <%--<ext:RecordField Name="Estatus" />--%>
                                        </Fields>
                                        <%--</ext:ArrayReader>--%>
                                    </ext:JsonReader>
                                </Reader>
                                <DirectEventConfig IsUpload="true" />
                                <SortInfo Field="Fecha" />
                            </ext:Store>
                            <ext:BorderLayout ID="BorderLayout3" runat="server">
                                <Center Split="true">
                                    <ext:GridPanel ID="GridPanel2" runat="server" StoreID="Store2" StripeRows="true"
                                        Header="false" Border="false" AutoExpandColumn="ClaveCadena">
                                        <LoadMask ShowMask="false" />
                                        <ColumnModel ID="ColumnModel2" runat="server">
                                            <Columns>
                                                <%--    <ext:DateColumn ColumnID="colFecha" Header="Fecha" Sortable="true" DataIndex="Fecha"
                                                    Format="yyyy-MM-dd" />--%>
                                                <ext:Column ColumnID="ClaveCadena" Header="Cadena Comercial" Sortable="true" DataIndex="ClaveCadena" />
                                                <ext:Column ColumnID="Beneficiario" Header="Marca" Sortable="true" DataIndex="Beneficiario" />
                                                <ext:Column ColumnID="TotalOperaciones" Header="Num. Trx."  Align="Right" Sortable="true" DataIndex="TotalOperaciones">
                                                </ext:Column>
                                                <ext:Column ColumnID="Monto" Header="Monto" Sortable="true" DataIndex="Monto"  Align="Right">
                                                    <Renderer Format="UsMoney" />
                                                </ext:Column>
                                            </Columns>
                                        </ColumnModel>
                                        <Plugins>
                                            <ext:GridFilters runat="server" ID="GridFilters2" Local="true">
                                                <Filters>
                                                    <ext:StringFilter DataIndex="Beneficiario" />
                                                    <ext:NumericFilter DataIndex="Monto" />
                                                </Filters>
                                            </ext:GridFilters>
                                        </Plugins>
                                        <BottomBar>
                                            <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="Store2" DisplayInfo="true"
                                                DisplayMsg="Mostrando Resultados {0} - {1} de {2}" />
                                        </BottomBar>
                                        <TopBar>
                                            <ext:Toolbar ID="Toolbar3" runat="server">
                                                <Items>
                                                    <ext:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                    <ext:Button ID="Button5" runat="server" Text="Exportar a XML" Icon="PageCode">
                                                        <Listeners>
                                                            <Click Handler="submitValue(#{GridPanel2}, #{FormatType}, 'xml');" />
                                                        </Listeners>
                                                    </ext:Button>
                                                    <ext:Button ID="Button6" runat="server" Text="Exportar a Excel" Icon="PageExcel">
                                                        <Listeners>
                                                            <Click Handler="submitValue(#{GridPanel2}, #{FormatType}, 'xls');" />
                                                        </Listeners>
                                                    </ext:Button>
                                                    <ext:Button ID="Button7" runat="server" Text="Exportar a CSV" Icon="PageAttach">
                                                        <Listeners>
                                                            <Click Handler="submitValue(#{GridPanel2}, #{FormatType}, 'csv');" />
                                                        </Listeners>
                                                    </ext:Button>
                                                </Items>
                                            </ext:Toolbar>
                                        </TopBar>
                                        <View>
                                            <ext:GroupingView ID="GroupingView2" runat="server" ForceFit="true" MarkDirty="false"
                                                ShowGroupName="false" EnableNoGroups="true" HideGroupedColumn="true" />
                                        </View>
                                    </ext:GridPanel>
                                </Center>
                            </ext:BorderLayout>
                        </Content>
                    </ext:Panel>
                      <ext:Panel ID="Panel4" runat="server" Title="Agrupado Grupo Comercial por Periodo" Collapsed="false"
                        Layout="Fit" AutoScroll="true">
                        <Content>
                            <ext:Store ID="storeGrupo" runat="server" OnSubmitData="Store2_Submit" OnRefreshData="btnBuscar_Click"
                                GroupField="GrupoComercial" RemoteSort="true" >
                                <DirectEventConfig IsUpload="true" />
                                <Reader>
                                    <ext:JsonReader IDProperty="ID" >
                                        <%--<ext:ArrayReader IDProperty="ID_Operacion">--%>
                                        <Fields>
                                        <ext:RecordField Name="ID" />
                                            <ext:RecordField Name="Fecha" Type="Date" />
                                            <ext:RecordField Name="Beneficiario" />
                                            <ext:RecordField Name="Monto" />
                                            <ext:RecordField Name="GrupoComercial" />
                                            <ext:RecordField Name="TotalOperaciones" />
                                            <%--<ext:RecordField Name="Estatus" />--%>
                                        </Fields>
                                        <%--</ext:ArrayReader>--%>
                                    </ext:JsonReader>
                                </Reader>
                                <DirectEventConfig IsUpload="true" />
                                <SortInfo Field="Fecha" />
                            </ext:Store>
                            <ext:BorderLayout ID="BorderLayout4" runat="server">
                                <Center Split="true">
                                    <ext:GridPanel ID="GridPanel3" runat="server" StoreID="storeGrupo" StripeRows="true"
                                        Header="false" Border="false" AutoExpandColumn="GrupoComercial">
                                        <LoadMask ShowMask="false" />
                                        <ColumnModel ID="ColumnModel3" runat="server">
                                            <Columns>
                                                <ext:Column ColumnID="GrupoComercial" Header="Grupo Comercial" Sortable="true" DataIndex="GrupoComercial" />
                                                <ext:Column ColumnID="Beneficiario" Header="Marca" Sortable="true" DataIndex="Beneficiario" />
                                                <ext:Column ColumnID="TotalOperaciones" Header="Num. Trx."  Align="Right" Sortable="true" DataIndex="TotalOperaciones">
                                                </ext:Column>
                                                <ext:Column ColumnID="Monto" Header="Monto" Sortable="true" DataIndex="Monto"  Align="Right">
                                                    <Renderer Format="UsMoney" />
                                                </ext:Column>
                                            </Columns>
                                        </ColumnModel>
                                        <Plugins>
                                            <ext:GridFilters runat="server" ID="GridFilters3" Local="true">
                                                <Filters>
                                                    <ext:StringFilter DataIndex="Beneficiario" />
                                                    <ext:NumericFilter DataIndex="Monto" />
                                                </Filters>
                                            </ext:GridFilters>
                                        </Plugins>
                                        <BottomBar>
                                            <ext:PagingToolbar ID="PagingToolBar3" runat="server" StoreID="Store2" DisplayInfo="true"
                                                DisplayMsg="Mostrando Resultados {0} - {1} de {2}" />
                                        </BottomBar>
                                        <TopBar>
                                            <ext:Toolbar ID="Toolbar4" runat="server">
                                                <Items>
                                                    <ext:ToolbarFill ID="ToolbarFill3" runat="server" />
                                                    <ext:Button ID="Button8" runat="server" Text="Exportar a XML" Icon="PageCode">
                                                        <Listeners>
                                                            <Click Handler="submitValue(#{GridPanel3}, #{FormatType}, 'xml');" />
                                                        </Listeners>
                                                    </ext:Button>
                                                    <ext:Button ID="Button9" runat="server" Text="Exportar a Excel" Icon="PageExcel">
                                                        <Listeners>
                                                            <Click Handler="submitValue(#{GridPanel3}, #{FormatType}, 'xls');" />
                                                        </Listeners>
                                                    </ext:Button>
                                                    <ext:Button ID="Button10" runat="server" Text="Exportar a CSV" Icon="PageAttach">
                                                        <Listeners>
                                                            <Click Handler="submitValue(#{GridPanel3}, #{FormatType}, 'csv');" />
                                                        </Listeners>
                                                    </ext:Button>
                                                </Items>
                                            </ext:Toolbar>
                                        </TopBar>
                                        <View>
                                            <ext:GroupingView ID="GroupingView3" runat="server" ForceFit="true" MarkDirty="false"
                                                ShowGroupName="false" EnableNoGroups="true" HideGroupedColumn="true" />
                                        </View>
                                    </ext:GridPanel>
                                </Center>
                            </ext:BorderLayout>
                        </Content>
                    </ext:Panel>
                
                </Items>
            </ext:TabPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
