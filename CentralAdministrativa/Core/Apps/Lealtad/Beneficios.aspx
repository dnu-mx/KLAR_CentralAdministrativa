<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Beneficios.aspx.cs" 
    Inherits="Lealtad.Beneficios" %>


<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="MainBorderLayout" runat="server">
        <West Split="true">
            <ext:Panel runat="server" Width="400" Title="Búsqueda de Promociones">
                <Content>
                    <ext:BorderLayout ID="LeftBorderLayout" runat="server">
                        <North Split="true">
                            <ext:FieldSet ID="FieldSetBusqueda" runat="server" Height="120" Frame="true"
                                Border="false" Padding="2">
                                <Content>
                                    <ext:Store ID="StoreCadena" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Cadena">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Cadena" />
                                                    <ext:RecordField Name="ClaveCadena" />
                                                    <ext:RecordField Name="NombreComercial" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Content>
                                <Items>
                                     <ext:ComboBox ID="cBoxCadena" runat="server" FieldLabel="Cadena" StoreID="StoreCadena"
                                        DisplayField="NombreComercial" ValueField="ID_Cadena" Width="350" Mode="Local"
                                        AutoSelect="true" Editable="true" ForceSelection="true" TypeAhead="true" MinChars="1"
                                        MatchFieldWidth="false" Name="colCadena"/>
                                    <ext:TextField ID="txtClave" runat="server" FieldLabel="Clave" MaxLength="30" Width="350" />
                                </Items>
                                <Buttons>
                                    <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                        <DirectEvents>
                                            <Click OnEvent="btnLimpiar_Click" />
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                        <DirectEvents>
                                            <Click OnEvent="btnBuscar_Click">
                                                <EventMask ShowMask="true" Msg="Buscando Promociones..." MinDelay="500" />
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                </Buttons>
                            </ext:FieldSet>
                        </North>
                        <Center Split="true">
                            <ext:GridPanel ID="GridResultados" runat="server" AutoExpandColumn="Descripcion"
                                Title="Resultados Promociones" Layout="FitLayout">
                                <Store>
                                    <ext:Store ID="StorePromociones" runat="server" OnRefreshData="StorePromociones_Refresh">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Promocion">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Promocion" />
                                                    <ext:RecordField Name="ClavePromocion" />
                                                    <ext:RecordField Name="Cadena" />
                                                    <ext:RecordField Name="ClaveCadena" />
                                                    <ext:RecordField Name="TipoCupon" />
                                                    <ext:RecordField Name="Descripcion" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <ColumnModel runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_Promocion" Hidden="true" />
                                        <ext:Column DataIndex="Cadena" Header="Cadena" Width="120" />
                                        <ext:Column DataIndex="ClaveCadena" Hidden="true" />
                                        <ext:Column DataIndex="ClavePromocion" Header="Clave Promoción"/>
                                        <ext:Column DataIndex="Descripcion" Header="Promoción"/>
                                        <ext:Column DataIndex="TipoCupon" Hidden="true" />
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel SingleSelect="true" />
                                </SelectionModel>
                                <DirectEvents>
                                    <RowClick OnEvent="selectRowResultados_Event">
                                        <ExtraParams>
                                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridResultados}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                        </ExtraParams>
                                    </RowClick>
                                </DirectEvents>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StorePromociones" DisplayInfo="true"
                                        DisplayMsg="Promociones {0} - {1} de {2}" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>       
        </West>
        <Center Split="true">
            <ext:Panel runat="server" Layout="FitLayout" Border="false">
                <Items>
                    <ext:TabPanel runat="server">
                        <Items>
                            <ext:FormPanel ID="FormPanelConfiguracion" runat="server" Title="Configuración" LabelAlign="Left" LabelWidth="180">
                                <Content>
                                    <ext:Store ID="StoreTipoCupon" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="Id_TipoCupon">
                                                <Fields>
                                                    <ext:RecordField Name="Id_TipoCupon" />
                                                    <ext:RecordField Name="ClaveTipoCupon" />
                                                    <ext:RecordField Name="Descripcion" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Content>
                                <Items>
                                    <ext:FieldSet ID="FieldSetConfig" runat="server" Title="Configura la Promoción">
                                        <Items>
                                            <ext:Panel ID="Panel1" runat="server" Title="Configuración Actual" AutoHeight="true" FormGroup="true"
                                                Padding="5">
                                                <Items>
                                                    <ext:TextField ID="txtCadena" runat="server" FieldLabel="Cadena" Width="450" ReadOnly="true"/>
                                                    <ext:TextField ID="txtClavePromo" runat="server" FieldLabel="Clave Promoción" Width="450" ReadOnly="true"/>
                                                    <ext:TextArea ID="txtPromocion" runat="server" FieldLabel="Promoción" Width="450" ReadOnly="true"/>
                                                    <ext:TextField ID="txtTipoCupon" runat="server" FieldLabel="Tipo de Cupón" Width="450" ReadOnly="true"/>
                                                </Items>
                                            </ext:Panel>
                                            <ext:Panel ID="Panel2" runat="server" Title="Nueva Configuración" AutoHeight="true" FormGroup="true"
                                                Padding="5">
                                                <Items>
                                                    <ext:ComboBox ID="cBoxTipoCupon" runat="server" FieldLabel="Tipo de Cupón" StoreID="StoreTipoCupon" Width="450"
                                                        DisplayField="Descripcion" ValueField="Id_TipoCupon" AllowBlank="false"/>
                                                </Items>
                                            </ext:Panel>
                                        </Items>
                                        <Buttons>
                                            <ext:Button ID="btnGuardar" runat="server" Text="Guardar Cambios" Icon="Disk">
                                                <DirectEvents>
                                                    <Click OnEvent="btnGuardar_Click" Before="var valid= #{FormPanelConfiguracion}.getForm().isValid(); if (!valid) {} return valid;" />
                                                </DirectEvents>
                                            </ext:Button>
                                        </Buttons>
                                    </ext:FieldSet>
                                </Items>
                            </ext:FormPanel>
                            <ext:FormPanel ID="FormPanelGenerarCupones" runat="server" Title="Generar Cupones" LabelWidth="180">
                                <Content>
                                    <ext:Store ID="StoreTipoEmision" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_GrupoMA">
                                                <Fields>
                                                    <ext:RecordField Name="ID_GrupoMA" />
                                                    <ext:RecordField Name="ClaveGrupo" />
                                                    <ext:RecordField Name="Descripcion" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                    <ext:Store ID="StoreAlgoritmos" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_AlgoritmosCupones">
                                                <Fields>
                                                    <ext:RecordField Name="ID_AlgoritmosCupones" />
                                                    <ext:RecordField Name="ClaveAlgoritmosCupones" />
                                                    <ext:RecordField Name="Descripcion" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Content>
                                <Items>
                                    <ext:FieldSet ID="FieldSetGenerarCupones" runat="server" Title="Datos para la Generación de Cupones">
                                        <Items>
                                            <ext:TextField ID="txtClavePromocion" runat="server" FieldLabel="Clave de la Promoción" Width="450"
                                                ReadOnly="true" />
                                            <ext:NumberField ID="nfCantCupones" runat="server" FieldLabel="Cantidad de Cupones" Width="450"
                                                AllowBlank="false" MinValue="1" MaxValue="100000"/>
                                            <ext:ComboBox ID="cBoxTipoEmision" runat="server" FieldLabel="Tipo de Emisión" StoreID="StoreTipoEmision"
                                                Width="450" DisplayField="Descripcion" ValueField="ID_GrupoMA" AllowBlank="false"/>
                                            <ext:ComboBox ID="cBoxAlgoritmo" runat="server" FieldLabel="Algoritmo" StoreID="StoreAlgoritmos"
                                                Width="450" DisplayField="Descripcion" ValueField="ClaveAlgoritmosCupones" AllowBlank="false"/>
                                            <ext:NumberField ID="nfLongCupon" runat="server" FieldLabel="Longitud del Cupón" Width="450"
                                                AllowBlank="false" MinValue="1"/>
                                            <ext:DateField ID="dfExpiracionCupon" runat="server" FieldLabel="Fecha de Expiración" Width="450"
                                                AllowBlank="false" Format="dd-MMM-yyyy" MaxLength="12" EnableKeyEvents="true" Name="FechaExpiracion" />
                                            <ext:NumberField ID="nfValorCupon" runat="server" FieldLabel="Valor del Cupón" Width="450"
                                                AllowBlank="false" AllowDecimals="false" AllowNegative="false"/>
                                            <ext:NumberField ID="nfConsumos" runat="server" FieldLabel="Consumos Válidos" Width="450"
                                                AllowBlank="false" AllowDecimals="false" AllowNegative="false"/>
                                            <ext:ComboBox ID="cBoxTCupon" runat="server" FieldLabel="Tipo de Cupón" StoreID="StoreTipoCupon" Width="450"
                                                DisplayField="Descripcion" ValueField="Id_TipoCupon" AllowBlank="false"/>
                                        </Items>
                                        <Buttons>
                                            <ext:Button ID="btnGenerarCupones" runat="server" Text="Generar Cupones" Icon="Add">
                                                <DirectEvents>
                                                    <Click OnEvent="btnGenerarCupones_Click" Before="var valid= #{FormPanelGenerarCupones}.getForm().isValid(); if (!valid) {} return valid;" />
                                                </DirectEvents>
                                            </ext:Button>
                                        </Buttons>
                                    </ext:FieldSet>
                                </Items>
                            </ext:FormPanel>
                            <ext:FormPanel ID="FormPanelCargarCupones" runat="server" Title="Cargar Cupones">
                            </ext:FormPanel>
                        </Items>
                    </ext:TabPanel>
                </Items>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
