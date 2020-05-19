using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazerScannerUWP.Models
{
    public class StoredItem
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
}
