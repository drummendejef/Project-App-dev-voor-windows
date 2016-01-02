using Libraries.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace Libraries
{
    public class LocationService
    {
        public static Geolocator geolocator;

        public static Location LastLocation;
        public static void Start()
        {
            geolocator = new Geolocator();
            geolocator.PositionChanged += Geolocator_PositionChanged;
            geolocator.StatusChanged += Geolocator_StatusChanged;
        }



        public static async Task<Location> GetCurrent()
        {
           

            try
            {
                return await Task.Run(async () =>
                {
                    Location location = LastLocation;
                    //await Geolocator.RequestAccessAsync();

                    geolocator = new Geolocator();

                    geolocator.ReportInterval = 100;
                    geolocator.DesiredAccuracyInMeters = 200;

                    Geoposition pos = Task.FromResult<Geoposition>(await geolocator.GetGeopositionAsync()).Result;
                    location = new Location() { Latitude = pos.Coordinate.Point.Position.Latitude, Longitude = pos.Coordinate.Point.Position.Longitude };
                    LastLocation = location;

                    return location;
                   
                    
                });
            }
            catch (Exception ex)
            {


            }
            return null;





        }

        private static async void Geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            //var location = await geolocator.GetGeopositionAsync();
        }

        private static async void Geolocator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            //var location = await geolocator.GetGeopositionAsync();
        }
    }
}
