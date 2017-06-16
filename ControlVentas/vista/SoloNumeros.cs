using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlVentas.modelo
{
    class SoloNumeros
    {
        public void soloNumeros(TextCompositionEventArgs e) 
        {
            int ascci = Convert.ToInt32(Convert.ToChar(e.Text));
            if (ascci >= 48 && ascci <= 57)
                e.Handled = false;
            else
                e.Handled = true;
        }
    }
}
