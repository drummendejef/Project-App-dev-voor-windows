using BOBApp.Messages;
using BOBApp.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Libraries.Models;
using Libraries.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace BOBApp.ViewModels
{
    public class VeranderWachtwoordVM : ViewModelBase
    {

        public bool Loading { get; set; }
        public string Error { get; set; }



        public RelayCommand WijzigCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public User.PutUser EditUser { get; set; }
        public User.Profile User { get; set; }

        public string Password { get; set; }
        public string PasswordRepeat { get; set; }
      




        public VeranderWachtwoordVM()
        {
           
            WijzigCommand = new RelayCommand(Wijzig);
            CancelCommand = new RelayCommand(Cancel);

            Messenger.Default.Register<NavigateTo>(typeof(bool), ExecuteNavigatedTo);
        }


        private void ExecuteNavigatedTo(NavigateTo obj)
        {
            if (obj.Name == "loaded")
            {
                Type view = (Type)obj.View;
                if (view == (typeof(VeranderWachtwoord)))
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
#pragma warning disable CS1998
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    this.Loading = false;
                    RaiseAll();

                });
#pragma warning restore CS1998

            });
        }



        private async void RaiseAll()
        {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                RaisePropertyChanged("Loading");
                RaisePropertyChanged("Password");
                RaisePropertyChanged("PasswordRepeat");
                RaisePropertyChanged("Error");
            });
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        }


        private void Cancel()
        {
            Frame rootFrame = MainViewVM.MainFrame as Frame;
            rootFrame.GoBack();
           
        }

        private void Wijzig()
        {
            //verander wachtwoord en keer terug naar profiel
            if(Password == PasswordRepeat)
            {
                ChangePassword();
            }
            else
            {
                this.Error = Libraries.Error.Password;
                RaiseAll();
            }
        }

        public async void ChangePassword()
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
            EditUser.Password = Libraries.md5.Create(Password);

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
    }
}
