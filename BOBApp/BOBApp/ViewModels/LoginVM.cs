using BOBApp.Messages;
using BOBApp.Models;
using BOBApp.services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOBApp.ViewModels
{
    public class LoginVM:ViewModelBase
    {
        //Properties
        private Task loginTask;
        public RelayCommand LoginCommand { get; set; }
        public string Email { get; set; }
        public string Pass { get; set; }

        //Constructor
        public LoginVM()
        {
            // Cities = new ObservableCollection<string>(new CityRepository().GetCities());
            RaisePropertyChanged("Email");
            RaisePropertyChanged("Pass");
            LoginCommand = new RelayCommand(Login);  
        }
        
        //Methods
        public void Login()
        {
            loginTask = LoginUser(this.Email,this.Pass);
        }
        
        private async Task<Boolean> LoginUser(string email, string pass)
        {
            Boolean ok = await LoginRepository.Login(email, pass);
            if (ok == true)
            {
                Login user = await LoginRepository.GetUser();
                BaseViewModelLocator.USER = user;
                //navigate to ritten
                Messenger.Default.Send<GoToPage>(new GoToPage()
                {
                    Name = "MainView"
                });
            }

            return ok;
        }

   
       
    }
}
