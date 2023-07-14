<%@ Page Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="ValidacionMembresias_Sams.aspx.cs" Inherits="CentroContacto.ValidacionMembresias_Sams" %>


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
        <Center Split="true">
            <ext:Panel ID="Panel1" runat="server" Width="350" Collapsible="false">
                <Content>
                    <ext:BorderLayout ID="LeftBorderLayout" runat="server">
                        <West Split="true">
                            <ext:FormPanel ID="FormPanelBusqueda" runat="server" Title="Validación de Membresia" Height="280" Frame="true" LabelWidth="120" Collapsible="false" Width="342">
                                <Items>
                                    <ext:FieldSet ID="FieldSetBusqueda" runat="server" Title="Información">
                                        <Items>
                                            <ext:TextField ID="txtMembresia" runat="server" LabelAlign="Right" FieldLabel="Membresia" MaxLength="30" Width="300" />
                                        </Items>
                                        <Buttons>
                                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar" Icon="ArrowRefresh">
                                                <DirectEvents>
                                                    <Click OnEvent="btnLimpiar_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="btnValidar" runat="server" Text="Validar" Icon="Magnifier">
                                                <DirectEvents>
                                                    <Click OnEvent="btnValidar_Click" Before="var valid= #{FormPanelBusqueda}.getForm().isValid(); if (!valid) {} return valid;">
                                                        <EventMask ShowMask="true" Msg="Validando Membresia..." MinDelay="500" />
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                        </Buttons>
                                    </ext:FieldSet>
                                </Items>
                            </ext:FormPanel>
                        </West>
                        <Center Split="true">
                            <ext:FormPanel ID="FormPanel1" runat="server" Title="Resultado de la Validación" LabelAlign="Left" LabelWidth="200">
                                <Items>
                                    <ext:FieldSet ID="FieldSet1" runat="server" Title="Información">
                                        <Items>
                                            <ext:TextField ID="txtCodigoRespuesta" runat="server" FieldLabel="Código Respuesta" Width="500" 
                                                ReadOnly="true" Enabled="false"/>
                                            <ext:TextField ID="txtDescripcion" runat="server" FieldLabel="Descripción" 
                                                Width="500" ReadOnly="true" Enabled="false"/>
                                            <ext:TextField ID="txtNivel" runat="server" FieldLabel="Nivel"
                                                Width="500" ReadOnly="true" Enabled="false"/>
                                            <ext:DateField ID="dtFechaVencimiento" runat="server" FieldLabel="Fecha de Vencimiento"
                                                Format="dd/MM/yyyy" Width="500" ReadOnly="true" Enabled="false"/>
                                            <ext:TextField ID="txtAsociado" runat="server" FieldLabel="Es Asociado"
                                                Width="500" ReadOnly="true" Enabled="false"/>
                                        </Items>
                                    </ext:FieldSet>
                                </Items>
                            </ext:FormPanel>
                        </Center>
                    </ext:BorderLayout>
                </Content>
            </ext:Panel>
        </Center>
        
    </ext:BorderLayout>
</asp:Content>
