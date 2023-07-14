﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Reporte_OperacionesConSaldo.aspx.cs" Inherits="TpvWeb.Reporte_OperacionesConSaldo" %>

<%@ Import Namespace="System.Xml.Xsl" %>
<%@ Import Namespace="System.Xml" %>
<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var onKeyUp = function (field) {
            var v = this.processValue(this.getRawValue()),
                field;

            if (this.startDateField) {
                field = Ext.getCmp(this.startDateField);
                field.setMaxValue();
                this.dateRangeMax = null;
            } else if (this.endDateField) {
                field = Ext.getCmp(this.endDateField);
                field.setMinValue();
                this.dateRangeMin = null;
            }

            field.validate();
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true" Collapsible="true">
            <ext:FormPanel ID="FormOpsConSaldo" Width="350" Title="Selecciona los Filtros" runat="server"
                Border="false" Layout="Fit">
                <Content>
                    <ext:Hidden ID="FormatType" runat="server" />
                    <ext:Store ID="StoreCadenaComercial" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                                <Fields>
                                    <ext:RecordField Name="NombreORazonSocial" />
                                    <ext:RecordField Name="ClaveColectiva" />
                                    <ext:RecordField Name="ID_Colectiva" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="NombreORazonSocial"  Direction="ASC"  />
                    </ext:Store>
                    <ext:Store ID="StoreSucursal" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                                <Fields>
                                    <ext:RecordField Name="NombreORazonSocial" />
                                    <ext:RecordField Name="ClaveColectiva" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="NombreORazonSocial"  Direction="ASC"  />
                    </ext:Store>
                    <ext:Store ID="StoreAfiliacion" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                                <Fields>
                                    <ext:RecordField Name="NombreORazonSocial" />
                                    <ext:RecordField Name="ClaveColectiva" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="NombreORazonSocial"  Direction="ASC"  />
                    </ext:Store>
                    <ext:Store ID="StoreTerminal" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                                <Fields>
                                    <ext:RecordField Name="NombreORazonSocial" />
                                    <ext:RecordField Name="ClaveColectiva" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="NombreORazonSocial"  Direction="ASC"  />
                    </ext:Store>
                      <ext:Store ID="StoreBeneficiario" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                                <Fields>
                                    <ext:RecordField Name="NombreORazonSocial" />
                                    <ext:RecordField Name="ClaveColectiva" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="NombreORazonSocial"  Direction="ASC"  />
                    </ext:Store>
                    <ext:Store ID="StoreOperador" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_Colectiva">
                                <Fields>
                                    <ext:RecordField Name="NameFin" />
                                    <ext:RecordField Name="ClaveColectiva" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                    <ext:Store ID="StoreEstatus" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="ID_EstatusOperacion">
                                <Fields>
                                    <ext:RecordField Name="ID_EstatusOperacion" />
                                    <ext:RecordField Name="Descripcion" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="Descripcion"  Direction="ASC"  />
                    </ext:Store>
                </Content>
                <Items>
                    <ext:Panel runat="server" Layout="FitLayout" Padding="10" Border="false">
                        <Items>
                            <ext:DateField ID="datInicio" runat="server" Vtype="daterange" FieldLabel="Fecha Inicial"
                                AllowBlank="false" MsgTarget="Side" Format="yyyy-MM-dd"
                                TabIndex="1"  MaxLength="12" Width="300" EnableKeyEvents="true" MaxWidth="300"  >
                                <CustomConfig>
                                    <ext:ConfigItem Name="endDateField" Value="#{datFinal}" Mode="Value" />
                                   </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:DateField ID="datFinal" runat="server" Vtype="daterange" FieldLabel="Fecha Final" 
                                AllowBlank="false"  MaxLength="12" Width="300" MsgTarget="Side" Format="yyyy-MM-dd" TabIndex="2" EnableKeyEvents="true">
                                <CustomConfig>
                                    <ext:ConfigItem Name="startDateField" Value="#{datInicio}" Mode="Value" />
                                </CustomConfig>
                                <Listeners>
                                    <KeyUp Fn="onKeyUp" />
                                </Listeners>
                            </ext:DateField>
                            <ext:ComboBox ID="cmbCadenaComercial" TabIndex="3" FieldLabel="Cadena Comercial" EmptyText="Todas"
                                Resizable="true" ListWidth="350" Width="300" runat="server" StoreID="StoreCadenaComercial"
                                DisplayField="NombreORazonSocial" ValueField="ID_Colectiva" Mode="Local" AutoSelect="true"
                                Editable="true" ForceSelection="true" TypeAhead="true" MinChars="1"
                                MatchFieldWidth="false" Name="colCadena">
                                <DirectEvents>
                                    <Select OnEvent="LlenaSucursales" Before="#{cmbSucursal}.reset();
                                        #{cmbAfiliacion}.reset(); #{cmbTerminal}.reset(); #{cmbOperador}.reset();">
                                        <EventMask ShowMask="true" Msg="Estableciendo Sucursales..." MinDelay="200" />
                                    </Select>
                                </DirectEvents>
                                 <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cmbSucursal" TabIndex="4" FieldLabel="Sucursal" EmptyText="Todas"
                                Resizable="true" ListWidth="350" Width="300" runat="server" StoreID="StoreSucursal"
                                DisplayField="NombreORazonSocial" ValueField="ClaveColectiva">
                                <DirectEvents>
                                    <Select OnEvent="LlenaAfiliaciones" Before="#{cmbAfiliacion}.reset();
                                        #{cmbTerminal}.reset(); #{cmbOperador}.reset();">
                                        <EventMask ShowMask="true" Msg="Estableciendo Afiliaciones..." MinDelay="200" />
                                    </Select>
                                </DirectEvents>
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cmbAfiliacion" TabIndex="5" FieldLabel="Afiliación" Width="300"
                                Resizable="true" ListWidth="350" EmptyText="Todas" runat="server" StoreID="StoreAfiliacion"
                                DisplayField="NombreORazonSocial" ValueField="ClaveColectiva">
                                <DirectEvents>
                                    <Select OnEvent="LlenaTerminales" Before="#{cmbTerminal}.reset();
                                        #{cmbOperador}.reset();">
                                        <EventMask ShowMask="true" Msg="Estableciendo Terminales..." MinDelay="200" />
                                    </Select>
                                </DirectEvents>
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cmbTerminal" FieldLabel="Terminal" TabIndex="6" EmptyText="Todas"
                                Resizable="true" ListWidth="350" Width="300" runat="server" StoreID="StoreTerminal"
                                DisplayField="NombreORazonSocial" ValueField="ClaveColectiva">
                                <DirectEvents>
                                    <Select OnEvent="LlenaOperadores" Before="#{cmbOperador}.reset();">
                                        <EventMask ShowMask="true" Msg="Estableciendo Operadores..." MinDelay="200" />
                                    </Select>
                                </DirectEvents>
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cmbOperador" FieldLabel="Operador" TabIndex="7" EmptyText="Todos"
                                Resizable="true" ListWidth="350" Width="300" runat="server" StoreID="StoreOperador"
                                DisplayField="NameFin" ValueField="ClaveColectiva">
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cmbEstatus" FieldLabel="Estatus" StoreID="StoreEstatus" TabIndex="8"
                                EmptyText="Todos" Resizable="true" ListWidth="350" Width="300" runat="server"
                                DisplayField="Descripcion" ValueField="ID_EstatusOperacion">
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                              <ext:ComboBox ID="cmbBeneficiario" FieldLabel="Marca" TabIndex="9" EmptyText="Todos"
                                Resizable="true" ListWidth="350" Width="300" runat="server" StoreID="StoreBeneficiario"
                                DisplayField="NombreORazonSocial" ValueField="ClaveColectiva">
                                <Items>
                                    <ext:ListItem Text="( Todos )" Value="-1" />
                                </Items>
                            </ext:ComboBox>
                            <ext:TextField ID="txtTelefono" FieldLabel="Referencia" EmptyText="Todos" TabIndex="10"
                                MaxLength="50" Width="300" runat="server" Text="" />
                        </Items>
                    </ext:Panel>
                </Items>
                <FooterBar>
                    <ext:Toolbar ID="Toolbar1" runat="server">
                        <Items>
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click" />
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnBuscarHide" runat="server" Hidden="true">
                                <Listeners>
                                    <Click Handler="Ext.net.Mask.show({ msg : 'Obteniendo Operaciones...' });
                                        #{GridOpsConSaldo}.getStore().reload({params:{start:0, sort:('','')}});" />
                                </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar..." Icon="Magnifier">
                                <DirectEvents>
                                    <Click OnEvent="btnBuscar_Click" Before="var valid= #{FormOpsConSaldo}.getForm().isValid(); if (!valid) {} return valid;">
                                        <EventMask ShowMask="true" Msg="Obteniendo Operaciones..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnDownloadHide" runat="server" Hidden="true">
                                <DirectEvents>
                                    <Click OnEvent="Download" IsUpload="true"
                                        After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                            RepOpsSaldo.StopMask();" />
                                </DirectEvents>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </FooterBar>
            </ext:FormPanel>
        </West>
        <Center Split="true" Collapsible="false">
            <ext:Panel runat="server" Title="Operaciones obtenidas con los filtros seleccionados" Layout="FitLayout"
                AutoScroll="true" Border="false">
                <Items>
                    <ext:GridPanel ID="GridOpsConSaldo" runat="server" StripeRows="true" Header="false" Border="false"
                        AutoExpandColumn="Referencia">
                        <LoadMask ShowMask="false" />
                        <Store>
                            <ext:Store ID="StoreOpsConSaldo" runat="server" OnRefreshData="StoreOpsConSaldo_RefreshData"
                                AutoLoad="false" RemoteSort="true">
                                <AutoLoadParams>
                                    <ext:Parameter Name="start" Value="0" Mode="Raw" />
                                </AutoLoadParams>
                                <Proxy>
                                    <ext:PageProxy />
                                </Proxy>                                
                                <DirectEventConfig IsUpload="true"/>
                                <Reader>
                                    <ext:JsonReader IDProperty="ID_Operacion">
                                        <Fields>
                                            <ext:RecordField Name="Fecha" Type="Date" />
                                            <ext:RecordField Name="ID_Operacion" />
                                            <ext:RecordField Name="ClaveCadena" />
                                            <ext:RecordField Name="Sucursal" />
                                            <ext:RecordField Name="Afiliacion" />
                                            <ext:RecordField Name="Terminal" />
                                            <ext:RecordField Name="Operador" />
                                            <ext:RecordField Name="Estatus" />
                                            <ext:RecordField Name="CodRespuesta" />
                                            <ext:RecordField Name="Autorizacion" />
                                            <ext:RecordField Name="Beneficiario" />
                                            <ext:RecordField Name="Referencia" />
                                            <ext:RecordField Name="Banco" />
                                            <ext:RecordField Name="GrupoMA" />
                                            <ext:RecordField Name="Monto" />
                                            <ext:RecordField Name="Mensualidades" />
                                            <ext:RecordField Name="NombreOperacion" />
                                            <ext:RecordField Name="ID_Cliente" />
                                            <ext:RecordField Name="TarjetaHabiente" />
                                            <ext:RecordField Name="DescripcionPromo" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                                <DirectEventConfig IsUpload="true" />
                                <SortInfo Field="Fecha" />
                            </ext:Store>
                        </Store>
                        <ColumnModel ID="ColumnModel1" runat="server">
                            <Columns>
                                <ext:DateColumn ColumnID="colFecha" Header="Fecha" Sortable="true" DataIndex="Fecha"
                                    Format="yyyy-MM-dd" />
                                <ext:DateColumn ColumnID="colhora" Header="Hora" Sortable="true" DataIndex="Fecha"
                                    Format="HH:mm:ss" />
                                <ext:Column ColumnID="ID_Operacion" Header="Identificador" Sortable="true" DataIndex="ID_Operacion" />
                                <ext:Column ColumnID="ClaveCadena" Header="Cadena Comercial" Sortable="true" DataIndex="ClaveCadena" />
                                <ext:Column ColumnID="Sucursal" Header="Sucursal" Sortable="true" DataIndex="Sucursal" />
                                <ext:Column ColumnID="Afiliacion" Header="Afiliacion" Sortable="true" DataIndex="Afiliacion" />
                                <ext:Column ColumnID="Terminal" Header="Terminal" Sortable="true" DataIndex="Terminal" />
                                <ext:Column ColumnID="Operador" Header="Operador" Sortable="true" DataIndex="Operador" />
                                <ext:Column ColumnID="Estatus" Header="Estatus" Sortable="true" DataIndex="Estatus" />
                                <ext:Column ColumnID="Beneficiario" Header="Marca" Sortable="true" DataIndex="Beneficiario" />
                                <ext:Column ColumnID="CodRespuesta" Header="Codigo Respuesta" Sortable="true" DataIndex="CodRespuesta" />
                                <ext:Column ColumnID="Autorizacion" Header="Autorización" Sortable="true" DataIndex="Autorizacion" />
                                <ext:Column ColumnID="Referencia" Header="Referencia" Sortable="true" DataIndex="Referencia" />
                                <ext:Column ColumnID="Banco" Header="Banco" Sortable="true" DataIndex="Banco" />
                                <ext:Column ColumnID="GrupoMA" Header="Grupo MA" Sortable="true" DataIndex="GrupoMA" />
                                <ext:Column ColumnID="Monto" Header="Monto" Sortable="true" DataIndex="Monto">
                                    <Renderer Format="UsMoney" />
                                </ext:Column>
                                <ext:Column ColumnID="Mensualidades" Header="Mensualidades" Sortable="true" DataIndex="Mensualidades" />
                                <ext:Column ColumnID="DescripcionPromo" Header="Promoción" Sortable="true" DataIndex="DescripcionPromo" />
                                <ext:Column ColumnID="NombreOperacion" Header="Operacion" Sortable="true" DataIndex="NombreOperacion" />
                                <ext:Column ColumnID="ID_Cliente" Header="ID_Cliente" Sortable="true" DataIndex="ID_Cliente" />
                                <ext:Column ColumnID="TarjetaHabiente" Header="TarjetaHabiente" Sortable="true" DataIndex="TarjetaHabiente" />
                            </Columns>
                        </ColumnModel>
                        <Plugins>
                            <ext:GridFilters runat="server" Local="true" FiltersText="Filtros">
                                <Filters>
                                    <ext:StringFilter DataIndex="NombreOperacion" />
                                    <ext:StringFilter DataIndex="ID_Cliente" />
                                    <ext:StringFilter DataIndex="TarjetaHabiente" />
                                    <ext:NumericFilter DataIndex="ID_Operacion" />
                                    <ext:StringFilter DataIndex="Sucursal" />
                                    <ext:StringFilter DataIndex="Afiliacion" />
                                    <ext:StringFilter DataIndex="Terminal" />
                                    <ext:StringFilter DataIndex="Operador" />
                                    <ext:StringFilter DataIndex="Autorizacion" />
                                    <ext:StringFilter DataIndex="Beneficiario" />
                                    <ext:StringFilter DataIndex="Telefono" />
                                    <ext:NumericFilter DataIndex="Monto" />
                                    <ext:DateFilter DataIndex="Fecha">
                                        <DatePickerOptions runat="server" TodayText="Hoy" />
                                    </ext:DateFilter>
                                </Filters>
                            </ext:GridFilters>
                        </Plugins>
                        <BottomBar>
                            <ext:PagingToolbar ID="PagingOpsConSaldo" runat="server" StoreID="StoreOpsConSaldo" DisplayInfo="true"
                                DisplayMsg="Mostrando Operaciones {0} - {1} de {2}" />
                        </BottomBar>
                        <TopBar>
                            <ext:Toolbar ID="Toolbar2" runat="server">
                                <Items>
                                    <ext:ToolbarFill ID="ToolbarFill1" runat="server" />
                                    <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" Icon="PageExcel" Disabled="true">
                                        <DirectEvents>
                                            <Click OnEvent="Download" IsUpload="true"
                                                After="Ext.net.Mask.show({ msg : 'Exportando Reporte a Excel...' });
                                                    e.stopEvent(); 
                                                    RepOpsSaldo.StopMask();" />
                                        </DirectEvents>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                    </ext:GridPanel>
                </Items>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
