<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="AltaPromocionesEcommerce.aspx.cs" Inherits="Lealtad.AltaPromocionesEcommerce" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .wrap-area{
            white-space:pre;
        }
    </style>

    <script type="text/javascript">
        var prepareToolbar = function (grid, toolbar, rowIndex, record) {
            if (record.get("EnAutorizador") == 1) { //SI
                toolbar.items.get(0).hide();
            } else {
                toolbar.items.get(1).hide();
            }
        }

        var confirmaPromo = function (grid, chkSams, enSams, chkEdenred, enEdenred) {
            var title = 'Módulo de Promociones';
            var paraSams = (chkSams && enSams) ? 0 : chkSams ? 1 : 0;
            var paraEdenred = (chkEdenred && enEdenred) ? 0 : chkEdenred ? 1 : 0
            var programas = ((chkSams && !enSams) && (chkEdenred && !enEdenred)) ? 'Sams Club Benefits y Edenred' :
                (chkSams && !enSams) ? 'Sams Club Benefits' : (chkEdenred && !enEdenred) ? 'Edenred' : '';
            var msg = programas == '' ? '¿Confirmas los cambios a la promoción?' :
                '¿Confirmas los cambios a la promoción para ' + programas + '?';

            if (grid.getSelectionModel().selections.length == 0) {
                Ext.Msg.alert(title, 'Por favor, selecciona al menos una sucursal.');
                return false;
            }

            if (!chkSams && !chkEdenred) {
                Ext.Msg.alert(title, 'Por favor, elige al menos un programa.');
                return false;
            }
            
            Ext.Msg.confirm(title, msg, function (btn) {
                if (btn == 'yes') {
                    Ext.net.Mask.show({ msg: 'Procesando...' });
                    AltaPromo.ModificaPromocion(paraSams, paraEdenred);
                    return true;
                } else {
                    return false;
                }
            });
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">    
    <ext:Window ID="WdwModuloPromos" runat="server" Width="650" Height="400" Resizable="False" Title="Módulo de Promociones"
        Hidden="true" Closable="true" Modal="true" Layout="FitLayout" Draggable="true" Padding="5" Icon="TagGreen">
        <Items>
            <ext:FormPanel runat="server" Layout="FitLayout">
                <TopBar>
                    <ext:Toolbar ID="Toolbar5" runat="server" Height="70">
                        <Items>
                            <ext:Hidden ID="hdnIdPromo" runat="server" />
                            <ext:TextArea ID="txaPromocion" runat="server" Width="350" Height="65" AutoEl="={{tag:'textarea', autocomplete: 'off', wrap: 'on'}}"
                                ReadOnly="true" Disabled="true"/>
                            <ext:ToolbarFill ID="ToolbarFill2" runat="server" />
                            <ext:CheckboxGroup ID="chkBoxPrograma" runat="server" Width="250" NoteAlign="Top" Note="Programas">
                                <Items>
                                    <ext:Checkbox ID="chkBoxSams" runat="server" BoxLabel="Sam's Club Benefits"  />
                                    <ext:Checkbox ID="chkBoxEdenred" runat="server" BoxLabel="Edenred" />
                                </Items>
                            </ext:CheckboxGroup>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <Items>
                    <ext:GridPanel ID="GridSucursales" runat="server" Header="true" Title="Sucursales">
                        <Store>
                            <ext:Store ID="StoreSucursalesPromo" runat="server">
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_Sucursal">
                                        <Fields>
                                            <ext:RecordField Name="ID_Sucursal" />
                                            <ext:RecordField Name="ClaveSucursal" />
                                            <ext:RecordField Name="NombreSucursal" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <ColumnModel ID="ColumnModel3" runat="server">
                            <Columns>
                                <ext:Column DataIndex="ID_Sucursal" Hidden="true" />
                                <ext:Column DataIndex="NombreSucursal" Header="Nombre de la Sucursal" Width="400" />
                                <ext:Column DataIndex="ClaveSucursal" Header="Clave" Width="180" />
                            </Columns>
                        </ColumnModel>
                        <SelectionModel>
                            <ext:CheckboxSelectionModel ID="CheckboxSelectionModel1" runat="server" />
                        </SelectionModel>
                        <BottomBar>
                            <ext:PagingToolbar ID="PagingToolBar3" runat="server" StoreID="StoreSucursalesPromo" DisplayInfo="true"
                                DisplayMsg="Sucursales {0} - {1} de {2}" />
                        </BottomBar>
                    </ext:GridPanel>
                </Items>
                <BottomBar>
                    <ext:Toolbar runat="server" Flat="false">
                        <Items>
                            <ext:ToolbarFill ID="ToolbarFillDummy" runat="server" />
                            <ext:ToolbarSeparator />
                            <ext:Button ID="btnModificarPromo" runat="server" Text="Aplicar Cambios" Icon="Tick">
                                <Listeners>
                                    <Click Handler="return confirmaPromo(#{GridSucursales}, #{chkBoxSams}.checked, #{chkBoxSams}.disabled,
                                        #{chkBoxEdenred}.checked, #{chkBoxEdenred}.disabled);"/>
                                </Listeners>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </BottomBar>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:Window ID="WdwAltaUsuarios" runat="server" Width="650" Height="400" Resizable="False" Title="Alta de Usuarios"
        Hidden="true" Closable="true" Modal="true" Layout="FitLayout" Draggable="true" Padding="5" LabelWidth="50"
        Icon="GroupAdd">
        <Items>
            <ext:GridPanel ID="GridAltaUsuarios" runat="server" StripeRows="true" Layout="FitLayout">
                <Store>
                    <ext:Store ID="StoreAltaUsuarios" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID">
                                <Fields>
                                    <ext:RecordField Name="ClaveCadena" />
                                    <ext:RecordField Name="ClaveSucursal" />
                                    <ext:RecordField Name="Email" />
                                    <ext:RecordField Name="EsGerente" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <TopBar>
                    <ext:Toolbar ID="Toolbar4" runat="server" Layout="HBoxLayout" BodyPadding="5"
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
                            <ext:Button ID="btnCargarArchivo" runat="server" Text="Cargar Archivo" Icon="PageWhitePut" Flex="1">
                                <DirectEvents>
                                    <Click OnEvent="btnCargarArchivo_Click" IsUpload="true" Before="#{GridAltaUsuarios}.getStore().removeAll();"
                                        After="#{FileUploadField1}.reset();">
                                        <EventMask ShowMask="true" Msg="Cargando Archivo..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Hidden ID="Hidden1" runat="server" Flex="1" />
                            <ext:Button ID="btnAltaUsuario" runat="server" Text="Sólo un Usuario" Icon="UserAdd" Flex="1">
                                <Listeners>
                                    <Click Handler="#{cBoxSucursal}.clearValue(); #{txtCorreo}.clear();
                                        #{chkGerente}.setValue(false); #{WdwAltaUsuario}.show();" />
                                </Listeners>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <ColumnModel ID="ColumnModel12" runat="server">
                    <Columns>
                        <ext:Column ColumnID="ClaveCadena" Header="Clave Cadena" Sortable="true" DataIndex="ClaveCadena"
                            Width="120" />
                        <ext:Column ColumnID="ClaveSucursal" Header="Clave Sucursal" Sortable="true" DataIndex="ClaveSucursal"
                            Width="100" />
                        <ext:Column ColumnID="Email" Header="Correo Electrónico" Sortable="true" DataIndex="Email"
                            Width="280" />
                        <ext:Column ColumnID="EsGerente" Header="Es Gerente" Sortable="true" DataIndex="EsGerente"
                            Width="100" />
                    </Columns>
                </ColumnModel>
                <BottomBar>
                    <ext:Toolbar ID="Toolbar3" runat="server">
                        <Items>
                            <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                            <ext:Button ID="btnAltaUsuarios" runat="server" Text="Dar de Alta Usuarios" Icon="Add">
                                <DirectEvents>
                                    <Click OnEvent="btnAltaUsuarios_Click">
                                        <EventMask ShowMask="true" Msg="Creando Usuarios..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </BottomBar>
            </ext:GridPanel>
        </Items>
    </ext:Window>
    <ext:Window ID="WdwAltaUsuario" runat="server" Title="Alta de Usuario" Width="350" Height="170" Hidden="true"
        Modal="true" Resizable="false" Closable="true" Icon="UserAdd">
        <Items>
            <ext:FormPanel ID="FormPanelAltaUsuario" runat="server" Padding="10" MonitorValid="true" LabelAlign="Left" LabelWidth="120">
                <Items>
                    <ext:ComboBox ID="cBoxSucursal" runat="server" FieldLabel="Sucursal" Width="180" AllowBlank="false"
                        ValueField="Clave" DisplayField="Nombre">
                        <Store>
                            <ext:Store ID="StoreSucursales" runat="server">
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_Sucursal">
                                        <Fields>
                                            <ext:RecordField Name="ID_Sucursal" />
                                            <ext:RecordField Name="Clave" />
                                            <ext:RecordField Name="Nombre" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        </ext:ComboBox>
                    <ext:TextField ID="txtCorreo" runat="server" FieldLabel="Correo Electrónico" Width="180"
                        AllowBlank="false" />
                    <ext:Checkbox ID="chkGerente" runat="server" FieldLabel="Gerente" />
                </Items>
                <Buttons>
                    <ext:Button ID="btnCrearUsuario" runat="server" Text="Crear Usuario" Icon="Add">
                        <DirectEvents>
                            <Click OnEvent="btnCrearUsuario_Click" 
                                Before="var valid= #{FormPanelAltaUsuario}.getForm().isValid(); if (!valid) {} return valid;"/>
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:Window ID="WdwEditaUsuario" runat="server" Title="Editar Usuario" Width="350" Height="140" Hidden="true"
        Modal="true" Resizable="false" Closable="true" Icon="UserEdit">
        <Items>
            <ext:FormPanel ID="FormPanel2" runat="server" Padding="10" MonitorValid="true" LabelAlign="Left" LabelWidth="120">
                <Items>
                    <ext:TextField ID="txtEmail" runat="server" FieldLabel="Correo Electrónico" Width="180" Enabled="false"
                        ReadOnly="true" />
                    <ext:RadioGroup ID="RadioGroupRol" runat="server" GroupName="RadioGroupRol" FieldLabel="Tipo de Acceso"
                        Cls="x-check-group-alt">
                        <Items>
                            <ext:Radio ID="rdGerente" runat="server" BoxLabel="Gerente" />
                            <ext:Radio ID="rdOperador" runat="server" BoxLabel="Operador" />
                        </Items>
                    </ext:RadioGroup>
                </Items>
                <Buttons>
                    <ext:Button ID="btnCancelar" runat="server" Text="Cancelar" Icon="Cross">
                        <Listeners>
                            <Click Handler="#{WdwEditaUsuario}.hide();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button ID="btnAplicarCambio" runat="server" Text="Aplicar Cambio">
                        <DirectEvents>
                            <Click OnEvent="btnAplicarCambio_Click" />
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>

    <%--<ext:Viewport ID="ViewPort1" runat="server">
        <Items>--%>
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center Split="true">
            <ext:GridPanel ID="GridCadenas" runat="server" Layout="FitLayout" StripeRows="true"
                Header="false" Border="false">
                <Store>
                    <ext:Store ID="StoreCadenas" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Cadena">
                                <Fields>
                                    <ext:RecordField Name="ID_Cadena" />
                                    <ext:RecordField Name="ClaveCadena" />
                                    <ext:RecordField Name="NombreComercial" />
                                    <ext:RecordField Name="Giro" />
                                    <ext:RecordField Name="FechaInsert" />
                                    <ext:RecordField Name="EnAutorizador" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <TopBar>
                    <ext:Toolbar ID="ToolbarConsulta" runat="server">
                        <Items>
                            <ext:Hidden ID="hdnIdCadena" runat="server" />
                            <ext:Hidden ID="hdnClaveCadena" runat="server" />
                            <ext:TextField ID="txtClaveCadena" EmptyText="Clave Cadena" Width="150" runat="server"
                                MaxLengthText="100" />
                            <ext:TextField ID="txtNombreCadena" EmptyText="Nombre Comercial" Width="250" runat="server"
                                 MaxLengthText="200" />
                            <ext:Button ID="btnBuscarCadenas" runat="server" Text="Buscar" Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscarCadenas_Click" Before="if ((#{txtClaveCadena}.getValue() == '')
                                        && (#{txtNombreCadena}.getValue() == '')) {return false};">
                                        <EventMask ShowMask="true" Msg="Buscando Cadenas..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Toolbar runat="server" Flat="true" Width="20" />
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <ColumnModel ID="ColumnModel8" runat="server">
                    <Columns>
                        <ext:Column DataIndex="ID_Cadena" Hidden="true" />
                        <ext:Column DataIndex="ClaveCadena" Header="Clave Cadena" Width="300" />
                        <ext:Column DataIndex="NombreComercial" Header="Nombre Comercial" Width="320" />
                        <ext:Column DataIndex="Giro" Header="Giro" Width="150" />
                        <ext:DateColumn DataIndex="FechaInsert" Header="Fecha de Creación" Width="120"
                            Format="dd/MM/yyyy" />
                    </Columns>
                </ColumnModel>
                <SelectionModel>
                    <ext:RowSelectionModel SingleSelect="true" />
                </SelectionModel>
                <DirectEvents>
                    <RowClick OnEvent="selectCadena_Event">
                        <EventMask ShowMask="true" Msg="Cargando Promociones y Usuarios..." MinDelay="500" />
                        <ExtraParams>
                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridCadenas}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                        </ExtraParams>
                    </RowClick>
                </DirectEvents>
            </ext:GridPanel>
        </Center>
        <South Split="true">
            <ext:Panel ID="PanelSur" runat="server" Height="350" Collapsible="true" Collapsed="true">
                <Items>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridPromociones" runat="server" Layout="FitLayout"
                                Width="500" Title="Promociones" AutoScroll="true">
                                <Store>
                                    <ext:Store ID="StorePromociones" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Promocion">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Promocion" />
                                                    <ext:RecordField Name="ClavePromocion" />
                                                    <ext:RecordField Name="Promocion" />
                                                    <ext:RecordField Name="Vigencia" />
                                                    <ext:RecordField Name="EnAutorizador" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <SelectionModel>
                                    <ext:RowSelectionModel SingleSelect="true" />
                                </SelectionModel>
                                <ColumnModel ID="ColumnModel6" runat="server">
                                    <Columns>
                                        <ext:CommandColumn Width="30">
                                            <PrepareToolbar Fn="prepareToolbar" />
                                            <Commands>
                                                <ext:GridCommand Icon="Cog" CommandName="Configurar">
                                                    <ToolTip Text="Configurar en Módulo de Promociones" />
                                                </ext:GridCommand>
                                            </Commands>
                                            <Commands>
                                                <ext:GridCommand Icon="Pencil" CommandName="Editar">
                                                    <ToolTip Text="Editar en Módulo de Promociones" />
                                                </ext:GridCommand>
                                            </Commands>
                                        </ext:CommandColumn>
                                        <ext:Column DataIndex="ID_Promocion" Hidden="true" />
                                        <ext:Column DataIndex="ClavePromocion" Header="Clave Promoción" />
                                        <ext:Column DataIndex="Promocion" Header="Descripción" Width="200" />
                                        <ext:Column DataIndex="Vigencia" Header="Vigencia" Width="180" />
                                        <ext:Column DataIndex="EnAutorizador" Hidden="true" />
                                    </Columns>
                                </ColumnModel>
                                <DirectEvents>
                                    <Command OnEvent="ModuloPromociones">
                                        <ExtraParams>
                                            <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                            <ext:Parameter Name="ID_Promocion" Value="Ext.encode(record.data.ID_Promocion)" Mode="Raw" />
                                            <ext:Parameter Name="Promocion" Value="Ext.encode(record.data.Promocion)" Mode="Raw" />
                                        </ExtraParams>
                                    </Command>
                                </DirectEvents>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="StorePromociones" DisplayInfo="true"
                                        DisplayMsg="Promociones {0} - {1} de {2}" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                        <East Split="true">
                            <ext:GridPanel ID="GridUsuarios" runat="server" Layout="FitLayout" StripeRows="true"
                                Title="Usuarios" Width="500" AutoScroll="true">
                                <Store>
                                    <ext:Store ID="StoreUsuarios" runat="server">
                                        <DirectEventConfig IsUpload="true"/>
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Colectiva">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Colectiva" />
                                                    <ext:RecordField Name="Sucursal" />
                                                    <ext:RecordField Name="Usuario" />
                                                    <ext:RecordField Name="Password" />
                                                    <ext:RecordField Name="TipoAcceso" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar2" runat="server">
                                        <Items>
                                            <ext:ToolbarFill runat="server" />
                                            <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="true">
                                                <DirectEvents>
                                                    <Click OnEvent="ExportGridToExcel" IsUpload="true" Success="Ext.Net.DirectMethods.Download({IsUpload:true})"
                                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                                e.stopEvent(); 
                                                                AltaPromo.StopMask();">
                                                        <ExtraParams>
                                                            <ext:Parameter Name="GridResult" Value="Ext.encode(#{GridUsuarios}.getRowsValues({selectedOnly : false}))" Mode="Raw" />
                                                        </ExtraParams>
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:ToolbarSeparator runat="server" />
                                            <ext:Button ID="btnAddUsers" runat="server" Text="Añadir Usuarios" Icon="GroupAdd">
                                                <Listeners>
                                                    <Click Handler="#{GridAltaUsuarios}.getStore().removeAll(); #{WdwAltaUsuarios}.show();" />
                                                </Listeners>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                        <ext:CommandColumn Width="60">
                                            <Commands>
                                                <ext:GridCommand Icon="UserEdit" CommandName="Edita">
                                                    <ToolTip Text="Editar Usuario" />
                                                </ext:GridCommand>
                                            </Commands>
                                            <Commands>
                                                <ext:GridCommand Icon="UserCross" CommandName="Elimina">
                                                    <ToolTip Text="Eliminar Usuario" />
                                                </ext:GridCommand>
                                            </Commands>
                                        </ext:CommandColumn>
                                        <ext:Column DataIndex="ID_Colectiva" Hidden="true" />
                                        <ext:Column DataIndex="Sucursal" Header="Sucursal" />
                                        <ext:Column DataIndex="Usuario" Header="Usuario" Width="150" />
                                        <ext:Column DataIndex="Password" Header="Contraseña" />
                                        <ext:Column DataIndex="TipoAcceso" Header="TipoAcceso" />
                                    </Columns>
                                </ColumnModel>
                                <DirectEvents>
                                    <Command OnEvent="EjecutarComando">
                                        <Confirmation BeforeConfirm="if (command == 'Edita') return false;"
                                            ConfirmRequest="true" Title="Confirmación" Message="¿Estás seguro eliminar al usuario?" />
                                        <ExtraParams>
                                            <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                            <ext:Parameter Name="ID_Colectiva" Value="Ext.encode(record.data.ID_Colectiva)" Mode="Raw" />
                                            <ext:Parameter Name="Usuario" Value="Ext.encode(record.data.Usuario)" Mode="Raw" />
                                            <ext:Parameter Name="TipoAcceso" Value="Ext.encode(record.data.TipoAcceso)" Mode="Raw" />
                                        </ExtraParams>
                                    </Command>
                                </DirectEvents>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreUsuarios" DisplayInfo="true"
                                        DisplayMsg="Usuarios {0} - {1} de {2}" />
                                </BottomBar>
                            </ext:GridPanel>
                        </East>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
        </South>
    </ext:BorderLayout>
    <%-- </Items>
    </ext:Viewport>--%>
</asp:Content>
