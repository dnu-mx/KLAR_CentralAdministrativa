<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReporteEnvioCorreosVentaSAMS.aspx.cs"
    Inherits="Lealtad.ReporteEnvioCorreosVentaSAMS" %>

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
                                ValueField="ClaveCadena" Mode="Local" AutoSelect="true" ForceSelection="true" TypeAhead="true"
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
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Correo Ventas Referidos...' });
                                        #{GridPanelCorreos}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                        Before="resetToolbar(#{PagingToolBar1});
                                        #{GridPanelCorreos}.getStore().sortInfo = null;">
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
            <ext:FormPanel ID="FormPanelCorreos" runat="server" Layout="FitLayout" Title="Cupones referidos obtenidos con los filtros">
                <Items>
                    <ext:GridPanel ID="GridPanelCorreos" runat="server" StripeRows="true" Layout="FitLayout" AutoScroll="true">
                        <Store>
                            <ext:Store ID="StoreCorreos" runat="server" OnSubmitData="StoreSubmit" RemoteSort="true" 
                                OnRefreshData="StoreCorreos_RefreshData" AutoLoad="false">
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
                                            <ext:RecordField Name="FechaCreacion" />
                                            <ext:RecordField Name="FechaCompra" />
                                            <ext:RecordField Name="ClaveCadena" />
                                            <ext:RecordField Name="NombreComercial" />
                                            <ext:RecordField Name="Correo" />
                                            <ext:RecordField Name="Sucursal" />
                                            <ext:RecordField Name="Operador" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <ColumnModel ID="ColumnModel1" runat="server">
                            <Columns>
                                <ext:DateColumn Header="Fecha Creación" Sortable="true" DataIndex="FechaCreacion"
                                    Format="dd/MM/yyyy HH:mm:ss" Width="120"/>
                                <ext:DateColumn Header="Fecha Compra" Sortable="true" DataIndex="FechaCompra"
                                    Format="dd/MM/yyyy HH:mm:ss" Width="120"/>
                                <ext:Column Header="Clave Cadena" Sortable="true" DataIndex="ClaveCadena" Width="150" />
                                <ext:Column Header="Nombre Comercial" Sortable="true" DataIndex="NombreComercial" Width="200" />
                                <ext:Column Header="Correo" Sortable="true" DataIndex="Correo" Width="250" />
                                <ext:Column Header="Sucursal" Sortable="true" DataIndex="Sucursal" Width="250" />
                                <ext:Column Header="Operador" Sortable="true" DataIndex="Operador" Width="250" />
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
                            <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreCorreos" DisplayInfo="true"
                                DisplayMsg="Cupones del {0} al {1} de {2}" />
                        </BottomBar>
                    </ext:GridPanel>
                </Items>
            </ext:FormPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
