using BOBApp.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Libraries;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Notifications;
using NotificationsExtensions.Toasts;  // NotificationsExtensions.Win10
using Microsoft.QueryStringDotNET; //QueryString.NET
using Windows.UI.Core;
using Libraries.Repositories;
using Libraries.Models;

namespace BOBApp.ViewModels
{
    public class LoginVM:ViewModelBase
    {
        //Properties
        private Task task;
        public RelayCommand LoginCommand { get; set; }
        public RelayCommand RegisterCommand { get; set; }
        public RelayCommand Login_FacebookCommand { get; set; }
        public string Email { get; set; }
        public string Pass { get; set; }
        public string Error { get; set; }
        public Boolean isLocationGiven { get; set; }

        //Constructor
        public LoginVM()
        {
            //Stap 1. Toestemming vragen voor locatie te krijgen
            isLocationGiven = true;//Moet op false staan, maar even op true zetten voor de commit
            //LocatieToestemmingVragen();

            // Cities = new ObservableCollection<string>(new CityRepository().GetCities());
            RaisePropertyChanged("Email");
            RaisePropertyChanged("Pass");
            LoginCommand = new RelayCommand(Login);
            RegisterCommand = new RelayCommand(Register);
            Login_FacebookCommand = new RelayCommand(Login_Facebook);
            Email = "stijn.vanhulle@outlook.com";
            Pass = "test";

           

        }


        //Methods 
        public void Login()
        {
            task = Login_task(this.Email,this.Pass);
        }
        private void Login_Facebook()
        {
            task = Login_Facebook_task();
        }
        private void Register()
        {
            Messenger.Default.Send<GoToPage>(new GoToPage()
            {
                Name = "Register"
            });
        }

        private async Task<Boolean> Login_task(string email, string pass)
        {
            Response res = await LoginRepository.Login(email, md5.Create(pass));
            if (res.Success == true)
            {
                User user = await UserRepository.GetUser();
                MainViewVM.USER = user;
                //navigate to ritten
                Messenger.Default.Send<GoToPage>(new GoToPage()
                {
                    //Verander naar iets anders afhankelijk van wat je wil testen, zolang navigatie niet werkt ( MainView is de default)
                    //Name = "Profiel"
                    Name = "MainView"
                });
            }
            else
            {
                this.Error = res.Error;
            }

            return res.Success;
        }
        private async Task<Boolean> Login_Facebook_task()
        {
            Boolean ok = await LoginRepository.LoginFacebook();
            if (ok == true)
            {
                User user = await UserRepository.GetUser();
                MainViewVM.USER = user;
                //navigate to ritten
                Messenger.Default.Send<GoToPage>(new GoToPage()
                {
                    Name = "MainView"
                });
            }

            return ok;
        }


        //Een (eenmalige) pop up tonen om toestemming aan de gebruiker te vragen voor zijn locatie
        private async void LocatieToestemmingVragen()
        {
            //De pop up tonen en toestemming vragen
            var accessStatus = await Geolocator.RequestAccessAsync();

            //De mogelijke antwoorden overlopen
            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed: //De gebruiker heeft ons toegang gegeven

                    //Mag verdergaan en inloggen
                    isLocationGiven = true;

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
                    Debug.WriteLine("Geen locatie: Toestemming geweigerd");

                    //We gaan een Toast tonen om te zeggen dat we de locatie nodig hebben.
                    //Aanmaken tekst voor in Toast
                    string title = "Locatie Nodig";
                    string content = "We krijgen geen toegang tot uw locatie, deze staat softwarematig uitgeschakeld of u geeft ons geen toegang.";
                    
                    //De visuals van de Toast aanmaken
                    ToastVisual visual = new ToastVisual()
                    {
                        TitleText = new ToastText()
                        {
                            Text = title
                        },
                        BodyTextLine1 = new ToastText()
                        {
                            Text = content
                        },
                        AppLogoOverride = new ToastAppLogo()
                        {
                            Source = new ToastImageSource("../Assets/StoreLogo.png"),
                            Crop = ToastImageCrop.Circle
                        }
                    };

                    //De interacties met de toast aanmaken
                    ToastActionsCustom actions = new ToastActionsCustom()
                    {
                        Buttons =
                        {
                            new ToastButton("Geef Toestemming", new QueryString()
                            {
                                {"action", "openLocationServices" }
                            }.ToString())
                        }
                    };

                    //De final toast content aanmaken
                    ToastContent toastContent = new ToastContent()
                    {
                        Visual = visual,
                        Actions = actions,

                        //Argumenten wanneer de user de body van de toast aanklikt
                        Launch = new QueryString()
                        {
                            { "action", "openBobApp"}
                        }.ToString()
                    };

                    //De toast notification maken
                    var toast = new ToastNotification(toastContent.GetXml());
                    toast.ExpirationTime = DateTime.Now.AddDays(2);//Tijd totdat de notification vanzelf verdwijnt

                    //En uiteindelijk de toast tonen
                    ToastNotificationManager.CreateToastNotifier().Show(toast);

                    break;

                case GeolocationAccessStatus.Unspecified: //Er is iets vreemds misgelopen
                    Debug.WriteLine("Geen locatie: Unspecified");
                    break;

            }
        }

        //Als de status van de locatie permissies veranderd is.
        async private void OnStatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            //TODO: Locatie opvragen afwerken?
            //  https://msdn.microsoft.com/en-us/library/windows/desktop/mt219698.aspx

            /*await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

            });*/

        }

        //Als de locatie veranderd is, zou Async moeten gebeuren!!
        private void OnPositionChanged(Geolocator sender, PositionChangedEventArgs e)
        {     
            (App.Current as App).UserLocation = e.Position;
        }

    }
}
