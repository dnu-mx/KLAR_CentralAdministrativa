<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="Reporte_LlamadasTiendasDiconsa.aspx.cs" Inherits="CentroContacto.Reporte_LlamadasTiendasDiconsa" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        td.x-grid3-td-Obs {
            overflow: hidden;
        }

        td.x-grid3-td-Obs div.x-grid3-cell-inner {
            white-space: normal;
        }
    </style>

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
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true" Collapsible="true">
            <ext:FormPanel ID="FormPanelBusqueda" Width="300" runat="server" Title="Periodo" Border="false" Layout="FitLayout">
                <Content>
                    <ext:Hidden ID="FormatType" runat="server" />                    
                </Content>
                <Items>
                    <ext:FieldSet ID="FieldSetBusqueda" runat="server" Title="Indique el Periodo de Búsqueda" LabelAlign="Right">
                        <Items>
                            <ext:DateField ID="datInicio" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                AllowBlank="false" MsgTarget="Side" Format="yyyy-MM-dd" MaxLength="12" Width="250"
                                EnableKeyEvents="true">
                                <CustomConfig>
                                    <ext:ConfigItem Name="endDateField" Value="#{datFinal}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:DateField ID="datFinal" runat="server" Vtype="daterange" FieldLabel="Fecha Final" 
                                AllowBlank="false" MaxLength="12" Width="250" MsgTarget="Side" Format="yyyy-MM-dd" 
                                EnableKeyEvents="true">
                                <CustomConfig>
                                    <ext:ConfigItem Name="startDateField" Value="#{datInicio}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>                        
                        </Items>
                        <Buttons>
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Before="var valid= #{FormPanelBusqueda}.getForm().isValid(); if (!valid) {} return valid;" />
                                </DirectEvents>
                            </ext:Button>
                        </Buttons>
                    </ext:FieldSet>
                </Items>
            </ext:FormPanel>
        </West>
        <Center Split="true" Collapsible="false">
            <ext:Panel ID="Panel2" runat="server" Title="Llamadas Obtenidas en el Periodo Indicado"
                Collapsed="false" Layout="Fit" AutoScroll="true">
                <Content>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridResultados" runat="server" StripeRows="true" Header="false" Border="false"
                                AutoExpandColumn="Obs" AutoExpandMax="1000">
                                <LoadMask ShowMask="false" />
                                <Store>
                                    <ext:Store ID="StoreResultados" runat="server" OnSubmitData="StoreResultados_Submit" OnRefreshData="btnBuscar_Click" RemoteSort="true">
                                        <DirectEventConfig IsUpload="true"/>
                                        <Reader>
                                            <ext:JsonReader IDProperty="FechaHora">
                                                <Fields>
                                                    <ext:RecordField Name="NombreUsuario" />
                                                    <ext:RecordField Name="FechaHora" />
                                                    <ext:RecordField Name="TipoLlamada" />
                                                    <ext:RecordField Name="Observaciones" />
                                                    <ext:RecordField Name="Tienda" />
                                                    <ext:RecordField Name="Encargado" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                        <DirectEventConfig IsUpload="true" />
                                    </ext:Store>
                                </Store>
                                <ColumnModel ID="ColumnModel1" runat="server" >
                                    <Columns>
                                        <ext:Column DataIndex="NombreUsuario" Header="Usuario" Sortable="true" Width="100"/>
                                        <ext:DateColumn DataIndex="FechaHora" Header="Fecha" Sortable="true" Format="dd-MMM-yyyy" Width="80" />
                                        <ext:DateColumn DataIndex="FechaHora" Header="Hora" Sortable="true" Format="HH:mm:ss" Width="60" />
                                        <ext:Column DataIndex="TipoLlamada" Header="Tipo de Llamada" Sortable="true" Width="120" />
                                        <ext:Column ColumnID="Obs" DataIndex="Observaciones" Header="Observaciones" Sortable="true" Width="500" Wrap="True" />
                                        <ext:Column DataIndex="Tienda" Header="Tienda" Sortable="true" Width="140" />
                                        <ext:Column DataIndex="Encargado" Header="Nombre del Encargado" Sortable="true" Width="130" Wrap="True" />
                                    </Columns>
                                </ColumnModel>
                                <Plugins>
                                    <ext:GridFilters ID="GridFilters1" runat="server" Local="true">
                                        <Filters>
                                            <ext:StringFilter DataIndex="Tienda" />
                                        </Filters>
                                    </ext:GridFilters>
                                </Plugins>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreResultados" DisplayInfo="true"
                                        DisplayMsg="Mostrando Reporte {0} - {1} de {2}" />
                                </BottomBar>
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar2" runat="server">
                                        <Items>
                                            <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                                            <ext:Button ID="Button2" runat="server" Text="Exportar a XML" Icon="PageCode">
                                                <Listeners>
                                                    <Click Handler="submitValue(#{GridResultados}, #{FormatType}, 'xml');" />
                                                </Listeners>
                                            </ext:Button>
                                            <ext:Button ID="Button3" runat="server" Text="Exportar a Excel" Icon="PageExcel">
                                                <Listeners>
                                                    <Click Handler="submitValue(#{GridResultados}, #{FormatType}, 'xls');" />
                                                </Listeners>
                                            </ext:Button>
                                            <ext:Button ID="Button4" runat="server" Text="Exportar a CSV" Icon="PageAttach">
                                                <Listeners>
                                                    <Click Handler="submitValue(#{GridResultados}, #{FormatType}, 'csv');" />
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


