<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="ActualizaBeneficiosPrana.aspx.cs" Inherits="Lealtad.ActualizaBeneficiosPrana" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content runat="server" ContentPlaceHolderID="MainContent">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center Split="true">
            <ext:Panel ID="Panel1" runat="server" Width="350">
                <Content>
                    <ext:BorderLayout ID="LeftBorderLayout" runat="server">
                        <North Split="true">
                            <ext:FormPanel ID="FormPanel1" runat="server" Height="30">
                                <Items>
                                    <ext:Toolbar ID="ToolbarConsulta" runat="server" Layout="HBoxLayout" BodyPadding="5"
                                        Region="North">
                                        <Defaults>
                                            <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                        </Defaults>
                                        <LayoutConfig>
                                            <ext:HBoxLayoutConfig Align="Middle" />
                                        </LayoutConfig>
                                        <Items>
                                            <ext:FileUploadField ID="FileUploadField1" runat="server" ButtonText="Examinar..."
                                                Icon="Magnifier" Flex="3" MarginSpec="0" />
                                            <ext:Hidden ID="Hidden1" runat="server" Flex="1" />
                                            <ext:Button ID="btnCargarArchivo" runat="server" Text="Cargar Archivo"
                                                Icon="PageWhitePut" Flex="1">
                                                <DirectEvents>
                                                    <Click OnEvent="btnCargarArchivo_Click" IsUpload="true" Before="#{GridDatosArchivo}.getStore().removeAll();"
                                                        After="#{FileUploadField1}.reset();">
                                                        <EventMask ShowMask="true" Msg="Cargando Archivo..." MinDelay="500" />
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </Items>
                            </ext:FormPanel>
                        </North>
                        <Center Split="true" Collapsible="false">
                            <ext:FormPanel ID="FormPanelResultados" runat="server" Layout="FitLayout">
                                <Items>
                                    <ext:GridPanel ID="GridDatosArchivo" runat="server" StripeRows="true" AutoScroll="true" Layout="FitLayout">
                                        <Store>
                                            <ext:Store runat="server">
                                                <Reader>
                                                    <ext:JsonReader IDProperty="ID_Colectiva">
                                                        <Fields>
                                                            <ext:RecordField Name="CLAVECADENA" />
                                                            <ext:RecordField Name="CADENA" />
                                                            <ext:RecordField Name="GIRO" />
                                                            <ext:RecordField Name="CLAVEPROMO" />
                                                            <ext:RecordField Name="TITULOPROMOCION" />
                                                            <ext:RecordField Name="TIPODESCUENTO" />
                                                            <ext:RecordField Name="DESCRIPCIONBENEFICIO" />
                                                            <ext:RecordField Name="RESTRICCIONES" />
                                                            <ext:RecordField Name="FACEBOOK" />
                                                            <ext:RecordField Name="WEB" />
                                                            <ext:RecordField Name="ESHOTDEAL" />
                                                            <ext:RecordField Name="CARRUSELHOME" />
                                                            <ext:RecordField Name="PROMOHOME" />
                                                            <ext:RecordField Name="ORDEN" />
                                                            <ext:RecordField Name="ACTIVA" />
                                                            <ext:RecordField Name="FECHAINICIO(DD/MM/AAAA)" />
                                                            <ext:RecordField Name="FECHAFIN(DD/MM/AAAA)" />
                                                            <ext:RecordField Name="TWIST" />
                                                            <ext:RecordField Name="TERRA" />
                                                            <ext:RecordField Name="PURINA" />
                                                            <ext:RecordField Name="EDENRED" />
                                                            <ext:RecordField Name="SAMS_BENEFITS" />
                                                            <ext:RecordField Name="SAMS_PLUS" />
                                                            <ext:RecordField Name="CUPONCLICK" />
                                                            <ext:RecordField Name="BOXITO" />
                                                            <ext:RecordField Name="BROXEL" />
                                                            <ext:RecordField Name="BIOBOX" />
                                                            <ext:RecordField Name="ADVANTAGE" />
                                                            <ext:RecordField Name="SIXTYNINE" />
                                                            <ext:RecordField Name="BONNUS" />
                                                            <ext:RecordField Name="SANTANDER_AFFLUENT" />
                                                            <ext:RecordField Name="CC_ROYALTY" />
                                                            <ext:RecordField Name="CC_BETS" />
                                                            <ext:RecordField Name="BENEFUL" />
                                                            <ext:RecordField Name="EDOMEX" />
                                                            <ext:RecordField Name="SMARTGIFT" />
                                                            <ext:RecordField Name="BACALAR" />
                                                            <ext:RecordField Name="MASPAMI" />
                                                            <ext:RecordField Name="AIRPAK" />
                                                            <ext:RecordField Name="PARCO" />
                                                            <ext:RecordField Name="YOURPAYCHOICE" />

                                                            <ext:RecordField Name="PRESENCIA" />
                                                            <ext:RecordField Name="CLASIFICACION" />
                                                            <ext:RecordField Name="PALABRASCLAVE" />
                                                            <ext:RecordField Name="CUENTACLABE" />
                                                            <ext:RecordField Name="NOMBRECONTACTO" />
                                                            <ext:RecordField Name="TELEFONOCONTACTO" />
                                                            <ext:RecordField Name="CARGO" />
                                                            <ext:RecordField Name="CELULARCONTACTO" />
                                                            <ext:RecordField Name="CORREO" />
                                                            <ext:RecordField Name="EXTRACTO" />
                                                            <ext:RecordField Name="SUBGIRO" />
                                                            <ext:RecordField Name="TICKETPROMEDIO" />
                                                            <ext:RecordField Name="PERFILNSE" />
                                                            <ext:RecordField Name="TIPOREDENCION" />
                                                            <ext:RecordField Name="URLCUPON" />
                                                            <ext:RecordField Name="GENERO" />
                                                            <ext:RecordField Name="PROMOPLUS" />
                                                            <ext:RecordField Name="RANGOEDAD" />
                                                            <ext:RecordField Name="TIPOESTABLECIMIENTO" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Store>
                                        <ColumnModel ID="ColumnModel12" runat="server">
                                            <Columns>
                                                <ext:Column ColumnID="CLAVECADENA" Header="Clave Cadena" Sortable="true" DataIndex="CLAVECADENA" />
                                                <ext:Column ColumnID="CADENA" Header="Cadena" Sortable="true" DataIndex="CADENA" />
                                                <ext:Column ColumnID="GIRO" Header="Giro" Sortable="true" DataIndex="GIRO" />
                                                <ext:Column ColumnID="CLAVEPROMO" Header="Clave Promoción" Sortable="true" DataIndex="CLAVEPROMO" />
                                                <ext:Column ColumnID="TITULOPROMOCION" Header="Título Promoción" Sortable="true" DataIndex="TITULOPROMOCION" />
                                                <ext:Column ColumnID="TIPODESCUENTO" Header="Tipo Descuento" Sortable="true" DataIndex="TIPODESCUENTO" />
                                                <ext:Column ColumnID="DESCRIPCIONBENEFICIO" Header="Descripción Beneficio" Sortable="true" DataIndex="DESCRIPCIONBENEFICIO" />
                                                <ext:Column ColumnID="RESTRICCIONES" Header="Restricciones" Sortable="true" DataIndex="RESTRICCIONES" />
                                                <ext:Column ColumnID="FACEBOOK" Header="Facebook" Sortable="true" DataIndex="FACEBOOK" />
                                                <ext:Column ColumnID="WEB" Header="Web" Sortable="true" DataIndex="WEB" />
                                                <ext:Column ColumnID="ESHOTDEAL" Header="EsHotDeal" Sortable="true" DataIndex="ESHOTDEAL" />
                                                <ext:Column ColumnID="CARRUSELHOME" Header="CarruselHome" Sortable="true" DataIndex="CARRUSELHOME" />
                                                <ext:Column ColumnID="PROMOHOME" Header="PromoHome" Sortable="true" DataIndex="PROMOHOME" />
                                                <ext:Column ColumnID="ORDEN" Header="Orden" Sortable="true" DataIndex="ORDEN" />
                                                <ext:Column ColumnID="ACTIVA" Header="Activa" Sortable="true" DataIndex="ACTIVA" />
                                                <ext:DateColumn ColumnID="FECHAINICIO(DD/MM/AAAA)" Header="Fecha Inicio" Sortable="true" DataIndex="FECHAINICIO(DD/MM/AAAA)"
                                                    Format="dd/MM/yyyy" />
                                                <ext:DateColumn ColumnID="FECHAFIN(DD/MM/AAAA)" Header="Fecha Fin" Sortable="true" DataIndex="FECHAFIN(DD/MM/AAAA)"
                                                    Format="dd/MM/yyyy" />
                                                <ext:Column ColumnID="TWIST" Header="Twist" Sortable="true" DataIndex="TWIST" />
                                                <ext:Column ColumnID="TERRA" Header="Terra" Sortable="true" DataIndex="TERRA" />
                                                <ext:Column ColumnID="PURINA" Header="Purina" Sortable="true" DataIndex="PURINA" />
                                                <ext:Column ColumnID="EDENRED" Header="Edenred" Sortable="true" DataIndex="EDENRED" />
                                                <ext:Column ColumnID="SAMS_BENEFITS" Header="Sam's Benefit's" Sortable="true" DataIndex="SAMS_BENEFITS" />
                                                <ext:Column ColumnID="SAMS_PLUS" Header="Sam's Plus" Sortable="true" DataIndex="SAMS_PLUS" />
                                                <ext:Column ColumnID="CUPONCLICK" Header="CuponClick" Sortable="true" DataIndex="CUPONCLICK" />
                                                <ext:Column ColumnID="BOXITO" Header="Boxito" Sortable="true" DataIndex="BOXITO" />
                                                <ext:Column ColumnID="BROXEL" Header="Broxel" Sortable="true" DataIndex="BROXEL" />
                                                <ext:Column ColumnID="BIOBOX" Header="BioBox" Sortable="true" DataIndex="BIOBOX" />
                                                <ext:Column ColumnID="ADVANTAGE" Header="Advantage" Sortable="true" DataIndex="ADVANTAGE" />
                                                <ext:Column ColumnID="SIXTYNINE" Header="Sixtynine" Sortable="true" DataIndex="SIXTYNINE" />
                                                <ext:Column ColumnID="BONNUS" Header="Bonnus" Sortable="true" DataIndex="BONNUS" />
                                                <ext:Column ColumnID="SANTANDER_AFFLUENT" Header="Santander_Affluent" Sortable="true" DataIndex="SANTANDER_AFFLUENT" />
                                                <ext:Column ColumnID="CC_ROYALTY" Header="CC_Royalty" Sortable="true" DataIndex="CC_ROYALTY" />
                                                <ext:Column ColumnID="CC_BETS" Header="CC_Bets" Sortable="true" DataIndex="CC_BETS" />
                                                <ext:Column ColumnID="BENEFUL" Header="Beneful" Sortable="true" DataIndex="BENEFUL" />
                                                <ext:Column ColumnID="EDOMEX" Header="EdoMex" Sortable="true" DataIndex="EDOMEX" />
                                                <ext:Column ColumnID="SMARTGIFT" Header="SmartGift" Sortable="true" DataIndex="SMARTGIFT" />
                                                <ext:Column ColumnID="BACALAR" Header="Bacalar" Sortable="true" DataIndex="BACALAR" />
                                                <ext:Column ColumnID="MASPAMI" Header="MasPaMi" Sortable="true" DataIndex="MASPAMI" />
                                                <ext:Column ColumnID="AIRPAK" Header="AirPak" Sortable="true" DataIndex="AIRPAK" />
                                                <ext:Column ColumnID="PARCO" Header="Parco" Sortable="true" DataIndex="PARCO" />
                                                <ext:Column ColumnID="YOURPAYCHOICE" Header="YourPayChoice" Sortable="true" DataIndex="YOURPAYCHOICE" />

                                                <ext:Column ColumnID="PRESENCIA" Header="Presencia" Sortable="true" DataIndex="PRESENCIA" Width="150" />
                                                <ext:Column ColumnID="CLASIFICACION" Header="Clasificación" Sortable="true" DataIndex="CLASIFICACION" Width="150" />
                                                <ext:Column ColumnID="PALABRASCLAVE" Header="Palabras Clave" Sortable="true" DataIndex="PALABRASCLAVE" Width="150" />
                                                <ext:Column ColumnID="CUENTACLABE" Header="Cuenta CLABE" Sortable="true" DataIndex="CUENTACLABE" Width="150" />
                                                <ext:Column ColumnID="NOMBRECONTACTO" Header="Nombre Contacto" Sortable="true" DataIndex="NOMBRECONTACTO" Width="200" />
                                                <ext:Column ColumnID="TELEFONOCONTACTO" Header="Tel Contacto" Sortable="true" DataIndex="TELEFONOCONTACTO" Width="100" />
                                                <ext:Column ColumnID="CARGO" Header="Cargo" Sortable="true" DataIndex="CARGO" Width="100" />
                                                <ext:Column ColumnID="CELULARCONTACTO" Header="Cel Contacto" Sortable="true" DataIndex="CELULARCONTACTO" Width="100" />
                                                <ext:Column ColumnID="CORREO" Header="Correo" Sortable="true" DataIndex="CORREO" Width="200" />
                                                <ext:Column ColumnID="EXTRACTO" Header="Extracto" Sortable="true" DataIndex="EXTRACTO" Width="200" />
                                                <ext:Column ColumnID="SUBGIRO" Header="SubGiro" Sortable="true" DataIndex="SUBGIRO" Width="100" />
                                                <ext:Column ColumnID="TICKETPROMEDIO" Header="TicketPromedio" Sortable="true" DataIndex="TICKETPROMEDIO" Width="100" />
                                                <ext:Column ColumnID="PERFILNSE" Header="PerfilNSE" Sortable="true" DataIndex="PERFILNSE" Width="100" />
                                                <ext:Column ColumnID="TIPOREDENCION" Header="Tipo Redención" Sortable="true" DataIndex="TIPOREDENCION" Width="100" />
                                                <ext:Column ColumnID="URLCUPON" Header="URL Cupón" Sortable="true" DataIndex="URLCUPON" Width="100" />
                                                <ext:Column ColumnID="GENERO" Header="Género" Sortable="true" DataIndex="GENERO" Width="100"/>
                                                <ext:Column ColumnID="PROMOPLUS" Header="Promo Plus" Sortable="true" DataIndex="PROMOPLUS" Width="100"/>
                                                <ext:Column ColumnID="RANGOEDAD" Header="Rangos de Edad" Sortable="true" DataIndex="RANGOEDAD" Width="100" />
                                                <ext:Column ColumnID="TIPOESTABLECIMIENTO" Header="Tipo Establecimiento" Sortable="true" DataIndex="TIPOESTABLECIMIENTO" Width="100" />
                                            </Columns>
                                        </ColumnModel>
                                        <BottomBar>
                                            <ext:Toolbar ID="Toolbar1" runat="server">
                                                <Items>
                                                    <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                    <ext:Button ID="btnAplicarCambios" runat="server" Text="Aplicar Cambios" Icon="Tick">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnAplicarCambios_Click">
                                                                <EventMask ShowMask="true" Msg="Aplicando cambios..." MinDelay="500" />
                                                            </Click>
                                                        </DirectEvents>
                                                    </ext:Button>
                                                </Items>
                                            </ext:Toolbar>
                                        </BottomBar>

                                    </ext:GridPanel>
                                </Items>
                            </ext:FormPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
