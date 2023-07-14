<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="ConsultarPrograma.aspx.cs" Inherits="Administracion.ConsultarPrograma" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Store ID="StoreProgramas" runat="server" OnRefreshData="StoreProgramas_Refresh">
        <Reader></Reader>
    </ext:Store>

    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center>
            <ext:GridPanel ID="GridProgramas" runat="server" StoreID="StoreProgramas">
                <SelectionModel><ext:RowSelectionModel SingleSelect="true"/></SelectionModel>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingToolbar1" runat="server" />
                </BottomBar>
            </ext:GridPanel>
        </Center>
    </ext:BorderLayout>

</asp:Content>
