<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="NuevoPrograma.aspx.cs" Inherits="Administracion.NuevoPrograma" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <ext:Store ID="StoreProgramas" runat="server" OnRefreshData="StoreProgramas_Refresh">
        <Reader></Reader>
    </ext:Store>

    <ext:Store ID="StoreVigencias" runat="server">
        <Reader></Reader>
    </ext:Store>

    <ext:Store ID="StoreColectivas" runat="server">
        <Reader></Reader>
    </ext:Store>

    <ext:Window ID="WNuevoPrograma" runat="server" Hidden="true" Modal="true" >
        <Items>
            <ext:FormPanel ID="FNuevoPrograma" runat="server" MonitorValid="true">
                <Items>
                    <ext:TextField ID="TxtProgramaClave" runat="server" />
                    <ext:TextField ID="TxtProgramaDescripcion" runat="server" />
                    <ext:SelectBox ID="SProgramaColectivaEmisor" runat="server" />
                    <ext:SelectBox ID="SProgramaVigencia" runat="server" />
                </Items>
                <Buttons>
                    <ext:Button ID="btnNuevoGrupoCuenta" runat="server" Text="Guardar" Icon="Disk">
                        <DirectEvents>
                            <Click OnEvent="GuardarNuevoPrograma_Click" />
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
                <Listeners>
                    <ClientValidation Handler="#{btnNuevoGrupoCuenta}.setDisabled(!valid);" />
                </Listeners>
            </ext:FormPanel>
        </Items>
    </ext:Window>

    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center>
            <ext:GridPanel ID="GridProgramas" runat="server" StoreID="StoreProgramas">
                <TopBar>
                    <ext:Toolbar ID="TProgramas" runat="server" >
                        <Items>
                            <ext:Button runat="server" Text="Nuevo Programa" Icon="Add">
                                <DirectEvents>
                                    <Click OnEvent="NuevoPrograma_Click" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <SelectionModel><ext:RowSelectionModel SingleSelect="true" /></SelectionModel>
                <BottomBar>
                    <ext:PagingToolbar runat="server" />
                </BottomBar>
            </ext:GridPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>