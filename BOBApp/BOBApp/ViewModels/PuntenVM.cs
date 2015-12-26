using BOBApp.Messages;
using BOBApp.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Libraries.Models;
using Libraries.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace BOBApp.ViewModels
{
    public class PuntenVM : ViewModelBase
    {
        //Properties
        public bool Loading { get; set; }
        public string Error { get; set; }


        public List<Point> Points { get; set; }
        public string TotalPoints { get; set; }
        public string PointsText { get; set; }

        //Constructor

        public PuntenVM()
        {
            Messenger.Default.Register<NavigateTo>(typeof(bool), ExecuteNavigatedTo);
           
            RaiseAll();
           
        }

        private void RaiseAll()
        {
            this.PointsText = "U hebt " + TotalPoints + " punten.";
            RaisePropertyChanged("Points");
            RaisePropertyChanged("TotalPoints");
            RaisePropertyChanged("PointsText");
            RaisePropertyChanged("Loading");
            RaisePropertyChanged("Error");
        }

        private void ExecuteNavigatedTo(NavigateTo obj)
        {
            if (obj.Name == "loaded")
            {
                Type view = (Type)obj.View;
                if (view == typeof(Punten))
                {
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
                GetUserPoints();
                GetPoints();
#pragma warning disable CS1998
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    this.Loading = false;
                    RaiseAll();

                });
#pragma warning restore CS1998

            });
        }

        //Methods
        private async void GetUserPoints()
        {
            this.TotalPoints = await PointRepository.GetTotalPoints();

        }
        private async void GetPoints()
        {
            this.Points = await PointRepository.GetPoints();

        }

    }
}
