<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ImportarDatosExcel.aspx.cs" Inherits="TpvWeb.ImportarDatosExcel" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <ext:BorderLayout ID="BorderLayout1" runat="server">
        <Center Split="true">
            <ext:FormPanel ID="FormPanel1" Frame="true" runat="server" Border="false">
                <Items>
                    <ext:FieldSet ID="FielSetImportar" runat="server" Title="Archivo por Importar" Height="250"
                        DefaultWidth="650" LabelWidth="100" LabelAlign="Left" Layout="Form">
                        <Items>
                            <ext:FileUploadField ID="FileUploadField1" runat="server" ButtonText="Examinar..." Icon="Magnifier"/>
                        </Items>
                        <Buttons>
                            <ext:Button ID="btnImportar" runat="server" Text="Importar" Icon="DatabaseGo">
                                <DirectEvents>
                                    <Click OnEvent="btnImportar_Click" IsUpload="true">
                                        <EventMask ShowMask="true" Msg="Importando archivo..." MinDelay="500" />
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button ID="btnLimpiar" runat="server" Text="Limpiar">
                                <DirectEvents>
                                    <Click OnEvent="btnLimpiar_Click" />
                                </DirectEvents>
                            </ext:Button>
                        </Buttons>
                    </ext:FieldSet>
                </Items>
            </ext:FormPanel>
        </Center>
    </ext:BorderLayout>
</asp:Content>

