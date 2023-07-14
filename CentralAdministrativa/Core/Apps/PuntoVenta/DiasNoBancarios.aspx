<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DiasNoBancarios.aspx.cs" Inherits="TpvWeb.DiasNoBancarios" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>

<ext:GroupStore ID="GroupStore1" runat="server">
    <Groups>
        <ext:Group CalendarId="4" Title="Día No Bancario" />
    </Groups>
</ext:GroupStore>

<ext:EventEditWindow ID="EventEditWindow1" runat="server" Hidden="true" GroupStoreID="GroupStore1">
    <Listeners>
        <%--<BeforeRender Handler="this.fbar.items.get(0).hide();
            this.fbar.items.get(2).text = 'Guardar'; this.fbar.items.get(3).text = 'Eliminar';
            this.fbar.items.get(4).text = 'Cancelar'; this.items.get(0).items.get(0).hide();
            this.items.get(0).items.get(1).hide(); this.items.get(0).items.get(2).fieldLabel = 'Marcar como';
            this.items.get(0).add({id: 'txtFecha', fieldLabel: 'Fecha', xtype:'textfield', dataIndex: 'Fecha', width: 490, readOnly: true});" />--%>
        <BeforeRender Handler="this.fbar.items.get(0).hide();
            this.fbar.items.get(2).text = 'Guardar';
            this.fbar.items.get(3).text = 'Eliminar';
            this.fbar.items.get(4).text = 'Cancelar';
            this.items.get(0).items.get(0).hide();
            this.items.get(0).items.get(1).hide();
            this.items.get(0).items.get(2).readOnly = true;
            this.items.get(0).items.get(2).fieldLabel = 'Etiqueta';" />
        <EventAdd Handler="#{EventEditWindow1}.hide(); DNB.NuevoDiaNoBancario(record.data.StartDate);" />
        <EventUpdate Handler="#{EventEditWindow1}.hide(); record.commit();" />
        <EventDelete Handler="if (record.data.StartDate > new Date()) {
            Ext.MessageBox.show({
                icon: Ext.MessageBox.WARNING,
                title: 'Confirmación',
                msg: '¿Estás seguro de eliminar la fecha ' + record.data.StartDate.ddmmyyyy() + ' como día NO bancario?',
                buttons: Ext.MessageBox.YESNO,
                fn: function (btn) {
                    if (btn == 'yes') {
                        DNB.EliminaDiaNoBancario(record.data.StartDate);
                        #{EventEditWindow1}.hide();
                        #{EventStore1}.remove(record);
                    }
                }
            }); }
            else {
                Ext.MessageBox.show({
                    icon: Ext.MessageBox.INFO,
                    msg: 'No es posible eliminar fechas anteriores al día de hoy',
                    buttons: Ext.MessageBox.OK
                }); }" />
    </Listeners>
</ext:EventEditWindow>

<ext:Hidden ID="hdnMeses" runat="server" />
<ext:Hidden ID="hdnSoloConsulta" runat="server" />

<ext:Window ID="WdwCalendarioLista" runat="server" Title="Lista Anual - Días No Bancarios" Width="400" Height="330" 
    Hidden="true" Modal="true" Resizable="false" Icon="PageEdit">
    <Items>
        <ext:GridPanel ID="GridPanelAnual" runat="server" Border="false" Header="false" AutoScroll="true"
            Layout="FitLayout" Height="270" >
            <Store>
                <ext:Store ID="StoreCalendario" runat="server" GroupField="Anyo">
                    <Reader>
                        <ext:JsonReader IDProperty="ID_Fecha">
                            <Fields>
                                <ext:RecordField Name="ID_Fecha" />
                                <ext:RecordField Name="Fecha" />
                                <ext:RecordField Name="DiaMes" />
                                <ext:RecordField Name="Anyo" />
                            </Fields>
                        </ext:JsonReader>
                    </Reader>
                </ext:Store>
            </Store>
            <ColumnModel runat="server">
                <Columns>
                    <ext:Column DataIndex="ID_Fecha" Hidden="true" />
                    <ext:ImageCommandColumn Width="10" Header="Año" />
                    <ext:GroupingSummaryColumn Header="Fecha" DataIndex="Fecha" Hidden="true" />
                    <ext:GroupingSummaryColumn DataIndex="Anyo" Hidden="true" />
                    <ext:GroupingSummaryColumn Header="Fecha" DataIndex="DiaMes" Width="50" />
                </Columns>
            </ColumnModel>
            <SelectionModel>
                <ext:RowSelectionModel SingleSelect="true" />
            </SelectionModel>
            <BottomBar>
                <ext:PagingToolbar ID="PagingCalendario" runat="server" StoreID="StoreCalendario" DisplayInfo="true"
                    DisplayMsg="Mostrando Fechas {0} - {1} de {2}" HideRefresh="true" PageSize="8" />
            </BottomBar>
            <View>
                <ext:GroupingView ID="GroupingView1" runat="server" ForceFit="true" MarkDirty="false"
                    ShowGroupName="false" EnableNoGroups="true" HideGroupedColumn="true" />
            </View>
        </ext:GridPanel>
    </Items>
    <BottomBar>
        <ext:Toolbar runat="server">
            <Items>
                <ext:ToolbarFill runat="server" />
                <ext:Button ID="btnOK" runat="server" Text="Aceptar" Icon="Tick">
                    <Listeners>
                        <Click Handler="#{WdwCalendarioLista}.hide();" />
                    </Listeners>
                </ext:Button>
            </Items>
        </ext:Toolbar>
    </BottomBar>
