﻿using LazerScannerUWP.Models;
using LazerScannerUWP.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace LazerScannerUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShoppingListPage : Page
    {
        private List<ShoppingListItem> Items;


        public ShoppingListPage()
        {
            InitializeComponent();
            Items = ShoppingListItemManager.GetShoppingItemList(Globals.uid);
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = (ShoppingListItem)e.ClickedItem;
            
            //ItemContentDialog.Show(item);
        }
        private void RefreshController_RefreshRequested(RefreshContainer sender, RefreshRequestedEventArgs args)
        {
            using (var RefreshCompletionDeferral = args.GetDeferral())
            {
                // Do some async operation to refresh the content
                GridView.ItemsSource = null;
                GridView.ItemsSource = ShoppingListItemManager.GetShoppingItemList(Globals.uid);
                RefreshCompletionDeferral.Complete();
                RefreshCompletionDeferral.Dispose();
            }
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            refreshController.RequestRefresh();
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            refreshController.RequestRefresh();
        }

        private void moveToInvButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
