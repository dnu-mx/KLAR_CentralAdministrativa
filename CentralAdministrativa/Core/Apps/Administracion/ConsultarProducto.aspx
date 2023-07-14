<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ConsultarProducto.aspx.cs" Inherits="Administracion.ConsultarProducto" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <script type="text/javascript">
        var checkNode = function (node) {
            MainContent_GridProductos.setVisible(false);
            MainContent_GridRangos.setVisible(false);
            MainContent_panelReglasMA.setVisible(false);
            MainContent_panelReglaPMA.setVisible(false);
            MainContent_panelTipoCuentaPMA.setVisible(false);
            MainContent_panelGpoCuentaPMA.setVisible(false);
            MainContent_panelGpoTarjetaPMA.setVisible(false);
            MainContent_panelTarjetaCtaPMA.setVisible(false);
            MainContent_panelValidaciones.setVisible(false);
            MainContent_GridVMA.setVisible(false);

            entityName = node.text;
            idGMA = node.id;

            if (node.text == 'Propiedades') {
                Administracion.LlenarGridProductos(idGMA, 0);
            }
            else if (node.text == 'Rangos') {
                Administracion.LlenarGridRangos(idGMA, 0);
            }
            else if (node.text == 'Reglas') {
                Administracion.LlenarGridReglasMA(idGMA, 0);
            }
            else if (node.text == 'Regla') {
                Administracion.LlenarFieldSetReglasPMA();
            }
            else if (node.text == 'Tipo de Cuenta') {
                Administracion.LlenarFieldSetTiposCtaPMA();
            }
            else if (node.text == 'Grupo de Cuenta') {
                Administracion.LlenarFieldSetGposCuentaPMA();
            }
            else if (node.text == 'Grupo de Tarjeta') {
                Administracion.LlenarGridGpoTarjetaPMA(idGMA);
            }
            else if (node.text == 'Tarjeta/Cuenta') {
                Administracion.LlenarFieldSetTarjetaCuentaPMA();
            }
            else if (node.text == 'Validaciones') {
                Administracion.LlenaGridVMA(MainContent_cmbCadenaComercial.value);
            }
        }

        function setGridName() {
            gridName = MainContent_GridVMA;
        }

        function refreshTree(tree, result) {
            var nodes = eval(result);

            if (nodes != null && nodes.length > 0) {
                tree.initChildren(nodes);
            } else {
                tree.getRootNode().removeChildren();
            }
        }

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:Store ID="StoreProductos" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_GrupoMA">
                <Fields>
                    <ext:RecordField Name="ID_GrupoMA" />
                    <ext:RecordField Name="Clave" />
                    <ext:RecordField Name="Descripcion" />
                    <ext:RecordField Name="ID_Vigencia" />
                    <ext:RecordField Name="Vigencia" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:Store ID="StoreVigencias" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_Vigencia">
                <Fields>
                    <ext:RecordField Name="ID_Vigencia" />
                    <ext:RecordField Name="Clave" />
                    <ext:RecordField Name="Descripcion" />
                    <ext:RecordField Name="ID_TipoVigencia" />
                    <ext:RecordField Name="TipoVigencia" />
                    <ext:RecordField Name="FechaIncial" />
                    <ext:RecordField Name="FechaFinal" />
                    <ext:RecordField Name="HoraInicial" />
                    <ext:RecordField Name="HoraFinal" />
                    <ext:RecordField Name="ID_Periodo" />
                    <ext:RecordField Name="Periodo" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:Store ID="StoreTipoVigencia" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_TipoVigencia">
                <Fields>
                    <ext:RecordField Name="ID_TipoVigencia" />
                    <ext:RecordField Name="Clave" />
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
                    <ext:RecordField Name="Cve_Periodo" />
                    <ext:RecordField Name="Descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:Store ID="StoreCadenaComercial" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_Colectiva">
                <Fields>
                    <ext:RecordField Name="ID_Colectiva" />
                    <ext:RecordField Name="NombreORazonSocial" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:Store ID="StoreRangos" runat="server">
        <Reader>
            <ext:JsonReader>
                <Fields>
                    <ext:RecordField Name="ID_Rango" />
                    <ext:RecordField Name="ID_GrupoMA" />
                    <ext:RecordField Name="Clave" />
                    <ext:RecordField Name="Descripcion" />
                    <ext:RecordField Name="Inicio" />
                    <ext:RecordField Name="Fin" />
                    <ext:RecordField Name="esActivo" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:Store ID="StoreGMAOrdenValidacion" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="Value">
                <Fields>
                    <ext:RecordField Name="Text" />
                    <ext:RecordField Name="Value" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:Store ID="StoreGMATipoElementoComparar" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_TipoElemento">
                <Fields>
                    <ext:RecordField Name="ID_TipoElemento" />
                    <ext:RecordField Name="Clave" />
                    <ext:RecordField Name="Descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:Store ID="StorePMA" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="ID_ParametroMultiasignacion">
                <Fields>
                    <ext:RecordField Name="ID_ValorParametroMultiasignacion" />
                    <ext:RecordField Name="ID_ParametroMultiasignacion" />
                    <ext:RecordField Name="ID_Entidad" />
                    <ext:RecordField Name="ID_CadenaComercial" />
                    <ext:RecordField Name="ID_Origen" />
                    <ext:RecordField Name="ID_Producto" />
                    <ext:RecordField Name="ID_Vigencia" />
                    <ext:RecordField Name="Nombre" />
                    <ext:RecordField Name="Descripcion" />
                    <ext:RecordField Name="Valor" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:Store ID="StoreReglas" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="Nombre">
                <Fields>
                    <ext:RecordField Name="ID_Regla" />
                    <ext:RecordField Name="Nombre" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:Store ID="StoreTiposCta" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="Descripcion">
                <Fields>
                    <ext:RecordField Name="ID_TipoCuenta" />
                    <ext:RecordField Name="ClaveTipoCuenta" />
                    <ext:RecordField Name="Descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:Store ID="StoreGpoCuenta" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="Descripcion">
                <Fields>
                    <ext:RecordField Name="ID_GrupoCuenta" />
                    <ext:RecordField Name="ClaveGrupoCuenta" />
                    <ext:RecordField Name="Descripcion" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>

    <ext:Store ID="StoreTarjetaCta" runat="server">
        <Reader>
            <ext:JsonReader IDProperty="Tarjeta">
                <Fields>
                    <ext:RecordField Name="ID_Cuenta" />
                    <ext:RecordField Name="ID_MA" />
                    <ext:RecordField Name="Tarjeta" />
                    <ext:RecordField Name="NombreTarjetahabiente" />
                </Fields>
            </ext:JsonReader>
        </Reader>
    </ext:Store>
     

    <ext:BorderLayout ID="BorderLayout5" runat="server">
        <North>
            <ext:Panel ID="PanelHeader" runat="server">
                <TopBar>
                    <ext:Toolbar ID="tbCadenaComercial" runat="server">
                        <Items>
                            <ext:ComboBox ID="cmbCadenaComercial"
                                runat="server"
                                FieldLabel="Cadena Comercial"
                                LabelAlign="Right"
                                AnchorHorizontal="80%"
                                EmptyText="Selecciona una Cadena Comercial"
                                StoreID="StoreCadenaComercial"
                                DisplayField="NombreORazonSocial"
                                ValueField="ID_Colectiva"
                                Width="300"
                                AllowBlank="false">
                                <Listeners>
                                    <Select Handler="if(this.isValid()) {
                                        #{pArbolMenu}.setTitle('Productos de ' + this.getRawValue());
                                        #{pArbolMenu}.expand();
                                        #{pArbolMenu}.setVisible(true); }" />
                                </Listeners>
                            </ext:ComboBox>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
            </ext:Panel>
        </North>
        <West>
            <ext:Panel ID="pArbolMenu" runat="server" Layout="accordion" Hidden="true" Collapsed="true"
                Width="300" MinWidth="300" MaxWidth="350" Split="true" Collapsible="true">
            </ext:Panel>
        </West>
        <Center>
            <ext:Panel ID="PanelCentral" runat="server">
                <Items>
                    <ext:GridPanel ID="GridProductos" runat="server" StoreID="StoreProductos" Hidden="true" Title="Propiedades del Producto" />
                    <ext:GridPanel ID="GridRangos" runat="server" StoreID="StoreRangos" Title="Configuraciones de Rangos" Hidden="true" />
                    <ext:Panel ID="panelReglasMA" runat="server" Title="Configuraciones de Reglas" Hidden="true">
                        <Items>
                            <ext:GridPanel ID="GridReglasMA" runat="server">
                                <Store>
                                    <ext:Store ID="StoreReglasMA" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Regla">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Regla" />
                                                    <ext:RecordField Name="ID_ReglaMultiasignacion" />
                                                    <ext:RecordField Name="ID_Entidad" />
                                                    <ext:RecordField Name="Nombre" />
                                                    <ext:RecordField Name="ID_Vigencia" />
                                                    <ext:RecordField Name="Vigencia" />
                                                    <ext:RecordField Name="Prioridad" />
                                                    <ext:RecordField Name="OrdenEjecucion" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <ColumnModel ID="ColumnModel6" runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_Regla" Hidden="true" />
                                        <ext:Column DataIndex="ID_ReglaMultiasignacion" Hidden="true" />
                                        <ext:Column DataIndex="ID_Entidad" Hidden="true" />
                                        <ext:Column DataIndex="Nombre" Header="Nombre" Width="280" />
                                         <ext:Column DataIndex="ID_Vigencia" Header="Vigencia" Width="120" />
                                        <ext:Column DataIndex="Prioridad" Header="Prioridad" Width="60" Align="Center" />
                                        <ext:Column DataIndex="OrdenEjecucion" Header="Orden de Ejecución" Width="120" Align="Center" />
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel SingleSelect="true" />
                                </SelectionModel>
                            </ext:GridPanel>
                        </Items>
                    </ext:Panel>
                    <ext:FormPanel ID="panelReglaPMA" runat="server" Title="Configuraciones de Parámetros" Hidden="true">
                        <Items>
                            <ext:GridPanel ID="GridPanelReglas" runat="server" StoreID="StoreReglas" Height="250" Layout="FitLayout" StripeRows="true"
                                    Header="false" Border="false">
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar4" runat="server">
                                        <Items>
                                            <ext:TextField ID="txtReglasPMA" EmptyText="Nombre Regla" Width="200" runat="server" />
                                            <ext:Button ID="btnReglasPMA" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnReglasPMA_Click" Before="var valid= #{panelReglaPMA}.getForm().isValid(); if (!valid) {} return valid;">
                                                        <ExtraParams>
                                                            <ext:Parameter Name="IdProducto" Value="idGMA" Mode="Raw" />
                                                        </ExtraParams>
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <ColumnModel ID="cmReglaPMA" runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_Regla" Hidden="true" />
                                        <ext:Column DataIndex="Nombre" Header="Regla" Width="400" />
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel SingleSelect="true" />
                                </SelectionModel>
                                <DirectEvents>
                                    <RowClick OnEvent="selectRegla_Event">
                                        <ExtraParams>
                                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridPanelReglas}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                            <ext:Parameter Name="IdProducto" Value="idGMA" Mode="Raw" />
                                        </ExtraParams>
                                    </RowClick>
                                </DirectEvents>
                            </ext:GridPanel>
                            <ext:GridPanel ID="GridReglaPMA" runat="server" StoreID="StorePMA" Title = "Parámetros de la Regla" 
                                    Height="500" Layout="FitLayout" StripeRows="true" >
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_ValorParametroMultiasignacion" Hidden="true" />
                                        <ext:Column DataIndex="ID_ParametroMultiasignacion" Hidden="true" />
                                        <ext:Column DataIndex="ID_Entidad" Hidden="true" />
                                        <ext:Column DataIndex="ID_CadenaComercial" Hidden="true" />
                                        <ext:Column DataIndex="ID_Origen" Hidden="true" />
                                        <ext:Column DataIndex="ID_Producto" Hidden="true" />
                                        <ext:Column DataIndex="Nombre" Header="Nombre" />
                                        <ext:Column DataIndex="Descripcion" Header="Descripcion" />
                                        <ext:Column DataIndex="Valor" Header="Valor" Width="250" />
                                        <ext:Column DataIndex="ID_Vigencia" Header="Vigencia" />
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true" />
                                </SelectionModel>
                            </ext:GridPanel>
                        </Items>
                    </ext:FormPanel>
                    <ext:FormPanel ID="panelTipoCuentaPMA" runat="server" Title="Configuraciones de Parámetros" Hidden="true">
                        <Items>
                            <ext:GridPanel ID="GridPanelTiposCta" runat="server" StoreID="StoreTiposCta" Height="250" Layout="FitLayout" StripeRows="true"
                                    Header="false" Border="false">
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar5" runat="server">
                                        <Items>
                                            <ext:TextField ID="txtTiposCtaPMA" EmptyText="Descripción" Width="200" runat="server" />
                                            <ext:Button ID="btnTiposCtaPMA" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnTiposCtaPMA_Click" Before="var valid= #{panelTipoCuentaPMA}.getForm().isValid(); if (!valid) {} return valid;">
                                                        <ExtraParams>
                                                            <ext:Parameter Name="IdProducto" Value="idGMA" Mode="Raw" />
                                                        </ExtraParams>
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <ColumnModel ID="ColumnModel2" runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_TipoCuenta" Hidden="true" />
                                        <ext:Column DataIndex="ClaveTipoCuenta" Header="Clave" />
                                        <ext:Column DataIndex="Descripcion" Header="Descripción" Width="250" />
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel SingleSelect="true" />
                                </SelectionModel>
                                <DirectEvents>
                                    <RowClick OnEvent="selectTipoCta_Event">
                                        <ExtraParams>
                                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridPanelTiposCta}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                            <ext:Parameter Name="IdProducto" Value="idGMA" Mode="Raw" />
                                        </ExtraParams>
                                    </RowClick>
                                </DirectEvents>
                            </ext:GridPanel>
                            <ext:GridPanel ID="GridTipoCuentaPMA" runat="server" StoreID="StorePMA" Title="Parámetros del Tipo de Cuenta" 
                                    Height="500" Layout="FitLayout" StripeRows="true">
                                <ColumnModel ID="ColumnModel3" runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_ValorParametroMultiasignacion" Hidden="true" />
                                        <ext:Column DataIndex="ID_ParametroMultiasignacion" Hidden="true" />
                                        <ext:Column DataIndex="ID_Entidad" Hidden="true" />
                                        <ext:Column DataIndex="ID_CadenaComercial" Hidden="true" />
                                        <ext:Column DataIndex="ID_Origen" Hidden="true" />
                                        <ext:Column DataIndex="ID_Producto" Hidden="true" />
                                        <ext:Column DataIndex="Nombre" Header="Nombre" />
                                        <ext:Column DataIndex="Descripcion" Header="Descripción" />
                                        <ext:Column Header="Valor" DataIndex="Valor" Width="300" />
                                        <ext:Column DataIndex="ID_Vigencia" Header="Vigencia" />
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel2" runat="server" SingleSelect="true" />
                                </SelectionModel>
                            </ext:GridPanel>
                        </Items>
                    </ext:FormPanel>
                    <ext:FormPanel ID="panelGpoCuentaPMA" runat="server" Title="Configuraciones de Parámetros" Hidden="true">
                        <Items>
                            <ext:GridPanel ID="GridPanelGposCuenta" runat="server" StoreID="StoreGpoCuenta" Height="250" Layout="FitLayout" StripeRows="true"
                                    Header="false" Border="false">
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar6" runat="server">
                                        <Items>
                                            <ext:TextField ID="txtGrupoCuenta" EmptyText="Descripción" Width="200" runat="server" />
                                            <ext:Button ID="btnGpoCuentaPMA" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnGpoCuentaPMA_Click" Before="var valid= #{panelGpoCuentaPMA}.getForm().isValid(); if (!valid) {} return valid;">
                                                        <ExtraParams>
                                                            <ext:Parameter Name="IdProducto" Value="idGMA" Mode="Raw" />
                                                        </ExtraParams>
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <ColumnModel ID="ColumnModel7" runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_GrupoCuenta" Hidden="true" />
                                        <ext:Column DataIndex="ClaveGrupoCuenta" Header="Clave" />
                                        <ext:Column DataIndex="Descripcion" Header="Descripción" Width="400" />
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel SingleSelect="true" />
                                </SelectionModel>
                                <DirectEvents>
                                    <RowClick OnEvent="selectGpoCuenta_Event">
                                        <ExtraParams>
                                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridPanelGposCuenta}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                            <ext:Parameter Name="IdProducto" Value="idGMA" Mode="Raw" />
                                        </ExtraParams>
                                    </RowClick>
                                </DirectEvents>
                            </ext:GridPanel>
                            <ext:GridPanel ID="GridGpoCuentaPMA" runat="server" StoreID="StorePMA" Title="Parámetros del Grupo de Cuenta" 
                                    Height="500" Layout="FitLayout" StripeRows="true">
                                <ColumnModel ID="ColumnModel4" runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_ValorParametroMultiasignacion" Hidden="true" />
                                        <ext:Column DataIndex="ID_ParametroMultiasignacion" Hidden="true" />
                                        <ext:Column DataIndex="ID_Entidad" Hidden="true" />
                                        <ext:Column DataIndex="ID_CadenaComercial" Hidden="true" />
                                        <ext:Column DataIndex="ID_Origen" Hidden="true" />
                                        <ext:Column DataIndex="ID_Producto" Hidden="true" />
                                        <ext:Column DataIndex="Nombre" Header="Nombre" />
                                        <ext:Column DataIndex="Descripcion" Header="Descripcion" />
                                        <ext:Column Header="Valor" DataIndex="Valor" Width="300" />
                                        <ext:Column DataIndex="ID_Vigencia" Header="Vigencia" />
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel3" runat="server" SingleSelect="true" />
                                </SelectionModel>
                            </ext:GridPanel>
                        </Items>
                    </ext:FormPanel>
                    <ext:FormPanel ID="panelGpoTarjetaPMA" runat="server" Title="Configuraciones de Parámetros" Hidden="true">
                        <Items>
                            <ext:GridPanel ID="GridGpoTarjetaPMA" runat="server" StoreID="StorePMA" AutoExpandColumn="Valor" Title="Grupo de Tarjeta">
                                <ColumnModel ID="ColumnModel5" runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_ValorParametroMultiasignacion" Hidden="true" />
                                        <ext:Column DataIndex="ID_ParametroMultiasignacion" Hidden="true" />
                                        <ext:Column DataIndex="ID_Entidad" Hidden="true" />
                                        <ext:Column DataIndex="ID_CadenaComercial" Hidden="true" />
                                        <ext:Column DataIndex="ID_Origen" Hidden="true" />
                                        <ext:Column DataIndex="ID_Producto" Hidden="true" />
                                        <ext:Column DataIndex="Nombre" Header="Nombre" />
                                        <ext:Column DataIndex="Descripcion" Header="Descripcion" />
                                        <ext:Column Header="Valor" DataIndex="Valor" Width="300" />
                                        <ext:Column DataIndex="ID_Vigencia" Header="Vigencia" />
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel4" runat="server" SingleSelect="true" />
                                </SelectionModel>
                            </ext:GridPanel>
                        </Items>
                    </ext:FormPanel>
                    <ext:FormPanel ID="panelTarjetaCtaPMA" runat="server" Title="Configuraciones de Parámetros" Hidden="true">
                        <Items>
                            <ext:GridPanel ID="GridPanelTarjetasCtas" runat="server" StoreID="StoreTarjetaCta" Height="250" Layout="FitLayout" StripeRows="true"
                                    Header="false" Border="false">
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar7" runat="server">
                                        <Items>
                                            <ext:NumberField ID="nfNumTarjeta" EmptyText="Número de Tarjeta" Width="100" runat="server"
                                                AllowNegative="False" MaxLength="16" />
                                            <ext:TextField ID="txtApPaterno" EmptyText="Apellido Paterno" Width="100" runat="server" />
                                            <ext:TextField ID="txtApMaterno" EmptyText="Apellido Materno" Width="100" runat="server" />
                                            <ext:TextField ID="txtNombre" EmptyText="Nombre" Width="200" runat="server" />
                                            <ext:Button ID="btnTarjetaCta" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnTarjetaCta_Click" Before="var valid= #{panelTarjetaCtaPMA}.getForm().isValid(); if (!valid) {} return valid;">
                                                        <ExtraParams>
                                                            <ext:Parameter Name="IdProducto" Value="idGMA" Mode="Raw" />
                                                        </ExtraParams>
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <ColumnModel ID="ColumnModel8" runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_Cuenta" Hidden="true" />
                                        <ext:Column DataIndex="ID_MA" Hidden="true" />
                                        <ext:Column DataIndex="Tarjeta" Header="Número de Tarjeta" Width="200" />
                                        <ext:Column DataIndex="NombreTarjetahabiente" Header="Tarjetahabiente" Width="400" />
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel SingleSelect="true" />
                                </SelectionModel>
                                <DirectEvents>
                                    <RowClick OnEvent="selectTarjetaCta_Event">
                                        <ExtraParams>
                                            <ext:Parameter Name="Values" Value="Ext.encode(#{GridPanelTarjetasCtas}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                            <ext:Parameter Name="IdProducto" Value="idGMA" Mode="Raw" />
                                        </ExtraParams>
                                    </RowClick>
                                </DirectEvents>
                            </ext:GridPanel>
                            <ext:GridPanel ID="GridTarjetaCtaPMA" runat="server" StoreID="StorePMA" AutoExpandColumn="Valor" Title="Tarjeta/Cuenta"
                                Height="500" Layout="FitLayout" StripeRows="true">
                                <ColumnModel ID="ColumnModel9" runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID_ValorParametroMultiasignacion" Hidden="true" />
                                        <ext:Column DataIndex="ID_ParametroMultiasignacion" Hidden="true" />
                                        <ext:Column DataIndex="ID_Entidad" Hidden="true" />
                                        <ext:Column DataIndex="ID_CadenaComercial" Hidden="true" />
                                        <ext:Column DataIndex="ID_Origen" Hidden="true" />
                                        <ext:Column DataIndex="ID_Producto" Hidden="true" />
                                        <ext:Column DataIndex="Nombre" Header="Nombre" />
                                        <ext:Column DataIndex="Descripcion" Header="Descripcion" />
                                        <ext:Column Header="Valor" DataIndex="Valor" Width="300" />
                                        <ext:Column DataIndex="ID_Vigencia" Header="Vigencia" />
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel5" runat="server" SingleSelect="true" />
                                </SelectionModel>
                            </ext:GridPanel>
                        </Items>
                    </ext:FormPanel>
                    <ext:Panel ID="panelValidaciones" runat="server" Title="Configuraciones de Validaciones" Hidden="true">
                        <Items>
                            <ext:TreeGrid ID="GridVMA" runat="server" AutoExpandColumn="Validacion" Hidden="true">
                                <Columns>
                                    <ext:TreeGridColumn DataIndex="ID_ValidadorMultiasignacion" Hidden="true" />
                                    <ext:TreeGridColumn DataIndex="Validacion" Header="Validación" Width="80" />
                                    <ext:TreeGridColumn DataIndex="Formula" Header="Formula" Width="80" />
                                    <ext:TreeGridColumn DataIndex="TipoValidacion" Header="Tipo de Validación" Width="150" />
                                    <ext:TreeGridColumn DataIndex="Declinar" Header="Declinar" Width="80" />
                                    <ext:TreeGridColumn DataIndex="Estatus" Header="Estatus" Width="80" />
                                    <ext:TreeGridColumn DataIndex="OrdenValidacion" Header="Orden Validación" Width="80" />
                                    <ext:TreeGridColumn DataIndex="PreReglas" Header="PreReglas" Width="80" />
                                    <ext:TreeGridColumn DataIndex="PostReglas" Header="PostReglas" Width="80" />
                                    <ext:TreeGridColumn DataIndex="Vigencia" Header="Vigencia" Width="80" />
                                    <ext:TreeGridColumn DataIndex="Prioridad" Header="Prioridad" Width="80" />
                                    <ext:TreeGridColumn DataIndex="NodosTrue" Hidden="true" />
                                    <ext:TreeGridColumn DataIndex="NodosFalse" Hidden="true" />
                                </Columns>
                                <Root>
                                    <ext:TreeNode NodeID="0" Text="Root" Icon="FolderGo" />
                                </Root>
                            </ext:TreeGrid>
                        </Items>
                    </ext:Panel>
                </Items>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
