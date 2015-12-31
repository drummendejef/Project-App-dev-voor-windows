﻿using BOBApp.Messages;
using BOBApp.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Libraries.Models;
using Libraries.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace BOBApp.ViewModels
{
    public class ProfielVM : ViewModelBase {

        #region props
        //Properties
        public bool Loading { get; set; }
        public string Error { get; set; }


        public RelayCommand AanpasCommand { get; set; }
        public RelayCommand WachtwoordCommand { get; set; }

        public User.Profile User { get; set; }
        public String Password { get; set; }
        public String PasswordRepeat { get; set; }
        public List<Autotype> Merken { get; set; }

        public List<BobsType> TypesBob { get; set; }

        public User.PutUser EditUser { get; set; }

        #endregion
        //Constructor
        public ProfielVM()
        {

            //GetUserDetails() werkt
            GetUserDetails();
            GetMerken();
            GetBobTypes();



            //Testen met statische data ( momenteel nog laten staan, in geval dit nog handig kan zijn voor iets)
            //User = new Register{ Lastname = "Van Lancker", Firstname = "Kevin", Email = "Test@test.be", Cellphone = "0494616943", LicensePlate = "1-43AE42", Password = "123" };
            AanpasCommand = new RelayCommand(Aanpassen);
            WachtwoordCommand = new RelayCommand(Wachtwoord);


            RaiseAll();

        }

        private void ExecuteNavigatedTo(NavigateTo obj)
        {
            if (obj.Name == "loaded")
            {
                Type view = (Type)obj.View;
                if (view == (typeof(Profiel)))
                {
                    Loaded();
                }
            }
        }

        private async void Loaded()
        {
            this.Loading = true;
            RaisePropertyChanged("Loading");

            await Task.Run(async () =>
            {
                // running in background
                GetUserDetails();
                GetMerken();
#pragma warning disable CS1998
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    this.Loading = false;
                    RaiseAll();

                });
#pragma warning restore CS1998

            });
        }

        private void RaiseAll()
        {
            RaisePropertyChanged("SearchLocation");
            RaisePropertyChanged("Destination");
            RaisePropertyChanged("Destinations");
            RaisePropertyChanged("MapCenter");
            RaisePropertyChanged("NewDestination");
            RaisePropertyChanged("Merken");
            RaisePropertyChanged("TypesBob");
            RaisePropertyChanged("Loading");
            RaisePropertyChanged("Error");
        }

        //Methods

        public void Wachtwoord()
        {
            Frame rootFrame = MainViewVM.MainFrame as Frame;
            rootFrame.Navigate(typeof(VeranderWachtwoord), true);
        }
        public async void Aanpassen()
        {
            EditUser = new User.PutUser();

            
                EditUser.Bobs_ID = User.Bob.ID;
                EditUser.Users_ID = User.User.ID;
                EditUser.Firstname = User.User.Firstname;
                EditUser.Lastname = User.User.Lastname;
                EditUser.Email = User.User.Email;
                EditUser.Cellphone = User.User.Cellphone;
                EditUser.IsBob = User.User.IsBob.Value;
                EditUser.PricePerKm = User.Bob.PricePerKm;
                EditUser.BobsType_ID = User.Bob.BobsType_ID;
                EditUser.LicensePlate = User.Bob.LicensePlate;
                EditUser.AutoType_ID = User.Autotype.ID;
            
            //Updaten naar DB
            Response r = await UserRepository.EditUser(EditUser);
            if (r.Success)
            {
                var dialog = new MessageDialog("Uw gegevens zijn opgeslaan.");
                await dialog.ShowAsync();
            }
            else
            {
                var dialog = new MessageDialog("Er is een probleem met het opslaan van uw gegevens. Probeer het nog eens opnieuw.");
                await dialog.ShowAsync();
            }
        }


        
        private async void GetUserDetails()
        {
            this.User = await UserRepository.GetProfile();
            if (User.Bob == null)
            {
                User.Bob = new Bob();
            }
            if (User.Autotype == null)
            {
                User.Autotype = new Autotype();
            }

        }

        private async void GetMerken()
        {
            this.Merken = await AutotypeRepository.GetAutotypes();
            RaiseAll();
        }

        private async void GetBobTypes()
        {
            this.TypesBob = await BobsRepository.GetTypes();
            RaiseAll();
        }

    }
}
