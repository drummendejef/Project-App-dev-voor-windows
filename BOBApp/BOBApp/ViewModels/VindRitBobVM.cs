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
using Windows.UI.Xaml.Controls.Maps;
using Windows.Services.Maps;
using Windows.UI;

namespace BOBApp.ViewModels
{
    public class VindRitBobVM : ViewModelBase
    {
        #region props
        #region static

        public static int Request { get; set; }
        public static float FindID { get; set; }

        public List<Bob> BobList { get; set; }
        


        #endregion

        public bool Loading { get; set; }
        public string Error { get; set; }
        DispatcherTimer timer = MainViewVM.TIMER;

        public Visibility VisibleModal { get; set; }
        public Visibility VisibleOffer { get; set; }
        public Visibility VisibleCancel { get; set; }
        public bool IsEnabledOffer { get; set; }
        public bool IsEnabledArrived { get; set; }
        public bool IsEnabledCancel { get; set; }
        public Frame Frame { get; set; }
        public string OfferText { get; set; }
        public string Status { get; set; }
        public Trip.All CurrentTrip { get; set; }
        public List<Party> Parties { get; set; }
        public List<User.All> Users { get; set; }
        public List<Bob.All> Bobs { get; set; }

        public MapControl Map { get; set; }

        public List<User> UserRequest { get; set; }

        private bool _CanOffer;

        public bool CanOffer
        {
            get { return _CanOffer; }
            set
            {
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



        #endregion

        //others
        public RelayCommand ArrivedCommand { get; set; }
        public RelayCommand ShowModalCommand { get; set; }
        public RelayCommand CloseModalCommand { get; set; }
        public RelayCommand GoChatCommand { get; set; }

        public RelayCommand CancelCommand { get; set; }
        public string BobRequests { get; set; }
        public string RitTime { get; set; }
        public Party SelectedParty { get; private set; }

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

            this.IsEnabledArrived = true;
            this.IsEnabledCancel = true;
            this.IsEnabledOffer = true;
            this.VisibleCancel = Visibility.Collapsed;
            this.VisibleOffer = Visibility.Collapsed;
            this.CanOffer = false;



            RaiseAll();
        }

        private void Cancel()
        {
            this.IsEnabledCancel = false;
            RaiseAll(); ;
            CancelTrip();
        }


        bool ToDestination = false;
        private async void Arrived()
        {
            this.IsEnabledArrived = false;
            this.Loading = true;
            RaiseAll();
            Location location = await LocationService.GetCurrent();
            Response farEnough = Task.FromResult<Response>(await TripRepository.Difference((Location)this.SelectedParty.Location, location)).Result;

            if(farEnough.Success && farEnough.Value != null)
            {
                if (double.Parse(farEnough.Value.ToString()) < 600)
                {
                    //bij het feestje
                    SetStatus(8);
                    ToDestination = true;
                    this.IsEnabledArrived = true;
                    this.Loading = false;
                    
                  
                }
                else
                {
                    SetStatus(4);

                    RaiseAll();
                    if (timer != null)
                    {
                        timer.Stop();
                    }

                    BobisDone("Trip is afgerond");

                }
            }

            RaiseAll();



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
                            await UserRepository.PostPoint(3);

                            var data = (Trip)obj.Data;
                            data.StatusID = 2;
                            SetStatus(2);

                            this.Loading = false;
                            RaiseAll();

                            newtrip_bob(data);
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
            if (data != null || MainViewVM.CurrentTrip == null)
            {
                
               

                this.IsEnabledOffer = false;
                this.VisibleCancel = Visibility.Visible;
                this.VisibleOffer = Visibility.Visible;
                MainViewVM.CurrentTrip = data;
                
                VindRitBobVM.Request = 1;
                SetStatus(data.StatusID.Value);


                Location location = await LocationService.GetCurrent();
                Party party = Task.FromResult<Party>(await PartyRepository.GetPartyById(MainViewVM.CurrentTrip.Party_ID)).Result;
                Users_Destinations destination = Task.FromResult<Users_Destinations>(await DestinationRepository.GetDestinationById(MainViewVM.CurrentTrip.Destinations_ID)).Result;

                VindRitFilterVM.SelectedDestination = destination;
                this.SelectedParty = party;
              

                Response farEnough = Task.FromResult<Response>(await TripRepository.Difference((Location)this.SelectedParty.Location, location)).Result;

                if (farEnough.Success && farEnough.Value != null)
                {
                    if (double.Parse(farEnough.Value.ToString()) < 600)
                    {
                        //bij het feestje

                        ShowRoute((Location)party.Location, (Location)destination.Location);
                        SetStatus(8);
                        ToDestination = true;
                    }
                    else
                    {
                        ShowRoute(location, (Location)party.Location);
                       
                    }
                }


                trip_location();


                //trip.all invullen
                if (MainViewVM.CurrentTrip != null)
                {
                    Trip.All trips_all = new Trip.All();


                    User.All user = await UsersRepository.GetUserById(MainViewVM.CurrentTrip.Users_ID);
                    Bob.All bob = await BobsRepository.GetBobById(MainViewVM.CurrentTrip.Bobs_ID);
                    

                    Trip.All newTrip = new Trip.All();



                    newTrip.Trip = MainViewVM.CurrentTrip;
                    newTrip.Party = party;
                    newTrip.User = user;
                    newTrip.Bob = bob;
                    newTrip.Destination = destination;

                    this.CurrentTrip = newTrip;
                }
               


                RaiseAll();

            }

        }

        private async void bob_accepted(bool accepted, float id)
        {

            VindRitBobVM.FindID = id;
            this.Loading = false;
            RaiseAll();

            if (accepted == true)
            {
                SetStatus(1);

                //verstuur trip
                User.All user = Task.FromResult<User.All>(await UsersRepository.GetUserById(VindRitVM.SelectedUser.ID)).Result;
                Libraries.Socket socketSend = new Libraries.Socket()
                {
                    From = MainViewVM.USER.ID,//from bob
                    To = user.User.ID,//to user
                    Status = true,
                    Object = user,
                    ID = id
                };
               

                MainViewVM.socket.Emit("trip_MAKE:send", JsonConvert.SerializeObject(socketSend));
                this.Loading = true;
                RaiseAll();
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
                RaisePropertyChanged("UserRequests");
                RaisePropertyChanged("OfferText");
                RaisePropertyChanged("RitTime");
                RaisePropertyChanged("CanOffer");
                RaisePropertyChanged("IsEnabledOffer");
                RaisePropertyChanged("IsEnabledArrived");
                RaisePropertyChanged("IsEnabledCancel");
                RaisePropertyChanged("CurrentTrip");
                RaisePropertyChanged("Parties");
                RaisePropertyChanged("Users");
                RaisePropertyChanged("Bobs");


                RaisePropertyChanged("Status");


            });
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        }

