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

namespace BOBApp.ViewModels
{
    public class LoginVM:ViewModelBase
    {
        //Properties
        private Task task;
        public RelayCommand LoginCommand { get; set; }
        public RelayCommand RegisterCommand { get; set; }
     //   public RelayCommand Login_FacebookCommand { get; set; }
        public string Email { get; set; }
        public string Pass { get; set; }
        public string Error { get; set; }
        public Boolean isLocationGiven { get; set; }
        public bool Loading { get; set; }
        public bool EnableLogin { get; set; }

        //Constructor
        public LoginVM()
        {
            //Stap 1. Toestemming vragen voor locatie te krijgen
            isLocationGiven = true;//Moet op false staan, maar even op true zetten voor de commit
            LocatieToestemmingVragen();

            // Cities = new ObservableCollection<string>(new CityRepository().GetCities());
            RaisePropertyChanged("Email");
            RaisePropertyChanged("Pass");
            RaisePropertyChanged("Error");
            RaisePropertyChanged("Loading");
            RaisePropertyChanged("EnableLogin");
            this.EnableLogin = true;
            LoginCommand = new RelayCommand(Login);
            RegisterCommand = new RelayCommand(Register);
          //  Login_FacebookCommand = new RelayCommand(Login_Facebook);
            Email = "stijn.vanhulle@outlook.com";
            Pass = "test";

           

        }

       


        //Methods 
        public void Login()
        {
            task = Login_task(this.Email,this.Pass);
        }
    /*    private void Login_Facebook()
        {
            task = Login_Facebook_task();
        }*/
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
            RaisePropertyChanged("Error");
            this.Loading = true;
            RaisePropertyChanged("Loading");
            this.EnableLogin = false;
            RaisePropertyChanged("EnableLogin");


            bool ok= serverOnline();
            bool internet = IsInternet();
            if (ok == true && internet==true)
            {
                Response res = await LoginRepository.Login(email, md5.Create(pass));
                if (res.Success == true)
                {
                    var json = JsonConvert.SerializeObject(new { Email =email, Password= md5.Create(pass) });
                    await saveStringToLocalFile("user.json", json);


                    User user = await UserRepository.GetUser();
                    MainViewVM.USER = user;
                    Messenger.Default.Send<GoToPage>(new GoToPage()
                    {
                        Name = "MainView"
                    });

                    this.Loading = false;
                    RaisePropertyChanged("Loading");

                    this.EnableLogin = true;
                    RaisePropertyChanged("EnableLogin");

                }
                else
                {

                    switch (res.Error)
                    {
                        case "Invalid Login":
                            this.Loading = false;
                            RaisePropertyChanged("Loading");

                            this.EnableLogin = true;
                            RaisePropertyChanged("EnableLogin");

                            this.Error = "Gegeven email en wachtwoord komen niet overeen of bestaan niet.";
                            RaisePropertyChanged("Error");

                            break;
                        case "Server offline":
                            this.Loading = false;
                            RaisePropertyChanged("Loading");

                            this.EnableLogin = true;
                            RaisePropertyChanged("EnableLogin");

                            this.Error = "De server is offline";
                            RaisePropertyChanged("Error");
                            break;
                        case "Connectie error":
                            this.Loading = false;
                            RaisePropertyChanged("Loading");

                            this.EnableLogin = true;
                            RaisePropertyChanged("EnableLogin");

                            this.Error = "Connectie error";
                            RaisePropertyChanged("Error");
                            break;
                        default:
                            this.Error = "Connectie error";
                            RaisePropertyChanged("Error");

                            task = Login_task(this.Email, this.Pass);
                            break;
                    }

                  

                }

                return res.Success;
            }
            else
            {
                this.Error = "Server is offline, even geduld aub";
                RaisePropertyChanged("Error");
                return false;
            }
            
        }

        private bool serverOnline()
        {
            return true;
        }
        public static bool IsInternet()
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            bool internet = connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            return internet;
        }

        /*      private async Task<Boolean> Login_Facebook_task()
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
     } */


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

        async Task saveStringToLocalFile(string filename, string content)
        {
            // saves the string 'content' to a file 'filename' in the app's local storage folder
            byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(content.ToCharArray());

            // create a file with the given filename in the local folder; replace any existing file with the same name
            StorageFile file = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

            // write the char array created from the content string into the file
            using (var stream = await file.OpenStreamForWriteAsync())
            {
                stream.Write(fileBytes, 0, fileBytes.Length);
            }
        }
    }
}
