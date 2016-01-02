using BOBApp.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Libraries.Models;
using Libraries.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Quobject.SocketIoClientDotNet.Client;
using GalaSoft.MvvmLight.Messaging;
using BOBApp.Messages;
using Libraries.Models.relations;
using Libraries;
using Windows.UI.Xaml;
using Windows.Devices.Geolocation;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using System.ComponentModel;
using System.Diagnostics;

namespace BOBApp.ViewModels
{
    public class VindRitBobVM : ViewModelBase
    {
        #region props
        #region static

        public static int Request { get; set; }
        public List<Bob> BobList { get; set; }


        #endregion

        public bool Loading { get; set; }
        public string Error { get; set; }
        DispatcherTimer timer = MainViewVM.TIMER;

        public Visibility VisibleModal { get; set; }
        public Visibility VisibleOffer { get; set; }
        public Visibility VisibleCancel { get; set; }
        public bool IsEnabledOffer { get; set; }
        public Frame Frame { get; set; }
        public string OfferText { get; set; }
        public string Status { get; set; }

     
        public List<User> UserRequest { get; set; }

        private bool _CanOffer;

        public bool CanOffer
        {
            get { return _CanOffer; }
            set {
                    SetActive(value);
                    _CanOffer = value;
                    

            }
        }

        private async void SetActive(bool value)
        {
            //todo in db
            Response res = Task.FromResult<Response>(await BobsRepository.SetOffer(value, MainViewVM.USER.Bobs_ID.Value)).Result;
         
        }


        #region gets

        public Visibility VisibleSelectedTrip { get; set; }
        public string GetSelectedTrip
        {
            get
            {

                if (MainViewVM.CurrentTrip != null)
                {
                  
                    this.VisibleSelectedTrip = Visibility.Visible;
                    return MainViewVM.CurrentTrip.Status_Name;
                }
                else
                {
                    this.VisibleSelectedTrip = Visibility.Collapsed;
                    return "";
                }

            }
        }

        #endregion

        //others
        public RelayCommand ArrivedCommand { get; set; }
        public RelayCommand ShowModalCommand { get; set; }
        public RelayCommand CloseModalCommand { get; set; }
        public RelayCommand GoChatCommand { get; set; }
       
        public RelayCommand CancelCommand { get; set; }
        public string BobRequests { get; set; }
        public string RitTime { get; set; }

        private async Task getRitTime(Location location)
        {
            if (location != null)
            {
                //checkhowfaraway
                Users_Destinations destination = await DestinationRepository.GetDestinationById(MainViewVM.CurrentTrip.Destinations_ID);
                Response farEnough = Task.FromResult<Response>(await TripRepository.Difference((Location)destination.Location, location)).Result;

                if (farEnough.Success == true)
                {
                    double distance;
                    double.TryParse(farEnough.Value.ToString(), out distance);


                    double speed = 50;
                    double time = distance / speed;

                    this.RitTime = ": " + time.ToString();
                    RaiseAll();
                }


            }
        }




        #endregion

        //Constructor
        public VindRitBobVM()
        {

           
            CloseModalCommand = new RelayCommand(CloseModal);
            ShowModalCommand = new RelayCommand(ShowModal);
            ArrivedCommand = new RelayCommand(Arrived);
            GoChatCommand = new RelayCommand(GoChat);
         
            CancelCommand = new RelayCommand(Cancel);

            Messenger.Default.Register<NavigateTo>(typeof(bool), ExecuteNavigatedTo);

            this.Loading = false;
          
            this.BobRequests = "Momenteel " + VindRitBobVM.Request.ToString() + " aanvragen";

            this.IsEnabledOffer = true;
            this.VisibleCancel = Visibility.Collapsed;
            this.VisibleOffer = Visibility.Collapsed;
            this.CanOffer = false;



            RaiseAll();
        }

        private void Cancel()
        {
            CancelTrip();
        }



