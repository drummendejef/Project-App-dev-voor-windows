using BOBApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
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
    public sealed partial class Bestemmingen_Nieuw : Page
    {
        //Properties
        RandomAccessStreamReference mapIconStreamReference;//Eigen icoontje

        //Methods
        public Bestemmingen_Nieuw()
        {
            this.InitializeComponent();

            mapIconStreamReference = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/userpin.png"));
        }



        private void map_holding(Windows.UI.Xaml.Controls.Maps.MapControl sender, Windows.UI.Xaml.Controls.Maps.MapInputEventArgs args)
        {
            Debug.WriteLine("map_holding");

            Geopoint chosenDestinationLocation;
            //Muis locatie ophalen
            sender.GetLocationFromOffset(args.Position,out chosenDestinationLocation);
            Debug.WriteLine("Geklikt op positie: " + chosenDestinationLocation.Position.Latitude + ", " + chosenDestinationLocation.Position.Longitude);

            //Tijdelijke locatie aanmaken
            BasicGeoposition tempbasic = new BasicGeoposition();

            //Parsen
            tempbasic.Latitude = chosenDestinationLocation.Position.Latitude;
            tempbasic.Longitude = chosenDestinationLocation.Position.Longitude;

            //Tijdelijke locatie aanmaken, om de gekozen locatie om te zetten.
            Geopoint temppoint = new Geopoint(tempbasic);

            //Muis locatie porten op kaart
            MapIcon mapIconDestinationLocation = new MapIcon();
            mapIconDestinationLocation.Location = temppoint;
            mapIconDestinationLocation.Image = mapIconStreamReference;
            mapIconDestinationLocation.Title = "Bestemming";
            mapIconDestinationLocation.NormalizedAnchorPoint = new Point(0.5, 1.0);//Verzet het icoontje, zodat de punt van de marker staat op waar de locatie is. (anders zou de linkerbovenhoek op de locatie staan) 
            MapNieuweBestemming.MapElements.Add(mapIconDestinationLocation); //Marker op de map plaatsen

            //TODO: Op bestemming zetten, opslaan als ie op de knop drukt.
            //Bestemming in string opslaan
            string locatie = "{latitude:" + tempbasic.Latitude + ",longitude:" + tempbasic.Longitude + "}";

            //string doorsturen 
            //BestemmingenVM.setDestinationLocation(locatie);
            txtBlockNewDestinationLocation.Text = locatie;



        }
    }
}
