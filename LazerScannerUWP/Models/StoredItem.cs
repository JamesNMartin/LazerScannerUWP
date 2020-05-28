using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazerScannerUWP.Models
{
    public class StoredItem
    {
        public long UPC { get; set; }


        public StoredItem(long theUPC)
        {
            UPC = theUPC;
        }


    }
}
