<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Principal.aspx.cs" Inherits="CentralAplicaciones.Principal" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<%@ Import Namespace="System.Web" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>.: Central Administrativa :. </title>
    <link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .x-grid3-cell-inner
        {
            font-family: "segoe ui" ,tahoma, arial, sans-serif;
        }
        
        .x-grid-group-hd div
        {
            font-family: "segoe ui" ,tahoma, arial, sans-serif;
        }
        
        .x-grid3-hd-inner
        {
            font-family: "segoe ui" ,tahoma, arial, sans-serif;
            font-size: 12px;
        }
        
        .x-grid3-body .x-grid3-td-Cost
        {
            background-color: #f1f2f4;
        }
        
        .x-grid3-summary-row .x-grid3-td-Cost
        {
            background-color: #e1e2e4;
        }
        
        .total-field
        {
            background-color: #fff;
            font-weight: bold !important;
            color: #000;
            border: solid 1px silver;
            padding: 2px;
            margin-right: 5px;
        }
    </style>
    <script type="text/javascript">
        function Counter(options) {
            var seconds;
            var timer;
            var instance = this;
            var onUpdateStatus = options.onUpdateStatus || function () { };
            var onCounterEnd = options.onCounterEnd || function () { };

            function decrementCounter() {
                onUpdateStatus(seconds);
                if (seconds === 0) {
                    stopCounter();
                    onCounterEnd();
                    return;
                }
                if (seconds <= 30) {
                    frmTimeOut.setVisible(true);
                    lblText.setVisible(true);
                }
                seconds--;
            };

            function startCounter() {
                seconds = (document.getElementById('txtMinTimeOut').value * 60);
                clearInterval(timer);
                timer = 0;
                decrementCounter();
                timer = setInterval(decrementCounter, 1000);
            };

            function stopCounter() {
                frmTimeOut.setVisible(false);
                lblText.setVisible(false);
                countdown.start();
            };

            return {
                start: function () {
                    startCounter();
                },
                stop: function () {
                    stopCounter();
                }
            }
        };

        var countdown = new Counter({
            // callback function for each second
            onUpdateStatus: function (second) {
                Ext.getCmp('lblText').setText(second);
            },

            // callback function for final action after countdown
            onCounterEnd: function () {
                top.location.replace("LogOut.aspx");
            }
        });

        var ocultarVentana = function () {
            countdown.stop();
        };

        // Stop counter on Load 
        document.onload = function () {
            countdown.stop();
        };
        // Stop counter when mouse move
        document.onmousemove = function () {
            countdown.stop();
        };
        // Stop counter when key is pressed
        document.onkeypress = function () {
            countdown.stop();
        };

        //// Session Timeout client side. Master and Detail detection. 
        ///** 
        //* Detecting in Master and Detail, Mouse and Keyboard events. 
        //* Using postMessage (HTML5) 
        //* @autor Camilo Martinez [â‚¬quiman], http://gplus.to/equiman 
        //* @created 2012-04-11 
        //* @updated 2012-05-02 
        //* @link http://www.inaltec.com.co/ 
        //* @licence CC BY-SA http://creativecommons.org/licenses/by-sa/3.0/ 
        //*/
        //// URL Redirection 
        //var timeOutSession = function () {
        //    top.location.replace("LogOut.aspx");
        //};


        //var ocultarVentana = function () {

        //    frmTimeOut.setVisible(false);
        //    stopCount();
        //};

        //// Add this into Master Page (TabPanelContain): 
        //// <DocumentReady Handler="masterTimeOut();" />
        //var masterTimeOut = function () {
        //    //Convert minutes value indicated in TimeOut del Web.Config to seconds 
        //    var min = 0;
        //    var sec = 0;
        //    var timer = null;
        //    var vis = false;
        //    doTimer = function () {
        //        if ((min - sec) < 60) {
        //            updateTime();
        //            if (frmTimeOut.hidden) {
        //                vis = true;
        //                frmTimeOut.setVisible(true);
        //                lblText.setVisible(true);
        //            }
        //        }
        //        else {
        //            if (!frmTimeOut.hidden) {
        //                vis = false;

        //            }
        //        }

        //        // Check the countdown counter 
        //        if ((min - sec) > 0) {
        //            sec++;
        //            // Repeat the process each minute 
        //            timer = setTimeout('doTimer()', 1000);
        //        }
        //        else {
        //            // When countdown is finish, redirect to Login page 
        //            timeOutSession();
        //        }
        //    };
        //    stopCount = function () {

        //        if (!frmTimeOut.hidden) {
        //            return;
        //        }

        //        if (vis === true) {
        //            //2 seconds Idle (no detect events, because showing alert window is detected as mousemove) 
        //            if ((min - sec) < 58) {
        //                vis = false;
        //            }
        //        }
        //        else {
        //            // Convert minutes to second, minus 10 to be sure Client TimeOut occurs first than Server Timeout 
        //            min = (document.getElementById('txtMinTimeOut').value * 60);
        //            sec = 0;
        //            clearTimeout(timer);
        //            doTimer();
        //        }
        //    };
        //    updateTime = function () {
        //        var message = '';
        //        var time = (min - sec);
        //        var unity = '';
        //        Ext.getCmp('lblText').setText(message + ' ' + time + ' ' + unity);


        //    };
        //    // Start counter on Load 
        //    document.onload = function () {
        //        stopCount();
        //        //return false; 
        //    };
        //    // Star counter when mouse move 
        //    document.onmousemove = function () {
        //        stopCount();
        //        //return false; 
        //    };
        //    // Star counter when key is pressed 
        //    document.onkeypress = function () {
        //        stopCount();
        //        //return false; 
        //    };
        //    // Read and event when is send from an iFrame 
        //    function displayMessage(e) {
        //        if ((e.origin.split(":", 2)[0] + ":" + e.origin.split(":", 2)[1] + "/") === (GetNewPath("/").split(":", 2)[0] + ":" + GetNewPath("/").split(":", 2)[1])) {
        //            // If the iFrame send any of this events start counter 
        //            switch (e.data) {
        //                case "onload":
        //                case "onmousemove":
        //                case "onkeypress":
        //                case "simulated":
        //                    stopCount();
        //            }
        //        }
        //    };
        //    if (window.addEventListener) {
        //        // For standards-compliant web browsers 
        //        window.addEventListener("message", displayMessage, false);
        //    }
        //    else {
        //        window.attachEvent("onmessage", displayMessage);
        //    };
        //};
        //// Add this into any iFrame Page: 
        //// <DocumentReady Handler="detailTimeOut();" /> 
        //var detailTimeOut = function () {
        //    // Start counter on Load 
        //    document.onload = function () {
        //        top.postMessage("onload", GetNewPath("/"));
        //        return false;
        //    };
        //    // Star counter when mouse move 
        //    document.onmousemove = function () {
        //        top.postMessage("onmousemove", GetNewPath("/"));
        //        //return false; 
        //    };
        //    // Star counter when key is pressed 
        //    document.onkeypress = function () {
        //        top.postMessage("onkeypress", GetNewPath("/"));
        //        //return false; 
        //    };
        //    // Simulated Event 
        //    simulatedEvent = function () {
        //        top.postMessage("simulated", GetNewPath("/"));
        //        //return false; 
        //    };
        //};
        //// Get the Path when use VirtualPath in .Net or IIS 
        //var GetNewPath = function (relative_path) {
        //    var url = window.location.href;
        //    if (url.substring(url.length - 1, url.length) == '/') {
        //        url = url.substring(0, url.length - 1);
        //    }
        //    var url_parts = url.split('/');
        //    if (relative_path.substring(0, 1) != '/') {
        //        url_parts[url_parts.length - 2] = relative_path;
        //        url_parts[url_parts.length - 1] = '';
        //    }
        //    else {
        //        url_parts[url_parts.length - 2] = relative_path.substring(1);
        //        url_parts[url_parts.length - 1] = '';
        //    }
        //    var new_page_absolute_path = url_parts.join('/');
        //    if (new_page_absolute_path.substring(new_page_absolute_path.length - 1, new_page_absolute_path.length) == '/') {
        //        new_page_absolute_path = new_page_absolute_path.substring(0, new_page_absolute_path.length - 1);
        //    }
        //    return new_page_absolute_path;
        //};

        //********************************
        var loadPage = function (tabPanel, node) {
            var tab = tabPanel.getItem(node.id);
            if (!tab) {
                tab = tabPanel.add({
                    id: node.id,
                    title: node.text,
                    closable: true,
                    border: true,

                    autoLoad: {
                        showMask: true,
                        url: node.attributes.href,
                        mode: "iframe",
                        split: true,
                        AutoScroll: true,
                        width: 200,
                        padding: 6,
                        //maskMsg: "Loading " + node.attributes.href + "..."
                        maskMsg: "Abriendo " + node.text + " ..."
                    },
                    listeners: {
                        update: {
                            fn: function (tab, cfg) {
                                cfg.iframe.setHeight(cfg.iframe.getSize().height);
                            },

                            scope: this,
                            single: true
                        }
                    }
                });
            }
            // pageLoad();
            tabPanel.setActiveTab(tab);
        }
    </script>
