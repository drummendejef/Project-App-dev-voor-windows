using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
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
    public sealed partial class Login : Page
    {
        public Login()
        {
            this.InitializeComponent();
            LocatieToestemmingVragen();
        }

        //Een (eenmalige) pop up tonen om toestemming aan de gebruiker te vragen voor zijn locatie
        private async void LocatieToestemmingVragen()
        {
            //De pop up tonen en toestemming vragen
            var accessStatus = await Geolocator.RequestAccessAsync();

            //De mogelijke antwoorden overlopen
            switch(accessStatus)
            {
                case GeolocationAccessStatus.Allowed: //De gebruiker heeft ons toegang gegeven

                    //aanmaken Geolocator
                    Geolocator geolocator = new Geolocator();

                    //Inschrijven op de StatusChanged voor updates van de permissies voor locaties.
                    geolocator.StatusChanged += OnStatusChanged;

                    //Locatie opvragen
                    Geoposition pos = await geolocator.GetGeopositionAsync();
                    Debug.WriteLine("Positie opgevraagd, lat: " + pos.Coordinate.Point.Position.Latitude + " lon: " + pos.Coordinate.Point.Position.Longitude);

                    //Locatie opslaan als gebruikerslocatie
                    (App.Current as App).UserLocation = pos;

                    break;

                case GeolocationAccessStatus.Denied: //De gebruiker heeft ons geen toegang gegeven.



                    break;

            }
        }

        //Als de status van de locatie permissies veranderd is.
        private void OnStatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            //throw new NotImplementedException();
           //  https://msdn.microsoft.com/en-us/library/windows/desktop/mt219698.aspx
        }
    }
}
