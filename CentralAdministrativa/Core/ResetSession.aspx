<%@ Page Title="Iniciar sesión" Language="C#" AutoEventWireup="true" CodeBehind="ResetSession.aspx.cs"
    Inherits="CentralAplicaciones.ResetSession" %>

<%@ Import Namespace="Panel=Ext.Net.Panel" %>
<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>.: Central Administrativa :. </title>
    <link href="../Styles/Site.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <ext:ResourceManager ID="ResourceManager1" runat="server" />
    <form runat="server">
        <ext:Window ID="WdwResetSession" runat="server" Closable="false" Resizable="false" Height="160"
            Icon="LockEdit" Title="Restablece tu Sesión" Draggable="false" Width="300" Modal="true">
            <Items>
                <ext:FormPanel runat="server" Padding="5" Border="false" LabelWidth="65">
                    <Items>
                        <ext:TextField ID="txtUsername" runat="server" FieldLabel="Usuario" AllowBlank="false"
                            BlankText="Tu nombre de Usuario es Requerido" AnchorHorizontal="100%" />
                        <ext:TextField ID="txtToken" runat="server" AllowBlank="false" BlankText="Ingresa el token"
                            FieldLabel="Código    <span style='color:red;'>*   </span>" AnchorHorizontal="100%"
                            EnableKeyEvents="true" MaskRe="[0-9]" MinLength="8" MaxLength="8">
                            <DirectEvents>
                                <KeyPress Before="if(e.getKey() == Ext.EventObject.ENTER){ return true;} else{ return false;}" 
                                    OnEvent="btnSetSession_Click">
                                    <EventMask ShowMask="true" Msg="Restableciendo Sesión..." MinDelay="500" />
                                </KeyPress>
                            </DirectEvents>
                        </ext:TextField>
                        <ext:Panel runat="server" Layout="FitLayout" Height="10" Border="false" />
                        <ext:Panel runat="server" Layout="FitLayout" Height="60" Border="false" LabelWidth="1">
                            <Items>
                                <ext:Label ID="lblInstrucciones" runat="server" LabelAlign="Right" LabelSeparator=" "
                                    Text=" Ingresa el código que recibiste por correo electrónico"
                                    FieldLabel="<span style='color:red;'>*   </span>"
                                    StyleSpec="font-style: italic;font-family:segoe ui;font-size: 11px;" />
                            </Items>
                        </ext:Panel>
                    </Items>
                </ext:FormPanel>
            </Items>
            <Buttons>
                <ext:Button ID="btnSetSession" runat="server" Text="Aceptar" Icon="Accept">
                    <Listeners>
                        <Click Handler="if (!#{txtToken}.isValid()) { return false; }" />
                    </Listeners>
                    <DirectEvents>
                        <Click OnEvent="btnSetSession_Click">
                            <EventMask ShowMask="true" Msg="Restableciendo Sesión..." MinDelay="500" />
                        </Click>
                    </DirectEvents>
                </ext:Button>
            </Buttons>
        </ext:Window>
        <ext:Window ID="WdwFinal" runat="server" Closable="false" Resizable="false" Height="190" LabelWidth="20"
            Title="Sesión Restablecida" Draggable="false" Width="350" Layout="FormLayout" Hidden="true">
            <Items>
                <ext:Label runat="server" LabelSeparator=" " Width="150" />
                <ext:Label runat="server" LabelSeparator=" " Width="150" />
                <ext:Label runat="server" LabelSeparator=" " Width="150"
                    StyleSpec="font-weight: bold;font-size: 13px;"
                    Text="Tu sesión se restableció correctamente." />
                <ext:Label runat="server" LabelSeparator=" " Width="150" />
                <ext:Label runat="server" LabelSeparator=" " Width="150"
                    StyleSpec="font-weight: bold;font-size: 13px;"
                    Text="Inicia una nueva sesión para finalizar." />
            </Items>
            <Buttons>
                <ext:Button ID="btnOkFinal" runat="server" Text="Aceptar" Icon="Accept">
                    <DirectEvents>
                        <Click OnEvent="btnOkFinal_Click" />
                    </DirectEvents>
                </ext:Button>
            </Buttons>
        </ext:Window>
    </form>
</body>
</html>
