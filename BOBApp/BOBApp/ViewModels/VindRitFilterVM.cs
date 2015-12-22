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

        //filter
        public static List<Friend.All> SelectedFriends { get; set; }

       

        public static Users_Destinations SelectedDestination { get; set; }
        public static BobsType SelectedBobsType { get; set; }
        public static int SelectedRating { get; set; }
        public static DateTime? SelectedMinDate { get; set; }
        public static string SelectedParty { get; set; }
       



        //others
        private Task task;
     
        public string Error { get; set; }
        public List<Friend.All> Friends { get; set; }
        
        public string SelectedFriendString { get; set; }

        public RelayCommand AddFriendCommand { get; set; }

        public List<BobsType> BobsTypes { get; set; }
        public List<Party> Parties { get; set; }

        public List<Users_Destinations> Destinations { get; set; }

        
       

        //Constructor
        public VindRitFilterVM()
        {
            Task task = GetFriends();
            Task task2 = GetBobTypes();
            Task task3 = GetDestinations();
            Task task4 = GetParties();
            AddFriendCommand = new RelayCommand(AddFriend);
            RaisePropertyChanged("SelectedFriendString");

            VindRitFilterVM.SelectedFriends = new List<Friend.All>();
            VindRitFilterVM.SelectedDestination = new Users_Destinations();
            VindRitFilterVM.SelectedBobsType = new BobsType();
            VindRitFilterVM.SelectedParty = "";
            VindRitFilterVM.SelectedMinDate = DateTime.Today;
        }

        


        //Methods
        private void AddFriend()
        {
            if (this.SelectedFriendString != null)
            {
                Friend.All friend = this.Friends.Where(r => r.User2.ToString() == this.SelectedFriendString).First();
                VindRitFilterVM.SelectedFriends.Add(friend);
            }
           

        }

        #region gets

        private async Task GetBobTypes()
        {
            this.BobsTypes = await BobsRepository.GetTypes();
            VindRitFilterVM.SelectedBobsType= this.BobsTypes[0];
        
            RaisePropertyChanged("_SelectedBobsType");
        }
        private async Task GetDestinations()
        {
            this.Destinations = await DestinationRepository.GetDestinations();
            VindRitFilterVM.SelectedDestination= this.Destinations[0];
         
            RaisePropertyChanged("_SelectedDestination");
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

        private async Task<Boolean> GetParties()
        {

            this.Parties = await PartyRepository.GetParties();

            RaisePropertyChanged("Parties");
            if (this.Parties.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        #endregion


    }
}
