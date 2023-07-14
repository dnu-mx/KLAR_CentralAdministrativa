using DALAdministracion.BaseDatos;
using DALAdministracion.Entidades;
using DALAutorizador.BaseDatos;
using DALAutorizador.Utilidades;
using DALCentralAplicaciones.Entidades;
using Interfases.Exceptiones;
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace DALAdministracion.LogicaNegocio
{
    /// <summary>
    /// Establece la lógica de negocio para la entidad Rango
    /// </summary>
    public class LNRangos
    {
        /// <summary>
        /// Establece las condiciones de validación para la creación de un nuevo rango
        /// </summary>
        /// <param name="nuevoRango">Nuevo rango por crear</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void ValidaNuevoRango(Rango nuevoRango, Usuario usuario)
        {
            try
            {
                //Si el tipo de medio de acceso seleccionado no es correo electrónico
                //o sin MA, se realizan validaciones de numeración
                if (!nuevoRango.DescTipoMA.ToUpperInvariant().Contains("CORREO") ||
                    !nuevoRango.DescTipoMA.ToUpperInvariant().Contains("SIN"))
                {

                    //Se obtiene la longitud del tipo de medio de acceso capturado
                    int longTipoMA = DAORango.ConsultaLongitudMA(
                            nuevoRango.ID_TipoMA, usuario,
                            Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                    //Se valida que la numeración de inicio tenga la longitud correcta
                    if (nuevoRango.Inicio.ToString().Length != longTipoMA)
                    {
                        throw new CAppException(8006, "La longitud de la Numeración de Inicio es Incorrecta, deben ser "
                            + longTipoMA.ToString() + " dígitos");
                    }

                    //Se valida que la numeración de fin tenga la longitud correcta
                    if (nuevoRango.Fin.ToString().Length != longTipoMA)
                    {
                        throw new CAppException(8006, "La longitud de la Numeración de Fin es Incorrecta, deben ser "
                            + longTipoMA.ToString() + " dígitos");
                    }

                    //Se valida numeración coherente en el rango
                    if (nuevoRango.Inicio > nuevoRango.Fin)
                    {
                        throw new CAppException(8006, "El Número Inicial del Rango debe ser Menor o Igual al Número Final.");
                    }

                    //Si el tipo de medio de acceso es tarjeta, se valida el DV
                    if (nuevoRango.DescTipoMA.ToUpperInvariant().Contains("TARJETA"))
                    {
                        if (!ValidaDigitoVerificador(nuevoRango.Inicio.ToString()))
                        {
                            throw new CAppException(8006, "El Dígito Verificador del Número Inicial del Rango es Incorrecto.");
                        }

                        if (!ValidaDigitoVerificador(nuevoRango.Fin.ToString()))
                        {
                            throw new CAppException(8006, "El Dígito Verificador del Número Final del Rango es Incorrecto.");
                        }
                    }
                }


                using (SqlConnection conn = BDAutorizador.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAORango.insertar(nuevoRango, usuario);
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
                            throw new CAppException(8006, "Falla al Insertar nuevo Rango de GMA en Base de Datos ", err);
                        }
                    }
                }
            }
            catch (CAppException err)
            {
                Loguear.Error(err, usuario.ClaveUsuario);
                throw err;
            }
            catch (Exception err)
            {
                Loguear.Error(err, usuario.ClaveUsuario);
                throw err;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la creación de un nuevo rango
        /// </summary>
        /// <param name="elRango">Nuevo rango por crear</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void ValidaRangoEditado(Rango elRango, Usuario usuario)
        {
            try
            {
                //Se valida que estén capturados clave y descripción para poder almacenar el registro
                if (String.IsNullOrEmpty(elRango.Clave)
                    || String.IsNullOrEmpty(elRango.Descripcion))
                {
                    throw new CAppException(8006, "Proporciona Todos los Datos para Modificar el Rango");
                }

                //Se valida que se capturen los números de inicio y fin del rango de tarjeta
                if (elRango.Inicio == 0 || elRango.Fin == 0)
                {
                    throw new CAppException(8006, "Proporciona la Numeración de Inicio y Fin para Modificar el Rango");
                }

                //Se valida numeración coherente en el rango
                if (elRango.Inicio > elRango.Fin)
                {
                    throw new CAppException(8006, "El Número Inicial del Rango debe ser Menor o Igual al Número Final.");
                }


                using (SqlConnection conn = BDAutorizador.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAORango.actualizar(elRango, usuario);
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
                            throw new CAppException(8006, "Falla al Actualizar Rango de GMA en Base de Datos ", err);
                        }
                    }
                }
            }
            catch (CAppException err)
            {
                Loguear.Error(err, usuario.ClaveUsuario);
                throw err;
            }
            catch (Exception err)
            {
                Loguear.Error(err, usuario.ClaveUsuario);
                throw err;
            }
        }

        /// <summary>
        /// Valida el dígito verificador de la tarjeta usando el algoritmo de Luhn. 
        /// </summary>
        /// <param name="numTarjeta">Número de la tarjeta por validar</param>
        /// <returns>True si el DV es correcto, False en caso contrario</returns>
        private static bool ValidaDigitoVerificador(string numTarjeta)
        {
            int longtarjeta, suma, resultado, i;	

            longtarjeta = numTarjeta.Length;
            suma = 0;
            resultado = 0;
            i = 1;

            while (i != longtarjeta)
            {
                resultado = ((i%2)+1) * (int.Parse(numTarjeta.Substring(longtarjeta-i-1, 1))&15);

                if (resultado > 9)
                {
                    resultado -= 9;
                }

                suma += resultado;

                i++;
            }
	
            suma = (10-(suma%10))%10;

            return ((int.Parse(numTarjeta.Substring(i-1, 1))&15) == suma) ? true : false;
        }
    }
}