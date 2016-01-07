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
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.UI;
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
                        ClearAllMapItems();

                        RaiseAll();
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
                User.All user = JsonConvert.DeserializeObject<User.All>(obj.Data.ToString());
                bool accepted = (bool)obj.Result;
                Response response = Task.FromResult<Response>(await FriendsRepository.PostFriend( user.User.ID,MainViewVM.USER.ID, accepted)).Result;

                if (response.Success == true)
                {
                    Messenger.Default.Send<Dialog>(new Dialog()
                    {
                        Message = user.User.ToString() + " is toegevoegd",
                        ViewOk=typeof(ZoekVrienden)
                    });

                    Libraries.Socket socketSend = new Libraries.Socket() { To = user.User.ID, From = MainViewVM.USER.ID, Status = true };

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

            ClearAllMapItems();

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

            foreach (var friend in newItems)
            {
                friend.VisibleShow = Visibility.Visible;

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

            UpdateMapOfType(typeof(List<Friend.All>));
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





        #region add friend


        private async void FindUserByEmail(string email)
        {
            User.All item = await UsersRepository.GetUserByEmail(email);
            if (item != null && item.User.Online == true)
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

                var items = friends.Where(r => r.User2.ID == user.User.ID).ToList();
                if (items != null && items.Count > 0)
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


        #region maps

        List<Friend.All> friends_all = new List<Friend.All>();
        private async Task GetFriends()
        {
            this.Loading = true;
            RaisePropertyChanged("Loading");

            friends_all = await FriendsRepository.GetFriends();

            //Try catch errond om te zorgen dat hij niet crasht bij lege bobs.
            if (this.friends_all != null)
            {
                for (int i = 0; i < friends_all.Count(); i++)
                {
                    try
                    {
                        Friend.All item = friends_all[i];
                        if (item.Location == null)
                        {
                            break;
                        }

                        item.VisibleShow = Visibility.Collapsed;
                        item.ShowCommand = new RelayCommand<object>(e => ShowFriend(e));
                        item.RouteCommand = new RelayCommand<object>(e => mapItem_Friend(e));
                        item.RouteCommandText = "Toon route";

                        BasicGeoposition tempbasic = new BasicGeoposition();
                        //Locaties omzetten en in de tijdelijke posities opslaan.
                        tempbasic.Latitude = ((Location)item.Location).Latitude;
                        tempbasic.Longitude = ((Location)item.Location).Longitude;

                        //Omzetten van tijdelijk punt naar echte locatie (anders krijg je die niet in de mapIconFeestLocation.Location)
                        Geopoint temppoint = new Geopoint(tempbasic);

                        MapIcon mapIconFeestLocation = new MapIcon();
                        mapIconFeestLocation.Location = temppoint; //Opgehaalde locatie
                                                                   //mapIconFeestLocation.Title = feest.Name; //Naam van het feestje;
                        mapIconFeestLocation.Image = MainViewVM.Pins.UserPin;
                        mapIconFeestLocation.Title = item.User1.ToString();
                        this.Map.MapElements.Add(mapIconFeestLocation);//Marker op de map zetten.
                    }
                    catch (Exception ex)
                    {


                    }

                }

                try
                {
                    var newControl = new MapItemsControl();
                    ZoekVrienden vindrit = MainViewVM.MainFrame.Content as ZoekVrienden;

                    newControl.ItemsSource = friends_all;
                    newControl.ItemTemplate = (DataTemplate)vindrit.Resources["FriendsMapTemplate"] as DataTemplate;



                    AddOrUpdateChild(newControl);
                }
                catch (Exception)
                {


                }




                this.Friends = friends_all.Take(10).ToList();
                this.Loading = false;
                RaiseAll();


            }


        }

        private void AddOrUpdateChild(MapItemsControl newControl)
        {
            bool done = false;
            foreach (var itemChild in this.Map.Children)
            {
                var control = itemChild as MapItemsControl;
                if (control.ItemsSource != null)
                {
                    if (control.ItemsSource.GetType() == newControl.ItemsSource.GetType())
                    {
                        if (newControl.ItemsSource.GetType() == typeof(List<Friend.All>))
                        {
                            control.ItemsSource = null;
                            control.ItemsSource = this.Friends;
                            done = true;
                        }



                    }

                }

            }

            if (done == false)
            {
                this.Map.Children.Add(newControl);
            }

        }

        private void ShowFriend(object obj)
        {
            var item = obj as Friend.All;
            if (item.VisibleShow == Visibility.Collapsed)
            {
                item.VisibleShow = Visibility.Visible;

            }
            else
            {
                item.VisibleShow = Visibility.Collapsed;

            }
            UpdateMapOfType(typeof(List<Friend.All>));

            RaiseAll();

        }
        private void ClearAllMapItems()
        {
            try
            {
                foreach (var item in this.Friends)
                {
                    item.VisibleShow = Visibility.Collapsed;
                }

                RaiseAll();

                foreach (var itemChild in this.Map.Children)
                {
                    var control = itemChild as MapItemsControl;
                    if (control.ItemsSource != null)
                    {
                        if (control.ItemsSource.GetType() == typeof(List<Friend.All>))
                        {
                            control.ItemsSource = null;
                            control.ItemsSource = this.Friends;
                        }
                    }

                }
            }
            catch (Exception ex)
            {


            }

        }
        private void UpdateMapOfType(Type type)
        {
            foreach (var itemChild in this.Map.Children)
            {
                var control = itemChild as MapItemsControl;
                if (control.ItemsSource != null)
                {
                    if (control.ItemsSource.GetType() == type)
                    {
                        if (type == typeof(List<Friend.All>))
                        {
                            control.ItemsSource = null;
                            control.ItemsSource = this.Friends;
                        }



                    }
                }

            }
        }



        public async void mapItem_Friend(object param)
        {
            if ((App.Current as App).UserLocation != null)
            {

                //Locatie uit gekozen feestje halen.
                User.All item = param as User.All;

                //Tijdelijke locatie aanmaken
                try
                {
                    BasicGeoposition tempbasic = new BasicGeoposition();

                    //Feestlocatie opsplitsen (word opgeslagen als string)
                    Location location = (Location)item.Location;

                    //Locaties omzetten en in de tijdelijke posities opslaan.
                    tempbasic.Latitude = location.Latitude;
                    tempbasic.Longitude = location.Longitude;

                    //Om de route aan te vragen, heb je een start en een eindpunt nodig. Die moeten er zo uit zien: "waypoint.1=47.610,-122.107".
                    //We gaan deze zelf aanmaken.
                    /*string startstring = "http://dev.virtualearth.net/REST/v1/Routes?wayPoint.1=";//Eerste deel van de url
                    startstring += (App.Current as App).UserLocation.Coordinate.Point.Position.Latitude.ToString() + "," + (App.Current as App).UserLocation.Coordinate.Point.Position.Longitude.ToString();
                    startstring += "&waypoint.2=";//Start van het eindpunt
                    startstring += tempbasic.Latitude.ToString() + "," + tempbasic.Longitude.ToString();//Endpoint
                    startstring += URL.URLBINGKEY + URL.BINGKEY;*/

                    Geopoint startpunt;
                    //Start en eindpunt ophalen en klaarzetten voor onderstaande vraag
                    if (VindRitFilterVM.SelectedDestination != null)
                    {
                        BasicGeoposition tempDest = new BasicGeoposition();
                        tempDest.Latitude = ((Location)VindRitFilterVM.SelectedDestination.Location).Latitude;
                        tempDest.Longitude = ((Location)VindRitFilterVM.SelectedDestination.Location).Longitude;
                        startpunt = new Geopoint(tempDest);
                    }
                    else
                    {
                        startpunt = (App.Current as App).UserLocation.Coordinate.Point;
                    }
                    Geopoint eindpunt = new Geopoint(tempbasic);
                    //De route tussen 2 punten opvragen
                    MapRouteFinderResult routeResult = await MapRouteFinder.GetDrivingRouteAsync(startpunt, eindpunt);

                    if (routeResult.Status == MapRouteFinderStatus.Success)//Het is gelukt, we hebben een antwoord gekregen.
                    {
                        MapRouteView viewOfRoute = new MapRouteView(routeResult.Route);
                        viewOfRoute.RouteColor = Color.FromArgb(255, 62, 94, 148);

                        if (item.RouteCommandText == "Toon route")
                        {
                            this.Map.Routes.Clear();
                        }

                        var items = this.Map.Routes;
                        if (items.Count() > 0)
                        {
                            item.RouteCommandText = "Toon route";
                            this.Map.Routes.Clear();
                        }
                        else
                        {
                            item.RouteCommandText = "Clear route";
                            this.Map.Routes.Add(viewOfRoute);
                        }

                        //MapRouteView toevoegen aan de Route Collectie


                        //Fit de mapcontrol op de route
                        await this.Map.TrySetViewBoundsAsync(routeResult.Route.BoundingBox, null, Windows.UI.Xaml.Controls.Maps.MapAnimationKind.Bow);
                    }
                }
                catch (Exception ex)
                {


                }
            }

            UpdateMapOfType(typeof(List<User.All>));
        }



        #endregion

    }
}
