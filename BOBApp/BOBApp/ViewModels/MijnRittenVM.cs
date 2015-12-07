using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Libraries.Models;
using Libraries.Models.relations;
using Libraries.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOBApp.ViewModels
{
    public class MijnRittenVM : ViewModelBase
    {
        //Properties
        public string SearchLocation { get; set; }
        private Task task;
      

        public List<Trip.All> Trips { get; set; }
        public Trip CurrentTrip { get; set; }

        //Constructor
        public MijnRittenVM()
        {
            GetTrips();
            GetCurrentTrip();
            RaisePropertyChanged("Trips");
        }


        //Methods
        private async void GetCurrentTrip()
        {
            this.CurrentTrip = await TripRepository.GetCurrentTrip();
        }
        private async void GetTrips()
        {
            List<Trip> trips = await TripRepository.GetTrips();

            List<Trip.All> trips_all = new List<Trip.All>();

           
            for (int i = 0; i < trips.Count; i++)
            {
                User.All user= await UsersRepository.GetUserById(trips[i].Users_ID);
                Bob.All bob = await BobsRepository.GetBobById(trips[i].Bobs_ID);
                Users_Destinations destination = await DestinationRepository.GetDestinationById((trips[i].Destinations_ID));
                Trip.All newTrip = new Trip.All();

                newTrip.Trip = trips[i];
                newTrip.User = user.User;
                newTrip.Bob = bob.Bob;
                newTrip.Destination = destination;
                trips_all.Add(newTrip);
            }

            this.Trips = trips_all;

        }
    }
}
