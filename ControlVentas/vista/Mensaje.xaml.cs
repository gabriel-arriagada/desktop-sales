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
    /// Lógica de interacción para Mensaje.xaml
    /// </summary>
    public partial class Mensaje : Window
    {
        public Mensaje()
        {
            InitializeComponent();
            this.Topmost = true;
            System.Media.SystemSounds.Beep.Play();
        }

        private void botonOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
