<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" ValidateRequest="false"
    CodeBehind="ReporteConsolidadoTarjetas.aspx.cs" Inherits="Empresarial.ReporteConsolidadoTarjetas" %>

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

        function resetToolbar(tbar) {
            tbar.updateInfo();
            tbar.inputItem.setValue(1);
            tbar.afterTextItem.setText(String.format(tbar.afterPageText, 1));
            tbar.next.setDisabled(true);
            tbar.prev.setDisabled(true);
            tbar.first.setDisabled(true);
            tbar.last.setDisabled(true);
        }

        var getAttributes = function (disabled) {
            return "class='x-combo-list-item" + (disabled === "0" ? " disabled-item'" : "'");
        };
    </script>
    <style type="text/css">
        .disabled-item {
            color: gray;
            padding : 2px;
        }
        
        .disabled-item.x-combo-selected {
            border: 1px solid white !important;
            background-color:inherit;
            cursor: default;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true">
            <ext:FormPanel ID="FormPanelBusqueda" Width="350" Title="Selecciona los Filtros" runat="server" Padding="10" Layout="FormLayout" LabelWidth="150">
                <Items>
                    <ext:ComboBox ID="cBoxSubemisor" runat="server" FieldLabel="Subemisor   <span style='color:red;'>*   </span>"
                        Width="170" AllowBlank="false" DisplayField="NombreORazonSocial" ValueField="ID_Colectiva">
                        <Store>
                            <ext:Store ID="StoreSubemisor" runat="server">
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_Colectiva">
                                        <Fields>
                                            <ext:RecordField Name="ID_Colectiva" />
                                            <ext:RecordField Name="ClaveColectiva" />
                                            <ext:RecordField Name="NombreORazonSocial" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <DirectEvents>
                            <Select OnEvent="EstableceProductos" Before="#{cBoxProducto}.clearValue();">
                                <EventMask ShowMask="true" Msg="Estableciendo Productos..." MinDelay="200" />
                            </Select>
                        </DirectEvents>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cBoxProducto" runat="server" FieldLabel="Producto   <span style='color:red;'>*   </span>"
                        Width="170" AllowBlank="false" DisplayField="Descripcion" ValueField="ID_Producto">
                        <Store>
                            <ext:Store ID="StoreProducto" runat="server">
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_Producto">
                                        <Fields>
                                            <ext:RecordField Name="ID_Producto" />
                                            <ext:RecordField Name="Clave" />
                                            <ext:RecordField Name="Descripcion" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cBoxTipo" runat="server" FieldLabel="Tipo   <span style='color:red;'>*   </span>"
                        Width="170" AllowBlank="false">
                        <Items>
                            <ext:ListItem Text="Todos" Value="Todos" />
                            <ext:ListItem Text="Tarjetas" Value="Tarjetas" />
                            <ext:ListItem Text="Cuentas" Value="Cuentas" />
                        </Items>
                        <DirectEvents>
                            <Select OnEvent="EstableceEstatus" Before="#{cBoxEstatus}.clearValue();">
                                <EventMask ShowMask="true" Msg="Estableciendo Estatus..." MinDelay="200" />
                            </Select>
                        </DirectEvents>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cBoxEstatus" runat="server" FieldLabel="Estatus   <span style='color:red;'>*   </span>"
                        Width="170" AllowBlank="false" ValueField="Clave" DisplayField="Descripcion">
                        <Store>
                            <ext:Store ID="StoreEstatus" runat="server">
                                <Reader>
                                    <ext:ArrayReader>
                                        <Fields>
                                            <ext:RecordField Name="Clave" />
                                            <ext:RecordField Name="Descripcion" />
                                        </Fields>
                                    </ext:ArrayReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                    </ext:ComboBox>
                    <ext:DateField ID="dfFI_RepConsTarj" runat="server" Vtype="daterange" Format="dd/MM/yyyy" Width="170" 
                        FieldLabel="Periodo Inicial   <span style='color:red;'>*   </span>" EnableKeyEvents="true"
                        Editable="false" AllowBlank="false">
                        <CustomConfig>
                            <ext:ConfigItem Name="endDateField" Value="#{dfFF_RepConsTarj}" Mode="Value" />
                        </CustomConfig>
                        <Listeners>
                            <KeyUp Fn="onKeyUp" />
                        </Listeners>
                    </ext:DateField>
                    <ext:DateField ID="dfFF_RepConsTarj" runat="server" Vtype="daterange" Width="170" Format="dd/MM/yyyy"
                        FieldLabel="Periodo Final   <span style='color:red;'>*   </span>" EnableKeyEvents="true"
                        Editable="false" AllowBlank="false">
                        <CustomConfig>
                            <ext:ConfigItem Name="startDateField" Value="#{dfFI_RepConsTarj}" Mode="Value" />
                        </CustomConfig>
                        <Listeners>
                            <KeyUp Fn="onKeyUp" />
                        </Listeners>
                    </ext:DateField>
                    <ext:Panel runat="server" Layout="FitLayout" Height="20" Border="false" />
                    <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false" LabelWidth="5">
                        <Defaults>
                            <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                        </Defaults>
                        <LayoutConfig>
                            <ext:HBoxLayoutConfig Align="Top" />
                        </LayoutConfig>
                        <Items>
                            <ext:Label runat="server" FieldLabel="<span style='color:red;'>*   </span>"
                                Text="Obligatorios" LabelSeparator=" " StyleSpec="font-style: italic;font-family:segoe ui;font-size: 11px;" />
                        </Items>
                    </ext:Panel>
                </Items>
                <FooterBar>
                    <ext:Toolbar runat="server">
                        <Items>
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Before="var valid= #{FormPanelBusqueda}.getForm().isValid();
                                        if (!valid) {} else { resetToolbar(#{PagingConsTarj});
                                        #{GridConsTarjetas}.getStore().sortInfo = null; } return valid;" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>
            </ext:FormPanel>
        </West>
        <Center Split="true">
            <ext:GridPanel ID="GridConsTarjetas" runat="server" StripeRows="true" Header="false" Border="false"
                Layout="FitLayout" AutoScroll="true">
                <Store>
                    <ext:Store ID="StoreConsTarjetas" runat="server" RemoteSort="true" AutoLoad="false"
                        OnRefreshData="StoreConsTarjetas_RefreshData">
                        <AutoLoadParams>
                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                        </AutoLoadParams>
                        <Proxy>
                            <ext:PageProxy />
                        </Proxy>
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader>
                                <Fields>
                                    <ext:RecordField Name="Tipo" />
                                    <ext:RecordField Name="Estatus" />
                                    <ext:RecordField Name="Cantidad" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <TopBar>
                    <ext:Toolbar ID="Toolbar2" runat="server" LabelWidth="70" LabelAlign="Right">
                        <Items>
                            <ext:Button ID="btnBuscarHide" runat="server" Hidden="true">
                                <Listeners>
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Buscando Consolidados...' });
                                        #{GridConsTarjetas}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                        RepConsTarjCtas.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarFill runat="server" />
                            <ext:Button ID="btnExcelRepConsTarj" runat="server" Text="Exportar a  Excel" Icon="PageExcel" ToolTip="Obtener Datos en un Archivo Excel"
                                Disabled="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                        e.stopEvent(); 
                                        RepConsTarjCtas.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <ColumnModel ID="ColumnModel1" runat="server">
                    <Columns>
                        <ext:Column Header="Tipo" Sortable="true" DataIndex="Tipo" />
                        <ext:Column Header="Estatus" Sortable="true" DataIndex="Estatus" Width="150" />
                        <ext:Column Header="Cantidad" Sortable="true" DataIndex="Cantidad" />
                    </Columns>
                </ColumnModel>
                <SelectionModel>
                    <ext:RowSelectionModel runat="server" SingleSelect="true" />
                </SelectionModel>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingConsTarj" runat="server" StoreID="StoreConsTarjetas" DisplayInfo="true"
                        DisplayMsg="Mostrando Consolidados {0} - {1} de {2}" HideRefresh="true" />
                </BottomBar>
            </ext:GridPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
