<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ImportarListaNegraComercios.aspx.cs" Inherits="ValidacionesBatch.ImportarListaNegraComercios" %>
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
            <ext:Panel ID="Panel1" runat="server"  Layout="FitLayout"  Title="Importación de Archivo de Lista Negra">
                <Items>
                    <ext:Panel ID="Panel555" runat="server"   AutoScroll="true" Width="250">
                        <Items>
                            <ext:FormPanel ID="FormPanel2" Padding="10" runat="server" Border="false" LabelAlign="Top">
                                <Items>
                                    <ext:FileUploadField ID="FileSelect" FieldLabel="Selecciona el Archivo de Lista Negra"
                                        LabelAlign="Top" runat="server" Width="500" Icon="Attach" Regex="\d+|.(csv|txt)$"
                                        RegexText="Selecciona Archivo (.csv) o (.txt)">
                                    </ext:FileUploadField>
                                </Items>
                                <FooterBar>
                                    <ext:Toolbar ID="Toolbar1" runat="server" EnableOverflow="true">
                                        <Items>
                                            <ext:Button ID="btnUpload" runat="server" Text="Subir Archivo" Icon="Add">
                                                <DirectEvents>
                                                    <Click OnEvent="FileUploadField_FileSelected" IsUpload="true" Before="if (!#{FormPanel2}.getForm().isValid()) { return false; } 
                                                            Ext.Msg.wait('Almacenando registros para Proceso Nocturno...', 'Procesando Archivo');"
                                                        Success="Ext.Msg.hide(); Ext.Msg.show({ 
                                                            title   : 'Upload Exitoso', 
                                                            msg     : 'El Archivo Se almacenó correctamente, por favor espera a recibir el resultado del procesamiento por email', 
                                                            minWidth: 200, 
                                                            modal   : true, 
                                                            icon    : Ext.Msg.INFO, 
                                                            buttons : Ext.Msg.OK 
                                                        });"
                                                        Failure="Ext.Msg.show({ 
                                                            title   : 'Error', 
                                                            msg     : 'Ocurrio un Error al Intenar subir el Archivo seleccionado al Servidor', 
                                                            minWidth: 200, 
                                                            modal   : true, 
                                                            icon    : Ext.Msg.ERROR, 
                                                            buttons : Ext.Msg.OK 
                                                        });">
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="Add">
                                                <Listeners>
                                                    <Click Handler="#{FormPanel2}.getForm().reset();" />
                                                </Listeners>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </FooterBar>
                            </ext:FormPanel>
                        </Items>
                    </ext:Panel>
                </Items>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>

