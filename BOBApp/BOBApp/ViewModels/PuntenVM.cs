using BOBApp.Repositories;
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
        public string Points { get; set; }
        public string PointsText { get; set; }

        //Constructor
        public PuntenVM()
        {
           
            GetUserPoints();
            this.PointsText = "U hebt " + Points + " punten.";
        }

        //Methods
        private async void GetUserPoints()
        {
            this.Points = await PointRepository.GetTotalPoints();

        }

    }
}
