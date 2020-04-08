using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazerScannerUWP
{
    class Item : Object
    {
        public string purchaseGroup { get; set;}
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

        
        public Item()
        {
            //No arg ctor
        }

        public Item(string purchaseGroup, long ean, string title, long upc, string description, string brand, string model, string weight, string category, int quantity, DateTime scandate, string imageurl)
        {
            this.purchaseGroup = purchaseGroup;
            this.ean = ean;
            this.title = title;
            this.upc = upc;
            this.description = description;
            this.brand = brand;
            this.model = model;
            this.weight = weight;
            this.category = category;
            this.quantity = quantity;
            this.scandate = scandate;
            this.imageurl = imageurl;
        }
    }
}
