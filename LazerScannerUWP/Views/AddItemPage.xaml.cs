using ColorCode.Compilation.Languages;
using LazerScannerUWP.Models;
using Microsoft.Toolkit.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.ApplicationModel.Chat;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace LazerScannerUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddItemPage : Page
    {
        private int XRateLimitRemaining = 0;
        private long CURRENT_ITEM_UPC = 0;
        private List<StoredItem> StoredItemsList = new List<StoredItem>();
        private int itemQuantity = 1;

        public AddItemPage()
        {
            InitializeComponent();
            scanDatePicker.SelectedDate = DateTime.Now;
        }

        public dynamic CheckAPIForItemData(long upc)
        {
            string html = string.Empty;
            string url = Globals.API_TEST_URL + upc;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
                XRateLimitRemaining = int.Parse(response.GetResponseHeader("X-RateLimit-Remaining"));
            }

            return JsonConvert.DeserializeObject(html);
        }
        public bool CheckSQLServerForDataToIncreaseQuantity(long ean)
        {
            using (SqlConnection myConnection = new SqlConnection(Globals.SQL_DATA_CONNECTION))
            {
                SqlCommand cmd = new SqlCommand("increaseQuan", myConnection)
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
                    return true;
                }
                else
                {
                    myConnection.Close();
                    return false;
                }
            }
        }
        public void IsWebRequestValid(dynamic requestResponse)
        {
            //var requestResponseCode = requestResponse.code;
            if (requestResponse.code == "INVALID_UPC")//TODO Handle all the codes
            {
                //Console.WriteLine("Not found on UPCItemDB. Set item aside, you'll have to manually enter data later.\n");
                requestsRemaining.Text = "Invalid UPC. If this persists, please add item manually.";
            }
            else if (requestResponse.code == "TOO_FAST")
            {
                requestsRemaining.Text = "Scanning too fast. Try again in ~30sec.";
                
                StoredItem storedItem = new StoredItem(CURRENT_ITEM_UPC);
                StoredItemsList.Add(storedItem);



            }
            else if (requestResponse.code == "OK" && requestResponse.total == 0)
            {
                //Console.WriteLine("Not found on UPCItemDB. Set item aside, you'll have to manually enter data later.\n");
                requestsRemaining.Text = "Item not found. Item must be added manually.";
            }
            else
            {
                long webEan = requestResponse.items[0].ean;
                string webTitle = requestResponse.items[0].title;
                long webUpc = requestResponse.items[0].upc;
                string webDesc = requestResponse.items[0].description;
                string webBrand = requestResponse.items[0].brand;
                string webModel = requestResponse.items[0].model;
                string webWeight = requestResponse.items[0].weight;
                string webCategory = requestResponse.items[0].category; //TODO #1 go through this string and pull out the category


                string imageURL = requestResponse.items[0].images[0];//TODO Some items do not have an image. (returns null?) Need to handle this
                                                                     //TEST WITH 20LB BAG OF SAFEWAY RICE

                // FORMATTING ALL THE TEXT TO HAVE DOUBLE SINGLE QUOTE SO SQL QUERY IS NOT ENDED IN THE WRONG PLACE
                string formattedPurchaseGroup = SQLArguementFormatter(recieptbarcodeInput.Text);
                string formattedTitle = SQLArguementFormatter(webTitle);
                itemName.Text = formattedTitle;
                string formattedDesc = SQLArguementFormatter(webDesc);
                descriptionTextbox.Text = formattedDesc;
                string formattedBrand = SQLArguementFormatter(webBrand);
                itemBrand.Text = formattedBrand;
                string formattedModel = SQLArguementFormatter(webModel);
                itemModel.Text = formattedModel;
                string formattedWeight = SQLArguementFormatter(webWeight);
                itemWeight.Text = formattedWeight;
                string formattedCategory = SQLArguementFormatter(webCategory);
                itemCategory.Text = formattedCategory; //TODO Refer to #1
                itemImageURL.Text = imageURL;

                //NUMBER OF SCANS REMAINING FROM UPCITEMDB API 100/day
                //MUST SCAN ITEM BEFORE IT WILL DISPLAY. COUNT COMES FROM HEADER.
                Globals.API_CALL_COUNT = int.Parse(XRateLimitRemaining.ToString());
                requestsRemaining.Text = "Number of scans remaining: " + XRateLimitRemaining.ToString();

                DateTime scannedDate = DateTime.Today;
                int quanNeeded = 1;

                //THIS IS TRULY PEAK GARBAGE CODE AND CODE PLACEMENT. YOU GOTTA MOVE THIS IF YOU EVER WANT THIS TO BE READABLE TO FUTURE YOU.
                /*
                 * @userId VARCHAR(11),
				   @purchaseGroup NVARCHAR(255),
				   @ean bigint,
				   @title NVARCHAR(255),
				   @upc bigint,
			       @description nvarchar(max),
				   @brand nvarchar(255),
				   @model nvarchar(255),
				   @weight nvarchar(255),
				   @category nvarchar(255),
				   @quantity int,
				   @scandate date,
				   @imageurl nvarchar(max)
                 * 
                 */
                if (auto_switch.IsOn)
                {
                    using (SqlConnection myConnection = new SqlConnection(Globals.SQL_DATA_CONNECTION))
                    {
                        SqlCommand cmd = new SqlCommand("insertData", myConnection)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                        cmd.Parameters.Add(new SqlParameter("@userId", Globals.uid));
                        cmd.Parameters.Add(new SqlParameter("@purchaseGroup", formattedPurchaseGroup));
                        cmd.Parameters.Add(new SqlParameter("@ean", webEan));
                        cmd.Parameters.Add(new SqlParameter("@title", formattedTitle));
                        cmd.Parameters.Add(new SqlParameter("@upc", webUpc));
                        cmd.Parameters.Add(new SqlParameter("@description", formattedDesc));
                        cmd.Parameters.Add(new SqlParameter("@brand", formattedBrand));
                        cmd.Parameters.Add(new SqlParameter("@model", formattedModel));
                        cmd.Parameters.Add(new SqlParameter("@weight", webWeight));
                        cmd.Parameters.Add(new SqlParameter("@category", formattedCategory));
                        cmd.Parameters.Add(new SqlParameter("@quantity", quanNeeded));
                        cmd.Parameters.Add(new SqlParameter("@scandate", scannedDate));
                        cmd.Parameters.Add(new SqlParameter("@imageurl", imageURL));

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
                if (!auto_switch.IsOn)
                {
                    //DO NOTHING? 
                    //I MEAN ALL THE TEXT BOXES HAVE THE TEXT IF ITS FOUND, IF NOT ENTER IT YOURSELF. 

                }
            }
        }
        public string SQLArguementFormatter(string input)
        {
            return input.Replace("'", "''");
        }
        private void LookupButton_Click(object sender, RoutedEventArgs e)
        {
            CURRENT_ITEM_UPC = long.Parse(barcodeInput.Text);
            bool SQLItemCheck = CheckSQLServerForDataToIncreaseQuantity(CURRENT_ITEM_UPC);
            if (SQLItemCheck == true)
            {
                requestsRemaining.Text = "Increased item count by 1.";
                Thread.Sleep(1000);
                requestsRemaining.Text = "Number of scans remaining: " + XRateLimitRemaining.ToString();
                
            }
            else
            {
                //BEFORE ASKING UPCITEMDB, I WANT TO CHECK IF I HAD ALREADY MADE A COPY OF THE ITEM.
                //THIS WAY I DON'T WASTE MY API CALL LIMIT OF 100



                IsWebRequestValid(CheckAPIForItemData(CURRENT_ITEM_UPC)); //WHY DID I DO THIS TO MYSELF, FUCKIN HELL!
            }
        }

        private string CheckSQLServerForItemData(long theEAN)
        {

            using (SqlConnection myConnection = new SqlConnection(Globals.SQL_DATA_CONNECTION))
            {
                string oString = $"SELECT (select * from StoredItems WHERE ean = '{theEAN}' FOR JSON PATH, ROOT('ItemInfo'))";
                SqlCommand oCmd = new SqlCommand(oString, myConnection);
                myConnection.Open();
                using (SqlDataReader oReader = oCmd.ExecuteReader())
                {
                    while (oReader.Read())
                    {
                        var dbnull = oReader.GetValue(0);
                        if (dbnull == DBNull.Value)
                        {
                            return null;
                        }
                        else
                        {
                            return (string)oReader.GetValue(0);
                        }
                    }
                }
                return null;
            }
        }
        private void RecieptbarcodeInput_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key.ToString() == "Enter")
            {
                //outputLabel.Text = "Looking up " + nameInput.Text;
                recieptbarcodeInput.IsEnabled = false;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            //descriptionTextbox.Text = JSONFromServer();
            using (SqlConnection myConnection = new SqlConnection(Globals.SQL_DATA_CONNECTION))
            {
                SqlCommand cmd = new SqlCommand("insertData", myConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@userId", Globals.uid));
                cmd.Parameters.Add(new SqlParameter("@purchaseGroup", SQLArguementFormatter(recieptbarcodeInput.Text)));
                cmd.Parameters.Add(new SqlParameter("@ean", barcodeInput.Text));
                cmd.Parameters.Add(new SqlParameter("@title", SQLArguementFormatter(itemName.Text)));
                cmd.Parameters.Add(new SqlParameter("@upc", barcodeInput.Text));
                cmd.Parameters.Add(new SqlParameter("@description", SQLArguementFormatter(descriptionTextbox.Text)));
                cmd.Parameters.Add(new SqlParameter("@brand", SQLArguementFormatter(itemBrand.Text)));
                cmd.Parameters.Add(new SqlParameter("@model", SQLArguementFormatter(itemModel.Text)));
                cmd.Parameters.Add(new SqlParameter("@weight", SQLArguementFormatter(itemWeight.Text)));
                cmd.Parameters.Add(new SqlParameter("@category", SQLArguementFormatter(itemCategory.Text)));
                cmd.Parameters.Add(new SqlParameter("@quantity", itemQuantity));
                cmd.Parameters.Add(new SqlParameter("@scandate", scanDatePicker.SelectedDate));
                cmd.Parameters.Add(new SqlParameter("@imageurl", itemImageURL.Text));

                myConnection.Open();
                int rowAffected = cmd.ExecuteNonQuery();
                if (rowAffected == 1)
                {
                    //EMPTY THE TEXT FIELD
                    CleanTextFields();
                    //MAKE KEYBOARD ACTIVE FOR NEW INPUT
                    barcodeInput.Focus(FocusState.Keyboard);
                    myConnection.Close();
                }
                else
                {
                    myConnection.Close();
                }
            }


        }
        private void ClearConfirmation_Click(object sender, RoutedEventArgs e)
        {
            CleanTextFields();
        }

        private void barcodeInput_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key.ToString() == "Enter" && auto_switch.IsEnabled)
            {
                LookupButton_Click(sender, e);
            }

        }
        private void CleanTextFields()
        {
            recieptbarcodeInput.IsEnabled = true;
            barcodeInput.Text = string.Empty;
            itemName.Text = string.Empty;
            itemBrand.Text = string.Empty;
            itemModel.Text = string.Empty;
            itemWeight.Text = string.Empty;
            itemCategory.Text = string.Empty;
            itemImageURL.Text = string.Empty;
            descriptionTextbox.Text = string.Empty;

            itemQuantity = 1;
            quantityTextBlock.Text = itemQuantity.ToString();
        }
        private void auto_switch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                if (toggleSwitch.IsOn == true)
                {
                    itemName.IsEnabled = false;
                    itemBrand.IsEnabled = false;
                    itemModel.IsEnabled = false;
                    itemCategory.IsEnabled = false;
                    itemWeight.IsEnabled = false;
                    itemImageURL.IsEnabled = false;
                    scanDatePicker.IsEnabled = false;
                    descriptionTextbox.IsEnabled = false;
                    plusButton.IsEnabled = false;
                    minusButton.IsEnabled = false;

                    addButton.IsEnabled = false;
                    clearButton.IsEnabled = false;

                    //barcodeInput.Focus(FocusState.Keyboard);
                }
                else
                {
                    itemName.IsEnabled = true;
                    itemBrand.IsEnabled = true;
                    itemModel.IsEnabled = true;
                    itemCategory.IsEnabled = true;
                    itemWeight.IsEnabled = true;
                    itemImageURL.IsEnabled = true;
                    scanDatePicker.IsEnabled = true;
                    descriptionTextbox.IsEnabled = true;
                    plusButton.IsEnabled = true;
                    minusButton.IsEnabled = true;

                    addButton.IsEnabled = true;
                    clearButton.IsEnabled = true;
                }
            }
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            barcodeInput.Focus(FocusState.Keyboard);//TODO hmmmmmmm
        }

        private void plusButton_Click(object sender, RoutedEventArgs e)
        {
            itemQuantity = int.Parse(quantityTextBlock.Text);

            itemQuantity++;
            quantityTextBlock.Text = "" + itemQuantity;
        }

        private void minusButton_Click(object sender, RoutedEventArgs e)
        {
            itemQuantity = int.Parse(quantityTextBlock.Text);
            if (itemQuantity == 1)
            {
                quantityTextBlock.Text = "" + itemQuantity;
            }
            else
            {
                itemQuantity--;
                quantityTextBlock.Text = "" + itemQuantity;
            }
        }
    }
}
