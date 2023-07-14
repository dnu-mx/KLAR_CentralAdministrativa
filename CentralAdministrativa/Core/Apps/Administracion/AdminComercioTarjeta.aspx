<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="AdminComercioTarjeta.aspx.cs" Inherits="Administracion.AdminComercioTarjeta" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Hidden ID="hdnValorAfiliacion" runat="server" />
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
     <ext:Window ID="WdwNuevoComercioTarjeta" runat="server" Title="Nuevo Comercio-Tarjeta" Width="420" Height="185" Hidden="true"
        Modal="true" Resizable="false" Icon="Add">
        <Items>
            <ext:FormPanel ID="FormPanelNuevoProd" runat="server" Padding="3" LabelAlign="Right" LabelWidth="130" Border="false">
                <Items>
                    <ext:FieldSet runat="server" Title="Comercio-Tarjeta" Layout="FormLayout">
                        <Items>
                            <ext:NumberField ID="NumAfiliacion" runat="server" FieldLabel="<span style='color:red;'>*</span> Número de Afiliación"
                                MaxLength="7" Width="235" AllowBlank="false" MaskRe="[0-9]" />
                            <ext:NumberField ID="NumTarjeta" runat="server" FieldLabel="<span style='color:red;'>*</span>Número de Tarjeta"
                                MinLength="16" MaxLength="16" Width="235" AllowBlank="false" MaskRe="[0-9]" /> 
                        </Items>
                    </ext:FieldSet>
                </Items>
                <Buttons>
                    <ext:Button ID="btnCancelar" runat="server" Text="Cancelar" Icon="Cancel">
                        <DirectEvents>
                            <Click OnEvent="btnCancelar_Click" />
                        </DirectEvents>
                    </ext:Button>
                    <ext:Button ID="btnNuevoProducto" runat="server" Text="Aceptar" Icon="Tick">
                        <DirectEvents>
                            <Click OnEvent="btnNuevoProducto_Click" Before="if (!#{NumAfiliacion}.isValid() && 
                                !#{NumTarjeta}.isValid()) { return false; }" />
                        </DirectEvents>
                    </ext:Button>                   
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:BorderLayout ID="MainBorderLayout" runat="server">
        <Center Split="true">
            <ext:GridPanel ID="GridComercioTarjeta" runat="server" Height="450" AutoDoLayout="true" Border="false" AutoExpandColumn="ClaveMA">
                <TopBar>
                    <ext:Toolbar runat="server">
                        <Items>
                            <ext:ComboBox ID="cBoxCliente" runat="server" EmptyText="Selecciona el Cliente" Width="150" 
                                AllowBlank="false" DisplayField="NombreORazonSocial" ValueField="ID_Colectiva"
                                StoreID="StoreSubEmisores" ListWidth="200">
                            </ext:ComboBox>
                            <ext:TextField ID="txtNumeroAfiliciacion" runat="server" EmptyText="Número de Afiliación" 
                                MaxLength="30" Width="150" MaskRe="[0-9]" />
                            <ext:TextField ID="txtNumeroTarjeta" runat="server" EmptyText="Número de Tarjeta" 
                                MaxLength="16" MaskRe="[0-9]" MinLength="16" Width="150" />
                            <ext:Button ID="btnConsultar" runat="server" Text="Buscar" Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnConsultar_Click" Before="if (!#{cBoxCliente}.isValid() ||
                                        !#{txtNumeroTarjeta}.isValid()) { return false; }">
                                        <EventMask ShowMask="true" Msg="Realizando Consulta..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarSeparator runat="server" />
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarFill runat="server" />
                            <ext:Button ID="btnAlta" runat="server" Text="Nuevo" Icon="Add">
                                <Listeners>
                                    <Click Handler="#{WdwNuevoComercioTarjeta}.show();" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnExcel" runat="server" Text="Exportar a Excel" Disabled="true" Icon="PageExcel"
                                ToolTip="Obtener Datos en un Archivo Excel">
                                <DirectEvents>
                                    <Click OnEvent="DownloadComercioTarjeta" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                            e.stopEvent(); 
                                            AdminComTarj.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                           <%-- <ext:Button ID="btnEliminar" runat="server" Text="Eliminar" Disabled="true">
                                <DirectEvents>
                                    <Click OnEvent="btnEliminar_Click">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>--%>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <Store>
                    <ext:Store ID="StoreComercio" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="IdTarjeta">
                                <Fields>
                                    <ext:RecordField Name="Cliente" />
                                    <ext:RecordField Name="Afiliacion" />
                                    <ext:RecordField Name="ClaveMA" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <ColumnModel ID="ColumnModel2" runat="server">
                    <Columns>
                        <ext:Column DataIndex="Cliente" Header="Cliente" Width="160" />
                        <ext:Column DataIndex="Afiliacion" Header="Número de Afiliación" Width="160" />
                        <ext:Column DataIndex="ClaveMA" Header="Número de Tarjeta" Width="160" />
                         <ext:CommandColumn Width="60">
                            <Commands>
                                <ext:GridCommand Icon="Cross" CommandName="Eliminar">
                                    <ToolTip Text="Eliminar Registro" />
                                </ext:GridCommand>
                            </Commands>
                        </ext:CommandColumn>
                    </Columns>
                </ColumnModel>
                <SelectionModel>
                    <ext:RowSelectionModel SingleSelect="true" />
                </SelectionModel>
                <%--<DirectEvents>
                    <RowClick OnEvent="selectRowResultados_Event">
                        <ExtraParams>
                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridResultados}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                        </ExtraParams>
                    </RowClick>
                </DirectEvents>--%>
                <DirectEvents>
                    <Command OnEvent="EjecutarComando">
                        <Confirmation ConfirmRequest="true"
                             Title="Confirmación" Message="¿Estás seguro de eliminar el registro?" />
                        <ExtraParams>
                            <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                            <ext:Parameter Name="Afiliacion" Value="Ext.encode(record.data.Afiliacion)" Mode="Raw" />
                        </ExtraParams>
                    </Command>
                </DirectEvents>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="StoreComercio" DisplayInfo="true" HideRefresh="true"
                        DisplayMsg="Mostrando Comercios - Tarjetas {0} - {1} de {2}" />
                </BottomBar>
            </ext:GridPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>

