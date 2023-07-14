<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RechazosEnCortoTiempo.aspx.cs" Inherits="ValidacionesBatch.RechazosEnCortoTiempo" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">

        var template = '<span style="color:{0};">{1}</span>';

        var change = function (value) {
            return String.format(template, (value > 0) ? "green" : "red", value);
        };

        var pctChange = function (value) {
            return String.format(template, (value > 0) ? "green" : "red", value + "%");
        };

        var startEditing = function (e) {
            if (e.getKey() == e.ENTER) {
                var grid = GridPanel1,
                    record = grid.getSelectionModel().getSelected(),
                    index = grid.store.indexOf(record);

                grid.startEditing(index, 1);
            }
        };

        var Prueba = function (o, e) {
            if (e.getKey() == e.ENTER) {
                var grid = GridPanel1,
                    record = grid.getSelectionModel().getSelected(),
                    index = grid.store.indexOf(record);

                grid.startEditing(index, 1);
            }
        };


        function validaFloat(numero) {
            if (!/^([0-9])*[.]?[0-9]*$/.test(numero))
                alert("El valor " + numero + " no es un número");
        }

        var prepareToolbar = function (grid, toolbar, rowIndex, record) {
            // for example hide 'Edit' button if price < 50

            if (record.get("EditarSaldoGrid") == true) { //ACTIVO
                toolbar.items.get(1).hide(); //sep
            } else if (record.get("EditarSaldoGrid") == false) { //Asignado
                toolbar.items.get(0).hide(); //Delete
                //grid.colModel.columns[7].editor = null;
            }

        };

        var afterEdit = function (e) {
            /*
            Properties of 'e' include:
            e.grid - This grid
            e.record - The record being edited
            e.field - The field name being edited
            e.value - The value being set
            e.originalValue - The original value for the field, before the edit.
            e.row - The grid row index
            e.column - The grid column index
            */



            // Call DirectMethod
            ValidacionesBatch.AfterEdit(e.record.data.ID_Valor, e.record.data.ID_Parametro, e.record.data.ValorAlertar, e.record.data.ValorBloquear, e.record.data.ValorCancelar, e.record.data.ID_Entidad, e.record.data.ID_EntidadEnTabla, e.record.data.ID_CadenaComercial, e.record.data.ID_Regla);
        };


        var fullName = function (value, metadata, record, rowIndex, colIndex, store) {
            return "<b>" + record.data.Nombre + "</b>";
        };




    </script>
    <style type="text/css">
        .x-grid3-td-Nombre .x-grid3-cell-inner {
            font-family: tahoma, verdana;
            display: block;
            font-weight: normal;
            font-style: normal;
            color: blue;
            white-space: normal;
        }

        .x-grid3-row-body p {
            margin: 5px 5px 10px 5px !important;
            width: 99%;
            color: black;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Content>
            <ext:Store ID="StoreCadenaComercial" runat="server">
                <Reader>
                    <ext:JsonReader IDProperty="ID_Colectiva">
                        <Fields>
                            <ext:RecordField Name="NombreORazonSocial" />
                            <ext:RecordField Name="ClaveColectiva" />
                            <ext:RecordField Name="ID_Colectiva" />
                        </Fields>
                    </ext:JsonReader>
                </Reader>
                <SortInfo Field="NombreORazonSocial" Direction="ASC" />
            </ext:Store>
            <ext:Store ID="stEventosManuales" runat="server">
                <Reader>
                    <ext:JsonReader IDProperty="ID_Evento">
                        <Fields>
                            <ext:RecordField Name="ID_Evento" />
                            <ext:RecordField Name="Clave" />
                            <ext:RecordField Name="Descripcion" />
                            <ext:RecordField Name="DescMostrar" />
                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>
            <ext:Store ID="stContratos" runat="server">
                <Reader>
                    <ext:JsonReader IDProperty="ID_Contrato">
                        <Fields>
                            <ext:RecordField Name="ID_Contrato" />
                            <ext:RecordField Name="Clave" />
                            <ext:RecordField Name="Descripcion" />
                            <ext:RecordField Name="CadenaComercial" />
                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>
            <ext:Store ID="StoreValoresTipoCuenta" runat="server" GroupField="Nombre">
                <Reader>
                    <ext:JsonReader IDProperty="ID_ReglaValorRegla">
                        <Fields>
                            <ext:RecordField Name="ID_ReglaValorRegla" />
                            <ext:RecordField Name="Nombre" />
                            <ext:RecordField Name="Descripcion" />
                            <ext:RecordField Name="ValorAlertar" />
                            <ext:RecordField Name="ValorBloquear" />
                            <ext:RecordField Name="ValorCancelar" />
                            <ext:RecordField Name="ID_Valor" />
                            <ext:RecordField Name="ID_EntidadEnTabla" />
                            <ext:RecordField Name="ID_Regla" />
                            <ext:RecordField Name="ID_CadenaComercial" />
                            <ext:RecordField Name="ID_Entidad" />
                            <ext:RecordField Name="ID_Parametro" />

                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>
            <ext:Store ID="StoreValoresGrupoCuenta" runat="server" GroupField="Nombre">
                <Reader>
                    <ext:JsonReader IDProperty="ID_ReglaValorRegla">
                        <Fields>
                            <ext:RecordField Name="ID_ReglaValorRegla" />
                            <ext:RecordField Name="Nombre" />
                            <ext:RecordField Name="Descripcion" />
                            <ext:RecordField Name="ValorAlertar" />
                            <ext:RecordField Name="ValorBloquear" />
                            <ext:RecordField Name="ValorCancelar" />
                            <ext:RecordField Name="ID_Valor" />
                            <ext:RecordField Name="ID_EntidadEnTabla" />
                            <ext:RecordField Name="ID_Regla" />
                            <ext:RecordField Name="ID_CadenaComercial" />
                            <ext:RecordField Name="ID_Entidad" />
                            <ext:RecordField Name="ID_Parametro" />

                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>

            <ext:Store ID="StoreValoresGrupoMA" runat="server" GroupField="Nombre">
                <Reader>
                    <ext:JsonReader IDProperty="ID_ReglaValorRegla">
                        <Fields>
                            <ext:RecordField Name="ID_ReglaValorRegla" />
                            <ext:RecordField Name="Nombre" />
                            <ext:RecordField Name="Descripcion" />
                            <ext:RecordField Name="ValorAlertar" />
                            <ext:RecordField Name="ValorBloquear" />
                            <ext:RecordField Name="ValorCancelar" />
                            <ext:RecordField Name="ID_Valor" />
                            <ext:RecordField Name="ID_EntidadEnTabla" />
                            <ext:RecordField Name="ID_Regla" />
                            <ext:RecordField Name="ID_CadenaComercial" />
                            <ext:RecordField Name="ID_Entidad" />
                            <ext:RecordField Name="ID_Parametro" />

                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>
            <ext:Store ID="StoreValoresMA" runat="server" GroupField="Nombre">
                <Reader>
                    <ext:JsonReader IDProperty="ID_ReglaValorRegla">
                        <Fields>
                            <ext:RecordField Name="ID_ReglaValorRegla" />
                            <ext:RecordField Name="Nombre" />
                            <ext:RecordField Name="Descripcion" />
                            <ext:RecordField Name="ValorAlertar" />
                            <ext:RecordField Name="ValorBloquear" />
                            <ext:RecordField Name="ValorCancelar" />
                            <ext:RecordField Name="ID_Valor" />
                            <ext:RecordField Name="ID_EntidadEnTabla" />
                            <ext:RecordField Name="ID_Regla" />
                            <ext:RecordField Name="ID_CadenaComercial" />
                            <ext:RecordField Name="ID_Entidad" />
                            <ext:RecordField Name="ID_Parametro" />

                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>

            <ext:Store ID="StoreValoresRegla" runat="server" GroupField="Nombre">
                <Reader>
                    <ext:JsonReader IDProperty="ID_ReglaValorRegla">
                        <Fields>
                            <ext:RecordField Name="ID_ReglaValorRegla" />
                            <ext:RecordField Name="Nombre" />
                            <ext:RecordField Name="Descripcion" />
                            <ext:RecordField Name="ValorAlertar" />
                            <ext:RecordField Name="ValorBloquear" />
                            <ext:RecordField Name="ValorCancelar" />
                            <ext:RecordField Name="ID_Valor" />
                            <ext:RecordField Name="ID_EntidadEnTabla" />
                            <ext:RecordField Name="ID_Regla" />
                            <ext:RecordField Name="ID_CadenaComercial" />
                            <ext:RecordField Name="ID_Entidad" />
                            <ext:RecordField Name="ID_Parametro" />
                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>

            <ext:Store ID="StoreReglas" runat="server">
                <Reader>
                    <ext:JsonReader IDProperty="ID_Regla">
                        <Fields>
                            <ext:RecordField Name="ID_Regla" />
                            <ext:RecordField Name="Nombre" />
                            <ext:RecordField Name="StoreProcedure" />
                            <ext:RecordField Name="OrdenEjecucion" />
                            <ext:RecordField Name="EsActiva" />
                            <ext:RecordField Name="EsAccion" />
                            <ext:RecordField Name="DescripcionRegla" />
                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>

            <ext:Store ID="StoreGrupoCuenta" runat="server">
                <Reader>
                    <ext:JsonReader IDProperty="ID">
                        <Fields>
                            <ext:RecordField Name="ID" />
                            <ext:RecordField Name="Clave" />
                            <ext:RecordField Name="Descripcion" />
                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>

        </Content>
        <Center Split="true">
            <ext:Panel ID="Panel1" runat="server" Title=" ">
                <Items>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <West Split="true">
                            <ext:FormPanel ID="FormPanel1" runat="server" Border="false" Width="380">
                                <Items>
                                    <ext:RowLayout ID="DatosRegla2" runat="server" FitHeight="true">
                                        <Rows>
                                            <ext:LayoutRow>
                                                <ext:Image ID="Image1" runat="server" Width="380" Height="100" ImageUrl="Images/Encabezado_1.jpeg" />
                                            </ext:LayoutRow>
                                            <ext:LayoutRow>

                                                <ext:FormPanel ID="FormDatos" runat="server" Border="false" Padding="10" LabelAlign="Top">
                                                    <Items>
                                                        <ext:TextArea ID="TxtNombreRegla" FieldLabel="Nombre Regla" LabelAlign="Top" AnchorHorizontal="95%" runat="server" />
                                                        <ext:TextArea ID="TxtDescripcion" FieldLabel="Descripción" LabelAlign="Top" AnchorHorizontal="95%" runat="server" />
                                                        <ext:ComboBox ID="cmbCadenaComercial" TabIndex="3" LabelAlign="Top" FieldLabel="Cadena Comercial" EmptyText="Selecciona una Cadena"
                                                            Resizable="true" AnchorHorizontal="95%" runat="server" StoreID="StoreCadenaComercial"
                                                            DisplayField="NombreORazonSocial" ValueField="ID_Colectiva">
                                                            <DirectEvents>
                                                                <Select OnEvent="SeleccionarCadena_Click">
                                                                </Select>
                                                            </DirectEvents>
                                                        </ext:ComboBox>
                                                    </Items>
                                                </ext:FormPanel>

                                            </ext:LayoutRow>
                                        </Rows>

                                    </ext:RowLayout>
                                </Items>
                            </ext:FormPanel>
                        </West>
                        <Center Split="true">
                            <ext:TabPanel ID="Pages2" Title="Valores de Parámetros" runat="server" Border="false" EnableTabScroll="true" AutoScroll="true">
                                <Items>
                                    <ext:Panel ID="PanelRegla" runat="server" Title="1. Nivel Regla" Split="true" Collapsible="false">
                                        <Items>
                                            <ext:BorderLayout ID="BorderLayout3" runat="server">
                                                <Center Split="true">
                                                    <ext:Panel ID="Panel5" runat="server" Title="Valores de los Parámetros del Nivel."
                                                        Collapsed="false" Layout="Fit" AutoScroll="true">
                                                        <Items>
                                                            <ext:GridPanel ID="GridPanel2" runat="server" Layout="FitLayout" StoreID="StoreValoresRegla" StripeRows="true"
                                                                Header="false" Border="false">

                                                                <ColumnModel ID="ColumnModel2" runat="server">
                                                                    <Columns>
                                                                        <ext:CommandColumn ColumnID="acciones" Width="50" Header="">
                                                                            <Commands>
                                                                                <ext:GridCommand Icon="Delete" CommandName="EliminarValor">
                                                                                    <ToolTip Text="Eliminar Valores" />
                                                                                </ext:GridCommand>
                                                                            </Commands>
                                                                        </ext:CommandColumn>
                                                                        <ext:Column ColumnID="Nombre" Header="Parámetro" Width="150" DataIndex="Nombre">
                                                                            <Renderer Fn="fullName" />
                                                                        </ext:Column>
                                                                        <%--<ext:Column ColumnID="Descripción" Header="Nombre" Width="150" Sortable="true" DataIndex="Descripcion" />--%>
                                                                        <ext:Column ColumnID="ValorInfo" Header="Informar" Width="100" Sortable="true" DataIndex="ValorAlertar">
                                                                            <Editor>
                                                                                <ext:TextField ID="TextField3" EmptyText="" runat="server" />
                                                                            </Editor>
                                                                        </ext:Column>
                                                                        <ext:Column ColumnID="ValorBloquea" Header="Bloquear" Width="100" Sortable="true" DataIndex="ValorBloquear">
                                                                            <Editor>
                                                                                <ext:TextField ID="TextField4" EmptyText="" runat="server" />
                                                                            </Editor>
                                                                        </ext:Column>
                                                                        <ext:Column ColumnID="ValorCancela" Header="Cancelar" Width="100" Sortable="true" DataIndex="ValorCancelar">
                                                                            <Editor>
                                                                                <ext:TextField ID="TextField8" EmptyText="" runat="server" />
                                                                            </Editor>
                                                                        </ext:Column>

                                                                    </Columns>
                                                                </ColumnModel>
                                                                <View>
                                                                    <ext:GridView runat="server" EnableRowBody="true">
                                                                        <GetRowClass Handler="rowParams.body = '<p>' + record.data.Descripcion + '</p>'; return 'x-grid3-row-expanded';" />
                                                                    </ext:GridView>
                                                                </View>
                                                                <DirectEvents>
                                                                    <Command OnEvent="EjecutarAccionValorRegla" IsUpload="true">
                                                                        <Confirmation ConfirmRequest="true" Title="Confirmación" Message="¿Confirmas Ejecutar la Acción Seleccionada?" />
                                                                        <ExtraParams>
                                                                            <ext:Parameter Name="ID_ValorReglaValor" Value="record.data['ID_Valor']" Mode="Raw" />
                                                                            <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                                                        </ExtraParams>
                                                                    </Command>
                                                                </DirectEvents>
                                                                <Listeners>
                                                                    <AfterEdit Fn="afterEdit" />
                                                                </Listeners>
                                                                <Plugins>
                                                                    <ext:RowEditor ID="RowEditor2" runat="server" SaveText="Actualizar" CancelText="Cancelar">
                                                                        <Listeners>
                                                                            <AfterEdit Fn="afterEdit" />
                                                                        </Listeners>
                                                                    </ext:RowEditor>
                                                                </Plugins>
                                                                <SelectionModel>
                                                                    <ext:RowSelectionModel ID="RowSelectionModel2" runat="server" SingleSelect="true">
                                                                    </ext:RowSelectionModel>
                                                                </SelectionModel>
                                                                <BottomBar>
                                                                    <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="StoreValoresRegla" DisplayInfo="true"
                                                                        DisplayMsg="Fichas {0} - {1} de {2}" />
                                                                </BottomBar>
                                                            </ext:GridPanel>
                                                        </Items>
                                                    </ext:Panel>
                                                </Center>
                                            </ext:BorderLayout>
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel ID="PanelTipoCuenta" runat="server" Title="2. Nivel Tipo Cuenta" Split="true" Collapsible="false">
                                        <Items>
                                            <ext:BorderLayout ID="BorderLayout4" runat="server">
                                                <North Split="true">
                                                    <ext:Panel ID="Panel222" runat="server" Layout="FitLayout" Height="200" Title="" Border="True" AutoScroll="true">
                                                        <Items>
                                                            <ext:FormPanel ID="FormPanel333" Layout="FitLayout" Height="200" runat="server" Border="false">
                                                                <Items>
                                                                    <ext:GridPanel ID="gridTipoCuenta" runat="server" Height="200" Layout="FitLayout" StripeRows="true"
                                                                        Header="false" Border="false">
                                                                        <Store>
                                                                            <ext:Store ID="StoreTipoCuenta" runat="server">
                                                                                <Reader>
                                                                                    <ext:JsonReader IDProperty="ID">
                                                                                        <Fields>
                                                                                            <ext:RecordField Name="ID" />
                                                                                            <ext:RecordField Name="Clave" />
                                                                                            <ext:RecordField Name="Descripcion" />
                                                                                        </Fields>
                                                                                    </ext:JsonReader>
                                                                                </Reader>
                                                                            </ext:Store>
                                                                        </Store>
                                                                        <LoadMask ShowMask="false" />
                                                                        <TopBar>
                                                                            <ext:Toolbar ID="Toolbar31" runat="server">
                                                                                <Items>
                                                                                    <ext:TextField ID="txtClaveTipoCuenta" LabelAlign="Top" EmptyText="Clave" Width="60" runat="server" />
                                                                                    <ext:TextField ID="txtDescTipoCuenta" EmptyText="Descripción" Width="200" runat="server" />
                                                                                    <ext:Button ID="Button21" runat="server" Text="Buscar" Icon="Magnifier">
                                                                                        <DirectEvents>
                                                                                            <Click OnEvent="btnBuscarTipoCuenta_Click">
                                                                                            </Click>
                                                                                        </DirectEvents>
                                                                                    </ext:Button>
                                                                                    <ext:ToolbarFill ID="ToolbarFill31" runat="server" />
                                                                                </Items>
                                                                            </ext:Toolbar>
                                                                        </TopBar>
                                                                        <ColumnModel ID="ColumnModel3" runat="server">
                                                                            <Columns>
                                                                                <%-- <ext:CommandColumn runat="server" Width="30">
                                                                                    <Commands>
                                                                                        <ext:GridCommand Icon="DiskEdit" CommandName="Select">
                                                                                            <ToolTip Text="Selecciona Parámetros" />
                                                                                        </ext:GridCommand>
                                                                                        <ext:CommandSeparator />
                                                                                        <ext:GridCommand Icon="NoteEdit" CommandName="Edit">
                                                                                            <ToolTip Text="Edit" />
                                                                                        </ext:GridCommand>
                                                                                    </Commands>
                                                                                </ext:CommandColumn>--%>
                                                                                <ext:Column ColumnID="Clave" Header="Clave" Sortable="true" Width="100" DataIndex="Clave" />
                                                                                <ext:Column ColumnID="Descripción" Header="Descripción" Width="300" Sortable="true" DataIndex="Descripcion" />
                                                                            </Columns>
                                                                        </ColumnModel>

                                                                        <DirectEvents>
                                                                            <Command OnEvent="EjecutarComando" IsUpload="true">
                                                                                <ExtraParams>
                                                                                    <ext:Parameter Name="ID" Value="record.data['ID']" Mode="Raw" />
                                                                                    <ext:Parameter Name="Titulo" Value="record.data['Descripcion']" Mode="Raw" />
                                                                                    <ext:Parameter Name="ID_Entidad" Value="2" Mode="Raw" />
                                                                                    <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                                                                </ExtraParams>
                                                                            </Command>
                                                                        </DirectEvents>

                                                                        <SelectionModel>

                                                                            <ext:RowSelectionModel ID="RowSeleccion" runat="server" SingleSelect="true">
                                                                                <DirectEvents>
                                                                                    <RowSelect OnEvent="EjecutarComando" Buffer="100">
                                                                                        <EventMask ShowMask="true" Target="CustomTarget" CustomTarget="#{GridPanel3}" />
                                                                                        <ExtraParams>
                                                                                            <ext:Parameter Name="ID" Value="record.data['ID']" Mode="Raw" />
                                                                                            <ext:Parameter Name="Titulo" Value="record.data['Descripcion']" Mode="Raw" />
                                                                                            <ext:Parameter Name="ID_Entidad" Value="2" Mode="Raw" />
                                                                                            <ext:Parameter Name="Comando" Value="Select" Mode="Value" />
                                                                                        </ExtraParams>
                                                                                    </RowSelect>
                                                                                </DirectEvents>
                                                                            </ext:RowSelectionModel>
                                                                        </SelectionModel>
                                                                    </ext:GridPanel>
                                                                </Items>

                                                            </ext:FormPanel>
                                                        </Items>
                                                    </ext:Panel>
                                                </North>
                                                <Center Split="true">
                                                    <ext:Panel ID="Panel3" runat="server" Title="." Collapsed="false" Layout="Fit" AutoScroll="true">
                                                        <Items>
                                                            <ext:GridPanel ID="GridPanel3" runat="server" Layout="FitLayout" StoreID="StoreValoresTipoCuenta" StripeRows="true"
                                                                Header="false" Border="false">
                                                                <LoadMask ShowMask="true" />
                                                                <ColumnModel ID="ColumnModel4" runat="server">
                                                                    <Columns>
                                                                        <ext:CommandColumn ColumnID="acciones" Width="50" Header="">
                                                                            <Commands>
                                                                                <ext:GridCommand Icon="Delete" CommandName="EliminarValor">
                                                                                    <ToolTip Text="Eliminar Valores" />
                                                                                </ext:GridCommand>
                                                                            </Commands>
                                                                        </ext:CommandColumn>
                                                                        <ext:Column ColumnID="Nombre" Header="Parámetro" Width="150" DataIndex="Nombre">
                                                                            <Renderer Fn="fullName" />
                                                                        </ext:Column>
                                                                        <%--<ext:Column ColumnID="Descripción" Header="Nombre" Width="150" Sortable="true" DataIndex="Descripcion" />--%>
                                                                        <ext:Column ColumnID="ValorInfo" Header="Informar" Width="100" Sortable="true" DataIndex="ValorAlertar">
                                                                            <Editor>
                                                                                <ext:TextField ID="TextField5" EmptyText="" runat="server" />
                                                                            </Editor>
                                                                        </ext:Column>
                                                                        <ext:Column ColumnID="ValorBloquea" Header="Bloquear" Width="100" Sortable="true" DataIndex="ValorBloquear">
                                                                            <Editor>
                                                                                <ext:TextField ID="TextField6" EmptyText="" runat="server" />
                                                                            </Editor>
                                                                        </ext:Column>
                                                                        <ext:Column ColumnID="ValorCancela" Header="Cancelar" Width="100" Sortable="true" DataIndex="ValorCancelar">
                                                                            <Editor>
                                                                                <ext:TextField ID="TextField7" EmptyText="" runat="server" />
                                                                            </Editor>
                                                                        </ext:Column>

                                                                    </Columns>
                                                                </ColumnModel>
                                                                <View>
                                                                    <ext:GridView runat="server" EnableRowBody="true">
                                                                        <GetRowClass Handler="rowParams.body = '<p>' + record.data.Descripcion + '</p>'; return 'x-grid3-row-expanded';" />
                                                                    </ext:GridView>
                                                                </View>
                                                                <Listeners>
                                                                    <AfterEdit Fn="afterEdit" />
                                                                </Listeners>
                                                                <DirectEvents>
                                                                    <Command OnEvent="EjecutarAccionValorRegla" IsUpload="true">
                                                                        <Confirmation ConfirmRequest="true" Title="Confirmación" Message="¿Confirmas Ejecutar la Acción Seleccionada?" />
                                                                        <ExtraParams>
                                                                            <ext:Parameter Name="ID_ValorReglaValor" Value="record.data['ID_Valor']" Mode="Raw" />
                                                                            <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                                                        </ExtraParams>
                                                                    </Command>
                                                                </DirectEvents>
                                                                <Plugins>
                                                                    <ext:RowEditor ID="RowEditor1" runat="server" SaveText="Actualizar" CancelText="Cancelar">
                                                                        <Listeners>
                                                                            <AfterEdit Fn="afterEdit" />
                                                                        </Listeners>
                                                                    </ext:RowEditor>
                                                                </Plugins>
                                                                <SelectionModel>
                                                                    <ext:RowSelectionModel ID="RowSelectionModel4" runat="server" SingleSelect="true">
                                                                    </ext:RowSelectionModel>
                                                                </SelectionModel>
                                                                <BottomBar>
                                                                    <ext:PagingToolbar ID="PagingToolBar4" runat="server" StoreID="StoreValoresTipoCuenta" DisplayInfo="true"
                                                                        DisplayMsg="Fichas {0} - {1} de {2}" />
                                                                </BottomBar>
                                                            </ext:GridPanel>
                                                        </Items>
                                                    </ext:Panel>
                                                </Center>
                                            </ext:BorderLayout>
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel ID="PanelGrupoCuenta" runat="server" Title="3. Nivel Grupo de Cuenta" Split="true" Collapsible="false">
                                        <Items>
                                            <ext:BorderLayout ID="BorderLayoutGrupoCta" runat="server">
                                                <North Split="true">
                                                    <ext:Panel ID="PanelgpoCta" runat="server" Height="200" Layout="FitLayout" Title=""
                                                        Border="True" AutoScroll="true">
                                                        <Items>
                                                            <ext:FormPanel ID="FormPanelgrupoCta" Layout="FitLayout" Height="200" runat="server" Border="false">
                                                                <Content>
                                                                </Content>
                                                                <Items>
                                                                    <ext:GridPanel ID="gridGrupoCuenta" runat="server" Height="200" Layout="FitLayout" StoreID="StoreGrupoCuenta" StripeRows="true"
                                                                        Header="false" Border="false">
                                                                        <LoadMask ShowMask="false" />
                                                                        <TopBar>
                                                                            <ext:Toolbar ID="Toolbar11" runat="server">
                                                                                <Items>
                                                                                    <ext:TextField ID="txtClaveGrupoCuenta" EmptyText="Clave" Width="100" runat="server" />
                                                                                    <ext:TextField ID="txtDescripcionGrupoCuenta" EmptyText="Descripción" Width="200" runat="server" />
                                                                                    <ext:Button ID="Button11" runat="server" Text="Buscar" Icon="Magnifier">
                                                                                        <DirectEvents>
                                                                                            <Click OnEvent="btnBuscarGrupoCuenta_Click">
                                                                                            </Click>
                                                                                        </DirectEvents>
                                                                                    </ext:Button>
                                                                                    <ext:ToolbarFill ID="ToolbarFill12" runat="server" />
                                                                                </Items>
                                                                            </ext:Toolbar>
                                                                        </TopBar>
                                                                        <ColumnModel ID="ColumnModel31" runat="server">
                                                                            <Columns>
                                                                                <%--  <ext:CommandColumn runat="server" Width="30">
                                                                                    <Commands>
                                                                                        <ext:GridCommand Icon="DiskEdit" CommandName="Select">
                                                                                            <ToolTip Text="Selecciona Parámetros" />
                                                                                        </ext:GridCommand>
                                                                                        <ext:CommandSeparator />
                                                                                        <ext:GridCommand Icon="NoteEdit" CommandName="Edit">
                                                                                            <ToolTip Text="Edit" />
                                                                                        </ext:GridCommand>
                                                                                    </Commands>
                                                                                </ext:CommandColumn>--%>
                                                                                <ext:Column ColumnID="Clave" Header="Clave" Sortable="true" Width="100" DataIndex="Clave" />
                                                                                <ext:Column ColumnID="Descripción" Header="Descripción" Width="300" Sortable="true" DataIndex="Descripcion" />
                                                                            </Columns>
                                                                        </ColumnModel>
                                                                        <DirectEvents>
                                                                            <Command OnEvent="EjecutarComando" IsUpload="true">
                                                                                <ExtraParams>
                                                                                    <ext:Parameter Name="ID" Value="record.data['ID']" Mode="Raw" />
                                                                                    <ext:Parameter Name="Titulo" Value="record.data['Descripcion']" Mode="Raw" />
                                                                                    <ext:Parameter Name="ID_Entidad" Value="3" Mode="Raw" />
                                                                                    <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                                                                </ExtraParams>
                                                                            </Command>
                                                                        </DirectEvents>
                                                                        <SelectionModel>
                                                                            <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                                                                                <DirectEvents>
                                                                                    <RowSelect OnEvent="EjecutarComando" Buffer="100">
                                                                                        <EventMask ShowMask="true" Target="CustomTarget" CustomTarget="#{GridPanel312}" />
                                                                                        <ExtraParams>
                                                                                            <ext:Parameter Name="ID" Value="record.data['ID']" Mode="Raw" />
                                                                                            <ext:Parameter Name="Titulo" Value="record.data['Descripcion']" Mode="Raw" />
                                                                                            <ext:Parameter Name="ID_Entidad" Value="3" Mode="Raw" />
                                                                                            <ext:Parameter Name="Comando" Value="Select" Mode="Value" />
                                                                                        </ExtraParams>
                                                                                    </RowSelect>
                                                                                </DirectEvents>
                                                                            </ext:RowSelectionModel>
                                                                        </SelectionModel>
                                                                        <%-- <BottomBar>
                                                                            <ext:PagingToolbar ID="PagingToolBar31" runat="server" StoreID="StoreGrupoCuenta" DisplayInfo="true"
                                                                                DisplayMsg="Fichas {0} - {1} de {2}" />
                                                                        </BottomBar>--%>
                                                                    </ext:GridPanel>
                                                                </Items>

                                                            </ext:FormPanel>
                                                        </Items>
                                                    </ext:Panel>
                                                </North>
                                                <Center Split="true">
                                                    <ext:Panel ID="Panel312" runat="server" Title="."
                                                        Collapsed="false" Layout="Fit" AutoScroll="true">
                                                        <Items>
                                                            <ext:GridPanel ID="GridPanel312" runat="server" Layout="FitLayout" StoreID="StoreValoresGrupoCuenta" StripeRows="true"
                                                                Header="false" Border="false">
                                                                <LoadMask ShowMask="true" />
                                                                <ColumnModel ID="ColumnModel412" runat="server">
                                                                    <Columns>
                                                                        <ext:CommandColumn ColumnID="acciones" Width="50" Header="">
                                                                            <Commands>
                                                                                <ext:GridCommand Icon="Delete" CommandName="EliminarValor">
                                                                                    <ToolTip Text="Eliminar Valores" />
                                                                                </ext:GridCommand>
                                                                            </Commands>
                                                                        </ext:CommandColumn>
                                                                        <ext:Column ColumnID="Nombre" Header="Parámetro" Width="150" DataIndex="Nombre">
                                                                            <Renderer Fn="fullName" />
                                                                        </ext:Column>
                                                                        <%--<ext:Column ColumnID="Descripción" Header="Nombre" Width="150" Sortable="true" DataIndex="Descripcion" />--%>
                                                                        <ext:Column ColumnID="ValorInfo" Header="Informar" Sortable="true" DataIndex="ValorAlertar">
                                                                            <Editor>
                                                                                <ext:TextField ID="TextField512" EmptyText="" runat="server" />
                                                                            </Editor>
                                                                        </ext:Column>
                                                                        <ext:Column ColumnID="ValorBloquea" Header="Bloquear" Sortable="true" DataIndex="ValorBloquear">
                                                                            <Editor>
                                                                                <ext:TextField ID="TextField126" EmptyText="" runat="server" />
                                                                            </Editor>
                                                                        </ext:Column>
                                                                        <ext:Column ColumnID="ValorCancela" Header="Cancelar" Sortable="true" DataIndex="ValorCancelar">
                                                                            <Editor>
                                                                                <ext:TextField ID="TextField712" EmptyText="" runat="server" />
                                                                            </Editor>
                                                                        </ext:Column>

                                                                    </Columns>
                                                                </ColumnModel>
                                                                <Plugins>
                                                                    <ext:RowEditor ID="RowEditor3" runat="server" SaveText="Actualizar" CancelText="Cancelar">
                                                                        <Listeners>
                                                                            <AfterEdit Fn="afterEdit" />
                                                                        </Listeners>
                                                                    </ext:RowEditor>
                                                                </Plugins>
                                                                <View>
                                                                    <ext:GridView runat="server" EnableRowBody="true">
                                                                        <GetRowClass Handler="rowParams.body = '<p>' + record.data.Descripcion + '</p>'; return 'x-grid3-row-expanded';" />
                                                                    </ext:GridView>
                                                                </View>
                                                                <DirectEvents>
                                                                    <Command OnEvent="EjecutarAccionValorRegla" IsUpload="true">
                                                                        <Confirmation ConfirmRequest="true" Title="Confirmación" Message="¿Confirmas Ejecutar la Acción Seleccionada?" />
                                                                        <ExtraParams>
                                                                            <ext:Parameter Name="ID_ValorReglaValor" Value="record.data['ID_Valor']" Mode="Raw" />
                                                                            <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                                                        </ExtraParams>
                                                                    </Command>
                                                                </DirectEvents>
                                                                <SelectionModel>
                                                                    <ext:RowSelectionModel ID="RowSelectionModel412" runat="server" SingleSelect="true">
                                                                    </ext:RowSelectionModel>
                                                                </SelectionModel>
                                                                <%--   <BottomBar>
                                                                    <ext:PagingToolbar ID="PagingToolBar412" runat="server" StoreID="StoreValoresGrupoCuenta" DisplayInfo="true"
                                                                        DisplayMsg="Fichas {0} - {1} de {2}" />
                                                                </BottomBar>--%>
                                                            </ext:GridPanel>
                                                        </Items>
                                                    </ext:Panel>
                                                </Center>
                                            </ext:BorderLayout>
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel ID="PanelGrupoTarjeta" runat="server" Title="4. Nivel Grupo Tarjeta" Split="true" Collapsible="false">
                                        <Items>
                                            <ext:BorderLayout ID="BorderLayout5" runat="server">
                                                <North Split="true">
                                                    <ext:Panel ID="Panel4" runat="server" Height="200" Layout="FitLayout" Title=""
                                                        Border="True" AutoScroll="true">
                                                        <Items>
                                                            <ext:FormPanel ID="FormPanel4" Layout="FitLayout" Height="200" runat="server" Border="false">
                                                                <Content>
                                                                    <ext:Store ID="StoreGrupoMA" runat="server">
                                                                        <Reader>
                                                                            <ext:JsonReader IDProperty="ID">
                                                                                <Fields>
                                                                                    <ext:RecordField Name="ID" />
                                                                                    <ext:RecordField Name="Clave" />
                                                                                    <ext:RecordField Name="Descripcion" />
                                                                                </Fields>
                                                                            </ext:JsonReader>
                                                                        </Reader>
                                                                    </ext:Store>
                                                                </Content>
                                                                <Items>
                                                                    <ext:GridPanel ID="gridGrupoMA" runat="server" Layout="FitLayout" Height="200" StoreID="StoreGrupoMA" StripeRows="true"
                                                                        Header="false" Border="false">
                                                                        <LoadMask ShowMask="false" />
                                                                        <TopBar>
                                                                            <ext:Toolbar ID="Toolbar3" runat="server">
                                                                                <Items>
                                                                                    <ext:TextField ID="txtClaveGrupoMA" EmptyText="Clave" Width="100" runat="server" />
                                                                                    <ext:TextField ID="txtDescripcionGrupoMA" EmptyText="Descripción" Width="200" runat="server" />
                                                                                    <ext:Button ID="Button2" runat="server" Text="Buscar" Icon="Magnifier">
                                                                                        <DirectEvents>
                                                                                            <Click OnEvent="btnBuscarGrupoMA_Click">
                                                                                            </Click>
                                                                                        </DirectEvents>
                                                                                    </ext:Button>
                                                                                    <ext:ToolbarFill ID="ToolbarFill3" runat="server" />
                                                                                </Items>
                                                                            </ext:Toolbar>
                                                                        </TopBar>
                                                                        <ColumnModel ID="ColumnModel5" runat="server">
                                                                            <Columns>
                                                                                <%--<ext:CommandColumn runat="server" Width="30">
                                                                                    <Commands>
                                                                                        <ext:GridCommand Icon="DiskEdit" CommandName="Select">
                                                                                            <ToolTip Text="Selecciona Parámetros" />
                                                                                        </ext:GridCommand>
                                                                                        <ext:CommandSeparator />
                                                                                        <ext:GridCommand Icon="NoteEdit" CommandName="Edit">
                                                                                            <ToolTip Text="Edit" />
                                                                                        </ext:GridCommand>
                                                                                    </Commands>
                                                                                </ext:CommandColumn>--%>
                                                                                <ext:Column ColumnID="Clave" Header="Clave" Sortable="true" DataIndex="Clave" />
                                                                                <ext:Column ColumnID="Descripción" Header="Descripción" Width="300" Sortable="true" DataIndex="Descripcion" />
                                                                            </Columns>
                                                                        </ColumnModel>
                                                                        <DirectEvents>
                                                                            <Command OnEvent="EjecutarComando" IsUpload="true">
                                                                                <ExtraParams>
                                                                                    <ext:Parameter Name="ID" Value="record.data['ID']" Mode="Raw" />
                                                                                    <ext:Parameter Name="Titulo" Value="record.data['Descripcion']" Mode="Raw" />
                                                                                    <ext:Parameter Name="ID_Entidad" Value="4" Mode="Raw" />
                                                                                    <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                                                                </ExtraParams>
                                                                            </Command>
                                                                        </DirectEvents>

                                                                        <SelectionModel>
                                                                            <ext:RowSelectionModel ID="RowSelectionModel3" runat="server" SingleSelect="true">
                                                                                <DirectEvents>
                                                                                    <RowSelect OnEvent="EjecutarComando" Buffer="100">
                                                                                        <EventMask ShowMask="true" Target="CustomTarget" CustomTarget="#{GridPanel5}" />
                                                                                        <ExtraParams>
                                                                                            <ext:Parameter Name="ID" Value="record.data['ID']" Mode="Raw" />
                                                                                            <ext:Parameter Name="Titulo" Value="record.data['Descripcion']" Mode="Raw" />
                                                                                            <ext:Parameter Name="ID_Entidad" Value="4" Mode="Raw" />
                                                                                            <ext:Parameter Name="Comando" Value="Select" Mode="Value" />
                                                                                        </ExtraParams>
                                                                                    </RowSelect>
                                                                                </DirectEvents>
                                                                            </ext:RowSelectionModel>
                                                                        </SelectionModel>
                                                                        <%--  <BottomBar>
                                                                            <ext:PagingToolbar ID="PagingToolBar5" runat="server" StoreID="StoreGrupoMA" DisplayInfo="true"
                                                                                DisplayMsg="Fichas {0} - {1} de {2}" />
                                                                        </BottomBar>--%>
                                                                    </ext:GridPanel>
                                                                </Items>

                                                            </ext:FormPanel>
                                                        </Items>
                                                    </ext:Panel>
                                                </North>
                                                <Center Split="true">
                                                    <ext:Panel ID="Panel7" runat="server" Title="."
                                                        Collapsed="false" Layout="Fit" AutoScroll="true">
                                                        <Items>
                                                            <ext:GridPanel ID="GridPanel5" runat="server" Layout="FitLayout" StoreID="StoreValoresGrupoMA" StripeRows="true"
                                                                Header="false" Border="false">
                                                                <LoadMask ShowMask="true" />
                                                                <ColumnModel ID="ColumnModel6" runat="server">
                                                                    <Columns>
                                                                        <%--             <ext:CommandColumn runat="server" Width="30">
                                                                            <Commands>
                                                                                <ext:GridCommand Icon="DiskEdit" CommandName="Select">
                                                                                    <ToolTip Text="Selecciona Parámetros" />
                                                                                </ext:GridCommand>
                                                                                <ext:CommandSeparator />
                                                                                <ext:GridCommand Icon="NoteEdit" CommandName="Edit">
                                                                                    <ToolTip Text="Edit" />
                                                                                </ext:GridCommand>
                                                                            </Commands>
                                                                        </ext:CommandColumn>
                                                                        <ext:Column ColumnID="Nombre" Header="Nombre" Sortable="true" DataIndex="Nombre" />--%>
                                                                        <ext:CommandColumn ColumnID="acciones" Width="50" Header="">
                                                                            <Commands>
                                                                                <ext:GridCommand Icon="Delete" CommandName="EliminarValor">
                                                                                    <ToolTip Text="Eliminar Valores" />
                                                                                </ext:GridCommand>
                                                                            </Commands>
                                                                        </ext:CommandColumn>
                                                                        <ext:Column ColumnID="Nombre" Header="Parámetro" Width="150" DataIndex="Nombre">
                                                                            <Renderer Fn="fullName" />
                                                                        </ext:Column>
                                                                        <%--<ext:Column ColumnID="Descripción" Header="Nombre" Width="150" Sortable="true" DataIndex="Descripcion" />--%>
                                                                        <ext:Column ColumnID="ValorInfo" Header="Informar" Sortable="true" DataIndex="ValorAlertar">
                                                                            <Editor>
                                                                                <ext:TextField ID="TextField10" EmptyText="" runat="server" />
                                                                            </Editor>
                                                                        </ext:Column>
                                                                        <ext:Column ColumnID="ValorBloquea" Header="Bloquear" Sortable="true" DataIndex="ValorBloquear">
                                                                            <Editor>
                                                                                <ext:TextField ID="TextField11" EmptyText="" runat="server" />
                                                                            </Editor>
                                                                        </ext:Column>
                                                                        <ext:Column ColumnID="ValorCancela" Header="Cancelar" Sortable="true" DataIndex="ValorCancelar">
                                                                            <Editor>
                                                                                <ext:TextField ID="TextField12" EmptyText="" runat="server" />
                                                                            </Editor>
                                                                        </ext:Column>

                                                                    </Columns>
                                                                </ColumnModel>
                                                                <View>
                                                                    <ext:GridView runat="server" EnableRowBody="true">
                                                                        <GetRowClass Handler="rowParams.body = '<p>' + record.data.Descripcion + '</p>'; return 'x-grid3-row-expanded';" />
                                                                    </ext:GridView>
                                                                </View>
                                                                <DirectEvents>
                                                                    <Command OnEvent="EjecutarAccionValorRegla" IsUpload="true">
                                                                        <Confirmation ConfirmRequest="true" Title="Confirmación" Message="¿Confirmas Ejecutar la Acción Seleccionada?" />
                                                                        <ExtraParams>
                                                                            <ext:Parameter Name="ID_ValorReglaValor" Value="record.data['ID_Valor']" Mode="Raw" />
                                                                            <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                                                        </ExtraParams>
                                                                    </Command>
                                                                </DirectEvents>
                                                                <SelectionModel>
                                                                    <ext:RowSelectionModel ID="RowSelectionModel6" runat="server" SingleSelect="true">
                                                                    </ext:RowSelectionModel>
                                                                </SelectionModel>
                                                                <Plugins>
                                                                    <ext:RowEditor ID="RowEditor4" runat="server" SaveText="Actualizar" CancelText="Cancelar">
                                                                        <Listeners>
                                                                            <AfterEdit Fn="afterEdit" />
                                                                        </Listeners>
                                                                    </ext:RowEditor>
                                                                </Plugins>
                                                                <%-- <BottomBar>
                                                                    <ext:PagingToolbar ID="PagingToolBar6" runat="server" StoreID="StoreValoresGrupoMA" DisplayInfo="true"
                                                                        DisplayMsg="Fichas {0} - {1} de {2}" />
                                                                </BottomBar>--%>
                                                            </ext:GridPanel>
                                                        </Items>
                                                    </ext:Panel>
                                                </Center>
                                            </ext:BorderLayout>
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel ID="PanelTarjeta" runat="server" Title="5. Nivel Tarjeta" Split="true" Collapsible="false">
                                        <Items>
                                            <ext:BorderLayout ID="BorderLayout6" runat="server">
                                                <North Split="true">
                                                    <ext:Panel ID="Panel8" runat="server" Height="200" Layout="FitLayout" Title="Catalogo"
                                                        Border="True" AutoScroll="true">
                                                        <Items>
                                                            <ext:FormPanel ID="FormPanel5" Layout="FitLayout" Height="200" runat="server" Border="false">
                                                                <Content>
                                                                    <ext:Store ID="StoreMA" runat="server">
                                                                        <Reader>
                                                                            <ext:JsonReader IDProperty="ID">
                                                                                <Fields>
                                                                                    <ext:RecordField Name="ID" />
                                                                                    <ext:RecordField Name="Clave" />
                                                                                    <ext:RecordField Name="Descripcion" />
                                                                                </Fields>
                                                                            </ext:JsonReader>
                                                                        </Reader>
                                                                    </ext:Store>
                                                                </Content>
                                                                <Items>
                                                                    <ext:GridPanel ID="gridTarjeta" runat="server" Layout="FitLayout" Height="200" StoreID="StoreMA" StripeRows="true"
                                                                        Header="false" Border="false">
                                                                        <LoadMask ShowMask="false" />
                                                                        <TopBar>
                                                                            <ext:Toolbar ID="Toolbar4" runat="server">
                                                                                <Items>
                                                                                    <ext:TextField ID="txtClaveMA" EmptyText="Clave" Width="200" runat="server" />
                                                                                    <ext:TextField ID="txtDescripcionMA" EmptyText="Descripción" Width="200" runat="server" />
                                                                                    <ext:Button ID="Button3" runat="server" Text="Buscar" Icon="Magnifier">
                                                                                        <DirectEvents>
                                                                                            <Click OnEvent="btnBuscarTarjeta_Click">
                                                                                            </Click>
                                                                                        </DirectEvents>
                                                                                    </ext:Button>
                                                                                    <ext:ToolbarFill ID="ToolbarFill4" runat="server" />
                                                                                </Items>
                                                                            </ext:Toolbar>
                                                                        </TopBar>
                                                                        <ColumnModel ID="ColumnModel7" runat="server">
                                                                            <Columns>
                                                                                <%--  <ext:CommandColumn runat="server" Width="30">
                                                                                    <Commands>
                                                                                        <ext:GridCommand Icon="DiskEdit" CommandName="Select">
                                                                                            <ToolTip Text="Selecciona Parámetros" />
                                                                                        </ext:GridCommand>
                                                                                        <ext:CommandSeparator />
                                                                                        <ext:GridCommand Icon="NoteEdit" CommandName="Edit">
                                                                                            <ToolTip Text="Edit" />
                                                                                        </ext:GridCommand>
                                                                                    </Commands>
                                                                                </ext:CommandColumn>--%>
                                                                                <ext:Column ColumnID="Clave" Header="Clave" Width="100" Sortable="true" DataIndex="Clave" />
                                                                                <ext:Column ColumnID="Descripción" Header="Descripción" Width="300" Sortable="true" DataIndex="Descripcion" />
                                                                            </Columns>
                                                                        </ColumnModel>
                                                                        <DirectEvents>
                                                                            <Command OnEvent="EjecutarComando" IsUpload="true">
                                                                                <ExtraParams>
                                                                                    <ext:Parameter Name="ID" Value="record.data['ID']" Mode="Raw" />
                                                                                    <ext:Parameter Name="Titulo" Value="record.data['Descripcion']" Mode="Raw" />
                                                                                    <ext:Parameter Name="ID_Entidad" Value="5" Mode="Raw" />
                                                                                    <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                                                                </ExtraParams>
                                                                            </Command>
                                                                        </DirectEvents>
                                                                        <SelectionModel>
                                                                            <ext:RowSelectionModel ID="RowSelectionModel5" runat="server" SingleSelect="true">
                                                                                <DirectEvents>
                                                                                    <RowSelect OnEvent="EjecutarComando" Buffer="100">
                                                                                        <EventMask ShowMask="true" Target="CustomTarget" CustomTarget="#{GridPanel4}" />
                                                                                        <ExtraParams>
                                                                                            <ext:Parameter Name="ID" Value="record.data['ID']" Mode="Raw" />
                                                                                            <ext:Parameter Name="Titulo" Value="record.data['Descripcion']" Mode="Raw" />
                                                                                            <ext:Parameter Name="ID_Entidad" Value="5" Mode="Raw" />
                                                                                            <ext:Parameter Name="Comando" Value="Select" Mode="Value" />
                                                                                        </ExtraParams>
                                                                                    </RowSelect>
                                                                                </DirectEvents>
                                                                            </ext:RowSelectionModel>
                                                                        </SelectionModel>
                                                                        <%--   <BottomBar>
                                                                            <ext:PagingToolbar ID="PagingToolBar7" runat="server" StoreID="StoreMA" DisplayInfo="true"
                                                                                DisplayMsg="Fichas {0} - {1} de {2}" />
                                                                        </BottomBar>--%>
                                                                    </ext:GridPanel>
                                                                </Items>

                                                            </ext:FormPanel>
                                                        </Items>
                                                    </ext:Panel>
                                                </North>
                                                <Center Split="true">
                                                    <ext:Panel ID="Panel9" runat="server" Title="."
                                                        Collapsed="false" Layout="Fit" AutoScroll="true">
                                                        <Items>
                                                            <ext:GridPanel ID="GridPanel4" runat="server" Layout="FitLayout" StoreID="StoreValoresMA" StripeRows="true"
                                                                Header="false" Border="false">
                                                                <LoadMask ShowMask="true" />
                                                                <ColumnModel ID="ColumnModel8" runat="server">
                                                                    <Columns>
                                                                        <%-- <ext:CommandColumn runat="server" Width="30">
                                                                            <Commands>
                                                                                <ext:GridCommand Icon="DiskEdit" CommandName="Select">
                                                                                    <ToolTip Text="Selecciona Parámetros" />
                                                                                </ext:GridCommand>
                                                                                <ext:CommandSeparator />
                                                                                <ext:GridCommand Icon="NoteEdit" CommandName="Edit">
                                                                                    <ToolTip Text="Edit" />
                                                                                </ext:GridCommand>
                                                                            </Commands>
                                                                        </ext:CommandColumn>
                                                                        <ext:Column ColumnID="Nombre" Header="Nombre" Sortable="true" DataIndex="Nombre" />--%>
                                                                        <ext:CommandColumn ColumnID="acciones" Width="50" Header="">
                                                                            <Commands>
                                                                                <ext:GridCommand Icon="Delete" CommandName="EliminarValor">
                                                                                    <ToolTip Text="Eliminar Valores" />
                                                                                </ext:GridCommand>
                                                                            </Commands>
                                                                        </ext:CommandColumn>
                                                                        <ext:Column ColumnID="Nombre" Header="Parámetro" Width="150" DataIndex="Nombre">
                                                                            <Renderer Fn="fullName" />
                                                                        </ext:Column>
                                                                        <%--<ext:Column ColumnID="Descripción" Header="Nombre" Width="150" Sortable="true" DataIndex="Descripcion" />--%>
                                                                        <ext:Column ColumnID="ValorInfo" Header="Informar" Sortable="true" DataIndex="ValorAlertar">
                                                                            <Editor>
                                                                                <ext:TextField ID="TextField15" EmptyText="" runat="server" />
                                                                            </Editor>
                                                                        </ext:Column>
                                                                        <ext:Column ColumnID="ValorBloquea" Header="Bloquear" Sortable="true" DataIndex="ValorBloquear">
                                                                            <Editor>
                                                                                <ext:TextField ID="TextField16" EmptyText="" runat="server" />
                                                                            </Editor>
                                                                        </ext:Column>
                                                                        <ext:Column ColumnID="ValorCancela" Header="Cancelar" Sortable="true" DataIndex="ValorCancelar">
                                                                            <Editor>
                                                                                <ext:TextField ID="TextField17" EmptyText="" runat="server" />
                                                                            </Editor>
                                                                        </ext:Column>

                                                                    </Columns>
                                                                </ColumnModel>
                                                                <View>
                                                                    <ext:GridView runat="server" EnableRowBody="true">
                                                                        <GetRowClass Handler="rowParams.body = '<p>' + record.data.Descripcion + '</p>'; return 'x-grid3-row-expanded';" />
                                                                    </ext:GridView>
                                                                </View>
                                                                <SelectionModel>
                                                                    <ext:RowSelectionModel ID="RowSelectionModel8" runat="server" SingleSelect="true">
                                                                    </ext:RowSelectionModel>
                                                                </SelectionModel>
                                                                <DirectEvents>
                                                                    <Command OnEvent="EjecutarAccionValorRegla" IsUpload="true">
                                                                        <Confirmation ConfirmRequest="true" Title="Confirmación" Message="¿Confirmas Ejecutar la Acción Seleccionada?" />
                                                                        <ExtraParams>
                                                                            <ext:Parameter Name="ID_ValorReglaValor" Value="record.data['ID_Valor']" Mode="Raw" />
                                                                            <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                                                        </ExtraParams>
                                                                    </Command>
                                                                </DirectEvents>
                                                                <Plugins>
                                                                    <ext:RowEditor ID="RowEditor5" runat="server" SaveText="Actualizar" CancelText="Cancelar">
                                                                        <Listeners>
                                                                            <AfterEdit Fn="afterEdit" />
                                                                        </Listeners>
                                                                    </ext:RowEditor>
                                                                </Plugins>
                                                                <%-- <BottomBar>
                                                                    <ext:PagingToolbar ID="PagingToolBar8" runat="server" StoreID="StoreValoresMA" DisplayInfo="true"
                                                                        DisplayMsg="Fichas {0} - {1} de {2}" />
                                                                </BottomBar>--%>
                                                            </ext:GridPanel>
                                                        </Items>
                                                    </ext:Panel>
                                                </Center>
                                            </ext:BorderLayout>
                                        </Items>
                                    </ext:Panel>
                                </Items>
                                <DirectEvents>
                                    <TabChange OnEvent="Unnamed_Event" IsUpload="true"></TabChange>
                                </DirectEvents>
                            </ext:TabPanel>
                        </Center>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
