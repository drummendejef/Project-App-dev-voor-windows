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

namespace BOBApp.ViewModels
{
    public class VindRitVM : ViewModelBase
    {
        //Properties
    
        //static
        public static Party SelectedParty;
        public static Bob SelectedBob;
        public static User SelectedUser;
        public static Trip CurrentTrip = new Trip();
        public static bool BobAccepted;
        public static int StatusID = 1;

        public static int Request;

        //filter
        public static class Filter
        {
            public static List<Friend.All> SelectedFriends = new List<Friend.All>();
            public static Users_Destinations SelectedDestination = new Users_Destinations();
            public static BobsType SelectedBobsType = new BobsType();
            public static int SelectedRating = 0;
            public static DateTime? SelectedMinDate = DateTime.Today;
        }

        //others
        private Task task;
        public RelayCommand GoChatCommand { get; set; }
        public RelayCommand GoFilterCommand { get; set; }
        public RelayCommand FindBobCommand { get; set; }
        public string Error { get; set; }
        public string BobRequests { get; set; }
        public List<Party> Parties { get; set; }
        public string SelectedPartyString { get; set; }

        public bool Loading { get; set; }



        //Constructor
        public VindRitVM()
        {
            GoChatCommand = new RelayCommand(GoChat);
            GoFilterCommand = new RelayCommand(GoFilter);
            FindBobCommand = new RelayCommand(FindBob);

            task = GetParties();
            Task task2 = GetDestinations();
            Task task3 = GetBobTypes();
            RaisePropertyChanged("SelectedPartyString");
            BobRequests = "Momenteel 0 aanvragen";
            RaisePropertyChanged("BobRequests");

            Messenger.Default.Register<NavigateTo>(typeof(bool), ExecuteNavigatedTo);

            GetCurrentTrip();
        }



        //Methods

