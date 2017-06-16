using ControlVentas.modelo;
using ControlVentas.modelo.dao;
using ControlVentas.modelo.poco;
using ControlVentas.vista;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ControlVentas
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private AbstractDao daoVenta;
        private AbstractDao daoCategoria;
        private AbstractDao daoProducto;
        private AbstractDao daoDetalleVenta;


        /*Menús abiertos*/
        private bool menuCategoriaAbierto = false;
        private bool menuProductoAbierto = false;
        private bool menuVentaAbierto = false;
        private bool menuInicioAbierto = true;



        /*Tablas cargadas*/
        private bool cargoCategorias = false;
        private bool cargoProductos = false;
        private bool cargoVentas = false;


        /*ARREGLOS para envíar las variable limit y offset para carga de datos*/
        int[] arregloLimitOffSetVentas = new int[2];
        int[] arregloLimitOffSetCategorias = new int[2];
        int[] arregloLimitOffSetProductos = new int[2];

        /*Trabajos en segundo plano*/
        private static readonly BackgroundWorker categoriaWorker = new BackgroundWorker();
        private ObservableCollection<Categoria> categoriaSource;

        private static readonly BackgroundWorker productoWorker = new BackgroundWorker();
        private ObservableCollection<Producto> productoSource;

        private static readonly BackgroundWorker ventaWorker = new BackgroundWorker();
        private ObservableCollection<Venta> ventaSource;

        private static readonly BackgroundWorker carroWorker = new BackgroundWorker();

        /*INICIO: Métodos para recargar Data grid desde otro formulario*/
        public static void ejecutarWorkerCategoria() 
        {
            if (!categoriaWorker.IsBusy) 
            {
                categoriaWorker.RunWorkerAsync(""/*Buscar vacio*/);
            }
        }

        public static void ejecutarWorkerProducto() 
        {
            if (!productoWorker.IsBusy) 
            {
                productoWorker.RunWorkerAsync(""/*Buscar vacio*/);
            }
        }

        public static void ejecutarWorkerVenta()
        {
            if (!ventaWorker.IsBusy) 
            {
                ventaWorker.RunWorkerAsync(""/*Buscar vacio*/);
            }
        } 
        /*FIN*/


        /*Lista Carro de compras*/
        List<CarroDeCompra> carroDeComprasList = new List<CarroDeCompra>();

        /*Timer para contar la intrducción de código*/
        DispatcherTimer dispatcherTimer;


        /*INICIO CONSTUCTOR*/
        public MainWindow()
        {
            InitializeComponent();
            /**Categoria*/
            categoriaWorker.DoWork += categoriaWorker_DoWork;
            categoriaWorker.RunWorkerCompleted += categoriaWorker_RunWorkerCompleted;

            /**Producto*/
            productoWorker.DoWork += productoWorker_DoWork;
            productoWorker.RunWorkerCompleted += productoWorker_RunWorkerCompleted;  

            /*Venta*/
            ventaWorker.DoWork += ventaWorker_DoWork;
            ventaWorker.RunWorkerCompleted += ventaWorker_RunWorkerComplete;


            /*Iniciar arreglo de variables limit y offset*/
            establecerArregloLimitOffsetVentas(10, 0);
            establecerArregloLimitOffsetCategorias(10, 0);
            establecerArregloLimitOfSettProductos(10, 0);

            botonCrear.Visibility = Visibility.Hidden;

            rescatarUltimaVenta();

            dispatcherTimer = new DispatcherTimer();

            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);  

        }

        /*FIN CONSTRUCTOR*/
      



        /*Metodos para actualizar valor de arreglos contenedores limit y offset*/
        public void establecerArregloLimitOffsetVentas(int limit, int offset) 
        {
            arregloLimitOffSetVentas[0] = limit;
            arregloLimitOffSetVentas[1] = offset;
        }

        public void establecerArregloLimitOfSettProductos(int limit, int offset) 
        {
            arregloLimitOffSetProductos[0] = limit;
            arregloLimitOffSetProductos[1] = offset;
        }


        public void establecerArregloLimitOffsetCategorias(int limit, int offset) 
        {
            arregloLimitOffSetCategorias[0] = limit;
            arregloLimitOffSetCategorias[1] = offset;
        }












        public void rescatarUltimaVenta() 
        {
            daoVenta = new VentaDao();
            Venta v = (Venta)daoVenta.buscar("");
            labelNumeroDeVenta.Content = "venta n° "+ (v.IdVenta + 1).ToString("D10");
        }









        /*INICIO: CARRO DE COMPRAS------------------------------------------------------------------------------------*/
        string codigoProducto = "";
        bool borrar = false;
        bool iniciarMultiplicarProducto = false;
        string numeroParaMultiplicar = "";
        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (checkComenzarToEscanear.IsChecked == true)
            {
                if (e.Key == Key.L) 
                {
                    codigoProducto = "";
                    textoCodigoBarras.Text = "";
                }
                else if (e.Key == Key.Delete)
                {
                    borrar = true;
                }
                else if (e.Key == Key.X)
                {
                    iniciarMultiplicarProducto = true;
                }
                else if (e.Key == Key.Return && iniciarMultiplicarProducto)
                {
                    /*En caso de que la persona no presione "enter" al multiplicar un número*/
                    if (!numeroParaMultiplicar.Equals("") && Convert.ToInt64(numeroParaMultiplicar) > 0 && Convert.ToInt64(numeroParaMultiplicar) < 5000)
                    {
                        multiplicarCantidadDeProductoCarroDeCompra(Convert.ToInt32(numeroParaMultiplicar));
                        cargarGridCarroDeCompraConIEnumerable();
                        calcularTotal();
                        iniciarMultiplicarProducto = false;
                        numeroParaMultiplicar = "";
                        codigoProducto = "";
                    }
                    else 
                    {
                        numeroParaMultiplicar = "";
                        iniciarMultiplicarProducto = false;
                    }
                }
                else if (e.Key >= Key.D0 && e.Key <= Key.D9)
                {
                    
                    if (iniciarMultiplicarProducto)
                    {
                        numeroParaMultiplicar += e.Key.ToString().Substring(1);           
                    }
                    else
                    {
                        string numero = e.Key.ToString().Substring(1);
                        codigoProducto += numero;

                        textoCodigoBarras.Text = codigoProducto;
                        dispatcherTimer.Start();
                    }
                }
                e.Handled = true;
            }
            else 
            {
                if (menuInicioAbierto)
                {
                    Mensaje mensaje = new Mensaje();
                    mensaje.Show();
                }
            }   
        }

        int contador = 0;
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (contador == 30)/*Cuando el contador llega a 30 mm, rescata el código de la caja de texto */
            {
                if (borrar)
                {
                    quitarProductoCarroDeCompras(textoCodigoBarras.Text);
                    cargarGridCarroDeCompraConIEnumerable();
                    calcularTotal();
                    borrar = false;
                }
                else
                {
                    agregarProductoToCarroDeCompra(textoCodigoBarras.Text);
                    cargarGridCarroDeCompraConIEnumerable();
                    calcularTotal();
                }
                esconderMostrarImagenBarCode();
                textoCodigoBarras.Text = "";
                contador = 0;
                codigoProducto = "";
                dispatcherTimer.Stop();
            }
            contador++;
        }









        public bool stockSuficiente(string codigoProducto, int cantidad) 
        {
            bool suficiente = false;
            Producto p = (Producto)daoProducto.buscar(codigoProducto);
            if (cantidad <= p.Stock)
            {
                suficiente = true;
            }
            else 
            {
                Mensaje mensaje = new Mensaje();
                mensaje.labelTexto.Content = "La cantidad seleccionada sobrepasa el stock \ndel producto " + p.Nombre + "."
                               + " El stock  máximo es de " + p.Stock + " unidades.";
                mensaje.Show();
            }
            return suficiente;
        }


        public void multiplicarCantidadDeProductoCarroDeCompra(int numeroParaMultiplicar) 
        {
            if (carroDeComprasList.Count > 0) 
            {
                CarroDeCompra carroDeCompra = carroDeComprasList.ElementAt(carroDeComprasList.Count - 1);
                if (stockSuficiente(carroDeCompra.Codigo, carroDeCompra.Cantidad * numeroParaMultiplicar)) 
                {
                    carroDeCompra.Cantidad = carroDeCompra.Cantidad * numeroParaMultiplicar;
                    carroDeCompra.SubTotal = carroDeCompra.Precio * carroDeCompra.Cantidad;
                }
            }
        }

        public void agregarProductoToCarroDeCompra(string codigo) 
        {
            bool repetido = false;
            if (carroDeComprasList.Count > 0)
            {
                foreach (var carro in carroDeComprasList)
                {
                    if (carro.Codigo == codigo)
                    {
                        repetido = true;
                        if (stockSuficiente(carro.Codigo, (1 + carro.Cantidad)))
                        {
                            carro.Cantidad = carro.Cantidad + 1;
                            carro.SubTotal = carro.Precio * carro.Cantidad;
                            gridCarroDeCompras.Items.Refresh();
                            break;
                        }
                        else 
                        {
                            break;
                        }
                    }
                }
            }
            if (repetido == false)
            {
                daoProducto = new ProductoDao();
                Producto p = (Producto)daoProducto.buscar(codigoProducto);
                if (p.Nombre != null)
                {
                    if (p.Stock >= 1)
                    {
                        carroDeComprasList.Add(
                        new CarroDeCompra
                        {
                            Nombre = p.Nombre,
                            Codigo = codigoProducto,
                            Cantidad = 1,
                            Precio = p.PrecioVenta,
                            SubTotal = (p.PrecioVenta * 1)
                        });
                    }
                    else 
                    {
                        Mensaje mensaje = new Mensaje();
                        mensaje.labelTexto.Content = "La cantidad seleccionada sobrepasa el stock \ndel producto " + p.Nombre + "."
                                       + " El stock  máximo es de " + p.Stock + " unidades.";
                        mensaje.Show();
                    }
                }
                else 
                {
                    Mensaje mensaje = new Mensaje();
                    mensaje.labelTexto.Content = "Este producto aún no ha sido creado en el sistema.";
                    mensaje.Show();
                }
            }
        }

        public void calcularTotal() 
        {
            int totalVenta = 0;
            foreach (var carro in carroDeComprasList)
            {
                totalVenta += carro.SubTotal;
            }
            labelTotalCarritoDeCompras.Content = totalVenta.ToString();
        }

        public void quitarProductoCarroDeCompras(string codigo) 
        {
            if (carroDeComprasList.Count > 0)
            {
                foreach (var carro in carroDeComprasList)
                {
                    if (carro.Codigo == codigo)
                    {
                        carro.Cantidad = carro.Cantidad - 1;
                        carro.SubTotal = carro.Precio * carro.Cantidad;
                        gridCarroDeCompras.Items.Refresh();
                        if (carro.Cantidad == 0) 
                        {
                            carroDeComprasList.Remove(carro);
                        }
                        break;
                    }
                }
            }
        }

        public void cargarGridCarroDeCompraConIEnumerable() 
        {
            IEnumerable<CarroDeCompra> carroIEnumerable = carroDeComprasList;
            gridCarroDeCompras.ItemsSource = null;
            gridCarroDeCompras.ItemsSource = carroIEnumerable;
        }

        public void esconderMostrarImagenBarCode()
        {
            if (gridCarroDeCompras.Items.Count > 0)
            {
                labelImagenBarCode.Visibility = Visibility.Hidden;
                labelTextoBarCode.Visibility = Visibility.Hidden;
            }
            else
            {
                labelImagenBarCode.Visibility = Visibility.Visible;
                labelTextoBarCode.Visibility = Visibility.Visible;
            }
        }

        public void reiniciarElemntosDeCarroDeCompras() 
        {
            carroDeComprasList.Clear();
            labelTotalCarritoDeCompras.Content = "0";
            cargarGridCarroDeCompraConIEnumerable();
            esconderMostrarImagenBarCode();
        }

        private void botonGuardarVenta_Click(object sender, RoutedEventArgs e)
        {
            if (gridCarroDeCompras.Items.Count > 0)
            {
                MontoDePago monto = new MontoDePago();
                monto.labelTotal.Content = labelTotalCarritoDeCompras.Content;
                monto.ShowDialog();

                if (monto.DialogResult.HasValue && monto.DialogResult.Value)
                {
                    daoVenta = new VentaDao();
                    Venta venta = new Venta();
                    
                    venta.Total = Convert.ToInt32(labelTotalCarritoDeCompras.Content);//monto total de la venta
                    venta.Fecha = DateTime.Now;
                    venta.Anulada = false;
                    int idVenta = 0;
                    idVenta = daoVenta.crear(venta);

                    if (idVenta > 0)
                    {
                        daoDetalleVenta = new DetalleVentaDao();
                        foreach (var carro in carroDeComprasList)
                        {
                            DetalleVenta detalleVenta = new DetalleVenta();
                            detalleVenta.IdVenta = idVenta;
                            detalleVenta.IdProducto = carro.Codigo;
                            detalleVenta.PrecioVenta = carro.Precio;
                            detalleVenta.Cantidad = carro.Cantidad;
                            detalleVenta.SubTotal = carro.SubTotal;
                            detalleVenta.Anulado = false;
                            if (daoDetalleVenta.crear(detalleVenta) != 2)
                            {
                                MessageBox.Show("No se ha podido completar la operación", "Mensaje del sistema", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;
                            }
                        }
                        reiniciarElemntosDeCarroDeCompras();
                        cargoProductos = false;
                        cargoVentas = false;
                        rescatarUltimaVenta();
                        MessageBox.Show("¡La venta fue guardada en el sistema!", "Mensaje del sistema", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("No se ha podido completar la operación", "Mensaje del sistema", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else 
                {
                    MessageBox.Show("La venta ha sido cancelada.", "Mensaje del sistema", MessageBoxButton.OK, MessageBoxImage.Error);
                }             
            }
            else
            {

                MessageBoxResult result = MessageBox.Show("Debe haber productos para guardar una nueva venta", "Mensaje del sistema", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                if (result == MessageBoxResult.OK) 
                {
                    checkComenzarToEscanear.Focus();
                }
            }  
        }

        private void inicio_GotFocus(object sender, RoutedEventArgs e)
        {
            menuInicioAbierto = true;
            menuProductoAbierto = false;
            menuCategoriaAbierto = false;
            menuVentaAbierto = false;
            botonCrear.Visibility = Visibility.Hidden;
            checkComenzarToEscanear.Focus();
        }

        private void eliminarFilaGridCarroDeCompras(object sender, RoutedEventArgs e)
        {          
            int indiceFila = gridCarroDeCompras.SelectedIndex;
            carroDeComprasList.RemoveAt(indiceFila);
            cargarGridCarroDeCompraConIEnumerable();
            calcularTotal();
            esconderMostrarImagenBarCode();
        }
        /**FIN: CARRO DE COMPRAS---------------------------------------------------------------------------------/

        
        
        
        
 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
        /*<----------------------------INICIO: CATEGORIAS----------------------------------------------->*/
        /**/

        private void cargarMasCategorias(object sender, MouseButtonEventArgs e)
        {
            string limit = ((ComboBoxItem)sender).Tag.ToString();
            if (limit.Equals("all"))
            {
                establecerArregloLimitOffsetCategorias(-1, 0);
            }
            else
            {
                establecerArregloLimitOffsetCategorias(gridCategoria.Items.Count + 10, 0);
            }
            if (!categoriaWorker.IsBusy)
            {
                gridCategoria.Cursor = Cursors.Wait;
                categoriaWorker.RunWorkerAsync(""/*Buscar vacio*/);
            }
        }

        private void botonBuscarCategoria_Click(object sender, RoutedEventArgs e)
        {
            if (!textBoxBuscarCategoria.Text.Equals("")) 
            {
                string buscar = textBoxBuscarCategoria.Text.Trim();
                if (!categoriaWorker.IsBusy) 
                {
                    gridCategoria.Cursor = Cursors.Wait;
                    categoriaWorker.RunWorkerAsync(buscar);
                }
            }
        }

        private void menuCategorias_GotFocus(object sender, RoutedEventArgs e)
        {
            botonCrear.Visibility = Visibility.Visible;
            menuCategoriaAbierto = true;
            menuProductoAbierto = false;
            menuInicioAbierto = false;
            menuVentaAbierto = false;
            if (!cargoCategorias && !categoriaWorker.IsBusy)
            {
                gridCategoria.Cursor = Cursors.Wait;
                categoriaWorker.RunWorkerAsync(""/*Buscar vacio*/);
                cargoCategorias = true;
            }
        }

        private void categoriaWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string buscar = e.Argument.ToString();
            daoCategoria = new CategoriaDao();  
            categoriaSource = new ObservableCollection<Categoria>(this.daoCategoria.ver(arregloLimitOffSetCategorias, buscar).Cast<Categoria>().ToList());
        }

        private void categoriaWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            gridCategoria.ItemsSource = categoriaSource;
            gridCategoria.Cursor = Cursors.Arrow;
        }

        private void eliminarCategoria_Click(object sender, RoutedEventArgs e)
        {
            daoCategoria = new CategoriaDao();  
            Categoria categoria = ((FrameworkElement)sender).DataContext as Categoria;
            MessageBoxResult result = MessageBox.Show("¿Estás seguro(a) de eliminar la categoría " + categoria.Nombre + "?",
                "Mensaje del sistema",MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes) 
            {
                if (daoCategoria.eliminar(categoria.IdCategoria.ToString()) == 1) 
                {
                    if (!categoriaWorker.IsBusy) 
                    {
                        categoriaWorker.RunWorkerAsync(""/*Buscar vacio*/);
                        MessageBox.Show("La categoría " + categoria.Nombre + " ha sido eliminada del sistema.", "Mensaje del sistema", MessageBoxButton.OK, MessageBoxImage.Information);
                    }                   
                }
                else
                {
                    MessageBox.Show("No se ha podido completar la operación!", "Mensaje del sistema", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void editarCategoria_Click(object sender, RoutedEventArgs e)
        {
            Categoria categoria = ((FrameworkElement)sender).DataContext as Categoria;
            EditarCategoria editarCategoria = new EditarCategoria();
            editarCategoria.textBoxIdCategoria.Text = categoria.IdCategoria.ToString();
            editarCategoria.textBoxNombre.Text = categoria.Nombre;
            editarCategoria.Show();
        }
        /**/
        /*<----------------------------FIN: CATEGORIAS------------------------------------------------------->*/

























        /*<-----------------------------------INICIO: PRODUCTOS------------------------------------------->*/
        /**/

        private void cargarMasProductos(object sender, MouseButtonEventArgs e)
        {
            string limit = ((ComboBoxItem)sender).Tag.ToString();
            if (limit.Equals("all"))
            {
                establecerArregloLimitOfSettProductos(-1, 0);
            }
            else
            {
                establecerArregloLimitOfSettProductos(gridProdcuto.Items.Count + 10, 0);
            }
            if (!productoWorker.IsBusy)
            {
                gridProdcuto.Cursor = Cursors.Wait;
                productoWorker.RunWorkerAsync(""/*Buscar vacio*/);
            }
        }

        private void botonBuscarProducto_Click(object sender, RoutedEventArgs e)
        {
            if (!textBoxBuscarProducto.Equals("") && !productoWorker.IsBusy) 
            {
                string buscar = textBoxBuscarProducto.Text.Trim();
                gridProdcuto.Cursor = Cursors.Wait;
                productoWorker.RunWorkerAsync(buscar);
            }
        }

        private void menuProductos_GotFocus(object sender, RoutedEventArgs e)
        {
            botonCrear.Visibility = Visibility.Visible;
            menuCategoriaAbierto = false;
            menuVentaAbierto = false;
            menuInicioAbierto = false;
            menuProductoAbierto = true;
            if (!cargoProductos && !productoWorker.IsBusy)
            {
                gridProdcuto.Cursor = Cursors.Wait;
                productoWorker.RunWorkerAsync(""/*Buscar vacio*/);
                cargoProductos = true;
            }
        }

        private void productoWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string buscar = e.Argument.ToString();
            daoProducto = new ProductoDao();
            productoSource = new ObservableCollection<Producto>(this.daoProducto.ver(arregloLimitOffSetProductos, buscar).Cast<Producto>().ToList());
        }

        private void productoWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            gridProdcuto.ItemsSource = productoSource;
            gridProdcuto.Cursor = Cursors.Arrow;
        }

        private void eliminarProducto_Click(object sender, RoutedEventArgs e)
        {
            daoProducto = new ProductoDao();
            Producto producto = ((FrameworkElement)sender).DataContext as Producto;
            MessageBoxResult result = MessageBox.Show("¿Estás seguro(a) de eliminar el producto " + producto.Nombre + "?",
                "Mensaje del sistema", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                if (daoProducto.eliminar(producto.IdProducto) == 1)
                {
                    if (!productoWorker.IsBusy) 
                    {
                        productoWorker.RunWorkerAsync(""/*Buscar Vacio*/);
                        MessageBox.Show("El producto " + producto.Nombre + " ha sido eliminado del sistema.", "Mensaje del sistema", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("No se ha podido completar la operación!", "Mensaje del sistema", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void editarProducto_Click(object sender, RoutedEventArgs e)
        {
            Producto producto = ((FrameworkElement)sender).DataContext as Producto;
            EditarProducto editarProducto = new EditarProducto();
            editarProducto.textBoxIdProducto.Text = producto.IdProducto.ToString();
            editarProducto.textBoxNombre.Text = producto.Nombre;
            editarProducto.textBoxDescripcion.Text = producto.Descripcion;
            editarProducto.textBoxStock.Text = producto.Stock.ToString();
            editarProducto.textBoxPrecioCompra.Text = producto.PrecioCompra.ToString();
            editarProducto.textBoxPrecioVenta.Text = producto.PrecioVenta.ToString();
            Categoria cat = new Categoria();
            cat.Nombre = producto.Categoria;
            editarProducto.comboBoxIdCategoria.SelectedValue = cat.Nombre;
            editarProducto.Show();
        }
        /**/
        /*<-----------------------------------FIN: PRODUCTOS------------------------------------------------------>*/




























        /**<------------------------------INICIO: Ventas ------------------------------>*/

        private void cargarMasVentas(object sender, MouseButtonEventArgs e)
        {
            string limit = ((ComboBoxItem)sender).Tag.ToString();
            if (limit.Equals("all"))
            {
                establecerArregloLimitOffsetVentas(-1, 0);
            }
            else
            {
                establecerArregloLimitOffsetVentas(gridVenta.Items.Count + 10, 0);
            }
            if (!ventaWorker.IsBusy)
            {
                ventaWorker.RunWorkerAsync(""/*Buscar vacio*/);
            }
        }

        private void botonBuscarVenta_Click(object sender, RoutedEventArgs e)
        {
            if (!textBoxBuscarVenta.Equals("")) 
            {
                string buscar = textBoxBuscarVenta.Text.Trim();
                gridVenta.Cursor = Cursors.Wait;
                ventaWorker.RunWorkerAsync(buscar);
            }
        }

        private void menuVentas_GotFocus(object sender, RoutedEventArgs e)
        {
            botonCrear.Visibility = Visibility.Hidden;
            menuCategoriaAbierto = false;
            menuProductoAbierto = false;
            menuInicioAbierto = false;
            menuVentaAbierto = true;
            if (!cargoVentas && !ventaWorker.IsBusy)
            {
                gridVenta.Cursor = Cursors.Wait;
                ventaWorker.RunWorkerAsync(""/*Buscar vacio*/);
                cargoVentas = true;
            }
        }

        private void eliminarVenta_Click(object sender, RoutedEventArgs e)
        {
            daoVenta = new VentaDao();
            Venta venta = ((FrameworkElement)sender).DataContext as Venta;
            MessageBoxResult result = MessageBox.Show("¿Estás seguro(a) de anular la venta n°" + venta.IdVenta + "?",
                "Mensaje del sistema", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                if (daoVenta.cambiarEstado(venta.IdVenta.ToString(), true/*Anulada = true*/) == 1)
                {
                    if (!ventaWorker.IsBusy && !productoWorker.IsBusy) 
                    {
                        ventaWorker.RunWorkerAsync(""/*Buscar vacio*/);
                        productoWorker.RunWorkerAsync(""/*Buscar vacio*/);
                        MessageBox.Show("La venta n°" + venta.IdVenta + " ha sido anulada.", "Mensaje del sistema", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else 
                {
                    MessageBox.Show("No se ha podido completar la operación!", "Mensaje del sistema", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void verDetalleVenta_Click(object sender, RoutedEventArgs e)
        {
            Venta venta = ((FrameworkElement)sender).DataContext as Venta;
            VerDetalleVenta verDetalleDeVenta = new VerDetalleVenta(venta.IdVenta);
            verDetalleDeVenta.labelTitulo.Content = "detalle de venta n° " + venta.IdVenta.ToString("D10");
            verDetalleDeVenta.Show();
        }

        private void ventaWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string buscar = e.Argument.ToString();
            daoVenta = new VentaDao();
            ventaSource = new ObservableCollection<Venta>(this.daoVenta.ver(arregloLimitOffSetVentas, buscar).Cast<Venta>().ToList());
        }

        private void ventaWorker_RunWorkerComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            gridVenta.ItemsSource = ventaSource;
            gridVenta.Cursor = Cursors.Arrow;
        }
         
        /*<-------------------------FIN: VENTAS-------------------------------------->*/






        /*INICIO: Abrir formulario de creación de datos correspondiente según menú seleccionado*/
        private void botonCrear_Click(object sender, RoutedEventArgs e)
        {
            if (menuCategoriaAbierto)
            {
                CrearCategoria crearCategoria = new CrearCategoria();
                crearCategoria.Show();
            }
            else if(menuProductoAbierto)
            {
                CrearProducto crearProducto = new CrearProducto();
                crearProducto.Show();
            }
        }

      
        /**FIN**/






        private void botonAbrirManual_Click(object sender, RoutedEventArgs e)
        {
           
        }


        /*Acciones para controlar escaneo de productos*/
        private void checkComenzarToEscanear_Click(object sender, RoutedEventArgs e)
        {
            switch(checkComenzarToEscanear.IsChecked)
            {
                case true:
                    menuProductos.Visibility = Visibility.Hidden;
                    menuVentas.Visibility = Visibility.Hidden;
                    menuCategorias.Visibility = Visibility.Hidden;
                    checkComenzarToEscanear.Content = "Click aquí para ver el menú.";
                    break;
                case false:
                    MessageBoxResult result = MessageBox.Show("Si desactiva esta función, la venta comenzará desde cero.", "Mensaje del sistema", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.OK)
                    {
                        carroDeComprasList.Clear();
                        labelTotalCarritoDeCompras.Content = "0";
                        cargarGridCarroDeCompraConIEnumerable();
                        esconderMostrarImagenBarCode();
                        menuProductos.Visibility = Visibility.Visible;
                        menuVentas.Visibility = Visibility.Visible;
                        menuCategorias.Visibility = Visibility.Visible;
                        checkComenzarToEscanear.Content = "Click aquí para escanear productos.";
                    }
                    else 
                    {
                        checkComenzarToEscanear.IsChecked = true;
                    }
                    
                    break;
            }
        }

        

        /*FIN ACCIONES*/

    }
}
