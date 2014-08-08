using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodesDecoder.ViewModels
{
    public class BarcodesListViewModel : INotifyPropertyChanged
    {
        private BarcodesUtility utility;
        public async Task Init()
        {
            this.utility = new BarcodesUtility();
            await utility.Init();
            this.BarcodesDb = utility.BarcodesDb;
            this.PropertyChanged += BarcodesListViewModel_PropertyChanged;
        }

        private void BarcodesListViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SearchText")
            {
                int dummy;
                if (Int32.TryParse(this.SearchText, out dummy))
                {
                    this.BarcodesDb = this.utility.BarcodesDb.Where(item => item.Key.StartsWith(this.SearchText)).ToDictionary(item => item.Key, item => item.Value);
                }
                else
                {
                    this.BarcodesDb = this.utility.BarcodesDb.Where(item => item.Value.StartsWith(this.SearchText)).ToDictionary(item => item.Key, item => item.Value);
                }
            }
        }

        private Dictionary<string, string> barcodesDb;

        public Dictionary<string, string> BarcodesDb
        {
            get { return barcodesDb; }
            set
            {
                barcodesDb = value;
                OnPropertyChanged();
            }
        }

        private string searchText;

        public string SearchText
        {
            get { return searchText; }
            set
            {
                searchText = value;
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