</head>
<body>
    <form id="Form1" runat="server">
    <ext:ResourceManager ID="ResourceManagerMaster2" runat="server">
        <Listeners>
            <DocumentReady Handler="countdown.start();" />
        </Listeners>
    </ext:ResourceManager>
    <ext:Hidden ID="txtMinTimeOut" runat="server" />
    <ext:Window ID="frmTimeOut" runat="server" Width="360" Height="340" Resizable="False"
        Hidden="true" Closable="false" Modal="true" Layout="FitLayout" Draggable="false"
        Padding="12">
        <Items>
            <ext:Panel ID="Panel2" runat="server" Title="" Region="North" Split="true" Padding="6"
                Collapsible="false">
                <Content>
                    <div>
                        <table>
                            <tr>
                                <td align="center">
                                    <asp:Label ID="Label1" runat="server" Text="Inactividad Detectada" Font-Size="Large" />
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <p>
                                        La sesión ha pasado por un período de inactividad.</p>
                                    <p>
                                        Para tu tranquilidad y por protección a los datos, se inicia el conteo regresivo
                                        para cerrar tu sesión automáticamente.
                                    </p>
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    Tu sesión terminará en
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <h2>
                                        <ext:Label ID="lblText" runat="server" HideLabel="true" Text="" AutoWidth="true" />
                                    </h2>
                                    segundos.
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <br />
                                    <p>
                                        Para detener el cierre automático,<br />
                                        presiona el botón
                                    </p>
                                    <ext:Button ID="btnCrearCta" runat="server" Flat="false" Text="Continuar Trabajando"
                                        ToolTip="No cerrar la Sesión" Icon="Accept" Height="30">
                                        <Listeners>
                                            <Click Handler="ocultarVentana();"></Click>
                                        </Listeners>
                                    </ext:Button>
                                </td>
                            </tr>
                        </table>
                    </div>
                </Content>
            </ext:Panel>
        </Items>
    </ext:Window>
    <ext:Viewport ID="Viewport1" runat="server" Layout="border">
        <Items>
            <ext:BorderLayout ID="BorderLayout1" runat="server">
                <North>
                    <ext:Panel ID="PnlHeader" runat="server" Title="" Region="North" Split="true" Height="120"
                        Padding="6" Collapsible="true">
                        <Content>
                            <div class="div_1">
                                <table width="100%">
                                    <tr>
                                        <td>
                                            <img src="images/logo.png" width="50px" height="51px" />
                                        </td>
                                        <td>
                                            <asp:Label ID="lblHeader"  CssClass="lblHead" runat="server" Text="Central Administrativa" Width="50"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div id="Div1" class="div_2" runat="server">
                                <table id="Table1" width="100%" runat="server">
                                    <tr>
                                        <td>
                                            Usuario:
                                        </td>
                                        <td align="left">
                                            <span class="bold">
                                                <asp:LoginName ID="LoginName1" runat="server" />
                                            </span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Email:
                                        </td>
                                        <td align="left">
                                            <span class="bold">
                                                <asp:Label ID="lblEmail" runat="server"></asp:Label>
                                            </span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Roles:
                                        </td>
                                        <td align="left">
                                            <span class="bold">
                                                <asp:Label ID="lblPerfiles" runat="server"></asp:Label>
                                            </span>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div id="Div2" class="div_3" runat="server">
                                <table id="Table2" width="100%" runat="server">
                                   <tr align="right">
                                       <td></td>
                                       <td>
                                           <span class="bold">
                                               <asp:LoginStatus ID="LoginStatus1" runat="server" LogoutAction="Redirect" LogoutText="Cerrar sesión"
                                                   LogoutPageUrl="Logout.aspx" />
                                           </span>
                                       </td>
                                   </tr>
                                    <tr align="right">
                                        <td></td>
                                        <td>
                                            <span class="bold">
                                                <a href="~/Account/CambioPassword.aspx" id="A1" runat="server">Cambiar contraseña</a>
                                            </span>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <%--  </LoggedInTemplate>
                                </asp:LoginView>--%>
                        </Content>
                    </ext:Panel>
                </North>
                <West>
                    <ext:Panel ID="PnlMenu" runat="server" Title="Menú general" Region="West" Layout="accordion"
                        Width="225" MinWidth="225" MaxWidth="400" Split="true" Collapsible="true">
                    </ext:Panel>
                </West>
                <Center>
                    <ext:TabPanel ID="Pages" runat="server" Border="false" EnableTabScroll="true" AutoScroll="false"   >
                        <Items>
                            <ext:Panel ID="Panel1" runat="server" Title="Inicio" Split="true" Collapsible="false">
                            </ext:Panel>
                        </Items>
                    </ext:TabPanel>
                </Center>
               <%-- <East>
                    <ext:Panel ID="Info" Frame="true" Width="198" runat="server" Split="true" Collapsible="true"
                        EnableTabScroll="true">
                        <Items>
                            <ext:DatePicker ID="DatePicker1" runat="server" Cls="ext-cal-nav-picker">
                            </ext:DatePicker>
                        </Items>
                    </ext:Panel>
                </East>--%>
            </ext:BorderLayout>
        </Items>
    </ext:Viewport>
    </form>
</body>
</html>
