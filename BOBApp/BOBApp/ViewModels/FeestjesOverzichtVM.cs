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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;

namespace BOBApp.ViewModels
{
    public class FeestjesOverzichtVM : ViewModelBase
    {
        #region props
        public bool Loading { get; set; }
        public string Error { get; set; }
        public MapControl Map { get; set; }


        public RelayCommand SearchCommand { get; set; }
        //Dingen die te maken hebben met de kaart
        RandomAccessStreamReference PartyIcon;

        public List<Party> Parties { get; set; }
        public List<Bob.All> Bobs { get; set; }
        public List<User.All> Users { get; set; }

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
                    if (parties_all != null)
                    {
                        this.Parties = parties_all;
                        ClearAllMapItems();

                        RaiseAll();
                    }
                }
            }
        }


        #endregion

        //Constructor
        public FeestjesOverzichtVM()
        {
            this.PartyIcon = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/feestpin.png"));
            Messenger.Default.Register<NavigateTo>(typeof(bool), ExecuteNavigatedTo);

            SearchItemCommand = new RelayCommand(Search);

            RaiseAll();
        }

        private void ExecuteNavigatedTo(NavigateTo obj)
        {
            if (obj.Name == "loaded")
            {
                Type view = (Type)obj.View;
                if (view == (typeof(FeestjesOverzicht)))
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
                
             
#pragma warning disable CS1998
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    await GetParties();
                    this.Loading = false;
                    RaiseAll();

                });
#pragma warning restore CS1998

            });
        }


        private async void RaiseAll()
        {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                RaisePropertyChanged("Loading");
                RaisePropertyChanged("Error");
                RaisePropertyChanged("Parties");

                RaisePropertyChanged("SearchItem");
            });
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        }

        //Methods
     

        private void Search()
        {
            if (SearchItem == null)
            {
                return;
            }
            ClearAllMapItems();


            string item = this.SearchItem.ToString().Trim().ToLower();

            var newItems = parties_all.Where(r => r.Name.ToString().Trim().ToLower() == item).ToList();



            foreach (var party in newItems)
            {
                party.VisibleShow = Visibility.Visible;

            }


            if (newItems != null && newItems.Count > 0)
            {
                this.Parties = newItems;
                this.Error = null;
            }
            else
            {
                this.Error = "Geen feestjes gevonden";
            }

            UpdateMapOfType(typeof(List<Party>));
            RaiseAll();
        }

        #region binds events

        public void SearchChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var value = args.SelectedItem as Party;
            this.SearchItem = value.Name;
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
        List<Party> parties_all = new List<Party>();
        private async Task GetParties()
        {

            parties_all = await PartyRepository.GetParties();

            if (parties_all != null)
            {
                for (int i = 0; i < parties_all.Count; i++)
                {
                    try
                    {
                        Party item = parties_all[i];
                        if (item.Location == null)
                        {
                            break;
                        }
                        item.VisibleShow = Visibility.Collapsed;
                        item.ShowCommand = new RelayCommand<object>(e => ShowParty(e));
                        item.RouteCommand = new RelayCommand<object>(e => mapItem_Party(e));
                        item.TakeCommand = new RelayCommand<object>(e => TakeParty(e));
                        item.RouteCommandText = "Toon route";
                        if (MainViewVM.USER.IsBob.Value)
                        {
                            item.VisibleTake = Visibility.Collapsed;
                        }
                        else
                        {
                            item.VisibleTake = Visibility.Visible;
                        }

                        BasicGeoposition tempbasic = new BasicGeoposition();
                        //Locaties omzetten en in de tijdelijke posities opslaan.
                        tempbasic.Latitude = ((Location)item.Location).Latitude;
                        tempbasic.Longitude = ((Location)item.Location).Longitude;

                        //Omzetten van tijdelijk punt naar echte locatie (anders krijg je die niet in de mapIconFeestLocation.Location)
                        Geopoint temppoint = new Geopoint(tempbasic);

                        MapIcon mapIconFeestLocation = new MapIcon();
                        mapIconFeestLocation.Location = temppoint; //Opgehaalde locatie
                                                                   //mapIconFeestLocation.Title = feest.Name; //Naam van het feestje;
                        mapIconFeestLocation.Image = MainViewVM.Pins.FeestPin;
                        mapIconFeestLocation.Title = item.Name;
                        this.Map.MapElements.Add(mapIconFeestLocation);//Marker op de map zetten.

                    }
                    catch (Exception ex)
                    {


                    }

                }


                try
                {
                    var newControl = new MapItemsControl();
                    FeestjesOverzicht view = MainViewVM.MainFrame.Content as FeestjesOverzicht;

                    newControl.ItemsSource = parties_all;
                    newControl.ItemTemplate = (DataTemplate)view.Resources["PartiesMapTemplate"] as DataTemplate;

                    AddOrUpdateChild(newControl);
                }
                catch (Exception)
                {

                  
                }

                this.Parties = parties_all.Take(10).ToList();
                RaiseAll();
            }





        }

        private void ShowParty(object obj)
        {
            var item = obj as Party;
            if (item.VisibleShow == Visibility.Collapsed)
            {
                item.VisibleShow = Visibility.Visible;

            }
            else
            {
                item.VisibleShow = Visibility.Collapsed;

            }
            UpdateMapOfType(typeof(List<Party>));

            RaiseAll();

        }
        private void TakeParty(object obj)
        {
            var item = obj as Party;

            if (MainViewVM.USER.IsBob == false)
            {
                VindRitFilterVM.SelectedParty = item.Name;
                VindRitVM.SelectedParty = item;

                MainViewVM.MainFrame.Navigate(typeof(VindRit), true);
            }

            RaiseAll();

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
                        if (newControl.ItemsSource.GetType() == typeof(List<Party>))
                        {
                            control.ItemsSource = null;
                            control.ItemsSource = this.Parties;
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

     
        private void ClearAllMapItems()
        {
            try
            {
                foreach (var item in this.Parties)
                {
                    item.VisibleShow = Visibility.Collapsed;
                }
               
                RaiseAll();

                foreach (var itemChild in this.Map.Children)
                {
                    var control = itemChild as MapItemsControl;
                    if (control.ItemsSource != null)
                    {
                        if (control.ItemsSource.GetType() == typeof(List<Party>))
                        {
                            control.ItemsSource = null;
                            control.ItemsSource = this.Parties;
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
                        if (type == typeof(List<Party>))
                        {
                            control.ItemsSource = null;
                            control.ItemsSource = this.Parties;
                        }

                       

                    }
                }

            }
        }

        public async void mapItem_Party(object param)
        {
            if ((App.Current as App).UserLocation != null)
            {

                //Locatie uit gekozen feestje halen.
                Party item = param as Party;

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

            UpdateMapOfType(typeof(List<Party>));
        }

       

        

        #endregion

    }
}
