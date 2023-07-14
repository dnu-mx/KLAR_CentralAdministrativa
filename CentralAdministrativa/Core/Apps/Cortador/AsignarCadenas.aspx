<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AsignarCadenas.aspx.cs" Inherits="Cortador.AsignarCadenas" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var prepareToolbar = function (grid, toolbar, rowIndex, record) {
            if (record.get("Activo") == true) {
                toolbar.items.get(1).hide();
            } else {
                toolbar.items.get(0).hide();
            }
        }

        var startEditing = function (e) {
            if (e.getKey() == e.ENTER) {
                var grid = GridCortes,
                    record = grid.getSelectionModel().getSelected(),
                    index = grid.store.indexOf(record);

                grid.startEditing(index, 1);
            }
        };

        var afterEdit = function (e) {
            // Call DirectMethod
            Cortador.ActualizaPeriodo(
                e.record.data.ID_Asignacion,
                e.record.data.ID_CadenaComercial,
                e.record.data.Cadena,
                e.record.data.ID_ConfiguracionCorte,
                e.record.data.Periodo);
        };
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">  
    <ext:Viewport ID="ViewPort1" runat="server">
        <Items>
            <ext:BorderLayout ID="BorderLayout1" runat="server">
                <Center Split="true">
                    <ext:FormPanel ID="FormPanelConsultaColectivas" runat="server" Title="Cadena Comercial" Layout="FitLayout">
                        <Items>
                            <ext:GridPanel ID="GridConsultaColectivas" runat="server" Layout="FitLayout" StripeRows="true"
                                Header="false" Border="false">
                                <Store>
                                    <ext:Store ID="StoreConsultaColectivas" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Colectiva">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Colectiva" />
                                                    <ext:RecordField Name="ClaveColectiva" />
                                                    <ext:RecordField Name="NombreCadena" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <TopBar>
                                    <ext:Toolbar ID="ToolbarConsultaColectivas" runat="server">
                                        <Items>
                                            <ext:TextField ID="txtClaveColectiva" EmptyText="Clave" Width="80" runat="server" />
                                            <ext:TextField ID="txtNombreCadena" EmptyText="Nombre" Width="200" runat="server" />
                                            <ext:Button ID="btnBuscarCadena" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnBuscarCadena_Click" Before="var valid= #{FormPanelConsultaColectivas}.getForm().isValid(); if (!valid) {} return valid;">
                                                        <EventMask ShowMask="true" Msg="Buscando Cadena Comercial..." MinDelay="500" />
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <ColumnModel ID="ColumnModel8" runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_Colectiva" Header="ID" Width="40" />
                                        <ext:CommandColumn Width="30">
                                            <Commands>
                                                <ext:GridCommand Icon="Tick" CommandName="Select">
                                                    <ToolTip Text="Seleccionar" />
                                                </ext:GridCommand>
                                            </Commands>
                                        </ext:CommandColumn>
                                        <ext:Column DataIndex="ClaveColectiva" Header="Clave" Width="200"/>
                                        <ext:Column DataIndex="NombreCadena" Header="Nombre" Width="400" />
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel SingleSelect="true" />
                                </SelectionModel>
                                <DirectEvents>
                                    <RowClick OnEvent="selectCadena_Event">
                                        <ExtraParams>
                                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridConsultaColectivas}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                        </ExtraParams>
                                    </RowClick>
                                     <Command OnEvent="selectComand_Event">
                                        <ExtraParams>
                                            <ext:Parameter Name="Values" Value="Ext.encode(record.data)" Mode="Raw" />
                                            <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                        </ExtraParams>
                                    </Command>
                                </DirectEvents>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="StoreConsultaColectivas" DisplayInfo="true"
                                        DisplayMsg="Cadenas Comerciales {0} - {1} de {2}" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Items>
                    </ext:FormPanel>
                </Center>
                <South Split="true">
                    <ext:FormPanel ID="FormPanelCortes" runat="server" Title="Cortes" Height="200" Layout="FitLayout">
                        <Content>
                            <ext:Store ID="StorePeriodos" runat="server">
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_Periodo">
                                        <Fields>
                                            <ext:RecordField Name="ID_Periodo" />
                                            <ext:RecordField Name="Cve_Periodo" />
                                            <ext:RecordField Name="Descripcion" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Content>
                        <Items>
                            <ext:GridPanel ID="GridCortes" runat="server" Header="false" Border="false">
                                <Store>
                                    <ext:Store ID="StoreCortes" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_ConfiguracionCorte">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Asignacion" />
                                                    <ext:RecordField Name="ID_CadenaComercial" />
                                                    <ext:RecordField Name="Cadena" />
                                                    <ext:RecordField Name="ID_ConfiguracionCorte" />
                                                    <ext:RecordField Name="Corte" />
                                                    <ext:RecordField Name="ID_Periodo" />
                                                    <ext:RecordField Name="Periodo" />
                                                    <ext:RecordField Name="Activo" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <Listeners>
                                    <AfterEdit Fn="afterEdit" />
                                </Listeners>
                                <Plugins>
                                    <ext:RowEditor ID="RowEditor1" runat="server" SaveText="Actualizar" CancelText="Cancelar">
                                        <Listeners>
                                            <AfterEdit Fn="afterEdit" />
                                        </Listeners>
                                    </ext:RowEditor>
                                </Plugins>
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel6" runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_Asignacion" Hidden="true" />
                                        <ext:Column DataIndex="ID_CadenaComercial" Hidden="true" />
                                        <ext:Column DataIndex="Cadena" Hidden="true" />
                                        <ext:Column DataIndex="ID_ConfiguracionCorte" Header="ID" Width="10" />
                                        <ext:Column DataIndex="ID_Periodo" Hidden="true" />
                                        <ext:CommandColumn Width="10" Align="Center">
                                            <Commands>
                                                <ext:GridCommand Icon="LinkBreak" CommandName="Unlink">
                                                    <ToolTip Text="Desasignar" />
                                                </ext:GridCommand>
                                                <ext:GridCommand Icon="Link" CommandName="Link">
                                                    <ToolTip Text="Asignar" />
                                                </ext:GridCommand>
                                            </Commands>
                                            <PrepareToolbar Fn="prepareToolbar" />
                                        </ext:CommandColumn>
                                        <ext:Column DataIndex="Corte" Header="Descripción del Corte" Width="300" />
                                        <ext:Column DataIndex="Periodo" Header="Periodo">
                                             <Editor>
                                                <ext:ComboBox ID="cmbPeriodo"
                                                    runat="server"
                                                    StoreID="StorePeriodos"
                                                    DisplayField="Descripcion"
                                                    ValueField="ID_Periodo" />
                                            </Editor>
                                        </ext:Column>
                                        <ext:Column DataIndex="Activo" Hidden="true" />
                                    </Columns>
                                </ColumnModel>
                                <DirectEvents>
                                    <Command OnEvent="EjecutaComando">
                                        <ExtraParams>
                                            <ext:Parameter Name="Values" Value="Ext.encode(record.data)" Mode="Raw" />
                                            <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                        </ExtraParams>
                                    </Command>
                                    <RowDblClick>
                                        <ExtraParams>
                                            <ext:Parameter Name="ID_Asignacion" Value="Ext.encode(#{GridPanelConsulta}.getRowsValues({selectedOnly:true})[0].ID_Asignacion)" Mode="Raw" />
                                        </ExtraParams>
                                    </RowDblClick>
                                </DirectEvents>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel1" SingleSelect="true" runat="server" />
                                </SelectionModel>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreCortes" DisplayInfo="true"
                                        DisplayMsg="Cortes {0} - {1} de {2}" />
                                </BottomBar>
                                <View>
                                    <ext:GroupingView ID="GroupingView1" runat="server" ForceFit="true" MarkDirty="false"
                                        ShowGroupName="false" EnableNoGroups="true" HideGroupedColumn="true" />
                                </View>
                            </ext:GridPanel>
                        </Items>
                    </ext:FormPanel>
                </South>
            </ext:BorderLayout>
        </Items>
    </ext:Viewport>
</asp:Content>