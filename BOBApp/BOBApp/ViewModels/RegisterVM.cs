using BOBApp.Messages;
using BOBApp.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Libraries;
using Libraries.Models;
using Libraries.Repositories;
using Newtonsoft.Json;
using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Geolocation;
using Windows.Networking.Connectivity;
using Windows.UI.Core;

namespace BOBApp.ViewModels
{
    public class RegisterVM : ViewModelBase
    {
        #region props
        //Properties
        public bool Loading { get; set; }
        public string Error { get; set; }
        public bool EnableLogin { get; set; }
        public bool Online { get; set; }
        public Task task { get; set; }

        public RelayCommand RegisterCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public Libraries.Models.Register NewRegister { get; set; }
        public string Password { get; set; }
        public string PasswordRepeat { get; set; }


        public List<Autotype> Merken { get; set; }
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

        private async void RaiseAll()
        {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
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
            });
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
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
                    task = Login_task(register.Email, register.Password);
                    /*Messenger.Default.Send<GoToPage>(new GoToPage()
                    {
                        //Keer terug naar login scherm
                        Name = "Login"
                    }); */

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

        private async void Tests()
        {
            bool serverOnline = await ServerOnline();
            bool hasInternet = IsInternet();

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

        //login, zelfde als bij loginVM
        private async Task<Boolean> Login_task(string email, string pass)
        {
            this.Error = "";
            this.Loading = true;
            this.EnableLogin = false;
            RaiseAll();
            Tests();

            if (this.Online == true)
            {
                Response res = await LoginRepository.Login(email, pass);
                if (res.Success == true)
                {


                    try
                    {
                        User user = await UserRepository.GetUser();
                        MainViewVM.USER = user;

                        string jsonUser = await Localdata.read("user.json");
                        var definitionMail = new { Email = "", Password = "" };
                        if (jsonUser != null)
                        {
                            var dataUser = JsonConvert.DeserializeAnonymousType(jsonUser, definitionMail);
                            if (user.ID != MainViewVM.USER.ID)
                            {
                                Clear();
                            }
                        }


                        var json = JsonConvert.SerializeObject(new { Email = email, Password = pass });
                        await Localdata.save("user.json", json);


                        string jsonChat = await Localdata.read("chatroom.json");
                        string jsonTrip = await Localdata.read("trip.json");
                        var definition = new { ID = 0, UserID = 0 };

                        if (jsonChat != null && jsonTrip != null)
                        {
                            var dataChat = JsonConvert.DeserializeAnonymousType(jsonChat, definition);
                            var dataTrip = JsonConvert.DeserializeObject<Trip>(jsonTrip);
                            if (dataChat == null | dataTrip == null)
                            {
                                Clear();
                            }
                            else if (MainViewVM.USER.IsBob == false && dataChat.UserID != MainViewVM.USER.ID)
                            {
                                Clear();

                            }
                            else if (MainViewVM.USER.IsBob == false && dataTrip.Users_ID != MainViewVM.USER.ID)
                            {
                                Clear();


                            }
                            else if (MainViewVM.USER.IsBob == true && dataTrip.Bobs_ID != MainViewVM.USER.Bobs_ID)
                            {
                                Clear();
                            }
                            else
                            {
                                // Clear();
                                MainViewVM.CurrentTrip = await TripRepository.GetCurrentTrip();

                                if (MainViewVM.CurrentTrip != null)
                                {
                                    VindRitChatVM.ID = dataChat.ID;
                                    if (dataTrip.ID != null && MainViewVM.CurrentTrip.ID == dataTrip.ID)
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
                        }
                        MainViewVM.socket = IO.Socket(URL.SOCKET);
                        MainViewVM.socket.Connect();

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
                            task = Login_task(NewRegister.Email, NewRegister.Password);
                            break;
                        case "Connectie error":
                            this.Error = "Connectie error";
                            task = Login_task(NewRegister.Email, NewRegister.Password);
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
