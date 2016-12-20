using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Imperatur_v2.rest
{
    public class Rest : IRestInterface
    {
        public string GetResultFromURL(string URL)
        {
            string json;
            using (WebClient wc = new WebClient())
            {
                json = wc.DownloadString(URL);
            }
            return json;
        }
    }
}
