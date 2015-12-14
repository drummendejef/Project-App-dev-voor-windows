using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Libraries.Models;
using Libraries.Models.relations;
using Libraries.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace BOBApp.ViewModels
{
    public class MijnRittenVM : ViewModelBase
    {
        //Properties
        public string SearchLocation { get; set; }
        private Task task;
     

        public List<Trip.All> Trips { get; set; }
        public Trip.All CurrentTrip { get; set; }
        public HorizontalAlignment Car { get; set; }

        //Constructor
        public MijnRittenVM()
        {
            GetTrips();
            GetCurrentTrip();
            RaisePropertyChanged("Trips");
            RaisePropertyChanged("CurrentTrip");
            RaisePropertyChanged("Car");

            MainViewVM.socket.On("update_trip", (msg) =>
            {
                Libraries.Socket _socket = JsonConvert.DeserializeObject<Libraries.Socket>((string)msg);
                //if (_socket.Status == true && _socket.To==MainViewVM.USER.ID)
                if (_socket.Status == true)
                {
                    GetCurrentTrip();
                    GetTrips();
                }

            });
        }



        //Methods
        private async void GetCurrentTrip()
        {
            Trip currenttrip = Task.FromResult<Trip>(await TripRepository.GetCurrentTrip()).Result;

            Trip.All trips_all = new Trip.All();


            User.All user = await UsersRepository.GetUserById(currenttrip.Users_ID);
            Bob.All bob = await BobsRepository.GetBobById(currenttrip.Bobs_ID);
            Users_Destinations destination = await DestinationRepository.GetDestinationById((currenttrip.Destinations_ID));
            Party party = await PartyRepository.GetPartyById(currenttrip.Party_ID);
            Trip.All newTrip = new Trip.All();

           

            newTrip.Trip = currenttrip;
            newTrip.Party = party;
            newTrip.User = user.User;
            newTrip.Bob = bob.User;
            newTrip.Destination = destination;

            MoveCar(newTrip.Trip.Status_Name);

            this.CurrentTrip = newTrip;

        }

        private async void MoveCar(string status_Name)
        {
            List<Status> statuses = Task.FromResult<List<Status>>(await StatusRepository.GetStatuses()).Result;

            if (status_Name == statuses[0].Name) {

            }
            switch (status_Name)
            {
                case "Aangevraagd":
                    Car = HorizontalAlignment.Left;
                  
                    break;
                case "Onderweg":
                    Car = HorizontalAlignment.Center;
                 
                    break;
                case "Toegekomen":
                    Car = HorizontalAlignment.Right;
                  
                    break;
                case "Done":
                    Car = HorizontalAlignment.Right;
                  
                    break;
                default:
                    break;
            }
            RaisePropertyChanged("Car");
            RaisePropertyChanged("CurrentTrip");
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
                Party party = await PartyRepository.GetPartyById(trips[i].Party_ID);
                Trip.All newTrip = new Trip.All();

                newTrip.Trip = trips[i];
                newTrip.User = user.User;
                newTrip.Bob = bob.User;
                newTrip.Party = party;
                newTrip.Destination = destination;
                trips_all.Add(newTrip);
            }

            this.Trips = trips_all;

        }
    }
}
