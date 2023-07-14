<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="EdoCtaBatch.aspx.cs" Inherits="Cajero.EdoCtaBatch" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center Split="true" Collapsible="true">
            <ext:Panel ID="Panel5" runat="server" Title="Cargar el Estado de Cuenta Bancario"
                Collapsed="false" Layout="Fit" AutoScroll="true" Height="50">
                <Items>
                    <ext:Panel runat="server" Border="false" AutoScroll="true" ID="TabPanel1">
                        <Items>
                            <ext:ComboBox ID="cmbBanco" FieldLabel="Banco" StoreID="StoreBanco" EmptyText="Selecciona una Opción..."
                                runat="server" Width="400" Editable="false" AnchorHorizontal="90%" AllowBlank="false"
                                MsgTarget="Side">
                                <Items>
                                    <ext:ListItem Text="Bancomer" />
                                    <ext:ListItem Text="Banamex" />
                                    <ext:ListItem Text="Banorte" />
                                    <ext:ListItem Text="Banco Azteca" />
                                </Items>
                            </ext:ComboBox>
                            <ext:FileUploadField ID="BasicField" runat="server" AllowBlank="false" Width="400"
                                EmptyText="Selecciona un Archivo" Icon="Attach" />
                        </Items>
                    </ext:Panel>
                </Items>
            </ext:Panel>
        </Center>
        <South Split="true" Collapsible="true">
            <ext:Panel ID="Panel2" runat="server" Title="Registros Obtenidos del Archivo" Collapsed="false"
                Layout="Fit" AutoScroll="true" Height="400">
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
