<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="CuentasVIPConsulta.aspx.cs" Inherits="Administracion.CuentasVIPConsulta" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
   
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
    <Center Split="true">
        <ext:GridPanel ID="GridTarjetasVIP" runat="server" StripeRows="true" Border="false" Layout="FitLayout" AutoScroll="true"
            Title="Cuentas VIP">
            <Store>
                <ext:Store ID="StoreParamsTarjetasVIPDetalle" runat="server">
                    <DirectEventConfig IsUpload="true"/>
                    <Reader>
                        <ext:JsonReader IDProperty="ID_TarjetaVip">
                            <Fields>
                                <ext:RecordField Name="ID_TarjetaVip" />
                                <ext:RecordField Name="NoCuenta" />
                                <ext:RecordField Name="Tarjeta" />
                                <ext:RecordField Name="Ejecutor" />
                                <ext:RecordField Name="Autorizador" />
                                <ext:RecordField Name="MontoSolicitado" />
                                <ext:RecordField Name="MontoAcumulado" />
                                <ext:RecordField Name="FechaSolicitud" />
                                <ext:RecordField Name="FechaResolucion" />
                                <ext:RecordField Name="EstatusTarjetaVip" />
                            </Fields>
                        </ext:JsonReader>
                    </Reader>
                </ext:Store>
            </Store>
            <TopBar>
                <ext:Toolbar runat="server">
                    <Items>
                        <ext:Hidden ID="FormatType" runat="server" />
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
                                <Click OnEvent="btnBuscarTajetasVIP_Click" Timeout="360000"
                                    Before="if (!#{cBoxClienteAut}.isValid() || !#{cBoxProductoAut}.isValid())
                                        { return false; } else { #{GridTarjetasVIP}.getStore().removeAll();
                                        return true; }">
                                    <EventMask ShowMask="true" Msg="Buscando Tarjetas VIP..." MinDelay="500" />
                                </Click>
                            </DirectEvents>
                        </ext:Button>
                        <ext:ToolbarSeparator runat="server" />
                        <ext:ToolbarFill runat="server" />
                        <ext:ToolbarSeparator runat="server" />
                        <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="false">
                            <DirectEvents>
                                <Click OnEvent="ExportGridToExcel" IsUpload="true" 
                                    After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                        e.stopEvent(); 
                                        ConsultaCtasVIP.StopMask();">
                                    <ExtraParams>
                                        <ext:Parameter Name="GridToExport" Value="Ext.encode(#{GridTarjetasVIP}.getRowsValues({selectedOnly : false}))" Mode="Raw" />
                                    </ExtraParams>
                                </Click>
                            </DirectEvents>
                        </ext:Button>
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
                    <ext:Column Header="#" DataIndex="ID_TarjetaVip" Width="60" Align="Left" />
                    <ext:Column Header="Tarjeta" Width="110" DataIndex="Tarjeta" Align="Right" />
                    <ext:Column Header="Cuenta" Width="110" DataIndex="NoCuenta" Align="Right" />
                    <ext:Column Header="Autorizador" Width="180" DataIndex="Autorizador" Align="Left" />
                    <ext:Column Header="Ejecutor" Width="180" DataIndex="Ejecutor" Align="Left" />
                    <ext:NumberColumn Header="Monto Solicitado" DataIndex="MontoSolicitado" Width="105" Align="Right">
                        <Renderer Format="UsMoney" />
                    </ext:NumberColumn>
                    <ext:NumberColumn Header="Monto Acumulado" DataIndex="MontoAcumulado" Width="110" Align="Right">
                        <Renderer Format="UsMoney" />
                    </ext:NumberColumn>
                    <ext:DateColumn Header="Fecha Solicitud" Width="140" DataIndex="FechaSolicitud" Align="Center"
                        Format="yyyy-MM-dd HH:mm:ss"/>
                    <ext:DateColumn Header="Fecha Resolución" Width="140" DataIndex="FechaResolucion" Align="Center"
                        Format="yyyy-MM-dd HH:mm:ss"/>
                    <ext:Column Header="Estatus" Width="100" DataIndex="EstatusTarjetaVip" Align="Center" />
                </Columns>
            </ColumnModel>
            <LoadMask ShowMask="false" />
            <BottomBar>
                <ext:PagingToolbar ID="PagingTBSolics" runat="server" StoreID="StoreParamsTarjetasVIPDetalle" DisplayInfo="true"
                    DisplayMsg="Solicitudes de Cambio {0} - {1} de {2}" HideRefresh="true" />
            </BottomBar>
        </ext:GridPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>