using BOBApp.Messages;
using BOBApp.Views;
using GalaSoft.MvvmLight;
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

namespace BOBApp.ViewModels
{
    public class FeestjesOverzichtVM : ViewModelBase
    {
        #region props
        public bool Loading { get; set; }
        public string Error { get; set; }

        public string ZoekVeld { get; set; }

        //Dingen die te maken hebben met de kaart
        RandomAccessStreamReference PartyIcon;

        public List<Party> Feestjes { get; set; }
        //De te binden dingen
        /* public string NaamFeest { get; set; }
         public string OrganisatorFeest { get; set; }
         public Geopoint GeoLocation { get; set; }*/

        #endregion

        //Constructor
        public FeestjesOverzichtVM()
        {
            this.PartyIcon = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/feestpin.png"));
            Messenger.Default.Register<NavigateTo>(typeof(bool), ExecuteNavigatedTo);

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
                GetFeestjes();
             
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
            RaisePropertyChanged("Feetjes");
        }

        //Methods
        public async void GetFeestjes()
        {
           
            this.Feestjes = new List<Party>();

            //Ingeladen feestjes ophalen
            List<Party> omtezettenfeestjes = await PartyRepository.GetParties();

            int aantalfeestjes = omtezettenfeestjes.Count();
            for(int i = 0; i < aantalfeestjes; i++)
            {
                Party feest = omtezettenfeestjes[i];

                //Feestlocatie opsplitsen (word opgeslagen als string)
                //string[] splittedcoord = feest.Location.Split(',', ':', '}');//Splitsen op } zodat de lon proper is

                //Naam
               /* NaamFeest = feest.Name;
                //Organisatior
                OrganisatorFeest = feest.Organisator;
                //Location
                GeoLocation = new Geopoint(new BasicGeoposition() { Latitude = double.Parse(splittedcoord[1].ToString()), Longitude = double.Parse(splittedcoord[3].ToString()) });*/

                Feestjes.Add(feest);
            }

        }
    }
}
