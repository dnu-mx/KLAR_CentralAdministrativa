<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReporteBeneficiosSams.aspx.cs" Inherits="Lealtad.ReporteBeneficiosSams" %>

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
            <ext:FormPanel ID="FormPanel1" Width="300" Title="Selecciona los Filtros" runat="server"
                Border="false" Layout="FitLayout">
                <Content>
                    <ext:Hidden ID="FormatType" runat="server" />
                    <ext:Store ID="StoreClaveCadena" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Cadena">
                                <Fields>
                                    <ext:RecordField Name="ID_Cadena" />
                                    <ext:RecordField Name="ClaveCadena" />
                                    <ext:RecordField Name="NombreComercial" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="ClaveCadena" Direction="ASC" />
                    </ext:Store>
                    <ext:Store ID="StoreGiros" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Giro">
                                <Fields>
                                    <ext:RecordField Name="ID_Giro" />
                                    <ext:RecordField Name="Clave" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:Store ID="StorePromociones" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Promocion">
                                <Fields>
                                    <ext:RecordField Name="ID_Promocion" />
                                    <ext:RecordField Name="ClavePromocion" />
                                    <ext:RecordField Name="Descripción" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Content>
                <Items>
                    <ext:Panel ID="PanelFiltros" runat="server" Border="false" Padding="10">
                        <Items>
                            <ext:ComboBox ID="cBoxClaveCadena" runat="server" FieldLabel="Clave Cadena" ListWidth="350"
                                Width="280" EmptyText="Todas" StoreID="StoreClaveCadena" DisplayField="ClaveCadena" ValueField="ID_Cadena"
                                Mode="Local" AutoSelect="true" Editable="true" ForceSelection="true" TypeAhead="true" MinChars="1"
                                MatchFieldWidth="false" Name="claveCadena">
                                <Items>
                                    <ext:ListItem Text="( Todas )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:TextField ID="txtCadena" runat="server" FieldLabel="Cadena" Width="280" EmptyText="Todas"/>
                            <ext:ComboBox ID="cBoxGiro" runat="server" FieldLabel="Giro" Width="280" ListWidth="350"
                                EmptyText="Todos" StoreID="StoreGiros" DisplayField="Descripcion" ValueField="ID_Giro">
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cBoxClaveBeneficio" runat="server" FieldLabel="Clave Promoción" EmptyText="Todas"
                                ListWidth="350" Width="280" StoreID="StorePromociones" DisplayField="ClavePromocion" ValueField="ID_Promocion">
                                <Items>
                                    <ext:ListItem Text="( Todas )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cBoxActiva" runat="server" FieldLabel="Activa" EmptyText="Todas"
                                Width="280">
                                 <Items>
                                     <ext:ListItem Text="( Todas )" Value="-1" />
                                     <ext:ListItem Text="Sí" Value="1" />
                                     <ext:ListItem Text="No" Value="0" />
                                </Items>
                            </ext:ComboBox>
                            <ext:DateField ID="dfFechaInicio" runat="server" Vtype="daterange" FieldLabel="Fecha de Inicio"
                                MsgTarget="Side" Format="yyyy/MM/dd" Width="280" EnableKeyEvents="true" MaxWidth="280">
                                <CustomConfig>
                                    <ext:ConfigItem Name="endDateField" Value="#{dfFechaFin}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:DateField ID="dfFechaFin" runat="server" Vtype="daterange" FieldLabel="Fecha de Fin"
                                MaxWidth="280" Width="280" MsgTarget="Side" Format="yyyy/MM/dd" EnableKeyEvents="true">
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
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Beneficios...' });
                                        #{GridPanelBeneficios}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                        Before="resetToolbar(#{PagingToolBar1});
                                        #{GridPanelBeneficios}.getStore().sortInfo = null;">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                            RepBenefSams.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>
            </ext:FormPanel>
        </West>
        <Center Split="true">
            <ext:FormPanel ID="FormPanelBeneficios" runat="server" Layout="FitLayout" Title="Beneficios obtenidos con los Filtros">
                <Items>
                    <ext:GridPanel ID="GridPanelBeneficios" runat="server" StripeRows="true" Header="true"
                        Layout="fit" Region="Center">
                        <Store>
                            <ext:Store ID="StoreBeneficios" runat="server" OnSubmitData="StoreSubmit" RemoteSort="true" 
                                OnRefreshData="StoreBeneficios_RefreshData" AutoLoad="false">
                                <AutoLoadParams>
                                    <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                </AutoLoadParams>
                                <Proxy>
                                    <ext:PageProxy />
                                </Proxy>                                
                                <DirectEventConfig IsUpload="true" />
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_Promocion">
                                        <Fields>
                                            <ext:RecordField Name="ClaveCadena" />
                                            <ext:RecordField Name="Cadena" />
                                            <ext:RecordField Name="Giro" />
                                            <ext:RecordField Name="ClavePromocion" />
                                            <ext:RecordField Name="TituloPromocion" />
                                            <ext:RecordField Name="TipoDescuento" />
                                            <ext:RecordField Name="DescripcionBeneficio" />
                                            <ext:RecordField Name="Restricciones" />
                                            <ext:RecordField Name="Facebook" />
                                            <ext:RecordField Name="Web" />
                                            <ext:RecordField Name="EsHotDeal" />
                                            <ext:RecordField Name="CarruselHome" />
                                            <ext:RecordField Name="PromoHome" />
                                            <ext:RecordField Name="Orden" />
                                            <ext:RecordField Name="Activa" />
                                            <ext:RecordField Name="FechaInicio" />
                                            <ext:RecordField Name="FechaFin" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <ColumnModel ID="ColumnModel12" runat="server">
                            <Columns>
                                <ext:Column ColumnID="ClaveCadena" Header="Clave Cadena" Sortable="true" DataIndex="ClaveCadena" />
                                <ext:Column ColumnID="Cadena" Header="Cadena" Sortable="true" DataIndex="Cadena" />
                                <ext:Column ColumnID="Giro" Header="Giro" Sortable="true" DataIndex="Giro" />
                                <ext:Column ColumnID="ClavePromocion" Header="Clave Promoción" Sortable="true" DataIndex="ClavePromocion" />
                                <ext:Column ColumnID="TituloPromocion" Header="Título Promoción" Sortable="true" DataIndex="TituloPromocion" />
                                <ext:Column ColumnID="TipoDescuento" Header="Tipo Descuento" Sortable="true" DataIndex="TipoDescuento" />
                                <ext:Column ColumnID="DescripcionBeneficio" Header="Descripción Beneficio" Sortable="true" DataIndex="DescripcionBeneficio" />
                                <ext:Column ColumnID="Restricciones" Header="Restricciones" Sortable="true" DataIndex="Restricciones" />
                                <ext:Column ColumnID="Facebook" Header="Facebook" Sortable="true" DataIndex="Facebook" />
                                <ext:Column ColumnID="Web" Header="Web" Sortable="true" DataIndex="Web" />
                                <ext:Column ColumnID="EsHotDeal" Header="EsHotDeal" Sortable="true" DataIndex="EsHotDeal" />
                                <ext:Column ColumnID="CarruselHome" Header="CarruselHome" Sortable="true" DataIndex="CarruselHome" />
                                <ext:Column ColumnID="PromoHome" Header="PromoHome" Sortable="true" DataIndex="PromoHome" />
                                <ext:Column ColumnID="Orden" Header="Orden" Sortable="true" DataIndex="Orden" />
                                <ext:Column ColumnID="Activa" Header="Activa" Sortable="true" DataIndex="Activa" />
                                <ext:DateColumn ColumnID="FechaInicio" Header="FechaInicio(dd/mm/aaaa)" Sortable="true" DataIndex="FechaInicio"
                                    Format="dd/MM/yyyy" />
                                <ext:DateColumn ColumnID="FechaFin" Header="FechaFin(dd/mm/aaaa)" Sortable="true" DataIndex="FechaFin"
                                    Format="dd/MM/yyyy" />
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
                                                    RepBenefSams.StopMask();" />
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnExportCSV" runat="server" Text="Exportar a CSV" Icon="PageAttach" Disabled="true">
                                        <Listeners>
                                            <Click Handler="submitValue(#{GridPanelBeneficios}, #{FormatType}, 'csv');" />
                                        </Listeners>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                        <BottomBar>
                            <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreBeneficios" DisplayInfo="true"
                                DisplayMsg="Mostrando Beneficios {0} - {1} de {2}" />
                        </BottomBar>
                    </ext:GridPanel>
                </Items>
            </ext:FormPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
