﻿using BOBApp.Messages;
using BOBApp.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Libraries.Models;
using Libraries.Models.relations;
using Libraries.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BOBApp.ViewModels
{
    public class MijnRittenVM : ViewModelBase
    {
        //for viewNavigation
        public bool Loading { get; set; }
        public string Error { get; set; }
        public HorizontalAlignment Car { get; set; }


        public string SearchLocation { get; set; }
        public List<Trip.All> Trips { get; set; }
        public Trip.All CurrentTrip { get; set; }
       
        private Trip.All _SelectedTrip;

        public Trip.All SelectedTrip
        {
            get { return _SelectedTrip; }
            set { _SelectedTrip = value;
               

            }
        }

        //search
        public RelayCommand SearchItemCommand { get; set; }
        private string _SearchItem;

        public string SearchItem
        {
            get { return _SearchItem; }
            set
            {
                _SearchItem = value;
                if (_SearchItem == null || _SearchItem.Trim() == "")
                {
                    if (trips_all != null)
                    {
                        this.Trips = trips_all;
                        RaisePropertyChanged("Trips");
                    }
                }
            }
        }



        //Constructor
        public MijnRittenVM()
        {
          

            Messenger.Default.Register<NavigateTo>(typeof(bool), ExecuteNavigatedTo);

            //sockets
            MainViewVM.socket.On("update_trip", async (msg) =>
            {
                Libraries.Socket _socket = JsonConvert.DeserializeObject<Libraries.Socket>((string)msg);
                //if (_socket.Status == true && _socket.To==MainViewVM.USER.ID)
                if (_socket.Status == true)
                {
                    await GetCurrentTrip();
                }

            });

            SearchItemCommand = new RelayCommand(Search);

            //raise
            RaiseAll();
        }

        private async void RaiseAll()
        {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                RaisePropertyChanged("Trips");
                RaisePropertyChanged("CurrentTrip");
                RaisePropertyChanged("Car");
                RaisePropertyChanged("Loading");
                RaisePropertyChanged("Error");
            });
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        }

        private async void ExecuteNavigatedTo(NavigateTo obj)
        {
            if (obj.Name == "loaded")
            {
                Type view = (Type)obj.View;
                if (view == (typeof(MijnRitten)))
                {
                    Loaded();
                }
            }
        }


        private async void Loaded()
        {
            this.Loading = true;
            RaisePropertyChanged("Loading");

            await Task.Run(async () =>
            {
                // running in background
               
#pragma warning disable CS1998
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    await GetCurrentTrip();
                    await GetTrips();

                    this.Loading = false;
                    RaiseAll();

                });
#pragma warning restore CS1998

            });
        }


        //Methods
        private async Task GetCurrentTrip()
        {
            this.Loading = true;
            RaisePropertyChanged("Loading");

            await Task.Run(async () =>
            {
                Trip currenttrip = Task.FromResult<Trip>(await TripRepository.GetCurrentTrip()).Result;
                if (currenttrip != null)
                {
                    if (currenttrip.Active == true)
                    {
                        Trip.All trips_all = new Trip.All();


                        User.All user = await UsersRepository.GetUserById(currenttrip.Users_ID);
                        Bob.All bob = await BobsRepository.GetBobById(currenttrip.Bobs_ID);
                        Users_Destinations destination = await DestinationRepository.GetDestinationById((currenttrip.Destinations_ID));
                        Party party = await PartyRepository.GetPartyById(currenttrip.Party_ID);
                        Trip.All newTrip = new Trip.All();



                        newTrip.Trip = currenttrip;
                        newTrip.Party = party;
                        newTrip.User = user;
                        newTrip.Bob = bob;
                        newTrip.Destination = destination;

                        MoveCar(newTrip.Trip.Status_Name);
                        this.CurrentTrip = newTrip;
                    }
                    else
                    {
                        this.CurrentTrip = null;
                        //geen current trip nu

                    }
                }
                RaiseAll();
            });
           

           

        }

        private async void MoveCar(string status_Name)
        {
            List<Status> statuses = Task.FromResult<List<Status>>(await StatusRepository.GetStatuses()).Result;

            if (status_Name == statuses[0].Name) {

            }
            switch (status_Name)
            {
                case "Aangevraagd":
                    Car = HorizontalAlignment.Left;
                  
                    break;
                case "Onderweg":
                    Car = HorizontalAlignment.Center;
                 
                    break;
                case "Toegekomen":
                    Car = HorizontalAlignment.Right;
                  
                    break;
                case "Done":
                    Car = HorizontalAlignment.Right;
                  
                    break;
                default:
                    break;
            }
            RaiseAll();
        }

        List<Trip.All> trips_all= new List<Trip.All>();

        private async Task GetTrips()
        {
            bool canRun = true;
            this.Loading = true;
            RaisePropertyChanged("Loading");

            await Task.Run(async () =>
            {
                List<Trip> trips = await TripRepository.GetTrips();

                trips_all = new List<Trip.All>();

                if (trips==null || trips.Count == 0)
                {
                    this.Error = "Geen trips gevonden";
                    canRun = false;
                    this.Loading = false;
                    RaiseAll();
                    return;
                }

                var count = trips.Count;
                if ((trips.Count >= 10) && canRun == true)
                {
                    count = 10;
                }

                for (int i = 0; i < count; i++)
                {
                    Task<User.All> user= Task.FromResult<User.All>(await UsersRepository.GetUserById(trips[i].Users_ID));
                    Task<Bob.All> bob = Task.FromResult<Bob.All>(await BobsRepository.GetBobById(trips[i].Bobs_ID));
                    Task<Users_Destinations> destination = Task.FromResult<Users_Destinations>(await DestinationRepository.GetDestinationById((trips[i].Destinations_ID)));
                    Task<Party> party = Task.FromResult<Party>(await PartyRepository.GetPartyById(trips[i].Party_ID));
                    Trip.All newTrip = new Trip.All();
                
                
                    newTrip.Trip = trips[i];
                    newTrip.User = user.Result;
                    newTrip.Bob = bob.Result;
                    newTrip.Party = party.Result;
                    newTrip.Destination = destination.Result;
                    trips_all.Add(newTrip);

                }


                this.Trips = trips_all;
                canRun = false;
                this.Loading = false;


                 RaiseAll();
              
            });


        }

        private void Search()
        {
            if (SearchItem == null)
            {
                return;
            }

            string item = this.SearchItem.ToString().Trim().ToLower();

            var newItems = trips_all.Where(r => r.User.User.ToString().Trim().ToLower() == item).ToList();
            if (newItems == null || newItems.Count == 0)
            {
                newItems = trips_all.Where(r => r.User.User.Firstname.Trim().ToLower() == item).ToList();

                if (newItems == null || newItems.Count == 0)
                {
                    newItems = trips_all.Where(r => r.User.User.Lastname.Trim().ToLower() == item).ToList();
                }

            }

            if (newItems != null && newItems.Count > 0)
            {
                this.Trips = newItems;
                this.Error = null;
            }
            else
            {
                this.Error = "Geen ritten gevonden";
            }



            RaiseAll();

        }


        //bind events
        public async void Changed(object sender, SelectionChangedEventArgs e)
        {

            Frame rootFrame = MainViewVM.MainFrame as Frame;
            rootFrame.Navigate(typeof(RitItem));


        }


        public void SearchChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {

            var value = args.SelectedItem as Trip.All;

            this.SearchItem = value.Bob.User.ToString();
            Search();
        }


    }
}
