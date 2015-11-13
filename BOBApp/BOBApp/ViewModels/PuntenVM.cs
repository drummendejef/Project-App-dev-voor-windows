using BOBApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOBApp.ViewModels
{
    public class PuntenVM
    {
        //Properties
        public int TotalPoints { get; set; }
        public string PointsText { get; set; }

        //Constructor
        public PuntenVM()
        {
           GetPoints();
        }

        //Methods
        private async void GetPoints()
        {
            this.TotalPoints = await PointRepository.GetTotalPoints();
            this.PointsText = TotalPoints + " punten.";
        }

    }
}
