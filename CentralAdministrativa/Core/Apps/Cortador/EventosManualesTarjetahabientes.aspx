<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="EventosManualesTarjetahabientes.aspx.cs" Inherits="Cortador.EventosManualesTarjetahabientes" %>


<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Viewport ID="ViewPort1" runat="server">
        <Items>
            <ext:BorderLayout ID="BorderLayout1" runat="server">
                <Center Split="true">
                    <ext:FormPanel ID="FormPanelEvManTH" runat="server" Border="false" Padding="10" LabelWidth="120">
                        <Items>
                            <ext:FieldSet ID="FieldSetEvManTH" runat="server" Title="Ejecución de Eventos Manuales para Tarjetahabientes" Layout="AnchorLayout"
                                DefaultAnchor="100%" Padding="20">
                                <Items>
                                    <ext:Hidden ID="hdnSaldoCLDC" runat="server" />
                                    <ext:Hidden ID="hdnClaveSubemisor" runat="server" />
                                    <ext:ComboBox ID="cBoxEvento" runat="server" FieldLabel="Evento <span style='color:red;'>*   </span>" Editable="false"
                                        DisplayField="Descripcion" EmptyText="Selecciona el Evento..." ValueField="ID_Evento" AllowBlank="false">
                                        <Store>
                                            <ext:Store ID="StoreEvManTH" runat="server">
                                                <Reader>
                                                    <ext:JsonReader IDProperty="ID_Evento">
                                                        <Fields>
                                                            <ext:RecordField Name="ID_Evento" />
                                                            <ext:RecordField Name="ClaveEvento" />
                                                            <ext:RecordField Name="Descripcion" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Store>
                                        <Triggers>
                                            <ext:FieldTrigger Icon="Clear" Qtip="Borrar" />
                                        </Triggers>
                                        <Listeners>
                                            <TriggerClick Handler="this.removeByValue(this.getValue());this.clearValue();" />
                                            <Select Handler="#{txtTarjeta}.reset(); #{txtImporte}.reset(); #{txtReferencia}.reset();
                                                #{txtObservaciones}.reset();" />
                                        </Listeners>
                                    </ext:ComboBox>
                                    <ext:TextField ID="txtTarjeta" runat="server" FieldLabel="Número de Tarjeta <span style='color:red;'>*   </span>" 
                                        AllowBlank="false" MaskRe="[0-9\.]" MaxLength="16" MinLength="16">
                                        <Listeners>
                                            <Change Handler="#{hdnClaveSubEm}.reset(); #{hdnSaldoCLDC}.clear();" />
                                        </Listeners>
                                    </ext:TextField>
                                    <ext:TextField ID="txtImporte" runat="server" MaskRe="[0-9\.]" FieldLabel="Importe <span style='color:red;'>*   </span>"
                                        AllowBlank="false">
                                        <Listeners>
                                            <Change Handler="var imp = Ext.util.Format.number(this.getValue(), '$0,0.00');
                                                this.setValue(imp);" />
                                        </Listeners>
                                    </ext:TextField>
                                    <ext:TextField ID="txtReferencia" FieldLabel="Referencia" runat="server" MaxLength="40" MaxLengthText="40"
                                        MaskRe="[0-9]" />
                                    <ext:TextArea ID="txtObservaciones" runat="server" FieldLabel="Observaciones <span style='color:red;'>*   </span>"
                                        MaxLength="500" MaxLengthText="500" AllowBlank="false" />
                                </Items>
                                <Buttons>
                                    <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                        <DirectEvents>
                                            <Click OnEvent="btnLimpiar_Click" />
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button runat="server" ID="btnEjecutaEvManTH" Text="Ejecutar Evento" Icon="Tick">
                                        <DirectEvents>
                                            <Click OnEvent="btnEjecutaEvManTH_Click" Before="var valid= #{FormPanelEvManTH}.getForm().isValid(); if (!valid) {} return valid;">
                                                <EventMask ShowMask="true" Msg="Procesando..." MinDelay="500" />
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
