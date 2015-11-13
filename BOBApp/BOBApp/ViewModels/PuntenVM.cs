using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOBApp.ViewModels
{
    public class PuntenVM : ViewModelBase
    {
        //Properties
        public string Punten { get; set; }

        //Constructor
        public PuntenVM()
        {
            Punten = "5";
            Punten += " Punten";
        }

        //Methods

    }
}
