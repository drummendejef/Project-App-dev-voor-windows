using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Libraries.Models;
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
        public static Destination SelectedDestination;

        //others
        private Task task;
      
        public string Error { get; set; }
        public List<Friend.All> Friends { get; set; }
        
        public string SelectedFriendString { get; set; }

        public RelayCommand AddFriendCommand { get; set; }
        public BobsType SelectedBobsType { get; set; }
        public List<BobsType> BobsTypes { get; set; }

        public List<Destination> Destinations { get; set; }

        //Constructor
        public VindRitFilterVM()
        {
            Task task = GetFriends();
            Task task2 = GetBobTypes();
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
