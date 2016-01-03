using BOBApp.Messages;
using BOBApp.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Libraries.Models;
using Libraries.Repositories;
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
                        RaisePropertyChanged("Parties");
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
        List<Party> parties_all = new List<Party>();
       
        public async Task GetParties()
        {

            parties_all = await PartyRepository.GetParties();


            int aantalfeestjes = parties_all.Count;// na het ophalen van de feestjes, het aantal feestjes tellen (NOG DOEN)
            for (int i = 0; i < aantalfeestjes; i++)//Alle feestjes overlopen en markers zetten.
            {
                //Kan weg gelaten worden door feestjes[i] op een van de onderstaande lijnen te zetten (optimalisatie?) maar dit is misschien duidelijker. Keuzes keuzes
                Party feest = parties_all[i];

                //Tijdelijke locatie aanmaken
                BasicGeoposition tempbasic = new BasicGeoposition();

                //Feestlocatie opsplitsen (word opgeslagen als string)
                string[] splittedcoord = feest.Location.Split(',', ':', '}');//Splitsen op } zodat de lon proper is

                //Locaties omzetten en in de tijdelijke posities opslaan.
                tempbasic.Latitude = double.Parse(splittedcoord[1].ToString());
                tempbasic.Longitude = double.Parse(splittedcoord[3].ToString());

                //Omzetten van tijdelijk punt naar echte locatie (anders krijg je die niet in de mapIconFeestLocation.Location)
                Geopoint temppoint = new Geopoint(tempbasic);

                MapIcon mapIconFeestLocation = new MapIcon();
                mapIconFeestLocation.Location = temppoint; //Opgehaalde locatie
                //mapIconFeestLocation.Title = feest.Name; //Naam van het feestje;
                mapIconFeestLocation.Image = MainViewVM.Pins.FeestPin;
                this.Map.MapElements.Add(mapIconFeestLocation);//Marker op de map zetten.


                feest.Click = new RelayCommand<object>(e => mapItemButton_Click(e));
            }
           

            this.Parties = parties_all;

            RaiseAll();

        }


        private void Search()
        {
            if (SearchItem == null)
            {
                return;
            }

            string item = this.SearchItem.ToString().Trim().ToLower();

            var newItems = parties_all.Where(r => r.Name.ToString().Trim().ToLower() == item).ToList();
            





            if (newItems != null && newItems.Count > 0)
            {
                this.Parties = newItems;
                this.Error = null;
            }
            else
            {
                this.Error = "Geen feestjes gevonden";
            }

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

        public async void mapItemButton_Click(object param)
        {
            if ((App.Current as App).UserLocation != null)
            {

                Party party = param as Party;
                //Locatie uit gekozen feestje halen.
                string locatie = party.Location;

                //Tijdelijke locatie aanmaken
                BasicGeoposition tempbasic = new BasicGeoposition();

                //Feestlocatie opsplitsen (word opgeslagen als string)
                string[] splittedcoord = party.Location.Split(',', ':', '}');//Splitsen op } zodat de lon proper is

                //Locaties omzetten en in de tijdelijke posities opslaan.
                tempbasic.Latitude = double.Parse(splittedcoord[1].ToString());
                tempbasic.Longitude = double.Parse(splittedcoord[3].ToString());

                //Om de route aan te vragen, heb je een start en een eindpunt nodig. Die moeten er zo uit zien: "waypoint.1=47.610,-122.107".
                //We gaan deze zelf aanmaken.
                /*string startstring = "http://dev.virtualearth.net/REST/v1/Routes?wayPoint.1=";//Eerste deel van de url
                startstring += (App.Current as App).UserLocation.Coordinate.Point.Position.Latitude.ToString() + "," + (App.Current as App).UserLocation.Coordinate.Point.Position.Longitude.ToString();
                startstring += "&waypoint.2=";//Start van het eindpunt
                startstring += tempbasic.Latitude.ToString() + "," + tempbasic.Longitude.ToString();//Endpoint
                startstring += URL.URLBINGKEY + URL.BINGKEY;*/

                //Start en eindpunt ophalen en klaarzetten voor onderstaande vraag
                Geopoint startpunt = (App.Current as App).UserLocation.Coordinate.Point;
                Geopoint eindpunt = new Geopoint(tempbasic);

                //De route tussen 2 punten opvragen
                MapRouteFinderResult routeResult = await MapRouteFinder.GetDrivingRouteAsync(startpunt, eindpunt);

                if (routeResult.Status == MapRouteFinderStatus.Success)//Het is gelukt, we hebben een antwoord gekregen.
                {
                    MapRouteView viewOfRoute = new MapRouteView(routeResult.Route);
                    viewOfRoute.RouteColor = Color.FromArgb(255, 62, 94, 148);

                    //MapRouteView toevoegen aan de Route Collectie
                    this.Map.Routes.Add(viewOfRoute);

                    //Fit de mapcontrol op de route
                    await this.Map.TrySetViewBoundsAsync(routeResult.Route.BoundingBox, null, Windows.UI.Xaml.Controls.Maps.MapAnimationKind.Bow);
                }
            }
        }

        #endregion

    }
}
