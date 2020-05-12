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
using LazerScannerUWP.Models;
using LazerScannerUWP.Views;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace LazerScannerUWP.Views
{
    public sealed partial class ItemContentDialog : ContentDialog
    {
        public ItemContentDialog(string theTitle, string theBrand, string theDescription, int theQuantity, string theCategory)
        {
            this.InitializeComponent();
            this.Title = theBrand;
            this.titleTextBlock.Text = theTitle;
            this.descriptionTextBlock.Text = theDescription;
            //this.quantityTextBlock.Text = "You have " + theQuantity + " on hand.";
            this.quantityTextBlock.Text = theQuantity.ToString();
            this.categoryTextBlock.Text = theCategory;
            
        }
        static public async void Show(Item theItem)
        {

            string brand = theItem.brand;
            string description = theItem.description;
            string title = theItem.title;
            int quantity = theItem.quantity;
            string category = theItem.category;

            ItemContentDialog dialog = new ItemContentDialog(title,brand,description, quantity, category);
            await dialog.ShowAsync();
        }

        private static Image ImageSource(string imageurl)
        {
            throw new NotImplementedException();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //UPDATE
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //DELETE
        }

        private void ContentDialog_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //CLOSE
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            //EDIT SWITCH TOGGLED
            
        }
    }
}
