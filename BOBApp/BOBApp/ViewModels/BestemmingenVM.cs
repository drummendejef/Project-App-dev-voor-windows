using BOBApp.Messages;
using BOBApp.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Libraries;
using Libraries.Models;
using Libraries.Models.relations;
using Libraries.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
        //public Destination Destination { get; set; }

        private City _Destination;

        public City Destination
        {
            get { return _Destination; }
            set { _Destination = value;

                

            }
        }




        //Constructor
        public BestemmingenVM()
        {
            GetCities();
            AddDestinationCommand = new RelayCommand(AddDestination);
            GoDestinationCommand = new RelayCommand(GoDestination);
            GoToCityCommand = new RelayCommand(TownToCoord);
            RaisePropertyChanged("SearchLocation");
            this.Destination = new City();
            RaisePropertyChanged("Destination");
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

            //URL aanmaken (tijdelijk, later in URL klasse zetten)
            /*string bingkey = "dOUBDBVwN5QvZ1iHg90c~s2bgtqxiAZX20yceA6JFuw~An9qrmMutNOdQJ0PiF_t7WMqjN4lZBOWQaKrphjthrGdwmqvhjUvX8--_O2kP2K5";
            string URLBasisTownToCoord = "http://dev.virtualearth.net/REST/v1/Locations?locality=";
            string URLBingKey = "&key=";*/

            //Volledige string maken
            string URLFullTownToCoord = URL.BASISURLTOWNTOCOORD + Town + URL.URLBINGKEY + URL.BINGKEY;

            using (HttpClient client = new HttpClient())
            {
                var result = client.GetAsync(URLFullTownToCoord);
                string json = await result.Result.Content.ReadAsStringAsync();

            }

            // https://msdn.microsoft.com/en-us/library/ff701714.aspx 
        }
    }
}
