using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using LazerScannerUWP.Views;
using System.Threading;

namespace LazerScannerUWP
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WelcomePage : Page
    {
        public WelcomePage()
        {
            InitializeComponent();
            apiCallsRemainingTextBlock.Text = Globals.API_CALL_COUNT.ToString();
            uniqueInventoryCountTextBlock.Text = Globals.UNIQUE_INVENTORY_ITEM_COUNT.ToString();
            inventoryCountTextBlock.Text = Globals.INVENTORY_ITEM_COUNT.ToString();
            shoppingListCountTextBlock.Text = Globals.SHOPPING_LIST_ITEM_COUNT.ToString();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
