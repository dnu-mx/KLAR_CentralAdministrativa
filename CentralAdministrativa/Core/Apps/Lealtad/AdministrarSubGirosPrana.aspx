<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="AdministrarSubGirosPrana.aspx.cs" Inherits="Lealtad.AdministrarSubGirosPrana" %>

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
                    Ext.net.DirectMethods.Eliminar_Event(record.json.ID_SubGiro);
                    break;

                case "Editar_Event":
                    Ext.net.DirectMethods.Editar_Event(record.json.ID_SubGiro, record.json.Clave, record.json.Descripcion);
                    break;
            }
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
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

    <%--<ext:Store ID="StoreEntidades" runat="server">
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
    </ext:Store>--%>

    <%-- Modal Alta SubGiro --%>
    <ext:Window ID="Modal_AltaSubGiro" runat="server" Title="Alta de SubGiro" Width="425" Height="265" Hidden="true"
        Modal="true" Resizable="false" Icon="LinkAdd">
        <Items>
            <ext:FormPanel ID="FormPanel_NewSubGiro" runat="server" Height="230" Padding="3" LabelWidth="100">
                <Items>
                    <ext:FieldSet runat="server" Title="Datos del Nuevo SubGiro">
                        <Items>
                            <ext:TextField ID="txtClave" runat="server" FieldLabel="Clave" MaxLength="200" Width="300" AllowBlank="false"/>
                            <ext:TextField ID="txtDescripcion" runat="server" FieldLabel="Descripcion" MaxLength="200" Width="300" AllowBlank="false"/>
                        </Items>
                    </ext:FieldSet>
                </Items>
                <Buttons>
                    <ext:Button runat="server" Text="Cancelar" Icon="Cancel">
                        <Listeners>
                            <Click Handler="#{Modal_AltaSubGiro}.hide();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button ID="btnAddSubGiro" runat="server" Text="Crear SubGiro" Icon="Tick">
                        <DirectEvents>
                            <Click OnEvent="btnAddSubGiro_Click" Before="var valid= #{FormPanel_NewSubGiro}.getForm().isValid(); if (!valid) {} return valid;">
                                <EventMask ShowMask="true" Msg="Creando SubGiro..." MinDelay="500" />
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>
    
    <%-- Modal Edición SubGiro --%>
    <ext:Window ID="Modal_EditaSubGiro" runat="server" Title="Edición de SubGiro" Width="425" Height="265" Hidden="true"
        Modal="true" Resizable="false" Icon="LinkAdd">
        <Items>
            <ext:FormPanel ID="FormPanel_EditarSubGiro" runat="server" Height="230" Padding="3" LabelWidth="100">
                <Items>
                    <ext:FieldSet runat="server" Title="Datos del SubGiro">
                        <Items>
                            <ext:Label ID="lblIDSubGiro" runat="server" FieldLabel="ID" ></ext:Label>
                            <ext:Hidden ID="hdnIdSubGiro" runat="server" ></ext:Hidden>
                            
                            <ext:TextField ID="txtClave_Update" runat="server" FieldLabel="Clave" MaxLength="200" Width="300" AllowBlank="false"/>
                            <ext:TextField ID="txtDescripcion_Update" runat="server" FieldLabel="Descripción" MaxLength="200" Width="300" AllowBlank="false"/>
                        </Items>
                    </ext:FieldSet>
                </Items>
                <Buttons>
                    <ext:Button runat="server" Text="Cancelar" Icon="Cancel">
                        <Listeners>
                            <Click Handler="#{Modal_EditaSubGiro}.hide();" />
                        </Listeners>
                    </ext:Button>
                    <ext:Button ID="Button2" runat="server" Text="Actualizar SubGiro" Icon="Tick">
                        <DirectEvents>
                            <Click OnEvent="btnEditaSubGiro_Click" Before="var valid= #{FormPanel_EditarSubGiro}.getForm().isValid(); if (!valid) {} return valid;">
                                <EventMask ShowMask="true" Msg="Editando SubGiro..." MinDelay="500" />
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
        </Items>
    </ext:Window>

    <%-- Panel Consulta Giros --%>
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true">

            <ext:FormPanel ID="FormPanel1" Width="300" Title="Giros" runat="server"
                Border="false" Layout="FitLayout">
                <Content>
                    <ext:Hidden ID="FormatType" runat="server" />
                </Content>
                <Items>
                    <ext:Panel ID="Panel1" runat="server" Border="false" Padding="10">
                        <Items>
                            <ext:ComboBox ID="cBoxGiro" runat="server" FieldLabel="Giro" Width="250" AllowBlank="false"
                                        DisplayField="Descripcion" ValueField="ID_Giro" StoreID="StoreGiros" />
                            
                            <ext:Button ID="Button1" runat="server" Text="Buscar SubGiros..." Icon="Magnifier">
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
                            <ext:Button runat="server" Icon="LinkAdd" ToolTip="Crear Nuevo SubGiro" Text="Nuevo SubGiro">
                                <Listeners>
                                    <Click Handler="#{FormPanel_NewSubgiro}.reset(); #{Modal_AltaSubGiro}.show();" />
                                </Listeners>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>
            </ext:FormPanel>
        </West>

        <%-- Grid SubGiros --%>
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
                    <ext:Store ID="StoreSubGiros" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ReaderSubGiros">
                                <Fields>
                                    <ext:RecordField Name="ID_SubGiro" />
                                    <ext:RecordField Name="Clave" />
                                    <ext:RecordField Name="Descripcion" />

                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <ColumnModel runat="server">
                    <Columns>
                        <%--<ext:CommandColumn Width="30">
                            <Commands>
                                <ext:GridCommand CommandName="Eliminar_Event" Icon="Delete">
                                    <ToolTip Text="Eliminar SubGiro" />
                                </ext:GridCommand>
                            </Commands>
                        </ext:CommandColumn>--%>
                        <ext:CommandColumn Width="30">
                            <Commands>
                                <ext:GridCommand CommandName="Editar_Event" Icon="BulletEdit">
                                    <ToolTip Text="Editar SubGiro" />
                                </ext:GridCommand>
                            </Commands>
                        </ext:CommandColumn>

                        <ext:Column DataIndex="ID_SubGiro" Header="ID" Width="50"/>
                        <ext:Column DataIndex="Clave" Header="Clave" Width="200" />
                        <ext:Column DataIndex="Descripcion" Header="Descripcion" Width="250" />
                    </Columns>
                </ColumnModel>
                <SelectionModel>
                    <ext:RowSelectionModel SingleSelect="true" />
                </SelectionModel>
                <Listeners>
                    <Command Fn="commandHandler" />
                </Listeners>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreSubGiros" DisplayInfo="true"
                        DisplayMsg="{0} - {1} de {2}" HideRefresh="true" />
                </BottomBar>
            </ext:GridPanel>
        </Center>
    </ext:BorderLayout>

</asp:Content>
