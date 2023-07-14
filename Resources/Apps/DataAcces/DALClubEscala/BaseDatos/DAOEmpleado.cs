using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DALClubEscala.Entidades;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data.Common;
using ClubEscala.Utilidades;
using System.Data;
using Interfases.Exceptiones;
using System.Data.SqlClient;

namespace DALClubEscala.BaseDatos
{
   public class DAOEmpleado
    {
       public static int Agregar(Empleado elEmpleado, Int64 ID_Detalle, Int64 ID_CadenaComercial, DALCentralAplicaciones.Entidades.Usuario elUsuario, DbConnection UnaConexion, DbTransaction UnaTransaccion)
       {
           try
           {


               DbCommand command = UnaConexion.CreateCommand();
               command.CommandText="web_InsertarEmpleado";
               command.CommandType = CommandType.StoredProcedure;
               command.Transaction = UnaTransaccion;

               int resp = -1;
               
               command.Parameters.Add(new SqlParameter( "@ID_Detalle",ID_Detalle));
               command.Parameters.Add(new SqlParameter("@ID_CadenaComercial",  ID_CadenaComercial));
               command.Parameters.Add(new SqlParameter("@Nombre", elEmpleado.Nombre));
               command.Parameters.Add(new SqlParameter("@APaterno",  elEmpleado.APaterno));
               command.Parameters.Add(new SqlParameter("@AMaterno",  elEmpleado.AMaterno));
               command.Parameters.Add(new SqlParameter("@TelefonoMovil",  elEmpleado.TelefonoMovil));
               command.Parameters.Add(new SqlParameter("@EmailEmpresarial",  elEmpleado.EmailEmpresarial));
               command.Parameters.Add(new SqlParameter("@EmailPersonal",  elEmpleado.EmailPersonal));
               command.Parameters.Add(new SqlParameter("@FechaNacimiento",  elEmpleado.FechaNacimiento));
               command.Parameters.Add(new SqlParameter("@NumeroEmpleado",  elEmpleado.NumeroEmpleado));
               command.Parameters.Add(new SqlParameter("@DiaPago",  elEmpleado.DiaPago));
               command.Parameters.Add(new SqlParameter("@LimiteCompra", elEmpleado.LimiteCompra));
               command.Parameters.Add(new SqlParameter("@CicloNominal",elEmpleado.CicloNominal));
               command.Parameters.Add(new SqlParameter("@CampoReservado1",  elEmpleado.Reservado1));
               command.Parameters.Add(new SqlParameter("@CampoReservado2",  elEmpleado.Reservado2));
               command.Parameters.Add(new SqlParameter("@Baja", elEmpleado.Baja));

               resp = command.ExecuteNonQuery();

               return 0;

           }
           catch (Exception ex)
           {
               Loguear.Error(ex, elUsuario.ClaveUsuario);
               throw new Exception(ex.Message);
           }
       }

       public static int Actualiza(Empleado elEmpleado, EstatusEmpleado elEstatus)
       {
           try
           {


               DbCommand command = BDEmpleados.BDEscritura.CreateCommand();
               command.CommandText = "web_ActualizaEstatusEmpleado";
               command.CommandType = CommandType.StoredProcedure;
               //command.Transaction = UnaTransaccion;

               int resp = -1;

               command.Parameters.Add(new SqlParameter("@ID_Detalle", elEmpleado.ID_Detalle));
               command.Parameters.Add(new SqlParameter("@ID_Estatus", (int)elEstatus));


               resp = command.ExecuteNonQuery();

               return 0;

           }
           catch (Exception ex)
           {
               Loguear.Error(ex, "ProcesadorNocturno");
               throw new Exception(ex.Message);
           }
       }


       public static Empleado ObtieneEmpleadoPorProcesar(Int64 ID_Empleado)
       {
           Empleado unEmpleado = new Empleado();
           try
           {
               SqlDatabase database = new SqlDatabase(BDEmpleados.strBDLectura);
               DbCommand command = database.GetStoredProcCommand("web_SeleccionaUnEmpleadoPorProcesar");
               database.AddInParameter(command, "@ID_Empleado", DbType.Int64, ID_Empleado);
               //return database.ExecuteDataSet(command);
               DataSet losDatos = (DataSet)database.ExecuteDataSet(command);

               if (losDatos.Tables[0].Rows.Count > 0)
               {
                   for (int k = 0; k < losDatos.Tables[0].Rows.Count; k++)
                   {

                      // Empleado unEmpleado = new Empleado();

                       unEmpleado.Nombre = (String)losDatos.Tables[0].Rows[k]["Nombre"];
                       unEmpleado.APaterno = (String)losDatos.Tables[0].Rows[k]["APaterno"];
                       unEmpleado.AMaterno = (String)losDatos.Tables[0].Rows[k]["AMaterno"];
                       unEmpleado.Baja = (String)losDatos.Tables[0].Rows[k]["Baja"];
                       unEmpleado.CicloNominal = (String)losDatos.Tables[0].Rows[k]["CicloNominal"];
                       unEmpleado.DiaPago = ((String)losDatos.Tables[0].Rows[k]["DiaPago"]).ToString();
                       unEmpleado.EmailEmpresarial = (String)losDatos.Tables[0].Rows[k]["EmailEmpresarial"];
                       unEmpleado.EmailPersonal = (String)losDatos.Tables[0].Rows[k]["EmailPersonal"];
                       unEmpleado.FechaNacimiento = (DateTime)losDatos.Tables[0].Rows[k]["FechaNacimiento"];
                       unEmpleado.ID_CadenaComercial = Int64.Parse(losDatos.Tables[0].Rows[k]["ID_CadenaComercial"].ToString());
                       unEmpleado.ID_Empleado = Int64.Parse(losDatos.Tables[0].Rows[k]["Consecutivo"].ToString());
                       unEmpleado.LimiteCompra = ((Decimal)losDatos.Tables[0].Rows[k]["LimiteCompra"]).ToString();
                       unEmpleado.NumeroEmpleado = (String)losDatos.Tables[0].Rows[k]["NumeroEmpleado"];
                       unEmpleado.Reservado1 = (String)losDatos.Tables[0].Rows[k]["Reservado1"];
                       unEmpleado.Reservado2 = (String)losDatos.Tables[0].Rows[k]["Reservado2"];
                       unEmpleado.TelefonoMovil = (String)losDatos.Tables[0].Rows[k]["TelefonoMovil"];
                       unEmpleado.ID_Estatus = Int64.Parse(losDatos.Tables[0].Rows[k]["ID_Estatus"].ToString());
                       unEmpleado.ID_Detalle = (Int64)losDatos.Tables[0].Rows[k]["ID_Detalle"];
                       unEmpleado.Sucursal = (String)losDatos.Tables[0].Rows[k]["Sucursal"];
                       unEmpleado.Afiliacion = (String)losDatos.Tables[0].Rows[k]["Afiliacion"];
                       unEmpleado.Terminal = (String)losDatos.Tables[0].Rows[k]["Terminal"];

                   }
               }

               return unEmpleado;
           }
           catch (Exception ex)
           {
               Loguear.Error(ex, "ProcesadorNocturno");
               throw new Exception(ex.Message);
           }
       }

