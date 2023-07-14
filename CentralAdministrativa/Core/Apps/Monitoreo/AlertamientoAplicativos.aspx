<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="AlertamientoAplicativos.aspx.cs" Inherits="Monitoreo.AlertamientoAplicativos" %>


<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var prepareToolbar = function (grid, toolbar, rowIndex, record) {
            if ((record.get("ATiempo") == 0 &&
                record.get("MenosDoceHoras") == 0) ||
                record.get("EstadoAlertamiento") == 'Inactivo') {
                toolbar.items.get(0).hide();
            }
        }

        var getRowClass = function (record, grid) {
            if (record.get("EstadoAlertamiento") == 'Inactivo') {
                return "verde_background_color";
            } else if ((record.get("ATiempo") == 0 &&
                record.get("MenosDoceHoras") == 0)) {
                return "rojo_background_color";
            } 
        }

        function resetToolbar(tbar) {
            tbar.updateInfo();
            tbar.inputItem.setValue(1);
            tbar.afterTextItem.setText(String.format(tbar.afterPageText, 1));
            tbar.next.setDisabled(true);
            tbar.prev.setDisabled(true);
            tbar.first.setDisabled(true);
            tbar.last.setDisabled(true);
        }
    </script>
    <style type="text/css">
        .rojo_background_color {
            background-color: lightpink;
        }
        .amarillo_background_color {
            background-color: lightgoldenrodyellow;
        }
        .verde_background_color {
            background-color: lightgreen;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Hidden ID="hdnIdAlerta" runat="server" />
    <ext:Window ID="WdwComentariosCierre" runat="server" Title="Cierre de Alertamiento" Width="350" Height="250"
        Hidden="true" Modal="true" Resizable="false" Icon="CheckError">
        <Items>
            <ext:FormPanel ID="FormPanelComentarios" runat="server" Padding="10" LabelAlign="Top" LabelWidth="70" 
                Border="false" Layout="FormLayout">
                <Items>
                    <ext:TextArea ID="txtComentarios" runat="server" FieldLabel="Razón del Cierre <span style='color:red;'>*   </span>"
                        AllowBlank="false" Width="310" Height="150" MaxLength="800"/>
                </Items>
            </ext:FormPanel>
        </Items>
        <Buttons>
            <ext:Button ID="btnComentarios" runat="server" Text="Cerrar Alertamiento" Icon="Tick">
                <DirectEvents>
                    <Click OnEvent="btnComentarios_Click" Before="var valid= #{FormPanelComentarios}.getForm().isValid(); if (!valid) {} return valid;">
                        <EventMask ShowMask="true" Msg="Cerrando Alertamiento..." MinDelay="500" />
                    </Click>
                </DirectEvents>
            </ext:Button>
            <ext:Button runat="server" Text="Cancelar" Icon="Cross">
                <Listeners>
                    <Click Handler="#{WdwComentariosCierre}.hide();" />
                </Listeners>
            </ext:Button>
        </Buttons>
    </ext:Window>
    <ext:BorderLayout runat="server">
        <Center Split="true">
            <ext:GridPanel ID="GridAlertamientos" runat="server" Border="false" Height="280" AutoScroll="true" Layout="FitLayout"
                MonitorWindowResize="true">
                <TopBar>
                    <ext:Toolbar runat="server">
                        <Items>
                            <ext:ComboBox ID="cmbEstatus" runat="server" EmptyText="Estatus" Width="100" AllowBlank="false">
                                <Items>
                                    <ext:ListItem Text="Activo" Value="1" />
                                    <ext:ListItem Text="Inactivo" Value="0" />
                                </Items>
                            </ext:ComboBox>
                            <ext:DateField ID="dfFecha" runat="server" Vtype="daterange" MaskRe="[0-9\/]" Width="150"
                                EmptyText="Fecha" Format="dd/MM/yyyy" EnableKeyEvents="true" AutoDataBind="true"
                                 MaxDate="<%# DateTime.Now %>" InvalidText="Fecha inválida. Debe tener el formato DD/MM/AAAA">
                            </ext:DateField>
                            <ext:ComboBox ID="cmbAplicacion" runat="server" EmptyText="Aplicación" Width="150"
                                DisplayField="Descripcion" ListWidth="350" ValueField="ID_AplicativoFirma">
                                <Store>
                                    <ext:Store ID="StoreAplicacion" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_AplicativoFirma">
                                                <Fields>
                                                    <ext:RecordField Name="ID_AplicativoFirma" />
                                                    <ext:RecordField Name="Descripcion" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                            </ext:ComboBox>
                            <ext:TextField ID="txtInstancia" runat="server" EmptyText="Instancia" Width="150" MaxLenght="100" />
                            <ext:ToolbarSeparator runat="server" />
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier" Width="100">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                        Before="var valid1 = #{dfFecha}.isValid();
                                        if (!valid1) {
                                            var valid = valid1; }
                                        else {
                                            resetToolbar(#{PagingAlertamientos});
                                            #{GridAlertamientos}.getStore().sortInfo = null; }
                                        return valid; ">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarSeparator runat="server" />
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh" Width="100">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnBuscarHide" runat="server" Hidden="true">
                                <Listeners>
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Alertamientos...' });
                                        #{GridAlertamientos}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <Store>
                    <ext:Store ID="StoreAlertas" runat="server" OnRefreshData="StoreAlertas_RefreshData" 
                        RemoteSort="true" AutoLoad="true">
                        <AutoLoadParams>
                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                        </AutoLoadParams>
                        <Proxy>
                            <ext:PageProxy />
                        </Proxy>
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Alertamiento">
                                <Fields>
                                    <ext:RecordField Name="ID_Alertamiento" />
                                    <ext:RecordField Name="ClaveAplicacion" />
                                    <ext:RecordField Name="Instancia" />
                                    <ext:RecordField Name="DescripcionAlertamiento" />
                                    <ext:RecordField Name="FechaCreacionAlertamiento" />
                                    <ext:RecordField Name="ATiempo" />
                                    <ext:RecordField Name="MenosDoceHoras" />
                                    <ext:RecordField Name="UsuarioCreacionAlertamiento" />
                                    <ext:RecordField Name="FechaModificacionAlertamiento" />
                                    <ext:RecordField Name="UsuarioModificacionAlertamiento" />
                                    <ext:RecordField Name="ComentariosAlertamiento" />
                                    <ext:RecordField Name="EstadoAlertamiento" />
                                    <ext:RecordField Name="ID_AplicativoFirma" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <ColumnModel id="ColumnModel1" runat="server">
                    <Columns>
                        <ext:CommandColumn Header="Acción" Width="60">
                            <PrepareToolbar Fn="prepareToolbar" />
                            <Commands>
                                <ext:GridCommand Icon="CheckError" CommandName="Cerrar">
                                    <Tooltip Text="Cerrar Alertamiento" />
                                </ext:GridCommand>
                            </Commands>
                        </ext:CommandColumn>
                        <ext:Column Header="ID" DataIndex="ID_Alertamiento" Width="50" />
                        <ext:Column Header="Aplicación" DataIndex="ClaveAplicacion" />
                        <ext:Column Header="Instancia" DataIndex="Instancia" />
                        <ext:Column Header="Descripción" Sortable="true" DataIndex="DescripcionAlertamiento" Width="220" />
                        <ext:DateColumn Header="Fecha de Creación" Sortable="true" DataIndex="FechaCreacionAlertamiento"
                            Width="120" Format="dd-MM-yyyy HH:mm:ss" />
                        <ext:Column Header="Usuario de Creación" DataIndex="UsuarioCreacionAlertamiento" Width="120" />
                        <ext:DateColumn Header="Fecha de Modificación" Sortable="true" DataIndex="FechaModificacionAlertamiento"
                            Width="120" Format="dd-MM-yyyy HH:mm:ss" />
                        <ext:Column Header="Usuario de Modificación" DataIndex="UsuarioModificacionAlertamiento" Width="140" />
                        <ext:Column Header="Estado de Alertamiento" DataIndex="EstadoAlertamiento" Width="150" />
                        <ext:Column Header="Comentarios" DataIndex="ComentariosAlertamiento" Width="350" />
                        <ext:Column Hidden="true" DataIndex="ID_AplicativoFirma" />
                    </Columns>
                </ColumnModel>
                <View>
                    <ext:GridView runat="server">
                        <GetRowClass Fn="getRowClass" />
                    </ext:GridView>
                </View>
                <Listeners>
                    <Command Handler="#{hdnIdAlerta}.setValue(record.data.ID_Alertamiento); #{FormPanelComentarios}.reset();
                        #{WdwComentariosCierre}.show();" />
                </Listeners>
                <SelectionModel>
                    <ext:RowSelectionModel SingleSelect="true" />
                </SelectionModel>
                <Bottombar>
                    <ext:PagingToolbar ID="PagingAlertamientos" runat="server" StoreID="StoreAlertas" DisplayInfo="true"
                        DisplayMsg="Alertamientos {0} - {1} de {2}" PageSize="20" HideRefresh="true" />
                </Bottombar>
            </ext:GridPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
