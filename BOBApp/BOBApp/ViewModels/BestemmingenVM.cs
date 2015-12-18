using BOBApp.Messages;
using BOBApp.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Libraries;
using Libraries.Models;
using Libraries.Models.relations;
using Libraries.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls;

namespace BOBApp.ViewModels
{
    public class BestemmingenVM: ViewModelBase
    {
        //Properties
        public string SearchLocation { get; set; }
        private Task task;
        public RelayCommand GoDestinationCommand { get; set; }
        public RelayCommand AddDestinationCommand { get; set; }
        public RelayCommand GoToCityCommand { get; set; }

        public List<City> Cities { get; set; }
        public List<Users_Destinations> Destinations { get; set; }
        public Point CityLocation { get; set; }

        private City _Destination;

        public City Destination
        {
            get { return _Destination; }
            set { _Destination = value;

                

            }
        }
        
        //databinding voor het center van de map
        public Geopoint MapCenter { get; set; }


        //Constructor
        public BestemmingenVM()
        {
            if ((App.Current as App).UserLocation != null)//Kijken of we de locatie van de gebruiker hebben.
                MapCenter = (App.Current as App).UserLocation.Coordinate.Point;//Zoja, center van de map eerst op de persoon focussen.


            GetCities();
            AddDestinationCommand = new RelayCommand(AddDestination);
            GoDestinationCommand = new RelayCommand(GoDestination);
            GoToCityCommand = new RelayCommand(TownToCoord);
            RaisePropertyChanged("SearchLocation");
            this.Destination = new City();
            RaisePropertyChanged("Destination");
            RaisePropertyChanged("MapCenter");
        }




        //Methods
        private void AddDestination()
        {
            task = AddDestination_task();
            
        }

        private async Task AddDestination_task()
        {
            Destination destination = new Destination()
            {
                Cities_ID= this.Destination.ID
            };
            Libraries.Models.Response ok = await DestinationRepository.PostDestination(destination);
        }

        private void GoDestination()
        {
            Frame rootFrame =MainViewVM.MainFrame as Frame;

            rootFrame.Navigate(typeof(Bestemmingen_Nieuw));
        }
        private async void GetDestinations()
        {
            this.Destinations = await DestinationRepository.GetDestinations();

        }
        private async void GetCities()
        {
            this.Cities = await CityRepository.GetCities();
            this.Cities.Sort((a, b) => a.Name.CompareTo(b.Name));

        }

        //De naam van de gemeente omzetten naar de locatie
        private async void TownToCoord()
        {
            //Naam ophalen uit de geselecteerde stad.
            string Town = Destination.Name;

            //Volledige string maken
            string URLFullTownToCoord = URL.BASISURLTOWNTOCOORD + Town + URL.URLBINGKEY + URL.BINGKEY + "&maxRes=1";//Volledige request url aanmaken, maar 1 resultaat terugvragen.

            using (HttpClient client = new HttpClient())
            {
                var result = client.GetAsync(URLFullTownToCoord);
                string json = await result.Result.Content.ReadAsStringAsync();
                var root = JsonConvert.DeserializeObject<TownToCoordinates.RootObject>(json);

                foreach(var rs in root.resourceSets)//In de Json resourcessets gaan
                {
                    foreach(var r in rs.resources)//Alle resources overlopen (maar 1, door onze vraag)
                    {
                        Debug.WriteLine("Geselecteerde stadsnaam: " + Destination.Name + ", ontvangen stadsnaam: " + r.name);
                        Debug.WriteLine("Coordinaten ontvangen stad: " + r.point.coordinates[0] + ", " + r.point.coordinates[1]);
                        Debug.WriteLine("Coordinaten persoon: " + MapCenter.Position.Latitude + ", " + MapCenter.Position.Longitude);

                        //Coordinaten aanmaken
                        Geopoint temppoint = new Geopoint(new BasicGeoposition() { Latitude = r.point.coordinates[0], Longitude = r.point.coordinates[1] });

                        MapCenter = temppoint;
                        RaisePropertyChanged("MapCenter");//Gebruiken zodat de mapcenter meteen veranderd
                    }
                }
            }
            
            // https://msdn.microsoft.com/en-us/library/ff701714.aspx 
        }
    }
}