        private async void Loaded()
        {
            this.Loading = true;
            RaiseAll();
           



            await Task.Run(async () =>
            {
                // running in background
#pragma warning disable CS1998
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    //default on start

                    VisibleModal = Visibility.Collapsed;

                  
                 
                    await GetParties();
                    await GetUsers();
                    await GetBobs();

                    this.BobRequests = "Momenteel " + VindRitBobVM.Request.ToString() + " aanvragen";

                    await GetCurrentTrip();

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
            RaiseAll();

            if (MainViewVM.CurrentTrip != null)
            {
                if (LocationService.LastLocation != null && statusID!=0)
                {
                    Trips_Locations item = new Trips_Locations()
                    {
                        Trips_ID = MainViewVM.CurrentTrip.ID,
                        Location = JsonConvert.SerializeObject(LocationService.LastLocation),
                        Statuses_ID = VindRitVM.StatusID
                    };
                    Response ok = Task.FromResult<Response>(await TripRepository.PostLocation(item)).Result;

                    if (ok.Success == true)
                    {
                        Libraries.Socket socketSend = new Libraries.Socket()
                        {
                            From = MainViewVM.USER.ID,//from bob
                            To = MainViewVM.CurrentTrip.Users_ID,//to user
                            Status = true,
                            ID = VindRitBobVM.FindID
                        };


                        MainViewVM.socket.Emit("status_UPDATE:send", JsonConvert.SerializeObject(socketSend));
                    }
                }
               


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
                    return "Wachter op gebruiker";
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
                case 8:
                    return "Bob is bij het feestje";
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
            this.Loading = false;
 
            StartTripLocationTimer();
            this.Map.MapElements.Clear();
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
            Location location = await LocationService.GetCurrent();

            

            if (location != null)
            {
                Party party = Task.FromResult<Party>(await PartyRepository.GetPartyById(MainViewVM.CurrentTrip.Party_ID)).Result;

                if (ToDestination == true)
                {
                    ShowRoute((Location)party.Location, (Location)VindRitFilterVM.SelectedDestination.Location);
                }
                else
                {
                    ShowRoute(location, (Location)party.Location);
                }
               
                //checkhowfaraway



                Response farEnough = Task.FromResult<Response>(await TripRepository.Difference((Location)VindRitFilterVM.SelectedDestination.Location, location)).Result;

                if (farEnough.Success == true)
                {
                    //kleiner dan 1km
                   

                    SetStatus(7);
                    RaiseAll();
                    timer.Stop();

                    if (canShowDialog == true)
                    {
                        canShowDialog = false;

                        bool done = Task.FromResult<bool>(await OnDestination()).Result;
                        if (done == true)
                        {
                            SetStatus(3);
                            timer.Stop();
                            RaiseAll();

                            BobisDone("Trip is afgerond");
                            SetStatus(4);
                            RaiseAll();

                        }
                        else
                        {
                            timer.Start();
                        }
                    }


                }
                else
                {
                    SetStatus(2);
                    RaiseAll();
                }


            }
        }

        private async void BobisDone(string text)
        {
            this.Map.MapElements.Clear();

            Bobs_Parties bobs_parties = new Bobs_Parties()
            {
                Bobs_ID = MainViewVM.CurrentTrip.Bobs_ID,
                Party_ID = MainViewVM.CurrentTrip.Party_ID,
                Trips_ID = MainViewVM.CurrentTrip.ID,
                Users_ID = MainViewVM.CurrentTrip.Users_ID

            };


            Response active = Task.FromResult<Response>(await TripRepository.PutActive(MainViewVM.CurrentTrip.ID, false)).Result;

            if (active.Success == true)
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
                    To = MainViewVM.CurrentTrip.Users_ID,
                    Status = true,
                    Object = JsonConvert.SerializeObject(bobs_parties),
                    Object2 = true
                };

                await UserRepository.PostPoint();

                MainViewVM.socket.Emit("trip_DONE:send", JsonConvert.SerializeObject(socketSendToUser));
                MainViewVM.socket.Emit("trip_DONE:send", JsonConvert.SerializeObject(socketSendToBob));


                var dialog = new MessageDialog(text);
                dialog.Commands.Add(new UICommand("Ok") { Id = 0 });
                dialog.DefaultCommandIndex = 0;
                var result = await dialog.ShowAsync();
                int id = int.Parse(result.Id.ToString());

                this.IsEnabledArrived = true;
                this.IsEnabledCancel = true;

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
                            this.Status = GetStatusName(data.ID);
                            newtrip_bob(data);
                            RaiseAll();
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
                this.Status = GetStatusName(MainViewVM.CurrentTrip.ID);

                newtrip_bob(MainViewVM.CurrentTrip);


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


            Bob.All bob = Task.FromResult<Bob.All>(await BobsRepository.GetBobById(MainViewVM.CurrentTrip.Bobs_ID)).Result;
            Libraries.Socket socketSend = new Libraries.Socket() { From = MainViewVM.USER.ID, To = bob.User.ID, Status = true };
            MainViewVM.socket.Emit("trip_UPDATE:send", JsonConvert.SerializeObject(socketSend));

            BobisDone("Trip is geannuleerd");



            this.IsEnabledOffer = true;
            this.VisibleCancel = Visibility.Collapsed;
            this.VisibleOffer = Visibility.Collapsed;
            VindRitBobVM.Request -= 1;
            this.Loading = false;
            this.Status = null;
            SetStatus(0);

            RaiseAll();


        }
        private async void ShowRoute(Location from, Location to)
        {
            try
            {
                if ((App.Current as App).UserLocation != null)
                {
                    ClearAllMapItems();

                    this.Map.Routes.Clear();


                    //Tijdelijke locatie aanmaken
                    BasicGeoposition tempFrom = new BasicGeoposition();
                    tempFrom.Longitude = from.Longitude;
                    tempFrom.Latitude = from.Latitude;

                    BasicGeoposition tempTo = new BasicGeoposition();
                    tempTo.Longitude = to.Longitude;
                    tempTo.Latitude = to.Latitude;

                    //Start en eindpunt ophalen en klaarzetten voor onderstaande vraag
                    Geopoint startpunt = new Geopoint(tempFrom);
                    Geopoint eindpunt = new Geopoint(tempTo);
                  

                    //De route tussen 2 punten opvragen
                    MapRouteFinderResult routeResult = await MapRouteFinder.GetDrivingRouteAsync(startpunt, eindpunt);


                  

                    if (routeResult.Status == MapRouteFinderStatus.Success)//Het is gelukt, we hebben een antwoord gekregen.
                    {
                        MapRouteView viewOfRoute = new MapRouteView(routeResult.Route);
                        viewOfRoute.RouteColor = Color.FromArgb(255, 62, 94, 148);

                        this.Map.Routes.Add(viewOfRoute);

                        //Fit de mapcontrol op de route
                        await this.Map.TrySetViewBoundsAsync(routeResult.Route.BoundingBox, null, Windows.UI.Xaml.Controls.Maps.MapAnimationKind.Bow);
                    }

                   
                   

                }
            }
            catch (Exception ex)
            {


            }
        }

        #region bind events
        public void MapLoaded(object sender, RoutedEventArgs e)//Als de map geladen is.
        {
            if ((App.Current as App).UserLocation != null)
            {

                //Map centreren op huidige locatie
                this.Map.Center = (App.Current as App).UserLocation.Coordinate.Point;//De userpoint ophalen, en de map hier op centreren.
                this.Map.ZoomLevel = 15;//Inzoomlevel instellen (hoe groter het getal, hoe dichterbij)
                this.Map.LandmarksVisible = true;

                //Marker voor eigen locatie plaatsen
                MapIcon mapIconUserLocation = new MapIcon();
                mapIconUserLocation.Location = this.Map.Center;
                mapIconUserLocation.NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1.0);//Verzet het icoontje, zodat de punt van de marker staat op waar de locatie is. (anders zou de linkerbovenhoek op de locatie staan) 
                mapIconUserLocation.Title = "Ik";//Titel die boven de marker komt.
                mapIconUserLocation.Image = MainViewVM.Pins.BobPin;
                this.Map.MapElements.Add(mapIconUserLocation);//Marker op de map zetten.
            }



        }

        #region maps
        private async Task GetParties()
        {

            this.Parties = await PartyRepository.GetParties();

            if (this.Parties != null)
            {
                for (int i = 0; i < this.Parties.Count(); i++)
                {
                    try
                    {
                        Party item = this.Parties[i];
                        if (item.Location == null)
                        {
                            break;
                        }
                        item.VisibleShow = Visibility.Collapsed;
                        item.ShowCommand = new RelayCommand<object>(e => ShowParty(e));
                        item.RouteCommand = new RelayCommand<object>(e => mapItem_Party(e));
                       
                        item.RouteCommandText = "Toon route";
                        if (MainViewVM.USER.IsBob.Value)
                        {
                            item.VisibleTake = Visibility.Collapsed;
                        }
                        else
                        {
                            item.VisibleTake = Visibility.Visible;
                        }


                        BasicGeoposition tempbasic = new BasicGeoposition();
                        //Locaties omzetten en in de tijdelijke posities opslaan.
                        tempbasic.Latitude = ((Location)item.Location).Latitude;
                        tempbasic.Longitude = ((Location)item.Location).Longitude;

                        //Omzetten van tijdelijk punt naar echte locatie (anders krijg je die niet in de mapIconFeestLocation.Location)
                        Geopoint temppoint = new Geopoint(tempbasic);

                        MapIcon mapIconFeestLocation = new MapIcon();
                        mapIconFeestLocation.Location = temppoint; //Opgehaalde locatie
                                                                   //mapIconFeestLocation.Title = feest.Name; //Naam van het feestje;
                        mapIconFeestLocation.Image = MainViewVM.Pins.FeestPin;
                        mapIconFeestLocation.Title = item.Name;
                        this.Map.MapElements.Add(mapIconFeestLocation);//Marker op de map zetten.

                    }
                    catch (Exception ex)
                    {


                    }

                }


                var newControl = new MapItemsControl();
                VindRitBob vindrit = MainViewVM.MainFrame.Content as VindRitBob;

                newControl.ItemsSource = Parties;
                newControl.ItemTemplate = (DataTemplate)vindrit.Resources["PartiesMapTemplate"] as DataTemplate;

                AddOrUpdateChild(newControl);


                RaiseAll();
            }





        }

        private void ShowParty(object obj)
        {
            var item = obj as Party;
            if (item.VisibleShow == Visibility.Collapsed)
            {
                item.VisibleShow = Visibility.Visible;

            }
            else
            {
                item.VisibleShow = Visibility.Collapsed;

            }
            UpdateMapOfType(typeof(List<Party>));

            RaiseAll();

        }
      


        private async Task GetBobs()
        {
            this.Bobs = await BobsRepository.GetBobsOnline();

            //Try catch errond om te zorgen dat hij niet crasht bij lege bobs.
            if (this.Bobs != null)
            {
                for (int i = 0; i < this.Bobs.Count(); i++)
                {
                    try
                    {
                        Bob.All item = this.Bobs[i];
                        if (item.Location == null)
                        {
                            break;
                        }

                        item.VisibleShow = Visibility.Collapsed;
                        item.ShowCommand = new RelayCommand<object>(e => ShowBob(e));
                        item.RouteCommand = new RelayCommand<object>(e => mapItem_Bob(e));

                        item.RouteCommandText = "Toon route";

                        BasicGeoposition tempbasic = new BasicGeoposition();
                        //Locaties omzetten en in de tijdelijke posities opslaan.
                        tempbasic.Latitude = ((Location)item.Location).Latitude;
                        tempbasic.Longitude = ((Location)item.Location).Longitude;

                        //Omzetten van tijdelijk punt naar echte locatie (anders krijg je die niet in de mapIconFeestLocation.Location)
                        Geopoint temppoint = new Geopoint(tempbasic);

                        MapIcon mapIconFeestLocation = new MapIcon();
                        mapIconFeestLocation.Location = temppoint; //Opgehaalde locatie
                                                                   //mapIconFeestLocation.Title = feest.Name; //Naam van het feestje;
                        mapIconFeestLocation.Image = MainViewVM.Pins.FeestPin;
                        mapIconFeestLocation.Title = item.User.ToString();
                        this.Map.MapElements.Add(mapIconFeestLocation);//Marker op de map zetten.
                    }
                    catch (Exception ex)
                    {


                    }

                }

                var newControl = new MapItemsControl();
                VindRitBob vindrit = MainViewVM.MainFrame.Content as VindRitBob;

                newControl.ItemsSource = this.Bobs;
                newControl.ItemTemplate = (DataTemplate)vindrit.Resources["BobsMapTemplate"] as DataTemplate;

                AddOrUpdateChild(newControl);


                RaiseAll();
            }


        }

        private void ShowBob(object obj)
        {
            var item = obj as Bob.All;
            if (item.VisibleShow == Visibility.Collapsed)
            {
                item.VisibleShow = Visibility.Visible;

            }
            else
            {
                item.VisibleShow = Visibility.Collapsed;

            }
            UpdateMapOfType(typeof(List<Bob.All>));

            RaiseAll();

        }


        private async Task GetUsers()
        {
            this.Users = await UsersRepository.GetUsersOnline();

            //Try catch errond om te zorgen dat hij niet crasht bij lege bobs.
            if (this.Users != null)
            {
                for (int i = 0; i < this.Users.Count(); i++)
                {
                    try
                    {
                        User.All item = this.Users[i];
                        if (item.Location == null)
                        {
                            break;
                        }

                        item.VisibleShow = Visibility.Collapsed;
                        item.ShowCommand = new RelayCommand<object>(e => ShowUser(e));
                        item.RouteCommand = new RelayCommand<object>(e => mapItem_User(e));
                        item.RouteCommandText = "Toon route";

                        BasicGeoposition tempbasic = new BasicGeoposition();
                        //Locaties omzetten en in de tijdelijke posities opslaan.
                        tempbasic.Latitude = ((Location)item.Location).Latitude;
                        tempbasic.Longitude = ((Location)item.Location).Longitude;

                        //Omzetten van tijdelijk punt naar echte locatie (anders krijg je die niet in de mapIconFeestLocation.Location)
                        Geopoint temppoint = new Geopoint(tempbasic);

                        MapIcon mapIconFeestLocation = new MapIcon();
                        mapIconFeestLocation.Location = temppoint; //Opgehaalde locatie
                                                                   //mapIconFeestLocation.Title = feest.Name; //Naam van het feestje;
                        mapIconFeestLocation.Image = MainViewVM.Pins.FeestPin;
                        mapIconFeestLocation.Title = item.User.ToString();
                        this.Map.MapElements.Add(mapIconFeestLocation);//Marker op de map zetten.
                    }
                    catch (Exception ex)
                    {


                    }

                }

                var newControl = new MapItemsControl();
                VindRitBob vindrit = MainViewVM.MainFrame.Content as VindRitBob;

                newControl.ItemsSource = this.Users;
                newControl.ItemTemplate = (DataTemplate)vindrit.Resources["UsersMapTemplate"] as DataTemplate;



                AddOrUpdateChild(newControl);




                RaiseAll();


            }


        }

        private void AddOrUpdateChild(MapItemsControl newControl)
        {
            bool done = false;
            foreach (var itemChild in this.Map.Children)
            {
                var control = itemChild as MapItemsControl;
                if (control.ItemsSource != null)
                {
                    if (control.ItemsSource.GetType() == newControl.ItemsSource.GetType())
                    {
                        if (newControl.ItemsSource.GetType() == typeof(List<Party>))
                        {
                            control.ItemsSource = null;
                            control.ItemsSource = this.Parties;
                            done = true;
                        }

                        if (newControl.ItemsSource.GetType() == typeof(List<User.All>))
                        {
                            control.ItemsSource = null;
                            control.ItemsSource = this.Users;
                            done = true;
                        }
                        if (newControl.ItemsSource.GetType() == typeof(List<Bob.All>))
                        {
                            control.ItemsSource = null;
                            control.ItemsSource = this.Bobs;
                            done = true;
                        }

                    }

                }

            }

            if (done == false)
            {
                this.Map.Children.Add(newControl);
            }

        }

        private void ShowUser(object obj)
        {
            var item = obj as User.All;
            if (item.VisibleShow == Visibility.Collapsed)
            {
                item.VisibleShow = Visibility.Visible;

            }
            else
            {
                item.VisibleShow = Visibility.Collapsed;

            }
            UpdateMapOfType(typeof(List<User.All>));

            RaiseAll();

        }
        private void ClearAllMapItems()
        {
            try
            {
                foreach (var item in this.Parties)
                {
                    item.VisibleShow = Visibility.Collapsed;
                }
                foreach (var item in this.Users)
                {
                    item.VisibleShow = Visibility.Collapsed;
                }
                foreach (var item in this.Bobs)
                {
                    item.VisibleShow = Visibility.Collapsed;
                }
                RaiseAll();

                foreach (var itemChild in this.Map.Children)
                {
                    var control = itemChild as MapItemsControl;
                    if (control.ItemsSource != null)
                    {
                        if (control.ItemsSource.GetType() == typeof(List<Party>))
                        {
                            control.ItemsSource = null;
                            control.ItemsSource = this.Parties;
                        }

                        if (control.ItemsSource.GetType() == typeof(List<User.All>))
                        {
                            control.ItemsSource = null;
                            control.ItemsSource = this.Users;
                        }
                        if (control.ItemsSource.GetType() == typeof(List<Bob.All>))
                        {
                            control.ItemsSource = null;
                            control.ItemsSource = this.Bobs;
                        }
                    }

                }
            }
            catch (Exception ex)
            {


            }

        }
        private void UpdateMapOfType(Type type)
        {
            foreach (var itemChild in this.Map.Children)
            {
                var control = itemChild as MapItemsControl;
                if (control.ItemsSource != null)
                {
                    if (control.ItemsSource.GetType() == type)
                    {
                        if (type == typeof(List<Party>))
                        {
                            control.ItemsSource = null;
                            control.ItemsSource = this.Parties;
                        }

                        if (type == typeof(List<User.All>))
                        {
                            control.ItemsSource = null;
                            control.ItemsSource = this.Users;
                        }
                        if (type == typeof(List<Bob.All>))
                        {
                            control.ItemsSource = null;
                            control.ItemsSource = this.Bobs;
                        }

                    }
                }

            }
        }

        public async void mapItem_Party(object param)
        {
            if ((App.Current as App).UserLocation != null)
            {

                //Locatie uit gekozen feestje halen.
                Party item = param as Party;

                //Tijdelijke locatie aanmaken
                try
                {
                    BasicGeoposition tempbasic = new BasicGeoposition();

                    //Feestlocatie opsplitsen (word opgeslagen als string)
                    Location location = (Location)item.Location;

                    //Locaties omzetten en in de tijdelijke posities opslaan.
                    tempbasic.Latitude = location.Latitude;
                    tempbasic.Longitude = location.Longitude;

                    //Om de route aan te vragen, heb je een start en een eindpunt nodig. Die moeten er zo uit zien: "waypoint.1=47.610,-122.107".
                    //We gaan deze zelf aanmaken.
                    /*string startstring = "http://dev.virtualearth.net/REST/v1/Routes?wayPoint.1=";//Eerste deel van de url
                    startstring += (App.Current as App).UserLocation.Coordinate.Point.Position.Latitude.ToString() + "," + (App.Current as App).UserLocation.Coordinate.Point.Position.Longitude.ToString();
                    startstring += "&waypoint.2=";//Start van het eindpunt
                    startstring += tempbasic.Latitude.ToString() + "," + tempbasic.Longitude.ToString();//Endpoint
                    startstring += URL.URLBINGKEY + URL.BINGKEY;*/

                    Geopoint startpunt;
                    //Start en eindpunt ophalen en klaarzetten voor onderstaande vraag
                    if (VindRitFilterVM.SelectedDestination != null)
                    {
                        BasicGeoposition tempDest = new BasicGeoposition();
                        tempDest.Latitude = ((Location)VindRitFilterVM.SelectedDestination.Location).Latitude;
                        tempDest.Longitude = ((Location)VindRitFilterVM.SelectedDestination.Location).Longitude;
                        startpunt = new Geopoint(tempDest);
                    }
                    else
                    {
                        startpunt = (App.Current as App).UserLocation.Coordinate.Point;
                    }

                    Geopoint eindpunt = new Geopoint(tempbasic);

                    //De route tussen 2 punten opvragen
                    MapRouteFinderResult routeResult = await MapRouteFinder.GetDrivingRouteAsync(startpunt, eindpunt);

                    if (routeResult.Status == MapRouteFinderStatus.Success)//Het is gelukt, we hebben een antwoord gekregen.
                    {
                        MapRouteView viewOfRoute = new MapRouteView(routeResult.Route);
                        viewOfRoute.RouteColor = Color.FromArgb(255, 62, 94, 148);

                        if (item.RouteCommandText == "Toon route")
                        {
                            this.Map.Routes.Clear();
                        }

                        var items = this.Map.Routes;
                        if (items.Count() > 0)
                        {
                            item.RouteCommandText = "Toon route";
                            this.Map.Routes.Clear();
                        }
                        else
                        {
                            item.RouteCommandText = "Clear route";
                            this.Map.Routes.Add(viewOfRoute);
                        }

                        //MapRouteView toevoegen aan de Route Collectie


                        //Fit de mapcontrol op de route
                        await this.Map.TrySetViewBoundsAsync(routeResult.Route.BoundingBox, null, Windows.UI.Xaml.Controls.Maps.MapAnimationKind.Bow);
                    }
                }
                catch (Exception ex)
                {


                }
            }

            UpdateMapOfType(typeof(List<Party>));
        }

        public async void mapItem_User(object param)
        {
            if ((App.Current as App).UserLocation != null)
            {

                //Locatie uit gekozen feestje halen.
                User.All item = param as User.All;

                //Tijdelijke locatie aanmaken
                try
                {
                    BasicGeoposition tempbasic = new BasicGeoposition();

                    //Feestlocatie opsplitsen (word opgeslagen als string)
                    Location location = (Location)item.Location;

                    //Locaties omzetten en in de tijdelijke posities opslaan.
                    tempbasic.Latitude = location.Latitude;
                    tempbasic.Longitude = location.Longitude;

                    //Om de route aan te vragen, heb je een start en een eindpunt nodig. Die moeten er zo uit zien: "waypoint.1=47.610,-122.107".
                    //We gaan deze zelf aanmaken.
                    /*string startstring = "http://dev.virtualearth.net/REST/v1/Routes?wayPoint.1=";//Eerste deel van de url
                    startstring += (App.Current as App).UserLocation.Coordinate.Point.Position.Latitude.ToString() + "," + (App.Current as App).UserLocation.Coordinate.Point.Position.Longitude.ToString();
                    startstring += "&waypoint.2=";//Start van het eindpunt
                    startstring += tempbasic.Latitude.ToString() + "," + tempbasic.Longitude.ToString();//Endpoint
                    startstring += URL.URLBINGKEY + URL.BINGKEY;*/

                    Geopoint startpunt;
                    //Start en eindpunt ophalen en klaarzetten voor onderstaande vraag
                    if (VindRitFilterVM.SelectedDestination != null)
                    {
                        BasicGeoposition tempDest = new BasicGeoposition();
                        tempDest.Latitude = ((Location)VindRitFilterVM.SelectedDestination.Location).Latitude;
                        tempDest.Longitude = ((Location)VindRitFilterVM.SelectedDestination.Location).Longitude;
                        startpunt = new Geopoint(tempDest);
                    }
                    else
                    {
                        startpunt = (App.Current as App).UserLocation.Coordinate.Point;
                    }
                    Geopoint eindpunt = new Geopoint(tempbasic);
                    //De route tussen 2 punten opvragen
                    MapRouteFinderResult routeResult = await MapRouteFinder.GetDrivingRouteAsync(startpunt, eindpunt);

                    if (routeResult.Status == MapRouteFinderStatus.Success)//Het is gelukt, we hebben een antwoord gekregen.
                    {
                        MapRouteView viewOfRoute = new MapRouteView(routeResult.Route);
                        viewOfRoute.RouteColor = Color.FromArgb(255, 62, 94, 148);

                        if (item.RouteCommandText == "Toon route")
                        {
                            this.Map.Routes.Clear();
                        }

                        var items = this.Map.Routes;
                        if (items.Count() > 0)
                        {
                            item.RouteCommandText = "Toon route";
                            this.Map.Routes.Clear();
                        }
                        else
                        {
                            item.RouteCommandText = "Clear route";
                            this.Map.Routes.Add(viewOfRoute);
                        }

                        //MapRouteView toevoegen aan de Route Collectie


                        //Fit de mapcontrol op de route
                        await this.Map.TrySetViewBoundsAsync(routeResult.Route.BoundingBox, null, Windows.UI.Xaml.Controls.Maps.MapAnimationKind.Bow);
                    }
                }
                catch (Exception ex)
                {


                }
            }

            UpdateMapOfType(typeof(List<User.All>));
        }

        public async void mapItem_Bob(object param)
        {
            if ((App.Current as App).UserLocation != null)
            {

                //Locatie uit gekozen feestje halen.
                Bob.All item = param as Bob.All;

                //Tijdelijke locatie aanmaken
                try
                {
                    BasicGeoposition tempbasic = new BasicGeoposition();

                    //Feestlocatie opsplitsen (word opgeslagen als string)
                    Location location = (Location)item.Location;

                    //Locaties omzetten en in de tijdelijke posities opslaan.
                    tempbasic.Latitude = location.Latitude;
                    tempbasic.Longitude = location.Longitude;

                    //Om de route aan te vragen, heb je een start en een eindpunt nodig. Die moeten er zo uit zien: "waypoint.1=47.610,-122.107".
                    //We gaan deze zelf aanmaken.
                    /*string startstring = "http://dev.virtualearth.net/REST/v1/Routes?wayPoint.1=";//Eerste deel van de url
                    startstring += (App.Current as App).UserLocation.Coordinate.Point.Position.Latitude.ToString() + "," + (App.Current as App).UserLocation.Coordinate.Point.Position.Longitude.ToString();
                    startstring += "&waypoint.2=";//Start van het eindpunt
                    startstring += tempbasic.Latitude.ToString() + "," + tempbasic.Longitude.ToString();//Endpoint
                    startstring += URL.URLBINGKEY + URL.BINGKEY;*/

                    Geopoint startpunt;
                    //Start en eindpunt ophalen en klaarzetten voor onderstaande vraag
                    if (VindRitFilterVM.SelectedDestination != null)
                    {
                        BasicGeoposition tempDest = new BasicGeoposition();
                        tempDest.Latitude = ((Location)VindRitFilterVM.SelectedDestination.Location).Latitude;
                        tempDest.Longitude = ((Location)VindRitFilterVM.SelectedDestination.Location).Longitude;
                        startpunt = new Geopoint(tempDest);
                    }
                    else
                    {
                        startpunt = (App.Current as App).UserLocation.Coordinate.Point;
                    }
                    Geopoint eindpunt = new Geopoint(tempbasic);
                    //De route tussen 2 punten opvragen
                    MapRouteFinderResult routeResult = await MapRouteFinder.GetDrivingRouteAsync(startpunt, eindpunt);

                    if (routeResult.Status == MapRouteFinderStatus.Success)//Het is gelukt, we hebben een antwoord gekregen.
                    {
                        MapRouteView viewOfRoute = new MapRouteView(routeResult.Route);
                        viewOfRoute.RouteColor = Color.FromArgb(255, 62, 94, 148);

                        if (item.RouteCommandText == "Toon route")
                        {
                            this.Map.Routes.Clear();
                        }

                        var items = this.Map.Routes;
                        if (items.Count() > 0)
                        {
                            item.RouteCommandText = "Toon route";
                            this.Map.Routes.Clear();
                        }
                        else
                        {
                            item.RouteCommandText = "Clear route";
                            this.Map.Routes.Add(viewOfRoute);
                        }

                        //MapRouteView toevoegen aan de Route Collectie


                        //Fit de mapcontrol op de route
                        await this.Map.TrySetViewBoundsAsync(routeResult.Route.BoundingBox, null, Windows.UI.Xaml.Controls.Maps.MapAnimationKind.Bow);
                    }
                }
                catch (Exception ex)
                {


                }
            }

            UpdateMapOfType(typeof(List<Bob.All>));
        }

        #endregion


        #endregion



    }
}
