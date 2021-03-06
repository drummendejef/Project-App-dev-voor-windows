﻿using Libraries;
using Libraries.Models;
using Libraries.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation;
using Windows.Storage;

namespace BobWorker
{
    public sealed class Service : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {

           


            try
            {
                if (BackgroundWorkCost.CurrentBackgroundWorkCost == BackgroundWorkCostValue.High)
                {
                    return;
                }


                BackgroundTaskDeferral deferal = taskInstance.GetDeferral();

                string json = await readStringFromLocalFile("user.json");
                var definition = new { Email = "", Password = "" };
                var user = JsonConvert.DeserializeAnonymousType(json, definition);

                Location location = await LocationService.GetCurrent();

                Response res = await LoginRepository.Login(user.Email, user.Password);
                if (res.Success == true)
                {
                    if (location != null)
                    {
                        Response ok = await UserRepository.PostLocation(location);
                        Debug.WriteLine(ok);
                    }
                }

                deferal.Complete();

            }
            catch (Exception ex)
            {

              
            }


           

           

           

           


         




        }




        //pasted methods


        async Task<string> readStringFromLocalFile(string filename)
        {
            string text;

            // create a file with the given filename in the local folder; replace any existing file with the same name
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;

            // write the char array created from the content string into the file
            Stream stream = await local.OpenStreamForReadAsync(filename);
            using (StreamReader reader = new StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }

            return text;
        }

    }
}
