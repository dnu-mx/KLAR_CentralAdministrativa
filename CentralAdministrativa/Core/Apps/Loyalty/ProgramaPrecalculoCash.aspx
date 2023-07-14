<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProgramaPrecalculoCash.aspx.cs" Inherits="CentroContacto.ProgramaPrecalculoCash" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .cbStates-list
        {
            width: auto;
            font: 11px tahoma,arial,helvetica,sans-serif;
        }
        
        .cbStates-list th
        {
            font-weight: bold;
        }
        
        .cbStates-list td, .cbStates-list th
        {
            padding: 3px;
        }
        .Titulo
        {
            font-size: 20px;
            font-weight: bolder;
            font-family: Arial Unicode MS;
            color: Black;
        }
        
        .descripcion
        {
            font-size: 12px;
            text-justify: distribute;
            font-weight: normal;
            font-family: Arial Unicode MS;
            color: Black;
        }
    </style>

    <script type="text/javascript">
        var prepareToolbar = function (grid, toolbar, rowIndex, record) {

            if (record.get("ID_EstatusPrecalculo") == 1) { //programado
                toolbar.items.itemAt(0).show();
                toolbar.items.itemAt(1).hide();

            } else if (record.get("ID_EstatusPrecalculo") == 2) { //procesando
                toolbar.items.itemAt(0).hide();
                toolbar.items.itemAt(1).hide();

            } else if (record.get("ID_EstatusPrecalculo") == 3) { //procesado
                toolbar.items.itemAt(0).hide();
                toolbar.items.itemAt(1).show();
            }
        };

        var commandHandler = function (cmd, record) {
            switch (cmd) {
                case "Eliminar_Event":
                    Ext.net.DirectMethods.Eliminar_Event(record.json.ID_Programacion);
                    break;

                case "Descargar_Event":
                    Ext.net.DirectMethods.Descargar_Event();
                    break;
            }
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="MainBorderLayout" runat="server">
       
        <Center Split="true">
            <ext:Panel ID="PanelGeneral" runat="server" Layout="FitLayout" Title="Precalculo de Vencimiento de Puntos">
                <Items>
                    <ext:BorderLayout ID="InternalBorderLayout" runat="server">
                        <Center Split="true">
                            <ext:FormPanel ID="FormPanel1" runat="server" Border="false" Layout="ColumnLayout"
                                Height="50" >
                                <Items>
                                    <ext:Panel ID="PanelColumnDummy" runat="server" Border="false" Width="50" Height="210">
                                        <Items>
                                        </Items>
                                    </ext:Panel>
                                   <ext:Panel ID="Panel1" runat="server" Border="false" Width="250" Height="210">
                                        <Items>
                                            <ext:Image ID="Image1" runat="server" Width="155" Height="150" ImageUrl="Images/timer.png">
                                            </ext:Image>
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel ID="Panel2" runat="server" Border="false" Layout="RowLayout" AutoScroll="true" Width="600">
                                        <Items>
                                            <ext:Panel ID="Panel_Dummy" runat="server" Border="false" Header="true" LabelAlign="Left"
                                                Layout="FormLayout">                                                
                                                <Items>                                                    
                                                    <ext:Label runat="server" ID="Label7" Cls="Titulo" FieldLabel="" Text="Programación de Precalculo">
                                                    </ext:Label>
                                                </Items>
                                            </ext:Panel>
                                            <ext:Panel ID="Panel5" runat="server" Border="false" Header="false" LabelAlign="Left"
                                                Layout="FormLayout">
                                                <Items>
                                                    <ext:Label ID="Label8" runat="server" Text="">
                                                    </ext:Label>
                                                </Items>
                                            </ext:Panel>
                                            <ext:Panel ID="Panel6" runat="server" Border="false" Header="false" LabelAlign="Left"
                                                Layout="FormLayout">
                                                <Items>
                                                    <ext:Label ID="Label9" runat="server" Text="">
                                                    </ext:Label>
                                                </Items>
                                            </ext:Panel>
                                            <ext:Panel ID="Panel8" runat="server" Border="false" Header="false" LabelAlign="Left"
                                                Layout="FormLayout">
                                                <Items>
                                                    <ext:Label ID="Label10" runat="server" Text="">
                                                    </ext:Label>
                                                </Items>
                                            </ext:Panel>

                                            <ext:Panel ID="Panel3" runat="server" Border="false" Header="false" LabelAlign="Left"
                                                Layout="HBoxLayout">
                                                <Items>
                                                    <ext:DateField ID="dtFechaCalculo" runat="server" FieldLabel="Fecha a Calcular"
                                                        AnchorHorizontal="100%" Format="dd/MM/yyyy" AllowBlank="false" Width="300" EnableKeyEvents="true">
                                                        <CustomConfig>
                                                            <ext:ConfigItem Name="startDateField" Value="#{dtFechaCalculo}" Mode="Value" />
                                                        </CustomConfig>
                                                    </ext:DateField>
                                                    <ext:Hidden runat="server" Flex="1" />
                                                    <ext:Button ID="btnNuevaPromocion" runat="server" Text="Programar" Icon="Add" Width="90">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnAgregar_Click" />
                                                        </DirectEvents>
                                                    </ext:Button>

                                                    <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                                        <DirectEvents>
                                                            <Click OnEvent="Download" IsUpload="true"
                                                                After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                                    ReporteMask.StopMask();" />
                                                        </DirectEvents>
                                                    </ext:Button>
                                                </Items>
                                            </ext:Panel>
                                            <ext:Panel ID="PanelDummy1" runat="server" Border="false" Header="false" LabelAlign="Left"
                                                Layout="FormLayout">
                                                <Items>
                                                    <ext:Label ID="Label2" runat="server" Text="">
                                                    </ext:Label>
                                                </Items>
                                            </ext:Panel>                                                                                       
                                            <ext:Panel ID="Panel4" runat="server" Width="500" Border="false" Header="false" LabelAlign="Left" Layout="FormLayout">
                                                <Items>
                                                    <ext:Label runat="server"  ID="lblDescripcionRegla" Cls="descripcion" FieldLabel=""
                                                        Text="Seleccione la fecha a la que desea realizar el precalculo de puntos y posteriormente descarge el reporte al siguiente día despúes de haber solicitado la información.">
                                                    </ext:Label>
                                                </Items>
                                            </ext:Panel>
                                        </Items>
                                    </ext:Panel>
                                </Items>
                            </ext:FormPanel>
                        </Center>
                        <South Split="true">
                            <ext:Panel ID="Panel7" runat="server" Title="Solicitudes de PreCalculo" Height="400"
                                Split="true" Padding="6" MinWidth="650" Collapsible="false" Layout="FitLayout">
                                <Items>
                                    <ext:GridPanel ID="GridPrecalculo" runat="server" StripeRows="true" Header="true"
                                        Layout="fit" Region="Center">
                                        <Store>
                                            <ext:Store ID="StorePrecalculo" runat="server" OnRefreshData="StorePrecalculo_Refresh">
                                                <Reader>
                                                    <ext:JsonReader IDProperty="ID_Programacion">
                                                        <Fields>
                                                            <ext:RecordField Name="FechaSolicitud" />
                                                            <ext:RecordField Name="FechaCalcular" />
                                                            <ext:RecordField Name="Estatus" />
                                                            <ext:RecordField Name="ID_EstatusPrecalculo" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Store>
                                        <ColumnModel ID="ColumnModel12" runat="server">
                                            <Columns>
                                                <ext:DateColumn Width="110" Header="Fecha Solicitud" Sortable="true" DataIndex="FechaSolicitud"
                                                    Format="dd/MM/yyyy" />
                                                <ext:DateColumn Width="110" Header="Fecha a Calcular" Sortable="true" DataIndex="FechaCalcular"
                                                    Format="dd/MM/yyyy" />
                                                <ext:Column Width="200" Header="Estatus" Sortable="true" DataIndex="Estatus" />
                                                
                                                <ext:CommandColumn Width="30">
                                                    <Commands>
                                                        <ext:GridCommand CommandName="Eliminar_Event" Icon="Delete">
                                                            <ToolTip Text="Eliminar Programación" />
                                                        </ext:GridCommand>
                                                        <ext:GridCommand CommandName="Descargar_Event" Icon="Disk">
                                                            <ToolTip Text="Descargar" />
                                                        </ext:GridCommand>
                                                    </Commands>
                                                    <PrepareToolbar Fn="prepareToolbar" />
                                                </ext:CommandColumn>
                                            </Columns>
                                        </ColumnModel>
                                        <Listeners>
                                            <Command Fn="commandHandler" />
                                        </Listeners>
                                    </ext:GridPanel>
                                </Items>
                            </ext:Panel>
                        </South>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
