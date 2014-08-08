using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BarcodesDecoder.ViewModels;
using ZXing;

namespace BarcodesDecoder.Views
{
    public partial class BarcodeInfoPage : PhoneApplicationPage
    {
        public BarcodeInfoPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var result = (Result)((App)App.Current).ParamsStack.Pop();
            this.DataContext = new BarcodeInfoViewModel(result);
        }
    }
}