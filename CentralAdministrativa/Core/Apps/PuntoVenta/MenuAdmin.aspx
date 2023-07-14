<%@ Page Title="" Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master"
    CodeBehind="MenuAdmin.aspx.cs" Inherits="TpvWeb.MenuAdmin" %>


<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <ext:Store ID="stMenus" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_Menu">
                <Fields>
                    <ext:RecordField Name="ID_Menu" />
                    <ext:RecordField Name="Clave" />
                    <ext:RecordField Name="Descripcion" />
                    <ext:RecordField Name="Version" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:Store ID="stPromociones" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_Promocion">
                <Fields>
                    <ext:RecordField Name="ID_Promocion" />
                    <ext:RecordField Name="Clave" />
                    <ext:RecordField Name="Descripcion" />
                    <ext:RecordField Name="Meses" />
                    <ext:RecordField Name="Etiqueta" />
                    <ext:RecordField Name="PrimerPago" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:Store ID="stPromocionMenu" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_Promocion">
                <Fields>
                    <ext:RecordField Name="ID_Promocion" />
                    <ext:RecordField Name="ID_Menu" />
                    <ext:RecordField Name="Clave" />
                    <ext:RecordField Name="Descripcion" />
                    <ext:RecordField Name="Meses" />
                    <ext:RecordField Name="Etiqueta" />
                    <ext:RecordField Name="PrimerPago" />
                    <ext:RecordField Name="Orden" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:Store runat="server" ID="stOrden">
        <Reader>
            <ext:JsonReader IDProperty="Value">
                <Fields>
                    <ext:RecordField Name="Text" />
                    <ext:RecordField Name="Value" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:Window runat="server" ID="dialog_Menu" Title="Nueva Menú" Modal="true"
        Width="500" Height="180" Closable="false" Hidden="true" Resizable="false">
        <Items>
            <ext:BorderLayout runat="server">
                <Center>

                    <ext:FormPanel ID="FormMenuPanel" runat="server" MonitorValid="true"
                        AutoHeight="true" Width="350" Padding="10">
                        <Items>
                            <ext:TextField ID="txtIDMenu" runat="server" Hidden="true" />
                            <ext:TextField ID="txtClave" runat="server" AllowBlank="false" FieldLabel="Clave" AnchorHorizontal="100%" MaxLength="5" />
                            <ext:TextField ID="txtDescripcion" runat="server" AllowBlank="false" FieldLabel="Descripción" AnchorHorizontal="100%" MaxLength="50" />
                            <ext:NumberField ID="txtVersion" runat="server" AllowBlank="false" FieldLabel="Versión" AnchorHorizontal="100%" MaxLength="4" />
                        </Items>
                        <Buttons>
                            <ext:Button ID="btnCancelar2" runat="server" Text="Cancelar" Icon="Cancel">
                                <DirectEvents>
                                    <Click OnEvent="Click_CancelarDialog" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnGuardar" runat="server" Text="Guardar" Icon="Disk">
                                <DirectEvents>
                                    <Click OnEvent="Click_GuardarNuevoMenu" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnActualizar" runat="server" Text="Actualizar" Icon="ArrowRefresh" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="Click_ActualizarMenu" />
                                </DirectEvents>
                            </ext:Button>
                        </Buttons>
                    </ext:FormPanel>

                </Center>
            </ext:BorderLayout>
        </Items>
    </ext:Window>

    <ext:Window runat="server" ID="dialog_promocion" Title="Nueva Opción de Menú" Modal="true"
        Width="500" Height="300" Closable="false" Hidden="true" Resizable="false">
        <Items>
            <ext:BorderLayout runat="server">
                <Center>

                    <ext:FormPanel ID="FormPromociones" runat="server" MonitorValid="true"
                        AutoHeight="true" Width="350" Padding="10">
                        <Items>
                            <ext:TextField ID="txtIDPromocion" runat="server" Hidden="true" />
                            <ext:TextField ID="txtClavePromocion" runat="server" AllowBlank="false" FieldLabel="Clave" AnchorHorizontal="100%" MaxLength="3" />
                            <ext:TextArea ID="txtDescripcionPromocion" runat="server" AllowBlank="false" FieldLabel="Descripción" AnchorHorizontal="100%" MaxLength="500" />
                            <ext:NumberField ID="txtMesesPromocion" runat="server" AllowBlank="false" FieldLabel="Meses" AnchorHorizontal="100%" MaxLength="4" />
                            <ext:TextField ID="txtEtiquePromocion" runat="server" AllowBlank="false" FieldLabel="Etiqueta" AnchorHorizontal="100%" MaxLength="19" />
                            <ext:SelectBox ID="txtPrimerPagoPromocion" runat="server" AllowBlank="false" FieldLabel="PrimerPago"
                                AnchorHorizontal="100%">
                                <Items>
                                    <ext:ListItem Text="SI" Value="1" />
                                    <ext:ListItem Text="NO" Value="0" />
                                </Items>
                            </ext:SelectBox>
                        </Items>
                        <Buttons>
                            <ext:Button ID="btnCancelar" runat="server" Text="Cancelar" Icon="Cancel">
                                <DirectEvents>
                                    <Click OnEvent="Click_CancelarDialog" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnGuardarPromocion" runat="server" Text="Guardar" Icon="Disk">
                                <DirectEvents>
                                    <Click OnEvent="Click_GuardarNuevaPromocion" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnActualizarPromocion" runat="server" Text="Actualizar" Icon="ArrowRefresh" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="Click_ActualizarPromocion" />
                                </DirectEvents>
                            </ext:Button>
                        </Buttons>
                    </ext:FormPanel>

                </Center>
            </ext:BorderLayout>
        </Items>
    </ext:Window>

    <ext:BorderLayout runat="server">
        <North>
            <ext:Panel ID="panel_menus_promociones" runat="server" Height="250">
                <Items>
                    <ext:ColumnLayout ID="ColumnLayout1" runat="server" FitHeight="true" Split="true">
                        <Columns>
                            <ext:LayoutColumn ColumnWidth="0.40">
                                <ext:Panel ID="Panel1" runat="server" Height="250" Title="Menús">
                                    <Items>
                                        <ext:BorderLayout ID="BorderLayout1" runat="server">
                                            <Center>
                                                <ext:GridPanel ID="panel_menu" runat="server" StoreID="stMenus" AutoExpandColumn="Descripcion">
                                                    <TopBar>
                                                        <ext:Toolbar ID="Toolbar1" runat="server">
                                                            <Items>
                                                                <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                                <ext:Button ID="btn_nuevo_menu" runat="server" Text="Nuevo Menú" Icon="Add">
                                                                    <DirectEvents>
                                                                        <Click OnEvent="Click_FormularioNuevoMenu" />
                                                                    </DirectEvents>
                                                                </ext:Button>
                                                            </Items>
                                                        </ext:Toolbar>
                                                    </TopBar>

                                                    <DirectEvents>
                                                        <RowClick OnEvent="Click_VerDatosMenu">
                                                            <ExtraParams>
                                                                <ext:Parameter Name="Values" Value="Ext.encode(#{panel_menu}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                                            </ExtraParams>
                                                        </RowClick>
                                                        <Command OnEvent="Click_ElimiarMenu">
                                                            <ExtraParams>
                                                                <ext:Parameter Name="ID_Menu" Value="record.data.ID_Menu" Mode="Raw" />
                                                                <ext:Parameter Name="CommandName" Value="command" Mode="Raw" />
                                                                <ext:Parameter Name="Values" Value="Ext.encode(record.data)" Mode="Raw" />
                                                            </ExtraParams>
                                                        </Command>
                                                    </DirectEvents>

                                                    <SelectionModel>
                                                        <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true" />
                                                    </SelectionModel>

                                                    <ColumnModel>
                                                        <Columns>
                                                            <ext:Column ColumnID="ID_Menu" DataIndex="ID_Menu" Header="ID Menú" Hidden="true" />
                                                            <ext:Column DataIndex="Clave" Header="Clave" Width="80" />
                                                            <ext:Column DataIndex="Descripcion" Header="Descripción" Width="80" />
                                                            <ext:Column DataIndex="Version" Header="Versión" Width="80" />
                                                            <ext:CommandColumn Width="60">
                                                                <Commands>
                                                                    <ext:GridCommand Icon="NoteEdit" CommandName="EditarMenu" />
                                                                    <ext:CommandSeparator />
                                                                    <ext:GridCommand Icon="Delete" CommandName="BorrarMenu" />
                                                                </Commands>
                                                            </ext:CommandColumn>
                                                        </Columns>
                                                    </ColumnModel>

                                                </ext:GridPanel>
                                            </Center>
                                        </ext:BorderLayout>
                                    </Items>
                                </ext:Panel>
                            </ext:LayoutColumn>
                            <ext:LayoutColumn ColumnWidth="0.60">
                                <ext:Panel ID="panel_nodos" runat="server" Title="Opciones">
                                    <Items>
                                        <ext:BorderLayout runat="server">
                                            <Center>
                                                <ext:GridPanel ID="GridPanelNodos" runat="server" StoreID="stPromociones" AutoExpandColumn="Descripcion">
                                                    <TopBar>
                                                        <ext:Toolbar ID="Toolbar2" runat="server">
                                                            <Items>
                                                                <ext:Button ID="Button1" runat="server" Text="Asignar a Menú" Icon="ArrowLeft">
                                                                    <DirectEvents>
                                                                        <Click OnEvent="Click_AsignarPromocionAMenu">
                                                                            <ExtraParams>
                                                                                <ext:Parameter Name="ValuesMenus" Value="Ext.encode(#{panel_menu}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                                                                <ext:Parameter Name="ValuesPromociones" Value="Ext.encode(#{GridPanelNodos}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                                                            </ExtraParams>
                                                                        </Click>
                                                                    </DirectEvents>
                                                                </ext:Button>
                                                                <ext:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                                <ext:Button ID="btnNuevaPromocion" runat="server" Text="Nueva Opción" Icon="Add">
                                                                    <DirectEvents>
                                                                        <Click OnEvent="Click_FormularioNuevaPromocion" />
                                                                    </DirectEvents>
                                                                </ext:Button>
                                                            </Items>
                                                        </ext:Toolbar>
                                                    </TopBar>
                                                    <DirectEvents>
                                                        <Command OnEvent="Click_ElimiarPromocion">
                                                            <ExtraParams>
                                                                <ext:Parameter Name="ID_Promocion" Value="record.data.ID_Promocion" Mode="Raw" />
                                                                <ext:Parameter Name="CommandName" Value="command" Mode="Raw" />
                                                                <ext:Parameter Name="Values" Value="Ext.encode(record.data)" Mode="Raw" />
                                                            </ExtraParams>
                                                        </Command>
                                                    </DirectEvents>

                                                    <SelectionModel>
                                                        <ext:CheckboxSelectionModel ID="CheckboxSelectionModel1" runat="server" RowSpan="2" />
                                                    </SelectionModel>

                                                    <ColumnModel>
                                                        <Columns>
                                                            <ext:Column ColumnID="ID_Promocion" DataIndex="ID_Promocion" Hidden="true" />
                                                            <ext:Column DataIndex="Clave" Header="Clave" Width="80" />
                                                            <ext:Column DataIndex="Descripcion" Header="Descripción" Width="80" />
                                                            <ext:Column DataIndex="Meses" Header="Meses" Width="80" />
                                                            <ext:Column DataIndex="Etiqueta" Header="Etiqueta" Width="80" />
                                                            <ext:Column DataIndex="PrimerPago" Header="Primer Pago" Width="80" />
                                                            <ext:CommandColumn Width="60">
                                                                <Commands>
                                                                    <ext:GridCommand Icon="NoteEdit" CommandName="EditarPromocion" />
                                                                    <ext:CommandSeparator />
                                                                    <ext:GridCommand Icon="Delete" CommandName="BorrarPromocion" />
                                                                </Commands>
                                                            </ext:CommandColumn>
                                                        </Columns>
                                                    </ColumnModel>

                                                </ext:GridPanel>
                                            </Center>
                                        </ext:BorderLayout>
                                    </Items>
                                </ext:Panel>
                            </ext:LayoutColumn>
                        </Columns>
                    </ext:ColumnLayout>
                </Items>
            </ext:Panel>
        </North>
        <Center>
            <ext:Panel ID="panel_nodos_menu" runat="server" Title="Opciones del Menú:">
                <Items>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center>
                            <ext:GridPanel ID="panel_promocion" runat="server" StoreID="stPromocionMenu" AutoExpandColumn="Descripcion">
                                <DirectEvents>
                                    <Command OnEvent="Click_EjecutarComando">
                                        <ExtraParams>
                                            <ext:Parameter Name="ID_Menu" Value="record.data.ID_Menu" Mode="Raw" />
                                            <ext:Parameter Name="ID_Promocion" Value="record.data.ID_Promocion" Mode="Raw" />
                                            <ext:Parameter Name="CommandName" Value="command" Mode="Raw" />
                                        </ExtraParams>
                                    </Command>
                                </DirectEvents>

                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel2" runat="server" SingleSelect="true" />
                                </SelectionModel>

                                <ColumnModel>
                                    <Columns>
                                        <ext:Column ColumnID="ID_Promocion" DataIndex="ID_Promocion" Hidden="true" />
                                        <ext:Column ColumnID="ID_Menu" DataIndex="ID_Menu" Hidden="true" />
                                        <ext:Column DataIndex="Clave" Header="Clave" Width="80" />
                                        <ext:Column DataIndex="Descripcion" Header="Descripción" Width="80" />
                                        <ext:Column DataIndex="Meses" Header="Meses" Width="80" />
                                        <ext:Column DataIndex="Etiqueta" Header="Etiqueta" Width="80" />
                                        <ext:Column DataIndex="PrimerPago" Header="Primer Pago" Width="80" />
                                        <ext:Column DataIndex="Orden" Header="Orden" Width="80" />
                                        <ext:CommandColumn Width="85">
                                            <Commands>
                                                <ext:GridCommand Icon="ArrowUp" CommandName="SubirPromocion" />
                                                <ext:CommandSeparator />
                                                <ext:GridCommand Icon="ArrowDown" CommandName="BajarPromocion" />
                                                <ext:CommandSeparator />
                                                <ext:GridCommand Icon="Delete" CommandName="DesasignarPromocion" />
                                            </Commands>
                                        </ext:CommandColumn>
                                    </Columns>
                                </ColumnModel>

                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>

</asp:Content>