       public static Empleado ObtieneEmpleadoParaActivar(String NumeroEmpleado, DateTime FechaNacimiento)
       {
           Empleado unEmpleado = new Empleado();
           try
           {
               SqlDatabase database = new SqlDatabase(BDEmpleados.strBDLectura);
               DbCommand command = database.GetStoredProcCommand("web_SeleccionaUnEmpleadoParaActivar");
               database.AddInParameter(command, "@NumeroEmpleado", DbType.String, NumeroEmpleado);
               database.AddInParameter(command, "@FechaNacimiento", DbType.DateTime, FechaNacimiento);
               //return database.ExecuteDataSet(command);
               DataSet losDatos = (DataSet)database.ExecuteDataSet(command);

               if (losDatos.Tables[0].Rows.Count > 0)
               {
                   for (int k = 0; k < losDatos.Tables[0].Rows.Count; k++)
                   {

                       // Empleado unEmpleado = new Empleado();

                       unEmpleado.Nombre = (String)losDatos.Tables[0].Rows[k]["Nombre"];
                       unEmpleado.APaterno = (String)losDatos.Tables[0].Rows[k]["APaterno"];
                       unEmpleado.AMaterno = (String)losDatos.Tables[0].Rows[k]["AMaterno"];
                       unEmpleado.Baja = (String)losDatos.Tables[0].Rows[k]["Baja"];
                       unEmpleado.CicloNominal = (String)losDatos.Tables[0].Rows[k]["CicloNominal"];
                       unEmpleado.DiaPago = ((String)losDatos.Tables[0].Rows[k]["DiaPago"]).ToString();
                       unEmpleado.EmailEmpresarial = (String)losDatos.Tables[0].Rows[k]["EmailEmpresarial"];
                       unEmpleado.EmailPersonal = (String)losDatos.Tables[0].Rows[k]["EmailPersonal"];
                       unEmpleado.FechaNacimiento = (DateTime)losDatos.Tables[0].Rows[k]["FechaNacimiento"];
                       unEmpleado.ID_CadenaComercial = Int64.Parse((losDatos.Tables[0].Rows[k]["ID_CadenaComercial"]).ToString());
                       unEmpleado.ID_Empleado = Int64.Parse((losDatos.Tables[0].Rows[k]["Consecutivo"]).ToString());
                       unEmpleado.LimiteCompra = ((Decimal)losDatos.Tables[0].Rows[k]["LimiteCompra"]).ToString();
                       unEmpleado.NumeroEmpleado = (String)losDatos.Tables[0].Rows[k]["NumeroEmpleado"];
                       unEmpleado.Reservado1 = (String)losDatos.Tables[0].Rows[k]["Reservado1"];
                       unEmpleado.Reservado2 = (String)losDatos.Tables[0].Rows[k]["Reservado2"];
                       unEmpleado.TelefonoMovil = (String)losDatos.Tables[0].Rows[k]["TelefonoMovil"];
                       unEmpleado.ID_Estatus = Int64.Parse(((Int32)losDatos.Tables[0].Rows[k]["ID_Estatus"]).ToString());
                       unEmpleado.ID_Detalle = (Int64)losDatos.Tables[0].Rows[k]["ID_Detalle"];
                       unEmpleado.Sucursal = (String)losDatos.Tables[0].Rows[k]["Sucursal"];
                       unEmpleado.Afiliacion = (String)losDatos.Tables[0].Rows[k]["Afiliacion"];
                       unEmpleado.Terminal = (String)losDatos.Tables[0].Rows[k]["Terminal"];

                   }
               }
               else
               {
                   throw new Exception("Empleado no Existe en la base de datos:" + NumeroEmpleado + " Fecha:" + FechaNacimiento.ToShortDateString());
               }

               return unEmpleado;
           }
           catch (Exception ex)
           {
               Loguear.Error(ex, "ProcesadorNocturno");
               throw new Exception(ex.Message);
           }
       }
    
    }
}
