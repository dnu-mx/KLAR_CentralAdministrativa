<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ErrorInicializarPagina.aspx.cs" Inherits="CentralAplicaciones.ErrorInicializarPagina" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title> .: Central Administrativa :. </title>
     <link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .style1
        {
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
            <span class="style1"><strong>Ocurrió un Error</strong></span><br />
            <table align="right">
                <tr align="right">
                    <td align="right">
                        <asp:Image ID="Image1" ImageUrl="images/Error.png" runat="server" 
                            Height="196px" Width="250px" />
                    </td>
                </tr>
            </table>
            <br />
            <br />
            <br />
            <br />
            <br />
            La Página a la que Solicitaste, Genero un error al ser Cargada, por favor contacta al Administrador</div>
        </form>
    </div>
    <div class="footer">
           
    
    </div>
</body>
</html>
