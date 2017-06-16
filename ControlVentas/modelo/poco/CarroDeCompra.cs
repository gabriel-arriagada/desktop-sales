using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlVentas.modelo.poco
{
    class CarroDeCompra
    {
        private String nombre;

        public String Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }
        private String codigo;

        public String Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        }
        private int cantidad;

        public int Cantidad
        {
            get { return cantidad; }
            set { cantidad = value; }
        }
        private int precio;

        public int Precio
        {
            get { return precio; }
            set { precio = value; }
        }
        private int subTotal;

        public int SubTotal
        {
            get { return subTotal; }
            set { subTotal = value; }
        }
         
    }
}
