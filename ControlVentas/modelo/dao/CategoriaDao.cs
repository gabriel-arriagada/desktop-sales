using ControlVentas.modelo.poco;
using Npgsql;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlVentas.modelo.dao
{
    class CategoriaDao : AbstractDao
    {

        private ConexionPostreSql conexionPostgreSql;
        private int filasAfectadas;

        public CategoriaDao() 
        {
            this.conexionPostgreSql = new ConexionPostreSql();
        }

        public override int crear(object o)
        {
            Categoria categoria = (Categoria)o;
            this.filasAfectadas = 0;
            try 
            { 
                if(conexionPostgreSql.abrirConexion())
                {

                    string consulta = "INSERT INTO categoria(nombre) VALUES(@nombre)";
                    NpgsqlCommand command = new NpgsqlCommand(consulta, conexionPostgreSql.retornarConexion());
                    command.Parameters.Add("@nombre", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
                    command.Parameters[0].Value = categoria.Nombre;
                    this.filasAfectadas = command.ExecuteNonQuery();
                    conexionPostgreSql.cerrarConexion();
                }
            }
            catch(NpgsqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
           return this.filasAfectadas;
        }

        public override int editar(object o)
        {
            Categoria categoria = (Categoria)o;
            this.filasAfectadas = 0;
            try
            {
                if (conexionPostgreSql.abrirConexion())
                {
                    string consulta = "UPDATE categoria set nombre = @nombre WHERE idcategoria = @idCategoria";
                    NpgsqlCommand command = new NpgsqlCommand(consulta, conexionPostgreSql.retornarConexion());
                    command.Parameters.Add("@nombre", NpgsqlTypes.NpgsqlDbType.Varchar, 50);
                    command.Parameters.Add("@idCategoria", NpgsqlTypes.NpgsqlDbType.Integer);
                    command.Parameters[0].Value = categoria.Nombre;
                    command.Parameters[1].Value = categoria.IdCategoria;
                    this.filasAfectadas = command.ExecuteNonQuery();
                    conexionPostgreSql.cerrarConexion();
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return this.filasAfectadas;
        }

        public override int cambiarEstado(string id, bool vigente)
        {
            throw new NotImplementedException();
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

            ArrayList categorias = new ArrayList();
            try 
            {
                if(conexionPostgreSql.abrirConexion())
                {
                    string consulta = "SELECT idcategoria, nombre FROM categoria WHERE nombre LIKE '%" + buscar + "%' order by idcategoria LIMIT " + finalLimit + " OFFSET " + offset;
                    NpgsqlCommand command = new NpgsqlCommand(consulta, conexionPostgreSql.retornarConexion());
                    NpgsqlDataReader dataReader = command.ExecuteReader();
                    while (dataReader.Read()) 
                    {
                        categorias.Add(
                            new Categoria 
                            { 
                                IdCategoria = Convert.ToInt32(dataReader[0]),
                                Nombre = dataReader[1].ToString()
                            });
                    }
                    conexionPostgreSql.cerrarConexion();
                }
            }
            catch(NpgsqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return categorias;
        }

        public override object buscar(string id)
        {
            throw new NotImplementedException();
        }

        public override int eliminar(string id)
        {
            this.filasAfectadas = 0;
            try
            {
                if (conexionPostgreSql.abrirConexion())
                {
                    string consultaProducto = "SELECT idcategoria FROM  producto WHERE producto.idcategoria = @IdCategoria";
                    NpgsqlCommand commandProducto = new NpgsqlCommand(consultaProducto, conexionPostgreSql.retornarConexion());
                    commandProducto.Parameters.Add("@IdCategoria", NpgsqlTypes.NpgsqlDbType.Integer);
                    commandProducto.Parameters[0].Value = Convert.ToInt32(id);
                    NpgsqlDataReader reader = commandProducto.ExecuteReader();

                    if (reader.HasRows == false)
                    {
                        string consulta = "DELETE FROM categoria WHERE idcategoria = @idCategoria";
                        NpgsqlCommand command = new NpgsqlCommand(consulta, conexionPostgreSql.retornarConexion());
                        command.Parameters.Add("@idCategoria", NpgsqlTypes.NpgsqlDbType.Integer);
                        command.Parameters[0].Value = Convert.ToInt32(id);
                        this.filasAfectadas = command.ExecuteNonQuery();                     
                    }
                    else 
                    {
                        this.filasAfectadas = 0;
                    }

                    conexionPostgreSql.cerrarConexion();
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return this.filasAfectadas;
           
        }
    }
}