        private async void Arrived()
        {
            SetStatus(4);

            RaiseAll();
            if (timer != null)
            {
                timer.Stop();
            }
           

            Geolocator geolocator = new Geolocator();
            Geoposition pos = await geolocator.GetGeopositionAsync();
            Location location = new Location() { Latitude = pos.Coordinate.Point.Position.Latitude, Longitude = pos.Coordinate.Point.Position.Longitude };


            Trips_Locations item = new Trips_Locations()
            {
                Trips_ID = MainViewVM.CurrentTrip.ID,
                Location = JsonConvert.SerializeObject(location),
                Statuses_ID = VindRitVM.StatusID
            };
            Response ok = Task.FromResult<Response>(await TripRepository.PostLocation(item)).Result;
            if (ok.Success == true)
            {

                Bob.All bob = Task.FromResult<Bob.All>(await BobsRepository.GetBobById(MainViewVM.CurrentTrip.Bobs_ID)).Result;
                Libraries.Socket socketSend = new Libraries.Socket() { From = MainViewVM.USER.ID, To = bob.User.ID, Status = true };
                MainViewVM.socket.Emit("trip_UPDATE:send", JsonConvert.SerializeObject(socketSend));
            }
            else
            {
                timer.Stop();

            }

            if (canShowDialog == true)
            {
                canShowDialog = false;

                Bob.All bob = Task.FromResult<Bob.All>(await BobsRepository.GetBobById(MainViewVM.CurrentTrip.Bobs_ID)).Result;

                BobisDone(location, "Trip is afgerond");
            }
        }

        private async void ExecuteNavigatedTo(NavigateTo obj)
        {
            if (obj.Name == "loaded")
            {
                Type view = (Type)obj.View;
                if (view == typeof(VindRitBob))
                {
                    //loaded
                    Loaded();
                }
            }
            if (obj.Reload == true)
            {
                Type view = (Type)obj.View;
                if (view == typeof(VindRitBob))
                {
                    //loaded
                    Loaded();
                }
            }

            if (obj.Name != null && obj.Name != "")
            {
                switch (obj.Name)
                {
                    case "bob_accepted":
                        float id = float.Parse(obj.Data.ToString());
                        bob_accepted((bool)obj.Result, id);
                        break;
                    case "newtrip_bob":
                        #pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
                        await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                        {
                            newtrip_bob((Trip)obj.Data);
                        });
                        #pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
                        break;
                    case "trip_location:reload":
                        Users_Destinations dest = Task.FromResult<Users_Destinations>(await DestinationRepository.GetDestinationById(MainViewVM.CurrentTrip.Destinations_ID)).Result;

                        //instellen voor timer, niet gebruiken in filter
                        VindRitFilterVM.SelectedDestination = dest;
                        if (VindRitFilterVM.SelectedDestination != null)
                        {
                            StartTripLocationTimer();
                        }
                        RaiseAll();
                        break;
                    case "map_reload":
                        //map reload
                        break;
                    default:
                        break;
                }
            }
        }

        private async void newtrip_bob(Trip data)
        {
            if (data != null || MainViewVM.CurrentTrip==null)
            {
                this.IsEnabledOffer = false;
                this.VisibleCancel = Visibility.Visible;
                this.VisibleOffer = Visibility.Visible;
                MainViewVM.CurrentTrip = data;
                VindRitFilterVM.SelectedDestination = await DestinationRepository.GetDestinationById(MainViewVM.CurrentTrip.Destinations_ID);
                VindRitBobVM.Request = 1;

                await UserRepository.PostPoint(3);

                trip_location();
                RaiseAll();

            }
           
        }

        private async void bob_accepted(bool accepted, float id)
        {
            this.Status = null;

            this.Loading = false;
            RaiseAll();

            if (accepted == true)
            {
                //verstuur trip
                User.All user = Task.FromResult<User.All>(await UsersRepository.GetUserById(VindRitVM.SelectedUser.ID)).Result;
                Libraries.Socket socketSend = new Libraries.Socket()
                {
                    From = MainViewVM.USER.ID,//from bob
                    To = user.User.ID,//to user
                    Status = true,
                    Object = user,
                    ID=id
                };

                MainViewVM.socket.Emit("trip_MAKE:send", JsonConvert.SerializeObject(socketSend));

            }

        }

