using BOBApp.Messages;
using BOBApp.Views;
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
using Windows.ApplicationModel.Core;
using Windows.Devices.Geolocation;
using Windows.UI.Core;

namespace BOBApp.ViewModels
{
    public class RegisterVM : ViewModelBase
    {
        #region props
        //Properties
        public bool Loading { get; set; }
        public string Error { get; set; }


        public RelayCommand RegisterCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public Libraries.Models.Register NewRegister { get; set; }
        public string Password { get; set; }
        public string PasswordRepeat { get; set; }
      

        public List<Autotype> Merken {get;set;}
        public List<BobsType> TypesBob { get; set; }

        public BobsType SelectedTypeBob { get; set; }

        public Autotype SelectedAutoType { get; set; }
        public String PricePerKm { get; set; }

        #endregion

        //Constructor
        public RegisterVM()
        {
            RegisterCommand = new RelayCommand(Register);
            CancelCommand = new RelayCommand(Cancel);

            this.NewRegister = new Libraries.Models.Register();
            GetMerken();
            GetBobTypes();
            Messenger.Default.Register<NavigateTo>(typeof(bool), ExecuteNavigatedTo);
           
            RaiseAll();
        }

        private void RaiseAll()
        {
            RaisePropertyChanged("Merken");
            RaisePropertyChanged("NewRegister");
            RaisePropertyChanged("Password");
            RaisePropertyChanged("PasswordRepeat");
            RaisePropertyChanged("Merken");
            RaisePropertyChanged("SelectedAutoType");
            RaisePropertyChanged("TypesBob");
            RaisePropertyChanged("SelectedTypeBob");
            RaisePropertyChanged("Error");
            RaisePropertyChanged("PicePerKm");
            RaisePropertyChanged("Loading");
        }

        private void ExecuteNavigatedTo(NavigateTo obj)
        {
            if (obj.Name == "loaded")
            {
                Type view = (Type)obj.View;
                if (view == (typeof(Views.Register)))
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
               // GetMerken();
                //this.NewRegister = new Libraries.Models.Register();

#pragma warning disable CS1998
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    this.Loading = false;
                    RaiseAll();

                });
#pragma warning restore CS1998

            });
        }

        //Methods
        private void Cancel()
        {
            Messenger.Default.Send<GoToPage>(new GoToPage()
            {
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
           
            
            bool task = await RegisterUser(NewRegister);
      
        }
        private async Task<Boolean> RegisterUser(Libraries.Models.Register register)
        {
            if (PasswordRepeat == null)
            {
                this.Error = Libraries.Error.PasswordEmpty;
                RaiseAll();

                return false;
            }
            else if (Password == PasswordRepeat)
            {
                register.Password = Password;

                //Controleer op bob
                if (register.IsBob == true)
                {
                    register.BobsType_ID = SelectedTypeBob.ID;
                    register.AutoType_ID = SelectedAutoType.ID;
                    double price;
                    Double.TryParse(PricePerKm, out price);
                    register.PricePerKm = price;
                }
                else
                {
                    register.BobsType_ID = null;
                    register.AutoType_ID = null;
                    register.PricePerKm = null;
                    register.LicensePlate = null;
                }
               
                Response res = await UserRepository.Register(register);
                if (res.Success == true)
                {
                    //await LoginUser(register.Email, register.Password);
                    Messenger.Default.Send<GoToPage>(new GoToPage()
                    {
                        //Keer terug naar login scherm
                        Name = "Login"
                    });

                }
                else
                {
                    this.Error = res.Error;
                    RaiseAll();

                }

                return res.Success;
            }
            else
            {
                this.Error = Libraries.Error.Password;
                RaiseAll();

                return false;
            }



        }


        //login, zelfde als bij loginVM
       /* private async Task<Boolean> LoginUser(string email, string pass)
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
        }*/


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
