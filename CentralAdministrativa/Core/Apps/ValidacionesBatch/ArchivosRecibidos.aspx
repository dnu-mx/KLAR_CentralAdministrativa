<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ArchivosRecibidos.aspx.cs" Inherits="ValidacionesBatch.ArchivosRecibidos" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">

        var prepareToolbar = function (grid, toolbar, rowIndex, record) {
            // for example hide 'Edit' button if price < 50


            if ((record.get("ID_Estatus") == 1))//Sin Procesar
            { //sin procesar
                //toolbar.items.get(0).hide(); //Delete
                toolbar.items.get(1).hide(); //sep
                toolbar.items.get(2).hide(); //asgina
                toolbar.items.get(3).hide(); //asgina
                toolbar.items.get(4).hide(); //asgina
                toolbar.items.get(5).hide(); //asgina
                toolbar.items.get(6).hide(); //asgina
                toolbar.items.get(7).hide(); //asgina
                toolbar.items.get(8).hide(); //asgina
                // toolbar.items.get(9).hide(); //asgina
                //toolbar.items.get(10).hide(); //asgina
            } else if ((record.get("ID_Estatus") == 2))//Colectiva Creada
            { //sin procesar
                toolbar.items.get(0).hide(); //Delete
                //toolbar.items.get(1).hide(); //sep
                toolbar.items.get(2).hide(); //asgina
                toolbar.items.get(3).hide(); //asgina
                toolbar.items.get(4).hide(); //asgina
                toolbar.items.get(5).hide(); //asgina
                toolbar.items.get(6).hide(); //asgina
                toolbar.items.get(7).hide(); //asgina
                toolbar.items.get(8).hide(); //asgina
                //toolbar.items.get(9).hide(); //asgina
                //toolbar.items.get(10).hide(); //asgina
            }
            else if ((record.get("ID_Estatus") == 3))//Cuentas Creadas
            { //sin procesar
                toolbar.items.get(0).hide(); //Delete
                toolbar.items.get(1).hide(); //sep
                //toolbar.items.get(2).hide(); //asgina
                toolbar.items.get(3).hide(); //asgina
                toolbar.items.get(4).hide(); //asgina
                toolbar.items.get(5).hide(); //asgina
                toolbar.items.get(6).hide(); //asgina
                toolbar.items.get(7).hide(); //asgina
                toolbar.items.get(8).hide(); //asgina
                //toolbar.items.get(9).hide(); //asgina
                //toolbar.items.get(10).hide(); //asgina
            }
            else if ((record.get("ID_Estatus") == 4))//Abono Realizado
            { //sin procesar
                toolbar.items.get(0).hide(); //Delete
                toolbar.items.get(1).hide(); //sep
                toolbar.items.get(2).hide(); //asgina
                //toolbar.items.get(3).hide(); //asgina
                toolbar.items.get(4).hide(); //asgina
                toolbar.items.get(5).hide(); //asgina
                toolbar.items.get(6).hide(); //asgina
                toolbar.items.get(7).hide(); //asgina
                toolbar.items.get(8).hide(); //asgina
                // toolbar.items.get(9).hide(); //asgina
                //toolbar.items.get(10).hide(); //asgina
            }
            else if ((record.get("ID_Estatus") == 5))//Procesada
            { //sin procesar
                toolbar.items.get(0).hide(); //Delete
                toolbar.items.get(1).hide(); //sep
                toolbar.items.get(2).hide(); //asgina
                toolbar.items.get(3).hide(); //asgina
                //toolbar.items.get(4).hide(); //asgina
                toolbar.items.get(5).hide(); //asgina
                toolbar.items.get(6).hide(); //asgina
                toolbar.items.get(7).hide(); //asgina
                toolbar.items.get(8).hide(); //asgina
                toolbar.items.get(9).hide(); //asgina
                toolbar.items.get(10).hide(); //asgina
            }
            else if ((record.get("ID_Estatus") == 8))//Error al Crear Colectiva
            { //sin procesar
                toolbar.items.get(0).hide(); //Delete
                toolbar.items.get(1).hide(); //sep
                toolbar.items.get(2).hide(); //asgina
                toolbar.items.get(3).hide(); //asgina
                toolbar.items.get(4).hide(); //asgina
                //toolbar.items.get(5).hide(); //asgina
                toolbar.items.get(6).hide(); //asgina
                toolbar.items.get(7).hide(); //asgina
                toolbar.items.get(8).hide(); //asgina
                // toolbar.items.get(9).hide(); //asgina
                //toolbar.items.get(10).hide(); //asgina
            }
            else if ((record.get("ID_Estatus") == 9))//Error al Crear Cuentas
            { //sin procesar
                toolbar.items.get(0).hide(); //Delete
                toolbar.items.get(1).hide(); //sep
                toolbar.items.get(2).hide(); //asgina
                toolbar.items.get(3).hide(); //asgina
                toolbar.items.get(4).hide(); //asgina
                toolbar.items.get(5).hide(); //asgina
                //toolbar.items.get(6).hide(); //asgina
                toolbar.items.get(7).hide(); //asgina
                toolbar.items.get(8).hide(); //asgina
                //toolbar.items.get(9).hide(); //asgina
                //toolbar.items.get(10).hide(); //asgina
            }
            else if ((record.get("ID_Estatus") == 10))//Error al Crear Deposito
            { //sin procesar
                toolbar.items.get(0).hide(); //Delete
                toolbar.items.get(1).hide(); //sep
                toolbar.items.get(2).hide(); //asgina
                toolbar.items.get(3).hide(); //asgina
                toolbar.items.get(4).hide(); //asgina
                toolbar.items.get(5).hide(); //asgina
                toolbar.items.get(6).hide(); //asgina
                //toolbar.items.get(7).hide(); //asgina
                toolbar.items.get(8).hide(); //asgina
                //toolbar.items.get(9).hide(); //asgina
                //toolbar.items.get(10).hide(); //asgina
            }
            else if ((record.get("ID_Estatus") == 11))//Error al Crear en Club Escala
            { //sin procesar
                toolbar.items.get(0).hide(); //Delete
                toolbar.items.get(1).hide(); //sep
                toolbar.items.get(2).hide(); //asgina
                toolbar.items.get(3).hide(); //asgina
                toolbar.items.get(4).hide(); //asgina
                toolbar.items.get(5).hide(); //asgina
                toolbar.items.get(6).hide(); //asgina
                toolbar.items.get(7).hide(); //asgina
                //toolbar.items.get(8).hide(); //asgina
                // toolbar.items.get(9).hide(); //asgina
                //toolbar.items.get(10).hide(); //asgina
            }


        };

        var prepareToolbarArch = function (grid, toolbar, rowIndex, record) {
            // for example hide 'Edit' button if price < 50


            if (record.get("ID_Estatus") == 1) { //sin procesar
                //toolbar.items.get(0).hide(); //Delete
                toolbar.items.get(1).hide(); //sep
                toolbar.items.get(2).hide(); //asgina
                toolbar.items.get(3).hide(); //asgina
            } else if (record.get("ID_Estatus") == 2) { //Parcialmente procesado 

                toolbar.items.get(0).hide(); //Delete
                //toolbar.items.get(1).hide(); //sep
                toolbar.items.get(2).hide(); //asgina
                toolbar.items.get(3).hide(); //asgina
            }
            else if (record.get("ID_Estatus") == 3) { //Procesado

                toolbar.items.get(0).hide(); //Delete
                toolbar.items.get(1).hide(); //sep
                //toolbar.items.get(2).hide(); //asgina
                toolbar.items.get(3).hide(); //asgina
            } else if (record.get("ID_Estatus") == 5) { //Procesado

                toolbar.items.get(0).hide(); //Delete
                toolbar.items.get(1).hide(); //sep
                toolbar.items.get(2).hide(); //asgina
                toolbar.items.get(3).hide(); //asgina
            }
            else { //Error

                toolbar.items.get(0).hide(); //Delete
                toolbar.items.get(1).hide(); //sep
                toolbar.items.get(2).hide(); //asgina
                // toolbar.items.get(3).hide(); //asgina
            }


        };

        var prepareToolbarAccion = function (grid, toolbar, rowIndex, record) {
            // for example hide 'Edit' button if price < 50


            if (record.get("ID_Estatus") == 5) { //sin procesar
                toolbar.items.get(0).hide(); //Delete
            } else { //Error

                // toolbar.items.get(0).hide(); //Delete

            }


        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center Split="true" Collapsible="false">
            <ext:Panel ID="Panel1" runat="server" Title="Archivos que han sido Importados" Collapsed="false"
                Layout="Fit" AutoScroll="true" Height="440">
                <Content>
                    <ext:Store ID="storeArchivo" runat="server" OnRefreshData="RefreshArchivos">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Archivo">
                                <Fields>
                                    <ext:RecordField Name="ResultadoProceso" />
                                    <ext:RecordField Name="ID_Archivo" />
                                    <ext:RecordField Name="Nombre" />
                                    <ext:RecordField Name="FechaRecepcion" />
                                    <ext:RecordField Name="FechaProceso" />
                                    <ext:RecordField Name="CA_Usuario" />
                                    <ext:RecordField Name="ID_Estatus" />
                                    <ext:RecordField Name="NombreArchivo" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:BorderLayout ID="BorderLayout3" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanel2" runat="server" StoreID="storeArchivo" StripeRows="true"
                                Header="false" Border="false" AutoExpandColumn="NombreArchivo">
                                <LoadMask ShowMask="false" />
                                <DirectEvents>
                                    <Command OnEvent="EjecutarComando">
                                        <ExtraParams>
                                            <ext:Parameter Name="ID_Archivo" Value="record.data.ID_Archivo" Mode="Raw">
                                            </ext:Parameter>
                                            <ext:Parameter Name="ID_Empleado" Value="0" Mode="Value">
                                            </ext:Parameter>
                                            <ext:Parameter Name="Comando" Value="command" Mode="Raw">
                                            </ext:Parameter>
                                        </ExtraParams>
                                    </Command>
                                </DirectEvents>
                                <ColumnModel ID="ColumnModel2" runat="server">
                                    <Columns>
                                        <ext:CommandColumn Header="Estatus" Width="90">
                                            <PrepareToolbar Fn="prepareToolbarArch" />
                                            <Commands>
                                                <ext:GridCommand ToolTip-Text="Sin Procesar" Icon="FlagWhite">
                                                </ext:GridCommand>
                                                <ext:GridCommand ToolTip-Text="Parcialmente Procesado" Icon="FlagOrange">
                                                </ext:GridCommand>
                                                <ext:GridCommand ToolTip-Text="Procesado" Icon="FlagGreen">
                                                </ext:GridCommand>
                                                <ext:GridCommand ToolTip-Text="Con Errores" CommandName="" Icon="FlagRed">
                                                </ext:GridCommand>
                                                <ext:CommandSeparator />
                                                <ext:GridCommand ToolTip-Text="Descargar Resultado de Proceso" CommandName="DescargarResultado" Icon="DiskDownload">
                                                </ext:GridCommand>
                                                 <ext:CommandSeparator />
                                                <ext:GridCommand ToolTip-Text="Descargar Resultado de Proceso en Excel" CommandName="DescargarXLS" Icon="PageExcel">
                                                </ext:GridCommand>
                                            </Commands>
                                        </ext:CommandColumn>
                                        <ext:Column ColumnID="ID_Archivo" Header="ID_Archivo" Sortable="true" DataIndex="ID_Archivo" />
                                        <ext:DateColumn ColumnID="FechaRecepcion" Header="Fecha Recepcion" Sortable="true" DataIndex="FechaRecepcion"
                                            Format="yyyy-MM-dd HH:mm" />
                                        <ext:DateColumn ColumnID="FechaProceso" Header="Fecha Proceso" Sortable="true" DataIndex="FechaProceso"
                                            Format="yyyy-MM-dd HH:mm" />
                                        <%--<ext:Column ColumnID="CA_Usuario" Header="CA_Usuario" Sortable="true" DataIndex="CA_Usuario" />--%>
                                        <ext:Column ColumnID="NombreArchivo" Header="Ubicación Archivo" Sortable="true" DataIndex="NombreArchivo" />
                                    </Columns>
                                </ColumnModel>
                                <DirectEvents>
                                    <RowDblClick OnEvent="GridEmpleados_DblClik">
                                        <ExtraParams>
                                            <ext:Parameter Name="ID_Archivo" Value="this.getRowsValues({ selectedOnly: true })[0].ID_Archivo"
                                                Mode="Raw" />
                                        </ExtraParams>
                                    </RowDblClick>
                                    <Command OnEvent="DescargaArchivo" IsUpload="true">
                                        <Confirmation ConfirmRequest="true" Title="Confirmación" Message="¿Confirmas Ejecutar la Acción Seleccionada?" />
                                        <ExtraParams>
                                            <ext:Parameter Name="ID_Archivo" Value="record.data['ID_Archivo']"
                                                Mode="Raw" />
                                            <ext:Parameter Name="Comando" Value="command" Mode="Raw" />
                                        </ExtraParams>
                                    </Command>
                                </DirectEvents>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel2" runat="server" SingleSelect="true">
                                        <DirectEvents>
                                            <RowDeselect OnEvent="QuitarSeleccion">
                                            </RowDeselect>
                                        </DirectEvents>
                                    </ext:RowSelectionModel>
                                </SelectionModel>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar2" runat="server" StoreID="storeArchivo" DisplayInfo="true"
                                        DisplayMsg="Elementos Importados {0} - {1} de {2}" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </Center>
        <East Split="true" Collapsible="true">
            <ext:Panel ID="Panel2" runat="server" Title="Detalle del Archivo Seleccionado" Collapsed="true"
                Layout="Fit" AutoScroll="true" Width="600">
                <Content>
                    <ext:Store ID="storeEmpleados" runat="server" OnRefreshData="RefreshDetalles">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_ArchivoDetalle">
                                <Fields>
                                    <ext:RecordField Name="ID_ArchivoDetalle" />
                                    <ext:RecordField Name="Detalle" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:BorderLayout ID="BorderLayout2" runat="server">
                        <Center Split="true">
                            <ext:GridPanel ID="GridPanel1" runat="server" StoreID="storeEmpleados" StripeRows="true"
                                Header="false" Border="false">
                                <LoadMask ShowMask="false" />

                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                        <ext:Column Header="No. " Sortable="true" DataIndex="ID_ArchivoDetalle" />
                                        <ext:Column ColumnID="Detalle" Header="Detalle" Sortable="true" Width="500" DataIndex="Detalle" />
                                        <%-- <ext:Column ColumnID="APaterno" Header="Apellido Paterno" Sortable="true" DataIndex="APaterno" />
                                        <ext:Column ColumnID="AMaterno" Header="Apellido Materno" Sortable="true" DataIndex="AMaterno" />
                                        <ext:DateColumn ColumnID="FechaNacimiento" Header="Fecha Nacimiento" Sortable="true"
                                            DataIndex="FechaNacimiento" Format="yyyy-MM-dd" />
                                        <ext:Column ColumnID="TelefonoMovil" Header="TelefonoMovil" Sortable="true" DataIndex="TelefonoMovil" />
                                        <ext:Column ColumnID="EmailEmpresarial" Header="Email Empresa" Sortable="true" DataIndex="EmailEmpresarial" />
                                        <ext:Column ColumnID="EmailPersonal" Header="EmailPersonal" Sortable="true" DataIndex="EmailPersonal" />
                                        <ext:Column ColumnID="LimiteCompra" Header="LimiteCompra" Sortable="true" DataIndex="LimiteCompra">
                                            <Renderer Format="UsMoney" />
                                        </ext:Column>
                                        <ext:Column ColumnID="CicloNominal" Header="CicloNominal" Sortable="true" DataIndex="CicloNominal" />--%>
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" SingleSelect="true">
                                        <DirectEvents>
                                            <%-- <RowSelect OnEvent="RowSelect" Buffer="100">
                                                <EventMask ShowMask="true" Target="CustomTarget" CustomTarget="#{FormPanel1}" />
                                                <ExtraParams>
                                                    <ext:Parameter Name="AfiliciacionID" Value="this.getSelected().id" Mode="Raw" />
                                                </ExtraParams>
                                            </RowSelect>--%>
                                        </DirectEvents>
                                    </ext:RowSelectionModel>
                                </SelectionModel>
                                <BottomBar>
                                    <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="storeEmpleados" DisplayInfo="true"
                                        DisplayMsg="Elementos Importados {0} - {1} de {2}" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </East>
    </ext:BorderLayout>
</asp:Content>
