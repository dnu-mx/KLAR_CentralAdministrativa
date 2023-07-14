<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdministracionReglasEFV.aspx.cs" Inherits="ValidacionesBatch.AdministracionReglasEFV" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var template = '<span style="color:blue;font-weight: normal;font-family: tahoma, verdana;">{1}</span>';

        var rowEdition = function (value) {
            alert(value.values['ext-comp-1033']);
            return String.format(template, value);
        };


        var afterEdit = function (e) {
            var title = 'Parámetros de la Regla';

            if (e.record.data.ValorAlertar == '') {
                Ext.Msg.alert(title, 'El valor de la Regla para Alertar es obligatorio.');
                return false;
            }

            if (e.record.data.ValorRechazar == '') {
                Ext.Msg.alert(title, 'El valor de la Regla para Rechazar es obligatorio.');
                return false;
            }

            if (e.record.data.ValorBloquear == '') {
                Ext.Msg.alert(title, 'El valor de la Regla para Bloquear es obligatorio.');
                return false;
            }

            ValidacionesBatch.ActualizaValorParametro(e.record.data.IdParametro, e.record.data.Clave, e.record.data.ValorDefault,
                e.record.data.ValorAlertar, e.record.data.ValorRechazar, e.record.data.ValorBloquear);
        };

        var fullName = function (value, metadata, record, rowIndex, colIndex, store) {
            return "<b>" + record.data.Clave + "</b>";
        };

    </script>
    <style type="text/css">
        .x-grid3-row-body p {
            margin: 5px 5px 10px 5px !important;
            width: 99%;
            color: black;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="MainBorderLayout" runat="server">
        <West Split="true">
            <ext:Panel ID="LeftPanel" runat="server" Width="350" Layout="FitLayout" Border="false">
                <Content>
                    <ext:BorderLayout ID="BorderLayoutLeft" runat="server">
                        <North Split="true">
                            <ext:FormPanel ID="FormPanel2" runat="server" Border="false" Height="230" Padding="2">
                                <Content>
                                    <ext:Store ID="StoreReglas" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Regla">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Regla" />
                                                    <ext:RecordField Name="Nombre" />
                                                    <ext:RecordField Name="DescripcionRegla" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                    <ext:Store ID="StoreCadenaComercial" runat="server">
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
                                    <ext:Store ID="StoreEntidad" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Entidad">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Entidad" />
                                                    <ext:RecordField Name="ClaveEntidad" />
                                                    <ext:RecordField Name="Descripcion" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Content>
                                <Items>
                                    <ext:RowLayout ID="RowLayout1" runat="server" FitHeight="true">
                                        <Rows>
                                            <ext:LayoutRow>
                                                <ext:Image ID="Image1" runat="server" Height="60" Align="Middle" ImageUrl="Images/LogoEfectivale.png" />
                                            </ext:LayoutRow>
                                            <ext:LayoutRow>
                                                <ext:FormPanel ID="FormDatos" runat="server" Border="false" Padding="10" LabelAlign="Left" LabelWidth="130">
                                                    <Items>
                                                        <ext:Hidden ID="hdnIdEntidad" runat="server" />
                                                        <ext:ComboBox ID="cBoxEmpresa" runat="server" FieldLabel="Empresa" EmptyText="Selecciona una Empresa..."
                                                            AnchorHorizontal="100%" StoreID="StoreCadenaComercial" DisplayField="NombreORazonSocial"
                                                            ValueField="ID_Colectiva" AllowBlank="false" />
                                                        <ext:ComboBox ID="cBoxReglas" runat="server" FieldLabel="Regla" EmptyText="Selecciona una Regla..." AnchorHorizontal="100%"
                                                            StoreID="StoreReglas" DisplayField="Nombre" ValueField="ID_Regla" ItemID="DescripcionRegla" AllowBlank="false">
                                                            <Listeners>
                                                                <Select Handler="#{StoreValoresParametros}.removeAll();
                                                                    #{StoreResultadoEntidad}.removeAll();
                                                                    #{cBoxNivelConfig}.clear();
                                                                    #{txtDescrRegla}.setValue(#{cBoxReglas}.store.getAt(item.selectedIndex).get('DescripcionRegla'));" />
                                                            </Listeners>
                                                        </ext:ComboBox>
                                                        <ext:TextArea ID="txtDescrRegla" runat="server" FieldLabel="Descripción" AnchorHorizontal="100%"
                                                            ReadOnly="true" Selectable="false" />
                                                        <ext:ComboBox ID="cBoxNivelConfig" runat="server" FieldLabel="Nivel de Configuración" EmptyText="Selecciona un Nivel..."
                                                            AnchorHorizontal="100%" StoreID="StoreEntidad" DisplayField="Descripcion" ValueField="ID_Entidad"
                                                            AllowBlank="false">
                                                            <Listeners>
                                                                <Select Handler="#{StoreValoresParametros}.removeAll();
                                                                    #{StoreResultadoEntidad}.removeAll();
                                                                    if(this.getValue() != 1) {
                                                                    #{hdnIdEntidad}.setValue(#{cBoxNivelConfig}.store.getAt(item.selectedIndex).get('ID_Entidad'));
                                                                    #{GridPanelEntidad}.setTitle('Búsqueda de ' + #{cBoxNivelConfig}.store.getAt(item.selectedIndex).get('Descripcion'));
                                                                    #{GridPanelEntidad}.setDisabled(false);
                                                                    } else {
                                                                        #{GridPanelEntidad}.setTitle('Búsqueda de ');
                                                                        #{GridPanelEntidad}.setDisabled(true);
                                                                        var valid= #{FormDatos}.getForm().isValid(); if (!valid) {} 
                                                                        else {
                                                                            Ext.net.Mask.show({ msg:'Buscando Parámetros...' });
                                                                            Ext.net.Mask.hide.defer(500);
                                                                            ValidacionesBatch.LlenaPanelParametros(-1);
                                                                        }
                                                                    }" />
                                                            </Listeners>
                                                        </ext:ComboBox>
                                                    </Items>
                                                </ext:FormPanel>
                                            </ext:LayoutRow>
                                        </Rows>
                                    </ext:RowLayout>
                                </Items>
                            </ext:FormPanel>
                        </North>
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanelEntidad" runat="server" Title="Búsqueda de " Layout="FitLayout" Height="295" StripeRows="true" Header="true" Border="true" Disabled="true">
                                <LoadMask ShowMask="false" />
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar1" runat="server">
                                        <Items>
                                            <ext:Hidden ID="hdnIdRegEntidad" runat="server" />
                                            <ext:Hidden ID="hdnClaveEntidad" runat="server" />
                                            <ext:Hidden ID="hdnEstatus" runat="server" />
                                            <ext:TextField ID="txtClave" EmptyText="Ingresa la Clave..." Width="130" runat="server" />
                                            <ext:TextField ID="txtDescripcion" EmptyText="... y/o la Descripción" Width="150" runat="server" />
                                            <ext:Button ID="btnBuscarEntidad" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnBuscarEntidad_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <Store>
                                    <ext:Store ID="StoreResultadoEntidad" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_RegistroEntidad">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Entidad" />
                                                    <ext:RecordField Name="ID_RegistroEntidad" />
                                                    <ext:RecordField Name="ClaveEntidad" />
                                                    <ext:RecordField Name="Descripcion" />
                                                    <ext:RecordField Name="Estatus" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                        <ext:Column ColumnID="ID_Entidad" Hidden="true" DataIndex="ID_Entidad" />
                                        <ext:Column ColumnID="ID_RegistroEntidad" Hidden="true" DataIndex="ID_RegistroEntidad" />
                                        <ext:Column ColumnID="ClaveEntidad" Header="Clave" Width="100" Sortable="true" DataIndex="ClaveEntidad" />
                                        <ext:Column ColumnID="Descripción" Header="Descripción" Width="300" Sortable="true" DataIndex="Descripcion" />
                                        <ext:Column ColumnID="Estatus" Hidden="true" DataIndex="Estatus" />
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel SingleSelect="true" />
                                </SelectionModel>
                                <DirectEvents>
                                    <RowClick OnEvent="selectRowEntidad_Event">
                                        <EventMask ShowMask="true" Msg="Buscando Parámetros..." MinDelay="500" />
                                        <ExtraParams>
                                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridPanelEntidad}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                        </ExtraParams>
                                    </RowClick>
                                </DirectEvents>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreResultadoEntidad" DisplayInfo="true"
                                        DisplayMsg="Tarjetas {0} - {1} de {2}" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </West>
        <Center Split="true">
            <ext:Panel ID="PanelParametros" runat="server" Split="true" Collapsible="false" Border="false"
                Layout="FitLayout">
                <Items>
                    <ext:GridPanel ID="GridPanelParametros" runat="server" Header="true" Border="false" Title="Parámetros de la Regla">
                        <Store>
                            <ext:Store ID="StoreValoresParametros" runat="server">
                                <Reader>
                                    <ext:JsonReader IDProperty="IdParametro">
                                        <Fields>
                                            <ext:RecordField Name="IdParametro" />
                                            <ext:RecordField Name="Clave" />
                                            <ext:RecordField Name="Descripcion" />
                                            <ext:RecordField Name="ValorDefault" />
                                            <ext:RecordField Name="ValorAlertar" />
                                            <ext:RecordField Name="ValorRechazar" />
                                            <ext:RecordField Name="ValorBloquear" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <TopBar>
                            <ext:Toolbar ID="ToolbarTarjeta" runat="server" Hidden="true">
                                <Items>
                                    <ext:ToolbarFill ID="dummy" runat="server" />
                                    <ext:Button ID="btnEstatusTarjeta" runat="server" Text="Desbloquear Tarjeta"
                                        Icon="LockOpen" Hidden="true">
                                        <Listeners>
                                            <Click Handler="Ext.MessageBox.show({
                                                            title: 'Confirmación',
                                                            msg: '¿Estás seguro de Desbloquear la Tarjeta?',
                                                            buttons: Ext.MessageBox.YESNO,
                                                            fn: function (btn) {
                                                                if (btn == 'yes') {
                                                                    ValidacionesBatch.DesbloqueaTarjeta();
                                                                }
                                                            }
                                                         });" />
                                        </Listeners>
                                        </ext:Button>
                                    <ext:ToolbarSeparator runat="server" />
                                    <ext:Button ID="btnCerrarCaso" runat="server" Text="Cerrar Caso" Icon="Cross"
                                        Hidden="true">
                                         <Listeners>
                                            <Click Handler="Ext.MessageBox.show({
                                                            title: 'Advertencia',
                                                            msg: 'Este caso se cerrará como NORMAL. Si deseas cerrarlo como fraudulento, se debe hacer desde la pantalla de Incidencias.</br></br>¿Estás seguro de Cerrar el Caso?',
                                                            icon: Ext.MessageBox.WARNING,
                                                            buttons: Ext.MessageBox.YESNO,
                                                            fn: function (btn) {
                                                                if (btn == 'yes') {
                                                                    ValidacionesBatch.CierraCaso();
                                                                }
                                                            }
                                                         });" />
                                        </Listeners>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                        <ColumnModel ID="ColumnModel8" runat="server">
                            <Columns>
                                <ext:Column ColumnID="IdParametro" runat="server" Hidden="true" DataIndex="IdParametro" />
                                <ext:Column ColumnID="Clave" Header="Parámetro" Width="280" DataIndex="Clave">
                                    <Renderer Fn="fullName" />
                                    <Editor>
                                        <ext:DisplayField runat="server" StyleSpec="font-weight:bold;font-family:segoe ui;font-size: 12px;"/>
                                    </Editor>
                                </ext:Column>
                                <ext:Column ColumnID="ValorDefault" runat="server" Hidden="true" DataIndex="ValorDefault" />
                                <ext:Column ColumnID="ValorAlertar" Header="Alertar" Sortable="true" DataIndex="ValorAlertar">
                                    <Editor>
                                        <ext:TextField ID="TextField15" EmptyText="" runat="server" />
                                    </Editor>
                                </ext:Column>
                                <ext:Column ColumnID="ValorRechazar" Header="Rechazar" Sortable="true" DataIndex="ValorRechazar">
                                    <Editor>
                                        <ext:TextField ID="TextField17" EmptyText="" runat="server" />
                                    </Editor>
                                </ext:Column>
                                <ext:Column ColumnID="ValorBloquear" Header="Bloquear" Sortable="true" DataIndex="ValorBloquear">
                                    <Editor>
                                        <ext:TextField ID="TextField16" EmptyText="" runat="server" />
                                    </Editor>
                                </ext:Column>
                            </Columns>
                        </ColumnModel>
                        <View>
                            <ext:GridView runat="server" EnableRowBody="true">
                                <GetRowClass Handler="rowParams.body = '<p>' + record.data.Descripcion + '</p>'; return 'x-grid3-row-expanded';" />
                            </ext:GridView>
                        </View>
                        <SelectionModel>
                            <ext:RowSelectionModel ID="RowSelectionModel8" runat="server" SingleSelect="true" />
                        </SelectionModel>
                        <Plugins>
                            <ext:RowEditor ID="RowEditor5" runat="server" SaveText="Actualizar" CancelText="Cancelar">
                                <Listeners>
                                    <AfterEdit Fn="afterEdit" />
                                </Listeners>
                            </ext:RowEditor>
                        </Plugins>
                        <LoadMask ShowMask="false" />
                    </ext:GridPanel>
                </Items>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>


