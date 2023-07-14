<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ConsultarSolicTarjetas.aspx.cs" Inherits="BovedaTarjetas.ConsultarSolicTarjetas" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true" Collapsible="true">
            <ext:FormPanel ID="FormPanel1" Width="320" Title="Selecciona los Filtros" runat="server" Padding="10"
                Border="false" Layout="FormLayout" LabelWidth="120">
                <Items>
                    <ext:TextField ID="txtNumLote" runat="server" FieldLabel="Número de Lote"
                        Width="170" MaskRe="[0-9]" />
                    <ext:MultiCombo ID="mcEmisor" runat="server" FieldLabel="Emisor(es)" Width="170" EmptyText="Todos">
                         <%--<Store>
                            <ext:Store ID="StoreTipoColectiva" runat="server">
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_TipoColectiva">
                                        <Fields>
                                            <ext:RecordField Name="ID_TipoColectiva" />
                                            <ext:RecordField Name="Clave" />
                                            <ext:RecordField Name="Descripcion" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                       <DirectEvents>
                            <Select OnEvent="EstableceColectivas" Before="#{cBoxGpoComercial}.clearValue();">
                                <EventMask ShowMask="true" Msg="Estableciendo Colectivas..." MinDelay="200" />
                            </Select>
                        </DirectEvents>--%>
                    </ext:MultiCombo>
                    <ext:MultiCombo ID="mcBIN" runat="server" FieldLabel="BIN(es)" Width="170" EmptyText="Todos" >
                        <%--<Store>
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
                        </Store>--%>
                    </ext:MultiCombo>
                    <ext:MultiCombo ID="mcTipoTarjeta" FieldLabel="Tipo(s) de Tarjeta" EmptyText="Todos" Width="170" 
                        runat="server">
                        <%--<Listeners>
                            <Select Handler="#{cBoxMotivoRechazo}.clear(); 
                                if (this.getValue() == 'ANOK') { #{cBoxMotivoRechazo}.setDisabled(false); }
                                else { #{cBoxMotivoRechazo}.setDisabled(true); }" />
                        </Listeners>--%>
                    </ext:MultiCombo>
                    <ext:MultiCombo ID="mcDisenyoTarj" FieldLabel="Diseño(s) de Tarjeta" EmptyText="Todos" Width="170"
                        runat="server">
                       
                    </ext:MultiCombo>
                </Items>
                <FooterBar>
                    <ext:Toolbar runat="server">
                        <Items>
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                              <%--  <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click" />
                                </DirectEvents>--%>
                            </ext:Button>
                        <%--    <ext:Button ID="btnBuscarHide" runat="server" Hidden="true">
                                <Listeners>
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Operaciones...' });
                                        #{GridPanelOperaciones}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>--%>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." Icon="Magnifier">
                                <%--<DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                        Before="if (!#{FormPanel1}.getForm().isValid()) { return false; }
                                        else if (!#{dfFechaInicio}.getValue() && !#{dfFechaFin}.getValue()
                                        && !#{dfFechaIniPresen}.getValue() && !#{dfFechaFinPresen}.getValue()) 
                                        { Ext.Msg.alert('Aviso', 'Ingresa al menos una fecha de filtro.');
                                        return false; }
                                        else { resetToolbar(#{PagingToolBar1});
                                        #{GridPanelOperaciones}.getStore().sortInfo = null; }">
                                    </Click>
                                </DirectEvents>--%>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>
            </ext:FormPanel>
        </West>
        <Center Split="true" Collapsible="false">
            <ext:FormPanel ID="FormPanelSolics" runat="server" Layout="FitLayout" Title="Solicitudes obtenidas con los filtros seleccionados">
                <Items>
                    <ext:GridPanel ID="GridPanelSolics" runat="server" StripeRows="true" Header="false" Border="false">
                        <LoadMask ShowMask="false" />
                        <Store>
                            <ext:Store ID="StoreSolicitudes" runat="server" AutoLoad="false">                           
                                <DirectEventConfig IsUpload="true"/>
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_Lote">
                                        <Fields>
                                            <ext:RecordField Name="ID_Lote" />
                                            <ext:RecordField Name="Emisor" />
                                            <ext:RecordField Name="BIN" />
                                            <ext:RecordField Name="TipoTarjeta" />
                                            <ext:RecordField Name="DisenyoTarjeta" />
                                            <ext:RecordField Name="MotivoRechazo" />
                                            <ext:RecordField Name="NumTarjetas" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <ColumnModel ID="ColumnModel1" runat="server">
                            <Columns>
                                <ext:CommandColumn Width="30">
                                    <Commands>
                                        <ext:GridCommand Icon="PageWhitePut" CommandName="Download">
                                            <ToolTip Text="Descargar Reporte" />
                                        </ext:GridCommand>
                                    </Commands>
                                </ext:CommandColumn>
                                <ext:Column Header="Número de Lote" Sortable="true" DataIndex="ID_Lote" />
                                <ext:Column Header="Emisor" Sortable="true" DataIndex="Emisor" />
                                <ext:Column Header="BIN" Sortable="true" DataIndex="BIN"/>
                                <ext:Column Header="Tipo de Tarjeta" Sortable="true" DataIndex="TipoTarjeta" />
                                <ext:Column Header="Diseño de Tarjeta" Sortable="true" DataIndex="DisenyoTarjeta"
                                    Width="120" />
                                <ext:Column Header="Cantidad de tarjetas solicitadas" Sortable="true" 
                                    DataIndex="NumTarjetas" Width="200" />
                            </Columns>
                        </ColumnModel>
                        <SelectionModel>
                            <ext:RowSelectionModel SingleSelect="true"  />
                        </SelectionModel>
                       <%-- <DirectEvents>
                            <Command OnEvent="EjecutarComando">
                                <ExtraParams>
                                    <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                    <ext:Parameter Name="ID_Lote" Value="Ext.encode(record.data.ID_Lote)" Mode="Raw" />
                                </ExtraParams>
                            </Command>
                        </DirectEvents>--%>
                        <Plugins>
                            <ext:GridFilters runat="server" Local="true" FiltersText="Filtros">
                                <Filters>
                                    <ext:NumericFilter DataIndex="ID_Lote" />
                                    <ext:StringFilter DataIndex="Emisor" />
                                    <ext:StringFilter DataIndex="BIN" />
                                    <ext:StringFilter DataIndex="TipoTarjeta" />
                                    <ext:NumericFilter DataIndex="DisenyoTarjeta" />
                                    <ext:StringFilter DataIndex="NumTarjetas" />
                                </Filters>
                            </ext:GridFilters>
                        </Plugins>
                        <BottomBar>
                            <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreSolicitudes" DisplayInfo="true"
                                DisplayMsg="Mostrando Solicitudes {0} - {1} de {2}" HideRefresh="true" />
                        </BottomBar>
                    </ext:GridPanel>
                </Items>
            </ext:FormPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>

