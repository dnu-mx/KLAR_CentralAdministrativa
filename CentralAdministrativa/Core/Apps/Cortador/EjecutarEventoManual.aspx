<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="EjecutarEventoManual.aspx.cs" Inherits="Cortador.EjecutarEventoManual" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
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
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Viewport ID="ViewPort1" runat="server">
        <Items>
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Content>
            <ext:Store ID="stEventosManuales" runat="server">
                <Reader>
                    <ext:ArrayReader>
                        <Fields>
                            <ext:RecordField Name="ID_Evento" />
                            <ext:RecordField Name="Clave" />
                            <ext:RecordField Name="Descripcion" />
                        </Fields>
                    </ext:ArrayReader>
                </Reader>
            </ext:Store>
            <ext:Store ID="stContratos" runat="server" OnRefreshData="refreshData">
                <Reader>
                    <ext:ArrayReader>
                        <Fields>
                            <ext:RecordField Name="ID_Contrato" />
                            <ext:RecordField Name="Clave" />
                            <ext:RecordField Name="Descripcion" />
                            <ext:RecordField Name="CadenaComercial" />
                        </Fields>
                    </ext:ArrayReader>
                </Reader>
            </ext:Store>
            <ext:Store ID="stGridVariables" runat="server" OnRefreshData="refreshData">
                <Reader>
                    <ext:JsonReader IDProperty="ID_ValorContrato">
                        <Fields>
                            <ext:RecordField Name="ID_Contrato" />
                            <ext:RecordField Name="ID_ValorContrato" />
                            <ext:RecordField Name="Nombre" />
                            <ext:RecordField Name="ParameterLabel" />
                            <ext:RecordField Name="Valor" />
                            <ext:RecordField Name="ValueLabel" />
                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>
            <ext:Store ID="stVariablesFaltantes" runat="server" OnRefreshData="refreshData">
                <Reader>
                    <ext:JsonReader IDProperty="ID_ValorContrato">
                        <Fields>
                            <ext:RecordField Name="ID_Contrato" />
                            <ext:RecordField Name="ID_ValorContrato" />
                            <ext:RecordField Name="Nombre" />
                            <ext:RecordField Name="ParameterLabel" />
                            <ext:RecordField Name="Valor" />
                            <ext:RecordField Name="ValueLabel" />
                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>
        </Content>
        <Center Split="true">
            <ext:Panel runat="server" Title="Ejecución de Eventos Manuales">
                <Items>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <North Split="true">
                            <ext:FormPanel ID="FormPanel1" runat="server" Border="false" Height="180" Padding="10">
                                <Items>
                                    <ext:ComboBox FieldLabel="Evento" ID="cmbEventos" TabIndex="1" ForceSelection="true"
                                        EmptyText="Selecciona una Opción..." runat="server" Width="180" StoreID="stEventosManuales"
                                        MsgTarget="Side" AllowBlank="false" DisplayField="Descripcion" ValueField="ID_Evento"
                                        AnchorHorizontal="90%" Resizable="true" Mode="Local" MinChars="1" EnableKeyEvents="true"
                                        PageSize="10" ItemSelector="tr.list-item" >
                                        <Template ID="Template1" runat="server">                                            
                                            <html>
                                                <head runat="server">
                                                  <title>EventoManual</title>
                                                </head>
                                                <tpl for=".">
						                            <tpl if="[xindex] == 1">
							                            <table class="cbStates-list">
                                                            <caption></caption>
								                            <tr>
									                            <th id="clave">Clave</th>
									                            <th id="evento">Evento</th>
								                            </tr>
						                            </tpl>
						                            <tr class="list-item">
							                            <td style="padding:2px 0px;">{Clave}</td>
							                            <td>{Descripcion}</td>
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
                                            <Select OnEvent="Contrato_Select" Before="var valid= #{FormPanel1}.getForm().isValid(); if (!valid) {} return valid;">
                                                <EventMask ShowMask="true" Msg="Obteniendo Contratos..." MinDelay="200" />
                                            </Select>
                                        </DirectEvents>
                                        <Listeners>
                                            <BeforeQuery Handler="this.triggers[0][ this.getRawValue().toString().length == 0 ? 'hide' : 'show']();
                                                var q = queryEvent.query;  queryEvent.query = new RegExp(q, 'i'); queryEvent.query.length = q.length;" />
                                            <TriggerClick Handler="if (index == 0) { this.focus().clearValue(); trigger.hide();}" />
                                            <Select Handler="this.triggers[0].show();" />
                                        </Listeners>
                                    </ext:ComboBox>
                                    <ext:ComboBox FieldLabel="SubEmisor" ID="cmbContratos" TabIndex="2" ForceSelection="true"
                                        EmptyText="Selecciona una Opción..." runat="server" Width="180" StoreID="stContratos"
                                        MsgTarget="Side" AllowBlank="false" DisplayField="Descripcion" ValueField="ID_Contrato"
                                        AnchorHorizontal="90%" Resizable="true" Mode="Local" MinChars="1" EnableKeyEvents="true"
                                        PageSize="10" ItemSelector="tr.list-item"   >
                                        <Template ID="Template2" runat="server">
                                            <Html>
                                                <head>
                                                      <title>EventoManual</title>
                                                    </head>
                                                <tpl for=".">
						                            <tpl if="[xindex] == 1">
							                            <table class="cbStates-list">
                                                            <caption></caption>
								                            <tr>
									                            <th id="cadenaComercial">Cadena Comercial</th>
									                            <th id="descripcion">Descripcion</th>
								                            </tr>
						                            </tpl>
						                            <tr class="list-item">
							                            <td style="padding:2px 0px;">{CadenaComercial}</td>
							                            <td>{Descripcion}</td>
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
                                        <Listeners>
                                            <BeforeQuery Handler="this.triggers[0][ this.getRawValue().toString().length == 0 ? 'hide' : 'show']();
                                                var q = queryEvent.query;  queryEvent.query = new RegExp(q, 'i'); queryEvent.query.length = q.length;" />
                                            <TriggerClick Handler="if (index == 0) { this.focus().clearValue(); trigger.hide();}" />
                                            <Select Handler="this.triggers[0].show();" />
                                        </Listeners>
                                        <DirectEvents>
                                            <Select OnEvent="Contrato_Select" Before="var valid= #{FormPanel1}.getForm().isValid(); if (!valid) {} return valid;">
                                            </Select>
                                        </DirectEvents>
                                    </ext:ComboBox>
                                    <ext:TextField ID="txtRefenreciaNumerica" FieldLabel="Referencia" runat="server" MaxLength="15" MaxLengthText="15"
                                        MsgTarget="Side" AllowBlank="true" AnchorHorizontal="90%">
                                    </ext:TextField>
                                    <ext:TextField ID="TxtObservaciones" FieldLabel="Observaciones" runat="server" MaxLength="1000" MaxLengthText="1000"
                                        MsgTarget="Side" AllowBlank="true" AnchorHorizontal="90%" Height="50" />
                                </Items>
                            </ext:FormPanel>
                        </North>
                        <Center Split="true">
                            <ext:Panel ID="Panel1" runat="server" Title="Parámetros de Evento" Region="West"
                                Split="true" Padding="6" MinWidth="500" Collapsible="false" Layout="FitLayout">
                                <Items>
                                    <%-- Poner las variables automaticas --%>
                                    <ext:PropertyGrid ID="Propiedades" runat="server" Header="false">
                                        <Source>
                                            <ext:PropertyGridParameter Name="(Los Parametros)" Value="Los Valores">
                                            </ext:PropertyGridParameter>
                                        </Source>
                                        <DirectEvents>
                                        </DirectEvents>
                                        <View>
                                            <ext:GridView ID="GridView1" ForceFit="true" ScrollOffset="2" runat="server" />
                                        </View>
                                        <FooterBar>
                                            <ext:Toolbar ID="Toolbar2" runat="server" EnableOverflow="true">
                                                <Items>
                                                    <ext:Button ID="btnEjecutar" runat="server" Text="Ejecutar Evento" Icon="Tick">
                                                        <DirectEvents>
                                                            <Click OnEvent="LanzarEvento_Click" Before="var valid= #{FormPanel1}.getForm().isValid(); if (!valid) {} return valid;">
                                                                <EventMask ShowMask="true" Msg="Procesando..." MinDelay="500" />
                                                            </Click>
                                                        </DirectEvents>
                                                    </ext:Button>
                                                </Items>
                                            </ext:Toolbar>
                                        </FooterBar>
                                    </ext:PropertyGrid>
                                </Items>
                            </ext:Panel>
                        </Center>
                        <East Collapsible="true">
                            <ext:Panel  ID="PnlHeader"  runat="server" Title="Parámetros con Valores de Contrato"
                                Region="Center" Split="true" Padding="6" Width="500" Collapsible="false" Layout="FitLayout">
                                <Items>
                                    <%-- Poner las variables automaticas --%>
                                    <ext:GridPanel Visible="false" ID="GridPanel1" runat="server" StoreID="stGridVariables" StripeRows="true"
                                        RemoveViewState="true" Header="false" Border="false">
                                        <LoadMask ShowMask="false" />
                                        <ColumnModel ID="ColumnModel2" runat="server">
                                            <Columns>
                                                <%-- <ext:Column ColumnID="Nombre" Header="Nombre" Sortable="true" DataIndex="Nombre" />--%>
                                                <ext:Column ColumnID="Vacia" Header="Parámetros" DataIndex="98" Sortable="true" />
                                                <%-- <ext:Column ColumnID="ParameterLabel" Header="Descripción de Paramentro" Sortable="true"
                                            DataIndex="ParameterLabel" />--%>
                                                <%--<ext:Column ColumnID="Afiliacion" Header="Afiliacion" Sortable="true" DataIndex="Afiliacion" />--%>
                                                <%--<ext:Column ColumnID="ValueLabel" Header="Valor" Sortable="true" DataIndex="ValueLabel" />--%>
                                            </Columns>
                                        </ColumnModel>
                                        <View>
                                            <ext:GroupingView ID="GroupingView1" HideGroupedColumn="true" runat="server" ForceFit="true"
                                                GroupTextTpl='{text} ({[values.rs.length]} {[values.rs.length > 1 ? "Items" : "Item"]})'
                                                EnableRowBody="true">
                                                <GetRowClass Handler="var d = record.data; rowParams.body = String.format('<div style=\'padding:0 0px 0px 0px;\'><b>Parámetro:</b> <i>{0}</i>  <br /><b>Descripción de Variable:</b><i>{1}</i>.<br /><b>Valor Sin Formato:</b><i> {2}</i> <br /><b>Etiqueta del Valor:</b><i> {3}</i> </div>', d.Nombre, d.ParameterLabel, d.Valor,d.ValueLabel);" />
                                            </ext:GroupingView>
                                        </View>
                                        <BottomBar>
                                            <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="stGridVariables" DisplayInfo="true"
                                                DisplayMsg="Mostrando Empleados {0} - {1} de {2}" />
                                        </BottomBar>
                                    </ext:GridPanel>
                                </Items>
                            </ext:Panel>
                        </East>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
            </Items>
        </ext:Viewport>
</asp:Content>
