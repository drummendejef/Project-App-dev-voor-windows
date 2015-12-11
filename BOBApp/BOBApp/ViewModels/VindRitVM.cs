using BOBApp.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Libraries.Models;
using Libraries.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace BOBApp.ViewModels
{
    public class VindRitVM : ViewModelBase
    {
        //Properties

        //static
        public static Party SelectedParty;
        public static Bob SelectedBob;

        //others
        private Task task;
        public RelayCommand GoChatCommand { get; set; }
        public RelayCommand GoFilterCommand { get; set; }
        public RelayCommand FindBobCommand { get; set; }
        public string Error { get; set; }
        public List<Party> Parties { get; set; }
        public string SelectedPartyString { get; set; }

     


        //Constructor
        public VindRitVM()
        {
            GoChatCommand = new RelayCommand(GoChat);
            GoFilterCommand = new RelayCommand(GoFilter);
            FindBobCommand = new RelayCommand(FindBob);

            task = GetParties();
            RaisePropertyChanged("SelectedPartyString");
        }






        //Methods
        private async Task<Boolean> GetParties()
        {

            this.Parties = await PartyRepository.GetParties();

            if (this.Parties.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        private void GoChat()
        {
            Frame rootFrame = MainViewVM.MainFrame as Frame;

            rootFrame.Navigate(typeof(VindRitChat));
        }
        private void GoFilter()
        {
            Frame rootFrame = MainViewVM.MainFrame as Frame;

            rootFrame.Navigate(typeof(VindRitFilter));
        }


        private async void FindBob()
        {
            List<Bob> bobs = Task.FromResult<List<Bob>>(await FindBob_task()).Result;

            while (bobs.Count > 0)
            {
                bool ok = Task.FromResult<bool>(await ShowBob(bobs.First())).Result;
                if (ok == false)
                {
                    bobs.Remove(bobs.First());
                    if (bobs.Count == 0)
                    {
                        var dialog = new MessageDialog("Geen Bob gevonden");
                        await dialog.ShowAsync();
                    }
                }
                else
                {
                    //take this bob
                    VindRitVM.SelectedBob = bobs.First();
                    break;

                }

            }
        }



        private async Task<bool> ShowBob(Bob bob)
        {
            var dialog = new MessageDialog(bob.LicensePlate);

            dialog.Commands.Add(new UICommand("Take bob") { Id = 0 });
            dialog.Commands.Add(new UICommand("Next bob") { Id = 1 });

            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily != "Windows.Mobile")
            {
                // Adding a 3rd command will crash the app when running on Mobile !!!
                dialog.Commands.Add(new UICommand("Maybe later") { Id = 2 });
            }

            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            var result = await dialog.ShowAsync();

            int id = int.Parse(result.Id.ToString());
            if(id == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //wanneer bob accepteer
        private async void MakeTrip()
        {
            Bob selectedBob = VindRitVM.SelectedBob;
            Party SelectedParty = VindRitVM.SelectedParty;
            Destination SelectedDestination = VindRitFilterVM.SelectedDestination;
            List<Friend.All> selectedFriends = VindRitFilterVM.SelectedFriends;


            Trip trip = new Trip()
            {
                Bobs_ID = selectedBob.ID.Value,
                Friends = JsonConvert.SerializeObject(selectedFriends),
                Destinations_ID = SelectedDestination.ID,
            };
            Response res = Task.FromResult<Response>(await TripRepository.PostTrip(trip)).Result;
            
            if (res.Success == true)
            {
                MakeChatroom(selectedBob.ID.Value);
            }
        }

        private async void MakeChatroom(int bobs_ID)
        {
            Response res = Task.FromResult<Response>(await ChatRoomRepository.PostChatRoom(bobs_ID)).Result;

            if (res.Success == true)
            {
                VindRitChatVM.ID = res.NewID.Value;
                Frame rootFrame = MainViewVM.MainFrame as Frame;

                rootFrame.Navigate(typeof(VindRitChat));
            }
           
        }


        //tasks
        private async Task<List<Bob>> FindBob_task()
        {
            if (this.SelectedPartyString != null)
            {
                VindRitVM.SelectedParty = Parties.Where(r => r.Name == this.SelectedPartyString).First();
            }
           
            int? rating = null;
            DateTime minDate = DateTime.Today;
            int bobsType_ID = 1;
            Location location = new Location() { Latitude = 51.177134640708495, Longitude = 3.2177382568767574 };
            int? maxDistance = null;

            List<Bob> bobs = await BobsRepository.FindBobs(rating, minDate, bobsType_ID, location, maxDistance);

          

            return bobs;
        }

    }
}
