<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DefaultOut.aspx.cs" Inherits="CentralAplicaciones.DefaultOut" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>.: Central Administrativa :. </title>
    <link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .style1 {
            font-size: xx-large;
        }
    </style>

</head>
<body>
    <div class="Uno">
        <form id="Form2" runat="server">

            <div class="header">
                <div class="title">
                    <asp:ScriptManager ID="ScriptMan" runat="server">
                    </asp:ScriptManager>
                </div>
                <div class="loginDisplay">
                    [ <a href="~/Account/Login.aspx" id="HeadLoginStatus" runat="server">Iniciar Sesión</a> ]
                </div>
            </div>
            <div class="main">
                <%-- <div id="div_noticias" class="div_noticias">
                <img src="images/Noticias.png" width="95%" height="95%" alt="Publicidad" />
            </div>
            <div>
                <div id="div_Mapa" class="div_mapa">
                                  </div>
                <div id="div_paginas" class="div_paginas">
                   
                </div>
            </div>
            <div id="div_Promo" class="div_publicidad">
                
            </div>--%>
                <br />
                <br />
                <table align="left">
                    <tr align="left">
                        <td align="left">
                            <span class="style1"><strong>Central Administrativa</strong></span><br />
                        </td>
                    </tr>
                </table>
                <table align="right">
                    <tr align="right">
                        <td align="right">
                            <asp:Image ID="Image1" ImageUrl="images/logo.png" runat="server" Height="250px" Width="250px" ImageAlign="Right" />
                        </td>
                    </tr>
                </table>
            </div>
        </form>
    </div>
</body>
</html>
