using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Microsoft.Phone.Tasks;
using System.Device.Location;
using System.Windows.Media;
using Microsoft.Phone.Maps.Controls;
using System.Windows.Shapes;
using Windows.Devices.Geolocation;

namespace FindMe
{
    public partial class FindPage : PhoneApplicationPage
    {
        public FindPage()
        {
            InitializeComponent();
            // Initialize "start" and "end" GeoCoordinates
            destination = new GeoCoordinate();
            departure = new GeoCoordinate();
        }

        private GeoCoordinate destination;
        private GeoCoordinate departure;
        private void findPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Parse app deep-link launched by user for GeoCoordinate values
            string url = System.Net.HttpUtility.UrlDecode(App.GetCurrent());
            int index = url.IndexOf("findme:location?");
            url = url.Substring(index, url.Length - index);
            index = url.IndexOf('?');
            ++index;
            url = url.Substring(index, url.Length - index);
            string[] parameters = url.Split('&');

            for (int i = 0; i < parameters.Length; i++)
            {
                string[] keyValue = parameters[i].Split('=');
                if (i == 0)
                    destination.Longitude = Double.Parse(keyValue[1]);
                else if (i == 1)
                    destination.Latitude = Double.Parse(keyValue[1]);
                else if (i == 2)
                    destination.Altitude = Double.Parse(keyValue[1]);
                else if (i == 3)
                    tbTime.Text += System.Net.HttpUtility.UrlDecode(keyValue[1]);
            }

            pbLoading.Visibility = System.Windows.Visibility.Collapsed;
            ShowLocationOnMap();    // Show shared location on map
            GetUserLocation();  // Get current user location
        }

        void ShowLocationOnMap()
        {
            map.Center = destination;
            map.ZoomLevel = 13;

            Ellipse marker = new Ellipse();
            marker.Fill = new SolidColorBrush(Colors.Blue);
            marker.Height = 20;
            marker.Width = 20;
            marker.Opacity = 50;

            MapOverlay mapOverlay = new MapOverlay();
            mapOverlay.Content = marker;
            mapOverlay.PositionOrigin = new Point(0.5, 0.5);
            mapOverlay.GeoCoordinate = destination;

            MapLayer mapLayer = new MapLayer();
            mapLayer.Add(mapOverlay);

            map.Layers.Add(mapLayer);
        }

        // Click directions appbar button
        private void abDirections_Click(object sender, EventArgs e)
        {
            // Windows Phone Task to show directions
            BingMapsDirectionsTask bmdTask = new BingMapsDirectionsTask();
            LabeledMapLocation destinationLocation = new LabeledMapLocation("End", destination);
            LabeledMapLocation startLocation = new LabeledMapLocation("Start", departure);

            bmdTask.Start = startLocation;  // Start location is current user's location
            bmdTask.End = destinationLocation;  // Destination location is location that was shared with the current user
            bmdTask.Show(); // Show directions
        }

        async void GetUserLocation()
        {
            Geolocator geolocator = new Geolocator();
            Geoposition geoposition = await geolocator.GetGeopositionAsync();
            Geocoordinate geocoordinate = geoposition.Coordinate;
            departure = CoordinateConverter.ConvertGeocoordinate(geocoordinate);

            //System.Diagnostics.Debug.WriteLine(departure.Longitude);
            //System.Diagnostics.Debug.WriteLine(departure.Latitude);
        }
    }
}