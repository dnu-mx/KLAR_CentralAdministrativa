<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="BloqueoTelefonos.aspx.cs" Inherits="SitioInterno.BloqueoTelefonos" %>
<asp:content id="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="pagina">
    <asp:label id="LabelError" runat="server" Visible="False" ForeColor="Red"></asp:label>
    <h2>Bloqueo de tel&eacute;fonos</h2>
        <table align=center>
            <caption></caption>
            <tr valign="top">
                <th scope="col" width="100%">
                    <table align=center width="100%">
                        <caption></caption>
                    <tr><td></td></tr>
                    <tr>
                        <td width="20px">
                        <table>
                            <caption></caption>
                            <tr>
                                <th scope="col">Correo Electrónico</th>
                                <th scope="col">Número Telefónico</th>
                            </tr>
                            <tr>
                                <td><asp:TextBox ID="txtEmail"  Width="350px" runat="server"></asp:TextBox> &nbsp;</td>
                                <td><asp:TextBox ID="TxtTelefono"  Width="350px"  runat="server"></asp:TextBox> &nbsp;</td>
                            </tr>
                            <tr>
                                <td></td>
                                <td align=right>
                                    <asp:Button ID="btnFiltrar" runat="server" Text="Buscar" onclick="btnFiltrar_Click" />
                                </td>
                            </tr>
                        </table>

                     </tr>
                    <tr><td></td></tr>
                    <tr>
                        <td width="20px">
                        <asp:Repeater ID="RepeaterTelefonos" runat="server">
                            <HeaderTemplate>
                                <table align=center >
                                    <caption></caption>
                                    <tr>
                                        <th scope="col" class="tdTitulo" width="25px">Estatus</th>
                                        <th scope="col" class="tdTitulo" width="230px">Email Usuario</th>
                                        <th scope="col" class="tdTitulo" width="220px">Tel&eacute;fono</th>
                                    </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr class="trAlterno">
                                 <td align="center">
										<asp:ImageButton ID="EditItem" ImageUrl='<%# (int)DataBinder.Eval(Container.DataItem,"Estado") == 1? "images/activo.png": "images/inactivo.png" %>' OnCommand="SetStatus" Runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Numero") %>'  AlternateText="">
										</asp:ImageButton>
                                    </td>
                                    <td><%# DataBinder.Eval(Container.DataItem, "IdPropietario")%></td>
                                    <td><%# DataBinder.Eval(Container.DataItem, "Numero")%></td>
                                <%--    <td><asp:CheckBox ID="chkHabilitado" runat="server" 
                                        Checked='<%# (int)DataBinder.Eval(Container.DataItem,"Estado") == 1? true: false %>'
                                        Text='<%# DataBinder.Eval(Container.DataItem, "Numero") %>' AutoPostBack="true"
                                        OnCheckedChanged="SetStatus" /></td>--%>
                                </tr>
                            </ItemTemplate>
                            <AlternatingItemTemplate>
							<tr>
								    <tr>
                                     <td align="center">
										<asp:ImageButton ID="EditItem" ImageUrl='<%# (int)DataBinder.Eval(Container.DataItem,"Estado") == 1? "images/activo.png": "images/inactivo.png" %>' OnCommand="SetStatus" Runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Numero") %>' AlternateText="">
										</asp:ImageButton>
                                    </td>
                                    <td><%# DataBinder.Eval(Container.DataItem, "IdPropietario")%></td>
                                    <td><%# DataBinder.Eval(Container.DataItem, "Numero")%></td>
                              <%--      <td><asp:CheckBox ID="chkHabilitado" runat="server" 
                                        Checked='<%# (int)DataBinder.Eval(Container.DataItem,"Estado") == 1? true: false %>'  AutoPostBack="true"
                                        Text='<%# DataBinder.Eval(Container.DataItem, "Numero") %>'
                                        OnCheckedChanged="SetStatus" /></td>--%>
                                </tr>
							</tr>
						</AlternatingItemTemplate>
                            <FooterTemplate>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                        </td>
                    </tr>
                    </table>
                </th>
            </tr>
              <tr>
                     <td>
                        <table width="100%">
                            <caption></caption>
                        <tr>
                            <th scope="col" align="left" width= "50%">
                                <asp:ImageButton ID="back" Runat="server" ImageUrl= "images/back.png" OnCommand="lbtnPrev_Click"></asp:ImageButton> 
                            </th>
                            <th  scope="col" align="right" width= "50%">
                                <asp:ImageButton  ID="next" Runat="server" ImageUrl= "images/Delante.png" OnCommand="lbtnNext_Click"></asp:ImageButton> 
                            </th>
                        </tr>
                        </table>
                    </td>
                     </tr> 
        </table>
        
        </div>
</asp:content>