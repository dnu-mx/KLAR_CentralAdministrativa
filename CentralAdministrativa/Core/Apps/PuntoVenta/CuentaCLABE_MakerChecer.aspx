<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="CuentaCLABE_MakerChecer.aspx.cs" Inherits="TpvWeb.CuentaCLABE_MakerChecer" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
   
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Store ID="StoreSubemisores" runat="server">
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
            <ext:Panel ID="PanelEjecutor" Title="Personas Morales" runat="server" Width="350" Border="false" LabelWidth="120">
                <Items>
                    <ext:FormPanel ID="FormPanelIzqCbe" runat="server" Layout="FormLayout" Border="false" FormGroup="true" AutoHeight="true" Padding="10">
                        <Items>
                            <ext:ComboBox ID="cBoxSubEmisor" runat="server" FieldLabel="SubEmisor <span style='color:red;'>*   </span>" 
                                ListWidth="250" Width="220" StoreID="StoreSubemisores" ValueField="ID_Colectiva" DisplayField="NombreORazonSocial"
                                Mode="Local" AutoSelect="true" Editable="true" ForceSelection="true" MinChars="1" TypeAhead="true"
                                MatchFieldWidth="false" Name="colClienteSolic" AllowBlank="false">
                                <DirectEvents>
                                    <Select OnEvent="EstableceProductosSolic" Before="#{cBoxProductoEjec}.clearValue();" />
                                </DirectEvents>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cBoxProductoEjec" runat="server" FieldLabel="Producto <span style='color:red;'>*   </span>"
                                ListWidth="250" Width="220" DisplayField="Descripcion" ValueField="ID_Producto" AllowBlank="false">
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
                            <ext:TextField ID="txtTarjeta" FieldLabel="Tarjeta <span style='color:red;'>*   </span>" runat="server" MaskRe="[0-9]"
                                MinLength="16" MaxLength="16" AllowBlank="false" Width="220" />
                            <ext:TextField ID="txtCLABE" runat="server" FieldLabel="Cuenta CLABE <span style='color:red;'>*   </span>"
                                MinLength="18" MaxLength="18" Width="220" MaskRe="[0-9]" AllowBlank="false">
                            </ext:TextField>
                            <ext:Panel runat="server" Layout="FitLayout" Height="20" Border="false" />
                            <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false" LabelWidth="5">
                                <Defaults>
                                    <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                </Defaults>
                                <LayoutConfig>
                                    <ext:HBoxLayoutConfig Align="Top" />
                                </LayoutConfig>
                                <Items>
                                    <ext:Label runat="server" LabelSeparator=" " Width="230" />
                                    <ext:Hidden runat="server" Flex="1" Width="25" />
                                    <ext:Label runat="server" FieldLabel="<span style='color:red;'>*   </span>"
                                        Text="Obligatorios" LabelSeparator=" " StyleSpec="font-style: italic;font-family:segoe ui;font-size: 11px;" />
                                </Items>
                            </ext:Panel>
                        </Items>
                        <Buttons>
                            <ext:Button ID="btnRegistrar" runat="server" Text="Registrar" Icon="PageAttach">
                                <DirectEvents>
                                    <Click OnEvent="btnRegistrar_Click" Before="var valid= #{FormPanelIzqCbe}.getForm().isValid(); 
                                        if (!valid) { return false; }">
                                        <EventMask ShowMask="true" Msg="Registrando Solicitud..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnRegAut" runat="server" Text="Registrar y Autorizar" Icon="Tick">
                                <Listeners>
                                    <Click Handler="var valid= #{FormPanelIzqCbe}.getForm().isValid(); 
                                        if (!valid) { return false; } else {
                                            Ext.Msg.confirm('Confirmación',
                                                '¿Estás seguro de solicitar y autorizar la Cuenta CLABE?',
                                                function (btn) {
                                                    if (btn == 'yes') {
                                                        Ext.net.Mask.show({ msg: 'Registrando y Autorizando Solicitud...' });
                                                        CtaCLABE_PM.RegistraYAutorizaSolicitud();
                                                    }
                                                }
                                            );
                                        }">
                                        <%--<EventMask ShowMask="true" Msg="Registrando y Autorizando Solicitud..." MinDelay="500" />--%>
                                    </Click>
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click" />
                                </DirectEvents>
                            </ext:Button>
                        </Buttons>
                    </ext:FormPanel>
                </Items>
            </ext:Panel>
        </West>
        <Center Split="true">
            <ext:GridPanel ID="GridPanelAutorizaciones" runat="server" StripeRows="true" Border="false" Layout="FitLayout" AutoScroll="true"
                Title="Autorización de Solicitudes" AutoExpandColumn="NumeroTarjeta">
                <Store>
                    <ext:Store ID="StoreClabes" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="IdTarjeta">
                                <Fields>
                                    <ext:RecordField Name="IdTarjeta" />
                                    <ext:RecordField Name="ID_EjecutorAutorizador" />
                                    <ext:RecordField Name="NumeroTarjeta" />
                                    <ext:RecordField Name="NuevoValor" />
                                    <ext:RecordField Name="Ejecutor" />
                                    <ext:RecordField Name="FechaHora" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <TopBar>
                    <ext:Toolbar runat="server">
                        <Items>
                            <ext:ComboBox ID="cBoxClienteAut" runat="server" EmptyText="Subemisor" ListWidth="200"
                                Width="150" DisplayField="NombreORazonSocial" Mode="Local" ValueField="ID_Colectiva"
                                AutoSelect="true" Editable="true" ForceSelection="true" MinChars="1" TypeAhead="true"
                                MatchFieldWidth="false" Name="colClienteAut" AllowBlank="false" StoreID="StoreSubemisores">
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
                                        Before="if (!#{cBoxClienteAut}.isValid() || !#{cBoxProductoAut}.isValid())
                                        { return false; } else { #{GridPanelAutorizaciones}.getStore().removeAll();
                                        return true; }">
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
                        <ext:NumberColumn Header="ID_EjecutorAutorizador" DataIndex="ID_EjecutorAutorizador" Width="110" Align="Right"
                            Hidden="true" />
                        <ext:NumberColumn Header="IdTarjeta" DataIndex="IdTarjeta" Width="110" Align="Right"
                            Hidden="true" />
                        <ext:Column Header="Tarjeta" DataIndex="NumeroTarjeta" />
                        <ext:Column Header="Cuenta CLABE Solicitada" Width="150" DataIndex="NuevoValor" />
                        <ext:Column Header="Usuario que Solicita" Width="150" DataIndex="Ejecutor" />
                        <ext:DateColumn Header="Fecha de Solicitud" Width="150" DataIndex="FechaHora" Format="yyyy-MM-dd HH:mm:ss"  />
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
                    <ext:PagingToolbar ID="PagingTBSolics" runat="server" StoreID="StoreClabes" DisplayInfo="true"
                        DisplayMsg="Solicitudes de Cambio {0} - {1} de {2}" HideRefresh="true" />
                </BottomBar>
            </ext:GridPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
