<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="VerProductosFiltro.aspx.cs" Inherits="ComercioElectronico.VerProductosFiltro" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
    <script type="text/javascript">
        var submitValue = function (grid, hiddenFormat, format) {
            hiddenFormat.setValue(format);
            grid.submitData(false, { isUpload: true });
        };

        var yourRenderFunction = function (value) {

            //Here you can format the value
            if (value != null)
                return "$ " + value.toFixed(2);

            return "$ " + "0";
        };
    </script>

    <ext:Hidden ID="FormatType" runat="server" />
    <ext:Viewport runat="server" Layout="BorderLayout">
        <Items>
            <ext:FormPanel runat="server" Title="Filtros" Region="West" Split="true" Collapsible="true" 
                Layout="FormLayout" Padding="2">
                <Items>
                    <ext:ComboBox ID="ComboFamilia" runat="server" FieldLabel="Familia" DisplayField="Name"
                        ValueField="Id">
                        <Store>
                            <ext:Store runat="server" AutoDataBind="False">
                                <Model>
                                    <ext:Model runat="server">
                                        <Fields>
                                            <ext:ModelField Name="Id" />
                                            <ext:ModelField Name="Name" />
                                        </Fields>
                                    </ext:Model>
                                </Model>
                            </ext:Store>
                        </Store>
                    </ext:ComboBox>
                    <ext:TextField runat="server" FieldLabel="Sku" ID="Sku" MinValue="0" AllowBlank="true" />
                    <ext:TextField runat="server" FieldLabel="Nombre" ID="Nombre" MinValue="0" AllowBlank="true" />
                </Items>
                <Buttons>
                    <ext:Button runat="server" Text="Buscar" FormBind="true" Icon="Magnifier">
                        <DirectEvents>
                            <Click OnEvent="VerProductosFiltrados">
                                <EventMask ShowMask="true" Msg="Buscando Productos..." MinDelay="500" />
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                </Buttons>
            </ext:FormPanel>
            <ext:GridPanel ID="gridPanelBase" runat="server" Title="Productos" Frame="true" Region="Center">
                <Store>
                    <ext:Store runat="server" ID="MasterGrid" OnSubmitData="Store1_Submit">
                        <Model>
                            <ext:Model runat="server" IDProperty="sku">
                                <Fields>
                                    <ext:ModelField Name="sku" />
                                    <ext:ModelField Name="secuencia" />
                                    <ext:ModelField Name="activo" />
                                    <ext:ModelField Name="familia" />
                                    <ext:ModelField Name="nombre" />
                                    <ext:ModelField Name="descripcion" />
                                    <ext:ModelField Name="precioBase" />
                                    <ext:ModelField Name="precioPromocion" />
                                    <ext:ModelField Name="piezapresentacion" />
                                    <ext:ModelField Name="terminococcion" />
                                    <ext:ModelField Name="rasurado" />
                                    <ext:ModelField Name="colorplatos" />
                                    <ext:ModelField Name="numsushibanda" />
                                    <ext:ModelField Name="vegetariano" />
                                    <ext:ModelField Name="servidocrudo" />
                                    <ext:ModelField Name="picante" />
                                    <ext:ModelField Name="ComboFijo" />
                                    <ext:ModelField Name="idCombo" />
                                    <ext:ModelField Name="althtml" />
                                    <ext:ModelField Name="titlehtml" />
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
                        <ext:Column ColumnID="familia" Header="Familia" Sortable="true" DataIndex="familia" />
                        <ext:Column ColumnID="nombre" Header="Nombre" Sortable="true" DataIndex="nombre" />
                        <ext:Column ColumnID="descripcion" Header="Descripción" Sortable="true" DataIndex="descripcion" />
                        <ext:Column ColumnID="precioBase" Header="Precio Base" Sortable="true" DataIndex="precioBase">
                            <Renderer Fn="yourRenderFunction" />
                        </ext:Column>
                        <ext:Column ColumnID="precioPromocion" Header="Precio de Promoción" Sortable="true"
                            DataIndex="precioPromocion" Width="120">
                            <Renderer Fn="yourRenderFunction" />
                        </ext:Column>
                        <ext:Column ColumnID="piezapresentacion" Header="Pieza Presentación" Sortable="true"
                            DataIndex="piezapresentacion" Width="120" />
                        <ext:Column ColumnID="terminococcion" Header="Término de Cocción" Sortable="true"
                            DataIndex="terminococcion" Width="120" />
                        <ext:Column ColumnID="rasurado" Header="Rasurado" Sortable="true" DataIndex="rasurado" />
                        <ext:Column ColumnID="colorplatos" Header="Color de Platos" Sortable="true" DataIndex="colorplatos" />
                        <ext:Column ColumnID="numsushibanda" Header="No. Sushi Banda" Sortable="true" DataIndex="numsushibanda" />
                        <ext:Column ColumnID="vegetariano" Header="Vegetariano" Sortable="true" DataIndex="vegetariano" />
                        <ext:Column ColumnID="servidocrudo" Header="Servido Crudo" Sortable="true" DataIndex="servidocrudo" />
                        <ext:Column ColumnID="picante" Header="Picante" Sortable="true" DataIndex="picante" />
                        <ext:Column ColumnID="ComboFijo" Header="Combo Fijo" Sortable="true" DataIndex="ComboFijo" />
                        <ext:Column ColumnID="idCombo" Header="Id Combo" Sortable="true" DataIndex="idCombo" />
                        <ext:Column ColumnID="althtml" Header="Alt HTML" Sortable="true" DataIndex="althtml" />
                        <ext:Column ColumnID="titlehtml" Header="Title HTML" Sortable="true" DataIndex="titlehtml" />
                    </Columns>
                </ColumnModel>
                <View>
                    <ext:GridView runat="server" LoadMask="true" LoadingText="Cargando" />
                </View>
                <BottomBar>
                    <ext:PagingToolbar runat="server" DisplayInfo="true" DisplayMsg="Mostrando productos {0} - {1} de {2}" />
                </BottomBar>
                <TopBar>
                    <ext:Toolbar ID="Toolbar2" runat="server">
                        <Items>
                            <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                            <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" Icon="PageExcel">
                                <DirectEvents>
                                    <Click OnEvent="ExportGridToExcel" IsUpload="true" Success="Ext.Net.DirectMethods.Download({IsUpload:true})">
                                        <ExtraParams>
                                            <ext:Parameter Name="GridToExport" Value="Ext.encode(#{gridPanelBase}.getRowsValues({selectedOnly : false}))" Mode="Raw" />
                                        </ExtraParams>
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="Button4" runat="server" Text="Exportar a CSV" Icon="PageAttach">
                                <Listeners>
                                    <Click Handler="submitValue(#{gridPanelBase}, #{FormatType}, 'csv');" />
                                </Listeners>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
            </ext:GridPanel>
        </Items>
    </ext:Viewport>




</asp:Content>