        #region navigate to
        private async void ExecuteNavigatedTo(NavigateTo obj)
        {
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
                        find_bob((bool) obj.Result);
                        break;
                    case "bob_accepted":
                        bob_accepted((bool) obj.Result);
                        break;
                    default:
                        break;
                }
            }
        }

        private async void bob_accepted(bool accepted)
        {
            //uitgevoerd bij de bob
            if (accepted == true)
            {
                //verstuur trip
                User.All user = Task.FromResult<User.All>(await UsersRepository.GetUserById(VindRitVM.SelectedUser.ID)).Result;
                Libraries.Socket socketSend = new Libraries.Socket() {
                    From = MainViewVM.USER.ID,//from bob
                    To = user.User.ID,//to user
                    Status = true,
                    Object=user
                };
 
                   MainViewVM.socket.Emit("trip_MAKE:send", JsonConvert.SerializeObject(socketSend));


            }
            else
            {

            }
        }

        private async void find_bob(bool ok)
        {
            if (ok == false)
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
                }
                else
                {
                    ShowBob(bobs.First());
                }
            }
            else
            {
                //take this bob
                VindRitVM.SelectedBob = bobs.First();

                Bob.All bob = Task.FromResult<Bob.All>(await BobsRepository.GetBobById(VindRitVM.SelectedBob.ID.Value)).Result;
                Libraries.Socket socketSend = new Libraries.Socket() { From = MainViewVM.USER.ID, To = bob.User.ID, Status = true };
                MainViewVM.socket.Emit("bob_ACCEPT:send", JsonConvert.SerializeObject(socketSend));


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

            if (location != null)
            {
                //checkhowfaraway
                Response farEnough = Task.FromResult<Response>(await TripRepository.Difference((Location)VindRitVM.Filter.SelectedDestination.Location,location)).Result;

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
                        Libraries.Socket socketSend = new Libraries.Socket() { From = MainViewVM.USER.ID, To = MainViewVM.LatestSocket.From, Status = true };
                        MainViewVM.socket.Emit("trip_UPDATE:send", JsonConvert.SerializeObject(socketSend));
                    }
                    else
                    {
                        timer.Stop();
                       
                    }

                    bool done = Task.FromResult<bool>(await OnDestination()).Result;
                    if (done == true)
                    {
                        BobisDone(location);
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

        private async void BobisDone(Location location)
        {
            Trips_Locations item = new Trips_Locations()
            {
                Trips_ID = VindRitVM.CurrentTrip.ID,
                Location = JsonConvert.SerializeObject(location),
                Statuses_ID = VindRitVM.StatusID
            };
            Response ok = Task.FromResult<Response>(await TripRepository.PostLocation(item)).Result;
            Response active = Task.FromResult<Response>(await TripRepository.PutActive(item.Trips_ID,false)).Result;

            if (ok.Success == true && active.Success==true)
            {
                var dialog = new MessageDialog("Trip is afgerond");

                dialog.Commands.Add(new UICommand("Ok") { Id = 0 });

               

                dialog.DefaultCommandIndex = 0;

                var result = await dialog.ShowAsync();

                int id = int.Parse(result.Id.ToString());
                if (id == 0)
                {
                    Libraries.Socket socketSend = new Libraries.Socket() { From = MainViewVM.USER.ID, To = MainViewVM.LatestSocket.From, Status = true };
                    MainViewVM.socket.Emit("trip_DONE:send", JsonConvert.SerializeObject(socketSend));
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
            VindRitVM.Filter.SelectedBobsType = lijst[0];
        }
        private async void GetCurrentTrip()
        {
            if (VindRitVM.CurrentTrip == null)
            {
                try
                {
                    string json = await Localdata.read("trip.json");

                    var data = JsonConvert.DeserializeObject<Trip>(json);
                    if (data.ID != -1)
                    {
                        VindRitVM.CurrentTrip = data;
                    }
                   
                }
                catch (Exception ex)
                {


                }
            }

        }

        private async Task<Boolean> GetParties()
        {

            this.Parties = await PartyRepository.GetParties();
            VindRitVM.SelectedParty = this.Parties[0];

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
            VindRitVM.Filter.SelectedDestination = lijst[0];
        }

        #endregion



        #region navigate

        private void GoChat()
        {
            Frame rootFrame = MainViewVM.MainFrame as Frame;

            rootFrame.Navigate(typeof(VindRitChat));
        }
        private void GoFilter()
        {
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
            if (this.SelectedPartyString != null)
            {
                VindRitVM.SelectedParty = Parties.Where(r => r.Name == this.SelectedPartyString).First();
            }

            //location
            Geolocator geolocator = new Geolocator();
            Geoposition pos = await geolocator.GetGeopositionAsync();


            int? rating = VindRitVM.Filter.SelectedRating;
            DateTime minDate = DateTime.Today; //moet nog gedaan worden
            int bobsType_ID = VindRitVM.Filter.SelectedBobsType.ID;
            Location location = new Location() { Latitude = pos.Coordinate.Point.Position.Latitude, Longitude = pos.Coordinate.Point.Position.Longitude };
            int? maxDistance = MainViewVM.searchArea;

            List<Bob> bobs = await BobsRepository.FindBobs(rating, minDate, bobsType_ID, location, maxDistance);



            return bobs;
        }
        private async void FindBob()
        {
            this.Loading = true;
            RaisePropertyChanged("Loading");

            bobs = Task.FromResult<List<Bob>>(await FindBob_task()).Result;

            this.Loading = false;
            RaisePropertyChanged("Loading");

            if (bobs.Count <= 0) {
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

            }else
            {
                ShowBob(bobs.First());

            }

        }



        private async void ShowBob(Bob bob)
        {
            Bob.All bob_full = Task.FromResult<Bob.All>(await BobsRepository.GetBobById(bob.ID.Value)).Result;
            string bob_text = bob_full.User.ToString() + " is in de buurt met volgende nummerplaat " + bob_full.Bob.LicensePlate + " voor een bedrag van " + bob_full.Bob.PricePerKm + " euro per km.";

            if (bob_full != null)
            {
                Messenger.Default.Send<Dialog>(new Dialog()
                {
                    Message = "Volgende bob gevonden: " + "\n" + bob_text,
                    Ok = "Take bob",
                    Nok = "Next bob",
                    ViewOk = null,
                    ViewNok = null,
                    ParamView = false,
                    Cb = "find_bob"
                });
            }




        }

        #endregion


       

        #region methods

       
        #endregion

       

      

    }
}
