﻿using System;
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

            var stack = ((App)App.Current).ParamsStack;
            if (stack.Count > 0)
            {
                var result = (Result)((App)App.Current).ParamsStack.Pop();
                this.DataContext = new BarcodeInfoViewModel(result);
            }
            else
            {
                MessageBox.Show(BarcodesDecoder.Resources.AppResources.BarcodeInfoDataIsNullErrorMessage);
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    NavigationService.Navigate(new Uri("/Views/CapturePage.xaml", UriKind.Relative));
                }));
            }
        }

        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                TextBlock textBlock = (TextBlock)sender;
                MessageBox.Show(textBlock.Text);
            }
            catch { }
        }
    }
}