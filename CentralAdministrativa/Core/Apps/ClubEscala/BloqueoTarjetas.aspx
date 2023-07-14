<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="BloqueoTarjetas.aspx.cs" Inherits="SitioInterno.BloqueoTarjetas" %>


<asp:content id="Content1" ContentPlaceHolderID="MainContent" runat="server"> 

    <div class="pagina">
    <asp:label id="LabelError" runat="server" Visible="False" ForeColor="Red"></asp:label>
    <h2>Bloqueo de medios de pago</h2>
        <table align=center>
            <caption></caption>
               <tr valign="top">
                    <th width="100%">
                    <table width="100%">
                        <caption></caption>
                    <tr>
                        <th width="20px">
                        <table align=center>
                            <caption></caption>
                            <tr>
                                <th></th>
                                <th scope="col">Nombre Titular:</th>
                                <th scope="col">Medio de Pago:</th>
                                <th scope="col">Tipo Medio Pago:</th>
                            </tr>
                             <tr>
                                <td></td>
                                <td><asp:TextBox ID="txtitular" Width="250px" runat="server"></asp:TextBox></td>
                                <td><asp:TextBox ID="txtMedioPago" Width="250px" runat="server"></asp:TextBox></td>
                                <td><asp:dropdownlist id="cardType" Width="250px" runat="server">
										<asp:ListItem Value="0">Todas las Disponibles</asp:ListItem>
										<asp:ListItem Value="VISA">Visa</asp:ListItem>
										<asp:ListItem Value="MASTER">Mastercard</asp:ListItem>
                                        <asp:ListItem Value="MONEDERO">Monedero Electrónico</asp:ListItem>
                                        <asp:ListItem Value="CREDITO">Crédito Electrónico</asp:ListItem>
									</asp:dropdownlist></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td align="right"> <asp:Button ID="btnFiltrar" runat="server" Text="Buscar" 
                                onclick="btnFiltrar_Click" /></td>
                            </tr>
                        </table>
                        </th>
                     </tr>
                    </table>
                    </th>
                    </tr>
                    <tr><td></td></tr>
               <tr>
                        <td width="20px">
                        <asp:Repeater ID="RepeaterTarjetas" runat="server" 
                            onitemcommand="RepeaterTarjetas_ItemCommand">
                            <HeaderTemplate>
                                <table align=center>
                                    <caption></caption>
                                    <tr>
                                        <th scope="col" class="tdTitulo" width="80px">Estatus</th>
                                        <th scope="col" class="tdTitulo" width="330px">Titular de la Cuenta</th>
                                        <th scope="col" class="tdTitulo" width="420px">Institución</th>
                                        <th scope="col" class="tdTitulo" width="420px">Medio de Pago</th>
                                    </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr class="trAlterno">
                                    <td align="center">
										<asp:ImageButton ID="EditItem" ImageUrl='<%# (Boolean)DataBinder.Eval(Container.DataItem,"BloqueoAdmin") == true? "images/Bloqueado.png": "images/NoBloqueado.png" %>' OnCommand="SetStatus" Runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "id") %>' AlternateText="">
										</asp:ImageButton>
                                    </td>
                                    <td><%# DataBinder.Eval(Container.DataItem, "Titular")%></td>
                                    <td><%# DataBinder.Eval(Container.DataItem, "Compania")%></td>
                                    <td><%# DataBinder.Eval(Container.DataItem, "Numero")%></td>
                                    
                                    <%--<td><asp:CheckBox ID="chkHabilitado"  runat="server"
                                        Checked='<%# (int)DataBinder.Eval(Container.DataItem,"Estado") == 1? true: false %>' 
                                        Text='<%# DataBinder.Eval(Container.DataItem, "Numero") %>' AutoPostBack="true" 
                                         /></td>--%>
                                </tr>
                            </ItemTemplate>
                            <AlternatingItemTemplate>
							<tr>
                                    <td align="center">
									    <asp:ImageButton ID="EditItem" ImageUrl='<%# (Boolean)DataBinder.Eval(Container.DataItem,"BloqueoAdmin") == true? "images/Bloqueado.png": "images/NoBloqueado.png" %>' OnCommand="SetStatus" Runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "id") %>' AlternateText="">
									    </asp:ImageButton>
                                    </td>
								 <td><%# DataBinder.Eval(Container.DataItem, "Titular")%></td>
                                 <td><%# DataBinder.Eval(Container.DataItem, "Compania")%></td>
								 <td><%# DataBinder.Eval(Container.DataItem, "Numero")%></td>
                                   
                                    <%--<td><asp:CheckBox ID="chkHabilitado"  runat="server"
                                        Checked='<%# (int)DataBinder.Eval(Container.DataItem,"Estado") == 1? true: false %>' 
                                        Text='<%# DataBinder.Eval(Container.DataItem, "Numero") %>' AutoPostBack="true" 
                                         /></td>--%>
							</tr>
						</AlternatingItemTemplate>
                            <FooterTemplate>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                        </td>
                    </tr>
              <tr>
                     <td>
                        <table width= "100%">
                            <caption></caption>
                        <tr>
                            <%--<td width="20px">&nbsp;</td>--%>
                            <th scope="col" align="left" width= "50%">
                                <asp:ImageButton ID="back" Runat="server" ImageUrl= "images/back.png" OnCommand="lbtnPrev_Click"></asp:ImageButton> 
                            </th>
                            <th scope="col" align="right" width= "50%">
                                <asp:ImageButton  ID="next" Runat="server" ImageUrl= "images/Delante.png" OnCommand="lbtnNext_Click"></asp:ImageButton> 
                            </th>
                        </tr>
                        </table>
                    </td>
                     </tr> 
        </table>
        
        </div>
</asp:content>
