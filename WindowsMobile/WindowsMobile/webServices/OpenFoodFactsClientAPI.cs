using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using WindowsMobile.model;
using Newtonsoft.Json.Linq;
using System.Reflection;
using WindowsMobile.Utils;
using System.Globalization;

namespace WindowsMobile.webServices
{
    public enum httpVerb {
           GET,
           POST,
           PUT,
           DELETE
    }

    public class OpenFoodFactsClientAPI 
    {
        string certificatePath = @"C:\Users\ACER\Desktop\certificat\certificat_mobile_public.pem";

        string statusCode = "";
        string certificatePassword = "fandry";
        

        public string endPoint;
        public httpVerb httpMethod;

        public httpVerb HttpMethod
        {
            get { return httpMethod; }
            set { httpMethod = value; }
        }
        public string EndPoint
        {
            get { return endPoint; }


            set { endPoint = value; }
        }

        public OpenFoodFactsClientAPI() { 
            endPoint = string.Empty;
            httpMethod = httpVerb.GET;
            System.Net.ServicePointManager.CertificatePolicy = new TrustAllCertificatePolicy();
        }

        public string makeRequest() {
            string strResponseValue = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endPoint);
            request.Method = httpMethod.ToString();
            request.UserAgent = "WindowsMobile - Windows ce - version 1.0";

 
          /*  X509Certificate2 certificate = new X509Certificate2(certificatePath, certificatePassword, X509KeyStorageFlags.Exportable);

            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser); // we also tried LocalMachine to no avail
            store.Open(OpenFlags.ReadWrite);
            store.Add(certificate);
            store.Close();

            request.PreAuthenticate = true;
            request.ClientCertificates.Add(certificate); */
            
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                if (response.StatusCode != HttpStatusCode.OK) {
                    throw new ApplicationException("Error code " + response.StatusCode.ToString());
                }

                using (Stream responseStream = response.GetResponseStream()) {
                    if (responseStream != null) {
                        using (StreamReader reader = new StreamReader(responseStream)) {
                            strResponseValue = reader.ReadToEnd();
                        }
                    }
                }
            }
            return strResponseValue; 
        }

        public Product GetProductByCode(string barcode)
        {
            //The proxy we created
            this.endPoint = "http://vps497332.ovh.net:3000/api/v0/product/" + barcode + ".json";
            string output = this.makeRequest();
            Product prod = new Product();
            Dictionary<string, string> dct = prod.getEquivalenceJson();
            FieldInfo[] fields = Tools.GetAllFields(prod.GetType(), null);

            JObject jo = JObject.Parse(output);
            JToken product = jo["product"];
            if (product == null) {
                throw new Exception("The product corresponding to this barcode does not exist");
            }
            
            for (int i = 0; i < fields.Length; i++) {
                string jsonConvert = dct[fields[i].Name].ToString();
                string prodProperty = jo["product"][jsonConvert].Value<string>();
                if (prodProperty == null || prodProperty.ToString().Trim().CompareTo("") == 0) {
                    
                    if (fields[i].FieldType.Equals(typeof(DateTime))) {
                        fields[i].SetValue(prod, DateTime.Today.Date);
                    }
                    if (fields[i].FieldType.Equals(typeof(string))) {
                        fields[i].SetValue(prod, "No value");
                    }
                    continue;
                }

                PropertyInfo pi = prod.GetType().GetProperty(Tools.firstUpperCase(fields[i].Name));
                if (pi == null) {
                    if (fields[i].FieldType.Equals(typeof(DateTime)))
                    {
                        DateTime realED = Tools.changeDateTimeFormat(prodProperty); 
                        fields[i].SetValue(prod,realED);
                    }
                    continue;
                } 
                Tools.convertType(pi, prod, prodProperty);
            } 

            return prod;
        }
    }
}
