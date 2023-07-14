<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="AdministrarPromosPrana.aspx.cs" Inherits="Lealtad.AdministrarPromosPrana" ValidateRequest="false" %>

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
    <ext:Store ID="StorePromos" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_Promocion">
                <Fields>
                    <ext:RecordField Name="ID_Promocion" />
                    <ext:RecordField Name="ClavePromocion" />
                    <ext:RecordField Name="Descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Store ID="StoreCadEcom" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_Cadena">
                <Fields>
                    <ext:RecordField Name="ID_Cadena" />
                    <ext:RecordField Name="ClaveCadena" />
                    <ext:RecordField Name="NombreComercial" />
                </Fields>
            </ext:JsonReader>
        </Reader>
        <SortInfo Field="NombreComercial" Direction="ASC" />
    </ext:Store>
    <ext:Store ID="StoreClaveCadenas" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_Cadena">
                <Fields>
                    <ext:RecordField Name="ID_Cadena" />
                    <ext:RecordField Name="ClaveCadena" />
                    <ext:RecordField Name="NombreComercial" />
                </Fields>
            </ext:JsonReader>
        </Reader>
        <SortInfo Field="ClaveCadena" Direction="ASC" />
    </ext:Store>

    <ext:Window ID="WdwNuevaPromo" runat="server" Title="Alta de Promoción" Width="450" Height="460" Hidden="true"
        Modal="true" Resizable="false" Icon="Add">
        <Items>
            <ext:FormPanel ID="FormPanelNP" runat="server" Height="430" Padding="3" LabelWidth="120">
                <Items>
                    <ext:FieldSet runat="server" Title="Datos Base">
                        <Items>
                            <ext:Panel runat="server" Layout="HBoxLayout" LabelAlign="Left" BodyPadding="5" Border="false">
                                <Defaults>
                                    <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                </Defaults>
                                <LayoutConfig>
                                    <ext:HBoxLayoutConfig Align="Top" />
                                </LayoutConfig>
                                <Items>
                                    <ext:ComboBox ID="cBoxClaveCadena" runat="server" FieldLabel="Clave Cadena" Width="300" Mode="Local"
                                        StoreID="StoreClaveCadenas" DisplayField="ClaveCadena" ValueField="ID_Cadena" AllowBlank="false"
                                        Name="claveCad" AutoSelect="true" ForceSelection="true" TypeAhead="true" MinChars="1" MatchFieldWidth="false">
                                        <Listeners>
                                            <Select Handler="#{cBoxCadena}.setValue(this.getValue());" />
                                        </Listeners>
                                    </ext:ComboBox>
                                    <ext:Hidden runat="server" Width="20" Flex="1" />
                                    <ext:LinkButton runat="server" Text="Nueva Cadena.." Width="100">
                                        <Listeners>
                                            <Click Handler="#{WdwNuevaCadena}.show();" />
                                        </Listeners>
                                    </ext:LinkButton>
                                </Items>
                            </ext:Panel>
                            <ext:Panel runat="server" Layout="FitLayout" Width="400" Height="5" Border="false" />
                            <ext:ComboBox ID="cBoxCadena" runat="server" FieldLabel="Cadena" Width="400" StoreID="StoreCadEcom"
                                DisplayField="NombreComercial" ValueField="ID_Cadena" Mode="Local" AllowBlank="false" Name="cadenas"
                                AutoSelect="true" ForceSelection="true" TypeAhead="true" MinChars="1" MatchFieldWidth="false">
                                <Listeners>
                                    <Select Handler="#{cBoxClaveCadena}.setValue(this.getValue());" />
                                </Listeners>
                            </ext:ComboBox>
                            <ext:TextField ID="txtClavePromo_Alta" runat="server" FieldLabel="Clave Promoción" MaxLength="50"
                                Width="400" AllowBlank="false"/>
                            <ext:TextArea ID="txtDescripcionPromo" runat="server" FieldLabel="Descripción" MaxLength="5000"
                                Width="400" Height="130" AllowBlank="false"/>
                            <ext:TextArea ID="txtPalabrasClave" runat="server" FieldLabel="Plabras Clave" MaxLength="1500"
                                Width="400" Height="130" AllowBlank="true"/>
                        </Items>
                    </ext:FieldSet>
                </Items>
                <Buttons>
                    <ext:Button runat="server" Text="Cancelar" Icon="Cancel">
                        <Listeners>
                            <Click Handler="#{WdwNuevaPromo}.hide();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button ID="btnNuevaPromo" runat="server" Text="Crear Promoción" Icon="Tick">
                        <DirectEvents>
                            <Click OnEvent="btnNuevaPromo_Click" Before="var valid= #{FormPanelNP}.getForm().isValid(); if (!valid) {} return valid;">
                                <EventMask ShowMask="true" Msg="Creando Promoción..." MinDelay="500" />
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    <ext:Window ID="WdwNuevaCadena" runat="server" Title="Alta de Cadena" Width="350" Height="440" Hidden="true"
        Modal="true" Resizable="false" Icon="LinkAdd">
        <Items>
            <ext:FormPanel ID="FormPanelNC" runat="server" Height="410" Padding="3" LabelWidth="120">
                <Items>
                    <ext:FieldSet runat="server" Title="Datos de la Nueva Cadena">
                        <Items>
                            <ext:TextField ID="txtClaveCadena" runat="server" FieldLabel="Clave Cadena" MaxLength="100"
                                Width="300" AllowBlank="false"/>
                            <ext:TextField ID="txtCadena" runat="server" FieldLabel="Nombre Comercial" MaxLength="200"
                                Width="300" AllowBlank="false"/>
                            <ext:ComboBox ID="cBoxGiro" runat="server" FieldLabel="Giro" Width="300" AllowBlank="false"
                                DisplayField="Descripcion" ValueField="ID_Giro">
                                <Store>
                                    <ext:Store ID="StoreGiros" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Giro">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Giro" />
                                                    <ext:RecordField Name="Clave" />
                                                    <ext:RecordField Name="Descripcion" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cBoxPresencia" runat="server" FieldLabel="Presencia" Width="300"
                                AllowBlank="false" DisplayField="Descripcion" ValueField="ID_Presencia">
                                 <Store>
                                    <ext:Store ID="StorePresencia" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Presencia">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Presencia" />
                                                    <ext:RecordField Name="Clave" />
                                                    <ext:RecordField Name="Descripcion" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                            </ext:ComboBox>
                            <ext:TextArea ID="txtFacebook" runat="server" FieldLabel="Facebook" MaxLength="250"
                                Width="300" Height="100"/>
                            <ext:TextArea ID="txtWeb" runat="server" FieldLabel="Web" MaxLength="250"
                                Width="300" Height="100"/>
                        </Items>
                    </ext:FieldSet>
                </Items>
                <Buttons>
                    <ext:Button runat="server" Text="Cancelar" Icon="Cancel">
                        <Listeners>
                            <Click Handler="#{WdwNuevaCadena}.hide();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button ID="btnAddCadena" runat="server" Text="Crear Cadena" Icon="Tick">
                        <DirectEvents>
                            <Click OnEvent="btnAddCadena_Click" Before="var valid= #{FormPanelNC}.getForm().isValid(); if (!valid) {} return valid;">
                                <EventMask ShowMask="true" Msg="Creando Cadena..." MinDelay="500" />
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
                               
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true">
            <ext:Panel runat="server" Width="325" Border="false" Layout="FitLayout" Title="Consulta">
                <Content>
                    <ext:BorderLayout ID="LeftBorderLayout" runat="server">
                        <South Split="true">
                            <ext:FormPanel ID="FormPanel3" runat="server" Height="25" Border="false">
                                <Items>
                                    <ext:Toolbar runat="server">
                                        <Items>
                                            <ext:Button ID="btnLimpiarIzq" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                                <DirectEvents>
                                                    <Click OnEvent="btnLimpiarIzq_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:ToolbarFill runat="server" />
                                            <ext:Button runat="server" Icon="Add" ToolTip="Crear nueva promoción"
                                                Text="Nueva Promoción">
                                                <Listeners>
                                                    <Click Handler="#{FormPanelNP}.reset(); #{WdwNuevaPromo}.show();" />
                                                </Listeners>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </Items>
                            </ext:FormPanel>
                        </South>
                        <Center Split="true">
                            <ext:GridPanel ID="GridResultados" runat="server" AutoExpandColumn="Descripcion" 
                                StoreID="StorePromos" Border="false" Header="false">
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar2" runat="server">
                                        <Items>
                                            <ext:TextField ID="txtClavePromo" runat="server" EmptyText="Clave Promoción" Width="100"/>
                                            <ext:TextField ID="txtPromocion" runat="server" EmptyText="Descripción" />
                                            <ext:Button ID="btnBuscarPromo" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnBuscar_Click" Before="#{PanelCentral}.setTitle('');
                                                        #{PanelCentral}.setDisabled(true);
                                                        if (!#{txtClavePromo}.getValue() && !#{txtPromocion}.getValue())
                                                        { return false; }">
                                                        <EventMask ShowMask="true" Msg="Buscando Promociones..." MinDelay="500" />
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <ColumnModel runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_Promocion" Hidden="true" />
                                        <ext:Column DataIndex="ClavePromocion" Header="Clave" Width="90" />
                                        <ext:Column DataIndex="Descripcion" Header="Descripción Beneficio" Width="110" />
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel SingleSelect="true" />
                                </SelectionModel>
                                <DirectEvents>
                                    <RowClick OnEvent="selectRowResultados_Event">
                                        <EventMask ShowMask="true" Msg="Obteniendo Información de la Promoción..." MinDelay="500" />
                                        <ExtraParams>
                                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridResultados}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                        </ExtraParams>
                                    </RowClick>
                                </DirectEvents>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StorePromos" DisplayInfo="true"
                                        DisplayMsg="{0} - {1} de {2}" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </West>
        <Center Split="true">
            <ext:Panel ID="PanelCentral" runat="server" Height="250" Title="_" Border="false" Disabled="true">
                <Items>
                    <ext:BorderLayout runat="server">
                        <Center>
                            <ext:TabPanel runat="server">
                                <Items>
                                    <ext:FormPanel ID="FormPanelInfoAd" runat="server" Title="Información Adicional" LabelAlign="Right" LabelWidth="120"
                                        AutoScroll="true">
                                        <Items>
                                            <ext:Hidden ID="hdnIdPromocion" runat="server" />
                                            <ext:FieldSet ID="FieldSetDatosAd" runat="server" Title="Datos Adicionales de la Promoción">
                                                <Items>
                                                    <ext:Panel runat="server" Layout="HBoxLayout" LabelAlign="Left" BodyPadding="5" Border="false">
                                                        <Defaults>
                                                            <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                        </Defaults>
                                                        <LayoutConfig>
                                                            <ext:HBoxLayoutConfig Align="Top" />
                                                        </LayoutConfig>
                                                        <Items>
                                                            <ext:TextField ID="txtClaveCad" runat="server" FieldLabel="Clave Cadena" Width="260" Disabled="true" />
                                                            <ext:Hidden runat="server" Width="30" Flex="1" />
                                                            <ext:TextField ID="txtClaveProm" runat="server" FieldLabel="Clave Promoción" Width="260" Disabled="true" />
                                                        </Items>
                                                    </ext:Panel>
                                                    <ext:Panel runat="server" Layout="FitLayout" Width="550" Height="5" Border="false" />
                                                    <ext:TextField ID="txtCad" runat="server" FieldLabel="Cadena" Width="550" Disabled="true" />
                                                    <ext:TextField ID="txtTituloPromo" runat="server" FieldLabel="Título Promoción" MaxLength="500"
                                                        Width="550" />
                                                    <ext:TextField ID="txtTipoDescuento" runat="server" FieldLabel="Tipo de Descuento" MaxLength="500"
                                                        Width="550"/>
                                                    <ext:TextArea ID="txtDescBenef" runat="server" FieldLabel="Descripción Beneficio" MaxLength="5000"
                                                         Height="150" AllowBlank="false" Width="550" />
                                                    <ext:TextArea ID="txtRestricciones" runat="server" FieldLabel="Restricciones" MaxLength="5000" 
                                                        Width="550" Height="150"/>
                                                    <ext:TextArea ID="txtPalabraClaveInfo" runat="server" FieldLabel="Palabras Clave" MaxLength="1500" 
                                                        Width="550" Height="150"/>
                                                    
                                                    <ext:TextArea ID="txtURLCupon" runat="server" FieldLabel="URL Cupón" MaxLength="1000" AllowBlank ="true"
                                                        Width="550" Height="150"/>

                                                    <ext:Panel runat="server" Layout="HBoxLayout" LabelAlign="Left" BodyPadding="5" Border="false">
                                                        <Defaults>
                                                            <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                        </Defaults>
                                                        <LayoutConfig>
                                                            <ext:HBoxLayoutConfig Align="Top" />
                                                        </LayoutConfig>
                                                        <Items>
                                                            <ext:ComboBox ID="cBoxHotDeal" runat="server" FieldLabel="EsHotDeal" Width="260"
                                                                AllowBlank="false">
                                                                <Items>
                                                                    <ext:ListItem Text="1" Value="1" />
                                                                    <ext:ListItem Text="0" Value="0" />
                                                                </Items>
                                                            </ext:ComboBox>
                                                            <ext:Hidden runat="server" Width="30" Flex="1" />
                                                            <ext:TextField ID="txtCarrusel" runat="server" FieldLabel="CarruselHome" MaxLength="10"
                                                                Width="260" MaskRe="[0-9]" AllowBlank="false" />
                                                        </Items>
                                                    </ext:Panel>
                                                    <ext:Panel runat="server" Layout="FitLayout" Width="550" Height="5" Border="false" />
                                                    <ext:Panel runat="server" Layout="HBoxLayout" LabelAlign="Left" BodyPadding="5" Border="false">
                                                        <Defaults>
                                                            <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                        </Defaults>
                                                        <LayoutConfig>
                                                            <ext:HBoxLayoutConfig Align="Top" />
                                                        </LayoutConfig>
                                                        <Items>
                                                            <ext:TextField ID="txtPromoHome" runat="server" FieldLabel="PromoHome" MaxLength="10" Width="260"
                                                                MaskRe="[0-9]" AllowBlank="false" />
                                                            <ext:Hidden runat="server" Width="30" Flex="1" />
                                                            <ext:TextField ID="txtOrden" runat="server" FieldLabel="Orden" MaxLength="10" Width="260"
                                                                MaskRe="[0-9]" AllowBlank="false"/>
                                                        </Items>
                                                    </ext:Panel>
                                                    <ext:Panel runat="server" Layout="FitLayout" Width="550" Height="5" Border="false" />
                                                    <ext:Panel runat="server" Layout="HBoxLayout" LabelAlign="Left" BodyPadding="5" Border="false">
                                                        <Defaults>
                                                            <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                        </Defaults>
                                                        <LayoutConfig>
                                                            <ext:HBoxLayoutConfig Align="Top" />
                                                        </LayoutConfig>
                                                        <Items>
                                                            <ext:DateField ID="dfFIniVigPromo" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial Vigencia"
                                                                Format="dd/MM/yyyy" Width="260" EnableKeyEvents="true" AllowBlank="false">
                                                                <CustomConfig>
                                                                    <ext:ConfigItem Name="endDateField" Value="#{dfFFinVigPromo}" Mode="Value" />
                                                                </CustomConfig>
                                                                <Listeners>
                                                                    <KeyUp Fn="onKeyUp" />
                                                                </Listeners>
                                                            </ext:DateField>
                                                            <ext:Hidden runat="server" Width="30" Flex="1" />
                                                            <ext:DateField ID="dfFFinVigPromo" runat="server" Vtype="daterange" FieldLabel="Fecha Final Vigencia"
                                                                Width="260" Format="dd/MM/yyyy" EnableKeyEvents="true" AllowBlank="false">
                                                                <CustomConfig>
                                                                    <ext:ConfigItem Name="startDateField" Value="#{dfFIniVigPromo}" Mode="Value" />
                                                                </CustomConfig>
                                                                <Listeners>
                                                                    <KeyUp Fn="onKeyUp" />
                                                                </Listeners>
                                                            </ext:DateField>
                                                        </Items>
                                                    </ext:Panel>

                                                    <ext:Panel runat="server" Layout="FitLayout" Width="550" Height="5" Border="false" />
                                                    <ext:Panel runat="server" Layout="HBoxLayout" LabelAlign="Left" BodyPadding="5" Border="false">
                                                        <Defaults>
                                                            <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                        </Defaults>
                                                        <LayoutConfig>
                                                            <ext:HBoxLayoutConfig Align="Top" />
                                                        </LayoutConfig>
                                                        <Items>
                                                            <ext:ComboBox ID="cBoxActiva" runat="server" FieldLabel="Activa" Width="260" AllowBlank="false">
                                                                <Items>
                                                                    <ext:ListItem Text="Sí" Value="1" />
                                                                    <ext:ListItem Text="No" Value="0" />
                                                                </Items>
                                                            </ext:ComboBox>
                                                            <ext:Hidden runat="server" Width="30" Flex="1" />
                                                            <ext:ComboBox ID="cBoxClasificacion" runat="server" FieldLabel="Clasificación" Width="260"
                                                                AllowBlank="false" DisplayField="Clave" ValueField="ID_Clasificacion">
                                                                <Store>
                                                                    <ext:Store ID="StoreClasif" runat="server">
                                                                        <Reader>
                                                                            <ext:JsonReader IDProperty="ID_Clasificacion">
                                                                                <Fields>
                                                                                    <ext:RecordField Name="ID_Clasificacion" />
                                                                                    <ext:RecordField Name="Clave" />
                                                                                    <ext:RecordField Name="Descripcion" />
                                                                                </Fields>
                                                                            </ext:JsonReader>
                                                                        </Reader>
                                                                    </ext:Store>
                                                                </Store>
                                                            </ext:ComboBox>
                                                        </Items>
                                                    </ext:Panel>


                                                    <ext:Panel runat="server" Layout="FitLayout" Width="550" Height="5" Border="false" />
                                                    <ext:Panel runat="server" Layout="HBoxLayout" LabelAlign="Left" BodyPadding="5" Border="false">
                                                        <Items>
                                                            <ext:ComboBox ID="cBoxGenero" runat="server" FieldLabel="Genero" Width="260"
                                                                AllowBlank="true" DisplayField="Descripcion" ValueField="Id_Genero">
                                                                <Store>
                                                                    <ext:Store ID="StoreGenero" runat="server">
                                                                        <Reader>
                                                                            <ext:JsonReader IDProperty="Id_Genero">
                                                                                <Fields>
                                                                                    <ext:RecordField Name="Id_Genero" />
                                                                                    <ext:RecordField Name="Descripcion" />
                                                                                </Fields>
                                                                            </ext:JsonReader>
                                                                        </Reader>
                                                                    </ext:Store>
                                                                </Store>
                                                            </ext:ComboBox>
                                                            
                                                            <ext:Hidden runat="server" Width="30" Flex="1" />
                                                            <ext:MultiCombo ID="cBoxMultiRandoEdad" runat="server" FieldLabel="Rango de Edad" Width="260"
                                                                AllowBlank="true" DisplayField="Descripcion" ValueField="Id_RangoEdad" ReadOnly="false" Selectable="true" >
                                                                <Store>
                                                                    <ext:Store ID="StoreRangoEdad" runat="server">
                                                                        <Reader>
                                                                            <ext:JsonReader IDProperty="Id_RangoEdad">
                                                                                <Fields>
                                                                                    <ext:RecordField Name="Id_RangoEdad" />
                                                                                    <ext:RecordField Name="Clave" />
                                                                                    <ext:RecordField Name="Descripcion" />
                                                                                </Fields>
                                                                            </ext:JsonReader>
                                                                        </Reader>
                                                                    </ext:Store>
                                                                </Store>
                                                            </ext:MultiCombo>
                                                        </Items>
                                                    </ext:Panel>

                                                    <ext:Panel runat="server" Layout="FitLayout" Width="550" Height="5" Border="false" />
                                                    <ext:Panel runat="server" Layout="HBoxLayout" LabelAlign="Left" BodyPadding="5" Border="false">
                                                        <Items>
                                                            <ext:ComboBox ID="cBoxTipoRedencion" runat="server" FieldLabel="Tipo de Redención" Width="260"
                                                                AllowBlank="true" DisplayField="Descripcion" ValueField="Id_TipoRedencion">
                                                                <Store>
                                                                    <ext:Store ID="StoreTipoRedencion" runat="server">
                                                                        <Reader>
                                                                            <ext:JsonReader IDProperty="Id_TipoRedencion">
                                                                                <Fields>
                                                                                    <ext:RecordField Name="Id_TipoRedencion" />
                                                                                    <ext:RecordField Name="Clave" />
                                                                                    <ext:RecordField Name="Descripcion" />
                                                                                </Fields>
                                                                            </ext:JsonReader>
                                                                        </Reader>
                                                                    </ext:Store>
                                                                </Store>
                                                            </ext:ComboBox>
                                                            <ext:Hidden runat="server" Width="30" Flex="1" />
                                                            <ext:ComboBox ID="cBoxPromocionPlus" runat="server" FieldLabel="Promoción Plus" Width="260"
                                                                AllowBlank="true" DisplayField="Descripcion" ValueField="Id_PromoPlus">
                                                                <Store>
                                                                    <ext:Store ID="StorePromoPlus" runat="server">
                                                                        <Reader>
                                                                            <ext:JsonReader IDProperty="Id_PromoPlus">
                                                                                <Fields>
                                                                                    <ext:RecordField Name="Id_PromoPlus" />
                                                                                    <ext:RecordField Name="Clave" />
                                                                                    <ext:RecordField Name="Descripcion" />
                                                                                </Fields>
                                                                            </ext:JsonReader>
                                                                        </Reader>
                                                                    </ext:Store>
                                                                </Store>
                                                            </ext:ComboBox>
                                                        </Items>
                                                    </ext:Panel>

                                                </Items>
                                                <Buttons>
                                                    <ext:Button ID="btnGuardarInfoAd" runat="server" Text="Guardar" Icon="Disk">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnGuardarInfoAd_Click" Before="var valid= #{FormPanelInfoAd}.getForm().isValid(); if (!valid) {} return valid;">
                                                                <EventMask ShowMask="true" Msg="Guardando Cambios..." MinDelay="500" />
                                                            </Click>
                                                        </DirectEvents>
                                                    </ext:Button>
                                                </Buttons>
                                            </ext:FieldSet>
                                        </Items>
                                    </ext:FormPanel>
                                    <ext:FormPanel ID="FormPanelProgramas" runat="server" Title="Programas" LabelAlign="Right" LabelWidth="110">
                                        <Items>
                                            <ext:FieldSet ID="FieldSetProgramas" runat="server" Title="Programas de la Promoción">
                                                <Items>
                                                    <ext:Panel runat="server" Layout="HBoxLayout" LabelAlign="Left" BodyPadding="5" Border="false">
                                                        <Defaults>
                                                            <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                        </Defaults>
                                                        <LayoutConfig>
                                                            <ext:HBoxLayoutConfig Align="Top" />
                                                        </LayoutConfig>
                                                        <Items>
                                                            <ext:ComboBox ID="cBoxTwist" runat="server" FieldLabel="Twist" Width="180" AllowBlank="false"
                                                                SelectedIndex="1">
                                                                <Items>
                                                                    <ext:ListItem Text="Sí" Value="1" />
                                                                    <ext:ListItem Text="No" Value="0" />
                                                                </Items>
                                                            </ext:ComboBox>
                                                            <ext:Hidden runat="server" Width="5" Flex="1" />
                                                            <ext:ComboBox ID="cBoxTerra" runat="server" FieldLabel="Terra" Width="180" AllowBlank="false"
                                                                SelectedIndex="1">
                                                                <Items>
                                                                    <ext:ListItem Text="Sí" Value="1" />
                                                                    <ext:ListItem Text="No" Value="0" />
                                                                </Items>
                                                            </ext:ComboBox>
                                                            <ext:Hidden runat="server" Width="5" Flex="2" />
                                                            <ext:ComboBox ID="cBoxPurina" runat="server" FieldLabel="Purina" Width="180" AllowBlank="false"
                                                                SelectedIndex="1">
                                                                <Items>
                                                                    <ext:ListItem Text="Sí" Value="1" />
                                                                    <ext:ListItem Text="No" Value="0" />
                                                                </Items>
                                                            </ext:ComboBox>
                                                        </Items>
                                                    </ext:Panel>
                                                    <ext:Panel runat="server" Layout="FitLayout" Width="550" Height="5" Border="false" />
                                                    <ext:Panel runat="server" Layout="HBoxLayout" LabelAlign="Left" BodyPadding="5" Border="false">
                                                        <Defaults>
                                                            <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                        </Defaults>
                                                        <LayoutConfig>
                                                            <ext:HBoxLayoutConfig Align="Top" />
                                                        </LayoutConfig>
                                                        <Items>
                                                            <ext:ComboBox ID="cBoxEdenred" runat="server" FieldLabel="Edenred" Width="180" AllowBlank="false"
                                                                SelectedIndex="1">
                                                                 <Items>
                                                                    <ext:ListItem Text="Sí" Value="1" />
                                                                    <ext:ListItem Text="No" Value="0" />
                                                                </Items>
                                                            </ext:ComboBox>
                                                            <ext:Hidden runat="server" Width="5" Flex="1" />
                                                            <ext:ComboBox ID="cBoxSams_Benefits" runat="server" FieldLabel="Sams_Benefits" Width="180" AllowBlank="false"
                                                                SelectedIndex="1">
                                                                <Items>
                                                                    <ext:ListItem Text="Sí" Value="1" />
                                                                    <ext:ListItem Text="No" Value="0" />
                                                                </Items>
                                                            </ext:ComboBox>
                                                            <ext:Hidden runat="server" Width="5" Flex="2" />
                                                            <ext:ComboBox ID="cBoxSams_Plus" runat="server" FieldLabel="Sams_Plus" Width="180" AllowBlank="false"
                                                                SelectedIndex="1">
                                                                <Items>
                                                                    <ext:ListItem Text="Sí" Value="1" />
                                                                    <ext:ListItem Text="No" Value="0" />
                                                                </Items>
                                                            </ext:ComboBox>
                                                        </Items>
                                                    </ext:Panel>
                                                    <ext:Panel runat="server" Layout="FitLayout" Width="180" Height="5" Border="false" />
                                                    <ext:Panel runat="server" Layout="HBoxLayout" LabelAlign="Left" BodyPadding="5" Border="false">
                                                        <Defaults>
                                                            <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                        </Defaults>
                                                        <LayoutConfig>
                                                            <ext:HBoxLayoutConfig Align="Top" />
                                                        </LayoutConfig>
                                                        <Items>
                                                            <ext:ComboBox ID="cBoxCuponClick" runat="server" FieldLabel="CuponClick" Width="180" AllowBlank="false"
                                                                SelectedIndex="1">
                                                                <Items>
                                                                    <ext:ListItem Text="Sí" Value="1" />
                                                                    <ext:ListItem Text="No" Value="0" />
                                                                </Items>
                                                            </ext:ComboBox>
                                                            <ext:Hidden runat="server" Width="5" Flex="1" />
                                                            <ext:ComboBox ID="cBoxBoxito" runat="server" FieldLabel="Boxito" Width="180" AllowBlank="false"
                                                                SelectedIndex="1">
                                                                <Items>
                                                                    <ext:ListItem Text="Sí" Value="1" />
                                                                    <ext:ListItem Text="No" Value="0" />
                                                                </Items>
                                                            </ext:ComboBox>
                                                            <ext:Hidden runat="server" Width="5" Flex="2" />
                                                            <ext:ComboBox ID="cBoxBroxel" runat="server" FieldLabel="Broxel" Width="180" AllowBlank="false"
                                                                SelectedIndex="1">
                                                                <Items>
                                                                    <ext:ListItem Text="Sí" Value="1" />
                                                                    <ext:ListItem Text="No" Value="0" />
                                                                </Items>
                                                            </ext:ComboBox>
                                                        </Items>
                                                    </ext:Panel>
                                                    <ext:Panel runat="server" Layout="FitLayout" Width="180" Height="5" Border="false" />
                                                    <ext:Panel runat="server" Layout="HBoxLayout" LabelAlign="Left" BodyPadding="5" Border="false">
                                                        <Defaults>
                                                            <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                        </Defaults>
                                                        <LayoutConfig>
                                                            <ext:HBoxLayoutConfig Align="Top" />
                                                        </LayoutConfig>
                                                        <Items>
                                                            <ext:ComboBox ID="cBoxBioBox" runat="server" FieldLabel="BioBox" Width="180" AllowBlank="false"
                                                                SelectedIndex="1">
                                                                <Items>
                                                                    <ext:ListItem Text="Sí" Value="1" />
                                                                    <ext:ListItem Text="No" Value="0" />
                                                                </Items>
                                                            </ext:ComboBox>
                                                            <ext:Hidden runat="server" Width="5" Flex="1" />
                                                            <ext:ComboBox ID="cBoxAdvantage" runat="server" FieldLabel="Advantage" Width="180" AllowBlank="false"
                                                                SelectedIndex="1">
                                                                <Items>
                                                                    <ext:ListItem Text="Sí" Value="1" />
                                                                    <ext:ListItem Text="No" Value="0" />
                                                                </Items>
                                                            </ext:ComboBox>
                                                            <ext:Hidden runat="server" Width="5" Flex="2" />
                                                            <ext:ComboBox ID="cBoxSixtynine" runat="server" FieldLabel="Sixtynine" Width="180" AllowBlank="false"
                                                                SelectedIndex="1">
                                                                <Items>
                                                                    <ext:ListItem Text="Sí" Value="1" />
                                                                    <ext:ListItem Text="No" Value="0" />
                                                                </Items>
                                                            </ext:ComboBox>
                                                        </Items>
                                                    </ext:Panel>
                                                    <ext:Panel runat="server" Layout="FitLayout" Width="180" Height="5" Border="false" />
                                                    <ext:Panel runat="server" Layout="HBoxLayout" LabelAlign="Left" BodyPadding="5" Border="false">
                                                        <Defaults>
                                                            <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                        </Defaults>
                                                        <LayoutConfig>
                                                            <ext:HBoxLayoutConfig Align="Top" />
                                                        </LayoutConfig>
                                                        <Items>
                                                            <ext:ComboBox ID="cBoxBonnus" runat="server" FieldLabel="Bonnus" Width="180" AllowBlank="false"
                                                                SelectedIndex="1">
                                                                <Items>
                                                                    <ext:ListItem Text="Sí" Value="1" />
                                                                    <ext:ListItem Text="No" Value="0" />
                                                                </Items>
                                                            </ext:ComboBox>
                                                            <ext:Hidden runat="server" Width="5" Flex="1" />
                                                            <ext:ComboBox ID="cBoxSantander_Affluent" runat="server" FieldLabel="Santander_Affluent" Width="180"
                                                                AllowBlank="false" SelectedIndex="1">
                                                                <Items>
                                                                    <ext:ListItem Text="Sí" Value="1" />
                                                                    <ext:ListItem Text="No" Value="0" />
                                                                </Items>
                                                            </ext:ComboBox>
                                                            <ext:Hidden runat="server" Width="5" Flex="2" />
                                                            <ext:ComboBox ID="cBoxCC_Royalty" runat="server" FieldLabel="CC_Royalty" Width="180" AllowBlank="false"
                                                                SelectedIndex="1">
                                                                <Items>
                                                                    <ext:ListItem Text="Sí" Value="1" />
                                                                    <ext:ListItem Text="No" Value="0" />
                                                                </Items>
                                                            </ext:ComboBox>
                                                        </Items>
                                                    </ext:Panel>
                                                    <ext:Panel runat="server" Layout="FitLayout" Width="180" Height="5" Border="false" />
                                                    <ext:Panel runat="server" Layout="HBoxLayout" LabelAlign="Left" BodyPadding="5" Border="false">
                                                        <Defaults>
                                                            <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                        </Defaults>
                                                        <LayoutConfig>
                                                            <ext:HBoxLayoutConfig Align="Top" />
                                                        </LayoutConfig>
                                                        <Items>
                                                            <ext:ComboBox ID="cBoxCC_Bets" runat="server" FieldLabel="CC_Bets" Width="180" AllowBlank="false"
                                                                SelectedIndex="1">
                                                                <Items>
                                                                    <ext:ListItem Text="Sí" Value="1" />
                                                                    <ext:ListItem Text="No" Value="0" />
                                                                </Items>
                                                            </ext:ComboBox>
                                                            <ext:Hidden runat="server" Width="5" Flex="1" />
                                                            <ext:ComboBox ID="cBoxBeneful" runat="server" FieldLabel="Beneful" Width="180" AllowBlank="false"
                                                                SelectedIndex="1">
                                                                <Items>
                                                                    <ext:ListItem Text="Sí" Value="1" />
                                                                    <ext:ListItem Text="No" Value="0" />
                                                                </Items>
                                                            </ext:ComboBox>
                                                            <ext:Hidden runat="server" Width="5" Flex="2" />
                                                            <ext:ComboBox ID="cBoxEdoMex" runat="server" FieldLabel="EdoMex" Width="180" AllowBlank="false"
                                                                SelectedIndex="1">
                                                                <Items>
                                                                    <ext:ListItem Text="Sí" Value="1" />
                                                                    <ext:ListItem Text="No" Value="0" />
                                                                </Items>
                                                            </ext:ComboBox>
                                                            <ext:Hidden runat="server" Width="5" Flex="2" />
                                                        </Items>
                                                    </ext:Panel>

                                                    <ext:Panel runat="server" Layout="FitLayout" Width="180" Height="5" Border="false" />
                                                    <ext:Panel runat="server" Layout="HBoxLayout" LabelAlign="Left" BodyPadding="5" Border="false">
                                                        <Defaults>
                                                            <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                        </Defaults>
                                                        <LayoutConfig>
                                                            <ext:HBoxLayoutConfig Align="Top" />
                                                        </LayoutConfig>
                                                        <Items>
                                                            <ext:ComboBox ID="cBoxSmartGift" runat="server" FieldLabel="Smartgift" Width="180" AllowBlank="false"
                                                                SelectedIndex="1">
                                                                <Items>
                                                                    <ext:ListItem Text="Sí" Value="1" />
                                                                    <ext:ListItem Text="No" Value="0" />
                                                                </Items>
                                                            </ext:ComboBox>
                                                            <ext:Hidden runat="server" Width="5" Flex="1" />
                                                            <ext:ComboBox ID="cBoxBacalar" runat="server" FieldLabel="Bacalar" Width="180" AllowBlank="false"
                                                                SelectedIndex="1">
                                                                <Items>
                                                                    <ext:ListItem Text="Sí" Value="1" />
                                                                    <ext:ListItem Text="No" Value="0" />
                                                                </Items>
                                                            </ext:ComboBox>
                                                            <ext:Hidden runat="server" Width="5" Flex="1" />
                                                            <ext:ComboBox ID="cBoxMasPaMi" runat="server" FieldLabel="MasPaMi" Width="180" AllowBlank="false"
                                                                SelectedIndex="1">
                                                                <Items>
                                                                    <ext:ListItem Text="Sí" Value="1" />
                                                                    <ext:ListItem Text="No" Value="0" />
                                                                </Items>
                                                            </ext:ComboBox>
                                                            <ext:Hidden runat="server" Width="5" Flex="2" />                                                           
                                                        </Items>
                                                    </ext:Panel>

                                                    <ext:Panel runat="server" Layout="FitLayout" Width="180" Height="5" Border="false" />
                                                    <ext:Panel runat="server" Layout="HBoxLayout" LabelAlign="Left" BodyPadding="5" Border="false">
                                                        <Defaults>
                                                            <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                                                        </Defaults>
                                                        <LayoutConfig>
                                                            <ext:HBoxLayoutConfig Align="Top" />
                                                        </LayoutConfig>
                                                        <Items>
                                                            <ext:ComboBox ID="cBoxAirPak" runat="server" FieldLabel="AirPak" Width="180" AllowBlank="false"
                                                                SelectedIndex="1">
                                                                <Items>
                                                                    <ext:ListItem Text="Sí" Value="1" />
                                                                    <ext:ListItem Text="No" Value="0" />
                                                                </Items>
                                                            </ext:ComboBox>
                                                            
                                                            <ext:Hidden runat="server" Width="5" Flex="2" />
                                                             <ext:ComboBox ID="cBoxParco" runat="server" FieldLabel="Parco" Width="180" AllowBlank="false"
                                                                SelectedIndex="1">
                                                                <Items>
                                                                    <ext:ListItem Text="Sí" Value="1" />
                                                                    <ext:ListItem Text="No" Value="0" />
                                                                </Items>
                                                            </ext:ComboBox>
                                                            <ext:Hidden runat="server" Width="5" Flex="2" />
                                                            <ext:ComboBox ID="cBoxYourPayChoice" runat="server" FieldLabel="YourPayChoice" Width="180" AllowBlank="false"
                                                                SelectedIndex="1">
                                                                <Items>
                                                                    <ext:ListItem Text="Sí" Value="1" />
                                                                    <ext:ListItem Text="No" Value="0" />
                                                                </Items>
                                                            </ext:ComboBox>
                                                        </Items>
                                                    </ext:Panel>
                                                </Items>
                                                <Buttons>
                                                    <ext:Button ID="btnGuardaProgramas" runat="server" Text="Guardar" Icon="Disk" Disabled="true">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnGuardaProgramas_Click" Before="var valid= #{FormPanelProgramas}.getForm().isValid(); if (!valid) {} return valid;">
                                                                <EventMask ShowMask="true" Msg="Guardando Cambios..." MinDelay="500" />
                                                            </Click>
                                                        </DirectEvents>
                                                    </ext:Button>
                                                </Buttons>
                                            </ext:FieldSet>
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