        private async void RaiseAll()
        {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                RaisePropertyChanged("Loading");
                RaisePropertyChanged("VisibleModal");
                RaisePropertyChanged("VisibleCancel");
                RaisePropertyChanged("VisibleOffer");
                RaisePropertyChanged("Frame");
                RaisePropertyChanged("BobRequests");
                RaisePropertyChanged("Status");
                RaisePropertyChanged("UserRequests");
                RaisePropertyChanged("OfferText");
                RaisePropertyChanged("RitTime");
                RaisePropertyChanged("CanOffer");
                RaisePropertyChanged("IsEnabledOffer");

                RaisePropertyChanged("GetSelectedTrip");


                RaisePropertyChanged("VisibleSelectedTrip");

            });
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
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
                    //default on start
                  
                    VisibleModal = Visibility.Collapsed;

                    await GetCurrentTrip();

                    this.BobRequests = "Momenteel " + VindRitBobVM.Request.ToString() + " aanvragen";
                    this.Loading = false;
                    RaiseAll();

                });
#pragma warning restore CS1998

            });
        }



        private void GoChat()
        {
            Frame rootFrame = MainViewVM.MainFrame as Frame;

            if (VindRitChatVM.ID != null)
            {

                rootFrame.Navigate(typeof(VindRitChat), true);
            }
            else
            {
                rootFrame.Navigate(typeof(VindRitChat), false);
            }



        }


        private void CloseModal()
        {
            //Loaded();

            VisibleModal = Visibility.Collapsed;
            RaiseAll();

           
           
        }
        private void ShowModal()
        {
           
            this.Frame.Navigate(typeof(VindRitFilter));
            this.Frame.Navigated += Frame_Navigated;

            VisibleModal = Visibility.Visible;
            RaiseAll();

        }

        private async void SetStatus(int statusID)
        {
            VindRitVM.StatusID = statusID;
            this.Status = GetStatusName(statusID);
            if (MainViewVM.CurrentTrip != null)
            {
                var party = Task.FromResult<Party>(await PartyRepository.GetPartyById(MainViewVM.CurrentTrip.Party_ID)).Result;
                var destination = Task.FromResult<Users_Destinations>(await DestinationRepository.GetDestinationById(MainViewVM.CurrentTrip.Destinations_ID)).Result;

                Toast.Tile("Party: " + party.Name, "Bestemming: " + destination.Name, "Status " + this.Status);

            }

            RaiseAll();
        }

        private string GetStatusName(int statusID)
        {
            switch (statusID)
            {
                case 0:
                    return "";
                case 1:
                    return "Bob is aangevraagd";
                case 2:
                    return "Bob is onderweg";
                case 3:
                    return "Bob is toegekomen";
                case 4:
                    return "Rit is gedaan";
                case 5:
                    return "Rit is geannuleerd";
                case 6://niet in db
                    return "Wachten op antwoord van bob";
                case 7: //niet in db
                    return "Bob is in de buurt";
                default:
                    return "";

            }
        }

        private void Frame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                bool reload = (bool)e.Parameter;

                Messenger.Default.Send<NavigateTo>(new NavigateTo()
                {
                    Reload = reload,
                    View = e.SourcePageType
                });

                if (reload == false)
                {
                    //e.Cancel = true;

                }
            }
        }




        //update location user/bob naar de db

        private void trip_location()
        {
            SetStatus(2);
            this.Loading = false;
            //todo: swtich
           

            StartTripLocationTimer();
            RaiseAll();
        }


        #region  StartTripLocationTimer
        
        bool canShowDialog;
        private async void StartTripLocationTimer()
        {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                if (timer != null)
                {
                    timer.Stop();
                    timer = null;
                    return;


                }

                timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 20);
                timer.Tick += Timer_Tick;
                canShowDialog = true;
                timer.Start();
            });
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        }

        private async void Timer_Tick(object sender, object e)
        {
            Geolocator geolocator = new Geolocator();
            Geoposition pos = await geolocator.GetGeopositionAsync();
            Location location = new Location() { Latitude = pos.Coordinate.Point.Position.Latitude, Longitude = pos.Coordinate.Point.Position.Longitude };

            await getRitTime(location);

            if (location != null)
            {
                //checkhowfaraway
                Response farEnough = Task.FromResult<Response>(await TripRepository.Difference((Location)VindRitFilterVM.SelectedDestination.Location, location)).Result;

                if (farEnough.Success == true)
                {
                    //kleiner dan 1km
                   


                    Trips_Locations item = new Trips_Locations()
                    {
                        Trips_ID = MainViewVM.CurrentTrip.ID,
                        Location = JsonConvert.SerializeObject(location),
                        Statuses_ID = VindRitVM.StatusID
                    };
                    Response ok = Task.FromResult<Response>(await TripRepository.PostLocation(item)).Result;
                    if (ok.Success == true)
                    {

                        User.All user = Task.FromResult<User.All>(await UsersRepository.GetUserById(MainViewVM.CurrentTrip.Users_ID)).Result;
                        Libraries.Socket socketSend = new Libraries.Socket() { From = MainViewVM.USER.ID, To = user.User.ID, Status = true };
                        MainViewVM.socket.Emit("user_location_NEW:send", JsonConvert.SerializeObject(socketSend)); //bob
                    }
                    else
                    {
                        timer.Stop();

                    }

                    if (canShowDialog == true)
                    {
                        canShowDialog = false;

                        bool done = Task.FromResult<bool>(await OnDestination()).Result;
                        if (done == true)
                        {
                            SetStatus(3);
                            timer.Stop();
                            RaiseAll();

                          

                            BobisDone(location, "Trip is afgerond");

                        }
                    }


                }
                else
                {
                    //VindRitFilterVM.SelectedDestination.Location;
                    Trips_Locations item = new Trips_Locations()
                    {
                        Trips_ID = MainViewVM.CurrentTrip.ID,
                        Location = JsonConvert.SerializeObject(location),
                        Statuses_ID = VindRitVM.StatusID
                    };
                    Response ok = Task.FromResult<Response>(await TripRepository.PostLocation(item)).Result;
                    if (ok.Success == false)
                    {
                        timer.Stop();
                    }
                }


            }
        }

        private async void BobisDone(Location location, string text)
        {
            SetStatus(4);

            Bobs_Parties bobs_parties = new Bobs_Parties()
            {
                Bobs_ID = MainViewVM.CurrentTrip.Bobs_ID,
                Party_ID = MainViewVM.CurrentTrip.Party_ID,
                Trips_ID = MainViewVM.CurrentTrip.ID,
                Users_ID=MainViewVM.CurrentTrip.Users_ID

            };

            Trips_Locations item = new Trips_Locations()
            {
                Trips_ID = MainViewVM.CurrentTrip.ID,
                Location = JsonConvert.SerializeObject(location),
                Statuses_ID = VindRitVM.StatusID
            };
            Response ok = Task.FromResult<Response>(await TripRepository.PostLocation(item)).Result;
            Response active = Task.FromResult<Response>(await TripRepository.PutActive(item.Trips_ID, false)).Result;

            if (ok.Success == true && active.Success == true)
            {

                Libraries.Socket socketSendToBob = new Libraries.Socket()
                {
                    From = MainViewVM.CurrentTrip.Users_ID,
                    To = MainViewVM.USER.ID,
                    Status = true,
                    Object = JsonConvert.SerializeObject(bobs_parties),
                    Object2 = false

                };
                Libraries.Socket socketSendToUser = new Libraries.Socket()
                {
                    From = MainViewVM.USER.ID,
                    To =MainViewVM.CurrentTrip.Users_ID,
                    Status = true,
                    Object = JsonConvert.SerializeObject(bobs_parties),
                    Object2 = true
                };






                MainViewVM.socket.Emit("trip_DONE:send", JsonConvert.SerializeObject(socketSendToUser));
                MainViewVM.socket.Emit("trip_DONE:send", JsonConvert.SerializeObject(socketSendToBob));


                var dialog = new MessageDialog(text);
                dialog.Commands.Add(new UICommand("Ok") { Id = 0 });
                dialog.DefaultCommandIndex = 0;
                var result = await dialog.ShowAsync();
                int id = int.Parse(result.Id.ToString());


                this.IsEnabledOffer = true;
                this.VisibleCancel = Visibility.Collapsed;
                this.VisibleOffer = Visibility.Collapsed;
                VindRitBobVM.Request -= 1;
                this.Loading = false;
                this.Status = null;
                SetStatus(0);




                canShowDialog = true;
                RaiseAll();


            }
        }



        private async Task<bool> OnDestination()
        {
            var dialog = new MessageDialog("Bent u toegekomen op de locatie?");

            dialog.Commands.Add(new UICommand("Yes") { Id = 0 });
            dialog.Commands.Add(new UICommand("No") { Id = 1 });



            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            var result = await dialog.ShowAsync();

            int id = int.Parse(result.Id.ToString());
            if (id == 0)
            {

                return true;
            }
            else
            {

                return false;
            }
        }

        #endregion


        private async Task GetCurrentTrip()
        {
            if (MainViewVM.CurrentTrip == null || MainViewVM.CurrentTrip.ID == 0)
            {
                try
                {
                    string json = await Localdata.read("trip.json");
                    if (json != null && json != "")
                    {
                        var data = JsonConvert.DeserializeObject<Trip>(json);
                        if (data.ID != -1)
                        {
                            newtrip_bob(data);
                            return;
                        }
                    }

                    this.IsEnabledOffer = true;
                    this.VisibleCancel = Visibility.Collapsed;
                    this.VisibleOffer = Visibility.Collapsed;



                }
                catch (Exception ex)
                {
                    this.IsEnabledOffer = true;
                    this.VisibleCancel = Visibility.Collapsed;
                    this.VisibleOffer = Visibility.Collapsed;

                }
            }
            else
            {
                this.IsEnabledOffer = false;
                this.VisibleCancel = Visibility.Visible;
                this.VisibleOffer = Visibility.Visible;

            }
            RaiseAll();




        }



        private async void CancelTrip()
        {
            if (timer != null)
            {
                timer.Stop();
            }
            
            SetStatus(5);

                Geolocator geolocator = new Geolocator();
                Geoposition pos = await geolocator.GetGeopositionAsync();
                Location location = new Location() { Latitude = pos.Coordinate.Point.Position.Latitude, Longitude = pos.Coordinate.Point.Position.Longitude };

                Trips_Locations item = new Trips_Locations()
                {
                    Trips_ID = MainViewVM.CurrentTrip.ID,
                    Location = JsonConvert.SerializeObject(location),
                    Statuses_ID = VindRitVM.StatusID
                };
                Response ok = Task.FromResult<Response>(await TripRepository.PostLocation(item)).Result;
                if (ok.Success == true)
                {

                    Bob.All bob = Task.FromResult<Bob.All>(await BobsRepository.GetBobById(MainViewVM.CurrentTrip.Bobs_ID)).Result;
                    Libraries.Socket socketSend = new Libraries.Socket() { From = MainViewVM.USER.ID, To = bob.User.ID, Status = true };
                    MainViewVM.socket.Emit("trip_UPDATE:send", JsonConvert.SerializeObject(socketSend));

                    BobisDone(location, "Trip is geannuleerd");
                }



                this.IsEnabledOffer = true;
                this.VisibleCancel = Visibility.Collapsed;
                this.VisibleOffer = Visibility.Collapsed;
                VindRitBobVM.Request -= 1;
                this.Loading = false;
                this.Status = null;
                SetStatus(0);

                RaiseAll();


        }

    }
}
