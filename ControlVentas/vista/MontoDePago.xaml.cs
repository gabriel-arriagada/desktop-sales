using ControlVentas.modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ControlVentas.vista
{
    /// <summary>
    /// Lógica de interacción para MontoDePago.xaml
    /// </summary>
    public partial class MontoDePago : Window
    {
        private SoloNumeros soloNumeros;

        public MontoDePago()
        {
            InitializeComponent();
            soloNumeros = new SoloNumeros();
            textBoxMontoPago.Focus();
        }

        private void textBoxMontoPago_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            soloNumeros.soloNumeros(e);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void textBoxMontoPago_KeyUp(object sender, KeyEventArgs e)
        {
            if (!textBoxMontoPago.Text.Equals(""))
            {
                long resta = Convert.ToInt32(textBoxMontoPago.Text) - Convert.ToInt32(labelTotal.Content);
                if (resta > 0 && resta < 2147483647) 
                {
                    int vuelto = Convert.ToInt32(textBoxMontoPago.Text) - Convert.ToInt32(labelTotal.Content);
                    labelVuelto.Content = vuelto.ToString();
                }
            }
            else 
            {
                labelVuelto.Content = "0";
            }
        }


    }
}
