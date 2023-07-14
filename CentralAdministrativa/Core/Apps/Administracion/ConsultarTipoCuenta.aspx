<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="ConsultarTipoCuenta.aspx.cs" Inherits="Administracion.ConsultarTipoCuenta" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Store ID="StoreTiposCuenta" runat="server" OnRefreshData="StoreTiposCuenta_Refresh">
        <Reader>
            <ext:JsonReader IDProperty="ID_TipoCuenta">
                <Fields>
                    <ext:RecordField Name="ID_TipoCuenta" />
                    <ext:RecordField Name="CodTipoCuentaISO" />
                    <ext:RecordField Name="ClaveTipoCuenta" />
                    <ext:RecordField Name="Descripcion" />
                    <ext:RecordField Name="GeneraDetalle" />
                    <ext:RecordField Name="GeneraCorte" />
                    <ext:RecordField Name="ID_Divisa" />
                    <ext:RecordField Name="Divisa" />
                    <ext:RecordField Name="ID_Periodo" />
                    <ext:RecordField Name="Periodo" />
                    <ext:RecordField Name="BreveDescripcion" />
                    <ext:RecordField Name="EditarSaldoGrid" />
                    <ext:RecordField Name="InteractuaCajero" />
                    <ext:RecordField Name="ID_NaturalezaCuenta" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center>
            <ext:GridPanel ID="GridTiposCuenta" runat="server" StoreID="StoreTiposCuenta">
                <SelectionModel><ext:RowSelectionModel SingleSelect="true"/></SelectionModel>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingToolbar1" runat="server" />
                </BottomBar>
            </ext:GridPanel>
        </Center>
    </ext:BorderLayout>

</asp:Content>
