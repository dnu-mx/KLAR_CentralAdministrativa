<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="AdministrarRoles.aspx.cs" Inherits="Usuarios.AdministrarRoles" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript">
    var loadPage = function (tabPanel, pagina, ID_rol,title) {
        var tab = tabPanel.getItem(ID_rol);

        if (!tab) {
            tab = tabPanel.add({
                id: ID_rol,
                title: title,
                closable: true,
                border: true,

                autoLoad: {
                    showMask: true,
                    url: pagina + "?guid="+ID_rol,
                    mode: "iframe",
                    split: true,
                    AutoScroll: true,
                    width: 200,
                    padding: 6,
                    //maskMsg: "Loading " + node.attributes.href + "..."
                    maskMsg: "Loading " + title + " ..."
                },
                listeners: {
                    update: {
                        fn: function (tab, cfg) {
                            cfg.iframe.setHeight(cfg.iframe.getSize().height);
                        },
                        scope: this,
                        single: true
                    }
                }
            });
        }
       
        tabPanel.setActiveTab(tab);
    }
    </script>
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Content>
            <ext:Store ID="SRoles" runat="server">
                <Reader>
                    <ext:JsonReader IDProperty="RoleId">
                        <Fields>
                            <ext:RecordField Name="RoleId" />
                            <ext:RecordField Name="RoleName" />
                            <ext:RecordField Name="Description" />
                        </Fields>
                    </ext:JsonReader>
                </Reader>
            </ext:Store>
        </Content>
        <Center Split="true">
            <ext:Panel runat="server" Title="Crear Nuevo Rol">
                <Items>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <North Split="true">
                            <ext:FormPanel ID="FormPanel1" runat="server" Border="false" Height="100" Padding="5">
                                <Items>
                                    <ext:TextField ID="txtRolName" TabIndex="3" FieldLabel="Nombre Corto" MaxLength="50" AnchorHorizontal="90%"
                                        runat="server" MsgTarget="Side" AllowBlank="false" Text="" Width="180" />
                                    <ext:TextField ID="txtDescripcion" TabIndex="3" FieldLabel="Descripcion" MaxLength="50"
                                        AnchorHorizontal="90%" runat="server" MsgTarget="Side" AllowBlank="false" Text=""
                                        Width="180" />
                                </Items>
                                <FooterBar>
                                    <ext:Toolbar ID="Toolbar2" runat="server" EnableOverflow="true">
                                        <Items>
                                            <ext:Button ID="btnGuardar" runat="server" Text="Añadir Rol" Icon="Add">
                                                <DirectEvents>
                                                    <Click OnEvent="btnGuardarConfig_Click" Before="var valid= #{FormPanel1}.getForm().isValid(); if (!valid) {} return valid;">
                                                        <EventMask ShowMask="true" Msg="Añadiendo Rol..." MinDelay="500" />
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </FooterBar>
                            </ext:FormPanel>
                        </North>
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanel1" runat="server" StoreID="SRoles" StripeRows="true" Header="false"
                                RemoveViewState="true" Border="false" AutoExpandColumn="Description">
                                <LoadMask ShowMask="false" />
                                <ColumnModel ID="ColumnModel2" runat="server">
                                </ColumnModel>
                                <SelectionModel>
                                  <ext:RowSelectionModel ID="RowSelectionModel2" runat="server" SingleSelect="true" />
                                </SelectionModel>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="SRoles" DisplayInfo="true"
                                        DisplayMsg="Mostrando Roles {0} - {1} de {2}" HideRefresh="true" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
        </Center>
        <East Split="true">
            <ext:Panel runat="server" Width="650" ID="Panel15" Collapsible="false" Collapsed="false"
                Title="Asignación de Menús">
                <Items>
                    <ext:BorderLayout ID="BorderlayCentro" runat="server">
                        <Center>
                            <ext:TabPanel ID="Pages" runat="server" Border="false" EnableTabScroll="false">
                            <Items>
                              <%--  <ext:Panel ID="PnlMenu2" runat="server" Title="Menu General" Layout="accordion" Split="true"
                                    Collapsible="true">
                                </ext:Panel>--%>
                                </Items>
                            </ext:TabPanel>
                        </Center>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
        </East>
    </ext:BorderLayout>
</asp:Content>
