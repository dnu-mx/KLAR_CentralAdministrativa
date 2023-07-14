using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using DALCajero.BaseDatos;
using DALCajero.Entidades;
using Interfases;
using Interfases.Exceptiones;
using DALCajero.Utilidades;

namespace DALCajero.LogicaNegocio
{
    public class LNFichaDeposito
    {

        public static int EliminarFichaDeposito(FichaDeposito laFicha, IUsuario elUser)
        {

            try
            {
                //if (laFicha.ID_FichaDeposito == null || laFicha.ID_FichaDeposito == 0)
                if (laFicha.ID_FichaDeposito == 0)
                {
                    throw new CAppException(8006, "No hay una Ficha de Deposito Seleccionada para eliminar " + laFicha.ToString());
                }

                int resp = -1;
                using (SqlConnection conn = BDCajero.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            resp = DAOFichaDeposito.Eliminar(laFicha, elUser, conn, transaccionSQL);
                            Loguear.Evento("Se ha Eliminado la Ficha: " + laFicha.ToString(), elUser.ClaveUsuario);

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
                            throw new CAppException(8006, "Falla al Eliminar la Ficha en Base de Datos " + laFicha.ToString(), err);
                        }
                    }
                    
                }
                return resp;
            }
            catch (CAppException err)
            {
                Loguear.Error(err, elUser.ClaveUsuario);
                throw err;
            }
            catch (Exception err)
            {
                Loguear.Error(err, elUser.ClaveUsuario);
                return -1;
            }
        }

        public static Int64 AgregarFichaDeposito(FichaDeposito laFicha, IUsuario elUser)
        {

            try
            {
                if (!laFicha.EsCorrectoParaAgregar())
                {
                    throw new CAppException(8006, "Algunos Datos en la Ficha de Deposito no son Correctos para ser Insertados en la Base de Datos " + laFicha.ToString());
                }

                Int64 resp = -1;
                using (SqlConnection conn = BDCajero.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            resp = DAOFichaDeposito.Insertar(laFicha, elUser, conn, transaccionSQL);

                            laFicha.ID_FichaDeposito = resp;

                            Loguear.Evento("Se ha Insertado una nueva Ficha de Deposito:" + laFicha.ToString(), elUser.ClaveUsuario);

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
                            throw new CAppException(8006, "Falla al Agregar la Ficha en Base de Datos " + laFicha.ToString(), err);
                        }
                    }
                }
                return resp;
            }
            catch (CAppException err)
            {
                Loguear.Error(err, elUser.ClaveUsuario);
                throw err;
            }
            catch (Exception err)
            {
                Loguear.Error(err, elUser.ClaveUsuario);
                return -1;
            }
        }

    }
}

    