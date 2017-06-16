using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlVentas.modelo.poco
{
    class Categoria
    {
        private int idCategoria;
        private string nombre;

        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }

        public int IdCategoria
        {
            get { return idCategoria; }
            set { idCategoria = value; }
        }
    }
}
