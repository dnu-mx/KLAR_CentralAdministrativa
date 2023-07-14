<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReporteCodigos.aspx.cs"
    Inherits="Lealtad.ReporteCodigos" %>

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
                Border="false" Layout="FitLayout" Padding="10">
                <Content>
                    <ext:Hidden ID="FormatType" runat="server" />
                </Content>
                <Items>
                    <ext:ComboBox ID="cBoxCadena" runat="server" FieldLabel="Cadena" Width="280" DisplayField="NombreORazonSocial"
                        ValueField="ID_Colectiva" Mode="Local" AutoSelect="true" ForceSelection="true" TypeAhead="true"
                        MinChars="1" MatchFieldWidth="false" Name="colectivas" ListWidth="300">
                        <Items>
                            <ext:ListItem Text="( Todas )" Value="-1" />
                        </Items>
                        <Store>
                            <ext:Store ID="StoreCadena" runat="server">
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
                            <ext:FieldTrigger Icon="Clear" Qtip="Borrar" />
                        </Triggers>
                        <Listeners>
                            <TriggerClick Handler="this.removeByValue(this.getValue());this.clearValue();" />
                        </Listeners>
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
                    <ext:ComboBox ID="cBoxTipoCodigo" runat="server" FieldLabel="Tipo de Código" Width="280" />
                    <ext:DateField ID="dfFechaExpiracion" runat="server" Vtype="daterange" FieldLabel="Fecha Expiración"
                        MsgTarget="Side" Format="dd/MM/yyyy" Width="280" EnableKeyEvents="true" AllowBlank="false" />
                    <ext:Panel runat="server" Title="Periodo de " FormGroup="true" Padding="3" Disabled="true">
                        <Items>
                            <ext:DateField ID="dfFechaInicio" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                MsgTarget="Side" Format="dd/MM/yyyy" Width="275" EnableKeyEvents="true" AllowBlank="false">
                                <CustomConfig>
                                    <ext:ConfigItem Name="endDateField" Value="#{dfFechaFin}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:DateField ID="dfFechaFin" runat="server" Vtype="daterange" FieldLabel="Fecha Final"
                                Width="275" MsgTarget="Side" Format="dd/MM/yyyy" EnableKeyEvents="true" AllowBlank="false">
                                <CustomConfig>
                                    <ext:ConfigItem Name="startDateField" Value="#{dfFechaInicio}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
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
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Cupones Redimidos...' });
                                        #{GridCodigos}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                        Before="resetToolbar(#{PagingToolBar1});
                                        #{GridCodigos}.getStore().sortInfo = null;">
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
            <ext:FormPanel runat="server" Layout="FitLayout" Title="Códigos obtenidos con los filtros">
                <Items>
                    <ext:GridPanel ID="GridCodigos" runat="server" StripeRows="true" Layout="FitLayout" Region="Center">
                        <Store>
                            <ext:Store ID="StoreCodigos" runat="server" OnSubmitData="StoreSubmit" RemoteSort="true" 
                                AutoLoad="false" OnRefreshData="StoreCodigos_RefreshData">
                                <AutoLoadParams>
                                    <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                </AutoLoadParams>
                                <Proxy>
                                    <ext:PageProxy />
                                </Proxy>                                
                                <DirectEventConfig IsUpload="true" />
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_MA">
                                        <Fields>
                                            <ext:RecordField Name="ID_MA" />
                                            <ext:RecordField Name="Codigo" />
                                            <ext:RecordField Name="Promocion" />
                                            <ext:RecordField Name="Valor" />
                                            <ext:RecordField Name="FechaExpiracion" />
                                            <ext:RecordField Name="FechaGeneracion" />
                                            <ext:RecordField Name="FechaRedencion" />
                                            <ext:RecordField Name="Autorizacion" />
                                            <ext:RecordField Name="Cliente" />
                                            <ext:RecordField Name="Email" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <ColumnModel ID="ColumnModel1" runat="server">
                            <Columns>
                                <ext:Column ColumnID="ID_MA" Hidden="true" DataIndex="ID_MA"/>
                                <ext:Column ColumnID="Codigo" Header="Código" Sortable="true" DataIndex="Codigo"
                                    Width="80" />
                                <ext:Column ColumnID="Promocion" Header="Promoción" Sortable="true"
                                    DataIndex="Promocion" Width="120" />
                                <ext:Column ColumnID="Valor" Header="Valor en Puntos" Sortable="true" 
                                    DataIndex="Valor" Width="120" />
                                <ext:DateColumn ColumnID="FechaExpiracion" Header="Fecha de Expiración" Sortable="true"
                                    DataIndex="FechaExpiracion" Format="dd/MM/yyyy HH:mm:ss" Width="120" />
                                <ext:DateColumn ColumnID="FechaGeneracion" Header="Fecha de Generación" Sortable="true"
                                    DataIndex="FechaGeneracion" Format="dd/MM/yyyy HH:mm:ss" Width="120"/>
                                <ext:DateColumn ColumnID="FechaRedencion" Header="Fecha de Redención" Sortable="true"
                                    DataIndex="FechaRedencion" Format="dd/MM/yyyy HH:mm:ss" Width="120"/>
                                <ext:Column ColumnID="Autorizacion" Header="Autorización" Sortable="true"
                                    DataIndex="Autorizacion" Width="80" />
                                <ext:Column ColumnID="Cliente" Header="Nombre del Cliente" Sortable="true"
                                    DataIndex="Cliente" Width="80" />
                                <ext:Column ColumnID="Email" Header="Correo Electrónico Cliente" Sortable="true"
                                    DataIndex="Email" Width="80" />
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
                                            <Click Handler="submitValue(#{GridCodigos}, #{FormatType}, 'csv');"  />
                                        </Listeners>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                        <BottomBar>
                            <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreCodigos" DisplayInfo="true"
                                DisplayMsg="Códigos del {0} al {1} de {2}" />
                        </BottomBar>
                    </ext:GridPanel>
                </Items>
            </ext:FormPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
