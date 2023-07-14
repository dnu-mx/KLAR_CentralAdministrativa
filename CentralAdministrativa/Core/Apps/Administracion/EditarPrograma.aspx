<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="EditarPrograma.aspx.cs" Inherits="Administracion.EditarPrograma" %>

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

    <ext:Window ID="DialogEditarPrograma" runat="server">
        <Items>
            <ext:FormPanel ID="FormEditarPrograma" runat="server">
                <Items>
                    <ext:TextField ID="TxtID_Programa" runat="server" />
                    <ext:TextField ID="TxtProgramaClave" runat="server" />
                    <ext:TextField ID="TxtProgramaDescripcion" runat="server" />
                    <ext:SelectBox ID="SProgramaColectivaEmisor" runat="server" />
                    <ext:SelectBox ID="SProgramaVigencia" runat="server" />
                </Items>
                <Buttons>
                    <ext:Button ID="btnGrupoCuenta" runat="server" Text="Guardar" Icon="Disk">
                        <DirectEvents>
                            <Click OnEvent="GuardarPrograma_Click" />
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
                <Listeners>
                    <ClientValidation Handler="#{btnGrupoCuenta}.setDisabled(!valid);" />
                </Listeners>
            </ext:FormPanel>
        </Items>
    </ext:Window>

    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center>
            <ext:GridPanel ID="GridProgramas" runat="server" StoreID="StoreProgramas">
                <SelectionModel><ext:RowSelectionModel SingleSelect="true"/></SelectionModel>
                <BottomBar><ext:PagingToolbar ID="PagingToolbar1" runat="server" /></BottomBar>
                <DirectEvents><Command OnEvent="EditarPrograma_Click" /></DirectEvents>
            </ext:GridPanel>
        </Center>
    </ext:BorderLayout>

</asp:Content>
