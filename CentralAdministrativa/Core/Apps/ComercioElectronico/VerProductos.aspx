<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="VerProductos.aspx.cs" Inherits="ComercioElectronico.VerProductos" %>



<asp:Content runat="server" ContentPlaceHolderID="MainContent">
    <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="VerProductosFull"
        TypeName="ComercioElectronico.VerProductos" />
    <ext:Viewport runat="server" Layout="Fit">
        <Items>
            <ext:GridPanel ID="gridPanelBase" runat="server" Title="Productos" Frame="true" Height="600">
                <Store>
                    <ext:Store runat="server" DataSourceID="ObjectDataSource1">
                        <Model>
                            <ext:Model runat="server" IDProperty="sku">
                                <Fields>
                                    <ext:ModelField Name="sku" />
                                    <ext:ModelField Name="secuencia" />
                                    <ext:ModelField Name="activo" />
                                    <ext:ModelField Name="NombreFamilia" />
                                    <ext:ModelField Name="nombre" />
                                    <ext:ModelField Name="descripcion" />
                                    <ext:ModelField Name="IdCombo" />
                                </Fields>
                            </ext:Model>
                        </Model>
                    </ext:Store>
                </Store>
                <ColumnModel runat="server">
                    <Columns>
                        <ext:Column ColumnID="sku" Header="SKU" Sortable="true" DataIndex="sku" />
                        <ext:Column ColumnID="secuencia" Header="Secuencia" Sortable="true" DataIndex="secuencia" />
                        <ext:Column ColumnID="activo" Header="Activo" Sortable="true" DataIndex="activo" />
                        <ext:Column ColumnID="NombreFamilia" Header="Familia" Sortable="true" DataIndex="NombreFamilia" />
                        <ext:Column ColumnID="nombre" Header="Nombre" Sortable="true" DataIndex="nombre" />
                        <ext:Column ColumnID="descripcion" Header="Descripción" Sortable="true" DataIndex="descripcion" />
                        <ext:Column ColumnID="IdCombo" Header="Id Combo" Sortable="true" DataIndex="IdCombo" />
                    </Columns>
                </ColumnModel>
                <View>
                    <ext:GridView runat="server" LoadMask="true" LoadingText="Cargando" />
                </View>
                <BottomBar>
                    <ext:PagingToolbar runat="server" DisplayInfo="true"
                        DisplayMsg="Mostrando productos {0} - {1} of {2}" />
                </BottomBar>
            </ext:GridPanel>
        </Items>
    </ext:Viewport>
</asp:Content>
