using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SnelleWiel.Classes;
using System.Collections.ObjectModel;
using Windows.UI.Popups;
using Windows.UI.Core;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SnelleWiel.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OrderPage : Page
    {
        Frame frame { get; set; }
        dynamic Orders;
        ObservableCollection<Order> orderslist = new ObservableCollection<Order>();
        string sDatum;
        public OrderPage()
        {
            this.InitializeComponent();
            sDatum = dpDatumPicker.Date.ToString("dd-MM-yyyy");
            GetOrderOnDateAsync(sDatum);

        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            frame = e.Parameter as Frame;
        }

        //Zorgen dat het user ID gebruikt wordt voor het ophalen van de orders
        private async void GetOrderOnDateAsync(string Date)
        {
            orderslist.Clear();
            //User ID aanpassen
            Orders = await UrlCalls.GetOrders("4", Date);
            if (Orders.Count > 0)
            {
                orderslist.Clear();
                foreach (var Order in Orders)
                {
                    orderslist.Add(new Order
                    {
                        id = (string)Order.id,
                        order = (string)Order.order,
                        chauffeur = (string)Order.chauffeur,
                        date = (string)Order.date
                    });
                }
                lvOrders.ItemsSource = orderslist;
            }
            else
            {
                orderslist.Clear();
                MessageBox("Er zijn geen orders op de geselecteerde dag");
            }
            

        }

        private void dpDatumPicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            sDatum = dpDatumPicker.Date.ToString("dd-MM-yyyy");
            GetOrderOnDateAsync(sDatum);
        }

        private async void MessageBox(string message)
        {
            var dialog = new MessageDialog(message.ToString());
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await dialog.ShowAsync());
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Grid send = sender as Grid;
            string order = send.Tag.ToString();
           
            frame.Navigate(typeof(AppPage), new Tuple<Frame, string>(frame, order));
        }
    }
}
