using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Libraries.Models;
using Libraries.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace BOBApp.ViewModels
{
    public class ProfielVM : ViewModelBase
    {
        //Properties
        public Task task { get; set; }
        public RelayCommand AanpasCommand { get; set; }
        public User.Profile User { get; set; }
        public String Password { get; set; }
        public String PasswordRepeat { get; set; }
        public ObservableCollection<Autotype> Merken { get; set; }
        public User.PutUser EditUser { get; set; }

        //Constructor
        public ProfielVM()
        {

            //GetUserDetails() werkt
            GetUserDetails();
            GetMerken();

            RaisePropertyChanged("Merken");

            //Testen met statische data ( momenteel nog laten staan, in geval dit nog handig kan zijn voor iets)
            //User = new Register{ Lastname = "Van Lancker", Firstname = "Kevin", Email = "Test@test.be", Cellphone = "0494616943", LicensePlate = "1-43AE42", Password = "123" };
            AanpasCommand = new RelayCommand(Aanpassen);

           
        }

        //Methods
        public async void Aanpassen()
        {
            EditUser = new User.PutUser();

            //Alle gegevens van ene klasse naar andere overbrengen
            if (User.Bob != null)
            {
                User.Bob = new Bob();
            }
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
        }

        private async void GetMerken()
        {
            List<Autotype> merkenLijst = await AutotypeRepository.GetAutotypes();
            this.Merken = new ObservableCollection<Autotype>();
            foreach (Autotype merk in merkenLijst)
            {
                this.Merken.Add(merk);
            }
        }

    }
}
