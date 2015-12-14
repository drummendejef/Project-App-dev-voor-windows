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
       

        //others
        private Task task;
     
        public string Error { get; set; }
        public List<Friend.All> Friends { get; set; }
        
        public string SelectedFriendString { get; set; }

        public RelayCommand AddFriendCommand { get; set; }

        public List<BobsType> BobsTypes { get; set; }
     

        private int __SelectedRating;

        public int _SelectedRating
        {
            get { return __SelectedRating; }
            set { __SelectedRating = value; VindRitVM.Filter.SelectedRating = value; }
        }

        private BobsType __SelectedBobsType;

        public BobsType _SelectedBobsType
        {
            get { return __SelectedBobsType; }
            set { __SelectedBobsType = value; VindRitVM.Filter.SelectedBobsType = value; }
        }


        public List<Users_Destinations> Destinations { get; set; }
        private Users_Destinations __SelectedDestinations;

        public Users_Destinations _SelectedDestination
        {
            get { return __SelectedDestinations; }
            set { __SelectedDestinations = value; VindRitVM.Filter.SelectedDestination = value; }
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
            VindRitVM.Filter.SelectedFriends.Add(friend);
        }

        #region gets

        private async Task GetBobTypes()
        {
            this.BobsTypes = await BobsRepository.GetTypes();
            _SelectedBobsType = this.BobsTypes[0];
        
            RaisePropertyChanged("_SelectedBobsType");
        }
        private async Task GetDestinations()
        {
            this.Destinations = await DestinationRepository.GetDestinations();
            _SelectedDestination = this.Destinations[0];
         
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

        #endregion


    }
}
