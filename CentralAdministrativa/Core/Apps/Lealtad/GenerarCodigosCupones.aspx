<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GenerarCodigosCupones.aspx.cs" 
    Inherits="Lealtad.GenerarCodigosCupones" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
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
    <ext:BorderLayout ID="MainBorderLayout" runat="server">
        <Content>
            <ext:Store ID="StoreCadenas" runat="server">
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
            <ext:Store ID="StoreEvento" runat="server">
                <Reader>
                    <ext:JsonReader IDProperty="ID_Evento">
                        <Fields>
                            <ext:RecordField Name="ID_Evento" />
                            <ext:RecordField Name="ClaveEvento" />
                            <ext:RecordField Name="Descripcion" />
                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>
        </Content>
        <North Split="true">
            <ext:FormPanel ID="FormPanel1" runat="server" Border="false" Layout="ColumnLayout"
                Height="210">
                <Items>
                    <ext:Panel runat="server" Border="false" Width="300" Height="210">
                        <Items>
                            <ext:Image ID="Image1" runat="server" Width="300" Height="200" ImageUrl="Images/GiftCard.jpg">
                            </ext:Image>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="PanelColumnDummy" runat="server" Border="false" Width="20" Height="210">
                        <Items>
                        </Items>
                    </ext:Panel>
                    <ext:Panel runat="server" Border="false" Layout="RowLayout" AutoScroll="true" LabelWidth="200"
                        LabelAlign="Top" Padding="10">
                        <Items>
                            <ext:Panel runat="server" Border="false" Header="false" Width="250" Layout="FormLayout">
                                <Items>
                                    <ext:ComboBox ID="cBoxCadena" runat="server" FieldLabel="Cadena" AnchorHorizontal="100%"
                                        StoreID="StoreCadenas" DisplayField="NombreORazonSocial" ValueField="ClaveColectiva"
                                        Mode="Local" AutoSelect="true" ForceSelection="true" TypeAhead="true" MinChars="1"
                                        MatchFieldWidth="false" AllowBlank="false" />
                                </Items>
                            </ext:Panel>
                            <ext:Panel runat="server" Border="false" Header="false" Width="250" Layout="FormLayout">
                                <Items>
                                    <ext:Hidden ID="hdnIdColectiva" runat="server" />
                                    <ext:TextField ID="txtValor" runat="server" FieldLabel="Valor en Puntos" MaskRe="[0-9\.]"
                                        AnchorHorizontal="100%" AllowBlank="false" MaxLengthText="6" />
                                </Items>
                            </ext:Panel>
                            <ext:Panel runat="server" Border="false" Header="false" Width="250" Layout="FormLayout">
                                <Items>
                                    <ext:TextField ID="txtCantidad" runat="server" FieldLabel="Cantidad de Códigos a Generar"
                                        MaskRe="[0-9]" AnchorHorizontal="100%" AllowBlank="false" MaxLengthText="5" />
                                </Items>
                            </ext:Panel>
                            <ext:Panel runat="server" Border="false" Header="false" Width="250" Layout="FormLayout">
                                <Items>
                                    <ext:TextField ID="txtLongitud" runat="server" FieldLabel="Longitud del Código"
                                        MaskRe="[0-9]" AnchorHorizontal="100%" Text="8" AllowBlank="false" MaxLengthText="2" />
                                </Items>
                            </ext:Panel>
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="PanelColumnDummy2" runat="server" Border="false" Width="40" Height="210" Layout="RowLayout">
                        <Items>
                        </Items>
                    </ext:Panel>
                    <ext:Panel runat="server" Border="false" Layout="RowLayout" AutoScroll="true" LabelWidth="200"
                        LabelAlign="Top" Padding="10">
                        <Items>
                            <ext:Panel runat="server" Border="false" Header="false" Width="250" Layout="FormLayout">
                                <Items>
                                    <ext:ComboBox ID="cBoxEvento" runat="server" FieldLabel="Promoción" AnchorHorizontal="100%" 
                                        ValueField="ClaveEvento" DisplayField="Descripcion" StoreID="StoreEvento" AllowBlank="false"/>
                                </Items>
                            </ext:Panel>
                            <ext:Panel runat="server" Border="false" Header="false" Width="250" Layout="FormLayout">
                                <Items>
                                    <ext:ComboBox ID="cBoxTipoCodigo" runat="server" FieldLabel="Tipo de Código"
                                        AnchorHorizontal="100%" AllowBlank="false" SelectedIndex="0">
                                        <Items>
                                            <ext:ListItem Text="Alfanumérico" Value="ALFANUM" />
                                            <ext:ListItem Text="Numérico" Value="NUM" />
                                        </Items>
                                    </ext:ComboBox>
                                </Items>
                            </ext:Panel>
                            <ext:Panel runat="server" Border="false" Header="false" Width="250" Layout="FormLayout">
                                <Items>
                                    <ext:DateField ID="dfExpiracion" runat="server" FieldLabel="Fecha de Expiración"
                                        AnchorHorizontal="100%" Format="dd/MM/yyyy" AllowBlank="false" />
                                </Items>
                            </ext:Panel>
                            <ext:Panel runat="server" Border="false" Header="false" Layout="FormLayout" Height="15">
                                <Items>
                                    <ext:Label runat="server" Text="" />
                                </Items>
                            </ext:Panel>
                            <ext:Panel runat="server" Border="false" Header="false" Width="250" Layout="ColumnLayout">
                                <Items>
                                    <ext:Panel runat="server" Border="false" Width="40" Height="70" />
                                    <ext:Panel runat="server" Border="false" Width="100" Height="70">
                                        <Items>
                                             <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh" 
                                                 Width="80">
                                                <DirectEvents>
                                                    <Click OnEvent="btnLimpiar_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel runat="server" Border="false" Height="70">
                                        <Items>
                                            <ext:Button ID="btnGenerar" runat="server" Text="Generar Códigos" Icon="ControlPlay">
                                                <DirectEvents>
                                                    <Click OnEvent="btnGenerar_Click" Before="var valid= #{FormPanel1}.getForm().isValid();
                                                        if (!valid) {} return valid;">
                                                        <EventMask ShowMask="true" Msg="Generando Códigos..." MinDelay="500" />
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Panel>
                                </Items>
                            </ext:Panel>
                        </Items>
                    </ext:Panel>
                </Items>
            </ext:FormPanel>
        </North>
        <Center Split="true">
            <ext:GridPanel ID="GridCodigosGenerados" runat="server" Title="Códigos Generados" Layout="FitLayout">
                <LoadMask ShowMask="false" />
                <Store>
                    <ext:Store ID="StoreCodigosGenerados" runat="server" RemoteSort="true" AutoLoad="false"
                        OnRefreshData="StoreCodigosGenerados_RefreshData">
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
                                    <ext:RecordField Name="Valor" />
                                    <ext:RecordField Name="FechaExpiracion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <View>
                    <ext:GridView ID="GridView1" ForceFit="true" ScrollOffset="2" runat="server" />
                </View>
                <ColumnModel runat="server">
                    <Columns>
                        <ext:Column DataIndex="ID_Codigo" Hidden="true" />
                        <ext:Column DataIndex="Codigo" Header="Código" />
                        <ext:Column DataIndex="Valor" Header="Valor" />
                        <ext:DateColumn DataIndex="FechaExpiracion" Header="Expiración" Format="dd/MM/yyyy HH:mm:ss" />                        
                    </Columns>
                </ColumnModel>
                <SelectionModel>
                    <ext:CellSelectionModel runat="server">
                        <Listeners>
                            <%--<CellSelect Handler="window.clipboardData.setData('Text','Text that I want to copy to cliboard);" /> --%>
                        </Listeners>
                    </ext:CellSelectionModel>
                </SelectionModel>
                <TopBar>
                    <ext:Toolbar ID="Toolbar5" runat="server">
                        <Items>
                            <ext:ToolbarFill ID="ToolbarFill6" runat="server" />
                            <ext:Button ID="btnBuscarHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click Timeout="360000" />
                                </DirectEvents>
                                <Listeners>
                                    <Click Handler="resetToolbar(#{PagingToolBar1});
                                        #{GridCodigosGenerados}.getStore().sortInfo = null;
                                        Ext.net.Mask.show({ msg : 'Obteniendo Códigos Generados...' });
                                        #{GridCodigosGenerados}.getStore().reload({params:{start:0, sort:('','')}});"/>
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                            GeneraCodigos.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                            e.stopEvent(); 
                                            GeneraCodigos.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreCodigosGenerados" DisplayInfo="true"
                        DisplayMsg="Códigos del {0} al {1} de {2}" HideRefresh="true" />
                </BottomBar>
            </ext:GridPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
