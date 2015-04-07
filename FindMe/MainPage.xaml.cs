using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Devices.Geolocation;
using System.Device.Location;
using FindMe.Resources;
using System.Windows.Media;
using System.Windows.Shapes;

using Microsoft.Phone.Tasks;
using System.Windows.Threading;

namespace FindMe
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        // Get users' location functiion
        private GeoCoordinate geoCoordinate;
        public async void ShowLocationOnMap()
        {
            Geolocator geolocator = new Geolocator();
            Geoposition geoposition = await geolocator.GetGeopositionAsync();   // Get geolocation
            Geocoordinate geocoordinate = geoposition.Coordinate;
            geoCoordinate = CoordinateConverter.ConvertGeocoordinate(geocoordinate);    // Convert to a maps control compatible class

            map.Center = geoCoordinate; // Set current location as the centre of the map
            map.ZoomLevel = 13;

            // Create blue circle shown in app
            Ellipse marker = new Ellipse();
            marker.Fill = new SolidColorBrush(Colors.Blue);
            marker.Height = 20;
            marker.Width = 20;
            marker.Opacity = 50;

            // Overlay blue circle on map control
            MapOverlay mapOverlay = new MapOverlay();
            mapOverlay.Content = marker;
            mapOverlay.PositionOrigin = new Point(0.5, 0.5);
            mapOverlay.GeoCoordinate = geoCoordinate;

            MapLayer mapLayer = new MapLayer();
            mapLayer.Add(mapOverlay);

            map.Layers.Add(mapLayer);
        }

        // Click to get users' location
        private void appLocation_Click(object sender, EventArgs e)
        {
            pbLoading.Visibility = System.Windows.Visibility.Visible;
            ShowLocationOnMap();    // Call function to display users' location
            pbLoading.Visibility = System.Windows.Visibility.Collapsed;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = true;
        }


        // Share location
        private void appShare_Click(object sender, EventArgs e)
        {
            if (geoCoordinate != null)
            {
                ShareLinkTask shareLink = new ShareLinkTask();  // Initialize share task
                string uri = "findme:location?long=" + geoCoordinate.Longitude
                    + "&lat=" + geoCoordinate.Latitude
                    + "&alt=" + geoCoordinate.Altitude
                    + "&time=" + DateTime.Now.ToString(); // Create app deep-link
                shareLink.LinkUri = new Uri(uri);
                shareLink.Show();   // Launch share task 
            }
            else
            {
                MessageBox.Show("Location not found!");
            }
        }

        private void mainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Not Implemented
        }
    }
}