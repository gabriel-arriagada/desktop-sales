using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlVentas.modelo.poco
{
    class Producto
    {
        /*POCO: Plain Old CLR(Common Language Runtime) Objetc*/
        private string idProducto;
        private int idCategoria;
        private string categoria;
        private String nombre;
        private String descripcion;
        private int stock;
        private int precioCompra;
        private int precioVenta;

        public string IdProducto
        {
            get { return idProducto; }
            set { idProducto = value; }
        }

        public int IdCategoria
        {
            get { return idCategoria; }
            set { idCategoria = value; }
        }


        public string Categoria
        {
            get { return categoria; }
            set { categoria = value; }
        }

        public String Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }
       
        public String Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; }
        }
        
        public int Stock
        {
            get { return stock; }
            set { stock = value; }
        }

        public int PrecioCompra
        {
            get { return precioCompra; }
            set { precioCompra = value; }
        }

        public int PrecioVenta
        {
            get { return precioVenta; }
            set { precioVenta = value; }
        }

    }
}
