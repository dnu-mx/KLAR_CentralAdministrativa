<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="RegistroTelefonos.aspx.cs" Inherits="CentralMovil.RegistroTelefonos" %>

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

        var afterEditSuper = function (e) {
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
            if (e.record.data.Telefono.length < 10) {
                alert("El Telefono debe ser de 10 posiciones");

                return;
            }

            if (e.record.data.CadenaComercial.length = 0) {
                alert("Selecciona una Cadena Comercial");
                return;
            }


            // Call DirectMethod
            CentralMovil.AgregarTelefono(e.record.data.ID_Telefono, e.record.data.Telefono, e.record.data.CadenaComercial);
        };


        var afterEditOper = function (e) {
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

            if (e.record.data.Telefono.length < 10) {
                alert("El Telefono debe ser de 10 posiciones");

                return;
            }



            // Call DirectMethod
            CentralMovil.AgregarTelefono(0, e.record.data.Telefono, 0);
        };

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Viewport ID="ViewPort1" runat="server">
        <Items>
            <ext:BorderLayout ID="BorderLayout1" runat="server">
                <Center MarginsSummary="0 5 0 5">
                    <ext:Panel runat="server" Frame="true" Title="Supervisores" Icon="User" Layout="Fit">
                        <Content>
                            <ext:Store ID="Store3" runat="server">
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_Colectiva">
                                        <Fields>
                                            <ext:RecordField Name="Nombre" />
                                            <ext:RecordField Name="Telefono" />
                                            <ext:RecordField Name="Nombre" />
                                            <ext:RecordField Name="APaterno" />
                                            <ext:RecordField Name="AMaterno" />
                                            <ext:RecordField Name="FechaActivacion" DateFormat="dd-MM-yyyy" />
                                            <ext:RecordField Name="CadenaComercial" />
                                            <ext:RecordField Name="ID_CadenaComercial" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Content>
                        <Items>
                            <ext:GridPanel ID="GridPanel1" runat="server" StripeRows="true">
                                <Store>
                                    <ext:Store ID="Store1" runat="server" OnRefreshData="Store1_Refresh">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Telefono">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Telefono" />
                                                    <ext:RecordField Name="Telefono" />
                                                    <ext:RecordField Name="Nombre" />
                                                    <ext:RecordField Name="APaterno" />
                                                    <ext:RecordField Name="AMaterno" />
                                                    <ext:RecordField Name="FechaActivacion" DateFormat="dd-MM-yyyy" />
                                                    <ext:RecordField Name="CadenaComercial" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <ColumnModel runat="server">
                                    <Columns>
                                        <ext:CommandColumn ColumnID="acciones" Width="50" Header="">
                                            <Commands>
                                                <ext:GridCommand Icon="Delete" CommandName="DeletePago">
                                                    <ToolTip Text="Eliminar" />
                                                </ext:GridCommand>
                                            </Commands>
                                        </ext:CommandColumn>
                                        <ext:Column ColumnID="ID_Telefono" DataIndex="ID_Telefono" Header="ID">
                                        </ext:Column>
                                        <ext:Column DataIndex="Telefono" Header="Telefono">
                                            <Editor>
                                                <ext:TextField ID="txtTelefono" MaxLength="10" MaxLengthText="10" runat="server" />
                                            </Editor>
                                        </ext:Column>
                                        <ext:Column DataIndex="Nombre" Header="Nombre" />
                                        <ext:Column DataIndex="APaterno" Header="Apellido Paterno" />
                                        <ext:Column DataIndex="AMaterno" Header="Apellido Materno" />
                                        <ext:Column DataIndex="FechaActivacion" Header="Fecha Activacion" />
                                        <ext:Column DataIndex="CadenaComercial" Header="Cadena Comercial" Width="300">
                                            <Editor>
                                                <ext:ComboBox ID="cmbCadenaComercial"  EmptyText="Selecciona una Cadena" runat="server" StoreID="Store3" DisplayField="CadenaComercial"
                                                    ValueField="ID_CadenaComercial">
                                                </ext:ComboBox>
                                            </Editor>
                                            <EditorOptions>
                                                <Listeners>
                                                    <StartEdit Handler="if (Ext.isEmpty(value)) { this.field.select(this.field.getStore().getAt(0)); } this.field.onTriggerClick();"
                                                        Delay="1" />
                                                </Listeners>
                                            </EditorOptions>
                                        </ext:Column>
                                    </Columns>
                                </ColumnModel>
                                <DirectEvents>
                                    <Command OnEvent="EliminarTelefonoSup" IsUpload="true">
                                        <Confirmation ConfirmRequest="true" Title="Confirmación" Message="¿Confirmas que deseas Eliminar el Pago?" />
                                        <ExtraParams>
                                            <ext:Parameter Name="ID_Telefono" Value="record.data['ID_Telefono']" Mode="Raw" />
                                            <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                        </ExtraParams>
                                    </Command>
                                </DirectEvents>
                                <SelectionModel>
                                    <ext:RowSelectionModel runat="server" SingleSelect="true">
                                        <Listeners>
                                            <RowSelect Handler="if (#{pnlSouth}.isVisible()) {#{Store2}.reload();}" Buffer="250">
                                            </RowSelect>
                                        </Listeners>
                                    </ext:RowSelectionModel>
                                </SelectionModel>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" PageSize="10" />
                                </BottomBar>
                                <View>
                                    <ext:GroupingView ID="GroupingView2" runat="server" ForceFit="true" MarkDirty="false"
                                        ShowGroupName="false" EnableNoGroups="true" HideGroupedColumn="true" />
                                </View>
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar2" runat="server">
                                        <Items>
                                            <ext:Button ID="Button3" runat="server" Text="Nuevo Supervisor" Icon="Add">
                                                <Listeners>
                                                    <Click Handler="#{GridPanel1}.insertRecord();#{GridPanel1}.getRowEditor().startEditing(0);" />
                                                </Listeners>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <Plugins>
                                    <ext:RowEditor ID="RowEditor1" runat="server" SaveText="Guardar" CancelText="Cancelar">
                                        <Listeners>
                                            <AfterEdit Fn="afterEditSuper" />
                                        </Listeners>
                                    </ext:RowEditor>
                                    <ext:GridFilters runat="server" ID="GridFilters1" Local="true">
                                        <Filters>
                                            <ext:StringFilter DataIndex="Telefono" />
                                            <ext:StringFilter DataIndex="Nombre" />
                                            <ext:StringFilter DataIndex="APaterno" />
                                            <ext:StringFilter DataIndex="AMaterno" />
                                            <ext:DateFilter DataIndex="FechaActivacion">
                                                <DatePickerOptions runat="server" TodayText="Hoy" />
                                            </ext:DateFilter>
                                        </Filters>
                                    </ext:GridFilters>
                                </Plugins>
                                <LoadMask ShowMask="true" />
                            </ext:GridPanel>
                        </Items>
                    </ext:Panel>
                </Center>
                <South Collapsible="true" Split="true" MarginsSummary="0 5 5 5">
                    <ext:Panel ID="pnlSouth" runat="server" Title="Operadores del Supervisor " Height="350"
                        Icon="Group" Layout="Fit">
                        <Items>
                            <ext:GridPanel ID="GridPanel2" runat="server" StripeRows="true" Border="false">
                                <Store>
                                    <ext:Store ID="Store2" runat="server" OnRefreshData="Store2_Refresh">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Telefono">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Telefono" />
                                                    <ext:RecordField Name="Telefono">
                                                    </ext:RecordField>
                                                    <ext:RecordField Name="Nombre" />
                                                    <ext:RecordField Name="APaterno" />
                                                    <ext:RecordField Name="AMaterno" />
                                                    <ext:RecordField Name="FechaActivacion" DateFormat="dd-MM-yyyy" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                        <BaseParams>
                                            <ext:Parameter Name="ID_Telefono" Value="Ext.getCmp('#{GridPanel1}') && #{GridPanel1}.getSelectionModel().hasSelection() ? #{GridPanel1}.getSelectionModel().getSelected().id : -1"
                                                Mode="Raw" />
                                        </BaseParams>
                                        <Listeners>
                                            <LoadException Handler="Ext.Msg.alert('Error en la Carga de Telefonos', e.message || response.statusText);" />
                                        </Listeners>
                                    </ext:Store>
                                </Store>
                                <ColumnModel runat="server">
                                    <Columns>
                                        <ext:CommandColumn ColumnID="acciones" Width="50" Header="">
                                            <Commands>
                                                <ext:GridCommand Icon="Delete" CommandName="DeletePago">
                                                    <ToolTip Text="Eliminar" />
                                                </ext:GridCommand>
                                            </Commands>
                                        </ext:CommandColumn>
                                        <ext:Column ColumnID="ID_Telefono" DataIndex="ID_Telefono" Header="ID" />
                                        <ext:Column DataIndex="Telefono" Header="Telefono">
                                            <Editor>
                                                <ext:TextField ID="txtTelefonoOperador" MaxLength="10" MaxLengthText="10" runat="server" />
                                            </Editor>
                                        </ext:Column>
                                        <ext:Column DataIndex="Nombre" Header="Nombre" />
                                        <ext:Column DataIndex="APaterno" Header="Apellido Paterno" />
                                        <ext:Column DataIndex="AMaterno" Header="Apellido Materno" />
                                        <ext:Column DataIndex="FechaActivacion" Header="Fecha Activacion" />
                                    </Columns>
                                </ColumnModel>
                                <DirectEvents>
                                    <Command OnEvent="EliminarTelefonoOper" IsUpload="true">
                                        <Confirmation ConfirmRequest="true" Title="Confirmación" Message="¿Confirmas que deseas Eliminar el Pago?" />
                                        <ExtraParams>
                                            <ext:Parameter Name="ID_Telefono" Value="record.data['ID_Telefono']" Mode="Raw" />
                                            <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                        </ExtraParams>
                                    </Command>
                                </DirectEvents>
                                <LoadMask ShowMask="true" />
                                <SelectionModel>
                                    <ext:RowSelectionModel runat="server" SingleSelect="true" />
                                </SelectionModel>
                                <BottomBar>
                                    <ext:PagingToolbar runat="server" PageSize="20" />
                                </BottomBar>
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar1" runat="server">
                                        <Items>
                                            <ext:Button ID="Button1" runat="server" Text="Nuevo Operador" Icon="Add">
                                                <Listeners>
                                                    <Click Handler="#{GridPanel2}.insertRecord();#{GridPanel2}.getRowEditor().startEditing(0);" />
                                                </Listeners>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <Plugins>
                                    <ext:RowEditor ID="RowEditor2" runat="server" SaveText="Guardar" CancelText="Cancelar">
                                        <Listeners>
                                            <AfterEdit Fn="afterEditOper" />
                                        </Listeners>
                                    </ext:RowEditor>
                                     <ext:GridFilters runat="server" ID="GridFilters2" Local="true">
                                        <Filters>
                                            <ext:StringFilter DataIndex="Telefono" />
                                            <ext:StringFilter DataIndex="Nombre" />
                                            <ext:StringFilter DataIndex="APaterno" />
                                            <ext:StringFilter DataIndex="AMaterno" />
                                            <ext:DateFilter DataIndex="FechaActivacion">
                                                <DatePickerOptions runat="server" TodayText="Hoy" />
                                            </ext:DateFilter>
                                        </Filters>
                                    </ext:GridFilters>
                                </Plugins>
                                <View>
                                    <ext:GroupingView ID="GroupingView1" runat="server" ForceFit="true" MarkDirty="false"
                                        ShowGroupName="false" EnableNoGroups="true" HideGroupedColumn="true" />
                                </View>
                            </ext:GridPanel>
                        </Items>
                        <Listeners>
                            <Expand Handler="#{Store2}.reload();" />
                        </Listeners>
                    </ext:Panel>
                </South>
            </ext:BorderLayout>
        </Items>
    </ext:Viewport>
</asp:Content>
