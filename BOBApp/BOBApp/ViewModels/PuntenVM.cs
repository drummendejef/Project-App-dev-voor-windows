using GalaSoft.MvvmLight;
using Libraries.Models;
using Libraries.Repositories;
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
        public List<Point> Points { get; set; }
        public string TotalPoints { get; set; }
        public string PointsText { get; set; }

        //Constructor

        public PuntenVM()
        {
           
            GetUserPoints();
            GetPoints();
            this.PointsText = "U hebt " + TotalPoints + " punten.";
        }

        //Methods
        private async void GetUserPoints()
        {
            this.TotalPoints = await PointRepository.GetTotalPoints();

        }
        private async void GetPoints()
        {
            this.Points = await PointRepository.GetPoints();

        }

    }
}
