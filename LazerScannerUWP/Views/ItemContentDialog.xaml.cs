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
        private static Item itemCopy;

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
            scanDateLabel.Text = "Scan Date: " + theDate.ToShortDateString();
            ean = theEan;

        }

        internal static void Show(ShoppingListItem item)
        {
            throw new NotImplementedException();
        }

        static public async void Show(Item theItem)
        {

            itemCopy = theItem;

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
                cmd.Parameters.Add(new SqlParameter("@userID", Globals.uid));
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
            Globals.SHOPPING_LIST_ITEM_COUNT = GenerateDashboard.GetShoppingListCount(Globals.uid);
            Globals.UNIQUE_INVENTORY_ITEM_COUNT = GenerateDashboard.GetUniqueItemCount(Globals.uid);
            Globals.INVENTORY_ITEM_COUNT = GenerateDashboard.GetInventoryListCount(Globals.uid);
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //DELETE
            if (Globals.MOVE_TO_SHOPPING_LIST_ON_DELETE)
            {
                using (SqlConnection myConnection = new SqlConnection(Globals.SQL_DATA_CONNECTION))
                {
                    SqlCommand cmd = new SqlCommand("sendToShoppingList", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.Add(new SqlParameter("@userID", Globals.uid));
                    cmd.Parameters.Add(new SqlParameter("@ean",itemCopy.ean));
                    cmd.Parameters.Add(new SqlParameter("@title",itemCopy.title));
                    cmd.Parameters.Add(new SqlParameter("@upc",itemCopy.upc));
                    cmd.Parameters.Add(new SqlParameter("@brand",itemCopy.brand));
                    cmd.Parameters.Add(new SqlParameter("@model",itemCopy.model));
                    cmd.Parameters.Add(new SqlParameter("@category",itemCopy.category));
                    cmd.Parameters.Add(new SqlParameter("@imageurl",itemCopy.imageurl));
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
                Globals.SHOPPING_LIST_ITEM_COUNT = GenerateDashboard.GetShoppingListCount(Globals.uid);
                Globals.UNIQUE_INVENTORY_ITEM_COUNT = GenerateDashboard.GetUniqueItemCount(Globals.uid);
                Globals.INVENTORY_ITEM_COUNT = GenerateDashboard.GetInventoryListCount(Globals.uid);

            }
            else
            {
                using (SqlConnection myConnection = new SqlConnection(Globals.SQL_DATA_CONNECTION))
                {
                    SqlCommand cmd = new SqlCommand("deleteItem", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.Add(new SqlParameter("@upcInput", ean));
                    cmd.Parameters.Add(new SqlParameter("@userID", Globals.uid));
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
                Globals.UNIQUE_INVENTORY_ITEM_COUNT = GenerateDashboard.GetUniqueItemCount(Globals.uid);
                Globals.SHOPPING_LIST_ITEM_COUNT = GenerateDashboard.GetShoppingListCount(Globals.uid);
                Globals.INVENTORY_ITEM_COUNT = GenerateDashboard.GetInventoryListCount(Globals.uid);
            }
        }
        private void ContentDialog_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //CLOSE
            
        }
        private void MinusButton_Click(object sender, RoutedEventArgs e)
        {
            updatedQuan = int.Parse(quantityTextBlock.Text);
            if (updatedQuan == 1)
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

            updatedQuan++;
            quantityTextBlock.Text = "" + updatedQuan;

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Globals.MOVE_TO_SHOPPING_LIST_ON_DELETE = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Globals.MOVE_TO_SHOPPING_LIST_ON_DELETE = false;
        }
    }
}
