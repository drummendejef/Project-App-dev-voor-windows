using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOBApp.ViewModels
{
    public class ProfielVM
    {
        //Properties
        public RelayCommand AanpasCommand { get; set; }

        //Constructor
        public ProfielVM()
        {
            AanpasCommand = new RelayCommand(Aanpassen);
        }

        //Methods
        public void Aanpassen()
        {

        }

    }
}
