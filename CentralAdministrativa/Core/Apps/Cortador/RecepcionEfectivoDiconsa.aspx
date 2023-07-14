<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RecepcionEfectivoDiconsa.aspx.cs" Inherits="Cortador.RecepcionEfectivoDiconsa" %>


<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Viewport ID="ViewPort1" runat="server">
        <Items>
            <ext:BorderLayout ID="BorderLayout1" runat="server">
                <Center Split="true">
                    <ext:FormPanel ID="FormPanel1" Frame="true" runat="server" Border="false">
                        <Content>
                            <ext:Store ID="StoreRecolector" runat="server">
                                <Reader>
                                    <ext:JsonReader IDProperty="IdRecolector">
                                        <Fields>
                                            <ext:RecordField Name="IdRecolector" />
                                            <ext:RecordField Name="Recolector" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Content>
                        <Items>
                            <ext:FieldSet ID="FielSetDatos" runat="server" Title="Datos de la Recepción" Height="250"
                                DefaultWidth="650" LabelWidth="250" LabelAlign="Right">
                                <Items>
                                    <ext:ComboBox ID="cmbRecolector" FieldLabel="Recolector"
                                        ForceSelection="true" EmptyText="Selecciona una Opción..." runat="server"
                                        StoreID="StoreRecolector" MsgTarget="Side" DisplayField="Recolector"
                                        ValueField="IdRecolector" AllowBlank="false">
                                        <DirectEvents>
                                            <Select OnEvent="LlenaTxtImporte">
                                            </Select>
                                        </DirectEvents>
                                    </ext:ComboBox>
                                    <ext:NumberField ID="nfImporte" FieldLabel="Importe" runat="server" MaxLength="40" MsgTarget="Side"
                                        AllowBlank="false" AllowDecimals="true" AllowNegative="false" />
                                    <ext:TextField ID="txtReferencia" FieldLabel="Referencia" runat="server" MaxLength="40" MaxLengthText="40"
                                        MsgTarget="Side" />
                                    <ext:TextArea ID="txtObservaciones" FieldLabel="Observaciones" runat="server" MaxLength="1000" MaxLengthText="1000"
                                        MsgTarget="Side" AllowBlank="true" Height="90" />
                                </Items>
                                <Buttons>
                                    <ext:Button ID="btnNuevoRegistro" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                        <DirectEvents>
                                            <Click OnEvent="btnNuevoRegistro_Click">
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnRegistrar" runat="server" Text="Registrar" Icon="MoneyAdd">
                                        <DirectEvents>
                                            <Click OnEvent="btnRegistrar_Click" Before="var valid= #{FormPanel1}.getForm().isValid(); if (!valid) {} return valid;">
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                </Buttons>
                            </ext:FieldSet>
                        </Items>
                    </ext:FormPanel>
                </Center>
            </ext:BorderLayout>
        </Items>
    </ext:Viewport>
</asp:Content>
