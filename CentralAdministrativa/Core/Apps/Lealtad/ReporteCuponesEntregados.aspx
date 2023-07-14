<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReporteCuponesEntregados.aspx.cs"
    Inherits="Lealtad.ReporteCuponesEntregados" %>

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

<asp:Content runat="server" ContentPlaceHolderID="MainContent">       
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true" Collapsible="true">
            <ext:FormPanel ID="FormPanel1" Width="300" Title="Selecciona los filtros" runat="server"
                Border="false" Layout="FitLayout">
                <Content>
                    <ext:Hidden ID="FormatType" runat="server" />
                </Content>
                <Items>
                    <ext:Panel ID="Panel1" runat="server" Border="false" Padding="10">
                        <Items>
                            <ext:DateField ID="dfFechaInicio" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                MsgTarget="Side" Format="yyyy/MM/dd" Width="280" EnableKeyEvents="true" MaxWidth="280"
                                AllowBlank="false">
                                <CustomConfig>
                                    <ext:ConfigItem Name="endDateField" Value="#{dfFechaFin}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:DateField ID="dfFechaFin" runat="server" Vtype="daterange" FieldLabel="Fecha Final"
                                MaxWidth="280" Width="280" MsgTarget="Side" Format="yyyy/MM/dd" EnableKeyEvents="true"
                                AllowBlank="false">
                                <CustomConfig>
                                    <ext:ConfigItem Name="startDateField" Value="#{dfFechaInicio}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                              <ext:ComboBox ID="cBoxCadena" runat="server" FieldLabel="Cadena" Width="280" DisplayField="NombreComercial"
                                ValueField="ID_Cadena" Mode="Local" AutoSelect="true" ForceSelection="true" TypeAhead="true"
                                MinChars="1" MatchFieldWidth="false" Name="cadenas" ListWidth="300">
                                <Items>
                                    <ext:ListItem Text="( Todas )" Value="-1" />
                                </Items>
                                <Store>
                                    <ext:Store ID="StoreCadena" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Cadena">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Cadena" />
                                                    <ext:RecordField Name="ClaveCadena" />
                                                    <ext:RecordField Name="NombreComercial" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                        <SortInfo Field="NombreComercial" Direction="ASC" />
                                    </ext:Store>
                                </Store>
                                <Triggers>
                                    <ext:FieldTrigger Icon="Clear" Qtip="Borrar" />
                                </Triggers>
                                <Listeners>
                                    <TriggerClick Handler="this.removeByValue(this.getValue());this.clearValue();" />
                                </Listeners>
                                <DirectEvents>
                                    <Select OnEvent="LlenaPromociones" Before="#{cBoxPromocion}.clearValue();">
                                        <EventMask ShowMask="true" Msg="Buscando Promociones..." MinDelay="200" />
                                    </Select>
                                </DirectEvents>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cBoxPromocion" runat="server" FieldLabel="Promoción" Width="280"
                                DisplayField="Descripcion" ValueField="ID_Evento" ListWidth="500">
                                <Items>
                                    <ext:ListItem Text="( Todas )" Value="-1" />
                                </Items>
                                <Store>
                                    <ext:Store ID="StorePromocion" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Evento">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Evento" />
                                                    <ext:RecordField Name="Descripcion" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <Triggers>
                                    <ext:FieldTrigger Icon="Clear" Qtip="Borrar" />
                                </Triggers>
                                <Listeners>
                                    <TriggerClick Handler="this.removeByValue(this.getValue());this.clearValue();" />
                                </Listeners>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cBoxTipoEmision" runat="server" FieldLabel="Tipo de Emisión" Width="280"
                                DisplayField="Descripcion" ValueField="ID_GrupoMA">
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                                <Store>
                                    <ext:Store ID="StoreTipoEmision" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_GrupoMA">
                                                <Fields>
                                                    <ext:RecordField Name="ID_GrupoMA" />
                                                    <ext:RecordField Name="ClaveGrupo" />
                                                    <ext:RecordField Name="Descripcion" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <Triggers>
                                    <ext:FieldTrigger Icon="Clear" Qtip="Borrar" />
                                </Triggers>
                                <Listeners>
                                    <TriggerClick Handler="this.removeByValue(this.getValue());this.clearValue();" />
                                </Listeners>
                            </ext:ComboBox>
                        </Items>
                    </ext:Panel>
                </Items>
                <FooterBar>
                    <ext:Toolbar ID="Toolbar2" runat="server">
                        <Items>
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                 <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnBuscarHide" runat="server" Hidden="true">
                                <Listeners>
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Cupones Entregados...' });
                                        #{GridPanelCupones}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                        Before="resetToolbar(#{PagingToolBar1});
                                        #{GridPanelCupones}.getStore().sortInfo = null;">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                            Lealtad.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>
            </ext:FormPanel>
        </West>
        <Center Split="true" Collapsible="false">
            <ext:FormPanel ID="FormPanelCupones" runat="server" Layout="FitLayout" Title="Cupones entregados obtenidos con los filtros">
                <Items>
                    <ext:GridPanel ID="GridPanelCupones" runat="server" StripeRows="true" Layout="FitLayout" AutoScroll="true">
                        <Store>
                            <ext:Store ID="StoreCupones" runat="server" OnSubmitData="StoreSubmit" RemoteSort="true" 
                                OnRefreshData="StoreCupones_RefreshData" AutoLoad="false">
                                <AutoLoadParams>
                                    <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                </AutoLoadParams>
                                <Proxy>
                                    <ext:PageProxy />
                                </Proxy>                                
                                <DirectEventConfig IsUpload="true" />
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_Bitacora">
                                        <Fields>
                                            <ext:RecordField Name="ID_Bitacora" />
                                            <ext:RecordField Name="CodigoCupon" />
                                            <ext:RecordField Name="Cadena" />
                                            <ext:RecordField Name="ClavePromocion" />
                                            <ext:RecordField Name="Promocion" />
                                            <ext:RecordField Name="TipoEmision" />
                                            <ext:RecordField Name="FechaEntrega" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <ColumnModel ID="ColumnModel1" runat="server">
                            <Columns>
                                <ext:Column Hidden="true" DataIndex="ID_Bitacora"/>                                
                                <ext:Column Header="Código del Cupón" Sortable="true" DataIndex="CodigoCupon" Width="100" />
                                <ext:Column Header="Cadena" Sortable="true" DataIndex="Cadena" Width="150" />
                                <ext:Column Header="Clave Promoción" Sortable="true" DataIndex="ClavePromocion" Width="100" />
                                <ext:Column Header="Promoción" Sortable="true" DataIndex="Promocion" Width="200" />
                                <ext:Column Header="Tipo de Emisión" Sortable="true" DataIndex="TipoEmision" />
                                <ext:DateColumn Header="Fecha de Entrega" Sortable="true" DataIndex="FechaEntrega"
                                    Format="dd/MM/yyyy HH:mm:ss" Width="120"/>
                            </Columns>
                        </ColumnModel>
                        <TopBar>
                            <ext:Toolbar ID="Toolbar5" runat="server">
                                <Items>
                                    <ext:ToolbarFill ID="ToolbarFill6" runat="server" />
                                    <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="true">
                                        <DirectEvents>
                                            <Click OnEvent="Download" IsUpload="true"
                                                After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                    e.stopEvent(); 
                                                    Lealtad.StopMask();" />
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnExportCSV" runat="server" Text="Exportar a CSV" Icon="PageAttach" Disabled="true">
                                        <Listeners>
                                            <Click Handler="submitValue(#{GridPanelCupones}, #{FormatType}, 'csv');"  />
                                        </Listeners>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                        <BottomBar>
                            <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreCupones" DisplayInfo="true"
                                DisplayMsg="Cupones del {0} al {1} de {2}" />
                        </BottomBar>
                    </ext:GridPanel>
                </Items>
            </ext:FormPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
