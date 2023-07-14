<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="AdminCertificados_Amazon.aspx.cs" Inherits="Lealtad.AdminCertificados_Amazon" ValidateRequest="false" %>

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

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Store ID="StoreDetalle" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID">
                <Fields>
                    <ext:RecordField Name="ID" />
                    <ext:RecordField Name="Monto" />
                    <ext:RecordField Name="Cantidad" />
                    <ext:RecordField Name="Completado" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Store ID="StoreLotes" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_Pedido">
                <Fields>
                    <ext:RecordField Name="ID_Pedido" />
                    <ext:RecordField Name="Fecha" />
                    <ext:RecordField Name="CantidadTotal" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Store ID="StoreDetallePedido" runat="server">
        <Reader>
            <ext:JsonReader>
                <Fields>
                    <ext:RecordField Name="Cantidad" />
                    <ext:RecordField Name="Monto" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <%--Solicitar nuevo lote de Certificados--%>
    <ext:Window ID="WdwNuevoLote" runat="server" Title="Solicitud Certificados Amazon" Width="350" Height="440" Hidden="true"
        Modal="true" Resizable="false" Icon="Add">
        <Items>
            <ext:FormPanel ID="FormPanelNP" runat="server" Height="410" Padding="3" LabelWidth="120" AutoScroll="true">
                <Items>
                    
                    <ext:GridPanel ID="GridPanelNuevo" runat="server" StoreID="StoreDetallePedido" Height="200" StripeRows="true" Header="false" Border="false" Hidden="false">
                        <LoadMask ShowMask="false" />
                        <ColumnModel ID="ColumnModel2" runat="server">
                            <Columns>
                                <ext:Column Header="ID" Width="40" DataIndex="Id" Hidden="true" />
                                <ext:Column ColumnID="Cantidad" Header="Cantidad" Sortable="true" DataIndex="Cantidad">
                                    <Editor>
                                        <ext:TextField ID="TextField1" runat="server" MaskRe="[0-9]" MaxLength="3" AllowBlank="false" />
                                    </Editor>
                                </ext:Column>
                                <ext:Column ColumnID="ColMonto" Header="Monto" Sortable="false" DataIndex="Monto">
                                    <Editor>
                                        <ext:ComboBox ID="cmbMontos" ValueField="Monto" DisplayField="Monto" runat="server" AllowBlank="false" >
                                            <Store>
                                                <ext:Store ID="StoreMontos" runat="server">
                                                    <Reader>
                                                        <ext:JsonReader IDProperty="ID_Monto">
                                                            <Fields>
                                                                <ext:RecordField Name="Monto" />
                                                            </Fields>
                                                        </ext:JsonReader>
                                                    </Reader>
                                                </ext:Store>
                                            </Store>
                                        </ext:ComboBox>
                                    </Editor>
                                </ext:Column>
                            </Columns>
                        </ColumnModel>
                        <SelectionModel>
                            <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" />
                        </SelectionModel>
                        <View>
                            <ext:GridView ID="GridView1" runat="server" ForceFit="true" />
                        </View>
                        <TopBar>
                            <ext:Toolbar ID="Toolbar1" runat="server">
                                <Items>
                                    <ext:Button ID="btnNuevoRow" runat="server" Text="Nuevo Registro" Icon="Add">
                                        <Listeners>
                                            <Click Handler="#{GridPanelNuevo}.insertRecord();#{GridPanelNuevo}.getRowEditor().startEditing(0);" />
                                        </Listeners>
                                    </ext:Button>
                                    <ext:Button ID="btnEliminarRow" runat="server" Text="Eliminar Seleccionado" Icon="Exclamation">
                                        <Listeners>
                                            <Click Handler="#{GridPanelNuevo}.deleteSelected();" />
                                        </Listeners>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                        <Plugins>
                            <ext:RowEditor ID="RowEditor1" runat="server" SaveText="Guardar" CancelText="Cancelar" />
                        </Plugins>
                    </ext:GridPanel>
                </Items>
                <Buttons>
                    <ext:Button runat="server" Text="Cancelar" Icon="Cancel">
                        <Listeners>
                            <Click Handler="#{WdwNuevoLote}.hide();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button ID="Button2" runat="server" Flat="false" Text="Generar Lote" Icon="Accept">
                        <DirectEvents>
                            <Click OnEvent="GenerarPedido" Timeout="900000" >
                                <EventMask ShowMask="true" Msg="Generando Lote..." MinDelay="500" />
                                <ExtraParams>
                                    <ext:Parameter Name="TotalRegistros" Value="#{GridPanelNuevo}.getStore().getTotalCount()"
                                        Mode="Raw" />
                                    <ext:Parameter Name="Values" Value="Ext.encode(#{GridPanelNuevo}.getRowsValues({selectedOnly:false}))"
                                        Mode="Raw" />
                                </ExtraParams>
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
            
        </Items>
    </ext:Window>

    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true">
            <ext:Panel runat="server" Width="325" Border="false" Layout="FitLayout" Title="Consulta de Lotes Generados">
                <Content>
                    <ext:BorderLayout ID="LeftBorderLayout" runat="server">
                        <South Split="true">
                            <ext:FormPanel ID="FormPanel3" runat="server" Height="25" Border="false">
                                <Items>
                                    <ext:Toolbar runat="server">
                                        <Items>
                                            <ext:Button ID="btnLimpiarIzq" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                                <DirectEvents>
                                                    <Click OnEvent="btnLimpiarIzq_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:ToolbarFill runat="server" />
                                            <ext:Button runat="server" Icon="Add" ToolTip="Solicitar nuevo lote de Certificados Amazon"
                                                Text="Solicitar Certificados">
                                                <Listeners>
                                                    <Click Handler="#{FormPanelNP}.reset(); #{WdwNuevoLote}.show(); #{GridPanelNuevo}.getStore().removeAll();" />
                                                </Listeners>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </Items>
                            </ext:FormPanel>
                        </South>
                        <Center Split="true">
                            <ext:GridPanel ID="GridResultados" runat="server" StoreID="StoreLotes" Border="false" Header="false">
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar2" runat="server">
                                        <Items>
                                           <ext:DateField ID="dtLote" runat="server" Format="MM/Y" Width="200" AllowBlank="false">
                                               <Plugins>
                                                   <ext:MonthPicker runat="server" />
                                               </Plugins>
                                           </ext:DateField>
                                            <ext:Button ID="btnBuscarLotes" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnBuscar_Click" Before="#{PanelCentral}.setTitle('');
                                                        #{PanelCentral}.setDisabled(true);
                                                        if (!#{dtLote}.getValue())
                                                        { return false; }">
                                                        <EventMask ShowMask="true" Msg="Buscando Lotes..." MinDelay="500" />
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <ColumnModel runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_Pedido" Header="ID" Width="70"/>
                                        <ext:DateColumn DataIndex="Fecha" Header="Fecha" Width="120" Format="dd/MM/yyyy" />
                                        <ext:Column DataIndex="CantidadTotal" Header="Total Certificados" Width="130" />
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel SingleSelect="true" />
                                </SelectionModel>
                                <DirectEvents>
                                    <RowClick OnEvent="selectRowResultados_Event">
                                        <EventMask ShowMask="true" Msg="Obteniendo Información del Lote..." MinDelay="500" />
                                        <ExtraParams>
                                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridResultados}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                        </ExtraParams>
                                    </RowClick>
                                </DirectEvents>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreLotes" DisplayInfo="true"
                                        DisplayMsg="{0} - {1} de {2}" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </West>
        <Center Split="true">
            <ext:Panel ID="PanelCentral" runat="server" Height="250" Title="_" Border="false" Disabled="true">
                <Items>
                    <ext:BorderLayout runat="server">
                        <Center>
                            <ext:TabPanel runat="server">
                                <Items>
                                    <ext:FormPanel ID="FormPanelInfoAd" runat="server" Title="Información del Lote" LabelAlign="Right" LabelWidth="120"
                                        AutoScroll="true">
                                        <Items>
                                            <ext:Hidden ID="hdnIdLote" runat="server" />
                                            <ext:FieldSet ID="FieldSetDatosAd" runat="server" Title="">
                                                <Items>
                                                    <ext:Panel runat="server" Layout="HBoxLayout" LabelAlign="Left" BodyPadding="5" Border="false">
                                                        <Items>
                                                            <ext:GridPanel ID="GridDetalle" runat="server" StoreID="StoreDetalle" Border="false" Header="false" Height="200" Hidden ="false">
                                                                <ColumnModel runat="server">
                                                                    <Columns>
                                                                        <ext:Column DataIndex="ID" Header="ID" Width="70" Hidden="true"/>
                                                                        <ext:Column DataIndex="Cantidad" Header="Cantidad" Width="130" />
                                                                        <ext:NumberColumn DataIndex="Monto" Header="Monto" Width="120" Format="$0,0.00" Align="Right">
                                                                                <Renderer Format="UsMoney" />
                                                                        </ext:NumberColumn>
                                                                    </Columns>
                                                                </ColumnModel>
                                                                <SelectionModel>
                                                                    <ext:RowSelectionModel SingleSelect="true" />
                                                                </SelectionModel>
                                                                <BottomBar>
                                                                    <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="StoreDetalle" DisplayInfo="true"
                                                                        DisplayMsg="{0} - {1} de {2}" />
                                                                </BottomBar>
                                                            </ext:GridPanel>
                                                        </Items>
                                                    </ext:Panel>
                                                    <ext:Panel runat="server" Layout="FitLayout" Width="550" Height="5" Border="false" />
                                                </Items>
                                                
                                                <Buttons>
                                                    
                                                </Buttons>
                                            </ext:FieldSet>
                                        </Items>
                                        <Items>
                                            <ext:Panel ID="pnlBotones" runat="server" Border="false" Header="false" LabelAlign="Right"
                                                Layout="FitLayout" Hidden="true" Padding="10">
                                                <Items>
                                                    <ext:Label runat="server"  ID="lblDescripcionRegla" Cls="descripcion" FieldLabel="" LabelAlign="Left"  
                                                        Text="* Ocurrió un error al generar los certificados, por favor intente nuevamente.">
                                                    </ext:Label>
                                                    <ext:Hidden runat="server" Flex="1" />
                                                    <ext:Panel runat="server" Layout="FitLayout" Width="5" Height="5" Border="false" />
                                                    <ext:Button ID="btnExportExcel" runat="server" Text="Descargar Certificados" Icon="Disk"  >
                                                        <DirectEvents>
                                                            <Click OnEvent="Download" IsUpload="true"
                                                                After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                                    e.stopEvent(); 
                                                                    Lealtad.StopMask();" />
                                                        </DirectEvents>
                                                    </ext:Button>
                                                    <ext:Panel runat="server" Layout="FitLayout" Width="5" Height="5" Border="false" />
                                                    <ext:Button ID="btnReintentar" runat="server" Text="Reintentar" Icon="ArrowRefresh" >
                                                        <DirectEvents>
                                                            <Click OnEvent="Reintentar" IsUpload="true" >
                                                                <%--After="Ext.net.Mask.show({ msg : 'Enviando solicitud...' });e.stopEvent(); Lealtad.StopMask();" >--%>
                                                                <EventMask ShowMask="true" Msg="ReGenerando Lote..." MinDelay="500" />
                                                            </Click>

                                                        </DirectEvents>
                                                    </ext:Button>
                                                    <ext:Panel runat="server" Layout="FitLayout" Width="10" Height="5" Border="false" />
                                                </Items>
                                            </ext:Panel>
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
