using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WindowsMobile.Utils;

namespace WindowsMobile.model
{
    public class Product : ConvertJson
    {
        private string id; 
        private string barcode;
        private string image;
        private string name;
        private DateTime expirationDate;

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Barcode
        {
            get { return barcode; }
            set { barcode = value; }
        }

        public string Image
        {
            get { return image; }
            set { image = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public DateTime ExpirationDate
        {
            get { return expirationDate; }
            set { expirationDate = value; }
        } 

        public void checkBarCode() {
            if (this.Barcode == null || this.Barcode.Trim().CompareTo("") == 0) {
                throw new Exception("The barcode field is mandatory");
            }

            int nbrOfLetter = Regex.Matches(this.Barcode, @"[a-zA-Z]").Count;
            if (nbrOfLetter > 0 || this.Barcode.Length != 13) {
                throw new Exception("Not a valid barcode");
            }
        }

        public static void checkExpiration_date(string date)
        {
            if (date == null || date.Trim().CompareTo("") == 0)
            {
                throw new Exception("The expiration date field is required");
            }
        }

        public override Dictionary<string, string> getEquivalenceJson() {
            Dictionary<string, string> dct = new Dictionary<string, string>();
            dct.Add("id", "id");
            dct.Add("barcode", "id");
            dct.Add("image", "image_small_url");
            dct.Add("name", "generic_name");
            dct.Add("expirationDate", "expiration_date");

            return dct;
        }

    }
}
