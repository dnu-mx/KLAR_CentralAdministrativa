using Interfases;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Utilidades;
using System;
using System.Data;
using System.Globalization;

namespace Utilerias
{
    public class Tarjetas
    {
        /// <summary>
        /// Enmascara los números de tarjeta de un DataTable, según la definición de enmascarado de PCI
        /// </summary>
        /// <param name="elOriginal">Tabla con los datos originales por enmascarar</param>
        /// <param name="campoTarjeta">Nombre del campo en la tabla con los datos de tarjetas</param>
        /// <param name="campoVista">Nombre del campo en la tabla donde se indica la vista de permisos 
        /// para enmascarar o no el número de tarjeta</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Tabla con los datos de tarjeta enmascarados o con formato, según aplique la vista</returns>
        public static DataTable EnmascaraTablaConTarjetas(DataTable elOriginal, string campoTarjeta, string campoVista,
            ILogHeader logHeader)
        {
            LogPCI log = new LogPCI(logHeader);
            log.Info("INICIA EnmascaraTablaConTarjetas()");

            try
            {
                DataTable elEnmascarado = elOriginal.Clone();

                foreach (DataRow row in elOriginal.Rows)
                {
                    string valorVista = row[campoVista].ToString();
                    string laTarjeta = row[campoTarjeta].ToString().Trim();

                    if (valorVista == "TARJ" && !string.IsNullOrEmpty(laTarjeta))
                    {
                        row[campoTarjeta] = MaskSensitiveData.cardNumber(row[campoTarjeta].ToString().Trim());
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(laTarjeta))
                        {
                            row[campoTarjeta] = laTarjeta.Substring(0, 4) + " " +
                                laTarjeta.Substring(4, 4) + " " +
                                laTarjeta.Substring(8, 4) + " " +
                                laTarjeta.Substring(12, 4);
                        }
                    }

                    elEnmascarado.ImportRow(row);
                }

                elEnmascarado.AcceptChanges();
                elEnmascarado.Columns.Remove(campoVista);
                
                log.Info("TERMINA EnmascaraTablaConTarjetas()");

                return elEnmascarado;

            }
            catch (CAppException caEx)
            {
                log.Error(caEx.Mensaje());
                throw caEx;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Implementa el algoritmo definido para la generación del dígito verificador de un
        /// medio de acceso
        /// </summary>
        /// <param name="numTarjeta">Tarjeta o medio de acceso</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Valor del dígito verificador</returns>
        public static int GeneraDigitoVerificador(string numTarjeta, ILogHeader logHeader)
        {
            LogPCI log = new LogPCI(logHeader);
            log.Info("GeneraDigitoVerificador()");

            try
            {
                int Total = 0, dig = 0, i = 0;
                string Factor = "212121212121212", sResultado;
                string Car1, Car2;
                float fCar1, fCar2, Resultado;

                while (i < numTarjeta.Length - 1)
                {
                    Car1 = numTarjeta.Substring(i, 1);
                    Car2 = Factor.Substring(i, 1);

                    fCar1 = float.Parse(Car1, CultureInfo.InvariantCulture.NumberFormat);
                    fCar2 = float.Parse(Car2, CultureInfo.InvariantCulture.NumberFormat);

                    Resultado = fCar1 * fCar2;
                    if (Resultado > 9)
                    {
                        sResultado = Convert.ToString(Resultado);
                        fCar1 = float.Parse(sResultado.Substring(0, 1), CultureInfo.InvariantCulture.NumberFormat);
                        fCar2 = float.Parse(sResultado.Substring(1, 1), CultureInfo.InvariantCulture.NumberFormat);
                        Resultado = fCar1 + fCar2;
                    }

                    Total = Total + Convert.ToInt32(Resultado);

                    i++;
                }

                dig = Total % 10;

                if (dig > 0)
                    dig = 10 - dig;

                return dig;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Enmascara los números de tarjeta de un DataTable, dejando visibles sólo los últimos 4 dígitos
        /// </summary>
        /// <param name="elOriginal">Tabla con los datos originales por enmascarar</param>
        /// <param name="campoTarjeta">Nombre del campo en la tabla con los datos de tarjetas</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Tabla con los datos de tarjeta enmascarados o con formato, según aplique la vista</returns>
        public static DataTable EnmascaraTarjetasSolo4Dig(DataTable elOriginal, string campoTarjeta, ILogHeader logHeader)
        {
            LogPCI log = new LogPCI(logHeader);

            try
            {
                DataTable elEnmascarado = elOriginal.Clone();

                foreach (DataRow row in elOriginal.Rows)
                {
                    row[campoTarjeta] = MaskSensitiveData.cardNumber4Digits(row[campoTarjeta].ToString());

                    elEnmascarado.ImportRow(row);
                }

                elEnmascarado.AcceptChanges();

                return elEnmascarado;

            }
            catch (CAppException caEx)
            {
                log.Error(caEx.Mensaje());
                throw caEx;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw ex;
            }
        }
    }
}
