<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" ValidateRequest="false"
    CodeBehind="ConfigurarCVVDinamico.aspx.cs" Inherits="TpvWeb.ConfigurarCVVDinamico" %>




<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    
    <script type="text/javascript">
        var prepareToolbar = function (grid, toolbar, rowIndex, record) {
            if (record.get("CVVDinamicoStatus") == 1) { //Activo
                toolbar.items.get(0).hide();
            } else {
                toolbar.items.get(1).hide();
            }
        }
    </script>

</asp:Content>






<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    

    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Content>
            <ext:Hidden ID="FormatType" runat="server" />
        </Content>
        <Center Split="true">

            <ext:FormPanel ID="FormPanelConfigurarCVVDinamico" runat="server" Layout="FitLayout" >
            <Items>

            <ext:GridPanel ID="GridPanelConfigurarCVVDinamico" runat="server" StripeRows="true" Header="false" Border="false"
                Layout="FitLayout" AutoScroll="true" AutoExpandColumn="ClaveProducto">
                <LoadMask ShowMask="false" />

                <Store>
                    <ext:Store ID="StoreConfigurarCVVDinamico" runat="server"  RemoteSort="true" 
                         OnRefreshData="StoreConfigurarCVVDinamico_RefreshData" AutoLoad="false">

                        <AutoLoadParams>
                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                        </AutoLoadParams>
                        
                        
                        <Proxy>
                            <ext:PageProxy />
                        </Proxy> 

                        <DirectEventConfig IsUpload="true"/>



                        <Reader>
                            <ext:JsonReader IDProperty="Evento">
                                <Fields>
                                    <ext:RecordField Name="ClaveProducto" />
                                    <ext:RecordField Name="Producto" />     
                                    <ext:RecordField Name="CVVDinamicoStatus" />
                                    <ext:RecordField Name="CVVDinamico" />
                                    

                                </Fields>
                            </ext:JsonReader>
                        </Reader>


                    </ext:Store>
                </Store>



                <TopBar>
                    <ext:Toolbar ID="Toolbar2" runat="server" LabelWidth="70" LabelAlign="Right">
                        <Items>

                            <ext:ComboBox ID="cBoxCliente" runat="server" EmptyText="SubEmisor" ListWidth="200"
                                Width="150" DisplayField="NombreORazonSocial" Mode="Local" ValueField="ID_Colectiva"
                                AutoSelect="true" Editable="true" ForceSelection="true" MinChars="1" TypeAhead="true"
                                MatchFieldWidth="false" Name="colSubEmisor" AllowBlank="false">


                                <DirectEvents>
                                    <Select OnEvent="PrestableceProductos" Before="#{cBoxProducto}.clearValue();" />
                                </DirectEvents>


                                <Store>
                                    <ext:Store ID="StoreSubEmisores" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Colectiva">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Colectiva" />
                                                    <ext:RecordField Name="ClaveColectiva" />
                                                    <ext:RecordField Name="NombreORazonSocial" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                        <SortInfo Field="NombreORazonSocial" Direction="ASC" />
                                    </ext:Store>
                                </Store>
                            </ext:ComboBox>
                            
                            
                            <ext:ToolbarSpacer runat="server" Width="15" />
                            <ext:ComboBox ID="cBoxProducto" runat="server" EmptyText="Producto" ListWidth="200"
                                Width="150" DisplayField="Descripcion" ValueField="ID_Producto" AllowBlank="false">
                                <Store>
                                    <ext:Store ID="StoreProductos" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID_Producto">
                                                <Fields>
                                                    <ext:RecordField Name="ID_Producto" />
                                                    <ext:RecordField Name="Clave" />
                                                    <ext:RecordField Name="Descripcion" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                            </ext:ComboBox>




                            <ext:ToolbarSeparator runat="server" />
                            
                            <ext:Button ID="btnBuscarHide" runat="server" Hidden="true">
                                <Listeners>
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo CVV de Productos ...' });
                                        #{GridPanelConfigurarCVVDinamico}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>


                            
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Timeout="360000"
                                        Before="if (!#{cBoxCliente}.isValid() ||
                                                !#{cBoxProducto}.isValid()) { return false; } else { 
                                                #{GridPanelConfigurarCVVDinamico}.getStore().removeAll(); return true; }">
                                        <EventMask ShowMask="true" Msg="Buscando CVV de Productos..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>


                            <%--  %><ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                            RepConfigurarCVVDinamico.StopMask();" />
                                </DirectEvents>
                            </ext:Button> --%>

                            <ext:ToolbarSeparator runat="server" />


                          
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click" />
                                </DirectEvents>
                            </ext:Button>

                            

                        </Items>
                    </ext:Toolbar>
                </TopBar>

                 <ColumnModel ID="ColumnModel1" runat="server">
                    <Columns>


                        <ext:Column Header="Clave" Sortable="true" DataIndex="ClaveProducto" Width="150" />
                        <ext:Column Header="Producto" Sortable="true" DataIndex="Producto" Width="150" />
                        <ext:Column Header="CVV Dinámico" Sortable="true" DataIndex="CVVDinamico" Width="100" Align="Center" />


                        <ext:CommandColumn ColumnID="ComandosGrid" Header="Acción" Width="60"  Align="Center"  >
                            <PrepareToolbar Fn="prepareToolbar" />
                            <Commands>
                                <ext:GridCommand Icon="RecordRed" CommandName="Unlock" >
                                    <ToolTip Text="Activar" />
                                </ext:GridCommand>
                                <ext:GridCommand Icon="RecordGreen" CommandName="Lock">
                                    <ToolTip Text="Desactivar" />
                                </ext:GridCommand>
                            </Commands>
                        </ext:CommandColumn>

                    </Columns>
                </ColumnModel>


                <DirectEvents>
                    <Command OnEvent="EjecutarComando">
                        <Confirmation BeforeConfirm="if (command == 'Unlock' ) return false;"
                            ConfirmRequest="true" Title="Confirmación" Message="¿Estás seguro de desactivar el Evento Manual?" />
                        <EventMask ShowMask="true" Msg="Procesando..." MinDelay="500" />
                        <ExtraParams>
                            <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                            <ext:Parameter Name="Values" Value="Ext.encode(record.data)" Mode="Raw" />
                            <%--<ext:Parameter Name="ID_Evento" Value="Ext.encode(record.data.ID_Evento)" Mode="Raw" />--%>
                        </ExtraParams>
                    </Command>
                </DirectEvents>
               
                
                <BottomBar>
                    <ext:PagingToolbar ID="PagStoreConfCVV" runat="server" StoreID="StoreConfigurarCVVDinamico" DisplayInfo="true"
                        DisplayMsg="Mostrando CVV Dinámicos {0} - {1} de {2}" />
                </BottomBar>



            </ext:GridPanel>

            </Items>
            </ext:FormPanel>

        </Center>
    </ext:BorderLayout>
</asp:Content>
