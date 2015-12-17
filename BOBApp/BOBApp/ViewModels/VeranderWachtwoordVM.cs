using BOBApp.Messages;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOBApp.ViewModels
{
    public class VeranderWachtwoordVM
    {
        public RelayCommand WijzigCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }

        public VeranderWachtwoordVM()
        {
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
            Messenger.Default.Send<GoToPage>(new GoToPage()
            {
                //Keer terug naar profiel scherm
                Name = "Profiel"
            });
        }
    }
}
