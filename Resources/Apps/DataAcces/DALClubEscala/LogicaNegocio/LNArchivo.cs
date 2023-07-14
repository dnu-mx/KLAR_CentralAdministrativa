using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DALCentralAplicaciones.Entidades;
using System.IO;
using System.Collections;
using DALClubEscala.Entidades;
using DALClubEscala.BaseDatos;
using ClubEscala.Utilidades;
using System.Data.SqlClient;
using System.Data.Common;

namespace DALClubEscala.LogicaNegocio
{
    public class LNArchivo
    {
        public static Int64 ImportaArchivoBD(String URLArchivo, Int64 ID_CadenaComercial, Usuario elUsuario)
        {
            try
            {
                Archivo elArchivo = new Archivo();
                List<Detalle> losDetalles = new List<Detalle>();
                //1.crear el archivo

                using (SqlConnection conn = BDEmpleados.BDEscritura)
                {
                    //conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            elArchivo = CrearArchivo(URLArchivo, ID_CadenaComercial, elUsuario, conn, transaccionSQL);

                            //2.agregar detalles
                            losDetalles = ObtieneDetallesDeArchivo(elArchivo);

                            //3.guardar detalles 
                            if (!(DAOArchivo.AgregarDetalle(elArchivo.ID_Archivo, ref losDetalles, elUsuario, conn, transaccionSQL) == 0))
                            {
                                throw new Exception("No se agregaron los detalles del archivo");
                            }

                            elArchivo.Detalles = losDetalles;

                            //Decodificar los Empleados.
                            if (!(InsertarEmpleados(elArchivo, ID_CadenaComercial, elUsuario, conn, transaccionSQL) == 0))
                            {
                                throw new Exception("No se importaron los Empleados del archivo");
                            }

                            transaccionSQL.Commit();

                            return elArchivo.ID_Archivo;
                        }
                        catch (Exception err)
                        {
                            Loguear.Error(err, elUsuario.ClaveUsuario);
                            transaccionSQL.Rollback();
                            return -1;
                        }

                       
                    }
                }

               

            }
            catch (Exception err)
            {
                Loguear.Error(err, elUsuario.ClaveUsuario);
                return -1;
            }

        }

        private static List<Detalle> ObtieneDetallesDeArchivo(Archivo UnArchivo)
        {
            try
            {
                StreamReader objReader = new StreamReader(UnArchivo.UrlArchivo);
                string sLine = "";
                List<Detalle> losDetalles = new List<Detalle>();

                while (sLine != null)
                {
                    sLine = objReader.ReadLine();
                    if (sLine != null)
                    {
                        //   arrText.Add(sLine);
                        Detalle undeta = new Detalle();
                        undeta.FilaCompleta = sLine;
                        undeta.ID_Archivo = UnArchivo.ID_Archivo;

                        losDetalles.Add(undeta);
                    }
                }
                objReader.Close();

                return losDetalles;
            }
            catch (Exception)
            {
                throw new Exception("No se pudo leer el Archivo copiado al servidor");
            }
        }

        private static Archivo CrearArchivo(String UrlArchivo, Int64 ID_CadenaComercial, Usuario elUsuario, DbConnection conn, DbTransaction transaccionSQL)
        {
            try
            {
                Archivo elArchivo = new Archivo();


                elArchivo.ID_CadenaComercial = ID_CadenaComercial;
                elArchivo.Nombre = Path.GetFileName(UrlArchivo);
                elArchivo.UrlArchivo = UrlArchivo;
                elArchivo.CA_Usuario = elUsuario.ClaveUsuario;

                DAOArchivo.Agregar(ref elArchivo, elUsuario, conn, transaccionSQL);

                return elArchivo;
            }
            catch (Exception)
            {
                throw new Exception("No se pudo crear el Archivo en la BD");
            }

        }

        private static int InsertarEmpleados(Archivo elArchivo, Int64 ID_CadenaComercial, Usuario elUsuario, DbConnection conn, DbTransaction transaccionSQL)
        {
            try
            {
                foreach (Detalle elDetalle in elArchivo.Detalles)
                {
                    try
                    {
                        String[] losDatos = elDetalle.FilaCompleta.Split(',');

                        if (losDatos.Length > 8)
                        {
                            Empleado unEmpleado = new Empleado();
                            unEmpleado.NumeroEmpleado = losDatos[1];
                            unEmpleado.Nombre = losDatos[2];
                            unEmpleado.APaterno = losDatos[3];
                            unEmpleado.AMaterno = losDatos[4];
                            try
                            {
                                unEmpleado.FechaNacimiento = new DateTime(Int32.Parse(losDatos[5]), Int32.Parse(losDatos[6]), Int32.Parse(losDatos[7]));
                            }
                            catch (Exception)
                            {
                                throw new Exception("Fecha invalida, el formato debe ser aaaa-mm-dd se recibió:" + losDatos[5] + "/" + losDatos[6] + "/" + losDatos[7]);
                            }
                            unEmpleado.LimiteCompra = losDatos[8];
                            unEmpleado.CicloNominal = losDatos[9];

                            if (losDatos.Length >= 11)
                            {
                                unEmpleado.DiaPago = losDatos[10];
                            }

                            if (losDatos.Length >= 12)
                            {
                                unEmpleado.EmailEmpresarial = losDatos[11];
                            }

                            if (losDatos.Length >= 13)
                            {
                                unEmpleado.EmailPersonal = losDatos[12];
                            }

                            if (losDatos.Length >= 14)
                            {
                                unEmpleado.TelefonoMovil = losDatos[13];
                            }

                            if (losDatos.Length >= 15)
                            {
                                unEmpleado.Baja = losDatos[14];
                            }

                            if (losDatos.Length >= 17)
                            {
                                unEmpleado.Reservado1 = losDatos[15];
                            }

                            if (losDatos.Length >= 17)
                            {
                                unEmpleado.Reservado2 = losDatos[16];
                            }



                                    DAOEmpleado.Agregar(unEmpleado, elDetalle.ID_Detalle, ID_CadenaComercial, elUsuario, conn, transaccionSQL);


                        }
                        else
                        {
                            throw new Exception("EL registro ID:" + elDetalle.ID_Detalle + " [" + elDetalle.FilaCompleta + "] no contiene los campos necesarios para procesarlo");
                        }
                    }
                    catch (Exception err)
                    {
                        Loguear.Error(err, elUsuario.ClaveUsuario);
                    }

                }
                return 0;
            }
            catch (Exception err)
            {
                Loguear.Error(err, elUsuario.ClaveUsuario);
                return -1;
            }
        }


        public static void ActualizaEstatusArchivo(Int64 ID_Archivo, EstatusArchivo elEstatus,Usuario elUsuario)
        {
            try
            {
                DAOArchivo.Actualiza(ID_Archivo, elEstatus);
            }
            catch (Exception err)
            {
                Loguear.Error(err, elUsuario.ClaveUsuario);
                return;
            }
        }
      }
}
