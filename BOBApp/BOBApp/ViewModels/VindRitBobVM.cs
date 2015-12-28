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
    public class VindRitBobVM : ViewModelBase
    {
        #region props
        #region static

        public static int Request { get; set; }
        public List<Bob> BobList { get; set; }


        #endregion

        public bool Loading { get; set; }
        public string Error { get; set; }

        public Visibility VisibleModal { get; set; }
        public Frame Frame { get; set; }
       


        #region gets



        #endregion

        //others

        public RelayCommand ShowModalCommand { get; set; }
        public RelayCommand CloseModalCommand { get; set; }

        public string BobRequests { get; set; }
 
       


      


        #endregion

        //Constructor
        public VindRitBobVM()
        {

           
            CloseModalCommand = new RelayCommand(CloseModal);
            ShowModalCommand = new RelayCommand(ShowModal);

            Messenger.Default.Register<NavigateTo>(typeof(bool), ExecuteNavigatedTo);

            this.Loading = false;
          
            this.BobRequests = "Momenteel " + VindRitBobVM.Request.ToString() + " aanvragen";


          
            RaiseAll();
        }

        private void ExecuteNavigatedTo(NavigateTo obj)
        {
            //throw new NotImplementedException();
        }

        private void RaiseAll()
        {
            RaisePropertyChanged("Loading");
          
            RaisePropertyChanged("VisibleModal");
         
            RaisePropertyChanged("Frame");
          
            RaisePropertyChanged("BobRequests");

      

           
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
                  
                    VisibleModal = Visibility.Collapsed;
                 
                  
                    this.BobRequests = "Momenteel " + VindRitBobVM.Request.ToString() + " aanvragen";
   
                    RaiseAll();

                });
#pragma warning restore CS1998

            });
        }



        
        

        private void CloseModal()
        {
            //Loaded();

            VisibleModal = Visibility.Collapsed;
            RaiseAll();

           
           
        }
        private void ShowModal()
        {
            this.Frame = new Frame();
            this.Frame.Navigate(typeof(VindRitFilter));
            this.Frame.Navigated += Frame_Navigated;

            VisibleModal = Visibility.Visible;
            RaiseAll();

        }

        private void Frame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                bool reload = (bool)e.Parameter;

                Messenger.Default.Send<NavigateTo>(new NavigateTo()
                {
                    Reload = reload,
                    View = e.SourcePageType
                });

                if (reload == false)
                {
                    //e.Cancel = true;

                }
            }
        }
    }
}
