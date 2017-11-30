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
using Windows.UI.Xaml.Controls.Maps;
using Windows.Devices.Geolocation;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;
using SnelleWiel.Classes;
using System.Diagnostics;
using System.Collections.ObjectModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SnelleWiel.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppPage : Page
    {
        dynamic OphaalAdress;
        dynamic AfhaalAdress;
        dynamic Orders;
        ObservableCollection<Order> orderslist = new ObservableCollection<Order>();
        ObservableCollection<Afleveradres> orderAfleverAdres = new ObservableCollection<Afleveradres>();
        ObservableCollection<Ophaaladres> orderOphaalAdres = new ObservableCollection<Ophaaladres>();
        Geolocator geolocator;
        public AppPage()
        {
            this.InitializeComponent();
            GetOrders();
            MapControl1.Center = new Geopoint(new BasicGeoposition() { Latitude = 51.44083, Longitude = 5.47778 });
            MapControl1.ZoomLevel = 10;
            this.Loaded += MainPage_Loaded;
            lvOrderAfhaalAdres.Visibility = Visibility.Collapsed;
            lvOrderOphaalAdres.Visibility = Visibility.Collapsed;
        }
        // Function to get all the orders
        private async void  GetOrders()
        {
            
            Orders =  await UrlCalls.GetOrders("4", "30-11-2017");
            string test = Orders[0].id;
            //Debug.WriteLine(test);
            foreach (var Order in Orders)
            {
                orderslist.Add(new Order {
                    id = (string)Order.id,
                    order = (string)Order.order,
                    chauffeur = (string)Order.chauffeur,
                    date = (string)Order.date
                });
            }
            lvOrders.ItemsSource = orderslist;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Map Token for testing purpose,   
            // otherwise you'll get an alart message in Map Control  
            MapControl1.MapServiceToken = "oGdzgATPQ13YFGUucQSf~5pSwjiQ3EKxfiqyDUPfk4Q~Aphycm_cASc_MHTChozchWdk-qCFDOpfpvDf1O7BbuP2kgNF-XRlOy1kcaNu1G2S";

            geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;
            try
            {
                // Getting Current Location  
                Geoposition geoposition = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10));

                MapIcon mapIcon = new MapIcon();
                // Locate your MapIcon  
                mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/my-position.png"));
                // Show above the MapIcon  
                mapIcon.Title = "Uw huidige positie";
                // Setting up MapIcon location  
                mapIcon.Location = new Geopoint(new BasicGeoposition()
                {
                    //Latitude = geoposition.Coordinate.Latitude, [Don't use]  
                    //Longitude = geoposition.Coordinate.Longitude [Don't use]  
                    Latitude = geoposition.Coordinate.Point.Position.Latitude,
                    Longitude = geoposition.Coordinate.Point.Position.Longitude
                });
                // Positon of the MapIcon  
                mapIcon.NormalizedAnchorPoint = new Point(0.5, 0.5);
                MapControl1.MapElements.Add(mapIcon);
                // Showing in the Map  
                await MapControl1.TrySetViewAsync(mapIcon.Location, 18D, 0, 0, MapAnimationKind.Bow);

                // Disable the ProgreesBar  
                // LocateMe.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                // Set the Zoom Level of the Slider Control  
                mySlider.Value = MapControl1.ZoomLevel;
            }
            catch (UnauthorizedAccessException)
            {
                
            }
            //base.OnNavigatedTo(e);
        }

        // Relocate the value of the zoombar
        private void ZoomValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (MapControl1 != null)
                MapControl1.ZoomLevel = e.NewValue;
        }

        // Locate Me Bottom App Bar  
        private async void LocateMe_Click(object sender, RoutedEventArgs e)
        {
            LocateMe.Visibility = Windows.UI.Xaml.Visibility.Visible;
            geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;

            try
            {
                Geoposition geoposition = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10));
                await MapControl1.TrySetViewAsync(geoposition.Coordinate.Point, 18D);
                mySlider.Value = MapControl1.ZoomLevel;
                LocateMe.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox("Location service is turned off!");
            }
        }

        // Custom Message Dialog Box  
        private async void MessageBox(string message)
        {
            var dialog = new MessageDialog(message.ToString());
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await dialog.ShowAsync());
        }

        private void MapControl1_ZoomLevelChanged(MapControl sender, object args)
        {
            if (MapControl1 != null)
                mySlider.Value = sender.ZoomLevel;
        }

        // 
        private async void StackPanel_TappedAsync(object sender, TappedRoutedEventArgs e)
        {
            StackPanel send = sender as StackPanel;
            Order test = send.Tag as Order;
            var Order = await UrlCalls.GetOrder(test.id.ToString());

            AfhaalAdress = await UrlCalls.GetLangLong(Order.afleveradres[0].straat, Order.afleveradres[0].huisnr, Order.afleveradres[0].plaats, Order.afleveradres[0].postcode);
            MapIcon mapIconAflever = new MapIcon();
            // Locate your MapIcon  
            mapIconAflever.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/my-position.png"));
            // Show above the MapIcon  
            mapIconAflever.Title = "Afhaal Adres";
            // Setting up MapIcon location  
            try
            {
                mapIconAflever.Location = new Geopoint(new BasicGeoposition()
                {
                    Latitude = AfhaalAdress.results[0].geometry.location.lat,
                    Longitude = AfhaalAdress.results[0].geometry.location.lng
                });
            }
            catch
            {
                
            }
            // Positon of the MapIcon  
            mapIconAflever.NormalizedAnchorPoint = new Point(0.5, 0.5);
            MapControl1.MapElements.Add(mapIconAflever);

            OphaalAdress = await UrlCalls.GetLangLong(Order.ophaaladres[0].straat, Order.ophaaladres[0].huisnr, Order.ophaaladres[0].plaats, Order.ophaaladres[0].postcode);
            MapIcon mapIconOphaal = new MapIcon();
            // Locate your MapIcon  
            mapIconOphaal.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/my-position.png"));
            // Show above the MapIcon  
            mapIconOphaal.Title = "Ophaal Adres";
            // Setting up MapIcon location  
            try { 
            mapIconOphaal.Location = new Geopoint(new BasicGeoposition()
            { 
                Latitude = OphaalAdress.results[0].geometry.location.lat,
                Longitude = OphaalAdress.results[0].geometry.location.lng
            });
            }
            catch
            {
                
            }
            // Positon of the MapIcon  
            mapIconOphaal.NormalizedAnchorPoint = new Point(0.5, 0.5);
            MapControl1.MapElements.Add(mapIconOphaal);

            lvOrders.Visibility = Visibility.Collapsed;

            lvOrderOphaalAdres.ItemsSource = Order.ophaaladres;
            lvOrderAfhaalAdres.ItemsSource = Order.afleveradres;
            lvOrderOphaalAdres.Visibility = Visibility.Visible;
            lvOrderAfhaalAdres.Visibility = Visibility.Visible;
            btReturnToOrders.Visibility = Visibility.Visible;
        }

        private void spOphaalAdres_Tapped(object sender, TappedRoutedEventArgs e)
        {
            StackPanel send = sender as StackPanel;
            Ophaaladres test = send.Tag as Ophaaladres;
            try
            {
                MapControl1.Center = new Geopoint(new BasicGeoposition()
                {
                    Latitude = OphaalAdress.results[0].geometry.location.lat,
                    Longitude = OphaalAdress.results[0].geometry.location.lng
                });
            }
            catch { }

        }

        private void spAfleverAdres_Tapped(object sender, TappedRoutedEventArgs e)
        {
            StackPanel send = sender as StackPanel;
            Afleveradres test = send.Tag as Afleveradres;
            try
            {
                MapControl1.Center = new Geopoint(new BasicGeoposition()
                {
                    Latitude = AfhaalAdress.results[0].geometry.location.lat,
                    Longitude = AfhaalAdress.results[0].geometry.location.lng
                });
            }
            catch { }
        }

        private void btReturnToOrders_Click(object sender, RoutedEventArgs e)
        {
            lvOrderOphaalAdres.Visibility = Visibility.Collapsed;
            lvOrderAfhaalAdres.Visibility = Visibility.Collapsed;
            lvOrders.Visibility = Visibility.Visible;
            btReturnToOrders.Visibility = Visibility.Collapsed;
        }
    }
}
