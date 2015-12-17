using BOBApp.Messages;
using BOBApp.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Libraries.Models;
using Libraries.Models.relations;
using Libraries.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
