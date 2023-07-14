<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ColectivaBatch.aspx.cs" Inherits="Usuarios.ColectivaBatch" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center Split="true" Collapsible="true">
            <ext:Panel ID="Panel5" runat="server" Title="Agregar Usuarios" Collapsed="false"
                Layout="Fit" AutoScroll="true" Height="150">
                <Items>
                    <ext:Panel runat="server"   Border="false" AutoScroll="true" ID="TabPanel1" >
                        <%--<Items>
                            <ext:Panel ID="Panel6" runat="server" Title="Nuevos Usuario en Batch" Border="false"
                                AutoScroll="true" Padding="6">--%>
                                <Content>
                                    <table>
                                        <tr>
                                            <td>
                                                <span class="x-label-text">Por favor, selecciona un archivo de Usuarios con formato
                                                    válido:</span><span style="color: red">*</span></p>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                            </td>
                                            <td>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right">
                                                <ext:FileUploadField ID="BasicField" runat="server" Width="400" EmptyText="Selecciona un Archivo"
                                                    Icon="Attach" />
                                            </td>
                                            <td>
                                            </td>
                                        </tr>
                                    </table>
                                </Content>
                            <%--</ext:Panel>
                        </Items>--%>
                    </ext:Panel>
                </Items>
            </ext:Panel>
        </Center>
        <South Split="true" Collapsible="true">
            <ext:Panel ID="Panel2" runat="server" Title="Registros Leídos del Archivo" Collapsed="false"
                Layout="Fit" AutoScroll="true" Height="450">
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
        </South>
    </ext:BorderLayout>
</asp:Content>

