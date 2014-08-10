using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;

namespace BarcodesDecoder.ViewModels
{
    public class BarcodeInfoViewModel : INotifyPropertyChanged
    {
        private Result barcode = null;
        public Result Barcode
        {
            get
            {
                return this.barcode;
            }
            set
            {
                this.barcode = value;
                this.OnPropertyChanged();
                this.DecodeInfo();
            }
        }

        public BarcodeInfoViewModel(Result barcode)
        {
            this.Barcode = barcode;
        }

        private async Task DecodeInfo()
        {
            var barcodesUtility = new BarcodesUtility();
            if (this.Barcode.BarcodeFormat == BarcodeFormat.EAN_13)
            {
                var regitratorCode = this.Barcode.Text.Substring(0, 3);
                this.CountryOfOrigin = await barcodesUtility.GetCountry(regitratorCode);
                return;
            }
            this.CountryOfOrigin = BarcodesDecoder.Resources.AppResources.BarcodeFormatNotSupportedText;
        }

        private string countryOfOrigin;

        public string CountryOfOrigin
        {
            get { return countryOfOrigin; }
            set
            {
                countryOfOrigin = value;
                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
