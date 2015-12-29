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
    public class VindRitVM : ViewModelBase
    {
        #region props
        #region static
        public static Party SelectedParty { get; set; }

        public static Bob.All SelectedBob { get; set; }
        public static User SelectedUser { get; set; }
        public static Trip CurrentTrip { get; set; }
        public static bool BobAccepted { get; set; }
        public static int StatusID { get; set; }
        public static int FindID { get; set; }



        public List<Bob.All> BobList { get; set; }


        #endregion

        public bool Loading { get; set; }
        public string Error { get; set; }
        public Visibility VisibleFind { get; set; }
        public Visibility VisibleCancel { get; set; }
        public Visibility VisibleChat { get; set; }
        public Visibility VisibleFilterContext { get; set; }
        public Visibility VisibleModal { get; set; }

       
        public Frame Frame { get; set; }
     
        public string CancelText { get; set; }

        #region gets

        public Visibility VisibleSelectedFriends { get; set; }
        public string GetSelectedFriendsString
        {
            get
            {

                if (VindRitFilterVM.SelectedFriends != null)
                {
                    string friends = "";
                    for (int i = 0; i < VindRitFilterVM.SelectedFriends.Count; i++)
                    {
                        if (VindRitFilterVM.SelectedFriends[i].User1.ID != MainViewVM.USER.ID)
                        {
                            friends += VindRitFilterVM.SelectedFriends[i].User1.ToString() + " - ";
                        }
                    }
                    this.VisibleSelectedFriends = Visibility.Visible;
                    return friends;
                }
                else
                {
                    this.VisibleSelectedFriends = Visibility.Collapsed;
                    return "";
                }

            }
        }
        
        public Visibility VisibleSelectedDestination { get; set; }
        public Users_Destinations GetSelectedDestination
        {
            get
            {
                if (VindRitFilterVM.SelectedDestination != null)
                {
                    this.VisibleSelectedDestination = Visibility.Visible;
                    return VindRitFilterVM.SelectedDestination;
                }
                else
                {
                    this.VisibleSelectedDestination = Visibility.Collapsed;
                    return null;
                }
               
            }
        }

        public Visibility VisibleSelectedRating { get; set; }
        public int GetSelectedRating
        {
            get
            {
                if(VindRitFilterVM.SelectedRating!=0 || VindRitFilterVM.SelectedRating != -1)
                {
                    this.VisibleSelectedRating = Visibility.Visible;
                    return VindRitFilterVM.SelectedRating;
                }
                else
                {
                    this.VisibleSelectedRating = Visibility.Collapsed;
                    return 0;
                }
               
            }
        }

        public Visibility VisibleSelectedBobsType { get; set; }
        public BobsType GetSelectedBobsType
        {
            get
            {
                if (VindRitFilterVM.SelectedBobsType != null)
                {
                    this.VisibleSelectedBobsType = Visibility.Visible;
                    return VindRitFilterVM.SelectedBobsType;
                }
                else
                {
                  
                    this.VisibleSelectedBobsType = Visibility.Collapsed;
                    return null;
                }
                
            }
        }

        public Visibility VisibleSelectedParty { get; set; }
        public Party GetSelectedParty
        {
            get
            {
                if (VindRitVM.SelectedParty != null)
                {
                    this.VisibleSelectedParty = Visibility.Visible;
                    return VindRitVM.SelectedParty;

                }
                else
                {
                    this.VisibleSelectedParty = Visibility.Collapsed;
                    return null;
                }

            }
        }

        public string GetStatus
        {
            get
            {
               if((this.Status=="" || this.Status==null))
                {
                    if (VindRitVM.SelectedParty != null)
                    {
                        return VindRitVM.SelectedParty.Name;
                    }
                    else
                    {
                        return "";
                    }
                   
                }
                else
                {
                    return this.Status;
                }
            }
        }


        private async void getRitTime(Location location)
        {
            if (location != null)
            {
                //checkhowfaraway
                Response farEnough = Task.FromResult<Response>(await TripRepository.Difference((Location)VindRitFilterVM.SelectedDestination.Location, location)).Result;

                double distance;
                double.TryParse(farEnough.Value.ToString(), out distance);
               

                double speed = 50;
                double time = distance / speed;

                RitTime = ": " + time.ToString();
                RaisePropertyChanged("RitTime");
                
            }
        }

        #endregion

        //others
        public RelayCommand GoChatCommand { get; set; }
        public RelayCommand GoFilterCommand { get; set; }
        public RelayCommand FindBobCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand ShowModalCommand { get; set; }
        public RelayCommand CloseModalCommand { get; set; }

        public string BobRequests { get; set; }
        public string RitTime { get; set; }
        public List<Party> Parties { get; set; }
        public string Status { get; set; }


        private bool _EnableFind;

        public bool EnableFind
        {
            get { return _EnableFind; }
            set
            {
                _EnableFind = value;
                if (_EnableFind == true)
                {
                    this.VisibleFind = Visibility.Visible;
                    this.VisibleCancel = Visibility.Collapsed;
                    this.VisibleChat = Visibility.Collapsed;
                    this.CancelText = "Annuleer";
                }
                else
                {
                    this.VisibleFind = Visibility.Collapsed;
                    this.VisibleCancel = Visibility.Visible;
                    this.VisibleChat = Visibility.Visible;
                    this.CancelText = "Annuleer huidige trip";
                }
                RaisePropertyChanged("CancelText");
                RaisePropertyChanged("VisibleFind"); RaisePropertyChanged("VisibleCancel"); RaisePropertyChanged("VisibleChat");
            }
        }

       


      


        #endregion

        //Constructor
        public VindRitVM()
        {

            GoChatCommand = new RelayCommand(GoChat);
            GoFilterCommand = new RelayCommand(GoFilter);
            FindBobCommand = new RelayCommand(FindBob);
            CancelCommand = new RelayCommand(CancelTrip);
            CloseModalCommand = new RelayCommand(CloseModal);
            ShowModalCommand = new RelayCommand(ShowModal);

            Messenger.Default.Register<NavigateTo>(typeof(bool), ExecuteNavigatedTo);

            this.Loading = false;
            this.EnableFind = true;
            VisibleModal = Visibility.Collapsed;
            this.Frame = new Frame();
            this.VisibleFilterContext = Visibility.Collapsed;
           

            //Ritten ophalen
            getBobs();

            //Loaded();
            RaiseAll();
        }

        private async void getBobs()
        {
            this.BobList = new List<Bob.All>();

            //Ingeladen bobs ophalen
            List<Bob.All> omtezettenbobs = await BobsRepository.GetBobs();

            for (int i = 0; i < omtezettenbobs.Count(); i++)
            {
                Bob.All bob = omtezettenbobs[i];

                BobList.Add(bob);
            }
        }

        private void RaiseAll()
        {
            RaisePropertyChanged("Loading");
            RaisePropertyChanged("EnableFind");
            RaisePropertyChanged("VisibleModal");
            RaisePropertyChanged("VisibleFind");
            RaisePropertyChanged("VisibleCancel");
            RaisePropertyChanged("CancelText");
            RaisePropertyChanged("VisibleChat");
            try
            {
                RaisePropertyChanged("Frame");
            }
            catch (Exception)
            {

                
            }
           
            RaisePropertyChanged("VisibleFilterContext");
            RaisePropertyChanged("BobRequests");

            RaisePropertyChanged("SelectedParty");
            RaisePropertyChanged("SelectedUser");
            RaisePropertyChanged("CurrentTrip");
            RaisePropertyChanged("BobAccepted");
            RaisePropertyChanged("SelectedBob");
            RaisePropertyChanged("StatusID");
            RaisePropertyChanged("Request");
            RaisePropertyChanged("Status");

            RaisePropertyChanged("GetStatus");
            RaisePropertyChanged("GetSelectedRating");
            RaisePropertyChanged("GetRitTime");
            RaisePropertyChanged("GetSelectedDestination");
            RaisePropertyChanged("GetSelectedBobsType");
            RaisePropertyChanged("GetSelectedParty");
            RaisePropertyChanged("GetSelectedFriendsString");


            RaisePropertyChanged("VisibleSelectedFriends");
            RaisePropertyChanged("VisibleSelectedDestination");
            RaisePropertyChanged("VisibleSelectedRating");
            RaisePropertyChanged("VisibleSelectedParty");
            RaisePropertyChanged("VisibleSelectedBobsType");

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
                    this.Loading = false;
                    this.EnableFind = true;
                    VisibleModal = Visibility.Collapsed;
                    this.Frame = new Frame();
                    this.VisibleFilterContext = Visibility.Collapsed;
                   
   

                    Task task = GetParties();
                    Task task2 = GetDestinations();
                    Task task3 = GetBobTypes();
                    GetCurrentTrip();

                   
                

                    RaiseAll();

                });
