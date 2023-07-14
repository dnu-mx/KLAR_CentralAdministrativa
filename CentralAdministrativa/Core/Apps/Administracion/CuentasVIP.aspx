<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="CuentasVIP.aspx.cs" Inherits="Administracion.CuentasVIP" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Hidden ID="hdnIdPlantilla" runat="server" />
    <ext:Hidden ID="hdnTarjetaHabiente" runat="server" />
    <ext:Store ID="StoreSubEmisores" runat="server">
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
    <ext:BorderLayout runat="server">
        <West Split="true">
            <ext:Panel ID="PanelIzq" runat="server" Width="350" Border="false" LabelWidth="120">
                <Content>
                    <ext:BorderLayout ID="LeftBorderLayout" runat="server">
                        <North Split="true">
                            <ext:FormPanel ID="FormPanelBusqueda" runat="server" Title="Consulta de Cuentas" Height="160" Frame="true" 
                                Border="false" Padding="10">
                                <Items>
                                    <ext:ComboBox ID="cBoxClienteSolic" runat="server" FieldLabel="SubEmisor" ListWidth="200" Width="200"
                                        StoreID="StoreSubEmisores" ValueField="ID_Colectiva" DisplayField="NombreORazonSocial"
                                        Mode="Local" AutoSelect="true" Editable="true" ForceSelection="true" MinChars="1" TypeAhead="true"
                                        MatchFieldWidth="false" Name="colClienteSolic" AllowBlank="false">
                                        <DirectEvents>
                                            <Select OnEvent="EstableceProductosSolic" Before="#{cBoxProductoSolic}.clearValue();" />
                                        </DirectEvents>
                                    </ext:ComboBox>
                                    <ext:ComboBox ID="cBoxProductoSolic" runat="server" FieldLabel="Producto" ListWidth="200"
                                        Width="200" DisplayField="Descripcion" ValueField="ID_Producto" AllowBlank="false">
                                        <Store>
                                            <ext:Store ID="StoreProductosSolic" runat="server">
                                                <Reader>
                                                    <ext:JsonReader IDProperty="ID_Producto">
                                                        <Fields>
                                                            <ext:RecordField Name="ID_Producto" />
                                                            <ext:RecordField Name="Clave" />
                                                            <ext:RecordField Name="Descripcion" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Store>
                                    </ext:ComboBox>
                                    <ext:TextField ID="txtTarjeta" FieldLabel="Tarjeta" runat="server" MaskRe="[0-9]" MinLength="16"
                                        MaxLength="16" AllowBlank="false" Width="200" />
                                </Items>
                                <Buttons>
                                    <ext:Button ID="btnValidar" runat="server" Text="Validar" Icon="Tick">
                                        <DirectEvents>
                                            <Click OnEvent="btnValidar_Click" Before="if (!#{cBoxClienteSolic}.isValid() ||
                                                !#{cBoxProductoSolic}.isValid() || !#{txtTarjeta}.isValid()) { return false; }
                                                else { #{txtMontoSolicitado}.clear(); #{txtMontoAcumulado}.clear();
                                                #{txtRazon}.clear(); return true; }">
                                                <EventMask ShowMask="true" Msg="Validando Tarjeta..." MinDelay="500" />
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnLimpiarSolic" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                        <DirectEvents>
                                            <Click OnEvent="btnLimpiarSolic_Click" />
                                        </DirectEvents>
                                    </ext:Button>
                                </Buttons>
                            </ext:FormPanel>
                        </North>
                        <Center Split="true">
                            <ext:FormPanel ID="FormPanelSolic" runat="server" Title="Solicitud de Cambio" LabelAlign="Top" Border="false">
                                <Items>
                                    <ext:FieldSet runat="server" Title="Captura los datos" Layout="AnchorLayout" Padding="10">
                                        <Items>
                                            <ext:TextField ID="txtMontoSolicitado" runat="server" FieldLabel="Monto máximo de cash in al mes (UDIS) <span style='color:red;'>*   </span>"
                                                MaxLength="20" Width="310" MaskRe="[0-9]" AllowBlank="false">
                                                <Listeners>
                                                    <Change Handler="var imp = Ext.util.Format.number(this.getValue(), '0,0');
                                                        this.setValue(imp);" />
                                                </Listeners>
                                            </ext:TextField>
                                            <ext:TextField ID="txtMontoAcumulado" runat="server" FieldLabel="Monto máximo acumulado en el mes (UDIS) <span style='color:red;'>*   </span>"
                                                MaxLength="20" Width="310" MaskRe="[0-9]" AllowBlank="false">
                                                <Listeners>
                                                    <Change Handler="var imp = Ext.util.Format.number(this.getValue(), '0,0');
                                                        this.setValue(imp);" />
                                                </Listeners>
                                            </ext:TextField>
                                            <ext:TextArea ID="txtRazon" runat="server" FieldLabel="Motivo por el que se solicita el cambio a VIP <span style='color:red;'>*   </span>"
                                                AllowBlank="false" MaxLength="5000" Width="310" />
                                        </Items>
                                        <Buttons>
                                            <ext:Button ID="btnSolic" runat="server" Text="Solicitar Cambio" Icon="PageWhiteStar">
                                                <DirectEvents>
                                                    <Click OnEvent="btnSolic_Click" Before="if (!#{txtMontoSolicitado}.isValid() || !#{txtMontoAcumulado}.isValid()
                                                        || !#{txtRazon}.isValid()) { return false; } return true;">
                                                        <EventMask ShowMask="true" Msg="Registrando Solicitud..." MinDelay="500" />
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Buttons>
                                    </ext:FieldSet>
                                </Items>
                            </ext:FormPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </West>
        <Center Split="true">
            <ext:GridPanel ID="GridPanelAutorizaciones" runat="server" StripeRows="true" Border="false" Layout="FitLayout" AutoScroll="true"
                Title="Autorización de Solicitudes" AutoExpandColumn="Motivo">
                <Store>
                    <ext:Store ID="StoreParamsPorAutorizar" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Plantilla">
                                <Fields>
                                    <ext:RecordField Name="ID_Plantilla" />
                                    <ext:RecordField Name="ID_EA_NivelCtaCumpl" />
                                    <ext:RecordField Name="ID_VPMA_NivelCtaCumpl" />
                                    <ext:RecordField Name="ID_EA_NivelCtaCumplPers" />
                                    <ext:RecordField Name="ID_PMA_NivelCtaCumplPers" />
                                    <ext:RecordField Name="ID_EA_SaldoMaxPers" />
                                    <ext:RecordField Name="ID_PMA_SaldoMaxPers" />
                                    <ext:RecordField Name="ID_EA_MaxAbonoPers" />
                                    <ext:RecordField Name="ID_PMA_MaxAbonoPers" />
                                    <ext:RecordField Name="MontoSolicitado" />
                                    <ext:RecordField Name="MontoAcumulado" />
                                    <ext:RecordField Name="Motivo" />
                                    <ext:RecordField Name="RespListaNegra" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <TopBar>
                    <ext:Toolbar runat="server">
                        <Items>
                            <ext:ComboBox ID="cBoxClienteAut" runat="server" EmptyText="SubEmisor" ListWidth="200"
                                Width="150" DisplayField="NombreORazonSocial" Mode="Local" ValueField="ID_Colectiva"
                                AutoSelect="true" Editable="true" ForceSelection="true" MinChars="1" TypeAhead="true"
                                MatchFieldWidth="false" Name="colClienteAut" AllowBlank="false" StoreID="StoreSubEmisores">
                                <DirectEvents>
                                    <Select OnEvent="EstableceProductosAut" Before="#{cBoxProductoAut}.clearValue();" />
                                </DirectEvents>
                            </ext:ComboBox>
                            <ext:ToolbarSpacer runat="server" Width="3" />
                            <ext:ComboBox ID="cBoxProductoAut" runat="server" EmptyText="Producto" ListWidth="200"
                                Width="150" DisplayField="Descripcion" ValueField="ID_Producto" AllowBlank="false">
                                <Store>
                                    <ext:Store ID="StoreProductosAut" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Producto">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Producto" />
                                                    <ext:RecordField Name="Clave" />
                                                    <ext:RecordField Name="Descripcion" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                            </ext:ComboBox>
                            <ext:ToolbarSpacer runat="server" Width="3" />
                            <ext:TextField ID="txtTarjetaAut" EmptyText="Tarjeta" runat="server" MaskRe="[0-9]" MinLength="16"
                                MaxLength="16" Width="110" />
                            <ext:Button ID="btnBuscarAut" runat="server" Text="Buscar" Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscarAut_Click" Timeout="360000"
                                        Before="if (!#{cBoxClienteAut}.isValid() || !#{cBoxProductoAut}.isValid()
                                        || !#{txtTarjetaAut}.isValid()) { return false; } else {
                                        #{GridPanelAutorizaciones}.getStore().removeAll(); return true; }">
                                        <EventMask ShowMask="true" Msg="Buscando Solicitudes..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarSeparator runat="server" />
                            <ext:ToolbarFill runat="server"/>
                            <ext:ToolbarSeparator runat="server" />
                            <ext:Button ID="btnLimpiarAut" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiarAut_Click" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <ColumnModel runat="server">
                    <Columns>
                        <ext:Column runat="server" Hidden="true" DataIndex="ID_Plantilla" />
                        <ext:Column runat="server" Hidden="true" DataIndex="ID_EA_NivelCtaCumpl" />
                        <ext:Column runat="server" Hidden="true" DataIndex="ID_VPMA_NivelCtaCumpl" />
                        <ext:Column runat="server" Hidden="true" DataIndex="ID_EA_NivelCtaCumplPers" />
                        <ext:Column runat="server" Hidden="true" DataIndex="ID_PMA_NivelCtaCumplPers" />
                        <ext:Column runat="server" Hidden="true" DataIndex="ID_EA_SaldoMaxPers" />
                        <ext:Column runat="server" Hidden="true" DataIndex="ID_PMA_SaldoMaxPers" />
                        <ext:Column runat="server" Hidden="true" DataIndex="ID_EA_MaxAbonoPers" />
                        <ext:Column runat="server" Hidden="true" DataIndex="ID_PMA_MaxAbonoPers" />
                        <ext:NumberColumn Header="Monto Solicitado" DataIndex="MontoSolicitado" Width="110" Align="Right"
                            Format="0,0" />
                        <ext:NumberColumn Header="Monto Acumulado" DataIndex="MontoAcumulado" Width="110" Align="Right"
                            Format="0,0" />
                        <ext:Column Header="Resultado en Lista Negra" Sortable="true" DataIndex="RespListaNegra" Width="150"
                            Align="Right" />
                        <ext:Column Header="Motivo de la Solicitud" Width="200" DataIndex="Motivo" />
                        <ext:CommandColumn Width="60">
                            <Commands>
                                <ext:GridCommand Icon="Tick" CommandName="Autorizar">
                                    <ToolTip Text="Autorizar Solicitud" />
                                </ext:GridCommand>
                                <ext:GridCommand Icon="Cross" CommandName="Rechazar">
                                    <ToolTip Text="Rechazar Solicitud" />
                                </ext:GridCommand>
                            </Commands>
                        </ext:CommandColumn>
                    </Columns>
                </ColumnModel>
                <SelectionModel>
                    <ext:RowSelectionModel runat="server" SingleSelect="true" />
                </SelectionModel>
                <DirectEvents>
                    <Command OnEvent="EjecutarComandoSolicitudes">
                        <Confirmation ConfirmRequest="true"
                             Title="Confirmación" Message="¿Estás seguro de autorizar/rechazar la solicitud?" />
                        <ExtraParams>
                            <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                            <ext:Parameter Name="Values" Value="Ext.encode(record.data)" Mode="Raw" />
                        </ExtraParams>
                    </Command>
                </DirectEvents>
                <LoadMask ShowMask="false" />
                <BottomBar>
                    <ext:PagingToolbar ID="PagingTBSolics" runat="server" StoreID="StoreParamsPorAutorizar" DisplayInfo="true"
                        DisplayMsg="Solicitudes de Cambio {0} - {1} de {2}" />
                </BottomBar>
            </ext:GridPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
