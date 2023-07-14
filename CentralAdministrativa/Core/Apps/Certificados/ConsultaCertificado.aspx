<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ConsultaCertificado.aspx.cs" Inherits="Certificados.ConsultaCertificado" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
 <script type="text/javascript">
     var onKeyUp = function (field) {
         var v = this.processValue(this.getRawValue()),
                field;

         if (this.startDateField) {
             field = Ext.getCmp(this.startDateField);
             field.setMaxValue();
             this.dateRangeMax = null;
         } else if (this.endDateField) {
             field = Ext.getCmp(this.endDateField);
             field.setMinValue();
             this.dateRangeMin = null;
         }

         field.validate();
     };

     var submitValue = function (grid, hiddenFormat, format) {
         hiddenFormat.setValue(format);
         grid.submitData(false);
     };

     var template = '<span style="color:{0};">{1}</span>';

     var change = function (value) {
         return String.format(template, (value > 0) ? "green" : "red", value);
     };

     var pctChange = function (value) {
         return String.format(template, (value > 0) ? "green" : "red", value + "%");
     };
    </script>
    <script type="text/javascript">

        var prepareToolbar = function (grid, toolbar, rowIndex, record) {
            // for example hide 'Edit' button if price < 50
            if (record.get("ID_Activacion") == 0) { //Activo
                toolbar.items.get(1).hide();
                toolbar.items.get(2).hide();
                toolbar.items.get(3).hide();

            } else {

                if (record.get("ID_Estatus") == 1) { //Otro no Activo
                    toolbar.items.get(0).hide();
                    toolbar.items.get(1).hide();
                    //toolbar.items.get(2).hide();
                    toolbar.items.get(3).hide();

                } else if (record.get("ID_Estatus") == 2) { //Otro no Activo

                    toolbar.items.get(0).hide();
                   // toolbar.items.get(1).hide();
                    toolbar.items.get(2).hide();
                    toolbar.items.get(3).hide();

                } else { //Otro no Activo
                    toolbar.items.get(0).hide();
                    toolbar.items.get(1).hide();
                    toolbar.items.get(2).hide();
                    //toolbar.items.get(3).hide();
                }
            }
        };

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center Split="true">
            <ext:Panel ID="Panel5" runat="server" Width="428.5" Title="Certificados" Collapsed="false"
                Collapsible="true" Layout="Fit" AutoScroll="true">
                <Content>
                 <ext:Hidden ID="FormatType" runat="server" />
                    <ext:Store ID="storeCertificados" runat="server" OnRefreshData="RefreshGrid">
                        <Reader>
                            <ext:JsonReader IDProperty="UserId">
                                <Fields>
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanel1" runat="server" StoreID="storeCertificados" StripeRows="true"
                                Header="false" Border="false">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel1" runat="server">
                                </ColumnModel>
                                <DirectEvents>
                                    <Command OnEvent="EjecutarComando">
                                        <ExtraParams>
                                            <ext:Parameter Name="ID_Colectiva" Value="record.data.ID_ColectivaTerminal" Mode="Raw" />
                                            <ext:Parameter Name="Sucursal" Value="record.data.Sucursal" Mode="Raw" />
                                            <ext:Parameter Name="Afiliacion" Value="record.data.Afiliacion" Mode="Raw" />
                                            <ext:Parameter Name="Terminal" Value="record.data.Terminal" Mode="Raw" />
                                            <ext:Parameter Name="ID_CadenaComercial" Value="record.data.ID_CadenaComercial" Mode="Raw" />
                                            <ext:Parameter Name="ClaveCadena" Value="record.data.ClaveCadena" Mode="Raw" />
                                            <ext:Parameter Name="ID_Certificado" Value="record.data.ID_Certificado" Mode="Raw" />
                                            <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                        </ExtraParams>
                                    </Command>
                                </DirectEvents>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                                    </ext:RowSelectionModel>
                                </SelectionModel>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" PageSize="10" StoreID="storeCertificados"
                                        DisplayInfo="true" DisplayMsg="Mostrando Colectivas {0} - {1} de {2}" />
                                </BottomBar>
                                 <%--<TopBar>
                                    <ext:Toolbar ID="Toolbar2" runat="server">
                                        <Items>
                                            <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                                            <ext:Button ID="Button2" runat="server" Text="Exportar a XML" Icon="PageCode">
                                                <Listeners>
                                                    <Click Handler="submitValue(#{GridPanel1}, #{FormatType}, 'xml');" />
                                                </Listeners>
                                            </ext:Button>
                                            <ext:Button ID="Button3" runat="server" Text="Exportar a Excel" Icon="PageExcel">
                                                <Listeners>
                                                    <Click Handler="submitValue(#{GridPanel1}, #{FormatType}, 'xls');" />
                                                </Listeners>
                                            </ext:Button>
                                            <ext:Button ID="Button4" runat="server" Text="Exportar a CSV" Icon="PageAttach">
                                                <Listeners>
                                                    <Click Handler="submitValue(#{GridPanel1}, #{FormatType}, 'csv');" />
                                                </Listeners>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>--%>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>

