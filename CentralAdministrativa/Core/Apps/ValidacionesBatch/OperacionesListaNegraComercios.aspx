<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="OperacionesListaNegraComercios.aspx.cs" Inherits="ValidacionesBatch.OperacionesListaNegraComercios" %>
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

        var afterEditTarjeta = function (e) {
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
            ValidacionesBatch.AfterEditTarjeta(e.record.data.ID_ListaNegraMA, e.record.data.Accion);
        };

        var afterEditComercio = function (e) {
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
            ValidacionesBatch.AfterEditComercio(e.record.data.ID_ListaNegraComercio, e.record.data.Accion);
        };

    </script>
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

            <ext:Store ID="StoreListaNegraTarjeta" runat="server">
                <Reader>
                    <ext:JsonReader IDProperty="ID_ListaNegraMA">
                        <Fields>
                            <ext:RecordField Name="ID_ListaNegraMA" />
                            <ext:RecordField Name="ClaveMA" />
                            <ext:RecordField Name="Accion" />

                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>

            <ext:Store ID="StoreListaNegraComercio" runat="server">
                <Reader>
                    <ext:JsonReader IDProperty="ID_ListaNegraComercio">
                        <Fields>
                            <ext:RecordField Name="ID_ListaNegraComercio" />
                            <ext:RecordField Name="ClaveComercio" />
                            <ext:RecordField Name="Accion" />

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
                                    <ext:Panel ID="Panel2" runat="server" Title="Lista Negra de Comercios" Split="true" Collapsible="false">
                                        <Items>
                                            <ext:BorderLayout ID="BorderLayout3" runat="server">
                                                <Center Split="true">
                                                    <ext:Panel ID="Panel4" runat="server" Title=""
                                                        Collapsed="false" Layout="Fit" AutoScroll="true">
                                                        <Items>
                                                            <ext:GridPanel ID="GridPanel1" runat="server" Layout="FitLayout" StoreID="StoreListaNegraComercio" StripeRows="true"
                                                                Header="false" Border="false">
                                                                <LoadMask ShowMask="true" />
                                                                <TopBar>
                                                                    <ext:Toolbar ID="Toolbar2" runat="server">
                                                                        <Items>
                                                                            <ext:TextField ID="txtComercio" EmptyText="Introduce el Nombre del Comercio" Width="400" runat="server" />
                                                                            <ext:Button ID="Button2" runat="server" Text="Buscar" Icon="Magnifier">
                                                                                <DirectEvents>
                                                                                    <Click OnEvent="btnBuscarListaNegraComercios_Click">
                                                                                    </Click>
                                                                                </DirectEvents>
                                                                            </ext:Button>
                                                                            <ext:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                                        </Items>
                                                                    </ext:Toolbar>
                                                                </TopBar>
                                                                <ColumnModel ID="ColumnModel1" runat="server">
                                                                    <Columns>
                                                                        <ext:Column ColumnID="ID_ListaNegraComercio" Header="ID"  Sortable="true" DataIndex="ID_ListaNegraComercio" />
                                                                        <ext:Column ColumnID="ClaveComercio" Header="Tarjeta" Width="350" Sortable="true" DataIndex="ClaveComercio" />
                                                                        <ext:Column ColumnID="Accion"  Header="Acciones: 1=Informa, 2=Bloquea, 3=Cancela" Width="300" Sortable="true" DataIndex="Accion">
                                                                            <Editor>
                                                                                <%--<ext:TextField ID="TextField5" EmptyText="" runat="server" />--%>
                                                                                <ext:ComboBox EmptyText="" runat="server" >
                                                                                    <Items > 
                                                                                        <ext:ListItem Text ="Autorizar" Value="0" />
                                                                                        <ext:ListItem Text ="Informar" Value="1" />
                                                                                        <ext:ListItem Text ="Bloquear" Value="2" />
                                                                                        <ext:ListItem Text ="Cancelar" Value="3" />
                                                                                    </Items>
                                                                                </ext:ComboBox>
                                                                            </Editor>
                                                                        </ext:Column>
                                                                        

                                                                    </Columns>
                                                                </ColumnModel>
                                                                <Listeners>
                                                                    <AfterEdit Fn="afterEditComercio" />
                                                                </Listeners>
                                                                <Plugins>
                                                                    <ext:RowEditor ID="RowEditor2" runat="server" SaveText="Actualizar" CancelText="Cancelar">
                                                                        <Listeners>
                                                                            <AfterEdit Fn="afterEditComercio" />
                                                                        </Listeners>
                                                                    </ext:RowEditor>
                                                                </Plugins>
                                                                <SelectionModel>
                                                                    <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                                                                    </ext:RowSelectionModel>
                                                                </SelectionModel>
                                                                <BottomBar>
                                                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreListaNegraComercio" DisplayInfo="true"
                                                                        DisplayMsg="Fichas {0} - {1} de {2}" />
                                                                </BottomBar>
                                                            </ext:GridPanel>
                                                        </Items>
                                                    </ext:Panel>
                                                </Center>
                                            </ext:BorderLayout>
                                        </Items>
                                    </ext:Panel>

                                </Items>
                            </ext:TabPanel>
                        </Center>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>

