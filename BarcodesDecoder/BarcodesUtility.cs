using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO;
using Newtonsoft.Json;

namespace BarcodesDecoder
{
    public class BarcodesUtility
    {
        public Dictionary<string, string> BarcodesDb { get; set; }
        private bool isInitialized = false;

        private async Task Init()
        {
            string filepath = @"Assets\barcodes.json";
            StorageFolder folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFile file = await folder.GetFileAsync(filepath); // error here
            using (var stream = await file.OpenStreamForReadAsync())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    var barcodesJson = await reader.ReadToEndAsync();
                    this.BarcodesDb = await JsonConvert.DeserializeObjectAsync<Dictionary<string, string>>(barcodesJson);
                }
            }
            isInitialized = true;
        }

        public async Task<string> GetCountry(string code)
        {
            if (!this.isInitialized)
                await this.Init();

            try
            {
                return this.BarcodesDb[code];
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
