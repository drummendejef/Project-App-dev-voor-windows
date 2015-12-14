using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Libraries
{
    public class Localdata
    {
        public static async Task<string> read(string filename)
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

        public static async Task<bool> save(string filename, string content)
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

            return true;
        }
    }
}
