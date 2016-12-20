using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;

namespace Imperatur.rest
{
    public class GenericRest
    {
        public string GetResultFromURL(string URL)
        {
            string json;

            using (WebClient wc = new WebClient())
            {
                json = wc.DownloadString(URL);

            }

            //http://finance.google.com/finance/info?client=ig&q=NASDAQ%3AAAPL,GOOG
            return json;
        }
    }
}
