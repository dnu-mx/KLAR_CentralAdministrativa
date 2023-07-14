<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="ReservarTarjetas.aspx.cs" Inherits="BovedaTarjetas.ReservarTarjetas" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
   <script type="text/javascript">
       var validaReserva = function (porReservar, disponibles, emisor, producto, tipo) {
           var title = 'Reservar Tarjetas';
           var res = parseInt(porReservar.replace(',', ''));
           var disp = parseInt(disponibles.replace(',', ''));
           var msg = '¿Confirmas la reservación de ' + porReservar + ' tarjetas del tipo ' + tipo + ', del Producto ' + producto  + ' para el Emisor ' + emisor + ' ?';

           if (res > disp) {
               Ext.MessageBox.show({ title,
                   icon: Ext.MessageBox.INFO,
                   msg: 'No puedes reservar más tarjetas que las disponibles',
                   buttons: Ext.MessageBox.OK
               });
               return false;
           }

           Ext.Msg.confirm(title, msg, function (btn) {
               if (btn == 'yes') {
                   Ext.net.Mask.show({ msg: 'Procesando...' });
                   ReservaTarjetasBoveda.ConfirmaReservarTarjetas();
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
            <ext:FormPanel ID="FormPanelGlobal" runat="server" Layout="FormLayout" Border="false" Padding="10">
                <Items>
                    <ext:Hidden ID="hdnTextoTipoTarj" runat="server" />
                    <ext:FieldSet runat="server" Title="Selecciona los Filtros" Layout="AnchorLayout" Padding="10">
                        <Items>
                            <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false" LabelAlign="Right">
                                <Defaults>
                                    <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                </Defaults>
                                <LayoutConfig>
                                    <ext:HBoxLayoutConfig Align="Top" />
                                </LayoutConfig>
                                <Items>
                                    <ext:ComboBox ID="cBoxEmisor" runat="server" FieldLabel="Emisor" Width="200" LabelWidth="50" Editable="false"
                                        AllowBlank="false" DisplayField="NombreEmisor" ValueField="ClaveEmisor">
                                        <Store>
                                            <ext:Store ID="StoreEmisor" runat="server">
                                                <Reader>
                                                    <ext:JsonReader IDProperty="ClaveEmisor">
                                                        <Fields>
                                                            <ext:RecordField Name="ClaveEmisor" />
                                                            <ext:RecordField Name="NombreEmisor" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Store>
                                        <DirectEvents>
                                            <Select OnEvent="EstableceProductos" Before="if (!this.isValid()) { return false; }">
                                                <EventMask ShowMask="true" Msg="Obteniendo Productos..." MinDelay="200" />
                                            </Select>
                                        </DirectEvents>
                                    </ext:ComboBox>
                                    <ext:Hidden runat="server" Width="100" />
                                    <ext:ComboBox ID="cBoxProducto" runat="server" FieldLabel="Producto" Width="200" LabelWidth="50" Disabled="true"
                                        Editable="false" AllowBlank="false" DisplayField="NombreProducto" ValueField="ClaveProducto" ListWidth="250">
                                        <Store>
                                            <ext:Store ID="StoreProducto" runat="server">
                                                <Reader>
                                                    <ext:JsonReader IDProperty="ClaveProducto">
                                                        <Fields>
                                                            <ext:RecordField Name="ClaveProducto" />
                                                            <ext:RecordField Name="NombreProducto" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Store>
                                        <DirectEvents>
                                            <Select OnEvent="EstableceTiposMan" Before="if (!this.isValid()) { return false; } else { 
                                                #{FieldSetInfo}.collapse(true); }">
                                                <EventMask ShowMask="true" Msg="Obteniendo Tipos de Tarjeta..." MinDelay="200" />
                                            </Select>
                                        </DirectEvents>
                                    </ext:ComboBox>
                                    <ext:Hidden runat="server" Width="100" />
                                    <ext:ComboBox ID="cBoxTipoTar" runat="server" FieldLabel="Tipo de Tarjeta" Width="250" LabelWidth="100" Editable="false"
                                        AllowBlank="false" Disabled="true" DisplayField="NombreTipoManufactura" ValueField="ClaveTipoManufactura">
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
                                            <Select OnEvent="EstableceCantidades" Before="var id = this.getValue();
                                                var record = this.getStore().getById(id);
                                                #{hdnTextoTipoTarj}.setValue(record.get('NombreTipoManufactura'));" />
                                        </DirectEvents>
                                    </ext:ComboBox>
                                </Items>
                            </ext:Panel>
                            <ext:Panel runat="server" Layout="FitLayout" Height="30" Width="500" Border="false" />
                            <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false" LabelAlign="Right">
                                <Defaults>
                                    <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                </Defaults>
                                <LayoutConfig>
                                    <ext:HBoxLayoutConfig Align="Top" />
                                </LayoutConfig>
                                <Items>
                                    <ext:Hidden runat="server" Width="380" />
                                    <ext:Button ID="btnLimpiar" runat="server" Icon="ArrowRefresh" Text="Limpiar Filtros">
                                        <DirectEvents>
                                            <Click OnEvent="btnLimpiar_Click" />
                                        </DirectEvents>
                                    </ext:Button>
                                </Items>
                            </ext:Panel>
                        </Items>
                    </ext:FieldSet>
                    <ext:FieldSet ID="FieldSetInfo" runat="server" Title="Información en Bóveda" Layout="AnchorLayout" Padding="10"
                        Collapsed="true">
                        <Items>
                            <ext:FormPanel ID="FormPanelInfo" runat="server" Layout="FormLayout" Header="false" Border="false" LabelWidth="195">
                                <Items>
                                    <ext:Panel runat="server" Layout="FitLayout" Width="500" Height="10" Border="false" />
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
                                            <ext:Hidden runat="server" Flex="2" Width="25" />
                                            <ext:Label ID="lblTarjDisp" runat="server" Text="-" LabelSeparator=" "
                                                Width="150" StyleSpec="font-weight:bold;font-family:segoe ui;font-size: 14px;text-align:right;" />
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel runat="server" Layout="FitLayout" Width="500" Height="20" Border="false" />
                                    <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false">
                                        <Defaults>
                                            <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                        </Defaults>
                                        <LayoutConfig>
                                            <ext:HBoxLayoutConfig Align="Top" />
                                        </LayoutConfig>
                                        <Items>
                                            <ext:Hidden runat="server" Flex="1" Width="200" />
                                            <ext:Label runat="server" FieldLabel="Número de Tarjetas Reservadas" />
                                            <ext:Hidden runat="server" Flex="2" Width="25" />
                                            <ext:Label ID="lblTarjReserv" runat="server" Text="-" LabelSeparator=" "
                                                Width="150" StyleSpec="font-weight:bold;font-family:segoe ui;font-size: 14px;text-align:right;" />
                                        </Items>
                                    </ext:Panel>
                                    <ext:Panel runat="server" Layout="FitLayout" Width="500" Height="20" Border="false" />
                                    <ext:Panel runat="server" Layout="HBoxLayout" BodyPadding="5" Border="false">
                                        <Defaults>
                                            <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                        </Defaults>
                                        <LayoutConfig>
                                            <ext:HBoxLayoutConfig Align="Top" />
                                        </LayoutConfig>
                                        <Items>
                                            <ext:Hidden runat="server" Flex="1" Width="200" />
                                            <ext:Label runat="server" FieldLabel="Número de de Tarjetas a Reservar" />
                                            <ext:Hidden runat="server" Flex="2" Width="25" />
                                            <ext:TextField ID="txtTarjReserv" runat="server" Width="150" MaskRe="[0-9]"
                                                AllowBlank="false" MaxLength="6" StyleSpec="text-align:right;" />
                                        </Items>
                                    </ext:Panel>
                                </Items>
                            </ext:FormPanel>
                        </Items>
                        <Buttons>
                            <ext:Button ID="btnReservar" runat="server" Text="Reservar" Icon="Tick">
                                <Listeners>
                                    <Click Handler="if (!#{cBoxProducto}.isValid() || !#{cBoxTipoTar}.isValid() || 
                                        !#{txtTarjReserv}.isValid()) { return false; } else {
                                        return validaReserva(#{txtTarjReserv}.getValue(), #{lblTarjDisp}.text,
                                        #{cBoxEmisor}.getValue(), #{cBoxProducto}.getValue(), #{hdnTextoTipoTarj}.getValue()); }"/>
                                </Listeners>
                            </ext:Button>
                        </Buttons>
                    </ext:FieldSet>
                </Items>
            </ext:FormPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>

