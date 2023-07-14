<%@ Page Title="" MasterPageFile="~/Site.Master" AutoEventWireup="true" Language="C#" 
    CodeBehind="CargaDescargaFTP.aspx.cs" Inherits="Lealtad.CargaDescargaFTP" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
      var Confirm = function (grid1, grid2) {
            var title = 'Subir Archivos';
            var msg = 'Se subirán a la carpeta FTP ' + grid1.getSelectionModel().selections.length + ' Archivo(s): </br> ¿Es correcto?';

            if (grid1.getSelectionModel().selections.length == 0) {
               Ext.Msg.alert('Origen', 'Por favor, selecciona al menos un Archivo para subir.');
                return false;
            }

            //if (grid2.getSelectionModel().selections.length == 0) {
            //    Ext.Msg.alert('Destino', 'Por favor, selecciona una carpeta destino.');
            //    return false;
            //}

            Ext.Msg.confirm(title, msg, function (btn) {
                if (btn == 'yes') {
                    Ext.net.Mask.show({ msg : 'Subiendo Archivos...' });
                    UploadFTP.CargarArchivos();
                    return true;
                } else {
                    return false;
                }
            });
        }
    </script>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <West Split="true">
            <ext:GridPanel ID="GridPanelOrigen" Width="500" runat="server" Border="false" Title="Origen"
                Layout="FitLayout">
                <Store>
                    <ext:Store ID="StoreOrigen" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="NombreArchivo">
                                <Fields>
                                    <ext:RecordField Name="NombreArchivo" />
                                    <ext:RecordField Name="Tamanyo" Type="Int" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <TopBar>
                    <ext:Toolbar runat="server">
                        <Items>
                            <ext:FileUploadField ID="FileUF_Archivos" runat="server" Width="250" ButtonText="Examinar..."
                                Icon="Magnifier" AllowBlank="false">
                                <Listeners>
                                    <Render Handler="this.fileInput.set({ multiple : 'multiple' });" />
                                </Listeners>
                            </ext:FileUploadField>
                            <ext:ToolbarFill runat="server" />
                            <ext:Button ID="btnObtieneArchivos" runat="server" Text="Obtener Archivos"
                                Icon="PageWhiteAdd">
                                <DirectEvents>
                                    <Click OnEvent="btnObtieneArchivos_Click" IsUpload="true" 
                                        Before="if (!#{FileUF_Archivos}.getValue()) { return false; }">
                                        <EventMask ShowMask="true" Msg="Obteniendo Archivos..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnSubir" runat="server" Icon="PageWhiteGet" Text="Subir Archivo(s)">
                               <Listeners>
                                    <Click Handler="if (!#{cBoxCarpeta}.getValue()) { 
                                        Ext.Msg.alert('Destino', 'Por favor, selecciona la carpeta destino.');
                                        return false; }
                                        else { return Confirm(#{GridPanelOrigen},#{GridPanelDestino}); }"/>
                                </Listeners>
                            </ext:Button>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <ColumnModel>
                    <Columns>
                        <ext:Column Hidden="true" DataIndex="Ruta" />
                        <ext:Column Header="Archivo" DataIndex="NombreArchivo" Width="350" />
                        <ext:Column Header="Tamaño" DataIndex="Tamanyo" Width="70" />
                    </Columns>
                </ColumnModel>
                <SelectionModel>
                    <ext:CheckboxSelectionModel runat="server" />
                </SelectionModel>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingLocalFiles" runat="server" StoreID="StoreOrigen" DisplayInfo="true"
                        DisplayMsg="Mostrando Archivos {0} - {1} de {2}" />
                </BottomBar>
            </ext:GridPanel>
        </West>
        <Center Split="true">
            <ext:GridPanel ID="GridPanelDestino" runat="server" Layout="FitLayout" Title="Destino">
                 <Store>
                    <ext:Store ID="StoreDestino" runat="server">
                        <Reader>
                            <ext:JsonReader IDProperty="NombreArchivo">
                                <Fields>
                                    <ext:RecordField Name="NombreArchivo" />
                                    <ext:RecordField Name="Tamanyo" Type="Int" />
                                    <ext:RecordField Name="Tipo" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                    </ext:Store>
                </Store>
                <TopBar>
                    <ext:Toolbar runat="server">
                        <Items>
                            <ext:Hidden ID="hdnSrvr" runat="server" />
                            <ext:Hidden ID="hdnUsr" runat="server" />
                            <ext:Hidden ID="hndPw" runat="server" />
                            <ext:Hidden ID="hdnSsl" runat="server" />
                            <ext:ComboBox ID="cBoxCarpeta" runat="server" EmptyText="Selecciona carpeta destino..."
                                Width="250" DisplayField="Server" ValueField="ID">
                                <Store>
                                    <ext:Store ID="StoreCarpetasFTP" runat="server">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID">
                                                <Fields>
                                                    <ext:RecordField Name="ID" />
                                                    <ext:RecordField Name="Server" />
                                                    <ext:RecordField Name="User" />
                                                    <ext:RecordField Name="Password" />
                                                    <ext:RecordField Name="SSL" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <Triggers>
                                    <ext:FieldTrigger Icon="Clear" Qtip="Borrar" />
                                </Triggers>
                                <Listeners>
                                    <TriggerClick Handler="this.removeByValue(this.getValue());this.clearValue();" />
                                    <Select Handler="Ext.net.Mask.show({ msg : 'Estableciendo Conexión...' });
                                        #{hdnSrvr}.setValue(record.get('Server'));
                                        #{hdnUsr}.setValue(record.get('User'));
                                        #{hndPw}.setValue(record.get('Password'));
                                        #{hdnSsl}.setValue(record.get('SSL'));
                                        FTP.EstableceConexionFTP(record.get('Server'),
                                        record.get('User'), record.get('Password'), record.get('SSL'));" />
                                </Listeners>
                            </ext:ComboBox>
                            <ext:ToolbarFill runat="server" />
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <ColumnModel>
                    <Columns>
                        <ext:Column Header="Nombre" DataIndex="NombreArchivo" Width="270" />
                        <ext:Column Header="Tamaño" DataIndex="Tamanyo" Width="70" />
                        <ext:Column Header="Tipo" DataIndex="Tipo" />
                    </Columns>
                </ColumnModel>
                <SelectionModel>
                    <ext:CheckboxSelectionModel runat="server" SingleSelect="true" />
                </SelectionModel>
                <BottomBar>
                    <ext:PagingToolbar ID="PagingDestino" runat="server" StoreID="StoreDestino" DisplayInfo="true"
                        DisplayMsg="Mostrando Carpetas {0} - {1} de {2}" />
                </BottomBar>
            </ext:GridPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>
