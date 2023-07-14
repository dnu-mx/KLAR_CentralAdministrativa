<%@ Page Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" 
    CodeBehind="ConsultaTarjetas_SmartTicketMC.aspx.cs" Inherits="CentroContacto.ConsultaTarjetas_SmartTicketMC" %>


<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">

        var submitValue = function (grid, hiddenFormat, format) {
            hiddenFormat.setValue(format);
            grid.submitData(false);
        };

        var afterEdit = function (e) {
            if (e.originalValue != e.value) {
                Ext.Msg.confirm('Confirmación', '¿Estás seguro que deseas Actualizar el Estatus de la Tarjeta?',
                    function (btn) {
                        if (btn == 'yes') {
                            CentroContacto.ActualizaEstatusTarjeta(e.record.data.IdTarjeta, e.value);
                        } else {
                            CentroContacto.RestableceEstatusTarjetas();
                        }
                    });
            }
        };
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">                                           
    <ext:BorderLayout ID="MainBorderLayout" runat="server">
        <West Split="true">
            <ext:Panel ID="Panel1" runat="server" Width="350" Border="false">
                <Content>
                    <ext:BorderLayout ID="LeftBorderLayout" runat="server">
                        <North Split="true">
                            <ext:FormPanel ID="FormPanelBusqueda" runat="server" Height="200" Frame="true" LabelWidth="120">
                                <Items>
                                    <ext:FieldSet ID="FieldSetBusqueda" runat="server" Title="Búsqueda de Clientes">
                                        <Items>
                                            <ext:Hidden ID="FormatType" runat="server" />
                                            <ext:Hidden ID="hdnIdCliente" runat="server" />
                                            <ext:TextField ID="txtNombre" runat="server" LabelAlign="Right" FieldLabel="Nombre" MaxLength="30" Width="300" />
                                            <ext:TextField ID="txtApPaterno" runat="server" LabelAlign="Right" FieldLabel="Primer Apellido" MaxLength="30" Width="300" />
                                            <ext:TextField ID="txtApMaterno" runat="server" LabelAlign="Right" FieldLabel="Segundo Apellido" MaxLength="30" Width="300" />
                                            <ext:TextField ID="txtCorreo" runat="server" LabelAlign="Right" FieldLabel="Correo Electrónico" MaxLength="60" Width="300" />
                                        </Items>
                                        <Buttons>
                                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                                <DirectEvents>
                                                    <Click OnEvent="btnLimpiar_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnBuscar_Click" Before="var valid= #{FormPanelBusqueda}.getForm().isValid(); if (!valid) {} return valid;">
                                                        <EventMask ShowMask="true" Msg="Buscando Clientes..." MinDelay="500" />
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Buttons>
                                    </ext:FieldSet>
                                </Items>
                            </ext:FormPanel>
                        </North>
                        <Center Split="true">
                            <ext:FormPanel ID="FormPanelResultados" runat="server" Title="Resultados Clientes" Layout="FitLayout">
                                <Items>
                                    <ext:GridPanel ID="GridResultados" runat="server" AutoExpandColumn="Nombre" Height="750" AutoDoLayout="true">
                                        <Store>
                                            <ext:Store ID="StoreClientes" runat="server" OnRefreshData="StoreClientes_Refresh">
                                                <Reader>
                                                    <ext:JsonReader IDProperty="IdCliente">
                                                        <Fields>
                                                            <ext:RecordField Name="IdCliente" />
                                                            <ext:RecordField Name="Nombre" />
                                                            <ext:RecordField Name="Email" />
                                                            <ext:RecordField Name="Empresa" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Store>
                                        <ColumnModel ID="ColumnModel1" runat="server">
                                            <Columns>
                                                <ext:Column DataIndex="IdCliente" Header="IdCliente" Width="60" />
                                                <ext:Column DataIndex="Email" Header="Correo" />
                                                <ext:Column DataIndex="Empresa" Header="Empresa" />
                                                <ext:Column DataIndex="Nombre" Header="Nombre" Width="100" />
                                            </Columns>
                                        </ColumnModel>
                                        <SelectionModel>
                                            <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                                                <DirectEvents>
                                                    <RowSelect OnEvent="selectRowResultados_Event" Before="#{GridPanelTarjetas}.getStore().removeAll();">
                                                        <EventMask ShowMask="true" Msg="Obteniendo Tarjetas del Cliente..." MinDelay="500" />
                                                        <ExtraParams>
                                                            <ext:Parameter Name="IdCliente" Value="this.getSelected().id" Mode="Raw" />
                                                        </ExtraParams>
                                                    </RowSelect>
                                                </DirectEvents>
                                            </ext:RowSelectionModel>
                                        </SelectionModel>
                                        <BottomBar>
                                            <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreClientes" DisplayInfo="true"
                                                DisplayMsg="Clientes {0} - {1} de {2}" />
                                        </BottomBar>
                                    </ext:GridPanel>
                                </Items>
                            </ext:FormPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </West>
        <Center Split="true">
            <ext:FormPanel ID="FormPanelCentral" runat="server" Layout="FitLayout" Title="Tarjetas del Cliente">
                <Items>
                    <ext:GridPanel ID="GridPanelTarjetas" runat="server" StripeRows="true" Layout="FitLayout" Region="Center"
                        EnableViewState="true">
                        <Store>
                            <ext:Store ID="StoreTarjetas" runat="server" OnSubmitData="StoreSubmit">          
                                <DirectEventConfig IsUpload="true" />
                                <Reader>
                                    <ext:JsonReader IDProperty="IdTarjeta">
                                        <Fields>
                                            <ext:RecordField Name="IdTarjeta" />
                                            <ext:RecordField Name="NombreTitular" />
                                            <ext:RecordField Name="NumeroTarjeta" />
                                            <ext:RecordField Name="Bandera" />
                                            <ext:RecordField Name="FechaInsertado" />
                                            <ext:RecordField Name="FechaModificado" />
                                            <ext:RecordField Name="Estatus" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <ColumnModel ID="ColumnModel2" runat="server">
                            <Columns>
                                <ext:Column ColumnID="IdTarjeta" Hidden="true" DataIndex="IdTarjeta" />
                                <ext:Column ColumnID="NombreTitular" Header="Nombre del Titular" Sortable="true" 
                                    DataIndex="NombreTitular" Width="140" />
                                <ext:Column ColumnID="NumeroTarjeta" Header="Número de Tarjeta" Sortable="true" 
                                    DataIndex="NumeroTarjeta" Width="110" />
                                <ext:Column ColumnID="Bandera" Header="Bandera" Sortable="true" DataIndex="Bandera"
                                    Width="60" />
                                <ext:DateColumn ColumnID="FechaInsertado" Header="Fecha Insertado" Sortable="true"
                                    DataIndex="FechaInsertado" Format="dd/MM/yyyy" Width="90"/>
                                <ext:DateColumn ColumnID="FechaModificado" Header="Fecha Modificado" Sortable="true"
                                    DataIndex="FechaModificado" Format="dd/MM/yyyy" Width="100"/>
                                <ext:Column ColumnID="Estatus" Header="Estatus" Sortable="true" DataIndex="Estatus"
                                    Width="90">
                                    <Editor>
                                        <ext:ComboBox ID="cBoxEstatus" runat="server" Shadow="Drop" Mode="Local"
                                            TriggerAction="All" ForceSelection="true" DisplayField="Descripcion"
                                            ValueField="ID_EstatusTarjeta">
                                            <Store>
                                                <ext:Store ID="StoreEstatus" runat="server">
                                                    <Reader>
                                                        <ext:JsonReader IDProperty="ID_EstatusTarjeta">
                                                            <Fields>
                                                                <ext:RecordField Name="ID_EstatusTarjeta" />
                                                                <ext:RecordField Name="Clave" />
                                                                <ext:RecordField Name="Descripcion" />
                                                            </Fields>
                                                        </ext:JsonReader>
                                                    </Reader>
                                                </ext:Store>
                                            </Store>
                                            <Listeners>
                                                <Change StopEvent="true" />
                                            </Listeners>
                                        </ext:ComboBox>
                                    </Editor>
                                </ext:Column>
                            </Columns>
                        </ColumnModel>
                        <SelectionModel>
                            <ext:RowSelectionModel ID="RowSelectionModel2" runat="server" />
                        </SelectionModel>
                        <Listeners>                           
                            <AfterEdit Fn="afterEdit" />
                        </Listeners>
                        <TopBar>
                            <ext:Toolbar ID="Toolbar5" runat="server">
                                <Items>
                                    <ext:ToolbarFill ID="ToolbarFill6" runat="server" />
                                    <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="true">
                                        <DirectEvents>
                                            <Click OnEvent="ExportGridToExcel" IsUpload="true" Success="Ext.Net.DirectMethods.Download({IsUpload:true})"
                                                After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                    e.stopEvent(); 
                                                    CentroContacto.StopMask();">
                                                <ExtraParams>
                                                    <ext:Parameter Name="GridResult" Value="Ext.encode(#{GridPanelTarjetas}.getRowsValues({selectedOnly : false}))" Mode="Raw" />
                                                </ExtraParams>
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnExportCSV" runat="server" Text="Exportar a CSV" Icon="PageAttach" Disabled="true">
                                        <Listeners>
                                            <Click Handler="submitValue(#{GridPanelTarjetas}, #{FormatType}, 'csv');"  />
                                        </Listeners>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                        <BottomBar>
                            <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="StoreTarjetas" DisplayInfo="true"
                                DisplayMsg="Mostrando Tarjetas {0} - {1} de {2}" />
                        </BottomBar>
                    </ext:GridPanel>
                </Items>
            </ext:FormPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
