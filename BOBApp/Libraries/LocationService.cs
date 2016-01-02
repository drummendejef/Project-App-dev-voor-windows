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
        private static Location LastLocation;
        public static async Task<Location> GetCurrent()
        {
           
            try
            {
                await Task.Run(async () =>
                {
                    Geolocator geolocator = new Geolocator();
                    Geoposition pos = await geolocator.GetGeopositionAsync();
                    Location location = new Location() { Latitude = pos.Coordinate.Point.Position.Latitude, Longitude = pos.Coordinate.Point.Position.Longitude };

                    return location;
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                if (LastLocation == null)
                {
                    await GetCurrent();
                }
                else
                {
                    return LastLocation;
                }
               
            }

            return LastLocation;
                }
            
        }
    }
}
