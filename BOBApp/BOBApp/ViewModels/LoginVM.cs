using BOBApp.Messages;
using BOBApp.Models;
using BOBApp.Repositories;
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

namespace BOBApp.ViewModels
{
    public class LoginVM:ViewModelBase
    {
        //Properties
        private Task loginTask;
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
            LocatieToestemmingVragen();

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
            loginTask = LoginUser(this.Email,this.Pass);

            //***********************************************
            //OM HET INLOGGEN TE OMZEILEN!!!! WEGDOEN ALS DAT OPGELOST IS
            //TODO: Onderstaande regel verwijderen als de server niet meer crasht :P
            //***********************************************
            //Messenger.Default.Send<GoToPage>(new GoToPage() { Name = "MainView" });
        }
        private void Login_Facebook()
        {
            loginTask = LoginFacebook();
        }
        private void Register()
        {
            Messenger.Default.Send<GoToPage>(new GoToPage()
            {
                Name = "Register"
            });
        }

        private async Task<Boolean> LoginUser(string email, string pass)
        {
            Response res = await LoginRepository.Login(email, md5.Create(pass));
            if (res.Success == true)
            {
                Login user = await LoginRepository.GetUser();
                BaseViewModelLocator.USER = user;
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
        private async Task<Boolean> LoginFacebook()
        {
            Boolean ok = await LoginRepository.LoginFacebook();
            if (ok == true)
            {
                Login user = await LoginRepository.GetUser();
                BaseViewModelLocator.USER = user;
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

                    //Als de app geen toegang krijgt tot de locatie, gaat hij vastlopen als hij een kaart gaat willen tonen op dat hij geen locatie voor de eigenaar heeft.
                    //We kunnen dat oplossen door de app af te sluiten, of gewoon door niet op die locatie te focussen
                    Debug.WriteLine("App sluit af wegens geen locatie toestemming");//Nog mooier maken door echt een bericht te geven op de app.
                    App.Current.Exit();

                    break;

                case GeolocationAccessStatus.Unspecified: //Er is iets vreemds misgelopen
                    Debug.WriteLine("Geen locatie: Unspecified");
                    break;

            }
        }

        //Als de status van de locatie permissies veranderd is.
        private void OnStatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            //TODO: Locatie opvragen afwerken?
            //throw new NotImplementedException();
            //  https://msdn.microsoft.com/en-us/library/windows/desktop/mt219698.aspx
        }




    }
}
