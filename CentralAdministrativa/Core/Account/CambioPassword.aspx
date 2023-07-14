<%@ Page Title="Iniciar sesión" Language="C#" AutoEventWireup="true" CodeBehind="CambioPassword.aspx.cs"
    Inherits="CentralAplicaciones.Account.CambioPassword" %>

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
        <ext:Window ID="WdwCambioPassword" runat="server" Closable="false" Resizable="false" Height="220"
            Icon="UserKey" Title="Cambio de Contraseña" Draggable="false" Width="360" Modal="true" >
            <Items>
                <ext:FormPanel runat="server" Padding="5" Border="false" LabelWidth="170">
                    <Items>
                        <ext:TextField ID="txtPasswordActual" runat="server" InputType="Password" FieldLabel="Contraseña Actual"
                            AllowBlank="false" BlankText="Ingresa tu contraseña actual" AnchorHorizontal="100%"/>
                        <ext:TextField ID="txtNuevoPassword" runat="server" InputType="Password" AllowBlank="false"
                            FieldLabel="Nueva Contraseña   <span style='color:red;'>*   </span>" AnchorHorizontal="100%"
                            BlankText="Ingresa tu nueva contraseña" EnableKeyEvents="true" />
                        <ext:TextField ID="txtReNuevoPassword" runat="server" InputType="Password" AllowBlank="false"
                            FieldLabel="Repite la Nueva Contraseña   <span style='color:red;'>*   </span>"
                            BlankText="Ingresa tu nueva contraseña" AnchorHorizontal="100%" EnableKeyEvents="true">
                            <DirectEvents>
                                <KeyPress Before="if(e.getKey() == Ext.EventObject.ENTER){ return true;} else{ return false;}" OnEvent="btnCambiarPswd_Click">
                                    <EventMask ShowMask="true" Msg="Cambiando Contraseña..." MinDelay="500" />
                                </KeyPress>
                            </DirectEvents>
                        </ext:TextField>
                        <ext:Panel runat="server" Layout="FitLayout" Height="10" Border="false" />
                        <ext:Panel runat="server" Layout="FitLayout" Height="60" Border="false" LabelWidth="1">
                            <Items>
                                <ext:Label ID="lblCondiciones" runat="server" LabelAlign="Right" LabelSeparator=" "
                                    FieldLabel="<span style='color:red;'>*   </span>"
                                    StyleSpec="font-style: italic;font-family:segoe ui;font-size: 11px;"/>
                            </Items>
                        </ext:Panel>
                    </Items>
                </ext:FormPanel>
            </Items>
            <Buttons>
                <ext:Button ID="btnCambiarPswd" runat="server" Text="Cambiar" Icon="Accept">
                    <Listeners>
                        <Click Handler="
                            if (!#{txtPasswordActual}.validate() || !#{txtNuevoPassword}.validate()
                                || !#{txtReNuevoPassword}.validate()) 
                            {
                                Ext.Msg.alert('Error','Todos los campos son requeridos');
                                return false; 
                            }
                            " />
                    </Listeners>
                    <DirectEvents>
                        <Click OnEvent="btnCambiarPswd_Click">
                            <EventMask ShowMask="true" Msg="Cambiando Contraseña..." MinDelay="500" />
                        </Click>
                    </DirectEvents>
                </ext:Button>
                <ext:Button ID="btnCancel" runat="server" Text="Cancelar" Icon="Decline">
                    <Listeners>
                        <Click Handler="#{WdwCambioPassword}.hide();" />
                    </Listeners>
                    <DirectEvents>
                        <Click OnEvent="btnCancel_Click" />
                    </DirectEvents>
                </ext:Button>
            </Buttons>
        </ext:Window>
        <ext:Window ID="WdwReinicia" runat="server" Closable="false" Resizable="false" Height="190" LabelWidth="20"
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
                    Text="Reinicia tu sesión para completar el cambio." />
            </Items>
            <Buttons>
                <ext:Button ID="btnOkReinicia" runat="server" Text="Aceptar" Icon="Accept">
                    <DirectEvents>
                        <Click OnEvent="btnOkReinicia_Click" />
                    </DirectEvents>
                </ext:Button>
            </Buttons>
        </ext:Window>
    </form>
</body>
</html>
