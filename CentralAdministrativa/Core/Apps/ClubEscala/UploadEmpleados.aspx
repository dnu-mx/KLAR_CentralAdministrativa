<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="UploadEmpleados.aspx.cs" Inherits="ClubEscala.UploadEmpleados" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <west Split="true" Collapsible="false">
            <ext:Panel ID="Panel5" runat="server" Title="Agregar Empleados" Collapsed="false"
                Layout="Fit" AutoScroll="true"  Width="320">
                <Items>
                    <ext:Panel runat="server" Border="false" AutoScroll="true" ID="TabPanel1">
                        <Items>
                            <ext:FormPanel ID="FormPanel1" runat="server" Border="false" LabelAlign="Top">
                                <Content>
                                    <ext:Store ID="storeCadenaComercial" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Colectiva">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Colectiva" />
                                                    <ext:RecordField Name="NombreORazonSocial" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                        <SortInfo Field="NombreORazonSocial" />
                                    </ext:Store>
                                    <ext:Store ID="storeArchivos" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Colectiva">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Colectiva" />
                                                    <ext:RecordField Name="NombreORazonSocial" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                        <SortInfo Field="NombreORazonSocial" />
                                    </ext:Store>
                                </Content>
                                <Items>
                                    <ext:ComboBox ID="cmbCadenaComercial" FieldLabel="Empresa" Visible="true"
                                        LabelAlign="Top" AllowBlank="false" MsgTarget="Side" runat="server" EmptyText="Selecciona una Empresa"
                                        Width="300" StoreID="storeCadenaComercial" DisplayField="NombreORazonSocial"
                                        ValueField="ID_Colectiva">
                                    </ext:ComboBox>
                                    <%--                                    <ext:ComboBox ID="ComboBox1" FieldLabel="Archivos" Visible="true" LabelAlign="Top"
                                        MsgTarget="Side" runat="server" EmptyText="Para Consultar selecciona el Archivo del Historial"
                                        Width="400" StoreID="storeArchivos" DisplayField="ArchivoName" ValueField="ID_Colectiva">
                                    </ext:ComboBox>--%>
                                    <ext:FileUploadField ID="FileSelect" FieldLabel="Selecciona el Archivo de Empleados"
                                        LabelAlign="Top" runat="server" Width="300" Icon="Attach" Regex="\d+|.(csv|txt)$"
                                        RegexText="Selecciona Archivo (.csv) o (.txt)">
                                        <%-- <DirectEvents>
                                            <FileSelected OnEvent="FileUploadField_FileSelected" IsUpload="true" />
                                        </DirectEvents>--%>
                                    </ext:FileUploadField>
                                </Items>
                                <FooterBar>
                                    <ext:Toolbar ID="Toolbar1" runat="server" EnableOverflow="true">
                                        <Items>
                                            <ext:Button ID="btnUpload" runat="server" Text="Subir Archivo" Icon="Add">
                                                <DirectEvents>
                                                    <Click OnEvent="FileUploadField_FileSelected" IsUpload="true" Before="if (!#{FormPanel1}.getForm().isValid()) { return false; } 
                                                            Ext.Msg.wait('Almacenando registros para Proceso Nocturno...', 'Procesando Archivo');"
                                                        Success="Ext.Msg.hide(); Ext.Msg.show({ 
                                                            title   : 'Upload Exitoso', 
                                                            msg     : 'El Archivo Se almacenó correctamente, por favor espera a recibir el resultado del procesamiento por email', 
                                                            minWidth: 200, 
                                                            modal   : true, 
                                                            icon    : Ext.Msg.INFO, 
                                                            buttons : Ext.Msg.OK 
                                                        });" Failure="Ext.Msg.show({ 
                                                            title   : 'Error', 
                                                            msg     : 'Ocurrio un Error al Intenar subir el Archivo seleccionado al Servidor', 
                                                            minWidth: 200, 
                                                            modal   : true, 
                                                            icon    : Ext.Msg.ERROR, 
                                                            buttons : Ext.Msg.OK 
                                                        });">
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="Add">
                                                <Listeners>
                                                    <Click Handler="#{FormPanel1}.getForm().reset();" />
                                                </Listeners>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </FooterBar>
                            </ext:FormPanel>
                        </Items>
                    </ext:Panel>
                </Items>
            </ext:Panel>
        </west>
        <Center Split="true" Collapsible="true">
            <ext:Panel ID="Panel2" runat="server" Title="Registros Leídos del Archivo" Collapsed="false"
                Layout="Fit" AutoScroll="true" >
                <Content>
                    <ext:Store ID="storeEmpleados" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID">
                                <Fields>
                                    <ext:RecordField Name="NumeroEmpleado" />
                                    <ext:RecordField Name="APaterno" />
                                    <ext:RecordField Name="AMaterno" />
                                    <ext:RecordField Name="Nombre" />
                                    <ext:RecordField Name="FechaNacimiento" Type="Date" />
                                    <ext:RecordField Name="TelefonoMovil" />
                                    <ext:RecordField Name="EmailEmpresarial" />
                                    <ext:RecordField Name="EmailPersonal" />
                                    <ext:RecordField Name="LimiteCompra" />
                                    <ext:RecordField Name="CicloNominal" />
                                    <ext:RecordField Name="Baja" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanel1" runat="server" StoreID="storeEmpleados" StripeRows="true"
                                Header="false" Border="false" >
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                        <ext:Column Header="No. Empleado"  Sortable="true" DataIndex="NumeroEmpleado" />
                                        <ext:Column ColumnID="Nombre" Header="Nombre" Sortable="true" DataIndex="Nombre" />
                                        <ext:Column ColumnID="APaterno" Header="Apellido Paterno" Sortable="true" DataIndex="APaterno" />
                                        <ext:Column ColumnID="AMaterno" Header="Apellido Materno" Sortable="true" DataIndex="AMaterno" />
                                        <ext:DateColumn ColumnID="FechaNacimiento" Header="Fecha Nacimiento" Sortable="true"
                                            DataIndex="FechaNacimiento" Format="yyyy-MM-dd" />
                                        <ext:Column ColumnID="TelefonoMovil" Header="TelefonoMovil" Sortable="true" DataIndex="TelefonoMovil" />
                                        <ext:Column ColumnID="EmailEmpresarial" Header="Email Empresa" Sortable="true" DataIndex="EmailEmpresarial" />
                                        <ext:Column ColumnID="EmailPersonal" Header="EmailPersonal" Sortable="true" DataIndex="EmailPersonal" />
                                        <ext:Column ColumnID="LimiteCompra" Header="LimiteCompra" Sortable="true" DataIndex="LimiteCompra">
                                            <Renderer Format="UsMoney" />
                                        </ext:Column>
                                        <ext:Column ColumnID="CicloNominal" Header="CicloNominal" Sortable="true" DataIndex="CicloNominal" />
                                        <%-- <ext:DateColumn ColumnID="ID_Estatus" Header="ID_Estatus" Sortable="true" DataIndex="ID_Estatus" />
                                        <ext:DateColumn ColumnID="Resultado" Header="Resultado" Sortable="true" DataIndex="Resultado" />--%>
                                        <%-- <ext:CommandColumn Width="160">
                                            <Commands>
                                                <ext:GridCommand Icon="Delete" CommandName="Procesar">
                                                    <ToolTip Text="Presiona para Procesar" />
                                                </ext:GridCommand>
                                                <ext:CommandSeparator />
                                                <ext:GridCommand Icon="NoteEdit" CommandName="Procesada">
                                                    <ToolTip Text="Procesada" />
                                                </ext:GridCommand>
                                                <ext:GridCommand Icon="NoteEdit" CommandName="Error">
                                                    <ToolTip Text="Error al Procesar" />
                                                </ext:GridCommand>
                                            </Commands>
                                        </ext:CommandColumn>--%>
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                                        <DirectEvents>
                                            <%-- <RowSelect OnEvent="RowSelect" Buffer="100">
                                                <EventMask ShowMask="true" Target="CustomTarget" CustomTarget="#{FormPanel1}" />
                                                <ExtraParams>
                                                    <ext:Parameter Name="AfiliciacionID" Value="this.getSelected().id" Mode="Raw" />
                                                </ExtraParams>
                                            </RowSelect>--%>
                                        </DirectEvents>
                                    </ext:RowSelectionModel>
                                </SelectionModel>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="storeEmpleados" DisplayInfo="true"
                                        DisplayMsg="Elementos Importados {0} - {1} de {2}" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
