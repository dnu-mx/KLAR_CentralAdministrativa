<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ConsultaLogsAuditoria.aspx.cs" Inherits="Administracion.ConsultaLogsAuditoria" %>

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
    <ext:Hidden ID="hdnPantallaAspx" runat="server" />
    <ext:Hidden ID="hdnApplicationID" runat="server" />
    <ext:Hidden ID="hdnIdParametro" runat="server" />
    <ext:Hidden ID="hdnOrigen" runat="server" />

    <ext:Store ID="StoreAplicaciones" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ApplicationId">
                <Fields>
                    <ext:RecordField Name="ApplicationId" />
                    <ext:RecordField Name="Description" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Store ID="StorePantallas" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="PaginaAspx">
                <Fields>
                    <ext:RecordField Name="PaginaAspx" />
                    <ext:RecordField Name="NombrePantalla" />
                    <ext:RecordField Name="Application_ID" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
                               
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true">
            <ext:Panel runat="server" Width="325" Border="false" Layout="FitLayout" Title="Consulta" Collapsible="true">
                <Content>
                    <ext:BorderLayout ID="LeftBorderLayout" runat="server">
                        <South Split="true">
                            <ext:FormPanel ID="FormPanel3" runat="server" Height="25" Border="false">
                                <Items>
                                    <ext:Toolbar runat="server">
                                        <Items>
                                            <ext:Button ID="btnLimpiarIzq" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                                <DirectEvents>
                                                    <Click OnEvent="btnLimpiarIzq_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                            
                                        </Items>
                                    </ext:Toolbar>
                                </Items>
                            </ext:FormPanel>
                        </South>
                        <Center Split="true">
                            <ext:GridPanel ID="GridResultados" runat="server" AutoExpandColumn="NombrePantalla"
                                StoreID="StorePantallas" Border="false" Header="false">
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar2" runat="server">
                                        <Items>
                                            <ext:ComboBox ID="cBoxAplicaciones" runat="server" EmptyText="( Todas )" Width="120" 
                                                StoreID="StoreAplicaciones" DisplayField="Description" ValueField="ApplicationId"
                                                ListWidth="150">
                                                <Items>
                                                    <ext:ListItem Text="( Todas )" Value="" />
                                                </Items>
                                            </ext:ComboBox>
                                            <ext:TextField ID="txtPantalla" runat="server" EmptyText="Nombre Pantalla" Width="130" />
                                            <ext:Button ID="btnBuscarPantalla" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnBuscar_Click" Before="#{PanelCentral}.setTitle('');
                                                        #{PanelCentral}.setDisabled(true);">
                                                        <EventMask ShowMask="true" Msg="Buscando Aplicaciones..." MinDelay="500" />
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <ColumnModel runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="PaginaAspx" Hidden="true" />
                                        <ext:Column DataIndex="NombrePantalla" Header="Pantalla" Width="90" />
                                        <ext:Column DataIndex="Application_ID" Header="Application_ID" Hidden="true" Width="90" />
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel SingleSelect="true" />
                                </SelectionModel>
                                <DirectEvents>
                                    <RowClick OnEvent="selectRowResultados_Event">
                                        <EventMask ShowMask="true" Msg="Obteniendo Logs de la Pantalla..." MinDelay="500" />
                                        <ExtraParams>
                                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridResultados}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                        </ExtraParams>
                                    </RowClick>
                                </DirectEvents>
                                <BottomBar>
                                    <ext:PagingToolbar runat="server" StoreID="StorePantallas" DisplayInfo="true" HideRefresh="true"
                                        DisplayMsg="{0} - {1} de {2}" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </West>
        <Center Split="true">
            <ext:Panel ID="PanelCentral" runat="server" Height="250" Border="false" Title="-o-" Disabled="true">
                <Items>
                    <ext:BorderLayout runat="server">
                        <Center>
                            <ext:TabPanel runat="server">
                                <Items>
                                    <ext:FormPanel ID="FormPanelLogs" runat="server" Title="Logs" Layout="FitLayout" Border="false">
                                        <Items>
                                            <ext:Panel runat="server" Layout="FitLayout" AutoScroll="true" Border="false">
                                                <TopBar>
                                                    <ext:Toolbar runat="server">
                                                        <Items>
                                                            <ext:ComboBox ID="cBoxUsuarios" runat="server" EmptyText="( Todos )" Width="150"
                                                                DisplayField="UserName" ValueField="UserName" AllowBlank="true" ListWidth="250">
                                                                <Items>
                                                                    <ext:ListItem Text="( Todos )" Value="" />
                                                                </Items>
                                                                <Store>
                                                                    <ext:Store ID="StoreUsuario" runat="server">
                                                                        <Reader>
                                                                            <ext:JsonReader IDProperty="UserName">
                                                                                <Fields>
                                                                                    <ext:RecordField Name="UserName" />
                                                                                </Fields>
                                                                            </ext:JsonReader>
                                                                        </Reader>
                                                                    </ext:Store>
                                                                </Store>
                                                            </ext:ComboBox>
                                                            <ext:ComboBox ID="cBoxAccion" runat="server" EmptyText="( Todos )" Width="150"
                                                                DisplayField="Accion" ValueField="NombreStoreProcedure" AllowBlank="true"
                                                                ListWidth="250">
                                                                <Items>
                                                                    <ext:ListItem Text="( Todos )" Value="" />
                                                                </Items>
                                                                <Store>
                                                                    <ext:Store ID="StoreAccion" runat="server">
                                                                        <Reader>
                                                                            <ext:JsonReader IDProperty="Accion">
                                                                                <Fields>
                                                                                    <ext:RecordField Name="Accion" />
                                                                                    <ext:RecordField Name="NombreStoreProcedure" />
                                                                                </Fields>
                                                                            </ext:JsonReader>
                                                                        </Reader>
                                                                    </ext:Store>
                                                                </Store>
                                                            </ext:ComboBox>                                                            
                                                            <ext:DateField ID="datInicio" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                                                AllowBlank="false" MsgTarget="Side" Format="yyyy-MM-dd" MaxLength="12"
                                                                Width="200" EnableKeyEvents="true" LabelAlign="Right" >
                                                                <CustomConfig>
                                                                    <ext:ConfigItem Name="endDateField" Value="#{datFinal}" Mode="Value" />
                                                                </CustomConfig>
                                                                <Listeners>
                                                                    <KeyUp Fn="onKeyUp" />
                                                                </Listeners>
                                                            </ext:DateField>
                                                            <ext:DateField ID="datFinal" runat="server" Vtype="daterange" FieldLabel="Fecha Final"
                                                                AllowBlank="false" MaxLength="12" Width="200" MsgTarget="Side" Format="yyyy-MM-dd"
                                                                EnableKeyEvents="true" LabelAlign="Right">
                                                                <CustomConfig>
                                                                    <ext:ConfigItem Name="startDateField" Value="#{datInicio}" Mode="Value" />
                                                                </CustomConfig>
                                                                <Listeners>
                                                                    <KeyUp Fn="onKeyUp" />
                                                                </Listeners>
                                                            </ext:DateField>
                                                            <ext:ToolbarFill runat="server" />
                                                            <ext:ToolbarSeparator runat="server" />
                                                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." ToolTip="Buscar Logs de Filtros Seleccionados..."
                                                                Icon="Magnifier">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnBuscarLogs_Click" Before="
                                                                            var valida= #{datInicio}.isValid();                                                                        
                                                                            if (!valida) 
                                                                                return false;
                                                                             
                                                                            valida = #{datFinal}.isValid();
                                                                            if (!valida) 
                                                                                return false;
                                                                             
                                                                            return true;">
                                                                        <EventMask ShowMask="true" Msg="Buscando Logs..." MinDelay="500" />
                                                                    </Click>
                                                                </DirectEvents>
                                                            </ext:Button>
                                                        </Items>
                                                    </ext:Toolbar>
                                                </TopBar>
                                                <Items>
                                                    <ext:GridPanel ID="GridPanelLogs" runat="server" Header="true" Border="false" AutoScroll="true"
                                                        AutoHeight="true" Layout="FitLayout" Hidden="false">
                                                        <Store>
                                                            <ext:Store ID="StoreBitacoraLogs" runat="server" AutoLoad="false">
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="ID_BitacoraDetalle">
                                                                        <Fields>
                                                                            <ext:RecordField Name="ID_BitacoraDetalle" />
                                                                            <ext:RecordField Name="Tabla" />
                                                                            <ext:RecordField Name="Campo" />
                                                                            <ext:RecordField Name="ID_Registro" />
                                                                            <ext:RecordField Name="NuevoValor" />
                                                                            <ext:RecordField Name="UsuarioEjecutor" />
                                                                            <ext:RecordField Name="FechaHora" />
                                                                            <ext:RecordField Name="Observaciones" />
                                                                            <ext:RecordField Name="Accion" />
                                                                        </Fields>
                                                                    </ext:JsonReader>
                                                                </Reader>
                                                            </ext:Store>
                                                        </Store>
                                                        <ColumnModel runat="server">
                                                            <Columns>
                                                                <ext:Column ColumnID="ID_BitacoraDetalle" runat="server" Hidden="true" DataIndex="ID_BitacoraDetalle" />
                                                                <ext:Column ColumnID="Tabla" Header="Tabla" Sortable="true" DataIndex="Tabla" Width="100" />
                                                                <ext:Column ColumnID="Campo" Header="Campo" Sortable="true" DataIndex="Campo" Width="120" />
                                                                <ext:Column ColumnID="ID_Registro" Header="ID Registro" Sortable="true" DataIndex="ID_Registro" Width="90" />
                                                                <ext:Column ColumnID="NuevoValor" Header="Nuevo Valor" Sortable="false" DataIndex="NuevoValor" Width="120" />
                                                                <ext:Column ColumnID="Accion" Header="Acción" Sortable="false" DataIndex="Accion" Width="90" />
                                                                <ext:Column ColumnID="Observaciones" Header="Observaciones" Sortable="false" DataIndex="Observaciones" Width="250" />
                                                                <ext:DateColumn ColumnID="FechaHora" Header="Fecha/Hora" Sortable="true" DataIndex="FechaHora" Width="120" Format="yyyy-MM-dd HH:mm:ss" />
                                                                <ext:Column ColumnID="UsuarioEjecutor" Header="Usuario Ejecutor" Sortable="true" DataIndex="UsuarioEjecutor" Width="120" />
                                                            </Columns>
                                                        </ColumnModel>
                                                        <SelectionModel>
                                                            <ext:RowSelectionModel runat="server" SingleSelect="true" />
                                                        </SelectionModel>
                                                        <LoadMask ShowMask="false" />
                                                    </ext:GridPanel>
                                                </Items>
                                            </ext:Panel>
                                        </Items>
                                    </ext:FormPanel>
                                </Items>
                            </ext:TabPanel>
                        </Center>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
