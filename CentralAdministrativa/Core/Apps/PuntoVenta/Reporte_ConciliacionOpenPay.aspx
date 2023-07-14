<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Reporte_ConciliacionOpenPay.aspx.cs" Inherits="TpvWeb.Reporte_ConciliacionOpenPay" %>


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
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">    
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center Split="true" Collapsible="false">
            <ext:GridPanel ID="GridPanelOperaciones" runat="server" StripeRows="true" Header="false" Border="false">
                <LoadMask ShowMask="false" />
                <Store>
                    <ext:Store ID="StoreOperaciones" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID">
                                <Fields>
                                    <%-- <ext:RecordField Name="ID_Operacion" />--%>
                                    <ext:RecordField Name="ID" />
                                    <ext:RecordField Name="IdPoliza" />
                                    <ext:RecordField Name="Fecha" />
                                    <ext:RecordField Name="NumeroReferencia" />
                                    <ext:RecordField Name="Cargo" />
                                    <ext:RecordField Name="Abono" />
                                    <ext:RecordField Name="SaldoAntes" />
                                    <ext:RecordField Name="SaldoDespues" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <TopBar>
                    <ext:Toolbar runat="server" LabelWidth="70" LabelAlign="Right">
                        <Items>
                            <ext:ComboBox ID="cBoxCliente" runat="server" EmptyText="Selecciona el SubEmisor..."
                                Width="180" DisplayField="NombreORazonSocial" ValueField="ID_Colectiva"
                                Mode="Local" AutoSelect="true" ForceSelection="true" TypeAhead="true" MinChars="1"
                                MatchFieldWidth="false" AllowBlank="false">
                                <Store>
                                    <ext:Store ID="StoreSubEmisores" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Colectiva">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Colectiva" />
                                                    <ext:RecordField Name="ClaveColectiva" />
                                                    <ext:RecordField Name="NombreORazonSocial" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                        <SortInfo Field="NombreORazonSocial" Direction="ASC" />
                                    </ext:Store>
                                </Store>
                            </ext:ComboBox>
                            <ext:DateField ID="dfFechaInicio" runat="server" Vtype="daterange" MaskRe="[0-9\/]" Width="180"
                                FieldLabel="Fecha Inicial" Format="yyyy/MM/dd" EnableKeyEvents="true" MaxDate="<%# DateTime.Now %>"
                                AutoDataBind="true" AllowBlank="false" InvalidText="Fecha inválida. Debe tener el formato AAAA/MM/DD">
                                <CustomConfig>
                                    <ext:ConfigItem Name="endDateField" Value="#{dfFechaFin}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                    <Change Handler="if (this.getValue() == '') { #{tfHoraInicio}.clear(); }" />
                                </Listeners>
                            </ext:DateField>
                            <ext:DateField ID="dfFechaFin" runat="server" Vtype="daterange" MaskRe="[0-9\/]" Width="180"
                                FieldLabel="Fecha Final" Format="yyyy/MM/dd" EnableKeyEvents="true" MaxDate="<%# DateTime.Now %>"
                                AutoDataBind="true" AllowBlank="false" InvalidText="Fecha inválida. Debe tener el formato AAAA/MM/DD">
                                <CustomConfig>
                                    <ext:ConfigItem Name="startDateField" Value="#{dfFechaInicio}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                    <Change Handler="if (this.getValue() == '') { #{tfHoraFin}.clear(); }" />
                                </Listeners>
                            </ext:DateField>
                            <ext:Button ID="btnBuscarHide" runat="server" Hidden="true">
                                <Listeners>
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Operaciones...' });
                                        #{GridPanelOperaciones}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                        Before="if (!#{cBoxCliente}.isValid() || !#{dfFechaInicio}.getValue() || !#{dfFechaFin}.getValue())
                                        { return false; } else { resetToolbar(#{PagingToolBar1});
                                        #{GridPanelOperaciones}.getStore().sortInfo = null; }">
                                         <EventMask ShowMask="true" Msg="Buscando Operaciones..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarFill runat="server" />
                            <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="true">
                                <DirectEvents>
                                    <Click OnEvent="DownloadOpenPay" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                            e.stopEvent(); 
                                            RepOpenPay.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <ColumnModel ID="ColumnModel1" runat="server">
                    <Columns>
                        <ext:Column Header="ID" DataIndex="ID" />
                        <ext:Column Header="Id Póliza" DataIndex="IdPoliza" />
                        <ext:DateColumn Header="Fecha" DataIndex="Fecha"
                            Format="yyyy-MM-dd HH:mm:ss" />
                        <ext:Column Header="Número de Referencia" DataIndex="NumeroReferencia"
                            Width="120" />
                        <ext:Column Header="Cargo" DataIndex="Cargo">
                            <Renderer Format="UsMoney" />
                        </ext:Column>
                        <ext:Column Header="Abono" DataIndex="Abono">
                            <Renderer Format="UsMoney" />
                        </ext:Column>
                        <ext:Column Header="Saldo antes de la Operación" DataIndex="SaldoAntes" Width="160" />
                        <ext:Column Header="Saldo después de la Operación" DataIndex="SaldoDespues" Width="180" />
                        <ext:Column Header="Descripción" DataIndex="Descripcion" Width="150" />
                    </Columns>
                </ColumnModel>
                <SelectionModel>
                    <ext:RowSelectionModel SingleSelect="true" />
                </SelectionModel>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreOperaciones" DisplayInfo="true"
                        DisplayMsg="Mostrando Operaciones {0} - {1} de {2}" HideRefresh="true" />
                </BottomBar>
            </ext:GridPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>