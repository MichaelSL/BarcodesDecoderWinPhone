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
            ((BarcodesListViewModel)this.DataContext).Init();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            // Update the binding source
            BindingExpression bindingExpr = textBox.GetBindingExpression(TextBox.TextProperty);
            bindingExpr.UpdateSource();
        }
    }
}