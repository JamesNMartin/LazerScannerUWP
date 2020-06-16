using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LazerScannerUWP
{
    class Globals
    {
        public static string uid;
        //public static bool perishable;
        public static int API_CALL_COUNT = 0;
        public static int UNIQUE_INVENTORY_ITEM_COUNT = 0;
        public static int SHOPPING_LIST_ITEM_COUNT = 0;
        public static int INVENTORY_ITEM_COUNT = 0;
        public static bool MOVE_TO_SHOPPING_LIST_ON_DELETE = false;

        public static string API_TEST_URL = "https://api.upcitemdb.com/prod/trial/lookup?upc=";
        public static string SQL_DATA_CONNECTION = "Data Source=tcp:[IP ADDRESS];Initial Catalog=[DATABASE];Persist Security Info=False;User ID=[USER ID];Password=[USER PASSWORD]";
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            // Use system back button to navigate back between content pages.
            SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;
            ContentFrame.Navigated += ContentFrame_Navigated;

            // Use custom title bar.
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            
            UpdateTitleBarLayout(coreTitleBar);
            //Window.Current.SetTitleBar(this.AppTitleBar);

            coreTitleBar.LayoutMetricsChanged += (s, a) => UpdateTitleBarLayout(s);
            
            Loaded += MainPage_Loaded;

        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            this.UpdateAppBackButton();
        }

        private void UpdateAppBackButton()
        {
            var backButtonVisibility = ContentFrame.CanGoBack ?
                                       AppViewBackButtonVisibility.Visible :
                                       AppViewBackButtonVisibility.Collapsed;

            var backgroundVisibility = backButtonVisibility == AppViewBackButtonVisibility.Visible ?
                                       Visibility.Visible :
                                       Visibility.Collapsed;

            var snm = SystemNavigationManager.GetForCurrentView();
            snm.AppViewBackButtonVisibility = backButtonVisibility;

            //BackButtonBackground.Visibility = backgroundVisibility;
        }

        private void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (ContentFrame.CanGoBack)
            {
                // Since back navigations may contain connected animations, suppress the default
                // animation transition.
                ContentFrame.GoBack(new SuppressNavigationTransitionInfo());
                e.Handled = true;
            }
        }

        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        {
            //// Get the size of the caption controls area and back button 
            //// (returned in logical pixels), and move your content around as necessary.
            var isVisible = SystemNavigationManager
                                .GetForCurrentView()
                                .AppViewBackButtonVisibility == AppViewBackButtonVisibility.Disabled;

            var width = isVisible ? coreTitleBar.SystemOverlayLeftInset : 0;

            //LeftPaddingColumn.Width = new GridLength(width);

            // Update title bar control size as needed to account for system size changes.
            //this.AppTitleBar.Height = coreTitleBar.Height;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.MainContent.SelectedItem = this.MainContent.MenuItems.First();
            ContentFrame.Navigate(typeof(WelcomePage));
        }

        private void OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            var item = args.SelectedItem as NavigationViewItem;
            var tag = item.Tag.ToString();
            //Console.WriteLine("TAG: " + tag);

            if (tag.ToLowerInvariant().Equals("home"))
            {
                ContentFrame.Navigate(typeof(WelcomePage));
                MainContent.Header = "Home";
                //ContentFrame.BackStack.Clear();
                //this.UpdateAppBackButton();
                return;
            }
            if (tag.ToLowerInvariant().Equals("add_item"))
            {
                ContentFrame.Navigate(typeof(AddItemPage));
                MainContent.Header = "Add Items";
                //ContentFrame.BackStack.Clear();
                //this.UpdateAppBackButton();
                return;
            }
            if (tag.ToLowerInvariant().Equals("view_item"))
            {
                ContentFrame.Navigate(typeof(ViewItemPage));
                MainContent.Header = "Inventory";
                //ContentFrame.BackStack.Clear();
                //this.UpdateAppBackButton();
                return;
            }
            if (tag.ToLowerInvariant().Equals("shopping_list"))
            {
                ContentFrame.Navigate(typeof(ShoppingListPage));
                MainContent.Header = "Shopping List";
                //ContentFrame.BackStack.Clear();
                //this.UpdateAppBackButton();
                return;
            }
            if (tag.ToLowerInvariant().Equals("statistics"))
            {
                ContentFrame.Navigate(typeof(StatisticsPage));
                MainContent.Header = "Statistics";
                //ContentFrame.BackStack.Clear();
                //this.UpdateAppBackButton();
                return;
            }
            //ContentFrame.Navigate(typeof(StatisticsPage), tag);
        }

        private void MainContent_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {

            if (args.IsSettingsInvoked)
            {
                // Code here
                ContentFrame.Navigate(typeof(SettingsPage));
                MainContent.Header = "Settings";
            }
            else
            {
                string navTo = args.InvokedItemContainer.Tag.ToString();

                if (navTo != null)
                {
                    switch (navTo)
                    {
                        case "home":
                            ContentFrame.Navigate(typeof(WelcomePage));
                            MainContent.Header = "Welcome";
                            break;

                        case "add_item":
                            ContentFrame.Navigate(typeof(AddItemPage));
                            MainContent.Header = "Add Items";
                            break;

                        case "view_item":
                            ContentFrame.Navigate(typeof(ViewItemPage));
                            MainContent.Header = "Inventory";
                            break;

                        case "shopping_list":
                            ContentFrame.Navigate(typeof(ShoppingListPage));
                            MainContent.Header = "Shopping List";
                            break;

                        case "statistics":
                            ContentFrame.Navigate(typeof(StatisticsPage));
                            MainContent.Header = "Statistics";
                            break;
                    }
                }
            }

        }

        private void MainContent_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {

        }
    }
}

