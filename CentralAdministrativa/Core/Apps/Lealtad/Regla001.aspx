<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Regla001.aspx.cs" Inherits="Lealtad.Regla001" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<%--<%@ Register Assembly="Coolite.Ext.Web" Namespace="Coolite.Ext.Web" TagPrefix="ext" %>--%>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .cbStates-list
        {
            width: auto;
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
        .Titulo
        {
            font-size: 20px;
            font-weight: bolder;
            font-family: Arial Unicode MS;
            color: Black;
        }
        
        .descripcion
        {
            font-size: 12px;
            text-justify: distribute;
            font-weight: normal;
            font-family: Arial Unicode MS;
            color: Black;
        }
    </style>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="BorderLayout2" runat="server">
        <Content>
            <ext:Label runat="server" ID="ID_REGLA" Text="17" Visible="false" Cls="descripcion"
                FieldLabel="Descripción">
            </ext:Label>
            <ext:Store ID="Store2" runat="server">
                <Reader>
                    <ext:JsonReader IDProperty="ID_Colectiva">
                        <Fields>
                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>
            <ext:Store ID="Store1" runat="server">
                <Reader>
                    <ext:ArrayReader IDProperty="valor">
                        <Fields>
                            <ext:RecordField Name="valor" />
                            <ext:RecordField Name="descripcion" />
                        </Fields>
                    </ext:ArrayReader>
                </Reader>
            </ext:Store>
        </Content>
        <Center Split="true">
            <ext:Panel ID="Panel6" runat="server" Layout="FitLayout" Title="Configuración de Regla">
                <Items>
                    <ext:BorderLayout ID="BorderLayout4" runat="server">
                        <North Split="true">
                            <ext:FormPanel ID="FormPanel1" runat="server" Border="false" Layout="ColumnLayout"
                                Height="210">
                                <Items>
                                    <ext:Panel ID="Panel1" runat="server" Border="false" Width="300" Height="210">
                                        <Items>
                                            <ext:Image ID="Image1" runat="server" Width="300" Height="210" ImageUrl="Images/promociones.jpg">
                                            </ext:Image>
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel ID="Panel2" runat="server" Border="false" Layout="RowLayout" AutoScroll="true">
                                        <Items>
                                            <ext:Panel ID="Panel3" runat="server" Border="false" Header="false" LabelAlign="Left"
                                                Width="500" Layout="FormLayout">
                                                <Items>
                                                    <ext:Label runat="server" ID="lblNombreRegla" Cls="Titulo" FieldLabel="Nombre de Regla"
                                                        Text="">
                                                    </ext:Label>
                                                </Items>
                                            </ext:Panel>
                                            <ext:Panel ID="Panel4" runat="server" Border="false" Header="false" LabelAlign="Left"
                                                Layout="FormLayout">
                                                <Items>
                                                    <ext:Label runat="server" ID="lblDescripcionRegla" Cls="descripcion" FieldLabel="Descripción"
                                                        Text="">
                                                    </ext:Label>
                                                </Items>
                                            </ext:Panel>
                                            <ext:Panel ID="Panel14" Visible="false" runat="server" Border="false" Header="false"
                                                LabelAlign="Left" Layout="FormLayout">
                                                <Items>
                                                    <ext:Label runat="server" ID="Label1" visible="false" Cls="descripcion" FieldLabel="Regla de Uso de Parametros"
                                                        Html="Del <b>@FechaInicio</b> al <b>@FechaFin</b> se acumulará el <b>@PorcentajeAbono</b> (%) de descuento sobre el monto total de la venta, hasta un máximo de <b>@ImporteMaximoOperacion</b> y <b>@ImporteMaximoDia</b>, Al acumular en la cuenta del tipo <b>@ID_TipoCuenta</b>  más o igual de <b>@MontoMinimoRecarga</b> dólares se disparará la recarga de Tiempo Aire.">
                                                    </ext:Label>
                                                </Items>
                                            </ext:Panel>
                                            <ext:Panel ID="Panel24" runat="server" Border="false" Header="false" LabelAlign="Left"
                                                Layout="FormLayout">
                                                <Items>
                                                    <ext:Label ID="Label2" runat="server" Text="">
                                                    </ext:Label>
                                                </Items>
                                            </ext:Panel>
                                            <ext:Panel ID="Panel5" runat="server" Border="false" Header="false" LabelAlign="Left"
                                                Layout="FormLayout">
                                                <Items>
                                                    <ext:ComboBox FieldLabel="Cadena" ID="cmbCadenaComercial" TabIndex="1" ForceSelection="true"
                                                        EmptyText="Selecciona una Opción..." runat="server" Width="480" StoreID="Store2"
                                                        MsgTarget="Side" AllowBlank="false" DisplayField="NombreORazonSocial" ValueField="ID_Colectiva"
                                                        Editable="false" AnchorHorizontal="90%" Resizable="true" TypeAhead="true" Mode="Local"
                                                        MinChars="1" PageSize="10" ItemSelector="tr.list-item">
                                                        <Template ID="Template1" runat="server">
                                                             <html>
                                                                 <head>
                                                                  <title>Regla001</title>
                                                                </head>
                                                                <tpl for=".">
						                            <tpl if="[xindex] == 1">
							                            <table class="cbStates-list">
                                                            <caption></caption>
								                            <tr>
									                            <th id="nombre">Nombre</th>
								                            </tr>
						                            </tpl>
						                            <tr class="list-item">
							                            <td style="padding:2px 0px;">{NombreORazonSocial} {APaterno} {AMaterno}</td>
						                            </tr>
						                            <tpl if="[xcount-xindex]==0">
							                            </table>
						                            </tpl>
					                            </tpl>
                                                            </Html>
                                                        </Template>
                                                        <Triggers>
                                                            <ext:FieldTrigger Icon="Clear" HideTrigger="true" />
                                                        </Triggers>
                                                        <DirectEvents>
                                                            <Select OnEvent="GridEmpleados_DblClik" Before="var valid= #{FormPanel1}.getForm().isValid(); if (!valid) {} return valid;">
                                                                <ExtraParams>
                                                                    <ext:Parameter Value="12" Name="ID_Regla" Mode="Value">
                                                                    </ext:Parameter>
                                                                </ExtraParams>
                                                            </Select>
                                                        </DirectEvents>
                                                        <Listeners>
                                                            <BeforeQuery Handler="this.triggers[0][ this.getRawValue().toString().length == 0 ? 'hide' : 'show']();" />
                                                            <TriggerClick Handler="if (index == 0) { this.focus().clearValue(); trigger.hide();}" />
                                                            <Select Handler="this.triggers[0].show();" />
                                                        </Listeners>
                                                    </ext:ComboBox>
                                                </Items>
                                            </ext:Panel>
                                        </Items>
                                    </ext:Panel>
                                </Items>
                            </ext:FormPanel>
                        </North>
                        <Center Split="true">
                            <ext:Panel ID="Panel7" runat="server" Title="Parámetros de Regla para la Cadena"
                                Region="West" Split="true" Padding="6" MinWidth="650" Collapsible="false" Layout="FitLayout">
                                <Items>
                                    <%-- Poner las variables automaticas --%>
                                    <ext:PropertyGrid ID="GridPropiedades" runat="server" Header="false">
                                        <Source>
                                            <ext:PropertyGridParameter Name="(Los Parametros)" Value="Los Valores">
                                            </ext:PropertyGridParameter>
                                        </Source>
                                        <DirectEvents>
                                        </DirectEvents>
                                        <View>
                                            <ext:GridView ID="GridView2" ForceFit="true" ScrollOffset="2" runat="server" />
                                        </View>
                                        <FooterBar>
                                            <ext:Toolbar ID="Toolbar2" runat="server" EnableOverflow="true">
                                                <Items>
                                                    <ext:Button ID="btnGuardar" runat="server" Text="Guardar" Icon="Add">
                                                        <DirectEvents>
                                                            <Click OnEvent="Button1_Click" />
                                                        </DirectEvents>
                                                    </ext:Button>
                                                    <ext:Button ID="btnCancelar" runat="server" Text="Cancelar" Icon="Cancel">
                                                        <%-- <DirectEvents>
                                            <Click OnEvent="btnCancelar_Click" />
                                        </DirectEvents>--%>
                                                    </ext:Button>
                                                </Items>
                                            </ext:Toolbar>
                                        </FooterBar>
                                    </ext:PropertyGrid>
                                </Items>
                            </ext:Panel>
                        </Center>
                        <East Collapsible="true">
                            <ext:Panel ID="PnlHeader" runat="server" Title="Parámetros Resultados."  Region="Center"
                                Split="true" Padding="6" Width="500" Collapsible="false" Layout="RowLayout">
                                <Items>
                                    <%-- Poner las variables automaticas --%>
                                    <ext:Label Visible="false" runat="server" ID="Label4" Cls="descripcion" FieldLabel="Regla de Uso de Parametros"
                                        Html="Del <b>@FechaInicio</b> al <b>@FechaFin</b> se acumulará el <b>@PorcentajeAbono</b> (%) de descuento sobre el monto total de la venta, hasta un máximo de <b>@ImporteMaximoOperacion</b> y <b>@ImporteMaximoDia</b>, Al acumular en la cuenta del tipo <b>@ID_TipoCuenta</b>  más o igual de <b>@MontoMinimoRecarga</b> dólares se disparará la recarga de Tiempo Aire.">
                                    </ext:Label>
                                    <ext:Label Visible="false" runat="server" ID="Label3" Cls="descripcion" FieldLabel="Parámetros de Salida"
                                        Html="Los valores que pueden aplicarse por el Script son: <br> <b>@ImporteBonificar</b>: Indica el importe a bonificar de acuerdo a la aplicacion de la regla. <br><b>@RecargaTAE</b>: Indica si se debe enviar la recarga TAE hacia la telefonica,. <br><b>@ImporteRecarga</b>: Es el Importe a recargar como TAE al Telefono enviado en la operación">
                                    </ext:Label>
                                </Items>
                            </ext:Panel>
                        </East>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
