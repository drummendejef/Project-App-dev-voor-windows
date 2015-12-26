using BOBApp.Messages;
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
                Frame rootFrame = MainViewVM.MainFrame as Frame;
                rootFrame.Navigate(typeof(RitItem));

            }
        }

      

        //Constructor
        public MijnRittenVM()
        {
          

            Messenger.Default.Register<NavigateTo>(typeof(bool), ExecuteNavigatedTo);

            //sockets
            MainViewVM.socket.On("update_trip", (msg) =>
            {
                Libraries.Socket _socket = JsonConvert.DeserializeObject<Libraries.Socket>((string)msg);
                //if (_socket.Status == true && _socket.To==MainViewVM.USER.ID)
                if (_socket.Status == true)
                {
                    GetCurrentTrip();
                }

            });

            //raise
            RaiseAll();
        }

        private void RaiseAll()
        {
            RaisePropertyChanged("Trips");
            RaisePropertyChanged("CurrentTrip");
            RaisePropertyChanged("Car");
            RaisePropertyChanged("Loading");
            RaisePropertyChanged("Error");
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
                GetCurrentTrip();
                GetTrips();
#pragma warning disable CS1998
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    this.Loading = false;
                    RaiseAll();

                });
#pragma warning restore CS1998

            });
        }


        //Methods
        private async void GetCurrentTrip()
        {
            Trip currenttrip = Task.FromResult<Trip>(await TripRepository.GetCurrentTrip()).Result;
            if(MainViewVM.USER.IsBob == false && currenttrip != null)
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
                    newTrip.User = user.User;
                    newTrip.Bob = bob.User;
                    newTrip.Destination = destination;

                    MoveCar(newTrip.Trip.Status_Name);

                    this.CurrentTrip = newTrip;
                }
                else
                {
                    //geen current trip nu

                }
            }
           

           

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
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                RaiseAll();
            });
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        }

        List<Trip.All> trips_all= new List<Trip.All>();
        private async void GetTrips()
        {
            bool canRun = true;
            List<Trip> trips = await TripRepository.GetTrips();

            trips_all = new List<Trip.All>();

           
            for (int i = 0; i < trips.Count; i++)
            {
                Task<User.All> user= Task.FromResult<User.All>(await UsersRepository.GetUserById(trips[i].Users_ID));
                Task<Bob.All> bob = Task.FromResult<Bob.All>(await BobsRepository.GetBobById(trips[i].Bobs_ID));
                Task<Users_Destinations> destination = Task.FromResult<Users_Destinations>(await DestinationRepository.GetDestinationById((trips[i].Destinations_ID)));
                Task<Party> party = Task.FromResult<Party>(await PartyRepository.GetPartyById(trips[i].Party_ID));
                Trip.All newTrip = new Trip.All();
                
                
                newTrip.Trip = trips[i];
                newTrip.User = user.Result.User;
                newTrip.Bob = bob.Result.User;
                newTrip.Party = party.Result;
                newTrip.Destination = destination.Result;
                trips_all.Add(newTrip);

#pragma warning disable CS1998
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    if (trips_all.Count >= 10 && canRun == true)
                    {
                        this.Trips = trips_all.Take(10).ToList();
                        RaiseAll();
                        canRun = false;
                    }



                });
#pragma warning restore CS1998

            }





        }
    }
}
