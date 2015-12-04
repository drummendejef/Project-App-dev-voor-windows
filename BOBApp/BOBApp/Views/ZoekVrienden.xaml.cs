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
    public sealed partial class ZoekVrienden : Page
    {
        RandomAccessStreamReference mapIconStreamReference;
        RandomAccessStreamReference mapIconStreamReferenceVriend;

        public ZoekVrienden()
        {
            this.InitializeComponent();

            MapZoekVriend.Loaded += myMap_Loaded;
            mapIconStreamReference = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/userpin.png"));
            mapIconStreamReferenceVriend = RandomAccessStreamReference.CreateFromUri(new Uri("s-appx:///Assets/vriendpin.png"));
        }

        private void myMap_Loaded(object sender, RoutedEventArgs e)
        {
            if ((App.Current as App).UserLocation != null)//Als de eigen locatie niet gekend is.
            {
                //Map centreren op huidige locatie
                MapZoekVriend.Center = (App.Current as App).UserLocation.Coordinate.Point;//De userpoint ophalen, en de map hier op centreren.
                MapZoekVriend.ZoomLevel = 15; //Inzoomlevel instellen (hoe groter het getal, hoe dichterbij)
                MapZoekVriend.LandmarksVisible = true;

                //Marker voor eigen locatie plaatsen
                MapIcon mapIconUserLocation = new MapIcon();
                mapIconUserLocation.Location = MapZoekVriend.Center;//De map is al gecentreerd op waar de gebruiker is, dus ook hier een marker zetten.
                mapIconUserLocation.NormalizedAnchorPoint = new Point(0.5, 1.0);//Verzet het icoontje, zodat de punt van de marker staat op waar de locatie is. (anders zou de linkerbovenhoek op de locatie staan) 
                mapIconUserLocation.Title = "Ik";//Titel die boven de marker komt.
                mapIconUserLocation.Image = mapIconStreamReference;
                MapZoekVriend.MapElements.Add(mapIconUserLocation);//Marker op de map zetten.
            }

            //TODO: locaties van vrienden ophalen (Joren) - API nog niet geschreven
            int aantalvrienden = 0;
            for(int i = 0; i < aantalvrienden; i++)//Alle vrienden overlopen en markers zetten op hun locatie
            {
                MapIcon mapIconVriendLocation = new MapIcon();
                //mapIconVriendLocation.Location = //Opgehaalde locatie;
                //mapIconVriendLocation.Title = //Naam van de vriend;
                mapIconVriendLocation.Image = mapIconStreamReferenceVriend;
                MapZoekVriend.MapElements.Add(mapIconVriendLocation);
            }

        }
    }
}
