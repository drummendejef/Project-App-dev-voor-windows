using BOBApp.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Libraries.Models;
using Libraries.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace BOBApp.ViewModels
{
    public class MainViewVM:ViewModelBase
    {
        //Properties
        //public static
        public static Frame MainFrame;
        public static User USER;
        public static ChatRoom ChatRoom;
      

        private Task logoffTask;
        public User User { get; set; }
        public RelayCommand LogOffCommand { get; set; }
        public double Points { get; set; }
        private IBackgroundTaskRegistration regTask = null;

        //Constructor
        public MainViewVM()
        {
            User = MainViewVM.USER;
            GetPoints();

            SetupBackgroundTask();
            LogOffCommand = new RelayCommand(LogOff);
            RaisePropertyChanged("User");
        }





        //Methods
        private void LogOff()
        {
            logoffTask = LogOffUser();
        }
        private async Task<Boolean> LogOffUser()
        {
            Response res = await LoginRepository.LogOff();
            if (res.Success == true)
            {
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
