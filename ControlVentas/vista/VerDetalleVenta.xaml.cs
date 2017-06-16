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
    /// Lógica de interacción para DetalleVenta.xaml
    /// </summary>
    public partial class VerDetalleVenta : Window
    {
        private static readonly BackgroundWorker detalleVentaWorker = new BackgroundWorker();
        private ObservableCollection<DetalleVenta> detalleVentaSource;
        private int idVenta;
        private AbstractDao daoDetalleVenta;

        public VerDetalleVenta(int idVenta)
        {
            InitializeComponent();
            this.daoDetalleVenta = new DetalleVentaDao();
            this.idVenta = idVenta;
            gridDetalleVenta.Cursor = Cursors.Wait;
            detalleVentaWorker.DoWork += detalleVentaWorker_DoWork;
            detalleVentaWorker.RunWorkerCompleted += detalleVentaWorker_RunWorkerComplete;
            if (!detalleVentaWorker.IsBusy) 
            {
                detalleVentaWorker.RunWorkerAsync();
            }
        }

        private void detalleVentaWorker_RunWorkerComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            gridDetalleVenta.ItemsSource = detalleVentaSource;
            gridDetalleVenta.Cursor = Cursors.Arrow;
        }

        private void detalleVentaWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int[] arreglo = new int[3];
            arreglo[0] = 100;//limit
            arreglo[1] = 0;//offset
            arreglo[2] = this.idVenta;//idVenta
            detalleVentaSource = new ObservableCollection<DetalleVenta>(this.daoDetalleVenta.ver(arreglo, "").Cast<DetalleVenta>().ToList());
        }

        //private void botonAnularDetalle_Click(object sender, RoutedEventArgs e)
        //{
        //    this.daoDetalleVenta = new DetalleVentaDao();
        //    DetalleVenta detalle = ((FrameworkElement)sender).DataContext as DetalleVenta;
        //    MessageBoxResult result = MessageBox.Show("¿Estás seguro(a) de anular el detalle n°" + detalle.IdDetalleVenta + "?",
        //        "Mensaje del sistema", MessageBoxButton.YesNo, MessageBoxImage.Question);

        //    if (result == MessageBoxResult.Yes)
        //    {
        //        if (daoDetalleVenta.cambiarEstado(detalle.IdDetalleVenta.ToString(), true/*Anulada = true*/) > 1)
        //        {
        //            if (!detalleVentaWorker.IsBusy) 
        //            {
        //                detalleVentaWorker.RunWorkerAsync(""/*Buscar vacio*/);
        //                MessageBox.Show("El detalle n°" + detalle.IdVenta + " ha sido anulado.", "Mensaje del sistema", MessageBoxButton.OK, MessageBoxImage.Information);
        //            }
        //        }
        //        else
        //        {
        //            MessageBox.Show("No se ha podido completar la operación!", "Mensaje del sistema", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //    }
        //}


    }
}
