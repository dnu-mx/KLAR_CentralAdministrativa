<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="EditarDatosCupon.aspx.cs" 
    Inherits="TpvWeb.EditarDatosCupon" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Window ID="WindowEditaOperacion" runat="server" Title="Editar Datos del Cupón" Hidden="true" Width="400" Height="370"
        Modal="true" Resizable="false">
        <Items>
            <ext:FormPanel ID="FormPanelEditaOperacion" runat="server" Padding="10" MonitorValid="true" 
                LabelAlign="Left" LabelWidth="150">
                <Items>
                    <ext:TextField ID="txtIdOperacion" runat="server" Hidden="true" />
                    <ext:TextField ID="txtCupon" runat="server" Hidden="true" />
                    <ext:DateField ID="dfFecha" runat="server" FieldLabel="Fecha/Hora" Format="yyyy-MM-dd HH:mm:ss"
                        Disabled="true" AnchorHorizontal="100%" />
                    <ext:TextField ID="txtPromocion" runat="server" FieldLabel="Promoción" Disabled="true"
                        AnchorHorizontal="100%" />
                    <ext:TextField ID="txtVigencia" runat="server" FieldLabel="Vigencia" Disabled="true"
                        AnchorHorizontal="100%" />
                    <ext:TextField ID="txtOrigenEmision" runat="server" FieldLabel="Origen de Emisión" Disabled="true"
                        AnchorHorizontal="100%" />
                    <ext:TextField ID="txtAutorizacion" runat="server" FieldLabel="Autorización" Disabled="true"
                        AnchorHorizontal="100%" />
                    <ext:TextField ID="txtOperador" runat="server" FieldLabel="Operador" Disabled="true"
                        AnchorHorizontal="100%" />
                    <ext:TextField ID="txtTicket" runat="server" FieldLabel="Ticket" MaxLength="20" AllowBlank="false"
                        AnchorHorizontal="100%" />
                    <ext:ComboBox ID="cmbFormaPago" runat="server" FieldLabel="Forma de Pago" AllowBlank="false"
                        AnchorHorizontal="100%">
                        <Items>
                            <ext:ListItem Text="Efectivo" Value="Efectivo" />
                            <ext:ListItem Text="Tarjeta" Value="Tarjeta" />
                        </Items>
                    </ext:ComboBox>
                    <ext:TextField ID="txtClaveCadena" runat="server" FieldLabel="Clave de la Cadena" Disabled="true"
                        AnchorHorizontal="100%" />
                    <ext:TextField ID="txtNombreCadena" runat="server" FieldLabel="Nombre de la Cadena" Disabled="true"
                        AnchorHorizontal="100%" />
                    <ext:TextField ID="txtSucursal" runat="server" FieldLabel="Sucursal" Disabled="true"
                        AnchorHorizontal="100%" />
                </Items>
            </ext:FormPanel>
        </Items>
        <FooterBar>
            <ext:Toolbar ID="ToolbarNuevoEvento" runat="server" EnableOverflow="true">
                <Items>
                    <ext:Button ID="btnGuardar" runat="server" Text="Guardar" Icon="Disk">
                        <DirectEvents>
                            <Click OnEvent="btnGuardar_Click" Before="var valid= #{FormPanelEditaOperacion}.getForm().isValid(); if (!valid) {} return valid;" />
                        </DirectEvents>
                    </ext:Button>
                </Items>
            </ext:Toolbar>
        </FooterBar>
    </ext:Window>
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true" Collapsible="true">
            <ext:FormPanel ID="FormPanelFiltros" Width="350" Title="Filtros de Búsqueda" runat="server"
                Border="false" Layout="Fit">
                <Content>
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
                        <SortInfo Field="NombreORazonSocial"  Direction="ASC"  />
                    </ext:Store>
                    <ext:Store ID="StorePromocion" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Evento">
                                <Fields>
                                    <ext:RecordField Name="ID_Evento" />
                                    <ext:RecordField Name="ClaveEvento" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="Descripcion"  Direction="ASC"  />
                    </ext:Store>
                </Content>
                <Items>
                    <ext:Panel ID="Panel1" runat="server" Layout="VBoxLayout" BodyPadding="5" LabelWidth="120" >
                        <Defaults>
                            <ext:Parameter Name="margin" Value="0 0 5 0" Mode="Value" />
                        </Defaults>
                        <LayoutConfig>
                            <ext:VBoxLayoutConfig Align="Center" />
                        </LayoutConfig>
                        <Items>
                            <ext:Panel ID="Panel3" runat="server" Height="10" Border="false" />
                            <ext:ComboBox ID="cmbCadena" runat="server" FieldLabel="Cadena" AllowBlank="false"
                                Resizable="true" ListWidth="350" Width="300" StoreID="StoreCadena"
                                MaxWidth="300" DisplayField="NombreORazonSocial" ValueField="ID_Colectiva"
                                Mode="Local" AutoSelect="true" ForceSelection="true" TypeAhead="true"
                                MinChars="1" MatchFieldWidth="false" Name="CadenasPRED">
                                <DirectEvents>
                                    <Select OnEvent="LlenaPromociones">
                                    </Select>
                                </DirectEvents>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cmbPromocion" runat="server" FieldLabel="Promoción" EmptyText="Todas"
                                 Resizable="true" ListWidth="350" Width="300" StoreID="StorePromocion"
                                DisplayField="Descripcion" ValueField="ID_Evento">
                                <Items>
                                    <ext:ListItem Text="( Todas )" Value="" />
                                </Items>
                            </ext:ComboBox>
                            <ext:TextField ID="txtClaveCupon" runat="server" FieldLabel="Número de Cupón"
                                MaxLength="50" Width="300" />
                        </Items>
                    </ext:Panel>
                </Items>
                <FooterBar>
                    <ext:Toolbar ID="Toolbar1" runat="server">
                        <Items>
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Before="var valid= #{FormPanelFiltros}.getForm().isValid(); if (!valid) {} return valid;">
                                        <EventMask ShowMask="true" Msg="Buscando Cupones..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>
            </ext:FormPanel>
        </West>
        <Center Split="true" Collapsible="false">
            <ext:FormPanel ID="FormPanelResultados" runat="server" Title="Cupones Obtenidos con los Filtros Seleccionados"
                Collapsed="false" Layout="Fit" AutoScroll="true">
                <Items>
                    <ext:GridPanel ID="GridPanelCupones" runat="server" StripeRows="true" Header="false" Border="false">
                        <Store>
                        <ext:Store ID="StoreCupones" runat="server" OnRefreshData="btnBuscar_Click" RemoteSort="true">
                                <DirectEventConfig IsUpload="true" />
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_Operacion">
                                        <Fields>
                                            <ext:RecordField Name="ID_Operacion" />
                                            <ext:RecordField Name="NumCupon" />
                                            <ext:RecordField Name="Fecha" Type="Date" />
                                            <ext:RecordField Name="Promocion" />
                                            <ext:RecordField Name="Vigencia" />
                                            <ext:RecordField Name="OrigenEmision" />
                                            <ext:RecordField Name="Autorizacion" />
                                            <ext:RecordField Name="Operador" />
                                            <ext:RecordField Name="Ticket" />
                                            <ext:RecordField Name="FormaPago" />
                                            <ext:RecordField Name="ClaveCadena" />
                                            <ext:RecordField Name="NombreCadena" />
                                            <ext:RecordField Name="Sucursal" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                                <DirectEventConfig IsUpload="true" />
                                <SortInfo Field="Fecha" />
                            </ext:Store>
                        </Store>
                        <LoadMask ShowMask="false" />
                        <ColumnModel ID="ColumnModel1" runat="server">
                            <Columns>
                                <ext:CommandColumn Width="25">
                                    <Commands>
                                        <ext:GridCommand Icon="PageWhiteEdit" CommandName="Edit">
                                            <ToolTip Text="Editar Datos del Cupón" />
                                        </ext:GridCommand>
                                    </Commands>
                                </ext:CommandColumn>
                                <ext:Column ColumnID="IdOperacion" Hidden="true" DataIndex="ID_Operacion" />
                                <ext:Column ColumnID="NumCupon" Hidden="true" DataIndex="NumCupon" />
                                <ext:DateColumn ColumnID="Fecha" Header="Fecha" Sortable="true" DataIndex="Fecha"
                                    Format="yyyy-MM-dd" Width="75" />
                                <ext:DateColumn ColumnID="Hora" Header="Hora" Sortable="true" DataIndex="Fecha"
                                    Format="HH:mm:ss" Width="65" />
                                <ext:Column ColumnID="Promocion" Header="Promoción" Sortable="true" DataIndex="Promocion"
                                    Width="200" />
                                <ext:Column ColumnID="Vigencia" Header="Vigencia" Sortable="true" DataIndex="Vigencia" 
                                    Width="75" />
                                <ext:Column ColumnID="OrigenEmision" Header="Origen de Emisión" Sortable="true" DataIndex="OrigenEmision"
                                    Width="120" />
                                <ext:Column ColumnID="Autorizacion" Header="Autorización" Sortable="true" DataIndex="Autorizacion" 
                                    Width="80" />
                                <ext:Column ColumnID="Operador" Header="Operador" Sortable="true" DataIndex="Operador"
                                    Width="80" />
                                <ext:Column ColumnID="Ticket" Header="Ticket" Sortable="true" DataIndex="Ticket" />
                                <ext:Column ColumnID="FormaPago" Header="Forma de Pago" Sortable="true" DataIndex="FormaPago" />
                                <ext:Column ColumnID="ClaveCadena" Header="Clave de la Cadena" Sortable="true" DataIndex="ClaveCadena"
                                    Width="120" />
                                <ext:Column ColumnID="NombreCadena" Header="Nombre de la Cadena" Sortable="true" DataIndex="NombreCadena"
                                    Width="120" />
                                <ext:Column ColumnID="Sucursal" Header="Sucursal" Sortable="true" DataIndex="Sucursal"
                                    Width="80" />
                            </Columns>
                        </ColumnModel>
                        <Plugins>
                            <ext:GridFilters runat="server" ID="GridFilters1" Local="true">
                                <Filters>
                                    <ext:DateFilter DataIndex="Fecha">
                                        <DatePickerOptions runat="server" TodayText="Hoy" />
                                    </ext:DateFilter>
                                </Filters>
                            </ext:GridFilters>
                        </Plugins>
                        <SelectionModel>
                            <ext:RowSelectionModel SingleSelect="true" />
                        </SelectionModel>
                        <DirectEvents>
                            <Command OnEvent="EventoComando">
                                <ExtraParams>
                                    <ext:Parameter Name="Values" Value="Ext.encode(record.data)" Mode="Raw" />
                                    <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                </ExtraParams>
                            </Command>
                        </DirectEvents>
                        <BottomBar>
                            <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreCupones" DisplayInfo="true"
                                DisplayMsg="Mostrando Cupones {0} - {1} de {2}" />
                        </BottomBar>
                    </ext:GridPanel>
                </Items>
            </ext:FormPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
