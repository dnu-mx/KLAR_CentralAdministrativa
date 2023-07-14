<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ReporteImagenesCadenas.aspx.cs" Inherits="Lealtad.ReporteImagenesCadenas" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
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
                Border="false" Layout="FormLayout" Padding="10" LabelAlign="Top">
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
                </Content>
                <Items>
                    <ext:Hidden ID="hdnClaveCadena" runat="server" />
                    <ext:Hidden ID="hdnNombreCadena" runat="server" />
                    <ext:ComboBox ID="cBoxClaveCadena" runat="server" FieldLabel="Clave Cadena" Width="280"
                        EmptyText="Selecciona una Clave de Cadena..." StoreID="StoreClaveCadena" 
                        DisplayField="ClaveCadena" ValueField="ID_Cadena" Mode="Local" AutoSelect="true"
                        Editable="true" ForceSelection="true" TypeAhead="true" MinChars="1"
                        MatchFieldWidth="false" >
                        <Items>
                            <ext:ListItem Text="( Todas )" Value="-1" />
                        </Items>
                        <Listeners>
                            <Select Handler="#{txtCadena}.clear();
                                #{hdnClaveCadena}.setValue(record.get('ClaveCadena'));
                                #{hdnNombreCadena}.setValue(record.get('NombreComercial'));
                                #{txtCadena}.setValue(record.get('NombreComercial'));"/>
                        </Listeners>
                    </ext:ComboBox>
                    <ext:TextField ID="txtCadena" runat="server" FieldLabel="Cadena" Width="280" />
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
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Imagenes...' });
                                        #{GridPanelImagenes}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                        Before="resetToolbar(#{PagingToolBar1});
                                        #{GridPanelImagenes}.getStore().sortInfo = null;">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                            RepImagenes.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>
            </ext:FormPanel>
        </West>
        <Center Split="true">
            <ext:FormPanel ID="FormPanelImagenes" runat="server" Layout="FitLayout" Title="Imagenes obtenidas con los Filtros">
                <Items>
                    <ext:GridPanel ID="GridPanelImagenes" runat="server" StripeRows="true" Header="true"
                        Layout="FitLayout" Region="Center">
                        <Store>
                            <ext:Store ID="StoreImagenes" runat="server" OnSubmitData="StoreSubmit" RemoteSort="true" 
                                OnRefreshData="StoreImagenes_RefreshData" AutoLoad="false">
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
                                            <ext:RecordField Name="ID_Cadena" />
                                            <ext:RecordField Name="ClaveCadena" />
                                            <ext:RecordField Name="NombreComercial" />
                                            <ext:RecordField Name="LogoSams" />
                                            <ext:RecordField Name="LogoEdenred" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <ColumnModel ID="ColumnModel12" runat="server">
                            <Columns>
                                <ext:Column Hidden="true" DataIndex="ID_Cadena" />
                                <ext:Column Header="Clave Cadena" Sortable="true" DataIndex="ClaveCadena" Width="150" />
                                <ext:Column Header="Nombre Cadena" Sortable="true" DataIndex="NombreComercial" Width="200" />
                                <ext:Column Header="Logo Sam's" Sortable="true" DataIndex="LogoSams" Width="100" />
                                <ext:Column Header="Logo Edenred" Sortable="true" DataIndex="LogoEdenred" Width="100" />
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
                                                    RepImagenes.StopMask();" />
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnExportCSV" runat="server" Text="Exportar a CSV" Icon="PageAttach" Disabled="true">
                                        <Listeners>
                                            <Click Handler="submitValue(#{GridPanelImagenes}, #{FormatType}, 'csv');" />
                                        </Listeners>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                        <BottomBar>
                            <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreImagenes" DisplayInfo="true"
                                DisplayMsg="Mostrando Imágenes Cadenas {0} - {1} de {2}" />
                        </BottomBar>
                    </ext:GridPanel>
                </Items>
            </ext:FormPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
