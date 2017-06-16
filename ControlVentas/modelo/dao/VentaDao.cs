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
    class VentaDao : AbstractDao
    {
        private ConexionPostreSql conexionPostgreSql;
        private int filasAfectadas;

        public VentaDao() 
        {
            this.conexionPostgreSql = new ConexionPostreSql();
        }


        public override int crear(object o)
        {
            Venta venta = (Venta)o;
            int lastIdVenta = 0;
            try
            {
                if (conexionPostgreSql.abrirConexion())
                {

                    string consulta = "INSERT INTO venta(total, fecha, anulada) VALUES(@Total, @Fecha, @Anulada) RETURNING idventa";
                    NpgsqlCommand command = new NpgsqlCommand(consulta, conexionPostgreSql.retornarConexion());
                    command.Parameters.Add("@Total", NpgsqlTypes.NpgsqlDbType.Integer);
                    command.Parameters.Add("@Fecha", NpgsqlTypes.NpgsqlDbType.Timestamp);
                    command.Parameters.Add("@Anulada", NpgsqlTypes.NpgsqlDbType.Boolean);
                    command.Parameters[0].Value = venta.Total;
                    command.Parameters[1].Value = venta.Fecha;
                    command.Parameters[2].Value = venta.Anulada;
                    lastIdVenta = Convert.ToInt32(command.ExecuteScalar());
                    conexionPostgreSql.cerrarConexion();
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return lastIdVenta;
        }

        public override int editar(object o)
        {
            throw new NotImplementedException();
        }

        public override int cambiarEstado(string id, bool anulado)
        {
            anulado = true;
            bool anuloVenta = false;
            bool anuloDetalle = false;
            bool devolvioStock = false;

            this.filasAfectadas = 0;

            if (conexionPostgreSql.abrirConexion())
            {
                NpgsqlTransaction anularVentaTransaction = null;
                List<DetalleVenta> detallesQueContieneProductos = new List<DetalleVenta>();

                try
                {

                    anularVentaTransaction = conexionPostgreSql.retornarConexion().BeginTransaction();
                    anularVentaTransaction.Save("antesDeAnularVenta");

                    try
                    {
                        string consultaVenta = "UPDATE venta set anulada = @Anulada WHERE idventa = @IdVenta";
                        NpgsqlCommand commandVenta = new NpgsqlCommand(consultaVenta, conexionPostgreSql.retornarConexion(), anularVentaTransaction);
                        commandVenta.Parameters.Add("@Anulada", NpgsqlTypes.NpgsqlDbType.Boolean);
                        commandVenta.Parameters.Add("@IdVenta", NpgsqlTypes.NpgsqlDbType.Integer);
                        commandVenta.Parameters[0].Value = anulado;
                        commandVenta.Parameters[1].Value = Convert.ToInt32(id);
                        if (commandVenta.ExecuteNonQuery() == 1)
                        {
                            anuloVenta = true;
                        }
                    }
                    catch (NpgsqlException ex)
                    {
                        anuloVenta = false;
                        MessageBox.Show(ex.ToString());
                        anularVentaTransaction.Rollback("antesDeAnularVenta");
                    }

                    try
                    {
                        string consultaDetalle = "UPDATE detalleventa set anulado = @Anulado WHERE idventa = @IdVenta";
                        NpgsqlCommand commandDetalle = new NpgsqlCommand(consultaDetalle, conexionPostgreSql.retornarConexion());
                        commandDetalle.Parameters.Add("@Anulado", NpgsqlTypes.NpgsqlDbType.Boolean);
                        commandDetalle.Parameters.Add("@IdVenta", NpgsqlTypes.NpgsqlDbType.Integer);
                        commandDetalle.Parameters[0].Value = anulado;
                        commandDetalle.Parameters[1].Value = Convert.ToInt32(id);
                        if (commandDetalle.ExecuteNonQuery() >= 1)
                        {
                            anuloDetalle = true;
                        }
                    }
                    catch (NpgsqlException ex)
                    {
                        anuloDetalle = false;
                        MessageBox.Show(ex.ToString());
                        anularVentaTransaction.Rollback("antesDeAnularVenta");
                    }

                    try
                    {
                        string consultaDetallePorProductos = "SELECT idproducto, cantidad FROM detalleventa WHERE idventa = @IdVenta";
                        NpgsqlCommand commandBuscarDetalles = new NpgsqlCommand(consultaDetallePorProductos, conexionPostgreSql.retornarConexion());
                        commandBuscarDetalles.Parameters.Add("@IdVenta", NpgsqlTypes.NpgsqlDbType.Integer);
                        commandBuscarDetalles.Parameters[0].Value = Convert.ToInt32(id);
                        NpgsqlDataReader dataReader = commandBuscarDetalles.ExecuteReader();
                        while (dataReader.Read())
                        {
                            detallesQueContieneProductos.Add(
                                new DetalleVenta
                                {
                                    IdProducto = dataReader[0].ToString(),
                                    Cantidad = Convert.ToInt32(dataReader[1])
                                });
                        }
                    }
                    catch (NpgsqlException ex)
                    {
                        MessageBox.Show(ex.ToString());
                        anularVentaTransaction.Rollback("antesDeAnularVenta");
                    }


                    try
                    {
                        foreach (var item in detallesQueContieneProductos)
                        {
                            string consultaProductos = "UPDATE producto SET stock = stock + " + item.Cantidad + " WHERE idproducto = '" + item.IdProducto + "'";
                            NpgsqlCommand commandProductos = new NpgsqlCommand(consultaProductos, conexionPostgreSql.retornarConexion());
                            if (commandProductos.ExecuteNonQuery() >= 1)
                            {
                                devolvioStock = true;
                            }
                        }
                    }
                    catch (NpgsqlException ex)
                    {
                        devolvioStock = false;
                        MessageBox.Show(ex.ToString());
                        anularVentaTransaction.Rollback("antesDeAnularVenta");
                    }

                    anularVentaTransaction.Commit();

                }
                catch (NpgsqlException ex)
                {
                    MessageBox.Show(ex.ToString());
                    anularVentaTransaction.Rollback();
                }
                finally 
                {
                    conexionPostgreSql.cerrarConexion();
                }
            }

            if (anuloVenta && anuloDetalle && devolvioStock) 
            {
                this.filasAfectadas = 1;
            }

            return this.filasAfectadas;
        }

        public override ArrayList ver(int[] arreglo, string buscar)
        {
            int limit = arreglo[0];
            int offset = arreglo[1];

            string finalLimit = "";

            if (limit == -1)
            {
                finalLimit = "All";
            }
            else 
            {
                finalLimit = limit.ToString();
            }

            ArrayList ventas = new ArrayList();
            try
            {
                if (conexionPostgreSql.abrirConexion())
                {
                    string consulta = "SELECT idventa, total, fecha FROM venta WHERE anulada = 'false' AND (CAST(idventa AS TEXT) LIKE '%" + buscar + "%'"
                        + " OR CAST(total AS TEXT) LIKE '%" + buscar + "%' OR CAST(to_char(fecha, 'DD-MM-YYYY hh24:MM:ss') AS TEXT) LIKE '%" + buscar + "%') order by idventa DESC LIMIT " + finalLimit + " OFFSET " + offset;
                    NpgsqlCommand command = new NpgsqlCommand(consulta, conexionPostgreSql.retornarConexion());
                    NpgsqlDataReader dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        ventas.Add(
                            new Venta
                            {
                                IdVenta = Convert.ToInt32(dataReader[0]),
                                Total = Convert.ToInt32(dataReader[1]),
                                Fecha = (DateTime)dataReader[2]
                            });
                    }
                    conexionPostgreSql.cerrarConexion();
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return ventas;
        }

        public override object buscar(string id)
        {
            Venta venta = new Venta();
            if (conexionPostgreSql.abrirConexion())
            {
                //string consulta = "SELECT last_value FROM venta_idventa_seq";
                string consulta = "SELECT COUNT(idventa) FROM venta";
                NpgsqlCommand command = new NpgsqlCommand(consulta, conexionPostgreSql.retornarConexion());
                venta.IdVenta = Convert.ToInt32(command.ExecuteScalar());
                conexionPostgreSql.cerrarConexion();
            }
            return venta;
        }

        public override int eliminar(string id)
        {
            throw new NotImplementedException();
        }
    }
}
