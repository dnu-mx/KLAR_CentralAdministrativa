<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CVDP.aspx.cs" Inherits="TpvWeb.CVDP" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var fullName = function (value, metadata, record, rowIndex, colIndex, store) {
            return "<b>" + record.data.Nombre + "</b>";
        };
    </script>
    <style type="text/css">
        .x-grid3-row-body p {
            margin: 3px 3px 7px 3px !important;
            width: 99%;
            color: black;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Hidden ID="hdnIdGrupoMA" runat="server" />
    <ext:Hidden ID="hdnDescripcion" runat="server" />
    <ext:Hidden ID="hdnIdValorReferido" runat="server" />
    <ext:Window ID="WdwValorParametro" runat="server" Title="Editar Valor Parámetro" Width="420" AutoHeight="true" Hidden="true"
        Resizable="false">
        <items>
            <ext:FormPanel ID="FormPanelValorParamTxt" runat="server" Padding="10" MonitorValid="true" LabelAlign="Left" LabelWidth="70">
                <Items>
                    <ext:TextField ID="txtParametro" runat="server" FieldLabel="Nombre" Width="300"
                        AllowBlank="false" Selectable="false" ReadOnly="true" />
                    <ext:TextField ID="txtValorParFloat" runat="server" FieldLabel="Valor" Width="300" MaxLength="50"
                        MaskRe="[0-9\.]" Hidden="true"/>
                    <ext:TextField ID="txtValorParInt" runat="server" FieldLabel="Valor" Width="300" MaxLength="50"
                        MaskRe="[0-9]" Hidden="true"/>
                    <ext:TextArea ID="txtValorParString" runat="server" FieldLabel="Valor" Width="300" AutoHeight="true"
                        MaxLength="50" Hidden="true" />
                    <ext:ComboBox ID="cBoxValorPar" runat="server" FieldLabel="Valor" Width="300" Hidden="true">
                        <Items>
                            <ext:ListItem Text="Sí" Value="true" />
                            <ext:ListItem Text="No" Value="false" />
                        </Items>
                    </ext:ComboBox>
                </Items>
                <Buttons>
                    <ext:Button runat="server" Text="Cancelar" Icon="Cancel">
                        <Listeners>
                            <Click Handler="#{WdwValorParametro}.hide();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button runat="server" Text="Guardar Cambio" Icon="Disk">
                        <DirectEvents>
                            <Click OnEvent="btnGuardarValorParametro_Click"/>
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </items>
    </ext:Window>
    <ext:BorderLayout ID="BorderLayout1" runat="server">
       <West Split="true">
            <ext:GridPanel ID="GridResultados" runat="server" AutoExpandColumn="Descripcion" Width="325"
                Layout="FitLayout" Title="Grupo de Tarjetas">
                <Store>
                    <ext:Store ID="StoreGrupoMA" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_GrupoMA">
                                <Fields>
                                    <ext:RecordField Name="ID_GrupoMA" />
                                    <ext:RecordField Name="ClaveGrupo" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <TopBar>
                    <ext:Toolbar ID="Toolbar2" runat="server">
                        <Items>
                            <ext:TextField ID="txtDescripcion" runat="server" EmptyText="Grupo de Tarjetas" Width="130" />
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click">
                                        <EventMask ShowMask="true" Msg="Buscando Grupo de Tarjetas..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarFill runat="server" />
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <ColumnModel runat="server">
                    <Columns>
                        <ext:Column DataIndex="ID_GrupoMA" Hidden="true" />
                        <ext:Column DataIndex="ClaveGrupo" Header="Clave" Width="90" />
                        <ext:Column DataIndex="Descripcion" Header="Descripcion" Width="110" />
                    </Columns>
                </ColumnModel>
                <SelectionModel>
                    <ext:RowSelectionModel SingleSelect="true" />
                </SelectionModel>
                <DirectEvents>
                    <RowClick OnEvent="selectRowResultados_Event">
                        <EventMask ShowMask="true" Msg="Obteniendo Parámetros..." MinDelay="500" />
                        <ExtraParams>
                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridResultados}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                        </ExtraParams>
                    </RowClick>
                </DirectEvents>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreGrupoMA" DisplayInfo="true"
                        DisplayMsg="{0} - {1} de {2}" />
                </BottomBar>
            </ext:GridPanel>
        </West>
        <Center Split="true">
            <ext:GridPanel ID="GridPanelParametros" runat="server" Header="true" AutoScroll="true" AutoWidth="true"
                Layout="FitLayout" Disabled="true" Title="-">
                <Store>
                    <ext:Store ID="StoreValoresParametros" runat="server" AutoLoad="false">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_ValorReferido">
                                <Fields>
                                    <ext:RecordField Name="ID_ValorReferido" />
                                    <ext:RecordField Name="Nombre" />
                                    <ext:RecordField Name="Descripcion" />
                                    <ext:RecordField Name="TipoDato" />
                                    <ext:RecordField Name="Valor" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <ColumnModel runat="server">
                    <Columns>
                        <ext:Column DataIndex="ID_ValorReferido" Hidden="true" />
                        <ext:Column DataIndex="Nombre" Header="Nombre" Width="370">
                            <Renderer Fn="fullName" />
                            <Editor>
                                <ext:DisplayField runat="server" StyleSpec="font-weight:bold;font-family:segoe ui;font-size: 12px;" />
                            </Editor>
                        </ext:Column>
                        <ext:Column DataIndex="Valor" Header="Valor" Width="150" />
                        <ext:CommandColumn Width="80">
                            <Commands>
                                <ext:GridCommand Icon="Pencil" CommandName="Edit">
                                    <ToolTip Text="Editar Valor" />
                                </ext:GridCommand>
                            </Commands>
                        </ext:CommandColumn>
                    </Columns>
                </ColumnModel>
                <View>
                    <ext:GridView runat="server" EnableRowBody="true">
                        <GetRowClass Handler="rowParams.body = '<p>' + record.data.Descripcion + '</p>'; return 'x-grid3-row-expanded';" />
                    </ext:GridView>
                </View>
                <SelectionModel>
                    <ext:RowSelectionModel runat="server" SingleSelect="true" />
                </SelectionModel>
                <DirectEvents>
                    <Command OnEvent="EjecutarComandoParametros">
                        <ExtraParams>
                            <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                            <ext:Parameter Name="Values" Value="Ext.encode(record.data)" Mode="Raw" />
                        </ExtraParams>
                    </Command>
                </DirectEvents>
            </ext:GridPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
