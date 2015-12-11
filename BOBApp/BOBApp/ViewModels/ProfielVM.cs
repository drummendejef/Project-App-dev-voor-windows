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
            
            //Dit werkt
            Debug.WriteLine("knop werkt");
            Debug.WriteLine(User.Autotype.ToString());
            //databinding werkt

            //Updaten naar database
            //Probleem nog met user
           /* User EditUser = null;
                Response r = await UserRepository.EditUser(EditUser);
                //na de edit ( en deze is correct) toon bevestiging aan de gebruiker
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
         */
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
