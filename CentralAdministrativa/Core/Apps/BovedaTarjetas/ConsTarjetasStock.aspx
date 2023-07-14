<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" ValidateRequest="false"
    CodeBehind="ConsTarjetasStock.aspx.cs" Inherits="BovedaTarjetas.ConsTarjetasStock" %>

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

        var prepareToolbar = function (grid, toolbar, rowIndex, record) {
            if (record.get("NombreArchivo") == "" ||
                record.get("RutaArchivo") == "") {
                toolbar.items.get(0).hide();
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center Split="true">
            <ext:GridPanel ID="GridSolicitudesStock" runat="server" StripeRows="true" Header="false" Border="false"
                Layout="FitLayout" AutoScroll="true">
                <Store>
                    <ext:Store ID="StoreSolicitudesStock" runat="server" RemoteSort="true" AutoLoad="false">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Peticion">
                                <Fields>
                                    <ext:RecordField Name="ID_Peticion" />
                                    <ext:RecordField Name="FechaSolicitud" />
                                    <ext:RecordField Name="NumLote" />
                                    <ext:RecordField Name="Cantidad" />
                                    <ext:RecordField Name="EstatusPeticion" />                                    
                                    <ext:RecordField Name="EstatusProcesamiento" />
                                    <ext:RecordField Name="Emisor" />
                                    <ext:RecordField Name="Producto" />
                                    <ext:RecordField Name="TipoManufactura" />
                                    <ext:RecordField Name="NombreArchivo" />
                                    <ext:RecordField Name="RutaArchivo" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <TopBar>
                    <ext:Toolbar runat="server" LabelWidth="70" LabelAlign="Right">
                        <Items>
                            <ext:DateField ID="dfFechaInicio" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                Format="dd/MM/yyyy" Width="200" EnableKeyEvents="true" AllowBlank="false">
                                <CustomConfig>
                                    <ext:ConfigItem Name="endDateField" Value="#{dfFechaFin}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:DateField ID="dfFechaFin" runat="server" Vtype="daterange" FieldLabel="Fecha Final"
                                Width="200" Format="dd/MM/yyyy" EnableKeyEvents="true" AllowBlank="false">
                                <CustomConfig>
                                    <ext:ConfigItem Name="startDateField" Value="#{dfFechaInicio}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:ToolbarSeparator runat="server" />
                            <ext:Button ID="btnBuscarSolics" runat="server" Text="Buscar" Icon="Magnifier" Width="70">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscarSolics_Click" Timeout="360000"
                                        Before="var valid1 = #{dfFechaInicio}.isValid(); 
                                        if (!valid1) {
                                            var valid = valid1; }
                                        else {
                                            var valid2 = #{dfFechaFin}.isValid();
                                            if (!valid2) {
                                                var valid = valid2; }
                                        } return valid; ">
                                        <EventMask ShowMask="true" Msg="Buscando Solicitudes..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarSeparator runat="server" />
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh" Width="70">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarSeparator runat="server" />
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <ColumnModel runat="server">
                    <Columns>
                        <ext:Column DataIndex="ID_Peticion" Hidden="true" />
                        <ext:DateColumn Header="Fecha de Solicitud" Sortable="true" DataIndex="FechaSolicitud"
                            Format="yyyy-MM-dd HH:mm:ss" Width="120"/>
                        <ext:Column Header="Lote" Sortable="true" DataIndex="NumLote" Width="100" />
                        <ext:Column Header="Cantidad" Sortable="true" DataIndex="Cantidad" Width="100"/>
                        <ext:Column Header="Estatus de Petición" Sortable="true" DataIndex="EstatusPeticion" Width="200"/>
                        <ext:Column Header="Estatus de Procesamiento" Sortable="true" DataIndex="EstatusProcesamiento" Width="200"/>
                        <ext:Column Header="Emisor" Sortable="true" DataIndex="Emisor" Width="100"/>
                        <ext:Column Header="Producto" Sortable="true" DataIndex="Producto" Width="100"/>
                        <ext:Column Header="Tipo de Manufactura" Sortable="true" DataIndex="TipoManufactura" Width="150" />
                        <ext:Column Hidden="true" DataIndex="NombreArchivo" />
                        <ext:Column Hidden="true" DataIndex="RutaArchivo" />
                        <ext:CommandColumn Header="Acción" Width="60">
                            <PrepareToolbar Fn="prepareToolbar" />
                            <Commands>
                                <ext:GridCommand Icon="PageWhitePut" CommandName="Download">
                                    <ToolTip Text="Descargar Archivo Tarjetas" />
                                </ext:GridCommand>
                            </Commands>
                        </ext:CommandColumn>
                    </Columns>
                </ColumnModel>
                <DirectEvents>
                    <Command OnEvent="EjecutarComando" IsUpload="true">
                        <Confirmation ConfirmRequest="true" Title="Confirmación"
                            Message="¿Deseas descargar este archivo?" />
                        <ExtraParams>
                            <ext:Parameter Name="NombreArchivo" Value="record.data['NombreArchivo']" Mode="Raw" />
                            <ext:Parameter Name="RutaArchivo" Value="record.data['RutaArchivo']" Mode="Raw" />
                        </ExtraParams>
                    </Command>
                </DirectEvents>
                <SelectionModel>
                    <ext:RowSelectionModel runat="server" SingleSelect="true" />
                </SelectionModel>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingSolicitudesStock" runat="server" StoreID="StoreSolicitudesStock" DisplayInfo="true"
                        DisplayMsg="Mostrando Solicitudes {0} - {1} de {2}" HideRefresh="true" />
                </BottomBar>
            </ext:GridPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
