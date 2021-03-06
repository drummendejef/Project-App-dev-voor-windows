﻿using BOBApp.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
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
    public sealed partial class ZoekVrienden : Page
    {
       
        public ZoekVriendenVM Vm
        {
            get
            {
                return this.DataContext as ZoekVriendenVM;
            }
        }

        public ZoekVrienden()
        {
            this.InitializeComponent();

           
            if (MapZoekVriend == null)
            {
                MapZoekVriend = new MapControl();
            }
            Vm.Frame = frame;
            Vm.Map = MapZoekVriend;
        }

        private void textBoxZoekLocaties_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {

        }
    }
}
