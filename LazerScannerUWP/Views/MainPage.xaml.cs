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
    static class Globals
    {
        public static string uid;
        public static bool perishable;

        private static string API_TEST_URL = "https://api.upcitemdb.com/prod/trial/lookup?upc=";
        public static string SQL_DATA_CONNECTION = "Data Source=tcp:73.118.249.57;Initial Catalog=LazerScanner;Persist Security Info=False;User ID=sa;Password=nothingtoseehere";
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            // Use system back button to navigate back between content pages.
            SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;
            this.ContentFrame.Navigated += ContentFrame_Navigated;

            // Use custom title bar.
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;

            this.UpdateTitleBarLayout(coreTitleBar);
            //Window.Current.SetTitleBar(this.AppTitleBar);

            coreTitleBar.LayoutMetricsChanged += (s, a) => UpdateTitleBarLayout(s);

            this.Loaded += MainPage_Loaded;
        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            this.UpdateAppBackButton();
        }

        private void UpdateAppBackButton()
        {
            var backButtonVisibility = this.ContentFrame.CanGoBack ?
                                       AppViewBackButtonVisibility.Visible :
                                       AppViewBackButtonVisibility.Collapsed;

            var backgroundVisibility = backButtonVisibility == AppViewBackButtonVisibility.Visible ?
                                       Visibility.Visible :
                                       Visibility.Collapsed;

            var snm = SystemNavigationManager.GetForCurrentView();
            snm.AppViewBackButtonVisibility = backButtonVisibility;

            //this.BackButtonBackground.Visibility = backgroundVisibility;
        }

        private void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (this.ContentFrame.CanGoBack)
            {
                // Since back navigations may contain connected animations, suppress the default
                // animation transition.
                this.ContentFrame.GoBack(new SuppressNavigationTransitionInfo());
                e.Handled = true;
            }
        }

        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        {
            //// Get the size of the caption controls area and back button 
            //// (returned in logical pixels), and move your content around as necessary.
            var isVisible = SystemNavigationManager
                                .GetForCurrentView()
                                .AppViewBackButtonVisibility == AppViewBackButtonVisibility.Visible;

            var width = isVisible ? coreTitleBar.SystemOverlayLeftInset : 0;

            //LeftPaddingColumn.Width = new GridLength(width);

            // Update title bar control size as needed to account for system size changes.
            //this.AppTitleBar.Height = coreTitleBar.Height;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.MainContent.SelectedItem = this.MainContent.MenuItems.First();
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
            ContentFrame.Navigate(typeof(StatisticsPage), tag);
        }
    }
}

