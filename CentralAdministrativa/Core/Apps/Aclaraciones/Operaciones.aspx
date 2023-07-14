<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Operaciones.aspx.cs" Inherits="Aclaraciones.Operaciones" %>

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
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Window ID="frmEnvioAcl" Title="Enviar Aclaración" Icon="ScriptGo"
        runat="server" Width="460" Height="320" Resizable="False" Hidden="true" Closable="true"
        Modal="true" Layout="FitLayout" Draggable="true" Padding="12">
        <Items>
            <ext:FormPanel ID="PanelEnvio" runat="server" Layout="Fit" LabelAlign="Left" >
                <Content>
                    <ext:Store ID="StoreReasonCode" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="Descripcion">
                                <Fields>
                                    <ext:RecordField Name="ID_ReasonCode" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:Store ID="StoreDI" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="Descripcion">
                                <Fields>
                                    <ext:RecordField Name="ID_DocumentationIndicator" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Content>
                <Items>
                    <ext:FieldSet ID="FieldSetEnvio" runat="server" Title="Datos para Envío" Layout="Fit" LabelAlign="Left">
                        <Items>
                            <ext:ComboBox ID="ComboBoxRC" runat="server" FieldLabel="Reason Code" ForceSelection="true"
                                EmptyText="Selecciona una Opción..." StoreID="StoreReasonCode" MsgTarget="Side"
                                DisplayField="Descripcion" ValueField="ID_ReasonCode" Editable="false" />
                            <ext:ComboBox ID="ComboBoxDI" runat="server" FieldLabel="Document Indicator" ForceSelection="true"
                                EmptyText="Selecciona una Opción..." StoreID="StoreDI" MsgTarget="Side" DisplayField="Descripcion"
                                ValueField="ID_DocumentationIndicator" Editable="false" Width="398" />
                            <ext:NumberField ID="nfImporte" runat="server" FieldLabel="Importe" Name="ImporteACL" MaxLength="12"
                                MsgTarget="Side" AllowNegative="False" Width="398" />
                            <ext:Label ID="lblObservaciones" runat="server" Text="Observaciones" Width="100" />
                            <ext:Label ID="Label1" runat="server" Text="                       " Width="100"/>
                            <ext:TextArea ID="txtObservaciones" runat="server" BoxLabel="CheckBox" Width="398" MaxLengthText="500"/>
                        </Items>
                        <Buttons>
                            <ext:Button ID="btnEnviarAclr" runat="server" Text="Enviar" Icon="ScriptGo">
                                <DirectEvents>
                                    <Click OnEvent="btnEnviarAclr_Click" Before="var valid= #{PanelEnvio}.getForm().isValid(); if (!valid) {} return valid;" />
                                </DirectEvents>
                            </ext:Button>
                        </Buttons>
                    </ext:FieldSet>
                </Items>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:Window ID="frmHistorial" Title="Historial de Envíos a Contracargo" Icon="ScriptGo"
        runat="server" Width="350" Height="350" Resizable="False" Hidden="true" Closable="true"
        Modal="true" Layout="FitLayout" Draggable="true" Padding="12">
        <Items>
            <ext:Panel ID="PanelHistorial" runat="server" Layout="Fit">
                <Content>
                    <ext:Store ID="Store2" runat="server" OnRefreshData="RefreshGrid">
                        <Reader>
                            <ext:JsonReader IDProperty="IdArchivo">
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Content>
                <Items>
                    <ext:GridPanel ID="GridPanel2" runat="server" StoreID="Store2" StripeRows="false" 
                        Header="false" Border="false">
                        <LoadMask ShowMask="false" />
                    </ext:GridPanel>
                </Items>
            </ext:Panel>
        </Items>
    </ext:Window>
    <ext:BorderLayout ID="BorderLayout1" runat="server">    
        <West Split="true">
            <ext:FormPanel ID="frmBusqueda" Width="400" Height="295" Frame="true" runat="server" Border="false" 
                Title="Aclaraciones" Collapsible="true">
                <Content>
                    <ext:Store ID="StoreGpoTarjeta" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="Descripcion">
                                <Fields>
                                    <ext:RecordField Name="ID_GrupoMA" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Content>
                <Items>
                    <ext:FieldSet ID="FieldSet1" runat="server" Title="Buscar Operación" DefaultWidth="300" LabelAlign="Right">
                        <Items>
                            <ext:NumberField ID="txtTarjeta" runat="server" FieldLabel="Tarjeta" MsgTarget="Side" MaxLength="16"
                                AutoFocus="True" AllowDecimals="False" AllowNegative="False" AllowBlank="false" />
                            <ext:NumberField ID="txtImporte" runat="server" FieldLabel="Importe" Name="Importe" MaxLength="12"
                                AnchorHorizontal="90%" MsgTarget="Side" AllowNegative="False" AllowBlank="false" />
