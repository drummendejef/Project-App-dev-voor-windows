using BOBApp.Messages;
using BOBApp.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Libraries;
using Libraries.Models;
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
        public object Console { get; private set; }

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

            MainViewVM.socket.On("bob_ACCEPT", (msg) =>
            {
                Libraries.Socket _socket = JsonConvert.DeserializeObject<Libraries.Socket>((string)msg);
                //if (_socket.Status == true && _socket.To==MainViewVM.USER.ID)
                if (_socket.Status == true)
                {
                    MainViewVM.LatestSocket = _socket;
                    VindRitVM.Request = VindRitVM.Request + 1;
                    Task task = NavigateAccept();
                }
               
            });

            MainViewVM.socket.On("disconnect", (msg) =>
            {
                LogOff();
            });

            MainViewVM.socket.On("trip_DONE", (msg) =>
            {
                Libraries.Socket _socket = JsonConvert.DeserializeObject<Libraries.Socket>((string)msg);
                //if (_socket.Status == true && _socket.To==MainViewVM.USER.ID)
                if (_socket.Status == true)
                {
                    //rating van bob voor gebruiker

                    //add points to user
                    //add rating to bobs-parties
                }

            });
        }









        //Methods
     

        private async Task NavigateAccept()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
             async () =>
             {
                 VindRitVM.BobAccepted = true;


                 bool ok = Task.FromResult<bool>(await AcceptUser("U choose Stijn as your user?")).Result;

                 if (ok == true)
                 {
                     Frame rootFrame = MainViewVM.MainFrame as Frame;
                     rootFrame.Navigate(typeof(VindRitBob), true);
                 }
                
             }
             );
        }

        private async Task<bool> AcceptUser(string text)
        {
            var dialog = new MessageDialog(text);

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

        private void LogOff()
        {
            logoffTask = LogOffUser();
        }
        private async Task<Boolean> LogOffUser()
        {
            Response res = await LoginRepository.LogOff();
            if (res.Success == true)
            {
                MainViewVM.socket.Disconnect();

                Messenger.Default.Send<GoToPage>(new GoToPage()
                {
                    Name = "Login"
                });
            }
        
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

       


    }
}
