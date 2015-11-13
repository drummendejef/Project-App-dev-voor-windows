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
        public string TotalPoints { get; set; }
        public string PointsText { get; set; }

        //Constructor
        public PuntenVM()
        {
           GetPoints();
        }

        //Methods
        private async void GetPoints()
        {
            //problemen hierbij, kijk wat erin zit,  server werkt niet op moment van testen, moet nog gedaan worden
            TotalPoints = await PointRepository.GetTotalPoints();
            PointsText = TotalPoints + " punten.";
        }

    }
}
