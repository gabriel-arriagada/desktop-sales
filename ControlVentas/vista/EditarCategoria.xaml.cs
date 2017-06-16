using ControlVentas.modelo.dao;
using ControlVentas.modelo.poco;
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
    /// Lógica de interacción para EditarCategoria.xaml
    /// </summary>
    public partial class EditarCategoria : Window
    {
        private AbstractDao daoCategoria;

        public EditarCategoria()
        {
            InitializeComponent();
            daoCategoria = new CategoriaDao();
        }

        private void botonGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!textBoxNombre.Text.Equals(""))
            {
                Categoria categoria = new Categoria();
                categoria.IdCategoria = Convert.ToInt32(textBoxIdCategoria.Text);
                categoria.Nombre = textBoxNombre.Text;
                if (daoCategoria.editar(categoria) == 1)
                {
                    MainWindow.ejecutarWorkerCategoria();
                    MessageBox.Show("¡La categoría fue editada en el sistema!", "Mensaje del sistema", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("No se ha podido completar la operación!", "Mensaje del sistema", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Debes ingresar un nombre para la categoría", "Mensaje del sistema", MessageBoxButton.OK, MessageBoxImage.Information);
                textBoxNombre.Focus();
            }
        }

    }
}
