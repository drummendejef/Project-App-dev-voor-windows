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
    public class RegisterVM: ViewModelBase
    {
        //Properties
        private Task registerTask;
        public RelayCommand RegisterCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public Register NewRegister { get; set; }
        public string Password { get; set; }
        public string PasswordRepeat { get; set; }
        public string Error { get; set; }

        //Constructor
        public RegisterVM()
        {
            this.NewRegister = new Register();

            RaisePropertyChanged("NewRegister");
            RaisePropertyChanged("Password");
            RaisePropertyChanged("PasswordRepeat");
            RaisePropertyChanged("Error");
            RegisterCommand = new RelayCommand(Register);
            CancelCommand = new RelayCommand(Cancel);

        }



        //Methods
        private void Cancel()
        {
            Messenger.Default.Send<GoToPage>(new GoToPage()
            {
                //Keer terug naar login scherm
                Name = "Login"
            });
        }
        public void Register()
        {
           
            registerTask = RegisterUser(NewRegister);
        }
        private async Task<Boolean> RegisterUser(Register register)
        {
            if (Password == PasswordRepeat)
            {
                register.Password = Password;
                Response res = await RegisterRepository.Register(register);
                if (res.Success == true)
                {
                   await LoginUser(register.Email, register.Password);
                }
                else
                {
                    this.Error = res.Error;
                }

                return res.Success;
            }
            else
            {
                this.Error = Libraries.Error.Password;
                return false;
            }

          
        }


        //login, zelfde als bij loginVM
        private async Task<Boolean> LoginUser(string email, string pass)
        {
            Response res = await LoginRepository.Login(email, pass);
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


    }
}
