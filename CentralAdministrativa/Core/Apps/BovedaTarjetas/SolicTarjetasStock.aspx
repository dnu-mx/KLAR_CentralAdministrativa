<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="SolicTarjetasStock.aspx.cs" Inherits="BovedaTarjetas.SolicTarjetasStock" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var validaEmision = function (porEmitir, disponibles, emisor, producto, tipo, dis) {
            var title = 'Solicitud de Tarjetas Stock';
            var emi = parseInt(porEmitir.replace(',', ''));
            var disp = parseInt(disponibles.replace(',', ''));
            var msgDis = dis == '' ? '' : 'Diseño: <b> ' + dis + '</b></br >';
            var msg = '¿Confirmas la solicitud de <b>' + porEmitir + '</b> tarjetas con las siguientes características?</br></br>Emisor: <b>' + emisor + '</b></br>Producto: <b>' + producto + '</b></br>Tipo de Manufactura: <b>' + tipo + '</b></br>' + msgDis;

            if (emi > disp) {
                Ext.MessageBox.show({
                    title,
                    icon: Ext.MessageBox.INFO,
                    msg: 'No puedes solicitar más tarjetas que las disponibles',
                    buttons: Ext.MessageBox.OK
                });
                return false;
            }

            Ext.Msg.confirm(title, msg, function (btn) {
                if (btn == 'yes') {
                    Ext.net.Mask.show({ msg: 'Procesando...' });
                    SolicitaTarjetasStock.ConfirmaEmisionTarjetas();
                    return true;
                } else {
                    return false;
                }
            });
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="MainBorderLayout" runat="server">
        <Center Split="true">
            <ext:FormPanel ID="FormPanelStock" runat="server" Layout="FormLayout" Border="false" Padding="10">
                <Items>
                    <ext:FieldSet runat="server" Title="Selecciona los Filtros" Layout="AnchorLayout" Padding="10">
                        <Items>
                            <ext:Panel runat="server" FormGroup="true" Layout="TableLayout" Border="false" LabelWidth="160" LabelAlign="Right"
                                AutoHeight="true" AutoWidth="true">
                                <Items>
                                    <ext:Panel runat="server" Border="false" Header="false" Width="320px" ColumnWidth=".5" Layout="Form">
                                        <Items>
                                            <ext:Hidden ID="hdnEmisor" runat="server" />
                                            <ext:Hidden ID="hdnClaveEmisor" runat="server" />
                                            <ext:ComboBox ID="cBoxEmisor" runat="server" FieldLabel="Emisor" AnchorHorizontal="95%" Editable="false"
                                                AllowBlank="false" DisplayField="NombreORazonSocial" ValueField="ID_Colectiva" TabIndex="1" ListWidth="250">
                                                <Store>
                                                    <ext:Store ID="StoreEmisores" runat="server">
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
                                                <DirectEvents>
                                                    <Select OnEvent="PrestableceProductos" Before="#{cBoxProducto}.setDisabled(true);
                                                        #{cBoxProducto}.clearValue(); var id = this.getValue(); var record = this.getStore().getById(id);
                                                        #{hdnEmisor}.setValue(record.get('NombreORazonSocial')); #{hdnClaveEmisor}.setValue(record.get('ClaveColectiva'));">
                                                        <EventMask ShowMask="true" Msg="Obteniendo Productos..." MinDelay="200" />
                                                    </Select>
                                                </DirectEvents>
                                            </ext:ComboBox>
                                            <ext:Panel runat="server" Layout="FitLayout" Border="false" Height="20" Width="500" />
                                            <ext:Hidden ID="hdnTipoTarjeta" runat="server" />
                                            <ext:ComboBox ID="cBoxTipoTar" runat="server" FieldLabel="Tipo de Manufactura" AnchorHorizontal="95%" Editable="false"
                                                AllowBlank="false" Disabled="true" DisplayField="NombreTipoManufactura" ValueField="ClaveTipoManufactura" TabIndex="3">
                                                <Store>
                                                    <ext:Store ID="StoreTipoMan" runat="server">
                                                        <Reader>
                                                            <ext:JsonReader IDProperty="ClaveTipoManufactura">
                                                                <Fields>
                                                                    <ext:RecordField Name="ClaveTipoManufactura" />
                                                                    <ext:RecordField Name="NombreTipoManufactura" />
                                                                </Fields>
                                                            </ext:JsonReader>
                                                        </Reader>
                                                    </ext:Store>
                                                </Store>
                                                <DirectEvents>
                                                    <Select OnEvent="PrestableceDisenyos" Before="#{cBoxDisenyo}.setDisabled(true); #{cBoxDisenyo}.clearValue();
                                                        #{hdnDisenyo}.setValue(''); var id = this.getValue(); var record = this.getStore().getById(id);
                                                        #{hdnTipoTarjeta}.setValue(record.get('NombreTipoManufactura'));" />
                                                </DirectEvents>
                                            </ext:ComboBox>
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel runat="server" Border="false" Header="false" Width="320px" ColumnWidth=".5" Layout="Form">
                                        <Items>
                                            <ext:Hidden ID="hdnProducto" runat="server" />
                                            <ext:Hidden ID="hdnCveProducto" runat="server" />
                                            <ext:ComboBox ID="cBoxProducto" runat="server" FieldLabel="Producto" AnchorHorizontal="95%" Editable="false"
                                                AllowBlank="false" Disabled="true" DisplayField="Descripcion" ValueField="ID_Producto" TabIndex="2"
                                                ListWidth="250">
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
                                                <DirectEvents>
                                                    <Select OnEvent="PrestableceTiposTarjeta" Before="if (!this.isValid()) { return false; } else { 
                                                        #{cBoxTipoTar}.setDisabled(true); #{FieldSetInfo}.collapse(true); #{cBoxTipoTar}.clearValue();
                                                        var id = this.getValue(); var record = this.getStore().getById(id); #{hdnCveProducto}.setValue(record.get('Clave'));
                                                        #{hdnProducto}.setValue(record.get('Descripcion')); }">
                                                        <EventMask ShowMask="true" Msg="Obteniendo Tipos de Manufactura..." MinDelay="200" />
                                                    </Select>
                                                </DirectEvents>
                                            </ext:ComboBox>
                                            <ext:Panel runat="server" Layout="FitLayout" Border="false" Height="20" Width="500" />
                                            <ext:Hidden ID="hdnDisenyo" runat="server" />
                                            <ext:ComboBox ID="cBoxDisenyo" runat="server" FieldLabel="Diseño de Tarjeta" AnchorHorizontal="95%" Editable="false"
                                                AllowBlank="false" Disabled="true" TabIndex="4" DisplayField="Descripcion" ValueField="ID_ValorPreePMA"
                                                ListWidth="250">
                                                <Store>
                                                    <ext:Store ID="StoreDisenyo" runat="server">
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
                                                <Listeners>
                                                    <Select Handler="var id = this.getValue(); var record = this.getStore().getById(id);
                                                        #{hdnDisenyo}.setValue(record.get('Descripcion'));" />
                                                </Listeners>
                                            </ext:ComboBox>
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel runat="server" Layout="FitLayout" Border="false" Height="50" Width="50" />
                                    <ext:Panel runat="server" Border="false" Header="false" Width="300px" ColumnWidth=".5" Layout="FormLayout">
                                        <Items>
                                            <ext:Button ID="btnLimpiar" runat="server" Icon="ArrowRefresh" Text="Limpiar Filtros"
                                                TabIndex="5">
                                                <DirectEvents>
                                                    <Click OnEvent="btnLimpiar_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Panel runat="server" Layout="FitLayout" Border="false" Height="25" Width="500" />
                                            <ext:Button ID="btnSolicitaInfo" runat="server" Icon="ArrowDown" Text="Solicitar Información"
                                                TabIndex="6">
                                                <DirectEvents>
                                                    <Click OnEvent="btnSolicitaInfo_click" Before="if (!#{cBoxProducto}.isValid() || 
                                                        !#{cBoxTipoTar}.isValid()) { return false; } else { #{lblTarjDisp}.reset();
                                                        #{lblTarjEmit}.reset(); #{txtClaveSolic}.reset(); #{txtTarjEmitir}.reset(); }">
                                                        <EventMask ShowMask="true" Msg="Solicitando Información a Bóveda..." MinDelay="500" />
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Panel>
                                </Items>
                            </ext:Panel>
                        </Items>
                    </ext:FieldSet>
                    <ext:FieldSet ID="FieldSetInfo" runat="server" Title="Información en Bóveda" Layout="FormLayout" Padding="10" LabelWidth="175"
                        Collapsed="true">
                        <Items>
                            <ext:FormPanel ID="FormPanelInfo" runat="server" Layout="FormLayout" Header="false" Border="false" LabelWidth="175">
                                <Items>
                                    <ext:Panel runat="server" Layout="FitLayout" Width="700" Height="10" Border="false" />
                                    <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false">
                                        <Defaults>
                                            <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                        </Defaults>
                                        <LayoutConfig>
                                            <ext:HBoxLayoutConfig Align="Top" />
                                        </LayoutConfig>
                                        <Items>
                                            <ext:Hidden runat="server" Flex="1" Width="200" />
                                            <ext:Label runat="server" FieldLabel="Número de Tarjetas Disponibles" />
                                            <ext:Hidden runat="server" Flex="2" Width="90" />
                                            <ext:Label ID="lblTarjDisp" runat="server" Text="-" LabelSeparator=" "
                                                Width="150" StyleSpec="font-weight:bold;font-family:segoe ui;font-size: 14px;text-align:right;" />
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel runat="server" Layout="FitLayout" Width="700" Height="15" Border="false" />
                                    <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false">
                                        <Defaults>
                                            <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                        </Defaults>
                                        <LayoutConfig>
                                            <ext:HBoxLayoutConfig Align="Top" />
                                        </LayoutConfig>
                                        <Items>
                                            <ext:Hidden runat="server" Flex="1" Width="200" />
                                            <ext:Label runat="server" FieldLabel="Número de Tarjetas Emitidas" />
                                            <ext:Hidden runat="server" Flex="2" Width="90" />
                                            <ext:Label ID="lblTarjEmit" runat="server" Text="-" LabelSeparator=" "
                                                Width="150" StyleSpec="font-weight:bold;font-family:segoe ui;font-size: 14px;text-align:right;" />
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel runat="server" Layout="FitLayout" Width="700" Height="15" Border="false" />
                                    <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false">
                                        <Defaults>
                                            <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                        </Defaults>
                                        <LayoutConfig>
                                            <ext:HBoxLayoutConfig Align="Top" />
                                        </LayoutConfig>
                                        <Items>
                                            <ext:Hidden runat="server" Flex="1" Width="200" />
                                            <ext:Label runat="server" FieldLabel="Clave de la Solicitud" />
                                            <ext:Hidden runat="server" Flex="2" Width="90" />
                                            <ext:TextField ID="txtClaveSolic" runat="server" Width="150" MaxLength="10" 
                                                AllowBlank="false" StyleSpec="text-align:right;" />
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel runat="server" Layout="FitLayout" Width="800" Height="20" Border="false" />
                                    <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false">
                                        <Defaults>
                                            <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                        </Defaults>
                                        <LayoutConfig>
                                            <ext:HBoxLayoutConfig Align="Top" />
                                        </LayoutConfig>
                                        <Items>
                                            <ext:Hidden runat="server" Flex="1" Width="200" />
                                            <ext:Label runat="server" FieldLabel="Número de Tarjetas por Emitir" />
                                            <ext:Hidden runat="server" Flex="2" Width="90" />
                                            <ext:TextField ID="txtTarjEmitir" runat="server" Width="150" MaskRe="[0-9]"
                                                AllowBlank="false" MaxLength="6" StyleSpec="text-align:right;" />
                                            <ext:Hidden runat="server" Flex="3" Width="70" />
                                            <ext:Button ID="btnSolicitarStock" runat="server" Text="Solicitar" Icon="Tick">
                                                <Listeners>
                                                    <Click Handler="if (!#{cBoxEmisor}.isValid() || !#{cBoxProducto}.isValid() ||
                                                        !#{cBoxTipoTar}.isValid() ||  !#{txtClaveSolic}.isValid() || 
                                                        !#{txtTarjEmitir}.isValid()) { return false; } else { return validaEmision(#{txtTarjEmitir}.getValue(),
                                                        #{lblTarjDisp}.text, #{hdnEmisor}.getValue(), #{hdnProducto}.getValue(), #{hdnTipoTarjeta}.getValue(),
                                                        #{hdnDisenyo}.getValue());}" />
                                                </Listeners>
                                            </ext:Button>
                                        </Items>
                                    </ext:Panel>
                                </Items>
                            </ext:FormPanel>
                        </Items>
                    </ext:FieldSet>
                </Items>
            </ext:FormPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>