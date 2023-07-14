<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="AdministrarObjetosPrana.aspx.cs" Inherits="Lealtad.AdministrarObjetosPrana" %>

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

        var commandHandler = function (cmd, record) {
            switch (cmd) {
                case "Eliminar_Event":
                    Ext.net.DirectMethods.Eliminar_Event(record.json.ID_ProgramasObjetos);
                    break;

                case "Editar_Event":
                    Ext.net.DirectMethods.Editar_Event(record.json.ID_ProgramasObjetos, record.json.Id_TipoEntidad, record.json.URL,
                        record.json.Orden, record.json.PathImagen, record.json.TipoEntidad_Desc);
                    break;
            }
        }
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
    <ext:Store ID="StoreEntidades" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_TipoEntidad">
                <Fields>
                    <ext:RecordField Name="ID_TipoEntidad" />
                    <ext:RecordField Name="Descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
    <ext:Store ID="StoreTipoObjeto" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_TipoObjeto">
                <Fields>
                    <ext:RecordField Name="ID_TipoObjeto" />
                    <ext:RecordField Name="Clave" />
                    <ext:RecordField Name="Descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <%-- Modal Alta Objeto --%>
    <ext:Window ID="Modal_AltaObjeto" runat="server" Title="Alta de Objeto" Width="425" Height="265" Hidden="true"
        Modal="true" Resizable="false" Icon="LinkAdd">
        <Items>
            <ext:FormPanel ID="FormPanel_NewObjeto" runat="server" Height="230" Padding="3" LabelWidth="100">
                <Items>
                    <ext:FieldSet runat="server" Title="Datos del Nuevo Objeto">
                        <Items>
                            <ext:ComboBox ID="cBoxTipoEntidad" runat="server" FieldLabel="Entidad" Width="300" AllowBlank="false"
                                DisplayField="Descripcion" ValueField="ID_TipoEntidad" StoreID="StoreEntidades" />

                            <ext:NumberField ID="nmbOrden" runat="server" FieldLabel="Orden" MaxLength="3" AllowDecimals="false"
                                AllowNegative="false" Width="300" AllowBlank="false"/>

                            <ext:TextField ID="txtURL" runat="server" FieldLabel="URL" MaxLength="200" Width="300" AllowBlank="false"/>
                            
                            <ext:FieldSet runat="server" FieldLabel="Imagen" AnchorHorizontal="100%" Layout="AnchorLayout" Border="false"
                                style="padding-left: 0px;">
                                <Items>
                                    <ext:FileUploadField ID="FileUF_Archivos" runat="server" Width="279" ButtonText="Examinar..."
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
                            <Click Handler="#{Modal_AltaObjeto}.hide();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button ID="btnAddObjeto" runat="server" Text="Crear Objeto" Icon="Tick">
                        <DirectEvents>
                            <Click OnEvent="btnAddObjeto_Click" Before="var valid= #{FormPanel_NewObjeto}.getForm().isValid(); if (!valid) {} return valid;">
                                <EventMask ShowMask="true" Msg="Creando Objeto..." MinDelay="500" />
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    
    <%-- Modal Edición Objeto --%>
    <ext:Window ID="Modal_EditaObjeto" runat="server" Title="Edición de Objeto" Width="425" Height="265" Hidden="true"
        Modal="true" Resizable="false" Icon="LinkAdd">
        <Items>
            <ext:FormPanel ID="FormPanel_EditarObjeto" runat="server" Height="230" Padding="3" LabelWidth="100">
                <Items>
                    <ext:FieldSet runat="server" Title="Datos del Objeto">
                        <Items>
                            <ext:Label ID="lblIDProgramaObjeto" runat="server" FieldLabel="ID" ></ext:Label>
                            <ext:Hidden ID="hdnIdProgramaObjeto" runat="server" ></ext:Hidden>
                            <ext:ComboBox ID="cBoxTipoEntidad_Update" runat="server" FieldLabel="Entidad" Width="300" AllowBlank="false"
                                DisplayField="Descripcion" ValueField="ID_TipoEntidad" StoreID="StoreEntidades" />

                            <ext:NumberField ID="nmbOrden_Update" runat="server" FieldLabel="Orden" MaxLength="3" AllowDecimals="false"
                                AllowNegative="false" Width="300" AllowBlank="false"/>

                            <ext:TextField ID="txtURL_Update" runat="server" FieldLabel="URL" MaxLength="200" Width="300" AllowBlank="false"/>
                            
                            <ext:FieldSet runat="server" FieldLabel="Imagen" AnchorHorizontal="100%" Layout="AnchorLayout" Border="false"
                                style="padding-left: 0px;">
                                <Items>
                                    <ext:FileUploadField ID="FileUF_Archivos_Update" runat="server" Width="279" ButtonText="Examinar..."
                                        Icon="Magnifier" AllowBlank="true" >
                                        <Listeners>
                                            <Render Handler="this.fileInputUpdate.set({ multiple : 'multiple' });"/>
                                        </Listeners>
                                    </ext:FileUploadField>
                                    <ext:Label ID="lblFile_Update" runat="server"></ext:Label>
                                    <ext:Hidden ID="hdnlblFile_Update" runat="server" ></ext:Hidden>
                                </Items>
                            </ext:FieldSet>

                        </Items>
                    </ext:FieldSet>
                </Items>
                <Buttons>
                    <ext:Button runat="server" Text="Cancelar" Icon="Cancel">
                        <Listeners>
                            <Click Handler="#{Modal_EditaObjeto}.hide();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button ID="Button2" runat="server" Text="Actualizar Objeto" Icon="Tick">
                        <DirectEvents>
                            <Click OnEvent="btnEditaObjeto_Click" Before="var valid= #{FormPanel_EditarObjeto}.getForm().isValid(); if (!valid) {} return valid;">
                                <EventMask ShowMask="true" Msg="Editando Objeto..." MinDelay="500" />
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>

    <%-- Panel Consulta Objetos --%>
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true">

            <ext:FormPanel ID="FormPanel1" Width="300" Title="Objetos" runat="server"
                Border="false" Layout="FitLayout">
                <Content>
                    <ext:Hidden ID="FormatType" runat="server" />
                </Content>
                <Items>
                    <ext:Panel ID="Panel1" runat="server" Border="false" Padding="10">
                        <Items>
                            <ext:ComboBox ID="cBoxPrograma" runat="server" FieldLabel="Programa" Width="250" AllowBlank="false"
                                        DisplayField="Descripcion" ValueField="ID_Programa" StoreID="StoreProgramas" />
                            <ext:ComboBox ID="cBoxTipoObjeto" runat="server" FieldLabel="Tipo Objeto" Width="250" AllowBlank="false"
                                DisplayField="Clave" ValueField="ID_TipoObjeto" StoreID="StoreTipoObjeto" />

                            <ext:Button ID="Button1" runat="server" Text="Buscar..." Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click">
                                        <EventMask ShowMask="true" Msg="Buscando..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>

                        </Items>
                    </ext:Panel>
                </Items>
                <FooterBar>
                    <ext:Toolbar ID="Toolbar2" runat="server">
                        <Items>
                            <ext:Button runat="server" Icon="LinkAdd" ToolTip="Crear Nuevo Objeto" Text="Nuevo Objeto">
                                <Listeners>
                                    <Click Handler="#{FormPanel_NewObjeto}.reset(); #{Modal_AltaObjeto}.show();" />
                                </Listeners>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>
            </ext:FormPanel>
        </West>

        <%-- Grid Objetos --%>
        <Center Split="true">
            <ext:GridPanel ID="GridResultados" runat="server" Border="false"
                Header="false">
                <TopBar>
                    <ext:Toolbar runat="server">
                        <Items>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <Store>
                    <ext:Store ID="StoreObjetos" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ReaderObjetos">
                                <Fields>
                                    <ext:RecordField Name="ID_ProgramasObjetos" />
                                    <ext:RecordField Name="ID_Programa" />
                                    <ext:RecordField Name="ID_TipoObjeto" />
                                    <ext:RecordField Name="TipoObjetos_Desc" />
                                    <ext:RecordField Name="Orden" />
                                    <ext:RecordField Name="Id_TipoEntidad" />
                                    <ext:RecordField Name="TipoEntidad_Desc" />
                                    <ext:RecordField Name="URL" />
                                    <ext:RecordField Name="PathImagen" />
                                    <ext:RecordField Name="Activo" />

                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <ColumnModel runat="server">
                    <Columns>
                        <ext:CommandColumn Width="30">
                            <Commands>
                                <ext:GridCommand CommandName="Eliminar_Event" Icon="Delete">
                                    <ToolTip Text="Eliminar Objeto" />
                                </ext:GridCommand>
                            </Commands>
                        </ext:CommandColumn>
                        <ext:CommandColumn Width="30">
                            <Commands>
                                <ext:GridCommand CommandName="Editar_Event" Icon="BulletEdit">
                                    <ToolTip Text="Editar Objeto" />
                                </ext:GridCommand>
                            </Commands>
                        </ext:CommandColumn>

                        <ext:Column DataIndex="ID_ProgramasObjetos" Header="ID" Width="50"/>
                        <ext:Column DataIndex="ID_TipoObjeto" Hidden="true"/>
                        <ext:Column DataIndex="Id_TipoEntidad" Hidden="true"/>
                        <ext:Column DataIndex="TipoEntidad_Desc" Header="Entidad" Width="100"/>
                        <ext:Column DataIndex="Orden" Header="Orden" Width="50" />
                        <ext:Column DataIndex="URL" Header="URL" Width="200" />
                        <ext:Column DataIndex="PathImagen" Header="Path" Width="250" />
                    </Columns>
                </ColumnModel>
                <SelectionModel>
                    <ext:RowSelectionModel SingleSelect="true" />
                </SelectionModel>
                <Listeners>
                    <Command Fn="commandHandler" />
                </Listeners>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreObjetos" DisplayInfo="true"
                        DisplayMsg="{0} - {1} de {2}" HideRefresh="true" />
                </BottomBar>
            </ext:GridPanel>
        </Center>
    </ext:BorderLayout>

</asp:Content>
