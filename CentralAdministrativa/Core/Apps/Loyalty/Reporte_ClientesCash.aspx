<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" 
    CodeBehind="Reporte_ClientesCash.aspx.cs" Inherits="CentroContacto.Reporte_ClientesCash" %>

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
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true" Collapsible="true">
            <ext:FormPanel ID="FormPanel1" Width="320" Title="Selecciona los Filtros" runat="server"
                Border="false" Layout="FitLayout">
                <Content>
                    <ext:Hidden ID="FormatType" runat="server" />
                    <ext:Store ID="StoreNivelLealtad" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_GrupoCuenta">
                                <Fields>
                                    <ext:RecordField Name="ID_GrupoCuenta" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="Descripcion"  Direction="ASC"  />
                    </ext:Store>
                </Content>
                <Items>
                    <ext:Panel ID="Panel1" runat="server" Layout="FitLayout" Padding="10">
                        <Items>
                            <ext:DateField ID="datInicio" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                AllowBlank="false" MsgTarget="Side" Format="yyyy-MM-dd" MaxLength="12" Width="300"
                                EnableKeyEvents="true" MaxWidth="300">
                                <CustomConfig>
                                    <ext:ConfigItem Name="endDateField" Value="#{datFinal}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:DateField ID="datFinal" runat="server" Vtype="daterange" FieldLabel="Fecha Final"
                                AllowBlank="false" MsgTarget="Side" Format="yyyy-MM-dd" MaxLength="12" Width="300"
                                EnableKeyEvents="true">
                                <CustomConfig>
                                    <ext:ConfigItem Name="startDateField" Value="#{datInicio}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:NumberField ID="nfTelefono" runat="server" FieldLabel="Teléfono" MaxLength="10" Width="300"
                                AllowDecimals="false" AllowNegative="false" />
                            <ext:ComboBox ID="cBoxNivelLealtad" TabIndex="3" FieldLabel="Nivel de Lealtad" EmptyText="Todas"
                                Resizable="true" ListWidth="350" Width="300" runat="server" StoreID="StoreNivelLealtad"
                                DisplayField="Descripcion" ValueField="ID_GrupoCuenta">
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cBoxEstatusUsuario" FieldLabel="Estatus Usuario" EmptyText="Todos" DisplayField="Descripcion"
                                ValueField="ID_EstatusColectiva" Width="300" runat="server">
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                                <Store>
                                    <ext:Store ID="StoreEstatusColectiva" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_EstatusColectiva">
                                                <Fields>
                                                    <ext:RecordField Name="ID_EstatusColectiva" />
                                                    <ext:RecordField Name="Descripcion" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                            </ext:ComboBox>
                        </Items>
                    </ext:Panel>
                </Items>
                <FooterBar>
                    <ext:Toolbar ID="Toolbar1" runat="server">
                        <Items>
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnBuscarHide" runat="server" Hidden="true">
                                <Listeners>
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Clientes...' });
                                        #{GridPanelClientes}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000" Before="var valid= #{FormPanel1}.getForm().isValid();
                                        if (!valid) {} else { resetToolbar(#{PagingToolBar1});
                                        #{GridPanelClientes}.getStore().sortInfo = null; } return valid;">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                            RepClientesCash.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>
            </ext:FormPanel>
        </West>
        <Center Split="true" Collapsible="false">
            <ext:Panel ID="Panel2" runat="server" Title="Clientes Obtenidos con los Filtros Seleccionados"
                Collapsed="false" Layout="FitLayout" AutoScroll="true">
                <Content>
                    <ext:Store ID="StoreClientes" runat="server" OnSubmitData="StoreSubmit" RemoteSort="true"
                        OnRefreshData="StoreClientes_RefreshData" AutoLoad="false">
                        <AutoLoadParams>
                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                        </AutoLoadParams>
                        <Proxy>
                            <ext:PageProxy />
                        </Proxy>
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                                <Fields>
                                    <ext:RecordField Name="ID_Colectiva" />
                                    <ext:RecordField Name="Nombre" />
                                    <ext:RecordField Name="PrimerApellido" />
                                    <ext:RecordField Name="SegundoApellido" />
                                    <ext:RecordField Name="FechaNacimiento" />
                                    <ext:RecordField Name="Genero" />
                                    <ext:RecordField Name="Email" />
                                    <ext:RecordField Name="CodigoPostal" />
                                    <ext:RecordField Name="RFC" />
                                    <ext:RecordField Name="Telefono" />
                                    <ext:RecordField Name="NumTarjeta" />
                                    <ext:RecordField Name="FechaAlta" />
                                    <ext:RecordField Name="FechaConfirmacion" />
                                    <ext:RecordField Name="NivelLealtad" />
                                    <ext:RecordField Name="Puntos" />
                                    <ext:RecordField Name="EstatusColectiva" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanelClientes" runat="server" StoreID="StoreClientes" StripeRows="true"
                                Header="false" Border="false">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                        <ext:Column Header="ID Cliente" Sortable="true" DataIndex="ID_Colectiva" />
                                        <ext:Column Header="Nombre" Sortable="true" DataIndex="Nombre" />
                                        <ext:Column Header="Primer Apellido" Sortable="true" DataIndex="PrimerApellido" />
                                        <ext:Column Header="Segundo Apellido" Sortable="true" DataIndex="SegundoApellido" />
                                        <ext:DateColumn Header="Fecha de Nacimiento" Sortable="true" DataIndex="FechaNacimiento" 
                                            Format="yyyy-MM-dd" Width="150" />
                                        <ext:Column Header="Género" Sortable="true" DataIndex="Genero" />
                                        <ext:Column Header="Correo Electrónico" Sortable="true" DataIndex="Email" Width="200" />
                                        <ext:Column Header="Código Postal" Sortable="true" DataIndex="CodigoPostal" />
                                        <ext:Column Header="RFC" Sortable="true" DataIndex="RFC" />
                                        <ext:Column Header="Teléfono" Sortable="true" DataIndex="Telefono" />
                                        <ext:Column Header="No. Tarjeta" Sortable="true" DataIndex="NumTarjeta" Width="120" />
                                        <ext:DateColumn Header="Fecha de Alta" Sortable="true" DataIndex="FechaAlta" Format="yyyy-MM-dd" />
                                        <ext:DateColumn Header="Fecha de Confirmación" Sortable="true" DataIndex="FechaConfirmacion"
                                            Format="yyyy-MM-dd" Width="150" />
                                        <ext:Column Header="Nivel de Lealtad" Sortable="true" DataIndex="NivelLealtad" />
                                        <ext:Column Header="Puntos" Sortable="true" DataIndex="Puntos" />
                                        <ext:Column Header="Estatus Usuario" Sortable="true" DataIndex="EstatusColectiva" Width="200"/>
                                    </Columns>
                                </ColumnModel>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreClientes" DisplayInfo="true"
                                        DisplayMsg="Mostrando Reporte {0} - {1} de {2}" />
                                </BottomBar>
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar2" runat="server">
                                        <Items>
                                            <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                                            <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="true">
                                                <DirectEvents>
                                                    <Click OnEvent="Download" IsUpload="true"
                                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                            e.stopEvent(); 
                                                            RepClientesCash.StopMask();" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="btnExportCSV" runat="server" Text="Exportar a CSV" Icon="PageAttach" Disabled="true">
                                                <Listeners>
                                                    <Click Handler="submitValue(#{GridPanelClientes}, #{FormatType}, 'csv');" />
                                                </Listeners>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
