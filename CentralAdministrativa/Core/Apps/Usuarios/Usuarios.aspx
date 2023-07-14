<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Usuarios.aspx.cs" Inherits="Usuarios.Usuarios" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center Split="true">
            <ext:Panel ID="Panel2" runat="server"  Width="428.5" Title="Usuarios de Mi Grupo Comercial"
                Collapsed="false" Layout="Fit" AutoScroll="true">
                <Content>
                    <ext:Store ID="Store1" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID">
                                <Fields>
                                    <ext:RecordField Name="ID" />
                                    <ext:RecordField Name="Name" />
                                    <ext:RecordField Name="Start" Type="Date" />
                                    <ext:RecordField Name="End" Type="Date" />
                                    <ext:RecordField Name="Completed" Type="Boolean" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store1" StripeRows="true"
                                Header="false" Border="false" AutoExpandColumn="Name">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                        <ext:Column Header="ID" Width="40" Sortable="true" DataIndex="ID" />
                                        <ext:Column ColumnID="Name" Header="Job Name" Sortable="true" DataIndex="Name" />
                                        <ext:DateColumn ColumnID="Start" Header="Start" Width="120" Sortable="true" DataIndex="Start"
                                            Format="yyyy-MM-dd" />
                                        <ext:DateColumn ColumnID="End" Header="End" Width="120" Sortable="true" DataIndex="End"
                                            Format="yyyy-MM-dd" />
                                        <ext:Column ColumnID="Completed" Header="Completed" Width="80" Sortable="true" DataIndex="Completed">
                                            <Renderer Handler="return (value) ? 'Yes':'No';" />
                                        </ext:Column>
                                        <ext:CommandColumn Width="60">
                                            <Commands>
                                                <ext:GridCommand Icon="Delete" CommandName="Delete">
                                                    <ToolTip Text="Delete" />
                                                </ext:GridCommand>
                                                <ext:CommandSeparator />
                                                <ext:GridCommand Icon="NoteEdit" CommandName="Edit">
                                                    <ToolTip Text="Edit" />
                                                </ext:GridCommand>
                                            </Commands>
                                        </ext:CommandColumn>
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                                        <DirectEvents>
                                            <RowSelect OnEvent="RowSelect" Buffer="100">
                                                <EventMask ShowMask="true" Target="CustomTarget" CustomTarget="#{FormPanel1}" />
                                                <ExtraParams>
                                                    <%-- or can use params[2].id as value --%>
                                                    <ext:Parameter Name="AfiliciacionID" Value="this.getSelected().id" Mode="Raw" />
                                                </ExtraParams>
                                            </RowSelect>
                                        </DirectEvents>
                                    </ext:RowSelectionModel>
                                </SelectionModel>
                                <Plugins>
                                    <ext:GridFilters runat="server" ID="GridFilters1" Local="true">
                                        <Filters>
                                            <ext:NumericFilter DataIndex="ID" />
                                            <ext:StringFilter DataIndex="Name" />
                                            <ext:DateFilter DataIndex="Start">
                                                <DatePickerOptions runat="server" TodayText="Now" />
                                            </ext:DateFilter>
                                            <ext:DateFilter DataIndex="End">
                                                <DatePickerOptions runat="server" TodayText="Now" />
                                            </ext:DateFilter>
                                            <ext:BooleanFilter DataIndex="Completed" />
                                        </Filters>
                                    </ext:GridFilters>
                                </Plugins>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="Store1" DisplayInfo="true"
                                        DisplayMsg="Mostrando Afiliaciones {0} - {1} de {2}" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </Center>
        <East Split="true">
            <ext:Panel ID="Panel5" runat="server"  Width="428.5" Title="Agregar un nuevo Usuario"
                Collapsed="false" Layout="Fit" AutoScroll="true" Collapsible="true">
                <Items>
                    <ext:TabPanel runat="server" ActiveTabIndex="0" TabPosition="Bottom" Border="false"
                        AutoScroll="true" ID="TabPanel1" Title="ctl71">
                        <Items>
                            <ext:Panel ID="Panel6" runat="server" Title="Nuevo Usuario" Border="false" AutoScroll="true"
                                Padding="6">
                                <FooterBar>
                                    <ext:Toolbar ID="Toolbar1" runat="server" EnableOverflow="true">
                                        <Items>
                                            <ext:Button ID="Button1" runat="server" Text="Guardar" Icon="Add" />
                                            <ext:Button ID="Button3" runat="server" Text="Cancelar" Icon="Cancel" />
                                        </Items>
                                    </ext:Toolbar>
                                </FooterBar>
                                <Content>
                                    <ext:Panel ID="Panel1" runat="server" Title="Datos de Sesion" AutoHeight="true" FormGroup="true">
                                        <Content>
                                            <table>
                                                <tr>
                                                    <td colspan="2">
                                                        <p>
                                                            <span class="x-label-text">Usuario:</span><span style="color: red">*</span></p>
                                                        <ext:TextField ID="TextField5" runat="server" Text="" Width="130" />
                                                    </td>
                                                    <td colspan="2">
                                                        <p>
                                                            <span class="x-label-text">Password:</span><span style="color: red">*</span></p>
                                                        <ext:TextField ID="TextField6" runat="server" Width="130" Text="" />
                                                    </td>
                                                    <td colspan="2">
                                                        <p>
                                                            <span class="x-label-text">Repite Password:</span><span style="color: red">*</span></p>
                                                        <ext:TextField ID="TextField7" runat="server" Text="" Width="130" />
                                                    </td>
                                                    <td colspan="2">
                                                    </td>
                                                </tr>
                                            </table>
                                        </Content>
                                    </ext:Panel>
                                    <ext:Panel ID="Panel7" runat="server" Title="Datos Generales" AutoHeight="true" FormGroup="true">
                                        <Content>
                                            <table>
                                                <tr>
                                                    <td colspan="2">
                                                        <p>
                                                            <span class="x-label-text">Clave Grupo:</span><span style="color: red">*</span></p>
                                                        <ext:TextField ID="TextField10" runat="server" Text="" Width="130" />
                                                    </td>
                                                    <td colspan="2">
                                                        <p>
                                                            <span class="x-label-text">Nombre:</span><span style="color: red">*</span></p>
                                                        <ext:TextField ID="TextField11" runat="server" Width="130" Text="" />
                                                    </td>
                                                    <td colspan="2">
                                                        <p>
                                                            <span class="x-label-text">RFC:</span><span style="color: red">*</span></p>
                                                        <ext:TextField ID="TextField12" runat="server" Text="" Width="130" />
                                                    </td>
                                                    <td colspan="2">
                                                        <p>
                                                            <span class="x-label-text">CURP:</span><span style="color: red">*</span></p>
                                                        <ext:TextField ID="TextField13" runat="server" Width="130" Text="" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2">
                                                        <p>
                                                            <span class="x-label-text">Telefono Movil:</span><span style="color: red">*</span></p>
                                                        <ext:TextField ID="TextField14" runat="server" Text="" Width="130" />
                                                    </td>
                                                    <td colspan="2">
                                                        <p>
                                                            <span class="x-label-text">Telefono Fijo:</span><span style="color: red">*</span></p>
                                                        <ext:TextField ID="TextField15" runat="server" Width="130" Text="" />
                                                    </td>
                                                    <td colspan="2">
                                                        <p>
                                                            <span class="x-label-text">email:</span><span style="color: red">*</span></p>
                                                        <ext:TextField ID="TextField16" runat="server" Text="" Width="130" />
                                                    </td>
                                                    <td colspan="2">
                                                        <p>
                                                            <span class="x-label-text">Telefono Fijo:</span><span style="color: red">*</span></p>
                                                        <ext:TextField ID="TextField17" runat="server" Width="130" Text="" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </Content>
                                    </ext:Panel>
                                    <ext:Panel ID="Panel8" runat="server" Title="Direccion Ubicación" AutoHeight="true"
                                        FormGroup="true">
                                        <Content>
                                            <table border="0">
                                                <tr>
                                                    <td colspan="2">
                                                        <p>
                                                            <span class="x-label-text">Estado:</span><span style="color: red">*</span></p>
                                                        <ext:ComboBox ID="ComboBox8" runat="server" Width="130">
                                                            <Items>
                                                                <ext:ListItem Text="Morelos" />
                                                                <ext:ListItem Text="Distrito Federal" />
                                                                <ext:ListItem Text="Puebla" />
                                                                <ext:ListItem Text="Morelia" />
                                                            </Items>
                                                        </ext:ComboBox>
                                                    </td>
                                                    <td colspan="2">
                                                        <p>
                                                            <span class="x-label-text">Municipio:</span><span style="color: red">*</span></p>
                                                        <ext:ComboBox ID="ComboBox9" runat="server" Width="130">
                                                            <Items>
                                                                <ext:ListItem Text="Morelos" />
                                                                <ext:ListItem Text="Distrito Federal" />
                                                                <ext:ListItem Text="Puebla" />
                                                                <ext:ListItem Text="Morelia" />
                                                            </Items>
                                                        </ext:ComboBox>
                                                    </td>
                                                    <td colspan="2">
                                                        <p>
                                                            <span class="x-label-text">Ciudad:</span><span style="color: red">*</span></p>
                                                        <ext:ComboBox ID="ComboBox10" runat="server" Width="130">
                                                            <Items>
                                                                <ext:ListItem Text="Morelos" />
                                                                <ext:ListItem Text="Distrito Federal" />
                                                                <ext:ListItem Text="Puebla" />
                                                                <ext:ListItem Text="Morelia" />
                                                            </Items>
                                                        </ext:ComboBox>
                                                    </td>
                                                    <td colspan="2">
                                                        <p>
                                                            <span class="x-label-text">Asentamiento:</span><span style="color: red">*</span></p>
                                                        <ext:ComboBox ID="ComboBox11" runat="server" Width="130">
                                                            <Items>
                                                                <ext:ListItem Text="Morelos" />
                                                                <ext:ListItem Text="Distrito Federal" />
                                                                <ext:ListItem Text="Puebla" />
                                                                <ext:ListItem Text="Morelia" />
                                                            </Items>
                                                        </ext:ComboBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2">
                                                        <p>
                                                            <span class="x-label-text">Codigo Postal:</span><span style="color: red">*</span></p>
                                                        <ext:TextField ID="TextField18" runat="server" Width="130" Text="" />
                                                    </td>
                                                    <td colspan="2">
                                                        <p>
                                                            <span class="x-label-text">Calle:</span><span style="color: red">*</span></p>
                                                        <ext:TextField ID="TextField19" runat="server" Width="130" Text="" />
                                                    </td>
                                                    <td colspan="2">
                                                        <p>
                                                            <span class="x-label-text">Num. Int:</span><span style="color: red">*</span></p>
                                                        <ext:TextField ID="TextField20" runat="server" Width="130" Text="" />
                                                    </td>
                                                    <td colspan="2">
                                                        <p>
                                                            <span class="x-label-text">Num. Ext.:</span><span style="color: red">*</span></p>
                                                        <ext:TextField ID="TextField21" runat="server" Width="130" Text="" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="4">
                                                        <p>
                                                            <span class="x-label-text">Entre Calles:</span><span style="color: red">*</span></p>
                                                        <%-- <ext:TextField ID="TextField5" runat="server" Width="260" Text="" />--%>
                                                        <ext:TextArea ID="TextArea6" runat="server" Width="260" Height="50" />
                                                    </td>
                                                    <td colspan="4">
                                                        <p>
                                                            <span class="x-label-text">Referencias:</span><span style="color: red">*</span></p>
                                                        <%--<ext:TextField ID="TextField6" runat="server" Width="260" Text="" />--%>
                                                        <ext:TextArea ID="TextArea7" runat="server" Width="260" Height="50" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </Content>
                                    </ext:Panel>
                                    <ext:Panel ID="Panel9" runat="server" Title="Dirección Facturación" AutoHeight="true"
                                        FormGroup="true">
                                        <Content>
                                            <table border="0">
                                                <tr>
                                                    <td colspan="2">
                                                        <p>
                                                            <span class="x-label-text">Estado:</span><span style="color: red">*</span></p>
                                                        <ext:ComboBox ID="ComboBox1" runat="server" Width="130">
                                                            <Items>
                                                                <ext:ListItem Text="Morelos" />
                                                                <ext:ListItem Text="Distrito Federal" />
                                                                <ext:ListItem Text="Puebla" />
                                                                <ext:ListItem Text="Morelia" />
                                                            </Items>
                                                        </ext:ComboBox>
                                                    </td>
                                                    <td colspan="2">
                                                        <p>
                                                            <span class="x-label-text">Municipio:</span><span style="color: red">*</span></p>
                                                        <ext:ComboBox ID="ComboBox2" runat="server" Width="130">
                                                            <Items>
                                                                <ext:ListItem Text="Morelos" />
                                                                <ext:ListItem Text="Distrito Federal" />
                                                                <ext:ListItem Text="Puebla" />
                                                                <ext:ListItem Text="Morelia" />
                                                            </Items>
                                                        </ext:ComboBox>
                                                    </td>
                                                    <td colspan="2">
                                                        <p>
                                                            <span class="x-label-text">Ciudad:</span><span style="color: red">*</span></p>
                                                        <ext:ComboBox ID="ComboBox3" runat="server" Width="130">
                                                            <Items>
                                                                <ext:ListItem Text="Morelos" />
                                                                <ext:ListItem Text="Distrito Federal" />
                                                                <ext:ListItem Text="Puebla" />
                                                                <ext:ListItem Text="Morelia" />
                                                            </Items>
                                                        </ext:ComboBox>
                                                    </td>
                                                    <td colspan="2">
                                                        <p>
                                                            <span class="x-label-text">Asentamiento:</span><span style="color: red">*</span></p>
                                                        <ext:ComboBox ID="ComboBox4" runat="server" Width="130">
                                                            <Items>
                                                                <ext:ListItem Text="Morelos" />
                                                                <ext:ListItem Text="Distrito Federal" />
                                                                <ext:ListItem Text="Puebla" />
                                                                <ext:ListItem Text="Morelia" />
                                                            </Items>
                                                        </ext:ComboBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2">
                                                        <p>
                                                            <span class="x-label-text">Codigo Postal:</span><span style="color: red">*</span></p>
                                                        <ext:TextField ID="TextField1" runat="server" Width="130" Text="" />
                                                    </td>
                                                    <td colspan="2">
                                                        <p>
                                                            <span class="x-label-text">Calle:</span><span style="color: red">*</span></p>
                                                        <ext:TextField ID="TextField2" runat="server" Width="130" Text="" />
                                                    </td>
                                                    <td colspan="2">
                                                        <p>
                                                            <span class="x-label-text">Num. Int:</span><span style="color: red">*</span></p>
                                                        <ext:TextField ID="TextField3" runat="server" Width="130" Text="" />
                                                    </td>
                                                    <td colspan="2">
                                                        <p>
                                                            <span class="x-label-text">Num. Ext.:</span><span style="color: red">*</span></p>
                                                        <ext:TextField ID="TextField4" runat="server" Width="130" Text="" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="4">
                                                        <p>
                                                            <span class="x-label-text">Entre Calles:</span><span style="color: red">*</span></p>
                                                        <%-- <ext:TextField ID="TextField5" runat="server" Width="260" Text="" />--%>
                                                        <ext:TextArea ID="TextArea1" runat="server" Width="260" Height="50" />
                                                    </td>
                                                    <td colspan="4">
                                                        <p>
                                                            <span class="x-label-text">Referencias:</span><span style="color: red">*</span></p>
                                                        <%--<ext:TextField ID="TextField6" runat="server" Width="260" Text="" />--%>
                                                        <ext:TextArea ID="TextArea2" runat="server" Width="260" Height="50" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </Content>
                                    </ext:Panel>
                                </Content>
                            </ext:Panel>
                        </Items>
                    </ext:TabPanel>
                </Items>
            </ext:Panel>
        </East>
    </ext:BorderLayout>
</asp:Content>

