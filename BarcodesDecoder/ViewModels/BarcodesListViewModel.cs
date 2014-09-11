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

        public List<BarcodeDisplayGroup> BarcodeGroups { get; set; }
        public List<BarcodeDisplayGroup> BarcodeGroupsView { get; set; }
        public async Task Init()
        {
            this.utility = new BarcodesUtility();
            await utility.Init();

            this.BarcodeGroups = new List<BarcodeDisplayGroup>();
            string currentManufacturer = null;
            foreach (var bcInfo in utility.BarcodesDb)
            {
                if (bcInfo.Value != currentManufacturer)
                {
                    this.BarcodeGroups.Add(new BarcodeDisplayGroup
                    {
                        Name = bcInfo.Value,
                        Codes = new List<string>()
                    });
                    this.BarcodeGroups.Last().Codes.Add(bcInfo.Key);
                    currentManufacturer = bcInfo.Value;
                }
                else
                {
                    this.BarcodeGroups.Last().Codes.Add(bcInfo.Key);
                }
            }
            this.BarcodeGroupsView = this.BarcodeGroups.ToList();
            this.OnPropertyChanged("BarcodeGroupsView");

            this.PropertyChanged += BarcodesListViewModel_PropertyChanged;
        }

        private void BarcodesListViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SearchText")
            {
                int dummy;
                if (Int32.TryParse(this.SearchText, out dummy))
                {
                    this.BarcodeGroupsView = this.BarcodeGroups.Where(item => item.Codes.Where(code => code.StartsWith(this.SearchText)).Count() > 0).ToList();
                }
                else
                {
                    this.BarcodeGroupsView = this.BarcodeGroups.Where(item => item.Name.ToLowerInvariant().StartsWith(this.SearchText.ToLowerInvariant())).ToList();
                }
                this.OnPropertyChanged("BarcodeGroupsView");
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

    public class BarcodeDisplayGroup
    {
        public string Name { get; set; }
        public List<string> Codes { get; set; }
        public string CodesString
        {
            get
            {
                int[] codesInt = new int[this.Codes.Count];
                for (int i = 0; i < this.Codes.Count; i++ )
                {
                    try
                    {
                        codesInt[i] = Int32.Parse(this.Codes[i]);
                    }
                    catch { }
                }
                int min = codesInt.Min();
                int max = codesInt.Max();
                if (min != max)
                    return String.Format("{0} - {1}", min, max);
                else
                    return min.ToString();
            }
        }
    }
}
