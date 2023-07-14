<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="ConfiguradorParametrosMA.aspx.cs" Inherits="Administracion.ConfiguradorParametrosMA" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .x-grid3-row-body p {
            margin: 5px 5px 10px 5px !important;
            width: 99%;
            color: black;
        }
    </style>
    <script type="text/javascript">
        var template = '<span style="color:black;font-size:11px;font-weight:bold;font-family:segoe ui;">{0}</span>';

        var fullName = function (value, metadata, record, rowIndex, colIndex, store) {
            return String.format(template, record.data.Clave);
        };

        var checkParams = function (grid) {
            if (grid.getSelectionModel().selections.length == 0) {
                Ext.Msg.alert(title, 'Debes seleccionar al menos un parámetro.');
                return false;
            }
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent"  runat="server">
    <ext:Window ID="WdwAddParam" runat="server" Title="Nuevo Parámetro" Width="410" Height="220" Hidden="true"
        Modal="true" Resizable="false" Closable="true" Icon="ControlAddBlue">
        <Items>
            <ext:FormPanel ID="FormPanel_WdwAddParam" runat="server" Padding="10" MonitorValid="true"
                LabelAlign="Left" LabelWidth="100">
                <Items>
                    <ext:TextField ID="txtClavePMA" runat="server" Width="270" FieldLabel="Clave" AllowBlank="false"
                        MaxLengthText="50" />
                    <ext:TextField ID="txtDescripcion" runat="server" Width="270" FieldLabel="Descripción"
                        AllowBlank="false" MaxLengthText="100" />
                    <ext:TextField ID="txtTipoDatoJava" runat="server" Width="270" FieldLabel="Tipo de Dato Java"
                        AllowBlank="false" MaxLengthText="50" />
                    <ext:TextField ID="txtTipoDatoSQL" runat="server" Width="270" FieldLabel="Tipo de Dato SQL"
                        AllowBlank="false" MaxLengthText="50" />
                    <ext:TextField ID="txtValorDefault" runat="server" Width="270" FieldLabel="Valor Default"
                        AllowBlank="false" MaxLengthText="50" />
                </Items>
                <Buttons>
                    <ext:Button ID="btnGuardarNuevoPMA" runat="server" Text="Guardar" Icon="Disk">
                        <DirectEvents>
                            <Click OnEvent="btnGuardarNuevoPMA_Click" Before="var valid= #{FormPanel_WdwAddParam}.getForm().isValid();
                                if (!valid) {} return valid;" />
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:Window ID="WdwEditPMA" runat="server" Title="Editar" Width="410" Height="170" Hidden="true"
        Modal="true" Resizable="false" Closable="true" Icon="Pencil">
        <Items>
            <ext:FormPanel ID="FormPanel_WdwEditPMA" runat="server" Padding="10" MonitorValid="true"
                LabelAlign="Left" LabelWidth="100">
                <Items>
                    <ext:ComboBox ID="cBoxEntidades" runat="server" FieldLabel="Entidad" Width="270"
                        DisplayField="Descripcion" ValueField="ID_Entidad" AllowBlank="false">
                        <Store>
                            <ext:Store ID="StoreEntidad" runat="server">
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_Entidad">
                                        <Fields>
                                            <ext:RecordField Name="ID_Entidad" />
                                            <ext:RecordField Name="ClaveEntidad" />
                                            <ext:RecordField Name="Descripcion" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <Triggers>
                            <ext:FieldTrigger Icon="Clear" Qtip="Borrar" />
                        </Triggers>
                        <Listeners>
                            <TriggerClick Handler="this.removeByValue(this.getValue());this.clearValue();" />
                        </Listeners>
                        <DirectEvents>
                            <Select OnEvent="selectEntidad" Before="#{cBoxRegistrosEntidad}.clear();">
                                <EventMask ShowMask="true" Msg="Estableciendo Registros Entidad..." MinDelay="200" />
                            </Select>
                        </DirectEvents>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cBoxRegistrosEntidad" runat="server" FieldLabel="Registro Entidad" Width="270"
                        DisplayField="Descripcion" ValueField="ID_RegistroEntidad" AllowBlank="false">
                        <Store>
                            <ext:Store ID="StoreRegEntidad" runat="server">
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_RegistroEntidad">
                                        <Fields>
                                            <ext:RecordField Name="ID_RegistroEntidad" />
                                            <ext:RecordField Name="Descripcion" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <Triggers>
                            <ext:FieldTrigger Icon="Clear" Qtip="Borrar" />
                        </Triggers>
                        <Listeners>
                            <TriggerClick Handler="this.removeByValue(this.getValue());this.clearValue();" />
                        </Listeners>
                    </ext:ComboBox>
                    <ext:TextField ID="txtValorPMA" runat="server" Width="270" FieldLabel="Valor" AllowBlank="false"
                        MaxLengthText="50" />
                </Items>
                <Buttons>
                    <ext:Button ID="btnGuardarValorPMA" runat="server" Text="Guardar Cambios" Icon="Disk">
                        <DirectEvents>
                            <Click OnEvent="btnGuardarValorPMA_Click" Before="var valid= #{FormPanel_WdwEditPMA}.getForm().isValid();
                                if (!valid) {} return valid;" />
                        </DirectEvents>
                    </ext:Button>
                    <ext:Button ID="btnCancelar" runat="server" Text="Cancelar" Icon="Cancel">
                        <Listeners>
                            <Click Handler="#{WdwEditPMA}.hide();" />
                        </Listeners>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:BorderLayout ID="MainBorderLayout" runat="server">
        <West Split="true">
            <ext:GridPanel ID="GridParametrosMA" runat="server" Layout="FitLayout" Border="false"
                Header="false" AutoScroll="true" Width="300">
                <Store>
                    <ext:Store ID="StoreParametrosMA" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_ParametroMA">
                                <Fields>
                                    <ext:RecordField Name="ID_ParametroMA" />
                                    <ext:RecordField Name="Clave" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <TopBar>
                    <ext:Toolbar runat="server">
                        <Items>
                            <ext:Button ID="btnNuevoParametroMA" runat="server" Text="Nuevo Parámetro" Icon="ControlAddBlue">
                                <Listeners>
                                    <Click Handler="#{FormPanel_WdwAddParam}.reset(); #{WdwAddParam}.show();" />
                                </Listeners>
                            </ext:Button>
                            <ext:ToolbarSeparator runat="server" />
                            <ext:ToolbarFill runat="server" />
                            <ext:ToolbarSeparator runat="server" />
                            <ext:Button ID="btnAddParams" runat="server" Icon="ForwardBlue" ToolTip="Añadir Parámetro(s) Seleccionado(s) a la Cadena"
                                Disabled="true" Text="Añadir">
                                <DirectEvents>
                                    <Click OnEvent="btnAddParams_Click" Before="return checkParams(#{GridParametrosMA});">
                                        <EventMask ShowMask="true" Msg="Añadiendo Parámetros..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <ColumnModel ID="ColumnModel6" runat="server">
                    <Columns>
                        <ext:Column DataIndex="ID_ParametroMA" Hidden="true" />
                        <ext:Column ColumnID="Clave" Header="Parámetro" Width="300" DataIndex="Clave">
                            <Renderer Fn="fullName" />
                        </ext:Column>
                    </Columns>
                </ColumnModel>
                <View>
                    <ext:GridView runat="server" EnableRowBody="true">
                        <GetRowClass Handler="rowParams.body = '<p>' + record.data.Descripcion + '</p>'; return 'x-grid3-row-expanded';" />
                    </ext:GridView>
                </View>
                <SelectionModel>
                    <ext:CheckboxSelectionModel ID="CheckboxSelectionModel1" runat="server" />
                </SelectionModel>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingToolbar1" runat="server" StoreID="StoreParametrosMA" DisplayInfo="true" />
                </BottomBar>
            </ext:GridPanel>
        </West>
        <Center Split="true">
            <ext:Panel ID="PanelCentral" runat="server" Layout="FitLayout" Header="false">
                <TopBar>
                    <ext:Toolbar runat="server">
                        <Items>
                            <ext:Hidden ID="hdnIdValorPMA" runat="server" />
                            <ext:ComboBox ID="cBoxCadenaComercial" runat="server" EmptyText="Cadena Comercial" Width="200"
                                DisplayField="NombreORazonSocial" ValueField="ID_Colectiva" AllowBlank="false" Mode="Local"
                                AutoSelect="true" ForceSelection="true" TypeAhead="true" MinChars="1" ListWidth="250"
                                MatchFieldWidth="false" Name="colCCM">
                                <Store>
                                    <ext:Store ID="StoreCCM" runat="server">
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
                                </Store>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cBoxProductos" runat="server" Width="200" AllowBlank="false" EmptyText="Selecciona el Producto"
                                DisplayField="Descripcion" ValueField="ID_Producto" ListWidth="250">
                                <Store>
                                    <ext:Store ID="StoreProductos" runat="server">
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
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Before="var valid = (#{cBoxCadenaComercial}.isValid()) && (#{cBoxProductos}.isValid());
                                        if (!valid) { #{GridParamsAsignados}.setDisabled(true); #{btnAddParams}.setDisabled(true); }
                                        else { #{GridParamsAsignados}.setDisabled(false); #{btnAddParams}.setDisabled(false); } return valid;">
                                        <EventMask ShowMask="true" Msg="Buscando Parámetros..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarSeparator runat="server" />
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <Items>
                    <ext:GridPanel ID="GridParamsAsignados" runat="server" Layout="FitLayout" StripeRows="true"
                        Header="false" Border="false" Disabled="true" AutoExpandColumn="Descripcion">
                        <Store>
                            <ext:Store ID="StoreParamsAsignados" runat="server">
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_ValorParametroMultiasignacion">
                                        <Fields>
                                            <ext:RecordField Name="ID_ValorParametroMultiasignacion" />
                                            <ext:RecordField Name="ID_ParametroMultiasignacion" />
                                            <ext:RecordField Name="Clave" />
                                            <ext:RecordField Name="Descripcion" />
                                            <ext:RecordField Name="ID_Entidad" />
                                            <ext:RecordField Name="Entidad" />
                                            <ext:RecordField Name="ID_RegistroEntidad" />
                                            <ext:RecordField Name="DescripcionRegEntidad" />
                                            <ext:RecordField Name="Valor" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <ColumnModel ID="ColumnModel1" runat="server">
                            <Columns>
                                <ext:Column ColumnID="ID_ValorParametroMultiasignacion" runat="server" Hidden="true" DataIndex="ID_ValorParametroMultiasignacion" />
                                <ext:Column ColumnID="Clave" Header="Clave" Width="100" DataIndex="Clave" />
                                <ext:Column ColumnID="Descripcion" Header="Descripción" DataIndex="Descripcion" />
                                <ext:Column ColumnID="Entidad" Header="Entidad" DataIndex="Entidad" />
                                <ext:Column ColumnID="DescripcionRegEntidad" Header="Descripción Registro Entidad" DataIndex="DescripcionRegEntidad" />
                                <ext:Column ColumnID="Valor" Header="Valor" Sortable="true" DataIndex="Valor" />
                                <ext:CommandColumn Width="60">
                                    <Commands>
                                        <ext:GridCommand Icon="Pencil" CommandName="Edit">
                                            <ToolTip Text="Editar Valor" />
                                        </ext:GridCommand>
                                        <ext:GridCommand Icon="Delete" CommandName="Delete">
                                            <ToolTip Text="Quitar Parámetro " />
                                        </ext:GridCommand>
                                    </Commands>
                                </ext:CommandColumn>
                            </Columns>
                        </ColumnModel>
                        <SelectionModel>
                            <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true" />
                        </SelectionModel>
                        <BottomBar>
                            <ext:PagingToolbar ID="PagingToolBar3" runat="server" StoreID="StoreParamsAsignados" DisplayInfo="true"
                                DisplayMsg="Parámetros Multiasignación Asignados {0} - {1} de {2}" />
                        </BottomBar>
                        <DirectEvents>
                            <Command OnEvent="EjecutarComando">
                                <EventMask ShowMask="true" Msg="Procesando..." MinDelay="200" />
                                <Confirmation BeforeConfirm="if (command == 'Edit') return false;"
                                    ConfirmRequest="true" Title="Confirmación" Message="¿Estás seguro de quitar el Parámetro Multiasignación a la Cadena Comercial?">
                                    
                                </Confirmation>
                                <ExtraParams>
                                    <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                    <ext:Parameter Name="ID_ValorParametroMultiasignacion" Value="Ext.encode(record.data.ID_ValorParametroMultiasignacion)" Mode="Raw" />
                                    <ext:Parameter Name="Clave" Value="Ext.encode(record.data.Clave)" Mode="Raw" />
                                    <ext:Parameter Name="Descripcion" Value="Ext.encode(record.data.Descripcion)" Mode="Raw" />
                                    <ext:Parameter Name="ID_Entidad" Value="Ext.encode(record.data.ID_Entidad)" Mode="Raw" />
                                    <ext:Parameter Name="ID_RegistroEntidad" Value="Ext.encode(record.data.ID_RegistroEntidad)" Mode="Raw" />
                                    <ext:Parameter Name="Valor" Value="Ext.encode(record.data.Valor)" Mode="Raw" />
                                </ExtraParams>
                            </Command>
                        </DirectEvents>
                        <LoadMask ShowMask="false" />
                    </ext:GridPanel>
                </Items>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
