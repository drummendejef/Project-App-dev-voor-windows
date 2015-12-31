using BOBApp.Messages;
using BOBApp.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Libraries.Models;
using Libraries.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BOBApp.ViewModels
{
    public class ZoekVriendenVM : ViewModelBase
    {
        //public static Frame Frame;
        //Properties
        public bool Loading { get; set; }
        public string Error { get; set; }


        public Visibility VisibleModal { get; set; }
        public Frame Frame { get; set; }
        public string SearchUser { get; set; }
        public List<Friend.All> Friends { get; set; }


        public RelayCommand ShowModalCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }
        public RelayCommand CloseModalCommand { get; set; }

        //search
        public RelayCommand SearchItemCommand { get; set; }
        private string _SearchItem;

        public string SearchItem
        {
            get { return _SearchItem; }
            set
            {
                _SearchItem = value;
                if (_SearchItem == null || _SearchItem.Trim() == "")
                {
                    if (friends_all != null)
                    {
                        this.Friends = friends_all;
                        RaisePropertyChanged("Friends");
                    }
                }
            }
        }

        //Constructor
        public ZoekVriendenVM()
        {
           
          
          
            CloseModalCommand = new RelayCommand(CloseModal);
            ShowModalCommand = new RelayCommand(ShowModal);
            SearchCommand = new RelayCommand(SearchByEmail);
            SearchItemCommand = new RelayCommand(Search);

            VisibleModal = Visibility.Collapsed;

            Messenger.Default.Register<NavigateTo>(typeof(bool), ExecuteNavigatedTo);
          
            RaiseAll();
        }

      

        private void RaiseAll()
        {
            RaisePropertyChanged("VisibleModal");
            try
            {
                RaisePropertyChanged("Frame");
            }
            catch (Exception ex)
            {

                
            }
            RaisePropertyChanged("SearchUsers");
            RaisePropertyChanged("Loading");
            RaisePropertyChanged("Error");
            RaisePropertyChanged("Friends");

            RaisePropertyChanged("SearchItem");

        }

        private async void Loaded()
        {
            this.Loading = true;
            RaisePropertyChanged("Loading");



            await Task.Run(async () =>
            {
                // running in background
#pragma warning disable CS1998
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {

                    VisibleModal = Visibility.Collapsed;

                    this.Frame = new Frame();


                    await GetFriends();
                    this.Loading = false;
                    RaiseAll();




                });
#pragma warning restore CS1998

            });
        }

      

        private async void ExecuteNavigatedTo(NavigateTo obj)
        {
            if (obj.Name == "loaded")
            {
                Type view = (Type)obj.View;
                if (view == (typeof(ZoekVrienden)))
                {
                    //loaded
                    Loaded();
                }
            }

            if (obj.Name == "friend_accepted")
            {
                User user = (User)obj.Data;
                bool accepted = (bool) obj.Result;
                Response response = Task.FromResult<Response>(await FriendsRepository.PostFriend(MainViewVM.USER.ID,user.ID,accepted)).Result;

                if (response.Success == true)
                {
                    Messenger.Default.Send<Dialog>(new Dialog()
                    {
                        Message = user.ToString() + " is toegevoegd",
                    });

                    Libraries.Socket socketSend = new Libraries.Socket() { From = MainViewVM.USER.ID, To = user.ID, Status = true };

                    MainViewVM.socket.Emit("friend_ADDED:send", JsonConvert.SerializeObject(socketSend));
                }
                else
                {
                    //iets misgelopen
                }
               
              
            }
        }



        //Methods

        #region modal
        private void CloseModal()
        {
            VisibleModal = Visibility.Collapsed;
            RaiseAll();
        }

        private void ShowModal()
        {
            this.Frame = new Frame();
            this.Frame.Navigate(typeof(ZoekVrienden_Add));
           

            VisibleModal = Visibility.Visible;
            RaiseAll();

        }
        #endregion


        private void Search()
        {
            if (SearchItem == null)
            {
                return;
            }

            string item = this.SearchItem.ToString().Trim().ToLower();

            var newItems = friends_all.Where(r => r.User2.ToString().Trim().ToLower() == item).ToList();
            if (newItems == null || newItems.Count == 0)
            {
                newItems = friends_all.Where(r => r.User2.Firstname.Trim().ToLower() == item).ToList();

                if (newItems == null || newItems.Count == 0)
                {
                    newItems = friends_all.Where(r => r.User2.Lastname.Trim().ToLower() == item).ToList();
                }

            }





            if (newItems != null && newItems.Count > 0)
            {
                this.Friends = newItems;
                this.Error = null;
            }
            else
            {
                this.Error = "Geen vrienden gevonden";
            }

            RaiseAll();
        }

        private void SearchByEmail()
        {
            if (this.SearchUser == null)
            {
                this.SearchUser = "";
            }
            FindUserByEmail(this.SearchUser.Trim());
        }


        List<Friend.All> friends_all = new List<Friend.All>();
        private async Task GetFriends()
        {
            this.Loading = true;
            RaisePropertyChanged("Loading");

            friends_all = await FriendsRepository.GetFriends();

            var count = friends_all.Count;
            if (friends_all.Count >= 10)
            {
                count = 10;
            }


            this.Friends = friends_all;
            this.Loading = false;
            RaiseAll();
        }


        #region add friend


        private async void FindUserByEmail(string email)
        {
            User.All item = await UsersRepository.GetUserByEmail(email);
            if (item != null && item.User.Online==true)
            {
                if (this.SearchUsers != null)
                {
                    this.SearchUsers.Clear();
                }

                this.SearchUsers = new List<User.All>();
                this.SearchUsers.Add(item);

            }
            else
            {
                
                this.SearchUsers = await UsersRepository.GetUsersOnline();
               
               
            }
            if (this.SearchUsers != null)
            {
                this.SearchUsers = this.SearchUsers.Where(r => r.User.ID != MainViewVM.USER.ID).ToList<User.All>();

                if (this.SearchUsers.Count == 0)
                {
                    this.Error = "Geen gebruikers gevonden met dit email";
                }
                else
                {
                    this.Error = null;
                }
            }
            
           
            RaiseAll();
        }

        public List<User.All> SearchUsers { get; set; }
        public User.All SelectedUser { get; set; }
        private async void AddFriend()
        {
            //todo: friends api
            User.All user = this.SelectedUser;

            List<Friend.All> friends = await FriendsRepository.GetFriends();

            if (friends != null)
            {

                var items= friends.Where(r => r.User2.ID==user.User.ID).ToList();
                if(items!=null && items.Count > 0)
                {
                    
                    Messenger.Default.Send<Dialog>(new Dialog()
                    {
                        Message = "U hebt al deze vriend.",
                        Ok = "Ok"
                    });
                }
                else
                {
                    Libraries.Socket socketSend = new Libraries.Socket() { From = MainViewVM.USER.ID, To = user.User.ID, Status = true };

                    MainViewVM.socket.Emit("friend_REQUEST:send", JsonConvert.SerializeObject(socketSend));
                    CloseModal();

                    Messenger.Default.Send<Dialog>(new Dialog()
                    {
                        Message = "Vrienddschapsverzoek is verzonden",
                        Ok = "Ok"
                    });
                }
            }

            
        }



        #endregion



        //bind events
        public void Changed(object sender, SelectionChangedEventArgs e)
        {
            ListView item = (ListView)sender;

            AddFriend();

        }

        public void SearchChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var value = args.SelectedItem as Friend.All;
            this.SearchItem = value.User2.ToString();
            Search();
        }

    


    }
}
