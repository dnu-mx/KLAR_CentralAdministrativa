using DALCajero.BaseDatos;
using DALCajero.Entidades;
using DALCajero.Utilidades;
using DALCentralAplicaciones.LogicaNegocio;
using Interfases;
using Interfases.Exceptiones;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DALCajero.LogicaNegocio
{
    public class LNMovimiento
    {
        public static RespuestaTransaccional EliminarMovimiento(Movimiento elMovimiento, IUsuario elUser)
        {
            RespuestaTransaccional laRespTrx = new RespuestaTransaccional();
            try
            {
               

                String ISO_ID = Guid.NewGuid().ToString();

                elMovimiento.ID_MensajeISO = ISO_ID;

                if ( elMovimiento.ID_Movimiento == 0)
                {
                    throw new CAppException(8006, "No hay una Movimiento Seleccionada para eliminar " + elMovimiento.ToString());
                }

                int resp = -1;
                using (SqlConnection conn = BDCajero.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            //Cambia de Estatus el Movimiento
                            resp = DAOMovimiento.Eliminar(elMovimiento, elUser, conn, transaccionSQL);


                            //if (resp == 0)
                            //{
                            //    //Crear el Objeto con los datos para la Transaccion
                            //    TrxOperacion laOperacion = new TrxOperacion
                            //    {
                            //        Afiliacion = "1234567",
                            //        Beneficiario = "CENTRAL_APLICACIONES",
                            //        FechaTransaccion = DateTime.Now,
                            //        HoraTransaccion = DateTime.Now,
                            //        Monto = elMovimiento.Importe.ToString(),
                            //        Operador = elUser.ClaveUsuario,
                            //        ProccesingCode = "000000",
                            //        Referencia =  elMovimiento.ID_Movimiento.ToString(),
                            //        Sucursal = "CAJERO",
                            //        Terminal = "ASG_FICHA",
                            //        Ticket = ISO_ID.ToString(),
                            //        MedioAcceso = "",
                            //        TipoMedioAcceso = "ID_Cuenta"
                            //    };

                            //    laRespTrx = LNTransaccional.ProcesaOperacion(laOperacion, ISO_ID, elUser);
                            //}
                            //else
                            //{

                            //    throw new CAppException(8005, "Ocurrio un Error al realizar la Asignación de la Ficha de Depósito" + elMovimiento.ToString());
                            //}

                            //if (laRespTrx.CodigoRespuesta != "0000")
                            //{

                            //    throw new CAppException(8004, "No se Autorizo la Operacion Transaccional para la Asignación de la Ficha de Depósito:<br/><b>Codigo Respuesta</b>: " + laRespTrx.CodigoRespuesta + "<br/><b>Descripcion</b>: " + laRespTrx.DescripcionRespuesta);
                            //}
                            Loguear.Evento("El Movimiento [" + elMovimiento.ID_Movimiento + "] Fue Eliminado correctamente", elUser.ClaveUsuario); 
                            transaccionSQL.Commit();
                            laRespTrx.Autorizacion = DateTime.Now.ToString("MMSSSM");
                            laRespTrx.CodigoRespuesta = "0000";
                        }
                        catch (CAppException err)
                        {
                            transaccionSQL.Rollback();
                            throw err;
                        }
                        catch (Exception err)
                        {
                            transaccionSQL.Rollback();
                            throw new CAppException(8006, "Falla al Eliminar el Movimiento en Base de Datos " + elMovimiento.ToString(), err);
                        }
                    }

                }
                return laRespTrx;
            }
            catch (CAppException err)
            {
                Loguear.Error(err, elUser.ClaveUsuario);
                throw err;
            }
            catch (Exception err)
            {
                Loguear.Error(err, elUser.ClaveUsuario);
                laRespTrx.Autorizacion ="000000";
                laRespTrx.CodigoRespuesta = "0001";
                return laRespTrx;
            }
        }

        public static RespuestaTransaccional AgregarMovimiento(Movimiento elMovimiento, IUsuario elUser, Guid AppId)
        {
            RespuestaTransaccional laRespTrx = new RespuestaTransaccional();
            TrxOperacion laOperacion = null;

            try
            {
               
                String ISO_ID = Guid.NewGuid().ToString();
                
                elMovimiento.ID_MensajeISO = ISO_ID;

                if (!elMovimiento.EsCorrectoParaAgregar())
                {
                    throw new CAppException(8006, "Algunos Datos del Movimiento no son Correctos para ser Insertados en la Base de Datos " + elMovimiento.ToString());
                }

                Int64 resp = -1;

                using (SqlConnection conn = BDCajero.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            //Inserta el movimiento
                            resp = DAOMovimiento.Insertar(elMovimiento, elUser, conn, transaccionSQL);
                            transaccionSQL.Commit();
                            elMovimiento.ID_Movimiento = resp;
                        }
                        catch (Exception err)
                        {
                            transaccionSQL.Rollback();
                            throw new CAppException(8005, "No fue posible Agregar el Movimiento",err);
                        }
                    }

                    String PCAsignacion = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "PrefijoDeposito").Valor + "0000"; /* +
                          DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "TCUniversal").Valor + 
                          DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "TCUniversal").Valor;*/




                        if (resp > 0)
                        {
                            //Crear el Objeto con los datos para la Transaccion
                             laOperacion = new TrxOperacion
                            {
                                Afiliacion = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "Afiliacion").Valor,
                                Beneficiario = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "Beneficiario").Valor,
                                FechaTransaccion = DateTime.Now,
                                HoraTransaccion = DateTime.Now,
                                Monto = elMovimiento.Importe.ToString(),
                                Operador = elUser.ClaveUsuario,
                                ProccesingCode = PCAsignacion, //DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "PCAgregarMovimiento").Valor,
                                Referencia = elMovimiento.ID_Movimiento.ToString(),
                                Sucursal = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "Sucursal").Valor,
                                Terminal = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "Terminal").Valor,
                                Adquirente = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "Afiliacion").Valor,
                                Ticket = Guid.NewGuid().ToString().Replace('-', '0').Substring(0, 12),
                                NIP ="0000",
                                CodigoMoneda = "MXN",
                                Track2 = "0000000000000000=0000",
                                MedioAcceso = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "MedioAccesoCajero").Valor,
                                TipoMedioAcceso = "IDCTA",
                                elTipoOperacion = TipoOperacion.Requerimiento
                            };

                            laRespTrx = LNTransaccional.ProcesaOperacion(laOperacion, ISO_ID, elUser);

                            using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                            {
                                try
                                {
                                    //Agregar la Operacion a la BD.
                                    DAOOperaciones.AgregarOperacion(laOperacion, laRespTrx, elUser, conn, transaccionSQL);
                                    transaccionSQL.Commit();
                                }
                                catch (Exception)
                                {
                                    transaccionSQL.Rollback();

                                    if ((laOperacion != null) && (laRespTrx.CodigoRespuesta == "0000"))
                                    {
                                        laOperacion.elTipoOperacion = TipoOperacion.Reverso;
                                        laRespTrx = LNTransaccional.ProcesaOperacion(laOperacion, ISO_ID, elUser);
                                        DAOOperaciones.AgregarOperacion(laOperacion, laRespTrx, elUser, conn, transaccionSQL);
                                    }

                                    throw new CAppException(8004, "Ocurrio un error al insertar la Operacion en la base de datos");
                                }
                            }

                            if (laRespTrx.CodigoRespuesta != "0000")
                            {

                                throw new CAppException(8004, "El Movimiento se Agregó a la BD pero NO se Autorizó la Activación:<br/><b>Codigo Respuesta</b>: " + laRespTrx.CodigoRespuesta + "<br/><b>Descripcion</b>: " + laRespTrx.DescripcionRespuesta + "<br/><br/>Intenta Activar el Movimiento desde el Listado de Movimientos.");
                            }
                            else
                            {
                                //si se autorizo al registrarlo se activa la operacion
                                using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                                {
                                    try
                                    {
                                        elMovimiento.elEstatus = enumEstatusMovimiento.Activo;

                                        //Cambia de Estatus el Movimiento
                                        resp = DAOMovimiento.Modificar(elMovimiento, elUser, conn, transaccionSQL);
                                        transaccionSQL.Commit();
                                    }
                                    catch (CAppException err)
                                    {
                                        transaccionSQL.Rollback();
                                        

                                        if ((laOperacion != null) && (laRespTrx.CodigoRespuesta == "0000"))
                                        {
                                            laOperacion.elTipoOperacion = TipoOperacion.Reverso;
                                            laRespTrx = LNTransaccional.ProcesaOperacion(laOperacion, ISO_ID, elUser);
                                            DAOOperaciones.AgregarOperacion(laOperacion, laRespTrx, elUser, conn, transaccionSQL);
                                        }

                                        throw new CAppException(8005, "El Movimiento se Activo correctamente pero no se pudo Cambiar el Estatus de la Ficha " + err.Mensaje(), err);
                                    }
                                    catch (Exception err)
                                    {
                                        transaccionSQL.Rollback();

                                        if ((laOperacion != null) && (laRespTrx.CodigoRespuesta == "0000"))
                                        {
                                            laOperacion.elTipoOperacion = TipoOperacion.Reverso;
                                            laRespTrx = LNTransaccional.ProcesaOperacion(laOperacion, ISO_ID, elUser);
                                            DAOOperaciones.AgregarOperacion(laOperacion, laRespTrx, elUser, conn, transaccionSQL);
                                        }
                                        throw new CAppException(8005, "El Movimiento fue activado correctamente pero no se pudo cambiar el estatus en la Base de Datos local.", err);
                                    }
                                }


                                //TRata de asignar el Movimiento con alguna ficha ya registrada.
                                using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                                {
                                    try
                                    {
                                       
                                        //Cambia de Estatus el Movimiento
                                        //resp = LNMovimiento..Modificar(elMovimiento, elUser, conn, transaccionSQL);
                                        laRespTrx = LNAsignacion.AsignarMovimientoAFichaDeposito(elMovimiento, elUser, enumTipoAsignacion.AlRegistrarMovimiento, AppId);
                                        transaccionSQL.Commit();
                                    }
                                    catch (CAppException err)
                                    {
                                        transaccionSQL.Rollback();
                                        throw new CAppException(8005, "El Movimiento se Activo correctamente pero no se pudo asignar a una ficha de depósito </BR>" + err.Mensaje(), err);
                                    }
                                    catch (Exception err)
                                    {
                                        transaccionSQL.Rollback();
                                        throw new CAppException(8005, "El Movimiento se Activo correctamente pero no se pudo asignar a una ficha de depósito " + err.Message, err);
                                    }
                                }
                             
                            }

                        }
                        else
                        {
                            throw new CAppException(8005, "No se Pudo agregar el Movimiento a la base de Datos local" + elMovimiento.ToString());
                        }

                }
                Loguear.Evento("El movimiento [" + resp + "]" + " Se Agregó, Se Activó y Se Asignó correctamente", elUser.ClaveUsuario);
                return laRespTrx;
            }
            catch (CAppException err)
            {
                Loguear.Error(err, elUser.ClaveUsuario);
                throw err;
            }
            catch (Exception err)
            {
                Loguear.Error(err, elUser.ClaveUsuario);
                laRespTrx.CodigoRespuesta = "-1";
                return laRespTrx;
            }
        }

        public static RespuestaTransaccional ActivarMovimiento(Movimiento elMovimiento, IUsuario elUser, Guid AppId)
        {
                RespuestaTransaccional laRespTrx = new RespuestaTransaccional();
                TrxOperacion laOperacion = null;
            try
            {
                
                String ISO_ID = Guid.NewGuid().ToString();

                elMovimiento.ID_MensajeISO = ISO_ID;

                //if (elMovimiento.ID_Movimiento == null || elMovimiento.ID_Movimiento == 0)
                if (elMovimiento.ID_Movimiento == 0)
                {
                    throw new CAppException(8006, "Movimiento invalido para modificar " + elMovimiento.ToString());
                }

                int resp = -1;
                using (SqlConnection conn = BDCajero.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            elMovimiento.elEstatus = enumEstatusMovimiento.Activo;

                            //Cambia de Estatus el Movimiento
                            resp = DAOMovimiento.Modificar(elMovimiento, elUser, conn, transaccionSQL);

                            String PCAsignacion = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "PrefijoDeposito").Valor + "0000"; /* +
                          DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "TCUniversal").Valor + 
                          DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "TCUniversal").Valor;*/

                            if (resp == 0)
                            {
                                //Crear el Objeto con los datos para la Transaccion
                                 laOperacion = new TrxOperacion
                                {
                                    Afiliacion = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "Afiliacion").Valor,
                                    Beneficiario = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "Beneficiario").Valor,
                                    FechaTransaccion = DateTime.Now,
                                    HoraTransaccion = DateTime.Now,
                                    Monto = elMovimiento.Importe.ToString(),
                                    Operador = elUser.ClaveUsuario,
                                    ProccesingCode =PCAsignacion,// DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "PCActivarMovimiento").Valor,
                                    Referencia = elMovimiento.ID_Movimiento.ToString(),
                                    Adquirente = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "Afiliacion").Valor,
                                    Sucursal = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "Sucursal").Valor,
                                    Terminal = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "Terminal").Valor,
                                    Ticket =  Guid.NewGuid().ToString().Replace('-', '0').Substring(0, 12),
                                    NIP = "0000",
                                     CodigoMoneda="MXN",
                                    Track2="0000000000000000=0000",
                                    MedioAcceso = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "MedioAccesoCajero").Valor,
                                    TipoMedioAcceso = "IDCTA",
                                    elTipoOperacion = TipoOperacion.Requerimiento
                                };

                                laRespTrx = LNTransaccional.ProcesaOperacion(laOperacion,  ISO_ID, elUser);

                                //Agregar la Operacion a la BD.
                                DAOOperaciones.AgregarOperacion(laOperacion, laRespTrx, elUser, conn, transaccionSQL);
                            }
                            else
                            {

                                throw new CAppException(8005, "Ocurrio un Error al realizar la Activacion del Movimiento" + elMovimiento.ToString());
                            }

                            if (laRespTrx.CodigoRespuesta != "0000")
                            {

                                throw new CAppException(8004, "No se Autorizo la Operacion Transaccional para la activacion del Movimiento:<br/><b>Codigo Respuesta</b>: " + laRespTrx.CodigoRespuesta + "<br/><b>Descripcion</b>: " + laRespTrx.DescripcionRespuesta);
                            }

                            transaccionSQL.Commit();
                            Loguear.Evento("El movimiento [" + elMovimiento.ID_Movimiento + "]" + " Fue Activado Satisfactoriamente", elUser.ClaveUsuario);
                        }
                        catch (CAppException err)
                        {
                            transaccionSQL.Rollback();
                            
                            if ((laOperacion != null) && (laRespTrx.CodigoRespuesta == "0000"))
                            {
                                laOperacion.elTipoOperacion = TipoOperacion.Reverso;
                                laRespTrx = LNTransaccional.ProcesaOperacion(laOperacion, ISO_ID, elUser);
                                DAOOperaciones.AgregarOperacion(laOperacion, laRespTrx, elUser, conn, transaccionSQL);
                            }

                            throw err;
                        }
                        catch (Exception err)
                        {
                            transaccionSQL.Rollback();

                            if ((laOperacion != null) && (laRespTrx.CodigoRespuesta == "0000"))
                            {
                                laOperacion.elTipoOperacion = TipoOperacion.Reverso;
                                laRespTrx = LNTransaccional.ProcesaOperacion(laOperacion, ISO_ID, elUser);
                                DAOOperaciones.AgregarOperacion(laOperacion, laRespTrx, elUser, conn, transaccionSQL);
                            }

                            throw new CAppException(8006, "Falla al modificar el Movimiento en Base de Datos " + elMovimiento.ToString(), err);
                        }
                    }

                }

                return laRespTrx;
            }
            catch (CAppException err)
            {
                Loguear.Error(err, elUser.ClaveUsuario);
                throw err;
            }
            catch (Exception err)
            {
                Loguear.Error(err, elUser.ClaveUsuario);
                laRespTrx.CodigoRespuesta = "-1";
                return laRespTrx;
            }
        }

        public static RespuestaTransaccional ResguardarMovimiento(Movimiento elMovimiento, IUsuario elUser, Guid AppId)
        {
            RespuestaTransaccional laRespTrx = new RespuestaTransaccional();
            TrxOperacion laOperacion = null;

            try
            {
                
                String ISO_ID = Guid.NewGuid().ToString();

                elMovimiento.ID_MensajeISO = ISO_ID;

                if ( elMovimiento.ID_Movimiento == 0)
                {
                    throw new CAppException(8006, "Movimiento invalido para modificar " + elMovimiento.ToString());
                }

                int resp = -1;
                using (SqlConnection conn = BDCajero.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            elMovimiento.elEstatus = enumEstatusMovimiento.EnResguardo;

                            //Cambia de Estatus el Movimiento
                            resp = DAOMovimiento.Modificar(elMovimiento, elUser, conn, transaccionSQL);

                            String PCAsignacion = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "PrefijoDeposito").Valor + "00" +
                          DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "TCResguardo").Valor;

                            if (resp == 0)
                            {
                                //Crear el Objeto con los datos para la Transaccion
                                laOperacion = new TrxOperacion
                                {
                                    Afiliacion = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "Afiliacion").Valor,
                                    Beneficiario = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "Beneficiario").Valor,
                                    FechaTransaccion = DateTime.Now,
                                    HoraTransaccion = DateTime.Now,
                                    Monto = elMovimiento.Importe.ToString(),
                                    Operador = elUser.ClaveUsuario,
                                    ProccesingCode = PCAsignacion,// DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "PCActivarMovimiento").Valor,
                                    Referencia = elMovimiento.ID_Movimiento.ToString(),
                                    Adquirente = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "Afiliacion").Valor,
                                    Sucursal = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "Sucursal").Valor,
                                    Terminal = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "Terminal").Valor,
                                    Ticket = Guid.NewGuid().ToString().Replace('-', '0').Substring(0, 12),
                                    NIP = "0000",
                                    CodigoMoneda = "MXN",
                                    Track2 = "0000000000000000=0000",
                                    MedioAcceso = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "MedioAccesoCajero").Valor,
                                    TipoMedioAcceso = "IDCTA",
                                    elTipoOperacion = TipoOperacion.Requerimiento
                                };

                                laRespTrx = LNTransaccional.ProcesaOperacion(laOperacion,  ISO_ID, elUser);

                                //Agregar la Operacion a la BD.
                                DAOOperaciones.AgregarOperacion(laOperacion, laRespTrx, elUser, conn, transaccionSQL);
                            }
                            else
                            {

                                throw new CAppException(8005, "Ocurrio un Error al realizar el Resguardo del Movimiento" + elMovimiento.ToString());
                            }

                            if (laRespTrx.CodigoRespuesta != "0000")
                            {

                                throw new CAppException(8004, "No se Autorizo la Operacion Transaccional para el Resguardo del Movimiento:<br/><b>Codigo Respuesta</b>: " + laRespTrx.CodigoRespuesta + "<br/><b>Descripcion</b>: " + laRespTrx.DescripcionRespuesta);
                            }

                            transaccionSQL.Commit();
                            Loguear.Evento("El movimiento [" + elMovimiento.ID_Movimiento + "]" + " Fue enviado a Resguardo Satisfactoriamente", elUser.ClaveUsuario);
                        }
                        catch (CAppException err)
                        {
                            transaccionSQL.Rollback();
                            if ((laOperacion != null) && (laRespTrx.CodigoRespuesta == "0000"))
                            {
                                laOperacion.elTipoOperacion = TipoOperacion.Reverso;
                                laRespTrx = LNTransaccional.ProcesaOperacion(laOperacion, ISO_ID, elUser);
                                DAOOperaciones.AgregarOperacion(laOperacion, laRespTrx, elUser, conn, transaccionSQL);
                            }
                            throw err;
                        }
                        catch (Exception err)
                        {
                            transaccionSQL.Rollback();

                            if ((laOperacion != null) && (laRespTrx.CodigoRespuesta == "0000"))
                            {
                                laOperacion.elTipoOperacion = TipoOperacion.Reverso;
                                laRespTrx = LNTransaccional.ProcesaOperacion(laOperacion, ISO_ID, elUser);
                                DAOOperaciones.AgregarOperacion(laOperacion, laRespTrx, elUser, conn, transaccionSQL);
                            }

                            throw new CAppException(8006, "Falla al Resguardar el Movimiento en Base de Datos " + elMovimiento.ToString(), err);
                        }
                    }

                }

                return laRespTrx;
            }
            catch (CAppException err)
            {
                Loguear.Error(err, elUser.ClaveUsuario);
                throw err;
            }
            catch (Exception err)
            {
                Loguear.Error(err, elUser.ClaveUsuario);
                laRespTrx.CodigoRespuesta = "-1";
                return laRespTrx;
            }
        }

        public static RespuestaTransaccional QuitarResguardoMovimiento(Movimiento elMovimiento, IUsuario elUser, Guid AppId)
        {
            RespuestaTransaccional laRespTrx = new RespuestaTransaccional();
            TrxOperacion laOperacion = null;
            try
            {
                
                String ISO_ID = Guid.NewGuid().ToString();

                elMovimiento.ID_MensajeISO = ISO_ID;

                //if (elMovimiento.ID_Movimiento == null || elMovimiento.ID_Movimiento == 0)
                if (elMovimiento.ID_Movimiento == 0)
                {
                    throw new CAppException(8006, "Movimiento invalido para modificar " + elMovimiento.ToString());
                }



                int resp = -1;
                using (SqlConnection conn = BDCajero.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            elMovimiento.elEstatus = enumEstatusMovimiento.Activo;

                            //Cambia de Estatus el Movimiento
                            resp = DAOMovimiento.Modificar(elMovimiento, elUser, conn, transaccionSQL);

                            String PCAsignacion = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "PrefijoDeposito").Valor  +
                                                  DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "TCResguardo").Valor + DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "TCUniversal").Valor; /* + 
                          DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "TCUniversal").Valor;*/



                            if (resp == 0)
                            {
                                //Crear el Objeto con los datos para la Transaccion
                                 laOperacion = new TrxOperacion
                                {
                                    Afiliacion = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "Afiliacion").Valor,
                                    Beneficiario = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "Beneficiario").Valor,
                                    FechaTransaccion = DateTime.Now,
                                    HoraTransaccion = DateTime.Now,
                                    Monto = elMovimiento.Importe.ToString(),
                                    Operador = elUser.ClaveUsuario,
                                    ProccesingCode =PCAsignacion,// DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "PCQuitarResguardo").Valor,
                                    Referencia = elMovimiento.ID_Movimiento.ToString(),
                                    Sucursal = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "Sucursal").Valor,
                                    Terminal = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "Terminal").Valor,
                                    Ticket = Guid.NewGuid().ToString().Replace('-', '0').Substring(0, 12),
                                    NIP = "0000",
                                    Adquirente=DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "Afiliacion").Valor,
                                    CodigoMoneda = "MXN",
                                    Track2 = "0000000000000000=0000",
                                    MedioAcceso = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "MedioAccesoCajero").Valor,
                                    TipoMedioAcceso = "IDCTA",
                                    elTipoOperacion = TipoOperacion.Requerimiento
                                     
                                };

                                laRespTrx = LNTransaccional.ProcesaOperacion(laOperacion, ISO_ID, elUser);

                                //Agregar la Operacion a la BD.
                                DAOOperaciones.AgregarOperacion(laOperacion, laRespTrx, elUser, conn, transaccionSQL);
                            }
                            else
                            {

                                throw new CAppException(8005, "Ocurrio un Error al realizar la Activacion del Movimiento" + elMovimiento.ToString());
                            }

                            if (laRespTrx.CodigoRespuesta != "0000")
                            {

                                throw new CAppException(8004, "No se Autorizo la Operacion Transaccional para la activacion del Movimiento:<br/><b>Codigo Respuesta</b>: " + laRespTrx.CodigoRespuesta + "<br/><b>Descripcion</b>: " + laRespTrx.DescripcionRespuesta + ". Movimiento: " + elMovimiento.ToString());
                            }

                            transaccionSQL.Commit();
                            Loguear.Evento("El movimiento [" + elMovimiento.ToString() + "]" + " Fue Sacado de Resguardo", elUser.ClaveUsuario);
                        }
                        catch (CAppException err)
                        {
                            transaccionSQL.Rollback();

                            if ((laOperacion != null) && (laRespTrx.CodigoRespuesta == "0000"))
                            {
                                laOperacion.elTipoOperacion = TipoOperacion.Reverso;
                                laRespTrx = LNTransaccional.ProcesaOperacion(laOperacion, ISO_ID, elUser);
                                DAOOperaciones.AgregarOperacion(laOperacion, laRespTrx, elUser, conn, transaccionSQL);
                            }

                            throw err;
                        }
                        catch (Exception err)
                        {
                            transaccionSQL.Rollback();

                            if ((laOperacion != null) && (laRespTrx.CodigoRespuesta == "0000"))
                            {
                                laOperacion.elTipoOperacion = TipoOperacion.Reverso;
                                laRespTrx = LNTransaccional.ProcesaOperacion(laOperacion, ISO_ID, elUser);
                                DAOOperaciones.AgregarOperacion(laOperacion, laRespTrx, elUser, conn, transaccionSQL);
                            }
                            throw new CAppException(8006, "Falla al modificar el Movimiento en Base de Datos " + elMovimiento.ToString(), err);
                        }
                    }

                }
                return laRespTrx;
            }
            catch (CAppException err)
            {
                Loguear.Error(err, elUser.ClaveUsuario);
                throw err;
            }
            catch (Exception err)
            {
                Loguear.Error(err, elUser.ClaveUsuario);
                laRespTrx.CodigoRespuesta = "-1";
                return laRespTrx;
            }
        }

        public static DataSet BuscarMovimiento(Movimiento elMovimiento, IUsuario elUser, Guid AppId)
        {
            DataSet losMovimientos;

            try
            {

                //valida los datos proporcionados por el usuario.
                int DatosProporcionados = 0;

                if (elMovimiento.ClaveColectivaBanco.Trim() != "")
                {
                    DatosProporcionados++;
                }

                if (elMovimiento.ClaveSucursalBancaria.Trim() != "")
                {
                    DatosProporcionados++;
                }

                if (elMovimiento.ConsecutivoBancario != 0)
                {
                    DatosProporcionados++;
                }

                if (elMovimiento.Importe != 0)
                {
                    DatosProporcionados++;
                }

                if (elMovimiento.FechaOperacion.Year >= DateTime.Now.Year-1)
                {
                    DatosProporcionados++;
                }

                //Si los datos proporcionado son menores a 3 entonces no realiza la busqueda
                if (DatosProporcionados<3)
                {
                    throw new CAppException(8006, "Proporciona por lo menos 3 datos en la busqueda");
                }


                //int resp = -1;
                using (SqlConnection conn = BDCajero.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            //Cambia de Estatus el Movimiento
                            losMovimientos = DAOMovimiento.BuscaMovimientosResguardo(elUser, elMovimiento, AppId);

                            Loguear.Evento("Se realizo una busqueda de movimientos en resguardo con los datos: " + elMovimiento.ToString(), elUser.ClaveUsuario);
                        }
                        catch (CAppException err)
                        {
                            throw err;
                        }
                        catch (Exception err)
                        {
                            transaccionSQL.Rollback();
                            throw new CAppException(8006, "Falla al modificar el Movimiento en Base de Datos " + elMovimiento.ToString(), err);
                        }
                    }

                }

               

                return losMovimientos;
            }
            catch (CAppException err)
            {
                Loguear.Error(err, elUser.ClaveUsuario);
                throw err;
            }
            catch (Exception err)
            {
                Loguear.Error(err, elUser.ClaveUsuario);
                throw err;
            }
        }

    }
}
