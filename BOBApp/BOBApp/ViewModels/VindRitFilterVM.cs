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
    public class VindRitFilterVM: ViewModelBase
    {
        //Properties

        //static
        public static List<Friend.All> SelectedFriends = new List<Friend.All>();
        public static Users_Destinations SelectedDestination= new Users_Destinations();
        public static BobsType SelectedBobsType= new BobsType();
        public static int SelectedRating = 0;
        public static DateTime? SelectedMinDate = DateTime.Today;

        //others
        private Task task;
      
        public string Error { get; set; }
        public List<Friend.All> Friends { get; set; }
        
        public string SelectedFriendString { get; set; }

        public RelayCommand AddFriendCommand { get; set; }

        public List<BobsType> BobsTypes { get; set; }
        private BobsType __SelectedBobsType;

        public BobsType _SelectedBobsType
        {
            get { return __SelectedBobsType; }
            set { __SelectedBobsType = value; VindRitFilterVM.SelectedBobsType = value; }
        }


        public List<Users_Destinations> Destinations { get; set; }
        private Users_Destinations __SelectedDestinations;

        public Users_Destinations _SelectedDestination
        {
            get { return __SelectedDestinations; }
            set { __SelectedDestinations = value; VindRitFilterVM.SelectedDestination = value; }
        }
        
       

        //Constructor
        public VindRitFilterVM()
        {
            Task task = GetFriends();
            Task task2 = GetBobTypes();
            Task task3 = GetDestinations();
            AddFriendCommand = new RelayCommand(AddFriend);
            RaisePropertyChanged("SelectedFriendString");
        }

        


        //Methods
        private void AddFriend()
        {
            Friend.All friend = this.Friends.Where(r => r.User2.ToString() == this.SelectedFriendString).First();
            VindRitFilterVM.SelectedFriends.Add(friend);
        }
        private async Task GetBobTypes()
        {
            this.BobsTypes = await BobsRepository.GetTypes();
            _SelectedBobsType = this.BobsTypes[0];
        }
        private async Task GetDestinations()
        {
            this.Destinations = await DestinationRepository.GetDestinations();
            _SelectedDestination = this.Destinations[0];
        }
        private async Task<Boolean> GetFriends()
        {

            this.Friends = await FriendsRepository.GetFriends();

            if (this.Friends.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
      
    }
}
