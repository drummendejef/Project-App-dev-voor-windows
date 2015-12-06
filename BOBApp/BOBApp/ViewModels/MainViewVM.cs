﻿using BOBApp.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Libraries.Models;
using Libraries.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace BOBApp.ViewModels
{
    public class MainViewVM:ViewModelBase
    {
        //Properties

        public User User { get; set; }
        public double Points { get; set; }
        private IBackgroundTaskRegistration regTask = null;

        //Constructor
        public MainViewVM()
        {
            User = BaseViewModelLocator.USER;
            GetPoints();

            SetupBackgroundTask();

            RaisePropertyChanged("User");
        }

       

        //Methods
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

                    var trigger = new TimeTrigger(2, false);// om de 2 minuten
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
            //string json = await readStringFromLocalFile("parking.json");
            // ParkingResult[] results = JsonConvert.DeserializeObject<ParkingResult[]>(json);

            Debug.WriteLine("Niewue locatie gestuurd");
           

            //done

        }


    }
}
