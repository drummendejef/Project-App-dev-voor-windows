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
    public sealed partial class VindRit : Page
    {
        RandomAccessStreamReference mapIconStreamReference;//Eigen icoon
        RandomAccessStreamReference mapIconStreamReferenceRit;//Icoon van de bobs

        public VindRit()
        {
            this.InitializeComponent();

            MapVindRit.Loaded += myMap_Loaded;

            mapIconStreamReference = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/userpin.png"));
            mapIconStreamReferenceRit = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/BOBpin.png"));
        }

        private void myMap_Loaded(object sender, RoutedEventArgs e)
        {
            if ((App.Current as App).UserLocation != null)//Als de eigen locatie niet gekend is.
            {
                //Map centreren op huidige locatie
                MapVindRit.Center = (App.Current as App).UserLocation.Coordinate.Point;//De userpoint ophalen, en de map hier op centreren.
                MapVindRit.ZoomLevel = 15;//Inzoomlevel instellen (hoe groter het getal, hoe dichterbij)
                MapVindRit.LandmarksVisible = true;

                //Marker voor eigen locatie plaatsen
                MapIcon mapIconUserLocation = new MapIcon();
                mapIconUserLocation.Location = MapVindRit.Center;
                mapIconUserLocation.NormalizedAnchorPoint = new Point(0.5, 1.0);//Verzet het icoontje, zodat de punt van de marker staat op waar de locatie is. (anders zou de linkerbovenhoek op de locatie staan) 
                mapIconUserLocation.Title = "Ik";//Titel die boven de marker komt.
                mapIconUserLocation.Image = mapIconStreamReference;
                MapVindRit.MapElements.Add(mapIconUserLocation);//Marker op de map zetten.
            }

            //TODO: Locaties van BOBs ophalen (Joren) - API nog niet geschreven
            int aantalBobs = 0;//Na het ophalen van de bobs, het aantal bobs tellen.
            for(int i = 0; i < aantalBobs; i++)//Alle bobs overlopen en markers zetten
            {
                MapIcon mapIconBobLocation = new MapIcon();
                //mapIconBobLocation.Location = //Opgehaalde locatie;
                //mapIconBobLocation.Title = //Naam van de bob;
                mapIconBobLocation.Image = mapIconStreamReferenceRit;
                MapVindRit.MapElements.Add(mapIconBobLocation);//Marker op de map zetten.
            }
        }
    }
}
