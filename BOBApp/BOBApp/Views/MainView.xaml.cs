using BOBApp.ViewModels;
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

            if (ShellSplitView.Content != null)
                ((Frame)ShellSplitView.Content).Navigate(typeof(VindRit));

            Frame rootFrame = (Frame)ShellSplitView.Content;
            MainViewVM.MainFrame = rootFrame;
            rootFrame.Navigated += OnNavigated;

            // Register a handler for BackRequested events and set the
            // visibility of the Back button

            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                rootFrame.CanGoBack ?
                AppViewBackButtonVisibility.Visible :
                AppViewBackButtonVisibility.Collapsed;

            
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
                ((Frame)ShellSplitView.Content).Navigate(typeof(VindRit));
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
        bool isBob = MainViewVM.USER.IsBob;
        private async void changeToBob()
        {
           
            Response ok = await UserRepository.ChanteToBob(isBob);
            if (ok.Success == true)
            {
                if (isBob == true)
                {
                    //bob
                    bobBiedJeAan.Visibility = Visibility.Visible;
                    isBob = false;
                    Change.Content = "BOB";
                }
                else
                {
                    //user
                    bobBiedJeAan.Visibility = Visibility.Collapsed;
                    isBob = true;
                    Change.Content = "USER";
                }
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
    }
}
