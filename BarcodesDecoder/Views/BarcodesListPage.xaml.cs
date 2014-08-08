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
using System.Windows.Data;

namespace BarcodesDecoder.Views
{
    public partial class BarcodesListPage : PhoneApplicationPage
    {
        public BarcodesListPage()
        {
            InitializeComponent();

            this.DataContext = new BarcodesListViewModel();

            ProgressIndicator prog;
            SystemTray.SetIsVisible(this, true);
            SystemTray.SetOpacity(this, 0);

            prog = new ProgressIndicator();
            prog.IsVisible = true;
            prog.IsIndeterminate = true;

            SystemTray.SetProgressIndicator(this, prog);
            var task = ((BarcodesListViewModel)this.DataContext).Init();
            task.ContinueWith(t =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    SystemTray.ProgressIndicator.IsVisible = false;
                }));
            });
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            // Update the binding source
            BindingExpression bindingExpr = textBox.GetBindingExpression(TextBox.TextProperty);
            bindingExpr.UpdateSource();
        }

        private void listCountries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var item = (KeyValuePair<string, string>)e.AddedItems[0];
                MessageBox.Show(String.Format("{0}: {1}", item.Key, item.Value));
            }
            catch (Exception ex)
            {

            }
        }
    }
}