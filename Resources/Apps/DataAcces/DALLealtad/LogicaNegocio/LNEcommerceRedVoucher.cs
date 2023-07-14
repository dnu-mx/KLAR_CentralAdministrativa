
using DALAutorizador.BaseDatos;
using DALAutorizador.Utilidades;
using DALCentralAplicaciones.Entidades;
using DALLealtad.BaseDatos;
using Interfases.Exceptiones;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DALLealtad.LogicaNegocio
{
    public class LNEcommerceRedVoucher
    {
        /// <summary>
        /// Establece las condiciones de validación para crear una nueva colectiva en el Autorizador,
        /// controlando la transacción (commit o rollback)
        /// </summary>
        /// <param name="idCliente">Identificador del Cliente</param>
        /// <param name="idSucursal">Identificador de la Sucursal</param>
        /// <param name="detallePedidos">Detalle de todos los pedidos a registrar. "[Cantidad] + ':' + [Monto] + '|' + ..."</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static DataTable CreaNuevoPedido(int idCliente, int idSucursal, string detallePedidos, Usuario elUsuario)
        {
            try
            {
                return DAOCertificadosAmazon.InsertaPedidoAmazon(idCliente, idSucursal, detallePedidos, elUsuario);
            }

            catch (CAppException caEx)
            {
                Loguear.Error(caEx, elUsuario.ClaveUsuario);
                throw caEx;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw ex;
            }
        }

    }
}
