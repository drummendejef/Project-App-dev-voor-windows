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
using Windows.Networking.Connectivity;
using Newtonsoft.Json;
using Windows.Storage;
using System.IO;
using Windows.ApplicationModel.Core;
using BOBApp.Views;

namespace BOBApp.ViewModels
{
    public class LoginVM:ViewModelBase
    {
        //Properties
        #region props
        public bool Loading { get; set; }

        private Task task;
        public RelayCommand LoginCommand { get; set; }
        public RelayCommand RegisterCommand { get; set; }
        public string Email { get; set; }
        public string Pass { get; set; }
        public string Error { get; set; }
        public Boolean isLocationGiven { get; set; }
      
        public bool EnableLogin { get; set; }
        public bool Online { get; set; }

        #endregion



        //Constructor
        public LoginVM()
        {
            //Stap 1. Toestemming vragen voor locatie te krijgen
            isLocationGiven = true;//Moet op false staan, maar even op true zetten voor de commit
            LocatieToestemmingVragen();

            // Cities = new ObservableCollection<string>(new CityRepository().GetCities());
           
            this.EnableLogin = true;
            LoginCommand = new RelayCommand(Login);
            RegisterCommand = new RelayCommand(Register);
        
            //DEVELOP
            Email = "stijn.vanhulle@outlook.com";
            Pass = "test";

            Tests();
             RaiseAll();
        }

        private async void Tests()
        {
            bool serverOnline = await ServerOnline();
            bool hasInternet= IsInternet();

            if (!serverOnline)
            {
                this.Error = "Geen connectie met de serer";
            }
            if (!hasInternet)
            {
                this.Error = "Geen internet";
            }
           

            if (serverOnline && hasInternet)
            {
                this.Online = true;
            }
            else
            {
                this.Online = false;
            }
            RaiseAll();
        }

        private void RaiseAll()
        {
            RaisePropertyChanged("Email");
            RaisePropertyChanged("Online");
            RaisePropertyChanged("Pass");
            RaisePropertyChanged("Error");
            RaisePropertyChanged("Loading");
            RaisePropertyChanged("EnableLogin");
        }




        //Methods 
        public void Login()
        {
            task = Login_task(this.Email, this.Pass);
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
            this.Error = "";
            this.Loading = true;
            this.EnableLogin = false;
            RaiseAll();
            Tests();

            if (this.Online==true)
            {
                Response res = await LoginRepository.Login(email, md5.Create(pass));
                if (res.Success == true)
                {
                   

                    try
                    {
                        User user = await UserRepository.GetUser();
                        MainViewVM.USER = user;

                        string jsonUser = await Localdata.read("user.json");
                        var definitionMail = new { Email = "", Password = "" };
                        var dataUser = JsonConvert.DeserializeAnonymousType(jsonUser, definitionMail);
                        if (user.ID != MainViewVM.USER.ID)
                        {
                            Clear();
                        }


                        var json = JsonConvert.SerializeObject(new { Email = email, Password = md5.Create(pass) });
                        await Localdata.save("user.json", json);

 
                        string jsonChat = await Localdata.read("chatroom.json");
                        string jsonTrip = await Localdata.read("trip.json");
                        var definition = new { ID = 0, UserID = 0 };
                        var dataChat = JsonConvert.DeserializeAnonymousType(jsonChat, definition);
                        var dataTrip = JsonConvert.DeserializeObject<Trip>(jsonTrip);

                        if (MainViewVM.USER.IsBob == false && dataChat.UserID != MainViewVM.USER.ID)
                        {
                            Clear();

                        }
                        else if (MainViewVM.USER.IsBob == false && dataTrip.Users_ID != MainViewVM.USER.ID)
                        {
                            Clear();
                            

                        }
                        else if (MainViewVM.USER.IsBob==true && dataTrip.Bobs_ID!=MainViewVM.USER.Bobs_ID)
                        {
                            Clear();
                        }
                        else
                        {
                           // Clear();
                            VindRitVM.CurrentTrip = await TripRepository.GetCurrentTrip();

                            if (VindRitVM.CurrentTrip != null)
                            {
                                VindRitChatVM.ID = dataChat.ID;
                                if (dataTrip.ID != null && VindRitVM.CurrentTrip.ID == dataTrip.ID)
                                {
                                    Messenger.Default.Send<NavigateTo>(new NavigateTo()
                                    {
                                        Name = "trip_location:reload"
                                    });
                                }
                            }
                            else
                            {
                                Clear();
                            }
                           
                        }

                        Messenger.Default.Send<GoToPage>(new GoToPage()
                        {
                            Name = "MainView"
                        });

                    }
                    catch (Exception ex)
                    {

                        var error = ex.Message;
                    }
                    

                    this.Loading = false;
                    this.EnableLogin = true;
                    RaiseAll();

                }
                else
                {
                    this.Loading = false;
                    this.EnableLogin = true;
                    RaiseAll();

                    switch (res.Error)
                    {
                        case "Invalid Login":
                            this.Error = "Gegeven email en wachtwoord komen niet overeen of bestaan niet.";
                            break;
                        case "Server Offline":
                            this.Error = "De server is offline";
                            task = Login_task(this.Email, this.Pass);
                            break;
                        case "Connectie error":
                            this.Error = "Connectie error";
                            task = Login_task(this.Email, this.Pass);
                            break;
                        default:
                            this.Error = "Connectie error";
                           
                            break;
                    }

                    RaiseAll();



                }

                return res.Success;
            }
            else
            {

                this.Loading = false;
                this.EnableLogin = true;
                RaiseAll();

                return false;
            }
            
        }

        private async void Clear()
        {
            try
            {
                var definition = new { ID = -1, UserID = -1 };
                var data = JsonConvert.SerializeObject(definition);
                var data2 = JsonConvert.SerializeObject(new Trip() { ID = -1 });


                bool ok_chatroom = Task.FromResult<bool>(await Localdata.save("chatroom.json", data)).Result;
                bool ok_trip = Task.FromResult<bool>(await Localdata.save("trip.json", data2)).Result;
            }
                catch (Exception ex)
            {
                var error = ex.Message;
            }

        }

        private async Task<bool> ServerOnline()
        {
            bool ok = Task.FromResult<Response>(await LoginRepository.Online()).Result.Success;
            return ok;
        }
        public static bool IsInternet()
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            bool internet = connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            return internet;
        }

        //Een (eenmalige) pop up tonen om toestemming aan de gebruiker te vragen voor zijn locatie
        private async void LocatieToestemmingVragen()
        {
            try
            {
                //De pop up tonen en toestemming vragen
                var accessStatus = await Geolocator.RequestAccessAsync();

                //De mogelijke antwoorden overlopen
                switch (accessStatus)
                {
                    case GeolocationAccessStatus.Allowed: //De gebruiker heeft ons toegang gegeven

                        //Mag verdergaan en inloggen
                        this.EnableLogin = true;

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
            catch (Exception ex)
            {

                
            }
        }

        //Als de status van de locatie permissies veranderd is.
        async private void OnStatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {

            //TODO: Locatie opvragen afwerken? - Joren


        }

        //Als de locatie veranderd is, zou Async moeten gebeuren!!
        private void OnPositionChanged(Geolocator sender, PositionChangedEventArgs e)
        {     
            (App.Current as App).UserLocation = e.Position;
        }

       
    }
}
