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

namespace BarcodesDecoder.Views
{
    public partial class CapturePage : PhoneApplicationPage
    {
        public CapturePage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            ((CaptureViewModel)DataContext).Orientation = this.Orientation;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ((CaptureViewModel)DataContext).InitializeAndGo();
            ((CaptureViewModel)DataContext).BarcodeScanned += CapturePage_BarcodeScanned;
        }

        private void CapturePage_BarcodeScanned(object sender, BarcodesDecoder.ViewModels.CaptureViewModel.ResultEventArgs e)
        {
            ((App)App.Current).ParamsStack.Push(e.Result);
            Dispatcher.BeginInvoke(new Action(() =>
            {
                NavigationService.Navigate(new Uri("/Views/BarcodeInfoPage.xaml", UriKind.Relative));
            }));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            ((CaptureViewModel)DataContext).BarcodeScanned -= CapturePage_BarcodeScanned;
            ((CaptureViewModel)DataContext).Stop();
        }
    }
}