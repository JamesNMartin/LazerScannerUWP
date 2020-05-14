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
using System.Data.SqlClient;
using System.Data;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace LazerScannerUWP.Views
{
    public sealed partial class ItemContentDialog : ContentDialog
    {
        private int updatedQuan = 0;
        private long ean = 0;





        public ItemContentDialog(string theTitle, string theBrand, string theDescription, int theQuantity, string theCategory, DateTime theDate, long theEan)
        {
            InitializeComponent();
            //IsPrimaryButtonEnabled = false;
            Title = theBrand;
            titleTextBlock.Text = theTitle;
            descriptionTextBlock.Text = theDescription;
            //this.quantityTextBlock.Text = "You have " + theQuantity + " on hand.";
            quantityTextBlock.Text = theQuantity.ToString();
            categoryTextBlock.Text = theCategory;
            scanDateLabel.Text = "Date first scanned: " + theDate.ToShortDateString();
            ean = theEan;

        }
        static public async void Show(Item theItem)
        {
            long ean = theItem.ean;
            string brand = theItem.brand;
            string description = theItem.description;
            string title = theItem.title;
            int quantity = theItem.quantity;
            string category = theItem.category;
            DateTime date = theItem.scandate;

            ItemContentDialog dialog = new ItemContentDialog(title, brand, description, quantity, category, date, ean);
            await dialog.ShowAsync();
        }

        private static Image ImageSource(string imageurl)
        {
            throw new NotImplementedException();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //UPDATE
            updatedQuan = int.Parse(quantityTextBlock.Text);
            using (SqlConnection myConnection = new SqlConnection(Globals.SQL_DATA_CONNECTION))
            {
                SqlCommand cmd = new SqlCommand("updateQuan", myConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@upcInput", ean));
                cmd.Parameters.Add(new SqlParameter("@theQuan", updatedQuan));
                myConnection.Open();
                int rowAffected = cmd.ExecuteNonQuery();
                if (rowAffected == 1)
                {
                    myConnection.Close();
                }
                else
                {
                    myConnection.Close();
                }
            }
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

        private void MinusButton_Click(object sender, RoutedEventArgs e)
        {
            updatedQuan = int.Parse(quantityTextBlock.Text);
            if (updatedQuan == 0)
            {
                quantityTextBlock.Text = "" + updatedQuan;
            }
            else
            {
                updatedQuan--;
                quantityTextBlock.Text = "" + updatedQuan;
            }

        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            updatedQuan = int.Parse(quantityTextBlock.Text);
            if (updatedQuan == 0)
            {
                quantityTextBlock.Text = "" + updatedQuan;
            }
            else
            {
                updatedQuan++;
                quantityTextBlock.Text = "" + updatedQuan;

            }
        }
    }
}
