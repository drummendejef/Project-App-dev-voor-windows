using BOBApp.Views;
using GalaSoft.MvvmLight;
using Libraries.Models;
using Libraries.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Storage.Streams;

namespace BOBApp.ViewModels
{
    public class FeestjesOverzichtVM : ViewModelBase
    {
        //Properties

        public string ZoekVeld { get; set; }

        //Dingen die te maken hebben met de kaart
        RandomAccessStreamReference PartyIcon = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/feestpin.png"));

        public List<Party> Feestjes { get; set; }
        //De te binden dingen
       /* public string NaamFeest { get; set; }
        public string OrganisatorFeest { get; set; }
        public Geopoint GeoLocation { get; set; }*/


        //Constructor
        public FeestjesOverzichtVM()
        {
            
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
