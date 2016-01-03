﻿using BOBApp.Messages;
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
using Windows.UI.Xaml.Controls.Maps;

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
        public MapControl Map { get; set; }


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



        private async void RaiseAll()
        {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                RaisePropertyChanged("VisibleModal");
                RaisePropertyChanged("SearchUsers");
                RaisePropertyChanged("Loading");
                RaisePropertyChanged("Error");
                RaisePropertyChanged("Friends");

                RaisePropertyChanged("SearchItem");

            });
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
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

        #region bind event

        public void SearchChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var value = args.SelectedItem as Friend.All;
            this.SearchItem = value.User2.ToString();
            Search();
        }

        public void MapLoaded(object sender, RoutedEventArgs e)//Als de map geladen is.
        {
            if ((App.Current as App).UserLocation != null)
            {
                //Map centreren op huidige locatie
                this.Map.Center = (App.Current as App).UserLocation.Coordinate.Point;//De userpoint ophalen, en de map hier op centreren.
                this.Map.ZoomLevel = 15;//Inzoomlevel instellen (hoe groter het getal, hoe dichterbij)
                this.Map.LandmarksVisible = true;

                //Marker voor eigen locatie plaatsen
                MapIcon mapIconUserLocation = new MapIcon();
                mapIconUserLocation.Location = this.Map.Center;
                mapIconUserLocation.NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1.0);//Verzet het icoontje, zodat de punt van de marker staat op waar de locatie is. (anders zou de linkerbovenhoek op de locatie staan) 
                mapIconUserLocation.Title = "Ik";//Titel die boven de marker komt.
                mapIconUserLocation.Image = MainViewVM.Pins.UserPin;
                this.Map.MapElements.Add(mapIconUserLocation);//Marker op de map zetten.
            }



        }

        #endregion




    }
}
