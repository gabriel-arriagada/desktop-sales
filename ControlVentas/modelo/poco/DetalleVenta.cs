using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlVentas.modelo.poco
{
    class DetalleVenta
    {
        private int idDetalleVenta;
        private int idVenta;
        private string idProducto;
        private string producto;
        private int precioVenta;
        private int cantidad;
        private int subTotal;
        private bool anulado;

        public int IdDetalleVenta
        {
            get { return idDetalleVenta; }
            set { idDetalleVenta = value; }
        }

        public int IdVenta
        {
            get { return idVenta; }
            set { idVenta = value; }
        }

        public string IdProducto
        {
            get { return idProducto; }
            set { idProducto = value; }
        }

        public int PrecioVenta
        {
            get { return precioVenta; }
            set { precioVenta = value; }
        }

        public int Cantidad
        {
            get { return cantidad; }
            set { cantidad = value; }
        }

        public int SubTotal
        {
            get { return subTotal; }
            set { subTotal = value; }
        }

        public bool Anulado
        {
            get { return anulado; }
            set { anulado = value; }
        }

        public string Producto
        {
            get { return producto; }
            set { producto = value; }
        }
    }
}
