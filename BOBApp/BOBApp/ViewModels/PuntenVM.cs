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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BOBApp.ViewModels
{
    public class PuntenVM : ViewModelBase
    {
        //Properties
        public bool Loading { get; set; }
        public string Error { get; set; }


        public List<Point> Points { get; set; }
        public Point SelectedPoint { get; set; }
        public string TotalPoints { get; set; }
        public string PointsText { get; set; }

        //Constructor

        public PuntenVM()
        {
            Messenger.Default.Register<NavigateTo>(typeof(bool), ExecuteNavigatedTo);
           
            RaiseAll();
           
        }

        private async void RaiseAll()
        {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                this.PointsText = "U hebt " + TotalPoints + " punten.";
                RaisePropertyChanged("Points");
                RaisePropertyChanged("TotalPoints");
                RaisePropertyChanged("SelectedPoint");
                RaisePropertyChanged("PointsText");
                RaisePropertyChanged("Loading");
                RaisePropertyChanged("Error");
            });
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
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

        //bind events
        public async void Changed(object sender, SelectionChangedEventArgs e)
        {
            ListView item = (ListView)sender;
            if (item.SelectedIndex == -1)
            {
                return;
            }


            var dialog = new ContentDialog()
            {
                Title = "",
            };

            // Setup Content
            var panel = new StackPanel();

            panel.Children.Add(new TextBlock
            {
                Text = "Volgende bestemming wilt u wijzigen: ",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 15)
            });

            var cb = new TextBox
            {
                TextWrapping = TextWrapping.Wrap,
                HorizontalContentAlignment = HorizontalAlignment.Stretch
            };



            panel.Children.Add(cb);
            dialog.Content = panel;

            // Add Buttons
            dialog.PrimaryButtonText = "Ok";
            dialog.PrimaryButtonClick += async delegate
            {
                string text = cb.Text;
              
                Loaded();
            };

            dialog.SecondaryButtonText = "Cancel";
            dialog.SecondaryButtonClick += delegate
            {

            };

            // Show Dialog
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.None)
            {

            }
            item.SelectedIndex = -1;


        }

    }
}
