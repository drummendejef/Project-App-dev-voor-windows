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
    public class VindRitVM : ViewModelBase
    {
        #region props
        #region static
        public static Party SelectedParty { get; set; }

        public Visibility VisibleSelectedBob { get; set; }
        public static Bob.All SelectedBob { get; set; }
        public static User SelectedUser { get; set; }
        public static Trip CurrentTrip { get; set; }
        public static bool BobAccepted { get; set; }
        public static int StatusID { get; set; }
        public static float FindID { get; set; }




        #endregion

        public bool Loading { get; set; }
        public string Error { get; set; }
        public Visibility VisibleFind { get; set; }
        public Visibility VisibleCancel { get; set; }
        public Visibility VisibleChat { get; set; }
        public Visibility VisibleFilterContext { get; set; }
        public Visibility VisibleModal { get; set; }

        DispatcherTimer timer = MainViewVM.TIMER;

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
        public int? GetSelectedRating
        {
            get
            {
                if(VindRitFilterVM.SelectedRating!=0 || VindRitFilterVM.SelectedRating!=-1)
                {
                    this.VisibleSelectedRating = Visibility.Visible;
                    return VindRitFilterVM.SelectedRating;
                }
                else
                {
                    this.VisibleSelectedRating = Visibility.Collapsed;
                    return null;
                }
               
            }
        }

        public Bob.All GetSelectedBob
        {
            get
            {
                if (VindRitVM.SelectedBob != null)
                {

                    return VindRitVM.SelectedBob;
                }
                else
                {

                   
                    return null;
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
                        return GetStatusName(VindRitVM.StatusID);
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


        private async Task getRitTime(Location location)
        {
            if (location != null)
            {
                //checkhowfaraway
                Response farEnough = Task.FromResult<Response>(await TripRepository.Difference((Location)VindRitFilterVM.SelectedDestination.Location, location)).Result;

                if(farEnough.Success==true && farEnough.Value!=null)
                {
                    double distance;
                    double.TryParse(farEnough.Value.ToString(), out distance);


                    double speed = 45;
                    double time = distance / speed;

                    this.RitTime = ": " + time.ToString();
                    RaiseAll();
                }
                
                
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
        public List<Bob.All> Bobs { get; set; }
        public List<User.All> Users { get; set; }
        public string Status { get; set; }
        public MapControl Map { get; set; }
        public List<Users_Destinations> Destinations { get; set; }


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
                RaiseAll();
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
            this.VisibleModal = Visibility.Collapsed;
            this.VisibleSelectedFriends = Visibility.Collapsed;
            this.VisibleSelectedBob = Visibility.Collapsed;
            this.VisibleSelectedBobsType = Visibility.Collapsed;
            this.VisibleSelectedRating = Visibility.Collapsed;
            this.VisibleSelectedParty = Visibility.Collapsed;
           
            this.VisibleFilterContext = Visibility.Collapsed;


            this.VisibleFind = Visibility.Collapsed;
            this.VisibleCancel = Visibility.Collapsed;
            this.VisibleChat = Visibility.Collapsed;


            //Ritten ophalen



            Loaded();
            RaiseAll();
        }

        //De bobs ophalen om op de kaart te tonen.
     

        private async void RaiseAll()
        {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                RaisePropertyChanged("Loading");
                RaisePropertyChanged("EnableFind");
                RaisePropertyChanged("VisibleModal");
                RaisePropertyChanged("VisibleFind");
                RaisePropertyChanged("VisibleCancel");
                RaisePropertyChanged("CancelText");
                RaisePropertyChanged("VisibleChat");


                RaisePropertyChanged("VisibleFilterContext");

                RaisePropertyChanged("SelectedParty");
                RaisePropertyChanged("SelectedUser");
                RaisePropertyChanged("CurrentTrip");
                RaisePropertyChanged("BobAccepted");
                RaisePropertyChanged("SelectedBob");
                RaisePropertyChanged("StatusID");
                RaisePropertyChanged("Request");
                RaisePropertyChanged("Status");
                RaisePropertyChanged("Bobs");
                RaisePropertyChanged("RitTime");
                RaisePropertyChanged("Parties");
                RaisePropertyChanged("Users");
                RaisePropertyChanged("Bobs");
                RaisePropertyChanged("Destinations");

                RaisePropertyChanged("GetStatus");
                RaisePropertyChanged("GetSelectedRating");
                RaisePropertyChanged("GetSelectedDestination");
                RaisePropertyChanged("GetSelectedBobsType");
                RaisePropertyChanged("GetSelectedParty");
                RaisePropertyChanged("GetSelectedFriendsString");


                RaisePropertyChanged("VisibleSelectedFriends");
                RaisePropertyChanged("VisibleSelectedDestination");
                RaisePropertyChanged("VisibleSelectedRating");
                RaisePropertyChanged("VisibleSelectedParty");
                RaisePropertyChanged("VisibleSelectedBobsType");
                RaisePropertyChanged("VisibleSelectedBob");
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
                 
                 
                 
                  
                    this.EnableFind = true;

                    this.Loading = false;
                    this.Status = null;

                    



                    canShowDialog = true;

                    await GetBobs();
                    await GetUsers();
                    await GetParties();
                    await GetDestinations();
                    await GetBobTypes();


                    this.VisibleModal = Visibility.Collapsed;
                    this.VisibleFilterContext = Visibility.Collapsed;
                    this.VisibleCancel = Visibility.Collapsed;
                    this.VisibleFind = Visibility.Collapsed;

                    await GetCurrentTrip();


                   


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
                if (view == typeof(VindRit))
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
                    this.VisibleFind = Visibility.Visible;
                    //loaded
                    Loaded();
                }
            }

            if (obj.Name != null && obj.Name != "")
            {
                switch (obj.Name)
                {
                    case "trip_location":
                        if (MainViewVM.CurrentTrip != null)
                        {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
                            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                            {
                                trip_location();
                            });
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
                        }

                        break;
                    case "rating_dialog":
                        Bobs_Parties item = JsonConvert.DeserializeObject<Bobs_Parties>(obj.Data.ToString());
                        RatingDialog(item);
                        break;
                    case "find_bob":
                        find_bob((bool)obj.Result);
                        break;
                    case "get_trip":
                        await GetCurrentTrip();
                        
                        break;
                    case "trip_location:reload":
                        Users_Destinations dest = Task.FromResult<Users_Destinations>(await DestinationRepository.GetDestinationById(MainViewVM.CurrentTrip.Destinations_ID)).Result;

                        //instellen voor timer, niet gebruiken in filter
                        VindRitFilterVM.SelectedDestination = dest;
                        if (VindRitFilterVM.SelectedDestination != null)
                        {
                            VindRitVM.SelectedParty = await PartyRepository.GetPartyById(MainViewVM.CurrentTrip.Party_ID);
                            this.EnableFind = false;
                         
                            RaiseAll();
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
                    await UserRepository.PostPoint();

                    Random random = new Random();
                    float randomNumber = random.Next(0, 1000);
                    VindRitVM.FindID = randomNumber;

                    Bob bob = bobs.First();

                    Bob.All bobAll = Task.FromResult<Bob.All>(await BobsRepository.GetBobById(bob.ID.Value)).Result;
                    VindRitVM.SelectedBob = bobAll;

                    Libraries.Socket socketSend = new Libraries.Socket() { From = MainViewVM.USER.ID, To = bobAll.User.ID, Status = true, ID=randomNumber };
                    MainViewVM.socket.Emit("bob_ACCEPT:send", JsonConvert.SerializeObject(socketSend));

                    ShowRoute((Location)VindRitVM.SelectedParty.Location, (Location)VindRitFilterVM.SelectedDestination.Location);

                    SetStatus(6);
                    this.VisibleSelectedBob = Visibility.Visible;
                    ShowedOnParty = false;
                    this.Loading = true;
                    RaiseAll();
                }


            }
        }

        #endregion


        //update location user/bob naar de db

        private void trip_location()
        {
          
            this.EnableFind = false;
            this.Loading = false;
           

            StartTripLocationTimer();
            //this.Map.MapElements.Clear();
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

            Toast.Tile("Party: " + this.GetSelectedParty.Name, "Bestemming: " + this.GetSelectedDestination.Name, "Status " + this.Status);


            if (location != null)
            {
                if (MainViewVM.CurrentTrip.StatusID == 2 || MainViewVM.CurrentTrip.StatusID == 8)
                {
                    ShowRoute((Location)VindRitVM.SelectedParty.Location, (Location)VindRitFilterVM.SelectedDestination.Location);
                }
                else
                {
                    ShowRoute(location, (Location)VindRitFilterVM.SelectedDestination.Location);
                }

                //checkhowfaraway
                Response farEnough = Task.FromResult<Response>(await TripRepository.Difference((Location)VindRitFilterVM.SelectedDestination.Location, location)).Result;

                if (farEnough.Success == true)
                {
                    //kleiner dan 1km
                    timer.Stop();

                    if (canShowDialog==true)
                    {
                        canShowDialog = false;

                        bool done = Task.FromResult<bool>(await OnDestination()).Result;
                        if (done == true)
                        {
                            Bob.All bob = Task.FromResult<Bob.All>(await BobsRepository.GetBobById(MainViewVM.CurrentTrip.Bobs_ID)).Result;

                            BobisDone("Trip is afgerond");
                            Bobs_Parties itemBobs = new Bobs_Parties()
                            {
                                Bobs_ID=MainViewVM.CurrentTrip.Bobs_ID,
                                Party_ID=MainViewVM.CurrentTrip.Party_ID,
                                Trips_ID=MainViewVM.CurrentTrip.ID,
                                Users_ID=MainViewVM.CurrentTrip.Users_ID
                            };

                            RatingDialog(itemBobs);
                        }
                        else
                        {
                            timer.Start();
                        }
                    }


                }



            }
        }

       

        private async void BobisDone(string text)
        {
            this.Map.MapElements.Clear();
            ClearAllMapItems();

            Response active = Task.FromResult<Response>(await TripRepository.PutActive(MainViewVM.CurrentTrip.ID, false)).Result;

            if (active.Success == true)
            {
                Bob.All bob = Task.FromResult<Bob.All>(await BobsRepository.GetBobById(MainViewVM.CurrentTrip.Bobs_ID)).Result;

                Libraries.Socket socketSendToBob = new Libraries.Socket() {
                    To = bob.User.ID,
                    Status = true
                };
                Libraries.Socket socketSendToUser = new Libraries.Socket()
                {
                    To = MainViewVM.USER.ID,
                    Status = true
                };


                //todo: rating
                await UserRepository.PostPoint();
                   

                MainViewVM.socket.Emit("trip_DONE:send", JsonConvert.SerializeObject(socketSendToUser));
                MainViewVM.socket.Emit("trip_DONE:send", JsonConvert.SerializeObject(socketSendToBob));


                var dialog = new MessageDialog(text);
                dialog.Commands.Add(new UICommand("Ok") { Id = 0 });
                dialog.DefaultCommandIndex = 0;
                var result = await dialog.ShowAsync();
                int id = int.Parse(result.Id.ToString());

                this.EnableFind = true;
                this.VisibleSelectedFriends = Visibility.Collapsed;
                this.VisibleSelectedBob = Visibility.Collapsed;
                this.VisibleSelectedBobsType = Visibility.Collapsed;
                this.VisibleSelectedRating = Visibility.Collapsed;
                this.VisibleSelectedParty = Visibility.Collapsed;
                ShowedOnParty = false;

                this.VisibleCancel = Visibility.Collapsed;
                this.VisibleFind = Visibility.Visible;
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


        #region gets
        List<BobsType> bobTypes;
        private async Task GetBobTypes()
        {
            bobTypes= await BobsRepository.GetTypes();
            VindRitFilterVM.SelectedBobsType = bobTypes.Where(r => r.ID == 2).First();
           
        }
        private async Task GetCurrentTrip()
        {
           

            if (MainViewVM.CurrentTrip == null ||MainViewVM.CurrentTrip.ID == 0)
            {
                try
                {
                    string json = await Localdata.read("trip.json");
                    if (json != null && json !="")
                    {
                        var data = JsonConvert.DeserializeObject<Trip>(json);
                        if (data.ID != -1)
                        {
                            this.Loading = true;
                            RaiseAll();

                            SetStatus(data.StatusID.Value);
                            MainViewVM.CurrentTrip = data;

                            if (data.StatusID.Value== 5)
                            {
                                Bobs_Parties bobs_parties = new Bobs_Parties()
                                {
                                    Bobs_ID = MainViewVM.CurrentTrip.Bobs_ID,
                                    Party_ID = MainViewVM.CurrentTrip.Party_ID,
                                    Trips_ID = MainViewVM.CurrentTrip.ID,
                                    Users_ID = MainViewVM.CurrentTrip.Users_ID

                                };

                                Libraries.Socket socketSendToUser = new Libraries.Socket()
                                {
                                    From = MainViewVM.USER.ID,
                                    To = MainViewVM.CurrentTrip.Users_ID,
                                    Status = true,
                                    Object = JsonConvert.SerializeObject(bobs_parties),
                                    Object2 = true
                                };
                                MainViewVM.socket.Emit("trip_DONE:send", JsonConvert.SerializeObject(socketSendToUser));
                            }
                           

                            Location location = await LocationService.GetCurrent();
                            Users_Destinations destination = Task.FromResult<Users_Destinations>(await DestinationRepository.GetDestinationById(MainViewVM.CurrentTrip.Destinations_ID)).Result;
                            Bob.All bob = Task.FromResult<Bob.All>(await BobsRepository.GetBobById(MainViewVM.CurrentTrip.Bobs_ID)).Result;
                            Party party = Task.FromResult<Party>(await PartyRepository.GetPartyById(MainViewVM.CurrentTrip.Party_ID)).Result;
                            VindRitVM.SelectedBob = bob;
                            VindRitVM.SelectedParty = party;

                            if (bobTypes != null)
                            {
                                VindRitFilterVM.SelectedBobsType = bobTypes.Where(r => r.ID == bob.Bob.BobsType_ID).First();
                            }
                            VindRitFilterVM.SelectedDestination = destination;

                            if (MainViewVM.CurrentTrip.StatusID.HasValue)
                            {
                                if (MainViewVM.CurrentTrip.StatusID == 2 || MainViewVM.CurrentTrip.StatusID == 8)
                                {
                                    ShowRoute((Location)VindRitVM.SelectedParty.Location, (Location)VindRitFilterVM.SelectedDestination.Location);
                                }
                                else
                                {
                                    ShowRoute(location, (Location)VindRitFilterVM.SelectedDestination.Location);
                                }
                            }

                            

                            ShowedOnParty = true;
                            this.EnableFind = false;
                            this.Loading = false;
                            RaiseAll();
                            return;
                        }
                    }

                    SetStatus(0);
                    ShowedOnParty = true;
                    this.EnableFind = true;



                }
                catch (Exception ex)
                {
                    SetStatus(0);
                    ShowedOnParty = true;
                    this.EnableFind = true;
                   
                }
            }
            else
            {
                this.Loading = true;
                RaiseAll();

                MainViewVM.CurrentTrip = Task.FromResult<Trip>(await TripRepository.GetCurrentTrip()).Result;

                if (MainViewVM.CurrentTrip.StatusID.Value == 5)
                {
                    Bobs_Parties bobs_parties = new Bobs_Parties()
                    {
                        Bobs_ID = MainViewVM.CurrentTrip.Bobs_ID,
                        Party_ID = MainViewVM.CurrentTrip.Party_ID,
                        Trips_ID = MainViewVM.CurrentTrip.ID,
                        Users_ID = MainViewVM.CurrentTrip.Users_ID

                    };

                    Libraries.Socket socketSendToUser = new Libraries.Socket()
                    {
                        From = MainViewVM.USER.ID,
                        To = MainViewVM.CurrentTrip.Users_ID,
                        Status = true,
                        Object = JsonConvert.SerializeObject(bobs_parties),
                        Object2 = true
                    };
                    MainViewVM.socket.Emit("trip_DONE:send", JsonConvert.SerializeObject(socketSendToUser));
                }

                Location location = await LocationService.GetCurrent();
                Users_Destinations destination = Task.FromResult<Users_Destinations>(await DestinationRepository.GetDestinationById(MainViewVM.CurrentTrip.Destinations_ID)).Result;
                Bob.All bob = Task.FromResult<Bob.All>(await BobsRepository.GetBobById(MainViewVM.CurrentTrip.Bobs_ID)).Result;
                Party party = Task.FromResult<Party>(await  PartyRepository.GetPartyById(MainViewVM.CurrentTrip.Party_ID)).Result;
                VindRitVM.SelectedBob = bob;
                VindRitVM.SelectedParty = party;

                if (bobTypes != null)
                {
                    VindRitFilterVM.SelectedBobsType = bobTypes.Where(r => r.ID == bob.Bob.BobsType_ID).First();
                }
                VindRitFilterVM.SelectedDestination = destination;


               

                ShowedOnParty = true;

                if (MainViewVM.CurrentTrip.StatusID.HasValue)
                {
                    if (MainViewVM.CurrentTrip.StatusID == 2 || MainViewVM.CurrentTrip.StatusID == 8)
                    {
                        ShowRoute((Location)VindRitVM.SelectedParty.Location, (Location)VindRitFilterVM.SelectedDestination.Location);
                    }
                    else
                    {
                        ShowRoute(location, (Location)VindRitFilterVM.SelectedDestination.Location);
                    }

                    SetStatus(MainViewVM.CurrentTrip.StatusID.Value);
                }

                this.EnableFind = false;
                this.Loading = false;
               

            }
            RaiseAll();




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
            rootFrame.Navigate(typeof(VindRitFilter));
        }


        #endregion



        //find bob
        #region findbob
        List<Bob> bobs;


        //tasks
        private async Task<List<Bob>> FindBob_task()
        {
           
            try
            {
                Location location = Task.FromResult<Location>(await LocationService.GetCurrent()).Result;

                int? rating = VindRitFilterVM.SelectedRating;
                DateTime minDate = DateTime.Today; //moet nog gedaan worden
                int bobsType_ID = VindRitFilterVM.SelectedBobsType.ID;
                int? maxDistance = MainViewVM.searchArea;

                List<Bob> bobs = Task.FromResult<List<Bob>>(await BobsRepository.FindBobs(rating, minDate, bobsType_ID, location, maxDistance)).Result;


                return bobs;
            }
            catch (Exception ex)
            {

                return null;
            }
               
 

            //location

        }
        private async void FindBob()
        {
            //controle
            if (VindRitFilterVM.SelectedDestination == null)
            {
                Messenger.Default.Send<Dialog>(new Dialog()
                {
                    Message = "U hebt geen bestemming aangemaakt \nGa naar bestemmingen en maak een nieuwe aan.",
                    Ok = "Ok",
                });
                this.EnableFind = true;
                RaiseAll();
                return;
            }

            if(VindRitFilterVM.SelectedParty==null || VindRitFilterVM.SelectedParty == "")
            {
                Messenger.Default.Send<Dialog>(new Dialog()
                {
                    Message = "Geen Feestje geselecteerd",
                    Ok = "Ok",
                });
                this.EnableFind = true;
                RaiseAll();
                return;
            }




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
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }
            
            if (this.EnableFind == true)
            {
                this.VisibleSelectedFriends = Visibility.Collapsed;
                this.VisibleSelectedBob = Visibility.Collapsed;
                this.VisibleSelectedBobsType = Visibility.Collapsed;
                this.VisibleSelectedRating = Visibility.Collapsed;
                this.VisibleSelectedParty = Visibility.Collapsed;

                this.VisibleCancel = Visibility.Collapsed;
                this.VisibleFind = Visibility.Visible;
                this.Loading = false;
                this.Status = null;
              
                SetStatus(0);

                RaiseAll();
            }
            else
            {

                BobisDone("Trip is geannuleerd");

                this.EnableFind = true;
                this.VisibleSelectedFriends = Visibility.Collapsed;
                this.VisibleSelectedBob = Visibility.Collapsed;
                this.VisibleSelectedBobsType = Visibility.Collapsed;
                this.VisibleSelectedRating = Visibility.Collapsed;
                this.VisibleSelectedParty = Visibility.Collapsed;

                this.VisibleCancel = Visibility.Collapsed;
                this.VisibleFind = Visibility.Visible;
                this.Loading = false;
                this.Status = null;
               
                SetStatus(0);

                RaiseAll();

                
                
               
            }

            
        }


        #endregion



        private void CloseModal()
        {
            

            if (this.Parties != null && VindRitFilterVM.SelectedParty!=null)
            {
                Party foundParty=  this.Parties.Find(r => r.Name.Trim() == VindRitFilterVM.SelectedParty.Trim());
                VindRitVM.SelectedParty = foundParty;
                this.VisibleSelectedParty = Visibility.Visible;

                if (VindRitVM.SelectedParty != null && VindRitFilterVM.SelectedDestination != null)
                {

                    ShowRoute((Location) VindRitVM.SelectedParty.Location,(Location) VindRitFilterVM.SelectedDestination.Location);
                }
               

               
               
            }

            VisibleModal = Visibility.Collapsed;
            RaiseAll();

           
           
        }
        private void SelectParty(int id)
        {
            VindRitVM.SelectedParty = this.Parties.Find(r => r.ID == id);
            RaiseAll();
        }
        private void ShowModal()
        {

            this.Frame.Navigated += Frame_Navigated;
            this.Frame.Navigate(typeof(VindRitFilter), true);

            VisibleModal = Visibility.Visible;
            RaiseAll();

        }
        bool ShowedOnParty = false;
        private void SetStatus(int statusID)
        {
            VindRitVM.StatusID = statusID;
            this.Status = GetStatusName(statusID);
            if (this.GetSelectedParty != null && this.GetSelectedDestination != null)
            {
                Toast.Tile("Party: " + this.GetSelectedParty.Name, "Bestemming: " + this.GetSelectedDestination.Name, "Status " + this.Status);
            }

            if (statusID == 8 && ShowedOnParty==false)
            {
                Messenger.Default.Send<Dialog>(new Dialog()
                {
                    Message = VindRitVM.SelectedBob.User.ToString() + "is toegekomen op " + VindRitVM.SelectedParty.Name + "met nummerplaat: + " + VindRitVM.SelectedBob.Bob.LicensePlate.ToString() ,
                    Ok = "Ok",
                    Nok="Negeer",
                    ViewOk = typeof(VindRit),
                    ViewNok = null,
                    ParamView = false,
                    Cb = null,
                    IsNotification = true
                });
                ShowedOnParty = true;

            }

            RaiseAll();
        }

        private string GetStatusName(int statusID)
        {
            switch (statusID)
            {
                case 0:
                    return "Kies je feestje";
                case 1:
                     return "Bob is aangevraagd";
                case 2:
                    return "Bob is onderweg";
                case 3:
                    return "Bob is toegekomen";
                case 4:
                    return  "Rit is gedaan";
                case 5:
                  return "Rit is geannuleerd";
                case 6://niet in db
                    return "Wachten op antwoord van bob";
                case 7:
                    return "Bob is in de buurt";
                case 8:
                    return "Bob is bij het feestje";
                default:
                    return "";

            }
        }

        private async void RatingDialog(Bobs_Parties item)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {

                var dialog = new ContentDialog()
                {
                    Title = "Rating",
                };

                // Setup Content
                var panel = new StackPanel();

                panel.Children.Add(new TextBlock
                {
                    Text = "Uw rit is ten einde, hoeveel rating wilt u deze bob geven? ",
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 0, 0, 15)
                });

                List<string> items = new List<string>();
                for (int i = 1; i < 5; i++)
                {
                    items.Add(i.ToString());
                }
                var cb = new ComboBox
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };
                cb.ItemsSource = items;

                var txt = new TextBox
                {
                    TextWrapping = TextWrapping.Wrap,
                    HorizontalContentAlignment = HorizontalAlignment.Stretch
                };



                panel.Children.Add(cb);
                panel.Children.Add(txt);
                dialog.Content = panel;

                // Add Buttons
                dialog.PrimaryButtonText = "Ok";
                dialog.PrimaryButtonClick += async delegate
                {
                    double rating;
                    string text = cb.SelectedValue.ToString();
                    string comment = txt.Text;

                    double.TryParse(text, out rating);


                    item.Rating = rating;

                    Response res = await TripRepository.AddRating(item);
                    var ok = res.Success;

                    if (ok == true || ok == false)
                    {
                        Libraries.Socket socketSendToUser = new Libraries.Socket()
                        {
                            To = MainViewVM.USER.ID,
                            Status = true,
                        };

                        MainViewVM.socket.Emit("trip_DONE:send", JsonConvert.SerializeObject(socketSendToUser));
                        
                        this.Map.MapElements.Clear();
                    }

                       
                 

                };



                // Show Dialog
                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.None)
                {

                }
            });
        }

      

        private void Frame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                bool reload = (bool)e.Parameter;

                Messenger.Default.Send<NavigateTo>(new NavigateTo()
                {
                    Reload = reload,
                    View=this.Frame.CurrentSourcePageType
                   
                });

                if (reload == false)
                {
                    //e.Cancel = true;

                }
            }
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
                mapIconUserLocation.Image = MainViewVM.Pins.UserPin;
                this.Map.MapElements.Add(mapIconUserLocation);//Marker op de map zetten.
            }



        }
        #endregion

        #region maps
        private async Task GetDestinations()
        {

            this.Destinations= await DestinationRepository.GetDestinations();
           
            if (this.Destinations != null && this.Destinations.Count != 0)
            {
                VindRitFilterVM.SelectedDestination = this.Destinations.Where(r => r.Default == true).First();
            }


            if (this.Destinations != null)
            {
                for (int i = 0; i < this.Destinations.Count(); i++)
                {
                    try
                    {
                        Users_Destinations item = this.Destinations[i];
                        if (item.Location == null)
                        {
                            break;
                        }
                        item.VisibleShow = Visibility.Collapsed;
                        item.ShowCommand = new RelayCommand<object>(e => ShowDestination(e));
                       

                        BasicGeoposition tempbasic = new BasicGeoposition();
                        //Locaties omzetten en in de tijdelijke posities opslaan.
                        tempbasic.Latitude = ((Location)item.Location).Latitude;
                        tempbasic.Longitude = ((Location)item.Location).Longitude;

                        //Omzetten van tijdelijk punt naar echte locatie (anders krijg je die niet in de mapIconFeestLocation.Location)
                        Geopoint temppoint = new Geopoint(tempbasic);

                        MapIcon mapIconFeestLocation = new MapIcon();
                        mapIconFeestLocation.Location = temppoint; //Opgehaalde locatie
                                                                   //mapIconFeestLocation.Title = feest.Name; //Naam van het feestje;
                        mapIconFeestLocation.Image = MainViewVM.Pins.HomePin;
                        mapIconFeestLocation.Title = item.Name;
                        this.Map.MapElements.Add(mapIconFeestLocation);//Marker op de map zetten.

                    }
                    catch (Exception ex)
                    {


                    }

                }


                try
                {
                    var newControl = new MapItemsControl();
                    VindRit vindrit = MainViewVM.MainFrame.Content as VindRit;

                    newControl.ItemsSource = Destinations;
                    newControl.ItemTemplate = (DataTemplate)vindrit.Resources["DestinationsMapTemplate"] as DataTemplate;

                    AddOrUpdateChild(newControl);
                }
                catch (Exception)
                {

                   
                }


                RaiseAll();
            }





        }

        private void ShowDestination(object obj)
        {
            var item = obj as Users_Destinations;
            if (item.VisibleShow == Visibility.Collapsed)
            {
                item.VisibleShow = Visibility.Visible;

            }
            else
            {
                item.VisibleShow = Visibility.Collapsed;

            }
            UpdateMapOfType(typeof(List<Users_Destinations>));

            RaiseAll();

        }

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
                        item.TakeCommand = new RelayCommand<object>(e => TakeParty(e));
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


                try
                {
                    var newControl = new MapItemsControl();
                    VindRit vindrit = MainViewVM.MainFrame.Content as VindRit;

                    newControl.ItemsSource = Parties;
                    newControl.ItemTemplate = (DataTemplate)vindrit.Resources["PartiesMapTemplate"] as DataTemplate;

                    AddOrUpdateChild(newControl);
                }
                catch (Exception)
                {

                    
                }


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
        private void TakeParty(object obj)
        {
            var item = obj as Party;

            if (MainViewVM.USER.IsBob==false)
            {
                VindRitFilterVM.SelectedParty = item.Name;
                VindRitVM.SelectedParty = item;



                if (VindRitVM.SelectedParty != null && VindRitFilterVM.SelectedDestination != null)
                {
                    ShowRoute((Location)VindRitVM.SelectedParty.Location, (Location)VindRitFilterVM.SelectedDestination.Location);
                }
            }
            
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
                        mapIconFeestLocation.Image = MainViewVM.Pins.BobPin;
                        mapIconFeestLocation.Title = item.User.ToString();
                        this.Map.MapElements.Add(mapIconFeestLocation);//Marker op de map zetten.
                    }
                    catch (Exception ex)
                    {


                    }

                }

                try
                {
                    var newControl = new MapItemsControl();
                    VindRit vindrit = MainViewVM.MainFrame.Content as VindRit;

                    newControl.ItemsSource = this.Bobs;
                    newControl.ItemTemplate = (DataTemplate)vindrit.Resources["BobsMapTemplate"] as DataTemplate;

                    AddOrUpdateChild(newControl);
                }
                catch (Exception ex)
                {

                    
                }


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
                        mapIconFeestLocation.Image = MainViewVM.Pins.UserPin;
                        mapIconFeestLocation.Title = item.User.ToString();
                        this.Map.MapElements.Add(mapIconFeestLocation);//Marker op de map zetten.
                    }
                    catch (Exception ex)
                    {


                    }

                }

                try
                {
                    var newControl = new MapItemsControl();
                    VindRit vindrit = MainViewVM.MainFrame.Content as VindRit;

                    newControl.ItemsSource = this.Users;
                    newControl.ItemTemplate = (DataTemplate)vindrit.Resources["UsersMapTemplate"] as DataTemplate;



                    AddOrUpdateChild(newControl);
                }
                catch (Exception)
                {

                   
                }




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
                        if (newControl.ItemsSource.GetType() == typeof(List<Users_Destinations>))
                        {
                            control.ItemsSource = null;
                            control.ItemsSource = this.Destinations;
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
                        if (control.ItemsSource.GetType() == typeof(List<Users_Destinations>))
                        {
                            control.ItemsSource = null;
                            control.ItemsSource = this.Destinations;
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
                        if (type == typeof(List<Users_Destinations>))
                        {
                            control.ItemsSource = null;
                            control.ItemsSource = this.Destinations;
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

                        if(item.RouteCommandText == "Toon route")
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

    }
}
