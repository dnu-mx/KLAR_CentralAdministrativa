using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Interfases.Exceptiones;
//using DALCentralAplicaciones.BaseDatos;
//using DALCentralAplicaciones.Entidades;
using Interfases;
using DALClubEscala.Entidades;
using DALClubEscala.BaseDatos;
//using DALCentralAplicaciones.Utilidades;

namespace ClubEscala.LogicaNegocio
{
    public class LNPropiedad
    {
        public static List<Propiedad> ObtieneParametros(Guid IDApp)
        {

            try
            {
                if (IDApp == null )
                {
                    throw new CAppException(8006, "No hay Aplicacion definida para obtener sus Parametros");
                }

                List<Propiedad> resp = new List<Propiedad>();
                using (SqlConnection conn = DBClubEscala.BDEscritura)
                {
                    conn.Open();

                    resp = DAOPropiedad.ObtenerPropiedades(IDApp);

                }
                return resp;
            }
            catch (CAppException err)
            {
               // Loguear.Error(err, "");
                throw err;
            }
            catch (Exception)
            {
              //  Loguear.Error(err, "");
                return new List<Propiedad>();
            }
        }

        public static void ModificaParametros(List<Propiedad> lasPropiedades, IUsuario elUser)
        {
             
            try
            {
                //int resp = -1;
                using (SqlConnection conn = DBClubEscala.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            foreach (Propiedad unaProp in lasPropiedades)
                            {
                                DAOPropiedad.Modicar(unaProp,elUser);
                            }

                            transaccionSQL.Commit();
                        }
                        catch (CAppException err)
                        {
                            transaccionSQL.Rollback();
                            throw err;
                        }
                        catch (Exception err)
                        {
                            transaccionSQL.Rollback();
                            throw new CAppException(8006, "Falla al Actualizar los parametros, No se Realizó la Actualizacion", err);
                        }
                    }
                }

            }
            catch (CAppException err)
            {
               // Loguear.Error(err, elUser.ClaveUsuario);
                throw err;
            }
            catch (Exception err)
            {
              //  Loguear.Error(err, elUser.ClaveUsuario);
                throw err;
            }


        }
    }
}
