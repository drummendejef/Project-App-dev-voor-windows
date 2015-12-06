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
        }


        private void OnMenuButtonClicked(object sender, RoutedEventArgs e)
        {
            ShellSplitView.IsPaneOpen = !ShellSplitView.IsPaneOpen;
            ((RadioButton)sender).IsChecked = false;

            CheckIsPaneOpen();

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
        bool isBob = BaseViewModelLocator.USER.IsBob;
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
