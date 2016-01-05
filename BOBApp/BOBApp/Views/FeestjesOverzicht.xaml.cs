using BOBApp.ViewModels;
using Libraries;
using Libraries.Models;
using Libraries.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using Windows.Storage.Streams;
using Windows.UI;
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
    public sealed partial class FeestjesOverzicht : Page
    {
      
        public FeestjesOverzichtVM Vm
        {
            get
            {
                return this.DataContext as FeestjesOverzichtVM;
            }
        }

        public FeestjesOverzicht()
        {
            this.InitializeComponent();
            if (MapFeestOverzicht == null)
            {
                MapFeestOverzicht = new MapControl();
            }
            Vm.Map = MapFeestOverzicht;

           
        }

    }
}
