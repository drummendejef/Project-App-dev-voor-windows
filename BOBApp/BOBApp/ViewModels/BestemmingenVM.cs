using BOBApp.Messages;
using BOBApp.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Libraries;
using Libraries.Models;
using Libraries.Models.relations;
using Libraries.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Geolocation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace BOBApp.ViewModels
{
    public class BestemmingenVM : ViewModelBase
    {
        #region props
        public bool Loading { get; set; }
        public string Error { get; set; }

        public string SearchLocation { get; set; }
        public RelayCommand GoDestinationCommand { get; set; }
        public RelayCommand AddDestinationCommand { get; set; }
        public RelayCommand GoToCityCommand { get; set; }


        public List<City> Cities { get; set; }
        public List<Users_Destinations> Destinations { get; set; }
        public Users_Destinations SelectedDestination { get; set; }
        public Point CityLocation { get; set; }

        private City _NewCity;

        public City NewCity
        {
            get { return _NewCity; }
            set { _NewCity = value; }
        }

        public Destination NewDestination { get; set; }

        //databinding voor het center van de map
        public Geopoint MapCenter { get; set; }


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
                    if (destinations_all != null)
                    {
                        this.Destinations = destinations_all;
                        RaisePropertyChanged("Destinations");
                    }
                }
            }
        }


        #endregion



        //Constructor
        public BestemmingenVM()
        {
            if ((App.Current as App).UserLocation != null)//Kijken of we de locatie van de gebruiker hebben.
                MapCenter = (App.Current as App).UserLocation.Coordinate.Point;//Zoja, center van de map eerst op de persoon focussen.

            Messenger.Default.Register<NavigateTo>(typeof(bool), ExecuteNavigatedTo);



            AddDestinationCommand = new RelayCommand(AddDestination);
            GoDestinationCommand = new RelayCommand(GoDestination);
            GoToCityCommand = new RelayCommand(TownToCoord);
            SearchItemCommand = new RelayCommand(Search);


            RaiseAll();


        }



        private void ExecuteNavigatedTo(NavigateTo obj)
        {
            if (obj.Name == "loaded")
            {
                Type view = (Type)obj.View;
                if (view == (typeof(Bestemmingen)))
                {
                    Loaded();
                }

                Type view2 = (Type)obj.View;
                if (view2 == (typeof(Bestemmingen_Nieuw)))
                {
                    LoadedNew();
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
                    await GetCities();
                    await GetDestinations();
                    this.NewCity = new City();
                    this.NewDestination = new Destination();

                    this.Loading = false;
                    RaiseAll();

                });
#pragma warning restore CS1998

            });
        }

        private async void LoadedNew()
        {
            this.Loading = true;
            RaisePropertyChanged("Loading");

            await Task.Run(async () =>
            {
                // running in background
                this.NewCity = new City();
                this.NewDestination = new Destination();

#pragma warning disable CS1998
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
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
                RaisePropertyChanged("SearchLocation");
                RaisePropertyChanged("Destination");
                RaisePropertyChanged("Destinations");
                RaisePropertyChanged("MapCenter");
                RaisePropertyChanged("NewDestination");
                RaisePropertyChanged("SearchItem");
                RaisePropertyChanged("NewCity");

            });
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        }

        //Methods
        private void AddDestination()
        {
            Task task = AddDestination_task();
        }

        private async Task AddDestination_task()
        {
            if (this.NewDestination.Name == null || this.NewDestination.Name == "")
            {
                Messenger.Default.Send<Dialog>(new Dialog()
                {
                    Message = "Geen naam ingevuld",
                    Ok = "Ok",
                    Nok = null,
                    ViewOk = null,
                    ViewNok = null,
                    ParamView = false,
                    Cb = null
                });
                return;
            }
            if (this.NewCity == null || this.NewCity.ID == null)
            {
                Messenger.Default.Send<Dialog>(new Dialog()
                {
                    Message = "Geen stad ingevuld",
                    Ok = "Ok",
                    Nok = null,
                    ViewOk = null,
                    ViewNok = null,
                    ParamView = false,
                    Cb = null
                });
                return;

            }
            Destination destination = new Destination()
            {
                Cities_ID = this.NewCity.ID,
                Location = this.NewDestination.Location,
                Name = this.NewDestination.Name

            };
            Response ok = await DestinationRepository.PostDestination(destination);
            if (ok.Success == true)
            {
                Messenger.Default.Send<Dialog>(new Dialog()
                {
                    Message = "Bestemming toegevoegd",
                    Ok = "Ok",
                    Nok = null,
                    ViewOk = typeof(Bestemmingen),
                    ViewNok = null,
                    ParamView = false,
                    Cb = null
                });
            }
        }

        private void GoDestination()
        {
            Frame rootFrame = MainViewVM.MainFrame as Frame;
            rootFrame.Navigate(typeof(Bestemmingen_Nieuw), true);
        }

        List<Users_Destinations> destinations_all = new List<Users_Destinations>();
        private async Task GetDestinations()
        {
            this.Loading = true;
            RaisePropertyChanged("Loading");

            destinations_all = await DestinationRepository.GetDestinations();

            var count = destinations_all.Count;
            if (destinations_all.Count >= 10)
            {
                count = 10;
            }

            for (int i = 0; i < count; i++)
            {
                if (destinations_all[i].Default == false)
                {
                    destinations_all[i].VisibleDefault = Visibility.Visible;
                    destinations_all[i].SetDefault = new RelayCommand<object>(SetDefault);
                }
                else
                {
                    destinations_all[i].VisibleDefault = Visibility.Collapsed;
                }

                destinations_all[i].Remove = new RelayCommand<object>(Remove);


            }
            this.Destinations = destinations_all;
            if (this.Destinations == null || this.Destinations.Count == 0)
            {
                this.Error = "Geen bestemmingen gevonden";
            }
            this.Loading = false;
            RaiseAll();
        }

        private async void Remove(object id)
        {
            int DestinationsID = int.Parse(id.ToString());
            Response response = Task.FromResult<Response>(await DestinationRepository.RemoveDestination(DestinationsID)).Result;

            if (response.Success == true)
            {
                Messenger.Default.Send<Dialog>(new Dialog()
                {
                    Message = "Bestemming verwijderd",
                    Ok = "Ok",
                    Nok = null,
                    ViewOk = null,
                    ViewNok = null,
                    ParamView = false,
                    Cb = null
                });
                Loaded();
            }
            else
            {
                //error
            }
        }

        private async void SetDefault(object id)
        {
            int DestinationsID = int.Parse(id.ToString());

            Response response = Task.FromResult<Response>(await DestinationRepository.PostDefaultDestination(DestinationsID)).Result;

            if (response.Success == true)
            {
                Messenger.Default.Send<Dialog>(new Dialog()
                {
                    Message = "Nieuwe standaard loctie ingesteld",
                    Ok = "Ok",
                    Nok = null,
                    ViewOk = null,
                    ViewNok = null,
                    ParamView = false,
                    Cb = null
                });
                Loaded();
            }
            else
            {
                //error
            }
        }

        private async Task GetCities()
        {
            this.Loading = true;
            RaisePropertyChanged("Loading");

            this.Cities = await CityRepository.GetCities();
            this.Cities.Sort((a, b) => a.Name.CompareTo(b.Name));

            //this.NewCity = this.Cities.Where(r => r.Name.ToLower() == "kortrijk").First();

            this.Loading = false;
            RaisePropertyChanged("Loading");

            RaiseAll();
        }

        //De naam van de gemeente omzetten naar de locatie
        private async void TownToCoord()
        {
            //Naam ophalen uit de geselecteerde stad.
            string Town = NewCity.Name;

            //Volledige string maken
            string URLFullTownToCoord = URL.BASISURLTOWNTOCOORD + Town + URL.URLBINGKEY + URL.BINGKEY + "&maxRes=1";//Volledige request url aanmaken, maar 1 resultaat terugvragen.

            using (HttpClient client = new HttpClient())
            {
                var result = client.GetAsync(URLFullTownToCoord);
                string json = await result.Result.Content.ReadAsStringAsync();
                var root = JsonConvert.DeserializeObject<TownToCoordinates.RootObject>(json);

                foreach (var rs in root.resourceSets)//In de Json resourcessets gaan
                {
                    foreach (var r in rs.resources)//Alle resources overlopen (maar 1, door onze vraag)
                    {
                        Debug.WriteLine("Geselecteerde stadsnaam: " + NewCity.Name + ", ontvangen stadsnaam: " + r.name);
                        Debug.WriteLine("Coordinaten ontvangen stad: " + r.point.coordinates[0] + ", " + r.point.coordinates[1]);
                        Debug.WriteLine("Coordinaten persoon: " + MapCenter.Position.Latitude + ", " + MapCenter.Position.Longitude);

                        //Coordinaten aanmaken
                        Geopoint temppoint = new Geopoint(new BasicGeoposition() { Latitude = r.point.coordinates[0], Longitude = r.point.coordinates[1] });

                        MapCenter = temppoint;
                        RaiseAll();//Gebruiken zodat de mapcenter meteen veranderd
                    }
                }
            }

            // https://msdn.microsoft.com/en-us/library/ff701714.aspx 
        }

        //Locatie van nieuwe destinatie opslaan
        public void setDestinationLocation(string location)
        {
            this.NewDestination.Location = location;
        }

        private void Search()
        {
            if (SearchItem == null)
            {
                return;
            }

            string item = this.SearchItem.Trim().ToLower();
            var newItems = destinations_all.Where(r => r.Name.Trim().ToLower() == item).ToList();

            if (newItems != null && newItems.Count > 0)
            {
                this.Destinations = newItems;
                this.Error = null;
            }
            else
            {
                this.Error = "Geen bestemmingen gevonden";
            }



            RaiseAll();

        }

        //bind events
        public async void Changed(object sender, SelectionChangedEventArgs e)
        {
            ListView item = (ListView)sender;
            if (item.SelectedIndex == -1)
            {
                return;
            }


            var dialog = new ContentDialog()
            {
                Title = "",
            };

            // Setup Content
            var panel = new StackPanel();

            panel.Children.Add(new TextBlock
            {
                Text = "Volgende bestemming wilt u wijzigen: " + this.SelectedDestination.Name,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 15)
            });

            var cb = new TextBox
            {
                TextWrapping = TextWrapping.Wrap,
                HorizontalContentAlignment = HorizontalAlignment.Stretch
            };



            panel.Children.Add(cb);
            dialog.Content = panel;

            // Add Buttons
            dialog.PrimaryButtonText = "Ok";
            dialog.PrimaryButtonClick += async delegate
            {
                string text = cb.Text;
                Response res = await DestinationRepository.PutDestinationName(this.SelectedDestination.Destinations_ID, text);
                Loaded();
            };

            dialog.SecondaryButtonText = "Cancel";
            dialog.SecondaryButtonClick += delegate
            {

            };

            // Show Dialog
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.None)
            {

            }
            item.SelectedIndex = -1;


        }


        public void SearchChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {

            var value = args.SelectedItem as Users_Destinations;

            this.SearchItem = value.Name;
            Search();
        }

    }
}