</ext:Window>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html lang="en" xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <title></title>
    <ext:ResourcePlaceHolder runat="server" Mode="Style" />
    <link rel="stylesheet" type="text/css" href="../Styles/Calendar.css" />
    <style type="text/css">
        .ext-color-4,
        .ext-ie .ext-color-4-ad,
        .ext-opera .ext-color-4-ad {
	        color: #7F0000;
        }
        .ext-cal-day-col .ext-color-4,
        .ext-dd-drag-proxy .ext-color-4,
        .ext-color-4-ad,
        .ext-color-4-ad .ext-cal-evm,
        .ext-color-4 .ext-cal-picker-icon,
        .ext-color-4-x dl,
        .ext-color-4-x .ext-cal-evb {
	        background: #7F0000;
        }
        .ext-color-4-x .ext-cal-evb,
        .ext-color-4-x dl {
            border-color: #7C3939;
        }
        .bigexcel {
            background-image: url(Images/Excel.gif) !important;
        }
        .biglist {
            background-image: url(Images/lista.gif) !important;
        }
    </style>
    <script type="text/javascript">
        var validaDiaEvento = function (clickDate, store, readFlag) {
            if (clickDate.getDay() > 0 && clickDate.getDay() < 6) { //No es sábado o domingo
                if (clickDate <= new Date()) {
                    return false; //La fecha seleccionada es pasada
                } else {
                    var diaNB = false;
                    store.each(function (record) {
                        if (record.get('StartDate').getTime() == clickDate.getTime()) {
                            diaNB = true;
                        }
                    });
                    if (diaNB) {
                        return false; //El día seleccionado ya está marcado como día no bancario
                    } else if (readFlag == 1) { //Si tiene permisos de sólo consulta
                        return false;
                    } else {
                        return true;
                    }
                }
            } else {
                return false; } 
        }

        Date.prototype.ddmmyyyy = function () {
            var mm = this.getMonth() + 1; // getMonth() is zero-based
            var dd = this.getDate();

            return [
                (dd > 9 ? '' : '0') + dd + '/',
                (mm > 9 ? '' : '0') + mm + '/',
                this.getFullYear()           
            ].join('');
        };

        var _Calendario = {
            getCalendar: function () {
                return _Calendario.CalendarPanel1;
            },

            getStore: function () {
                return _Calendario.EventStore1;
            },

            getWindow: function () {
                return _Calendario.EventEditWindow1;
            },

            viewChange: function (p, vw, dateInfo) {
                var win = this.getWindow();

                if (win) {
                    win.hide();
                }

                if (dateInfo !== null) {
                    // will be null when switching to the event edit form, so ignore
                    this.DatePicker1.setValue(dateInfo.activeDate);
                    this.updateTitle(dateInfo.viewStart, dateInfo.viewEnd);
                }
            },

            updateTitle: function (startDt, endDt) {
                var msg = '';

                if (startDt.clearTime().getTime() == endDt.clearTime().getTime()) {
                    msg = startDt.format('F j, Y');
                } else if (startDt.getFullYear() == endDt.getFullYear()) {
                    if (startDt.getMonth() == endDt.getMonth()) {
                        msg = startDt.format('F j') + ' - ' + endDt.format('j, Y');
                    } else {
                        msg = startDt.format('F j') + ' - ' + endDt.format('F j, Y');
                    }
                } else {
                    msg = startDt.format('F j, Y') + ' - ' + endDt.format('F j, Y');
                }

                this.Panel1.setTitle(msg);
            },

            setStartDate: function (picker, date) {
                this.getCalendar().setStartDate(date);
            },

            rangeSelect: function (cal, dates, callback) {
                this.record.show(cal, dates);
                this.getWindow().on('hide', callback, cal, { single: true });
            },
        };
    </script>
