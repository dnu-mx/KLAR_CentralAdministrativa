<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="GrupoComercial.aspx.cs" Inherits="Empresarial.GrupoComercial" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center Split="true">
            <ext:FormPanel ID="FormPanel1" Width="428.5"  Title="Mi Grupo Comercial" runat="server" Border="false" Layout="Fit">
                <Items>
                    <ext:TabPanel runat="server" ActiveTabIndex="0" TabPosition="Bottom" Border="false"
                        AutoScroll="true" ID="TabPanel1" Title="ctl71">
                        <Items>
                            <ext:Panel ID="Panel7" runat="server" Title="Datos Generales" AutoHeight="true" FormGroup="true">
                                <Content>
                                 <ext:Store ID="SEstado" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="CveEstado">
                                                <Fields>
                                                    <ext:RecordField Name="CveEstado" />
                                                    <ext:RecordField Name="DesEstado" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                    <ext:Store ID="SMunicipio" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="CveMunicipio">
                                                <Fields>
                                                    <ext:RecordField Name="CveMunicipio" />
                                                    <ext:RecordField Name="DesMunicipio" />
                                                    <ext:RecordField Name="CveEstado" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                    <ext:Store ID="SCiudad" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="CveCiudad">
                                                <Fields>
                                                    <ext:RecordField Name="CveCiudad" />
                                                    <ext:RecordField Name="DesCiudad" />
                                                    <ext:RecordField Name="CveEstado" />
                                                    <ext:RecordField Name="CveMunicipio" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                     <ext:Store ID="SAsentamiento" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="CveAsentamiento">
                                                <Fields>
                                                    <ext:RecordField Name="CveAsentamiento" />
                                                    <ext:RecordField Name="DesAsentamiento" />
                                                    <ext:RecordField Name="CveEstado" />
                                                    <ext:RecordField Name="CveMunicipio" />
                                                    <ext:RecordField Name="CveCiudad" />
                                                    <ext:RecordField Name="CodigoPostal" />

                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>

                                    <table>
                                        <caption></caption>
                                        <tr>
                                            <td style="width: 50%;">
                                                <p>
                                                    <span class="x-label-text">Clave Grupo:</span><span style="color: red">*</span></p>
                                                <ext:TextField ID="txtclave" runat="server" Text="" Width="200" />
                                            </td>
                                            <td colspan="2">
                                                <p>
                                                    <span class="x-label-text">Nombre o Razón Social:</span><span style="color: red">*</span></p>
                                                <ext:TextField ID="txtNombre" runat="server" Width="200" Text="" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 50%;">
                                                <p>
                                                    <span class="x-label-text">RFC:</span><span style="color: red">*</span></p>
                                                <ext:TextField ID="txtRFC" runat="server" Text="" Width="200" />
                                            </td>
                                            <td colspan="2">
                                                <p>
                                                    <span class="x-label-text">CURP:</span><span style="color: red">*</span></p>
                                                <ext:TextField ID="txtCURP" runat="server" Width="200" Text="" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 50%;">
                                                <p>
                                                    <span class="x-label-text">Telefono Movil:</span><span style="color: red">*</span></p>
                                                <ext:TextField ID="txtMovil" runat="server" Text="" Width="200" />
                                            </td>
                                            <td colspan="2">
                                                <p>
                                                    <span class="x-label-text">Telefono Fijo:</span><span style="color: red">*</span></p>
                                                <ext:TextField ID="txtFijo" runat="server" Width="200" Text="" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 50%;">
                                                <p>
                                                    <span class="x-label-text">email:</span><span style="color: red">*</span></p>
                                                <ext:TextField ID="txtemail" runat="server" Text="" Width="200" />
                                            </td>
                                            <td colspan="2">
                                              
                                            </td>
                                        </tr>
                                    </table>
                                </Content>
                            </ext:Panel>
                            <ext:Panel ID="Panel8" runat="server" Title="Direccion Ubicación" AutoHeight="true"
                                FormGroup="true">
                                <Content>
                                    <table border="0" cellpadding="5" cellspacing="5">
                                        <caption></caption>
                                     <tr>
                                            <td colspan="2" style="width: 50%;">
                                                <p>
                                                    <span class="x-label-text">Codigo Postal:</span><span style="color: red">*</span></p>
                                                <ext:TextField ID="UtxtCodigoPostal" runat="server" Width="200" Text="" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" style="width: 50%;">
                                                <p>
                                                    <span class="x-label-text">Estado:</span><span style="color: red">*</span></p>
                                                <ext:ComboBox ID="UcmbEstado" runat="server" Width="200" StoreID="SEstado"  DisplayField="DesEstado" ValueField="CveEstado">
                                                  
                                                </ext:ComboBox>
                                                <td colspan="2" style="width: 50%;">
                                                    <p>
                                                        <span class="x-label-text">Municipio o Delegación:</span><span style="color: red">*</span></p>
                                                    <ext:ComboBox ID="UcmbMunicipios" runat="server" Width="200" StoreID="SMunicipio"  DisplayField="DesMunicipio" ValueField="CveMunicipio">
           
                                                    </ext:ComboBox>
                                                </td>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" style="width: 50%;">
                                                <p>
                                                    <span class="x-label-text">Ciudad:</span><span style="color: red">*</span></p>
                                                <ext:ComboBox ID="UcmbCiudad" runat="server" Width="200" StoreID="SCiudad"  DisplayField="DesCiudad" ValueField="CveCiudad">

                                                </ext:ComboBox>
                                                <td colspan="2" style="width: 50%;">
                                                    <p>
                                                        <span class="x-label-text">Asentamiento:</span><span style="color: red">*</span></p>
                                                    <ext:ComboBox ID="UcmbAsentamiento" runat="server" Width="200" StoreID="SAsentamiento"  DisplayField="DesAsentamiento" ValueField="CveAsentamiento">

                                                    </ext:ComboBox>
                                                </td>
                                            </td>
                                        </tr>
                                       
                                        <tr>
                                            <td colspan="2" style="width: 50%;">
                                                <p>
                                                    <span class="x-label-text">Calle:</span><span style="color: red">*</span></p>
                                                <ext:TextField ID="UtxtCalle" runat="server" Width="200" Text="" />
                                            </td>
                                            <td style="width: 25%;">
                                                <p>
                                                    <span class="x-label-text">Num. Int:</span><span style="color: red">*</span></p>
                                                <ext:TextField ID="Utxtinterior" runat="server" Width="90" Text="" />
                                            </td>
                                            <td style="width: 25%;">
                                                <p>
                                                    <span class="x-label-text">Num. Ext.:</span><span style="color: red">*</span></p>
                                                <ext:TextField ID="UtxtExterior" runat="server" Width="90" Text="" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="4" style="width: 100%;">
                                                <p>
                                                    <span class="x-label-text">Entre Calles:</span><span style="color: red">*</span></p>
                                                <%-- <ext:TextField ID="TextField5" runat="server" Width="460" Text="" />--%>
                                                <ext:TextArea ID="UtxtEntreCalles" runat="server" Width="400" Height="50" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="4" style="width: 100%;">
                                                <p>
                                                    <span class="x-label-text">Referencias:</span><span style="color: red">*</span></p>
                                                <%--<ext:TextField ID="TextField6" runat="server" Width="460" Text="" />--%>
                                                <ext:TextArea ID="UtxtReferencias" runat="server" Width="400" Height="50" />
                                            </td>
                                        </tr>
                                    </table>
                                </Content>
                            </ext:Panel>
                            <ext:Panel ID="Panel9" runat="server" Title="Dirección Facturación" AutoHeight="true"
                                FormGroup="true">
                                <Content>
                                    <table border="0" cellpadding="5" cellspacing="5">
                                        <caption></caption>
                                     <tr>
                                            <td colspan="2" style="width: 50%;">
                                                <p>
                                                    <span class="x-label-text">Codigo Postal:</span><span style="color: red">*</span></p>
                                                <ext:TextField ID="FtxtCodigoPostal" runat="server" Width="200" Text="" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" style="width: 50%;">
                                                <p>
                                                    <span class="x-label-text">Estado:</span><span style="color: red">*</span></p>
                                                <ext:ComboBox ID="fcmbEstado" runat="server" Width="200" StoreID="SEstado"  DisplayField="DescEstado" ValueField="CveEstado">
                                                 
                                                </ext:ComboBox>
                                                <td colspan="2" style="width: 50%;">
                                                    <p>
                                                        <span class="x-label-text">Municipio o Delegación:</span><span style="color: red">*</span></p>
                                                    <ext:ComboBox ID="FcmbMunicipio" runat="server" Width="200" StoreID="SMunicipio"  DisplayField="DesMunicipio" ValueField="CveMunicipio">
                                                       
                                                    </ext:ComboBox>
                                                </td>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" style="width: 50%;">
                                                <p>
                                                    <span class="x-label-text">Ciudad:</span><span style="color: red">*</span></p>
                                                <ext:ComboBox ID="FcmbCiudad" runat="server" Width="200" StoreID="SCiudad"  DisplayField="DesCiudad" ValueField="CveCiudad">
                                                   
                                                </ext:ComboBox>
                                                <td colspan="2" style="width: 50%;">
                                                    <p>
                                                        <span class="x-label-text">Asentamiento:</span><span style="color: red">*</span></p>
                                                    <ext:ComboBox ID="FcmbAsentamiento" runat="server" Width="200" StoreID="SAsentamiento"  DisplayField="DesAsentamiento" ValueField="CveAsentamiento">
                                                       
                                                    </ext:ComboBox>
                                                </td>
                                            </td>
                                        </tr>
                                       
                                        <tr>
                                            <td colspan="2" style="width: 50%;">
                                                <p>
                                                    <span class="x-label-text">Calle:</span><span style="color: red">*</span></p>
                                                <ext:TextField ID="FtxtCalle" runat="server" Width="200" Text="" />
                                            </td>
                                            <td style="width: 25%;">
                                                <p>
                                                    <span class="x-label-text">Num. Int:</span><span style="color: red">*</span></p>
                                                <ext:TextField ID="FtxtInterior" runat="server" Width="90" Text="" />
                                            </td>
                                            <td style="width: 25%;">
                                                <p>
                                                    <span class="x-label-text">Num. Ext.:</span><span style="color: red">*</span></p>
                                                <ext:TextField ID="FtxtExterior" runat="server" Width="90" Text="" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="4" style="width: 100%;">
                                                <p>
                                                    <span class="x-label-text">Entre Calles:</span><span style="color: red">*</span></p>
                                                <%-- <ext:TextField ID="TextField5" runat="server" Width="460" Text="" />--%>
                                                <ext:TextArea ID="FtxtEntreCalle" runat="server" Width="400" Height="50" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="4" style="width: 100%;">
                                                <p>
                                                    <span class="x-label-text">Referencias:</span><span style="color: red">*</span></p>
                                                <%--<ext:TextField ID="TextField6" runat="server" Width="460" Text="" />--%>
                                                <ext:TextArea ID="FtxtReferencia" runat="server" Width="400" Height="50" />
                                            </td>
                                        </tr>
                                    </table>
                                </Content>
                            </ext:Panel>
                        </Items>
                        <FooterBar>
                            <ext:Toolbar ID="Toolbar1" runat="server" EnableOverflow="true">
                                <Items>
                                    <ext:Button ID="btnGuardar" runat="server" Text="Guardar" Icon="Add">
                                        <DirectEvents>
                                            <Click OnEvent="btnGuardar_Click" />
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnCancelar" runat="server" Text="Cancelar" Icon="Cancel">
                                        <DirectEvents>
                                            <Click OnEvent="btnCancelar_Click" />
                                        </DirectEvents>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </FooterBar>
                    </ext:TabPanel>
                </Items>
            </ext:FormPanel>
        </Center>
        <East>
                <ext:GridPanel
            ID="GridPanel1"
            runat="server"
            Frame="true"
            StripeRows="true"
            Title="Sponsored Projects"
            AutoExpandColumn="Description"
            Collapsible="true"
            AnimCollapse="false"
            Icon="ApplicationViewColumns"
            TrackMouseOver="false"
            Width="800"
            Height="450"           
            ClicksToEdit="1">
            <Store>
                <ext:Store ID="Store1" runat="server" GroupField="Name">
                    <SortInfo Direction="ASC" Field="Due" />
                    <Reader>
                        <ext:JsonReader IDProperty="TaskID">
                            <Fields>
                                <ext:RecordField Name="ProjectID" />
                                <ext:RecordField Name="Name" />
                                <ext:RecordField Name="TaskID" />
                                <ext:RecordField Name="Description" />
                                <ext:RecordField Name="Estimate" Type="Int" />
                                <ext:RecordField Name="Rate" Type="Float" />
                                <ext:RecordField Name="Due" Type="Date" />
                            </Fields>
                        </ext:JsonReader>
                    </Reader>
                </ext:Store>
            </Store>
            <ColumnModel ID="ColumnModel1" runat="server">
                <Columns>
                    <ext:GroupingSummaryColumn
                        ColumnID="Description"
                        Header="Task"
                        Sortable="true"
                        DataIndex="Description"
                        Hideable="false"
                        SummaryType="Count">
                        <SummaryRenderer Handler="return ((value === 0 || value > 1) ? '(' + value +' Tasks)' : '(1 Task)');" />    
                        <Editor>
                            <ext:TextField ID="TextField1" runat="server" AllowBlank="false" />
                        </Editor>
                    </ext:GroupingSummaryColumn>
                     
                    <ext:Column ColumnID="Name" Header="Project" DataIndex="Name" Width="20" />
                     
                    <ext:GroupingSummaryColumn
                        ColumnID="Due"
                        Width="25"
                        Header="Due Date"
                        Sortable="true"
                        DataIndex="Due"
                        SummaryType="Max">
                        <Renderer Format="Date" FormatArgs="'m/d/Y'" />
                        <Editor>
                            <ext:DateField ID="DateField1" runat="server" Format="MM/dd/yyyy" />
                        </Editor>
                    </ext:GroupingSummaryColumn>
 
                    <ext:GroupingSummaryColumn
                        Width="20"
                        ColumnID="Estimate"
                        Header="Estimate"
                        Sortable="true"
                        DataIndex="Estimate"
                        SummaryType="Sum">
                        <Renderer Handler="return value +' hours';" />
                        <Editor>
                            <ext:NumberField ID="NumberField1" runat="server" AllowBlank="false" AllowNegative="false" StyleSpec="text-align:left" />
                        </Editor>
                    </ext:GroupingSummaryColumn>
                     
                    <ext:GroupingSummaryColumn
                        Width="20"
                        ColumnID="Rate"
                        Header="Rate"
                        Sortable="true"
                        DataIndex="Rate"
                        SummaryType="Average">
                        <Renderer Format="UsMoney" />
                         <Editor>
                            <ext:NumberField ID="NumberField2" runat="server" AllowBlank="false" AllowNegative="false" StyleSpec="text-align:left" />
                        </Editor>
                    </ext:GroupingSummaryColumn>
                     
                    <ext:GroupingSummaryColumn
                        Width="20"
                        ColumnID="Cost"
                        Header="Cost"
                        Sortable="false"
                        Groupable="false"
                        DataIndex="Cost"
                        CustomSummaryType="totalCost">
                        <Renderer Handler="return Ext.util.Format.usMoney(record.data.Estimate * record.data.Rate);" />
                        <SummaryRenderer Format="UsMoney" />
                    </ext:GroupingSummaryColumn>
                </Columns>
                <Listeners>
                    <WidthChange Handler="updateTotal(#{GridPanel1});" />
                </Listeners>
            </ColumnModel>
            <Listeners>
                <ColumnResize Handler="updateTotal(#{GridPanel1});" />
                <AfterRender Handler="updateTotal(#{GridPanel1});" Delay="100" />
            </Listeners>
            <View>
                <ext:GroupingView ID="GroupingView1"
                    runat="server"
                    ForceFit="true"
                    MarkDirty="false"
                    ShowGroupName="false"
                    EnableNoGroups="true"
                    HideGroupedColumn="true"
                    />
            </View>
            <TopBar>
                <ext:Toolbar ID="Toolbar2" runat="server">
                    <Items>
                        <ext:Button ID="Button1" runat="server" Text="Toggle" ToolTip="Toggle the visibility of summary row">
                            <Listeners>
                                <Click Handler="#{GridPanel1}.getGroupingSummary().toggleSummaries();" />
                            </Listeners>
                        </ext:Button>
                    </Items>
                </ext:Toolbar>
            </TopBar>
            <Plugins>
                <ext:GroupingSummary ID="GroupingSummary1" runat="server">
                    <Calculations>
                        <ext:JFunction Name="totalCost" Handler="return v + (record.data.Estimate * record.data.Rate);" />
                    </Calculations>
                </ext:GroupingSummary>
            </Plugins>
            <BottomBar>
                <ext:Toolbar ID="Toolbar3" runat="server">
                    <Items>
                        <ext:DisplayField ID="ColumnField1" runat="server" DataIndex="Description" Cls="total-field" Text="-" />
                        <ext:DisplayField ID="ColumnField2" runat="server" DataIndex="Due" Cls="total-field" Text="-"  />
                        <ext:DisplayField ID="ColumnField3" runat="server" DataIndex="Estimate" Cls="total-field" Text="-"  />
                        <ext:DisplayField ID="ColumnField4" runat="server" DataIndex="Rate" Cls="total-field" Text="-"  />
                        <ext:DisplayField ID="ColumnField5" runat="server" DataIndex="Cost" Cls="total-field" Text="-"  />
                    </Items>
                </ext:Toolbar>
            </BottomBar>
        </ext:GridPanel>
        </East>
    <%--    <East Split="true" Collapsible="true" >
            <ext:FormPanel  Visible="false" ID="FormPanel2" runat="server" TitleCollapse="true" Icon="ApplicationEdit" Border="false" Width="428.5" Title="Entidades Comerciales"
                Layout="Fit">
                <Items>
                    <ext:TabPanel runat="server" Width="428.5" ActiveTabIndex="0" TabPosition="Bottom"
                        Border="false" AutoScroll="true" ID="ctl71" Title="ctl71">
                        <Items>
                            <ext:Panel ID="Panel1" runat="server" Title="Cadenas Comerciales" Border="false"
                                AutoScroll="true" Layout="Fit" Padding="6">
                                <Content>
                                    <ext:Store ID="Store1" runat="server" OnRefreshData="RefreshGrid">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_FichaDeposito">
                                                <Fields>
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Content>
                                <Items>
                                    <ext:GridPanel ID="GridPanel1" runat="server" StoreID="Store1" StripeRows="true"
                                        Header="false" Border="false">
                                        <LoadMask ShowMask="false" />
                                        <ColumnModel ID="ColumnModel1" runat="server">
                                        </ColumnModel>
                                        <DirectEvents>
                                            <RowDblClick OnEvent="Seleccionar">
                                            </RowDblClick>
                                        </DirectEvents>
                                        <SelectionModel>
                                            <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                                            </ext:RowSelectionModel>
                                        </SelectionModel>
                                        <BottomBar>
                                            <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="Store1" DisplayInfo="true"
                                                DisplayMsg="Cadenas Comerciales {0} - {1} de {2}" />
                                        </BottomBar>
                                    </ext:GridPanel>
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel2" Layout="Fit" runat="server" Title="Sucursales" Border="false"
                                AutoScroll="true" Padding="6">
                                <Content>
                                    <ext:Store ID="Store2" runat="server" OnRefreshData="RefreshGrid">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Colectiva">
                                                <Fields>
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Content>
                                <Items>
                                    <ext:GridPanel ID="GridPanel2" runat="server" StoreID="Store1" StripeRows="true"
                                        Header="false" Border="false">
                                        <LoadMask ShowMask="false" />
                                        <ColumnModel ID="ColumnModel2" runat="server">
                                        </ColumnModel>
                                        <DirectEvents>
                                            <RowDblClick OnEvent="Seleccionar">
                                            </RowDblClick>
                                        </DirectEvents>
                                        <SelectionModel>
                                            <ext:RowSelectionModel ID="RowSelectionModel2" runat="server" SingleSelect="true">
                                            </ext:RowSelectionModel>
                                        </SelectionModel>
                                        <BottomBar>
                                            <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="Store1" DisplayInfo="true"
                                                DisplayMsg="Sucursales {0} - {1} de {2}" />
                                        </BottomBar>
                                    </ext:GridPanel>
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel3" Layout="Fit" runat="server" Title="Afiliaciones" Border="false"
                                AutoScroll="true" Padding="6">
                                <Content>
                                    <ext:Store ID="Store3" runat="server" OnRefreshData="RefreshGrid">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Colectiva">
                                                <Fields>
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Content>
                                <Items>
                                    <ext:GridPanel ID="GridPanel3" runat="server" StoreID="Store1" StripeRows="true"
                                        Header="false" Border="false">
                                        <LoadMask ShowMask="false" />
                                        <ColumnModel ID="ColumnModel3" runat="server">
                                        </ColumnModel>
                                        <DirectEvents>
                                            <RowDblClick OnEvent="Seleccionar">
                                            </RowDblClick>
                                        </DirectEvents>
                                        <SelectionModel>
                                            <ext:RowSelectionModel ID="RowSelectionModel3" runat="server" SingleSelect="true">
                                            </ext:RowSelectionModel>
                                        </SelectionModel>
                                        <BottomBar>
                                            <ext:PagingToolbar ID="PagingToolBar3" runat="server" StoreID="Store1" DisplayInfo="true"
                                                DisplayMsg="Afiliaciones {0} - {1} de {2}" />
                                        </BottomBar>
                                    </ext:GridPanel>
                                </Items>
                            </ext:Panel>
                            <ext:Panel ID="Panel4" Layout="Fit" runat="server" Title="Operadores" Border="false"
                                AutoScroll="true" Padding="6">
                                <Content>
                                    <ext:Store ID="Store4" runat="server" OnRefreshData="RefreshGrid">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Colectiva">
                                                <Fields>
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Content>
                                <Items>
                                    <ext:GridPanel ID="GridPanel4" runat="server" StoreID="Store1" StripeRows="true"
                                        Header="false" Border="false">
                                        <LoadMask ShowMask="false" />
                                        <ColumnModel ID="ColumnModel4" runat="server">
                                        </ColumnModel>
                                        <DirectEvents>
                                            <RowDblClick OnEvent="Seleccionar">
                                            </RowDblClick>
                                        </DirectEvents>
                                        <SelectionModel>
                                            <ext:RowSelectionModel ID="RowSelectionModel4" runat="server" SingleSelect="true">
                                            </ext:RowSelectionModel>
                                        </SelectionModel>
                                        <BottomBar>
                                            <ext:PagingToolbar ID="PagingToolBar4" runat="server" StoreID="Store1" DisplayInfo="true"
                                                DisplayMsg="Operadores {0} - {1} de {2}" />
                                        </BottomBar>
                                    </ext:GridPanel>
                                </Items>
                            </ext:Panel>
                        </Items>
                    </ext:TabPanel>
                </Items>
            </ext:FormPanel>
        </East>--%>
    </ext:BorderLayout>
</asp:Content>
