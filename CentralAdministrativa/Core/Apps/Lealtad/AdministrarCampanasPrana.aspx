<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="AdministrarCampanasPrana.aspx.cs" Inherits="Lealtad.AdministrarCampanasPrana" %>

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
    
    <ext:Store ID="StoreProgramas" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_Programa">
                <Fields>
                    <ext:RecordField Name="ID_Programa" />
                    <ext:RecordField Name="Clave" />
                    <ext:RecordField Name="Descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <%-- Modal Alta Campaña --%>
    <ext:Window ID="WdwNC_Camp" runat="server" Title="Alta de Campaña" Width="450" Height="440" Hidden="true"
        Modal="true" Resizable="false" Icon="LinkAdd">
        <Items>
            <ext:FormPanel ID="FormPanelNC_Camp" runat="server" Height="410" Padding="3" LabelWidth="120">
                <Items>
                    <ext:FieldSet runat="server" Title="Datos de la Nueva Campaña">
                        <Items>
                            <ext:TextField ID="txtClaveCampana" runat="server" FieldLabel="Clave Campaña" MaxLength="100"
                                Width="300" AllowBlank="false"/>
                            <ext:TextField ID="txtCampana" runat="server" FieldLabel="Nombre" MaxLength="200"
                                Width="300" AllowBlank="false"/>
                            <ext:ComboBox ID="cBoxProgramas" runat="server" FieldLabel="Programa" Width="300" AllowBlank="false"
                                DisplayField="Descripcion" ValueField="ID_Programa" StoreID="StoreProgramas" />
                            <ext:FieldSet runat="server" FieldLabel="Imagen" AnchorHorizontal="100%" Layout="AnchorLayout" Border="false"
                                style="padding-left: 0px;">
                                <Items>
                                    <ext:FileUploadField ID="FileUF_Archivos" runat="server" Width="250" ButtonText="Examinar..."
                                        Icon="Magnifier" AllowBlank="false" >
                                        <Listeners>
                                            <Render Handler="this.fileInput.set({ multiple : 'multiple' });" />
                                        </Listeners>
                                    </ext:FileUploadField>
                                </Items>
                            </ext:FieldSet>
                        </Items>
                    </ext:FieldSet>
                </Items>
                <Buttons>
                    <ext:Button runat="server" Text="Cancelar" Icon="Cancel">
                        <Listeners>
                            <Click Handler="#{WdwNC_Camp}.hide();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button ID="btnAddCampana" runat="server" Text="Crear Campana" Icon="Tick">
                        <DirectEvents>
                            <Click OnEvent="btnAddCampana_Click" Before="var valid= #{FormPanelNC_Camp}.getForm().isValid(); if (!valid) {} return valid;">
                                <EventMask ShowMask="true" Msg="Creando Campaña..." MinDelay="500" />
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    
    <%-- Panel Consulta Campañas --%>
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true">
            <ext:Panel runat="server" Width="325" Border="false" Layout="FitLayout" Title="Consulta de Campañas">
                <Content>
                    <ext:BorderLayout ID="LeftBorderLayout" runat="server">
                        <%-- Panel botón abajo Limpiar y Nueva Campaña--%>
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
                                            <ext:Button runat="server" Icon="LinkAdd" ToolTip="Crear Nueva Campaña"
                                                Text="Nueva Campaña">
                                                <Listeners>
                                                    <Click Handler="#{FormPanelNC_Camp}.reset(); #{WdwNC_Camp}.show();" />
                                                </Listeners>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </Items>
                            </ext:FormPanel>
                        </South>
                        <%-- Panel Buscar Campaña --%>
                        <Center Split="true">
                            <ext:GridPanel ID="GridResultados" runat="server" AutoExpandColumn="Nombre" Border="false"
                                Header="false">
                                <TopBar>
                                    <ext:Toolbar runat="server">
                                        <Items>
                                            <ext:TextField ID="txtClaveCamp" runat="server" EmptyText="Clave Campaña" Width="100"/>
                                            <ext:TextField ID="txtNombreCamp" runat="server" EmptyText="Nombre" MaxLength="200" />
                                            <ext:Button ID="btnBuscarCamp" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnBuscarCamp_Click" Before="#{PanelCentralCamp}.setTitle('');
                                                        #{PanelCentralCamp}.setDisabled(true);">
                                                        <EventMask ShowMask="true" Msg="Buscando Campañas..." MinDelay="500" />
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <Store>
                                    <ext:Store ID="StoreCampanas" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ReaderCampanas">
                                                <Fields>
                                                    <ext:RecordField Name="Id_Campana" />
                                                    <ext:RecordField Name="ClaveCampana" />
                                                    <ext:RecordField Name="Nombre" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <ColumnModel runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="Id_Campana" Hidden="true" />
                                        <ext:Column DataIndex="ClaveCampana" Header="Clave" Width="90" />
                                        <ext:Column DataIndex="Nombre" Header="Nombre Comercial" Width="110" />
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel SingleSelect="true" />
                                </SelectionModel>
                                <DirectEvents>
                                    <RowClick OnEvent="selectRowResultados_Event">
                                        <EventMask ShowMask="true" Msg="Obteniendo Información de la Campaña..." MinDelay="500" />
                                        <ExtraParams>
                                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridResultados}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                        </ExtraParams>
                                    </RowClick>
                                </DirectEvents>

                                
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreCampanas" DisplayInfo="true"
                                        DisplayMsg="{0} - {1} de {2}" HideRefresh="true" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </West>

        <%-- Panel pestañas Info.Campañas | Promociones --%>
        <Center Split="true">
            <ext:Panel ID="PanelCentralCamp" runat="server" Height="250" Border="false" Title="_" Disabled="true">
                <Items>
                    <ext:BorderLayout runat="server">
                        <Center>
                            <ext:TabPanel runat="server">
                                <Items>

                                    <%-- Panel Edición Campañas --%>
                                    <ext:FormPanel ID="FormPanelInfoAdCampana" runat="server" Title="Campaña" LabelAlign="Left" LabelWidth="120"
                                        AutoScroll="true">
                                        <Items>
                                            <ext:FieldSet runat="server" Title="Datos de la Campaña">
                                                <Items>
                                                    <ext:Hidden ID="hdnlblClaveCampAd" runat="server" />
                                                    <ext:Hidden ID="hdnIdCamp" runat="server" />
                                                    <ext:TextField ID="txtClaveCampAd" runat="server" FieldLabel="Clave Campaña" MaxLength="100"
                                                        Width="550" Disabled="true" />
                                                    <ext:TextField ID="txtNombreCampAd" runat="server" FieldLabel="Nombre Comercial" MaxLength="200"
                                                        Width="550"  AllowBlank="false" />
                                                    <ext:ComboBox ID="cBoxProgramasAd" runat="server" FieldLabel="Programa" Width="550" AllowBlank="false"
                                                        DisplayField="Descripcion" ValueField="ID_Programa" StoreID="StoreProgramas" />
                                                    <ext:Checkbox ID="chkActivo" runat="server" FieldLabel="Activo"> </ext:Checkbox>
                                                    <ext:FieldSet runat="server" FieldLabel="Imagen" AnchorHorizontal="100%" Layout="AnchorLayout" Border="false"
                                                        style="padding-left: 0px;">
                                                        <Items>
                                                            <ext:FileUploadField ID="FileUF_Archivos_Update" runat="server" Width="279" ButtonText="Examinar..."
                                                                Icon="Magnifier" AllowBlank="true" >
                                                                <Listeners>
                                                                    <Render Handler="this.fileInput.set({ multiple : 'multiple' });"/>
                                                                </Listeners>
                                                            </ext:FileUploadField>
                                                            <ext:Label ID="lblFile_Update" runat="server"></ext:Label>
                                                            <ext:Hidden ID="hdnlblFile_Update" runat="server" ></ext:Hidden>
                                                        </Items>
                                                    </ext:FieldSet>
                                                </Items>
                                                <Buttons>
                                                    <ext:Button ID="btnGuardaInfoAdCampana" runat="server" Text="Guardar" Icon="Disk">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnGuardarInfoAd_Click" Before="var valid= #{FormPanelInfoAdCampana}.getForm().isValid(); if (!valid) {} return valid;">
                                                                <Confirmation ConfirmRequest="true" Title="Confirmación" Message="Si realizaste algún cambio en el Programa, se borrarán todas las promociones asignadas. ¿Deseas Continuar?" />
                                                                <EventMask ShowMask="true" Msg="Guardando Cambios..." MinDelay="500" />
                                                            </Click>
                                                        </DirectEvents>
                                                    </ext:Button>
                                                </Buttons>

                                            </ext:FieldSet>
                                        </Items>
                                    </ext:FormPanel>

                                    <%-- Panel Promociones--%>
                                    <ext:FormPanel ID="FormPanelPromociones" runat="server" Title="Promociones" Layout="FitLayout" Border="false">
                                        <Items>
                                            <ext:Panel runat="server" Border="false">
                                                <Content>
                                                    <ext:BorderLayout runat="server">

                                                        <%-- Panel Promociones Disponibles --%>
                                                        <Center Split="true">
                                                            <ext:Panel runat="server" Border="false">
                                                                <Content>
                                                                    <ext:BorderLayout runat="server">
                                                                        <Center Split="true">
                                                                            <ext:GridPanel ID="GridPromociones" runat="server" AutoExpandColumn="Descripción" Border="false" Header="false"
                                                                                Layout="FitLayout">
                                                                                <TopBar>
                                                                                    <ext:Toolbar runat="server">
                                                                                        <Items>
                                                                                            <ext:TextField ID="txtClavePromo" runat="server" EmptyText="Clave Promoción" Width="100" />
                                                                                            <ext:TextField ID="txtNombrePromo" runat="server" EmptyText="Nombre" />
                                                                                            <ext:Button ID="btnBuscarPromo" runat="server" Text="Buscar" Icon="Magnifier">
                                                                                                <DirectEvents>
                                                                                                    <Click OnEvent="btnBuscarPromo_Click">
                                                                                                        <EventMask ShowMask="true" Msg="Buscando Promociones..." MinDelay="500" />
                                                                                                    </Click>
                                                                                                </DirectEvents>
                                                                                            </ext:Button>
                                                                                        </Items>
                                                                                    </ext:Toolbar>
                                                                                </TopBar>
                                                                                <Store>
                                                                                    <ext:Store ID="StorePromociones" runat="server">
                                                                                        <Reader>
                                                                                            <ext:JsonReader IDProperty="ID_Promocion">
                                                                                                <Fields>
                                                                                                    <ext:RecordField Name="ID_Promocion" />
                                                                                                    <ext:RecordField Name="ClavePromocion" />
                                                                                                    <ext:RecordField Name="Descripción" />
                                                                                                </Fields>
                                                                                            </ext:JsonReader>
                                                                                        </Reader>
                                                                                    </ext:Store>
                                                                                </Store>
                                                                                <ColumnModel runat="server">
                                                                                    <Columns>
                                                                                        <ext:CommandColumn ColumnID="acciones" Width="30" Header="">
                                                                                            <Commands>
                                                                                                <ext:GridCommand Icon="Add" CommandName="AddPromo">
                                                                                                    <ToolTip Text="Agregar una Promoción" />
                                                                                                </ext:GridCommand>
                                                                                            </Commands>
                                                                                        </ext:CommandColumn>

                                                                                        <ext:Column DataIndex="ID_Promocion" Hidden="true" />
                                                                                        <ext:Column DataIndex="ClavePromocion" Header="Clave" Width="120" />
                                                                                        <ext:Column DataIndex="Descripción" Header="Nombre" Width="120" />
                                                                                    </Columns>
                                                                                </ColumnModel>
                                                                                <SelectionModel>
                                                                                    <ext:RowSelectionModel SingleSelect="true" />
                                                                                </SelectionModel>
                                                                                <DirectEvents>
                                                                                    <Command OnEvent="AgregarPromocion">
                                                                                        <ExtraParams>
                                                                                            <ext:Parameter Name="Promociones" Value="Ext.encode(#{GridPromociones}.getRowsValues())" Mode="Raw" />
                                                                                            <ext:Parameter Name="ID_Promocion" Value="record.data['ID_Promocion']" Mode="Raw" />
                                                                                            <ext:Parameter Name="ClavePromocion" Value="record.data['ClavePromocion']" Mode="Raw" />
                                                                                            <ext:Parameter Name="Descripcion" Value="record.data['Descripción']" Mode="Raw" />
                                                                                        </ExtraParams>
                                                                                    </Command>
                                                                                </DirectEvents>
                                                                                <BottomBar>
                                                                                    <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="StorePromociones" DisplayInfo="true"
                                                                                        DisplayMsg="{0} - {1} de {2}" HideRefresh="true" />
                                                                                </BottomBar>
                                                                            </ext:GridPanel>
                                                                        </Center>
                                                                    </ext:BorderLayout>
                                                                </Content>
                                                            </ext:Panel>
                                                        </Center>
                                                        
                                                        <%-- Panel Promociones Asignadas --%>
                                                        <East Split="true">
                                                            <ext:FormPanel ID="FormPanelPromocionesAsignadas" runat="server" Title="Promociones Asignadas" LabelAlign="Top" Width="340"
                                                                Padding="5" Layout="FormLayout" AutoScroll="true" Disabled="false" Collapsible="true">
                                                                <Content>
                                                                    <ext:BorderLayout runat="server">
                                                                        <Center Split="true">
                                                                            <ext:GridPanel ID="GridPromocionesAsignadas" runat="server" AutoExpandColumn="Descripción" Border="false" Header="false"
                                                                                Layout="FitLayout">
                                                                                
                                                                                <Store>
                                                                                    <ext:Store ID="StorePromocionesAsignadas" runat="server">
                                                                                        <Reader>
                                                                                            <ext:JsonReader IDProperty="ID_Promocion">
                                                                                                <Fields>
                                                                                                    <ext:RecordField Name="ID_Promocion" />
                                                                                                    <ext:RecordField Name="ClavePromocion" />
                                                                                                    <ext:RecordField Name="Descripción" />
                                                                                                </Fields>
                                                                                            </ext:JsonReader>
                                                                                        </Reader>
                                                                                    </ext:Store>
                                                                                </Store>
                                                                                <ColumnModel runat="server">
                                                                                    <Columns>
                                                                                        <ext:CommandColumn ColumnID="acciones" Width="30" Header="">
                                                                                            <Commands>
                                                                                                <ext:GridCommand Icon="Delete" CommandName="DeletePromo">
                                                                                                    <ToolTip Text="Eliminar una promoción" />
                                                                                                </ext:GridCommand>
                                                                                            </Commands>
                                                                                        </ext:CommandColumn>
                                                                                        <ext:Column DataIndex="ID_Promocion" Hidden="true" />
                                                                                        <ext:Column DataIndex="ClavePromocion" Header="Clave" Width="120" />
                                                                                        <ext:Column DataIndex="Descripción" Header="Nombre" Width="120" />
                                                                                    </Columns>
                                                                                </ColumnModel>
                                                                                <SelectionModel>
                                                                                    <ext:RowSelectionModel SingleSelect="true" />
                                                                                </SelectionModel>
                                                                                <DirectEvents>
                                                                                    <Command OnEvent="EliminarPromocion">
                                                                                        <ExtraParams>
                                                                                            <ext:Parameter Name="Promociones" Value="Ext.encode(#{GridPromocionesAsignadas}.getRowsValues())" Mode="Raw" />
                                                                                            <ext:Parameter Name="ID_Promocion" Value="record.data['ID_Promocion']" Mode="Raw" />
                                                                                            <ext:Parameter Name="ClavePromocion" Value="record.data['ClavePromocion']" Mode="Raw" />
                                                                                            <ext:Parameter Name="Descripcion" Value="record.data['Descripción']" Mode="Raw" />
                                                                                        </ExtraParams>
                                                                                    </Command>
                                                                                </DirectEvents>
                                                                                <BottomBar>
                                                                                    <ext:PagingToolbar ID="PagingToolBar3" runat="server" StoreID="StorePromocionesAsignadas" DisplayInfo="true"
                                                                                        DisplayMsg="{0} - {1} de {2}" HideRefresh="true" />
                                                                                </BottomBar>
                                                                            </ext:GridPanel>
                                                                        </Center>
                                                                    </ext:BorderLayout>
                                                                </Content>
                                                                <Buttons>
                                                                    <ext:Button ID="btnGuardarPromociones" runat="server" Text="Guardar Asignaciones" Icon="Disk" CausesValidation="false">
                                                                        <ToolTips>
                                                                            <ext:ToolTip ID="ToolTip1" runat="server" Title="Guardar las promociones asignadas" Html="Guardar las promociones asignadas" />
                                                                        </ToolTips>
                                                                        <DirectEvents>
                                                                            <Click OnEvent="btnGuardarPromos_Click">
                                                                                <ExtraParams>
                                                                                    <ext:Parameter Name="Promociones" Value="Ext.encode(#{GridPromocionesAsignadas}.getRowsValues())" Mode="Raw" />
                                                                                </ExtraParams>
                                                                            </Click>
                                                                        </DirectEvents>
                                                                    </ext:Button>
                                                                </Buttons>
                                                            </ext:FormPanel>
                                                        </East>
                                                    </ext:BorderLayout>
                                                </Content>
                                            </ext:Panel>
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
