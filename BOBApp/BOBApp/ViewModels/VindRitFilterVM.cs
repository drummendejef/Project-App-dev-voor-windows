using BOBApp.Messages;
using BOBApp.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Libraries.Models;
using Libraries.Models.relations;
using Libraries.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace BOBApp.ViewModels
{
    public class VindRitFilterVM: ViewModelBase
    {

        #region props

        public bool Loading { get; set; }
        public string Error { get; set; }


        #region static
        public static List<Friend.All> SelectedFriends { get; set; }
        public static Users_Destinations SelectedDestination { get; set; }
        public static BobsType SelectedBobsType { get; set; }
        public static int SelectedRating { get; set; }
        public static DateTime? SelectedMinDate { get; set; }
        public static string SelectedParty { get; set; }
        #endregion



        //others     
        public List<Friend.All> Friends { get; set; }
        public string SelectedFriendString { get; set; }
        public RelayCommand AddFriendCommand { get; set; }
        public List<BobsType> BobsTypes { get; set; }
        public List<Party> Parties { get; set; }
        public List<Users_Destinations> Destinations { get; set; }


        #endregion

        //Constructor
        public VindRitFilterVM()
        {
          
            AddFriendCommand = new RelayCommand(AddFriend);
           

            Messenger.Default.Register<NavigateTo>(typeof(bool), ExecuteNavigatedTo);
            RaiseAll();
        }


        private void RaiseAll()
        {
            RaisePropertyChanged("Loading");
            RaisePropertyChanged("Error");
            RaisePropertyChanged("SelectedFriendString");
            RaisePropertyChanged("BobsTypes");
            RaisePropertyChanged("Parties");
            RaisePropertyChanged("Destinations");
            RaisePropertyChanged("Friends");
        }
        private void ExecuteNavigatedTo(NavigateTo obj)
        {
            if (obj.Name == "loaded")
            {
                Type view = (Type)obj.View;
                if (view == (typeof(VindRitFilter)))
                {
                    Loaded();
                }


            }
        }

        private async void Loaded()
        {
            this.Loading = true;
            RaisePropertyChanged("Loading");

            await Task.Run(async () =>
            {
                // running in background
                VindRitFilterVM.SelectedFriends = new List<Friend.All>();
                VindRitFilterVM.SelectedDestination = new Users_Destinations();
                VindRitFilterVM.SelectedBobsType = new BobsType();
                VindRitFilterVM.SelectedParty = "";
                VindRitFilterVM.SelectedMinDate = DateTime.Today;

                Task task = GetFriends();
                Task task2 = GetBobTypes();
                Task task3 = GetDestinations();
                Task task4 = GetParties();

#pragma warning disable CS1998
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    this.Loading = false;
                    RaiseAll();

                });
#pragma warning restore CS1998

            });
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
            RaiseAll();
        }
        private async Task GetDestinations()
        {
            this.Destinations = await DestinationRepository.GetDestinations();
            VindRitFilterVM.SelectedDestination= this.Destinations[0];
            RaiseAll();
        }
        private async Task<Boolean> GetFriends()
        {
            this.Friends = await FriendsRepository.GetFriends();
            RaiseAll();
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

            RaiseAll();
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
