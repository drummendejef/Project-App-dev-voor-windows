﻿using BOBApp.Messages;
using BOBApp.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using Libraries;
using Libraries.Models;
using Libraries.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BOBApp.Views
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainView : Page
    {
        public MainView()
        {
            this.InitializeComponent();
            this.ShellSplitView.Content = frame;

            Frame rootFrame = (Frame)ShellSplitView.Content;
            rootFrame.Navigated += OnNavigated;
            MainViewVM.MainFrame = rootFrame;


            if (ShellSplitView.Content != null)
            {
                if (MainViewVM.USER.IsBob==true)
                {
                    ShellSplitView.IsPaneOpen = false;
                    if (ShellSplitView.Content != null)
                        ((Frame)ShellSplitView.Content).Navigate(typeof(VindRitBob),true);
                    CheckIsPaneOpen();
                }
                else
                {
                    ShellSplitView.IsPaneOpen = false;
                    if (ShellSplitView.Content != null)
                        ((Frame)ShellSplitView.Content).Navigate(typeof(VindRit),true);
                    CheckIsPaneOpen();
                }
               
            }
               

           
           

            // Register a handler for BackRequested events and set the
            // visibility of the Back button

            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                rootFrame.CanGoBack ?
                AppViewBackButtonVisibility.Visible :
                AppViewBackButtonVisibility.Collapsed;


            IsBob();
        }

        private void OnMenuButtonClicked(object sender, RoutedEventArgs e)
        {
            ShellSplitView.IsPaneOpen = !ShellSplitView.IsPaneOpen;
            ((RadioButton)sender).IsChecked = false;

            CheckIsPaneOpen();

        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            // Each time a navigation event occurs, update the Back button's visibility
            bool ok = ((Frame)sender).CanGoBack;
            if (ok == true)
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
            else
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }


        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame rootFrame = (Frame)ShellSplitView.Content;

            if (rootFrame.CanGoBack)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        private void OnVindEenRitChecked(object sender, RoutedEventArgs e)
        {
            ShellSplitView.IsPaneOpen = false;
            if (ShellSplitView.Content != null)
                ((Frame)ShellSplitView.Content).Navigate(typeof(VindRit));
            CheckIsPaneOpen();
        }


        private void OnVindVriendenChecked(object sender, RoutedEventArgs e)
        {
            ShellSplitView.IsPaneOpen = false;
            if (ShellSplitView.Content != null)
                ((Frame)ShellSplitView.Content).Navigate(typeof(ZoekVrienden));
            CheckIsPaneOpen();
        }

        private void OnMijnRittenChecked(object sender, RoutedEventArgs e)
        {
            ShellSplitView.IsPaneOpen = false;
            if (ShellSplitView.Content != null)
                ((Frame)ShellSplitView.Content).Navigate(typeof(MijnRitten));
            CheckIsPaneOpen();
        }

        private void OnFeestjesInDeBuurtChecked(object sender, RoutedEventArgs e)
        {
            ShellSplitView.IsPaneOpen = false;
            if (ShellSplitView.Content != null)
                ((Frame)ShellSplitView.Content).Navigate(typeof(FeestjesOverzicht));
            CheckIsPaneOpen();
        }
        private void OnBestemmingenChecked(object sender, RoutedEventArgs e)
        {
            ShellSplitView.IsPaneOpen = false;
            if (ShellSplitView.Content != null)
                ((Frame)ShellSplitView.Content).Navigate(typeof(Bestemmingen));
            CheckIsPaneOpen();
        }
        private void OnProfielChecked(object sender, RoutedEventArgs e)
        {
            ShellSplitView.IsPaneOpen = false;
            if (ShellSplitView.Content != null)
                ((Frame)ShellSplitView.Content).Navigate(typeof(Profiel));
            CheckIsPaneOpen();
        }


        private void OnBiedJeAanChecked(object sender, RoutedEventArgs e)
        {
            ShellSplitView.IsPaneOpen = false;
            if (ShellSplitView.Content != null)
                ((Frame)ShellSplitView.Content).Navigate(typeof(VindRitBob));
            CheckIsPaneOpen();
        }

       
        private void OnChangeChecked(object sender, RoutedEventArgs e)
        {
            ShellSplitView.IsPaneOpen = false;
            if (ShellSplitView.Content != null)
            {
                changeToBob();
            }
            CheckIsPaneOpen();


        }
        bool isBob;
        private async void changeToBob()
        {
            isBob = MainViewVM.USER.IsBob.Value;

            if (isBob == true)
            {
                isBob = false;
            }
            else
            {
                isBob = true;
            }


            if (MainViewVM.USER.CanBeBob == false && isBob){
                Messenger.Default.Send<Dialog>(new Dialog()
                {
                    Message = "Gelieve bob gegevens in te vullen",
                    Ok = null,
                    Nok = null
                });


                ShellSplitView.IsPaneOpen = false;
                if (ShellSplitView.Content != null)
                    ((Frame)ShellSplitView.Content).Navigate(typeof(Profiel));
                CheckIsPaneOpen();



            }
            else
            {
                Response ok = await UserRepository.ChanteToBob(isBob);
                if (ok.Success == true)
                {
                   // Location location = await LocationService.GetCurrent();
                    //Response ok2 = await UserRepository.PostLocation(location);

                    MainViewVM.USER= await UserRepository.GetUser();
                    IsBob();
                }
            }


           


        }

        private void IsBob()
        {
            isBob = MainViewVM.USER.IsBob.Value;
            if (isBob == true)
            {
                //bob
                bobBiedJeAan.Visibility = Visibility.Visible;
                bobVindEenRit.Visibility = Visibility.Collapsed;
                Change.Content = "Type: BOB";
            }
            else
            {
                //user
                bobBiedJeAan.Visibility = Visibility.Collapsed;
                bobVindEenRit.Visibility = Visibility.Visible;
                Change.Content = "Type: USER";
            }
        }


        //functions

        private void CheckIsPaneOpen()
        {
            if (ShellSplitView.IsPaneOpen == true)
            {
                user.Visibility = Visibility.Visible;
            }
            else
            {
                user.Visibility = Visibility.Collapsed;
            }
        }


        private void frame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.SourcePageType == typeof(VindRit))
            {
                bobVindEenRit.IsChecked = true;
                
            }else if (e.SourcePageType == typeof(VindRitBob))
            {
                bobBiedJeAan.IsChecked = true;

            }
            else
            {
                bobVindEenRit.IsChecked =false;
                bobBiedJeAan.IsChecked = false;

            }



            if (e.Parameter != null)
            {
                bool reload = (bool)e.Parameter;

                Messenger.Default.Send<NavigateTo>(new NavigateTo()
                {
                    Reload = reload,
                    View = ((Frame)ShellSplitView.Content).CurrentSourcePageType
                });

                if (reload == false)
                {
                    //e.Cancel = true;
                    
                }
            }
        }

        private void ShellSplitView_PaneClosing(SplitView sender, SplitViewPaneClosingEventArgs args)
        {
            user.Visibility = Visibility.Collapsed;
        }

        private void frame_Navigated(object sender, NavigationEventArgs e)
        {
            
            Messenger.Default.Send<NavigateTo>(new NavigateTo()
            {
                Name="loaded",
                View= ((Frame)ShellSplitView.Content).CurrentSourcePageType
            });
        }

        private void frame_NavigationStopped(object sender, NavigationEventArgs e)
        {
          
        }

        private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ShellSplitView.IsPaneOpen = false;
            if (ShellSplitView.Content != null)
                ((Frame)ShellSplitView.Content).Navigate(typeof(Punten));
            CheckIsPaneOpen();
        }
    }
}
