<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ParametrosOpenPay.aspx.cs" Inherits="Administracion.ParametrosOpenPay" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var fullName = function (value, metadata, record, rowIndex, colIndex, store) {
            return "<b>" + record.data.Nombre + "</b>";
        };

        var prepareToolbar = function (grid, toolbar, rowIndex, record) {
            if (record.get("EsEjecutor") == 1) {
                toolbar.items.get(1).hide();
                toolbar.items.get(2).hide();
            }
            else if (record.get("EsAutorizador") == 1) {
                toolbar.items.get(0).hide();
                if (record.get("ValorPorAutorizar").length == 0) {
                    toolbar.items.get(1).hide();
                    toolbar.items.get(2).hide();
                }
            }
            else {
                toolbar.items.get(0).hide();
                toolbar.items.get(1).hide();
                toolbar.items.get(2).hide();
            }
        }
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
    <ext:Store ID="StoreCatalogoPMA" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_ValorPreePMA">
                <Fields>
                    <ext:RecordField Name="ID_ValorPreePMA" />
                    <ext:RecordField Name="Clave" />
                    <ext:RecordField Name="Descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Hidden ID="hdnIdValorPMA" runat="server" />
    <ext:Hidden ID="hdnValorIniPMA" runat="server" />
    <ext:Hidden ID="hdnValorFinPMA" runat="server" />
    <ext:Window ID="WdwValorParametro" runat="server" Title="Asignar Valor" Width="420" AutoHeight="true" Hidden="true"
        Resizable="false">
        <Items>
            <ext:FormPanel ID="FormPanelValorParamTxt" runat="server" Padding="10" MonitorValid="true" LabelAlign="Left" LabelWidth="70">
                <Items>
                    <ext:TextField ID="txtParametro" runat="server" FieldLabel="Descripción" Width="300"
                        AllowBlank="false" Selectable="false" ReadOnly="true" />
                    <ext:TextField ID="txtValorParFloat" runat="server" FieldLabel="Valor" Width="300" MaxLength="50"
                        MaskRe="[0-9\.]" Hidden="true" Regex="^-?[0-9]*(\.[0-9]{1,4})?$">
                        <Listeners>
                            <Change Handler="var inicial = parseFloat(#{hdnValorIniPMA}.getValue());
                                var final = parseFloat(#{hdnValorFinPMA}.getValue());
                                var actual = parseFloat(this.getValue());
                                var _vmsg = 'El valor del parámetro debe estar entre ' + inicial + ' y ' + final;
                                if ((actual < inicial || actual > final)) {
                                    this.clear();
                                    Ext.MessageBox.show({
                                        icon: Ext.MessageBox.ERROR,
                                        title: 'Valor erróneo',
                                        msg: _vmsg,
                                        buttons: Ext.MessageBox.OK,
                                        });
                                    return false; }" />
                        </Listeners>
                    </ext:TextField>
                    <ext:TextField ID="txtValorParInt" runat="server" FieldLabel="Valor" Width="300" MaxLength="50"
                        MaskRe="[0-9]" Hidden="true">
                        <Listeners>
                            <Change Handler="var inicial = parseInt(#{hdnValorIniPMA}.getValue());
                                var final = parseInt(#{hdnValorFinPMA}.getValue());
                                var actual = parseInt(this.getValue());
                                var _vmsg = 'El valor del parámetro debe estar entre ' + inicial + ' y ' + final;
                                if (actual < inicial || actual > final) {
                                    this.clear();
                                    Ext.MessageBox.show({
                                        icon: Ext.MessageBox.ERROR,
                                        title: 'Valor erróneo',
                                        msg: _vmsg,
                                        buttons: Ext.MessageBox.OK,
                                        });
                                    return false; }" />
                        </Listeners>
                    </ext:TextField>
                    <ext:TextArea ID="txtValorParString" runat="server" FieldLabel="Valor" Width="300" AutoHeight="true"
                        MaxLength="5000" Hidden="true" />
                    <ext:ComboBox ID="cBoxValorPar" runat="server" FieldLabel="Valor" Width="300" Hidden="true">
                        <Items>
                            <ext:ListItem Text="Sí" Value="true" />
                            <ext:ListItem Text="No" Value="false" />
                        </Items>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cBoxCatalogoPMA" runat="server" FieldLabel="Valor" Width="300" Hidden="true"
                        StoreID="StoreCatalogoPMA" ValueField="ID_ValorPreePMA" DisplayField="Descripcion" />
                </Items>
                <Buttons>
                    <ext:Button runat="server" Text="Cancelar" Icon="Cancel">
                        <Listeners>
                            <Click Handler="#{WdwValorParametro}.hide();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button runat="server" Text="Guardar Cambio" Icon="Disk">
                        <DirectEvents>
                            <Click OnEvent="btnGuardarValorParametro_Click" 
                                Before="if ((#{txtValorParFloat}.hidden == false) && (!#{txtValorParFloat}.isValid())) { return false; }" />
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:BorderLayout runat="server">
        <Center Split="true">
            <ext:GridPanel ID="GridPanelParametros" runat="server" StripeRows="true" Header="false" Border="false"
                Layout="FitLayout" AutoScroll="true">
                <Store>
                    <ext:Store ID="StoreValoresParametros" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_ParametroMultiasignacion">
                                <Fields>
                                    <ext:RecordField Name="ID_ParametroMultiasignacion" />
                                    <ext:RecordField Name="ID_Plantilla" />
                                    <ext:RecordField Name="ID_ValorParametroMultiasignacion" />
                                    <ext:RecordField Name="Nombre" />
                                    <ext:RecordField Name="Descripcion" />
                                    <ext:RecordField Name="ValorPorAutorizar" />
                                    <ext:RecordField Name="ValorAutorizado" />
                                    <ext:RecordField Name="TipoDato" />
                                    <ext:RecordField Name="EsAutorizador" />
                                    <ext:RecordField Name="EsEjecutor" />
                                    <ext:RecordField Name="TipoValidacion" />
                                    <ext:RecordField Name="ValorInicial" />
                                    <ext:RecordField Name="ValorFinal" />
                                    <ext:RecordField Name="ExpresionRegular" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <TopBar>
                    <ext:Toolbar runat="server">
                        <Items>
                            <ext:ComboBox ID="cBoxCliente" runat="server" EmptyText="SubEmisor" ListWidth="200"
                                Width="150" DisplayField="NombreORazonSocial" Mode="Local" ValueField="ID_Colectiva"
                                AutoSelect="true" Editable="true" ForceSelection="true" MinChars="1" TypeAhead="true"
                                MatchFieldWidth="false" Name="colSubEmisor" AllowBlank="false">
                                <DirectEvents>
                                    <Select OnEvent="PrestableceProductos" Before="#{cBoxProducto}.clearValue();" />
                                </DirectEvents>
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
                            <ext:ToolbarSpacer runat="server" Width="15" />
                            <ext:ComboBox ID="cBoxProducto" runat="server" EmptyText="Producto" ListWidth="200"
                                Width="150" DisplayField="Descripcion" ValueField="ID_Producto" AllowBlank="false">
                                <Store>
                                    <ext:Store ID="StoreProductos" runat="server">
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
                            <ext:ToolbarSpacer runat="server" Width="15" />
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Before="if (!#{cBoxCliente}.isValid() ||
                                                !#{cBoxProducto}.isValid()) { return false; } else { 
                                                #{GridPanelParametros}.getStore().removeAll(); return true; }">
                                        <EventMask ShowMask="true" Msg="Buscando Parámetros..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:ToolbarSeparator runat="server" />
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <ColumnModel runat="server">
                    <Columns>
                        <ext:Column runat="server" Hidden="true" DataIndex="ID_ParametroMultiasignacion" />
                        <ext:Column runat="server" Hidden="true" DataIndex="ID_Plantilla" />
                        <ext:Column runat="server" Hidden="true" DataIndex="ID_ValorParametroMultiasignacion" />
                        <ext:Column Header="Parámetro" Width="370" DataIndex="Nombre">
                            <Renderer Fn="fullName" />
                            <Editor>
                                <ext:DisplayField runat="server" StyleSpec="font-weight:bold;font-family:segoe ui;font-size: 12px;" />
                            </Editor>
                        </ext:Column>
                        <ext:Column Header="Valor por Autorizar" Sortable="true" DataIndex="ValorPorAutorizar" Width="230" />
                        <ext:Column Header="Valor Autorizado" Sortable="true" DataIndex="ValorAutorizado" Width="230" />
                        <ext:Column runat="server" Hidden="true" DataIndex="EsAutorizador" />
                        <ext:Column runat="server" Hidden="true" DataIndex="EsEjecutor" />
                        <ext:Column runat="server" Hidden="true" DataIndex="TipoDato" />
                        <ext:CommandColumn Width="80">
                            <PrepareToolbar Fn="prepareToolbar" />
                            <Commands>
                                <ext:GridCommand Icon="Disk" CommandName="Edit">
                                    <ToolTip Text="Guardar Valor" />
                                </ext:GridCommand>
                                <ext:GridCommand Icon="Tick" CommandName="Accept">
                                    <ToolTip Text="Autorizar Valor" />
                                </ext:GridCommand>
                                <ext:GridCommand Icon="Cross" CommandName="Reject">
                                    <ToolTip Text="Rechazar Valor por Autorizar" />
                                </ext:GridCommand>
                            </Commands>
                        </ext:CommandColumn>
                    </Columns>
                </ColumnModel>
                <View>
                    <ext:GridView runat="server" EnableRowBody="true">
                        <GetRowClass Handler="rowParams.body = '<p>' + record.data.Descripcion + '</p>'; return 'x-grid3-row-expanded';" />
                    </ext:GridView>
                </View>
                <SelectionModel>
                    <ext:RowSelectionModel runat="server" SingleSelect="true" />
                </SelectionModel>
                <DirectEvents>
                    <Command OnEvent="EjecutarComandoParametros">
                        <Confirmation BeforeConfirm="if (command == 'Edit') return false;"
                            ConfirmRequest="true" Title="Confirmación" Message="¿Estás seguro de autorizar/rechazar el cambio al valor del Parámetro?" />
                        <ExtraParams>
                            <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                            <ext:Parameter Name="Values" Value="Ext.encode(record.data)" Mode="Raw" />
                        </ExtraParams>
                    </Command>
                </DirectEvents>
                <LoadMask ShowMask="false" />
            </ext:GridPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
