using BOBApp.Models;
using BOBApp.Repositories;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOBApp.ViewModels
{
    public class ProfielVM
    {
        //Properties
        public RelayCommand AanpasCommand { get; set; }
        public Register User { get; set; }

        //Constructor
        public ProfielVM()
        {
            
           //GetUserDetails werkt nog niet
           //GetUserDetails(BaseViewModelLocator.USER.Email);

            AanpasCommand = new RelayCommand(Aanpassen);
        }

    

        //Methods
        public void Aanpassen()
        {
            //Testen of knop werkt
            Debug.WriteLine("knop werkt");
        }

        //Haal de gegevens op van de user via zijn E-mail
        private async void GetUserDetails(string email)
        {
          this.User = await UserRepository.GetUser(email);
        }

    }
}
