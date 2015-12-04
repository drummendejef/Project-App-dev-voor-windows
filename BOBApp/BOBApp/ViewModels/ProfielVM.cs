using BOBApp.Models;
using BOBApp.Repositories;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
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
        public RelayCommand AanpasCommand { get; set; }
        public User User { get; set; }
        public String Password { get; set; }
        public String PasswordRepeat { get; set; }
        public List<Autotype> Merken { get; set; }

        //Constructor
        public ProfielVM()
        {

            //GetUserDetails() werkt
            GetUserDetails();

            GetMerken();

            //Testen met statische data ( momenteel nog laten staan, in geval dit nog handig kan zijn voor iets)
            //User = new Register{ Lastname = "Van Lancker", Firstname = "Kevin", Email = "Test@test.be", Cellphone = "0494616943", LicensePlate = "1-43AE42", Password = "123" };
            AanpasCommand = new RelayCommand(Aanpassen);
        }

    

        //Methods
        public async void Aanpassen()
        {
            //Dit werkt
            Debug.WriteLine("knop werkt");

            //databinding werkt/Password veranderen werkt ook
            if(PasswordRepeat == null){
                Debug.WriteLine("Password niet verandert");
                
                //Updaten naar database
                Response r = await UserRepository.EditUser(User);
                //na de edit ( en deze is correct) toon bevestiging aan de gebruiker
                if (r.Success)
                {
                    var dialog = new MessageDialog("Uw gegevens zijn opgeslaan.");
                    await dialog.ShowAsync();
                }
                else
                {
                    var dialog = new MessageDialog("Er is een probleem met het updaten van uw gegevens. Probeer het nog eens opnieuw.");
                    await dialog.ShowAsync();
                }
            }
            else if(PasswordRepeat == Password)
            {
                
                User.Password = Password;
              
                //Updaten naar database
                Response r = await UserRepository.EditUser(User);
                //na de edit ( en deze is correct) toon bevestiging aan de gebruiker
                if (r.Success)
                {
                    var dialog = new MessageDialog("Uw gegevens zijn opgeslaan en uw wachtwoord is verandert.");
                    await dialog.ShowAsync();
                }
                else
                {
                    var dialog = new MessageDialog("Er is een probleem met het updaten van uw gegevens. Probeer het nog eens opnieuw.");
                    await dialog.ShowAsync();
                }

                //Debug.WriteLine("Password verandert");
            }
            else
            {
                //de wachtwoorden zijn niet hetzelfde
                var dialog = new MessageDialog("De twee wachtwoorden zijn niet hetzelfde.");
                await dialog.ShowAsync();
            }

           
            //Testcode voor databinding
            //Debug.WriteLine("naam: " + User.Lastname + " - Voornaam: " + User.Firstname + " - Email: " + User.Email + " - Cellphone: " + User.Cellphone + " - LicensePlate: " + User.LicensePlate + " - Password: " + User.Password);
            
        }

       //Dit werkt
        private async void GetUserDetails()
        {
            this.User = await UserRepository.GetUser();
        }

        private async void GetMerken()
        {
            this.Merken = await MerkenRepository.GetMerken();
        }

    }
}
