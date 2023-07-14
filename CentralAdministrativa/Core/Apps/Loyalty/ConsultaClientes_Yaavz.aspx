<%@ Page Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="ConsultaClientes_Yaavz.aspx.cs" Inherits="CentroContacto.ConsultaClientes_Yaavz" %>


<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var onKeyUpPed = function (field) {
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

        var onKeyUpMov = function (field) {
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
    <ext:BorderLayout ID="MainBorderLayout" runat="server">
        <West Split="true">
            <ext:Panel ID="Panel1" runat="server" Width="350" Collapsible="true">
                <Content>
                    <ext:BorderLayout ID="LeftBorderLayout" runat="server">
                        <North Split="true">
                            <ext:FormPanel ID="FormPanelBusqueda" runat="server" Title="Consulta de Clientes Convenios" Height="280" Frame="true" LabelWidth="120" Collapsible="true">
                                <Items>
                                    <ext:FieldSet ID="FieldSetBusqueda" runat="server" Title="Búsqueda">
                                        <Items>
                                            <ext:TextField ID="txtNombre" runat="server" LabelAlign="Right" FieldLabel="Nombre" MaxLength="30" Width="300" />
                                            <ext:TextField ID="txtApPaterno" runat="server" LabelAlign="Right" FieldLabel="Primer Apellido" MaxLength="30" Width="300" />
                                            <ext:TextField ID="txtApMaterno" runat="server" LabelAlign="Right" FieldLabel="Segundo Apellido" MaxLength="30" Width="300" />
                                            <ext:TextField ID="txtCorreo" runat="server" LabelAlign="Right" FieldLabel="Correo Electrónico" MaxLength="60" Width="300" />
                                            <ext:DateField ID="dfFechaNac" runat="server" LabelAlign="Right" FieldLabel="Fecha de Nacimiento" Width="300"
                                                Format="dd/MM/yyyy" Name="FechaNacimiento" Vtype="daterange" Editable="false" />
                                            <ext:TextField ID="txtMembresiaSearch" runat="server" LabelAlign="Right" FieldLabel="Membresia" MaxLength="150" Width="300" />
                                        </Items>
                                        <Buttons>
                                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                                <DirectEvents>
                                                    <Click OnEvent="btnLimpiar_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="btnBuscar" runat="server" Text="Buscar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnBuscar_Click" Before="var valid= #{FormPanelBusqueda}.getForm().isValid(); if (!valid) {} return valid;">
                                                        <EventMask ShowMask="true" Msg="Buscando Clientes..." MinDelay="500" />
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Buttons>
                                    </ext:FieldSet>
                                </Items>
                            </ext:FormPanel>
                        </North>
                        <Center Split="true">
                            <ext:FormPanel ID="FormPanelResultados" runat="server" Title="Resultados Clientes" Layout="FitLayout">
                                <Items>
                                    <ext:GridPanel ID="GridResultados" runat="server" AutoExpandColumn="Nombre" Height="600" AutoDoLayout="true">
                                        <Store>
                                            <ext:Store ID="StoreClientes" runat="server" OnRefreshData="StoreClientes_Refresh">
                                                <Reader>
                                                    <ext:JsonReader IDProperty="IdCliente">
                                                        <Fields>
                                                            <ext:RecordField Name="IdCliente" />
                                                            <ext:RecordField Name="IdColectiva" />
                                                            <ext:RecordField Name="IdTipoColectiva" />
                                                            <ext:RecordField Name="EstatusMA" />
                                                            <ext:RecordField Name="Email" />
                                                            <ext:RecordField Name="Nombre" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Store>
                                        <ColumnModel ID="ColumnModel1" runat="server">
                                            <Columns>
                                                <ext:Column DataIndex="IdCliente" Header="IdCliente" Width="60" />
                                                <ext:Column DataIndex="IdColectiva" Hidden="true" />
                                                <ext:Column DataIndex="IdTipoColectiva" Hidden="true" />
                                                <ext:Column DataIndex="EstatusMA" Hidden="true" />
                                                <ext:Column DataIndex="Email" Header="Correo" />
                                                <ext:Column DataIndex="Nombre" Header="Nombre" Width="100" />
                                            </Columns>
                                        </ColumnModel>
                                        <SelectionModel>
                                            <ext:RowSelectionModel SingleSelect="true" />
                                        </SelectionModel>
                                        <DirectEvents>
                                            <RowClick OnEvent="selectRowResultados_Event">
                                                <EventMask ShowMask="true" Msg="Obteniendo Información del Cliente..." MinDelay="500" />
                                                <ExtraParams>
                                                    <ext:Parameter Name="Values" Value="Ext.encode(#{GridResultados}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                                </ExtraParams>
                                            </RowClick>
                                        </DirectEvents>
                                        <BottomBar>
                                            <ext:PagingToolbar ID="PagingToolBar1" runat="server" StoreID="StoreClientes" DisplayInfo="true"
                                                DisplayMsg="Clientes {0} - {1} de {2}" />
                                        </BottomBar>
                                    </ext:GridPanel>
                                </Items>
                            </ext:FormPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </West>
        <Center Split="true">
            <ext:Panel ID="PanelCentral" runat="server" Height="250">
                <Items>
                    <ext:BorderLayout ID="BorderLayout1" runat="server">
                        <Center>
                            <ext:TabPanel ID="TabPanel1" runat="server">
                                <Items>
                                    <ext:FormPanel ID="FormPanelDatos" runat="server" Title="Datos" LabelAlign="Left" LabelWidth="200">
                                        <Items>
                                            <ext:FieldSet ID="FieldSetDatosCliente" runat="server" Title="Información del Cliente">
                                                <Items>
                                                    <ext:Hidden ID="hdID_Cliente" runat="server" />
                                                    <ext:Hidden ID="hdID_Colectiva" runat="server" />
                                                    <ext:Hidden ID="hdID_TipoColectiva" runat="server" />
                                                    <ext:Hidden ID="hdeMail" runat="server" />
                                                    <ext:Hidden ID="hdIdEstatusMA" runat="server" />
                                                    <ext:TextField ID="txtNombreCliente" runat="server" FieldLabel="Nombre" Width="500" 
                                                        ReadOnly="true" Enabled="false"/>
                                                    <ext:TextField ID="txtApPaternoCliente" runat="server" FieldLabel="Primer Apellido" 
                                                        Width="500" ReadOnly="true" Enabled="false"/>
                                                    <ext:TextField ID="txtApMaternoCliente" runat="server" FieldLabel="Segundo Apellido"
                                                        Width="500" ReadOnly="true" Enabled="false"/>
                                                    <ext:TextField ID="txtEmailCliente" runat="server" FieldLabel="Correo Electrónico" Width="500"
                                                        ReadOnly="true" Enabled="false" />
                                                    <ext:DateField ID="dfFechaNacCliente" runat="server" FieldLabel="Fecha de Nacimiento"
                                                        Format="dd/MM/yyyy" Width="500" ReadOnly="true" Enabled="false"/>
                                                    <ext:DateField ID="dfFechaAlta" runat="server" FieldLabel="Fecha de alta" Format="dd/MM/yyyy"
                                                        Width="500" ReadOnly="true" Enabled="false" />
                                                    <ext:DateField ID="dfFechaAltaLealtad" runat="server" FieldLabel="Fecha de Confirmación"
                                                        Format="dd/MM/yyyy" Width="500" ReadOnly="true" Enabled="false" />
                                                    <ext:ComboBox ID="txtEmpresa" runat="server" FieldLabel="Empresa"  
                                                        Width="500" ReadOnly="true" Enabled="false" />

                                                    <ext:TextField ID="txtMembresiaDatos" runat="server" FieldLabel="Membresia"
                                                        Width="500" ReadOnly="true" Enabled="false"/>
                                                    <ext:TextField ID="txtNivel" runat="server" FieldLabel="Nivel"
                                                        Width="500" ReadOnly="true" Enabled="false"/>
                                                    <ext:DateField ID="dfFechaRenovacion" runat="server" FieldLabel="Fecha Renovación"
                                                        Format="dd/MM/yyyy"  Width="500" ReadOnly="true" Enabled="false"/>

                                                </Items>
                                            </ext:FieldSet>
                                        </Items>
                                    </ext:FormPanel>

                                    <ext:FormPanel ID="FormPanelAjustesManuales" runat="server" Title="Ajustes Manuales" LabelAlign="Left" LabelWidth="120" AutoScroll="true">
                                        <Items>
                                            <ext:FieldSet ID="FieldSetEstatus" runat="server" Title="Confirmación Manual de Correo" Layout="AnchorLayout"
                                                DefaultAnchor="100%" Collapsible="true">
                                                <Content>
                                                    <ext:Store ID="StoreEstatusConfirma" runat="server" AutoLoad="true">
                                                        <Reader>
                                                            <ext:ArrayReader>
                                                                <Fields>
                                                                    <ext:RecordField Name="ClaveEstatus" />
                                                                    <ext:RecordField Name="NombreEstatus" />
                                                                </Fields>
                                                            </ext:ArrayReader>
                                                        </Reader>
                                                    </ext:Store>
                                                </Content>
                                                <Items>
                                                    <ext:ComboBox ID="cboEstatusConfirma" runat="server" FieldLabel="Estatus Actual" Width="500"
                                                        ListWidth="350" StoreID="StoreEstatusConfirma" DisplayField="NombreEstatus"
                                                        ValueField="ClaveEstatus" />
                                                </Items>
                                                <Buttons>
                                                    <ext:Button ID="btnGuardarEstatus" runat="server" Text="Guardar" Icon="Disk">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnGuardarEstatus_Click" />
                                                        </DirectEvents>
                                                    </ext:Button>
                                                </Buttons>
                                            </ext:FieldSet>

                                            <ext:FieldSet ID="FieldSetCambioContrasena" runat="server" Title="Cambio Manual de Contraseña" Layout="AnchorLayout"
                                                DefaultAnchor="100%" Collapsible="true">
                                                <Items>
                                                    <ext:TextField ID="txt_Contrasenia" runat="server" FieldLabel="Nueva Contraseña Cliente" ReadOnly="true" Enabled="false" ></ext:TextField>
                                                </Items>
                                                <Buttons>
                                                    <ext:Button ID="btnGenerarContrasena" runat="server" Text="Generar Contraseña" Icon="Disk">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnGenerarContrasena_Click" />
                                                        </DirectEvents>
                                                    </ext:Button>
                                                </Buttons>
                                            </ext:FieldSet>

                                            <ext:FieldSet ID="FieldSetBloqueoContracargo" runat="server" Title="Bloqueo/Desbloqueo por Contracargo" Layout="AnchorLayout"
                                                DefaultAnchor="100%" Collapsible="true">
                                                <Items>
                                                    <ext:RadioGroup ID="RadioGroupEstatus" runat="server" GroupName="RadioGroupEstatus" FieldLabel="Seleccione Estatus"
                                                        Cls="x-check-group-alt">
                                                        <Items>
                                                            <ext:Radio ID="rdBloqueado" runat="server" BoxLabel="Bloqueado" InputValue="bloqueado" />
                                                            <ext:Radio ID="rdDesbloqueado" runat="server" BoxLabel="Desbloqueado" InputValue="desbloqueado" />
                                                        </Items>
                                                    </ext:RadioGroup>

                                                    <ext:DateField ID="dfFechaBloqueo" runat="server" FieldLabel="Último Bloqueo"
                                                        Format="dd/MM/yyyy" Width="350" ReadOnly="true" Enabled="false"/>

                                                    <ext:TextArea ID="txtComentario" runat="server" FieldLabel="Comentario" MaxLength="500"
                                                        MaxLengthText="350" AllowBlank="false" />
                                                </Items>
                                                <Buttons>
                                                    <ext:Button ID="btnAplicar" runat="server" Text="Aplicar" Icon="Disk">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnAplicarBloqueo_Click" />
                                                        </DirectEvents>
                                                    </ext:Button>
                                                </Buttons>
                                            </ext:FieldSet>

                                            <ext:FieldSet ID="FieldSetCambioMembresia" runat="server" Title="Cambio de Membresia" Layout="AnchorLayout"
                                                DefaultAnchor="100%" Collapsible="true">
                                                <Items>
                                                    <ext:TextField ID="txtMembresia" runat="server" FieldLabel="Membresia" ReadOnly="true" Enabled="false" ></ext:TextField>
                                                    <ext:TextField ID="txtMembresiaNew" runat="server" FieldLabel="Membresia Nueva" MaxLength="150"></ext:TextField>
                                                </Items>
                                                <Buttons>
                                                    <ext:Button ID="btnMembresia" runat="server" Text="Cambiar" Icon="Disk">
                                                        <DirectEvents>
                                                            <Click OnEvent="btnCambiarMembresia_Click" />
                                                        </DirectEvents>
                                                    </ext:Button>
                                                </Buttons>
                                            </ext:FieldSet>
                                        </Items>
                                    </ext:FormPanel>
                                </Items>
                            </ext:TabPanel>
                        </Center>
                    </ext:BorderLayout>
                </Items>
            </ext:Panel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
