using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazerScannerUWP;
using System.Collections.ObjectModel;

namespace LazerScannerUWP.Models
{
    public class Item
    {
        public string userId { get; set; }
        public string purchaseGroup { get; set; }
        public long ean { get; set; }
        public string title { get; set; }
        public long upc { get; set; }
        public string description { get; set; }
        public string brand { get; set; }
        public string model { get; set; }
        public string weight { get; set; }
        public string category { get; set; }
        public int quantity { get; set; }
        public DateTime scandate { get; set; }
        public string imageurl { get; set; }
    }

    public class ItemManager
    {

        public static List<Item> GetItemList(string uid)
        {
            var items = new List<Item>();

            string json = JSONFromServer(uid);
            var resultObjects = AllChildren(JObject.Parse(json))
            .First(c => c.Type == JTokenType.Array && c.Path.Contains("Items"))
            .Children<JObject>();

            foreach (JObject result in resultObjects)
            {
                //FOR EACH RESULT, GRAB ALL THE DATA
                string theUserId = (string)result.GetValue("userId");
                string thePurchaseGroup = (string)result.GetValue("purchaseGroup");
                long theEan = (long)result.GetValue("ean");
                string theTitle = (string)result.GetValue("title");
                long theUpc = (long)result.GetValue("upc");
                string theDescription = (string)result.GetValue("description");
                string theBrand = (string)result.GetValue("brand");
                string theModel = (string)result.GetValue("model");
                string theWeight = (string)result.GetValue("weight");
                string theCategory = (string)result.GetValue("category");
                int theQuantity = (int)result.GetValue("quantity");
                DateTime theScandate = (DateTime)result.GetValue("scandate");
                string theImageurl = (string)result.GetValue("imageurl");

                //ONCE ALL THE VALUES ARE COLLECTED, ADD THEM TO A ITEM MODEL
                items.Add(new Item()
                {
                    userId = theUserId,
                    purchaseGroup = thePurchaseGroup,
                    ean = theEan,
                    title = theTitle,
                    upc = theUpc,
                    description = theDescription,
                    brand = theBrand,
                    model = theModel,
                    weight = theWeight,
                    category = theCategory,
                    quantity = theQuantity,
                    scandate = theScandate,
                    imageurl = theImageurl
                });
            }
            return items;
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
        public static string JSONFromServer(string uid)
        {
            string SQL_DATA_CONNECTION = "Data Source=tcp:73.118.249.57;Initial Catalog=LazerScanner;Persist Security Info=False;User ID=sa;Password=nothingtoseehere";

            using (SqlConnection myConnection = new SqlConnection(SQL_DATA_CONNECTION))
            {
                string oString = $"SELECT (select * from Items WHERE userId = '{uid}' FOR JSON PATH, ROOT('Items'))";
                SqlCommand oCmd = new SqlCommand(oString, myConnection);
                myConnection.Open();
                using (SqlDataReader oReader = oCmd.ExecuteReader())
                {
                    while (oReader.Read())
                    {
                        return (string)oReader.GetValue(0);
                    }
                }
                return null;
            }
        }

    }

}
