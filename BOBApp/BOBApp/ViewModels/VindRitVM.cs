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

namespace BOBApp.ViewModels
{
    public class VindRitVM : ViewModelBase
    {
        //Properties

        //static
        public static Party SelectedParty;
        public static Bob SelectedBob;
        public static Trip CurrentTrip;
        public static bool BobAccepted;
        public static int StatusID = 1;

        public static int Request;

        //others
        private Task task;
        public RelayCommand GoChatCommand { get; set; }
        public RelayCommand GoFilterCommand { get; set; }
        public RelayCommand FindBobCommand { get; set; }
        public string Error { get; set; }
        public string BobRequests { get; set; }
        public List<Party> Parties { get; set; }
        public string SelectedPartyString { get; set; }

       
        


        //Constructor
        public VindRitVM()
        {
            GoChatCommand = new RelayCommand(GoChat);
            GoFilterCommand = new RelayCommand(GoFilter);
            FindBobCommand = new RelayCommand(FindBob);

            task = GetParties();
            Task task2 = GetDestinations();
            RaisePropertyChanged("SelectedPartyString");
            BobRequests = "Momenteel 0 aanvragen";
            RaisePropertyChanged("BobRequests");

            Messenger.Default.Register<NavigateTo>(typeof(bool), ExecuteNavigatedTo);

            GetCurrentTrip();
        }










