using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using System.Data;
using System.Windows;

namespace ControlVentas.modelo.dao
{
    class ConexionPostreSql //: AbstractConexion
    {

        private NpgsqlConnection npgsqlConnection;
        private string servidor;
        private string baseDeDatos;
        private string usuario;
        private string clave;

        public ConexionPostreSql() 
        {
            this.servidor = "Server=localhost;";
            this.baseDeDatos = "Database=controlventas;";
            this.usuario = "User id=postgres;";
            this.clave = "Password=super8;";
            npgsqlConnection = new NpgsqlConnection(servidor + baseDeDatos + usuario + clave);
        }

        public bool abrirConexion()
        {
            try
            {
                if (npgsqlConnection != null && npgsqlConnection.State == ConnectionState.Closed)
                {
                    npgsqlConnection.Open();
                }
                return true;
            }
            catch (NpgsqlException ex)
            {
                /*switch (ex.ErrorCode)
                {
                    case 0:
                        //MessageBox.Show("Imposible conectarse al servidor");
                        break;
                    case 1045:
                        //MessageBox.Show("Datos incorrectos");
                        break;
                }*/
                MessageBox.Show("Error de coexión con la base de datos:" + ex.ToString(),"Mensaje del sistema", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool cerrarConexion()
        {
            try
            {
                npgsqlConnection.Close();
                return true;
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.ToString(), "Mensaje del sistema", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public NpgsqlConnection retornarConexion()
        {
            return npgsqlConnection;
        }
    }
}
