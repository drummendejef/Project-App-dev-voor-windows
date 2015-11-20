using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BOBApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FeestjesOverzicht : Page
    {
        RandomAccessStreamReference mapIconStreamReference;

        public FeestjesOverzicht()
        {
            this.InitializeComponent();

            MapFeestOverzicht.Loaded += myMap_Loaded;
            mapIconStreamReference = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/testpin.png"));
        }

        //Methods
        private void myMap_Loaded(object sender, RoutedEventArgs e)//Als de map geladen is.
        {
            //Map centreren op huidige locatie
            MapFeestOverzicht.Center = (App.Current as App).UserLocation.Coordinate.Point;//De userpoint ophalen, en de map hier op centreren.
            MapFeestOverzicht.ZoomLevel = 15;//Inzoomlevel instellen (hoe groter het getal, hoe dichterbij)
            MapFeestOverzicht.LandmarksVisible = true;

            //Marker voor eigen locatie plaatsen
            MapIcon mapIconUserLocation = new MapIcon();
            mapIconUserLocation.Location = MapFeestOverzicht.Center;
            mapIconUserLocation.NormalizedAnchorPoint = new Point(0.5, 1.0);
            mapIconUserLocation.Title = "Ik";//Titel die boven de marker komt.
            mapIconUserLocation.Image = mapIconStreamReference;
            MapFeestOverzicht.MapElements.Add(mapIconUserLocation);//Marker op de map zetten.

            //TODO: Locaties van andere feestjes ophalen (Joren) - API nog niet geschreven
            



        }


    }
}