#pragma warning restore CS1998

            });
        }




        //Methods

        #region navigate to
        private async void ExecuteNavigatedTo(NavigateTo obj)
        {
            if (obj.Name == "loaded")
            {
                Type view = (Type)obj.View;
                if (view == typeof(VindRit) || view == typeof(VindRitBob))
                {
                    //loaded
                    Loaded();
                }
            }
            if (obj.Reload == true)
            {
                Type view = (Type)obj.View;
                if (view == typeof(VindRit))
                {
                    //loaded
                    Loaded();
                }
            }

            if (obj.Name != null && obj.Name != "")
            {
                switch (obj.Name)
                {
                    case "trip_location":
                        if (VindRitVM.CurrentTrip != null)
                        {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
                            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                            {
                                trip_location();
                            });
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
                        }

                        break;
                    case "find_bob":
                        find_bob((bool)obj.Result);
                        break;
                   
                    case "trip_location:reload":
                        Users_Destinations dest = Task.FromResult<Users_Destinations>(await DestinationRepository.GetDestinationById(VindRitVM.CurrentTrip.Destinations_ID)).Result;

                        VindRitFilterVM.SelectedDestination = dest;
                        if (VindRitFilterVM.SelectedDestination != null)
                        {
                            VindRitVM.StatusID = 2;
                            StartTripLocationTimer();
                        }
                        break;

                    default:
                        break;
                }
            }
        }


      

        private async void find_bob(bool ok)
        {
            if (ok == false)
            {
                this.Loading = false;
               

                RaiseAll();
                if (bobs != null)
                {
                    try
                    {
                        bobs.Remove(bobs.First());

                        if (bobs.Count == 0)
                        {
                            Messenger.Default.Send<Dialog>(new Dialog()
                            {
                                Message = "Geen bob gevonden",
                                Ok = "Ok",
                                Nok = null,
                                ViewOk = null,
                                ViewNok = null,
                                ParamView = false,
                                Cb = null
                            });
                            this.VisibleFind = Visibility.Visible;
                            this.VisibleCancel = Visibility.Collapsed;
                            RaiseAll();
                        }
                        else
                        {
                            ShowBob(bobs.First());
                        }
                    }
                    catch (Exception ex )
                    {

                        Debug.WriteLine(ex.Message.ToString());
                    }
                    
                }
                else
                {

                    Messenger.Default.Send<Dialog>(new Dialog()
                    {
                        Message = "Geen bob gevonden",
                        Ok = "Ok",
                        Nok = null,
                        ViewOk = null,
                        ViewNok = null,
                        ParamView = false,
                        Cb = null
                    });
                    this.VisibleFind = Visibility.Visible;
                    this.VisibleCancel = Visibility.Collapsed;
                    RaiseAll();
                }
               
              
            }
            else
            {
                //take this bob
                if (bobs != null)
                {
                    Random random = new Random();
                    int randomNumber = random.Next(0, 1000);
                    VindRitVM.FindID = randomNumber;

                    Bob bob = bobs.First();

                    Bob.All bobAll = Task.FromResult<Bob.All>(await BobsRepository.GetBobById(bob.ID.Value)).Result;
                    VindRitVM.SelectedBob = bobAll;

                    Libraries.Socket socketSend = new Libraries.Socket() { From = MainViewVM.USER.ID, To = bobAll.User.ID, Status = true, ID=randomNumber };
                    MainViewVM.socket.Emit("bob_ACCEPT:send", JsonConvert.SerializeObject(socketSend));


                    this.Status = "Wachten op antwoord van bob";

                    this.Loading = true;
                    RaiseAll();
                }


            }
        }

        #endregion


        //update location user/bob naar de db

        private async void trip_location()
        {
            VindRitVM.StatusID = 1;

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
                VindRitVM.StatusID = 2;
                StartTripLocationTimer();


            }
        }


        #region  StartTripLocationTimer
        DispatcherTimer timer = new DispatcherTimer();
        private void StartTripLocationTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 20);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private async void Timer_Tick(object sender, object e)
        {
            Geolocator geolocator = new Geolocator();
            Geoposition pos = await geolocator.GetGeopositionAsync();
            Location location = new Location() { Latitude = pos.Coordinate.Point.Position.Latitude, Longitude = pos.Coordinate.Point.Position.Longitude };

            getRitTime(location);

            if (location != null)
            {
                //checkhowfaraway
                Response farEnough = Task.FromResult<Response>(await TripRepository.Difference((Location)VindRitFilterVM.SelectedDestination.Location, location)).Result;

                if (farEnough.Success == true)
                {
                    //kleiner dan 1km
                    VindRitVM.StatusID = 3;
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

                    bool done = Task.FromResult<bool>(await OnDestination()).Result;
                    if (done == true)
                    {
                        BobisDone(location, "Trip is afgerond");
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

                    Libraries.Socket socketSendToBob = new Libraries.Socket() {
                        To = bob.User.ID,
                        Status = true
                    };

                    Libraries.Socket socketSendToUser = new Libraries.Socket()
                    {
                        To = MainViewVM.USER.ID,
                        Status = true
                    };

                    MainViewVM.socket.Emit("trip_DONE:send", JsonConvert.SerializeObject(socketSendToBob));

                    MainViewVM.socket.Emit("trip_DONE:send", JsonConvert.SerializeObject(socketSendToUser));


                    //todo: rating
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


        #region gets
        private async Task GetBobTypes()
        {
            List<BobsType> lijst = await BobsRepository.GetTypes();
            VindRitFilterVM.SelectedBobsType = lijst.Where(r => r.ID == 2).First();
           
        }
        private async void GetCurrentTrip()
        {
            if (VindRitVM.CurrentTrip == null || VindRitVM.CurrentTrip.ID == 0)
            {
                try
                {
                    string json = await Localdata.read("trip.json");

                    var data = JsonConvert.DeserializeObject<Trip>(json);
                    if (data.ID != -1)
                    {
                        VindRitVM.CurrentTrip = data;

                        this.EnableFind = false;
                    }
                    else
                    {
                        this.EnableFind = true;
                    }
                  


                }
                catch (Exception ex)
                {

                    this.EnableFind = true;
                   
                }
            }
            else
            {
                this.EnableFind = false;
             
            }
            RaiseAll();




        }
        private async Task<Boolean> GetParties()
        {

            this.Parties = await PartyRepository.GetParties();
            

            RaiseAll();

            if (this.Parties.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }



        private async Task GetDestinations()
        {
            List<Users_Destinations> lijst = await DestinationRepository.GetDestinations();
            VindRitFilterVM.SelectedDestination = lijst.Where(r => r.Default == true).First() ;



        }

        #endregion



        #region navigate

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
        private void GoFilter()
        {
            this.VisibleFilterContext = Visibility.Visible;
            RaiseAll();

           

            Frame rootFrame = MainViewVM.MainFrame as Frame;
            rootFrame.Navigate(typeof(VindRitFilter),true);
        }


        #endregion



        //find bob
        #region findbob
        List<Bob> bobs;


        //tasks
        private async Task<List<Bob>> FindBob_task()
        {
            if (VindRitFilterVM.SelectedParty != "" && VindRitFilterVM.SelectedParty!=null)
            {

                try
                {
                    Geolocator geolocator = new Geolocator();
                    Geoposition pos = await geolocator.GetGeopositionAsync();


                    int? rating = VindRitFilterVM.SelectedRating;
                    DateTime minDate = DateTime.Today; //moet nog gedaan worden
                    int bobsType_ID = VindRitFilterVM.SelectedBobsType.ID;
                    Location location = new Location() { Latitude = pos.Coordinate.Point.Position.Latitude, Longitude = pos.Coordinate.Point.Position.Longitude };
                    int? maxDistance = MainViewVM.searchArea;

                    List<Bob> bobs = await BobsRepository.FindBobs(rating, minDate, bobsType_ID, location, maxDistance);


                    return bobs;
                }
                catch (Exception ex)
                {

                    return null;
                }
               
            }
            else
            {
                Bob bob= new Bob()
                {
                    ID = -1
                };
                List<Bob> bobs = new List<Bob>();
                bobs.Add(bob);
                return bobs;
            }

            //location

        }
        private async void FindBob()
        {
            this.VisibleFilterContext = Visibility.Visible;
            this.Loading = true;
            this.VisibleFind = Visibility.Collapsed;
            this.VisibleCancel = Visibility.Visible;

            RaiseAll();
            bobs = new List<Bob>();
            bobs = Task.FromResult<List<Bob>>(await FindBob_task()).Result;

            this.Loading = false;
            RaiseAll();

            if (bobs == null || bobs.Count <= 0) {
                Messenger.Default.Send<Dialog>(new Dialog()
                {
                    Message = "Geen bob gevonden",
                    Ok = "Ok",
                    Nok = null,
                    ViewOk = null,
                    ViewNok = null,
                    ParamView = false,
                    Cb = null
                });
                this.EnableFind = true;
                RaiseAll();

            }else if (bobs[0].ID == -1)
            {
                Messenger.Default.Send<Dialog>(new Dialog()
                {
                    Message = "Geen Feestje geselecteerd",
                    Ok = "Ok",
                    Nok = null,
                    ViewOk = null,
                    ViewNok = null,
                    ParamView = false,
                    Cb = null
                });
                this.EnableFind = true;
                RaiseAll();

            }
            else
            {
                ShowBob(bobs.First());

            }

        }



        private async void ShowBob(Bob bob)
        {
            Bob.All bob_full = Task.FromResult<Bob.All>(await BobsRepository.GetBobById(bob.ID.Value)).Result;
            
            if (bob_full != null && bob_full.Bob!=null)
            {
                string bob_text = bob_full.User.ToString() + " is in de buurt met volgende nummerplaat " + bob_full.Bob.LicensePlate + " voor een bedrag van " + bob_full.Bob.PricePerKm + " euro per km.";

                Messenger.Default.Send<Dialog>(new Dialog()
                {
                    Message = "Volgende bob gevonden: " + "\n" + bob_text,
                    Ok = "Take bob",
                    Nok = "Next bob",
                    ViewOk = null,
                    ViewNok = null,
                    ParamView = false,
                    Cb = "find_bob",
                    IsNotification=true
                });
               
            }




        }

        #endregion




        #region methods

        private async void CancelTrip()
        {
            if (this.EnableFind == true)
            {
                this.VisibleCancel = Visibility.Collapsed;
                this.VisibleFind = Visibility.Visible;
                this.Loading = false;
                this.Status = null;

                RaiseAll();
            }
            else
            {
                Geolocator geolocator = new Geolocator();
                Geoposition pos = await geolocator.GetGeopositionAsync();
                Location location = new Location() { Latitude = pos.Coordinate.Point.Position.Latitude, Longitude = pos.Coordinate.Point.Position.Longitude };


                VindRitVM.StatusID = 5; //canceld
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

                BobisDone(location, "Trip is geannuleerd");

                this.EnableFind = true;
                RaiseAll();
            }

            
        }


        #endregion



        private void CloseModal()
        {
            //Loaded();

            if (this.Parties != null && VindRitFilterVM.SelectedParty!=null)
            {
                Party foundParty=  this.Parties.Find(r => r.Name.Trim() == VindRitFilterVM.SelectedParty.Trim());
                VindRitVM.SelectedParty = foundParty;
                this.VisibleSelectedParty = Visibility.Visible;
               
            }

            VisibleModal = Visibility.Collapsed;
            RaiseAll();

           
           
        }
        private void ShowModal()
        {
            this.Frame = new Frame();
            this.Frame.Navigated += Frame_Navigated;
            this.Frame.Navigate(typeof(VindRitFilter),true);
           

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
                    View=typeof(VindRitFilter)
                   
                });

                if (reload == false)
                {
                    //e.Cancel = true;

                }
            }
        }
    }
}