</head>
<body>
    <form runat="server">
        <ext:ResourceManager runat="server" IDMode="Explicit" InitScriptMode="Linked" Locale="es-MX"
            RemoveViewState="true" Namespace="_Calendario"/>
        
        <ext:Viewport runat="server" Layout="Border">
            <Items>
                <ext:BorderLayout ID="BorderLayout1" runat="server">
                    <North>
                        <ext:Panel ID="Panel1" runat="server" Title="..." Height="20" Border="false" />
                    </North>
                    <Center>
                        <ext:Panel ID="Panel2" runat="server" Layout="Border">
                            <Items>
                                <ext:DatePicker ID="DatePicker1" runat="server" Cls="ext-cal-nav-picker">
                                    <Listeners>
                                        <Select Fn="_Calendario.setStartDate" Scope="_Calendario" />
                                    </Listeners>
                                </ext:DatePicker>
                            </Items>
                            <TopBar>
                                <ext:Toolbar runat="server">
                                    <Items>
                                        <ext:ComboBox ID="cBoxPais" runat="server" FieldLabel="País" LabelWidth="30"
                                            DisplayField="Descripcion" ValueField="ID_Pais" AllowBlank="false"
                                            Width="170">
                                            <Store>
                                                <ext:Store ID="StorePaises" runat="server">
                                                    <Reader>
                                                        <ext:JsonReader IDProperty="ID_Pais">
                                                            <Fields>
                                                                <ext:RecordField Name="ID_Pais" />
                                                                <ext:RecordField Name="Clave" />
                                                                <ext:RecordField Name="Descripcion" />
                                                            </Fields>
                                                        </ext:JsonReader>
                                                    </Reader>
                                                </ext:Store>
                                            </Store>
                                            <DirectEvents>
                                                <Select OnEvent="SeleccionaPais">
                                                    <EventMask ShowMask="true" Msg="Estableciendo Calendario..." MinDelay="200" />
                                                </Select>
                                            </DirectEvents>
                                        </ext:ComboBox>
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                        </ext:Panel>
                    </Center>
                    <East>
                        <ext:CalendarPanel ID="CalendarPanel1" runat="server" ActiveIndex="1" Border="false" Width="950"  ShowDayView="false"
                            ShowWeekView="false" MonthText="Mes" ShowTodayText="false" ShowTime="false">
                            <EventStore ID = "EventStore1" runat="server" />
                            <MonthView runat="server" ShowHeader="true" ShowWeekNumbers="true" />
                            <Listeners>
                                <ViewChange Fn="_Calendario.viewChange" Scope="_Calendario" />
                                <DayClick Handler="
                                    if (validaDiaEvento(dt, #{EventStore1}, #{hdnSoloConsulta}.getValue())) {
                                        #{EventEditWindow1}.titleTextAdd = 'Nuevo día no bancario - ' + dt.ddmmyyyy();
                                        #{EventEditWindow1}.fbar.items.get(2).show();
                                        #{EventEditWindow1}.show({StartDate: dt, IsAllDay: allDay}, this);
                                    } else {
                                        return false; }" />
                                <EventClick Handler="
                                    if (#{hdnSoloConsulta}.getValue() != 1) {
                                        #{EventEditWindow1}.titleTextEdit = 'Eliminar día no bancario - ' 
                                        + record.data.StartDate.ddmmyyyy(); #{EventEditWindow1}.fbar.items.get(2).hide();
                                        #{EventEditWindow1}.show(record, this);
                                    } else {
                                        return false; }" />
                            </Listeners>
                            <BottomBar>
                                <ext:Toolbar runat="server">
                                    <Items>
                                        <ext:ToolbarFill runat="server" />
                                        <ext:Button ID="btnListaAnual" runat="server" Text="Ver Lista Anual" Icon="PageEdit"
                                            Disabled="true">
                                            <DirectEvents>
                                                <Click OnEvent="VerListaAnual" IsUpload="true" />
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:ToolbarSeparator runat="server" />
                                        <ext:Button ID="btnExportExcel" runat="server" Text="Exportar a Excel" Icon="PageExcel"
                                            Disabled="true">
                                            <DirectEvents>
                                                <Click OnEvent="ExportarExcel" IsUpload="true" />
                                            </DirectEvents>
                                        </ext:Button>
                                    </Items>
                                </ext:Toolbar>
                            </BottomBar>
                        </ext:CalendarPanel>
                    </East>
                </ext:BorderLayout>
            </Items>
        </ext:Viewport>
    </form>
</body>
</html>
 