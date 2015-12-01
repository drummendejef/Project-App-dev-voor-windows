using BOBApp.Models;
using BOBApp.Repositories;
using System;
using System.Collections.Generic;
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
    public sealed partial class FeestjesOverzicht : Page
    {
        RandomAccessStreamReference mapIconStreamReference;
        RandomAccessStreamReference mapIconStreamReferenceParty;

        public FeestjesOverzicht()
        {
            this.InitializeComponent();

            MapFeestOverzicht.Loaded += myMap_Loaded;
            mapIconStreamReference = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/userpin.png"));
            mapIconStreamReferenceParty = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/feestpin.png"));
        }

        //Methods
        private async void myMap_Loaded(object sender, RoutedEventArgs e)//Als de map geladen is.
        {
            //Map centreren op huidige locatie
            MapFeestOverzicht.Center = (App.Current as App).UserLocation.Coordinate.Point;//De userpoint ophalen, en de map hier op centreren.
            MapFeestOverzicht.ZoomLevel = 15;//Inzoomlevel instellen (hoe groter het getal, hoe dichterbij)
            MapFeestOverzicht.LandmarksVisible = true;

            //Marker voor eigen locatie plaatsen
            MapIcon mapIconUserLocation = new MapIcon();
            mapIconUserLocation.Location = MapFeestOverzicht.Center;
            mapIconUserLocation.NormalizedAnchorPoint = new Point(0.5, 1.0);//Verzet het icoontje, zodat de punt van de marker staat op waar de locatie is. (anders zou de linkerbovenhoek op de locatie staan) 
            mapIconUserLocation.Title = "Ik";//Titel die boven de marker komt.
            mapIconUserLocation.Image = mapIconStreamReference;
            MapFeestOverzicht.MapElements.Add(mapIconUserLocation);//Marker op de map zetten.

            //TODO: Locaties van andere feestjes ophalen (Joren) - API nog niet geschreven

            List<Party> feestjes = await PartyRepository.GetPartys();


            int aantalfeestjes = feestjes.Count;// na het ophalen van de feestjes, het aantal feestjes tellen (NOG DOEN)
            for(int i = 0; i < aantalfeestjes; i++)//Alle feestjes overlopen en markers zetten.
            {
                //Kan weg gelaten worden door feestjes[i] op een van de onderstaande lijnen te zetten (optimalisatie?) maar dit is misschien duidelijker. Keuzes keuzes
                Party feest = feestjes[i];

                //Tijdelijke locatie aanmaken
                BasicGeoposition tempbasic = new BasicGeoposition();

                //Feestlocatie opsplitsen (word opgeslagen als string)
                string[] splittedcoord = feest.Location.Split(',',':','}');//Splitsen op } zodat de lon proper is

                //Locaties omzetten en in de tijdelijke posities opslaan.
                tempbasic.Latitude = double.Parse(splittedcoord[1].ToString());
                tempbasic.Longitude = double.Parse(splittedcoord[3].ToString());

                //Omzetten van tijdelijk punt naar echte locatie (anders krijg je die niet in de mapIconFeestLocation.Location)
                Geopoint temppoint = new Geopoint(tempbasic);

                MapIcon mapIconFeestLocation = new MapIcon();
                mapIconFeestLocation.Location = temppoint; //Opgehaalde locatie
                mapIconFeestLocation.Title = feest.Name; //Naam van het feestje;
                mapIconFeestLocation.Image = mapIconStreamReferenceParty;
                MapFeestOverzicht.MapElements.Add(mapIconFeestLocation);//Marker op de map zetten.
            }
        }
    }
}