<%--                            <ext:Label
                                ID="Label1" 
                                runat="server" 
                                Text="Periodo" />--%>
                            <ext:DateField ID="datFechaInicial" runat="server" FieldLabel="Desde" Format="yyyy-MM-dd"
                                Name="FechaInicial" AnchorHorizontal="90%" MsgTarget="Side" EmptyText="Selecciona Fecha"
                                AllowBlank="false" Vtype="daterange">
                                <CustomConfig>
                                    <ext:ConfigItem Name="endDateField" Value="#{datFechaFinal}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:DateField ID="datFechaFinal" runat="server" FieldLabel="Hasta" Format="yyyy-MM-dd"
                                Name="FechaFinal" AnchorHorizontal="90%" MsgTarget="Side" EmptyText="Selecciona Fecha"
                                AllowBlank="false" Vtype="daterange">
                                <CustomConfig>
                                    <ext:ConfigItem Name="startDateField" Value="#{datFechaInicial}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:ComboBox ID="cmbGpoTarjeta" runat="server" FieldLabel="Grupo de Tarjeta" ForceSelection="true"
                                EmptyText="Selecciona una Opción..." StoreID="StoreGpoTarjeta" MsgTarget="Side"
                                DisplayField="Descripcion" ValueField="ID_GrupoMA" Editable="false" AnchorHorizontal="90%"
                                AllowBlank="false" />
                        </Items>
                        <Buttons>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Before="var valid= #{frmBusqueda}.getForm().isValid(); if (!valid) {} return valid;">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnLimpiar" runat="server" Text="Nueva Búsqueda"  Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscarNuevo_Click">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                        </Buttons>
                    </ext:FieldSet>
                </Items>
            </ext:FormPanel>
        </West>
        <Center Split="true">
            <ext:FormPanel ID="frmGridBusqueda" runat="server" Width="428.5" Collapsible="true" Title="Operación"
                Collapsed="false" Layout="Fit" AutoScroll="true" LabelAlign="Left" ForceLayout="False">
                <Content>
                    <ext:Store ID="StoreBusqueda" runat="server" OnRefreshData="RefreshGrid">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Operacion">
                                <Fields>
                                    <ext:RecordField Name="Importe" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Content>
                <Items>
                    <ext:GridPanel ID="GridBusqueda" runat="server" StoreID="StoreBusqueda" StripeRows="true"
                        Header="false" Border="false">
                        <LoadMask ShowMask="false" />
                        <DirectEvents>
                            <Command OnEvent="EjecutarComando">
                                <Confirmation ConfirmRequest="true" Message="¿Estás Seguro que Deseas Ejecutar la Acción Seleccionada?" Title="Confirmación" />
                                <ExtraParams>
                                    <ext:Parameter Name="id" Value="record.data.ID" Mode="Raw" />
                                </ExtraParams>
                            </Command>
                        </DirectEvents>
                    </ext:GridPanel>
                </Items>
            </ext:FormPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
