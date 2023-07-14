<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="AddCertificado.aspx.cs" Inherits="Certificados.AddCertificado" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">

        var prepareToolbar = function (grid, toolbar, rowIndex, record) {
            // for example hide 'Edit' button if price < 50
            if (record.get("ID_Estatus") == 1) { //Activo

                //toolbar.items.get(0).hide(); //Delete
                //toolbar.items.get(0).hide(); //asgina
                toolbar.items.get(0).hide(); //asgina
                toolbar.items.get(2).hide();
                toolbar.items.get(3).hide();


            } else if (record.get("ID_Estatus") == 2) { //Otro no Activo
                toolbar.items.get(2).hide(); //asgina
                toolbar.items.get(1).hide(); //asgina
                toolbar.items.get(3).hide();

            } else if (record.get("ID_Estatus") == 4) { //Otro no Activo
                toolbar.items.get(0).hide(); //asgina
                toolbar.items.get(1).hide(); //asgina
                toolbar.items.get(3).hide();

            }  else { //Otro no Activo
                toolbar.items.get(1).hide(); //asgina
                //toolbar.items.get(1).hide(); //asgina
                toolbar.items.get(2).hide();
                toolbar.items.get(0).hide();


            }
        };

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true">
            <ext:FormPanel ID="FormPanel1" Width="428.5" Title="Agregar Usuario" runat="server"
                Border="false" Layout="Fit">
                <Content>
                    <ext:Store ID="SCColectiva" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                                <Fields>
                                    <ext:RecordField Name="ID_Colectiva" />
                                    <ext:RecordField Name="NombreORazonSocial" />
                                    <ext:RecordField Name="ColectivaNombre" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Content>
                <Items>
                    <ext:FormPanel ID="Panel55" runat="server" Border="false" Header="false" Width="500px"
                        LabelAlign="Left" Layout="FormLayout">
                        <Items>
                            <ext:Panel ID="Panel3" runat="server" Title="Creación de Certificados" AutoHeight="true"
                                LabelAlign="Top" FormGroup="true" Layout="TableLayout" Width="500px">
                                <Items>
                                    <ext:Panel ID="Panel7" runat="server" Border="false" Header="false" Width="400px"
                                        Layout="Form" LabelAlign="Top">
                                        <Items>
                                            <ext:ComboBox ID="cmbTotalCertificados" EmptyText="Selecciona una Opcion" TabIndex="7"
                                                FieldLabel="Total de Certificados a Crear" MaxLength="100" runat="server" Text=""
                                                MsgTarget="Side" AllowBlank="false" AnchorHorizontal="90%">
                                                <Items>
                                                    <ext:ListItem Text="01" Value="1" />
                                                    <ext:ListItem Text="02" Value="2" />
                                                    <ext:ListItem Text="03" Value="3" />
                                                    <ext:ListItem Text="04" Value="4" />
                                                    <ext:ListItem Text="05" Value="5" />
                                                    <ext:ListItem Text="06" Value="6" />
                                                    <ext:ListItem Text="07" Value="7" />
                                                    <ext:ListItem Text="08" Value="8" />
                                                    <ext:ListItem Text="09" Value="9" />
                                                    <ext:ListItem Text="10" Value="10" />
                                                </Items>
                                            </ext:ComboBox>
                                            <ext:ComboBox ID="cmbColectiva" Resizable="true" TabIndex="8" EmptyText="Selecciona una Opcion"
                                                FieldLabel="Colectiva Asignada al Usuario" MaxLength="100" runat="server" Text=""
                                                MsgTarget="Side" AllowBlank="false" AnchorHorizontal="90%" StoreID="SCColectiva"
                                                DisplayField="ColectivaNombre" ValueField="ID_Colectiva" />
                                            <ext:ComboBox ID="cmbExpiracion" EmptyText="Selecciona una Opcion" TabIndex="7" FieldLabel="Dias Expiración"
                                                MaxLength="100" runat="server" Text="" MsgTarget="Side" AllowBlank="false" AnchorHorizontal="90%">
                                                <Items>
                                                    <ext:ListItem Text="02" Value="2" />
                                                    <ext:ListItem Text="04" Value="4" />
                                                    <ext:ListItem Text="06" Value="6" />
                                                    <ext:ListItem Text="08" Value="8" />
                                                    <ext:ListItem Text="10" Value="10" />
                                                </Items>
                                            </ext:ComboBox>
                                        </Items>
                                    </ext:Panel>
                                </Items>
                            </ext:Panel>
                        </Items>
                        <FooterBar>
                            <ext:Toolbar ID="Toolbar1" runat="server" EnableOverflow="true">
                                <Items>
                                    <ext:Button ID="btnGuardar" TabIndex="9" runat="server" Text="Crear Certificado"
                                        Icon="ShieldAdd">
                                        <DirectEvents>
                                            <Click OnEvent="btnGuardar_Click" Before="var valid= #{FormPanel1}.getForm().isValid(); if (!valid) {} return valid;" />
                                        </DirectEvents>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </FooterBar>
                    </ext:FormPanel>
                </Items>
            </ext:FormPanel>
        </West>
        <Center Split="true">
            <ext:Panel ID="Panel5" runat="server" Width="428.5" Title="Certificados" Collapsed="false"
                Collapsible="true" Layout="Fit" AutoScroll="true">
                <Content>
                    <ext:Store ID="storeCertificados" runat="server" OnRefreshData="RefreshGrid">
                        <Reader>
                            <ext:JsonReader IDProperty="UserId">
                                <Fields>
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanel1" runat="server" StoreID="storeCertificados" StripeRows="true"
                                Header="false" Border="false">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel1" runat="server">
                                </ColumnModel>
                                <DirectEvents>
                                    <Command OnEvent="EjecutarComando">
                                        <ExtraParams>
                                            <ext:Parameter Name="ID_Certificado" Value="record.data.ID_Certificado" Mode="Raw" />
                                            <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                        </ExtraParams>
                                    </Command>
                                </DirectEvents>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                                    </ext:RowSelectionModel>
                                </SelectionModel>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="storeCertificados"
                                        DisplayInfo="true" DisplayMsg="Mostrando Colectivas {0} - {1} de {2}" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>

