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

        public Visibility VisibleSelectedBob { get; set; }
        public static Bob.All SelectedBob { get; set; }
        public static User SelectedUser { get; set; }
        public static Trip CurrentTrip { get; set; }
        public static bool BobAccepted { get; set; }
        public static int StatusID { get; set; }
        public static float FindID { get; set; }



        public List<Bob.All> BobList { get; set; }


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
                if(VindRitFilterVM.SelectedRating.HasValue)
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

                if(farEnough.Value!=null)
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
           

            //Ritten ophalen
            getBobs();

            Loaded();
            RaiseAll();
        }

        //De bobs ophalen om op de kaart te tonen.
        private async void getBobs()
        {
            this.BobList = new List<Bob.All>();
            List<Bob.All> omtezettenbobs = new List<Bob.All>();

            //Try catch errond om te zorgen dat hij niet crasht bij lege bobs.
            try
            { 
            //Ingeladen bobs ophalen
            omtezettenbobs = await BobsRepository.GetBobs();
            }
            catch
            {
                Debug.WriteLine("Lege bob");
            }

            for (int i = 0; i < omtezettenbobs.Count(); i++)
            {
                Bob.All bob = omtezettenbobs[i];

                BobList.Add(bob);
            }
        }

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
                RaisePropertyChanged("RitTime");

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
                    this.Loading = false;
                 
                    this.VisibleModal = Visibility.Collapsed;
                    
                    this.VisibleFilterContext = Visibility.Collapsed;
                    this.EnableFind = true;

                    this.VisibleCancel = Visibility.Collapsed;
                    this.VisibleFind = Visibility.Visible;
                    this.Loading = false;
                    this.Status = null;

                    



                    canShowDialog = true;

                    getBobs();
                    await GetParties();
                    await GetDestinations();
                    await GetBobTypes();
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
                                await trip_location();
                            });
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
                        }

                        break;
                    case "rating_dialog":
                        SetStatus(4);
                        RaiseAll();
                        Bobs_Parties item = JsonConvert.DeserializeObject<Bobs_Parties>(obj.Data.ToString());
                        RatingDialog(item);
                        break;
                    case "find_bob":
                        find_bob((bool)obj.Result);
                        break;
                   
                    case "trip_location:reload":
                        Users_Destinations dest = Task.FromResult<Users_Destinations>(await DestinationRepository.GetDestinationById(MainViewVM.CurrentTrip.Destinations_ID)).Result;

                        //instellen voor timer, niet gebruiken in filter
                        VindRitFilterVM.SelectedDestination = dest;
                        if (VindRitFilterVM.SelectedDestination != null)
                        {
                         
                            VindRitVM.SelectedParty = await PartyRepository.GetPartyById(MainViewVM.CurrentTrip.Party_ID);
                            this.EnableFind = false;
                            SetStatus(2);
                            
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
                    Random random = new Random();
                    float randomNumber = random.Next(0, 1000);
                    VindRitVM.FindID = randomNumber;

                    Bob bob = bobs.First();

                    Bob.All bobAll = Task.FromResult<Bob.All>(await BobsRepository.GetBobById(bob.ID.Value)).Result;
                    VindRitVM.SelectedBob = bobAll;

                    Libraries.Socket socketSend = new Libraries.Socket() { From = MainViewVM.USER.ID, To = bobAll.User.ID, Status = true, ID=randomNumber };
                    MainViewVM.socket.Emit("bob_ACCEPT:send", JsonConvert.SerializeObject(socketSend));


                    SetStatus(6);
                    this.VisibleSelectedBob = Visibility.Visible;
                    this.Loading = true;
                    RaiseAll();
                }


            }
        }

        #endregion


        //update location user/bob naar de db

        private async Task trip_location()
        {
            SetStatus(2);
            this.EnableFind = false;
            this.Loading = false;
            //todo: swtich
            RaiseAll();


            try
            {
                Location location = await LocationService.GetCurrent();

                Trips_Locations item = new Trips_Locations()
                {
                    Trips_ID =MainViewVM.CurrentTrip.ID,
                    Location = JsonConvert.SerializeObject(location),
                    Statuses_ID = VindRitVM.StatusID
                };
                Response ok = Task.FromResult<Response>(await TripRepository.PostLocation(item)).Result;

                if (ok.Success == true)
                {
                    StartTripLocationTimer();


                }
            }
            catch (Exception ex)
            {

               
            }
           

          

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
            await getRitTime(location);



            if (location != null)
            {
                

                //checkhowfaraway
                Response farEnough = Task.FromResult<Response>(await TripRepository.Difference((Location)VindRitFilterVM.SelectedDestination.Location, location)).Result;

                if (farEnough.Success == true)
                {
                    //kleiner dan 1km
                    SetStatus(7);
                    RaiseAll();
                    timer.Stop();


                    Trips_Locations item = new Trips_Locations()
                    {
                        Trips_ID =MainViewVM.CurrentTrip.ID,
                        Location = JsonConvert.SerializeObject(location),
                        Statuses_ID = VindRitVM.StatusID
                    };
                    Response ok = Task.FromResult<Response>(await TripRepository.PostLocation(item)).Result;
                    if (ok.Success == true)
                    {

                        Bob.All bob = Task.FromResult<Bob.All>(await BobsRepository.GetBobById(MainViewVM.CurrentTrip.Bobs_ID)).Result;
                        Libraries.Socket socketSend = new Libraries.Socket() { From = MainViewVM.USER.ID, To = bob.User.ID, Status = true };
                        MainViewVM.socket.Emit("user_location_NEW:send", JsonConvert.SerializeObject(socketSend)); //bob
                    }
                    else
                    {
                        timer.Stop();

                    }

                    if (canShowDialog==true)
                    {
                        canShowDialog = false;

                        bool done = Task.FromResult<bool>(await OnDestination()).Result;
                        if (done == true)
                        {
                            Bob.All bob = Task.FromResult<Bob.All>(await BobsRepository.GetBobById(MainViewVM.CurrentTrip.Bobs_ID)).Result;

                            BobisDone(location, "Trip is afgerond");
                            Bobs_Parties itemBobs = new Bobs_Parties()
                            {
                                Bobs_ID=MainViewVM.CurrentTrip.Bobs_ID,
                                Party_ID=MainViewVM.CurrentTrip.Party_ID,
                                Trips_ID=MainViewVM.CurrentTrip.ID,
                                Users_ID=MainViewVM.CurrentTrip.Users_ID
                            };

                            RatingDialog(itemBobs);
                        }
                    }


                }
                else
                {
                    //VindRitFilterVM.SelectedDestination.Location;
                    Trips_Locations item = new Trips_Locations()
                    {
                        Trips_ID =MainViewVM.CurrentTrip.ID,
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
           

            Trips_Locations item = new Trips_Locations()
            {
                Trips_ID =MainViewVM.CurrentTrip.ID,
                Location = JsonConvert.SerializeObject(location),
                Statuses_ID = VindRitVM.StatusID
            };
            Response ok = Task.FromResult<Response>(await TripRepository.PostLocation(item)).Result;
            Response active = Task.FromResult<Response>(await TripRepository.PutActive(item.Trips_ID, false)).Result;

            if (ok.Success == true && active.Success == true)
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
        private async Task GetBobTypes()
        {
            List<BobsType> lijst = await BobsRepository.GetTypes();
            VindRitFilterVM.SelectedBobsType = lijst.Where(r => r.ID == 2).First();
           
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
                            SetStatus(data.StatusID.Value);
                            MainViewVM.CurrentTrip = data;

                            this.EnableFind = false;
                            return;
                        }
                    }

                    SetStatus(0);
                    this.EnableFind = true;



                }
                catch (Exception ex)
                {
                    SetStatus(0);
                    this.EnableFind = true;
                   
                }
            }
            else
            {
                if (MainViewVM.CurrentTrip.StatusID.HasValue)
                {
                    SetStatus(MainViewVM.CurrentTrip.StatusID.Value);
                }
               
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
            if (lijst != null && lijst.Count!=0)
            {
                VindRitFilterVM.SelectedDestination = lijst.Where(r => r.Default == true).First();
            }
          
           



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
                Location location = await LocationService.GetCurrent();

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

                BobisDone(location, "Trip is geannuleerd");

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
        private void SetStatus(int statusID)
        {
            VindRitVM.StatusID = statusID;
            this.Status = GetStatusName(statusID);
            if(this.GetSelectedParty!=null && this.GetSelectedDestination != null)
            {
                Toast.Tile("Party: " + this.GetSelectedParty.Name,"Bestemming: " +  this.GetSelectedDestination.Name,"Status " + this.Status);
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
                    return  "Rit is gedaan";
                case 5:
                  return "Rit is geannuleerd";
                case 6://niet in db
                    return "Wachten op antwoord van bob";
                case 7:
                    return "Bob is in de buurt";
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
    }
}
