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
        private string SQL_DATA_CONNECTION = "Data Source=tcp:73.118.249.57;Initial Catalog=ItemDirectory;Persist Security Info=False;User ID=sa;Password=nothingtoseehere";


        public AddItemPage()
        {
            this.InitializeComponent();
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
        public bool CheckSQLServerForData(long ean)
        {
            using (SqlConnection myConnection = new SqlConnection(SQL_DATA_CONNECTION))
            {
                SqlCommand cmd = new SqlCommand("increaseQuan", myConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@upcInput", ean));
                myConnection.Open();
                int rowAffected = cmd.ExecuteNonQuery();
                if (rowAffected == 1)
                {
                    return true;
                }
                else
                {
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
            }
            else if (requestResponse.code == "TOO_FAST")
            {
                Console.WriteLine("##############################################");
                Console.WriteLine("\nPlease wait a few minutes before scanning...\n");
                Console.WriteLine("##############################################");
            }
            else if (requestResponse.code == "OK" && requestResponse.total == 0)
            {
                Console.WriteLine("Not found on UPCItemDB. Set item aside, you'll have to manually enter data later.\n");
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
                string imageURL = requestResponse.items[0].images[0];

                // FORMATTING ALL THE TEXT TO HAVE DOUBLE SINGLE QUOTE SO SQL QUERY IS NOT ENDED IN THE WRONG PLACE
                string formatterPurchaseGroup = SQLArguementFormatter(recieptbarcodeInput.Text);

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

                requestsRemaining.Text = "Number of scans remaining: " + XRateLimitRemaining.ToString();


                DateTime scannedDate = DateTime.Today;
                int quanNeeded = 1;
                using (SqlConnection myConnection = new SqlConnection(SQL_DATA_CONNECTION))
                {
                    string oString = $"INSERT INTO Items(purchaseGroup,ean,title,upc,description,brand,model,weight,category,quantity,scandate,imageurl)VALUES('{formatterPurchaseGroup}','{webEan}','{formattedTitle}','{webUpc}','{formattedDesc}','{formattedBrand}','{formattedModel}','{formattedWeight}','{formattedCategory}','{quanNeeded}','{scannedDate}','{imageURL}')";
                    SqlCommand oCmd = new SqlCommand(oString, myConnection);
                    myConnection.Open();
                    using (SqlDataReader oReader = oCmd.ExecuteReader())
                    {
                        myConnection.Close();
                    }
                    Console.WriteLine("Item added to SQL server");
                }
            }
        }
        public string SQLArguementFormatter(string input)
        {
            return input.Replace("'", "''");
        }
        public void PrintJSONData()
        {
            Console.WriteLine(JObject.Parse(JSONFromServer()));
        }
        private static StreamReader ExtractData(Stream data)
        {
            return new StreamReader(data);
        }
        public string JSONFromServer()
        {
            using (SqlConnection myConnection = new SqlConnection(SQL_DATA_CONNECTION))
            {
                string oString = "SELECT (select * from Items FOR JSON PATH, ROOT('Items'))";
                SqlCommand oCmd = new SqlCommand(oString, myConnection);
                myConnection.Open();
                using (SqlDataReader oReader = oCmd.ExecuteReader())
                {
                    while (oReader.Read())
                    {
                        return (dynamic)oReader.GetValue(0);
                    }
                }
                myConnection.Close();
                return null;//TODO CAN'T RETURN NULL.
            }
        }
        private void lookupButton_Click(object sender, RoutedEventArgs e)
        {
            long input = long.Parse(barcodeInput.Text);
            bool SQLItemCheck = CheckSQLServerForData(input);
            if (SQLItemCheck == true)
            {
                //Console.WriteLine("Found data on SQL server. Increased quantity by +1\n");
            }
            else
            {
                //Console.WriteLine("Not found on the SQL server. Checking UPCItemDB...");
                IsWebRequestValid(WebLookup(input));
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
            descriptionTextbox.Text = JSONFromServer();
        }
        private void ClearConfirmation_Click(object sender, RoutedEventArgs e)
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
