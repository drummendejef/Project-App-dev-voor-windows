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

            //GetUserDetails() werkt
            GetUserDetails();

            //Testen met statische data ( momenteel nog laten staan, in geval dit nog handig kan zijn voor iets)
            //User = new Register{ Lastname = "Van Lancker", Firstname = "Kevin", Email = "Test@test.be", Cellphone = "0494616943", LicensePlate = "1-43AE42", Password = "123" };
            AanpasCommand = new RelayCommand(Aanpassen);
        }

    

        //Methods
        public void Aanpassen()
        {
            //Dit werkt
            Debug.WriteLine("knop werkt");

            //databinding werkt/Password veranderen werkt ook
            if(PasswordRepeat == Password)
            {
                User.Password = Password;
                //Updaten naar database ( + confirmatie dat password verandert is ( samen met eventuele andere aanpassingen)
                Debug.WriteLine("Password verandert");
            }
            else
            {
                Debug.WriteLine("Password niet verandert");
            }

            //Updaten naar database ( + confirmatiescherm dat gegevens verandert zijn)
            Debug.WriteLine("naam: " + User.Lastname + " - Voornaam: " + User.Firstname + " - Email: " + User.Email + " - Cellphone: " + User.Cellphone + " - LicensePlate: " + User.LicensePlate + " - Password: " + User.Password);

        }

       //Dit werkt
        private async void GetUserDetails()
        {
            this.User = await RegisterRepository.GetUser();
        }

    }
}
