using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace ControlVentas.vista
{
    class BuscarTextBoxVacios
    {
        private bool retorno;

        public bool buscarTextBoxVacios(TextBox[] textBoxes) 
        {
            retorno = false;
            for (int i = 0; i < textBoxes.Length; i++)
            {
                if (textBoxes[i].Text == "") 
                {
                    retorno = true;
                    break;
                }
            }
            return retorno;
        }
    }
}
