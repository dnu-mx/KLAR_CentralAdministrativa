<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" ValidateRequest="false"
    CodeBehind="RegistraMonitoreo.aspx.cs" Inherits="CentroContacto.RegistraMonitoreo" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var afterEdit = function (e) {
            if (e.record.data.Valor == '') {
                Ext.Msg.alert('Registro de Datos', 'El valor es obligatorio.');
                return false;
            }

            RegistroMonitoreo.ActualizaDato(e.record.data.ID_ConceptoMonitoreo, e.record.data.Valor);
        };

        var fullName = function (value, metadata, record, rowIndex, colIndex, store) {
            return "<b>" + record.data.DatoACapturar + "</b>";
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
    <ext:BorderLayout runat="server">
        <Center Split="true">
            <ext:FormPanel runat="server" Border="false" Padding="50" Layout="FitLayout">
                <Items>
                    <ext:GridPanel ID="GridDatosMonitoreo" runat="server" Layout="FitLayout" Height="500" Width="300"
                        AutoExpandColumn="DatoACapturar">
                        <Store>
                            <ext:Store ID="StoreDatosMonitoreo" runat="server">
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_ConceptoMonitoreo">
                                        <Fields>
                                            <ext:RecordField Name="ID_ConceptoMonitoreo" />
                                            <ext:RecordField Name="Clave" />
                                            <ext:RecordField Name="DatoACapturar" />
                                            <ext:RecordField Name="Valor" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <ColumnModel runat="server">
                            <Columns>
                                <ext:Column runat="server" Hidden="true" DataIndex="ID_ConceptoMonitoreo" />
                                <ext:Column DataIndex="DatoACapturar" Header="Dato" Width="350">
                                    <Renderer Fn="fullName" />
                                    <Editor>
                                        <ext:DisplayField runat="server" StyleSpec="font-weight:bold;font-family:segoe ui;font-size: 12px;" />
                                    </Editor>
                                </ext:Column>
                                <ext:Column DataIndex="Valor" Header="Valor" Sortable="true" Width="200">
                                    <Editor>
                                        <ext:TextField EmptyText="" runat="server" />
                                    </Editor>
                                </ext:Column>
                            </Columns>
                        </ColumnModel>
                        <View>
                            <ext:GridView runat="server" EnableRowBody="true">
                                <GetRowClass Handler="rowParams.body = '<p>' + record.data.Clave + '</p>'; return 'x-grid4-row-expanded';" />
                            </ext:GridView>
                        </View>
                        <SelectionModel>
                            <ext:RowSelectionModel runat="server" SingleSelect="true" />
                        </SelectionModel>
                        <Plugins>
                            <ext:RowEditor runat="server" SaveText="Actualizar" CancelText="Cancelar">
                                <Listeners>
                                    <AfterEdit Fn="afterEdit" />
                                </Listeners>
                            </ext:RowEditor>
                        </Plugins>
                        <FooterBar>
                            <ext:Toolbar runat="server" EnableOverflow="true">
                                <Items>
                                    <ext:Button ID="btnRefresh" runat="server" Text="Actualizar" Icon="ArrowRefresh">
                                        <DirectEvents>
                                            <Click OnEvent="btnRefresh_Click" />
                                        </DirectEvents>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </FooterBar>
                        <LoadMask ShowMask="false" />
                    </ext:GridPanel>
                </Items>
            </ext:FormPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
