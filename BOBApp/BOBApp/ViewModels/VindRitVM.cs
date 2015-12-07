using BOBApp.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace BOBApp.ViewModels
{
    public class VindRitVM : ViewModelBase
    {
        //Properties

        public RelayCommand GoChatCommand { get; set; }
        public RelayCommand GoFilterCommand { get; set; }

        //Constructor
        public VindRitVM()
        {
            GoChatCommand = new RelayCommand(GoChat);
            GoFilterCommand = new RelayCommand(GoFilter);
        }



        //Methods
        private void GoChat()
        {
            Frame rootFrame = MainViewVM.MainFrame as Frame;

            rootFrame.Navigate(typeof(VindRitChat));
        }
        private void GoFilter()
        {
            Frame rootFrame = MainViewVM.MainFrame as Frame;

            rootFrame.Navigate(typeof(VindRitFilter));
        }
    }
}
