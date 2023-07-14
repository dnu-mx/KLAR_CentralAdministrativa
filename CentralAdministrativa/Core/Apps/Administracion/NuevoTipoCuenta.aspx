<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="NuevoTipoCuenta.aspx.cs" Inherits="Administracion.NuevoTipoCuenta" %>

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

    <ext:Store ID="StoreCodTipoCuentaISO" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="CodTipoCuentaISO">
                <Fields>
                    <ext:RecordField Name="CodTipoCuentaISO" />
                    <ext:RecordField Name="Descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:Store ID="StoreDivisas" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_Divisa">
                <Fields>
                    <ext:RecordField Name="ID_Divisa" />
                    <ext:RecordField Name="Descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:Store ID="StorePeriodos" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_Periodo">
                <Fields>
                    <ext:RecordField Name="ID_Periodo" />
                    <ext:RecordField Name="Descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:Window ID="Dialog_NuevoTipoCuenta" runat="server">
        <Items>
            <ext:BorderLayout runat="server">
                <Center>
                    <ext:FormPanel ID="FormNuevoTipoCuenta" runat="server" Padding="10" MonitorValid="true">
                        <Items>
                            <ext:TextField ID="TxtID_TipoCuenta" runat="server" />
		                    <ext:SelectBox ID="SelectCodTipoCuentaISO" StoreID="StoreCodTipoCuentaISO" runat="server" />
		                    <ext:TextField ID="TxtClaveTipoCuenta" runat="server" />
		                    <ext:TextField ID="TxtDescripcion" runat="server" />
		                    <ext:SelectBox ID="SelectGeneraDetalle" runat="server" />
		                    <ext:SelectBox ID="SelectGeneraCorte" runat="server" />
                            <ext:SelectBox ID="SelectDivisa" StoreID="StoreDivisas" runat="server" />
                            <ext:SelectBox ID="SelectPeriodo" StoreID="StorePeriodos" runat="server" />
		                    <ext:TextField ID="TxtBreveDescripcion" runat="server" />
		                    <ext:SelectBox ID="SelectEditarSaldoGrid" runat="server" />
		                    <ext:SelectBox ID="SelectInteractuaCajero" runat="server" />
                        </Items>
                        <Buttons>
                            <ext:Button ID="btnGuardarTipoCuenta" runat="server" Icon="Disk" Text="Guardar">
                                <DirectEvents>
                                    <Click OnEvent="GuardarTipoCuenta_Event" />
                                </DirectEvents>
                            </ext:Button>
                        </Buttons>
                        <Listeners><ClientValidation Handler="#{btnGuardarTipoCuenta}.setDisabled(!valid);" /></Listeners>
                    </ext:FormPanel>
                </Center>
            </ext:BorderLayout>
        </Items>
    </ext:Window>

    <ext:BorderLayout runat="server">
        <Center>
            <ext:GridPanel ID="GridTiposCuenta" runat="server" StoreID="StoreTiposCuenta">
                <TopBar>
                    <ext:Toolbar ID="Toolbar1" runat="server">
                        <Items>
                            <ext:Button ID="Button1" runat="server" Text="Nuevo Tipo Cuenta" Icon="Add">
                                <DirectEvents>
                                    <Click OnEvent="NuevoTipoCuenta_Event" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <SelectionModel>
                    <ext:RowSelectionModel SingleSelect="true">
                    </ext:RowSelectionModel>
                </SelectionModel>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingToolbar1" runat="server" />
                </BottomBar>
            </ext:GridPanel>
        </Center>
    </ext:BorderLayout>

</asp:Content>
