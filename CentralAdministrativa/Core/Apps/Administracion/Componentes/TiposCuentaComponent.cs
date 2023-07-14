using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Administracion.Componentes
{
    public class TiposCuentaComponent
    {
        public const string COMMAND_NAME = "EditTipoCuenta";

        public static GridPanel CrearGridPanelBase(GridPanel panel, bool editable)
        {
            GroupingSummaryColumn ID_TipoCuenta = new GroupingSummaryColumn();
            ID_TipoCuenta.DataIndex = "ID_TipoCuenta";
            ID_TipoCuenta.Hidden = true;
            panel.ColumnModel.Columns.Add(ID_TipoCuenta);

            GroupingSummaryColumn CodTipoCuentaISO = new GroupingSummaryColumn();
            CodTipoCuentaISO.DataIndex = "CodTipoCuentaISO";
            CodTipoCuentaISO.Header = "Código ISO";
            CodTipoCuentaISO.Width = 80;
            panel.ColumnModel.Columns.Add(CodTipoCuentaISO);

            GroupingSummaryColumn ClaveTipoCuenta = new GroupingSummaryColumn();
            ClaveTipoCuenta.DataIndex = "ClaveTipoCuenta";
            ClaveTipoCuenta.Header = "Clave";
            ClaveTipoCuenta.Width = 80;
            panel.ColumnModel.Columns.Add(ClaveTipoCuenta);

            GroupingSummaryColumn Descripcion = new GroupingSummaryColumn();
            Descripcion.DataIndex = "Descripcion";
            Descripcion.Header = "Descripción";
            panel.ColumnModel.Columns.Add(Descripcion);

            GroupingSummaryColumn GeneraDetalle = new GroupingSummaryColumn();
            GeneraDetalle.DataIndex = "GeneraDetalle";
            GeneraDetalle.Header = "Detalle";
            GeneraDetalle.Width = 60;
            panel.ColumnModel.Columns.Add(GeneraDetalle);

            GroupingSummaryColumn GeneraCorte = new GroupingSummaryColumn();
            GeneraCorte.DataIndex = "GeneraCorte";
            GeneraCorte.Header = "Corte";
            GeneraCorte.Width = 40;
            panel.ColumnModel.Columns.Add(GeneraCorte);

            GroupingSummaryColumn ID_Divisa = new GroupingSummaryColumn();
            ID_Divisa.DataIndex = "ID_Divisa";
            ID_Divisa.Hidden = true;
            panel.ColumnModel.Columns.Add(ID_Divisa);

            GroupingSummaryColumn Divisa = new GroupingSummaryColumn();
            Divisa.DataIndex = "Divisa";
            Divisa.Header = "Divisa";
            Divisa.Width = 100;
            panel.ColumnModel.Columns.Add(Divisa);

            GroupingSummaryColumn ID_Periodo = new GroupingSummaryColumn();
            ID_Periodo.DataIndex = "ID_Periodo";
            ID_Periodo.Hidden = true;
            panel.ColumnModel.Columns.Add(ID_Periodo);

            GroupingSummaryColumn Periodo = new GroupingSummaryColumn();
            Periodo.DataIndex = "Periodo";
            Periodo.Header = "Periodo";
            Periodo.Width = 80;
            panel.ColumnModel.Columns.Add(Periodo);

            GroupingSummaryColumn BreveDescripcion = new GroupingSummaryColumn();
            BreveDescripcion.DataIndex = "BreveDescripcion";
            BreveDescripcion.Header = "Breve Descripción";
            BreveDescripcion.Width = 120;
            panel.ColumnModel.Columns.Add(BreveDescripcion);

            GroupingSummaryColumn EditarSaldoGrid = new GroupingSummaryColumn();
            EditarSaldoGrid.DataIndex = "EditarSaldoGrid";
            EditarSaldoGrid.Header = "Saldo";
            EditarSaldoGrid.Width = 40;
            panel.ColumnModel.Columns.Add(EditarSaldoGrid);

            GroupingSummaryColumn InteractuaCajero = new GroupingSummaryColumn();
            InteractuaCajero.DataIndex = "InteractuaCajero";
            InteractuaCajero.Header = "Interactua Cajero";
            InteractuaCajero.Width = 100;
            panel.ColumnModel.Columns.Add(InteractuaCajero);

            GroupingSummaryColumn ID_NaturalezaCuenta = new GroupingSummaryColumn();
            ID_NaturalezaCuenta.DataIndex = "ID_NaturalezaCuenta";
            ID_NaturalezaCuenta.Header = "Naturaleza Cuenta";
            ID_NaturalezaCuenta.Width = 100;
            panel.ColumnModel.Columns.Add(ID_NaturalezaCuenta);

            panel.Title = "Tipos de Cuenta";
            panel.AutoExpandColumn = "Descripcion";

            if (!editable)
            {
                return panel;
            }

            ImageCommand EditarCommand = new ImageCommand();
            EditarCommand.Icon = Icon.Pencil;
            EditarCommand.CommandName = COMMAND_NAME;

            ImageCommandColumn EditarColumn = new ImageCommandColumn();
            EditarColumn.Width = 25;
            EditarColumn.Commands.Add(EditarCommand);
            panel.ColumnModel.Columns.Add(EditarColumn);

            panel.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter(
                    "ID_TipoCuenta", "record.data.ID_TipoCuenta", ParameterMode.Raw
                ));
            panel.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter(
                    "CommandName", "command", ParameterMode.Raw
                ));
            panel.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter(
                    "Values", "Ext.encode(record.data)", ParameterMode.Raw
                ));

            return panel;
        }
    }
}