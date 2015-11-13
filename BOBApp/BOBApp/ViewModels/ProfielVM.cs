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

namespace BOBApp.ViewModels
{
    public class ProfielVM : ViewModelBase
    {
        //Properties
        public RelayCommand AanpasCommand { get; set; }
        public Register User { get; set; }
        public String Password { get; set; }
        public String PasswordRepeat { get; set; }

        //Constructor
        public ProfielVM()
        {

            //GetUserDetails zou moeten werken, pas testbaar na de navigatie implementatie, login en register
            //GetUserDetails();

            //Testen met statische data
            User = new Register{ Lastname = "Van Lancker", Firstname = "Kevin", Email = "Test@test.be", Cellphone = "0494616943", LicensePlate = "1-43AE42", Password = "123" };
            AanpasCommand = new RelayCommand(Aanpassen);
        }

    

        //Methods
        public void Aanpassen()
        {
            //Testen of knop werkt
            Debug.WriteLine("knop werkt");

            //testen of databinding werkt
            if(PasswordRepeat == Password)
            {
                User.Password = Password;
                Debug.WriteLine("Password verandert");
            }
            else
            {
                Debug.WriteLine("Password niet verandert");
            }

            Debug.WriteLine("naam: " + User.Lastname + " - Voornaam: " + User.Firstname + " - Email: " + User.Email + " - Cellphone: " + User.Cellphone + " - LicensePlate: " + User.LicensePlate + " - Password: " + User.Password);
        }

        //Haal de gegevens op van de user via zijn E-mail
        private async void GetUserDetails()
        {
            this.User = await RegisterRepository.GetUser(BaseViewModelLocator.USER.Email);
        }

    }
}