        //Methods
        private async void GetCurrentTrip()
        {
            if (VindRitVM.CurrentTrip == null)
            {
                try
                {
                    string json = await Localdata.read("trip.json");

                    var data = JsonConvert.DeserializeObject<Trip>(json);
                    VindRitVM.CurrentTrip = data;
                }
                catch (Exception ex)
                {


                }
            }
            
        }
        private async void ExecuteNavigatedTo(NavigateTo obj)
        {


            if (obj.Reload == true && (obj.Name == null || obj.Name == ""))
            {
                if (VindRitVM.BobAccepted == true)
                {

                    MakeTrip();
                    BobRequests = "Momenteel " + VindRitVM.Request.ToString() + " aanvragen";

                    VindRitVM.BobAccepted = false;
                }
            }
            else
            {
                if (obj.Name == "trip_location")
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
            }
        }
        #region  StartTripLocationTimer
        DispatcherTimer timer = new DispatcherTimer();
        private void StartTripLocationTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 2, 0);
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
                Response farEnough = Task.FromResult<Response>(await TripRepository.Difference(VindRitFilterVM.SelectedDestination.Location,location)).Result;

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
                    if (ok.Success == false)
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
                    MainViewVM.socket.Emit("trip_DONE", JsonConvert.SerializeObject(socketSend));
                }
                
            }
        }

        private async Task<bool> OnDestination()
        {
            var dialog = new MessageDialog("Bent u toegekomen op de locatie?");

            dialog.Commands.Add(new UICommand("Yes") { Id = 0 });
            dialog.Commands.Add(new UICommand("No") { Id = 1 });

            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily != "Windows.Mobile")
            {
                // Adding a 3rd command will crash the app when running on Mobile !!!
                dialog.Commands.Add(new UICommand("Maybe later") { Id = 2 });
            }

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

        private async Task<Boolean> GetParties()
        {

            this.Parties = await PartyRepository.GetParties();

            if (this.Parties.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
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


        private async void FindBob()
        {
            List<Bob> bobs = Task.FromResult<List<Bob>>(await FindBob_task()).Result;

            while (bobs.Count > 0)
            {
                bool ok = Task.FromResult<bool>(await ShowBob(bobs.First())).Result;
                if (ok == false)
                {
                    bobs.Remove(bobs.First());
                    if (bobs.Count == 0)
                    {
                        var dialog = new MessageDialog("Geen Bob gevonden");
                        await dialog.ShowAsync();
                    }
                }
                else
                {
                    //take this bob
                    VindRitVM.SelectedBob = bobs.First();

                    Bob.All bob = Task.FromResult<Bob.All>(await BobsRepository.GetBobById(VindRitVM.SelectedBob.ID.Value)).Result;
                    Libraries.Socket socketSend =new Libraries.Socket() { From=MainViewVM.USER.ID, To = bob.User.ID, Status = true };
                    MainViewVM.socket.Emit("findBob_DONE", JsonConvert.SerializeObject(socketSend));
                    break;

                }

            }
        }



        private async Task<bool> ShowBob(Bob bob)
        {
            var dialog = new MessageDialog(bob.LicensePlate);

            dialog.Commands.Add(new UICommand("Take bob") { Id = 0 });
            dialog.Commands.Add(new UICommand("Next bob") { Id = 1 });

            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily != "Windows.Mobile")
            {
                // Adding a 3rd command will crash the app when running on Mobile !!!
                dialog.Commands.Add(new UICommand("Maybe later") { Id = 2 });
            }

            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            var result = await dialog.ShowAsync();

            int id = int.Parse(result.Id.ToString());
            if(id == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //wanneer bob accepteer
        private async void MakeTrip()
        {
            Bob selectedBob = VindRitVM.SelectedBob;
            Party SelectedParty = VindRitVM.SelectedParty;
            Users_Destinations SelectedDestination = VindRitFilterVM.SelectedDestination;
            List<Friend.All> selectedFriends = VindRitFilterVM.SelectedFriends;
            BobsType type=VindRitFilterVM.SelectedBobsType;
            DateTime? minDate=VindRitFilterVM.SelectedMinDate;
            int? rating=VindRitFilterVM.SelectedRating;

      
            //destinations edit

            Trip trip = new Trip()
            {
                Bobs_ID = selectedBob.ID.Value,
                Friends = JsonConvert.SerializeObject(selectedFriends),
                Destinations_ID = SelectedDestination.Destinations_ID
            };
            Response res = Task.FromResult<Response>(await TripRepository.PostTrip(trip)).Result;
            
            if (res.Success == true)
            {
              
                Trip currentTrip = Task.FromResult<Trip>(await TripRepository.GetCurrentTrip()).Result;

                var data = JsonConvert.SerializeObject(CurrentTrip);
                bool ok = Task.FromResult<bool>(await Localdata.save("trip.json", data)).Result;

                if (ok == true && currentTrip != null)
                    VindRitVM.CurrentTrip = currentTrip;
                {
                    //update very minuten location for trip
                    Messenger.Default.Send<NavigateTo>(new NavigateTo()
                    {
                        Name = "trip_location"
                    });


                    MakeChatroom(selectedBob.ID.Value);
                }

                
            }
        }

       

        private async void MakeChatroom(int bobs_ID)
        {
            Response res = Task.FromResult<Response>(await ChatRoomRepository.PostChatRoom(bobs_ID)).Result;

            if (res.Success == true)
            {
                VindRitChatVM.ID = res.NewID.Value;

                

                var definition = new { ID = res.NewID.Value };
                var data = JsonConvert.SerializeObject(definition);
                await Localdata.save("chatroom.json", data);
               

                Frame rootFrame = MainViewVM.MainFrame as Frame;

                rootFrame.Navigate(typeof(VindRitChat));
            }
           
        }


        //tasks
        private async Task<List<Bob>> FindBob_task()
        {
            if (this.SelectedPartyString != null)
            {
                VindRitVM.SelectedParty = Parties.Where(r => r.Name == this.SelectedPartyString).First();
            }
           
            int? rating = null;
            DateTime minDate = DateTime.Today;
            int bobsType_ID = 1;
            Location location = new Location() { Latitude = 51.177134640708495, Longitude = 3.2177382568767574 };
            int? maxDistance = null;

            List<Bob> bobs = await BobsRepository.FindBobs(rating, minDate, bobsType_ID, location, maxDistance);

          

            return bobs;
        }

        private async Task GetDestinations()
        {
            List<Users_Destinations> lijst = await DestinationRepository.GetDestinations();
            VindRitFilterVM.SelectedDestination = lijst[0];
        }

    }
}
