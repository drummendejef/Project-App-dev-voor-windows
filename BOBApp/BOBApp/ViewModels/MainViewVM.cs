﻿using BOBApp.Messages;
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
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace BOBApp.ViewModels
{

    public class MainViewVM:ViewModelBase
    {
        //Properties
        //public static
        public static Frame MainFrame;
        public static User USER;
        public static ChatRoom ChatRoom;
        public static Quobject.SocketIoClientDotNet.Client.Socket socket;
        public static Libraries.Socket LatestSocket;
        public static int searchArea = 10000;

        private Task logoffTask;
        public User User { get; set; }
        public RelayCommand LogOffCommand { get; set; }
        public double Points { get; set; }
        public bool Loading { get; set; }

        private IBackgroundTaskRegistration regTask = null;



        //Constructor
        public MainViewVM()
        {
            User = MainViewVM.USER;
            GetPoints();

            SetupBackgroundTask();
            LogOffCommand = new RelayCommand(LogOff);
            RaisePropertyChanged("User");


    


            MainViewVM.socket = IO.Socket(URL.SOCKET);
            MainViewVM.socket.Connect();

            //to bob
            MainViewVM.socket.On("bob_ACCEPT", (msg) =>
            {
                Libraries.Socket _socket = JsonConvert.DeserializeObject<Libraries.Socket>((string)msg);
                if (_socket.Status == true && _socket.To==MainViewVM.USER.ID)
                //if (_socket.Status == true)
                {
                    MainViewVM.LatestSocket = _socket;
                    VindRitVM.Request = VindRitVM.Request + 1;

                    Bob_Accept(_socket.From);

                }
               
            });

            MainViewVM.socket.On("disconnect", (msg) =>
            {
                LogOff();
            });
            //to bob
            MainViewVM.socket.On("trip_START", async (msg) =>
            {
                Libraries.Socket _socket = JsonConvert.DeserializeObject<Libraries.Socket>((string)msg);
                if (_socket.Status == true && _socket.To==MainViewVM.USER.ID)
                //if (_socket.Status == true)
                {
                    User.All fromUser = Task.FromResult<User.All>(await UsersRepository.GetUserById(_socket.From)).Result;
                    Trip currentTrip = JsonConvert.DeserializeObject<Trip>(_socket.Object.ToString());

                    //StartTrip(currentTrip);

                }

            });
            //to bob
            MainViewVM.socket.On("trip_DONE", async (msg) =>
            {
                Libraries.Socket _socket = JsonConvert.DeserializeObject<Libraries.Socket>((string)msg);
                if (_socket.Status == true && _socket.To==MainViewVM.USER.ID)
                //if (_socket.Status == true)
                {
                    User.All fromUser = Task.FromResult<User.All>(await UsersRepository.GetUserById(_socket.From)).Result;

                    TripDone();
                }

            });
            //to bob and user
            MainViewVM.socket.On("friend_REQUEST", async (msg) =>
            {
                Libraries.Socket _socket = JsonConvert.DeserializeObject<Libraries.Socket>((string)msg);
                if (_socket.Status == true && _socket.To==MainViewVM.USER.ID)
                //if (_socket.Status == true)
                {
                    //friend add
                    User.All fromUser = Task.FromResult<User.All>(await UsersRepository.GetUserById(_socket.From)).Result;


                }

            });

            //to user
            MainViewVM.socket.On("trip_MAKE", async (msg) =>
            {
                Libraries.Socket _socket = JsonConvert.DeserializeObject<Libraries.Socket>((string)msg);
                
                if (_socket.Status == true && _socket.To==MainViewVM.USER.ID)
                //if (_socket.Status == true)
                {
                    //from bob
                    User.All fromBob = Task.FromResult<User.All>(await UsersRepository.GetUserById(_socket.From)).Result;
                    User.All user = JsonConvert.DeserializeObject<User.All>(_socket.Object.ToString());
                    Trip newTrip =(Trip) GetTripObject();


                    MakeTrip(newTrip, fromBob.Bob.ID.Value);
                }

            });
            //to bob and user
            MainViewVM.socket.On("chatroom_OPEN", async (msg) =>
            {
                Libraries.Socket _socket = JsonConvert.DeserializeObject<Libraries.Socket>((string)msg);
                if (_socket.Status == true && _socket.To==MainViewVM.USER.ID)
                //if (_socket.Status == true)
                {
                    //friend add
                    
                    User.All fromUser = Task.FromResult<User.All>(await UsersRepository.GetUserById(_socket.From)).Result;
                    int bobsID = JsonConvert.DeserializeObject<int>(_socket.Object.ToString());


                    OpenChatroom(bobsID);


                }
            });

            MainViewVM.socket.On("chatroom_COMMENT", (msg) =>
            {
                Libraries.Socket _socket = JsonConvert.DeserializeObject<Libraries.Socket>((string)msg);
                if (_socket.Status == true && _socket.To == MainViewVM.USER.ID)
                //if (_socket.Status == true)
                {
                    Messenger.Default.Send<NavigateTo>(new NavigateTo()
                    {
                        Name = "newComment"
                    });



                }
            });

        }

        private async void TripDone()
        {
            var definition = new { ID =-1 , UserID =-1};
            var data = JsonConvert.SerializeObject(definition);
            var data2 = JsonConvert.SerializeObject(new Trip() { ID = -1 });
           

            bool ok_chatroom = Task.FromResult<bool>(await Localdata.save("chatroom.json", data)).Result;
            bool ok_trip = Task.FromResult<bool>(await Localdata.save("trip.json", data2)).Result;


            VindRitVM.Filter.SelectedDestination = new Users_Destinations();
            MainViewVM.ChatRoom = null;
            VindRitChatVM.ID = null;
            //alles op null zetten, van start

        }

        private async void OpenChatroom(int chatroomID)
        {
            var definition = new { ID = chatroomID, UserID= MainViewVM.USER.ID };
            var data = JsonConvert.SerializeObject(definition);
            bool ok_chatroom = Task.FromResult<bool>(await Localdata.save("chatroom.json", data)).Result;

            if (ok_chatroom == true)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    Frame rootFrame = MainViewVM.MainFrame as Frame;
                    rootFrame.Navigate(typeof(VindRitChat),true);
                });
            }

           
        }

        private async void Bob_Accept(int from)
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
                    Cb= "bob_accepted"
                });
            }
            else
            {
                //error
            }
        }









        //Methods




        private void LogOff()
        {
            logoffTask = LogOffUser();
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
            string points = await PointRepository.GetTotalPoints();

            this.Points = double.Parse(points);
        }
        private void SetupBackgroundTask()
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
                RegisterTask(BackgroundTaskName, BackgroundTaskEntryPoint);
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
           

            //done

        }





        #region methods
        //by user
        //wanneer bob accepteer, wordt door gebruiker die aanvraagd aangemaakt
        private async void MakeTrip(Trip trip, int bobID)
        {
          

            Response res = Task.FromResult<Response>(await TripRepository.PostTrip(trip)).Result;

            if (res.Success == true)
            {
               

                Trip currentTrip = Task.FromResult<Trip>(await TripRepository.GetCurrentTrip()).Result;


                Bob.All bob = Task.FromResult<Bob.All>(await BobsRepository.GetBobById(VindRitVM.SelectedBob.ID.Value)).Result;
                Libraries.Socket socketSendToBob = new Libraries.Socket()
                {
                    From = MainViewVM.USER.ID,//from user
                    To = bob.User.ID,//to bob
                    Status = true,
                    Object = currentTrip
                };


                MainViewVM.socket.Emit("trip_START:send", JsonConvert.SerializeObject(socketSendToBob)); //bob
                StartTrip(currentTrip); //user


                MakeChatroom(bobID);

                


            }
        }

        private async void StartTrip(Trip currentTrip)
        {
            var data = JsonConvert.SerializeObject(currentTrip);
            bool ok = Task.FromResult<bool>(await Localdata.save("trip.json", data)).Result;

            if (ok == true && currentTrip != null)
            {
                VindRitVM.CurrentTrip = currentTrip;
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

                Bob.All bob = Task.FromResult<Bob.All>(await BobsRepository.GetBobById(VindRitVM.SelectedBob.ID.Value)).Result;
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


        #endregion


        private object GetTripObject()
        {
            try
            {
                Bob selectedBob = VindRitVM.SelectedBob;
                Party SelectedParty = VindRitVM.SelectedParty;
                Users_Destinations SelectedDestination = VindRitVM.Filter.SelectedDestination;
                List<Friend.All> selectedFriends = VindRitVM.Filter.SelectedFriends;
                BobsType type = VindRitVM.Filter.SelectedBobsType;
                DateTime? minDate = VindRitVM.Filter.SelectedMinDate;
                int? rating = VindRitVM.Filter.SelectedRating;


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

                throw ex;
                //return null;
            }

        }
    }
}
