using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data.Common;
using System.Data;
using ClubEscala.Utilidades;
using Interfases.Entidades;

namespace DALClubEscala.BaseDatos
{
    class DAOConfiguracion
    {
        public static Dictionary<Int64, Dictionary<String, Parametro>> GetConf()
        {

            Dictionary<Int64, Dictionary<String, Parametro>> laRespuesta = new Dictionary<Int64, Dictionary<String, Parametro>>();
            Dictionary<String, Parametro> lasPropiedades = new Dictionary<string, Parametro>();
            try
            {
                SqlDatabase database = new SqlDatabase(BDEmpleados.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ObtieneValoresConfiguracion");

                DataSet losDatos = (DataSet)database.ExecuteDataSet(command);

                if (null != losDatos)
                {
                    Int64 AppActual;

                    for (int k = 0; k < losDatos.Tables[0].Rows.Count; k++)
                    {
                        AppActual = losDatos.Tables[0].Rows[k]["ID_CadenaComercial"] == null ? 0 : (Int64)losDatos.Tables[0].Rows[k]["ID_CadenaComercial"]; //(String)losDatos.Tables[0].Rows[k]["RoleName"];
                        bool LeyoDatos = true;

                        while ((AppActual == (losDatos.Tables[0].Rows[k]["ID_CadenaComercial"] == null ? 0 : (Int64)losDatos.Tables[0].Rows[k]["ID_CadenaComercial"])))
                        {
                            String nombre = losDatos.Tables[0].Rows[k]["Nombre"] == null ? "" : (String)losDatos.Tables[0].Rows[k]["Nombre"];
                            String value = losDatos.Tables[0].Rows[k]["Valor"] == null ? "" : (String)losDatos.Tables[0].Rows[k]["Valor"];


                            Parametro unParamentro = new Parametro();
                            unParamentro.Nombre = nombre;
                            unParamentro.Valor = value;

                            lasPropiedades.Add(nombre.ToUpper(), unParamentro);

                            if (k == (losDatos.Tables[0].Rows.Count - 1))
                            {
                                break;
                            }
                            else
                            {
                                k++;// LeyoDatos = losDatos.Read();
                            }

                        }

                        laRespuesta.Add(AppActual, lasPropiedades);


                        lasPropiedades = new Dictionary<string, Parametro>();

                        if (LeyoDatos)
                        {
                            String nombre = losDatos.Tables[0].Rows[k]["Nombre"] == null ? "" : (String)losDatos.Tables[0].Rows[k]["Nombre"];
                            String value = losDatos.Tables[0].Rows[k]["Valor"] == null ? "" : (String)losDatos.Tables[0].Rows[k]["Valor"];



                            Parametro unParamentro = new Parametro();
                            unParamentro.Nombre = nombre;
                            unParamentro.Valor = value;

                            lasPropiedades.Add(nombre.ToUpper(), unParamentro);


                            AppActual = losDatos.Tables[0].Rows[k]["ID_CadenaComercial"] == null ? 0 : (Int64)losDatos.Tables[0].Rows[k]["ID_CadenaComercial"];
                        }
                    }
                }
                return laRespuesta;
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, "");
                throw new Exception(ex.Message);
            }
        }

        public static Dictionary<String, Parametro> MuestraConfiguraciones()
        {

            //  Dictionary<Int64, Dictionary<String, Parametro>> laRespuesta = new Dictionary<Int64, Dictionary<String, Parametro>>();
            Dictionary<String, Parametro> lasPropiedades = new Dictionary<string, Parametro>();
            try
            {
                SqlDatabase database = new SqlDatabase(BDEmpleados.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ObtieneValoresConfiguracion");

                DataSet losDatos = (DataSet)database.ExecuteDataSet(command);

                if (null != losDatos)
                {
                    Int64 AppActual;

                    for (int k = 0; k < losDatos.Tables[0].Rows.Count; k++)
                    {
                        AppActual = losDatos.Tables[0].Rows[k]["ID_CadenaComercial"] == null ? 0 : (Int64)losDatos.Tables[0].Rows[k]["ID_CadenaComercial"]; //(String)losDatos.Tables[0].Rows[k]["RoleName"];

                        String nombre = losDatos.Tables[0].Rows[k]["Nombre"] == null ? "" : (String)losDatos.Tables[0].Rows[k]["Nombre"];
                        String value = losDatos.Tables[0].Rows[k]["Valor"] == null ? "" : (String)losDatos.Tables[0].Rows[k]["Valor"];


                        Parametro unParamentro = new Parametro();
                        unParamentro.Nombre = "Cadena:" + AppActual + ", Nombre: " + nombre;
                        unParamentro.Valor = value;

                        lasPropiedades.Add(unParamentro.Nombre.ToUpper(), unParamentro);

                    }
                }
                return lasPropiedades;
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, "");
                throw new Exception(ex.Message);
            }
        }
    }
}
