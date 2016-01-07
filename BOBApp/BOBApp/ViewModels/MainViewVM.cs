using BOBApp.Messages;
using BOBApp.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Libraries;
using Libraries.Models;
using Libraries.Models.relations;
using Libraries.Repositories;
using Newtonsoft.Json;
using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace BOBApp.ViewModels
{

    public class MainViewVM : ViewModelBase
    {
        #region props
        public static Frame MainFrame;
        public static DispatcherTimer TIMER = new DispatcherTimer();
        public static User USER;
        public static ChatRoom ChatRoom;
        public static Trip CurrentTrip;
        public static Quobject.SocketIoClientDotNet.Client.Socket socket;
        public static Libraries.Socket LatestSocket;
        public static int searchArea = 100000;

        public static class Pins
        {
            public static RandomAccessStreamReference UserPin = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/userpin.png"));
            public static RandomAccessStreamReference FeestPin = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/feestpin.png"));
            public static RandomAccessStreamReference BobPin = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/BOBpin.png"));
            public static RandomAccessStreamReference HomePin = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/home.png"));
        }


        public User User { get; set; }
        public RelayCommand LogOffCommand { get; set; }
        private IBackgroundTaskRegistration regTask = null;
        private object currentTrip;

        public double Points { get; set; }
        public bool Loading { get; set; }




        #endregion

        //Constructor
        public MainViewVM()
        {
            User = MainViewVM.USER;

            LogOffCommand = new RelayCommand(LogOff);


            Messenger.Default.Register<NavigateTo>(typeof(bool), ExecuteNavigatedTo);
            this.Loading = false;
            RaisePropertyChanged("User");
        }

        #region socket
        private void StartSocket()
        {


            //to bob
            MainViewVM.socket.On("bob_ACCEPT", (msg) =>
            {
                Libraries.Socket _socket = JsonConvert.DeserializeObject<Libraries.Socket>((string)msg);
                if (_socket.Status == true && _socket.To == MainViewVM.USER.ID)
                //if (_socket.Status == true)
                {
                    MainViewVM.LatestSocket = _socket;
                    VindRitBobVM.Request = VindRitBobVM.Request + 1;

                    Bob_Accept(_socket.From, _socket.ID);

                }

            });

            MainViewVM.socket.On("disconnect", (msg) =>
            {
                LogOff();
            });
            //not implemented
            MainViewVM.socket.On("user_location_NEW", (msg) =>
            {
                Libraries.Socket _socket = JsonConvert.DeserializeObject<Libraries.Socket>((string)msg);
                if (_socket.Status == true && _socket.To == MainViewVM.USER.ID)
                //if (_socket.Status == true)
                {

                    Messenger.Default.Send<NavigateTo>(new NavigateTo()
                    {
                        Name = "map_reload",
                    });

                }

            });
            //to bob
            MainViewVM.socket.On("trip_START", async (msg) =>
            {
                Libraries.Socket _socket = JsonConvert.DeserializeObject<Libraries.Socket>((string)msg);
                if (_socket.Status == true && _socket.To == MainViewVM.USER.ID)
                //if (_socket.Status == true)
                {

                    User.All fromUser = Task.FromResult<User.All>(await UsersRepository.GetUserById(_socket.From)).Result;
                    Trip currentTrip = JsonConvert.DeserializeObject<Trip>(_socket.Object.ToString());

                    TripSave(currentTrip);

                }

            });
            //to bob
            MainViewVM.socket.On("trip_DONE", async (msg) =>
            {
                Libraries.Socket _socket = JsonConvert.DeserializeObject<Libraries.Socket>((string)msg);
                if (_socket.Status == true && _socket.To == MainViewVM.USER.ID)
                //if (_socket.Status == true)
                {
                    User.All fromUser = Task.FromResult<User.All>(await UsersRepository.GetUserById(_socket.From)).Result;

                    if (fromUser != null)
                    {
                        if (fromUser.User.IsBob.Value == true && (bool)_socket.Object2 == true)
                        {
                            Messenger.Default.Send<NavigateTo>(new NavigateTo()
                            {
                                Name = "rating_dialog",
                                View = typeof(VindRit),
                                Data = _socket.Object
                            });

                        }
                        else
                        {
                            TripDone();
                        }
                    }
                    else
                    {
                        TripDone();
                    }

                }

            });
            //to bob and user
            MainViewVM.socket.On("friend_REQUEST", async (msg) =>
            {
                Libraries.Socket _socket = JsonConvert.DeserializeObject<Libraries.Socket>((string)msg);
                if (_socket.Status == true && _socket.To == MainViewVM.USER.ID)
                //if (_socket.Status == true)
                {
                    //friend add
                    User.All fromUser = Task.FromResult<User.All>(await UsersRepository.GetUserById(_socket.From)).Result;

                    FriendRequest(fromUser);
                }

            });
            MainViewVM.socket.On("friend_ADDED", async (msg) =>
            {
                Libraries.Socket _socket = JsonConvert.DeserializeObject<Libraries.Socket>((string)msg);
                if (_socket.Status == true && _socket.To == MainViewVM.USER.ID)
                //if (_socket.Status == true)
                {
                    //friend add
                    User.All fromUser = Task.FromResult<User.All>(await UsersRepository.GetUserById(_socket.From)).Result;

                    Messenger.Default.Send<Dialog>(new Dialog()
                    {
                        Message = fromUser.User.ToString() + " heeft u vriendschapsverzoek geaccepteerd",
                        ViewOk=typeof(ZoekVrienden)
                    });
                }

            });



            MainViewVM.socket.On("status_UPDATE", (msg) =>
            {
                Libraries.Socket _socket = JsonConvert.DeserializeObject<Libraries.Socket>((string)msg);

                if (_socket.Status == true && _socket.To == MainViewVM.USER.ID)
                //if (_socket.Status == true)
                {
                    //from bob
                    if (VindRitVM.FindID == _socket.ID)
                    {
                        Messenger.Default.Send<NavigateTo>(new NavigateTo()
                        {
                            Name = "get_trip",
                            View = typeof(VindRit)
                        });


                    }
                }

            });

            //to user
            MainViewVM.socket.On("trip_MAKE", async (msg) =>
            {
                Libraries.Socket _socket = JsonConvert.DeserializeObject<Libraries.Socket>((string)msg);

                if (_socket.Status == true && _socket.To == MainViewVM.USER.ID)
                //if (_socket.Status == true)
                {
                    //from bob
                    if (VindRitVM.FindID == _socket.ID)
                    {

                        User.All fromBob = Task.FromResult<User.All>(await UsersRepository.GetUserById(_socket.From)).Result;
                        User.All user = JsonConvert.DeserializeObject<User.All>(_socket.Object.ToString());
                        Trip newTrip = (Trip)GetTripObject();

                        if (newTrip != null)
                        {
                            MakeTrip(newTrip, fromBob.Bob.ID.Value);
                        }

                    }
                }

            });
            //to bob and user
            MainViewVM.socket.On("chatroom_OPEN", async (msg) =>
            {
                Libraries.Socket _socket = JsonConvert.DeserializeObject<Libraries.Socket>((string)msg);
                if (_socket.Status == true && _socket.To == MainViewVM.USER.ID)
                //if (_socket.Status == true)
                {
                    //friend add

                    User.All fromUser = Task.FromResult<User.All>(await UsersRepository.GetUserById(_socket.From)).Result;
                    int bobsID = JsonConvert.DeserializeObject<int>(_socket.Object.ToString());


                    OpenChatroom(bobsID);


                }
            });

            MainViewVM.socket.On("chatroom_COMMENT", async (msg) =>
            {
                Libraries.Socket _socket = JsonConvert.DeserializeObject<Libraries.Socket>((string)msg);
                if (_socket.Status == true && _socket.To == MainViewVM.USER.ID)
                //if (_socket.Status == true)
                {
                    User.All fromUser = Task.FromResult<User.All>(await UsersRepository.GetUserById(_socket.From)).Result;

                    Messenger.Default.Send<NavigateTo>(new NavigateTo()
                    {
                        Name = "newComment"
                    });


                    if ((bool)_socket.Object2 == true && fromUser != null)
                    {
                        Messenger.Default.Send<Dialog>(new Dialog()
                        {
                            Message = fromUser.User.ToString() + " zegt: " + _socket.Object.ToString(),
                            Ok = "Antwoord",
                            Nok = "Negeer",
                            ViewOk = typeof(VindRitChat),
                            ViewNok = null,
                            ParamView = false,
                            Cb = null,
                            IsNotification = true
                        });
                    }
                }



            });
        }

        private async void TripSave(Trip trip)
        {
            if (trip != null)
            {
                await UserRepository.PostPoint();
                var data = JsonConvert.SerializeObject(trip);

                bool ok = Task.FromResult<bool>(await Localdata.save("trip.json", data)).Result;


                Messenger.Default.Send<NavigateTo>(new NavigateTo()
                {
                    Name = "newtrip_bob",
                    Data = trip
                });
            }

        }



        private void FriendRequest(User.All fromUser)
        {

            Messenger.Default.Send<Dialog>(new Dialog()
            {
                Message = fromUser.User.ToString() + " wilt u toevoegen als vriend",
                Ok = "Accept",
                Nok = "Ignore",
                Data = JsonConvert.SerializeObject(fromUser),
                ViewOk = typeof(ZoekVrienden),
                Cb = "friend_accepted",
                IsNotification = true
            });
        }

        private async void TripDone()
        {
            var definition = new { ID = -1, UserID = -1 };
            var data = JsonConvert.SerializeObject(definition);
            var data2 = JsonConvert.SerializeObject(new Trip() { ID = -1 });


            bool ok_chatroom = Task.FromResult<bool>(await Localdata.save("chatroom.json", data)).Result;
            bool ok_trip = Task.FromResult<bool>(await Localdata.save("trip.json", data2)).Result;


            //alles op null zetten, van start
            VindRitFilterVM.SelectedDestination = new Users_Destinations();
            MainViewVM.ChatRoom = null;
            VindRitChatVM.ID = null;
            VindRitVM.SelectedParty = null;
            VindRitVM.SelectedBob = null;
            VindRitVM.SelectedUser = null;
            VindRitVM.BobAccepted = false;
            VindRitVM.StatusID = 0;

            MainViewVM.CurrentTrip = null;

            Toast.TileClear();



            Frame rootFrame = MainViewVM.MainFrame as Frame;

            if (MainViewVM.USER.IsBob == true)
            {

                Messenger.Default.Send<NavigateTo>(new NavigateTo()
                {
                    Name = "loaded",
                    View = typeof(VindRitBob)

                });
            }
            else
            {
                Messenger.Default.Send<NavigateTo>(new NavigateTo()
                {
                    Name = "loaded",
                    View = typeof(VindRit)
                });
            }


        }

        private async void OpenChatroom(int chatroomID)
        {
            var definition = new { ID = chatroomID, UserID = MainViewVM.USER.ID };
            var data = JsonConvert.SerializeObject(definition);
            bool ok_chatroom = Task.FromResult<bool>(await Localdata.save("chatroom.json", data)).Result;

            if (ok_chatroom == true)
            {
                //#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
                //                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                //                {
                //                    Frame rootFrame = MainViewVM.MainFrame as Frame;
                //                    rootFrame.Navigate(typeof(VindRitChat),true);
                //                });
                //#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            }


        }

        private async void Bob_Accept(int from, float id)
        {
            User user = Task.FromResult<User.All>(await UsersRepository.GetUserById(from)).Result.User;

            VindRitVM.SelectedUser = user;
            if (user != null)
            {

                Messenger.Default.Send<Dialog>(new Dialog()
                {
                    Message = user.ToString() + " wilt u als bob",
                    Ok = "Accept",
                    Nok = "Ignore",
                    ParamView = true,
                    Cb = "bob_accepted",
                    Data = id,
                    IsNotification = true
                });
            }
            else
            {
                //error
            }
        }
        #endregion



        private async void ExecuteNavigatedTo(NavigateTo obj)
        {
            if (obj.Name == "loaded")
            {
                Type view = (Type)obj.View;
                if (view == typeof(MainView))
                {
                    //loaded
                    Loaded();
                }
            }
            if (obj.Reload == true)
            {
                Type view = (Type)obj.View;
                if (view == typeof(MainView))
                {
                    //loaded
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
                    //default on start
                    User = MainViewVM.USER;
                    SetupBackgroundTask();
                    StartSocket();

                    GetPoints();
                    await PostLocation();

                    this.Loading = false;
                    RaisePropertyChanged("Loading");
                    RaisePropertyChanged("User");

                });
#pragma warning restore CS1998

            });
        }



        //Methods



        #region methods

        #region background
        private async void SetupBackgroundTask()
        {
            string BackgroundTaskName = "UserLocation";
            string BackgroundTaskEntryPoint = "BobWorker.Service";




            foreach (var item in BackgroundTaskRegistration.AllTasks)
            {
                if (item.Value.Name == BackgroundTaskName)
                {
                    regTask = item.Value;
                }
            }

            if (regTask != null)
            {
                regTask.Completed += RegTask_Completed;
            }
            else
            {
                await RegisterTask(BackgroundTaskName, BackgroundTaskEntryPoint);
            }

        }

        private async Task RegisterTask(string BackgroundTaskName, string BackgroundTaskEntryPoint)
        {
            try
            {
                BackgroundAccessStatus status = await BackgroundExecutionManager.RequestAccessAsync();
                if (status == BackgroundAccessStatus.Denied)
                {
                    //geen toegang

                }
                else
                {
                    //toegang



                    BackgroundTaskBuilder builder = new BackgroundTaskBuilder();
                    builder.Name = BackgroundTaskName;
                    builder.TaskEntryPoint = BackgroundTaskEntryPoint;

                    var trigger = new TimeTrigger(15, false);// om de 2 minuten


                    builder.SetTrigger(trigger);

                    var condition = new SystemCondition(SystemConditionType.InternetAvailable);

                    builder.AddCondition(condition);


                    regTask = builder.Register();
                    regTask.Completed += RegTask_Completed;

                }
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex);
            }
        }

        private async void RegTask_Completed(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            Debug.WriteLine("Niewue locatie gestuurd");
        }
        #endregion

        private void LogOff()
        {
            Task<bool> task = LogOffUser();
        }
        private async Task<Boolean> LogOffUser()
        {
            Response res = await LoginRepository.LogOff();
            MainViewVM.socket.Disconnect();

            Messenger.Default.Send<GoToPage>(new GoToPage()
            {
                Name = "Login"
            });

            return res.Success;
        }
        private async void GetPoints()
        {
            try
            {
                string points = await PointRepository.GetTotalPoints();
                this.Points = double.Parse(points);
            }
            catch (Exception ex)
            {


            }
            RaisePropertyChanged("Points");
        }

        //by user
        //wanneer bob accepteer, wordt door gebruiker die aanvraagd aangemaakt
        private async void MakeTrip(Trip trip, int bobID)
        {
            Response res = Task.FromResult<Response>(await TripRepository.PostTrip(trip)).Result;

            if (res.Success == true)
            {
                VindRitVM.StatusID = 1;
                Location location = await LocationService.GetCurrent();
                Trips_Locations tripL = new Trips_Locations()
                {
                    Trips_ID = res.NewID.Value,
                    Location = JsonConvert.SerializeObject(location),
                    Statuses_ID = VindRitVM.StatusID
                };

                Response okTripL = await TripRepository.PostLocation(tripL);


                Trip currentTrip = Task.FromResult<Trip>(await TripRepository.GetCurrentTrip()).Result;


                Bob.All bob = VindRitVM.SelectedBob;
                Libraries.Socket socketSendToBob = new Libraries.Socket()
                {
                    From = MainViewVM.USER.ID,//from user
                    To = bob.User.ID,//to bob
                    Status = true,
                    Object = currentTrip
                };


                MainViewVM.socket.Emit("trip_START:send", JsonConvert.SerializeObject(socketSendToBob)); //bob
                StartTrip(currentTrip); //user
                TripSave(currentTrip);


                MakeChatroom(bobID);

            }
        }

        private void StartTrip(Trip currentTrip)
        {
            if (currentTrip != null)
            {
                MainViewVM.CurrentTrip = currentTrip;
                //update very minuten location for trip
                Messenger.Default.Send<NavigateTo>(new NavigateTo()
                {
                    Name = "trip_location"
                });


            }
        }



        private async void MakeChatroom(int bobs_ID)
        {
            Response res = Task.FromResult<Response>(await ChatRoomRepository.PostChatRoom(bobs_ID)).Result;

            if (res.Success == true)
            {
                VindRitChatVM.ID = res.NewID.Value;

                Bob.All bob = VindRitVM.SelectedBob;
                Libraries.Socket socketSendToBob = new Libraries.Socket()
                {
                    From = MainViewVM.USER.ID,//from user
                    To = bob.User.ID,//to bob
                    Status = true,
                    Object = res.NewID.Value
                };

                Libraries.Socket socketSendToUser = new Libraries.Socket()
                {
                    From = bob.User.ID,//from bob
                    To = MainViewVM.USER.ID,//to user
                    Status = true,
                    Object = res.NewID.Value
                };


                MainViewVM.socket.Emit("chatroom_OPEN:send", JsonConvert.SerializeObject(socketSendToBob)); //bob
                MainViewVM.socket.Emit("chatroom_OPEN:send", JsonConvert.SerializeObject(socketSendToUser)); //bob
            }

        }


        private async Task PostLocation()
        {
            Location location = await LocationService.GetCurrent();
            Response ok = await UserRepository.PostLocation(location);

        }

        private object GetTripObject()
        {
            try
            {
                Bob selectedBob = VindRitVM.SelectedBob.Bob;
                Party SelectedParty = VindRitVM.SelectedParty;
                Users_Destinations SelectedDestination = VindRitFilterVM.SelectedDestination;
                List<Friend.All> selectedFriends = VindRitFilterVM.SelectedFriends;
                BobsType type = VindRitFilterVM.SelectedBobsType;
                DateTime? minDate = VindRitFilterVM.SelectedMinDate;
                int? rating = VindRitFilterVM.SelectedRating;

                if (SelectedDestination == null || selectedBob == null || SelectedParty == null || type == null)
                {

                    //trip afronden
                    TripDone();


                    return null;
                }

                //destinations edit

                Trip trip = new Trip()
                {
                    Bobs_ID = selectedBob.ID.Value,
                    Party_ID = SelectedParty.ID,
                    Friends = JsonConvert.SerializeObject(selectedFriends),
                    Destinations_ID = SelectedDestination.Destinations_ID
                };

                return trip;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message.ToString());
                return null;
                //return null;
            }

        }

        #endregion
    }
}
