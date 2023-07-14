<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AjustarSaldoCupon.aspx.cs" Inherits="Cortador.AjustarSaldoCupon" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Viewport ID="ViewPort1" runat="server">
        <Items>
            <ext:BorderLayout ID="BorderLayout1" runat="server">
                <Center Split="true">
                    <ext:FormPanel ID="FormPanelBusqueda" Frame="true" runat="server" Border="false">
                        <Content>
                            <ext:Store ID="StorePromocion" runat="server">
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_Evento">
                                        <Fields>
                                            <ext:RecordField Name="ID_Evento" />
                                            <ext:RecordField Name="Descripcion" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                            <ext:Store ID="StoreDetallePromocion" runat="server">
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_GrupoMA">
                                        <Fields>
                                            <ext:RecordField Name="ID_GrupoMA" />
                                            <ext:RecordField Name="Descripcion" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Content>
                        <Items>
                            <ext:FieldSet ID="FielSetDatos" runat="server" Title="Búsqueda de Cupón" Height="155"
                                DefaultWidth="650" LabelWidth="250" LabelAlign="Right" >
                                <Items>
                                    <ext:ComboBox ID="cmbPromocion" runat="server" FieldLabel="Promoción"
                                        ForceSelection="true" EmptyText="Selecciona una Promoción..."
                                        StoreID="StorePromocion" MsgTarget="Side" DisplayField="Descripcion"
                                        ValueField="ID_Evento" AllowBlank="false">
                                        <DirectEvents>
                                            <Select OnEvent="LlenaDetallePromocion">
                                            </Select>
                                        </DirectEvents>
                                    </ext:ComboBox>
                                    <ext:ComboBox ID="cmbDetallePromocion" runat="server" FieldLabel="Detalle Promoción"
                                        ForceSelection="true" EmptyText="Selecciona el Detalle de la Promoción..."
                                        StoreID="StoreDetallePromocion" MsgTarget="Side" DisplayField="Descripcion"
                                        ValueField="ID_GrupoMA" AllowBlank="false" />
                                    <ext:TextField ID="txtCupon" FieldLabel="Código del Cupón" runat="server" MaxLength="40"
                                        MaxLengthText="40" MsgTarget="Side" AllowBlank="false"/>
                                </Items>
                                <Buttons>
                                    <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                        <DirectEvents>
                                            <Click OnEvent="btnLimpiar_Click" />
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Find">
                                        <DirectEvents>
                                            <Click OnEvent="btnBuscar_Click" Before="var valid= #{FormPanelBusqueda}.getForm().isValid(); if (!valid) {} return valid;">
                                                <EventMask ShowMask="true" Msg="Buscando Promoción..." MinDelay="500" />
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                </Buttons>
                            </ext:FieldSet>
                        </Items>
                    </ext:FormPanel>
                </Center>
                <South Split="true">
                    <ext:FormPanel ID="FormPanelResultados" runat="server" Collapsible="true" Height="320">
                        <Items>
                            <ext:FieldSet ID="FieldSetResultados" runat="server" Title="Ajustar Saldo" Height="250"
                                DefaultWidth="650" LabelWidth="250" LabelAlign="Right">
                                <Items>
                                    <ext:Hidden ID="hidIdColectiva" runat="server" />
                                    <ext:Hidden ID="hidIdTipoColectiva" runat="server" />
                                    <ext:Hidden ID="hidClaveCadena" runat="server" />
                                    <ext:TextField ID="txtSaldoActual" runat="server" FieldLabel="Saldo Actual" ReadOnly="true" />
                                    <ext:NumberField ID="nfSaldoNuevo" FieldLabel="Nuevo Saldo" runat="server" MaxLength="40" MsgTarget="Side"
                                        AllowBlank="false" AllowDecimals="false" AllowNegative="false" />
                                    <ext:TextArea ID="txtObservaciones" FieldLabel="Observaciones" runat="server" MaxLength="1000" MaxLengthText="1000"
                                        MsgTarget="Side" AllowBlank="false" Height="120" />
                                </Items>
                                <Buttons>
                                    <ext:Button ID="btnAceptar" runat="server" Text="Aceptar" Icon="Accept" >
                                        <DirectEvents>
                                            <Click OnEvent="btnAceptar_Click" Before="var valid= #{FormPanelResultados}.getForm().isValid(); if (!valid) {} return valid;">
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnCancelar" runat="server" Text="Cancelar" Icon="Cancel">
                                        <DirectEvents>
                                            <Click OnEvent="btnCancelar_Click" />
                                        </DirectEvents>
                                    </ext:Button>
                                </Buttons>
                            </ext:FieldSet>
                        </Items>
                    </ext:FormPanel>
                </South>
            </ext:BorderLayout>
        </Items>
    </ext:Viewport>
</asp:Content>
