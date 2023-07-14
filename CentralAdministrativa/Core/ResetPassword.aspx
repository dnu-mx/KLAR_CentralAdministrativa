<%@ Page Title="Iniciar sesión" Language="C#" AutoEventWireup="true" CodeBehind="ResetPassword.aspx.cs"
    Inherits="CentralAplicaciones.ResetPassword" %>

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
        <ext:Window ID="WdwResetPswd" runat="server" Closable="false" Resizable="false" Height="220"
            Icon="LockEdit" Title="Restablece tu Contraseña" Draggable="false" Width="360" Modal="true">
            <Items>
                <ext:FormPanel runat="server" Padding="5" Border="false" LabelWidth="170">
                    <Items>
                        <ext:TextField ID="txtUsername" runat="server" FieldLabel="Usuario" AllowBlank="false"
                            BlankText="Tu nombre de Usuario es Requerido" AnchorHorizontal="100%" />
                        <ext:TextField ID="txtNuevoPassword" runat="server" InputType="Password" AllowBlank="false"
                            FieldLabel="Nueva Contraseña   <span style='color:red;'>*   </span>" AnchorHorizontal="100%"
                            BlankText="Ingresa tu nueva contraseña" EnableKeyEvents="true" />
                        <ext:TextField ID="txtReNuevoPassword" runat="server" InputType="Password" AllowBlank="false"
                            FieldLabel="Repite la Nueva Contraseña   <span style='color:red;'>*   </span>"
                            BlankText="Ingresa tu nueva contraseña" AnchorHorizontal="100%" EnableKeyEvents="true">
                            <DirectEvents>
                                <KeyPress Before="if(e.getKey() == Ext.EventObject.ENTER){ return true;} else{ return false;}" OnEvent="btnSetPswd_Click">
                                    <EventMask ShowMask="true" Msg="Estableciendo Contraseña..." MinDelay="500" />
                                </KeyPress>
                            </DirectEvents>
                        </ext:TextField>
                        <ext:Panel runat="server" Layout="FitLayout" Height="10" Border="false" />
                        <ext:Panel runat="server" Layout="FitLayout" Height="60" Border="false" LabelWidth="1">
                            <Items>
                                <ext:Label ID="lblCondiciones" runat="server" LabelAlign="Right" LabelSeparator=" "
                                    FieldLabel="<span style='color:red;'>*   </span>"
                                    StyleSpec="font-style: italic;font-family:segoe ui;font-size: 11px;" />
                            </Items>
                        </ext:Panel>
                    </Items>
                </ext:FormPanel>
            </Items>
            <Buttons>
                <ext:Button ID="btnSetPswd" runat="server" Text="Aceptar" Icon="Accept">
                    <Listeners>
                        <Click Handler="
                            if (!#{txtUsername}.validate() || !#{txtNuevoPassword}.validate()
                                || !#{txtReNuevoPassword}.validate()) {
                                Ext.Msg.alert('Error','Todos los campos son requeridos');
                                return false; 
                            }
                            " />
                    </Listeners>
                    <DirectEvents>
                        <Click OnEvent="btnSetPswd_Click">
                            <EventMask ShowMask="true" Msg="Estableciendo Contraseña..." MinDelay="500" />
                        </Click>
                    </DirectEvents>
                </ext:Button>
            </Buttons>
        </ext:Window>
        <ext:Window ID="WdwFinal" runat="server" Closable="false" Resizable="false" Height="190" LabelWidth="20"
            Title="Cambio de Contraseña" Draggable="false" Width="350" Layout="FormLayout" Hidden="true">
            <Items>
                <ext:Label runat="server" LabelSeparator=" " Width="150" />
                <ext:Label runat="server" LabelSeparator=" " Width="150" />
                <ext:Label runat="server" LabelSeparator=" " Width="150"
                    StyleSpec="font-weight: bold;font-size: 13px;"
                    Text="La contraseña se cambió correctamente." />
                <ext:Label runat="server" LabelSeparator=" " Width="150" />
                <ext:Label runat="server" LabelSeparator=" " Width="150"
                    StyleSpec="font-weight: bold;font-size: 13px;"
                    Text="Inicia nuevamente sesión para completar el cambio." />
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
