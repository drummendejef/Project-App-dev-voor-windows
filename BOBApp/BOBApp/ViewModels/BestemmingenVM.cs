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
    public class BestemmingenVM: ViewModelBase
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
            set { _NewCity = value;}
        }

        public Destination NewDestination { get; set; }

        //databinding voor het center van de map
        public Geopoint MapCenter { get; set; }

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
                GetCities();
                GetDestinations();
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

        private void RaiseAll()
        {
            RaisePropertyChanged("Loading");
            RaisePropertyChanged("Error");
            RaisePropertyChanged("SearchLocation");
            RaisePropertyChanged("Destination");
            RaisePropertyChanged("Destinations");
            RaisePropertyChanged("MapCenter");
            RaisePropertyChanged("NewDestination");
        }

        //Methods
        private void AddDestination()
        {
           Task task = AddDestination_task(); 
        }

        private async Task AddDestination_task()
        {
            Destination destination = new Destination()
            {
                Cities_ID = this.NewCity.ID,
                Location = this.NewDestination.Location

            };
            Response ok = await DestinationRepository.PostDestination(destination);
        }

        private void GoDestination()
        {
            Frame rootFrame =MainViewVM.MainFrame as Frame;
            rootFrame.Navigate(typeof(Bestemmingen_Nieuw),true);
        }
        private async void GetDestinations()
        {
            this.Destinations = await DestinationRepository.GetDestinations();

            for (int i = 0; i < this.Destinations.Count; i++)
            {
                this.Destinations[i].SetDefault = new RelayCommand<object>(SetDefault);
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
            }
            else
            {
                //error
            }
        }

        private async void GetCities()
        {
            this.Cities = await CityRepository.GetCities();
            this.Cities.Sort((a, b) => a.Name.CompareTo(b.Name));

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

                foreach(var rs in root.resourceSets)//In de Json resourcessets gaan
                {
                    foreach(var r in rs.resources)//Alle resources overlopen (maar 1, door onze vraag)
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
                Margin=new Thickness(0,0,0,15)
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
    }
}
