using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlVentas.modelo.poco
{
    class Venta
    {
        private int idVenta;
        private int total;
        private DateTime fecha;
        private bool anulada;

        public int IdVenta
        {
            get { return idVenta; }
            set { idVenta = value; }
        }

        public int Total
        {
            get { return total; }
            set { total = value; }
        }
        
        public DateTime Fecha
        {
            get { return fecha; }
            set { fecha = value; }
        }

        public bool Anulada
        {
            get { return anulada; }
            set { anulada = value; }
        }
    }
}
