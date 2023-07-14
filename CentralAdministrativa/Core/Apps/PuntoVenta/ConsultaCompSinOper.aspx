<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="ConsultaCompSinOper.aspx.cs" Inherits="TpvWeb.ConsultaCompSinOper" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var prepareToolbar = function (grid, toolbar, rowIndex, record) {
            if (record.get("ID_EstatusRelacionCSO") != 1) {
                toolbar.items.get(0).hide();
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
    <ext:Hidden ID="hdnIdRelacionCSO" runat="server" />
    <ext:Hidden ID="hdnIdOpRelacionada" runat="server" />
    <ext:Hidden ID="hdnCardMsk" runat="server" />
    <ext:Window ID="WdwMotivoCancelacion" runat="server" Title="Cancelar Relación" Width="450" Height="230"
        Hidden="true" Modal="true" Resizable="false">
        <Items>
            <ext:FormPanel ID="FormPanelMotivo" runat="server" Padding="10" LabelWidth="5" Border="false" Layout="FormLayout">
                <Items>
                    <ext:Label ID="lblMotivo" runat="server" LabelAlign="Top" Width="350" AutoHeight="true"
                        Text="Por favor, Ingresa el Motivo de la Cancelación:" />
                    <ext:Panel runat="server" Layout="FitLayout" Height="10" Border="false" />
                    <ext:TextArea ID="txtMotivo" runat="server" LabelAlign="Top" AllowBlank="false" Width="400" 
                        MaxLength="250" Height="100"/>
                </Items>
                <Buttons>
                    <ext:Button ID="btnOkMotivo" runat="server" Text="Aceptar" Icon="Tick">
                        <DirectEvents>
                            <Click OnEvent="btnOkMotivo_Click" Before="var valid= #{FormPanelMotivo}.getForm().isValid(); if (!valid) {} return valid;">
                                <EventMask ShowMask="true" Msg="Cancelando Relación..." MinDelay="500" />
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                    <ext:Button runat="server" Text="Cancelar" Icon="Cross">
                        <Listeners>
                            <Click Handler="#{WdwMotivoCancelacion}.hide();" />
                        </Listeners>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:BorderLayout ID="MainBorderLayout" runat="server">
        <Center Split="true">
            <ext:GridPanel ID="GridConsultaRelCSO" runat="server" StripeRows="true" Header="false" Border="false" Layout="FitLayout"
                AutoScroll="true">
                <Store>
                    <ext:Store ID="StoreConsultaRelCSO" runat="server" RemoteSort="true" AutoLoad="false" OnRefreshData="StoreConsultaRelCSO_RefreshData">
                        <AutoLoadParams>
                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                        </AutoLoadParams>
                        <Proxy>
                            <ext:PageProxy />
                        </Proxy>
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="ID_RelacionCSO">
                                <Fields>
                                    <ext:RecordField Name="ID_RelacionCSO" />
                                    <ext:RecordField Name="ID_EstatusRelacionCSO" />
                                    <ext:RecordField Name="ID_Operacion" />
                                    <ext:RecordField Name="Estatus" />
                                    <ext:RecordField Name="Motivo" />
                                    <ext:RecordField Name="CodigoProceso" />
                                    <ext:RecordField Name="Tarjeta" />
                                    <ext:RecordField Name="ClaveMA" />
                                    <ext:RecordField Name="Autorizacion" />
                                    <ext:RecordField Name="Referencia" />
                                    <ext:RecordField Name="Referencia2" />
                                    <ext:RecordField Name="FechaOperacion" />
                                    <ext:RecordField Name="ImporteOperacion" />
                                    <ext:RecordField Name="MonedaOperacion" />
                                    <ext:RecordField Name="ImporteCompensacion" />
                                    <ext:RecordField Name="MonedaCompensacion" />
                                    <ext:RecordField Name="Comercio" />
                                    <ext:RecordField Name="MCC" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <TopBar>
                    <ext:Toolbar runat="server">
                        <Items>
                            <ext:DateField ID="dfFechaIniPres" runat="server" Vtype="daterange" Format="dd/MM/yyyy" Editable="false"
                                EmptyText="Fecha Inicial Presentación" Width="160" AllowBlank="false">
                                <CustomConfig>
                                    <ext:ConfigItem Name="endDateField" Value="#{dfFechaFinPres}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:DateField ID="dfFechaFinPres" runat="server" Vtype="daterange" Format="dd/MM/yyyy" Editable="false"
                                EmptyText="Fecha Final Presentación" Width="160">
                                <CustomConfig>
                                    <ext:ConfigItem Name="startDateField" Value="#{dfFechaIniPres}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:TextField ID="txtNoTarjeta" runat="server" EmptyText="Número de Tarjeta" MaxLength="16"
                                MinLength="16" Width="150" MaskRe="[0-9]" />
                            <ext:TextField ID="txtRefer" runat="server" EmptyText="Referencia" MaxLength="25" Width="180"
                                MaskRe="[0-9]" />
                            <ext:ComboBox ID="cBoxEstatusRelCSO" EmptyText="Estatus" Width="120" runat="server" AllowBlank="false"
                                DisplayField="Descripcion" ListWidth="200" ValueField="ID_EstatusRelacionCSO">
                                <Store>
                                    <ext:Store ID="StoreEstatusRelCSO" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_EstatusRelacionCSO">
                                                <Fields>
                                                    <ext:RecordField Name="ID_EstatusRelacionCSO" />
                                                    <ext:RecordField Name="Clave" />
                                                    <ext:RecordField Name="Descripcion" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                                <Triggers>
                                    <ext:FieldTrigger Icon="Clear" Qtip="Borrar" />
                                </Triggers>
                                <Listeners>
                                    <TriggerClick Handler="this.removeByValue(this.getValue());this.clearValue();" />
                                </Listeners>
                            </ext:ComboBox>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                        Before="if (!#{dfFechaIniPres}.isValid() || !#{cBoxEstatusRelCSO}.isValid())
                                        { return false; } else { resetToolbar(#{PagingConsultaRelCSO});
                                        #{GridConsultaRelCSO}.getStore().sortInfo = null; }">
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarSeparator runat="server" />
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarSeparator runat="server" />
                            <ext:Button ID="btnBuscarHide" runat="server" Hidden="true">
                                <Listeners>
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Compensaciones...' });
                                        #{GridConsultaRelCSO}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                        RepActTraspasos.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <ColumnModel ID="ColumnModel1" runat="server">
                    <Columns>
                        <ext:CommandColumn Header="Acción" Width="50">
                            <PrepareToolbar Fn="prepareToolbar" />
                            <Commands>
                                <ext:GridCommand Icon="Cross" CommandName="Cancel">
                                    <ToolTip Text="Cancelar" />
                                </ext:GridCommand>
                            </Commands>
                        </ext:CommandColumn>
                        <ext:Column Header="ID_RelacionCSO" Hidden="true" />
                        <ext:Column Header="Estatus" Sortable="true" DataIndex="Estatus" Width="100" />
                        <ext:Column Header="Código de Proceso" Sortable="true" DataIndex="CodigoProceso" Width="120" />
                        <ext:Column Header="Tarjeta" Sortable="true" DataIndex="ClaveMA" Width="120" />
                        <ext:Column Header="Autorización" Sortable="true" DataIndex="Autorizacion" />
                        <ext:Column Header="Referencia" Sortable="true" DataIndex="Referencia" Width="150" />
                        <ext:Column Header="Referencia 2" Sortable="true" DataIndex="Referencia2" Width="150" />
                        <ext:DateColumn Header="Fecha Operación" Sortable="true" DataIndex="FechaOperacion" Format="yyyy-MM-dd" />
                        <ext:Column Header="Importe Operación" Sortable="true" DataIndex="ImporteOperacion" Align="Right" Width="115">
                            <Renderer Format="UsMoney" />
                        </ext:Column>
                        <ext:Column Header="Moneda Operación" Sortable="true" DataIndex="MonedaOperacion" Width="120" Align="Center" />
                        <ext:Column Header="Importe Compensación" Sortable="true" DataIndex="ImporteCompensacion" Align="Right" Width="135">
                            <Renderer Format="UsMoney" />
                        </ext:Column>
                        <ext:Column Header="Moneda Compensación" Sortable="true" DataIndex="MonedaCompensacion" Width="140" Align="Center" />
                        <ext:Column Header="Comercio" Sortable="true" DataIndex="Comercio" Width="200" />
                        <ext:Column Header="MCC" Sortable="true" DataIndex="MCC" />
                    </Columns>
                </ColumnModel>
                <SelectionModel>
                    <ext:RowSelectionModel SingleSelect="true" />
                </SelectionModel>
                <DirectEvents>
                    <RowClick OnEvent="selectRowResultados_Event">
                        <EventMask ShowMask="true" Target="CustomTarget" CustomTarget="#{GridOperacionRel}" />
                        <ExtraParams>
                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridConsultaRelCSO}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                        </ExtraParams>
                    </RowClick>
                    <Command OnEvent="EjecutarComando">
                        <Confirmation ConfirmRequest="true" Title="Confirmación de Cancelación" BeforeConfirm=""                    
                             Message="¿Estás seguro de Cancelar la Relación de Compensación sin Operación?" />
                    </Command>
                </DirectEvents>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingConsultaRelCSO" runat="server" StoreID="StoreConsultaRelCSO" DisplayInfo="true"
                        DisplayMsg="Mostrando Compensaciones {0} - {1} de {2}" HideRefresh="true" />
                </BottomBar>
            </ext:GridPanel>  
        </Center>
        <South Split="true">
            <ext:GridPanel ID="GridOperacionRel" runat="server" StripeRows="true" Height="200" Title="Detalle de la Operación" Border="false"
                Layout="FitLayout" Disabled="true" Collapsible="true" AutoExpandColumn="Comercio">
                <Store>
                    <ext:Store ID="StoreOperacionRel" runat="server">
                        <DirectEventConfig IsUpload="true" />
                        <Reader>
                            <ext:JsonReader IDProperty="IdOperacion">
                                <Fields>
                                    <ext:RecordField Name="IdOperacion" />
                                    <ext:RecordField Name="NumTarjeta" />
                                    <ext:RecordField Name="Autorizacion" />
                                    <ext:RecordField Name="FechaOperacion" />
                                    <ext:RecordField Name="ImporteOperacion" />
                                    <ext:RecordField Name="MonedaOperacion" />
                                    <ext:RecordField Name="Comercio" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <DirectEventConfig IsUpload="true" />
                    </ext:Store>
                </Store>
                <ColumnModel runat="server">
                    <Columns>
                        <ext:Column Header="IdOperacion" DataIndex="IdOperacion" Width="100" />
                        <ext:Column Header="Tarjeta" DataIndex="NumTarjeta" Width="150" />
                        <ext:Column Header="Autorización" DataIndex="Autorizacion" Width="100" />
                        <ext:DateColumn Header="Fecha Operación" DataIndex="FechaOperacion" Width="150" Format="yyyy-MM-dd HH:mm:ss" />
                        <ext:Column Header="Importe Operación" DataIndex="ImporteOperacion" Align="Right" Width="120">
                            <Renderer Format="UsMoney" />
                        </ext:Column>
                        <ext:Column Header="Moneda Operación" DataIndex="MonedaOperacion" Align="Center" Width="120" />
                        <ext:Column Header="Comercio" DataIndex="Comercio" Width="300" />
                    </Columns>
                </ColumnModel>
                <SelectionModel>
                    <ext:RowSelectionModel SingleSelect="true" />
                </SelectionModel>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingOperacionRel" runat="server" StoreID="StoreOperacionRel" DisplayInfo="true"
                        DisplayMsg="{0} - {1} de {2}" HideRefresh="true" />
                </BottomBar>
            </ext:GridPanel>
        </South>
    </ext:BorderLayout>
</asp:Content>
