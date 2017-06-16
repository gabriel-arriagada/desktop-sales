using ControlVentas.modelo.poco;
using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlVentas.modelo.dao
{
    class DetalleVentaDao : AbstractDao
    {

        private ConexionPostreSql conexionPostgreSql;
        private int filasAfectadas;

        public DetalleVentaDao() 
        {
            this.conexionPostgreSql = new ConexionPostreSql();
        }

        public override int crear(object o)
        {
            DetalleVenta detalleVenta = (DetalleVenta)o;
            this.filasAfectadas = 0;
            try
            {
                if (conexionPostgreSql.abrirConexion())
                {

                    string consulta = "INSERT INTO detalleventa(idventa, idproducto, precioventa, cantidad, subtotal, anulado) "
                    +"VALUES(@IdVenta, @IdProducto, @PrecioVenta, @Cantidad, @SubTotal, @Anulado)";
                    NpgsqlCommand command = new NpgsqlCommand(consulta, conexionPostgreSql.retornarConexion());
                    command.Parameters.Add("@IdVenta", NpgsqlTypes.NpgsqlDbType.Integer);
                    command.Parameters.Add("@IdProducto", NpgsqlTypes.NpgsqlDbType.Varchar, 20);
                    command.Parameters.Add("@PrecioVenta", NpgsqlTypes.NpgsqlDbType.Integer);
                    command.Parameters.Add("@Cantidad", NpgsqlTypes.NpgsqlDbType.Integer);
                    command.Parameters.Add("@SubTotal", NpgsqlTypes.NpgsqlDbType.Integer);
                    command.Parameters.Add("@Anulado", NpgsqlTypes.NpgsqlDbType.Boolean);
                    command.Parameters[0].Value = detalleVenta.IdVenta;
                    command.Parameters[1].Value = detalleVenta.IdProducto;
                    command.Parameters[2].Value = detalleVenta.PrecioVenta;
                    command.Parameters[3].Value = detalleVenta.Cantidad;
                    command.Parameters[4].Value = detalleVenta.SubTotal;
                    command.Parameters[5].Value = detalleVenta.Anulado;
                    this.filasAfectadas = command.ExecuteNonQuery();

                    string consultaStock = "UPDATE producto SET stock = stock - " + detalleVenta.Cantidad + " WHERE idproducto = '" + detalleVenta.IdProducto + "'";
                    NpgsqlCommand commandStock = new NpgsqlCommand(consultaStock, conexionPostgreSql.retornarConexion());
                    this.filasAfectadas += commandStock.ExecuteNonQuery();
                    conexionPostgreSql.cerrarConexion();
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return this.filasAfectadas;
        }

        public override int editar(object o)
        {
            throw new NotImplementedException();
        }

        public override int cambiarEstado(string id, bool anulado)
        {
            this.filasAfectadas = 0;
            try
            {
                if (conexionPostgreSql.abrirConexion())
                {
                    string consultaDetalle = "UPDATE detalleventa set anulado = @Anulado WHERE idventa = @IdVenta";
                    NpgsqlCommand commandDetalle = new NpgsqlCommand(consultaDetalle, conexionPostgreSql.retornarConexion());
                    commandDetalle.Parameters.Add("@Anulado", NpgsqlTypes.NpgsqlDbType.Boolean);
                    commandDetalle.Parameters.Add("@IdVenta", NpgsqlTypes.NpgsqlDbType.Integer);
                    commandDetalle.Parameters[0].Value = anulado;
                    commandDetalle.Parameters[1].Value = Convert.ToInt32(id);
                    this.filasAfectadas += commandDetalle.ExecuteNonQuery();
                    conexionPostgreSql.cerrarConexion();
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return this.filasAfectadas;
        }

        public override ArrayList ver(int[] arreglo, string buscar)
        {
            int limit = arreglo[0];
            int offset = arreglo[1];
            int idVenta = arreglo[2];

            ArrayList detalles = new ArrayList();
            try
            {
                if (conexionPostgreSql.abrirConexion())
                {
                    conexionPostgreSql.retornarConexion().BeginTransaction();
                    string consulta = "SELECT d.idventa, d.iddetalleventa, p.nombre, d.precioventa, d.cantidad, d.subtotal FROM detalleventa d" 
                        +" INNER JOIN producto p ON d.idproducto = p.idproducto INNER JOIN venta v ON d.idventa = v.idventa" 
                        +" WHERE d.anulado = 'false' AND d.idVenta = " + idVenta + "order by d.iddetalleventa LIMIT " + limit + " OFFSET " + offset;
                    NpgsqlCommand command = new NpgsqlCommand(consulta, conexionPostgreSql.retornarConexion());
                    NpgsqlDataReader dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        detalles.Add(
                            new DetalleVenta
                            {
                                IdVenta = Convert.ToInt32(dataReader[0]),
                                IdDetalleVenta = Convert.ToInt32(dataReader[1]),
                                Producto = dataReader[2].ToString(),
                                PrecioVenta = Convert.ToInt32(dataReader[3]),
                                Cantidad = Convert.ToInt32(dataReader[4]),
                                SubTotal = Convert.ToInt32(dataReader[5]),
                            });
                    }
                    conexionPostgreSql.cerrarConexion();
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return detalles;
        }

        public override object buscar(string id)
        {
            throw new NotImplementedException();
        }

        public override int eliminar(string id)
        {
            throw new NotImplementedException();
        }

    }
}
