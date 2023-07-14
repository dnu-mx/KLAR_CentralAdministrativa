<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LaGrafica.aspx.cs" Inherits="Facturas.LaGrafica" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <script type="text/javascript">
         var onBeforeRender = function (panel) {
             panel.add({
                 xtype: 'linechart',
                 store: Store1,
                 url: 'charts.swf',
                 xField: 'FechaEmision',
                 yField: 'ImporteTotal',
                 yAxis: new Ext.chart.NumericAxis({
                     displayName: 'Fecha',
                     labelRenderer: Ext.util.Format.numberRenderer('0,0')
                 }),
                 tipRenderer: function (chart, record) {
                     return Ext.util.Format.number(record.data.ImporteTotal, '$0,0.00') + ' facturado el ' + record.data.FechaEmision + ' (' + record.data.NoFacturas + ' Facturas)';
                 }
             });
         };
    </script>
</head>
<body>
    <form id="Form1" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server" />
        <ext:Store ID="Store1" runat="server">
            <Reader>
                <ext:JsonReader>
                    <Fields>
                        <ext:RecordField Name="FechaEmision" />
                        <ext:RecordField Name="ImporteTotal" Type="Float" />
                        <ext:RecordField Name="NoFacturas" Type="Int" />
                    </Fields>
                </ext:JsonReader>
            </Reader>
        </ext:Store>
 
        <ext:Panel
            ID="Panel1"
            runat="server"
            Width="500"
             Border="false"
            Height="250">
             <AutoLoad ShowMask="true" 
                MaskMsg="Generando Gráfica, por favor espere..." ></AutoLoad>
            <Listeners>
                <BeforeRender Fn="onBeforeRender" />
            </Listeners>   
        </ext:Panel>
    </form>
</body>
</html>
