<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="AdminSubProductosParabilia.aspx.cs" Inherits="Administracion.AdminSubProductosParabilia" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function resetToolbar(tbar) {
            tbar.updateInfo();
            tbar.inputItem.setValue(1);
            tbar.afterTextItem.setText(String.format(tbar.afterPageText, 1));
            tbar.next.setDisabled(true);
            tbar.prev.setDisabled(true);
            tbar.first.setDisabled(true);
            tbar.last.setDisabled(true);
        }
        
        var fullName = function (value, metadata, record, rowIndex, colIndex, store) {
            return "<b>" + record.data.Nombre + "</b>";
        };

        var prepareTB_SubPParams = function (grid, toolbar, rowIndex, record) {
            if (record.get("esAutorizable") == 1) {
                toolbar.items.get(0).hide();
                toolbar.items.get(1).hide();
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
    <ext:Hidden ID="hdnValorIniPMA" runat="server" />
    <ext:Hidden ID="hdnValorFinPMA" runat="server" />
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
    <ext:Window ID="WdwValorParametro" runat="server" Title="Editar Valor Parámetro" Width="420" AutoHeight="true" Hidden="true"
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
                        ValueField="ID_ValorPreePMA" DisplayField="Descripcion">
                        <Store>
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
                        </Store>
                    </ext:ComboBox>
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
        <West Split="true">
            <ext:Panel runat="server" Width="335" Border="false" Layout="FitLayout" Title="Consulta de Suproductos">
                <Content>
                    <ext:BorderLayout runat="server">
                        <South Split="true">
                            <ext:FormPanel runat="server" Height="25" Border="false">
                                <Items>
                                    <ext:Toolbar runat="server">
                                        <Items>
                                            <ext:ToolbarFill runat="server" />
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
                            <ext:GridPanel ID="GridResultSubProdsParab" runat="server" AutoExpandColumn="Descripcion" 
                                Border="false" Header="false">
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar2" runat="server">
                                        <Items>
                                            <ext:Hidden ID="hdnIdColectiva" runat="server" />
                                            <ext:ComboBox ID="cBoxSubEmisor" runat="server" EmptyText="SubEmisor" ListWidth="200"
                                                Width="120" StoreID="StoreSubEmisores" DisplayField="NombreORazonSocial" Mode="Local"
                                                ValueField="ID_Colectiva" AutoSelect="true" Editable="true" ForceSelection="true" MinChars="1"
                                                TypeAhead="true" MatchFieldWidth="false" Name="colSubEmisor" AllowBlank="false">
                                                <DirectEvents>
                                                    <Select OnEvent="PrestableceProductos" Before="#{hdnIdColectiva}.clear();
                                                        #{hdnIdColectiva}.setValue(this.getValue()); #{cBoxProducto}.clearValue();" />
                                                </DirectEvents>
                                            </ext:ComboBox>
                                            <ext:ComboBox ID="cBoxProducto" runat="server" EmptyText="Producto" ListWidth="200"
                                                Width="120" DisplayField="Descripcion" ValueField="ID_Producto" AllowBlank="false">
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
                                            <ext:Button ID="btnBuscarSubProducto" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnBuscarSubProducto_Click" Before="if (!#{cBoxSubEmisor}.isValid() ||
                                                        !#{cBoxProducto}.isValid()) { return false; } else { 
                                                        #{GridResultSubProdsParab}.getStore().removeAll();
                                                        #{PanelCentralSubProd}.setTitle('_'); #{PanelCentralSubProd}.setDisabled(true); }">
                                                        <EventMask ShowMask="true" Msg="Buscando Subproductos..." MinDelay="500" />
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <Store>
                                    <ext:Store ID="StoreSubproductos" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Plantilla">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Plantilla" />
                                                    <ext:RecordField Name="Clave" />
                                                    <ext:RecordField Name="Descripcion" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <ColumnModel runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_SubProducto" Hidden="true" />
                                        <ext:Column DataIndex="Clave" Header="Clave" Width="90" />
                                        <ext:Column DataIndex="Descripcion" Header="Subproducto" Width="110" />
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel SingleSelect="true" />
                                </SelectionModel>
                                <DirectEvents>
                                    <RowClick OnEvent="selectRowResultadosSP_Event">
                                        <EventMask ShowMask="true" Msg="Obteniendo Información del Subproducto..." MinDelay="500" />
                                        <ExtraParams>
                                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridResultSubProdsParab}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                        </ExtraParams>
                                    </RowClick>
                                </DirectEvents>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreSubproductos" DisplayInfo="true"
                                        DisplayMsg="{0} - {1} de {2}" HideRefresh="true" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </West>
        <Center Split="true">
            <ext:Panel ID="PanelCentralSubProd" runat="server" Height="250" Border="false" Title="_" Disabled="true">
                <Items>
                    <ext:BorderLayout runat="server">
                        <Center>
                            <ext:TabPanel runat="server">
                                <Items>
                                    <ext:FormPanel ID="FormPanelInfoAdSP" runat="server" Title="Información General" AutoScroll="true" Border="false">
                                        <Items>
                                            <ext:FormPanel ID="FormPanelDataInfoAdSP" runat="server" LabelAlign="Left" LabelWidth="150">
                                                <Items>
                                                    <ext:FieldSet runat="server" Title="Datos Generales del Subproducto" Layout="FormLayout">
                                                        <Items>
                                                            <ext:Hidden ID="hdnIdSubproducto" runat="server" />
                                                            <ext:TextField ID="txtClaveSubProd" runat="server" FieldLabel="<span style='color:red;'>*</span> Clave"
                                                                MaxLength="10" Width="540" AllowBlank="false" />
                                                            <ext:TextArea ID="txtDescSubProd" runat="server" FieldLabel="<span style='color:red;'>*</span> Nombre o Descripción"
                                                                MaxLength="200" Width="540" Height="100" AllowBlank="false" />
                                                            <ext:TextField ID="txtProducto" runat="server" FieldLabel="Producto" Width="540" Disabled="true" />
                                                        </Items>
                                                        <Buttons>
                                                            <ext:Button ID="btnGuardarInfoAdSP" runat="server" Text="Guardar" Icon="Disk">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnGuardarInfoAdSP_Click" Before="var valid= #{FormPanelDataInfoAdSP}.getForm().isValid(); if (!valid) {} return valid;">
                                                                        <EventMask ShowMask="true" Msg="Guardando Información..." MinDelay="500" />
                                                                    </Click>
                                                                </DirectEvents>
                                                            </ext:Button>
                                                        </Buttons>
                                                    </ext:FieldSet>
                                                </Items>
                                            </ext:FormPanel>
                                        </Items>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelParametrosSP" runat="server" Title="Parámetros" Layout="FitLayout" Border="false">
                                        <Items>
                                            <ext:Panel runat="server" Layout="FitLayout" AutoScroll="true" Border="false">
                                                <TopBar>
                                                    <ext:Toolbar runat="server">
                                                        <Items>
                                                            <ext:Hidden ID="hdnIdParametroMA" runat="server" />
                                                            <ext:Hidden ID="hdnIdValorPMA" runat="server" />
                                                            <ext:ComboBox ID="cBoxTipoParametroMA" runat="server" EmptyText="Tipo de Parámetros..." Width="150"
                                                                DisplayField="Descripcion" ValueField="ID_TipoParametroMultiasignacion" AllowBlank="false">
                                                                <Store>
                                                                    <ext:Store ID="StoreTipoParametroMA" runat="server">
                                                                        <Reader>
                                                                            <ext:JsonReader IDProperty="ID_TipoParametroMultiasignacion">
                                                                                <Fields>
                                                                                    <ext:RecordField Name="ID_TipoParametroMultiasignacion" />
                                                                                    <ext:RecordField Name="Clave" />
                                                                                    <ext:RecordField Name="Descripcion" />
                                                                                </Fields>
                                                                            </ext:JsonReader>
                                                                        </Reader>
                                                                    </ext:Store>
                                                                </Store>
                                                                <DirectEvents>
                                                                    <Select OnEvent="SeleccionaTipoPMA" Before="#{cBoxParametros}.setDisabled(false);
                                                                        #{btnAddParametros}.setDisabled(false); #{cBoxParametros}.getStore().removeAll();
                                                                        #{cBoxParametros}.reset(); #{GridPanelParametros}.getStore().removeAll();">
                                                                        <EventMask ShowMask="true" Msg="Obteniendo Parámetros..." MinDelay="200" />
                                                                    </Select>
                                                                </DirectEvents>
                                                            </ext:ComboBox>
                                                            <ext:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                            <ext:ComboBox ID="cBoxParametros" runat="server" EmptyText="Parámetros sin Asignar..." Width="180"
                                                                DisplayField="Nombre" ValueField="ID_ParametroMultiasignacion" Disabled="true" AllowBlank="false">
                                                                <Store>
                                                                    <ext:Store ID="StoreParametros" runat="server">
                                                                        <Reader>
                                                                            <ext:JsonReader IDProperty="ID_ParametroMultiasignacion">
                                                                                <Fields>
                                                                                    <ext:RecordField Name="ID_ParametroMultiasignacion" />
                                                                                    <ext:RecordField Name="Nombre" />
                                                                                    <ext:RecordField Name="Descripcion" />
                                                                                </Fields>
                                                                            </ext:JsonReader>
                                                                        </Reader>
                                                                    </ext:Store>
                                                                </Store>
                                                            </ext:ComboBox>
                                                            <ext:Button ID="btnAddParametros" runat="server" Text="Asignar Parámetro" Icon="Add" Disabled="true">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnAddParametros_Click" Before="var valid= #{cBoxParametros}.isValid(); if (!valid) {} return valid;">
                                                                        <EventMask ShowMask="true" Msg="Asignando Parámetro..." MinDelay="500" />
                                                                    </Click>
                                                                </DirectEvents>
                                                            </ext:Button>
                                                        </Items>
                                                    </ext:Toolbar>
                                                </TopBar>
                                                <Items>
                                                    <ext:GridPanel ID="GridPanelParametros" runat="server" Header="true" Border="false" AutoScroll="true"
                                                        AutoHeight="true" Layout="FitLayout" AutoExpandColumn="Nombre">
                                                        <Store>
                                                            <ext:Store ID="StoreValoresParametros" runat="server">
                                                                <Reader>
                                                                    <ext:JsonReader IDProperty="ID_ValorParametroMultiasignacion">
                                                                        <Fields>
                                                                            <ext:RecordField Name="ID_ParametroMultiasignacion" />
                                                                            <ext:RecordField Name="ID_ValorParametroMultiasignacion" />
                                                                            <ext:RecordField Name="Nombre" />
                                                                            <ext:RecordField Name="Descripcion" />
                                                                            <ext:RecordField Name="Valor" />
                                                                            <ext:RecordField Name="TipoDato" />
                                                                            <ext:RecordField Name="esAutorizable" />
                                                                        </Fields>
                                                                    </ext:JsonReader>
                                                                </Reader>
                                                            </ext:Store>
                                                        </Store>
                                                        <ColumnModel runat="server">
                                                            <Columns>
                                                                <ext:Column ColumnID="ID_Parametro" runat="server" Hidden="true" DataIndex="ID_Parametro" />
                                                                <ext:Column ColumnID="Nombre" Header="Parámetro" Width="350" DataIndex="Nombre">
                                                                    <Renderer Fn="fullName" />
                                                                    <Editor>
                                                                        <ext:DisplayField runat="server" StyleSpec="font-weight:bold;font-family:segoe ui;font-size: 12px;" />
                                                                    </Editor>
                                                                </ext:Column>
                                                                <ext:Column ColumnID="Valor" Header="Valor" Sortable="true" DataIndex="Valor" Width="140" />
                                                                <ext:Column runat="server" Hidden="true" DataIndex="TipoDato" />
                                                                <ext:CommandColumn Header="Acciones" Width="80" >
                                                                    <PrepareToolbar Fn="prepareTB_SubPParams" />
                                                                    <Commands>
                                                                        <ext:GridCommand Icon="Pencil" CommandName="Edit">
                                                                            <ToolTip Text="Editar Valor" />
                                                                        </ext:GridCommand>
                                                                        <ext:GridCommand Icon="Delete" CommandName="Delete">
                                                                            <ToolTip Text="Quitar Parámetro al Subproducto" />
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
                                                                    ConfirmRequest="true" Title="Confirmación" Message="¿Estás seguro de quitar el parámetro a la colectiva?" />
                                                                <ExtraParams>
                                                                    <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                                                    <ext:Parameter Name="Values" Value="Ext.encode(record.data)" Mode="Raw" />
                                                                </ExtraParams>
                                                            </Command>
                                                        </DirectEvents>
                                                        <LoadMask ShowMask="false" />
                                                    </ext:GridPanel>
                                                </Items>
                                            </ext:Panel>
                                        </Items>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelTarjetas" runat="server" Title="Tarjetas" Layout="FitLayout">
                                        <Items>
                                            <ext:GridPanel ID="GridTarjetas" runat="server" Header="true" Height="200" AutoExpandColumn="NombreEmbozo">
                                                <TopBar>
                                                    <ext:Toolbar runat="server">
                                                        <Items>
                                                            <ext:TextField ID="txtFiltroTarjeta" runat="server" Width="200" EmptyText="Ingresa un BIN de tarjeta"
                                                                MaskRe="[0-9]" MinLength="8" MaxLength="8" AllowBlank="false"/>
                                                            <ext:ToolbarSeparator runat="server" />
                                                            <ext:Button ID="btnFiltrar" runat="server" Text="Filtrar" Icon="Drink">
                                                                <DirectEvents>
                                                                    <Click OnEvent="btnFiltrar_Click" Timeout="360000"
                                                                        Before="if (!#{txtFiltroTarjeta}.isValid()) { return false; } else { 
                                                                        resetToolbar(#{PagingTB_Tarjetas}); #{GridTarjetas}.getStore().sortInfo = null; }" />
                                                                </DirectEvents>
                                                            </ext:Button>
                                                            <ext:Button ID="btnFiltrarHide" runat="server" Hidden="true">
                                                                <Listeners>
                                                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Tarjetas...' });
                                                                        #{GridTarjetas}.getStore().reload({params:{start:0, sort:('','')}});" />
                                                                </Listeners>
                                                            </ext:Button>
                                                            <ext:ToolbarFill runat="server" />
                                                            <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                                                <DirectEvents>
                                                                    <Click OnEvent="Download" IsUpload="true"
                                                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                                        AdminSubProd.StopMask();" />
                                                                </DirectEvents>
                                                            </ext:Button>
                                                            <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="true">
                                                                <DirectEvents>
                                                                    <Click OnEvent="Download" IsUpload="true"
                                                                        After="Ext.net.Mask.show({ msg : 'Exportando Tarjetas a Excel...' }); e.stopEvent();
                                                                        AdminSubProd.StopMask();" />
                                                                </DirectEvents>
                                                            </ext:Button>
                                                        </Items>
                                                    </ext:Toolbar>
                                                </TopBar>
                                                <Store>
                                                    <ext:Store ID="StoreTarjetas" runat="server" RemoteSort="true" OnRefreshData="StoreTarjetas_RefreshData"
                                                        AutoLoad="false">
                                                        <AutoLoadParams>
                                                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                                        </AutoLoadParams>
                                                        <Proxy>
                                                            <ext:PageProxy />
                                                        </Proxy>
                                                        <DirectEventConfig IsUpload="true" />
                                                        <Reader>
                                                            <ext:JsonReader IDProperty="ID_MA">
                                                                <Fields>
                                                                    <ext:RecordField Name="ID_MA" />
                                                                    <ext:RecordField Name="ClaveMA" />
                                                                    <ext:RecordField Name="NombreEmbozo" />
                                                                </Fields>
                                                            </ext:JsonReader>
                                                        </Reader>
                                                        <DirectEventConfig IsUpload="true" />
                                                    </ext:Store>
                                                </Store>
                                                <ColumnModel runat="server">
                                                    <Columns>
                                                        <ext:Column DataIndex="ID_Tarjeta" Hidden="true" />
                                                        <ext:Column DataIndex="ClaveMA" Header="Número de Tarjeta" Width="200" />
                                                        <ext:Column DataIndex="NombreEmbozo" Header="Nombre Embozo" Width="350" />
                                                    </Columns>
                                                </ColumnModel>
                                                <SelectionModel>
                                                    <ext:RowSelectionModel SingleSelect="true" />
                                                </SelectionModel>
                                                <BottomBar>
                                                    <ext:PagingToolbar ID="PagingTB_Tarjetas" runat="server" StoreID="StoreTarjetas" DisplayInfo="true"
                                                        DisplayMsg="Mostrando Tarjetas {0} - {1} de {2}" HideRefresh="true" />
                                                </BottomBar>
                                            </ext:GridPanel>
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
