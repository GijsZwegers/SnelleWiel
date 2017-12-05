using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SnelleWiel.Classes;
using System.Threading;
using Windows.UI.Popups;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SnelleWiel
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
        private async void BtInloggen_ClickAsync(object sender, RoutedEventArgs e)
        {
            var test = await UrlCalls.GetUser(tbUser.Text, pbPass.Password);
            if (test.success == true)
            {
                this.Frame.Navigate(typeof(Pages.OptionPage));
            }
            else
            {
                tbUser.Text = "";
                pbPass.Password = "";
                MessageBox("De inloggegevens zijn incorrect");
            }
        }

       
        private void TbUser_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbUser.Text != "")
            {
                BtInloggen.IsEnabled = true;
            }
            if (tbUser.Text == "")
            {
                BtInloggen.IsEnabled = false;
            }
        }

        private async void MessageBox(string message)
        {
            var dialog = new MessageDialog(message.ToString());
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await dialog.ShowAsync());
        }
    }

}