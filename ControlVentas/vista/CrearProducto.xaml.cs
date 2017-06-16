using ControlVentas.modelo;
using ControlVentas.modelo.dao;
using ControlVentas.modelo.poco;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Lógica de interacción para CrearCategoria.xaml
    /// </summary>
    public partial class CrearProducto : Window
    {

        //Máximo int32 2.147.483.647

        private AbstractDao daoCategoria;
        private AbstractDao daoProducto;
        private TextBox[] cajasDeTexto;
        private BuscarTextBoxVacios buscarCajasDeTextoVacias;
        private bool comproboLaExistenciaDeProducto;
        private static readonly BackgroundWorker categoriaWorker = new BackgroundWorker();
        private ObservableCollection<Categoria> categoriaSource;
        private SoloNumeros soloNumeros;

        public CrearProducto()
        {
            InitializeComponent();    
            comproboLaExistenciaDeProducto = false;
            cajasDeTexto = new TextBox[6];
            cargarCajasDeTexto();
            buscarCajasDeTextoVacias = new BuscarTextBoxVacios();
            agregarEventosCajas_ComboBox();
            daoProducto = new ProductoDao();
            daoCategoria = new CategoriaDao();
            categoriaWorker.DoWork += categoriaWorker_DoWork;
            categoriaWorker.RunWorkerCompleted += categoriaWorker_RunWorkerCompleted;
            categoriaWorker.RunWorkerAsync(""/*Buscar vacio*/);
            soloNumeros = new SoloNumeros();
        }

        private void botonGuardar_Click(object sender, RoutedEventArgs e)
        {
            
            if (!buscarCajasDeTextoVacias.buscarTextBoxVacios(cajasDeTexto) && comboBoxIdCategoria.SelectedIndex != -1)
            {
                Categoria c = (Categoria)comboBoxIdCategoria.SelectedItem;
                Producto producto = new Producto();

                producto.IdProducto = textBoxIdProducto.Text;
                producto.IdCategoria = c.IdCategoria;
                producto.Nombre = textBoxNombre.Text;
                producto.Descripcion = textBoxDescripcion.Text;
                producto.Stock = Convert.ToInt32(textBoxStock.Text);
                producto.PrecioCompra = Convert.ToInt32(textBoxPrecioCompra.Text);
                producto.PrecioVenta = Convert.ToInt32(textBoxPrecioVenta.Text);

                if (daoProducto.crear(producto) == 1) 
                {
                    MainWindow.ejecutarWorkerProducto();
                    MessageBox.Show("¡El nuevo producto fue creado en el sistema!", "Mensaje del sistema", MessageBoxButton.OK, MessageBoxImage.Information);
                    limpiarCajasDeTexto();
                    textBoxIdProducto.Focus();
                }
                else
                {
                    MessageBox.Show("No se ha podido completar la operación!", "Mensaje del sistema", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else 
            {
                MessageBox.Show("Debes completar todos los campos antes de guardar.","Mensaje del sistema",MessageBoxButton.OK,MessageBoxImage.Exclamation);
                textBoxNombre.Focus();
            }
        }

        public void cargarCajasDeTexto()
        {
            cajasDeTexto[0] = textBoxIdProducto;
            cajasDeTexto[1] = textBoxNombre;
            cajasDeTexto[2] = textBoxDescripcion;
            cajasDeTexto[3] = textBoxStock;
            cajasDeTexto[4] = textBoxPrecioCompra;
            cajasDeTexto[5] = textBoxPrecioVenta;
        }

        public void limpiarCajasDeTexto() 
        {
            for (int i = 0; i < cajasDeTexto.Length; i++)
            {
                cajasDeTexto[i].Text = "";
            }
            comboBoxIdCategoria.SelectedIndex = -1;
        }

        public void agregarEventosCajas_ComboBox()
        {
            for (int i = 0; i < cajasDeTexto.Length; i++)
            {
                if (i == 0 || i > 2) 
                {
                    cajasDeTexto[i].PreviewTextInput += cajaDeTexto_PreviewTextInput;
                }
                if (i > 0) 
                {
                    cajasDeTexto[i].GotFocus += cajasDeTexto_GotFocus;
                }
            }
            comboBoxIdCategoria.GotFocus += cajasDeTexto_GotFocus;
        }

        //solo números
        private void cajaDeTexto_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            soloNumeros.soloNumeros(e);
        }

        private void cajasDeTexto_GotFocus(object sender, RoutedEventArgs e)
        {
            if(textBoxIdProducto.Text != "" && comproboLaExistenciaDeProducto == false)
            {
                Producto producto = (Producto)this.daoProducto.buscar(textBoxIdProducto.Text.ToString().Trim());
                if (producto.Nombre != null)
                {
                    MessageBox.Show("El producto de nombre '"+ producto.Nombre +"' ya se encuentra registrado en el sistema!", "Mensaje del sistema", MessageBoxButton.OK, MessageBoxImage.Warning);
                    textBoxIdProducto.Text = "";
                    textBoxIdProducto.Focus();
                }
                else 
                {
                    comproboLaExistenciaDeProducto = true;
                }
            }
        }

        private void textBoxIdProducto_KeyUp(object sender, KeyEventArgs e)
        {
            if (textBoxIdProducto.Text == "") 
            {
                comproboLaExistenciaDeProducto = false;
            }
        }

        private void categoriaWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int[] arreglo = new int[2];
            arreglo[0] = 100;//limit
            arreglo[1] = 0;//offset
            string buscar = e.Argument.ToString();
            categoriaSource = new ObservableCollection<Categoria>(this.daoCategoria.ver(arreglo, buscar).Cast<Categoria>().ToList());
        }

        private void categoriaWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            comboBoxIdCategoria.ItemsSource = categoriaSource;
        }



    }
}
