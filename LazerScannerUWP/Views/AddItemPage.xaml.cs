using LazerScannerUWP.Models;
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
        private const string API_TEST_URL = "https://api.upcitemdb.com/prod/trial/lookup?upc=";
        private string SQL_DATA_CONNECTION = "Data Source=tcp:73.118.249.57;Initial Catalog=LazerScanner;Persist Security Info=False;User ID=sa;Password=nothingtoseehere";


        public AddItemPage()
        {
            InitializeComponent();
            scanDatePicker.SelectedDate = DateTime.Now;
        }

        public dynamic WebLookup(long upc)
        {
            string html = string.Empty;
            string url = API_TEST_URL + upc;

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
            using (SqlConnection myConnection = new SqlConnection(SQL_DATA_CONNECTION))
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
                requestsRemaining.Text = "Number of scans remaining: " + XRateLimitRemaining.ToString();


                DateTime scannedDate = DateTime.Today;
                int quanNeeded = 1;

                //THIS IS TRULY PEAK GARBAGE CODE AND CODE PLACEMENT. YOU GOTTA MOVE THIS IF YOU EVER WANT THIS TO BE READABLE TO FUTURE YOU.
                if (auto_switch.IsOn)
                {
                    using (SqlConnection myConnection = new SqlConnection(SQL_DATA_CONNECTION))
                    {
                        string oString = $"INSERT INTO Items(userId,purchaseGroup,ean,title,upc,description,brand,model,weight,category,quantity,scandate,imageurl)VALUES('{Globals.uid}','{formattedPurchaseGroup}','{webEan}','{formattedTitle}','{webUpc}','{formattedDesc}','{formattedBrand}','{formattedModel}','{formattedWeight}','{formattedCategory}','{quanNeeded}','{scannedDate}','{imageURL}')";
                        SqlCommand oCmd = new SqlCommand(oString, myConnection);
                        myConnection.Open();
                        using (SqlDataReader oReader = oCmd.ExecuteReader())
                        {
                            //myConnection.Close();
                        }
                        CleanTextFields();
                        barcodeInput.Focus(FocusState.Programmatic);
                        //Console.WriteLine("Item added to SQL server");
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
            long input = long.Parse(barcodeInput.Text);
            bool SQLItemCheck = CheckSQLServerForDataToIncreaseQuantity(input);
            if (SQLItemCheck == true)
            {
                requestsRemaining.Text = "Increased item count by 1.";
                Thread.Sleep(1000);
                requestsRemaining.Text = "Number of scans remaining: " + XRateLimitRemaining.ToString();
                //Console.WriteLine("Found data on SQL server. Increased quantity by +1\n");
            }
            else
            {
                //BEFORE ASKING UPCITEMDB, I WANT TO CHECK IF I HAD ALREADY MADE A COPY OF THE ITEM.
                //THIS WAY I DONT WASTE MY API CALL LIMIT OF 100


                IsWebRequestValid(WebLookup(input)); //WHY DID I DO THIS TO MYSELF!?!?!?
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
            descriptionTextbox.Text = string.Empty;
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
                    scanDatePicker.IsEnabled = false;
                    descriptionTextbox.IsEnabled = false;

                    addButton.IsEnabled = false;
                    clearButton.IsEnabled = false;
                }
                else
                {
                    itemName.IsEnabled = true;
                    itemBrand.IsEnabled = true;
                    itemModel.IsEnabled = true;
                    itemCategory.IsEnabled = true;
                    itemWeight.IsEnabled = true;
                    scanDatePicker.IsEnabled = true;
                    descriptionTextbox.IsEnabled = true;

                    addButton.IsEnabled = true;
                    clearButton.IsEnabled = true;
                }
            }
        }
    }
}
