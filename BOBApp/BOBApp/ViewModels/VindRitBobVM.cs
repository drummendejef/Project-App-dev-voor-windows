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

        public Visibility VisibleModal { get; set; }
        public Visibility VisibleOffer { get; set; }
        public Visibility VisibleCancel { get; set; }
        public Frame Frame { get; set; }
        public string OfferText { get; set; }
        public string Status { get; set; }

        public Trip SelectedTrip { get; set; }
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

                if (this.SelectedTrip != null)
                {
                  
                    this.VisibleSelectedTrip = Visibility.Visible;
                    return SelectedTrip.Status_Name;
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
                Users_Destinations destination = await DestinationRepository.GetDestinationById(this.SelectedTrip.Destinations_ID);
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


            this.VisibleCancel = Visibility.Collapsed;
            this.VisibleOffer = Visibility.Collapsed;
            this.CanOffer = true;

            RaiseAll();
        }

        private void Cancel()
        {
            throw new NotImplementedException();
        }

      

        private void Arrived()
        {
            throw new NotImplementedException();
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
                        bob_accepted((bool)obj.Result, (float)obj.Data);
                        break;
                    case "newtrip_bob":
                        newtrip_bob((Trip)obj.Data);
                        break;
                    case "trip_location:reload":
                        Users_Destinations dest = Task.FromResult<Users_Destinations>(await DestinationRepository.GetDestinationById(VindRitVM.CurrentTrip.Destinations_ID)).Result;

                        //instellen voor timer, niet gebruiken in filter
                        VindRitFilterVM.SelectedDestination = dest;
                        if (VindRitFilterVM.SelectedDestination != null)
                        {
                            StartTripLocationTimer();
                        }
                        RaiseAll();
                        break;
                    default:
                        break;
                }
            }
        }

        private void newtrip_bob(Trip data)
        {
            this.VisibleCancel = Visibility.Visible;
            this.VisibleOffer = Visibility.Visible;
            this.SelectedTrip = data;
            RaiseAll();
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

        private void RaiseAll()
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


            RaisePropertyChanged("GetSelectedTrip");


            RaisePropertyChanged("VisibleSelectedTrip");
          
           
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
            this.Frame = new Frame();
            this.Frame.Navigate(typeof(VindRitFilter));
            this.Frame.Navigated += Frame_Navigated;

            VisibleModal = Visibility.Visible;
            RaiseAll();

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

        private async Task<bool> trip_location()
        {
           
            this.Loading = false;
            //todo: swtich
            RaiseAll();



            Geolocator geolocator = new Geolocator();
            Geoposition pos = await geolocator.GetGeopositionAsync();
            Location location = new Location() { Latitude = pos.Coordinate.Point.Position.Latitude, Longitude = pos.Coordinate.Point.Position.Longitude };


            Trips_Locations item = new Trips_Locations()
            {
                Trips_ID = VindRitVM.CurrentTrip.ID,
                Location = JsonConvert.SerializeObject(location),
                Statuses_ID = VindRitVM.StatusID
            };
            Response ok = Task.FromResult<Response>(await TripRepository.PostLocation(item)).Result;

            if (ok.Success == true)
            {
                StartTripLocationTimer();

                return true;
            }
            else
            {
                return false;
            }
        }


        #region  StartTripLocationTimer
        DispatcherTimer timer = new DispatcherTimer();
        bool canShowDialog;
        private void StartTripLocationTimer()
        {
            if (timer.IsEnabled == true)
            {
                timer.Stop();
                timer = new DispatcherTimer();

            }


            timer.Interval = new TimeSpan(0, 0, 20);
            timer.Tick += Timer_Tick;
            canShowDialog = true;
            timer.Start();
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
                    VindRitVM.StatusID = 3;
                    RaiseAll();
                    timer.Stop();


                    Trips_Locations item = new Trips_Locations()
                    {
                        Trips_ID = VindRitVM.CurrentTrip.ID,
                        Location = JsonConvert.SerializeObject(location),
                        Statuses_ID = VindRitVM.StatusID
                    };
                    Response ok = Task.FromResult<Response>(await TripRepository.PostLocation(item)).Result;
                    if (ok.Success == true)
                    {

                        Bob.All bob = Task.FromResult<Bob.All>(await BobsRepository.GetBobById(VindRitVM.CurrentTrip.Bobs_ID)).Result;
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

                        bool done = Task.FromResult<bool>(await OnDestination()).Result;
                        if (done == true)
                        {
                            BobisDone(location, "Trip is afgerond");
                        }
                    }


                }
                else
                {
                    //VindRitFilterVM.SelectedDestination.Location;
                    Trips_Locations item = new Trips_Locations()
                    {
                        Trips_ID = VindRitVM.CurrentTrip.ID,
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
            VindRitVM.StatusID = 4;
            RaiseAll();

            Trips_Locations item = new Trips_Locations()
            {
                Trips_ID = VindRitVM.CurrentTrip.ID,
                Location = JsonConvert.SerializeObject(location),
                Statuses_ID = VindRitVM.StatusID
            };
            Response ok = Task.FromResult<Response>(await TripRepository.PostLocation(item)).Result;
            Response active = Task.FromResult<Response>(await TripRepository.PutActive(item.Trips_ID, false)).Result;

            if (ok.Success == true && active.Success == true)
            {
                var dialog = new MessageDialog(text);

                dialog.Commands.Add(new UICommand("Ok") { Id = 0 });



                dialog.DefaultCommandIndex = 0;

                var result = await dialog.ShowAsync();

                int id = int.Parse(result.Id.ToString());
                if (id == 0)
                {


                    Bob.All bob = Task.FromResult<Bob.All>(await BobsRepository.GetBobById(VindRitVM.CurrentTrip.Bobs_ID)).Result;



                    Libraries.Socket socketSendToUser = new Libraries.Socket()
                    {
                        To = MainViewVM.USER.ID,
                        Status = true
                    };


                    MainViewVM.socket.Emit("trip_DONE:send", JsonConvert.SerializeObject(socketSendToUser));

                    canShowDialog = true;

                }

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



    }
}
