using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
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
    public sealed partial class ViewItemPage : Page
    {

        private string SQL_DATA_CONNECTION = "Data Source=tcp:73.118.249.57;Initial Catalog=ItemDirectory;Persist Security Info=False;User ID=sa;Password=nothingtoseehere";

        public ViewItemPage()
        {
            this.InitializeComponent();
            GetItemList();
        }
        public void GetItemList()
        {
            string json = JSONFromServer();
            var resultObjects = AllChildren(JObject.Parse(json))
            .First(c => c.Type == JTokenType.Array && c.Path.Contains("Items"))
            .Children<JObject>();

            foreach (JObject result in resultObjects)
            {
                string purchaseGroup = (string)result.GetValue("purchaseGroup");
                long ean = (long)result.GetValue("ean");
                string title = (string)result.GetValue("title");
                long upc = (long)result.GetValue("upc");
                string description = (string)result.GetValue("description");
                string brand = (string)result.GetValue("brand");
                string model = (string)result.GetValue("model");
                string weight = (string)result.GetValue("weight");
                string category = (string)result.GetValue("category");
                int quantity = (int)result.GetValue("quantity");
                DateTime scandate = (DateTime)result.GetValue("scandate");
                string imageurl = (string)result.GetValue("imageurl");
                
                Item newItem = new Item(purchaseGroup,ean,title,upc,description,brand,model,weight,category,quantity,scandate,imageurl);
            }
        }
        private static IEnumerable<JToken> AllChildren(JToken json)
        {
            foreach (var c in json.Children())
            {
                yield return c;
                foreach (var cc in AllChildren(c))
                {
                    yield return cc;
                }
            }
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
                        return (string)oReader.GetValue(0);
                    }
                }
                myConnection.Close();
                return null;//TODO CAN'T RETURN NULL.
            }
        }

        private void itemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
