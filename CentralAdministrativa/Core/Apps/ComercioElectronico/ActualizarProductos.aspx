<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ActualizarProductos.aspx.cs" 
    Inherits="ComercioElectronico.ActualizarProductos" %>


<asp:Content runat="server" ContentPlaceHolderID="MainContent">
     <ext:Viewport runat="server"  layout="BorderLayout"  >
        <Items>
             <ext:FormPanel ID="FormPanel1" runat="server"  Region="North" >
                <Items> 
					<ext:Toolbar ID="ToolbarConsulta" runat="server" Layout="HBoxLayout" BodyPadding="5" Region="North">
                        <Defaults>
                            <ext:Parameter Name="margin" Value="0 5 0 0" Mode="Value" />
                        </Defaults>
                        <LayoutConfig>
                            <ext:HBoxLayoutConfig Align="Middle" />
                        </LayoutConfig>
                        <Items>
                            <ext:FileUploadField ID="FileUploadField1" runat="server" ButtonText="Examinar..."
                                Icon="Magnifier" Flex="3" MarginSpec="0" />
                            <ext:Hidden ID="Hidden1" runat="server" Flex="1" />
                            <ext:Button ID="btnCargarArchivo" runat="server" Text="Cargar Archivo"
                                Icon="PageWhitePut" Flex="1">
                                <DirectEvents>
                                    <Click OnEvent="btnCargarArchivo_Click" IsUpload="true" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </Items>
            </ext:FormPanel>
			<ext:GridPanel ID="GridDatosArchivo" runat="server" StripeRows="true" Layout="fit" Region="Center">
                <Store>
                    <ext:Store runat="server"   AutoLoad="False"   >
                        <Model>
                            <ext:Model runat="server" IDProperty="SKU">
                                <Fields>
                                    <ext:ModelField Name="SKU" />
                                    <ext:ModelField Name="SECUENCIA"  />
                                    <ext:ModelField Name="ACTIVO" />
                                    <ext:ModelField Name="FAMILIA" />
                                    <ext:ModelField Name="NOMBRE" />
                                    <ext:ModelField Name="DESCRIPCION" />
                                    <ext:ModelField Name="PRECIOBASE" />
                                    <ext:ModelField Name="PRECIOPROMOCION" />
                                    <ext:ModelField Name="PIEZAPRESENTACION" />
                                    <ext:ModelField Name="TERMINOCOCCION" />
                                    <ext:ModelField Name="RASURADO" />
                                    <ext:ModelField Name="COLORPLATOS" />
                                    <ext:ModelField Name="NUMSUSHIBANDA" />
                                    <ext:ModelField Name="VEGETARIANO" />
                                    <ext:ModelField Name="SERVIDOCRUDO" />
                                    <ext:ModelField Name="PICANTE" />
                                    <ext:ModelField Name="COMBOFIJO" />
                                    <ext:ModelField Name="IDCOMBO" />  
                                    <ext:ModelField Name="ALTHTML" />  
                                    <ext:ModelField Name="TITLEHTML" />  
                                </Fields>
                            </ext:Model>
                        </Model>
                    </ext:Store>
                </Store>
                <ColumnModel ID="ColumnModel12" runat="server">
                    <Columns>
                        <ext:Column ColumnID="SKU" Header="SKU" Sortable="true" DataIndex="SKU"  />
                        <ext:Column ColumnID="SECUENCIA" Header="Secuencia" Sortable="true" DataIndex="SECUENCIA" />
                        <ext:Column ColumnID="ACTIVO" Header="Activo" Sortable="true" DataIndex="ACTIVO" />
                        <ext:Column ColumnID="FAMILIA" Header="Familia" Sortable="true" DataIndex="FAMILIA" />
                        <ext:Column ColumnID="NOMBRE" Header="Nombre" Sortable="true" DataIndex="NOMBRE" />
                        <ext:Column ColumnID="DESCRIPCION" Header="Descripción" Sortable="true" DataIndex="DESCRIPCION" />
                        <ext:Column ColumnID="PRECIOBASE" Header="Precio Base" Sortable="true" DataIndex="PRECIOBASE">
                            <Renderer Format="UsMoney" />
                        </ext:Column>
                        <ext:Column ColumnID="PRECIOPROMOCION" Header="Precio de Promoción" Sortable="true"
                            DataIndex="PRECIOPROMOCION" Width="120">
                            <Renderer Format="UsMoney" />
                        </ext:Column>
                        <ext:Column ColumnID="PIEZAPRESENTACION" Header="Pieza Presentación" Sortable="true" 
                            DataIndex="PIEZAPRESENTACION"  Width="120"/>
                        <ext:Column ColumnID="TERMINOCOCCION" Header="Término de Cocción" Sortable="true"
                            DataIndex="TERMINOCOCCION"  Width="120"/>
                        <ext:Column ColumnID="RASURADO" Header="Rasurado" Sortable="true" DataIndex="RASURADO" />
                        <ext:Column ColumnID="COLORPLATOS" Header="Color de Platos" Sortable="true" DataIndex="COLORPLATOS" />
                        <ext:Column ColumnID="NUMSUSHIBANDA" Header="No. Sushi Banda" Sortable="true" DataIndex="NUMSUSHIBANDA" />
                        <ext:Column ColumnID="VEGETARIANO" Header="Vegetariano" Sortable="true" DataIndex="VEGETARIANO" />
                        <ext:Column ColumnID="SERVIDOCRUDO" Header="Servido Crudo" Sortable="true" DataIndex="SERVIDOCRUDO" />
                        <ext:Column ColumnID="PICANTE" Header="Picante" Sortable="true" DataIndex="PICANTE" />
                        <ext:Column ColumnID="COMBOFIJO" Header="Combo Fijo" Sortable="true" DataIndex="COMBOFIJO" />
                        <ext:Column ColumnID="IDCOMBO" Header="Id Combo" Sortable="true" DataIndex="IDCOMBO" />
                        <ext:Column ColumnID="ALTHTML" Header="Alt Html" Sortable="true" DataIndex="ALTHTML" />
                        <ext:Column ColumnID="TITLEHTML" Header="Title Html" Sortable="true" DataIndex="TITLEHTML" />
                    </Columns>
                </ColumnModel>
                <BottomBar>
                    <ext:Toolbar ID="Toolbar1" runat="server">
                        <Items>
                            <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                            <ext:Button ID="btnAplicarCambios" runat="server" Text="Aplicar Cambios" Icon="Tick">
                                <DirectEvents>
                                    <Click OnEvent="btnAplicarCambios_Click">
                                        <EventMask ShowMask="true" Msg="Aplicando cambios..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </BottomBar>
            </ext:GridPanel>
        </Items>
    </ext:Viewport>
</asp:Content>
