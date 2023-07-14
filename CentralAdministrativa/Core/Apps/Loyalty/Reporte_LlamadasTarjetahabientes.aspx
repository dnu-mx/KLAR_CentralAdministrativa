<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Reporte_LlamadasTarjetahabientes.aspx.cs" Inherits="CentroContacto.Reporte_LlamadasTarjetahabientes" %>

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
            <ext:FormPanel ID="FormPanelFiltros" Width="320" Title="Selecciona los Filtros" runat="server" Padding="10"
                Border="false" Layout="FitLayout" LabelWidth="100">
                <Items>
                    <ext:ComboBox ID="cBoxGpoComercial" runat="server" FieldLabel="SubEmisor <span style='color:red;'>*   </span>"
                        Width="300" DisplayField="NombreORazonSocial" ValueField="ID_Colectiva" Mode="Local" AutoSelect="true"
                        ForceSelection="true" TypeAhead="true" MinChars="1" MatchFieldWidth="false" AllowBlank="false" ListWidth="350">
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
                    <ext:DateField ID="dfFechaInicio" runat="server" Vtype="daterange" Width="300" Format="yyyy/MM/dd"
                        FieldLabel="Fecha Inicial <span style='color:red;'>*   </span>" Editable="false" AllowBlank="false">
                        <CustomConfig>
                            <ext:ConfigItem Name="endDateField" Value="#{dfFechaFin}" Mode="Value" />
                        </CustomConfig>
                        <Listeners>
                            <KeyUp Fn="onKeyUp" />
                            <Change Handler="if (this.getValue() == '') { #{tfHoraInicio}.clear(); }" />
                        </Listeners>
                    </ext:DateField>
                    <ext:DateField ID="dfFechaFin" runat="server" Vtype="daterange" Width="300" Format="yyyy/MM/dd"
                        FieldLabel="Fecha Final <span style='color:red;'>*   </span>" Editable="false" AllowBlank="false">
                        <CustomConfig>
                            <ext:ConfigItem Name="startDateField" Value="#{dfFechaInicio}" Mode="Value" />
                        </CustomConfig>
                        <Listeners>
                            <KeyUp Fn="onKeyUp" />
                            <Change Handler="if (this.getValue() == '') { #{tfHoraFin}.clear(); }" />
                        </Listeners>
                    </ext:DateField>
                    <ext:Panel runat="server" Layout="FitLayout" Height="20" Border="false" />
                    <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false" LabelWidth="5">
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
                    <ext:Toolbar runat="server">
                        <Items>
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnBuscarHide" runat="server" Hidden="true">
                                <Listeners>
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Llamadas...' });
                                        #{GridLlamadasTH}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                        Before="if (!#{FormPanelFiltros}.getForm().isValid()) { return false; }
                                        else if (!#{dfFechaInicio}.getValue() && !#{dfFechaFin}.getValue()) 
                                        { Ext.Msg.alert('Aviso', 'Ingresa al menos una fecha de filtro.');
                                        return false; }
                                        else { resetToolbar(#{PagingRepLlamTH});
                                        #{GridLlamadasTH}.getStore().sortInfo = null; }">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                            RepLlamadasTH.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>
            </ext:FormPanel>
        </West>
        <Center Split="true" Collapsible="false">
            <ext:FormPanel ID="FormPanelLlamadas" runat="server" Layout="FitLayout" Title="Llamadas obtenidas con los filtros seleccionados">
                <Items>
                    <ext:GridPanel ID="GridLlamadasTH" runat="server" StripeRows="true" Header="false" Border="false" AutoScroll="true">
                        <LoadMask ShowMask="false" />
                        <Store>
                            <ext:Store ID="StoreLlamadasTH" runat="server" RemoteSort="true" OnRefreshData="StoreLlamadasTH_RefreshData"
                                AutoLoad="false">
                                <AutoLoadParams>
                                    <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                </AutoLoadParams>
                                <Proxy>
                                    <ext:PageProxy />
                                </Proxy>                                
                                <DirectEventConfig IsUpload="true"/>
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_BitacoraLlamada">
                                        <Fields>
                                            <ext:RecordField Name="ID_BitacoraLlamada" />
                                            <ext:RecordField Name="Tarjeta" />
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
                                <ext:Column DataIndex="Tarjeta" Header="Tarjeta" Width="100" />
                                <ext:DateColumn DataIndex="Fecha" Header="Fecha/Hora" Format="yyyy-MM-dd HH:mm:ss" Width="120" />
                                <ext:Column DataIndex="Motivo" Header="Motivo" Width="150" />
                                <ext:Column DataIndex="Persona" Header="Persona" Width="200" />
                                <ext:Column DataIndex="Comentarios" Header="Comentarios" Width="300" />
                            </Columns>
                        </ColumnModel>
                        <SelectionModel>
                            <ext:RowSelectionModel SingleSelect="true"  />
                        </SelectionModel>
                        <Plugins>
                            <ext:GridFilters runat="server" Local="true" FiltersText="Filtros">
                                <Filters>
                                    <ext:DateFilter DataIndex="Fecha" BeforeText="Antes de" OnText="El día"
                                        AfterText="Después de">
                                        <DatePickerOptions runat="server" TodayText="Hoy" />
                                    </ext:DateFilter>                                    
                                    <ext:StringFilter DataIndex="Motivo" />
                                    <ext:StringFilter DataIndex="Persona" />                                    
                                </Filters>
                            </ext:GridFilters>
                        </Plugins>
                        <BottomBar>
                            <ext:PagingToolbar ID="PagingRepLlamTH" runat="server" StoreID="StoreLlamadasTH" DisplayInfo="true"
                                DisplayMsg="Mostrando Llamadas {0} - {1} de {2}" HideRefresh="true"/>
                        </BottomBar>
                        <TopBar>
                            <ext:Toolbar runat="server">
                                <Items>
                                    <ext:ToolbarFill runat="server" />
                                    <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="true">
                                        <DirectEvents>
                                            <Click OnEvent="Download" IsUpload="true"
                                                After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                    e.stopEvent(); 
                                                    RepLlamadasTH.StopMask();" />
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
