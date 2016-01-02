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
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace BOBApp.ViewModels
{
    public class FeestjesOverzichtVM : ViewModelBase
    {
        #region props
        public bool Loading { get; set; }
        public string Error { get; set; }

      

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
           
            this.Parties = new List<Party>();

            //Ingeladen feestjes ophalen
            parties_all = await PartyRepository.GetParties();

            
            for(int i = 0; i < parties_all.Count; i++)
            {
                Party feest = parties_all[i];

                //Feestlocatie opsplitsen (word opgeslagen als string)
                //string[] splittedcoord = feest.Location.Split(',', ':', '}');//Splitsen op } zodat de lon proper is

                //Naam
               /* NaamFeest = feest.Name;
                //Organisatior
                OrganisatorFeest = feest.Organisator;
                //Location
                GeoLocation = new Geopoint(new BasicGeoposition() { Latitude = double.Parse(splittedcoord[1].ToString()), Longitude = double.Parse(splittedcoord[3].ToString()) });*/

                //parties_all.Add(feest);
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

        public void SearchChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var value = args.SelectedItem as Party;
            this.SearchItem = value.Name;
            Search();
        }



    }
}
