using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Administracion.Componentes
{
    public class ProgramaComponente : GridPanel
    {
        public const string COMMAND_NAME = "EditPrograma";

        public static GridPanel CrearGridPanelBase(GridPanel panel, bool editable) 
        { 
            GroupingSummaryColumn ID_GrupoCuenta = new GroupingSummaryColumn();
            ID_GrupoCuenta.DataIndex = "ID_GrupoCuenta";
            ID_GrupoCuenta.Hidden = true;
            panel.ColumnModel.Columns.Add(ID_GrupoCuenta);

            GroupingSummaryColumn ClaveGrupoCuenta = new GroupingSummaryColumn();
            ClaveGrupoCuenta.DataIndex = "ClaveGrupoCuenta";
            ClaveGrupoCuenta.Header = "Clave";
            panel.ColumnModel.Columns.Add(ClaveGrupoCuenta);

            GroupingSummaryColumn Descripcion = new GroupingSummaryColumn();
            Descripcion.DataIndex = "Descripcion";
            Descripcion.Header = "Descripción";
            panel.ColumnModel.Columns.Add(Descripcion);

            GroupingSummaryColumn ID_ColectivaEmisor = new GroupingSummaryColumn();
            ID_ColectivaEmisor.DataIndex = "ID_ColectivaEmisor";
            ID_ColectivaEmisor.Hidden = true;
            panel.ColumnModel.Columns.Add(ID_ColectivaEmisor);

            GroupingSummaryColumn ColectivaDescripcion = new GroupingSummaryColumn();
            ColectivaDescripcion.DataIndex = "ColectivaDescripcion";
            ColectivaDescripcion.Header = "Colectiva";
            ColectivaDescripcion.Width = 180;
            panel.ColumnModel.Columns.Add(ColectivaDescripcion);

            GroupingSummaryColumn ID_Vigencia = new GroupingSummaryColumn();
            ID_Vigencia.DataIndex = "ID_Vigencia";
            ID_Vigencia.Hidden = true;
            panel.ColumnModel.Columns.Add(ID_Vigencia);

            GroupingSummaryColumn VigenciaDescripcion = new GroupingSummaryColumn();
            VigenciaDescripcion.DataIndex = "VigenciaDescripcion";
            VigenciaDescripcion.Header = "Vigenia";
            panel.ColumnModel.Columns.Add(VigenciaDescripcion);

            panel.Title = "Grupos Cuenta";
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
                    "ID_GrupoCuenta", "record.data.ID_GrupoCuenta", ParameterMode.Raw
                ));
            panel.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter(
                    "CommandName", "command", ParameterMode.Raw
                ));
            panel.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter(
                    "Values", "Ext.encode(record.data)", ParameterMode.Raw
                ));

            return panel;
        }

        public static Store CrearStore(Store storeProgramas) 
        {
            JsonReader jsonReader = new JsonReader();
            jsonReader.IDProperty = "ID_GrupoCuenta";

            jsonReader.Fields.Add("Descripcion");
            jsonReader.Fields.Add("ID_Vigencia");
            jsonReader.Fields.Add("ID_GrupoCuenta");
            jsonReader.Fields.Add("ClaveGrupoCuenta");
            jsonReader.Fields.Add("ID_ColectivaEmisor");
            jsonReader.Fields.Add("VigenciaDescripcion");
            jsonReader.Fields.Add("ColectivaDescripcion");

            storeProgramas.Reader.Add(jsonReader);

            return storeProgramas;
        }

        public static Store CrearStoreColectivas(Store storeColectivas)
        {
            JsonReader jsonReader = new JsonReader();
            jsonReader.IDProperty = "ID_Colectiva";

            jsonReader.Fields.Add("ID_Colectiva");
            jsonReader.Fields.Add("Descripcion");

            storeColectivas.Reader.Add(jsonReader);

            return storeColectivas;
        }

        public static Store CrearStoreVigencias(Store storeVigencias)
        {
            JsonReader jsonReader = new JsonReader();
            jsonReader.IDProperty = "ID_Vigencia";
   
            jsonReader.Fields.Add("ID_Vigencia");
            jsonReader.Fields.Add("Descripcion");
   
            storeVigencias.Reader.Add(jsonReader);

            return storeVigencias;
        }

    }
}