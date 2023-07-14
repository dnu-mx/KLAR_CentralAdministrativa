<%@ Page Title="Iniciar sesión" Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs"
    Inherits="CentralAplicaciones.Account.Login" %>

<%@ Import Namespace="Panel=Ext.Net.Panel" %>
<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title> .: Central Administrativa :. </title>
    <link href="../Styles/Site.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <ext:ResourceManager ID="ResourceManager1" runat="server" />
    <form runat="server">
        <ext:Window ID="WdwLogin" runat="server" Closable="false" Resizable="false" Height="150"
            Icon="Lock" Title="Inicio de Sesión" Draggable="false" Width="350" Modal="true">
            <Items>
                <ext:Hidden ID="hdnPswdCounter" runat="server" />
                <ext:FormPanel runat="server" Padding="5" Border="false" Layout="FormLayout" LabelWidth="80">
                    <Items>
                        <ext:TextField ID="txtUsername" runat="server" FieldLabel="Usuario" AllowBlank="false"
                            BlankText="Tu nombre de Usuario es Requerido" AnchorHorizontal="100%" />
                        <ext:TextField ID="txtPassword" runat="server" InputType="Password" FieldLabel="Contraseña"
                            AllowBlank="false" BlankText="Tu Contraseña es requerida."
                            AnchorHorizontal="100%" EnableKeyEvents="true">
                            <DirectEvents>
                                <KeyPress OnEvent="btnLogin_Click" Timeout="360000"
                                    Before="if(e.getKey() == Ext.EventObject.ENTER) { 
                                        if (!#{txtUsername}.validate() || !#{txtPassword}.validate()) {
                                            Ext.Msg.alert('Error','El Nombre de Usuario y Contraseña son requeridos');
                                            return false; }
                                        return true; } 
                                        else { return false; }">
                                    <EventMask ShowMask="true" Msg="Verificando Identidad..." MinDelay="500" />
                                </KeyPress>
                            </DirectEvents>
                        </ext:TextField>
                        <ext:Panel runat="server" Layout="FitLayout" Height="10" Border="false" />
                        <ext:Panel runat="server" Layout="FitLayout" Height="60" Border="false" LabelWidth="1">
                            <Items>
                                <ext:LinkButton runat="server" Text="¿Olvidaste tu contraseña?">
                                    <Listeners>
                                        <Click Handler="#{WdwLogin}.hide(); #{WdwResetPswd}.show();" />
                                    </Listeners>
                                </ext:LinkButton>
                            </Items>
                        </ext:Panel>
                    </Items>
                </ext:FormPanel>
            </Items>
            <Buttons>
                <ext:Button ID="btnLogin" runat="server" Text="Login" Icon="Accept">
                    <DirectEvents>
                        <Click OnEvent="btnLogin_Click" Timeout="360000"
                            Before="if (!#{txtUsername}.validate() || !#{txtPassword}.validate()) {
                            Ext.Msg.alert('Error','El Nombre de Usuario y Contraseña son requeridos');
                            return false; }">
                            <EventMask ShowMask="true" Msg="Verificando Identidad..." MinDelay="500" />
                        </Click>
                    </DirectEvents>
                </ext:Button>
                <ext:Button ID="btnCancel" runat="server" Text="Cancel" Icon="Decline">
                    <Listeners>
                        <Click Handler="#{WdwLogin}.hide();" />
                    </Listeners>
                    <DirectEvents>
                        <Click OnEvent="btnCancel_Click">
                            <EventMask ShowMask="true" Msg="Cancelando..." MinDelay="500" />
                        </Click>
                    </DirectEvents>
                </ext:Button>
                <ext:Button ID="btnSessionMail" runat="server" Hidden="true">
                    <Listeners>
                        <Click Handler="#{WdwLogin}.hide(); #{WdwResetSession}.show();" />
                    </Listeners>
                </ext:Button>
            </Buttons>
        </ext:Window>
        <ext:Window ID="WdwResetPswd" runat="server" Resizable="false" Height="190" Title="Restablecer contraseña"
            Icon="Lock" Draggable="false" Width="350" Modal="true" Hidden="true" Closable="false">
            <Items>
                <ext:FormPanel runat="server" Padding="5" Border="false" Layout="FormLayout" LabelWidth="120">
                    <Items>
                        <ext:TextField ID="txtUserReset" runat="server" FieldLabel="Usuario    <span style='color:red;'>*   </span>"
                            AllowBlank="false" BlankText="Tu Nombre de Usuario es requerido" AnchorHorizontal="100%" />
                        <ext:TextField ID="txtEmail" runat="server" AllowBlank="false" AnchorHorizontal="100%"
                            FieldLabel="Correo Electrónico   <span style='color:red;'>*   </span>" Vtype="email"
                            EnableKeyEvents="true">
                            <DirectEvents>
                                <KeyPress OnEvent="btnReset_Click" Timeout="360000"
                                    Before="if(e.getKey() == Ext.EventObject.ENTER) { 
                                        if (!#{txtEmail}.validate()) {
                                            Ext.Msg.alert('Error','El Correo Electrónico es requerido');
                                            return false; }
                                        return true; } 
                                        else { return false; }">
                                    <EventMask ShowMask="true" Msg="Restableciendo Contraseña..." MinDelay="500" />
                                </KeyPress>
                            </DirectEvents>
                        </ext:TextField>
                        <ext:Panel runat="server" Layout="FitLayout" Height="2" Border="false" />
                        <ext:Panel runat="server" Layout="FitLayout" Height="20" Border="false" LabelWidth="1">
                            <Items>
                                <ext:Label runat="server" LabelSeparator=" " LabelWidth="3" FieldLabel="<span style='color:red;'>* </span>"
                                    Text="Obligatorios. Recuerda que distingue mayúsculas y minúsculas."
                                    StyleSpec="font-style: italic;font-family:segoe ui;font-size: 11px;" />
                            </Items>
                        </ext:Panel>
                        <ext:Panel runat="server" Layout="FitLayout" Height="10" Border="false" />
                        <ext:Panel runat="server" Layout="FitLayout" Height="60" Border="false" LabelWidth="1">
                            <Items>
                                <ext:Label runat="server" LabelSeparator=" " LabelWidth="1" 
                                    Text="Recibirás por correo electrónico las indicaciones para restablecer tu contraseña."
                                    StyleSpec="font-style: italic;font-family:segoe ui;font-size: 12px;" />                                
                            </Items>
                        </ext:Panel>
                    </Items>
                </ext:FormPanel>
            </Items>
            <Buttons>
                <ext:Button ID="btnReset" runat="server" Text="Aceptar" Icon="Accept">
                    <DirectEvents>
                        <Click OnEvent="btnReset_Click" Timeout="360000"
                            Before="if (!#{txtEmail}.validate()) { return false; }">
                            <EventMask ShowMask="true" Msg="Restableciendo Contraseña..." MinDelay="500" />
                        </Click>
                    </DirectEvents>
                </ext:Button>
                <ext:Button runat="server" Text="Cancelar" Icon="Decline">
                    <Listeners>
                        <Click Handler="#{WdwResetPswd}.hide();" />
                    </Listeners>
                    <DirectEvents>
                        <Click OnEvent="btnCancel_Click">
                            <EventMask ShowMask="true" Msg="Cancelando..." MinDelay="500" />
                        </Click>
                    </DirectEvents>
                </ext:Button>
            </Buttons>
        </ext:Window>
        <ext:Window ID="WdwResetSession" runat="server" Resizable="false" Height="140" Title="Restablecer sesión"
            Icon="Lock" Draggable="false" Width="350" Modal="true" Hidden="true" Closable="false">
            <Items>
                <ext:FormPanel runat="server" Padding="5" Border="false" Layout="FormLayout" LabelWidth="120">
                    <Items>
                        <ext:TextField ID="txtSessionEmail" runat="server" AllowBlank="false" AnchorHorizontal="100%"
                            FieldLabel="Correo Electrónico   <span style='color:red;'>*   </span>" Vtype="email"
                            EnableKeyEvents="true">
                            <DirectEvents>
                                <KeyPress OnEvent="btnRestableceSesion_Click" Timeout="360000"
                                    Before="if(e.getKey() == Ext.EventObject.ENTER) { 
                                        if (!#{txtSessionEmail}.validate()) {
                                            Ext.Msg.alert('Error','El Correo Electrónico es requerido');
                                            return false; }
                                        return true; } 
                                        else { return false; }">
                                    <EventMask ShowMask="true" Msg="Restableciendo Sesión..." MinDelay="500" />
                                </KeyPress>
                            </DirectEvents>
                        </ext:TextField>
                        <ext:Panel runat="server" Layout="FitLayout" Height="2" Border="false" />
                        <ext:Panel runat="server" Layout="FitLayout" Height="75" Border="false" LabelWidth="1">
                            <Items>
                                <ext:Label runat="server" LabelSeparator=" " LabelWidth="1" FieldLabel="<span style='color:red;'>*   </span>"
                                    Text="Por favor, ingresa el correo electrónico asociado a tu usuario para recibir el código."
                                    StyleSpec="font-style: italic;font-family:segoe ui;font-size: 11px;" />
                            </Items>
                        </ext:Panel>
                    </Items>
                </ext:FormPanel>
            </Items>
            <Buttons>
                <ext:Button ID="btnRestableceSesion" runat="server" Text="Aceptar" Icon="Accept">
                    <DirectEvents>
                        <Click OnEvent="btnRestableceSesion_Click" Timeout="360000"
                            Before="if (!#{txtSessionEmail}.validate()) { return false; }">
                            <EventMask ShowMask="true" Msg="Restableciendo Sesión..." MinDelay="500" />
                        </Click>
                    </DirectEvents>
                </ext:Button>
                <ext:Button ID="btnCancelaRstSesn" runat="server" Hidden="true">
                    <DirectEvents>
                        <Click OnEvent="btnCancel_Click">
                            <EventMask ShowMask="true" Msg="Cancelando..." MinDelay="500" />
                        </Click>
                    </DirectEvents>
                </ext:Button>
                <ext:Button runat="server" Text="Cancelar" Icon="Decline">
                    <Listeners>
                        <Click Handler="#{WdwResetSession}.hide();" />
                    </Listeners>
                    <DirectEvents>
                        <Click OnEvent="btnCancel_Click">
                            <EventMask ShowMask="true" Msg="Cancelando..." MinDelay="500" />
                        </Click>
                    </DirectEvents>
                </ext:Button>
            </Buttons>
        </ext:Window>
    </form>
</body>
</html>
