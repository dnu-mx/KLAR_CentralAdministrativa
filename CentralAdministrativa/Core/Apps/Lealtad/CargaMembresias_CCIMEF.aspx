<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CargaMembresias_CCIMEF.aspx.cs"
    Inherits="Lealtad.CargaMembresias_CCIMEF" %>

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
                </Content>
                <Items>
                    <ext:Panel ID="Panel1" runat="server" Border="false" Padding="10">
                        <Items>
                            <ext:TextField ID="txtMembresia" FieldLabel="Membresía" EmptyText="Todas" AnchorHorizontal="95%" MaxLength="50" Width="275" runat="server" Text="" AllowBlank="true" />
                            
                            <ext:Panel runat="server" Title="Periodo de Registro" Padding="3" FormGroup="true">
                                <Items>
                                    <ext:DateField ID="dfFechaInicio" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                        Format="yyyy/MM/dd" Width="275" EnableKeyEvents="true" AllowBlank="true">
                                        <CustomConfig>
                                            <ext:ConfigItem Name="endDateField" Value="#{dfFechaFin}" Mode="Value" />
                                        </CustomConfig>
                                        <Listeners>
                                            <KeyUp Fn="onKeyUp" />
                                        </Listeners>
                                    </ext:DateField>
                                    <ext:DateField ID="dfFechaFin" runat="server" Vtype="daterange" FieldLabel="Fecha Final"
                                        Width="275" Format="yyyy/MM/dd" EnableKeyEvents="true" AllowBlank="true">
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
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Membresias...' });
                                        #{GridPanelMembresias}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                        Before="resetToolbar(#{PagingToolBar1});
                                        #{GridPanelMembresias}.getStore().sortInfo = null;">
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
            <ext:Panel ID="PanelCentral" runat="server" Height="250" Title="Membresías" Border="false" Disabled="false">
                <Items>
                    <ext:BorderLayout runat="server">
                        <Center>
                            <ext:TabPanel runat="server">
                                <Items>
                                    <ext:FormPanel ID="FormPanelMembresias" runat="server" Layout="FitLayout" Title="Consulta">
                                        <Items>
                                            <ext:GridPanel ID="GridPanelMembresias" runat="server" StripeRows="true" Layout="FitLayout" Region="Center">
                                                <Store>
                                                    <ext:Store ID="StoreMembresias" runat="server" OnSubmitData="StoreSubmit" RemoteSort="true" 
                                                        OnRefreshData="StoreMembresias_RefreshData" AutoLoad="false">
                                                        <AutoLoadParams>
                                                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                                        </AutoLoadParams>
                                                        <Proxy>
                                                            <ext:PageProxy />
                                                        </Proxy>                                
                                                        <DirectEventConfig IsUpload="true" />
                                                        <Reader>
                                                            <ext:JsonReader IDProperty="ID">
                                                                <Fields>
                                                                    <ext:RecordField Name="Cliente" />
                                                                    <ext:RecordField Name="Membresia" />
                                                                    <ext:RecordField Name="FechaInsert" />
                                                                    <ext:RecordField Name="FechaExpiracion" />
                                                                    <ext:RecordField Name="UsuarioInsert" />
                                                                </Fields>
                                                            </ext:JsonReader>
                                                        </Reader>
                                                    </ext:Store>
                                                </Store>
                                                <ColumnModel ID="ColumnModel1" runat="server">
                                                    <Columns>
                                                        <ext:Column ColumnID="Cliente" Header="Cliente" DataIndex="Cliente" Width="100" Sortable="true"/>
                                                        <ext:Column ColumnID="Membresia" Header="Membresia" DataIndex="Membresia" Width="200" Sortable="true"/>
                                                        <ext:DateColumn ColumnID="FechaInsert" Header="Fecha Insert" Sortable="true"
                                                            DataIndex="FechaInsert" Format="dd/MM/yyyy" Width="100"/>
                                                        <ext:DateColumn ColumnID="FechaExpiracion" Header="Fecha Expiración" Sortable="true"
                                                            DataIndex="FechaExpiracion" Format="dd/MM/yyyy" Width="100"/>
                                                        <ext:Column ColumnID="UsuarioInsert" Header="Usuario Inserto" DataIndex="UsuarioInsert" Width="200" Sortable="true"/>
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
                                                        </Items>
                                                    </ext:Toolbar>
                                                </TopBar>
                                                <BottomBar>
                                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreMembresias" DisplayInfo="true"
                                                        DisplayMsg="Mostrando Membresias {0} - {1} de {2}" />
                                                </BottomBar>
                                            </ext:GridPanel>
                                        </Items>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanel2" runat="server" Layout="FitLayout" Title="Carga de Membresías">
                                        <Items>
                                            <ext:BorderLayout ID="LeftBorderLayout" runat="server">
                                            <North Split="true">
                                                <ext:FormPanel ID="FormPanel3" runat="server" Height="30">
                                                    <Items>
                                                        <ext:Toolbar ID="ToolbarConsulta" runat="server" Layout="HBoxLayout" BodyPadding="5"
                                                            Region="North">
                                                            <Defaults>
                                                                <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                            </Defaults>
                                                            <LayoutConfig>
                                                                <ext:HBoxLayoutConfig Align="Middle" />
                                                            </LayoutConfig>
                                                            <Items>
                                                                <ext:FileUploadField ID="FileUploadField1" runat="server" ButtonText="Examinar..."
                                                                    Icon="Magnifier" Flex="3" MarginSpec="0" />
                                                                <ext:Hidden ID="Hidden1" runat="server" Flex="1" />
                                                                <ext:Button ID="btnCargarArchivo" runat="server" Text="Cargar Archivo"
                                                                    Icon="PageWhitePut" Flex="1">
                                                                    <DirectEvents>
                                                                        <Click OnEvent="btnCargarArchivo_Click" IsUpload="true">
                                                                            <EventMask ShowMask="true" Msg="Cargando Archivo..." MinDelay="500" />
                                                                        </Click>
                                                                    </DirectEvents>
                                                                </ext:Button>
                                                            </Items>
                                                        </ext:Toolbar>
                                                    </Items>
                                                </ext:FormPanel>
                                            </North>
                                            <Center Split="true" Collapsible="false">
                                                <ext:FormPanel ID="FormPanelResultados" runat="server" Layout="FitLayout">
                                                    <Items>
                                                        <ext:GridPanel ID="GridDatosArchivo" runat="server" StripeRows="true"
                                                            Layout="fit" Region="Center">
                                                            <Store>
                                                                <ext:Store runat="server">
                                                                    <Reader>
                                                                        <ext:JsonReader IDProperty="ID">
                                                                            <Fields>
                                                                                <ext:RecordField Name="IDCliente" />
                                                                                <ext:RecordField Name="Membresia" />
                                                                                <ext:RecordField Name="Vigencia" />
                                                                            </Fields>
                                                                        </ext:JsonReader>
                                                                    </Reader>
                                                                </ext:Store>
                                                            </Store>
                                                            <ColumnModel ID="ColumnModel12" runat="server">
                                                                <Columns>
                                                                    <ext:Column ColumnID="IDCliente" Header="ID Cliente" Sortable="true" DataIndex="IDCliente" Width="80"/>
                                                                    <ext:Column ColumnID="Membresia" Header="Membresia" Sortable="true" DataIndex="Membresia" Width="90"/>
                                                                    <ext:DateColumn ColumnID="Vigencia" Header="Vigencia" Sortable="true" DataIndex="Vigencia" Format="dd/MM/yyyy"/>
                                                                    
                                                                </Columns>
                                                            </ColumnModel>
                                                            <BottomBar>
                                                                <ext:Toolbar ID="Toolbar1" runat="server">
                                                                    <Items>
                                                                        <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                                        <ext:Button ID="btnAplicarCambios" runat="server" Text="Aplicar Cambios" Icon="Tick">
                                                                            <DirectEvents>
                                                                                <Click OnEvent="btnAplicarCambios_Click">
                                                                                    <EventMask ShowMask="true" Msg="Aplicando cambios..." MinDelay="500" />
                                                                                </Click>
                                                                            </DirectEvents>
                                                                        </ext:Button>
                                                                    </Items>
                                                                </ext:Toolbar>
                                                            </BottomBar>
                                                        </ext:GridPanel>
                                                    </Items>
                                                </ext:FormPanel>
                                            </Center>
                                        </ext:BorderLayout>
                                        </Items>
                                    </ext:FormPanel>
                                </Items>
                            </ext:TabPanel>
                        </Center>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
