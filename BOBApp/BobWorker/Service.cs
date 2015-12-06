using Libraries.Models;
using Libraries.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;

namespace BobWorker
{
    public sealed class Service : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {

            if (BackgroundWorkCost.CurrentBackgroundWorkCost == BackgroundWorkCostValue.High)
            {
                return;
            }

            BackgroundTaskDeferral deferal = taskInstance.GetDeferral();

            Location location = new Location() { Latitude = 50, Longitude = 13 };

            Response ok = await UserRepository.PostLocation(location);

            Debug.WriteLine(ok);

            //var json = JsonConvert.SerializeObject(resul);
            //await saveStringToLocalFile("parking.json", json);


            deferal.Complete();




        }




        //pasted methods


        async Task saveStringToLocalFile(string filename, string content)
        {
            // saves the string 'content' to a file 'filename' in the app's local storage folder
            byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(content.ToCharArray());

            // create a file with the given filename in the local folder; replace any existing file with the same name
            StorageFile file = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

            // write the char array created from the content string into the file
            using (var stream = await file.OpenStreamForWriteAsync())
            {
                stream.Write(fileBytes, 0, fileBytes.Length);
            }
        }



    }
}
