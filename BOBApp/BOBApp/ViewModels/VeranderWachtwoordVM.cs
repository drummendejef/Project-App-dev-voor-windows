using BOBApp.Messages;
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
using Windows.UI.Popups;

namespace BOBApp.ViewModels
{
    public class VeranderWachtwoordVM : ViewModelBase
    {
        public RelayCommand WijzigCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public User.PutUser EditUser { get; set; }
        public User.Profile User { get; set; }

        public string Password { get; set; }
        public string PasswordRepeat { get; set; }
        public string Error { get; set; }




        public VeranderWachtwoordVM()
        {
            GetUserDetails();
            WijzigCommand = new RelayCommand(Wijzig);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void Cancel()
        {
            Messenger.Default.Send<GoToPage>(new GoToPage()
            {
                //Keer terug naar profiel scherm
                Name = "Profiel"
            });
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
                RaisePropertyChanged("Error");
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
            EditUser.IsBob = User.User.IsBob;
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
