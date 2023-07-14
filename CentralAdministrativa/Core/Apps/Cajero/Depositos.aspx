<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Depositos.aspx.cs" Inherits="Cajero.Depositos" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <style type="text/css">
        .cbStates-list
        {
            width: 298px;
            font: 11px tahoma,arial,helvetica,sans-serif;
        }
        
        .cbStates-list th
        {
            font-weight: bold;
        }
        
        .cbStates-list td, .cbStates-list th
        {
            padding: 3px;
        }
    </style>
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
            if (record.get("ID_EstatusFichaDeposito") == 1) { //ACTIVO
                //                toolbar.items.get(0).hide(); //Delete
                //                toolbar.items.get(1).hide(); //sep
                //                toolbar.items.get(2).hide(); //asgina
            } else if (record.get("ID_EstatusFichaDeposito") == 2) { //Asignado

                toolbar.items.get(0).hide(); //Delete
                toolbar.items.get(1).hide(); //sep
                toolbar.items.get(2).hide(); //asgina
            }
            else if (record.get("ID_EstatusFichaDeposito") == 3) { //Inactivo

                //toolbar.items.get(0).hide(); //Delete
                toolbar.items.get(1).hide(); //sep
                toolbar.items.get(2).hide(); //asgina
            } else if (record.get("ID_EstatusFichaDeposito") == 4) { //Eliminado

                toolbar.items.get(0).hide(); //Delete
                toolbar.items.get(1).hide(); //sep
                toolbar.items.get(2).hide(); //asgina
            }


        };

    </script>
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true">
            <ext:Panel ID="Panel6" runat="server" Width="428.5" Title="Agregar Ficha de Depósito"
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
                    <ext:Store ID="StoreOper" runat="server" OnRefreshData="Refreshcombo">
                        <Reader>
                            <ext:JsonReader IDProperty="Clave">
                                <Fields>
                                    <ext:RecordField Name="DescripcionCuenta" />
                                    <ext:RecordField Name="Cuenta" />
                                    <ext:RecordField Name="Value" />
                                    <ext:RecordField Name="DespliegaDatos" />
                                    <ext:RecordField Name="ID_CadenaComercial" />
                                    <ext:RecordField Name="NombreCadena" />
                                    <ext:RecordField Name="Afiliacion" />
                                    <ext:RecordField Name="GeneraReverso" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:Store ID="storeCCM" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="Clave">
                                <Fields>
                                    <ext:RecordField Name="Afiliacion" />
                                    <ext:RecordField Name="NombreCadenaComercial" />
                                    <ext:RecordField Name="ClaveCCM" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:Store ID="StoreTipoMA" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="Clave">
                                <Fields>
                                    <ext:RecordField Name="Clave" />
                                    <ext:RecordField Name="Descripcion" />
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
                                            <ext:ComboBox FieldLabel="Banco" ID="cmbBanco" TabIndex="1" Name="Banco" ForceSelection="true"
                                                EmptyText="Selecciona una Opción..." runat="server" Width="180" StoreID="StoreBanco"
                                                MsgTarget="Side" AllowBlank="false" DisplayField="Descripcion" ValueField="Clave"
                                                Editable="false" AnchorHorizontal="90%">
                                            </ext:ComboBox>
                                            <ext:TextField FieldLabel="Caja" ID="txtCaja" Visible="false" Name="Caja" MaxLength="20"
                                                runat="server" AnchorHorizontal="90%" Width="180" Text="" />
                                            <ext:TextField ID="txtConsecutivo" MaxLength="12" TabIndex="3" FieldLabel="Consecutivo/Autorización"
                                                Name="Consecutivo"  AnchorHorizontal="90%" runat="server" MsgTarget="Side"
                                                AllowBlank="false" Text="" Width="180" />
                                            <ext:TextField Name="Importe" TabIndex="5" FieldLabel="Importe" MaxLength="12" ID="txtImporte"
                                                AnchorHorizontal="90%" runat="server" Width="180" MsgTarget="Side" AllowBlank="false"
                                                Text="" />
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel ID="Panel55" runat="server" Border="false" Header="false" ColumnWidth=".5"
                                        Width="200px" Layout="Form" LabelAlign="Top">
                                        <Items>
                                            <ext:TextField FieldLabel="Sucursal" MaxLength="12" TabIndex="2" ID="txtSucursal" Name="Sucursal"
                                                MsgTarget="Side" AllowBlank="false"  runat="server" Width="180"
                                                Text="" AnchorHorizontal="90%" />
                                            <ext:DateField FieldLabel="Fecha" Format="yyyy-MM-dd" Name="Fecha" TabIndex="4" ID="datFecha"
                                                AnchorHorizontal="90%" runat="server" Width="90" MsgTarget="Side" AllowBlank="false"
                                                EmptyText="Selecciona una Opción" Vtype="daterange" />
                                            <ext:TextField ID="txtOperador" MaxLength="12" Visible="false" FieldLabel="Operador" Name="Operador"
                                                 AnchorHorizontal="90%" runat="server" Text="" Width="180" />
                                            <ext:TextField ID="txtReferencia" MaxLength="12" Visible="false" FieldLabel="Referencia" Name="Referencia"
                                                 AnchorHorizontal="90%" runat="server" Width="180" Text="" />
                                            <ext:TimeField FieldLabel="Hora" Visible="false" ID="datHora" Name="Hora" runat="server"
                                                EmptyText="Selecciona una Opción" ForceSelection="true" Editable="false" Width="80"
                                                AnchorHorizontal="90%" MinTime="8:00" MsgTarget="Side" AllowBlank="false" MaxTime="19:00"
                                                Increment="1" Format="HH:mm" />
                                        </Items>
                                    </ext:Panel>
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel3" runat="server" Title="Destino del Depósito" AutoHeight="true"
                                LabelAlign="Top" FormGroup="true">
                                <Items>
                                    <ext:ComboBox ID="cmbTipoCuentaAbono" TabIndex="6" FieldLabel="Selecciona la Opción deseada"
                                        Editable="false" ForceSelection="true" AllowBlank="false" MsgTarget="Side" EmptyText="Selecciona una Opción..."
                                        StoreID="StoreOper" runat="server" Width="370" DisplayField="DespliegaDatos"
                                        ValueField="Value" Resizable="true" TypeAhead="true" Mode="Local" MinChars="1" ListWidth="300"
                                        PageSize="10" ItemSelector="tr.list-item">
                                        <Template ID="Template1" runat="server">
                                            <html>
                                                <head>
                                                  <title>Depositoa</title>
                                                </head>
                                                <tpl for=".">
						                            <tpl if="[xindex] == 1">
							                            <table class="cbStates-list">
                                                            <caption></caption>
								                            <tr>
									                            <th id="cadenaComercial">Cadena Comercial</th>
									                            <th id="tipoCuenta">Tipo de Cuenta</th>
								                            </tr>
						                            </tpl>
						                            <tr class="list-item">
							                            <td style="padding:2px 0px;">{NombreCadena}</td>
							                            <td>{DespliegaDatos}</td>
						                            </tr>
						                            <tpl if="[xcount-xindex]==0">
							                            </table>
						                            </tpl>
					                            </tpl>
                                            </html>
                                        </Template>
                                        <Triggers>
                                            <ext:FieldTrigger Icon="Clear" HideTrigger="true" />
                                        </Triggers>
                                        <Listeners>
                                            <BeforeQuery Handler="this.triggers[0][ this.getRawValue().toString().length == 0 ? 'hide' : 'show']();" />
                                            <TriggerClick Handler="if (index == 0) { this.focus().clearValue(); trigger.hide();}" />
                                            <Select Handler="this.triggers[0].show();" />
                                        </Listeners>
                                    </ext:ComboBox>
                                    <%-- <ext:ComboBox ID="cmbCadenaComercial" TabIndex="6" Name="CCMAfiliacion" FieldLabel="Selecciona la Cadena Comercial"
                                        Editable="false" ForceSelection="true" AllowBlank="false" MsgTarget="Side" EmptyText="Selecciona una Opción..."
                                        StoreID="storeCCM" runat="server" Width="370" DisplayField="NombreCadenaComercial" ValueField="Afiliacion">
                                    </ext:ComboBox>--%>
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel8" runat="server" Title="Observaciones" AutoHeight="true" FormGroup="true"
                                LabelAlign="Top">
                                <Items>
                                    <ext:TextArea TabIndex="7" FieldLabel="Ingresa una observación referente a la Ficha de Depósito"
                                        ID="txtObservaciones" Name="Observaciones" MaxLength="4000" runat="server" Width="370px"
                                        Height="80px" />
                                </Items>
                            </ext:Panel>
                        </Items>
                        <FooterBar>
                            <ext:Toolbar ID="Toolbar1" runat="server" EnableOverflow="true">
                                <Items>
                                    <ext:Button ID="btnGuardar" runat="server" Text="Guardar" Icon="Add">
                                        <DirectEvents>
                                            <Click OnEvent="btnGuardar_Click" Before="var valid= #{FormPanel1}.getForm().isValid(); if (!valid) {} return valid;">
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnCancelar" runat="server" Text="Cancelar" Icon="Cancel">
                                        <DirectEvents>
                                            <Click OnEvent="btnCancelar_Click" />
                                        </DirectEvents>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </FooterBar>
                    </ext:FormPanel>
                </Content>
            </ext:Panel>
        </West>
        <Center Split="true">
            <ext:Panel ID="Panel2" runat="server" Width="428.5" Collapsible="true" Title="Mis Fichas de Depositos Bancarios"
                Collapsed="false" Layout="Fit" AutoScroll="true">
                <Content>
                    <ext:Store ID="Store1" runat="server" OnRefreshData="RefreshGrid">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_FichaDeposito">
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
                            <Columns>
                                <%--<ext:CommandColumn runat="server" Width="100" >
                                    <Commands>
                                        <ext:GridCommand Icon="Delete" CommandName="Delete">
                                            <ToolTip Text="Delete" />
                                        </ext:GridCommand>
                                        <ext:CommandSeparator />
                                        <ext:GridCommand Icon="NoteEdit" CommandName="Edit">
                                            <ToolTip Text="Edit" />
                                        </ext:GridCommand>
                                    </Commands>
                                    <PrepareToolbar Fn="prepareToolbar" />
                                </ext:CommandColumn>--%>
                            </Columns>
                        </ColumnModel>
                        <DirectEvents>
                            <Command OnEvent="EjecutarComando">
                                <Confirmation BeforeConfirm="if (command=='Eliminar') return false;" ConfirmRequest="true"
                                    Message="¿Estas Seguro que deseas Intentar Asignar el Registro?" Title="Asingación Ficha Deposito" />
                                <Confirmation BeforeConfirm="if (command=='Asignar') return false;" ConfirmRequest="true"
                                    Message="¿Estas Seguro que deseas Eliminar el Registro?" Title="Eliminar Ficha Deposito" />
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
