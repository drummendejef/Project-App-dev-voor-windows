using BOBApp.Messages;
using BOBApp.Models;
using BOBApp.Repositories;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Libraries;
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
        public RelayCommand RegisterCommand { get; set; }
        public RelayCommand Login_FacebookCommand { get; set; }
        public string Email { get; set; }
        public string Pass { get; set; }
        public string Error { get; set; }

        //Constructor
        public LoginVM()
        {
            // Cities = new ObservableCollection<string>(new CityRepository().GetCities());
            RaisePropertyChanged("Email");
            RaisePropertyChanged("Pass");
            LoginCommand = new RelayCommand(Login);
            RegisterCommand = new RelayCommand(Register);
            Login_FacebookCommand = new RelayCommand(Login_Facebook);
            Email = "stijn.vanhulle@outlook.com";
            Pass = "test";
        }

     

        //Methods
        public void Login()
        {
            loginTask = LoginUser(this.Email,this.Pass);
        }
        private void Login_Facebook()
        {
            loginTask = LoginFacebook();
        }
        private void Register()
        {
            Messenger.Default.Send<GoToPage>(new GoToPage()
            {
                Name = "Register"
            });
        }

        private async Task<Boolean> LoginUser(string email, string pass)
        {
            Response res = await LoginRepository.Login(email, md5.Create(pass));
            if (res.Success == true)
            {
                Login user = await LoginRepository.GetUser();
                BaseViewModelLocator.USER = user;
                //navigate to ritten
                Messenger.Default.Send<GoToPage>(new GoToPage()
                {
                    Name = "MainView"
                });
            }
            else
            {
                this.Error = res.Error;
            }

            return res.Success;
        }
        private async Task<Boolean> LoginFacebook()
        {
            Boolean ok = await LoginRepository.LoginFacebook();
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
