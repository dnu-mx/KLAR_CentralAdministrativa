<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="EdoCtaManual.aspx.cs" Inherits="Cajero.EdoCtaManual" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        var prepareCommand = function (grid, command, record, row) {
            // you can prepare group command
            if (command.command == 'Delete' && record.data.Price < 5) {
                command.hidden = true;
                command.hideMode = 'visibility'; //you can try 'display' also                 
            }
        };

        var prepareGroupCommand = function (grid, command, groupId, group) {
            // you can prepare group command
        };



        var prepareToolbar = function (grid, toolbar, rowIndex, record) {
            // for example hide 'Edit' button if price < 50
            if (record.get("ID_EstatusMovimiento") == 1) { //ACTIVO

                toolbar.items.get(0).hide(); //activar
                toolbar.items.get(1).hide(); //sep
                //toolbar.items.get(2).hide(); //asign
                toolbar.items.get(5).hide(); //sep
                toolbar.items.get(6).hide(); //delete
                toolbar.items.get(3).hide(); //sep
                toolbar.items.get(4).hide(); //quitar Resguardo
//                toolbar.items.get(7).hide(); //quitar Resguardo
//                toolbar.items.get(8).hide(); //quitar Resguardo


            } else if (record.get("ID_EstatusMovimiento") == 2) { //En proceso de Activar

                //            toolbar.items.get(0).hide(); //activar
                toolbar.items.get(1).hide(); //sep
                toolbar.items.get(2).hide(); //asign
                //            toolbar.items.get(5).hide(); //sep
                //            toolbar.items.get(6).hide(); //delete
                toolbar.items.get(3).hide(); //sep
                toolbar.items.get(4).hide(); //quitar Resguardo
                toolbar.items.get(7).hide(); //quitar Resguardo
                toolbar.items.get(8).hide(); //quitar Resguardo

            }
            else if (record.get("ID_EstatusMovimiento") == 3) { //En asignado

                toolbar.items.get(0).hide(); //activar
                toolbar.items.get(1).hide(); //sep
                toolbar.items.get(2).hide(); //asign
                toolbar.items.get(5).hide(); //sep
                toolbar.items.get(6).hide(); //delete
                toolbar.items.get(3).hide(); //sep
                toolbar.items.get(4).hide(); //quitar Resguardo
                toolbar.items.get(7).hide(); //quitar Resguardo
                toolbar.items.get(8).hide(); //quitar Resguardo

            } else if (record.get("ID_EstatusMovimiento") == 4) { //resguardo

                toolbar.items.get(0).hide(); //activar
                toolbar.items.get(1).hide(); //sep
                toolbar.items.get(2).hide(); //asign
                toolbar.items.get(5).hide(); //sep
                toolbar.items.get(6).hide(); //delete
                toolbar.items.get(3).hide(); //sep
                toolbar.items.get(4).hide();
                toolbar.items.get(7).hide(); //quitar Resguardo
                toolbar.items.get(8).hide(); //quitar Resguardo
                //             toolbar.items.get(4).hide(); //quitar Resguardo
            } else if (record.get("ID_EstatusMovimiento") == 5) { //Eliminado

                toolbar.items.get(0).hide(); //activar
                toolbar.items.get(1).hide(); //sep
                toolbar.items.get(2).hide(); //asign
                toolbar.items.get(3).hide(); //sep
                toolbar.items.get(4).hide(); //delete
                toolbar.items.get(5).hide(); //sep
                toolbar.items.get(6).hide(); //delete
                toolbar.items.get(7).hide(); //quitar Resguardo
                toolbar.items.get(8).hide(); //quitar Resguardo
            }


        };

    </script>
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true">
            <ext:Panel ID="Panel6" runat="server" Width="428.5" Title="Registrar Depósitos Bancarios"
                Border="True" AutoScroll="true" Padding="6">
                <Content>
                    <ext:Store ID="StoreBanco" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="Clave">
                                <Fields>
                                    <ext:RecordField Name="Clave" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:Store ID="StoreOper" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="Clave">
                                <Fields>
                                    <ext:RecordField Name="CodigoProceso" />
                                    <ext:RecordField Name="Descripcion" />
                                    <ext:RecordField Name="EventoAutorizador" />
                                    <ext:RecordField Name="GeneraReverso" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:FormPanel ID="FormPanel1" runat="server" Border="false">
                        <Items>
                            <ext:Panel ID="Panel1" runat="server" Title="Sucursal Bancaria" AutoHeight="true"
                                LabelAlign="Top" FormGroup="true" Layout="TableLayout">
                                <Items>
                                    <ext:Panel ID="Panel4" runat="server" Border="false" Header="false" Width="200px"
                                        ColumnWidth=".5" Layout="Form" LabelAlign="Top">
                                        <Items>
                                            <ext:ComboBox ID="cmbBanco" TabIndex="1" FieldLabel="Banco" StoreID="StoreBanco" EmptyText="Selecciona una Opción..."
                                                runat="server" Width="180" DisplayField="Descripcion" ValueField="Clave" Editable="false"
                                                AnchorHorizontal="90%" AllowBlank="false" MsgTarget="Side">
                                            </ext:ComboBox>
                                            <ext:TextField ID="txtCaja" MaxLength="20" TabIndex="3" Visible="false" FieldLabel="Caja" runat="server" Width="180"
                                                Text="" AnchorHorizontal="90%" />
                                            <ext:TextField ID="txtConsecutivo" MaxLength="20" TabIndex="3" FieldLabel="Consecutivo/Autorizacion" runat="server"
                                                Text="" Width="180" AnchorHorizontal="90%" AllowBlank="false" MsgTarget="Side" />
                                            <ext:DateField AllowBlank="false" Format="yyyy-MM-dd" TabIndex="5" FieldLabel="Fecha" ID="datFecha"
                                                EmptyText="Selecciona una Opción" ForceSelection="true" Editable="false" runat="server"
                                                Width="90" Vtype="daterange" AnchorHorizontal="90%" MsgTarget="Side" />
                                           
                                            <ext:TextField ID="txtImporte" MaxLength="12" TabIndex="7" runat="server" Width="180" FieldLabel="Importe"
                                                Text="" AnchorHorizontal="90%" AllowBlank="false" MsgTarget="Side" />
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel ID="Panel55" runat="server" Border="false" Header="false" ColumnWidth=".5"
                                        Width="200px" Layout="Form" LabelAlign="Top">
                                        <Items>
                                            <ext:TextField ID="txtSucursal" TabIndex="2" FieldLabel="Sucursal" MaxLength="20" runat="server"
                                                Width="180" Text="" AnchorHorizontal="90%" AllowBlank="false" MsgTarget="Side" />
                                            <ext:TextField ID="txtOperador" Visible="false" TabIndex="4" MaxLength="20" FieldLabel="Operador" runat="server"
                                                Text="" Width="180" AnchorHorizontal="90%" />
                                            <ext:TextField ID="txtReferencia" Visible="false"  MaxLength="50" TabIndex="6" FieldLabel="Referencia" runat="server"
                                                Width="180" Text="" AnchorHorizontal="90%" />
                                            <ext:TimeField ID="datHora" Visible="false" TabIndex="8" AllowBlank="false" FieldLabel="Hora" EmptyText="Selecciona una Opción"
                                                ForceSelection="true" Editable="false" runat="server" Width="80" MinTime="8:00"
                                                MaxTime="19:00" Increment="1" Format="HH:mm" AnchorHorizontal="90%" MsgTarget="Side" />
                                            <ext:TextField ID="txtCheque" MaxLength="50" TabIndex="4" runat="server" FieldLabel="Cheque" Width="180"
                                                Text="" AnchorHorizontal="90%" />
                                                 <ext:DateField ID="datFechaValor" runat="server" TabIndex="6" FieldLabel="Fecha Valor" Format="yyyy-MM-dd"
                                                EmptyText="Selecciona una Opción" ForceSelection="true" Editable="false" Width="180"
                                                Vtype="daterange" AnchorHorizontal="90%">
                                            </ext:DateField>
                                        </Items>
                                    </ext:Panel>
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel8" runat="server" Title="Observaciones" AutoHeight="true" Layout="TableLayout"
                                LabelAlign="Top" FormGroup="true">
                                <Items>
                                    <ext:TextArea FieldLabel="Ingresa un Comentario del Movimiento" TabIndex="12" ID="txtobservaciones"
                                        MaxLength="4000" runat="server" Height="70" AnchorHorizontal="90%" Width="390px" />
                                </Items>
                            </ext:Panel>
                        </Items>
                        <FooterBar>
                            <ext:Toolbar ID="Toolbar1" runat="server" EnableOverflow="true">
                                <Items>
                                    <ext:Button ID="btnGuardar" TabIndex="13" runat="server" Text="Guardar" Icon="Add">
                                        <DirectEvents>
                                            <Click OnEvent="btnGuardar_Click" Before="var valid= #{FormPanel1}.getForm().isValid(); if (!valid) {} return valid;" />
                                        </DirectEvents>
                                    </ext:Button>
                                    <%-- <ext:Button ID="btnCancelar" runat="server" Text="Cancelar" Icon="Cancel">
                                        <DirectEvents>
                                            <Click OnEvent="btnCancelar_Click" />
                                        </DirectEvents>
                                    </ext:Button>--%>
                                </Items>
                            </ext:Toolbar>
                        </FooterBar>
                    </ext:FormPanel>
                </Content>
            </ext:Panel>
        </West>
        <Center Split="true">
            <ext:Panel ID="Panel2" runat="server" Width="428.5" Collapsible="true" Title="Mis Depósitos Bancarios"
                Collapsed="false" Layout="Fit" AutoScroll="true">
                <Content>
                    <ext:Store ID="Store1" runat="server" OnRefreshData="RefreshGrid">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Movimiento">
                                <Fields>
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Content>
                <Items>
                    <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store1" StripeRows="true"
                        Header="false" Border="false">
                        <LoadMask ShowMask="false" />
                        <ColumnModel ID="ColumnModel1" runat="server">
                        </ColumnModel>
                        <DirectEvents>
                            <Command OnEvent="EjecutarComando">
                                <Confirmation ConfirmRequest="true"
                                    Message="¿Estas Seguro de Ejecutar la Accion Seleccionada?" Title="Confirmación de Acción" />
                                <ExtraParams>
                                    <ext:Parameter Name="id" Value="record.data.ID" Mode="Raw" />
                                </ExtraParams>
                            </Command>
                        </DirectEvents>
                        <SelectionModel>
                            <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                            </ext:RowSelectionModel>
                        </SelectionModel>
                        <BottomBar>
                            <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="Store1" DisplayInfo="true"
                                DisplayMsg="Fichas {0} - {1} de {2}" />
                        </BottomBar>
                    </ext:GridPanel>
                </Items>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
