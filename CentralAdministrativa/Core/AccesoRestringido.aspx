<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AccesoRestringido.aspx.cs" Inherits="CentralAplicaciones.AccesoRestringido" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en">
<head runat="server">
    <title>.: Central Administrativa :. </title>
    <link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .style1 {
            font-size: xx-large;
        }
    </style>
</head>
<body>
<div class="Uno" >
    <form id="Form2" runat="server">
    
        <div class="Denegado">
            <div class="title">
                <asp:ScriptManager ID="ScriptMan" runat="server">
                </asp:ScriptManager>
            </div>
            <div class="Denegado">

            </div>
        </div>
        <div class="main">
            <br />
            <br />
            <span class="style1"><strong>Acceso Denegado</strong></span><br />
            <table align="right">
                <caption> </caption>
                <tr>
                    <th scope="col"> </th>
                </tr>
                <tr align="right">
                    <td align="right">
                        <asp:Image ID="Image1" ImageUrl="images/Stop.png" runat="server" Height="250px" Width="250px" />
                    </td>
                </tr>
            </table>
            <br />
            <br />
            <br />
            <br />
            <br />
            La página a la que solicitaste, esta restringida para tu nivel de usuario, Si 
            crees que deberías tener acceso, por favor, contacta al Administrador del 
            Sistema.</div>
        </form>
    </div>
    <div class="footer">
           
    
    </div>
</body>
</html>
