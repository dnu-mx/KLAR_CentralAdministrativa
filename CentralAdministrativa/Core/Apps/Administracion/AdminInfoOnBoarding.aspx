<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="AdminInfoOnBoarding.aspx.cs" Inherits="Administracion.AdminInfoOnBoarding" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var fullName = function (value, metadata, record, rowIndex, colIndex, store) {
            return "<b>" + record.data.Nombre + "</b>";
        };

        var commandHandlerEliminarNodo = function () {
            Ext.Msg.confirm('Eliminar Nodo',
                '¿Esta seguro que desea eliminar el Nodo?',
                function (btn) {
                    if (btn == 'yes') {
                        Ext.net.Mask.show({ msg: 'Eliminando Nodo...' });
                        AdminObjeto.EliminarNodo();
                    }
                });
        }
    </script>
    <style type="text/css">
        .x-grid3-row-body p {
            margin: 3px 3px 7px 3px !important;
            width: 99%;
            color: black;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Window ID="WdwNuevoNodo" runat="server" Title="Nuevo Nodo" Width="400" Height="180" Hidden="true"
        Modal="true" Resizable="false" Icon="Add">
        <Items>
            <ext:FormPanel ID="FormPanelNuevoNodo" runat="server" Height="150" Padding="3" LabelAlign="Right" LabelWidth="120" Border="false">
                <Items>
                    <ext:FieldSet runat="server" Title="Nodo">
                        <Items>
                            <ext:TextField ID="txtNombreKeyNuevo" runat="server" FieldLabel="<span style='color:red;'>*</span> Key"
                                MaxLength="100" Width="355" AllowBlank="false" />
                            <ext:TextField ID="txtNombreNodoNuevo" runat="server" FieldLabel="<span style='color:red;'>*</span> Nombre"
                                MaxLength="100" Width="355" AllowBlank="false" />
                        </Items>
                    </ext:FieldSet>

                    <ext:FieldSet runat="server" Title="Parámetro Inicial">
                        <Items>
                            <ext:TextField ID="txtKeyNuevoParametroDefault" runat="server" FieldLabel="Valor Key <span style='color:red;'>*</span>" AllowBlank="false" Width="355" MaxLength="100" />
                            <ext:TextArea ID="txtDescNuevoParametroDefault" runat="server" FieldLabel="Descripción" MaxLength="200" Width="355" Height="50" />
                        </Items>
                    </ext:FieldSet>
                </Items>
                <Buttons>
                    <ext:Button runat="server" Text="Cancelar" Icon="Cancel">
                        <Listeners>
                            <Click Handler="#{WdwNuevoNodo}.hide();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button ID="btnNuevoNodo" runat="server" Text="Aceptar" Icon="Tick">
                        <DirectEvents>
                            <Click OnEvent="btnNuevoNodo_Click" Before="
                                var valid= #{FormPanelNuevoNodo}.getForm().isValid();
                                if (!valid) { return false; }" />
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>

    <ext:Window ID="WdwEditarParametro" runat="server" Title="Editar Parámetro" Icon="Pencil" Width="420" Height="270"
        Hidden="true" Modal="true" Resizable="false">
        <Items>
            <ext:FormPanel ID="FormPanelEditarParametro" runat="server" Padding="10" LabelWidth="80" Border="false"
                Layout="FormLayout" LabelAlign="Right">
                <Items>
                    <ext:Hidden ID="hdnIdParametro" runat="server" />
                    <ext:TextField ID="txtKeyParametro" runat="server" FieldLabel="Valor Key <span style='color:red;'>*</span>" AllowBlank="false" Width="300" MaxLength="100" />
                    <ext:TextArea ID="txtDescParametro" runat="server" FieldLabel="Descripción" MaxLength="200" Width="300" Height="150" />
                </Items>
                <Buttons>
                    <ext:Button runat="server" Text="Cancelar" Icon="Cancel">
                        <Listeners>
                            <Click Handler="#{WdwEditarParametro}.hide();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button ID="btnGuardarEdicionParametro" runat="server" Text="Guardar Cambios" Icon="Disk">
                        <DirectEvents>
                            <Click OnEvent="btnGuardarEdicionParametro_Click" Before="var valid= #{FormPanelEditarParametro}.getForm().isValid(); if (!valid) {} return valid;">
                                <EventMask ShowMask="true" Msg="Guardando Cambios..." MinDelay="500" />
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>

    <ext:BorderLayout runat="server">
        <West Split="true">
            <ext:Panel runat="server" Width="335" Border="false" Layout="FitLayout" Title="Nodos">
                <Content>
                    <ext:BorderLayout runat="server">
                        <South Split="true">
                            <ext:FormPanel runat="server" Height="25" Border="false">
                                <Items>
                                    <ext:Toolbar runat="server">
                                        <Items>
                                            <ext:Button ID="btnLimpiarIzq" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                                <DirectEvents>
                                                    <Click OnEvent="btnLimpiarIzq_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:ToolbarFill runat="server" />
                                            <ext:Button runat="server" Icon="Add" ToolTip="Crear Nuevo Nodo" Text="Nuevo Nodo">
                                                <Listeners>
                                                    <Click Handler="#{FormPanelNuevoNodo}.reset(); 
                                                                    #{WdwNuevoNodo}.show();" />
                                                </Listeners>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </Items>
                            </ext:FormPanel>
                        </South>
                        <Center Split="true">
                            <ext:GridPanel ID="GridResultadosNodos" runat="server" Border="false" Header="false">
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar2" runat="server">
                                        <Items>
                                            <ext:TextField ID="txtBusquedaNodo" runat="server" EmptyText="Key/Nombre Nodo" />
                                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnBuscar_Click" Before="#{GridResultadosNodos}.getStore().removeAll(); 
                                                        #{PanelCentralNodos}.setTitle('_'); #{PanelCentralNodos}.setDisabled(true);">
                                                        <EventMask ShowMask="true" Msg="Buscando Nodos..." MinDelay="500" />
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <Store>
                                    <ext:Store ID="StoreNodos" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_ScoreNodo">
                                                <Fields>
                                                    <ext:RecordField Name="ID_ScoreNodo" />
                                                    <ext:RecordField Name="NombreKey" />
                                                    <ext:RecordField Name="NombreNodoPadre" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <ColumnModel runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_ScoreNodo" Hidden="true" />
                                        <ext:Column DataIndex="NombreKey" Header="Key" Width="180" />
                                        <ext:Column DataIndex="NombreNodoPadre" Header="Nombre" Width="150" />
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel SingleSelect="true" />
                                </SelectionModel>
                                <DirectEvents>
                                    <RowClick OnEvent="selectRowResultadosPP_Event">
                                        <EventMask ShowMask="true" Msg="Obteniendo Información del Nodo..." MinDelay="500" />
                                        <ExtraParams>
                                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridResultadosNodos}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                        </ExtraParams>
                                    </RowClick>
                                </DirectEvents>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreNodos" DisplayInfo="true"
                                        DisplayMsg="{0} - {1} de {2}" HideRefresh="true" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </West>
        <Center Split="true">
            <ext:Panel ID="PanelCentralNodos" runat="server" Height="250" Border="false" Title="_" Disabled="true">
                <Items>
                    <ext:BorderLayout runat="server">
                        <Center>
                            <ext:TabPanel runat="server">
                                <Items>
                                    <ext:FormPanel ID="FormPanelInfoAd" runat="server" Title="Información General" AutoScroll="true" Border="false">
                                        <Content>
                                            <ext:BorderLayout ID="BorderLayout2" runat="server">
                                                <Center Split="true">
                                                    <ext:FormPanel ID="FormPanelDataInfoAd" runat="server" LabelAlign="Left" LabelWidth="150">
                                                        <Items>
                                                            <ext:FieldSet runat="server" Title="Datos Generales del Nodo" Layout="FormLayout">
                                                                <Items>
                                                                    <ext:Hidden ID="hdnIdNodo" runat="server" />
                                                                    <ext:TextField ID="txtNombreKey" runat="server" FieldLabel="<span style='color:red;'>*</span> Key"
                                                                        MaxLength="10" Width="540" AllowBlank="false" Disabled="true" />
                                                                    <ext:TextField ID="txtNombreNodo" runat="server" FieldLabel="<span style='color:red;'>*</span> Nombre"
                                                                        MaxLength="50" Width="540" AllowBlank="false" />
                                                                </Items>
                                                                <Buttons>
                                                                    <ext:Button ID="btnGuardarInfoAd" runat="server" Text="Guardar" Icon="Disk">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="btnGuardarInfoAd_Click"
                                                                                Before="var valid= #{FormPanelDataInfoAd}.getForm().isValid(); 
                                                                                        if (!valid) { return false; } ">
                                                                                <EventMask ShowMask="true" Msg="Guardando Información..." MinDelay="500" />
                                                                            </Click>
                                                                        </DirectEvents>
                                                                    </ext:Button>
                                                                    <ext:Button ID="btnEliminarNodo" runat="server" Text="Eliminar Nodo" Icon="Cross">
                                                                        <Listeners>
                                                                            <Click Handler="commandHandlerEliminarNodo();" />
                                                                        </Listeners>
                                                                    </ext:Button>
                                                                </Buttons>
                                                            </ext:FieldSet>
                                                        </Items>
                                                    </ext:FormPanel>
                                                </Center>
                                            </ext:BorderLayout>
                                        </Content>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelParametros" runat="server" Title="Parámetros" Layout="FitLayout">
                                        <Content>
                                            <ext:BorderLayout runat="server">
                                                <North Split="true">
                                                    <ext:FormPanel ID="FormPanelNuevoParametro" runat="server" LabelWidth="150" Height="180">
                                                        <Items>
                                                            <ext:FieldSet runat="server" Title="Datos del Nuevo Parámetro" Layout="FormLayout">
                                                                <Items>
                                                                    <ext:TextField ID="txtKeyNuevoParametro" runat="server" FieldLabel="Valor Key <span style='color:red;'>*   </span>"
                                                                        AllowBlank="false" Width="550" MaxLength="100" />
                                                                    <ext:TextArea ID="txtDescNuevoParametro" runat="server" FieldLabel="Descripción" MaxLength="200" Width="550"
                                                                        Height="75"/>
                                                                </Items>
                                                                <Buttons>
                                                                    <ext:Button ID="btnAddParametro" runat="server" Text="Añadir Parámetro" Icon="Add">
                                                                        <DirectEvents>
                                                                            <Click OnEvent="btnAddParametro_Click" Before="var valid= #{FormPanelNuevoParametro}.getForm().isValid(); if (!valid) {} return valid;">
                                                                                <EventMask ShowMask="true" Msg="Añadiendo Parámetro al Nodo..." MinDelay="500" />
                                                                            </Click>
                                                                        </DirectEvents>
                                                                    </ext:Button>
                                                                </Buttons>
                                                            </ext:FieldSet>
                                                        </Items>
                                                    </ext:FormPanel>
                                                </North>
                                                <Center Split="true">
                                                    <ext:GridPanel ID="GridParametros" runat="server" Header="true" Height="200" AutoExpandColumn="DescripcionValor">
                                                        <Store>
                                                            <ext:Store ID="StoreParametros" runat="server">
                                                                <DirectEventConfig IsUpload="true" />
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="ID_ValorNodo">
                                                                        <Fields>
                                                                            <ext:RecordField Name="ID_ValorNodo" />
                                                                            <ext:RecordField Name="ID_ScoreNodo" />
                                                                            <ext:RecordField Name="ValorKey" />
                                                                            <ext:RecordField Name="DescripcionValor" />
                                                                        </Fields>
                                                                    </ext:JsonReader>
                                                                </Reader>
                                                                <DirectEventConfig IsUpload="true" />
                                                            </ext:Store>
                                                        </Store>
                                                        <ColumnModel runat="server">
                                                            <Columns>
                                                                <ext:Column DataIndex="ID_ValorNodo" Hidden="true" />
                                                                <ext:Column DataIndex="ID_ScoreNodo" Hidden="true" />
                                                                <ext:Column DataIndex="ValorKey" Header="Valor Key" Width="120" />
                                                                <ext:Column DataIndex="DescripcionValor" Header="Descripción" Width="400" />
                                                                <ext:CommandColumn Width="60">
                                                                    <Commands>
                                                                        <ext:GridCommand Icon="Pencil" CommandName="Edit">
                                                                            <ToolTip Text="Editar Parámetro" />
                                                                        </ext:GridCommand>
                                                                        <ext:GridCommand Icon="Delete" CommandName="Delete">
                                                                            <ToolTip Text="Eliminar Parámetro" />
                                                                        </ext:GridCommand>
                                                                    </Commands>
                                                                </ext:CommandColumn>
                                                            </Columns>
                                                        </ColumnModel>
                                                        <DirectEvents>
                                                            <Command OnEvent="EjecutarComandoParametros">
                                                                <Confirmation BeforeConfirm="if (command == 'Edit') return false;" 
                                                                    ConfirmRequest="true" Title="Confirmación" Message="¿Estás seguro de Eliminar el parámetro?" />
                                                                <ExtraParams>
                                                                    <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                                                    <ext:Parameter Name="Values" Value="Ext.encode(record.data)" Mode="Raw" />
                                                                </ExtraParams>
                                                            </Command>
                                                        </DirectEvents>
                                                        <SelectionModel>
                                                            <ext:RowSelectionModel SingleSelect="true" />
                                                        </SelectionModel>
                                                        <BottomBar>
                                                            <ext:PagingToolbar ID="PagingToolBar3" runat="server" StoreID="StoreParametros" DisplayInfo="true"
                                                                DisplayMsg="Bines {0} - {1} de {2}" HideRefresh="true" />
                                                        </BottomBar>
                                                    </ext:GridPanel>
                                                </Center>
                                            </ext:BorderLayout>
                                        </Content>
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
