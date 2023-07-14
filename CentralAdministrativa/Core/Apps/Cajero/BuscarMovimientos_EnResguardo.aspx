<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="BuscarMovimientos_EnResguardo.aspx.cs" Inherits="Cajero.BuscarMovimientos_EnResguardo" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript">
    var showResult = function (btn) {
        Ext.Msg.notify("Button Click", "You clicked the " + btn + " button");
    };

    var fnConfirmar = function (btn, text) {

        if (btn != "ok") {
            return;
        }

        if (!/^([0-9])*?[0-9]*$/.test(text)) {
            alert("La Ficha Ingresada no es un Valor Numérico Válido");
            return;
        }
        Cajero.ConfirmarOperacion(text);
    };

    var fnAsignar = function (btn) {
        if (btn == "ok") {
            Cajero.AsignarOperacion();
        }

    };
    </script> 


    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <North Split="true">
            <ext:FormPanel ID="FormPanel1" Height="80px" runat="server" Border="false">
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
                </Content>
                <Items>
                    <ext:Panel ID="Panel1" runat="server"  Border="false" Title="Buscar en los Movimientos en Resguardo" AutoHeight="true"
                        LabelAlign="Top"  Layout="TableLayout">
                        <Items>
                            <ext:Panel ID="Panel4" runat="server" Border="false" Header="false" Width="180px"
                                ColumnWidth=".5" Layout="Form" LabelAlign="Top">
                                <Items>
                                    <ext:ComboBox FieldLabel="Banco" ID="cmbBanco" Name="Banco" ForceSelection="true"
                                        EmptyText="Selecciona una Opción..." runat="server" Width="180" StoreID="StoreBanco"
                                        MsgTarget="Side"  DisplayField="Descripcion" ValueField="Clave" 
                                        Editable="false" AnchorHorizontal="90%">
                                    </ext:ComboBox>
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel55" runat="server" Border="false" Header="false" ColumnWidth=".5"
                                Width="150px" Layout="Form" LabelAlign="Top">
                                <Items>
                                    <ext:DateField FieldLabel="Fecha" Format="yyyy-MM-dd" Name="Fecha" ID="datFecha"
                                        AnchorHorizontal="90%" runat="server" Width="90" MsgTarget="Side" 
                                        EmptyText="Selecciona Fecha" AllowBlank="false" Vtype="daterange" />
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel65" runat="server" Border="false" Header="false" ColumnWidth=".5"
                                Width="150px" Layout="Form" LabelAlign="Top">
                                <Items>
                                    <ext:TextField FieldLabel="Sucursal" ID="txtSucursal" Name="Sucursal" MsgTarget="Side"
                                         MaxLength="50" runat="server" Width="80" Text="" AnchorHorizontal="90%" />
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel75" runat="server" Border="false" Header="false" ColumnWidth=".5"
                                Width="150px" Layout="Form" LabelAlign="Top">
                                <Items>
                                    <ext:TextField ID="txtConsecutivo" FieldLabel="Consecutivo/Autorizacion" Name="Consecutivo"
                                        MaxLength="50" AnchorHorizontal="90%" runat="server" MsgTarget="Side" 
                                        Text="" Width="80" />
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel85" runat="server" Border="false" Header="false" ColumnWidth=".5"
                                Width="150px" Layout="Form" LabelAlign="Top">
                                <Items>
                                    <ext:TextField Name="Importe" FieldLabel="Importe" MaxLength="12" ID="txtImporte"
                                        AnchorHorizontal="90%" runat="server" Width="80" MsgTarget="Side" 
                                        Text="" />
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel3" runat="server" Border="false" Header="false" ColumnWidth=".5"
                                Width="100px" Layout="Form" LabelAlign="Top">
                                <Items>
                                    <ext:Button ID="Button2" FieldLabel=" Buscar" runat="server" Text="Buscar" Icon="Magnifier">
                                        <DirectEvents>
                                            <Click OnEvent="btnBuscar_Click" Before="var valid= #{FormPanel1}.getForm().isValid(); if (!valid) {} return valid;">
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                </Items>
                            </ext:Panel>
                        </Items>
                    </ext:Panel>
                </Items>
                <%-- <FooterBar>
                    <ext:Toolbar ID="Toolbar1" runat="server" EnableOverflow="true">
                        <Items>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Before="var valid= #{FormPanel1}.getForm().isValid(); if (!valid) {} return valid;">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>--%>
            </ext:FormPanel>
        </North>
        <Center Split="true">
            <ext:Panel ID="Panel2" runat="server" Width="428.5" Collapsible="true" Title="Mis Movimientos En Resguardo"
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
                                <Confirmation ConfirmRequest="true" Message="¿Estas Seguro de Ejecutar la Accion Seleccionada?"
                                    Title="Confirmación de Acción" />
                                <ExtraParams>
                                    <ext:Parameter Name="id" Value="record.data.ID" Mode="Raw" />
                                </ExtraParams>
                            </Command>
                        </DirectEvents>
                        <%--                        <DirectEvents>
                            <RowDblClick OnEvent="Seleccionar">
                            </RowDblClick>
                        </DirectEvents>--%>
                        <SelectionModel>
                            <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                            </ext:RowSelectionModel>
                        </SelectionModel>
                        <BottomBar>
                            <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="Store1" DisplayInfo="true"
                                DisplayMsg="Movimientos en Resguardo {0} - {1} de {2}" />
                        </BottomBar>
                    </ext:GridPanel>
                </Items>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
