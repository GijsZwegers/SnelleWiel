﻿using System;
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
using Windows.Services.Maps;
using Windows.UI;
using System.ComponentModel;
using System.Runtime.CompilerServices;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SnelleWiel.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppPage : Page, INotifyPropertyChanged
    {
        Frame frame { get; set; }
        string sOrder { get; set; }
        Addresses addresses { get; set; }
        private Ophaaladres _OphaalAdress;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public Ophaaladres OphaalAdress
        {
            get { return _OphaalAdress; }
            set { _OphaalAdress = value; NotifyPropertyChanged(); }
        }

        private Afleveradres _AfhaalAdress;

        public Afleveradres AfhaalAdress
        {
            get { return _AfhaalAdress; }
            set { _AfhaalAdress = value; NotifyPropertyChanged();}
        }
        Location location { get; set; } = new Location();        
        RootObject rootObject { get; set; }

        ObservableCollection<Afleveradres> orderAfleverAdres = new ObservableCollection<Afleveradres>();
        ObservableCollection<Ophaaladres> orderOphaalAdres = new ObservableCollection<Ophaaladres>();
        Geolocator geolocator;
        public AppPage()
        {
            this.InitializeComponent();
            MapControl1.Center = new Geopoint(new BasicGeoposition() { Latitude = 51.44083, Longitude = 5.47778 });
            MapControl1.ZoomLevel = 10;
            this.Loaded += MainPage_Loaded;
            DataContext = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Tuple<Frame, string> tuple = e.Parameter as Tuple<Frame, string>;
            frame = tuple.Item1;
            sOrder = tuple.Item2;
            GetOrderAsync();
            
        }

        private async void GetOrderAsync()
        {
            rootObject = await UrlCalls.GetOrder(sOrder);

            OphaalAdress = new Ophaaladres()
            {
                id = rootObject.order.addresses.ophaaladres[0].id,
                plaats = rootObject.order.addresses.ophaaladres[0].plaats,
                straat = rootObject.order.addresses.ophaaladres[0].straat,
                huisnr = rootObject.order.addresses.ophaaladres[0].huisnr,
                postcode = rootObject.order.addresses.ophaaladres[0].postcode,
                telefoonnr = rootObject.order.addresses.ophaaladres[0].telefoonnr
            };

            AfhaalAdress = new Afleveradres()
            {
                id = rootObject.order.addresses.afleveradres[0].id,
                plaats = rootObject.order.addresses.afleveradres[0].plaats,
                straat = rootObject.order.addresses.afleveradres[0].straat,
                huisnr = rootObject.order.addresses.afleveradres[0].huisnr,
                postcode = rootObject.order.addresses.ophaaladres[0].postcode,
                telefoonnr = rootObject.order.addresses.afleveradres[0].telefoonnr
            };

            

            await PlaceOrderMarkersAsync();
        }

        private async System.Threading.Tasks.Task PlaceOrderMarkersAsync()
        {
            dynamic test = await UrlCalls.GetLangLong(
                rootObject.order.addresses.afleveradres[0].straat, 
                rootObject.order.addresses.afleveradres[0].huisnr, 
                rootObject.order.addresses.afleveradres[0].plaats, 
                rootObject.order.addresses.afleveradres[0].postcode
                );
            dynamic tester = test.results[0].geometry.location;
            location.lat = tester.lat;
            location.lng = tester.lng;
            MapIcon mapIconAflever = new MapIcon();
            // Locate your MapIcon  
            mapIconAflever.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/my-position.png"));
            // Show above the MapIcon  
            mapIconAflever.Title = "Afhaal Adres";
            // Setting up MapIcon location  
            mapIconAflever.Location = new Geopoint(new BasicGeoposition()
            {
                //Latitude = geoposition.Coordinate.Latitude, [Don't use]  
                //Longitude = geoposition.Coordinate.Longitude [Don't use]  
                Latitude = location.lat,
                Longitude =location.lng
            });
            // Positon of the MapIcon  
            mapIconAflever.NormalizedAnchorPoint = new Point(0.5, 0.5);
            MapControl1.MapElements.Add(mapIconAflever);

            //rootObject = await UrlCalls.GetOrder(sOrder);

            dynamic test2 = await UrlCalls.GetLangLong(
                rootObject.order.addresses.ophaaladres[0].straat,
                rootObject.order.addresses.ophaaladres[0].huisnr,
                rootObject.order.addresses.ophaaladres[0].plaats,
                rootObject.order.addresses.ophaaladres[0].postcode);
            dynamic tester2 = test.results[0].geometry.location;
            location.lat = tester2.lat;
            location.lng = tester2.lng;
            MapIcon mapIconOphaal = new MapIcon();
            // Locate your MapIcon  
            mapIconOphaal.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/my-position.png"));
            // Show above the MapIcon  
            mapIconOphaal.Title = "Ophaal Adres";
            // Setting up MapIcon location  
            mapIconOphaal.Location = new Geopoint(new BasicGeoposition()
            {
                //Latitude = geoposition.Coordinate.Latitude, [Don't use]  
                //Longitude = geoposition.Coordinate.Longitude [Don't use]  
                Latitude = location.lat,
                Longitude = location.lng
            });
            // Positon of the MapIcon  
            mapIconOphaal.NormalizedAnchorPoint = new Point(0.5, 0.5);
            MapControl1.MapElements.Add(mapIconOphaal);
            geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;

            try
            {
                Geoposition geoposition = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10));
               Geopoint geopoint = new Geopoint(new BasicGeoposition()
                {
                    //Latitude = geoposition.Coordinate.Latitude, [Don't use]  
                    //Longitude = geoposition.Coordinate.Longitude [Don't use]  
                    Latitude = geoposition.Coordinate.Point.Position.Latitude,
                    Longitude = geoposition.Coordinate.Point.Position.Longitude
                });
                DrawRoute(geopoint, mapIconOphaal.Location);
            }
            catch { }
            }

        private async void DrawRoute(Geopoint Start, Geopoint End)
        {
            MapRouteFinderResult Route = await MapRouteFinder.GetDrivingRouteAsync(
                Start, End,
                MapRouteOptimization.Time,
                MapRouteRestrictions.None);
            if (Route.Status == MapRouteFinderStatus.Success)
            {
                MapControl1.Routes.Clear();
                MapRouteView viewOfRoute = new MapRouteView(Route.Route)
                {
                    RouteColor = Colors.Yellow,
                    OutlineColor = Colors.Black
                };
                MapControl1.Routes.Add(viewOfRoute);
            }
            else
            {
                MessageBox("Kan de route niet vinden");
            }
        }

        #region MapFuncties
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
                //LocateMe.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
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
        #endregion
        

        private void btReturnToOrders_Click(object sender, RoutedEventArgs e)
        {
            frame.Navigate(typeof(OrderPage), frame);
        }
    }
}
