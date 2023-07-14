<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="BloqueoUsuarios.aspx.cs" Inherits="SitioInterno.BloqueoUsuarios" %>

<asp:content id="Content1" ContentPlaceHolderID="MainContent" runat="server">    
        <div class="pagina">
            <asp:label id="LabelError" runat="server" Visible="False" ForeColor="Red"></asp:label>
            <h2>Bloqueo de Usuarios</h2>
        <table align=center>
            <caption></caption>
            <tr valign="top">
                <th scope="col" width="100%">
                    <table  align=center>
                        <caption></caption>
                    <tr>
                        <th scope="col">Nombre(s):</th>
                        <th scope="col">Apellido(s):</th>
                        <th scope="col">Email:</th>
                    </tr>
                    <tr>
                        <td ><asp:TextBox ID="txtNombre" Width="250px" runat="server"></asp:TextBox></td>
                        <td><asp:TextBox ID="txtApellido" Width="250px" runat="server"></asp:TextBox></td>
                        <td><asp:TextBox ID="txtEmail" Width="250px" runat="server"></asp:TextBox></td>
                     </tr>
                    <tr>
                        <td ></td>
                        <td ></td>
                        <td align=right>
                            <asp:Button ID="btnFiltrar" runat="server" Text="Buscar" onclick="btnFiltrar_Click" />
                        </td>
                    </tr>
                    </table>
                     <table  align=center>
                        <caption></caption>
                    <tr><td></td></tr>

                    <tr>
                        <td> </td>
                        <asp:Repeater ID="RepeaterUsuarios" runat="server">
                            <HeaderTemplate>
                                <%--<td class="tdTitulo" width="80%">Email usuario</td>--%>
                                <table align=center>
                                    <caption></caption>
                                    <tr>
                                    <th scope="col" class="tdTitulo" width="80px">Estatus</th>
                                    <th scope="col" class="tdTitulo" width="230px">Nombre(s)</th>
                                    <th scope="col" class="tdTitulo" width="230px">Apellido(s)</th>
                                    <th scope="col" class="tdTitulo" width="230px">Email</th>
                                    </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr class="trAlterno">
                                <td align="center">
										<asp:ImageButton ID="EditItem" ImageUrl='<%# (int)DataBinder.Eval(Container.DataItem,"Estatus") == 1? "images/activo.png": "images/inactivo.png" %>' OnCommand="SetStatus" Runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Mail1") %>' AlternateText="">
										</asp:ImageButton>
                                    </td>
                                    <td><%# DataBinder.Eval(Container.DataItem, "Nombre1") %></td>
                                    <td><%# DataBinder.Eval(Container.DataItem, "Apellido1")%></td>
                                    <td><%# DataBinder.Eval(Container.DataItem, "Mail1")%></td>
                                    <%--<td><asp:CheckBox ID="chkHabilitado" runat="server" 
                                        Checked='<%# (int)DataBinder.Eval(Container.DataItem,"Estatus") == 1? true: false %>'
                                        Text='<%# DataBinder.Eval(Container.DataItem, "Mail1") %>' AutoPostBack="true"
                                        OnCheckedChanged="SetStatus" /></td>--%>
                                </tr>
                            </ItemTemplate>
                            <AlternatingItemTemplate>
							 <tr>
                              <td align="center">
										<asp:ImageButton ID="EditItem" ImageUrl='<%# (int)DataBinder.Eval(Container.DataItem,"Estatus") == 1? "images/activo.png": "images/inactivo.png" %>' OnCommand="SetStatus" Runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Mail1") %>' AlternateText="">
										</asp:ImageButton>
                                    </td>
                                    <td><%# DataBinder.Eval(Container.DataItem, "Nombre1") %></td>
                                    <td><%# DataBinder.Eval(Container.DataItem, "Apellido1")%></td>
                                    <td><%# DataBinder.Eval(Container.DataItem, "Mail1")%></td>
                                </tr>
						    </AlternatingItemTemplate>
                            <FooterTemplate>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </tr>
                   </table>
                </th>
            </tr>
              <tr>
                     <td>
                        <table width="100%">
                        <tr>
                            <td align="left" width= "50%">
                                <asp:ImageButton ID="back" Runat="server" ImageUrl= "images/back.png" OnCommand="lbtnPrev_Click"></asp:ImageButton> 
                            </td>
                            <td align="right" width= "50%">
                                <asp:ImageButton  ID="next" Runat="server" ImageUrl= "images/Delante.png" OnCommand="lbtnNext_Click"></asp:ImageButton> 
                            </td>
                        </tr>
                        </table>
                    </td>
                     </tr> 
        </table>
        
    </div>
    
</asp:content>
<asp:Content ID="Content2" runat="server" contentplaceholderid="HeadContent">
    <style type="text/css">
        .style1
        {
            width: 278px;
        }
        .style2
        {
            width: 199px;
        }
    </style>
</asp:Content>
