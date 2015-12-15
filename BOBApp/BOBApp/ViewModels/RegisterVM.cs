﻿using BOBApp.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Libraries;
using Libraries.Models;
using Libraries.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace BOBApp.ViewModels
{
    public class RegisterVM : ViewModelBase
    {
        //Properties
        private Boolean RegisterTask;
        private Task LoginTask;
        public RelayCommand RegisterCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public Register NewRegister { get; set; }
        public string Password { get; set; }
        public string PasswordRepeat { get; set; }
        public string Error { get; set; }

        public ObservableCollection<Autotype> Merken {get;set;}

        public Autotype SelectedAutoType { get; set; }
        public String PricePerKm { get; set; }



        //Constructor
        public RegisterVM()
        {
            this.NewRegister = new Register();

            //Uit commentaar halen als server online is
            //GetMerken();

            RaisePropertyChanged("Merken");
            RaisePropertyChanged("NewRegister");
            RaisePropertyChanged("Password");
            RaisePropertyChanged("PasswordRepeat");
            RaisePropertyChanged("Error");
            RegisterCommand = new RelayCommand(Register);
            CancelCommand = new RelayCommand(Cancel);
        }



        //Methods
        private void Cancel()
        {
            Messenger.Default.Send<GoToPage>(new GoToPage()
            {
                //Keer terug naar login scherm
                Name = "Login"
            });
        }
        public async void Register()
        {
            //TODO: Databinding IsBob met toggleswitch
            if (NewRegister.IsBob == null)
            {
                NewRegister.IsBob = false;
            }
            double price;
            Double.TryParse(PricePerKm, out price);
            NewRegister.PricePerKm = price;
            
            RegisterTask = await RegisterUser(NewRegister);
      
        }
        private async Task<Boolean> RegisterUser(Register register)
        {
            if (PasswordRepeat == null)
            {
                this.Error = Libraries.Error.PasswordEmpty;
                RaisePropertyChanged("Error");

                return false;
            }
            else if (Password == PasswordRepeat)
            {
                register.Password = Password;
                if (SelectedAutoType != null)
                {
                    register.AutoType_ID = SelectedAutoType.ID;
                }
                Response res = await UserRepository.Register(register);
                if (res.Success == true)
                {
                    await LoginUser(register.Email, register.Password);
                }
                else
                {
                    this.Error = res.Error;
                    RaisePropertyChanged("Error");

                }

                return res.Success;
            }
            else
            {
                this.Error = Libraries.Error.Password;
                RaisePropertyChanged("Error");

                return false;
            }



        }


        //login, zelfde als bij loginVM
        private async Task<Boolean> LoginUser(string email, string pass)
        {
            Response res = await LoginRepository.Login(email, pass);
            if (res.Success == true)
            {
                Geolocator geolocator = new Geolocator();
                Geoposition pos = await geolocator.GetGeopositionAsync();
                Location current = new Location() { Latitude = pos.Coordinate.Point.Position.Latitude, Longitude = pos.Coordinate.Point.Position.Longitude };
                Location.Current = current;

                User user = await UserRepository.GetUser();
                MainViewVM.USER = user;
                Messenger.Default.Send<GoToPage>(new GoToPage()
                {
                    Name = "MainView"
                });
            }
            else
            {
                if (res.Error == "Invalid Login")
                {
                    this.Error = "Gegeven email en wachtwoord komen niet overeen of bestaan niet.";
                    RaisePropertyChanged("Error");
                }
                if (res.Error == "Server offline")
                {
                    this.Error = "De server is offline";
                    RaisePropertyChanged("Error");
                }

            }

            return res.Success;
        }


        private async void GetMerken()
        {

            List<Autotype> merkenLijst = await AutotypeRepository.GetAutotypes();
            this.Merken = new ObservableCollection<Autotype>();
            foreach(Autotype merk in merkenLijst)
            {
                this.Merken.Add(merk);
            }
        }
    }
}
